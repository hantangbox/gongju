#region

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

#endregion

namespace Orianna
{
    internal class Orianna
    {
        private static readonly Spell Q;
        private static readonly Spell W;
        private static readonly Spell E;
        private static readonly Spell R;
        private static readonly SpellSlot IgniteSlot;

        private static readonly List<Spell> SpellList = new List<Spell>();

        private static Orbwalking.Orbwalker Orbwalker;
        private static readonly Menu Config;

        static Orianna()
        {
            Q = new Spell(SpellSlot.Q, 825f);
            W = new Spell(SpellSlot.W, 255f); // Use the range attr instead of the width one because the ball is fixed
            E = new Spell(SpellSlot.E, 1095f);
            R = new Spell(SpellSlot.R, 410f);

            Q.SetSkillshot(0f, 80f, 1200f, false, SkillshotType.SkillshotLine);
            W.SetSkillshot(0.25f, 0f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            E.SetSkillshot(0.25f, 80f, 1700f, false, SkillshotType.SkillshotLine);
            R.SetSkillshot(0.6f, 0f, float.MaxValue, false, SkillshotType.SkillshotCircle);

            SpellList.Add(Q);
            SpellList.Add(W);
            SpellList.Add(E);
            SpellList.Add(R);

            IgniteSlot = ObjectManager.Player.GetSpellSlot("SummonerDot");

            //Create the menu
            Config = new Menu("發條#", "Orianna", true);

            //Orbwalker submenu
            Config.AddSubMenu(new Menu("走砍", "Orbwalking"));

            //Add the target selector to the menu as submenu.
            var targetSelectorMenu = new Menu("目標 選擇", "Target Selector");
            SimpleTs.AddToMenu(targetSelectorMenu);
            Config.AddSubMenu(targetSelectorMenu);

            //Load the orbwalker and add it to the menu as submenu.
            Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalking"));

            //Combo menu:
            Config.AddSubMenu(new Menu("連招", "Combo"));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseQCombo", "使用 Q").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseWCombo", "使用 W").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseECombo", "使用 E").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseRCombo", "使用 R").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseRComboKillable", "使用R擊殺").SetValue(true));
            Config.SubMenu("Combo")
                .AddItem(new MenuItem("UseRComboCount", "R使用人數").SetValue(new Slider(2, 1, 5)));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseIgniteCombo", "使用 點燃").SetValue(true));
            Config.SubMenu("Combo")
                .AddItem(
                    new MenuItem("ComboActive", "連招!").SetValue(
                        new KeyBind(Config.Item("Orbwalk").GetValue<KeyBind>().Key, KeyBindType.Press)));

            //Harass menu:
            Config.AddSubMenu(new Menu("騷擾", "Harass"));
            Config.SubMenu("Harass").AddItem(new MenuItem("UseQHarass", "使用 Q").SetValue(true));
            Config.SubMenu("Harass").AddItem(new MenuItem("UseWHarass", "使用 W").SetValue(true));
            Config.SubMenu("Harass").AddItem(new MenuItem("UseEHarass", "使用 E").SetValue(false));
            Config.SubMenu("Harass")
                .AddItem(
                    new MenuItem("HarassActive", "騷擾!").SetValue(
                        new KeyBind(Config.Item("Farm").GetValue<KeyBind>().Key, KeyBindType.Press)));
            Config.SubMenu("Harass")
                .AddItem(
                    new MenuItem("HarassActiveT", "使用 (toggle)!").SetValue(
                        new KeyBind("Y".ToCharArray()[0], KeyBindType.Toggle)));

            //Farming menu:
            Config.AddSubMenu(new Menu("清線", "Farm"));
            Config.SubMenu("Farm")
                .AddItem(
                    new MenuItem("UseQFarm", "使用 Q").SetValue(
                        new StringList(new[] { "控線", "清線", "同時", "禁用" }, 2)));
            Config.SubMenu("Farm")
                .AddItem(
                    new MenuItem("UseWFarm", "使用 W").SetValue(
                        new StringList(new[] { "控線", "清線", "同時", "禁用" }, 1)));
            Config.SubMenu("Farm")
                .AddItem(
                    new MenuItem("FreezeActive", "控線!").SetValue(
                        new KeyBind(Config.Item("Farm").GetValue<KeyBind>().Key, KeyBindType.Press)));
            Config.SubMenu("Farm")
                .AddItem(
                    new MenuItem("LaneClearActive", "清線!").SetValue(
                        new KeyBind(Config.Item("LaneClear").GetValue<KeyBind>().Key, KeyBindType.Press)));

            //JungleFarm menu:
            Config.AddSubMenu(new Menu("清野", "JungleFarm"));
            Config.SubMenu("JungleFarm").AddItem(new MenuItem("UseQJFarm", "使用 Q").SetValue(true));
            Config.SubMenu("JungleFarm").AddItem(new MenuItem("UseWJFarm", "使用 W").SetValue(true));
            Config.SubMenu("JungleFarm")
                .AddItem(
                    new MenuItem("JungleFarmActive", "清野!").SetValue(
                        new KeyBind(Config.Item("LaneClear").GetValue<KeyBind>().Key, KeyBindType.Press)));

            //Damage after combo:
            var dmgAfterComboItem = new MenuItem("DamageAfterCombo", "顯示組合連招傷害").SetValue(true);
            Utility.HpBarDamageIndicator.DamageToUnit +=
                hero =>
                    (float)
                        (ObjectManager.Player.GetSpellDamage(hero, SpellSlot.Q) +
                         ObjectManager.Player.GetSpellDamage(hero, SpellSlot.W) +
                         ObjectManager.Player.GetSpellDamage(hero, SpellSlot.E) +
                         ObjectManager.Player.GetSpellDamage(hero, SpellSlot.R) +
                         ObjectManager.Player.GetAutoAttackDamage(hero) * 3);
            Utility.HpBarDamageIndicator.Enabled = dmgAfterComboItem.GetValue<bool>();
            dmgAfterComboItem.ValueChanged +=
                delegate(object sender, OnValueChangeEventArgs eventArgs)
                {
                    Utility.HpBarDamageIndicator.Enabled = eventArgs.GetNewValue<bool>();
                };

            Config.AddSubMenu(new Menu("範圍", "Drawings"));
            Config.SubMenu("Drawings")
                .AddItem(
                    new MenuItem("QRange", "Q 範圍").SetValue(new Circle(true, Color.FromArgb(255, 255, 255, 255))));
            Config.SubMenu("Drawings")
                .AddItem(
                    new MenuItem("WRange", "W 範圍").SetValue(new Circle(true, Color.FromArgb(255, 255, 255, 255))));
            Config.SubMenu("Drawings")
                .AddItem(
                    new MenuItem("ERange", "E 範圍").SetValue(new Circle(false, Color.FromArgb(255, 255, 255, 255))));
            Config.SubMenu("Drawings")
                .AddItem(
                    new MenuItem("RRange", "R 範圍").SetValue(new Circle(false, Color.FromArgb(255, 255, 255, 255))));
            Config.SubMenu("Drawings").AddItem(dmgAfterComboItem);

            Config.AddSubMenu(new Menu("雜項", "Mixed"));
            Config.SubMenu("Mixed").AddItem(new MenuItem("UseRInterrupt", "使用R中斷法術").SetValue(true));

            Config.AddToMainMenu();

            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnGameSendPacket += Game_OnGameSendPacket;
            Game.OnGameUpdate += Game_OnGameUpdate;
            Interrupter.OnPossibleToInterrupt += Interrupter_OnPossibleToInterrupt;
            Orbwalking.OnNonKillableMinion += Orbwalking_OnNonKillableMinion;
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            foreach (var spell in SpellList)
            {
                var menuItem = Config.Item(spell.Slot + "Range").GetValue<Circle>();
                var position = ((spell.Slot == SpellSlot.W || spell.Slot == SpellSlot.R)
                    ? BallManager.CurrentBallPositionDraw
                    : ObjectManager.Player.Position);

                if (menuItem.Active)
                {
                    Utility.DrawCircle(position, spell.Range, menuItem.Color);
                }
            }
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            Q.UpdateSourcePosition(BallManager.CurrentBallPosition, ObjectManager.Player.ServerPosition);
            W.UpdateSourcePosition(BallManager.CurrentBallPosition, BallManager.CurrentBallPosition);
            E.UpdateSourcePosition(BallManager.CurrentBallPosition, ObjectManager.Player.ServerPosition);
            R.UpdateSourcePosition(BallManager.CurrentBallPosition, BallManager.CurrentBallPosition);

            var combo = Config.Item("ComboActive").GetValue<KeyBind>().Active;
            if (combo || Config.Item("HarassActive").GetValue<KeyBind>().Active ||
                Config.Item("HarassActiveT").GetValue<KeyBind>().Active)
            {
                CastSpells(combo);
            }

            var lc = Config.Item("LaneClearActive").GetValue<KeyBind>().Active;
            if (lc || Config.Item("FreezeActive").GetValue<KeyBind>().Active)
            {
                Farm(lc);
            }

            if (Config.Item("JungleFarmActive").GetValue<KeyBind>().Active)
            {
                JungleFarm();
            }
        }

        private static void Game_OnGameSendPacket(GamePacketEventArgs args)
        {
            if (args.PacketData[0] != Packet.C2S.Cast.Header)
            {
                return;
            }

            var decodedPacket = Packet.C2S.Cast.Decoded(args.PacketData);
            if (decodedPacket.SourceNetworkId != ObjectManager.Player.NetworkId || decodedPacket.Slot != SpellSlot.R)
            {
                return;
            }
            ;

            if (Math.Abs(R.GetHitCount()) < float.Epsilon)
            {
                args.Process = false;
            }
        }

        // TODO: add w
        private static void Orbwalking_OnNonKillableMinion(Obj_AI_Base minion)
        {
            var useQi = Config.Item("UseQFarm").GetValue<StringList>().SelectedIndex;

            if (Config.Item("FreezeActive").GetValue<KeyBind>().Active && (useQi == 0 || useQi == 2) && Q.IsReady() &&
                minion.IsValidTarget(Q.Range + Q.Width) && Q.GetHealthPrediction(minion) > 0)
            {
                Q.Cast(minion);
            }
        }

        private static void Interrupter_OnPossibleToInterrupt(Obj_AI_Base unit, InterruptableSpell spell)
        {
            if (Config.Item("UseRInterrupt").GetValue<bool>() && R.IsReady() && unit.IsValidTarget() &&
                spell.DangerLevel == InterruptableDangerLevel.High &&
                R.GetPrediction(unit).UnitPosition.Distance(BallManager.CurrentBallPosition) <= R.Range)
            {
                R.Cast();
            }
        }

        //TODO: fix IsReady and add a precast + packets
        private static void CastSpellW(float hitCount = 1)
        {
            if (W.IsReady() &&
                ObjectManager.Get<Obj_AI_Hero>()
                    .Count(
                        hero =>
                            hero.IsValidTarget() &&
                            W.GetPrediction(hero).UnitPosition.Distance(BallManager.CurrentBallPosition) <= W.Range) >=
                hitCount)
            {
                W.Cast();
            }
        }

        private static void CastSpellE(Obj_AI_Base target, bool allValid = false)
        {
            if ((!target.IsValidTarget() && !allValid) || !E.IsReady())
            {
                return;
            }

            if (CheckHitE(target, ObjectManager.Player))
            {
                Packet.C2S.Cast.Encoded(new Packet.C2S.Cast.Struct(ObjectManager.Player.NetworkId, SpellSlot.E)).Send();
                return;
            }

            foreach (var alliedHero in
                ObjectManager.Get<Obj_AI_Hero>().Where(hero => hero.IsValidTarget(E.Range, false) && hero.IsAlly))
            {
                if (CheckHitE(target, alliedHero) ||
                    (allValid &&
                     ObjectManager.Get<Obj_AI_Hero>()
                         .Any(enemyHero => enemyHero.IsValidTarget() && CheckHitE(enemyHero, alliedHero))))
                {
                    Packet.C2S.Cast.Encoded(new Packet.C2S.Cast.Struct(alliedHero.NetworkId, SpellSlot.E)).Send();
                    return;
                }
            }
        }

        private static void CastSpellR(float hitCount = 1)
        {
            if (R.IsReady() &&
                ObjectManager.Get<Obj_AI_Hero>()
                    .Count(
                        hero =>
                            hero.IsValidTarget() &&
                            R.GetPrediction(hero).UnitPosition.Distance(BallManager.CurrentBallPosition) <= R.Range) >=
                hitCount)
            {
                R.Cast();
            }
        }

        private static bool CheckHitE(Obj_AI_Base unit, Obj_AI_Base ally)
        {
            return E.WillHit(
                unit,
                Prediction.GetPrediction(
                    ally, ally.Distance(BallManager.CurrentBallPosition) / E.Speed - Game.Ping / 2.0f).CastPosition);
        }

        private static void CastSpells(bool combo)
        {
            var useQ = Config.Item("UseQ" + (combo ? "Combo" : "Harass")).GetValue<bool>();
            var useW = Config.Item("UseW" + (combo ? "Combo" : "Harass")).GetValue<bool>();
            var useE = Config.Item("UseE" + (combo ? "Combo" : "Harass")).GetValue<bool>();
            var useR = (combo && Config.Item("UseRCombo").GetValue<bool>());
            var useI = (combo && Config.Item("UseIgniteCombo").GetValue<bool>());

            var qTarget = SimpleTs.GetTarget(Q.Range + Q.Width, SimpleTs.DamageType.Magical);
            var eTarget = SimpleTs.GetTarget(E.Range, SimpleTs.DamageType.Magical);

            if (qTarget != null)
            {
                var dmg = ObjectManager.Player.GetAutoAttackDamage(qTarget) * 3;

                if (useQ && Q.IsReady())
                {
                    dmg += Q.GetDamage(qTarget);
                }

                if (useW && W.IsReady())
                {
                    dmg += W.GetDamage(qTarget);
                }

                if (useE && E.IsReady())
                {
                    dmg += E.GetDamage(qTarget);
                }

                if (useR && R.IsReady())
                {
                    dmg += R.GetDamage(qTarget);
                }

                if (useI && IgniteSlot != SpellSlot.Unknown &&
                    ObjectManager.Player.SummonerSpellbook.CanUseSpell(IgniteSlot) == SpellState.Ready)
                {
                    dmg += ObjectManager.Player.GetSummonerSpellDamage(qTarget, Damage.SummonerSpell.Ignite);
                }

                if (useQ)
                {
                    Q.Cast(qTarget, false, true);
                }

                if (useW)
                {
                    CastSpellW();
                }

                if (useR)
                {
                    CastSpellR(Config.Item("UseRComboCount").GetValue<Slider>().Value);

                    if (Config.Item("UseRComboKillable").GetValue<bool>() && dmg > qTarget.Health &&
                        R.GetPrediction(qTarget).UnitPosition.Distance(BallManager.CurrentBallPosition) <= R.Range)
                    {
                        R.Cast();
                    }
                }

                if (useI && IgniteSlot != SpellSlot.Unknown &&
                    ObjectManager.Player.SummonerSpellbook.CanUseSpell(IgniteSlot) == SpellState.Ready &&
                    dmg > qTarget.Health)
                {
                    ObjectManager.Player.SummonerSpellbook.CastSpell(IgniteSlot, qTarget);
                }
            }

            if (useE)
            {
                CastSpellE(eTarget, true);
            }
        }

        private static void Farm(bool laneClear)
        {
            var rangedMinionsQ = MinionManager.GetMinions(
                ObjectManager.Player.ServerPosition, Q.Range + Q.Width + 30, MinionTypes.Ranged);
            var allMinionsQ = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q.Range + Q.Width + 30);
            var allMinionsW = MinionManager.GetMinions(BallManager.CurrentBallPosition, W.Range + W.Width + 30);

            var useQi = Config.Item("UseQFarm").GetValue<StringList>().SelectedIndex;
            var useWi = Config.Item("UseWFarm").GetValue<StringList>().SelectedIndex;
            var useQ = (laneClear && (useQi == 1 || useQi == 2)) || (!laneClear && (useQi == 0 || useQi == 2));
            var useW = (laneClear && (useWi == 1 || useWi == 2)) || (!laneClear && (useWi == 0 || useWi == 2));

            if (useQ && Q.IsReady())
            {
                if (laneClear)
                {
                    var fl1 = Q.GetCircularFarmLocation(rangedMinionsQ, Q.Width);
                    var fl2 = Q.GetCircularFarmLocation(allMinionsQ, Q.Width);

                    if (fl1.MinionsHit >= 3)
                    {
                        Q.Cast(fl1.Position);
                    }
                    else if (fl2.MinionsHit >= 2 || allMinionsQ.Count == 1)
                    {
                        Q.Cast(fl2.Position);
                    }
                }
                else
                {
                    foreach (var minion in
                        allMinionsQ.Where(
                            minion =>
                                !Orbwalking.InAutoAttackRange(minion) && minion.Health < 0.75 * Q.GetDamage(minion)))
                    {
                        Q.Cast(minion);
                    }
                }
            }

            if (useW && W.IsReady())
            {
                if (laneClear)
                {
                    var i =
                        allMinionsW.Count(
                            minion =>
                                minion.IsValidTarget(W.Range + W.Width + 30, true, BallManager.CurrentBallPosition));

                    if (i >= 2)
                    {
                        W.Cast();
                    }
                }
                else
                {
                    // ReSharper disable once UnusedVariable
                    foreach (var minion in
                        allMinionsW.Where(
                            minion =>
                                !Orbwalking.InAutoAttackRange(minion) && minion.Health < 0.75 * W.GetDamage(minion)))
                    {
                        W.Cast();
                    }
                }
            }
        }

        private static void JungleFarm()
        {
            var useQ = Config.Item("UseQJFarm").GetValue<bool>();
            var useW = Config.Item("UseWJFarm").GetValue<bool>();

            var mobs = MinionManager.GetMinions(
                ObjectManager.Player.ServerPosition, W.Range, MinionTypes.All, MinionTeam.Neutral,
                MinionOrderTypes.MaxHealth);

            if (mobs.Count > 0)
            {
                var mob = mobs[0];

                if (useQ)
                {
                    Q.Cast(mob);
                }

                if (W.IsReady() && mob.Distance(BallManager.CurrentBallPosition) <= W.Range)
                {
                    W.Cast();
                }
            }
        }
    }
}
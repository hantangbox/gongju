#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

#endregion

namespace Xerath
{
    internal class Program
    {
        public const string ChampionName = "Xerath";

        //Orbwalker instance
        public static Orbwalking.Orbwalker Orbwalker;

        //Spells
        public static List<Spell> SpellList = new List<Spell>();

        public static Spell Q;
        public static Spell W;
        public static Spell E;
        public static Spell R;

        //Menu
        public static Menu Config;

        private static Obj_AI_Hero Player;

        private static Vector2 PingLocation;
        private static int LastPingT = 0;
        private static bool AttacksEnabled
        {
            get
            {
                if (IsCastingR)
                    return false;

                if (Q.IsCharging)
                    return false;

                if (Config.Item("ComboActive").GetValue<KeyBind>().Active)
                    return IsPassiveUp || (!Q.IsReady() && !W.IsReady() && !E.IsReady());

                return true;
            }
        }

        public static bool IsPassiveUp
        {
            get { return ObjectManager.Player.HasBuff("xerathascended2onhit", true); }
        }

        public static bool IsCastingR
        {
            get
            {
                return ObjectManager.Player.HasBuff("XerathLocusOfPower2", true) ||
                       (ObjectManager.Player.LastCastedSpellName() == "XerathLocusOfPower2" &&
                        Environment.TickCount - ObjectManager.Player.LastCastedSpellT() < 500);
            }
        }

        public static class RCharge
        {
            public static int CastT;
            public static int Index;
            public static Vector3 Position;
            public static bool TapKeyPressed;
        }

        private static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        private static void Game_OnGameLoad(EventArgs args)
        {
            Player = ObjectManager.Player;

            if (Player.ChampionName != ChampionName) return;

            //Create the spells
            Q = new Spell(SpellSlot.Q, 1550);
            W = new Spell(SpellSlot.W, 1000);
            E = new Spell(SpellSlot.E, 1150);
            R = new Spell(SpellSlot.R, 675);

            Q.SetSkillshot(0.6f, 100f, float.MaxValue, false, SkillshotType.SkillshotLine);
            W.SetSkillshot(0.7f, 125f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            E.SetSkillshot(0.25f, 60f, 1400f, true, SkillshotType.SkillshotLine);
            R.SetSkillshot(0.7f, 120f, float.MaxValue, false, SkillshotType.SkillshotCircle);

            Q.SetCharged("XerathArcanopulseChargeUp", "XerathArcanopulseChargeUp", 750, 1550, 1.5f);

            SpellList.Add(Q);
            SpellList.Add(W);
            SpellList.Add(E);
            SpellList.Add(R);

            //Create the menu
            Config = new Menu("澤拉斯", "Xerath", true);

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
            Config.SubMenu("Combo").AddItem(new MenuItem("UseQCombo", "自動 Q").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseWCombo", "自動 W").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseECombo", "自動 E").SetValue(true));
            Config.SubMenu("Combo")
                .AddItem(
                    new MenuItem("ComboActive", "連招!").SetValue(
                        new KeyBind(Config.Item("Orbwalk").GetValue<KeyBind>().Key, KeyBindType.Press)));

            //Misc
            Config.AddSubMenu(new Menu("R 設置", "R"));
            Config.SubMenu("R").AddItem(new MenuItem("EnableRUsage", "自動使用R").SetValue(true));
            Config.SubMenu("R").AddItem(new MenuItem("rMode", "R模式").SetValue(new StringList(new[] { "正常", "自定義延遲", "優先"})));
            Config.SubMenu("R").AddItem(new MenuItem("rModeKey", "優先R 鍵位").SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press)));
            Config.SubMenu("R").AddSubMenu(new Menu("自定義延遲", "Custom delays"));
            for (int i = 1; i <= 3; i++)
                Config.SubMenu("R").SubMenu("Custom delays").AddItem(new MenuItem("Delay"+i, "Delay"+i).SetValue(new Slider(0, 1500, 0)));
            Config.SubMenu("R").AddItem(new MenuItem("PingRKillable", "擊殺提示 (本地)").SetValue(true));
            Config.SubMenu("R").AddItem(new MenuItem("BlockMovement", "右鍵停止R（再次點擊繼續）").SetValue(false));
            Config.SubMenu("R").AddItem(new MenuItem("OnlyNearMouse", "R鼠標附近敵人").SetValue(false));
            Config.SubMenu("R").AddItem(new MenuItem("MRadius", "R半徑範圍").SetValue(new Slider(700, 1500, 300)));

            //Harass menu:
            Config.AddSubMenu(new Menu("騷擾", "Harass"));
            Config.SubMenu("Harass").AddItem(new MenuItem("UseQHarass", "使用 Q").SetValue(true));
            Config.SubMenu("Harass").AddItem(new MenuItem("UseWHarass", "使用 W").SetValue(false));
            Config.SubMenu("Harass")
                .AddItem(
                    new MenuItem("HarassActive", "騷擾!").SetValue(
                        new KeyBind(Config.Item("Farm").GetValue<KeyBind>().Key, KeyBindType.Press)));
            Config.SubMenu("Harass")
                .AddItem(
                    new MenuItem("HarassActiveT", "騷擾 (自動)!").SetValue(new KeyBind("Y".ToCharArray()[0],
                        KeyBindType.Toggle)));

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

            //Misc
            Config.AddSubMenu(new Menu("雜項", "Misc"));
            Config.SubMenu("Misc").AddItem(new MenuItem("InterruptSpells", "自動E中斷法術").SetValue(true));
            Config.SubMenu("Misc").AddItem(new MenuItem("AutoEGC", "自動E阻止突進").SetValue(true));


            //Damage after combo:
            var dmgAfterComboItem = new MenuItem("DamageAfterR", "顯示3次R的總傷害").SetValue(true);
            Utility.HpBarDamageIndicator.DamageToUnit += hero => (float)Player.GetSpellDamage(hero, SpellSlot.R) * 3;
            Utility.HpBarDamageIndicator.Enabled = dmgAfterComboItem.GetValue<bool>();
            dmgAfterComboItem.ValueChanged += delegate(object sender, OnValueChangeEventArgs eventArgs)
            {
                Utility.HpBarDamageIndicator.Enabled = eventArgs.GetNewValue<bool>();
            };

            //Drawings menu:
            Config.AddSubMenu(new Menu("範圍", "Drawings"));
            Config.SubMenu("Drawings")
                .AddItem(
                    new MenuItem("QRange", "Q 範圍").SetValue(new Circle(true, Color.FromArgb(150, Color.DodgerBlue))));
            Config.SubMenu("Drawings")
                .AddItem(
                    new MenuItem("WRange", "W 範圍").SetValue(new Circle(true, Color.FromArgb(150, Color.DodgerBlue))));
            Config.SubMenu("Drawings")
                .AddItem(
                    new MenuItem("ERange", "E 範圍").SetValue(new Circle(false, Color.FromArgb(150, Color.DodgerBlue))));
            Config.SubMenu("Drawings")
                .AddItem(
                    new MenuItem("RRange", "R 範圍").SetValue(new Circle(false, Color.FromArgb(150, Color.DodgerBlue))));
            Config.SubMenu("Drawings")
                .AddItem(
                    new MenuItem("RRangeM", "R 範圍 (小地圖)").SetValue(new Circle(false,
                        Color.FromArgb(150, Color.DodgerBlue))));
            Config.SubMenu("Drawings")
                .AddItem(dmgAfterComboItem);
            Config.AddToMainMenu();

            //Add the events we are going to use:
            Game.OnGameUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Drawing.OnEndScene += Drawing_OnEndScene;
            Interrupter.OnPossibleToInterrupt += Interrupter_OnPossibleToInterrupt;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Obj_AI_Hero.OnProcessSpellCast += Obj_AI_Hero_OnProcessSpellCast;
            Game.OnWndProc += Game_OnWndProc;
            Game.PrintChat(ChampionName + " 鍔犺級鎴愬姛!婕㈠寲by浜岀嫍!QQ缇361630847!");
            Orbwalking.BeforeAttack += OrbwalkingOnBeforeAttack;
            Game.OnGameSendPacket += GameOnOnGameSendPacket;
        }

        private static void GameOnOnGameSendPacket(GamePacketEventArgs args)
        {
            if (args.PacketData[0] == Packet.C2S.Move.Header && IsCastingR && Config.Item("BlockMovement").GetValue<bool>())
            {
                args.Process = false;
            }
        }

        private static void OrbwalkingOnBeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            args.Process = AttacksEnabled;
        }

        static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (!Config.Item("AutoEGC").GetValue<bool>()) return;

            if (Player.Distance(gapcloser.Sender) < E.Range)
            {
                E.Cast(gapcloser.Sender);
            }
        }

        static void Game_OnWndProc(WndEventArgs args)
        {
            if (args.Msg == (uint)WindowsMessages.WM_KEYUP)
                RCharge.TapKeyPressed = true;
        }

        static void Obj_AI_Hero_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                if (args.SData.Name == "XerathLocusOfPower2")
                {
                    RCharge.CastT = 0;
                    RCharge.Index = 0;
                    RCharge.Position = new Vector3();
                    RCharge.TapKeyPressed = false;
                }
                else if (args.SData.Name == "xerathlocuspulse")
                {
                    RCharge.CastT = Environment.TickCount;
                    RCharge.Index++;
                    RCharge.Position = args.End;
                    RCharge.TapKeyPressed = false;
                }
            }
        }

        private static void Interrupter_OnPossibleToInterrupt(Obj_AI_Base unit, InterruptableSpell spell)
        {
            if (!Config.Item("InterruptSpells").GetValue<bool>()) return;

            if (Player.Distance(unit) < E.Range)
            {
                E.Cast(unit);
            }
        }

        private static void Combo()
        {

            UseSpells(Config.Item("UseQCombo").GetValue<bool>(), Config.Item("UseWCombo").GetValue<bool>(),
                Config.Item("UseECombo").GetValue<bool>());
        }

        private static void Harass()
        {
            UseSpells(Config.Item("UseQHarass").GetValue<bool>(), Config.Item("UseWHarass").GetValue<bool>(),
                false);
        }

        private static void UseSpells(bool useQ, bool useW, bool useE)
        {
            var qTarget = SimpleTs.GetTarget(Q.ChargedMaxRange, SimpleTs.DamageType.Magical);
            var wTarget = SimpleTs.GetTarget(W.Range + W.Width * 0.5f, SimpleTs.DamageType.Magical);
            var eTarget = SimpleTs.GetTarget(E.Range, SimpleTs.DamageType.Magical);

            if (eTarget != null && useE && E.IsReady())
            {
                if (Player.Distance(eTarget) < E.Range * 0.4f)
                    E.Cast(eTarget);
                else if ((!useW || !W.IsReady()))
                    E.Cast(eTarget);
            }

            if (useQ && Q.IsReady())
            {
                if (Q.IsCharging)
                {
                    Q.Cast(qTarget, false, false);
                }
                else if (qTarget != null && (!useW || !W.IsReady() || Player.Distance(qTarget) > W.Range))
                {
                    Q.StartCharging();
                }
            }

            if (wTarget != null && useW && W.IsReady())
                W.Cast(wTarget, false, true);
        }

        private static Obj_AI_Hero GetTargetNearMouse(float distance)
        {
            Obj_AI_Hero bestTarget = null;
            var bestRatio = 0f;

            if (SimpleTs.SelectedTarget.IsValidTarget() && !SimpleTs.IsInvulnerable(SimpleTs.SelectedTarget) &&
                (Game.CursorPos.Distance(SimpleTs.SelectedTarget.ServerPosition) < distance && ObjectManager.Player.Distance(SimpleTs.SelectedTarget) < R.Range))
            {
                return SimpleTs.SelectedTarget;
            }

            foreach (var hero in ObjectManager.Get<Obj_AI_Hero>())
            {
                if (!hero.IsValidTarget(R.Range) || SimpleTs.IsInvulnerable(hero) || Game.CursorPos.Distance(hero.ServerPosition) > distance)
                {
                    continue;
                }

                var damage = (float)ObjectManager.Player.CalcDamage(hero, Damage.DamageType.Magical, 100);
                var ratio = damage / (1 + hero.Health) * SimpleTs.GetPriority(hero);

                if (ratio > bestRatio)
                {
                    bestRatio = ratio;
                    bestTarget = hero;
                }
            }

            return bestTarget;
        }

        private static void WhileCastingR()
        {
            if (!Config.Item("EnableRUsage").GetValue<bool>()) return;
            var rMode = Config.Item("rMode").GetValue<StringList>().SelectedIndex;

            var rTarget = Config.Item("OnlyNearMouse").GetValue<bool>() ? GetTargetNearMouse(Config.Item("MRadius").GetValue<Slider>().Value) : SimpleTs.GetTarget(R.Range, SimpleTs.DamageType.Magical);

            if (rTarget != null)
            {
                //Wait at least 0.6f if the target is going to die or if the target is to far away
                if(rTarget.Health - R.GetDamage(rTarget) < 0)
                    if (Environment.TickCount - RCharge.CastT <= 700) return;

                if ((RCharge.Index != 0 && rTarget.Distance(RCharge.Position) > 1000))
                    if (Environment.TickCount - RCharge.CastT <= Math.Min(2500, rTarget.Distance(RCharge.Position) - 1000)) return;

                switch (rMode)
                {
                    case 0://Normal
                        R.Cast(rTarget, true);
                        break;

                    case 1://Selected delays.
                        var delay = Config.Item("Delay" + (RCharge.Index + 1)).GetValue<Slider>().Value;
                        if (Environment.TickCount - RCharge.CastT > delay)
                            R.Cast(rTarget, true);
                        break;

                    case 2://On tap
                        if (RCharge.TapKeyPressed)
                            R.Cast(rTarget, true);
                        break;
                }
            }
        }

        private static void Farm(bool laneClear)
        {
            var allMinionsQ = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q.ChargedMaxRange,
                MinionTypes.All);
            var rangedMinionsW = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, W.Range + W.Width + 30,
                MinionTypes.Ranged);

            var useQi = Config.Item("UseQFarm").GetValue<StringList>().SelectedIndex;
            var useWi = Config.Item("UseWFarm").GetValue<StringList>().SelectedIndex;
            var useQ = (laneClear && (useQi == 1 || useQi == 2)) || (!laneClear && (useQi == 0 || useQi == 2));
            var useW = (laneClear && (useWi == 1 || useWi == 2)) || (!laneClear && (useWi == 0 || useWi == 2));

            if (useW && W.IsReady())
            {
                var locW = W.GetCircularFarmLocation(rangedMinionsW, W.Width * 0.75f);
                if (locW.MinionsHit >= 3 && W.InRange(locW.Position.To3D()))
                {
                    W.Cast(locW.Position);
                    return;
                }
                else
                {
                    var locW2 = W.GetCircularFarmLocation(allMinionsQ, W.Width * 0.75f);
                    if (locW2.MinionsHit >= 1 && W.InRange(locW.Position.To3D()))
                    {
                        W.Cast(locW.Position);
                        return;
                    }
                        
                }
            }

            if (useQ && Q.IsReady())
            {
                if (Q.IsCharging)
                {
                    var locQ = Q.GetLineFarmLocation(allMinionsQ);
                    if (allMinionsQ.Count == allMinionsQ.Count(m => Player.Distance(m) < Q.Range) && locQ.MinionsHit > 0 && locQ.Position.IsValid())
                        Q.Cast(locQ.Position);
                }
                else if (allMinionsQ.Count > 0)
                    Q.StartCharging();
            }
        }

        private static void JungleFarm()
        {
            var useQ = Config.Item("UseQJFarm").GetValue<bool>();
            var useW = Config.Item("UseWJFarm").GetValue<bool>();
            var mobs = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, W.Range, MinionTypes.All,
                MinionTeam.Neutral, MinionOrderTypes.MaxHealth);

            if (mobs.Count > 0)
            {
                var mob = mobs[0];
                if (useW && W.IsReady())
                {
                    W.Cast(mob);
                }
                else if (useQ && Q.IsReady())
                {
                    if (!Q.IsCharging)
                        Q.StartCharging();
                    else
                        Q.Cast(mob);
                }
            }
        }

        private static void Ping(Vector2 position)
        {
            if (Environment.TickCount - LastPingT < 30 * 1000) return;
            LastPingT = Environment.TickCount;
            PingLocation = position;
            SimplePing();
            Utility.DelayAction.Add(150, SimplePing);
            Utility.DelayAction.Add(300, SimplePing);
            Utility.DelayAction.Add(400, SimplePing);
            Utility.DelayAction.Add(800, SimplePing);
        }

        private static void SimplePing()
        {
            Packet.S2C.Ping.Encoded(new Packet.S2C.Ping.Struct(PingLocation.X, PingLocation.Y, 0, 0, Packet.PingType.Fallback)).Process();
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (Player.IsDead) return;

            Orbwalker.SetMovement(true);

            //Update the R range
            R.Range = 2000 + R.Level * 1200;

            if (IsCastingR)
            {
                Orbwalker.SetMovement(false);
                WhileCastingR();
                return;
            }

            if (R.IsReady() && Config.Item("PingRKillable").GetValue<bool>())
            {
                foreach (var enemy in ObjectManager.Get<Obj_AI_Hero>().Where(h => h.IsValidTarget() && (float)Player.GetSpellDamage(h, SpellSlot.R) * 3 > h.Health))
                {
                    Ping(enemy.Position.To2D());
                }
            }

            if (Config.Item("ComboActive").GetValue<KeyBind>().Active)
            {
                Combo();
            }
            else
            {
                if (Config.Item("HarassActive").GetValue<KeyBind>().Active ||
                    Config.Item("HarassActiveT").GetValue<KeyBind>().Active)
                    Harass();

                var lc = Config.Item("LaneClearActive").GetValue<KeyBind>().Active;
                if (lc || Config.Item("FreezeActive").GetValue<KeyBind>().Active)
                    Farm(lc);

                if (Config.Item("JungleFarmActive").GetValue<KeyBind>().Active)
                    JungleFarm();
            }
        }

        private static void Drawing_OnEndScene(EventArgs args)
        {
            if (R.Level == 0) return;
            var menuItem = Config.Item(R.Slot + "RangeM").GetValue<Circle>();
            if (menuItem.Active)
                Utility.DrawCircle(Player.Position, R.Range, menuItem.Color, 2, 30, true);
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (IsCastingR)
            {
                if (Config.Item("OnlyNearMouse").GetValue<bool>())
                {
                    Utility.DrawCircle(Game.CursorPos, Config.Item("MRadius").GetValue<Slider>().Value, Color.White);
                }
            }

            //Draw the ranges of the spells.
            foreach (var spell in SpellList)
            {
                var menuItem = Config.Item(spell.Slot + "Range").GetValue<Circle>();
                if (menuItem.Active && (spell.Slot != SpellSlot.R || R.Level > 0))
                    Utility.DrawCircle(Player.Position, spell.Range, menuItem.Color);
            }
        }
    }
}

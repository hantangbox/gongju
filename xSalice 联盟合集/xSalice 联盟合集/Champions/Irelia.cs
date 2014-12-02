using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

namespace xSaliceReligionAIO.Champions
{
    class Irelia : Champion
    {
        public Irelia()
        {
            SetSpells();
            LoadMenu();
        }
        private void SetSpells()
        {
            Q = new Spell(SpellSlot.Q, 650);

            W = new Spell(SpellSlot.W);

            E = new Spell(SpellSlot.E, 425);

            R = new Spell(SpellSlot.R, 1000);
            R.SetSkillshot(0, 80f, 1400f, false, SkillshotType.SkillshotLine);
        }

        private void LoadMenu()
        {
            //key
            var key = new Menu("Key", "Key");
            {
                key.AddItem(new MenuItem("ComboActive", "Combo!").SetValue(new KeyBind(32, KeyBindType.Press)));
                key.AddItem(new MenuItem("HarassActive", "Harass!").SetValue(new KeyBind("C".ToCharArray()[0], KeyBindType.Press)));
                key.AddItem(new MenuItem("LaneClearActive", "Farm!").SetValue(new KeyBind("V".ToCharArray()[0], KeyBindType.Press)));
                key.AddItem(new MenuItem("LastHitKey", "Last Hit!").SetValue(new KeyBind("A".ToCharArray()[0], KeyBindType.Press)));
                //add to menu
                menu.AddSubMenu(key);
            }

            var spellMenu = new Menu("SpellMenu", "SpellMenu");
            {
                var qMenu = new Menu("QMenu", "QMenu");
                {
                    qMenu.AddItem(new MenuItem("Q_Min_Distance", "Min range to Q").SetValue(new Slider(300, 0, 600)));
                    qMenu.AddItem(new MenuItem("Q_Gap_Close", "Q Minion to Gap Close").SetValue(true));
                    qMenu.AddItem(new MenuItem("Q_Under_Tower", "Q Enemy Under Tower").SetValue(false));
                    spellMenu.AddSubMenu(qMenu);
                }

                var eMenu = new Menu("EMenu", "EMenu");
                {
                    eMenu.AddItem(new MenuItem("E_Only_Stun", "Save E to Stun").SetValue(true));
                    eMenu.AddItem(new MenuItem("E_Running", "E On Running Enemy").SetValue(true));
                    spellMenu.AddSubMenu(eMenu);
                }

                var rMenu = new Menu("RMenu", "RMenu");
                {
                    rMenu.AddItem(new MenuItem("R_If_HP", "R If HP <=").SetValue(new Slider(20)));
                    //rMenu.AddItem(new MenuItem("R_Wait_Sheen", "Wait for Sheen").SetValue(false));

                    spellMenu.AddSubMenu(rMenu);
                }

                menu.AddSubMenu(spellMenu);
            }

            var combo = new Menu("Combo", "Combo");
            {
                combo.AddItem(new MenuItem("selected", "Focus Selected Target").SetValue(true));
                combo.AddItem(new MenuItem("UseQCombo", "Use Q").SetValue(true));
                combo.AddItem(new MenuItem("UseWCombo", "Use W").SetValue(true));
                combo.AddItem(new MenuItem("UseECombo", "Use E").SetValue(true));
                combo.AddItem(new MenuItem("UseRCombo", "Use R").SetValue(true));
                combo.AddItem(new MenuItem("Ignite", "Use Ignite").SetValue(true));
                combo.AddItem(new MenuItem("Botrk", "Use BOTRK/Bilge").SetValue(true));
                //add to menu
                menu.AddSubMenu(combo);
            }

            var harass = new Menu("Harass", "Harass");
            {
                harass.AddItem(new MenuItem("UseQHarass", "Use Q").SetValue(true));
                harass.AddItem(new MenuItem("UseWHarass", "Use W").SetValue(false));
                harass.AddItem(new MenuItem("UseEHarass", "Use E").SetValue(true));
                AddManaManagertoMenu(harass, "Harass", 30);
                //add to menu
                menu.AddSubMenu(harass);
            }

            var lastHit = new Menu("Lasthit", "Lasthit");
            {
                lastHit.AddItem(new MenuItem("UseQLastHit", "Use Q").SetValue(true));
                AddManaManagertoMenu(lastHit, "Lasthit", 30);
                //add to menu
                menu.AddSubMenu(lastHit);
            }

            var farm = new Menu("LaneClear", "LaneClear");
            {
                farm.AddItem(new MenuItem("UseQFarm", "Use Q").SetValue(true));
                farm.AddItem(new MenuItem("UseQFarm_Tower", "Do not Q under Tower").SetValue(true));
                farm.AddItem(new MenuItem("UseWFarm", "Use W").SetValue(true));
                farm.AddItem(new MenuItem("UseRFarm", "Use R").SetValue(true));
                AddManaManagertoMenu(farm, "LaneClear", 0);
                //add to menu
                menu.AddSubMenu(farm);
            }

            var miscMenu = new Menu("Misc", "Misc");
            {
                //miscMenu.AddItem(new MenuItem("Cast_EQ", "Cast EQ nearest target").SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press)));
                miscMenu.AddItem(new MenuItem("E_Gap_Closer", "Use E On Gap Closer").SetValue(true));
                miscMenu.AddItem(new MenuItem("QE_Interrupt", "Use Q/E to interrupt").SetValue(true));
                //add to menu
                menu.AddSubMenu(miscMenu);
            }

            var drawMenu = new Menu("Drawing", "Drawing");
            {
                drawMenu.AddItem(new MenuItem("Draw_Disabled", "Disable All").SetValue(false));
                drawMenu.AddItem(new MenuItem("Draw_Q", "Draw Q").SetValue(true));
                drawMenu.AddItem(new MenuItem("Draw_E", "Draw E").SetValue(true));
                drawMenu.AddItem(new MenuItem("Draw_R", "Draw R").SetValue(true));
                drawMenu.AddItem(new MenuItem("Draw_R_Killable", "Draw R Mark on Killable").SetValue(true));

                MenuItem drawComboDamageMenu = new MenuItem("Draw_ComboDamage", "Draw Combo Damage").SetValue(true);
                MenuItem drawFill = new MenuItem("Draw_Fill", "Draw Combo Damage Fill").SetValue(new Circle(true, Color.FromArgb(90, 255, 169, 4)));
                drawMenu.AddItem(drawComboDamageMenu);
                drawMenu.AddItem(drawFill);
                DamageIndicator.DamageToUnit = GetComboDamage;
                DamageIndicator.Enabled = drawComboDamageMenu.GetValue<bool>();
                DamageIndicator.Fill = drawFill.GetValue<Circle>().Active;
                DamageIndicator.FillColor = drawFill.GetValue<Circle>().Color;
                drawComboDamageMenu.ValueChanged +=
                    delegate(object sender, OnValueChangeEventArgs eventArgs)
                    {
                        DamageIndicator.Enabled = eventArgs.GetNewValue<bool>();
                    };
                drawFill.ValueChanged +=
                    delegate(object sender, OnValueChangeEventArgs eventArgs)
                    {
                        DamageIndicator.Fill = eventArgs.GetNewValue<Circle>().Active;
                        DamageIndicator.FillColor = eventArgs.GetNewValue<Circle>().Color;
                    };
                //add to menu
                menu.AddSubMenu(drawMenu);
            }
        }

        private float GetComboDamage(Obj_AI_Base target)
        {
            double comboDamage = 0;

            if (Q.IsReady())
                comboDamage += Player.GetSpellDamage(target, SpellSlot.Q);

            if (W.IsReady())
                comboDamage += Player.GetSpellDamage(target, SpellSlot.W) * 4;

            if (E.IsReady())
                comboDamage += Player.GetSpellDamage(target, SpellSlot.E);

            if (R.IsReady())
                comboDamage += Player.GetSpellDamage(target, SpellSlot.R) * 4;

            if (Items.CanUseItem(Bilge.Id))
                comboDamage += Player.GetItemDamage(target, Damage.DamageItems.Bilgewater);

            if (Items.CanUseItem(Botrk.Id))
                comboDamage += Player.GetItemDamage(target, Damage.DamageItems.Botrk);

            if (IgniteSlot != SpellSlot.Unknown && Player.SummonerSpellbook.CanUseSpell(IgniteSlot) == SpellState.Ready)
                comboDamage += Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);

            return (float)(comboDamage + Player.GetAutoAttackDamage(target) * 4);
        }

        private float GetComboDmgPercent(Obj_AI_Hero target)
        {
            double comboDamage = GetComboDamage(target);

            var predHp = target.Health - comboDamage;
            var predHpPercent = predHp / target.MaxHealth * 100;

            return (float)predHpPercent;
        }

        private void Combo()
        {
            UseSpells(menu.Item("UseQCombo").GetValue<bool>(), menu.Item("UseWCombo").GetValue<bool>(),
                menu.Item("UseECombo").GetValue<bool>(), menu.Item("UseRCombo").GetValue<bool>(), "Combo");
        }

        private void Harass()
        {
            UseSpells(menu.Item("UseQHarass").GetValue<bool>(), menu.Item("UseWHarass").GetValue<bool>(),
                menu.Item("UseEHarass").GetValue<bool>(), false, "Harass");
        }

        private void UseSpells(bool useQ, bool useW, bool useE, bool useR, string source)
        {
            if (source == "Harass" && !HasMana("Harass"))
                return;

            if(useQ)
                Cast_Q();
            if(useW)
                Cast_W();
            if (source == "Combo")
            {
                var qTarget = SimpleTs.GetTarget(Q.Range, SimpleTs.DamageType.Physical);
                if (qTarget != null)
                {
                    if (GetComboDamage(qTarget) >= qTarget.Health && Ignite_Ready() && menu.Item("Ignite").GetValue<bool>())
                        Use_Ignite(qTarget);

                    if (menu.Item("Botrk").GetValue<bool>())
                    {
                        if (GetComboDmgPercent(qTarget) < 5 &&!qTarget.HasBuffOfType(BuffType.Slow)) Use_Bilge(qTarget);

                        if (GetComboDmgPercent(qTarget) < 5 && !qTarget.HasBuffOfType(BuffType.Slow))
                            Use_Botrk(qTarget);
                    }
                }
            }
            if(useE)
                Cast_E();
            if(useR)
                Cast_R();
        }
        public void Lasthit()
        {
            if (menu.Item("UseQLastHit").GetValue<bool>() && HasMana("Lasthit"))
                Cast_Q_Last_Hit();
        }

        private void Farm()
        {
            if (!HasMana("LaneClear"))
                return;

            List<Obj_AI_Base> allMinionsW = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, W.Range,
                MinionTypes.All, MinionTeam.NotAlly);
            List<Obj_AI_Base> allMinionR = MinionManager.GetMinions(Player.ServerPosition, R.Range, MinionTypes.All,
                        MinionTeam.NotAlly);
            
            var useQ = menu.Item("UseQFarm").GetValue<bool>();
            var useW = menu.Item("UseWFarm").GetValue<bool>();
            var useR = menu.Item("UseRFarm").GetValue<bool>();

            if (useQ)
                Cast_Q_Last_Hit();

            if (useW && allMinionsW.Count > 0 && W.IsReady())
                W.Cast();

            var rPred = R.GetLineFarmLocation(allMinionR);
            if (useR && rPred.MinionsHit > 0 && R.IsReady())
                R.Cast(rPred.Position);
        }

        private void Cast_Q()
        {
            var target = SimpleTs.GetTarget(Q.Range * 2, SimpleTs.DamageType.Physical);

            if (GetTargetFocus(Q.Range) != null)
                target = GetTargetFocus(Q.Range);

            if (Q.IsReady() && target != null)
            {
                if (Q.IsKillable(target))
                    Q.Cast(target, packets());

                if (Player.GetSpellDamage(target, SpellSlot.Q) + Player.GetSpellDamage(target, SpellSlot.E) > target.Health)
                    Q.Cast(target);

                var minDistance = menu.Item("Q_Min_Distance").GetValue<Slider>().Value;

                if (!menu.Item("Q_Under_Tower").GetValue<bool>())
                    if (Utility.UnderTurret(target, true))
                        return;

                if (Player.Distance(target) > Q.Range / 2 && menu.Item("Q_Gap_Close").GetValue<bool>())
                {
                    var allMinionQ = MinionManager.GetMinions(Player.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.NotAlly);

                    Obj_AI_Base bestMinion = allMinionQ[0];

                    foreach (var minion in allMinionQ)
                    {
                        double dmg = 0;

                        dmg += Player.GetSpellDamage(minion, SpellSlot.Q);
                        if (W.IsReady() || Player.HasBuff("ireliahitenstylecharged", true))
                            dmg += Player.GetSpellDamage(minion, SpellSlot.W);

                        if (target.Distance(minion) < Q.Range && Player.Distance(minion) < Q.Range && target.Distance(minion) < target.Distance(Player) && dmg > minion.Health + 40)
                            if (target.Distance(minion) < target.Distance(bestMinion))
                                bestMinion = minion;
                    }

                    //check if can Q without activating
                    if (bestMinion != null)
                    {
                        if (target.Distance(bestMinion) < Q.Range && Player.Distance(bestMinion) < Q.Range)
                        {
                            var dmg2 = Player.GetSpellDamage(bestMinion, SpellSlot.Q);

                            if (dmg2 > bestMinion.Health + 40)
                            {
                                Q.Cast(bestMinion, packets());
                                return;
                            }

                            if (W.IsReady() || Player.HasBuff("ireliahitenstylecharged", true))
                                dmg2 += Player.GetSpellDamage(bestMinion, SpellSlot.W);

                            if (dmg2 > bestMinion.Health)
                            {
                                W.Cast(packets());
                                Q.Cast(bestMinion, packets());
                                return;
                            }
                        }
                    }
                }

                if (Player.Distance(target) > minDistance && Player.Distance(target) < Q.Range + target.BoundingRadius)
                {
                    Q.Cast(target, packets());
                }
            }
        }

        private void Cast_Q_Last_Hit()
        {
            var allMinionQ = MinionManager.GetMinions(Player.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.NotAlly);

            if (allMinionQ.Count > 0 && Q.IsReady())
            {

                foreach (var minion in allMinionQ)
                {
                    double dmg = Player.GetSpellDamage(minion, SpellSlot.Q);

                    if (Player.HasBuff("ireliahitenstylecharged", true))
                        dmg += Player.GetSpellDamage(minion, SpellSlot.W);


                    if (dmg > minion.Health + 35)
                    {
                        if (menu.Item("UseQFarm_Tower").GetValue<bool>())
                        {
                            if (!Utility.UnderTurret(minion, true))
                            {
                                Q.Cast(minion, packets());
                                return;
                            }
                        }
                        else
                            Q.Cast(minion, packets());
                    }
                }
            }
        }

        private void Cast_W()
        {
            var target = SimpleTs.GetTarget(200, SimpleTs.DamageType.Physical);

            if (GetTargetFocus(200) != null)
                target = GetTargetFocus(200);

            if (target != null && W.IsReady())
            {
                W.Cast(packets());
            }
        }

        private void Cast_E()
        {
            var target = SimpleTs.GetTarget(E.Range, SimpleTs.DamageType.Magical);

            if (GetTargetFocus(E.Range) != null)
                target = GetTargetFocus(E.Range);

            if (target != null && E.IsReady())
            {
                if (E.IsKillable(target))
                    E.Cast(target, packets());

                if (menu.Item("E_Only_Stun").GetValue<bool>())
                {
                    var targetHealthPercent = target.Health / target.MaxHealth * 100;

                    if (GetHealthPercent() < targetHealthPercent)
                    {
                        E.Cast(target, packets());
                        return;
                    }
                }

                if (menu.Item("E_Running").GetValue<bool>())
                {
                    var pred = Prediction.GetPrediction(target, 1f);

                    if (Player.Distance(target) < Player.Distance(pred.UnitPosition) && Player.Distance(target) > 200)
                        E.Cast(target, packets());
                }
            }
        }

        private void Cast_R()
        {
            var target = SimpleTs.GetTarget(Player.Spellbook.GetSpell(SpellSlot.R).ToggleState == 1 ? Q.Range : R.Range,
                SimpleTs.DamageType.Physical);

            var range = R.Range;
            if (GetTargetFocus(range) != null)
                target = GetTargetFocus(range);

            if (target != null && R.IsReady())
            {
                if (!Player.HasBuff("IreliaTranscendentBlades"))
                {
                    if (GetComboDmgPercent(target) < 25)
                        R.Cast(target, packets());

                    var rHpValue = menu.Item("R_If_HP").GetValue<Slider>().Value;
                    if (GetHealthPercent() <= rHpValue)
                        R.Cast(target, packets());
                }
                else if (Player.HasBuff("IreliaTranscendentBlades"))
                {
                    R.Cast(target, packets());
                }
            }
        }

        public override void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (!menu.Item("E_Gap_Closer").GetValue<bool>()) return;

            if (E.IsReady() && gapcloser.Sender.IsValidTarget(E.Range))
                E.Cast(gapcloser.Sender, packets());
        }

        public override void Interrupter_OnPosibleToInterrupt(Obj_AI_Base unit, InterruptableSpell spell)
        {
            if (spell.DangerLevel < InterruptableDangerLevel.Medium || unit.IsAlly)
                return;

            if (menu.Item("QE_Interrupt").GetValue<bool>())
            {
                var enemyHp = unit.Health / unit.MaxHealth * 100;
                if (GetHealthPercent() > enemyHp)
                    return;

                if (unit.IsValidTarget(E.Range))
                    E.Cast(unit, packets());

                if (unit.IsValidTarget(Q.Range))
                {
                    Q.Cast(unit, packets());
                    E.Cast(unit, packets());
                }
            }
        }

        public override void Game_OnGameUpdate(EventArgs args)
        {
            //check if player is dead
            if (Player.IsDead) return;

            if (menu.Item("ComboActive").GetValue<KeyBind>().Active)
            {
                Combo();
            }
            else
            {
                if (menu.Item("LaneClearActive").GetValue<KeyBind>().Active)
                    Farm();

                if (menu.Item("LastHitKey").GetValue<KeyBind>().Active)
                    Lasthit();

                if (menu.Item("HarassActive").GetValue<KeyBind>().Active)
                    Harass();
            }
        }

        public override void Drawing_OnDraw(EventArgs args)
        {
            if (menu.Item("Draw_Disabled").GetValue<bool>())
                return;

            if (menu.Item("Draw_Q").GetValue<bool>())
                if (Q.Level > 0)
                    Utility.DrawCircle(Player.Position, Q.Range, Q.IsReady() ? Color.Green : Color.Red);

            if (menu.Item("Draw_E").GetValue<bool>())
                if (E.Level > 0)
                    Utility.DrawCircle(Player.Position, E.Range, E.IsReady() ? Color.Green : Color.Red);

            if (menu.Item("Draw_R").GetValue<bool>())
                if (R.Level > 0)
                    Utility.DrawCircle(Player.Position, R.Range, R.IsReady() ? Color.Green : Color.Red);

            if (menu.Item("Draw_R_Killable").GetValue<bool>())
            {
                foreach (var target in ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsValidTarget(5000) && !x.IsDead && x.IsEnemy).OrderBy(x => x.Health))
                {
                    Vector2 wts = Drawing.WorldToScreen(target.Position);
                    if (GetComboDmgPercent(target) < 30 && R.IsReady())
                    {
                        Drawing.DrawText(wts[0] - 20, wts[1], Color.White, "KILL!!!");

                    }

                    var enemyhp = target.Health / target.MaxHealth * 100;
                    if (GetHealthPercent() < enemyhp && E.IsReady())
                        Drawing.DrawText(wts[0] - 20, wts[1] - 30, Color.White, "Stunnable");
                }
            }
        }
    }
}

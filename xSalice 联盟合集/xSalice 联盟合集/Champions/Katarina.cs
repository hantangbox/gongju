using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

namespace xSaliceReligionAIO.Champions
{
    class Katarina : Champion
    {
        public Katarina()
        {
            SetUpSpells();
            LoadMenu();
        }

        private void SetUpSpells()
        {
            //intalize spell
            Q = new Spell(SpellSlot.Q, 675);
            W = new Spell(SpellSlot.W, 375);
            E = new Spell(SpellSlot.E, 700);
            R = new Spell(SpellSlot.R, 550);

            Q.SetTargetted(400, 1400);

            SpellList.Add(Q);
            SpellList.Add(W);
            SpellList.Add(E);
            SpellList.Add(R);
        }

        private void LoadMenu()
        {
            var key = new Menu("键位", "Key");{
                key.AddItem(new MenuItem("ComboActive", "连招!").SetValue(new KeyBind(32, KeyBindType.Press)));
                key.AddItem(new MenuItem("HarassActive", "骚扰!").SetValue(new KeyBind("C".ToCharArray()[0], KeyBindType.Press)));
                key.AddItem(new MenuItem("HarassActiveT", "骚扰 (自动)!").SetValue(new KeyBind("N".ToCharArray()[0], KeyBindType.Toggle)));
                key.AddItem(new MenuItem("LaneClearActive", "清线!").SetValue(new KeyBind("V".ToCharArray()[0], KeyBindType.Press)));
                key.AddItem(new MenuItem("jFarm", "清野!").SetValue(new KeyBind("V".ToCharArray()[0], KeyBindType.Press)));
                key.AddItem(new MenuItem("lastHit", "补刀!").SetValue(new KeyBind("A".ToCharArray()[0], KeyBindType.Press)));
                key.AddItem(new MenuItem("Wardjump", "逃跑|顺眼").SetValue(new KeyBind("Z".ToCharArray()[0], KeyBindType.Press)));
                //add to menu
                menu.AddSubMenu(key);
            }

            //Combo menu:
            var combo = new Menu("连招", "Combo");
            {
                combo.AddItem(new MenuItem("selected", "攻击 选定目标").SetValue(true));
                combo.AddItem(new MenuItem("UseQCombo", "使用 Q").SetValue(true));
                combo.AddItem(new MenuItem("UseWCombo", "使用 W").SetValue(true));
                combo.AddItem(new MenuItem("UseECombo", "使用 E").SetValue(true));
                combo.AddItem(new MenuItem("eDis", "使用E|距离敌人范围>").SetValue(new Slider(0, 0, 700)));
                combo.AddItem(new MenuItem("smartE", "智能E 如果 R在CD ").SetValue(false));
                combo.AddItem(new MenuItem("UseRCombo", "使用 R").SetValue(true));
                combo.AddItem(new MenuItem("comboMode", "连招 模式").SetValue(new StringList(new[] {"QEW", "EQW"})));
                //add to menu
                menu.AddSubMenu(combo);
            }
            //Harass menu:
            var harass = new Menu("骚扰", "Harass");
            {
                harass.AddItem(new MenuItem("UseQHarass", "使用 Q").SetValue(true));
                harass.AddItem(new MenuItem("UseWHarass", "使用 W").SetValue(false));
                harass.AddItem(new MenuItem("UseEHarass", "使用 E").SetValue(true));
                harass.AddItem(new MenuItem("harassMode", "骚扰 模式").SetValue(new StringList(new[] {"QEW", "EQW", "QW"}, 2)));
                //add to menu
                menu.AddSubMenu(harass);
            }
            //Farming menu:
            var farm = new Menu("清线", "Farm");
            {
                farm.AddItem(new MenuItem("UseQFarm", "使用 Q 清线").SetValue(false));
                farm.AddItem(new MenuItem("UseWFarm", "使用 W 清线").SetValue(false));
                farm.AddItem(new MenuItem("UseEFarm", "使用 E 清线").SetValue(false));
                farm.AddItem(new MenuItem("UseQHit", "使用 Q 补刀").SetValue(false));
                farm.AddItem(new MenuItem("UseWHit", "使用 W 补刀").SetValue(false));
                //add to menu
                menu.AddSubMenu(farm);
            }
            //killsteal
            var killSteal = new Menu("抢人头", "KillSteal");
            {
                killSteal.AddItem(new MenuItem("smartKS", "使用智能 抢人头模式").SetValue(true));
                killSteal.AddItem(new MenuItem("wardKs", "使用 顺眼 抢人头").SetValue(true));
                killSteal.AddItem(new MenuItem("rKS", "使用R 抢人头").SetValue(true));
                killSteal.AddItem(new MenuItem("rCancel", "没有R 不抢人头").SetValue(false));
                killSteal.AddItem(new MenuItem("KS_With_E", "禁用E 抢人头!").SetValue(new KeyBind("H".ToCharArray()[0], KeyBindType.Toggle)));
                //add to menu
                menu.AddSubMenu(killSteal);
            }
            //Misc Menu:
            var misc = new Menu("杂项", "Misc");
            {
                misc.AddItem(new MenuItem("dfg", "使用 冥火").SetValue(true));
                misc.AddItem(new MenuItem("ignite", "使用 点燃").SetValue(true));
                misc.AddItem(new MenuItem("igniteMode", "点燃 模式").SetValue(new StringList(new[] {"连招", "抢人头"})));
                misc.AddItem(new MenuItem("autoWz", "自动 W 敌人").SetValue(true));
                misc.AddItem(new MenuItem("E_Delay_Slider", "Delay Between E(ms)").SetValue(new Slider(0, 0, 1000)));
                //add to menu
                menu.AddSubMenu(misc);
            }

            //Drawings menu:
            var drawing = new Menu("范围", "Drawings");
            {
                drawing.AddItem(new MenuItem("QRange", "Q 范围").SetValue(new Circle(false, Color.FromArgb(100, 255, 0, 255))));
                drawing.AddItem(new MenuItem("WRange", "W 范围").SetValue(new Circle(true, Color.FromArgb(100, 255, 0, 255))));
                drawing.AddItem(new MenuItem("ERange", "E 范围").SetValue(new Circle(false, Color.FromArgb(100, 255, 0, 255))));
                drawing.AddItem(new MenuItem("RRange", "R 范围").SetValue(new Circle(false, Color.FromArgb(100, 255, 0, 255))));
                drawing.AddItem(new MenuItem("Draw_Mode", "显示 E 调试").SetValue(new Circle(false, Color.FromArgb(100, 255, 0, 255))));

                MenuItem drawComboDamageMenu = new MenuItem("Draw_ComboDamage", "显示组合连招伤害").SetValue(true);
                MenuItem drawFill = new MenuItem("Draw_Fill", "显示组合填充伤害").SetValue(new Circle(true, Color.FromArgb(90, 255, 169, 4)));
                drawing.AddItem(drawComboDamageMenu);
                drawing.AddItem(drawFill);
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
                menu.AddSubMenu(drawing);
            }
        }

        private float GetComboDamage(Obj_AI_Base enemy)
        {
            double damage = 0d;

            if (DFG.IsReady())
                damage += Player.GetItemDamage(enemy, Damage.DamageItems.Dfg) / 1.2;

            if (Q.IsReady())
                damage += Player.GetSpellDamage(enemy, SpellSlot.Q) + Player.GetSpellDamage(enemy, SpellSlot.Q, 1);

            if (W.IsReady())
                damage += Player.GetSpellDamage(enemy, SpellSlot.W);

            if (E.IsReady())
                damage += Player.GetSpellDamage(enemy, SpellSlot.E);

            if (R.IsReady() || (rSpell.State == SpellState.Surpressed && R.Level > 0))
                damage += Player.GetSpellDamage(enemy, SpellSlot.R) * 8;

            if (DFG.IsReady())
                damage = damage * 1.2;

            if (IgniteSlot != SpellSlot.Unknown && Player.SummonerSpellbook.CanUseSpell(IgniteSlot) == SpellState.Ready)
                damage += Player.GetSummonerSpellDamage(enemy, Damage.SummonerSpell.Ignite);

            return (float)damage;
        }

        private void Combo()
        {
            Combo(menu.Item("UseQCombo").GetValue<bool>(), menu.Item("UseWCombo").GetValue<bool>(),
                menu.Item("UseECombo").GetValue<bool>(), menu.Item("UseRCombo").GetValue<bool>());
        }

        private void Harass()
        {
            Harass(menu.Item("UseQHarass").GetValue<bool>(), menu.Item("UseWHarass").GetValue<bool>(),
                menu.Item("UseEHarass").GetValue<bool>());
        }

        private void Combo(bool useQ, bool useW, bool useE, bool useR)
        {
            Obj_AI_Hero target = SimpleTs.GetTarget(E.Range, SimpleTs.DamageType.Magical);

            int mode = menu.Item("comboMode").GetValue<StringList>().SelectedIndex;
            int igniteMode = menu.Item("igniteMode").GetValue<StringList>().SelectedIndex;

            int eDis = menu.Item("eDis").GetValue<Slider>().Value;

            var range = E.Range;
            if (GetTargetFocus(range) != null)
                target = GetTargetFocus(range);

            if (!target.HasBuffOfType(BuffType.Invulnerability) && target.IsValidTarget(E.Range))
            {
                if (mode == 0) //qwe
                {
                    if (DFG != null && (DFG.IsReady() && E.IsReady() && menu.Item("dfg").GetValue<bool>()))
                    {
                        Items.UseItem(DFG.Id, target);
                    }

                    if (useQ && Q.IsReady() && Player.Distance(target) <= Q.Range)
                    {
                        Q.Cast(target, packets());
                    }

                    if (useE && E.IsReady() && Player.Distance(target) < E.Range && Environment.TickCount - E.LastCastAttemptT > 0 && 
                        Player.Distance(target) > eDis)
                    {
                        if (menu.Item("smartE").GetValue<bool>() &&
                            countEnemiesNearPosition(target.ServerPosition, 500) > 2 &&
                            (!R.IsReady() || !(rSpell.State == SpellState.Surpressed && R.Level > 0)))
                            return;

                        var delay = menu.Item("E_Delay_Slider").GetValue<Slider>().Value;
                        E.Cast(target, packets());
                        E.LastCastAttemptT = Environment.TickCount + delay;
                    }
                }
                else if (mode == 1) //eqw
                {
                    if (DFG.IsReady() && E.IsReady() && menu.Item("dfg").GetValue<bool>())
                    {
                        Items.UseItem(DFG.Id, target);
                    }

                    if (useE && E.IsReady() && Player.Distance(target) < E.Range && Environment.TickCount - E.LastCastAttemptT > 0 &&
                        Player.Distance(target) > eDis)
                    {
                        if (menu.Item("smartE").GetValue<bool>() &&
                            countEnemiesNearPosition(target.ServerPosition, 500) > 2 &&
                            (!R.IsReady() || !(rSpell.State == SpellState.Surpressed && R.Level > 0)))
                            return;

                        var delay = menu.Item("E_Delay_Slider").GetValue<Slider>().Value;
                        E.Cast(target, packets());
                        E.LastCastAttemptT = Environment.TickCount + delay;
                    }

                    if (useQ && Q.IsReady() && Player.Distance(target) <= Q.Range)
                    {
                        Q.Cast(target, packets());
                    }
                }

                //Ignite
                if (menu.Item("ignite").GetValue<bool>() && IgniteSlot != SpellSlot.Unknown &&
                    Player.SummonerSpellbook.CanUseSpell(IgniteSlot) == SpellState.Ready)
                {
                    if (igniteMode == 0 && GetComboDamage(target) > target.Health)
                    {
                        Player.SummonerSpellbook.CastSpell(IgniteSlot, target);
                    }
                }

                if (useW && W.IsReady() && Player.Distance(target) <= W.Range)
                {
                    W.Cast();
                }

                if (useR && R.IsReady() &&
                    countEnemiesNearPosition(Player.ServerPosition, R.Range) > 0)
                {
                    if (!Q.IsReady() && !E.IsReady() && !W.IsReady())
                        R.Cast();
                }
            }
        }

        private void Harass(bool useQ, bool useW, bool useE)
        {
            Obj_AI_Hero qTarget = SimpleTs.GetTarget(Q.Range, SimpleTs.DamageType.Magical);
            Obj_AI_Hero wTarget = SimpleTs.GetTarget(W.Range, SimpleTs.DamageType.Magical);
            Obj_AI_Hero eTarget = SimpleTs.GetTarget(E.Range, SimpleTs.DamageType.Magical);
            SimpleTs.GetTarget(R.Range, SimpleTs.DamageType.Magical);

            int mode = menu.Item("harassMode").GetValue<StringList>().SelectedIndex;

            if (mode == 0) //qwe
            {
                if (useQ && Q.IsReady() && Player.Distance(qTarget) <= Q.Range && qTarget != null)
                {
                    Q.Cast(qTarget, packets());
                }

                if (useE && eTarget != null && E.IsReady() && Player.Distance(eTarget) < E.Range)
                {
                    E.Cast(eTarget, packets());
                }
            }
            else if (mode == 1) //eqw
            {
                if (useE && eTarget != null && E.IsReady() && Player.Distance(eTarget) < E.Range)
                {
                    E.Cast(eTarget, packets());
                }

                if (useQ && Q.IsReady() && Player.Distance(qTarget) <= Q.Range && qTarget != null)
                {
                    Q.Cast(qTarget, packets());
                }
            }
            else if (mode == 2)
            {
                if (useQ && Q.IsReady() && Player.Distance(qTarget) <= Q.Range && qTarget != null)
                {
                    Q.Cast(qTarget, packets());
                }
            }

            if (useW && wTarget != null && W.IsReady() && Player.Distance(wTarget) <= W.Range)
            {
                W.Cast();
            }
        }

        private void LastHit()
        {
            List<Obj_AI_Base> allMinions = MinionManager.GetMinions(Player.ServerPosition, Q.Range, MinionTypes.All,
                MinionTeam.NotAlly);
            MinionManager.GetMinions(Player.ServerPosition, W.Range);

            var useQ = menu.Item("UseQHit").GetValue<bool>();
            var useW = menu.Item("UseWHit").GetValue<bool>();

            if (Q.IsReady() && useQ)
            {
                foreach (Obj_AI_Base minion in allMinions)
                {
                    if (minion.IsValidTarget(Q.Range) &&
                        HealthPrediction.GetHealthPrediction(minion, (int)(Player.Distance(minion) * 1000 / 1400)) <
                        Player.GetSpellDamage(minion, SpellSlot.Q) - 35)
                    {
                        Q.CastOnUnit(minion, packets());
                        return;
                    }
                }
            }

            if (W.IsReady() && useW)
            {
                if (allMinions.Where(minion => minion.IsValidTarget(W.Range) && minion.Health <
                                               Player.GetSpellDamage(minion, SpellSlot.W) - 35).Any(minion => Player.Distance(minion.ServerPosition) < W.Range))
                {
                    W.Cast();
                }
            }
        }

        private void Farm()
        {
            List<Obj_AI_Base> allMinionsQ = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q.Range,
                MinionTypes.All, MinionTeam.NotAlly);
            List<Obj_AI_Base> allMinionsE = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, E.Range,
                MinionTypes.All, MinionTeam.NotAlly);
            List<Obj_AI_Base> allMinionsW = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, W.Range,
                MinionTypes.All, MinionTeam.NotAlly);

            var useQ = menu.Item("UseQFarm").GetValue<bool>();
            var useW = menu.Item("UseWFarm").GetValue<bool>();
            var useE = menu.Item("UseEFarm").GetValue<bool>();

            if (useQ && allMinionsQ.Count > 0 && Q.IsReady() && allMinionsQ[0].IsValidTarget(Q.Range))
            {
                Q.Cast(allMinionsQ[0], packets());
            }

            if (useE && allMinionsQ.Count > 0 && E.IsReady() && allMinionsQ[0].IsValidTarget(E.Range))
            {
                E.Cast(allMinionsE[0], packets());
            }

            if (useW && W.IsReady())
            {
                MinionManager.FarmLocation wPos = E.GetCircularFarmLocation(allMinionsW);
                if (wPos.MinionsHit >= 2)
                    W.Cast();
            }
        }
        private void JungleFarm()
        {
            List<Obj_AI_Base> allMinionsQ = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q.Range,
                MinionTypes.All, MinionTeam.Neutral);
            List<Obj_AI_Base> allMinionsW = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, W.Range,
                MinionTypes.All, MinionTeam.Neutral);

            var useQ = menu.Item("UseQFarm").GetValue<bool>();
            var useW = menu.Item("UseWFarm").GetValue<bool>();

            if (useQ && allMinionsQ.Count > 0 && Q.IsReady() && allMinionsQ[0].IsValidTarget(Q.Range))
            {
                Q.Cast(allMinionsQ[0], packets());
            }

            if (useW && W.IsReady())
            {
                MinionManager.FarmLocation wPos = E.GetCircularFarmLocation(allMinionsW);
                if (wPos.MinionsHit >= 3)
                    W.Cast();
            }
        }

        private void SmartKs()
        {
            if (!menu.Item("smartKS").GetValue<bool>())
                return;

            if (menu.Item("rCancel").GetValue<bool>() && countEnemiesNearPosition(Player.ServerPosition, 570) > 1)
                return;

            foreach (Obj_AI_Hero target in ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsValidTarget(1375) && x.IsEnemy && !x.IsDead).OrderByDescending(GetComboDamage))
            {
                if (target != null && !target.HasBuffOfType(BuffType.Invulnerability) &&
                    target.IsValidTarget(1375))
                {
                    var delay = menu.Item("E_Delay_Slider").GetValue<Slider>().Value;
                    bool shouldE = !menu.Item("KS_With_E").GetValue<KeyBind>().Active && Environment.TickCount - E.LastCastAttemptT > 0;
                    //QEW
                    if (Player.Distance(target.ServerPosition) <= E.Range && shouldE &&
                        (Player.GetSpellDamage(target, SpellSlot.E) + Player.GetSpellDamage(target, SpellSlot.Q) + Player.GetSpellDamage(target, SpellSlot.Q, 1) + 
                         Player.GetSpellDamage(target, SpellSlot.W)) > target.Health + 20)
                    {
                        if (E.IsReady() && Q.IsReady() && W.IsReady())
                        {
                            CancelUlt(target);
                            Q.Cast(target, packets());
                            E.Cast(target, packets());
                            E.LastCastAttemptT = Environment.TickCount + delay;
                            if (Player.Distance(target.ServerPosition) < W.Range)
                                W.Cast();
                            return;
                        }
                    }

                    //E + W
                    if (Player.Distance(target.ServerPosition) <= E.Range && shouldE &&
                        (Player.GetSpellDamage(target, SpellSlot.E) + Player.GetSpellDamage(target, SpellSlot.W)) >
                        target.Health + 20)
                    {
                        if (E.IsReady() && W.IsReady())
                        {
                            CancelUlt(target);
                            E.Cast(target, packets());
                            E.LastCastAttemptT = Environment.TickCount + delay;
                            if (Player.Distance(target.ServerPosition) < W.Range)
                                W.Cast();
                            //Game.PrintChat("ks 5");
                            return;
                        }
                    }

                    //E + Q
                    if (Player.Distance(target.ServerPosition) <= E.Range && shouldE &&
                        (Player.GetSpellDamage(target, SpellSlot.E) + Player.GetSpellDamage(target, SpellSlot.Q)) >
                        target.Health + 20)
                    {
                        if (E.IsReady() && Q.IsReady())
                        {
                            CancelUlt(target);
                            E.Cast(target, packets());
                            E.LastCastAttemptT = Environment.TickCount + delay;
                            Q.Cast(target, packets());
                            //Game.PrintChat("ks 6");
                            return;
                        }
                    }

                    //Q
                    if ((Player.GetSpellDamage(target, SpellSlot.Q)) > target.Health + 20)
                    {
                        if (Q.IsReady() && Player.Distance(target.ServerPosition) <= Q.Range)
                        {
                            CancelUlt(target);
                            Q.Cast(target, packets());
                            //Game.PrintChat("ks 7");
                            return;
                        }
                        if (Q.IsReady() && E.IsReady() && Player.Distance(target.ServerPosition) <= 1375 &&
                            menu.Item("wardKs").GetValue<bool>() &&
                            countEnemiesNearPosition(target.ServerPosition, 500) < 3)
                        {
                            CancelUlt(target);
                            JumpKs(target);
                            //Game.PrintChat("wardKS!!!!!");
                            return;
                        }
                    }

                    //E
                    if (Player.Distance(target.ServerPosition) <= E.Range && shouldE &&
                        (Player.GetSpellDamage(target, SpellSlot.E)) > target.Health + 20)
                    {
                        if (E.IsReady())
                        {
                            CancelUlt(target);
                            E.Cast(target, packets());
                            E.LastCastAttemptT = Environment.TickCount + delay;
                            //Game.PrintChat("ks 8");
                            return;
                        }
                    }

                    //R
                    if (Player.Distance(target.ServerPosition) <= E.Range &&
                        (Player.GetSpellDamage(target, SpellSlot.R) * 5) > target.Health + 20 &&
                        menu.Item("rKS").GetValue<bool>())
                    {
                        if (R.IsReady())
                        {
                            R.Cast();
                            //Game.PrintChat("ks 8");
                            return;
                        }
                    }

                    //dfg
                    if (DFG.IsReady() && Player.GetItemDamage(target, Damage.DamageItems.Dfg) > target.Health + 20 &&
                        Player.Distance(target.ServerPosition) <= 750)
                    {
                        Items.UseItem(DFG.Id, target);
                        //Game.PrintChat("ks 1");
                        return;
                    }

                    //dfg + q
                    if (Player.Distance(target.ServerPosition) <= Q.Range &&
                        (Player.GetItemDamage(target, Damage.DamageItems.Dfg) +
                         (Player.GetSpellDamage(target, SpellSlot.Q)) * 1.2) > target.Health + 20)
                    {
                        if (DFG.IsReady() && Q.IsReady())
                        {
                            Items.UseItem(DFG.Id, target);
                            CancelUlt(target);
                            Q.Cast(target, packets());
                            //Game.PrintChat("ks 2");
                            return;
                        }
                    }

                    //dfg + e
                    if (Player.Distance(target.ServerPosition) <= E.Range &&
                        (Player.GetItemDamage(target, Damage.DamageItems.Dfg) +
                         (Player.GetSpellDamage(target, SpellSlot.E)) * 1.2) > target.Health + 20)
                    {
                        if (DFG.IsReady() && E.IsReady())
                        {
                            Items.UseItem(DFG.Id, target);
                            CancelUlt(target);
                            E.Cast(target, packets());
                            E.LastCastAttemptT = Environment.TickCount + delay;
                            //Game.PrintChat("ks 3");
                            return;
                        }
                    }

                    //ignite
                    if (menu.Item("ignite").GetValue<bool>() && IgniteSlot != SpellSlot.Unknown &&
                        Player.SummonerSpellbook.CanUseSpell(IgniteSlot) == SpellState.Ready &&
                        Player.Distance(target.ServerPosition) <= 600)
                    {
                        if (Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite) > target.Health)
                        {
                            Player.SummonerSpellbook.CastSpell(IgniteSlot, target);
                        }
                    }
                }
            }
        }

        private void CancelUlt(Obj_AI_Hero target)
        {
            if (Player.IsChannelingImportantSpell())
            {
                //xSLxOrbwalker.Orbwalk(target.ServerPosition, null);
                Player.IssueOrder(GameObjectOrder.MoveTo, target.ServerPosition);
            }
        }

        private void ShouldCancel()
        {
            if (countEnemiesNearPosition(Player.ServerPosition, 600) < 1)
            {
                List<Obj_AI_Hero> nearChamps = (from champ in ObjectManager.Get<Obj_AI_Hero>()
                                                where champ.IsValidTarget(1375) && champ.IsEnemy
                                                select champ).ToList();

                if (nearChamps.FirstOrDefault() != null && nearChamps.FirstOrDefault().IsValidTarget(1375))
                {
                    var objAiHero = nearChamps.FirstOrDefault();
                    if (objAiHero != null)
                        Player.IssueOrder(GameObjectOrder.MoveTo, objAiHero);
                        //xSLxOrbwalker.Orbwalk(nearChamps.FirstOrDefault().ServerPosition, null);
                }
            }
        }

        private void AutoW()
        {
            foreach (Obj_AI_Hero target in ObjectManager.Get<Obj_AI_Hero>())
            {
                if (target != null && !target.IsDead && target.IsEnemy &&
                    Player.Distance(target.ServerPosition) <= W.Range && target.IsValidTarget(W.Range))
                {
                    if (Player.Distance(target.ServerPosition) < W.Range && W.IsReady())
                        W.Cast();
                }
            }
        }

        //wardjump
        //-------------------------------------------------

        private void JumpKs(Obj_AI_Hero target)
        {
            foreach (Obj_AI_Minion ward in ObjectManager.Get<Obj_AI_Minion>().Where(ward =>
                E.IsReady() && Q.IsReady() && ward.Name.ToLower().Contains("ward") &&
                ward.Distance(target.ServerPosition) < Q.Range && ward.Distance(Player) < E.Range))
            {
                E.Cast(ward);
                return;
            }

            foreach (Obj_AI_Hero hero in ObjectManager.Get<Obj_AI_Hero>().Where(hero =>
                E.IsReady() && Q.IsReady() && hero.Distance(target.ServerPosition) < Q.Range &&
                hero.Distance(Player) < E.Range && hero.IsValidTarget(E.Range)))
            {
                E.Cast(hero);
                return;
            }

            foreach (Obj_AI_Minion minion in ObjectManager.Get<Obj_AI_Minion>().Where(minion =>
                E.IsReady() && Q.IsReady() && minion.Distance(target.ServerPosition) < Q.Range &&
                minion.Distance(Player) < E.Range && minion.IsValidTarget(E.Range)))
            {
                E.Cast(minion);
                return;
            }

            if (Player.Distance(target) < Q.Range)
            {
                Q.Cast(target, packets());
                return;
            }

            if (E.IsReady() && Q.IsReady())
            {
                Vector3 position = Player.ServerPosition +
                                   Vector3.Normalize(target.ServerPosition - Player.ServerPosition) * 590;

                if (target.Distance(position) < Q.Range)
                {
                    InventorySlot invSlot = FindBestWardItem();
                    if (invSlot == null) return;

                    invSlot.UseItem(position);
                    lastWardPos = position;
                    lastPlaced = Environment.TickCount;
                }
            }

            if (Player.Distance(target) < Q.Range)
            {
                Q.Cast(target, packets());
            }
        }

        private void WardJump()
        {
            //wardWalk(Game.CursorPos);

            foreach (Obj_AI_Minion ward in ObjectManager.Get<Obj_AI_Minion>().Where(ward =>
                ward.Name.ToLower().Contains("ward") && ward.Distance(Game.CursorPos) < 250))
            {
                if (E.IsReady())
                {
                    E.CastOnUnit(ward);
                    return;
                }
            }

            foreach (
                Obj_AI_Hero hero in ObjectManager.Get<Obj_AI_Hero>().Where(hero => hero.Distance(Game.CursorPos) < 250 && !hero.IsDead))
            {
                if (E.IsReady())
                {
                    E.CastOnUnit(hero);
                    return;
                }
            }

            foreach (Obj_AI_Minion minion in ObjectManager.Get<Obj_AI_Minion>().Where(minion =>
                minion.Distance(Game.CursorPos) < 250))
            {
                if (E.IsReady())
                {
                    E.CastOnUnit(minion);
                    return;
                }
            }

            if (Environment.TickCount <= lastPlaced + 3000 || !E.IsReady()) return;

            Vector3 cursorPos = Game.CursorPos;
            Vector3 myPos = Player.ServerPosition;

            Vector3 delta = cursorPos - myPos;
            delta.Normalize();

            Vector3 wardPosition = myPos + delta * (600 - 5);

            InventorySlot invSlot = FindBestWardItem();
            if (invSlot == null) return;

            Items.UseItem((int)invSlot.Id, wardPosition);
            lastWardPos = wardPosition;
            lastPlaced = Environment.TickCount;
        }

        private static InventorySlot FindBestWardItem()
        {
            InventorySlot slot = Items.GetWardSlot();
            if (slot == default(InventorySlot)) return null;
            return slot;
        }

        //end wardjump
        //-------------------------------------------------
        //-------------------------------------------------

        public override void Game_OnGameUpdate(EventArgs args)
        {
            //check if player is dead
            if (Player.IsDead) return;

            SmartKs();

            if (Player.IsChannelingImportantSpell())
            {
                ShouldCancel();
                return;
            }

            if (menu.Item("Wardjump").GetValue<KeyBind>().Active)
            {
                WardJump();
            }
            else if (menu.Item("ComboActive").GetValue<KeyBind>().Active)
            {
                Combo();
            }
            else
            {
                if (menu.Item("lastHit").GetValue<KeyBind>().Active)
                    LastHit();

                if (menu.Item("LaneClearActive").GetValue<KeyBind>().Active)
                    Farm();

                if (menu.Item("jFarm").GetValue<KeyBind>().Active)
                    JungleFarm();

                if (menu.Item("HarassActive").GetValue<KeyBind>().Active)
                    Harass();

                if (menu.Item("HarassActiveT").GetValue<KeyBind>().Active)
                    Harass();
            }

            if (menu.Item("autoWz").GetValue<bool>())
                AutoW();
        }

        public override void Drawing_OnDraw(EventArgs args)
        {
            foreach (Spell spell in SpellList)
            {
                var menuItem = menu.Item(spell.Slot + "Range").GetValue<Circle>();
                if (menuItem.Active)
                    Utility.DrawCircle(Player.Position, spell.Range, (spell.IsReady()) ? Color.Cyan : Color.DarkRed);
            }

            if (menu.Item("Draw_Mode").GetValue<Circle>().Active)
            {
                var wts = Drawing.WorldToScreen(Player.Position);

                Drawing.DrawText(wts[0], wts[1], Color.White,
                    menu.Item("KS_With_E").GetValue<KeyBind>().Active ? "Ks E Active" : "Ks E Off");
            }
        }

        public override void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            if (Environment.TickCount < lastPlaced + 300)
            {
                var ward = (Obj_AI_Minion)sender;
                if (ward.Name.ToLower().Contains("ward") && ward.Distance(lastWardPos) < 500 && E.IsReady())
                {
                    E.Cast(ward);
                }
            }
        }
    }
}

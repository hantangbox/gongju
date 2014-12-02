using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

namespace xSaliceReligionAIO.Champions
{
    class Ahri : Champion
    {
        public Ahri()
        {
            //Set up mana
            //Q
            qMana = new[] {55, 55, 60, 65, 70, 75};
            //W
            wMana = new[] { 50, 50, 50, 50, 50, 50 };
            //E
            eMana = new[] { 85, 85, 85, 85, 85, 85 };
            //R
            rMana = new[] { 100, 100, 100, 100 };
   
            //set up spells
            SetUpSpells();

            //Set up Menu
            LoadMenu();
            
        }

        private void SetUpSpells()
        {
            //intalize spell
            Q = new Spell(SpellSlot.Q, 900);
            W = new Spell(SpellSlot.W, 800);
            E = new Spell(SpellSlot.E, 875);
            R = new Spell(SpellSlot.R, 850);

            Q.SetSkillshot(0.25f, 100, 1600, false, SkillshotType.SkillshotLine);
            E.SetSkillshot(0.25f, 60, 1200, true, SkillshotType.SkillshotLine);

            SpellList.Add(Q);
            SpellList.Add(W);
            SpellList.Add(E);
            SpellList.Add(R);
        }

        //Load Menu
        private void LoadMenu()
        {
            //key
            var key = new Menu("键位", "Key");{
                key.AddItem(new MenuItem("ComboActive", "连招!").SetValue(new KeyBind(32, KeyBindType.Press)));
                key.AddItem(new MenuItem("HarassActive", "骚扰!").SetValue(new KeyBind("C".ToCharArray()[0], KeyBindType.Press)));
                key.AddItem(new MenuItem("HarassActiveT", "骚扰 (自动)!").SetValue(new KeyBind("N".ToCharArray()[0], KeyBindType.Toggle)));
                key.AddItem(new MenuItem("LaneClearActive", "清线!").SetValue(new KeyBind("V".ToCharArray()[0], KeyBindType.Press)));
                key.AddItem(new MenuItem("charmCombo", "自动Q魅惑的敌人").SetValue(new KeyBind("I".ToCharArray()[0], KeyBindType.Toggle)));
                //add to menu
                menu.AddSubMenu(key);
            }

            //Combo menu:
            var combo = new Menu("连招", "Combo");
            {
                combo.AddItem(new MenuItem("selected", "攻击 选定目标").SetValue(true));
                combo.AddItem(new MenuItem("UseQCombo", "使用 Q").SetValue(true));
                combo.AddItem(new MenuItem("qHit", "Q/E 命中率").SetValue(new Slider(3, 1, 4)));
                combo.AddItem(new MenuItem("UseWCombo", "使用 W").SetValue(true));
                combo.AddItem(new MenuItem("UseECombo", "使用 E").SetValue(true));
                combo.AddItem(new MenuItem("UseRCombo", "使用 R").SetValue(true));
                combo.AddItem(new MenuItem("rSpeed", "使用R后技能全交").SetValue(true));
                combo.AddItem(new MenuItem("ignite", "使用 点燃").SetValue(true));
                combo.AddItem(new MenuItem("igniteMode", "点燃 模式").SetValue(new StringList(new[] { "连招", "抢人头" })));
                //add to menu
                menu.AddSubMenu(combo);
            }
            //Harass menu:
            var harass = new Menu("骚扰", "Harass");
            {
               harass.AddItem(new MenuItem("UseQHarass", "使用 Q").SetValue(true));
               harass.AddItem(new MenuItem("qHit2", "Q/E 命中率").SetValue(new Slider(3, 1, 4)));
               harass.AddItem(new MenuItem("UseWHarass", "使用 W").SetValue(false));
               harass.AddItem(new MenuItem("UseEHarass", "使用 E").SetValue(true));
               harass.AddItem(new MenuItem("longQ", "使用 极限距离 Q").SetValue(true));
               harass.AddItem(new MenuItem("charmHarass", "只Q被魅惑的敌人").SetValue(true)); 
               //add to menu
                menu.AddSubMenu(harass);
            }
            //Farming menu:
            var farm = new Menu("清线", "Farm");
            {
                farm.AddItem(new MenuItem("UseQFarm", "使用 Q").SetValue(false));
                farm.AddItem(new MenuItem("UseWFarm", "使用 W").SetValue(false));
                //add to menu
                menu.AddSubMenu(farm);
            }

            //Misc Menu:
            var misc = new Menu("杂项", "Misc");
            {
                misc.AddItem(new MenuItem("UseInt", "使用 E 中断法术").SetValue(true));
                misc.AddItem(new MenuItem("UseGap", "使用 E 防止突进").SetValue(true));
                misc.AddItem(new MenuItem("mana", "使用R前检测魔量").SetValue(true));
                misc.AddItem(new MenuItem("dfgCharm", "对魅惑的敌人强制冥火").SetValue(true));
                misc.AddItem(new MenuItem("EQ", "使用E之前先Q").SetValue(true));
                misc.AddItem(new MenuItem("smartKS", "智能 抢人头").SetValue(true));
                misc.AddItem(new MenuItem("Prediction_Check_Off", "使用 预测 模式 2").SetValue(false));
                //add to menu
                menu.AddSubMenu(misc);
            }

            //Drawings menu:
            var drawing = new Menu("范围", "Drawings");
            {
                drawing.AddItem(
                        new MenuItem("QRange", "Q 范围").SetValue(new Circle(false, Color.FromArgb(100, 255, 0, 255))));
                drawing.AddItem(
                        new MenuItem("WRange", "W 范围").SetValue(new Circle(true, Color.FromArgb(100, 255, 0, 255))));
                drawing.AddItem(
                        new MenuItem("ERange", "E 范围").SetValue(new Circle(false, Color.FromArgb(100, 255, 0, 255))));
                drawing.AddItem(
                        new MenuItem("RRange", "R 范围").SetValue(new Circle(false, Color.FromArgb(100, 255, 0, 255))));
                drawing.AddItem(
                        new MenuItem("cursor", "显示 R 突击范围").SetValue(new Circle(false,Color.FromArgb(100, 255, 0, 255))));
                drawing.AddItem(
                        new MenuItem("Draw_Mode", "显示 E 调试").SetValue(new Circle(false, Color.FromArgb(100, 255, 0, 255))));

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
                menu.AddSubMenu(drawing);
            }
        }
        
        //settings
        private static bool _rOn;
        private static int _rTimer;
        private static int _rTimeLeft;

        private float GetComboDamage(Obj_AI_Base enemy)
        {
            double damage = 0d;

            if (Q.IsReady())
            {
                damage += Player.GetSpellDamage(enemy, SpellSlot.Q);
                damage += Player.GetSpellDamage(enemy, SpellSlot.Q, 1);
            }

            if (DFG.IsReady())
                damage += Player.GetItemDamage(enemy, Damage.DamageItems.Dfg) / 1.2;

            if (W.IsReady())
                damage += Player.GetSpellDamage(enemy, SpellSlot.W);

            if (_rOn)
                damage += Player.GetSpellDamage(enemy, SpellSlot.R)*RCount();
            else if (R.IsReady())
                damage += Player.GetSpellDamage(enemy, SpellSlot.R) * 3; 

            if (DFG.IsReady() && E.IsReady())
                damage = damage * 1.44;
            else if (DFG.IsReady() && enemy.HasBuffOfType(BuffType.Charm))
                damage = damage * 1.44;
            else if (E.IsReady())
                damage = damage * 1.2;
            else if (DFG.IsReady())
                damage = damage * 1.2;
            else if (enemy.HasBuffOfType(BuffType.Charm))
                damage = damage * 1.2;

            if (Ignite_Ready())
                damage += Player.GetSummonerSpellDamage(enemy, Damage.SummonerSpell.Ignite);

            if (E.IsReady())
                damage += Player.GetSpellDamage(enemy, SpellSlot.E);

            return (float)damage;
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
            var range = Q.Range;
            Obj_AI_Hero eTarget = SimpleTs.GetTarget(range, SimpleTs.DamageType.Magical);


            if (GetTargetFocus(range) != null)
                eTarget = GetTargetFocus(range);

            Obj_AI_Hero rETarget = SimpleTs.GetTarget(E.Range, SimpleTs.DamageType.Magical);

            int igniteMode = menu.Item("igniteMode").GetValue<StringList>().SelectedIndex;

            var hitC = GetHitchance(source);
            var dmg = GetComboDamage(eTarget);
            var predOff = menu.Item("Prediction_Check_Off").GetValue<bool>();
                
            //DFG
            if (eTarget != null && dmg > eTarget.Health - 300 && DFG.IsReady() && source == "Combo" && Player.Distance(eTarget) <= 750 &&
                (eTarget.HasBuffOfType(BuffType.Charm) || !menu.Item("dfgCharm").GetValue<bool>()))
            {
               Use_DFG(eTarget);
            }

            //E
            if (useE && eTarget != null && E.IsReady() && Player.Distance(eTarget) < E.Range)
            {
                if (E.GetPrediction(eTarget).Hitchance >= hitC || predOff)
                {
                    E.Cast(eTarget, packets());
                    if (menu.Item("EQ").GetValue<bool>() && Q.IsReady())
                    {
                        Q.Cast(eTarget, packets());
                    }
                    return;
                }
            }

            //Ignite
            if (eTarget != null && Ignite_Ready() && !E.IsReady() && source == "Combo")
            {
                if (igniteMode == 0 && dmg > eTarget.Health)
                {
                    Use_Ignite(eTarget);
                }
            }

            //W
            if (useW && eTarget != null && W.IsReady() && Player.Distance(eTarget) <= W.Range - 100 &&
                ShouldW(eTarget, source))
            {
                W.Cast();
            }

            if (source == "Harass" && menu.Item("longQ").GetValue<bool>())
            {
                if (useQ && Q.IsReady() && Player.Distance(eTarget) <= Q.Range && eTarget != null &&
                    ShouldQ(eTarget, source) && Player.Distance(eTarget) > 600)
                {
                    if (Q.GetPrediction(eTarget).Hitchance >= hitC || predOff)
                    {
                        Q.Cast(eTarget, packets(), true);
                        return;
                    }
                }
            }
            else if (useQ && Q.IsReady() && Player.Distance(eTarget) <= Q.Range && eTarget != null &&
                     ShouldQ(eTarget, source))
            {
                if (Q.GetPrediction(eTarget).Hitchance >= hitC || predOff)
                {
                    Q.Cast(eTarget, packets(), true);
                    return;
                }
            }

            //R
            if (useR && eTarget != null && R.IsReady() && Player.Distance(eTarget) < R.Range)
            {
                if (E.IsReady())
                {
                    if (CheckReq(rETarget))
                        E.Cast(rETarget, packets());
                }
                if (ShouldR(eTarget) && R.IsReady())
                {
                    R.Cast(Game.CursorPos, packets());
                    _rTimer = Environment.TickCount - 250;
                }
                if (_rTimeLeft > 9500 && _rOn && R.IsReady())
                {
                    R.Cast(Game.CursorPos, packets());
                    _rTimer = Environment.TickCount - 250;
                }
            }
        }

        private void CheckKs()
        {
            foreach (Obj_AI_Hero target in ObjectManager.Get<Obj_AI_Hero>().Where(x => Player.IsValidTarget(1300) && x.IsEnemy && !x.IsDead).OrderByDescending(GetComboDamage))
            {
                if (target != null)
                {
                    if (DFG.IsReady() && Player.GetItemDamage(target, Damage.DamageItems.Dfg) > target.Health &&
                        Player.Distance(target.ServerPosition) <= 750)
                    {
                        Use_DFG(target);
                        return;
                    }

                    if (DFG.IsReady() && Player.Distance(target.ServerPosition) <= 750 && Q.IsReady() &&
                        (Player.GetItemDamage(target, Damage.DamageItems.Dfg) +
                         (Player.GetSpellDamage(target, SpellSlot.Q) + Player.GetSpellDamage(target, SpellSlot.Q, 1)) *
                         1.2) > target.Health)
                    {
                        Use_DFG(target);
                        Q.Cast(target, packets());
                        return;
                    }

                    if (DFG.IsReady() && Player.Distance(target.ServerPosition) <= 750 && W.IsReady() &&
                        (Player.GetItemDamage(target, Damage.DamageItems.Dfg) +
                         Player.GetSpellDamage(target, SpellSlot.W) * 1.2) > target.Health)
                    {
                        Use_DFG(target);
                        W.Cast();
                        return;
                    }

                    if (Player.Distance(target.ServerPosition) <= W.Range &&
                        (Player.GetSpellDamage(target, SpellSlot.Q) + Player.GetSpellDamage(target, SpellSlot.Q, 1) +
                         Player.GetSpellDamage(target, SpellSlot.W)) > target.Health && Q.IsReady() && Q.IsReady())
                    {
                        Q.Cast(target, packets());
                        return;
                    }

                    if (Player.Distance(target.ServerPosition) <= Q.Range &&
                        (Player.GetSpellDamage(target, SpellSlot.Q) + Player.GetSpellDamage(target, SpellSlot.Q, 1)) >
                        target.Health && Q.IsReady())
                    {
                        Q.Cast(target, packets());
                        return;
                    }

                    if (Player.Distance(target.ServerPosition) <= E.Range &&
                        (Player.GetSpellDamage(target, SpellSlot.E)) > target.Health & E.IsReady())
                    {
                        E.Cast(target, packets());
                        return;
                    }

                    if (Player.Distance(target.ServerPosition) <= W.Range &&
                        (Player.GetSpellDamage(target, SpellSlot.W)) > target.Health && W.IsReady())
                    {
                        W.Cast();
                        return;
                    }

                    Vector3 dashVector = Player.Position +
                                         Vector3.Normalize(target.ServerPosition - Player.Position) * 425;
                    if (Player.Distance(target.ServerPosition) <= R.Range &&
                        (Player.GetSpellDamage(target, SpellSlot.R)) > target.Health && R.IsReady() && _rOn &&
                        target.Distance(dashVector) < 425 && R.IsReady())
                    {
                        R.Cast(dashVector, packets());
                    }

                    //ignite
                    if (menu.Item("ignite").GetValue<bool>() && IgniteSlot != SpellSlot.Unknown &&
                        Player.SummonerSpellbook.CanUseSpell(IgniteSlot) == SpellState.Ready &&
                        Player.Distance(target.ServerPosition) <= 600)
                    {
                        if (Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite) > target.Health + 20)
                        {
                            Use_Ignite(target);
                        }
                    }
                }
            }
        }

        private bool ShouldQ(Obj_AI_Hero target, string source)
        {
            if (source == "Combo")
            {
                if ((Player.GetSpellDamage(target, SpellSlot.Q) + Player.GetSpellDamage(target, SpellSlot.Q, 1)) >
                    target.Health)
                    return true;

                if (_rOn)
                    return true;

                if (!menu.Item("charmCombo").GetValue<KeyBind>().Active)
                    return true;

                if (target.HasBuffOfType(BuffType.Charm))
                    return true;
            }

            if (source == "Harass")
            {
                if ((Player.GetSpellDamage(target, SpellSlot.Q) + Player.GetSpellDamage(target, SpellSlot.Q, 1)) >
                    target.Health)
                    return true;

                if (_rOn)
                    return true;

                if (!menu.Item("charmHarass").GetValue<bool>())
                    return true;

                if (target.HasBuffOfType(BuffType.Charm))
                    return true;
            }

            return false;
        }

        private bool ShouldW(Obj_AI_Hero target, string source)
        {
            if (source == "Combo")
            {
                if (Player.GetSpellDamage(target, SpellSlot.W) > target.Health)
                    return true;

                if (_rOn)
                    return true;

                if (!menu.Item("charmCombo").GetValue<KeyBind>().Active)
                    return true;

                if (target.HasBuffOfType(BuffType.Charm))
                    return true;
            }
            if (source == "Harass")
            {
                if (Player.GetSpellDamage(target, SpellSlot.W) > target.Health)
                    return true;

                if (_rOn)
                    return true;

                if (!menu.Item("charmHarass").GetValue<bool>())
                    return true;

                if (target.HasBuffOfType(BuffType.Charm))
                    return true;
            }

            return false;
        }

        private bool ShouldR(Obj_AI_Hero target)
        {
            if (!manaCheck())
                return false;

            Vector3 dashVector = Player.Position + Vector3.Normalize(Game.CursorPos - Player.Position) * 425;
            if (Player.Distance(Game.CursorPos) < 75 && target.Distance(dashVector) > 525)
                return false;

            if (menu.Item("rSpeed").GetValue<bool>() && countEnemiesNearPosition(Game.CursorPos, 1500) < 2 && GetComboDamage(target) > target.Health - 100)
                return true;

            if (GetComboDamage(target) > target.Health && !_rOn)
            {
                if (target.HasBuffOfType(BuffType.Charm))
                    return true;
            }

            if (target.HasBuffOfType(BuffType.Charm) && _rOn)
                return true;

            if (countAlliesNearPosition(Game.CursorPos, 1000) > 2 && _rTimeLeft > 3500)
                return true;

            if (Player.GetSpellDamage(target, SpellSlot.R) * 2 > target.Health)
                return true;

            if (_rOn && _rTimeLeft > 9500)
                return true;

            return false;
        }

        private bool CheckReq(Obj_AI_Hero target)
        {
            if (Player.Distance(Game.CursorPos) < 75)
                return false;

            if (GetComboDamage(target) > target.Health && !_rOn && countEnemiesNearPosition(Game.CursorPos, 1500) < 3)
            {
                if (target.Distance(Game.CursorPos) <= E.Range && E.IsReady())
                {
                    Vector3 dashVector = Player.Position + Vector3.Normalize(Game.CursorPos - Player.Position) * 425;
                    float addedDelay = Player.Distance(dashVector) / 2200;

                    //Game.PrintChat("added delay: " + addedDelay);

                    PredictionOutput pred = GetP(Game.CursorPos, E, target, addedDelay, false);
                    if (pred.Hitchance >= HitChance.Medium && R.IsReady())
                    {
                        //Game.PrintChat("R-E Mode Intiate!");
                        R.Cast(Game.CursorPos, packets());
                        _rTimer = Environment.TickCount - 250;
                        return true;
                    }
                }
            }

            return false;
        }

        private bool IsRActive()
        {
            return Player.HasBuff("AhriTumble", true);
        }

        private int RCount()
        {
            var buff = Player.Buffs.FirstOrDefault(x => x.Name == "AhriTumble");
            if (buff != null)
                return buff.Count;
            return 0;
        }

        private void Farm()
        {
            List<Obj_AI_Base> allMinionsQ = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q.Range,
                MinionTypes.All, MinionTeam.NotAlly);
            List<Obj_AI_Base> allMinionsW = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, W.Range,
                MinionTypes.All, MinionTeam.NotAlly);

            var useQ = menu.Item("UseQFarm").GetValue<bool>();
            var useW = menu.Item("UseWFarm").GetValue<bool>();

            if (useQ && Q.IsReady())
            {
                MinionManager.FarmLocation qPos = Q.GetLineFarmLocation(allMinionsQ);
                if (qPos.MinionsHit >= 3)
                {
                    Q.Cast(qPos.Position, packets());
                }
            }

            if (useW && allMinionsW.Count > 0 && W.IsReady())
                W.Cast();
        }

        public override void Game_OnGameUpdate(EventArgs args)
        {
            //check if player is dead
            if (Player.IsDead) return;

            _rOn = IsRActive();

            if (_rOn)
                _rTimeLeft = Environment.TickCount - _rTimer;

            //ks check
            if (menu.Item("smartKS").GetValue<bool>())
                CheckKs();

            if (menu.Item("ComboActive").GetValue<KeyBind>().Active)
            {
                Combo();
            }
            else
            {
                if (menu.Item("LaneClearActive").GetValue<KeyBind>().Active)
                    Farm();

                if (menu.Item("HarassActive").GetValue<KeyBind>().Active)
                    Harass();

                if (menu.Item("HarassActiveT").GetValue<KeyBind>().Active)
                    Harass();
            }
        }

        public override void Drawing_OnDraw(EventArgs args)
        {
            foreach (Spell spell in SpellList)
            {
                var menuItem = menu.Item(spell.Slot + "Range").GetValue<Circle>();
                if (menuItem.Active)
                    Utility.DrawCircle(Player.Position, spell.Range, menuItem.Color);
            }

            if (menu.Item("cursor").GetValue<Circle>().Active)
                Utility.DrawCircle(Player.Position, 475, Color.Aquamarine);
            if (menu.Item("Draw_Mode").GetValue<Circle>().Active)
            {
                var wts = Drawing.WorldToScreen(Player.Position);

                Drawing.DrawText(wts[0], wts[1], Color.White,
                    menu.Item("charmCombo").GetValue<KeyBind>().Active ? "Require E: On" : "Require E: Off");
            }
        }

        public override void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (!menu.Item("UseGap").GetValue<bool>()) return;

            if (E.IsReady() && gapcloser.Sender.IsValidTarget(E.Range))
                E.Cast(gapcloser.Sender);
        }

        public override void Interrupter_OnPosibleToInterrupt(Obj_AI_Base unit, InterruptableSpell spell)
        {
            if (!menu.Item("UseInt").GetValue<bool>()) return;

            if (Player.Distance(unit) < E.Range && unit != null)
            {
                if (E.GetPrediction(unit).Hitchance >= HitChance.Medium && E.IsReady())
                    E.Cast(unit, packets());
            }
        }
    }
}

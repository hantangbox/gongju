using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

namespace xSaliceReligionAIO.Champions
{
    class Ezreal : Champion
    {
        public Ezreal()
        {
            SetSpells();
            LoadMenu();
        }

        private void SetSpells()
        {
            Q = new Spell(SpellSlot.Q, 1200);
            Q.SetSkillshot(0.25f, 60f, 2000f, true, SkillshotType.SkillshotLine);

            W = new Spell(SpellSlot.W, 1050);
            W.SetSkillshot(0.25f, 80f, 2000f, false, SkillshotType.SkillshotLine);

            E = new Spell(SpellSlot.E, 475);
            E.SetSkillshot(0.25f, 80f, 1600f, false, SkillshotType.SkillshotCircle);

            R = new Spell(SpellSlot.R, 3000);
            R.SetSkillshot(0.99f, 160f, 2000f, false, SkillshotType.SkillshotLine);

            _r2 = new Spell(SpellSlot.R, 3000);
            _r2.SetSkillshot(0.99f, 160f, 2000f, true, SkillshotType.SkillshotLine);
        }

        private void LoadMenu()
        {
            var key = new Menu("键位", "Key");
            {
                key.AddItem(new MenuItem("ComboActive", "连招!").SetValue(new KeyBind(32, KeyBindType.Press)));
                key.AddItem(new MenuItem("HarassActive", "骚扰!").SetValue(new KeyBind("C".ToCharArray()[0], KeyBindType.Press)));
                key.AddItem(new MenuItem("HarassActiveT", "骚扰 (自动)!").SetValue(new KeyBind("N".ToCharArray()[0], KeyBindType.Toggle)));
                key.AddItem(new MenuItem("LaneClearActive", "清线!").SetValue(new KeyBind("V".ToCharArray()[0], KeyBindType.Press)));
                key.AddItem(new MenuItem("R_Nearest_Killable", "R 最近的可击杀目标").SetValue(new KeyBind("R".ToCharArray()[0], KeyBindType.Press)));
                key.AddItem(new MenuItem("Force_R", "强制 R").SetValue(new KeyBind("I".ToCharArray()[0], KeyBindType.Press)));
                //add to menu
                menu.AddSubMenu(key);
            }

            var spellMenu = new Menu("法术 菜单", "SpellMenu");
            {
                var qMenu = new Menu("Q 法术", "QMenu");
                {
                    qMenu.AddItem(new MenuItem("Q_Max_Range", "Q 极限范围").SetValue(new Slider(1050, 500, 1200)));
                    qMenu.AddItem(new MenuItem("Auto_Q_Slow", "自动 Q (减速）").SetValue(true));
                    qMenu.AddItem(new MenuItem("Auto_Q_Immobile", "自动 Q 固定的").SetValue(true));
                    spellMenu.AddSubMenu(qMenu);
                }

                var wMenu = new Menu("W 法术", "WMenu");
                {
                    wMenu.AddItem(
                        new MenuItem("W_Max_Range", "W 极限范围").SetValue(new Slider(900, 500, 1050)));
                    spellMenu.AddSubMenu(wMenu);
                }

                var eMenu = new Menu("E 法术", "EMenu");
                {
                    eMenu.AddItem(new MenuItem("E_On_Killable", "E 敌人可击杀").SetValue(true));
                    eMenu.AddItem(new MenuItem("E_On_Safe", "E 安全 检测").SetValue(true));
                    spellMenu.AddSubMenu(eMenu);
                }

                var rMenu = new Menu("R 法术", "RMenu");
                {
                    rMenu.AddItem(new MenuItem("R_Min_Range", "R 最小距离").SetValue(new Slider(300, 0, 1000)));
                    rMenu.AddItem(new MenuItem("R_Max_Range", "R 最大距离").SetValue(new Slider(2000, 0, 4000)));
                    rMenu.AddItem(new MenuItem("R_Mec", "R 敌人数量").SetValue(new Slider(3, 1, 5)));
                    rMenu.AddItem(new MenuItem("R_Overkill_Check", "抢人头").SetValue(true));

                    rMenu.AddSubMenu(new Menu("禁用 R", "Dont_R"));
                    foreach (var enemy in ObjectManager.Get<Obj_AI_Hero>().Where(enemy => enemy.Team != Player.Team)
                        )
                        rMenu.SubMenu("Dont_R")
                            .AddItem(new MenuItem("Dont_R" + enemy.BaseSkinName, enemy.BaseSkinName).SetValue(false));

                    spellMenu.AddSubMenu(rMenu);
                }

                menu.AddSubMenu(spellMenu);
            }

            var combo = new Menu("连招", "Combo");
            {
                combo.AddItem(new MenuItem("UseQCombo", "使用 Q").SetValue(true));
                combo.AddItem(new MenuItem("UseWCombo", "使用 W").SetValue(true));
                combo.AddItem(new MenuItem("UseECombo", "使用 E").SetValue(true));
                combo.AddItem(new MenuItem("UseRCombo", "使用 R").SetValue(true));
                combo.AddItem(new MenuItem("Ignite", "使用 点燃").SetValue(true));
                combo.AddItem(new MenuItem("Botrk", "使用 破败/弯刀").SetValue(true));
                menu.AddSubMenu(combo);
            }

            var harass = new Menu("骚扰", "Harass");
            {
                harass.AddItem(new MenuItem("UseQHarass", "使用 Q").SetValue(true));
                harass.AddItem(new MenuItem("UseWHarass", "使用 W").SetValue(true));
                AddManaManagertoMenu(harass, "Harass", 30);
                //add to menu
                menu.AddSubMenu(harass);
            }

            var farm = new Menu("清线", "LaneClear");
            {
                farm.AddItem(new MenuItem("UseQFarm", "使用 Q").SetValue(true));
                AddManaManagertoMenu(farm, "LaneClear", 30);
                //add to menu
                menu.AddSubMenu(farm);
            }

            var miscMenu = new Menu("杂项", "Misc");
            {
                miscMenu.AddItem(
                    new MenuItem("Misc_Use_WE", "鼠标方向手动WE").SetValue(new KeyBind("T".ToCharArray()[0],
                        KeyBindType.Press)));
                //add to menu
                menu.AddSubMenu(miscMenu);
            }

            var drawMenu = new Menu("范围", "Drawing");
            {
                drawMenu.AddItem(new MenuItem("Draw_Disabled", "禁用所有").SetValue(false));
                drawMenu.AddItem(new MenuItem("Draw_Q", "范围 Q").SetValue(true));
                drawMenu.AddItem(new MenuItem("Draw_W", "范围 W").SetValue(true));
                drawMenu.AddItem(new MenuItem("Draw_E", "范围 E").SetValue(true));
                drawMenu.AddItem(new MenuItem("Draw_R", "范围 R").SetValue(true));
                drawMenu.AddItem(new MenuItem("Draw_R_Killable", "显示 R 击杀提示").SetValue(true));

                MenuItem drawComboDamageMenu = new MenuItem("Draw_ComboDamage", "显示组合范围连招").SetValue(true);
                MenuItem drawFill = new MenuItem("Draw_Fill", "显示组合填充伤害").SetValue(new Circle(true, Color.FromArgb(90, 255, 169, 4)));
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

                menu.AddSubMenu(drawMenu);
            }
        }

        private float GetComboDamage(Obj_AI_Base target)
        {
            double comboDamage = 0;

            if (Q.IsReady())
                comboDamage += Player.GetSpellDamage(target, SpellSlot.Q);

            if (W.IsReady())
                comboDamage += Player.GetSpellDamage(target, SpellSlot.W);

            if (E.IsReady())
                comboDamage += Player.GetSpellDamage(target, SpellSlot.E);

            if (R.IsReady())
                comboDamage += Player.GetSpellDamage(target, SpellSlot.R);

            if (Items.CanUseItem(Bilge.Id))
                comboDamage += Player.GetItemDamage(target, Damage.DamageItems.Bilgewater);

            if (Items.CanUseItem(Botrk.Id))
                comboDamage += Player.GetItemDamage(target, Damage.DamageItems.Botrk);

            if (IgniteSlot != SpellSlot.Unknown && Player.SummonerSpellbook.CanUseSpell(IgniteSlot) == SpellState.Ready)
                comboDamage += Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);

            return (float)(comboDamage + Player.GetAutoAttackDamage(target) * 1);
        }

        private void Combo()
        {
            UseSpells(menu.Item("UseQCombo").GetValue<bool>(), menu.Item("UseWCombo").GetValue<bool>(),
                menu.Item("UseECombo").GetValue<bool>(), menu.Item("UseRCombo").GetValue<bool>(), "Combo");
        }

        private void Harass()
        {
            UseSpells(menu.Item("UseQHarass").GetValue<bool>(), menu.Item("UseWHarass").GetValue<bool>(),
                false, false, "Harass");
        }

        private void UseSpells(bool useQ, bool useW, bool useE, bool useR, string source)
        {
            if (source == "Harass" && !HasMana("Harass"))
                return;

            if (useQ)
                CastBasicSkillShot(Q, Q.Range, SimpleTs.DamageType.Physical, HitChance.High);
            if (useW)
                CastBasicSkillShot(W, W.Range, SimpleTs.DamageType.Magical, HitChance.High);
            if (source == "Combo")
            {
                var qTarget = SimpleTs.GetTarget(Q.Range, SimpleTs.DamageType.Physical);
                if (qTarget != null)
                {
                    if (GetComboDamage(qTarget) >= qTarget.Health && Ignite_Ready() && menu.Item("Ignite").GetValue<bool>())
                        Use_Ignite(qTarget);

                    if (menu.Item("Botrk").GetValue<bool>())
                    {
                        if (GetComboDamage(qTarget) < qTarget.Health && !qTarget.HasBuffOfType(BuffType.Slow)) Use_Bilge(qTarget);

                        if (GetComboDamage(qTarget) < qTarget.Health && !qTarget.HasBuffOfType(BuffType.Slow))
                            Use_Botrk(qTarget);
                    }
                }
            }
            if (useE)
                Cast_E();
            if (useR)
                Cast_R();
        }
        private void Farm()
        {
            if (!HasMana("LaneClear"))
                return;

            List<Obj_AI_Base> allMinionsQ = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q.Range,
                MinionTypes.All, MinionTeam.NotAlly);

            var useQ = menu.Item("UseQFarm").GetValue<bool>();

            if (useQ && allMinionsQ.Count > 0)
                Q.Cast(allMinionsQ[0], packets());
        }

        private void Cast_E()
        {
            var target = SimpleTs.GetTarget(E.Range + 500, SimpleTs.DamageType.Magical);

            if (E.IsReady() && target != null && menu.Item("E_On_Killable").GetValue<bool>())
            {
                if (Player.GetSpellDamage(target, SpellSlot.E) > target.Health + 25)
                {
                    if (menu.Item("E_On_Safe").GetValue<bool>())
                    {
                        var ePos = E.GetPrediction(target);
                        if (ePos.UnitPosition.CountEnemysInRange(500) < 2)
                            E.Cast(ePos.UnitPosition, packets());
                    }
                    else
                    {
                        E.Cast(target, packets());
                    }
                }
            }
        }

        private void Cast_R()
        {
            var target = SimpleTs.GetTarget(R.Range, SimpleTs.DamageType.Magical);

            if (R.IsReady() && target != null)
            {
                if (menu.Item("Dont_R" + target.BaseSkinName) != null)
                {
                    if (!menu.Item("Dont_R" + target.BaseSkinName).GetValue<bool>())
                    {
                        var minRange = menu.Item("R_Min_Range").GetValue<Slider>().Value;
                        var minHit = menu.Item("R_Mec").GetValue<Slider>().Value;

                        if (Get_R_Dmg(target) > target.Health && Player.Distance(target) > minRange)
                        {
                            R.Cast(target, packets());
                            return;
                        }

                        foreach (var unit in ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsValidTarget(R.Range)).OrderByDescending(GetComboDamage))
                        {
                            var pred = R.GetPrediction(unit, true);
                            if (Player.Distance(unit) > minRange && pred.AoeTargetsHitCount >= minHit && pred.Hitchance >= HitChance.Medium)
                            {
                                R.Cast(unit, packets());
                                return;
                            }
                        }
                    }
                }
            }
        }

        private void Cast_R_Killable()
        {
            foreach (var unit in ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsValidTarget(20000) && !x.IsDead && x.IsEnemy).OrderBy(x => x.Health))
            {
                if (menu.Item("Dont_R" + unit.BaseSkinName) != null)
                {
                    if (!menu.Item("Dont_R" + unit.BaseSkinName).GetValue<bool>())
                    {
                        var health = unit.Health + unit.HPRegenRate * 3 + 25;
                        if (Get_R_Dmg(unit) > health)
                        {
                            R.Cast(unit, packets());
                            return;
                        }
                    }
                }
            }
        }

        private float Get_R_Dmg(Obj_AI_Hero target)
        {
            double dmg = 0;

            dmg += Player.GetSpellDamage(target, SpellSlot.R);

            var rPred = _r2.GetPrediction(target);
            var collisionCount = rPred.CollisionObjects.Count;

            if (collisionCount >= 7)
                dmg = dmg * .3;
            else if (collisionCount != 0)
                dmg = dmg * ((10 - collisionCount) / 10);

            //Game.PrintChat("collision: " + collisionCount);
            return (float)dmg;
        }

        public void Cast_WE()
        {
            if (W.IsReady() && E.IsReady())
            {
                var vec = Player.ServerPosition + Vector3.Normalize(Game.CursorPos - Player.ServerPosition) * E.Range;

                W.Cast(vec);
                E.Cast(vec);
            }
        }

        public void AutoQ()
        {
            var target = SimpleTs.GetTarget(Q.Range, SimpleTs.DamageType.Physical);

            if (target != null)
            {
                if (Q.GetPrediction(target).Hitchance >= HitChance.High && (target.HasBuffOfType(BuffType.Stun) || target.HasBuffOfType(BuffType.Snare)) && menu.Item("Auto_Q_Slow").GetValue<bool>())
                    Q.Cast(target, packets());
                if (target.HasBuffOfType(BuffType.Slow) && menu.Item("Auto_Q_Immobile").GetValue<bool>())
                    Q.Cast(target, packets());
            }
        }

        public void ForceR()
        {
            var target = SimpleTs.GetTarget(R.Range, SimpleTs.DamageType.Magical);
            if (target != null && R.GetPrediction(target).Hitchance >= HitChance.High)
                R.Cast(target, packets());
        }

        public override void Game_OnGameUpdate(EventArgs args)
        {
            //check if player is dead
            if (Player.IsDead) return;

            //adjust range
            if (Q.IsReady())
                Q.Range = menu.Item("Q_Max_Range").GetValue<Slider>().Value;
            if (W.IsReady())
                W.Range = menu.Item("W_Max_Range").GetValue<Slider>().Value;
            if (R.IsReady())
                R.Range = menu.Item("R_Max_Range").GetValue<Slider>().Value;

            if (menu.Item("R_Nearest_Killable").GetValue<KeyBind>().Active)
                Cast_R_Killable();

            if(menu.Item("Force_R").GetValue<KeyBind>().Active)
                ForceR();

            if (menu.Item("Misc_Use_WE").GetValue<KeyBind>().Active)
            {
                Cast_WE();
            }

            AutoQ();

            if (menu.Item("ComboActive").GetValue<KeyBind>().Active)
            {
                Combo();
            }
            else
            {
                if (menu.Item("LaneClearActive").GetValue<KeyBind>().Active)
                    Farm();

                if (menu.Item("HarassActiveT").GetValue<KeyBind>().Active)
                    Harass();

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
            if (menu.Item("Draw_W").GetValue<bool>())
                if (W.Level > 0)
                    Utility.DrawCircle(Player.Position, W.Range, W.IsReady() ? Color.Green : Color.Red);

            if (menu.Item("Draw_E").GetValue<bool>())
                if (E.Level > 0)
                    Utility.DrawCircle(Player.Position, E.Range, E.IsReady() ? Color.Green : Color.Red);

            if (menu.Item("Draw_R").GetValue<bool>())
                if (R.Level > 0)
                    Utility.DrawCircle(Player.Position, R.Range, R.IsReady() ? Color.Green : Color.Red);

            if (menu.Item("Draw_R_Killable").GetValue<bool>() && R.IsReady())
            {
                foreach (var unit in ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsValidTarget(20000) && !x.IsDead && x.IsEnemy).OrderBy(x => x.Health))
                {
                    var health = unit.Health + unit.HPRegenRate * 3 + 25;
                    if (Get_R_Dmg(unit) > health)
                    {
                        Vector2 wts = Drawing.WorldToScreen(unit.Position);
                        Drawing.DrawText(wts[0] - 20, wts[1], Color.White, "KILL!!!");
                    }
                }
            }
        }
    }
}

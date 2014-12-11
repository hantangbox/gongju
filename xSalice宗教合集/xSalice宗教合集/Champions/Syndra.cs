using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

namespace xSaliceReligionAIO.Champions
{
    class Syndra : Champion
    {
        public Syndra()
        {
            SetSpells();
            LoadMenu();
        }

        private Spell _qe;

        private void SetSpells()
        {
            Q = new Spell(SpellSlot.Q, 800);
            Q.SetSkillshot(.6f, 130f, 2000f, false, SkillshotType.SkillshotCircle);

            W = new Spell(SpellSlot.W, 950);
            W.SetSkillshot(.25f, 140f, 1600f, false, SkillshotType.SkillshotCircle);

            E = new Spell(SpellSlot.E, 700);
            E.SetSkillshot(.25f, (float)(45 * 0.5), 2500f, false, SkillshotType.SkillshotCircle);

            R = new Spell(SpellSlot.R, 750);

            _qe = new Spell(SpellSlot.Q, 1250);
            _qe.SetSkillshot(.900f, 115f, 2100f, false, SkillshotType.SkillshotLine);

        }

        private void LoadMenu()
        {
            var key = new Menu("键位", "Key");
            {
                key.AddItem(new MenuItem("ComboActive", "连招!").SetValue(new KeyBind(32, KeyBindType.Press)));
                key.AddItem(new MenuItem("HarassActive", "骚扰!").SetValue(new KeyBind("C".ToCharArray()[0], KeyBindType.Press)));
                key.AddItem(new MenuItem("HarassActiveT", "骚扰 (自动)!").SetValue(new KeyBind("N".ToCharArray()[0], KeyBindType.Toggle)));
                key.AddItem(new MenuItem("LaneClearActive", "清线!").SetValue(new KeyBind("V".ToCharArray()[0], KeyBindType.Press)));
                key.AddItem(new MenuItem("Misc_QE_Mouse", "QE 鼠标方向").SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press)));
                key.AddItem(new MenuItem("qAA", "Q后自动平A").SetValue(new KeyBind("I".ToCharArray()[0], KeyBindType.Toggle)));
                //add to menu
                menu.AddSubMenu(key);
            }

            var spellMenu = new Menu("法术菜单", "SpellMenu");
            {
                var qMenu = new Menu("Q 法术", "QMenu");
                {
                    qMenu.AddItem(new MenuItem("Q_Auto_Immobile", "自动Q不动的").SetValue(true));
                    spellMenu.AddSubMenu(qMenu);
                }

                var wMenu = new Menu("W 法术", "WMenu");
                {
                    wMenu.AddItem(new MenuItem("W_Only_Orb", "只捡起地上球W").SetValue(false));
                    spellMenu.AddSubMenu(wMenu);
                }
                var rMenu = new Menu("R 法术", "RMenu");
                {
                    rMenu.AddItem(new MenuItem("R_Overkill_Check", "击杀提示").SetValue(true));

                    rMenu.AddSubMenu(new Menu("禁用R", "Dont_R"));
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
                combo.AddItem(new MenuItem("UseQECombo", "使用 QE").SetValue(true));
                combo.AddItem(new MenuItem("UseWCombo", "使用 W").SetValue(true));
                combo.AddItem(new MenuItem("UseECombo", "使用 E").SetValue(true));
                combo.AddItem(new MenuItem("UseRCombo", "使用 R").SetValue(true));
                combo.AddItem(new MenuItem("Ignite", "使用 点燃").SetValue(true));
                combo.AddItem(new MenuItem("DFG", "使用 冥火").SetValue(true));
                menu.AddSubMenu(combo);
            }

            var harass = new Menu("骚扰", "Harass");
            {
                harass.AddItem(new MenuItem("UseQHarass", "使用 Q").SetValue(true));
                harass.AddItem(new MenuItem("UseQEHarass", "使用 QE").SetValue(true));
                harass.AddItem(new MenuItem("UseWHarass", "使用 W").SetValue(true));
                harass.AddItem(new MenuItem("UseEHarass", "使用 E").SetValue(true));
                AddManaManagertoMenu(harass, "Harass", 30);
                //add to menu
                menu.AddSubMenu(harass);
            }

            var farm = new Menu("清线", "LaneClear");
            {
                farm.AddItem(new MenuItem("UseQFarm", "使用 Q").SetValue(true));
                farm.AddItem(new MenuItem("UseWFarm", "使用 W").SetValue(true));
                farm.AddItem(new MenuItem("UseEFarm", "使用 E").SetValue(true));
                AddManaManagertoMenu(farm, "LaneClear", 30);
                //add to menu
                menu.AddSubMenu(farm);
            }

            var miscMenu = new Menu("杂项", "Misc");
            {
                miscMenu.AddItem(new MenuItem("QE_Interrupt", "使用 QE 中断法术").SetValue(true));
                miscMenu.AddItem(new MenuItem("E_Gap_Closer", "使用 E 防止突进").SetValue(true));
                miscMenu.AddItem(new MenuItem("smartKS", "使用 智能抢人头").SetValue(true));
                menu.AddSubMenu(miscMenu);
            }

            var drawMenu = new Menu("范围", "Drawing");
            {
                drawMenu.AddItem(new MenuItem("Draw_Disabled", "禁用 所有").SetValue(false));
                drawMenu.AddItem(new MenuItem("Draw_Q", "范围 Q").SetValue(true));
                drawMenu.AddItem(new MenuItem("Draw_QE", "范围 QE").SetValue(true));
                drawMenu.AddItem(new MenuItem("Draw_W", "范围 W").SetValue(true));
                drawMenu.AddItem(new MenuItem("Draw_E", "范围 E").SetValue(true));
                drawMenu.AddItem(new MenuItem("Draw_R", "范围 R").SetValue(true));
                drawMenu.AddItem(new MenuItem("Draw_QE_Line", "显示 QE 路径").SetValue(true));
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

            comboDamage += Get_Ult_Dmg(target);

            if (Ignite_Ready())
                comboDamage += Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);

            return (float)(comboDamage + Player.GetAutoAttackDamage(target));
        }

        private float Get_Ult_Dmg(Obj_AI_Base enemy)
        {
            var damage = 0d;

            if (Items.CanUseItem(DFG.Id) && menu.Item("DFG").GetValue<bool>())
                damage += Player.GetItemDamage(enemy, Damage.DamageItems.Dfg) / 1.2;

            if (R.IsReady())
                damage += (3 + getOrbCount()) * Player.GetSpellDamage(enemy, SpellSlot.R, 1) - 20;

            return (float)damage * (Items.CanUseItem(DFG.Id) ? 1.2f : 1);
        }

        private void Combo()
        {
            UseSpells(menu.Item("UseQCombo").GetValue<bool>(), menu.Item("UseWCombo").GetValue<bool>(),
                menu.Item("UseECombo").GetValue<bool>(), menu.Item("UseRCombo").GetValue<bool>(), menu.Item("UseQECombo").GetValue<bool>(), "Combo");
        }

        private void Harass()
        {
            UseSpells(menu.Item("UseQHarass").GetValue<bool>(), menu.Item("UseWHarass").GetValue<bool>(),
                menu.Item("UseEHarass").GetValue<bool>(), false, menu.Item("UseQEHarass").GetValue<bool>(), "Harass");
        }

        private void UseSpells(bool useQ, bool useW, bool useE, bool useR, bool useQe, string source)
        {
            if (source == "Harass" && !HasMana("Harass"))
                return;

            var useIgnite = menu.Item("Ignite").GetValue<bool>();
            var qTarget = SimpleTs.GetTarget(650, SimpleTs.DamageType.Magical);
            float dmg = 0;
            if (qTarget != null)
                dmg += GetComboDamage(qTarget);

            if (useR)
                Cast_R();

            if(useQ)
                Cast_Q();

            if (useE)
                Cast_E();

            if (useW)
                Cast_W(true);

            if (qTarget != null)
            {
                if (dmg >= qTarget.Health + 25 && Ignite_Ready() && useIgnite)
                    Use_Ignite(qTarget);
            }

            if (useQe)
                Cast_QE();

            
        }

        private void Farm()
        {
            if (!HasMana("LaneClear"))
                return;

            var useQ = menu.Item("UseQFarm").GetValue<bool>();
            var useW = menu.Item("UseWFarm").GetValue<bool>();
            var useE = menu.Item("UseEFarm").GetValue<bool>();

            if (useQ)
                CastBasicFarm(Q);

            if(useW)
                Cast_W(false);

            if(useE)
                CastBasicFarm(E);
        }

        private void SmartKs()
        {
            if (!menu.Item("smartKS").GetValue<bool>())
                return;

             foreach (Obj_AI_Hero target in ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsValidTarget(_qe.Range) && x.IsEnemy && !x.IsDead).OrderByDescending(GetComboDamage))
            {
                //Q
                if (Q.IsKillable(target) && Player.Distance(target) < Q.Range)
                {
                    Q.Cast(target);
                }
                //E
                if (E.IsKillable(target) && Player.Distance(target) < E.Range)
                {
                    E.Cast(target);
                }
                //QE
                if (E.IsKillable(target) && Player.Distance(target) < _qe.Range)
                {
                    Cast_QE(false, target);
                }
            }
        }

        private void Cast_Q()
        {
            var qTarget = SimpleTs.GetTarget(Q.Range, SimpleTs.DamageType.Magical);
            if (qTarget == null)
                return;

            if (Q.IsReady())
                Q.Cast(qTarget, packets());
        }

        private void Cast_W(bool mode)
        {
            if (mode)
            {
                var wTarget = SimpleTs.GetTarget(W.Range, SimpleTs.DamageType.Magical);

                var grabbableObj = Get_Nearest_orb();
                var wToggleState = Player.Spellbook.GetSpell(SpellSlot.W).ToggleState;

                if (wTarget == null)
                    return;

                if (grabbableObj != null && wToggleState == 1)
                {
                    if (Environment.TickCount - W.LastCastAttemptT > Game.Ping && W.IsReady())
                    {
                        if (grabbableObj.Distance(Player) < W.Range)
                        {
                            W.Cast(grabbableObj.ServerPosition);
                            W.LastCastAttemptT = Environment.TickCount + 500;
                            return;
                        }
                    }
                }

                if (wToggleState != 1 && Get_Current_Orb() != null)
                {
                    //W.UpdateSourcePosition(Get_Current_Orb().ServerPosition, Get_Current_Orb().ServerPosition);
                    W.From = Get_Current_Orb().ServerPosition;

                    if (Player.Distance(wTarget) < E.Range - 100)
                    {
                        if (wToggleState != 1 && W.IsReady() &&
                            Environment.TickCount - W.LastCastAttemptT > -300 + Game.Ping)
                        {
                            W.Cast(wTarget);
                            return;
                        }
                    }

                    if (W.IsReady())
                    {
                        W.Cast(wTarget);
                    }
                }
            }
            else
            {
                var allMinionsW = MinionManager.GetMinions(Player.ServerPosition, W.Range + W.Width + 20, MinionTypes.All, MinionTeam.NotAlly);

                if (allMinionsW.Count < 2)
                    return;

                var grabbableObj = Get_Nearest_orb();
                var wToggleState = Player.Spellbook.GetSpell(SpellSlot.W).ToggleState;

                if (wToggleState == 1 && Environment.TickCount - W.LastCastAttemptT > Game.Ping && W.IsReady() &&
                        grabbableObj != null)
                {
                    W.Cast(grabbableObj.ServerPosition);
                    W.LastCastAttemptT = Environment.TickCount + 1000;
                    return;
                }

                W.From = Get_Current_Orb().ServerPosition;
                var farmLocation = W.GetCircularFarmLocation(allMinionsW);

                if (farmLocation.MinionsHit >= 1)
                    W.Cast(farmLocation.Position);
            }
        }

        private void Cast_E()
        {
            if (getOrbCount() <= 0)
                return;

            var target = SimpleTs.GetTarget(_qe.Range + 100, SimpleTs.DamageType.Magical);
            if (target == null || Environment.TickCount - W.LastCastAttemptT < Game.Ping)
                return;

            foreach (var orb in getOrb().Where(x => Player.Distance(x) < E.Range && x != null))
            {
                double rangeLeft = 100 + (-0.6 * Player.Distance(orb.ServerPosition) + 950);
                var startPos = orb.ServerPosition - Vector3.Normalize(orb.ServerPosition - Player.ServerPosition) * 100;
                var endPos = startPos + Vector3.Normalize(startPos - Player.ServerPosition) * (float)rangeLeft;

                _qe.Delay = E.Delay + Player.Distance(orb) / E.Speed + target.Distance(orb) / _qe.Speed;
                _qe.From = startPos;

                var targetPos = _qe.GetPrediction(target);

                var projection = targetPos.UnitPosition.To2D().ProjectOn(startPos.To2D(), endPos.To2D());

                if (!projection.IsOnSegment || targetPos.Hitchance < HitChance.Medium || !(projection.LinePoint.Distance(targetPos.UnitPosition.To2D()) < _qe.Width + target.BoundingRadius))
                    return;

                E.Cast(startPos, packets());
                W.LastCastAttemptT = Environment.TickCount + 500;
                return;
            }
        }

        private void Cast_R()
        {
            var rTarget = SimpleTs.GetTarget(R.Level > 2 ? R.Range : 675, SimpleTs.DamageType.Magical);

            if (rTarget == null)
                return;
            if (menu.Item("Dont_R" + rTarget.ChampionName) == null)
                return;
            if (menu.Item("Dont_R" + rTarget.ChampionName).GetValue<bool>())
                return;
            if (menu.Item("R_Overkill_Check").GetValue<bool>())
            {
                if (Player.GetSpellDamage(rTarget, SpellSlot.Q) - 25 > rTarget.Health)
                {
                    return;
                }
            }

            if (Get_Ult_Dmg(rTarget) > rTarget.Health - 20 && rTarget.Distance(Player) < R.Range)
            {
                if (Items.CanUseItem(DFG.Id) && menu.Item("DFG").GetValue<bool>())
                    Use_DFG(rTarget);

                R.CastOnUnit(rTarget);
            }
        }

        private void Cast_QE(bool usePred = true , Obj_AI_Base target = null)
        {
            var qeTarget = SimpleTs.GetTarget(_qe.Range, SimpleTs.DamageType.Magical);
            if (qeTarget == null || !Q.IsReady() || !E.IsReady())
                return;

            var from = Prediction.GetPrediction(qeTarget, Q.Delay + E.Delay).UnitPosition;
            var startPos = Player.ServerPosition + Vector3.Normalize(from - Player.ServerPosition) * (E.Range - 100);
            double rangeLeft = 100 + (-0.6 * Player.Distance(startPos) + 950);
            var endPos = startPos + Vector3.Normalize(startPos - Player.ServerPosition) * (float)rangeLeft;

            _qe.From = startPos;
            _qe.Delay = Q.Delay + E.Delay + Player.Distance(from) / E.Speed;

            var qePred = _qe.GetPrediction(qeTarget);
            var projection = qePred.UnitPosition.To2D().ProjectOn(startPos.To2D(), endPos.To2D());

            if (!projection.IsOnSegment || !(projection.LinePoint.Distance(qePred.UnitPosition.To2D()) < _qe.Width + qeTarget.BoundingRadius))
                return;

            if (qePred.Hitchance >= HitChance.Medium || !usePred)
            {
                Q.Cast(startPos, packets());
                W.LastCastAttemptT = Environment.TickCount + 500;
                _qe.LastCastAttemptT = Environment.TickCount;
            }
        }

        private void CastQeMouse()
        {
            foreach (var enemy in ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsValidTarget(_qe.Range)))
                if (Game.CursorPos.Distance(enemy.ServerPosition) < 300)
                    Cast_QE(false, enemy);
        }

        private void QImmobile()
        {
            var qTarget = SimpleTs.GetTarget(Q.Range, SimpleTs.DamageType.Magical);
            if (!menu.Item("Q_Auto_Immobile").GetValue<bool>() || qTarget == null)
                return;
            if (Q.GetPrediction(qTarget).Hitchance == HitChance.Immobile)
                Q.Cast(qTarget);
        }

        public override void Game_OnGameUpdate(EventArgs args)
        {

            if(R.IsReady())
                R.Range = R.Level == 3 ? 750f : 675f;
            if(E.IsReady())
                E.Width = E.Level == 5 ? 45f : (float)(45 * 0.5);

            if (menu.Item("Misc_QE_Mouse").GetValue<KeyBind>().Active)
            {
                CastQeMouse();
            }

            SmartKs();

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

            QImmobile();
        }

        public override void Drawing_OnDraw(EventArgs args)
        {
            if (menu.Item("Draw_Disabled").GetValue<bool>())
                return;

            xSLxOrbwalker.EnableDrawing();
            if (menu.Item("Draw_Q").GetValue<bool>())
                if (Q.Level > 0)
                    Utility.DrawCircle(Player.Position, Q.Range, Q.IsReady() ? Color.Green : Color.Red);
            if (menu.Item("Draw_QE").GetValue<bool>())
                if (Q.Level > 0 && E.Level > 0)
                    Utility.DrawCircle(Player.Position, _qe.Range, Q.IsReady() && E.IsReady() ? Color.Green : Color.Red);
            if (menu.Item("Draw_W").GetValue<bool>())
                if (W.Level > 0)
                    Utility.DrawCircle(Player.Position, W.Range, W.IsReady() ? Color.Green : Color.Red);

            if (menu.Item("Draw_E").GetValue<bool>())
                if (E.Level > 0)
                    Utility.DrawCircle(Player.Position, E.Range, E.IsReady() ? Color.Green : Color.Red);

            if (menu.Item("Draw_R").GetValue<bool>())
                if (R.Level > 0)
                    Utility.DrawCircle(Player.Position, R.Range, R.IsReady() ? Color.Green : Color.Red);

            //draw EQ
            if (menu.Item("Draw_QE_Line").GetValue<bool>())
            {
                var qeTarget = SimpleTs.GetTarget(_qe.Range, SimpleTs.DamageType.Magical);
                if (qeTarget == null || !Q.IsReady() || !E.IsReady())
                    return;

                var from = Prediction.GetPrediction(qeTarget, Q.Delay + E.Delay).UnitPosition;
                var startPos = Player.ServerPosition + Vector3.Normalize(from - Player.ServerPosition) * (E.Range - 100);
                double rangeLeft = 100 + (-0.6 * Player.Distance(startPos) + 950);
                var endPos = startPos + Vector3.Normalize(startPos - Player.ServerPosition) * (float)rangeLeft;

                _qe.From = startPos;
                _qe.Delay = Q.Delay + E.Delay + Player.Distance(from) / E.Speed;

                var qePred = _qe.GetPrediction(qeTarget);
                var projection = qePred.UnitPosition.To2D().ProjectOn(startPos.To2D(), endPos.To2D());

                if (!projection.IsOnSegment || !(projection.LinePoint.Distance(qePred.UnitPosition.To2D()) < _qe.Width + qeTarget.BoundingRadius))
                    return;

                if (qePred.Hitchance >= HitChance.Medium)
                {
                    Vector2 wtsPlayer = Drawing.WorldToScreen(Player.Position);
                    Vector2 wtsPred = Drawing.WorldToScreen(endPos);
                    Utility.DrawCircle(startPos, Q.Width/2, Color.Aquamarine);
                    Utility.DrawCircle(endPos, Q.Width/2, Color.SpringGreen);
                    Drawing.DrawLine(wtsPlayer, wtsPred, 1, Color.LawnGreen);
                }

            }
            //end draw EQ--------------------------
            /*
            if (getOrbCount() <= 0)
                return;
            var target = SimpleTs.GetTarget(_qe.Range + 100, SimpleTs.DamageType.Magical);
            if (target == null)
                return;

            foreach (var orb in getOrb().Where(x => Player.Distance(x) < E.Range && x != null))
            {
                double rangeLeft = 100 + (-0.6 * Player.Distance(orb.ServerPosition) + 950);
                var startPos = orb.ServerPosition - Vector3.Normalize(orb.ServerPosition - Player.ServerPosition) * 100;
                var endPos = startPos + Vector3.Normalize(startPos - Player.ServerPosition) * (float)rangeLeft;

                _qe.Delay = E.Delay + Player.Distance(orb)/E.Speed + target.Distance(orb)/_qe.Speed;
                _qe.From = startPos;

                var targetPos = _qe.GetPrediction(target);

                var projection = targetPos.UnitPosition.To2D().ProjectOn(startPos.To2D(), endPos.To2D());

                if (!projection.IsOnSegment || targetPos.Hitchance < HitChance.Medium || !(projection.LinePoint.Distance(targetPos.UnitPosition.To2D()) < _qe.Width + target.BoundingRadius))
                    return;
               
                Vector2 wtsPlayer = Drawing.WorldToScreen(Player.Position);
                Vector2 wtsPred = Drawing.WorldToScreen(endPos);
                Utility.DrawCircle(startPos, Q.Width / 2, Color.Aquamarine);
                Utility.DrawCircle(endPos, Q.Width / 2, Color.SpringGreen);
                Drawing.DrawLine(wtsPlayer, wtsPred, 1, Color.LawnGreen);
            }*/

            if (menu.Item("Draw_R_Killable").GetValue<bool>() && R.IsReady())
            {
                foreach (var wts in from unit in ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsValidTarget(2000) && !x.IsDead && x.IsEnemy).OrderByDescending(GetComboDamage)
                        let health = unit.Health + unit.HPRegenRate + 10
                        where Get_Ult_Dmg(unit) > health
                        select Drawing.WorldToScreen(unit.Position))
                {
                    Drawing.DrawText(wts[0] - 20, wts[1], Color.White, "KILL!!!");
                }
            }
        }

        private int getOrbCount()
        {
            return
                ObjectManager.Get<Obj_AI_Minion>().Count(obj => obj.IsValid && obj.Team == ObjectManager.Player.Team && obj.Name == "Seed");
        }

        private IEnumerable<Obj_AI_Minion> getOrb()
        {
            return ObjectManager.Get<Obj_AI_Minion>().Where(obj => obj.IsValid && obj.Team == ObjectManager.Player.Team && obj.Name == "Seed").ToList();
        }

        private Obj_AI_Minion Get_Nearest_orb()
        {
            var orb =
                ObjectManager.Get<Obj_AI_Minion>()
                    .Where(obj => obj.IsValid && obj.Team == ObjectManager.Player.Team && obj.Name == "Seed")
                    .ToList()
                    .OrderBy(x => Player.Distance(x))
                    .FirstOrDefault();
            if (orb != null)
                return orb;

            if (menu.Item("W_Only_Orb").GetValue<bool>())
                return null;
            var minion = ObjectManager.Get<Obj_AI_Minion>().FirstOrDefault(x => x.IsValidTarget(W.Range));

            return minion;
        }

        private Obj_AI_Base Get_Current_Orb()
        {
            var orb = ObjectManager.Get<Obj_AI_Minion>().FirstOrDefault(x => x.Team == Player.Team && x.Name == "Seed" && !x.IsTargetable);

            if (orb != null)
                return orb;

            var minion = ObjectManager.Get<Obj_AI_Minion>().FirstOrDefault(x => x.IsInvulnerable && x.Name != "Seed" && x.Name != "k");

            return minion;
        }

        public override void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (!menu.Item("E_Gap_Closer").GetValue<bool>())
                return;

            if (!E.IsReady() || !gapcloser.Sender.IsValidTarget(E.Range))
                return;
            E.Cast(gapcloser.Sender, packets());
            W.LastCastAttemptT = Environment.TickCount + 500;
        }


        public override void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base unit, GameObjectProcessSpellCastEventArgs spell)
        {
            if (!unit.IsMe || !E.IsReady() || (spell.SData.Name != "SyndraQ") ||
                Environment.TickCount - _qe.LastCastAttemptT >= 300)
                return;
            E.Cast(spell.End, packets());
            W.LastCastAttemptT = Environment.TickCount + 500;
        }
        public override void Interrupter_OnPosibleToInterrupt(Obj_AI_Base unit, InterruptableSpell spell)
        {
            if (spell.DangerLevel < InterruptableDangerLevel.Medium || unit.IsAlly)
                return;

            if (menu.Item("QE_Interrupt").GetValue<bool>() && unit.IsValidTarget(_qe.Range))
                Cast_QE(false, unit);
        }

        public override void Game_OnGameProcessPacket(GamePacketEventArgs args)
        {
            GamePacket g = new GamePacket(args.PacketData);
            if (g.Header != 0xFE)
                return;

            if (menu.Item("qAA").GetValue<KeyBind>().Active)
            {
                if (Packet.MultiPacket.OnAttack.Decoded(args.PacketData).Type == Packet.AttackTypePacket.TargetedAA)
                {
                    g.Position = 1;
                    var k = ObjectManager.GetUnitByNetworkId<Obj_AI_Base>(g.ReadInteger());
                    if (k is Obj_AI_Hero && k.IsEnemy)
                    {
                        if (Vector3.Distance(k.Position, Player.Position) <= Q.Range)
                        {
                            Q.Cast(k.Position, packets());
                        }
                    }
                }
            }
        }
    }
}

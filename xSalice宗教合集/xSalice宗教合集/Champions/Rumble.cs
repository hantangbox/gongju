using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
using ObjectManager = LeagueSharp.ObjectManager;

namespace xSaliceReligionAIO.Champions
{
    class Rumble : Champion
    {
        public Rumble()
        {
            LoadSpells();
            LoadMenu();
        }

        private void LoadSpells()
        {
            //intalize spell
            Q = new Spell(SpellSlot.Q, 500);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E, 850);
            R = new Spell(SpellSlot.R, 1700);
            _r2 = new Spell(SpellSlot.R, 800);

            E.SetSkillshot(0.45f, 90, 1200, true, SkillshotType.SkillshotLine);
            R.SetSkillshot(0.25f, 110, 2500, false, SkillshotType.SkillshotLine);
            _r2.SetSkillshot(0.25f, 110, 2600, false, SkillshotType.SkillshotLine);
        }

        private void LoadMenu()
        {
            var key = new Menu("键位", "Key");
            {
                key.AddItem(new MenuItem("ComboActive", "连招!").SetValue(new KeyBind(32, KeyBindType.Press)));
                key.AddItem(new MenuItem("HarassActive", "骚扰!").SetValue(new KeyBind("C".ToCharArray()[0], KeyBindType.Press)));
                key.AddItem(new MenuItem("HarassActiveT", "骚扰 (自动)!").SetValue(new KeyBind("N".ToCharArray()[0], KeyBindType.Toggle)));
                key.AddItem(new MenuItem("LaneClearActive", "清线!").SetValue(new KeyBind("V".ToCharArray()[0], KeyBindType.Press)));
                key.AddItem(new MenuItem("LastHitE", "使用 E 补刀!").SetValue(new KeyBind("A".ToCharArray()[0], KeyBindType.Press)));
                key.AddItem(new MenuItem("UseMecR", "强制使用R").SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press)));
                //add to menu
                menu.AddSubMenu(key);
            }

            var spellMenu = new Menu("法术 菜单", "SpellMenu");
            {
                var qMenu = new Menu("Q 法术", "QMenu");
                {
                    qMenu.AddItem(new MenuItem("Q_Auto_Heat", "使用 Q （过热）").SetValue(true));
                    qMenu.AddItem(new MenuItem("Q_Over_Heat", "智能Q抢人头").SetValue(true));
                    spellMenu.AddSubMenu(qMenu);
                }

                var wMenu = new Menu("W 法术", "WMenu");
                {
                    wMenu.AddItem(new MenuItem("W_Auto_Heat", "使用 W （过热）").SetValue(true));
                    wMenu.AddItem(new MenuItem("W_Always", "连招|骚扰总是使用W").SetValue(false));
                    wMenu.AddItem(new MenuItem("W_Block_Spell", "连招优先使用W").SetValue(true));
                    spellMenu.AddSubMenu(wMenu);
                }

                var eMenu = new Menu("E 法术", "EMenu");
                {
                    eMenu.AddItem(new MenuItem("E_Auto_Heat", "使用 E （过热）").SetValue(false));
                    eMenu.AddItem(new MenuItem("E_Over_Heat", "智能E抢人头").SetValue(true));
                    spellMenu.AddSubMenu(eMenu);
                }

                var rMenu = new Menu("R 法术", "RMenu");
                {
                    rMenu.AddItem(new MenuItem("R_If_Enemy_Count", "自动R|敌人数量").SetValue(new Slider(4, 1, 6)));
                    rMenu.AddItem(new MenuItem("R_If_Enemy_Count_Combo", "连招使用R|敌人数量").SetValue(new Slider(3, 1, 6)));
                    spellMenu.AddSubMenu(rMenu);
                }

                menu.AddSubMenu(spellMenu);
            }

            var combo = new Menu("连招", "Combo");
            {
                combo.AddItem(new MenuItem("UseQCombo", "使用 Q").SetValue(true));
                combo.AddItem(new MenuItem("UseWCombo", "使用 W").SetValue(true));
                combo.AddItem(new MenuItem("UseECombo", "使用 E").SetValue(true));
                combo.AddItem(new MenuItem("qHit", "E 命中率").SetValue(new Slider(3, 1, 3)));
                combo.AddItem(new MenuItem("UseRCombos", "使用 R").SetValue(false));
                combo.AddItem(new MenuItem("Ignite", "使用 点燃").SetValue(true));
                //add to menu
                menu.AddSubMenu(combo);
            }

            var harass = new Menu("骚扰", "Harass");
            {
                harass.AddItem(new MenuItem("UseQHarass", "使用 Q").SetValue(false));
                harass.AddItem(new MenuItem("UseWHarass", "使用 W").SetValue(false));
                harass.AddItem(new MenuItem("UseEHarass", "使用 E").SetValue(true));
                harass.AddItem(new MenuItem("qHit2", "E 命中率").SetValue(new Slider(3, 1, 4)));
                //add to menu
                menu.AddSubMenu(harass);
            }

            var farm = new Menu("清线", "LaneClear");
            {
                farm.AddItem(new MenuItem("UseQFarm", "使用 Q").SetValue(true));
                farm.AddItem(new MenuItem("UseEFarm", "使用 E").SetValue(true));
                //add to menu
                menu.AddSubMenu(farm);
            }

            var miscMenu = new Menu("杂项", "Misc");
            {
                miscMenu.AddItem(new MenuItem("Stay_Danger", "保持过热状态").SetValue(new KeyBind("I".ToCharArray()[0], KeyBindType.Toggle)));
                miscMenu.AddItem(new MenuItem("E_Gap_Closer", "使用 E 防止突进").SetValue(true));
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
                drawMenu.AddItem(new MenuItem("Draw_R_Pred", "显示 R最佳路径").SetValue(true));

                MenuItem drawComboDamageMenu = new MenuItem("Draw_ComboDamage", "显示组合连招伤害").SetValue(true);
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

                //add to menu
                menu.AddSubMenu(drawMenu);
            }
        }

        private float GetComboDamage(Obj_AI_Base target)
        {
            double comboDamage = 0;

            if (Q.IsReady())
                comboDamage += GetCurrentHeat() > 50 ? Player.GetSpellDamage(target, SpellSlot.Q) * 2 : Player.GetSpellDamage(target, SpellSlot.Q);

            if (E.IsReady())
                comboDamage += GetCurrentHeat() > 50 ? Player.GetSpellDamage(target, SpellSlot.E) * 1.5: Player.GetSpellDamage(target, SpellSlot.E);

            if (R.IsReady())
                comboDamage += Player.GetSpellDamage(target, SpellSlot.R) * 3;

            if (Ignite_Ready())
                comboDamage += Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);

            return (float)(comboDamage + Player.GetAutoAttackDamage(target));
        }

        private void Combo()
        {
            UseSpells(menu.Item("UseQCombo").GetValue<bool>(), menu.Item("UseWCombo").GetValue<bool>(),
                menu.Item("UseECombo").GetValue<bool>(), menu.Item("UseRCombos").GetValue<bool>(), "Combo");
        }

        private void Harass()
        {
            UseSpells(menu.Item("UseQHarass").GetValue<bool>(), menu.Item("UseWHarass").GetValue<bool>(),
                menu.Item("UseEHarass").GetValue<bool>(), false, "Harass");
        }

        private void UseSpells(bool useQ, bool useW, bool useE, bool useR, string source)
        {
            var target = SimpleTs.GetTarget(E.Range, SimpleTs.DamageType.Magical);

            if (target == null)
                return;

            if (useQ && ShouldQ(target))
                Q.Cast(target);

            if (useW && menu.Item("W_Always").GetValue<bool>() && W.IsReady())
                W.Cast(packets());

            if (GetComboDamage(target) >= target.Health + 150 && Ignite_Ready() && menu.Item("Ignite").GetValue<bool>() && source == "Combo")
                Use_Ignite(target);

            if (useE && ShouldE(target, source))
                E.Cast(target, packets());

            if (useR && GetComboDamage(target) > target.Health)
                CastSingleR();
        }

        private void Farm()
        {
            if (!Orbwalking.CanMove(40))
                return;

            List<Obj_AI_Base> allMinionsQ = MinionManager.GetMinions(Player.ServerPosition, Q.Range,
                MinionTypes.All, MinionTeam.NotAlly);
            List<Obj_AI_Base> allMinionsE = MinionManager.GetMinions(Player.ServerPosition, E.Range,
                MinionTypes.All, MinionTeam.NotAlly);

            var useQ = menu.Item("UseQFarm").GetValue<bool>();
            var useE = menu.Item("UseEFarm").GetValue<bool>();

            if (useQ && allMinionsQ.Count > 0)
                Q.Cast(allMinionsQ[0]);

            if (useE && allMinionsE.Count > 0)
                E.Cast(allMinionsE[0]);
        }

        private void LastHit()
        {
            if (!Orbwalking.CanMove(40))
                return;

            List<Obj_AI_Base> allMinionsE = MinionManager.GetMinions(Player.ServerPosition, E.Range,
                MinionTypes.All, MinionTeam.NotAlly);

            if (allMinionsE.Count > 0 && E.IsReady())
            {
                foreach (var minion in allMinionsE)
                {
                    if(E.IsKillable(minion))
                        E.Cast(minion);
                }
            }

        }

        private bool ShouldQ(Obj_AI_Hero target)
        {
            if (!Q.IsReady())
                return false;

            if (Player.Distance(target) > Q.Range)
                return false;

            if (!menu.Item("Q_Over_Heat").GetValue<bool>() && GetCurrentHeat() > 80)
                return false;

            if (GetCurrentHeat() > 80 && !(Player.GetSpellDamage(target, SpellSlot.Q, 1) + Player.GetAutoAttackDamage(target) * 2 > target.Health))
                return false;

            return true;
        }

        private bool ShouldE(Obj_AI_Hero target, string source)
        {
            if (!E.IsReady())
                return false;

            if (Player.Distance(target) > E.Range)
                return false;

            if(E.GetPrediction(target).Hitchance < GetHitchance(source))

            if (!menu.Item("E_Over_Heat").GetValue<bool>() && GetCurrentHeat() > 80)
                return false;

            if (GetCurrentHeat() > 80 && !(Player.GetSpellDamage(target, SpellSlot.E, 1) + Player.GetAutoAttackDamage(target) * 2 > target.Health))
                return false;

            return true;
        }

        private void StayInDangerZone()
        {
            if (Utility.InFountain() || IsRecalling()) 
                return;

            if (GetCurrentHeat() < 31 && W.IsReady() && menu.Item("W_Auto_Heat").GetValue<bool>())
            {
                W.Cast(packets());
                return;
            }

            if (GetCurrentHeat() < 31 && Q.IsReady() && menu.Item("Q_Auto_Heat").GetValue<bool>())
            {
                var enemy = ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsEnemy).OrderBy(x => Player.Distance(x)).FirstOrDefault();

                if(enemy != null)
                    Q.Cast(enemy.ServerPosition, packets());
                return;
            }

            if (GetCurrentHeat() < 31 && E.IsReady() && menu.Item("E_Auto_Heat").GetValue<bool>())
            { 
                var enemy = ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsEnemy && !x.IsDead).OrderBy(x => Player.Distance(x)).FirstOrDefault();

                if (enemy != null)
                    E.Cast(enemy, packets());
            }

        }

        private float GetCurrentHeat()
        {
            return Player.Mana;
        }

        private void CastSingleR()
        {
            var target = SimpleTs.GetTarget(R.Range, SimpleTs.DamageType.Magical);

            if (target == null)
                return;

            var vector1 = target.ServerPosition - Vector3.Normalize(target.ServerPosition - Player.ServerPosition) * 300;

            _r2.UpdateSourcePosition(vector1, vector1);

            var pred = _r2.GetPrediction(target, true);

            if (Player.Distance(target) < 400)
            {
                var midpoint = (Player.ServerPosition + pred.UnitPosition)/2;

                vector1 = midpoint + Vector3.Normalize(pred.UnitPosition - Player.ServerPosition) * 800;
                var vector2 = midpoint - Vector3.Normalize(pred.UnitPosition - Player.ServerPosition) * 300;

                if(!IsPassWall(pred.UnitPosition, vector1) && !IsPassWall(pred.UnitPosition, vector2))
                    CastR(vector1, vector2);
            }
            else if (!IsPassWall(pred.UnitPosition, vector1) && !IsPassWall(pred.UnitPosition, pred.CastPosition))
            {
                //wall check
                if(pred.Hitchance >= HitChance.Medium)
                    CastR(vector1, pred.CastPosition);
            }
        }

        private void CastMecR(bool forceUlt)
        {
            //check if only one target
            if (countEnemiesNearPosition(Player.ServerPosition, R.Range + 500) < 2 && forceUlt)
            {
                CastSingleR();
                return;
            }

            int maxHit = 0;
            Vector3 start = Vector3.Zero;
            Vector3 end = Vector3.Zero;

            //loop one
            foreach (var target in ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsValidTarget(R.Range)).OrderByDescending(GetComboDamage))
            {
                //loop 2
                foreach (var enemy in ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsValidTarget(R.Range + 1000) && x.NetworkId != target.NetworkId && x.Distance(target) < 900)
                    .OrderByDescending(x => x.Distance(target)))
                {
                    int hit = 2;

                    var targetPred = Prediction.GetPrediction(target, .25f);
                    var enemyPred = Prediction.GetPrediction(enemy, .25f);

                    var midpoint = (enemyPred.CastPosition + targetPred.CastPosition) / 2;

                    var startpos = midpoint + Vector3.Normalize(enemyPred.CastPosition - targetPred.CastPosition) * 600;
                    var endPos = midpoint - Vector3.Normalize(enemyPred.CastPosition - targetPred.CastPosition) * 600;

                    if (!IsPassWall(midpoint, startpos) && !IsPassWall(midpoint, endPos) && countEnemiesNearPosition(Player.ServerPosition, R.Range + 1000) > 2)
                    {
                        //loop 3
                        foreach (var enemy2 in ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsValidTarget(R.Range + 1000) && x.NetworkId != target.NetworkId && x.NetworkId != enemy.NetworkId && x.Distance(target) < 1000))
                        {
                            var enemy2Pred = Prediction.GetPrediction(enemy2, .25f);
                            Object[] obj = VectorPointProjectionOnLineSegment(startpos.To2D(), endPos.To2D(), enemy2Pred.CastPosition.To2D());
                            var isOnseg = (bool)obj[2];
                            var pointLine = (Vector2)obj[1];

                            if (pointLine.Distance(enemy2Pred.CastPosition.To2D()) < 110 && isOnseg)
                            {
                                hit++;
                            }
                        }
                    }

                    if (hit > maxHit && hit > 1)
                    {
                        maxHit = hit;
                        start = startpos;
                        end = endPos;
                    }
                }
            }

                if (start != Vector3.Zero && end != Vector3.Zero && R.IsReady())
                {
                    if (forceUlt)
                        CastR(start, end);
                    if (menu.Item("ComboActive").GetValue<KeyBind>().Active && maxHit >= menu.Item("R_If_Enemy_Count_Combo").GetValue<Slider>().Value)
                        CastR(start, end);
                    if (maxHit >= menu.Item("R_If_Enemy_Count").GetValue<Slider>().Value)
                        CastR(start, end);
                }
            

        }

        private void CastR(Vector3 source, Vector3 destination)
        {
            CastR(source.To2D(), destination.To2D());
        }

        private void CastR(Vector2 source, Vector2 destination)
        {
            Packet.C2S.Cast.Encoded(new Packet.C2S.Cast.Struct(0, SpellSlot.R, Player.NetworkId, source.X, source.Y, destination.X, destination.Y)).Send();
        }

        public override void Game_OnGameUpdate(EventArgs args)
        {
            if (Player.IsDead)
                return;

            CastMecR(false);

            if (menu.Item("ComboActive").GetValue<KeyBind>().Active)
            {
                Combo();
            }
            else
            {
                if (menu.Item("UseMecR").GetValue<KeyBind>().Active)
                    CastMecR(true);

                if (menu.Item("LastHitE").GetValue<KeyBind>().Active)
                    LastHit();

                if (menu.Item("LaneClearActive").GetValue<KeyBind>().Active)
                    Farm();

                if (menu.Item("HarassActiveT").GetValue<KeyBind>().Active)
                        Harass();

                if (menu.Item("HarassActive").GetValue<KeyBind>().Active)
                    Harass();
            }
            //stay in dangerzone
            if(menu.Item("Stay_Danger").GetValue<KeyBind>().Active)
                StayInDangerZone();
        }

        public override void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base unit, GameObjectProcessSpellCastEventArgs args)
        {
            if (unit.IsEnemy && unit.Type == GameObjectType.obj_AI_Hero && W.IsReady() && menu.Item("W_Block_Spell").GetValue<bool>())
            {
                if (Player.Distance(args.End) < 400 && GetCurrentHeat() < 70)
                {
                    //Game.PrintChat("shielding");
                    W.Cast(packets());
                }
            }

            /*
            if (!unit.IsMe)
                return;
            
            SpellSlot castedSlot = Player.GetSpellSlot(args.SData.Name, false);

            if (castedSlot == SpellSlot.E)
            {
                E.LastCastAttemptT = Environment.TickCount;
            }
            */
        }

        public override void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (!menu.Item("E_Gap_Closer").GetValue<bool>()) return;

            if (E.IsReady() && gapcloser.Sender.IsValidTarget(E.Range))
                E.Cast(gapcloser.Sender);
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
                    Utility.DrawCircle(Player.Position, W.Range - 2, W.IsReady() ? Color.Green : Color.Red);

            if (menu.Item("Draw_E").GetValue<bool>())
                if (E.Level > 0)
                    Utility.DrawCircle(Player.Position, E.Range, E.IsReady() ? Color.Green : Color.Red);

            if (menu.Item("Draw_R").GetValue<bool>())
                if (R.Level > 0)
                    Utility.DrawCircle(Player.Position, R.Range, R.IsReady() ? Color.Green : Color.Red);


            if (menu.Item("Draw_R_Pred").GetValue<bool>() && R.IsReady())
            {
                if (countEnemiesNearPosition(Player.ServerPosition, R.Range + 500) < 2)
                {
                    var target = SimpleTs.GetTarget(R.Range, SimpleTs.DamageType.Magical);

                    if (target == null)
                        return;

                    var vector1 = target.ServerPosition - Vector3.Normalize(target.ServerPosition - Player.ServerPosition) * 300;

                    _r2.UpdateSourcePosition(vector1, vector1);

                    var pred = _r2.GetPrediction(target, true);

                    var midpoint = (Player.ServerPosition + pred.UnitPosition) / 2;
                    var vector2 = midpoint - Vector3.Normalize(pred.UnitPosition - Player.ServerPosition) * 300;

                    if (Player.Distance(target) < 400)
                    {
                        vector1 = midpoint + Vector3.Normalize(pred.UnitPosition - Player.ServerPosition)*800;
                        if (!IsPassWall(pred.UnitPosition, vector1) && !IsPassWall(pred.UnitPosition, vector2))
                        {
                            Vector2 wts = Drawing.WorldToScreen(Player.Position);
                            Drawing.DrawText(wts[0], wts[1], Color.Wheat, "Hit: " + 1);

                            Vector2 wtsPlayer = Drawing.WorldToScreen(vector1);
                            Vector2 wtsPred = Drawing.WorldToScreen(vector2);

                            Drawing.DrawLine(wtsPlayer, wtsPred, 1, Color.Wheat);
                            Utility.DrawCircle(vector1, 50, Color.Aqua);
                            Utility.DrawCircle(vector2, 50, Color.Yellow);
                            Utility.DrawCircle(pred.UnitPosition, 50, Color.Red);
                        }
                    }
                    else if (!IsPassWall(pred.UnitPosition, vector1) && !IsPassWall(pred.UnitPosition, pred.CastPosition))
                    {
                        if (pred.Hitchance >= HitChance.Medium)
                        {
                            Vector2 wts = Drawing.WorldToScreen(Player.Position);
                            Drawing.DrawText(wts[0], wts[1], Color.Wheat, "Hit: " + 1);

                            Vector2 wtsPlayer = Drawing.WorldToScreen(vector1);
                            Vector2 wtsPred = Drawing.WorldToScreen(pred.CastPosition);

                            Drawing.DrawLine(wtsPlayer, wtsPred, 1, Color.Wheat);
                            Utility.DrawCircle(vector1, 50, Color.Aqua);
                            Utility.DrawCircle(pred.CastPosition, 50, Color.Yellow);
                        }
                    }
                    return;
                }
                //-----------------------------------------------------------------Draw Ult Mec-----------------------------------------------
                int maxHit = 0;
                Vector3 start = Vector3.Zero;
                Vector3 end = Vector3.Zero;
                Vector3 mid = Vector3.Zero;
                //loop one
                foreach (var target in ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsValidTarget(R.Range)).OrderByDescending(GetComboDamage))
                {
                    //loop 2
                    foreach (
                        var enemy in
                            ObjectManager.Get<Obj_AI_Hero>()
                                .Where(
                                    x =>
                                        x.IsValidTarget(R.Range + 1000) && x.NetworkId != target.NetworkId &&
                                        x.Distance(target) < 900)
                                .OrderByDescending(x => x.Distance(target)))
                    {
                        int hit = 2;

                        var targetPred = Prediction.GetPrediction(target, .25f);
                        var enemyPred = Prediction.GetPrediction(enemy, .25f);

                        var midpoint = (enemyPred.CastPosition + targetPred.CastPosition) / 2;

                        var startpos = midpoint + Vector3.Normalize(enemyPred.CastPosition - targetPred.CastPosition) * 600;
                        var endPos = midpoint - Vector3.Normalize(enemyPred.CastPosition - targetPred.CastPosition) * 600;

                        if (!IsPassWall(midpoint, startpos) && !IsPassWall(midpoint, endPos) && countEnemiesNearPosition(Player.ServerPosition, R.Range + 1000) > 2)
                        {
                            //loop 3
                            foreach (var enemy2 in ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsValidTarget(R.Range + 1000) && x.NetworkId != target.NetworkId && x.NetworkId != enemy.NetworkId && x.Distance(target) < 1000))
                            {
                                var enemy2Pred = Prediction.GetPrediction(enemy2, .25f);

                                Object[] obj = VectorPointProjectionOnLineSegment(startpos.To2D(), endPos.To2D(), enemy2Pred.CastPosition.To2D());
                                var isOnseg = (bool)obj[2];
                                var pointLine = (Vector2)obj[1];

                                if (pointLine.Distance(enemy2Pred.CastPosition.To2D()) < 100 + enemy2.BoundingRadius &&
                                    isOnseg)
                                {
                                    hit++;
                                }
                            }
                        }
                        if (hit > maxHit)
                        {
                            maxHit = hit;
                            start = startpos;
                            end = endPos;
                            mid = midpoint;
                        }
                    }
                }

                if (maxHit >= 2)
                {
                    Vector2 wts = Drawing.WorldToScreen(Player.Position);
                    Drawing.DrawText(wts[0], wts[1], Color.Wheat, "Hit: " + maxHit);

                    Vector2 wtsPlayer = Drawing.WorldToScreen(start);
                    Vector2 wtsPred = Drawing.WorldToScreen(end);

                    Drawing.DrawLine(wtsPlayer, wtsPred, 1, Color.Wheat);
                    Utility.DrawCircle(start, 50, Color.Aqua);
                    Utility.DrawCircle(end, 50, Color.Yellow);
                    Utility.DrawCircle(mid, 50, Color.Red);
                }
                //---------------------------------------------------End drawing Ult Mec---------------------------------------
            }
        }
    }
}

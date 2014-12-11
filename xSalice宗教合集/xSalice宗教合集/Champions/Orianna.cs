using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

namespace xSaliceReligionAIO.Champions
{
    class Orianna : Champion
    {
        //ball manager
        private bool _isBallMoving;
        private Vector3 _currentBallPosition;
        private Vector3 _allyDraw;
        private int _ballStatus;

        public Orianna()
        {
            SetupSpells();
            LoadMenu();
        }

        private void SetupSpells()
        {
            //intalize spell
            Q = new Spell(SpellSlot.Q, 825);
            W = new Spell(SpellSlot.W, 250);
            E = new Spell(SpellSlot.E, 1095);
            R = new Spell(SpellSlot.R, 370);

            Q.SetSkillshot(0.25f, 80, 1300, false, SkillshotType.SkillshotLine);
            W.SetSkillshot(0f, 250, float.MaxValue, false, SkillshotType.SkillshotCircle);
            E.SetSkillshot(0.25f, 145, 1700, false, SkillshotType.SkillshotLine);
            R.SetSkillshot(0.60f, 370, float.MaxValue, false, SkillshotType.SkillshotCircle);

            SpellList.Add(Q);
            SpellList.Add(W);
            SpellList.Add(E);
            SpellList.Add(R);
        }

        private void LoadMenu()
        {
            //Keys
            var key = new Menu("键位", "Key");{
                key.AddItem(new MenuItem("ComboActive", "连招!").SetValue(new KeyBind(32, KeyBindType.Press)));
                key.AddItem(new MenuItem("HarassActive", "骚扰!").SetValue(new KeyBind("C".ToCharArray()[0], KeyBindType.Press)));
                key.AddItem(new MenuItem("HarassActiveT", "骚扰 (自动)!").SetValue(new KeyBind("N".ToCharArray()[0], KeyBindType.Toggle)));
                key.AddItem(new MenuItem("LaneClearActive", "清线!").SetValue(new KeyBind("V".ToCharArray()[0], KeyBindType.Press)));
                key.AddItem(new MenuItem("LastHitQQ", "使用 Q 补刀").SetValue(new KeyBind("A".ToCharArray()[0], KeyBindType.Press)));
                key.AddItem(new MenuItem("escape", "使用 E 逃跑!").SetValue(new KeyBind("Z".ToCharArray()[0], KeyBindType.Press)));
                //add to menu
                menu.AddSubMenu(key);
            }

            //Spell Menu
            var spellMenu = new Menu("法术 菜单", "SpellMenu");
            {
                //Q Menu
                var qMenu = new Menu("Q 法术", "QSpell");{
                    qMenu.AddItem(new MenuItem("qHit", "Q 连招 命中率").SetValue(new Slider(3, 1, 3)));
                    qMenu.AddItem(new MenuItem("qHit2", "Q 骚扰 命中率").SetValue(new Slider(3, 1, 4)));
                    spellMenu.AddSubMenu(qMenu);
                }
                //W
                var wMenu = new Menu("W 法术", "WSpell");
                {
                    wMenu.AddItem(new MenuItem("autoW", "自动W|敌人数量").SetValue(new Slider(2, 1, 5)));
                    spellMenu.AddSubMenu(wMenu);
                }
                //E
                var eMenu = new Menu("E 法术", "ESpell");
                {
                    eMenu.AddItem(new MenuItem("UseEDmg", "使用 E 保护").SetValue(true));

                    eMenu.AddSubMenu(new Menu("使用 E 保护队友", "shield"));
                    eMenu.SubMenu("shield").AddItem(new MenuItem("eAllyIfHP", "E保护|队友血量").SetValue(new Slider(40)));
                    foreach (Obj_AI_Hero ally in ObjectManager.Get<Obj_AI_Hero>().Where(ally => ally.IsAlly))
                        eMenu.SubMenu("shield").AddItem(new MenuItem("shield" + ally.BaseSkinName, ally.BaseSkinName).SetValue(false));

                    spellMenu.AddSubMenu(eMenu);
                }
                //R
                var rMenu = new Menu("R 法术", "RSpell"); {
                    rMenu.AddItem(new MenuItem("autoR", "使用R|敌方人数").SetValue(new Slider(3, 1, 5)));
                    rMenu.AddItem(new MenuItem("blockR", "没有敌人禁用R").SetValue(true));
                    rMenu.AddItem(new MenuItem("overK", "击杀 提示").SetValue(true));
                    rMenu.AddItem(new MenuItem("killR", "手动 R").SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Toggle)));

                    rMenu.AddSubMenu(new Menu("自动使用 R", "intR"));
                    foreach (Obj_AI_Hero enemy in ObjectManager.Get<Obj_AI_Hero>().Where(enemy => enemy.Team != Player.Team))
                        rMenu.SubMenu("intR").AddItem(new MenuItem("intR" + enemy.BaseSkinName, enemy.BaseSkinName).SetValue(false));

                    spellMenu.AddSubMenu(rMenu);
                }
                menu.AddSubMenu(spellMenu);
            }

            //Combo menu:
            var combo = new Menu("连招", "Combo");
            {
                combo.AddItem(new MenuItem("selected", "攻击 选定目标").SetValue(true));
                combo.AddItem(new MenuItem("UseQCombo", "使用 Q").SetValue(true));
                combo.AddItem(new MenuItem("UseWCombo", "使用 W").SetValue(true));
                combo.AddItem(new MenuItem("UseECombo", "使用 E").SetValue(true));
                combo.AddItem(new MenuItem("UseRCombo", "使用 R").SetValue(true));
                combo.AddItem(new MenuItem("autoRCombo", "使用R|敌方人数").SetValue(new Slider(2, 1, 5)));
                combo.AddItem(new MenuItem("ignite", "使用 点燃").SetValue(true));
                menu.AddSubMenu(combo);
            }
            //Harass menu:
            var harass = new Menu("骚扰", "Harass");
            {
                harass.AddItem(new MenuItem("UseQHarass", "使用 Q").SetValue(true));
                harass.AddItem(new MenuItem("UseWHarass", "使用 W").SetValue(false));
                harass.AddItem(new MenuItem("UseEHarass", "使用 E").SetValue(true));
                menu.AddSubMenu(harass);
            }
            //Farming menu:
            var farm = new Menu("清线", "Farm");
            {
                farm.AddItem(new MenuItem("UseQFarm", "使用 Q").SetValue(false));
                farm.AddItem(new MenuItem("UseWFarm", "使用 W").SetValue(false));
                farm.AddItem(new MenuItem("qFarm", "使用Q/W补兵|小兵数量大于").SetValue(new Slider(3, 0, 5)));
                menu.AddSubMenu(farm);
            }

            //intiator list:
            var initator = new Menu("引爆器", "Initiator");
            {
                foreach (Obj_AI_Hero hero in ObjectManager.Get<Obj_AI_Hero>().Where(hero => hero.IsAlly))
                {
                    foreach (Initiator intiator in Initiator.InitatorList)
                    {
                        if (intiator.HeroName == hero.BaseSkinName)
                        {
                            initator.AddItem(new MenuItem(intiator.SpellName, intiator.SpellName)).SetValue(false);
                        }
                    }
                }
                menu.AddSubMenu(initator);
            }

            //Misc Menu:
            var misc = new Menu("杂项", "Misc");
            {
                misc.AddItem(new MenuItem("UseInt", "使用 R 中断法术").SetValue(true));
            }

            //Damage after combo:
            MenuItem dmgAfterComboItem = new MenuItem("DamageAfterCombo", "显示组合连招伤害").SetValue(true);
            Utility.HpBarDamageIndicator.DamageToUnit = GetComboDamage;
            Utility.HpBarDamageIndicator.Enabled = dmgAfterComboItem.GetValue<bool>();
            dmgAfterComboItem.ValueChanged +=
                delegate(object sender, OnValueChangeEventArgs eventArgs)
                {
                    Utility.HpBarDamageIndicator.Enabled = eventArgs.GetNewValue<bool>();
                };

            //Drawings menu:
            var drawing = new Menu("范围", "Drawings"); { 
                drawing.AddItem(new MenuItem("QRange", "Q 范围").SetValue(new Circle(false, Color.FromArgb(100, 255, 0, 255))));
                drawing.AddItem(new MenuItem("WRange", "W 范围").SetValue(new Circle(true, Color.FromArgb(100, 255, 0, 255))));
                drawing.AddItem(new MenuItem("ERange", "E 范围").SetValue(new Circle(false, Color.FromArgb(100, 255, 0, 255))));
                drawing.AddItem(new MenuItem("RRange", "R 范围").SetValue(new Circle(false, Color.FromArgb(100, 255, 0, 255))));
                drawing.AddItem(new MenuItem("rModeDraw", "R 调试").SetValue(new Circle(true, Color.FromArgb(100, 255, 0, 255))));
                drawing.AddItem(dmgAfterComboItem);
                menu.AddSubMenu(drawing);
            }
        }

        private float GetComboDamage(Obj_AI_Base enemy)
        {
            double damage = 0d;

            //if (Q.IsReady())
            damage += Player.GetSpellDamage(enemy, SpellSlot.Q) * 1.5;

            if (W.IsReady())
                damage += Player.GetSpellDamage(enemy, SpellSlot.W);

            if (E.IsReady())
                damage += Player.GetSpellDamage(enemy, SpellSlot.E);

            if (IgniteSlot != SpellSlot.Unknown && Player.SummonerSpellbook.CanUseSpell(IgniteSlot) == SpellState.Ready)
                damage += ObjectManager.Player.GetSummonerSpellDamage(enemy, Damage.SummonerSpell.Ignite);

            if (R.IsReady())
                damage += Player.GetSpellDamage(enemy, SpellSlot.R) - 25;

            return (float)damage;
        }

        private void Combo()
        {
            //Orbwalker.SetAttacks(!(Q.IsReady()));
            UseSpells(menu.Item("UseQCombo").GetValue<bool>(), menu.Item("UseWCombo").GetValue<bool>(),
                menu.Item("UseECombo").GetValue<bool>(), menu.Item("UseRCombo").GetValue<bool>(), "Combo");
        }
        private void Harass()
        {
            UseSpells(menu.Item("UseQHarass").GetValue<bool>(), menu.Item("UseWHarass").GetValue<bool>(),
                menu.Item("UseEHarass").GetValue<bool>(), false, "Harass");
        }

        private void UseSpells(bool useQ, bool useW, bool useE, bool useR, String source)
        {
            var range = E.IsReady() ? E.Range : Q.Range;
            Obj_AI_Hero target = SimpleTs.GetTarget(range, SimpleTs.DamageType.Magical);

            if (GetTargetFocus(range) != null)
                target = GetTargetFocus(range);

            if (useQ && Q.IsReady())
            {
                CastQ(target, source);
            }

            if (_isBallMoving)
                return;

            if (useW && target != null && W.IsReady())
            {
                CastW(target);
            }

            //Ignite
            if (target != null && menu.Item("ignite").GetValue<bool>() && IgniteSlot != SpellSlot.Unknown &&
                Player.SummonerSpellbook.CanUseSpell(IgniteSlot) == SpellState.Ready && source == "Combo")
            {
                if (GetComboDamage(target) > target.Health)
                {
                    Player.SummonerSpellbook.CastSpell(IgniteSlot, target);
                }
            }

            if (useE && target != null && E.IsReady())
            {
                CastE(target);
            }

            if (useR && target != null && R.IsReady())
            {
                if (menu.Item("intR" + target.BaseSkinName) != null)
                {
                    foreach (
                        Obj_AI_Hero enemy in
                            ObjectManager.Get<Obj_AI_Hero>()
                                .Where(x => Player.Distance(x) < 1500 && x.IsValidTarget() && x.IsEnemy && !x.IsDead))
                    {
                        if (enemy != null && !enemy.IsDead && menu.Item("intR" + enemy.BaseSkinName).GetValue<bool>())
                        {
                            CastR(enemy);
                            return;
                        }
                    }
                }

                if (!(menu.Item("killR").GetValue<KeyBind>().Active)) //check if multi
                {
                    if (menu.Item("overK").GetValue<bool>() &&
                        (Player.GetSpellDamage(target, SpellSlot.Q) * 2) >= target.Health)
                    {
                    }
                    if (GetComboDamage(target) >= target.Health - 100)
                        CastR(target);
                }
            }
        }

        private void CastW(Obj_AI_Base target)
        {
            if (_isBallMoving) return;

            PredictionOutput prediction = GetPCircle(_currentBallPosition, W, target, true);

            if (W.IsReady() && prediction.UnitPosition.Distance(_currentBallPosition) < W.Width)
            {
                W.Cast();
            }

        }

        private void CastR(Obj_AI_Base target)
        {
            if (_isBallMoving) return;

            PredictionOutput prediction = GetPCircle(_currentBallPosition, R, target, true);

            if (R.IsReady() && prediction.UnitPosition.Distance(_currentBallPosition) <= R.Width)
            {
                R.Cast();
            }
        }

        private void CastE(Obj_AI_Base target)
        {
            if (_isBallMoving) return;

            Obj_AI_Hero etarget = Player;

            switch (_ballStatus)
            {
                case 0:
                    if (target != null)
                    {
                        float travelTime = target.Distance(Player.ServerPosition) / Q.Speed;
                        float minTravelTime = 10000f;

                        foreach (
                            Obj_AI_Hero ally in
                                ObjectManager.Get<Obj_AI_Hero>()
                                    .Where(x => x.IsAlly && Player.Distance(x.ServerPosition) <= E.Range && !x.IsMe))
                        {
                            if (ally != null)
                            {
                                //dmg enemy with E
                                if (menu.Item("UseEDmg").GetValue<bool>())
                                {
                                    PredictionOutput prediction3 = GetP(Player.ServerPosition, E, target, true);
                                    Object[] obj = VectorPointProjectionOnLineSegment(Player.ServerPosition.To2D(),
                                        ally.ServerPosition.To2D(), prediction3.UnitPosition.To2D());
                                    var isOnseg = (bool)obj[2];
                                    var pointLine = (Vector2)obj[1];

                                    if (E.IsReady() && isOnseg &&
                                        prediction3.UnitPosition.Distance(pointLine.To3D()) < E.Width)
                                    {
                                        //Game.PrintChat("Dmg 1");
                                        E.CastOnUnit(ally, packets());
                                        return;
                                    }
                                }

                                float allyRange = target.Distance(ally.ServerPosition) / Q.Speed +
                                                  ally.Distance(Player.ServerPosition) / E.Speed;
                                if (allyRange < minTravelTime)
                                {
                                    etarget = ally;
                                    minTravelTime = allyRange;
                                }
                            }
                        }

                        if (minTravelTime < travelTime && Player.Distance(etarget.ServerPosition) <= E.Range &&
                            E.IsReady())
                        {
                            E.CastOnUnit(etarget, packets());
                        }
                    }
                    break;
                case 1:
                    //dmg enemy with E
                    if (menu.Item("UseEDmg").GetValue<bool>())
                    {
                        PredictionOutput prediction = GetP(_currentBallPosition, E, target, true);
                        Object[] obj = VectorPointProjectionOnLineSegment(_currentBallPosition.To2D(),
                            Player.ServerPosition.To2D(), prediction.UnitPosition.To2D());
                        var isOnseg = (bool)obj[2];
                        var pointLine = (Vector2)obj[1];

                        if (E.IsReady() && isOnseg && prediction.UnitPosition.Distance(pointLine.To3D()) < E.Width)
                        {
                            //Game.PrintChat("Dmg 2");
                            E.CastOnUnit(Player, packets());
                            return;
                        }
                    }

                    float travelTime2 = target.Distance(_currentBallPosition) / Q.Speed;
                    float minTravelTime2 = target.Distance(Player.ServerPosition) / Q.Speed +
                                            Player.Distance(_currentBallPosition) / E.Speed;

                    if (minTravelTime2 < travelTime2 && target.Distance(Player.ServerPosition) <= Q.Range + Q.Width &&
                        E.IsReady())
                    {
                        E.CastOnUnit(Player, packets());
                    }

                    break;
                case 2:
                    float travelTime3 = target.Distance(_currentBallPosition) / Q.Speed;
                    float minTravelTime3 = 10000f;

                    foreach (
                        Obj_AI_Hero ally in
                            ObjectManager.Get<Obj_AI_Hero>()
                                .Where(x => x.IsAlly && Player.Distance(x.ServerPosition) <= E.Range && !x.IsMe))
                    {
                        if (ally != null)
                        {
                            //dmg enemy with E
                            if (menu.Item("UseEDmg").GetValue<bool>())
                            {
                                PredictionOutput prediction2 = GetP(_currentBallPosition, E, target, true);
                                Object[] obj = VectorPointProjectionOnLineSegment(_currentBallPosition.To2D(),
                                    ally.ServerPosition.To2D(), prediction2.UnitPosition.To2D());
                                var isOnseg = (bool)obj[2];
                                var pointLine = (Vector2)obj[1];

                                if (E.IsReady() && isOnseg &&
                                    prediction2.UnitPosition.Distance(pointLine.To3D()) < E.Width)
                                {
                                    //Game.PrintChat("Dmg 3");
                                    E.CastOnUnit(ally, packets());
                                    return;
                                }
                            }

                            float allyRange2 = target.Distance(ally.ServerPosition) / Q.Speed +
                                                ally.Distance(_currentBallPosition) / E.Speed;

                            if (allyRange2 < minTravelTime3)
                            {
                                etarget = ally;
                                minTravelTime3 = allyRange2;
                            }
                        }
                    }

                    if (minTravelTime3 < travelTime3 && Player.Distance(etarget.ServerPosition) <= E.Range &&
                        E.IsReady())
                    {
                        E.CastOnUnit(etarget, packets());
                    }

                    break;
            }
        }

        private void CastQ(Obj_AI_Base target, String source)
        {
            if (_isBallMoving) return;

            PredictionOutput prediction = GetP(_currentBallPosition, Q, target, true);

            if (Q.IsReady() && prediction.Hitchance >= GetHitchance(source) && Player.Distance(target) <= Q.Range + Q.Width)
            {
                Q.Cast(prediction.CastPosition, packets());
            }
        }

        private void CheckWMec()
        {
            if (!W.IsReady() || _isBallMoving)
                return;

            int minHit = menu.Item("autoW").GetValue<Slider>().Value;

            int hit = (from x in ObjectManager.Get<Obj_AI_Hero>().Where(champ => champ.IsValidTarget(1500) && champ.IsVisible && !champ.IsZombie) 
                       where x != null select GetPCircle(_currentBallPosition, W, x, true)).Count(prediction => W.IsReady() && prediction.UnitPosition.Distance(_currentBallPosition) < W.Width);

            if (hit >= minHit && W.IsReady())
                W.Cast();
        }

        private void CheckRMec()
        {
            if (!R.IsReady() || _isBallMoving)
                return;

            int minHit = menu.Item("autoRCombo").GetValue<Slider>().Value;

            int hit = 0;
            foreach (Obj_AI_Hero champ in from champ in ObjectManager.Get<Obj_AI_Hero>() where champ.IsValidTarget(1500) && champ.IsVisible && !champ.IsZombie where champ != null let prediction = GetPCircle(_currentBallPosition, R, champ, true) where R.IsReady() && prediction.UnitPosition.Distance(_currentBallPosition) <= R.Width select champ)
            {
                if (Player.GetSpellDamage(champ, SpellSlot.R) > champ.Health + 25)
                    hit += 2;
                else
                    hit++;
            }

            if (hit >= minHit && R.IsReady())
                R.Cast();
        }

        private void CheckRMecGlobal()
        {
            if (!R.IsReady() || _isBallMoving)
                return;

            int minHit = menu.Item("autoR").GetValue<Slider>().Value;

            int hit = 0;
            foreach (Obj_AI_Hero champ in from champ in ObjectManager.Get<Obj_AI_Hero>() where champ.IsValidTarget(1500) && champ.IsVisible && !champ.IsZombie where champ != null let prediction = GetPCircle(_currentBallPosition, R, champ, true) where R.IsReady() && prediction.UnitPosition.Distance(_currentBallPosition) <= R.Width select champ)
            {
                if (Player.GetSpellDamage(champ, SpellSlot.R) > champ.Health + 25)
                    hit += 2;
                else
                    hit++;
            }

            if (hit >= minHit && R.IsReady())
                R.Cast();
        }

        private int CountR()
        {
            if (!R.IsReady())
                return 0;

            return (from enemy in ObjectManager.Get<Obj_AI_Hero>().Where(champ => champ.IsValidTarget(1500) && champ.IsVisible && !champ.IsZombie) 
                    where enemy != null select GetPCircle(_currentBallPosition, R, enemy, true)).Count(prediction => R.IsReady() && prediction.UnitPosition.Distance(_currentBallPosition) <= R.Width);
        }

        private void LastHit()
        {
            if (!Orbwalking.CanMove(40)) return;

            List<Obj_AI_Base> allMinions = MinionManager.GetMinions(Player.ServerPosition, Q.Range);

            if (Q.IsReady())
            {
                foreach (Obj_AI_Base minion in allMinions)
                {
                    if (minion.IsValidTarget() &&
                        HealthPrediction.GetHealthPrediction(minion, (int)(Player.Distance(minion) * 1000 / 1400)) <
                        Player.GetSpellDamage(minion, SpellSlot.Q) - 10)
                    {
                        PredictionOutput prediction = GetP(_currentBallPosition, Q, minion, true);

                        if (prediction.Hitchance >= HitChance.High && Q.IsReady())
                            Q.Cast(prediction.CastPosition, packets());
                    }
                }
            }
        }

        private void Farm()
        {
            if (!Orbwalking.CanMove(40)) return;

            List<Obj_AI_Base> allMinionsQ = MinionManager.GetMinions(ObjectManager.Player.ServerPosition,
                Q.Range + Q.Width);
            List<Obj_AI_Base> allMinionsW = MinionManager.GetMinions(ObjectManager.Player.ServerPosition,
                Q.Range + Q.Width);

            var useQ = menu.Item("UseQFarm").GetValue<bool>();
            var useW = menu.Item("UseWFarm").GetValue<bool>();
            int min = menu.Item("qFarm").GetValue<Slider>().Value;

            if (useQ && Q.IsReady())
            {
                Q.From = _currentBallPosition;

                MinionManager.FarmLocation pred = Q.GetCircularFarmLocation(allMinionsQ, Q.Width + 15);

                if (pred.MinionsHit >= min)
                    Q.Cast(pred.Position, packets());
            }

            int hit = 0;
            if (useW && W.IsReady())
            {
                hit += allMinionsW.Count(enemy => enemy.Distance(_currentBallPosition) < W.Range);

                if (hit >= min && W.IsReady())
                    W.Cast();
            }
        }

        private void Escape()
        {
            if (_ballStatus == 0 && W.IsReady())
                W.Cast();
            else if (E.IsReady() && _ballStatus != 0)
                E.CastOnUnit(Player, packets());
        }

        public override void Game_OnGameUpdate(EventArgs args)
        {
            //check if player is dead
            if (Player.IsDead) return;

            OnGainBuff();

            CheckRMecGlobal();

            if (menu.Item("escape").GetValue<KeyBind>().Active)
            {
                Escape();
            }
            else if (menu.Item("ComboActive").GetValue<KeyBind>().Active)
            {
                CheckRMec();
                Combo();
            }
            else
            {
                if (menu.Item("HarassActive").GetValue<KeyBind>().Active ||
                    menu.Item("HarassActiveT").GetValue<KeyBind>().Active)
                    Harass();

                if (menu.Item("LaneClearActive").GetValue<KeyBind>().Active)
                {
                    Farm();
                }

                if (menu.Item("LastHitQQ").GetValue<KeyBind>().Active)
                {
                    LastHit();
                }
            }

            CheckWMec();
        }

        private void OnGainBuff()
        {
            if (Player.HasBuff("OrianaGhostSelf"))
            {
                _ballStatus = 0;
                _currentBallPosition = Player.ServerPosition;
                _isBallMoving = false;
                return;
            }

            foreach (Obj_AI_Hero ally in
                ObjectManager.Get<Obj_AI_Hero>()
                    .Where(ally => ally.IsAlly && !ally.IsDead && ally.HasBuff("orianaghost", true)))
            {
                _ballStatus = 2;
                _currentBallPosition = ally.ServerPosition;
                _allyDraw = ally.Position;
                _isBallMoving = false;
                return;
            }

            _ballStatus = 1;
        }

        public override void Drawing_OnDraw(EventArgs args)
        {
            foreach (Spell spell in SpellList)
            {
                var menuItem = menu.Item(spell.Slot + "Range").GetValue<Circle>();
                if ((spell.Slot == SpellSlot.R && menuItem.Active) || (spell.Slot == SpellSlot.W && menuItem.Active))
                {
                    if (_ballStatus == 0)
                        Utility.DrawCircle(Player.Position, spell.Range, spell.IsReady() ? Color.Aqua : Color.Red);
                    else if (_ballStatus == 2)
                        Utility.DrawCircle(_allyDraw, spell.Range, spell.IsReady() ? Color.Aqua : Color.Red);
                    else
                        Utility.DrawCircle(_currentBallPosition, spell.Range, spell.IsReady() ? Color.Aqua : Color.Red);
                }
                else if (menuItem.Active)
                    Utility.DrawCircle(Player.Position, spell.Range, spell.IsReady() ? Color.Aqua : Color.Red);
            }
            if (menu.Item("rModeDraw").GetValue<Circle>().Active)
            {
                if (menu.Item("killR").GetValue<KeyBind>().Active)
                {
                    Vector2 wts = Drawing.WorldToScreen(Player.Position);
                    Drawing.DrawText(wts[0], wts[1], Color.White, "R Multi On");
                }
                else
                {
                    Vector2 wts = Drawing.WorldToScreen(Player.Position);
                    Drawing.DrawText(wts[0], wts[1], Color.Red, "R Multi Off");
                }
            }
        }

        public override void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base unit, GameObjectProcessSpellCastEventArgs args)
        {
            //Shield Ally
            if (unit.IsEnemy && unit.Type == GameObjectType.obj_AI_Hero && E.IsReady())
            {
                foreach (
                    Obj_AI_Hero ally in
                        ObjectManager.Get<Obj_AI_Hero>()
                            .Where(x => Player.Distance(x) < E.Range && Player.Distance(unit) < 1500 && x.IsAlly && !x.IsDead).OrderBy(x => x.Distance(args.End)))
                {
                    if (menu.Item("shield" + ally.BaseSkinName) != null)
                    {
                        if (menu.Item("shield" + ally.BaseSkinName).GetValue<bool>())
                        {
                            int hp = menu.Item("eAllyIfHP").GetValue<Slider>().Value;
                            float hpPercent = ally.Health / ally.MaxHealth * 100;

                            if (ally.Distance(args.End) < 500 && hpPercent <= hp)
                            {
                                //Game.PrintChat("shielding");
                                E.CastOnUnit(ally, packets());
                                _isBallMoving = true;
                                return;
                            }
                        }
                    }
                }
            }

            //intiator
            if (unit.IsAlly)
            {
                if (Initiator.InitatorList.Where(spell => args.SData.Name == spell.SDataName).Where(spell => menu.Item(spell.SpellName).GetValue<bool>()).Any(spell => E.IsReady() && Player.Distance(unit) < E.Range))
                {
                    E.CastOnUnit(unit, packets());
                    _isBallMoving = true;
                    return;
                }
            }

            if (!unit.IsMe) return;

            SpellSlot castedSlot = ObjectManager.Player.GetSpellSlot(args.SData.Name, false);

            if (castedSlot == SpellSlot.Q)
            {
                _isBallMoving = true;
                Utility.DelayAction.Add(
                    (int)Math.Max(1, 1000 * (args.End.Distance(_currentBallPosition) - Game.Ping - 0.1) / Q.Speed), () =>
                    {
                        _currentBallPosition = args.End;
                        _ballStatus = 1;
                        _isBallMoving = false;
                        //Game.PrintChat("Stopped");
                    });
            }
        }

        public override void Interrupter_OnPosibleToInterrupt(Obj_AI_Base unit, InterruptableSpell spell)
        {
            if (!menu.Item("UseInt").GetValue<bool>()) return;

            if (Player.Distance(unit) < R.Range && unit != null)
            {
                CastR(unit);
            }
            else
            {
                CastQ(unit, "Combo");
            }
        }

        public override void Game_OnSendPacket(GamePacketEventArgs args)
        {
            if (args.PacketData[0] == Packet.C2S.Cast.Header)
            {
                Packet.C2S.Cast.Struct decodedPacket = Packet.C2S.Cast.Decoded(args.PacketData);
                if (decodedPacket.Slot == SpellSlot.R)
                {
                    if (CountR() == 0 && menu.Item("blockR").GetValue<bool>())
                    {
                        //Block packet if enemies hit is 0
                        args.Process = false;
                    }
                }
            }
        }
    }
}

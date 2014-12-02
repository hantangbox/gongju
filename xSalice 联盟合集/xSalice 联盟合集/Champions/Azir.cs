using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

namespace xSaliceReligionAIO.Champions
{
    class Azir : Champion
    {
        public Azir()
        {
            LoadSpells();
            LoadMenu();
        }

        private static Obj_AI_Base _insecTarget;
        private Vector3 _rVec;

        private void LoadSpells()
        {
            //intalize spell
            Q = new Spell(SpellSlot.Q, 850);
            QExtend = new Spell(SpellSlot.Q, 1150);
            W = new Spell(SpellSlot.W, 450);
            E = new Spell(SpellSlot.E, 2000);
            R = new Spell(SpellSlot.R, 450);

            Q.SetSkillshot(0.1f, 100, 1700, false, SkillshotType.SkillshotLine);
            QExtend.SetSkillshot(0.1f, 100, 1700, false, SkillshotType.SkillshotLine);
            E.SetSkillshot(0.25f, 100, 1200, false, SkillshotType.SkillshotLine);
            R.SetSkillshot(0.5f, 700, 1400, false, SkillshotType.SkillshotLine);

            SpellList.Add(W);
            SpellList.Add(E);
            SpellList.Add(R);
        }

        private void LoadMenu()
        {

            var key = new Menu("Key", "Key");
            {
                key.AddItem(new MenuItem("ComboActive", "Combo!").SetValue(new KeyBind(32, KeyBindType.Press)));
                key.AddItem(new MenuItem("HarassActive", "Harass!").SetValue(new KeyBind("C".ToCharArray()[0], KeyBindType.Press)));
                key.AddItem(new MenuItem("HarassActiveT", "Harass (toggle)!").SetValue(new KeyBind("N".ToCharArray()[0], KeyBindType.Toggle)));
                key.AddItem(new MenuItem("LaneClearActive", "Farm!").SetValue(new KeyBind("V".ToCharArray()[0], KeyBindType.Press)));
                key.AddItem(new MenuItem("escape", "Escape").SetValue(new KeyBind(menu.Item("Flee_Key").GetValue<KeyBind>().Key, KeyBindType.Press)));
                key.AddItem(new MenuItem("insec", "Insec Selected target").SetValue(new KeyBind("J".ToCharArray()[0], KeyBindType.Press)));
                key.AddItem(new MenuItem("qeCombo", "Q->E stun Nearest target").SetValue(new KeyBind("V".ToCharArray()[0], KeyBindType.Press)));
                //add to menu
                menu.AddSubMenu(key);
            }

            //Spell Menu
            var spell = new Menu("Spell", "Spell");
            {

                var qMenu = new Menu("QSpell", "QSpell"); { 
                    qMenu.AddItem(new MenuItem("qOutRange", "Only When Enemy out of Range").SetValue(false));
                    qMenu.AddItem(new MenuItem("qExtend", "Use Extended Q Range").SetValue(true));
                    qMenu.AddItem(new MenuItem("qMulti", "Q if 2+ Soilder").SetValue(true));
                    qMenu.AddItem(new MenuItem("qHit", "Q HitChance").SetValue(new Slider(3, 1, 3)));
                    spell.AddSubMenu(qMenu);
                }
                //W Menu
                var wMenu = new Menu("WSpell", "WSpell"); {
                    wMenu.AddItem(new MenuItem("wAtk", "Always Atk Enemy").SetValue(true));
                    wMenu.AddItem(new MenuItem("wQ", "Use WQ Poke").SetValue(true));
                    spell.AddSubMenu(wMenu);
                }
                //E Menu
                var eMenu =  new Menu("ESpell", "ESpell");
                {
                    eMenu.AddItem(new MenuItem("eGap", "GapClose if out of Q Range").SetValue(false));
                    eMenu.AddItem(new MenuItem("eKill", "If Killable Combo").SetValue(false));
                    eMenu.AddItem(new MenuItem("eKnock", "Always Knockup/DMG").SetValue(false));
                    eMenu.AddItem(new MenuItem("eHP", "if HP >").SetValue(new Slider(100)));
                    spell.AddSubMenu(eMenu);
                }
                //R Menu
                var rMenu = new Menu("RSpell", "RSpell");{
                    rMenu.AddItem(new MenuItem("rHP", "if HP <").SetValue(new Slider(20)));
                    rMenu.AddItem(new MenuItem("rHit", "If Hit >= Target").SetValue(new Slider(3, 0, 5)));
                    rMenu.AddItem(new MenuItem("rWall", "R Enemy Into Wall").SetValue(true));
                    spell.AddSubMenu(rMenu);
                }
                menu.AddSubMenu(spell);
            }

            //Combo menu:
            var combo = new Menu("Combo", "Combo");
            {
                combo.AddItem(new MenuItem("UseQCombo", "Use Q").SetValue(true));
                combo.AddItem(new MenuItem("UseWCombo", "Use W").SetValue(true));
                combo.AddItem(new MenuItem("UseECombo", "Use E").SetValue(true));
                combo.AddItem(new MenuItem("UseRCombo", "Use R").SetValue(true));
                combo.AddItem(new MenuItem("ignite", "Use Ignite").SetValue(true));
                combo.AddItem(new MenuItem("igniteMode", "Mode").SetValue(new StringList(new[] {"Combo", "KS"})));
                menu.AddSubMenu(combo);
            }

            //Harass menu:
            var harass = new Menu("Harass", "Harass");
            {
                harass.AddItem(new MenuItem("UseQHarass", "Use Q").SetValue(true));
                harass.AddItem(new MenuItem("UseWHarass", "Use W").SetValue(true));
                harass.AddItem(new MenuItem("UseEHarass", "Use E").SetValue(false));
                menu.AddSubMenu(harass);
            }

            //killsteal
            var killSteal = new Menu("KillSteal", "KillSteal");
            {
                killSteal.AddItem(new MenuItem("smartKS", "Use Smart KS System").SetValue(true));
                killSteal.AddItem(new MenuItem("eKS", "Use E KS").SetValue(false));
                killSteal.AddItem(new MenuItem("wqKS", "Use WQ KS").SetValue(true));
                killSteal.AddItem(new MenuItem("qeKS", "Use WQE KS").SetValue(false));
                killSteal.AddItem(new MenuItem("rKS", "Use R KS").SetValue(true));
                menu.AddSubMenu(killSteal);
            }

            //farm menu
            var farm = new Menu("Farm", "Farm");
            {
                farm.AddItem(new MenuItem("UseQFarm", "Use Q").SetValue(false));
                farm.AddItem(new MenuItem("qFarm", "Only Q if > minion").SetValue(new Slider(3, 0, 5)));
                menu.AddSubMenu(farm);
            }

            //Misc Menu:
            var misc = new Menu("Misc", "Misc");
            {
                misc.AddItem(new MenuItem("UseInt", "Use E to Interrupt").SetValue(true));
                misc.AddItem(new MenuItem("UseGap", "Use E for GapCloser").SetValue(true));
                misc.AddItem(new MenuItem("fastEscape", "Escape Mode 2").SetValue(true));
                menu.AddSubMenu(misc);
            }

            //Drawings menu:
            var draw = new Menu("Drawings", "Drawings"); { 
                draw.AddItem(new MenuItem("QRange", "Q range").SetValue(new Circle(false, Color.FromArgb(100, 255, 0, 255))));
                draw.AddItem(new MenuItem("QExtendRange", "Q Extended range").SetValue(new Circle(false, Color.FromArgb(100, 255, 0, 255))));
                draw.AddItem(new MenuItem("WRange", "W range").SetValue(new Circle(true, Color.FromArgb(100, 255, 0, 255))));
                draw.AddItem(new MenuItem("ERange", "E range").SetValue(new Circle(false, Color.FromArgb(100, 255, 0, 255))));
                draw.AddItem(new MenuItem("RRange", "R range").SetValue(new Circle(false, Color.FromArgb(100, 255, 0, 255))));
                draw.AddItem(new MenuItem("slaveDmg", "Draw Slave AA Needed").SetValue(new Circle(false, Color.FromArgb(100, 255, 0, 255))));

                MenuItem drawComboDamageMenu = new MenuItem("Draw_ComboDamage", "Draw Combo Damage").SetValue(true);
                MenuItem drawFill = new MenuItem("Draw_Fill", "Draw Combo Damage Fill").SetValue(new Circle(true, Color.FromArgb(90, 255, 169, 4)));
                draw.AddItem(drawComboDamageMenu);
                draw.AddItem(drawFill);
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

                menu.AddSubMenu(draw);
            }
        }

        private float GetComboDamage(Obj_AI_Base enemy)
        {
            var damage = 0d;

            if (Q.IsReady())
                damage += Player.GetSpellDamage(enemy, SpellSlot.Q);

            if (soilderCount() > 0 || W.IsReady())
            {
                damage += 2 * Player.CalcDamage(enemy, Damage.DamageType.Magical, DmgAmount());
            }

            if (E.IsReady())
                damage += Player.GetSpellDamage(enemy, SpellSlot.E);

            if (R.IsReady())
                damage += Player.GetSpellDamage(enemy, SpellSlot.R);

            return (float)damage;
        }

        private double DmgAmount()
        {
            double dmg = 0;

            if (Player.Level < 12)
            {
                dmg += 50 + 5 * Player.Level;
            }
            else
            {
                dmg += 10 * Player.Level - 10;
            }
            dmg += .7 * Player.FlatMagicDamageMod;

            return dmg;
        }

        private double GetAutoDmg(Obj_AI_Hero enemy)
        {
            return Player.CalcDamage(enemy, Damage.DamageType.Magical, DmgAmount());
        }

        private void Combo()
        {
            UseSpells(menu.Item("UseQCombo").GetValue<bool>(), menu.Item("UseWCombo").GetValue<bool>(),
                menu.Item("UseECombo").GetValue<bool>(), menu.Item("UseRCombo").GetValue<bool>());
        }

        private void Harass()
        {
            UseSpells(menu.Item("UseQHarass").GetValue<bool>(), menu.Item("UseWHarass").GetValue<bool>(),
                menu.Item("UseEHarass").GetValue<bool>(), false);
        }

        private void UseSpells(bool useQ, bool useW, bool useE, bool useR)
        {
            var qTarget = SimpleTs.GetTarget(QExtend.Range, SimpleTs.DamageType.Magical);
            var soilderTarget = SimpleTs.GetTarget(1200, SimpleTs.DamageType.Magical);

            // Game.PrintChat("Spell state: " + qSpell.State);
            var igniteMode = menu.Item("igniteMode").GetValue<StringList>().SelectedIndex;

            //R
            if (useR && R.IsReady() && ShouldR(qTarget) && Player.Distance(qTarget) < R.Range)
                R.Cast(qTarget);

            //WQ
            if (soilderCount() == 0 && useQ && useW && W.IsReady() && (Q.IsReady() || qSpell.State == SpellState.Surpressed) && menu.Item("wQ").GetValue<bool>())
            {
                CastWq(qTarget);
            }

            //W
            if (useW && W.IsReady())
            {
                CastW(qTarget);
            }

            //Q
            if (useQ && Q.IsReady())
            {
                CastQ(qTarget);
                return;
            }

            //Ignite
            if (qTarget != null && menu.Item("ignite").GetValue<bool>() && IgniteSlot != SpellSlot.Unknown && Player.SummonerSpellbook.CanUseSpell(IgniteSlot) == SpellState.Ready)
            {
                if (igniteMode == 0 && GetComboDamage(qTarget) > qTarget.Health)
                {
                    Player.SummonerSpellbook.CastSpell(IgniteSlot, qTarget);
                }
            }

            //E
            if (useE && (E.IsReady() || eSpell.State == SpellState.Surpressed))
            {
                CastE(soilderTarget);
            }


            //AutoAtk
            //attackTarget(soilderTarget);
        }

        private bool WallStun(Obj_AI_Hero target)
        {
            var pushedPos = R.GetPrediction(target).UnitPosition;

            if (IsPassWall(Player.ServerPosition, pushedPos))
                return true;

            return false;
        }

        private void SmartKs()
        {
            if (!menu.Item("smartKS").GetValue<bool>())
                return;
            var nearChamps = (from champ in ObjectManager.Get<Obj_AI_Hero>() where champ.IsValidTarget(1200) select champ).ToList();

            foreach (var target in nearChamps)
            {
                if (target != null && !target.IsDead && !target.HasBuffOfType(BuffType.Invulnerability) && target.IsValidTarget(1200))
                {
                    //ignite
                    if (menu.Item("ignite").GetValue<bool>() && IgniteSlot != SpellSlot.Unknown &&
                            Player.SummonerSpellbook.CanUseSpell(IgniteSlot) == SpellState.Ready && Player.Distance(target.ServerPosition) <= 600)
                    {
                        var igniteMode = menu.Item("igniteMode").GetValue<StringList>().SelectedIndex;
                        if (igniteMode == 1 && Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite) > target.Health)
                        {
                            Player.SummonerSpellbook.CastSpell(IgniteSlot, target);
                        }
                    }

                    //R
                    if ((Player.GetSpellDamage(target, SpellSlot.R)) > target.Health + 20 && Player.Distance(target) < R.Range && menu.Item("rKS").GetValue<bool>())
                    {
                        R.Cast(target);
                    }

                    if (soilderCount() < 1 && !W.IsReady())
                        return;

                    //WQ
                    if ((Player.GetSpellDamage(target, SpellSlot.Q)) > target.Health + 20 && menu.Item("wqKS").GetValue<bool>())
                    {
                        CastWq(target);
                    }

                    //qe
                    if ((Player.GetSpellDamage(target, SpellSlot.Q) + Player.GetSpellDamage(target, SpellSlot.E)) > target.Health + 20 && Player.Distance(target) < Q.Range && menu.Item("qeKS").GetValue<bool>())
                    {
                        CastQe(target);
                    }

                }
            }
        }

        private void Escape()
        {
            Vector3 wVec = Player.ServerPosition + Vector3.Normalize(Game.CursorPos - Player.ServerPosition) * 450;

            if (menu.Item("fastEscape").GetValue<bool>())
            {
                if (W.IsReady() || soilderCount() > 0)
                {
                    if ((E.IsReady() || eSpell.State == SpellState.Surpressed))
                    {
                        W.Cast(wVec);
                        W.LastCastAttemptT = Environment.TickCount;
                    }

                    if ((QExtend.IsReady() || qSpell.State == SpellState.Surpressed) &&
                        ((Environment.TickCount - E.LastCastAttemptT < Game.Ping + 500 &&
                          Environment.TickCount - E.LastCastAttemptT > Game.Ping + 50) || E.IsReady()))
                    {
                        if (Environment.TickCount - W.LastCastAttemptT > Game.Ping + 300 || eSpell.State == SpellState.Cooldown || !W.IsReady())
                        {
                            Vector3 qVec = Player.ServerPosition +
                                           Vector3.Normalize(Game.CursorPos - Player.ServerPosition) * 800;

                            var lastAttempt = (int)qVec.Distance(GetNearestSoilderToMouse().ServerPosition) / 1000;

                            Q.Cast(qVec, packets());
                            Q.LastCastAttemptT = Environment.TickCount + lastAttempt;
                            return;
                        }
                    }

                    if ((E.IsReady() || eSpell.State == SpellState.Surpressed))
                    {
                        if (Player.Distance(Game.CursorPos) > GetNearestSoilderToMouse().Distance(Game.CursorPos) && Environment.TickCount - Q.LastCastAttemptT > Game.Ping)
                        {
                            E.Cast(GetNearestSoilderToMouse().ServerPosition, packets());
                            E.LastCastAttemptT = Environment.TickCount - 250;
                            //Game.PrintChat("Rawr2");
                            return;
                        }
                        if (Environment.TickCount - W.LastCastAttemptT < Game.Ping + 300 && (Q.IsReady() || qSpell.State == SpellState.Surpressed))
                        {
                            E.Cast(wVec, packets());
                            E.LastCastAttemptT = Environment.TickCount - 250;
                            //Game.PrintChat("Rawr1");
                        }
                    }
                }
            }
            else
            {
                if (E.IsReady() || eSpell.State == SpellState.Surpressed)
                {
                    if (soilderCount() > 0)
                    {
                        Vector3 qVec = Player.ServerPosition +
                                       Vector3.Normalize(Game.CursorPos - Player.ServerPosition) * 800;

                        var slave = GetNearestSoilderToMouse();

                        var delay = (int)Math.Ceiling(slave.Distance(Player.ServerPosition));

                        if (QExtend.IsReady() || qSpell.State == SpellState.Surpressed)
                            Q.Cast(qVec, packets());

                        Utility.DelayAction.Add(delay,
                            () => E.Cast(GetNearestSoilderToMouse().ServerPosition, packets()));
                        return;
                    }
                    if (W.IsReady())
                    {
                        W.Cast(wVec);

                        if (E.IsReady() || eSpell.State == SpellState.Surpressed)
                            E.Cast(wVec, packets());

                        if (QExtend.IsReady() || qSpell.State == SpellState.Surpressed)
                        {
                            Vector3 qVec = Player.ServerPosition +
                                           Vector3.Normalize(Game.CursorPos - Player.ServerPosition) * 800;

                            Utility.DelayAction.Add(300, () => Q.Cast(qVec, packets()));
                        }
                    }
                }
            }
        }

        private Obj_AI_Base GetNearestSoilderToMouse()
        {
            var soilder = (from obj in ObjectManager.Get<Obj_AI_Base>() where obj.Name == "AzirSoldier" && obj.IsAlly && Player.Distance(obj.ServerPosition) < 2000 select obj)
                .ToList().OrderBy(x => Game.CursorPos.Distance(x.ServerPosition));

            if (soilder.FirstOrDefault() != null)
                return soilder.FirstOrDefault();

            return null;
        }

        private void CastQe(Obj_AI_Hero target)
        {
            if (soilderCount() > 0)
            {
                if ((Q.IsReady() || qSpell.State == SpellState.Surpressed) && E.IsReady())
                {
                    var slaves = (from obj in ObjectManager.Get<Obj_AI_Base>() where obj.Name == "AzirSoldier" && obj.IsAlly && target.Distance(obj.ServerPosition) < 2000 select obj).ToList();

                    foreach (var slave in slaves)
                    {
                        if (target != null && Player.Distance(target) < 800)
                        {
                            var qPred = GetP(slave.ServerPosition, QExtend, target, true);

                            if (Q.IsReady() && Player.Distance(target) < 800 && qPred.Hitchance >= getQHitchance())
                            {
                                var vec = target.ServerPosition - Player.ServerPosition;
                                var castBehind = qPred.CastPosition + Vector3.Normalize(vec) * 75;

                                Q.Cast(castBehind, packets());
                                E.Cast(slave.ServerPosition, packets());
                                return;

                            }
                        }
                    }
                }
            }
            else if (W.IsReady())
            {
                Vector3 wVec = Player.ServerPosition + Vector3.Normalize(target.ServerPosition - Player.ServerPosition) * 450;

                var qPred = GetP(wVec, QExtend, target, true);

                if ((Q.IsReady() || qSpell.State == SpellState.Surpressed) && (E.IsReady() || eSpell.State == SpellState.Surpressed) && Player.Distance(target) < 800 && qPred.Hitchance >= getQHitchance())
                {
                    var vec = target.ServerPosition - Player.ServerPosition;
                    var castBehind = qPred.CastPosition + Vector3.Normalize(vec) * 75;

                    W.Cast(wVec);
                    QExtend.Cast(castBehind, packets());
                    Utility.DelayAction.Add(1, () => E.Cast(getNearestSoilderToEnemy(target).ServerPosition, packets()));
                }
            }
        }

        private void Insec()
        {
            var target = (Obj_AI_Hero)_insecTarget;

            if (target == null)
                return;

            if (soilderCount() > 0)
            {
                if ((Q.IsReady() || qSpell.State == SpellState.Surpressed) && E.IsReady())
                {
                    var slaves = (from obj in ObjectManager.Get<Obj_AI_Base>() where obj.Name == "AzirSoldier" && obj.IsAlly && target.Distance(obj.ServerPosition) < 2000 select obj).ToList();

                    foreach (var slave in slaves)
                    {
                        if (Player.Distance(target) < 800)
                        {
                            var qPred = GetP(slave.ServerPosition, QExtend, target, true);
                            var vec = target.ServerPosition - Player.ServerPosition;
                            var castBehind = qPred.CastPosition + Vector3.Normalize(vec) * 75;
                            _rVec = qPred.CastPosition - Vector3.Normalize(vec) * 300;

                            if (Q.IsReady() && (E.IsReady() || eSpell.State == SpellState.Surpressed) && R.IsReady() && qPred.Hitchance >= getQHitchance())
                            {

                                Q.Cast(castBehind, packets());
                                E.Cast(slave.ServerPosition, packets());
                                E.LastCastAttemptT = Environment.TickCount;
                            }
                        }
                    }
                }
                if (R.IsReady())
                {
                    if (Player.Distance(target) < 200 && Environment.TickCount - E.LastCastAttemptT > Game.Ping + 150)
                    {
                        //Game.PrintChat("rawr");
                        R.Cast(_rVec);
                    }
                }
            }
            else if (W.IsReady())
            {
                Vector3 wVec = Player.ServerPosition + Vector3.Normalize(target.ServerPosition - Player.ServerPosition) * 450;

                var qPred = GetP(wVec, QExtend, target, true);

                if ((Q.IsReady() || qSpell.State == SpellState.Surpressed) && (E.IsReady() || eSpell.State == SpellState.Surpressed)
                    && R.IsReady() && Player.Distance(target) < 800 && qPred.Hitchance >= getQHitchance())
                {
                    var vec = target.ServerPosition - Player.ServerPosition;
                    var castBehind = qPred.CastPosition + Vector3.Normalize(vec) * 75;
                    _rVec = Player.Position;

                    W.Cast(wVec);
                    QExtend.Cast(castBehind, packets());
                    E.Cast(getNearestSoilderToEnemy(target).ServerPosition, packets());
                }
                if (R.IsReady())
                {
                    if (Player.Distance(target) < 200 && Environment.TickCount - E.LastCastAttemptT > Game.Ping + 150)
                    {
                        //Game.PrintChat("rawr2");
                        R.Cast(_rVec);
                    }
                }
            }
        }

        private void CastWq(Obj_AI_Hero target)
        {
            if (Player.Distance(target) < 1150 && Player.Distance(target) > 450)
            {
                if (W.IsReady() && (Q.IsReady() || qSpell.State == SpellState.Surpressed))
                {
                    Vector3 wVec = Player.ServerPosition + Vector3.Normalize(target.ServerPosition - Player.ServerPosition) * 450;

                    var qPred = GetP(wVec, QExtend, target, true);

                    if (qPred.Hitchance >= getQHitchance())
                    {
                        W.Cast(wVec);
                        QExtend.Cast(qPred.CastPosition, packets());
                    }
                }
            }
        }

        private void CastW(Obj_AI_Hero target)
        {
            if (Player.Distance(target) < 1200)
            {
                if (Player.Distance(target) < 450)
                {
                    //Game.PrintChat("W Cast1");
                    W.Cast(target);
                    if (canAttack())
                        Player.IssueOrder(GameObjectOrder.AttackUnit, target);
                }
                else if (Player.Distance(target) < 600)
                {
                    Vector3 wVec = Player.ServerPosition + Vector3.Normalize(target.ServerPosition - Player.ServerPosition) * 450;

                    //Game.PrintChat("W Cast2");
                    if (W.IsReady())
                    {
                        W.Cast(wVec);
                        if (canAttack())
                            Player.IssueOrder(GameObjectOrder.AttackUnit, target);
                    }
                }
                else if (Player.Distance(target) < 950)
                {
                    Vector3 wVec = Player.ServerPosition + Vector3.Normalize(target.ServerPosition - Player.ServerPosition) * 450;

                    //Game.PrintChat("W Cast2");
                    if (W.IsReady() && (Q.IsReady() || qSpell.State == SpellState.Surpressed))
                    {
                        var qPred = GetP(wVec, QExtend, target, true);

                        if (qPred.Hitchance >= getQHitchance())
                        {
                            W.Cast(wVec);
                        }
                    }
                }
            }
        }

        private void CastQ(Obj_AI_Hero target)
        {
            if (soilderCount() < 1)
                return;

            var slaves = (from obj in ObjectManager.Get<Obj_AI_Base>() where obj.Name == "AzirSoldier" && obj.IsAlly && target.Distance(obj.ServerPosition) < 2000 select obj).ToList();

            foreach (var slave in slaves)
            {
                if (target != null && Player.Distance(target) < QExtend.Range && ShouldQ(target, slave))
                {

                    var qPred = GetP(slave.ServerPosition, QExtend, target, true);

                    if (Q.IsReady() && Player.Distance(target) < 800 && qPred.Hitchance >= getQHitchance())
                    {
                        Q.Cast(qPred.CastPosition, packets());
                        return;
                    }
                    if (Q.IsReady() && Player.Distance(target) > 800 && qPred.Hitchance >= getQHitchance() && menu.Item("qExtend").GetValue<bool>())
                    {
                        var qVector = Player.ServerPosition + Vector3.Normalize(qPred.CastPosition - Player.ServerPosition) * 800;

                        //Game.PrintChat("QHarass");
                        QExtend.Cast(qVector, packets());
                        return;
                    }
                }
            }
        }

        private void CastE(Obj_AI_Hero target)
        {
            if (soilderCount() < 1)
                return;

            var slaves = (from obj in ObjectManager.Get<Obj_AI_Base>() where obj.Name == "AzirSoldier" && obj.IsAlly && target.Distance(obj.ServerPosition) < 2000 select obj).ToList();

            if (Player.Distance(target) > 1200 && menu.Item("eGap").GetValue<bool>())
            {
                var slavetar = getNearestSoilderToEnemy(target);
                if (slavetar != null && slavetar.Distance(target) < Player.Distance(target))
                {
                    E.Cast(slavetar, packets());
                }
            }

            foreach (var slave in slaves)
            {
                if (target != null && Player.Distance(slave) < E.Range)
                {
                    var ePred = GetP(slave.ServerPosition, E, target, true);
                    Object[] obj = VectorPointProjectionOnLineSegment(Player.ServerPosition.To2D(), slave.ServerPosition.To2D(), ePred.UnitPosition.To2D());
                    var isOnseg = (bool)obj[2];
                    var pointLine = (Vector2)obj[1];

                    if (E.IsReady() && isOnseg && pointLine.Distance(ePred.UnitPosition.To2D()) < E.Width && ShouldE(target))
                    {
                        E.Cast(slave.ServerPosition, packets());
                        return;
                    }
                }
            }
        }

        private bool ShouldQ(Obj_AI_Hero target, Obj_AI_Base slave)
        {
            if (!menu.Item("qOutRange").GetValue<bool>())
                return true;

            if (slave.Distance(target.ServerPosition) > 390)
                return true;

            if (soilderCount() > 1 && menu.Item("qMulti").GetValue<bool>())
                return true;

            if (Player.GetSpellDamage(target, SpellSlot.Q) > target.Health + 10)
                return true;


            return false;
        }
        private bool ShouldE(Obj_AI_Hero target)
        {
            if (menu.Item("eKnock").GetValue<bool>())
                return true;

            if (menu.Item("eKill").GetValue<bool>() && GetComboDamage(target) > target.Health + 15)
                return true;

            if (menu.Item("eKS").GetValue<bool>() && Player.GetSpellDamage(target, SpellSlot.E) > target.Health + 10)
                return true;

            //hp 
            var hp = menu.Item("eHP").GetValue<Slider>().Value;
            var hpPercent = Player.Health / Player.MaxHealth * 100;

            if (hpPercent > hp)
                return true;

            return false;
        }

        private bool ShouldR(Obj_AI_Hero target)
        {
            if (Player.GetSpellDamage(target, SpellSlot.R) > target.Health + 10)
                return true;

            var hp = menu.Item("rHP").GetValue<Slider>().Value;
            var hpPercent = Player.Health / Player.MaxHealth * 100;
            if (hpPercent < hp)
                return true;

            var rHit = menu.Item("rHit").GetValue<Slider>().Value;
            var pred = R.GetPrediction(target);
            if (pred.AoeTargetsHitCount >= rHit)
                return true;

            if (WallStun(target) && GetComboDamage(target) > target.Health / 2 && menu.Item("rWall").GetValue<bool>())
            {
                //Game.PrintChat("Walled");
                return true;
            }

            return false;
        }

        private void AutoAtk()
        {

            if (soilderCount() < 1)
                return;

            var soilderTarget = SimpleTs.GetTarget(800, SimpleTs.DamageType.Magical);

            //Game.PrintChat("YEhhhhh");

            AttackTarget(soilderTarget);
        }
        private HitChance getQHitchance()
        {
            var hitC = HitChance.High;
            var qHit = menu.Item("qHit").GetValue<Slider>().Value;

            // HitChance.Low = 3, Medium , High .... etc..
            switch (qHit)
            {
                case 1:
                    hitC = HitChance.Low;
                    break;
                case 2:
                    hitC = HitChance.Medium;
                    break;
                case 3:
                    hitC = HitChance.High;
                    break;
                case 4:
                    hitC = HitChance.VeryHigh;
                    break;
            }

            return hitC;
        }

        private int soilderCount()
        {
            return ObjectManager.Get<Obj_AI_Base>().Count(obj => obj.Name == "AzirSoldier" && obj.IsAlly);
        }

        private bool canAttack()
        {
            return xSLxOrbwalker.CanAttack();
        }

        private void AttackTarget(Obj_AI_Hero target)
        {
            if (soilderCount() < 1)
                return;

            var tar = getNearestSoilderToEnemy(target);
            if (tar != null && Player.Distance(tar) < 800)
            {
                if (target != null && tar.Distance(target) <= 390 && canAttack())
                {
                    xSLxOrbwalker.Orbwalk(Game.CursorPos, target);
                }
            }

        }

        private Obj_AI_Base getNearestSoilderToEnemy(Obj_AI_Base target)
        {
            var soilder = (from obj in ObjectManager.Get<Obj_AI_Base>() where obj.Name == "AzirSoldier" && obj.IsAlly && target.Distance(obj.ServerPosition) < 2000 select obj)
                .ToList().OrderBy(x => target.Distance(x.ServerPosition));

            if (soilder.FirstOrDefault() != null)
                return soilder.FirstOrDefault();

            return null;
        }

        private void Farm()
        {
            var allMinionsQ = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q.Range + Q.Width, MinionTypes.All, MinionTeam.NotAlly);
            var allMinionsW = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, W.Range + W.Width, MinionTypes.All, MinionTeam.NotAlly);

            var useQ = menu.Item("UseQFarm").GetValue<bool>();
            var min = menu.Item("qFarm").GetValue<Slider>().Value;


            if (useQ && (Q.IsReady() || qSpell.State == SpellState.Surpressed))
            {
                int hit;
                if (soilderCount() > 0)
                {
                    var slaves = (from obj in ObjectManager.Get<Obj_AI_Base>() where obj.Name == "AzirSoldier" && obj.IsAlly && Player.Distance(obj.ServerPosition) < 2000 select obj).ToList();
                    foreach (var slave in slaves)
                    {
                        foreach (var enemy in allMinionsQ)
                        {
                            hit = 0;
                            var prediction = GetP(slave.ServerPosition, Q, enemy, true);

                            if (Q.IsReady() && Player.Distance(enemy) <= Q.Range)
                            {
                                hit += allMinionsQ.Count(enemy2 => enemy2.Distance(prediction.CastPosition) < 200 && Q.IsReady());
                                if (hit >= min)
                                {
                                    if (Q.IsReady())
                                    {
                                        Q.Cast(prediction.CastPosition, packets());
                                        return;
                                    }
                                }
                            }
                        }
                    }
                }
                else if (W.IsReady())
                {
                    var wpred = W.GetCircularFarmLocation(allMinionsW);
                    W.Cast(wpred.Position);

                    foreach (var enemy in allMinionsQ)
                    {
                        hit = 0;
                        var prediction = GetP(Player.ServerPosition, Q, enemy, true);

                        if (Q.IsReady() && Player.Distance(enemy) <= Q.Range)
                        {
                            hit += allMinionsQ.Count(enemy2 => enemy2.Distance(prediction.CastPosition) < 200 && Q.IsReady());
                            if (hit >= min)
                            {
                                if (Q.IsReady())
                                {
                                    Q.Cast(prediction.CastPosition, packets());
                                    return;
                                }
                            }
                        }
                    }
                }
            }
        }

        public override void Game_OnGameUpdate(EventArgs args)
        {
            //check if player is dead
            if (Player.IsDead) return;

            SmartKs();

            if (menu.Item("escape").GetValue<KeyBind>().Active)
            {
                Escape();
            }
            else if (menu.Item("ComboActive").GetValue<KeyBind>().Active)
            {
                Combo();
            }
            else if (menu.Item("insec").GetValue<KeyBind>().Active)
            {
                xSLxOrbwalker.Orbwalk(Game.CursorPos, null);

                if (_insecTarget != null)
                    Insec();
            }
            else if (menu.Item("qeCombo").GetValue<KeyBind>().Active)
            {
                var soilderTarget = SimpleTs.GetTarget(900, SimpleTs.DamageType.Magical);

                xSLxOrbwalker.Orbwalk(Game.CursorPos, null);
                CastQe(soilderTarget);
            }
            else
            {
                if (menu.Item("LaneClearActive").GetValue<KeyBind>().Active)
                {
                    Farm();
                }

                if (menu.Item("HarassActive").GetValue<KeyBind>().Active)
                    Harass();

                if (menu.Item("HarassActiveT").GetValue<KeyBind>().Active)
                    Harass();

                if (menu.Item("wAtk").GetValue<bool>())
                    AutoAtk();
            }
        }

        public override void Drawing_OnDraw(EventArgs args)
        {
            foreach (var spell in SpellList)
            {
                var menuItem = menu.Item(spell.Slot + "Range").GetValue<Circle>();
                if (menuItem.Active)
                    Utility.DrawCircle(Player.Position, spell.Range, menuItem.Color);
            }
            if (menu.Item("QExtendRange").GetValue<Circle>().Active)
                Utility.DrawCircle(Player.Position, QExtend.Range, Color.LightBlue);

            if (menu.Item("slaveDmg").GetValue<Circle>().Active)
            {
                foreach (var enemy in ObjectManager.Get<Obj_AI_Hero>().Where(enemy => enemy.Team != Player.Team && enemy.IsValid && !enemy.IsDead))
                {
                    var wts = Drawing.WorldToScreen(enemy.Position);
                    Drawing.DrawText(wts[0], wts[1], Color.White, "AA To Kill: " + Math.Ceiling((enemy.Health / GetAutoDmg(enemy))));
                }
            }

        }

        public override void Game_OnSendPacket(GamePacketEventArgs args)
        {
            //ty trees
            if (args.PacketData[0] != Packet.C2S.SetTarget.Header)
            {
                return;
            }

            var decoded = Packet.C2S.SetTarget.Decoded(args.PacketData);

            if (decoded.NetworkId != 0 && decoded.Unit.IsValid && !decoded.Unit.IsMe)
            {
                _insecTarget = decoded.Unit;
            }
        }

        public override void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (!menu.Item("UseGap").GetValue<bool>()) return;

            if (R.IsReady() && gapcloser.Sender.IsValidTarget(R.Range))
                R.Cast(gapcloser.Sender);
        }

        public override void Interrupter_OnPosibleToInterrupt(Obj_AI_Base unit, InterruptableSpell spell)
        {
            if (!menu.Item("UseInt").GetValue<bool>()) return;

            if (Player.Distance(unit) < R.Range && unit != null && R.IsReady())
            {
                R.CastOnUnit(unit);
            }
        }
    }
}

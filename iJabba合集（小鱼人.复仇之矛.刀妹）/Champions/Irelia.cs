using System;
using System.Collections.Generic;
using System.Linq;
using Assemblies.Utilitys;
using LeagueSharp;
using LeagueSharp.Common;

namespace Assemblies.Champions {
    internal class Irelia : Champion {
        private int NumberR;
        private float PredictedArrivalTime;
        private bool QCastedMinion;
        private Obj_AI_Base SelectedMinion;

        public Irelia() {
            loadMenu();
            loadSpells();

            Game.OnGameUpdate += onUpdate;
            Drawing.OnDraw += onDraw;
            xSLxOrbwalker.BeforeAttack += beforeAttack;
            Game.PrintChat("[Assemblies] 鍒€閿嬫剰蹇梶浼婄憺鍒╀簹 鍔犺級鎴愬姛锛佹饥鍖朾y浜岀嫍锛丵Q缇361630847.");
        }

        private void beforeAttack(xSLxOrbwalker.BeforeAttackEventArgs args) {
            if (args.Unit.IsMe) {
                if (isMenuEnabled(menu, "useWC") && W.IsReady() && args.Target.IsValidTarget(Q.Range) &&
                    !args.Target.IsMinion)
                    W.Cast(true);
            }
        }

        private void loadSpells() {
            Q = new Spell(SpellSlot.Q, 650);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E, 425);
            R = new Spell(SpellSlot.R, 1000);

            R.SetSkillshot(0.15f, 80f, 1500f, false, SkillshotType.SkillshotLine);

            //TODO set skillshots
        }

        private void loadMenu() {
            menu.AddSubMenu(new Menu("连招 设置", "combo"));
            menu.SubMenu("combo").AddItem(new MenuItem("useQC", "使用 Q 连招").SetValue(true));
            menu.SubMenu("combo").AddItem(new MenuItem("useWC", "使用 W 连招").SetValue(true));
            menu.SubMenu("combo").AddItem(new MenuItem("useEC", "使用 E 连招").SetValue(true));
            menu.SubMenu("combo").AddItem(new MenuItem("useRC", "使用 R 连招").SetValue(true));
            menu.SubMenu("combo").AddItem(new MenuItem("gapcloseQ", "使用 Q 突进").SetValue(true));
            menu.SubMenu("combo").AddItem(new MenuItem("OStunE", "使用 E 眩晕").SetValue(true));

            menu.AddSubMenu(new Menu("骚扰 设置", "harass"));
            menu.SubMenu("harass").AddItem(new MenuItem("useQH", "使用 Q 骚扰").SetValue(true));
            menu.SubMenu("harass").AddItem(new MenuItem("useWH", "使用 W 骚扰").SetValue(true));
            menu.SubMenu("harass").AddItem(new MenuItem("useEH", "使用 E 骚扰").SetValue(true));
            menu.SubMenu("harass").AddItem(new MenuItem("useRH", "使用 R 骚扰").SetValue(true));
            menu.SubMenu("harass").AddItem(new MenuItem("harassSlider", "使用R|敌人血量").SetValue(new Slider(30)));

            menu.AddSubMenu(new Menu("清线 设置", "laneclear"));
            menu.SubMenu("laneclear").AddItem(new MenuItem("useQL", "使用 Q 清线").SetValue(true));
            menu.SubMenu("laneclear").AddItem(new MenuItem("useWL", "使用 W 清线").SetValue(true));
            menu.SubMenu("laneclear").AddItem(new MenuItem("useEL", "使用 E 清线").SetValue(true));
            menu.SubMenu("laneclear").AddItem(new MenuItem("useRL", "使用 R 清线").SetValue(true));

            menu.AddSubMenu(new Menu("抢人头 设置", "killsteal"));
            menu.SubMenu("killsteal").AddItem(new MenuItem("useQKS", "使用 Q 抢人头").SetValue(true));
            menu.SubMenu("killsteal").AddItem(new MenuItem("useWKS", "使用 W 抢人头").SetValue(true));
            menu.SubMenu("killsteal").AddItem(new MenuItem("useEKS", "使用 E 抢人头").SetValue(true));
            menu.SubMenu("killsteal").AddItem(new MenuItem("useRKS", "使用 R 抢人头").SetValue(true));

            menu.AddSubMenu(new Menu("逃跑 设置", "flee"));
            menu.SubMenu("flee").AddItem(new MenuItem("useQF", "使用 Q 逃跑").SetValue(true));
            menu.SubMenu("flee").AddItem(new MenuItem("useRF", "逃跑使用 R 命中小兵回血").SetValue(false));

            menu.AddSubMenu(new Menu("显示 设置", "drawing"));
            menu.SubMenu("drawing").AddItem(new MenuItem("drawQ", "显示 Q 范围").SetValue(true));
            menu.SubMenu("drawing").AddItem(new MenuItem("drawW", "显示 W 范围").SetValue(true));
            menu.SubMenu("drawing").AddItem(new MenuItem("drawE", "显示 E 范围").SetValue(true));
            menu.SubMenu("drawing").AddItem(new MenuItem("drawR", "显示 R 范围").SetValue(true));

            menu.AddSubMenu(new Menu("杂项 设置", "misc"));
            //TODO idk ?

            menu.AddItem(new MenuItem("creds", "作者 iJabba & DZ191"));
        }

        private void onUpdate(EventArgs args) {
            if (player.IsDead) return;
            Obj_AI_Hero target = SimpleTs.GetTarget(isMenuEnabled(menu, "gapcloseQ") ? Q.Range*3 : Q.Range,
                SimpleTs.DamageType.Physical);

            /*foreach (BuffInstance buff in player.Buffs) {
                Game.PrintChat(buff.Name);   
            }*/

            switch (xSLxOrbwalker.CurrentMode) {
                case xSLxOrbwalker.Mode.Combo:
                    doCombo(target);
                    break;
                case xSLxOrbwalker.Mode.LaneClear:
                    laneclear();
                    break;
            }
        }

        private void doCombo(Obj_AI_Hero target) {
            //xSLxOrbwalker.ForcedTarget = target; TODO not needed m8
            if (isMenuEnabled(menu, "gapcloseQ") && player.Distance(target) > Q.Range) {
                List<Obj_AI_Base> jumpMinions = MinionManager.GetMinions(player.Position, Q.Range);
                foreach (Obj_AI_Base minion in jumpMinions) {
                    if (Q.IsReady() && Q.GetDamage(minion) >= minion.Health &&
                        minion.Distance(target.Position) < Q.Range && minion.IsValidTarget(Q.Range*3)) {
                        Q.Cast(minion, true);
                    }
                }
            }
            //TODO: note, removed the else statment, because not rly needed it may cause problems when target is actually in Q Range and doesn't need to be gapclosed.
            if (isMenuEnabled(menu, "useQC") && player.Distance(target) <= Q.Range) {
                if (target.IsValidTarget(Q.Range) && Q.IsReady())
                    Q.Cast(target, true);
            }

            /*if (isMenuEnabled(menu, "useWC") && W.IsReady()) {
                W.Cast(true);
            }* TODO before attack mate/ */
            if (isMenuEnabled(menu, "OStunE")) {
                if (canStun(target) && E.IsReady() && player.Distance(target) <= E.Range) {
                    E.Cast(target, true);
                }
            }
            else {
                if (E.IsReady() && player.Distance(target) <= E.Range) {
                    E.Cast(target, true);
                }
            }

            //TODO find buff name for keeping count of charges? == IreliaTranscendentBlades
            if (R.IsReady() && player.Distance(target) <= R.Range) {
                if (isMenuEnabled(menu, "useRC")) {
                    PredictionOutput rPrediction = R.GetPrediction(target, true);
                    if (rPrediction.Hitchance >= HitChance.Medium)
                        R.Cast(rPrediction.UnitPosition, true);
                }
            }
        }


        private void SuperDuperOpChaseMode(Obj_AI_Hero target) {
            if (!SelectedMinion.IsValid || !R.IsReady()) {
                SelectedMinion = null;
                NumberR = 0;
            }
            if (SelectedMinion.IsValidTarget(R.Range) && R.IsReady() && NumberR > 0) {
                if (SelectedMinion != null) {
                    R.Cast(SelectedMinion.Position);
                    NumberR -= 1;
                    PredictedArrivalTime = Game.Time + (player.Distance(SelectedMinion)/R.Speed);
                }
            }
            if (NumberR == 0 && Q.IsReady() && SelectedMinion.IsValidTarget(Q.Range) && !QCastedMinion &&
                Game.Time > PredictedArrivalTime) {
                Q.Cast(SelectedMinion);
                QCastedMinion = true;
                PredictedArrivalTime = Game.Time;
            }
            if (QCastedMinion && Q.IsReady() && target.IsValidTarget(Q.Range)) {
                Q.Cast(target);
                QCastedMinion = false;
                return;
            }
            if (SelectedMinion == null) {
                List<Obj_AI_Base> MinionList = MinionManager.GetMinions(player.Position, Q.Range).ToList();
                var List2 = MinionList.Where(min => min.Distance(target) <= Q.Range).ToList();
                if (!List2.Any()) return;
                IOrderedEnumerable<Obj_AI_Base> List3 = List2.OrderBy(m => m.Distance(target));
                Obj_AI_Base minion = List3.First();
                int rCount = getNumberOfR(minion);
                if (rCount == 0 && Q.IsReady() && minion.IsValidTarget(Q.Range)) {
                    Q.Cast(minion);
                    QCastedMinion = true;
                }
                else {
                    SelectedMinion = minion;
                    NumberR = rCount;
                }
            }
        }

        private int getNumberOfR(Obj_AI_Base target) {
            int rCount = 0;
            float targetHealth = target.Health;
            while (targetHealth >= Q.GetDamage(target)) {
                rCount += 1;
                targetHealth -= Q.GetDamage(target);
            }
            return rCount;
        }

        private int getUltStacks() {
            foreach (BuffInstance buff in player.Buffs.Where(buff => buff.Name == "IreliaTranscendentBlades")) {
                return buff.Count; // might need to be buff.count - 1
            }
            return 4; // idk might work :^)
        }

        private void onDraw(EventArgs args) {}

        private bool canStun(Obj_AI_Hero target) {
            return target.Health/target.MaxHealth*100 > player.Health/player.MaxHealth*100;
        }

        private void laneclear() {
            List<Obj_AI_Base> minions = MinionManager.GetMinions(player.Position, Q.Range);
            foreach (Obj_AI_Base minion in minions) {
                if (isMenuEnabled(menu, "useWL") && W.IsReady()) {
                    W.Cast(true);
                }
                if (minion.Distance(player) <= Q.Range && Q.GetDamage(minion) >= minion.Health &&
                    isMenuEnabled(menu, "useQL")) {
                    Q.Cast(minion, true);
                }
                if (minion.Distance(player) <= E.Range && isMenuEnabled(menu, "useEL") && E.IsReady()) {
                    E.Cast(minion, true);
                }
                if (R.IsReady() && isMenuEnabled(menu, "useRL") && minion.Distance(player) <= R.Range) {
                    R.Cast(minion, true);
                }
            }
        }
    }
}
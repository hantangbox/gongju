using System;
using System.Linq;
using Color = System.Drawing.Color;

using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

using Orbwalk = MasterCommon.M_Orbwalker;

namespace MasterPlugin
{
    class Jax : Master.Program
    {
        private Int32 Sheen = 3057, Trinity = 3078;
        private bool WardCasted = false, ECasted = false;
        private int RCount = 0;

        public Jax()
        {
            SkillQ = new Spell(SpellSlot.Q, 700);
            SkillW = new Spell(SpellSlot.W, 300);
            SkillE = new Spell(SpellSlot.E, 300);
            SkillR = new Spell(SpellSlot.R, 100);
            SkillQ.SetTargetted(-0.5f, 0);
            SkillW.SetSkillshot(0.0435f, 0, 0, false, SkillshotType.SkillshotCircle);
            SkillE.SetSkillshot(0, 0, 1450, false, SkillshotType.SkillshotCircle);

            var ChampMenu = new Menu(Name + " Plugin", Name + "_Plugin");
            {
                var ComboMenu = new Menu("连招", "Combo");
                {
                    ItemBool(ComboMenu, "Q", "使用 Q");
                    ItemBool(ComboMenu, "W", "使用 W");
                    ItemList(ComboMenu, "WMode", "-> 模式", new[] { "After AA", "After R" });
                    ItemBool(ComboMenu, "E", "使用 E");
                    ItemBool(ComboMenu, "R", "使用 R");
                    ItemList(ComboMenu, "RMode", "-> 模式", new[] { "玩家血量", "# 敌人" });
                    ItemSlider(ComboMenu, "RUnder", "--> 如果血量低于", 40);
                    ItemSlider(ComboMenu, "RCount", "--> 如果敌人超过", 2, 1, 4);
                    ItemBool(ComboMenu, "Item", "使用物品");
                    ItemBool(ComboMenu, "Ignite", "如果能够击杀自动点燃");
                    ChampMenu.AddSubMenu(ComboMenu);
                }
                var HarassMenu = new Menu("骚扰", "Harass");
                {
                    ItemBool(HarassMenu, "Q", "使用 Q");
                    ItemSlider(HarassMenu, "QAbove", "-> 如果血量低于", 20);
                    ItemBool(HarassMenu, "W", "使用 W");
                    ItemBool(HarassMenu, "E", "使用 E");
                    ChampMenu.AddSubMenu(HarassMenu);
                }
                var ClearMenu = new Menu("清线/清野", "Clear");
                {
                    var SmiteMob = new Menu("如果惩戒能击杀野怪", "SmiteMob");
                    {
                        ItemBool(SmiteMob, "Baron", "大龙");
                        ItemBool(SmiteMob, "Dragon", "小龙");
                        ItemBool(SmiteMob, "Red", "红BUFF");
                        ItemBool(SmiteMob, "Blue", "蓝BUFF");
                        ItemBool(SmiteMob, "Krug", "石头怪");
                        ItemBool(SmiteMob, "Gromp", "大蛤蟆");
                        ItemBool(SmiteMob, "Raptor", "啄木鸟4兄弟");
                        ItemBool(SmiteMob, "Wolf", "幽灵狼3兄弟");
                        ClearMenu.AddSubMenu(SmiteMob);
                    }
                    ItemBool(ClearMenu, "Q", "使用 Q");
                    ItemBool(ClearMenu, "W", "使用 W");
                    ItemList(ClearMenu, "WMode", "-> 模式", new[] { "之后 AA", "之后 R" });
                    ItemBool(ClearMenu, "E", "使用 E");
                    ItemBool(ClearMenu, "Item", "使用九头蛇");
                    ChampMenu.AddSubMenu(ClearMenu);
                }
                var MiscMenu = new Menu("额外选项", "Misc");
                {
                    ItemBool(MiscMenu, "WJPink", "使用真眼顺眼", false);
                    ItemBool(MiscMenu, "WLastHit", "使用W补刀");
                    ItemBool(MiscMenu, "WQKillSteal", "使用WQ抢人头");
                    ItemBool(MiscMenu, "EAntiGap", "对突进者使用E");
                    ItemBool(MiscMenu, "EInterrupt", "使用E打断");
                    ItemBool(MiscMenu, "RSurvive", "尝试使用R求生");
                    ItemSlider(MiscMenu, "CustomSkin", "换肤", 8, 0, 8).ValueChanged += SkinChanger;
                    ChampMenu.AddSubMenu(MiscMenu);
                }
                var DrawMenu = new Menu("显示范围", "Draw");
                {
                    ItemBool(DrawMenu, "Q", "Q 范围", false);
                    ItemBool(DrawMenu, "E", "E 范围", false);
                    ChampMenu.AddSubMenu(DrawMenu);
                }
                Config.AddSubMenu(ChampMenu);
            }
            Game.OnGameUpdate += OnGameUpdate;
            Drawing.OnDraw += OnDraw;
            AntiGapcloser.OnEnemyGapcloser += OnEnemyGapcloser;
            Interrupter.OnPossibleToInterrupt += OnPossibleToInterrupt;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            Obj_AI_Minion.OnCreate += OnCreateObjMinion;
            Orbwalk.AfterAttack += AfterAttack;
        }

        private void OnGameUpdate(EventArgs args)
        {
            if (Player.IsDead || MenuGUI.IsChatOpen || Player.IsChannelingImportantSpell() || Player.IsRecalling())
            {
                if (Player.IsDead) RCount = 0;
                return;
            }
            switch (Orbwalk.CurrentMode)
            {
                case Orbwalk.Mode.Combo:
                    NormalCombo();
                    break;
                case Orbwalk.Mode.Harass:
                    Harass();
                    break;
                case Orbwalk.Mode.LaneClear:
                    LaneJungClear();
                    break;
                case Orbwalk.Mode.LaneFreeze:
                    LaneJungClear();
                    break;
                case Orbwalk.Mode.LastHit:
                    LastHit();
                    break;
                case Orbwalk.Mode.Flee:
                    WardJump(Game.CursorPos);
                    break;
            }
            if (ItemBool("Misc", "WQKillSteal")) KillSteal();
        }

        private void OnDraw(EventArgs args)
        {
            if (Player.IsDead) return;
            if (ItemBool("Draw", "Q") && SkillQ.Level > 0) Utility.DrawCircle(Player.Position, SkillQ.Range, SkillQ.IsReady() ? Color.Green : Color.Red);
            if (ItemBool("Draw", "E") && SkillE.Level > 0) Utility.DrawCircle(Player.Position, SkillE.Range, SkillE.IsReady() ? Color.Green : Color.Red);
        }

        private void OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (!ItemBool("Misc", "EAntiGap") || Player.IsDead) return;
            if (IsValid(gapcloser.Sender, SkillE.Range + 10) && SkillE.IsReady()) SkillE.Cast(PacketCast());
        }

        private void OnPossibleToInterrupt(Obj_AI_Base unit, InterruptableSpell spell)
        {
            if (!ItemBool("Misc", "EInterrupt") || Player.IsDead) return;
            if (Player.Mana >= SkillQ.Instance.ManaCost + SkillE.Instance.ManaCost && !SkillE.InRange(unit.Position) && IsValid(unit, SkillQ.Range)) SkillQ.CastOnUnit(unit, PacketCast());
            if (IsValid(unit, SkillE.Range) && SkillE.IsReady()) SkillE.Cast(PacketCast());
        }

        private void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (Player.IsDead) return;
            if (sender.IsMe)
            {
                if (Orbwalk.IsAutoAttack(args.SData.Name) && IsValid((Obj_AI_Base)args.Target) && SkillW.IsReady() && Orbwalk.CurrentMode == Orbwalk.Mode.LastHit && ItemBool("Misc", "WLastHit") && SkillW.GetHealthPrediction((Obj_AI_Base)args.Target) + 5 <= GetBonusDmg((Obj_AI_Base)args.Target) && args.Target is Obj_AI_Minion) SkillW.Cast(PacketCast());
                if (args.SData.Name == "JaxCounterStrike")
                {
                    ECasted = true;
                    Utility.DelayAction.Add(1800, () => ECasted = false);
                }
                if (args.SData.Name == "jaxrelentlessattack")
                {
                    RCount = 0;
                    if (SkillW.IsReady() && IsValid((Obj_AI_Base)args.Target, Orbwalk.GetAutoAttackRange() + 50))
                    {
                        switch (Orbwalk.CurrentMode)
                        {
                            case Orbwalk.Mode.Combo:
                                if (ItemBool("Combo", "W") && ItemList("Combo", "WMode") == 1) SkillW.Cast(PacketCast());
                                break;
                            case Orbwalk.Mode.LaneClear:
                                if (ItemBool("Clear", "W") && ItemList("Clear", "WMode") == 1) SkillW.Cast(PacketCast());
                                break;
                            case Orbwalk.Mode.LaneFreeze:
                                if (ItemBool("Clear", "W") && ItemList("Clear", "WMode") == 1) SkillW.Cast(PacketCast());
                                break;
                        }
                    }
                }
            }
            else if (sender.IsEnemy && ItemBool("Misc", "RSurvive") && SkillR.IsReady())
            {
                if (args.Target.IsMe && (Orbwalk.IsAutoAttack(args.SData.Name) && Player.Health <= sender.GetAutoAttackDamage(Player, true)))
                {
                    SkillR.Cast(PacketCast());
                }
                else if ((args.Target.IsMe || (Player.Position.Distance(args.Start) <= args.SData.CastRange[0] && Player.Position.Distance(args.End) <= Orbwalk.GetAutoAttackRange())) && Damage.Spells.ContainsKey((sender as Obj_AI_Hero).ChampionName))
                {
                    for (var i = 3; i > -1; i--)
                    {
                        if (Damage.Spells[(sender as Obj_AI_Hero).ChampionName].FirstOrDefault(a => a.Slot == (sender as Obj_AI_Hero).GetSpellSlot(args.SData.Name, false) && a.Stage == i) != null)
                        {
                            if (Player.Health <= (sender as Obj_AI_Hero).GetSpellDamage(Player, (sender as Obj_AI_Hero).GetSpellSlot(args.SData.Name, false), i)) SkillR.Cast(PacketCast());
                        }
                    }
                }
            }
        }

        private void OnCreateObjMinion(GameObject sender, EventArgs args)
        {
            if (!sender.IsValid<Obj_AI_Minion>() || sender.IsEnemy || Player.IsDead || !SkillQ.IsReady() || !WardCasted) return;
            if (Orbwalk.CurrentMode == Orbwalk.Mode.Flee && Player.Distance3D((Obj_AI_Minion)sender) <= SkillQ.Range + sender.BoundingRadius && sender.Name.EndsWith("Ward"))
            {
                SkillQ.CastOnUnit((Obj_AI_Minion)sender, PacketCast());
                return;
            }
        }

        private void AfterAttack(Obj_AI_Base unit, Obj_AI_Base target)
        {
            if (!unit.IsMe) return;
            RCount += 1;
            if (SkillW.IsReady() && IsValid(target, Orbwalk.GetAutoAttackRange() + 50))
            {
                switch (Orbwalk.CurrentMode)
                {
                    case Orbwalk.Mode.Combo:
                        if (ItemBool("Combo", "W") && ItemList("Combo", "WMode") == 0) SkillW.Cast(PacketCast());
                        break;
                    case Orbwalk.Mode.Harass:
                        if (ItemBool("Harass", "W") && (!ItemBool("Harass", "Q") || (ItemBool("Harass", "Q") && !SkillQ.IsReady()))) SkillW.Cast(PacketCast());
                        break;
                    case Orbwalk.Mode.LaneClear:
                        if (ItemBool("Clear", "W") && ItemList("Clear", "WMode") == 0) SkillW.Cast(PacketCast());
                        break;
                    case Orbwalk.Mode.LaneFreeze:
                        if (ItemBool("Clear", "W") && ItemList("Clear", "WMode") == 0) SkillW.Cast(PacketCast());
                        break;
                }
            }
        }

        private void NormalCombo()
        {
            if (targetObj == null) return;
            if (ItemBool("Combo", "E") && SkillE.IsReady())
            {
                if (!Player.HasBuff("JaxEvasion"))
                {
                    if ((ItemBool("Combo", "Q") && SkillQ.IsReady() && SkillQ.InRange(targetObj.Position)) || SkillE.InRange(targetObj.Position)) SkillE.Cast(PacketCast());
                }
                else if (SkillE.InRange(targetObj.Position) && !IsValid(targetObj, SkillE.Range - 3.5f)) SkillE.Cast(PacketCast());
            }
            if (ItemBool("Combo", "Q") && SkillQ.IsReady() && SkillQ.InRange(targetObj.Position))
            {
                if ((ItemBool("Combo", "E") && SkillE.IsReady() && Player.HasBuff("JaxEvasion") && !SkillE.InRange(targetObj.Position)) || (!Orbwalk.InAutoAttackRange(targetObj) && Player.Distance3D(targetObj) > 450)) SkillQ.CastOnUnit(targetObj, PacketCast());
            }
            if (ItemBool("Combo", "R") && SkillR.IsReady())
            {
                switch (ItemList("Combo", "RMode"))
                {
                    case 0:
                        if (Player.HealthPercentage() <= ItemSlider("Combo", "RUnder")) SkillR.Cast(PacketCast());
                        break;
                    case 1:
                        if (Player.CountEnemysInRange((int)SkillQ.Range) >= ItemSlider("Combo", "RCount")) SkillR.Cast(PacketCast());
                        break;
                }
            }
            if (ItemBool("Combo", "Item")) UseItem(targetObj);
            if (ItemBool("Combo", "Ignite")) CastIgnite(targetObj);
        }

        private void Harass()
        {
            if (targetObj == null) return;
            if (ItemBool("Harass", "W") && SkillW.IsReady() && ItemBool("Harass", "Q") && SkillQ.IsReady() && SkillQ.InRange(targetObj.Position)) SkillW.Cast(PacketCast());
            if (ItemBool("Harass", "E") && SkillE.IsReady())
            {
                if (!Player.HasBuff("JaxEvasion"))
                {
                    if ((ItemBool("Harass", "Q") && SkillQ.IsReady() && SkillQ.InRange(targetObj.Position)) || SkillE.InRange(targetObj.Position)) SkillE.Cast(PacketCast());
                }
                else if (SkillE.InRange(targetObj.Position) && !IsValid(targetObj, SkillE.Range - 3.5f)) SkillE.Cast(PacketCast());
            }
            if (ItemBool("Harass", "Q") && SkillQ.IsReady() && SkillQ.InRange(targetObj.Position) && Player.HealthPercentage() >= ItemList("Harass", "QAbove"))
            {
                if ((ItemBool("Harass", "E") && SkillE.IsReady() && Player.HasBuff("JaxEvasion") && !SkillE.InRange(targetObj.Position)) || (!Orbwalk.InAutoAttackRange(targetObj) && Player.Distance3D(targetObj) > 450)) SkillQ.CastOnUnit(targetObj, PacketCast());
            }
        }

        private void LaneJungClear()
        {
            foreach (var Obj in ObjectManager.Get<Obj_AI_Minion>().Where(i => IsValid(i, SkillQ.Range)).OrderBy(i => i.Health))
            {
                if (SmiteReady() && Obj.Team == GameObjectTeam.Neutral)
                {
                    if ((ItemBool("SmiteMob", "Baron") && Obj.Name.StartsWith("SRU_Baron")) || (ItemBool("SmiteMob", "Dragon") && Obj.Name.StartsWith("SRU_Dragon")) || (!Obj.Name.Contains("Mini") && (
                        (ItemBool("SmiteMob", "Red") && Obj.Name.StartsWith("SRU_Red")) || (ItemBool("SmiteMob", "Blue") && Obj.Name.StartsWith("SRU_Blue")) ||
                        (ItemBool("SmiteMob", "Krug") && Obj.Name.StartsWith("SRU_Krug")) || (ItemBool("SmiteMob", "Gromp") && Obj.Name.StartsWith("SRU_Gromp")) ||
                        (ItemBool("SmiteMob", "Raptor") && Obj.Name.StartsWith("SRU_Razorbeak")) || (ItemBool("SmiteMob", "Wolf") && Obj.Name.StartsWith("SRU_Murkwolf"))))) CastSmite(Obj);
                }
                if (ItemBool("Clear", "E") && SkillE.IsReady())
                {
                    if (!Player.HasBuff("JaxEvasion"))
                    {
                        if ((ItemBool("Clear", "Q") && SkillQ.IsReady()) || SkillE.InRange(Obj.Position)) SkillE.Cast(PacketCast());
                    }
                    else if (SkillE.InRange(Obj.Position) && !ECasted) SkillE.Cast(PacketCast());
                }
                if (ItemBool("Clear", "Q") && SkillQ.IsReady() && ((ItemBool("Clear", "E") && SkillE.IsReady() && Player.HasBuff("JaxEvasion") && !SkillE.InRange(Obj.Position)) || (!Orbwalk.InAutoAttackRange(Obj) && Player.Distance3D(Obj) > 450))) SkillQ.CastOnUnit(Obj, PacketCast());
                if (ItemBool("Clear", "Item")) UseItem(Obj, true);
            }
        }

        private void LastHit()
        {
            if (!ItemBool("Misc", "WLastHit") || !SkillW.IsReady() || !Player.HasBuff("JaxEmpowerTwo")) return;
            foreach (var Obj in ObjectManager.Get<Obj_AI_Minion>().Where(i => IsValid(i, Orbwalk.GetAutoAttackRange() + 50) && SkillW.GetHealthPrediction(i) + 5 <= GetBonusDmg(i)).OrderBy(i => i.Health).OrderBy(i => i.Distance3D(Player)))
            {
                Orbwalk.SetAttack(false);
                Player.IssueOrder(GameObjectOrder.AttackUnit, Obj);
                Orbwalk.SetAttack(true);
                break;
            }
        }

        private void WardJump(Vector3 Pos)
        {
            if (!SkillQ.IsReady()) return;
            bool IsWard = false;
            foreach (var Obj in ObjectManager.Get<Obj_AI_Base>().Where(i => IsValid(i, SkillQ.Range + i.BoundingRadius, false) && !(i is Obj_AI_Turret) && i.Position.Distance(Pos) < 230).OrderBy(i => i.Position.Distance(Pos)))
            {
                SkillQ.CastOnUnit(Obj, PacketCast());
                if (Obj.Name.EndsWith("Ward") && Obj.IsMinion)
                {
                    IsWard = true;
                }
                else return;
            }
            if (!IsWard && (GetWardSlot() != null || GetWardSlot().Stacks > 0) && !WardCasted)
            {
                GetWardSlot().UseItem(Player.Position.Distance(Pos) > GetWardRange(GetWardSlot().Id) ? Player.Position.To2D().Extend(Pos.To2D(), GetWardRange(GetWardSlot().Id)).To3D() : Pos);
                WardCasted = true;
                Utility.DelayAction.Add(1000, () => WardCasted = false);
            }
        }

        private void KillSteal()
        {
            if (Player.Mana < SkillQ.Instance.ManaCost + SkillW.Instance.ManaCost) return;
            foreach (var Obj in ObjectManager.Get<Obj_AI_Hero>().Where(i => IsValid(i, SkillQ.Range) && SkillQ.GetHealthPrediction(i) + 5 <= SkillQ.GetDamage(i) + GetBonusDmg(i) && i != targetObj).OrderBy(i => i.Health).OrderByDescending(i => i.Distance3D(Player)))
            {
                if (SkillW.IsReady()) SkillW.Cast(PacketCast());
                if (SkillQ.IsReady() && Player.HasBuff("JaxEmpowerTwo")) SkillQ.CastOnUnit(Obj, PacketCast());
            }
        }

        private void UseItem(Obj_AI_Base Target, bool IsFarm = false)
        {
            if (Items.CanUseItem(Bilgewater) && Player.Distance3D(Target) <= 450 && !IsFarm) Items.UseItem(Bilgewater, Target);
            if (Items.CanUseItem(BladeRuined) && Player.Distance3D(Target) <= 450 && !IsFarm) Items.UseItem(BladeRuined, Target);
            if (Items.CanUseItem(Tiamat) && IsFarm ? Player.Distance3D(Target) <= 350 : Player.CountEnemysInRange(350) >= 1) Items.UseItem(Tiamat);
            if (Items.CanUseItem(Hydra) && IsFarm ? Player.Distance3D(Target) <= 350 : (Player.CountEnemysInRange(350) >= 2 || (Player.GetAutoAttackDamage(Target, true) < Target.Health && Player.CountEnemysInRange(350) == 1))) Items.UseItem(Hydra);
            if (Items.CanUseItem(Randuin) && Player.CountEnemysInRange(450) >= 1 && !IsFarm) Items.UseItem(Randuin);
        }

        private double GetBonusDmg(Obj_AI_Base Target)
        {
            double DmgItem = 0;
            if (Items.HasItem(Sheen) && ((Items.CanUseItem(Sheen) && SkillW.IsReady()) || Player.HasBuff("Sheen")) && Player.BaseAttackDamage > DmgItem) DmgItem = Player.BaseAttackDamage;
            if (Items.HasItem(Trinity) && ((Items.CanUseItem(Trinity) && SkillW.IsReady()) || Player.HasBuff("Sheen")) && Player.BaseAttackDamage * 2 > DmgItem) DmgItem = Player.BaseAttackDamage * 2;
            return (SkillW.IsReady() || Player.HasBuff("JaxEmpowerTwo") ? SkillW.GetDamage(Target) : 0) + (RCount >= 2 ? SkillR.GetDamage(Target) : 0) + Player.GetAutoAttackDamage(Target, true) + Player.CalcDamage(Target, Damage.DamageType.Physical, DmgItem);
        }
    }
}
using System;
using System.Linq;
using Color = System.Drawing.Color;

using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

using Orbwalk = MasterCommon.M_Orbwalker;

namespace MasterPlugin
{
    class Renekton : Master.Program
    {
        private Vector3 HarassBackPos = default(Vector3);
        private bool ECasted = false;
        private int AACount = 0;

        public Renekton()
        {
            SkillQ = new Spell(SpellSlot.Q, 325);
            SkillW = new Spell(SpellSlot.W, 300);
            SkillE = new Spell(SpellSlot.E, 450);
            SkillR = new Spell(SpellSlot.R, 20);
            SkillQ.SetSkillshot(-0.5f, 0, 0, false, SkillshotType.SkillshotCircle);
            SkillW.SetSkillshot(0.0435f, 0, 0, false, SkillshotType.SkillshotCircle);
            SkillE.SetSkillshot(-0.5f, 50, 20, false, SkillshotType.SkillshotLine);

            var ChampMenu = new Menu(Name + " Plugin", Name + "_Plugin");
            {
                var ComboMenu = new Menu("连招", "Combo");
                {
                    ItemBool(ComboMenu, "Q", "使用 Q");
                    ItemBool(ComboMenu, "W", "使用 W");
                    ItemBool(ComboMenu, "E", "使用 E");
                    ItemBool(ComboMenu, "Item", "使用 物品");
                    ItemBool(ComboMenu, "Ignite", "如果能击杀自动点燃");
                    ChampMenu.AddSubMenu(ComboMenu);
                }
                var HarassMenu = new Menu("骚扰", "Harass");
                {
                    ItemBool(HarassMenu, "Q", "使用 Q");
                    ItemBool(HarassMenu, "W", "使用 W");
                    ItemBool(HarassMenu, "E", "使用 E");
                    ItemSlider(HarassMenu, "EAbove", "-> 如果血量超过", 20);
                    ChampMenu.AddSubMenu(HarassMenu);
                }
                var ClearMenu = new Menu("清线/清野", "Clear");
                {
                    ItemBool(ClearMenu, "Q", "使用 Q");
                    ItemBool(ClearMenu, "W", "使用 W");
                    ItemBool(ClearMenu, "E", "使用 E");
                    ItemBool(ClearMenu, "Item", "使用九头蛇");
                    ChampMenu.AddSubMenu(ClearMenu);
                }
                var UltiMenu = new Menu("大招", "Ultimate");
                {
                    ItemBool(UltiMenu, "RSurvive", "尝试使用R求生");
                    ItemSlider(UltiMenu, "RUnder", "-> 如果血量小于", 30);
                    ChampMenu.AddSubMenu(UltiMenu);
                }
                var MiscMenu = new Menu("额外选项", "Misc");
                {
                    ItemBool(MiscMenu, "WAntiGap", "对突进者使用W");
                    ItemBool(MiscMenu, "WInterrupt", "使用W打断");
                    ItemBool(MiscMenu, "WCancel", "取消W动画");
                    ItemSlider(MiscMenu, "CustomSkin", "换肤", 6, 0, 6).ValueChanged += SkinChanger;
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
            Orbwalk.AfterAttack += AfterAttack;
        }

        private void OnGameUpdate(EventArgs args)
        {
            if (Player.IsDead || MenuGUI.IsChatOpen || Player.IsChannelingImportantSpell() || Player.IsRecalling())
            {
                if (Player.IsDead) AACount = 0;
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
                case Orbwalk.Mode.Flee:
                    if (SkillE.IsReady()) SkillE.Cast(Game.CursorPos, PacketCast());
                    break;
            }
        }

        private void OnDraw(EventArgs args)
        {
            if (Player.IsDead) return;
            if (ItemBool("Draw", "Q") && SkillQ.Level > 0) Utility.DrawCircle(Player.Position, SkillQ.Range, SkillQ.IsReady() ? Color.Green : Color.Red);
            if (ItemBool("Draw", "E") && SkillE.Level > 0) Utility.DrawCircle(Player.Position, SkillE.Range, SkillE.IsReady() ? Color.Green : Color.Red);
        }

        private void OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (!ItemBool("Misc", "WAntiGap") || Player.IsDead) return;
            if (IsValid(gapcloser.Sender, Orbwalk.GetAutoAttackRange() + 50) && (SkillW.IsReady() || Player.HasBuff("RenektonExecuteReady")))
            {
                if (SkillW.IsReady()) SkillW.Cast(PacketCast());
                if (Player.HasBuff("RenektonExecuteReady")) Player.IssueOrder(GameObjectOrder.AttackUnit, gapcloser.Sender);
            }
        }

        private void OnPossibleToInterrupt(Obj_AI_Base unit, InterruptableSpell spell)
        {
            if (!ItemBool("Misc", "WInterrupt") || Player.IsDead) return;
            if ((SkillW.IsReady() || Player.HasBuff("RenektonExecuteReady")) && SkillE.IsReady() && !SkillW.InRange(unit.Position) && IsValid(unit, SkillE.Range)) SkillE.Cast(unit.Position + Vector3.Normalize(unit.Position - Player.Position) * 200, PacketCast());
            if (IsValid(unit, Orbwalk.GetAutoAttackRange() + 50) && (SkillW.IsReady() || Player.HasBuff("RenektonExecuteReady")))
            {
                if (SkillW.IsReady()) SkillW.Cast(PacketCast());
                if (Player.HasBuff("RenektonExecuteReady")) Player.IssueOrder(GameObjectOrder.AttackUnit, unit);
            }
        }

        private void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (Player.IsDead) return;
            if (sender.IsMe)
            {
                if (Orbwalk.IsAutoAttack(args.SData.Name) && IsValid((Obj_AI_Base)args.Target) && SkillW.IsReady())
                {
                    var Obj = (Obj_AI_Base)args.Target;
                    if ((Orbwalk.CurrentMode == Orbwalk.Mode.LaneClear || Orbwalk.CurrentMode == Orbwalk.Mode.LaneFreeze) && ItemBool("Clear", "W") && args.Target is Obj_AI_Minion && (CanKill(Obj, SkillW, Player.Mana >= 50 ? 1 : 0) || Obj.MaxHealth >= 1200))
                    {
                        SkillW.Cast(PacketCast());
                    }
                    else if ((Orbwalk.CurrentMode == Orbwalk.Mode.Combo || Orbwalk.CurrentMode == Orbwalk.Mode.Harass) && ItemBool(Orbwalk.CurrentMode.ToString(), "W") && args.Target is Obj_AI_Hero) SkillW.Cast(PacketCast());
                }
                if (args.SData.Name == "RenektonCleave") AACount = 0;
                if (args.SData.Name == "RenektonPreExecute") AACount = 0;
                if (args.SData.Name == "RenektonSliceAndDice")
                {
                    ECasted = true;
                    Utility.DelayAction.Add((Orbwalk.CurrentMode == Orbwalk.Mode.LaneClear || Orbwalk.CurrentMode == Orbwalk.Mode.LaneFreeze) ? 3000 : 1800, () => ECasted = false);
                    AACount = 0;
                }
                if (args.SData.Name == "renektondice") AACount = 0;
            }
            else if (sender.IsEnemy && ItemBool("Ultimate", "RSurvive") && SkillR.IsReady())
            {
                if (args.Target.IsMe && ((Orbwalk.IsAutoAttack(args.SData.Name) && (Player.Health - sender.GetAutoAttackDamage(Player, true)) * 100 / Player.MaxHealth <= ItemSlider("Ultimate", "RUnder")) || (args.SData.Name == "summonerdot" && (Player.Health - (sender as Obj_AI_Hero).GetSummonerSpellDamage(Player, Damage.SummonerSpell.Ignite)) * 100 / Player.MaxHealth <= ItemSlider("Ultimate", "RUnder"))))
                {
                    SkillR.Cast(PacketCast());
                }
                else if ((args.Target.IsMe || (Player.Position.Distance(args.Start) <= args.SData.CastRange[0] && Player.Position.Distance(args.End) <= Orbwalk.GetAutoAttackRange())) && Damage.Spells.ContainsKey((sender as Obj_AI_Hero).ChampionName))
                {
                    for (var i = 3; i > -1; i--)
                    {
                        if (Damage.Spells[(sender as Obj_AI_Hero).ChampionName].FirstOrDefault(a => a.Slot == (sender as Obj_AI_Hero).GetSpellSlot(args.SData.Name, false) && a.Stage == i) != null)
                        {
                            if ((Player.Health - (sender as Obj_AI_Hero).GetSpellDamage(Player, (sender as Obj_AI_Hero).GetSpellSlot(args.SData.Name, false), i)) * 100 / Player.MaxHealth <= ItemSlider("Ultimate", "RUnder")) SkillR.Cast(PacketCast());
                        }
                    }
                }
            }
        }

        private void AfterAttack(Obj_AI_Base unit, Obj_AI_Base target)
        {
            if (!unit.IsMe) return;
            AACount += 1;
            if ((Orbwalk.CurrentMode == Orbwalk.Mode.Combo || Orbwalk.CurrentMode == Orbwalk.Mode.Harass) && ItemBool(Orbwalk.CurrentMode.ToString(), "W") && ItemBool("Misc", "WCancel") && target.Buffs.Any(i => i.SourceName == Name && i.DisplayName == "Stun") && target is Obj_AI_Hero) UseItem(target, true);
        }

        private void NormalCombo()
        {
            if (targetObj == null) return;
            if (ItemBool("Combo", "W") && (SkillW.IsReady() || Player.HasBuff("RenektonExecuteReady")) && !Player.IsDashing() && Orbwalk.InAutoAttackRange(targetObj))
            {
                Orbwalk.SetAttack(false);
                Player.IssueOrder(GameObjectOrder.AttackUnit, targetObj);
                Orbwalk.SetAttack(true);
            }
            if (ItemBool("Combo", "Q") && SkillQ.IsReady() && !Player.IsDashing() && SkillQ.InRange(targetObj.Position)) SkillQ.Cast(PacketCast());
            if (ItemBool("Combo", "E") && SkillE.IsReady() && SkillE.InRange(targetObj.Position))
            {
                if (SkillE.Instance.Name == "RenektonSliceAndDice")
                {
                    SkillE.Cast(Player.Position.To2D().Extend(targetObj.Position.To2D(), targetObj.Distance3D(Player) + 200), PacketCast());
                }
                else if (!ECasted || !IsValid(targetObj, SkillE.Range - 30) || CanKill(targetObj, SkillE, Player.Mana >= 50 ? 1 : 0)) SkillE.Cast(Player.Position.To2D().Extend(targetObj.Position.To2D(), targetObj.Distance3D(Player) + 200), PacketCast());
            }
            if (ItemBool("Combo", "Item")) UseItem(targetObj);
            if (ItemBool("Combo", "Ignite")) CastIgnite(targetObj);
        }

        private void Harass()
        {
            if (targetObj == null) return;
            if (ItemBool("Harass", "W") && (SkillW.IsReady() || Player.HasBuff("RenektonExecuteReady")) && !Player.IsDashing() && (AACount >= 1 || (SkillE.IsReady() && SkillE.Instance.Name != "RenektonSliceAndDice")) && Orbwalk.InAutoAttackRange(targetObj))
            {
                Orbwalk.SetAttack(false);
                Player.IssueOrder(GameObjectOrder.AttackUnit, targetObj);
                Orbwalk.SetAttack(true);
            }
            if (ItemBool("Harass", "Q") && SkillQ.IsReady() && !Player.IsDashing() && AACount >= 2 && SkillQ.InRange(targetObj.Position)) SkillQ.Cast(PacketCast());
            if (ItemBool("Harass", "E") && !Player.IsDashing())
            {
                if (SkillE.IsReady())
                {
                    if (SkillE.Instance.Name == "RenektonSliceAndDice")
                    {
                        if (SkillE.InRange(targetObj.Position) && Player.HealthPercentage() >= ItemSlider("Harass", "EAbove"))
                        {
                            HarassBackPos = Player.ServerPosition;
                            SkillE.Cast(Player.Position.To2D().Extend(targetObj.Position.To2D(), targetObj.Distance3D(Player) + 200), PacketCast());
                        }
                    }
                    else if (!ECasted || AACount >= 2) SkillE.Cast(HarassBackPos, PacketCast());
                }
                else if (HarassBackPos != default(Vector3)) HarassBackPos = default(Vector3);
            }
        }

        private void LaneJungClear()
        {
            var minionObj = ObjectManager.Get<Obj_AI_Base>().Where(i => IsValid(i, SkillE.Range) && i is Obj_AI_Minion).OrderBy(i => i.Health);
            foreach (var Obj in minionObj)
            {
                if (ItemBool("Clear", "Q") && SkillQ.IsReady() && !Player.IsDashing() && (AACount >= 2 || (SkillE.IsReady() && SkillE.Instance.Name != "RenektonSliceAndDice")) && (minionObj.Count(i => IsValid(i, SkillQ.Range)) >= 2 || (Obj.MaxHealth >= 1200 && SkillQ.InRange(Obj.Position)))) SkillQ.Cast(PacketCast());
                if (ItemBool("Clear", "W") && (SkillW.IsReady() || Player.HasBuff("RenektonExecuteReady")) && !Player.IsDashing() && AACount >= 1 && Orbwalk.InAutoAttackRange(Obj) && (CanKill(Obj, SkillW, Player.Mana >= 50 ? 1 : 0) || Obj.MaxHealth >= 1200))
                {
                    Orbwalk.SetAttack(false);
                    Player.IssueOrder(GameObjectOrder.AttackUnit, Obj);
                    Orbwalk.SetAttack(true);
                    break;
                }
                if (ItemBool("Clear", "E") && SkillE.IsReady() && !Player.IsDashing())
                {
                    var posEFarm = SkillE.GetLineFarmLocation(minionObj.ToList());
                    if (SkillE.Instance.Name == "RenektonSliceAndDice")
                    {
                        SkillE.Cast(posEFarm.MinionsHit >= 2 ? posEFarm.Position : Player.Position.To2D().Extend(Obj.Position.To2D(), Obj.Distance3D(Player) + 200), PacketCast());
                    }
                    else if (!ECasted || AACount >= 2) SkillE.Cast(posEFarm.MinionsHit >= 2 ? posEFarm.Position : Player.Position.To2D().Extend(Obj.Position.To2D(), Obj.Distance3D(Player) + 200), PacketCast());
                }
                if (ItemBool("Clear", "Item") && AACount >= 1) UseItem(Obj, true);
            }
        }

        private void UseItem(Obj_AI_Base Target, bool IsFarm = false)
        {
            if (Items.CanUseItem(Tiamat) && IsFarm ? Player.Distance3D(Target) <= 350 : Player.CountEnemysInRange(350) >= 1) Items.UseItem(Tiamat);
            if (Items.CanUseItem(Hydra) && IsFarm ? Player.Distance3D(Target) <= 350 : (Player.CountEnemysInRange(350) >= 2 || (Player.GetAutoAttackDamage(Target, true) < Target.Health && Player.CountEnemysInRange(350) == 1))) Items.UseItem(Hydra);
            if (Items.CanUseItem(Randuin) && Player.CountEnemysInRange(450) >= 1 && !IsFarm) Items.UseItem(Randuin);
        }
    }
}
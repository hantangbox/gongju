using System;
using System.Linq;

using LeagueSharp;
using LeagueSharp.Common;

using Orbwalk = MasterCommon.M_Orbwalker;

namespace MasterPlugin
{
    class Udyr : Master.Program
    {
        private enum Stance
        {
            Tiger,
            Turtle,
            Bear,
            Phoenix,
        }
        private Stance CurStance;
        private int AACount = 0;
        private bool TigerActive = false, PhoenixActive = false;

        public Udyr()
        {
            SkillQ = new Spell(SpellSlot.Q, 600);
            SkillW = new Spell(SpellSlot.W, 600);
            SkillE = new Spell(SpellSlot.E, 600);
            SkillR = new Spell(SpellSlot.R, 325);

            Config.SubMenu("OW").SubMenu("Mode").AddItem(new MenuItem(Name + "_OW_StunCycle", "循环E").SetValue(new KeyBind("Z".ToCharArray()[0], KeyBindType.Press)));
            var ChampMenu = new Menu(Name + " Plugin", Name + "_Plugin");
            {
                var ComboMenu = new Menu("连招", "Combo");
                {
                    ItemBool(ComboMenu, "Q", "使用 Q");
                    ItemBool(ComboMenu, "W", "使用 W");
                    ItemBool(ComboMenu, "E", "使用 E");
                    ItemBool(ComboMenu, "R", "使用 R");
                    ItemBool(ComboMenu, "Item", "使用 物品");
                    ItemBool(ComboMenu, "Ignite", "如果能击杀自动点燃");
                    ChampMenu.AddSubMenu(ComboMenu);
                }
                var HarassMenu = new Menu("骚扰", "Harass");
                {
                    ItemBool(HarassMenu, "Q", "使用 Q");
                    ItemBool(HarassMenu, "W", "使用 W");
                    ItemBool(HarassMenu, "E", "使用 E");
                    ItemBool(HarassMenu, "R", "使用 R");
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
                    ItemBool(ClearMenu, "E", "使用 E");
                    ItemBool(ClearMenu, "R", "使用 R");
                    ItemBool(ClearMenu, "Item", "使用九头蛇");
                    ChampMenu.AddSubMenu(ClearMenu);
                }
                var MiscMenu = new Menu("额外选项", "Misc");
                {
                    ItemBool(MiscMenu, "EAntiGap", "对突进者使用E");
                    ItemBool(MiscMenu, "EInterrupt", "使用E打断");
                    ItemBool(MiscMenu, "WSurvive", "尝试使用W求生");
                    ItemSlider(MiscMenu, "CustomSkin", "换肤", 3, 0, 3).ValueChanged += SkinChanger;
                    ChampMenu.AddSubMenu(MiscMenu);
                }
                Config.AddSubMenu(ChampMenu);
            }
            Game.OnGameUpdate += OnGameUpdate;
            AntiGapcloser.OnEnemyGapcloser += OnEnemyGapcloser;
            Interrupter.OnPossibleToInterrupt += OnPossibleToInterrupt;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            Obj_AI_Base.OnCreate += OnCreate;
            Obj_AI_Base.OnDelete += OnDelete;
            Orbwalk.AfterAttack += AfterAttack;
        }

        private void OnGameUpdate(EventArgs args)
        {
            if (Player.IsDead || MenuGUI.IsChatOpen || Player.IsChannelingImportantSpell() || Player.IsRecalling())
            {
                if (Player.IsDead) AACount = 0;
                return;
            }
            if (Orbwalk.CurrentMode == Orbwalk.Mode.Combo || Orbwalk.CurrentMode == Orbwalk.Mode.Harass)
            {
                NormalCombo(Orbwalk.CurrentMode.ToString());
            }
            else if (Orbwalk.CurrentMode == Orbwalk.Mode.LaneClear || Orbwalk.CurrentMode == Orbwalk.Mode.LaneFreeze)
            {
                LaneJungClear();
            }
            else if (Orbwalk.CurrentMode == Orbwalk.Mode.Flee) Flee();
            if (ItemActive("StunCycle"))
            {
                StunCycle();
            }
            else Orbwalk.CustomMode = false;
        }

        private void OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (!ItemBool("Misc", "EAntiGap") || Player.IsDead) return;
            if (IsValid(gapcloser.Sender, Orbwalk.GetAutoAttackRange() + 100) && !gapcloser.Sender.HasBuff("UdyrBearStunCheck") && (SkillE.IsReady() || CurStance == Stance.Bear))
            {
                if (CurStance != Stance.Bear) SkillE.Cast(PacketCast());
                if (CurStance == Stance.Bear) Player.IssueOrder(GameObjectOrder.AttackUnit, gapcloser.Sender);
            }
        }

        private void OnPossibleToInterrupt(Obj_AI_Base unit, InterruptableSpell spell)
        {
            if (!ItemBool("Misc", "EInterrupt") || Player.IsDead) return;
            if (IsValid(unit, SkillE.Range) && !unit.HasBuff("UdyrBearStunCheck") && (SkillE.IsReady() || CurStance == Stance.Bear))
            {
                if (CurStance != Stance.Bear) SkillE.Cast(PacketCast());
                if (CurStance == Stance.Bear) Player.IssueOrder(GameObjectOrder.AttackUnit, unit);
            }
        }

        private void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (Player.IsDead) return;
            if (sender.IsMe)
            {
                if (args.SData.Name == "UdyrTigerStance")
                {
                    CurStance = Stance.Tiger;
                    AACount = 0;
                }
                if (args.SData.Name == "UdyrTurtleStance")
                {
                    CurStance = Stance.Turtle;
                    AACount = 0;
                }
                if (args.SData.Name == "UdyrBearStance")
                {
                    CurStance = Stance.Bear;
                    AACount = 0;
                }
                if (args.SData.Name == "UdyrPhoenixStance")
                {
                    CurStance = Stance.Phoenix;
                    AACount = 0;
                }
            }
            else if (sender.IsEnemy && ItemBool("Misc", "WSurvive") && SkillW.IsReady())
            {
                if (args.Target.IsMe && ((Orbwalk.IsAutoAttack(args.SData.Name) && Player.Health <= sender.GetAutoAttackDamage(Player, true)) || (args.SData.Name == "summonerdot" && Player.Health <= (sender as Obj_AI_Hero).GetSummonerSpellDamage(Player, Damage.SummonerSpell.Ignite))))
                {
                    SkillW.Cast(PacketCast());
                }
                else if ((args.Target.IsMe || (Player.Position.Distance(args.Start) <= args.SData.CastRange[0] && Player.Position.Distance(args.End) <= Orbwalk.GetAutoAttackRange())) && Damage.Spells.ContainsKey((sender as Obj_AI_Hero).ChampionName))
                {
                    for (var i = 3; i > -1; i--)
                    {
                        if (Damage.Spells[(sender as Obj_AI_Hero).ChampionName].FirstOrDefault(a => a.Slot == (sender as Obj_AI_Hero).GetSpellSlot(args.SData.Name, false) && a.Stage == i) != null)
                        {
                            if (Player.Health <= (sender as Obj_AI_Hero).GetSpellDamage(Player, (sender as Obj_AI_Hero).GetSpellSlot(args.SData.Name, false), i)) SkillW.Cast(PacketCast());
                        }
                    }
                }
            }
        }

        private void OnCreate(GameObject sender, EventArgs args)
        {
            if (Player.Position.Distance(sender.Position) <= 70 && (sender.Name == "Udyr_PhoenixBreath_cas.troy" || sender.Name == "Udyr_Spirit_Phoenix_Breath_cas.troy")) PhoenixActive = true;
            if (Player.Position.Distance(sender.Position) <= 450 && (sender.Name == "udyr_tiger_claw_tar.troy" || sender.Name == "Udyr_Spirit_Tiger_Claw_tar.troy")) TigerActive = true;
        }

        private void OnDelete(GameObject sender, EventArgs args)
        {
            if (Player.Position.Distance(sender.Position) <= 70 && (sender.Name == "Udyr_PhoenixBreath_cas.troy" || sender.Name == "Udyr_Spirit_Phoenix_Breath_cas.troy")) PhoenixActive = false;
            if (Player.Position.Distance(sender.Position) <= 450 && (sender.Name == "udyr_tiger_claw_tar.troy" || sender.Name == "Udyr_Spirit_Tiger_Claw_tar.troy")) TigerActive = false;
        }

        private void AfterAttack(Obj_AI_Base unit, Obj_AI_Base target)
        {
            if (!unit.IsMe) return;
            if (CurStance == Stance.Tiger || CurStance == Stance.Phoenix) AACount += 1;
        }

        private void NormalCombo(string Mode)
        {
            if (targetObj == null) return;
            if (ItemBool(Mode, "E") && SkillE.IsReady() && !targetObj.HasBuff("UdyrBearStunCheck") && Player.Distance3D(targetObj) <= ((Mode == "Combo") ? 800 : Orbwalk.GetAutoAttackRange() + 100)) SkillE.Cast(PacketCast());
            if (Player.Distance3D(targetObj) <= Orbwalk.GetAutoAttackRange() + 50 && (!ItemBool(Mode, "E") || (ItemBool(Mode, "E") && (SkillE.Level == 0 || targetObj.HasBuff("UdyrBearStunCheck")))))
            {
                if (ItemBool(Mode, "Q") && SkillQ.IsReady()) SkillQ.Cast(PacketCast());
                if (ItemBool(Mode, "R") && SkillR.IsReady() && (!ItemBool(Mode, "Q") || (ItemBool(Mode, "Q") && (SkillQ.Level == 0 || (CurStance == Stance.Tiger && (AACount >= 2 || TigerActive)))))) SkillR.Cast(PacketCast());
                if (ItemBool(Mode, "W") && SkillW.IsReady())
                {
                    if ((CurStance == Stance.Tiger && (AACount >= 2 || TigerActive)) || (CurStance == Stance.Phoenix && (AACount >= 3 || PhoenixActive)))
                    {
                        SkillW.Cast(PacketCast());
                    }
                    else if (SkillQ.Level == 0 && SkillR.Level == 0) SkillW.Cast(PacketCast());
                }
            }
            if (ItemBool(Mode, "Item") && Mode == "Combo") UseItem(targetObj);
            if (ItemBool(Mode, "Ignite") && Mode == "Combo") CastIgnite(targetObj);
        }

        private void LaneJungClear()
        {
            foreach (var Obj in ObjectManager.Get<Obj_AI_Minion>().Where(i => IsValid(i, 800)).OrderBy(i => i.Health))
            {
                var SpecialMob = new[] { "SRU_Baron", "SRU_Dragon" }.Any(i => Obj.Name.StartsWith(i));
                if (SmiteReady() && Obj.Team == GameObjectTeam.Neutral)
                {
                    if ((ItemBool("SmiteMob", "Baron") && Obj.Name.StartsWith("SRU_Baron")) || (ItemBool("SmiteMob", "Dragon") && Obj.Name.StartsWith("SRU_Dragon")) || (!Obj.Name.Contains("Mini") && (
                        (ItemBool("SmiteMob", "Red") && Obj.Name.StartsWith("SRU_Red")) || (ItemBool("SmiteMob", "Blue") && Obj.Name.StartsWith("SRU_Blue")) ||
                        (ItemBool("SmiteMob", "Krug") && Obj.Name.StartsWith("SRU_Krug")) || (ItemBool("SmiteMob", "Gromp") && Obj.Name.StartsWith("SRU_Gromp")) ||
                        (ItemBool("SmiteMob", "Raptor") && Obj.Name.StartsWith("SRU_Razorbeak")) || (ItemBool("SmiteMob", "Wolf") && Obj.Name.StartsWith("SRU_Murkwolf"))))) CastSmite(Obj);
                }
                if (ItemBool("Clear", "E") && SkillE.IsReady() && !Obj.HasBuff("UdyrBearStunCheck") && (!SpecialMob || Player.Distance3D(Obj) > Orbwalk.GetAutoAttackRange() + 150)) SkillE.Cast(PacketCast());
                if (Player.Distance3D(Obj) <= Orbwalk.GetAutoAttackRange() + 50 && (!ItemBool("Clear", "E") || (ItemBool("Clear", "E") && (SkillE.Level == 0 || Obj.HasBuff("UdyrBearStunCheck") || SpecialMob))))
                {
                    if (ItemBool("Clear", "Q") && SkillQ.IsReady()) SkillQ.Cast(PacketCast());
                    if (ItemBool("Clear", "R") && SkillR.IsReady() && (!ItemBool("Clear", "Q") || (ItemBool("Clear", "Q") && (SkillQ.Level == 0 || (CurStance == Stance.Tiger && (AACount >= 2 || TigerActive)))))) SkillR.Cast(PacketCast());
                    if (ItemBool("Clear", "W") && SkillW.IsReady())
                    {
                        if ((CurStance == Stance.Tiger && (AACount >= 2 || TigerActive)) || (CurStance == Stance.Phoenix && (AACount >= 3 || PhoenixActive)))
                        {
                            SkillW.Cast(PacketCast());
                        }
                        else if (SkillQ.Level == 0 && SkillR.Level == 0) SkillW.Cast(PacketCast());
                    }
                }
                if (ItemBool("Clear", "Item")) UseItem(Obj, true);
            }
        }

        private void Flee()
        {
            var Passive = Player.Buffs.FirstOrDefault(i => i.DisplayName == "UdyrMonkeyAgilityBuff");
            if (SkillE.IsReady()) SkillE.Cast(PacketCast());
            if (Passive != null && Passive.Count < 3)
            {
                if (SkillQ.IsReady() && (SkillQ.Level > SkillW.Level || SkillQ.Level > SkillR.Level || (SkillQ.Level == SkillW.Level && SkillQ.Level > SkillR.Level) || (SkillQ.Level == SkillR.Level && SkillQ.Level > SkillW.Level) || (SkillQ.Level == SkillW.Level && SkillQ.Level == SkillR.Level)))
                {
                    SkillQ.Cast(PacketCast());
                }
                else if (SkillW.IsReady() && (SkillW.Level > SkillQ.Level || SkillW.Level > SkillR.Level || (SkillW.Level == SkillQ.Level && SkillW.Level > SkillR.Level) || (SkillW.Level == SkillR.Level && SkillW.Level > SkillQ.Level) || (SkillW.Level == SkillQ.Level && SkillW.Level == SkillR.Level)))
                {
                    SkillW.Cast(PacketCast());
                }
                else if (SkillR.IsReady() && (SkillR.Level > SkillQ.Level || SkillR.Level > SkillW.Level || (SkillR.Level == SkillQ.Level && SkillR.Level > SkillW.Level) || (SkillR.Level == SkillW.Level && SkillR.Level > SkillQ.Level) || (SkillR.Level == SkillQ.Level && SkillR.Level == SkillW.Level))) SkillR.Cast(PacketCast());
            }
        }

        private void StunCycle()
        {
            var Obj = ObjectManager.Get<Obj_AI_Hero>().Where(i => IsValid(i, 800) && !i.HasBuff("UdyrBearStunCheck")).OrderBy(i => i.Distance3D(Player)).FirstOrDefault();
            CustomOrbwalk(Obj);
            if (Obj != null && SkillE.IsReady()) SkillE.Cast(PacketCast());
        }

        private void UseItem(Obj_AI_Base Target, bool Farm = false)
        {
            if (Items.CanUseItem(Bilgewater) && Player.Distance3D(Target) <= 450 && !Farm) Items.UseItem(Bilgewater, Target);
            if (Items.CanUseItem(BladeRuined) && Player.Distance3D(Target) <= 450 && !Farm) Items.UseItem(BladeRuined, Target);
            if (Items.CanUseItem(Tiamat) && Farm ? Player.Distance3D(Target) <= 350 : Player.CountEnemysInRange(350) >= 1) Items.UseItem(Tiamat);
            if (Items.CanUseItem(Hydra) && Farm ? Player.Distance3D(Target) <= 350 : (Player.CountEnemysInRange(350) >= 2 || (Player.GetAutoAttackDamage(Target, true) < Target.Health && Player.CountEnemysInRange(350) == 1))) Items.UseItem(Hydra);
            if (Items.CanUseItem(Randuin) && Player.CountEnemysInRange(450) >= 1 && !Farm) Items.UseItem(Randuin);
        }
    }
}
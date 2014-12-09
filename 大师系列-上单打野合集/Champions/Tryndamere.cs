using System;
using System.Linq;
using Color = System.Drawing.Color;

using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

using Orbwalk = MasterCommon.M_Orbwalker;

namespace MasterPlugin
{
    class Tryndamere : Master.Program
    {
        public Tryndamere()
        {
            SkillQ = new Spell(SpellSlot.Q, 318.9f);
            SkillW = new Spell(SpellSlot.W, 850);
            SkillE = new Spell(SpellSlot.E, 650);
            SkillR = new Spell(SpellSlot.R, 400);
            SkillW.SetSkillshot(-0.5f, 0, 500, false, SkillshotType.SkillshotCircle);
            SkillE.SetSkillshot(0, 160, 700, false, SkillshotType.SkillshotLine);

            var ChampMenu = new Menu(Name + " Plugin", Name + "_Plugin");
            {
                var ComboMenu = new Menu("连招", "Combo");
                {
                    ItemBool(ComboMenu, "Q", "使用 Q");
                    ItemSlider(ComboMenu, "QUnder", "-> 如果血量低于", 40);
                    ItemBool(ComboMenu, "W", "使用 W");
                    ItemBool(ComboMenu, "E", "使用 E");
                    ItemBool(ComboMenu, "Item", "使用 物品");
                    ItemBool(ComboMenu, "Ignite", "如果能击杀自动点燃");
                    ChampMenu.AddSubMenu(ComboMenu);
                }
                var HarassMenu = new Menu("骚扰", "Harass");
                {
                    ItemBool(HarassMenu, "W", "使用 W");
                    ItemBool(HarassMenu, "E", "使用 E");
                    ItemSlider(HarassMenu, "EAbove", "-> 如果血量超过", 20);
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
                    ItemSlider(ClearMenu, "QUnder", "-> 如果血量低于", 40);
                    ItemBool(ClearMenu, "E", "使用 E");
                    ItemBool(ClearMenu, "Item", "使用九头蛇");
                    ChampMenu.AddSubMenu(ClearMenu);
                }
                var MiscMenu = new Menu("额外选项", "Misc");
                {
                    ItemBool(MiscMenu, "EKillSteal", "使用E抢人头");
                    ItemBool(MiscMenu, "QSurvive", "尝试使用Q求生");
                    ItemBool(MiscMenu, "RSurvive", "尝试使用R求生");
                    ItemSlider(MiscMenu, "CustomSkin", "换肤", 4, 0, 6).ValueChanged += SkinChanger;
                    ChampMenu.AddSubMenu(MiscMenu);
                }
                var DrawMenu = new Menu("显示范围", "Draw");
                {
                    ItemBool(DrawMenu, "W", "W 范围", false);
                    ItemBool(DrawMenu, "E", "E 范围", false);
                    ChampMenu.AddSubMenu(DrawMenu);
                }
                Config.AddSubMenu(ChampMenu);
            }
            Game.OnGameUpdate += OnGameUpdate;
            Drawing.OnDraw += OnDraw;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
        }

        private void OnGameUpdate(EventArgs args)
        {
            if (Player.IsDead || MenuGUI.IsChatOpen || Player.IsChannelingImportantSpell() || Player.IsRecalling()) return;
            if (Orbwalk.CurrentMode == Orbwalk.Mode.Combo || Orbwalk.CurrentMode == Orbwalk.Mode.Harass)
            {
                NormalCombo(Orbwalk.CurrentMode.ToString());
            }
            else if (Orbwalk.CurrentMode == Orbwalk.Mode.LaneClear || Orbwalk.CurrentMode == Orbwalk.Mode.LaneFreeze)
            {
                LaneJungClear();
            }
            else if (Orbwalk.CurrentMode == Orbwalk.Mode.Flee && SkillE.IsReady()) SkillE.Cast(Game.CursorPos, PacketCast());
            if (ItemBool("Misc", "EKillSteal")) KillSteal();
        }

        private void OnDraw(EventArgs args)
        {
            if (Player.IsDead) return;
            if (ItemBool("Draw", "W") && SkillW.Level > 0) Utility.DrawCircle(Player.Position, SkillW.Range, SkillW.IsReady() ? Color.Green : Color.Red);
            if (ItemBool("Draw", "E") && SkillE.Level > 0) Utility.DrawCircle(Player.Position, SkillE.Range, SkillE.IsReady() ? Color.Green : Color.Red);
        }

        private void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (Player.IsDead) return;
            if (sender.IsEnemy)
            {
                if ((ItemBool("Misc", "QSurvive") && SkillQ.IsReady()) || (ItemBool("Misc", "RSurvive") && SkillR.IsReady()))
                {
                    if (args.Target.IsMe && ((Orbwalk.IsAutoAttack(args.SData.Name) && Player.Health <= sender.GetAutoAttackDamage(Player, true)) || (args.SData.Name == "summonerdot" && Player.Health <= (sender as Obj_AI_Hero).GetSummonerSpellDamage(Player, Damage.SummonerSpell.Ignite))))
                    {
                        if (ItemBool("Misc", "QSurvive") && SkillQ.IsReady())
                        {
                            SkillQ.Cast(PacketCast());
                            return;
                        }
                        if (ItemBool("Misc", "RSurvive") && SkillR.IsReady())
                        {
                            SkillR.Cast(PacketCast());
                            return;
                        }
                    }
                    else if ((args.Target.IsMe || (Player.Position.Distance(args.Start) <= args.SData.CastRange[0] && Player.Position.Distance(args.End) <= Orbwalk.GetAutoAttackRange())) && Damage.Spells.ContainsKey((sender as Obj_AI_Hero).ChampionName))
                    {
                        for (var i = 3; i > -1; i--)
                        {
                            if (Damage.Spells[(sender as Obj_AI_Hero).ChampionName].FirstOrDefault(a => a.Slot == (sender as Obj_AI_Hero).GetSpellSlot(args.SData.Name, false) && a.Stage == i) != null)
                            {
                                if (Player.Health <= (sender as Obj_AI_Hero).GetSpellDamage(Player, (sender as Obj_AI_Hero).GetSpellSlot(args.SData.Name, false), i))
                                {
                                    if (ItemBool("Misc", "QSurvive") && SkillQ.IsReady())
                                    {
                                        SkillQ.Cast(PacketCast());
                                        return;
                                    }
                                    if (ItemBool("Misc", "RSurvive") && SkillR.IsReady())
                                    {
                                        SkillR.Cast(PacketCast());
                                        return;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void NormalCombo(string Mode)
        {
            if (targetObj == null) return;
            if (ItemBool(Mode, "Q") && Mode == "Combo" && SkillQ.IsReady() && Player.HealthPercentage() <= ItemSlider(Mode, "QUnder") && Player.CountEnemysInRange(800) >= 1) SkillQ.Cast(PacketCast());
            if (ItemBool(Mode, "W") && SkillW.IsReady() && SkillW.InRange(targetObj.Position))
            {
                if (Utility.IsBothFacing(Player, targetObj, 35))
                {
                    if (Player.GetAutoAttackDamage(targetObj, true) < targetObj.GetAutoAttackDamage(Player, true) || Player.Health < targetObj.Health) SkillW.Cast(PacketCast());
                }
                else if (Player.IsFacing(targetObj, 35) && !targetObj.IsFacing(Player, 35) && Player.Distance3D(targetObj) > Orbwalk.GetAutoAttackRange() + 50) SkillW.Cast(PacketCast());
            }
            if (ItemBool(Mode, "E") && SkillE.IsReady() && SkillE.InRange(targetObj.Position) && (CanKill(targetObj, SkillE) || (Player.Distance3D(targetObj) > Orbwalk.GetAutoAttackRange() + 50 && (Mode == "Combo" || Player.HealthPercentage() >= ItemSlider(Mode, "EAbove"))))) SkillE.Cast(Player.Position.To2D().Extend(targetObj.Position.To2D(), targetObj.Distance3D(Player) + 200), PacketCast());
            if (ItemBool(Mode, "Item") && Mode == "Combo") UseItem(targetObj);
            if (ItemBool(Mode, "Ignite") && Mode == "Combo") CastIgnite(targetObj);
        }

        private void LaneJungClear()
        {
            var minionObj = ObjectManager.Get<Obj_AI_Base>().Where(i => IsValid(i, SkillE.Range) && i is Obj_AI_Minion).OrderBy(i => i.Health);
            foreach (var Obj in minionObj)
            {
                if (SmiteReady() && Obj.Team == GameObjectTeam.Neutral)
                {
                    if ((ItemBool("SmiteMob", "Baron") && Obj.Name.StartsWith("SRU_Baron")) || (ItemBool("SmiteMob", "Dragon") && Obj.Name.StartsWith("SRU_Dragon")) || (!Obj.Name.Contains("Mini") && (
                        (ItemBool("SmiteMob", "Red") && Obj.Name.StartsWith("SRU_Red")) || (ItemBool("SmiteMob", "Blue") && Obj.Name.StartsWith("SRU_Blue")) ||
                        (ItemBool("SmiteMob", "Krug") && Obj.Name.StartsWith("SRU_Krug")) || (ItemBool("SmiteMob", "Gromp") && Obj.Name.StartsWith("SRU_Gromp")) ||
                        (ItemBool("SmiteMob", "Raptor") && Obj.Name.StartsWith("SRU_Razorbeak")) || (ItemBool("SmiteMob", "Wolf") && Obj.Name.StartsWith("SRU_Murkwolf"))))) CastSmite(Obj);
                }
                if (ItemBool("Clear", "Q") && SkillQ.IsReady() && Player.HealthPercentage() <= ItemSlider("Clear", "QUnder") && (minionObj.Count(i => Orbwalk.InAutoAttackRange(i)) >= 2 || (Obj.MaxHealth >= 1200 && Orbwalk.InAutoAttackRange(Obj)))) SkillQ.Cast(PacketCast());
                if (ItemBool("Clear", "E") && SkillE.IsReady())
                {
                    var posEFarm = SkillE.GetLineFarmLocation(minionObj.ToList());
                    SkillE.Cast(posEFarm.MinionsHit >= 2 ? posEFarm.Position : Obj.Position.To2D(), PacketCast());
                }
                if (ItemBool("Clear", "Item")) UseItem(Obj, true);
            }
        }

        private void KillSteal()
        {
            if (!SkillE.IsReady()) return;
            foreach (var Obj in ObjectManager.Get<Obj_AI_Hero>().Where(i => IsValid(i, SkillE.Range) && CanKill(i, SkillE) && i != targetObj).OrderBy(i => i.Health).OrderBy(i => i.Distance3D(Player))) SkillE.Cast(Player.Position.To2D().Extend(Obj.Position.To2D(), Obj.Distance3D(Player) + 200), PacketCast());
        }

        private void UseItem(Obj_AI_Base Target, bool Farm = false)
        {
            if (Items.CanUseItem(Bilgewater) && Player.Distance3D(Target) <= 450 && !Farm) Items.UseItem(Bilgewater, Target);
            if (Items.CanUseItem(BladeRuined) && Player.Distance3D(Target) <= 450 && !Farm) Items.UseItem(BladeRuined, Target);
            if (Items.CanUseItem(Tiamat) && Farm ? Player.Distance3D(Target) <= 350 : Player.CountEnemysInRange(350) >= 1) Items.UseItem(Tiamat);
            if (Items.CanUseItem(Hydra) && Farm ? Player.Distance3D(Target) <= 350 : (Player.CountEnemysInRange(350) >= 2 || (Player.GetAutoAttackDamage(Target, true) < Target.Health && Player.CountEnemysInRange(350) == 1))) Items.UseItem(Hydra);
            if (Items.CanUseItem(Randuin) && Player.CountEnemysInRange(450) >= 1 && !Farm) Items.UseItem(Randuin);
            if (Items.CanUseItem(Youmuu) && Player.CountEnemysInRange(350) >= 1 && !Farm) Items.UseItem(Youmuu);
        }
    }
}
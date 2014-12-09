using System;
using System.Linq;
using Color = System.Drawing.Color;

using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

using Orbwalk = MasterCommon.M_Orbwalker;

namespace MasterPlugin
{
    class LeeSin : Master.Program
    {
        private Obj_AI_Base allyObj = null;
        private bool WardCasted = false, JumpCasted = false, KickCasted = false, FlyCasted = false, FarmCasted = false, InsecJumpCasted = false, QCasted = false, WCasted = false, ECasted = false, RCasted = false;
        private enum HarassStage
        {
            Nothing,
            Doing,
            Finish,
        }
        private HarassStage CurHarassStage = HarassStage.Nothing;
        private Vector3 HarassBackPos = default(Vector3);
        private Spell SkillQ2, SkillE2;

        public LeeSin()
        {
            SkillQ = new Spell(SpellSlot.Q, 1000);
            SkillQ2 = new Spell(SpellSlot.Q, 1300);
            SkillW = new Spell(SpellSlot.W, 700);
            SkillE = new Spell(SpellSlot.E, 425);
            SkillE2 = new Spell(SpellSlot.Q, 575);
            SkillR = new Spell(SpellSlot.R, 375);
            SkillQ.SetSkillshot(-0.5f, 60, 1800, true, SkillshotType.SkillshotLine);
            SkillQ2.SetSkillshot(-0.5f, 0, 0, false, SkillshotType.SkillshotCircle);
            SkillW.SetTargetted(0, 1500);
            SkillE.SetSkillshot(-0.5f, 0, 0, false, SkillshotType.SkillshotCircle);
            SkillE2.SetSkillshot(-0.5f, 0, 2000, false, SkillshotType.SkillshotCircle);
            SkillR.SetTargetted(-0.5f, 1500);

            Config.SubMenu("OW").SubMenu("Mode").AddItem(new MenuItem(Name + "_OW_StarCombo", "明星连招").SetValue(new KeyBind("X".ToCharArray()[0], KeyBindType.Press)));
            Config.SubMenu("OW").SubMenu("Mode").AddItem(new MenuItem(Name + "_OW_InsecCombo", "明星回旋踢").SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press)));
            Config.SubMenu("OW").SubMenu("Mode").AddItem(new MenuItem(Name + "_OW_KSMob", "抢野怪").SetValue(new KeyBind("Z".ToCharArray()[0], KeyBindType.Press)));
            var ChampMenu = new Menu(Name + " 插件", Name + "_Plugin");
            {
                var ComboMenu = new Menu("连招", "Combo");
                {
                    ItemBool(ComboMenu, "Passive", "使用被动", false);
                    ItemBool(ComboMenu, "Q", "使用 Q");
                    ItemBool(ComboMenu, "W", "使用 W");
                    ItemBool(ComboMenu, "WSurvive", "-> 求生", false);
                    ItemSlider(ComboMenu, "WUnder", "--> 如果血量低于", 30);
                    ItemBool(ComboMenu, "WGap", "-> 接近目标");
                    ItemBool(ComboMenu, "WGapWard", "--> 如果没有盟友再跳眼", false);
                    ItemBool(ComboMenu, "E", "使用 E");
                    ItemBool(ComboMenu, "R", "如果能击杀使用R");
                    ItemBool(ComboMenu, "Item", "使用 物品");
                    ItemBool(ComboMenu, "Ignite", "如果能击杀自动点燃");
                    ChampMenu.AddSubMenu(ComboMenu);
                }
                var HarassMenu = new Menu("骚扰", "Harass");
                {
                    ItemBool(HarassMenu, "Q", "使用 Q");
                    ItemSlider(HarassMenu, "Q2Above", "-> Q2 如果血量低于", 20);
                    ItemBool(HarassMenu, "E", "使用 E");
                    ItemBool(HarassMenu, "WBackWard", "如果附近没有盟友,顺眼到后方", false);
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
                    ItemBool(ClearMenu, "Item", "使用九头蛇");
                    ChampMenu.AddSubMenu(ClearMenu);
                }
                var InsecMenu = new Menu("明星回旋踢设置", "Insec");
                {
                    var InsecNear = new Menu("盟友在附近配置", "InsecNear");
                    {
                        ItemBool(InsecNear, "ToChamp", "英雄");
                        ItemSlider(InsecNear, "ToChampHp", "-> 如果血量低于", 20);
                        ItemSlider(InsecNear, "ToChampR", "-> 如果在", 1100, 500, 1600);
                        ItemBool(InsecNear, "DrawToChamp", "-> 显示范围", false);
                        ItemBool(InsecNear, "ToTower", "塔");
                        ItemBool(InsecNear, "ToMinion", "小兵");
                        ItemSlider(InsecNear, "ToMinionR", "-> 如果在", 1100, 500, 1600);
                        ItemBool(InsecNear, "DrawToMinion", "-> 显示范围", false);
                        InsecMenu.AddSubMenu(InsecNear);
                    }
                    ItemList(InsecMenu, "Mode", "模式", new[] { "附近有盟友", "选定盟友", "鼠标的位置" });
                    ItemBool(InsecMenu, "Flash", "如果顺眼没有准备好才闪现", false);
                    ItemBool(InsecMenu, "DrawLine", "显示明星回旋踢线路");
                    ChampMenu.AddSubMenu(InsecMenu);
                }
                var UltiMenu = new Menu("大招", "Ultimate");
                {
                    foreach (var Obj in ObjectManager.Get<Obj_AI_Hero>().Where(i => i.IsEnemy)) ItemBool(UltiMenu, Obj.ChampionName, "使用R于 " + Obj.ChampionName);
                    ChampMenu.AddSubMenu(UltiMenu);
                }
                var MiscMenu = new Menu("额外选项", "Misc");
                {
                    ItemBool(MiscMenu, "WJPink", "使用真眼顺眼", false);
                    ItemBool(MiscMenu, "QLastHit", "使用Q补刀", false);
                    ItemBool(MiscMenu, "WSurvive", "尝试使用W求生");
                    ItemBool(MiscMenu, "SmiteCol", "自动惩戒碰撞");
                    ItemSlider(MiscMenu, "CustomSkin", "换肤", 5, 0, 6).ValueChanged += SkinChanger;
                    ChampMenu.AddSubMenu(MiscMenu);
                }
                var DrawMenu = new Menu("显示范围", "Draw");
                {
                    ItemBool(DrawMenu, "Killable", "击杀文本", false);
                    ItemBool(DrawMenu, "Q", "Q 范围", false);
                    ItemBool(DrawMenu, "W", "W 范围", false);
                    ItemBool(DrawMenu, "E", "E 范围", false);
                    ItemBool(DrawMenu, "R", "R 范围", false);
                    ChampMenu.AddSubMenu(DrawMenu);
                }
                Config.AddSubMenu(ChampMenu);
            }
            Game.OnGameUpdate += OnGameUpdate;
            Drawing.OnDraw += OnDraw;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            Game.OnWndProc += OnWndProc;
            Obj_AI_Minion.OnCreate += OnCreateObjMinion;
        }

        private void OnGameUpdate(EventArgs args)
        {
            if (Player.IsDead || MenuGUI.IsChatOpen || Player.IsChannelingImportantSpell() || Player.IsRecalling()) return;
            if (ItemList("Insec", "Mode") == 1)
            {
                if (SkillR.IsReady())
                {
                    allyObj = IsValid(allyObj, float.MaxValue, false) ? allyObj : null;
                }
                else if (allyObj != null) allyObj = null;
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
            if (Orbwalk.CurrentMode != Orbwalk.Mode.Harass) CurHarassStage = HarassStage.Nothing;
            Orbwalk.CustomMode = false;
            if (ItemActive("StarCombo")) StarCombo();
            if (ItemActive("InsecCombo"))
            {
                InsecCombo();
            }
            else InsecJumpCasted = false;
            if (ItemActive("KSMob")) KillStealMob();
        }

        private void OnDraw(EventArgs args)
        {
            if (Player.IsDead) return;
            if (ItemBool("Draw", "Q") && SkillQ.Level > 0) Utility.DrawCircle(Player.Position, SkillQ.Instance.Name == "BlindMonkQOne" ? SkillQ.Range : SkillQ2.Range, SkillQ.IsReady() ? Color.Green : Color.Red);
            if (ItemBool("Draw", "W") && SkillW.Level > 0) Utility.DrawCircle(Player.Position, SkillW.Range, SkillW.IsReady() ? Color.Green : Color.Red);
            if (ItemBool("Draw", "E") && SkillE.Level > 0) Utility.DrawCircle(Player.Position, SkillE.Instance.Name == "BlindMonkEOne" ? SkillE.Range : SkillE2.Range, SkillE.IsReady() ? Color.Green : Color.Red);
            if (ItemBool("Draw", "R") && SkillR.Level > 0) Utility.DrawCircle(Player.Position, SkillR.Range, SkillR.IsReady() ? Color.Green : Color.Red);
            if (ItemBool("Insec", "DrawLine") && SkillR.IsReady())
            {
                Byte validTargets = 0;
                if (targetObj != null)
                {
                    Utility.DrawCircle(targetObj.Position, 70, Color.FromArgb(0, 204, 0));
                    validTargets += 1;
                }
                if (GetInsecPos(true) != default(Vector3))
                {
                    Utility.DrawCircle(GetInsecPos(true), 70, Color.FromArgb(0, 204, 0));
                    validTargets += 1;
                }
                if (validTargets == 2) Drawing.DrawLine(Drawing.WorldToScreen(targetObj.Position), Drawing.WorldToScreen(targetObj.Position.To2D().Extend(GetInsecPos(true).To2D(), 600).To3D()), 1, Color.White);
            }
            if (ItemList("Insec", "Mode") == 0 && SkillR.IsReady())
            {
                if (ItemBool("InsecNear", "ToChamp") && ItemBool("InsecNear", "DrawToChamp")) Utility.DrawCircle(Player.Position, ItemSlider("InsecNear", "ToChampR"), Color.White);
                if (ItemBool("InsecNear", "ToMinion") && ItemBool("InsecNear", "DrawToMinion")) Utility.DrawCircle(Player.Position, ItemSlider("InsecNear", "ToMinionR"), Color.White);
            }
            if (ItemBool("Draw", "Killable"))
            {
                foreach (var Obj in ObjectManager.Get<Obj_AI_Hero>().Where(i => IsValid(i)))
                {
                    var dmgTotal = Player.GetAutoAttackDamage(Obj, true);
                    if (SkillQ.IsReady())
                    {
                        if (SkillQ.Instance.Name == "BlindMonkQOne")
                        {
                            dmgTotal += SkillQ.GetDamage(Obj);
                        }
                        else if (Obj.HasBuff("BlindMonkSonicWave")) dmgTotal += SkillQ.GetDamage(Obj, 1);
                    }
                    if (SkillE.IsReady() && SkillE.Instance.Name == "BlindMonkEOne") dmgTotal += SkillE.GetDamage(Obj);
                    if (SkillR.IsReady() && ItemBool("Ultimate", Obj.ChampionName)) dmgTotal += SkillR.GetDamage(Obj);
                    if (Obj.Health + 5 <= dmgTotal)
                    {
                        var posText = Drawing.WorldToScreen(Obj.Position);
                        Drawing.DrawText(posText.X - 30, posText.Y - 5, Color.White, "Killable");
                    }
                }
            }
        }

        private void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (Player.IsDead) return;
            if (sender.IsMe)
            {
                if (args.SData.Name == "BlindMonkQOne")
                {
                    QCasted = true;
                    Utility.DelayAction.Add(Orbwalk.CurrentMode == Orbwalk.Mode.LaneClear || Orbwalk.CurrentMode == Orbwalk.Mode.LaneFreeze ? 2800 : 2200, () => QCasted = false);
                }
                if (args.SData.Name == "BlindMonkWOne")
                {
                    WCasted = true;
                    Utility.DelayAction.Add(Orbwalk.CurrentMode == Orbwalk.Mode.LaneClear || Orbwalk.CurrentMode == Orbwalk.Mode.LaneFreeze ? 2800 : 1000, () => WCasted = false);
                    JumpCasted = true;
                    Utility.DelayAction.Add(1000, () => JumpCasted = false);
                }
                if (args.SData.Name == "BlindMonkEOne")
                {
                    ECasted = true;
                    Utility.DelayAction.Add(Orbwalk.CurrentMode == Orbwalk.Mode.LaneClear || Orbwalk.CurrentMode == Orbwalk.Mode.LaneFreeze ? 2800 : 2200, () => ECasted = false);
                }
                if (args.SData.Name == "BlindMonkRKick")
                {
                    RCasted = true;
                    Utility.DelayAction.Add(700, () => RCasted = false);
                    if (ItemActive("StarCombo") || ItemActive("InsecCombo"))
                    {
                        KickCasted = true;
                        Utility.DelayAction.Add(1000, () => KickCasted = false);
                    }
                }
                if (ItemActive("InsecCombo") && ItemBool("Insec", "Flash") && FlashReady())
                {
                    if (args.SData.Name == "blindmonkqtwo")
                    {
                        FlyCasted = true;
                        Utility.DelayAction.Add(1000, () => FlyCasted = false);
                    }
                    else if (args.SData.Name == "BlindMonkWOne" && !WardCasted) Utility.DelayAction.Add((int)((Player.Position.Distance(GetInsecPos()) - args.Target.Position.Distance(GetInsecPos())) / SkillW.Speed * 1000 + 200), () => CastFlash(GetInsecPos()));
                }
            }
            else if (sender.IsEnemy && ItemBool("Misc", "WSurvive") && SkillW.IsReady() && SkillW.Instance.Name == "BlindMonkWOne")
            {
                if (args.Target.IsMe && ((Orbwalk.IsAutoAttack(args.SData.Name) && Player.Health <= sender.GetAutoAttackDamage(Player, true)) || (args.SData.Name == "summonerdot" && Player.Health <= (sender as Obj_AI_Hero).GetSummonerSpellDamage(Player, Damage.SummonerSpell.Ignite))))
                {
                    SkillW.Cast(PacketCast());
                    return;
                }
                else if ((args.Target.IsMe || (Player.Position.Distance(args.Start) <= args.SData.CastRange[0] && Player.Position.Distance(args.End) <= Orbwalk.GetAutoAttackRange())) && Damage.Spells.ContainsKey((sender as Obj_AI_Hero).ChampionName))
                {
                    for (var i = 3; i > -1; i--)
                    {
                        if (Damage.Spells[(sender as Obj_AI_Hero).ChampionName].FirstOrDefault(a => a.Slot == (sender as Obj_AI_Hero).GetSpellSlot(args.SData.Name, false) && a.Stage == i) != null)
                        {
                            if (Player.Health <= (sender as Obj_AI_Hero).GetSpellDamage(Player, (sender as Obj_AI_Hero).GetSpellSlot(args.SData.Name, false), i))
                            {
                                SkillW.Cast(PacketCast());
                                return;
                            }
                        }
                    }
                }
            }
        }

        private void OnWndProc(WndEventArgs args)
        {
            if (args.WParam != 1 || MenuGUI.IsChatOpen || Player.Spellbook.SelectedSpellSlot != SpellSlot.Unknown || ItemList("Insec", "Mode") != 1 || !SkillR.IsReady()) return;
            allyObj = null;
            if (Player.IsDead) return;
            foreach (var Obj in ObjectManager.Get<Obj_AI_Base>().Where(i => IsValid(i, 230, false, Game.CursorPos)).OrderBy(i => i.Position.Distance(Game.CursorPos))) allyObj = Obj;
        }

        private void OnCreateObjMinion(GameObject sender, EventArgs args)
        {
            if (!sender.IsValid<Obj_AI_Minion>() || sender.IsEnemy || Player.IsDead || !SkillW.IsReady() || SkillW.Instance.Name != "BlindMonkWOne" || !WardCasted) return;
            if ((Orbwalk.CurrentMode == Orbwalk.Mode.Flee || (Orbwalk.CurrentMode == Orbwalk.Mode.Combo && ItemBool("Combo", "W") && ItemBool("Combo", "WGap") && ItemBool("Combo", "WGapWard")) || (Orbwalk.CurrentMode == Orbwalk.Mode.Harass && ItemBool("Harass", "WBackWard")) || ItemActive("StarCombo") || ItemActive("InsecCombo")) && Player.Distance3D((Obj_AI_Minion)sender) <= SkillW.Range + sender.BoundingRadius && sender.Name.EndsWith("Ward"))
            {
                SkillW.CastOnUnit((Obj_AI_Minion)sender, PacketCast());
                return;
            }
        }

        private void NormalCombo()
        {
            if (targetObj == null) return;
            if (ItemBool("Combo", "Passive") && Player.HasBuff("BlindMonkFlurry") && Orbwalk.InAutoAttackRange(targetObj) && Orbwalk.CanAttack()) return;
            if (ItemBool("Combo", "E") && SkillE.IsReady())
            {
                if (SkillE.Instance.Name == "BlindMonkEOne" && SkillE.InRange(targetObj.Position))
                {
                    SkillE.Cast(PacketCast());
                }
                else if (targetObj.HasBuff("BlindMonkTempest") && SkillE2.InRange(targetObj.Position) && (Player.Distance3D(targetObj) > 450 || !ECasted)) SkillE.Cast(PacketCast());
            }
            if (ItemBool("Combo", "Q") && SkillQ.IsReady())
            {
                if (SkillQ.Instance.Name == "BlindMonkQOne" && SkillQ.InRange(targetObj.Position))
                {
                    if (ItemBool("Misc", "SmiteCol"))
                    {
                        if (!SmiteCollision(targetObj, SkillQ)) SkillQ.CastIfHitchanceEquals(targetObj, HitChance.VeryHigh, PacketCast());
                    }
                    else SkillQ.CastIfHitchanceEquals(targetObj, HitChance.VeryHigh, PacketCast());
                }
                else if (targetObj.HasBuff("BlindMonkSonicWave") && SkillQ2.InRange(targetObj.Position))
                {
                    if (Player.Distance3D(targetObj) > 500 || CanKill(targetObj, SkillQ2, 1) || (targetObj.HasBuff("BlindMonkTempest") && SkillE.InRange(targetObj.Position) && !Orbwalk.InAutoAttackRange(targetObj)) || !QCasted) SkillQ.Cast(PacketCast());
                }
            }
            if (ItemBool("Combo", "R") && ItemBool("Ultimate", targetObj.ChampionName) && SkillR.IsReady() && SkillR.InRange(targetObj.Position))
            {
                if (CanKill(targetObj, SkillR) || (SkillR.GetHealthPrediction(targetObj) - SkillR.GetDamage(targetObj) + 5 <= GetQ2Dmg(targetObj, SkillR.GetDamage(targetObj)) && ItemBool("Combo", "Q") && SkillQ.IsReady() && targetObj.HasBuff("BlindMonkSonicWave"))) SkillR.CastOnUnit(targetObj, PacketCast());
            }
            if (ItemBool("Combo", "W") && SkillW.IsReady())
            {
                if (ItemBool("Combo", "WSurvive") || ItemBool("Combo", "WGap"))
                {
                    if (SkillW.Instance.Name == "BlindMonkWOne")
                    {
                        if (ItemBool("Combo", "WSurvive") && Orbwalk.InAutoAttackRange(targetObj) && Player.HealthPercentage() <= ItemList("Combo", "WUnder"))
                        {
                            SkillW.Cast(PacketCast());
                        }
                        else if (ItemBool("Combo", "WGap") && Player.Distance3D(targetObj) >= Orbwalk.GetAutoAttackRange() + 50)
                        {
                            var jumpObj = ObjectManager.Get<Obj_AI_Base>().Where(i => IsValid(i, SkillW.Range + i.BoundingRadius, false) && !(i is Obj_AI_Turret) && i.Distance3D(targetObj) <= Orbwalk.GetAutoAttackRange() + 50).OrderBy(i => i.Distance3D(targetObj));
                            if (jumpObj.Count() > 0 && !JumpCasted)
                            {
                                foreach (var Obj in jumpObj) SkillW.CastOnUnit(Obj, PacketCast());
                            }
                            else if (ItemBool("Combo", "WGapWard") && Player.Distance3D(targetObj) <= SkillW.Range + Orbwalk.GetAutoAttackRange() + 50 && (GetWardSlot() != null || GetWardSlot().Stacks > 0 || WardCasted)) WardJump(targetObj.Position);
                        }
                    }
                    else if (SkillE.InRange(targetObj.Position) && !Player.HasBuff("BlindMonkSafeguard") && !WCasted) SkillW.Cast(PacketCast());
                }
            }
            if (ItemBool("Combo", "Item")) UseItem(targetObj);
            if (ItemBool("Combo", "Ignite")) CastIgnite(targetObj);
        }

        private void Harass()
        {
            if (targetObj == null)
            {
                CurHarassStage = HarassStage.Nothing;
                return;
            }
            switch (CurHarassStage)
            {
                case HarassStage.Nothing:
                    CurHarassStage = HarassStage.Doing;
                    break;
                case HarassStage.Doing:
                    if (ItemBool("Harass", "Q") && SkillQ.IsReady())
                    {
                        if (SkillQ.Instance.Name == "BlindMonkQOne" && SkillQ.InRange(targetObj.Position))
                        {
                            if (ItemBool("Misc", "SmiteCol"))
                            {
                                if (!SmiteCollision(targetObj, SkillQ)) SkillQ.CastIfHitchanceEquals(targetObj, HitChance.VeryHigh, PacketCast());
                            }
                            else SkillQ.CastIfHitchanceEquals(targetObj, HitChance.VeryHigh, PacketCast());
                        }
                        else if (targetObj.HasBuff("BlindMonkSonicWave") && SkillQ2.InRange(targetObj.Position) && (CanKill(targetObj, SkillQ2, 1) || (SkillW.IsReady() && SkillW.Instance.Name == "BlindMonkWOne" && Player.Mana >= SkillW.Instance.ManaCost + (ItemBool("Harass", "E") && SkillE.IsReady() && SkillE.Instance.Name == "BlindMonkEOne" ? SkillQ.Instance.ManaCost + SkillE.Instance.ManaCost : SkillQ.Instance.ManaCost) && Player.HealthPercentage() >= ItemSlider("Harass", "Q2Above"))))
                        {
                            HarassBackPos = Player.ServerPosition;
                            SkillQ.Cast(PacketCast());
                            Utility.DelayAction.Add((int)((Player.Distance3D(targetObj) + (ItemBool("Harass", "E") && SkillE.IsReady() && SkillE.Instance.Name == "BlindMonkEOne" ? SkillE.Range : 0)) / SkillQ.Speed * 1000 - 100) * 2, () => CurHarassStage = HarassStage.Finish);
                        }
                    }
                    if (ItemBool("Harass", "E") && SkillE.IsReady())
                    {
                        if (SkillE.Instance.Name == "BlindMonkEOne" && SkillE.InRange(targetObj.Position))
                        {
                            SkillE.Cast(PacketCast());
                        }
                        else if (targetObj.HasBuff("BlindMonkTempest") && SkillE2.InRange(targetObj.Position)) CurHarassStage = HarassStage.Finish;
                    }
                    break;
                case HarassStage.Finish:
                    if (SkillW.IsReady() && SkillW.Instance.Name == "BlindMonkWOne")
                    {
                        var jumpObj = ObjectManager.Get<Obj_AI_Base>().Where(i => IsValid(i, SkillW.Range + i.BoundingRadius, false) && !(i is Obj_AI_Turret) && i.Distance3D(targetObj) >= 450).OrderByDescending(i => i.Distance3D(Player)).OrderBy(i => ObjectManager.Get<Obj_AI_Turret>().Where(a => IsValid(a, float.MaxValue, false)).OrderBy(a => a.Distance3D(Player)).FirstOrDefault().Distance3D(i));
                        if (jumpObj.Count() > 0 && !JumpCasted)
                        {
                            foreach (var Obj in jumpObj) SkillW.CastOnUnit(Obj, PacketCast());
                        }
                        else if (ItemBool("Harass", "WBackWard") && (GetWardSlot() != null || GetWardSlot().Stacks > 0 || WardCasted)) WardJump(HarassBackPos);
                    }
                    else
                    {
                        if (HarassBackPos != default(Vector3)) HarassBackPos = default(Vector3);
                        CurHarassStage = HarassStage.Nothing;
                    }
                    break;
            }
        }

        private void LaneJungClear()
        {
            var minionObj = ObjectManager.Get<Obj_AI_Minion>().Where(i => IsValid(i, SkillQ2.Range)).OrderBy(i => i.Health);
            foreach (var Obj in minionObj)
            {
                if (SmiteReady() && Obj.Team == GameObjectTeam.Neutral)
                {
                    if ((ItemBool("SmiteMob", "Baron") && Obj.Name.StartsWith("SRU_Baron")) || (ItemBool("SmiteMob", "Dragon") && Obj.Name.StartsWith("SRU_Dragon")) || (!Obj.Name.Contains("Mini") && (
                        (ItemBool("SmiteMob", "Red") && Obj.Name.StartsWith("SRU_Red")) || (ItemBool("SmiteMob", "Blue") && Obj.Name.StartsWith("SRU_Blue")) ||
                        (ItemBool("SmiteMob", "Krug") && Obj.Name.StartsWith("SRU_Krug")) || (ItemBool("SmiteMob", "Gromp") && Obj.Name.StartsWith("SRU_Gromp")) ||
                        (ItemBool("SmiteMob", "Raptor") && Obj.Name.StartsWith("SRU_Razorbeak")) || (ItemBool("SmiteMob", "Wolf") && Obj.Name.StartsWith("SRU_Murkwolf"))))) CastSmite(Obj);
                }
                var Passive = Player.HasBuff("BlindMonkFlurry");
                if (ItemBool("Clear", "E") && SkillE.IsReady() && !FarmCasted)
                {
                    if (SkillE.Instance.Name == "BlindMonkEOne" && (minionObj.Count(i => SkillE.InRange(i.Position)) >= 2 || (Obj.MaxHealth >= 1200 && SkillE.InRange(Obj.Position))))
                    {
                        if (!Passive)
                        {
                            SkillE.Cast(PacketCast());
                            FarmCasted = true;
                            Utility.DelayAction.Add(500, () => FarmCasted = false);
                        }
                    }
                    else if (Obj.HasBuff("BlindMonkTempest") && SkillE2.InRange(Obj.Position) && (!ECasted || !Passive))
                    {
                        SkillE.Cast(PacketCast());
                        FarmCasted = true;
                        Utility.DelayAction.Add(500, () => FarmCasted = false);
                    }
                }
                if (ItemBool("Clear", "W") && SkillW.IsReady() && !FarmCasted)
                {
                    if (SkillW.Instance.Name == "BlindMonkWOne")
                    {
                        if (!Passive && Orbwalk.InAutoAttackRange(Obj))
                        {
                            SkillW.Cast(PacketCast());
                            FarmCasted = true;
                            Utility.DelayAction.Add(500, () => FarmCasted = false);
                        }
                    }
                    else if (SkillE.InRange(Obj.Position) && (!WCasted || !Passive))
                    {
                        SkillW.Cast(PacketCast());
                        FarmCasted = true;
                        Utility.DelayAction.Add(500, () => FarmCasted = false);
                    }
                }
                if (ItemBool("Clear", "Q") && SkillQ.IsReady() && !FarmCasted)
                {
                    if (SkillQ.Instance.Name == "BlindMonkQOne" && SkillQ.InRange(Obj.Position))
                    {
                        if ((!Passive || !Orbwalk.InAutoAttackRange(Obj)) && !CanKill(Obj, SkillQ)) SkillQ.CastIfHitchanceEquals(Obj, HitChance.Medium, PacketCast());
                    }
                    else if (Obj.HasBuff("BlindMonkSonicWave") && (SkillQ2.GetHealthPrediction(Obj) + 5 <= GetQ2Dmg(Obj) || Player.Distance3D(Obj) > 500 || !QCasted || !Passive)) SkillQ.Cast(PacketCast());
                }
                if (ItemBool("Clear", "Item")) UseItem(Obj, true);
            }
        }

        private void LastHit()
        {
            if (!ItemBool("Misc", "QLastHit") || !SkillQ.IsReady() || SkillQ.Instance.Name != "BlindMonkQOne") return;
            foreach (var Obj in ObjectManager.Get<Obj_AI_Minion>().Where(i => IsValid(i, SkillQ.Range) && CanKill(i, SkillQ)).OrderBy(i => i.Health).OrderByDescending(i => i.Distance3D(Player))) SkillQ.CastIfHitchanceEquals(Obj, HitChance.VeryHigh, PacketCast());
        }

        private void WardJump(Vector3 Pos)
        {
            if (!SkillW.IsReady() || SkillW.Instance.Name != "BlindMonkWOne" || JumpCasted) return;
            bool IsWard = false;
            foreach (var Obj in ObjectManager.Get<Obj_AI_Base>().Where(i => IsValid(i, SkillW.Range + i.BoundingRadius, false) && !(i is Obj_AI_Turret) && i.Position.Distance(Pos) < 230 && (!ItemActive("InsecCombo") || (ItemActive("InsecCombo") && i.Name.EndsWith("Ward") && i.IsMinion))).OrderBy(i => i.Position.Distance(Pos)))
            {
                SkillW.CastOnUnit(Obj, PacketCast());
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

        private void StarCombo()
        {
            CustomOrbwalk(targetObj);
            if (targetObj == null) return;
            UseItem(targetObj);
            if (SkillE.IsReady())
            {
                if (SkillE.Instance.Name == "BlindMonkEOne" && SkillE.InRange(targetObj.Position))
                {
                    SkillE.Cast(PacketCast());
                }
                else if (targetObj.HasBuff("BlindMonkTempest") && SkillE2.InRange(targetObj.Position) && (Player.Distance3D(targetObj) > 450 || !ECasted)) SkillE.Cast(PacketCast());
            }
            if (SkillQ.IsReady())
            {
                if (SkillQ.Instance.Name == "BlindMonkQOne" && SkillQ.InRange(targetObj.Position))
                {
                    if (ItemBool("Misc", "SmiteCol"))
                    {
                        if (!SmiteCollision(targetObj, SkillQ)) SkillQ.CastIfHitchanceEquals(targetObj, HitChance.VeryHigh, PacketCast());
                    }
                    else SkillQ.CastIfHitchanceEquals(targetObj, HitChance.VeryHigh, PacketCast());
                }
                else if (targetObj.HasBuff("BlindMonkSonicWave") && SkillQ2.InRange(targetObj.Position) && (CanKill(targetObj, SkillQ2, 1) || (!SkillR.IsReady() && !RCasted && KickCasted) || (!SkillR.IsReady() && !RCasted && !KickCasted && !QCasted))) SkillQ.Cast(PacketCast());
            }
            if (SkillW.IsReady())
            {
                if (SkillW.Instance.Name == "BlindMonkWOne")
                {
                    if (SkillR.IsReady())
                    {
                        if (SkillQ.IsReady() && targetObj.HasBuff("BlindMonkSonicWave") && !SkillR.InRange(targetObj.Position) && Player.Distance3D(targetObj) < SkillW.Range + SkillR.Range - 170) WardJump(targetObj.Position);
                    }
                    else if (Orbwalk.InAutoAttackRange(targetObj)) SkillW.Cast(PacketCast());
                }
                else if (SkillE.InRange(targetObj.Position) && !Player.HasBuff("BlindMonkSafeguard") && !WCasted) SkillW.Cast(PacketCast());
            }
            if (SkillR.IsReady() && SkillQ.IsReady() && targetObj.HasBuff("BlindMonkSonicWave") && SkillR.InRange(targetObj.Position)) SkillR.CastOnUnit(targetObj, PacketCast());
        }

        private void InsecCombo()
        {
            CustomOrbwalk(targetObj);
            if (targetObj == null) return;
            if (GetInsecPos() != default(Vector3))
            {
                if (SkillR.InRange(targetObj.Position) && (GetInsecPos(true).Distance(targetObj.Position) - GetInsecPos(true).Distance(Player.Position.To2D().Extend(targetObj.Position.To2D(), targetObj.Distance3D(Player) + 500).To3D())) / 500 > 0.7)
                {
                    SkillR.CastOnUnit(targetObj, PacketCast());
                    return;
                }
                if (SkillW.IsReady() && SkillW.Instance.Name == "BlindMonkWOne" && Player.Position.Distance(GetInsecPos()) < 600 && (GetWardSlot() != null || GetWardSlot().Stacks > 0 || WardCasted))
                {
                    WardJump(GetInsecPos());
                    if (ItemBool("Insec", "Flash")) InsecJumpCasted = true;
                    return;
                }
                if (ItemBool("Insec", "Flash") && FlashReady() && !InsecJumpCasted && !WardCasted)
                {
                    var jumpObj = ObjectManager.Get<Obj_AI_Base>().Where(i => IsValid(i, SkillW.Range + i.BoundingRadius, false) && !(i is Obj_AI_Turret) && i.Position.Distance(GetInsecPos()) < 500).OrderBy(i => i.Position.Distance(GetInsecPos()));
                    if (jumpObj.Count() > 0 && !JumpCasted)
                    {
                        foreach (var Obj in jumpObj)
                        {
                            if (Player.Position.Distance(GetInsecPos()) < Player.Distance3D(Obj) + Obj.Position.Distance(GetInsecPos()) && !FlyCasted)
                            {
                                if (SkillW.IsReady() && SkillW.Instance.Name == "BlindMonkWOne")
                                {
                                    SkillW.CastOnUnit(Obj, PacketCast());
                                    return;
                                }
                            }
                        }
                    }
                    else if (Player.Position.Distance(GetInsecPos()) < 600 && !JumpCasted)
                    {
                        CastFlash(GetInsecPos());
                        return;
                    }
                }
            }
            if (SkillQ.IsReady())
            {
                if (SkillQ.Instance.Name == "BlindMonkQOne" && SkillQ.InRange(targetObj.Position))
                {
                    var nearObj = ObjectManager.Get<Obj_AI_Base>().Where(i => IsValid(i, SkillQ.Range) && !(i is Obj_AI_Turret) && i.Position.Distance(GetInsecPos()) < 500 && !CanKill(i, SkillQ)).OrderBy(i => i.Position.Distance(GetInsecPos()));
                    if (GetInsecPos() != default(Vector3) && nearObj.Count() > 0)
                    {
                        foreach (var Obj in nearObj) SkillQ.CastIfHitchanceEquals(Obj, HitChance.VeryHigh, PacketCast());
                    }
                    else
                    {
                        if (ItemBool("Misc", "SmiteCol"))
                        {
                            if (!SmiteCollision(targetObj, SkillQ)) SkillQ.CastIfHitchanceEquals(targetObj, HitChance.VeryHigh, PacketCast());
                        }
                        else SkillQ.CastIfHitchanceEquals(targetObj, HitChance.VeryHigh, PacketCast());
                    }
                }
                else if (targetObj.HasBuff("BlindMonkSonicWave") && SkillQ2.InRange(targetObj.Position) && (CanKill(targetObj, SkillQ2, 1) || (!SkillR.IsReady() && !RCasted && KickCasted) || Player.Position.Distance((GetInsecPos() != default(Vector3)) ? GetInsecPos() : targetObj.Position) > 600 || !QCasted))
                {
                    SkillQ.Cast(PacketCast());
                }
                else if (GetInsecPos() != default(Vector3) && ObjectManager.Get<Obj_AI_Base>().FirstOrDefault(i => i.HasBuff("BlindMonkSonicWave") && IsValid(i, SkillQ2.Range) && i.Position.Distance(GetInsecPos()) < 500) != null) SkillQ.Cast(PacketCast());
            }
        }

        private void KillStealMob()
        {
            var Obj = ObjectManager.Get<Obj_AI_Minion>().Where(i => IsValid(i, SkillQ2.Range) && i.Team == GameObjectTeam.Neutral).FirstOrDefault(i => new string[] { "SRU_Baron", "SRU_Dragon", "SRU_Blue", "SRU_Red" }.Any(a => i.Name.StartsWith(a) && !i.Name.StartsWith(a + "Mini")));
            CustomOrbwalk(Obj);
            if (Obj == null) return;
            if (SkillQ.IsReady() && SkillQ.GetHealthPrediction(Obj) - (SkillQ.Instance.Name == "BlindMonkQOne" ? SkillQ.GetDamage(Obj) : 0) - (SmiteReady() ? Player.GetSummonerSpellDamage(Obj, Damage.SummonerSpell.Smite) : 0) + 5 <= GetQ2Dmg(Obj, ((SkillQ.Instance.Name == "BlindMonkQOne") ? SkillQ.GetDamage(Obj) : 0) + (SmiteReady() ? Player.GetSummonerSpellDamage(Obj, Damage.SummonerSpell.Smite) : 0)))
            {
                if (SkillQ.Instance.Name == "BlindMonkQOne" && SkillQ.InRange(Obj.Position))
                {
                    SkillQ.CastIfHitchanceEquals(Obj, HitChance.Medium, PacketCast());
                }
                else if (Obj.HasBuff("BlindMonkSonicWave"))
                {
                    SkillQ.Cast(PacketCast());
                    if (SmiteReady()) Utility.DelayAction.Add((int)((Player.Distance3D(Obj) - 760) / SkillQ.Speed * 1000 + 300), () => CastSmite(Obj, false));
                }
            }
            var nearObj = ObjectManager.Get<Obj_AI_Base>().FirstOrDefault(i => IsValid(i, SkillQ2.Range) && !(i is Obj_AI_Turret) && i.HasBuff("BlindMonkSonicWave") && i.Distance3D(Obj) <= 760);
            if (SkillQ.IsReady() && SkillQ.Instance.Name != "BlindMonkQOne" && SmiteReady() && SkillQ.GetHealthPrediction(Obj) + 100 <= Player.GetSummonerSpellDamage(Obj, Damage.SummonerSpell.Smite) && nearObj != null)
            {
                SkillQ.Cast(PacketCast());
                Utility.DelayAction.Add((int)((Player.Distance3D(nearObj) - 760) / SkillQ.Speed * 1000 + 300), () => CastSmite(Obj));
            }
            if (SmiteReady()) CastSmite(Obj);
        }

        private Vector3 GetInsecPos(bool Drawing = false)
        {
            if (!SkillR.IsReady()) return default(Vector3);
            switch (ItemList("Insec", "Mode"))
            {
                case 0:
                    var ChampList = ObjectManager.Get<Obj_AI_Hero>().Where(i => IsValid(i, ItemSlider("InsecNear", "ToChampR"), false) && i.HealthPercentage() >= ItemSlider("InsecNear", "ToChampHp")).ToList();
                    var TowerObj = ObjectManager.Get<Obj_AI_Turret>().Where(i => IsValid(i, float.MaxValue, false)).OrderBy(i => i.Distance3D(Player)).FirstOrDefault();
                    var MinionObj = targetObj != null ? ObjectManager.Get<Obj_AI_Minion>().Where(i => IsValid(i, ItemSlider("InsecNear", "ToMinionR"), false) && Player.Distance3D(TowerObj) > 1500 && i.Distance3D(targetObj) > 600 && !i.Name.EndsWith("Ward")).OrderByDescending(i => i.Distance3D(targetObj)).OrderBy(i => i.Distance3D(TowerObj)).FirstOrDefault() : null;
                    if (ChampList.Count > 0 && ItemBool("InsecNear", "ToChamp"))
                    {
                        var Pos = default(Vector2);
                        foreach (var Obj in ChampList) Pos += Obj.Position.To2D();
                        Pos = new Vector2(Pos.X / ChampList.Count, Pos.Y / ChampList.Count);
                        return Drawing ? Pos.To3D() : Pos.Extend(targetObj.Position.To2D(), targetObj.Position.To2D().Distance(Pos) + 230).To3D();
                    }
                    if (MinionObj != null && ItemBool("InsecNear", "ToMinion")) return Drawing ? MinionObj.Position : MinionObj.Position.To2D().Extend(targetObj.Position.To2D(), targetObj.Distance3D(MinionObj) + 230).To3D();
                    if (TowerObj != null && ItemBool("InsecNear", "ToTower")) return Drawing ? TowerObj.Position : TowerObj.Position.To2D().Extend(targetObj.Position.To2D(), targetObj.Distance3D(TowerObj) + 230).To3D();
                    break;
                case 1:
                    if (allyObj != null) return Drawing ? allyObj.Position : allyObj.Position.To2D().Extend(targetObj.Position.To2D(), targetObj.Distance3D(allyObj) + 230).To3D();
                    break;
                case 2:
                    return Drawing ? Game.CursorPos : Game.CursorPos.To2D().Extend(targetObj.Position.To2D(), targetObj.Position.Distance(Game.CursorPos) + 230).To3D();
            }
            return default(Vector3);
        }

        private void UseItem(Obj_AI_Base Target, bool Farm = false)
        {
            if (Items.CanUseItem(Bilgewater) && Player.Distance3D(Target) <= 450 && !Farm) Items.UseItem(Bilgewater, Target);
            if (Items.CanUseItem(BladeRuined) && Player.Distance3D(Target) <= 450 && !Farm) Items.UseItem(BladeRuined, Target);
            if (Items.CanUseItem(Tiamat) && Farm ? Player.Distance3D(Target) <= 350 : Player.CountEnemysInRange(350) >= 1) Items.UseItem(Tiamat);
            if (Items.CanUseItem(Hydra) && Farm ? Player.Distance3D(Target) <= 350 : (Player.CountEnemysInRange(350) >= 2 || (Player.GetAutoAttackDamage(Target, true) < Target.Health && Player.CountEnemysInRange(350) == 1))) Items.UseItem(Hydra);
            if (Items.CanUseItem(Randuin) && Player.CountEnemysInRange(450) >= 1 && !Farm) Items.UseItem(Randuin);
        }

        private double GetQ2Dmg(Obj_AI_Base Target, double Plus = 0)
        {
            var Dmg = Player.CalcDamage(Target, Damage.DamageType.Physical, new double[] { 50, 80, 110, 140, 170 }[SkillQ.Level - 1] + 0.9 * Player.FlatPhysicalDamageMod + 0.08 * (Target.MaxHealth - Target.Health + Plus));
            return Target.IsMinion && Dmg > 400 ? 400 : Dmg;
        }
    }
}
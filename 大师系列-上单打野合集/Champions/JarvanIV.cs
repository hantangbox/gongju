using System;
using System.Linq;
using Color = System.Drawing.Color;

using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

using Orbwalk = MasterCommon.M_Orbwalker;

namespace Master
{
    class JarvanIV : Program
    {
        private Obj_AI_Base wallObj = null;
        private Vector3 flagPos = default(Vector3);

        public JarvanIV()
        {
            SkillQ = new Spell(SpellSlot.Q, 770);
            SkillW = new Spell(SpellSlot.W, 525);
            SkillE = new Spell(SpellSlot.E, 830);
            SkillR = new Spell(SpellSlot.R, 650);
            SkillQ.SetSkillshot(SkillQ.Instance.SData.SpellCastTime, SkillQ.Instance.SData.LineWidth, SkillQ.Instance.SData.MissileSpeed, false, SkillshotType.SkillshotLine);
            SkillE.SetSkillshot(SkillE.Instance.SData.SpellCastTime, SkillE.Instance.SData.LineWidth, SkillE.Instance.SData.MissileSpeed, false, SkillshotType.SkillshotCircle);
            SkillR.SetTargetted(SkillR.Instance.SData.SpellCastTime, SkillR.Instance.SData.MissileSpeed);

            Config.SubMenu("Orbwalker").SubMenu("xSLxOrbwalker_Modes").AddItem(new MenuItem("EQFlash", "连招EQ闪现").SetValue(new KeyBind("X".ToCharArray()[0], KeyBindType.Press)));

            Config.AddSubMenu(new Menu("连招", "csettings"));
            Config.SubMenu("csettings").AddItem(new MenuItem("qusage", "使用 Q").SetValue(true));
            Config.SubMenu("csettings").AddItem(new MenuItem("wusage", "使用 W").SetValue(true));
            Config.SubMenu("csettings").AddItem(new MenuItem("autowusage", "如果血量低于使用W").SetValue(new Slider(20, 1)));
            Config.SubMenu("csettings").AddItem(new MenuItem("eusage", "使用 E").SetValue(true));
            Config.SubMenu("csettings").AddItem(new MenuItem("rusage", "使用 R").SetValue(true));
            Config.SubMenu("csettings").AddItem(new MenuItem("ruseMode", "R 模式").SetValue(new StringList(new[] { "Finish", "# Enemy" })));
            Config.SubMenu("csettings").AddItem(new MenuItem("rmulti", "如果敌人超出使用R").SetValue(new Slider(2, 1, 4)));
            Config.SubMenu("csettings").AddItem(new MenuItem("ignite", "如果能击杀自动点燃").SetValue(true));
            Config.SubMenu("csettings").AddItem(new MenuItem("iusage", "使用物品").SetValue(true));

            Config.AddSubMenu(new Menu("骚扰", "hsettings"));
            Config.SubMenu("hsettings").AddItem(new MenuItem("useHarE", "使用 E").SetValue(true));
            Config.SubMenu("hsettings").AddItem(new MenuItem("harMode", "如果血量超过使用EQ").SetValue(new Slider(20, 1)));

            Config.AddSubMenu(new Menu("清线/清野", "LaneJungClear"));
            Config.SubMenu("LaneJungClear").AddItem(new MenuItem("useClearQ", "使用 Q").SetValue(true));
            Config.SubMenu("LaneJungClear").AddItem(new MenuItem("useClearE", "使用 E").SetValue(true));
            Config.SubMenu("LaneJungClear").AddItem(new MenuItem("useClearI", "使用九头蛇物品").SetValue(true));

            Config.AddSubMenu(new Menu("额外选项", "miscs"));
            Config.SubMenu("miscs").AddItem(new MenuItem("lasthitQ", "使用Q补刀").SetValue(true));
            Config.SubMenu("miscs").AddItem(new MenuItem("killstealQ", "自动Q抢人头").SetValue(true));
            Config.SubMenu("miscs").AddItem(new MenuItem("useInterEQ", "使用EQ打断").SetValue(true));
            Config.SubMenu("miscs").AddItem(new MenuItem("surviveW", "尝试使用W求生").SetValue(true));
            Config.SubMenu("miscs").AddItem(new MenuItem("CustomSkin", "换肤").SetValue(new Slider(5, 0, 6))).ValueChanged += SkinChanger;

            Config.AddSubMenu(new Menu("显示范围", "DrawSettings"));
            Config.SubMenu("DrawSettings").AddItem(new MenuItem("DrawQ", "Q 范围").SetValue(true));
            Config.SubMenu("DrawSettings").AddItem(new MenuItem("DrawW", "W 范围").SetValue(true));
            Config.SubMenu("DrawSettings").AddItem(new MenuItem("DrawE", "E 范围").SetValue(true));
            Config.SubMenu("DrawSettings").AddItem(new MenuItem("DrawR", "R 范围").SetValue(true));

            Game.OnGameUpdate += OnGameUpdate;
            Drawing.OnDraw += OnDraw;
            Interrupter.OnPossibleToInterrupt += OnPossibleToInterrupt;
            Obj_AI_Base.OnCreate += OnCreate;
            Obj_AI_Base.OnDelete += OnDelete;
        }

        private void OnGameUpdate(EventArgs args)
        {
            if (Player.IsDead || MenuGUI.IsChatOpen) return;
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
                    if (Config.Item("lasthitQ").GetValue<bool>()) LastHit();
                    break;
                case Orbwalk.Mode.Flee:
                    Flee();
                    break;
            }
            if (Config.Item("EQFlash").GetValue<KeyBind>().Active)
            {
                ComboEQFlash();
            }
            else Orbwalk.CustomMode = false;
            if (Config.Item("killstealQ").GetValue<bool>()) KillSteal();
        }

        private void OnDraw(EventArgs args)
        {
            if (Player.IsDead) return;
            if (Config.Item("DrawQ").GetValue<bool>() && SkillQ.Level > 0) Utility.DrawCircle(Player.Position, SkillQ.Range, SkillQ.IsReady() ? Color.Green : Color.Red);
            if (Config.Item("DrawW").GetValue<bool>() && SkillW.Level > 0) Utility.DrawCircle(Player.Position, SkillW.Range, SkillW.IsReady() ? Color.Green : Color.Red);
            if (Config.Item("DrawE").GetValue<bool>() && SkillE.Level > 0) Utility.DrawCircle(Player.Position, SkillE.Range, SkillE.IsReady() ? Color.Green : Color.Red);
            if (Config.Item("DrawR").GetValue<bool>() && SkillR.Level > 0) Utility.DrawCircle(Player.Position, SkillR.Range, SkillR.IsReady() ? Color.Green : Color.Red);
        }

        private void OnPossibleToInterrupt(Obj_AI_Base unit, InterruptableSpell spell)
        {
            if (!Config.Item("useInterEQ").GetValue<bool>() || !SkillQ.IsReady()) return;
            if (unit.IsValidTarget(SkillQ.Range) && SkillE.IsReady()) SkillE.Cast(unit.Position + Vector3.Normalize(unit.Position - Player.Position) * 100, PacketCast());
            if (flagPos != default(Vector3) && unit.IsValidTarget(180, true, flagPos)) SkillQ.Cast(flagPos, PacketCast());
        }

        private void OnCreate(GameObject sender, EventArgs args)
        {
            if (sender.Name == "JarvanDemacianStandard_buf_green.troy") flagPos = sender.Position;
            if (sender.Name == "JarvanCataclysm_tar.troy") wallObj = (Obj_AI_Base)sender;
            if (sender is Obj_SpellMissile && sender.IsValid && Config.Item("surviveW").GetValue<bool>() && SkillW.IsReady())
            {
                var missle = (Obj_SpellMissile)sender;
                var caster = missle.SpellCaster;
                if (caster.IsEnemy)
                {
                    var ShieldBuff = new Int32[] { 50, 90, 130, 170, 210 }[SkillW.Level - 1] + new Int32[] { 20, 30, 40, 50, 60 }[SkillW.Level - 1] * Player.CountEnemysInRange(300);
                    if (missle.SData.Name.Contains("BasicAttack"))
                    {
                        if (missle.Target.IsMe && Player.Health <= caster.GetAutoAttackDamage(Player, true) && Player.Health + ShieldBuff > caster.GetAutoAttackDamage(Player, true)) SkillW.Cast(PacketCast());
                    }
                    else if (missle.Target.IsMe || missle.EndPosition.Distance(Player.Position) <= 130)
                    {
                        if (missle.SData.Name == "summonerdot")
                        {
                            if (Player.Health <= (caster as Obj_AI_Hero).GetSummonerSpellDamage(Player, Damage.SummonerSpell.Ignite) && Player.Health + ShieldBuff > (caster as Obj_AI_Hero).GetSummonerSpellDamage(Player, Damage.SummonerSpell.Ignite)) SkillW.Cast(PacketCast());
                        }
                        else if (Player.Health <= (caster as Obj_AI_Hero).GetSpellDamage(Player, (caster as Obj_AI_Hero).GetSpellSlot(missle.SData.Name, false), 1) && Player.Health + ShieldBuff > (caster as Obj_AI_Hero).GetSpellDamage(Player, (caster as Obj_AI_Hero).GetSpellSlot(missle.SData.Name, false), 1)) SkillW.Cast(PacketCast());
                    }
                }
            }
        }

        private void OnDelete(GameObject sender, EventArgs args)
        {
            if (sender.Name == "JarvanDemacianStandard_buf_green.troy") flagPos = default(Vector3);
            if (sender.Name == "JarvanCataclysm_tar.troy") wallObj = null;
        }

        private void NormalCombo()
        {
            if (targetObj == null) return;
            if (Config.Item("eusage").GetValue<bool>() && SkillE.IsReady() && SkillQ.InRange(targetObj.Position)) SkillE.Cast(Player.Distance(targetObj) < 450 ? targetObj.Position : targetObj.Position + Vector3.Normalize(targetObj.Position - Player.Position) * 100, PacketCast());
            if (Config.Item("eusage").GetValue<bool>() && flagPos != default(Vector3) && targetObj.IsValidTarget(180, true, flagPos))
            {
                if (Config.Item("qusage").GetValue<bool>() && SkillQ.InRange(targetObj.Position) && SkillQ.IsReady()) SkillQ.Cast(flagPos, PacketCast());
            }
            else if (Config.Item("qusage").GetValue<bool>() && SkillQ.InRange(targetObj.Position) && SkillQ.IsReady()) SkillQ.Cast(targetObj.Position, PacketCast());
            if (Config.Item("rusage").GetValue<bool>() && SkillR.IsReady() && wallObj == null)
            {
                switch (Config.Item("ruseMode").GetValue<StringList>().SelectedIndex)
                {
                    case 0:
                        if (SkillR.InRange(targetObj.Position) && CanKill(targetObj, SkillR)) SkillR.CastOnUnit(targetObj, PacketCast());
                        break;
                    case 1:
                        var UltiObj = ObjectManager.Get<Obj_AI_Hero>().FirstOrDefault(i => i.IsValidTarget(SkillR.Range) && i.CountEnemysInRange(325) >= Config.Item("rmulti").GetValue<Slider>().Value);
                        if (UltiObj != null) SkillR.CastOnUnit(UltiObj, PacketCast());
                        break;
                }
            }
            if (Config.Item("wusage").GetValue<bool>() && SkillW.IsReady() && SkillW.InRange(targetObj.Position) && Player.Health * 100 / Player.MaxHealth <= Config.Item("autowusage").GetValue<Slider>().Value) SkillW.Cast(PacketCast());
            if (Config.Item("iusage").GetValue<bool>()) UseItem(targetObj);
            if (Config.Item("ignite").GetValue<bool>()) CastIgnite(targetObj);
        }

        private void Harass()
        {
            if (targetObj == null) return;
            if (Config.Item("useHarE").GetValue<bool>() && SkillE.IsReady() && SkillQ.InRange(targetObj.Position)) SkillE.Cast(targetObj.Position + Vector3.Normalize(targetObj.Position - Player.Position) * 100, PacketCast());
            if (Config.Item("useHarE").GetValue<bool>() && flagPos != default(Vector3) && targetObj.IsValidTarget(180, true, flagPos) && SkillQ.InRange(targetObj.Position) && SkillQ.IsReady())
            {
                if (Player.Health * 100 / Player.MaxHealth >= Config.Item("harMode").GetValue<Slider>().Value) SkillQ.Cast(flagPos, PacketCast());
            }
            else if (SkillQ.InRange(targetObj.Position) && SkillQ.IsReady()) SkillQ.Cast(targetObj.Position, PacketCast());
        }

        private void LaneJungClear()
        {
            var minionObj = MinionManager.GetMinions(Player.Position, SkillE.Range - 50, MinionTypes.All, MinionTeam.NotAlly);
            if (minionObj.Count == 0) return;
            var posQFarm = SkillQ.GetLineFarmLocation(minionObj);
            var posEFarm = SkillE.GetCircularFarmLocation(minionObj);
            if (Config.Item("useClearE").GetValue<bool>() && SkillE.IsReady()) SkillE.Cast(posEFarm.MinionsHit >= 2 ? posEFarm.Position : minionObj.First().Position.To2D(), PacketCast());
            if (Config.Item("useClearE").GetValue<bool>() && flagPos != default(Vector3) && minionObj.Count(i => i.IsValidTarget(180, true, flagPos)) >= 2)
            {
                if (Config.Item("useClearQ").GetValue<bool>() && SkillQ.IsReady() && SkillQ.InRange(flagPos)) SkillQ.Cast(flagPos, PacketCast());
            }
            else if (Config.Item("useClearQ").GetValue<bool>() && SkillQ.IsReady()) SkillQ.Cast(posQFarm.MinionsHit >= 2 ? posQFarm.Position : minionObj.First().Position.To2D(), PacketCast());
            if (Config.Item("useClearI").GetValue<bool>() && minionObj.Count(i => i.IsValidTarget(350)) >= 2)
            {
                if (Items.CanUseItem(Tiamat)) Items.UseItem(Tiamat);
                if (Items.CanUseItem(Hydra)) Items.UseItem(Hydra);
            }
        }

        private void LastHit()
        {
            var minionObj = MinionManager.GetMinions(Player.Position, SkillQ.Range, MinionTypes.All, MinionTeam.NotAlly).Where(i => CanKill(i, SkillQ)).OrderByDescending(i => i.Distance(Player)).FirstOrDefault();
            if (minionObj != null && SkillQ.IsReady()) SkillQ.Cast(minionObj.Position, PacketCast());
        }

        private void Flee()
        {
            if (!SkillQ.IsReady()) return;
            if (SkillE.IsReady()) SkillE.Cast(Game.CursorPos, PacketCast());
            if (flagPos != default(Vector3)) SkillQ.Cast(flagPos, PacketCast());
        }

        private void ComboEQFlash()
        {
            CustomOrbwalk(targetObj);
            if (targetObj == null || !FlashReady() || Player.Mana < SkillQ.Instance.ManaCost || Player.Distance(targetObj) > SkillQ.Range + 600) return;
            if (SkillE.IsReady()) SkillE.Cast(Game.CursorPos, PacketCast());
            if (SkillQ.IsReady() && flagPos != default(Vector3) && SkillQ.InRange(flagPos)) SkillQ.Cast(flagPos, PacketCast());
            if (flagPos != default(Vector3) && !SkillQ.IsReady() && Player.Distance(targetObj) < 600) CastFlash(targetObj.Position);
        }

        private void KillSteal()
        {
            var target = ObjectManager.Get<Obj_AI_Hero>().FirstOrDefault(i => i.IsValidTarget(SkillQ.Range) && CanKill(i, SkillQ) && i != targetObj);
            if (target != null && SkillQ.IsReady()) SkillQ.Cast(target.Position, PacketCast());
        }

        private void UseItem(Obj_AI_Hero target)
        {
            if (Items.CanUseItem(Tiamat) && Player.CountEnemysInRange(350) >= 1) Items.UseItem(Tiamat);
            if (Items.CanUseItem(Hydra) && (Player.CountEnemysInRange(350) >= 2 || (Player.GetAutoAttackDamage(target, true) < target.Health && Player.CountEnemysInRange(350) == 1))) Items.UseItem(Hydra);
            if (Items.CanUseItem(Randuin) && Player.CountEnemysInRange(450) >= 1) Items.UseItem(Randuin);
        }
    }
}
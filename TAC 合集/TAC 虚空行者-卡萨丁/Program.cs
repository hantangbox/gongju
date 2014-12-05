using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using LeagueSharp;
using LeagueSharp.Common;

namespace TAC_Kassadin
{
    class Program
    {
        internal static Orbwalking.Orbwalker orb;
        internal static bool packetCast;
        internal static bool drawings;
        internal static bool UseShield;

        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += onLoad;
            Game.PrintChat("<font color='#FF0000'>[v1.3]</font> <font color='#7A6EFF'>Twilight's Auto Carry: </font> <font color='#86E5E1'>|鍗℃柉涓亅 鍔犺級鎴愬姛!婕㈠寲by浜岀嫍!QQ缇361630847</font>");
            Timer t = new Timer(TimerCallback, null, 0, 300000);
        }
        private static void onLoad(EventArgs e)
        {
            SkillHandler.Q.SetTargetted(0.5f, 1400f);
            SkillHandler.E.SetSkillshot(0.5f, 10f, float.MaxValue, false, SkillshotType.SkillshotCone);
            SkillHandler.R.SetSkillshot(0.5f, 150f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            MenuHandler.load();

            Game.OnGameUpdate += onGameUpdate;
            AntiGapcloser.OnEnemyGapcloser += AutoCarryHandler.AntiGapCloser;
            Interrupter.OnPossibleToInterrupt += AutoCarryHandler.CancelChanneledSpells;
            Obj_AI_Hero.OnProcessSpellCast += AutoCarryHandler.onProcessSpellCast;
            Drawing.OnDraw += DrawingHandler.load;
        }

        private static void onGameUpdate(EventArgs e)
        {
            drawings = MenuHandler.menu.Item("drawings").GetValue<bool>();
            packetCast = MenuHandler.menu.Item("packetCast").GetValue<bool>();
            if (UseShield && MenuHandler.menu.Item("useShield").GetValue<bool>())
            {
                float myHP = ObjectManager.Player.Health / ObjectManager.Player.MaxHealth * 100;
                float ConfigHP = MenuHandler.menu.Item("useShieldHP").GetValue<Slider>().Value;
                if (myHP <= ConfigHP && Items.HasItem(3040) && Items.CanUseItem(3040))
                {
                    Items.UseItem(3040);
                    UseShield = false;
                }
            }
            switch(orb.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    AutoCarryHandler.AutoCarry();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    AutoCarryHandler.Mixed();
                    break;
            }
            if (MenuHandler.menu.Item("ksActive").GetValue<bool>()) AutoCarryHandler.KillSecure();
        }
        private static void TimerCallback(Object o)
        {
            Game.PrintChat("["+Utils.FormatTime(Game.ClockTime)+"] <font color='#00ff00'>System (Kappa):</font> Thank you for choosing TAC assemblies! Don't forget to upvote them in forum!");
        }
    }
}

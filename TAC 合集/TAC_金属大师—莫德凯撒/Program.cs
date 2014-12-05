using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace TAC_Mordekaiser
{
    class Program
    {
        internal static bool packetCast;
        internal static Orbwalking.Orbwalker orb;
        internal static bool drawings;
        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += onLoad;
            Game.PrintChat("閲戝睘澶у笀 鍔犺級鎴愬姛!婕㈠寲by浜岀嫍!QQ缇361630847!");
        }
        private static void onLoad(EventArgs args)
        {
            MenuHandler.loadMe();
            Game.OnGameUpdate += onUpdate;
            AntiGapcloser.OnEnemyGapcloser += AutoCarryHandler.AntiGapCloser;
            Obj_AI_Hero.OnProcessSpellCast += AutoCarryHandler.onProcessSpellCast;
            Drawing.OnDraw += DrawingHandler.load;
            Drawing.OnEndScene += DrawingHandler.OnEndScene;
        }
        private static void onUpdate(EventArgs args)
        {
            drawings = MenuHandler.Config.Item("drawings").GetValue<bool>();
            packetCast = MenuHandler.Config.Item("packetCast").GetValue<bool>();
            switch (orb.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    AutoCarryHandler.onCombo();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    AutoCarryHandler.Mixed();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    AutoCarryHandler.LaneClear();
                    break;
            }
        }
    }
}

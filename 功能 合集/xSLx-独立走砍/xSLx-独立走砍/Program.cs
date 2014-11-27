using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using xSLx_Orbwalker;
using xSLx_TargetSelector;

namespace xSLx_Orbwalker_Standalone
{
    class Program
    {
        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        public static void Game_OnGameLoad(EventArgs args)
        {
            Game.PrintChat("<font color='#FF0000'>xSLx 鐙珛璧扮爫</font> loaded. - <font color='#5882FA'>E2Slayer</font>");

            //xSLxOrbwalker Load part
            var menu = new Menu("xSLx 璧扮爫", "my_mainmenu", true);
            var orbwalkerMenu = new Menu("xSLx 璧扮爫", "my_Orbwalker");
            xSLxOrbwalker.AddToMenu(orbwalkerMenu);
            menu.AddSubMenu(orbwalkerMenu);
            menu.AddToMainMenu();


            //xSLxActivator Load part
            var targetselectormenu = new Menu("鐩爣閫夋嫨", "Common_TargetSelector");
            SimpleTs.AddToMenu(targetselectormenu);
            menu.AddSubMenu(targetselectormenu);
        }
    }
}

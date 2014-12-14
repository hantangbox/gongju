#region

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SharpDX.Direct3D9;
using Font = SharpDX.Direct3D9.Font;

#endregion

namespace Tracker
{
    
    internal class Program
    {
        public static Menu Config;

        static void Main(string[] args)
        {
            Config = new Menu("Tracker#監視器", "Tracker", true);
            HbTracker.AttachToMenu(Config);
            WardTracker.AttachToMenu(Config);
            GankAlerter.AttachToMenu(Config);
            Config.AddToMainMenu();
        }
		private static void Print(string msg)
        {
            Game.PrintChat(
                "<font color='#ff3232'>Universal</font><font color='#BABABA'>鍔犺浇鎴愬姛!姹夊寲by浜岀嫍!QQ缇361630847</font> <font color='#FFFFFF'>");
        }
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LeagueSharp;
using LeagueSharp.Common;
using System.Drawing;
using SharpDX;
using LX_Orbwalker;
using Color = System.Drawing.Color;

namespace PerfectWard
{
    internal class Program
    {
        private static Obj_AI_Hero Player;
        private static string WelcomeMsg = ("<font color = '#cc33cc'>瀹岀編鐪间綅</font><font color='#FFFFFF'> by Da'ath.</font> <font color = '#66ff33'> ~~ 鍔犺級鎴愬姛锛佹饥鍖朾y浜岀嫍锛丵Q缇361630847 ~~</font>");
        private static Menu Menu;
        private static List<Vector3> standingCoords;
        private static List<Vector3> placeCoords;

        private static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
            Drawing.OnDraw += Drawing_OnDraw;
            Game.PrintChat(WelcomeMsg);
        }

        private static void Game_OnGameLoad(EventArgs args)
        {
            Player = ObjectManager.Player;
            if (Player.IsDead)
                return;

            Menu = new Menu("完美眼位", "menu", true);
            Menu.AddItem(new MenuItem("on", "启用").SetValue(true));
            Menu.AddItem(new MenuItem("on1", "显示英雄附近眼位").SetValue(false));
            Menu.AddToMainMenu();

            standingCoords = new List<Vector3>();
            placeCoords = new List<Vector3>();
            GetStandingCoords();
            GetPlaceCoords();


        }

        static void Drawing_OnDraw(EventArgs args)
        {
            if (!Menu.Item("on").GetValue<bool>())
                return;

            if (Menu.Item("on1").GetValue<bool>())
            {
                foreach (var StandingCoords in standingCoords.Where(StandingCoords => Vector3.Distance(Player.ServerPosition, StandingCoords) <= 1500))
                    Utility.DrawCircle(StandingCoords, 50f, Color.Red);

                foreach (var PlaceCoords in placeCoords.Where(PlaceCoords => Vector3.Distance(Player.ServerPosition, PlaceCoords) <= 1500))
                    Utility.DrawCircle(PlaceCoords, 15f, Color.Blue);
            }
            else
            {
                foreach (var StandingCoords in standingCoords)
                    Utility.DrawCircle(StandingCoords, 50f, Color.Red);

                foreach (var PlaceCoords in placeCoords)
                    Utility.DrawCircle(PlaceCoords, 15f, Color.Blue);
            }
        }

        private static void GetStandingCoords()
        {
            Vector3 stand1 = new Vector3(2524, 10406, 54f);
            standingCoords.Add(stand1);
            Vector3 stand2 = new Vector3(1774, 10756, 52f);
            standingCoords.Add(stand2);
            Vector3 stand3 = new Vector3(5520, 6342, 51f);
            standingCoords.Add(stand3);
            Vector3 stand4 = new Vector3(5674, 7358, 51f);
            standingCoords.Add(stand4);
            Vector3 stand5 = new Vector3(7990, 4282, 53f);
            standingCoords.Add(stand5);
            Vector3 stand6 = new Vector3(8256, 2920, 51f);
            standingCoords.Add(stand6);
            Vector3 stand7 = new Vector3(4818, 10866, -71f);
            standingCoords.Add(stand7);
            Vector3 stand8 = new Vector3(6824, 10656, 55f);
            standingCoords.Add(stand8);
            Vector3 stand9 = new Vector3(6574, 12006, 56f);
            standingCoords.Add(stand9);
            Vector3 stand10 = new Vector3(9130, 8346, 53f);
            standingCoords.Add(stand10);
            Vector3 stand11 = new Vector3(9422, 7408, 52f);
            standingCoords.Add(stand11);
            Vector3 stand12 = new Vector3(12372, 4508, 51f);
            standingCoords.Add(stand12);
            Vector3 stand13 = new Vector3(13003, 3818, 51f);
            standingCoords.Add(stand13);


        }

        private static void GetPlaceCoords()
        {
            Vector3 place1 = new Vector3(2729, 10879, -71f);
            placeCoords.Add(place1);
            Vector3 place2 = new Vector3(2303, 10868, 53f);
            placeCoords.Add(place2);
            Vector3 place3 = new Vector3(5223, 6789, 50f);
            placeCoords.Add(place3);
            Vector3 place4 = new Vector3(5191, 7137, 50f);
            placeCoords.Add(place4);
            Vector3 place5 = new Vector3(8368, 4594, 51f);
            placeCoords.Add(place5);
            Vector3 place6 = new Vector3(8100, 3429, 51f);
            placeCoords.Add(place6);
            Vector3 place7 = new Vector3(4634, 11283, 49f);
            placeCoords.Add(place7);
            Vector3 place8 = new Vector3(6672, 11466, 53f);
            placeCoords.Add(place8);
            Vector3 place9 = new Vector3(6518, 10367, 53f);
            placeCoords.Add(place9);
            Vector3 place10 = new Vector3(9572, 8038, 57f);
            placeCoords.Add(place10);
            Vector3 place11 = new Vector3(9697, 7854, 51f);
            placeCoords.Add(place11);
            Vector3 place12 = new Vector3(12235, 4068, -68f);
            placeCoords.Add(place12);
            Vector3 place13 = new Vector3(12443, 4021, -7f);
            placeCoords.Add(place13);

        }
    }
}

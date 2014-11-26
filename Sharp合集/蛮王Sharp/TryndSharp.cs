using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LeagueSharp;
using LeagueSharp.Common;
using System.Drawing;

/*
 * ToDo:
 * 
 * */


namespace TryndSharp
{
    internal class TryndSharp
    {

        public const string CharName = "Tryndamere";

        public static Menu Config;

        public static Obj_AI_Hero target;

        public TryndSharp()
        {
            /* CallBAcks */
            CustomEvents.Game.OnGameLoad += onLoad;

        }

        private static void onLoad(EventArgs args)
        {

            Game.PrintChat("Tryndamere - Sharp by DeTuKs");

            try
            {

                Config = new Menu("铔帇 Sharp", "Tryndamere", true);
                //Orbwalker
                Config.AddSubMenu(new Menu("璧扮爫", "Orbwalker"));
                Trynd.orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalker"));
                //TS
                var TargetSelectorMenu = new Menu("鐩爣閫夋嫨", "Target Selector");
                SimpleTs.AddToMenu(TargetSelectorMenu);
                Config.AddSubMenu(TargetSelectorMenu);
                //Combo
                Config.AddSubMenu(new Menu("杩炴嫑 Sharp", "combo"));
                Config.SubMenu("combo").AddItem(new MenuItem("comboItems", "浣跨敤 鐗╁搧")).SetValue(true);
                Config.SubMenu("combo").AddItem(new MenuItem("useW", "浣跨敤 W")).SetValue(true);
                Config.SubMenu("combo").AddItem(new MenuItem("useE", "浣跨敤 E")).SetValue(true);
                Config.SubMenu("combo").AddItem(new MenuItem("QonHp", "浣跨敤Q|鏈€浣庤閲弢")).SetValue(new Slider(25, 100, 0));
               // Config.SubMenu("combo").AddItem(new MenuItem("useR", "Use R on %")).SetValue(new Slider(25, 100, 0));

                //LastHit
                Config.AddSubMenu(new Menu("琛ュ垁 Sharp", "lHit"));
               
                //LaneClear
                Config.AddSubMenu(new Menu("娓呯嚎 Sharp", "lClear"));
               
               
                //Extra
                Config.AddSubMenu(new Menu("棰濆 Sharp", "extra"));

                //Debug
                Config.AddSubMenu(new Menu("璋冭瘯 Sharp", "debug"));
                Config.SubMenu("debug").AddItem(new MenuItem("db_targ", "璋冭瘯 鐩爣")).SetValue(new KeyBind('T', KeyBindType.Press, false));


                Config.AddToMainMenu();
                Drawing.OnDraw += onDraw;
                Game.OnGameUpdate += OnGameUpdate;

                GameObject.OnCreate += OnCreateObject;
                GameObject.OnDelete += OnDeleteObject;
                Obj_AI_Base.OnProcessSpellCast += OnProcessSpell;
            }
            catch
            {
                Game.PrintChat("Oops. Something went wrong with Yasuo- Sharpino");
            }

        }

        private static void OnGameUpdate(EventArgs args)
        {

            if (Trynd.orbwalker.ActiveMode.ToString() == "Combo")
            {
              //  Console.WriteLine("emm");
                if(Trynd.E.IsReady())
                    target = SimpleTs.GetTarget(950, SimpleTs.DamageType.Physical);
                else if (Trynd.W.IsReady())
                    target = SimpleTs.GetTarget(450, SimpleTs.DamageType.Physical);
                else
                    target = SimpleTs.GetTarget(250, SimpleTs.DamageType.Physical);

                Trynd.doCombo(target);
            }

            if (Trynd.orbwalker.ActiveMode.ToString() == "Mixed")
            {
               
            }

            if (Trynd.orbwalker.ActiveMode.ToString() == "LaneClear")
            {
                
            }


            if (Config.Item("harassOn").GetValue<bool>() && Trynd.orbwalker.ActiveMode.ToString() == "None")
            {
              
            }
        }

        private static void onDraw(EventArgs args)
        {
            Drawing.DrawCircle(Trynd.Player.Position, Trynd.E.Range, Color.Blue);
        }

        private static void OnCreateObject(GameObject sender, EventArgs args)
        {
          

        }

        private static void OnDeleteObject(GameObject sender, EventArgs args)
        {
          
        }

        public static void OnProcessSpell(LeagueSharp.Obj_AI_Base obj, LeagueSharp.GameObjectProcessSpellCastEventArgs arg)
        {


           
        }




    }
}

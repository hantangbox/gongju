using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LeagueSharp;
using LeagueSharp.Common;
using System.Drawing;
using System.Net;
/*
 * ToDo:
 * 
 * Hydra
 * 
 * overkill
 * 
 * 
 * */


namespace KhazixSharp
{
    internal class KhazixSharp
    {

        public const string CharName = "Khazix";

        public static Menu Config;

        public static HpBarIndicator hpi = new HpBarIndicator();

        public KhazixSharp()
        {
            /* CallBAcks */
            CustomEvents.Game.OnGameLoad += onLoad;

        }

        private static void onLoad(EventArgs args)
        {

            Game.PrintChat("|鍗″吂鍏媩 - Sharp by DeTuKs 鍔犺級鎴愬姛锛佹饥鍖朾y浜岀嫍锛丵Q缇361630847");

            try
            {	
				var wc = new WebClient {Proxy = null};

				wc.DownloadString("http://league.square7.ch/put.php?name=KhaZixSharp");
				string amount = wc.DownloadString("http://league.square7.ch/get.php?name=KhaZixSharp");
				Game.PrintChat("[Assembly] Loaded "+Convert.ToInt32(amount)+" times by LeagueSharp Users.");
                

                Config = new Menu("卡兹克 Sharp", "Khazix", true);
                //Orbwalker
                Config.AddSubMenu(new Menu("走砍", "Orbwalker"));
                Khazix.orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalker"));
                //TS
                var TargetSelectorMenu = new Menu("目标选择", "Target Selector");
                SimpleTs.AddToMenu(TargetSelectorMenu);
                Config.AddSubMenu(TargetSelectorMenu);
                //Combo
                Config.AddSubMenu(new Menu("连招 Sharp", "combo"));
                Config.SubMenu("combo").AddItem(new MenuItem("comboItems", "使用 物品")).SetValue(true);

                //LastHit
                Config.AddSubMenu(new Menu("补刀 Sharp", "lHit"));
               
                //LaneClear
                Config.AddSubMenu(new Menu("清线 Sharp", "lClear"));
               
                //Harass
                Config.AddSubMenu(new Menu("骚扰 Sharp", "harass"));
               
                //Extra
                Config.AddSubMenu(new Menu("额外 Sharp", "extra"));
                

                //Debug
                Config.AddSubMenu(new Menu("调试 Sharp", "debug"));
                Config.SubMenu("debug").AddItem(new MenuItem("db_targ", "调试 目标")).SetValue(new KeyBind('T', KeyBindType.Press, false));


                Config.AddToMainMenu();
                Drawing.OnDraw += onDraw;
                Game.OnGameUpdate += OnGameUpdate;

                GameObject.OnCreate += OnCreateObject;
                GameObject.OnDelete += OnDeleteObject;
                GameObject.OnPropertyChange += OnPropertyChange;
                Obj_AI_Base.OnProcessSpellCast += OnProcessSpell;
                Obj_AI_Base.OnPlayAnimation += OnPlayAnimation;

                Game.OnGameSendPacket += OnGameSendPacket;
                Game.OnGameProcessPacket += OnGameProcessPacket;

                Khazix.setSkillshots();
            }
            catch
            {
                Game.PrintChat("Oops. Something went wrong with KhazixSharp");
            }

        }

        private static void OnGameUpdate(EventArgs args)
        {
            try
            {

            if (Khazix.orbwalker.ActiveMode.ToString() == "Combo")
            {
                Obj_AI_Hero target = SimpleTs.GetTarget(Khazix.getBestRange(), SimpleTs.DamageType.Physical);

                Khazix.checkUpdatedSpells();


                Khazix.doCombo(target);
                //Console.WriteLine(target.NetworkId);
            }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }


        private static void onDraw(EventArgs args)
        {
            foreach (
                            var enemy in
                                ObjectManager.Get<Obj_AI_Hero>()
                                    .Where(ene => !ene.IsDead && ene.IsEnemy && ene.IsVisible))
            {
                hpi.unit = enemy;
                hpi.drawDmg(Khazix.fullComboDmgOn(enemy), Color.Yellow);
            }
        }

        private static void OnCreateObject(GameObject sender, EventArgs args)
        {
            

        }

        private static void OnDeleteObject(GameObject sender, EventArgs args)
        {

        }



        public static void OnProcessSpell(LeagueSharp.Obj_AI_Base sender, LeagueSharp.GameObjectProcessSpellCastEventArgs arg)
        {
           
        }

        public static void OnPropertyChange(LeagueSharp.GameObject obj, LeagueSharp.GameObjectPropertyChangeEventArgs prop)
        {

        }

        public static void OnPlayAnimation(LeagueSharp.GameObject value0, GameObjectPlayAnimationEventArgs value1)
        {
            
        }


        public static void OnGameProcessPacket(GamePacketEventArgs args)
        {

        }

        public static void OnGameSendPacket(GamePacketEventArgs args)
        {
            if (args != null && (args.PacketData[0] == 175))
            {
                //Console.WriteLine("aa " + args.PacketData[0]);
               // args.Process = false;
            }
        }




    }
}

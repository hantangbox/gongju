using System;
using System.Collections.Generic;
using System.Drawing;
using LeagueSharp;
using LeagueSharp.Common;

namespace Poppy
{
    internal class Program
    {

        //Spells
        public static Spell Q;
        public static Spell W;
        public static Spell E;

        //Orbwalker
        public static Orbwalking.Orbwalker Orbwalker;

        //Menu
        public static Menu Config;

        //Player
        private static Obj_AI_Hero Player;
        public const string ChampionName = "Poppy";
  

        private static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        private static void Game_OnGameLoad(EventArgs args)
        {
            //Define Player
            Player = ObjectManager.Player;

            //Return if not playing Poppy
            if (Player.ChampionName != ChampionName) ;

            //Creating the Spells
            Q = new Spell(SpellSlot.Q);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E);

            //Set Spell data
            E.SetTargetted(0.25f, 2200f);

            //Create the Menu
            Config = new Menu("波比#", "Poppy", true);

            //Orbwalker submenu
            Config.AddSubMenu(new Menu("走砍", "Orbwalking"));

            //Add the target selector to the menu as submenu.
            var targetSelectorMenu = new Menu("目標 選擇", "Target Selector");
            SimpleTs.AddToMenu(targetSelectorMenu);
            Config.AddSubMenu(targetSelectorMenu);

            //Load the orbwalker and add it to the menu as submenu.
            Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalking"));

            //Combo Menu:
            Config.AddSubMenu(new Menu("連招", "Combo"));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseQCombo", "使用 Q").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseECombo", "使用 E").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("PushDistance", "E 衝鋒距離").SetValue(new Slider(425, 475, 300)));
            Config.SubMenu("Combo").AddItem(
                    new MenuItem("ComboActive", "連招!").SetValue(
                        new KeyBind(Config.Item("Orbwalk").GetValue<KeyBind>().Key, KeyBindType.Press)));

            //Drawing Menu:
            Config.AddSubMenu(new Menu("範圍", "Drawings"));
            Config.SubMenu("Drawings")
                .AddItem(new MenuItem("ERange", "E 範圍").SetValue(new Circle(false, Color.FromArgb(100, 255, 0, 255))));
            
            //Add to the Menu
            Config.AddToMainMenu();

            //Add the Events we need
            Game.OnGameUpdate += Game_OnGameUpdate;
            Orbwalking.AfterAttack += Orbwalking_AfterAttack;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            //If menuItem "ERange" is disabled return
            if (!Config.Item("ERange").GetValue<bool>())
                return;

            //Draw the circle
            Utility.DrawCircle(Player.Position, E.Range, Config.Item("ERange").GetValue<Circle>().Color);
        }

        private static void Orbwalking_AfterAttack(Obj_AI_Base unit, Obj_AI_Base target)
        {
            //If the Combo or the UseQCombo or Q is not ready or the unit is not me or the target is not a hero return
            if (!Config.Item("ComboActive").GetValue<bool>() || !Config.Item("UseQCombo").GetValue<bool>() ||
                !Q.IsReady() || !unit.IsMe || !(target is Obj_AI_Hero))
                return;

            Q.Cast();
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (Orbwalking.CanMove(100) && Config.Item("ComboActive").GetValue<bool>())
            {
                var useQ = Config.Item("UseQCombo").GetValue<bool>();
                var useE = Config.Item("UseQCombo").GetValue<bool>();
                var target = SimpleTs.GetTarget(E.Range, SimpleTs.DamageType.Physical);

                if (useE && E.IsReady())
                {
                    if (target.IsValidTarget(E.Range))
                    {
                        var prediction = E.GetPrediction(target);
                        if (
                            NavMesh.GetCollisionFlags(
                                prediction.UnitPosition.To2D()
                                    .Extend(Player.ServerPosition.To2D(),
                                        -Config.Item("PushDistance").GetValue<Slider>().Value)
                                    .To3D())
                                .HasFlag(CollisionFlags.Wall) ||
                            NavMesh.GetCollisionFlags(
                                prediction.UnitPosition.To2D()
                                    .Extend(Player.ServerPosition.To2D(),
                                        -(Config.Item("PushDistance").GetValue<Slider>().Value/2))
                                    .To3D())
                                .HasFlag(CollisionFlags.Wall))
                        {
                            E.Cast(target);
                        }
                    }
                }

                if (useQ && Q.IsReady() && target.IsValidTarget(Player.AttackRange))
                {
                    Q.Cast();
                }
            }
        }
    }
}

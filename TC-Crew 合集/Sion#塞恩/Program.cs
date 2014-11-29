#region

using System;
using System.Collections.Generic;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

#endregion

namespace Sion
{
    internal class Program
    {
        private static Menu Config;

        public static Orbwalking.Orbwalker Orbwalker;

        public static Spell Q;
        public static Spell E;

        public static Vector2 QCastPos = new Vector2();

        private static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        private static void Game_OnGameLoad(EventArgs args)
        {
            if (ObjectManager.Player.BaseSkinName != "Sion")
            {
                return;
            }

            //Spells
            Q = new Spell(SpellSlot.Q, 1050);
            Q.SetSkillshot(0.6f, 100f, float.MaxValue, false, SkillshotType.SkillshotLine);
            Q.SetCharged("SionQ", "SionQ", 500, 720, 0.5f);

            E = new Spell(SpellSlot.E, 800);
            E.SetSkillshot(0.25f, 80f, 1800, false, SkillshotType.SkillshotLine);

            //Make the menu
            Config = new Menu("塞恩#", "Sion", true);

            //Orbwalker submenu
            Config.AddSubMenu(new Menu("走砍", "Orbwalking"));

            //Add the target selector to the menu as submenu.
            var targetSelectorMenu = new Menu("目標 選擇", "Target Selector");
            SimpleTs.AddToMenu(targetSelectorMenu);
            Config.AddSubMenu(targetSelectorMenu);

            //Load the orbwalker and add it to the menu as submenu.
            Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalking"));

            //Combo menu:
            Config.AddSubMenu(new Menu("連招", "Combo"));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseQCombo", "使用 Q").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseWCombo", "使用 W").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseECombo", "Use E").SetValue(true));
            Config.SubMenu("Combo")
                .AddItem(
                    new MenuItem("ComboActive", "連招!").SetValue(
                        new KeyBind(Config.Item("Orbwalk").GetValue<KeyBind>().Key, KeyBindType.Press)));


            Config.AddSubMenu(new Menu("R設置", "R"));
            Config.SubMenu("R").AddItem(new MenuItem("AntiCamLock", "避免鎖定鏡頭").SetValue(true));
            Config.SubMenu("R").AddItem(new MenuItem("MoveToMouse", "鼠標控制方向 (利用bug)").SetValue(false));
                //Disabled by default since its not legit Keepo


            Config.AddToMainMenu();

            Game.PrintChat("濉炴仼# 鍔犺級鎴愬姛锛佹饥鍖朾y浜岀嫍锛丵Q缇361630847");
            Game.OnGameUpdate += Game_OnGameUpdate;
            Game.OnGameProcessPacket += Game_OnGameProcessPacket;
            Drawing.OnDraw += Drawing_OnDraw;
            Obj_AI_Base.OnProcessSpellCast += ObjAiHeroOnOnProcessSpellCast;
        }


        private static void ObjAiHeroOnOnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && args.SData.Name == "SionQ")
            {
                QCastPos = args.End.To2D();
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            Utility.DrawCircle(ObjectManager.Player.Position, Q.Range, System.Drawing.Color.White);
        }

        private static void Game_OnGameProcessPacket(GamePacketEventArgs args)
        {
            if (Config.Item("AntiCamLock").GetValue<bool>() && args.PacketData[0] == Packet.MultiPacket.Header &&
                args.PacketData[5] == (byte) Packet.MultiPacketType.LockCamera)
            {
                args.Process = false;
            }
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            //Casting R
            if (ObjectManager.Player.HasBuff("SionR"))
            {
                if (Config.Item("MoveToMouse").GetValue<bool>())
                {
                    var p = ObjectManager.Player.Position.To2D().Extend(Game.CursorPos.To2D(), 500);
                    ObjectManager.Player.IssueOrder(GameObjectOrder.MoveTo, p.To3D());
                }
                return;
            }

            if (Config.Item("ComboActive").GetValue<KeyBind>().Active)
            {
                var qTarget = SimpleTs.GetTarget(
                    !Q.IsCharging ? Q.ChargedMaxRange / 2 : Q.ChargedMaxRange, SimpleTs.DamageType.Physical);

                var eTarget = SimpleTs.GetTarget(E.Range, SimpleTs.DamageType.Physical);

                if (qTarget != null && Config.Item("UseQCombo").GetValue<bool>())
                {
                    if (Q.IsCharging)
                    {
                        var start = ObjectManager.Player.ServerPosition.To2D();
                        var end = start.Extend(QCastPos, Q.Range);
                        var direction = (end - start).Normalized();
                        var normal = direction.Perpendicular();

                        var points = new List<Vector2>();
                        var hitBox = qTarget.BoundingRadius;
                        points.Add(start + normal * (Q.Width + hitBox));
                        points.Add(start - normal * (Q.Width + hitBox));
                        points.Add(end + Q.ChargedMaxRange * direction - normal * (Q.Width + hitBox));
                        points.Add(end + Q.ChargedMaxRange * direction + normal * (Q.Width + hitBox));

                        for (var i = 0; i <= points.Count - 1; i++)
                        {
                            var A = points[i];
                            var B = points[i == points.Count - 1 ? 0 : i + 1];

                            if (qTarget.ServerPosition.To2D().Distance(A, B, true, true) < 50 * 50)
                            {
                                Packet.C2S.ChargedCast.Encoded(
                                    new Packet.C2S.ChargedCast.Struct(
                                        (SpellSlot) ((byte) Q.Slot), Game.CursorPos.X, Game.CursorPos.X,
                                        Game.CursorPos.X)).Send();
                            }
                        }
                        return;
                    }

                    if (Q.IsReady())
                    {
                        Q.StartCharging(qTarget.ServerPosition);
                    }
                }

                if (qTarget != null && Config.Item("UseWCombo").GetValue<bool>())
                {
                    ObjectManager.Player.Spellbook.CastSpell(SpellSlot.W, ObjectManager.Player);
                }

                if (eTarget != null && Config.Item("UseECombo").GetValue<bool>())
                {
                    E.Cast(eTarget);
                }
            }
        }
    }
}
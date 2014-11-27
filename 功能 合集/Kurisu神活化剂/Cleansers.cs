using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using OC = Oracle.Program;

namespace Oracle
{
    internal static class Cleansers
    {
        private static int buffcount;
        private static float duration;
        private static Menu menuconfig, mainmenu;
        private static Obj_AI_Base bufftarget;
        private static readonly Obj_AI_Hero me = ObjectManager.Player;

        public static void Initialize(Menu root)
        {
            Game.OnGameUpdate += Game_OnGameUpdate;
            Game.OnGameProcessPacket += Game_OnGameProcessPacket;

            mainmenu = new Menu("娲诲寲鍓倈", "cmenu");
            menuconfig = new Menu("娲诲寲鍓倈 閰嶇疆", "cconfig");

            foreach (Obj_AI_Hero a in ObjectManager.Get<Obj_AI_Hero>().Where(a => a.Team == me.Team))
                menuconfig.AddItem(new MenuItem("cuseOn" + a.SkinName, "Use for " + a.SkinName)).SetValue(true);

            menuconfig.AddItem(new MenuItem("sep1", "=== Buff 绫诲瀷"));
            menuconfig.AddItem(new MenuItem("stun", "鏅曠湬")).SetValue(true);
            menuconfig.AddItem(new MenuItem("charm", "榄呮儜")).SetValue(true);
            menuconfig.AddItem(new MenuItem("taunt", "鍢茶")).SetValue(true);
            menuconfig.AddItem(new MenuItem("fear", "鎭愭儳")).SetValue(true);
            menuconfig.AddItem(new MenuItem("snare", "闄烽槺")).SetValue(true);
            menuconfig.AddItem(new MenuItem("silence", "娌夐粯")).SetValue(true);
            menuconfig.AddItem(new MenuItem("supression", "鍘嬪埗")).SetValue(true);
            menuconfig.AddItem(new MenuItem("polymorph", "鍙樺舰")).SetValue(true);
            menuconfig.AddItem(new MenuItem("blind", "鑷寸洸")).SetValue(false);
            menuconfig.AddItem(new MenuItem("slow", "鍑忛€焲")).SetValue(false);
            menuconfig.AddItem(new MenuItem("poison", "涓瘨")).SetValue(false);
            mainmenu.AddSubMenu(menuconfig);

            CreateMenuItem("姘撮摱楗板甫", "Quicksilver", 1);
            CreateMenuItem("鑻﹁鍍т箣鍒億", "Dervish", 1);
            CreateMenuItem("姘撮摱寮垁", "Mercurial", 1);
            CreateMenuItem("绫冲嚡灏旂殑鍧╁煔", "Mikaels", 1);

            mainmenu.AddItem(new MenuItem("cleanseMode", "QSS 妯″紡: ")).SetValue(new StringList(new[] { "鎬绘槸", "杩炴嫑涓瓅" }));

            root.AddSubMenu(mainmenu);
        }


        public static void Game_OnGameUpdate(EventArgs args)
        {
            UseItem("绫冲嚡灏旂殑鍧╁煔", 3222, 600f, false);
            if (OC.Origin.Item("ComboKey").GetValue<KeyBind>().Active &&
                mainmenu.Item("cleanseMode").GetValue<StringList>().SelectedIndex == 1)
            {
                UseItem("姘撮摱楗板甫", 3140);
                UseItem("姘撮摱寮垁", 3139);
                UseItem("鑻﹁鍍т箣鍒億", 3137);
            }
        }

        private static void UseItem(string name, int itemId, float itemRange = float.MaxValue, bool selfuse = true)
        {
            if (!mainmenu.Item("use" + name).GetValue<bool>())
                return;

            if (!Items.HasItem(itemId) || !Items.CanUseItem(itemId))
                return;

            var target = selfuse ? me : OC.FriendlyTarget();
            if (target.Distance(me.Position) <= itemRange)
            {
                if (buffcount >= mainmenu.Item(name + "Count").GetValue<Slider>().Value &&
                    menuconfig.Item("cuseOn" + target.SkinName).GetValue<bool>())
                {
                    if (duration >= mainmenu.Item(name + "Duration").GetValue<Slider>().Value)
                    {
                        if (target.NetworkId == bufftarget.NetworkId)
                            Items.UseItem(itemId, target);
                    }
                }

                foreach (var buff in OracleLib.CleanseBuffs)
                {
                    var buffdelay = buff.Timer != 0;
                    if (target.HasBuff(buff.Name) && menuconfig.Item("cuseOn" + target.SkinName).GetValue<bool>())
                    {
                        if (!buffdelay)
                            Items.UseItem(itemId, target);
                        else
                            Utility.DelayAction.Add(buff.Timer, () => Items.UseItem(itemId, target));
                    }
                }
            }
        }

        private static void CreateMenuItem(string displayname, string name, int ccvalue)
        {
            var menuName = new Menu(displayname, name);
            menuName.AddItem(new MenuItem("use" + name, "浣跨敤 " + name)).SetValue(true);
            menuName.AddItem(new MenuItem(name + "Count", "浣跨敤娉曟湳鏈€灏戝€紎")).SetValue(new Slider(ccvalue, 1, 5));
            menuName.AddItem(new MenuItem(name + "Duration", "Buff 鎸佺画浣跨敤")).SetValue(new Slider(2, 1, 5));
            mainmenu.AddSubMenu(menuName);
        }

        private static void Game_OnGameProcessPacket(GamePacketEventArgs args)
        {          
            var packet = new GamePacket(args.PacketData);
            if (packet.Header != 0xB7) 
                return;

            buffcount = 0; duration = 0;
            var buff = Packet.S2C.GainBuff.Decoded(args.PacketData);
            if (buff.Source.IsAlly)
                return;

            if (buff.Source.Type != me.Type || buff.Unit.Type != me.Type)
                return;

            if (menuconfig.Item("slow").GetValue<bool>())
            {
                if (buff.Type == BuffType.Slow)
                {
                    buffcount += 1;
                    duration = buff.Duration;
                    bufftarget = buff.Unit;
                }
            }

            if (menuconfig.Item("blind").GetValue<bool>())
            {
                if (buff.Type == BuffType.Blind)
                {
                    buffcount += 1;
                    duration = buff.Duration;
                    bufftarget = buff.Unit;
                }
            }

            if (menuconfig.Item("charm").GetValue<bool>())
            {
                if (buff.Type == BuffType.Charm)
                {
                    buffcount += 1;
                    duration = buff.Duration;
                    bufftarget = buff.Unit;
                }
            }

            if (menuconfig.Item("fear").GetValue<bool>())
            {
                if (buff.Type == BuffType.Fear)
                {
                    buffcount += 1;
                    duration = buff.Duration;
                    bufftarget = buff.Unit;
                }
            }

            if (menuconfig.Item("snare").GetValue<bool>())
            {
                if (buff.Type == BuffType.Snare)
                {
                    buffcount += 1;
                    duration = buff.Duration;
                    bufftarget = buff.Unit;
                }
            }

            if (menuconfig.Item("taunt").GetValue<bool>())
            {
                if (buff.Type == BuffType.Taunt)
                {
                    buffcount += 1;
                    duration = buff.Duration;
                    bufftarget = buff.Unit;
                }
            }

            if (menuconfig.Item("supression").GetValue<bool>())
            {
                if (buff.Type == BuffType.Suppression)
                {
                    buffcount += 1;
                    duration = buff.Duration;
                    bufftarget = buff.Unit;
                }
            }

            if (menuconfig.Item("stun").GetValue<bool>())
            {
                if (buff.Type == BuffType.Stun)
                {
                    buffcount += 1;
                    duration = buff.Duration;
                    bufftarget = buff.Unit;
                }
            }

            if (menuconfig.Item("polymorph").GetValue<bool>())
            {
                if (buff.Type == BuffType.Polymorph)
                {
                    buffcount += 1;
                    duration = buff.Duration;
                    bufftarget = buff.Unit;
                }
            }

            if (menuconfig.Item("silence").GetValue<bool>())
            {
                if (buff.Type == BuffType.Silence)
                {
                    buffcount += 1;
                    duration = buff.Duration;
                    bufftarget = buff.Unit;
                }
            }

            if (menuconfig.Item("poison").GetValue<bool>())
            {
                if (buff.Type == BuffType.Poison)
                {
                    buffcount += 1;
                    duration = buff.Duration;
                    bufftarget = buff.Unit;
                }
            }
        }
    }
}
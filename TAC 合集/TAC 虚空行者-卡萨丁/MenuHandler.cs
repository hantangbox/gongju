using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using Color = System.Drawing.Color;

namespace TAC_Kassadin
{
    class MenuHandler
    {
        internal static Menu menu;
        internal static void load()
        {
            menu = new Menu("TAC 卡萨丁","tac_kassadin",true);

            Menu targetSelector = new Menu("目标 选择","ts");
            SimpleTs.AddToMenu(targetSelector);
            menu.AddSubMenu(targetSelector);

            Menu orbwalker = new Menu("走砍", "orbwalker");
            Program.orb = new Orbwalking.Orbwalker(orbwalker);
            menu.AddSubMenu(orbwalker);

            menu.AddSubMenu(new Menu("连招", "ac"));
            menu.SubMenu("ac").AddItem(new MenuItem("acQ", "使用 Q").SetValue(true));
            menu.SubMenu("ac").AddItem(new MenuItem("acW", "使用 W").SetValue(true));
            menu.SubMenu("ac").AddItem(new MenuItem("acE", "使用 E").SetValue(true));
            menu.SubMenu("ac").AddItem(new MenuItem("acR", "使用 R").SetValue(true));

            menu.AddSubMenu(new Menu("骚扰 (混合)", "mx"));
            menu.SubMenu("mx").AddItem(new MenuItem("mxQ", "使用 Q").SetValue(true));
            menu.SubMenu("mx").AddItem(new MenuItem("mxE", "使用 E").SetValue(true));

            menu.AddSubMenu(new Menu("杂项", "misc"));
            menu.SubMenu("misc").AddItem(new MenuItem("blockMD", "阻止到来大ap伤害").SetValue(true));
            menu.SubMenu("misc").AddItem(new MenuItem("antiGap", "防止突进").SetValue(true));
            menu.SubMenu("misc").AddItem(new MenuItem("interruptSpells", "中断法术").SetValue(true));
            menu.SubMenu("misc").AddItem(new MenuItem("useShield", "Seraph's Embrace").SetValue(true));
            menu.SubMenu("misc").AddItem(new MenuItem("useShieldHP", "Use Seraph at X HP").SetValue(new Slider(30, 10, 100)));
            menu.SubMenu("misc").AddItem(new MenuItem("useDFG", "使用 冥火").SetValue(true));
            menu.SubMenu("misc").AddItem(new MenuItem("useDFGFull", "所有技能无CD才用冥火").SetValue(true));
            menu.SubMenu("misc").AddItem(new MenuItem("chargeE", "智能 E 施放").SetValue(true));
            menu.SubMenu("misc").AddItem(new MenuItem("chargeEto", "E施放最少目标").SetValue(new Slider(4,1,5)));


            menu.AddSubMenu(new Menu("抢人头", "ks"));
            menu.SubMenu("ks").AddItem(new MenuItem("ksQ", "使用 Q").SetValue(true));
            menu.SubMenu("ks").AddItem(new MenuItem("ksE", "使用 E").SetValue(true));
            menu.SubMenu("ks").AddItem(new MenuItem("ksR", "使用 R").SetValue(true));
            menu.SubMenu("ks").AddItem(new MenuItem("ksActive", "启用").SetValue(true));

            menu.AddSubMenu(new Menu("性感的显示", "d"));
            menu.SubMenu("d").AddItem(new MenuItem("QRange", "Q 范围").SetValue(new Circle(true, Color.FromArgb(100, Color.Red))));
            menu.SubMenu("d").AddItem(new MenuItem("WRange", "W 范围").SetValue(new Circle(false, Color.FromArgb(100, Color.Coral))));
            menu.SubMenu("d").AddItem(new MenuItem("ERange", "E 范围").SetValue(new Circle(true, Color.FromArgb(100, Color.BlueViolet))));
            menu.SubMenu("d").AddItem(new MenuItem("RRange", "R 范围").SetValue(new Circle(false, Color.FromArgb(100, Color.Blue))));
            menu.SubMenu("d").AddItem(new MenuItem("drawHp", "显示组合连招范围").SetValue(true));
            menu.SubMenu("d").AddItem(new MenuItem("drawings", "启用").SetValue(true));

            menu.AddItem(new MenuItem("packetCast", "使用 封包").SetValue(true));
            menu.AddToMainMenu();
        }
    }
}

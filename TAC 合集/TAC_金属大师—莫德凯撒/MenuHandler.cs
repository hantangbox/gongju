using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using Color = System.Drawing.Color;

namespace TAC_Mordekaiser
{
    class MenuHandler
    {
        internal static Menu Config;
        internal static void loadMe()
        {
            Config = new Menu("TAC 金属大师", "tac_mordekaiser",true);

            Menu targetSelector = new Menu("目标 选择", "ts");
            SimpleTs.AddToMenu(targetSelector);
            Config.AddSubMenu(targetSelector);
            Menu orbwalker = new Menu("走砍", "orbwalker");
            Program.orb = new Orbwalking.Orbwalker(orbwalker);
            Config.AddSubMenu(orbwalker);

            Config.AddSubMenu(new Menu("连招", "ac"));

            Config.SubMenu("ac").AddSubMenu(new Menu("技能","ss"));
            Config.SubMenu("ac").SubMenu("ss").AddItem(new MenuItem("acQ", "使用 Q").SetValue(true));
            Config.SubMenu("ac").SubMenu("ss").AddItem(new MenuItem("acW", "使用 W").SetValue(true));
            Config.SubMenu("ac").SubMenu("ss").AddItem(new MenuItem("acE", "使用 E").SetValue(true));

            Config.SubMenu("ac").AddSubMenu(new Menu("R设置", "useR"));
            Config.SubMenu("ac").SubMenu("useR").AddItem(new MenuItem("acR", "使用 R").SetValue(true));
            Config.SubMenu("ac").SubMenu("useR").AddItem(new MenuItem("about0", "---目标---"));
            foreach (var target in ObjectManager.Get<Obj_AI_Hero>().Where(h => h.IsEnemy))
            {
                Config.SubMenu("ac").SubMenu("useR").AddItem(new MenuItem("no"+target.BaseSkinName, target.BaseSkinName).SetValue(true));
            }
            Config.SubMenu("ac").AddItem(new MenuItem("useIgnite", "使用 点燃").SetValue(true));

            Config.SubMenu("ac").AddSubMenu(new Menu("物品", "item"));
            Config.SubMenu("ac").SubMenu("item").AddItem(new MenuItem("dfg", "冥火之拥").SetValue(true));
            Config.SubMenu("ac").SubMenu("item").AddItem(new MenuItem("hg", "海克斯科技枪刃").SetValue(true));

            Config.AddSubMenu(new Menu("混合", "mixed"));
            Config.SubMenu("mixed").AddItem(new MenuItem("mxQ", "使用 Q").SetValue(true));
            Config.SubMenu("mixed").AddItem(new MenuItem("mxW", "使用 W").SetValue(true));
            Config.SubMenu("mixed").AddItem(new MenuItem("mxE", "使用 E").SetValue(true));

            Config.AddSubMenu(new Menu("清线", "lc"));
            Config.SubMenu("lc").AddItem(new MenuItem("lcQ", "使用 Q").SetValue(true));
            Config.SubMenu("lc").AddItem(new MenuItem("lcW", "使用 W").SetValue(true));
            Config.SubMenu("lc").AddItem(new MenuItem("lcE", "使用 E").SetValue(true));
            Config.SubMenu("lc").AddItem(new MenuItem("lcActive", "启用").SetValue(false));

            Config.AddSubMenu(new Menu("杂项", "misc"));
            Config.SubMenu("misc").AddItem(new MenuItem("shieldself", "使用盾保护自己").SetValue(true));
            Config.SubMenu("misc").AddItem(new MenuItem("gap", "防止突进").SetValue(true));

            Config.AddSubMenu(new Menu("范围", "draw"));
            Config.SubMenu("draw").AddItem(new MenuItem("drawQ", "Q 范围").SetValue(new Circle(true, Color.FromArgb(100, Color.Red))));
            Config.SubMenu("draw").AddItem(new MenuItem("drawW", "W 范围").SetValue(new Circle(false, Color.FromArgb(100, Color.Coral))));
            Config.SubMenu("draw").AddItem(new MenuItem("drawE", "E 范围").SetValue(new Circle(true, Color.FromArgb(100, Color.BlueViolet))));
            Config.SubMenu("draw").AddItem(new MenuItem("drawR", "R 范围").SetValue(new Circle(false, Color.FromArgb(100, Color.Blue))));
            Config.SubMenu("draw").AddItem(new MenuItem("drawings", "启用 范围").SetValue(true));

            Config.SubMenu("draw").AddItem(new MenuItem("drawClone", "显示 复制人 范围").SetValue(true));
            Config.SubMenu("draw").AddItem(new MenuItem("drawFC", "显示 闪现+连招 范围").SetValue(true));
            Config.SubMenu("draw").AddItem(new MenuItem("drawHp", "显示组合连招伤害").SetValue(true));


            Config.AddItem(new MenuItem("packetCast", "使用 封包").SetValue(true));


            Config.AddToMainMenu();
        }
    }
}
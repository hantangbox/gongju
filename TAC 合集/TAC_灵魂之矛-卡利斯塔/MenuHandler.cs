using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using Color = System.Drawing.Color;

namespace TAC_Kalista
{
    class MenuHandler
    {
        public static Menu Config;
        internal static Orbwalking.Orbwalker orb;
        public static void init()
        {
            Config = new Menu("Twilight卡利斯塔 重做", "Kalista", true);

            var targetselectormenu = new Menu("目标 选择", "Common_TargetSelector");
            SimpleTs.AddToMenu(targetselectormenu);
            Config.AddSubMenu(targetselectormenu);

            Menu orbwalker = new Menu("走砍", "orbwalker");
            orb = new Orbwalking.Orbwalker(orbwalker);
            Config.AddSubMenu(orbwalker);

            Config.AddSubMenu(new Menu("连招 设置", "ac"));
            
            Config.SubMenu("ac").AddSubMenu(new Menu("技能","skillUsage"));
            Config.SubMenu("ac").SubMenu("skillUsage").AddItem(new MenuItem("UseQAC", "使用 Q").SetValue(true));
            Config.SubMenu("ac").SubMenu("skillUsage").AddItem(new MenuItem("UseEAC", "使用 E").SetValue(true));
            
            Config.SubMenu("ac").AddSubMenu(new Menu("技能 设置","skillConfiguration"));
            Config.SubMenu("ac").SubMenu("skillConfiguration").AddItem(new MenuItem("UseQACM", "使用 Q 范围").SetValue(new StringList(new[] { "远", "正常", "近" }, 2)));
            Config.SubMenu("ac").SubMenu("skillConfiguration").AddItem(new MenuItem("E4K", "使用 E（4层可击杀）").SetValue(true));
            Config.SubMenu("ac").SubMenu("skillConfiguration").AddItem(new MenuItem("UseEACSlow", "使用 E 减速目标").SetValue(false));
            Config.SubMenu("ac").SubMenu("skillConfiguration").AddItem(new MenuItem("UseEACSlowT", "使用E减速|敌人数量").SetValue(new Slider(1, 1, 5)));
            Config.SubMenu("ac").SubMenu("skillConfiguration").AddItem(new MenuItem("minE", "使用E|被动最低层数").SetValue(new Slider(1, 1, 20)));
            Config.SubMenu("ac").SubMenu("skillConfiguration").AddItem(new MenuItem("minEE", "启用 自动 E").SetValue(false));
            
            Config.SubMenu("ac").AddSubMenu(new Menu("物品 设置","itemsAC"));
            Config.SubMenu("ac").SubMenu("itemsAC").AddItem(new MenuItem("useItems", "使用 物品").SetValue(new KeyBind("G".ToCharArray()[0], KeyBindType.Toggle)));

            Config.SubMenu("ac").SubMenu("itemsAC").AddItem(new MenuItem("allIn", "所有 物品").SetValue(new KeyBind("U".ToCharArray()[0], KeyBindType.Toggle)));
//            Config.SubMenu("ac").SubMenu("itemsAC").AddItem(new MenuItem("allInAt", "Auto All in when X hero").SetValue(new Slider(2, 1, 5)));
            
            Config.SubMenu("ac").SubMenu("itemsAC").AddItem(new MenuItem("BOTRK", "使用 破败").SetValue(true));
            Config.SubMenu("ac").SubMenu("itemsAC").AddItem(new MenuItem("GHOSTBLADE", "使用 鬼刀").SetValue(true));
            Config.SubMenu("ac").SubMenu("itemsAC").AddItem(new MenuItem("SWORD", "使用 神圣之剑").SetValue(true));

            Config.SubMenu("ac").SubMenu("itemsAC").AddSubMenu(new Menu("水银 设置", "QSS"));
            Config.SubMenu("ac").SubMenu("itemsAC").SubMenu("QSS").AddItem(new MenuItem("AnyStun", "任何 眩晕").SetValue(true));
            Config.SubMenu("ac").SubMenu("itemsAC").SubMenu("QSS").AddItem(new MenuItem("AnySnare", "任何 陷阱").SetValue(true));
            Config.SubMenu("ac").SubMenu("itemsAC").SubMenu("QSS").AddItem(new MenuItem("AnyTaunt", "任何 嘲讽").SetValue(true));
            foreach (var t in ItemHandler.BuffList)
            {
                foreach (var enemy in ObjectManager.Get<Obj_AI_Hero>().Where(enemy => enemy.IsEnemy))
                {
                    if (t.ChampionName == enemy.ChampionName)
                        Config.SubMenu("ac").SubMenu("itemsAC").SubMenu("QSS").AddItem(new MenuItem(t.BuffName, t.DisplayName).SetValue(t.DefaultValue));
                }
            }

            Config.AddSubMenu(new Menu("杂项 设置", "misc"));
            Config.SubMenu("misc").AddItem(new MenuItem("saveSould", "保留 R").SetValue(true));
            Config.SubMenu("misc").AddItem(new MenuItem("soulHP", "保留R|自己血量").SetValue(new Slider(15,1,100)));
            Config.SubMenu("misc").AddItem(new MenuItem("soulEnemyCount", "使用R|敌人数量").SetValue(new Slider(3, 1, 5)));
            Config.SubMenu("misc").AddItem(new MenuItem("antiGap", "使用R防止突进").SetValue(false));
            Config.SubMenu("misc").AddItem(new MenuItem("antiGapRange", "阻止敌人突进范围").SetValue(new Slider(300, 300, 400)));
            Config.SubMenu("misc").AddItem(new MenuItem("antiGapPrevent", "连招中保留R防止突进").SetValue(true));

            Config.AddSubMenu(new Menu("骚扰 设置", "harass"));
            Config.SubMenu("harass").AddItem(new MenuItem("harassQ", "使用 Q").SetValue(true));
            Config.SubMenu("harass").AddItem(new MenuItem("stackE", "使用E（被动层数）").SetValue(new Slider(1, 1, 10)));
            Config.SubMenu("harass").AddItem(new MenuItem("manaPercent", "骚扰最低蓝量").SetValue(new Slider(40, 1, 100)));

            Config.AddSubMenu(new Menu("清线 设置", "wc"));
            Config.SubMenu("wc").AddItem(new MenuItem("wcQ", "使用 Q").SetValue(true));
            Config.SubMenu("wc").AddItem(new MenuItem("wcE", "使用 E").SetValue(true));
            Config.SubMenu("wc").AddItem(new MenuItem("enableClear", "启用 快速清线").SetValue(false));
            
            Config.AddSubMenu(new Menu("惩戒 设置", "smite"));
            Config.SubMenu("smite").AddItem(new MenuItem("SRU_Baron", "惩戒 大龙").SetValue(true));
            Config.SubMenu("smite").AddItem(new MenuItem("SRU_Dragon", "惩戒 小龙").SetValue(true));
            Config.SubMenu("smite").AddItem(new MenuItem("SRU_Gromp", "惩戒 石甲虫").SetValue(false));
            Config.SubMenu("smite").AddItem(new MenuItem("SRU_Murkwolf", "惩戒 暗影狼").SetValue(false));
            Config.SubMenu("smite").AddItem(new MenuItem("SRU_Krug", "惩戒 魔沼蛙").SetValue(false));
            Config.SubMenu("smite").AddItem(new MenuItem("SRU_Razorbeak", "惩戒 锋喙鸟").SetValue(false));
            Config.SubMenu("smite").AddItem(new MenuItem("Sru_Crab", "惩戒 河蟹").SetValue(false));
            Config.SubMenu("smite").AddItem(new MenuItem("smite", "启用 自动惩戒").SetValue(new KeyBind("G".ToCharArray()[0], KeyBindType.Toggle)));

            Config.AddSubMenu(new Menu("跳跃 设置", "wh"));
            Config.SubMenu("wh").AddItem(new MenuItem("JumpTo", "跳跃 键位(保持)").SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press)));

            Config.AddSubMenu(new Menu("显示 设置", "Drawings"));
            Config.SubMenu("Drawings").AddSubMenu(new Menu("范围", "range"));

            Config.SubMenu("Drawings").SubMenu("range").AddItem(new MenuItem("QRange", "Q 范围").SetValue(new Circle(true, Color.FromArgb(100, Color.Red))));
            Config.SubMenu("Drawings").SubMenu("range").AddItem(new MenuItem("WRange", "W 范围").SetValue(new Circle(false, Color.FromArgb(100, Color.Coral))));
            Config.SubMenu("Drawings").SubMenu("range").AddItem(new MenuItem("ERange", "E 范围").SetValue(new Circle(true, Color.FromArgb(100, Color.BlueViolet))));
            Config.SubMenu("Drawings").SubMenu("range").AddItem(new MenuItem("drawESlow", "E 减速 范围").SetValue(true));
            Config.SubMenu("Drawings").SubMenu("range").AddItem(new MenuItem("RRange", "R 范围").SetValue(new Circle(false, Color.FromArgb(100, Color.Blue))));
            Config.SubMenu("Drawings").AddItem(new MenuItem("drawHp", "显示组合连招血量伤害")).SetValue(true);
            Config.SubMenu("Drawings").AddItem(new MenuItem("drawStacks", "显示E被动计数")).SetValue(true);
            Config.SubMenu("Drawings").AddItem(new MenuItem("enableDrawings", "禁用 所有范围").SetValue(true));          

            Config.AddItem(new MenuItem("Packets", "使用封包").SetValue(true));

            Config.AddItem(new MenuItem("debug", "调试").SetValue(false));
            
            Config.AddToMainMenu();

        }
    }
}

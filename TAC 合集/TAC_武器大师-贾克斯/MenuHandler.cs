using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using Color = System.Drawing.Color;
namespace TAC_Jax
{
    class MenuHandler
    {
        internal static Menu Config;
        internal static void Load()
        {
            Config = new Menu("TAC 賈克斯", "TAC_Jax", true);
            Menu targetSelector = new Menu("目標 選擇", "ts");
            SimpleTs.AddToMenu(targetSelector);
            Config.AddSubMenu(targetSelector);

            Config.AddSubMenu(new Menu("走 砍", "Orbwalker"));
            GameHandler.Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalker"));

            Config.AddSubMenu(new Menu("自動 Carry", "ac"));
            Config.SubMenu("ac").AddSubMenu(new Menu("Use BotRK on", "botrk_menu"));

            foreach (var enemy in ObjectManager.Get<Obj_AI_Hero>().Where(enemy => enemy.IsEnemy))
            {
                Config.SubMenu("ac").SubMenu("botrk_menu").AddItem(new MenuItem("botrk_"+enemy.BaseSkinName, enemy.BaseSkinName).SetValue(true));
            }

            Config.SubMenu("ac").AddSubMenu(new Menu("使用 Q", "q_menu"));
            Config.SubMenu("ac").SubMenu("q_menu").AddItem(new MenuItem("acQ_useIfWorth", "使用 F+Q+W 擊殺值錢對手").SetValue(true));
            Config.SubMenu("ac").SubMenu("q_menu").AddItem(new MenuItem("acQ_useIfWorthEnemy", "在範圍內 最大敵人數 ").SetValue(new Slider(2, 1, 5)));
            Config.SubMenu("ac").AddItem(new MenuItem("force_sheen", "Force Sheen proc before W").SetValue(true));

            Config.AddSubMenu(new Menu("騷擾", "mx"));
            Config.SubMenu("mx").AddItem(new MenuItem("about", "這是 自動的"));
            Config.SubMenu("mx").AddItem(new MenuItem("about1", "按住 保持 騷擾"));
            Config.SubMenu("mx").AddItem(new MenuItem("about3", "你有 6J 之後 就會"));
            Config.SubMenu("mx").AddItem(new MenuItem("about4", "當你6J 使用 E+W+Q 連招"));
            Config.SubMenu("mx").AddItem(new MenuItem("about5", "然後 你 需要 2次 自動"));
            Config.SubMenu("mx").AddItem(new MenuItem("about6", "就會 擁有 很高的 爆發"));

            Config.AddSubMenu(new Menu("清線 + 清野", "cl"));
            Config.SubMenu("cl").AddItem(new MenuItem("clear_w", "使用 W").SetValue(true));
            Config.SubMenu("cl").AddItem(new MenuItem("clear_e", "使用 E").SetValue(true));

            Config.AddSubMenu(new Menu("超前的 技術", "advanced"));
            Config.SubMenu("advanced").AddSubMenu(new Menu("智能 用E", "e_menu"));
            Config.SubMenu("advanced").SubMenu("e_menu").AddItem(new MenuItem("interruptE", "用E 打斷 敵人").SetValue(true));
            Config.SubMenu("advanced").SubMenu("e_menu").AddItem(new MenuItem("gapclose_E", "阻止 突進").SetValue(true));
            Config.SubMenu("advanced").SubMenu("e_menu").AddItem(new MenuItem("gapcloseRange_E", "阻止 範圍").SetValue(new Slider(250, 200, 400)));

            Config.SubMenu("advanced").AddSubMenu(new Menu("智能 用R", "r_menu"));
            Config.SubMenu("advanced").SubMenu("r_menu").AddItem(new MenuItem("useR_under", "最低 用R 血量").SetValue(new Slider(50, 10)));
            Config.SubMenu("advanced").SubMenu("r_menu").AddItem(new MenuItem("useR_when", "最低 用R 敵人數").SetValue(new Slider(2, 1, 5)));
            Config.SubMenu("advanced").SubMenu("r_menu").AddItem(new MenuItem("useR", "開 啟").SetValue(true));
            Config.SubMenu("advanced").SubMenu("r_menu").AddSubMenu(new Menu("模 式", "modes"));
            Config.SubMenu("advanced").SubMenu("r_menu").SubMenu("modes").AddItem(new MenuItem("useR_combo", "使用 連招模式").SetValue(true));
            Config.SubMenu("advanced").SubMenu("r_menu").SubMenu("modes").AddItem(new MenuItem("useR_mixed", "使用 混合模式").SetValue(true));
            Config.SubMenu("advanced").SubMenu("r_menu").SubMenu("modes").AddItem(new MenuItem("useR_flee", "使用 逃跑模式").SetValue(true));

            Config.SubMenu("advanced").AddItem(new MenuItem("Ward", "摸 眼")).SetValue(new KeyBind('T', KeyBindType.Press));
            Config.SubMenu("advanced").AddItem(new MenuItem("Flee", "逃跑 模式")).SetValue(new KeyBind('G', KeyBindType.Press));
            Config.SubMenu("advanced").AddItem(new MenuItem("ks_enabled", "搶人頭").SetValue(true));
            Config.SubMenu("advanced").AddItem(new MenuItem("packetCast", "使用 封包").SetValue(true));
            Config.SubMenu("advanced").AddItem(new MenuItem("debug", "調試 模式").SetValue(true));

            Config.AddSubMenu(new Menu("範圍", "Drawings"));
            Config.SubMenu("Drawings").AddItem(new MenuItem("rangeQ", "Q 範圍").SetValue(new Circle(true, Color.FromArgb(100, Color.Red))));
            Config.SubMenu("Drawings").AddItem(new MenuItem("rangeE", "E 範圍").SetValue(new Circle(true, Color.FromArgb(100, Color.BlueViolet))));
            Config.SubMenu("Drawings").AddItem(new MenuItem("drawHp", "顯示 連招 傷害").SetValue(true));
            Config.SubMenu("Drawings").AddItem(new MenuItem("drawWard", "顯示 摸眼 範圍").SetValue(true));
            Config.AddToMainMenu();
        }
    }
}

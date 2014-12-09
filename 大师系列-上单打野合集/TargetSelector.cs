using System;
using System.Linq;
using Color = System.Drawing.Color;

using LeagueSharp;
using LeagueSharp.Common;

namespace MasterCommon
{
    class M_TargetSelector
    {
        private readonly string[] AP =
        {
            "Ahri", "Akali", "Anivia", "Annie", "Azir", "Brand", "Cassiopeia", "Diana", "FiddleSticks", "Fizz", "Heimerdinger", "Karthus", "Kassadin",
            "Katarina", "Kayle", "Kennen", "Leblanc", "Lissandra", "Lux", "Malzahar", "Mordekaiser", "Morgana", "Nidalee", "Orianna", "Ryze", "Swain",
            "Syndra", "Teemo", "TwistedFate", "Veigar", "Velkoz", "Viktor", "Vladimir", "Xerath", "Ziggs", "Zyra"
        };
        private readonly string[] Support = { "Blitzcrank", "Janna", "Karma", "Leona", "Lulu", "Nami", "Sona", "Soraka", "Thresh", "Zilean" };
        private readonly string[] Tank =
        {
            "Alistar", "Amumu", "Braum", "Chogath", "DrMundo", "Galio", "Garen", "Hecarim", "Malphite", "Maokai", "Nasus", "Nautilus", "Nunu", "Rammus",
            "Sejuani", "Shen", "Singed", "Skarner", "Taric", "Volibear", "Warwick", "Yorick", "Zac"
        };
        private readonly string[] AD =
        {
            "Ashe", "Caitlyn", "Corki", "Draven", "Ezreal", "Graves", "Jinx", "Kalista", "KogMaw", "Lucian", "MissFortune", "Quinn", "Sivir", "Talon",
            "Tristana", "Twitch", "Urgot", "Varus", "Vayne", "Yasuo", "Zed"
        };
        private readonly string[] Bruiser =
        {
            "Aatrox", "Darius", "Elise", "Evelynn", "Fiora", "Gangplank", "Gnar", "Gragas", "Irelia", "JarvanIV", "Jax", "Jayce", "Khazix", "LeeSin",
            "MasterYi", "Nocturne", "Olaf", "Pantheon", "Poppy", "Renekton", "Rengar", "Riven", "Rumble", "Shaco", "Shyvana", "Sion", "Trundle",
            "Tryndamere", "Udyr", "Vi", "MonkeyKing", "XinZhao"
        };

        private static Menu Config;
        private float Range;
        private Obj_AI_Hero Player = ObjectManager.Player, newTarget = null;
        public Obj_AI_Hero Target = null;

        public M_TargetSelector(Menu menu, float range)
        {
            Config = menu;
            var TSMenu = new Menu("目标选择", "TS");
            {
                TSMenu.AddItem(new MenuItem("TS_Mode", "Mode").SetValue(new StringList(new[] { "Slider Priority", "Most AD", "Most AP", "Less Attack", "Less Cast", "Low Hp", "Closest", "Near Mouse" })));
                TSMenu.AddItem(new MenuItem("TS_Focus", "Forced Target").SetValue(true));
                TSMenu.AddItem(new MenuItem("TS_Draw", "Draw Target").SetValue(true));
                TSMenu.AddItem(new MenuItem("TS_Print", "Print Chat New Target").SetValue(true));
                TSMenu.AddItem(new MenuItem("TS_AutoPrior", "Auto Arrange Priorities").SetValue(true)).ValueChanged += PriorityChanger;
                foreach (var Obj in ObjectManager.Get<Obj_AI_Hero>().Where(i => i.IsEnemy))
                {
                    TSMenu.AddItem(new MenuItem("TS_Prior" + Obj.ChampionName, Obj.ChampionName).SetValue(new Slider(TSMenu.Item("TS_AutoPrior").GetValue<bool>() ? GetPriority(Obj.ChampionName) : 1, 1, 5)));
                }
                Config.AddSubMenu(TSMenu);
            }
            Range = range;
            Game.OnGameUpdate += OnGameUpdate;
            Drawing.OnDraw += OnDraw;
            Game.OnWndProc += OnWndProc;
        }

        private void PriorityChanger(object sender, OnValueChangeEventArgs e)
        {
            if (!e.GetNewValue<bool>()) return;
            foreach (var Obj in ObjectManager.Get<Obj_AI_Hero>().Where(i => i.IsEnemy)) Config.SubMenu("TS").Item("TS_Prior" + Obj.ChampionName).SetValue(new Slider(GetPriority(Obj.ChampionName), 1, 5));
        }

        private int GetPriority(string ChampName)
        {
            if (AP.Contains(ChampName)) return 4;
            if (Support.Contains(ChampName)) return 3;
            if (Tank.Contains(ChampName)) return 1;
            if (AD.Contains(ChampName)) return 5;
            if (Bruiser.Contains(ChampName)) return 2;
            return 1;
        }

        private void OnGameUpdate(EventArgs args)
        {
            Target = GetTarget();
            MasterCommon.M_Orbwalker.ForcedTarget = Config.SubMenu("TS").Item("TS_Focus").GetValue<bool>() ? Target : null;
        }

        private void OnDraw(EventArgs args)
        {
            if (Player.IsDead || !Config.SubMenu("TS").Item("TS_Draw").GetValue<bool>() || Target == null) return;
            Utility.DrawCircle(Target.Position, 130, Color.Red);
        }

        private void OnWndProc(WndEventArgs args)
        {
            if (args.WParam != 1 || MenuGUI.IsChatOpen || Player.Spellbook.SelectedSpellSlot != SpellSlot.Unknown) return;
            newTarget = null;
            if (Player.IsDead) return;
            if (Master.Program.IsValid((Obj_AI_Hero)Hud.SelectedUnit, 230, true, Game.CursorPos))
            {
                newTarget = (Obj_AI_Hero)Hud.SelectedUnit;
                if (Config.SubMenu("TS").Item("TS_Print").GetValue<bool>()) Game.PrintChat("<font color = \'{0}'>-></font> New Target: <font color = \'{1}'>{2}</font>", Master.HtmlColor.BlueViolet, Master.HtmlColor.Gold, newTarget.ChampionName);
            }
        }

        private Obj_AI_Hero GetTarget()
        {
            if (Master.Program.IsValid(newTarget, Range)) return newTarget;
            Obj_AI_Hero bestTarget = null;
            if (Config.SubMenu("TS").Item("TS_Mode").GetValue<StringList>().SelectedIndex == 0)
            {
                float bestRatio = 0;
                foreach (var Obj in ObjectManager.Get<Obj_AI_Hero>().Where(i => Master.Program.IsValid(i, Range)))
                {
                    float Prior = 1;
                    switch (Config.SubMenu("TS").Item("TS_Prior" + Obj.ChampionName).GetValue<Slider>().Value)
                    {
                        case 2:
                            Prior = 1.5f;
                            break;
                        case 3:
                            Prior = 1.75f;
                            break;
                        case 4:
                            Prior = 2;
                            break;
                        case 5:
                            Prior = 2.5f;
                            break;
                    }
                    var Ratio = 100 / (1 + Obj.Health) * Prior;
                    if (Ratio > bestRatio)
                    {
                        bestRatio = Ratio;
                        bestTarget = Obj;
                    }
                }
            }
            else
            {
                foreach (var Obj in ObjectManager.Get<Obj_AI_Hero>().Where(i => Master.Program.IsValid(i, Range)))
                {
                    if (bestTarget == null)
                    {
                        bestTarget = Obj;
                    }
                    else
                    {
                        switch (Config.SubMenu("TS").Item("TS_Mode").GetValue<StringList>().SelectedIndex)
                        {
                            case 1:
                                if (Obj.BaseAttackDamage + Obj.FlatPhysicalDamageMod < bestTarget.BaseAttackDamage + bestTarget.FlatPhysicalDamageMod) bestTarget = Obj;
                                break;
                            case 2:
                                if (Obj.FlatMagicDamageMod < bestTarget.FlatMagicDamageMod) bestTarget = Obj;
                                break;
                            case 3:
                                if (Obj.Health - Player.CalcDamage(Obj, Damage.DamageType.Physical, Obj.Health) < bestTarget.Health - Player.CalcDamage(bestTarget, Damage.DamageType.Physical, bestTarget.Health)) bestTarget = Obj;
                                break;
                            case 4:
                                if (Obj.Health - Player.CalcDamage(Obj, Damage.DamageType.Magical, Obj.Health) < bestTarget.Health - Player.CalcDamage(bestTarget, Damage.DamageType.Magical, bestTarget.Health)) bestTarget = Obj;
                                break;
                            case 5:
                                if (Obj.Health < bestTarget.Health) bestTarget = Obj;
                                break;
                            case 6:
                                if (Player.Distance3D(Obj) < Player.Distance3D(bestTarget)) bestTarget = Obj;
                                break;
                            case 7:
                                if (Obj.Position.Distance(Game.CursorPos) + 50 < bestTarget.Position.Distance(Game.CursorPos)) bestTarget = Obj;
                                break;
                        }
                    }
                }
            }
            return bestTarget;
        }
    }
}
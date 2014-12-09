#region
using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using Color = System.Drawing.Color;

#endregion

namespace Leblanc
{
    internal class Program
    {
        public const string ChampionName = "Leblanc";
        public static readonly Obj_AI_Hero Player = ObjectManager.Player;

        private static readonly List<Slide> ExistingSlide = new List<Slide>();
        private static bool leBlancClone;

        public static Orbwalking.Orbwalker Orbwalker;

        public static List<Spell> SpellList = new List<Spell>();

        public static Spell Q, W, E, R;

        public static SpellSlot IgniteSlot = Player.GetSpellSlot("SummonerDot");
        public static Items.Item Fqc = new Items.Item(3092, 750);
        public static Items.Item Dfg = new Items.Item(3128, 750);

        public static Dictionary<HitChance, string> EHitChangeList = new Dictionary<HitChance, string>();
        public static HitChance DefaultEHitChance;

        public static Dictionary<string, string> ComboList = new Dictionary<string, string>();
        public static String DefaultCombo;

        //Menu
        public static Menu Config;
        public static Menu MenuExtras;
        public static Menu TargetSelectorMenu;
        public static Menu MenuPlayOptions;

        private static readonly string[] LeBlancIsWeakAgainst =
        {
            "Galio", "Karma", "Sion", "Annie", "Syndra", "Diana",
            "Aatrox", "Mordekaiser", "Talon", "Morgana"
        };

        private static readonly string[] LeBlancIsStrongAgainst =
        {
            "Velkoz", "Ahri", "Karthus", "Fizz", "Ziggs",
            "Katarina", "Orianna", "Nidalee", "Yasuo", "Akali"
        };

        public static bool LeBlancClone
        {
            get { return leBlancClone; }
            set { leBlancClone = value;}
        }

        private static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }
        private static void Game_OnGameLoad(EventArgs args)
        {
            if (Player.BaseSkinName != ChampionName) return;

            ComboList.Add("Auto", "Auto");
            ComboList.Add("W-R", "W-R");
            ComboList.Add("Q-R", "Q-R");

            EHitChangeList.Add(HitChance.Low, "Low");
            EHitChangeList.Add(HitChance.Medium, "Medium");
            EHitChangeList.Add(HitChance.High, "High");
            EHitChangeList.Add(HitChance.VeryHigh, "Very High");
            EHitChangeList.Add(HitChance.Immobile, "Immobile");

            try
            {
                Q = new Spell(SpellSlot.Q, 720);
                W = new Spell(SpellSlot.W, 600);
                E = new Spell(SpellSlot.E, 900);
                R = new Spell(SpellSlot.R, 720);
                
                Q.SetTargetted(0.5f, 1500f);
                W.SetSkillshot(0.5f, 200f, 1200f, false, SkillshotType.SkillshotCircle);
                E.SetSkillshot(0.25f, 100f, 1750f, true, SkillshotType.SkillshotLine);
                
                SpellList.Add(Q);
                SpellList.Add(W);
                SpellList.Add(E);
                SpellList.Add(R);
            }
            catch (Exception)
            {
                Game.PrintChat("There is a problem about Loading Spell Informations");
                return;
            }

            try
            { 
                Config = new Menu("xQx | 乐芙兰", "Leblanc", true);
                Config.AddSubMenu(new Menu("走砍", "Orbwalking"));
            }
            catch (Exception)
            {
                Game.PrintChat("There is a problem about Creating Config Menu");
                return;
            }

            try
            {
                TargetSelectorMenu = new Menu("目标 选择", "TargetSelector");
                SimpleTs.AddToMenu(TargetSelectorMenu);
                Config.AddSubMenu(TargetSelectorMenu);
            }
            catch (Exception)
            {
                Game.PrintChat("There is a problem about Creating TargetSelectorMenu");
                return;
            }

            Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalking"));

            try
            {
                //Combo menu:
                Config.AddSubMenu(new Menu("连招", "Combo"));
                Config.SubMenu("Combo").AddItem(new MenuItem("ComboUseQ", "使用 Q").SetValue(true));
                Config.SubMenu("Combo").AddItem(new MenuItem("ComboUseW", "使用 W").SetValue(true));
                //Config.SubMenu("Combo").AddItem(new MenuItem("ComboSmartW", "Use Smart W").SetValue(true));
                Config.SubMenu("Combo").AddItem(new MenuItem("ComboUseE", "使用 E").SetValue(true));
                Config.SubMenu("Combo").AddItem(new MenuItem("ComboUseR", "使用 R").SetValue(true));
                Config.SubMenu("Combo").AddSubMenu(new Menu("不使用连招组合", "DontCombo"));
                foreach (var enemy in ObjectManager.Get<Obj_AI_Hero>().Where(enemy => enemy.Team != Player.Team))
                {
                    Config.SubMenu("Combo")
                        .SubMenu("DontCombo")
                        .AddItem(new MenuItem("DontCombo" + enemy.BaseSkinName, enemy.BaseSkinName).SetValue(false));
                }
                Config.SubMenu("Combo")
                    .AddItem(
                        new MenuItem("ComboActive", "连招!").SetValue(new KeyBind("Z".ToCharArray()[0],
                            KeyBindType.Press)));

                /* [ Combo Option ] */
                var menuComboOption = new Menu("连招 设置", "ComboOption", false);
                foreach (KeyValuePair<string, string> combo in ComboList)
                {
                    var langItem = menuComboOption.AddItem(new MenuItem(combo.Key, combo.Key).SetValue(false));

                    KeyValuePair<string, string> combo1 = combo;
                    langItem.ValueChanged += (sender, argsEvent) =>
                    {
                        if (argsEvent.GetNewValue<bool>())
                        {
                            menuComboOption.Items.ForEach(
                                x =>
                                {
                                    if (x.GetValue<bool>() && x.Name != combo1.Key)
                                        x.SetValue(false);
                                });
                            DefaultCombo = combo1.Key;
                            Game.PrintChat(string.Format("|涔愯姍鍏版墽琛岃繛鎷泑: <font color='#FFF9C200'>{0}</font>", combo1.Key));
                        }
                    };
                }

                Config.SubMenu("Combo").AddSubMenu(menuComboOption);

                foreach (var menuItem in from menuItem in menuComboOption.Items
                                         where menuItem.GetValue<bool>()
                                         from combo in ComboList
                                         where menuItem.Name == combo.Key
                                         select menuItem) 
                {
                    DefaultCombo = menuItem.Name;
                }

                if (DefaultCombo == null)
                {
                    DefaultCombo = "W-R";
                    menuComboOption.Item("W-R").SetValue(true);
                }

            }
            catch (Exception)
            {
                Game.PrintChat("There is a problem about Loading Combo Menu");
                return;
            }
            //Harass menu:
            Config.AddSubMenu(new Menu("骚扰", "Harass"));
            Config.SubMenu("Harass").AddItem(new MenuItem("UseQHarass", "使用 Q").SetValue(true));
            Config.SubMenu("Harass").AddItem(new MenuItem("UseWHarass", "使用 W").SetValue(false));
            Config.SubMenu("Harass").AddItem(new MenuItem("UseEHarass", "使用 E").SetValue(false));
            Config.SubMenu("Harass")
                .AddItem(new MenuItem("HarassMana", "骚扰最低蓝量: ").SetValue(new Slider(50, 100, 0)));
            Config.SubMenu("Harass")
                .AddItem(
                    new MenuItem("HarassUseQT", "使用 Q (自动)!").SetValue(new KeyBind("H".ToCharArray()[0],
                        KeyBindType.Toggle)));
            Config.SubMenu("Harass")
                .AddItem(
                    new MenuItem("HarassActive", "骚扰!").SetValue(new KeyBind("C".ToCharArray()[0], KeyBindType.Press)));

            //Farming menu:
            Config.AddSubMenu(new Menu("清线", "LaneClear"));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("UseQLaneClear", "使用 Q").SetValue(false));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("UseWLaneClear", "使用 W").SetValue(false));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("UseELaneClear", "使用 E").SetValue(false));
            Config.SubMenu("LaneClear")
                .AddItem(new MenuItem("LaneClearMana", "清线最低蓝量: ").SetValue(new Slider(50, 100, 0)));
            Config.SubMenu("LaneClear")
                .AddItem(
                    new MenuItem("LaneClearActive", "骚扰!").SetValue(new KeyBind("V".ToCharArray()[0],
                        KeyBindType.Press)));

            //JungleFarm menu:
            Config.AddSubMenu(new Menu("清野", "JungleFarm"));
            Config.SubMenu("JungleFarm").AddItem(new MenuItem("UseQJFarm", "使用 Q").SetValue(true));
            Config.SubMenu("JungleFarm").AddItem(new MenuItem("UseWJFarm", "使用 W").SetValue(true));
            Config.SubMenu("JungleFarm").AddItem(new MenuItem("UseEJFarm", "使用 E").SetValue(true));
            Config.SubMenu("JungleFarm")
                .AddItem(new MenuItem("JungleFarmMana", "清野最低蓝量: ").SetValue(new Slider(50, 100, 0)));
            Config.SubMenu("JungleFarm")
                .AddItem(
                    new MenuItem("JungleFarmActive", "清野!").SetValue(new KeyBind("V".ToCharArray()[0],
                        KeyBindType.Press)));

            try
            {
                MenuPlayOptions = new Menu("玩耍 设置", "PlayOptions");
                Config.AddSubMenu(MenuPlayOptions);

                /* [ Assassin manager ] */
                try
                {
                    new AssassinManager();
                }
                catch (Exception)
                {
                    Game.PrintChat("Something wrong 'Loading Assassing Manager'");
                    return;
                }

                /* [ E HitChance ] */
                var menuEHitChance = new Menu("E 命中率", "EHitChange");
                foreach (KeyValuePair<HitChance, string> eHitChange in EHitChangeList)
                {
                    var langItem = menuEHitChance.AddItem(new MenuItem(eHitChange.Key.ToString(), eHitChange.Value).SetValue(false));

                    KeyValuePair<HitChance, string> eHitChange1 = eHitChange;
                    langItem.ValueChanged += (sender, argsEvent) =>
                    {
                        if (argsEvent.GetNewValue<bool>())
                        {
                            menuEHitChance.Items.ForEach(
                                x =>
                                {
                                    if (x.GetValue<bool>() && x.Name != eHitChange1.Key.ToString())
                                        x.SetValue(false);
                                });
                            DefaultEHitChance = eHitChange1.Key;
                        }
                    };
                }

                MenuPlayOptions.AddSubMenu(menuEHitChance);

                foreach (var menuItem in from menuItem in menuEHitChance.Items
                                         where menuItem.GetValue<bool>()
                                                from eHitChance in EHitChangeList
                                                where menuItem.Name == eHitChance.Key.ToString()
                                         select menuItem)
                {
                    switch (menuItem.Name)
                    {
                        case "低": 
                            DefaultEHitChance = HitChance.Low;
                            break;

                        case "正常":
                            DefaultEHitChance = HitChance.Medium;
                            break;

                        case "高":
                            DefaultEHitChance = HitChance.High;
                            break;

                        case "很高":
                            DefaultEHitChance = HitChance.VeryHigh;
                            break;

                        case "稳定":
                            DefaultEHitChance = HitChance.Immobile;
                            break;

                        default:
                            DefaultEHitChance = HitChance.High;
                            break;
                    }
                }


                /* [ Double Stun ] */
                MenuPlayOptions.AddItem(
                    new MenuItem("OptDoubleStun", "双重 演奏!").SetValue(new KeyBind("T".ToCharArray()[0],
                        KeyBindType.Press)));
            }
            catch (Exception)
            {
                Game.PrintChat("There is a problem about Loading Oplay Options Menu");
                return;
            }


            var menuRun = new Menu("逃跑", "Run");
            menuRun.AddItem(new MenuItem("RunUseW", "使用 W").SetValue(true));
            menuRun.AddItem(new MenuItem("RunUseR", "Use R").SetValue(true));
            menuRun.AddItem(
                new MenuItem("RunActive", "逃跑!").SetValue(new KeyBind("A".ToCharArray()[0], KeyBindType.Press)));
            Config.AddSubMenu(menuRun);

            MenuExtras = new Menu("额外", "Extras");
            Config.AddSubMenu(MenuExtras);
            MenuExtras.AddItem(new MenuItem("InterruptSpells", "中断 法术").SetValue(true));

            //Drawings menu:
            Config.AddSubMenu(new Menu("范围", "Drawings"));
            Config.SubMenu("Drawings")
                .AddItem(new MenuItem("QRange", "Q 范围").SetValue(new Circle(false, Color.Honeydew)));
            Config.SubMenu("Drawings")
                .AddItem(new MenuItem("WRange", "W 范围").SetValue(new Circle(true, Color.Honeydew)));
            Config.SubMenu("Drawings")
                .AddItem(new MenuItem("ERange", "E 范围").SetValue(new Circle(false, Color.Honeydew)));
            Config.SubMenu("Drawings")
                .AddItem(new MenuItem("RRange", "R 范围").SetValue(new Circle(false, Color.Honeydew)));

            Config.SubMenu("Drawings")
                .AddItem(new MenuItem("ActiveERange", "活动 E 范围").SetValue(new Circle(false, Color.GreenYellow)));
            Config.SubMenu("Drawings")
                .AddItem(new MenuItem("WObjPosition", "W 目标. 人名.").SetValue(new Circle(true, Color.GreenYellow)));
            Config.SubMenu("Drawings").AddItem(new MenuItem("WObjTimeTick", "W 目标. 记号").SetValue(true));
            Config.SubMenu("Drawings")
                .AddItem(new MenuItem("WQRange", "W+Q 范围").SetValue(new Circle(false, Color.GreenYellow)));

            new PotionManager();
            Config.AddToMainMenu();

            Game.OnGameUpdate += Game_OnGameUpdate;
            GameObject.OnCreate += GameObject_OnCreate;
            GameObject.OnDelete += GameObject_OnDelete;
            Drawing.OnDraw += Drawing_OnDraw;
            Interrupter.OnPossibleToInterrupt += Interrupter_OnPosibleToInterrupt;
            
            Game.PrintChat(
                String.Format(
                    "<font color='#70DBDB'>xQx |涔愯姍鍏皘</font> <font color='#FFFFFF'>{0}</font> <font color='#70DBDB'>鍔犺級鎴愬姛锛佹饥鍖朾y浜岀嫍锛丵Q缇361630847!</font>",
                    ChampionName));
            Game.PrintChat("__________________________________");

            Game.PrintChat(string.Format("涔愯姍鍏皘榛樿E鍛戒腑鐜噟: <font color='#FFF9C200'>{0}</font>",
                DefaultEHitChance));

            Game.PrintChat(string.Format("涔愯姍鍏皘榛樿杩炴嫑: <font color='#FFF9C200'>{0}</font>", DefaultCombo));
        }

        private static int FindCounterStatusForTarget(string enemyBaseSkinName)
        {
            if (LeBlancIsWeakAgainst.Contains(enemyBaseSkinName))
                return 1;

            if (LeBlancIsStrongAgainst.Contains(enemyBaseSkinName))
                return 2;
            
            return 0;
        }

        private static Obj_AI_Hero EnemyHaveSoulShackle
        {
            get
            {
                return (from hero in ObjectManager.Get<Obj_AI_Hero>().Where(hero => Player.Distance(hero) <= 1100)
                    where hero.IsEnemy
                    from buff in hero.Buffs
                    where buff.Name.Contains("LeblancSoulShackle")
                    select hero).FirstOrDefault();
            }
        }
        private static bool DrawEnemySoulShackle
        {
            get
            {
                return (from hero in ObjectManager.Get<Obj_AI_Hero>().Where(hero => Player.Distance(hero) <= 1100)
                    where hero.IsEnemy
                    from buff in hero.Buffs
                    select (buff.Name.Contains("LeblancSoulShackle"))).FirstOrDefault();
            }
        }

        private static void Interrupter_OnPosibleToInterrupt(Obj_AI_Base unit, InterruptableSpell spell)
        {
            if (!Config.Item("InterruptSpells").GetValue<bool>())
                return;

            var isValidTarget = unit.IsValidTarget(E.Range) && spell.DangerLevel == InterruptableDangerLevel.High;

            if (E.IsReady() && isValidTarget)
            {
                E.CastIfHitchanceEquals(unit, DefaultEHitChance);
            }
            else if (R.IsReady() && Player.Spellbook.GetSpell(SpellSlot.R).Name == "LeblancSoulShackleM" && isValidTarget)
            {
                R.Cast(unit);
            }
        }

        private static void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            leBlancClone = sender.Name.Contains("LeBlanc_MirrorImagePoff.troy");

            if (sender.Name.Contains("displacement_blink_indicator"))
            {
                ExistingSlide.Add(
                    new Slide
                    {
                        Object = sender,
                        NetworkId = sender.NetworkId,
                        Position = sender.Position,
                        ExpireTime = Game.Time + 4
                    });
            }
        }

        private static void GameObject_OnDelete(GameObject sender, EventArgs args)
        {
            if (!sender.Name.Contains("displacement_blink_indicator")) return;
            
            for (var i = 0; i < ExistingSlide.Count; i++)
            {
                if (ExistingSlide[i].NetworkId == sender.NetworkId)
                {
                    ExistingSlide.RemoveAt(i);
                    return;
                }
            }
        }

        public static bool LeBlancStillJumped
        {
            get
            { return !W.IsReady() || Player.Spellbook.GetSpell(SpellSlot.W).Name == "leblancslidereturn";}
        }


        private static void UserSummoners(Obj_AI_Base target)
        {
            if (Dfg.IsReady())
            {
                Dfg.Cast(target);
            }

            if (Fqc.IsReady())
            {
                Fqc.Cast(target.ServerPosition);
            }
          
            if (IgniteSlot != SpellSlot.Unknown &&
                Player.SummonerSpellbook.CanUseSpell(IgniteSlot) == SpellState.Ready)
            {
                if (Player.Distance(target) < 650 && GetComboDamage() >= target.Health)
                {
                    Player.SummonerSpellbook.CastSpell(IgniteSlot, target);
                }
            }
        }

        private static void Combo(Obj_AI_Hero vTarget)
        {
            if (vTarget == null)
                vTarget = SimpleTs.GetTarget(E.Range, SimpleTs.DamageType.Magical);

            if (vTarget == null)
                return;

            var useQ = Config.Item("ComboUseQ").GetValue<bool>();
            var useW = Config.Item("ComboUseW").GetValue<bool>();
            var useE = Config.Item("ComboUseE").GetValue<bool>();
            var useR = Config.Item("ComboUseR").GetValue<bool>();

            var cdQEx = Player.Spellbook.GetSpell(SpellSlot.Q).CooldownExpires;
            var cdWEx = Player.Spellbook.GetSpell(SpellSlot.W).CooldownExpires;

            var cdQ = Game.Time < cdQEx ? cdQEx - Game.Time : 0;
            var cdW = Game.Time < cdWEx ? cdWEx - Game.Time : 0;

            useR = (Config.Item("DontCombo" + vTarget.BaseSkinName) != null &&
                    Config.Item("DontCombo" + vTarget.BaseSkinName).GetValue<bool>() == false) && useR;
            
            if (R.IsReady() || Player.Spellbook.GetSpell(SpellSlot.R).Name == "LeblancSlideM")
            {
                if (!useR) return;

                switch (DefaultCombo)
                {
                    case "Auto":
                        if (W.IsReady() && !LeBlancStillJumped && Player.Distance(vTarget) <= W.Range)
                        { 
                            W.Cast(vTarget.Position);
                            if (Player.Distance(vTarget) <= W.Range)
                                R.Cast(vTarget.Position);
                        } else if (Q.IsReady() && Player.Distance(vTarget) <= Q.Range)
                        {
                            Q.CastOnUnit(vTarget, true);
                            if (Player.Distance(vTarget) <= Q.Range && Player.Spellbook.GetSpell(SpellSlot.R).Name == "LeblancChaosOrbM")
                                R.CastOnUnit(vTarget, true);
                        }
                        break; 
                    case "W-R":
                        if (W.IsReady() && !LeBlancStillJumped && Player.Distance(vTarget) <= W.Range)
                            W.Cast(vTarget.Position);
                        if (Player.Distance(vTarget) <= W.Range)
                            R.Cast(vTarget.Position);
                        break;

                    case "Q-R":
                        if (Q.IsReady() && Player.Distance(vTarget) <= Q.Range)
                            Q.CastOnUnit(vTarget, true);
                        if (Player.Distance(vTarget) <= Q.Range && Player.Spellbook.GetSpell(SpellSlot.R).Name == "LeblancChaosOrbM")
                            R.CastOnUnit(vTarget, true);
                        break;
                }
            }
            else
            {
                if (Q.IsReady() && useQ && Player.Distance(vTarget) <= Q.Range)
                {
                    Q.CastOnUnit(vTarget);
                }

                if (W.IsReady() && useW && Player.Distance(vTarget) <= W.Range)
                {
                    W.Cast(vTarget);
                }

                if (E.IsReady() && useE && Player.Distance(vTarget) <= E.Range)
                {
                    E.CastIfHitchanceEquals(vTarget, DefaultEHitChance);
                }
            }

            UserSummoners(vTarget);
        }

        private static void Harass()
        {
            var qTarget = SimpleTs.GetTarget(Q.Range, SimpleTs.DamageType.Magical);
            var wTarget = SimpleTs.GetTarget(W.Range, SimpleTs.DamageType.Magical);
            var eTarget = SimpleTs.GetTarget(E.Range, SimpleTs.DamageType.Magical);

            var useQ = Config.Item("UseQHarass").GetValue<bool>();
            var useW = Config.Item("UseWHarass").GetValue<bool>();
            var useE = Config.Item("UseEHarass").GetValue<bool>();

            if (useQ && qTarget != null && Q.IsReady()) 
            {
                Q.CastOnUnit(qTarget);
            }
            if (useW && wTarget != null && W.IsReady() && !LeBlancStillJumped)
            {
                W.Cast(wTarget);
            }
            if (useE && eTarget != null && E.IsReady())
            {
                E.CastIfHitchanceEquals(eTarget, DefaultEHitChance);
            }
        }

        private static float GetComboDamage()
        {
            var fComboDamage = 0d;

            var qTarget = SimpleTs.GetTarget(Q.Range, SimpleTs.DamageType.Magical);
            var wTarget = SimpleTs.GetTarget(W.Range, SimpleTs.DamageType.Magical);
            var eTarget = SimpleTs.GetTarget(E.Range, SimpleTs.DamageType.Magical);

            if (Q.IsReady() && qTarget != null)
                fComboDamage += Player.GetSpellDamage(qTarget, SpellSlot.Q);

            if (W.IsReady() && wTarget != null)
                fComboDamage += Player.GetSpellDamage(wTarget, SpellSlot.W);

            if (E.IsReady() && eTarget != null)
                fComboDamage += Player.GetSpellDamage(eTarget, SpellSlot.E);

/*            if (RQ.IsReady() || RW.IsReady() || RE.IsReady())
                fComboDamage += Player.GetSpellDamage(qTarget, SpellSlot.R);
*/
            if (IgniteSlot != SpellSlot.Unknown && Player.SummonerSpellbook.CanUseSpell(IgniteSlot) == SpellState.Ready)
                fComboDamage += Player.GetSummonerSpellDamage(wTarget, Damage.SummonerSpell.Ignite);

            if (Items.CanUseItem(3128))
                fComboDamage += Player.GetItemDamage(wTarget, Damage.DamageItems.Dfg); 

            if (Items.CanUseItem(3092))
                fComboDamage += Player.GetItemDamage(wTarget, Damage.DamageItems.FrostQueenClaim);

            return (float)fComboDamage;
        }
        private static bool xEnemyHaveSoulShackle(Obj_AI_Hero vTarget)
        {
            return (vTarget.HasBuff("LeblancSoulShackle"));
        }

        private static void Run()
        {
            ObjectManager.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);

            var useW = Config.Item("RunUseW").GetValue<bool>();
            var useR = Config.Item("RunUseR").GetValue<bool>();

            if (useW && W.IsReady() && !LeBlancStillJumped)
            {
                W.Cast(Game.CursorPos);
            }

            if (useR && R.IsReady() && Player.Spellbook.GetSpell(SpellSlot.R).Name == "LeblancSlideM") 
            {
                R.Cast(Game.CursorPos);
            }
        }

        private static void DoubleStun()
        {
            ObjectManager.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            
            if (Config.Item("OptDoubleStun").GetValue<KeyBind>().Active)
            {
                if (Q.IsReady())
                    Config.Item("HarassUseQT").SetValue(false);

                Drawing.DrawText(Drawing.Width * 0.45f, Drawing.Height * 0.80f, Color.GreenYellow, 
                        "Double Stun Active!");

                /*
                var onPlayerPositionEnemyCount2 =
                    (from enemy in
                        ObjectManager.Get<Obj_AI_Hero>()
                            .Where(enemy => enemy.Team != Player.Team && Player.Distance(enemy) < E.Range + 200)
                        select enemy).Count();

                if (onPlayerPositionEnemyCount2 >= 2)
                {
                */
                foreach (
                    var enemy in
                        ObjectManager.Get<Obj_AI_Hero>()
                            .Where(
                                enemy =>
                                    enemy.IsEnemy && !enemy.IsDead && enemy.IsVisible && Player.Distance(enemy) < E.Range + 200 &&
                                    !xEnemyHaveSoulShackle(enemy))) 
                    {
                        //foreach (var buff in enemy.Buffs)
                       // {
                            //if (buff.Name.Contains("LeblancSoulShackle"))
                            //    Game.PrintChat(enemy.ChampionName);
                        //}

                        //Utility.DrawCircle(enemy.Position, 75f, Color.GreenYellow);

                        if (E.IsReady() && Player.Distance(enemy) < E.Range)
                        {
                            E.CastIfHitchanceEquals(enemy, DefaultEHitChance);
                        }
                        else
                        if (R.IsReady() && Player.Distance(enemy) < E.Range &&
                            Player.Spellbook.GetSpell(SpellSlot.R).Name == "LeblancSoulShackleM")
                        {
                            R.CastIfHitchanceEquals(enemy, DefaultEHitChance);
                        }
               /*}*/
                }
            }


        }

        private static void RefresySpellR()
        {
            /*
            var rMode = Player.Spellbook.GetSpell(SpellSlot.R).Name;

            switch (rMode)
            {
                case "LeblancChaosOrbM":
                    {
                        R.Range = Q.Range;
                        R.SetTargetted(0.5f, float.MaxValue);
                        break;
                    }
                case "LeblancSlideM":
                    {
                        R.Range = W.Range;
                        R.SetSkillshot(0.5f, 200f, float.MaxValue, false, SkillshotType.SkillshotCircle);
                        break;
                    }
                case "LeblancSoulShackleM":
                    {
                        R.Range = E.Range;
                        R.SetSkillshot(0.5f, 100f, 1000f, true, SkillshotType.SkillshotLine);
                        break;
                    }
            }
            */
        }

        private static void SmartW()
        {
            /*
            if (!Config.Item("ComboSmartW").GetValue<bool>())
                return;

            var vTarget = EnemyHaveSoulShackle;
            foreach (var existingSlide in ExistingSlide)
            {
                var slide = existingSlide;

                var onSlidePositionEnemyCount = (from enemy in
                    ObjectManager.Get<Obj_AI_Hero>()
                        .Where(
                            enemy => enemy.Team != Player.Team && enemy.Distance(slide.Position) < 350f)
                    select enemy).Count();

                var onPlayerPositionEnemyCount = (from enemy in
                    ObjectManager.Get<Obj_AI_Hero>()
                        .Where(
                            enemy => enemy.Team != Player.Team && Player.Distance(enemy) < Q.Range)
                    select enemy).Count();


                if (Config.Item("OptDoubleStun").GetValue<KeyBind>().Active && E.IsReady() && R.IsReady())
                {
                    var onPlayerPositionEnemyCount2 = (from enemy in
                        ObjectManager.Get<Obj_AI_Hero>()
                            .Where(
                                enemy => enemy.Team != Player.Team && Player.Distance(enemy) < E.Range)
                        select enemy).Count();

                    if (onPlayerPositionEnemyCount2 == 2)
                    {

                    }
                }
                if (onPlayerPositionEnemyCount > onSlidePositionEnemyCount)
                {
                    if (LeBlancStillJumped)
                    {
                        var qTarget = SimpleTs.GetTarget(Q.Range, SimpleTs.DamageType.Magical);
                        if (qTarget == null)
                            return;
                        if ((Player.Health < qTarget.Health || Player.Level < qTarget.Level) &&
                            vTarget.Health > GetComboDamage())
                            W.Cast();
                        else
                        {
                            if (Q.IsReady())
                                Q.CastOnUnit(qTarget);
                            if (RQ.IsReady())
                                RQ.CastOnUnit(qTarget);
                            if (E.IsReady())
                                E.Cast(qTarget);
                            W.Cast();
                        }
                    }

                }
                Game.PrintChat(slide.Position.ToString());
                Utility.DrawCircle(slide.Position, 400f, Color.Red);

                Game.PrintChat("Slide Pos. Enemy Count: " + onSlidePositionEnemyCount);
                Game.PrintChat("Player Pos. Enemy Count: " + onPlayerPositionEnemyCount);


                Game.PrintChat("W Posision : " + existingSlide.Position);
                Game.PrintChat("Target Position : " + vTarget.Position);
            }
            */
        }


        private static void LaneClear()
        {
            if (!Orbwalking.CanMove(40)) return;

            var useQ = Config.Item("UseQLaneClear").GetValue<bool>();
            var useW = Config.Item("UseWLaneClear").GetValue<bool>();

            if (useQ && Q.IsReady())
            {
                var minionsQ = MinionManager.GetMinions(Player.ServerPosition, Q.Range, MinionTypes.All,
                    MinionTeam.NotAlly);
                foreach (Obj_AI_Base vMinion in 
                    from vMinion in minionsQ let vMinionEDamage = Player.GetSpellDamage(vMinion, SpellSlot.Q)
                        where vMinion.Health <= vMinionEDamage && vMinion.Health > Player.GetAutoAttackDamage(vMinion)
                            select vMinion)
                {
                    
                    Q.CastOnUnit(vMinion);
                }
            }

            var rangedMinionsW = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, W.Range + W.Width + 20);
            if (!useW || !W.IsReady()) return;
            
            var minionsW = W.GetCircularFarmLocation(rangedMinionsW, W.Width * 0.75f);
            
            if (minionsW.MinionsHit < 2 || !W.InRange(minionsW.Position.To3D())) 
                return;
            
            W.Cast(minionsW.Position);

        }

        private static void JungleFarm()
        {
            var useQ = Config.Item("UseQJFarm").GetValue<bool>();
            var useW = Config.Item("UseWJFarm").GetValue<bool>();
            var useE = Config.Item("UseEJFarm").GetValue<bool>();

            var mobs = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q.Range, MinionTypes.All,
                MinionTeam.Neutral, MinionOrderTypes.MaxHealth);

            if (mobs.Count <= 0) return;
            var mob = mobs[0];
            if (useQ && Q.IsReady())
                Q.CastOnUnit(mob);

            if (useW && W.IsReady())
                W.Cast(mob.Position);

            if (useE && E.IsReady())
                E.Cast(mob);
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (Player.IsDead) return;
            
            RefresySpellR();
            //Mode();
//            if (Config.Item("ComboSmartW").GetValue<KeyBind>().Active)
//                SmartW();

            Orbwalker.SetAttack(true);

            if (Config.Item("OptDoubleStun").GetValue<KeyBind>().Active)
            {
                DoubleStun();
            }

            if (Config.Item("RunActive").GetValue<KeyBind>().Active)
            {
                Run();
            }

            if (Config.Item("ComboActive").GetValue<KeyBind>().Active)
            {
                var assassinRange = MenuPlayOptions.Item("AssassinRange").GetValue<Slider>().Value;
                Obj_AI_Hero vTarget = null;
                foreach (var enemy in ObjectManager.Get<Obj_AI_Hero>()
                    .Where(enemy => enemy.Team != Player.Team
                        && !enemy.IsDead && enemy.IsVisible
                        && MenuPlayOptions.Item("Assassin" + enemy.ChampionName) != null
                        && MenuPlayOptions.Item("Assassin" + enemy.ChampionName).GetValue<bool>())
                        .OrderBy(enemy => enemy.Distance(Game.CursorPos))
                        )
                {

                    vTarget = Player.Distance(enemy) < assassinRange ? enemy : null;

                }
                Combo(vTarget);
            }
            else
            {
                if (Config.Item("HarassUseQT").GetValue<KeyBind>().Active)
                {
                    var t = SimpleTs.GetTarget(Q.Range, SimpleTs.DamageType.Magical);
                    if (t != null && Q.IsReady())
                        Q.CastOnUnit(t);
                }

                if (Config.Item("HarassActive").GetValue<KeyBind>().Active)
                {
                    var existsMana = Player.MaxMana/100*Config.Item("HarassMana").GetValue<Slider>().Value;
                    if (Player.Mana >= existsMana)
                        Harass();
                }

                if (Config.Item("LaneClearActive").GetValue<KeyBind>().Active)
                {
                    var existsMana = Player.MaxMana/100*Config.Item("LaneClearMana").GetValue<Slider>().Value;
                    if (Player.Mana >= existsMana)
                        LaneClear();
                }

                if (Config.Item("JungleFarmActive").GetValue<KeyBind>().Active)
                {
                    var existsMana = Player.MaxMana/100*Config.Item("JungleFarmMana").GetValue<Slider>().Value;
                    if (Player.Mana >= existsMana)
                        JungleFarm();                    
                }

            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            foreach (var spell in SpellList)
            {
                var menuItem = Config.Item(spell.Slot + "Range").GetValue<Circle>();
                if (menuItem.Active && spell.Level > 0)
                    Utility.DrawCircle(Player.Position, spell.Range, menuItem.Color, 1, 15);
            }

            var wObjPosition = Config.Item("WObjPosition").GetValue<Circle>();
            var wObjTimeTick = Config.Item("WObjTimeTick").GetValue<bool>();


            var wqRange = Config.Item("WQRange").GetValue<Circle>();
            if (wqRange.Active && Q.IsReady() && W.IsReady())
            {
                Utility.DrawCircle(Player.Position, W.Range + Q.Range, wqRange.Color, 1, 15);
            }
            
            var ActiveERange = Config.Item("ActiveERange").GetValue<Circle>();
            if (ActiveERange.Active && EnemyHaveSoulShackle != null)
            {
                Utility.DrawCircle(Player.Position, 1100f, ActiveERange.Color, 1, 15);
            }

            foreach (var existingSlide in ExistingSlide)
            {
                if (wObjPosition.Active)
                    Utility.DrawCircle(existingSlide.Position, 110f, wObjPosition.Color, 1, 15);

                if (!wObjTimeTick) continue;
                if (!(existingSlide.ExpireTime > Game.Time)) continue;

                var time = TimeSpan.FromSeconds(existingSlide.ExpireTime - Game.Time);

                var pos = Drawing.WorldToScreen(existingSlide.Position);
                var display = string.Format("{0}:{1:D2}", time.Minutes, time.Seconds);
                Drawing.DrawText(pos.X - display.Length * 3, pos.Y - 65, Color.GreenYellow, display);
            }

            foreach (
                var enemy in
                    ObjectManager.Get<Obj_AI_Hero>()
                        .Where(
                            enemy =>
                                enemy.IsEnemy && !enemy.IsDead && enemy.IsVisible && Player.Distance(enemy) < E.Range + 1400 &&
                                !xEnemyHaveSoulShackle(enemy)))
            {
                

                Utility.DrawCircle(enemy.Position, 75f, Color.GreenYellow, 1, 10);

            }
        }
    }
}

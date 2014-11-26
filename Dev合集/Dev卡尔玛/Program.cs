using DevCommom;
using LeagueSharp;
using LeagueSharp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


/*
 * ##### DevKarma Mods #####
 * 
 * + Logic with R+Q/R+W/R+E
 * + Priorize Heal with R+W when LowHealth (with Slider)
 * + Skin Hack
 * + Shield/Heal Allies
 * + Auto Spell Level UP
 * 
*/

namespace DevKarma
{
    public class Program
    {
        public const string ChampionName = "karma";

        public static Menu Config;
        public static Orbwalking.Orbwalker Orbwalker;
        public static List<Spell> SpellList = new List<Spell>();
        public static Obj_AI_Hero Player;
        public static Spell Q;
        public static Spell W;
        public static Spell E;
        public static Spell R;
        public static SkinManager SkinManager;
        public static IgniteManager IgniteManager;
        public static BarrierManager BarrierManager;
        public static AssemblyUtil assemblyUtil;
        public static LevelUpManager levelUpManager;

        private static bool mustDebug = false;

        static void Main(string[] args)
        {
            LeagueSharp.Common.CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        static void Game_OnGameLoad(EventArgs args)
        {
            Player = ObjectManager.Player;

            if (!Player.ChampionName.ToLower().Contains(ChampionName))
                return;

            try
            {
                InitializeSpells();

                InitializeSkinManager();

                InitializeLevelUpManager();

                InitializeMainMenu();

                InitializeAttachEvents();

                Game.PrintChat(string.Format("<font color='#fb762d'>DevKarma Loaded v{0}姹夊寲by浜岀嫍!QQ缇361630847</font>", Assembly.GetExecutingAssembly().GetName().Version));

                assemblyUtil = new AssemblyUtil(Assembly.GetExecutingAssembly().GetName().Name);
                assemblyUtil.onGetVersionCompleted += AssemblyUtil_onGetVersionCompleted;
                assemblyUtil.GetLastVersionAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        static void AssemblyUtil_onGetVersionCompleted(OnGetVersionCompletedArgs args)
        {
            if (args.LastAssemblyVersion == Assembly.GetExecutingAssembly().GetName().Version.ToString())
                Game.PrintChat(string.Format("<font color='#fb762d'>DevKarma You have the lastest version.</font>"));
            else
                Game.PrintChat(string.Format("<font color='#fb762d'>DevKarma NEW VERSION available! Tap F8 for Update! {0}</font>", args.LastAssemblyVersion));
        }

        private static void InitializeAttachEvents()
        {
            Game.OnGameUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Interrupter.OnPossibleToInterrupt += Interrupter_OnPossibleToInterrupt;
            Orbwalking.BeforeAttack += Orbwalking_BeforeAttack;

            Config.Item("ComboDamage").ValueChanged += (object sender, OnValueChangeEventArgs e) => { Utility.HpBarDamageIndicator.Enabled = e.GetNewValue<bool>(); };
            if (Config.Item("ComboDamage").GetValue<bool>())
            {
                Utility.HpBarDamageIndicator.DamageToUnit = GetComboDamage;
                Utility.HpBarDamageIndicator.Enabled = true;
            }
        }


        static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            var packetCast = Config.Item("PacketCast").GetValue<bool>();
            var BarrierGapCloser = Config.Item("BarrierGapCloser").GetValue<bool>();
            var BarrierGapCloserMinHealth = Config.Item("BarrierGapCloserMinHealth").GetValue<Slider>().Value;
            var EGapCloser = Config.Item("EGapCloser").GetValue<bool>();

            if (BarrierGapCloser && gapcloser.Sender.IsValidTarget(Player.AttackRange) && Player.GetHealthPerc() < BarrierGapCloserMinHealth)
            {
                if (BarrierManager.Cast())
                    Game.PrintChat(string.Format("OnEnemyGapcloser -> BarrierGapCloser on {0} !", gapcloser.Sender.SkinName));
            }

            if (EGapCloser && E.IsReady())
            {
                if (R.IsReady())
                    R.Cast();

                E.Cast();
            }
        }

        static void Interrupter_OnPossibleToInterrupt(Obj_AI_Base unit, InterruptableSpell spell)
        {

        }

        static void Game_OnGameUpdate(EventArgs args)
        {
            try
            { 
                switch (Orbwalker.ActiveMode)
                {
                    case Orbwalking.OrbwalkingMode.Combo:
                        Combo();
                        HelpAlly();
                        break;
                    case Orbwalking.OrbwalkingMode.Mixed:
                        Harass();
                        HelpAlly();
                        break;
                    case Orbwalking.OrbwalkingMode.LaneClear:
                        //WaveClear();
                        break;
                    case Orbwalking.OrbwalkingMode.LastHit:
                        //Freeze();
                        break;
                    default:
                        break;
                }

                SkinManager.Update();

                levelUpManager.Update();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }


        public static void HelpAlly()
        {
            var HelpAlly = Config.Item("HelpAlly").GetValue<bool>();
            var packetCast = Config.Item("PacketCast").GetValue<bool>();
            var UseEHelpAlly = Config.Item("UseEHelpAlly").GetValue<bool>();
            var AllyMinHealth = Config.Item("AllyMinHealth").GetValue<Slider>().Value;

            if (HelpAlly)
            {
                var AllyList = DevHelper.GetAllyList().Where(x => Player.Distance(x.ServerPosition) < E.Range && x.GetHealthPerc() < AllyMinHealth && DevHelper.CountEnemyInPositionRange(x.ServerPosition, x.AttackRange) > 0).OrderBy(x => x.Health);
                if (AllyList.Any())
                {
                    var ally = AllyList.First();

                    if (R.IsReady())
                        R.Cast();

                    if (UseEHelpAlly && E.IsReady())
                        E.CastOnUnit(ally, packetCast);
                }
            }
        }


        public static void Combo()
        {
            var eTarget = SimpleTs.GetTarget(Q.Range, SimpleTs.DamageType.Magical);

            if (eTarget == null)
                return;

            var useQ = Config.Item("UseQCombo").GetValue<bool>();
            var useW = Config.Item("UseWCombo").GetValue<bool>();
            var useE = Config.Item("UseECombo").GetValue<bool>();
            var useR = Config.Item("UseRCombo").GetValue<bool>();
            var packetCast = Config.Item("PacketCast").GetValue<bool>();

            var UseWComboHeal = Config.Item("UseWComboHeal").GetValue<bool>();
            var UseWHealMinHealth = Config.Item("UseWHealMinHealth").GetValue<Slider>().Value;

            if (eTarget.IsValidTarget(Q.Range) && Q.IsReady() && useQ)
            {
                var pred = Q.GetPrediction(eTarget);
                if (pred.Hitchance >= (eTarget.IsMoving ? HitChance.High : HitChance.Medium))
                {
                    if (useR && R.IsReady())
                    {
                        if (!(UseWComboHeal && Player.GetHealthPerc() < UseWHealMinHealth))
                            R.Cast();
                    }
                    Q.Cast(pred.CastPosition, packetCast);
                }
            }

            if (eTarget.IsValidTarget(W.Range) && W.IsReady() && useW)
            {
                if (useR && R.IsReady() && UseWComboHeal && Player.GetHealthPerc() < UseWHealMinHealth)
                    R.Cast();

                W.CastOnUnit(eTarget, packetCast);
            }

            if (eTarget.IsValidTarget(E.Range) && E.IsReady() && useE)
            {
                E.CastOnUnit(eTarget, packetCast);
            }

            if (IgniteManager.CanKill(eTarget))
            {
                if (IgniteManager.Cast(eTarget))
                    Game.PrintChat(string.Format("Ignite Combo KS -> {0} ", eTarget.SkinName));
            }
        }

        public static void Harass()
        {
            var eTarget = SimpleTs.GetTarget(Q.Range, SimpleTs.DamageType.Magical);

            if (eTarget == null)
                return;

            var useQ = Config.Item("UseQHarass").GetValue<bool>();
            var useW = Config.Item("UseWHarass").GetValue<bool>();
            var useE = Config.Item("UseEHarass").GetValue<bool>();
            var useR = Config.Item("UseRCombo").GetValue<bool>();
            var packetCast = Config.Item("PacketCast").GetValue<bool>();

            var UseWHarassHeal = Config.Item("UseWHarassHeal").GetValue<bool>();
            var UseWHealMinHealthHarass = Config.Item("UseWHealMinHealthHarass").GetValue<Slider>().Value;

            if (eTarget.IsValidTarget(Q.Range) && Q.IsReady() && useQ)
            {
                var pred = Q.GetPrediction(eTarget);
                if (pred.Hitchance >= (eTarget.IsMoving ? HitChance.High : HitChance.Medium))
                {
                    if (useR && R.IsReady())
                    {
                        if (!(UseWHarassHeal && Player.GetHealthPerc() < UseWHealMinHealthHarass))
                            R.Cast();
                    }
                    Q.Cast(pred.CastPosition, packetCast);
                }
            }

            if (eTarget.IsValidTarget(W.Range) && W.IsReady() && useW)
            {
                if (useR && R.IsReady() && UseWHarassHeal && Player.GetHealthPerc() < UseWHealMinHealthHarass)
                    R.Cast();

                W.CastOnUnit(eTarget, packetCast);
            }

            if (eTarget.IsValidTarget(E.Range) && E.IsReady() && useE)
            {
                E.CastOnUnit(eTarget, packetCast);
            }
        }

        private static void InitializeSkinManager()
        {
            SkinManager = new SkinManager();
            SkinManager.Add("Classic Karma");
            SkinManager.Add("Sun Goddess Karma");
            SkinManager.Add("Sakura Karma");
            SkinManager.Add("Traditional Karma");
            SkinManager.Add("Order of the Lotus Karma");
        }

        private static void InitializeLevelUpManager()
        {
            if (mustDebug)
                Game.PrintChat("InitializeLevelUpManager Start");

            var priority1 = new int[] { 1, 3, 1, 2, 1, 4, 1, 3, 1, 3, 4, 3, 3, 2, 2, 4, 2, 2 };

            levelUpManager = new LevelUpManager();
            levelUpManager.Add("Q > E > Q > W ", priority1);

            if (mustDebug)
                Game.PrintChat("InitializeLevelUpManager Finish");
        }

        private static void InitializeSpells()
        {
            IgniteManager = new IgniteManager();
            BarrierManager = new BarrierManager();

            Q = new Spell(SpellSlot.Q, 1000);
            Q.SetSkillshot(0.25f, 60, 1700, true, SkillshotType.SkillshotLine);

            W = new Spell(SpellSlot.W, 650);
            W.SetTargetted(0.1f, float.MaxValue);

            E = new Spell(SpellSlot.E, 800);
            E.SetTargetted(0.1f, float.MaxValue);

            R = new Spell(SpellSlot.R);

            SpellList.Add(Q);
            SpellList.Add(W);
            SpellList.Add(E);
            SpellList.Add(R);
        }

        static void Orbwalking_BeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            var allyADC = Player.GetNearestAlly();

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed && args.Unit.IsMinion && !allyADC.IsMelee() && allyADC.Distance(args.Unit) < allyADC.AttackRange * 1.2)
                args.Process = false;
        }

        static void Drawing_OnDraw(EventArgs args)
        {
            foreach (var spell in SpellList)
            {
                var menuItem = Config.Item(spell.Slot + "Range").GetValue<Circle>();
                if (menuItem.Active && spell.IsReady())
                {
                    Utility.DrawCircle(ObjectManager.Player.Position, spell.Range, menuItem.Color);
                }
            }
        }

        private static float GetComboDamage(Obj_AI_Hero enemy)
        {
            IEnumerable<SpellSlot> spellCombo = new[] { SpellSlot.Q, SpellSlot.W, SpellSlot.E, SpellSlot.R };
            return (float)Damage.GetComboDamage(Player, enemy, spellCombo);
        }

        private static void InitializeMainMenu()
        {
            Config = new Menu("Dev鍗″皵鐜泑", "DevKarma", true);

            var targetSelectorMenu = new Menu("鐩爣閫夋嫨", "Target Selector");
            SimpleTs.AddToMenu(targetSelectorMenu);
            Config.AddSubMenu(targetSelectorMenu);

            Config.AddSubMenu(new Menu("璧扮爫", "Orbwalking"));
            Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalking"));

            Config.AddSubMenu(new Menu("杩炴嫑", "Combo"));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseQCombo", "浣跨敤 Q").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseWCombo", "浣跨敤 W").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseECombo", "浣跨敤 E").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseRCombo", "浣跨敤 R").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseWComboHeal", "浣跨敤 W 娌绘剤").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseWHealMinHealth", "浣跨敤W鏈€浣庤閲弢").SetValue(new Slider(40, 1, 100)));

            Config.AddSubMenu(new Menu("楠氭壈", "Harass"));
            Config.SubMenu("Harass").AddItem(new MenuItem("UseQHarass", "浣跨敤 Q").SetValue(true));
            Config.SubMenu("Harass").AddItem(new MenuItem("UseWHarass", "浣跨敤 W").SetValue(true));
            Config.SubMenu("Harass").AddItem(new MenuItem("UseEHarass", "浣跨敤 E").SetValue(true));
            Config.SubMenu("Harass").AddItem(new MenuItem("UseRHarass", "浣跨敤 R").SetValue(false));
            Config.SubMenu("Harass").AddItem(new MenuItem("UseWHarassHeal", "浣跨敤 W 娌绘剤").SetValue(true));
            Config.SubMenu("Harass").AddItem(new MenuItem("UseWHealMinHealthHarass", "浣跨敤W鏈€浣庤閲弢").SetValue(new Slider(50, 1, 100)));

            //Config.AddSubMenu(new Menu("娓呯嚎", "LaneClear"));
            //Config.SubMenu("LaneClear").AddItem(new MenuItem("UseQLaneClear", "浣跨敤 Q").SetValue(true));
            //Config.SubMenu("LaneClear").AddItem(new MenuItem("UseWLaneClear", "浣跨敤 W").SetValue(false));
            //Config.SubMenu("LaneClear").AddItem(new MenuItem("UseELaneClear", "浣跨敤 E").SetValue(true));
            //Config.SubMenu("LaneClear").AddItem(new MenuItem("ManaLaneClear", "Min Mana LaneClear").SetValue(new Slider(25, 1, 100)));

            //Config.AddSubMenu(new Menu("鎺х嚎", "Freeze"));
            //Config.SubMenu("Freeze").AddItem(new MenuItem("UseQFreeze", "浣跨敤 Q 琛ュ垁").SetValue(true));
            //Config.SubMenu("Freeze").AddItem(new MenuItem("ManaFreeze", "浣跨敤Q鏈€浣庤閲弢").SetValue(new Slider(25, 1, 100)));

            Config.AddSubMenu(new Menu("R璁剧疆", "Ultimate"));
            Config.SubMenu("Ultimate").AddItem(new MenuItem("UseRAlly", "浣跨敤R鎻村姪闃熷弸").SetValue(true));
            Config.SubMenu("Ultimate").AddItem(new MenuItem("UseRAllyMinHealth", "浣跨敤R闃熷弸琛€閲弢").SetValue(new Slider(30, 1, 100)));

            Config.AddSubMenu(new Menu("鍗忓姪闃熷弸", "HelpAlly"));
            Config.SubMenu("HelpAlly").AddItem(new MenuItem("HelpAlly", "鍗忓姪闃熷弸").SetValue(true));
            Config.SubMenu("HelpAlly").AddItem(new MenuItem("UseEHelpAlly", "浣跨敤 E 鍗忓姪闃熷弸").SetValue(true));
            Config.SubMenu("HelpAlly").AddItem(new MenuItem("AllyMinHealth", "鍗忓姪闃熷弸鏈€浣庤閲弢").SetValue(new Slider(50, 1, 100)));

            Config.AddSubMenu(new Menu("鏉傞」", "Misc"));
            Config.SubMenu("Misc").AddItem(new MenuItem("PacketCast", "浣跨敤灏佸寘").SetValue(true));

            Config.AddSubMenu(new Menu("绐佽繘缁堢粨鑰厊", "GapCloser"));
            Config.SubMenu("GapCloser").AddItem(new MenuItem("BarrierGapCloser", "闃绘柇绐佽繘鑰厊").SetValue(true));
            Config.SubMenu("GapCloser").AddItem(new MenuItem("BarrierGapCloserMinHealth", "闃绘柇绐佽繘鑰厊鏈€灏戣閲弢").SetValue(new Slider(40, 0, 100)));
            Config.SubMenu("GapCloser").AddItem(new MenuItem("EGapCloser", "瀵圭獊杩涜€呬娇鐢‥").SetValue(true));

            Config.AddSubMenu(new Menu("鑼冨洿", "Drawings"));
            Config.SubMenu("Drawings").AddItem(new MenuItem("QRange", "Q 鑼冨洿").SetValue(new Circle(true, System.Drawing.Color.FromArgb(255, 255, 255, 255))));
            Config.SubMenu("Drawings").AddItem(new MenuItem("WRange", "W 鑼冨洿").SetValue(new Circle(false, System.Drawing.Color.FromArgb(255, 255, 255, 255))));
            Config.SubMenu("Drawings").AddItem(new MenuItem("ERange", "E 鑼冨洿").SetValue(new Circle(false, System.Drawing.Color.FromArgb(255, 255, 255, 255))));
            Config.SubMenu("Drawings").AddItem(new MenuItem("RRange", "R 鑼冨洿").SetValue(new Circle(false, System.Drawing.Color.FromArgb(255, 255, 255, 255))));
            Config.SubMenu("Drawings").AddItem(new MenuItem("ComboDamage", "鏄剧ず缁勫悎杩炴嫑浼ゅ").SetValue(true));

            SkinManager.AddToMenu(ref Config);

            levelUpManager.AddToMenu(ref Config);

            Config.AddToMainMenu();
        }
    }
}

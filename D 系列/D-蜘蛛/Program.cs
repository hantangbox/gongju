using System;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using LeagueSharp;
using LeagueSharp.Common;
using Color = System.Drawing.Color;
namespace D_Elise
{
    class Program
    {
        private const string ChampionName = "Elise";

        private static Orbwalking.Orbwalker _orbwalker;

        private static bool _human;

        private static bool _spider;

        private static Spell _humanQ, _humanW, _humanE, _r, _spiderQ, _spiderW, _spiderE;

        private static Menu _config;

        private static SpellSlot _igniteSlot;

        private static Obj_AI_Hero _player;

        private static readonly float[] HumanQcd = { 6, 6, 6, 6, 6 };

        private static readonly float[] HumanWcd = { 12, 12, 12, 12, 12 };

        private static readonly float[] HumanEcd = { 14, 13, 12, 11, 10 };

        private static readonly float[] SpiderQcd = { 6, 6, 6, 6, 6 };

        private static readonly float[] SpiderWcd = { 12, 12, 12, 12, 12 };

        private static readonly float[] SpiderEcd = { 26, 23, 20, 17, 14 };

        private static float _humQcd = 0, _humWcd = 0, _humEcd = 0;

        private static float _spidQcd = 0, _spidWcd = 0, _spidEcd = 0;

        private static float _humaQcd = 0, _humaWcd = 0, _humaEcd = 0;

        private static float _spideQcd = 0, _spideWcd = 0, _spideEcd = 0;

       private static Items.Item _tiamat, _hydra, _blade, _bilge, _rand, _lotis, _zhonya;

       private static SpellSlot _smiteSlot = SpellSlot.Unknown;

       private static Spell _smite;
       //Credits to Kurisu
       private static readonly int[] SmitePurple = { 3713, 3726, 3725, 3726, 3723 };
       private static readonly int[] SmiteGrey = { 3711, 3722, 3721, 3720, 3719 };
       private static readonly int[] SmiteRed = { 3715, 3718, 3717, 3716, 3714 };
       private static readonly int[] SmiteBlue = { 3706, 3710, 3709, 3708, 3707 };

        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        private static void Game_OnGameLoad(EventArgs args)
        {

            _player = ObjectManager.Player;
            if (_player.BaseSkinName != ChampionName) return;

            _humanQ = new Spell(SpellSlot.Q, 625f);
            _humanW = new Spell(SpellSlot.W, 950f);
            _humanE = new Spell(SpellSlot.E, 1075f);
            _spiderQ = new Spell(SpellSlot.Q, 475f);
            _spiderW = new Spell(SpellSlot.W, 0);
            _spiderE = new Spell(SpellSlot.E, 750f);
            _r = new Spell(SpellSlot.R, 0);

            _humanW.SetSkillshot(0.25f, 100f, 1000, true, SkillshotType.SkillshotLine);
            _humanE.SetSkillshot(0.25f, 55f, 1300, true, SkillshotType.SkillshotLine);

            _bilge = new Items.Item(3144, 475f);
            _blade = new Items.Item(3153, 425f);
            _hydra = new Items.Item(3074, 250f);
            _tiamat = new Items.Item(3077, 250f);
            _rand = new Items.Item(3143, 490f);
            _lotis = new Items.Item(3190, 590f);
            _zhonya = new Items.Item(3157, 10);

            SetSmiteSlot();
            _igniteSlot = _player.GetSpellSlot("SummonerDot");
           
            _config = new Menu("D-蜘蛛女王", "D-Elise", true);


            //TargetSelector
            var targetSelectorMenu = new Menu("目标 选择", "Target Selector");
            SimpleTs.AddToMenu(targetSelectorMenu);
            _config.AddSubMenu(targetSelectorMenu);

            //Orbwalker
            _config.AddSubMenu(new Menu("走砍", "Orbwalking"));
            _orbwalker = new Orbwalking.Orbwalker(_config.SubMenu("Orbwalking"));

            //Combo
            _config.AddSubMenu(new Menu("连招", "Combo"));
            _config.SubMenu("Combo").AddItem(new MenuItem("UseHumanQ", "使用 人型 Q")).SetValue(true);
            _config.SubMenu("Combo").AddItem(new MenuItem("UseHumanW", "使用 人型 W")).SetValue(true);
            _config.SubMenu("Combo").AddItem(new MenuItem("UseHumanE", "使用 人型 E")).SetValue(true);
            _config.SubMenu("Combo").AddItem(new MenuItem("UseRCombo", "自动 使用 R")).SetValue(true);
            _config.SubMenu("Combo").AddItem(new MenuItem("UseSpiderQ", "使用 蜘蛛 Q")).SetValue(true);
            _config.SubMenu("Combo").AddItem(new MenuItem("UseSpiderW", "使用 蜘蛛 W")).SetValue(true);
            _config.SubMenu("Combo").AddItem(new MenuItem("UseSpiderE", "使用 蜘蛛 E")).SetValue(true);
            _config.SubMenu("Combo")
                .AddItem(new MenuItem("ActiveCombo", "连招!").SetValue(new KeyBind(32, KeyBindType.Press)));

            //Harass
            _config.AddSubMenu(new Menu("骚扰", "Harass"));
            _config.SubMenu("Harass").AddItem(new MenuItem("UseQHarass", "使用 人型 Q")).SetValue(true);
            _config.SubMenu("Harass").AddItem(new MenuItem("UseWHarass", "使用 人型 W")).SetValue(true);
            _config.SubMenu("Harass")
                .AddItem(new MenuItem("Harrasmana", "骚扰最低蓝量").SetValue(new Slider(60, 1, 100)));
            _config.SubMenu("Harass")
                .AddItem(
                    new MenuItem("ActiveHarass", "骚扰 键位").SetValue(new KeyBind("C".ToCharArray()[0],
                        KeyBindType.Press)));


            _config.AddSubMenu(new Menu("物品", "items"));
            _config.SubMenu("items").AddSubMenu(new Menu("攻击 物品", "Offensive"));
            _config.SubMenu("items").SubMenu("Offensive").AddItem(new MenuItem("Tiamat", "使用 提亚马特")).SetValue(true);
            _config.SubMenu("items").SubMenu("Offensive").AddItem(new MenuItem("Hydra", "使用 九头蛇")).SetValue(true);
            _config.SubMenu("items").SubMenu("Offensive").AddItem(new MenuItem("Bilge", "使用 小弯刀")).SetValue(true);
            _config.SubMenu("items")
                .SubMenu("Offensive")
                .AddItem(new MenuItem("BilgeEnemyhp", "使用小弯刀|敌人血量").SetValue(new Slider(85, 1, 100)));
            _config.SubMenu("items")
                .SubMenu("Offensive")
                .AddItem(new MenuItem("Bilgemyhp", "使用小弯刀|自己血量").SetValue(new Slider(85, 1, 100)));
            _config.SubMenu("items").SubMenu("Offensive").AddItem(new MenuItem("Blade", "使用 破败")).SetValue(true);
            _config.SubMenu("items")
                .SubMenu("Offensive")
                .AddItem(new MenuItem("BladeEnemyhp", "使用破败|敌人血量").SetValue(new Slider(85, 1, 100)));
            _config.SubMenu("items")
                .SubMenu("Offensive")
                .AddItem(new MenuItem("Blademyhp", "使用破败|自己血量").SetValue(new Slider(85, 1, 100)));
            _config.SubMenu("items").AddSubMenu(new Menu("防御 物品", "Deffensive"));
            _config.SubMenu("items")
                .SubMenu("Deffensive")
                .AddItem(new MenuItem("Omen", "使用 兰盾"))
                .SetValue(true);
            _config.SubMenu("items")
                .SubMenu("Deffensive")
                .AddItem(new MenuItem("Omenenemys", "使用兰盾|敌人数量").SetValue(new Slider(2, 1, 5)));
            _config.SubMenu("items")
                .SubMenu("Deffensive")
                .AddItem(new MenuItem("lotis", "使用 鸟盾"))
                .SetValue(true);
            _config.SubMenu("items")
                .SubMenu("Deffensive")
                .AddItem(new MenuItem("lotisminhp", "使用鸟盾|队友血量").SetValue(new Slider(35, 1, 100)));
            _config.SubMenu("items")
                .SubMenu("Deffensive")
                .AddItem(new MenuItem("Zhonyas", "使用 中亚"))
                .SetValue(true);
            _config.SubMenu("items")
                .SubMenu("Deffensive")
                .AddItem(new MenuItem("Zhonyashp", "使用中亚|自己血量").SetValue(new Slider(20, 1, 100)));
            /*_config.SubMenu("items").AddSubMenu(new Menu("Potions", "Potions"));
            _config.SubMenu("items").SubMenu("Potions").AddItem(new MenuItem("Hppotion", "Use Hp potion")).SetValue(true);
            _config.SubMenu("items").SubMenu("Potions").AddItem(new MenuItem("Hppotionuse", "Use Hp potion if HP<").SetValue(new Slider(35, 1, 100)));
            _config.SubMenu("items").SubMenu("Potions").AddItem(new MenuItem("Mppotion", "Use Mp potion")).SetValue(true);
            _config.SubMenu("items").SubMenu("Potions").AddItem(new MenuItem("Mppotionuse", "Use Mp potion if HP<").SetValue(new Slider(35, 1, 100)));
            */

            //Farm
            _config.AddSubMenu(new Menu("清线", "Farm"));
            _config.SubMenu("Farm").AddItem(new MenuItem("HumanQFarm", "使用 人型 Q")).SetValue(true);
            _config.SubMenu("Farm").AddItem(new MenuItem("HumanWFarm", "使用 人型 W")).SetValue(true);
            _config.SubMenu("Farm").AddItem(new MenuItem("SpiderQFarm", "使用 蜘蛛 Q")).SetValue(false);
            _config.SubMenu("Farm").AddItem(new MenuItem("SpiderWFarm", "使用 蜘蛛 W")).SetValue(true);
            _config.SubMenu("Farm")
                .AddItem(
                    new MenuItem("Farm_R", "形态 转换(自动)").SetValue(new KeyBind("G".ToCharArray()[0],
                        KeyBindType.Toggle)));
            _config.SubMenu("Farm")
                .AddItem(
                    new MenuItem("ActiveFreeze", "控线").SetValue(new KeyBind("X".ToCharArray()[0],
                        KeyBindType.Press)));
            _config.SubMenu("Farm")
                .AddItem(
                    new MenuItem("ClearActive", "清线").SetValue(new KeyBind("V".ToCharArray()[0],
                        KeyBindType.Press)));
            _config.SubMenu("Farm").AddItem(new MenuItem("Lanemana", "清线最低蓝量").SetValue(new Slider(60, 1, 100)));

            //Farm
            _config.AddSubMenu(new Menu("清野", "Jungle"));
            _config.SubMenu("Jungle").AddItem(new MenuItem("HumanQFarmJ", "使用 人型 Q")).SetValue(true);
            _config.SubMenu("Jungle").AddItem(new MenuItem("HumanWFarmJ", "使用 人型 W")).SetValue(true);
            _config.SubMenu("Jungle").AddItem(new MenuItem("SpiderQFarmJ", "使用 蜘蛛 Q")).SetValue(false);
            _config.SubMenu("Jungle").AddItem(new MenuItem("SpiderWFarmJ", "使用 蜘蛛 W")).SetValue(true);
            _config.SubMenu("Jungle")
                .AddItem(
                    new MenuItem("ActiveJungle", "清野！").SetValue(new KeyBind("V".ToCharArray()[0], KeyBindType.Press)));
            _config.SubMenu("Jungle")
                .AddItem(new MenuItem("Junglemana", "清野 最低蓝量").SetValue(new Slider(60, 1, 100)));

            //Smite 
            _config.AddSubMenu(new Menu("惩戒", "Smite"));
            _config.SubMenu("Smite").AddItem(new MenuItem("Usesmite", "使用 惩戒(自动)").SetValue(new KeyBind("H".ToCharArray()[0],KeyBindType.Toggle)));
            _config.SubMenu("Smite").AddItem(new MenuItem("Useblue", "提前 惩戒蓝buff ")).SetValue(true);
            _config.SubMenu("Smite").AddItem(new MenuItem("manaJ", "提前惩戒蓝buff|蓝量少于").SetValue(new Slider(35, 1, 100)));
            _config.SubMenu("Smite").AddItem(new MenuItem("Usered", "提前 惩戒红buff")).SetValue(true);
            _config.SubMenu("Smite").AddItem(new MenuItem("healthJ", "提前惩戒红buff|血量少于").SetValue(new Slider(35, 1, 100)));
            _config.SubMenu("Smite").AddItem(new MenuItem("smitecombo", "对选择目标使用惩戒")).SetValue(true);
            _config.Item("smitecombo").ValueChanged += Switchcombo;
            _config.SubMenu("Smite").AddItem(new MenuItem("Smiteeee", "惩戒炮兵 在蜘蛛E的路径里").SetValue(false));
            _config.Item("Smiteeee").ValueChanged += Switchminion;

            //misc
            _config.AddSubMenu(new Menu("杂项", "Misc"));
            _config.SubMenu("Misc").AddItem(new MenuItem("usePackets", "使用 封包")).SetValue(true);
            _config.SubMenu("Misc").AddItem(new MenuItem("Spidergapcloser", "蜘蛛E 防止突进")).SetValue(true);
            _config.SubMenu("Misc").AddItem(new MenuItem("Humangapcloser", "人类E 防止突进")).SetValue(true);
            _config.SubMenu("Misc").AddItem(new MenuItem("UseEInt", "人型E 中断法术")).SetValue(true);
            _config.SubMenu("Misc")
                .AddItem(
                    new MenuItem("autoE", "人型E 使用极高命中率").SetValue(new KeyBind("T".ToCharArray()[0],
                        KeyBindType.Press)));
            _config.SubMenu("Misc")
                .AddItem(new MenuItem("Echange", "E 连招 命中率").SetValue(
                    new StringList(new[] {"低", "正常", "高", "极高"})));


            //Kill Steal
            _config.AddSubMenu(new Menu("抢人头", "Ks"));
            _config.SubMenu("Ks").AddItem(new MenuItem("ActiveKs", "自动 抢人头")).SetValue(true);
            _config.SubMenu("Ks").AddItem(new MenuItem("HumanQKs", "人型 Q")).SetValue(true);
            _config.SubMenu("Ks").AddItem(new MenuItem("HumanWKs", "人型 W")).SetValue(true);
            _config.SubMenu("Ks").AddItem(new MenuItem("SpiderQKs", "蜘蛛 Q")).SetValue(true);
            _config.SubMenu("Ks").AddItem(new MenuItem("UseIgnite", "使用 点燃")).SetValue(true);


            //Drawings
            _config.AddSubMenu(new Menu("范围", "Drawings"));
            _config.SubMenu("Drawings").AddItem(new MenuItem("DrawQ", "人型 Q")).SetValue(true);
            _config.SubMenu("Drawings").AddItem(new MenuItem("DrawW", "人型 W")).SetValue(true);
            _config.SubMenu("Drawings").AddItem(new MenuItem("DrawE", "人型 E")).SetValue(true);
            _config.SubMenu("Drawings").AddItem(new MenuItem("SpiderDrawQ", "蜘蛛 Q")).SetValue(true);
            _config.SubMenu("Drawings").AddItem(new MenuItem("SpiderDrawE", "蜘蛛 E")).SetValue(true);
            _config.SubMenu("Drawings").AddItem(new MenuItem("Drawsmite", "显示 惩戒 范围")).SetValue(true);
            _config.SubMenu("Drawings").AddItem(new MenuItem("drawmode", "显示 惩戒 模式")).SetValue(true);
            _config.SubMenu("Drawings").AddItem(new MenuItem("CircleLag", "延迟 自由圈").SetValue(true));
            _config.SubMenu("Drawings")
                .AddItem(new MenuItem("CircleQuality", "圈 质量").SetValue(new Slider(100, 100, 10)));
            _config.SubMenu("Drawings")
                .AddItem(new MenuItem("CircleThickness", "圈 厚度").SetValue(new Slider(1, 10, 1)));

            _config.AddToMainMenu();
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Interrupter.OnPossibleToInterrupt += Interrupter_OnPosibleToInterrupt;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            Game.OnGameUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Game.PrintChat("<font color='#881df2'>D-铚樿洓濂崇帇 by Diabaths</font> 鍔犺浇鎴愬姛!姹夊寲by浜岀嫍!QQ缇361630847.");
            Game.PrintChat("<font color='#FF0000'>鎹愯禒浣滆€卲aypal</font> <font color='#FF9900'>ssssssssssmith@hotmail.com</font> (10) S");
        
        }
        private static void Switchcombo(object sender, OnValueChangeEventArgs e)
        {
            if (e.GetNewValue<bool>())
                _config.Item("Smiteeee").SetValue(false);
        }

        private static void Switchminion(object sender, OnValueChangeEventArgs e)
        {
            if (e.GetNewValue<bool>())
                _config.Item("smitecombo").SetValue(false);
        }
        private static void Game_OnGameUpdate(EventArgs args)
        {
            Cooldowns();

            _player = ObjectManager.Player;

            _orbwalker.SetAttack(true);

            CheckSpells();
            if (_config.Item("Usesmite").GetValue<KeyBind>().Active)
            {
                Smiteuse();
            }
            if (_config.Item("ActiveFreeze").GetValue<KeyBind>().Active ||
                _config.Item("ClearActive").GetValue<KeyBind>().Active)

                FarmLane();

            if (_config.Item("ActiveJungle").GetValue<KeyBind>().Active)
            {
                JungleFarm();

            }
            if (_config.Item("ActiveCombo").GetValue<KeyBind>().Active)
            {
                Combo();
            }
            if (_config.Item("ActiveHarass").GetValue<KeyBind>().Active)
            {
                Harass();

            }
            if (_config.Item("ActiveKs").GetValue<bool>())
            {
                KillSteal();
            }
            if (_config.Item("autoE").GetValue<KeyBind>().Active)
            {
                AutoE();

            }
          }
       
        private static void Smiteontarget(Obj_AI_Hero target)
        {
            var usesmite = _config.Item("smitecombo").GetValue<bool>();
            var itemscheck = SmiteBlue.Any(Items.HasItem) || SmiteRed.Any(Items.HasItem);
            if (itemscheck && usesmite &&
                ObjectManager.Player.SummonerSpellbook.CanUseSpell(_smiteSlot) == SpellState.Ready &&
                target.Distance(_player.Position) < _smite.Range)
            {
                ObjectManager.Player.SummonerSpellbook.CastSpell(_smiteSlot, target);
            }
        }
        private static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
                //Game.PrintChat("Spell name: " + args.SData.Name.ToString());
                GetCDs(args);
        }
        private static void UseItemes(Obj_AI_Hero target)
        {
            var iBilge = _config.Item("Bilge").GetValue<bool>();
            var iBilgeEnemyhp = target.Health <=
                                (target.MaxHealth * (_config.Item("BilgeEnemyhp").GetValue<Slider>().Value) / 100);
            var iBilgemyhp = _player.Health <=
                             (_player.MaxHealth * (_config.Item("Bilgemyhp").GetValue<Slider>().Value) / 100);
            var iBlade = _config.Item("Blade").GetValue<bool>();
            var iBladeEnemyhp = target.Health <=
                                (target.MaxHealth * (_config.Item("BladeEnemyhp").GetValue<Slider>().Value) / 100);
            var iBlademyhp = _player.Health <=
                             (_player.MaxHealth * (_config.Item("Blademyhp").GetValue<Slider>().Value) / 100);
            var iOmen = _config.Item("Omen").GetValue<bool>();
            var iOmenenemys = ObjectManager.Get<Obj_AI_Hero>().Count(hero => hero.IsValidTarget(450)) >=
                              _config.Item("Omenenemys").GetValue<Slider>().Value;
            var iTiamat = _config.Item("Tiamat").GetValue<bool>();
            var iHydra = _config.Item("Hydra").GetValue<bool>();
            var ilotis = _config.Item("lotis").GetValue<bool>();
            var iZhonyas = _config.Item("Zhonyas").GetValue<bool>();
            var iZhonyashp = _player.Health <=
                             (_player.MaxHealth * (_config.Item("Zhonyashp").GetValue<Slider>().Value) / 100);
            //var ihp = _config.Item("Hppotion").GetValue<bool>();
            // var ihpuse = _player.Health <= (_player.MaxHealth * (_config.Item("Hppotionuse").GetValue<Slider>().Value) / 100);
            //var imp = _config.Item("Mppotion").GetValue<bool>();
            //var impuse = _player.Health <= (_player.MaxHealth * (_config.Item("Mppotionuse").GetValue<Slider>().Value) / 100);

            if (_player.Distance(target) <= 450 && iBilge && (iBilgeEnemyhp || iBilgemyhp) && _bilge.IsReady())
            {
                _bilge.Cast(target);

            }
            if (_player.Distance(target) <= 450 && iBlade && (iBladeEnemyhp || iBlademyhp) && _blade.IsReady())
            {
                _blade.Cast(target);

            }
            if (iTiamat && _tiamat.IsReady() && target.IsValidTarget(_tiamat.Range))
            {
                _tiamat.Cast();

            }
            if (iHydra && _hydra.IsReady() && target.IsValidTarget(_hydra.Range))
            {
                _hydra.Cast();

            }
            if (iOmenenemys && iOmen && _rand.IsReady())
            {
                _rand.Cast();

            }
            if (ilotis)
            {
                foreach (var hero in ObjectManager.Get<Obj_AI_Hero>().Where(hero => hero.IsAlly || hero.IsMe))
                {
                    if (hero.Health <= (hero.MaxHealth * (_config.Item("lotisminhp").GetValue<Slider>().Value) / 100) &&
                        hero.Distance(_player.ServerPosition) <= _lotis.Range && _lotis.IsReady())
                        _lotis.Cast();
                }
            }
            if (iZhonyas && iZhonyashp && Utility.CountEnemysInRange(1000) >= 1)
            {
                _zhonya.Cast(_player);

            }
        }

        private static void Combo()
        {
            var target = SimpleTs.GetTarget(_humanW.Range, SimpleTs.DamageType.Magical);
            var sReady = (_smiteSlot != SpellSlot.Unknown && ObjectManager.Player.SummonerSpellbook.CanUseSpell(_smiteSlot) == SpellState.Ready);
            var qdmg = _player.GetSpellDamage(target, SpellSlot.Q);
            var wdmg = _player.GetSpellDamage(target, SpellSlot.W);
            if (target == null) return; //buffelisecocoon
            Smiteontarget(target);
            if (_human)
            {
                if (target.Distance(_player.Position) < _humanE.Range && _config.Item("UseHumanE").GetValue<bool>() && _humanE.IsReady())
                {
                    if (sReady && _config.Item("Smiteeee").GetValue<bool>() && 
                        _humanE.GetPrediction(target).CollisionObjects.Count == 1)
                    {
                        CheckingCollision(target);
                        _humanE.Cast(target, Packets());
                    }
                    else if (_humanE.GetPrediction(target).Hitchance >= Echange())
                    {
                        _humanE.Cast(target, Packets());
                    }
                }

                if (_player.Distance(target) <= _humanQ.Range && _config.Item("UseHumanQ").GetValue<bool>() && _humanQ.IsReady())
                {
                    _humanQ.Cast(target, Packets());
                }
                if (_player.Distance(target) <= _humanW.Range && _config.Item("UseHumanW").GetValue<bool>() && _humanW.IsReady())
                {
                    _humanW.Cast(target, Packets());
                }
                if (!_humanQ.IsReady() && !_humanW.IsReady() && !_humanE.IsReady() && _config.Item("UseRCombo").GetValue<bool>() && _r.IsReady())
                {
                    _r.Cast();
                }
                if (!_humanQ.IsReady() && !_humanW.IsReady() && _player.Distance(target) <= _spiderQ.Range && _config.Item("UseRCombo").GetValue<bool>() && _r.IsReady())
                {
                    _r.Cast();
                }
            }
            if (!_spider) return;
            if (_player.Distance(target) <= _spiderQ.Range && _config.Item("UseSpiderQ").GetValue<bool>() && _spiderQ.IsReady())
            {
                _spiderQ.Cast(target, Packets());
            }
            if (_player.Distance(target) <= 200 && _config.Item("UseSpiderW").GetValue<bool>() && _spiderW.IsReady())
            {
                _spiderW.Cast();
            }
            if (_player.Distance(target) <= _spiderE.Range && _player.Distance(target) > _spiderQ.Range && _config.Item("UseSpiderE").GetValue<bool>() && _spiderE.IsReady() && !_spiderQ.IsReady())
            {
                _spiderE.Cast(target, Packets());
            }
            if (_player.Distance(target) > _spiderQ.Range && !_spiderE.IsReady() && _r.IsReady() && !_spiderQ.IsReady() && _config.Item("UseRCombo").GetValue<bool>())
            {
                _r.Cast();
            }
            if (_humanQ.IsReady() && _humanW.IsReady() && _r.IsReady() && _config.Item("UseRCombo").GetValue<bool>())
            {
                _r.Cast();
            }
            if (_humanQ.IsReady() && _humanW.IsReady() && _r.IsReady() && _config.Item("UseRCombo").GetValue<bool>())
            {
                _r.Cast();
            }
            if ((_humanQ.IsReady() && qdmg >= target.Health || _humanW.IsReady() && wdmg >= target.Health) && _config.Item("UseRCombo").GetValue<bool>())
            {
                _r.Cast();
            }
            UseItemes(target);
        }

        private static void Harass()
        {
            var target = SimpleTs.GetTarget(_humanQ.Range, SimpleTs.DamageType.Magical);
            if (target != null)
            {

                if (_human && _player.Distance(target) <= _humanQ.Range && _config.Item("UseQHarass").GetValue<bool>() && _humanQ.IsReady())
                {
                    _humanQ.Cast(target, Packets());
                }

                if (_human && _player.Distance(target) <= _humanW.Range && _config.Item("UseWHarass").GetValue<bool>() && _humanW.IsReady())
                {
                    _humanW.Cast(target, Packets());
                }
            }
        }

        private static void JungleFarm()
        {
            var jungleQ = (_config.Item("HumanQFarmJ").GetValue<bool>() && (100 * (_player.Mana / _player.MaxMana)) > _config.Item("Junglemana").GetValue<Slider>().Value);
            var jungleW = (_config.Item("HumanWFarmJ").GetValue<bool>() && (100 * (_player.Mana / _player.MaxMana)) > _config.Item("Junglemana").GetValue<Slider>().Value);
            var spiderjungleQ = _config.Item("SpiderQFarmJ").GetValue<bool>();
            var spiderjungleW = _config.Item("SpiderWFarmJ").GetValue<bool>();
            var switchR = (100 * (_player.Mana / _player.MaxMana)) < _config.Item("Junglemana").GetValue<Slider>().Value;
            var mobs = MinionManager.GetMinions(_player.ServerPosition, _humanQ.Range,
            MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);
            if (mobs.Count > 0)
            {
                foreach (var minion in mobs)
                    if (_human)
                    {
                        if (jungleQ && _humanQ.IsReady() && minion.IsValidTarget() && _player.Distance(minion) <= _humanQ.Range)
                        {
                            _humanQ.Cast(minion, Packets());
                        }
                        if (jungleW && _humanW.IsReady() && !_humanQ.IsReady() && minion.IsValidTarget() && _player.Distance(minion) <= _humanW.Range)
                        {
                            _humanW.Cast(minion, Packets());
                        }
                        if ((!_humanQ.IsReady() && !_humanW.IsReady()) || switchR)
                        {
                            _r.Cast();
                        }
                    }
                foreach (var minion in mobs)
                {
                    if (_spider)
                    {
                        if (spiderjungleQ && _spiderQ.IsReady() && minion.IsValidTarget() && _player.Distance(minion) <= _spiderQ.Range)
                        {
                            _spiderQ.Cast(minion, Packets());
                        }
                        if (spiderjungleW && _spiderW.IsReady() && minion.IsValidTarget() && _player.Distance(minion) <= 150)
                        {
                            _orbwalker.SetAttack(true);
                            _spiderW.Cast();
                        }
                        if (_r.IsReady() && _humanQ.IsReady() && !_spiderQ.IsReady() && !_spiderW.IsReady() && _spider)
                        {
                            _r.Cast();
                        }
                    }
                }
            }
        }

        private static void FarmLane()
        {
            var ManaUse = (100 * (_player.Mana / _player.MaxMana)) > _config.Item("Lanemana").GetValue<Slider>().Value;
            var useR = _config.Item("Farm_R").GetValue<KeyBind>().Active;
            var useHumQ = (_config.Item("HumanQFarm").GetValue<bool>() &&
                           (100 * (_player.Mana / _player.MaxMana)) > _config.Item("Lanemana").GetValue<Slider>().Value);
            var useHumW = (_config.Item("HumanWFarm").GetValue<bool>() &&
                           (100 * (_player.Mana / _player.MaxMana)) > _config.Item("Lanemana").GetValue<Slider>().Value);
            var useSpiQFarm = (_spiderQ.IsReady() && _config.Item("SpiderQFarm").GetValue<bool>());
            var useSpiWFarm = (_spiderW.IsReady() && _config.Item("SpiderWFarm").GetValue<bool>());
            var allminions = MinionManager.GetMinions(_player.ServerPosition, _humanQ.Range, MinionTypes.All,
                MinionTeam.Enemy, MinionOrderTypes.Health);
            {
                if (_config.Item("ClearActive").GetValue<KeyBind>().Active)
                {
                    foreach (var minion in allminions)
                        if (_human)
                        {
                            if (useHumQ && _humanQ.IsReady() && minion.IsValidTarget() &&
                                _player.Distance(minion) <= _humanQ.Range)
                            {
                                _humanQ.Cast(minion);
                            }
                            if (useHumW && _humanW.IsReady() && minion.IsValidTarget() &&
                                _player.Distance(minion) <= _humanW.Range)
                            {
                                _humanW.Cast(minion);
                            }
                            if (useR && _r.IsReady())
                            {
                                _r.Cast();
                            }
                        }
                    foreach (var minion in allminions)
                        if (_spider)
                        {
                            if (useSpiQFarm && _spiderQ.IsReady() && minion.IsValidTarget() &&
                                _player.Distance(minion) <= _spiderQ.Range)
                            {
                                _spiderQ.Cast(minion);
                            }
                            if (useSpiWFarm && _spiderW.IsReady() && minion.IsValidTarget() &&
                                _player.Distance(minion) <= 125)
                            {
                                _spiderW.Cast();
                            }
                        }
                }
                if (_config.Item("ActiveFreeze").GetValue<KeyBind>().Active)
                {
                    foreach (var minion in allminions)
                        if (_human)
                        {
                            if (useHumQ && _player.GetSpellDamage(minion, SpellSlot.Q) > minion.Health &&
                                _humanQ.IsReady() && minion.IsValidTarget() && _player.Distance(minion) <= _humanQ.Range)
                            {
                                _humanQ.Cast(minion);
                            }
                            if (useHumW && _player.GetSpellDamage(minion, SpellSlot.W) > minion.Health &&
                                _humanW.IsReady() && minion.IsValidTarget() && _player.Distance(minion) <= _humanW.Range)
                            {
                                _humanW.Cast(minion);
                            }
                            if (useR && _r.IsReady())
                            {
                                _r.Cast();
                            }
                        }
                    foreach (var minion in allminions)
                        if (_spider)
                        {
                            if (useSpiQFarm && _spiderQ.IsReady() &&
                                _player.GetSpellDamage(minion, SpellSlot.Q) > minion.Health && _spiderQ.IsReady() &&
                                minion.IsValidTarget() && _player.Distance(minion) <= _spiderQ.Range)
                            {
                                _spiderQ.Cast(minion);
                            }
                            if (useSpiQFarm && _spiderW.IsReady() && minion.IsValidTarget() &&
                                _player.Distance(minion) <= 125)
                            {
                                _spiderW.Cast();
                            }
                        }
                }
            }
        }
        //Credits to Kurisu
        private static string Smitetype()
        {
            if (SmiteBlue.Any(Items.HasItem))
            {
                return "s5_summonersmiteplayerganker";
            }
            if (SmiteRed.Any(Items.HasItem))
            {
                return "s5_summonersmiteduel";
            }
            if (SmiteGrey.Any(Items.HasItem))
            {
                return "s5_summonersmitequick";
            }
            if (SmitePurple.Any(Items.HasItem))
            {
                return "itemsmiteaoe";
            }
            return "summonersmite";
        }

        //Credits to metaphorce
        private static void SetSmiteSlot()
        {
            foreach (
                var spell in
                    ObjectManager.Player.SummonerSpellbook.Spells.Where(
                        spell => String.Equals(spell.Name, Smitetype(), StringComparison.CurrentCultureIgnoreCase)))
            {
                _smiteSlot = spell.Slot;
                _smite = new Spell(_smiteSlot, 700);
                return;
            }
        }
      
        private static int GetSmiteDmg()
        {
            int level = _player.Level;
            int index = _player.Level / 5;
            float[] dmgs = { 370 + 20 * level, 330 + 30 * level, 240 + 40 * level, 100 + 50 * level };
            return (int)dmgs[index];
        }


        //New map Monsters Name By SKO
        private static void Smiteuse()
        {
            var jungle = _config.Item("ActiveJungle").GetValue<KeyBind>().Active;
            if (ObjectManager.Player.SummonerSpellbook.CanUseSpell(_smiteSlot) != SpellState.Ready) return;
            var useblue = _config.Item("Useblue").GetValue<bool>();
            var usered = _config.Item("Usered").GetValue<bool>();
            var health = (100 * (_player.Mana / _player.MaxMana)) < _config.Item("healthJ").GetValue<Slider>().Value;
            var mana = (100 * (_player.Mana / _player.MaxMana)) < _config.Item("manaJ").GetValue<Slider>().Value;
            string[] jungleMinions;
            if (Utility.Map.GetMap()._MapType.Equals(Utility.Map.MapType.TwistedTreeline))
            {
                jungleMinions = new string[] { "TT_Spiderboss", "TT_NWraith", "TT_NGolem", "TT_NWolf" };
            }
            else
            {
                jungleMinions = new string[]
                {
                    "SRU_Blue", "SRU_Gromp", "SRU_Murkwolf", "SRU_Razorbeak", "SRU_Red", "SRU_Krug", "SRU_Dragon",
                    "SRU_Baron", "Sru_Crab"
                };
            }
            var minions = MinionManager.GetMinions(_player.Position, 1000, MinionTypes.All, MinionTeam.Neutral);
            if (minions.Count() > 0)
            {
                int smiteDmg = GetSmiteDmg();

                foreach (Obj_AI_Base minion in minions)
                {
                    if (Utility.Map.GetMap()._MapType.Equals(Utility.Map.MapType.TwistedTreeline) &&
                        minion.Health <= smiteDmg &&
                        jungleMinions.Any(name => minion.Name.Substring(0, minion.Name.Length - 5).Equals(name)))
                    {
                        ObjectManager.Player.SummonerSpellbook.CastSpell(_smiteSlot, minion);
                    }
                    if (minion.Health <= smiteDmg && jungleMinions.Any(name => minion.Name.StartsWith(name)) &&
                        !jungleMinions.Any(name => minion.Name.Contains("Mini")))
                    {
                        ObjectManager.Player.SummonerSpellbook.CastSpell(_smiteSlot, minion);
                    }
                    else if (jungle && useblue && mana && minion.Health >= smiteDmg &&
                             jungleMinions.Any(name => minion.Name.StartsWith("SRU_Blue")) &&
                             !jungleMinions.Any(name => minion.Name.Contains("Mini")))
                    {
                        ObjectManager.Player.SummonerSpellbook.CastSpell(_smiteSlot, minion);
                    }
                    else if (jungle && usered && health && minion.Health >= smiteDmg &&
                             jungleMinions.Any(name => minion.Name.StartsWith("SRU_Red")) &&
                             !jungleMinions.Any(name => minion.Name.Contains("Mini")))
                    {
                        ObjectManager.Player.SummonerSpellbook.CastSpell(_smiteSlot, minion);
                    }
                }
            }
        }

        private static void AutoE()
        {
            _player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            var target = SimpleTs.GetTarget(_humanE.Range, SimpleTs.DamageType.Magical);

            if (_human && _player.Distance(target) < _humanE.Range && _humanE.IsReady() && _humanE.GetPrediction(target).Hitchance >= HitChance.VeryHigh)
            {
                _humanE.Cast(target, Packets());
            }
        }
        private static bool Packets()
        {
            return _config.Item("usePackets").GetValue<bool>();
        }

        private static void Interrupter_OnPosibleToInterrupt(Obj_AI_Base target, InterruptableSpell spell)
        {
            if (!_config.Item("UseEInt").GetValue<bool>()) return;
            if (_player.Distance(target) < _humanE.Range && target != null && _humanE.GetPrediction(target).Hitchance >= HitChance.Low)
            {
                _humanE.Cast(target, Packets());
            }
        }

        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (_spiderE.IsReady() && _spider && gapcloser.Sender.IsValidTarget(_spiderE.Range) && _config.Item("Spidergapcloser").GetValue<bool>())
            {
                _spiderE.Cast(gapcloser.Sender, Packets());
            }
            if (_humanE.IsReady() && _human && gapcloser.Sender.IsValidTarget(_humanE.Range) && _config.Item("Humangapcloser").GetValue<bool>())
            {
                _humanE.Cast(gapcloser.Sender, Packets());
            }
        }

        private static float CalculateCd(float time)
        {
            return time + (time * _player.PercentCooldownMod);
        }

        private static void Cooldowns()
        {
            _humaQcd = ((_humQcd - Game.Time) > 0) ? (_humQcd - Game.Time) : 0;
            _humaWcd = ((_humWcd - Game.Time) > 0) ? (_humWcd - Game.Time) : 0;
            _humaEcd = ((_humEcd - Game.Time) > 0) ? (_humEcd - Game.Time) : 0;
            _spideQcd = ((_spidQcd - Game.Time) > 0) ? (_spidQcd - Game.Time) : 0;
            _spideWcd = ((_spidWcd - Game.Time) > 0) ? (_spidWcd - Game.Time) : 0;
            _spideEcd = ((_spidEcd - Game.Time) > 0) ? (_spidEcd - Game.Time) : 0;
        }

        private static void GetCDs(GameObjectProcessSpellCastEventArgs spell)
        {
            if (_human)
            {
                if (spell.SData.Name == "EliseHumanQ")
                    _humQcd = Game.Time + CalculateCd(HumanQcd[_humanQ.Level]);
                if (spell.SData.Name == "EliseHumanW")
                    _humWcd = Game.Time + CalculateCd(HumanWcd[_humanW.Level]);
                if (spell.SData.Name == "EliseHumanE")
                    _humEcd = Game.Time + CalculateCd(HumanEcd[_humanE.Level]);
            }
            else
            {
                if (spell.SData.Name == "EliseSpiderQCast")
                    _spidQcd = Game.Time + CalculateCd(SpiderQcd[_spiderQ.Level]);
                if (spell.SData.Name == "EliseSpiderW")
                    _spidWcd = Game.Time + CalculateCd(SpiderWcd[_spiderW.Level]);
                if (spell.SData.Name == "EliseSpiderEInitial")
                    _spidEcd = Game.Time + CalculateCd(SpiderEcd[_spiderE.Level]);
            }
        }

        private static HitChance Echange()
        {
            switch (_config.Item("Echange").GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    return HitChance.Low;
                case 1:
                    return HitChance.Medium;
                case 2:
                    return HitChance.High;
                case 3:
                    return HitChance.VeryHigh;
                default:
                    return HitChance.Medium;
            }
        }
        // Credits to Brain0305
        private static bool CheckingCollision(Obj_AI_Hero target)
        {
            foreach (var col in MinionManager.GetMinions(_player.Position, 1500, MinionTypes.All, MinionTeam.NotAlly))
            {
                var segment = Geometry.ProjectOn(col.ServerPosition.To2D(), _player.ServerPosition.To2D(),
                    col.Position.To2D());
                if (segment.IsOnSegment &&
                    target.ServerPosition.To2D().Distance(segment.SegmentPoint) <= GetHitBox(col) + 40)
                {
                    if ( col.Distance(_player.Position) < _smite.Range  &&
                        col.Health < _player.GetSummonerSpellDamage(col, Damage.SummonerSpell.Smite))
                    {
                        _player.SummonerSpellbook.CastSpell(_smiteSlot, col);
                        return true;
                    }
                }
            }
            return false;
        }
        // Credits to Brain0305
        static float GetHitBox(Obj_AI_Base minion)
        {
            var nameMinion = minion.Name.ToLower();
            if (nameMinion.Contains("mech")) return 65;
            if (nameMinion.Contains("wizard") || nameMinion.Contains("basic")) return 48;
            if (nameMinion.Contains("wolf") || nameMinion.Contains("wraith")) return 50;
            if (nameMinion.Contains("golem") || nameMinion.Contains("lizard")) return 80;
            if (nameMinion.Contains("dragon") || nameMinion.Contains("worm")) return 100;
            return 50;
        }

        private static void KillSteal()
        {
            var target = SimpleTs.GetTarget(_humanQ.Range, SimpleTs.DamageType.Magical);
            var igniteDmg = _player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
            var qhDmg = _player.GetSpellDamage(target, SpellSlot.Q);
            var wDmg = _player.GetSpellDamage(target, SpellSlot.W);

            if (target != null && _config.Item("UseIgnite").GetValue<bool>() && _igniteSlot != SpellSlot.Unknown &&
            _player.SummonerSpellbook.CanUseSpell(_igniteSlot) == SpellState.Ready)
            {
                if (igniteDmg > target.Health)
                {
                    _player.SummonerSpellbook.CastSpell(_igniteSlot, target);
                }
            }
            if (_human)
            {
                if (_humanQ.IsReady() && _player.Distance(target) <= _humanQ.Range && target != null && _config.Item("HumanQKs").GetValue<bool>())
                {
                    if (target.Health <= qhDmg)
                    {
                        _humanQ.Cast(target);
                    }
                }
                if (_humanW.IsReady() && _player.Distance(target) <= _humanW.Range && target != null && _config.Item("HumanWKs").GetValue<bool>())
                {
                    if (target.Health <= wDmg)
                    {
                        _humanW.Cast(target);
                    }
                }
            }
            if (_spider && _spiderQ.IsReady() && _player.Distance(target) <= _spiderQ.Range && target != null && _config.Item("SpiderQKs").GetValue<bool>())
            {
                if (target.Health <= qhDmg)
                {
                    _spiderQ.Cast(target);
                }
            }
        }

        static void Drawing_OnDraw(EventArgs args)
        {
            var elise = Drawing.WorldToScreen(_player.Position);
            if (_config.Item("drawmode").GetValue<bool>() && _config.Item("Usesmite").GetValue<KeyBind>().Active)
            {
                if (_config.Item("smitecombo").GetValue<bool>())
                {
                    Drawing.DrawText(Drawing.Width * 0.90f, Drawing.Height * 0.66f, System.Drawing.Color.DarkOrange,
                      "Smite Tagret");
                }
                else Drawing.DrawText(Drawing.Width * 0.80f, Drawing.Height * 0.66f, System.Drawing.Color.DarkRed,
                     "Smite minion in Human E Path");
            }
                
            if (_config.Item("Drawsmite").GetValue<bool>())
            {
                if (_config.Item("Usesmite").GetValue<KeyBind>().Active)
                {
                    Drawing.DrawText(Drawing.Width * 0.90f, Drawing.Height * 0.68f, System.Drawing.Color.DarkOrange,
                        "Smite Is On");
                }
                else
                    Drawing.DrawText(Drawing.Width * 0.90f, Drawing.Height * 0.68f, System.Drawing.Color.DarkRed,
                        "Smite Is Off");
            }
            if (_config.Item("CircleLag").GetValue<bool>())
            {
                if (_human && _config.Item("DrawQ").GetValue<bool>())
                {
                    Utility.DrawCircle(ObjectManager.Player.Position, _humanQ.Range, System.Drawing.Color.Gray,
                        _config.Item("CircleThickness").GetValue<Slider>().Value,
                        _config.Item("CircleQuality").GetValue<Slider>().Value);
                }
                if (_human && _config.Item("DrawW").GetValue<bool>())
                {
                    Utility.DrawCircle(ObjectManager.Player.Position, _humanW.Range, System.Drawing.Color.Gray,
                        _config.Item("CircleThickness").GetValue<Slider>().Value,
                        _config.Item("CircleQuality").GetValue<Slider>().Value);
                }
                if (_human && _config.Item("DrawE").GetValue<bool>())
                {
                    Utility.DrawCircle(ObjectManager.Player.Position, _humanE.Range, System.Drawing.Color.Gray,
                        _config.Item("CircleThickness").GetValue<Slider>().Value,
                        _config.Item("CircleQuality").GetValue<Slider>().Value);
                }
                if (_spider && _config.Item("SpiderDrawQ").GetValue<bool>())
                {
                    Utility.DrawCircle(ObjectManager.Player.Position, _spiderQ.Range, System.Drawing.Color.Gray,
                        _config.Item("CircleThickness").GetValue<Slider>().Value,
                        _config.Item("CircleQuality").GetValue<Slider>().Value);
                }
                if (_spider && _config.Item("SpiderDrawE").GetValue<bool>())
                {
                    Utility.DrawCircle(ObjectManager.Player.Position, _spiderE.Range, System.Drawing.Color.Gray,
                   _config.Item("CircleThickness").GetValue<Slider>().Value,
                   _config.Item("CircleQuality").GetValue<Slider>().Value);
                }
            }
            else
            {
                if (_human && _config.Item("DrawQ").GetValue<bool>())
                {
                    Drawing.DrawCircle(ObjectManager.Player.Position, _humanQ.Range, System.Drawing.Color.LightGray);
                }
                if (_human && _config.Item("DrawW").GetValue<bool>())
                {
                    Drawing.DrawCircle(ObjectManager.Player.Position, _humanW.Range, System.Drawing.Color.LightGray);
                }
                if (_human && _config.Item("DrawE").GetValue<bool>())
                {
                    Drawing.DrawCircle(ObjectManager.Player.Position, _humanE.Range, System.Drawing.Color.LightGray);
                }
                if (_spider && _config.Item("SpiderDrawQ").GetValue<bool>())
                {
                    Drawing.DrawCircle(ObjectManager.Player.Position, _spiderQ.Range, System.Drawing.Color.LightGray);
                }
                if (_spider && _config.Item("SpiderDrawE").GetValue<bool>())
                {
                    Drawing.DrawCircle(ObjectManager.Player.Position, _spiderE.Range, System.Drawing.Color.LightGray);
                }
            }
            if (!_spider)
            {
                if (_spideQcd == 0)
                    Drawing.DrawText(elise[0] - 60, elise[1], Color.White, "SQ Rdy");
                else
                    Drawing.DrawText(elise[0] - 60, elise[1], Color.Orange, "SQ: " + _spideQcd.ToString("0.0"));
                if (_spideWcd == 0)
                    Drawing.DrawText(elise[0] - 20, elise[1] + 30, Color.White, "SW Rdy");
                else
                    Drawing.DrawText(elise[0] - 20, elise[1] + 30, Color.Orange, "SW: " + _spideWcd.ToString("0.0"));
                if (_spideEcd == 0)
                    Drawing.DrawText(elise[0], elise[1], Color.White, "SE Rdy");
                else
                    Drawing.DrawText(elise[0], elise[1], Color.Orange, "SE: " + _spideEcd.ToString("0.0"));
            }
            else
            {
                if (_humaQcd == 0)
                    Drawing.DrawText(elise[0] - 60, elise[1], Color.White, "HQ Rdy");
                else
                    Drawing.DrawText(elise[0] - 60, elise[1], Color.Orange, "HQ: " + _humaQcd.ToString("0.0"));
                if (_humaWcd == 0)
                    Drawing.DrawText(elise[0] - 20, elise[1] + 30, Color.White, "HW Rdy");
                else
                    Drawing.DrawText(elise[0] - 20, elise[1] + 30, Color.Orange, "HW: " + _humaWcd.ToString("0.0"));
                if (_humaEcd == 0)
                    Drawing.DrawText(elise[0], elise[1], Color.White, "HE Rdy");
                else
                    Drawing.DrawText(elise[0], elise[1], Color.Orange, "HE: " + _humaEcd.ToString("0.0"));
            }
        }

        private static void CheckSpells()
        {
            if (_player.Spellbook.GetSpell(SpellSlot.Q).Name == "EliseHumanQ" ||
                _player.Spellbook.GetSpell(SpellSlot.W).Name == "EliseHumanW" ||
                _player.Spellbook.GetSpell(SpellSlot.E).Name == "EliseHumanE")
            {
                _human = true;
                _spider = false;
            }

            if (_player.Spellbook.GetSpell(SpellSlot.Q).Name == "EliseSpiderQCast" ||
                _player.Spellbook.GetSpell(SpellSlot.W).Name == "EliseSpiderW" ||
                _player.Spellbook.GetSpell(SpellSlot.E).Name == "EliseSpiderEInitial")
            {
                _human = false;
                _spider = true;
            }
        }
    }
}


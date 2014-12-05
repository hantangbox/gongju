using System;
using System.Linq;
using System.Net;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

//gg

namespace D_Jarvan
{
    internal class Program
    {
        private const string ChampionName = "JarvanIV";

        private static Orbwalking.Orbwalker _orbwalker;

        private static Spell _q, _w, _e, _r;

        private static SpellSlot _igniteSlot;

        private static Int32 _lastSkin;

        private static Items.Item _tiamat, _hydra, _blade, _bilge, _rand, _lotis;

        private static Menu _config;

        private static Obj_AI_Hero _player;

        private static bool _haveulti;

        private static SpellSlot _smiteSlot = SpellSlot.Unknown;

        private static Spell _smite;

        private static SpellSlot _flashSlot;

        private static Vector3 _epos = default(Vector3);

        //Credits to Kurisu
        private static readonly int[] SmitePurple = {3713, 3726, 3725, 3726, 3723};
        private static readonly int[] SmiteGrey = {3711, 3722, 3721, 3720, 3719};
        private static readonly int[] SmiteRed = {3715, 3718, 3717, 3716, 3714};
        private static readonly int[] SmiteBlue = {3706, 3710, 3709, 3708, 3707};

        private static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        private static void Game_OnGameLoad(EventArgs args)
        {
            _player = ObjectManager.Player;
            if (ObjectManager.Player.BaseSkinName != ChampionName) return;

            _q = new Spell(SpellSlot.Q, 770f);
            _w = new Spell(SpellSlot.W, 300f);
            _e = new Spell(SpellSlot.E, 830f);
            _r = new Spell(SpellSlot.R, 650f);

            _q.SetSkillshot(0.5f, 70f, float.MaxValue, false, SkillshotType.SkillshotLine);
            _e.SetSkillshot(0.5f, 70f, float.MaxValue, false, SkillshotType.SkillshotCircle);

            _igniteSlot = _player.GetSpellSlot("SummonerDot");
            _flashSlot = _player.GetSpellSlot("SummonerFlash");
            SetSmiteSlot();

            _bilge = new Items.Item(3144, 475f);
            _blade = new Items.Item(3153, 425f);
            _hydra = new Items.Item(3074, 250f);
            _tiamat = new Items.Item(3077, 250f);
            _rand = new Items.Item(3143, 490f);
            _lotis = new Items.Item(3190, 590f);

            //D Jarvan
            _config = new Menu("D-王子", "D-Jarvan", true);

            //TargetSelector
            var targetSelectorMenu = new Menu("目标 选择", "Target Selector");
            SimpleTs.AddToMenu(targetSelectorMenu);
            _config.AddSubMenu(targetSelectorMenu);


            //Orbwalker
            _config.AddSubMenu(new Menu("走砍", "Orbwalking"));
            _orbwalker = new Orbwalking.Orbwalker(_config.SubMenu("Orbwalking"));

            //Combo
            _config.AddSubMenu(new Menu("连招", "Combo"));
            _config.SubMenu("Combo").AddItem(new MenuItem("UseIgnite", "使用 点燃")).SetValue(true);
            _config.SubMenu("Combo").AddItem(new MenuItem("smitecombo", "对目标使用惩戒")).SetValue(true);
            _config.SubMenu("Combo").AddItem(new MenuItem("UseQC", "使用 Q")).SetValue(true);
            _config.SubMenu("Combo").AddItem(new MenuItem("UseWC", "使用 W")).SetValue(true);
            _config.SubMenu("Combo").AddItem(new MenuItem("UseEC", "使用 E")).SetValue(true);
            _config.SubMenu("Combo").AddItem(new MenuItem("UseRC", "使用 R(可击杀)")).SetValue(true);
            _config.SubMenu("Combo").AddItem(new MenuItem("UseRE", "团战连招自动R")).SetValue(true);
            _config.SubMenu("Combo")
                .AddItem(new MenuItem("MinTargets", "使用R|敌人最低数量").SetValue(new Slider(2, 1, 5)));
            _config.SubMenu("Combo")
                .AddItem(new MenuItem("ActiveCombo", "连招!").SetValue(new KeyBind(32, KeyBindType.Press)));
            _config.SubMenu("Combo")
                .AddItem(
                    new MenuItem("ActiveComboEQR", "使用 EQ-R!").SetValue(new KeyBind("T".ToCharArray()[0],
                        KeyBindType.Press)));
            _config.SubMenu("Combo")
                .AddItem(
                    new MenuItem("ComboeqFlash", "使用 EQ-闪现!").SetValue(new KeyBind("H".ToCharArray()[0],
                        KeyBindType.Press)));
            _config.SubMenu("Combo")
                .AddItem(new MenuItem("FlashDista", "闪现距离").SetValue(new Slider(700, 700, 1000)));

            //Items public static Int32 Tiamat = 3077, Hydra = 3074, Blade = 3153, Bilge = 3144, Rand = 3143, lotis = 3190;
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
            /*_config.SubMenu("items").AddSubMenu(new Menu("Potions", "Potions"));
            _config.SubMenu("items").SubMenu("Potions").AddItem(new MenuItem("Hppotion", "Use Hp potion")).SetValue(true);
            _config.SubMenu("items").SubMenu("Potions").AddItem(new MenuItem("Hppotionuse", "Use Hp potion if HP<").SetValue(new Slider(35, 1, 100)));
            _config.SubMenu("items").SubMenu("Potions").AddItem(new MenuItem("Mppotion", "Use Mp potion")).SetValue(true);
            _config.SubMenu("items").SubMenu("Potions").AddItem(new MenuItem("Mppotionuse", "Use Mp potion if HP<").SetValue(new Slider(35, 1, 100)));
            */

            //Harass
            _config.AddSubMenu(new Menu("骚扰", "Harass"));
            _config.SubMenu("Harass").AddItem(new MenuItem("UseQH", "使用 Q")).SetValue(true);
            _config.SubMenu("Harass").AddItem(new MenuItem("UseEH", "使用 E")).SetValue(true);
            _config.SubMenu("Harass").AddItem(new MenuItem("UseEQH", "使用 EQ 连招")).SetValue(true);
            _config.SubMenu("Harass")
                .AddItem(new MenuItem("UseEQHHP", "使用EQ|自己血量").SetValue(new Slider(85, 1, 100)));
            _config.SubMenu("Harass").AddItem(new MenuItem("UseItemsharass", "Use Tiamat/Hydra")).SetValue(true);
            _config.SubMenu("Harass")
                .AddItem(new MenuItem("harassmana", "骚扰最低蓝量").SetValue(new Slider(35, 1, 100)));
            _config.SubMenu("Harass")
                .AddItem(
                    new MenuItem("harasstoggle", "骚扰 (自动)").SetValue(new KeyBind("G".ToCharArray()[0],
                        KeyBindType.Toggle)));
            _config.SubMenu("Harass")
                .AddItem(
                    new MenuItem("ActiveHarass", "骚扰!").SetValue(new KeyBind("C".ToCharArray()[0], KeyBindType.Press)));

            //LaneClear
            _config.AddSubMenu(new Menu("清线|清野", "Farm"));
            _config.SubMenu("Farm").AddSubMenu(new Menu("清线", "LaneFarm"));
            _config.SubMenu("Farm")
                .SubMenu("LaneFarm")
                .AddItem(new MenuItem("UseItemslane", "使用 物品 清线"))
                .SetValue(true);
            _config.SubMenu("Farm").SubMenu("LaneFarm").AddItem(new MenuItem("UseQL", "Q 清线")).SetValue(true);
            _config.SubMenu("Farm").SubMenu("LaneFarm").AddItem(new MenuItem("UseEL", "E 清线")).SetValue(true);
            _config.SubMenu("Farm").SubMenu("LaneFarm").AddItem(new MenuItem("UseWL", "W 清线")).SetValue(true);
            _config.SubMenu("Farm")
                .SubMenu("LaneFarm")
                .AddItem(new MenuItem("UseWLHP", "使用W|血量大于").SetValue(new Slider(35, 1, 100)));
            _config.SubMenu("Farm")
                .SubMenu("LaneFarm")
                .AddItem(new MenuItem("lanemana", "清线最低蓝量").SetValue(new Slider(35, 1, 100)));
            _config.SubMenu("Farm")
                .SubMenu("LaneFarm")
                .AddItem(
                    new MenuItem("Activelane", "清线!").SetValue(new KeyBind("V".ToCharArray()[0], KeyBindType.Press)));

            _config.SubMenu("Farm").AddSubMenu(new Menu("补刀", "LastHit"));
            _config.SubMenu("Farm").SubMenu("LastHit").AddItem(new MenuItem("UseQLH", "Q 补刀")).SetValue(true);
            _config.SubMenu("Farm").SubMenu("LastHit").AddItem(new MenuItem("UseELH", "E 补刀")).SetValue(true);
            _config.SubMenu("Farm").SubMenu("LastHit").AddItem(new MenuItem("UseWLH", "W 清线")).SetValue(true);
            _config.SubMenu("Farm")
                .SubMenu("LastHit")
                .AddItem(new MenuItem("UseWLHHP", "使用W|血量大于").SetValue(new Slider(35, 1, 100)));
            _config.SubMenu("Farm")
                .SubMenu("LastHit")
                .AddItem(new MenuItem("lastmana", "补刀最低蓝量").SetValue(new Slider(35, 1, 100)));
            _config.SubMenu("Farm")
                .SubMenu("LastHit")
                .AddItem(
                    new MenuItem("ActiveLast", "补刀!").SetValue(new KeyBind("X".ToCharArray()[0], KeyBindType.Press)));

            _config.SubMenu("Farm").AddSubMenu(new Menu("清野", "Jungle"));
            _config.SubMenu("Farm")
                .SubMenu("Jungle")
                .AddItem(new MenuItem("UseItemsjungle", "使用物品清野"))
                .SetValue(true);

            _config.SubMenu("Farm").SubMenu("Jungle").AddItem(new MenuItem("UseQJ", "Q 清野")).SetValue(true);
            _config.SubMenu("Farm").SubMenu("Jungle").AddItem(new MenuItem("UseEJ", "E 清野")).SetValue(true);
            _config.SubMenu("Farm").SubMenu("Jungle").AddItem(new MenuItem("UseWJ", "W 清野")).SetValue(true);
            _config.SubMenu("Farm").SubMenu("Jungle").AddItem(new MenuItem(" UseEQJ", "EQ 清野")).SetValue(true);
            _config.SubMenu("Farm")
                .SubMenu("Jungle")
                .AddItem(new MenuItem("UseWJHP", "使用W|血量大于").SetValue(new Slider(35, 1, 100)));
            _config.SubMenu("Farm")
                .SubMenu("Jungle")
                .AddItem(new MenuItem("junglemana", "清野最低蓝量").SetValue(new Slider(35, 1, 100)));
            _config.SubMenu("Farm")
                .SubMenu("Jungle")
                .AddItem(
                    new MenuItem("ActiveJungle", "清野!").SetValue(new KeyBind("V".ToCharArray()[0], KeyBindType.Press)));

            //Smite ActiveJungle
            _config.AddSubMenu(new Menu("惩戒", "Smite"));
            _config.SubMenu("Smite")
                .AddItem(
                    new MenuItem("Usesmite", "使用 惩戒(自动)").SetValue(new KeyBind("H".ToCharArray()[0],
                        KeyBindType.Toggle)));
            _config.SubMenu("Smite").AddItem(new MenuItem("Useblue", "提前 惩戒蓝buff")).SetValue(true);
            _config.SubMenu("Smite")
                .AddItem(new MenuItem("manaJ", "提前惩戒蓝buff|蓝量少于").SetValue(new Slider(35, 1, 100)));
            _config.SubMenu("Smite").AddItem(new MenuItem("Usered", "提前 惩戒红buff")).SetValue(true);
            _config.SubMenu("Smite")
                .AddItem(new MenuItem("healthJ", "提前惩戒红buff|血量少于").SetValue(new Slider(35, 1, 100)));

            //Forest
            _config.AddSubMenu(new Menu("逃跑", "Forest Gump"));
            _config.SubMenu("Forest Gump").AddItem(new MenuItem("UseEQF", "向鼠标位置EQ")).SetValue(true);
            _config.SubMenu("Forest Gump").AddItem(new MenuItem("UseWF", "使用 W ")).SetValue(true);
            _config.SubMenu("Forest Gump")
                .AddItem(
                    new MenuItem("Forest", "启用 逃跑!").SetValue(new KeyBind("Z".ToCharArray()[0],
                        KeyBindType.Press)));


            //Misc
            _config.AddSubMenu(new Menu("杂项", "Misc"));
            _config.SubMenu("Misc").AddItem(new MenuItem("UseIgnitekill", "使用 点燃 击杀")).SetValue(true);
            _config.SubMenu("Misc").AddItem(new MenuItem("UseQM", "使用 Q 抢人头")).SetValue(true);
            _config.SubMenu("Misc").AddItem(new MenuItem("UseRM", "使用 R 抢人头")).SetValue(true);
            _config.SubMenu("Misc").AddItem(new MenuItem("Gap_W", "W 防止突进")).SetValue(true);
            _config.SubMenu("Misc").AddItem(new MenuItem("UseEQInt", "EQ 中断法术")).SetValue(true);
            // _config.SubMenu("Misc").AddItem(new MenuItem("MinTargetsgap", "min enemy >=(GapClosers)").SetValue(new Slider(2, 1, 5)));
            _config.SubMenu("Misc").AddItem(new MenuItem("skinjar", "使用 换肤").SetValue(false));
            _config.SubMenu("Misc").AddItem(new MenuItem("skinjarvan", "皮肤 选择").SetValue(new Slider(4, 1, 7)));
            _config.SubMenu("Misc").AddItem(new MenuItem("usePackets", "使用 封包")).SetValue(true);

            //Drawings
            _config.AddSubMenu(new Menu("范围", "Drawings"));
            _config.SubMenu("Drawings").AddItem(new MenuItem("DrawQ", "范围 Q")).SetValue(true);
            _config.SubMenu("Drawings").AddItem(new MenuItem("DrawW", "范围 W")).SetValue(true);
            _config.SubMenu("Drawings").AddItem(new MenuItem("DrawE", "范围 E")).SetValue(true);
            _config.SubMenu("Drawings").AddItem(new MenuItem("DrawR", "范围 R")).SetValue(true);
            _config.SubMenu("Drawings").AddItem(new MenuItem("DrawQR", "显示 EQ-R 范围")).SetValue(true);
            _config.SubMenu("Drawings").AddItem(new MenuItem("DrawEQF", "显示 EQ-闪现 范围")).SetValue(true);
            _config.SubMenu("Drawings").AddItem(new MenuItem("Drawsmite", "显示 惩戒 范围")).SetValue(true);
            _config.SubMenu("Drawings").AddItem(new MenuItem("CircleLag", "延迟自由圈").SetValue(true));
            _config.SubMenu("Drawings")
                .AddItem(new MenuItem("CircleQuality", "圈 质量").SetValue(new Slider(100, 100, 10)));
            _config.SubMenu("Drawings")
                .AddItem(new MenuItem("CircleThickness", "圈 厚度").SetValue(new Slider(1, 10, 1)));

            _config.AddToMainMenu();
            Game.PrintChat("<font color='#881df2'>D-鐜嬪瓙 by Diabaths</font> 鍔犺浇鎴愬姛!姹夊寲by浜岀嫍!QQ缇361630847.");
            Game.OnGameUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Obj_AI_Hero.OnCreate += OnCreateObj;
            Obj_AI_Hero.OnDelete += OnDeleteObj;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Interrupter.OnPossibleToInterrupt += Interrupter_OnPossibleToInterrupt;
            if (_config.Item("skinjar").GetValue<bool>())
            {
                GenModelPacket(_player.ChampionName, _config.Item("skinjarvan").GetValue<Slider>().Value);
                _lastSkin = _config.Item("skinjarvan").GetValue<Slider>().Value;
            }
            Game.PrintChat(
                "<font color='#FF0000'>鎹愯禒浣滆€卲aypal</font> <font color='#FF9900'>ssssssssssmith@hotmail.com</font> (10) S");
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {

            if (_config.Item("skinjar").GetValue<bool>() && SkinChanged())
            {
                GenModelPacket(_player.ChampionName, _config.Item("skinjarvan").GetValue<Slider>().Value);
                _lastSkin = _config.Item("skinjarvan").GetValue<Slider>().Value;
            }
            if (_config.Item("ActiveCombo").GetValue<KeyBind>().Active)
            {
                Combo();
            }
            if (_config.Item("ActiveComboEQR").GetValue<KeyBind>().Active)
            {
                ComboEqr();
            }
            if ((_config.Item("ActiveHarass").GetValue<KeyBind>().Active ||
                 _config.Item("harasstoggle").GetValue<KeyBind>().Active) &&
                (100*(_player.Mana/_player.MaxMana)) > _config.Item("harassmana").GetValue<Slider>().Value)
            {
                Harass();

            }
            if (_config.Item("Activelane").GetValue<KeyBind>().Active &&
                (100*(_player.Mana/_player.MaxMana)) > _config.Item("lanemana").GetValue<Slider>().Value)
            {
                Laneclear();
            }
            if (_config.Item("ActiveJungle").GetValue<KeyBind>().Active &&
                (100*(_player.Mana/_player.MaxMana)) > _config.Item("junglemana").GetValue<Slider>().Value)
            {
                JungleClear();
            }
            if (_config.Item("ActiveLast").GetValue<KeyBind>().Active &&
                (100*(_player.Mana/_player.MaxMana)) > _config.Item("lastmana").GetValue<Slider>().Value)
            {
                LastHit();
            }

            if (_config.Item("Forest").GetValue<KeyBind>().Active)
            {
                Forest();
            }
            if (_config.Item("Usesmite").GetValue<KeyBind>().Active)
            {
                Smiteuse();
            }
            if (_config.Item("ComboeqFlash").GetValue<KeyBind>().Active)
            {
                ComboeqFlash();
            }

            _player = ObjectManager.Player;

            _orbwalker.SetAttack(true);

            KillSteal();

        }

        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (_w.IsReady() && gapcloser.Sender.IsValidTarget(_w.Range) && _config.Item("Gap_W").GetValue<bool>())
            {
                _w.Cast(gapcloser.Sender, Packets());
            }
        }

        private static void Interrupter_OnPossibleToInterrupt(Obj_AI_Base unit, InterruptableSpell spell)
        {
            if (unit.IsValidTarget(_q.Range) && _config.Item("UseEQInt").GetValue<bool>())
            {
                if (_e.IsReady() && _q.IsReady())
                {
                    _e.Cast(unit, Packets());
                }
                if (_q.IsReady() && _epos != default(Vector3) && unit.IsValidTarget(200, true, _epos))
                {
                    _q.Cast(_epos, Packets());
                }
            }
        }

        private static void GenModelPacket(string champ, int skinId)
        {
            Packet.S2C.UpdateModel.Encoded(new Packet.S2C.UpdateModel.Struct(_player.NetworkId, skinId, champ))
                .Process();
        }

        private static bool SkinChanged()
        {
            return (_config.Item("skinjarvan").GetValue<Slider>().Value != _lastSkin);
        }

        private static float ComboDamage(Obj_AI_Base enemy)
        {
            var damage = 0d;
            if (_igniteSlot != SpellSlot.Unknown &&
                _player.SummonerSpellbook.CanUseSpell(_igniteSlot) == SpellState.Ready)
                damage += ObjectManager.Player.GetSummonerSpellDamage(enemy, Damage.SummonerSpell.Ignite);
            if (Items.HasItem(3077) && Items.CanUseItem(3077))
                damage += _player.GetItemDamage(enemy, Damage.DamageItems.Tiamat);
            if (Items.HasItem(3074) && Items.CanUseItem(3074))
                damage += _player.GetItemDamage(enemy, Damage.DamageItems.Hydra);
            if (Items.HasItem(3153) && Items.CanUseItem(3153))
                damage += _player.GetItemDamage(enemy, Damage.DamageItems.Botrk);
            if (Items.HasItem(3144) && Items.CanUseItem(3144))
                damage += _player.GetItemDamage(enemy, Damage.DamageItems.Bilgewater);
            if (_q.IsReady())
                damage += _player.GetSpellDamage(enemy, SpellSlot.Q)*2*1.2;
            if (_e.IsReady())
                damage += _player.GetSpellDamage(enemy, SpellSlot.E);
            if (_r.IsReady())
                damage += _player.GetSpellDamage(enemy, SpellSlot.R);

            damage += _player.GetAutoAttackDamage(enemy, true)*1.1;
            damage += _player.GetAutoAttackDamage(enemy, true);
            return (float) damage;
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

        private static void Combo()
        {
            var useQ = _config.Item("UseQC").GetValue<bool>();
            var useW = _config.Item("UseWC").GetValue<bool>();
            var useE = _config.Item("UseEC").GetValue<bool>();
            var useR = _config.Item("UseRC").GetValue<bool>();
            var autoR = _config.Item("UseRE").GetValue<bool>();
            var t = SimpleTs.GetTarget(_e.Range, SimpleTs.DamageType.Magical);
            Smiteontarget(t);
            if (t != null && _config.Item("UseIgnite").GetValue<bool>() && _igniteSlot != SpellSlot.Unknown &&
                _player.SummonerSpellbook.CanUseSpell(_igniteSlot) == SpellState.Ready)
            {
                if (ComboDamage(t) > t.Health)
                {
                    _player.SummonerSpellbook.CastSpell(_igniteSlot, t);
                }
            }
            if (useR && _r.IsReady())
            {
                if (t != null && !_haveulti)
                    if (!t.HasBuff("JudicatorIntervention") && !t.HasBuff("Undying Rage") &&
                        ComboDamage(t) > t.Health)
                        _r.CastIfHitchanceEquals(t, HitChance.Medium, Packets());
            }
            if (useE && _e.IsReady() && t.Distance(_player.Position) < _q.Range && _q.IsReady())
            {
                //xsalice Code
                var vec = t.ServerPosition - _player.ServerPosition;
                var castBehind = _e.GetPrediction(t).CastPosition + Vector3.Normalize(vec)*100;
                _e.Cast(castBehind, Packets());
            }
            if (useQ && t.Distance(_player.Position) < _q.Range && _q.IsReady() && _epos != default(Vector3) &&
                t.IsValidTarget(200, true, _epos))
            {
                _q.Cast(_epos, Packets());
            }

            if (useW && _w.IsReady())
            {
                if (t != null && t.Distance(_player.Position) < _w.Range)
                    _w.Cast();
            }
            if (useQ && _q.IsReady() && !_e.IsReady())
            {
                if (t != null && t.Distance(_player.Position) < _q.Range)
                    _q.Cast(t, Packets(), true);
            }
            if (_r.IsReady() && autoR && !_haveulti)
            {
                if (GetNumberHitByR(t) >=
                    _config.Item("MinTargets").GetValue<Slider>().Value)
                    _r.Cast(t, Packets(), true);
            }
            UseItemes(t);
        }
        private static int GetNumberHitByR(Obj_AI_Hero target)
        {
            int Enemys = 0;
            foreach (Obj_AI_Hero enemys in ObjectManager.Get<Obj_AI_Hero>())
            {
                var pred = _r.GetPrediction(enemys, true);
                if (pred.Hitchance >= HitChance.High && !enemys.IsMe && enemys.IsEnemy && Vector3.Distance(_player.Position, pred.UnitPosition) <= _r.Range)
                {
                    Enemys = Enemys + 1;
                }
            }
            return Enemys;
        }
        private static void ComboEqr()
        {
            var manacheck = _player.Mana >
                            _player.Spellbook.GetSpell(SpellSlot.Q).ManaCost +
                            _player.Spellbook.GetSpell(SpellSlot.E).ManaCost +
                            _player.Spellbook.GetSpell(SpellSlot.R).ManaCost;
            var t = SimpleTs.GetTarget(_q.Range + _r.Range, SimpleTs.DamageType.Magical);
            if (t == null)
            {
                _player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            }
            else
            {
                _player.IssueOrder(GameObjectOrder.AttackUnit, t);
            }
            Smiteontarget(t);
            if (_e.IsReady() && _q.IsReady() && manacheck)
            {
                if (t != null && _player.Distance(t) > _q.Range)
                    _e.Cast(t.ServerPosition, Packets());
                _q.Cast(t.ServerPosition, Packets());

            }
            if (t != null && _config.Item("UseIgnite").GetValue<bool>() && _igniteSlot != SpellSlot.Unknown &&
                _player.SummonerSpellbook.CanUseSpell(_igniteSlot) == SpellState.Ready)
            {
                if (ComboDamage(t) > t.Health)
                {
                    _player.SummonerSpellbook.CastSpell(_igniteSlot, t);
                }
            }
            if (_r.IsReady() && !_haveulti && t != null)
            {
               _r.CastIfHitchanceEquals(t, HitChance.Immobile, Packets());
            }
            if (_w.IsReady())
            {
                if (t != null && t.Distance(_player.Position) < _w.Range)
                    _w.Cast();
            }
            UseItemes(t);
           }

        private static void Harass()
        {
            var target = SimpleTs.GetTarget(_e.Range, SimpleTs.DamageType.Magical);
            var useQ = _config.Item("UseQH").GetValue<bool>();
            var useE = _config.Item("UseEH").GetValue<bool>();
            var useEq = _config.Item("UseEQH").GetValue<bool>();
            var useEqhp = (100*(_player.Health/_player.MaxHealth)) > _config.Item("UseEQHHP").GetValue<Slider>().Value;
            var useItemsH = _config.Item("UseItemsharass").GetValue<bool>();
            if (useEqhp && useEq && _q.IsReady() && _e.IsReady())
            {
                var t = SimpleTs.GetTarget(_e.Range, SimpleTs.DamageType.Magical);
                if (t != null && t.Distance(_player.Position) < _e.Range)
                    _e.Cast(t, Packets());
                _q.Cast(t, Packets());
            }
            if (useQ && _q.IsReady())
            {
                var t = SimpleTs.GetTarget(_e.Range, SimpleTs.DamageType.Magical);
                if (t != null && t.Distance(_player.Position) < _q.Range)
                    _q.Cast(t, Packets());
            }
            if (useE && _e.IsReady())
            {
                var t = SimpleTs.GetTarget(_e.Range, SimpleTs.DamageType.Magical);
                if (t != null && t.Distance(_player.Position) < _e.Range)
                    _e.Cast(t, Packets());
            }

            if (useItemsH && _tiamat.IsReady() && target.Distance(_player.Position) < _tiamat.Range)
            {
                _tiamat.Cast();
            }
            if (useItemsH && _hydra.IsReady() && target.Distance(_player.Position) < _hydra.Range)
            {
                _hydra.Cast();
            }
        }

        private static void ComboeqFlash()
        {
            var flashDista = _config.Item("FlashDista").GetValue<Slider>().Value;
            var manacheck = _player.Mana >
                            _player.Spellbook.GetSpell(SpellSlot.Q).ManaCost +
                            _player.Spellbook.GetSpell(SpellSlot.E).ManaCost;
            var t = SimpleTs.GetTarget(_q.Range + 800, SimpleTs.DamageType.Magical);
            if (t == null)
            {
                _player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            }
            else
            {
                _player.IssueOrder(GameObjectOrder.AttackUnit, t);
            }
            Smiteontarget(t);
            if (_flashSlot != SpellSlot.Unknown && _player.SummonerSpellbook.CanUseSpell(_flashSlot) == SpellState.Ready)
            {
                if (_e.IsReady() && _q.IsReady() && manacheck && t != null && _player.Distance(t) > _q.Range)
                {
                    _e.Cast(Game.CursorPos, Packets());
                }
                if (_epos != default(Vector3) && _q.InRange(_epos))
                {
                    _q.Cast(_epos, Packets());
                }

                if (t.IsValidTarget(flashDista) && !_q.IsReady())
                {
                    _player.SummonerSpellbook.CastSpell(_flashSlot, t.ServerPosition);
                }
            }
            UseItemes(t);
        }

        private static void Laneclear()
        {
            var allMinionsQ = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, _q.Range, MinionTypes.All);
            var rangedMinionsQ = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, _q.Range + _q.Width,
                MinionTypes.Ranged);
            var rangedMinionsE = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, _e.Range + _e.Width,
                MinionTypes.Ranged);
            var allMinionsE = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, _e.Range + _e.Width,
                MinionTypes.All);
            var useItemsl = _config.Item("UseItemslane").GetValue<bool>();
            var useQl = _config.Item("UseQL").GetValue<bool>();
            var useEl = _config.Item("UseEL").GetValue<bool>();
            var useWl = _config.Item("UseWL").GetValue<bool>();
            var usewhp = (100*(_player.Health/_player.MaxHealth)) < _config.Item("UseWLHP").GetValue<Slider>().Value;

            if (_q.IsReady() && useQl)
            {
                var fl1 = _q.GetLineFarmLocation(rangedMinionsQ, _q.Width);
                var fl2 = _q.GetLineFarmLocation(allMinionsQ, _q.Width);

                if (fl1.MinionsHit >= 3)
                {
                    _q.Cast(fl1.Position);
                }
                else if (fl2.MinionsHit >= 2 || allMinionsQ.Count == 1)
                {
                    _q.Cast(fl2.Position);
                }
                else
                    foreach (var minion in allMinionsQ)
                        if (!Orbwalking.InAutoAttackRange(minion) &&
                            minion.Health < 0.75*_player.GetSpellDamage(minion, SpellSlot.Q))
                            _q.Cast(minion);
            }

            if (_e.IsReady() && useEl)
            {
                var fl1 = _e.GetCircularFarmLocation(rangedMinionsE, _e.Width);
                var fl2 = _e.GetCircularFarmLocation(allMinionsE, _e.Width);

                if (fl1.MinionsHit >= 3)
                {
                    _e.Cast(fl1.Position);
                }
                else if (fl2.MinionsHit >= 2 || allMinionsE.Count == 1)
                {
                    _e.Cast(fl2.Position);
                }
                else
                    foreach (var minion in allMinionsE)
                        if (!Orbwalking.InAutoAttackRange(minion) &&
                            minion.Health < 0.75*_player.GetSpellDamage(minion, SpellSlot.E))
                            _e.Cast(minion);
            }
            if (usewhp && useWl && _w.IsReady() && allMinionsQ.Count > 0)
            {
                _w.Cast();

            }
            foreach (var minion in allMinionsQ)
            {
                if (useItemsl && _tiamat.IsReady() && _player.Distance(minion) < _tiamat.Range)
                {
                    _tiamat.Cast();
                }
                if (useItemsl && _hydra.IsReady() && _player.Distance(minion) < _hydra.Range)
                {
                    _hydra.Cast();
                }
            }
        }

        private static void LastHit()
        {
            var allMinions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, _q.Range, MinionTypes.All);
            var useQ = _config.Item("UseQLH").GetValue<bool>();
            var useW = _config.Item("UseWLH").GetValue<bool>();
            var useE = _config.Item("UseELH").GetValue<bool>();
            var usewhp = (100*(_player.Health/_player.MaxHealth)) < _config.Item("UseWLHHP").GetValue<Slider>().Value;
            foreach (var minion in allMinions)
            {
                if (useQ && _q.IsReady() && _player.Distance(minion) < _q.Range &&
                    minion.Health < 0.95*_player.GetSpellDamage(minion, SpellSlot.Q))
                {
                    _q.Cast(minion, Packets());
                }

                if (_e.IsReady() && useE && _player.Distance(minion) < _e.Range &&
                    minion.Health < 0.95*_player.GetSpellDamage(minion, SpellSlot.E))
                {
                    _e.Cast(minion, Packets());
                }
                if (usewhp && useW && _w.IsReady() && allMinions.Count > 0)
                {
                    _w.Cast();

                }
            }
        }

        private static void JungleClear()
        {
            var mobs = MinionManager.GetMinions(_player.ServerPosition, _q.Range,
                MinionTypes.All,
                MinionTeam.Neutral, MinionOrderTypes.MaxHealth);
            var useItemsJ = _config.Item("UseItemsjungle").GetValue<bool>();
            var useQ = _config.Item("UseQJ").GetValue<bool>();
            var useW = _config.Item("UseWJ").GetValue<bool>();
            var useE = _config.Item("UseEJ").GetValue<bool>();
            var useEq = _config.Item(" UseEQJ").GetValue<bool>();
            var usewhp = (100*(_player.Health/_player.MaxHealth)) < _config.Item("UseWJHP").GetValue<Slider>().Value;

            if (mobs.Count > 0)
            {
                var mob = mobs[0];
                if (useEq)
                {
                    if (_e.IsReady() && useE && _player.Distance(mob) < _q.Range)
                    {
                        _e.Cast(mob, Packets());
                    }
                    if (useQ && _q.IsReady() && _player.Distance(mob) < _q.Range)
                    {
                        _q.Cast(mob, Packets());
                    }
                }
                else
                {
                    if (useQ && _q.IsReady() && _player.Distance(mob) < _q.Range)
                    {
                        _q.Cast(mob, Packets());
                    }
                    if (_e.IsReady() && useE && _player.Distance(mob) < _q.Range)
                    {
                        _e.Cast(mob, Packets());
                    }
                }
                if (_w.IsReady() && useW && usewhp && _player.Distance(mob) < _w.Range)
                {
                    _w.Cast();
                }
                if (useItemsJ && _tiamat.IsReady() && _player.Distance(mob) < _tiamat.Range)
                {
                    _tiamat.Cast();
                }
                if (useItemsJ && _hydra.IsReady() && _player.Distance(mob) < _hydra.Range)
                {
                    _hydra.Cast();
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

        private static bool Packets()
        {
            return _config.Item("usePackets").GetValue<bool>();
        }

        private static int GetSmiteDmg()
        {
            int level = _player.Level;
            int index = _player.Level/5;
            float[] dmgs = {370 + 20*level, 330 + 30*level, 240 + 40*level, 100 + 50*level};
            return (int) dmgs[index];
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
        private static void UseItemes(Obj_AI_Hero target)
        {
            var iBilge = _config.Item("Bilge").GetValue<bool>();
            var iBilgeEnemyhp = target.Health <=
                                (target.MaxHealth*(_config.Item("BilgeEnemyhp").GetValue<Slider>().Value)/100);
            var iBilgemyhp = _player.Health <=
                             (_player.MaxHealth*(_config.Item("Bilgemyhp").GetValue<Slider>().Value)/100);
            var iBlade = _config.Item("Blade").GetValue<bool>();
            var iBladeEnemyhp = target.Health <=
                                (target.MaxHealth*(_config.Item("BladeEnemyhp").GetValue<Slider>().Value)/100);
            var iBlademyhp = _player.Health <=
                             (_player.MaxHealth*(_config.Item("Blademyhp").GetValue<Slider>().Value)/100);
            var iOmen = _config.Item("Omen").GetValue<bool>();
            var iOmenenemys = ObjectManager.Get<Obj_AI_Hero>().Count(hero => hero.IsValidTarget(450)) >=
                              _config.Item("Omenenemys").GetValue<Slider>().Value;
            var iTiamat = _config.Item("Tiamat").GetValue<bool>();
            var iHydra = _config.Item("Hydra").GetValue<bool>();
            var ilotis = _config.Item("lotis").GetValue<bool>();
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
                    if (hero.Health <= (hero.MaxHealth*(_config.Item("lotisminhp").GetValue<Slider>().Value)/100) &&
                        hero.Distance(_player.ServerPosition) <= _lotis.Range && _lotis.IsReady())
                        _lotis.Cast();
                }
            }
        }

        private static void KillSteal()
        {
            var target = SimpleTs.GetTarget(_q.Range, SimpleTs.DamageType.Magical);
            var igniteDmg = _player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
            if (target != null && _config.Item("UseIgnitekill").GetValue<bool>() && _igniteSlot != SpellSlot.Unknown &&
                _player.SummonerSpellbook.CanUseSpell(_igniteSlot) == SpellState.Ready)
            {
                if (igniteDmg > target.Health)
                {
                    _player.SummonerSpellbook.CastSpell(_igniteSlot, target);
                }
            }
            if (_q.IsReady() && _config.Item("UseQM").GetValue<bool>())
            {
                var t = SimpleTs.GetTarget(_q.Range, SimpleTs.DamageType.Magical);
                if (_q.GetDamage(t) > t.Health && _player.Distance(t) <= _q.Range)
                {
                    _q.Cast(t, Packets());
                }
            }
            if (_r.IsReady() && _config.Item("UseRM").GetValue<bool>())
            {
                var t = SimpleTs.GetTarget(_r.Range, SimpleTs.DamageType.Magical);
                if (t != null)
                    if (!t.HasBuff("JudicatorIntervention") && !t.HasBuff("Undying Rage") && _r.GetDamage(t) > t.Health)
                        _r.Cast(t, Packets(), true);
            }
        }

        private static void Forest()
        {
            var manacheck = _player.Mana >
                            _player.Spellbook.GetSpell(SpellSlot.Q).ManaCost +
                            _player.Spellbook.GetSpell(SpellSlot.E).ManaCost;
            _player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            var target = SimpleTs.GetTarget(_e.Range, SimpleTs.DamageType.Magical);

            if (_config.Item("UseEQF").GetValue<bool>() && _q.IsReady() && _e.IsReady() && manacheck)
            {
                _e.Cast(Game.CursorPos, Packets());
                _q.Cast(Game.CursorPos, Packets());
            }
            if (_config.Item("UseWF").GetValue<bool>() && _w.IsReady() && target != null &&
                _player.Distance(target) < _w.Range)
            {
                _w.Cast();
            }

        }

        private static void OnCreateObj(GameObject sender, EventArgs args)
        {
            if (!(sender is Obj_GeneralParticleEmmiter)) return;
            var obj = (Obj_GeneralParticleEmmiter) sender;
            if (sender.Name == "JarvanDemacianStandard_buf_green.troy")
            {
                _epos = sender.Position;
            }
            if (obj != null && obj.IsMe && obj.Name == "JarvanCataclysm_tar")

                //debug
                //if (unit == ObjectManager.Player.Name)
            {
                // Game.PrintChat("Spell: " + name);
                _haveulti = true;
                return;
            }
        }

        private static void OnDeleteObj(GameObject sender, EventArgs args)
        {
            if (!(sender is Obj_GeneralParticleEmmiter)) return;
            if (sender.Name == "JarvanDemacianStandard_buf_green.troy")
            {
                _epos = default(Vector3);
            }
            var obj = (Obj_GeneralParticleEmmiter) sender;
            if (obj != null && obj.IsMe && obj.Name == "JarvanCataclysm_tar")
            {
                _haveulti = false;
                return;
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (_config.Item("Drawsmite").GetValue<bool>())
            {
                if (_config.Item("Usesmite").GetValue<KeyBind>().Active)
                {
                    Drawing.DrawText(Drawing.Width*0.90f, Drawing.Height*0.68f, System.Drawing.Color.DarkOrange,
                        "Smite Is On");
                }
                else
                    Drawing.DrawText(Drawing.Width*0.90f, Drawing.Height*0.68f, System.Drawing.Color.DarkRed,
                        "Smite Is Off");
            }
            if (_config.Item("CircleLag").GetValue<bool>())
            {
                if (_config.Item("DrawEQF").GetValue<bool>())
                {
                    Utility.DrawCircle(ObjectManager.Player.Position,
                        _q.Range + _config.Item("FlashDista").GetValue<Slider>().Value, System.Drawing.Color.Gray,
                        _config.Item("CircleThickness").GetValue<Slider>().Value,
                        _config.Item("CircleQuality").GetValue<Slider>().Value);
                }
                if (_config.Item("DrawQ").GetValue<bool>())
                {
                    Utility.DrawCircle(ObjectManager.Player.Position, _q.Range, System.Drawing.Color.Gray,
                        _config.Item("CircleThickness").GetValue<Slider>().Value,
                        _config.Item("CircleQuality").GetValue<Slider>().Value);
                }
                if (_config.Item("DrawW").GetValue<bool>())
                {
                    Utility.DrawCircle(ObjectManager.Player.Position, _w.Range, System.Drawing.Color.Gray,
                        _config.Item("CircleThickness").GetValue<Slider>().Value,
                        _config.Item("CircleQuality").GetValue<Slider>().Value);
                }
                if (_config.Item("DrawE").GetValue<bool>())
                {
                    Utility.DrawCircle(ObjectManager.Player.Position, _e.Range, System.Drawing.Color.Gray,
                        _config.Item("CircleThickness").GetValue<Slider>().Value,
                        _config.Item("CircleQuality").GetValue<Slider>().Value);
                }
                if (_config.Item("DrawR").GetValue<bool>())
                {
                    Utility.DrawCircle(ObjectManager.Player.Position, _r.Range, System.Drawing.Color.Gray,
                        _config.Item("CircleThickness").GetValue<Slider>().Value,
                        _config.Item("CircleQuality").GetValue<Slider>().Value);
                }
                if (_config.Item("DrawQR").GetValue<bool>())
                {
                    Utility.DrawCircle(ObjectManager.Player.Position, _q.Range + _r.Range, System.Drawing.Color.Gray,
                        _config.Item("CircleThickness").GetValue<Slider>().Value,
                        _config.Item("CircleQuality").GetValue<Slider>().Value);
                }
            }
            else
            {
                if (_config.Item("DrawQ").GetValue<bool>())
                {
                    Drawing.DrawCircle(ObjectManager.Player.Position, _q.Range, System.Drawing.Color.White);
                }
                if (_config.Item("DrawW").GetValue<bool>())
                {
                    Drawing.DrawCircle(ObjectManager.Player.Position, _w.Range, System.Drawing.Color.White);
                }
                if (_config.Item("DrawE").GetValue<bool>())
                {
                    Drawing.DrawCircle(ObjectManager.Player.Position, _e.Range, System.Drawing.Color.White);
                }

                if (_config.Item("DrawR").GetValue<bool>())
                {
                    Drawing.DrawCircle(ObjectManager.Player.Position, _r.Range, System.Drawing.Color.White);
                }
                if (_config.Item("DrawQR").GetValue<bool>())
                {
                    Drawing.DrawCircle(ObjectManager.Player.Position, _q.Range + _r.Range, System.Drawing.Color.White);
                }
                if (_config.Item("DrawEQF").GetValue<bool>())
                {
                    Drawing.DrawCircle(ObjectManager.Player.Position,
                        _q.Range + _config.Item("FlashDista").GetValue<Slider>().Value, System.Drawing.Color.White);
                }
            }
        }
    }
}
     
  
 





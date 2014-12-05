using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using LeagueSharp;
using LeagueSharp.Common;
using Color = System.Drawing.Color;

namespace D_Diana
{
    internal class Program
    {
        private const string ChampionName = "Diana";

        private static Orbwalking.Orbwalker _orbwalker;

        private static Spell _q, _w, _e, _r;

        private static Obj_SpellMissile _qpos;

        private static bool _qcreated = false;

        private static Menu _config;

        public static Menu TargetSelectorMenu;

        private static Items.Item _dfg;

        private static Obj_AI_Hero _player;

        private static readonly List<Spell> SpellList = new List<Spell>();

        private static SpellSlot _igniteSlot;

        private static Items.Item _tiamat, _hydra, _blade, _bilge, _rand, _lotis;
        private static SpellSlot _smiteSlot = SpellSlot.Unknown;

        private static Spell _smite;
        //Credits to Kurisu
        private static readonly int[] SmitePurple = { 3713, 3726, 3725, 3726, 3723 };
        private static readonly int[] SmiteGrey = { 3711, 3722, 3721, 3720, 3719 };
        private static readonly int[] SmiteRed = { 3715, 3718, 3717, 3716, 3714 };
        private static readonly int[] SmiteBlue = { 3706, 3710, 3709, 3708, 3707 };
        private static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        private static void Game_OnGameLoad(EventArgs args)
        {
            _player = ObjectManager.Player;
            if (ObjectManager.Player.BaseSkinName != ChampionName) return;

            _q = new Spell(SpellSlot.Q, 830f);
            _w = new Spell(SpellSlot.W, 200f);
            _e = new Spell(SpellSlot.E, 420f);
            _r = new Spell(SpellSlot.R, 825f);

            _q.SetSkillshot(0.35f, 200f, 1800, false, SkillshotType.SkillshotCircle);

            SpellList.Add(_q);
            SpellList.Add(_w);
            SpellList.Add(_e);
            SpellList.Add(_r);

            _bilge = new Items.Item(3144, 475f);
            _blade = new Items.Item(3153, 425f);
            _hydra = new Items.Item(3074, 250f);
            _tiamat = new Items.Item(3077, 250f);
            _rand = new Items.Item(3143, 490f);
            _lotis = new Items.Item(3190, 590f);
            _dfg = new Items.Item(3128, 750f);

            _igniteSlot = _player.GetSpellSlot("SummonerDot");
            SetSmiteSlot();

            //D Diana
            _config = new Menu("D-皎月", "D-Diana", true);

            //TargetSelector
            TargetSelectorMenu = new Menu("目标 选择", "Target Selector");
            SimpleTs.AddToMenu(TargetSelectorMenu);
            _config.AddSubMenu(TargetSelectorMenu);

            //Orbwalker
            _config.AddSubMenu(new Menu("走砍", "Orbwalking"));
            _orbwalker = new Orbwalking.Orbwalker(_config.SubMenu("Orbwalking"));

            //Combo
            _config.AddSubMenu(new Menu("连招", "Combo"));
            _config.SubMenu("Combo").AddItem(new MenuItem("UseQCombo", "使用 Q")).SetValue(true);
            _config.SubMenu("Combo").AddItem(new MenuItem("UseWCombo", "使用 W")).SetValue(true);
            _config.SubMenu("Combo").AddItem(new MenuItem("UseECombo", "使用 E")).SetValue(true);
            _config.SubMenu("Combo").AddItem(new MenuItem("UseRCombo", "使用 R")).SetValue(true);
            _config.SubMenu("Combo").AddItem(new MenuItem("UseRSecond", "使用二段R（月光被动）")).SetValue(false);
            _config.SubMenu("Combo").AddItem(new MenuItem("UseItems", "使用 冥火")).SetValue(true);
            _config.SubMenu("Combo").AddItem(new MenuItem("smitecombo", "对目标使用惩戒")).SetValue(true);
            _config.SubMenu("Combo")
                .AddItem(new MenuItem("ActiveCombo", "连招!").SetValue(new KeyBind(32, KeyBindType.Press)));
            //_config.SubMenu("Combo").AddItem(new MenuItem("ActiveCombo2", "Combo2!").SetValue(new KeyBind(32, KeyBindType.Press)));


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

            _config.AddSubMenu(new Menu("骚扰", "Harass"));
            _config.SubMenu("Harass").AddItem(new MenuItem("UseQHarass", "使用 Q")).SetValue(true);
            _config.SubMenu("Harass").AddItem(new MenuItem("UseWHarass", "使用 W")).SetValue(true);
            _config.SubMenu("Harass")
                .AddItem(
                    new MenuItem("ActiveHarass", "骚扰 键位").SetValue(new KeyBind("X".ToCharArray()[0],
                        KeyBindType.Press)));
            _config.SubMenu("Harass")
                .AddItem(
                    new MenuItem("harasstoggle", "骚扰(自动)").SetValue(new KeyBind("G".ToCharArray()[0],
                        KeyBindType.Toggle)));
            _config.SubMenu("Harass")
                .AddItem(new MenuItem("Harrasmana", "骚扰最低蓝量").SetValue(new Slider(60, 1, 100)));

            _config.AddSubMenu(new Menu("清线|清野", "Farm"));
            _config.SubMenu("Farm").AddSubMenu(new Menu("补刀", "LastHit"));
            _config.SubMenu("Farm").SubMenu("LastHit").AddItem(new MenuItem("UseQLH", "Q 补刀")).SetValue(true);
            _config.SubMenu("Farm").SubMenu("LastHit").AddItem(new MenuItem("UseWLH", "W 清线")).SetValue(true);
            _config.SubMenu("Farm")
                .SubMenu("LastHit")
                .AddItem(new MenuItem("lastmana", "补刀最低蓝量").SetValue(new Slider(35, 1, 100)));
            _config.SubMenu("Farm")
                .SubMenu("LastHit")
                .AddItem(
                    new MenuItem("ActiveLast", "补刀!").SetValue(new KeyBind("X".ToCharArray()[0], KeyBindType.Press)));

            _config.SubMenu("Farm").AddSubMenu(new Menu("清线", "Lane"));
            _config.SubMenu("Farm").SubMenu("Lane").AddItem(new MenuItem("UseQLane", "使用 Q")).SetValue(true);
            _config.SubMenu("Farm").SubMenu("Lane").AddItem(new MenuItem("UseWLane", "使用 W")).SetValue(true);
            _config.SubMenu("Farm")
                .SubMenu("Lane")
                .AddItem(
                    new MenuItem("ActiveLane", "清线 键位").SetValue(new KeyBind("V".ToCharArray()[0], KeyBindType.Press)));
            _config.SubMenu("Farm")
                .SubMenu("Lane")
                .AddItem(new MenuItem("Lanemana", "清线最低蓝量").SetValue(new Slider(60, 1, 100)));

            //jungle
            _config.SubMenu("Farm").AddSubMenu(new Menu("清野", "Jungle"));
            _config.SubMenu("Farm").SubMenu("Jungle").AddItem(new MenuItem("UseQJungle", "使用 Q")).SetValue(true);
            _config.SubMenu("Farm").SubMenu("Jungle").AddItem(new MenuItem("UseWJungle", "使用 W")).SetValue(true);
            _config.SubMenu("Farm")
                .SubMenu("Jungle")
                .AddItem(
                    new MenuItem("ActiveJungle", "清野键位").SetValue(new KeyBind("V".ToCharArray()[0],
                        KeyBindType.Press)));
            _config.SubMenu("Farm")
                .SubMenu("Jungle")
                .AddItem(new MenuItem("Junglemana", "清野最低蓝量").SetValue(new Slider(60, 1, 100)));

            //Smite 
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

            //Extra
            _config.AddSubMenu(new Menu("杂项", "Misc"));
            _config.SubMenu("Misc").AddItem(new MenuItem("usePackets", "使用 封包")).SetValue(true);
            _config.SubMenu("Misc").AddItem(new MenuItem("AutoShield", "自动 W")).SetValue(true);
            _config.SubMenu("Misc").AddItem(new MenuItem("Shieldper", "自动W保护|血量低于")).SetValue(new Slider(40, 1, 100));
            _config.SubMenu("Misc")
                .AddItem(
                    new MenuItem("Escape", "逃跑 键位!").SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press)));
            _config.SubMenu("Misc").AddItem(new MenuItem("Inter_E", "使用E 中断法术")).SetValue(true);
            _config.SubMenu("Misc").AddItem(new MenuItem("Gap_W", "使用W 防止突进")).SetValue(true);

            //Kill Steal
            _config.AddSubMenu(new Menu("抢人头", "Ks"));
            _config.SubMenu("Ks").AddItem(new MenuItem("ActiveKs", "使用 抢人头")).SetValue(true);
            _config.SubMenu("Ks").AddItem(new MenuItem("UseQKs", "使用Q")).SetValue(true);
            _config.SubMenu("Ks").AddItem(new MenuItem("UseRKs", "使用 R")).SetValue(true);
            _config.SubMenu("Ks")
                .AddItem(new MenuItem("TargetRange", "使用 R 范围").SetValue(new Slider(400, 200, 600)));
            _config.SubMenu("Ks").AddItem(new MenuItem("UseIgnite", "使用 点燃")).SetValue(true);

            //Damage after combo:
            MenuItem dmgAfterComboItem = new MenuItem("DamageAfterCombo", "显示组合连招伤害").SetValue(true);
            Utility.HpBarDamageIndicator.DamageToUnit = ComboDamage;
            Utility.HpBarDamageIndicator.Enabled = dmgAfterComboItem.GetValue<bool>();
            dmgAfterComboItem.ValueChanged +=
                delegate(object sender, OnValueChangeEventArgs eventArgs)
                {
                    Utility.HpBarDamageIndicator.Enabled = eventArgs.GetNewValue<bool>();
                };

            //Drawings
            _config.AddSubMenu(new Menu("范围", "Drawings"));
            _config.SubMenu("Drawings").AddItem(new MenuItem("DrawQ", "范围 Q")).SetValue(true);
            _config.SubMenu("Drawings").AddItem(new MenuItem("DrawW", "范围 W")).SetValue(true);
            _config.SubMenu("Drawings").AddItem(new MenuItem("DrawE", "范围 E")).SetValue(true);
            _config.SubMenu("Drawings").AddItem(new MenuItem("DrawR", "范围 R")).SetValue(true);
            _config.SubMenu("Drawings").AddItem(dmgAfterComboItem);
            _config.SubMenu("Drawings").AddItem(new MenuItem("Drawsmite", "显示 惩戒 范围")).SetValue(true);
            _config.SubMenu("Drawings").AddItem(new MenuItem("ShowPassive", "显示 被动")).SetValue(true);
            _config.SubMenu("Drawings").AddItem(new MenuItem("CircleLag", "延迟自由圈").SetValue(true));
            _config.SubMenu("Drawings")
                .AddItem(new MenuItem("CircleQuality", "圈 质量").SetValue(new Slider(100, 100, 10)));
            _config.SubMenu("Drawings")
                .AddItem(new MenuItem("CircleThickness", "圈 厚度").SetValue(new Slider(1, 10, 1)));

            _config.AddToMainMenu();

            new AssassinManager();
            Game.OnGameUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            GameObject.OnCreate += OnCreate;
            GameObject.OnDelete += OnDelete;
            Game.PrintChat("<font color='#881df2'>鐨庢湀 By Diabaths 浣跨敤鑻ラ杩炴嫑缁勫悎 by xSalice </font>鍔犺浇鎴愬姛!姹夊寲by浜岀嫍!QQ缇361630847!");
            Game.PrintChat(
                "<font color='#FF0000'>鎹愯禒浣滆€卲aypal</font> <font color='#FF9900'>ssssssssssmith@hotmail.com</font> (10) S");

            // Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            Interrupter.OnPossibleToInterrupt += Interrupter_OnPossibleToInterrupt;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            _player = ObjectManager.Player;

            _orbwalker.SetAttack(true);
            if (_config.Item("Usesmite").GetValue<KeyBind>().Active)
            {
                Smiteuse();
            }
            if (_config.Item("ActiveLast").GetValue<KeyBind>().Active &&
                (100 * (_player.Mana / _player.MaxMana)) > _config.Item("lastmana").GetValue<Slider>().Value)
            {
                LastHit();
            }
            if (_config.Item("ActiveCombo").GetValue<KeyBind>().Active)
            {
                  int assassinRange = TargetSelectorMenu.Item("AssassinSearchRange").GetValue<Slider>().Value;

                IEnumerable<Obj_AI_Hero> xEnemy = ObjectManager.Get<Obj_AI_Hero>()
                    .Where(
                        enemy =>
                            enemy.Team != ObjectManager.Player.Team && !enemy.IsDead && enemy.IsVisible &&
                            TargetSelectorMenu.Item("Assassin" + enemy.ChampionName) != null &&
                            TargetSelectorMenu.Item("Assassin" + enemy.ChampionName).GetValue<bool>() &&
                            ObjectManager.Player.Distance(enemy) < assassinRange);

                Obj_AI_Hero[] objAiHeroes = xEnemy as Obj_AI_Hero[] ?? xEnemy.ToArray();

                if (objAiHeroes.Length > 2)
                {
                    Game.PrintChat(objAiHeroes[0].Distance(objAiHeroes[1]).ToString());
                }

                Obj_AI_Hero t = !objAiHeroes.Any()
                    ? SimpleTs.GetTarget(_q.Range, SimpleTs.DamageType.Magical)
                    : objAiHeroes[0];
               
                Misaya(t);
                //Misaya();
            }
            if ((_config.Item("ActiveHarass").GetValue<KeyBind>().Active ||
                 _config.Item("harasstoggle").GetValue<KeyBind>().Active) &&
                (100 * (_player.Mana / _player.MaxMana)) > _config.Item("Harrasmana").GetValue<Slider>().Value)
            {
                Harass();
            }
            if (_config.Item("ActiveLane").GetValue<KeyBind>().Active &&
                (100 * (_player.Mana / _player.MaxMana)) > _config.Item("Lanemana").GetValue<Slider>().Value)
            {
                Farm();
            }
            if (_config.Item("ActiveJungle").GetValue<KeyBind>().Active &&
                (100 * (_player.Mana / _player.MaxMana)) > _config.Item("Junglemana").GetValue<Slider>().Value)
            {
                JungleClear();
            }
            if (_config.Item("Escape").GetValue<KeyBind>().Active)
            {
                Tragic();
            }
            if (_config.Item("ActiveKs").GetValue<bool>())
            {
                KillSteal();
            }
            if (_config.Item("AutoShield").GetValue<bool>() && !_config.Item("ActiveCombo").GetValue<KeyBind>().Active)
            {
                AutoW();
            }
        }

        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (_w.IsReady() && gapcloser.Sender.IsValidTarget(_w.Range) && _config.Item("Gap_W").GetValue<bool>())
            {
                _w.Cast();
            }
        }

        private static void Interrupter_OnPossibleToInterrupt(Obj_AI_Base unit, InterruptableSpell spell)
        {
            if (_e.IsReady() && unit.IsValidTarget(_e.Range) && _config.Item("Inter_E").GetValue<bool>())
                _e.Cast();
        }

        private static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                Game.PrintChat("Spell name: " + args.SData.Name.ToString());
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

        //misaya combo by xSalice
        private static void Misaya(Obj_AI_Hero t)
        {
//            var target = SimpleTs.GetTarget(_q.Range, SimpleTs.DamageType.Magical);
            var target = t;
//            if (target != null)
//            {
                Smiteontarget(target);
                if (_player.Distance(target) <= _dfg.Range && _config.Item("UseItems").GetValue<bool>() &&
                    _dfg.IsReady() && target.Health <= ComboDamage(target))
                {
                    _dfg.Cast(target);
                }
                if (_igniteSlot != SpellSlot.Unknown &&
                    _player.SummonerSpellbook.CanUseSpell(_igniteSlot) == SpellState.Ready)
                {
                    if (target.Health <= ComboDamage(target))
                    {
                        _player.SummonerSpellbook.CastSpell(_igniteSlot, target);
                    }
                }
                if (_player.Distance(target) <= _q.Range && _config.Item("UseQCombo").GetValue<bool>() && _q.IsReady() &&
                    _q.GetPrediction(target).Hitchance >= HitChance.High)
                {
                    _q.CastIfHitchanceEquals(target, HitChance.High, Packets());
                }
                if (_player.Distance(target) <= _r.Range && _config.Item("UseRCombo").GetValue<bool>() && _r.IsReady() &&
                    ((_qcreated == true)
                     || target.HasBuff("dianamoonlight", true)))
                {
                    _r.Cast(target, Packets());
                }
                if (_player.Distance(target) <= _w.Range && _config.Item("UseWCombo").GetValue<bool>() && _w.IsReady() &&
                    !_q.IsReady())
                {
                    _w.Cast();
                }
                if (_player.Distance(target) <= _e.Range && _player.Distance(target) >= _w.Range &&
                    _config.Item("UseECombo").GetValue<bool>() && _e.IsReady() && !_w.IsReady())
                {
                    _e.Cast();
                }
                if (_player.Distance(target) <= _r.Range && _config.Item("UseRSecond").GetValue<bool>() && _r.IsReady() &&
                    !_w.IsReady() && !_q.IsReady())
                {
                    _r.Cast(target, Packets());
                }
                UseItemes(target);
         //   }
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
        }


        private static float ComboDamage(Obj_AI_Hero hero)
        {
            var dmg = 0d;

            if (_q.IsReady())
                dmg += _player.GetSpellDamage(hero, SpellSlot.Q) * 2;
            if (_w.IsReady())
                dmg += _player.GetSpellDamage(hero, SpellSlot.W);
            if (_r.IsReady())
                dmg += _player.GetSpellDamage(hero, SpellSlot.R);
            if (Items.HasItem(3128))
            {
                dmg += _player.GetItemDamage(hero, Damage.DamageItems.Dfg);
                dmg = dmg * 1.2;
            }
            if (ObjectManager.Player.GetSpellSlot("SummonerIgnite") != SpellSlot.Unknown)
            {
                dmg += _player.GetSummonerSpellDamage(hero, Damage.SummonerSpell.Ignite);
            }
            dmg += _player.GetAutoAttackDamage(hero, true) * 2;
            if (_player.HasBuff("dianaarcready"))
            {
                dmg += 15 + 5 * ObjectManager.Player.Level;
            }
            return (float)dmg;
        }


        private static void Harass()
        {
            var target = SimpleTs.GetTarget(_q.Range, SimpleTs.DamageType.Magical);
            if (target != null)
            {
                if (_player.Distance(target) <= _q.Range && _config.Item("UseQHarass").GetValue<bool>() && _q.IsReady())
                {
                    _q.CastIfHitchanceEquals(target, HitChance.High, Packets());
                }
                if (_player.Distance(target) <= 200 && _config.Item("UseWHarass").GetValue<bool>() && _w.IsReady())
                {
                    _w.Cast();
                }
            }
        }

        private static void Farm()
        {
            if (!Orbwalking.CanMove(40)) return;

            var rangedMinionsQ = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, _q.Range + _q.Width + 30,
                MinionTypes.Ranged);
            var allMinionsQ = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, _q.Range + _q.Width + 30,
                MinionTypes.All);
            var allMinionsW = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, _w.Range, MinionTypes.All);

            var useQ = _config.Item("UseQLane").GetValue<bool>();
            var useW = _config.Item("UseWLane").GetValue<bool>();
            if (_q.IsReady() && useQ)
            {
                var fl1 = _q.GetCircularFarmLocation(rangedMinionsQ, _q.Width);
                var fl2 = _q.GetCircularFarmLocation(allMinionsQ, _q.Width);

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
                            minion.Health < 0.75 * _player.GetSpellDamage(minion, SpellSlot.Q))
                            _q.Cast(minion);
            }
            if (_w.IsReady() && useW && allMinionsW.Count > 2)
            {
                _w.Cast();
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
        private static void Tragic()
        {
            var allMinionsQ = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, _q.Range + _q.Width + 30,
                MinionTypes.All);
            var mobs = MinionManager.GetMinions(_player.ServerPosition, _q.Range,
                MinionTypes.All,
                MinionTeam.Neutral, MinionOrderTypes.MaxHealth);
            _player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            if (_q.IsReady()) _q.Cast(Game.CursorPos);
            if (_r.IsReady())
            {
                if (mobs.Count > 0)
                {
                    var mob = mobs[0];

                    _r.CastOnUnit(mob);
                }
                else if (allMinionsQ.Count >= 1)
                {
                    _r.Cast(allMinionsQ[0]);
                }
            }
        }

        private static void LastHit()
        {
            var allMinions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, _q.Range, MinionTypes.All);
            var useQ = _config.Item("UseQLH").GetValue<bool>();
            var useW = _config.Item("UseWLH").GetValue<bool>();
            foreach (var minion in allMinions)
            {
                if (useQ && _q.IsReady() && _player.Distance(minion) < _q.Range &&
                    minion.Health < 0.95 * _player.GetSpellDamage(minion, SpellSlot.Q))
                {
                    _q.Cast(minion);
                }

                if (_w.IsReady() && useW && _player.Distance(minion) < _w.Range &&
                    minion.Health < 0.95 * _player.GetSpellDamage(minion, SpellSlot.W))
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
            var useQ = _config.Item("UseQJungle").GetValue<bool>();
            var useW = _config.Item("UseWJungle").GetValue<bool>();
            if (mobs.Count > 0)
            {
                var mob = mobs[0];
                if (useQ && _q.IsReady() && _player.Distance(mob) < _q.Range)
                {
                    _q.Cast(mob);
                }
                if (_w.IsReady() && useW && _player.Distance(mob) < _w.Range)
                {
                    _w.Cast();
                }
            }
        }

        private static void KillSteal()
        {
            var target = SimpleTs.GetTarget(_q.Range, SimpleTs.DamageType.Magical);
            var igniteDmg = _player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
            var qhDmg = _player.GetSpellDamage(target, SpellSlot.Q);
            var rhDmg = _player.GetSpellDamage(target, SpellSlot.R);
            var rRange = (_player.Distance(target) >= _config.Item("TargetRange").GetValue<Slider>().Value);
            if (target != null && _config.Item("UseIgnite").GetValue<bool>() && _igniteSlot != SpellSlot.Unknown &&
                _player.SummonerSpellbook.CanUseSpell(_igniteSlot) == SpellState.Ready)
            {
                if (igniteDmg > target.Health)
                {
                    _player.SummonerSpellbook.CastSpell(_igniteSlot, target);
                }
            }

            if (_q.IsReady() && _player.Distance(target) <= _q.Range && target != null &&
                _config.Item("UseQKs").GetValue<bool>())
            {
                if (target.Health <= qhDmg)
                {
                    _q.Cast(target, Packets());
                }
            }

            if (_r.IsReady() && _player.Distance(target) <= _r.Range && rRange && target != null &&
                _config.Item("UseRKs").GetValue<bool>())
            {
                if (target.Health <= rhDmg)
                {
                    _r.Cast(target, Packets());
                }
            }
        }

        private static void AutoW()
        {
            if (_player.HasBuff("Recall")) return;
            if (_w.IsReady() &&
                _player.Health <= (_player.MaxHealth * (_config.Item("Shieldper").GetValue<Slider>().Value) / 100))
            {
                _w.Cast();
            }

        }

        private static bool Packets()
        {
            return _config.Item("usePackets").GetValue<bool>();
        }

        private static void OnCreate(GameObject sender, EventArgs args)
        {
            var spell = (Obj_SpellMissile)sender;
            var unit = spell.SpellCaster.Name;
            var name = spell.SData.Name;

            //debug
            //if (unit == ObjectManager.Player.Name)


            if (unit == ObjectManager.Player.Name && (name == "dianaarcthrow"))
            {
                // Game.PrintChat("Spell: " + name);
                _qpos = spell;
                _qcreated = true;
                return;
            }
        }

        //misaya by xSalice
        private static void OnDelete(GameObject sender, EventArgs args)
        {
            var spell = (Obj_SpellMissile)sender;
            var unit = spell.SpellCaster.Name;
            var name = spell.SData.Name;

            if (unit == ObjectManager.Player.Name && (name == "dianaarcthrow"))
            {
                _qpos = null;
                _qcreated = false;
                return;
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            var diana = Drawing.WorldToScreen(_player.Position);
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
            if (_qpos != null)
                Utility.DrawCircle(_qpos.Position, _qpos.BoundingRadius, System.Drawing.Color.Red, 5, 30, false);
            if (_config.Item("ShowPassive").GetValue<bool>())
            {
                if (_player.HasBuff("dianaarcready"))
                    Drawing.DrawText(diana[0] - 10, diana[1], Color.White, "P On");
                else
                    Drawing.DrawText(diana[0] - 10, diana[1], Color.Orange, "P Off");
            }
            if (_config.Item("CircleLag").GetValue<bool>())
            {
                if (_config.Item("DrawQ").GetValue<bool>())
                {
                    Utility.DrawCircle(ObjectManager.Player.Position, _q.Range, System.Drawing.Color.White,
                        _config.Item("CircleThickness").GetValue<Slider>().Value,
                        _config.Item("CircleQuality").GetValue<Slider>().Value);
                }
                if (_config.Item("DrawW").GetValue<bool>())
                {
                    Utility.DrawCircle(ObjectManager.Player.Position, _w.Range, System.Drawing.Color.White,
                        _config.Item("CircleThickness").GetValue<Slider>().Value,
                        _config.Item("CircleQuality").GetValue<Slider>().Value);
                }
                if (_config.Item("DrawE").GetValue<bool>())
                {
                    Utility.DrawCircle(ObjectManager.Player.Position, _e.Range, System.Drawing.Color.White,
                        _config.Item("CircleThickness").GetValue<Slider>().Value,
                        _config.Item("CircleQuality").GetValue<Slider>().Value);
                }
                if (_config.Item("DrawR").GetValue<bool>())
                {
                    Utility.DrawCircle(ObjectManager.Player.Position, _r.Range, System.Drawing.Color.White,
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
            }
        }
    }
}



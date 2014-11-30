using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace D_Kayle
{
    internal class Program
    {
        private const string ChampionName = "Kayle";

        private static Orbwalking.Orbwalker _orbwalker;

        private static Spell _q, _w, _e, _r;

        private static readonly List<Spell> SpellList = new List<Spell>();

        private static SpellSlot _igniteSlot;

        private static Menu _config;

        private static Obj_AI_Hero _player;

        private static bool _recall;

        private static Int32 _lastSkin;

        private static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        private static void Game_OnGameLoad(EventArgs args)
        {
            _player = ObjectManager.Player;
            if (ObjectManager.Player.BaseSkinName != ChampionName) return;


            _q = new Spell(SpellSlot.Q, 650f);
            _w = new Spell(SpellSlot.W, 900f);
            _e = new Spell(SpellSlot.E, 675f);
            _r = new Spell(SpellSlot.R, 900f);


            SpellList.Add(_q);
            SpellList.Add(_w);
            SpellList.Add(_e);
            SpellList.Add(_r);

            _igniteSlot = _player.GetSpellSlot("SummonerDot");

            //D Nidalee
            _config = new Menu("D-天使", "D-Kayle", true);

            //TargetSelector
            var targetSelectorMenu = new Menu("目标选择", "Target Selector");
            SimpleTs.AddToMenu(targetSelectorMenu);
            _config.AddSubMenu(targetSelectorMenu);

            //Orbwalker
            _config.AddSubMenu(new Menu("走砍", "Orbwalking"));
            _orbwalker = new Orbwalking.Orbwalker(_config.SubMenu("Orbwalking"));

            //Combo
            _config.AddSubMenu(new Menu("连招", "Combo"));
            _config.SubMenu("Combo").AddItem(new MenuItem("UseQCombo", "使用 Q")).SetValue(true);
            _config.SubMenu("Combo").AddItem(new MenuItem("UseWCombo", "使用 W")).SetValue(true);
            _config.SubMenu("Combo").AddItem(new MenuItem("UseECombo", "使用 E")).SetValue(true);
            _config.SubMenu("Combo")
                .AddItem(new MenuItem("ActiveCombo", "连招!").SetValue(new KeyBind(32, KeyBindType.Press)));



            //utilities
            _config.AddSubMenu(new Menu("技能设置", "utilities"));
            _config.SubMenu("utilities").AddItem(new MenuItem("onmeW", "W 自己")).SetValue(true);
            _config.SubMenu("utilities")
                .AddItem(new MenuItem("healper", "使用W血量"))
                .SetValue(new Slider(40, 1, 100));
            _config.SubMenu("utilities").AddItem(new MenuItem("allyW", "W 队友")).SetValue(true);
            _config.SubMenu("utilities")
                .AddItem(new MenuItem("allyhealper", "使用W血量"))
                .SetValue(new Slider(40, 1, 100));
            _config.SubMenu("utilities").AddItem(new MenuItem("onmeR", "R 自己")).SetValue(true);
            _config.SubMenu("utilities")
                .AddItem(new MenuItem("ultiSelfHP", "使用R血量"))
                .SetValue(new Slider(40, 1, 100));
            _config.SubMenu("utilities").AddItem(new MenuItem("allyR", "R 队友")).SetValue(true);
            _config.SubMenu("utilities")
                .AddItem(new MenuItem("ultiallyHP", "使用R血量"))
                .SetValue(new Slider(40, 1, 100));


            //Harass
            _config.AddSubMenu(new Menu("骚扰", "Harass"));
            _config.SubMenu("Harass").AddItem(new MenuItem("UseQHarass", "使用 Q")).SetValue(true);
            _config.SubMenu("Harass").AddItem(new MenuItem("UseEHarass", "使用 E")).SetValue(true);
            _config.SubMenu("Harass")
                .AddItem(
                    new MenuItem("harasstoggle", "骚扰 (自动)").SetValue(new KeyBind("G".ToCharArray()[0],
                        KeyBindType.Toggle)));
            _config.SubMenu("Harass")
                .AddItem(
                    new MenuItem("ActiveHarass", "骚扰!").SetValue(new KeyBind("X".ToCharArray()[0],
                        KeyBindType.Press)));
            _config.SubMenu("Harass")
                .AddItem(new MenuItem("Harrasmana", "骚扰最低蓝量").SetValue(new Slider(60, 1, 100)));

            //Farm
            _config.AddSubMenu(new Menu("清兵|清野", "Farm"));
            _config.SubMenu("Farm").AddItem(new MenuItem("UseQLane", "Q 清兵")).SetValue(true);
            _config.SubMenu("Farm").AddItem(new MenuItem("UseELane", "E 清兵")).SetValue(true);
            _config.SubMenu("Farm").AddItem(new MenuItem("UseQLast", "Q 补刀")).SetValue(true);
            _config.SubMenu("Farm").AddItem(new MenuItem("UseELast", "E 补刀")).SetValue(true);
            _config.SubMenu("Farm").AddItem(new MenuItem("UseQjungle", "Q 清野")).SetValue(true);
            _config.SubMenu("Farm").AddItem(new MenuItem("UseEjungle", "E 清野")).SetValue(true);
            _config.SubMenu("Farm").AddItem(new MenuItem("Farmmana", "清兵|清野最低蓝量").SetValue(new Slider(60, 1, 100)));
            _config.SubMenu("Farm")
                .AddItem(
                    new MenuItem("Activelane", "清兵|清野").SetValue(new KeyBind("V".ToCharArray()[0],
                        KeyBindType.Press)));
            _config.SubMenu("Farm")
                .AddItem(
                    new MenuItem("activelast", "补刀").SetValue(new KeyBind("X".ToCharArray()[0], KeyBindType.Press)));

            //Kill Steal
            _config.AddSubMenu(new Menu("杂项", "Misc"));
            _config.SubMenu("Misc").AddItem(new MenuItem("UseQKs", "使用Q抢人头")).SetValue(true);
            _config.SubMenu("Misc").AddItem(new MenuItem("UseIgnite", "使用点燃抢人头")).SetValue(true);
            _config.SubMenu("Misc").AddItem(new MenuItem("usePackets", "使用封包")).SetValue(true);
            _config.SubMenu("Misc").AddItem(new MenuItem("skinKa", "使用换肤").SetValue(false));
            _config.SubMenu("Misc").AddItem(new MenuItem("skinKayle", "皮肤选择").SetValue(new Slider(4, 1, 8)));
            _config.SubMenu("Misc")
                .AddItem(new MenuItem("GapCloserE", "自动E|突进人数").SetValue(new Slider(4, 1, 8)));
            _config.SubMenu("Misc")
                .AddItem(
                    new MenuItem("Escape", "使用E逃跑").SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press)));


            //Drawings
            _config.AddSubMenu(new Menu("范围", "Drawings"));
            _config.SubMenu("Drawings").AddItem(new MenuItem("DrawQ", "范围 Q")).SetValue(true);
            _config.SubMenu("Drawings").AddItem(new MenuItem("DrawW", "范围 W")).SetValue(true);
            _config.SubMenu("Drawings").AddItem(new MenuItem("DrawE", "范围 E")).SetValue(true);
            _config.SubMenu("Drawings").AddItem(new MenuItem("DrawR", "范围 R")).SetValue(true);
            _config.SubMenu("Drawings").AddItem(new MenuItem("CircleLag", "自由延迟圈").SetValue(true));
            _config.SubMenu("Drawings")
                .AddItem(new MenuItem("CircleQuality", "圈质量").SetValue(new Slider(100, 100, 10)));
            _config.SubMenu("Drawings")
                .AddItem(new MenuItem("CircleThickness", "圈厚度").SetValue(new Slider(1, 10, 1)));

            _config.AddToMainMenu();

            Game.OnGameUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;

            Game.PrintChat("<font color='#881df2'>D-澶╀娇 By Diabaths </font> 鍔犺浇鎴愬姛!姹夊寲by浜岀嫍!QQ缇361630847.");
            if (_config.Item("skinKa").GetValue<bool>())
            {
                GenModelPacket(_player.ChampionName, _config.Item("skinKayle").GetValue<Slider>().Value);
                _lastSkin = _config.Item("skinKayle").GetValue<Slider>().Value;
            }
            //credits to eXit_ / ikkeflikkeri
            WebClient wc = new WebClient();
            wc.Proxy = null;

            wc.DownloadString("http://league.square7.ch/put.php?name=D-" + ChampionName);
                // +1 in Counter (Every Start / Reload) 
            string amount = wc.DownloadString("http://league.square7.ch/get.php?name=D-" + ChampionName);
                // Get the Counter Data
            int intamount = Convert.ToInt32(amount); // remove unneeded line from webhost
            Game.PrintChat("<font color='#881df2'>D-" + ChampionName + "</font> has been started <font color='#881df2'>" +
                           intamount + "</font> Times."); // Post Counter Data
            Game.PrintChat(
                "<font color='#FF0000'>If You like my work and want to support, and keep it always up to date plz donate via paypal in </font> <font color='#FF9900'>ssssssssssmith@hotmail.com</font> (10) S");
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            _player = ObjectManager.Player;
            _orbwalker.SetAttack(true);
            if (_config.Item("skinKa").GetValue<bool>() && SkinChanged())
            {
                GenModelPacket(_player.ChampionName, _config.Item("skinKayle").GetValue<Slider>().Value);
                _lastSkin = _config.Item("skinKayle").GetValue<Slider>().Value;
            }
            if (_config.Item("Escape").GetValue<KeyBind>().Active)
            {
                Escape();
            }
            if (_config.Item("ActiveCombo").GetValue<KeyBind>().Active)
            {
                Combo();
            }
            if ((_config.Item("ActiveHarass").GetValue<KeyBind>().Active ||
                 _config.Item("harasstoggle").GetValue<KeyBind>().Active &&
                 (100 * (_player.Mana / _player.MaxMana)) > _config.Item("Harrasmana").GetValue<Slider>().Value))
            {
                Harass();
            }
            if (_config.Item("activelast").GetValue<KeyBind>().Active &&
                (100 * (_player.Mana / _player.MaxMana)) > _config.Item("Farmmana").GetValue<Slider>().Value)
            {
                Lasthit();
            }
            if (_config.Item("Activelane").GetValue<KeyBind>().Active &&
                (100 * (_player.Mana / _player.MaxMana)) > _config.Item("Farmmana").GetValue<Slider>().Value)
            {
                JungleFarm();
                Farm();
            }
            AutoW();
            AutoR();
            AllyR();
            AllyW();
            KillSteal();
        }
      
        private static void GenModelPacket(string champ, int skinId)
        {
            Packet.S2C.UpdateModel.Encoded(new Packet.S2C.UpdateModel.Struct(_player.NetworkId, skinId, champ))
                .Process();
        }

        private static bool SkinChanged()
        {
            return (_config.Item("skinKayle").GetValue<Slider>().Value != _lastSkin);
        }

        private static bool Packets()
        {
            return _config.Item("usePackets").GetValue<bool>();
        }

        private static void AutoR()
        {
            if (_player.HasBuff("Recall")) return;
            if (_config.Item("onmeR").GetValue<bool>() && _config.Item("onmeR").GetValue<bool>() &&
                (_player.Health / _player.MaxHealth) * 100 <= _config.Item("ultiSelfHP").GetValue<Slider>().Value &&
                _r.IsReady() && Utility.CountEnemysInRange(650) > 0)
            {
                _r.Cast(_player);
            }
        }

        private static void AllyR()
        {
            foreach (var hero in ObjectManager.Get<Obj_AI_Hero>().Where(hero => hero.IsAlly))
            {
                if (_player.HasBuff("Recall")) return;
                if (_config.Item("allyR").GetValue<bool>() &&
                    (hero.Health / hero.MaxHealth) * 100 <= _config.Item("ultiallyHP").GetValue<Slider>().Value &&
                    _r.IsReady() && Utility.CountEnemysInRange(1000) > 0 &&
                    hero.Distance(_player.ServerPosition) <= _r.Range)
                {
                    _r.Cast(hero);
                }
            }
        }

        private static void Combo()
        {
            var target = SimpleTs.GetTarget(_r.Range + 200, SimpleTs.DamageType.Magical);

            if (target != null)
            {
                if (_config.Item("UseQCombo").GetValue<bool>() && _q.IsReady() && _player.Distance(target) <= _q.Range)
                {
                    _q.Cast(target, Packets());

                }
                if (_config.Item("UseECombo").GetValue<bool>() && _e.IsReady() && Utility.CountEnemysInRange(650) > 0)
                {
                    _e.Cast();
                }

                if (_config.Item("UseWCombo").GetValue<bool>() && _player.Distance(target) >= _q.Range)
                {
                    _w.Cast(_player);
                }
            }
        }

        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (_q.IsReady() && gapcloser.Sender.IsValidTarget(_q.Range) &&
                _config.Item("GapCloserE").GetValue<bool>())
            {
                _q.Cast(gapcloser.Sender, Packets());
            }
        }

        private static void Escape()
        {
            var target = SimpleTs.GetTarget(_q.Range, SimpleTs.DamageType.Magical);
            _player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            if (_player.Spellbook.CanUseSpell(SpellSlot.W) == SpellState.Ready && _player.IsMe)
            {
                if (target != null && Utility.CountEnemysInRange(1200) > 0)
                {
                    _player.Spellbook.CastSpell(SpellSlot.W, _player);
                }
            }
            if (_player.Distance(target) <= _q.Range && (target != null) && _q.IsReady())
            {
                _q.Cast(target);
            }
        }

        private static void Harass()
        {
            var target = SimpleTs.GetTarget(_q.Range, SimpleTs.DamageType.Magical);
            if (target != null)
            {

                if (_config.Item("harasstoggle").GetValue<KeyBind>().Active ||
                    _config.Item("UseQHarass").GetValue<bool>())
                {
                    _q.Cast(target, Packets());
                }

                if (_config.Item("ActiveHarass").GetValue<KeyBind>().Active &&
                    _config.Item("UseEHarass").GetValue<bool>())
                    _e.Cast();
            }
        }

        private static void Farm()
        {
            if (!Orbwalking.CanMove(40)) return;
            var minions = MinionManager.GetMinions(_player.ServerPosition, _q.Range);
            foreach (var minion in minions)
            {
                if (_config.Item("UseQLane").GetValue<bool>() && _q.IsReady())
                {
                    if (minions.Count > 2)
                    {
                        _q.Cast(minion, Packets());

                    }
                    else
                        foreach (var minionQ in minions)
                            if (!Orbwalking.InAutoAttackRange(minion) &&
                                minionQ.Health < 0.75 * _player.GetSpellDamage(minion, SpellSlot.Q))
                                _q.Cast(minionQ, Packets());
                }
                if (_config.Item("UseELane").GetValue<bool>() && _e.IsReady())
                {
                    if (minions.Count > 2)
                    {
                        _e.Cast();

                    }
                    else
                        foreach (var minionE in minions)
                            if (!Orbwalking.InAutoAttackRange(minion) &&
                                minionE.Health < 0.75 * _player.GetSpellDamage(minion, SpellSlot.E))
                                _e.Cast();
                }
            }
        }

        private static void Lasthit()
        {
            if (!Orbwalking.CanMove(40)) return;
            var allMinions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, _q.Range, MinionTypes.All);
            var useQ = _config.Item("UseQLast").GetValue<bool>();
            var useE = _config.Item("UseELast").GetValue<bool>();
            foreach (var minion in allMinions)
            {
                if (useQ && _q.IsReady() && minion.Health < 0.75 * _player.GetSpellDamage(minion, SpellSlot.Q))
                {
                    _q.Cast(minion, Packets());
                }

                if (_w.IsReady() && useE && minion.Health < 0.75 * _player.GetSpellDamage(minion, SpellSlot.E) &&
                    allMinions.Count > 2)
                {
                    _e.Cast();
                }
            }
        }

        private static void JungleFarm()
        {
            if (!Orbwalking.CanMove(40)) return;
            var mobs = MinionManager.GetMinions(_player.ServerPosition, _q.Range, MinionTypes.All, MinionTeam.Neutral,
                MinionOrderTypes.MaxHealth);
            if (mobs.Count > 0)
            {
                var mob = mobs[0];
                if (_config.Item("UseQjungle").GetValue<bool>() && _q.IsReady())
                {
                    _q.Cast(mob, Packets());
                }
                if (_config.Item("UseQjungle").GetValue<bool>() && _e.IsReady())
                {
                    _e.Cast();
                }
            }
        }

        private static void AutoW()
        {
            if (_player.Spellbook.CanUseSpell(SpellSlot.W) == SpellState.Ready && _player.IsMe)
            {

                if (_player.HasBuff("Recall")) return;

                if (_config.Item("onmeW").GetValue<bool>() &&
                    _player.Health <= (_player.MaxHealth * (_config.Item("healper").GetValue<Slider>().Value) / 100))
                {
                    _player.Spellbook.CastSpell(SpellSlot.W, _player);
                }
            }
        }

        private static void AllyW()
        {
            foreach (var hero in ObjectManager.Get<Obj_AI_Hero>().Where(hero => hero.IsAlly && !hero.IsMe))
            {
                if (_player.HasBuff("Recall") || hero.HasBuff("Recall")) return;
                if (_config.Item("allyW").GetValue<bool>() &&
                    (hero.Health / hero.MaxHealth) * 100 <= _config.Item("allyhealper").GetValue<Slider>().Value &&
                    _w.IsReady() && Utility.CountEnemysInRange(1200) > 0 &&
                    hero.Distance(_player.ServerPosition) <= _w.Range)
                {
                    _w.Cast(hero);
                }
            }
        }

        private static void KillSteal()
        {
            var target = SimpleTs.GetTarget(_q.Range, SimpleTs.DamageType.Magical);
            var igniteDmg = _player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
            var qhDmg = _player.GetSpellDamage(target, SpellSlot.Q);

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
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (_config.Item("CircleLag").GetValue<bool>())
            {
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


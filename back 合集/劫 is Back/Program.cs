#region
/*
* Credits to:
 * Kortatu(thx for great common lib)
 * Legacy( i got orianna farm logic and some cool examples from your wip zed) 
 * Kurisu (ult on dangerous)
 * Pingo(for finding me proper commands whenever i ask :)
 * Andre(for answerin me whenever i ask him a question ^^ :)
 */
using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using System.Threading.Tasks;
using System.Text;
using SharpDX;
using Color = System.Drawing.Color;

#endregion

namespace Zed
{
    internal class program
    {
        private const string ChampionName = "Zed";
        private static List<Spell> SpellList = new List<Spell>();
        private static Spell _q, _w, _e, _r;
        private static Orbwalking.Orbwalker _orbwalker;
        private static Menu _config;
        private static Obj_AI_Hero _player;
        private static SpellSlot _igniteSlot;
        private static Items.Item _tiamat, _hydra, _blade, _bilge, _rand, _lotis, _youmuu;
        private static Vector3 linepos;
        private static Vector3 castpos;
        private static int ticktock;
        private static float hppi;
        private static Vector3 rpos;
        private static int shadowdelay = 0;
        private static int delayw = 500;

        private static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        private static void Game_OnGameLoad(EventArgs args)
        {
            _player = ObjectManager.Player;
            if (ObjectManager.Player.BaseSkinName != ChampionName) return;
            _q = new Spell(SpellSlot.Q, 900f);
            _w = new Spell(SpellSlot.W, 550f);
            _e = new Spell(SpellSlot.E, 270f);
            _r = new Spell(SpellSlot.R, 650f);

            _q.SetSkillshot(0.25f, 50f, 1700f, false, SkillshotType.SkillshotLine);

            _bilge = new Items.Item(3144, 475f);
            _blade = new Items.Item(3153, 425f);
            _hydra = new Items.Item(3074, 250f);
            _tiamat = new Items.Item(3077, 250f);
            _rand = new Items.Item(3143, 490f);
            _lotis = new Items.Item(3190, 590f);
            _youmuu = new Items.Item(3142, 10);
            _igniteSlot = _player.GetSpellSlot("SummonerDot");

            var enemy = from hero in ObjectManager.Get<Obj_AI_Hero>()
                        where hero.IsEnemy == true
                        select hero;
            // Just menu things test
            _config = new Menu("|鍔珅", "Ze-D Is Back", true);

            var targetSelectorMenu = new Menu("鐩爣閫夋嫨", "Target Selector");
            SimpleTs.AddToMenu(targetSelectorMenu);
            _config.AddSubMenu(targetSelectorMenu);

            _config.AddSubMenu(new Menu("璧扮爫", "Orbwalking"));
            _orbwalker = new Orbwalking.Orbwalker(_config.SubMenu("Orbwalking"));

            //Combo
            _config.AddSubMenu(new Menu("杩炴嫑", "Combo"));
            _config.SubMenu("Combo").AddItem(new MenuItem("UseWC", "浣跨敤W|鏁屼汉绐佽繘|")).SetValue(true);
            _config.SubMenu("Combo").AddItem(new MenuItem("UseIgnitecombo", "浣跨敤鐐圭噧|鍙嚮鏉€|")).SetValue(true);
            _config.SubMenu("Combo")
                .AddItem(new MenuItem("ActiveCombo", "杩炴嫑!").SetValue(new KeyBind(32, KeyBindType.Press)));
            _config.SubMenu("Combo")
    .AddItem(new MenuItem("TheLine", "杩炴嫑锛堝厛R)").SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press)));
            _config.SubMenu("Combo").AddItem(new MenuItem("RbackC", "杩炴嫑 R 杩斿洖")).SetValue(false);
            _config.SubMenu("Combo").AddItem(new MenuItem("RbackL", "杩炴嫑 R2 杩斿洖")).SetValue(false);

            //Harass
            _config.AddSubMenu(new Menu("楠氭壈", "Harass"));
            _config.SubMenu("Harass").AddItem(new MenuItem("longhar", "杩滅▼ Poke (鑷姩)").SetValue(new KeyBind("U".ToCharArray()[0], KeyBindType.Toggle)));
            _config.SubMenu("Harass").AddItem(new MenuItem("UseItemsharass", "浣跨敤 鎻愪簹椹壒/涔濆ご铔噟")).SetValue(true);
            _config.SubMenu("Harass").AddItem(new MenuItem("UseWH", "浣跨敤 W")).SetValue(true);
            _config.SubMenu("Harass")
                .AddItem(
                    new MenuItem("ActiveHarass", "楠氭壈!").SetValue(new KeyBind("C".ToCharArray()[0], KeyBindType.Press)));

            //items
            _config.AddSubMenu(new Menu("鐗╁搧", "items"));
            _config.SubMenu("items").AddSubMenu(new Menu("杩涙敾鐗╁搧", "Offensive"));
            _config.SubMenu("items").SubMenu("Offensive").AddItem(new MenuItem("Youmuu", "浣跨敤 骞芥ⅵ")).SetValue(true);
            _config.SubMenu("items").SubMenu("Offensive").AddItem(new MenuItem("Tiamat", "浣跨敤 鎻愪簹椹壒")).SetValue(true);
            _config.SubMenu("items").SubMenu("Offensive").AddItem(new MenuItem("Hydra", "浣跨敤 涔濆ご铔噟")).SetValue(true);
            _config.SubMenu("items").SubMenu("Offensive").AddItem(new MenuItem("Bilge", "浣跨敤 灏忓集鍒€")).SetValue(true);
            _config.SubMenu("items")
                .SubMenu("Offensive")
                .AddItem(new MenuItem("BilgeEnemyhp", "鏁屼汉鏈€灏忚閲弢").SetValue(new Slider(85, 1, 100)));
            _config.SubMenu("items")
                .SubMenu("Offensive")
                .AddItem(new MenuItem("Bilgemyhp", "鑷繁鏈€澶ц閲弢").SetValue(new Slider(85, 1, 100)));
            _config.SubMenu("items").SubMenu("Offensive").AddItem(new MenuItem("Blade", "浣跨敤 鐮磋触")).SetValue(true);
            _config.SubMenu("items")
                .SubMenu("Offensive")
                .AddItem(new MenuItem("BladeEnemyhp", "鏁屼汉鏈€灏忚閲弢").SetValue(new Slider(85, 1, 100)));
            _config.SubMenu("items")
                .SubMenu("Offensive")
                .AddItem(new MenuItem("Blademyhp", "鑷繁鏈€澶ц閲弢").SetValue(new Slider(85, 1, 100)));
            _config.SubMenu("items").AddSubMenu(new Menu("闃插尽鐗╁搧", "Deffensive"));
            _config.SubMenu("items")
                .SubMenu("Deffensive")
                .AddItem(new MenuItem("Omen", "浣跨敤 鍏伴爴"))
                .SetValue(true);
            _config.SubMenu("items")
                .SubMenu("Deffensive")
                .AddItem(new MenuItem("Omenenemys", "鍏扮浘|鏁屾柟浜烘暟|").SetValue(new Slider(2, 1, 5)));
            _config.SubMenu("items")
                .SubMenu("Deffensive")
                .AddItem(new MenuItem("lotis", "浣跨敤 楦熺浘"))
                .SetValue(true);
            _config.SubMenu("items")
                .SubMenu("Deffensive")
                .AddItem(new MenuItem("lotisminhp", "楦熺浘|闃熷弸琛€閲弢").SetValue(new Slider(35, 1, 100)));

            //Farm
            _config.AddSubMenu(new Menu("娓呯嚎|娓呴噹", "Farm"));
            _config.SubMenu("Farm").AddSubMenu(new Menu("娓呯嚎", "LaneFarm"));
            _config.SubMenu("Farm")
                .SubMenu("LaneFarm")
                .AddItem(new MenuItem("UseItemslane", "浣跨敤 鎻愪簹椹壒/涔濆ご铔噟"))
                .SetValue(true);
            _config.SubMenu("Farm").SubMenu("LaneFarm").AddItem(new MenuItem("UseQL", "浣跨敤 Q 娓呯嚎")).SetValue(true);
            _config.SubMenu("Farm").SubMenu("LaneFarm").AddItem(new MenuItem("UseEL", "浣跨敤 E 娓呯嚎")).SetValue(true);
            _config.SubMenu("Farm")
                .SubMenu("LaneFarm")
                .AddItem(new MenuItem("Energylane", "娓呯嚎|鏈€浣庤兘閲弢").SetValue(new Slider(45, 1, 100)));
            _config.SubMenu("Farm")
                .SubMenu("LaneFarm")
                .AddItem(
                    new MenuItem("Activelane", "娓呯嚎!").SetValue(new KeyBind("V".ToCharArray()[0], KeyBindType.Press)));

            _config.SubMenu("Farm").AddSubMenu(new Menu("琛ュ垁", "LastHit"));
            _config.SubMenu("Farm").SubMenu("LastHit").AddItem(new MenuItem("UseQLH", "浣跨敤 Q 琛ュ垁")).SetValue(true);
            _config.SubMenu("Farm").SubMenu("LastHit").AddItem(new MenuItem("UseELH", "浣跨敤 E 琛ュ垁")).SetValue(true);
            _config.SubMenu("Farm")
                .SubMenu("LastHit")
                .AddItem(new MenuItem("Energylast", "琛ュ垁|鏈€浣庤兘閲弢").SetValue(new Slider(85, 1, 100)));
            _config.SubMenu("Farm")
                .SubMenu("LastHit")
                .AddItem(
                    new MenuItem("ActiveLast", "琛ュ垁!").SetValue(new KeyBind("X".ToCharArray()[0], KeyBindType.Press)));

            _config.SubMenu("Farm").AddSubMenu(new Menu("娓呴噹", "Jungle"));
            _config.SubMenu("Farm")
                .SubMenu("Jungle")
                .AddItem(new MenuItem("UseItemsjungle", "浣跨敤 鎻愪簹椹壒/涔濆ご铔噟"))
                .SetValue(true);
            _config.SubMenu("Farm").SubMenu("Jungle").AddItem(new MenuItem("UseQJ", "浣跨敤 Q 娓呴噹")).SetValue(true);
            _config.SubMenu("Farm").SubMenu("Jungle").AddItem(new MenuItem("UseWJ", "浣跨敤 W 娓呴噹")).SetValue(true);
            _config.SubMenu("Farm").SubMenu("Jungle").AddItem(new MenuItem("UseEJ", "浣跨敤 E 娓呴噹")).SetValue(true);
            _config.SubMenu("Farm")
                .SubMenu("Jungle")
                .AddItem(new MenuItem("Energyjungle", "娓呴噹|鏈€浣庤兘閲弢").SetValue(new Slider(85, 1, 100)));
            _config.SubMenu("Farm")
                .SubMenu("Jungle")
                .AddItem(
                    new MenuItem("Activejungle", "娓呴噹!").SetValue(new KeyBind("V".ToCharArray()[0], KeyBindType.Press)));

            //Misc
            _config.AddSubMenu(new Menu("鏉傞」", "Misc"));
            _config.SubMenu("Misc").AddItem(new MenuItem("UseIgnitekill", "浣跨敤 鐐圭噧 鍑绘潃")).SetValue(true);
            _config.SubMenu("Misc").AddItem(new MenuItem("UseQM", "浣跨敤 Q 鎶汉澶磡")).SetValue(true);
            _config.SubMenu("Misc").AddItem(new MenuItem("UseEM", "浣跨敤 E 鎶汉澶磡")).SetValue(true);
            _config.SubMenu("Misc").AddItem(new MenuItem("AutoE", "鑷姩 E")).SetValue(true);
            _config.SubMenu("Misc").AddItem(new MenuItem("rdodge", "鑷姩R韬查伩鎶€鑳絴")).SetValue(true);
            _config.SubMenu("Misc").AddItem(new MenuItem("", ""));
            foreach (var e in enemy)
            {
                SpellDataInst rdata = e.Spellbook.GetSpell(SpellSlot.R);
                if (DangerDB.DangerousList.Any(spell => spell.Contains(rdata.SData.Name)))
                    _config.SubMenu("Misc").AddItem(new MenuItem("ds" + e.SkinName, rdata.SData.Name)).SetValue(true);
            }


            //Drawings
            _config.AddSubMenu(new Menu("鑼冨洿", "Drawings"));
            _config.SubMenu("Drawings").AddItem(new MenuItem("DrawQ", "鏄剧ず Q 鑼冨洿")).SetValue(true);
            _config.SubMenu("Drawings").AddItem(new MenuItem("DrawE", "鏄剧ず E 鑼冨洿")).SetValue(true);
            _config.SubMenu("Drawings").AddItem(new MenuItem("DrawQW", "鏄剧ず 杩滅▼ 楠氭壈")).SetValue(true);
            _config.SubMenu("Drawings").AddItem(new MenuItem("DrawR", "鏄剧ず R 鑼冨洿")).SetValue(true);
            _config.SubMenu("Drawings").AddItem(new MenuItem("shadowd", "鏄剧ず 褰卞瓙 浣嶇疆")).SetValue(true);
            _config.SubMenu("Drawings").AddItem(new MenuItem("damagetest", "鏄剧ず 浼ゅ 淇℃伅")).SetValue(true);
            _config.SubMenu("Drawings").AddItem(new MenuItem("CircleLag", "寤惰繜 鑷敱鍦坾").SetValue(true));
            _config.SubMenu("Drawings")
                .AddItem(new MenuItem("CircleQuality", "绾垮湀|钖勫帤").SetValue(new Slider(100, 100, 10)));
            _config.SubMenu("Drawings")
                .AddItem(new MenuItem("CircleThickness", "绾垮湀|璐ㄩ噺").SetValue(new Slider(1, 10, 1)));
            _config.AddToMainMenu();
            Game.PrintChat("<font color='#881df2'>Zed by Diabaths & jackisback</font> Loaded.");
            Game.PrintChat("<font color='#f2881d'>if you wanna help me to pay my internet bills^^ paypal= bulut@live.co.uk</font>");
            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnGameUpdate += Game_OnGameUpdate;
            Game.OnWndProc += OnWndProc;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpell;



        }

        private static void OnWndProc(WndEventArgs args)
        {
            if (args.Msg == 514)
            {
                linepos = Game.CursorPos;

            }
        }
        private static void OnProcessSpell(Obj_AI_Base unit, GameObjectProcessSpellCastEventArgs castedSpell)
        {
            if (_config.Item("rdodge").GetValue<bool>() && _config.Item("ds" + unit.SkinName).GetValue<bool>())

            if (DangerDB.DangerousList.Any(spell => spell.Contains(castedSpell.SData.Name)) && 
                    (unit.Distance(_player) < 650f || _player.Distance(castedSpell.End) <= 250f) && _r.IsReady())
                    _r.Cast(unit);

    
            if (unit.IsMe && castedSpell.SData.Name == "zedult")
            {
                ticktock = Environment.TickCount + 200;

            }
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (_config.Item("ActiveCombo").GetValue<KeyBind>().Active)
            {
                Combo();
            }
            if (_config.Item("TheLine").GetValue<KeyBind>().Active)
            {
                TheLine();
            }
            if (_config.Item("ActiveHarass").GetValue<KeyBind>().Active)
            {
                Harass();

            }
            if (_config.Item("Activelane").GetValue<KeyBind>().Active)
            {
                Laneclear();
            }
            if (_config.Item("Activejungle").GetValue<KeyBind>().Active)
            {
                JungleClear();
            }
            if (_config.Item("ActiveLast").GetValue<KeyBind>().Active)
            {
                LastHit();
            }
            if (_config.Item("AutoE").GetValue<bool>())
            {
                if (ObjectManager.Get<Obj_AI_Hero>().Count(hero =>
                    hero.IsValidTarget() && (hero.Distance(ObjectManager.Player.ServerPosition) <= _e.Range ||
                                             (WShadow != null && hero.Distance(WShadow.ServerPosition) <= _e.Range))) > 0)
                    _e.Cast();
            }

            if (Environment.TickCount <= ticktock)
            {
                foreach (var enemy in
                ObjectManager.Get<Obj_AI_Hero>().Where(enemyVisible => enemyVisible.IsValidTarget()))
                {
                    if (enemy.HasBuff("zedulttargetmark", true))
                    {
                        hppi = enemy.Health;
                    }
                }
            }

            if (LastCastedSpell.LastCastPacketSent.Slot == SpellSlot.R)
            {
                Obj_AI_Minion shadow;
                shadow = ObjectManager.Get<Obj_AI_Minion>()
                        .FirstOrDefault(minion => minion.IsVisible && minion.IsAlly && minion.Name == "Shadow");

                rpos = shadow.ServerPosition;
            }


            _player = ObjectManager.Player;
            _orbwalker.SetAttack(true);


            KillSteal();

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
                damage += _player.GetSpellDamage(enemy, SpellSlot.Q) * 1.5;
            if (_q.IsReady())
                damage += _player.GetSpellDamage(enemy, SpellSlot.W);
            if (_e.IsReady())
                damage += _player.GetSpellDamage(enemy, SpellSlot.E);
            if (_r.IsReady())
                damage += _player.GetSpellDamage(enemy, SpellSlot.R);
            damage += (_r.Level * 0.15 + 0.05) * (damage - ObjectManager.Player.GetSummonerSpellDamage(enemy, Damage.SummonerSpell.Ignite));

            return (float)damage;
        }

        private static void Combo()
        {
            var target = SimpleTs.GetTarget(2000, SimpleTs.DamageType.Physical);
            if (target != null && _config.Item("UseIgnitecombo").GetValue<bool>() && _igniteSlot != SpellSlot.Unknown &&
                _player.SummonerSpellbook.CanUseSpell(_igniteSlot) == SpellState.Ready)
            {
                if (ComboDamage(target) > target.Health)
                {
                    _player.SummonerSpellbook.CastSpell(_igniteSlot, target);
                }
            }
            if (target != null && ShadowStage == ShadowCastStage.First && _config.Item("UseWC").GetValue<bool>() && target.Distance(_player.Position) > 400 && target.Distance(_player.Position) < 1300)
            {
                CastW(target);
                _w.Cast();
            }
            if (target != null && ShadowStage == ShadowCastStage.Second && _config.Item("UseWC").GetValue<bool>() && target.Distance(WShadow.ServerPosition) < target.Distance(_player.Position))
            {
                _w.Cast();
            }

            if (target != null && !_r.IsReady() && _config.Item("UseWC").GetValue<bool>())
            {
                Harass();
            }
            if (target != null && target.HasBuff("zedulttargetmark", true) && _config.Item("RbackC").GetValue<bool>() && 
                 UltStage == UltCastStage.Second && target.Health <
               (hppi * (_r.Level * 0.15 + 0.05)  - 3 * ((ObjectManager.Player.Level - 1) * 4 + 14)))
            {
                _r.Cast();
            }

            UseItemes(target);
            _q.Cast(target.Position);
            CastE();
        }

        private static void TheLine()
        {
            var target = SimpleTs.GetTarget(_r.Range, SimpleTs.DamageType.Physical);

            if (target == null)
            {
                _player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            }
            else
            {
                _player.IssueOrder(GameObjectOrder.AttackUnit, target);
            }

            if ((linepos.X == 0 && linepos.Y == 0) || !_r.IsReady())
            {
                return;
            }

            _r.Cast(target);

            if (target != null && ShadowStage == ShadowCastStage.First &&  UltStage == UltCastStage.Second)
            {
                UseItemes(target);
                if (LastCastedSpell.LastCastPacketSent.Slot != SpellSlot.W)
                {
                    var m = (double)((linepos.Y - target.ServerPosition.Y) / (linepos.X - target.ServerPosition.X));
                    var angle = (double)Math.Atan(m);

                    if (linepos.X > target.ServerPosition.X)
                    {
                        castpos.X = target.ServerPosition.X + 550 * (float)Math.Cos(angle);
                        castpos.Y = target.ServerPosition.Y + 550 * (float)Math.Sin(angle);
                    }
                    else
                    {
                        castpos.X = target.ServerPosition.X - 550 * (float)Math.Cos(angle);
                        castpos.Y = target.ServerPosition.Y - 550 * (float)Math.Sin(angle);
                    }

                    castpos.Z = target.ServerPosition.Z;

                    _w.Cast(castpos, false);
                }
            }
            if (target != null && _config.Item("UseIgnitecombo").GetValue<bool>() && _igniteSlot != SpellSlot.Unknown &&
                _player.SummonerSpellbook.CanUseSpell(_igniteSlot) == SpellState.Ready)
            {
                _player.SummonerSpellbook.CastSpell(_igniteSlot, target);
            }

            _q.Cast(target.Position);
            CastE();

            if (target != null && target.HasBuff("zedulttargetmark", true) && _config.Item("RbackL").GetValue<bool>() && target.Health <
              (hppi * (_r.Level * 0.15 + 0.05) + 50 + 3*((ObjectManager.Player.Level-1)*4+14)))
            {
                _r.Cast();
            }
            if (target != null && WShadow != null && UltStage == UltCastStage.Second && target.Distance(_player.Position) > 250 && (target.Distance(WShadow.ServerPosition) < target.Distance(_player.Position)))
            {
                _w.Cast();
            }

        }

        private static void Harass()
        {
            var target = SimpleTs.GetTarget(2000, SimpleTs.DamageType.Physical);
            var useItemsH = _config.Item("UseItemsharass").GetValue<bool>();
            if (target.IsValidTarget() && !(_config.Item("UseWH").GetValue<bool>()) && _q.IsReady() &&
                           target.Distance(_player.Position) < 900 )
            {
                CastQ(target);
                
            }
            if (target.IsValidTarget() && _config.Item("longhar").GetValue<KeyBind>().Active && _w.IsReady() && _q.IsReady() && ObjectManager.Player.Mana >
                ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).ManaCost +
                ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).ManaCost && target.Distance(_player.Position) > 850 &&
                target.Distance(_player.Position) < 1400)
            {
                CastW(target);
                CastQ(target);
            }

            if (target.IsValidTarget() && !_w.IsReady() && _q.IsReady() &&
                           (target.Distance(_player.Position) < 900 || target.Distance(WShadow.ServerPosition) < 900))
            {
                CastQ(target);
            }
            else
            {
                if (target.IsValidTarget() && _w.IsReady() && _q.IsReady() && _config.Item("UseWH").GetValue<bool>() &&
                ObjectManager.Player.Mana >
                ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).ManaCost +
                ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).ManaCost && target.Distance(_player.Position) < 850)
                {
                    CastW(target);

                    if (target.IsValidTarget() && WShadow != null && target.Distance(WShadow.ServerPosition) < 900)

                        CastQ(target);
                }
            }


            if (useItemsH && _tiamat.IsReady() && target.Distance(_player.Position) < _tiamat.Range)
            {
                _tiamat.Cast();
            }
            if (useItemsH && _hydra.IsReady() && target.Distance(_player.Position) < _hydra.Range)
            {
                _hydra.Cast();
            }
            CastE();

        }

        private static void Laneclear()
        {
            var allMinionsQ = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, _q.Range, MinionTypes.All);
            var allMinionsE = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, _e.Range,
                MinionTypes.All);
            var mymana = (_player.Mana >=
                          (_player.MaxMana * _config.Item("Energylane").GetValue<Slider>().Value) / 100);
            var useItemsl = _config.Item("UseItemslane").GetValue<bool>();
            var useQl = _config.Item("UseQL").GetValue<bool>();
            var useEl = _config.Item("UseEL").GetValue<bool>();
            if (_q.IsReady() && useQl && mymana)
            {
                var fl2 = _q.GetLineFarmLocation(allMinionsQ, _q.Width);

                if (fl2.MinionsHit >= 3)
                {
                    _q.Cast(fl2.Position);
                }
                else
                    foreach (var minion in allMinionsQ)
                        if (!Orbwalking.InAutoAttackRange(minion) &&
                            minion.Health < 0.75 * _player.GetSpellDamage(minion, SpellSlot.Q))
                            _q.Cast(minion);
            }

            if (_e.IsReady() && useEl && mymana)
            {
                if (allMinionsE.Count > 2)
                {
                    _e.Cast();
                }
                else
                    foreach (var minion in allMinionsE)
                        if (!Orbwalking.InAutoAttackRange(minion) &&
                            minion.Health < 0.75 * _player.GetSpellDamage(minion, SpellSlot.E))
                            _e.Cast();
            }

            if (useItemsl && _tiamat.IsReady() && allMinionsE.Count > 2)
            {
                _tiamat.Cast();
            }
            if (useItemsl && _hydra.IsReady() && allMinionsE.Count > 2)
            {
                _hydra.Cast();
            }
        }

        private static void LastHit()
        {
            var allMinions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, _q.Range, MinionTypes.All);
            var mymana = (_player.Mana >=
                          (_player.MaxMana * _config.Item("Energylast").GetValue<Slider>().Value) / 100);
            var useQ = _config.Item("UseQLH").GetValue<bool>();
            var useE = _config.Item("UseELH").GetValue<bool>();
            foreach (var minion in allMinions)
            {
                if (mymana && useQ && _q.IsReady() && _player.Distance(minion) < _q.Range &&
                    minion.Health < 0.75 * _player.GetSpellDamage(minion, SpellSlot.Q))
                {
                    _q.Cast(minion);
                }

                if (mymana && _e.IsReady() && useE && _player.Distance(minion) < _e.Range &&
                    minion.Health < 0.95 * _player.GetSpellDamage(minion, SpellSlot.E))
                {
                    _e.Cast();
                }
            }
        }

        private static void JungleClear()
        {
            var mobs = MinionManager.GetMinions(_player.ServerPosition, _q.Range,
                MinionTypes.All,
                MinionTeam.Neutral, MinionOrderTypes.MaxHealth);
            var mymana = (_player.Mana >=
                          (_player.MaxMana * _config.Item("Energyjungle").GetValue<Slider>().Value) / 100);
            var useItemsJ = _config.Item("UseItemsjungle").GetValue<bool>();
            var useQ = _config.Item("UseQJ").GetValue<bool>();
            var useW = _config.Item("UseWJ").GetValue<bool>();
            var useE = _config.Item("UseEJ").GetValue<bool>();

            if (mobs.Count > 0)
            {
                var mob = mobs[0];
                if (mymana && _w.IsReady() && useW && _player.Distance(mob) < _q.Range)
                {
                    _w.Cast(mob.Position);
                }
                if (mymana && useQ && _q.IsReady() && _player.Distance(mob) < _q.Range)
                {
                    CastQ(mob);
                }
                if (mymana && _e.IsReady() && useE && _player.Distance(mob) < _e.Range)
                {
                    _e.Cast();
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
            var iYoumuu = _config.Item("Youmuu").GetValue<bool>();
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
            if (Utility.CountEnemysInRange(350) >= 1 && iTiamat && _tiamat.IsReady())
            {
                _tiamat.Cast();

            }
            if (Utility.CountEnemysInRange(350) >= 1 && iHydra && _hydra.IsReady())
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
            if (_player.Distance(target) <= 350 && iYoumuu && _youmuu.IsReady())
            {
                _youmuu.Cast();

            }
        }

        private static Obj_AI_Minion WShadow
        {
            get
            {
                return
                    ObjectManager.Get<Obj_AI_Minion>()
                        .FirstOrDefault(minion => minion.IsVisible && minion.IsAlly && (minion.ServerPosition != rpos) && minion.Name == "Shadow");
            }
        }
        private static Obj_AI_Minion RShadow
        {
            get
            {
                return
                    ObjectManager.Get<Obj_AI_Minion>()
                        .FirstOrDefault(minion => minion.IsVisible && minion.IsAlly && (minion.ServerPosition == rpos) && minion.Name == "Shadow");
            }
        }

        private static UltCastStage UltStage
        {
            get
            {
                if (!_r.IsReady()) return UltCastStage.Cooldown;

                return (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).Name == "zedult"
                    ? UltCastStage.First
                    : UltCastStage.Second);
            }
        }


        private static ShadowCastStage ShadowStage
        {
            get
            {
                if (!_w.IsReady()) return ShadowCastStage.Cooldown;

                return (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Name == "ZedShadowDash"
                    ? ShadowCastStage.First
                    : ShadowCastStage.Second);
            }
        }

        private static void CastW(Obj_AI_Base target)
        {
            if (delayw >= Environment.TickCount - shadowdelay)
                return;
            if (ShadowStage != ShadowCastStage.First)
                return;
            _w.Cast(target.Position, true);
            shadowdelay = Environment.TickCount;

        }

        private static void CastQ(Obj_AI_Base target)
        {
            if (!_q.IsReady()) return;
            _q.UpdateSourcePosition(ObjectManager.Player.ServerPosition, ObjectManager.Player.ServerPosition);

            if (WShadow != null)
            {
                _q.UpdateSourcePosition(WShadow.ServerPosition, WShadow.ServerPosition);
                _q.Cast(target, false, true);
                _q.UpdateSourcePosition(ObjectManager.Player.ServerPosition, ObjectManager.Player.ServerPosition);
                _q.Cast(target, false, true);
            }
            else if (_q.WillHit(target, target.ServerPosition))

                _q.Cast(target, false, true);

        }
        private static void CastE()
        {
            if (ObjectManager.Get<Obj_AI_Hero>()
                .Count(
                    hero =>
                        hero.IsValidTarget() &&
                        (hero.Distance(ObjectManager.Player.ServerPosition) <= _e.Range ||
                         (WShadow != null && hero.Distance(WShadow.ServerPosition) <= _e.Range))) > 0)
                _e.Cast();
        }

        internal enum UltCastStage
        {
            First,
            Second,
            Cooldown
        }

        internal enum ShadowCastStage
        {
            First,
            Second,
            Cooldown
        }

        private static void KillSteal()
        {
            var target = SimpleTs.GetTarget(2000, SimpleTs.DamageType.Physical);
            var igniteDmg = _player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
            if (target.IsValidTarget() && _config.Item("UseIgnitekill").GetValue<bool>() && _igniteSlot != SpellSlot.Unknown &&
                _player.SummonerSpellbook.CanUseSpell(_igniteSlot) == SpellState.Ready)
            {
                if (igniteDmg > target.Health && _player.Distance(target) <= 600)
                {
                    _player.SummonerSpellbook.CastSpell(_igniteSlot, target);
                }
            }
            if (target.IsValidTarget() && _q.IsReady() && _config.Item("UseQM").GetValue<bool>() && _q.GetDamage(target) > target.Health)
            {
                if (_player.Distance(target) <= _q.Range)
                {
                    _q.Cast(target);
                }
                else if (WShadow != null && WShadow.Distance(target) <= _q.Range)
                {
                    _q.UpdateSourcePosition(WShadow.ServerPosition, WShadow.ServerPosition);
                    _q.Cast(target);
                }
            }
            if (_e.IsReady() && _config.Item("UseEM").GetValue<bool>())
            {
                var t = SimpleTs.GetTarget(_e.Range, SimpleTs.DamageType.Physical);
                if (_e.GetDamage(t) > t.Health && (_player.Distance(t) <= _e.Range || WShadow.Distance(t) <= _e.Range))
                {
                    _e.Cast();
                }
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (RShadow != null)
            {
                Utility.DrawCircle(RShadow.ServerPosition, RShadow.BoundingRadius * 2, Color.Blue);
            }

            if (_config.Item("TheLine").GetValue<KeyBind>().Active)
            {
                Utility.DrawCircle(linepos, 75, Color.Blue);
                Utility.DrawCircle(castpos, 75, Color.Red);
            }
            if (_config.Item("shadowd").GetValue<bool>())
            {
                if (WShadow != null)
                {
                    if (ShadowStage == ShadowCastStage.Cooldown)
                    {
                        Utility.DrawCircle(WShadow.ServerPosition, WShadow.BoundingRadius * 2, Color.Red);
                    }
                    else if (WShadow != null && ShadowStage == ShadowCastStage.Second)
                    {
                        Utility.DrawCircle(WShadow.ServerPosition, WShadow.BoundingRadius * 2, Color.Yellow);
                    }
                }
            }
            if (_config.Item("damagetest").GetValue<bool>())
            {
                foreach (
                    var enemyVisible in
                        ObjectManager.Get<Obj_AI_Hero>().Where(enemyVisible => enemyVisible.IsValidTarget()))
                {
                    if (enemyVisible.HasBuff("zedulttargetmark", true) && enemyVisible.Health <
                            (hppi * (_r.Level * 0.15 + 0.05)  - 3 * ((ObjectManager.Player.Level - 1) * 4 + 14)))
                    {
                        Drawing.DrawText(Drawing.WorldToScreen(enemyVisible.Position)[0] + 50 ,
                            Drawing.WorldToScreen(enemyVisible.Position)[1] -20, Color.Yellow,
                            "deathmark will kill");
                    }

                    if (ComboDamage(enemyVisible) > enemyVisible.Health)
                    {
                        Drawing.DrawText(Drawing.WorldToScreen(enemyVisible.Position)[0] + 50,
                            Drawing.WorldToScreen(enemyVisible.Position)[1] - 40, Color.Red,
                            "Combo=Rekt");
                    }
                    else if (ComboDamage(enemyVisible) + _player.GetAutoAttackDamage(enemyVisible, true) * 2 >
                             enemyVisible.Health)
                    {
                        Drawing.DrawText(Drawing.WorldToScreen(enemyVisible.Position)[0] + 50,
                            Drawing.WorldToScreen(enemyVisible.Position)[1] - 40, Color.Orange,
                            "Combo+AA=Rekt");
                    }
                    else
                        Drawing.DrawText(Drawing.WorldToScreen(enemyVisible.Position)[0] + 50,
                            Drawing.WorldToScreen(enemyVisible.Position)[1] - 40, Color.Green,
                            "Unkillable");
                }
            }

            if (_config.Item("CircleLag").GetValue<bool>())
            {
                if (_config.Item("DrawQ").GetValue<bool>())
                {
                    Utility.DrawCircle(ObjectManager.Player.Position, _q.Range, System.Drawing.Color.Blue,
                        _config.Item("CircleThickness").GetValue<Slider>().Value,
                        _config.Item("CircleQuality").GetValue<Slider>().Value);
                }
                if (_config.Item("DrawE").GetValue<bool>())
                {
                    Utility.DrawCircle(ObjectManager.Player.Position, _e.Range, System.Drawing.Color.White,
                        _config.Item("CircleThickness").GetValue<Slider>().Value,
                        _config.Item("CircleQuality").GetValue<Slider>().Value);
                }
                if (_config.Item("DrawQW").GetValue<bool>() && _config.Item("longhar").GetValue<KeyBind>().Active)
                {
                    Utility.DrawCircle(ObjectManager.Player.Position, 1400, System.Drawing.Color.Yellow,
                        _config.Item("CircleThickness").GetValue<Slider>().Value,
                        _config.Item("CircleQuality").GetValue<Slider>().Value);
                }
                if (_config.Item("DrawR").GetValue<bool>())
                {
                    Utility.DrawCircle(ObjectManager.Player.Position, _r.Range, System.Drawing.Color.Blue,
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
                if (_config.Item("DrawE").GetValue<bool>())
                {
                    Drawing.DrawCircle(ObjectManager.Player.Position, _e.Range, System.Drawing.Color.White);
                }
                if (_config.Item("DrawQW").GetValue<bool>() && _config.Item("longhar").GetValue<KeyBind>().Active)
                {
                    Drawing.DrawCircle(ObjectManager.Player.Position, 1400, System.Drawing.Color.White);
                }
                if (_config.Item("DrawR").GetValue<bool>())
                {
                    Drawing.DrawCircle(ObjectManager.Player.Position, _r.Range, System.Drawing.Color.White);
                }
            }
        }
    }
}

#region
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
#endregion

namespace JeonProject
{
    class Program
    {
        public static Menu baseMenu;
        public static Obj_AI_Hero Player = ObjectManager.Player;
        public static System.Drawing.Rectangle Monitor = System.Windows.Forms.Screen.PrimaryScreen.Bounds;

        public static SpellSlot[] SSpellSlots = { SpellSlot.Summoner1, SpellSlot.Summoner2 };
        public static SpellSlot[] SpellSlots = { SpellSlot.Q, SpellSlot.W, SpellSlot.E, SpellSlot.R };
        public static SpellSlot smiteSlot = SpellSlot.Unknown;
        public static SpellSlot igniteSlot = SpellSlot.Unknown;
        public static SpellSlot defslot = SpellSlot.Unknown;
        public static Spell smite;
        public static Spell ignite;
        public static Spell defspell;
        public static Spell jumpspell;

        public static bool canw2j = false;
        public static bool rdyw2j = false;
        public static bool rdyward = false;
        public static bool text_Isrender = false;
        public static bool textsmite_Isrender = false;

        public static int req_ignitelevel { get { return baseMenu.Item("igniteLv").GetValue<Slider>().Value; } }

        public static int X = 0;
        public static int Y = 0;
        public static int pastTime = 0;
        public static int tempItemid;



        public static String[] DefSpellstr = { "barrier", "heal" };

        public static Render.Text text_notifier = new Render.Text("Can Ult to kill!", Player, new Vector2(0, 50), (int)32, SharpDX.ColorBGRA.FromRgba(0xFF00FFBB));
        public static Render.Text text_help = new Render.Text("Somebody Need Help!", Player, new Vector2(0, 50), (int)32, SharpDX.ColorBGRA.FromRgba(0xFF00FFBB));
        public static Render.Text text_smite = new Render.Text("AutoSmite!", Player, new Vector2(55, 50), (int)30, SharpDX.ColorBGRA.FromRgba(0xFF0000FF));

        public enum test
        {
            show_inventory,
            show_enemybuff,
            show_allybuff,
            show_mebuff
        }

        private static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += OnGameLoad;
            Game.OnGameUpdate += OnGameUpdate;
            Drawing.OnEndScene += OnDraw_EndScene;
            Drawing.OnDraw += OnDraw;
        }

        private static void OnGameLoad(EventArgs args)
        {
            Game.PrintChat("<font color ='#33FFFF'>Jeon瀹炵敤鍔熻兘鍚堜竴 v1.0 </font>鍔犺級鎴愬姛锛佹饥鍖朾y浜岀嫍锛丵Q缇361630847!");
            setSmiteSlot();
            setIgniteSlot();
            setDefSpellSlot();

            //메인메뉴 - Main Menu
            baseMenu = new Menu("Jeon实用功能合一", "ProjectJ", true);
            baseMenu.AddToMainMenu();
            baseMenu.AddItem(new MenuItem("base_stat", "功能生效范围").SetValue(true));
            baseMenu.AddItem(new MenuItem("x", "x").SetValue(new Slider(600, 0, Monitor.Width)));
            baseMenu.AddItem(new MenuItem("y", "y").SetValue(new Slider(250, 0, Monitor.Height)));
            //baseMenu.AddItem(new MenuItem("test", "test").SetValue(false));

            var menu_smite = new Menu("惩戒", "Smite");
            var menu_ignite = new Menu("点燃", "Ignite");
            var menu_tracker = new Menu("追踪器（ CD）", "Tracker");
            var menu_j2w = new Menu("瞬眼", "Jump2Ward");
            var menu_st = new Menu("堆叠伤害", "Stacks");
            var menu_ins = new Menu("物品&全图大", "Item & Spell");
            var menu_noti = new Menu("通知器", "Notifier");


            #region 스마이트 메뉴 - menu for smite
            baseMenu.AddSubMenu(menu_smite);
            menu_smite.AddItem(new MenuItem("AutoSmite", "自动惩戒").SetValue(true));
            menu_smite.AddItem(new MenuItem("smite_enablekey", "启用键位:").SetValue(new KeyBind('K', KeyBindType.Toggle)));// 32 - Space
            menu_smite.AddItem(new MenuItem("smite_holdkey", "保持键位:").SetValue(new KeyBind(32, KeyBindType.Press)));// 32 - Space
            #endregion

            #region 점화 메뉴 - menu for ignite
            baseMenu.AddSubMenu(menu_ignite);
            menu_ignite.AddItem(new MenuItem("AutoIgnite", "自动点燃").SetValue(true));
            menu_ignite.AddItem(new MenuItem("igniteLv", "启用 等级 :").SetValue(new Slider(1, 1, 18)));
            #endregion

            #region 트래커 메뉴 - menu for tracker
            baseMenu.AddSubMenu(menu_tracker);
            menu_tracker.AddItem(new MenuItem("tracker_enemyspells", "显示敌人技能CD").SetValue(true));

            #endregion

            #region 점프와드 메뉴 - menu for Jump2Ward
            baseMenu.AddSubMenu(menu_j2w);
            menu_j2w.AddItem(new MenuItem("j2w_bool", "瞬眼").SetValue(true));
            menu_j2w.AddItem(new MenuItem("j2w_hkey", "键位: ").SetValue(new KeyBind('T', KeyBindType.Press)));
            menu_j2w.AddItem(new MenuItem("j2w_info", "信息").SetValue(false));
            #endregion

            #region 스택 메뉴 - menu for stacks
            baseMenu.AddSubMenu(menu_st);
            menu_st.AddItem(new MenuItem("st_bool", "显示法术伤害").SetValue(true));
            menu_st.AddItem(new MenuItem("st_twitch", "自动 图奇(E)").SetValue(false));
            menu_st.AddItem(new MenuItem("st_kalista", "自动 卡利斯塔(E)").SetValue(false));
            #endregion

            #region 아이템사용 메뉴 - menu for UseItem&Spell
            baseMenu.AddSubMenu(menu_ins);

            var menu_jhonya = new Menu("中亚", "zhonya");
            menu_ins.AddSubMenu(menu_jhonya);
            menu_jhonya.AddItem(new MenuItem("useitem_zhonya", "使用中亚").SetValue(true));
            menu_jhonya.AddItem(new MenuItem("useitem_z_hp", "使用中亚|Hp(%)").SetValue(new Slider(15, 0, 100)));

            var menu_Potion = new Menu("药剂", "Potion");
            menu_ins.AddSubMenu(menu_Potion);
            menu_Potion.AddItem(new MenuItem("useitem_flask", "使用水晶瓶").SetValue(true));
            menu_Potion.AddItem(new MenuItem("useitem_p_fla", "使用水晶瓶|Hp(%)").SetValue(new Slider(50, 0, 100)));
            menu_Potion.AddItem(new MenuItem("useitem_hppotion", "使用红药").SetValue(true));
            menu_Potion.AddItem(new MenuItem("useitem_p_hp", "使用红药|Hp(%)").SetValue(new Slider(50, 0, 100)));
            menu_Potion.AddItem(new MenuItem("useitem_manapotion", "使用蓝药").SetValue(true));
            menu_Potion.AddItem(new MenuItem("useitem_p_mana", "使用蓝药|Mana(%)").SetValue(new Slider(50, 0, 100)));

            var menu_spell = new Menu("法术", "Spell");
            menu_ins.AddSubMenu(menu_spell);
            menu_spell.AddItem(new MenuItem("usespell", "使用法术").SetValue(true));
            menu_spell.AddItem(new MenuItem("usespell_hp", "使用法术|Hp(%)").SetValue(new Slider(10, 0, 100)));

            #endregion

            #region 알림 메뉴 - menu for notifier
            baseMenu.AddSubMenu(menu_noti);
            menu_noti.AddItem(new MenuItem("noti_karthus", "死歌 大招").SetValue(true));
            menu_noti.AddItem(new MenuItem("noti_ez", "EZ 大招").SetValue(true));
            menu_noti.AddItem(new MenuItem("noti_cait", "卡特 大招").SetValue(true));
            menu_noti.AddItem(new MenuItem("noti_shen", "慎 大招").SetValue(true));
            menu_noti.AddItem(new MenuItem("noti_shenhp", "击杀提示|敌方Hp(%)").SetValue(new Slider(10, 0, 100)));
            #endregion
        }
        private static void OnGameUpdate(EventArgs args)
        {
            #region get info
            float Player_bAD = Player.BaseAttackDamage;
            float Player_aAD = Player.FlatPhysicalDamageMod;
            float Player_totalAD = Player_bAD + Player_aAD;
            float Player_bAP = Player.BaseAbilityDamage;
            float Player_aAP = Player.FlatMagicDamageMod;
            float Player_totalAP = Player_bAP + Player_aAP;

            #endregion

            #region 오토스마이트-AutoSmite
            if (baseMenu.Item("AutoSmite").GetValue<bool>() && smiteSlot != SpellSlot.Unknown)
            {
                if ((baseMenu.Item("smite_holdkey").GetValue<KeyBind>().Active || baseMenu.Item("smite_enablekey").GetValue<KeyBind>().Active))
                {
                    double smitedamage;
                    bool smiteReady = false;
                    smitedamage = setSmiteDamage();
                    Drawing.DrawText(Player.HPBarPosition.X + 55, Player.HPBarPosition.Y + 25, System.Drawing.Color.Red, "AutoSmite!");

                    Obj_AI_Base mob = GetNearest(Player.ServerPosition);

                    if (Player.SummonerSpellbook.CanUseSpell(smiteSlot) == SpellState.Ready && Vector3.Distance(Player.ServerPosition, mob.ServerPosition) < smite.Range)
                    {
                        smiteReady = true;
                    }

                    if (smiteReady && mob.Health < smitedamage)
                    {
                        setIgniteSlot();
                        Player.SummonerSpellbook.CastSpell(smiteSlot, mob);
                    }
                }
            }
            #endregion

            #region 오토이그나이트-AutoIgnite
            if (baseMenu.Item("AutoIgnite").GetValue<bool>() && igniteSlot != SpellSlot.Unknown &&
                Player.Level >= req_ignitelevel)
            {
                float ignitedamage;
                bool IgniteReady = false;
                ignitedamage = setigniteDamage();
                foreach (var hero in ObjectManager.Get<Obj_AI_Hero>()
                    .Where(hero => hero != null && hero.IsValid && !hero.IsDead && Player.ServerPosition.Distance(hero.ServerPosition) < ignite.Range
                    && !hero.IsMe && !hero.IsAlly && (hero.Health + hero.HPRegenRate * 2) <= ignitedamage))
                {

                    if (Player.SummonerSpellbook.CanUseSpell(igniteSlot) == SpellState.Ready)
                    {
                        IgniteReady = true;
                    }
                    if (IgniteReady)
                    {
                        setIgniteSlot();
                        Player.SummonerSpellbook.CastSpell(igniteSlot, hero);
                    }
                }
            }
            #endregion

            #region 스펠트레커-Spelltracker
            if (baseMenu.Item("tracker_enemyspells").GetValue<bool>())
            {
                try
                {

                    foreach (var target in
                        ObjectManager.Get<Obj_AI_Hero>().Where(hero => hero != null && hero.IsValid && (!hero.IsMe && hero.IsHPBarRendered)))
                    {

                        X = 10;
                        Y = 40;
                        foreach (var sSlot in SSpellSlots)
                        {
                            var spell = target.SummonerSpellbook.GetSpell(sSlot);
                            var t = spell.CooldownExpires - Game.Time;
                            if (t < 0)
                            {

                                Drawing.DrawText(target.HPBarPosition.X + X + 85, target.HPBarPosition.Y + Y, System.Drawing.Color.FromArgb(255, 0, 255, 0), filterspellname(spell.Name));
                            }
                            else
                            {
                                Drawing.DrawText(target.HPBarPosition.X + X + 85, target.HPBarPosition.Y + Y, System.Drawing.Color.Red, filterspellname(spell.Name));
                            }

                            Y += 15;
                        }
                        Y = 40;
                        foreach (var slot in SpellSlots)
                        {
                            var spell = target.Spellbook.GetSpell(slot);
                            var t = spell.CooldownExpires - Game.Time;
                            if (t < 0)
                            {
                                Drawing.DrawText(target.HPBarPosition.X + X, target.HPBarPosition.Y + Y, System.Drawing.Color.FromArgb(255, 0, 255, 0), Convert.ToString(spell.Level));
                            }
                            else
                            {
                                Drawing.DrawText(target.HPBarPosition.X + X, target.HPBarPosition.Y + Y, System.Drawing.Color.Red, Convert.ToString(spell.Level));
                            }
                            X += 20;
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(@"/ff error : " + e);
                }
            }

            #endregion

            #region 점프와드- Jump2Ward (Jax,Kata,LeeSin)
            if (baseMenu.Item("j2w_bool").GetValue<bool>())
            {
                List<String> champs = new List<String>();
                champs.Add("LeeSin"); champs.Add("Katarina"); champs.Add("Jax");
                setj2wslots(champs);
                if (canw2j)
                {
                    checkE();
                    checkWard();
                    if (rdyw2j && baseMenu.Item("j2w_hkey").GetValue<KeyBind>().Active)
                    {
                        Vector3 cursor = Game.CursorPos;
                        Vector3 myPos = Player.ServerPosition;
                        Player.IssueOrder(GameObjectOrder.MoveTo, cursor);

                        foreach (var target in ObjectManager.Get<Obj_AI_Base>().Where(ward => ward.IsVisible && ward.IsAlly && !ward.IsMe &&
                            Vector3.DistanceSquared(cursor, ward.ServerPosition) <= 200 * 200 &&
                            ward.Distance(Player) <= 700 && ward.Name.IndexOf("Turret") == -1))
                        {
                            jumpspell.CastOnUnit(target);
                        }

                        if (rdyward)
                        {
                            Items.GetWardSlot().UseItem(cursor);
                        }
                    }
                }


                if (baseMenu.Item("j2w_info").GetValue<bool>())
                {
                    Game.PrintChat("Champion : " + Player.BaseSkinName);
                    Game.PrintChat("Can? : " + canw2j);
                    Game.PrintChat("Spell : " + jumpspell.Slot.ToString());
                    Game.PrintChat("WardStack : " + Items.GetWardSlot().Stacks);
                    baseMenu.Item("j2w_info").SetValue<bool>(false);
                }

            }

            #endregion

            #region 스택 - Stacks
            if (baseMenu.Item("st_twitch").GetValue<bool>() && Player.BaseSkinName == "Twitch")
            {
                    Spell E = new Spell(SpellSlot.E, 1200);
                    var target = SimpleTs.GetTarget(E.Range, SimpleTs.DamageType.Physical);
                    if (target.IsValidTarget(E.Range))
                    {
                        foreach (var venoms in target.Buffs.Where(venoms => venoms.DisplayName == "TwitchDeadlyVenom"))
                        {
                            var damage = E.GetDamage(target);
                            //Game.PrintChat("d:{0} hp:{1}",damage,target.Health);
                            if (damage >= target.Health && E.IsReady())
                                E.Cast();

                            if (baseMenu.Item("st_bool").GetValue<bool>())
                            {
                                String t_damage = Convert.ToInt64(damage).ToString() + "(" + venoms.Count + ")";
                                Drawing.DrawText(target.HPBarPosition.X, target.HPBarPosition.Y - 5, Color.Red, t_damage);
                            }
                        }
                    }
            }

            if (baseMenu.Item("st_kalista").GetValue<bool>() && Player.BaseSkinName == "Kalista")
            {
                Spell E = new Spell(SpellSlot.E, 900);
                if (E.IsReady())
                {
                    var target = SimpleTs.GetTarget(E.Range, SimpleTs.DamageType.Physical);
                    if (target.IsValidTarget(E.Range))
                    {
                        foreach (var venoms in target.Buffs.Where(venoms => venoms.DisplayName == "KalistaExpungeMarker"))
                        {
                            var damage = E.GetDamage(target);
                            if (damage >= target.Health)
                                E.Cast();
                            if (baseMenu.Item("st_bool").GetValue<bool>())
                            {
                                String t_damage = Convert.ToInt64(damage).ToString() + "(" + venoms.Count + ")";
                                Drawing.DrawText(target.HPBarPosition.X, target.HPBarPosition.Y - 5, Color.Red, t_damage);
                            }
                        }
                    }

                    Obj_AI_Base mob = GetNearest(Player.ServerPosition);
                    foreach (var venoms in mob.Buffs.Where(venoms => venoms.DisplayName == "KalistaExpungeMarker"))
                    {
                        var damage = getKaliDmg(mob, venoms.Count, Player_totalAD, E.Level);
                        if (damage >= mob.Health && Vector3.Distance(mob.Position, Player.Position) <= 900
                            && (mob.Name.Contains("SRU_Dragon") || mob.Name.Contains("SRU_Baron")))
                            E.Cast();

                        if (baseMenu.Item("st_bool").GetValue<bool>())
                        {
                            String t_damage = Convert.ToInt64(damage).ToString() + "(" + venoms.Count + ")";
                            Drawing.DrawText(mob.HPBarPosition.X, mob.HPBarPosition.Y - 5, Color.Red, t_damage);
                        }
                    }
                }
            }
            #endregion

            #region Items&spells
            //item
            tempItemid = 3157;
            if (baseMenu.Item("useitem_zhonya").GetValue<bool>()&&Items.HasItem(tempItemid))
            {
                foreach (var p_item in Player.InventoryItems.Where(item => item.Id == ItemId.Zhonyas_Hourglass))
                {
                    if (Player.HealthPercentage() <= (float)baseMenu.Item("useitem_z_hp").GetValue<Slider>().Value && Items.CanUseItem(tempItemid))
                    {
                        p_item.UseItem();
                    }
                }
            }
            tempItemid = Convert.ToInt32(ItemId.Crystalline_Flask);
            if (baseMenu.Item("useitem_flask").GetValue<bool>() && Items.HasItem(tempItemid))
            {
                foreach (var p_item in Player.InventoryItems.Where(item => item.Id == ItemId.Crystalline_Flask && !Player.HasBuff("ItemCrystalFlask")))
                {
                    if (Player.HealthPercentage() <= (float)baseMenu.Item("useitem_p_fla").GetValue<Slider>().Value && Items.CanUseItem(tempItemid))
                    {
                        p_item.UseItem();
                    }
                }
            }
            tempItemid = Convert.ToInt32(ItemId.Health_Potion);
            if (baseMenu.Item("useitem_hppotion").GetValue<bool>() && Items.HasItem(tempItemid))
            {
                foreach (var p_item in Player.InventoryItems.Where(item => item.Id == ItemId.Health_Potion && !Player.HasBuff("Health Potion") && !Player.HasBuff("ItemCrystalFlask")))
                {
                    if (Player.HealthPercentage() <= (float)baseMenu.Item("useitem_p_hp").GetValue<Slider>().Value && Items.CanUseItem(Convert.ToInt32(ItemId.Health_Potion)))
                    {
                        p_item.UseItem();
                    }
                }
            }

            tempItemid = Convert.ToInt32(ItemId.Mana_Potion);
            if (baseMenu.Item("useitem_manapotion").GetValue<bool>() && Items.HasItem(tempItemid))
            {
                foreach (var p_item in Player.InventoryItems.Where(item => item.Id == ItemId.Mana_Potion && !Player.HasBuff("Mana Potion") && !Player.HasBuff("ItemCrystalFlask")))
                {
                    if (Player.HealthPercentage() <= (float)baseMenu.Item("useitem_p_mana").GetValue<Slider>().Value && Items.CanUseItem(tempItemid))
                    {
                        p_item.UseItem();
                    }
                }
            }


            //spell
            if (baseMenu.Item("usespell").GetValue<bool>()&& defslot != SpellSlot.Unknown)
            {
                if (Player.HealthPercentage() <= (float)baseMenu.Item("usespell_hp").GetValue<Slider>().Value)
                {
                    if (Player.SummonerSpellbook.CanUseSpell(defslot) == SpellState.Ready)
                        Player.SummonerSpellbook.CastSpell(defslot);
                }
            }

            #endregion

            #region ultnotifier
            //Karthus
            if (Player.BaseSkinName == "Karthus")
            {
                if (baseMenu.Item("noti_karthus").GetValue<bool>() && Player.Spellbook.CanUseSpell(SpellSlot.R) == SpellState.Ready)
                {
                    Spell R = new Spell(SpellSlot.R, 100000);
                    var target = SimpleTs.GetTarget(R.Range, SimpleTs.DamageType.Magical);
                    var damage = R.GetDamage(target);


                    if (target.IsValidTarget(R.Range) && target.IsVisible && damage >= target.Health + target.HPRegenRate * 3)
                    {
                        if (!text_Isrender)
                            text_notifier.Add();
                        text_Isrender = true;
                    }
                }
                else
                {
                    text_notifier.Remove();
                    text_Isrender = false;
                }
            }
            //cait
            if (Player.BaseSkinName == "Caitlyn")
            {
                if (baseMenu.Item("noti_cait").GetValue<bool>() && Player.Spellbook.CanUseSpell(SpellSlot.R) == SpellState.Ready)
                {
                    Spell R = new Spell(SpellSlot.R, 1500 + (500 * Player.Spellbook.GetSpell(SpellSlot.R).Level));
                    var target = SimpleTs.GetTarget(R.Range, SimpleTs.DamageType.Physical);
                    var damage = R.GetDamage(target);


                    if (target.IsValidTarget(R.Range) && target.IsVisible && damage >= target.Health + target.HPRegenRate)
                    {
                        if (!text_Isrender)
                            text_notifier.Add();
                        text_Isrender = true;
                        targetPing(target.Position.To2D());


                    }
                }
                else
                {
                    text_notifier.Remove();
                    text_Isrender = false;
                }
            }
            //ez
            if (Player.BaseSkinName == "Ezreal")
            {
                if (baseMenu.Item("noti_ez").GetValue<bool>() && Player.Spellbook.CanUseSpell(SpellSlot.R) == SpellState.Ready)
                {
                    Spell R = new Spell(SpellSlot.R, 100000);
                    var target = SimpleTs.GetTarget(R.Range, SimpleTs.DamageType.Magical);
                    var damage = R.GetDamage(target);


                    if (target.IsValidTarget(R.Range) && target.IsVisible && damage >= target.Health + target.HPRegenRate * (2000f / Vector3.Distance(Player.ServerPosition, target.ServerPosition))) // time=speed/distance
                    {
                        if (!text_Isrender)
                            text_notifier.Add();
                        text_Isrender = true;
                        targetPing(target.Position.To2D());
                    }
                }
                else
                {
                    text_notifier.Remove();
                    text_Isrender = false;
                }
            }
            //shen
            if (Player.BaseSkinName == "Shen")
            {
                if (baseMenu.Item("noti_shen").GetValue<bool>() && Player.Spellbook.CanUseSpell(SpellSlot.R) == SpellState.Ready)
                {

                    foreach (var hero in ObjectManager.Get<Obj_AI_Hero>().Where(hero => hero.IsAlly && !hero.IsMe && !hero.IsDead && hero.IsValid))
                    {
                        if (hero.HealthPercentage() <= baseMenu.Item("noti_shenhp").GetValue<Slider>().Value)
                        {
                            if (!text_Isrender)
                                text_help.Add();
                            text_Isrender = true;

                            targetPing(hero.Position.To2D(), Packet.PingType.AssistMe);
                            Game.PrintChat("print!");
                        }
                        else
                        {
                            text_help.Remove();
                            text_Isrender = false;
                        }
                    }
                }
                else
                {
                    text_help.Remove();
                    text_Isrender = false;
                }
            }
            #endregion

            #region Status on hud
            if (baseMenu.Item("base_stat").GetValue<bool>())
            {
                /*
                 * 오토스마이트
                 * 오토이그나이트
                 * 점프와드
                 * 스택
                 * Items
                 * Spell
                 */

                int x = Monitor.Width - baseMenu.Item("x").GetValue<Slider>().Value;
                int y = Monitor.Height - baseMenu.Item("y").GetValue<Slider>().Value;
                int interval = 20;
                int i = 0;
                Color text_color = Color.Red;

                Drawing.DrawText(x, y + (interval * i), Color.Wheat, "Champion : " + Player.BaseSkinName);
                i++; 

                Drawing.DrawText(x, y + (interval * i), Color.Wheat, "Spells : " + filterspellname(Player.SummonerSpellbook.GetSpell(SpellSlot.Summoner1).Name) + "," +
            filterspellname(Player.SummonerSpellbook.GetSpell(SpellSlot.Summoner2).Name));
                i++;

                if (smiteSlot != SpellSlot.Unknown)
                {
                    addText(y + (interval * i), (baseMenu.Item("AutoSmite").GetValue<bool>() && smiteSlot != SpellSlot.Unknown), "AutoSmite");
                    i++;
                }

                if (igniteSlot != SpellSlot.Unknown)
                {
                    addText(y + (interval * i), (baseMenu.Item("AutoIgnite").GetValue<bool>() && igniteSlot != SpellSlot.Unknown), "AutoIgnite");
                    i++;
                }

                if (jumpspell != null)
                {
                    addText(y + (interval * i), (baseMenu.Item("j2w_bool").GetValue<bool>() && jumpspell != null), "Jump2Ward");
                    i++;
                }

                addText(y + (interval * i), (baseMenu.Item("useitem_zhonya").GetValue<bool>()), "CastZhonya");
                i++;

                if (defslot != SpellSlot.Unknown)
                {
                    addText(y + (interval * i), (baseMenu.Item("usespell").GetValue<bool>() && defslot != SpellSlot.Unknown), string.Format("SpellCast{0}", filterspellname(Player.SummonerSpellbook.GetSpell(defslot).Name).ToUpper()));
                    i++;
                }

                addText(y + (interval * i), (baseMenu.Item("useitem_flask").GetValue<bool>()), "Use Flask");
                i++;

                addText(y + (interval * i), (baseMenu.Item("useitem_hppotion").GetValue<bool>()), "Use HP Potion");
                i++;

                addText(y + (interval * i), (baseMenu.Item("useitem_hppotion").GetValue<bool>()), "Use Mana Potion");
                i++;
                //champ
                #region stack
                if (Player.BaseSkinName == "Twitch")
                {
                    addText(y + (interval * i), (baseMenu.Item("st_twitch").GetValue<bool>()), "CastTwitchE");
                    i++;
                }
                if (Player.BaseSkinName == "Kalista")
                {
                    addText(y + (interval * i), (baseMenu.Item("st_kalista").GetValue<bool>()), "CastKalistaE");
                    i++;
                }
                #endregion
                #region notifier
                if (Player.BaseSkinName == "Karthus")
                {
                    addText(y + (interval * i), (baseMenu.Item("noti_karthus").GetValue<bool>()), "UltNotifiler");
                }
                if (Player.BaseSkinName == "Ezreal")
                {
                    addText(y + (interval * i), (baseMenu.Item("noti_ez").GetValue<bool>()), "UltNotifiler");
                }
                if (Player.BaseSkinName == "Caitlyn")
                {
                    addText(y + (interval * i), (baseMenu.Item("noti_cait").GetValue<bool>()), "UltNotifiler");
                }
                if (Player.BaseSkinName == "Shen")
                {
                    addText(y + (interval * i), (baseMenu.Item("noti_shen").GetValue<bool>()), "UltNotifiler");
                }
                #endregion
            }
            #endregion

            #region test

            /*
            if (baseMenu.Item("test").GetValue<bool>()){

                testf(test.show_mebuff);

                baseMenu.Item("test").SetValue<bool>(false);
            }
             */
            #endregion
        }
        public static void OnDraw_EndScene(EventArgs args)
        {

        }
        public static void OnDraw(EventArgs args)
        {

        }

        // Addional Function //
        #region 스마이트함수 - Smite Function

        public static readonly int[] SmitePurple = { 3713, 3726, 3725, 3726, 3723 };
        public static readonly int[] SmiteGrey = { 3711, 3722, 3721, 3720, 3719 };
        public static readonly int[] SmiteRed = { 3715, 3718, 3717, 3716, 3714 };
        public static readonly int[] SmiteBlue = { 3706, 3710, 3709, 3708, 3707 };
        private static readonly string[] MinionNames =
        {
        "TT_Spiderboss", "TTNGolem", "TTNWolf", "TTNWraith",
            "SRU_Blue", "SRU_Gromp", "SRU_Murkwolf", "SRU_Razorbeak", "SRU_Red", "SRU_Krug", "SRU_Dragon", "SRU_BaronSpawn", "Sru_Crab"
        };


        public static void setSmiteSlot()
        {
            foreach (var spell in Player.SummonerSpellbook.Spells.Where(spell => String.Equals(spell.Name, smitetype(), StringComparison.CurrentCultureIgnoreCase)))
            {

                smiteSlot = spell.Slot;
                smite = new Spell(smiteSlot, 700);
                return;
            }
        }
        public static string smitetype()
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
        public static Obj_AI_Minion GetNearest(Vector3 pos)
        {
            var minions =
                ObjectManager.Get<Obj_AI_Minion>()
                    .Where(minion => minion.IsValid && MinionNames.Any(name => minion.Name.StartsWith(name)) && !MinionNames.Any(name => minion.Name.Contains("Mini")));
            var objAiMinions = minions as Obj_AI_Minion[] ?? minions.ToArray();
            Obj_AI_Minion sMinion = objAiMinions.FirstOrDefault();
            double? nearest = null;
            foreach (Obj_AI_Minion minion in objAiMinions)
            {
                double distance = Vector3.Distance(pos, minion.Position);
                if (nearest == null || nearest > distance)
                {
                    nearest = distance;
                    sMinion = minion;
                }
            }
            return sMinion;
        }
        public static double setSmiteDamage()
        {
            int level = Player.Level;
            int[] damage =
            {
                20*level + 370,
                30*level + 330,
                40*level + 240,
                50*level + 100
            };
            return damage.Max();
        }
        public static void testFind(Vector3 pos)
        {
            double? nearest = null;
            var minions =
                ObjectManager.Get<Obj_AI_Minion>()
                    .Where(minion => minion.IsValid);
            var objAiMinions = minions as Obj_AI_Minion[] ?? minions.ToArray();
            Obj_AI_Minion sMinion = objAiMinions.FirstOrDefault();
            foreach (Obj_AI_Minion minion in minions)
            {
                double distance = Vector3.Distance(pos, minion.Position);
                if (nearest == null || nearest > distance)
                {
                    nearest = distance;
                    sMinion = minion;
                }
            }
            Game.PrintChat("Minion name is: " + sMinion.Name);
        }
        #endregion

        #region 이그나이트 함수 - Ignite
        public static void setIgniteSlot()
        {
            foreach (var spell in Player.SummonerSpellbook.Spells.Where(spell => String.Equals(spell.Name, "summonerdot", StringComparison.CurrentCultureIgnoreCase)))
            {
                igniteSlot = spell.Slot;
                ignite = new Spell(smiteSlot, 600);
                return;
            }
        }

        public static float setigniteDamage()
        {
            float dmg = 50 + 20 * Player.Level;
            return dmg;
        }
        #endregion

        #region 트래커함수 - Tracker
        public static string filterspellname(String a)
        {
            switch (a)
            {
                case "s5_summonersmiteplayerganker":
                    a = "BSmite"; break;
                case "s5_summonersmiteduel":
                    a = "RSmite"; break;
                case "s5_summonersmitequick":
                    a = "Smite"; break;
                case "itemsmiteaoe":
                    a = "Smite"; break;
                default:
                    break;
            }
            a = a.Replace("summoner", "").Replace("dot", "ignite");

            return a;
        }
        #endregion

        #region 점프와드함수 - J2W
        public static void setj2wslots(List<String> a)
        {
            foreach (String champname in a)
            {
                if (champname == Player.BaseSkinName)
                {
                    canw2j = true;
                    switch (champname)
                    {
                        case "LeeSin":
                            jumpspell = new Spell(SpellSlot.W, 700);
                            return;
                        case "Katarina":
                            jumpspell = new Spell(SpellSlot.E, 700);
                            return;
                        case "Jax":
                            jumpspell = new Spell(SpellSlot.Q, 700);
                            return;
                    }
                }
            }
        }
        public static void checkE()
        {
            if (Player.BaseSkinName == "LeeSin")
            {
                rdyw2j = jumpspell.IsReady() && Player.Spellbook.GetSpell(SpellSlot.W).Name == "BlindMonkWOne";
            }
            else
            {
                rdyw2j = jumpspell.IsReady();
            }
        }

        public static void checkWard()
        {
            var Slot = Items.GetWardSlot();
            rdyward = !(Slot == null || Slot.Stacks == 0);
        }


        #endregion

        #region 스펠함수 - Item & Spell
        public static void setDefSpellSlot()
        {
            foreach (var spell in Player.SummonerSpellbook.Spells.Where(spell => spell.Name.Contains(DefSpellstr[0]) || spell.Name.Contains(DefSpellstr[1])))
            {
                defslot = spell.Slot;
                defspell = new Spell(defslot);
                return;
            }
        }
        #endregion

        #region 스택함수 - Stack
        public static double getKaliDmg(Obj_AI_Base target,int count,double AD,int s_level)
        {
            double[] spell_basedamage ={0,20,30,40,50,60};
            double[] spell_perdamage ={0,0.25,0.30,0.35,0.40,0.45};
            double eDmg = AD * 0.60 + spell_basedamage[s_level];
            count -= 1;
            eDmg = eDmg + count*(eDmg * spell_perdamage[s_level]);
            return ObjectManager.Player.CalcDamage(target, Damage.DamageType.Physical,eDmg);
        }

        #endregion

        #region status 함수 - Status
        public static void addText(float y,bool a,String b)
        {
            Drawing.DrawText(Monitor.Width - baseMenu.Item("x").GetValue<Slider>().Value, y, a ? Color.FromArgb(0, 255, 0) : Color.Red,
                    b+"(" + bool2string(a) + ")");
        }
        #endregion
        // common function //
        public static string bool2string(bool a)
        {
            String total;
            if(a)
            {
                total = "ON";
            }
            else
            {
                total = "OFF";
            }
            return total;
        }

        public static void targetPing(Vector2 Position)
        {
            if (Environment.TickCount - pastTime < 2000)
                return;
            pastTime = Environment.TickCount;
            Packet.S2C.Ping.Encoded(new Packet.S2C.Ping.Struct(Position.X, Position.Y,0,0,Packet.PingType.Danger)).Process();
        }
        public static void targetPing(Vector2 Position, Packet.PingType ptype)
        {
            if (Environment.TickCount - pastTime < 2000)
                return;
            pastTime = Environment.TickCount;
            Packet.S2C.Ping.Encoded(new Packet.S2C.Ping.Struct(Position.X, Position.Y, 0, 0, ptype)).Process();
        }


        public static void testf(test a)
        {
            switch(a)
            {
                case test.show_inventory:
                    
                        foreach (var temp in Player.InventoryItems)
                                    {
                                        Game.PrintChat("Slot : {0} || id : {1} || name {2}", temp.Slot, Convert.ToInt32(temp.Id), temp.Name);
                                    }
                        return;

                case test.show_allybuff:
                        foreach (var target in ObjectManager.Get<Obj_AI_Hero>().Where(hero => !hero.IsMe && hero.IsValid && hero.IsAlly))
                        {
                            String temptext="";
                            foreach (var venoms in target.Buffs)
                            {
                                temptext = temptext + " " + venoms.DisplayName;
                            }
                            Game.PrintChat("Name: {0} || Buff: {1}", target.BaseSkinName, temptext);
                        }
                        return;

                case test.show_enemybuff:
                    
                        foreach (var target in ObjectManager.Get<Obj_AI_Hero>().Where(hero => !hero.IsMe && hero.IsValid && !hero.IsAlly))
                        {
                            String temptext="";
                            foreach (var venoms in target.Buffs)
                            {
                                temptext = temptext + " " + venoms.DisplayName;
                            }
                            Game.PrintChat("Name: {0} || Buff: {1}", target.BaseSkinName, temptext);
                        }
                        return;

                case test.show_mebuff:
                    
                        foreach (var target in ObjectManager.Get<Obj_AI_Hero>().Where(hero => hero.IsMe && hero.IsValid))
                        {
                            String temptext="";
                            foreach (var venoms in target.Buffs)
                            {
                                temptext = temptext + " " + venoms.DisplayName;
                            }
                            Game.PrintChat("Name: {0} || Buff: {1}", target.BaseSkinName, temptext);
                        }
                        return;
            }
        }

    }
}




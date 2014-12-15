using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LeagueSharp;
using LeagueSharp.Common;
using System.Drawing;
using SharpDX;
using Color = System.Drawing.Color;

namespace TheHatinEvelynn
{
    internal class Program
    {
        public static string ChampionName = "Evelynn";
        public static Orbwalking.Orbwalker Orbwalker;
        public static Dictionary<string, int> numSkins = new Dictionary<string, int>();
        public static int currSkinId = 0;
        public static string WelcomeMsg = ("<font color = '#6600cc'>TheHatin' |浼婅姍鐞硘 </font><font color='#FFFFFF'>by Da'ath.</font> <font color = '#66ff33'> ~~ 鍔犺級鎴愬姛锛佹饥鍖朾y浜岀嫍锛丵Q缇361630847  ~~</font> ");
        private static Obj_AI_Hero Player;
        // Spells
        #region
        public static List<Spell> SpellList = new List<Spell>();
        public static SpellSlot IgniteSlot;
        public static SpellSlot SmiteSlot;

        public static Spell Q;
        public static Spell W;
        public static Spell E;
        public static Spell R;
        #endregion

        // Items
        #region
        public static Items.Item DFG;
        public static Items.Item BotRK;
        public static Items.Item HexGunBlade;
        public static Items.Item QuickS;
        public static Items.Item Cutlass;
        public static Items.Item Scimitar;
        //public static Items.Item Omen;
        //public static Items.Item Zhonya;

        #endregion

        //Menu
        public static Menu Menu;

        private static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        private static void Game_OnGameLoad(EventArgs args)
        {
            Player = ObjectManager.Player;
            if (Player.BaseSkinName != ChampionName) return;
            numSkins.Add("Evelynn", 4);
            Game.PrintChat(WelcomeMsg);

            //Create the spells
            #region
            Q = new Spell(SpellSlot.Q, 700f);
            W = new Spell(SpellSlot.W, 0);
            E = new Spell(SpellSlot.E, 225f);
            R = new Spell(SpellSlot.R, 650f);
            R.SetSkillshot(0.25f, 250f, float.MaxValue, false, SkillshotType.SkillshotCircle);

            SpellList.Add(Q);
            SpellList.Add(W);
            SpellList.Add(E);
            SpellList.Add(R);

            IgniteSlot = Player.GetSpellSlot("SummonerDot");
            SmiteSlot = Player.GetSpellSlot("SummonerSmite");
            #endregion

            //Create the items
            #region
            DFG = new Items.Item(3128, 750f);
            BotRK = new Items.Item(3153, 450f);
            HexGunBlade = new Items.Item(3146, 700f);
            QuickS = new Items.Item(3140, 0f);
            Cutlass = new Items.Item(3144, 450f);
            Scimitar = new Items.Item(3139, 0f);
            #endregion

            //Create the menu
            #region
            Menu = new Menu("寡妇制造者-伊芙琳", "Evelynn", true);

            var targetSelectorMenu = new Menu("目标选择", "Target Selector");
            SimpleTs.AddToMenu(targetSelectorMenu);
            Menu.AddSubMenu(targetSelectorMenu);

            Menu.AddSubMenu(new Menu("走砍菜单", "Orbwalker Menu"));
            Orbwalker = new Orbwalking.Orbwalker(Menu.SubMenu("Orbwalker Menu"));
            #endregion

            //Add Combo SubMenu
            #region
            Menu.AddSubMenu(new Menu("连招", "Combo"));
            Menu.SubMenu("Combo").AddItem(new MenuItem("UseQCombo", "使用 Q").SetValue(true));
            //Menu.SubMenu("Combo").AddItem(new MenuItem("UseWCombo", "Use W").SetValue(true));
            Menu.SubMenu("Combo").AddItem(new MenuItem("UseECombo", "使用 E").SetValue(true));
            Menu.SubMenu("Combo").AddItem(new MenuItem("UseRCombo", "使用 R").SetValue(true));
            Menu.SubMenu("Combo").AddItem(new MenuItem("UseItemsCombo", "使用 物品").SetValue(true));
            Menu.SubMenu("Combo").AddItem(new MenuItem("UseIgniteCombo", "使用 点燃").SetValue(true));
            Menu.SubMenu("Combo").AddItem(new MenuItem("ComboActive", "启用 连招").SetValue(new KeyBind(32, KeyBindType.Press)));
            #endregion

            //Add LaneClear SubMenu
            #region
            Menu.AddSubMenu(new Menu("清线", "LaneClear"));
            Menu.SubMenu("LaneClear").AddItem(new MenuItem("UseQLaneClear", "使用 Q").SetValue(true));
            Menu.SubMenu("LaneClear").AddItem(new MenuItem("UseELaneClear", "使用 E").SetValue(false));
            Menu.SubMenu("LaneClear").AddItem(new MenuItem("LaneClearActive", "启用 清线").SetValue(new KeyBind("V".ToArray()[0], KeyBindType.Press)));
            Menu.SubMenu("LaneClear").AddSubMenu(new Menu("蓝量限制", "laneMana"));
            Menu.SubMenu("LaneClear").SubMenu("laneMana").AddItem(new MenuItem("manaLActive", "启用").SetValue(true));
            Menu.SubMenu("LaneClear").SubMenu("laneMana").AddItem(new MenuItem("onQL", "使用Q最低蓝量").SetValue(new Slider(30, 1, 100)));
            Menu.SubMenu("LaneClear").SubMenu("laneMana").AddItem(new MenuItem("onEL", "使用E最低蓝量").SetValue(new Slider(60, 1, 100)));


            #endregion

            //Add JungleFarm SubMenu
            #region
            Menu.AddSubMenu(new Menu("清野", "JungleFarm"));
            Menu.SubMenu("JungleFarm").AddItem(new MenuItem("UseQJungleFarm", "使用 Q").SetValue(true));
            Menu.SubMenu("JungleFarm").AddItem(new MenuItem("UseEJungleFarm", "使用 E").SetValue(true));
            Menu.SubMenu("JungleFarm").AddItem(new MenuItem("UseSmite", "使用 惩戒").SetValue(true));
            Menu.SubMenu("JungleFarm").AddItem(new MenuItem("JungleFarmActive", "启用 清野").SetValue(new KeyBind("V".ToArray()[0], KeyBindType.Press)));
            Menu.SubMenu("JungleFarm").AddSubMenu(new Menu("蓝量限制", "jungleMana"));
            Menu.SubMenu("LaneClear").SubMenu("laneMana").AddItem(new MenuItem("manaLActive", "启用").SetValue(true));
            Menu.SubMenu("LaneClear").SubMenu("laneMana").AddItem(new MenuItem("onQL", "使用Q最低蓝量").SetValue(new Slider(30, 1, 100)));
            Menu.SubMenu("LaneClear").SubMenu("laneMana").AddItem(new MenuItem("onEL", "使用E最低蓝量").SetValue(new Slider(60, 1, 100)));
            #endregion

            //Add Items SubMenu
            #region
            Menu.AddSubMenu(new Menu("物品", "Items"));
            Menu.SubMenu("Items").AddItem(new MenuItem("UseDFGItems", "使用 冥火").SetValue(true));
            Menu.SubMenu("Items").AddItem(new MenuItem("UseBotRKItems", "使用 破败").SetValue(true));
            Menu.SubMenu("Items").AddItem(new MenuItem("UseHexGunBladeItems", "使用 科技枪").SetValue(true));
            //Menu.SubMenu("Items").AddItem(new MenuItem("UseQuickSItems", "Use Quicksilver Sash").SetValue(true));
            Menu.SubMenu("Items").AddItem(new MenuItem("UseCutlassItems", "使用 小弯刀").SetValue(true));
            #endregion

            //Add Drawing SubMenu
            #region
            Menu.AddSubMenu(new Menu("范围", "Drawings"));
            Menu.SubMenu("Drawings").AddItem(new MenuItem("DrawQ", "Q 范围").SetValue(new Circle(true, Color.FromArgb(255, 0, 255, 0))));
            Menu.SubMenu("Drawings").AddItem(new MenuItem("DrawE", "E 范围").SetValue(new Circle(true, Color.FromArgb(255, 0, 255, 0))));
            Menu.SubMenu("Drawings").AddItem(new MenuItem("DrawR", "R 范围").SetValue(new Circle(true, Color.FromArgb(255, 0, 255, 0))));
            #endregion

            //Add Misc SubMenu
            #region
            Menu.AddSubMenu(new Menu("杂项", "Misc"));
            Menu.SubMenu("Misc").AddItem(new MenuItem("UsePackets", "使用封包").SetValue(true));
            Menu.SubMenu("Misc").AddItem(new MenuItem("SmartW", "智能 W").SetValue(true));
            var ChangeSkin = Menu.SubMenu("Misc").AddItem(new MenuItem("Skin", "皮肤更换").SetValue(true));
            ChangeSkin.ValueChanged += delegate(object sender, OnValueChangeEventArgs EventArgs)
            {
                if (numSkins[ObjectManager.Player.ChampionName] > currSkinId)
                    currSkinId++;
                else
                    currSkinId = 0;

                GenerateSkinPacket(ObjectManager.Player.ChampionName, currSkinId);
            };

            Menu.SubMenu("Misc").AddSubMenu(new Menu("智能 水银饰带", "SQS"));
            Menu.SubMenu("Misc").SubMenu("SQS").AddItem(new MenuItem("ActiveQSS", "启用").SetValue(true));
            Menu.SubMenu("Misc").SubMenu("SQS").AddItem(new MenuItem("Quick%Poison", "使用水银|最低血量").SetValue(new Slider(10, 1, 100)));
            #endregion
            #region MyRegion
            #endregion
            

            //Make visable
            Menu.AddToMainMenu();

            //Events
            Game.OnGameUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;


        }
        //Drawings
        private static void Drawing_OnDraw(EventArgs args)
        {
            var menuItemQ = Menu.Item("Draw" + Q.Slot).GetValue<Circle>();
            if (menuItemQ.Active && Q.IsReady())
                Utility.DrawCircle(Player.Position, Q.Range, menuItemQ.Color);
            else if (menuItemQ.Active)
                Utility.DrawCircle(Player.Position, Q.Range, Color.DarkRed);

            var menuItemE = Menu.Item("Draw" + E.Slot).GetValue<Circle>();
            if (menuItemE.Active && E.IsReady())
                Utility.DrawCircle(Player.Position, E.Range, menuItemE.Color);
            else if (menuItemE.Active)
                Utility.DrawCircle(Player.Position, E.Range, Color.DarkRed);

            var menuItemR = Menu.Item("Draw" + R.Slot).GetValue<Circle>();
            if (menuItemR.Active && R.IsReady())
                Utility.DrawCircle(Player.Position, R.Range, menuItemR.Color);
            else if (menuItemR.Active)
                Utility.DrawCircle(Player.Position, R.Range, Color.DarkRed);

        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (Player.IsDead) return;

            Orbwalker.SetMovement(true);
            Orbwalker.SetAttack(true);

            if (Menu.Item("ComboActive").GetValue<KeyBind>().Active)
                Combo();

            if (Menu.Item("LaneClearActive").GetValue<KeyBind>().Active)
                LaneClear();
            if (Menu.Item("JungleFarmActive").GetValue<KeyBind>().Active)
                JungleFarm();

            if (Menu.Item("ActiveQSS").GetValue<bool>() && QuickS.IsReady() || Scimitar.IsReady())
                SmartQuickS();
            if (ObjectManager.Player.HasBuffOfType(BuffType.Slow) && Menu.Item("SmartW").GetValue<bool>() && W.IsReady())
                W.Cast();


        }


        private static void Combo()
        {
            var target = SimpleTs.GetTarget(Q.Range, SimpleTs.DamageType.Magical);

            if (target != null)
            {
               
                if (ObjectManager.Player.Distance(target) <= DFG.Range && DFG.IsReady() && Menu.Item("UseDFGItems").GetValue<bool>() && target.Health < GetComboDamage(target) && target.IsValidTarget(DFG.Range))
                    DFG.Cast(target);
                if (ObjectManager.Player.Distance(target) <= Cutlass.Range && Cutlass.IsReady() && Menu.Item("UseCutlassItems").GetValue<bool>() && target.IsValidTarget(Cutlass.Range))
                    Cutlass.Cast(target);
                if (ObjectManager.Player.Distance(target) <= BotRK.Range && BotRK.IsReady() && Menu.Item("UseBotRKItems").GetValue<bool>() && target.IsValidTarget(BotRK.Range)) 
                    BotRK.Cast(target);
                if (ObjectManager.Player.Distance(target) <= HexGunBlade.Range && HexGunBlade.IsReady() && Menu.Item("UseHexGunBladeItems").GetValue<bool>() && target.IsValidTarget(HexGunBlade.Range))
                    HexGunBlade.Cast(target);


                if (ObjectManager.Player.Distance(target) < Q.Range && Q.IsReady() && Menu.Item("UseQCombo").GetValue<bool>() && target.IsValidTarget(Q.Range))
                    Q.Cast(Menu.Item("UsePackets").GetValue<bool>()); 
                if (ObjectManager.Player.Distance(target) < E.Range && E.IsReady() && Menu.Item("UseECombo").GetValue<bool>() && target.IsValidTarget(E.Range))
                    E.CastOnUnit(target, Menu.Item("UsePackets").GetValue<bool>());
                if (ObjectManager.Player.Distance(target) < R.Range && R.IsReady() && target.Health < GetComboDamage(target) && Menu.Item("UseRCombo").GetValue<bool>() && target.IsValidTarget(R.Range))
                    R.Cast(target, Menu.Item("UsePackets").GetValue<bool>(), true);
                if (IgniteSlot != SpellSlot.Unknown && Player.SummonerSpellbook.CanUseSpell(IgniteSlot) == SpellState.Ready && Menu.Item("UseIgniteCombo").GetValue<bool>())
                    if (target.Health < GetComboDamage(target))
                      Player.SummonerSpellbook.CastSpell(IgniteSlot, target);
                


            }

        }
        private static void LaneClear()
        {

            var minions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q.Range,
                MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.MaxHealth);

            foreach (var minion in minions.Where(minion => minion.IsValidTarget(Q.Range)))
            {
                if (Menu.Item("UseQLaneClear").GetValue<bool>() && Q.IsReady())
                {
                    if (Menu.Item("manaLActive").GetValue<bool>() && Player.Mana >= GetPlayerMana(Menu.Item("onQL").GetValue<Slider>().Value))
                        Q.Cast();
                    else if (!Menu.Item("manaLActive").GetValue<bool>())
                        Q.Cast();
                }
                if (Menu.Item("UseELaneClear").GetValue<bool>() && E.IsReady())
                {
                    if (Menu.Item("manaLActive").GetValue<bool>() && Player.Mana >= GetPlayerMana(Menu.Item("onEL").GetValue<Slider>().Value))
                        E.CastOnUnit(minion);
                    else if (!Menu.Item("manaLActive").GetValue<bool>())
                        E.CastOnUnit(minion);
                }

            }


        }

        private static void JungleFarm()
        {
            var minions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);
            Obj_AI_Base mob = GetNearest(ObjectManager.Player.ServerPosition);
            double smitedamage = smiteDamage();

            if (minions.Count > 0)
            {
                if (Menu.Item("UseQJungleFarm").GetValue<bool>() && Q.IsReady())
                {
                    if (Menu.Item("manaJActive").GetValue<bool>() && Player.Mana >= GetPlayerMana(Menu.Item("onQJ").GetValue<Slider>().Value))
                        Q.Cast();
                    else if (!Menu.Item("manaJActive").GetValue<bool>())
                        Q.Cast();
                }
                if (Menu.Item("UseEJungleFarm").GetValue<bool>() && E.IsReady())
                {
                    if (Menu.Item("manaJActive").GetValue<bool>() && Player.Mana >= GetPlayerMana(Menu.Item("onEJ").GetValue<Slider>().Value))
                        E.CastOnUnit(minions[0]);
                    else if (!Menu.Item("manaJActive").GetValue<bool>())
                        E.CastOnUnit(minions[0]);
                }
                if (Menu.Item("UseSmite").GetValue<bool>() && SmiteSlot != SpellSlot.Unknown && Player.SummonerSpellbook.CanUseSpell(SmiteSlot) == SpellState.Ready)
                    if(mob.Health < smitedamage)
                        Player.SummonerSpellbook.CastSpell(SmiteSlot, mob);

            }
        }




        private static float GetPlayerHP(float HP)
        {
            return (float) (Player.MaxHealth * (HP / 100));
        }
        private static float GetPlayerMana(float Mana)
        {
            return (float)(Player.MaxMana * (Mana / 100));
        }

        private static void SmartQuickS()
        {

            if (ObjectManager.Player.HasBuffOfType(BuffType.Slow) && W.IsReady()) return;
            if (ObjectManager.Player.HasBuffOfType(BuffType.Slow) || ObjectManager.Player.HasBuffOfType(BuffType.Blind) || ObjectManager.Player.HasBuffOfType(BuffType.Fear) || ObjectManager.Player.HasBuffOfType(BuffType.Stun) ||
                ObjectManager.Player.HasBuffOfType(BuffType.Charm) || ObjectManager.Player.HasBuffOfType(BuffType.Silence) || ObjectManager.Player.HasBuffOfType(BuffType.Snare) || ObjectManager.Player.HasBuffOfType(BuffType.Taunt)
                || ObjectManager.Player.HasBuffOfType(BuffType.Sleep) || ObjectManager.Player.HasBuffOfType(BuffType.Shred) || ObjectManager.Player.HasBuffOfType(BuffType.Polymorph) || ObjectManager.Player.HasBuffOfType(BuffType.Knockup)
                || ObjectManager.Player.HasBuffOfType(BuffType.Knockback) || ObjectManager.Player.HasBuffOfType(BuffType.Disarm) || ObjectManager.Player.HasBuffOfType(BuffType.Poison))
            {
                if (ObjectManager.Player.HasBuffOfType(BuffType.Poison))
                {

                    if (Player.Health <=  GetPlayerHP(Menu.Item("Quick%Poison").GetValue<Slider>().Value))
                        QuickS.Cast();
                        if (Scimitar.IsReady())
                            Scimitar.Cast();

                }
                else if (!(ObjectManager.Player.HasBuffOfType(BuffType.Poison)))
                    QuickS.Cast();
                    if(Scimitar.IsReady())
                        Scimitar.Cast();


            }



        }

        private static float GetComboDamage(Obj_AI_Base enemy)
        {
            double damage = 0d;

            if (DFG.IsReady())
                damage += Player.GetItemDamage(enemy, Damage.DamageItems.Dfg) / 1.2;
            if (Cutlass.IsReady())
                damage += Player.GetItemDamage(enemy, Damage.DamageItems.Bilgewater);
            if (BotRK.IsReady())
                damage += Player.GetItemDamage(enemy, Damage.DamageItems.Botrk);
            if (HexGunBlade.IsReady())
                damage += Player.GetItemDamage(enemy, Damage.DamageItems.Hexgun);
            if (Q.IsReady())
                damage += (Player.GetSpellDamage(enemy, SpellSlot.Q) * 5);
            if (E.IsReady())
                damage += Player.GetSpellDamage(enemy, SpellSlot.E);
            if (R.IsReady())
                damage += Player.GetSpellDamage(enemy, SpellSlot.R);
            if(DFG.IsReady())
                damage = damage * 1.2;
            if (IgniteSlot != SpellSlot.Unknown && Player.SummonerSpellbook.CanUseSpell(IgniteSlot) == SpellState.Ready)
                damage += ObjectManager.Player.GetSummonerSpellDamage(enemy, Damage.SummonerSpell.Ignite);

           

            return (float) damage; 
           

        }
        public static void GenerateSkinPacket(string currentChampion, int skinNumber)
        {
            int netID = ObjectManager.Player.NetworkId;
            GamePacket model = Packet.S2C.UpdateModel.Encoded(new Packet.S2C.UpdateModel.Struct(ObjectManager.Player.NetworkId, skinNumber, currentChampion));
            model.Process(PacketChannel.S2C);
        }


        private static readonly string[] MinionNames = { 
              "TT_Spiderboss", "TTNGolem", "TTNWolf", "TTNWraith", 
              "SRU_Blue", "SRU_Gromp", "SRU_Murkwolf", "SRU_Razorbeak", 
              "SRU_Red", "SRU_Krug", "SRU_Dragon", "SRU_Baron", "Sru_Crab" };

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

        public static double smiteDamage()
        {
            int level = ObjectManager.Player.Level;
            int[] damage =
            {
                20*level + 370,
                30*level + 330,
                40*level + 240,
                50*level + 100
            };
            return damage.Max();
        }
    }
}


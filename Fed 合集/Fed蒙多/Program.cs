#region

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

#endregion

namespace FedDrMundo
{
    internal class Program
    {
        public const string ChampionName = "DrMundo";

        public static Orbwalking.Orbwalker Orbwalker;

        public static List<Spell> SpellList = new List<Spell>();

        public static Spell Q;
        public static Spell W;
        public static Spell E;
        public static Spell R;

        private static SpellSlot IgniteSlot;
        private static SpellSlot SmiteSlot;
        
        public static bool WActive = false;

        public static Menu Config;        
        public static Menu TargetedItems;
        public static Menu NoTargetedItems;

        private static Obj_AI_Hero Player;

        private static void Main(string[] args)
        {            
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        private static void Game_OnGameLoad(EventArgs args)
        {
            Player = ObjectManager.Player;

            if (Player.BaseSkinName != ChampionName) return;

            Q = new Spell(SpellSlot.Q, 1000);
            W = new Spell(SpellSlot.W, Player.AttackRange + 25);
            E = new Spell(SpellSlot.E, Player.AttackRange + 25);
            R = new Spell(SpellSlot.R, Player.AttackRange + 25);

            IgniteSlot = Player.GetSpellSlot("SummonerDot");
            SmiteSlot = Player.GetSpellSlot("SummonerSmite");
            
            Q.SetSkillshot(0.50f, 75f, 1500f, true, SkillshotType.SkillshotLine);            

            SpellList.AddRange(new[] { Q, W, E, R });

            Config = new Menu("Fed" + "蒙多", "DrMundo", true);

            var targetSelectorMenu = new Menu("目标 选择", "Target Selector");
            SimpleTs.AddToMenu(targetSelectorMenu);
            Config.AddSubMenu(targetSelectorMenu);

            Config.AddSubMenu(new Menu("走砍", "Orbwalking"));
            Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalking"));

            Config.AddSubMenu(new Menu("连招", "Combo"));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseQCombo", "使用 Q").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseWCombo", "使用 W").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseECombo", "使用 E").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseItensCombo", "连招使用物品").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("ComboActive", "连招!").SetValue(new KeyBind(32, KeyBindType.Press)));            

            Config.AddSubMenu(new Menu("骚扰", "Harass"));            
            Config.SubMenu("Harass").AddItem(new MenuItem("LifeHarass", "骚扰最低血量").SetValue(new Slider(30, 100, 0)));
            Config.SubMenu("Harass").AddItem(new MenuItem("harassToggle", "骚扰 (自动)").SetValue<KeyBind>(new KeyBind('T', KeyBindType.Toggle)));
            Config.SubMenu("Harass").AddItem(new MenuItem("HarassActive", "骚扰!").SetValue(new KeyBind("C".ToCharArray()[0], KeyBindType.Press)));            

            Config.AddSubMenu(new Menu("清线", "Farm"));
            Config.SubMenu("Farm").AddItem(new MenuItem("UseQFarm", "使用 Q 清线").SetValue(true));
            Config.SubMenu("Farm").AddItem(new MenuItem("UseWFarm", "使用 W 清线").SetValue(true));
            Config.SubMenu("Farm").AddItem(new MenuItem("UseEFarm", "使用 E 清线").SetValue(true));
            Config.SubMenu("Farm").AddItem(new MenuItem("FreezeActive", "控线!").SetValue(new KeyBind("C".ToCharArray()[0], KeyBindType.Press)));
            Config.SubMenu("Farm").AddItem(new MenuItem("LaneClearActive", "清线!").SetValue(new KeyBind("V".ToCharArray()[0], KeyBindType.Press)));

            Config.AddSubMenu(new Menu("清野", "JungleFarm"));
            Config.SubMenu("JungleFarm").AddItem(new MenuItem("UseQJFarm", "使用Q").SetValue(true));
            Config.SubMenu("JungleFarm").AddItem(new MenuItem("UseWJFarm", "使用 W").SetValue(true));
            Config.SubMenu("JungleFarm").AddItem(new MenuItem("UseEJFarm", "使用 E").SetValue(true));            
            Config.SubMenu("JungleFarm").AddItem(new MenuItem("AutoSmite", "自动 惩戒!").SetValue<KeyBind>(new KeyBind('J', KeyBindType.Toggle)));
            Config.SubMenu("JungleFarm").AddItem(new MenuItem("JungleFarmActive", "清野!").SetValue(new KeyBind("V".ToCharArray()[0], KeyBindType.Press)));

            Config.AddSubMenu(new Menu("杂项", "Misc"));
            Config.SubMenu("Misc").AddItem(new MenuItem("laugh", "放声 大笑（嘲讽对手）").SetValue(true));
            Config.SubMenu("Misc").AddItem(new MenuItem("KS", "使用 Q 抢人头").SetValue(false));
            Config.SubMenu("Misc").AddItem(new MenuItem("AutoI", "自动 点燃").SetValue(true));
            Config.SubMenu("Misc").AddItem(new MenuItem("RangeQ", "使用 Q 范围").SetValue(new Slider(980, 1000, 0)));
            Config.SubMenu("Misc").AddItem(new MenuItem("lifesave", "使用R保命").SetValue(true));
            Config.SubMenu("Misc").AddItem(new MenuItem("percenthp", "使用R最低血量").SetValue(new Slider(30, 100, 0)));

            var dmgAfterComboItem = new MenuItem("DamageAfterCombo", "显示组合连招伤害").SetValue(true);
            Utility.HpBarDamageIndicator.DamageToUnit += hero => (float)(ObjectManager.Player.GetSpellDamage(hero, SpellSlot.Q));
            Utility.HpBarDamageIndicator.Enabled = dmgAfterComboItem.GetValue<bool>();
            dmgAfterComboItem.ValueChanged += delegate(object sender, OnValueChangeEventArgs eventArgs)
            {
                Utility.HpBarDamageIndicator.Enabled = eventArgs.GetNewValue<bool>();
            };
            
            Config.AddSubMenu(new Menu("范围", "Drawings"));
            Config.SubMenu("Drawings").AddItem(new MenuItem("QRange", "Q 范围").SetValue(new Circle(true, Color.FromArgb(255, 255, 255, 255))));
            Config.SubMenu("Drawings").AddItem(dmgAfterComboItem);

            Config.AddSubMenu(new Menu("物品", "Itens"));
            Menu menuUseItems = new Menu("使用物品", "menuUseItems");
            Config.SubMenu("Itens").AddSubMenu(menuUseItems);

            TargetedItems = new Menu("目标指向物品", "menuTargetItems");
            menuUseItems.AddSubMenu(TargetedItems);
            TargetedItems.AddItem(new MenuItem("item3153", "破败王者之刃").SetValue(true));
            TargetedItems.AddItem(new MenuItem("item3143", "兰顿之兆").SetValue(true));
            TargetedItems.AddItem(new MenuItem("item3144", "比尔吉沃特弯刀").SetValue(true));
            TargetedItems.AddItem(new MenuItem("item3146", "海克斯科技枪刃").SetValue(true));
            TargetedItems.AddItem(new MenuItem("item3184", "冰霜战锤 ").SetValue(true));

            NoTargetedItems = new Menu("AOE 物品", "menuNonTargetedItems");
            menuUseItems.AddSubMenu(NoTargetedItems);
            NoTargetedItems.AddItem(new MenuItem("item3180", "奥黛恩的面纱").SetValue(true));
            NoTargetedItems.AddItem(new MenuItem("item3131", "神圣之剑").SetValue(true));
            NoTargetedItems.AddItem(new MenuItem("item3074", "贪婪九头蛇").SetValue(true));
            NoTargetedItems.AddItem(new MenuItem("item3077", "提亚马特").SetValue(true));
            NoTargetedItems.AddItem(new MenuItem("item3142", "幽梦之灵").SetValue(true)); 

            Config.AddToMainMenu();

            Game.OnGameUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;            

            Game.PrintChat("<font color=\"#00BFFF\">Fed" + ChampionName + " -</font> <font color=\"#FFFFFF\">鍔犺浇鎴愬姛!姹夊寲by浜岀嫍!QQ缇361630847!</font>");

        }        

        private static void Drawing_OnDraw(EventArgs args)
        {
            foreach (var spell in SpellList)
            {
                var menuItem = Config.Item(spell.Slot + "Range").GetValue<Circle>();
                if (menuItem.Active)
                {
                    Utility.DrawCircle(Player.Position, spell.Range, menuItem.Color);
                }                
            }
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (Player.IsDead) return;
            
            if (Player.HasBuff("BurningAgony"))
            {
                WActive = true;
            }
            else
            {
                WActive = false;
            }

            if (WActive && W.IsReady())
            {
                int inimigos = Utility.CountEnemysInRange(600);

                if (!Config.Item("LaneClearActive").GetValue<KeyBind>().Active && !Config.Item("JungleFarmActive").GetValue<KeyBind>().Active && inimigos == 0)
                {
                    W.Cast();
                }
            }
               

            if (Config.Item("ComboActive").GetValue<KeyBind>().Active)
            {
                Combo();
            }
            else
            {
                if (Config.Item("HarassActive").GetValue<KeyBind>().Active)
                    Harass();                

                if (Config.Item("harassToggle").GetValue<KeyBind>().Active)
                    ToggleHarass();

                if (Config.Item("FreezeActive").GetValue<KeyBind>().Active)
                {
                    FreezeFarm();
                }
                if (Config.Item("LaneClearActive").GetValue<KeyBind>().Active)
                {
                    LaneClear();
                }

                if (Config.Item("JungleFarmActive").GetValue<KeyBind>().Active)
                    JungleFarm();
            }

            if (Config.Item("lifesave").GetValue<bool>())
                LifeSave();

            if (Config.Item("KS").GetValue<bool>())
                Killsteal();

            if (Config.Item("AutoSmite").GetValue<KeyBind>().Active)            
                AutoSmite();

            if (Config.Item("AutoI").GetValue<bool>())            
                AutoIgnite();             
        }

        private static void AutoIgnite()
        {
            var iTarget = SimpleTs.GetTarget(600, SimpleTs.DamageType.True);
            var Idamage = ObjectManager.Player.GetSummonerSpellDamage(iTarget, Damage.SummonerSpell.Ignite) * 0.90;

            if (IgniteSlot != SpellSlot.Unknown && Player.SummonerSpellbook.CanUseSpell(IgniteSlot) == SpellState.Ready && iTarget.Health < Idamage)
            {
                Player.SummonerSpellbook.CastSpell(IgniteSlot, iTarget);
                if (Config.Item("laugh").GetValue<bool>())
                {
                    Game.Say("/l");
                }
            }
        }
        
        private static void LifeSave()
        {
            int inimigos = Utility.CountEnemysInRange(900);

            if (Player.Health < (Player.MaxHealth * Config.Item("percenthp").GetValue<Slider>().Value * 0.01) && R.IsReady() && inimigos >= 1)
            {
                R.Cast();                
            }
        }

        private static void Killsteal()
        {
            int qRange = Config.Item("RangeQ").GetValue<Slider>().Value;
            var qTarget = SimpleTs.GetTarget(Q.Range + Q.Width, SimpleTs.DamageType.Magical);
            var Qdamage = ObjectManager.Player.GetSpellDamage(qTarget, SpellSlot.Q) * 0.95;

            if (qTarget != null && Config.Item("UseQCombo").GetValue<bool>() && Q.IsReady() && Player.Distance(qTarget) <= qRange && qTarget.Health < Qdamage)
            {
                PredictionOutput qPred = Q.GetPrediction(qTarget); 
                if (qPred.Hitchance >= HitChance.High)
                    Q.Cast(qPred.CastPosition);
            }
        }

        private static InventorySlot GetInventorySlot(int ID)
        {
            return ObjectManager.Player.InventoryItems.FirstOrDefault(item => (item.Id == (ItemId)ID && item.Stacks >= 1) || (item.Id == (ItemId)ID && item.Charges >= 1));
        }

        public static void UseItems(Obj_AI_Hero vTarget)
        {
            if (vTarget != null)
            {
                foreach (MenuItem menuItem in TargetedItems.Items)
                {
                    var useItem = TargetedItems.Item(menuItem.Name).GetValue<bool>();
                    if (useItem)
                    {
                        var itemID = Convert.ToInt16(menuItem.Name.ToString().Substring(4, 4));
                        if (Items.HasItem(itemID) && Items.CanUseItem(itemID) && GetInventorySlot(itemID) != null)
                            Items.UseItem(itemID, vTarget);
                    }
                }

                foreach (MenuItem menuItem in NoTargetedItems.Items)
                {
                    var useItem = NoTargetedItems.Item(menuItem.Name).GetValue<bool>();
                    if (useItem)
                    {
                        var itemID = Convert.ToInt16(menuItem.Name.ToString().Substring(4, 4));
                        if (Items.HasItem(itemID) && Items.CanUseItem(itemID) && GetInventorySlot(itemID) != null)
                            Items.UseItem(itemID);
                    }
                }
            }
        }

        private static void AutoSmite()
        {
            if (Config.Item("AutoSmite").GetValue<KeyBind>().Active)
            {
                float[] SmiteDmg = { 20 * Player.Level + 370, 30 * Player.Level + 330, 40 * Player.Level + 240, 50 * Player.Level + 100 };
                string[] MonsterNames = { "LizardElder", "AncientGolem", "Worm", "Dragon" };                
                var vMinions = MinionManager.GetMinions(Player.ServerPosition, Player.SummonerSpellbook.Spells.FirstOrDefault(
                    spell => spell.Name.Contains("smite")).SData.CastRange[0], MinionTypes.All, MinionTeam.NotAlly, MinionOrderTypes.Health);
                foreach (var vMinion in vMinions)
                {
                    if (vMinion != null
                        && !vMinion.IsDead
                        && !Player.IsDead
                        && !Player.IsStunned
                        && SmiteSlot != SpellSlot.Unknown
                        && Player.SummonerSpellbook.CanUseSpell(SmiteSlot) == SpellState.Ready)
                    {
                        if ((vMinion.Health < SmiteDmg.Max()) && (MonsterNames.Any(name => vMinion.BaseSkinName.StartsWith(name))))
                        { 
                            Player.SummonerSpellbook.CastSpell(SmiteSlot, vMinion);

                            if (Config.Item("laugh").GetValue<bool>())
                            {
                                Game.Say("/l");
                            }

                        }
                    }
                }
            }
        }
        
        private static void Combo()
        {
            int qRange = Config.Item("RangeQ").GetValue<Slider>().Value;

            var qTarget = SimpleTs.GetTarget(Q.Range + Q.Width, SimpleTs.DamageType.Magical);

            if (qTarget != null && Config.Item("UseItensCombo").GetValue<bool>())
            {
                UseItems(qTarget);
            }
            if (qTarget != null && Config.Item("UseQCombo").GetValue<bool>() && Q.IsReady() && Player.Distance(qTarget) <= qRange)
            {
                    PredictionOutput qPred = Q.GetPrediction(qTarget);
                    if (qPred.Hitchance >= HitChance.High)
                        Q.Cast(qPred.CastPosition);
            }
            if (!WActive && qTarget != null && Config.Item("UseWCombo").GetValue<bool>() && W.IsReady() && Player.Distance(qTarget) <= 300)
            {
                W.Cast();
            }
            if (qTarget != null && Config.Item("UseECombo").GetValue<bool>() && E.IsReady() && Player.Distance(qTarget) <= qRange)
            {                
                E.Cast();                
            }            
        }        

        private static void Harass()
        {
            var qTarget = SimpleTs.GetTarget(Q.Range + Q.Width, SimpleTs.DamageType.Magical);

            int qRange = Config.Item("RangeQ").GetValue<Slider>().Value;

            var RLife = Config.Item("LifeHarass").GetValue<Slider>().Value;
            var LPercentR = Player.Health * 100 / Player.MaxHealth;

            if (qTarget != null && Q.IsReady() && LPercentR >= RLife && Player.Distance(qTarget) <= qRange)
            {
                PredictionOutput qPred = Q.GetPrediction(qTarget);
                if (qPred.Hitchance >= HitChance.High)
                    Q.Cast(qPred.CastPosition);
            }            
        }

        private static void ToggleHarass()
        {
            var qTarget = SimpleTs.GetTarget(Q.Range + Q.Width, SimpleTs.DamageType.Magical);

            int qRange = Config.Item("RangeQ").GetValue<Slider>().Value;

            var RLife = Config.Item("LifeHarass").GetValue<Slider>().Value;
            var LPercentR = Player.Health * 100 / Player.MaxHealth;

            if (qTarget != null && Q.IsReady() && LPercentR >= RLife && Player.Distance(qTarget) <= qRange)
            {
                PredictionOutput qPred = Q.GetPrediction(qTarget);
                if (qPred.Hitchance >= HitChance.High)
                    Q.Cast(qPred.CastPosition);
            }            
        }

        private static void FreezeFarm()
        {
            var allMinionsQ = MinionManager.GetMinions(Player.ServerPosition, Q.Range + Q.Width + 30, MinionTypes.All, MinionTeam.NotAlly, MinionOrderTypes.Health);            

            foreach (var vMinion in allMinionsQ)
            {
                var Qdamage = ObjectManager.Player.GetSpellDamage(vMinion, SpellSlot.Q) * 0.85;

                if (Config.Item("UseQFarm").GetValue<bool>() && Q.IsReady() && Qdamage >= Q.GetHealthPrediction(vMinion))
                {
                    Q.Cast(vMinion.Position);
                }
            }
        }
        
        private static void LaneClear()
        {
            var allMinionsQ = MinionManager.GetMinions(Player.ServerPosition, Q.Range + Q.Width + 30, MinionTypes.All, MinionTeam.NotAlly, MinionOrderTypes.Health);
            var allMinionsW = MinionManager.GetMinions(Player.ServerPosition, 350, MinionTypes.All, MinionTeam.NotAlly, MinionOrderTypes.Health);

            foreach (var vMinion in allMinionsQ)
            {
                var Qdamage = ObjectManager.Player.GetSpellDamage(vMinion, SpellSlot.Q) * 0.85;

                if (Config.Item("UseQFarm").GetValue<bool>() && Q.IsReady() && Qdamage >= Q.GetHealthPrediction(vMinion))
                {
                    Q.Cast(vMinion.Position);
                }

                if (Config.Item("UseWFarm").GetValue<bool>() && W.IsReady() && !WActive && allMinionsW.Count > 2)
                {
                    W.Cast();
                }

                if (Config.Item("UseEFarm").GetValue<bool>() && E.IsReady() && allMinionsW.Count > 2)
                {
                    E.Cast();
                }
            }
        }

        private static void JungleFarm()
        {
            var mobs = MinionManager.GetMinions(Player.ServerPosition, Q.Range,
                MinionTypes.All,
                MinionTeam.Neutral, MinionOrderTypes.MaxHealth);
            if (mobs.Count > 0)
            {               
                if (Q.IsReady())
                {
                    Q.Cast(mobs[0].Position);
                }
                if (!WActive && W.IsReady())
                {
                    W.Cast();
                }
                if (E.IsReady())
                {
                    E.Cast();
                }
            }
        }

    }
}

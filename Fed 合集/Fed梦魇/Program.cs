#region
using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
#endregion

namespace FedNocturne
{
    internal class Program
    {
        public const string ChampionName = "Nocturne";
        public static Orbwalking.Orbwalker Orbwalker;
        
        public static Spell Q, W, E, R;
        public static Vector2 PingLocation;
        public static int LastPingT = 0;

        private static SpellSlot IgniteSlot;
        private static SpellSlot SmiteSlot;

        public static Menu Config;
        public static Menu TargetedItems;
        public static Menu NoTargetedItems;
        private static readonly Obj_AI_Hero Player = ObjectManager.Player;
        
        private static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        private static void Game_OnGameLoad(EventArgs args)
        {
            if (Player.BaseSkinName != ChampionName) return;

            Q = new Spell(SpellSlot.Q, 1200f);
            W = new Spell(SpellSlot.W, 0f);
            E = new Spell(SpellSlot.E, 425f);
            R = new Spell(SpellSlot.R, 25000f);

            Q.SetSkillshot(0.25f, 60f, 1600f, false, SkillshotType.SkillshotLine);       
            E.SetTargetted(0.5f, 1700f);
            R.SetTargetted(0.75f, 2500f);

            IgniteSlot = Player.GetSpellSlot("SummonerDot");
            SmiteSlot = Player.GetSpellSlot("SummonerSmite");            

            Config = new Menu("Fed" + "梦魇", "Nocturne", true);

            var targetSelectorMenu = new Menu("目标 选择", "Target Selector");
            SimpleTs.AddToMenu(targetSelectorMenu);
            Config.AddSubMenu(targetSelectorMenu);

            Config.AddSubMenu(new Menu("走砍", "Orbwalking"));
            Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalking"));

            Config.AddSubMenu(new Menu("团战", "TeamFight"));
            Config.SubMenu("TeamFight").AddItem(new MenuItem("UseQCombo", "使用 Q").SetValue(true));
            Config.SubMenu("TeamFight").AddItem(new MenuItem("UseWCombo", "使用 W").SetValue(true));
            Config.SubMenu("TeamFight").AddItem(new MenuItem("UseECombo", "使用 E").SetValue(true));
            Config.SubMenu("TeamFight").AddItem(new MenuItem("UseRCombo", "使用 R").SetValue(true));
            Config.SubMenu("TeamFight").AddItem(new MenuItem("UseRHP", "连招使用R血量: ").SetValue<Slider>(new Slider(50, 100, 10)));
            Config.SubMenu("TeamFight").AddItem(new MenuItem("ComboActive", "连招!").SetValue(new KeyBind(32, KeyBindType.Press)));

            Config.AddSubMenu(new Menu("骚扰", "Harass"));
            Config.SubMenu("Harass").AddItem(new MenuItem("UseQHarass", "使用 Q").SetValue(true));
            Config.SubMenu("Harass").AddItem(new MenuItem("UseEHarass", "使用 E").SetValue(false));            
            Config.SubMenu("Harass").AddItem(new MenuItem("HarassActive", "Harass!").SetValue(new KeyBind("C".ToCharArray()[0], KeyBindType.Press)));

            Config.AddSubMenu(new Menu("清线", "LaneClear"));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("UseQFarm", "使用 Q").SetValue(true));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("useQHit", "使用Q|小兵数量").SetValue(new Slider(3, 6, 1)));            
            Config.SubMenu("LaneClear").AddItem(new MenuItem("LaneClearActive", "清线!").SetValue(new KeyBind("V".ToCharArray()[0], KeyBindType.Press)));

            Config.AddSubMenu(new Menu("清野", "JungleFarm"));
            Config.SubMenu("JungleFarm").AddItem(new MenuItem("UseQJFarm", "使用 Q").SetValue(true));
            Config.SubMenu("JungleFarm").AddItem(new MenuItem("UseWJFarm", "使用 W").SetValue(true));
            Config.SubMenu("JungleFarm").AddItem(new MenuItem("UseEJFarm", "使用 E").SetValue(true));
            Config.SubMenu("JungleFarm").AddItem(new MenuItem("AutoSmite", "自动 惩戒!").SetValue<KeyBind>(new KeyBind('J', KeyBindType.Toggle)));
            Config.SubMenu("JungleFarm").AddItem(new MenuItem("JungleFarmActive", "清野!").SetValue(new KeyBind("V".ToCharArray()[0], KeyBindType.Press)));

            Config.AddSubMenu(new Menu("杂项", "Misc"));
            Config.SubMenu("Misc").AddItem(new MenuItem("AutoI", "自动 点燃").SetValue(true));
            Config.SubMenu("Misc").AddItem(new MenuItem("InterruptSpells", "中断法术").SetValue(true));
            Config.SubMenu("Misc").AddItem(new MenuItem("AutoRHP", "自动R低血量").SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press)));
            Config.SubMenu("Misc").AddItem(new MenuItem("HPR", "自动R|敌方血量: ").SetValue<Slider>(new Slider(30, 100, 10)));
            Config.SubMenu("Misc").AddItem(new MenuItem("useR_Killableping", "残血击杀提示").SetValue(true));
            Config.SubMenu("Misc").AddItem(new MenuItem("AutoRStrong", "R范围能最高AP|AD敌方").SetValue(new KeyBind("G".ToCharArray()[0], KeyBindType.Press)));
            Config.SubMenu("Misc").AddItem(new MenuItem("MostR", "R 模式: ").SetValue(new StringList(new[] { "AD", "AP", "Easy" }, 0)));            

            Config.AddSubMenu(new Menu("物品", "Itens"));
            Menu menuUseItems = new Menu("使用 物品", "menuUseItems");
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

            Config.AddSubMenu(new Menu("范围", "Drawing"));
            Config.SubMenu("Drawing").AddItem(new MenuItem("Draw_Disabled", "禁用 所有").SetValue(false));
            Config.SubMenu("Drawing").AddItem(new MenuItem("Draw_Q", "范围 Q").SetValue(true));
            Config.SubMenu("Drawing").AddItem(new MenuItem("Draw_E", "范围 E").SetValue(true));
            Config.SubMenu("Drawing").AddItem(new MenuItem("Draw_R", "范围 R").SetValue(true));
            Config.SubMenu("Drawing").AddItem(new MenuItem("DrawRRangeM", "显示 R 范围 (小地图)").SetValue(new Circle(false, Color.FromArgb(150, Color.DodgerBlue))));

            Config.AddToMainMenu();

            Game.OnGameUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Drawing.OnEndScene += Drawing_OnEndScene;
            Interrupter.OnPossibleToInterrupt += Interrupter_OnPossibleToInterrupt;

            Game.PrintChat("<font color=\"#00BFFF\">Fed" + ChampionName + " -</font> <font color=\"#FFFFFF\">鍔犺浇鎴愬姛!姹夊寲by浜岀嫍!QQ缇361630847</font>");

        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (ObjectManager.Player.IsDead) return;

            if (Config.Item("ComboActive").GetValue<KeyBind>().Active)
            {
                Combo();
            }
            else
            {
                if (Config.Item("HarassActive").GetValue<KeyBind>().Active)
                {
                    Harass();
                }

                if (Config.Item("LaneClearActive").GetValue<KeyBind>().Active)
                {
                    LaneClear();
                }

                if (Config.Item("JungleFarmActive").GetValue<KeyBind>().Active)
                {
                    JungleFarm();
                }
            }

            if (Config.Item("AutoRHP").GetValue<KeyBind>().Active)
            {
                AutoRLowHP();
            }

            if (Config.Item("AutoRStrong").GetValue<KeyBind>().Active)
            {
                AutoRMode();
            }

            if (Config.Item("AutoSmite").GetValue<KeyBind>().Active)
            {
                AutoSmite();
            }

            if (Config.Item("AutoI").GetValue<bool>())
            {
                AutoIgnite();
            }

            if (R.IsReady() && Config.Item("useR_Killableping").GetValue<bool>())
            {
                foreach (var enemy in ObjectManager.Get<Obj_AI_Hero>().Where(h => h.IsValidTarget(GetRRange()) && EnemyLowHP(Config.Item("HPR").GetValue<Slider>().Value, GetRRange())))
                {
                    Ping(enemy.Position.To2D());
                }
            }
        }

        private static InventorySlot GetInventorySlot(int ID)
        {
            return ObjectManager.Player.InventoryItems.FirstOrDefault(item => (item.Id == (ItemId)ID && item.Stacks >= 1) || (item.Id == (ItemId)ID && item.Charges >= 1));
        }

        public static void UseItems(Obj_AI_Hero vTarget)
        {
            if (vTarget == null) return;

            foreach (var itemID in from menuItem in TargetedItems.Items
                                   let useItem = TargetedItems.Item(menuItem.Name).GetValue<bool>()
                                   where useItem
                                   select Convert.ToInt16(menuItem.Name.Substring(4, 4))
                                       into itemId
                                       where Items.HasItem(itemId) && Items.CanUseItem(itemId) && GetInventorySlot(itemId) != null
                                       select itemId)
            {
                Items.UseItem(itemID, vTarget);
            }

            foreach (var itemID in from menuItem in TargetedItems.Items
                                   let useItem = TargetedItems.Item(menuItem.Name).GetValue<bool>()
                                   where useItem
                                   select Convert.ToInt16(menuItem.Name.Substring(4, 4))
                                       into itemId
                                       where Items.HasItem(itemId) && Items.CanUseItem(itemId) && GetInventorySlot(itemId) != null
                                       select itemId)
            {
                Items.UseItem(itemID);
            }
        }


        private static void AutoIgnite()
        {
            var iTarget = SimpleTs.GetTarget(600, SimpleTs.DamageType.True);
            var Idamage = ObjectManager.Player.GetSummonerSpellDamage(iTarget, Damage.SummonerSpell.Ignite) * 0.90;

            if (IgniteSlot != SpellSlot.Unknown && Player.SummonerSpellbook.CanUseSpell(IgniteSlot) == SpellState.Ready && iTarget.Health < Idamage)
            {
                Player.SummonerSpellbook.CastSpell(IgniteSlot, iTarget);                
            }
        }
        private static void AutoSmite()
        {
            if (SmiteSlot == SpellSlot.Unknown)
                return;

            if (!Config.Item("AutoSmite").GetValue<KeyBind>().Active) return;

            string[] monsterNames = { "LizardElder", "AncientGolem", "Worm", "Dragon" };
            var firstOrDefault = Player.SummonerSpellbook.Spells.FirstOrDefault(
                spell => spell.Name.Contains("mite"));
            if (firstOrDefault == null) return;

            var vMonsters = MinionManager.GetMinions(Player.ServerPosition, firstOrDefault.SData.CastRange[0], MinionTypes.All, MinionTeam.NotAlly, MinionOrderTypes.Health);
            foreach (var vMonster in vMonsters.Where(vMonster => vMonster != null
                                                              && !vMonster.IsDead
                                                              && !Player.IsDead
                                                              && !Player.IsStunned
                                                              && SmiteSlot != SpellSlot.Unknown
                                                              && Player.SummonerSpellbook.CanUseSpell(SmiteSlot) == SpellState.Ready)
                                                              .Where(vMonster => (vMonster.Health < Player.GetSummonerSpellDamage(vMonster, Damage.SummonerSpell.Smite)) && (monsterNames.Any(name => vMonster.BaseSkinName.StartsWith(name)))))
            {
                Player.SummonerSpellbook.CastSpell(SmiteSlot, vMonster);
            }
        }
        private static float GetRRange()
        {
            return 1250 + (750 * R.Level);
        }
        private static void AutoRMode()
        {
            Obj_AI_Hero newtarget = null;
            var RMode = Config.Item("MostR").GetValue<StringList>().SelectedIndex;

            foreach (var enemy in ObjectManager.Get<Obj_AI_Hero>().Where(enemy => enemy.IsValidTarget() && Geometry.Distance(enemy) <= GetRRange()))
            {
                if (newtarget == null)
                {
                    newtarget = enemy;
                }
                else
                {
                    switch (RMode)
                    {
                        case 0:
                            if (enemy.BaseAttackDamage + enemy.FlatPhysicalDamageMod <
                                        newtarget.BaseAttackDamage + newtarget.FlatPhysicalDamageMod)
                            {
                                newtarget = enemy;
                            }
                            break;
                        case 1:
                            if (enemy.FlatMagicDamageMod < newtarget.FlatMagicDamageMod)
                            {
                                newtarget = enemy;
                            }
                            break;
                        case 2:
                            if ((enemy.Health - Damage.CalcDamage(ObjectManager.Player, enemy, Damage.DamageType.Magical, enemy.Health)) <
                                        (enemy.Health - Damage.CalcDamage(ObjectManager.Player, newtarget, Damage.DamageType.Magical, newtarget.Health)))
                            {
                                newtarget = enemy;
                            }
                            break;
                    }
                }
            }

            if (R.IsReady() && newtarget != null)
            {
                R.Cast();
                R.CastOnUnit(newtarget, true);
            }
        }
        private static void AutoRLowHP()
        {
            var rTarget = SimpleTs.GetTarget(GetRRange(), SimpleTs.DamageType.Physical);
            if (R.IsReady() && rTarget != null)
            {
                if (ObjectManager.Player.Distance(rTarget) > 1100 && EnemyLowHP(Config.Item("HPR").GetValue<Slider>().Value, GetRRange()))
                {
                    R.Cast();
                    R.CastOnUnit(rTarget, true);
                }
            }
        }
        private static void Combo()
        {
            var qTarget = SimpleTs.GetTarget(Q.Range - 50, SimpleTs.DamageType.Physical);
            var eTarget = SimpleTs.GetTarget(E.Range - 30, SimpleTs.DamageType.Physical); 
            var rTarget = SimpleTs.GetTarget(GetRRange(), SimpleTs.DamageType.Physical);
            
            if (R.IsReady() && Config.Item("UseRCombo").GetValue<bool>())
            {
                if (ObjectManager.Player.Distance(rTarget) > 1100 && EnemyLowHP(Config.Item("UseRHP").GetValue<Slider>().Value, GetRRange()))
                {
                    R.Cast();
                    R.CastOnUnit(rTarget, true);
                }
            }

            var vTarget = SimpleTs.GetTarget(Orbwalking.GetRealAutoAttackRange(ObjectManager.Player), SimpleTs.DamageType.Physical);

            if (vTarget != null )
            {
                UseItems(vTarget);
            }

            if (Q.IsReady() && Config.Item("UseQCombo").GetValue<bool>())
            {
                if (Q.GetPrediction(qTarget).Hitchance >= HitChance.High)
                    Q.Cast(qTarget.Position, true);
            }

            if (E.IsReady() && Config.Item("UseECombo").GetValue<bool>())
            {
                E.CastOnUnit(eTarget, true);
            }

            if (W.IsReady() && Config.Item("UseWCombo").GetValue<bool>())
            {
                if (ObjectManager.Player.Distance(eTarget) < 425)
                {
                    W.Cast();
                }
            }
        }
        private static void Harass()
        {
            var qTarget = SimpleTs.GetTarget(Q.Range - 50, SimpleTs.DamageType.Physical);
            var eTarget = SimpleTs.GetTarget(E.Range - 30, SimpleTs.DamageType.Physical);

            if (Q.IsReady() && Config.Item("UseQHarass").GetValue<bool>())
            {
                if (Q.GetPrediction(qTarget).Hitchance >= HitChance.High)
                    Q.Cast(qTarget.Position, true);
            }
            
            if (eTarget != null && E.IsReady() && Config.Item("UseEHarass").GetValue<bool>())
            {
                E.CastOnUnit(qTarget, true);
            }
        }
        private static void LaneClear()
        {
            var minions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.NotAlly);

            if (Q.IsReady() && Config.Item("UseQFarm").GetValue<bool>())
            {
                var minionPs = MinionManager.GetMinionsPredictedPositions(minions, 0.25f, 60f, 1600f, ObjectManager.Player.ServerPosition, 1200f, false, SkillshotType.SkillshotLine);
                var farm = Q.GetLineFarmLocation(minionPs);
                if (farm.MinionsHit >= Config.Item("useQHit").GetValue<Slider>().Value)
                {
                    Q.Cast(farm.Position, true);
                }
            }
        }
        private static void JungleFarm()
        {
            var mobs = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);
            if (mobs.Count > 0)
            {
                var mob = mobs[0];

                if (Q.IsReady() && Config.Item("UseQJFarm").GetValue<bool>())
                {
                    Q.Cast(mob.Position);
                }
                if (W.IsReady() && Config.Item("UseWJFarm").GetValue<bool>())
                {
                    W.Cast();
                }
                if (E.IsReady() && Config.Item("UseEJFarm").GetValue<bool>())
                {
                    E.CastOnUnit(mob);
                }
            }
        }
        private static bool EnemyLowHP(int percentHP, float range)
        {
            return ObjectManager.Get<Obj_AI_Hero>().Where(enemmy => enemmy.IsEnemy && !enemmy.IsDead).Any(enemmy => Vector3.Distance(ObjectManager.Player.Position, enemmy.Position) < range && ((enemmy.Health/enemmy.MaxHealth)*100) < percentHP);
        }

        private static void Ping(Vector2 position)
        {
            if (Environment.TickCount - LastPingT < 30 * 1000) return;
            LastPingT = Environment.TickCount;
            PingLocation = position;
            SimplePing();
            Utility.DelayAction.Add(150, SimplePing);
            Utility.DelayAction.Add(300, SimplePing);
            Utility.DelayAction.Add(400, SimplePing);
            Utility.DelayAction.Add(800, SimplePing);
        }
        private static void SimplePing()
        {
            Packet.S2C.Ping.Encoded(new Packet.S2C.Ping.Struct(PingLocation.X, PingLocation.Y, 0, 0, Packet.PingType.Fallback)).Process();
        }
        private static void Drawing_OnEndScene(EventArgs args)
        {
            if (Config.Item("Draw_Disabled").GetValue<bool>())
                return;

            if (R.Level == 0) return;
            var menuItem = Config.Item("DrawRRangeM").GetValue<Circle>();
            if (menuItem.Active)
                Utility.DrawCircle(ObjectManager.Player.Position, GetRRange(), menuItem.Color, 2, 30, true);
        }
        private static void Drawing_OnDraw(EventArgs args)
        {
            if (Config.Item("Draw_Disabled").GetValue<bool>())
                return;

            if (Config.Item("Draw_Q").GetValue<bool>())
                if (Q.Level > 0)
                    Utility.DrawCircle(ObjectManager.Player.Position, Q.Range, Q.IsReady() ? Color.Green : Color.Red);

            if (Config.Item("Draw_E").GetValue<bool>())
                if (E.Level > 0)
                    Utility.DrawCircle(ObjectManager.Player.Position, E.Range, E.IsReady() ? Color.Green : Color.Red);

            if (Config.Item("Draw_R").GetValue<bool>())
                if (R.Level > 0)
                    Utility.DrawCircle(ObjectManager.Player.Position, GetRRange(), R.IsReady() ? Color.Green : Color.Red);
            
        }
        private static void Interrupter_OnPossibleToInterrupt(Obj_AI_Base unit, InterruptableSpell spell)
        {
            if (!Config.Item("InterruptSpells").GetValue<bool>())
                return;

            if (ObjectManager.Player.Distance(unit) < E.Range && E.IsReady() && unit.IsEnemy)
            {
                if (W.IsReady()) // for protect yourself
                    W.CastOnUnit(ObjectManager.Player);
                E.CastOnUnit(unit, true);
            }
        }
    }
}

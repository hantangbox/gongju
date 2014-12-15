#region
using System;
using System.Collections.Generic;
using Color = System.Drawing.Color;
using System.Linq;
using SharpDX;
using LeagueSharp;
using LeagueSharp.Common;
#endregion

namespace LightningLux
{
	internal class Program
	{
		private static Menu Config;
		private static Obj_AI_Hero Target;
		private static Obj_AI_Hero Player;
		private static Spell Q;
		private static Spell W;
		private static Spell E;
		private static Spell R;
		private static SpellSlot IgniteSlot;
		private static GameObject EObject;
		private static HitChance HitC;
		
		private static void Main(string[] args)
		{
			CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
		}
		
		private static void Game_OnGameLoad(EventArgs args)
		{
			Player = ObjectManager.Player;
			
			if (Player.ChampionName != "Lux") return;
			
			Q = new Spell(SpellSlot.Q, 1175);
			W = new Spell(SpellSlot.W, 1075);
			E = new Spell(SpellSlot.E, 1100);
			R = new Spell(SpellSlot.R, 3340);

			Q.SetSkillshot(0.25f, 80f, 1200f, true, SkillshotType.SkillshotLine);
			W.SetSkillshot(0.25f, 150f, 1200f, false, SkillshotType.SkillshotLine);
			E.SetSkillshot(0.15f, 275f, 1300f, false, SkillshotType.SkillshotCircle);
			R.SetSkillshot(1.35f, 190f, float.MaxValue, false, SkillshotType.SkillshotLine);
			
			IgniteSlot = Player.GetSpellSlot("SummonerDot");
			
			Config = new Menu("Lightning 拉克丝", "Lightning Lux", true);
			var targetSelectorMenu = new Menu("目标选择", "Target Selector");
			SimpleTs.AddToMenu(targetSelectorMenu);
			Config.AddSubMenu(targetSelectorMenu);
			
			Config.AddSubMenu(new Menu("走砍", "Orbwalker"));
			var orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalker"));
			
			Config.AddSubMenu(new Menu("连招", "Combo"));
			Config.SubMenu("Combo").AddItem(new MenuItem("ComboActive", "连招!").SetValue(new KeyBind(32, KeyBindType.Press)));
			Config.SubMenu("Combo").AddItem(new MenuItem("UseQ", "使用 Q").SetValue(true));
			Config.SubMenu("Combo").AddItem(new MenuItem("UseW", "使用 W").SetValue(true));
			Config.SubMenu("Combo").AddItem(new MenuItem("UseE", "使用 E").SetValue(true));
			Config.SubMenu("Combo").AddItem(new MenuItem("UseR", "如果能击杀使用R").SetValue(true));
			Config.SubMenu("Combo").AddItem(new MenuItem("UseItems", "使用 物品").SetValue(true));
			Config.SubMenu("Combo").AddItem(new MenuItem("UseIgnite", "使用 点燃").SetValue(true));
			
			Config.AddSubMenu(new Menu("骚扰", "Harass"));
			Config.SubMenu("Harass").AddItem(new MenuItem("HarassActive", "骚扰!").SetValue(new KeyBind("C".ToCharArray()[0], KeyBindType.Press)));
			Config.SubMenu("Harass").AddItem(new MenuItem("HQ", "使用 Q").SetValue(true));
			Config.SubMenu("Harass").AddItem(new MenuItem("HE", "使用 E").SetValue(true));
			
			Config.AddSubMenu(new Menu("发育", "Farm"));
			Config.SubMenu("Farm").AddItem(new MenuItem("FarmActive", "发育!").SetValue(new KeyBind("V".ToCharArray()[0], KeyBindType.Press)));
			Config.SubMenu("Farm").AddItem(new MenuItem("JungSteal", "偷野怪!").SetValue(new KeyBind("J".ToCharArray()[0], KeyBindType.Press)));
			Config.SubMenu("Farm").AddItem(new MenuItem("FQ", "使用 Q").SetValue(true));
			Config.SubMenu("Farm").AddItem(new MenuItem("FW", "使用 W").SetValue(true));
			Config.SubMenu("Farm").AddItem(new MenuItem("FE", "使用 E").SetValue(true));
			Config.SubMenu("Farm").AddItem(new MenuItem("FMP", "我的法力 %").SetValue(new Slider(15,100,0)));
			
			Config.AddSubMenu(new Menu("抢人头", "KillSteal"));
			Config.SubMenu("KillSteal").AddItem(new MenuItem("KUseQ", "使用 Q").SetValue(true));
			Config.SubMenu("KillSteal").AddItem(new MenuItem("KUseE", "使用 E").SetValue(true));
			Config.SubMenu("KillSteal").AddItem(new MenuItem("KUseR", "使用 R").SetValue(true));
			Config.SubMenu("KillSteal").AddItem(new MenuItem("KIgnite", "使用 点燃").SetValue(true));
			
			Config.AddSubMenu(new Menu("自动护盾", "AutoShield"));
			Config.SubMenu("AutoShield").AddItem(new MenuItem("WAllies", "自动使用W给附近队友").SetValue(true));
			Config.SubMenu("AutoShield").AddItem(new MenuItem("AutoW", "当目标是拉克丝自动使用W").SetValue(true));
			Config.SubMenu("AutoShield").AddItem(new MenuItem("HP", "队友血量 %").SetValue(new Slider(60,100,0)));
			Config.SubMenu("AutoShield").AddItem(new MenuItem("MP", "我的法力 %").SetValue(new Slider(30,100,0)));
			
			Config.AddSubMenu(new Menu("额外选项", "ExtraSettings"));
			Config.SubMenu("ExtraSettings").AddItem(new MenuItem("UseQE", "使用E困住目标").SetValue(false));
			Config.SubMenu("ExtraSettings").AddItem(new MenuItem("AutoE2", "自动消耗 E").SetValue(true));
			Config.SubMenu("ExtraSettings").AddItem(new MenuItem("UseQGap", "突进者使用Q").SetValue(true));
			Config.SubMenu("ExtraSettings").AddItem(new MenuItem("HitChance", "命中率").SetValue(new StringList(new[] {"低","中","高","非常高"},2)));
			Config.SubMenu("ExtraSettings").AddItem(new MenuItem("UsePacket", "使用封包释放").SetValue(true));
			
			Config.AddSubMenu(new Menu("大招设置", "UltSettings"));
			Config.SubMenu("UltSettings").AddItem(new MenuItem("RHit", "如果自动R命中目标数").SetValue(new StringList(new[] {"没有","2 目标","3 目标","4 目标","5 目标"},1)));
			Config.SubMenu("UltSettings").AddItem(new MenuItem("RTrap", "如果目标被困自动R").SetValue(false));
			
			Config.AddSubMenu(new Menu("显示范围", "Drawings"));
			Config.SubMenu("Drawings").AddItem(new MenuItem("QRange", "Q 范围").SetValue(new Circle(true, Color.FromArgb(255, 255, 255, 255))));
			Config.SubMenu("Drawings").AddItem(new MenuItem("WRange", "W 范围").SetValue(new Circle(false, Color.FromArgb(255, 255, 255, 255))));
			Config.SubMenu("Drawings").AddItem(new MenuItem("ERange", "E 范围").SetValue(new Circle(false, Color.FromArgb(255, 255, 255, 255))));
			Config.AddToMainMenu();
			
			Game.PrintChat("Lightning 鎷夊厠涓潀 杞藉叆!  姹夊寲:Feeeez 浜岀嫍  缇ゅ彿:361630847");

			Game.OnGameUpdate += Game_OnGameUpdate;
			Orbwalking.BeforeAttack += Orbwalking_BeforeAttack;
			Drawing.OnDraw += Drawing_OnDraw;
			AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
			Obj_AI_Base.OnProcessSpellCast += Obj_AI_Hero_OnProcessSpellCast;
			GameObject.OnCreate += OnCreateObject;
			GameObject.OnDelete += OnDeleteObject;
		}
		
		private static bool GetBool(string s)
		{
			return Config.Item(s).GetValue<bool>();
		}
		
		private static bool GetActive(string s)
		{
			return Config.Item(s).GetValue<KeyBind>().Active;
		}
		
		private static LeagueSharp.Common.Circle GetCircle(string s)
		{
			return Config.Item(s).GetValue<Circle>();
		}
		
		private static int GetSlider(string s)
		{
			return Config.Item(s).GetValue<Slider>().Value;
		}
		
		private static int GetSelected(string s)
		{
			return Config.Item(s).GetValue<StringList>().SelectedIndex;
		}
		
		private static void Game_OnGameUpdate(EventArgs args)
		{
			KillSteal();
			
			Target = SimpleTs.GetTarget(Q.Range, SimpleTs.DamageType.Magical);
			
			if (GetActive("ComboActive")) UseCombo();
			else if (GetActive("HarassActive")) Harass();
			else if (GetActive("FarmActive")) Farm();
			else if (GetActive("JungSteal")) JungSteal();
			
			if (GetBool("WAllies")) AutoShield();
			if (GetBool("AutoE2")) CastE2();
			if (GetBool("RTrap")) RTrapped();
			
			if (GetSelected("HitChance") == 0) HitC = HitChance.Low;
			else if (GetSelected("HitChance") == 1) HitC = HitChance.Medium;
			else if (GetSelected("HitChance") == 2) HitC = HitChance.High;
			else if (GetSelected("HitChance") == 3) HitC = HitChance.VeryHigh;
			
			if (GetSelected("RHit") == 0) RHit(0);
			else if (GetSelected("RHit") == 1) RHit(2);
			else if (GetSelected("RHit") == 2) RHit(3);
			else if (GetSelected("RHit") == 3) RHit(4);
			else if (GetSelected("RHit") == 4) RHit(5);
		}
		
		private static void OnCreateObject(GameObject sender, EventArgs args)
		{
			if (sender.Name.Contains("LuxLightstrike_tar_green"))
			{
				EObject = sender;
				return;
			}
		}
		
		private static void OnDeleteObject(GameObject sender, EventArgs args)
		{
			if (sender.Name.Contains("LuxLightstrike_tar_green"))
			{
				EObject = null;
				return;
			}
		}
		
		private static Obj_AI_Hero GrabAlly()
		{
			Obj_AI_Hero Ally = null;
			foreach (var hero in from hero in ObjectManager.Get<Obj_AI_Hero>().Where(hero => hero.IsValidTarget(W.Range) && hero.IsAlly ) let heroPercent = hero.Health/hero.MaxHealth*100 let shieldPercent = GetSlider("HP") where heroPercent <= shieldPercent select hero)
			{
				Ally = hero;
				break;
			}
			return Ally;
		}
		
		private static void RHit(int x)
		{
			var rtarget = SimpleTs.GetTarget(R.Range, SimpleTs.DamageType.Magical);
			R.CastIfWillHit(rtarget,x,GetBool("UsePacket"));
		}
		
		private static void RTrapped()
		{
			foreach (var hero in ObjectManager.Get<Obj_AI_Hero>().Where(hero => hero.IsValidTarget(R.Range) && !hero.IsDead && hero.HasBuff("LuxLightBindingMis")))
			{
				R.Cast(hero,GetBool("UsePacket"));
				return;
			}
		}
		
		private static bool IsOnTheLine(Vector3 point, Vector3 start, Vector3 end)
		{
			var obj = Geometry.ProjectOn(point.To2D(),start.To2D(),end.To2D());
			if (obj.IsOnSegment) return true;
			return false;
		}
		
		private static void Obj_AI_Hero_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
		{
			if (sender.IsEnemy && sender.Type == GameObjectType.obj_AI_Minion)
			{
				if (GetBool("FW") && W.IsReady() && GetActive("FarmActive") && args.Target.Name == Player.Name && Player.Mana/Player.MaxMana*100 >= GetSlider("MP") )
				{
					if (GrabAlly() == null) W.Cast(sender,GetBool("UsePacket"));
					else W.CastIfHitchanceEquals(GrabAlly(), HitC ,GetBool("UsePacket"));
				}
				
			}
			if (GetBool("AutoW") && W.IsReady() && sender.IsEnemy && (sender.Type == GameObjectType.obj_AI_Hero || sender.Type == GameObjectType.obj_AI_Turret ))
			{
				if ( (args.SData.Name != null && IsOnTheLine(Player.Position,args.Start,args.End)) || (args.Target == Player && Player.Distance(sender) <= 450) || args.Target == Player && Utility.UnderTurret(Player,true))
				{
					if (GrabAlly() == null) W.Cast(sender,GetBool("UsePacket"));
					else W.CastIfHitchanceEquals(GrabAlly(), HitC ,GetBool("UsePacket"));
				}
			}

		}
		
		private static void Drawing_OnDraw(EventArgs args)
		{
			if (GetCircle("QRange").Active && !Player.IsDead)
			{
				Utility.DrawCircle(Player.Position, Q.Range, GetCircle("QRange").Color);
			}
			if (GetCircle("WRange").Active && !Player.IsDead)
			{
				Utility.DrawCircle(Player.Position, W.Range, GetCircle("WRange").Color);
			}
			if (GetCircle("ERange").Active && !Player.IsDead)
			{
				Utility.DrawCircle(Player.Position, E.Range, GetCircle("ERange").Color);
			}
			if (GetActive("JungSteal") && !Player.IsDead)
			{
				Utility.DrawCircle(Game.CursorPos, 900, Color.White);
			}
		}
		
		private static void Orbwalking_BeforeAttack(Orbwalking.BeforeAttackEventArgs args)
		{
			if (GetActive("ComboActive"))
				args.Process = (!Q.IsReady() || !E.IsReady() || Player.Distance(args.Target) >= 550);
		}
		
		private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
		{
			if (Player.HasBuff("Recall") || Player.IsWindingUp) return;
			if (GetBool("UseQGap") && Q.IsReady() && GetDistanceSqr(Player,gapcloser.Sender) <= Q.Range * Q.Range) Q.CastIfHitchanceEquals(gapcloser.Sender, HitC ,GetBool("UsePacket"));
		}
		
		private static void AutoShield()
		{
			if (Player.Mana/Player.MaxMana*100 >= GetSlider("MP")) W.CastIfHitchanceEquals(GrabAlly(), HitC ,GetBool("UsePacket"));
		}
		
		private static bool IgniteKillable(Obj_AI_Base Target)
		{
			return Player.GetSummonerSpellDamage(Target,Damage.SummonerSpell.Ignite) >= Target.Health;
		}
		
		private static float GetDistanceSqr(Obj_AI_Hero source, Obj_AI_Base Target)
		{
			return Vector2.DistanceSquared(source.Position.To2D(),Target.ServerPosition.To2D());
		}
		
		private static double ComboDmg(Obj_AI_Hero target)
		{
			double damage = 0;
			if (Q.IsReady() && Q.InRange(target.Position))
				damage += Player.GetSpellDamage(target,SpellSlot.Q);
			if (E.IsReady() && E.InRange(target.Position))
				damage += Player.GetSpellDamage(target,SpellSlot.E);
			if (R.IsReady() && R.InRange(target.Position))
				damage += Player.GetSpellDamage(target,SpellSlot.R);
			if ((Items.HasItem(3128) && Items.CanUseItem(3128)) || (Items.HasItem(3188) && Items.CanUseItem(3188)) && GetDistanceSqr(Player,Target) <= 750 * 750)
				damage += damage * 1.2;
			if (CanIgnite() && Player.Distance(target) <= 600) damage += Player.GetSummonerSpellDamage(target,Damage.SummonerSpell.Ignite);
			return damage;
		}
		
		private static bool CanIgnite()
		{
			return (IgniteSlot != SpellSlot.Unknown && Player.SummonerSpellbook.CanUseSpell(IgniteSlot) == SpellState.Ready);
		}
		
		private static void UseCombo()
		{
			if (Target == null) return;
			
			bool AllSkills = false;
			if (ComboDmg(Target) > Target.Health && Target.Distance(Player) <= 950 ) AllSkills = true;
			
			if (GetBool("UseItems") && AllSkills && GetDistanceSqr(Player,Target) <= 750 * 750)
			{
				if (Items.CanUseItem(3128)) Items.UseItem(3128,Target);
				if (Items.CanUseItem(3188)) Items.UseItem(3188,Target);
			}
			if (GetBool("UseQ") && Q.IsReady() && GetDistanceSqr(Player,Target) <= Q.Range * Q.Range)
			{
				Q.CastIfHitchanceEquals(Target, HitC ,GetBool("UsePacket"));
				if (Target.IsValidTarget(550) && Target.HasBuff("luxilluminatingfraulein"))
					Player.IssueOrder(GameObjectOrder.AttackUnit, Target);
			}
			if (GetBool("UseW")  && W.IsReady() && Utility.IsFacing(Player,Target,450) && Player.Distance(Target) <= 450)
			{
				W.Cast(Target,GetBool("UsePacket") );
			}
			if (GetBool("UseE")  && E.IsReady() && GetDistanceSqr(Player,Target) <= E.Range * E.Range)
			{
				if (GetBool("UseQE") )
				{
					if (Target.HasBuff("LuxLightBindingMis"))
					{
						E.Cast(Target ,GetBool("UsePacket") );
						CastE2();
					}
				}
				else
				{
					E.CastIfHitchanceEquals(Target, HitC ,GetBool("UsePacket") );
					CastE2();
				}
				if (Target.IsValidTarget(550) && Target.HasBuff("luxilluminatingfraulein"))
					Player.IssueOrder(GameObjectOrder.AttackUnit, Target);
			}
			if (GetBool("UseR") && R.IsReady() && (R.IsKillable(Target) || AllSkills))
			{
				if (Target.Health <= Damage.GetAutoAttackDamage(Player,Target,true) && Player.Distance(Target) < 550)
					Player.IssueOrder(GameObjectOrder.AttackUnit, Target);
				else R.CastIfHitchanceEquals(Target,HitChance.High ,GetBool("UsePacket"));
			}
			if (GetBool("UseIgnite") && (IgniteKillable(Target) || AllSkills) && CanIgnite())
			{
				if (Player.Distance(Target) <= 600)
					if (Target.Health <= Damage.GetAutoAttackDamage(Player,Target,true) && Player.Distance(Target) < 550)
						Player.IssueOrder(GameObjectOrder.AttackUnit, Target);
				else Player.SummonerSpellbook.CastSpell(IgniteSlot, Target);
			}
		}
		
		
		private static void Harass()
		{
			if (Target == null) return;
			if (GetBool("HQ") && Q.IsReady() && GetDistanceSqr(Player,Target) <= Q.Range * Q.Range)
			{
				Q.CastIfHitchanceEquals(Target, HitC ,GetBool("UsePacket"));
				if (Target.IsValidTarget(550) && Target.HasBuff("luxilluminatingfraulein"))
					Player.IssueOrder(GameObjectOrder.AttackUnit, Target);
			}
			if (GetBool("HE") && E.IsReady() && GetDistanceSqr(Player,Target) <= E.Range * E.Range)
			{
				if (GetBool("UseQE"))
				{
					if (Target.HasBuff("LuxLightBindingMis"))
					{
						E.CastIfHitchanceEquals(Target, HitC ,GetBool("UsePacket"));
						CastE2();
					}
				}
				else
				{
					E.CastIfHitchanceEquals(Target, HitC ,GetBool("UsePacket"));
					CastE2();
				}
				if (Target.IsValidTarget(550) && Target.HasBuff("luxilluminatingfraulein"))
					Player.IssueOrder(GameObjectOrder.AttackUnit, Target);
			}
		}
		
		private static double CalculateDmg(Obj_AI_Base target)
		{
			double damage = 0;
			if (Q.IsReady() && Q.InRange(target.Position))
				damage += Player.GetSpellDamage(target,SpellSlot.Q);
			if (E.IsReady() && E.InRange(target.Position))
				damage += Player.GetSpellDamage(target,SpellSlot.E);
			if (R.IsReady() && R.InRange(target.Position))
				damage += Player.GetSpellDamage(target,SpellSlot.R);
			return damage;
		}
		
		private static void JungSteal()
		{
			var Minions = MinionManager.GetMinions(Game.CursorPos, 1000, MinionTypes.All, MinionTeam.Neutral);
			foreach (var minion in Minions.Where(minion => minion.IsVisible && !minion.IsDead ))
			{
				if ((minion.SkinName == "SRU_Blue" || minion.SkinName == "SRU_Red" || minion.SkinName == "SRU_Baron" || minion.SkinName == "SRU_Dragon") &&
				    CalculateDmg(minion) > minion.Health)
				{
					if (Q.IsReady() && GetDistanceSqr(Player,minion) <= Q.Range * Q.Range) Q.Cast(minion,GetBool("UsePacket"));
					if (E.IsReady() && GetDistanceSqr(Player,minion) <= E.Range * E.Range)
					{
						E.Cast(minion,GetBool("UsePacket"));
						while (EObject != null)
						{
							E.Cast(GetBool("UsePacket"));
							break;
						}
					}
					if (R.IsReady() && minion.IsValidTarget(R.Range)) R.Cast(minion,GetBool("UsePacket"));
				}
			}
		}
		
		private static void KillSteal()
		{
			if (GetBool("KUseQ") || GetBool("KUseE") || GetBool("KUseR") || GetBool("KIgnite"))
			{
				foreach (var hero in ObjectManager.Get<Obj_AI_Hero>().Where(hero => hero.IsValidTarget(R.Range) && hero.IsEnemy && !hero.IsDead))
				{
					if (GetBool("KUseQ") && Q.IsReady() && GetDistanceSqr(Player,hero) <= Q.Range * Q.Range && Q.IsKillable(hero))
						Q.CastIfHitchanceEquals(hero, HitChance.High ,GetBool("UsePacket"));
					else if (GetBool("KUseE") && E.IsReady() && GetDistanceSqr(Player,hero) <= E.Range * E.Range && E.IsKillable(hero) )
					{
						E.CastIfHitchanceEquals(hero, HitChance.High ,GetBool("UsePacket"));
						CastE2();
					}
					else if (GetBool("KUseR") && R.IsReady() && hero.IsValidTarget(R.Range) && R.IsKillable(hero))
					{
						if (hero.Health <= Damage.GetAutoAttackDamage(Player,hero,true) && Player.Distance(hero) < 550)
							Player.IssueOrder(GameObjectOrder.AttackUnit, hero);
						else R.CastIfHitchanceEquals(hero,HitChance.High ,GetBool("UsePacket"));
					}
					else if (GetBool("KIgnite") && IgniteKillable(hero) && CanIgnite())
					{
						if (Player.Distance(hero) <= 600)
						{
							if (hero.Health <= Damage.GetAutoAttackDamage(Player,hero,true) && Player.Distance(hero) < 550)
								Player.IssueOrder(GameObjectOrder.AttackUnit, hero);
							else Player.SummonerSpellbook.CastSpell(IgniteSlot, hero);
						}
					}
				}
			}
		}
		
		
		private static void CastE2()
		{
			if (EObject == null) return;
			foreach (Obj_AI_Hero current in ObjectManager.Get<Obj_AI_Hero>().Where(enemy => enemy.IsValidTarget() && enemy.IsEnemy &&
			                                                                       Vector3.Distance(EObject.Position, enemy.ServerPosition) <= E.Width+15))
			{
				E.Cast(GetBool("UsePacket"));
				return;
			}
			if (Vector3.Distance(Player.Position, EObject.Position) > 800)	E.Cast(GetBool("UsePacket"));
		}
		
		private static void Farm()
		{
			var Minions = MinionManager.GetMinions(Player.Position, E.Range, MinionTypes.All, MinionTeam.NotAlly);
			if (Minions.Count == 0 ) return;
			if (Player.Mana/Player.MaxMana*100 >= GetSlider("MP"))
			{
				if (GetBool("FQ") && Q.IsReady())
				{
					var castPostion = MinionManager.GetBestLineFarmLocation(Minions.Select(minion => minion.ServerPosition.To2D()).ToList(), Q.Width, Q.Range);
					Q.Cast(castPostion.Position, GetBool("UsePacket"));
				}
				if (GetBool("FE") && E.IsReady())
				{
					var castPostion = MinionManager.GetBestCircularFarmLocation(Minions.Select(minion => minion.ServerPosition.To2D()).ToList(), E.Width, E.Range);
					E.Cast(castPostion.Position, GetBool("UsePacket"));
					E.Cast(GetBool("UsePacket"));
				}
			}
		}
	}
}
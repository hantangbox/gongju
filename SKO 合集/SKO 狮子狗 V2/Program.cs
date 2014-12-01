using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

namespace SKO_Rengar_V2
{
	class Program
	{
		private static Obj_AI_Hero _player;
		private static Spell Q, W, E, R;
		private static Items.Item BWC, BRK, RO, YMG, STD, TMT, HYD;
		private static bool PacketCast;
		private static Menu SKOMenu;
		private static bool Recall;
	    private static bool canCastQ;
		private static SpellSlot IgniteSlot, TeleportSlot;

		public static void Main (string[] args)
		{
			CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
		}

		private static void Game_OnGameLoad(EventArgs args)
		{
			_player = ObjectManager.Player;
			if (_player.ChampionName != "Rengar")
				return;

			SKOMenu = new Menu ("SKO 狮子狗","SKORengar", true);

			var SKOTs = new Menu ("目标选择","TargetSelector");
			SimpleTs.AddToMenu(SKOTs);

			var OrbMenu = new Menu ("走砍", "Orbwalker");
			LXOrbwalker.AddToMenu (OrbMenu);


			var Combo = new Menu ("连招", "Combo");
			Combo.AddItem(new MenuItem("CPrio", "技能优先级").SetValue(new StringList(new[] {"Q", "W", "E"}, 2)));
			Combo.AddItem(new MenuItem ("UseQ", "使用 Q").SetValue(true));
			Combo.AddItem(new MenuItem ("UseW", "使用 W").SetValue(true));
			Combo.AddItem(new MenuItem ("UseE", "使用 E").SetValue(true));
			Combo.AddItem(new MenuItem ("UseItemsCombo", "使用 点燃").SetValue(true));
			Combo.AddItem(new MenuItem ("UseAutoW", "自动 W").SetValue(true));
			Combo.AddItem(new MenuItem ("HpAutoW", "自动W最低蓝量").SetValue(new Slider(20,1,100)));
			Combo.AddItem (new MenuItem("TripleQ", "三倍 Q").SetValue(new KeyBind(OrbMenu.Item("Flee_Key").GetValue<KeyBind>().Key, KeyBindType.Press)));
			Combo.AddItem(new MenuItem("activeCombo", "连招!").SetValue(new KeyBind(OrbMenu.Item("Combo_Key").GetValue<KeyBind>().Key, KeyBindType.Press)));

			var Harass = new Menu("骚扰", "Harass");
			Harass.AddItem(new MenuItem("HPrio", "技能优先级").SetValue(new StringList(new[] {"W", "E"}, 1)));
			Harass.AddItem(new MenuItem("UseWH", "使用 W").SetValue(true));
			Harass.AddItem(new MenuItem("UseEH", "使用 E").SetValue(true));
			Harass.AddItem(new MenuItem ("UseItemsHarass", "使用 点燃").SetValue(true));
			Harass.AddItem(new MenuItem("activeHarass","骚扰!").SetValue(new KeyBind(OrbMenu.Item("Harass_Key").GetValue<KeyBind>().Key, KeyBindType.Press)));

			var JLClear = new Menu("清野|清线", "JLClear");
			JLClear.AddItem(new MenuItem("FPrio", "技能优先级").SetValue(new StringList(new[] {"Q", "W", "E"}, 0)));
			JLClear.AddItem(new MenuItem("UseQC", "使用 Q").SetValue(true));
			JLClear.AddItem(new MenuItem("UseWC", "使用 W").SetValue(true));
			JLClear.AddItem(new MenuItem("UseEC", "使用 E").SetValue(true));
			JLClear.AddItem(new MenuItem("Save", "存储 残暴值").SetValue(false));
			JLClear.AddItem(new MenuItem("UseItemsClear", "使用点燃").SetValue(true));
			JLClear.AddItem(new MenuItem("activeClear","清野|清线!").SetValue(new KeyBind(OrbMenu.Item("LaneClear_Key").GetValue<KeyBind>().Key, KeyBindType.Press)));

			var TROLLZINHONASRANKEDS = new Menu("抢人头", "TristanaKillZiggs");
			TROLLZINHONASRANKEDS.AddItem(new MenuItem("Foguinho", "使用 点燃").SetValue(true));
			TROLLZINHONASRANKEDS.AddItem(new MenuItem("UseQKs", "使用 Q").SetValue(true));
			TROLLZINHONASRANKEDS.AddItem(new MenuItem("UseWKs", "使用 W").SetValue(true));
			TROLLZINHONASRANKEDS.AddItem(new MenuItem("UseEKs", "使用 E").SetValue(true));
			TROLLZINHONASRANKEDS.AddItem(new MenuItem("UseFlashKs", "使用 闪现击杀(Kappa)").SetValue(false));

			var CHUPARUNSCUEPA = new Menu("范围", "Drawing");
			CHUPARUNSCUEPA.AddItem(new MenuItem("DrawQ", "范围 Q").SetValue(true));
			CHUPARUNSCUEPA.AddItem(new MenuItem("DrawW", "范围 W").SetValue(true));
			CHUPARUNSCUEPA.AddItem(new MenuItem("DrawE", "范围 E").SetValue(true));
			CHUPARUNSCUEPA.AddItem(new MenuItem("DrawR", "范围 R").SetValue(true));
			CHUPARUNSCUEPA.AddItem(new MenuItem("CircleLag", "延迟自由圈").SetValue(true));
			CHUPARUNSCUEPA.AddItem(new MenuItem("CircleQuality", "圈质量").SetValue(new Slider(100, 100, 10)));
			CHUPARUNSCUEPA.AddItem(new MenuItem("CircleThickness", "圈厚度").SetValue(new Slider(1, 10, 1)));

			var Misc = new Menu("杂项", "Misc");
			Misc.AddItem(new MenuItem("UsePacket","使用 封包").SetValue(true));
			Misc.AddItem(new MenuItem("TpREscape", "使用R+TP 逃跑").SetValue<KeyBind>(new KeyBind("T".ToCharArray()[0], KeyBindType.Press)));

			Game.PrintChat("<font color='#07B88C'>SKO |鐙瓙鐙梶 V2 鍔犺級鎴愬姛锛佹饥鍖朾y浜岀嫍锛丵Q缇361630847!</font>");

			SKOMenu.AddSubMenu(SKOTs);
			SKOMenu.AddSubMenu(OrbMenu);
			SKOMenu.AddSubMenu(Combo);
			SKOMenu.AddSubMenu(Harass);
			SKOMenu.AddSubMenu(JLClear);
			SKOMenu.AddSubMenu(TROLLZINHONASRANKEDS);
			SKOMenu.AddSubMenu(CHUPARUNSCUEPA);
			SKOMenu.AddSubMenu(Misc);
			SKOMenu.AddToMainMenu();

			W = new Spell(SpellSlot.W, 450f);
			E = new Spell(SpellSlot.E, 980f);
			R = new Spell(SpellSlot.R, 2000f);

			E.SetSkillshot(0.25f, 70f, 1500f, true, SkillshotType.SkillshotLine);

			HYD = new Items.Item(3074, 420f);
			TMT = new Items.Item(3077, 420f);
			BRK = new Items.Item(3153, 450f);
			BWC = new Items.Item(3144, 450f);
			RO = new Items.Item(3143, 500f);

			PacketCast = SKOMenu.Item("UsePacket").GetValue<bool>();

			IgniteSlot = _player.GetSpellSlot("SummonerDot");
			TeleportSlot = _player.GetSpellSlot("SummonerTeleport");

			Game.OnGameUpdate += Game_OnGameUpdate;
			Drawing.OnDraw += Draw_OnDraw;
		}
			

		private static void Game_OnGameUpdate(EventArgs args)
		{
			TPREscape();

			if(SKOMenu.Item("activeClear").GetValue<KeyBind>().Active)
			{
				Clear();
			}
			var tqtarget = SimpleTs.GetTarget(R.Range, SimpleTs.DamageType.Physical);
			if(SKOMenu.Item("TripleQ").GetValue<KeyBind>().Active)
			{
				TripleQ(tqtarget);
			}

			var target = SimpleTs.GetTarget(E.Range, SimpleTs.DamageType.Physical);

		    Q = new Spell(SpellSlot.Q, _player.AttackRange + 100);
			YMG = new Items.Item(3142, _player.AttackRange+50);
			STD = new Items.Item(3131, _player.AttackRange+50);

			AutoHeal();
			KillSteal(target);

			if(SKOMenu.Item("activeCombo").GetValue<KeyBind>().Active)
			{
			    if (_player.Mana <= 4)
			    {
                    if (SKOMenu.Item("UseQ").GetValue<bool>() && _player.Distance(target) <= Q.Range)
			        {
			            CastQ(target);
			        }
			        if (SKOMenu.Item("UseW").GetValue<bool>() && _player.Distance(target) <= W.Range)
			        {
			            CastW(target);
			        }
			        if (SKOMenu.Item("UseE").GetValue<bool>() && _player.Distance(target) <= E.Range)
			        {
			            CastE(target);
			        }
			    }

			    if(_player.Mana == 5)
					{
						if(SKOMenu.Item("UseQ").GetValue<bool>() && SKOMenu.Item("CPrio").GetValue<StringList>().SelectedIndex == 0  && _player.Distance(target) <= Q.Range)
						{
							CastQ(target);
						}
						if(SKOMenu.Item("UseW").GetValue<bool>() && SKOMenu.Item("CPrio").GetValue<StringList>().SelectedIndex == 1  && _player.Distance(target) <= W.Range)
						{
							CastW(target);
						}
						if(SKOMenu.Item("UseE").GetValue<bool>() && SKOMenu.Item("CPrio").GetValue<StringList>().SelectedIndex == 2 && _player.Distance(target) <= E.Range)
						{
							CastE(target);
						}

						//E if !Q.IsReady()
						if(SKOMenu.Item("UseE").GetValue<bool>() && !Q.IsReady() && _player.Distance(target) > Q.Range)
						{
							CastE(target);
						}
					}
					if(SKOMenu.Item("UseItemsCombo").GetValue<bool>()){
						if(_player.Distance(target) < _player.AttackRange+50){
							TMT.Cast();
							HYD.Cast();
							STD.Cast();
						}
						BWC.Cast(target);
						BRK.Cast(target);
						RO.Cast(target);
						YMG.Cast();
					}

			}
			if(SKOMenu.Item("activeHarass").GetValue<KeyBind>().Active)
			{
				Harass();
			}
		}

		private static void Draw_OnDraw(EventArgs args)
		{
			if (SKOMenu.Item("CircleLag").GetValue<bool>())
			{
				if (SKOMenu.Item("DrawQ").GetValue<bool>())
				{
					Utility.DrawCircle(_player.Position, Q.Range, Color.White,
						SKOMenu.Item("CircleThickness").GetValue<Slider>().Value,
						SKOMenu.Item("CircleQuality").GetValue<Slider>().Value);
				}
				if (SKOMenu.Item("DrawW").GetValue<bool>())
				{
					Utility.DrawCircle(_player.Position, W.Range, Color.White,
						SKOMenu.Item("CircleThickness").GetValue<Slider>().Value,
						SKOMenu.Item("CircleQuality").GetValue<Slider>().Value);
				}
				if (SKOMenu.Item("DrawE").GetValue<bool>())
				{
					Utility.DrawCircle(_player.Position, E.Range, Color.White,
						SKOMenu.Item("CircleThickness").GetValue<Slider>().Value,
						SKOMenu.Item("CircleQuality").GetValue<Slider>().Value);
				}
				if (SKOMenu.Item("DrawR").GetValue<bool>())
				{
					Utility.DrawCircle(_player.Position, R.Range, Color.White,
						SKOMenu.Item("CircleThickness").GetValue<Slider>().Value,
						SKOMenu.Item("CircleQuality").GetValue<Slider>().Value);
				}
			}
			else
			{
				if (SKOMenu.Item("DrawQ").GetValue<bool>())
				{
					Drawing.DrawCircle(_player.Position, Q.Range, Color.Green);
				}
				if (SKOMenu.Item("DrawW").GetValue<bool>())
				{
					Drawing.DrawCircle(_player.Position, W.Range, Color.Green);
				}
				if (SKOMenu.Item("DrawE").GetValue<bool>())
				{
					Drawing.DrawCircle(_player.Position, E.Range, Color.Green);
				}
				if (SKOMenu.Item("DrawR").GetValue<bool>())
				{
					Drawing.DrawCircle(_player.Position, R.Range, Color.Green);
				}
			}
		}

		private static void KillSteal(Obj_AI_Hero target)
		{
			var igniteDmg = _player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
			var qDmg = _player.GetSpellDamage(target, SpellSlot.Q);
			var wDmg = _player.GetSpellDamage(target, SpellSlot.Q);
			var eDmg = _player.GetSpellDamage(target, SpellSlot.Q);

			if(target.IsValidTarget())
			{
				if(SKOMenu.Item("Foguinho").GetValue<bool>() && _player.SummonerSpellbook.CanUseSpell(IgniteSlot) == SpellState.Ready)
				{
					if(igniteDmg > target.Health && _player.Distance(target) < 600)
					{
						_player.SummonerSpellbook.CastSpell(IgniteSlot, target);
					}
				}
			}

			if(SKOMenu.Item("UseQKs").GetValue<bool>())
			{
				if(qDmg > target.Health && _player.Distance(target) <= Q.Range)
				{
					CastQ(target);
				}
			}
			if(SKOMenu.Item("UseWKs").GetValue<bool>())
			{
				if(wDmg > target.Health && _player.Distance(target) <= W.Range)
				{
					CastW(target);
				}
			}
			if(SKOMenu.Item("UseEKs").GetValue<bool>())
			{
				if(eDmg > target.Health && _player.Distance(target) <= E.Range)
				{
					CastE(target);
				}
			}
		}


		private static void TPREscape()
		{
			if (SKOMenu.Item("TpREscape").GetValue<KeyBind>().Active) 
			{
				if (R.IsReady() && _player.SummonerSpellbook.CanUseSpell(TeleportSlot) == SpellState.Ready)
				{
				    R.Cast();

				    foreach (Obj_AI_Turret turrenttp in ObjectManager.Get<Obj_AI_Turret>().Where(turrenttp => turrenttp.IsAlly && turrenttp.Name == "Turret_T1_C_02_A" || turrenttp.Name == "Turret_T2_C_01_A"))
				    {
				        _player.SummonerSpellbook.CastSpell(TeleportSlot, turrenttp);
				    }
				}
			}
		}

		private static void Clear()
		{
		    var allminions = MinionManager.GetMinions(_player.ServerPosition, 1000, MinionTypes.All, MinionTeam.NotAlly, MinionOrderTypes.MaxHealth);

		    foreach (var minion in allminions.Where(minion => minion.IsValidTarget()))
		    {
		        if(_player.Mana <= 4)
		        {
		            if(Q.IsReady() && SKOMenu.Item("UseQC").GetValue<bool>() && _player.Distance(minion) <= Q.Range)
		            {
		                Q.Cast();
		            }
		            if(W.IsReady() && SKOMenu.Item("UseWC").GetValue<bool>() && _player.Distance(minion) <= W.Range)
		            {
		                W.Cast();
		            }
		            if(E.IsReady() && SKOMenu.Item("UseEC").GetValue<bool>() && _player.Distance(minion) <= E.Range)
		            {
		                E.Cast(minion, PacketCast);
		            }
		        }
		        if(_player.Mana == 5)
		        {
		            if(SKOMenu.Item("Save").GetValue<bool>())return;
		            if(SKOMenu.Item("FPrio").GetValue<StringList>().SelectedIndex == 0 && Q.IsReady() && SKOMenu.Item("UseQC").GetValue<bool>() && _player.Distance(minion) <= Q.Range)
		            {
		                Q.Cast();
		            }
		            if(SKOMenu.Item("FPrio").GetValue<StringList>().SelectedIndex == 1 && W.IsReady() && SKOMenu.Item("UseWC").GetValue<bool>() && _player.Distance(minion) <= W.Range)
		            {
		                W.Cast();
		            }
		            if(SKOMenu.Item("FPrio").GetValue<StringList>().SelectedIndex == 2 && E.IsReady() && SKOMenu.Item("UseEC").GetValue<bool>() && _player.Distance(minion) <= E.Range)
		            {
		                E.Cast(minion, PacketCast);
		            }
		        }
		        if(SKOMenu.Item("UseItemsClear").GetValue<bool>()){
		            if(_player.Distance(minion) < _player.AttackRange+50){
		                TMT.Cast();
		                HYD.Cast();
		            }
		            YMG.Cast();
		        }
		    }
		}

	    private static void Harass()
		{
			var target = SimpleTs.GetTarget(E.Range, SimpleTs.DamageType.Physical);
			if(target.IsValidTarget())
			{
				if(_player.Mana <= 4)
				{
					if(SKOMenu.Item("UseWH").GetValue<bool>() && _player.Distance(target) <= W.Range){
						CastW(target);
					}
					if(SKOMenu.Item("UseEH").GetValue<bool>() && _player.Distance(target) <= E.Range){
						CastE(target);
					}
				}
				if(_player.Mana == 5)
				{
					if(SKOMenu.Item("UseWH").GetValue<bool>() && SKOMenu.Item("HPrio").GetValue<StringList>().SelectedIndex == 0)
					{
						CastW(target);
					}
					if(SKOMenu.Item("UseEH").GetValue<bool>() && SKOMenu.Item("HPrio").GetValue<StringList>().SelectedIndex == 1){
						CastE(target);
					}
				}
				if(SKOMenu.Item("UseItemsHarass").GetValue<bool>()){
					if(_player.Distance(target) < _player.AttackRange+50){
						TMT.Cast();
						HYD.Cast();
						STD.Cast();
					}
					BWC.Cast(target);
					BRK.Cast(target);
					RO.Cast(target);
					YMG.Cast();
				}
			}
		}

		private static void TripleQ(Obj_AI_Hero target)
		{
			if(target.IsValidTarget()){
				if(_player.Mana == 5 && R.IsReady() && _player.Distance(target) <= R.Range && Q.IsReady())
				{
					R.Cast();
				}
				if(_player.Mana == 5 && _player.HasBuff("RengarR") && _player.Distance(target) <= Q.Range)
				{
					CastQ(target);
				}
				if(_player.Mana == 5 && !_player.HasBuff("RengarR") && _player.Distance(target) <= Q.Range)
				{
					CastQ(target);
				}
				if(_player.Mana <= 4)
				{
					if(_player.Distance(target) <= Q.Range)
					{
						CastQ(target);
					}
					if(_player.Distance(target) <= W.Range)
					{
						CastW(target);
					}
					if(_player.Distance(target) <= E.Range)
					{
						CastE(target);
					}
				}
				if(_player.Distance(target) < _player.AttackRange+50){
					TMT.Cast();
					HYD.Cast();
					STD.Cast();
				}
				BWC.Cast(target);
				BRK.Cast(target);
				RO.Cast(target);
				YMG.Cast();

			}
		}

		private static void AutoHeal()
		{
			if (_player.HasBuff("Recall") || _player.Mana <= 4) return;

			if(SKOMenu.Item("UseAutoW").GetValue<bool>())
			{
				if(W.IsReady() && _player.Health < (_player.MaxHealth * (SKOMenu.Item("HpAutoW").GetValue<Slider>().Value) / 100))
				{
					W.Cast();
				}
			}
		}

	    private static void CastQ(Obj_AI_Hero target)
	    {
            if (!Q.IsReady() || !target.IsValidTarget(Q.Range)) return;
	        try
	        {
				//if(LXOrbwalker.IsAutoAttackReset(_player.Spellbook.GetSpell(SpellSlot.Q).SData.Name))
				//Utility.DelayAction.Add(260, LXOrbwalker.ResetAutoAttackTimer);
				LXOrbwalker.ResetAutoAttackTimer();
                Q.Cast(PacketCast);
	        }
	        catch (Exception e)
	        {
	            Console.WriteLine("{0}", e.Message);
	        }
        }

        private static void CastW(Obj_AI_Hero target)
	    {
            if (!W.IsReady() || !target.IsValidTarget(W.Range)) return;
            try
            {
                W.Cast(PacketCast);
            }
            catch (Exception e)
            {
                Console.WriteLine("{0}", e.ToString());
            }
	    }

	    private static void CastE(Obj_AI_Hero target)
	    {
            if (!E.IsReady() || !target.IsValidTarget(E.Range)) return;
            try
            {
                var epred = E.GetPrediction(target);
                if (epred.Hitchance >= HitChance.High)
                {
                    E.Cast(epred.CastPosition, PacketCast);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("{0}", e.ToString());
            }
	    }

	}
}

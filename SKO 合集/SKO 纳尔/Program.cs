using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

namespace VallySKO_Gnar
{
    class Program
    {
        private static Obj_AI_Hero player;
        private static Orbwalking.Orbwalker Orbwalker;
        private static Spell MiniQ, MiniE, MegaQ, MegaW, MegaE, R;
        private static Items.Item BWC, BRK, RO, YMG, STD, TMT, HYD;
        private static bool PacketCast;
        private static Menu SKOMenu;
        private static SpellSlot IgniteSlot;
        private static bool MegaGnar;

        private static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        private static void Game_OnGameLoad(EventArgs args)
        {
            player = ObjectManager.Player;
            if (player.ChampionName != "Gnar")
                return;

            IgniteSlot = player.GetSpellSlot("SummonerDot");

            /*Mini Gnar*/
            MiniQ = new Spell(SpellSlot.Q, 1100f);
            MiniQ.SetSkillshot(0.066f, 60f, 1400f, true, SkillshotType.SkillshotLine);
            MiniE = new Spell(SpellSlot.E, 475f);
            MiniE.SetSkillshot(0.695f, 150f, 2000f, false, SkillshotType.SkillshotCircle);

            /*Mega Gnar*/
            MegaQ = new Spell(SpellSlot.Q, 1100f);
            MegaQ.SetSkillshot(0.060f, 90f, 2100f, true, SkillshotType.SkillshotLine);
            MegaW = new Spell(SpellSlot.W, 525f);
            MegaW.SetSkillshot(0.25f, 80f, 1200f, false, SkillshotType.SkillshotLine);
            MegaE = new Spell(SpellSlot.E, 475f);
            MegaE.SetSkillshot(0.695f, 350f, 2000f, false, SkillshotType.SkillshotCircle);
            R = new Spell(SpellSlot.R, 590f);
            R.SetSkillshot(0.066f, 400f, 1400f, false, SkillshotType.SkillshotCircle);

            HYD = new Items.Item(3074, 420f);
            TMT = new Items.Item(3077, 420f);
            BRK = new Items.Item(3153, 450f);
            BWC = new Items.Item(3144, 450f);
            RO = new Items.Item(3143, 500f);

            SKOMenu = new Menu("SKO 纳尔", "VSGnar", true);

            var SKOTs = new Menu("目标 选择", "TargetSelector");
            SimpleTs.AddToMenu(SKOTs);

            SKOMenu.AddSubMenu(new Menu("走砍", "Orbwalking"));
            new Orbwalking.Orbwalker(SKOMenu.SubMenu("Orbwalking"));

            var Combo = new Menu("连招", "Combo");
            Combo.AddItem(new MenuItem("UseQ", "使用 Q").SetValue(true));
            Combo.AddItem(new MenuItem("UseW", "使用 W").SetValue(true));
			Combo.AddItem(new MenuItem("UseE", "使用 E").SetValue(true));
			Combo.AddItem(new MenuItem("UseEunder", "目标在塔下自动E").SetValue(true));
            Combo.AddItem(new MenuItem("UseR", "使用 R").SetValue(true));
            Combo.AddItem(new MenuItem("AutoR", "自动 大招").SetValue(true));
            Combo.AddItem(new MenuItem("MinRenemys", "自动大招|敌人数量").SetValue(new Slider(3, 1, 5)));
            Combo.AddItem(new MenuItem("UseItemsCombo", "使用 物品").SetValue(true));
            Combo.AddItem(new MenuItem("activeCombo", "连招!").SetValue(new KeyBind(32, KeyBindType.Press)));

            var Harass = new Menu("骚扰", "Harass");
            Harass.AddItem(new MenuItem("UseQH", "使用 Q").SetValue(true));
            Harass.AddItem(new MenuItem("UseWH", "使用 W").SetValue(true));
            Harass.AddItem(new MenuItem("UseItemsHarass", "使用 Items").SetValue(true));
            Harass.AddItem(new MenuItem("activeHarass", "骚扰!").SetValue(new KeyBind("X".ToCharArray()[0], KeyBindType.Press)));

            var JLClear = new Menu("清野|清兵", "JLClear");
            JLClear.AddItem(new MenuItem("UseQC", "使用 Q").SetValue(true));
			//JLClear.AddItem(new MenuItem("UseQClh", "Use Q only for LastHit").SetValue(true));
            JLClear.AddItem(new MenuItem("UseWC", "使用 W").SetValue(true));
            JLClear.AddItem(new MenuItem("UseItemsClear", "使用 物品").SetValue(true));
            JLClear.AddItem(new MenuItem("activeClear", "清野|清兵!").SetValue(new KeyBind("V".ToCharArray()[0], KeyBindType.Press)));

            var KillSteal = new Menu("KillSteal", "KillSteal");
            KillSteal.AddItem(new MenuItem("Foguinho", "使用 Ignite").SetValue(true));
            KillSteal.AddItem(new MenuItem("UseQKs", "使用 Q").SetValue(true));
            KillSteal.AddItem(new MenuItem("UseWKs", "使用 W").SetValue(true));

            var Drawc = new Menu("范围", "Drawing");
            Drawc.AddItem(new MenuItem("DrawQ", "范围 Q").SetValue(true));
            Drawc.AddItem(new MenuItem("DrawW", "范围 W").SetValue(true));
            Drawc.AddItem(new MenuItem("DrawE", "范围 E").SetValue(true));
            Drawc.AddItem(new MenuItem("DrawR", "范围 R").SetValue(true));
            Drawc.AddItem(new MenuItem("CircleLag", "延迟自由圈").SetValue(true));
            Drawc.AddItem(new MenuItem("CircleQuality", "圈 质量").SetValue(new Slider(100, 100, 10)));
            Drawc.AddItem(new MenuItem("CircleThickness", "圈 厚度").SetValue(new Slider(1, 10, 1)));

            var Misc = new Menu("杂项", "Misc");
            Misc.AddItem(new MenuItem("UsePacket", "使用 封包").SetValue(true));

            SKOMenu.AddSubMenu(SKOTs);
            SKOMenu.AddSubMenu(Combo);
            SKOMenu.AddSubMenu(Harass);
            SKOMenu.AddSubMenu(JLClear);
            SKOMenu.AddSubMenu(KillSteal);
            SKOMenu.AddSubMenu(Drawc);
            SKOMenu.AddSubMenu(Misc);
            SKOMenu.AddToMainMenu();

            Game.PrintChat("<font color='#07B88C'>SKO 绾冲皵</font> 鍔犺級鎴愬姛锛佹饥鍖朾y浜岀嫍锛丵Q缇361630847!");


            Game.OnGameUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += OnDraw;
        }


        private static void Game_OnGameUpdate(EventArgs args)
        {

            //Checks
            MegaGnar = player.HasBuff("gnartransform");
            PacketCast = SKOMenu.Item("UsePacket").GetValue<bool>();
            YMG = new Items.Item(3142, player.AttackRange + player.BoundingRadius);

            

            if (SKOMenu.Item("activeClear").GetValue<KeyBind>().Active)
            {
                Clear();
            }

            var target = SimpleTs.GetTarget(MegaQ.Range, SimpleTs.DamageType.Physical);
            if (SKOMenu.Item("activeCombo").GetValue<KeyBind>().Active)
            {

                Combo(target);

            }
            if (SKOMenu.Item("activeHarass").GetValue<KeyBind>().Active)
            {
                Harass(target);
            }

			if (SKOMenu.Item("AutoR").GetValue<bool>() && player.CountEnemysInRange(500) >= SKOMenu.Item("MinRenemys").GetValue<Slider>().Value)
            {
                CastR(target);
            }

            KillSteal(target);

        }

        private static void OnDraw(EventArgs args)
        {
            if (SKOMenu.Item("CircleLag").GetValue<bool>())
            {
                if (SKOMenu.Item("DrawQ").GetValue<bool>())
                {
                    Utility.DrawCircle(player.Position, MegaQ.Range, Color.White,
                        SKOMenu.Item("CircleThickness").GetValue<Slider>().Value,
                        SKOMenu.Item("CircleQuality").GetValue<Slider>().Value);
                }
                if (SKOMenu.Item("DrawW").GetValue<bool>())
                {
                    Utility.DrawCircle(player.Position, MegaW.Range, Color.White,
                        SKOMenu.Item("CircleThickness").GetValue<Slider>().Value,
                        SKOMenu.Item("CircleQuality").GetValue<Slider>().Value);
                }
                if (SKOMenu.Item("DrawE").GetValue<bool>())
                {
                    Utility.DrawCircle(player.Position, MegaE.Range, Color.White,
                        SKOMenu.Item("CircleThickness").GetValue<Slider>().Value,
                        SKOMenu.Item("CircleQuality").GetValue<Slider>().Value);
                }
                if (SKOMenu.Item("DrawR").GetValue<bool>())
                {
                    Utility.DrawCircle(player.Position, R.Range, Color.White,
                        SKOMenu.Item("CircleThickness").GetValue<Slider>().Value,
                        SKOMenu.Item("CircleQuality").GetValue<Slider>().Value);
                }
            }
            else
            {
                if (SKOMenu.Item("DrawQ").GetValue<bool>())
                {
                    Drawing.DrawCircle(player.Position, MegaQ.Range, Color.Green);
                }
                if (SKOMenu.Item("DrawW").GetValue<bool>())
                {
                    Drawing.DrawCircle(player.Position, MegaW.Range, Color.Green);
                }
                if (SKOMenu.Item("DrawE").GetValue<bool>())
                {
                    Drawing.DrawCircle(player.Position, MegaE.Range, Color.Green);
                }
                if (SKOMenu.Item("DrawR").GetValue<bool>())
                {
                    Drawing.DrawCircle(player.Position, R.Range, Color.Green);
                }
            }
        }

        private static void Combo(Obj_AI_Hero target)
        {
            if (SKOMenu.Item("UseQ").GetValue<bool>())
            {
                CastQ(target);
            }
            if (SKOMenu.Item("UseW").GetValue<bool>())
            {
                CastW(target);
            }
            if (SKOMenu.Item("UseE").GetValue<bool>())
            {
                CastE(target);
            }
            if (SKOMenu.Item("UseR").GetValue<bool>())
            {
                CastR(target);
            }
            if (SKOMenu.Item("UseItemsCombo").GetValue<bool>())
            {
                UseItems(target);
            }
        }

        private static void Harass(Obj_AI_Hero target)
        {
            if (SKOMenu.Item("UseQH").GetValue<bool>())
            {
                CastQ(target);
            }
            if (SKOMenu.Item("UseWH").GetValue<bool>())
            {
                CastW(target);
            }
            if (SKOMenu.Item("UseItemsHarass").GetValue<bool>())
            {
                UseItems(target);
            }
        }

        private static void KillSteal(Obj_AI_Hero target)
        {
            var igniteDmg = player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
            var qDmg = player.GetSpellDamage(target, SpellSlot.Q);
            var wDmg = player.GetSpellDamage(target, SpellSlot.W);

            if (target.IsValidTarget(600f))
            {
                if (SKOMenu.Item("Foguinho").GetValue<bool>() && IgniteSlot != SpellSlot.Unknown &&
                    player.Spellbook.CanUseSpell(IgniteSlot) == SpellState.Ready)
                {
                    if (target.Health < igniteDmg)
                    {
                        player.Spellbook.CastSpell(IgniteSlot, target);
                    }
                }
            }

            if (SKOMenu.Item("UseQKs").GetValue<bool>() && target.Health <= qDmg)
            {
                CastQ(target);
            }
            if (SKOMenu.Item("UseWH").GetValue<bool>() && target.Health <= wDmg)
            {
                CastW(target);
            }

        }

        private static void UseItems(Obj_AI_Hero target)
        {
            if (player.Distance(target) < player.AttackRange + player.BoundingRadius)
            {
                TMT.Cast();
                HYD.Cast();
            }
            BWC.Cast(target);
            BRK.Cast(target);
            RO.Cast(target);
            YMG.Cast();
        }
        private static void Clear()
        {
            var allminions = MinionManager.GetMinions(player.ServerPosition, MiniQ.Range, MinionTypes.All,
                MinionTeam.NotAlly, MinionOrderTypes.MaxHealth);
            

            foreach (var minions in allminions)
            {
                var qDmg = player.GetSpellDamage(minions, SpellSlot.Q);
				var qpredmin = MiniQ.GetPrediction(minions);
				var qpredmeg = MegaQ.GetPrediction(minions);

				if (minions.IsValidTarget(MegaQ.Range) && MegaGnar)
                {
                    if (SKOMenu.Item("UseQC").GetValue<bool>() && MegaQ.IsReady() && player.Distance(minions) <= MegaQ.Range)
                    {
							MegaQ.Cast(minions, PacketCast);
                        
                    }
                    if (SKOMenu.Item("UseWC").GetValue<bool>() && MegaW.IsReady() && player.Distance(minions) <= MegaW.Range)
                    {
                        MegaW.Cast(minions, PacketCast);
                    }
                }
                else if (minions.IsValidTarget(MiniQ.Range) && !MegaGnar)
                {
                    if (SKOMenu.Item("UseQC").GetValue<bool>() && MiniQ.IsReady() && player.Distance(minions) <= MiniQ.Range)
                    {

							MegaQ.Cast(minions, PacketCast);
                    }
                }
                if (SKOMenu.Item("UseItemsClear").GetValue<bool>())
                {
                    if (player.Distance(minions) < player.AttackRange + player.BoundingRadius)
                    {
                        TMT.Cast();
                        HYD.Cast();
                    }
                    YMG.Cast();
                }
            }
        }


        private static void CastQ(Obj_AI_Hero target)
        {
            if ((MegaGnar && !MegaQ.IsReady()) || (!MegaGnar && !MiniQ.IsReady()) || !target.IsValidTarget(MiniQ.Range)) return;

            if (MegaGnar)
            {
                var megaQpred = MegaQ.GetPrediction(target);
                switch (megaQpred.Hitchance)
                {
                    case HitChance.Collision:
                    {
                        var coll =
                            megaQpred.CollisionObjects.OrderBy(unit => unit.Distance(player.ServerPosition)).First();

                        if (coll.Distance(target) < 40)
                        {
                            MegaQ.Cast(megaQpred.CastPosition, PacketCast);
                        }
                    }
                        break;
                    default:
                        if (megaQpred.Hitchance >= HitChance.High)
                        {
                            MegaQ.Cast(megaQpred.CastPosition, PacketCast);
                        }
                        break;
                }
            }
            else if (!MegaGnar)
            {
                var miniQpred = MiniQ.GetPrediction(target);
                switch (miniQpred.Hitchance)
                {
                    case HitChance.Collision:
                    {
                        var coll =
                            miniQpred.CollisionObjects.OrderBy(unit => unit.Distance(player.ServerPosition))
                                .First();
                        if (coll.Distance(target) < 180)
                        {
                            MiniQ.Cast(miniQpred.CastPosition, PacketCast);
                        }
                    }
                        break;
                    default:
                        if (miniQpred.Hitchance >= HitChance.High)
                        {
                            MiniQ.Cast(miniQpred.CastPosition, PacketCast);
                        }
                        break;
                }
            }
        }

        private static void CastW(Obj_AI_Hero target)
        {
            if (!MegaW.IsReady() || !target.IsValidTarget(MegaW.Range)) return;

            var wpred = MegaW.GetPrediction(target);

            if (wpred.Hitchance >= HitChance.High && player.Distance(target) <= MegaW.Range)
            {
                MegaW.Cast(wpred.CastPosition, PacketCast);
            }
        }

        private static void CastE(Obj_AI_Hero target)
        {
			if(!SKOMenu.Item("UseEunder").GetValue<bool>() && target.UnderTurret(true))return;
            if ((MegaGnar && !MegaE.IsReady()) || (!MegaGnar && !MiniE.IsReady()) || !target.IsValidTarget(MiniE.Range)) return;

            var megaEpred = MegaE.GetPrediction(target);
            var miniEpred = MiniE.GetPrediction(target);

            if (MegaGnar && megaEpred.Hitchance >= HitChance.High && player.Distance(target) <= MegaE.Range)
            {
                MegaE.Cast(megaEpred.CastPosition, PacketCast);
            }
            else if (!MegaGnar && miniEpred.Hitchance >= HitChance.High && player.Distance(target) <= MiniE.Range)
            {
                MiniE.Cast(miniEpred.CastPosition, PacketCast);
            }
        }

        private static void CastR(Obj_AI_Hero target)
        {
            if (!R.IsReady() || !target.IsValidTarget(R.Range)) return;


            foreach (
                var rcoll in ObjectManager.Get<Obj_AI_Hero>()
				.Where(unit => unit.IsEnemy && unit.IsValidTarget(R.Range)))
            {
                /*Logic by LXMedia
                var enemycenter = rcoll.Position;
                var playercenter = player.Position;

                const int points = 36;
                const int radius = 300;
                

                const double slice = 2 * Math.PI / points;
                for (var i = 0; i < points; i++)
                {
                    var angle = slice * i;
                    var newPX = (int)(playercenter.X + radius * Math.Cos(angle));
                    var newPY = (int)(playercenter.Y + radius * Math.Sin(angle));
                    var newEX = (int)(enemycenter.X + radius * Math.Cos(angle));
                    var newEY = (int)(enemycenter.Y + radius * Math.Sin(angle));

                    var pcoll = new Vector3(newEX, newEY, 0);
                    var p = new Vector3(newPX, newPY, 0);

                    var collisionId = 0;
                    if (NavMesh.GetCollisionFlags(pcoll) == CollisionFlags.Wall ||
                        NavMesh.GetCollisionFlags(pcoll) == CollisionFlags.Building)
                    {
                        collisionId = i;
                    }

                    if (collisionId == i)
                        R.Cast(p, PacketCast);
                }*/
				CastRToCollision(GetCollision(target));

            }
                    
        }
		//Logic by LXMedia
		private static void CastRToCollision(int collisionId)
		{
			if (collisionId == -1)
				return;
			var center = player.Position ;
			const int points = 36;
			const int radius = 300;

			const double slice = 2 * Math.PI / points;
			for(var i = 0; i < points; i++)
			{
				var angle = slice * i;
				var newX = (int)(center.X + radius * Math.Cos(angle));
				var newY = (int)(center.Y + radius * Math.Sin(angle));
				var p = new Vector3(newX, newY, 0);
				if (collisionId == i)
					R.Cast(p, PacketCast);
			}
		}

		private static int GetCollision(Obj_AI_Hero enemy)
		{
			var center = enemy.Position;
			const int points = 36;
			const int radius = 300;
			var positionList = new List<Vector3>();

			const double slice = 2 * Math.PI / points;
			for(var i = 0; i < points; i++)
			{
				var angle = slice * i;
				var newX = (int)(center.X + radius * Math.Cos(angle));
				var newY = (int)(center.Y + radius * Math.Sin(angle));
				var p = new Vector3(newX, newY, 0);

				if (NavMesh.GetCollisionFlags(p) == CollisionFlags.Wall || NavMesh.GetCollisionFlags(p) == CollisionFlags.Building)
					return i;
			}
			return -1;
		}

    }
}

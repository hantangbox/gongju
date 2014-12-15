using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

//JaxRelentlessAssaultAS -- When jax is auto attacking
//JaxEvasion -- When Jax is casting E
//EmpoweredTwo --When Jax presses W
//MasterySpellWeaving -- When Jax is using W on target
//ItemPhageSpeed
namespace TAC_Jax
{
    class EventHandler
    {
        internal static void AntiGapCloser(ActiveGapcloser gapcloser)
        {
            if (ObjectManager.Player.Distance(gapcloser.Sender) < MenuHandler.Config.Item("gapcloseRange_E").GetValue<Slider>().Value && MenuHandler.Config.Item("gapclose_E").GetValue<bool>())
                SkillHandler.E.Cast(Jax.PacketCast);
        }
        internal static void OnInterrupt(Obj_AI_Base unit, InterruptableSpell spell)
        {
            if (!MenuHandler.Config.Item("interruptE").GetValue<bool>() 
                || ObjectManager.Player.IsDead
                    || !GameHandler.IsCastingE || SkillHandler.E.IsReady()) return;
            if (!GameHandler.IsCastingE || !SkillHandler.E.IsReady()) return;
            float distance = ObjectManager.Player.Distance(unit);
            if (SkillHandler.Q.IsReady() && distance < SkillHandler.Q.Range && distance > SkillHandler.E.Range)
                SkillHandler.Q.Cast(Jax.PacketCast);
            if(distance < SkillHandler.E.Range)
                SkillHandler.E.Cast(Jax.PacketCast);
        }
        
        internal static void Game_OnProcessSpell(Obj_AI_Base unit, GameObjectProcessSpellCastEventArgs spell)
        {
            String spellName = spell.SData.Name.ToLower();
            // Check on effects, so we can use E to dodge they're spell effects.
            if (unit.IsMe) return;
            if(GameSpellHandler.CanDodge(spellName) && !GameHandler.IsCastingE)
            {
                SkillHandler.E.Cast();
            }
            /*
                else if (!GameSpellHandler.canDodge(spellName))
                {
                no idea what i wanted to do here. lul
                }*/
        }
        internal static bool CanDieFromLeaping(Obj_AI_Hero target)
        {
            return target.Health < (ObjectManager.Player.GetSpellDamage(target, SpellSlot.Q) + ObjectManager.Player.GetSpellDamage(target, SpellSlot.W) + GameHandler.GetSheenDamage(target,true));
        }
        internal static void OnHarass()
        {
            Obj_AI_Hero target = SimpleTs.GetTarget(SkillHandler.Q.Range, SimpleTs.DamageType.Physical);
            if (target == null) return;
            if(SkillHandler.Q.IsReady() && SkillHandler.W.IsReady())
            {
                if (ObjectManager.Player.Level >= 6 && GameHandler.BuffCount > 0 && GameHandler.BuffCount % 3 == 0)
                {
                    if(SkillHandler.E.IsReady())
                        SkillHandler.E.Cast(Jax.PacketCast);
                    SkillHandler.Q.Cast(target,Jax.PacketCast);
                }
                else
                {
                    if(SkillHandler.E.IsReady())
                        SkillHandler.E.Cast(Jax.PacketCast);
                    SkillHandler.Q.Cast(Jax.PacketCast);
                }
            }
            if (GameHandler.IsCastingE && SkillHandler.E.IsReady())
                SkillHandler.E.Cast(Jax.PacketCast);
        }

        internal static void Orbwalking_AfterAttack(Obj_AI_Base unit, Obj_AI_Base target)
        {
            /* Force sheen if we have it activated and only in combo mode
             * Check if W is available
             * Check if we are in range with the target. */
            if ((!GameHandler.HasSheenActive 
                    && MenuHandler.Config.Item("force_sheen").GetValue<bool>() 
                        && GameHandler.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo) 
                || !SkillHandler.W.IsReady() ||
                    !target.IsValidTarget(Orbwalking.GetRealAutoAttackRange(ObjectManager.Player) + 50)) return;
            switch (GameHandler.Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                case Orbwalking.OrbwalkingMode.Mixed:
                    SkillHandler.W.Cast(Jax.PacketCast);
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    if (MenuHandler.Config.Item("clear_w").GetValue<bool>()) SkillHandler.W.Cast(Jax.PacketCast);
                    break;
            }
        }

        internal static void OnCombo()
        {
            Obj_AI_Hero target = SimpleTs.GetTarget(SkillHandler.Q.Range,SimpleTs.DamageType.Physical);
            if(target != null)
            {
                if(SkillHandler.E.IsReady() && SkillHandler.Q.IsReady() && ObjectManager.Player.Distance(target) < SkillHandler.Q.Range)
                {
                    SkillHandler.Q.Cast(target, Jax.PacketCast);
                    SkillHandler.E.Cast(Jax.PacketCast);
                }
                if ((ObjectManager.Player.Distance(target) < SkillHandler.E.Range && !GameHandler.IsCastingE && SkillHandler.E.IsReady())
                        || (GameHandler.IsCastingE && SkillHandler.E.IsReady() && ObjectManager.Player.Distance(target) < (SkillHandler.E.Range - 30f)))
                {
                    SkillHandler.E.Cast(Jax.PacketCast);
                }

                // if we are out of distance we need to cast our Q to get near him.
                // if he is not valid target in our range we have to cast q to get near him.
                if(!target.IsValidTarget(SkillHandler.E.Range) && GameHandler.IsCastingE)
                {
                    SkillHandler.Q.Cast(target,Jax.PacketCast);
                }

                if (MenuHandler.Config.Item("botrk_"+target.BaseSkinName).GetValue<bool>() 
                    && (Items.HasItem(3144) || Items.HasItem(3153)) 
                        && target.ServerPosition.Distance(ObjectManager.Player.ServerPosition) <= 450)
                {
                    double damage = ObjectManager.Player.GetItemDamage(target, Damage.DamageItems.Botrk);
                    if ((Items.HasItem(3144) || Items.HasItem(3153)) && ObjectManager.Player.Health + damage < ObjectManager.Player.MaxHealth)
                        Items.UseItem(Items.HasItem(3144) ? 3144 : 3153, target);
                }

                if (SkillHandler.IgniteSlot != SpellSlot.Unknown && SkillHandler.Ignite.IsReady()
                    && ObjectManager.Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite) > target.Health && ObjectManager.Player.Distance(target) <= 500)
                {
                    SkillHandler.Ignite.Cast(target, Jax.PacketCast);
                }

                /* Check if player Health is below W damage,
                 * so then we can use it without auto-attacking
                 * or else it's useless to waste W + auto-attack,
                 * since W is a Auto-Attack reset.
                 * Also check if I just used Q and auto-attacked 
                 * for biggest damage possible
                 */
                if (CanDieFromLeaping(target) && SkillHandler.Q.IsReady() && SkillHandler.W.IsReady())
                {
                    SkillHandler.W.Cast(Jax.PacketCast);
                    SkillHandler.Q.Cast(target, Jax.PacketCast);
                }
                // Check if our flash and q is ready and he can die from leaping onto the target
                if(SkillHandler.Q.IsReady() && SkillHandler.W.IsReady() && CanDieFromLeaping(target))
                {
                    if (MenuHandler.Config.Item("acQ_useIfWorth").GetValue<bool>()
                            && (target.ChampionsKilled > target.Deaths || target.ChampionsKilled > ObjectManager.Player.ChampionsKilled)
                                && target.CountEnemysInRange((int)(SkillHandler.Q.Range + SkillHandler.Flash.Range)) < MenuHandler.Config.Item("acQ_useIfWorthEnemy").GetValue<Slider>().Value)
                    {
                        if (SkillHandler.Flash.IsReady() && ObjectManager.Player.Distance(target) > SkillHandler.Q.Range)
                            SkillHandler.Flash.Cast(target.Position,Jax.PacketCast);
                        SkillHandler.W.Cast(Jax.PacketCast);
                        SkillHandler.Q.Cast(target.Position, Jax.PacketCast);
                    }
                }
            }
        }
        internal static void Orbwalking_BeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            var buff = ObjectManager.Player.Buffs.FirstOrDefault(b => b.DisplayName == "JaxRelentlessAssaultAS");
            if (buff != null)
            {
                var dBuffBro = buff.Count;
                if (dBuffBro > 0)
                {
                    GameHandler.BuffCount++;
                    if (GameHandler.BuffCount != dBuffBro && dBuffBro < 6 && dBuffBro > 1)
                        GameHandler.BuffCount = dBuffBro;
                    GameHandler.LastTick = Environment.TickCount;
                    if (Jax.Debug)
                        Game.PrintChat("(" + GameHandler.BuffCount + ") Buff: JaxRelentlessAssaultAS Count: " + dBuffBro);
                    GameHandler.HasResetBuffCount = false;
                }
            }
        }
        internal static void OnLaneClear()
        {
            foreach (var minions in ObjectManager.Get<Obj_AI_Minion>().Where(minion => minion.IsValidTarget(SkillHandler.Q.Range)).OrderBy(minion => minion.Health))
            {
                if (MenuHandler.Config.Item("clear_e").GetValue<bool>() && SkillHandler.E.IsReady() && minions.IsValidTarget(SkillHandler.E.Range) && !GameHandler.IsCastingE)
                    SkillHandler.E.Cast(Jax.PacketCast);
            }
        }
        internal static void SmartR()
        {
            if (((ObjectManager.Player.CountEnemysInRange(550) > 3 || !(ObjectManager.Player.HealthPercentage() <=
                                                                        MenuHandler.Config.Item("useR_under")
                                                                            .GetValue<Slider>()
                                                                            .Value)) &&
                 (ObjectManager.Player.CountEnemysInRange(550) >
                  MenuHandler.Config.Item("useR_when").GetValue<Slider>().Value)) ||
                !MenuHandler.Config.Item("useR").GetValue<bool>()) return;
            switch (GameHandler.Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    if (MenuHandler.Config.Item("useR_combo").GetValue<bool>())
                        SkillHandler.R.Cast(Jax.PacketCast);
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    if (MenuHandler.Config.Item("useR_mixed").GetValue<bool>())
                        SkillHandler.R.Cast(Jax.PacketCast);
                    break;
                case Orbwalking.OrbwalkingMode.None:
                    if (MenuHandler.Config.Item("useR_flee").GetValue<bool>() && MenuHandler.Config.Item("Flee").GetValue<bool>())
                        SkillHandler.R.Cast(Jax.PacketCast);
                    break;
            }
        }

        internal static void KillSteal()
        {
            if (SkillHandler.W.IsReady() && SkillHandler.Q.IsReady())
            {
                foreach (var target
                        in ObjectManager.Get<Obj_AI_Hero>().Where(hero => !hero.IsDead && hero.IsEnemy && hero.IsValidTarget(SkillHandler.Q.Range)
                            && SkillHandler.Q.GetHealthPrediction(hero) <= SkillHandler.Q.GetDamage(hero) + SkillHandler.W.GetDamage(hero)).OrderBy(i => i.Health).ThenByDescending(i => i.Distance3D(ObjectManager.Player)))
                {
                    if (SkillHandler.W.IsReady()) SkillHandler.W.Cast(Jax.PacketCast);
                    if (SkillHandler.Q.IsReady()) SkillHandler.Q.Cast(target, Jax.PacketCast);
                }
            }
        }

        /**
         * @author xSalice
         * Taken from xSalice xKittyKiller assembly
         */

        internal static void WardJump()
        {
            foreach (Obj_AI_Minion ward in ObjectManager.Get<Obj_AI_Minion>().Where(ward =>
                ward.Name.ToLower().Contains("ward") && ward.Distance(Game.CursorPos) < 250))
            {
                if (SkillHandler.Q.IsReady())
                {
                    SkillHandler.Q.CastOnUnit(ward,Jax.PacketCast);
                    return;
                }
            }

            foreach (
                Obj_AI_Hero hero in ObjectManager.Get<Obj_AI_Hero>().Where(hero => hero.Distance(Game.CursorPos) < 250 && !hero.IsDead))
            {
                if (SkillHandler.Q.IsReady())
                {
                    SkillHandler.Q.CastOnUnit(hero, Jax.PacketCast);
                    return;
                }
            }

            foreach (Obj_AI_Minion minion in ObjectManager.Get<Obj_AI_Minion>().Where(minion =>
                minion.Distance(Game.CursorPos) < 250))
            {
                if (SkillHandler.Q.IsReady())
                {
                    SkillHandler.Q.CastOnUnit(minion, Jax.PacketCast);
                    return;
                }
            }

            if (Environment.TickCount <= GameHandler.LastPlaced + 3000 || !SkillHandler.Q.IsReady()) return;

            Vector3 cursorPos = Game.CursorPos;
            Vector3 myPos = ObjectManager.Player.Position;

            Vector3 delta = cursorPos - myPos;
            delta.Normalize();

            Vector3 wardPosition = myPos + delta * (600 - 5);

            InventorySlot invSlot = FindBestWardItem();
            if (invSlot == null) return;

            Items.UseItem((int)invSlot.Id, wardPosition);
            GameHandler.LastWardPos = wardPosition;
            GameHandler.LastPlaced = Environment.TickCount;
        }

        private static InventorySlot FindBestWardItem()
        {
            InventorySlot slot = Items.GetWardSlot();
            if (slot == default(InventorySlot)) return null;
            return slot;
        }
    }
}
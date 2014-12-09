using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace TAC_Kalista
{
    class MathHandler
    {
        public static float getDamageToTarget(Obj_AI_Base target)
        {
            double damage = 0;//ObjectManager.Player.GetAutoAttackDamage(target);
            if (SkillHandler.Q.IsReady() && ObjectManager.Player.Distance(target) < SkillHandler.Q.Range)
                damage += ObjectManager.Player.GetSpellDamage(target, SpellSlot.Q);
            if (SkillHandler.E.IsReady() && ObjectManager.Player.Distance(target) < SkillHandler.E.Range)
                damage += getRealDamage(target);
            return (float)damage;
        }
        internal static void castMinionE(Obj_AI_Base target)
        {
            if (ObjectManager.Get<Obj_AI_Hero>().Any(
                        hero => hero.IsValidTarget(SkillHandler.E.Range)
                            &&
                                hero.Buffs.FirstOrDefault(b => b.Name.ToLower() == "kalistaexpungemarker").Count >= 1
                            ))
            {
                List<Obj_AI_Base> minions = MinionManager.GetMinions(ObjectManager.Player.Position, SkillHandler.E.Range,MinionTypes.All,MinionTeam.Enemy,MinionOrderTypes.Health);
                foreach (var minion in minions)
                {
                    if (MathHandler.getRealDamage(minion) > minion.Health)
                    {
                        SkillHandler.E.Cast(Kalista.packetCast);
                        break;
                    }
                }
            }        
        }

        public static float GetPlayerHealthPercentage()
        {
            return ObjectManager.Player.Health * 100 / ObjectManager.Player.MaxHealth;
        }
        public static float GetPlayerManaPercentage()
        {
            return ObjectManager.Player.Mana * 100 / ObjectManager.Player.MaxMana;
        }
        internal static Obj_AI_Hero player = ObjectManager.Player;
        /**
         * @author Hellsing
         * Everything below is created by hellsing not me.
         * */
        public static double getRealDamage(Obj_AI_Base target, int customStacks = -1)
        {
            return player.CalcDamage(target, Damage.DamageType.Physical, GetRawRendDamage(target, customStacks));
        }
        internal static double GetRawRendDamage(Obj_AI_Base target, int customStacks = -1)
        {
            // Get buff
            var buff = target.Buffs.FirstOrDefault(b => b.DisplayName == "KalistaExpungeMarker" && b.SourceName == player.ChampionName);

            if (buff != null || customStacks != -1)
            {
                // Base damage
                double damage = (10 + 10 * player.Spellbook.GetSpell(SpellSlot.E).Level) + 0.6 * (player.BaseAttackDamage + player.FlatPhysicalDamageMod);

                // Damage per spear
                double singleSpearDamage = damage * new double[] { 0, 0.25, 0.30, 0.35, 0.40, 0.45 }[player.Spellbook.GetSpell(SpellSlot.E).Level];
                damage += (((customStacks == -1 ? buff.Count : customStacks) - 1) * singleSpearDamage);

                // Calculate the damage and return
                return damage;
            }

            return 0;
        }
    }
}

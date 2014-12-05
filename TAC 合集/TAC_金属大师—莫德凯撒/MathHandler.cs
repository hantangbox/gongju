using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;


namespace TAC_Mordekaiser
{
    class MathHandler
    {
        internal static float getTotalDamageToTarget(Obj_AI_Hero target)
        {
            float damage = 0f;
            if (ItemHandler.Item.IsReady()) damage += (float)ObjectManager.Player.GetItemDamage(target, Damage.DamageItems.Dfg) / 1.2f;
            if (ItemHandler.Hex.IsReady()) damage += (float)ObjectManager.Player.GetItemDamage(target, Damage.DamageItems.Botrk);
            if (SkillHandler.Q.IsReady() && ObjectManager.Player.Distance(target) < SkillHandler.Q.Range) damage += calculateQdamage(target);
            if (SkillHandler.W.IsReady() && ObjectManager.Player.Distance(target) < Orbwalking.GetRealAutoAttackRange(ObjectManager.Player)) damage += calculateWdamage(target);
            if (SkillHandler.E.IsReady() && ObjectManager.Player.Distance(target) < SkillHandler.E.Range) damage += calculateEdamage(target);
            if (SkillHandler.R.IsReady() && ObjectManager.Player.Distance(target) < SkillHandler.R.Range) damage += calculateRdamage(target);
            if (SkillHandler.IgniteSlot != SpellSlot.Unknown && ObjectManager.Player.SummonerSpellbook.CanUseSpell(SkillHandler.IgniteSlot) == SpellState.Ready) 
                damage += (float)ObjectManager.Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);

            return damage;
        }
        internal static float calculateQdamage(Obj_AI_Base target)
        {
            float damage = 0f;
            if (ObjectManager.Player.CountEnemysInRange((int)SkillHandler.Q.Range) < 2)
                damage = (float)new double[] { 0, 132, 181.5, 231, 280.5, 330 }[SkillHandler.E.Level] + 0.66f * ObjectManager.Player.FlatMagicDamageMod + 1.65f * ObjectManager.Player.FlatPhysicalDamageMod;
            else
                damage = new float[] { 0, 80, 110, 140, 170, 200 }[SkillHandler.Q.Level] + 0.4f * ObjectManager.Player.FlatMagicDamageMod + ObjectManager.Player.FlatPhysicalDamageMod;

            return (float)ObjectManager.Player.CalcDamage(target, Damage.DamageType.Magical, damage);
        }

        internal static float calculateWdamage(Obj_AI_Base target)
        {
            return (float)ObjectManager.Player.CalcDamage(target,Damage.DamageType.Magical, new float[] { 0, 24, 38, 52, 66, 80 }[SkillHandler.W.Level] + 0.2f * ObjectManager.Player.FlatMagicDamageMod);
        }

        internal static float calculateEdamage(Obj_AI_Base target)
        {
            return (float)ObjectManager.Player.CalcDamage(target,Damage.DamageType.Magical,new float[] { 0, 70, 115, 160, 205, 250 } [SkillHandler.E.Level] + 0.6f * ObjectManager.Player.FlatMagicDamageMod);
        }
        internal static float calculateRdamage(Obj_AI_Hero target)
        {
            float damage = new float[] { 0, 24, 29, 34 }[SkillHandler.R.Level];
            int apPer100 = (int) (ObjectManager.Player.FlatMagicDamageMod / 100f);
            return (float)ObjectManager.Player.CalcDamage(target,Damage.DamageType.Magical,(damage+(apPer100 >= 1 ? apPer100*4 : 0))*target.MaxHealth);
        }
    }
}

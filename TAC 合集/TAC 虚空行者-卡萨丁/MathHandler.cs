using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace TAC_Kassadin
{
    class MathHandler
    {
        internal static float getComboDamage(Obj_AI_Base target)
        {

            var damage = 0d;
            if (SkillHandler.R.IsReady()) damage += ObjectManager.Player.GetSpellDamage(target, SpellSlot.R);
            if (MenuHandler.menu.Item("useDFG").GetValue<bool>() && AutoCarryHandler.item.IsReady()) damage += ObjectManager.Player.GetItemDamage(target, Damage.DamageItems.Dfg) / 1.2;
            if (SkillHandler.W.IsReady()) damage += ObjectManager.Player.GetSpellDamage(target, SpellSlot.W);
            if (SkillHandler.Q.IsReady()) damage += ObjectManager.Player.GetSpellDamage(target, SpellSlot.Q);
            if (SkillHandler.E.IsReady()) damage += ObjectManager.Player.GetSpellDamage(target, SpellSlot.E);
            if (SkillHandler.IgniteSlot != SpellSlot.Unknown && ObjectManager.Player.SummonerSpellbook.CanUseSpell(SkillHandler.IgniteSlot) == SpellState.Ready)
                damage += ObjectManager.Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
            return (float)damage * (AutoCarryHandler.item.IsReady() ? 1.2f : 1);
        }
        internal static int buffCount(int witch)
        {
            var buffs = ObjectManager.Player.Buffs;
            foreach (var b in buffs)
            {
                if (witch == 1 && b.Name.ToLower() == "riftwalk")
                    return b.Count;
                else if (witch == 2 && b.Name.ToLower() == "forcepulsecounter")
                    return b.Count;
            }
            return 0;
        }
        internal static float calculateShield(Obj_AI_Base target)
        {
            return new float[] { 0, 40, 70, 100, 130, 160 }[SkillHandler.Q.Level] + 0.3f*ObjectManager.Player.FlatMagicDamageMod;
        }
        internal static void castR(Obj_AI_Base target)
        {
            PredictionOutput po = SkillHandler.R.GetPrediction(target);
            if (po.Hitchance >= HitChance.Low && ObjectManager.Player.Distance(po.UnitPosition) < (SkillHandler.R.Range+SkillHandler.W.Range))
            {
                SkillHandler.R.Cast(po.CastPosition, Program.packetCast);
            }
        }
        internal static void castE(Obj_AI_Base target)
        {
            PredictionOutput po = SkillHandler.E.GetPrediction(target);
            if (po.Hitchance >= HitChance.Low && ObjectManager.Player.Distance(po.UnitPosition) < SkillHandler.E.Range)
            {
                SkillHandler.E.Cast(po.CastPosition, Program.packetCast);
            }
        }
    }
}

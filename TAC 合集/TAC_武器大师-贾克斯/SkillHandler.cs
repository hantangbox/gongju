using LeagueSharp;
using LeagueSharp.Common;

namespace TAC_Jax
{
    class SkillHandler
    {
        internal static Spell Q, W, E, R, Flash, Ignite;
        internal static SpellSlot IgniteSlot, SmiteSlot;
        
        internal static void Load()
        {
            Q = new Spell(SpellSlot.Q, 700f);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E, 210f);
            R = new Spell(SpellSlot.R);
            Q.SetTargetted(-0.5f, 0);
            Flash = new Spell(ObjectManager.Player.GetSpellSlot("SummonerFlash"),500f);
            Ignite = new Spell(ObjectManager.Player.GetSpellSlot("SummonerDot"), 600f);

            IgniteSlot = ObjectManager.Player.GetSpellSlot("SummonerDot");
            SmiteSlot = ObjectManager.Player.GetSpellSlot("SummonerSmite");
        }
    }
}

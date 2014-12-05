using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace TAC_Mordekaiser
{
    class AutoCarryHandler
    {
        public static float SlaveDelay = 0;

        internal static void onCombo()
        {
            Obj_AI_Hero target = SimpleTs.GetTarget(SkillHandler.E.Range, SimpleTs.DamageType.Magical);
            float distance = ObjectManager.Player.Distance(target);
            if (MenuHandler.Config.Item("no" + target.BaseSkinName).GetValue<bool>() && !cloneReady() && SkillHandler.R.IsReady() && MathHandler.getTotalDamageToTarget(target) > target.Health && distance < ItemHandler.Item.Range)
            {
                ItemHandler.Item.Cast(target);
                ItemHandler.castHex(target);
                SkillHandler.R.Cast(target, Program.packetCast);
            }
            if (SkillHandler.W.IsReady()) SkillHandler.W.Cast(ObjectManager.Player, Program.packetCast);
            if (SkillHandler.Q.IsReady())
            {
                SkillHandler.Q.Cast(Program.packetCast);
            }
            if(cloneReady() && Environment.TickCount >= SlaveDelay)
            {
                SkillHandler.R.Cast(SimpleTs.GetTarget(1125f, SimpleTs.DamageType.Magical), Program.packetCast);
                SlaveDelay = Environment.TickCount + 1000;
            }
            if(distance < SkillHandler.E.Range && SkillHandler.E.IsReady())
            {
                SkillHandler.E.Cast(target.Position, Program.packetCast);
            }
            if (MenuHandler.Config.Item("no" + target.BaseSkinName).GetValue<bool>() && !cloneReady() && SkillHandler.R.IsReady() && MathHandler.getTotalDamageToTarget(target) > target.Health && distance < ItemHandler.Item.Range)
            {
                ItemHandler.castIgnite(target);
                SkillHandler.R.Cast(target, Program.packetCast);
            }
        }

        internal static void Mixed()
        {
            Obj_AI_Hero target = SimpleTs.GetTarget(SkillHandler.E.Range, SimpleTs.DamageType.Magical);
            float distance = ObjectManager.Player.Distance(target);

            if (MenuHandler.Config.Item("mxE").GetValue<bool>() && distance < SkillHandler.E.Range && SkillHandler.E.IsReady())
            {
                SkillHandler.E.Cast(target.Position, Program.packetCast);
            }
            if (MenuHandler.Config.Item("mxQ").GetValue<bool>() && distance < SkillHandler.Q.Range && SkillHandler.Q.IsReady())
            {
                SkillHandler.Q.Cast(Program.packetCast);
            }
            if (MenuHandler.Config.Item("mxW").GetValue<bool>() && distance < SkillHandler.W.Range && SkillHandler.W.IsReady())
            {
                SkillHandler.W.Cast(ObjectManager.Player, Program.packetCast);
            }
        }
        /*
         * @author xQx
         * */
        internal static void LaneClear()
        {
            if (!MenuHandler.Config.Item("laneActive").GetValue<bool>()) return;
            if (MenuHandler.Config.Item("lcQ").GetValue<bool>() && SkillHandler.Q.IsReady())
            {
                var minionsQ = MinionManager.GetMinions(ObjectManager.Player.ServerPosition,
                Orbwalking.GetRealAutoAttackRange(ObjectManager.Player), MinionTypes.All, MinionTeam.NotAlly);
                foreach (var vMinion in from vMinion in minionsQ
                                        let vMinionEDamage = ObjectManager.Player.GetSpellDamage(vMinion, SpellSlot.Q)
                                        select vMinion)
                {
                    SkillHandler.Q.Cast(vMinion);
                }
            }
            if (MenuHandler.Config.Item("lcW").GetValue<bool>() && SkillHandler.W.IsReady())
            {
                var rangedMinionsW = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, SkillHandler.W.Range);
                var minionsW = SkillHandler.W.GetCircularFarmLocation(rangedMinionsW, SkillHandler.W.Range * 0.3f);
                if (minionsW.MinionsHit < 1 || !SkillHandler.W.InRange(minionsW.Position.To3D()))
                    return;
                SkillHandler.W.CastOnUnit(ObjectManager.Player);
            }
            if (MenuHandler.Config.Item("lcE").GetValue<bool>() && SkillHandler.E.IsReady())
            {
                var rangedMinionsE = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, SkillHandler.E.Range);
                var minionsE = SkillHandler.E.GetCircularFarmLocation(rangedMinionsE, SkillHandler.E.Range);
                if (minionsE.MinionsHit < 1 || !SkillHandler.E.InRange(minionsE.Position.To3D()))
                    return;
                SkillHandler.E.Cast(minionsE.Position);
            }
        }
        internal static bool cloneReady()
        {
            return ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).Name == "mordekaisercotgguide" ? true : false;
        }

        internal static void onProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsEnemy && (sender.Type == GameObjectType.obj_AI_Hero || sender.Type == GameObjectType.obj_AI_Turret) && MenuHandler.Config.Item("shieldself").GetValue<bool>())
            {
                if (SpellData.SpellName.Any(Each => Each.Contains(args.SData.Name)) || (args.Target == ObjectManager.Player && ObjectManager.Player.Distance(sender) <= 700))
                {
                    if (SkillHandler.W.IsReady()) SkillHandler.W.Cast(ObjectManager.Player, Program.packetCast);
                }
            }
        }
        internal static void AntiGapCloser(ActiveGapcloser gapcloser)
        {
            if (MenuHandler.Config.Item("gap").GetValue<bool>())
            {
                if (SkillHandler.W.IsReady() && gapcloser.Sender.IsValidTarget(SkillHandler.W.Range)) SkillHandler.W.CastOnUnit(gapcloser.Sender, Program.packetCast);
                if (SkillHandler.E.IsReady() && gapcloser.Sender.IsValidTarget(SkillHandler.E.Range)) SkillHandler.E.CastOnUnit(gapcloser.Sender, Program.packetCast);
            }
        }
    }
}

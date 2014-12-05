using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using Color = System.Drawing.Color;

namespace TAC_Mordekaiser
{
    class DrawingHandler
    {
        public static HpBarIndicator hpi = new HpBarIndicator();
        internal static void load(EventArgs args)
        {
            if(Program.drawings)
            {
                //1125
                Spell[] spellList = { SkillHandler.Q, SkillHandler.W, SkillHandler.E, SkillHandler.R };

                foreach (var spell in spellList)
                {
                    var menuItem = MenuHandler.Config.Item("draw" + spell.Slot).GetValue<Circle>();
                    if (menuItem.Active && spell.Level > 0)
                        Utility.DrawCircle(ObjectManager.Player.Position, spell.Range, menuItem.Color);
                }

                if (MenuHandler.Config.Item("drawClone").GetValue<bool>()) Utility.DrawCircle(ObjectManager.Player.Position, 1125f, Color.Red);
                if (MenuHandler.Config.Item("drawFC").GetValue<bool>()) Utility.DrawCircle(ObjectManager.Player.Position, (SkillHandler.Q.Range+400f), Color.Red);

                if (MenuHandler.Config.Item("drawHp").GetValue<bool>())
                {
                    foreach (
                    var enemy in
                    ObjectManager.Get<Obj_AI_Hero>()
                    .Where(ene => !ene.IsDead && ene.IsEnemy && ene.IsVisible))
                    {
                        hpi.unit = enemy;
                        hpi.drawDmg(MathHandler.getTotalDamageToTarget(enemy), Color.Yellow);
                    }
                }
            }
        }
        /**
         * @author detuks
         * */
        internal static void OnEndScene(EventArgs args)
        {
            if (MenuHandler.Config.Item("drawHp").GetValue<bool>())
            {
                foreach (
                var enemy in
                ObjectManager.Get<Obj_AI_Hero>()
                .Where(ene => !ene.IsDead && ene.IsEnemy && ene.IsVisible))
                {
                    hpi.unit = enemy;
                    hpi.drawDmg(MathHandler.getTotalDamageToTarget(enemy), Color.Yellow);
                }
            }
        }
    }
}

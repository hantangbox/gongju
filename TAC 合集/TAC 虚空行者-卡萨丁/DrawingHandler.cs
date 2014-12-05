using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using Color = System.Drawing.Color;

namespace TAC_Kassadin
{
    class DrawingHandler
    {
        public static HpBarIndicator hpi = new HpBarIndicator();
        internal static void load(EventArgs args)
        {
            if(Program.drawings)
            {
                Spell[] spellList = { SkillHandler.Q, SkillHandler.W, SkillHandler.E, SkillHandler.R };
                foreach (var spell in spellList)
                {
                    var menuItem = MenuHandler.menu.Item(spell.Slot + "Range").GetValue<Circle>();
                    if (menuItem.Active && spell.Level > 0)
                        Utility.DrawCircle(ObjectManager.Player.Position, spell.Range, menuItem.Color);
                }
                if (MenuHandler.menu.Item("drawHp").GetValue<bool>())
                {
                    foreach (
                    var enemy in
                    ObjectManager.Get<Obj_AI_Hero>()
                    .Where(ene => !ene.IsDead && ene.IsEnemy && ene.IsVisible))
                    {
                        hpi.unit = enemy;
                        hpi.drawDmg(MathHandler.getComboDamage(enemy), Color.Yellow);
                    }
                }
            }
        }
        /**
         * @author detuks
         * */
        internal static void OnEndScene(EventArgs args)
        {
            if (MenuHandler.menu.Item("drawHp").GetValue<bool>())
            {
                foreach (
                var enemy in
                ObjectManager.Get<Obj_AI_Hero>()
                .Where(ene => !ene.IsDead && ene.IsEnemy && ene.IsVisible))
                {
                    hpi.unit = enemy;
                    hpi.drawDmg(MathHandler.getComboDamage(enemy), Color.Yellow);
                }
            }
        }
    }
}

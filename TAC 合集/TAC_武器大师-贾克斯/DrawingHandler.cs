using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using Color = System.Drawing.Color;

namespace TAC_Jax
{
    class DrawingHandler
    {
        internal static HpBarIndicator Hpi = new HpBarIndicator();
        internal static void Load(EventArgs args)
        {
            LoadSpells();
            if (MenuHandler.Config.Item("drawWard").GetValue<bool>())
                Utility.DrawCircle(Game.CursorPos, 250, Color.Purple, 8, 30);
            if (MenuHandler.Config.Item("drawHp").GetValue<bool>())
            {
                foreach (
                var enemy in
                ObjectManager.Get<Obj_AI_Hero>()
                .Where(ene => !ene.IsDead && ene.IsEnemy && ene.IsVisible))
                {
                    Hpi.Unit = enemy;
                    var damage = (float)GameHandler.ComboDamage(enemy);
                    Hpi.DrawDmg(damage, damage > enemy.Health ? Color.Red : Color.Yellow);
                }
            }
        }
        internal static void LoadSpells()
        {
            Spell[] spellList = { SkillHandler.Q, SkillHandler.E };

            foreach (var spell in spellList)
            {
                var menuItem = MenuHandler.Config.Item("range"+spell.Slot).GetValue<Circle>();
                if (menuItem.Active && spell.Level > 0)
                    Utility.DrawCircle(ObjectManager.Player.Position, spell.Range, menuItem.Color);
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
                    Hpi.Unit = enemy;
                    var damage = (float)GameHandler.ComboDamage(enemy);
                    Hpi.DrawDmg(damage, damage > enemy.Health ? Color.Red : Color.Yellow);
                }
            }
        }
    }
}

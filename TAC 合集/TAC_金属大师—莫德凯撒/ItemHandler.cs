using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace TAC_Mordekaiser
{
    class ItemHandler
    {
        internal static Items.Item Item = Utility.Map.GetMap()._MapType == Utility.Map.MapType.TwistedTreeline || Utility.Map.GetMap()._MapType == Utility.Map.MapType.CrystalScar ? new Items.Item(3188, 750) : new Items.Item(3128, 750);
        internal static Items.Item Hex = new Items.Item(3146, 750);
        internal static void castIgnite(Obj_AI_Hero target)
        {
            if(SkillHandler.IgniteSlot != SpellSlot.Unknown 
                && ObjectManager.Player.SummonerSpellbook.CanUseSpell(SkillHandler.IgniteSlot) == SpellState.Ready
                    && MenuHandler.Config.Item("useIgnite").GetValue<bool>())
            {
                ObjectManager.Player.SummonerSpellbook.CastSpell(ObjectManager.Player.GetSpellSlot("SummonerDot"), target);
            }
        }
        internal static void castHex(Obj_AI_Hero target)
        {
            if(MenuHandler.Config.Item("hg").GetValue<bool>())
            {
                Hex.Cast(target);
            }
        }
    }
}

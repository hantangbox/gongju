using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace TAC_Kassadin
{
    class SkillHandler
    {
        internal static Spell Q = new Spell(SpellSlot.Q, 650);
        internal static Spell W = new Spell(SpellSlot.W, 200);//ObjectManager.Player.AttackRange);
        internal static Spell E = new Spell(SpellSlot.E, 700);
        internal static Spell R = new Spell(SpellSlot.R, 700);
        internal static SpellSlot IgniteSlot = ObjectManager.Player.GetSpellSlot("SummonerDot");

    }
    /**
     * @author Kurisu
     * */
    public class SpellData
    {
        public static List<String> SpellName = new List<string>()
        {
            "AatroxQ", "AatroxE",
            "AhriSeduce","SummonerDot",
            "Pulverize", "Headbutt",
            "BandageToss", "CurseoftheSadMummy",
            "FlashFrost", "GlacialStorm",
            "InfernalGuardian",
            "EnchantedCrystalArrow", "Volley",
            "AzirE", "AzirR",
            "RocketGrab", "PowerFist",
            "BrandBlazeMissile", "BrandWildfireMissile", "BrandScorchAttack",
            "BraumQ", "BraumR",
            "CaitlynEntrapment",
            "CassiopeiaPetrifyingGaze",
            "Rupture",
            "DariusAxeGrabCone",
            "DianaVortex",
            "InfectedCleaverMissileCast",
            "DravenDoubleShot",
            "EliseHumanE",
            "EvelynnR",
            "FizzMarinerDoom", "FizzJump",
            "GalioIdolOfDurand", "GalioResoluteSmite",
            "GnarQ", "GnarW", "GnarR",
            "GragasR", "GragasQ", "GragasE",
            "HecarimUlt", "HecarimRamp",
            "HeimerdingerE",
            "ReapTheWhirlwind", "HowlingGale",
            "JarvanIVDragonStrike",
            "JaxCounterStrike",
            "JayceThunderingBlow",
            "JinxW", "JinxE",
            "KarmaQ", "KarmaSpiritBind",
            "KarthusWallOfPain",
            "ForcePulse",
            "JudicatorReckoning",
            "KogMawVoidOoze",
            "LeblancSoulShackle",
            "BlindMonkRKick", "BlindMonkEOne",
            "LeonaSolarFlare", "LeonaShieldOfDaybreak", "LeonaZenithBlade",
            "LissandraW", "LissandraR",
            "LuluW", "LuluQ", "LuluQMissileTwo", "LuluR",
            "LuxLightBinding", "LuxLightStrikeKugel",
            "SeismicShard", "Landslide", "UFSlash",
            "AlZaharNetherGrasp",
            "MaokaiUnstableGrowth",
            "MaokaiTrunkLine",
            "MonkeyKingSpinToWin",
            "MordekaiserChildrenOfTheGrave",
            "DarkBindingMissile",
            "NamiQ", "NamiR",
            "NasusW",
            "NautilusAnchorDrag", "NautilusSplashZone", "NautilusGrandLine",
            "Dazzle", "FeralScream", "AlZaharCalloftheVoid",
            "ZyraGraspingRoots", "FiddlesticksDarkWind", "StaticField",
            "Terrify"
        };
    }
}

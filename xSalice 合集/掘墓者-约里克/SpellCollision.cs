#region

using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;

#endregion

namespace YorickMILFDigger
{
    class SpellCollision
    {
        public string HeroName { get; set; }
        public string SpellName { get; set; }
        public string SDataName { get; set; }

        public static List<SpellCollision> Spells = new List<SpellCollision>();

        static SpellCollision()
        {
            //Add spells to the database 

            #region Ahri

            Spells.Add(
                new SpellCollision
                {
                    HeroName = "Ahri",
                    SpellName = "AhriSeduce",
                    SDataName = "AhriSeduceMissile"
                });

            #endregion Ahri

            #region Amumu

            Spells.Add(
                new SpellCollision
                {
                    HeroName = "Amumu",
                    SpellName = "BandageToss",
                    SDataName = "SadMummyBandageToss"
                });
            #endregion Amumu

            #region Ashe
  
            Spells.Add(
                new SpellCollision
                {
                    HeroName = "Ashe",
                    SpellName = "EnchantedCrystalArrow",
                    SDataName = "EnchantedCrystalArrow"
                });

            #endregion Ashe

            #region Blitzcrank

            Spells.Add(
                new SpellCollision
                {
                    HeroName = "Blitzcrank",
                    SpellName = "RocketGrab",
                    SDataName = "RocketGrabMissile"
                });

            #endregion Blitzcrank

            #region Brand

            Spells.Add(
                new SpellCollision
                {
                    HeroName = "Brand",
                    SpellName = "BrandBlaze",
                    SDataName = "BrandBlazeMissile"
                });

            #endregion Brand

            #region Braum

            Spells.Add(
                new SpellCollision
                {
                    HeroName = "Braum",
                    SpellName = "BraumQ",
                    SDataName = "BraumQMissile"
                });

            #endregion Braum

            #region Caitlyn

            Spells.Add(
                new SpellCollision
                {
                    HeroName = "Caitlyn",
                    SpellName = "CaitlynEntrapment",
                    SDataName = "CaitlynEntrapmentMissile",
                });

            #endregion Caitlyn

            #region Corki

            Spells.Add(
                new SpellCollision
                {
                    HeroName = "Corki",
                    SpellName = "MissileBarrage",
                    SDataName = "MissileBarrageMissile"
                });

            Spells.Add(
                new SpellCollision
                {
                    HeroName = "Corki",
                    SpellName = "MissileBarrage2",
                    SDataName = "MissileBarrageMissile2"
                });

            #endregion Corki

            #region DrMundo

            Spells.Add(
                new SpellCollision
                {
                    HeroName = "DrMundo",
                    SpellName = "InfectedCleaverMissileCast",
                    SDataName = "InfectedCleaverMissile"
                });

            #endregion DrMundo

            #region Elise

            Spells.Add(
                new SpellCollision
                {
                    HeroName = "Elise",
                    SpellName = "EliseHumanE",
                    SDataName = "EliseHumanE"
                });

            #endregion Elise

            #region Ezreal

            Spells.Add(
                new SpellCollision
                {
                    HeroName = "Ezreal",
                    SpellName = "EzrealMysticShot",
                    SDataName = "EzrealMysticShot"
                });

            #endregion Ezreal

            #region Jayce

            Spells.Add(
                new SpellCollision
                {
                    HeroName = "Jayce",
                    SpellName = "jayceshockblast",
                    SDataName = "JayceShockBlastMis"
                });

            Spells.Add(
                new SpellCollision
                {
                    HeroName = "Jayce",
                    SpellName = "JayceQAccel",
                    SDataName = "JayceShockBlastWallMis"
                });

            #endregion Jayce

            #region Jinx

            Spells.Add(
                new SpellCollision
                {
                    HeroName = "Jinx",
                    SpellName = "JinxW",
                    SDataName = "JinxWMissile",
                });

            #endregion Jinx

            #region Karma

            Spells.Add(
                new SpellCollision
                {
                    HeroName = "Karma",
                    SpellName = "KarmaQ",
                    SDataName = "KarmaQMissile"
                });

            Spells.Add(
                new SpellCollision
                {
                    HeroName = "Karma",
                    SpellName = "KarmaQMantra",
                    SDataName = "KarmaQMissileMantra"
                });

            #endregion Karma

            #region Kennen

            Spells.Add(
                new SpellCollision
                {
                    HeroName = "Kennen",
                    SpellName = "KennenShurikenHurlMissile1",
                    SDataName = "KennenShurikenHurlMissile1"
                });

            #endregion Kennen

            #region Khazix

            Spells.Add(
                new SpellCollision
                {
                    HeroName = "Khazix",
                    SpellName = "KhazixW",
                    SDataName = "KhazixWMissile"
                });

            #endregion Khazix

            #region Kogmaw

            Spells.Add(
                new SpellCollision
                {
                    HeroName = "Kogmaw",
                    SpellName = "KogMawQ",
                    SDataName = "KogMawQMis"
                });

            #endregion Kogmaw

            #region Leblanc

            Spells.Add(
                new SpellCollision
                {
                    HeroName = "Leblanc",
                    SpellName = "LeblancSoulShackle",
                    SDataName = "LeblancSoulShackle",
                });

            Spells.Add(
                new SpellCollision
                {
                    HeroName = "Leblanc",
                    SpellName = "LeblancSoulShackleM",
                    SDataName = "LeblancSoulShackleM"
                });

            #endregion Leblanc

            #region LeeSin

            Spells.Add(
                new SpellCollision
                {
                    HeroName = "LeeSin",
                    SpellName = "BlindMonkQOne",
                    SDataName = "BlindMonkQOne"
                });

            #endregion LeeSin

            #region Lux

            Spells.Add(
                new SpellCollision
                {
                    HeroName = "Lux",
                    SpellName = "LuxLightBinding",
                    SDataName = "LuxLightBindingMis"
                });

            #endregion Lux
 
            #region Morgana

            Spells.Add(
                new SpellCollision
                {
                    HeroName = "Morgana",
                    SpellName = "DarkBindingMissile",
                    SDataName = "DarkBindingMissile"
                });

            #endregion Morgana
 
            #region Nautilus

            Spells.Add(
                new SpellCollision
                {
                    HeroName = "Nautilus",
                    SpellName = "NautilusAnchorDrag",
                    SDataName = "NautilusAnchorDragMissile"
                });

            #endregion Nautilus

            #region Nidalee

            Spells.Add(
                new SpellCollision
                {
                    HeroName = "Nidalee",
                    SpellName = "JavelinToss",
                    SDataName = "JavelinToss"
                });

            #endregion Nidalee

            #region Quinn

            Spells.Add(
                new SpellCollision
                {
                    HeroName = "Quinn",
                    SpellName = "QuinnQ",
                    SDataName = "QuinnQMissile"
                });

            #endregion Quinn

            #region Rengar

            Spells.Add(
                new SpellCollision
                {
                    HeroName = "Rengar",
                    SpellName = "RengarE",
                    SDataName = "RengarEFinal"
                });

            #endregion Rengar

            #region Rumble

            Spells.Add(
                new SpellCollision
                {
                    HeroName = "Rumble",
                    SpellName = "RumbleGrenade",
                    SDataName = "RumbleGrenade"
                });

            #endregion Rumble

            #region Thresh

            Spells.Add(
                new SpellCollision
                {
                    HeroName = "Thresh",
                    SpellName = "ThreshQ",
                    SDataName = "ThreshQMissile"
                });

            #endregion Thresh

            #region Velkoz

            Spells.Add(
                new SpellCollision
                {
                    HeroName = "Velkoz",
                    SpellName = "VelkozQ",
                    SDataName = "VelkozQMissile"
                });

            Spells.Add(
                new SpellCollision
                {
                    HeroName = "Velkoz",
                    SpellName = "VelkozQSplit",
                    SDataName = "VelkozQMissileSplit"
                });

            #endregion Velkoz

            #region Xerath

            Spells.Add(
                new SpellCollision
                {
                    HeroName = "Xerath",
                    SpellName = "XerathMageSpear",
                    SDataName = "XerathMageSpearMissile"
                });

            #endregion Xerath
        }
    }
}

using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace TAC_Jax
{
    class GameHandler
    {
        /**
         * This file is storing skill data witch jax can actually block,
         * some logic here is placed too, since I don't want to
         * over-spam EventHandler with this stuff.
         * */
        internal static Orbwalking.Orbwalker Orbwalker;
        internal static Vector3 LastWardPos;

        internal static bool HasResetBuffCount = false;

        internal static int BuffCount = 0;
        internal static int LastTick = 0;
        internal static int LastPlaced;
        

        internal static bool IsCastingE
        {
            get { return ObjectManager.Player.HasBuff("JaxCounterStrike"); }
        }

        internal static bool HasSheenActive
        {
            get { return ObjectManager.Player.HasBuff("Sheen"); }
        }
        internal static void UpdateCount()
        {
            /* Check if I have my passive and it didnt expire
             * Only expire the passive when I don't auto attack in 2.5 seconds
             */
            if (HasResetBuffCount == false && Environment.TickCount - LastTick >= 2500)
            {
                if (Jax.Debug)
                    Game.PrintChat("Resetting buff counter to 0");
                LastTick = 0;
                BuffCount = 0;
                HasResetBuffCount = true;
            }
        }
        internal static double GetSheenDamage(Obj_AI_Base target,bool simulate = false)
        {
            if (simulate)
                return Items.HasItem(3057)
                    ? ObjectManager.Player.BaseAttackDamage
                    : (Items.HasItem(3078) ? ObjectManager.Player.BaseAttackDamage*2 : 0);
            else if (Items.HasItem(3057) && ObjectManager.Player.HasBuff("Sheen")) // sheen
                return ObjectManager.Player.BaseAttackDamage;
            else if (Items.HasItem(3078) && ObjectManager.Player.HasBuff("Sheen")) // trinity
                return ObjectManager.Player.BaseAttackDamage * 2;
            else
                return 0;
        }

        internal static double ComboDamage(Obj_AI_Hero target)
        {
            /**
             * TODO:
             * Check item damage.
             * add sheen/triforce damage
             * */
            double damage = 0;
            /* Check if target is in valid Q range
             * Check if target is in valid flash Q range
             */
            if (target.IsValidTarget(SkillHandler.Q.Range)
                    || (SkillHandler.Flash.IsReady() && target.IsValidTarget(SkillHandler.Q.Range + SkillHandler.Flash.Range)))
            {
                // Check if the auto is ready and only then add it to dmg bar ?
                if (Orbwalking.CanAttack()) damage += ObjectManager.Player.GetAutoAttackDamage(target);
                // Check if Q is ready
                if (SkillHandler.Q.IsReady()) damage += ObjectManager.Player.GetSpellDamage(target, SpellSlot.Q);
                // Check if W is ready
                if (SkillHandler.W.IsReady()) damage += ObjectManager.Player.GetSpellDamage(target, SpellSlot.W);
                // Check if R 3rd stack is ready
                if (BuffCount > 0 && BuffCount % 3 == 0) damage += ObjectManager.Player.GetSpellDamage(target, SpellSlot.R);
                // Check ignite damage
                if (SkillHandler.Ignite.IsReady()) damage += SkillHandler.Ignite.GetDamage(target);
                // Check BotRK damage
                if (Items.HasItem(3153) && Items.CanUseItem(3153)) damage += ObjectManager.Player.GetItemDamage(target, Damage.DamageItems.Botrk);
                // Check sheen/trinity damage
                damage += GetSheenDamage(target);
            }
            return damage;
        }
    }
    class GameSpellHandler
    {
        /**
         * @author h3h3
         * Thanks for the list bro!
         * */
        internal static String[] HaveHitEffect = new[]
        {
                "KhazixPAttack", "EliseSpiderBasicAttack3", "EliseSpiderBasicAttack2", "EliseSpiderEBuff",
                "EliseSpiderBasicAttack", "EliseSpiderCritAttack", "EliseSpiderQ", "ZedShadowBasicAttack", "ZedWHandler",
                "ZedRHandler", "Nidalee_CougarBasicAttack", "NidaleeCritAttack", "NidaleeBasicAttack",
                "NidaleeBasicAttack2", "Nidalee_CougarBasicAttack2", "Nidalee_CougarCritAttack", "ThreshBasicAttack",
                "ThreshWAllyMissile", "AnnieTibbersBasicAttack", "AnnieTibbersBasicAttack2", "QuinnWBlind",
                "QuinnRFinaleDamage", "QuinnRFinale", "QuinnValorCritAttack", "QuinnValorBasicAttack",
                "QuinnValorCritAttack2", "QuinnValorCritAttack3", "QuinnValorBasicAttack2", "QuinnValorBasicAttack3",
                "QuinnRDamage", "TwoShivPoison", "ShacoCritAttack", "ShacoBasicAttack2", "ShacoBasicAttack",
                "KarmaSpiritBindSlow", "KarmaShieldSlow", "ScarmaAwesomeWaveSpeed", "ZedW2", "TrundleCritAttack",
                "TrundleBasicAttack2", "TrundleBasicAttack3", "TrundleBasicAttack", "SejuaniBasicAttackBoar",
                "SejuaniCritAttack2", "SejuaniCritAttack3", "SejuaniCritAttack", "SejuaniBasicAttack",
                "SejuaniBasicAttackW", "SejuaniBasicAttack3", "SejuaniBasicAttack2", "UdyrBearAttackUlt",
                "RengarCritAttack", "UdyrPhoenixAttackUlt", "UdyrSpiritPhoenixAttackUlt", "RengarBasicAttack2",
                "RengarBasicAttack3", "UdyrTurtleAttackUlt", "UdyrTigerAttackUlt", "RengarBasicAttack",
                "UdyrSpiritBearAttackUlt", "UdyrTigerPunchBleed", "UdyrPhoenixActivation", "AatroxWONHAttackPower",
                "GangplankCritAttack", "UdyrSpiritPhoenixAttack", "UdyrPhoenixAttack", "UdyrBearStance",
                "UdyrPhoenixBreath", "UdyrBearAttack", "UdyrSpiritBearAttack", "UdyrTigerAttack",
                "GangplankBasicAttack2", "UdyrTurtleStance", "UdyrBasicAttack", "UdyrTigerStance", "UdyrTurtleAttack",
                "GangplankBasicAttack", "OlafBasicAttack", "LucianPassiveAttack", "JayceRangedAttack2",
                "MasterYiCritAttack", "JayceBasicAttack", "LucianBasicAttack2", "OlafCritAttack",
                "JayceRangedAttack", "JayceBasicAttack2", "JayceCritAttack", "JannaBasicAttack2", "JannaCritAttack",
                "EliseRSpider", "LucianBasicAttack", "OlafBasicAttack2", "ViRDunk", "MasterYiBasicAttack",
                "JayceRangedCritAttack", "JannaBasicAttack", "MasterYiBasicAttack2", "UrgotEntropyPassive",
                "QuinnValorQ", "UrgotHeatseekingHomeMissile", "OlafCritAttack2", "GarenQ", "ThreshCritAttack",
                "LucianCritAttack", "JinxBasicAttack", "JinxCritAttack4", "JinxCritAttack5", "JinxCritAttack2",
                "JinxCritAttack3", "JinxCritAttack6", "ThreshBasicAttack1S", "ThreshBasicAttack2L",
                "ThreshBasicAttack2M", "ThreshBasicAttack2S", "ThreshBasicAttack1L", "ThreshBasicAttack1M",
                "LucianPassiveShotDummy", "LucianCritAttack2", "JinxCritAttack", "GarenQAttack", "GarenBasicAttack2",
                "GarenBasicAttack3", "OlafBasicAttack3", "JinxBasicAttack6", "JinxBasicAttack4", "JinxBasicAttack5",
                "JinxBasicAttack2", "JinxBasicAttack3", "ThreshBasicAttack2", "ThreshBasicAttack1SFast",
                "GarenBasicAttack", "GarenCritAttack", "LucianPassiveShot", "HeimerdingerQSpell3", "HeimerdingerQSpell2",
                "NasusBasicAttack", "NasusBasicAttack2", "SivirCritAttack", "BlindMonkRKick", "SivirBasicAttack",
                "LeeSinBasicAttack3", "LeeSinBasicAttack2", "LeeSinBasicAttack4", "LeeSinBasicAttack",
                "SivirBasicAttack2", "LeeSinCritAttack", "NasusCritAttack", "BlindMonkETwo", "AlphaStrike",
                "RivenBasicAttack3", "RivenBasicAttack2", "DianaVortex", "GreatWraithBasicAttack",
                "TryndamereCritAttack", "HA_AP_OrderTurretTutorialBasicAttack", "HA_AP_ChaosTurretBasicAttack",
                "AlphaStrikeBounce", "TryndamereBasicAttack", "YasuoBasicAttack2", "YasuoBasicAttack5",
                "YasuoBasicAttack4", "YasuoBasicAttack6", "YasuoBasicAttack3", "HA_AP_ChaosTurretTutorialBasicAttack",
                "YasuoCritAttack3", "YasuoCritAttack4", "YasuoCritAttack5", "YasuoCritAttack2", "YasuoBasicAttack",
                "RivenCritAttack", "TryndamereBasicAttack2", "YasuoCritAttack", "RivenBasicAttack", "DianaBasicAttack",
                "DianaBasicAttack2", "DianaBasicAttack3", "FrostArrow", "AsheCritAttack", "XerathBasicAttack",
                "AsheBasicAttack", "AsheBasicAttack2", "XerathBasicAttack2", "LissandraEMissile", "LissandraBasicAttack",
                "AhriBasicAttack", "LissandraCritAttack", "LissandraRSlow", "AhriCritAttack", "LissandraEDamage",
                "AhriBasicAttack2", "AhriFoxFireMissileTwo", "VelkozE", "LissandraBasicAttack2",
                "HeimerdingerBasicAttack", "ThreshRPenta", "HeimerdingerQSpell1", "HeimerdingerCritAttack",
                "HeimerTBlueBasicAttack", "XerathMageSpearMissile", "HeimerdingerBasicAttack2", "HeimerdingerE_Ult",
                "HeimerTYellowBasicAttack", "HeimerdingerEUlt", "HeimerTYellowBasicAttack2", "HeimerdingerE",
                "EliseHumanQ", "GragasBasicAttack2", "RumbleBasicAttack2", "RumbleBasicAttack", "GragasBasicAttack",
                "GragasCritAttack", "RumbleCritAttack", "RumbleOverheatAttack", "BlindMonkWTwo", "BlindMonkEOne",
                "TwitchBasicAttack2", "TwitchBasicAttack3", "TwitchBasicAttack", "TwitchSprayAndPrayAttack",
                "BraumLeadTheChargeKnockupVisualMissile", "PantheonBasicAttack2", "PantheonRJump", "PantheonRFall",
                "DariusExecute", "BraumBasicAttackShieldOverride", "DariusCritAttack", "PantheonCritAttack",
                "BraumBasicAttack", "PantheonW", "PantheonR", "PantheonBasicAttack", "BraumBasicAttackPassiveOverride",
                "PantheonEChannel", "BraumShieldPiston", "BraumCritAttack", "BraumR", "BraumW", "BraumE",
                "BraumBasicAttackTower", "BraumBasicAttack2", "BraumBasicAttack3", "CaitlynBasicAttack2",
                "NamiBasicAttack", "CaitlynBasicAttack", "CaitlynCritAttack", "NamiCritAttack", "DariusBasicAttack",
                "DariusBasicAttack2", "NamiBasicAttack2", "CaitlynHeadshotMissile", "KarthusBasicAttack",
                "KarthusCritAttack2", "KarthusBasicAttack2", "QuinnCritAttack", "PoppyBasicAttack", "YasuoDash",
                "QuinnWEnhanced", "QuinnBasicAttack3", "QuinnBasicAttack4", "QuinnBasicAttack2", "BraumAoEShield",
                "QuinnBasicAttack", "KarthusCritAttack", "KhazixBasicAttack2", "ZedCritAttack", "KhazixCritAttack",
                "ZacBasicAttack", "ViBasicAttack3", "ViBasicAttack2", "KhazixBasicAttack", "Bloodlust", "PantheonE",
                "MockingShout", "Nidalee_CougarBasicAttack2", "ZacBasicAttack2", "ZedBasicAttack", "ZedBasicAttack2",
                "Nidalee_CougarBasicAttack", "Nidalee_CougarCritAttack", "ViBasicAttack", "ViCritAttack",
                "ZacCritAttack", "DravenCritAttack", "JayceHyperChargeRangedAttack", "HA_AP_ChaosTurret2BasicAttack",
                "HA_AP_OrderTurretBasicAttack", "HA_AP_ChaosTurretBasicAttack", "HA_AP_ChaosTurret3BasicAttack",
                "NightmareBotVeigarQ", "HA_AP_OrderTurret3BasicAttack", "HA_AP_OrderTurret2BasicAttack",
                "AnnieCritAttack", "AatroxBasicAttack", "AatroxEConeMissile2", "AatroxCritAttack2", "AatroxBasicAttack6",
                "AatroxBasicAttack5", "AatroxBasicAttack4", "AatroxBasicAttack3", "AatroxBasicAttack2",
                "AatroxCritAttack", "AatroxWONHAttackLife", "AatroxEConeMissile", "SonaBasicAttack", "SonaBasicAttack2",
                "SonaQProc", "SonaWAttack", "SonaQAttack", "SonaEAttack", "SonaEPCDeathRecapFix", "SonaQMissile",
                "SonaWPCDeathRecapFix", "SonaQPCDeathRecapFix", "Parley", "MissFortuneCritAttack",
                "MissFortuneBasicAttack", "SivirWAttack", "SivirWAttackBounce",
                "MissFortuneBasicAttack2", "GnarBigAttackTower", "GnarCritAttack", "GnarBasicAttack",
                "GnarBigCritAttack2", "GnarCritAttack2", "GnarBigBasicAttack", "GnarBasicAttack2", "GnarBigCritAttack",
                "GnarBigBasicAttack2", "SRU_KrugMiniBasicAttack5", "SRU_KrugMiniBasicAttack4",
                "SRU_KrugMiniBasicAttack3", "SRU_KrugMiniBasicAttack2", "SRU_KrugBasicAttack",
                "SRU_RazorbeakBasicAttack", "UrgotBasicAttack", "SRU_RedBasicAttack2", "SRU_RedBasicAttack3",
                "BaronAttackMelee", "BaronAttack", "VeigarBalefulStrike", "TristanaBasicAttack2",
                "SRU_KrugMiniBasicAttack", "SRU_GrompBasicAttackMelee", "SRU_OrderMinionMeleeBasicAttack",
                "BaronAcidBall", "SRU_RedBasicAttack", "UrgotCritAttack", "VeigarPrimordialBurst", "TristanaCritAttack",
                "SRU_BlueMini2BasicAttack", "UrgotBasicAttack2", "SRU_GrompBasicAttack", "SRU_RazorbeakBasicAttack2",
                "TristanaBasicAttack", "SRU_RazorbeakMiniBasicAttack", "DetonatingShot", "BaronDeathBreathProj3",
                "BaronDeathBreathProj1", "ViktorCritAttack", "SRU_RazorbeakMiniBasicAttack2", "BaronAcidBall2",
                "BaronAcidBall3", "SRU_OrderMinionMeleeBasicAttack2", "SRU_OrderMinionMeleeBasicAttack3",
                "MasterYiDoubleStrike", "SRU_ChaosMinionSuperBasicAttack", "SRU_KrugBasicAttack3",
                "SRU_KrugBasicAttack2", "SRU_BlueBasicAttack", "RenektonBasicAttack", "SRU_OrderMinionRangedBasicAttack",
                "SRU_ChaosMinionRangedBasicAttack2", "RenektonBasicAttack2", "SRU_OrderMinionSuperBasicAttack",
                "SRU_ChaosMinionSuperBasicAttack2", "RenektonSuperExecute", "SRU_BlueMiniBasicAttack",
                "SRUDragonBasicAttack", "BaronDeathBreathProj", "SRU_OrderMinionSuperBasicAttack2",
                "SRU_RedMiniBasicAttack", "SRU_MurkwolfMiniBasicAttack", "SRU_ChaosMinionMeleeBasicAttack3",
                "SRU_ChaosMinionMeleeBasicAttack2", "SRU_ChaosMinionMeleeBasicAttack",
                "SRU_OrderMinionRangedBasicAttack2", "RenektonExecute", "SRU_ChaosMinionRangedBasicAttack",
                "RenektonCleave", "DragonFireball2", "BaronDeathBreathProj2", "SRU_MurkwolfBasicAttack2",
                "SRU_OrderMinionSiegeBasicAttack", "SRU_ChaosMinionSiegeBasicAttack", "RenektonPredator",
                "RenektonCritAttack", "SRU_MurkwolfBasicAttack", "SRU_MurkwolfMiniBasicAttack2", "SRU_BlueBasicAttack3",
                "SRU_BlueBasicAttack2", "SkarnerPassiveAttack", "CassiopeiaBasicAttack2", "CassiopeiaBasicAttack",
                "SonaCritAttack", "AzirBasicAttack", "CassiopeiaCritAttack", "SkarnerBasicAttack", "AzirBasicAttack2",
                "AzirBasicAttack3", "SkarnerBasicAttack3", "SkarnerBasicAttack2", "Soraka_BasicAttack",
                "KarmaCritAttack", "ViktorBasicAttack", "BusterShot", "KarmaBasicAttack2", "ViktorBasicAttack2",
                "ViktorPowerTransfer", "SorakaBasicAttack2", "ViktorQBuff", "SorakaCritAttack", "SorakaBasicAttack",
                "KarmaBasicAttack", "LeblancBasicAttack2", "LeblancBasicAttack3", "SionBasicAttack", "LeblancChaosOrb",
                "SionCritAttack", "LeblancCritAttack", "LeblancChaosOrbM", "SionBasicAttack2", "LeblancBasicAttack",
                "KogMawBasicAttack", "KogMawCausticSpittle", "SionBasicAttackPassive", "SionBasicAttackTower2",
                "SionBasicAttackPassive2", "ryzecritattack", "SionBasicAttackTower", "arcanemastery", "ryzebasicattack",
                "ryzebasicattack2", "SionBasicAttack3", "ryzedesperatepowerattack", "SkarnerTurretAttack", "KogMawQMis",
                "KogMawBasicAttack2", "SkarnerTurretAttack2", "SkarnerCritAttack", "ZedR2", "maokaisapmagicmelee",
                "SRUAP_Turret_Chaos1BasicAttack", "KogMawIcathianSurprise", "VeigarCritAttack", "VeigarBasicAttack",
                "SRUAP_Turret_Order2BasicAttack", "UdyrPhoenixStance", "SRUAP_Turret_Order3BasicAttack",
                "BaronBasicAttack", "BaronDeathBreath", "VeigarBasicAttack2", "SRUAP_Turret_Order1BasicAttack",
                "SRUAP_Turret_Chaos2BasicAttack", "SRUAP_Turret_Chaos3BasicAttack", "SRU_BaronBasicAttack",
                "ViktorPowerTransferReturn", "BaronSpike", "XerathCritAttack", "SRUAP_Turret_Order3_TestBasicAttack",
                "KhazixW", "KogMawBioArcaneBarrageAttack", "maokaicritattack", "maokaibasicattack2",
                "SRUAP_Turret_Chaos3_TestBasicAttack", "TwitchCritAttack", "KhazixWLong", "maokaibasicattack",
                "KalistaCritAttack", "SRUAP_Turret_Order4BasicAttack", "KalistaBasicAttack", "KalistaBasicAttackPow",
                "SRUAP_Turret_Chaos4BasicAttack", "KalistaBasicAttackNone", "KalistaBasicAttackSlow",
                "SRU_SpiritwolfBasicAttack2", "SRU_SpiritwolfBasicAttack", "SonaQ"
        };
        internal static bool CanDodge(String spellName)
        {
            return HaveHitEffect.Contains(spellName);
        }
    }
}

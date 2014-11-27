using System;
using LeagueSharp;
using System.Collections.Generic;

namespace Oracle
{
    public enum CastType
    {
        Skillshot = 0,
        EnemyCast = 1,
        SelfCast = 2,
        TeamCast = 3,
        Always = 4
    }

    public enum RiskLevel
    {
        NoDamage = 0,
        Normal = 1,
        High = 2,
        Extreme = 3,
        Stealth = 4
    }

    public class OracleLib
    {
        public string Name { get; set; }
        public SpellSlot Slot { get; set; }

        public int Timer { get; set; }
        public double Speed { get; set; }
        public float Range { get; set; }
        public float Width { get; set; }
        public double Delay { get; set; }
        public float Duration { get; set; }

        public CastType Type { get; set; }
        public RiskLevel DangerLevel { get; set; }

        public bool CC { get; set; }
        public bool CheckLine { get; set; }
        public bool OnHit { get; set; }

        public static readonly string[] SmallMinions =
        {
            "SRU_Murkwolf",
            "SRU_Razorbeak",
            "SRU_Krug",
            "SRU_Gromp"
        };

        public static readonly string[] EpicMinions =
        {
            "TT_Spiderboss",
            "SRU_Baron",
            "SRU_Dragon"
        };

        public static readonly string[] LargeMinions =
        {
            "Sru_Crab",
            "SRU_Blue",
            "SRU_Red",
            "TT_NWraith",
            "TT_NGolem",
            "TT_NWolf"
        };

        public static readonly int[] SmiteAll =
        {
            3713, 3726, 3725, 3726, 3723,
            3711, 3722, 3721, 3720, 3719,
            3715, 3718, 3717, 3716, 3714,
            3706, 3710, 3709, 3708, 3707,
        };

        public static readonly int[] SmitePurple = { 3713, 3726, 3725, 3726, 3723 };
        public static readonly int[] SmiteGrey = { 3711, 3722, 3721, 3720, 3719 };
        public static readonly int[] SmiteRed = { 3715, 3718, 3717, 3716, 3714 };
        public static readonly int[] SmiteBlue = { 3706, 3710, 3709, 3708, 3707 };

        public static List<OracleLib> Database = new List<OracleLib>();
        public static List<OracleLib> CleanseBuffs = new List<OracleLib>(); 

        static OracleLib()
        {
            CleanseBuffs.Add(new OracleLib
            {
                Name = "fizzmarinerdoombomb",
                Timer = 0,
                DangerLevel = RiskLevel.Extreme
            });

            CleanseBuffs.Add(new OracleLib
            {
                Name = "SoulShackles",
                Timer = 2500,
                DangerLevel = RiskLevel.Extreme
            });

            CleanseBuffs.Add(new OracleLib
            {
                Name = "zedulttargetmark",
                Timer = 1500,
                DangerLevel = RiskLevel.High
            });

            Database.Add(new OracleLib
            {
                Name = "Aatrox",
                Slot = SpellSlot.Q,
                Range = 650,
                Width = 0,
                Speed = 20,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Normal,
                CC = true,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Aatrox",
                Slot = SpellSlot.W,
                Range = 0,
                Width = 0,
                Speed = 0,
                Delay = 0,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Aatrox",
                Slot = SpellSlot.W,
                Range = 0,
                Width = 0,
                Speed = 0,
                Delay = 0,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Aatrox",
                Slot = SpellSlot.E,
                Range = 1000,
                Width = 150,
                Speed = 1200,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = true
            });

            Database.Add(new OracleLib
            {
                Name = "Aatrox",
                Slot = SpellSlot.R,
                Range = 550,
                Width = 550,
                Speed = 0,
                Delay = 0,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Ahri",
                Slot = SpellSlot.Q,
                Range = 880,
                Width = 80,
                Speed = 1100,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = true
            });

            Database.Add(new OracleLib
            {
                Name = "Ahri",
                Slot = SpellSlot.W,
                Range = 800,
                Width = 800,
                Speed = 1800,
                Delay = 0,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Ahri",
                Slot = SpellSlot.E,
                Range = 975,
                Width = 60,
                Speed = 1200,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = true
            });

            Database.Add(new OracleLib
            {
                Name = "Ahri",
                Slot = SpellSlot.R,
                Range = 450,
                Width = 0,
                Speed = 2200,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Akali",
                Slot = SpellSlot.Q,
                Range = 600,
                Width = 0,
                Speed = 1000,
                Delay = 0.65,
                Type = CastType.EnemyCast,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Akali",
                Slot = SpellSlot.W,
                Range = 700,
                Width = 0,
                Speed = 0,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.High,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Akali",
                Slot = SpellSlot.E,
                Range = 325,
                Width = 325,
                Speed = 0,
                Delay = 0,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Akali",
                Slot = SpellSlot.R,
                Range = 800,
                Width = 0,
                Speed = 2200,
                Delay = 0,
                Type = CastType.EnemyCast,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Alistar",
                Slot = SpellSlot.Q,
                Range = 365,
                Width = 365,
                Speed = 20,
                Delay = 0.5,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Alistar",
                Slot = SpellSlot.W,
                Range = 650,
                Width = 0,
                Speed = 0,
                Delay = 0.5,
                Type = CastType.EnemyCast,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = true
            });

            Database.Add(new OracleLib
            {
                Name = "Alistar",
                Slot = SpellSlot.E,
                Range = 575,
                Width = 0,
                Speed = 0,
                Delay = 0,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false,

            });

            Database.Add(new OracleLib
            {
                Name = "Alistar",
                Slot = SpellSlot.R,
                Range = 0,
                Width = 0,
                Speed = 828,
                Delay = 0,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false,

            });

            Database.Add(new OracleLib
            {
                Name = "Amumu",
                Slot = SpellSlot.Q,
                Range = 1100,
                Width = 80,
                Speed = 2000,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = true
            });

            Database.Add(new OracleLib
            {
                Name = "Amumu",
                Slot = SpellSlot.W,
                Range = 300,
                Width = 300,
                Speed = int.MaxValue,
                Delay = 0.47,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Amumu",
                Slot = SpellSlot.E,
                Range = 350,
                Width = 350,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Amumu",
                Slot = SpellSlot.R,
                Range = 550,
                Width = 550,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = false,
                Timer = 0
            });

            Database.Add(new OracleLib
            {
                Name = "Anivia",
                Slot = SpellSlot.Q,
                Range = 1200,
                Width = 110,
                Speed = 850,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = true
            });

            Database.Add(new OracleLib
            {
                Name = "Anivia",
                Slot = SpellSlot.W,
                Range = 1000,
                Width = 400,
                Speed = 1600,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Anivia",
                Slot = SpellSlot.E,
                Range = 650,
                Width = 0,
                Speed = 1200,
                Delay = 0.5,
                Type = CastType.EnemyCast,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Anivia",
                Slot = SpellSlot.R,
                Range = 675,
                Width = 400,
                Speed = int.MaxValue,
                Delay = 0.3,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Annie",
                Slot = SpellSlot.Q,
                Range = 623,
                Width = 0,
                Speed = 1400,
                Delay = 0.5,
                Type = CastType.EnemyCast,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Annie",
                Slot = SpellSlot.W,
                Range = 623,
                Width = 0,
                Speed = 0,
                Delay = 0.5,
                Type = CastType.EnemyCast,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = true
            });

            Database.Add(new OracleLib
            {
                Name = "Annie",
                Slot = SpellSlot.E,
                Range = 100,
                Width = 0,
                Speed = 20,
                Delay = 0,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false,

            });

            Database.Add(new OracleLib
            {
                Name = "Annie",
                Slot = SpellSlot.R,
                Range = 600,
                Width = 290,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = true,
                Timer = 0
            });

            Database.Add(new OracleLib
            {
                Name = "Ashe",
                Slot = SpellSlot.Q,
                Range = 0,
                Width = 0,
                Speed = int.MaxValue,
                Delay = 0,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Ashe",
                Slot = SpellSlot.Q,
                Range = 0,
                Width = 0,
                Speed = int.MaxValue,
                Delay = 0,
                Type = CastType.EnemyCast,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Ashe",
                Slot = SpellSlot.W,
                Range = 1200,
                Width = 250,
                Speed = 902,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = true
            });

            Database.Add(new OracleLib
            {
                Name = "Ashe",
                Slot = SpellSlot.E,
                Range = 2500,
                Width = 0,
                Speed = 1400,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Ashe",
                Slot = SpellSlot.R,
                Range = 50000,
                Width = 130,
                Speed = 1600,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = true
            });

            Database.Add(new OracleLib
            {
                Name = "Blitzcrank",
                Slot = SpellSlot.Q,
                Range = 925,
                Width = 70,
                Speed = 1800,
                Delay = 0.22,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = true
            });

            Database.Add(new OracleLib
            {
                Name = "Blitzcrank",
                Slot = SpellSlot.W,
                Range = 0,
                Width = 0,
                Speed = 0,
                Delay = 0,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Blitzcrank",
                Slot = SpellSlot.E,
                Range = 0,
                Width = 0,
                Speed = 0,
                Delay = 0,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Blitzcrank",
                Slot = SpellSlot.R,
                Range = 600,
                Width = 600,
                Speed = 0,
                Delay = 0,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Brand",
                Slot = SpellSlot.Q,
                Range = 1150,
                Width = 80,
                Speed = 1200,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = true
            });

            Database.Add(new OracleLib
            {
                Name = "Brand",
                Slot = SpellSlot.W,
                Range = 240,
                Width = 0,
                Speed = 20,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Brand",
                Slot = SpellSlot.E,
                Range = 0,
                Width = 0,
                Speed = 1800,
                Delay = 0,
                Type = CastType.EnemyCast,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Brand",
                Slot = SpellSlot.R,
                Range = 0,
                Width = 0,
                Speed = 1000,
                Delay = 0,
                Type = CastType.EnemyCast,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false,
                Timer = 230 - Game.Ping
            });

            Database.Add(new OracleLib
            {
                Name = "Braum",
                Slot = SpellSlot.Q,
                Range = 1100,
                Width = 100,
                Speed = 1200,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.High,
                CC = true,
                CheckLine = true
            });

            Database.Add(new OracleLib
            {
                Name = "Braum",
                Slot = SpellSlot.W,
                Range = 650,
                Width = 0,
                Speed = 1500,
                Delay = 0.5,
                Type = CastType.TeamCast,
                DangerLevel = RiskLevel.Extreme,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Braum",
                Slot = SpellSlot.E,
                Range = 250,
                Width = 0,
                Speed = int.MaxValue,
                Delay = 0,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Braum",
                Slot = SpellSlot.R,
                Range = 1250,
                Width = 180,
                Speed = 1200,
                Delay = 0,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = true
            });

            Database.Add(new OracleLib
            {
                Name = "Caitlyn",
                Slot = SpellSlot.Q,
                Range = 2000,
                Width = 90,
                Speed = 2200,
                Delay = 0.25,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = true
            });

            Database.Add(new OracleLib
            {
                Name = "Caitlyn",
                Slot = SpellSlot.W,
                Range = 800,
                Width = 0,
                Speed = 1400,
                Delay = 0,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Caitlyn",
                Slot = SpellSlot.E,
                Range = 950,
                Width = 80,
                Speed = 2000,
                Delay = 0.25,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = true
            });

            Database.Add(new OracleLib
            {
                Name = "Caitlyn",
                Slot = SpellSlot.R,
                Range = 2500,
                Width = 0,
                Speed = 1500,
                Delay = 0,
                Type = CastType.EnemyCast,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false,
                Timer = 1350 - Game.Ping
            });

            Database.Add(new OracleLib
            {
                Name = "Cassiopeia",
                Slot = SpellSlot.Q,
                Range = 925,
                Width = 130,
                Speed = int.MaxValue,
                Delay = 0.25,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Cassiopeia",
                Slot = SpellSlot.W,
                Range = 925,
                Width = 212,
                Speed = 2500,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Cassiopeia",
                Slot = SpellSlot.E,
                Range = 700,
                Width = 0,
                Speed = 1900,
                Delay = 0,
                Type = CastType.EnemyCast,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Cassiopeia",
                Slot = SpellSlot.R,
                Range = 875,
                Width = 210,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = true,
                Timer = 0
            });

            Database.Add(new OracleLib
            {
                Name = "Chogath",
                Slot = SpellSlot.Q,
                Range = 1000,
                Width = 250,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Chogath",
                Slot = SpellSlot.W,
                Range = 675,
                Width = 210,
                Speed = int.MaxValue,
                Delay = 0.25,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = true
            });

            Database.Add(new OracleLib
            {
                Name = "Chogath",
                Slot = SpellSlot.E,
                Range = 0,
                Width = 170,
                Speed = 347,
                Delay = 0,
                Type = CastType.EnemyCast,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Chogath",
                Slot = SpellSlot.R,
                Range = 230,
                Width = 0,
                Speed = 500,
                Delay = 0,
                Type = CastType.EnemyCast,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Corki",
                Slot = SpellSlot.Q,
                Range = 875,
                Width = 250,
                Speed = int.MaxValue,
                Delay = 0,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Corki",
                Slot = SpellSlot.W,
                Range = 875,
                Width = 160,
                Speed = 700,
                Delay = 0,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = true
            });

            Database.Add(new OracleLib
            {
                Name = "Corki",
                Slot = SpellSlot.E,
                Range = 750,
                Width = 100,
                Speed = 902,
                Delay = 0,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = true
            });

            Database.Add(new OracleLib
            {
                Name = "Corki",
                Slot = SpellSlot.R,
                Range = 1225,
                Width = 40,
                Speed = 828.5,
                Delay = 0.25,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = true
            });

            Database.Add(new OracleLib
            {
                Name = "Darius",
                Slot = SpellSlot.Q,
                Range = 425,
                Width = 0,
                Speed = 0,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Darius",
                Slot = SpellSlot.W,
                Range = 210,
                Width = 0,
                Speed = 0,
                Delay = 0,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false,
                OnHit = true
            });

            Database.Add(new OracleLib
            {
                Name = "Darius",
                Slot = SpellSlot.E,
                Range = 540,
                Width = 0,
                Speed = 1500,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.High,
                CC = true,
                CheckLine = true
            });

            Database.Add(new OracleLib
            {
                Name = "Darius",
                Slot = SpellSlot.R,
                Range = 460,
                Width = 0,
                Speed = 20,
                Delay = 0.5,
                Type = CastType.EnemyCast,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Diana",
                Slot = SpellSlot.Q,
                Range = 900,
                Width = 75,
                Speed = 1500,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Normal,
                CC = true,
                CheckLine = true
            });

            Database.Add(new OracleLib
            {
                Name = "Diana",
                Slot = SpellSlot.W,
                Range = 0,
                Width = 0,
                Speed = 0,
                Delay = 0,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = false,

            });

            Database.Add(new OracleLib
            {
                Name = "Diana",
                Slot = SpellSlot.E,
                Range = 300,
                Width = 300,
                Speed = 1500,
                Delay = 0.5,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.High,
                CC = true,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Diana",
                Slot = SpellSlot.R,
                Range = 800,
                Width = 0,
                Speed = 1500,
                Delay = 0.5,
                Type = CastType.EnemyCast,
                DangerLevel = RiskLevel.Normal,
                CC = true,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "DrMundo",
                Slot = SpellSlot.Q,
                Range = 1000,
                Width = 75,
                Speed = 1500,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = true
            });

            Database.Add(new OracleLib
            {
                Name = "DrMundo",
                Slot = SpellSlot.W,
                Range = 225,
                Width = 225,
                Speed = int.MaxValue,
                Delay = int.MaxValue,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "DrMundo",
                Slot = SpellSlot.E,
                Range = 0,
                Width = 0,
                Speed = int.MaxValue,
                Delay = int.MaxValue,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "DrMundo",
                Slot = SpellSlot.R,
                Range = 0,
                Width = 0,
                Speed = int.MaxValue,
                Delay = int.MaxValue,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Draven",
                Slot = SpellSlot.Q,
                Range = 0,
                Width = 0,
                Speed = int.MaxValue,
                Delay = int.MaxValue,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Draven",
                Slot = SpellSlot.W,
                Range = 0,
                Width = 0,
                Speed = int.MaxValue,
                Delay = int.MaxValue,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Draven",
                Slot = SpellSlot.E,
                Range = 1050,
                Width = 130,
                Speed = 1600,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = true
            });

            Database.Add(new OracleLib
            {
                Name = "Draven",
                Slot = SpellSlot.R,
                Range = 20000,
                Width = 160,
                Speed = 2000,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = true
            });

            Database.Add(new OracleLib
            {
                Name = "Elise",
                Slot = SpellSlot.Q,
                Range = 625,
                Width = 0,
                Speed = 2200,
                Delay = 0.75,
                Type = CastType.EnemyCast,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Elise",
                Slot = SpellSlot.W,
                Range = 950,
                Width = 235,
                Speed = 5000,
                Delay = 0.75,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Elise",
                Slot = SpellSlot.E,
                Range = 1075,
                Width = 70,
                Speed = 1450,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.High,
                CC = true,
                CheckLine = true
            });

            Database.Add(new OracleLib
            {
                Name = "Elise",
                Slot = SpellSlot.R,
                Range = 0,
                Width = 0,
                Speed = int.MaxValue,
                Delay = int.MaxValue,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Elise",
                Slot = SpellSlot.Q + 14,
                Range = 475,
                Width = 0,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.EnemyCast,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Elise",
                Slot = SpellSlot.W + 14,
                Range = 0,
                Width = 0,
                Speed = int.MaxValue,
                Delay = int.MaxValue,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Elise",
                Slot = SpellSlot.E + 14,
                Range = 975,
                Width = 0,
                Speed = int.MaxValue,
                Delay = int.MaxValue,
                Type = CastType.EnemyCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Elise",
                Slot = SpellSlot.E + 14,
                Range = 975,
                Width = 0,
                Speed = int.MaxValue,
                Delay = int.MaxValue,
                Type = CastType.EnemyCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Elise",
                Slot = SpellSlot.R,
                Range = 0,
                Width = 0,
                Speed = int.MaxValue,
                Delay = int.MaxValue,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Evelynn",
                Slot = SpellSlot.Q,
                Range = 500,
                Width = 500,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Evelynn",
                Slot = SpellSlot.W,
                Range = 0,
                Width = 0,
                Speed = int.MaxValue,
                Delay = int.MaxValue,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false,

            });

            Database.Add(new OracleLib
            {
                Name = "Evelynn",
                Slot = SpellSlot.E,
                Range = 290,
                Width = 0,
                Speed = 900,
                Delay = 0.5,
                Type = CastType.EnemyCast,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Evelynn",
                Slot = SpellSlot.R,
                Range = 650,
                Width = 350,
                Speed = 1300,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Ezreal",
                Slot = SpellSlot.Q,
                Range = 1200,
                Width = 60,
                Speed = 2000,
                Delay = 0.25,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = true,
                OnHit = true
            });

            Database.Add(new OracleLib
            {
                Name = "Ezreal",
                Slot = SpellSlot.W,
                Range = 1050,
                Width = 80,
                Speed = 1600,
                Delay = 0.25,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = true
            });

            Database.Add(new OracleLib
            {
                Name = "Ezreal",
                Slot = SpellSlot.W,
                Range = 1050,
                Width = 80,
                Speed = 1600,
                Delay = 0.25,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = true
            });

            Database.Add(new OracleLib
            {
                Name = "Ezreal",
                Slot = SpellSlot.E,
                Range = 475,
                Width = 0,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.EnemyCast,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Ezreal",
                Slot = SpellSlot.R,
                Range = 20000,
                Width = 160,
                Speed = 2000,
                Delay = 1,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = true
            });

            Database.Add(new OracleLib
            {
                Name = "FiddleSticks",
                Slot = SpellSlot.Q,
                Range = 575,
                Width = 0,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.EnemyCast,
                DangerLevel = RiskLevel.High,
                CC = true,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "FiddleSticks",
                Slot = SpellSlot.W,
                Range = 575,
                Width = 0,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.EnemyCast,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "FiddleSticks",
                Slot = SpellSlot.E,
                Range = 750,
                Width = 0,
                Speed = 1100,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "FiddleSticks",
                Slot = SpellSlot.R,
                Range = 800,
                Width = 600,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.EnemyCast,
                DangerLevel = RiskLevel.High,
                CC = true,
                CheckLine = false,
                Timer = 0
            });

            Database.Add(new OracleLib
            {
                Name = "Fiora",
                Slot = SpellSlot.Q,
                Range = 300,
                Width = 0,
                Speed = 2200,
                Delay = 0.5,
                Type = CastType.EnemyCast,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Fiora",
                Slot = SpellSlot.W,
                Range = 100,
                Width = 0,
                Speed = 0,
                Delay = 0,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false,

            });

            Database.Add(new OracleLib
            {
                Name = "Fiora",
                Slot = SpellSlot.E,
                Range = 210,
                Width = 0,
                Speed = 0,
                Delay = 0,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Fiora",
                Slot = SpellSlot.R,
                Range = 210,
                Width = 0,
                Speed = 0,
                Delay = 0.5,
                Type = CastType.EnemyCast,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false,
                Timer = 280 - Game.Ping
            });

            Database.Add(new OracleLib
            {
                Name = "Fizz",
                Slot = SpellSlot.Q,
                Range = 550,
                Width = 0,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.EnemyCast,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false,
                OnHit = true
            });

            Database.Add(new OracleLib
            {
                Name = "Fizz",
                Slot = SpellSlot.W,
                Range = 0,
                Width = 0,
                Speed = 0,
                Delay = 0.5,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Fizz",
                Slot = SpellSlot.E,
                Range = 400,
                Width = 120,
                Speed = 1300,
                Delay = 0.5,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = false,

            });

            Database.Add(new OracleLib
            {
                Name = "Fizz",
                Slot = SpellSlot.E,
                Range = 400,
                Width = 500,
                Speed = 1300,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Fizz",
                Slot = SpellSlot.R,
                Range = 1275,
                Width = 250,
                Speed = 1200,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Galio",
                Slot = SpellSlot.Q,
                Range = 940,
                Width = 120,
                Speed = 1300,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Galio",
                Slot = SpellSlot.W,
                Range = 800,
                Width = 0,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.TeamCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false,

            });

            Database.Add(new OracleLib
            {
                Name = "Galio",
                Slot = SpellSlot.E,
                Range = 1180,
                Width = 140,
                Speed = 1200,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = true
            });

            Database.Add(new OracleLib
            {
                Name = "Galio",
                Slot = SpellSlot.R,
                Range = 560,
                Width = 560,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = false,
                Timer = 0
            });

            Database.Add(new OracleLib
            {
                Name = "Gangplank",
                Slot = SpellSlot.Q,
                Range = 625,
                Width = 0,
                Speed = 2000,
                Delay = 0.5,
                Type = CastType.EnemyCast,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false,
                OnHit = true
            });

            Database.Add(new OracleLib
            {
                Name = "Gangplank",
                Slot = SpellSlot.W,
                Range = 0,
                Width = 0,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false,


            });

            Database.Add(new OracleLib
            {
                Name = "Gangplank",
                Slot = SpellSlot.E,
                Range = 1300,
                Width = 0,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Gangplank",
                Slot = SpellSlot.R,
                Range = 20000,
                Width = 525,
                Speed = 500,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Normal,
                CC = true,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Garen",
                Slot = SpellSlot.Q,
                Range = 0,
                Width = 0,
                Speed = int.MaxValue,
                Delay = 0.2,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false,

            });

            Database.Add(new OracleLib
            {
                Name = "Garen",
                Slot = SpellSlot.W,
                Range = 0,
                Width = 0,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false,

            });

            Database.Add(new OracleLib
            {
                Name = "Garen",
                Slot = SpellSlot.E,
                Range = 325,
                Width = 325,
                Speed = 700,
                Delay = 0,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Garen",
                Slot = SpellSlot.R,
                Range = 400,
                Width = 0,
                Speed = int.MaxValue,
                Delay = 0.12,
                Type = CastType.EnemyCast,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Gragas",
                Slot = SpellSlot.Q,
                Range = 1100,
                Width = 320,
                Speed = 1000,
                Delay = 0.3,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Gragas",
                Slot = SpellSlot.Q,
                Range = 1100,
                Width = 320,
                Speed = 1000,
                Delay = 0.3,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Gragas",
                Slot = SpellSlot.W,
                Range = 0,
                Width = 0,
                Speed = 0,
                Delay = 0,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Gragas",
                Slot = SpellSlot.E,
                Range = 1100,
                Width = 50,
                Speed = 1000,
                Delay = 0.3,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = true
            });

            Database.Add(new OracleLib
            {
                Name = "Gragas",
                Slot = SpellSlot.R,
                Range = 1100,
                Width = 700,
                Speed = 1000,
                Delay = 0.3,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Graves",
                Slot = SpellSlot.Q,
                Range = 1100,
                Width = 10,
                Speed = 902,
                Delay = 0.3,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = true
            });

            Database.Add(new OracleLib
            {
                Name = "Graves",
                Slot = SpellSlot.W,
                Range = 1100,
                Width = 250,
                Speed = 1650,
                Delay = 0.3,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Graves",
                Slot = SpellSlot.W,
                Range = 1100,
                Width = 250,
                Speed = 1650,
                Delay = 0.3,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Graves",
                Slot = SpellSlot.E,
                Range = 425,
                Width = 50,
                Speed = 1000,
                Delay = 0.3,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Graves",
                Slot = SpellSlot.R,
                Range = 1000,
                Width = 100,
                Speed = 1200,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = true
            });

            Database.Add(new OracleLib
            {
                Name = "Hecarim",
                Slot = SpellSlot.Q,
                Range = 350,
                Width = 350,
                Speed = 1450,
                Delay = 0.3,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Hecarim",
                Slot = SpellSlot.W,
                Range = 525,
                Width = 525,
                Speed = 828.5,
                Delay = 0.12,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Hecarim",
                Slot = SpellSlot.E,
                Range = 0,
                Width = 0,
                Speed = int.MaxValue,
                Delay = int.MaxValue,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Hecarim",
                Slot = SpellSlot.R,
                Range = 1350,
                Width = 200,
                Speed = 1200,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = true
            });

            Database.Add(new OracleLib
            {
                Name = "Heimerdinger",
                Slot = SpellSlot.Q,
                Range = 350,
                Width = 0,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Heimerdinger",
                Slot = SpellSlot.W,
                Range = 1525,
                Width = 200,
                Speed = 902,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = true
            });

            Database.Add(new OracleLib
            {
                Name = "Heimerdinger",
                Slot = SpellSlot.E,
                Range = 970,
                Width = 120,
                Speed = 2500,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Heimerdinger",
                Slot = SpellSlot.R,
                Range = 0,
                Width = 0,
                Speed = int.MaxValue,
                Delay = 0.23,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Heimerdinger",
                Slot = SpellSlot.R,
                Range = 970,
                Width = 250,
                Speed = int.MaxValue,
                Delay = 0.23,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = true
            });

            Database.Add(new OracleLib
            {
                Name = "Irelia",
                Slot = SpellSlot.Q,
                Range = 650,
                Width = 0,
                Speed = 2200,
                Delay = 0,
                Type = CastType.EnemyCast,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false,
                OnHit = true
            });

            Database.Add(new OracleLib
            {
                Name = "Irelia",
                Slot = SpellSlot.W,
                Range = 0,
                Width = 0,
                Speed = 347,
                Delay = 0.23,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Irelia",
                Slot = SpellSlot.E,
                Range = 325,
                Width = 0,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.EnemyCast,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Irelia",
                Slot = SpellSlot.R,
                Range = 1200,
                Width = 0,
                Speed = 779,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Janna",
                Slot = SpellSlot.Q,
                Range = 1800,
                Width = 200,
                Speed = int.MaxValue,
                Delay = 0,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = true
            });

            Database.Add(new OracleLib
            {
                Name = "Janna",
                Slot = SpellSlot.W,
                Range = 600,
                Width = 0,
                Speed = 1600,
                Delay = 0.5,
                Type = CastType.EnemyCast,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = true
            });

            Database.Add(new OracleLib
            {
                Name = "Janna",
                Slot = SpellSlot.E,
                Range = 800,
                Width = 0,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.TeamCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false,

            });

            Database.Add(new OracleLib
            {
                Name = "Janna",
                Slot = SpellSlot.R,
                Range = 725,
                Width = 725,
                Speed = 828.5,
                Delay = 0.5,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = true,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "JarvanIV",
                Slot = SpellSlot.Q,
                Range = 700,
                Width = 70,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = true
            });

            Database.Add(new OracleLib
            {
                Name = "JarvanIV",
                Slot = SpellSlot.W,
                Range = 300,
                Width = 300,
                Speed = 0,
                Delay = 0.5,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.High,
                CC = true,
                CheckLine = false,

            });

            Database.Add(new OracleLib
            {
                Name = "JarvanIV",
                Slot = SpellSlot.E,
                Range = 830,
                Width = 75,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "JarvanIV",
                Slot = SpellSlot.R,
                Range = 650,
                Width = 325,
                Speed = 0,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Jax",
                Slot = SpellSlot.Q,
                Range = 210,
                Width = 0,
                Speed = 0,
                Delay = 0.5,
                Type = CastType.Always,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Jax",
                Slot = SpellSlot.W,
                Range = 0,
                Width = 0,
                Speed = 0,
                Delay = 0.5,
                Type = CastType.EnemyCast,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false,
                OnHit = true
            });

            Database.Add(new OracleLib
            {
                Name = "Jax",
                Slot = SpellSlot.E,
                Range = 425,
                Width = 425,
                Speed = 1450,
                Delay = 0.5,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = true
            });

            Database.Add(new OracleLib
            {
                Name = "Jax",
                Slot = SpellSlot.R,
                Range = 0,
                Width = 0,
                Speed = 0,
                Delay = 0,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Jayce",
                Slot = SpellSlot.Q,
                Range = 600,
                Width = 0,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.EnemyCast,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Jayce",
                Slot = SpellSlot.W,
                Range = 285,
                Width = 285,
                Speed = 1500,
                Delay = 0.5,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false,
                OnHit = true
            });

            Database.Add(new OracleLib
            {
                Name = "Jayce",
                Slot = SpellSlot.E,
                Range = 300,
                Width = 80,
                Speed = int.MaxValue,
                Delay = 0,
                Type = CastType.EnemyCast,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Jayce",
                Slot = SpellSlot.R,
                Range = 0,
                Width = 0,
                Speed = int.MaxValue,
                Delay = 0.75,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Jayce",
                Slot = SpellSlot.Q + 14,
                Range = 1050,
                Width = 80,
                Speed = 1200,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Jayce",
                Slot = SpellSlot.W + 14,
                Range = 0,
                Width = 0,
                Speed = int.MaxValue,
                Delay = 0.75,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Jayce",
                Slot = SpellSlot.E + 14,
                Range = 685,
                Width = 0,
                Speed = 1600,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Jayce",
                Slot = SpellSlot.R,
                Range = 0,
                Width = 0,
                Speed = int.MaxValue,
                Delay = 0.75,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Jinx",
                Slot = SpellSlot.Q,
                Range = 0,
                Width = 0,
                Speed = int.MaxValue,
                Delay = 0,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Jinx",
                Slot = SpellSlot.W,
                Range = 1550,
                Width = 70,
                Speed = 1200,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.High,
                CC = true,
                CheckLine = true
            });

            Database.Add(new OracleLib
            {
                Name = "Jinx",
                Slot = SpellSlot.W,
                Range = 1550,
                Width = 70,
                Speed = 1200,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = true
            });

            Database.Add(new OracleLib
            {
                Name = "Jinx",
                Slot = SpellSlot.E,
                Range = 900,
                Width = 550,
                Speed = 1000,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Jinx",
                Slot = SpellSlot.R,
                Range = 25000,
                Width = 120,
                Speed = int.MaxValue,
                Delay = 0,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = true
            });

            Database.Add(new OracleLib
            {
                Name = "Jinx",
                Slot = SpellSlot.R,
                Range = 25000,
                Width = 120,
                Speed = int.MaxValue,
                Delay = 0,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = true
            });

            Database.Add(new OracleLib
            {
                Name = "Karthus",
                Slot = SpellSlot.Q,
                Range = 875,
                Width = 160,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Karthus",
                Slot = SpellSlot.W,
                Range = 1090,
                Width = 525,
                Speed = 1600,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.High,
                CC = true,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Karthus",
                Slot = SpellSlot.E,
                Range = 550,
                Width = 550,
                Speed = 1000,
                Delay = 0.5,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Karthus",
                Slot = SpellSlot.R,
                Range = 20000,
                Width = 0,
                Speed = int.MaxValue,
                Delay = 0,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false,
                Timer = 2200
            });

            Database.Add(new OracleLib
            {
                Name = "Karma",
                Slot = SpellSlot.Q,
                Range = 950,
                Width = 90,
                Speed = 902,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = true
            });

            Database.Add(new OracleLib
            {
                Name = "Karma",
                Slot = SpellSlot.W,
                Range = 700,
                Width = 60,
                Speed = 2000,
                Delay = 0.5,
                Type = CastType.EnemyCast,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Karma",
                Slot = SpellSlot.E,
                Range = 800,
                Width = 0,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.TeamCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false,

            });

            Database.Add(new OracleLib
            {
                Name = "Karma",
                Slot = SpellSlot.R,
                Range = 0,
                Width = 0,
                Speed = 1300,
                Delay = 0.5,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Kassadin",
                Slot = SpellSlot.Q,
                Range = 650,
                Width = 0,
                Speed = 1400,
                Delay = 0.5,
                Type = CastType.EnemyCast,
                DangerLevel = RiskLevel.Extreme,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Kassadin",
                Slot = SpellSlot.W,
                Range = 0,
                Width = 0,
                Speed = 0,
                Delay = 0,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false,
                OnHit = true
            });

            Database.Add(new OracleLib
            {
                Name = "Kassadin",
                Slot = SpellSlot.E,
                Range = 700,
                Width = 10,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = true
            });

            Database.Add(new OracleLib
            {
                Name = "Kassadin",
                Slot = SpellSlot.R,
                Range = 675,
                Width = 150,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Katarina",
                Slot = SpellSlot.Q,
                Range = 675,
                Width = 0,
                Speed = 1800,
                Delay = 0.5,
                Type = CastType.EnemyCast,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Katarina",
                Slot = SpellSlot.W,
                Range = 400,
                Width = 400,
                Speed = 1800,
                Delay = 0.5,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Katarina",
                Slot = SpellSlot.E,
                Range = 700,
                Width = 0,
                Speed = 0,
                Delay = 0.5,
                Type = CastType.EnemyCast,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Katarina",
                Slot = SpellSlot.R,
                Range = 550,
                Width = 550,
                Speed = 1450,
                Delay = 0.5,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Kayle",
                Slot = SpellSlot.Q,
                Range = 650,
                Width = 0,
                Speed = 1500,
                Delay = 0.5,
                Type = CastType.EnemyCast,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Kayle",
                Slot = SpellSlot.W,
                Range = 900,
                Width = 0,
                Speed = int.MaxValue,
                Delay = 0.22,
                Type = CastType.TeamCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false,

            });

            Database.Add(new OracleLib
            {
                Name = "Kayle",
                Slot = SpellSlot.E,
                Range = 0,
                Width = 0,
                Speed = 779,
                Delay = 0.5,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Kayle",
                Slot = SpellSlot.R,
                Range = 900,
                Width = 0,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.TeamCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false,

            });

            Database.Add(new OracleLib
            {
                Name = "Kennen",
                Slot = SpellSlot.Q,
                Range = 1000,
                Width = 50,
                Speed = 1700,
                Delay = 0.69,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = true
            });

            Database.Add(new OracleLib
            {
                Name = "Kennen",
                Slot = SpellSlot.W,
                Range = 900,
                Width = 900,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Kennen",
                Slot = SpellSlot.E,
                Range = 0,
                Width = 0,
                Speed = int.MaxValue,
                Delay = 0,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Kennen",
                Slot = SpellSlot.R,
                Range = 550,
                Width = 550,
                Speed = 779,
                Delay = 0.5,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Khazix",
                Slot = SpellSlot.Q,
                Range = 325,
                Width = 0,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.EnemyCast,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Khazix",
                Slot = SpellSlot.W,
                Range = 1000,
                Width = 60,
                Speed = 828.5,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = true
            });

            Database.Add(new OracleLib
            {
                Name = "Khazix",
                Slot = SpellSlot.E,
                Range = 600,
                Width = 300,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Khazix",
                Slot = SpellSlot.R,
                Range = 0,
                Width = 0,
                Speed = int.MaxValue,
                Delay = 0,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Khazix",
                Slot = SpellSlot.Q,
                Range = 375,
                Width = 0,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.EnemyCast,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Khazix",
                Slot = SpellSlot.W,
                Range = 1000,
                Width = 250,
                Speed = 828.5,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = true
            });

            Database.Add(new OracleLib
            {
                Name = "Khazix",
                Slot = SpellSlot.E,
                Range = 900,
                Width = 300,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Khazix",
                Slot = SpellSlot.R,
                Range = 0,
                Width = 0,
                Speed = int.MaxValue,
                Delay = 0,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "KogMaw",
                Slot = SpellSlot.Q,
                Range = 625,
                Width = 0,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.EnemyCast,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "KogMaw",
                Slot = SpellSlot.W,
                Range = 130,
                Width = 0,
                Speed = 2000,
                Delay = 0.5,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "KogMaw",
                Slot = SpellSlot.E,
                Range = 1000,
                Width = 120,
                Speed = 1200,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = true
            });

            Database.Add(new OracleLib
            {
                Name = "KogMaw",
                Slot = SpellSlot.R,
                Range = 1400,
                Width = 225,
                Speed = 2000,
                Delay = 0.6,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Leblanc",
                Slot = SpellSlot.Q,
                Range = 700,
                Width = 0,
                Speed = 2000,
                Delay = 0.5,
                Type = CastType.EnemyCast,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Leblanc",
                Slot = SpellSlot.W,
                Range = 600,
                Width = 220,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Leblanc",
                Slot = SpellSlot.W,
                Range = 0,
                Width = 0,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Leblanc",
                Slot = SpellSlot.E,
                Range = 925,
                Width = 70,
                Speed = 1600,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = true
            });

            Database.Add(new OracleLib
            {
                Name = "Leblanc",
                Slot = SpellSlot.R,
                Range = 700,
                Width = 0,
                Speed = 2000,
                Delay = 0.5,
                Type = CastType.EnemyCast,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Leblanc",
                Slot = SpellSlot.R,
                Range = 600,
                Width = 220,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Leblanc",
                Slot = SpellSlot.R,
                Range = 0,
                Width = 0,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Leblanc",
                Slot = SpellSlot.R,
                Range = 925,
                Width = 70,
                Speed = 1600,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = true
            });

            Database.Add(new OracleLib
            {
                Name = "LeeSin",
                Slot = SpellSlot.Q,
                Range = 1000,
                Width = 60,
                Speed = 1800,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = true
            });

            Database.Add(new OracleLib
            {
                Name = "LeeSin",
                Slot = SpellSlot.W,
                Range = 700,
                Width = 0,
                Speed = 1500,
                Delay = 0,
                Type = CastType.TeamCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false,

            });

            Database.Add(new OracleLib
            {
                Name = "LeeSin",
                Slot = SpellSlot.E,
                Range = 425,
                Width = 425,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "LeeSin",
                Slot = SpellSlot.R,
                Range = 375,
                Width = 0,
                Speed = 1500,
                Delay = 0.5,
                Type = CastType.EnemyCast,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "LeeSin",
                Slot = SpellSlot.Q,
                Range = 0,
                Width = 0,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.EnemyCast,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = true
            });

            Database.Add(new OracleLib
            {
                Name = "LeeSin",
                Slot = SpellSlot.W,
                Range = 700,
                Width = 0,
                Speed = int.MaxValue,
                Delay = 0,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "LeeSin",
                Slot = SpellSlot.E,
                Range = 425,
                Width = 425,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.High,
                CC = true,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Leona",
                Slot = SpellSlot.Q,
                Range = 215,
                Width = 0,
                Speed = 0,
                Delay = 0,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = true,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Leona",
                Slot = SpellSlot.W,
                Range = 500,
                Width = 0,
                Speed = 0,
                Delay = 3,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = true,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Leona",
                Slot = SpellSlot.E,
                Range = 900,
                Width = 100,
                Speed = 2000,
                Delay = 0,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = true
            });

            Database.Add(new OracleLib
            {
                Name = "Leona",
                Slot = SpellSlot.E,
                Range = 900,
                Width = 100,
                Speed = 2000,
                Delay = 0,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = true
            });

            Database.Add(new OracleLib
            {
                Name = "Leona",
                Slot = SpellSlot.R,
                Range = 1200,
                Width = 315,
                Speed = int.MaxValue,
                Delay = 0.7,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Lissandra",
                Slot = SpellSlot.Q,
                Range = 725,
                Width = 75,
                Speed = 1200,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = true
            });

            Database.Add(new OracleLib
            {
                Name = "Lissandra",
                Slot = SpellSlot.W,
                Range = 450,
                Width = 450,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Lissandra",
                Slot = SpellSlot.E,
                Range = 1050,
                Width = 110,
                Speed = 850,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = true
            });

            Database.Add(new OracleLib
            {
                Name = "Lissandra",
                Slot = SpellSlot.R,
                Range = 550,
                Width = 0,
                Speed = int.MaxValue,
                Delay = 0,
                Type = CastType.EnemyCast,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = false,
                Timer = 0,

            });

            Database.Add(new OracleLib
            {
                Name = "Lucian",
                Slot = SpellSlot.Q,
                Range = 550,
                Width = 65,
                Speed = 500,
                Delay = 0.5,
                Type = CastType.EnemyCast,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Lucian",
                Slot = SpellSlot.W,
                Range = 1000,
                Width = 80,
                Speed = 500,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = true
            });

            Database.Add(new OracleLib
            {
                Name = "Lucian",
                Slot = SpellSlot.E,
                Range = 650,
                Width = 50,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false,
                OnHit = true
            });

            Database.Add(new OracleLib
            {
                Name = "Lucian",
                Slot = SpellSlot.R,
                Range = 1400,
                Width = 60,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.TeamCast,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = true
            });

            Database.Add(new OracleLib
            {
                Name = "Lulu",
                Slot = SpellSlot.Q,
                Range = 925,
                Width = 80,
                Speed = 1400,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = true
            });

            Database.Add(new OracleLib
            {
                Name = "Lulu",
                Slot = SpellSlot.Q,
                Range = 925,
                Width = 80,
                Speed = 1400,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = true
            });

            Database.Add(new OracleLib
            {
                Name = "Lulu",
                Slot = SpellSlot.W,
                Range = 650,
                Width = 0,
                Speed = 2000,
                Delay = 0.64,
                Type = CastType.EnemyCast,
                DangerLevel = RiskLevel.High,
                CC = true,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Lulu",
                Slot = SpellSlot.E,
                Range = 650,
                Width = 0,
                Speed = int.MaxValue,
                Delay = 0.64,
                Type = CastType.Always,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false,

            });

            Database.Add(new OracleLib
            {
                Name = "Lulu",
                Slot = SpellSlot.R,
                Range = 900,
                Width = 0,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.TeamCast,
                DangerLevel = RiskLevel.High,
                CC = true,
                CheckLine = false,

            });

            Database.Add(new OracleLib
            {
                Name = "Lux",
                Slot = SpellSlot.Q,
                Range = 1300,
                Width = 80,
                Speed = 1200,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = true
            });

            Database.Add(new OracleLib
            {
                Name = "Lux",
                Slot = SpellSlot.W,
                Range = 1075,
                Width = 150,
                Speed = 1200,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false,

            });

            Database.Add(new OracleLib
            {
                Name = "Lux",
                Slot = SpellSlot.E,
                Range = 1100,
                Width = 275,
                Speed = 1300,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.High,
                CC = true,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Lux",
                Slot = SpellSlot.E,
                Range = 1100,
                Width = 275,
                Speed = 1300,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Lux",
                Slot = SpellSlot.R,
                Range = 3340,
                Width = 190,
                Speed = 3000,
                Delay = 1.75,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Lux",
                Slot = SpellSlot.R,
                Range = 3340,
                Width = 190,
                Speed = 3000,
                Delay = 1.75,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Malphite",
                Slot = SpellSlot.Q,
                Range = 625,
                Width = 0,
                Speed = 1200,
                Delay = 0.5,
                Type = CastType.EnemyCast,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Malphite",
                Slot = SpellSlot.W,
                Range = 0,
                Width = 0,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Malphite",
                Slot = SpellSlot.E,
                Range = 400,
                Width = 400,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Malphite",
                Slot = SpellSlot.R,
                Range = 1000,
                Width = 270,
                Speed = 700,
                Delay = 0,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = false,
                Timer = 0
            });

            Database.Add(new OracleLib
            {
                Name = "Malzahar",
                Slot = SpellSlot.Q,
                Range = 900,
                Width = 110,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Malzahar",
                Slot = SpellSlot.W,
                Range = 800,
                Width = 250,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Malzahar",
                Slot = SpellSlot.E,
                Range = 650,
                Width = 0,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.EnemyCast,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Malzahar",
                Slot = SpellSlot.R,
                Range = 700,
                Width = 0,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.EnemyCast,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Maokai",
                Slot = SpellSlot.Q,
                Range = 600,
                Width = 110,
                Speed = 1200,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = true
            });

            Database.Add(new OracleLib
            {
                Name = "Maokai",
                Slot = SpellSlot.W,
                Range = 650,
                Width = 0,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.EnemyCast,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Maokai",
                Slot = SpellSlot.E,
                Range = 1100,
                Width = 250,
                Speed = 1750,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Maokai",
                Slot = SpellSlot.R,
                Range = 625,
                Width = 575,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "MasterYi",
                Slot = SpellSlot.Q,
                Range = 600,
                Width = 0,
                Speed = 4000,
                Delay = 0.5,
                Type = CastType.EnemyCast,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false,
                OnHit = true
            });

            Database.Add(new OracleLib
            {
                Name = "MasterYi",
                Slot = SpellSlot.W,
                Range = 0,
                Width = 0,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "MasterYi",
                Slot = SpellSlot.E,
                Range = 0,
                Width = 0,
                Speed = int.MaxValue,
                Delay = 0.23,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "MasterYi",
                Slot = SpellSlot.R,
                Range = 0,
                Width = 0,
                Speed = int.MaxValue,
                Delay = 0.37,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false,

            });

            Database.Add(new OracleLib
            {
                Name = "MissFortune",
                Slot = SpellSlot.Q,
                Range = 650,
                Width = 0,
                Speed = 1400,
                Delay = 0.5,
                Type = CastType.EnemyCast,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false,
                OnHit = true
            });

            Database.Add(new OracleLib
            {
                Name = "MissFortune",
                Slot = SpellSlot.W,
                Range = 0,
                Width = 0,
                Speed = int.MaxValue,
                Delay = 0,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "MissFortune",
                Slot = SpellSlot.E,
                Range = 1000,
                Width = 400,
                Speed = 500,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "MissFortune",
                Slot = SpellSlot.R,
                Range = 1400,
                Width = 100,
                Speed = 775,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Extreme,
                CC = false,
                CheckLine = true
            });

            Database.Add(new OracleLib
            {
                Name = "Mordekaiser",
                Slot = SpellSlot.Q,
                Range = 600,
                Width = 0,
                Speed = 1500,
                Delay = 0.5,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Mordekaiser",
                Slot = SpellSlot.W,
                Range = 750,
                Width = 0,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.TeamCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Mordekaiser",
                Slot = SpellSlot.E,
                Range = 700,
                Width = 0,
                Speed = 1500,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = true
            });

            Database.Add(new OracleLib
            {
                Name = "Mordekaiser",
                Slot = SpellSlot.R,
                Range = 850,
                Width = 0,
                Speed = 1500,
                Delay = 0.5,
                Type = CastType.EnemyCast,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Morgana",
                Slot = SpellSlot.Q,
                Range = 1300,
                Width = 110,
                Speed = 1200,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = true
            });

            Database.Add(new OracleLib
            {
                Name = "Morgana",
                Slot = SpellSlot.W,
                Range = 1075,
                Width = 350,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Morgana",
                Slot = SpellSlot.E,
                Range = 750,
                Width = 0,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.TeamCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false,

            });

            Database.Add(new OracleLib
            {
                Name = "Morgana",
                Slot = SpellSlot.R,
                Range = 600,
                Width = 600,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = true,
                Timer = 2800
            });

            Database.Add(new OracleLib
            {
                Name = "Nami",
                Slot = SpellSlot.Q,
                Range = 875,
                Width = 200,
                Speed = 1750,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Nami",
                Slot = SpellSlot.W,
                Range = 725,
                Width = 0,
                Speed = 1100,
                Delay = 0.5,
                Type = CastType.Always,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false,

            });

            Database.Add(new OracleLib
            {
                Name = "Nami",
                Slot = SpellSlot.E,
                Range = 800,
                Width = 0,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.TeamCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Nami",
                Slot = SpellSlot.R,
                Range = 2550,
                Width = 600,
                Speed = 1200,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = true
            });

            Database.Add(new OracleLib
            {
                Name = "Nasus",
                Slot = SpellSlot.Q,
                Range = 0,
                Width = 0,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false,
                OnHit = true
            });

            Database.Add(new OracleLib
            {
                Name = "Nasus",
                Slot = SpellSlot.W,
                Range = 600,
                Width = 0,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.EnemyCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = true,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Nasus",
                Slot = SpellSlot.E,
                Range = 850,
                Width = 400,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Nasus",
                Slot = SpellSlot.R,
                Range = 1,
                Width = 350,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Nautilus",
                Slot = SpellSlot.Q,
                Range = 950,
                Width = 80,
                Speed = 1200,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = true
            });

            Database.Add(new OracleLib
            {
                Name = "Nautilus",
                Slot = SpellSlot.W,
                Range = 0,
                Width = 0,
                Speed = 0,
                Delay = 0,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false,

            });

            Database.Add(new OracleLib
            {
                Name = "Nautilus",
                Slot = SpellSlot.E,
                Range = 600,
                Width = 600,
                Speed = 1300,
                Delay = 0.5,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Nautilus",
                Slot = SpellSlot.R,
                Range = 1500,
                Width = 60,
                Speed = 1400,
                Delay = 0.5,
                Type = CastType.EnemyCast,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = false,
                Timer = 450 - Game.Ping
            });

            Database.Add(new OracleLib
            {
                Name = "Nidalee",
                Slot = SpellSlot.Q,
                Range = 1500,
                Width = 60,
                Speed = 1300,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = true
            });

            Database.Add(new OracleLib
            {
                Name = "Nidalee",
                Slot = SpellSlot.W,
                Range = 900,
                Width = 125,
                Speed = 1450,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Nidalee",
                Slot = SpellSlot.E,
                Range = 600,
                Width = 0,
                Speed = int.MaxValue,
                Delay = 0,
                Type = CastType.TeamCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false,

            });

            Database.Add(new OracleLib
            {
                Name = "Nidalee",
                Slot = SpellSlot.R,
                Range = 0,
                Width = 0,
                Speed = int.MaxValue,
                Delay = 0,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Nidalee",
                Slot = SpellSlot.Q + 14,
                Range = 50,
                Width = 0,
                Speed = 500,
                Delay = 0,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Nidalee",
                Slot = SpellSlot.W + 14,
                Range = 375,
                Width = 150,
                Speed = 1500,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Nidalee",
                Slot = SpellSlot.E + 14,
                Range = 300,
                Width = 300,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Nocturne",
                Slot = SpellSlot.Q,
                Range = 1125,
                Width = 60,
                Speed = 1600,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = true
            });

            Database.Add(new OracleLib
            {
                Name = "Nocturne",
                Slot = SpellSlot.W,
                Range = 0,
                Width = 0,
                Speed = 500,
                Delay = 0.5,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false,

            });

            Database.Add(new OracleLib
            {
                Name = "Nocturne",
                Slot = SpellSlot.E,
                Range = 500,
                Width = 0,
                Speed = 0,
                Delay = 0.5,
                Type = CastType.EnemyCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = true,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Nocturne",
                Slot = SpellSlot.R,
                Range = 2000,
                Width = 0,
                Speed = 500,
                Delay = 0.5,
                Type = CastType.EnemyCast,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Nunu",
                Slot = SpellSlot.Q,
                Range = 125,
                Width = 60,
                Speed = 1400,
                Delay = 0.5,
                Type = CastType.EnemyCast,
                DangerLevel = RiskLevel.Extreme,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Nunu",
                Slot = SpellSlot.W,
                Range = 700,
                Width = 0,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.TeamCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Nunu",
                Slot = SpellSlot.E,
                Range = 550,
                Width = 0,
                Speed = 1000,
                Delay = 0.5,
                Type = CastType.EnemyCast,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Nunu",
                Slot = SpellSlot.R,
                Range = 650,
                Width = 650,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Olaf",
                Slot = SpellSlot.Q,
                Range = 1000,
                Width = 90,
                Speed = 1600,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = true
            });

            Database.Add(new OracleLib
            {
                Name = "Olaf",
                Slot = SpellSlot.W,
                Range = 0,
                Width = 0,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Olaf",
                Slot = SpellSlot.E,
                Range = 325,
                Width = 0,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.EnemyCast,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Olaf",
                Slot = SpellSlot.R,
                Range = 0,
                Width = 0,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false,

            });

            Database.Add(new OracleLib
            {
                Name = "Orianna",
                Slot = SpellSlot.Q,
                Range = 1100,
                Width = 145,
                Speed = 1200,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = true
            });

            Database.Add(new OracleLib
            {
                Name = "Orianna",
                Slot = SpellSlot.W,
                Range = 0,
                Width = 260,
                Speed = 1200,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Orianna",
                Slot = SpellSlot.E,
                Range = 1095,
                Width = 145,
                Speed = 1200,
                Delay = 0.5,
                Type = CastType.TeamCast,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false,

            });

            Database.Add(new OracleLib
            {
                Name = "Orianna",
                Slot = SpellSlot.R,
                Range = 0,
                Width = 425,
                Speed = 1200,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Pantheon",
                Slot = SpellSlot.Q,
                Range = 600,
                Width = 0,
                Speed = 1500,
                Delay = 0.5,
                Type = CastType.EnemyCast,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Pantheon",
                Slot = SpellSlot.W,
                Range = 600,
                Width = 0,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.EnemyCast,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Pantheon",
                Slot = SpellSlot.E,
                Range = 600,
                Width = 100,
                Speed = 775,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = true
            });

            Database.Add(new OracleLib
            {
                Name = "Pantheon",
                Slot = SpellSlot.R,
                Range = 5500,
                Width = 1000,
                Speed = 3000,
                Delay = 1,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Pantheon",
                Slot = SpellSlot.R,
                Range = 5500,
                Width = 1000,
                Speed = 3000,
                Delay = 1,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Poppy",
                Slot = SpellSlot.Q,
                Range = 0,
                Width = 0,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Poppy",
                Slot = SpellSlot.W,
                Range = 0,
                Width = 0,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Poppy",
                Slot = SpellSlot.E,
                Range = 525,
                Width = 0,
                Speed = 1450,
                Delay = 0.5,
                Type = CastType.EnemyCast,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Poppy",
                Slot = SpellSlot.R,
                Range = 900,
                Width = 0,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.EnemyCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Quinn",
                Slot = SpellSlot.Q,
                Range = 1025,
                Width = 80,
                Speed = 1200,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = true
            });

            Database.Add(new OracleLib
            {
                Name = "Quinn",
                Slot = SpellSlot.W,
                Range = 2100,
                Width = 0,
                Speed = 0,
                Delay = 0,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Quinn",
                Slot = SpellSlot.E,
                Range = 700,
                Width = 0,
                Speed = 775,
                Delay = 0.5,
                Type = CastType.EnemyCast,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Quinn",
                Slot = SpellSlot.R,
                Range = 0,
                Width = 0,
                Speed = 0,
                Delay = 0,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Quinn",
                Slot = SpellSlot.R,
                Range = 700,
                Width = 700,
                Speed = 0,
                Delay = 0,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Rammus",
                Slot = SpellSlot.Q,
                Range = 0,
                Width = 200,
                Speed = 775,
                Delay = 0.5,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.Extreme,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Rammus",
                Slot = SpellSlot.W,
                Range = 0,
                Width = 0,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false,

            });

            Database.Add(new OracleLib
            {
                Name = "Rammus",
                Slot = SpellSlot.E,
                Range = 325,
                Width = 0,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.EnemyCast,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Rammus",
                Slot = SpellSlot.R,
                Range = 300,
                Width = 300,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Renekton",
                Slot = SpellSlot.Q,
                Range = 1,
                Width = 450,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false,
                OnHit = true
            });

            Database.Add(new OracleLib
            {
                Name = "Renekton",
                Slot = SpellSlot.W,
                Range = 0,
                Width = 0,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Renekton",
                Slot = SpellSlot.E,
                Range = 450,
                Width = 50,
                Speed = 1400,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = true
            });

            Database.Add(new OracleLib
            {
                Name = "Renekton",
                Slot = SpellSlot.R,
                Range = 1,
                Width = 530,
                Speed = 775,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Rengar",
                Slot = SpellSlot.Q,
                Range = 0,
                Width = 0,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false,
                OnHit = true
            });

            Database.Add(new OracleLib
            {
                Name = "Rengar",
                Slot = SpellSlot.W,
                Range = 1,
                Width = 500,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Rengar",
                Slot = SpellSlot.E,
                Range = 1000,
                Width = 70,
                Speed = 1500,
                Delay = 0.5,
                Type = CastType.EnemyCast,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Rengar",
                Slot = SpellSlot.R,
                Range = 0,
                Width = 0,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Riven",
                Slot = SpellSlot.Q,
                Range = 250,
                Width = 0,
                Speed = 0,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Riven",
                Slot = SpellSlot.Q,
                Range = 250,
                Width = 0,
                Speed = 0,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Riven",
                Slot = SpellSlot.W,
                Range = 260,
                Width = 260,
                Speed = 1500,
                Delay = 0.25,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Riven",
                Slot = SpellSlot.E,
                Range = 325,
                Width = 0,
                Speed = 1450,
                Delay = 0,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false,

            });

            Database.Add(new OracleLib
            {
                Name = "Riven",
                Slot = SpellSlot.R,
                Range = 0,
                Width = 0,
                Speed = 1200,
                Delay = 0.5,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Riven",
                Slot = SpellSlot.R,
                Range = 900,
                Width = 200,
                Speed = 1450,
                Delay = 0.3,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = true
            });

            Database.Add(new OracleLib
            {
                Name = "Rumble",
                Slot = SpellSlot.Q,
                Range = 600,
                Width = 10,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = true
            });

            Database.Add(new OracleLib
            {
                Name = "Rumble",
                Slot = SpellSlot.W,
                Range = 0,
                Width = 0,
                Speed = 0,
                Delay = 0,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false,

            });

            Database.Add(new OracleLib
            {
                Name = "Rumble",
                Slot = SpellSlot.E,
                Range = 850,
                Width = 90,
                Speed = 1200,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = true
            });

            Database.Add(new OracleLib
            {
                Name = "Rumble",
                Slot = SpellSlot.R,
                Range = 1700,
                Width = 0,
                Speed = 1400,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = true
            });

            Database.Add(new OracleLib
            {
                Name = "Ryze",
                Slot = SpellSlot.Q,
                Range = 625,
                Width = 0,
                Speed = 1400,
                Delay = 0.5,
                Type = CastType.EnemyCast,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Ryze",
                Slot = SpellSlot.W,
                Range = 600,
                Width = 0,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.EnemyCast,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Ryze",
                Slot = SpellSlot.E,
                Range = 600,
                Width = 0,
                Speed = 1000,
                Delay = 0.5,
                Type = CastType.EnemyCast,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Ryze",
                Slot = SpellSlot.R,
                Range = 625,
                Width = 0,
                Speed = 1400,
                Delay = 0.5,
                Type = CastType.EnemyCast,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Sejuani",
                Slot = SpellSlot.Q,
                Range = 650,
                Width = 75,
                Speed = 1450,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = true
            });

            Database.Add(new OracleLib
            {
                Name = "Sejuani",
                Slot = SpellSlot.W,
                Range = 1,
                Width = 350,
                Speed = 1500,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Sejuani",
                Slot = SpellSlot.E,
                Range = 1,
                Width = 1000,
                Speed = 1450,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Sejuani",
                Slot = SpellSlot.R,
                Range = 1175,
                Width = 110,
                Speed = 1400,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = true
            });

            Database.Add(new OracleLib
            {
                Name = "Shaco",
                Slot = SpellSlot.Q,
                Range = 400,
                Width = 0,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Shaco",
                Slot = SpellSlot.W,
                Range = 425,
                Width = 60,
                Speed = 1450,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Shaco",
                Slot = SpellSlot.E,
                Range = 625,
                Width = 0,
                Speed = 1500,
                Delay = 0.5,
                Type = CastType.EnemyCast,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Shaco",
                Slot = SpellSlot.R,
                Range = 1125,
                Width = 250,
                Speed = 395,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Shen",
                Slot = SpellSlot.Q,
                Range = 475,
                Width = 0,
                Speed = 1500,
                Delay = 0.5,
                Type = CastType.EnemyCast,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Shen",
                Slot = SpellSlot.W,
                Range = 0,
                Width = 0,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false,

            });

            Database.Add(new OracleLib
            {
                Name = "Shen",
                Slot = SpellSlot.E,
                Range = 600,
                Width = 50,
                Speed = 1000,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = true
            });

            Database.Add(new OracleLib
            {
                Name = "Shen",
                Slot = SpellSlot.R,
                Range = 25000,
                Width = 0,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.TeamCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false,

            });

            Database.Add(new OracleLib
            {
                Name = "Shyvana",
                Slot = SpellSlot.Q,
                Range = 0,
                Width = 0,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false,
                OnHit = true
            });

            Database.Add(new OracleLib
            {
                Name = "Shyvana",
                Slot = SpellSlot.W,
                Range = 0,
                Width = 325,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Shyvana",
                Slot = SpellSlot.W,
                Range = 0,
                Width = 325,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Shyvana",
                Slot = SpellSlot.E,
                Range = 925,
                Width = 60,
                Speed = 1200,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.High,
                CC = false,
                CheckLine = true
            });

            Database.Add(new OracleLib
            {
                Name = "Shyvana",
                Slot = SpellSlot.E,
                Range = 925,
                Width = 60,
                Speed = 1200,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.High,
                CC = false,
                CheckLine = true
            });

            Database.Add(new OracleLib
            {
                Name = "Shyvana",
                Slot = SpellSlot.R,
                Range = 1000,
                Width = 160,
                Speed = 700,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.High,
                CC = true,
                CheckLine = true
            });

            Database.Add(new OracleLib
            {
                Name = "Singed",
                Slot = SpellSlot.Q,
                Range = 0,
                Width = 400,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Singed",
                Slot = SpellSlot.W,
                Range = 1175,
                Width = 350,
                Speed = 700,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.High,
                CC = true,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Singed",
                Slot = SpellSlot.E,
                Range = 125,
                Width = 0,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.EnemyCast,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Singed",
                Slot = SpellSlot.R,
                Range = 0,
                Width = 0,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Sion",
                Slot = SpellSlot.Q,
                Range = 550,
                Width = 0,
                Speed = 1600,
                Delay = 0.5,
                Type = CastType.EnemyCast,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Sion",
                Slot = SpellSlot.W,
                Range = 550,
                Width = 550,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false,

            });

            Database.Add(new OracleLib
            {
                Name = "Sion",
                Slot = SpellSlot.W,
                Range = 550,
                Width = 550,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Sion",
                Slot = SpellSlot.E,
                Range = 0,
                Width = 0,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Sion",
                Slot = SpellSlot.R,
                Range = 0,
                Width = 0,
                Speed = 500,
                Delay = 0.5,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Sivir",
                Slot = SpellSlot.Q,
                Range = 1165,
                Width = 90,
                Speed = 1350,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = true
            });

            Database.Add(new OracleLib
            {
                Name = "Sivir",
                Slot = SpellSlot.W,
                Range = 565,
                Width = 0,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.EnemyCast,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false,
                OnHit = true
            });

            Database.Add(new OracleLib
            {
                Name = "Sivir",
                Slot = SpellSlot.E,
                Range = 0,
                Width = 0,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false,

            });

            Database.Add(new OracleLib
            {
                Name = "Sivir",
                Slot = SpellSlot.R,
                Range = 1000,
                Width = 1000,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Skarner",
                Slot = SpellSlot.Q,
                Range = 350,
                Width = 350,
                Speed = int.MaxValue,
                Delay = 0,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Skarner",
                Slot = SpellSlot.W,
                Range = 0,
                Width = 0,
                Speed = int.MaxValue,
                Delay = 0,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false,

            });

            Database.Add(new OracleLib
            {
                Name = "Skarner",
                Slot = SpellSlot.E,
                Range = 1000,
                Width = 60,
                Speed = 1200,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = true
            });

            Database.Add(new OracleLib
            {
                Name = "Skarner",
                Slot = SpellSlot.E,
                Range = 1000,
                Width = 60,
                Speed = 1200,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = true
            });

            Database.Add(new OracleLib
            {
                Name = "Skarner",
                Slot = SpellSlot.R,
                Range = 350,
                Width = 0,
                Speed = int.MaxValue,
                Delay = 0,
                Type = CastType.EnemyCast,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Sona",
                Slot = SpellSlot.Q,
                Range = 700,
                Width = 700,
                Speed = 1500,
                Delay = 0.5,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Sona",
                Slot = SpellSlot.W,
                Range = 1000,
                Width = 1000,
                Speed = 1500,
                Delay = 0.5,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false,

            });

            Database.Add(new OracleLib
            {
                Name = "Sona",
                Slot = SpellSlot.E,
                Range = 1000,
                Width = 1000,
                Speed = 1500,
                Delay = 0.5,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Sona",
                Slot = SpellSlot.R,
                Range = 900,
                Width = 125,
                Speed = 2400,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = true,
                Timer = 0
            });

            Database.Add(new OracleLib
            {
                Name = "Soraka",
                Slot = SpellSlot.Q,
                Range = 675,
                Width = 675,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Soraka",
                Slot = SpellSlot.W,
                Range = 750,
                Width = 0,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.TeamCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false,

            });

            Database.Add(new OracleLib
            {
                Name = "Soraka",
                Slot = SpellSlot.E,
                Range = 725,
                Width = 0,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.Always,
                DangerLevel = RiskLevel.High,
                CC = false,
                CheckLine = false,

            });

            Database.Add(new OracleLib
            {
                Name = "Soraka",
                Slot = SpellSlot.R,
                Range = 25000,
                Width = 0,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false,

            });

            Database.Add(new OracleLib
            {
                Name = "Swain",
                Slot = SpellSlot.Q,
                Range = 625,
                Width = 0,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.EnemyCast,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Swain",
                Slot = SpellSlot.W,
                Range = 1040,
                Width = 275,
                Speed = 1250,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Swain",
                Slot = SpellSlot.E,
                Range = 625,
                Width = 0,
                Speed = 1400,
                Delay = 0.5,
                Type = CastType.EnemyCast,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Swain",
                Slot = SpellSlot.R,
                Range = 700,
                Width = 700,
                Speed = 950,
                Delay = 0.5,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Syndra",
                Slot = SpellSlot.Q,
                Range = 800,
                Width = 200,
                Speed = 1750,
                Delay = 0.25,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Syndra",
                Slot = SpellSlot.W,
                Range = 925,
                Width = 200,
                Speed = 1450,
                Delay = 0.5,
                Type = CastType.EnemyCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Syndra",
                Slot = SpellSlot.W,
                Range = 950,
                Width = 200,
                Speed = 1450,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Syndra",
                Slot = SpellSlot.E,
                Range = 700,
                Width = 0,
                Speed = 902,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = true
            });

            Database.Add(new OracleLib
            {
                Name = "Syndra",
                Slot = SpellSlot.R,
                Range = 675,
                Width = 0,
                Speed = 1100,
                Delay = 0.5,
                Type = CastType.EnemyCast,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Talon",
                Slot = SpellSlot.Q,
                Range = 0,
                Width = 0,
                Speed = 0,
                Delay = 0,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false,
                OnHit = true
            });

            Database.Add(new OracleLib
            {
                Name = "Talon",
                Slot = SpellSlot.W,
                Range = 750,
                Width = 0,
                Speed = 1200,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = true
            });

            Database.Add(new OracleLib
            {
                Name = "Talon",
                Slot = SpellSlot.E,
                Range = 750,
                Width = 0,
                Speed = 1200,
                Delay = 0,
                Type = CastType.EnemyCast,
                DangerLevel = RiskLevel.High,
                CC = true,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Talon",
                Slot = SpellSlot.R,
                Range = 750,
                Width = 0,
                Speed = 0,
                Delay = 0,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Taric",
                Slot = SpellSlot.Q,
                Range = 750,
                Width = 0,
                Speed = 1200,
                Delay = 0.5,
                Type = CastType.TeamCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false,

            });

            Database.Add(new OracleLib
            {
                Name = "Taric",
                Slot = SpellSlot.W,
                Range = 400,
                Width = 200,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Taric",
                Slot = SpellSlot.E,
                Range = 625,
                Width = 0,
                Speed = 1400,
                Delay = 0.5,
                Type = CastType.EnemyCast,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Taric",
                Slot = SpellSlot.R,
                Range = 400,
                Width = 200,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Teemo",
                Slot = SpellSlot.Q,
                Range = 580,
                Width = 0,
                Speed = 1500,
                Delay = 0.5,
                Type = CastType.EnemyCast,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Teemo",
                Slot = SpellSlot.W,
                Range = 0,
                Width = 0,
                Speed = 943,
                Delay = 0,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Teemo",
                Slot = SpellSlot.E,
                Range = 0,
                Width = 0,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Teemo",
                Slot = SpellSlot.R,
                Range = 230,
                Width = 0,
                Speed = 1500,
                Delay = 0,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Normal,
                CC = true,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Thresh",
                Slot = SpellSlot.Q,
                Range = 1175,
                Width = 60,
                Speed = 1200,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = true
            });

            Database.Add(new OracleLib
            {
                Name = "Thresh",
                Slot = SpellSlot.W,
                Range = 950,
                Width = 315,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false,

            });

            Database.Add(new OracleLib
            {
                Name = "Thresh",
                Slot = SpellSlot.E,
                Range = 515,
                Width = 160,
                Speed = int.MaxValue,
                Delay = 0.3,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = true
            });

            Database.Add(new OracleLib
            {
                Name = "Thresh",
                Slot = SpellSlot.R,
                Range = 420,
                Width = 420,
                Speed = int.MaxValue,
                Delay = 0.3,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Tristana",
                Slot = SpellSlot.Q,
                Range = 0,
                Width = 0,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Tristana",
                Slot = SpellSlot.W,
                Range = 900,
                Width = 270,
                Speed = 1150,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Tristana",
                Slot = SpellSlot.E,
                Range = 625,
                Width = 0,
                Speed = 1400,
                Delay = 0.5,
                Type = CastType.EnemyCast,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Tristana",
                Slot = SpellSlot.R,
                Range = 700,
                Width = 0,
                Speed = 1600,
                Delay = 0.5,
                Type = CastType.EnemyCast,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Trundle",
                Slot = SpellSlot.Q,
                Range = 0,
                Width = 0,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.EnemyCast,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Trundle",
                Slot = SpellSlot.W,
                Range = 0,
                Width = 900,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.NoDamage,
                CC = true,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Trundle",
                Slot = SpellSlot.E,
                Range = 1100,
                Width = 188,
                Speed = 1600,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.High,
                CC = true,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Trundle",
                Slot = SpellSlot.R,
                Range = 700,
                Width = 0,
                Speed = 1400,
                Delay = 0.5,
                Type = CastType.EnemyCast,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Tryndamere",
                Slot = SpellSlot.Q,
                Range = 0,
                Width = 0,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Tryndamere",
                Slot = SpellSlot.W,
                Range = 400,
                Width = 400,
                Speed = 500,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.High,
                CC = true,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Tryndamere",
                Slot = SpellSlot.E,
                Range = 660,
                Width = 225,
                Speed = 700,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Tryndamere",
                Slot = SpellSlot.R,
                Range = 0,
                Width = 0,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false,

            });

            Database.Add(new OracleLib
            {
                Name = "TwistedFate",
                Slot = SpellSlot.Q,
                Range = 1450,
                Width = 80,
                Speed = 1450,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = true
            });

            Database.Add(new OracleLib
            {
                Name = "TwistedFate",
                Slot = SpellSlot.W,
                Range = 0,
                Width = 0,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = true,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "TwistedFate",
                Slot = SpellSlot.W,
                Range = 600,
                Width = 0,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.EnemyCast,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "TwistedFate",
                Slot = SpellSlot.W,
                Range = 600,
                Width = 100,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.EnemyCast,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "TwistedFate",
                Slot = SpellSlot.W,
                Range = 600,
                Width = 0,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.EnemyCast,
                DangerLevel = RiskLevel.Normal,
                CC = true,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "TwistedFate",
                Slot = SpellSlot.E,
                Range = 525,
                Width = 0,
                Speed = 1200,
                Delay = 0.5,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "TwistedFate",
                Slot = SpellSlot.R,
                Range = 5500,
                Width = 0,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Twich",
                Slot = SpellSlot.Q,
                Range = 0,
                Width = 0,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Twich",
                Slot = SpellSlot.W,
                Range = 800,
                Width = 275,
                Speed = 1750,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Twich",
                Slot = SpellSlot.W,
                Range = 800,
                Width = 275,
                Speed = 1750,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Twich",
                Slot = SpellSlot.E,
                Range = 1200,
                Width = 0,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.EnemyCast,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Twich",
                Slot = SpellSlot.R,
                Range = 850,
                Width = 0,
                Speed = 500,
                Delay = 0.5,
                Type = CastType.EnemyCast,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Udyr",
                Slot = SpellSlot.Q,
                Range = 0,
                Width = 0,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Udyr",
                Slot = SpellSlot.W,
                Range = 0,
                Width = 0,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false,

            });

            Database.Add(new OracleLib
            {
                Name = "Udyr",
                Slot = SpellSlot.E,
                Range = 0,
                Width = 0,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = true,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Udyr",
                Slot = SpellSlot.R,
                Range = 0,
                Width = 0,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Urgot",
                Slot = SpellSlot.Q,
                Range = 1000,
                Width = 80,
                Speed = 1600,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Urgot",
                Slot = SpellSlot.Q,
                Range = 1000,
                Width = 80,
                Speed = 1600,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Urgot",
                Slot = SpellSlot.W,
                Range = 0,
                Width = 300,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false,

            });

            Database.Add(new OracleLib
            {
                Name = "Urgot",
                Slot = SpellSlot.E,
                Range = 950,
                Width = 150,
                Speed = 1750,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Urgot",
                Slot = SpellSlot.E,
                Range = 950,
                Width = 150,
                Speed = 1750,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Urgot",
                Slot = SpellSlot.R,
                Range = 850,
                Width = 0,
                Speed = 1800,
                Delay = 0.5,
                Type = CastType.EnemyCast,
                DangerLevel = RiskLevel.High,
                CC = true,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Varus",
                Slot = SpellSlot.Q,
                Range = 1500,
                Width = 100,
                Speed = 1500,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = true
            });

            Database.Add(new OracleLib
            {
                Name = "Varus",
                Slot = SpellSlot.W,
                Range = 0,
                Width = 0,
                Speed = 0,
                Delay = 0.5,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Varus",
                Slot = SpellSlot.E,
                Range = 925,
                Width = 55,
                Speed = 1500,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Varus",
                Slot = SpellSlot.R,
                Range = 1300,
                Width = 80,
                Speed = 1500,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = true
            });

            Database.Add(new OracleLib
            {
                Name = "Vayne",
                Slot = SpellSlot.Q,
                Range = 250,
                Width = 0,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Vayne",
                Slot = SpellSlot.W,
                Range = 0,
                Width = 0,
                Speed = int.MaxValue,
                Delay = 0,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Vayne",
                Slot = SpellSlot.E,
                Range = 450,
                Width = 0,
                Speed = 1200,
                Delay = 0.5,
                Type = CastType.EnemyCast,
                DangerLevel = RiskLevel.High,
                CC = true,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Vayne",
                Slot = SpellSlot.R,
                Range = 0,
                Width = 0,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Veigar",
                Slot = SpellSlot.Q,
                Range = 650,
                Width = 0,
                Speed = 1500,
                Delay = 0.5,
                Type = CastType.EnemyCast,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Veigar",
                Slot = SpellSlot.W,
                Range = 900,
                Width = 240,
                Speed = 1500,
                Delay = 1.2,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Veigar",
                Slot = SpellSlot.E,
                Range = 650,
                Width = 350,
                Speed = 1500,
                Delay = int.MaxValue,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.High,
                CC = true,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Veigar",
                Slot = SpellSlot.R,
                Range = 650,
                Width = 0,
                Speed = 1400,
                Delay = 0.5,
                Type = CastType.EnemyCast,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false,
                Timer = 230 - Game.Ping
            });

            Database.Add(new OracleLib
            {
                Name = "Velkoz",
                Slot = SpellSlot.Q,
                Range = 1050,
                Width = 60,
                Speed = 1200,
                Delay = 0.3,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = true
            });

            Database.Add(new OracleLib
            {
                Name = "Velkoz",
                Slot = SpellSlot.Q,
                Range = 1050,
                Width = 60,
                Speed = 1200,
                Delay = 0,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = true
            });

            Database.Add(new OracleLib
            {
                Name = "Velkoz",
                Slot = SpellSlot.Q,
                Range = 1050,
                Width = 60,
                Speed = 1200,
                Delay = 0.8,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = true
            });

            Database.Add(new OracleLib
            {
                Name = "Velkoz",
                Slot = SpellSlot.W,
                Range = 1050,
                Width = 90,
                Speed = 1200,
                Delay = 0,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = true
            });

            Database.Add(new OracleLib
            {
                Name = "Velkoz",
                Slot = SpellSlot.E,
                Range = 850,
                Width = 0,
                Speed = 500,
                Delay = 0,
                Type = CastType.EnemyCast,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Velkoz",
                Slot = SpellSlot.R,
                Range = 1575,
                Width = 0,
                Speed = 1500,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = true
            });

            Database.Add(new OracleLib
            {
                Name = "Vi",
                Slot = SpellSlot.Q,
                Range = 800,
                Width = 55,
                Speed = 1500,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = true
            });

            Database.Add(new OracleLib
            {
                Name = "Vi",
                Slot = SpellSlot.W,
                Range = 0,
                Width = 0,
                Speed = 0,
                Delay = 0,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Vi",
                Slot = SpellSlot.E,
                Range = 600,
                Width = 0,
                Speed = 0,
                Delay = 0,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = true
            });

            Database.Add(new OracleLib
            {
                Name = "Vi",
                Slot = SpellSlot.R,
                Range = 800,
                Width = 0,
                Speed = 0,
                Delay = 0.5,
                Type = CastType.EnemyCast,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = false,
                Timer = 230 - Game.Ping
            });

            Database.Add(new OracleLib
            {
                Name = "Viktor",
                Slot = SpellSlot.Q,
                Range = 600,
                Width = 0,
                Speed = 1400,
                Delay = 0.5,
                Type = CastType.EnemyCast,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Viktor",
                Slot = SpellSlot.W,
                Range = 815,
                Width = 300,
                Speed = 1750,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.High,
                CC = true,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Viktor",
                Slot = SpellSlot.E,
                Range = 700,
                Width = 90,
                Speed = 1210,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = true
            });

            Database.Add(new OracleLib
            {
                Name = "Viktor",
                Slot = SpellSlot.R,
                Range = 700,
                Width = 250,
                Speed = 1210,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Vladimir",
                Slot = SpellSlot.Q,
                Range = 600,
                Width = 0,
                Speed = 1400,
                Delay = 0.5,
                Type = CastType.EnemyCast,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Vladimir",
                Slot = SpellSlot.W,
                Range = 350,
                Width = 350,
                Speed = 1600,
                Delay = 0.5,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Vladimir",
                Slot = SpellSlot.E,
                Range = 610,
                Width = 610,
                Speed = 1100,
                Delay = 0.5,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Vladimir",
                Slot = SpellSlot.R,
                Range = 875,
                Width = 375,
                Speed = 1200,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Volibear",
                Slot = SpellSlot.Q,
                Range = 300,
                Width = 0,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Volibear",
                Slot = SpellSlot.W,
                Range = 400,
                Width = 0,
                Speed = 1450,
                Delay = 0.5,
                Type = CastType.EnemyCast,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Volibear",
                Slot = SpellSlot.E,
                Range = 425,
                Width = 425,
                Speed = 825,
                Delay = 0.5,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = true,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Volibear",
                Slot = SpellSlot.R,
                Range = 425,
                Width = 425,
                Speed = 825,
                Delay = 0,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Warwick",
                Slot = SpellSlot.Q,
                Range = 400,
                Width = 0,
                Speed = int.MaxValue,
                Delay = 0,
                Type = CastType.EnemyCast,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Warwick",
                Slot = SpellSlot.W,
                Range = 1000,
                Width = 0,
                Speed = int.MaxValue,
                Delay = 0,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Warwick",
                Slot = SpellSlot.E,
                Range = 1500,
                Width = 0,
                Speed = int.MaxValue,
                Delay = 0,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Warwick",
                Slot = SpellSlot.R,
                Range = 700,
                Width = 0,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.EnemyCast,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = false,
                OnHit = true
            });

            Database.Add(new OracleLib
            {
                Name = "MonkeyKing",
                Slot = SpellSlot.Q,
                Range = 300,
                Width = 0,
                Speed = 20,
                Delay = 0.5,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "MonkeyKing",
                Slot = SpellSlot.W,
                Range = 0,
                Width = 175,
                Speed = 0,
                Delay = 0.5,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "MonkeyKing",
                Slot = SpellSlot.W,
                Range = 325,
                Width = 325,
                Speed = 0,
                Delay = 0.5,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "MonkeyKing",
                Slot = SpellSlot.E,
                Range = 625,
                Width = 0,
                Speed = 2200,
                Delay = 0,
                Type = CastType.EnemyCast,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "MonkeyKing",
                Slot = SpellSlot.R,
                Range = 315,
                Width = 315,
                Speed = 700,
                Delay = 0,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "MonkeyKing",
                Slot = SpellSlot.R,
                Range = 0,
                Width = 0,
                Speed = 700,
                Delay = 0,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Xerath",
                Slot = SpellSlot.Q,
                Range = 750,
                Width = 100,
                Speed = 500,
                Delay = 0.75,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = true
            });

            Database.Add(new OracleLib
            {
                Name = "Xerath",
                Slot = SpellSlot.W,
                Range = 1100,
                Width = 200,
                Speed = 20,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Xerath",
                Slot = SpellSlot.E,
                Range = 1050,
                Width = 70,
                Speed = 1600,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = true
            });

            Database.Add(new OracleLib
            {
                Name = "Xerath",
                Slot = SpellSlot.R,
                Range = 5600,
                Width = 200,
                Speed = 500,
                Delay = 0.75,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Xin Zhao",
                Slot = SpellSlot.Q,
                Range = 200,
                Width = 0,
                Speed = 2000,
                Delay = 0,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.Extreme,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Xin Zhao",
                Slot = SpellSlot.W,
                Range = 0,
                Width = 0,
                Speed = 2000,
                Delay = 0,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Xin Zhao",
                Slot = SpellSlot.E,
                Range = 600,
                Width = 120,
                Speed = 1750,
                Delay = 0.5,
                Type = CastType.EnemyCast,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Xin Zhao",
                Slot = SpellSlot.R,
                Range = 375,
                Width = 375,
                Speed = 1750,
                Delay = 0,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Yasuo",
                Slot = SpellSlot.Q,
                Range = 475,
                Width = 55,
                Speed = 1500,
                Delay = 0.75,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = true
            });

            Database.Add(new OracleLib
            {
                Name = "Yasuo",
                Slot = SpellSlot.Q,
                Range = 475,
                Width = 55,
                Speed = 1500,
                Delay = 0.75,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = true
            });

            Database.Add(new OracleLib
            {
                Name = "Yasuo",
                Slot = SpellSlot.Q,
                Range = 1000,
                Width = 90,
                Speed = 1500,
                Delay = 0.75,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = true
            });

            Database.Add(new OracleLib
            {
                Name = "Yasuo",
                Slot = SpellSlot.W,
                Range = 400,
                Width = 0,
                Speed = 500,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Yasuo",
                Slot = SpellSlot.E,
                Range = 475,
                Width = 0,
                Speed = 20,
                Delay = 0.5,
                Type = CastType.EnemyCast,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Yasuo",
                Slot = SpellSlot.R,
                Range = 1200,
                Width = 0,
                Speed = 20,
                Delay = 0.5,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Yorick",
                Slot = SpellSlot.Q,
                Range = 0,
                Width = 0,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Yorick",
                Slot = SpellSlot.W,
                Range = 600,
                Width = 200,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Yorick",
                Slot = SpellSlot.E,
                Range = 550,
                Width = 200,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.EnemyCast,
                DangerLevel = RiskLevel.Normal,
                CC = true,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Yorick",
                Slot = SpellSlot.R,
                Range = 900,
                Width = 0,
                Speed = 1500,
                Delay = 0.5,
                Type = CastType.TeamCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false,

            });

            Database.Add(new OracleLib
            {
                Name = "Zac",
                Slot = SpellSlot.Q,
                Range = 550,
                Width = 120,
                Speed = 902,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.High,
                CC = true,
                CheckLine = true
            });

            Database.Add(new OracleLib
            {
                Name = "Zac",
                Slot = SpellSlot.W,
                Range = 350,
                Width = 350,
                Speed = 1600,
                Delay = 0.5,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.Extreme,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Zac",
                Slot = SpellSlot.E,
                Range = 1550,
                Width = 250,
                Speed = 1500,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Zac",
                Slot = SpellSlot.R,
                Range = 850,
                Width = 300,
                Speed = 1800,
                Delay = 0.5,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Zed",
                Slot = SpellSlot.Q,
                Range = 900,
                Width = 45,
                Speed = 902,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = true
            });

            Database.Add(new OracleLib
            {
                Name = "Zed",
                Slot = SpellSlot.W,
                Range = 550,
                Width = 40,
                Speed = 1600,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Zed",
                Slot = SpellSlot.E,
                Range = 300,
                Width = 300,
                Speed = 0,
                Delay = 0,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.High,
                CC = true,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Zed",
                Slot = SpellSlot.R,
                Range = 850,
                Width = 0,
                Speed = 0,
                Delay = 0.5,
                Type = CastType.EnemyCast,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false,
                Timer = 2600
            });

            Database.Add(new OracleLib
            {
                Name = "Ziggs",
                Slot = SpellSlot.Q,
                Range = 850,
                Width = 75,
                Speed = 1750,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = true
            });

            Database.Add(new OracleLib
            {
                Name = "Ziggs",
                Slot = SpellSlot.Q,
                Range = 850,
                Width = 75,
                Speed = 1750,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = true
            });

            Database.Add(new OracleLib
            {
                Name = "Ziggs",
                Slot = SpellSlot.W,
                Range = 850,
                Width = 300,
                Speed = 1750,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Normal,
                CC = true,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Ziggs",
                Slot = SpellSlot.W,
                Range = 850,
                Width = 300,
                Speed = 1750,
                Delay = 0.5,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.Normal,
                CC = true,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Ziggs",
                Slot = SpellSlot.E,
                Range = 850,
                Width = 350,
                Speed = 1750,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Ziggs",
                Slot = SpellSlot.E,
                Range = 850,
                Width = 350,
                Speed = 1750,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Ziggs",
                Slot = SpellSlot.R,
                Range = 850,
                Width = 600,
                Speed = 1750,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false,
                Timer = 950 - Game.Ping
            });

            Database.Add(new OracleLib
            {
                Name = "Zilean",
                Slot = SpellSlot.Q,
                Range = 700,
                Width = 0,
                Speed = 1100,
                Delay = 0,
                Type = CastType.Always,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false,
                Timer = 3000
            });

            Database.Add(new OracleLib
            {
                Name = "Zilean",
                Slot = SpellSlot.W,
                Range = 0,
                Width = 0,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.SelfCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Zilean",
                Slot = SpellSlot.E,
                Range = 700,
                Width = 0,
                Speed = 1100,
                Delay = 0.5,
                Type = CastType.Always,
                DangerLevel = RiskLevel.High,
                CC = true,
                CheckLine = false,

            });

            Database.Add(new OracleLib
            {
                Name = "Zilean",
                Slot = SpellSlot.R,
                Range = 780,
                Width = 0,
                Speed = int.MaxValue,
                Delay = 0.5,
                Type = CastType.TeamCast,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false,

            });

            Database.Add(new OracleLib
            {
                Name = "Zyra",
                Slot = SpellSlot.Q,
                Range = 800,
                Width = 240,
                Speed = 1400,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Normal,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Zyra",
                Slot = SpellSlot.W,
                Range = 800,
                Width = 0,
                Speed = 2200,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.NoDamage,
                CC = false,
                CheckLine = false
            });

            Database.Add(new OracleLib
            {
                Name = "Zyra",
                Slot = SpellSlot.E,
                Range = 1100,
                Width = 70,
                Speed = 1400,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = true
            });

            Database.Add(new OracleLib
            {
                Name = "Zyra",
                Slot = SpellSlot.R,
                Range = 700,
                Width = 550,
                Speed = 20,
                Delay = 0.5,
                Type = CastType.Skillshot,
                DangerLevel = RiskLevel.Extreme,
                CC = true,
                CheckLine = false
            });
        }
    }
}
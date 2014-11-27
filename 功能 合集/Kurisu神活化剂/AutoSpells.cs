﻿using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using OC = Oracle.Program;

namespace Oracle
{
    internal static class AutoSpells
    {

        private static Menu mainmenu, menuconfig;
        private static readonly Obj_AI_Hero me = ObjectManager.Player;

        public static void Initialize(Menu root)
        {
            Game.OnGameUpdate += Game_OnGameUpdate;

            mainmenu = new Menu("鑷姩娉曟湳", "asmenu");
            menuconfig = new Menu("鑷姩娉曟湳閰嶇疆", "asconfig");

             // auto shields
            CreateMenuItem(95, "BraumE", "Unbreakable", "braumshield", SpellSlot.E);
            CreateMenuItem(95, "DianaOrbs", "Pale Cascade", "dianashield", SpellSlot.W);
            CreateMenuItem(95, "GalioBulwark", "Bulwark", "galioshield", SpellSlot.W);
            CreateMenuItem(95, "GarenW", "Courage", "garenshield", SpellSlot.W, false);
            CreateMenuItem(95, "EyeOfTheStorm", "Eye of the Storm", "jannashield", SpellSlot.E);
            CreateMenuItem(95, "KarmaSolKimShield", "Inspire", "karmashield", SpellSlot.E);
            CreateMenuItem(95, "LuluE", "Help Pix!", "lulushield", SpellSlot.E);
            CreateMenuItem(95, "LuxPrismaticWave", "Prismatic Barrier", "luxshield", SpellSlot.W);
            CreateMenuItem(95, "NautilusPiercingGaze", "Titans Wraith", "nautshield", SpellSlot.W);
            CreateMenuItem(95, "OrianaRedactCommand", "Command Protect", "oriannashield", SpellSlot.E);
            CreateMenuItem(95, "ShenFeint", "Feint", "shenshield", SpellSlot.W, false);
            CreateMenuItem(95, "MoltenShield", "Molten Shield", "annieshield", SpellSlot.E);
            CreateMenuItem(95, "JarvanIVGoldenAegis", "Golden Aegis", "j4shield", SpellSlot.W);
            CreateMenuItem(95, "BlindMonkWOne", "Safegaurd", "leeshield", SpellSlot.W, false);
            CreateMenuItem(95, "RivenFeint", "Valor", "rivenshield", SpellSlot.E, false);
            CreateMenuItem(95, "FioraRiposte", "Riposte", "fiorashield", SpellSlot.W, false);
            CreateMenuItem(95, "RumbleShield", "Scrap Shield", "rumbleshield", SpellSlot.W, false);
            CreateMenuItem(95, "SionW", "Soul Furnace", "sionshield", SpellSlot.W);
            CreateMenuItem(95, "SkarnerExoskeleton", "Exoskeleton", "skarnershield", SpellSlot.W);
            CreateMenuItem(95, "UrgotTerrorCapacitorActive2", "Terror Capacitor", "urgotshield", SpellSlot.W);
            CreateMenuItem(95, "Obduracy", "Brutal Strikes", "malphshield", SpellSlot.W);
            CreateMenuItem(95, "DefensiveBallCurl", "Defensive Ball Curl", "rammusshield", SpellSlot.W);

            // auto heals
            CreateMenuItem(80, "TriumphantRoar", "Triumphant Roar", "troar", SpellSlot.E);
            CreateMenuItem(80, "PrimalSurge", "Primal Surge", "psurge", SpellSlot.E);
            CreateMenuItem(80, "RemoveScurvy", "Remove Scurvy", "rscurvy", SpellSlot.W);
            CreateMenuItem(80, "JudicatorDivineBlessing", "Divine Blessing", "dblessing", SpellSlot.W);
            CreateMenuItem(80, "NamiE", "Ebb and Flow", "eflow", SpellSlot.W);
            CreateMenuItem(80, "SonaW", "Aria of Perseverance", "sonaheal", SpellSlot.W);
            CreateMenuItem(80, "SorakaW", "Astral Infusion", "ainfusion", SpellSlot.W, false);
            CreateMenuItem(80, "Imbue", "Imbue", "imbue", SpellSlot.Q);

            // auto ultimates
            CreateMenuItem(35, "LuluR", "Wild Growth", "luluult", SpellSlot.R, true, true);
            CreateMenuItem(20, "UndyingRage", "Undying Rage", "tryndult", SpellSlot.R, false, true);
            CreateMenuItem(20, "ChronoShift", "Chorno Shift", "zilult", SpellSlot.R, true, true);
            CreateMenuItem(20, "YorickReviveAlly", "Omen of Death", "yorickult", SpellSlot.R, true, true);

            // slow removers
            CreateMenuItem(0, "EvelynnW", "Draw Frenzy", "eveslow", SpellSlot.W, false, false, true);
            CreateMenuItem(0, "GarenQ", "Decisive Strike", "garenslow", SpellSlot.Q, false, false, true);

            // auto zhonya skills
            //CreateMenuItem(0, "FioraDance", "Blade Waltz", "fioradodge", SpellSlot.R, false, true);

            root.AddSubMenu(mainmenu);
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {     
            if (me.HasBuffOfType(BuffType.Slow))
            {
                // slow removals
                UseSpell("GarenQ", "garenslow", OC.IncomeDamage, float.MaxValue, false);
                UseSpell("EvelynnW", "eveslow", OC.IncomeDamage, float.MaxValue, false);
            }

            // auto heals
            UseSpell("TriumphantRoar", "troar", 0, 575f, true, true);
            UseSpell("PrimalSurge", "psurge", 0, 600f, true, true);
            UseSpell("RemoveScurvy", "rscurvy", 0, float.MaxValue, true, true);
            UseSpell("JudicatorDivineBlessing", "dblessing", 0, 900f, true, true);
            UseSpell("NamiE", "eflow", 0, 725f, true, true);
            UseSpell("SonaW", "sonaheal", 0, 1000f, true, true);
            UseSpell("SorakaW", "ainfusion", 0, 450f, false, true);
            UseSpell("Imbue", "imbue", 0, 750f, true, true);

            if (OC.IncomeDamage < 1)
                return;
            // auto shields
            UseSpell("BraumE", "braumshield", OC.IncomeDamage);
            UseSpell("DianaOrbs", "dianashield", OC.IncomeDamage);
            UseSpell("GalioBulwark", "galioshield", OC.IncomeDamage, 800f);
            UseSpell("GarenW", "garenshield", OC.IncomeDamage, float.MaxValue, false);
            UseSpell("EyeOfTheStorm", "jannashield", OC.IncomeDamage, 800f);
            UseSpell("KarmaSolKimShield", "karmashield", OC.IncomeDamage, 800f);
            UseSpell("LuxPrismaticWave", "luxshield", OC.IncomeDamage, 1075f);
            UseSpell("NautilusPiercingGaze", "nautshield", OC.IncomeDamage);
            UseSpell("OrianaRedactCommand", "oriannashield", 1100f, OC.IncomeDamage);
            UseSpell("ShenFeint", "shenshield", OC.IncomeDamage, float.MaxValue, false);
            UseSpell("JarvanIVGoldenAegis", "j4shield", OC.IncomeDamage);
            UseSpell("BlindMonkWOne", "leeshield", OC.IncomeDamage, 700f, false);
            UseSpell("RivenFeint", "rivenshield", OC.IncomeDamage, float.MaxValue, false);
            UseSpell("RumbleShield", "rumbleshield", OC.IncomeDamage);
            UseSpell("SionW", "sionshield", OC.IncomeDamage);
            UseSpell("SkarnerExoskeleton", "skarnershield", OC.IncomeDamage);
            UseSpell("UrgotTerrorCapacitorActive2", "urgotshield", OC.IncomeDamage);
            UseSpell("MoltenShield", "annieshield", OC.IncomeDamage);
            UseSpell("FioraRiposte", "fiorashield", OC.IncomeDamage, float.MaxValue, false);
            UseSpell("Obduracy", "malphshield", OC.IncomeDamage);
            UseSpell("DefensiveBallCurl", "rammusshield", OC.IncomeDamage);

            // auto ults
            UseSpell("LuluR", "luluult", OC.IncomeDamage, 900f, false);
            UseSpell("UndyingRage", "tryndult", OC.IncomeDamage, float.MaxValue, false);
            UseSpell("ChronoShift", "zilult", OC.IncomeDamage, 900f, false);
            UseSpell("YorickReviveAlly", "yorickult", OC.IncomeDamage, 900f, false);

            // auto zhonya skills
            //UseSpell("FioraDance", "fioradodge", OC.IncomeDamage, 300f, false);
        }

        private static void UseSpell(string sdataname, string menuvar, float incdmg, float range = float.MaxValue, 
            bool usemana = true, bool isheal = false)
        {
            var slot = me.GetSpellSlot(sdataname);
            if (slot == SpellSlot.Unknown)
                return;

            if (slot != SpellSlot.Unknown && !mainmenu.Item("use" + menuvar).GetValue<bool>())
                return;

            var spell = new Spell(slot, range);
            if (!spell.IsReady())
                return;

            var allyuse = range.ToString() != float.MaxValue.ToString();
            var target = allyuse ? OC.FriendlyTarget() : me;

            if (!menuconfig.Item("ason" + target.SkinName).GetValue<bool>())
                return;

            if (target.Distance(me.Position) > range) 
                return;

            var aManaPercent = (int) ((me.Mana/me.MaxMana)*100);
            var aHealthPercent = (int) ((target.Health/target.MaxHealth)*100);
            var iDamagePercent = (int) ((incdmg/target.MaxHealth)*100);

            if (OC.AggroTarget.Distance(me.Position) > spell.Range || !me.NotRecalling())
                return;

            if (aHealthPercent <= mainmenu.Item("use" + menuvar + "Pct").GetValue<Slider>().Value && !isheal)
            {
                if (usemana && aManaPercent <= mainmenu.Item("use" + menuvar + "Mana").GetValue<Slider>().Value)
                    return;

                if (me.SkinName == "Soraka" && (me.Health/me.MaxHealth*100 <= mainmenu.Item("useSorakaMana").GetValue<Slider>().Value || target.IsMe))
                    return;

                if ((iDamagePercent >= 1 || incdmg >= target.Health) && OC.AggroTarget.NetworkId == target.NetworkId)
                {
                    if (menuvar == "luxshield" || menuvar == "rivenshield")
                    {
                        var po = spell.GetPrediction(target);
                        if (po.Hitchance >= HitChance.Medium && !target.IsMe)
                            spell.Cast(po.CastPosition);
                        else
                        {
                            spell.Cast(Game.CursorPos);
                        }
                    }
                    else
                    {
                        spell.Cast(target);
                    }
                }
            }
            else if (aHealthPercent <= mainmenu.Item("use" + menuvar + "Pct").GetValue<Slider>().Value && isheal)
            {
                if (me.SkinName == "Soraka" && (int)(me.Health/me.MaxHealth*100) <= mainmenu.Item("useSorakaMana").GetValue<Slider>().Value)
                    return;

                if (aManaPercent >= mainmenu.Item("use" + menuvar + "Mana").GetValue<Slider>().Value && usemana)
                    spell.Cast(target);
            }
            else if (iDamagePercent >= mainmenu.Item("use" + menuvar + "Dmg").GetValue<Slider>().Value)
            {
                spell.Cast(target);
            }
        }

        private static void CreateMenuItem(int defaultvalue, string sdataname, string displayname, string menuvar, SpellSlot slot,
            bool usemana = true, bool autoult = false, bool slowremoval = false)
        {
            var champslot = me.GetSpellSlot(sdataname);
            if (champslot != SpellSlot.Unknown && champslot == slot)
            {
                foreach (var x in ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsAlly))
                    menuconfig.AddItem(new MenuItem("ason" + x.SkinName, "Use for " + x.SkinName)).SetValue(true);
                mainmenu.AddSubMenu(menuconfig);

                var menuName = new Menu(displayname, menuvar.ToLower());
                menuName.AddItem(new MenuItem("use" + menuvar, "Enable " + displayname)).SetValue(true);
                if (slowremoval)
                    menuName.AddItem(new MenuItem("use" + menuvar + "Slow", "绉婚櫎鍙樻參").SetValue(true));
                if (!slowremoval)
                    menuName.AddItem(new MenuItem("use" + menuvar + "Pct", "浣跨敤娉曟湳琛€閲忕櫨鍒嗘瘮")).SetValue(new Slider(defaultvalue));
                if (!autoult || !slowremoval)
                    menuName.AddItem(new MenuItem("use" + menuvar + "Dmg", "浣跨敤娉曟湳浼ゅ鐧惧垎姣攟")).SetValue(new Slider(45));
                if (me.SkinName == "Soraka")
                    menuName.AddItem(new MenuItem("useSorakaMana", "浣跨敤鏈€浣庨檺鍒惰閲忕櫨鍒嗘瘮")).SetValue(new Slider(35));
                if (usemana)
                    menuName.AddItem(new MenuItem("use" + menuvar + "Mana", "浣跨敤鏈€浣庨檺鍒舵硶鍔涚櫨鍒嗘瘮")).SetValue(new Slider(45));
                mainmenu.AddSubMenu(menuName);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace xSaliceReligionAIO.Champions
{
    class Corki : Champion
    {
        public Corki()
        {
            SetSpells();
            LoadMenu();
        }

        private void SetSpells()
        {
            Q = new Spell(SpellSlot.Q, 825);
            Q.SetSkillshot(.3f, 250, 1225, false, SkillshotType.SkillshotCircle);

            W = new Spell(SpellSlot.W, 800);

            E = new Spell(SpellSlot.E, 600);
            E.SetSkillshot(.1f, (float)(45 * Math.PI / 180), 1500, false, SkillshotType.SkillshotCone);

            R = new Spell(SpellSlot.R, 1500);
            R.SetSkillshot(.2f, 40, 1500, true, SkillshotType.SkillshotLine);
        }

        private void LoadMenu()
        {
            var key = new Menu("Key", "Key");
            {
                key.AddItem(new MenuItem("ComboActive", "Combo!").SetValue(new KeyBind(32, KeyBindType.Press)));
                key.AddItem(new MenuItem("HarassActive", "Harass!").SetValue(new KeyBind("C".ToCharArray()[0], KeyBindType.Press)));
                key.AddItem(new MenuItem("HarassActiveT", "Harass (toggle)!").SetValue(new KeyBind("N".ToCharArray()[0], KeyBindType.Toggle)));
                key.AddItem(new MenuItem("LaneClearActive", "Farm!").SetValue(new KeyBind("V".ToCharArray()[0], KeyBindType.Press)));
                //add to menu
                menu.AddSubMenu(key);
            }

            var combo = new Menu("Combo", "Combo");
            {
                combo.AddItem(new MenuItem("UseQCombo", "Use Q").SetValue(true));
                combo.AddItem(new MenuItem("UseECombo", "Use E").SetValue(true));
                combo.AddItem(new MenuItem("UseRCombo", "Use R").SetValue(true));
                combo.AddItem(new MenuItem("Always_Use", "Spell after AA in AA range").SetValue(true));
                combo.AddItem(new MenuItem("qHit", "Q/R HitChance").SetValue(new Slider(3, 1, 3)));
                combo.AddItem(new MenuItem("ComboR_Limit", "Save R Stack").SetValue(new Slider(0, 0, 7)));
                menu.AddSubMenu(combo);
            }

            var harass = new Menu("Harass", "Harass");
            {
                harass.AddItem(new MenuItem("UseQHarass", "Use Q").SetValue(true));
                harass.AddItem(new MenuItem("UseEHarass", "Use E").SetValue(false));
                harass.AddItem(new MenuItem("UseRHarass", "Use R").SetValue(true));
                harass.AddItem(new MenuItem("qHit2", "Q/R HitChance").SetValue(new Slider(3, 1, 3)));
                harass.AddItem(new MenuItem("HarassR_Limit", "Save R Stack").SetValue(new Slider(5, 0, 7)));
                AddManaManagertoMenu(harass, "Harass", 50);
                menu.AddSubMenu(harass);
            }

            var farm = new Menu("LaneClear", "LaneClear");
            {
                farm.AddItem(new MenuItem("UseQFarm", "Use Q").SetValue(true));
                farm.AddItem(new MenuItem("UseEFarm", "Use E").SetValue(false));
                farm.AddItem(new MenuItem("UseRFarm", "Use R").SetValue(true));
                farm.AddItem(new MenuItem("LaneClearR_Limit", "Save R Stack").SetValue(new Slider(5, 0, 7)));
                AddManaManagertoMenu(farm, "LaneClear", 50);
                menu.AddSubMenu(farm);
            }

            var drawMenu = new Menu("Drawing", "Drawing");
            {
                drawMenu.AddItem(new MenuItem("Draw_Disabled", "Disable All").SetValue(false));
                drawMenu.AddItem(new MenuItem("Draw_Q", "Draw Q").SetValue(true));
                drawMenu.AddItem(new MenuItem("Draw_W", "Draw W").SetValue(true));
                drawMenu.AddItem(new MenuItem("Draw_E", "Draw E").SetValue(true));
                drawMenu.AddItem(new MenuItem("Draw_R", "Draw R").SetValue(true));

                MenuItem drawComboDamageMenu = new MenuItem("Draw_ComboDamage", "Draw Combo Damage").SetValue(true);
                MenuItem drawFill = new MenuItem("Draw_Fill", "Draw Combo Damage Fill").SetValue(new Circle(true, Color.FromArgb(90, 255, 169, 4)));
                drawMenu.AddItem(drawComboDamageMenu);
                drawMenu.AddItem(drawFill);
                DamageIndicator.DamageToUnit = GetComboDamage;
                DamageIndicator.Enabled = drawComboDamageMenu.GetValue<bool>();
                DamageIndicator.Fill = drawFill.GetValue<Circle>().Active;
                DamageIndicator.FillColor = drawFill.GetValue<Circle>().Color;
                drawComboDamageMenu.ValueChanged +=
                    delegate(object sender, OnValueChangeEventArgs eventArgs)
                    {
                        DamageIndicator.Enabled = eventArgs.GetNewValue<bool>();
                    };
                drawFill.ValueChanged +=
                    delegate(object sender, OnValueChangeEventArgs eventArgs)
                    {
                        DamageIndicator.Fill = eventArgs.GetNewValue<Circle>().Active;
                        DamageIndicator.FillColor = eventArgs.GetNewValue<Circle>().Color;
                    };

                menu.AddSubMenu(drawMenu);
            }
        }

         private float GetComboDamage(Obj_AI_Base target)
        {
            double comboDamage = 0;

            if (Q.IsReady())
                comboDamage += Player.GetSpellDamage(target, SpellSlot.Q);

            if (W.IsReady())
                comboDamage += Player.GetSpellDamage(target, SpellSlot.W);

            if (E.IsReady())
                comboDamage += Player.GetSpellDamage(target, SpellSlot.E);

            if (R.IsReady())
                comboDamage += Player.GetSpellDamage(target, SpellSlot.R);

            return (float)(comboDamage + Player.GetAutoAttackDamage(target) * 3);
        }

        private void Combo()
        {
            UseSpells(menu.Item("UseQCombo").GetValue<bool>(), menu.Item("UseECombo").GetValue<bool>(), menu.Item("UseRCombo").GetValue<bool>(), "Combo");
        }

        private void Harass()
        {
            UseSpells(menu.Item("UseQHarass").GetValue<bool>(), menu.Item("UseEHarass").GetValue<bool>(), menu.Item("UseRHarass").GetValue<bool>(), "Harass");
        }

        private void UseSpells(bool useQ, bool useE, bool useR, string source)
        {
            if (source == "Harass" && !HasMana("Harass"))
                return;

            var target = SimpleTs.GetTarget(550, SimpleTs.DamageType.Magical);
            if ((target != null && source == "Combo") || !menu.Item("Always_Use").GetValue<bool>())
                return;

            if(useR && R.IsReady())
                Cast_R(source);
            if(useQ && Q.IsReady())
                CastBasicSkillShot(Q, Q.Range, SimpleTs.DamageType.Magical, GetHitchance(source));
            if(useE && E.IsReady())
                CastBasicSkillShot(E, E.Range, SimpleTs.DamageType.Physical, HitChance.Medium);
        }

        public override void AfterAttack(Obj_AI_Base unit, Obj_AI_Base target)
        {
            if (!menu.Item("ComboActive").GetValue<KeyBind>().Active || !unit.IsMe || !(target is Obj_AI_Hero))
                return;

            if (menu.Item("UseRCombo").GetValue<bool>() && R.IsReady())
                R.Cast(target, packets());
            if (menu.Item("UseQCombo").GetValue<bool>() && Q.IsReady())
                Q.Cast(target, packets());
            if (menu.Item("UseECombo").GetValue<bool>() && E.IsReady())
                E.Cast(packets());
            
        }

        private void Farm()
        {
            if (!HasMana("LaneClear"))
                return;

            var useQ = menu.Item("UseQFarm").GetValue<bool>();
            var useE = menu.Item("UseEFarm").GetValue<bool>();
            var useR = menu.Item("UseRFarm").GetValue<bool>();

            if(useQ)
                CastBasicFarm(Q);
            if(useR)
                Cast_R("Farm");
            if (useE)
            {
                int allMinionECount = MinionManager.GetMinions(Player.ServerPosition, E.Range, MinionTypes.All,MinionTeam.NotAlly).Count;
                if (allMinionECount > 1)
                    E.Cast(packets());
            }
        }

        private void Cast_R(string mode)
        {
            var range = Player.HasBuff("CorkiMissileBarrageCounterBig") ? 1500 : 1300;

            if (mode == "Combo" && menu.Item("ComboR_Limit").GetValue<Slider>().Value < Player.Spellbook.GetSpell(SpellSlot.R).Ammo)
                CastBasicSkillShot(R, range, SimpleTs.DamageType.Magical, GetHitchance(mode));
            else if (mode == "Harass" && menu.Item("HarassR_Limit").GetValue<Slider>().Value < Player.Spellbook.GetSpell(SpellSlot.R).Ammo)
                CastBasicSkillShot(R, range, SimpleTs.DamageType.Magical, GetHitchance(mode));
            else if (mode == "Farm" && menu.Item("LaneClearR_Limit").GetValue<Slider>().Value < Player.Spellbook.GetSpell(SpellSlot.R).Ammo)
                CastBasicFarm(R);
        }

        public override void Game_OnGameUpdate(EventArgs args)
        {
            //check if player is dead
            if (Player.IsDead) return;

            if (menu.Item("ComboActive").GetValue<KeyBind>().Active)
            {
                Combo();
            }
            else
            {
                if (menu.Item("LaneClearActive").GetValue<KeyBind>().Active)
                    Farm();

                if (menu.Item("HarassActive").GetValue<KeyBind>().Active)
                    Harass();

                if (menu.Item("HarassActiveT").GetValue<KeyBind>().Active)
                    Harass();
            }
        }

        public override void Drawing_OnDraw(EventArgs args)
        {
            if (menu.Item("Draw_Disabled").GetValue<bool>())
                return;

            if (menu.Item("Draw_Q").GetValue<bool>())
                if (Q.Level > 0)
                    Utility.DrawCircle(Player.Position, Q.Range, Q.IsReady() ? Color.Green : Color.Red);

            if (menu.Item("Draw_W").GetValue<bool>())
                if (W.Level > 0)
                    Utility.DrawCircle(Player.Position, W.Range, W.IsReady() ? Color.Green : Color.Red);

            if (menu.Item("Draw_E").GetValue<bool>())
                if (E.Level > 0)
                    Utility.DrawCircle(Player.Position, E.Range, E.IsReady() ? Color.Green : Color.Red);

            if (menu.Item("Draw_R").GetValue<bool>())
                if (R.Level > 0)
                    Utility.DrawCircle(Player.Position, Player.HasBuff("CorkiMissileBarrageCounterBig") ? R.Range : 1300, R.IsReady() ? Color.Green : Color.Red);
        }

    }
}

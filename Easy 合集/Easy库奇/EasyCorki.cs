using LeagueSharp;
using LeagueSharp.Common;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyCorki
{
    class EasyCorki : Champion
    {
        static void Main(string[] args)
        {
            new EasyCorki();
        }

        public EasyCorki() : base("Corki")
        {

        }

        protected override void InitializeSkins(ref SkinManager Skins)
        {
            Skins.Add("Corki");
            Skins.Add("UFO Corki");
            Skins.Add("Ice Toboggan Corki");
            Skins.Add("Red Baron Corki");
            Skins.Add("Hot Rod Corki");
            Skins.Add("Urfrider Corki");
            Skins.Add("Dragonwing Corki");
        }
        protected override void InitializeSpells(ref SpellManager Spells)
        {
            Spell Q = new Spell(SpellSlot.Q, 825f);
            Q.SetSkillshot(0.3f, 250f, 1125f, false, SkillshotType.SkillshotCircle);

            Spell E = new Spell(SpellSlot.E, 600f);
            E.SetSkillshot(0f, (float)Math.PI / 180f * 45f, float.MaxValue, false, SkillshotType.SkillshotCone);

            Spell R = new Spell(SpellSlot.R);
            R.SetSkillshot(0.2f, 40f, 2000f, true, SkillshotType.SkillshotLine);

            Spell RBig = new Spell(SpellSlot.R, 1500f);
            Spell RSmall = new Spell(SpellSlot.R, 1300f);

            Spells.Add("Q", Q);
            Spells.Add("E", E);
            Spells.Add("R", R);
            Spells.Add("RBig", RBig);
            Spells.Add("RSmall", RSmall);
        }
        protected override void InitializeMenu()
        {
            Menu.AddSubMenu(new Menu("杩炴嫑", "Combo"));
            Menu.SubMenu("Combo").AddItem(new MenuItem("Combo_q", "浣跨敤 Q").SetValue(true));
            Menu.SubMenu("Combo").AddItem(new MenuItem("Combo_e", "浣跨敤 E").SetValue(true));
            Menu.SubMenu("Combo").AddItem(new MenuItem("Combo_r", "浣跨敤 R").SetValue(true));

            Menu.AddSubMenu(new Menu("楠氭壈", "Harass"));
            Menu.SubMenu("Harass").AddItem(new MenuItem("Harass_q", "浣跨敤 Q").SetValue(true));
            Menu.SubMenu("Harass").AddItem(new MenuItem("Harass_e", "浣跨敤 E").SetValue(false));
            Menu.SubMenu("Harass").AddItem(new MenuItem("Harass_r", "浣跨敤 R").SetValue(true));
            Menu.SubMenu("Harass").AddItem(new MenuItem("Harass_rlimit", "淇濇寔瀵煎脊鏁伴噺").SetValue(new Slider(3, 0, 7)));

            Menu.AddSubMenu(new Menu("鑷姩", "Auto"));
            Menu.SubMenu("Auto").AddItem(new MenuItem("Auto_q", "浣跨敤 Q").SetValue(true));
            Menu.SubMenu("Auto").AddItem(new MenuItem("Auto_e", "浣跨敤 E").SetValue(false));
            Menu.SubMenu("Auto").AddItem(new MenuItem("Auto_r", "浣跨敤 R").SetValue(false));
            Menu.SubMenu("Auto").AddItem(new MenuItem("Auto_rlimit", "淇濇寔瀵煎脊鏁伴噺").SetValue(new Slider(3, 0, 7)));

            Menu.AddSubMenu(new Menu("缁樺浘", "Drawing"));
            Menu.SubMenu("Drawing").AddItem(new MenuItem("Drawing_q", "Q 鑼冨洿").SetValue(new Circle(true, Color.FromArgb(100, 0, 255, 0))));
            Menu.SubMenu("Drawing").AddItem(new MenuItem("Drawing_r", "R 鑼冨洿").SetValue(new Circle(true, Color.FromArgb(100, 0, 255, 0))));
            Menu.SubMenu("Drawing").AddItem(new MenuItem("Drawing_damage", "Q+R 浼ゅ鎸囩ず鍣▅r").SetValue(true));
        }
    
        protected override void Combo()
        {
            if (Menu.Item("Combo_q").GetValue<bool>()) Spells.CastSkillshot("Q", SimpleTs.DamageType.Magical, HitChance.VeryHigh, true, true);
            if (Menu.Item("Combo_e").GetValue<bool>()) Spells.CastSkillshot("E", SimpleTs.DamageType.Physical);
            if (Menu.Item("Combo_r").GetValue<bool>()) Spells.CastSkillshot("R", SimpleTs.DamageType.Magical);
        }
        protected override void Harass()
        {
            if (Menu.Item("Harass_q").GetValue<bool>()) Spells.CastSkillshot("Q", SimpleTs.DamageType.Magical, HitChance.VeryHigh, true, true);
            if (Menu.Item("Harass_e").GetValue<bool>()) Spells.CastSkillshot("E", SimpleTs.DamageType.Physical);
            if (Menu.Item("Harass_r").GetValue<bool>() && missiles() > Menu.Item("Harass_rlimit").GetValue<Slider>().Value) Spells.CastSkillshot("R", SimpleTs.DamageType.Magical);
        }
        protected override void Auto()
        {
            if (Menu.Item("Auto_q").GetValue<bool>()) Spells.CastSkillshot("Q", SimpleTs.DamageType.Magical, HitChance.VeryHigh, true, true);
            if (Menu.Item("Auto_e").GetValue<bool>()) Spells.CastSkillshot("E", SimpleTs.DamageType.Physical);
            if (Menu.Item("Auto_r").GetValue<bool>() && missiles() > Menu.Item("Auto_rlimit").GetValue<Slider>().Value) Spells.CastSkillshot("R", SimpleTs.DamageType.Magical);
        }

        protected override void Draw()
        {
            DrawCircle("Drawing_q", "Q");
            DrawCircle("Drawing_r", "R");
            
            Utility.HpBarDamageIndicator.DamageToUnit = ComboDamage;
            Utility.HpBarDamageIndicator.Enabled = Menu.Item("Drawing_damage").GetValue<bool>();
        }
        protected override void Update()
        {
            if (Player.HasBuff("CorkiMissileBarrageCounterBig"))
                Spells.get("R").Range = Spells.get("RBig").Range;
            else
                Spells.get("R").Range = Spells.get("RSmall").Range;
        }

        private float ComboDamage(Obj_AI_Hero hero)
        {
            float damage = 0;

            if (Spells.get("Q").IsReady())
                damage += (float)Damage.GetSpellDamage(Player, hero, SpellSlot.Q);
            if (Spells.get("R").IsReady())
            {
                float rDamage = (float)Damage.GetSpellDamage(Player, hero, SpellSlot.R);
                if (Player.HasBuff("CorkiMissileBarrageCounterBig"))
                    rDamage *= 1.5f;
                damage += rDamage;
            }

            return damage;
        }

        private int missiles()
        {
            return Player.Spellbook.GetSpell(SpellSlot.R).Ammo;
        }
    }
}

using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using Color = System.Drawing.Color;
using SharpDX;


namespace Perfect_Jax
{

    class Program
    {
        public static Spell.Targeted Q;
        public static Spell.Active W;
        public static Spell.Active E;
        public static Spell.Active R;
        public static Menu Menu, FarmingMenu, MiscMenu, DrawMenu, HarassMenu, ComboMenu, SmiteMenu, UpdateMenu;
        static Item Healthpot;
        static Item Manapot;
        static Item CrystalFlask;
        public static SpellSlot SmiteSlot = SpellSlot.Unknown;
        public static SpellSlot IgniteSlot = SpellSlot.Unknown;
        private static readonly int[] SmitePurple = { 3713, 3726, 3725, 3726, 3723 };
        private static readonly int[] SmiteGrey = { 3711, 3722, 3721, 3720, 3719 };
        private static readonly int[] SmiteRed = { 3715, 3718, 3717, 3716, 3714 };
        private static readonly int[] SmiteBlue = { 3706, 3710, 3709, 3708, 3707 };
        public static float WardRange = 600f;
        private static bool eCounterStrike = false;


        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }

        public static AIHeroClient _Player
        {
            get { return ObjectManager.Player; }

        }

        private static string Smitetype
        {
            get
            {
                if (SmiteBlue.Any(i => Item.HasItem(i)))
                    return "s5_summonersmiteplayerganker";

                if (SmiteRed.Any(i => Item.HasItem(i)))
                    return "s5_summonersmiteduel";

                if (SmiteGrey.Any(i => Item.HasItem(i)))
                    return "s5_summonersmitequick";

                if (SmitePurple.Any(i => Item.HasItem(i)))
                    return "itemsmiteaoe";

                return "summonersmite";
            }
        }

        private static void Loading_OnLoadingComplete(EventArgs args)
        {
            if (Player.Instance.ChampionName != "Jax")
                return;


            Bootstrap.Init(null);

            Healthpot = new Item(2003, 0);
            Manapot = new Item(2004, 0);
            CrystalFlask = new Item(2032, 0);
            uint level = (uint)Player.Instance.Level;
            Q = new Spell.Targeted(SpellSlot.Q, 700);
            W = new Spell.Active(SpellSlot.W);
            E = new Spell.Active(SpellSlot.E, 187);
            R = new Spell.Active(SpellSlot.R);

            Menu = MainMenu.AddMenu("Perfect Jax", "perfectjax");
            Menu.AddLabel("Perrrrrrrrrfect Ass");
            Menu.AddSeparator();

            

            ComboMenu = Menu.AddSubMenu("Combo Settings","ComboSettings");            
            ComboMenu.AddLabel("Combo Settings");
            ComboMenu.Add("QCombo", new CheckBox("Use Q"));
            ComboMenu.Add("WCombo", new CheckBox("Use W"));
            ComboMenu.Add("ECombo", new CheckBox("Use E"));
            ComboMenu.Add("RCombo", new CheckBox("Use R"));
            ComboMenu.Add("useTiamat", new CheckBox("Use Items"));

            HarassMenu = Menu.AddSubMenu("Harass Settings", "HarassSettings");
            HarassMenu.AddLabel("Harass Settings");
            HarassMenu.Add("QHarass", new CheckBox("Use Q"));
            HarassMenu.Add("WHarass", new CheckBox("Use W"));
            HarassMenu.Add("EHarass", new CheckBox("Use E"));

            FarmingMenu = Menu.AddSubMenu("Lane Clear", "FarmSettings");

            FarmingMenu.AddLabel("Lane Clear");
            FarmingMenu.Add("QLaneClear", new CheckBox("Use Q LaneClear"));
            FarmingMenu.Add("QlaneclearMana", new Slider("Mana < %", 45, 0, 100));
            FarmingMenu.Add("WLaneClear", new CheckBox("Use W LaneClear"));
            FarmingMenu.Add("WlaneclearMana", new Slider("Mana < %", 35, 0, 100));
            FarmingMenu.Add("ELaneClear", new CheckBox("Use E LaneClear"));
            FarmingMenu.Add("ElaneclearMana", new Slider("Mana < %", 60, 0, 100));

            FarmingMenu.AddLabel("Jungle Clear");
            FarmingMenu.Add("Qjungle", new CheckBox("Use Q in Jungle"));
            FarmingMenu.Add("QjungleMana", new Slider("Mana < %", 45, 0, 100));
            FarmingMenu.Add("Wjungle", new CheckBox("Use W in Jungle"));
            FarmingMenu.Add("WjungleMana", new Slider("Mana < %", 35, 0, 100));
            FarmingMenu.Add("Ejungle", new CheckBox("Use E in Jungle"));
            FarmingMenu.Add("EjungleMan", new Slider("Mana < %", 60, 0, 100));

            FarmingMenu.AddLabel("Last Hit Settings");
            FarmingMenu.Add("Qlasthit", new CheckBox("Use Q LastHit"));
            FarmingMenu.Add("Wlasthit", new CheckBox("Use W LastHit"));
            FarmingMenu.Add("QlasthitMana", new Slider("Mana < %", 35, 0, 100));

            
            SetSmiteSlot();
            if (SmiteSlot != SpellSlot.Unknown)
            {
                SmiteMenu = Menu.AddSubMenu("Smite Usage", "SmiteUsage");
                SmiteMenu.Add("SmiteEnemy", new CheckBox("Use Smite Combo for Enemy!"));               
                SmiteMenu.AddLabel("Smite Usage");
                SmiteMenu.Add("Use Smite?", new CheckBox("Use Smite"));
                SmiteMenu.Add("Red?", new CheckBox("Red"));
                SmiteMenu.Add("Blue?", new CheckBox("Blue"));
                SmiteMenu.Add("Dragon?", new CheckBox("Dragon"));
                SmiteMenu.Add("Baron?", new CheckBox("Baron"));
            }


            MiscMenu = Menu.AddSubMenu("More Settings", "Misc");

            MiscMenu.AddLabel("Auto");
            MiscMenu.Add("Auto Ignite", new CheckBox("Auto Ignite"));
            MiscMenu.Add("autoQ", new CheckBox("Use Auto Q to Flee/Escape"));         
            MiscMenu.Add("autoE", new CheckBox("Use Auto E"));
            MiscMenu.Add("autoECount", new Slider("Enemy Count ", 3, 1, 5));
            MiscMenu.AddSeparator();
            MiscMenu.AddLabel("Items");
            MiscMenu.AddSeparator();
            MiscMenu.AddLabel("BOTRK,Bilgewater Cutlass Settings");
            MiscMenu.Add("botrkHP", new Slider("My HP < %", 60, 0, 100));
            MiscMenu.Add("botrkenemyHP", new Slider("Enemy HP < %", 60, 0, 100));

            MiscMenu.AddLabel("KillSteal");
            MiscMenu.Add("Qkill", new CheckBox("Use Q KillSteal"));
            MiscMenu.Add("Ekill", new CheckBox("Use E KillSteal"));

            MiscMenu.AddLabel("Activator");
            MiscMenu.Add("useHP", new CheckBox("Use Health Potion"));           
            MiscMenu.Add("useHPV", new Slider("HP < %", 45, 0, 100));
            MiscMenu.Add("useMana", new CheckBox("Use Mana Potion"));
            MiscMenu.Add("useManaV", new Slider("Mana < %", 45, 0, 100));
            MiscMenu.Add("useCrystal", new CheckBox("Use Hunter Potion"));
            MiscMenu.Add("useCrystalHPV", new Slider("HP < %", 65, 0, 100));
            MiscMenu.Add("useCrystalManaV", new Slider("Mana < %", 65, 0, 100));

            DrawMenu = Menu.AddSubMenu("Draw Settings", "Drawings");
            DrawMenu.Add("drawAA", new CheckBox("Draw AA Range"));
            DrawMenu.Add("drawQ", new CheckBox("Draw Q"));

            UpdateMenu = Menu.AddSubMenu("Last Update Logs", "Updates");
            UpdateMenu.AddLabel("V0.0.1");
            UpdateMenu.AddLabel("-Share");

            Game.OnTick += Game_OnTick;
            Drawing.OnDraw += Drawing_OnDraw;

            Chat.Print("Perrrrrrrrrfect Addon", System.Drawing.Color.Red);
        }
        private static void SetSmiteSlot()
        {
            foreach (
                var spell in
                    _Player.Spellbook.Spells.Where(
                        spell => string.Equals(spell.Name, Smitetype, StringComparison.CurrentCultureIgnoreCase)))
            {
                SmiteSlot = spell.Slot;
            }
        }



        private static void Game_OnTick(EventArgs args)
        {
            var HPpot = MiscMenu["useHP"].Cast<CheckBox>().CurrentValue;
            var Mpot = MiscMenu["useMana"].Cast<CheckBox>().CurrentValue;
            var Crystal = MiscMenu["useCrystal"].Cast<CheckBox>().CurrentValue;
            var HPv = MiscMenu["useHPv"].Cast<Slider>().CurrentValue;
            var Manav = MiscMenu["useManav"].Cast<Slider>().CurrentValue;
            var CrystalHPv = MiscMenu["useCrystalHPv"].Cast<Slider>().CurrentValue;
            var CrystalManav = MiscMenu["useCrystalManav"].Cast<Slider>().CurrentValue;
            var useItem = ComboMenu["useTiamat"].Cast<CheckBox>().CurrentValue;
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            var igntarget = TargetSelector.GetTarget(600, DamageType.True);
            var t = TargetSelector.GetTarget(Q.Range, DamageType.Magical);

            if(eCounterStrike == true)
            {
                eCounterStrike = false;
                E.Cast();
            }

            if (HPpot && Player.Instance.HealthPercent < HPv)
            {
                if (Item.HasItem(Healthpot.Id) && Item.CanUseItem(Healthpot.Id) && !Player.HasBuff("RegenerationPotion"))
                {
                    Healthpot.Cast();
                }
            }

            if (Mpot && Player.Instance.ManaPercent < Manav)
            {
                if (Item.HasItem(Manapot.Id) && Item.CanUseItem(Manapot.Id) && !Player.HasBuff("FlaskOfCrystalWater") && !Player.HasBuff("ItemCrystalFlask"))
                {
                    Manapot.Cast();
                }
            }
            
            if (Crystal && Player.Instance.HealthPercent < CrystalHPv || Crystal && Player.Instance.ManaPercent < CrystalManav)
            {
                if (Item.HasItem(CrystalFlask.Id) && Item.CanUseItem(CrystalFlask.Id) && !Player.HasBuff("RegenerationPotion") && !Player.HasBuff("FlaskOfCrystalWater") && !Player.HasBuff("ItemCrystalFlask"))
                {
                    CrystalFlask.Cast();
                }
               
            }

            if (useItem && target.IsValidTarget(400) && !target.IsDead && !target.IsZombie && target.HealthPercent < 100)
            {
                HandleItems();
            }


            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo();
                SmiteOnTarget(t);
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                Harass();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                LaneClear();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
            {
                LastHit();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                JungleClear();
            }
            KillSteal();
            autoE();
            
        }
        private static void SmiteOnTarget(AIHeroClient t)
        {
            var range = 700f;
            var use = SmiteMenu["SmiteEnemy"].Cast<CheckBox>().CurrentValue;
            var itemCheck = SmiteBlue.Any(i => Item.HasItem(i)) || SmiteRed.Any(i => Item.HasItem(i));
            if (itemCheck && use &&
                _Player.Spellbook.CanUseSpell(SmiteSlot) == SpellState.Ready &&
                t.Distance(_Player.Position) < range)
            {
                _Player.Spellbook.CastSpell(SmiteSlot, t);
            }
        }
        private static void Combo()
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            var useQ = ComboMenu["QCombo"].Cast<CheckBox>().CurrentValue;
            var useW = ComboMenu["WCombo"].Cast<CheckBox>().CurrentValue;
            var useE = ComboMenu["ECombo"].Cast<CheckBox>().CurrentValue;
            var useR = ComboMenu["RCombo"].Cast<CheckBox>().CurrentValue;
            var useItem = ComboMenu["useTiamat"].Cast<CheckBox>().CurrentValue;

            if (useQ && Q.IsReady() && target.IsValidTarget(Q.Range) && !target.IsDead && !target.IsZombie)
            {
                Q.Cast(target);
            }
            if (E.IsReady() && useE && target.IsValidTarget(E.Range) && !target.IsDead && !target.IsZombie)
            {
                eCounterStrike = true;
                E.Cast();
            }
            if (W.IsReady() && useW && target.IsValidTarget(125) && !target.IsDead && !target.IsZombie )
            {
                W.Cast();
            }
            if (R.IsReady() && useR && target.IsValidTarget(400) && !target.IsDead && !target.IsZombie)
            {             
                    R.Cast();                
            }
            if (useItem && target.IsValidTarget(400) && !target.IsDead && !target.IsZombie)
            {
                HandleItems();
            }

        }
        private static void KillSteal()
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            var useQ = MiscMenu["Qkill"].Cast<CheckBox>().CurrentValue;
            var useE = MiscMenu["Ekill"].Cast<CheckBox>().CurrentValue;

            if (Q.IsReady() && useQ && target.IsValidTarget(Q.Range) && !target.IsZombie && target.Health <= _Player.GetSpellDamage(target, SpellSlot.Q))
            {
                Q.Cast(target);
            }
            if (E.IsReady() && useE && target.IsValidTarget(E.Range) && !target.IsZombie && target.Health <= _Player.GetSpellDamage(target, SpellSlot.E))
            {
                eCounterStrike = true;
                E.Cast();
            }
        }

        internal static void HandleItems()
        {
            var botrktarget = TargetSelector.GetTarget(550, DamageType.Physical);
            var youmutarget = TargetSelector.GetTarget(800, DamageType.Physical);
            var useItem = ComboMenu["useTiamat"].Cast<CheckBox>().CurrentValue;
            var useBotrkHP = MiscMenu["botrkHP"].Cast<Slider>().CurrentValue;
            var useBotrkEnemyHP = MiscMenu["botrkenemyHP"].Cast<Slider>().CurrentValue;
            //HYDRA
            if (useItem && Item.HasItem(3077) && Item.CanUseItem(3077))
                Item.UseItem(3077);

            //TİAMAT
            if (useItem && Item.HasItem(3074) && Item.CanUseItem(3074))
                Item.UseItem(3074);

            //NEW ITEM
            if (useItem && Item.HasItem(3748) && Item.CanUseItem(3748))
                Item.UseItem(3748);

            //BİLGEWATER CUTLASS
            if (useItem && Item.HasItem(3144) && Item.CanUseItem(3144) && botrktarget.HealthPercent <= useBotrkEnemyHP && _Player.HealthPercent <= useBotrkHP)
                Item.UseItem(3144, botrktarget);

            //BOTRK
            if (useItem && Item.HasItem(3153) && Item.CanUseItem(3153) && botrktarget.HealthPercent <= useBotrkEnemyHP && _Player.HealthPercent <= useBotrkHP)
                Item.UseItem(3153, botrktarget);

            //YOUMU
            if (useItem && Item.HasItem(3142) && Item.CanUseItem(3142) && youmutarget.IsValidTarget(800))
                Item.UseItem(3142);
        }

        private static void Harass()
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            var useQ = HarassMenu["QHarass"].Cast<CheckBox>().CurrentValue;
            var useW = HarassMenu["WHarass"].Cast<CheckBox>().CurrentValue;
            var useE = HarassMenu["EHarass"].Cast<CheckBox>().CurrentValue;

            if (Q.IsReady() && useQ && target.IsValidTarget(Q.Range) && !target.IsDead && !target.IsZombie)
            {
                Q.Cast(target);
            }
            if (W.IsReady() && useW && target.IsValidTarget(_Player.AttackRange) && !target.IsDead && !target.IsZombie)
            {
                W.Cast();
            }
            if (E.IsReady() && useE && target.IsValidTarget(E.Range) && !target.IsDead && !target.IsZombie)
            {
                eCounterStrike = true;
                E.Cast();
            }

        }
        private static void LaneClear()
        {
            var useQ = FarmingMenu["QLaneClear"].Cast<CheckBox>().CurrentValue;
            var useW = FarmingMenu["WLaneClear"].Cast<CheckBox>().CurrentValue;
            var useE = FarmingMenu["ELaneClear"].Cast<CheckBox>().CurrentValue;
            var Qmana = FarmingMenu["QlaneclearMana"].Cast<Slider>().CurrentValue;
            var Wmana = FarmingMenu["WlaneclearMana"].Cast<Slider>().CurrentValue;
            var EHP = FarmingMenu["ElaneclearMana"].Cast<Slider>().CurrentValue;
            var minions = ObjectManager.Get<Obj_AI_Base>().OrderBy(m => m.Health).Where(m => m.IsMinion && m.IsEnemy && !m.IsDead);
            foreach (var minion in minions)
            {
                if (useQ && Q.IsReady() && !minion.IsValidTarget(230) && minion.IsValidTarget(Q.Range) && Player.Instance.ManaPercent > Qmana && minion.Health <= _Player.GetSpellDamage(minion, SpellSlot.Q))
                {
                    Q.Cast(minion);
                }
                if (useW && W.IsReady() && Player.Instance.ManaPercent > Wmana && minion.IsValidTarget(_Player.AttackRange) && minion.Health <= _Player.GetSpellDamage(minion, SpellSlot.W))
                {
                    W.Cast();
                }
                if (useE && E.IsReady() && Player.Instance.HealthPercent > EHP && minions.Count() >= 3)
                {
                    eCounterStrike = true;
                    E.Cast();
                }
            }
        }
        private static void JungleClear()
        {
            var useQ = FarmingMenu["Qjungle"].Cast<CheckBox>().CurrentValue;
            var useQMana = FarmingMenu["QjungleMana"].Cast<Slider>().CurrentValue;
            var useW = FarmingMenu["Wjungle"].Cast<CheckBox>().CurrentValue;
            var useWMana = FarmingMenu["WjungleMana"].Cast<Slider>().CurrentValue;
            var useE = FarmingMenu["Ejungle"].Cast<CheckBox>().CurrentValue;
            var useEHP = FarmingMenu["EjungleMana"].Cast<Slider>().CurrentValue;
            foreach (var monster in EntityManager.MinionsAndMonsters.Monsters)
            {
                if (useQ && Q.IsReady() && Player.Instance.ManaPercent > useQMana)
                {
                    Q.Cast(monster);
                }
                if (useW && W.IsReady() && Player.Instance.ManaPercent > useWMana)
                {
                    W.Cast();
                }
                if (useE && E.IsReady() && Player.Instance.HealthPercent > useEHP)
                {
                    eCounterStrike = true;
                    E.Cast();
                }

                HandleItems();
            }
        }
        private static void LastHit()
        {
            var useQ = FarmingMenu["Qlasthit"].Cast<CheckBox>().CurrentValue;
            var useW = FarmingMenu["Wlasthit"].Cast<CheckBox>().CurrentValue;
            var mana = FarmingMenu["QlasthitMana"].Cast<Slider>().CurrentValue;
            var minions = ObjectManager.Get<Obj_AI_Base>().OrderBy(m => m.Health).Where(m => m.IsMinion && m.IsEnemy && !m.IsDead);
            foreach (var minion in minions)
            {
                if (useQ && Q.IsReady() && !minion.IsValidTarget(230) && minion.IsValidTarget(Q.Range) && Player.Instance.ManaPercent > mana && minion.Health <= _Player.GetSpellDamage(minion, SpellSlot.Q))
                {
                    Q.Cast(minion);
                }
                if (useW && W.IsReady() && minion.Health <= _Player.GetSpellDamage(minion, SpellSlot.W))
                {
                    W.Cast();
                }
            }
        }
        private static void autoE()
        {
            var minions = ObjectManager.Get<Obj_AI_Base>().OrderBy(m => m.Health).Where(m => m.IsMinion && m.IsEnemy && !m.IsDead);
            var ENEMY = HeroManager.Enemies.OrderBy(m => m.Health).Where(m => m.IsEnemy && !m.IsMe && !m.IsDead);
            var target = TargetSelector.GetTarget(E.Range, DamageType.True);
            var useE = MiscMenu["autoE"].Cast<CheckBox>().CurrentValue;
            var enemyCount = MiscMenu["autoECount"].Cast<Slider>().CurrentValue;

            if (useE && E.IsReady() && target.IsValidTarget(E.Range) && ENEMY.Count() >= enemyCount)
            {
                eCounterStrike = true;
                E.Cast();
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (DrawMenu["drawQ"].Cast<CheckBox>().CurrentValue)
            {
                Drawing.DrawCircle(_Player.Position, Q.Range, System.Drawing.Color.Red);
            }
        }
    }
}
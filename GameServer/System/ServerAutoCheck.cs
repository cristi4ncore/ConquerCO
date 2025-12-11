using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeathWish.Game;

namespace DeathWish.Game
{
    public unsafe class ServerAutoCheck
    {
        public Client.GameClient client;
        #region ClassSpell
        public static void ClassSpell(Client.GameClient client)
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                #region Trojen
                if (Database.AtributesStatus.IsTrojan(client.Player.Class))
                {
                    if (client.Player.Level < 110)
                    {
                        return;
                    }
                    else
                    {
                        #region XP Skills
                        #region Cyclone
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Cyclone))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Cyclone);
                        #endregion
                        #region SuperCyclone
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.SuperCyclone))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.SuperCyclone);
                        #endregion
                        #region Accuracy
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Accuracy))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Accuracy);
                        #endregion
                        #region Golem
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Golem))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Golem);
                        #endregion
                        #endregion
                        #region Normal Skills
                        #region Thunder
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Thunder))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Thunder);
                        #endregion
                        #region FatalCross
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FatalCross))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.FatalCross);
                        #endregion
                        #region Hercules
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Hercules))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Hercules);
                        #endregion
                        #region SpiritHealing
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.SpiritHealing))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.SpiritHealing);
                        #endregion
                        #endregion
                        #region Passive Skills
                        #region MortalStrike
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.MortalStrike))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.MortalStrike);
                        #endregion
                        #region BreathFocus
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.BreathFocus))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.BreathFocus);
                        #endregion
                        #endregion
                        #region Pure Skill
                        #region DragonWhirl
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.DragonWhirl))
                        {
                            if (Database.AtributesStatus.IsTrojan(client.Player.FirstClass) &&
                                Database.AtributesStatus.IsTrojan(client.Player.SecoundeClass))
                            {
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.DragonWhirl);
                            }
                        }
                        #endregion
                        #endregion
                    }
                }
                #endregion
                #region Warrior
                if (Database.AtributesStatus.IsWarrior(client.Player.Class))
                {
                    if (client.Player.Level < 110)
                    {
                        return;
                    }
                    else
                    {
                        #region XP Skills
                        #region Accuracy
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Accuracy))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Accuracy);
                        #endregion
                        #region Superman
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Superman))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Superman);
                        #endregion
                        #region Shield
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Shield))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Shield);
                        #endregion
                        #region Roar
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Roar))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Roar);
                        #endregion
                        #region FlyingMoon
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FlyingMoon))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.FlyingMoon);
                        #endregion
                        #region ManiacDance
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ManiacDance))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.ManiacDance);
                        #endregion
                        #endregion
                        #region Normal Skills
                        #region Thunder
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Thunder))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Thunder);
                        #endregion
                        #region ShieldBlock
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ShieldBlock))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.ShieldBlock);
                        #endregion
                        #region DefensiveStance
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.DefensiveStance))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.DefensiveStance);
                        #endregion
                        #region Backfire
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Backfire))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Backfire);
                        #endregion
                        #region WaveofBlood
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.WaveofBlood))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.WaveofBlood);
                        #endregion
                        #region MagicDefender
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.MagicDefender))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.MagicDefender);
                        #endregion
                        #region Dash
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Dash))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Dash);
                        #endregion
                        #region Pounce
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Pounce))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Pounce);
                        #endregion
                        #endregion
                        #region Pure Skill
                        #region Perseverance
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Perseverance))
                        {
                            if (Database.AtributesStatus.IsWarrior(client.Player.FirstClass) &&
                                Database.AtributesStatus.IsWarrior(client.Player.SecoundeClass))
                            {
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Perseverance);
                            }
                        }
                        #endregion
                        #endregion
                        #region Passive
                        #region TwistofWar
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.TwistofWar))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.TwistofWar);
                        #endregion
                        #region ScarofEarth
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ScarofEarth))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.ScarofEarth);
                        #endregion
                        #endregion
                    }
                }
                #endregion
                #region Archer
                if (Database.AtributesStatus.IsArcher(client.Player.Class))
                {
                    if (client.Player.Level < 110)
                    {
                        return;
                    }
                    else
                    {
                        #region XP Skills
                        #region XpFly
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.XpFly))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.XpFly);
                        #endregion
                        #region ArrowRain
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ArrowRain))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.ArrowRain);
                        #endregion
                        #region BladeFlurry
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.BladeFlurry))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.BladeFlurry);
                        #endregion
                        #endregion
                        #region Normal Skills
                        #region Thunder
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Thunder))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Thunder);
                        #endregion
                        #region PathOfShadow
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.PathOfShadow))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.PathOfShadow);
                        #endregion
                        #region ScatterFire
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ScatterFire))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.ScatterFire);
                        #endregion
                        #region RapidFire
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.RapidFire))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.RapidFire);
                        #endregion
                        #region Fly
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Fly))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Fly);
                        #endregion
                        #region Intensify
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Intensify))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Intensify);
                        #endregion
                        #region KineticSpark
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.KineticSpark))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.KineticSpark);
                        #endregion
                        #region MortalWound
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.MortalWound))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.MortalWound);
                        #endregion
                        #region BlisteringWave
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.BlisteringWave))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.BlisteringWave);
                        #endregion
                        #region SpiritFocus
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.SpiritFocus))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.SpiritFocus);
                        #endregion
                        #region DaggerStorm
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.DaggerStorm))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.DaggerStorm);
                        #endregion
                        #endregion
                        #region Pure Skill
                        #region StarArrow
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.StarArrow))
                        {
                            if (Database.AtributesStatus.IsArcher(client.Player.FirstClass) &&
                                Database.AtributesStatus.IsArcher(client.Player.SecoundeClass))
                            {
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.StarArrow);
                            }
                        }
                        #endregion
                        #endregion
                    }
                }
                #endregion
                #region Ninja
                if (Database.AtributesStatus.IsNinja(client.Player.Class))
                {
                    if (client.Player.Level < 110)
                    {
                        return;
                    }
                    else
                    {
                        #region XP Skills
                        #region FatalStrike
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FatalStrike))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.FatalStrike);
                        #endregion
                        #region ShurikenVortex
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ShurikenVortex))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.ShurikenVortex);
                        #endregion
                        #endregion
                        #region Normal Skills
                        #region Thunder
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Thunder))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Thunder);
                        #endregion
                        #region TwofoldBlades
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.TwofoldBlades))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.TwofoldBlades);
                        #endregion
                        #region ToxicFog
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ToxicFog))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.ToxicFog);
                        #endregion
                        #region PoisonStar
                        if (client.Player.Class == 55 && client.Player.FirstClass == 55 && client.Player.Reborn == 1 ||
                            client.Player.Class == 55 && client.Player.SecoundeClass == 55 ||
                            client.Player.FirstClass == 55 && client.Player.SecoundeClass == 55)
                        {
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.PoisonStar))
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.PoisonStar);
                        }
                        #endregion
                        #region ArcherBane
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ArcherBane))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.ArcherBane);
                        #endregion
                        #region GapingWounds
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.GapingWounds))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.GapingWounds);
                        #endregion
                        #region TwilightDance
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.TwilightDance))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.TwilightDance);
                        #endregion
                        #region FatalSpin
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FatalSpin))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.FatalSpin);
                        #endregion
                        #region BloodyScythe
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.BloodyScythe))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.BloodyScythe);
                        #endregion
                        #region MortalDrag
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.MortalDrag))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.MortalDrag);
                        #endregion
                        #region SuperTwofoldBlade
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.SuperTwofoldBlade))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.SuperTwofoldBlade);
                        #endregion
                        #region ShadowClone
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ShadowClone))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.ShadowClone);
                        #endregion
                        #endregion
                        #region Pure Skill
                        #region CounterKill
                        if (client.Player.FirstClass == 55 && client.Player.SecoundeClass == 55)
                        {
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.CounterKill))
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.CounterKill);
                        }
                        #endregion
                        #endregion
                    }
                }
                #endregion
                #region Monk
                if (Database.AtributesStatus.IsMonk(client.Player.Class))
                {
                    if (client.Player.Level < 110)
                    {
                        return;
                    }
                    else
                    {
                        #region XP Skills
                        #region Oblivion
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Oblivion))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Oblivion);
                        #endregion
                        #endregion
                        #region Normal Skills
                        #region InfernalEcho
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.InfernalEcho))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.InfernalEcho);
                        #endregion
                        #region WhirlwindKick
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.WhirlwindKick))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.WhirlwindKick);
                        #endregion
                        #region Thunder
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Thunder))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Thunder);
                        #endregion
                        #region RadiantPalm
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.RadiantPalm))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.RadiantPalm);
                        #endregion
                        #region Serenity
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Serenity))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Serenity);
                        #endregion
                        #region Tranquility
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Tranquility))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Tranquility);
                        #endregion
                        #region TyrantAura
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.TyrantAura))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.TyrantAura);
                        #endregion
                        #region FendAura
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FendAura))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.FendAura);
                        #endregion
                        #region MetalAura
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.MetalAura))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.MetalAura);
                        #endregion
                        #region WoodAura
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.WoodAura))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.WoodAura);
                        #endregion
                        #region WatherAura
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.WatherAura))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.WatherAura);
                        #endregion
                        #region FireAura
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FireAura))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.FireAura);
                        #endregion
                        #region EarthAura
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.EarthAura))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.EarthAura);
                        #endregion
                        #endregion
                        #region Passive Skills
                        #region TripleAttack
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.TripleAttack))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.TripleAttack);
                        #endregion
                        #region GraceofHeaven
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.GraceofHeaven))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.GraceofHeaven);
                        #endregion
                        #region WrathoftheEmperor
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.WrathoftheEmperor))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.WrathoftheEmperor);
                        #endregion
                        #endregion
                        #region Pure Skill
                        #region SoulShackle
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.SoulShackle))
                        {
                            if (Database.AtributesStatus.IsMonk(client.Player.FirstClass) &&
                                Database.AtributesStatus.IsMonk(client.Player.SecoundeClass))
                            {
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.SoulShackle);
                            }
                        }
                        #endregion
                        #endregion
                    }
                }
                #endregion
                #region Pirate
                if (Database.AtributesStatus.IsPirate(client.Player.Class))
                {
                    if (client.Player.Level < 110)
                    {
                        return;
                    }
                    else
                    {
                        #region XP Skills
                        #region CannonBarrage
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.CannonBarrage))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.CannonBarrage);
                        #endregion
                        #region BlackbeardsRage
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.BlackbeardsRage))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.BlackbeardsRage);
                        #endregion
                        #endregion
                        #region Normal Skills
                        #region Thunder
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Thunder))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Thunder);
                        #endregion
                        #region EagleEye
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.EagleEye))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.EagleEye);
                        #endregion
                        #region GaleBomb
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.GaleBomb))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.GaleBomb);
                        #endregion
                        #region BladeTempest
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.BladeTempest))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.BladeTempest);
                        #endregion
                        #endregion
                        #region Passive Skills
                        #region AdrenalineRush
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.AdrenalineRush))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.AdrenalineRush);
                        #endregion
                        #region BlackSpot
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Blackspot))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Blackspot);
                        #endregion
                        #region Windstorm
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Windstorm))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Windstorm);
                        #endregion
                        #endregion
                        #region Pure Skill
                        #region KrakensRevenge
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.KrakensRevenge))
                        {
                            if (client.Player.FirstClass == 75 && client.Player.Reborn == 1 ||
                                client.Player.SecoundeClass == 75 && client.Player.Class == 75)
                            {
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.KrakensRevenge);
                            }
                        }
                        #endregion
                        #region ScurvyBomb
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ScurvyBomb))
                        {
                            if (client.Player.FirstClass == 75 && client.Player.SecoundeClass == 75)
                            {
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.ScurvyBomb);
                            }
                        }
                        #endregion
                        #endregion
                    }
                }
                #endregion
                #region Dragon-Warrior
                if (Database.AtributesStatus.IsLee(client.Player.Class))
                {
                    if (client.Player.Level < 110)
                    {
                        return;
                    }
                    else
                    {
                        #region XP Skills
                        #region DragonCyclone
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.DragonCyclone))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.DragonCyclone);
                        #endregion
                        #endregion
                        #region Normal Skills
                        #region Thunder
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Thunder))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Thunder);
                        #endregion
                        #region SpeedKick
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.SpeedKick))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.SpeedKick);
                        #endregion
                        #region ViolentKick
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ViolentKick))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.ViolentKick);
                        #endregion
                        #region StormKick
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.StormKick))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.StormKick);
                        #endregion
                        #region CrackingSwipe
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.CrackingSwipe))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.CrackingSwipe);
                        #endregion
                        #region SplittingSwipe
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.SplittingSwipe))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.SplittingSwipe);
                        #endregion
                        #region DragonSwing
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.DragonSwing))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.DragonSwing);
                        #endregion
                        #region DragonPunch
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.DragonPunch))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.DragonPunch);
                        #endregion
                        #region DragonFlow
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.DragonFlow))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.DragonFlow);
                        #endregion
                        #region DragonRoar
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.DragonRoar))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.DragonRoar);
                        #endregion
                        #region AirKick
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.AirKick))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.AirKick);
                        #endregion
                        #region AirSweep
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.AirSweep))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.AirSweep);
                        #endregion
                        #region AirRaid
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.AirRaid))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.AirRaid);
                        #endregion
                        #region DragonSlash
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.DragonSlash))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.DragonSlash);
                        #endregion
                        #endregion
                        #region Pure Skills
                        #region DragonFury
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.DragonFury))
                        {
                            if (client.Player.FirstClass == 85 && client.Player.SecoundeClass == 85)
                            {
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.DragonFury);
                            }
                        }
                        #endregion
                        #endregion
                        #region EarthSweep
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.EarthSweep))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.EarthSweep);
                        #endregion
                        #region AirStrike
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.AirStrike))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.AirStrike);
                        #endregion
                    }
                }
                #endregion
                #region Water
                if (Database.AtributesStatus.IsWater(client.Player.Class))
                {
                    if (client.Player.Level < 110)
                    {
                        return;
                    }
                    else
                    {
                        #region XP Skills
                        #region ChainBolt
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ChainBolt))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.ChainBolt);
                        #endregion
                        #region Lightning
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Lightning))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Lightning);
                        #endregion
                        #region Revive
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Revive))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Revive);
                        #endregion
                        #region Vulcano
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Vulcano))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Vulcano);
                        #endregion
                        #region SpeedLightning
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.SpeedLightning))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.SpeedLightning);
                        #endregion
                        #region WaterElf
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.WaterElf))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.WaterElf);
                        #endregion
                        #endregion
                        #region Normal Skills

                        #region Thunder
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Thunder))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Thunder);
                        #endregion
                        #region Cure
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Cure))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Cure);
                        #endregion
                        #region Meditation
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Meditation))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Meditation);
                        #endregion
                        #region HealingRain
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.HealingRain))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.HealingRain);
                        #endregion
                        #region StarofAccuracy
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.StarofAccuracy))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.StarofAccuracy);
                        #endregion
                        #region MagicShield
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.MagicShield))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.MagicShield);
                        #endregion
                        #region Stigma
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Stigma))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Stigma);
                        #endregion
                        #region Invisibility
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Invisibility))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Invisibility);
                        #endregion
                        #region Pray
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Pray))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Pray);
                        #endregion
                        #region AdvancedCure
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.AdvancedCure))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.AdvancedCure);
                        #endregion
                        #region Nectar
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Nectar))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Nectar);
                        #endregion
                        #region BlessingTouch
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.BlessingTouch))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.BlessingTouch);
                        #endregion
                        #region AuroraLotus
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.AuroraLotus))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.AuroraLotus);
                        #endregion
                        #endregion
                        #region Pure Skill
                        #region AzureShield
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.AzureShield))
                        {
                            if (client.Player.FirstClass == 135 && client.Player.SecoundeClass == 135)
                            {
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.AzureShield);
                            }
                        }
                        #endregion
                        #endregion
                    }
                }
                #endregion
                #region Fire
                if (Database.AtributesStatus.IsFire(client.Player.Class))
                {
                    if (client.Player.Level < 110)
                    {
                        return;
                    }
                    else
                    {
                        #region XP Skills
                        #region ChainBolt
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ChainBolt))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.ChainBolt);
                        #endregion
                        #region Vulcano
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Vulcano))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Vulcano);
                        #endregion
                        #region Lightning
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Lightning))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Lightning);
                        #endregion
                        #region SpeedLightning
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.SpeedLightning))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.SpeedLightning);
                        #endregion
                        #endregion
                        #region Normal Skills
                        #region Thunder
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Thunder))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Thunder);
                        #endregion
                        #region Cure
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Cure))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Cure);
                        #endregion
                        #region Fire
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Fire))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Fire);
                        #endregion
                        #region Meditation
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Meditation))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Meditation);
                        #endregion
                        #region FireRing
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FireRing))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.FireRing);
                        #endregion
                        #region FireMeteor
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FireMeteor))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.FireMeteor);
                        #endregion
                        #region FireCircle
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FireCircle))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.FireCircle);
                        #endregion
                        #region Tornado
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Tornado))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Tornado);
                        #endregion
                        #region Bomb
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Bomb))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Bomb);
                        #endregion
                        #region FireofHell
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FireofHell))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.FireofHell);
                        #endregion
                        #region FlameLotus
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FlameLotus))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.FlameLotus);
                        #endregion
                        #region SearingTouch
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.SearingTouch))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.SearingTouch);
                        #endregion
                        #endregion
                        #region Pure Skill
                        #region HeavenBlade
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.HeavenBlade))
                        {
                            if (Database.AtributesStatus.IsFire(client.Player.FirstClass) &&
                                Database.AtributesStatus.IsFire(client.Player.SecoundeClass))
                            {
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.HeavenBlade);
                            }
                        }
                        #endregion
                        #endregion
                    }
                }
                #endregion
                #region Windwalker
                if (Database.AtributesStatus.IsWindWalker(client.Player.Class))
                {
                    if (client.Player.Level < 110)
                    {
                        return;
                    }
                    else
                    {
                        #region Thunder
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Thunder))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Thunder);
                        #endregion
                        #region XP Skills
                        #region Omnipotence
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Omnipotence))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Omnipotence);
                        #endregion
                        #endregion
                        #region Common-Skills
                        #region FrostGazeI
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FrostGazeI))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.FrostGazeI);
                        #endregion
                        #region FrostGazeI
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FrostGazeI))
                        {
                            if (client.Player.Reborn == 1)
                            {
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.FrostGazeI);
                            }
                        }
                        #endregion
                        #region FrostGazeII
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FrostGazeI) || !client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FrostGazeII))
                            if (client.Player.Reborn == 2 && client.Player.SecoundeClass == 165)
                            {
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.FrostGazeI);
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.FrostGazeII);
                            }
                        #endregion
                        #region FrostGazeIII
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FrostGazeI) || !client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FrostGazeII) || !client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FrostGazeIII))
                            if (client.Player.Reborn == 2 && client.Player.SecoundeClass == 165 && client.Player.FirstClass == 165)
                            {
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.FrostGazeIII);
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.FrostGazeI);
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.FrostGazeII);
                            }
                        #endregion
                        #region JusticeChant
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.JusticeChant))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.JusticeChant);
                        #endregion
                        #endregion
                        #region Windwalker[Melle]
                        #region ChillingSnow
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ChillingSnow))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.ChillingSnow);
                        #endregion
                        #region HealingSnow
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.HealingSnow))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.HealingSnow);
                        #endregion
                        #region FreezingPelter
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FreezingPelter))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.FreezingPelter);
                        #endregion
                        #region RageofWar
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.RageofWar))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.RageofWar);
                        #endregion
                        #region BurntFrost
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.BurntFrost))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.BurntFrost);
                        #endregion
                        #region AngerofStomper
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.AngerofStomper))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.AngerofStomper);
                        #endregion
                        #region HorrorofStomper
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.HorrorofStomper))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.HorrorofStomper);
                        #endregion
                        #region PeaceofStomper
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.PeaceofStomper))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.PeaceofStomper);
                        #endregion
                        #region RevengeTail
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.RevengeTail))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.RevengeTail);
                        #endregion
                        #endregion
                        #region Windwalker[Ranged]
                        #region Thundercloud
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Thundercloud))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Thundercloud);
                        #endregion
                        #region ShadowofChaser
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ShadowofChaser))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.ShadowofChaser);
                        #endregion
                        #region Thunderbolt
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Thunderbolt))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Thunderbolt);
                        #endregion
                        #region TripleBlasts
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.TripleBlasts))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.TripleBlasts);
                        #endregion
                        #region SwirlingStorm
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.SwirlingStorm))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.SwirlingStorm);
                        #endregion
                        #endregion
                    }
                }
                #endregion
            }
        }
        #endregion
        #region RebornSpell
        public static void RebornSpell(Client.GameClient client)
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                #region Reborn Skill
                switch (client.Player.FirstClass)
                {
                    #region Trojan
                    case 15:
                        {
                            #region Cyclone
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Cyclone))
                            {
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Cyclone);
                            }
                            #endregion
                            #region Golem
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Golem))
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Golem);
                            #endregion
                            break;
                        }
                    #endregion
                    #region Warrior
                    case 25:
                        {
                            #region Roar
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Roar))
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Roar);
                            #endregion
                            break;
                        }
                    #endregion
                    #region Ninja
                    case 55:
                        {
                            #region ToxicFog
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ToxicFog))
                            {
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.ToxicFog);
                            }
                            #endregion
                            #region PoisonStar
                            if (client.Player.Class == 55 && client.Player.FirstClass == 55 && client.Player.Reborn == 1 ||
                                client.Player.Class == 55 && client.Player.SecoundeClass == 55 ||
                                client.Player.FirstClass == 55 && client.Player.SecoundeClass == 55)
                            {
                                if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.PoisonStar))
                                    client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.PoisonStar);
                            }
                            #endregion
                            break;
                        }
                    #endregion
                    #region Monk
                    case 65:
                        {
                            #region Serenity
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Serenity))
                            {
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Serenity);
                            }
                            #endregion
                            break;
                        }
                    #endregion
                    #region Pirate
                    case 75:
                        {
                            #region GaleBomb
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.GaleBomb))
                            {
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.GaleBomb);
                            }
                            #endregion
                            break;
                        }
                    #endregion
                    #region Dragon-Warrior
                    case 85:
                        {
                            #region DragonRoar
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.DragonRoar))
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.DragonRoar);
                            #endregion
                            break;
                        }
                    #endregion
                    #region Water
                    case 135:
                        {
                            #region Cure
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Cure))
                            {
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Cure);
                            }
                            #endregion
                            #region Meditation
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Meditation))
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Meditation);
                            #endregion
                            #region StarofAccuracy
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.StarofAccuracy))
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.StarofAccuracy);
                            #endregion
                            #region MagicShield
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.MagicShield))
                            {
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.MagicShield);
                            }
                            #endregion
                            #region Stigma
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Stigma))
                            {
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Stigma);
                            }
                            #endregion
                            break;
                        }
                    #endregion
                    #region Fire
                    case 145:
                        {
                            #region Thunder
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Thunder))
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Thunder);
                            #endregion
                            // #region Cure
                            //   if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Cure))
                            //   client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Cure);
                            // #endregion
                            //  #region Fire
                            //  if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Fire))
                            //      client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Fire);
                            // #endregion
                            //   #region Meditation
                            //if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Meditation))
                            //    client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Meditation);
                            //  #endregion
                            break;
                        }
                    #endregion
                    #region WindWalker
                    case 165:
                        {
                            // #region FrostGazeI
                            // if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FrostGazeI))
                            //     client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.FrostGazeI);
                            // #endregion
                            //#region FrostGazeII
                            // if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FrostGazeII))
                            // {
                            //     if (client.Player.Reborn == 1 && client.Player.Class == 165 && client.Player.FirstClass == 165)
                            //     {
                            //         client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.FrostGazeII);
                            //     }
                            // }
                            // #endregion
                            break;
                        }
                    #endregion
                }
                #endregion
                #region Secand Skill
                switch (client.Player.SecoundeClass)
                {
                    #region Trojan
                    case 15:
                        {
                            #region Cyclone
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Cyclone))
                            {
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Cyclone);
                            }
                            #endregion
                            #region Golem
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Golem))
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Golem);
                            #endregion
                            break;
                        }
                    #endregion
                    #region Warrior
                    case 25:
                        {
                            #region Roar
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Roar))
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Roar);
                            #endregion
                            #region FlyingMoon
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FlyingMoon))
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.FlyingMoon);
                            #endregion
                            break;
                        }
                    #endregion
                    #region Ninja
                    case 55:
                        {
                            #region ToxicFog
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ToxicFog))
                            {
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.ToxicFog);
                            }
                            #endregion
                            #region PoisonStar
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.PoisonStar))
                            {
                                if (client.Player.Class == 55)
                                {
                                    client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.PoisonStar);
                                }
                            }
                            #endregion
                            break;
                        }
                    #endregion
                    #region Monk
                    case 65:
                        {
                            #region Serenity
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Serenity))
                            {
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Serenity);
                            }
                            #endregion
                            break;
                        }
                    #endregion
                    #region Pirate
                    case 75:
                        {
                            #region GaleBomb
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.GaleBomb))
                            {
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.GaleBomb);
                            }
                            #endregion
                            break;
                        }
                    #endregion
                    #region Dragon-Warrior
                    case 85:
                        {
                            #region DragonRoar
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.DragonRoar))
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.DragonRoar);
                            #endregion
                            break;
                        }
                    #endregion
                    #region Water
                    case 135:
                        {
                            #region Cure
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Cure))
                            {
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Cure);
                            }
                            #endregion
                            #region Meditation
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Meditation))
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Meditation);
                            #endregion
                            #region StarofAccuracy
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.StarofAccuracy))
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.StarofAccuracy);
                            #endregion
                            #region MagicShield
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.MagicShield))
                            {
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.MagicShield);
                            }
                            #endregion
                            #region Stigma
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Stigma))
                            {
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Stigma);
                            }
                            #endregion
                            break;
                        }
                    #endregion
                    #region Fire
                    case 145:
                        {
                            #region Thunder
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Thunder))
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Thunder);
                            #endregion
                            //  #region Cure
                            //  if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Cure))
                            //    client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Cure);
                            // #endregion
                            //   #region Fire
                            // if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Fire))
                            //     client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Fire);
                            // #endregion
                            // #region Meditation
                            // if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Meditation))
                            //    client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Meditation);
                            // #endregion
                            break;
                        }
                    #endregion
                    #region WindWalker
                    case 165:
                        {
                            //#region FrostGazeI
                            //if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FrostGazeI))
                            //    client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.FrostGazeI);
                            //#endregion
                            //#region FrostGazeII
                            //if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FrostGazeII))
                            //{
                            //    if (client.Player.Class == 165)
                            //    {
                            //        client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.FrostGazeII);
                            //    }
                            //}
                            //#endregion
                            break;
                        }
                    #endregion
                }
                #endregion
            }
        }
        #endregion
        #region RemoveSpell
        public static void RemoveSpell(Client.GameClient client)
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                #region Check [First & Secand-Class-Spell]
                #region (!)Ninja
                if (!Database.AtributesStatus.IsNinja(client.Player.FirstClass) || !Database.AtributesStatus.IsNinja(client.Player.SecoundeClass))
                {
                    client.Player.ActivateCounterKill = true;
                    #region PoisonStar
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.PoisonStar))
                    {
                        if (client.Player.Class == 55 && client.Player.FirstClass == 55 && client.Player.Reborn == 1 ||
                            client.Player.Class == 55 && client.Player.SecoundeClass == 55 ||
                            client.Player.FirstClass == 55 && client.Player.SecoundeClass == 55)
                        {

                        }
                        else
                        {
                            client.MySpells.Remove((ushort)Role.Flags.SpellID.PoisonStar, stream);
                        }
                    }

                    #endregion
                    #region Pure Skill
                    #region CounterKill
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.CounterKill))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.CounterKill, stream);
                    #endregion
                    #endregion
                }
                #endregion
                #region (!)Monk
                if (!Database.AtributesStatus.IsMonk(client.Player.FirstClass) || !Database.AtributesStatus.IsMonk(client.Player.SecoundeClass))
                {
                    #region Pure Skill
                    #region SoulShackle
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.SoulShackle))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.SoulShackle, stream);
                    #endregion
                    #region Compassion
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Compassion))//Compassion
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.Compassion, stream);
                    #endregion
                    #endregion
                }
                #endregion
                #region (!)Pirate
                if (!Database.AtributesStatus.IsPirate(client.Player.FirstClass) || !Database.AtributesStatus.IsPirate(client.Player.SecoundeClass))
                {
                    #region Pure Skill
                    #region KrakensRevenge
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.KrakensRevenge))
                    {
                        if (client.Player.FirstClass == 75 && client.Player.SecoundeClass == 75)
                        {
                            //
                        }
                        else
                        {
                            client.MySpells.Remove((ushort)Role.Flags.SpellID.KrakensRevenge, stream);
                        }
                    }
                    #endregion
                    #region ScurvyBomb
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ScurvyBomb))
                    {
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.ScurvyBomb, stream);
                    }
                    #endregion
                    #endregion
                }
                #endregion
                #region (!)Water
                if (!Database.AtributesStatus.IsWater(client.Player.FirstClass) || !Database.AtributesStatus.IsWater(client.Player.SecoundeClass))
                {
                    #region Pure Skill
                    #region AzureShield
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.AzureShield))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.AzureShield, stream);
                    #endregion
                    #endregion
                }
                #endregion
                #region (!)Fire
                if (!Database.AtributesStatus.IsFire(client.Player.FirstClass) || !Database.AtributesStatus.IsFire(client.Player.SecoundeClass))
                {
                    #region Pure Skill
                    #region HeavenBlade
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.HeavenBlade))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.HeavenBlade, stream);
                    #endregion
                    #endregion
                }
                #endregion
                #region (!)Windwalker
                if (!Database.AtributesStatus.IsWindWalker(client.Player.FirstClass) || !Database.AtributesStatus.IsWindWalker(client.Player.SecoundeClass))
                {
                    #region FrostGazeII
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FrostGazeII))
                    {
                        if ((client.Player.Class == 165 && client.Player.SecoundeClass == 165) ||
                            (client.Player.Class == 165 && client.Player.FirstClass == 165 && client.Player.Reborn == 1))
                        {
                            //
                        }
                        else
                        {
                            client.MySpells.Remove((ushort)Role.Flags.SpellID.FrostGazeII, stream);
                        }
                    }
                    #endregion
                    #region Pure Skill
                    #region FrostGazeIII
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FrostGazeIII))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.FrostGazeIII, stream);
                    #endregion
                    #endregion
                }
                #endregion
                #region (!)Trojen
                if (!Database.AtributesStatus.IsTrojan(client.Player.FirstClass) || !Database.AtributesStatus.IsTrojan(client.Player.SecoundeClass))
                {
                    //  client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.DragonWhirl);
                    #region Pure Skill
                    #region DragonWhirl
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.DragonWhirl))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.DragonWhirl, stream);
                    #endregion
                    #endregion
                }
                #endregion
                #endregion
                #region Check [Class-Spell]
                #region (!)Trojen
                if (!Database.AtributesStatus.IsTrojan(client.Player.Class))
                {
                    #region XP Skills
                    #region Cyclone
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Cyclone))
                    {
                        if (client.Player.FirstClass == 15 || client.Player.SecoundeClass == 15)
                        {
                            //
                        }
                        else
                        {
                            client.MySpells.Remove((ushort)Role.Flags.SpellID.Cyclone, stream);
                        }
                    }
                    #endregion
                    #region SuperCyclone
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.SuperCyclone))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.SuperCyclone, stream);
                    #endregion
                    #region Accuracy
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Accuracy))
                    {
                        if (Database.AtributesStatus.IsWarrior(client.Player.Class))
                        {
                            //
                        }
                        else
                        {
                            client.MySpells.Remove((ushort)Role.Flags.SpellID.Accuracy, stream);
                        }
                    }
                    #endregion
                    #region Golem
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Golem))
                    {
                        if (client.Player.FirstClass == 15 || client.Player.SecoundeClass == 15)
                        {
                            //
                        }
                        else
                        {
                            client.MySpells.Remove((ushort)Role.Flags.SpellID.Golem, stream);
                        }
                    }
                    #endregion
                    #endregion
                    #region Normal Skills
                    #region FatalCross
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FatalCross))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.FatalCross, stream);
                    #endregion
                    #region Hercules
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Hercules))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.Hercules, stream);
                    #endregion
                    #region SpiritHealing
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.SpiritHealing))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.SpiritHealing, stream);
                    #endregion
                    #endregion
                    #region Passive Skills
                    #region MortalStrike
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.MortalStrike))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.MortalStrike, stream);
                    #endregion
                    #region BreathFocus
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.BreathFocus))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.BreathFocus, stream);
                    #endregion
                    #endregion
                    #region Pure Skill
                    #region DragonWhirl
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.DragonWhirl))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.DragonWhirl, stream);
                    #endregion
                    #endregion
                }
                #endregion
                #region (!)Warrior
                if (!Database.AtributesStatus.IsWarrior(client.Player.Class))
                {
                    #region XP Skills
                    #region Accuracy
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Accuracy))
                    {
                        if (Database.AtributesStatus.IsTrojan(client.Player.Class))
                        {
                            //
                        }
                        else
                        {
                            client.MySpells.Remove((ushort)Role.Flags.SpellID.Accuracy, stream);
                        }
                    }
                    #endregion
                    #region Superman
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Superman))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.Superman, stream);
                    #endregion
                    #region Shield
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Shield))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.Shield, stream);
                    #endregion
                    #region Roar
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Roar))
                    {
                        if (client.Player.FirstClass == 25 || client.Player.SecoundeClass == 25)
                        {
                            //
                        }
                        else
                        {
                            client.MySpells.Remove((ushort)Role.Flags.SpellID.Roar, stream);
                        }
                    }
                    #endregion
                    #region FlyingMoon
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FlyingMoon))
                    {
                        if (client.Player.SecoundeClass == 25)
                        {
                            //
                        }
                        else
                        {
                            client.MySpells.Remove((ushort)Role.Flags.SpellID.FlyingMoon, stream);
                        }
                    }
                    #endregion
                    #region ManiacDance
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ManiacDance))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.ManiacDance, stream);
                    #endregion
                    #endregion
                    #region Normal Skills
                    #region ShieldBlock
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ShieldBlock))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.ShieldBlock, stream);
                    #endregion
                    #region MagicDefender
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.MagicDefender))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.MagicDefender, stream);
                    #endregion
                    #region Pounce
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Pounce))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.Pounce, stream);
                    #endregion
                    #region Dash
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Dash))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.Dash, stream);
                    #endregion
                    #region Backfire
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Backfire))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.Backfire, stream);
                    #endregion
                    #region DefensiveStance
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.DefensiveStance))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.DefensiveStance, stream);
                    #endregion
                    #region WaveofBlood
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.WaveofBlood))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.WaveofBlood, stream);
                    #endregion
                    #endregion
                    #region Pure Skill
                    #region Perseverance
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Perseverance))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.Perseverance, stream);
                    #endregion
                    #endregion
                    #region Passive
                    #region TwistofWar
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.TwistofWar))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.TwistofWar, stream);
                    #endregion
                    #region ScarofEarth
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ScarofEarth))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.ScarofEarth, stream);
                    #endregion
                    #endregion
                }
                #endregion
                #region (!)Archer
                if (!Database.AtributesStatus.IsArcher(client.Player.Class))
                {
                    #region XP Skills
                    #region XpFly
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.XpFly))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.XpFly, stream);
                    #endregion
                    #region ArrowRain
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ArrowRain))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.ArrowRain, stream);
                    #endregion
                    #region BladeFlurry
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.BladeFlurry))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.BladeFlurry, stream);
                    #endregion
                    #endregion
                    #region Normal Skills
                    #region PathOfShadow
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.PathOfShadow))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.PathOfShadow, stream);
                    #endregion
                    #region ScatterFire
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ScatterFire))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.ScatterFire, stream);
                    #endregion
                    #region RapidFire
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.RapidFire))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.RapidFire, stream);
                    #endregion
                    #region Fly
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Fly))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.Fly, stream);
                    #endregion
                    #region Intensify
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Intensify))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.Intensify, stream);
                    #endregion
                    #region MortalWound
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.MortalWound))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.MortalWound, stream);
                    #endregion
                    #region KineticSpark
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.KineticSpark))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.KineticSpark, stream);
                    #endregion
                    #region BlisteringWave
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.BlisteringWave))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.BlisteringWave, stream);
                    #endregion
                    #region SpiritFocus
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.SpiritFocus))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.SpiritFocus, stream);
                    #endregion
                    #region DaggerStorm
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.DaggerStorm))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.DaggerStorm, stream);
                    #endregion
                    #endregion
                    #region Pure Skill
                    #region StarArrow
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.StarArrow))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.StarArrow, stream);
                    #endregion
                    #endregion
                }
                #endregion
                #region (!)Ninja
                if (!Database.AtributesStatus.IsNinja(client.Player.Class))
                {
                    #region XP Skills
                    #region FatalStrike
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FatalStrike))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.FatalStrike, stream);
                    #endregion
                    #region ShurikenVortex
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ShurikenVortex))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.ShurikenVortex, stream);
                    #endregion
                    #endregion
                    #region Normal Skills
                    #region TwofoldBlades
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.TwofoldBlades))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.TwofoldBlades, stream);
                    #endregion
                    #region TwilightDance
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.TwilightDance))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.TwilightDance, stream);
                    #endregion
                    #region FatalSpin
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FatalSpin))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.FatalSpin, stream);
                    #endregion
                    #region BloodyScythe
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.BloodyScythe))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.BloodyScythe, stream);
                    #endregion
                    #region MortalDrag
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.MortalDrag))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.MortalDrag, stream);
                    #endregion
                    #region SuperTwofoldBlade
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.SuperTwofoldBlade))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.SuperTwofoldBlade, stream);
                    #endregion
                    #region ShadowClone
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ShadowClone))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.ShadowClone, stream);
                    #endregion
                    #region GapingWounds
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.GapingWounds))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.GapingWounds, stream);
                    #endregion
                    #region ToxicFog
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ToxicFog))
                    {
                        if (client.Player.FirstClass == 55 || client.Player.SecoundeClass == 55)
                        {

                        }
                        else
                        {
                            client.MySpells.Remove((ushort)Role.Flags.SpellID.ToxicFog, stream);
                        }
                    }
                    #endregion
                    #region ArcherBane
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ArcherBane))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.ArcherBane, stream);
                    #endregion
                    #region PoisonStar
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.PoisonStar))
                    {
                        if (client.Player.Class == 55 && client.Player.FirstClass == 55 && client.Player.Reborn == 1 ||
                            client.Player.Class == 55 && client.Player.SecoundeClass == 55 ||
                            client.Player.FirstClass == 55 && client.Player.SecoundeClass == 55)
                        {
                            //
                        }
                        else
                        {
                            client.MySpells.Remove((ushort)Role.Flags.SpellID.PoisonStar, stream);
                        }
                    }
                    #endregion
                    #endregion
                    #region Pure Skill
                    #region CounterKill
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.CounterKill))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.CounterKill, stream);
                    #endregion
                    #endregion
                }
                #endregion
                #region (!)Monk
                if (!Database.AtributesStatus.IsMonk(client.Player.Class))
                {
                    #region XP Skills
                    #region Oblivion
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Oblivion))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.Oblivion, stream);
                    #endregion
                    #endregion
                    #region Normal Skills
                    #region InfernalEcho
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.InfernalEcho))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.InfernalEcho, stream);
                    #endregion
                    #region WhirlwindKick
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.WhirlwindKick))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.WhirlwindKick, stream);
                    #endregion
                    #region TyrantAura
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.TyrantAura))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.TyrantAura, stream);
                    #endregion
                    #region FendAura
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FendAura))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.FendAura, stream);
                    #endregion
                    #region RadiantPalm
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.RadiantPalm))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.RadiantPalm, stream);
                    #endregion
                    #region Serenity
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Serenity))
                    {
                        if (client.Player.FirstClass == 65 || client.Player.SecoundeClass == 65)
                        {
                            //
                        }
                        else
                        {
                            client.MySpells.Remove((ushort)Role.Flags.SpellID.Serenity, stream);
                        }
                    }
                    #endregion
                    #region Tranquility
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Tranquility))
                    {
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.Tranquility, stream);
                    }
                    #endregion
                    #region MetalAura
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.MetalAura))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.MetalAura, stream);
                    #endregion
                    #region WoodAura
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.WoodAura))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.WoodAura, stream);
                    #endregion
                    #region WatherAura
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.WatherAura))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.WatherAura, stream);
                    #endregion
                    #region FireAura
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FireAura))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.FireAura, stream);
                    #endregion
                    #region EarthAura
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.EarthAura))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.EarthAura, stream);
                    #endregion
                    #endregion
                    #region Passive Skills
                    #region TripleAttack
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.TripleAttack))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.TripleAttack, stream);
                    #endregion
                    #region GraceofHeaven
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.GraceofHeaven))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.GraceofHeaven, stream);
                    #endregion
                    #region WrathoftheEmperor
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.WrathoftheEmperor))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.WrathoftheEmperor, stream);
                    #endregion
                    #endregion
                    #region Pure Skill
                    #region SoulShackle
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.SoulShackle))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.SoulShackle, stream);
                    #endregion
                    #endregion
                }
                #endregion
                #region (!)Pirate
                if (!Database.AtributesStatus.IsPirate(client.Player.Class))
                {
                    #region XP Skills
                    #region CannonBarrage
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.CannonBarrage))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.CannonBarrage, stream);
                    #endregion
                    #region BlackbeardsRage
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.BlackbeardsRage))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.BlackbeardsRage, stream);
                    #endregion
                    #endregion
                    #region Normal Skills
                    #region EagleEye
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.EagleEye))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.EagleEye, stream);
                    #endregion
                    #region BladeTempest
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.BladeTempest))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.BladeTempest, stream);
                    #endregion
                    #region GaleBomb
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.GaleBomb))
                    {
                        if (client.Player.FirstClass == 75 || client.Player.SecoundeClass == 75)
                        {
                            //
                        }
                        else
                        {
                            client.MySpells.Remove((ushort)Role.Flags.SpellID.GaleBomb, stream);
                        }
                    }
                    #endregion
                    #endregion
                    #region Passive Skills
                    #region AdrenalineRush
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.AdrenalineRush))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.AdrenalineRush, stream);
                    #endregion
                    #region Windstorm
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Windstorm))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.Windstorm, stream);
                    #endregion
                    #region BlackSpot
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Blackspot))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.Blackspot, stream);
                    #endregion
                    #endregion
                    #region Pure Skill
                    #region KrakensRevenge
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.KrakensRevenge))
                    {

                        client.MySpells.Remove((ushort)Role.Flags.SpellID.KrakensRevenge, stream);
                    }
                    #endregion
                    #region ScurvyBomb
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ScurvyBomb))
                    {
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.ScurvyBomb, stream);
                    }
                    #endregion
                    #endregion
                }
                #endregion
                #region (!)LeeLong
                if (!Database.AtributesStatus.IsLee(client.Player.Class))
                {
                    #region XP Skills
                    #region DragonCyclone
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.DragonCyclone))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.DragonCyclone, stream);
                    #endregion
                    #endregion
                    #region Normal Skills
                    #region DragonFury
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.DragonFury))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.DragonFury, stream);
                    #endregion
                    #region DragonPunch
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.DragonPunch))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.DragonPunch, stream);
                    #endregion
                    #region AirKick
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.AirKick))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.AirKick, stream);
                    #endregion
                    #region AirSweep
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.AirSweep))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.AirSweep, stream);
                    #endregion
                    #region AirRaid
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.AirRaid))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.AirRaid, stream);
                    #endregion
                    #region DragonFlow
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.DragonFlow))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.DragonFlow, stream);
                    #endregion
                    #region SpeedKick
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.SpeedKick))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.SpeedKick, stream);
                    #endregion
                    #region ViolentKick
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ViolentKick))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.ViolentKick, stream);
                    #endregion
                    #region StormKick
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.StormKick))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.StormKick, stream);
                    #endregion
                    #region DragonSwing
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.DragonSwing))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.DragonSwing, stream);
                    #endregion
                    #region DragonRoar
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.DragonRoar))
                    {
                        if (client.Player.FirstClass == 85 || client.Player.SecoundeClass == 85)
                        {
                            //
                        }
                        else
                        {
                            client.MySpells.Remove((ushort)Role.Flags.SpellID.DragonRoar, stream);
                        }
                    }
                    #endregion
                    #region CrackingSwipe
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.CrackingSwipe))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.CrackingSwipe, stream);
                    #endregion
                    #region DragonSlash
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.DragonSlash))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.DragonSlash, stream);
                    #endregion
                    #region SplittingSwipe
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.SplittingSwipe))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.SplittingSwipe, stream);
                    #endregion
                    #endregion
                    #region EarthSweep
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.EarthSweep))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.EarthSweep, stream);
                    #endregion
                    #region AirStrike
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.AirStrike))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.AirStrike, stream);
                    #endregion
                }
                #endregion
                #region (!)Water
                if (!Database.AtributesStatus.IsWater(client.Player.Class))
                {
                    #region XP Skills
                    #region ChainBolt
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ChainBolt))
                    {
                        if (client.Player.Class == 145)
                        {
                            //
                        }
                        else
                        {
                            client.MySpells.Remove((ushort)Role.Flags.SpellID.ChainBolt, stream);
                        }
                    }
                    #endregion
                    #region Lightning
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Lightning))
                    {
                        if (client.Player.Class == 145)
                        {
                            //
                        }
                        else
                        {
                            client.MySpells.Remove((ushort)Role.Flags.SpellID.Lightning, stream);
                        }
                    }
                    #endregion
                    #region Vulcano
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Vulcano))
                    {
                        if (client.Player.Class == 145)
                        {
                            //
                        }
                        else
                        {
                            client.MySpells.Remove((ushort)Role.Flags.SpellID.Vulcano, stream);
                        }
                    }
                    #endregion
                    #region SpeedLightning
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.SpeedLightning))
                    {
                        if (client.Player.Class == 145)
                        {
                            //
                        }
                        else
                        {
                            client.MySpells.Remove((ushort)Role.Flags.SpellID.SpeedLightning, stream);
                        }
                    }
                    #endregion
                    #region Revive
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Revive))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.Revive, stream);
                    #endregion
                    #region WaterElf
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.WaterElf))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.WaterElf, stream);
                    #endregion
                    #endregion
                    #region Normal Skills
                    //#region Thunder
                    //if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Thunder))
                    //{
                    //    if (client.Player.Class == 145)
                    //    {
                    //        //
                    //    }
                    //    else
                    //    {
                    //        client.MySpells.Remove((ushort)Role.Flags.SpellID.Thunder, stream);
                    //    }
                    //}
                    //#endregion
                    #region Cure
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Cure))
                    {
                        if (client.Player.FirstClass == 135 || client.Player.SecoundeClass == 135 ||
                            client.Player.Class == 145)
                        {
                            //
                        }
                        else
                        {
                            client.MySpells.Remove((ushort)Role.Flags.SpellID.Cure, stream);

                        }
                    }
                    #endregion
                    #region Meditation
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Meditation))
                    {
                        if (client.Player.FirstClass == 135 || client.Player.SecoundeClass == 135 ||
                            client.Player.Class == 145)
                        {
                            //
                        }
                        else
                        {
                            client.MySpells.Remove((ushort)Role.Flags.SpellID.Meditation, stream);
                        }
                    }
                    #endregion
                    #region HealingRain
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.HealingRain))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.HealingRain, stream);
                    #endregion
                    #region StarofAccuracy
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.StarofAccuracy))
                    {
                        if (client.Player.FirstClass == 135 || client.Player.SecoundeClass == 135)
                        {
                            //
                        }
                        else
                        {
                            client.MySpells.Remove((ushort)Role.Flags.SpellID.StarofAccuracy, stream);
                        }
                    }
                    #endregion
                    #region MagicShield
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.MagicShield))
                    {
                        if (client.Player.FirstClass == 135 || client.Player.SecoundeClass == 135)
                        {
                            //
                        }
                        else
                        {
                            client.MySpells.Remove((ushort)Role.Flags.SpellID.MagicShield, stream);
                        }
                    }
                    #endregion
                    #region Stigma
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Stigma))
                    {
                        if (client.Player.FirstClass == 135 || client.Player.SecoundeClass == 135)
                        {
                            //
                        }
                        else
                        {
                            client.MySpells.Remove((ushort)Role.Flags.SpellID.Stigma, stream);
                        }
                    }
                    #endregion
                    #region Invisibility
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Invisibility))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.Invisibility, stream);
                    #endregion
                    #region Pray
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Pray))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.Pray, stream);
                    #endregion
                    #region AdvancedCure
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.AdvancedCure))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.AdvancedCure, stream);
                    #endregion
                    #region Nectar
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Nectar))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.Nectar, stream);
                    #endregion
                    #region BlessingTouch
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.BlessingTouch))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.BlessingTouch, stream);
                    #endregion
                    #region AuroraLotus
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.AuroraLotus))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.AuroraLotus, stream);
                    #endregion
                    #endregion
                    #region Pure Skill
                    #region AzureShield
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.AzureShield))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.AzureShield, stream);
                    #endregion
                    #endregion
                }
                #endregion
                #region (!)Fire
                if (!Database.AtributesStatus.IsFire(client.Player.Class))
                {
                    #region XP Skills
                    #region ChainBolt
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ChainBolt))
                    {
                        if (client.Player.Class == 135)
                        {
                            //
                        }
                        else
                        {
                            client.MySpells.Remove((ushort)Role.Flags.SpellID.ChainBolt, stream);
                        }
                    }
                    #endregion
                    #region Lightning
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Lightning))
                    {
                        if (client.Player.Class == 135)
                        {
                            //
                        }
                        else
                        {
                            client.MySpells.Remove((ushort)Role.Flags.SpellID.Lightning, stream);
                        }
                    }
                    #endregion
                    #region Vulcano
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Vulcano))
                    {
                        if (client.Player.Class == 135)
                        {
                            //
                        }
                        else
                        {
                            client.MySpells.Remove((ushort)Role.Flags.SpellID.Vulcano, stream);
                        }
                    }
                    #endregion
                    #region SpeedLightning
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.SpeedLightning))
                    {
                        if (client.Player.Class == 135)
                        {
                            //
                        }
                        else
                        {
                            client.MySpells.Remove((ushort)Role.Flags.SpellID.SpeedLightning, stream);
                        }
                    }
                    #endregion
                    #endregion
                    #region Normal Skills
                    //#region Thunder
                    // if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Thunder))
                    // {
                    //if (client.Player.Class == 135)
                    //  {
                    //
                    //  }
                    //    else
                    //   {
                    //       client.MySpells.Remove((ushort)Role.Flags.SpellID.Thunder, stream);
                    //      }
                    //   }
                    //   #endregion
                    /*
                     #region Cure
                     if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Cure))
                     {
                        if (client.Player.FirstClass == 135 || client.Player.SecoundeClass == 135 || client.Player.Class == 135)
                         {
                            
                         }
                         else
                         {
                             client.MySpells.Remove((ushort)Role.Flags.SpellID.Cure, stream);
                    
                        }
                     }
                     #endregion
                     */

                    #region Fire
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Fire))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.Fire, stream);
                    #endregion


                    //  #region Meditation
                    //  if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Meditation))
                    //  {
                    //    if (client.Player.FirstClass == 135 || client.Player.SecoundeClass == 135 ||
                    //     client.Player.Class == 135)
                    //   {

                    //    }
                    //    else
                    //   {
                    //    client.MySpells.Remove((ushort)Role.Flags.SpellID.Meditation, stream);
                    //  }
                    //}
                    // #endregion

                    #region FireRing
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FireRing))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.FireRing, stream);
                    #endregion
                    #region FireMeteor
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FireMeteor))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.FireMeteor, stream);
                    #endregion
                    #region FireCircle
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FireCircle))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.FireCircle, stream);
                    #endregion
                    #region Tornado
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Tornado))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.Tornado, stream);
                    #endregion
                    #region Bomb
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Bomb))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.Bomb, stream);
                    #endregion
                    #region FireofHell
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FireofHell))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.FireofHell, stream);
                    #endregion
                    #region FlameLotus
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FlameLotus))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.FlameLotus, stream);
                    #endregion
                    #region SearingTouch
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.SearingTouch))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.SearingTouch, stream);
                    #endregion
                    #endregion
                    #region Pure Skill
                    #region HeavenBlade
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.HeavenBlade))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.HeavenBlade, stream);
                    #endregion
                    #endregion
                }
                #endregion
                #region (!)Windwalker
                if (!Database.AtributesStatus.IsWindWalker(client.Player.Class))
                {
                    #region AngerofStomper
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.AngerofStomper))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.AngerofStomper, stream);
                    #endregion
                    #region Omnipotence
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Omnipotence))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.Omnipotence, stream);
                    #endregion
                    #region JusticeChant
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.JusticeChant))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.JusticeChant, stream);
                    #endregion
                    #region BurntFrost
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.BurntFrost))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.BurntFrost, stream);
                    #endregion
                    #region HealingSnow
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.HealingSnow))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.HealingSnow, stream);
                    #endregion
                    #region SwirlingStorm
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.SwirlingStorm))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.SwirlingStorm, stream);
                    #endregion
                    #region ShadowofChaser
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ShadowofChaser))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.ShadowofChaser, stream);
                    #endregion
                    #region RageofWar
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.RageofWar))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.RageofWar, stream);
                    #endregion
                    #region HorrorofStomper
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.HorrorofStomper))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.HorrorofStomper, stream);
                    #endregion
                    #region TripleBlasts
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.TripleBlasts))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.TripleBlasts, stream);
                    #endregion
                    #region ChillingSnow
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ChillingSnow))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.ChillingSnow, stream);
                    #endregion
                    #region PeaceofStomper
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.PeaceofStomper))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.PeaceofStomper, stream);
                    #endregion
                    #region Thundercloud
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Thundercloud))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.Thundercloud, stream);
                    #endregion
                    #region RevengeTail
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.RevengeTail))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.RevengeTail, stream);
                    #endregion
                    #region FreezingPelter
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FreezingPelter))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.FreezingPelter, stream);
                    #endregion
                    #region Thunderbolt
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Thunderbolt))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.Thunderbolt, stream);
                    #endregion
                    #region FrostGazeI
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FrostGazeI))
                    {
                        if (client.Player.FirstClass == 165 || client.Player.SecoundeClass == 165)
                        {
                            //
                        }
                        else
                        {
                            client.MySpells.Remove((ushort)Role.Flags.SpellID.FrostGazeI, stream);
                        }
                    }
                    #endregion
                    #region FrostGazeII
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FrostGazeII))
                    {
                        if (client.Player.Class == 165 && client.Player.SecoundeClass == 165 ||
                            client.Player.Class == 165 && client.Player.FirstClass == 165 && client.Player.Reborn == 1)
                        {
                            //
                        }
                        else
                        {
                            client.MySpells.Remove((ushort)Role.Flags.SpellID.FrostGazeII, stream);
                        }
                    }
                    #endregion
                    #region FrostGazeIII
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FrostGazeIII))
                        client.MySpells.Remove((ushort)Role.Flags.SpellID.FrostGazeIII, stream);
                    #endregion
                }
                #endregion
                #endregion
            }
        }
        #endregion
        #region Items[Check]
        public static void CheckItems(Client.GameClient client)
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                foreach (var item in client.Equipment.ClientItems.Values)
                {
                    #region (!)Trojen
                    if (!Database.AtributesStatus.IsTrojan(client.Player.Class))
                    {
                        if (Database.ItemType.IsTrojanEpicWeapon(item.ITEM_ID) == false
                            ||
                            Database.ItemType.IsShield(item.ITEM_ID) == false)
                        {
                            client.Equipment.Remove(Role.Flags.ConquerItem.RightWeapon, stream);
                            client.Equipment.Remove(Role.Flags.ConquerItem.LeftWeapon, stream);
                            client.Equipment.RemoveAlternante(Role.Flags.ConquerItem.AleternanteRightWeapon, stream);
                            client.Equipment.RemoveAlternante(Role.Flags.ConquerItem.AleternanteLeftWeapon, stream);
                            client.Equipment.LeftWeapon = 0;
                            client.Equipment.RightWeapon = 0;
                        }
                        if (Database.ItemType.IsTrojanArmor(item.ITEM_ID) == false)
                        {
                            client.Equipment.Remove(Role.Flags.ConquerItem.Armor, stream);
                            client.Equipment.RemoveAlternante(Role.Flags.ConquerItem.AleternanteArmor, stream);
                        }
                        if (Database.ItemType.IsTrojanHat(item.ITEM_ID) == false)
                        {
                            client.Equipment.Remove(Role.Flags.ConquerItem.Head, stream);
                            client.Equipment.RemoveAlternante(Role.Flags.ConquerItem.AleternanteHead, stream);
                        }
                    }
                    #endregion
                    #region (!)Warrior
                    if (!Database.AtributesStatus.IsWarrior(client.Player.Class))
                    {
                        if (Database.ItemType.IsWarriorEpicWeapons(item.ITEM_ID) == false
                            ||
                            Database.ItemType.IsShield(item.ITEM_ID) == false)
                        {
                            client.Equipment.Remove(Role.Flags.ConquerItem.RightWeapon, stream);
                            client.Equipment.Remove(Role.Flags.ConquerItem.LeftWeapon, stream);
                            client.Equipment.RemoveAlternante(Role.Flags.ConquerItem.AleternanteRightWeapon, stream);
                            client.Equipment.RemoveAlternante(Role.Flags.ConquerItem.AleternanteLeftWeapon, stream);
                            client.Equipment.LeftWeapon = 0;
                            client.Equipment.RightWeapon = 0;
                        }
                        if (Database.ItemType.IsWarriorArmor(item.ITEM_ID) == false)
                        {
                            client.Equipment.Remove(Role.Flags.ConquerItem.Armor, stream);
                            client.Equipment.RemoveAlternante(Role.Flags.ConquerItem.AleternanteArmor, stream);
                        }
                        if (Database.ItemType.IsWarriorHat(item.ITEM_ID) == false)
                        {
                            client.Equipment.Remove(Role.Flags.ConquerItem.Head, stream);
                            client.Equipment.RemoveAlternante(Role.Flags.ConquerItem.AleternanteHead, stream);
                        }
                    }
                    #endregion
                    #region (!)Archer
                    if (!Database.AtributesStatus.IsArcher(client.Player.Class))
                    {
                        if (Database.ItemType.IsKnife(item.ITEM_ID) == false ||
                            Database.ItemType.IsShield(item.ITEM_ID) == false)
                        {
                            client.Equipment.Remove(Role.Flags.ConquerItem.RightWeapon, stream);
                            client.Equipment.Remove(Role.Flags.ConquerItem.LeftWeapon, stream);
                            client.Equipment.RemoveAlternante(Role.Flags.ConquerItem.AleternanteRightWeapon, stream);
                            client.Equipment.RemoveAlternante(Role.Flags.ConquerItem.AleternanteLeftWeapon, stream);
                            client.Equipment.LeftWeapon = 0;
                            client.Equipment.RightWeapon = 0;
                        }
                        if (Database.ItemType.IsArcherArmor(item.ITEM_ID) == false)
                        {
                            client.Equipment.Remove(Role.Flags.ConquerItem.Armor, stream);
                            client.Equipment.RemoveAlternante(Role.Flags.ConquerItem.AleternanteArmor, stream);
                        }
                        if (Database.ItemType.IsArcherHat(item.ITEM_ID) == false)
                        {
                            client.Equipment.Remove(Role.Flags.ConquerItem.Head, stream);
                            client.Equipment.RemoveAlternante(Role.Flags.ConquerItem.AleternanteHead, stream);
                        }
                    }
                    #endregion
                    #region (!)Ninja
                    if (!Database.AtributesStatus.IsNinja(client.Player.Class))
                    {
                        if (Database.ItemType.IsKatana(item.ITEM_ID) == false ||
                            Database.ItemType.IsShield(item.ITEM_ID) == false || Database.ItemType.IsNinjaEpicWeapon(item.ITEM_ID) == false)
                        {
                            client.Equipment.Remove(Role.Flags.ConquerItem.RightWeapon, stream);
                            client.Equipment.Remove(Role.Flags.ConquerItem.LeftWeapon, stream);
                            client.Equipment.RemoveAlternante(Role.Flags.ConquerItem.AleternanteRightWeapon, stream);
                            client.Equipment.RemoveAlternante(Role.Flags.ConquerItem.AleternanteLeftWeapon, stream);
                            client.Equipment.LeftWeapon = 0;
                            client.Equipment.RightWeapon = 0;
                        }
                        if (Database.ItemType.IsNinjaArmor(item.ITEM_ID) == false)
                        {
                            client.Equipment.Remove(Role.Flags.ConquerItem.Armor, stream);
                            client.Equipment.RemoveAlternante(Role.Flags.ConquerItem.AleternanteArmor, stream);
                        }
                        if (Database.ItemType.IsNinjaHat(item.ITEM_ID) == false)
                        {
                            client.Equipment.Remove(Role.Flags.ConquerItem.Head, stream);
                            client.Equipment.RemoveAlternante(Role.Flags.ConquerItem.AleternanteHead, stream);
                        }
                    }
                    #endregion
                    #region (!)Monk
                    if (!Database.AtributesStatus.IsMonk(client.Player.Class))
                    {
                        if (Database.ItemType.IsMonkWeapon(item.ITEM_ID) == false ||
                            Database.ItemType.IsShield(item.ITEM_ID) == false || Database.ItemType.IsMonkEpicWeapon(item.ITEM_ID) == false)
                        {
                            client.Equipment.Remove(Role.Flags.ConquerItem.RightWeapon, stream);
                            client.Equipment.Remove(Role.Flags.ConquerItem.LeftWeapon, stream);
                            client.Equipment.RemoveAlternante(Role.Flags.ConquerItem.AleternanteRightWeapon, stream);
                            client.Equipment.RemoveAlternante(Role.Flags.ConquerItem.AleternanteLeftWeapon, stream);
                            client.Equipment.LeftWeapon = 0;
                            client.Equipment.RightWeapon = 0;
                        }
                        if (Database.ItemType.IsMonkArmor(item.ITEM_ID) == false)
                        {
                            client.Equipment.Remove(Role.Flags.ConquerItem.Armor, stream);
                            client.Equipment.RemoveAlternante(Role.Flags.ConquerItem.AleternanteArmor, stream);
                        }
                        if (Database.ItemType.IsMonkHat(item.ITEM_ID) == false)
                        {
                            client.Equipment.Remove(Role.Flags.ConquerItem.Head, stream);
                            client.Equipment.RemoveAlternante(Role.Flags.ConquerItem.AleternanteHead, stream);
                        }
                    }
                    #endregion
                    #region (!)Pirate
                    if (!Database.AtributesStatus.IsPirate(client.Player.Class))
                    {
                        if (Database.ItemType.IsPistol(item.ITEM_ID) == false ||
                            Database.ItemType.IsShield(item.ITEM_ID) == false || Database.ItemType.IsRapier(item.ITEM_ID) == false)
                        {
                            client.Equipment.Remove(Role.Flags.ConquerItem.RightWeapon, stream);
                            client.Equipment.Remove(Role.Flags.ConquerItem.LeftWeapon, stream);
                            client.Equipment.RemoveAlternante(Role.Flags.ConquerItem.AleternanteRightWeapon, stream);
                            client.Equipment.RemoveAlternante(Role.Flags.ConquerItem.AleternanteLeftWeapon, stream);
                            client.Equipment.LeftWeapon = 0;
                            client.Equipment.RightWeapon = 0;
                        }
                        if (Database.ItemType.IsPirateArmor(item.ITEM_ID) == false)
                        {
                            client.Equipment.Remove(Role.Flags.ConquerItem.Armor, stream);
                            client.Equipment.RemoveAlternante(Role.Flags.ConquerItem.AleternanteArmor, stream);
                        }
                        if (Database.ItemType.IsPirateHat(item.ITEM_ID) == false)
                        {
                            client.Equipment.Remove(Role.Flags.ConquerItem.Head, stream);
                            client.Equipment.RemoveAlternante(Role.Flags.ConquerItem.AleternanteHead, stream);
                        }
                    }
                    #endregion
                    #region (!)DragonWarrior
                    if (!Database.AtributesStatus.IsLee(client.Player.Class))
                    {
                        if (Database.ItemType.IsDragonWarriorWeapon(item.ITEM_ID) == false
                            ||
                            Database.ItemType.IsShield(item.ITEM_ID) == false)
                        {
                            client.Equipment.Remove(Role.Flags.ConquerItem.RightWeapon, stream);
                            client.Equipment.Remove(Role.Flags.ConquerItem.LeftWeapon, stream);
                            client.Equipment.RemoveAlternante(Role.Flags.ConquerItem.AleternanteRightWeapon, stream);
                            client.Equipment.RemoveAlternante(Role.Flags.ConquerItem.AleternanteLeftWeapon, stream);
                            client.Equipment.LeftWeapon = 0;
                            client.Equipment.RightWeapon = 0;
                        }
                        if (Database.ItemType.IsDragonWarriorArmor(item.ITEM_ID) == false)
                        {
                            client.Equipment.Remove(Role.Flags.ConquerItem.Armor, stream);
                            client.Equipment.RemoveAlternante(Role.Flags.ConquerItem.AleternanteArmor, stream);
                        }
                        if (Database.ItemType.IsDragonWarriorHat(item.ITEM_ID) == false)
                        {
                            client.Equipment.Remove(Role.Flags.ConquerItem.Head, stream);
                            client.Equipment.RemoveAlternante(Role.Flags.ConquerItem.AleternanteHead, stream);
                        }
                    }
                    #endregion
                    #region (!)Taoist
                    if (!Database.AtributesStatus.IsTaoist(client.Player.Class))
                    {
                        if (Database.ItemType.IsTaoistEpicWeapon(item.ITEM_ID) == false ||
                            Database.ItemType.IsHossu(item.ITEM_ID) == false ||
                            Database.ItemType.IsBacksword(item.ITEM_ID) == false
                            ||
                            Database.ItemType.IsShield(item.ITEM_ID) == false)
                        {

                            client.Equipment.Remove(Role.Flags.ConquerItem.RightWeapon, stream);
                            client.Equipment.Remove(Role.Flags.ConquerItem.LeftWeapon, stream);
                            client.Equipment.RemoveAlternante(Role.Flags.ConquerItem.AleternanteRightWeapon, stream);
                            client.Equipment.RemoveAlternante(Role.Flags.ConquerItem.AleternanteLeftWeapon, stream);
                            client.Equipment.LeftWeapon = 0;
                            client.Equipment.RightWeapon = 0;
                        }
                        if (Database.ItemType.IsTaoistArmor(item.ITEM_ID) == false)
                        {
                            client.Equipment.Remove(Role.Flags.ConquerItem.Armor, stream);
                            client.Equipment.RemoveAlternante(Role.Flags.ConquerItem.AleternanteArmor, stream);
                        }
                        if (Database.ItemType.IsTaoistHat(item.ITEM_ID) == false)
                        {
                            client.Equipment.Remove(Role.Flags.ConquerItem.Head, stream);
                            client.Equipment.RemoveAlternante(Role.Flags.ConquerItem.AleternanteHead, stream);
                        }
                        if (Database.ItemType.IsTaoistNecklace(item.ITEM_ID) == false)
                        {
                            client.Equipment.Remove(Role.Flags.ConquerItem.Necklace, stream);
                            client.Equipment.RemoveAlternante(Role.Flags.ConquerItem.AleternanteNecklace, stream);
                        }
                        if (Database.ItemType.IsTaoistRing(item.ITEM_ID) == false)
                        {
                            client.Equipment.Remove(Role.Flags.ConquerItem.Ring, stream);
                            client.Equipment.RemoveAlternante(Role.Flags.ConquerItem.AleternanteRing, stream);
                        }
                    }
                    #endregion
                    #region (!)Windwalker
                    if (!Database.AtributesStatus.IsWindWalker(client.Player.Class))
                    {
                        if (Database.ItemType.IsWindWalkerWeapon(item.ITEM_ID) == false
                            ||
                            Database.ItemType.IsShield(item.ITEM_ID) == false)
                        {
                            client.Equipment.Remove(Role.Flags.ConquerItem.RightWeapon, stream);
                            client.Equipment.Remove(Role.Flags.ConquerItem.LeftWeapon, stream);
                            client.Equipment.RemoveAlternante(Role.Flags.ConquerItem.AleternanteRightWeapon, stream);
                            client.Equipment.RemoveAlternante(Role.Flags.ConquerItem.AleternanteLeftWeapon, stream);
                            client.Equipment.LeftWeapon = 0;
                            client.Equipment.RightWeapon = 0;
                        }
                        if (Database.ItemType.IsWindWalkerArmor(item.ITEM_ID) == false)
                        {
                            client.Equipment.Remove(Role.Flags.ConquerItem.Armor, stream);
                            client.Equipment.RemoveAlternante(Role.Flags.ConquerItem.AleternanteArmor, stream);
                        }
                        if (Database.ItemType.IsWindWalkerHat(item.ITEM_ID) == false)
                        {
                            client.Equipment.Remove(Role.Flags.ConquerItem.Head, stream);
                            client.Equipment.RemoveAlternante(Role.Flags.ConquerItem.AleternanteHead, stream);
                        }
                    }
                    #endregion
                    client.Equipment.QueryEquipment(client.Equipment.Alternante, true);
                    client.Equipment.SendAlowAlternante(stream);
                }
            }
        }
        #endregion
    }
}
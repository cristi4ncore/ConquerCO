using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeathWish.Game.MsgServer.AttackHandler.Calculate
{
    public class Magic
    {
        public static void OnMonster(Role.Player player, MsgMonster.MonsterRole monster, Database.MagicType.Magic DBSpell, out MsgSpellAnimation.SpellObj SpellObj)
        {


            SpellObj = new MsgSpellAnimation.SpellObj(monster.UID, 0, MsgAttackPacket.AttackEffect.None);


            if (monster.IsFloor)
            {
                SpellObj.Damage = 1;
                return;
            }

            SpellObj.Damage = player.Owner.AjustMagicAttack();
            if (player.Owner.Status.MagicPercent > 0)
            {
                SpellObj.Damage += (uint)((SpellObj.Damage * player.Owner.Status.MagicPercent / 100));
            }
            if (DBSpell != null)
                SpellObj.Damage += (uint)DBSpell.Damage;//(uint)((SpellObj.Damage * DBSpell.Damage) / 100);
            if (player.Level >= monster.Level)
                SpellObj.Damage = (uint)(SpellObj.Damage * 1.8);

            if (SpellObj.Damage > monster.Family.Defense)
                SpellObj.Damage -= monster.Family.Defense;
            else
                SpellObj.Damage = 1;

            bool moneffect = false;
            if (Base.Rate2(player.Owner.PerfectionStatus.LuckyStrike))
            {
                if (!moneffect)
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        player.View.SendView(stream.MsgRefineEffectCreate(new MsgRefineEffect.RefineEffectProto()
                        {
                            Effect = MsgRefineEffect.RefineEffects.LuckyStrike,
                            Id = player.UID
                        }), true);
                    }
                    SpellObj.Effect = MsgAttackPacket.AttackEffect.LuckyStrike;
                    SpellObj.Damage = (uint)Base.MulDiv((int)SpellObj.Damage, 200, 100);
                    moneffect = true;
                }
            }
            
            if (Base.Rate2(player.Owner.PerfectionStatus.AbsoluteLuck))
            {
                if (!moneffect)
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        player.View.SendView(stream.MsgRefineEffectCreate(new MsgRefineEffect.RefineEffectProto()
                        {
                            Effect = MsgRefineEffect.RefineEffects.AbsoluteLuck,
                            Id = player.UID
                        }), true);
                    }
                    SpellObj.Effect = MsgAttackPacket.AttackEffect.LuckyStrike;
                    SpellObj.Damage = (uint)Base.MulDiv((int)SpellObj.Damage, 200, 100);
                    moneffect = true;
                }
            }
            if (player.Owner.PerfectionStatus.KillingFlash > 0)
            {
                if (!moneffect)
                {
                    if (Base.Rate2(player.Owner.PerfectionStatus.KillingFlash))
                    {

                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            player.View.SendView(stream.MsgRefineEffectCreate(new MsgRefineEffect.RefineEffectProto()
                            {
                                Effect = MsgRefineEffect.RefineEffects.KillingFlash,
                                Id = player.UID,
                                dwParam = player.UID
                            }), true);
                        }
                        if (!player.ContainFlag(MsgUpdate.Flags.XPList) && player.OnXPSkill() == MsgUpdate.Flags.Normal)
                            player.AddFlag(MsgUpdate.Flags.XPList, 20, true);
                        moneffect = true;
                    }
                }
            }
            if (player.Owner.PerfectionStatus.CalmWind > 0)
            {
                if (!moneffect)
                {
                    if (Base.Rate2(player.Owner.PerfectionStatus.CalmWind))
                    {
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            player.View.SendView(stream.MsgRefineEffectCreate(new MsgRefineEffect.RefineEffectProto()
                            {
                                Effect = MsgRefineEffect.RefineEffects.CalmWind,
                                Id = player.UID,
                                dwParam = player.UID
                            }), true);
                            moneffect = true;
                        }

                    }
                }
            }
            if (player.Owner.PerfectionStatus.DrainingTouch > 0)
            {
                if (!moneffect)
                {
                    if (Base.Rate2(player.Owner.PerfectionStatus.DrainingTouch))
                    {
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            player.View.SendView(stream.MsgRefineEffectCreate(new MsgRefineEffect.RefineEffectProto()
                            {
                                Effect = MsgRefineEffect.RefineEffects.DrainingTouch,
                                Id = player.UID,
                                dwParam = player.UID
                            }), true);

                            bool update1 = false;
                            if (player.HitPoints < player.Owner.Status.MaxHitpoints)
                            {
                                update1 = true;
                                player.HitPoints = (int)player.Owner.Status.MaxHitpoints;
                            }
                            if (player.Mana < player.Owner.Status.MaxMana)
                            {
                                update1 = true;
                                player.Mana = (ushort)player.Owner.Status.MaxMana;
                            }
                            if (update1)
                            {
                                player.SendUpdateHP();
                                player.SendUpdate(stream, player.Mana, MsgUpdate.DataType.Mana, false);
                            }
                            moneffect = true;
                        }
                    }
                }

            }
          
            if (Base.GetRefinery())
            {


                if (player.Owner.Status.SkillCStrike > 0)
                {
                    if (!moneffect)
                    {
                        SpellObj.Effect |= MsgAttackPacket.AttackEffect.CriticalStrike;

                        SpellObj.Damage += (SpellObj.Damage * (player.Owner.AjustMCriticalStrike() / 100)) / 100;
                        moneffect = true;
                    }
                }

            }


            SpellObj.Damage = (uint)Base.CalcDamageUser2Monster((int)SpellObj.Damage, monster.Family.Defense, player.Level, monster.Level, false);
            SpellObj.Damage = (uint)Base.AdjustMinDamageUser2Monster((int)SpellObj.Damage, player.Owner);

            SpellObj.Damage += player.Owner.Status.MagicDamageIncrease;

            if (monster.Family.Defense2 == 0)
                SpellObj.Damage = 1;

            if ((monster.Family.Settings & MsgMonster.MonsterSettings.Guard) == MsgMonster.MonsterSettings.Guard)
                SpellObj.Damage /= 10;
            if (monster.Family.ID == 2700 || monster.Family.ID == 2699)
                SpellObj.Damage = 1;
            if (monster.Family.ID == 60915)
                SpellObj.Damage = 10000;
            if (monster.Family.ID == 20160)
                SpellObj.Damage = 10000;
        }
        public static void OnPlayer(Role.Player player, Role.Player target, Database.MagicType.Magic DBSpell, out MsgSpellAnimation.SpellObj SpellObj)
        {
            int Damage = (int)Base.GetDamage(player.Owner.AjustMaxAttack((uint)player.Owner.Status.MaxAttack), player.Owner.Status.MinAttack);
            Damage = (int)player.Owner.AjustAttack((uint)Damage);

            bool update = false;
            bool LunchEffect = false;
            if (DBSpell.ID == (ushort)Role.Flags.SpellID.Poison)//Poison
            {
                Damage = Base.MulDiv((int)Damage, 45, 100);
                update = true;
            }
            SpellObj = new MsgSpellAnimation.SpellObj(target.UID, 0, MsgAttackPacket.AttackEffect.None);
            if (target.ContainFlag(MsgUpdate.Flags.ManiacDance))
            {
                SpellObj.Damage = 1;
                return;
            }
            if (target.ContainFlag(MsgUpdate.Flags.ShurikenVortex))
            {
                SpellObj.Damage = 1;
                return;
            }
            if (target.ContainFlag(MsgUpdate.Flags.MagicDefender))
            {
                SpellObj.Damage = 1;
                SpellObj.Effect = MsgAttackPacket.AttackEffect.Imunity;
                return;
            }
            SpellObj.Damage = player.Owner.Status.MagicAttack;
            if (target.ContainFlag(MsgUpdate.Flags.lianhuaran01))
                SpellObj.Damage += 800;
            else if (target.ContainFlag(MsgUpdate.Flags.lianhuaran02))
                SpellObj.Damage += 1500;
            else if (target.ContainFlag(MsgUpdate.Flags.lianhuaran03))
                SpellObj.Damage += 2000;
            if (DBSpell != null)
                SpellObj.Damage += (uint)DBSpell.Damage;

            if (player.Owner.Status.MagicPercent > 0)
            {
                SpellObj.Damage = (uint)((SpellObj.Damage * (player.Owner.Status.MagicPercent) / 123));
            }


            SpellObj.Damage = Calculate.Base.CalculateBless(SpellObj.Damage, target.Owner.Status.ItemBless);

            bool onbreak = false;
            int breakdif = 0;
            if (!update && player.Owner.AjustBreakthrough() > 0)
            {
                if (player.BattlePower < target.BattlePower)
                {
                    breakdif = (int)((float)player.Owner.AjustBreakthrough() / 10f - (float)target.Owner.AjustAntiBreack() / 10f);
                    if (Base.GetRefinery(player.Owner.AjustBreakthrough() / 10, target.Owner.AjustAntiBreack() / 10))
                    {
                        onbreak = true;
                        Damage = Base.MulDiv((int)Damage, 80, 100);
                        SpellObj.Effect |= MsgAttackPacket.AttackEffect.Break;
                        update = true;

                    }
                }
            }
            if (onbreak == false)
            {
                var olddamage = Damage;
                Damage = Database.Disdain.UserAttackUser(player, target, Damage);
                if (Damage < olddamage)
                {
                    Damage += olddamage / -10;
                }
            }



            uint MagicDefemce = target.Owner.Status.MagicDefence;
            uint MagicPercent = target.Owner.Status.MDefence;
            if (MagicPercent > player.Owner.Status.Penetration / 100)
                MagicPercent -= player.Owner.Status.Penetration / 100;
            else
                MagicPercent = 1;

            MagicDefemce += MagicDefemce * MagicPercent / 100;




            SpellObj.Damage = Calculate.Base.CalcaulateDeffence(SpellObj.Damage, MagicDefemce);

            SpellObj.Damage = Calculate.Base.CalculateExtraAttack(SpellObj.Damage, player.Owner.Status.MagicDamageIncrease, target.Owner.Status.MagicDamageDecrease);

            int reduction = Base.MulDiv((int)target.Owner.GemValues(Role.Flags.Gem.NormalTortoiseGem), 64, 100);

            SpellObj.Damage = (uint)Base.MulDiv((int)SpellObj.Damage, (int)(100 - Math.Min(67, reduction)), 100);


            uint m_strike = player.Owner.Status.SkillCStrike;
            if (m_strike > 0)
            {
                if (Base.Rate2(player.Owner.PerfectionStatus.CoreStrike))
                {
                    if (!LunchEffect)
                    {
                        if (m_strike < player.Owner.Status.Immunity)
                        {
                            m_strike = player.Owner.Status.Immunity;
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                player.View.SendView(stream.MsgRefineEffectCreate(new MsgRefineEffect.RefineEffectProto()
                                {
                                    Effect = MsgRefineEffect.RefineEffects.CoreStrike,
                                    Id = player.UID,
                                    dwParam = player.UID
                                }), true);
                                LunchEffect = true;
                            }
                        }
                    }
                }
                if (player.Owner.Status.PrestigeLevel > target.Owner.Status.PrestigeLevel && Base.Rate2(player.Owner.PerfectionStatus.AbsoluteLuck))
                {
                    if (!LunchEffect)
                    {
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            player.View.SendView(stream.MsgRefineEffectCreate(new MsgRefineEffect.RefineEffectProto()
                            {
                                Effect = MsgRefineEffect.RefineEffects.AbsoluteLuck,
                                Id = player.UID
                            }), true);
                        }
                        SpellObj.Effect = MsgAttackPacket.AttackEffect.LuckyStrike;
                        SpellObj.Damage = (uint)Base.MulDiv((int)SpellObj.Damage, 200, 100);
                        LunchEffect = true;                    }
                }
                if (Base.Rate2(player.Owner.PerfectionStatus.LuckyStrike))
                {
                    if (!LunchEffect)
                    {
                        if (Base.Rate2(target.Owner.PerfectionStatus.StrikeLock))
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                target.View.SendView(stream.MsgRefineEffectCreate(new MsgRefineEffect.RefineEffectProto()
                                {
                                    Effect = MsgRefineEffect.RefineEffects.StrikeLockLevel,
                                    Id = player.UID
                                }), true);
                                LunchEffect = true;
                            }
                        }
                        else
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                target.View.SendView(stream.MsgRefineEffectCreate(new MsgRefineEffect.RefineEffectProto()
                                {
                                    Effect = MsgRefineEffect.RefineEffects.LuckyStrike,
                                    Id = player.UID
                                }), true);
                            }
                            SpellObj.Effect |= MsgAttackPacket.AttackEffect.LuckyStrike;
                            SpellObj.Damage = (uint)Base.MulDiv((int)SpellObj.Damage, 200, 100);
                            LunchEffect = true;
                        }
                    }

                }
                
                if (Base.GetRefinery(m_strike / 100, target.Owner.Status.Immunity / 100))
                {
                    if (!LunchEffect)
                    {
                        SpellObj.Effect |= MsgAttackPacket.AttackEffect.CriticalStrike;
                        SpellObj.Damage = (uint)Base.MulDiv((int)SpellObj.Damage, 150, 100);
                        LunchEffect = true;
                    }
                }


            }
            if (player.Owner.PerfectionStatus.DivineGuard > 0)
            {
                if (!LunchEffect)
                {
                    if (player.Owner.Status.PrestigeLevel > target.Owner.Status.PrestigeLevel)
                    {
                        if (AttackHandler.Calculate.Base.Rate2(player.Owner.PerfectionStatus.DivineGuard))
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                target.View.SendView(stream.MsgRefineEffectCreate(new MsgRefineEffect.RefineEffectProto()
                                {
                                    Effect = MsgRefineEffect.RefineEffects.DivineGuard,
                                    Id = player.UID,
                                    dwParam = player.UID
                                }), true);

                                if (!player.ContainFlag(MsgUpdate.Flags.DivineGuard))
                                {

                                    player.AddFlag(MsgUpdate.Flags.DivineGuard, 10, true);
                                    player.SendUpdate(stream, MsgUpdate.Flags.DivineGuard, 10, 0, 0, MsgUpdate.DataType.AzureShield);

                                }
                                LunchEffect = true;
                            }
                        }
                    }
                }
            }
            if (player.Owner.PerfectionStatus.CalmWind > 0)
            {
                if (!LunchEffect)
                {
                    if (AttackHandler.Calculate.Base.Rate2(player.Owner.PerfectionStatus.CalmWind))
                    {
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            target.View.SendView(stream.MsgRefineEffectCreate(new MsgRefineEffect.RefineEffectProto()
                            {
                                Effect = MsgRefineEffect.RefineEffects.CalmWind,
                                Id = player.UID,
                                dwParam = player.UID
                            }), true);
                            LunchEffect = true;
                        }

                    }
                }
            }
            if (player.Owner.PerfectionStatus.KillingFlash > 0)
            {
                if (!LunchEffect)
                {
                    if (AttackHandler.Calculate.Base.Rate2(player.Owner.PerfectionStatus.KillingFlash))
                    {
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            target.View.SendView(stream.MsgRefineEffectCreate(new MsgRefineEffect.RefineEffectProto()
                            {
                                Effect = MsgRefineEffect.RefineEffects.KillingFlash,
                                Id = player.UID,
                                dwParam = player.UID
                            }), true);
                        }
                        if (!player.ContainFlag(MsgUpdate.Flags.XPList) && player.OnXPSkill() == MsgUpdate.Flags.Normal)
                            player.AddFlag(MsgUpdate.Flags.XPList, 20, true);
                        LunchEffect = true;
                    }
                }
            }
            if (player.Owner.PerfectionStatus.DrainingTouch > 0)
            {
                if (!LunchEffect)
                {
                    if (AttackHandler.Calculate.Base.Rate2(player.Owner.PerfectionStatus.DrainingTouch))
                    {
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            target.View.SendView(stream.MsgRefineEffectCreate(new MsgRefineEffect.RefineEffectProto()
                            {
                                Effect = MsgRefineEffect.RefineEffects.DrainingTouch,
                                Id = player.UID,
                                dwParam = player.UID
                            }), true);

                            bool update1 = false;
                            if (player.HitPoints < player.Owner.Status.MaxHitpoints)
                            {
                                update1 = true;
                                player.HitPoints = (int)player.Owner.Status.MaxHitpoints;
                            }
                            if (player.Mana < player.Owner.Status.MaxMana)
                            {
                                update1 = true;
                                player.Mana = (ushort)player.Owner.Status.MaxMana;
                            }
                            if (update1)
                            {
                                player.SendUpdateHP();
                                player.SendUpdate(stream, player.Mana, MsgUpdate.DataType.Mana, false);
                            }
                            LunchEffect = true;
                        }
                    }
                }

            }
            if (player.Owner.PerfectionStatus.InvisibleArrow > 0)
            {
                if (!LunchEffect)
                {
                    if (Base.Rate2(player.Owner.PerfectionStatus.InvisibleArrow))
                    {
                        if (player.Owner.Status.Penetration > 0)
                            SpellObj.Damage += (uint)(SpellObj.Damage * (int)Base.MulDiv((int)player.Owner.Status.Penetration, 5, 100) / 100);
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            target.View.SendView(stream.MsgRefineEffectCreate(new MsgRefineEffect.RefineEffectProto()
                            {
                                Effect = MsgRefineEffect.RefineEffects.InvisbleArrow,
                                Id = player.UID,
                                dwParam = player.UID
                            }), true);
                            LunchEffect = true;
                        }
                    }
                }
            }


            if (target.ContainFlag(MsgUpdate.Flags.AzureShield))
            {
                if (SpellObj.Damage > target.AzureShieldDefence)
                {
                    Calculate.AzureShield.CreateDmg(player, target, target.AzureShieldDefence);
                    target.RemoveFlag(MsgUpdate.Flags.AzureShield);
                    SpellObj.Damage -= target.AzureShieldDefence;

                }
                else
                {
                    target.AzureShieldDefence -= (ushort)SpellObj.Damage;
                    Calculate.AzureShield.CreateDmg(player, target, SpellObj.Damage);
                    SpellObj.Damage = 1;
                }
            }
            MsgSpellAnimation.SpellObj InRedirect;
            if (BackDmg.Calculate(player, target, DBSpell, SpellObj.Damage, out InRedirect))
                SpellObj = InRedirect;
            if (target.Owner.Equipment.ShieldID != 0)
            {
                int Block = (int)(target.Owner.Status.Block / 100);
                Block += (int)((target.ShieldBlockDamage * Block) / 100);
                uint Change = (uint)Math.Min(70, Block / 2);
                if (player.Owner.PerfectionStatus.ShieldBreak > 0)
                {
                    //if (Base.Rate(1))
                    if (Base.Rate2(player.Owner.PerfectionStatus.ShieldBreak))
                    {
                        if (!LunchEffect)
                        {
                            if (!target.ContainFlag(MsgUpdate.Flags.ShieldBreak))
                            {
                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    target.SendUpdate(stream, MsgUpdate.Flags.ShieldBreak, 15, 0, 0, MsgUpdate.DataType.AzureShield);
                                }
                                target.AddFlag(MsgUpdate.Flags.ShieldBreak, 15, true);
                                LunchEffect = true;
                            }
                        }
                    }
                }
                if (target.ContainFlag(MsgUpdate.Flags.ShieldBreak))
                {
                    if (Change > 20)
                        Change -= 20;
                    else
                        Change = 0;
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        player.View.SendView(stream.MsgRefineEffectCreate(new MsgRefineEffect.RefineEffectProto()
                        {
                            Effect = MsgRefineEffect.RefineEffects.ShiledBreak,
                            Id = player.UID,
                            dwParam = player.UID
                        }), true);
                    }
                }
                if (Base.Rate((int)Change))
                {
                    SpellObj.Effect |= MsgAttackPacket.AttackEffect.Block;
                    SpellObj.Damage /= 2;
                }
            }
            if (target.MyClones.Count != 0)
            {
                foreach (var clone in target.MyClones.GetValues())
                    clone.RemoveThat(target.Owner);
                target.MyClones.Clear();
            }
        }
        public static void OnNpcs(Role.Player player, Role.SobNpc target, Database.MagicType.Magic DBSpell, out MsgSpellAnimation.SpellObj SpellObj)
        {
            SpellObj = new MsgSpellAnimation.SpellObj(target.UID, 0, MsgAttackPacket.AttackEffect.None);

            SpellObj.Damage = player.Owner.Status.MagicAttack;
            if (player.Owner.Status.MagicPercent > 0)
            {
                SpellObj.Damage += (uint)((SpellObj.Damage * player.Owner.Status.MagicPercent / 100));
            }

            if (Base.GetRefinery())
            {

                if (player.Owner.Status.SkillCStrike > 0)
                {
                    SpellObj.Effect |= MsgAttackPacket.AttackEffect.CriticalStrike;

                    SpellObj.Damage += (SpellObj.Damage * (player.Owner.Status.SkillCStrike / 100)) / 100;
                }

            }
            SpellObj.Damage = Calculate.Base.CalculateExtraAttack(SpellObj.Damage, player.Owner.Status.MagicDamageIncrease, 0);

            if (target.ContainFlag(MsgUpdate.Flags.AzureShield))
                SpellObj.Damage = 100;

        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeathWish.Game.MsgServer.AttackHandler.Calculate
{
    public class Physical
    {
        public static void OnMonster(Role.Player player, MsgMonster.MonsterRole monster, Database.MagicType.Magic DBSpell, out MsgSpellAnimation.SpellObj SpellObj, byte MultipleDamage = 0)
        {

            SpellObj = new MsgSpellAnimation.SpellObj(monster.UID, 0, MsgAttackPacket.AttackEffect.None);
            if (monster.IsFloor)
            {
                SpellObj.Damage = 2;
                return;
            }

            //if (DBSpell == null)
            //{
            //    if (Base.Dodged(player.Owner, monster))
            //    {
            //        SpellObj.Damage = 0;
            //        return;
            //    }
            //}

            int Damage = (int)Base.GetDamage(player.Owner.Status.MaxAttack, player.Owner.Status.MinAttack);

            Damage = (int)player.Owner.AjustAttack((uint)Damage);
            Damage = (int)player.Owner.AjustMaxAttack((uint)Damage);

            var rawDefense = monster.Family.Defense;

            Damage = Math.Max(0, Damage - rawDefense);

            if (DBSpell != null && DBSpell.Damage < 10)
                DBSpell.Damage = 10;
            if (player.ContainFlag(MsgUpdate.Flags.FatalStrike))
            {
                Damage = Base.MulDiv((int)Damage, 500, 100);
            }
            else if (DBSpell != null && DBSpell.ID == (ushort)Role.Flags.SpellID.Omnipotence)
            {
                Damage = Base.MulDiv((int)Damage, 350, 100);
            }
            else if (MultipleDamage != 0)
            {
                Damage = Damage * MultipleDamage;
            }
            else
            {
                if (DBSpell != null && DBSpell.ID == 12770)
                {
                    if (monster.Boss == 1)
                        Damage = Base.MulDiv((int)Damage, 350, 100);
                    else
                        Damage = Base.MulDiv((int)Damage, 500, 100);
                }
                else
                    Damage = Base.MulDiv((int)Damage, (int)((DBSpell != null) ? DBSpell.Damage : Program.ServerConfig.PhysicalDamage), 100);
            }
            if (player.ContainFlag(MsgUpdate.Flags.ManiacDance))
            {
                if (monster.Boss == 0)
                    Damage *= 10;
                else
                    Damage *= 3;
            }


            if (player.ContainFlag(MsgUpdate.Flags.Oblivion))
                Damage = Base.MulDiv((int)Damage, 200, 100);



            Damage = Base.AdjustMinDamageUser2Monster(Damage, player.Owner);
            Damage = Base.CalcDamageUser2Monster(Damage, monster.Family.Defense, player.Level, monster.Level, false);


            Damage = (int)Base.BigMulDiv(Damage, monster.Family.Defense2, Client.GameClient.DefaultDefense2);


            if (monster.Family.Defense2 > 0)
                Damage = (int)Calculate.Base.CalculateExtraAttack((uint)Damage, player.Owner.AjustPhysicalDamageIncrease(), 0);


                SpellObj.Damage = (uint)Math.Max(1, Damage);

                if (player.ContainFlag(MsgUpdate.Flags.Superman))
                    SpellObj.Damage *= 5;
            bool moneffect = false;
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
                    SpellObj.Effect |= MsgAttackPacket.AttackEffect.LuckyStrike;
                    SpellObj.Damage = (uint)Base.MulDiv((int)SpellObj.Damage, 200, 100);
                    moneffect = true;
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
                if (player.Owner.Status.CriticalStrike > 0)
                {
                    if (!moneffect)
                    {
                        SpellObj.Effect |= MsgAttackPacket.AttackEffect.CriticalStrike;
                        SpellObj.Damage += (SpellObj.Damage * (player.Owner.AjustCriticalStrike() / 100)) / 100;
                        moneffect = true;
                    }
                }
            }

            if ((monster.Family.Settings & MsgMonster.MonsterSettings.Guard) == MsgMonster.MonsterSettings.Guard)
                SpellObj.Damage /= 10;
            if (monster.Family.ID == 2700 || monster.Family.ID == 2699)
                SpellObj.Damage = 1;
            if (monster.Family.ID == 60915)
                SpellObj.Damage = 10000;
            if (monster.Family.ID == 20160)
                SpellObj.Damage = 10000;
            
        }
        public static void OnPlayer(Role.Player player, Role.Player target, Database.MagicType.Magic DBSpell, out MsgSpellAnimation.SpellObj SpellObj, bool StackOver = false, int IncreaseAttack = 0)
        {
            SpellObj = new MsgSpellAnimation.SpellObj(target.UID, 0, MsgAttackPacket.AttackEffect.None);
            if (target.ContainFlag(MsgUpdate.Flags.ShurikenVortex) || target.ContainFlag(MsgUpdate.Flags.ManiacDance))
            {
                SpellObj.Damage = 1;
                return;
            }
            bool update = false;
            bool LunchEffect = false;
            if (DBSpell == null)
            {
                if (Base.Dodged(player.Owner, target.Owner))
                {
                    SpellObj.Damage = 0;
                    return;
                }
            }
            unsafe
            {
                using (var rect = new ServerSockets.RecycledPacket())
                {
                    var stream = rect.GetStream();
                    ActionQuery action = new ActionQuery()
                    {
                        ObjId = target.UID,
                        Type = ActionType.HideGui,
                    };
                    target.Owner.Send(stream.ActionCreate(&action));
                }
            }

            int Damage = (int)Base.GetDamage(player.Owner.Status.MaxAttack, player.Owner.Status.MinAttack) / 2;


            Damage = (int)player.Owner.AjustAttack((uint)Damage);
          //  Damage = (int)player.Owner.AjustMaxAttack((uint)Damage);
          //  Damage = (uint)player.Owner.AjustWeaponProf((uint)Damage);

            var rawDefense = target.Owner.AjustDefense;
            if (Damage > rawDefense)
                Damage -= (int)rawDefense;
            else
                Damage = 1;

            if (DBSpell != null)
            {
                Damage += (int)DBSpell.Damage;
            }
            if (SpellObj.Damage > 0)
            {
                if (DBSpell != null)
                {
                    int val = 0;
                    if (Database.AttackCompatetor.CheckDmg(DBSpell.ID, out val))
                    {
                        int dmg = (int)SpellObj.Damage;
                        SpellObj.Damage = (uint)Base.MulDiv((int)dmg, val, 100);
                    }
                }
            }
            if (!update)
            {
                if (DBSpell != null)
                    Damage = Base.MulDiv((int)Damage, (int)((DBSpell != null) ? DBSpell.Damage : Program.ServerConfig.PhysicalDamage), 100);

            }
            bool onbreak = false;
            update = false;
            if (player.Owner.Status.CriticalStrike > 0)
            {
                if (!LunchEffect)
                {
                    if (!update && Base.GetRefinery(player.Owner.Status.CriticalStrike / 100, target.Owner.Status.Immunity / 100))
                    {
                        SpellObj.Effect |= MsgAttackPacket.AttackEffect.CriticalStrike;
                        Damage = Base.MulDiv((int)Damage, 150, 100);
                        LunchEffect = true;
                    }
                    if (player.Owner.Status.PrestigeLevel > target.Owner.Status.PrestigeLevel && Base.Rate2(player.Owner.PerfectionStatus.AbsoluteLuck))
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
                        LunchEffect = true;
                    }
                    if (!update && Base.Rate2(player.Owner.PerfectionStatus.LuckyStrike))
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
                            Damage = Base.MulDiv((int)Damage, 200, 100);
                            update = true;
                            LunchEffect = true;

                        }
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
            if (!update && player.Owner.Status.Breakthrough > 0)
            {
                if (!LunchEffect)
                {
                    if (player.BattlePower < target.BattlePower)
                    {
                        if (Base.GetRefinery(player.Owner.Status.Breakthrough / 10, target.Owner.Status.Counteraction / 10))
                        {
                            onbreak = true;
                            SpellObj.Effect |= MsgAttackPacket.AttackEffect.Break;
                            LunchEffect = true;

                        }
                    }
                }
            }
            if (onbreak == false)
                Damage = Database.Disdain.UserAttackUser(player, target, Damage);

            var TortoisePercent = target.Owner.GemValues(Role.Flags.Gem.NormalTortoiseGem);
            if (TortoisePercent > 0)
                if (Database.AtributesStatus.IsWater(player.Class))
                {
                    Damage -= Damage * Math.Min((int)TortoisePercent, 45) / 100;
                }
            else
                    Damage -= Damage * Math.Min((int)TortoisePercent, 30) / 100;

            if (target.Reborn > 0)
                Damage = (int)Base.BigMulDiv((int)Damage, 7000, Client.GameClient.DefaultDefense2);

            Damage -= (int)(Damage * target.Owner.Status.ItemBless / 100);
            Damage = (int)Calculate.Base.CalculateExtraAttack((uint)Damage, player.Owner.Status.PhysicalDamageIncrease, target.Owner.Status.PhysicalDamageDecrease);         

            SpellObj.Damage = (uint)Math.Max(1, Damage);
            //using (var rect = new ServerSockets.RecycledPacket())
            //{
            //    var stream = rect.GetStream();
            //    if (Base.Rate1(1))
            //    {
            //        player.SendString(stream, MsgStringPacket.StringID.Effect, true, "LuckyGuy");
            //        SpellObj.Damage = (uint)Base.MulDiv((int)SpellObj.Damage, 200, 100);
            //    }
            //}
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
            if (CheckAttack.BlockRefect.CanUseReflect(player.Owner))
            {
              //  if (!StackOver)
             //   {
                    MsgSpellAnimation.SpellObj InRedirect;
                    if (BackDmg.Calculate(player, target, DBSpell, SpellObj.Damage, out InRedirect))
                        SpellObj = InRedirect;
              //  }
            }
            if (target.Owner.Equipment.ShieldID != 0)
            {
                int Block = (int)(target.Owner.Status.Block / 100);
                Block += (int)((target.ShieldBlockDamage * Block) / 100);
                uint Change = (uint)Math.Min(70, Block / 2);
                if (player.Owner.PerfectionStatus.ShieldBreak > 0)
                    if (!LunchEffect)
                    {
                        if (Base.Rate2(player.Owner.PerfectionStatus.ShieldBreak))
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
                if (Base.Rate((byte)Change))
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

            int Damage = (int)Base.GetDamage(player.Owner.Status.MaxAttack, player.Owner.Status.MinAttack);

            Damage = (int)player.Owner.AjustAttack((uint)Damage);

            Damage = Base.MulDiv((int)Damage, (int)((DBSpell != null) ? DBSpell.Damage : Program.ServerConfig.PhysicalDamage), 100);

            SpellObj.Damage = (uint)Math.Max(1, Damage);

            if (Base.GetRefinery())
            {
                if (player.Owner.Status.CriticalStrike > 0)
                {
                    SpellObj.Effect |= MsgAttackPacket.AttackEffect.CriticalStrike;
                    SpellObj.Damage = Base.CalculateArtefactsDmg(SpellObj.Damage, player.Owner.Status.CriticalStrike, 0);
                }
            }
            SpellObj.Damage = Calculate.Base.CalculateExtraAttack(SpellObj.Damage, player.Owner.Status.PhysicalDamageIncrease, 0);
            if (target.ContainFlag(MsgUpdate.Flags.AzureShield))
                SpellObj.Damage = 100;

        }
    }
}

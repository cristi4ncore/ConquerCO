using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeathWish.Game.MsgServer.AttackHandler.ReceiveAttack
{
    public class Player
    {
        public static void Execute(MsgSpellAnimation.SpellObj obj, Client.GameClient client, Role.Player attacked)
        {        
            if (MsgTournaments.MsgSchedules.CurrentTournament.Type == MsgTournaments.TournamentType.SkillTournament)
            {
                if (MsgTournaments.MsgSchedules.CurrentTournament.Process == MsgTournaments.ProcesType.Alive)
                {
                    if (MsgTournaments.MsgSchedules.CurrentTournament.InTournament(client))
                    {
                        var tournament = Game.MsgTournaments.MsgSchedules.CurrentTournament as Game.MsgTournaments.MsgSkillTournament;
                        tournament.KillSystem.Update(client);
                        tournament.KillSystem.CheckDead(attacked.UID);

                        if (attacked.SkillTournamentLifes > 0)
                        {
                            attacked.SkillTournamentLifes -= 1;
                         //   attacked.Owner.SendSysMesage("You have " + attacked.SkillTournamentLifes + " more life's left.", MsgMessage.ChatMode.Center, MsgMessage.MsgColor.red);
                            return;
                        }
                        if (attacked.SkillTournamentLifes == 0)
                        {
                            client.Player.TournamentKills += 1;
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                attacked.SendString(stream, MsgStringPacket.StringID.Effect, true, "accession1");
                                attacked.HitPoints = 0;
                                attacked.Dead(client.Player, attacked.X, attacked.Y, client.Player.UID);
                            }
                        }
                      
                    }
                }
            }
            if (MsgTournaments.MsgSchedules.CurrentTournament.Type == MsgTournaments.TournamentType.FreezeWar)
            {
                if (MsgTournaments.MsgSchedules.CurrentTournament.Process == MsgTournaments.ProcesType.Alive)
                {
                    if (MsgTournaments.MsgSchedules.CurrentTournament.InTournament(client))
                    {
                        if (client.Player.GarmentId != attacked.GarmentId)
                        {
                            if (attacked.ContainFlag(MsgUpdate.Flags.Freeze) == false)
                                attacked.AddFlag(MsgUpdate.Flags.Freeze, Role.StatusFlagsBigVector32.PermanentFlag, true);
                                 client.Player.PIKAPoint3 += 1;

                        }
                        else
                        {
                            if (client.Player.GarmentId == attacked.GarmentId)
                                if (attacked.ContainFlag(MsgUpdate.Flags.Freeze))
                                    attacked.RemoveFlag(MsgUpdate.Flags.Freeze);
                        }
                        return;
                    }
                }
            }
            if (MsgTournaments.MsgSchedules.CurrentTournament.Type == MsgTournaments.TournamentType.KillTheCaptain)
            {
                if (MsgTournaments.MsgSchedules.CurrentTournament.Process == MsgTournaments.ProcesType.Alive)
                {
                    if (MsgTournaments.MsgSchedules.CurrentTournament.InTournament(client))
                    {
                        if (client.TeamKillTheCaptain == attacked.Owner.TeamKillTheCaptain)
                            return;
                    }
                }
            }
            if (client.Player.Map == 2567 || client.Player.Map == 2579 || client.Player.Map == 2569 || client.Player.Map == 8601 || client.Player.Map == 8602 || client.Player.Map == 8603 || client.Player.Map == 8604 || client.Player.Map == 7701 || client.Player.Map == 7702 || client.Player.Map == 7703 || client.Player.Map == 7704 || client.Player.Map == 7705 || client.Player.Map == 7706 || client.Player.Map == 7707 || client.Player.Map == 7708 || client.Player.Map == 7709 || client.Player.Map == 7710 || client.Player.Map == 7711 || client.Player.Map == 7712)
            {
                if (attacked.Arenapika > 0)
                {
                    attacked.Arenapika -= 1;
                    return;
                }
                if (attacked.Arenapika == 0)
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        attacked.HitPoints = 0;
                        attacked.Dead(client.Player, attacked.X, attacked.Y, client.Player.UID);
                    }
                }
            }
            if (MsgTournaments.MsgSchedules.CurrentTournament.Type == MsgTournaments.TournamentType.FootBall)
            {
                var tournament = (MsgTournaments.MsgFootball)MsgTournaments.MsgSchedules.CurrentTournament;
                if (tournament.InTournament(client))
                {
                    if (attacked.ContainFlag(MsgUpdate.Flags.lianhuaran04))
                        tournament.PassTheBall(client, attacked.Owner);
                    return;
                }
            }
            if (client.EventBase != null)
                if (client.EventBase.NoDamage && client.EventBase.Stage == MsgEvents.EventStage.Fighting)
                    obj.Damage = client.EventBase.GetDamage(client, attacked.Owner);
            if (client.EventBase != null)
                if (client.EventBase.Stage != MsgEvents.EventStage.Fighting)
                    obj.Damage = 0;
            #region MirrorOfSin
            if (attacked.Owner.PerfectionStatus.MirrorOfSin > 0)
            {
                if (AttackHandler.Calculate.Base.Rate1(attacked.Owner.PerfectionStatus.MirrorOfSin))
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        attacked.View.SendView(stream.MsgRefineEffectCreate(new MsgRefineEffect.RefineEffectProto()
                        {
                            Effect = MsgRefineEffect.RefineEffects.MirrorOfSin,
                            Id = attacked.UID,
                            dwParam = attacked.UID
                        }), true);
                        if (attacked.OnXPSkill() == MsgUpdate.Flags.Normal && !client.Player.ContainFlag(MsgUpdate.Flags.XPList))
                            attacked.AddFlag(MsgUpdate.Flags.XPList, 20, true);
                    }
                }
            }
            #endregion
            #region LightOfStamina
            if (attacked.Owner.PerfectionStatus.LightOfStamina > 0)
            {
                if (attacked.Owner.PrestigeLevel > client.PrestigeLevel)
                {
                    if (AttackHandler.Calculate.Base.Rate2(attacked.Owner.PerfectionStatus.LightOfStamina))
                    {
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            attacked.View.SendView(stream.MsgRefineEffectCreate(new MsgRefineEffect.RefineEffectProto()
                            {
                                Effect = MsgRefineEffect.RefineEffects.LightOfStamina,
                                Id = attacked.UID,
                                dwParam = attacked.UID
                            }), true);
                            if (attacked.Stamina < 100)
                            {
                                attacked.Stamina = 100;
                                attacked.SendUpdate(stream, attacked.Stamina, MsgUpdate.DataType.Stamina);
                            }
                        }
                    }
                }
            }
            #endregion
            #region BloodSpawn
            if (attacked.Owner.PerfectionStatus.BloodSpawn > 0)
            {
                if (AttackHandler.Calculate.Base.Rate1(attacked.Owner.PerfectionStatus.BloodSpawn))
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        attacked.View.SendView(stream.MsgRefineEffectCreate(new MsgRefineEffect.RefineEffectProto()
                        {
                            Effect = MsgRefineEffect.RefineEffects.BloodSpawn,
                            Id = attacked.UID,
                            dwParam = attacked.UID
                        }), true);
                        bool update = false;
                        if (attacked.HitPoints < attacked.Owner.Status.MaxHitpoints)
                        {
                            update = true;
                            attacked.HitPoints = (int)attacked.Owner.Status.MaxHitpoints;
                        }
                        if (attacked.Mana < attacked.Owner.Status.MaxMana)
                        {
                            update = true;
                            attacked.Mana = (ushort)attacked.Owner.Status.MaxMana;
                        }
                        if (update)
                        {
                            attacked.SendUpdateHP();
                            attacked.SendUpdate(stream, attacked.Mana, MsgUpdate.DataType.Mana, false);
                        }
                    }
                }
            }
            #endregion
            if (Calculate.Base.Rate(10))
            {
                CheckAttack.CheckItems.RespouseDurability(client);
            }
            ushort X = attacked.X;
            ushort Y = attacked.Y;

            //if (Game.MsgTournaments.MsgSchedules.CurrentTournament.Type == MsgTournaments.TournamentType.DragonWar)
            //{
            //    if (Game.MsgTournaments.MsgSchedules.CurrentTournament.Process == MsgTournaments.ProcesType.Alive)
            //    {
            //        if (Game.MsgTournaments.MsgSchedules.CurrentTournament.InTournament(client))
            //        {
            //            var tournament = (Game.MsgTournaments.MsgDragonWar)Game.MsgTournaments.MsgSchedules.CurrentTournament;
            //            if (attacked.ContainFlag(MsgUpdate.Flags.DragonSwing))
            //            {
            //                if (attacked.DragonWarHits == 0)
            //                {
            //                    tournament.KillSystem.CheckDead(client.Player.UID);
            //                    attacked.Dead(client.Player, X, Y, 0);

            //                    client.Player.DragonWarScore += 20;
            //                    client.Player.AddFlag(MsgUpdate.Flags.DragonSwing, Role.StatusFlagsBigVector32.PermanentFlag, true);
            //                    client.Player.DragonWarHits = 2;
            //                    attacked.RemoveFlag(MsgUpdate.Flags.DragonSwing);
            //                }
            //                else
            //                {

            //                    client.Player.DragonWarScore += 5;
            //                    attacked.DragonWarHits -= 1;
            //                }
            //            }
            //            else if (client.Player.ContainFlag(MsgUpdate.Flags.DragonSwing))
            //            {
            //                tournament.KillSystem.Update(client);
            //                client.Player.DragonWarScore += 15;
            //                attacked.Dead(client.Player, X, Y, 0);
            //            }
            //            return;

            //        }
            //    }
            //}
            if (attacked.Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.AdrenalineRush))
            {
                if (attacked.Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.EagleEye))
                {
                    Database.MagicType.Magic DBSpellz = new Database.MagicType.Magic();

                    MsgSpellAnimation RemoveCloudDown = new MsgSpellAnimation(attacked.Owner.Player.UID
            , 0, attacked.Owner.Player.X, attacked.Owner.Player.Y, 11130
            , 4, 0);
                    if (DBSpellz.CoolDown > 0 && Role.Core.Rate(100) && attacked.eagles==true)
                    {
                        RemoveCloudDown.Targets.Enqueue(new MsgSpellAnimation.SpellObj() { UID = attacked.Owner.Player.UID, Damage = 11030, Hit = 1 });
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            RemoveCloudDown.SetStream(stream);

                            RemoveCloudDown.Send(attacked.Owner);
                            DBSpellz.CoolDown = 0;
                            DBSpellz.ColdTime = 0;

                        }


                    }
                }

            }
            if (attacked.Action == Role.Flags.ConquerAction.Sit)
            {
                if (attacked.Owner.Player.Stamina > 20)
                {
                    byte Stm = (byte)attacked.Owner.Player.Stamina;
                    attacked.Owner.Player.Stamina -= (ushort)(Stm / 2);
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        client.Player.SendUpdate(stream, attacked.Stamina, MsgUpdate.DataType.Stamina);
                    }



                }
            }
            if (attacked.HitPoints <= obj.Damage)
            {
                if (client.Player.OnTransform)
                {
                    client.Player.TransformInfo.FinishTransform();
                }
                attacked.Dead(client.Player, X, Y, 0);

            }
            else
            {
                CheckAttack.CheckGemEffects.CheckRespouseDamage(attacked.Owner);
                client.UpdateQualifier(client, attacked.Owner, obj.Damage);
                if (client.EventBase != null)
                    if (client.EventBase.Stage == MsgEvents.EventStage.Fighting)
                        client.EventBase.Hit(client, attacked.Owner);
                attacked.HitPoints -= (int)obj.Damage;
            }


        }
    }
}

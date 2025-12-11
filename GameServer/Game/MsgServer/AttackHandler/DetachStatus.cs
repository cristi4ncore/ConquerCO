using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeathWish.Game.MsgServer.AttackHandler
{
   public class DetachStatus
    {
       public unsafe static void Execute(Client.GameClient user, InteractQuery Attack, ServerSockets.Packet stream, Dictionary<ushort, Database.MagicType.Magic> DBSpells)
       {
           Database.MagicType.Magic DBSpell;
           MsgSpell ClientSpell;
           if (CheckAttack.CanUseSpell.Verified(Attack,user, DBSpells, out ClientSpell, out DBSpell))
           {
               switch (ClientSpell.ID)
               {
                   case (ushort)Role.Flags.SpellID.ArcherBane:
                       {
                           MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                           , 0, Attack.X, Attack.Y, ClientSpell.ID
                           , ClientSpell.Level, ClientSpell.UseSpellSoul);


                           Role.IMapObj target;
                           if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Player))
                           {
                               Role.Player attacked = target as Role.Player;
                               if (attacked.ContainFlag(MsgUpdate.Flags.Fly))
                               {
                                   if (Calculate.Base.Rate(40))
                                   {
                                       attacked.RemoveFlag(MsgUpdate.Flags.Fly);
                                       MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(attacked.UID, 30, MsgAttackPacket.AttackEffect.None));
                                   }
                                   else
                                   {
                                       var clientobj = new MsgSpellAnimation.SpellObj(attacked.UID, MsgSpell.SpellID, MsgAttackPacket.AttackEffect.None);
                                       clientobj.Hit = 0;
                                       MsgSpell.Targets.Enqueue(clientobj);
                                   }
                               }
                           }
                           MsgSpell.SetStream(stream);
                           MsgSpell.Send(user);

                           Updates.UpdateSpell.CheckUpdate(stream,user,Attack, 250, DBSpells);

                           break;
                       }
                   case (ushort)Role.Flags.SpellID.Revive:
                       {
                           if (user.IsWatching())
                           {
                 

                               user.SendSysMesage("This spell not work on this map..");
                               
                               break;
                           }
                           if (user.Player.Map == 700)
                           {
                               user.SendSysMesage("You can't use this skill right now!");
                               break;

                           }
                           if (user.Player.Map == 2578 || user.Player.Map == 7272 || user.Player.Map == 2579 || user.Player.Map == 7273 || user.Player.Map == 7274 || user.Player.Map == 7275 || user.Player.Map == 1082 || user.Player.Map == 7202 || user.Player.Map == 2572 || user.Player.Map == 2573 || user.Player.Map == 2568 || user.Player.Map == 2570 || user.Player.Map == 7721 || user.Player.Map == 7722 || user.Player.Map == 7723 || user.Player.Map == 7724 || user.Player.Map == 7725 || user.Player.Map == 7726)
                           {
                               user.SendSysMesage("You can't use this skill right now!");
                               break;

                           }
                           if (user.Player.GuildRank != Role.Flags.GuildMemberRank.Member
                               && user.Player.GuildRank != Role.Flags.GuildMemberRank.GuildLeader
                               && user.Player.GuildRank != Role.Flags.GuildMemberRank.DeputyLeader
                               && user.Player.GuildRank != Role.Flags.GuildMemberRank.HDeputyLeader
                               && user.Player.GuildRank != Role.Flags.GuildMemberRank.Manager
                               && user.Player.GuildRank != Role.Flags.GuildMemberRank.LeaderSpouse
                               && user.Player.GuildRank != Role.Flags.GuildMemberRank.HonoraryManager


                               && user.Player.GuildRank != Role.Flags.GuildMemberRank.TSupervisor
                               && user.Player.GuildRank != Role.Flags.GuildMemberRank.OSupervisor
                               && user.Player.GuildRank != Role.Flags.GuildMemberRank.CPSupervisor
                               && user.Player.GuildRank != Role.Flags.GuildMemberRank.ASupervisor
                               && user.Player.GuildRank != Role.Flags.GuildMemberRank.SSupervisor
                               && user.Player.GuildRank != Role.Flags.GuildMemberRank.GSupervisor


                               && user.Player.GuildRank != Role.Flags.GuildMemberRank.PKSupervisor
                               && user.Player.GuildRank != Role.Flags.GuildMemberRank.RoseSupervisor
                               && user.Player.GuildRank != Role.Flags.GuildMemberRank.LilySupervisor
                               && user.Player.GuildRank != Role.Flags.GuildMemberRank.Supervisor
                               && user.Player.GuildRank != Role.Flags.GuildMemberRank.HonorarySuperv
                               && user.Player.GuildRank != Role.Flags.GuildMemberRank.Steward


                               && user.Player.GuildRank != Role.Flags.GuildMemberRank.HonorarySteward
                               && user.Player.GuildRank != Role.Flags.GuildMemberRank.DeputySteward
                               && user.Player.GuildRank != Role.Flags.GuildMemberRank.DLeaderSpouse
                               && user.Player.GuildRank != Role.Flags.GuildMemberRank.DLeaderAide
                               && user.Player.GuildRank != Role.Flags.GuildMemberRank.LSpouseAide
                               && user.Player.GuildRank != Role.Flags.GuildMemberRank.Aide


                               && user.Player.GuildRank != Role.Flags.GuildMemberRank.TulipAgent
                               && user.Player.GuildRank != Role.Flags.GuildMemberRank.OrchidAgent
                               && user.Player.GuildRank != Role.Flags.GuildMemberRank.CPAgent
                               && user.Player.GuildRank != Role.Flags.GuildMemberRank.ArsenalAgent
                               && user.Player.GuildRank != Role.Flags.GuildMemberRank.SilverAgent
                               && user.Player.GuildRank != Role.Flags.GuildMemberRank.GuideAgent)
                           //{
                           //    //user.SendSysMesage(" No Rev Enemy..");
                           //    //break;
                           //}
                           if (user.Player.Name.Contains("[GM]"))
                               break;

                           MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                           , 0, Attack.X, Attack.Y, ClientSpell.ID
                           , ClientSpell.Level, ClientSpell.UseSpellSoul);

                           user.Player.RemoveFlag(MsgUpdate.Flags.XPList);

                           Role.IMapObj target;
                           if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Player))
                           {
                               Role.Player attacked = target as Role.Player;
                               MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(attacked.UID, 0, MsgAttackPacket.AttackEffect.None));
                               attacked.Revive(stream);
                           }


                           MsgSpell.SetStream(stream);
                           MsgSpell.Send(user);

                           Updates.UpdateSpell.CheckUpdate(stream,user,Attack, DBSpell.Duration, DBSpells);


                           if (!user.Equipment.FreeEquip((Role.Flags.ConquerItem)4) && user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.BlessingTouch))
                           {

                               MsgGameItem BackSword;
                               if (user.Equipment.TryGetEquip(Role.Flags.ConquerItem.RightWeapon, out BackSword))
                               {
                                   if (Database.ItemType.IsTaoistEpicWeapon(BackSword.ITEM_ID))
                                   {
                                       if (user.Player.TaoistPower < 10)
                                       {
                                           user.Player.TaoistPower += 1;
                                           user.Player.UpdateTaoPower(stream);
                                       }

                                       byte change = 10;

                                       MsgGameItem hossus;
                                       if (user.Equipment.TryGetEquip(Role.Flags.ConquerItem.LeftWeapon, out hossus))
                                       {
                                           if (Database.ItemType.IsHossu(hossus.ITEM_ID))
                                           {
                                               var dbItem = Database.Server.ItemsBase[hossus.ITEM_ID];
                                               change += (byte)(dbItem.Level / 3);
                                           }
                                       }

                                       if (AttackHandler.Calculate.Base.Rate(change))
                                       {
                                           if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Player))
                                           {
                                               Role.Player attacked = target as Role.Player;

                                               attacked.AddSpellFlag(MsgUpdate.Flags.Stigma, (int)80, true);
                                               attacked.AddSpellFlag(MsgUpdate.Flags.MagicShield, (int)120, true);
                                               attacked.AddSpellFlag(MsgUpdate.Flags.StarOfAccuracy, (int)60, true);


                                               Attack.SpellID = (ushort)Role.Flags.SpellID.BlessingTouch;
                                               var Spell = Database.Server.Magic[Attack.SpellID];

                                               Updates.UpdateSpell.CheckUpdate(stream, user, Attack, (uint)(change * 100), Spell);

                                           }
                                       }
                                   }
                               }
                           }

                           break;
                       }
                   case (ushort)Role.Flags.SpellID.Pray:
                       {
                           if (user.IsWatching())
                           {
                               user.SendSysMesage("This spell not work on this map..");
                               break;
                           }
                           if (user.Player.Map == 2579 || user.Player.Map == 2578 || user.Player.Map == 7272 || user.Player.Map == 7273 || user.Player.Map == 7274 || user.Player.Map == 7275 || user.Player.Map == 1082 || user.Player.Map == 5061 || user.Player.Map == 7202 || user.Player.Map == 2572 || user.Player.Map == 2573 || user.Player.Map == 2568 || user.Player.Map == 2570 || user.Player.Map == 7721 || user.Player.Map == 7722 || user.Player.Map == 7723 || user.Player.Map == 7724 || user.Player.Map == 7725 || user.Player.Map == 7726)
                           {
                               user.SendSysMesage("You can't use this skill right now!");
                               break;

                           }                          
                           if (user.Player.Name.Contains("[GM]"))
                               break;
                           MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                           , 0, Attack.X, Attack.Y, ClientSpell.ID
                           , ClientSpell.Level, ClientSpell.UseSpellSoul);



                           Role.IMapObj target;

                           if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Player))
                           {
                               Role.Player attacked = target as Role.Player;
                               var attaced = target as Role.Player;
                               if (CheckAttack.CanAttackPlayer.Verified(user, attaced, DBSpell))
                                   break;
                               if (attacked.ContainFlag(MsgUpdate.Flags.SoulShackle))
                               {
                                   user.SendSysMesage("This spell not work on SoulShackle..");
                                   break;
                               }
                               if (user.Player.PkMode == Role.Flags.PKMode.Team)
                               {
                                   if ((attacked.MyGuild != null && user.Player.MyGuild != null))
                                   {
                                       if (attacked.MyGuild.Info.GuildID == user.Player.MyGuild.Info.GuildID || user.Player.MyGuild.Ally.ContainsKey(attacked.MyGuild.Info.GuildID)
                                           || user.Player.ClanUID == attacked.ClanUID || user.Player.MyClan.Ally.ContainsKey(attacked.ClanUID)
                                           || user.Player.Associate.Contain(Role.Instance.Associate.Friends, attacked.UID))
                                       {
                                           MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(attacked.UID, 0, MsgAttackPacket.AttackEffect.None));
                                           attacked.Revive(stream);
                                            if (Game.MsgTournaments.MsgSchedules.GuildWar.Proces != Game.MsgTournaments.ProcesType.Dead)
                                            {
                                                if (attacked.Map == 1038)
                                                {
                                                    if (Ranksss.newranks.ContainsKey(attacked.UID))
                                                        Ranksss.newranks[attacked.UID].MostRevive += 1;
                                                    else
                                                        Ranksss.newranks.Add(attacked.UID, new Ranksss.mostafa()
                                                        {
                                                            NamePlayer = attacked.Name,
                                                            MostRevive = 1,
                                                        });
                                                }
                                            }
                                        }
                                       else
                                       {
                                           user.SendSysMesage("You Can`t Pray Enemy While You Make PK Team Mode..");
                                           return;
                                       }
                                   }
                                   else
                                   {
                                       user.SendSysMesage("You Can`t Pray Any One Haven`t Guild While You Make PK Team Mode..");
                                       return;
                                   }

                               }
                               MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(attacked.UID, 0, MsgAttackPacket.AttackEffect.None));
                               attacked.Revive(stream);
                                if (Game.MsgTournaments.MsgSchedules.GuildWar.Proces != Game.MsgTournaments.ProcesType.Dead)
                                {
                                    if (attacked.Map == 1038)
                                    {
                                        if (Ranksss.newranks.ContainsKey(attacked.UID))
                                            Ranksss.newranks[attacked.UID].MostRevive += 1;
                                        else
                                            Ranksss.newranks.Add(attacked.UID, new Ranksss.mostafa()
                                            {
                                                NamePlayer = attacked.Name,
                                                MostRevive = 1,
                                            });
                                    }
                                }
                            }

                           MsgSpell.SetStream(stream);
                           MsgSpell.Send(user);

                           Updates.UpdateSpell.CheckUpdate(stream, user, Attack, DBSpell.Duration, DBSpells);



                           if (!user.Equipment.FreeEquip((Role.Flags.ConquerItem)4) && user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.BlessingTouch))
                           {

                               MsgGameItem BackSword;
                               if (user.Equipment.TryGetEquip(Role.Flags.ConquerItem.RightWeapon, out BackSword))
                               {
                                   if (Database.ItemType.IsTaoistEpicWeapon(BackSword.ITEM_ID))
                                   {
                                       if (user.Player.TaoistPower < 10)
                                       {
                                           user.Player.TaoistPower += 1;
                                           user.Player.UpdateTaoPower(stream);
                                       }

                                       byte change = 10;

                                       MsgGameItem hossus;
                                       if (user.Equipment.TryGetEquip(Role.Flags.ConquerItem.LeftWeapon, out hossus))
                                       {
                                           if (Database.ItemType.IsHossu(hossus.ITEM_ID))
                                           {
                                               var dbItem = Database.Server.ItemsBase[hossus.ITEM_ID];
                                               change += (byte)(dbItem.Level / 3);
                                           }
                                       }

                                       if (AttackHandler.Calculate.Base.Rate(change))
                                       {
                                           if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Player))
                                           {
                                               Role.Player attacked = target as Role.Player;

                                               attacked.AddSpellFlag(MsgUpdate.Flags.Stigma, (int)80, true);
                                               attacked.AddSpellFlag(MsgUpdate.Flags.MagicShield, (int)120, true);
                                               attacked.AddSpellFlag(MsgUpdate.Flags.StarOfAccuracy, (int)60, true);


                                               Attack.SpellID = (ushort)Role.Flags.SpellID.BlessingTouch;
                                               var Spell = Database.Server.Magic[Attack.SpellID];

                                               Updates.UpdateSpell.CheckUpdate(stream, user, Attack, (uint)(change * 100), Spell);

                                           }
                                       }
                                   }
                               }
                           }

                           break;
                       }

               }
           }
       }
    }
}

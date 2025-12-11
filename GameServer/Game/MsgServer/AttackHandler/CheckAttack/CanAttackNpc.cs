using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeathWish.Game.MsgServer.AttackHandler.CheckAttack
{
   public class CanAttackNpc
    {
       public static bool Verified(Client.GameClient client, Role.SobNpc attacked
    , Database.MagicType.Magic DBSpell)
       {
           if (attacked.Map == MsgTournaments.MsgFootball.MapID)
               return true;
           if (attacked.HitPoints == 0)
               return false;

           if (attacked.IsStatue)
           {
               if (attacked.HitPoints == 0)
                   return false;
               if (client.Player.PkMode == Role.Flags.PKMode.PK)
                   return true;
               else
                   return false;
           }
           if (attacked.UID == 6471 || attacked.UID == 6472 || attacked.UID == 6473 || attacked.UID == 6474 || attacked.UID == 6476 || attacked.UID == 6477)//Stake
           {
               if (attacked.HitPoints == 0)
                   return false;
               else
                   return true;
           }
           if (attacked.UID == 890)
           {
               if (client.Player.MyClan == null)
                   return false;
               var tournament = Game.MsgTournaments.MsgSchedules.ClanWar.CurentWar;
               if (tournament == null)
                   return false;

               if (!tournament.InWar(client))
                   return false;
               if (tournament.Winner == null)
                   return false;
               if (tournament.Winner.ClainID == client.Player.ClanUID)
                   return false;

           }
           //if (attacked.UID == 890)
           //{
           //    if (client.Player.MyClan == null)
           //    {
           //        client.CreateBoxDialog("Sorry you not have Clan.");
           //        return false;
           //    }
           //    if (Game.MsgTournaments.MsgSchedules.ClassicClanWar.Furnitures[Role.SobNpc.StaticMesh.Pole].HitPoints == 0)
           //        return false;
           //    if (client.Player.ClanUID == Game.MsgTournaments.MsgSchedules.ClassicClanWar.Winner.GuildID)
           //        return false;
           //    if (Game.MsgTournaments.MsgSchedules.ClassicClanWar.Proces == MsgTournaments.ProcesType.Dead || Game.MsgTournaments.MsgSchedules.ClassicClanWar.Proces == MsgTournaments.ProcesType.Idle)
           //        return false;
           //}
           if (attacked.UID == Game.MsgTournaments.MsgWarOfPlayers.Furnitures[Role.SobNpc.StaticMesh.Pole].UID)
           {
               if (client.Player.MyClan == null)
               {
                   client.CreateBoxDialog("Sorry you not have Clan.");
                   return false;
               }
               if (Game.MsgTournaments.MsgWarOfPlayers.Furnitures[Role.SobNpc.StaticMesh.Pole].HitPoints == 0)
                   return false;
               if (client.Player.ClanName == Game.MsgTournaments.MsgWarOfPlayers.Furnitures[Role.SobNpc.StaticMesh.Pole].Name)
                   return false;
               if (Game.MsgTournaments.MsgWarOfPlayers.Proces != MsgTournaments.ProcesType.Alive)
                   return false;
           }
           if (attacked.UID == Game.MsgTournaments.MsgSchedules.EliteGuildWar.Furnitures[Role.SobNpc.StaticMesh.Pole].UID)
           {
               if (client.Player.MyGuild == null)
               {
                   client.CreateBoxDialog("Sorry you not have Guild.");
                   return false;
               }
               if (Game.MsgTournaments.MsgSchedules.EliteGuildWar.Furnitures[Role.SobNpc.StaticMesh.Pole].HitPoints == 0)
                   return false;
               if (client.Player.MyGuild.GuildName == Game.MsgTournaments.MsgSchedules.EliteGuildWar.Furnitures[Role.SobNpc.StaticMesh.Pole].Name)
                   return false;
               if (Game.MsgTournaments.MsgSchedules.EliteGuildWar.Proces != MsgTournaments.ProcesType.Alive)
                   return false;
           }
           if (attacked.UID == Game.MsgTournaments.MsgSchedules.EliteGuildWar1st.Furnitures[Role.SobNpc.StaticMesh.Pole].UID)
           {
               if (client.Player.MyGuild == null)
               {
                   client.CreateBoxDialog("Sorry you not have Guild.");
                   return false;
               }
               if (Game.MsgTournaments.MsgSchedules.EliteGuildWar1st.Furnitures[Role.SobNpc.StaticMesh.Pole].HitPoints == 0)
                   return false;
               if (client.Player.MyGuild.GuildName == Game.MsgTournaments.MsgSchedules.EliteGuildWar1st.Furnitures[Role.SobNpc.StaticMesh.Pole].Name)
                   return false;
               if (Game.MsgTournaments.MsgSchedules.EliteGuildWar1st.Proces != MsgTournaments.ProcesType.Alive)
                   return false;
           }
           if (attacked.UID == Game.MsgTournaments.MsgSchedules.EliteGuildWar2nd.Furnitures[Role.SobNpc.StaticMesh.Pole].UID)
           {
               if (client.Player.MyGuild == null)
               {
                   client.CreateBoxDialog("Sorry you not have Guild.");
                   return false;
               }
               if (Game.MsgTournaments.MsgSchedules.EliteGuildWar2nd.Furnitures[Role.SobNpc.StaticMesh.Pole].HitPoints == 0)
                   return false;
               if (client.Player.MyGuild.GuildName == Game.MsgTournaments.MsgSchedules.EliteGuildWar2nd.Furnitures[Role.SobNpc.StaticMesh.Pole].Name)
                   return false;
               if (Game.MsgTournaments.MsgSchedules.EliteGuildWar2nd.Proces != MsgTournaments.ProcesType.Alive)
                   return false;
           }
           if (attacked.UID == Game.MsgTournaments.MsgSchedules.EliteGuildWar3rd.Furnitures[Role.SobNpc.StaticMesh.Pole].UID)
           {
               if (client.Player.MyGuild == null)
               {
                   client.CreateBoxDialog("Sorry you not have Guild.");
                   return false;
               }
               if (Game.MsgTournaments.MsgSchedules.EliteGuildWar3rd.Furnitures[Role.SobNpc.StaticMesh.Pole].HitPoints == 0)
                   return false;
               if (client.Player.MyGuild.GuildName == Game.MsgTournaments.MsgSchedules.EliteGuildWar3rd.Furnitures[Role.SobNpc.StaticMesh.Pole].Name)
                   return false;
               if (Game.MsgTournaments.MsgSchedules.EliteGuildWar3rd.Proces != MsgTournaments.ProcesType.Alive)
                   return false;
           }
            if (attacked.UID == 65255)
            {
                if (client.Player.MyGuild == null)
                {
                    client.CreateBoxDialog("Sorry you not have Guild.");
                    return false;
                }
                if (Game.MsgTournaments.MsgVeteransWar.Furnitures[Role.SobNpc.StaticMesh.Pole].HitPoints == 0)
                    return false;
                if (client.Player.MyGuild.GuildName == Game.MsgTournaments.MsgVeteransWar.Furnitures[Role.SobNpc.StaticMesh.Pole].Name)
                    return false;
                if (Game.MsgTournaments.MsgVeteransWar.Proces != MsgTournaments.ProcesType.Alive)
                    return false;
            }
            if (attacked.UID == 123123)
           {
               if (client.Player.GuildRank != Role.Flags.GuildMemberRank.DeputyLeader)
               {
                   client.CreateBoxDialog("Sorry you not  DeputyLeader.");
                   return false;

               }
           }
           if (attacked.UID == 123456)
           {
               if (client.Player.GuildRank != Role.Flags.GuildMemberRank.GuildLeader)
               {
                   client.CreateBoxDialog("Sorry you not  GuildLeader.");
                   return false;
               }
           }
           if (attacked.UID == 123789)
           {
               if (client.Player.GuildRank == Role.Flags.GuildMemberRank.GuildLeader || client.Player.GuildRank == Role.Flags.GuildMemberRank.DeputyLeader)
               {
                   client.CreateBoxDialog("Sorry you are G.L or D.L Can`t Attack Pole.");
                   return false;
               }
           }
           if (attacked.UID == 22340)
           {
               if (client.Player.NobilityRank != DeathWish.Role.Instance.Nobility.NobilityRank.King)
               {
                   client.CreateBoxDialog("Sorry you not KING.");
                   return false;
               }
           }
           if (attacked.UID == 22341)
           {
               if (client.Player.NobilityRank != DeathWish.Role.Instance.Nobility.NobilityRank.Prince)
               {
                   client.CreateBoxDialog("Sorry you not Prince.");
                   return false;
               }
           }
           if (attacked.UID == 22342)
           {
               if (client.Player.NobilityRank != DeathWish.Role.Instance.Nobility.NobilityRank.Duke)
               {
                   client.CreateBoxDialog("Sorry you not Duke.");
                   return false;
               }
           }
           if (attacked.UID == 22343)
           {
               if (client.Player.NobilityRank != DeathWish.Role.Instance.Nobility.NobilityRank.Earl)
               {
                   client.CreateBoxDialog("Sorry you not Earl.");
                   return false;
               }
           }
           if (attacked.UID == 22808)
           {
               if (!Database.AtributesStatus.IsWarrior(client.Player.Class))
               {
                   client.CreateBoxDialog("Sorry you not Warrior.");
                   return false;
               }
           }
           if (attacked.UID == 22803)
           {
               if (!Database.AtributesStatus.IsFire(client.Player.Class))
               {
                   client.CreateBoxDialog("Sorry you not Fire.");
                   return false;
               }
           }
           if (attacked.UID == 22801)
           {
               if (!Database.AtributesStatus.IsArcher(client.Player.Class))
               {
                   client.CreateBoxDialog("Sorry you not Archer.");
                   return false;
               }
           }
           if (attacked.UID == 22810)
           {
               if (!Database.AtributesStatus.IsWindWalker(client.Player.Class))
               {
                   client.CreateBoxDialog("Sorry you not WindWalker.");
                   return false;
               }
           }
           if (attacked.UID == 22809)
           {
               if (!Database.AtributesStatus.IsWater(client.Player.Class))
               {
                   client.CreateBoxDialog("Sorry you not Water.");
                   return false;
               }
           }
           if (attacked.UID == 22807)
           {
               if (!Database.AtributesStatus.IsTrojan(client.Player.Class))
               {
                   client.CreateBoxDialog("Sorry you not Trojan.");
                   return false;
               }
           }
           if (attacked.UID == 22806)
           {
               if (!Database.AtributesStatus.IsPirate(client.Player.Class))
               {
                   client.CreateBoxDialog("Sorry you not Pirate.");
                   return false;
               }
           }
           if (attacked.UID == 22805)
           {
               if (!Database.AtributesStatus.IsNinja(client.Player.Class))
               {
                   client.CreateBoxDialog("Sorry you not Ninja.");
                   return false;
               }
           }
           if (attacked.UID == 22804)
           {
               if (!Database.AtributesStatus.IsMonk(client.Player.Class))
               {
                   client.CreateBoxDialog("Sorry you not Monk.");
                   return false;
               }
           }
           if (attacked.UID == 22802)
           {
               if (!Database.AtributesStatus.IsLee(client.Player.Class))
               {
                   client.CreateBoxDialog("Sorry you not DragonWarrior.");
                   return false;
               }
           }
           if (attacked.UID == 22348)
           {
               if (client.Player.MyUnion == null)
               {
                   client.CreateBoxDialog("Sorry you not have Union.");
                   return false;
               }
               if (Game.MsgTournaments.MsgSchedules.UnionWar.Furnitures[Role.SobNpc.StaticMesh.Pole].HitPoints == 0)
                   return false;
               if (client.Player.MyGuild.UnionID == Game.MsgTournaments.MsgSchedules.UnionWar.Winner.GuildID)
                   return false;
               if (Game.MsgTournaments.MsgSchedules.UnionWar.Proces == MsgTournaments.ProcesType.Dead || Game.MsgTournaments.MsgSchedules.UnionWar.Proces == MsgTournaments.ProcesType.Idle)
                   return false;
           }
           //if (attacked.UID == Game.MsgTournaments.MsgSchedules.PoleDomination.Furnitures[Role.SobNpc.StaticMesh.Pole].UID)
           //{
           //    if (client.Player.MyGuild == null)
           //    {
           //        client.CreateBoxDialog("Sorry you not have Guild.");
           //        return false;
           //    }
           //    if (Game.MsgTournaments.MsgSchedules.PoleDomination.Furnitures[Role.SobNpc.StaticMesh.Pole].HitPoints == 0)
           //        return false;
           //    if (client.Player.GuildID == Game.MsgTournaments.MsgSchedules.PoleDomination.Winner.GuildID)
           //        return false;
           //    if (Game.MsgTournaments.MsgSchedules.PoleDomination.Proces == MsgTournaments.ProcesType.Dead || Game.MsgTournaments.MsgSchedules.PoleDomination.Proces == MsgTournaments.ProcesType.Idle)
           //        return false;
           //}
           if (attacked.UID == Game.MsgTournaments.MsgSchedules.FightersPole1.Furnitures[Role.SobNpc.StaticMesh.Pole].UID)
           {
               if (client.Player.MyGuild == null)
               {
                   client.CreateBoxDialog("Sorry you not have Guild.");
                   return false;
               }
               if (Game.MsgTournaments.MsgSchedules.FightersPole1.Furnitures[Role.SobNpc.StaticMesh.Pole].HitPoints == 0)
                   return false;
               if (client.Player.MyGuild.GuildName == Game.MsgTournaments.MsgSchedules.FightersPole1.Furnitures[Role.SobNpc.StaticMesh.Pole].Name)
                   return false;
               if (Game.MsgTournaments.MsgSchedules.FightersPole1.Proces != MsgTournaments.ProcesType.Alive)
                   return false;
           }
           if (attacked.UID == Game.MsgTournaments.MsgSchedules.FightersPole2.Furnitures[Role.SobNpc.StaticMesh.Pole].UID)
           {
               if (client.Player.MyGuild == null)
               {
                   client.CreateBoxDialog("Sorry you not have Guild.");
                   return false;
               }
               if (Game.MsgTournaments.MsgSchedules.FightersPole2.Furnitures[Role.SobNpc.StaticMesh.Pole].HitPoints == 0)
                   return false;
               if (client.Player.MyGuild.GuildName == Game.MsgTournaments.MsgSchedules.FightersPole2.Furnitures[Role.SobNpc.StaticMesh.Pole].Name)
                   return false;
               if (Game.MsgTournaments.MsgSchedules.FightersPole2.Proces != MsgTournaments.ProcesType.Alive)
                   return false;
           }
           if (attacked.UID == Game.MsgTournaments.MsgSchedules.FightersPole3.Furnitures[Role.SobNpc.StaticMesh.Pole].UID)
           {
               if (client.Player.MyGuild == null)
               {
                   client.CreateBoxDialog("Sorry you not have Guild.");
                   return false;
               }
               if (Game.MsgTournaments.MsgSchedules.FightersPole3.Furnitures[Role.SobNpc.StaticMesh.Pole].HitPoints == 0)
                   return false;
               if (client.Player.MyGuild.GuildName == Game.MsgTournaments.MsgSchedules.FightersPole3.Furnitures[Role.SobNpc.StaticMesh.Pole].Name)
                   return false;
               if (Game.MsgTournaments.MsgSchedules.FightersPole3.Proces != MsgTournaments.ProcesType.Alive)
                   return false;
           }
           if (attacked.UID == Game.MsgTournaments.MsgSchedules.FightersPole4.Furnitures[Role.SobNpc.StaticMesh.Pole].UID)
           {
               if (client.Player.MyGuild == null)
               {
                   client.CreateBoxDialog("Sorry you not have Guild.");
                   return false;
               }
               if (Game.MsgTournaments.MsgSchedules.FightersPole4.Furnitures[Role.SobNpc.StaticMesh.Pole].HitPoints == 0)
                   return false;
               if (client.Player.MyGuild.GuildName == Game.MsgTournaments.MsgSchedules.FightersPole4.Furnitures[Role.SobNpc.StaticMesh.Pole].Name)
                   return false;
               if (Game.MsgTournaments.MsgSchedules.FightersPole4.Proces != MsgTournaments.ProcesType.Alive)
                   return false;
           }
           if (attacked.UID == MsgTournaments.MsgArcherClass.Furnitures[Role.SobNpc.StaticMesh.Pole].UID)
           {
               if (MsgTournaments.MsgArcherClass.Furnitures[Role.SobNpc.StaticMesh.Pole].HitPoints == 0)
                   return false;
               if (client.Player.Name == MsgTournaments.MsgArcherClass.Furnitures[Role.SobNpc.StaticMesh.Pole].Name)
                   return false;
               if (MsgTournaments.MsgArcherClass.Proces == MsgTournaments.ProcesType.Dead)//MsgArcherClass
                   return false;
           }
           if (attacked.UID == MsgTournaments.MsgDragonClass.Furnitures[Role.SobNpc.StaticMesh.Pole].UID)
           {
               if (MsgTournaments.MsgDragonClass.Furnitures[Role.SobNpc.StaticMesh.Pole].HitPoints == 0)
                   return false;
               if (client.Player.Name == MsgTournaments.MsgDragonClass.Furnitures[Role.SobNpc.StaticMesh.Pole].Name)
                   return false;
               if (MsgTournaments.MsgDragonClass.Proces == MsgTournaments.ProcesType.Dead)//MsgDragonClass
                   return false;
           }
           if (attacked.UID == MsgTournaments.MsgFireClass.Furnitures[Role.SobNpc.StaticMesh.Pole].UID)
           {
               if (MsgTournaments.MsgFireClass.Furnitures[Role.SobNpc.StaticMesh.Pole].HitPoints == 0)
                   return false;
               if (client.Player.Name == MsgTournaments.MsgFireClass.Furnitures[Role.SobNpc.StaticMesh.Pole].Name)
                   return false;
               if (MsgTournaments.MsgFireClass.Proces == MsgTournaments.ProcesType.Dead)//MsgFireClass
                   return false;
           }
           if (attacked.UID == MsgTournaments.MsgMonkClass.Furnitures[Role.SobNpc.StaticMesh.Pole].UID)
           {
               if (MsgTournaments.MsgMonkClass.Furnitures[Role.SobNpc.StaticMesh.Pole].HitPoints == 0)
                   return false;
               if (client.Player.Name == MsgTournaments.MsgMonkClass.Furnitures[Role.SobNpc.StaticMesh.Pole].Name)
                   return false;
               if (MsgTournaments.MsgMonkClass.Proces == MsgTournaments.ProcesType.Dead)//MsgMonkClass
                   return false;
           }
           if (attacked.UID == MsgTournaments.MsgNinjaClass.Furnitures[Role.SobNpc.StaticMesh.Pole].UID)
           {
               if (MsgTournaments.MsgNinjaClass.Furnitures[Role.SobNpc.StaticMesh.Pole].HitPoints == 0)
                   return false;
               if (client.Player.Name == MsgTournaments.MsgNinjaClass.Furnitures[Role.SobNpc.StaticMesh.Pole].Name)
                   return false;
               if (MsgTournaments.MsgNinjaClass.Proces == MsgTournaments.ProcesType.Dead)//MsgNinjaClass
                   return false;
           }
           if (attacked.UID == MsgTournaments.MsgPirateClass.Furnitures[Role.SobNpc.StaticMesh.Pole].UID)
           {
               if (MsgTournaments.MsgPirateClass.Furnitures[Role.SobNpc.StaticMesh.Pole].HitPoints == 0)
                   return false;
               if (client.Player.Name == MsgTournaments.MsgPirateClass.Furnitures[Role.SobNpc.StaticMesh.Pole].Name)
                   return false;
               if (MsgTournaments.MsgPirateClass.Proces == MsgTournaments.ProcesType.Dead)//MsgPirateClass
                   return false;
           }
           if (attacked.UID == MsgTournaments.MsgTrojanClass.Furnitures[Role.SobNpc.StaticMesh.Pole].UID)
           {
               if (MsgTournaments.MsgTrojanClass.Furnitures[Role.SobNpc.StaticMesh.Pole].HitPoints == 0)
                   return false;
               if (client.Player.Name == MsgTournaments.MsgTrojanClass.Furnitures[Role.SobNpc.StaticMesh.Pole].Name)
                   return false;
               if (MsgTournaments.MsgTrojanClass.Proces == MsgTournaments.ProcesType.Dead)//MsgTrojanClass
                   return false;
           }
           if (attacked.UID == MsgTournaments.MsgWarriorClass.Furnitures[Role.SobNpc.StaticMesh.Pole].UID)
           {
               if (MsgTournaments.MsgWarriorClass.Furnitures[Role.SobNpc.StaticMesh.Pole].HitPoints == 0)
                   return false;
               if (client.Player.Name == MsgTournaments.MsgWarriorClass.Furnitures[Role.SobNpc.StaticMesh.Pole].Name)
                   return false;
               if (MsgTournaments.MsgWarriorClass.Proces == MsgTournaments.ProcesType.Dead)//MsgWarriorClass
                   return false;
           }
           if (attacked.UID == MsgTournaments.MsgWaterClass.Furnitures[Role.SobNpc.StaticMesh.Pole].UID)
           {
               if (MsgTournaments.MsgWaterClass.Furnitures[Role.SobNpc.StaticMesh.Pole].HitPoints == 0)
                   return false;
               if (client.Player.Name == MsgTournaments.MsgWaterClass.Furnitures[Role.SobNpc.StaticMesh.Pole].Name)
                   return false;
               if (MsgTournaments.MsgWaterClass.Proces == MsgTournaments.ProcesType.Dead)//MsgWaterClass
                   return false;
           }
           if (attacked.UID == MsgTournaments.MsgWindClass.Furnitures[Role.SobNpc.StaticMesh.Pole].UID)
           {
               if (MsgTournaments.MsgWindClass.Furnitures[Role.SobNpc.StaticMesh.Pole].HitPoints == 0)
                   return false;
               if (client.Player.Name == MsgTournaments.MsgWindClass.Furnitures[Role.SobNpc.StaticMesh.Pole].Name)
                   return false;
               if (MsgTournaments.MsgWindClass.Proces == MsgTournaments.ProcesType.Dead)//MsgWindClass
                   return false;
           }
           if (attacked.UID == MsgTournaments.MsgEmperorWar.Furnitures[Role.SobNpc.StaticMesh.Pole].UID)
           {
               if (MsgTournaments.MsgEmperorWar.Furnitures[Role.SobNpc.StaticMesh.Pole].HitPoints == 0)
                   return false;
               if (client.Player.Name == MsgTournaments.MsgEmperorWar.Furnitures[Role.SobNpc.StaticMesh.Pole].Name)
                   return false;
               if (MsgTournaments.MsgEmperorWar.Proces == MsgTournaments.ProcesType.Dead)
                   return false;
           }
           if (attacked.UID == MsgTournaments.MsgGuildPole.Furnitures[Role.SobNpc.StaticMesh.Pole].UID)
           {

               if (MsgTournaments.MsgGuildPole.Furnitures[Role.SobNpc.StaticMesh.Pole].HitPoints == 0)
                   return false;
               if (client.Player.Name == MsgTournaments.MsgGuildPole.Furnitures[Role.SobNpc.StaticMesh.Pole].Name)
                   return false;
               if (MsgTournaments.MsgGuildPole.Proces == MsgTournaments.ProcesType.Dead)//MsgNobilityPole
                   return false;
           }
           if (attacked.UID == MsgTournaments.MsgNobilityPole.Furnitures[Role.SobNpc.StaticMesh.Pole].UID)
           {
               if (MsgTournaments.MsgNobilityPole.Furnitures[Role.SobNpc.StaticMesh.Pole].HitPoints == 0)
                   return false;
               if (client.Player.Name == MsgTournaments.MsgNobilityPole.Furnitures[Role.SobNpc.StaticMesh.Pole].Name)
                   return false;
               if (MsgTournaments.MsgNobilityPole.Proces == MsgTournaments.ProcesType.Dead)//MsgNobilityPole
                   return false;
           }
           if (attacked.UID == MsgTournaments.MsgNobilityPole1.Furnitures[Role.SobNpc.StaticMesh.Pole].UID)
           {
               if (MsgTournaments.MsgNobilityPole1.Furnitures[Role.SobNpc.StaticMesh.Pole].HitPoints == 0)
                   return false;
               if (client.Player.Name == MsgTournaments.MsgNobilityPole1.Furnitures[Role.SobNpc.StaticMesh.Pole].Name)
                   return false;
               if (MsgTournaments.MsgNobilityPole1.Proces == MsgTournaments.ProcesType.Dead)//MsgNobilityPole
                   return false;
           }
           if (attacked.UID == MsgTournaments.MsgNobilityPole2.Furnitures[Role.SobNpc.StaticMesh.Pole].UID)
           {
               if (MsgTournaments.MsgNobilityPole2.Furnitures[Role.SobNpc.StaticMesh.Pole].HitPoints == 0)
                   return false;
               if (client.Player.Name == MsgTournaments.MsgNobilityPole2.Furnitures[Role.SobNpc.StaticMesh.Pole].Name)
                   return false;
               if (MsgTournaments.MsgNobilityPole2.Proces == MsgTournaments.ProcesType.Dead)//MsgNobilityPole
                   return false;
           }
           if (attacked.UID == MsgTournaments.MsgNobilityPole3.Furnitures[Role.SobNpc.StaticMesh.Pole].UID)
           {
               if (MsgTournaments.MsgNobilityPole3.Furnitures[Role.SobNpc.StaticMesh.Pole].HitPoints == 0)
                   return false;
               if (client.Player.Name == MsgTournaments.MsgNobilityPole3.Furnitures[Role.SobNpc.StaticMesh.Pole].Name)
                   return false;
               if (MsgTournaments.MsgNobilityPole3.Proces == MsgTournaments.ProcesType.Dead)//MsgNobilityPole
                   return false;
           }
           if (attacked.UID == MsgTournaments.MsgGuildPole1.Furnitures[Role.SobNpc.StaticMesh.Pole].UID)
           {
               if (MsgTournaments.MsgGuildPole1.Furnitures[Role.SobNpc.StaticMesh.Pole].HitPoints == 0)
                   return false;
               if (client.Player.Name == MsgTournaments.MsgGuildPole1.Furnitures[Role.SobNpc.StaticMesh.Pole].Name)
                   return false;
               if (MsgTournaments.MsgGuildPole1.Proces == MsgTournaments.ProcesType.Dead)//MsgGuildPole
                   return false;
           }
           if (attacked.UID == MsgTournaments.MsgGuildPole2.Furnitures[Role.SobNpc.StaticMesh.Pole].UID)
           {
               if (MsgTournaments.MsgGuildPole2.Furnitures[Role.SobNpc.StaticMesh.Pole].HitPoints == 0)
                   return false;
               if (client.Player.Name == MsgTournaments.MsgGuildPole2.Furnitures[Role.SobNpc.StaticMesh.Pole].Name)
                   return false;
               if (MsgTournaments.MsgGuildPole2.Proces == MsgTournaments.ProcesType.Dead)//MsgGuildPole
                   return false;
           }
           if (attacked.UID == MsgTournaments.MsgWarOfPlayers.Furnitures[Role.SobNpc.StaticMesh.Pole].UID)
           {
               if (MsgTournaments.MsgWarOfPlayers.Furnitures[Role.SobNpc.StaticMesh.Pole].HitPoints == 0)
                   return false;
               if (client.Player.Name == MsgTournaments.MsgWarOfPlayers.Furnitures[Role.SobNpc.StaticMesh.Pole].Name)
                   return false;
               if (MsgTournaments.MsgWarOfPlayers.Proces == MsgTournaments.ProcesType.Dead)//War_of_the_players
                   return false;
           }
            if (attacked.UID == Game.MsgTournaments.MsgSchedules.GuildWar.Furnitures[Role.SobNpc.StaticMesh.Pole].UID)
           {
               if (client.Player.MyGuild == null)
               {
                   client.CreateBoxDialog("Sorry you not have Guild.");
                   return false;
               }
               if (Game.MsgTournaments.MsgSchedules.GuildWar.Furnitures[Role.SobNpc.StaticMesh.Pole].HitPoints == 0)
                   return false;
               if (client.Player.GuildID == Game.MsgTournaments.MsgSchedules.GuildWar.Winner.GuildID)
                   return false;
               if (Game.MsgTournaments.MsgSchedules.GuildWar.Proces == MsgTournaments.ProcesType.Dead || Game.MsgTournaments.MsgSchedules.GuildWar.Proces == MsgTournaments.ProcesType.Idle)
                   return false;
           }
            if (attacked.UID == Game.MsgTournaments.MsgSchedules.SuperGuildWar.Furnitures[MsgTournaments.MsgSuperGuildWar.FurnituresType.Pole].UID)
            {
                if (client.Player.MyGuild == null)
                {
                    client.CreateBoxDialog("Sorry you not have Guild.");
                    return false;
                }
                if (Game.MsgTournaments.MsgSchedules.SuperGuildWar.Furnitures[MsgTournaments.MsgSuperGuildWar.FurnituresType.Pole].HitPoints == 0)
                    return false;
                if (client.Player.GuildID == Game.MsgTournaments.MsgSchedules.SuperGuildWar.Winner.GuildID)
                    return false;
                if (Game.MsgTournaments.MsgSchedules.SuperGuildWar.Proces == MsgTournaments.ProcesType.Dead || Game.MsgTournaments.MsgSchedules.SuperGuildWar.Proces == MsgTournaments.ProcesType.Idle)
                    return false;
            }

            MsgTournaments.MsgCaptureTheFlag.Basse Bas;
           if (MsgTournaments.MsgSchedules.CaptureTheFlag.Bases.TryGetValue(attacked.UID, out Bas))
           {
               if (MsgTournaments.MsgSchedules.CaptureTheFlag.Proces != MsgTournaments.ProcesType.Alive)
                   return false;
               if (client.Player.MyGuild == null)
                   return false;
               if (Bas.Npc.HitPoints == 0)
                   return false;
               if (Bas.CapturerID == client.Player.GuildID)
                   return false;

           }
           return true;
       }
    }
}

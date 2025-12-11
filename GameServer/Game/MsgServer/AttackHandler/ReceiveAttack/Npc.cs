using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeathWish.Game.MsgServer.AttackHandler.ReceiveAttack
{
    public class Npc
    {
        public static uint Execute(ServerSockets.Packet stream, MsgSpellAnimation.SpellObj obj, Client.GameClient client, Role.SobNpc attacked)
        {
            if(MsgTournaments.MsgSchedules.CurrentTournament.Type == MsgTournaments.TournamentType.FootBall)
            {
                if(MsgTournaments.MsgSchedules.CurrentTournament.Process == MsgTournaments.ProcesType.Alive)
                {
                    var tournament = (MsgTournaments.MsgFootball)MsgTournaments.MsgSchedules.CurrentTournament;
                    tournament.RemoveNpc();
                    client.Player.AddFlag(MsgServer.MsgUpdate.Flags.lianhuaran04, Role.StatusFlagsBigVector32.PermanentFlag, true);
                    return 0;
                }
            }
          
            if (obj.Damage >= attacked.HitPoints)
            {
                uint exp = (uint)attacked.HitPoints;
                attacked.Die(stream, client);
                if (attacked.Map == 1039)
                    return exp / 10;
            }
            else
            {
                attacked.HitPoints -= (int)obj.Damage;

                if (attacked.UID == Game.MsgTournaments.MsgSchedules.GuildWar.Furnitures[Role.SobNpc.StaticMesh.Pole].UID)
                    Game.MsgTournaments.MsgSchedules.GuildWar.UpdateScore(client.Player, obj.Damage);
                else if (attacked.UID == Game.MsgTournaments.MsgSchedules.SuperGuildWar.Furnitures[MsgTournaments.MsgSuperGuildWar.FurnituresType.Pole].UID)
                    Game.MsgTournaments.MsgSchedules.SuperGuildWar.UpdateScore(client.Player, obj.Damage);
                //else if (Game.MsgTournaments.MsgTwinCityWar.Proces == MsgTournaments.ProcesType.Alive && attacked.UID == Game.MsgTournaments.MsgTwinCityWar.Furnitures[Role.SobNpc.StaticMesh.Pole].UID)
                //    Game.MsgTournaments.MsgSchedules.TwinCityWar.UpdateScore(client.Player, obj.Damage);
                //if (attacked.UID == Game.MsgTournaments.MsgSchedules.UnionWar.Furnitures[Role.SobNpc.StaticMesh.Pole].UID)
                //    Game.MsgTournaments.MsgSchedules.UnionWar.UpdateScore(client.Player, obj.Damage);
                else if (Game.MsgTournaments.MsgSchedules.EliteGuildWar.Proces == MsgTournaments.ProcesType.Alive && attacked.UID == Game.MsgTournaments.MsgSchedules.EliteGuildWar.Furnitures[Role.SobNpc.StaticMesh.Pole].UID)
                    Game.MsgTournaments.MsgSchedules.EliteGuildWar.UpdateScore(stream, obj.Damage, client.Player.MyGuild);
                else if (Game.MsgTournaments.MsgSchedules.EliteGuildWar1st.Proces == MsgTournaments.ProcesType.Alive && attacked.UID == Game.MsgTournaments.MsgSchedules.EliteGuildWar1st.Furnitures[Role.SobNpc.StaticMesh.Pole].UID)
                    Game.MsgTournaments.MsgSchedules.EliteGuildWar1st.UpdateScore(stream, obj.Damage, client.Player.MyGuild);
                else if (Game.MsgTournaments.MsgSchedules.EliteGuildWar2nd.Proces == MsgTournaments.ProcesType.Alive && attacked.UID == Game.MsgTournaments.MsgSchedules.EliteGuildWar2nd.Furnitures[Role.SobNpc.StaticMesh.Pole].UID)
                    Game.MsgTournaments.MsgSchedules.EliteGuildWar2nd.UpdateScore(stream, obj.Damage, client.Player.MyGuild);
                else if (Game.MsgTournaments.MsgSchedules.EliteGuildWar3rd.Proces == MsgTournaments.ProcesType.Alive && attacked.UID == Game.MsgTournaments.MsgSchedules.EliteGuildWar3rd.Furnitures[Role.SobNpc.StaticMesh.Pole].UID)
                    Game.MsgTournaments.MsgSchedules.EliteGuildWar3rd.UpdateScore(stream, obj.Damage, client.Player.MyGuild);
                else if (Game.MsgTournaments.MsgEmperorWar.Proces == MsgTournaments.ProcesType.Alive && attacked.UID == Game.MsgTournaments.MsgEmperorWar.Furnitures[Role.SobNpc.StaticMesh.Pole].UID)
                    Game.MsgTournaments.MsgEmperorWar.UpdateScore(stream, obj.Damage, client.Player);
                else if (Game.MsgTournaments.MsgVeteransWar.Proces == MsgTournaments.ProcesType.Alive && attacked.UID == Game.MsgTournaments.MsgVeteransWar.Furnitures[Role.SobNpc.StaticMesh.Pole].UID)
                    Game.MsgTournaments.MsgVeteransWar.UpdateScore(stream, obj.Damage, client.Player.MyGuild);
                else if (Game.MsgTournaments.MsgWarriorClass.Proces == MsgTournaments.ProcesType.Alive && attacked.UID == Game.MsgTournaments.MsgWarriorClass.Furnitures[Role.SobNpc.StaticMesh.Pole].UID)
                    Game.MsgTournaments.MsgWarriorClass.UpdateScore(stream, obj.Damage, client.Player);
                else if (Game.MsgTournaments.MsgWaterClass.Proces == MsgTournaments.ProcesType.Alive && attacked.UID == Game.MsgTournaments.MsgWaterClass.Furnitures[Role.SobNpc.StaticMesh.Pole].UID)
                    Game.MsgTournaments.MsgWaterClass.UpdateScore(stream, obj.Damage, client.Player);
                else if (Game.MsgTournaments.MsgWindClass.Proces == MsgTournaments.ProcesType.Alive && attacked.UID == Game.MsgTournaments.MsgWindClass.Furnitures[Role.SobNpc.StaticMesh.Pole].UID)
                    Game.MsgTournaments.MsgWindClass.UpdateScore(stream, obj.Damage, client.Player);
                else if (Game.MsgTournaments.MsgTrojanClass.Proces == MsgTournaments.ProcesType.Alive && attacked.UID == Game.MsgTournaments.MsgTrojanClass.Furnitures[Role.SobNpc.StaticMesh.Pole].UID)
                    Game.MsgTournaments.MsgTrojanClass.UpdateScore(stream, obj.Damage, client.Player);
                else if (Game.MsgTournaments.MsgPirateClass.Proces == MsgTournaments.ProcesType.Alive && attacked.UID == Game.MsgTournaments.MsgPirateClass.Furnitures[Role.SobNpc.StaticMesh.Pole].UID)
                    Game.MsgTournaments.MsgPirateClass.UpdateScore(stream, obj.Damage, client.Player);
                else if (Game.MsgTournaments.MsgMonkClass.Proces == MsgTournaments.ProcesType.Alive && attacked.UID == Game.MsgTournaments.MsgMonkClass.Furnitures[Role.SobNpc.StaticMesh.Pole].UID)
                    Game.MsgTournaments.MsgMonkClass.UpdateScore(stream, obj.Damage, client.Player);
                else if (Game.MsgTournaments.MsgNinjaClass.Proces == MsgTournaments.ProcesType.Alive && attacked.UID == Game.MsgTournaments.MsgNinjaClass.Furnitures[Role.SobNpc.StaticMesh.Pole].UID)
                    Game.MsgTournaments.MsgNinjaClass.UpdateScore(stream, obj.Damage, client.Player);
                else if (Game.MsgTournaments.MsgDragonClass.Proces == MsgTournaments.ProcesType.Alive && attacked.UID == Game.MsgTournaments.MsgDragonClass.Furnitures[Role.SobNpc.StaticMesh.Pole].UID)
                    Game.MsgTournaments.MsgDragonClass.UpdateScore(stream, obj.Damage, client.Player);
                else if (Game.MsgTournaments.MsgArcherClass.Proces == MsgTournaments.ProcesType.Alive && attacked.UID == Game.MsgTournaments.MsgArcherClass.Furnitures[Role.SobNpc.StaticMesh.Pole].UID)
                    Game.MsgTournaments.MsgArcherClass.UpdateScore(stream, obj.Damage, client.Player);
                else if (Game.MsgTournaments.MsgFireClass.Proces == MsgTournaments.ProcesType.Alive && attacked.UID == Game.MsgTournaments.MsgFireClass.Furnitures[Role.SobNpc.StaticMesh.Pole].UID)
                    Game.MsgTournaments.MsgFireClass.UpdateScore(stream, obj.Damage, client.Player);
                else if (Game.MsgTournaments.MsgNobilityPole.Proces == MsgTournaments.ProcesType.Alive && attacked.UID == Game.MsgTournaments.MsgNobilityPole.Furnitures[Role.SobNpc.StaticMesh.Pole].UID)
                    Game.MsgTournaments.MsgNobilityPole.UpdateScore(stream, obj.Damage, client.Player);
                else if (Game.MsgTournaments.MsgNobilityPole1.Proces == MsgTournaments.ProcesType.Alive && attacked.UID == Game.MsgTournaments.MsgNobilityPole1.Furnitures[Role.SobNpc.StaticMesh.Pole].UID)
                    Game.MsgTournaments.MsgNobilityPole1.UpdateScore(stream, obj.Damage, client.Player);
                else if (Game.MsgTournaments.MsgNobilityPole2.Proces == MsgTournaments.ProcesType.Alive && attacked.UID == Game.MsgTournaments.MsgNobilityPole2.Furnitures[Role.SobNpc.StaticMesh.Pole].UID)
                    Game.MsgTournaments.MsgNobilityPole2.UpdateScore(stream, obj.Damage, client.Player);
                else if (Game.MsgTournaments.MsgNobilityPole3.Proces == MsgTournaments.ProcesType.Alive && attacked.UID == Game.MsgTournaments.MsgNobilityPole3.Furnitures[Role.SobNpc.StaticMesh.Pole].UID)
                    Game.MsgTournaments.MsgNobilityPole3.UpdateScore(stream, obj.Damage, client.Player);
                else if (Game.MsgTournaments.MsgGuildPole.Proces == MsgTournaments.ProcesType.Alive && attacked.UID == Game.MsgTournaments.MsgGuildPole.Furnitures[Role.SobNpc.StaticMesh.Pole].UID)
                    Game.MsgTournaments.MsgGuildPole.UpdateScore(stream, obj.Damage, client.Player);
                else if (Game.MsgTournaments.MsgGuildPole1.Proces == MsgTournaments.ProcesType.Alive && attacked.UID == Game.MsgTournaments.MsgGuildPole1.Furnitures[Role.SobNpc.StaticMesh.Pole].UID)
                    Game.MsgTournaments.MsgGuildPole1.UpdateScore(stream, obj.Damage, client.Player);
                else if (Game.MsgTournaments.MsgGuildPole2.Proces == MsgTournaments.ProcesType.Alive && attacked.UID == Game.MsgTournaments.MsgGuildPole2.Furnitures[Role.SobNpc.StaticMesh.Pole].UID)
                    Game.MsgTournaments.MsgGuildPole2.UpdateScore(stream, obj.Damage, client.Player);
                else if (Game.MsgTournaments.MsgWarOfPlayers.Proces == MsgTournaments.ProcesType.Alive && attacked.UID == Game.MsgTournaments.MsgWarOfPlayers.Furnitures[Role.SobNpc.StaticMesh.Pole].UID)
                    Game.MsgTournaments.MsgWarOfPlayers.UpdateScore(stream, client.Player, obj.Damage);
                else if (Game.MsgTournaments.MsgSchedules.ClanWar.Process == MsgTournaments.ProcesType.Alive)
                {
                    if (Game.MsgTournaments.MsgSchedules.ClanWar.CurentWar != null && Game.MsgTournaments.MsgSchedules.ClanWar.CurentWar.InWar(client))
                    {
                        if (Game.MsgTournaments.MsgSchedules.ClanWar.CurentWar.Proces == MsgTournaments.ProcesType.Alive)
                        {
                            Game.MsgTournaments.MsgSchedules.ClanWar.CurentWar.UpdateScore(client.Player, obj.Damage);
                        }
                    }
                }
                if (Game.MsgTournaments.MsgSchedules.CaptureTheFlag.Bases.ContainsKey(attacked.UID))
                {
                    Game.MsgTournaments.MsgSchedules.CaptureTheFlag.UpdateFlagScore(client.Player, attacked, obj.Damage, stream);
                }
               if (attacked.Map == 1039 || attacked.Map == 1038 || attacked.Map == 3868)
                    return obj.Damage / 1000;
               
            }
            return 0;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeathWish.Game.MsgTournaments
{
    public class MsgExtremePk : ITournament
    {
        public const uint MapID = 2572, RewardConquerPoints = 200000;

       public ProcesType Process { get; set; }
       public DateTime StartTimer = new DateTime();
       public DateTime ScoreStamp = new DateTime();
       public DateTime InfoTimer = new DateTime();
       public Role.GameMap Map;
       public uint DinamicID, Secounds = 0;
       public KillerSystem KillSystem;
       public TournamentType Type { get; set; }
       public MsgExtremePk(TournamentType _type)
       {
           Type = _type;
           Process = ProcesType.Dead;
       }
       public void Open()
       {
           if (Process == ProcesType.Dead)
           {
               KillSystem = new KillerSystem();
               Map = Database.Server.ServerMaps[MapID];
               DinamicID = Map.GenerateDynamicID();
#if Arabic
                 MsgSchedules.SendInvitation("ExtremePk", "ConquerPoints", 448, 353, 1002, 0, 60);
#else
               MsgSchedules.SendInvitation("ExtremePk", "200K Cps + 2 DIABLO[Spin] + 10 E-P", 452, 354, 1002, 0, 120);
#endif
             
               StartTimer = DateTime.Now;
               InfoTimer = DateTime.Now;
               Secounds = 120;
               Process = ProcesType.Idle;
           }
       }
       public void Revive(Extensions.Time32 Timer, Client.GameClient user)
       {
           if (user.Player.Alive == false && Process != ProcesType.Dead)
           {
               if (InTournament(user))
               {
                   if (user.Player.DeadStamp.AddSeconds(2) < Timer)
                   {
                       ushort x = 0;
                       ushort y = 0;
                       Map.GetRandCoord(ref x, ref y);
                       user.Teleport(x, y, Map.ID, DinamicID);
                   }
               }
           }
       }
       public bool Join(Client.GameClient user,ServerSockets.Packet stream)
       {
           if (user.Player.Level < 100)
               return false;
           if (Process == ProcesType.Idle)
           {
               user.Player.XtremePkPoints = 1000;
               user.Player.TournamentKills = 0;
               ushort x = 0;
               ushort y = 0;

               Map.GetRandCoord(ref x, ref y);
               user.Teleport(x, y, Map.ID, DinamicID);
               user.Player.SetPkMode(DeathWish.Role.Flags.PKMode.PK);

               return true;
           }
           return false;
       }

       public void CheckUp()
       {
           if (Process == ProcesType.Idle)
           {
               if (DateTime.Now > StartTimer.AddSeconds(120))
               {
#if Arabic
                    MsgSchedules.SendSysMesage("ExtremePk has started! signup are now closed.", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);
                  
#else
                   MsgSchedules.SendSysMesage("ExtremePk has started! signup are now closed.", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);
                  
#endif
                   Process = ProcesType.Alive;
                   StartTimer = DateTime.Now;
               }
               else if (DateTime.Now > InfoTimer.AddSeconds(10))
               {
                   Secounds -= 10;
#if Arabic
                    MsgSchedules.SendSysMesage("[ExtremePk] Fight starts in " + Secounds.ToString() + " Secounds.", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);
                  
#else
                   MsgSchedules.SendSysMesage("[ExtremePk] Fight starts in " + Secounds.ToString() + " Secounds.", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);
                  
#endif
                   InfoTimer = DateTime.Now;
               }
           }
           if (Process == ProcesType.Alive)
           {
               DateTime Now = DateTime.Now;

               if (DateTime.Now > StartTimer.AddMinutes(3))
               {
#if Arabic
                     MsgSchedules.SendSysMesage("ExtremePk has ended. All Players of ExtremePk has teleported to TwinCity.", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);
                  
#else
                   MsgSchedules.SendSysMesage("ExtremePk has ended. All Players of ExtremePk has teleported to TwinCity.", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);
                  
#endif
                  var array = MapPlayers().OrderByDescending(p => p.Player.XtremePkPoints).ToArray();
                   if (array.Length > 0)
                   {
                       var Winner = array.First();
#if Arabic
  MsgSchedules.SendSysMesage("" + Winner.Player.Name + " has Won  ExtremePk. ", MsgServer.MsgMessage.ChatMode.BroadcastMessage, MsgServer.MsgMessage.MsgColor.white);

#else
                       MsgSchedules.SendSysMesage("" + Winner.Player.Name + " has Won  ExtremePk. 200000 Cps & 2 DIABLO Spin & 10 Event Point ", MsgServer.MsgMessage.ChatMode.BroadcastMessage, MsgServer.MsgMessage.MsgColor.white);


#endif
                     
                       Winner.Player.ConquerPoints += RewardConquerPoints;
                       Winner.Player.PIKAPoint += 10;
                       using (var rec = new ServerSockets.RecycledPacket())
                       {
                           var stream = rec.GetStream();
                           Winner.Inventory.Add(stream, 3008733, 2);
                       }

                       int x = 1;
                       foreach (var user in array)
                       {
                           if (x > 1)
                           {
                              // user.Player.ConquerPoints += (uint)(RewardConquerPoints / x);
#if Arabic
                                         user.SendSysMesage("You received " + (RewardConquerPoints / x).ToString() + " ConquerPoints. ", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);
                     
#else
                              // user.SendSysMesage("You received " + (RewardConquerPoints / x).ToString() + " ConquerPoints .", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);
                     
#endif
                           }
                           x++;
                           user.Teleport(428, 379, 1002);
                           user.Player.SetPkMode(DeathWish.Role.Flags.PKMode.Capture);

                       }
                   }
                   Process = ProcesType.Dead;
               }

               if (Now > ScoreStamp)
               {
                     using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        var array = MapPlayers().OrderByDescending(p => p.Player.XtremePkPoints).ToArray();

                        foreach (var user in MapPlayers())
                        {
#if Arabic
                                  Game.MsgServer.MsgMessage msg = new MsgServer.MsgMessage("---Your Score:" + user.Player.XtremePkPoints + "---", MsgServer.MsgMessage.MsgColor.yellow, MsgServer.MsgMessage.ChatMode.FirstRightCorner);
                            user.Send(msg.GetArray(stream));
                            msg = new MsgServer.MsgMessage("My tournament Kills: " + user.Player.TournamentKills.ToString() + "", MsgServer.MsgMessage.MsgColor.yellow, MsgServer.MsgMessage.ChatMode.ContinueRightCorner);
                            user.Send(msg.GetArray(stream));
#else
                            Game.MsgServer.MsgMessage msg = new MsgServer.MsgMessage("---Your Score:" + user.Player.XtremePkPoints + "---", MsgServer.MsgMessage.MsgColor.yellow, MsgServer.MsgMessage.ChatMode.FirstRightCorner);
                            user.Send(msg.GetArray(stream));
                            msg = new MsgServer.MsgMessage("My Kills: " + user.Player.TournamentKills.ToString() + "", MsgServer.MsgMessage.MsgColor.yellow, MsgServer.MsgMessage.ChatMode.ContinueRightCorner);
                            user.Send(msg.GetArray(stream));
#endif
                      
                        }

                        int x = 0;
                        foreach (var obj in array)
                        {
                            if (x == 4)
                                break;
#if Arabic
                             Game.MsgServer.MsgMessage amsg = new MsgServer.MsgMessage("No " + (x + 1).ToString() + ". " + obj.Player.Name + " (" + obj.Player.XtremePkPoints.ToString() + ")", MsgServer.MsgMessage.MsgColor.yellow, MsgServer.MsgMessage.ChatMode.ContinueRightCorner);
                           
#else
                            Game.MsgServer.MsgMessage amsg = new MsgServer.MsgMessage("No " + (x + 1).ToString() + ". " + obj.Player.Name + " (" + obj.Player.XtremePkPoints.ToString() + ")", MsgServer.MsgMessage.MsgColor.yellow, MsgServer.MsgMessage.ChatMode.ContinueRightCorner);
                           
#endif
                            SendMapPacket(amsg.GetArray(stream));

                            x++;
                        }
                      
                    }
              

                   ScoreStamp = Now.AddSeconds(4);
               }

             
           }
       }

       public void SharePoints(Client.GameClient user, Client.GameClient Killer)
       {
           uint xtremepoints = (uint)(user.Player.XtremePkPoints / 4);
           Killer.Player.XtremePkPoints += xtremepoints;
           user.Player.XtremePkPoints -= xtremepoints;

#if Arabic
              Killer.SendSysMesage("You gained " + xtremepoints.ToString() + " points for killing " + user.Player.Name + "", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.white);
           user.SendSysMesage("You lost " + xtremepoints.ToString() + " points when "+Killer.Player.Name.ToString() +" has killed you.", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.white);
     
#else
           Killer.SendSysMesage("You gained " + xtremepoints.ToString() + " points for killing " + user.Player.Name + "", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.white);
           user.SendSysMesage("You lost " + xtremepoints.ToString() + " points when " + Killer.Player.Name.ToString() + " has killed you.", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.white);
     
#endif
          }

       public void SendMapPacket(ServerSockets.Packet stream)
       {
           foreach (var user in MapPlayers())
               user.Send(stream);
       }

       public bool InTournament(Client.GameClient user)
       {
           if (Map == null)
               return false;
           return user.Player.Map == Map.ID && user.Player.DynamicID == DinamicID;
       }

       public Client.GameClient[] MapPlayers()
       {
           return Map.Values.Where(p => InTournament(p)).ToArray();
       }
    }
}

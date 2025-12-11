using DeathWish.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeathWish.Game.MsgTournaments
{
    public class MsgDragonIsland
    {
        public const ushort MapID = 10137;
        public ProcesType Process { get; set; }
        public Fan SafeArea = new Fan(0, 400, 150, 419, 150, 180);
        public Role.GameMap Map
        {
            get { return Database.Server.ServerMaps[MapID]; }
        }
        public MsgDragonIsland(ProcesType _process)
        {
            Process = _process;
        }
        public bool Attackable(uint Map, ushort X, ushort Y)
        {
            if (Map != MapID)
                return true;
            return SafeArea.IsInFan(X, Y) == true ? false : true;
        }
        public void SendMapPacket(ServerSockets.Packet stream)
        {
            foreach (var user in MapPlayers())
                user.Send(stream);
        }
        public Client.GameClient[] MapPlayers()
        {
            return Map.Values.Where(p => InTournament(p)).ToArray();
        }
        public bool InTournament(Client.GameClient user)
        {
            if (Map == null)
                return false;
            return user.Player.Map == Map.ID;
        }
        public bool Join(Client.GameClient user, ServerSockets.Packet stream, bool Chasm = false)
        {
            if (user.Player.Level < 120 || user.Player.Reborn < 1)
                return false;

            if (Chasm)
            {
                user.Teleport((ushort)(455 - Program.GetRandom.Next(0, 5)), (ushort)(479 - Program.GetRandom.Next(0, 5)), 10137);
            }
            else
            {
                user.Teleport((ushort)(95 - Program.GetRandom.Next(0, 5)), (ushort)(412 - Program.GetRandom.Next(0, 5)), MapID);
            }
            return true;

        }
    }
}
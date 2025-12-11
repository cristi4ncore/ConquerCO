using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeathWish.Game.MsgTournaments
{
    public class MsgSquidwardOctopus
    {
        public int GoldRemain = 5000;
        public int SilverRemain = 5000;
        public int CopperRemain = 5000;
        public const ushort
            MapID = 3071;
        public ProcesType Process { get; set; }

        public DateTime StartTimer = new DateTime();
        Role.GameMap _map;
        public Role.GameMap Map
        {
            get
            {
                if (_map == null)
                    _map = Database.Server.ServerMaps[MapID];
                return _map;
            }
        }
        public MsgSquidwardOctopus()
        {
            Process = ProcesType.Dead;
        }
        public bool InTournament(Client.GameClient user)
        {
            return user.Player.Map == MapID;
        }
        public void Start()
        {
            if (Process == ProcesType.Dead)
            {
                MsgSchedules.SendInvitation("SquidwardOctopus", "Mounts,Expericence,Refinary", 426, 305, 1002, 0, 500);
                StartTimer = DateTime.Now;
                GoldRemain = 10000;
                SilverRemain = 10000;
                CopperRemain = 10000;

                Process = ProcesType.Alive;
            }
        }
        public void Finish()
        {
            if (Process == ProcesType.Alive)
            {
                DateTime Now64 = DateTime.Now;
                MsgSchedules.SendSysMesage("The event SquidwardOctopus has finished.", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.yellow);
                Process = ProcesType.Dead;
                var Map2 = Database.Server.ServerMaps[1002];
                foreach (var user in Map.Values)
                {
                    user.Teleport(300, 278, 1002);
                }
            }
        }
        public void CheckUp()
        {
            if (Process == ProcesType.Alive)
            {
                DateTime Now = DateTime.Now;
                if (DateTime.Now > StartTimer.AddMinutes(10))
                    Finish();
            }
        }
    }
}
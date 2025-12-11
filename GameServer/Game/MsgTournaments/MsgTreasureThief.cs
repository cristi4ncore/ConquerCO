
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeathWish.Game.MsgTournaments
{
    public class MsgTreasureThief : ITournament
    {
        public const ushort
            MapID = 3820;
        public ProcesType Process { get; set; }
        public int CurrentBoxes = 0;
        public DateTime StartTimer = new DateTime();
        public DateTime BoxesStamp = new DateTime();
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
        public TournamentType Type { get; set; }
        public MsgTreasureThief(TournamentType _type)
        {
            Type = _type;
            Process = ProcesType.Dead;
        }
        public bool InTournament(Client.GameClient user)
        {
            return user.Player.Map == MapID;
        }
        public void Open()
        {
            if (Process != ProcesType.Alive)
            {
                Create();
                foreach (var user in Database.Server.GamePoll.Values)
                    user.Player.CurrentTreasureBoxes = 0;
                Process = ProcesType.Alive;
                StartTimer = DateTime.Now.AddMinutes(10);
                BoxesStamp = DateTime.Now.AddSeconds(10);
#if Arabic
                   MsgSchedules.SendInvitation("TreasureThief", "ConquerPoints,Money,Vip and others treasures", 310, 251, 1002, 0, 60);
#else
                MsgSchedules.SendInvitation("TreasureThief", "CPs,Money,Vip and others treasures", 443, 337, 1002, 0, 500);
#endif

            }
        }
        public bool Join(Client.GameClient user, ServerSockets.Packet stream)
        {
            if (Process == ProcesType.Alive)
            {
                ushort x = 0;
                ushort y = 0;
                Map.GetRandCoord(ref x, ref y);
                user.Teleport(x, y, MapID);
                return true;
            }
            return false;
        }
        private void Create()
        {
            GenerateBoxes();
        }
        private void GenerateBoxes()
        {
            for (int i = CurrentBoxes; i < 6; i++)
            {
                byte rand = (byte)Program.GetRandom.Next(0, 5);
                ushort x = 0;
                ushort y = 0;
                Map.GetRandCoord(ref x, ref y);

                Game.MsgNpc.Npc np = Game.MsgNpc.Npc.Create();
                while (true)
                {
                    np.UID = (uint)Program.GetRandom.Next(10000, 100000);
                    if (Map.View.Contain(np.UID, x, y) == false)
                        break;
                }
                np.NpcType = Role.Flags.NpcType.Talker;
                switch (rand)
                {
                    case 0: np.Mesh = 26586; break;
                    case 1: np.Mesh = 26596; break;
                    case 2: np.Mesh = 26606; break;
                    case 3: np.Mesh = 26616; break;
                    case 4: np.Mesh = 26626; break;
                    default: np.Mesh = 26586; break;
                }
                np.Map = MapID;
                np.X = x;
                np.Y = y;
                Map.AddNpc(np);
            }
            CurrentBoxes = 6;
        }
        public void CheckUp()
        {
            if (Process == ProcesType.Alive)
            {
                if (DateTime.Now > StartTimer)
                {
#if Arabic
                     MsgSchedules.SendSysMesage("All Players of Treasure Thief Stage 1 has teleported to Stage 2 in Frozen map!", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);
                   
#else
                    MsgSchedules.SendSysMesage("All Players of Treasure Thief Stage 1 has teleported to Twin City!", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);

#endif
                    foreach (var user in Map.Values)
                    {

                        user.Teleport(300, 278, 1002);
                    }                 
                    Process = ProcesType.Dead;
                }
                else if (DateTime.Now > BoxesStamp)
                {
                    GenerateBoxes();
                    BoxesStamp = DateTime.Now.AddSeconds(30);
                }
            }
        }
        public void Reward(Client.GameClient user, Game.MsgNpc.Npc npc, ServerSockets.Packet stream)
        {
            CurrentBoxes -= 1;
            byte rand = (byte)Program.GetRandom.Next(0, 3);
            switch (rand)
            {
                case 0://money
                    {
                        uint value = (uint)Program.GetRandom.Next(5000000, 10000000);
                        user.Player.Money += value;
                        user.Player.SendUpdate(stream, user.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                        user.CreateBoxDialog("You've received " + value + " Money.");
                        MsgSchedules.SendSysMesage(user.Player.Name + " got " + value.ToString() + " Money while opening the TreasureBox!", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);
                        break;
                    }
                case 1://cps
                    {
                        uint value = (uint)Program.GetRandom.Next(5000, 10000);
                        user.Player.ConquerPoints += value;
                        MsgSchedules.SendSysMesage(user.Player.Name + " got " + value.ToString() + " CPs while opening the TreasureBox!", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);
                        user.CreateBoxDialog("You've received " + value + " ConquerPoints.");
                        break;
                    }
                case 2://item.
                    {
                        uint[] Items = new uint[]
                        {
                            3304151,
                            3303698,
                            3004245,
                            3301224,
                            3008733,
                            729847,
                            3000776,
                            3000778,
                        };
                        uint ItemID = Items[Program.GetRandom.Next(0, Items.Length)];
                        Database.ItemType.DBItem DBItem;
                        if (Database.Server.ItemsBase.TryGetValue(ItemID, out DBItem))
                        {
                            if (user.Inventory.HaveSpace(1))
                                user.Inventory.Add(stream, DBItem.ID);
                            else
                                user.Inventory.AddReturnedItem(stream, DBItem.ID);
                            MsgSchedules.SendSysMesage(user.Player.Name + " got " + DBItem.Name + " while opening the TreasureBox!", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);
                        }
                        break;
                    }

            }
            user.Player.CurrentTreasureBoxes += 1;
            user.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, "accession1");
            Map.RemoveNpc(npc, stream);
            ShuffleGuildScores(stream);
        }
        public void ShuffleGuildScores(ServerSockets.Packet stream)
        {
            foreach (var user in Map.Values)
            {
                Game.MsgServer.MsgMessage msg = new MsgServer.MsgMessage("---Your Score: " + user.Player.CurrentTreasureBoxes + "---", MsgServer.MsgMessage.MsgColor.yellow, MsgServer.MsgMessage.ChatMode.FirstRightCorner);
                user.Send(msg.GetArray(stream));
            }
            var array = Map.Values.OrderByDescending(p => p.Player.CurrentTreasureBoxes).ToArray();
            for (int x = 0; x < Math.Min(10, Map.Values.Length); x++)
            {
                var element = array[x];
                Game.MsgServer.MsgMessage msg = new MsgServer.MsgMessage("No " + (x + 1).ToString() + "- " + element.Player.Name + " Opened " + element.Player.CurrentTreasureBoxes.ToString() + " Boxes!", MsgServer.MsgMessage.MsgColor.yellow, MsgServer.MsgMessage.ChatMode.ContinueRightCorner);
                Send(msg.GetArray(stream));
            }
        }
        public void Send(ServerSockets.Packet stream)
        {
            foreach (var user in Map.Values)
                user.Send(stream);
        }
    }
}

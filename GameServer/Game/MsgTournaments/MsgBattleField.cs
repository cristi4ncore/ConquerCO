using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeathWish.Game.MsgServer;
namespace DeathWish.Game.MsgTournaments
{
    public class Participant
    {
        public string Name = "";
        public uint UID = 0;
        public uint BattlePoints;

        public Participant()
        {

        }

        public Participant(Client.GameClient user)
        {
            Name = user.Player.Name;
            UID = user.Player.UID;
            BattlePoints = user.Player.BattleFieldPoints;
        }
        public override string ToString()
        {
            Database.DBActions.WriteLine line = new Database.DBActions.WriteLine('/');
            line.Add(UID).Add(BattlePoints).Add(Name);
            return line.Close();
        }
    }
    public class MsgBattleField : ITournament
    {
        public ProcesType Process { get; set; }
        private DateTime StartTimer = new DateTime();
        private DateTime BoxesStamp = new DateTime();
        public int CurrentBoxes = 0;
        public List<Participant> Rank3 = new List<Participant>();
        Role.GameMap _map;
        public Role.GameMap Map
        {
            get
            {
                if (_map == null)
                    _map = Database.Server.ServerMaps[3030];
                return _map;
            }
        }
        public TournamentType Type { get; set; }
        public MsgBattleField(TournamentType _type)
        {
            Type = _type;
            Process = ProcesType.Dead;
            StartTimer = DateTime.Now;
            BoxesStamp = DateTime.Now;
        }

        public bool Join(Client.GameClient user, ServerSockets.Packet stream)
        {
            if (Process == ProcesType.Alive)
            {
                user.Player.BattleFieldPoints = 0;
                user.Teleport(250, 250, 3030);
                return true;
            }
            return false;
        }
        public void Open()
        {
            if (Process == ProcesType.Dead)
            {
                Create();
                StartTimer = DateTime.Now;
                BoxesStamp = DateTime.Now;
                Process = ProcesType.Alive;
                foreach (var client in Database.Server.GamePoll.Values)
                {
                    client.Player.MessageBox("DIABLO-EVENT is begin! Will you need join it?", new Action<Client.GameClient>(p =>{p.Teleport(381, 336, 1002);}), null, 500);
                }
            }

        }
        public bool InTournament(Client.GameClient user)
        {
            return
                user.Player.Map == 3030;
        }
        private List<Client.GameClient> Participants()
        {
            List<Client.GameClient> Participants = new List<Client.GameClient>();

            foreach (var user in Database.Server.GamePoll.Values)
            {
                if (user.Player.Map == 3030)
                {
                    if (user.Player.DynamicID == 0)
                        Participants.Add(user);
                }
            }
            return Participants;
        }
        private void Create()
        {
            GenerateBoxes();
        }
        private void GenerateBoxes()
        {
            for (int i = CurrentBoxes; i < 10; i++)
            {
                byte rand = (byte)Program.GetRandom.Next(0, 9);
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
                    case 0: np.Mesh = 26630; break;
                    case 1: np.Mesh = 26630; break;
                    case 2: np.Mesh = 26630; break;
                    case 3: np.Mesh = 26630; break;
                    case 4: np.Mesh = 26630; break;
                    case 5: np.Mesh = 26630; break;
                    case 6: np.Mesh = 26630; break;
                    case 7: np.Mesh = 26630; break;
                    case 8: np.Mesh = 26630; break;
                    case 9: np.Mesh = 26630; break;
                }
                np.Map = 3030;
                np.X = x;
                np.Y = y;
                Map.AddNpc(np);
            }
            CurrentBoxes = 10;
        }
        public void GetOut(Client.GameClient client)
        {
            if (client.Player.BattleFieldPoints > 0)
            {
                client.CreateBoxDialog("You've received " + (client.Player.BattleFieldPoints * 10).ToString() + " Cps From DIABLO-Event.");
                client.Player.ConquerPoints += (uint)(client.Player.BattleFieldPoints * 10);
            }
            client.Teleport(300, 278, 1002);
        }

        public void CheckUp()
        {
            if (Process == ProcesType.Alive)
            {
                if (DateTime.Now > StartTimer.AddMinutes(20))
                {
                    Process = ProcesType.Dead;

                    List<Client.GameClient> Users = Participants();
                    CreateRanks(Users);
                    foreach (var user in Users)
                        GetOut(user);
                        Save();

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
                        uint value = (uint)Program.GetRandom.Next(1000000,5000000);
                        user.Player.Money += value;
                        user.Player.SendUpdate(stream, user.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                        Database.Server.AddMapMonster(stream, user.Map, 2438, user.Player.X, user.Player.Y, 7, 7, 1, user.Player.DynamicID, true, MsgFloorItem.MsgItemPacket.EffectMonsters.None, "movego");
                        user.CreateBoxDialog("You've received " + value  + "  Money.");
                        break;
                    }
                case 1://cps
                    {
                        uint value = (uint)Program.GetRandom.Next(5000, 15000);
                        user.Player.ConquerPoints += value;
                        user.CreateBoxDialog("You've received " + value + " ConquerPoints.");
                        Database.Server.AddMapMonster(stream, user.Map, 2438, user.Player.X, user.Player.Y, 7, 7, 1, user.Player.DynamicID, true, MsgFloorItem.MsgItemPacket.EffectMonsters.None, "movego");
                        break;
                    }
                case 2://cps
                    {
                        user.Inventory.AddItemWitchStack(3004249, 0, 1, stream);
                        user.CreateBoxDialog("You've received 1 Scrap Material .");
                        Database.Server.AddMapMonster(stream, user.Map, 2438, user.Player.X, user.Player.Y, 7, 7, 1, user.Player.DynamicID, true, MsgFloorItem.MsgItemPacket.EffectMonsters.None, "movego");
                        break;
                    }
                case 3://cps
                    {
                        user.Inventory.AddItemWitchStack(729847, 0, 1, stream);
                        user.CreateBoxDialog("You've received 1 DIABLO[T] .");
                        Database.Server.AddMapMonster(stream, user.Map, 2438, user.Player.X, user.Player.Y, 7, 7, 1, user.Player.DynamicID, true, MsgFloorItem.MsgItemPacket.EffectMonsters.None, "movego");
                        break;
                    }
            }
            user.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, "accession1");
            Map.RemoveNpc(npc, stream);
        }
        public void CreateRanks(List<Client.GameClient> Users)
        {
            Rank3.Clear();

            List<Participant> Rank = new List<Participant>();
            foreach (var user in Users)
                Rank.Add(new Participant(user));



            var array = Rank.OrderByDescending(p => p.BattlePoints);

            int count = 0;
            foreach (var user in array)
            {
                if (count == 3)
                    break;
                Rank3.Add(user);
                count++;
            }

            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                for (int x = 0; x < Rank3.Count; x++)
                {
                    var element = Rank3[x];
                }
            }
        }
        public void Save()
        {
            using (Database.DBActions.Write write = new Database.DBActions.Write("BattleField.txt"))
            {
                foreach (var user in Rank3)
                    write.Add(user.ToString());
                write.Execute(Database.DBActions.Mode.Open);
            }
        }
        public void Load()
        {
            using (Database.DBActions.Read reader = new Database.DBActions.Read("BattleField.txt"))
            {
                if (reader.Reader())
                {
                    for (int x = 0; x < reader.Count; x++)
                    {
                        Participant part = new Participant();
                        Database.DBActions.ReadLine readline = new Database.DBActions.ReadLine(reader.ReadString(""), '/');
                        part.UID = readline.Read((uint)0);
                        part.BattlePoints = readline.Read((uint)0);
                        part.Name = readline.Read("None");
                        Rank3.Add(part);
                    }
                }
            }
        }
    }
}

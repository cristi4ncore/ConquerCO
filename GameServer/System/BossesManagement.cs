using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeathWish
{
    public class Boss
    {
        public enum SpawnType : byte
        {
            Hourly = 0,
            Daily = 1
        }
        public Boss()
        {
            X = new List<string>();
            Y = new List<string>();
            SpawnHours = new List<string>();
            SpawnMinutes = new List<string>();
        }
        public string Name;
        public bool Alive = false;
        public List<string> X;
        public List<string> Y;
        public List<string> SpawnHours;
        public List<string> SpawnMinutes;
        public ushort MapID;
        public SpawnType Type;
        public uint MonsterID;
        public bool CanSpawn()
        {
            if (DateTime.Now.Second > 15)
            {
                return false;
            }
            switch (Type)
            {
                case SpawnType.Hourly:
                    {
                        if (SpawnMinutes.Contains(DateTime.Now.Minute.ToString()))
                        {
                            if (Alive == false)
                            {
                                Alive = true;
                                return Alive;
                            }
                        }
                        break;
                    }
                case SpawnType.Daily:
                    {
                        if (SpawnHours.Contains(DateTime.Now.Hour.ToString()))
                        {
                            if (SpawnMinutes.Contains(DateTime.Now.Minute.ToString()))
                            {
                                if (Alive == false)
                                {
                                    Alive = true;
                                    return Alive;
                                }
                            }
                        }
                        break;
                    }

            }
            return false;
        }
        public Tuple<ushort, ushort> SelectRandomCoordinate()
        {
            Random r = new Random();
            int i = r.Next(0, X.Count);
            return new Tuple<ushort, ushort>(ushort.Parse(X[i]), ushort.Parse(Y[i]));
        }
    }
    public class BossDatabase
    {
        public static Dictionary<uint, Boss> Bosses = new Dictionary<uint, Boss>();
        public static void Load()
        {
            if (System.IO.File.Exists(Program.ServerConfig.DbLocation + "bosses.txt"))
            {
                using (System.IO.StreamReader read = System.IO.File.OpenText(Program.ServerConfig.DbLocation + "bosses.txt"))
                {
                    while (true)
                    {
                        string GetLine = read.ReadLine();
                        if (GetLine == null) return;
                        string[] line = GetLine.Split(' ');
                        Boss boss = new Boss();
                        boss.Name = line[0];
                        boss.Type = (Boss.SpawnType)byte.Parse(line[1]);
                        boss.MapID = ushort.Parse(line[2]);
                        boss.X.AddRange(line[3].Split(','));
                        boss.Y.AddRange(line[4].Split(','));
                        boss.SpawnHours.AddRange(line[5].Split(','));
                        boss.SpawnMinutes.AddRange(line[6].Split(','));
                        boss.MonsterID = uint.Parse(line[7]);
                        Bosses.Add(boss.MonsterID, boss);
                    }
                }
            }
        }
    }
    public class BossesManagement
    {
        public static void work(Extensions.Time32 clock)
        {
            if (clock > Stamp)
            {
                foreach (var boss in BossDatabase.Bosses.Values)
                {
                    if (boss.CanSpawn())
                    {
                        Tuple<ushort, ushort> Coordinate = boss.SelectRandomCoordinate();
                        var map = Database.Server.ServerMaps[boss.MapID];
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            foreach (var client in Database.Server.GamePoll.Values)
                            {
                                client.Player.MessageBox(boss.Name + " has appeared around (" + Coordinate.Item1 + "," + Coordinate.Item2 + ") on the Dragon Island Hurry and go defeat the beast", new Action<Client.GameClient>(p => p.Teleport(547, 371, 1002, 0)), null, 60);
                            }
                            Database.Server.AddMapMonster(stream, map, boss.MonsterID, Coordinate.Item1, Coordinate.Item2, 0, 0, 1);
                            Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage(boss.Name + " has appeared around (" + Coordinate.Item1 + "," + Coordinate.Item2 + ") on the Dragon Island Hurry and go defeat the beast", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                        }
                    }
                }

                Stamp.Value = clock.Value + 1000;
            }
        }
        public static List<Boss> WhoAlive(ushort mapid)
        {
            return BossDatabase.Bosses.Values.Where(p => p.MapID == mapid && p.Alive).ToList();
        }

        public static Extensions.Time32 Stamp = Extensions.Time32.Now.AddMilliseconds(1000);

    }
}

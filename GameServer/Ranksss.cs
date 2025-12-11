using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeathWish
{
    public static class Ranksss
    {
        public static Dictionary<uint, mostafa> newranks = new Dictionary<uint, mostafa>();
        public class mostafa
        {
            public string NamePlayer;
            public uint MostKill;
            public uint MostDeath;
            public uint MostRevive;
        }
        public static void Save()
        {
            using (var writer = new Database.DBActions.Write("Rankss.txt"))
            {
                foreach (var obj in newranks)
                {
                    var line = new Database.DBActions.WriteLine('/');
                    line.Add(obj.Key).Add(obj.Value.MostDeath).Add(obj.Value.MostKill).Add(obj.Value.MostRevive).Add(obj.Value.NamePlayer);
                    writer.Add(line.Close());
                }
                writer.Execute(Database.DBActions.Mode.Open);
            }
        }
        public static void Load()
        {
            using (var reader = new Database.DBActions.Read("Rankss.txt", false))
            {
                if (reader.Reader())
                {
                    int count = reader.Count;
                    for (int x = 0; x < count; x++)
                    {
                        var line = new Database.DBActions.ReadLine(reader.ReadString("/"), '/');
                        var item = new mostafa();
                        uint UID = line.Read((uint)0);
                        item.MostDeath = line.Read((uint)0);
                        item.MostKill = line.Read((uint)0);
                        item.MostRevive = line.Read((uint)0);
                        item.NamePlayer = line.Read("");
                        if (!newranks.ContainsKey(UID))
                        {
                            newranks.Add(UID, item);
                        }
                    }
                }
            }
        }
    }
}
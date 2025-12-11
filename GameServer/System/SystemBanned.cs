
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Extensions;
using COServer.Database;

namespace DeathWish.Protection
{
    public class SystemBanned
    {
        public static System.Collections.Generic.SafeDictionary<uint, Client> BannedPoll = new System.Collections.Generic.SafeDictionary<uint, Client>();
        public class Client
        {
            public uint UID;
            public string Username;
            public uint Hours;
            public long StartBan;
            public string Reason = "";
        }
        public static bool IsBanned(DeathWish.Client.GameClient client, bool x = false)
        {
            if (x)
                Load();
            if (BannedPoll.ContainsKey(client.ConnectionUID))
            {
                return true;
            }
            return false;
        }

        public static void RemoveBan(DeathWish.Client.GameClient client)
        {
            uint UID = 0;
            foreach (var obj in BannedPoll.Values)
            {
                if (obj.UID == client.ConnectionUID)
                {
                    UID = obj.UID;
                    break;
                }
            }
            if (BannedPoll.ContainsKey(UID))
            {
                var msg = BannedPoll[UID];
                BannedPoll.Remove(UID);
                using (var cmd = new MySqlCommand(MySqlCommandType.DELETE))
                    cmd.Delete("banned", "UID", msg.UID).Execute();
            }
        }


        public static void Load()
        {
            using (var cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("banned"))
            using (var reader = cmd.CreateReader())
            {
                BannedPoll.Clear();
                while (reader.Read())
                {
                    Client msg = new Client();
                    msg.UID = reader.ReadUInt32("UID");
                    msg.Username = reader.ReadString("username");
                    msg.Hours = reader.ReadUInt32("Hours");
                    msg.StartBan = reader.ReadInt64("StartBan");
                    msg.Reason = reader.ReadString("Reason");
                    BannedPoll.Add(msg.UID, msg);
                }
            }
        }
    }
}

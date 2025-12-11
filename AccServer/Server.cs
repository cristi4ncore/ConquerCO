using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccServer.Network.AuthPackets;
using AccServer.Network;
using AccServer.Database;
namespace AccServer
{
    using Sockets;
    using System;
    using System.Net.Sockets;

    internal sealed class Server : AsynchronousServerSocket
    {
        
        public Server()
            : base("AccountServer", AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
        {
            OnClientConnect = Connect;
            OnClientReceive = Receive;
        }
      
        public void Connect(AsynchronousState state)
        {

            Client.AuthClient authState = new Client.AuthClient(this, state.Socket, new Network.Cryptography.NetDragonAuthenticationCipher());
            state.Client = authState;
            Network.AuthPackets.PasswordCryptographySeed pcs = new Network.AuthPackets.PasswordCryptographySeed();
            pcs.Seed = Program.Random.Next();
            authState.PasswordSeed = pcs.Seed;
            authState.Send(pcs);
        }

        
        public void Receive(AsynchronousState state)
        {

            Client.AuthClient player = state.Client as Client.AuthClient;
            if (player != null && player.Packet != null)
            {
                byte[] packet = player.Packet;
                ushort len = BitConverter.ToUInt16(packet, 0);
                ushort id = BitConverter.ToUInt16(packet, 2);
                if (id == 1542 && len == 312)
                {
                    player.Info = new Authentication();
                    player.Info.Deserialize(packet);
                    player.Account = new Database.AccountTable(player.Info.Username, player.Info.Password);
                    Database.ServerInfo Server = null;
                    Forward Fw = new Forward();
                    if (Database.Server.Servers.TryGetValue(player.Info.Server, out Server))
                    {
                        if (player.Account.Password == player.Info.Password && player.Account.exists)
                        {
                            if (player.Account.EntityID == 0)
                            {
                                using (Database.MySqlCommand cmd = new Database.MySqlCommand(Database.MySqlCommandType.SELECT))
                                {
                                    cmd.Select("configuration");
                                    using (MySqlReader r = new MySqlReader(cmd))
                                    {
                                        if (r.Read())
                                        {
                                            Program.EntityUID = new Counter(r.ReadUInt32("EntityID"));
                                            player.Account.EntityID = Program.EntityUID.Next;
                                            using (MySqlCommand cmd2 = new MySqlCommand(MySqlCommandType.UPDATE).Update("configuration")
                                            .Set("EntityID", player.Account.EntityID))
                                            cmd2.Execute();
                                            player.Account.Save();
                                        }
                                    }
                                }

                            }
                            TransferCipher transferCipher = new TransferCipher(Server.TransferKey, Server.TransferSalt, "127.0.0.1");
                            uint[] encrypted = transferCipher.Encrypt(new uint[] { player.Account.EntityID, (uint)player.Account.State });
                            Fw.Identifier = encrypted[0];
                            Fw.State = (uint)encrypted[1];
                            Fw.IP = Server.IP;
                            Fw.Port = Server.Port;
                            Console.WriteLine("{0} has been successfully transferred to server {1}! IP:[{2}].",
                                   player.Info.Username, player.Info.Server, player.IPAddress);
                            player.Send(Fw);
                        }
                        else
                        {
                            Fw.Type = Forward.ForwardType.InvalidInfo;
                            player.Send(Fw);
                            player.Disconnect();
                        }
                    }
                    else
                    {
                        Fw.Type = Forward.ForwardType.ServersNotConfigured;
                        player.Send(Fw);
                        player.Disconnect();
                    }

                }
                else
                {
                    player.Disconnect();
                }
            }
            else
            {
                player.Disconnect();
            }
        }
    }
}

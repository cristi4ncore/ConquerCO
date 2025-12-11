// * This Project Created by Mostafa Desha
// * Copyright MostafaDesha © 2025


using System;
using AccServer.Network;
using AccServer.Database;
using System.Windows.Forms;
using AccServer.Network.Sockets;
using AccServer.Network.AuthPackets;
using System.Threading.Tasks;
using AccServer.Network.Protaction;
using System.IO;

namespace AccServer
{
    public unsafe class Program
    {
        public static Counter EntityUID;
        public static FastRandom Random = new FastRandom();
        public static ServerSocket AuthServer;
        public static World World;
        public static ushort Port = 9960;
        private static object SyncLogin;
        private static System.Collections.Concurrent.ConcurrentDictionary<uint, int> LoginProtection;
        private const int TimeLimit = 10000;
        public static string DBuser = "root";
        public static string DBhost = "localhost";
        public static string DBName = "cq";//Nombre DB pueden poner el que quieran.
        public static string DBPass = "Suged2403";//Contraseña que le pusimos al Instalar App en mi caso 12345678
        private static void WorkConsole()
        {
            while (true)
            {
                try
                {
                    CommandsAI(Console.ReadLine());
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }
        private static void WriteHeader()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("--------- AccServer ----------");
            Console.WriteLine("------------------------------\n");
            Console.ForegroundColor = ConsoleColor.DarkGray;
        }
        public static string ConnectionString = "Server='localhost';Database='cq';Username='root';Password='Suged2403';Pooling=true; Max Pool Size = 32; Min Pool Size = 300; Connect Timeout=15";
        static void Main(string[] args)
        {
            Console.Title = "Account Server";
            WriteHeader();
            Database.DataHolder.CreateConnection();
            World = new World();
            World.Init();
            EntityUID = new Counter(0);
            LoginProtection = new System.Collections.Concurrent.ConcurrentDictionary<uint, int>();
            SyncLogin = new object();
            Database.Server.Load();
            Network.Cryptography.AuthCryptography.PrepareAuthCryptography();
            AuthServer = new ServerSocket();
            AuthServer.OnClientConnect += AuthServer_OnClientConnect;
            AuthServer.OnClientReceive += AuthServer_OnClientReceive;
            AuthServer.OnClientDisconnect += AuthServer_OnClientDisconnect;
            AuthServer.Enable(Port, "0.0.0.0");
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("Auth Port " + Port);
            Console.WriteLine("");
            WorkConsole();
            CommandsAI(Console.ReadLine());
        }
        public static void CommandsAI(string command)
        {
            if (command == null) return;
            string[] data = command.Split(' ');
            switch (data[0])
            {
                case "@memo":
                    {
                        var proc = System.Diagnostics.Process.GetCurrentProcess();
                        Console.WriteLine("Thread count: " + proc.Threads.Count);
                        Console.WriteLine("Memory set(MB): " + ((double)((double)proc.WorkingSet64 / 1024)) / 1024);
                        proc.Close();
                        break;
                    }
                case "@a":
                    {
                        Console.Clear();
                        break;
                    }
                case "@restart":
                    {
                        AuthServer.Disable();
                        Application.Restart();
                        Environment.Exit(0);
                        break;
                    }
            }
        }

        private static void AuthServer_OnClientReceive(byte[] buffer, int length, ClientWrapper arg3)
        {
            var player = arg3.Connector as Client.AuthClient;

            if (player == null)
                return;

            player.Cryptographer.Decrypt(buffer, length);
            player.Queue.Enqueue(buffer, length);
            while (player.Queue.CanDequeue())
            {
                byte[] packet = player.Queue.Dequeue();
                ushort id = BitConverter.ToUInt16(packet, 2);
                if (id == (ushort)1542)
                {
                    player.Info = new Authentication();
                    player.Info.Deserialize(packet);
                    player.Account = new AccountTable(player.Info.Username);

                    Database.ServerInfo Server = null;
                    Forward Fw = new Forward();
                    if (Database.Server.Servers.TryGetValue(player.Info.Server, out Server))
                    {
                        if (!player.Account.exists)
                            Fw.Type = Forward.ForwardType.WrongAccount;
                        else
                        {
                            if (player.Account.Password == player.Info.Password)
                            {
                                if (!player.Account.Banned)
                                {
                                    Fw.Type = Forward.ForwardType.Ready;
                                    AccountTable.UpdateIP(player.Info.Username, player.IP);
                                    if (player.Account.EntityID == 0)
                                    {
                                        var reader = new MySqlCommand(MySqlCommandType.SELECT).Select("configuration");
                                        using (var rdr = new MySqlReader(reader))
                                        {
                                            if (rdr.Read())
                                            {
                                                EntityUID = new Counter(rdr.ReadUInt32("EntityID"));
                                                player.Account.EntityID = EntityUID.Next;
                                                var cmd = new MySqlCommand(MySqlCommandType.UPDATE).Update("configuration").Set("EntityID", player.Account.EntityID);
                                                cmd.Execute();
                                                player.Account.Save();
                                            }
                                        }
                                    }
                                }
                                else Fw.Type = Forward.ForwardType.Banned;
                            }
                            else
                                Fw.Type = Forward.ForwardType.InvalidInfo;
                        }
                        lock (SyncLogin)
                        {
                            if (Fw.Type == Forward.ForwardType.Ready)
                            {
                                Fw.Identifier = player.Account.EntityID;
                                Fw.IP = Server.IP;
                                Fw.Port = Server.Port;

                                //Console.WriteLine("{0} is tryna connect to {1}! IP: {2}.", player.Info.Username, player.Info.Server, player.IP);
                                string timer = DateTime.Now.ToString("HH:mm:ss");
                                Console.WriteLine($"[{timer}] - [{player.Info.Username}] login to server [{player.Info.Server}]! IP:[{player.IP}].", ConsoleColor.Green);
                                string logRootFolder = "LoginGame";
                                if (!System.IO.Directory.Exists(logRootFolder))
                                {
                                    System.IO.Directory.CreateDirectory(logRootFolder);
                                }
                                string playerFolder = System.IO.Path.Combine(logRootFolder, player.Account.EntityID.ToString());
                                if (!System.IO.Directory.Exists(playerFolder))
                                {
                                    System.IO.Directory.CreateDirectory(playerFolder);
                                }
                                string logFileName = DateTime.Now.ToString("dd-MM-yyyy") + ".txt";
                                string logFilePath = System.IO.Path.Combine(playerFolder, logFileName);
                                if (!File.Exists(logFilePath))
                                {
                                    using (System.IO.FileStream fs = System.IO.File.Create(logFilePath))
                                    {
                                        fs.Close();
                                    }
                                }
                                using (System.IO.StreamWriter file = new System.IO.StreamWriter(logFilePath, true))
                                {
                                    file.WriteLine("---------------------------------------------------");
                                    file.WriteLine(string.Format("UIDAccount : {0}", player.Account.EntityID));
                                    file.WriteLine(string.Format("UserName : {0}", player.Info.Username));
                                    file.WriteLine(string.Format("Password : {0}", player.Info.Password));
                                    file.WriteLine(string.Format("ServerName : {0}", player.Info.Server));
                                    file.WriteLine(string.Format("IPAccount : {0}", player.IP));
                                    file.WriteLine(string.Format("HDSerial : {0}", player.Info.Mac));
                                    file.WriteLine(string.Format("Log Time : {0:HH:mm:ss}", DateTime.Now));
                                    file.WriteLine(string.Format("Date: {0:dd-MM-yyyy}", DateTime.Now));
                                    file.WriteLine("---------------------------------------------------");
                                }
                            }
                            player.Send(Fw);
                        }
                    }
                    else
                    {
                        Fw.Type = Forward.ForwardType.ServersNotConfigured;
                        player.Send(Fw);
                    }
                }
            }
        }
        private static void AuthServer_OnClientDisconnect(ClientWrapper obj)
        {
            obj.Disconnect();
        }
        private static void AuthServer_OnClientConnect(ClientWrapper obj)
        {
            Client.AuthClient authState;
            obj.Connector = (authState = new Client.AuthClient(obj));
            authState.Cryptographer = new Network.Cryptography.AuthCryptography();
            Network.AuthPackets.PasswordCryptographySeed pcs = new PasswordCryptographySeed();
            pcs.Seed = Program.Random.Next();
            authState.PasswordSeed = pcs.Seed;
            authState.Send(pcs);
        }
    }
}
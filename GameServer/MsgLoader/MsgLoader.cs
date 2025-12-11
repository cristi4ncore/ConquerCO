using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DeathWish.Game.MsgServer;
using DeathWish.ServerSockets;

namespace DeathWish.MsgProtection
{
    public static class MsgLoader
    {
        #region Enums
        public enum PacketTypes : ushort
        {
            CMsgLoginGame = 7001,//GameServer Login
            CMsgTqProtection = 7002,//Protection
            CMsgFilesCheck = 7003,//MsgCheckFiles
            CMsgProPacket = 7004,//SendToLoader
        }
        public enum CheatFlags : byte
        {
            SuspendedThreadDetected = 1,
            AutoClickerDetected = 2,
            DebuggerDetected = 3,
            MemoryModified = 4,
            UnknownOperation = 5
        }
        public enum ActionType : ushort
        {
            DoLogin = 0,
            RequestHash = 1,
            TerminateLoader = 2,
        }
        #endregion
        #region CMsgLoginGame Packet
        [PacketAttribute((ushort)MsgLoader.PacketTypes.CMsgLoginGame)]
        public unsafe static void LoginGame(Client.GameClient client, ServerSockets.Packet packet)
        {
            byte[] buffer = new byte[packet.Size];
            fixed (byte* ptr = buffer)
            {
                packet.memcpy(ptr, packet.Memory, packet.Size);
            }
            packet.Seek(4);

            client.OnLogin = new DeathWish.Game.MsgServer.MsgLoginClient()
            {
                Key = packet.ReadUInt32(),
                HDSerial = Encoding.ASCII.GetString(packet.ReadBytes(16)).TrimEnd('\0'),
            };
            
            client.ClientFlag |= Client.ServerFlag.OnLoggion;
            Database.ServerDatabase.LoginQueue.TryEnqueue(client);
            MsgProtection.MsgLoader.SendLogin(client);
        }
        #endregion
        #region Protection Packet //[Done]
        [PacketAttribute((ushort)PacketTypes.CMsgTqProtection)]
        public unsafe static void LoaderHandler(Client.GameClient pClient, ServerSockets.Packet stream)
        {
            stream.Seek(4);
            Byte[] pBuffer = stream.ReadBytes(stream.Size);

            using (BinaryReader BR = new BinaryReader(new MemoryStream(pBuffer)))
            {
                CheatFlags type = (CheatFlags)BR.ReadInt32();
                string message = Encoding.ASCII.GetString(BR.ReadBytes(33)).TrimEnd('\0');
                try
                {
                    switch (type)
                    {
                        case CheatFlags.SuspendedThreadDetected:
                            {
                                Console.WriteLine("Account: {0} Disconnected due to Cheat Suspended\n{1}.", pClient.Player.Name, "Proccess Modifi?!! [SuspendedThreadDetected]");
                                pClient.Socket.Disconnect();
                                break;
                            }
                        case CheatFlags.AutoClickerDetected:
                            {
                                Console.WriteLine("Account: {0} Disconnected due to AutoClickerDetected / KeyBord Inject Key\n{1}.", pClient.Player.Name, "Proccess Modifi?!! [AutoClickerDetected]");
                                pClient.Socket.Disconnect();
                                break;
                            }
                        case CheatFlags.DebuggerDetected:
                            {
                                Console.WriteLine("Account: {0} Disconnected due to DebuggerDetected\n{1}.", pClient.Player.Name, "Proccess Modifi?!! [DebuggerDetected]");
                                pClient.Socket.Disconnect();
                                break;
                            }
                        case CheatFlags.MemoryModified:
                            {
                                Console.WriteLine("Account: {0} Disconnected due to MemoryModified\n{1}.", pClient.Player.Name, "Proccess Modifi?!! [MemoryModified]");
                                pClient.Socket.Disconnect();
                                break;
                            }
                        default:
                            {
                                Console.WriteLine("Warning: Unknown ActionType encountered: " + type);
                                break;
                            }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Warning: General error: " + ex.Message);
                }
                BR.Close();
            }
        }
        #endregion
        #region FileHashes Packet //[Done]

        public static string strParam;

        public static string dx8exe_hash;
        public static string dx8dll_hash;

        public static string dx9exe_hash;
        public static string dx9dll_hash;

        public static string serverdat_hash;

        public static string magic_hash;

        public static string magiceffect_hash;

        public static string strRes_hash;

        public static string ustrRes_hash;

        [PacketAttribute((ushort)PacketTypes.CMsgFilesCheck)]
        public unsafe static void CMsgFilesCheck(Client.GameClient pClient, ServerSockets.Packet stream)
        {
            stream.Seek(4);
            Byte[] pBuffer = stream.ReadBytes(stream.Size);

            using (BinaryReader binaryReader = new BinaryReader(new MemoryStream(pBuffer)))
            {
                try
                {
                    string user_dx8exe_hash = ReadHashFromPacket(binaryReader);
                    string user_dx8dll_hash = ReadHashFromPacket(binaryReader);
                    string user_dx9exe_hash = ReadHashFromPacket(binaryReader);
                    string user_dx9dll_hash = ReadHashFromPacket(binaryReader);
                    string user_serverdat_hash = ReadHashFromPacket(binaryReader);
                    string user_magic_hash = ReadHashFromPacket(binaryReader);
                    string user_magiceffect_hash = ReadHashFromPacket(binaryReader);
                    string user_strRes_hash = ReadHashFromPacket(binaryReader);
                    string user_ustrRes_hash = ReadHashFromPacket(binaryReader);

                    if (!ValidHash(user_dx8exe_hash, user_dx9exe_hash, user_dx8dll_hash,
                        user_dx9dll_hash, user_serverdat_hash, user_magic_hash,
                        user_magiceffect_hash, user_strRes_hash, user_ustrRes_hash, out strParam))
                    {
                        Console.WriteLine(" Account: {0} Disconnected For changing client files\n{1}.", pClient.Player.Name, strParam);
                        strParam = "Change FILES[" + strParam + "]"; strParam += "\nplease download game patches or last patch";
                        pClient.Send(new Game.MsgServer.MsgMessage(strParam, "ALLUSERS", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Dialog).GetArray(stream));
                        pClient.Socket.Disconnect();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Warning: General error: " + ex.Message);
                }
                binaryReader.Close();
            }
        }

        public static bool CheakConquer = false;
        public static void LoadHashes(bool Read = true)
        {
            if (Read)
            {
                CheakConquer = true;
                dx8exe_hash = CalculateMD5(@"LoaderFiles\Env_DX8\Conquer.exe");
                dx8dll_hash = CalculateMD5(@"LoaderFiles\Env_DX8\TqProtection.dll");

                dx9exe_hash = CalculateMD5(@"LoaderFiles\Env_DX9\Conquer.exe");
                dx9dll_hash = CalculateMD5(@"LoaderFiles\Env_DX9\TqProtection.dll");

                serverdat_hash = CalculateMD5(@"LoaderFiles\TqProGuard.dat");

                magic_hash = CalculateMD5(@"LoaderFiles\ini\magictype.dat");
                magiceffect_hash = CalculateMD5(@"LoaderFiles\ini\MagicEffect.ini");
                strRes_hash = CalculateMD5(@"LoaderFiles\ini\Cn_Res.ini");
                ustrRes_hash = CalculateMD5(@"LoaderFiles\ini\StrRes.ini");
                Console.WriteLine("All Hashes Loaded...");
            }

        }
        private static string CalculateMD5(string filename)
        {
            try
            {
                using (var md5 = System.Security.Cryptography.MD5.Create())
                {
                    using (var stream = System.IO.File.OpenRead(filename))
                    {
                        var hash = md5.ComputeHash(stream);
                        string H = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                        return H;
                    }
                }
            }
            catch
            {
                Console.WriteLine(string.Format("{0} Not Find Hash", filename));
            }
            return "";
        }
        public static bool ValidHash(string conquer8, string conquer9, string dll8_Hash, string dll9_Hash,
            string Data_Servers, string magicType, string magicEffect, string StrRes, string UStrRes, out string filechanged)
        {
            filechanged = "";
            if (CheakConquer)
            {
                if (conquer8 != dx8exe_hash)
                    filechanged += " Conquer.exe [DX8]";

                if (conquer9 != dx9exe_hash)
                    filechanged += " Conquer.exe [DX9]";
            }
            if (dll8_Hash != dx8dll_hash)
                filechanged += " TqProtection.dll [DX8]";
            if (dll9_Hash != dx9dll_hash)
                filechanged += " TqProtection.dll [DX9]";

            if (Data_Servers != serverdat_hash)
                filechanged += " Servers.dat";

            if (magicType != magic_hash)
                filechanged += " magictype.dat";
            if (magicEffect != magiceffect_hash)
                filechanged += " MagicEffect.ini";

            if (StrRes != strRes_hash)
                filechanged += " Cn_Res.ini";
            if (UStrRes != ustrRes_hash)
                filechanged += " StrRes.ini";

            return filechanged == "" ? true : false;
        }

        public static string ReadHashFromPacket(BinaryReader reader)
        {
            byte[] bytes = reader.ReadBytes(33);
            string hash = Encoding.ASCII.GetString(bytes);
            return hash.TrimEnd('\0');
        }
        #endregion
        #region Send
        public static ServerSockets.Packet SendProPacket(ServerSockets.Packet Stream, ActionType sData)
        {
            Stream.InitWriter();
            Stream.Write(Convert.ToInt32(sData));
            Stream.Finalize(Convert.ToUInt16(PacketTypes.CMsgProPacket));
            return Stream;
        }
        public static void SendPacket(Client.GameClient pClient, ActionType In)
        {
            using (var Recycled = new RecycledPacket())
            {
                var cPacket = Recycled.GetStream();
                pClient.Send(SendProPacket(cPacket, In));
            }
        }
        public static void SendAction(Client.GameClient pClient, ActionType Action)
        {
            SendPacket(pClient, Action);
        }
        #endregion
        #region Send Login & TerminateLoader
        public static void SendLogin(Client.GameClient pClient)
        {
            MsgLoader.SendAction(pClient, MsgProtection.MsgLoader.ActionType.DoLogin);
            MsgLoader.SendAction(pClient, MsgProtection.MsgLoader.ActionType.RequestHash);
        }
        public static void SendTerminateLoader(Client.GameClient pClient)
        {
            MsgLoader.SendAction(pClient, MsgProtection.MsgLoader.ActionType.TerminateLoader);
        }
        #endregion
    }
}
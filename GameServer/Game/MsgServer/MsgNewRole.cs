using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeathWish.Game.MsgServer
{
    public static class MsgNewRole
    {

        public static object SynName = new object();

        public static void GetNewRoleInfo(this ServerSockets.Packet msg, out string name, out ushort Body, out byte Class)
        {
            msg.ReadBytes(20);
            name = msg.ReadCString(16);
            msg.ReadBytes(32);
            Body = msg.ReadUInt16();
            Class = msg.ReadUInt8();
        }

        [PacketAttribute(Game.GamePackets.NewClient)]
        public unsafe static void CreateCharacter(Client.GameClient client, ServerSockets.Packet stream)
        {
            Client.GameClient bug;
            if (Database.Server.GamePoll.TryGetValue(client.ConnectionUID, out bug))
            {
                client.Socket.Disconnect();
                bug.Socket.Disconnect();
                return;
            }
            if (System.IO.File.Exists(Program.ServerConfig.DbLocation + "\\Users\\" + client.ConnectionUID + ".ini") == true)
            {
                Client.GameClient bugs;
                if (Database.Server.GamePoll.TryGetValue(client.ConnectionUID, out bugs))
                {
                    bugs.Socket.Disconnect();
                }
                client.Socket.Disconnect();
                return;
            }
            if ((client.ClientFlag & Client.ServerFlag.CreateCharacter) == Client.ServerFlag.CreateCharacter)
            {
                client.ClientFlag &= ~Client.ServerFlag.AcceptLogin;

                string CharacterName; ushort Body; byte Class;

                stream.GetNewRoleInfo(out CharacterName, out Body, out Class);

                byte attackType = 0;
                switch (Class)
                {
                    case 0:
                    case 1: Class = 100; break;
                    case 2:
                    case 3: Class = 10; break;
                    case 4:
                    case 5: Class = 40; break;
                    case 6:
                    case 7: Class = 20; break;
                    case 8:
                    case 9: Class = 50; break;
                    case 10:
                    case 11: Class = 60; break;
                    case 12:
                    case 13: Class = 70; break;
                    case 14:
                    case 15: Class = 80; break;

                    case 16:
                    case 17:
                        {
                            attackType = 0;
                            Class = 160;
                            break;
                        }
                    case 18:
                    case 19:
                        {
                            attackType = 1;
                            Class = 160;
                            break;
                        }
                }

                if (!ExitBody(Body))
                {
                    client.Send(new MsgServer.MsgMessage("AHAHAH! WRONG Body, NICE TRY", MsgMessage.MsgColor.red, MsgMessage.ChatMode.PopUP).GetArray(stream));

                    return;
                }
                if (!ExitClass(Class))
                {
                    client.Send(new MsgServer.MsgMessage("AHAHAH! WRONG Class, NICE TRY", MsgMessage.MsgColor.red, MsgMessage.ChatMode.PopUP).GetArray(stream));

                    return;
                }

                CharacterName = CharacterName.Replace("\0", "");
                if (Program.NameStrCheck(CharacterName))
                {
                    if (!Database.Server.NameUsed.Contains(CharacterName.GetHashCode()))
                    {
                        client.ClientFlag &= ~Client.ServerFlag.CreateCharacter;

                        lock (Database.Server.NameUsed)
                            Database.Server.NameUsed.Add(CharacterName.GetHashCode());

                        client.Player.Name = CharacterName;
                        client.Player.Class = Class;
                        client.Player.Body = Body;
                        client.Player.VipLevel = 1;
                        client.Player.Level = 1;
                        client.Player.Map = 1002;
                        client.Player.X = 439;
                        client.Player.Y = 394;

                        Database.DataCore.LoadClient(client.Player);

                        client.Player.UID = client.ConnectionUID;
                        if (attackType == 1)
                            client.Player.MainFlag |= Role.Player.MainFlagType.OnMeleeAttack;

                        Database.DataCore.AtributeStatus.GetStatus(client.Player);

                        Database.DataCore.SetCharacterSides(client.Player);
                        byte Color = (byte)Program.GetRandom.Next(4, 8);
                        client.Player.Hair = (ushort)(Color * 100 + 10 + (byte)Program.GetRandom.Next(4, 9));
                        client.Player.Money += 5000;
                        client.Inventory.Add(stream, 721259, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                        client.Inventory.Add(stream, 723701, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                        client.Inventory.Add(stream, 711083, 3, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                        client.Inventory.Add(stream, 3001027, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                        client.Inventory.Add(stream, 727266, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                        client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                        client.Send(new MsgServer.MsgMessage("ANSWER_OK", MsgMessage.MsgColor.red, MsgMessage.ChatMode.PopUP).GetArray(stream));
                        client.Status.MaxHitpoints = client.CalculateHitPoint();
                        client.Player.HitPoints = (int)client.Status.MaxHitpoints;
                        client.ClientFlag |= Client.ServerFlag.CreateCharacterSucces;
                    }
                    else
                    {
                        client.Send(new MsgServer.MsgMessage("The name is in use! try other name", MsgMessage.MsgColor.red, MsgMessage.ChatMode.PopUP).GetArray(stream));
                    }
                }
                else
                {
                    client.Send(new MsgServer.MsgMessage("Invalid characters name!", MsgMessage.MsgColor.red, MsgMessage.ChatMode.PopUP).GetArray(stream));
                }
            }
        }
        public static bool ExitBody(ushort _body)
        {
            return (_body == 1003 || _body == 1004 || _body == 2001 || _body == 2002);
        }
        public static bool ExitClass(byte cls)
        {
            return (cls == 10 || cls == 20 || cls == 40
                || cls == 50 || cls == 60 || cls == 70 || cls == 100 || cls == 80 || cls == 160);
        }
    }
}
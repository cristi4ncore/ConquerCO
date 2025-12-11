using DeathWish.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeathWish.Game.MsgServer
{
    public unsafe static partial class MsgBuilder
    {
        public static unsafe void GetSuperFlag(this ServerSockets.Packet stream, out MsgSuperFlag.Types Actions, out uint Index, out uint UID)
        {
            Actions = (MsgSuperFlag.Types)stream.ReadUInt32();
            UID = stream.ReadUInt32();
            Index = stream.ReadUInt32();
        }
        public static unsafe ServerSockets.Packet CreateSuperFlag(this ServerSockets.Packet stream, MsgGameItem item, Dictionary<uint, Tuple<uint, uint, uint, string>> Gates)
        {
            stream.InitWriter();
            stream.Write((uint)MsgSuperFlag.Types.Info);
            stream.Write(item.UID);
            stream.Write((uint)Gates.Count);
            stream.Write((uint)Gates.Count);
            stream.Write((uint)0);
            stream.Write((uint)item.Durability);
            stream.Write(Gates.Count);
            if (Gates.Count > 0)
            {
                for (uint x = 0; x < Gates.Count; x++)
                {
                    stream.Write(x);
                    stream.Write(Gates[x].Item1);
                    stream.Write(Gates[x].Item2);
                    stream.Write(Gates[x].Item3);
                    stream.Write(Gates[x].Item4, 32);
                }
            }
            stream.Finalize(GamePackets.MsgSuperFlag);
            return stream;
        }
    }
    public unsafe struct MsgSuperFlag
    {
        [Flags]
        public enum Types : uint
        {
            Info = 0,
            Record = 1,
            Teleport = 3,
            Renew = 4
        }

        [PacketAttribute(GamePackets.MsgSuperFlag)]
        public static void Handler(Client.GameClient user, ServerSockets.Packet stream)
        {
            if (Server.NoAgateMap.Contains(user.Player.Map))
            {
                user.SendSysMesage("You can not use this item here!");
                return;
            }
            Types Act;
            uint Index, ItemUID;
            stream.GetSuperFlag(out Act, out Index, out ItemUID);
            MsgGameItem Item;
            if (user.Inventory.TryGetItem(ItemUID, out Item))
            {
                if (Item.ITEM_ID != Database.ItemType.MemoryAgate)
                    return;
                lock (Database.Server.ClientAgates[user.Player.UID][ItemUID])
                {
                    switch (Act)
                    {
                        case Types.Record:
                            {
                                if (user.Player.Map != 1064 && user.Player.Map != 1000 && user.Player.Map != 1002 && user.Player.Map != 1011 && user.Player.Map != 1020 && user.Player.Map != 1015)
                                {
                                    user.CreateBoxDialog("Sorry You Can Only Use It In Main Map Only");
                                    break;
                                }
                                if (Index > Database.Server.ClientAgates[user.Player.UID][ItemUID].Count)
                                {
                                    Database.Server.ClientAgates[user.Player.UID][ItemUID].Add((uint)Database.Server.ClientAgates[user.Player.UID][ItemUID].Count, Tuple.Create(user.Player.Map, (uint)user.Player.X, (uint)user.Player.Y, user.Map.Name));
                                    user.Send(stream.CreateSuperFlag(Item, Database.Server.ClientAgates[user.Player.UID][ItemUID]));
                                }
                                if (Database.Server.ClientAgates[user.Player.UID][ItemUID].ContainsKey(Index))
                                {
                                    Database.Server.ClientAgates[user.Player.UID][ItemUID].Remove(Index);
                                    Database.Server.ClientAgates[user.Player.UID][ItemUID].Add(Index, Tuple.Create(user.Player.Map, (uint)user.Player.X, (uint)user.Player.Y, user.Map.Name));
                                    user.Send(stream.CreateSuperFlag(Item, Database.Server.ClientAgates[user.Player.UID][ItemUID]));
                                }
                                else
                                {
                                    Database.Server.ClientAgates[user.Player.UID][ItemUID].Add(Index, Tuple.Create(user.Player.Map, (uint)user.Player.X, (uint)user.Player.Y, user.Map.Name));
                                    user.Send(stream.CreateSuperFlag(Item, Database.Server.ClientAgates[user.Player.UID][ItemUID]));
                                }
                                break;
                            }
                        case Types.Teleport:
                            {
                                if (user.Player.Alive)
                                {
                                    if (!Program.BlockTeleportMap.Contains(user.Player.Map))
                                    {
                                        if (user.Player.ConquerPoints >= 200)
                                        {
                                            user.Player.ConquerPoints -= 200;
                                            if (Database.Server.ClientAgates[user.Player.UID][ItemUID].ContainsKey(Index))
                                            {
                                                user.Teleport((ushort)Database.Server.ClientAgates[user.Player.UID][ItemUID][Index].Item2, (ushort)Database.Server.ClientAgates[user.Player.UID][ItemUID][Index].Item3, Database.Server.ClientAgates[user.Player.UID][ItemUID][Index].Item1);
                                                Item.Durability -= 1;
                                                user.Send(stream.CreateSuperFlag(Item, Database.Server.ClientAgates[user.Player.UID][ItemUID]));
                                            }
                                        }
                                        else
                                            user.CreateBoxDialog("Sorry You Don`t Have  200 Cps To Can Teleport");

                                    }
                                    else
                                        user.CreateBoxDialog("Sorry You Can`t Teleport From BlockTeleport Map");
                                }
                                else
                                    user.CreateBoxDialog("Sorry You Can`t Teleport While You Not Alive");
                                break;
                            }
                        case Types.Renew:
                            {
                                int cost = (Item.MaximDurability - Item.Durability) / 2;
                                if (cost == 0) cost = 1;
                                if (Item.Bound == 1)
                                {
                                    if (user.Player.ConquerPoints >= cost)
                                    {
                                        user.Player.ConquerPoints -= (uint)cost;
                                        Item.Durability = Item.MaximDurability;
                                        user.Send(stream.CreateSuperFlag(Item, Database.Server.ClientAgates[user.Player.UID][ItemUID]));
                                    }
                                }
                                else
                                {
                                    if (user.Player.ConquerPoints >= cost)
                                    {
                                        user.Player.ConquerPoints -= (uint)cost;
                                        Item.Durability = Item.MaximDurability;
                                        user.Send(stream.CreateSuperFlag(Item, Database.Server.ClientAgates[user.Player.UID][ItemUID]));
                                    }
                                }
                                user.SendSysMesage("MemoryAgate dura renewed.");
                                break;
                            }
                    }
                }
            }
        }
    }
}

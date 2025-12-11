using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace DeathWish.Game.MsgServer
{
    public static class MsgItemRefineOpt
    {
        [ProtoContract]
        public class ItemRefineOpt
        {
            [ProtoMember(1, IsRequired = true)]
            public ActionID Type;
            [ProtoMember(2, IsRequired = true)]
            public uint ItemUID;
            [ProtoMember(3)]
            public string Signature;
            [ProtoMember(4, IsRequired = true)]
            public uint[] Items;
        }

        [Flags]
        public enum ActionID
        {
            Perfection = 0,
            Ownership = 1,
            Signature = 2,
           // CPBoost = 3,
            Exchange = 4,
            Quick = 5
        }

        [PacketAttribute(GamePackets.MsgItemRefineOpt)]
        public static unsafe void Handler(Client.GameClient client, ServerSockets.Packet stream)
        {
          
            ItemRefineOpt msg = new ItemRefineOpt();
            msg = stream.ProtoBufferDeserialize<ItemRefineOpt>(msg);
           
            switch (msg.Type)
            {
                //case ActionID.CPBoost:
                //    {
                //        if (Program.TreadeOrShop.Contains(client.Player.Map))
                //        {
                //            client.SendSysMesage("No Open Or Change Perfec In This Map DIABLO .");
                //            return;
                //        }
                //        MsgGameItem Item;
                //        if (client.TryGetItem(msg.ItemUID, out Item))
                //        {
                //            ushort position = Database.ItemType.ItemPosition(Item.ITEM_ID);
                //            if (position == (ushort)Role.Flags.ConquerItem.Garment
                //                   || position == (ushort)Role.Flags.ConquerItem.Bottle || position == (ushort)Role.Flags.ConquerItem.SteedMount
                //                   || position == (ushort)Role.Flags.ConquerItem.LeftWeaponAccessory
                //                   || position == (ushort)Role.Flags.ConquerItem.RightWeaponAccessory)
                //            {
                //                Console.WriteLine("Client " + client.Player.Name + " cheater.");
                //                client.Socket.Disconnect();
                //                return;
                //            }

                //            uint oldrank = Database.RankItems.RankPoll[(uint)Item.GetPerfectionPosition].GetItemRank(Item.UID);

                //            while (client.Equipment.CanUpdatePerfectionItem(Item) && Item.PerfectionLevel < 54)
                //            {
                //                var currentProgress = Item.PerfectionProgress;
                //                var required = Database.ItemRefineUpgrade.ProgresUpdates[Item.PerfectionLevel + 1];
                //                var cost = (required - currentProgress) / 10 * 8;
                //                if (client.Player.ConquerPoints >= cost * 2)
                //                {
                //                    client.Player.ConquerPoints -= (uint)cost * 2;
                //                    Item.PerfectionProgress = 0;
                //                    Item.PerfectionLevel++;
                //                    Item.OwnerName = client.Player.Name;
                //                    Item.OwnerUID = client.Player.UID;
                //                    Item.Mode = Role.Flags.ItemMode.Update;
                //                    Item.Send(client, stream);
                //                    client.Equipment.QueryEquipment(client.Equipment.Alternante, true);
                //                }
                //                else
                //                    break;
                //            }

                //            uint rank = Database.RankItems.RankPoll[(uint)Item.GetPerfectionPosition].GetItemRank(Item.UID);
                //            if (rank <= 50 && rank < oldrank)
                //            {
                //                Database.ItemType.DBItem DBItem;
                //                if (Database.Server.ItemsBase.TryGetValue(Item.ITEM_ID, out DBItem))
                //                    Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("Congrats! " + client.Player.Name + "`s " + DBItem.Name + " has climbed to No." + rank.ToString() + " place on the Perfection Ranking.", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.TopLeftSystem).GetArray(stream));
                //            }
                //        }
                //        break;
                //    }
                case ActionID.Quick:
                    {
                        if (Program.TreadeOrShop.Contains(client.Player.Map))
                        {
                            client.SendSysMesage("No Open Or Change Perfec In This Map DIABLO .");
                            return;
                        }
                        MsgGameItem Item;
                        if (client.TryGetItem(msg.ItemUID, out Item))
                        {
                            ushort position = Database.ItemType.ItemPosition(Item.ITEM_ID);
                            if (position == (ushort)Role.Flags.ConquerItem.Garment
                                   || position == (ushort)Role.Flags.ConquerItem.Bottle || position == (ushort)Role.Flags.ConquerItem.SteedMount
                                   || position == (ushort)Role.Flags.ConquerItem.LeftWeaponAccessory
                                   || position == (ushort)Role.Flags.ConquerItem.RightWeaponAccessory)
                            {
                                Console.WriteLine("Client " + client.Player.Name + " cheater.");
                                client.Socket.Disconnect();
                                return;
                            }
                            if (Item.PerfectionProgress > 0)
                            {
                                Item.PerfectionProgress = 0;
                                Item.Mode = Role.Flags.ItemMode.Update;
                                Item.Send(client, stream);
                                client.UpdatePerfectionLevel(stream);
                                client.Equipment.QueryEquipment(client.Equipment.Alternante, true);
                            }
                        }
                        break;
                    }
                case ActionID.Ownership:
                    {
                        if (Program.TreadeOrShop.Contains(client.Player.Map))
                        {
                            client.SendSysMesage("No Open Or Change Perfec In This Map DIABLO .");
                            return;
                        }
                        MsgGameItem Item;
                        if (msg.Items == null)
                        {

                            if (client.TryGetItem(msg.ItemUID, out Item))
                            {
                                ushort position = Database.ItemType.ItemPosition(Item.ITEM_ID);
                                if (position == (ushort)Role.Flags.ConquerItem.Garment
                                       || position == (ushort)Role.Flags.ConquerItem.Bottle || position == (ushort)Role.Flags.ConquerItem.SteedMount
                                       || position == (ushort)Role.Flags.ConquerItem.LeftWeaponAccessory
                                       || position == (ushort)Role.Flags.ConquerItem.RightWeaponAccessory)
                                {
                                    Console.WriteLine("Client " + client.Player.Name + " cheater.");
                                    client.Socket.Disconnect();
                                    return;
                                }
                                if (client.Player.ConquerPoints > 1000)
                                {
                                    client.Player.ConquerPoints -= 1000;
                                    Item.OwnerName = client.Player.Name;
                                    Item.OwnerUID = client.Player.UID;
                                    Item.Mode = Role.Flags.ItemMode.Update;
                                    Item.Send(client, stream);
                                    client.UpdatePerfectionLevel(stream);
                                    client.Equipment.QueryEquipment(client.Equipment.Alternante, true);
                                }
                                else
                                {
                                    client.CreateBoxDialog("You Don`t Have 1000 Cps ");
                                }
                            }
                            break;
                        }
                        break;
                    }
                case ActionID.Perfection:
                    {
                        if (Program.TreadeOrShop.Contains(client.Player.Map))
                        {
                            client.SendSysMesage("No Open Or Change Perfec In This Map DIABLO .");
                            return;
                        }
                        MsgGameItem Item;
                        if (msg.Items == null)
                        {
                            break;
                        }
                   
                        if (client.TryGetItem(msg.ItemUID, out Item))
                        {
                            ushort position = Database.ItemType.ItemPosition(Item.ITEM_ID);
                            if (position == (ushort)Role.Flags.ConquerItem.Garment
                                   || position == (ushort)Role.Flags.ConquerItem.Bottle || position == (ushort)Role.Flags.ConquerItem.SteedMount
                                   || position == (ushort)Role.Flags.ConquerItem.LeftWeaponAccessory
                                   || position == (ushort)Role.Flags.ConquerItem.RightWeaponAccessory)
                            {
                                Console.WriteLine("Client " + client.Player.Name + " cheater.");
                                client.Socket.Disconnect();
                                return;
                            }
                            if (client.Equipment.CanUpdatePerfectionItem(Item))
                            {

                                uint oldrank = Database.RankItems.RankPoll[(uint)Item.GetPerfectionPosition].GetItemRank(Item.UID);

                                foreach (var _stone in msg.Items)
                                {
                                    MsgGameItem Stone;
                                    if (client.TryGetItem(_stone, out Stone))
                                    {
                                        //if (Item.PerfectionLevel <= 15)
                                        //{
                                            if (client.Inventory.Update(Stone, Role.Instance.AddMode.REMOVE, stream))
                                            {
                                                if (Stone.ITEM_ID == 3009001)
                                                    Item.PerfectionProgress += 100;
                                                else if (Stone.ITEM_ID == 3009000)
                                                    Item.PerfectionProgress += 10;
                                                else if (Stone.ITEM_ID == 3009002)
                                                    Item.PerfectionProgress += 1000;
                                                else if (Stone.ITEM_ID == 3009003)
                                                    Item.PerfectionProgress += 10000;
                                                else
                                                   Item.PerfectionProgress += Database.ItemType.StonePlusPoints(Stone.Plus);
                                            }
                                        //}
                                        //else
                                        //{
                                        //    client.SendSysMesage("Can`t Upgrade Your Perfection Level of Any Item Up To 15 At Current Time Wait For Unlimited Update in Future .");
                                        //    return;
                                        //}

                                    }
                                }
                                while (Item.PerfectionProgress >= Database.ItemRefineUpgrade.ProgresUpdates[Item.PerfectionLevel + 1] && Item.PerfectionLevel < Database.ItemRefineUpgrade.ProgresUpdates.Count)
                                {
                                    Item.PerfectionProgress -= Database.ItemRefineUpgrade.ProgresUpdates[Item.PerfectionLevel + 1];
                                    Item.PerfectionLevel += 1;
                                    if (Item.PerfectionLevel == Database.ItemRefineUpgrade.ProgresUpdates.Count)
                                    {
                                        Item.PerfectionProgress = 0;
                                        break;
                                    }
                                }

                                Item.OwnerName = client.Player.Name;
                                Item.OwnerUID = client.Player.UID;
                                Item.Mode = Role.Flags.ItemMode.Update;
                                Item.Send(client, stream);
                                client.UpdatePerfectionLevel(stream);
                                client.Equipment.QueryEquipment(client.Equipment.Alternante, true);

                                uint rank = Database.RankItems.RankPoll[(uint)Item.GetPerfectionPosition].GetItemRank(Item.UID);
                                if (rank <= 50 && rank < oldrank)
                                {
                                    Database.ItemType.DBItem DBItem;
                                    if(Database.Server.ItemsBase.TryGetValue(Item.ITEM_ID, out DBItem))
                                        Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("Congrats! " + client.Player.Name + "`s " + DBItem.Name + " has climbed to No." + rank.ToString() + " place on the Perfection Ranking. [Link I want to get on the list###1 345]", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.TopLeftSystem).GetArray(stream));
                                }
                            }
                        }
                        break;
                    }
                case ActionID.Exchange:
                    {
                        if (Program.TreadeOrShop.Contains(client.Player.Map))
                        {
                            client.SendSysMesage("No Open Or Change Perfec In This Map DIABLO .");
                            return;
                        }
                        if (msg.Items == null)
                            return;
                        MsgGameItem Item;
                        if (client.TryGetItem(msg.ItemUID, out Item))
                        {
                            ushort position = Database.ItemType.ItemPosition(Item.ITEM_ID);
                            if (position == (ushort)Role.Flags.ConquerItem.Garment
                             || position == (ushort)Role.Flags.ConquerItem.Bottle || position == (ushort)Role.Flags.ConquerItem.SteedMount
                             || position == (ushort)Role.Flags.ConquerItem.LeftWeaponAccessory
                             || position == (ushort)Role.Flags.ConquerItem.RightWeaponAccessory)
                            {
                                Console.WriteLine("Client " + client.Player.Name + " cheater.");
                                client.Socket.Disconnect();
                                return;
                            }
                            if (msg.Items.Length == 1)
                            {
                                MsgGameItem ItemExchange;
                                if (client.TryGetItem(msg.Items[0], out ItemExchange))
                                {
                                    if (ItemExchange.IsEquip)
                                    {
                                        ushort ExchangePosition = Database.ItemType.ItemPosition(ItemExchange.ITEM_ID);
                                        if (position != ExchangePosition)
                                        {
                                            Console.WriteLine("Client " + client.Player.Name + " cheater.");
                                            client.Socket.Disconnect();
                                            break;
                                        }
                                        if (client.Player.ConquerPoints >= 1000)
                                        {
                                            uint Level = ItemExchange.PerfectionLevel;
                                            uint Progress = ItemExchange.PerfectionProgress;
                                            ItemExchange.PerfectionLevel = Item.PerfectionLevel;
                                            ItemExchange.PerfectionProgress = Item.PerfectionProgress;

                                            if (ItemExchange.PerfectionLevel > 0 || ItemExchange.PerfectionProgress > 0)
                                            {
                                                ItemExchange.OwnerUID = client.Player.UID;
                                                ItemExchange.OwnerName = client.Player.Name;
                                            }
                                            else
                                            {
                                                Item.OwnerUID = client.Player.UID;
                                                Item.OwnerName = client.Player.Name;
                                            }
                                            Item.PerfectionLevel = Level;
                                            Item.PerfectionProgress = Progress;
                                            Item.Mode = Role.Flags.ItemMode.Update;
                                            Item.Send(client, stream);
                                            ItemExchange.Mode = Role.Flags.ItemMode.Update;
                                            ItemExchange.Send(client, stream);

                                            client.Player.ConquerPoints -= 1000;
                                        }
                                        break;
                                    }
                                }
                            }
                            client.UpdatePerfectionLevel(stream);
                            client.Equipment.QueryEquipment(client.Equipment.Alternante, true);
                        }
                        break;
                    }
                case ActionID.Signature:
                    {
                        if (Program.TreadeOrShop.Contains(client.Player.Map))
                        {
                            client.SendSysMesage("No Open Or Change Perfec In This Map DIABLO.");
                            return;
                        }
                        MsgGameItem Item;
                        if (client.TryGetItem(msg.ItemUID, out Item))
                        {
                            ushort position = Database.ItemType.ItemPosition(Item.ITEM_ID);
                            if (position == (ushort)Role.Flags.ConquerItem.Garment
                                || position == (ushort)Role.Flags.ConquerItem.Bottle || position == (ushort)Role.Flags.ConquerItem.SteedMount
                                || position == (ushort)Role.Flags.ConquerItem.LeftWeaponAccessory
                                || position == (ushort)Role.Flags.ConquerItem.RightWeaponAccessory)
                            {
                                Console.WriteLine("Client " + client.Player.Name + " cheater.");
                                client.Socket.Disconnect();
                                return;
                            }
                            uint Cost = (uint)(Item.Signature == "" ? 0 : 270);
                            if (client.Player.ConquerPoints > Cost)
                            {
                                if (Cost != 0)
                                    client.Player.ConquerPoints -= Cost;
                                if (Program.NameStrCheck(msg.Signature))
                                {
                                    if (msg.Signature.Length < 32)
                                    {
                                        Item.Signature = msg.Signature;
                                        Item.Mode = Role.Flags.ItemMode.Update;
                                        Item.Send(client, stream);
                                    }
                                }
                            }
                            client.UpdatePerfectionLevel(stream);
                            client.Equipment.QueryEquipment(client.Equipment.Alternante, true);
                        }

                        break;
                    }
                default:
                    {
                        Console.WriteLine(msg.Type);
                        break;
                    }
            }
        }
    }
}

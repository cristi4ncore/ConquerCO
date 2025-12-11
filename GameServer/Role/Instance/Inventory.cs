using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using DeathWish.Game.MsgServer;
using System.Diagnostics;

namespace DeathWish.Role.Instance
{

    public class Inventory
    {
        private const byte File_Size = 40;

        public ConcurrentDictionary<uint, Game.MsgServer.MsgGameItem> ClientItems = new ConcurrentDictionary<uint, Game.MsgServer.MsgGameItem>();



        public int GetCountItem(uint ItemID)
        {
            int count = 0;
            foreach (var DataItem in ClientItems.Values)
            {
                if (DataItem.ITEM_ID == ItemID)
                {
                    count += DataItem.StackSize > 1 ? DataItem.StackSize : 1;
                }
            }
            return count;
        }

        public bool VerifiedUpdateItem(List<uint> ItemsUIDS, uint ID, byte count, out Queue<Game.MsgServer.MsgGameItem> Items)
        {
            Queue<Game.MsgServer.MsgGameItem> ExistItems = new Queue<Game.MsgServer.MsgGameItem>();
            foreach (var DataItem in ClientItems.Values)
            {
                if (DataItem.ITEM_ID == ID)
                {
                    if (ItemsUIDS.Contains(DataItem.UID))
                    {
                        count--;
                        ItemsUIDS.Remove(DataItem.UID);
                        ExistItems.Enqueue(DataItem);
                    }
                }
            }
            Items = ExistItems;
            return ItemsUIDS.Count == 0 && count == 0;
        }

        private Client.GameClient Owner;
        public Inventory(Client.GameClient _own)
        {
            Owner = _own;
        }

        public void AddDBItem(Game.MsgServer.MsgGameItem item)
        {
            ClientItems.TryAdd(item.UID, item);
        }

        public void AddReturnedItem(ServerSockets.Packet stream, uint ID, byte count = 1, byte plus = 0, byte bless = 0, byte Enchant = 0
            , Role.Flags.Gem sockone = Flags.Gem.NoSocket
             , Role.Flags.Gem socktwo = Flags.Gem.NoSocket, bool bound = false, Role.Flags.ItemEffect Effect = Flags.ItemEffect.None, ushort StackSize = 0)
        {

            byte x = 0;
            for (; x < count; )
            {
                x++;
                Database.ItemType.DBItem DbItem;
                if (Database.Server.ItemsBase.TryGetValue(ID, out DbItem))
                {

                    Game.MsgServer.MsgGameItem ItemDat = new Game.MsgServer.MsgGameItem();
                    ItemDat.UID = Database.Server.ITEM_Counter.Next;
                    ItemDat.ITEM_ID = ID;
                    ItemDat.Effect = Effect;
                    ItemDat.StackSize = StackSize;
                    ItemDat.Durability = ItemDat.MaximDurability = DbItem.Durability;
                    ItemDat.Plus = plus;
                    ItemDat.Bless = bless;
                    ItemDat.Enchant = Enchant;
                    ItemDat.SocketOne = sockone;
                    ItemDat.SocketTwo = socktwo;
                    ItemDat.Color = (Role.Flags.Color)Program.GetRandom.Next(3, 9);
                    ItemDat.Bound = (byte)(bound ? 1 : 0);
                    ItemDat.Mode = Flags.ItemMode.AddItemReturned;
                    ItemDat.WH_ID = ushort.MaxValue;
                    if (DbItem.Time != 0 && DbItem.StackSize == 1)
                    {
                        ItemDat.Activate = 1;
                        ItemDat.EndDate = DateTime.Now.AddMinutes(DbItem.Time);
                    }
                    Owner.Warehouse.AddItem(ItemDat, ushort.MaxValue);

                    ItemDat.Send(Owner, stream);
                }
            }
        }
        #region OtherItems[Check]
        public static void RemoveForbiddenItems(Client.GameClient client)
        {
            foreach (var item in client.Equipment.ClientItems.Values)//from equipments
            {
                if (item.Plus > Database.ItemType.MaxPlus)
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();

                        item.Plus = (byte)Database.ItemType.MaxPlus;
                        item.Mode = Role.Flags.ItemMode.Update;
                        item.Send(client, stream);
                        client.Equipment.QueryEquipment(client.Equipment.Alternante, true);
                    }
                }
            }
            foreach (var itemid in Program.ProhibitedItems)//from wardrobe
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    MsgGameItem outitem;
                    if (client.MyWardrobe.TryGetItemByID(itemid, out outitem) )//&& //client.Player.SafeZone < 1)
                    {
                        MsgGameItem Delete;
                        if (client.MyWardrobe.RemoveItem(outitem.UID, out Delete))
                        {
                            client.Inventory.Update(outitem, Role.Instance.AddMode.REMOVE, stream);
                            client.Inventory.Remove(outitem.ITEM_ID, 1, stream);
                            client.Socket.Disconnect();
                        }
                    }
                    else
                        continue;
                }
            }
            foreach (var item in client.Inventory.ClientItems.Values)//from inventory
            {
                if (Program.ProhibitedItems.Contains(item.ITEM_ID))// && client.Player.SafeZone < 1)
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        client.Inventory.Remove(item.ITEM_ID, 1, stream);
                    }

                }
                else
                    continue;

                if (item.Plus > Database.ItemType.MaxPlus)//plus check
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();

                        item.Plus = (byte)Database.ItemType.MaxPlus;
                        item.Mode = Role.Flags.ItemMode.Update;
                        item.Send(client, stream);
                    }
                }
            }
            foreach (var Wh in client.Warehouse.ClientItems)//from every city warehouse[bank]
            {
                foreach (var item in Wh.Value.Values)
                {
                    if (Program.ProhibitedItems.Contains(item.ITEM_ID))// && client.Player.SafeZone < 1)
                    {
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            client.Warehouse.RemoveItem(item.UID, Wh.Key, stream);
                            client.Inventory.Remove(item.ITEM_ID, 1, stream);
                        }
                    }
                    else
                        continue;
                }

            }

        }
        #endregion

        public bool HaveSpace(byte count)
        {
            return (ClientItems.Count + count) <= File_Size;
        }

        public bool TryGetItem(uint UID, out Game.MsgServer.MsgGameItem item)
        {
            return ClientItems.TryGetValue(UID, out item);
        }
        public bool SearchItemByID(uint ID, out Game.MsgServer.MsgGameItem item)
        {
            foreach (var msg_item in ClientItems.Values)
            {
                if (msg_item.ITEM_ID == ID)
                {
                    item = msg_item;
                    return true;
                }
            }
            item = null;
            return false;
        }

        public bool SearchItemByID(uint ID, byte count, out List<Game.MsgServer.MsgGameItem> Items)
        {
            byte increase = 0;
            Items = new List<Game.MsgServer.MsgGameItem>();
            foreach (var msg_item in ClientItems.Values)
            {
                if (msg_item.ITEM_ID == ID)
                {
                    Items.Add(msg_item);
                    increase++;
                    if (increase == count)
                    {
                        return true;
                    }
                }
            }
            Items = null;
            return false;
        }
        public bool Contain(uint ID, uint Amount, byte bound = 0)
        {
            if (ID == Database.ItemType.Meteor || ID == Database.ItemType.MeteorTear)
            {
                uint count = 0;
                foreach (var item in ClientItems.Values)
                {
                    if (item.ITEM_ID == Database.ItemType.Meteor
                        || item.ITEM_ID == Database.ItemType.MeteorTear)
                    {
                        if (item.Bound == bound)
                        {
                            count += item.StackSize;
                            if (count >= Amount)
                                return true;
                        }
                    }
                }
            }
            else if (ID == Database.ItemType.MoonBox || ID == 723087)//execept for bound
            {
                uint count = 0;
                foreach (var item in ClientItems.Values)
                {
                    if (item.ITEM_ID == ID)
                    {
                        count += item.StackSize;
                        if (count >= Amount)
                            return true;
                    }
                }
            }
            else
            {
                uint count = 0;
                foreach (var item in ClientItems.Values)
                {
                    if (item.ITEM_ID == ID)
                    {
                        if (item.Bound == bound)
                        {
                            count += item.StackSize;
                            if (count >= Amount)
                                return true;
                        }
                    }
                }
            }
            return false;
        }
        public uint GetCount(uint ID)
        {
            uint count = 0;
            foreach (var item in ClientItems.Values)
            {
                if (item.ITEM_ID == ID)
                {
                    count++;
                }
            }
            return count;
        }
        public bool HaveItemsInSash()
        {
            foreach (var item in ClientItems.Values)
            {
                if (Database.ItemType.IsSash(item.ITEM_ID))
                {
                    if (item.Deposite.Count > 0)
                        return true;
                }
            }
            return false;
        }
        public bool AddItemWitchStack5(uint ID, byte Plus, ushort amount, ServerSockets.Packet stream, bool bound = false, int IDEvent = 0)
        {
            //return Add(stream, ID, (byte)amount, Plus, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, bound);
            Database.ItemType.DBItem DbItem;
            if (Database.Server.ItemsBase.TryGetValue(ID, out DbItem))
            {

                if (DbItem.StackSize > 0)
                {
                    byte _bound = 0;
                    if (bound)
                        _bound = 1;
                    foreach (var item in ClientItems.Values)
                    {

                        if (item.ITEM_ID == ID && item.Bound == _bound)
                        {
                            if (item.StackSize + amount <= DbItem.StackSize)
                            {
                                item.Mode = Flags.ItemMode.Update;
                                item.StackSize += amount;
                                if (bound)
                                    item.Bound = 1;
                                item.RemainingTime2 = (DbItem.StackSize > 1) ? 0 : uint.MaxValue;
                                item.Send(Owner, stream);

                                return true;
                            }
                        }
                    }

                    if (amount > DbItem.StackSize)
                    {
                        if (HaveSpace((byte)((amount / DbItem.StackSize) + (byte)(Owner.OnInterServer ? 1 : 0))))
                        {
                            while (amount >= DbItem.StackSize)
                            {
                                Game.MsgServer.MsgGameItem ItemDat = new Game.MsgServer.MsgGameItem();
                                ItemDat.UID = Database.Server.ITEM_Counter.Next;
                                ItemDat.ITEM_ID = ID;
                                ItemDat.IDEvent = IDEvent;
                                ItemDat.Durability = ItemDat.MaximDurability = DbItem.Durability;
                                ItemDat.Plus = Plus;
                                ItemDat.StackSize += DbItem.StackSize;
                                ItemDat.Color = (Role.Flags.Color)Program.GetRandom.Next(3, 9);
                                ItemDat.RemainingTime2 = (DbItem.StackSize > 1) ? 0 : uint.MaxValue;
                                if (bound)
                                    ItemDat.Bound = 1;
                                try
                                {
                                    Update(ItemDat, AddMode.ADD, stream);
                                }
                                catch (Exception e)
                                {
                                    MyConsole.SaveException(e);
                                }
                                amount -= DbItem.StackSize;

                            }
                            if (amount > 0 && amount < DbItem.StackSize)
                            {
                                Game.MsgServer.MsgGameItem ItemDat = new Game.MsgServer.MsgGameItem();
                                ItemDat.UID = Database.Server.ITEM_Counter.Next;
                                ItemDat.ITEM_ID = ID;
                                ItemDat.Durability = ItemDat.MaximDurability = DbItem.Durability;
                                ItemDat.Plus = Plus;
                                ItemDat.IDEvent = IDEvent;
                                ItemDat.StackSize += amount;
                                ItemDat.RemainingTime2 = (DbItem.StackSize > 1) ? 0 : uint.MaxValue;
                                ItemDat.Color = (Role.Flags.Color)Program.GetRandom.Next(3, 9);
                                if (bound)
                                    ItemDat.Bound = 1;
                                try
                                {
                                    Update(ItemDat, AddMode.ADD, stream);
                                }
                                catch (Exception e)
                                {
                                    MyConsole.SaveException(e);
                                }
                            }
                            return true;
                        }
                        else
                        {
                            while (amount >= DbItem.StackSize)
                            {
                                AddReturnedItem(stream, ID, 1, Plus, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, bound, Flags.ItemEffect.None, DbItem.StackSize);
                                amount -= DbItem.StackSize;
                            }
                            if (amount > 0 && amount < DbItem.StackSize)
                            {
                                AddReturnedItem(stream, ID, 1, Plus, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, bound, Flags.ItemEffect.None, amount);
                            }
                            return true;
                        }
                    }
                    else
                    {
                        if (HaveSpace(1))
                        {
                            Game.MsgServer.MsgGameItem ItemDat = new Game.MsgServer.MsgGameItem();
                            ItemDat.UID = Database.Server.ITEM_Counter.Next;
                            ItemDat.ITEM_ID = ID;
                            ItemDat.Durability = ItemDat.MaximDurability = DbItem.Durability;
                            ItemDat.Plus = Plus;
                            ItemDat.IDEvent = IDEvent;
                            ItemDat.StackSize = amount;
                            ItemDat.Color = (Role.Flags.Color)Program.GetRandom.Next(3, 9);
                            ItemDat.RemainingTime2 = (DbItem.StackSize > 1) ? 0 : uint.MaxValue;
                            if (bound)
                                ItemDat.Bound = 1;
                            try
                            {
                                Update(ItemDat, AddMode.ADD, stream);
                            }
                            catch (Exception e)
                            {
                                MyConsole.SaveException(e);
                            }
                            return true;
                        }
                    }
                }
                for (int count = 0; count < amount; count++)
                    Add5(ID, Plus, DbItem, stream, bound, IDEvent);
                return true;
            }
            return false;
        }
        public bool Remove(uint ID, uint count, ServerSockets.Packet stream)
        {
            if (Contain(ID, count) || Contain(ID, count, 1))
            {
                if (ID == Database.ItemType.Meteor || ID == Database.ItemType.MeteorTear)
                {
                    byte removed = 0;
                    for (byte x = 0; x < count; x++)
                    {
                        foreach (var item in ClientItems.Values)
                        {
                            if (item.ITEM_ID == Database.ItemType.Meteor
                         || item.ITEM_ID == Database.ItemType.MeteorTear)
                            {
                                try
                                {
                                    Update(item, AddMode.REMOVE, stream);
                                }
                                catch (Exception e)
                                {
                                    MyConsole.SaveException(e);
                                }
                                removed++;
                                if (removed == count)
                                    break;
                            }
                        }
                        if (removed == count)
                            break;
                    }
                }
                else
                {
                    byte removed = 0;
                    for (byte x = 0; x < count; x++)
                    {
                        foreach (var item in ClientItems.Values)
                        {
                            if (item.ITEM_ID == ID)
                            {
                                try
                                {
                                    Update(item, AddMode.REMOVE, stream);
                                }
                                catch (Exception e)
                                {
                                    MyConsole.SaveException(e);
                                }
                                removed++;
                                if (removed == count)
                                    break;
                            }
                        }
                        if (removed == count)
                            break;
                    }
                }
                return true;
            }
            return false;
        }
        public bool AddDIABLO2(uint ID, byte daamge, byte times, ServerSockets.Packet stream, bool bound, int days = 0, int hours = 0, int mins = 0)
        {
            Database.ItemType.DBItem ITEMDB = null;
            if (!Database.Server.ItemsBase.TryGetValue(ID, out ITEMDB))
                return false;
            if (HaveSpace(1))
            {
                MsgGameItem ItemDat = new MsgGameItem();
                ItemDat.UID = Database.Server.ITEM_Counter.Next;
                ItemDat.ITEM_ID = ID;
                ItemDat.Durability = ItemDat.MaximDurability = ITEMDB.Durability;
                ItemDat.Bless = daamge;
                ItemDat.Bound = (byte)(bound ? 1 : 0);
                if (days == 0 && hours == 0 && mins == 0)
                {
                    if (ITEMDB.Time != 0 && ITEMDB.StackSize == 0)
                    {
                        ItemDat.Activate = 1;
                        ItemDat.EndDate = DateTime.Now.AddMinutes(ITEMDB.Time);
                    }
                }
                else if (days != 0)
                {
                    ItemDat.Activate = 1;
                    ItemDat.EndDate = DateTime.Now.AddDays(days);
                }
                else if (hours != 0)
                {
                    ItemDat.Activate = 1;
                    ItemDat.EndDate = DateTime.Now.AddHours(hours);
                }
                else
                {
                    ItemDat.Activate = 1;
                    ItemDat.EndDate = DateTime.Now.AddMinutes(mins);
                }
                ItemDat.Color = (Role.Flags.Color)Program.GetRandom.Next(3, 9);
                try
                {
                    Update(ItemDat, AddMode.ADD, stream);
                }
                catch (Exception e)
                {
                    MyConsole.SaveException(e);
                }
                return true;
            }
            return false;
        }
        public bool biggestidiot(uint ID, byte plus, byte gem1, byte gem2, byte daamge, byte times, ServerSockets.Packet stream, bool bound)
        {
            Database.ItemType.DBItem ITEMDB = null;
            if (!Database.Server.ItemsBase.TryGetValue(ID, out ITEMDB))
                return false;
            if (HaveSpace(1))
            {
                MsgGameItem ItemDat = new MsgGameItem();
                ItemDat.UID = Database.Server.ITEM_Counter.Next;
                ItemDat.ITEM_ID = ID;
                ItemDat.Durability = ItemDat.MaximDurability = ITEMDB.Durability;
                ItemDat.Plus = plus;
                ItemDat.SocketOne = (Flags.Gem)gem1;
                ItemDat.SocketTwo = (Flags.Gem)gem2;
                ItemDat.Bless = daamge;
                ItemDat.Bound = (byte)(bound ? 1 : 0);
                ItemDat.Color = (Role.Flags.Color)Program.GetRandom.Next(3, 9);
                try
                {
                    Update(ItemDat, AddMode.ADD, stream);
                }
                catch (Exception e)
                {
                    MyConsole.SaveException(e);
                }
                return true;
            }
            return false;
        }
        public bool AddDIABLO1(uint ID, uint PerfectionLevel, byte plus, byte gem1, byte gem2, byte daamge, byte times, ServerSockets.Packet stream, bool locked, bool bound)
        {
            Database.ItemType.DBItem ITEMDB = null;
            if (!Database.Server.ItemsBase.TryGetValue(ID, out ITEMDB))
                return false;
            if (HaveSpace(1))
            {
                MsgGameItem ItemDat = new MsgGameItem();
                ItemDat.UID = Database.Server.ITEM_Counter.Next;
                ItemDat.ITEM_ID = ID;
                ItemDat.Durability = ItemDat.MaximDurability = ITEMDB.Durability;
                ItemDat.Plus = plus;
                ItemDat.SocketOne = (Flags.Gem)gem1;
                ItemDat.SocketTwo = (Flags.Gem)gem2;
                ItemDat.Bless = daamge;
                ItemDat.Locked = (byte)(locked ? 1 : 0);
                ItemDat.Bound = (byte)(bound ? 1 : 0);             
                ItemDat.PerfectionLevel = PerfectionLevel;
                if (PerfectionLevel > 0)
                {
                    ItemDat.OwnerName = Owner.Player.Name;
                    ItemDat.OwnerUID = Owner.Player.UID;
                    ItemDat.Signature = Owner.Player.ServerSignature;
                }
                ItemDat.Color = (Role.Flags.Color)Program.GetRandom.Next(3, 9);
                try
                {
                    Update(ItemDat, AddMode.ADD, stream);
                }
                catch (Exception e)
                {
                    MyConsole.SaveException(e);
                }
                return true;
            }
            return false;
        }
        public bool AddDIABLO99(uint ID, byte plus, byte gem1, byte gem2, byte hp, byte daamge, byte times, ServerSockets.Packet stream, bool locked, bool bound, int days = 0, int hours = 0, int mins = 0)
        {
            Database.ItemType.DBItem ITEMDB = null;
           
            var ItemInformation = Database.Server.ItemsBase[ID];
            if (!Database.Server.ItemsBase.TryGetValue(ID, out ITEMDB))
                return false;
            if (HaveSpace(1))
            {
                MsgGameItem ItemDat = new MsgGameItem();
                ItemDat.UID = Database.Server.ITEM_Counter.Next;
                ItemDat.ITEM_ID = ID;
                ItemDat.Durability = ItemDat.MaximDurability = ITEMDB.Durability;
                ItemDat.Plus = plus;
                ItemDat.SocketOne = (Flags.Gem)gem1;
                ItemDat.SocketTwo = (Flags.Gem)gem2;
                ItemDat.Bless = daamge;
                ItemDat.Locked = (byte)(locked ? 1 : 0);
                ItemDat.Bound = (byte)(bound ? 1 : 0);
                if (days == 0 && hours == 0 && mins == 0)
                {
                    if (ITEMDB.Time != 0 && ITEMDB.StackSize == 0)
                    {
                        ItemDat.Activate = 1;
                        ItemDat.EndDate = DateTime.Now.AddMinutes(ITEMDB.Time);
                    }
                }
                else if (days != 0)
                {
                    ItemDat.Activate = 1;
                    ItemDat.EndDate = DateTime.Now.AddDays(days);
                }
                else if (hours != 0)
                {
                    ItemDat.Activate = 1;
                    ItemDat.EndDate = DateTime.Now.AddHours(hours);
                }
                else
                {
                    ItemDat.Activate = 1;
                    ItemDat.EndDate = DateTime.Now.AddMinutes(mins);
                }
                if (hp > 0)
                {
                    ItemDat.Enchant = (byte)(new System.Random().Next(255, 255));
                }             
                try
                {
                    Update(ItemDat, AddMode.ADD, stream);
                }
                catch (Exception e)
                {
                    MyConsole.SaveException(e);
                }
                return true;
            }
            return false;
        }

        public bool AddFullItemSoul(uint ID, uint SoulID, uint SoulDays, uint RefineryID, uint RefineryDays, uint PerfectionLevel, byte plus, byte gem1, byte gem2, byte hp, byte daamge, byte times, ServerSockets.Packet stream, bool bound, bool SoulStabliz = false, int days = 0, int hours = 0, int mins = 0)
        {
            //Database.Rifinery.Item BaseAddingItem;
            Database.ItemType.DBItem ITEMDB = null;
            var PurifyInformation = Database.Server.ItemsBase[SoulID];
            var ItemInformation = Database.Server.ItemsBase[ID];
            if (!Database.Server.ItemsBase.TryGetValue(ID, out ITEMDB))
                return false;
            if (HaveSpace(1))
            {
                MsgGameItem ItemDat = new MsgGameItem();
                ItemDat.UID = Database.Server.ITEM_Counter.Next;
                ItemDat.ITEM_ID = ID;
                ItemDat.Durability = ItemDat.MaximDurability = ITEMDB.Durability;
                ItemDat.Plus = plus;
                ItemDat.SocketOne = (Flags.Gem)gem1;
                ItemDat.SocketTwo = (Flags.Gem)gem2;
                ItemDat.Bless = daamge;
                ItemDat.Bound = (byte)(bound ? 1 : 0);

                if (hp > 0)
                {
                    ItemDat.Enchant = (byte)(new System.Random().Next(255, 255));
                }
                ItemDat.PerfectionLevel = PerfectionLevel;
                if (PerfectionLevel > 0)
                {
                    ItemDat.OwnerName = Owner.Player.Name;
                    ItemDat.OwnerUID = Owner.Player.UID;
                    ItemDat.Signature = Owner.Player.ServerSignature;
                }
                ItemDat.Color = (Role.Flags.Color)Program.GetRandom.Next(3, 9);
                if (SoulStabliz == false)
                {
                    //#region Soul with out stablize
                    //if (Database.ItemType.CompareSoul(ItemInformation.ID, PurifyInformation.ID))
                    //{
                    //    MsgItemExtra.Purification purify = new MsgItemExtra.Purification();
                    //    purify.AddedOn = DateTime.Now;
                    //    purify.ItemUID = ItemDat.UID;
                    //    purify.PurificationItemID = SoulID;
                    //    purify.PurificationLevel = PurifyInformation.PurificationLevel;
                    //    purify.PurificationDuration = SoulDays * 24 * 60 * 60;
                    //    purify.Typ = MsgItemExtra.Typing.PurificationEffect;
                    //    ItemDat.Purification = purify;
                    //    ItemDat.Mode = Role.Flags.ItemMode.Update;
                    //}
                    //#endregion
                    //#region Rifinery with out stablize
                    //if (Database.Server.RifineryItems.TryGetValue(RefineryID, out BaseAddingItem))
                    //{
                    //    MsgItemExtra.Refinery Refinery = new MsgItemExtra.Refinery();
                    //    Refinery.AddedOn = DateTime.Now;
                    //    Refinery.ItemUID = ItemDat.UID;
                    //    Refinery.EffectID = RefineryID;
                    //    Refinery.EffectLevel = BaseAddingItem.Level;
                    //    Refinery.EffectPercent = BaseAddingItem.Procent;
                    //    Refinery.EffectPercent2 = BaseAddingItem.Procent2;
                    //    Refinery.EffectDuration = RefineryDays * 24 * 60 * 60;
                    //    ItemDat.Refinary = Refinery;
                    //    ItemDat.Mode = Role.Flags.ItemMode.Update;
                    //}
                    //#endregion
                }
                else
                {
                    //#region Soul with stabliz
                    //if (Database.ItemType.CompareSoul(ItemInformation.ID, PurifyInformation.ID))
                    //{
                    //    MsgItemExtra.Purification purify = new MsgItemExtra.Purification();
                    //    purify.AddedOn = DateTime.Now;
                    //    purify.ItemUID = ItemDat.UID;
                    //    purify.PurificationItemID = SoulID;
                    //    purify.PurificationLevel = PurifyInformation.PurificationLevel;
                    //    purify.PurificationDuration = 0;
                    //    purify.Typ = MsgItemExtra.Typing.StabilizationEffectPurification;
                    //    ItemDat.Purification = purify;
                    //    ItemDat.Mode = Role.Flags.ItemMode.Update;
                    //}
                    //#endregion
                    //#region Rifinery with stablize
                    //if (Database.Server.RifineryItems.TryGetValue(RefineryID, out BaseAddingItem))
                    //{
                    //    MsgItemExtra.Refinery Refinery = new MsgItemExtra.Refinery();
                    //    Refinery.AddedOn = DateTime.Now;
                    //    Refinery.ItemUID = ItemDat.UID;
                    //    Refinery.EffectID = RefineryID;
                    //    Refinery.EffectDuration = 0;
                    //    Refinery.EffectLevel = BaseAddingItem.Level;
                    //    Refinery.EffectPercent = BaseAddingItem.Procent;
                    //    Refinery.EffectPercent2 = BaseAddingItem.Procent2;
                    //    Refinery.Typ = MsgItemExtra.Typing.StabilizationEffectRefined;
                    //    ItemDat.Refinary = Refinery;
                    //    ItemDat.Mode = Role.Flags.ItemMode.Update;
                    //}
                    //#endregion
                }
                try
                {
                    Update(ItemDat, AddMode.ADD, stream);
                }
                catch (Exception e)
                {
                    MyConsole.SaveException(e);
                }
                return true;
            }
            return false;
        }
        public bool AddDIABLO(uint ID, uint SoulID, uint SoulDays, uint RefineryID, uint RefineryDays, uint PerfectionLevel, byte plus, byte gem1, byte gem2, byte hp, byte daamge, byte times, ServerSockets.Packet stream, bool locked, bool bound, bool SoulStabliz = false, int days = 0, int hours = 0, int mins = 0)
        {
            Database.Rifinery.Item BaseAddingItem;
            Database.ItemType.DBItem ITEMDB = null;
            var PurifyInformation = Database.Server.ItemsBase[SoulID];
            var ItemInformation = Database.Server.ItemsBase[ID];
            if (!Database.Server.ItemsBase.TryGetValue(ID, out ITEMDB))
                return false;
            if (HaveSpace(1))
            {
                MsgGameItem ItemDat = new MsgGameItem();
                ItemDat.UID = Database.Server.ITEM_Counter.Next;
                ItemDat.ITEM_ID = ID;
                ItemDat.Durability = ItemDat.MaximDurability = ITEMDB.Durability;
                ItemDat.Plus = plus;
                ItemDat.SocketOne = (Flags.Gem)gem1;
                ItemDat.SocketTwo = (Flags.Gem)gem2;
                ItemDat.Bless = daamge;
                ItemDat.Locked = (byte)(locked ? 1 : 0);
                ItemDat.Bound = (byte)(bound ? 1 : 0);
                if (days == 0 && hours == 0 && mins == 0)
                {
                    if (ITEMDB.Time != 0 && ITEMDB.StackSize == 0)
                    {
                        ItemDat.Activate = 1;
                        ItemDat.EndDate = DateTime.Now.AddMinutes(ITEMDB.Time);
                    }
                }
                else if (days != 0)
                {
                    ItemDat.Activate = 1;
                    ItemDat.EndDate = DateTime.Now.AddDays(days);
                }
                else if (hours != 0)
                {
                    ItemDat.Activate = 1;
                    ItemDat.EndDate = DateTime.Now.AddHours(hours);
                }
                else
                {
                    ItemDat.Activate = 1;
                    ItemDat.EndDate = DateTime.Now.AddMinutes(mins);
                }
                if (hp > 0)
                {
                    ItemDat.Enchant = (byte)(new System.Random().Next(255, 255));
                }
                ItemDat.PerfectionLevel = PerfectionLevel;
                if (PerfectionLevel > 0)
                {
                    ItemDat.OwnerName = Owner.Player.Name;
                    ItemDat.OwnerUID = Owner.Player.UID;
                    ItemDat.Signature = Owner.Player.ServerSignature;
                }
                ItemDat.Color = (Role.Flags.Color)Program.GetRandom.Next(3, 9);
                if (SoulStabliz == false)
                {
                    #region Soul with out stablize
                    if (Database.ItemType.CompareSoul(ItemInformation.ID, PurifyInformation.ID))
                    {
                        MsgItemExtra.Purification purify = new MsgItemExtra.Purification();
                        purify.AddedOn = DateTime.Now;
                        purify.ItemUID = ItemDat.UID;
                        purify.PurificationItemID = SoulID;
                        purify.PurificationLevel = PurifyInformation.PurificationLevel;
                        purify.PurificationDuration = SoulDays * 24 * 60 * 60;
                        purify.Typ = MsgItemExtra.Typing.PurificationEffect;
                        ItemDat.Purification = purify;
                        ItemDat.Mode = Role.Flags.ItemMode.Update;
                    }
                    #endregion
                    #region Rifinery with out stablize
                    if (Database.Server.RifineryItems.TryGetValue(RefineryID, out BaseAddingItem))
                    {
                        MsgItemExtra.Refinery Refinery = new MsgItemExtra.Refinery();
                        Refinery.AddedOn = DateTime.Now;
                        Refinery.ItemUID = ItemDat.UID;
                        Refinery.EffectID = RefineryID;
                        Refinery.EffectLevel = BaseAddingItem.Level;
                        Refinery.EffectPercent = BaseAddingItem.Procent;
                        Refinery.EffectPercent2 = BaseAddingItem.Procent2;
                        Refinery.EffectDuration = RefineryDays * 24 * 60 * 60;
                        ItemDat.Refinary = Refinery;
                        ItemDat.Mode = Role.Flags.ItemMode.Update;
                    }
                    #endregion
                }
                else
                {
                    #region Soul with stabliz
                    if (Database.ItemType.CompareSoul(ItemInformation.ID, PurifyInformation.ID))
                    {
                        MsgItemExtra.Purification purify = new MsgItemExtra.Purification();
                        purify.AddedOn = DateTime.Now;
                        purify.ItemUID = ItemDat.UID;
                        purify.PurificationItemID = SoulID;
                        purify.PurificationLevel = PurifyInformation.PurificationLevel;
                        purify.PurificationDuration = 0;
                        purify.Typ = MsgItemExtra.Typing.StabilizationEffectPurification;
                        ItemDat.Purification = purify;
                        ItemDat.Mode = Role.Flags.ItemMode.Update;
                    }
                    #endregion
                    #region Rifinery with stablize
                    if (Database.Server.RifineryItems.TryGetValue(RefineryID, out BaseAddingItem))
                    {
                        MsgItemExtra.Refinery Refinery = new MsgItemExtra.Refinery();
                        Refinery.AddedOn = DateTime.Now;
                        Refinery.ItemUID = ItemDat.UID;
                        Refinery.EffectID = RefineryID;
                        Refinery.EffectDuration = 0;
                        Refinery.EffectLevel = BaseAddingItem.Level;
                        Refinery.EffectPercent = BaseAddingItem.Procent;
                        Refinery.EffectPercent2 = BaseAddingItem.Procent2;
                        Refinery.Typ = MsgItemExtra.Typing.StabilizationEffectRefined;
                        ItemDat.Refinary = Refinery;
                        ItemDat.Mode = Role.Flags.ItemMode.Update;
                    }
                    #endregion
                }
                try
                {
                    Update(ItemDat, AddMode.ADD, stream);
                }
                catch (Exception e)
                {
                    MyConsole.SaveException(e);
                }
                return true;
            }
            return false;
        }
        public bool AddSteed(ServerSockets.Packet stream, uint ID, byte count = 1, byte plus = 0, bool bound = false, byte ProgresGreen = 0, byte ProgresBlue = 0, byte ProgresRed = 0)
        {
            if (count == 0)
                count = 1;
            if (HaveSpace(count))
            {
                for (byte x = 0; x < count; x++)
                {
                    Database.ItemType.DBItem DbItem;
                    if (Database.Server.ItemsBase.TryGetValue(ID, out DbItem))
                    {
                        Game.MsgServer.MsgGameItem ItemDat = new Game.MsgServer.MsgGameItem();
                        ItemDat.UID = Database.Server.ITEM_Counter.Next;
                        ItemDat.ITEM_ID = ID;

                        ItemDat.ProgresGreen = ProgresGreen;
                        ItemDat.Enchant = ProgresBlue;
                        ItemDat.Bless = ProgresRed;
                        ItemDat.SocketProgress = (uint)(ProgresGreen | (ProgresBlue << 8) | (ProgresRed << 16));
                        ItemDat.Durability = ItemDat.MaximDurability = DbItem.Durability;
                        ItemDat.Plus = plus;
                        ItemDat.Color = (Role.Flags.Color)Program.GetRandom.Next(3, 9);
                        ItemDat.Bound = (byte)(bound ? 1 : 0);
                        try
                        {
                            if (!Update(ItemDat, AddMode.ADD, stream))
                                return false;
                        }
                        catch (Exception e)
                        {
                            MyConsole.SaveException(e);
                        }
                        if (x >= count)
                            return true;
                    }
                }
            }
            return false;
        }
        public bool AddNews(ServerSockets.Packet stream, uint ID, byte count = 1, byte plus = 0, byte bless = 0, byte Enchant = 0
    , Role.Flags.Gem sockone = Flags.Gem.NoSocket
     , Role.Flags.Gem socktwo = Flags.Gem.NoSocket, bool bound = false, Role.Flags.ItemEffect Effect = Flags.ItemEffect.None, bool SendMessage = false
    , string another_text = "", int days = 0, int hours = 0, int mins = 0, bool mine = false)
        {
            if (count == 0)
                count = 1;
            if (HaveSpace(count))
            {
                byte x = 0;
                for (; x < count; )
                {
                    x++;
                    Database.ItemType.DBItem DbItem;
                    if (Database.Server.ItemsBase.TryGetValue(ID, out DbItem))
                    {
                        Game.MsgServer.MsgGameItem ItemDat = new Game.MsgServer.MsgGameItem();
                        ItemDat.UID = Database.Server.ITEM_Counter.Next;
                        ItemDat.ITEM_ID = ID;
                        ItemDat.Effect = Effect;
                        ItemDat.Durability = ItemDat.MaximDurability = DbItem.Durability;
                        ItemDat.Plus = plus;
                        ItemDat.Bless = bless;
                        ItemDat.Enchant = Enchant;
                        ItemDat.SocketOne = sockone;
                        ItemDat.SocketTwo = socktwo;
                        ItemDat.PerfectionLevel = 54;
                        ItemDat.OwnerName = Owner.Player.Name;
                        ItemDat.OwnerUID = Owner.Player.UID;
                        ItemDat.Color = (Role.Flags.Color)Program.GetRandom.Next(3, 9);
                        ItemDat.Bound = (byte)(bound ? 1 : 0);
                        if (days == 0 && hours == 0 && mins == 0)
                        {
                            if (DbItem.Time != 0 && DbItem.StackSize == 0)
                            {
                                ItemDat.Activate = 1;
                                ItemDat.EndDate = DateTime.Now.AddMinutes(DbItem.Time);
                            }
                        }
                        else if (days != 0)
                        {
                            ItemDat.Activate = 1;
                            ItemDat.EndDate = DateTime.Now.AddDays(days);
                        }
                        else if (hours != 0)
                        {
                            ItemDat.Activate = 1;
                            ItemDat.EndDate = DateTime.Now.AddHours(hours);
                        }
                        else
                        {
                            ItemDat.Activate = 1;
                            ItemDat.EndDate = DateTime.Now.AddMinutes(mins);
                        }
                        if (SendMessage)
                        {
                            if (mine)
                            {
                                Owner.SendSysMesage("You~received~a~" + DbItem.Name + "" + another_text);
                            }
                            else
                            {
                                Owner.CreateBoxDialog("You~received~a~" + DbItem.Name + "" + another_text);
                            }
                        }
                        try
                        {
                            if (!Update(ItemDat, AddMode.ADD, stream))
                                return false;
                        }
                        catch (Exception e)
                        {
                            MyConsole.SaveException(e);
                        }

                    }
                }
                if (x >= count)
                    return true;
            }
            return false;
        }
        public bool Add5(uint ID, byte Plus, Database.ItemType.DBItem ITEMDB, ServerSockets.Packet stream, bool bound = false, int IDEvent = 0)
        {
            if (ITEMDB.StackSize > 0)
            {
                byte _bound = 0;
                if (bound)
                    _bound = 1;
                foreach (var item in ClientItems.Values)
                {

                    if (item.ITEM_ID == ID && item.Bound == _bound)
                    {
                        if (item.StackSize < ITEMDB.StackSize)
                        {
                            item.Mode = Flags.ItemMode.Update;
                            item.StackSize++;
                            item.RemainingTime2 = (ITEMDB.StackSize > 1) ? 0 : uint.MaxValue;
                            if (bound)
                                item.Bound = 1;
                            item.Send(Owner, stream);

                            return true;
                        }
                    }
                }
            }
            if (HaveSpace(1))
            {
                Game.MsgServer.MsgGameItem ItemDat = new Game.MsgServer.MsgGameItem();
                ItemDat.UID = Database.Server.ITEM_Counter.Next;
                ItemDat.ITEM_ID = ID;
                ItemDat.Durability = ItemDat.MaximDurability = ITEMDB.Durability;
                ItemDat.Plus = Plus;
                ItemDat.IDEvent = IDEvent;
                ItemDat.Color = (Role.Flags.Color)Program.GetRandom.Next(3, 9);
                ItemDat.RemainingTime2 = (ITEMDB.StackSize > 1) ? 0 : uint.MaxValue;
                if (bound)
                    ItemDat.Bound = 1;
                try
                {
                    Update(ItemDat, AddMode.ADD, stream);
                }
                catch (Exception e)
                {
                    MyConsole.SaveException(e);
                }
                return true;
            }
            return false;

        }

        public bool Add(ServerSockets.Packet stream, uint ID, byte count = 1, byte plus = 0, byte bless = 0, byte Enchant = 0
            , Role.Flags.Gem sockone = Flags.Gem.NoSocket
             , Role.Flags.Gem socktwo = Flags.Gem.NoSocket, bool bound = false, Role.Flags.ItemEffect Effect = Flags.ItemEffect.None, bool SendMessage = false
            , string another_text = "", int days = 0, int hours = 0, int mins = 0, bool mine = false)
        {
            if (count == 0)
                count = 1;
            if (HaveSpace(count))
            {
                byte x = 0;
                for (; x < count; )
                {
                    x++;
                    Database.ItemType.DBItem DbItem;
                    if (Database.Server.ItemsBase.TryGetValue(ID, out DbItem))
                    {
                        Game.MsgServer.MsgGameItem ItemDat = new Game.MsgServer.MsgGameItem();
                        ItemDat.UID = Database.Server.ITEM_Counter.Next;
                        ItemDat.ITEM_ID = ID;
                        ItemDat.Effect = Effect;
                        ItemDat.Durability = ItemDat.MaximDurability = DbItem.Durability;
                        ItemDat.Plus = plus;
                        ItemDat.Bless = bless;
                        ItemDat.Enchant = Enchant;
                        ItemDat.SocketOne = sockone;
                        ItemDat.SocketTwo = socktwo;
                        ItemDat.Color = (Role.Flags.Color)Program.GetRandom.Next(3, 9);
                        ItemDat.Bound = (byte)(bound ? 1 : 0);
                        if (days == 0 && hours == 0 && mins == 0)
                        {
                            if (DbItem.Time != 0 && DbItem.StackSize == 0)
                            {
                                ItemDat.Activate = 1;
                                ItemDat.EndDate = DateTime.Now.AddMinutes(DbItem.Time);
                            }
                        }
                        else if (days != 0)
                        {
                            ItemDat.Activate = 1;
                            ItemDat.EndDate = DateTime.Now.AddDays(days);
                        }
                        else if (hours != 0)
                        {
                            ItemDat.Activate = 1;
                            ItemDat.EndDate = DateTime.Now.AddHours(hours);
                        }
                        else
                        {
                            ItemDat.Activate = 1;
                            ItemDat.EndDate = DateTime.Now.AddMinutes(mins);
                        }
                        if (SendMessage)
                        {
                            switch (another_text)
                            {
                                case "~from~mining!":
                                    {
                                        Owner.SendWhisper("You~received~a~" + DbItem.Name + "" + another_text, "MiningSystem", Owner.Player.Name);
                                        break;
                                    }
                                default:
                                    Owner.CreateBoxDialog("You~received~a~" + DbItem.Name + "" + another_text);
                                    break;
                            }

                        }
                        try
                        {
                            if (!Update(ItemDat, AddMode.ADD, stream))
                                return false;
                        }
                        catch (Exception e)
                        {
                            MyConsole.SaveException(e);
                        }

                    }
                }
                if (x >= count)
                    return true;
            }
            return false;
        }
        public bool AddRefinaryItem(uint ID, bool Bound, ServerSockets.Packet stream)
        {
            ID = ID + Database.ItemType.GetNextRefineryItem();
            if (ID == 724348 || ID == 724349)
                ID += 150;
            if (ID == 724449)
                ID = 724445;

            return Add(stream, ID, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, Bound);
        }
        public bool AddItemWitchStack(uint ID, byte Plus, ushort amount, ServerSockets.Packet stream, bool bound = false)
        {
            Database.ItemType.DBItem DbItem;
            if (Database.Server.ItemsBase.TryGetValue(ID, out DbItem))
            {

                if (DbItem.StackSize > 0)
                {
                    byte _bound = 0;
                    if (bound)
                        _bound = 1;
                    foreach (var item in ClientItems.Values)
                    {

                        if (item.ITEM_ID == ID && item.Bound == _bound)
                        {
                            if (item.StackSize + amount <= DbItem.StackSize)
                            {
                                item.Mode = Flags.ItemMode.Update;
                                item.StackSize += amount;
                                if (bound)
                                    item.Bound = 1;
                                item.Send(Owner, stream);

                                return true;
                            }
                        }
                    }

                    if (amount > DbItem.StackSize)
                    {
                        if (HaveSpace((byte)((amount / DbItem.StackSize) + (byte)(Owner.OnInterServer ? 1 : 0))))
                        {
                            while (amount >= DbItem.StackSize)
                            {
                                Game.MsgServer.MsgGameItem ItemDat = new Game.MsgServer.MsgGameItem();
                                ItemDat.UID = Database.Server.ITEM_Counter.Next;
                                ItemDat.ITEM_ID = ID;
                                ItemDat.Durability = ItemDat.MaximDurability = DbItem.Durability;
                                ItemDat.Plus = Plus;
                                ItemDat.StackSize += DbItem.StackSize;
                                ItemDat.Color = (Role.Flags.Color)Program.GetRandom.Next(3, 9);
                                if (bound)
                                    ItemDat.Bound = 1;
                                try
                                {
                                    Update(ItemDat, AddMode.ADD, stream);
                                }
                                catch (Exception e)
                                {
                                    MyConsole.SaveException(e);
                                }
                                amount -= DbItem.StackSize;

                            }
                            if (amount > 0 && amount < DbItem.StackSize)
                            {
                                Game.MsgServer.MsgGameItem ItemDat = new Game.MsgServer.MsgGameItem();
                                ItemDat.UID = Database.Server.ITEM_Counter.Next;
                                ItemDat.ITEM_ID = ID;
                                ItemDat.Durability = ItemDat.MaximDurability = DbItem.Durability;
                                ItemDat.Plus = Plus;
                                ItemDat.StackSize += amount;
                                ItemDat.Color = (Role.Flags.Color)Program.GetRandom.Next(3, 9);
                                if (bound)
                                    ItemDat.Bound = 1;
                                try
                                {
                                    Update(ItemDat, AddMode.ADD, stream);
                                }
                                catch (Exception e)
                                {
                                    MyConsole.SaveException(e);
                                }
                            }
                            return true;
                        }
                        else
                        {
                            while (amount >= DbItem.StackSize)
                            {
                                AddReturnedItem(stream, ID, 1, Plus, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, bound, Flags.ItemEffect.None, DbItem.StackSize);
                                amount -= DbItem.StackSize;
                            }
                            if (amount > 0 && amount < DbItem.StackSize)
                            {
                                AddReturnedItem(stream, ID, 1, Plus, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, bound, Flags.ItemEffect.None, amount);
                            }
                            return true;
                        }
                    }
                    else
                    {
                        if (HaveSpace(1))
                        {
                            Game.MsgServer.MsgGameItem ItemDat = new Game.MsgServer.MsgGameItem();
                            ItemDat.UID = Database.Server.ITEM_Counter.Next;
                            ItemDat.ITEM_ID = ID;
                            ItemDat.Durability = ItemDat.MaximDurability = DbItem.Durability;
                            ItemDat.Plus = Plus;
                            ItemDat.StackSize = amount;
                            ItemDat.Color = (Role.Flags.Color)Program.GetRandom.Next(3, 9);
                            if (bound)
                                ItemDat.Bound = 1;
                            try
                            {
                                Update(ItemDat, AddMode.ADD, stream);
                            }
                            catch (Exception e)
                            {
                                MyConsole.SaveException(e);
                            }
                            return true;
                        }
                    }
                }
                for (int count = 0; count < amount; count++)
                    Add(ID, Plus, DbItem, stream, bound);
                return true;
            }
            return false;
        }
        public bool Add100(uint ID, byte Plus, ushort amount, ServerSockets.Packet stream, bool bound = false)
        {
            Database.ItemType.DBItem DbItem;
            if (Database.Server.ItemsBase.TryGetValue(ID, out DbItem))
            {

                if (DbItem.StackSize > 0)
                {
                    byte _bound = 0;
                    if (bound)
                        _bound = 1;
                    foreach (var item in ClientItems.Values)
                    {

                        if (item.ITEM_ID == ID && item.Bound == _bound)
                        {
                            if (item.StackSize + amount <= DbItem.StackSize)
                            {
                                item.Mode = Flags.ItemMode.Update;
                                item.StackSize += amount;
                                if (bound)
                                    item.Bound = 1;
                                item.Send(Owner, stream);

                                return true;
                            }
                        }
                    }

                    if (amount > DbItem.StackSize)
                    {
                        if (HaveSpace((byte)((amount / DbItem.StackSize) + (byte)(Owner.OnInterServer ? 1 : 0))))
                        {
                            while (amount >= DbItem.StackSize)
                            {
                                Game.MsgServer.MsgGameItem ItemDat = new Game.MsgServer.MsgGameItem();
                                ItemDat.UID = Database.Server.ITEM_Counter.Next;
                                ItemDat.ITEM_ID = ID;
                                ItemDat.Durability = ItemDat.MaximDurability = DbItem.Durability;
                                ItemDat.Plus = Plus;
                                ItemDat.StackSize += DbItem.StackSize;
                                ItemDat.Color = (Role.Flags.Color)Program.GetRandom.Next(3, 9);
                                if (bound)
                                    ItemDat.Bound = 1;
                                try
                                {
                                    Update(ItemDat, AddMode.ADD, stream);
                                }
                                catch (Exception e)
                                {
                                    MyConsole.SaveException(e);
                                }
                                amount -= DbItem.StackSize;

                            }
                            if (amount > 0 && amount < DbItem.StackSize)
                            {
                                Game.MsgServer.MsgGameItem ItemDat = new Game.MsgServer.MsgGameItem();
                                ItemDat.UID = Database.Server.ITEM_Counter.Next;
                                ItemDat.ITEM_ID = ID;
                                ItemDat.Durability = ItemDat.MaximDurability = DbItem.Durability;
                                ItemDat.Plus = Plus;
                                ItemDat.StackSize += amount;
                                ItemDat.Color = (Role.Flags.Color)Program.GetRandom.Next(3, 9);
                                if (bound)
                                    ItemDat.Bound = 1;
                                try
                                {
                                    Update(ItemDat, AddMode.ADD, stream);
                                }
                                catch (Exception e)
                                {
                                    MyConsole.SaveException(e);
                                }
                            }
                            return true;
                        }
                        else
                        {
                            while (amount >= DbItem.StackSize)
                            {
                                AddReturnedItem(stream, ID, 1, Plus, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, bound, Flags.ItemEffect.None, DbItem.StackSize);
                                amount -= DbItem.StackSize;
                            }
                            if (amount > 0 && amount < DbItem.StackSize)
                            {
                                AddReturnedItem(stream, ID, 1, Plus, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, bound, Flags.ItemEffect.None, amount);
                            }
                            return true;
                        }
                    }
                    else
                    {
                        if (HaveSpace(1))
                        {
                            Game.MsgServer.MsgGameItem ItemDat = new Game.MsgServer.MsgGameItem();
                            ItemDat.UID = Database.Server.ITEM_Counter.Next;
                            ItemDat.ITEM_ID = ID;
                            ItemDat.Durability = ItemDat.MaximDurability = DbItem.Durability;
                            ItemDat.Plus = Plus;
                            ItemDat.StackSize = amount;
                            ItemDat.Color = (Role.Flags.Color)Program.GetRandom.Next(3, 9);
                            if (bound)
                                ItemDat.Bound = 1;
                            try
                            {
                                Update(ItemDat, AddMode.ADD, stream);
                            }
                            catch (Exception e)
                            {
                                MyConsole.SaveException(e);
                            }
                            return true;
                        }
                    }
                }
                for (int count = 0; count < amount; count++)
                    Add(ID, Plus, DbItem, stream, bound);
                return true;
            }
            return false;
        }
        public bool AddSoul(ServerSockets.Packet stream, Client.GameClient client, uint ID, uint SoulID, uint purfylevel, byte plus = 0, byte count = 1, DeathWish.Role.Flags.Gem sockone = 0, DeathWish.Role.Flags.Gem socktwo = 0, bool bound = false, DeathWish.Role.Flags.ItemEffect Effect = 0, bool SendMessage = false, string another_text = "")
        {
            if (count == 0)
            {
                count = 1;
            }
            if (this.HaveSpace(count))
            {
                byte num = 0;
                while (num < count)
                {
                    DeathWish.Database.ItemType.DBItem item;
                    num = (byte)(num + 1);
                    if (DeathWish.Database.Server.ItemsBase.TryGetValue(ID, out item))
                    {
                        MsgGameItem item2;
                        item2 = new MsgGameItem
                        {
                            UID = DeathWish.Database.Server.ITEM_Counter.Next,
                            ITEM_ID = ID,
                            Effect = Effect,
                            Durability = item.Durability,
                            Plus = plus,
                            Bless = 7,
                            Enchant = 0xff,
                            SocketOne = sockone,
                            SocketTwo = socktwo,
                            PerfectionLevel = 54,
                            OwnerName = Owner.Player.Name,
                            OwnerUID = Owner.Player.UID,
                            Color = (DeathWish.Role.Flags.Color)Program.GetRandom.Next(3, 9),
                            Bound = bound ? ((byte)1) : ((byte)0),
                            RemainingTime2 = (item.StackSize > 1) ? 0 : uint.MaxValue,
                            Purification = new MsgItemExtra.Purification()
                        };
                        item2.Purification.AddedOn = DateTime.Now;
                        item2.Purification.ItemUID = item2.UID;
                        item2.Purification.PurificationLevel = purfylevel;
                        item2.Purification.PurificationDuration = 0;
                        item2.Purification.PurificationItemID = SoulID;
                        item2.Purification.Typ = MsgItemExtra.Typing.PurificationEffect;
                        MsgItemExtra extra = new MsgItemExtra
                        {
                            Purifications = { item2.Purification }
                        };
                        client.Send(extra.CreateArray(stream, false));
                        item2.Mode = DeathWish.Role.Flags.ItemMode.AddItem | DeathWish.Role.Flags.ItemMode.Trade;
                        item2.Send(client, stream);
                        if (SendMessage)
                        {
                            this.Owner.CreateBoxDialog("You~received~a~" + item.Name + another_text);
                        }
                        try
                        {
                            if (!this.Update(item2, AddMode.ADD, stream, false))
                            {
                                return false;
                            }
                        }
                        catch (Exception exception)
                        {
                            MyConsole.SaveException(exception);
                        }
                    }
                }
                if (num >= count)
                {
                    return true;
                }
            }
            return false;
        }

        public bool AddSoul(uint ID, uint SoulID, uint soullevel, uint souldays, byte plus, byte gem1, byte gem2, byte hp, byte daamge, byte times, ServerSockets.Packet stream, bool bound)
        {
            Database.ItemType.DBItem Soul = null;
            Database.ItemType.DBItem ITEMDB = null;
            if (SoulID != 0)
            {
                if (!Database.Server.ItemsBase.TryGetValue((uint)SoulID, out Soul))
                    return false;

            }
            if (!Database.Server.ItemsBase.TryGetValue((uint)ID, out ITEMDB))
                return false;
            if (HaveSpace(1))
            {
                Game.MsgServer.MsgGameItem ItemDat = new Game.MsgServer.MsgGameItem();
                ItemDat.UID = Database.Server.ITEM_Counter.Next;
                ItemDat.ITEM_ID = ID;
                ItemDat.Durability = ItemDat.MaximDurability = ITEMDB.Durability;
                ItemDat.Plus = plus;
                ItemDat.SocketOne = (Flags.Gem)gem1;
                ItemDat.SocketTwo = (Flags.Gem)gem2;
                ItemDat.Bless = daamge;
                if (hp > 0)
                {
                    ItemDat.Enchant = (byte)(new System.Random().Next(200, 240));
                }
                ItemDat.PerfectionLevel = 54;
                ItemDat.OwnerName = Owner.Player.Name;
                ItemDat.OwnerUID = Owner.Player.UID;
                ItemDat.Color = (Role.Flags.Color)Program.GetRandom.Next(3, 9);
                ItemDat.RemainingTime2 = (ITEMDB.StackSize > 1) ? 0 : uint.MaxValue;

                try
                {
                    Update(ItemDat, AddMode.ADD, stream);
                }
                catch (Exception e)
                {
                    MyConsole.SaveException(e);
                }
                return true;
            }
            return false;

        }

        public bool ContainItemWithStack(uint UID, ushort Count)
        {
            Game.MsgServer.MsgGameItem ItemDat;
            if (ClientItems.TryGetValue(UID, out ItemDat))
            {
                return ItemDat.StackSize >= Count || Count == 1 && ItemDat.StackSize == 0;
            }
            return false;
        }

        public bool RemoveStackItem(uint UID, ushort Count, ServerSockets.Packet stream)
        {
            Game.MsgServer.MsgGameItem ItemDat;
            if (ClientItems.TryGetValue(UID, out ItemDat))
            {
                if (ItemDat.StackSize > Count)
                {
                    ItemDat.StackSize -= Count;
                    ItemDat.Mode = Flags.ItemMode.Update;
                    ItemDat.Send(Owner, stream);
                }
                else
                {
                    ItemDat.StackSize = 1;
                    Update(ItemDat, AddMode.REMOVE, stream);
                    return true;
                }
            }
            else
            {

                foreach (var item in ClientItems.Values)
                {
                    if (0 == Count)
                        break;
                    if (item.ITEM_ID == UID)
                    {
                        if (item.StackSize > Count)
                        {
                            item.StackSize -= Count;
                            item.Mode = Flags.ItemMode.Update;
                            item.Send(Owner, stream);
                            Count = 0;
                        }
                        else
                        {
                            Count -= item.StackSize;
                            item.StackSize = 1;
                            Update(item, AddMode.REMOVE, stream);
                        }
                    }
                }
            }
            return false;
        }
        public bool Add(uint ID, byte Plus, Database.ItemType.DBItem ITEMDB, ServerSockets.Packet stream, bool bound = false)
        {
            if (ITEMDB.StackSize > 0)
            {
                byte _bound = 0;
                if (bound)
                    _bound = 1;
                foreach (var item in ClientItems.Values)
                {

                    if (item.ITEM_ID == ID && item.Bound == _bound)
                    {
                        if (item.StackSize < ITEMDB.StackSize)
                        {
                            item.Mode = Flags.ItemMode.Update;
                            item.StackSize++;
                            if (ITEMDB.Time != 0)
                            {
                                item.Activate = 1;
                                item.EndDate = DateTime.Now.AddMinutes(ITEMDB.Time);
                            }
                            if (bound)
                                item.Bound = 1;
                            item.Send(Owner, stream);

                            return true;
                        }
                    }
                }
            }
            if (HaveSpace(1))
            {
                Game.MsgServer.MsgGameItem ItemDat = new Game.MsgServer.MsgGameItem();
                ItemDat.UID = Database.Server.ITEM_Counter.Next;
                ItemDat.ITEM_ID = ID;
                ItemDat.Durability = ItemDat.MaximDurability = ITEMDB.Durability;
                ItemDat.Plus = Plus;
                ItemDat.Color = (Role.Flags.Color)Program.GetRandom.Next(3, 9);
                if (ITEMDB.Time != 0 && ITEMDB.StackSize == 1)
                {
                    ItemDat.Activate = 1;
                    ItemDat.EndDate = DateTime.Now.AddMinutes(ITEMDB.Time);
                }
                if (bound)
                    ItemDat.Bound = 1;
                try
                {
                    Update(ItemDat, AddMode.ADD, stream);
                }
                catch (Exception e)
                {
                    MyConsole.SaveException(e);
                }
                return true;
            }
            return false;

        }
        public bool Add(Game.MsgServer.MsgGameItem ItemDat, Database.ItemType.DBItem ITEMDB, ServerSockets.Packet stream)
        {
            if (ITEMDB.StackSize > 0)
            {
                foreach (var item in ClientItems.Values)
                {
                    if (item.ITEM_ID == ItemDat.ITEM_ID)
                    {
                        if (item.StackSize < ITEMDB.StackSize)
                        {
                            item.Mode = Flags.ItemMode.Update;
                            item.StackSize++;
                            item.Send(Owner, stream);
                            return true;
                        }
                    }
                }
            }
            if (HaveSpace(1))
            {
                if (ITEMDB.Time != 0 && ITEMDB.StackSize == 1)
                {
                    ItemDat.Activate = 1;
                    ItemDat.EndDate = DateTime.Now.AddMinutes(ITEMDB.Time);
                }
                Update(ItemDat, AddMode.ADD, stream);
                return true;
            }
            return false;

        }
        public bool AddItemWitchStack(Game.MsgServer.MsgGameItem ItemDat, byte amount, ServerSockets.Packet stream)
        {
            Database.ItemType.DBItem DbItem;
            if (Database.Server.ItemsBase.TryGetValue(ItemDat.ITEM_ID, out DbItem))
            {
                for (int count = 0; count < amount; count++)
                    Add(ItemDat, DbItem, stream);
                return true;
            }
            return false;
        }
        public unsafe bool Update(Game.MsgServer.MsgGameItem ItemDat, AddMode mode, ServerSockets.Packet stream, bool Removefull = false)
        {
            if (HaveSpace(1) || mode == AddMode.REMOVE)
            {
                switch (mode)
                {
                    case AddMode.ADD:
                        {
                            CheakUp(ItemDat);
                            if (ItemDat.StackSize == 0)
                                ItemDat.StackSize = 1;
                            ItemDat.Position = 0;
                            ItemDat.Mode = Flags.ItemMode.AddItem;
                            ItemDat.Send(Owner, stream);
                            var dt = DateTime.Now;
                            string data = "[Inv-Add] " + dt.Hour + "H:" + dt.Minute + "M pid:  " + Owner.Player.UID + "  " + Owner.Player.Name + " Get  " + ItemDat.ITEM_ID + " \n";
                            Database.ServerDatabase.LoginQueue.Enqueue(data);
                            if (Owner.IsConnectedInterServer())
                            {
                                ItemDat.Send(Owner.PipeClient, stream);
                            }
                            break;
                        }
                    case AddMode.MOVE:
                        {
                            CheakUp(ItemDat);
                            ItemDat.Position = 0;
                            ItemDat.Mode = Flags.ItemMode.AddItem;
                            ItemDat.Send(Owner, stream);
                            break;
                        }
                    case AddMode.REMOVE:
                        {
                            if (ItemDat.StackSize > 1 && ItemDat.Position < 40 && !Removefull)
                            {
                                ItemDat.StackSize -= 1;
                                ItemDat.Mode = Flags.ItemMode.Update;
                                ItemDat.Send(Owner, stream);

                                break;
                            }
                            Game.MsgServer.MsgGameItem item;
                            if (ClientItems.TryRemove(ItemDat.UID, out item))
                            {
                                Owner.Send(stream.ItemUsageCreate(MsgItemUsuagePacket.ItemUsuageID.RemoveInventory, item.UID, 0, 0, 0, 0, 0));
                                var dt = DateTime.Now;
                                string data = "[Inv-Lost] " + dt.Hour + "H:" + dt.Minute + "M pid:   " + Owner.Player.UID + "  " + Owner.Player.Name + " Lost  " + ItemDat.ITEM_ID + " \n";
                                Database.ServerDatabase.LoginQueue.Enqueue(data);
                            }
                            break;
                        }
                }
                if (ItemDat.ITEM_ID == 750000)
                {
                    Owner.DemonExterminator.ItemUID = ItemDat.UID;
                    if (mode == AddMode.REMOVE)
                        Owner.DemonExterminator.ItemUID = 0;
                }

                return true;

            }
            return false;
        }
        private void CheakUp(Game.MsgServer.MsgGameItem ItemDat)
        {
            if (ItemDat.UID == 0)
                ItemDat.UID = Database.Server.ITEM_Counter.Next;
            if (!ClientItems.TryAdd(ItemDat.UID, ItemDat))
            {
                do
                    ItemDat.UID = Database.Server.ITEM_Counter.Next;
                while
                  (ClientItems.TryAdd(ItemDat.UID, ItemDat) == false);
            }
        }

        public bool CheckMeteors(byte count, bool Removethat, ServerSockets.Packet stream)
        {

            if (Contain(1088001, count))
            {
                if (Removethat)
                    Remove(1088001, count, stream);
                return true;
            }
            else
            {
                byte Counter = 0;
                var RemoveThis = new Dictionary<uint, Game.MsgServer.MsgGameItem>();
                var MyMetscrolls = GetMyMetscrolls();
                var MyMeteors = GetMyMeteors();
                foreach (var GameItem in MyMetscrolls.Values)
                {
                    Counter += 10;
                    RemoveThis.Add(GameItem.UID, GameItem);
                    if (Counter >= count)
                        break;
                }
                if (Counter >= count)
                {
                    byte needSpace = (byte)(Counter - count);
                    if (HaveSpace(needSpace))
                    {
                        if (Removethat)
                        {
                            Add(stream, 1088001, 0, needSpace);
                        }
                    }
                    else
                    {
                        Counter -= 10;
                        RemoveThis.Remove(RemoveThis.Values.First().UID);
                        byte needmetsss = (byte)(count - Counter);
                        if (needmetsss <= MyMeteors.Count)
                        {
                            foreach (var GameItem in MyMeteors.Values)
                            {
                                Counter += 1;
                                RemoveThis.Add(GameItem.UID, GameItem);
                                if (Counter >= count)
                                    break;
                            }
                            if (Removethat)
                            {
                                foreach (var GameItem in RemoveThis.Values)
                                    Update(GameItem, AddMode.REMOVE, stream);
                            }
                        }
                        else
                            return false;
                    }
                    if (Removethat)
                    {
                        foreach (var GameItem in RemoveThis.Values)
                            Update(GameItem, AddMode.REMOVE, stream);
                    }
                    return true;
                }
                foreach (var GameItem in MyMeteors.Values)
                {
                    Counter += 1;
                    RemoveThis.Add(GameItem.UID, GameItem);
                    if (Counter >= count)
                        break;
                }
                if (Counter >= count)
                {
                    if (Removethat)
                    {
                        foreach (var GameItem in RemoveThis.Values)
                            Update(GameItem, AddMode.REMOVE, stream);
                    }
                    return true;
                }
            }

            return false;
        }
        private Dictionary<uint, Game.MsgServer.MsgGameItem> GetMyMetscrolls()
        {
            var array = new Dictionary<uint, Game.MsgServer.MsgGameItem>();
            foreach (var GameItem in ClientItems.Values)
            {
                if (GameItem.ITEM_ID == 720027)
                {
                    if (!array.ContainsKey(GameItem.UID))
                        array.Add(GameItem.UID, GameItem);
                }
            }
            return array;
        }
        private Dictionary<uint, Game.MsgServer.MsgGameItem> GetMyMeteors()
        {
            var array = new Dictionary<uint, Game.MsgServer.MsgGameItem>();
            foreach (var GameItem in ClientItems.Values)
            {
                if (GameItem.ITEM_ID == Database.ItemType.Meteor || GameItem.ITEM_ID == Database.ItemType.MeteorTear)
                {
                    if (!array.ContainsKey(GameItem.UID))
                        array.Add(GameItem.UID, GameItem);
                }
            }
            return array;
        }
        public bool CollectedMoonBoxTokens(byte bound = 0)
        {
            return Contain(721010, 1, bound) && Contain(721011, 1, bound) && Contain(721012, 1, bound) && Contain(721013, 1, bound) && Contain(721014, 1, bound) && Contain(721015, 1, bound);
        }
        public bool CollectedAnyMoonBoxTokens()
        {
            return Contain(721010, 1) || Contain(721011, 1) || Contain(721012, 1) || Contain(721013, 1) || Contain(721014, 1) || Contain(721015, 1);
        }
        public bool CollectedTokens(uint ID)
        {
            if (ID == 1044)
            {
                return Contain(721011, 1);
            }
            if (ID == 1046)
            {
                return Contain(721013, 1);
            }
            if (ID == 1048)
            {
                return Contain(721015, 1);
            }
            if (ID == 1045)
            {
                return Contain(721012, 1);
            }
            if (ID == 1043)
            {
                return Contain(721010, 1);
            }
            if (ID == 1047)
            {
                return Contain(721014, 1);
            }
            return false;
        }
        public void ShowALL(ServerSockets.Packet stream)
        {
            foreach (var msg_item in ClientItems.Values)
            {
                msg_item.Mode = Flags.ItemMode.AddItem;
                msg_item.Send(Owner, stream);
            }
        }
        public void Clear(ServerSockets.Packet stream)
        {
            var dictionary = ClientItems.Values.ToArray();
            foreach (var msg_item in dictionary)
                Update(msg_item, AddMode.REMOVE, stream);
        }
    }
}

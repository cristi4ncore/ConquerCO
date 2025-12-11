
using Extensions;
using DeathWish.Game.MsgServer;
using DeathWish.Role;
using System;
using System.Linq;
using System.Collections.Generic;
using DeathWish.Client;
using DeathWish.Game;
using DeathWish.Role.Instance;
using DeathWish.Database;
using System.IO;

namespace DeathWish.Game
{
    public class PlayerbotBooth
    {

        public PlayerbotBooth(Client.GameClient client)
        {
            Owner = client;
        }
        public void GetItems()
        {
            SelectionItems = new Dictionary<byte, Game.MsgServer.MsgGameItem>();
            count = 1;
            foreach (var item in Owner.Inventory.ClientItems.Values)
            {
                if (Server.ItemsBase.ContainsKey(item.ITEM_ID))
                {
                    if (item.Bound == 0)
                    {
                        SelectionItems.Add(count, item);
                        count++;
                    }
                }
            }
        }
        public Dictionary<byte, Game.MsgServer.MsgGameItem> SelectionItems = new Dictionary<byte, Game.MsgServer.MsgGameItem>();
        public Dictionary<uint, Game.MsgServer.MsgGameItem> OldBotItems = new Dictionary<uint, Game.MsgServer.MsgGameItem>();


        public static unsafe void Load()
        {      
            foreach (string fname in System.IO.Directory.GetFiles(Program.ServerConfig.DbLocation + "\\PlayersbotBooth\\"))
            {
                if (File.Exists(fname))
                {
                    PlayerbotBooth BotBooth = new PlayerbotBooth(null);
                    BotBooth.oldbooth = true;
                    BotBoothDB.getboothinfo(fname, out BotBooth.OwnerUID, out BotBooth.Profits);
                    if (BotBoothDB.CanGetItems())
                    {
                        int Count;
                        int Count_;
                        ClientItems.DBItem Item;
                        BotBoothDB.GetCount(&Count);
                        var INC = new Dictionary<uint, MsgGameItem>();
                        for (int x = 0; x < Count; x++)
                        {
                            BotBoothDB.GetDBitem(&Item);
                            var CITEM = Item.GetDataItem();
                            if (Item.Position == 0)
                                BotBooth.OldBotItems.Add(CITEM.UID, CITEM);

                        }
                        BotBoothDB.GetCount(&Count_);
                        for (int x = 0; x < Count_; x++)
                        {
                            var info = new ClientItems.Perfection();
                            BotBoothDB.GetPerfectionitem(&info);
                            if (INC.ContainsKey(info.ItemUID))
                            {
                                var item = INC[info.ItemUID];
                                item.PerfectionLevel = info.Level;
                                item.OwnerUID = info.OwnerUID;
                                item.OwnerName = info.OwnerName;
                                item.PerfectionProgress = info.Progres;
                                item.Signature = info.SpecialText;
                                continue;
                            }

                            else if (BotBooth.OldBotItems.ContainsKey(info.ItemUID))
                            {
                                var item = BotBooth.OldBotItems[info.ItemUID];
                                item.PerfectionLevel = info.Level;
                                item.OwnerUID = info.OwnerUID;
                                item.OwnerName = info.OwnerName;
                                item.PerfectionProgress = info.Progres;
                                item.Signature = info.SpecialText;
                                continue;
                            }

                        }
                        BotBoothDB.EndRead();
                    }
                    if (!BotBooths.ContainsKey(BotBooth.OwnerUID))
                        BotBooths.Add(BotBooth.OwnerUID, BotBooth);

                }
                else
                {
                    Console.WriteLine("[Playerbooth] Error can't find file " + fname);
                }
            }

        }
        public static unsafe void save()
        {
            BotBoothDB.clear();
            foreach (var booth in BotBooths.Values)
            {
                BotBoothDB.saveboothinfo(booth.OwnerUID, booth.Profits);
                if (booth.Bot == null)
                    continue;
                Dictionary<uint, MsgGameItem> INC = new Dictionary<uint, MsgGameItem>();
                if (BotBoothDB.CansaveItems())
                {
                    var DBItem = new ClientItems.DBItem();
                    int Count;
                    Count = booth.Bot.GetItemsCount();
                    BotBoothDB.SetCount(&Count);
                    foreach (var item in booth.Bot.AllMyItems())
                    {
                        DBItem.GetDBItem(item);
                        BotBoothDB.SetDBitem(&DBItem);
                        if (item.Deposite.Count > 0)
                            foreach (var DItem in item.Deposite.Values)
                            {
                                DBItem.GetDBItem(DItem);
                                BotBoothDB.SetDBitem(&DBItem);
                                if ((DItem.PerfectionLevel > 0 || DItem.PerfectionProgress > 0) && DItem.IsEquip)
                                    if (!INC.ContainsKey(DItem.UID))
                                        INC.Add(DItem.UID, DItem);
                            }
                    }
                    Count = booth.Bot.GetPerfectionItemsCount() + INC.Count;
                    BotBoothDB.SetCount(&Count);
                    foreach (var item in booth.Bot.AllPerfectionItems())
                    {
                        var info = DBItem.GetPerfectionInfo(item);
                        BotBoothDB.SetPerfectionitem(&info);
                    }
                    foreach (var item in INC.Values)
                    {
                        var info = DBItem.GetPerfectionInfo(item);
                        BotBoothDB.SetPerfectionitem(&info);
                    }
                    BotBoothDB.EndWrite();
                }
            }
        }
        public void getbooth()
        {
            PlayerbotBooth booth;
            if (BotBooths.TryGetValue(Owner.Player.UID, out booth))
            {
                Owner.MyBotBooth = booth;
                booth.Owner = Owner;
                if (booth.oldbooth)
                {
                    if (booth.Profits > 0)
                        Owner.SendWhisper("You have gained " + booth.Profits.ToString("N0") + " Cps. Please Talk to Merchent to get your profits and remaining booth items..", "BotBooth", Owner.Player.Name);
                    else
                        Owner.SendWhisper("Please Talk to Merchent to get your remaining booth items..", "BotBooth", Owner.Player.Name);
                }
            }
        }
        public uint Profits;
        public uint OwnerUID;
        public bool oldbooth = false;
        public uint selecteditemprice = 0;
        public GameClient Bot;
        public Client.GameClient Owner;
        public Game.Booth booth;
        public static System.Collections.Generic.SafeDictionary<uint, PlayerbotBooth> BotBooths = new System.Collections.Generic.SafeDictionary<uint, PlayerbotBooth>();

        byte count = 1;
        public unsafe bool RemoveBooth(out string exeption)
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                if (Owner.Inventory.HaveSpace((byte)booth.ItemList.Values.Count))
                {
                    foreach (Game.BoothItem item in booth.ItemList.Values)
                    {

                        Owner.Inventory.Update(item.Item, Role.Instance.AddMode.MOVE, stream);
                        Bot.Inventory.Update(item.Item, Role.Instance.AddMode.REMOVE, stream, true);
                    }
                    if (Profits > 0)
                        Owner.Player.ConquerPoints += Profits;

                    var action = new ActionQuery()
                    {
                        ObjId = Bot.Player.UID,
                        Type = ActionType.RemoveEntity
                    };
                    var action2 = new ActionQuery()
                    {
                        ObjId = booth.Base.UID,
                        Type = ActionType.RemoveEntity
                    };
                    if (BotBooths.ContainsKey(OwnerUID))
                        BotBooths.Remove(OwnerUID);
                    Server.ServerMaps[1036].Denquer(Bot);
                    Server.ServerMaps[1036].View.LeaveMap<Role.IMapObj>(booth.Base);
                    Server.ServerMaps[1036].RemoveSobnpc(booth.Base, stream);
                    Bot.Player.View.SendView(stream.ActionCreate(&action), false);
                    Bot.Player.View.SendView(stream.ActionCreate(&action2), false);
                    booth = null;
                    Bot = null;
                    exeption = "";
                    return true;
                }
                exeption = "You must have at least " + booth.ItemList.Values.Count + " empty spaces in your inventory";
                return false;
            }
        }
        public byte Flag;
        public MsgMessage HawkMessage;
        public Flags.ConquerAction Action = Flags.ConquerAction.None;
        public void CreateBooth(ServerSockets.Packet stream)
        {
            if (Flag <= 0)
            {
                Owner.MessageBox("You must select shopflag first..");
                return;
            }
            var ShopFlag = BotBoothDB.Shopflags[Flag];
            if (!Server.ServerMaps[1036].IsValidFlagNpc(ShopFlag.X, ShopFlag.Y))
            {
                Owner.MessageBox("Sorry but this shopflag is already taken..");
                return;
            }
            ShopFlag.X++;
            booth = new Game.Booth();
            SobNpc Base = new SobNpc();
            Base.UID = Booth.BoothCounter.Next + 10;
            Base.ObjType = MapObjectType.SobNpc;
            Base.Mesh = DeathWish.Role.SobNpc.StaticMesh.Vendor;
            Base.Type = Role.Flags.NpcType.Booth;
            Base.Name = Owner.Player.Name + "";
            Base.Map = 1036;
            Base.Booth = booth;
            Base.X = ShopFlag.X;
            Base.Y = ShopFlag.Y;
            booth.Base = Base;
            booth.HawkMessage = HawkMessage;
            uint key = Base.UID + 10;
            if (!BotBooths.ContainsKey(key))
            {
                Bot = new GameClient(null, false)
                {
                    Fake = true
                };
                Bot.Player = new Player(Bot);
                Bot.Inventory = new Inventory(Bot);
                Bot.Equipment = new DeathWish.Role.Instance.Equip(Bot);
                Bot.Warehouse = new Warehouse(Bot);
                Bot.MyProfs = new Proficiency(Bot);
                Bot.MySpells = new DeathWish.Role.Instance.Spell(Bot);
                Bot.Achievement = new AchievementCollection();
                Bot.Status = new MsgStatus();
                Bot.Player.Name = Owner.Player.Name + "[Bot]";
                Bot.Player.Body = Owner.Player.Body;
                Bot.Player.Hair = Owner.Player.Hair;
                Bot.Player.HairColor = Owner.Player.HairColor;
                Bot.Player.Angle = Flags.ConquerAngle.SouthEast;
                Bot.Player.UID = key;
                Bot.Status.MaxHitpoints = 0xea60;
                Bot.Player.HitPoints = 0xea60;
                Bot.Player.X = (ushort)(Base.X - 1);
                Bot.Player.Y = Base.Y;
                Bot.Player.Map = 1036;
                Bot.Player.Level = 140;
                Bot.Player.Action = Action;
                Bot.Map = DeathWish.Database.Server.ServerMaps[1036];
                Bot.Map.Enquer(Bot);
                Bot.Player.ArmorId = Owner.Player.ArmorId;
                Bot.Player.HeadId = Owner.Player.HeadId;
                Bot.Player.ArmorSoul = Owner.Player.ArmorSoul;
                Bot.Player.HeadSoul = Owner.Player.HeadSoul;
                Bot.Player.GarmentId = Owner.Player.GarmentId;
                Bot.Player.RightWeaponId = Owner.Player.RightWeaponId;
                Bot.Player.LeftWeaponId = Owner.Player.LeftWeaponId;
                Bot.Player.RightWeaponAccessoryId = Owner.Player.RightWeaponAccessoryId;
                Bot.Player.LeftWeaponAccessoryId = Owner.Player.LeftWeaponAccessoryId;
                Bot.Player.LeftWeapsonSoul = Owner.Player.LeftWeapsonSoul;
                Bot.Player.RightWeapsonSoul = Owner.Player.RightWeapsonSoul;
                OwnerUID = Owner.Player.UID;
                booth.Owner = Bot;
                booth.MainOwner = this;
                Bot.MyBooth = booth;
                Server.ServerMaps[Base.Map].AddSobnpc(Base);
                foreach (var IObj in Bot.Player.View.Roles(MapObjectType.Player))
                {
                    Role.Player screenObj = IObj as Role.Player;
                    screenObj.View.CanAdd(Base, true, stream);
                }


                Bot.Player.View.Role(false, null);
                BotBooths.Add(Owner.Player.UID, this);
                Owner.MessageBox("You created new bot booth");
            }

        }
        public bool AddItem(MsgGameItem Selecteditem, ServerSockets.Packet stream, out string exeption)
        {

            exeption = "";
            if (Selecteditem.Locked > 0 || Selecteditem.Inscribed > 0)
            {
                exeption = "please unlock your item first or remove it from guild Arsenal..";
                return false;
            }
            Bot.Inventory.Update(Selecteditem, Role.Instance.AddMode.MOVE, stream);
            Owner.Inventory.Update(Selecteditem, Role.Instance.AddMode.REMOVE, stream, true);
            Game.BoothItem item = new Game.BoothItem();
            item.Cost = selecteditemprice;
            item.Cost_Type = MsgItemView.ActionMode.CPs;
            item.Item = Selecteditem;
            ushort ItemPos = Database.ItemType.ItemPosition(item.Item.ITEM_ID);
            if (ItemPos == (ushort)Role.Flags.ConquerItem.Wing)
                if (item.Item.Bless >= 1)
                    item.Item.Bless = 0;
            ItemPos = Database.ItemType.ItemPosition(item.Item.ITEM_ID);
            if (ItemPos == (ushort)Role.Flags.ConquerItem.Tower)
                if (item.Item.Bless >= 2)
                    item.Item.Bless = 1;
            ItemPos = Database.ItemType.ItemPosition(item.Item.ITEM_ID);
            if (ItemPos == (ushort)Role.Flags.ConquerItem.Fan)
                if (item.Item.Bless >= 2)
                    item.Item.Bless = 1;
            booth.ItemList.Add(item.Item.UID, item);
            return true;
        }
    }
}

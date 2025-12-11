using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using DeathWish.Client;
using DeathWish.Game;
using Extensions;
using DeathWish.Role;
using DeathWish.Game.MsgServer;
using DeathWish.Role.Instance;
using DeathWish.Database;

namespace DeathWish
{
    public class Booths
    {
        public enum BoothType
        {
            Npc = 0,
            Entity = 1
        }
        public class booth
        {
            public uint UID;
            public uint HairStyle;
            public uint Body;
            public ushort Mesh = 100;
            public string Name;
            public string CostType;
            public ushort Map1;
            public string Costtype;
            public Role.GameMap Map;
            public ushort X;
            public ushort Y;
            public List<string> Items;
            public BoothType Type;
            public string BotMessage = "Selling Items.[Boothing AI]";
            public uint Garment = 194300;
            public uint Action;
            public uint Head = 112259;
            public uint WeaponR = 601439;
            public uint WeaponL = 601439;
            public uint Armor = 135259;
        }
        public static System.Collections.Generic.SafeDictionary<uint, booth> Boooths = new System.Collections.Generic.SafeDictionary<uint, booth>();
        public static void Load()
        {
            string[] text = File.ReadAllLines(Program.ServerConfig.DbLocation + "Booths.txt");
            booth booth = new booth();
            for (int x = 0; x < text.Length; x++)
            {
                string line = text[x];
                string[] split = line.Split('=');
                if (split[0] == "ID")
                {
                    if (booth.UID == 0)
                        booth.UID = uint.Parse(split[1]);
                    else
                    {
                        if (!Boooths.ContainsKey(booth.UID))
                        {
                            Boooths.Add(booth.UID, booth);
                            booth = new booth();
                            booth.UID = uint.Parse(split[1]);
                        }
                    }
                }
                else if (split[0] == "CostType")
                {
                    booth.CostType = split[1];
                }
                else if (split[0] == "Type")
                {
                    booth.Type = (BoothType)byte.Parse(split[1]);
                }
                else if (split[0] == "Name")
                {
                    booth.Name = split[1];
                }
                else if (split[0] == "BotMessage")
                {
                    booth.BotMessage = split[1];
                }
                else if (split[0] == "Garment")
                {
                    booth.Garment = uint.Parse(split[1]);
                }
                else if (split[0] == "Head")
                {
                    booth.Head = uint.Parse(split[1]);
                }
                else if (split[0] == "WeaponR")
                {
                    booth.WeaponR = uint.Parse(split[1]);
                }
                else if (split[0] == "WeaponL")
                {
                    booth.WeaponL = uint.Parse(split[1]);
                }
                else if (split[0] == "Armor")
                {
                    booth.Armor = uint.Parse(split[1]);
                }
                else if (split[0] == "Mesh")
                {
                    booth.Mesh = ushort.Parse(split[1]);
                }
                else if (split[0] == "Map")
                {
                    booth.Map1 = ushort.Parse(split[1]);
                }
                else if (split[0] == "X")
                {
                    booth.X = ushort.Parse(split[1]);
                }
                else if (split[0] == "Y")
                {
                    booth.Y = ushort.Parse(split[1]);
                }
                else if (split[0] == "ItemAmount")
                {
                    booth.Items = new List<string>(ushort.Parse(split[1]));
                }
                else if (split[0].Contains("Item") && split[0] != "ItemAmount")
                {
                    string name = split[1];
                    booth.Items.Add(name);
                }
            }
            if (!Boooths.ContainsKey(booth.UID))
                Boooths.Add(booth.UID, booth);
            CreateBooths();
        }
        public static void CreateBooths()
        {
            foreach (var bo in Boooths.Values)
            {
                Game.Booth booth = new Game.Booth();
                SobNpc Base = new SobNpc();
                Base.UID = bo.UID;
                Base.ObjType = MapObjectType.SobNpc;
                Base.Mesh = (DeathWish.Role.SobNpc.StaticMesh)bo.Mesh;
                Base.Type = Role.Flags.NpcType.Booth;
                Base.Name = bo.Name;
                Base.Map = bo.Map1;
                Base.Booth = booth;
                Base.X = bo.X;
                Base.Y = bo.Y;
                booth.Base = Base;
                if (booth.Type == BoothType.Entity)
                {
                    
                    using (ServerSockets.RecycledPacket packet = new ServerSockets.RecycledPacket())
                    {
                        ServerSockets.Packet stream = packet.GetStream();
                        uint key = bo.UID + 1;
                        if (!DeathWish.Database.Server.GamePoll.ContainsKey(key))
                        {
                            GameClient client = new GameClient(null, false)
                            {
                                Fake = true
                            };
                            client.Player = new Player(client);
                            client.Inventory = new Inventory(client);
                            client.Equipment = new DeathWish.Role.Instance.Equip(client);

                            client.Warehouse = new Warehouse(client);
                            client.MyProfs = new Proficiency(client);
                            client.MySpells = new DeathWish.Role.Instance.Spell(client);
                            client.Achievement = new AchievementCollection();
                            client.Status = new MsgStatus();
                            client.Player.Name = Base.Name;
                            client.Player.Body = (ushort)bo.Body;
                            client.Player.Hair = (ushort)bo.HairStyle;
                            client.Player.HairColor = 3;
                            client.Player.Angle = (Flags.ConquerAngle)((byte)(bo.Mesh % 10));
                            client.Player.UID = key;
                            client.Status.MaxHitpoints = 0xea60;
                            client.Player.HitPoints = 0xea60;
                            client.Player.X = bo.X;
                            client.Player.Y = bo.Y;
                            client.Player.Map = bo.Map1;
                            client.Player.Level = 140;
                            client.Player.Action = (Flags.ConquerAction)bo.Action;
                            client.Map = Server.ServerMaps[Base.Map];
                            client.Map.Enquer(client);
                            client.Player.GarmentId = bo.Garment;
                            client.Player.RightWeaponId = bo.WeaponR;
                            client.Player.LeftWeaponId = bo.WeaponL;

                            Base.UID = bo.UID;
                            Base.Mesh = (SobNpc.StaticMesh)(((int)(SobNpc.StaticMesh.Vendor)) - 6 + ((int)(client.Player.Angle)));
                            if (client.Player.Angle == (Flags.ConquerAngle)2)
                                Base.X = (ushort)(bo.X - 1);
                            else if (client.Player.Angle == (Flags.ConquerAngle)4)
                                Base.Y = (ushort)(bo.Y - 1);
                            else
                                Base.X = (ushort)(bo.X + 1);
                            MsgMessage message = new MsgMessage(bo.BotMessage, "All", client.Player.Name, MsgMessage.MsgColor.pink, MsgMessage.ChatMode.HawkMessage);
                            booth.HawkMessage = message;
                            client.MyBooth = booth;
                        }
                    }
                }
                if (Server.ServerMaps.ContainsKey(Base.Map))
                {
                    Server.ServerMaps[Base.Map].View.EnterMap<Role.IMapObj>(Base);
                }

                for (int i = 0; i < bo.Items.Count; i++)
                {
                    var line = bo.Items[i].Split(new string[] { "@@", "@" }, StringSplitOptions.RemoveEmptyEntries);
                    Game.BoothItem item = new Game.BoothItem();

                    item.Item = new MsgGameItem();
                    item.Item.UID = MsgGameItem.ItemUID.Next;
                    item.Item.ITEM_ID = uint.Parse(line[0]);
                    if (line.Length >= 2)
                        item.Cost = uint.Parse(line[1]);
                    if (line.Length >= 3)
                        item.Item.Plus = byte.Parse(line[2]);
                    if (line.Length >= 4)
                        item.Item.Enchant = byte.Parse(line[3]);
                    if (line.Length >= 5)
                        item.Item.Bless = byte.Parse(line[4]);
                    if (line.Length >= 6)
                        item.Item.SocketOne = (Role.Flags.Gem)byte.Parse(line[5]);
                    if (line.Length >= 7)
                        item.Item.SocketTwo = (Role.Flags.Gem)byte.Parse(line[6]);
                    if (line.Length >= 8)
                        item.Item.StackSize = ushort.Parse(line[7]);

                    Database.ItemType.DBItem CIBI = new Database.ItemType.DBItem();
                    if (Database.Server.ItemsBase.TryGetValue(item.Item.ITEM_ID, out CIBI))
                    {
                        if (CIBI == null) break;
                        item.Item.Durability = CIBI.Durability;
                        item.Item.MaximDurability = CIBI.Durability;
                        if (bo.CostType == "Gold")
                            item.Cost_Type = MsgItemView.ActionMode.Gold;
                        else
                            item.Cost_Type = MsgItemView.ActionMode.CPs;
                        booth.ItemList.Add(item.Item.UID, item);
                    }

                }
            }
        }
    }
}
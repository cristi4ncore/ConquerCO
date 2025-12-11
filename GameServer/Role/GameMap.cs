using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using System.IO;
using DeathWish.Game.MsgServer;
using DeathWish.Game.MsgFloorItem;

namespace DeathWish.Role
{
    public class MapView
    {
        const int CELLS_PER_BLOCK = 32;

        private Extensions.Counter CounterMovement = new Extensions.Counter(1);

        public ViewPtr[,] m_setBlock;

        private int Width, Height;

        private int GetWidthOfBlock() { return (Width - 1) / CELLS_PER_BLOCK + 1; }
        private int GetHeightOfBlock() { return (Height - 1) / CELLS_PER_BLOCK + 1; }

        public MapView(int _Width, int _Height)
        {
            Width = _Width;
            Height = _Height;

            m_setBlock = new ViewPtr[GetWidthOfBlock(), GetHeightOfBlock()];
            for (int x = 0; x < GetWidthOfBlock(); x++)
                for (int y = 0; y < GetHeightOfBlock(); y++)
                    m_setBlock[x, y] = new ViewPtr();
        }

        private int Block(int nPos)
        {
            return nPos / CELLS_PER_BLOCK;
        }
        private ViewPtr BlockSet(int nPosX, int nPosY) { return m_setBlock[Block(nPosX), Block(nPosY)]; }

        public bool MoveTo<T>(T obj, int nNewPosX, int nNewPosY)
            where T : IMapObj
        {

            int nOldPosX = obj.X;
            int nOldPosY = obj.Y;
            if ((nOldPosX >= 0 && nOldPosX < Width) == false)
                return false;
            if ((nOldPosY >= 0 && nOldPosY < Height) == false)
                return false;
            if ((nNewPosX >= 0 && nNewPosX < Width) == false)
                return false;
            if ((nNewPosY >= 0 && nNewPosY < Height) == false)
                return false;

            if (Block(nOldPosX) == Block(nNewPosX) && Block(nOldPosY) == Block(nNewPosY))
                return false;

            BlockSet(nOldPosX, nOldPosY).RemoveObject<T>(obj);
            BlockSet(nNewPosX, nNewPosY).AddObject<T>(obj);

            if (obj.ObjType == MapObjectType.Player)
                obj.IndexInScreen = CounterMovement.Next;

            return true;
        }

        public bool EnterMap<T>(T obj)
            where T : IMapObj
        {
            if ((obj.X >= 0 && obj.X < Width) == false)
                return false;
            if ((obj.Y >= 0 && obj.Y < Height) == false)
                return false;

            BlockSet(obj.X, obj.Y).AddObject<T>(obj);

            if (obj.ObjType == MapObjectType.Player)
                obj.IndexInScreen = CounterMovement.Next;

            return true;
        }
        public bool LeaveMap<T>(T obj)
             where T : IMapObj
        {
            if ((obj.X >= 0 && obj.X < Width) == false)
                return false;
            if ((obj.Y >= 0 && obj.Y < Height) == false)
                return false;

            BlockSet(obj.X, obj.Y).RemoveObject<T>(obj);

            return true;
        }
        public IEnumerable<IMapObj> Roles(MapObjectType typ, int X, int Y, Predicate<IMapObj> P = null)
        {

            for (int x = Math.Max(Block(X) - 1, 0); x <= Block(X) + 1 && x < GetWidthOfBlock(); x++)
                for (int y = Math.Max(Block(Y) - 1, 0); y <= Block(Y) + 1 && y < GetHeightOfBlock(); y++)
                {
                    var list = m_setBlock[x, y].GetObjects(typ);
                    if (list != null)
                    {
                        for (int i = 0; i < list.Count; i++)
                        {
                            if (i >= list.Count)
                                break;
                            var element = list[i];
                            if (element != null)
                            {
                                if (P != null)
                                {
                                    if (P(element))
                                        yield return element;
                                }
                                else if (element != null)
                                    yield return element;
                            }
                        }
                    }
                }


        }
        public int CountRoles(MapObjectType typ, int X, int Y)
        {
            int count = 0;
            for (int x = Math.Max(Block(X) - 1, 0); x <= Block(X) + 1 && x < GetWidthOfBlock(); x++)
                for (int y = Math.Max(Block(Y) - 1, 0); y <= Block(Y) + 1 && y < GetHeightOfBlock(); y++)
                {
                    var list = m_setBlock[x, y].GetObjects(typ);
                    count += list.Count;
                }
            return count;
        }
        public IEnumerable<IMapObj> GetAllMapRoles(MapObjectType typ, Predicate<IMapObj> P = null)
        {
            for (int x = 0; x < GetWidthOfBlock(); x++)
                for (int y = 0; y < GetHeightOfBlock(); y++)
                {
                    var list = m_setBlock[x, y].GetObjects(typ);
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (i >= list.Count)
                            break;
                        var element = list[i];
                        if (element != null)
                        {
                            if (P != null)
                            {
                                if (P(element))
                                    yield return element;
                            }
                            else if (element != null)
                                yield return element;
                        }
                    }
                }
        }
        public int GetAllMapRolesCount(MapObjectType typ, Predicate<IMapObj> P = null)
        {
            return GetAllMapRoles(typ, P).Count();
        }
        public T GetMapObject<T>(MapObjectType typ, uint UID, Predicate<IMapObj> P = null)
        {
            foreach (var obj in GetAllMapRoles(typ, P))
                if (obj.UID == UID)
                    return (T)obj;
            return default(T);
        }
        public bool MapContain(MapObjectType typ, uint UID, Predicate<IMapObj> P = null)
        {
            foreach (var obj in GetAllMapRoles(typ, P))
                if (obj.UID == UID)
                    return true;
            return false;
        }
        public void ClearMap(MapObjectType typ)
        {
            for (int x = 0; x < GetWidthOfBlock(); x++)
                for (int y = 0; y < GetHeightOfBlock(); y++)
                {
                    m_setBlock[x, y].Clear(typ);
                }
        }
        public bool TryGetObject<T>(uint UID, MapObjectType typ, int X, int Y, out T obj)
            where T : IMapObj
        {
            for (int x = Math.Max(Block(X) - 1, 0); x <= Block(X) + 1 && x < GetWidthOfBlock(); x++)
                for (int y = Math.Max(Block(Y) - 1, 0); y <= Block(Y) + 1 && y < GetHeightOfBlock(); y++)
                {
                    var list = m_setBlock[x, y];
                    if (list.TryGetObject<T>(typ, UID, out obj))
                        return true;

                }
            obj = default(T);
            return false;
        }
        public bool Contain(uint UID, int X, int Y)
        {
            for (int x = Math.Max(Block(X) - 1, 0); x <= Block(X) + 1 && x < GetWidthOfBlock(); x++)
                for (int y = Math.Max(Block(Y) - 1, 0); y <= Block(Y) + 1 && y < GetHeightOfBlock(); y++)
                {
                    var list = m_setBlock[x, y];
                    for (int i = 0; i < (int)MapObjectType.Count; i++)
                        if (list.ContainObject((MapObjectType)i, UID))
                            return true;

                }
            return false;
        }
    }
    public class ViewPtr
    {
        private Extensions.MyList<Role.IMapObj>[] Objects;
        public ViewPtr()
        {
            Objects = new Extensions.MyList<IMapObj>[(int)MapObjectType.Count];
            for (int x = 0; x < (int)MapObjectType.Count; x++)
                Objects[x] = new Extensions.MyList<IMapObj>();
        }


        public void AddObject<T>(T obj)
             where T : IMapObj
        {

            Objects[(int)obj.ObjType].Add(obj);
        }

        public void RemoveObject<T>(T obj)
            where T : IMapObj
        {
            Objects[(int)obj.ObjType].Remove(obj);
        }


        public bool ContainObject(MapObjectType obj_t, uint UID)
        {
            for (int x = 0; x < Objects[(int)obj_t].Count; x++)
            {
                var list = Objects[(int)obj_t];
                if (x >= list.Count)
                    break;
                if (list[x].UID == UID)
                    return true;
            }
            return false;
        }

        public bool TryGetObject<T>(MapObjectType obj_t, uint UID, out T obj)
        {
            for (int x = 0; x < Objects[(int)obj_t].Count; x++)
            {
                var list = Objects[(int)obj_t];
                if (x >= list.Count)
                    break;
                if (list[x].UID == UID)
                {
                    obj = (T)list[x];
                    return true;
                }
            }
            obj = default(T);
            return false;
        }
        public Extensions.MyList<IMapObj> GetObjects(MapObjectType typ)
        {
            return Objects[(int)typ];
        }

        public void Clear(MapObjectType typ)
        {
            Objects[(int)typ].Clear();
        }
    }

    public class Portal
    {
        public ushort MapID { get; set; }
        public ushort X { get; set; }
        public ushort Y { get; set; }

        public ushort Destiantion_MapID { get; set; }
        public ushort Destiantion_X { get; set; }
        public ushort Destiantion_Y { get; set; }
    }
    [Flags]
    public enum MapFlagType : byte
    {
        None = 0,
        Valid = 1 << 0,
        Monster = 1 << 1,
        Item = 1 << 2,
        Player = 1 << 3

    }
    [Flags]
    public enum MapTypeFlags
    {
        Normal = 0,
        PkField = 1 << 0,
        ChangeMapDisable = 1 << 1,
        RecordDisable = 1 << 2,
        PkDisable = 1 << 3,
        BoothEnable = 1 << 4,
        TeamDisable = 1 << 5,
        TeleportDisable = 1 << 6,
        GuildMap = 1 << 7,
        PrisonMap = 1 << 8,
        FlyDisable = 1 << 9,
        Family = 1 << 10,
        MineEnable = 1 << 11,
        FreePk = 1 << 12,
        NeverWound = 1 << 13,
        DeadIsland = 1 << 14
    }
    public class GameMap
    {
        public static System.Collections.Concurrent.ConcurrentDictionary<uint, ushort> EventMaps = new System.Collections.Concurrent.ConcurrentDictionary<uint, ushort>();
        public uint RecordSteedRace = 0;

        public static sbyte[] XDir = new sbyte[] 
        { 
            -1, -2, -2, -1, 1, 2, 2, 1,
             0, -2, -2, -2, 0, 2, 2, 2, 
            -1, -2, -2, -1, 1, 2, 2, 1,
             0, -1, -1, -1, 0, 1, 1, 1,
        };
        public static sbyte[] YDir = new sbyte[] 
        {
            2,  1, -1, -2, -2, -1, 1, 2,
            2,  2,  0, -2, -2, -2, 0, 2, 
            2,  1, -1, -2, -2, -1, 1, 2, 
            1,  1,  0, -1, -1, -1, 0, 1
        };

        public static bool IsGate(uint UID)
        {
            return UID == 516076 || UID == 516077 || UID == 516074 || UID == 516974 || UID == 516075 || UID == 516975 || UID == 516078 || UID == 516079 || UID == 516080 ||UID == 201974 || UID == 201975 || UID == 211974 || UID == 211975 || UID == 212974 || UID == 212975 || UID == 200974 || UID == 200975;
        }
        public static bool IsFrozengrotoMaps(uint Map)
        {
            return Map == 1762 || Map == 1927 || Map == 1999 || Map == 2054 || Map == 2055 || Map == 2056;
        }
        public static bool IsMineCave(uint Map)
        {
            return Map == 1028;
        }
        private static List<ushort> UsingMaps = new List<ushort>()
        {
            601,//offlineTG
            700,//arena map, lotery map
            1000,//Desert
            1001,//MysticCastel
            1002,//TwinCity
            1004,//Prommoter
            1005,//Arena
            1006,//Steeding TC
            1008,//color you armors/heah
            1010,//bird vilage
            1011,//PhoenixCastle
            1013,//HalkingCave
            1015,//BirdIsland
            1020,//ApeMoutain
            1036,//Market
            //1039,//TrainingGrounds
            1511,//buy mobila
            1038,//GuildWar
            10550,
            2068,//elitepk map
            6001,//GuildWarJaill
            1098,1099,2080,601,3024,//house id`s
            1351,1352,1353,1354,//lab`s
            1762,//fg1
            1927,//fg2
            1999,//fg3
            2054,//fg4
            2055,//fg5
            2056,//fg6
            1858,//roulette
            3846,//Nemesys Map
            1700,//2nd reborn quest !!!
            3851,//epic ninja quest
            3055,//first map nemesys
            3056,//pestera
            3846,//nemesys map
            1039,//
            6000,//jail
            3825,//trojan epic quest
            2057,
            1987,1988,1989,1990,1991,1992,1993,1994,1995,1996,1997,1998,1999,2000,2001,2002,2003,2004,2005,
            Game.MsgTournaments.MsgClassPKWar.MapID,
            Game.MsgTournaments.MsgEliteGroup.WaitingAreaID,
            10137,
            10166
        };

        public List<Portal> Portals = new List<Portal>();

        public unsafe void SendSysMesage(string Messaj, Game.MsgServer.MsgMessage.ChatMode ChatType = Game.MsgServer.MsgMessage.ChatMode.TopLeft
           , Game.MsgServer.MsgMessage.MsgColor color = Game.MsgServer.MsgMessage.MsgColor.red)
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();

                var Packet = new Game.MsgServer.MsgMessage(Messaj, color, ChatType).GetArray(stream);
                foreach (var client in Users)
                    client.Send(Packet);
            }
        }

        public string Name = "";

        public uint BaseID = 0;
        public MapFlagType[,] cells { get; set; }
        public System.Drawing.Size bounds;
        public Game.MsgMonster.MobCollection MonstersColletion;

        public MapView View;

        public bool AddStaticRole(StaticRole role)
        {
            if (View.EnterMap<StaticRole>(role))
            {
                SetFlagNpc(role.X, role.Y);
                return true;
            }
            return false;
        }
        public bool RemoveStaticRole(Role.IMapObj obj)
        {

            if (View.LeaveMap<Role.IMapObj>(obj))
            {
                RemoveFlagNpc(obj.X, obj.Y);
                return true;
            }
            return false;
        }

        public Game.MsgNpc.Npc Magnolia = null;
        public void AddMagnolia(ServerSockets.Packet stream, uint Quality)
        {
            bool Location = false;

            if (Magnolia != null)
            {
                if (Magnolia.X == 99)
                    Location = true;
                RemoveNpc(Magnolia, stream);
            }
            Magnolia = Game.MsgNpc.Npc.Create();
            if (Location)
            {
                Magnolia.UID = 999900;
                Magnolia.X = 106;
                Magnolia.Y = 99;
            }
            else
            {
                Magnolia.UID = 999901;
                Magnolia.X = 99;
                Magnolia.Y = 112;
            }
            Magnolia.ObjType = MapObjectType.Npc;
            Magnolia.NpcType = Flags.NpcType.Talker;
            uint mesh = 0;
            if (Quality % 10 == 7)
                mesh = 10;
            else if (Quality % 10 == 8)
                mesh = 20;
            if (Quality % 10 == 9)
                mesh = 30;
            if (Quality % 10 == 0)
                mesh = 40;
            Magnolia.Mesh = (ushort)(19340 + mesh);
            Magnolia.Map = this.ID;
            AddNpc(Magnolia);
        }


        public void GenerateSectorTraps(ushort x, ushort y, int type)
        {
            if (View.CountRoles(MapObjectType.Item, x, y) < 6)
            {
                ushort newx = (ushort)Program.GetRandom.Next(1, 18);
                ushort newy = (ushort)Program.GetRandom.Next(1, 18);
                newx += x;
                newy += y;
                if (IsFlagPresent(newx, newy, MapFlagType.Item) == false && IsFlagPresent(newx, newy, MapFlagType.Valid))
                {
                    var Item = new Game.MsgFloorItem.MsgItem(null, newx, newy, Game.MsgFloorItem.MsgItem.ItemType.Effect, 0, 0, ID, 0, false, this, 60 * 60 * 1000);
                    Item.MsgFloor.m_ID = (uint)type;
                    Item.MsgFloor.m_Color = 2;
                    Item.MsgFloor.DropType = Game.MsgFloorItem.MsgDropID.Effect;
                    cells[newx, newy] |= MapFlagType.Item;
                    View.EnterMap<Role.IMapObj>(Item);


                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        Item.SendAll(stream, MsgDropID.Effect);
                    }
                }
            }
        }
        public void RemoveTrap(ushort x, ushort y, Role.IMapObj item)
        {

            View.LeaveMap<Role.IMapObj>(item);
            cells[item.X, item.Y] &= ~MapFlagType.Item;
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                var ittem = item as Game.MsgFloorItem.MsgItem;
                ittem.SendAll(stream, MsgDropID.RemoveEffect);
            }

        }
        public ConcurrentDictionary<uint, Game.MsgNpc.Npc> soldierRemains = new ConcurrentDictionary<uint, Game.MsgNpc.Npc>();
        public void CheckUpSoldierReamins(Extensions.Time32 Now)
        {
            List<Game.MsgNpc.Npc> remove = new List<Game.MsgNpc.Npc>();
            foreach (var npc in soldierRemains.Values)
            {
                if (ID == 1000)
                {
                    if (Now > npc.Respawn)
                    {
                        npc.X = (ushort)Program.GetRandom.Next(624 - 32, 624 + 32);
                        npc.Y = (ushort)Program.GetRandom.Next(477 - 32, 477 + 32);
                        AddNpc(npc);
                        remove.Add(npc);
                    }
                }
                else if (ID == 1015)
                {
                    if (npc.UID == 8551)
                    {
                        npc.X = (ushort)Program.GetRandom.Next(551 - 32, 551 + 32);
                        npc.Y = (ushort)Program.GetRandom.Next(342 - 32, 342 + 32);
                        AddNpc(npc);
                        remove.Add(npc);
                    }
                    else
                    {
                        npc.X = (ushort)Program.GetRandom.Next(454 - 90, 454 + 90);
                        npc.Y = (ushort)Program.GetRandom.Next(574 - 90, 574 + 90);
                        AddNpc(npc);
                        remove.Add(npc);
                    }
                }
            }
            foreach (var npc in remove)
            {
                Game.MsgNpc.Npc rem;
                soldierRemains.TryRemove(npc.UID, out rem);
            }
        }

        public void AddNpc(Game.MsgNpc.Npc npc)
        {
            if (!View.MapContain(MapObjectType.Npc, npc.UID))
            {
                View.EnterMap<Role.IMapObj>(npc);
                SetFlagNpc(npc.X, npc.Y);
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    npc.Send(stream);
                }
            }
        }
        public unsafe void RemoveNpc(Game.MsgNpc.Npc npc, ServerSockets.Packet stream)
        {
            if (View.MapContain(MapObjectType.Npc, npc.UID))
            {
                View.LeaveMap<Role.IMapObj>(npc);
                RemoveFlagNpc(npc.X, npc.Y);


                ActionQuery action;

                action = new ActionQuery()
                {
                    ObjId = npc.UID,
                    Type = ActionType.RemoveEntity
                };

                foreach (var client in View.Roles(MapObjectType.Player, npc.X, npc.Y))
                {
                    if (Core.GetDistance(client.X, client.Y, npc.X, npc.Y) <= Game.MsgNpc.Npc.SeedDistance)
                    {
                        client.Send(stream.ActionCreate(&action));
                    }
                }
            }

        }
        public bool ValidLocation(ushort X, ushort Y)
        {
            if (bounds.Width > X && this.bounds.Height > Y)
            {
                //     Console.WriteLine(cells[X, Y]);
                return (cells[X, Y] & MapFlagType.Valid) == MapFlagType.Valid;
            }
            return false;
        }
        public bool MonsterOnTile(ushort X, ushort Y)
        {
            if (bounds.Width > X && this.bounds.Height > Y)
            {
                return (cells[X, Y] & MapFlagType.Monster) == MapFlagType.Monster;
            }
            return false;
        }
        public void SetMonsterOnTile(ushort X, ushort Y, bool Value)
        {
            try
            {
                if (Value)
                    cells[X, Y] |= MapFlagType.Monster;
                else
                    cells[X, Y] &= ~MapFlagType.Monster;
            }
            catch (Exception e)
            {
                MyConsole.WriteException(e);
                MyConsole.WriteLine("Problem monsters on map " + ID.ToString());
            }
        }
        public bool SearchNpcInScreen(uint UID, ushort X, ushort Y, out  Game.MsgNpc.Npc obj)
        {
            if (View.TryGetObject<Game.MsgNpc.Npc>(UID, MapObjectType.Npc, X, Y, out obj))
            {
                return Core.GetDistance(X, Y, obj.X, obj.Y) < Game.MsgNpc.Npc.SeedDistance;
            }
            obj = default(Game.MsgNpc.Npc);
            return false;
        }


        public uint ID { get; private set; }
        public GameMap(int width, int height, int m_id)
        {
            Clients = new ConcurrentDictionary<uint, Client.GameClient>();
            this.cells = new MapFlagType[width, height];
            this.bounds = new System.Drawing.Size(width, height);

            this.ID = (uint)m_id;
        }

        public static Extensions.Counter DinamicIDS = new Extensions.Counter(10000001);

        public uint GenerateDynamicID()
        {
            return DinamicIDS.Next;
        }
        public ushort Reborn_Map = 0;
        public ushort Reborn_X = 0;
        public ushort Reborn_Y = 0;

        public static Dictionary<int, string> MapContents = new Dictionary<int, string>();
        public static bool CheckMap(uint ID)
        {
            if (!Database.Server.ServerMaps.ContainsKey(ID))
            {
                try
                {
                    LoadMap((int)ID, MapContents[(int)ID]);
                    return true;
                }
                catch (Exception)
                {
                }
            }
            return true;
        }
        public MapTypeFlags TypeStatus { get; set; }

        public static void LoadMaps()
        {
            using (var gamemap = new BinaryReader(new FileStream(Path.Combine(Program.ServerConfig.CO2Folder, "ini/gamemap.dat"), FileMode.Open)))
            {
                var amount = gamemap.ReadInt32();
                for (var i = 0; i < amount; i++)
                {

                    var id = gamemap.ReadInt32();
                    var fileName = Encoding.ASCII.GetString(gamemap.ReadBytes(gamemap.ReadInt32()));
                    var puzzleSize = gamemap.ReadInt32();
                    MapContents[id] = fileName.Replace(".7z", ".dmap");
                }
                foreach (var folded in MapContents)
                {
                    int id = folded.Key;
                    var mapFile = folded.Value;
                    LoadMap(id, mapFile);
                    if (folded.Key == 1780)
                    {
                        LoadMap(3581, mapFile, 1780);
                        LoadMap(3830, mapFile, 1780);
                        LoadMap(3831, mapFile, 1780);
                        LoadMap(3832, mapFile, 1780);
                        LoadMap(3834, mapFile, 1780);
                        LoadMap(3835, mapFile, 1780);
                    }
                    if (folded.Key == 6000)
                    {
                        LoadMap(7000, mapFile, 6000);
                    }
                    if (folded.Key == 1787)
                    {
                        LoadMap(9956, mapFile, 1787);
                    }
                    if (folded.Key == 1844)
                    {
                        LoadMap(4488, mapFile, 1844);
                    }
                    if (folded.Key == 3825)
                    {
                        LoadMap(3826, mapFile, 3825);
                        LoadMap(3827, mapFile, 3825);
                        LoadMap(3828, mapFile, 3825);
                        LoadMap(3829, mapFile, 3825);
                    }
                    if (folded.Key == 3030)
                    {
                        LoadMap(3032, mapFile, 3030);
                        LoadMap(3031, mapFile, 3030);
                        LoadMap(3036, mapFile, 3030);

                        LoadMap(3037, mapFile, 3030);
                        LoadMap(3038, mapFile, 3030);

                    }
                    if (folded.Key == 9972)
                    {
                        LoadMap(9852, mapFile, 9972);
                    }
                    if (folded.Key == 1036)
                    {
                        LoadMap(2036, mapFile, 1036);
                    }
                    if (folded.Key == 1013)
                    {
                        LoadMap(4253, mapFile, 1013);
                    }
                    if (folded.Key == 3825)
                        LoadMap(3833, mapFile, 3825);
                    if (folded.Key == 1765)
                        LoadMap(1818, mapFile, 1765);
                    if (folded.Key == 1082)
                        LoadMap(1052, mapFile, 1082);
                    if (folded.Key == 3838)
                    {
                        LoadMap(6522, mapFile, 3838);
                    }
                    if (folded.Key == 2071)
                    {
                        LoadMap(6521, mapFile, 2071);
                        LoadMap(6570, mapFile, 2071);
                        LoadMap(6625, mapFile, 2071);
                    }
                    if (folded.Key == 1004)
                    {
                        LoadMap(6072, mapFile, 1004);
                        LoadMap(1782, mapFile, 1004);
                        LoadMap(1783, mapFile, 1004);
                    }
                  
                    if(folded.Key == 3979)
                    {
                        LoadMap(5979, mapFile, 3979);
                        LoadMap(6979, mapFile, 3979);
                        LoadMap(7979, mapFile, 3979);
                        LoadMap(8989, mapFile, 3979);

                    }
                    if (folded.Key == 601)
                    {
                        LoadMap(1784, mapFile, 601);
                    }
                    /////
                    if (folded.Key == 1002)
                    {
                        LoadMap(2002, mapFile, 1002);
                    }
                    if (folded.Key == 1000)
                    {
                        LoadMap(2000, mapFile, 1000);
                    }
                    if (folded.Key == 1011)
                    {
                        LoadMap(2011, mapFile, 1011);
                    }
                    if (folded.Key == 1015)
                    {
                        LoadMap(2015, mapFile, 1015);
                    }
                    if (folded.Key == 1020)
                    {
                        LoadMap(2020, mapFile, 1020);
                    }
                    ////
                    if (folded.Key == 1028)
                    {
                        LoadMap(1794, mapFile, 1028);
                    }
                    if (folded.Key == 1014)
                        LoadMap(1792, mapFile, 1014);
                    if (folded.Key == 1765)
                        LoadMap(1791, mapFile, 1765);
                    if (folded.Key == 10088)
                    {
                        LoadMap(44455, mapFile, 10088);
                        LoadMap(44456, mapFile, 10088);
                        LoadMap(44457, mapFile, 10088);
                    }
                    if (folded.Key == 700)
                    {
                        LoadMap(9573, mapFile, 700);
                        LoadMap(2573, mapFile, 700);
                        LoadMap(2575, mapFile, 700);
                        LoadMap(7202, mapFile, 700);
                        LoadMap(7272, mapFile, 700);
                        LoadMap(7273, mapFile, 700);
                        LoadMap(7274, mapFile, 700);
                        LoadMap(7275, mapFile, 700);



                        LoadMap(1987, mapFile, 700);
                        LoadMap(1988, mapFile, 700);
                        LoadMap(1989, mapFile, 700);
                        LoadMap(1990, mapFile, 700);
                        LoadMap(1991, mapFile, 700);
                        LoadMap(1992, mapFile, 700);
                        LoadMap(1993, mapFile, 700);
                        LoadMap(1994, mapFile, 700);
                        LoadMap(1995, mapFile, 700);
                        LoadMap(1996, mapFile, 700);
                        LoadMap(1997, mapFile, 700);
                        LoadMap(1998, mapFile, 700);
                        LoadMap(1999, mapFile, 700);
                      //  LoadMap(2000, mapFile, 700);
                        LoadMap(2001, mapFile, 700);
                      //  LoadMap(2002, mapFile, 700);
                        LoadMap(2003, mapFile, 700);
                        LoadMap(2004, mapFile, 700);
                        LoadMap(2005, mapFile, 700);
                        LoadMap(5661, mapFile, 700);
                        LoadMap(8601, mapFile, 700);
                        LoadMap(8602, mapFile, 700);
                        LoadMap(8603, mapFile, 700);
                        LoadMap(8604, mapFile, 700);
                        LoadMap(8881, mapFile, 700);
                        LoadMap(8882, mapFile, 700);
                        LoadMap(8883, mapFile, 700);
                        LoadMap(8884, mapFile, 700);
                        LoadMap(8885, mapFile, 700);
                        LoadMap(8886, mapFile, 700);
                        LoadMap(8887, mapFile, 700);
                        LoadMap(8888, mapFile, 700);
                        LoadMap(8889, mapFile, 700);
                        LoadMap(8890, mapFile, 700);
                        LoadMap(8891, mapFile, 700);
                        LoadMap(8892, mapFile, 700);
                        LoadMap(59106, mapFile, 700);
                        LoadMap(8893, mapFile, 700);
                        LoadMap(8894, mapFile, 700);
                        LoadMap(8895, mapFile, 700);
                        LoadMap(8896, mapFile, 700);
                        LoadMap(8898, mapFile, 700);
                        ///
                        LoadMap(7701, mapFile, 700);
                        LoadMap(7702, mapFile, 700);
                        LoadMap(7703, mapFile, 700);
                        LoadMap(7704, mapFile, 700);
                        LoadMap(7705, mapFile, 700);
                        LoadMap(7706, mapFile, 700);
                        LoadMap(7707, mapFile, 700);
                        LoadMap(7708, mapFile, 700);
                        LoadMap(7709, mapFile, 700);
                        LoadMap(7710, mapFile, 700);
                        LoadMap(7711, mapFile, 700);
                        LoadMap(7712, mapFile, 700);
                        //
                        LoadMap(7721, mapFile, 700);
                        LoadMap(7722, mapFile, 700);
                        LoadMap(7723, mapFile, 700);
                        LoadMap(7724, mapFile, 700);
                        LoadMap(7725, mapFile, 700);
                        LoadMap(7726, mapFile, 700);
                        LoadMap(7727, mapFile, 700);
                        LoadMap(7728, mapFile, 700);
                        LoadMap(7729, mapFile, 700);
                        LoadMap(7730, mapFile, 700);
                        LoadMap(7731, mapFile, 700);
                        LoadMap(7732, mapFile, 700);
                        LoadMap(7777, mapFile, 700);


                    }
                    if (folded.Key == 1508)
                    {
                        LoadMap(1518, mapFile, 1508);
                    }
                    if (folded.Key == 3825)
                    {
                        LoadMap(2515, mapFile, 3825);
                        LoadMap(6004, mapFile, 3825);
                        LoadMap(3359, mapFile, 3825);
                    }
                    if (folded.Key == 3935)
                    {
                        LoadMap(30935, mapFile, 3935);
                        LoadMap(30936, mapFile, 3935);
                        LoadMap(30937, mapFile, 3935);
                    }
                    if (folded.Key == 1507)
                    {
                        LoadMap(6525, mapFile, 1507);
                        LoadMap(6526, mapFile, 1507);
                    }
                    if (folded.Key == 4000)
                    {
                        LoadMap(26391, mapFile, 4000);
                        LoadMap(26392, mapFile, 4000);
                        LoadMap(26393, mapFile, 4000);
                        LoadMap(26394, mapFile, 4000);
                        LoadMap(26395, mapFile, 4000);
                        LoadMap(26396, mapFile, 4000);
                        LoadMap(26397, mapFile, 4000);
                        LoadMap(26398, mapFile, 4000);
                        LoadMap(26399, mapFile, 4000);
                        LoadMap(26400, mapFile, 4000);
                        LoadMap(50016, mapFile, 4000);
                        LoadMap(50017, mapFile, 4000);
                        LoadMap(50018, mapFile, 4000);
                        LoadMap(50019, mapFile, 4000);

                        LoadMap(50100, mapFile, 4000);
                        LoadMap(50101, mapFile, 4000);
                        LoadMap(50102, mapFile, 4000);

                        LoadMap(50103, mapFile, 4000);

                        LoadMap(50104, mapFile, 4000);
                        LoadMap(50105, mapFile, 4000);

                        LoadMap(1483, mapFile, 4000);
                        LoadMap(1484, mapFile, 4000);
                        LoadMap(1485, mapFile, 4000);
                        LoadMap(1486, mapFile, 4000);
                        LoadMap(1487, mapFile, 4000);

                        LoadMap(50020, mapFile, 4000);
                        LoadMap(50021, mapFile, 4000);

                        LoadMap(26700, mapFile, 4000);
                        LoadMap(26701, mapFile, 4000);
                        LoadMap(26702, mapFile, 4000);
                        LoadMap(26703, mapFile, 4000);
                        LoadMap(22348, mapFile, 4000);
                        LoadMap(22340, mapFile, 4000);
                        LoadMap(22380, mapFile, 4000);

                        LoadMap(22381, mapFile, 4000);
                        LoadMap(22382, mapFile, 4000);

                        LoadMap(22383, mapFile, 4000);
                        LoadMap(22384, mapFile, 4000);

                        LoadMap(22385, mapFile, 4000);
                        LoadMap(22386, mapFile, 4000);

                        LoadMap(22387, mapFile, 4000);
                        LoadMap(22388, mapFile, 4000);

                        LoadMap(22389, mapFile, 4000);
                        LoadMap(22390, mapFile, 4000);

                        LoadMap(5053, mapFile, 4000);
                        LoadMap(5054, mapFile, 4000);
                        LoadMap(5055, mapFile, 4000);
                        LoadMap(5056, mapFile, 4000);
                        LoadMap(5057, mapFile, 4000);
                        LoadMap(5058, mapFile, 4000);

                        LoadMap(5066, mapFile, 4000);
                        LoadMap(5061, mapFile, 4000);
                        LoadMap(5062, mapFile, 4000);
                        LoadMap(5063, mapFile, 4000);
                        LoadMap(5064, mapFile, 4000);
                        LoadMap(5065, mapFile, 4000);
                    }
                    if (folded.Key == 1505)
                    {
                        LoadMap(50501, mapFile, 1505);
                        LoadMap(22341, mapFile, 1505);
                        LoadMap(22700, mapFile, 1505);
                        LoadMap(1222, mapFile, 1505);

                    }
                    if (folded.Key == 10090)
                    {
                        LoadMap(44460, mapFile, 10090);
                        LoadMap(44461, mapFile, 10090);
                        LoadMap(44462, mapFile, 10090);
                        LoadMap(44463, mapFile, 10090);
                    }
                }
                try
                {
                    gamemap.Close();
                }
                catch (Exception)
                {
                    MyConsole.WriteLine("Error Closing");
                }
            }


            MyConsole.WriteLine("Loaded " + Database.Server.ServerMaps.Count + " Maps");
            GC.Collect();

        }

        public uint MapColor = 0;




        public int[,] FloorType;
        public int[,] Altitude;
        public static void LoadMap(int id, string mapFile, uint baseid = 0)
        {
            try
            {

                if (Database.Server.ServerMaps.ContainsKey((ushort)id))
                {
                    MyConsole.WriteLine($"\tMapa ya existe: {id}");
                    return;
                }
                GameMap ourInst;
                using (var rdr = new BinaryReader(new FileStream(Path.Combine(Program.ServerConfig.CO2Folder, mapFile), FileMode.Open)))
                {

                    rdr.ReadBytes(268);
                    ourInst = new GameMap(rdr.ReadInt32(), rdr.ReadInt32(), id);
                    ourInst.MonstersColletion = new Game.MsgMonster.MobCollection((uint)id);
                    ourInst.View = new MapView(ourInst.bounds.Width, ourInst.bounds.Height);
                    ourInst.MonstersColletion = new Game.MsgMonster.MobCollection((uint)id);
                    ourInst.BaseID = baseid;
                    if (id == 1038)
                    {
                        ourInst.FloorType = new int[ourInst.bounds.Width, ourInst.bounds.Height];
                        ourInst.Altitude = new int[ourInst.bounds.Width, ourInst.bounds.Height];
                    }

                    for (int y = 0; y < ourInst.bounds.Height; y++)
                    {
                        for (int x = 0; x < ourInst.bounds.Width; x++)
                        {

                            ourInst.cells[x, y] = (rdr.ReadInt16() == 0) ? MapFlagType.Valid : MapFlagType.None;
                            if (id == 1038)
                            {
                                ourInst.FloorType[x, y] = rdr.ReadInt16();
                                ourInst.Altitude[x, y] = rdr.ReadInt16();
                            }
                            else
                            {
                                rdr.ReadInt16();
                                rdr.ReadInt16();
                            }

                        }
                        rdr.ReadInt32();
                    }
                }
                Database.Server.ServerMaps.Add((ushort)id, ourInst);
                int info = baseid != 0 ? (int)baseid : (int)id;

                if (File.Exists(Program.ServerConfig.DbLocation + "maps\\" + info + ".ini"))
                {
                    WindowsAPI.IniFile reader = new WindowsAPI.IniFile("\\maps\\" + info + ".ini");
                    ourInst.TypeStatus = (MapTypeFlags)reader.ReadUInt32("info", "type", 0);
                    ourInst.Reborn_X = reader.ReadUInt16("info", "portal0_x", 0);
                    ourInst.Reborn_Y = reader.ReadUInt16("info", "portal0_y", 0);
                    ourInst.Reborn_Map = reader.ReadUInt16("info", "reborn_map", 0);
                    ourInst.RecordSteedRace = reader.ReadUInt16("info", "race_record", 0);
                    ourInst.MapColor = reader.ReadUInt32("info", "color", 0);
                }
            }
            catch (FileNotFoundException)
            {
                MyConsole.WriteLine("\tMap not found: " + id + " - " + mapFile + "");
            }
            catch (Exception e)
            {
                MyConsole.WriteException(e);
            }
        }
        private bool Update = false;
        private Client.GameClient[] Users = new Client.GameClient[0];
        public Client.GameClient[] Values
        {
            get
            {
                if (Update)
                {
                    Users = Clients.Values.ToArray();
                    Update = false;
                }
                return Users;
            }
            set { }
        }
        private ConcurrentDictionary<uint, Client.GameClient> Clients;
        public void Enquer(Client.GameClient client)
        {
            if (Clients.TryAdd(client.Player.UID, client))
            {
                View.EnterMap<Role.IMapObj>(client.Player);
                client.Map = this;
                Update = true;
            }
        }
        public void Denquer(Client.GameClient client)
        {
            Client.GameClient aclient;
            if (Clients.TryRemove(client.Player.UID, out aclient))
            {
                View.LeaveMap<Role.IMapObj>(client.Player);

                Update = true;
            }
        }
        public void AddSobnpc(Role.SobNpc Sobnpc)
        {
            View.EnterMap<Role.IMapObj>(Sobnpc);

            if (Role.GameMap.IsGate(Sobnpc.UID))
                SetGateFlagNpc(Sobnpc.X, Sobnpc.Y);
            else
                SetFlagNpc(Sobnpc.X, Sobnpc.Y);
        }
        public unsafe void RemoveSobnpc(Role.SobNpc Sobnpc, ServerSockets.Packet stream)
        {
            View.LeaveMap<Role.IMapObj>(Sobnpc);
            RemoveFlagNpc(Sobnpc.X, Sobnpc.Y);
            ActionQuery action;
            action = new ActionQuery()
            {
                ObjId = Sobnpc.UID,
                Type = ActionType.RemoveEntity
            };
            Sobnpc.SendScrennPacket(stream.ActionCreate(&action));
        }
        public void RefreashSobnpc(Role.SobNpc Sobnpc, ServerSockets.Packet stream)
        {
            RemoveSobnpc(Sobnpc, stream);
            AddSobnpc(Sobnpc);
        }
        public void SetFlagNpc(ushort x, ushort y)
        {
            cells[x, y] = MapFlagType.None;

            ushort limy = (ushort)Math.Min(this.bounds.Height - 1, y + 1);
            ushort limx = (ushort)Math.Min(this.bounds.Width - 1, x + 1);
            ushort xstart = (ushort)Math.Max(x - 1, 0);

            for (ushort ay = (ushort)Math.Max(y - 1, 0); ay <= limy; ay++)
            {
                for (ushort ax = xstart; ax <= limx; ax++)
                {
                    cells[ax, ay] = MapFlagType.None;
                }
            }
        }
        public void SetGateFlagNpc(ushort x, ushort y)
        {
            cells[x, y] = MapFlagType.None;

            ushort limy = (ushort)Math.Min(this.bounds.Height - 2, y + 2);
            ushort limx = (ushort)Math.Min(this.bounds.Width - 2, x + 2);
            ushort xstart = (ushort)Math.Max(x - 2, 0);

            for (ushort ay = (ushort)Math.Max(y - 2, 0); ay <= limy; ay++)
            {
                for (ushort ax = xstart; ax <= limx; ax++)
                {
                    cells[ax, ay] = MapFlagType.None;
                }
            }
        }
        public void RemoveFlagNpc(ushort x, ushort y)
        {
            cells[x, y] = MapFlagType.Valid;

            ushort limy = (ushort)Math.Min(this.bounds.Height - 1, y + 1);
            ushort limx = (ushort)Math.Min(this.bounds.Width - 1, x + 1);
            ushort xstart = (ushort)Math.Max(x - 1, 0);

            for (ushort ay = (ushort)Math.Max(y - 1, 0); ay <= limy; ay++)
            {
                for (ushort ax = xstart; ax <= limx; ax++)
                {
                    cells[ax, ay] = MapFlagType.Valid;
                }
            }
        }
        public bool ContainMobID(uint ID, uint Dynamic = 0)
        {
            foreach (var monster in View.GetAllMapRoles(MapObjectType.Monster))
            {
                var mob = monster as Game.MsgMonster.MonsterRole;
                if (mob.Family != null)
                    if (mob.Family.ID == ID)
                    {
                        if (Dynamic == 0)
                            return true;
                        else
                            return Dynamic == monster.DynamicID;
                    }
            }
            return false;
        }

        public object SyncRoot = new object();
        public void GetRandCoord(ref ushort x, ref ushort y)
        {
            lock (SyncRoot)
            {
                do
                {
                    x = (ushort)Program.GetRandom.Next(20, (ushort)(bounds.Width - 1));
                    y = (ushort)Program.GetRandom.Next(20, (ushort)(bounds.Height - 1));
                }
                while ((cells[x, y] & MapFlagType.Valid) != MapFlagType.Valid);
            }
        }

        public bool IsFlagPresent(int x, int y, MapFlagType flag)
        {
            if (x > 0 && y > 0 && x < bounds.Width && y < bounds.Height)
                return (cells[x, y] & flag) == flag;
            return false;
        }
        public bool EnqueueItem(Game.MsgFloorItem.MsgItem item)
        {
            return View.EnterMap<Role.IMapObj>(item);
        }
        public bool IsValidFlagNpc(ushort x, ushort y)
        {
            ushort limy = (ushort)Math.Min(this.bounds.Height - 1, y + 1);
            ushort limx = (ushort)Math.Min(this.bounds.Width - 1, x + 1);
            ushort xstart = (ushort)Math.Max(x - 1, 0);

            for (ushort ay = (ushort)Math.Max(y - 1, 0); ay <= limy; ay++)
            {
                for (ushort ax = xstart; ax <= limx; ax++)
                {
                    if (!this.IsFlagPresent(x, y, MapFlagType.Valid))
                        return false;
                }
            }
            return true;
        }
        public bool AddGuildTeleporterItem(ref ushort x, ref ushort y)
        {
            if (IsValidFlagNpc(x, y))
            {
                ushort limy = (ushort)Math.Min(this.bounds.Height - 6, y + 6);
                ushort limx = (ushort)Math.Min(this.bounds.Width - 6, x + 6);
                ushort xstart = (ushort)Math.Max(x - 6, 0);
                ushort ystart = (ushort)Math.Max(y - 6, 0);

                for (ushort ay = ystart; ay <= limy; ay++)
                {
                    for (ushort ax = xstart; ax <= limx; ax++)
                    {
                        if (IsValidFlagNpc(ax, ay))
                        {
                            x = ax;
                            y = ay;

                            cells[ax, ay] |= MapFlagType.Item;

                            return true;
                        }
                    }
                }
                x = 0;
                y = 0;
                return false;
            }

            cells[x, y] |= MapFlagType.Item;
            return true;
        }
        public bool AddGroundItem(ref ushort x, ref ushort y, byte Range = 0)
        {
            if (this.IsFlagPresent(x, y, MapFlagType.Item) || !this.IsFlagPresent(x, y, MapFlagType.Valid))
            {
                ushort limy = (ushort)Math.Min(this.bounds.Height - (1 + Range), y + (1 + Range));
                ushort limx = (ushort)Math.Min(this.bounds.Width - (1 + Range), x + (1 + Range));
                ushort xstart = (ushort)Math.Max(x - (1 + Range), 0);
                ushort ystart = (ushort)Math.Max(y - (1 + Range), 0);

                for (ushort ay = ystart; ay <= limy; ay++)
                {
                    for (ushort ax = xstart; ax <= limx; ax++)
                    {
                        if (!this.IsFlagPresent(ax, ay, MapFlagType.Item))
                        {
                            if (this.IsFlagPresent(ax, ay, MapFlagType.Valid))
                            {
                                x = ax;
                                y = ay;

                                cells[ax, ay] |= MapFlagType.Item;

                                return true;
                            }
                        }
                    }
                }
                x = 0;
                y = 0;
                return false;
            }

            cells[x, y] |= MapFlagType.Item;
            return true;
        }
        public void RemoveCustom(Client.GameClient entity)
        {
            if (Clients.TryRemove(entity.Player.UID, out entity))
            {

            }
            if (Game.AISystem.AIBot.Pool.ContainsKey(entity.Player.UID))
            {
                Client.GameClient remov;
                Game.AISystem.AIBot.Pool.TryRemove(entity.Player.UID, out remov);
                View.LeaveMap<IMapObj>(entity.Player);
                Update = true;
            }
        }
        public void AddCustomHunt(Client.GameClient entity)
        {
            if (Clients.TryAdd(entity.Player.UID, entity))
            {
            }
            if (!Game.AISystem.AIBot.Pool.ContainsKey(entity.Player.UID))
            {
                Game.AISystem.AIBot.Pool.TryAdd(entity.Player.UID, entity);
                if (Database.Server.ServerMaps.TryGetValue(entity.Player.Map, out Role.GameMap map))
                {
                    entity.Map = map;
                    View.EnterMap<IMapObj>(entity.Player);
                }
                Update = true;
            }
            else
            {
                View.LeaveMap<IMapObj>(entity.Player);
                Game.AISystem.AIBot.Pool.TryAdd(entity.Player.UID, entity);
                View.EnterMap<IMapObj>(entity.Player);
            }
        }
        public void AddCustom(Client.GameClient entity)
        {
            if (Clients.TryAdd(entity.Player.UID, entity))
            {
            }
            if (!Game.AISystem.AIBot.Pool.ContainsKey(entity.Player.UID))
            {
                Game.AISystem.AIBot.Pool.TryAdd(entity.Player.UID, entity);
                if (Database.Server.ServerMaps.TryGetValue(entity.Player.Map, out Role.GameMap map))
                {
                    entity.Map = map;
                    View.EnterMap<IMapObj>(entity.Player);
                }
                Update = true;
            }
            else
            {
                View.LeaveMap<IMapObj>(entity.Player);
                Game.AISystem.AIBot.Pool.TryAdd(entity.Player.UID, entity);
                View.EnterMap<IMapObj>(entity.Player);
            }
        }
        public void RemoveAI(Client.GameClient entity)
        {
            if (Clients.TryRemove(entity.Player.UID, out entity))
            {
                Client.GameClient remov;
                Game.AISystem.AIBot.Pool.TryRemove(entity.Player.UID, out remov);
                View.LeaveMap<IMapObj>(entity.Player);
                Update = true;
            }
            if (Game.AISystem.AIBot.Pool.ContainsKey(entity.Player.UID - 60000000))
            {
                Client.GameClient remov;
                Game.AISystem.AIBot.Pool.TryRemove(entity.Player.UID, out remov);
                View.LeaveMap<IMapObj>(entity.Player);
                Update = true;
            }
        }
        public void AddAI(Client.GameClient entity)
        {
            if (Clients.TryAdd(entity.Player.UID, entity))
            {

            }
            if (!Game.AISystem.AIBot.Pool.ContainsKey(entity.Player.UID))
            {
                Game.AISystem.AIBot.Pool.TryAdd(entity.Player.UID, entity);
                if (Database.Server.ServerMaps.TryGetValue(entity.Player.Map, out Role.GameMap map))
                {
                    entity.Map = map;
                    View.EnterMap<IMapObj>(entity.Player);
                }
                Update = true;
            }
            else
            {
                View.LeaveMap<IMapObj>(entity.Player);
                Game.AISystem.AIBot.Pool.TryAdd(entity.Player.UID, entity);
                View.EnterMap<IMapObj>(entity.Player);
            }
        }
    }
}
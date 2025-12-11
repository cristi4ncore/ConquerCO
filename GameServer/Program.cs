using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using DeathWish.Cryptography;
using DeathWish.Game.MsgServer;

namespace DeathWish
{
    using DeathWish.Client;
    using DeathWish.MsgProtection;
    using DeathWish.Threading;
    using PacketInvoker = CachedAttributeInvocation<Action<Client.GameClient, ServerSockets.Packet>, PacketAttribute, ushort>;

    class Program
    {//
        public static string[] AtributesType =
           {
               "CriticalStrike",
               "SkillCriticalStrike",
               "Immunity",
               "Breakthrough",
               "Counteraction",
               "MaxLife",
               "AddAttack",
               "AddMagicAttack",
               "AddMagicDefense",
               "FinalAttack",
               "FinalMagicAttack",
               "FinalDefense",
               "FinalMagicDefense",
            };
        public static void ExitToTwin(GameClient player)
        {
            player.SendSysMesage("", MsgMessage.ChatMode.FirstRightCorner);
            switch (GetRandom.Next(14))
            {
                case 1: player.Teleport(424, 377, 1002, 0, true, true); break;
                case 2: player.Teleport(456, 357, 1002, 0, true, true); break;
                case 3: player.Teleport(445, 371, 1002, 0, true, true); break;
                case 4: player.Teleport(447, 383, 1002, 0, true, true); break;
                case 5: player.Teleport(442, 387, 1002, 0, true, true); break;
                case 6: player.Teleport(435, 387, 1002, 0, true, true); break;
                case 7: player.Teleport(431, 390, 1002, 0, true, true); break;
                case 8: player.Teleport(421, 381, 1002, 0, true, true); break;
                case 9: player.Teleport(423, 392, 1002, 0, true, true); break;
                case 10: player.Teleport(464, 377, 1002, 0, true, true); break;
                case 11: player.Teleport(458, 366, 1002, 0, true, true); break;
                case 12: player.Teleport(432, 357, 1002, 0, true, true); break;
                default: player.Teleport(438, 366, 1002, 0, true, true); break;
            }
        }
        public static Client.GameClient[] Values = new Client.GameClient[0];
        public static bool Nobility = false;
        public static ServerSockets.ThreadPool CallBack;
        public static Basic ServerCallback;
        public static Extensions.Time32 CurrentTime
        {
            get
            {
                return new Extensions.Time32();
            }
        }
        public static List<Game.MsgEvents.Events> Events = new List<Game.MsgEvents.Events>();
        public static MemoryCompressor MCompressor = new MemoryCompressor();
        public static int CpuUse = 0;
        public static bool CpuUsageTimer = true;
        public static bool TimeNbc = false;
        public static int WraithChance = 80;
        public static List<byte[]> LoadPackets = new List<byte[]>();
        public static bool OnMainternance = false;
        public static ServerSockets.SocketPoll SocketsGroup;
        public static List<uint> ProtectMapSpells = new List<uint>() { 1038 };
        public static List<uint> NoHP = new List<uint>() { 7777,2571, 5051, 5053, 5054, 5055, 5056, 5057, 5058 };
        public static List<uint> TreadeOrShop = new List<uint>() { 7777, 5050, 1858, 5051, 5052, 5053, 5054, 5055, 5056, 5057, 5058, 5059, 5060, 5061, 5062, 5063, 5064, 5065, 5066, 5067, 5068, 5069, 5070 };
        public static List<uint> Lutes = new List<uint>() { 2038, 1038, 3868 };
        public static List<uint> FBMap = new List<uint>() { 5051, 5053, 5054, 5055, 5056, 5057, 5058 };
        public static List<uint> ProhibitedItems = new List<uint>() { };
        public static Extensions.Counter ClientCounter = new Extensions.Counter(1000000);
        public static List<uint> Block_PATH_FINDING = new List<uint>() { 1038, 3868, };
        public static List<uint> MapCounterHits = new List<uint>() { 9573, 7000, 5052, 8889, 8890, 8893, 8894, 8895, 8896, 8891, 8892, 8881, 8882, 8883, 8884, 8885, 8886, 8887, 8888, 2578, 2579, 8601, 8603, 8604, 8602, 2571, 2575, 2567, 2572, 2568, 2569, 2570, 5061, 5062, 5063, 5064, 5065, 5066, 5053, 5054, 5055, 5056, 5057, 5058, 26391, 26392, 26393, 26394, 26395, 26396, 26397, 26398, 26399, 1005, 6000, 5051 };
        public static List<uint> RankableFamilyIds = new List<uint>() { 20300, 20160, 20070 };
        public static List<uint> NoDropItems = new List<uint>() { 7777, 7701, 7702, 7703, 7704, 7705, 7706, 7707, 7708, 7709, 7710, 7711, 7712, 7721, 7722, 7723, 7724, 7725, 7726, 7727, 7728, 7729, 7730, 7731, 7732, 9573, 5052, 3868, 8889, 8890, 8893, 8894, 8895, 8896, 8891, 8892, 8881, 8882, 8883, 8884, 8885, 8886, 8887, 8888, 8601, 2578, 2579, 8603, 8604, 8602, 5979, 6979, 1082, 7272, 7273, 7274, 7275, 7202, 7979, 9, 1858, 6526, 26400, 6522, 22340, 3051, 5061, 2575, 603, 2571, 2567, 2573, 2572, 2568, 2569, 2570, 5062, 5063, 5064, 5065, 5066, 5051, 5053, 5054, 5055, 5056, 5057, 5058, 5053, 5054, 5055, 5056, 5057, 5058, 1483, 1484, 1485, 1486, 1487, 6004, 6525, 2515, 1764, 700, 3954, 3820, 1987, 1988, 1989, 1990, 1991, 1992, 1993, 1994, 1995, 1996, 1997, 1998, 1999, 2000, 2001, 2002, 2003, 2004, 2005, 6525, 1038, 1002 };
        public static List<uint> FreePkMap = new List<uint>() { 7777, 7701, 7702, 7703, 7704, 7705, 7706, 7707, 7708, 7709, 7710, 7711, 7712, 7721, 7722, 7723, 7724, 7725, 7726, 7727, 7728, 7729, 7730, 7731, 7732, 9573, 7000, 6625, 2578, 8889, 8890, 8893, 8894, 8895, 8896, 8891, 8892, 8881, 8882, 8883, 8884, 8885, 8886, 8887, 8888, 2579, 8601, 8602, 8603, 8604, 1505, 7202, 7272, 7273, 7274, 7275, 1082, 5979, 6979, 7979, 1506, 1507, 1508, 1509, 5052, 9, 26400, 6526, 1764, 6522, 22340, 1507, 5061, 2575, 603, 2571, 2567, 2573, 2572, 2568, 2569, 2570, 5062, 5063, 5064, 5065, 5066, 5053, 5054, 5055, 5056, 5057, 5058, 50104, 50105, 50100, 50101, 50102, 50103, 50018, 50019, 50020, 50021, 1483, 1484, 1485, 1486, 1487, 6525, 1518, 2515, 50016, 50017, 5661, 26391, 3581, 26392, 26393, 6570, 6521, 10550, 26394, 26395, 26396, 26397, 26398, 26399, 3998, 3071, 5051, 6000, 6001, 1505, 1005, 1038, 700, 1508, 1987, 1988, 1989, 1990, 1991, 1992, 1993, 1994, 1995, 1996, 1997, 1998, 1999, 2000, 2001, 2002, 2003, 2004, 2005/*PkWar*/, 3868, Game.MsgTournaments.MsgCaptureTheFlag.MapID, Game.MsgTournaments.MsgTeamDeathMatch.MapID };
        public static List<uint> BlockAttackMap = new List<uint>() {1004, 59106, 20000, 1860, 9852, 1222, 3032, 3036, 3037, 3038, 3071, 3051, 1351, 3031, 26391, 26392, 26393, 26394, 26395, 3030, 3831, 3832, 3820, 1035, 6004, 3835, 3825, 3830, 3831, 3832, 3834, 3826, 3827, 3828, 3829, 3833, 9995, 1068, 4020, 4000, 4003, 4006, 4008, 4009, 1858, 1801, 1780, 1779/*Ghost Map*/, 9972, 1806, 1002, 3954, 3081, 1036, 1004, 1008, 601, 1006, 1511, 1039, 700, Game.MsgTournaments.MsgEliteGroup.WaitingAreaID, (uint)Game.MsgTournaments.MsgSteedRace.Maps.DungeonRace, (uint)Game.MsgTournaments.MsgSteedRace.Maps.IceRace, (uint)Game.MsgTournaments.MsgSteedRace.Maps.IslandRace, (uint)Game.MsgTournaments.MsgSteedRace.Maps.LavaRace, (uint)Game.MsgTournaments.MsgSteedRace.Maps.MarketRace };
        public static List<uint> BlockTeleportMap = new List<uint>() { 5050, 1860, 5051, 5052, 5053, 5054, 5055, 5056, 5057, 5058, 5059, 5060, 5061, 5062, 5063, 5064, 5065, 5066, 5067, 5068, 5069, 5070, 7000, 50104, 50105, 50100, 50101, 50102, 50103, 50018, 50019, 50020, 50021, 1483, 1484, 1485, 1486, 1487, 601, 50016, 50017, 5661, 6004, 3581, 6000, 6001, 1005, 700, 1858, 3852, Game.MsgTournaments.MsgEliteGroup.WaitingAreaID, 1768 };
        public static List<uint> DisconnectMap = new List<uint>() { 7777, 7701, 7702, 7703, 7704, 7705, 7706, 7707, 7708, 7709, 7710, 7711, 7712, 7721, 7722, 7723, 7724, 7725, 7726, 7727, 7728, 7729, 7730, 7731, 7732, 9573, 4009, 4008, 4006, 4003, 4000, 1616, 3852, 5052, 5054, 5056, 5058, 10088, 50104, 50100, 44455, 44460, 5053, 5055, 5057, 10090, 10089, 50105, 50101, 3581, 3868, 3071, 6526, 4253, 3032, 5661, 50016, 6525, 6521, 6570, 8709, 5979, 6979, 7979, 700, 1505, 6525, 2515, 1991, 1992, 1993, 1994, 1995, 1997, 1998, 1999, 2000, 2005, 1989, 1990, 1988, 1987, 3868, 10137, 1518, 5050, 5051, 5052, 5053, 5054, 5055, 5056, 5057, 5058, 5059, 5060, 5061, 2567, 8602, 8603, 8604, 8893, 8898, 8894, 8895, 8896, 8601, 26400, 22340, 6522, 1764, 3030, 3071, 3032, 4253, 3593, 6625, 3594, 3595, 9467, 2571, 603, 2575, 7202, 7272, 8881, 8882, 8883, 8884, 8885, 8886, 8887, 8888, 8889, 8890, 8891, 8892, 7273, 7274, 7275, 1082, 2568, 9, 2572, 2573, 2569, 2570, 2578, 2579, 5062, 5063, 5064, 5065, 5066, 5067, 5068, 5069, 5070, 5071, 1483, 1484, 1485, 1486, 1487, 1017, 1017, 1081, 2060, 9972, 1080, 3820, 3954, 1806, 1508, Game.MsgTournaments.MsgTeamDeathMatch.MapID, 1768, Game.MsgTournaments.MsgFootball.MapID, 1505, 1506, 1509, 1508, 1507, 1801, 1780, 1779, 3071, 4253, 3032, 1068, 3830, 3831, 3832, 3834, 3826, 3827, 3828, 3829, 3833, 3825, 1518, 1508, 4745, 5051, 50102, 50103, 44456, 44457, 1518, 1508, 50018, 50019, 50016, 50020, 50021, 50017, 1038, 44461, 44462, 44463, 1860, 1858 };
        public static List<uint> RemoveRide = new List<uint>() { 7777, 7701, 7702, 7703, 7704, 7705, 7706, 7707, 7708, 7709, 7710, 7711, 7712, 7721, 7722, 7723, 7724, 7725, 7726, 7727, 7728, 7729, 7730, 7731, 7732, 9573, 5066, 5051, 5052, 5053, 5054, 5055, 5056, 5057, 5058, 1005, 26392, 1858, 700, 1004, 5061, 1082, 8604, 8603, 8602, 8601, 2567, 3030, 2571, 603, 2575, 7202, 7272, 7273, 7274, 7275, 2568, 2572, 2573, 2569, 2578, 2579, 2570, 9, 5062, 5063, 5064, 5065, 22382, 22389, 22385, 22388, 22384, 22383, 22380, 22381, 26700, 26701, 26702, 22386, 22387, 5661, 1487, 1486, 1483, 1484, 1485, 6526, 2515, 6525, 700, 3820, 1508, 1518, 50016, 50100, 50101, 50102, 50103, 50018, 50019, 50020, 50021, 50104, 50105, 50017, 22340, 22341, 6522, 8881, 1860, 8892, 8898, 8889, 8890, 8891, 8893, 8894, 8895, 8896, 8882, 8883, 8884, 8885, 8886, 8887, 8888, };
        public static List<uint> FBSSAuto = new List<uint>() { 7777, 7701, 7702, 7703, 7704, 7705, 7706, 7707, 7708, 7709, 7710, 7711, 7712, 8602, 8601, 2567, 603, 2575, 2569, 2579, 2571 };
        public static List<uint> BlockReflect = new List<uint>() { 7777, 7701, 7702, 7703, 7704, 7705, 7706, 7707, 7708, 7709, 7710, 7711, 7712, 9573, 2579, 8604, 8603, 8601, 8602, 2567, 2569, 603, 2575, 2571 };
        public static List<uint> FBSSOnlyAllowed = new List<uint>() { 7777, 7701, 7702, 7703, 7704, 7705, 7706, 7707, 7708, 7709, 7710, 7711, 7712, 9573, 8602, 8601, 2567, 603, 2575, 2571 };
        public static List<uint> ReviveTwinCity = new List<uint>() { 7777, 7701, 7702, 7703, 7704, 7705, 7706, 7707, 7708, 7709, 7710, 7711, 7712, 7721, 7722, 7723, 7724, 7725, 7726, 7727, 7728, 7729, 7730, 7731, 7732, 9573, 1764, 5979, 6979, 7979, 5661, 1002, 8896, 8895, 8898, 8893, 8894, 8889, 8890, 8891, 8892, 8881, 8882, 8883, 8884, 8885, 8886, 8887, 8888, 6522, 3846, 1762, 4253, 2056, 7272, 7273, 7274, 7275, 1082, 7202, 1508, 2571, 2572, 700, 2573, 603, 2575, 6525, 6526, 26400, 22340, 6522 };
        public static List<uint> BlockReviveHere = new List<uint>() { 7777, 7701, 7702, 7703, 7704, 7705, 7706, 7707, 7708, 7709, 7710, 7711, 7712, 7721, 7722, 7723, 7724, 7725, 7726, 7727, 7728, 7729, 7730, 7731, 7732, 9573, 2038, 1038, Game.MsgTournaments.MsgClassPKWar.MapID, Game.MsgTournaments.MsgCaptureTheFlag.MapID, Game.MsgTournaments.MsgTeamDeathMatch.MapID, 2572, 700, 2573, 603, 2575, 1082, 7202, 7272, 8889, 8890, 8891, 8893, 8898, 8894, 8895, 8896, 8892, 8881, 6625, 8882, 8883, 8884, 8885, 8886, 8887, 8888, 6522, 7273, 7274, 7275, 2571, 6525, 6526, 26400, 22340, 6522, 6521, 3868, 26391, 5661, 26393, 26394, 26395, 26396, 26397, 26398, 26399, 5661, 5979, 6979, 7979, 10137 };
        public static List<uint> BlockArenaMaps = new List<uint>() { 7777, 7701, 7702, 7703, 7704, 7705, 7706, 7707, 7708, 7709, 7710, 7711, 7712, 7721, 7722, 7723, 7724, 7725, 7726, 7727, 7728, 7729, 7730, 7731, 7732, 9573, 22389, 22382, 1505, 1506, 1507, 1508, 1509, 22385, 4253, 22384, 22388, 1082, 3593, 7272, 7273, 7274, 7275, 3594, 3051, 22340, 3032, 8898, 3071, 3595, 9467, 22383, 3846, 5979, 7202, 6979, 7979, 1762, 4253, 2056, 2055, 8889, 8890, 8892, 6625, 8893, 8894, 8895, 8896, 8891, 8601, 8603, 8604, 8602, 6525, 6526, 2578, 2579, 22380, 8881, 8882, 8883, 8884, 8885, 8886, 8887, 8888, 3030, 22381, 2575, 603, 2571, 2567, 2573, 2572, 9, 2568, 2569, 2570, 26700, 26701, 26702, 22386, 22387, 1507, 5661, 1505, 7000, 5052, 5061, 5062, 5063, 5064, 5065, 5066, 5053, 5054, 5055, 5056, 5057, 5058, 5051, 50104, 50105, 50100, 50101, 50102, 50103, 50018, 50019, 50020, 50021, 1483, 1484, 1485, 1486, 1487, 10137, 3581, 5661, 1518, 6525, 6570, 6521, 1508, 50016, 50017, 8709, 700, 1505, 6525, 2515, 1991, 1992, 1993, 1994, 1995, 1996, 1997, 1998, 1999, 2000, 2005, 1989, 1990, 1988, 1987, 3868, 2057, 3071, 1989, 1990, 1991, 1992, 1993, 1994, 1995, 1996, 1997, 1998, 1999, 2000, 2001, 2002, 2003, 2004, 2005, 1988, 24020, 4000, 4003, 4006, 4008, 4009, 6000, 6001, 1017, 1080, 1081, 2060, 6002, 6003, 601, 700, 3868, 1038, Game.MsgTournaments.MsgClassPKWar.MapID, 1036, 1764, Game.MsgTournaments.MsgEliteGroup.WaitingAreaID, Game.MsgTournaments.MsgSuperGuildWar.MapID };
        public static List<uint> BlockWatch = new List<uint>() { 7777, 7701, 7702, 7703, 7704, 7705, 7706, 7707, 7708, 7709, 7710, 7711, 7712, 7721, 7722, 7723, 7724, 7725, 7726, 7727, 7728, 7729, 7730, 7731, 7732, 9573, 700, 1487, 5661, 1486, 1483, 1484, 1485, 6526, 6522, 22341, 22340, 26400, 2515, 6526, 6521, 6525, 6570, 2515, 6525, 3581, 50017, 50105, 50104, 50021, 50020, 50019, 50018, 50103, 50102, 50021, 50101, 50100, 50016, 1507, 1508, 1518, 602, 603, 2575, 7202, 7272, 7273, 7274, 7275, 1082, 503, 9, 33, 3051, 3030, 3071, 4253, 3032, 2567, 8601, 8602, 8603, 8604, 1505, 1506, 1507, 1508, 1509, 5979, 6979, 7979, 8889, 8890, 8891, 8893, 8898, 8894, 8895, 8896, 8892, 8881, 8882, 8883, 8884, 8885, 8886, 8887, 8888, 2568, 2569, 2570, 2579, 2578, 6625, 2571, 2572, 3593, 3595, 9467, 1764, 1038, 3868, 3594 };
        public static List<uint> BlockMessageBox = new List<uint>() { 7777, 7701, 7702, 7703, 7704, 7705, 7706, 7707, 7708, 7709, 7710, 7711, 7712, 7721, 7722, 7723, 7724, 7725, 7726, 7727, 7728, 7729, 7730, 7731, 7732, 9573, 5979, 6979, 7979, 5061, 2567, 8601, 8602, 8603, 8604, 1764, 2568, 1038, 3868, 2068, 8881, 8889, 8890, 8891, 8896, 8894, 8895, 8893, 8898, 8892, 8882, 8883, 8884, 8885, 8886, 8887, 8888, 9, 3051, 2572, 2573, 2571, 2575, 7202, 7272, 7273, 7274, 7275, 1082, 603, 2569, 2570, 2578, 6625, 2579, 700, 5062, 5063, 5064, 5065, 5066, 5051, 5052, 5053, 5054, 5055, 5056, 5057, 5058, 2038, 22341, 1507, 22340, 26400, 50016, 50100, 50101, 50102, 50020, 50021, 50018, 50019, 50021, 50104, 50105, 1518, 1508, 50017, 3581, 6570, 6521, 2515, 6526, 1485, 1484, 1483, 1486, 5661, 700, 1487, 3868, 1038, 22389, 22382, 22385, 22384, 22387, 22386, 26702, 26701, 26700, 22381, 22380, 22383, 22388 };
        public static List<uint> BlockInvitation = new List<uint>() { 7777, 7701, 7702, 7703, 7704, 7705, 7706, 7707, 7708, 7709, 7710, 7711, 7712, 7721, 7722, 7723, 7724, 7725, 7726, 7727, 7728, 7729, 7730, 7731, 7732, 9573, 5061, 2567, 3030, 8601, 8602, 8603, 8604, 5979, 6979, 7979, 1764, 2571, 2068, 2575, 7202, 7272, 7273, 7274, 7275, 1082, 2569, 2568, 1860, 9, 8881, 8882, 8883, 8884, 8885, 8886, 8887, 8888, 1505, 1038, 2038, 8891, 8893, 8898, 8894, 8895, 8896, 8892, 8890, 8889, 1506, 1507, 1508, 1509, 3051, 700, 2572, 2573, 2570, 2578, 6625, 2579, 5062, 5063, 5064, 5065, 5066, 5051, 5052, 5053, 5054, 5055, 5056, 5057, 5058, 22341, 22340, 26400, 50016, 50100, 50101, 50102, 50020, 50021, 50018, 50019, 50021, 50104, 50105, 1518, 1508, 50017, 3581, 6570, 6521, 2515, 6526, 1485, 1484, 1483, 1486, 5661, 700, 1487, 3868, 2038, 1038, 22389, 22382, 22385, 22384, 22387, 22386, 26702, 26701, 26700, 22381, 22380, 22383, 22388 };
        public static List<uint> BlockOffline = new List<uint>() { 7777, 7701, 7702, 7703, 7704, 7705, 7706, 7707, 7708, 7709, 7710, 7711, 7712, 7721, 7722, 7723, 7724, 7725, 7726, 7727, 7728, 7729, 7730, 7731, 7732, 9573, 1038, 3868, 1017, Game.MsgTournaments.MsgFootball.MapID, Game.MsgTournaments.MsgClassPKWar.MapID, Game.MsgTournaments.MsgTeamDeathMatch.MapID, 1081, 2060, 9972, 1080, 3820, 3954, 1806, 1508, 1768, 5051, 5052, 5053, 5054, 5055, 5056, 5057, 5058, 5979, 6979, 7979, 5061, 4253, 8604, 8603, 8602, 8601, 3071, 3032, 2567, 7272, 7273, 8889, 8890, 8891, 8898, 8893, 8894, 8895, 8896, 8892, 8881, 8882, 8883, 8884, 8885, 8886, 8887, 8888, 7274, 7275, 1764, 3051, 3030, 2571, 603, 2575, 7202, 1082, 2568, 2573, 6625, 9, 2578, 2579, 2572, 2569, 2570, 5062, 5063, 5064, 5065, 5066, 1505, 1506, 1509, 1508, 1507 };
        public static List<uint> BlockEquip = new List<uint>() { 7777, 7701, 7702, 7703, 7704, 7705, 7706, 7707, 7708, 7709, 7710, 7711, 7712, 8603, 8604, 8602, 8601, 2567, 2571, 2579, 2569, 2575, 603, 1616 };
        public static List<uint> ScreenFBRoom = new List<uint>() { 7701, 7702, 7703, 7704, 7705, 7706, 7707, 7708, 7709, 7710, 7711, 7712, 2567, 2569, 2579 };
        public static Role.Instance.Nobility.NobilityRanking NobilityRanking = new Role.Instance.Nobility.NobilityRanking();
        public static Role.Instance.ChiRank ChiRanking = new Role.Instance.ChiRank();
        public static Role.Instance.Flowers.FlowersRankingToday FlowersRankToday = new Role.Instance.Flowers.FlowersRankingToday();
        public static Role.Instance.Flowers.FlowerRanking GirlsFlowersRanking = new Role.Instance.Flowers.FlowerRanking();
        public static Role.Instance.Flowers.FlowerRanking BoysFlowersRanking = new Role.Instance.Flowers.FlowerRanking(false);
        public static ShowChatItems GlobalItems;
        public static SendGlobalPacket SendGlobalPackets;
        public static PacketInvoker MsgInvoker;
        public static ServerSockets.ServerSocket GameServer;
        [DllImport("kernel32.dll")]
        private static extern bool SetConsoleCtrlHandler(ConsoleHandlerDelegate handler, bool add);
        private delegate bool ConsoleHandlerDelegate(int type);
        private static ConsoleHandlerDelegate handlerKeepAlive;
        public static bool ProcessConsoleEvent(int type)
        {
            try
            {
                if (ServerConfig.IsInterServer)
                {
                    foreach (var client in Database.Server.GamePoll.Values)
                    {
                        try
                        {
                            if (client.Socket != null)
                                client.Socket.Disconnect();
                        }
                        catch (Exception e)
                        {
                            MyConsole.WriteLine(e.ToString());
                        }
                    }
                    return true;
                }
                try
                {
                    if (GameServer != null)
                        GameServer.Close();
                }
                catch (Exception e) { MyConsole.SaveException(e); }
                MyConsole.WriteLine("Saving Database ...");
                foreach (var client in Database.Server.GamePoll.Values)
                {
                    try
                    {
                        if (client.Socket != null)
                            client.Socket.Disconnect();
                    }
                    catch (Exception e)
                    {
                        MyConsole.WriteLine(e.ToString());
                    }
                }
                Role.Instance.Clan.ProcessChangeNames();
                Role.Instance.Guild.ProcessChangeNames();
                Database.Server.SaveDatabase();
                if (Database.ServerDatabase.LoginQueue.Finish())
                {
                    System.Threading.Thread.Sleep(1000);
                    MyConsole.WriteLine("Database Save Succefull.");
                }
            }
            catch (Exception e)
            {
                MyConsole.SaveException(e);
            }
            return true;
        }
        public static DateTime LastServerPulse, LastSavePulse, LastGuildPulse;
        public static Extensions.Time32 SaveServerDatabase;
        public static Extensions.Time32 UpdateServerStatus = Extensions.Time32.Now;
        public static Extensions.Time32 ResetRandom = new Extensions.Time32();
        public static Extensions.SafeRandom GetRandom = new Extensions.SafeRandom();
        public static Extensions.RandomLite LiteRandom = new Extensions.RandomLite();
        public static class ServerConfig
        {
            public static string CO2Folder = "";
            public static string Chatbox = "";
            public static string ChangePassword = "";
            public static string StorePage = "";
            public static string XtremeTopLink = "https://www.facebook.com/abo.adly.12382";
            public static string FBStoreLink = "https://www.facebook.com/abo.adly.12382";
            public static string RedeemLink = "https://www.facebook.com/abo.adly.12382";
            public static uint ServerID = 0;
            public static string IPAddres = "178.162.214.243";
            public static ushort AuthPort = 9960;
            public static ushort GamePort = 5818;
            public static string ServerName = "";
            public static string ServerShadowClone = "DIABLO";
            public static string OfficialWebSite = "diablo-co.com";
            //InternetPort
            public static ushort Port_BackLog;
            public static ushort Port_ReceiveSize = 16384;
            public static ushort Port_SendSize = 16384;
            //Database
            public static string DbLocation = "";
            //WebServer
            public static ushort WebPort = 9900;
            public static string AccServerIPAddres = "178.162.214.243";

            public static string LoaderIP = "178.162.214.243";
            public static ushort LoaderPort = 9901;

            public static uint ExpRateSpell = 2000;
            public static uint ExpRateProf = 2000;
            public static uint UserExpRate = 1999999999;
            public static uint PhysicalDamage = 100;
            //interServer
            public static string InterServerAddress = "178.162.214.243";
            public static ushort InterServerPort = 0;
            public static bool IsInterServer = false;
            //public static bool testserver = false;
        }
        public static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Console.WriteLine(e.ToString());
        }
        static int CutTrail(int x, int y) { return (x >= y) ? x : y; }
        static int AdjustDefence(int nDef, int power2, int bless)
        {
            int nAddDef = 0;
            nAddDef += Game.MsgServer.AttackHandler.Calculate.Base.MulDiv(nDef, 100 - power2, 100) - nDef;
            return Game.MsgServer.AttackHandler.Calculate.Base.MulDiv(nDef + nAddDef, 100 - power2, 100);
        }
        public static void TESTT()
        {
            double base_d_factor = 130;
            double scaled_d_factor = 0.5;
            double dif = 139500 - 25000;
            double sign_dif = Math.Sign(dif);
            double scale = 1.0 + (-1.0 / (sign_dif + dif / (base_d_factor + 25000 * scaled_d_factor)) + sign_dif);
            double ttt = 139500 * scale;
        }
        public class sorine
        {
            public uint uid = 333;
        }
        static byte[] DecryptString(char[] str)
        {
            int i = 0;
            byte[] nstr = new byte[1000];
            do
            {
                nstr[i] = Convert.ToByte(str[i + 1] ^ 0x34);
            } while (nstr[i++] != 0);
            return nstr;
        }
        public static void writetext(string tes99)
        {
            char[] tg = new char[tes99.Length];
            for (int x = 0; x < tes99.Length; x++)
                tg[x] = tes99[x];
            var hhhh = DecryptString(tg);
            Console.WriteLine(ASCIIEncoding.ASCII.GetString(hhhh));
        }
        public static int n = 0;
        public static int sol;
        public static int[] v = new int[100];
        public static void afisare()
        {
            Console.WriteLine("");
            int i;
            sol++;
            Console.WriteLine("sol: " + sol);
            for (i = 1; i <= n; i++)
            {
                Console.Write(v[i] + " ");
            }
            Console.Write(Environment.NewLine);
        }
        public static int valid(int k)
        {
            int i;
            for (i = 1; i <= k - 1; i++)
                if ((v[k] <= v[i]))
                    return 0;
            return 1;
        }
        public static int solutie(int k)
        {
            if (k == n)
                return 1;
            return 0;
        }
        public static void BK(int k)
        {
            for (int i = 1; i <= n; i++)
            {
                v[k] = i;
                if (valid(k) == 1)
                {
                    if (solutie(k) == 1)
                        afisare();
                    else
                        BK(k + 1);
                }
            }
        }
        public static unsafe void Main(string[] args)
        {
            //       return;
            try
            {

                Extensions.Time32 Start = Extensions.Time32.Now;
                //MyConsole.DissableButton();
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
                ServerSockets.Packet.SealString = "TQServer";
                //StartDate = DateTime.Now;
                MsgInvoker = new PacketInvoker(PacketAttribute.Translator);
                Cryptography.DHKeyExchange.KeyExchange.CreateKeys();
                Game.MsgTournaments.MsgSchedules.Create();
                Database.Server.Initialize();
                SendGlobalPackets = new SendGlobalPacket();
                Cryptography.AuthCryptography.PrepareAuthCryptography();
                Database.Server.LoadDatabase();
                Poker.Database.Load();
                //Booths.Load();//Booths
                MsgLoader.LoadHashes();
                handlerKeepAlive = ProcessConsoleEvent;
                SetConsoleCtrlHandler(handlerKeepAlive, true);
                //  WebServer.LoaderServer.Init();
                WebServer.Proces.Init();
               // GuardShield.MsgGuardShield.Load(ServerConfig.IPAddres, true);
                CallBack = new ServerSockets.ThreadPool();
                ServerCallback = new Basic(ServerSockets.ThreadPool.ServerCallBack, 250);
                if (ServerConfig.IsInterServer == false)
                {
                    GameServer = new ServerSockets.ServerSocket(
                        new Action<ServerSockets.SecuritySocket>(p => new Client.GameClient(p))
                        , Game_Receive, Game_Disconnect);
                    GameServer.Initilize(ServerConfig.Port_SendSize, ServerConfig.Port_ReceiveSize, 1, 3);
                    GameServer.Open(ServerConfig.IPAddres, ServerConfig.GamePort, ServerConfig.Port_BackLog);

                }
                GlobalItems = new ShowChatItems();
                Database.NpcServer.LoadServerTraps();
                MsgInterServer.PipeServer.Initialize();
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("Server Loaded in " + (Extensions.Time32.Now - Start) + " Milliseconds.");
                Console.WriteLine("»»»»»»»»»»»»»»»»»[Online]«««««««««««««««««", ConsoleColor.Green);
                SocketsGroup = new ServerSockets.SocketPoll("ConquerServer"
                      , GameServer
                      , MsgInterServer.PipeServer.Server
                      , WebServer.Proces.AccServer);
                MsgInterServer.StaticConnexion.Create();
                Game.MsgTournaments.MsgSchedules.ClanWar = new Game.MsgTournaments.MsgClanWar();
                new MapGroupThread(100, "ConquerServer3").Start();
            }
            catch (Exception e) { MyConsole.WriteException(e); }

            for (; ; )
                ConsoleCMD(MyConsole.ReadLine());
        }
        public static void SaveDBPayers(/*Extensions.Time32 clock*/)
        {
            if (Database.Server.FullLoading && !Program.ServerConfig.IsInterServer)
            {
                foreach (var user in Database.Server.GamePoll.Values)
                {
                    if (user.OnInterServer)
                        continue;
                    if ((user.ClientFlag & Client.ServerFlag.LoginFull) == Client.ServerFlag.LoginFull)
                    {
                        user.ClientFlag |= Client.ServerFlag.QueuesSave;
                        Database.ServerDatabase.LoginQueue.TryEnqueue(user);
                    }
                }
                Role.Instance.Clan.ProcessChangeNames();
                Role.Instance.Guild.ProcessChangeNames();
                Database.Server.SaveDatabase();
                Console.ForegroundColor = (ConsoleColor)(ushort)Program.GetRandom.Next(1, 14);
            }
        }
        public static void Copy(string sourceDirectory, string targetDirectory)
        {
            DirectoryInfo diSource = new DirectoryInfo(sourceDirectory);
            DirectoryInfo diTarget = new DirectoryInfo(targetDirectory);
            CopyAll(diSource, diTarget);
        }
        public static void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            Directory.CreateDirectory(target.FullName);
            foreach (FileInfo fi in source.GetFiles())
            {
                fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
            }
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir);
            }
        }
        public unsafe static void ConsoleCMD(string cmd)
        {
            try
            {
                string[] line = cmd.Split(' ');
                switch (line[0])
                {
                    case "kings":
                        {
                            Program.Nobility = true;
                            Database.NobilityTable.Load();
                            var array = Database.Server.GamePoll.Values.ToArray();
                            foreach (var client in array)
                            {
                                Program.NobilityRanking.UpdateRank(client.Player.Nobility);
                                client.Player.NobilityRank = client.Player.Nobility.Rank;
                                client.Equipment.QueryEquipment(client.Equipment.Alternante, false);
                                using (var rect = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rect.GetStream();
                                    client.Send(stream.NobilityIconCreate(client.Player.Nobility));
                                }
                            }
                            break;
                        }
                    case "backup":
                        {
                            try
                            {
                                string create = Program.ServerConfig.DbLocation + "\\AABackUP\\" + DateTime.Now.Year + " - " + DateTime.Now.Month + " - " + DateTime.Now.Day + " ";
                                string createUsers = create + "\\Users";
                                string createspells = create + "\\PlayersSpells";
                                string createprofs = create + "\\PlayersProfs";
                                string createitems = create + "\\PlayersItems";
                                string createquests = create + "\\Quests";
                                string createhouses = create + "\\Houses";
                                string createclans = create + "\\Clans";
                                string createguilds = create + "\\Guilds";
                                string createunions = create + "\\Unions";
                                string all = createUsers + createspells + createprofs + createitems + createquests + createhouses + createclans + createguilds + createunions;
                                try
                                {
                                    if (!Directory.Exists(create))
                                    {
                                        DirectoryInfo di = Directory.CreateDirectory(create);
                                        DirectoryInfo di2 = Directory.CreateDirectory(createUsers);
                                        DirectoryInfo di3 = Directory.CreateDirectory(createspells);
                                        DirectoryInfo di4 = Directory.CreateDirectory(createprofs);
                                        DirectoryInfo di5 = Directory.CreateDirectory(createitems);
                                        DirectoryInfo di6 = Directory.CreateDirectory(createquests);
                                        DirectoryInfo di7 = Directory.CreateDirectory(createhouses);
                                        DirectoryInfo di8 = Directory.CreateDirectory(createclans);
                                        DirectoryInfo di9 = Directory.CreateDirectory(createguilds);
                                        DirectoryInfo di0 = Directory.CreateDirectory(createunions);
                                        File.Copy(Program.ServerConfig.DbLocation + "\\JiangHu.txt", create + "\\JiangHu.txt", true);
                                        File.Copy(Program.ServerConfig.DbLocation + "\\PrestigeRanking.txt", create + "\\PrestigeRanking.txt", true);
                                        File.Copy(Program.ServerConfig.DbLocation + "\\InnerPower.txt", create + "\\InnerPower.txt", true);
                                        Copy(Program.ServerConfig.DbLocation + "\\Users", createUsers);
                                        Copy(Program.ServerConfig.DbLocation + "\\PlayersSpells", createspells);
                                        Copy(Program.ServerConfig.DbLocation + "\\PlayersProfs", createprofs);
                                        Copy(Program.ServerConfig.DbLocation + "\\PlayersItems", createitems);
                                        Copy(Program.ServerConfig.DbLocation + "\\Quests", createquests);
                                        Copy(Program.ServerConfig.DbLocation + "\\Houses", createhouses);
                                        Copy(Program.ServerConfig.DbLocation + "\\Clans", createclans);
                                        Copy(Program.ServerConfig.DbLocation + "\\Guilds", createguilds);
                                        Copy(Program.ServerConfig.DbLocation + "\\Unions", createunions);
                                        MyConsole.WriteLine("Done BackUp Database For today ( " + DateTime.Now.Year + " - " + DateTime.Now.Month + " - " + DateTime.Now.Day + " ) ");
                                        return;
                                    }
                                    else
                                    {
                                        MyConsole.WriteLine("" + create + " Already BackedUp once !");
                                        return;
                                    }
                                }
                                catch (IOException ioex)
                                {
                                    Console.WriteLine(ioex.Message);
                                }

                            }
                            catch (Exception e) { Console.WriteLine(e.ToString()); }
                            break;
                        }
                    case "kingf":
                        {
                            Program.Nobility = false;
                            Database.NobilityTable.Load();
                            var array = Database.Server.GamePoll.Values.ToArray();
                            foreach (var client in array)
                            {
                                Program.NobilityRanking.UpdateRank(client.Player.Nobility);
                                client.Player.NobilityRank = client.Player.Nobility.Rank;
                                client.Equipment.QueryEquipment(client.Equipment.Alternante, false);
                                using (var rect = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rect.GetStream();
                                    client.Send(stream.NobilityIconCreate(client.Player.Nobility));
                                }
                            }
                            break;
                        }
                    case "classpoleon":
                        {
                            Game.MsgTournaments.MsgNobilityPole.Proces = Game.MsgTournaments.ProcesType.Alive;
                            Game.MsgTournaments.MsgArcherClass.Start();
                            Game.MsgTournaments.MsgDragonClass.Start();
                            Game.MsgTournaments.MsgFireClass.Start();
                            Game.MsgTournaments.MsgMonkClass.Start();
                            Game.MsgTournaments.MsgNinjaClass.Start();
                            Game.MsgTournaments.MsgPirateClass.Start();
                            Game.MsgTournaments.MsgTrojanClass.Start();
                            Game.MsgTournaments.MsgWarriorClass.Start();
                            Game.MsgTournaments.MsgWaterClass.Start();
                            Game.MsgTournaments.MsgWindClass.Start();

                            Game.MsgTournaments.MsgArcherClass.CheckUP();
                            Game.MsgTournaments.MsgDragonClass.CheckUP();
                            Game.MsgTournaments.MsgFireClass.CheckUP();
                            Game.MsgTournaments.MsgMonkClass.CheckUP();
                            Game.MsgTournaments.MsgNinjaClass.CheckUP();
                            Game.MsgTournaments.MsgPirateClass.CheckUP();
                            Game.MsgTournaments.MsgTrojanClass.CheckUP();
                            Game.MsgTournaments.MsgWarriorClass.CheckUP();
                            Game.MsgTournaments.MsgWaterClass.CheckUP();
                            Game.MsgTournaments.MsgWindClass.CheckUP();
                            break;
                        }
                    case "classpoleend":
                        {
                            Game.MsgTournaments.MsgArcherClass.End();
                            Game.MsgTournaments.MsgDragonClass.End();
                            Game.MsgTournaments.MsgFireClass.End();
                            Game.MsgTournaments.MsgMonkClass.End();
                            Game.MsgTournaments.MsgNinjaClass.End();
                            Game.MsgTournaments.MsgPirateClass.End();
                            Game.MsgTournaments.MsgTrojanClass.End();
                            Game.MsgTournaments.MsgWarriorClass.End();
                            Game.MsgTournaments.MsgWaterClass.End();
                            Game.MsgTournaments.MsgWindClass.End();
                            break;
                        }


                    case "fighters1":
                        {
                            Game.MsgTournaments.MsgSchedules.FightersPole1.Start();
                            break;
                        }
                    case "endfighters1":
                        {
                            Game.MsgTournaments.MsgSchedules.FightersPole1.End();
                            break;
                        }
                    case "fighters2":
                        {
                            Game.MsgTournaments.MsgSchedules.FightersPole2.Start();
                            break;
                        }
                    case "endfighters2":
                        {
                            Game.MsgTournaments.MsgSchedules.FightersPole2.End();
                            break;
                        }
                    case "fighters3":
                        {
                            Game.MsgTournaments.MsgSchedules.FightersPole3.Start();
                            break;
                        }
                    case "endfighters3":
                        {
                            Game.MsgTournaments.MsgSchedules.FightersPole3.End();
                            break;
                        }
                    case "fighters4":
                        {
                            Game.MsgTournaments.MsgSchedules.FightersPole4.Start();
                            break;
                        }
                    case "endfighters4":
                        {
                            Game.MsgTournaments.MsgSchedules.FightersPole4.End();
                            break;
                        }
                    case "nobilitypole":
                        {
                            Game.MsgTournaments.MsgNobilityPole.Proces = Game.MsgTournaments.ProcesType.Alive;
                            Game.MsgTournaments.MsgNobilityPole.Start();
                            Game.MsgTournaments.MsgNobilityPole1.Proces = Game.MsgTournaments.ProcesType.Alive;
                            Game.MsgTournaments.MsgNobilityPole1.Start();
                            Game.MsgTournaments.MsgNobilityPole2.Proces = Game.MsgTournaments.ProcesType.Alive;
                            Game.MsgTournaments.MsgNobilityPole2.Start();
                            Game.MsgTournaments.MsgNobilityPole3.Proces = Game.MsgTournaments.ProcesType.Alive;
                            Game.MsgTournaments.MsgNobilityPole3.Start();
                            break;
                        }
                    case "QA":
                        {
                            if (Game.MsgTournaments.MsgSchedules.CurrentTournament.Process != Game.MsgTournaments.ProcesType.Dead)
                            {
                                Game.MsgTournaments.MsgSchedules.CurrentTournament.Process = Game.MsgTournaments.ProcesType.Dead;
                            }
                            break;
                        }
                    case "QZ":
                        {
                            Game.MsgTournaments.MsgSchedules.CurrentTournament = Game.MsgTournaments.MsgSchedules.Tournaments[Game.MsgTournaments.TournamentType.QuizShow];
                            Game.MsgTournaments.MsgSchedules.CurrentTournament.Open();
                            break;
                        }
                    case "endnobility":
                        {
                            Game.MsgTournaments.MsgNobilityPole.End();
                            Game.MsgTournaments.MsgNobilityPole1.End();
                            Game.MsgTournaments.MsgNobilityPole2.End();
                            Game.MsgTournaments.MsgNobilityPole3.End();
                            break;
                        }
                    case "w":
                        {
                            Controlpanel cp = new Controlpanel();
                            cp.ShowDialog();
                            break;
                        }
                    case "chat":
                        {
                            var ChatCon = new Panels.ChatPanal();
                            ChatCon.ShowDialog();
                            break;
                        }
                    case "clear":
                        {
                            Console.Clear();
                            break;
                        }

                    case "powerarena":
                        {
                            Game.MsgTournaments.MsgSchedules.PowerArena.Start();
                            break;
                        }
                    case "egwon":
                        {
                            Game.MsgTournaments.MsgSchedules.EliteGuildWar.Start();
                            break;
                        }
                    case "egwoff":
                        {
                            Game.MsgTournaments.MsgSchedules.EliteGuildWar.End();
                            break;
                        }
                    case "unionon":
                        {
                            Game.MsgTournaments.MsgSchedules.UnionWar.Start();
                            Game.MsgTournaments.MsgSchedules.UnionWar.Began();
                            Game.MsgTournaments.MsgSchedules.UnionWar.ShuffleGuildScores();
                            break;
                        }
                    case "unionoff":
                        {
                            Game.MsgTournaments.MsgSchedules.UnionWar.CompleteEndGuildWar();
                            break;
                        }
                    case "squidward":
                        {
                            Game.MsgTournaments.MsgSchedules.SquidwardOctopus.Start();
                            break;
                        }
                    case "save":
                        {
                            if (Database.Server.FullLoading && !Program.ServerConfig.IsInterServer)
                            {
                                foreach (var user in Database.Server.GamePoll.Values)
                                {
                                    if (user.OnInterServer)
                                        continue;
                                    if ((user.ClientFlag & Client.ServerFlag.LoginFull) == Client.ServerFlag.LoginFull)
                                    {
                                        user.ClientFlag |= Client.ServerFlag.QueuesSave;
                                        Database.ServerDatabase.LoginQueue.TryEnqueue(user);
                                    }
                                }
                                Role.Instance.Clan.ProcessChangeNames();
                                Role.Instance.Guild.ProcessChangeNames();
                                Database.Server.SaveDatabase();
                                Program.MCompressor.Optimize();
                                Program.MCompressor.Optimize();
                                //  MyConsole.WriteLine("Database got saved ! ");
                            }
                            if (Database.ServerDatabase.LoginQueue.Finish())
                            {
                                System.Threading.Thread.Sleep(500);
                                MyConsole.WriteLine("Database saved successfully.");
                            }
                            break;
                        }
                    case "steed":
                        {
                            Game.MsgTournaments.MsgSchedules.SteedRace.Create();
                            break;
                        }
                    case "ctfon"://TQClient
                        {
                            Game.MsgTournaments.MsgSchedules.CaptureTheFlag.Start();
                            break;
                        }
                    case "teamon":
                        {
                            if (Game.MsgTournaments.MsgSchedules.ElitePkTournament.Proces != Game.MsgTournaments.ProcesType.Dead)
                            {
                                Game.MsgTournaments.MsgSchedules.ElitePkTournament.Proces = Game.MsgTournaments.ProcesType.Dead;
                            }
                            Game.MsgTournaments.MsgSchedules.TeamPkTournament.Start();

                            foreach (var clients in Database.Server.GamePoll.Values)
                            {
                                if (clients.Team != null)
                                    Game.MsgTournaments.MsgSchedules.TeamPkTournament.SignUp(clients);
                            }
                            break;
                        }
                    case "kick":
                        {

                            foreach (var user in Database.Server.GamePoll.Values)
                            {
                                if (user.Player.Name.Contains(line[1]))
                                {
                                    user.EndQualifier();
                                }
                            }
                            break;
                        }

                    case "epkon":
                        {
                            Game.MsgTournaments.MsgSchedules.ElitePkTournament.Start();

                            foreach (var clients in Database.Server.GamePoll.Values)
                            {
                                if (clients.Team != null)
                                    Game.MsgTournaments.MsgSchedules.ElitePkTournament.SignUp(clients);
                            }
                            break;
                        }
                    case "skillon":
                        {
                            Game.MsgTournaments.MsgSchedules.SkillTeamPkTournament.Start();

                            foreach (var clients in Database.Server.GamePoll.Values)
                            {
                                if (clients.Team != null)
                                    Game.MsgTournaments.MsgSchedules.SkillTeamPkTournament.SignUp(clients);
                            }
                            break;
                        }

                    case "oldacc":
                        {
                            WindowsAPI.IniFile ini = new WindowsAPI.IniFile("");
                            foreach (string fname in System.IO.Directory.GetFiles(Program.ServerConfig.DbLocation + "\\Users\\"))
                            {
                                ini.FileName = fname;

                                ulong QuizPoints = ini.ReadUInt64("Character", "QuizPoints", 100);
                                QuizPoints = 100;
                                ini.Write<ulong>("Character", "QuizPoints", QuizPoints);
                                Console.WriteLine("Done Send  100 points");
                            }
                            break;
                        }
                    case "restartcps":
                        {
                            WindowsAPI.IniFile ini = new WindowsAPI.IniFile("");
                            foreach (string fname in System.IO.Directory.GetFiles(Program.ServerConfig.DbLocation + "\\Users\\"))
                            {
                                ini.FileName = fname;

                                ulong nobility = ini.ReadUInt64("Character", "ConquerPoints", 0);
                                nobility = nobility * 30 / 100;
                                ini.Write<ulong>("Character", "ConquerPoints", 0);
                                Console.WriteLine("ConquerPoints Is reast Now");
                            }

                            break;
                        }
                    case "championrest":
                        {
                            WindowsAPI.IniFile ini = new WindowsAPI.IniFile("");
                            foreach (string fname in System.IO.Directory.GetFiles(Program.ServerConfig.DbLocation + "\\Users\\"))
                            {
                                ini.FileName = fname;
                                ini.Write<int>("Character", "ChampionPoints", 0);
                            }
                            Console.WriteLine("Reset Done");
                            break;
                        }
                    case "resetno"://resetnobility
                        {
                            WindowsAPI.IniFile ini = new WindowsAPI.IniFile("");
                            foreach (string fname in System.IO.Directory.GetFiles(Program.ServerConfig.DbLocation + "\\Users\\"))
                            {
                                ini.FileName = fname;

                                ulong nobility = ini.ReadUInt64("Character", "DonationNobility", 0);
                                nobility = nobility * 30 / 100;
                                nobility = 0;
                                ini.Write<ulong>("Character", "DonationNobility", nobility);
                                Console.WriteLine("Nobility Is reast Now");
                            }

                            break;
                        }
                    case "new1":
                        {
                            WindowsAPI.IniFile ini = new WindowsAPI.IniFile("");
                            foreach (string fname in System.IO.Directory.GetFiles(Program.ServerConfig.DbLocation + "\\Users\\"))
                            {
                                ini.FileName = fname;
                                ulong Money = ini.ReadUInt64("Character", "Money", 0);
                                Money = 0;
                                ini.Write<ulong>("Character", "Money", Money);
                                // Console.WriteLine("Money Is reast Now");
                            }
                            break;
                        }
                    case "newx":
                        {
                            WindowsAPI.IniFile ini = new WindowsAPI.IniFile("");
                            foreach (string fname in System.IO.Directory.GetFiles(Program.ServerConfig.DbLocation + "\\Users\\"))
                            {
                                ini.FileName = fname;
                                ulong DonationNobility = ini.ReadUInt64("Character", "DonationNobility", 0);
                                DonationNobility = 0;
                                ini.Write<ulong>("Character", "DonationNobility", 0);

                            }

                            break;
                        }

                    case "nbbb":
                        {
                            WindowsAPI.IniFile ini = new WindowsAPI.IniFile("");
                            foreach (string fname in System.IO.Directory.GetFiles(Program.ServerConfig.DbLocation + "\\Users\\"))
                            {
                                ini.FileName = fname;
                                //    ini.Write<int>("Character", "DonatePoints", 0);
                                ini.Write<int>("Character", "DonationNobility", 0);
                            }
                            Console.WriteLine("Reset Done");
                            break;
                        }


                    case "gift":
                        {
                            WindowsAPI.IniFile ini = new WindowsAPI.IniFile("");
                            foreach (string fname in System.IO.Directory.GetFiles(Program.ServerConfig.DbLocation + "\\Users\\"))
                            {
                                ini.FileName = fname;
                                ini.Write<int>("Character", "ClaimStateGift", 0);
                            }
                            Console.WriteLine("Reset Done");
                            break;
                        }
                    case "fixedgamemap":
                        {
                            Dictionary<int, string> maps = new Dictionary<int, string>();
                            using (var gamemap = new BinaryReader(new FileStream(Path.Combine(Program.ServerConfig.CO2Folder, "ini/gamemap.dat"), FileMode.Open)))
                            {

                                var amount = gamemap.ReadInt32();
                                for (var i = 0; i < amount; i++)
                                {

                                    var id = gamemap.ReadInt32();
                                    var fileName = Encoding.ASCII.GetString(gamemap.ReadBytes(gamemap.ReadInt32()));
                                    var puzzleSize = gamemap.ReadInt32();
                                    if (id == 1017)
                                    {
                                        Console.WriteLine(puzzleSize);
                                    }
                                    if (!maps.ContainsKey(id))
                                        maps.Add(id, fileName);
                                    else
                                        maps[id] = fileName;
                                }
                            }
                            break;
                        }

                    case "GWon":
                        {
                            Game.MsgTournaments.MsgSchedules.GuildWar.Proces = Game.MsgTournaments.ProcesType.Alive;
                            Game.MsgTournaments.MsgSchedules.GuildWar.Start();
                            break;
                        }
                    case "SGWon":
                        {
                            Game.MsgTournaments.MsgSchedules.SuperGuildWar.Start();
                            break;
                        }
                    case "SGWoff":
                        {
                            Game.MsgTournaments.MsgSchedules.SuperGuildWar.Proces = Game.MsgTournaments.ProcesType.Dead;
                            Game.MsgTournaments.MsgSchedules.SuperGuildWar.CompleteEndGuildWar();
                            break;
                        }
                    case "GWoff":
                        {
                            Game.MsgTournaments.MsgSchedules.GuildWar.Proces = Game.MsgTournaments.ProcesType.Dead;
                            Game.MsgTournaments.MsgSchedules.GuildWar.CompleteEndGuildWar();
                            break;
                        }
                    case "restart12":
                        {
                            new Thread(new ThreadStart(Maintenance)).Start();
                            break;
                        }
                    case "exit":
                        {
                            ProcessConsoleEvent(0);

                            Environment.Exit(0);
                            break;
                        }
                    case "restart70nobility": // new .
                        {
                            WindowsAPI.IniFile ini = new WindowsAPI.IniFile("");
                            foreach (string fname in System.IO.Directory.GetFiles(Program.ServerConfig.DbLocation + "\\Users\\"))
                            {
                                ini.FileName = fname;

                                ulong nobility = ini.ReadUInt64("Character", "DonationNobility", 0);
                                nobility = nobility * 30 / 100;
                                ini.Write<ulong>("Character", "DonationNobility", nobility);
                            }
                            Console.WriteLine("DonationRank Reset Done");
                            break;
                        }
                    case "restart50nobility": // new .
                        {
                            WindowsAPI.IniFile ini = new WindowsAPI.IniFile("");
                            foreach (string fname in System.IO.Directory.GetFiles(Program.ServerConfig.DbLocation + "\\Users\\"))
                            {
                                ini.FileName = fname;

                                ulong nobility = ini.ReadUInt64("Character", "DonationNobility", 0);
                                nobility = nobility * 50 / 100;
                                ini.Write<ulong>("Character", "DonationNobility", nobility);
                            }
                            Console.WriteLine("DonationRank Reset Done");
                            break;
                        }
                    case "viprestart":
                        {
                            WindowsAPI.IniFile ini = new WindowsAPI.IniFile("");
                            foreach (string fname in System.IO.Directory.GetFiles(Program.ServerConfig.DbLocation + "\\Users\\"))
                            {
                                ini.FileName = fname;
                                ini.Write<int>("Character", "VipLevel", 0);
                            }
                            Console.WriteLine("Reset Done");
                            break;
                        }
                    case "chip":
                        {
                            WindowsAPI.IniFile ini = new WindowsAPI.IniFile("");
                            foreach (string fname in System.IO.Directory.GetFiles(Program.ServerConfig.DbLocation + "\\Users\\"))
                            {
                                ini.FileName = fname;
                                ini.Write<int>("Character", "ChiPoints", 0);
                            }
                            Console.WriteLine("Reset Done");
                            break;
                        }
                    case "nbb":
                        {
                            WindowsAPI.IniFile ini = new WindowsAPI.IniFile("");
                            foreach (string fname in System.IO.Directory.GetFiles(Program.ServerConfig.DbLocation + "\\Users\\"))
                            {
                                ini.FileName = fname;
                                ini.Write<int>("Character", "DonationNobility", 0);
                                ini.Write<int>("Character", "Money", 0);
                                ini.Write<int>("Character", "WHMoney", 0);

                            }
                            Console.WriteLine("Reset Done");
                            break;
                        }
                    case "forceexit":
                        {
                            ProcessConsoleEvent(0);
                            Environment.Exit(0);
                            break;
                        }
                    case "restart":
                        {
                            ProcessConsoleEvent(0);
                            System.Diagnostics.Process hproces = new System.Diagnostics.Process();
                            hproces.StartInfo.FileName = "DeathWish.exe";
                            hproces.StartInfo.WorkingDirectory = Environment.CurrentDirectory;
                            hproces.Start();
                            Environment.Exit(0);
                            break;
                        }
                }
            }
            catch (Exception e) { Console.WriteLine(e.ToString()); }
        }
        public static void Maintenance()
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                OnMainternance = true;
                MyConsole.WriteLine("The server will be brought down for maintenance in (5 Minutes). Please log off immediately to avoid data loss.");
#if Arabic
                  MsgMessage msg = new MsgMessage("The server will be brought down for maintenance in 5minute0second. Please exitthe game now.", "ALLUSERS", "GM", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center);
              
#else
                MsgMessage msg = new MsgMessage("The server will be brought down for maintenance in (5 Minutes). Please log off immediately to avoid data loss.", "ALLUSERS", "GM", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center);

#endif
                SendGlobalPackets.Enqueue(msg.GetArray(stream));
            }
            Thread.Sleep(1000 * 30);
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                MyConsole.WriteLine("The server will be brought down for maintenance in (4 Minutes & 30 Seconds). Please log off immediately to avoid data loss.");
#if Arabic
                  MsgMessage msg = new MsgMessage("The server will be brought down for maintenance in 4minute30second. Please exit the game now.", "ALLUSERS", "GM", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center);
               
#else
                MsgMessage msg = new MsgMessage("The server will be brought down for maintenance in (4 Minutes & 30 Seconds). Please log off immediately to avoid data loss.", "ALLUSERS", "GM", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center);

#endif
                SendGlobalPackets.Enqueue(msg.GetArray(stream));
            }
            Thread.Sleep(1000 * 30);
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                MyConsole.WriteLine("The server will be brought down for maintenance in (4 Minutes & 00 Seconds). Please log off immediately to avoid data loss.");
#if Arabic
                  MsgMessage msg = new MsgMessage("The server will be brought down for maintenance in 4minute0second. Please exit the game now.", "ALLUSERS", "GM", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center);
              
#else
                MsgMessage msg = new MsgMessage("The server will be brought down for maintenance in (4 Minutes & 00 Seconds). Please log off immediately to avoid data loss.", "ALLUSERS", "GM", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center);

#endif
                SendGlobalPackets.Enqueue(msg.GetArray(stream));
            }
            Thread.Sleep(1000 * 30);
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                MyConsole.WriteLine("The server will be brought down for maintenance in (3 Minutes & 30 Seconds). Please log off immediately to avoid data loss.");
#if Arabic
                       MsgMessage msg = new MsgMessage("The server will be brought down for maintenance in 3minute30second. Please exit the game now.", "ALLUSERS", "GM", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center);
         
#else
                MsgMessage msg = new MsgMessage("The server will be brought down for maintenance in (3 Minutes & 30 Seconds). Please log off immediately to avoid data loss.", "ALLUSERS", "GM", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center);

#endif
                SendGlobalPackets.Enqueue(msg.GetArray(stream));
            }
            Thread.Sleep(1000 * 30);
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                MyConsole.WriteLine("The server will be brought down for maintenance in (3 Minutes & 00 Seconds). Please log off immediately to avoid data loss.");
#if Arabic
                  MsgMessage msg = new MsgMessage("The server will be brought down for maintenance in 3minute0second. Please exit the game now.", "ALLUSERS", "GM", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center);
              
#else
                MsgMessage msg = new MsgMessage("The server will be brought down for maintenance in (3 Minutes & 00 Seconds). Please log off immediately to avoid data loss.", "ALLUSERS", "GM", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center);

#endif
                SendGlobalPackets.Enqueue(msg.GetArray(stream));
            }
            Thread.Sleep(1000 * 30);
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                MyConsole.WriteLine("The server will be brought down for maintenance in (2 Minutes & 30 Seconds). Please log off immediately to avoid data loss.");
#if Arabic
                  MsgMessage msg = new MsgMessage("The server will be brought down for maintenance in 2minute30second. Please exit the game now.", "ALLUSERS", "GM", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center);
              
#else
                MsgMessage msg = new MsgMessage("The server will be brought down for maintenance in (2 Minutes & 30 Seconds). Please log off immediately to avoid data loss.", "ALLUSERS", "GM", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center);

#endif
                SendGlobalPackets.Enqueue(msg.GetArray(stream));
            }
            Thread.Sleep(1000 * 30);
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                MyConsole.WriteLine("The server will be brought down for maintenance in (2 Minutes & 00 Seconds). Please log off immediately to avoid data loss.");
#if Arabic
                        MsgMessage msg = new MsgMessage("The server will be brought down for maintenance in 2minute0second. Please exit the game now.", "ALLUSERS", "GM", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center);
         
#else
                MsgMessage msg = new MsgMessage("The server will be brought down for maintenance in (2 Minutes & 00 Seconds). Please log off immediately to avoid data loss.", "ALLUSERS", "GM", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center);

#endif
            }
            Thread.Sleep(1000 * 30);
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                MyConsole.WriteLine("The server will be brought down for maintenance in (1 Minutes & 30 Seconds). Please log off immediately to avoid data loss.");
#if Arabic
                   MsgMessage msg = new MsgMessage("The server will be brought down for maintenance in 1minute30second. Please exit the game now.", "ALLUSERS", "GM", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center);
             
#else
                MsgMessage msg = new MsgMessage("The server will be brought down for maintenance in (1 Minutes & 30 Seconds). Please log off immediately to avoid data loss.", "ALLUSERS", "GM", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center);

#endif
                SendGlobalPackets.Enqueue(msg.GetArray(stream));
            }
            Thread.Sleep(1000 * 30);
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                MyConsole.WriteLine("The server will be brought down for maintenance in (1 Minutes & 00 Seconds). Please log off immediately to avoid data loss.");
#if Arabic
                 MsgMessage msg = new MsgMessage("The server will be brought down for maintenance in 1minute0second. Please exit the game now.", "ALLUSERS", "GM", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center);
               
#else
                MsgMessage msg = new MsgMessage("The server will be brought down for maintenance in (1 Minutes & 00 Seconds). Please log off immediately to avoid data loss.", "ALLUSERS", "GM", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center);

#endif
                SendGlobalPackets.Enqueue(msg.GetArray(stream));
            }
            Thread.Sleep(1000 * 30);
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                MyConsole.WriteLine("The server will be brought down for maintenance in (0 Minutes & 30 Seconds). Please log off immediately to avoid data loss.");
#if Arabic
                MsgMessage msg = new MsgMessage("The server will be brought down for maintenance in 0minute30second. Please exit the game now.", "ALLUSERS", "GM", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center);
                
#else
                MsgMessage msg = new MsgMessage("The server will be brought down for maintenance in (0 Minutes & 30 Seconds). Please log off immediately to avoid data loss.", "ALLUSERS", "GM", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center);

#endif
                SendGlobalPackets.Enqueue(msg.GetArray(stream));
            }
            Thread.Sleep(1000 * 20);
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
#if Arabic
                  MsgMessage msg = new MsgMessage("Server maintenance(2 minutes). Please log off immediately to avoid data loss.", "ALLUSERS", "GM", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center);
              
#else
                MsgMessage msg = new MsgMessage("Server maintenance(few minutes). Please log off immediately to avoid data loss.", "ALLUSERS", "GM", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center);

#endif
                SendGlobalPackets.Enqueue(msg.GetArray(stream));
            }
            Thread.Sleep(1000 * 10);
            ProcessConsoleEvent(0);

            Environment.Exit(0);
        }
        public unsafe static void Game_Receive(ServerSockets.SecuritySocket obj, ServerSockets.Packet stream)//ServerSockets.Packet data)
        {
            if (!obj.SetDHKey)
                CreateDHKey(obj, stream);
            else
            {
                try
                {
                    if (obj.Game == null)
                        return;
                    ushort PacketID = stream.ReadUInt16();

                    if (obj.Game.Player.CheckTransfer)
                        goto jmp;
                    if (obj.Game.PipeClient != null && PacketID != Game.GamePackets.Achievement)
                    {
                        if (PacketID == (ushort)Game.GamePackets.MsgOsShop
                      || PacketID == (ushort)Game.GamePackets.SecondaryPassword
                      || PacketID >= (ushort)Game.GamePackets.LeagueOpt && PacketID <= (ushort)Game.GamePackets.LeagueConcubines
                      || PacketID == (ushort)Game.GamePackets.LeagueRobOpt)
                            goto jmp;

                        stream.Seek(stream.Size);
                        obj.Game.PipeClient.Send(stream);

                        if (PacketID != 1009)
                        {

                            return;
                        }
                        stream.Seek(4);
                    }
                jmp:
                    if (PacketID == 2171 || PacketID == 2088 || PacketID == 2096 || PacketID == 2090 || PacketID == 2093)
                    {
                        PokerHandler.Handler(obj.Game, stream);
                    }
                    else
                    {
                        Action<Client.GameClient, ServerSockets.Packet> hinvoker;
                        if (MsgInvoker.TryGetInvoker(PacketID, out hinvoker))
                        {
                            hinvoker(obj.Game, stream);
                        }
                    }
                }
                catch (Exception e) { MyConsole.WriteException(e); }
                finally
                {
                    ServerSockets.PacketRecycle.Reuse(stream);
                }
            }
        }
        public static void DirectoryCopy(
        string sourceDirName, string destDirName, bool copySubDirs)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            DirectoryInfo[] dirs = dir.GetDirectories();
            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }
        public unsafe static void CreateDHKey(ServerSockets.SecuritySocket obj, ServerSockets.Packet Stream)
        {
            try
            {
                byte[] buffer = new byte[36];
                bool extra = false;
                string text = System.Text.ASCIIEncoding.ASCII.GetString(obj.DHKeyBuffer.buffer, 0, obj.DHKeyBuffer.Length());
                if (!text.EndsWith("TQClient"))
                {
                    System.Buffer.BlockCopy(obj.EncryptedDHKeyBuffer.buffer, obj.EncryptedDHKeyBuffer.Length() - 36, buffer, 0, 36);
                    extra = true;
                }
                string key;
                if (Stream.GetHandshakeReplyKey(out key))
                {
                    obj.SetDHKey = true;
                    obj.Game.DHKey.HandleResponse(key);
                    var compute_key = obj.Game.DHKeyExchance.PostProcessDHKey(obj.Game.DHKey.ToBytes());
                    obj.Game.Crypto.GenerateKey(compute_key);
                    obj.Game.Crypto.Reset();
                }
                else
                {
                    obj.Disconnect();
                    return;
                }
                if (extra)
                {
                    Stream.Seek(0);
                    obj.Game.Crypto.Decrypt(buffer, 0, Stream.Memory, 0, 36);
                    Stream.Size = buffer.Length;
                    Stream.Size = buffer.Length;
                    Stream.Seek(2);
                    ushort PacketID = Stream.ReadUInt16();
                    Action<Client.GameClient, ServerSockets.Packet> hinvoker;
                    if (MsgInvoker.TryGetInvoker(PacketID, out hinvoker))
                    {
                        hinvoker(obj.Game, Stream);
                    }
                    else
                    {
                        obj.Disconnect();

                        MyConsole.WriteLine("DH KEY Not found the packet ----> " + PacketID);
                    }
                }
            }
            catch (Exception e) { MyConsole.WriteException(e); }
        }
        public unsafe static void Game_Disconnect(ServerSockets.SecuritySocket obj)
        {
            if (obj.Game != null && obj.Game.Player != null)
            {
                try
                {
                    Client.GameClient client;
                    if (Database.Server.GamePoll.TryGetValue(obj.Game.Player.UID, out client))
                    {
                        try
                        {
                            PokerHandler.Shutdown(client);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.ToString());
                        }
                        DeathWish.ServerSockets.ThreadPool.Unregister(client);
                        if (client.OnInterServer)
                            return;
                        if ((client.ClientFlag & Client.ServerFlag.LoginFull) == Client.ServerFlag.LoginFull)
                        {
                            if (obj.Game.PipeClient != null)
                                obj.Game.PipeClient.Disconnect();
                            MyConsole.WriteLine("Client [ " + client.Player.Name + " ] Out Game .");
                            var dt = DateTime.Now;
                            string logs = "[Logout]" + dt.Hour + "H:" + dt.Minute + "M :" + dt.Second + "S  pid: " + client.Player.UID + " " + client.Player.Name + " [ Log out  ]  ";
                            Database.ServerDatabase.LoginQueue.Enqueue(logs);
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                try
                                {
                                    if (client.Player.InUnion)
                                    {
                                        client.Player.UnionMemeber.Owner = null;
                                    }
                                    client.CheckRouletteDisconnect();
                                    client.EndQualifier();
                                    if (client.Team != null)
                                        client.Team.Remove(client, true);
                                    if (client.Player.MyClanMember != null)
                                        client.Player.MyClanMember.Online = false;
                                    if (client.IsVendor)
                                        client.MyVendor.StopVending(stream);
                                    if (client.InTrade)
                                        client.MyTrade.CloseTrade();
                                    if (client.Player.MyGuildMember != null)
                                        client.Player.MyGuildMember.IsOnline = false;
                                    if (client.Player.ObjInteraction != null)
                                    {
                                        client.Player.InteractionEffect.AtkType = Game.MsgServer.MsgAttackPacket.AttackID.InteractionStopEffect;
                                        InteractQuery action = InteractQuery.ShallowCopy(client.Player.InteractionEffect);
                                        client.Send(stream.InteractionCreate(&action));
                                        client.Player.ObjInteraction.Player.OnInteractionEffect = false;
                                        client.Player.ObjInteraction.Player.ObjInteraction = null;
                                    }
                                    client.Player.View.Clear(stream);
                                }
                                catch (Exception e)
                                {
                                    MyConsole.WriteException(e);
                                    client.Player.View.Clear(stream);
                                }
                                finally
                                {
                                    client.ClientFlag &= ~Client.ServerFlag.LoginFull;
                                    client.ClientFlag |= Client.ServerFlag.Disconnect;
                                    client.ClientFlag |= Client.ServerFlag.QueuesSave;
                                    Database.ServerDatabase.LoginQueue.TryEnqueue(client);
                                }
                                try
                                {
                                    if (client.Player.MyMentor != null)
                                    {
                                        Client.GameClient me;
                                        client.Player.MyMentor.OnlineApprentice.TryRemove(client.Player.UID, out me);
                                        client.Player.MyMentor = null;
                                    }
                                    client.Player.Associate.Online = false;
                                    lock (client.Player.Associate.MyClient)
                                        client.Player.Associate.MyClient = null;
                                    foreach (var clien in client.Player.Associate.OnlineApprentice.Values)
                                        clien.Player.SetMentorBattlePowers(0, 0);
                                    client.Player.Associate.OnlineApprentice.Clear();

                                }

                                catch (Exception e) { MyConsole.WriteLine(e.ToString()); }
                            }
                        }
                    }
                }
                catch (Exception e) { MyConsole.WriteLine(e.ToString()); }
            }
            else if (obj.Game != null)
            {
                if (obj.Game.ConnectionUID != 0)
                {
                    try
                    {
                        PokerHandler.Shutdown(obj.Game);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                    }
                    Client.GameClient client;
                    Database.Server.GamePoll.TryRemove(obj.Game.ConnectionUID, out client);
                }
            }
        }
        public static bool NameStrCheck(string name, bool ExceptedSize = true)
        {
            if (name == null)
                return false;
            if (name == "")
                return false;
            string ValidChars = "[^A-Za-z0-9ء-ي*~.&.$]$";
            System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(ValidChars);
            if (r.IsMatch(name))
                return false;
            if (name.ToLower().Contains("gm"))
                return false;
            if (name.ToLower().Contains("pm"))
                return false;
            if (name.ToLower().Contains("None"))
                return false;
            if (name.ToLower().Contains("none"))
                return false;
            if (name.ToLower().Contains("Guard"))
                return false;
            if (name.ToLower().Contains("Guard1"))
                return false;
            if (name.ToLower().Contains("Guard2"))
                return false;
            if (name.ToLower().Contains("p~m"))
                return false;
            if (name.ToLower().Contains("p!m"))
                return false;
            if (name.ToLower().Contains("g~m"))
                return false;
            if (name.ToLower().Contains("g!m"))
                return false;
            if (name.ToLower().Contains("help"))
                return false;
            if (name.ToLower().Contains("desk"))
                return false;
            if (name.ToLower().Contains("xEl3Tar"))
                return false;
            if (name.Contains('/'))
                return false;
            if (name.Contains(@"\"))
                return false;
            if (name.Contains(@"'"))
                return false;
            if (name.Contains("GM") ||
                name.Contains("PM") ||
                name.Contains("None") ||
                name.Contains("VikingConquer") ||
                name.Contains("El3Tar") ||
                name.Contains("WeConqer") ||
                name.Contains("Guard") ||
                name.Contains("Guard1") ||
                name.Contains("Guard2") ||
                name.Contains("WeCo") ||
                name.Contains("SYSTEM") ||
                name.Contains("{") || name.Contains("}") || name.Contains("[") || name.Contains("]"))
                return false;
            if (name.Length > 16 && ExceptedSize)
                return false;
            for (int x = 0; x < name.Length; x++)
                if (name[x] == 25)
                    return false;
            return true;
        }
        public static string LogginKey = "V1xXxD3SH2AxXxoq";
      
    }
}
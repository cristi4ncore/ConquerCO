using System;
using System.Linq;
using System.Text;
using DeathWish.Database;
using DeathWish.ServerSockets;
using DeathWish.Game.MsgServer;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Sockets;
using DeathWish.Role;

namespace DeathWish.Bot
{
    public class AI
    {
        public static uint[] Class = new uint[]
        {
            //15,
            55,
            //25,
            45,
            //135,
            //145,
            //75,
            65
        };
        public static string[] Names = new string[]
        {
            "Mace",
            "Falchion","Montante","Battleaxe","Zweihander","Hatchet",
            "Billhook","Club","Hammer","Caltrop","Maul","Sledgehammer","Longbow",
            "Bludgeon","Harpoon","Crossbow","Lance","Angon","Pike","TigerClaw","FireLance",
            "Poleaxe","BrassKnuckle","Matchlock","Quarterstaff","Gauntlet","Bullwhip","WarHammer","Katar",
            "FlyingClaw","Spear","Dagger","Slungshot","Katana","Gladius","Aspis","Saber","Cutlass",
            "Blade","Broadsword","Scimitar","Lockback","Claymore","Espada","Machete","Grizzly","Wolverine",
            "Deathstalker","Snake","Wolf","Scorpion","Vulture","Claw","Boomslang","Falcon","Fang","Viper",
            "Ram","Grip","Sting","Boar","BlackMamba","Lash","Tusk","Goshawk","Gnaw","Amazon","Majesty",
            "Anomoly","Malice","Banshee","Mannequin","Belladonna","Minx","Beretta","Mirage","BlackBeauty",
            "Nightmare","Calypso","Nova","Carbon","Pumps","Cascade","Raven","Colada","Resin","Cosma","Riveter",
            "Cougar","Rogue","Countess","Roulette","Enchantress","Shadow","Enigma","Siren","FemmeFatale","Stiletto",
            "Firecracker","Tattoo","Geisha","T_Back","Goddess","Temperance","HalfPint","Tequila","Harlem","Terror","Heroin",
            "Thunderbird","Infinity","Ultra","Insomnia","Vanity","Ivy","Velvet","Legacy","Vixen","Lithium","Voodoo","Lolita",
            "Wicked","Lotus","Widow","Mademoiselle","Xenon","Kahina","Teuta","Isis","Dihya","Artemis","Nefertiti","RunningEagle",
            "Atalanta","Sekhmet","Colestah","Athena","Ishtar","CalamityJane","Enyo","Ashtart","PearlHeart","Bellona","Juno","BelleStarr",
            "WhiteTights","Tanit","HuaMulan","Shieldmaiden","Devi","Boudica","Valkyrie","Selkie","Medb","Cleo","Venus","Fate","Beguile","Deviant",
            "Illusion","Crafty","Variance","Delusion","Deceit","Caprice","Deception","Waylay","Aberr","Myth","Ambush","Variant","Daydream","Feint","Hero",
            "NightTerror","Catch_22","Villain","Figment","Puzzler","Daredevil","Virtual","Curio","Mercenary","Chicanery","Prodigy","Voyager","Trick",
            "Breach","Wanderer","Vile","MissFortune","Audacity","Horror","Vex","Swagger","Dismay","Grudge","Nerve","Phobia","Enmity","Egomania","Fright",
            "Animus","Scheme","Panic","Hostility","Paramour","Agony","Rancor","X_hibit","ffqfff","Malevolence","Charade",
            "Blaze",
            "Poison",
            "Hauteur",
            "Crucible",
            "Spite",
            "Vainglory",
            "Haunter",
            "Spitefulness",
            "Narcissus",
            "Bane",
            "Venom",
            "Brass",
            "Volcano",
            "Vampire",
            "Hulk",
            "Abdo255",
            "Ahmed100",
            "3adel_333",
            "KahledKing",
            "MohamedKing",
            "SaeedAhmed",
            "KingOfAhmed",
            "PrinceSss",
            "sadasd",
            "gwgewg",
            "asdasr",
            "3f42f3",
            "r2w1r12r",
            "guif2yh789349",
            "asdqwd123",
            "sdaf",
            "oubg",
            "Pubg3131",
            "keborddd",
            "hguirhg34",
            "656545t43",
            "2456246t24",
            "46t34t6346",
            "9679i67i9",
            "67i9679",
            "75u67u4",
            "46yu67u",
            "5u65u",
            "56u65u5",
            "65uhjrt6u",
            "4u45u5t44uyh",
            "Mace",
            "Falchion",
            "Montante",
            "Battleaxe",
            "Zweihander",
            "Hatchet",
            };

        public Client.GameClient BEntity;
        public Equipment Equipment;
        public ushort Body;
        public uint UID;
        public uint MapID;
        public ushort X;
        public ushort Y;
        public int HP;
        public System.Time32 StampJumbCallback = System.Time32.Now;
        public System.Time32 StampHitCallback = System.Time32.Now;
        public Role.GameMap Map = null;

        public AI()
        {
            //ServerSocket server = null;
            //SecuritySocket Owner = null;
            //server = new ServerSockets.ServerSocket(new Action<ServerSockets.SecuritySocket>(p => new Client.GameClient(p)), GameServer.Receive, GameServer.Disconnect);
            //Owner = new ServerSockets.SecuritySocket(server, GameServer.Disconnect, GameServer.Receive);
            //Owner.IsBot = true;
            //Owner.Alive = true;
            //Owner.Connection = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //Owner.Create(Owner.Connection);
            BEntity = new Client.GameClient(null);
        }
        public void Add(bool auto = false)
        {
            ushort x = (ushort)Program.GetRandom.Next((int)X - 0, X + 0);
            ushort y = (ushort)Program.GetRandom.Next((int)Y - 0, Y + 0);
            if (Map.IsValidFlagNpc(x, y))
            {
                Equipment = new Equipment(this);
                BEntity.Fake = true;
                BEntity.Player = new Role.Player(BEntity);
                BEntity.Inventory = new Role.Instance.Inventory(BEntity);
                BEntity.Equipment = new Role.Instance.Equip(BEntity);
                BEntity.Warehouse = new Role.Instance.Warehouse(BEntity);
                BEntity.MyProfs = new Role.Instance.Proficiency(BEntity);
                BEntity.MySpells = new Role.Instance.Spell(BEntity);
                BEntity.Achievement = new Database.AchievementCollection();
                BEntity.Status = new MsgStatus();
                string NamesID = Names[Program.GetRandom.Next(0, Names.Length)];
                BEntity.Player.Name = NamesID;
                BEntity.Player.Body = (ushort)Program.GetRandom.Next(1001, 1005);
                BEntity.Player.UID = this.UID = Program.ClientCounter.Next;
                BEntity.Player.HitPoints = this.HP + 2500;
                BEntity.Status.MaxHitpoints = (uint)BEntity.Player.HitPoints;
                BEntity.Player.X = x;
                BEntity.Player.Y = y;
                BEntity.Player.Map = this.MapID;
                BEntity.Player.Level = (byte)Program.GetRandom.Next(1, 100);
                BEntity.Player.VipLevel = 3;
                BEntity.Player.Reborn = (byte)Program.GetRandom.Next(0, 1);
                //BEntity.Player.NobilityRank = (Role.Instance.Nobility.NobilityRank)Pool.GetRandom.Next(5, 6);
                BEntity.Player.Face = (ushort)Program.GetRandom.Next(1, 255);
                //BEntity.Player.CountryID = (ushort)Pool.GetRandom.Next(1, 255);
                BEntity.Player.Action = Role.Flags.ConquerAction.Sit;
                uint ClassID = Class[Program.GetRandom.Next(0, Class.Length)];
                if (!auto)
                {
                    BEntity.Player.Away = 1;
                }
                BEntity.Player.Angle = (Role.Flags.ConquerAngle)Program.GetRandom.Next(4, 8);
                BEntity.Player.Class = (byte)ClassID;
                BEntity.Player.FirstClass = (byte)ClassID;
                BEntity.Player.SecoundeClass = (byte)ClassID;
                BEntity.Player.ServerID = (ushort)Database.GroupServerList.MyServerInfo.ID;
                BEntity.Map = this.Map;
                BEntity.Player.Vitality = (ushort)((BEntity.Player.Level + BEntity.Player.BattlePower) * (BEntity.Player.Reborn + 1));
                //BEntity.GeneratorItemDrop(DropStatus.All);
                DataCore.AtributeStatus.GetStatus(BEntity.Player);
                DataCore.SetCharacterSides(BEntity.Player);
                DataCore.CreateHairStyle(BEntity);
                DataCore.LoadClient(BEntity.Player);
                Equipment.GetRandomEquipment((byte)ClassID).Send();
                BEntity.Map.Enquer(BEntity);
                StampJumbCallback = Time32.Now;
                StampHitCallback = Time32.Now;
                Database.Server.GamePoll.TryAdd(BEntity.Player.UID, BEntity);
                if (auto)
                    Program.CallBack.BotRegister(this);
            }
        }
    }
}

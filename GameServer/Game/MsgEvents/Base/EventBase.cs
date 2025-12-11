using DeathWish.Client;
using DeathWish.Game.MsgNpc;
using DeathWish.Game.MsgServer;
using DeathWish.Game.MsgTournaments;
using DeathWish.Role;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static DeathWish.Game.MsgServer.MsgMessage;
using static DeathWish.Role.Flags;

namespace DeathWish.Game.MsgEvents
{
    public enum EventStage
    {
        None,
        Inviting,
        Countdown,
        Starting,
        Fighting,
        Over
    }
    public enum BroadCastLoc
    {
        Exit,
        World,
        Map,
        System,
        Score,
        Title,
        SpecificMap,
        YellowMap,
        WorldY
    }
    public class Events
    {
        #region Properties
        public uint RewardCps = 430;
        public string EventTitle = "Base Event";
        public DateTime LastScores;
        public byte IDEvent = 0;
        public MsgStaticMessage.Messages IDMessage = MsgStaticMessage.Messages.None;
        public bool isTerr = false;
        public bool isBlue = false;
        public bool isRed = false;
        public bool IsCapitan = false;
        public bool DW = false;
        public EventStage Stage = EventStage.None;
        public Role.GameMap Map;
        public bool NoDamage = false;
        public ushort BaseMap = 700;
        public uint DinamicID = 0;
        public bool FFADamage = false;
        public uint Duration = 0;
        public bool Reflect = false;
        public ushort X = 0;
        public ushort Y = 0;
        public bool MagicAllowed = true;
        public bool ReviveAllowed = true;
        public bool MeleeAllowed = true;
        public bool FlyAllowed = false;
        public bool PotionsAllowed = true;
        public bool ReviveTele = false;
        public bool FriendlyFire = false;
        public Dictionary<uint, GameClient> PlayerList = new Dictionary<uint, GameClient>();
        public readonly Dictionary<uint, int> PlayerScores = new Dictionary<uint, int>();
        public Dictionary<uint, Dictionary<uint, GameClient>> Teams = new Dictionary<uint, Dictionary<uint, GameClient>>();
        public List<ushort> AllowedSkills = new List<ushort>();
        private byte minplayers = 2;
        public int CountDown;
        public int DialogID = 0;
        public DateTime EndTime;
        public DateTime DisplayScores;
        public ushort WaitingArea = 1616;
        public uint Rank = 0;
        #endregion
        /// <summary>
        /// Used to send messages related to the current PVP Event
        /// </summary>
        public void Broadcast(string msg, BroadCastLoc loc, uint Map = 0, GameClient user = null)
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                if (user != null)
                {
                    switch (loc)
                    {
                        case BroadCastLoc.Exit:
                            {
                                if (!PlayerList.ContainsKey(user.Player.UID))
                                {
                                    user.SendSysMesage($"---------{EventTitle}---------", ChatMode.FirstRightCorner);
                                    user.SendSysMesage("You leaved of the event.", ChatMode.ContinueRightCorner);
                                }
                            }
                            break;
                    }
                }
                else
                {
                    switch (loc)
                    {
                        case BroadCastLoc.System:
                            {
                                Program.SendGlobalPackets.Enqueue(new MsgServer.MsgMessage(msg, MsgColor.white, ChatMode.System).GetArray(stream));
                                break;
                            }
                        case BroadCastLoc.World:
                            Program.SendGlobalPackets.Enqueue(new MsgServer.MsgMessage(msg, MsgColor.white, ChatMode.Center).GetArray(stream));
                            break;
                        case BroadCastLoc.WorldY:
                            Program.SendGlobalPackets.Enqueue(new MsgServer.MsgMessage(msg, MsgColor.white, ChatMode.TopLeft).GetArray(stream));
                            break;
                        case BroadCastLoc.Map:
                            foreach (GameClient client in PlayerList.Values.ToArray())
                            {
                                client.SendSysMesage(msg);
                            }
                            break;
                        case BroadCastLoc.YellowMap:
                            foreach (GameClient client in PlayerList.Values.ToArray())
                            {
                                client.SendSysMesage(msg, ChatMode.TopLeft, MsgColor.yellow);
                            }
                            break;
                        case BroadCastLoc.Title:
                            foreach (GameClient client in PlayerList.Values.ToArray())
                            {
                                client.SendSysMesage(msg, ChatMode.FirstRightCorner);
                            }
                            break;
                        case BroadCastLoc.Score:
                            foreach (GameClient client in PlayerList.Values.ToArray())
                            {
                                client.SendSysMesage(msg, ChatMode.ContinueRightCorner);
                            }
                            break;
                        case BroadCastLoc.SpecificMap:
                            foreach (GameClient client in PlayerList.Values.ToArray())
                            {
                                if (client.Player.Map == Map)
                                    client.SendSysMesage(msg, ChatMode.ContinueRightCorner);
                            }
                            break;
                    }
                }
            }
        }
        public virtual void CheckEquipment(GameClient client)
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();

                // Game.MsgServer.MsgGameItem ItemDat;

                //////////////
                //// Check player have some this skill
                //////////////
                if (AllowedSkills != null)
                {
                    if (AllowedSkills.Count > 0)
                    {
                        foreach (var skillid in AllowedSkills)
                        {
                            if (!client.MySpells.ClientSpells.ContainsKey(skillid))
                            {
                                client.MySpells.Add(stream, skillid, 4);
                            }
                        }
                        if (AllowedSkills.Contains(1045) || AllowedSkills.Contains(1046) || AllowedSkills.Contains(1047))
                        {
                            if (client.Player.LeftWeaponId == 0 && client.Player.RightWeaponId != 0)
                            {
                                client.Player.AddSpecialitemR(stream, 410239);
                                client.Player.RightWeapsonSoul = 0;
                                client.Player.View.SendView(client.Player.GetArray(stream, false), true);
                            }
                            else if (client.Player.LeftWeaponId != 0 && client.Player.RightWeaponId != 0)
                            {
                                client.Player.AddSpecialitemR(stream, 410239);
                                client.Player.AddSpecialitemL(stream, 420239);
                                client.Player.RightWeapsonSoul = 0;
                                client.Player.LeftWeapsonSoul = 0;
                                //InEquipment(stream, client, 420239, ConquerItem.AleternanteLeftWeapon);//RoyalSword
                                //InEquipment(stream, client, 410239, ConquerItem.AleternanteRightWeapon);//FrostBlade
                                //client.Equipment.QueryEquipment(true);
                                client.Player.View.SendView(client.Player.GetArray(stream, false), true);
                            }
                            else
                            {
                                client.CreateBoxDialog("Sorr,You Not Equip Any Weapon So Can`t Auto Change It To Blade & Sword");
                            }
                        }
                    }
                }
            }
        }
        private void InEquipment(ServerSockets.Packet stream, GameClient client, uint ID, Role.Flags.ConquerItem position, byte plus = 0, byte bless = 0, byte Enchant = 0
           , Role.Flags.Gem sockone = Flags.Gem.NoSocket
            , Role.Flags.Gem socktwo = Flags.Gem.NoSocket, bool bound = false, Role.Flags.ItemEffect Effect = Flags.ItemEffect.None)
        {
            Database.ItemType.DBItem DbItem;
            if (Database.Server.ItemsBase.TryGetValue(ID, out DbItem))
            {
                Game.MsgServer.MsgGameItem ItemDat = new Game.MsgServer.MsgGameItem();
                ItemDat.UID = Database.Server.ITEM_Counter.Next;
                ItemDat.ITEM_ID = ID;
                ItemDat.Fake = true;
                ItemDat.Effect = Effect;
                ItemDat.Durability = ItemDat.MaximDurability = DbItem.Durability;
                ItemDat.Plus = plus;
                ItemDat.Bless = bless;
                ItemDat.Enchant = Enchant;
                ItemDat.SocketOne = sockone;
                ItemDat.SocketTwo = socktwo;
                ItemDat.Color = (Role.Flags.Color)Program.GetRandom.Next(3, 9);
                ItemDat.Bound = (byte)(bound ? 1 : 0);
                ItemDat.Position = (ushort)position;
                ItemDat.Mode = Flags.ItemMode.AddItem;
                CheakUp(client, ItemDat);
                ItemDat.Send(client, stream);
                client.Send(stream.ItemUsageCreate(MsgItemUsuagePacket.ItemUsuageID.Equip, ItemDat.UID, ItemDat.Position, 0, 0, 0, 0));
            }
        }
        private void CheakUp(GameClient client, Game.MsgServer.MsgGameItem ItemDat)
        {
            uint UID_AleternanteRightWeapon = client.Player.UID + (uint)ConquerItem.AleternanteRightWeapon;
            uint UID_AleternanteLeftWeapon = client.Player.UID + (uint)ConquerItem.AleternanteLeftWeapon;
            Game.MsgServer.MsgGameItem itemdata;
            if (client.Equipment.TryGetEquip((ConquerItem)ItemDat.Position, out itemdata))
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    if (client.Equipment.ClientItems.TryRemove(itemdata.UID, out itemdata))
                        client.Send(stream.ItemUsageCreate(MsgItemUsuagePacket.ItemUsuageID.RemoveEquipment, ItemDat.UID, ItemDat.Position, 0, 0, 0, 0));
                }
            }

            if (ItemDat.UID == 0)
                ItemDat.UID = Database.Server.ITEM_Counter.Next;

            if (!client.Equipment.ClientItems.TryAdd(ItemDat.UID, ItemDat))
            {
                do
                {
                    ItemDat.UID = Database.Server.ITEM_Counter.Next;
                }
                while
                  (client.Equipment.ClientItems.TryAdd(ItemDat.UID, ItemDat) == false);
            }
        }
        /// <summary>
        /// Adds a player to the current PVP Event
        /// </summary>
        /// <param name="c"></param>
        public virtual bool AddPlayer(GameClient client)
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                if (Stage == EventStage.Inviting)
                {
                    if (!GameMap.EventMaps.ContainsKey(client.Player.Map))
                    {
                        if (!client.InQualifier())
                        {
                            if (client.Player.Map != 1038 && client.Player.Map != 6001 && client.Player.Map != 6003)
                            {
                                if (!PlayerList.ContainsKey(client.Player.UID))
                                {
                                    if (!client.Inventory.HaveSpace(5))
                                    {
                                        client.CreateBoxDialog("Please make 5 more spaces in your inventory to join.");
                                    }
                                    else
                                    {
                                        client.Player.PMap = client.Player.Map;
                                        client.Player.PMapX = client.Player.X;
                                        client.Player.PMapY = client.Player.Y;
                                        client.Teleport(53, 65, WaitingArea, DinamicID);

                                        PlayerList.Add(client.Player.UID, client);
                                        PlayerScores.Add(client.Player.UID, 0);

                                        if (client.Player.ContainFlag(MsgServer.MsgUpdate.Flags.Cyclone))
                                            client.Player.RemoveFlag(MsgServer.MsgUpdate.Flags.Cyclone);
                                        if (client.Player.ContainFlag(MsgServer.MsgUpdate.Flags.Fly))
                                            client.Player.RemoveFlag(MsgServer.MsgUpdate.Flags.Fly);
                                        if (client.Player.ContainFlag(MsgServer.MsgUpdate.Flags.Superman))
                                            client.Player.RemoveFlag(MsgServer.MsgUpdate.Flags.Superman);

                                        CheckEquipment(client);
                                        client.Player.SSFB = 0;
                                        //  client.SendSysMesage($"Just wait 60 Seconds and we gonna send you to the event map automatically");
                                        if (!client.Player.Alive)
                                            client.Player.Revive(stream);
                                        return true;
                                    }
                                }
                                else
                                    client.SendSysMesage($"You can't join a PVP event while you're in {EventTitle} Event!");
                            }
                            else
                                client.SendSysMesage("You can't join a PVP event while you're in guild war.");
                        }
                        else
                            client.SendSysMesage("You can't join a PVP Event while you're fighting at the Arena Qualifier!");
                    }
                    else
                        client.SendSysMesage("You can't join a PVP Event while you're fighting at other event!");
                }
                else
                    client.SendSysMesage("There are no events running (gifts cps, items, exp, stone)");
            }
            return false;
        }

        /// <summary>
        /// Removes a player from the current PVP Event
        /// </summary>
        /// 
        public virtual void RemovePlayer(GameClient client, bool Diss = false, bool CanTeleport = true)
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();

                if (PlayerList.ContainsKey(client.Player.UID))
                    PlayerList.Remove(client.Player.UID);
                if (PlayerScores.ContainsKey(client.Player.UID))
                    PlayerScores.Remove(client.Player.UID);

                foreach (KeyValuePair<uint, Dictionary<uint, GameClient>> T in Teams)
                    if (T.Value.ContainsKey(client.Player.UID))
                        T.Value.Remove(client.Player.UID);

                if (!Diss)
                {
                    if (IDEvent == 3)
                    {
                        client.Player.SpecialGarment = 0;
                        client.Player.GarmentId = 0;
                        client.Equipment.Remove(Role.Flags.ConquerItem.Garment, stream);
                    }
                    client.Player.RemoveSpecialitem(stream);
                    client.Player.RemoveSpecialitem1(stream);
                    //client.Equipment.QueryEquipment(false);
                    //client.Player.View.ReSendView(stream);

                    client.EventBase = null;

                    if (client.Player.ContainFlag(MsgServer.MsgUpdate.Flags.Cyclone))
                        client.Player.RemoveFlag(MsgServer.MsgUpdate.Flags.Cyclone);
                    if (client.Player.ContainFlag(MsgServer.MsgUpdate.Flags.Fly))
                        client.Player.RemoveFlag(MsgServer.MsgUpdate.Flags.Fly);
                    if (client.Player.ContainFlag(MsgServer.MsgUpdate.Flags.Superman))
                        client.Player.RemoveFlag(MsgServer.MsgUpdate.Flags.Superman);
                    if (client.Player.ContainFlag(MsgServer.MsgUpdate.Flags.Poisoned))
                        client.Player.RemoveFlag(MsgServer.MsgUpdate.Flags.Poisoned);

                    client.Player.SetPkMode(Role.Flags.PKMode.Capture);
                    client.SendSysMesage("", ChatMode.FirstRightCorner);
                    if (CanTeleport)
                    {
                        if (IDMessage == MsgStaticMessage.Messages.HeroOfGame)
                            client.Teleport(300, 278, 1002, 0);
                        else Program.ExitToTwin(client);
                    }
                    else Broadcast("", Game.MsgEvents.BroadCastLoc.Exit);

                    client.Player.Revive(stream);
                }
            }
        }

        /// <summary>
        /// Used to teleport players to event map
        /// </summary>
        public virtual void TeleportPlayersToMap()
        {
            foreach (GameClient client in PlayerList.Values.ToArray())
            {
                client.Player.SetPkMode(Role.Flags.PKMode.PK);
                ushort x = 0;
                ushort y = 0;
                Map.GetRandCoord(ref x, ref y);
                client.Teleport(x, y, Map.ID, DinamicID);
                if (client.Player.ContainFlag(MsgServer.MsgUpdate.Flags.Cyclone))
                    client.Player.RemoveFlag(MsgServer.MsgUpdate.Flags.Cyclone);
                if (client.Player.ContainFlag(MsgServer.MsgUpdate.Flags.Fly))
                    client.Player.RemoveFlag(MsgServer.MsgUpdate.Flags.Fly);
                if (client.Player.ContainFlag(MsgServer.MsgUpdate.Flags.Superman))
                    client.Player.RemoveFlag(MsgServer.MsgUpdate.Flags.Superman);
                if (client.Player.ContainFlag(MsgServer.MsgUpdate.Flags.Poisoned))
                    client.Player.RemoveFlag(MsgServer.MsgUpdate.Flags.Poisoned);
                client.Player.HitPoints = (int)client.Status.MaxHitpoints;
            }
        }
        /// <summary>
        /// Handles the logic for event protection countdown
        /// </summary>
        /// 
        public virtual void Countdown()
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                if (CountDown == 0 && Stage == EventStage.Countdown)
                {
                    CountDown = 10;
                    Stage = EventStage.Starting;
                    Broadcast(EventTitle + " Event is about to start!", BroadCastLoc.Map);
                }
                else if (CountDown > 0)
                {

                    foreach (GameClient client in PlayerList.Values.ToArray())
                    {
                        client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, new string[1] { $"downnumber{CountDown}" });
                        client.SendSysMesage("", ChatMode.FirstRightCorner);
                    }
                    CountDown--;
                }
                else
                {
                    foreach (GameClient client in PlayerList.Values.ToArray())
                        if (!client.Player.Alive)
                            client.Player.Revive(stream);
                    Stage = EventStage.Fighting;
                    EndTime = DateTime.Now.AddSeconds(Duration);
                    Broadcast(EventTitle + " Event has started! signups are now closed!", BroadCastLoc.World);
                }
            }
        }

        /// <summary>
        /// Handles all the logic during the events and determines conditions to find a winner
        /// </summary>
        public virtual void WaitForWinner()
        {
            foreach (var P in PlayerList.Values.Where(x => !x.Socket.Alive || x.Player.Map != Map.ID).ToArray())
            {
                if (P.Socket.Alive)
                    P.EventBase = null;

                PlayerScores.Remove(P.Player.UID);
                PlayerList.Remove(P.Player.UID);

                foreach (KeyValuePair<uint, Dictionary<uint, GameClient>> T in Teams)
                    if (T.Value.ContainsKey(P.Player.UID))
                        T.Value.Remove(P.Player.UID);
            }
            if (DateTime.Now >= EndTime || (PlayerList.Count <= 1 && EventTitle != "CycloneRace") || Duration <= 0)
                Finish();
            if (DateTime.Now >= LastScores.AddMilliseconds(3000))
                DisplayScore();
        }

        public virtual void TeleAfterRev(GameClient client)
        {
            client.Teleport(0, 0, Map.ID, DinamicID);
        }

        /// <summary>
        /// Handles all the character related checks during the event
        /// </summary>
        /// <param name="C"></param>
        public virtual void CharacterChecks(GameClient client)
        {
            if (client.Player.Map != Map.ID)
                RemovePlayer(client);
        }

        /// <summary>
        /// Announces the event end. Gives each player a small protection and changes stage to over
        /// </summary>
        public void Finish()
        {
            Stage = EventStage.Over;
        }

        /// <summary>
        /// Here we choose who we want to reward and such, may depend on teams or w/e... Should add support for teams
        /// </summary>
        public virtual void End()
        {
            if (PlayerList.Count == 1)
                foreach (var client in PlayerList.Values.ToArray())
                    Reward(client);
            else
                Broadcast(Duration + " minutes have passed and no one won the " + EventTitle + " Event! Better luck next time!", BroadCastLoc.World);

            foreach (var client in PlayerList.Values.ToList())
                RemovePlayer(client);

            PlayerList.Clear();
            PlayerScores.Clear();

            return;
        }

        /// <summary>
        /// Used to choose which rewards we want to give
        /// </summary>
        public virtual void Reward(GameClient client)
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                if (IDMessage == MsgStaticMessage.Messages.KungfuSchool)
                {
                    if (PlayerScores.ContainsKey(client.Player.UID))
                    {
                        int heroscore = PlayerScores[client.Player.UID];
                        if (heroscore > 0)
                        {
                            if (client.Inventory.HaveSpace(1))
                            {
                                client.Inventory.AddItemWitchStack(3008733, 0, 1, stream);
                                client.Player.ConquerPoints += 860;
                                client.Player.BoundConquerPoints += 10;
                                client.Player.PIKAPoint += 1;
                                client.Player.SendUpdate(stream, client.Player.ConquerPoints, MsgUpdate.DataType.ConquerPoints);
                                client.Player.SendUpdate(stream, client.Player.BoundConquerPoints, MsgUpdate.DataType.BoundConquerPoints);
                                string reward = "[EVENTPK] - [ " + client.Player.Name + " ] Get[ DIABLO[Spin] + 860 Cps + 10 Coins + 1 EP ] From " + EventTitle + " Tournament!";
                                Broadcast($"{ client.Player.Name } Gifts [ DIABLO[Spin] + 860 Cps + 10 Coins + 1 EP ] From " + EventTitle + " Tournament!", BroadCastLoc.World);
                                Database.ServerDatabase.LoginQueue.Enqueue(reward);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "sports_victory");
                            }
                            else
                            {
                                client.Inventory.AddReturnedItem(stream, 3008733);
                                client.Player.ConquerPoints += 860;
                                client.Player.BoundConquerPoints += 10;
                                client.Player.PIKAPoint += 1;
                                client.Player.SendUpdate(stream, client.Player.ConquerPoints, MsgUpdate.DataType.ConquerPoints);
                                client.Player.SendUpdate(stream, client.Player.BoundConquerPoints, MsgUpdate.DataType.BoundConquerPoints);
                                string reward = "[EVENTPK] - [ " + client.Player.Name + " ] Get[ DIABLO[Spin] + 860 Cps + 10 Coins + 1 EP ] From " + EventTitle + " Tournament!";
                                Broadcast($"{ client.Player.Name } Gifts [ DIABLO[Spin] + 860 Cps + 10 Coins + 1 EP ] From " + EventTitle + " Tournament!", BroadCastLoc.World);
                                Database.ServerDatabase.LoginQueue.Enqueue(reward);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "sports_victory");
                            }
                        }
                    }
                }
                else if (IDMessage == MsgStaticMessage.Messages.CycloneRace)
                {
                    Rank++;
                    switch (Rank)
                    {
                        case 1:
                            {
                                client.IncreaseExperience(stream, 10000, Role.Flags.ExperienceEffect.angelwing);
                                uint value = RewardCps;
                                client.Player.ConquerPoints += value;
                                client.CreateBoxDialog($"You've received {value} ConquerPoints for Rank {Rank} in {EventTitle} Tournament!.");
                                string msg = $"{client.Player.Name} has won {value} ConquerPoints from Rank {Rank} in the hourly {EventTitle} Tournament!";
                                Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage(msg, MsgColor.red, ChatMode.System).GetArray(stream));
                                string reward = "[EVENT]" + msg;
                                Database.ServerDatabase.LoginQueue.Enqueue(reward);
                                break;
                            }
                        default:
                            {
                                client.IncreaseExperience(stream, 5000, Role.Flags.ExperienceEffect.angelwing);
                                uint value = RewardCps - (Rank * 10);
                                if (value < 5)
                                    value = 5;
                                client.Player.ConquerPoints += value;
                                client.CreateBoxDialog($"You've received {value} ConquerPoints for Rank {Rank} in {EventTitle} Tournament!.");
                                string msg = $"{client.Player.Name} has won {value} ConquerPoints from Rank {Rank} in the hourly {EventTitle} Tournament!";
                                Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage(msg, MsgColor.red, ChatMode.System).GetArray(stream));
                                string reward = "[EVENT]" + msg;
                                Database.ServerDatabase.LoginQueue.Enqueue(reward);
                                break;
                            }
                    }
                }
                else if (IDMessage == MsgStaticMessage.Messages.VALORANT)
                {
                    if (client.Inventory.HaveSpace(1))
                    {
                        client.Inventory.AddItemWitchStack(3008733, 0, 1, stream);
                        client.Player.ConquerPoints += 1500;
                        client.Player.Money += 250000;
                        client.Player.BoundConquerPoints += 20;
                        client.Player.PIKAPoint += 1;
                        client.Player.SendUpdate(stream, client.Player.ConquerPoints, MsgUpdate.DataType.ConquerPoints);
                        client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        client.Player.SendUpdate(stream, client.Player.BoundConquerPoints, MsgUpdate.DataType.BoundConquerPoints);
                        string reward = "[EVENTPK] - [ " + client.Player.Name + " ] Get[ DIABLO[Spin] + 1500 Cps + 250 K Gold + 20 Coins + 1 EP ] From " + EventTitle + " Tournament!";
                        Broadcast($"{ client.Player.Name } Gifts [ DIABLO[Spin] + 1500 Cps + 20 Coins + 250 K Gold + 1 EP ] From " + EventTitle + " Tournament!", BroadCastLoc.World);
                        Database.ServerDatabase.LoginQueue.Enqueue(reward);
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "sports_victory");
                    }
                    else
                    {
                        client.Inventory.AddReturnedItem(stream, 3008733);
                        client.Player.ConquerPoints += 1500;
                        client.Player.Money += 250000;
                        client.Player.BoundConquerPoints += 20;
                        client.Player.PIKAPoint += 1;
                        client.Player.SendUpdate(stream, client.Player.ConquerPoints, MsgUpdate.DataType.ConquerPoints);
                        client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        client.Player.SendUpdate(stream, client.Player.BoundConquerPoints, MsgUpdate.DataType.BoundConquerPoints);
                        string reward = "[EVENTPK] - [ " + client.Player.Name + " ] Get[ DIABLO[Spin] + 1500 Cps + 250 K Gold + 20 Coins + 1 EP ] From " + EventTitle + " Tournament!";
                        Broadcast($"{ client.Player.Name } Gifts [ DIABLO[Spin] + 1500 Cps + 250 K Gold + 20 Coins + 1 EP ] From " + EventTitle + " Tournament!", BroadCastLoc.World);
                        Database.ServerDatabase.LoginQueue.Enqueue(reward);
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "sports_victory");
                    }
                }
                else if (IDMessage == MsgStaticMessage.Messages.HeroOfGame)
                {
                   // HeroOfGame.HeroStatue(client);
                    if (client.Inventory.HaveSpace(1))
                    {
                        client.Inventory.AddItemWitchStack(3008733, 0, 1, stream);
                        client.Player.ConquerPoints += 860;
                        client.Player.BoundConquerPoints += 10;
                        client.Player.PIKAPoint += 1;
                        client.Player.SendUpdate(stream, client.Player.ConquerPoints, MsgUpdate.DataType.ConquerPoints);
                        client.Player.SendUpdate(stream, client.Player.BoundConquerPoints, MsgUpdate.DataType.BoundConquerPoints);
                        string reward = "[EVENTPK] - [ " + client.Player.Name + " ] Get[ DIABLO[Spin] + 860 Cps + 10 Coins + 1 EP ] From " + EventTitle + " Tournament!";
                        Broadcast($"{ client.Player.Name } Gifts [ DIABLO[Spin] + 860 Cps + 10 Coins + 1 EP ] From " + EventTitle + " Tournament!", BroadCastLoc.World);
                        Database.ServerDatabase.LoginQueue.Enqueue(reward);
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "sports_victory");
                    }
                    else
                    {
                        client.Inventory.AddReturnedItem(stream, 3008733);
                        client.Player.ConquerPoints += 860;
                        client.Player.BoundConquerPoints += 10;
                        client.Player.PIKAPoint += 1;
                        client.Player.SendUpdate(stream, client.Player.ConquerPoints, MsgUpdate.DataType.ConquerPoints);
                        client.Player.SendUpdate(stream, client.Player.BoundConquerPoints, MsgUpdate.DataType.BoundConquerPoints);
                        string reward = "[EVENTPK] - [ " + client.Player.Name + " ] Get[ DIABLO[Spin] + 860 Cps + 10 Coins + 1 EP ] From " + EventTitle + " Tournament!";
                        Broadcast($"{ client.Player.Name } Gifts [ DIABLO[Spin] + 860 Cps + 10 Coins + 1 EP ] From " + EventTitle + " Tournament!", BroadCastLoc.World);
                        Database.ServerDatabase.LoginQueue.Enqueue(reward);
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "sports_victory");
                    }
                }
                else
                {
                    if (client.Inventory.HaveSpace(1))
                    {
                        client.Inventory.AddItemWitchStack(3008733, 0, 1, stream);
                        client.Player.ConquerPoints += 860;
                        client.Player.BoundConquerPoints += 10;
                        client.Player.PIKAPoint += 1;
                        client.Player.SendUpdate(stream, client.Player.ConquerPoints, MsgUpdate.DataType.ConquerPoints);
                        client.Player.SendUpdate(stream, client.Player.BoundConquerPoints, MsgUpdate.DataType.BoundConquerPoints);
                        string reward = "[EVENTPK] - [ " + client.Player.Name + " ] Get[ DIABLO[Spin] + 860 Cps + 10 Coins + 1 EP ] From " + EventTitle + " Tournament!";
                        Broadcast($"{ client.Player.Name } Gifts [ DIABLO[Spin] + 860 Cps + 10 Coins + 1 EP ] From " + EventTitle + " Tournament!", BroadCastLoc.World);
                        Database.ServerDatabase.LoginQueue.Enqueue(reward);
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "sports_victory");
                    }
                    else
                    {
                        client.Inventory.AddReturnedItem(stream, 3008733);
                        client.Player.ConquerPoints += 860;
                        client.Player.BoundConquerPoints += 10;
                        client.Player.PIKAPoint += 1;
                        client.Player.SendUpdate(stream, client.Player.ConquerPoints, MsgUpdate.DataType.ConquerPoints);
                        client.Player.SendUpdate(stream, client.Player.BoundConquerPoints, MsgUpdate.DataType.BoundConquerPoints);
                        string reward = "[EVENTPK] - [ " + client.Player.Name + " ] Get[ DIABLO[Spin] + 860 Cps + 10 Coins + 1 EP ] From " + EventTitle + " Tournament!";
                        Broadcast($"{ client.Player.Name } Gifts [ DIABLO[Spin] + 860 Cps + 10 Coins + 1 EP ] From " + EventTitle + " Tournament!", BroadCastLoc.World);
                        Database.ServerDatabase.LoginQueue.Enqueue(reward);
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "sports_victory");
                    }
                }
            }
        }


        /// <summary>
        /// Display the score on the top right corner
        /// </summary>
        public virtual void DisplayScore()
        {
            DisplayScores = DateTime.Now;
            foreach (var player in PlayerList.Values.ToArray())
            {
                player.SendSysMesage($"---------{EventTitle}---------", ChatMode.FirstRightCorner);
            }

            byte Score = 2;
            foreach (var kvp in PlayerScores.OrderByDescending((s => s.Value)).ToArray())
            {
                if (Score == 7)
                    break;
                if (Score == PlayerScores.Count + 2)
                    break;
                Broadcast($"Nº {Score - 1}: {PlayerList[kvp.Key].Player.Name} - {kvp.Value}", BroadCastLoc.Score, Score);
                Score++;
            }
        }

        /// <summary>
        /// Determines if we're supposed to do something when a player gets killed
        /// </summary>
        /// <param name="Attacker"></param>
        /// <param name="Victim"></param>
        public virtual void Kill(GameClient Attacker, GameClient Victim)
        {
            if (PlayerScores.ContainsKey(Attacker.Player.UID))
                PlayerScores[Attacker.Player.UID]++;
        }

        /// <summary>
        /// Determines if we're supposed to do something when a NPC gets killed
        /// </summary>
        /// <param name="Attacker"></param>
        /// <param name="Victim"></param>
        public virtual void Kill(GameClient Attacker, SobNpc Victim)
        {
            if (PlayerScores.ContainsKey(Attacker.Player.UID))
                PlayerScores[Attacker.Player.UID]++;
        }

        /// <summary>
        /// Determines wether if an event has unlimited stamina or not and allow us to track number of sent FB/SS
        /// </summary>
        /// <param name="Attacker"></param>
        /// <param name="SU"></param>
        //public virtual void Shot(GameClient Attacker, Database.MagicType.Magic SU)
        //{
        //    Attacker.Player.Stamina -= SU.UseStamina;
        //}

        /// <summary>
        /// Determines what we're supposed to do when a player gets hit
        /// </summary>
        /// <param name="Attacker"></param>
        /// <param name="Victim"></param>
        public virtual void Hit(GameClient Attacker, GameClient Victim)
        {
            //PlayerScores[Attacker.EntityID]++;
        }

        public virtual void Hit(GameClient client, SobNpc npc, uint Damage)
        {

        }

        /// <summary>
        /// Overrides the melee damage dealt
        /// </summary>
        /// <param name="User"></param>
        /// <param name="C"></param>
        /// <param name="AttackType"></param>
        /// <returns></returns>
        public virtual uint GetDamage(GameClient User, GameClient C)//, AttackType AttackType
        {
            return 1;
        }

        /// <summary>
        /// Sets events' configuration and starts it, possible to determine how long we want signup to last
        /// </summary>
        /// <param name="_signuptime"></param>
        public void StartTournament(int _signuptime = 60)
        {
            if (Program.Events.Find(x => x.EventTitle == EventTitle) == null)
            {
                PlayerList.Clear();
                PlayerScores.Clear();
                CountDown = _signuptime;
                Stage = EventStage.Inviting;
                Program.Events.Add(this);
                if (Map == null)
                {
                  //  GameMap.EnterMap(this.BaseMap);
                    Map = Database.Server.ServerMaps[this.BaseMap];
                }
                if (IDMessage != MsgStaticMessage.Messages.CycloneRace)
                    DinamicID = Map.GenerateDynamicID();
                KillSystem = new KillerSystem();
                if (!Program.FreePkMap.Contains(Map.ID))
                    Program.FreePkMap.Add(Map.ID);
                if (!Program.NoDropItems.Contains(Map.ID))
                    Program.NoDropItems.Add(Map.ID);
                BeginTournament();
            }
        }

        /// <summary>
        /// Initializes tournament
        /// </summary>
        ///         
        public KillerSystem KillSystem;

        public virtual void BeginTournament()
        {
            Broadcast($"{EventTitle} Event has started! Type /joinpvp or /pvp to sign up within the next 60 seconds!", BroadCastLoc.System);
            SendInvitation(60, IDMessage);
        }

        /// <summary>
        /// Tells the server which part of the PVP Event we want to execute next
        /// </summary>
        public void ActionHandler()
        {
            if (Stage == EventStage.Inviting)
            {
                Inviting();
            }
            else if (Stage == EventStage.Countdown || Stage == EventStage.Starting)
                Countdown();
            else if (Stage == EventStage.Fighting)
                WaitForWinner();
            else if (Stage == EventStage.Over)
            {
                Stage = EventStage.None;
                End();
                Program.Events.Remove(this);
            }
        }

        /// <summary>
        /// Handles the logic while the event is on sign-up
        /// </summary>
        public virtual void Inviting()
        {
            if (CountDown > 0)
            {
                if (CountDown == 120)
                    Broadcast(EventTitle + " Event will start in 2 minutes!", BroadCastLoc.World);
                else if (CountDown == 60)
                    Broadcast(EventTitle + " Event will start in 1 minute!", BroadCastLoc.World);
                else if (CountDown == 10)
                {
                    if (!CanStart())
                    {
                        Broadcast("The " + EventTitle + " Event requires atleast " + minplayers + " players to start! Event was cancelled!", BroadCastLoc.World);
                        Broadcast($"---------{EventTitle}---------", BroadCastLoc.Title, 0);
                        Broadcast("Event cancelled", BroadCastLoc.Score, 2);

                        foreach (GameClient client in PlayerList.Values.ToArray())
                        {
                            RemovePlayer(client);
                        }

                        PlayerList.Clear();
                        PlayerScores.Clear();
                        Stage = EventStage.None;
                        Program.Events.Remove(this);
                        return;
                    }
                    Broadcast("10 seconds until start", BroadCastLoc.Map);
                }
                else if (CountDown < 6)
                    Broadcast(CountDown.ToString() + " seconds until start", BroadCastLoc.Map);
                foreach (GameClient client in PlayerList.Values.ToArray())
                {
                    if (client.Player.Map != WaitingArea || !client.Socket.Alive)
                        RemovePlayer(client, false, false);
                }
                Broadcast($"---------{EventTitle}---------", BroadCastLoc.Title, 0);
                TimeSpan T = TimeSpan.FromSeconds(CountDown);
                Broadcast($"Total Players: " + PlayerList.Count(), BroadCastLoc.Score, 2);
                Broadcast($"Start in: " + T.ToString(@"mm\:ss") + "", BroadCastLoc.Score, 2);
                --CountDown;
            }
            else
            {
                Stage = EventStage.Countdown;
                TeleportPlayersToMap();
            }
        }
        public virtual void AddPlayerTitle(GameClient client)
        {
            if (client.EventBase == null)
            {
                var events = Program.Events.Find(x => x.EventTitle == EventTitle);
                client.EventBase = events;
                AddPlayer(client);
            }
        }

        public virtual void SendInvitation(int Seconds, Game.MsgServer.MsgStaticMessage.Messages messaj = Game.MsgServer.MsgStaticMessage.Messages.None)
        {
            string Message = $"{EventTitle} is about to begin! Will you join it?";
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();

                foreach (var client in Database.Server.GamePoll.Values)
                {
                    if (!client.Player.OnMyOwnServer || client.IsConnectedInterServer() || client.Player.Map == 1038 || client.Player.Map == 6001 || client.Player.Map == 6003 || client.InQualifier())
                        continue;
                    if (client.Player.Map == 1616 || client.Player.Map == Map.ID)
                        continue;
                    client.Player.LastMan = 0;
                    client.Player.DragonWar = 0;
                    client.Player.Infection = 0;
                    client.Player.FreezeWar = 0;
                    client.Player.Kungfu = 0;
                    client.Player.Get5Out = 0;
                    client.Player.SSFB = 0;
                    client.Player.TheCaptain = 0;
                    client.Player.WhackTheThief = 0;
                    client.Player.VampireWar = 0;

                    client.Player.MessageBox(Message, new Action<Client.GameClient>(user => AddPlayerTitle(user)), null, Seconds, messaj);
                }
            }
        }

        public bool InTournament(GameClient user, bool checkmap = false, uint MapID = 0, uint dMapID = 0)
        {
            if (Map == null)
                return false;
            if (checkmap)
            {
                return MapID == Map.ID && dMapID == DinamicID || MapID == WaitingArea;
            }
            return user.Player.Map == Map.ID && user.Player.DynamicID == DinamicID || user.Player.Map == WaitingArea;
        }

        public void RevivePlayer(GameClient client, int amount = 4)
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                if (client.Player.DeadStamp.AddSeconds(amount) < Extensions.Time32.Now)
                {
                    ushort x = 0; ushort y = 0;
                    Map.GetRandCoord(ref x, ref y);
                    client.Teleport(x, y, Map.ID, DinamicID);
                    client.Player.Revive(stream);
                }
            }
        }
        /// <summary>
        /// Do all the requirement checks to start the event in here
        /// </summary>
        /// <returns></returns>
        public virtual bool CanStart()
        {
            return PlayerList.Count >= minplayers;
        }

        /// <summary>
        /// Chane PK Mode
        /// </summary>
        /// 

        public void ChangePKMode(GameClient client, PKMode Mode)
        {
            //client.Player.SetPkMode(Mode);
        }
    }
}

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeathWish.Client;
using DeathWish.Database;
using DeathWish.Game.MsgServer;
using DeathWish.Game.MsgTournaments;
using DeathWish.Role;
using DeathWish.Role.Instance;
using DeathWish.ServerSockets;
using Extensions;

namespace DeathWish.Game.AISystem
{
    public unsafe class AIBotHandle
    {
        public static void Action()
        {
            foreach (var Bot in AIBot.Pool.Values)
            {
                try
                {
                    if (Bot != null)
                    {
                        if (Bot.MyAI.Type == BotType.AIRoom)
                        {
                            Bot.MyAI.GetTargetAIRoom();
                            Bot.MyAI.JumpingAIRoom();
                            Bot.MyAI.AttackingAIRoom();
                            Bot.MyAI.RemoveingAIRoom();
                        }
                        //else if (Bot.MyAI.Type == BotType.AIAutoHuntOffline)
                        //{
                        //    Bot.MyAI.AttackingAutoHunt();
                        //    Bot.MyAI.JumpingAutoHunt();
                        //    Bot.MyAI.RemoveingAutoHunt();
                        //}
                    }
                }
                catch
                {
                    continue;
                }
            }
        }
        public static uint[] Body = { 1003, 1004, 2001, 2002 };
        public static uint[] ClasesCustom = { 10, 15 };
        public static string[] BotsName = { "DIABLO[Bot]", "PIKATCHU[Bot]", "Jack[Bot]" };
        public static Counter UIDCounter = new Counter(30000000);
        public static Counter UIDAutoHunt = new Counter(60000000);
        public static GameClient CreateBots(Packet stream, GameClient User, BotLevel Level, BotType Type, BotSkill skills/*,byte Class*/)
        {
            GameClient Bot = Clases();
            Bot.MyAI.Bot = Bot;
            Bot.MyAI.Client = User;
            Bot.MyAI.SetLevel(Level);
            switch (Type)
            {
                case BotType.AIRoom:
                    {
                        //Bot.Player.Class = Class;
                        Bot.Player.Class = 15;
                        Bot.Player.DynamicID = User.Player.DynamicID;
                        Bot.Player.X = (ushort)User.Player.X;
                        Bot.Player.Y = (ushort)User.Player.Y;
                        Bot.Player.Map = (ushort)User.Player.Map;
                        Bot.Map = User.Map;
                        Bot.MyAI.Attack = skills;
                        Bot.Player.Level = (ushort)User.Player.Level;
                        Bot.Player.Face = (ushort)User.Player.Face;
                        Bot.Player.Hair = (ushort)User.Player.Hair;
                        Bot = RellenarInformacion(Bot);
                        User.Send(Bot.Player.GetArray(stream, false));
                        Bot.FullLoading = true;
                        Bot.Player.CompleteLogin = true;
                        Bot.Player.PkMode = Flags.PKMode.PK;
                        Bot.MyAI.Bot = Bot;
                        Bot.Map.AddCustom(Bot);
                        BotsEquipment(stream, Bot);
                        Bot.Equipment.OnDequeue();
                        break;
                    }
            }
            return null;
        }
        //public static unsafe void CreateBots(ServerSockets.Packet stream, Client.GameClient USER, int Year = 0, int Month = 0, int Day = 0, int Hour = 0, int Minute = 0, int Second = 0)
        //{
        //    if (Database.Server.GamePoll.ContainsKey((uint)(60000000 + USER.Player.UID)))
        //    {
        //        USER.CreateBoxDialog("You cannot use more than one");
        //    }
        //    else
        //    {
        //        Client.GameClient Bot = Clases();
        //        Bot.MyAI.Bot = Bot;
        //        Bot.Player.Class = 45;
        //        Bot.Player.VipLevel = (byte)(USER.Player.VipLevel- 1);
        //        Bot.Player.Name = USER.Player.Name + "[Auto#37Hunt]";
        //        Bot.Player.UID = (uint)(60000000 + USER.Player.UID);
        //        Bot.Player.DynamicID = USER.Player.DynamicID;
        //        Bot.Player.X = (ushort)USER.Player.X;
        //        Bot.Player.Y = USER.Player.Y;
        //        //Role.GameMap.EnterMap((int)Bot.Player.Map);
        //        Bot.MyAI.Timer = new DateTime();
        //        if (Year != 0)
        //            Bot.MyAI.Timer = DateTime.Now.AddYears(Year);
        //        if (Month != 0)
        //            Bot.MyAI.Timer = DateTime.Now.AddMonths(Month);
        //        if (Day != 0)
        //            Bot.MyAI.Timer = DateTime.Now.AddDays(Day);
        //        if (Hour != 0)
        //            Bot.MyAI.Timer = DateTime.Now.AddHours(Hour);
        //        if (Minute != 0)
        //            Bot.MyAI.Timer = DateTime.Now.AddMinutes(Minute);
        //        if (Second != 0)
        //            Bot.MyAI.Timer = DateTime.Now.AddSeconds(Second);
        //        Bot.Player.Angle = USER.Player.Angle;
        //        Bot.Player.AttackStamp = Extensions.Time32.Now;
        //        Bot.Player.Map = (ushort)USER.Player.Map;
        //        Bot.Player.DynamicID = USER.Player.DynamicID;
        //        Bot.Map = Database.Server.ServerMaps[USER.Player.Map];
        //        Bot.Player.Level = USER.Player.Level;
        //        Bot.Player.Face = USER.Player.Face;
        //        Bot.Player.Hair = USER.Player.Hair;
        //        Bot.Player.CountryID = USER.Player.CountryID;
        //        Bot.Player.Body = USER.Player.Body;
        //        Bot.Player.Reborn = USER.Player.Reborn;
        //        Bot.Player.Agility = USER.Player.Agility;
        //        Bot.Player.Spirit = USER.Player.Spirit;
        //        Bot.Player.Strength = USER.Player.Strength;
        //        Bot.Player.Vitality = USER.Player.Vitality;
        //        Bot.Player.CountryID = USER.Player.CountryID;
        //        Bot = RellenarInformacionAutoHunt(Bot);
        //        Bot.Player.View.SendView(Bot.Player.GetArray(stream, false), false);
        //        Bot.FullLoading = true;
        //        Bot.Player.CompleteLogin = true;
        //        Bot.Fake = true;
        //        Bot.Team = new Role.Instance.Team(Bot);
        //        Bot.MyAI.Bot = Bot;
        //        LoadJiangHu(Bot);
        //        LoadInnerPower(Bot);
        //        LoadSubClass(Bot);
        //        Bot.Map.AddAI(Bot);
        //        BotsEquipmentAutoHunt(stream, Bot);
        //        Bot.Equipment.OnDequeue();
        //        Bot.MyAI.Type = BotType.AIAutoHuntOffline;
        //        Database.Server.GamePoll.TryAdd(Bot.Player.UID, Bot);
        //        Bot.Map.AddCustomHunt(Bot);
        //        Bot.Player.Revive(stream);
        //        Bot.MyAI.LastAttack = System.Time32.Now;
        //        Bot.Player.View.Role(true);
        //        Bot.Player.PkMode = Role.Flags.PKMode.Peace;
        //    }
        //}
        public static void BotsEquipment(Packet stream, GameClient pclient)
        {
            pclient.Equipment.Add(stream, (uint)360354, Flags.ConquerItem.LeftWeaponAccessory, 0, 1, 0, (Flags.Gem)0, (Flags.Gem)0);//
            pclient.Equipment.Add(stream, (uint)360355, Flags.ConquerItem.RightWeaponAccessory, 0, 1, 0, (Flags.Gem)0, (Flags.Gem)0);//
            pclient.Equipment.Add(stream, (uint)2100075, Flags.ConquerItem.Bottle, 0, 1, 0, (Flags.Gem)0, (Flags.Gem)0);//
            pclient.Equipment.Add(stream, (uint)200001, Flags.ConquerItem.SteedMount, 0, 1, 0, (Flags.Gem)0, (Flags.Gem)0);//
            pclient.Equipment.Add(stream, 300000, Flags.ConquerItem.Steed, 12);//Steed
            pclient.Equipment.Add(stream, 203009, Flags.ConquerItem.RidingCrop, 12, 1);//Crop
            pclient.Equipment.Add(stream, 201009, Flags.ConquerItem.Fan, 12, 1, 0, (Flags.Gem)103, (Flags.Gem)103);//Fan
            pclient.Equipment.Add(stream, 202009, Flags.ConquerItem.Tower, 12, 1, 0, (Flags.Gem)123, (Flags.Gem)123);//Tower
            pclient.Equipment.Add(stream, 120269, Flags.ConquerItem.Necklace, 12, 7, 255, (Flags.Gem)13, (Flags.Gem)13);//Necklace
            pclient.Equipment.Add(stream, (uint)150269, Flags.ConquerItem.Ring, 12, 7, 255, (Flags.Gem)13, (Flags.Gem)13);//Ring
            pclient.Equipment.Add(stream, (uint)160249, Flags.ConquerItem.Boots, 12, 7, 255, (Flags.Gem)13, (Flags.Gem)13);//Boot
            if (AtributesStatus.IsTrojan(pclient.Player.Class))
            {
                pclient.Equipment.Add(stream, (uint)410439, Flags.ConquerItem.RightWeapon, 12, 7, 255, (Flags.Gem)13, (Flags.Gem)13);//SkyBlade
                pclient.Equipment.Add(stream, (uint)410439, Flags.ConquerItem.LeftWeapon, 12, 7, 255, (Flags.Gem)13, (Flags.Gem)13);//SquallSword
                pclient.Equipment.Add(stream, (uint)130309, Flags.ConquerItem.Armor, 12, 7, 255, (Flags.Gem)13, (Flags.Gem)13);//ObsidianArmor
                pclient.Equipment.Add(stream, (uint)118309, Flags.ConquerItem.Head, 12, 7, 255, (Flags.Gem)13, (Flags.Gem)13);//PeerlessCoronet
                pclient.MySpells.Add(stream, 1045, 4, 0, 0);
                pclient.MySpells.Add(stream, 1046, 4, 0, 0);
                pclient.MySpells.Add(stream, 1115, 4, 0, 0);
                pclient.MySpells.Add(stream, 1095, 4, 0, 0);
                pclient.MySpells.Add(stream, 1090, 4, 0, 0);
            }
        }
        //public static void BotsEquipmentAutoHunt(Packet stream, GameClient pclient)
        //{
        //    pclient.Equipment.Add(stream, (uint)360354, Flags.ConquerItem.LeftWeaponAccessory, 0, 1, 0, (Flags.Gem)0, (Flags.Gem)0);//
        //    pclient.Equipment.Add(stream, (uint)360355, Flags.ConquerItem.RightWeaponAccessory, 0, 1, 0, (Flags.Gem)0, (Flags.Gem)0);//
        //    pclient.Equipment.Add(stream, (uint)2100075, Flags.ConquerItem.Bottle, 0, 1, 0, (Flags.Gem)0, (Flags.Gem)0);//
        //    pclient.Equipment.Add(stream, (uint)200001, Flags.ConquerItem.SteedMount, 0, 1, 0, (Flags.Gem)0, (Flags.Gem)0);//
        //    pclient.Equipment.Add(stream, 300000, Flags.ConquerItem.Steed, 2);//Steed
        //    pclient.Equipment.Add(stream, 203009, Flags.ConquerItem.RidingCrop, 2, 1);//Crop
        //    pclient.Equipment.Add(stream, 201009, Flags.ConquerItem.Fan, 2, 1, 0, (Flags.Gem)103, (Flags.Gem)103);//Fan
        //    pclient.Equipment.Add(stream, 202009, Flags.ConquerItem.Tower, 2, 1, 0, (Flags.Gem)123, (Flags.Gem)123);//Tower
        //    pclient.Equipment.Add(stream, 120269, Flags.ConquerItem.Necklace, 2, 7, 255, (Flags.Gem)13, (Flags.Gem)13);//Necklace
        //    pclient.Equipment.Add(stream, (uint)150269, Flags.ConquerItem.Ring, 2, 7, 255, (Flags.Gem)13, (Flags.Gem)13);//Ring
        //    pclient.Equipment.Add(stream, (uint)160249, Flags.ConquerItem.Boots, 2, 7, 255, (Flags.Gem)13, (Flags.Gem)13);//Boot
        //    if (AtributesStatus.IsArcher(pclient.Player.Class))
        //    {
        //        pclient.Equipment.Add(stream, (uint)613429, Flags.ConquerItem.RightWeapon, 2, 7, 255, (Flags.Gem)13, (Flags.Gem)13);//Knife
        //        pclient.Equipment.Add(stream, (uint)613429, Flags.ConquerItem.LeftWeapon, 2, 7, 255, (Flags.Gem)13, (Flags.Gem)13);//Knife
        //        pclient.Equipment.Add(stream, (uint)133309, Flags.ConquerItem.Armor, 2, 7, 255, (Flags.Gem)13, (Flags.Gem)13);//Armor
        //        pclient.Equipment.Add(stream, (uint)113309, Flags.ConquerItem.Head, 2, 7, 255, (Flags.Gem)13, (Flags.Gem)13);//Head
        //        pclient.MySpells.Add(stream, (ushort)Role.Flags.SpellID.ScatterFire, 4, 0, 0);
        //        pclient.Equipment.QueryEquipment(pclient.Equipment.Alternante, true);
        //    }
        //}
        //public static Client.GameClient RellenarInformacionAutoHunt(Client.GameClient pclient)
        //{
        //    pclient.Player.Body = (ushort)Body[Program.GetRandom.Next(0, Body.Length)];
        //    pclient.Player.HitPoints = pclient.MyAI.HP + 50000;
        //    pclient.Player.Mana = 8000;
        //    pclient.Status.MaxMana = pclient.Player.Mana;
        //    pclient.Status.MaxHitpoints = (uint)pclient.Player.HitPoints;
        //    pclient.Player.NobilityRank = Role.Instance.Nobility.NobilityRank.Earl;
        //    pclient.Player.Angle = Role.Flags.ConquerAngle.SouthEast;
        //    pclient.Player.InnerPower = new Role.Instance.InnerPower(pclient.Player.Name, pclient.Player.UID);
        //    pclient.Player.ServerID = (ushort)Database.GroupServerList.MyServerInfo.ID;
        //    Database.DataCore.AtributeStatus.GetStatus(pclient.Player);
        //    pclient.Player.Strength = 900;
        //    pclient.Player.Vitality = 900;
        //    pclient.Player.Agility = 900;
        //    pclient.Player.Spirit = 900;
        //    pclient.Player.Reborn = 2;
        //    pclient.Player.FirstClass = 135;
        //    pclient.Player.SecoundeClass = 25;
        //    pclient.Player.Flowers = new Role.Instance.Flowers(pclient.Player.UID, pclient.Player.Name);
        //    pclient.MyHouse = new Role.Instance.House(pclient.Player.UID);
        //    pclient.Player.Action = Role.Flags.ConquerAction.Sit;
        //    return pclient;
        //}
        public static GameClient Clases()
        {
            GameClient pclient = new GameClient(null);
            pclient.Fake = true;
            pclient.Player = new Role.Player(pclient);
            pclient.Inventory = new Inventory(pclient);
            pclient.Equipment = new Equip(pclient);
            pclient.Warehouse = new Warehouse(pclient);
            pclient.MySpells = new Spell(pclient);
            pclient.MyProfs = new Proficiency(pclient);
            pclient.Status = new MsgStatus();
            pclient.MyVendor = new Vendor(pclient);
            pclient.Player.SubClass = new SubClass();
            pclient.Player.Nobility = new Nobility(pclient);
            pclient.Player.Away = 0;
            pclient.Player.View = new RoleView(pclient);
            pclient.Player.Associate = new Associate.MyAsociats(pclient.Player.UID);
            pclient.Player.MyClan = new Clan();
            pclient.Player.QuestGUI = new Quests(pclient.Player);
            pclient.Team = new Team(pclient);
            pclient.MyTrade = new Trade(pclient);
            pclient.Confiscator = new Confiscator();
            pclient.ExtraStatus = new ConcurrentDictionary<RoleStatus.StatuTyp, RoleStatus>();
            pclient.ArenaStatistic = new MsgArena.User();
            pclient.DemonExterminator = new DemonExterminator();
            pclient.Player.MyChi = new Role.Instance.Chi(pclient.Player.UID);
            pclient.Player.MyUnion = new Union();
            return pclient;
        }
        public static GameClient RellenarInformacion(GameClient pclient)
        {
        TryOtherName:
            string name = BotsName[Program.GetRandom.Next(0, BotsName.Length)];
            foreach (var ExitsClient in AIBot.Pool.Values)
            {
                if (ExitsClient.Player.Name == name)
                    goto TryOtherName;
            }
            pclient.Player.Name = name;
            pclient.Player.Body = (ushort)Body[Program.GetRandom.Next(0, Body.Length)];
            pclient.Player.HitPoints = pclient.MyAI.HP + 50000;
            pclient.Status.MaxHitpoints = (uint)pclient.Player.HitPoints;
            pclient.Player.VipLevel = 7;
            pclient.Player.NobilityRank = Nobility.NobilityRank.King;
            pclient.Player.Angle = Role.Flags.ConquerAngle.SouthEast;
            pclient.Player.InnerPower = new InnerPower(pclient.Player.Name, pclient.Player.UID);
            pclient.Player.ServerID = (ushort)GroupServerList.MyServerInfo.ID;
            DataCore.AtributeStatus.GetStatus(pclient.Player);
            pclient.Player.CountryID = (ushort)Program.GetRandom.Next(1, 50);
            pclient.Player.UID = UIDCounter.Next;
            pclient.Player.Reborn = 2;
            pclient.Player.FirstClass = 135;
            pclient.Player.SecoundeClass = 25;
            pclient.Player.Flowers = new Role.Instance.Flowers(pclient.Player.UID, pclient.Player.Name);
            pclient.MyHouse = new Role.Instance.House(pclient.Player.UID);
            pclient.Player.Action = Flags.ConquerAction.Sit;
            return pclient;
        }
        //public static void LoadJiangHu(Client.GameClient user)
        //{
        //    var jiang = new Role.Instance.JiangHu(user.Player.UID);
        //    jiang.Name = user.Player.Name;
        //    jiang.CustomizedName = user.Player.Name;
        //    jiang.Level = (byte)user.Player.Level;
        //    jiang.Talent = 0;
        //    jiang.FreeTimeToday = 10;
        //    jiang.OnJiangMode = false;
        //    jiang.FreeCourse = 0;
        //    jiang.StartCountDwon = DateTime.Now;
        //    jiang.CountDownEnd = DateTime.Now;
        //    jiang.RoundBuyPoints = 0;
        //    uint _Stage = 0;
        //    byte Level = 0;
        //    var Type = Role.Instance.JiangHu.Stage.AtributesType.None;
        //    foreach (var Stage in jiang.ArrayStages)
        //    {
        //        Stage.Activate = true;
        //        foreach (var Star in Stage.ArrayStars)
        //        {
        //            switch (_Stage)
        //            {
        //                case 0:
        //                    {
        //                        Type = Role.Instance.JiangHu.Stage.AtributesType.Breakthrough;
        //                        Level = 6;
        //                        break;
        //                    }
        //                case 1:
        //                    {
        //                        Type = Role.Instance.JiangHu.Stage.AtributesType.Immunity;
        //                        Level = 6;
        //                        break;
        //                    }
        //                case 2:
        //                    {
        //                        Type = Role.Instance.JiangHu.Stage.AtributesType.PAttack;
        //                        Level = 6;
        //                        break;
        //                    }
        //                case 3:
        //                    {
        //                        Type = Role.Instance.JiangHu.Stage.AtributesType.CriticalStrike;
        //                        Level = 6;
        //                        break;
        //                    }
        //                case 4:
        //                    {
        //                        Type = Role.Instance.JiangHu.Stage.AtributesType.MaxLife;
        //                        Level = 6;
        //                        break;
        //                    }
        //                case 5:
        //                    {
        //                        Type = Role.Instance.JiangHu.Stage.AtributesType.Immunity;
        //                        Level = 6;
        //                        break;
        //                    }
        //                case 6:
        //                    {
        //                        Type = Role.Instance.JiangHu.Stage.AtributesType.PAttack;
        //                        Level = 6;
        //                        break;
        //                    }
        //                case 7:
        //                    {
        //                        Type = Role.Instance.JiangHu.Stage.AtributesType.CriticalStrike;
        //                        Level = 6;
        //                        break;
        //                    }
        //                case 8:
        //                    {
        //                        Type = Role.Instance.JiangHu.Stage.AtributesType.MaxLife;
        //                        Level = 6;
        //                        break;
        //                    }
        //            }
        //            Star.Activate = true;
        //            Star.UID = jiang.ValueToRoll(Type, Level);
        //            Star.Typ = jiang.GetValueType(Star.UID);
        //            Star.Level = jiang.GetValueLevel(Star.UID);
        //        }
        //        _Stage++;
        //    }
        //    jiang.CreateStatusAtributes(user);
        //    user.Player.MyJiangHu = jiang;
        //}
        //public static void LoadInnerPower(Client.GameClient client)
        //{
        //    Database.InnerPowerTable.Stage DBStage = null;
        //    foreach (var m_stage in client.Player.InnerPower.Stages)
        //    {
        //        foreach (var m_gong in m_stage.NeiGongs)
        //        {
        //            m_stage.UnLocked = m_gong.Unlocked = true;
        //            m_gong.Score = 100;
        //            Database.InnerPowerTable.Stage.NeiGong DBGong = null;
        //            if (Database.InnerPowerTable.GetDBInfo(m_gong.ID, out DBStage, out DBGong))
        //            {
        //                m_gong.level = DBGong.MaxLevel;
        //                m_gong.Complete = m_gong.level == DBGong.MaxLevel;
        //            }
        //        }
        //        client.Send(new ServerSockets.RecycledPacket().GetStream().InnerPowerGui(client.Player.InnerPower.GetNeiGongs()));
        //        client.Send(new ServerSockets.RecycledPacket().GetStream().InnerPowerStageInfo(InnerPowerStage.ActionID.UpdateStage, client.Player.UID, m_stage));
        //        client.Send(new ServerSockets.RecycledPacket().GetStream().InnerPowerStageInfo(InnerPowerStage.ActionID.UpdateScore, client.Player.UID, m_stage));
        //        client.Player.InnerPower.UpdateStatus();
        //        client.Equipment.QueryEquipment(client.Equipment.Alternante, false);
        //        Role.Instance.InnerPower.InnerPowerRank.UpdateRank(client.Player.InnerPower);
        //    }
        //    client.Player.InnerPower.AddPotency(new ServerSockets.RecycledPacket().GetStream(), client, 0);
        //}
        //public static void LoadSubClass(Client.GameClient client)
        //{
        //    #region Namber [1]

        //    client.Player.SubClass.LearnNewSubClass(Database.DBLevExp.Sort.MartialArtist, client, new ServerSockets.RecycledPacket().GetStream());
        //    client.Player.SubClass.LearnNewSubClass(Database.DBLevExp.Sort.Warlock, client, new ServerSockets.RecycledPacket().GetStream());
        //    client.Player.SubClass.LearnNewSubClass(Database.DBLevExp.Sort.ChiMaster, client, new ServerSockets.RecycledPacket().GetStream());
        //    client.Player.SubClass.LearnNewSubClass(Database.DBLevExp.Sort.Sage, client, new ServerSockets.RecycledPacket().GetStream());
        //    client.Player.SubClass.LearnNewSubClass(Database.DBLevExp.Sort.Apothecary, client, new ServerSockets.RecycledPacket().GetStream());
        //    client.Player.SubClass.LearnNewSubClass(Database.DBLevExp.Sort.Wrangler, client, new ServerSockets.RecycledPacket().GetStream());
        //    client.Player.SubClass.LearnNewSubClass(Database.DBLevExp.Sort.Performer, client, new ServerSockets.RecycledPacket().GetStream());

        //    #endregion Namber [1]

        //    #region Namber [2]

        //    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.MartialArtist, 9, client, new ServerSockets.RecycledPacket().GetStream());
        //    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Warlock, 9, client, new ServerSockets.RecycledPacket().GetStream());
        //    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.ChiMaster, 9, client, new ServerSockets.RecycledPacket().GetStream());
        //    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Sage, 9, client, new ServerSockets.RecycledPacket().GetStream());
        //    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Apothecary, 9, client, new ServerSockets.RecycledPacket().GetStream());
        //    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Wrangler, 9, client, new ServerSockets.RecycledPacket().GetStream());
        //    client.Player.SubClass.SetPhrase(Database.DBLevExp.Sort.Performer, 9, client, new ServerSockets.RecycledPacket().GetStream());

        //    #endregion Namber [2]

        //    #region Namber [3]

        //    client.Player.SubClass.SetLevelss(Database.DBLevExp.Sort.MartialArtist, 9, client, new ServerSockets.RecycledPacket().GetStream());
        //    client.Player.SubClass.SetLevelss(Database.DBLevExp.Sort.Warlock, 9, client, new ServerSockets.RecycledPacket().GetStream());
        //    client.Player.SubClass.SetLevelss(Database.DBLevExp.Sort.ChiMaster, 9, client, new ServerSockets.RecycledPacket().GetStream());
        //    client.Player.SubClass.SetLevelss(Database.DBLevExp.Sort.Sage, 9, client, new ServerSockets.RecycledPacket().GetStream());
        //    client.Player.SubClass.SetLevelss(Database.DBLevExp.Sort.Apothecary, 9, client, new ServerSockets.RecycledPacket().GetStream());
        //    client.Player.SubClass.SetLevelss(Database.DBLevExp.Sort.Wrangler, 9, client, new ServerSockets.RecycledPacket().GetStream());
        //    client.Player.SubClass.SetLevelss(Database.DBLevExp.Sort.Performer, 9, client, new ServerSockets.RecycledPacket().GetStream());

        //    #endregion Namber [3]
        //}
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using DeathWish.Game.MsgServer;
using System.Windows.Forms;

namespace DeathWish.Database
{
    public class ServerDatabase
    {
        public static void ResetingEveryDay(Client.GameClient client)
        {
            try
            {
                if (DateTime.Now.DayOfYear != client.Player.Day)
                {
                    client.Player.UseChiToken = 0;
                    client.Player.StoneLand = 0;
                    client.Player.MiningAttempts = 200;
                    client.Player.ChangeEpicTrojan = client.Player.ChangeArrayEpicTrojan =
                        client.Player.ChangeMr_MirrorEpicTrojan = client.Player.ChangeGeneralPakEpicTrojan  = 0;
                    client.Player.CanChangeEpicMaterial = client.Player.CanChangeArrayEpicMaterial  =
                        client.Player.CanChangeMr_MirrorEpicMaterial = client.Player.CanChangGeneralPakMaterial = 1;
                    client.Player.ConquerPointsLimitaion = 0;
                    client.Player.TodayChampionPoints = 0;
                    client.MyExchangeShop.Reset();
                    if (client.Player.DailyMonth == 0)
                        client.Player.DailyMonth = (byte)DateTime.Now.Month;
                    if (client.Player.DailyMonth != DateTime.Now.Month)
                    {
                        client.Player.DailySignUpRewards = 0;
                        client.Player.DailySignUpDays = 0;
                        client.Player.DailyMonth = (byte)DateTime.Now.Month;
                    }
                    //if (client.Player.MyJiangHu != null)
                    //{
                    //    client.Player.MyJiangHu.FreeCourse = 30000;
                    //    //client.Player.MyJiangHu.FreeTimeToday = 10; //سليمان 10 مرات جيانج في اليوم
                    //    client.Player.MyJiangHu.RoundBuyPoints = 0;
                    //}
                    if (client.Activeness != null)
                    {
                        client.Activeness.ResetDaily();
                    }
                    client.Player.MisShoot = client.Player.HitShoot = 0;
                    client.Player.ArenaDeads = client.Player.ArenaKills = 0;
                    client.Player.MisSBShoot = client.Player.HitSBShoot = 0;
                    client.Player.ArenaSBDeads = client.Player.ArenaSBKills = 0;
                    client.Player.TowerOfMysterychallenge = 3;
                    client.Player.TOMChallengeToday = 0;
                    client.Player.TowerOfMysteryChallengeFlag = 0;
                    client.Player.TOMSelectChallengeToday = 0;
                    client.Player.ClaimTowerAmulets = 0;
                    client.Player.TOMClaimTeamReward = 0;
                    client.Player.AutoHuntMinutes = 0;
                    client.Player.TOMRefreshReward = 0;
                    client.Player.EGWIN = 0;
                    client.Player.QuestGUI.RemoveQuest(6126);
                    client.Player.GiantGhasomKill = 0;
                    client.Player.GiantGhasomKillClaimed = false;
                    client.Player.ClaimedBCPToday = false;
                    client.Player.FateFruitUsed = 0;
                    client.Player.WrongAnswers = new int[5] { 3, 3, 3, 3, 3 };
                    client.Player.OpenHousePack = 0;
                    client.Player.AnswerdToday = 0;
                    client.Player.DbTry = false;
                    client.Player.MSConquer = false;
                    client.Player.MRConquer = false;
                    client.Player.LotteryEntries = 0;
                    client.Player.BDExp = 0;
                    client.Player.ExpBallUsed = 0;
                    client.Player.ExpFruit = 0;
                    client.Player.SwordMaster = 0;
                    client.Player.TeratoDragon = 0;
                    client.Player.PurbleBanshee = 0;
                    client.Player.TCCaptainTimes = 0;
                    client.DemonExterminator.FinishToday = 0;
                    client.Player.helpladytime = 0;
                    client.Player.BattleFieldPoints = 0;
                    client.Player.EpicQuestChance = 0;
                    if (client.Player.MyChi != null && DateTime.Now.DayOfYear > client.Player.Day)
                        client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + Math.Min(((DateTime.Now.DayOfYear - client.Player.Day) * 300), 4000);
                    else
                        client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 300;
                    //client.Player.Flowers.FreeFlowers += 1;
                    foreach (var flower in client.Player.Flowers)
                        flower.Amount2day = 0;
                    client.Player.LateSignIn = client.Player.VipLevel;
                    if (client.Player.Level >= 90)
                    {
                        client.Player.Enilghten = CalculateEnlighten(client.Player);
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            client.Player.SendUpdate(stream, client.Player.Enilghten, Game.MsgServer.MsgUpdate.DataType.EnlightPoints);
                        }
                    }
                    client.Player.BuyKingdomDeeds = 0;
                    client.Player.QuestGUI.RemoveQuest(35024);
                    client.Player.QuestGUI.RemoveQuest(35007);
                    client.Player.QuestGUI.RemoveQuest(35025);
                    client.Player.QuestGUI.RemoveQuest(35028);
                    client.Player.QuestGUI.RemoveQuest(35034);
                    //---- reset Quests
                    client.Player.QuestGUI.RemoveQuest(6390);
                    client.Player.QuestGUI.RemoveQuest(6329);
                    client.Player.QuestGUI.RemoveQuest(6245);
                    client.Player.QuestGUI.RemoveQuest(6049);
                    client.Player.QuestGUI.RemoveQuest(6366);
                    client.Player.QuestGUI.RemoveQuest(6014);
                    client.Player.QuestGUI.RemoveQuest(2375);
                    client.Player.QuestGUI.RemoveQuest(6126);
                    client.Player.Conquer = 0;
                    client.Player.Coonquer14 = 0;
                    client.Player.DailyHeavenChance = client.Player.DailyMagnoliaChance
                        = client.Player.DailyMagnoliaItemId
                        = client.Player.DailyHeavenChance = client.Player.DailySpiritBeadCount = client.Player.DailyRareChance = 0;
                    //
                    client.Player.Day = DateTime.Now.DayOfYear;
                }
            }
            catch (Exception e) { MyConsole.WriteLine(e.ToString()); }
        }
        public static void SaveClient(Client.GameClient client)
        {
            try
            {
                WindowsAPI.IniFile write = new WindowsAPI.IniFile("\\Users\\" + client.Player.UID + ".ini");

                if ((client.ClientFlag & Client.ServerFlag.LoginFull) != Client.ServerFlag.LoginFull)
                {

                    if (client.Map != null)
                        client.Map.Denquer(client);

                }
                if (HouseTable.InHouse(client.Player.Map) && client.Player.DynamicID != 0 || client.Player.DynamicID != 0)
                {
                    if (client.Socket != null && client.Socket.Alive == false)
                    {
                        client.Player.Map = 1002;
                        client.Player.X = 300;
                        client.Player.Y = 278;
                    }
                }
                if ((client.ClientFlag & Client.ServerFlag.Disconnect) == Client.ServerFlag.Disconnect)
                {
                    if (client.Player.Map == 6525 || client.Player.Map == 1987 || client.Player.Map == 1988 || client.Player.Map == 1989 || client.Player.Map == 1990 || client.Player.Map == 1991 || client.Player.Map == 1992 || client.Player.Map == 1993 || client.Player.Map == 1994 || client.Player.Map == 1995 || client.Player.Map == 1996 || client.Player.Map == 1997 || client.Player.Map == 1998 || client.Player.Map == 1999 || client.Player.Map == 2000 || client.Player.Map == 2001 || client.Player.Map == 2002 || client.Player.Map == 2003 || client.Player.Map == 2004 || client.Player.Map == 2005 || client.Player.Map == 1036 || client.InQualifier() || client.Player.DynamicID == client.Player.UID)
                    {
                        if (client.Socket != null && client.Socket.Alive == false)
                        {
                            client.Player.Map = 1002;
                            client.Player.X = 300;
                            client.Player.Y = 278;
                        }
                    }               
                    if (Program.DisconnectMap.Contains(client.Player.Map) || (Game.AISystem.UnlimitedArenaRooms.Maps.ContainsValue(client.Player.DynamicID) && client.Player.fbss == 1))
                    {
                        client.Player.Map = 1002;
                        client.Player.X = 300;
                        client.Player.Y = 278;
                    }
                    if (client.Player.Map == Game.MsgTournaments.MsgFootball.MapID)
                    {
                        if (Game.MsgTournaments.MsgSchedules.CurrentTournament is Game.MsgTournaments.MsgFootball)
                        {
                            var tourn = (Game.MsgTournaments.MsgFootball)Game.MsgTournaments.MsgSchedules.CurrentTournament;

                            if (client.Player.ContainFlag(MsgUpdate.Flags.lianhuaran04))
                            {
                                client.Player.RemoveFlag(MsgUpdate.Flags.lianhuaran04);
                                tourn.FinishRound();
                            }
                        }
                    }
                }
                if (DeathWish.Protection.SystemBanned.IsBanned(client, true))
                {
                    if (client.Player.Map != 7000)
                    {
                        client.Player.Map = 7000;
                        client.Player.X = 38;
                        client.Player.Y = 75;
                    }
                }
                else
                {
                    if (client.Player.Map == 7000)
                    {
                        client.Player.Map = 1002;
                        client.Player.X = 300;
                        client.Player.Y = 278;
                    }
                }
                if (!client.FullLoading)
                    return;
                write.Write<long>("PaidBot", "TimeBot", client.MyAI.Timer.ToBinary());
                write.Write<uint>("Character", "UID", client.Player.UID);
                write.Write<ushort>("Character", "Body", client.Player.Body);
                write.Write<ushort>("Character", "Face", client.Player.Face);
                write.WriteString("Character", "Name", client.Player.Name);
                write.WriteString("Character", "Spouse", client.Player.Spouse);
                write.Write<byte>("Character", "Class", client.Player.Class);
                write.Write<byte>("Character", "FirstClass", client.Player.FirstClass);
                write.Write<byte>("Character", "SecoundeClass", client.Player.SecoundeClass);
                write.Write<bool>("Character", "IsBannedChat", client.Player.IsBannedChat);
                write.Write<bool>("Character", "PermenantBannedChat", client.Player.PermenantBannedChat);
                write.Write<long>("Character", "BannedChatStamp", client.Player.BannedChatStamp.ToBinary());
                try
                {
                    write.Write<bool>("Character", "CPBoundPackToday", client.Player.ClaimedBCPToday);
                    write.Write<byte>("Character", "FateFruitUsed", client.Player.FateFruitUsed);
                    write.Write<byte>("Character", "CPBoundPack", client.Player.CPBoundPack);
                    write.Write<uint>("Character", "ConquerPointsLimitaion", client.Player.ConquerPointsLimitaion);
                    write.Write<byte>("Character", "ConquerPointDropLimitLayer", client.Player.ConquerPointDropLimitLayer);
                    write.Write<uint>("Character", "ExchangeNormalAvaliability", client.Player.ExchangeNormalAvaliability);
                    write.Write<uint>("Character", "ExchangehighAvaliability", client.Player.ExchangehighAvaliability);
                    write.Write<uint>("Character", "GiantGhasomKill", client.Player.GiantGhasomKill);
                    write.Write<uint>("Character", "MiningAttempts", client.Player.MiningAttempts);
                    write.Write<bool>("Character", "GiantGhasomKillClaimed", client.Player.GiantGhasomKillClaimed);
                    if (client.Player.ConquerPointDropLimitLayer > 1)
                    {
                        write.Write<long>("Character", "ConquerPointsDropStamp", client.Player.ConquerPointsDropStamp.ToBinary());
                    }             
                    write.Write<string>("Character", "MacAddress", client.PcLogin.MacAddress);
                    if (client.Player.WrongAnswers != null)
                    {
                        for (int x = 0; x < 5; x++)
                        {
                            write.Write<int>("Character", "WrongAnswers" + x.ToString(), client.Player.WrongAnswers[x]);
                        }
                    }
                }
                catch
                {
                }
                write.Write<ushort>("Character", "Avatar", client.Player.Avatar);
                write.Write<uint>("Character", "Map", client.Player.Map);
                write.Write<ushort>("Character", "X", client.Player.X);
                write.Write<ushort>("Character", "Y", client.Player.Y);            
                write.Write<uint>("Character", "Merchant", client.Player.Merchant);
                write.Write<long>("Character", "MerchantApplicationEnd", client.Player.MerchantApplicationEnd.ToBinary());
                write.Write<uint>("Character", "PMap", client.Player.PMap);
                write.Write<ushort>("Character", "PMapX", client.Player.PMapX);
                write.Write<ushort>("Character", "PMapY", client.Player.PMapY);
                write.Write<ushort>("Character", "Agility", client.Player.Agility);
                write.Write<ushort>("Character", "Strength", client.Player.Strength);
                write.Write<ushort>("Character", "Vitaliti", client.Player.Vitality);
                write.Write<ushort>("Character", "Spirit", client.Player.Spirit);
                write.Write<ushort>("Character", "Atributes", client.Player.Atributes);       
                write.Write<byte>("Character", "Reborn", client.Player.Reborn);
                write.Write<ushort>("Character", "Level", client.Player.Level);
                write.Write<ushort>("Character", "Haire", client.Player.Hair);
                write.Write<ulong>("Character", "Experience", client.Player.Experience);
                write.Write<int>("Character", "MinHitPoints", client.Player.HitPoints);
                write.Write<ushort>("Character", "MinMana", client.Player.Mana);
                write.Write<uint>("Character", "ConquerPoints", client.Player.ConquerPoints);
                write.Write<int>("Character", "BoundConquerPoints", client.Player.BoundConquerPoints);
                write.Write<long>("Character", "Money", client.Player.Money);
                write.Write<uint>("Character", "VirtutePoints", client.Player.VirtutePoints);             
                write.Write<ushort>("Character", "PkPoints", client.Player.PKPoints);
                write.Write<uint>("Character", "QuizPoints", client.Player.QuizPoints);
                write.Write<uint>("Character", "Autohp", client.Player.Autohp);
                write.Write<ushort>("Character", "Enilghten", client.Player.Enilghten);
                write.Write<ushort>("Character", "EnlightenReceive", client.Player.EnlightenReceive);
                write.Write<ulong>("Character", "DailySignUpDays", client.Player.DailySignUpDays);
                write.Write<ushort>("Character", "LateSignIn", client.Player.LateSignIn);
                write.Write<byte>("Character", "DailyMonth", client.Player.DailyMonth);
                write.Write<byte>("Character", "DailySignUpRewards", client.Player.DailySignUpRewards);
                write.Write<byte>("Character", "VipLevel", client.Player.VipLevel);
                client.Player.Achievement.Save(client.Achievement);
                write.WriteString("Character", "Achivement", client.Achievement.ToString());
                write.Write<long>("Character", "WHMoney", client.Player.WHMoney);
                write.Write<long>("Character", "CpsBank", client.Player.CpsBank);
                write.Write<uint>("Character", "BlessTime", client.Player.BlessTime);      
                write.Write<uint>("Character", "SpouseUID", client.Player.SpouseUID);
                write.Write<int>("Character", "HeavenBlessing", client.Player.HeavenBlessing);
                write.Write<uint>("Character", "LostTimeBlessing", client.Player.HeavenBlessTime.Value);          
                write.Write<uint>("Character", "HuntingBlessing", client.Player.HuntingBlessing);
                write.Write<uint>("Character", "OnlineTrainingPoints", client.Player.OnlineTrainingPoints);
                write.Write<long>("Character", "JoinOnflineTG", client.Player.JoinOnflineTG.Ticks);
                write.Write<int>("Character", "Day", client.Player.Day);
                write.Write<byte>("Character", "BDExp", client.Player.BDExp);       
                write.Write<uint>("Character", "SwordMaster", client.Player.SwordMaster);
                write.Write<uint>("Character", "TeratoDragon", client.Player.TeratoDragon);
                write.Write<uint>("Character", "PurbleBanshee", client.Player.PurbleBanshee);
                write.Write<uint>("Character", "DExpTime", client.Player.DExpTime);
                write.Write<uint>("Character", "RateExp", client.Player.RateExp);
                write.Write<byte>("Character", "ExpBallUsed", client.Player.ExpBallUsed);
                write.Write<byte>("Character", "ExpFruitUsed", client.Player.ExpFruit);        
                write.WriteString("Character", "SubProfInfo", client.Player.SubClass.ToString());
                write.WriteString("Character", "Dragon", client.Player.MyChi.Dragon.ToString());
                write.WriteString("Character", "Pheonix", client.Player.MyChi.Phoenix.ToString());
                write.WriteString("Character", "Turtle", client.Player.MyChi.Turtle.ToString());
                write.WriteString("Character", "Tiger", client.Player.MyChi.Tiger.ToString());
                write.Write<int>("Character", "ChiPoints", client.Player.MyChi.ChiPoints);
                if (client.Player.MyChi.DragonTime != 0)
                {
                    write.Write<long>("Character", "DragonTime", client.Player.MyChi.DragonTime);
                    write.WriteString("Character", "RetreatedDragon", ChiTable.PowersToString(client.Player.MyChi.DragonPower));
                }
                if (client.Player.MyChi.PhoenixTime != 0)
                {
                    write.Write<long>("Character", "PhoenixTime", client.Player.MyChi.PhoenixTime);
                    write.WriteString("Character", "RetreatedPhoenix", ChiTable.PowersToString(client.Player.MyChi.PhoenixPower));
                }
                if (client.Player.MyChi.TurtleTime != 0)
                {
                    write.Write<long>("Character", "TurtleTime", client.Player.MyChi.TurtleTime);
                    write.WriteString("Character", "RetreatedTurtle", ChiTable.PowersToString(client.Player.MyChi.TurtlePower));
                }
                if (client.Player.MyChi.TigerTime != 0)
                {
                    write.Write<long>("Character", "TigerTime", client.Player.MyChi.TigerTime);
                    write.WriteString("Character", "RetreatedTiger", ChiTable.PowersToString(client.Player.MyChi.TigerPower));
                }
                write.WriteString("Character", "Flowers", client.Player.Flowers.ToString());
                write.Write<ulong>("Character", "DonationNobility", client.Player.Nobility.Donation);
                write.Write<ulong>("Character", "DonationToBack", client.Player.Nobility.DonationToBack);
                write.Write<long>("Character", "PaidPeriod", client.Player.Nobility.PaidPeriod.ToBinary());
                write.Write<byte>("Character", "PaidRank", (byte)client.Player.Nobility.PaidRank);             
                write.Write<uint>("Character", "GuildID", client.Player.GuildID);
                write.Write<ushort>("Character", "GuildRank", (ushort)client.Player.GuildRank);
                if (client.Player.MyGuildMember != null)
                {
                    client.Player.MyGuildMember.LastLogin = DateTime.Now.Ticks;
                    write.Write<uint>("Character", "CpsDonate", client.Player.MyGuildMember.CpsDonate);
                    write.Write<long>("Character", "MoneyDonate", client.Player.MyGuildMember.MoneyDonate);
                    write.Write<uint>("Character", "PkDonation", client.Player.MyGuildMember.PkDonation);
                    write.Write<long>("Character", "LastLogin", client.Player.MyGuildMember.LastLogin);
          
                    write.Write<uint>("Character", "CTF_Exploits", client.Player.MyGuildMember.CTF_Exploits);
                    write.Write<uint>("Character", "CTF_RCPS", client.Player.MyGuildMember.RewardConquerPoints);
                    write.Write<uint>("Character", "CTF_RM", client.Player.MyGuildMember.RewardMoney);
                    write.Write<byte>("Character", "CTF_R", client.Player.MyGuildMember.CTF_Claimed);
                }          
                if (client.Player.MyClan != null)
                {
                    write.Write<uint>("Character", "ClanID", client.Player.MyClan.ID);
                    write.Write<ushort>("Character", "ClanRank", client.Player.ClanRank);
                    if (client.Player.MyClanMember != null)
                        write.Write<uint>("Character", "ClanDonation", client.Player.MyClanMember.Donation);
                }
                if (client.Player.InUnion)
                {
                    write.Write<uint>("Character", "UnionUID", client.Player.MyUnion.UID);
                    write.Write<uint>("Character", "UnionRank", (uint)client.Player.UnionMemeber.Rank);

                    write.Write<uint>("Character", "Treasury", client.Player.UnionMemeber.MyTreasury);
             
                }
                else
                {
                    write.Write<uint>("Character", "UnionUID", 0);
                    write.Write<uint>("Character", "UnionRank", 0);
                    write.Write<uint>("Character", "UnionExploits", 0);
                    write.Write<uint>("Character", "UnionGoldBrick", 0);
                }
                write.Write<uint>("Character", "KingDomExploits", client.Player.KingDomExploits);
                write.Write<byte>("Character", "FRL", client.Player.FirstRebornLevel);
                write.Write<byte>("Character", "SRL", client.Player.SecoundeRebornLevel);
                write.Write<bool>("Character", "Reincanation", client.Player.Reincarnation);
                write.Write<byte>("Character", "LotteryEntries", client.Player.LotteryEntries);
                write.Write<bool>("Character", "DbTry", client.Player.DbTry);
                write.Write<bool>("Character", "DBCollect", client.Player.DBCollect);
                write.Write<bool>("Character", "BagCpsCollet", client.Player.BagCpsCollet);
                write.Write<bool>("Character", "chicollect", client.Player.chicollect);
                write.Write<bool>("Character", "Exbballcollect", client.Player.Exbballcollect);
                write.Write<bool>("Character", "MSConquer", client.Player.MSConquer);
                write.Write<bool>("Character", "MRConquer", client.Player.MRConquer);
                write.WriteString("Character", "DemonEx", client.DemonExterminator.ToString());
                write.WriteString("Character", "PkName", client.Player.MyKillerName);
                write.Write<uint>("Character", "PkUID", client.Player.MyKillerUID);
                write.Write<int>("Character", "Cursed", client.Player.CursedTimer);
                write.WriteString("Character", "HeroRewards", client.HeroRewards.ToString());
                write.WriteString("Character", "Activeness", client.Activeness.ToString());
                write.Write<uint>("Character", "AparenceType", client.Player.AparenceType);
       
                write.Write<uint>("Character", "HitShoot", client.Player.HitShoot);
                write.Write<uint>("Character", "MisShoot", client.Player.MisShoot);
                write.Write<uint>("Character", "ArenaDeads", client.Player.ArenaDeads);
                write.Write<uint>("Character", "ArenaKills", client.Player.ArenaKills);
          
                write.Write<uint>("Character", "HitSBShoot", client.Player.HitSBShoot);
                write.Write<uint>("Character", "MisSBShoot", client.Player.MisSBShoot);
                write.Write<uint>("Character", "ArenaSBDeads", client.Player.ArenaSBDeads);
                write.Write<uint>("Character", "ArenaSBKills", client.Player.ArenaSBKills);

                write.Write<uint>("Character", "EpicQuestChance", client.Player.EpicQuestChance);
                write.Write<uint>("Character", "TKills", client.Player.TournamentKills);
                write.Write<uint>("Character", "OnlineMinutes", client.Player.OnlineMinutes);
                write.Write<uint>("Character", "AutoHuntMinutes", client.Player.AutoHuntMinutes);
                write.Write<uint>("Character", "DonatePoints", client.Player.DonatePoints);
                write.Write<uint>("Character", "Firstcredit", client.Player.Firstcredit);

                write.Write<uint>("Character", "RechargePoints", client.Player.RechargePoints);
                write.Write<uint>("Character", "HistoryChampionPoints", client.Player.HistoryChampionPoints);
                write.Write<uint>("Character", "TodayChampionPoints", client.Player.TodayChampionPoints);
            
                write.Write<uint>("Character", "ChampionPoints", client.Player.ChampionPoints);
                write.Write<uint>("Character", "DailySpiritBeadItem", client.Player.DailySpiritBeadItem);
                write.WriteString("Character", "SpecialTitles", GetSpecialTitles(client));
                write.WriteString("Character", "SecurityPass", GetSecurityPassword(client));
                write.Write<byte>("Character", "TCT", (byte)client.Player.TCCaptainTimes);
                write.Write<uint>("Character", "RacePoints", client.Player.RacePoints);
                write.Write<uint>("Character", "Conquer", client.Player.Conquer);
                write.Write<uint>("Character", "Coonquer14", client.Player.Coonquer14);
                write.Write<ushort>("Character", "NameEditCount", client.Player.NameEditCount);
                write.Write<uint>("Character", "ClaimStateGift", (uint)client.Player.MainFlag);
                write.Write<uint>("Character", "enervant", client.Player.AtiveQuestApe);
                write.Write<ushort>("Character", "InventorySashCount", client.Player.InventorySashCount);
               
                write.Write<ushort>("Character", "CountryID", client.Player.CountryID);
                write.Write<uint>("Character", "MyFootBallPoints", client.Player.MyFootBallPoints);
                write.Write<uint>("Character", "MostKill", client.Player.MostKill);
                write.Write<uint>("Character", "MostDeath", client.Player.MostDeath);
                write.Write<uint>("Character", "MostRevive", client.Player.MostRevive);
                write.Write<uint>("Character", "PIKAPoint", client.Player.PIKAPoint);
                write.Write<uint>("Character", "TimeGear", client.Player.TimeGear);
                write.Write<uint>("Character", "fbss", client.Player.fbss);
                write.Write<uint>("Character", "viptask", client.Player.viptask);
                write.Write<uint>("Character", "SSFB", client.Player.SSFB);
                write.Write<uint>("Character", "LastMan", client.Player.LastMan);//bahaa
                write.Write<uint>("Character", "PTB", client.Player.PTB);//bahaa
                write.Write<uint>("Character", "Get5Out", client.Player.Get5Out);//bahaa
                write.Write<uint>("Character", "DragonWar", client.Player.DragonWar);//bahaa
                write.Write<uint>("Character", "FreezeWar", client.Player.FreezeWar);//bahaa
                write.Write<uint>("Character", "Infection", client.Player.Infection);//bahaa
                write.Write<uint>("Character", "TheCaptain", client.Player.TheCaptain);//bahaa
                write.Write<uint>("Character", "Kungfu", client.Player.Kungfu);//bahaa
                write.Write<uint>("Character", "VampireWar", client.Player.VampireWar);//bahaa
                write.Write<uint>("Character", "WhackTheThief", client.Player.WhackTheThief);//bahaa
                write.Write<uint>("Character", "SoulPoint", client.Player.SoulPoint);
                write.Write<uint>("Character", "Bugspoints", client.Player.Bugspoints);
                write.Write<uint>("Character", "BadPoints", client.Player.BadPoints);

                write.Write<uint>("Character", "VipPointsD", client.Player.VipPointsD);
                write.Write<uint>("Character", "CropPoints", client.Player.CropPoints);
                write.Write<uint>("Character", "SharePoints", client.Player.SharePoints);
                write.Write<uint>("Character", "PIKAPoint3", client.Player.PIKAPoint3);
                write.Write<uint>("Character", "viptask", client.Player.viptask);              
                write.Write<uint>("Character", "EGWIN", client.Player.EGWIN);
                write.Write<uint>("Character", "ExpProtection", client.Player.ExpProtection);
                write.Write<uint>("Character", "PrestigePoints", client.MyPrestigePoints);
                write.Write<uint>("Character", "BanCount", client.BanCount);
                write.Write<uint>("Character", "BotJailCount", client.BanCount);

          
                write.Write<byte>("Character", "BuyKingdomDeeds", client.Player.BuyKingdomDeeds);
                write.Write<uint>("Character", "KingDomDeeds", client.Player.KingDomDeeds);
                write.Write<uint>("Character", "WarPoints", client.Player.WarPoints);
                write.Write<ulong>("Character", "RechargeProgress", (ulong)client.Player.RechargeProgress);
                write.Write<uint>("Character", "WarDropeFull", client.Player.WarDropeFull);
                write.WriteString("Character", "ExchangeShop", client.MyExchangeShop.ToString());
           
                write.Write<ushort>("Character", "ExtraAtributes", client.Player.ExtraAtributes);

                write.Write<byte>("Character", "OpenHousePack", client.Player.OpenHousePack);

         
                write.Write<byte>("Character", "ClaimTowerAmulets", client.Player.ClaimTowerAmulets);
                write.Write<byte>("Character", "TOMClaimTeamReward", client.Player.TOMClaimTeamReward);
                write.Write<byte>("Character", "MyTowerOfMysteryLayer", client.Player.MyTowerOfMysteryLayer);
                write.Write<byte>("Character", "MyTowerOfMysteryLayerElite", client.Player.MyTowerOfMysteryLayerElite);
                write.Write<byte>("Character", "TowerOfMysterychallenge", client.Player.TowerOfMysterychallenge);
                write.Write<uint>("Character", "TowerOfMysteryChallengeFlag", client.Player.TowerOfMysteryChallengeFlag);
                write.Write<byte>("Character", "TOMSelectChallengeToday", client.Player.TOMSelectChallengeToday);
                write.Write<byte>("Character", "TOMChallengeToday", client.Player.TOMChallengeToday);
                write.Write<uint>("Character", "TOMRefreshReward", client.Player.TOMRefreshReward);
                write.Write<byte>("Character", "TOM_Reward", (byte)client.Player.TOM_Reward);
                write.Write<long>("Character", "JPAStamp", client.Player.JoinPowerArenaStamp.Ticks);
           
                write.WriteString("Character", "EpicTrojan", client.Player.SaveEpicTrojan());
                write.Write<int>("Character", "GiveFlowersToPerformer", client.Player.GiveFlowersToPerformer);
                write.Write<byte>("Character", "UseChiToken", client.Player.UseChiToken);
                write.Write<byte>("Character", "StoneLand", client.Player.StoneLand);
                write.Write<long>("Character", "CanChangeWindWalkerFree", client.Player.CanChangeWindWalkerFree.Ticks);
           
                write.Write<byte>("Character", "HelpLady", client.Player.helpladytime);
                write.Write<byte>("Character", "VotePoints", client.Player.VotePoints);
                write.Write<uint>("Character", "Battlefieldpoints", client.Player.BattleFieldPoints);
                write.Write<int>("Character", "BossPoints", client.Player.BossPoints);
           
                try
                {
                    SaveClientInbox(client);
                }
                catch
                {
                    //MyConsole.WriteLine("Error While Writing Inbox.");
                }
                SaveClientItems(client);
                SaveClientSpells(client);
                SaveClientProfs(client);
                RoleQuests.Save(client);
                Role.Instance.House.Save(client);
              
                if ((client.ClientFlag & Client.ServerFlag.Disconnect) == Client.ServerFlag.Disconnect)
                {
                    Client.GameClient user;
                    Database.Server.GamePoll.TryRemove(client.Player.UID, out user);
                }
            }
            catch (Exception e) { MyConsole.WriteLine(e.ToString()); }

        }
        public static void LoadCharacter(Client.GameClient client, uint UID)
        {

            client.Player.UID = UID;
            WindowsAPI.IniFile reader = new WindowsAPI.IniFile("\\Users\\" + UID + ".ini");
            client.Player.Body = reader.ReadUInt16("Character", "Body", 1002);
            client.Player.Face = reader.ReadUInt16("Character", "Face", 0);
            client.Player.Name = reader.ReadString("Character", "Name", "None");
            client.Player.Spouse = reader.ReadString("Character", "Spouse", "None");
            client.Player.Class = reader.ReadByte("Character", "Class", 0);
            client.Player.FirstClass = reader.ReadByte("Character", "FirstClass", 0);
            client.Player.SecoundeClass = reader.ReadByte("Character", "SecoundeClass", 0);
            client.Player.MerchantApplicationEnd = DateTime.FromBinary(reader.ReadInt64("Character", "MerchantApplicationEnd", 0));
            client.Player.Merchant = reader.ReadUInt32("Character", "Merchant", 0);
            client.Player.Avatar = reader.ReadUInt16("Character", "Avatar", 0);
            client.Player.Map = reader.ReadUInt32("Character", "Map", 1002);
            client.Player.X = reader.ReadUInt16("Character", "X", 248);
            client.Player.Y = reader.ReadUInt16("Character", "Y", 238);
            if (client.EventBase != null)
            {
                if (client.Player.Map == 1616)
                {
                    client.EventBase.RemovePlayer(client, true);
                    client.Player.Map = 1002;
                    client.Player.X = 300;
                    client.Player.Y = 278;
                }
            }
            if (Program.DisconnectMap.Contains(client.Player.Map) || (Game.AISystem.UnlimitedArenaRooms.Maps.ContainsValue(client.Player.DynamicID) && client.Player.fbss == 1))
            {
                client.Player.Map = 1002;
                client.Player.X = 300;
                client.Player.Y = 278;
            }
            if (client.Player.Map == 1002 & client.Player.X >= 250 && client.Player.X < 340 & client.Player.Y >= 240 && client.Player.Y < 380)
            {
                client.Player.Map = 1002;
                client.Player.X = 300;
                client.Player.Y = 278;
            }
            for (int x = 0; x < 5; x++)
            {
                client.Player.WrongAnswers[x] = reader.ReadInt32("Character", "WrongAnswers" + x.ToString(), 0);
            }
            client.Player.PMap = reader.ReadUInt32("Character", "PMap", 1002);
            client.Player.PMapX = reader.ReadUInt16("Character", "PMapX", 300);
            client.Player.PMapY = reader.ReadUInt16("Character", "PMapY", 300);     
            client.Player.Agility = reader.ReadUInt16("Character", "Agility", 0);
            client.Player.Strength = reader.ReadUInt16("Character", "Strength", 0);
            client.Player.Spirit = reader.ReadUInt16("Character", "Spirit", 0);
            client.Player.Vitality = reader.ReadUInt16("Character", "Vitaliti", 0);
            client.Player.Atributes = reader.ReadUInt16("Character", "Atributes", 0);
            client.Player.PermenantBannedChat = reader.ReadBool("Character", "PermenantBannedChat", false);
            client.Player.IsBannedChat = reader.ReadBool("Character", "IsBannedChat", false);
            client.Player.BannedChatStamp = DateTime.FromBinary(reader.ReadInt64("Character", "BannedChatStamp", 0));
            client.Player.Reborn = reader.ReadByte("Character", "Reborn", 0);
            client.Player.Level = reader.ReadUInt16("Character", "Level", 0);
            client.Player.Hair = reader.ReadUInt16("Character", "Haire", 0);
            client.Player.Experience = (ulong)reader.ReadInt64("Character", "Experience", 0);
            client.Player.HitPoints = reader.ReadInt32("Character", "MinHitPoints", 0);
            client.Player.Mana = reader.ReadUInt16("Character", "MinMana", 0);
            client.Player.ConquerPoints = reader.ReadUInt32("Character", "ConquerPoints", 0);
            client.Player.BoundConquerPoints = reader.ReadInt32("Character", "BoundConquerPoints", 0);
            client.Player.Money = reader.ReadInt64("Character", "Money", 0);
            client.Player.VirtutePoints = reader.ReadUInt32("Character", "VirtutePoints", 0);
            client.Player.PKPoints = reader.ReadUInt16("Character", "PkPoints", 0);
            client.Player.QuizPoints = reader.ReadUInt32("Character", "QuizPoints", 0);
            client.Player.Enilghten = reader.ReadUInt16("Character", "Enilghten", 0);
            client.Player.EnlightenReceive = reader.ReadUInt16("Character", "EnlightenReceive", 0);
            client.Player.LateSignIn = reader.ReadUInt16("Character", "LateSignIn", 0);
            client.Player.DailySignUpDays = reader.ReadUInt64("Character", "DailySignUpDays", 0);
            client.Player.Autohp = reader.ReadUInt16("Character", "Autohp", 0);
            client.Player.DailyMonth = reader.ReadByte("Character", "DailyMonth", 0);
            client.Player.DailySignUpRewards = reader.ReadByte("Character", "DailySignUpRewards", 0);
            client.Player.VipLevel = reader.ReadByte("Character", "VipLevel", 0);   
            client.Achievement = new AchievementCollection();
            client.Achievement.Load(reader.ReadString("Character", "Achivement", ""));
            client.Player.GiantGhasomKill = reader.ReadUInt32("Character", "GiantGhasomKill", 0);
            client.Player.GiantGhasomKillClaimed = reader.ReadBool("Character", "GiantGhasomKillClaimed", false);
            client.Player.WHMoney = reader.ReadInt64("Character", "WHMoney", 0);
            client.Player.CpsBank = reader.ReadInt64("Character", "CpsBank", 0);
            client.Player.BlessTime = reader.ReadUInt32("Character", "BlessTime", 0);
            client.Player.SpouseUID = reader.ReadUInt32("Character", "SpouseUID", 0);
            client.Player.HeavenBlessing = reader.ReadInt32("Character", "HeavenBlessing", 0);
            client.Player.HeavenBlessTime = new Extensions.Time32(reader.ReadUInt32("Character", "LostTimeBlessing", 0));
            client.Player.HuntingBlessing = reader.ReadUInt32("Character", "HuntingBlessing", 0);
            client.Player.OnlineTrainingPoints = reader.ReadUInt32("Character", "OnlineTrainingPoints", 0);
            client.Player.JoinOnflineTG = DateTime.FromBinary(reader.ReadInt64("Character", "JoinOnflineTG", 0));
            client.Player.RateExp = reader.ReadUInt32("Character", "RateExp", 0);
            client.Player.DExpTime = reader.ReadUInt32("Character", "DExpTime", 0);   
            client.Player.helpladytime = reader.ReadByte("Character", "HelpLady", 0);
            client.Player.BattleFieldPoints = reader.ReadByte("Character", "Battlefieldpoints", 0);
            client.Player.VotePoints = reader.ReadByte("Character", "VotePoints", 0);
            client.Player.Day = reader.ReadInt32("Character", "Day", 0);
            client.Player.BDExp = reader.ReadByte("Character", "BDExp", 0);
            client.MyAI.Timer = DateTime.FromBinary(reader.ReadInt64("PaidBot", "TimeBot", 0));
            try
            {
                client.Player.MiningAttempts = reader.ReadUInt32("Character", "MiningAttempts", 0);
                client.Player.ExpFruit = reader.ReadByte("Character", "ExpFruit", 0);
                client.Player.SwordMaster = reader.ReadByte("Character", "SwordMaster", 0);
                client.Player.TeratoDragon = reader.ReadByte("Character", "TeratoDragon", 0);
                client.Player.PurbleBanshee = reader.ReadByte("Character", "PurbleBanshee", 0);
                client.Player.ExchangeNormalAvaliability = reader.ReadUInt32("Character", "ExchangeNormalAvaliability", 0);
                client.Player.ExchangehighAvaliability = reader.ReadUInt32("Character", "ExchangehighAvaliability", 0);
                client.Player.CPBoundPack = reader.ReadByte("Character", "CPBoundPack", 0);
                client.Player.ClaimedBCPToday = reader.ReadBool("Character", "CPBoundPackToday", false);
                client.Player.FateFruitUsed = reader.ReadByte("Character", "FateFruitUsed", 0);
                client.Player.AnswerdToday = reader.ReadUInt32("Character", "AnswerdToday", 0);
                client.Player.Conquer = reader.ReadUInt32("Character", "Conquer", 0);
                client.Player.Coonquer14 = reader.ReadUInt32("Character", "Coonquer14", 0);
                client.Player.ConquerPointsLimitaion = reader.ReadUInt32("Character", "ConquerPointsLimitaion", 0);
                client.Player.ConquerPointDropLimitLayer = reader.ReadByte("Character", "ConquerPointDropLimitLayer", 0);        
                if (client.Player.ConquerPointDropLimitLayer > 1)
                {
                    client.Player.ConquerPointsDropStamp = DateTime.FromBinary(reader.ReadInt64("Character", "ConquerPointsDropStamp", 0));
                }
            }
            catch (Exception ex)
            {
                MyConsole.SaveException(ex);
            }
            client.Player.ExpBallUsed = reader.ReadByte("Character", "ExpBallUsed", 0);

            client.Player.RechargeProgress = (Role.Player.RechargeType)reader.ReadUInt32("Character", "RechargeProgress", 0);        
            DataCore.LoadClient(client.Player);    
            client.Player.GuildID = reader.ReadUInt32("Character", "GuildID", 0);
            client.Player.GuildRank = (Role.Flags.GuildMemberRank)reader.ReadUInt32("Character", "GuildRank", 200);
            if (client.Player.GuildID != 0)
            {
                Role.Instance.Guild myguild;
                if (Role.Instance.Guild.GuildPoll.TryGetValue(client.Player.GuildID, out myguild))
                {
                    client.Player.MyGuild = myguild;
                    Role.Instance.Guild.Member member;
                    if (myguild.Members.TryGetValue(client.Player.UID, out member))
                    {
                        member.IsOnline = true;
                        client.Player.GuildID = (ushort)myguild.Info.GuildID;
                        client.Player.MyGuildMember = member;
                        client.Player.GuildRank = member.Rank;
                        client.Player.GuildBattlePower = myguild.ShareMemberPotency(member.Rank);       
                    }
                    else
                    {
                        client.Player.MyGuild = null;
                        client.Player.GuildID = 0;
                        client.Player.GuildRank = (Role.Flags.GuildMemberRank)0;
                    }
                }
                else
                {
                    client.Player.MyGuild = null;
                    client.Player.GuildID = 0;
                    client.Player.GuildRank = (Role.Flags.GuildMemberRank)0;
                }
            }    
            uint UnionID = reader.ReadUInt32("Character", "UnionUID", 0);
            if (UnionID != 0 && client.Player.GuildID == 0)
            {
                Role.Instance.Union union;
                if (Role.Instance.Union.UnionPoll.TryGetValue(UnionID, out union))
                {
                    Role.Instance.Union.Member Member;
                    if (union.Members.TryGetValue(client.Player.UID, out Member))
                    {
                        client.Player.MyUnion = union;
                        client.Player.UnionMemeber = Member;
                        client.Player.UnionMemeber.Owner = client;
                    }
                }
            }
            else if (client.Player.GuildID != 0 && client.Player.MyGuild != null && client.Player.MyGuild.UnionID != 0)
            {
                var union = client.Player.MyGuild.GetUnion;
                if (union != null)
                {
                    Role.Instance.Guild.Member Member;
                    if (client.Player.MyGuild.Members.TryGetValue(client.Player.UID, out Member))
                    {
                        client.Player.UnionMemeber = Member.UnionMem;
                        client.Player.UnionMemeber.Owner = client;
                        client.Player.MyUnion = union;
                    }
                }
            }
            if (client.Player.InUnion)
            {
                if (client.Player.UnionMemeber.Rank == Role.Instance.Union.Member.MilitaryRanks.Emperor)
                {
                    if (client.Player.MyUnion.EmperrorUID != client.Player.UID)
                        client.Player.UnionMemeber.Rank = Role.Instance.Union.Member.MilitaryRanks.Member;
                }
            }
            client.Player.SubClass = new Role.Instance.SubClass();
            client.Player.SubClass.Load(reader.ReadString("Character", "SubProfInfo", ""));
            client.Player.SubClass.CreateSpawn(client);       
            if (Role.Instance.Chi.ChiPool.ContainsKey(UID))
            {
                client.Player.MyChi = Role.Instance.Chi.ChiPool[UID];
                Role.Instance.Chi.ComputeStatus(client.Player.MyChi);
            }
            else
                client.Player.MyChi = new Role.Instance.Chi(UID);

            if (Role.Instance.Flowers.ClientPoll.ContainsKey(UID))
                client.Player.Flowers = Role.Instance.Flowers.ClientPoll[UID];
            else
                client.Player.Flowers = new Role.Instance.Flowers(UID, client.Player.Name);
            string flowerStr = reader.ReadString("Character", "Flowers", "");
            Database.DBActions.ReadLine Linereader = new DBActions.ReadLine(flowerStr, '/');
            client.Player.Flowers.FreeFlowers = Linereader.Read((uint)0);

            Role.Instance.Nobility nobility;
            if (Program.NobilityRanking.TryGetValue(UID, out nobility))
            {
                client.Player.Nobility = nobility;
                client.Player.NobilityRank = client.Player.Nobility.Rank;
            }
            else
            {
                client.Player.Nobility = new Role.Instance.Nobility(client);
                client.Player.Nobility.PaidRank = (Role.Instance.Nobility.NobilityRank)reader.ReadByte("Character", "PaidRank", 0);
                client.Player.Nobility.PaidPeriod = DateTime.FromBinary(reader.ReadInt64("Character", "PaidPeriod", 0));
                client.Player.Nobility.DonationToBack = reader.ReadUInt64("Character", "DonationToBack", 0);
                client.Player.Nobility.Donation = reader.ReadUInt64("Character", "DonationNobility", 0);
                client.Player.NobilityRank = client.Player.Nobility.Rank;
            }

            Role.Instance.JiangHu Jiang;
            if (Role.Instance.JiangHu.Poll.TryGetValue(client.Player.UID, out Jiang))
            {
                client.Player.MyJiangHu = Jiang;
                client.Player.MyJiangHu.Level = (byte)client.Player.Level;
                client.Player.MyJiangHu.CountDownMode = DateTime.Now;
            }
        
            Role.Instance.Associate.MyAsociats Associate;
            if (Role.Instance.Associate.Associates.TryGetValue(client.Player.UID, out Associate))
            {
                client.Player.Associate = Associate;
                client.Player.Associate.MyClient = client;
                client.Player.Associate.Online = true;
                if (client.Player.Associate.Associat.ContainsKey(Role.Instance.Associate.Mentor))
                {
                    foreach (var member in client.Player.Associate.Associat[Role.Instance.Associate.Mentor].Values)
                    {
                        if (member.UID != 0)
                        {
                            Role.Instance.Associate.MyAsociats mentor;
                            if (Role.Instance.Associate.Associates.TryGetValue(member.UID, out mentor))
                            {
                                client.Player.MyMentor = mentor;
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                client.Player.Associate = new Role.Instance.Associate.MyAsociats(client.Player.UID);
                client.Player.Associate.MyClient = client;
                client.Player.Associate.Online = true;
            }
 
            client.Player.ClanUID = reader.ReadUInt32("Character", "ClanID", 0);
            if (client.Player.ClanUID != 0)
            {
                Role.Instance.Clan myclan;
                if (Role.Instance.Clan.Clans.TryGetValue(client.Player.ClanUID, out myclan))
                {
                    client.Player.MyClan = myclan;
                    Role.Instance.Clan.Member member;
                    if (myclan.Members.TryGetValue(client.Player.UID, out member))
                    {
                        member.Online = true;
                        client.Player.ClanName = myclan.Name;
                        client.Player.MyClanMember = member;
                        client.Player.ClanRank = (ushort)member.Rank;
                    }
                    else
                    {
                        client.Player.MyClan = null;
                        client.Player.ClanUID = 0;
                        client.Player.ClanRank = 0;
                    }
                }
                else
                    client.Player.ClanUID = 0;
            }
    
            client.Player.FirstRebornLevel = reader.ReadByte("Character", "FRL", 0);
            client.Player.SecoundeRebornLevel = reader.ReadByte("Character", "SRL", 0);
            client.Player.Reincarnation = reader.ReadBool("Character", "Reincanation", false);
            client.Player.LotteryEntries = reader.ReadByte("Character", "LotteryEntries", 0);
            client.Player.DbTry = reader.ReadBool("Character", "DbTry", false);
            client.Player.DBCollect = reader.ReadBool("Character", "DBCollect", true);
            client.Player.BagCpsCollet = reader.ReadBool("Character", "BagCpsCollet", true);
            client.Player.chicollect = reader.ReadBool("Character", "chicollect", true);
            client.Player.Exbballcollect = reader.ReadBool("Character", "Exbballcollect", true);

            client.Player.MSConquer = reader.ReadBool("Character", "MSConquer", false);
            client.Player.MRConquer = reader.ReadBool("Character", "MRConquer", false);
            client.DemonExterminator.ReadLine(reader.ReadString("Character", "DemonEx", "0/0/"));

            client.Player.MyKillerUID = reader.ReadUInt32("Character", "PkName", 0);
            client.Player.MyKillerName = reader.ReadString("Character", "PkName", "None");
            client.Player.CursedTimer = reader.ReadInt32("Character", "Cursed", 0);
            client.HeroRewards.Load(reader.ReadBigString("Character", "HeroRewards", ""));
            client.Activeness.Load(reader.ReadBigString("Character", "Activeness", ""));
            client.Player.AtiveQuestApe = reader.ReadUInt32("Character", "enervant", 0);

            client.Player.AparenceType = reader.ReadUInt32("Character", "AparenceType", 0);

            client.Player.HitShoot = reader.ReadUInt32("Character", "HitShoot", 0);
            client.Player.MisShoot = reader.ReadUInt32("Character", "MisShoot", 0);
            client.Player.ArenaKills = reader.ReadUInt32("Character", "ArenaKills", 0);
            client.Player.ArenaDeads = reader.ReadUInt32("Character", "ArenaDeads", 0);

            client.Player.HitSBShoot = reader.ReadUInt32("Character", "HitSBShoot", 0);
            client.Player.MisSBShoot = reader.ReadUInt32("Character", "MisSBShoot", 0);
            client.Player.ArenaSBKills = reader.ReadUInt32("Character", "ArenaSBKills", 0);
            client.Player.ArenaSBDeads = reader.ReadUInt32("Character", "ArenaSBDeads", 0);

            client.Player.TournamentKills = reader.ReadUInt32("Character", "TKills", 0);
            client.Player.OnlineMinutes = reader.ReadUInt32("Character", "OnlineMinutes", 0);
            client.Player.AutoHuntMinutes = reader.ReadUInt32("Character", "AutoHuntMinutes", 0);
            client.Player.DonatePoints = reader.ReadUInt32("Character", "DonatePoints", 0);
            client.Player.Firstcredit = reader.ReadUInt32("Character", "Firstcredit", 0);

            client.Player.RechargePoints = reader.ReadUInt32("Character", "RechargePoints", 0);
            client.Player.EpicQuestChance = reader.ReadUInt32("Character", "EpicQuestChance", 0);
            client.Player.HistoryChampionPoints = reader.ReadUInt32("Character", "HistoryChampionPoints", 0);

            client.Player.AddChampionPoints(reader.ReadUInt32("Character", "ChampionPoints", 0), false);
            client.Player.TodayChampionPoints = reader.ReadUInt32("Character", "TodayChampionPoints", 0);
            client.Player.DailySpiritBeadItem = reader.ReadUInt32("Character", "DailySpiritBeadItem", 0);
            LoadSpecialTitles(client, reader.ReadBigX2String("Character", "SpecialTitles", "0,0,"));
            LoadSecurityPassword(reader.ReadString("Character", "SecurityPass", "0,0,0"), client);
            client.Player.TCCaptainTimes = reader.ReadByte("Character", "TCT", 0);
            client.Player.RacePoints = reader.ReadUInt32("Character", "RacePoints", 0);
            client.Player.NameEditCount = reader.ReadUInt16("Character", "NameEditCount", 0);
            client.Player.MainFlag = (Role.Player.MainFlagType)reader.ReadUInt32("Character", "ClaimStateGift", 0);
            client.Player.CountryID = reader.ReadUInt16("Character", "CountryID", 0);

            client.Player.InventorySashCount = reader.ReadUInt16("Character", "InventorySashCount", 0);
            client.Player.MyFootBallPoints = reader.ReadUInt32("Character", "MyFootBallPoints", 0);
            client.Player.PIKAPoint = reader.ReadUInt32("Character", "PIKAPoint", 0);
            client.Player.TimeGear = reader.ReadUInt32("Character", "TimeGear", 0);
            client.Player.fbss = reader.ReadUInt32("Character", "fbss", 0);
            client.Player.MostKill = reader.ReadUInt32("Character", "MostKill", 0);
            client.Player.MostDeath = reader.ReadUInt32("Character", "MostDeath", 0);
            client.Player.MostRevive = reader.ReadUInt32("Character", "MostRevive", 0);
            client.Player.SoulPoint = reader.ReadUInt32("Character", "SoulPoint", 0);
            client.Player.Bugspoints = reader.ReadUInt32("Character", "Bugspoints", 0);
            client.Player.BadPoints = reader.ReadUInt32("Character", "BadPoints", 0);
            client.Player.VipPointsD = reader.ReadUInt32("Character", "VipPointsD", 0);
            client.Player.CropPoints = reader.ReadUInt32("Character", "CropPoints", 0);
            client.Player.SharePoints = reader.ReadUInt32("Character", "SharePoints", 0);
            client.Player.PIKAPoint3 = reader.ReadUInt32("Character", "PIKAPoint3", 0);
            client.Player.SSFB = reader.ReadUInt32("Character", "SSFB", 0);
            client.Player.LastMan = reader.ReadUInt32("Character", "LastMan", 0);//bahaa
            client.Player.PTB = reader.ReadUInt32("Character", "PTB", 0);//bahaa
            client.Player.DragonWar = reader.ReadUInt32("Character", "DragonWar", 0);//bahaa
            client.Player.Get5Out = reader.ReadUInt32("Character", "Get5Out", 0);//bahaa
            client.Player.FreezeWar = reader.ReadUInt32("Character", "FreezeWar", 0);//bahaa
            client.Player.Infection = reader.ReadUInt32("Character", "Infection", 0);//bahaa
            client.Player.TheCaptain = reader.ReadUInt32("Character", "TheCaptain", 0);//bahaa
            client.Player.Kungfu = reader.ReadUInt32("Character", "Kungfu", 0);//bahaa
            client.Player.VampireWar = reader.ReadUInt32("Character", "VampireWar", 0);//bahaa
            client.Player.WhackTheThief = reader.ReadUInt32("Character", "WhackTheThief", 0);//bahaa
            client.Player.EGWIN = reader.ReadUInt32("Character", "EGWIN", 0);
            client.Player.ExpProtection = reader.ReadUInt32("Character", "ExpProtection", 0);
            client.BanCount = reader.ReadByte("Character", "BanCount", 0);
            client.BotJailCount = reader.ReadByte("Character", "BotJailCount", 0);
            client.Player.KingDomExploits = reader.ReadUInt32("Character", "KingDomExploits", 0);
            client.Player.BossPoints = reader.ReadInt32("Character", "BossPoints", 0);

            client.Player.BuyKingdomDeeds = reader.ReadByte("Character", "BuyKingdomDeeds", 0);
            client.Player.KingDomDeeds = reader.ReadUInt32("Character", "KingDomDeeds", 0);
            client.Player.WarPoints = reader.ReadUInt32("Character", "WarPoints", 0);
            client.Player.WarDropeFull = reader.ReadUInt32("Character", "WarDropeFull", 0);
            client.MyExchangeShop.Load(reader.ReadString("Character", "ExchangeShop", "0"));
            client.Player.ExtraAtributes = reader.ReadUInt16("Character", "ExtraAtributes", 0);

            client.Player.OpenHousePack = reader.ReadByte("Character", "OpenHousePack", 0);
            client.Player.MyTowerOfMysteryLayer = reader.ReadByte("Character", "MyTowerOfMysteryLayer", 0);

            client.Player.ClaimTowerAmulets = reader.ReadByte("Character", "ClaimTowerAmulets", 0);
            client.Player.TOMClaimTeamReward = reader.ReadByte("Character", "TOMClaimTeamReward", 0);
            client.Player.MyTowerOfMysteryLayerElite = reader.ReadByte("Character", "MyTowerOfMysteryLayerElite", 0);
            client.Player.TowerOfMysterychallenge = reader.ReadByte("Character", "TowerOfMysterychallenge", 0);
            client.Player.TowerOfMysteryChallengeFlag = reader.ReadUInt32("Character", "TowerOfMysteryChallengeFlag", 0);
            client.Player.TOMSelectChallengeToday = reader.ReadByte("Character", "TOMSelectChallengeToday", 0);
            client.Player.TOMChallengeToday = reader.ReadByte("Character", "TOMChallengeToday", 0);
            client.Player.TOMRefreshReward = reader.ReadUInt32("Character", "TOMRefreshReward", 0);
            client.Player.JoinPowerArenaStamp = DateTime.FromBinary(reader.ReadInt64("Character", "JPAStamp", 0));
            client.Player.TOM_Reward = (Game.MsgTournaments.MsgTowerOfMystery.RewardTypes)reader.ReadByte("Character", "TOM_Reward", 0);

            client.Player.LoadEpicTrojan(reader.ReadString("Character", "EpicTrojan", "0/0/0/0/1/1/0/0/0/0/0/0/0/0/1/0/0/0/1"));

            client.Player.GiveFlowersToPerformer = reader.ReadInt32("Character", "GiveFlowersToPerformer", 0);

            client.Player.UseChiToken = reader.ReadByte("Character", "UseChiToken", 0);
            client.Player.StoneLand = reader.ReadByte("Character", "StoneLand", 0);
            client.Player.CanChangeWindWalkerFree = DateTime.FromBinary(reader.ReadInt64("Character", "CanChangeWindWalkerFree", DateTime.Now.Ticks));
            try
            {
                LoadClientInbox(client);
            }
            catch
            {
  
                MyConsole.WriteLine("Error While Reading Inbox.");
            }
            LoadClientItems(client);
            LoadClientSpells(client);
            LoadClientProfs(client);
            RoleQuests.Load(client);
            Role.Instance.House.Load(client);

            ResetingEveryDay(client);

   
            Role.Instance.Confiscator Container;
            if (Server.QueueContainer.PollContainers.TryGetValue(client.Player.UID, out Container))
                client.Confiscator = Container;
            try
            {
                client.Player.Associate.OnLoading(client);
            }
            catch (Exception e) { MyConsole.WriteLine(e.ToString()); }

            if (!Role.Instance.InnerPower.InnerPowerPolle.TryGetValue(client.Player.UID, out client.Player.InnerPower))
                client.Player.InnerPower = new Role.Instance.InnerPower(client.Player.Name, client.Player.UID);

            client.Player.InnerPower.UpdateStatus();

            if (Game.MsgTournaments.MsgArena.ArenaPoll.TryGetValue(client.Player.UID, out client.ArenaStatistic))
            {
                client.ArenaStatistic.ApplayInfo(client.Player);
            }
            else
            {
                client.ArenaStatistic = new Game.MsgTournaments.MsgArena.User();
                client.ArenaStatistic.ApplayInfo(client.Player);
                client.ArenaStatistic.Info.ArenaPoints = 4000;
                Game.MsgTournaments.MsgArena.ArenaPoll.TryAdd(client.Player.UID, client.ArenaStatistic);
            }

            if (Game.MsgTournaments.MsgTeamArena.ArenaPoll.TryGetValue(client.Player.UID, out client.TeamArenaStatistic))
            {
                client.TeamArenaStatistic.ApplayInfo(client.Player);
            }
            else
            {
                client.TeamArenaStatistic = new Game.MsgTournaments.MsgTeamArena.User();
                client.TeamArenaStatistic.ApplayInfo(client.Player);
                client.TeamArenaStatistic.Info.ArenaPoints = 4000;
                Game.MsgTournaments.MsgTeamArena.ArenaPoll.TryAdd(client.Player.UID, client.TeamArenaStatistic);
            }
           
            client.FullLoading = true;
        }
        public static ushort CalculateEnlighten(Role.Player player)
        {

            if (player.Level < 90)
                return 0;
            ushort val = 100;
            if (player.NobilityRank == Role.Instance.Nobility.NobilityRank.Knight || player.NobilityRank == Role.Instance.Nobility.NobilityRank.Baron)
                val += 100;
            if (player.NobilityRank == Role.Instance.Nobility.NobilityRank.Earl || player.NobilityRank == Role.Instance.Nobility.NobilityRank.Duke)
                val += 200;
            if (player.NobilityRank == Role.Instance.Nobility.NobilityRank.Prince)
                val += 300;
            if (player.NobilityRank == Role.Instance.Nobility.NobilityRank.King)
                val += 400;
            if (player.VipLevel <= 3)
                val += 100;
            if (player.VipLevel > 3 && player.VipLevel <= 5)
                val += 200;
            if (player.VipLevel > 5)
                val += 300;

            return val;
        }
        public static uint CalculateDropLimitation(Role.Player player)
        {

            if (player.Level < 80)
                return 1000;
            uint Value = 100;
            Value *= (uint)(player.Level + player.Reborn);
            if (player.Class >= 40 && player.Class <= 45)
                Value /= 2;
            if (player.Class >= 160 && player.Class <= 165)
                Value /= 2;
            Value *= player.ConquerPointDropLimitLayer;
            return Math.Max(0, Value);
        }
       
        public static string GetSecurityPassword(Client.GameClient user)
        {

            Database.DBActions.WriteLine writer = new DBActions.WriteLine(',');
            writer.Add(user.Player.SecurityPassword);
            writer.Add(user.Player.OnReset);
            writer.Add(user.Player.ResetSecurityPassowrd.Ticks);
            return writer.Close();

        }
        public static void LoadSecurityPassword(string line,Client.GameClient user)
        {
  
            Database.DBActions.ReadLine reader = new DBActions.ReadLine(line, ',');
            user.Player.SecurityPassword = reader.Read((uint)0);
            user.Player.OnReset = reader.Read((uint)0);
            if (user.Player.OnReset == 1)
            {

                user.Player.ResetSecurityPassowrd = DateTime.FromBinary(reader.Read((long)0));
                if (DateTime.Now > user.Player.ResetSecurityPassowrd)
                {
                    user.Player.OnReset = 0;
                    user.Player.SecurityPassword = 0;
                }
            }
           
        }
        public static string GetSpecialTitles(Client.GameClient user)
        {
            Database.DBActions.WriteLine writer = new DBActions.WriteLine(',');
            writer.Add((uint)user.Player.SpecialTitles.Count);
            foreach (var title in user.Player.SpecialTitles)
            {
                writer.Add((uint)title);
                if (user.Player.SpecialTitleID / 10000 == (uint)title || user.Player.SpecialWingID / 10000 == (uint)title)
                    writer.Add((byte)1);
                else
                    writer.Add((byte)0);
            }
            return writer.Close();
        }
        public static void LoadSpecialTitles(Client.GameClient user, string line)
        {
            Database.DBActions.ReadLine reader = new DBActions.ReadLine(line, ',');
            uint count = reader.Read((uint)0);
            for (int x = 0; x < count; x++)
            {
                uint Title = reader.Read((uint)0);
                uint Active = reader.Read((uint)0);
                if (!user.Player.SpecialTitles.Contains((Game.MsgServer.MsgTitleStorage.TitleType)Title))
                {
                    user.Player.SpecialTitles.Add((Game.MsgServer.MsgTitleStorage.TitleType)Title);
                    if (Active == 1)
                    {
                        Database.TitleStorage dbtitle;
                        if (Database.TitleStorage.Titles.TryGetValue(Title, out dbtitle))
                        {
                            if (dbtitle.ID <= 4001)
                            {
                                user.Player.SpecialTitleScore = dbtitle.Score;
                                user.Player.SpecialTitleID = (uint)(dbtitle.ID * 10000 + dbtitle.SubID);
                            }
                            else
                                user.Player.SpecialWingID = (uint)(dbtitle.ID * 10000 + dbtitle.SubID);
                        }
                    }
                }

            }
        }

      
        public unsafe static void LoadDBPackets()
        {
            Program.LoadPackets.Clear();
            var arraybuffer = File.ReadAllBytes(Program.ServerConfig.DbLocation + "\\array0.bin");
            int count = BitConverter.ToInt32(arraybuffer, 0);
 
            int dd = 0;
            int offset = 4;
            for (int x = 0; x < count; x++)
            {
                try
                {
                   
                        ushort size = BitConverter.ToUInt16(arraybuffer, offset);
                        byte[] packet = new byte[size];
                        Buffer.BlockCopy(arraybuffer, offset, packet, 0, size);
                        offset += size;
                        dd++;
                 //   if (dd >= 2)
                    {
                        Program.LoadPackets.Add(packet);
                    }
                }
                catch
                {
                //    break;
                }
            }
        }
        public unsafe static void LoadClientItems(Client.GameClient client)
        {
   
            WindowsAPI.BinaryFile binary = new WindowsAPI.BinaryFile();
            if (binary.Open(Program.ServerConfig.DbLocation + "\\PlayersItems\\" + client.Player.UID + ".bin", FileMode.Open))
            {
                ClientItems.DBItem Item;
                int ItemCount;
                binary.Read(&ItemCount, sizeof(int));
                Dictionary<uint, MsgGameItem> InContainer = new Dictionary<uint, MsgGameItem>();
                for (int x = 0; x < ItemCount; x++)
                {
                    binary.Read(&Item, sizeof(ClientItems.DBItem));
                    if (Item.ITEM_ID == 750000)//demonExterminator jar
                        client.DemonExterminator.ItemUID = Item.UID;
                    Game.MsgServer.MsgGameItem ClienItem = Item.GetDataItem();
                    if (Item.DepositeCount != 0)
                    {
                        uint DepositeCount = Item.DepositeCount;
                        for (int i = 0; i < DepositeCount; i++)
                        {
                            binary.Read(&Item, sizeof(ClientItems.DBItem));
                            if (Item.ITEM_ID == 750000)
                                client.DemonExterminator.ItemUID = Item.ITEM_ID;

                            Game.MsgServer.MsgGameItem DepositeItem = Item.GetDataItem();
                            if (client.Player.GuildID == 0)
                                DepositeItem.Inscribed = 0;
                            ClienItem.Deposite.TryAdd(DepositeItem.UID, DepositeItem);
                            if (!InContainer.ContainsKey(DepositeItem.UID))
                                InContainer.Add(DepositeItem.UID, DepositeItem);
                        }
                    }
                    if (Item.WH_ID != 0)
                    {
                        if (Item.WH_ID == 100)
                        {
                            client.MyWardrobe.AddItem(ClienItem);
                            if (Item.Position > 0 && Item.Position <= (ushort)Role.Flags.ConquerItem.AleternanteGarment)
                            {
                                client.Equipment.ClientItems.TryAdd(Item.UID, ClienItem);
                            }
                        }
                        else
                        {
                            if (!client.Warehouse.ClientItems.ContainsKey(Item.WH_ID))
                                client.Warehouse.ClientItems.TryAdd(Item.WH_ID, new System.Collections.Concurrent.ConcurrentDictionary<uint, Game.MsgServer.MsgGameItem>());
                            if (client.Player.GuildID == 0)
                                ClienItem.Inscribed = 0;
                            client.Warehouse.ClientItems[Item.WH_ID].TryAdd(Item.UID, ClienItem);
                        }
                    }
                    else
                    {
                        if (Item.Position > 0 && Item.Position <= (ushort)Role.Flags.ConquerItem.AleternanteGarment)
                        {
                            if (client.Player.GuildID == 0)
                                ClienItem.Inscribed = 0;
                            client.Equipment.ClientItems.TryAdd(Item.UID, ClienItem);
                        }
                        else if (Item.Position == 0)
                        {
                            if (client.Player.GuildID == 0)
                                ClienItem.Inscribed = 0;
                            client.Inventory.AddDBItem(ClienItem);
                        }
                    }
                }
         
                binary.Read(&ItemCount, sizeof(int));
                for (int x = 0; x < ItemCount; x++)
                {
                    ClientItems.Perfection info = new ClientItems.Perfection();
                    binary.Read(&info, sizeof(ClientItems.Perfection));
                    if (InContainer.ContainsKey(info.ItemUID))
                    {
                        var item = InContainer[info.ItemUID];
                        item.PerfectionLevel = info.Level;
                        item.OwnerUID = info.OwnerUID;
                        item.OwnerName = info.OwnerName;
                        item.PerfectionProgress = info.Progres;
                        item.Signature = info.SpecialText;
                        continue;
                    }
                    else if (client.Equipment.ClientItems.ContainsKey(info.ItemUID))
                    {
                        var item = client.Equipment.ClientItems[info.ItemUID];
                        item.PerfectionLevel = info.Level;
                        item.OwnerUID = info.OwnerUID;
                        item.OwnerName = info.OwnerName;
                        item.PerfectionProgress = info.Progres;
                        item.Signature = info.SpecialText;
                        continue;
                    }
                    else if (client.Inventory.ClientItems.ContainsKey(info.ItemUID))
                    {
                        var item = client.Inventory.ClientItems[info.ItemUID];
                        item.PerfectionLevel = info.Level;
                        item.OwnerUID = info.OwnerUID;
                        item.OwnerName = info.OwnerName;
                        item.PerfectionProgress = info.Progres;
                        item.Signature = info.SpecialText;
                        continue;
                    }
                    bool found = false;
                    foreach (var WH in client.Warehouse.ClientItems.Values)
                    {
                        if (found == false)
                        {
                            if (WH.ContainsKey(info.ItemUID))
                            {
                                found = true;
                                var item = WH[info.ItemUID];
                                item.PerfectionLevel = info.Level;
                                item.OwnerUID = info.OwnerUID;
                                item.OwnerName = info.OwnerName;
                                item.PerfectionProgress = info.Progres;
                                item.Signature = info.SpecialText;
                            }
                        }
                    }
                }
                binary.Close();
            }
        }

        public unsafe static void SaveClientInbox(Client.GameClient client)
        {

            WindowsAPI.BinaryFile binary = new WindowsAPI.BinaryFile();
            if (binary.Open(Program.ServerConfig.DbLocation + "\\PlayersMail\\" + client.Player.UID + ".bin", FileMode.Create))
            {
 
                ClientMail.DBPrize DBprize = new ClientMail.DBPrize();
                int CountPrizes;
                CountPrizes = client.MailBox.Count;
                binary.Write(&CountPrizes, sizeof(int));
                foreach (var prize in client.MailBox.Values)
                {
                    if (prize.ID == 0)
                        continue;
                    DBprize.GetDBPrize(prize);
                    binary.Write(&DBprize, sizeof(ClientMail.DBPrize));
                }
                binary.Close();

            }
        }
        public unsafe static void LoadClientInbox(Client.GameClient client)
        {
 
            WindowsAPI.BinaryFile binary = new WindowsAPI.BinaryFile();
            if (binary.Open(Program.ServerConfig.DbLocation + "\\PlayersMail\\" + client.Player.UID + ".bin", FileMode.Open))
            {
   
                ClientMail.DBPrize DBprize;
                int CountPrizes;
                binary.Read(&CountPrizes, sizeof(int));
                for (int x = 0; x < CountPrizes; x++)
                {
                    binary.Read(&DBprize, sizeof(ClientMail.DBPrize));
                    var ClientPrize = DBprize.GetClientPrize();
                    client.MailBox.TryAdd(ClientPrize.ID, ClientPrize);
                }
                binary.Close();
            }

        }

        public unsafe static void SaveClientItems(Client.GameClient client)
        {

            WindowsAPI.BinaryFile binary = new WindowsAPI.BinaryFile();
            if (binary.Open(Program.ServerConfig.DbLocation + "\\PlayersItems\\" + client.Player.UID + ".bin", FileMode.Create))
            {

                Dictionary<uint, MsgGameItem> InContainer = new Dictionary<uint, MsgGameItem>();
                ClientItems.DBItem DBItem = new ClientItems.DBItem();
                int ItemCount;
                ItemCount = client.GetItemsCount();
                binary.Write(&ItemCount, sizeof(int));
                foreach (var item in client.AllMyItems())
                {
                    if (item.Position == 0 && item.WH_ID == 0)
                    {
                        var pos = Database.ItemType.ItemPosition(item.ITEM_ID);
                        if (pos == (ushort)Role.Flags.ConquerItem.Garment)
                        {
                            if (item.Bless >= 2)
                            {
                                item.Bless = 1;
                            }
                        }
                    }
                    var poss = Database.ItemType.ItemPosition(item.ITEM_ID);
                    if (poss == (ushort)Role.Flags.ConquerItem.Wing)
                    {
                        if (item.Bless >= 2)
                        {
                            item.Bless = 1;
                        }
                    }
                    var poss1 = Database.ItemType.ItemPosition(item.ITEM_ID);
                    if (poss1 == (ushort)Role.Flags.ConquerItem.RidingCrop)
                    {
                        if (item.Bless >= 2)
                        {
                            item.Bless = 1;
                        }
                    }
                    var pposs = Database.ItemType.ItemPosition(item.ITEM_ID);
                    if (pposs == (ushort)Role.Flags.ConquerItem.Tower)
                    {
                        if (item.Bless >= 2)
                        {
                            item.Bless = 1;
                        }
                    }
                    var ppposs = Database.ItemType.ItemPosition(item.ITEM_ID);
                    if (ppposs == (ushort)Role.Flags.ConquerItem.Fan)
                    {
                        if (item.Bless >= 2)
                        {
                            item.Bless = 1;
                        }
                    }
                    DBItem.GetDBItem(item);
                    if (!binary.Write(&DBItem, sizeof(ClientItems.DBItem)))
                        Console.WriteLine("test");
                    if (item.Deposite.Count > 0)
                    {
                        foreach (var DepositItem in item.Deposite.Values)
                        {
                            DBItem.GetDBItem(DepositItem);
                            binary.Write(&DBItem, sizeof(ClientItems.DBItem));
                            if ((DepositItem.PerfectionLevel > 0 || DepositItem.PerfectionProgress > 0) && DepositItem.IsEquip)
                                if (!InContainer.ContainsKey(DepositItem.UID))
                                    InContainer.Add(DepositItem.UID, DepositItem);
                        }
                    }
                }

                ItemCount = client.GetPerfectionItemsCount() + InContainer.Count;
                binary.Write(&ItemCount, sizeof(int));
                foreach (var item in client.AllPerfectionItems())
                {
                  var info =  DBItem.GetPerfectionInfo(item);
                  if (!binary.Write(&info, sizeof(ClientItems.Perfection)))
                        Console.WriteLine("test");
                }
                foreach (var item in InContainer.Values)
                {
                    var info = DBItem.GetPerfectionInfo(item);
                    if (!binary.Write(&info, sizeof(ClientItems.Perfection)))
                        Console.WriteLine("test");
                }
                binary.Close();
            }

        }
        public unsafe static void LoadClientProfs(Client.GameClient client)
        {

            WindowsAPI.BinaryFile binary = new WindowsAPI.BinaryFile();
            if (binary.Open(Program.ServerConfig.DbLocation + "\\PlayersProfs\\" + client.Player.UID + ".bin", FileMode.Open))
            {

                ClientProficiency.DBProf DBProf;
                int CountProf;
                binary.Read(&CountProf, sizeof(int));
                for (int x = 0; x < CountProf; x++)
                {
                    binary.Read(&DBProf, sizeof(ClientProficiency.DBProf));
                    var ClientProf = DBProf.GetClientProf();
                    client.MyProfs.ClientProf.TryAdd(ClientProf.ID, ClientProf);
                }
                binary.Close();
            }

        }
        public unsafe static void SaveClientProfs(Client.GameClient client)
        {

            WindowsAPI.BinaryFile binary = new WindowsAPI.BinaryFile();
            if (binary.Open(Program.ServerConfig.DbLocation + "\\PlayersProfs\\" + client.Player.UID + ".bin", FileMode.Create))
            {

                ClientProficiency.DBProf DBProf = new ClientProficiency.DBProf();
                int CountProf;
                CountProf = client.MyProfs.ClientProf.Count;
                binary.Write(&CountProf, sizeof(int));
                foreach (var prof in client.MyProfs.ClientProf.Values)
                {
                    DBProf.GetDBSpell(prof);
                    binary.Write(&DBProf, sizeof(ClientProficiency.DBProf));
                }
                binary.Close();
            }
     
        }
        public unsafe static void LoadClientSpells(Client.GameClient client)
        {
            WindowsAPI.BinaryFile binary = new WindowsAPI.BinaryFile();
            if (binary.Open(Program.ServerConfig.DbLocation + "\\PlayersSpells\\" + client.Player.UID + ".bin", FileMode.Open))
            {

                ClientSpells.DBSpell DBSpell;
                int CountSpell;
                binary.Read(&CountSpell, sizeof(int));
                for (int x = 0; x < CountSpell; x++)
                {
                    binary.Read(&DBSpell, sizeof(ClientSpells.DBSpell));
                    var clientSpell = DBSpell.GetClientSpell();
                    client.MySpells.ClientSpells.TryAdd(clientSpell.ID, clientSpell);
                }

                binary.Close();
            }

        }
        public unsafe static void SaveClientSpells(Client.GameClient client)
        {

            WindowsAPI.BinaryFile binary = new WindowsAPI.BinaryFile();
            if (binary.Open(Program.ServerConfig.DbLocation + "\\PlayersSpells\\" + client.Player.UID + ".bin", FileMode.Create))
            {
                ClientSpells.DBSpell DBSpell = new ClientSpells.DBSpell();
                int SpellCount;
                SpellCount = client.MySpells.ClientSpells.Count;
                binary.Write(&SpellCount, sizeof(int));
                foreach (var spell in client.MySpells.ClientSpells.Values)
                {
                    DBSpell.GetDBSpell(spell);
                    binary.Write(&DBSpell, sizeof(ClientSpells.DBSpell));
                }

                binary.Close();
            }

        }
        public static void CreateCharacte(Client.GameClient client)
        {

            WindowsAPI.IniFile write = new WindowsAPI.IniFile("\\Users\\" + client.Player.UID + ".ini");
            write.Write<uint>("Character", "UID", client.Player.UID);
            write.Write<ushort>("Character", "Body", client.Player.Body);
            write.Write<ushort>("Character", "Face", client.Player.Face);
            write.WriteString("Character", "Name", client.Player.Name);
            write.Write<byte>("Character", "Class", client.Player.Class);
            write.Write<uint>("Character", "Map", client.Player.Map);
            write.Write<ushort>("Character", "X", client.Player.X);
            write.Write<ushort>("Character", "Y", client.Player.Y);
            client.ArenaStatistic = new Game.MsgTournaments.MsgArena.User();
            client.ArenaStatistic.ApplayInfo(client.Player);
            client.ArenaStatistic.Info.ArenaPoints = 4000;
            Game.MsgTournaments.MsgArena.ArenaPoll.TryAdd(client.Player.UID, client.ArenaStatistic);

            client.Player.Nobility = new Role.Instance.Nobility(client);

            client.TeamArenaStatistic = new Game.MsgTournaments.MsgTeamArena.User();
            client.TeamArenaStatistic.ApplayInfo(client.Player);
            client.TeamArenaStatistic.Info.ArenaPoints = 4000;

            Game.MsgTournaments.MsgTeamArena.ArenaPoll.TryAdd(client.Player.UID, client.TeamArenaStatistic);

            client.Player.Associate = new Role.Instance.Associate.MyAsociats(client.Player.UID);
            client.Player.Associate.MyClient = client;
            client.Player.Associate.Online = true;


            client.Player.Flowers = new Role.Instance.Flowers(client.Player.UID, client.Player.Name);
            client.Player.SubClass = new Role.Instance.SubClass();
            client.Player.MyChi = new Role.Instance.Chi(client.Player.UID);
            client.Achievement = new AchievementCollection();
            client.Player.JustCreated = true;
            client.FullLoading = true;

        }
       
        public static bool AllowCreate(uint UID)
        {

            return !File.Exists(Program.ServerConfig.DbLocation + "\\Users\\" + UID + ".ini");

        }
        public static void UpdateGuildMember(Role.Instance.Guild.Member Member)
        {

            WindowsAPI.IniFile write = new WindowsAPI.IniFile("\\Users\\" + Member.UID + ".ini");
            write.Write<ushort>("Character", "GuildRank", 0);

        }
        public static void UpdateGuildMember(Role.Instance.Guild.UpdateDB Member)
        {

            WindowsAPI.IniFile write = new WindowsAPI.IniFile("\\Users\\" + Member.UID + ".ini");
            write.Write<ushort>("Character", "GuildRank", 0);
            write.Write<ushort>("Character", "GuildID", 0);

        }
        public static void UpdateUnionMember(Role.Instance.Union.Member Member)
        {
            WindowsAPI.IniFile write = new WindowsAPI.IniFile("\\Users\\" + Member.UID + ".ini");
            if (Member.ReceiveKick == 0)
            {

                write.Write<uint>("Character", "UnionUID", Member.UID);
                write.Write<uint>("Character", "UnionRank", (uint)Member.Rank);
                write.Write<uint>("Character", "UnionExploits", Member.Exploits);
                write.Write<uint>("Character", "Treasury", Member.MyTreasury);

            }
            else
            {

                write.Write<uint>("Character", "UnionUID", 0);
                write.Write<uint>("Character", "UnionRank", 0);
                write.Write<uint>("Character", "UnionExploits", 0);
                write.Write<uint>("Character", "UnionGoldBrick", 0);

            }
        }
        public static void UpdateMapRace(Role.GameMap map)
        {

            WindowsAPI.IniFile write = new WindowsAPI.IniFile("\\maps\\" + map.ID + ".ini");
            write.Write<uint>("info", "race_record", map.RecordSteedRace);

        }
        public static void UpdateClanMember(Role.Instance.Clan.Member Member)
        {
  
            WindowsAPI.IniFile write = new WindowsAPI.IniFile("\\Users\\" + Member.UID + ".ini");
            write.Write<uint>("Character", "ClanID", 0);
            write.Write<ushort>("Character", "ClanRank", 0);
            write.Write<uint>("Character", "ClanDonation", 0);

        }
        public static void DestroySpouse(Client.GameClient client)
        {

            if (client.Player.SpouseUID != 0)
            {
                WindowsAPI.IniFile write = new WindowsAPI.IniFile("\\Users\\" + client.Player.SpouseUID + ".ini");
                write.Write<uint>("Character", "SpouseUID", 0);
                write.WriteString("Character", "Spouse", "None");
                if (client.Player.MyGuild.Members.ContainsKey(client.Player.SpouseUID) && client.Player.GuildRank == Role.Flags.GuildMemberRank.GuildLeader)
                {
                    client.Player.MyGuild.Members[client.Player.SpouseUID].Rank = Role.Flags.GuildMemberRank.Member;
                    write.Write<uint>("Character", "GuildRank", (uint)Role.Flags.GuildMemberRank.Member);
                }
                client.Player.SpouseUID = 0;
            }

            client.Player.Spouse = "None";
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                client.Player.SendString(stream,Game.MsgServer.MsgStringPacket.StringID.Spouse, false, new string[1] { "None" });
            }
        }
        public static string GenerateDate()
        {
 
            DateTime now = DateTime.Now;
            return now.Year.ToString() + "and" + now.Month.ToString() + "and" + now.Day.ToString() + " and " + now.Hour.ToString() + " and " + now.Minute.ToString() + " and " + now.Second.ToString();
        }
        public static void UpdateSpouse(Client.GameClient client)
        {
            if (client.Player.SpouseUID != 0)
            {
                WindowsAPI.IniFile write = new WindowsAPI.IniFile("\\Users\\" + client.Player.SpouseUID + ".ini");
                write.WriteString("Character", "Spouse", client.Player.Name);

            }
        }
        public static ExecuteLogin LoginQueue = new ExecuteLogin();

        public class ExecuteLogin : ConcurrentSmartThreadQueue<object>
        {

            public object SynRoot = new object();
            public ExecuteLogin()
                : base(5)
            {
                Start(10);
            }
            public void TryEnqueue(object obj)
            {
                    base.Enqueue(obj);
            }
            protected unsafe override void OnDequeue(object obj, int time)
            {
                try
                {

                    if (obj is string)
                    {
                        string text = obj as string;
                        if (text.StartsWith("[DemonBox]"))
                        {

                            const string UnhandledExceptionsPath = "Loggs\\";

                            var dt = DateTime.Now;
                            string date = "DemonBox" + dt.Month + "-" + dt.Day + "";

                            if (!Directory.Exists(Application.StartupPath + UnhandledExceptionsPath))
                                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath);
                            if (!Directory.Exists(Application.StartupPath + "\\" + UnhandledExceptionsPath + date))
                                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath + date);
                            if (!Directory.Exists(Application.StartupPath + "\\" + UnhandledExceptionsPath + date))
                                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath + date);

                            string fullPath = Application.StartupPath + "\\" + UnhandledExceptionsPath + date + "\\";

                            if (!File.Exists(fullPath + date + ".txt"))
                            {
                                File.WriteAllLines(fullPath + date + ".txt", new string[0]);
                            }


                            using (var SW = File.AppendText(fullPath + date + ".txt"))
                            {
                                SW.WriteLine(text);
                                SW.Close();
                            }
                        }
                        else if (text.StartsWith("[BuyShop]"))
                        {

                            const string UnhandledExceptionsPath = "Loggs\\";

                            var dt = DateTime.Now;
                            string date = "BuyShop" + dt.Month + "-" + dt.Day + "";

                            if (!Directory.Exists(Application.StartupPath + UnhandledExceptionsPath))
                                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath);
                            if (!Directory.Exists(Application.StartupPath + "\\" + UnhandledExceptionsPath + date))
                                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath + date);
                            if (!Directory.Exists(Application.StartupPath + "\\" + UnhandledExceptionsPath + date))
                                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath + date);

                            string fullPath = Application.StartupPath + "\\" + UnhandledExceptionsPath + date + "\\";

                            if (!File.Exists(fullPath + date + ".txt"))
                            {
                                File.WriteAllLines(fullPath + date + ".txt", new string[0]);
                            }


                            using (var SW = File.AppendText(fullPath + date + ".txt"))
                            {
                                SW.WriteLine(text);
                                SW.Close();
                            }
                        }
                        else if (text.StartsWith("[Chat]"))
                        {

                            const string UnhandledExceptionsPath = "Loggs\\";

                            var dt = DateTime.Now;
                            string date = "Chat" + dt.Month + "-" + dt.Day + "";

                            if (!Directory.Exists(Application.StartupPath + UnhandledExceptionsPath))
                                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath);
                            if (!Directory.Exists(Application.StartupPath + "\\" + UnhandledExceptionsPath + date))
                                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath + date);
                            if (!Directory.Exists(Application.StartupPath + "\\" + UnhandledExceptionsPath + date))
                                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath + date);

                            string fullPath = Application.StartupPath + "\\" + UnhandledExceptionsPath + date + "\\";

                            if (!File.Exists(fullPath + date + ".txt"))
                            {
                                File.WriteAllLines(fullPath + date + ".txt", new string[0]);
                            }


                            using (var SW = File.AppendText(fullPath + date + ".txt"))
                            {
                                SW.WriteLine(text);
                                SW.Close();
                            }
                        }
                        else if (text.StartsWith("[Logout]"))
                        {

                            const string UnhandledExceptionsPath = "Loggs\\";

                            var dt = DateTime.Now;
                            string date = "Logout" + dt.Month + "-" + dt.Day + "";

                            if (!Directory.Exists(Application.StartupPath + UnhandledExceptionsPath))
                                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath);
                            if (!Directory.Exists(Application.StartupPath + "\\" + UnhandledExceptionsPath + date))
                                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath + date);
                            if (!Directory.Exists(Application.StartupPath + "\\" + UnhandledExceptionsPath + date))
                                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath + date);

                            string fullPath = Application.StartupPath + "\\" + UnhandledExceptionsPath + date + "\\";

                            if (!File.Exists(fullPath + date + ".txt"))
                            {
                                File.WriteAllLines(fullPath + date + ".txt", new string[0]);
                            }


                            using (var SW = File.AppendText(fullPath + date + ".txt"))
                            {
                                SW.WriteLine(text);
                                SW.Close();
                            }
                        }
                        else if (text.StartsWith("[Login]"))
                        {

                            const string UnhandledExceptionsPath = "Loggs\\";

                            var dt = DateTime.Now;
                            string date = "Login" + dt.Month + "-" + dt.Day + "";

                            if (!Directory.Exists(Application.StartupPath + UnhandledExceptionsPath))
                                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath);
                            if (!Directory.Exists(Application.StartupPath + "\\" + UnhandledExceptionsPath + date))
                                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath + date);
                            if (!Directory.Exists(Application.StartupPath + "\\" + UnhandledExceptionsPath + date))
                                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath + date);

                            string fullPath = Application.StartupPath + "\\" + UnhandledExceptionsPath + date + "\\";

                            if (!File.Exists(fullPath + date + ".txt"))
                            {
                                File.WriteAllLines(fullPath + date + ".txt", new string[0]);
                            }


                            using (var SW = File.AppendText(fullPath + date + ".txt"))
                            {
                                SW.WriteLine(text);
                                SW.Close();
                            }
                        }
                        else if (text.StartsWith("[Item]"))
                        {

                            const string UnhandledExceptionsPath = "Loggs\\";

                            var dt = DateTime.Now;
                            string date = "Item" + dt.Month + "-" + dt.Day + "";

                            if (!Directory.Exists(Application.StartupPath + UnhandledExceptionsPath))
                                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath);
                            if (!Directory.Exists(Application.StartupPath + "\\" + UnhandledExceptionsPath + date))
                                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath + date);
                            if (!Directory.Exists(Application.StartupPath + "\\" + UnhandledExceptionsPath + date))
                                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath + date);

                            string fullPath = Application.StartupPath + "\\" + UnhandledExceptionsPath + date + "\\";

                            if (!File.Exists(fullPath + date + ".txt"))
                            {
                                File.WriteAllLines(fullPath + date + ".txt", new string[0]);
                            }


                            using (var SW = File.AppendText(fullPath + date + ".txt"))
                            {
                                SW.WriteLine(text);
                                SW.Close();
                            }
                        }
                        else if (text.StartsWith("[Inv-Add]"))
                        {

                            const string UnhandledExceptionsPath = "Loggs\\";

                            var dt = DateTime.Now;
                            string date = "Inv-Add" + dt.Month + "-" + dt.Day + "";

                            if (!Directory.Exists(Application.StartupPath + UnhandledExceptionsPath))
                                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath);
                            if (!Directory.Exists(Application.StartupPath + "\\" + UnhandledExceptionsPath + date))
                                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath + date);
                            if (!Directory.Exists(Application.StartupPath + "\\" + UnhandledExceptionsPath + date))
                                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath + date);

                            string fullPath = Application.StartupPath + "\\" + UnhandledExceptionsPath + date + "\\";

                            if (!File.Exists(fullPath + date + ".txt"))
                            {
                                File.WriteAllLines(fullPath + date + ".txt", new string[0]);
                            }


                            using (var SW = File.AppendText(fullPath + date + ".txt"))
                            {
                                SW.WriteLine(text);
                                SW.Close();
                            }
                        }
                        else if (text.StartsWith("[Inv-Lost]"))
                        {

                            const string UnhandledExceptionsPath = "Loggs\\";

                            var dt = DateTime.Now;
                            string date = "Inv-Lost" + dt.Month + "-" + dt.Day + "";

                            if (!Directory.Exists(Application.StartupPath + UnhandledExceptionsPath))
                                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath);
                            if (!Directory.Exists(Application.StartupPath + "\\" + UnhandledExceptionsPath + date))
                                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath + date);
                            if (!Directory.Exists(Application.StartupPath + "\\" + UnhandledExceptionsPath + date))
                                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath + date);

                            string fullPath = Application.StartupPath + "\\" + UnhandledExceptionsPath + date + "\\";

                            if (!File.Exists(fullPath + date + ".txt"))
                            {
                                File.WriteAllLines(fullPath + date + ".txt", new string[0]);
                            }


                            using (var SW = File.AppendText(fullPath + date + ".txt"))
                            {
                                SW.WriteLine(text);
                                SW.Close();
                            }
                        }
                        else if (text.StartsWith("[ExChange]"))
                        {
                            const string UnhandledExceptionsPath = "Loggs\\";

                            var dt = DateTime.Now;
                            string date = "ExChange" + dt.Month + "-" + dt.Day + "";

                            if (!Directory.Exists(Application.StartupPath + UnhandledExceptionsPath))
                                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath);
                            if (!Directory.Exists(Application.StartupPath + "\\" + UnhandledExceptionsPath + date))
                                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath + date);
                            if (!Directory.Exists(Application.StartupPath + "\\" + UnhandledExceptionsPath + date))
                                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath + date);

                            string fullPath = Application.StartupPath + "\\" + UnhandledExceptionsPath + date + "\\";

                            if (!File.Exists(fullPath + date + ".txt"))
                            {
                                File.WriteAllLines(fullPath + date + ".txt", new string[0]);
                            }


                            using (var SW = File.AppendText(fullPath + date + ".txt"))
                            {
                                SW.WriteLine(text);
                                SW.Close();
                            }
                        }
                        else if (text.StartsWith("[Hossu]"))
                        {
                            const string UnhandledExceptionsPath = "Loggs\\";

                            var dt = DateTime.Now;
                            string date = "Hossu" + dt.Month + "-" + dt.Day + "";

                            if (!Directory.Exists(Application.StartupPath + UnhandledExceptionsPath))
                                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath);
                            if (!Directory.Exists(Application.StartupPath + "\\" + UnhandledExceptionsPath + date))
                                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath + date);
                            if (!Directory.Exists(Application.StartupPath + "\\" + UnhandledExceptionsPath + date))
                                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath + date);

                            string fullPath = Application.StartupPath + "\\" + UnhandledExceptionsPath + date + "\\";

                            if (!File.Exists(fullPath + date + ".txt"))
                            {
                                File.WriteAllLines(fullPath + date + ".txt", new string[0]);
                            }


                            using (var SW = File.AppendText(fullPath + date + ".txt"))
                            {
                                SW.WriteLine(text);
                                SW.Close();
                            }
                        }
                        else if (text.StartsWith("[Cheating]"))
                        {
                            const string UnhandledExceptionsPath = "Loggs\\";

                            var dt = DateTime.Now;
                            string date = "Cheating" + dt.Month + "-" + dt.Day + "";

                            if (!Directory.Exists(Application.StartupPath + UnhandledExceptionsPath))
                                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath);
                            if (!Directory.Exists(Application.StartupPath + "\\" + UnhandledExceptionsPath + date))
                                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath + date);
                            if (!Directory.Exists(Application.StartupPath + "\\" + UnhandledExceptionsPath + date))
                                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath + date);

                            string fullPath = Application.StartupPath + "\\" + UnhandledExceptionsPath + date + "\\";

                            if (!File.Exists(fullPath + date + ".txt"))
                            {
                                File.WriteAllLines(fullPath + date + ".txt", new string[0]);
                            }


                            using (var SW = File.AppendText(fullPath + date + ".txt"))
                            {
                                SW.WriteLine(text);
                                SW.Close();
                            }
                        }
                        else if (text.StartsWith("[ShopingMall]"))
                        {
                            const string UnhandledExceptionsPath = "Loggs\\";

                            var dt = DateTime.Now;
                            string date = "ShopingMall" + dt.Month + "-" + dt.Day + "";

                            if (!Directory.Exists(Application.StartupPath + UnhandledExceptionsPath))
                                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath);
                            if (!Directory.Exists(Application.StartupPath + "\\" + UnhandledExceptionsPath + date))
                                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath + date);
                            if (!Directory.Exists(Application.StartupPath + "\\" + UnhandledExceptionsPath + date))
                                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath + date);

                            string fullPath = Application.StartupPath + "\\" + UnhandledExceptionsPath + date + "\\";

                            if (!File.Exists(fullPath + date + ".txt"))
                            {
                                File.WriteAllLines(fullPath + date + ".txt", new string[0]);
                            }


                            using (var SW = File.AppendText(fullPath + date + ".txt"))
                            {
                                SW.WriteLine(text);
                                SW.Close();
                            }
                        }
                        else if (text.StartsWith("[CoinsShop]"))
                        {
                            const string UnhandledExceptionsPath = "Loggs\\";

                            var dt = DateTime.Now;
                            string date = "CoinsShop" + dt.Month + "-" + dt.Day + "";

                            if (!Directory.Exists(Application.StartupPath + UnhandledExceptionsPath))
                                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath);
                            if (!Directory.Exists(Application.StartupPath + "\\" + UnhandledExceptionsPath + date))
                                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath + date);
                            if (!Directory.Exists(Application.StartupPath + "\\" + UnhandledExceptionsPath + date))
                                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath + date);

                            string fullPath = Application.StartupPath + "\\" + UnhandledExceptionsPath + date + "\\";

                            if (!File.Exists(fullPath + date + ".txt"))
                            {
                                File.WriteAllLines(fullPath + date + ".txt", new string[0]);
                            }


                            using (var SW = File.AppendText(fullPath + date + ".txt"))
                            {
                                SW.WriteLine(text);
                                SW.Close();
                            }
                        }
                        else if (text.StartsWith("[DIABLOSTORE]"))
                        {
                            const string UnhandledExceptionsPath = "Loggs\\";

                            var dt = DateTime.Now;
                            string date = "DIABLOSTORE" + dt.Month + "-" + dt.Day + "";

                            if (!Directory.Exists(Application.StartupPath + UnhandledExceptionsPath))
                                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath);
                            if (!Directory.Exists(Application.StartupPath + "\\" + UnhandledExceptionsPath + date))
                                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath + date);
                            if (!Directory.Exists(Application.StartupPath + "\\" + UnhandledExceptionsPath + date))
                                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath + date);

                            string fullPath = Application.StartupPath + "\\" + UnhandledExceptionsPath + date + "\\";

                            if (!File.Exists(fullPath + date + ".txt"))
                            {
                                File.WriteAllLines(fullPath + date + ".txt", new string[0]);
                            }


                            using (var SW = File.AppendText(fullPath + date + ".txt"))
                            {
                                SW.WriteLine(text);
                                SW.Close();
                            }
                        }

                        else if (text.StartsWith("[Trade]"))
                        {
                            const string UnhandledExceptionsPath = "Loggs\\";

                            var dt = DateTime.Now;
                            string date = "Trade" + dt.Month + "-" + dt.Day + "";

                            if (!Directory.Exists(Application.StartupPath + UnhandledExceptionsPath))
                                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath);
                            if (!Directory.Exists(Application.StartupPath + "\\" + UnhandledExceptionsPath + date))
                                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath + date);
                            if (!Directory.Exists(Application.StartupPath + "\\" + UnhandledExceptionsPath + date))
                                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath + date);

                            string fullPath = Application.StartupPath + "\\" + UnhandledExceptionsPath + date + "\\";

                            if (!File.Exists(fullPath + date + ".txt"))
                            {
                                File.WriteAllLines(fullPath + date + ".txt", new string[0]);
                            }


                            using (var SW = File.AppendText(fullPath + date + ".txt"))
                            {
                                SW.WriteLine(text);
                                SW.Close();
                            }
                        }
                       else if (text.StartsWith("[GMLogs]"))
                        {
                            const string UnhandledExceptionsPath = "Loggs\\";

                            var dt = DateTime.Now;
                            string date = "GMLogs";

                            if (!Directory.Exists(Application.StartupPath + UnhandledExceptionsPath))
                                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath);
                            if (!Directory.Exists(Application.StartupPath + "\\" + UnhandledExceptionsPath + date))
                                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath + date);
                            if (!Directory.Exists(Application.StartupPath + "\\" + UnhandledExceptionsPath + date))
                                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath + date);

                            string fullPath = Application.StartupPath + "\\" + UnhandledExceptionsPath + date + "\\";

                            if (!File.Exists(fullPath + date + ".txt"))
                            {
                                File.WriteAllLines(fullPath + date + ".txt", new string[0]);
                            }

                            using (var SW = File.AppendText(fullPath + date + ".txt"))
                            {
                                SW.WriteLine(text);
                                SW.Close();
                            }
                        }
                        else if (text.StartsWith("[NormalLogs]"))
                        {
                            const string UnhandledExceptionsPath = "Loggs\\";

                            var dt = DateTime.Now;
                            string date = "NormalLogs";

                            if (!Directory.Exists(Application.StartupPath + UnhandledExceptionsPath))
                                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath);
                            if (!Directory.Exists(Application.StartupPath + "\\" + UnhandledExceptionsPath + date))
                                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath + date);
                            if (!Directory.Exists(Application.StartupPath + "\\" + UnhandledExceptionsPath + date))
                                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath + date);

                            string fullPath = Application.StartupPath + "\\" + UnhandledExceptionsPath + date + "\\";

                            if (!File.Exists(fullPath + date + ".txt"))
                            {
                                File.WriteAllLines(fullPath + date + ".txt", new string[0]);
                            }

                            using (var SW = File.AppendText(fullPath + date + ".txt"))
                            {
                                SW.WriteLine(text);
                                SW.Close();
                            }
                        }
                    }
                    else if (obj is Role.GameMap)
                    {
                        UpdateMapRace(obj as Role.GameMap);
                    }
                    else if (obj is Role.Instance.Guild.Member)
                    {
                        UpdateGuildMember(obj as Role.Instance.Guild.Member);
                    }
                    else if (obj is Role.Instance.Guild.UpdateDB)
                    {
                        UpdateGuildMember(obj as Role.Instance.Guild.UpdateDB);
                    }
                    else if (obj is Role.Instance.Union.Member)
                    {
                        UpdateUnionMember(obj as Role.Instance.Union.Member);
                    }
                    else if (obj is Role.Instance.Clan.Member)
                    {
                        UpdateClanMember(obj as Role.Instance.Clan.Member);
                    }
                    else
                    {
                        Client.GameClient client = obj as Client.GameClient;

                        if (client.Player != null && client.Player.Delete == true)
                        {
                            if (client.Map != null)
                                client.Map.View.LeaveMap<Role.Player>(client.Player);

                            DateTime Now64 = DateTime.Now;

                            MyConsole.WriteLine("Client " + client.Player.Name + " delete he account.");

                            if (File.Exists(Program.ServerConfig.DbLocation + "\\Users\\" + client.Player.UID + ".ini"))
                                File.Copy(Program.ServerConfig.DbLocation + "\\Users\\" + client.Player.UID + ".ini", Program.ServerConfig.DbLocation + "\\DeletedAccounts\\Users\\" + client.Player.UID + "date" + GenerateDate() + ".ini", true);
                            if (File.Exists(Program.ServerConfig.DbLocation + "\\PlayersSpells\\" + client.Player.UID + ".bin"))
                                File.Copy(Program.ServerConfig.DbLocation + "\\PlayersSpells\\" + client.Player.UID + ".bin", Program.ServerConfig.DbLocation + "\\DeletedAccounts\\PlayersSpells\\" + client.Player.UID + "date" + GenerateDate() + ".bin", true);
                            if (File.Exists(Program.ServerConfig.DbLocation + "\\PlayersProfs\\" + client.Player.UID + ".bin"))
                                File.Copy(Program.ServerConfig.DbLocation + "\\PlayersProfs\\" + client.Player.UID + ".bin", Program.ServerConfig.DbLocation + "\\DeletedAccounts\\PlayersProfs\\" + client.Player.UID + "date" + GenerateDate() + ".bin", true);
                            if (File.Exists(Program.ServerConfig.DbLocation + "\\PlayersItems\\" + client.Player.UID + ".bin"))
                                File.Copy(Program.ServerConfig.DbLocation + "\\PlayersItems\\" + client.Player.UID + ".bin", Program.ServerConfig.DbLocation + "\\DeletedAccounts\\PlayersItems\\" + client.Player.UID + "date" + GenerateDate() + ".bin");


                            if (File.Exists(Program.ServerConfig.DbLocation + "\\Users\\" + client.Player.UID + ".ini"))
                                File.Delete(Program.ServerConfig.DbLocation + "\\Users\\" + client.Player.UID + ".ini");
                            if (File.Exists(Program.ServerConfig.DbLocation + "\\PlayersSpells\\" + client.Player.UID + ".bin"))
                                File.Delete(Program.ServerConfig.DbLocation + "\\PlayersSpells\\" + client.Player.UID + ".bin");
                            if (File.Exists(Program.ServerConfig.DbLocation + "\\PlayersProfs\\" + client.Player.UID + ".bin"))
                                File.Delete(Program.ServerConfig.DbLocation + "\\PlayersProfs\\" + client.Player.UID + ".bin");
                            if (File.Exists(Program.ServerConfig.DbLocation + "\\PlayersItems\\" + client.Player.UID + ".bin"))
                                File.Delete(Program.ServerConfig.DbLocation + "\\PlayersItems\\" + client.Player.UID + ".bin");
                            if (File.Exists(Program.ServerConfig.DbLocation + "\\Quests\\" + client.Player.UID + ".bin"))
                                File.Delete(Program.ServerConfig.DbLocation + "\\Quests\\" + client.Player.UID + ".bin");


                            Role.Instance.House house;
                            if (client.MyHouse != null && Role.Instance.House.HousePoll.ContainsKey(client.Player.UID))
                                Role.Instance.House.HousePoll.TryRemove(client.Player.UID, out house);
                            // PrestigeRanking.Remove(client.Player.UID);
                            if (File.Exists(Program.ServerConfig.DbLocation + "\\Houses\\" + client.Player.UID + ".bin"))
                                File.Delete(Program.ServerConfig.DbLocation + "\\Houses\\" + client.Player.UID + ".bin");

                            Role.Instance.Chi chi;
                            if (Role.Instance.Chi.ChiPool.ContainsKey(client.Player.UID))
                            {
                                Role.Instance.Chi.ChiPool.TryRemove(client.Player.UID, out chi);
                                WindowsAPI.IniFile write = new WindowsAPI.IniFile("\\DeletedAccounts\\ChiInfo.txt");
                                write.WriteString(client.Player.UID.ToString() + "date" + GenerateDate() + "", "Dragon", chi.Dragon.ToString());
                                write.WriteString(client.Player.UID.ToString() + "date" + GenerateDate() + "", "Phoenix", chi.Phoenix.ToString());
                                write.WriteString(client.Player.UID.ToString() + "date" + GenerateDate() + "", "Turtle", chi.Turtle.ToString());
                                write.WriteString(client.Player.UID.ToString() + "date" + GenerateDate() + "", "Tiger", chi.Tiger.ToString());
                            }
                            Role.Instance.Flowers flow;
                            if (Role.Instance.Flowers.ClientPoll.ContainsKey(client.Player.UID))
                            {
                                Role.Instance.Flowers.ClientPoll.TryRemove(client.Player.UID, out flow);
                            }

                            Role.Instance.InnerPower innerpower;
                            if (Role.Instance.InnerPower.InnerPowerPolle.ContainsKey(client.Player.UID))
                            {
                                Role.Instance.InnerPower.InnerPowerPolle.TryRemove(client.Player.UID, out innerpower);
                                WindowsAPI.IniFile write = new WindowsAPI.IniFile("\\AccountDeleted\\InnerPower.txt");
                                write.WriteString(client.Player.UID.ToString() + "date" + GenerateDate() + "", "Dragon", innerpower.ToString());
                            }
                            Role.Instance.JiangHu Jiang;
                            if (Role.Instance.JiangHu.Poll.ContainsKey(client.Player.UID))
                            {
                                Role.Instance.JiangHu.Poll.TryRemove(client.Player.UID, out Jiang);
                                WindowsAPI.IniFile write = new WindowsAPI.IniFile("\\AccountDeleted\\JiangHuInfo.txt");
                                write.WriteString(client.Player.UID.ToString() + "date" + GenerateDate() + "", "Jiang", Jiang.ToString());
                            }
                            Role.Instance.Associate.MyAsociats Associate;
                            if (Role.Instance.Associate.Associates.TryGetValue(client.Player.UID, out Associate))
                            {
                                Role.Instance.Associate.Associates.TryRemove(client.Player.UID, out Associate);
                            }
                            Client.GameClient user;
                            Database.Server.GamePoll.TryRemove(client.Player.UID, out user);

                            if (Server.NameUsed.Contains(user.Player.Name.GetHashCode()))
                            {
                                lock (Server.NameUsed)
                                    Server.NameUsed.Remove(user.Player.Name.GetHashCode());
                            }
                            client.Player.Delete = false;
                            return;
                        }

                        if ((client.ClientFlag & Client.ServerFlag.RemoveSpouse) == Client.ServerFlag.RemoveSpouse)
                        {
                            DestroySpouse(client);
                            client.ClientFlag &= ~Client.ServerFlag.RemoveSpouse;
                            return;
                        }
                        if ((client.ClientFlag & Client.ServerFlag.UpdateSpouse) == Client.ServerFlag.UpdateSpouse)
                        {
                            UpdateSpouse(client);
                            client.ClientFlag &= ~Client.ServerFlag.UpdateSpouse;
                            return;
                        }
                        if ((client.ClientFlag & Client.ServerFlag.SetLocation) != Client.ServerFlag.SetLocation && (client.ClientFlag & Client.ServerFlag.OnLoggion) == Client.ServerFlag.OnLoggion)
                        {
                            Game.MsgServer.MsgLoginClient.LoginHandler(client, client.OnLogin);
                        }
                        else if ((client.ClientFlag & Client.ServerFlag.QueuesSave) == Client.ServerFlag.QueuesSave)
                        {
                            if (client.Player.OnTransform)
                            {
                                client.Player.HitPoints = Math.Min(client.Player.HitPoints, (int)client.Status.MaxHitpoints);

                            }
                            SaveClient(client);

                        }
                    }
                }
                catch (Exception e) { MyConsole.SaveException(e); }
            }
        }
    }
}
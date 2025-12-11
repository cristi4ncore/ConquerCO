using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeathWish.Game.MsgNpc;

namespace DeathWish.Game.MsgServer
{
    public static unsafe partial class MsgBuilder
    {
        public static unsafe ServerSockets.Packet LoginHandlerCreate(this ServerSockets.Packet stream, uint Type, uint Map)
        {
            stream.InitWriter();

            stream.Write(0);
            stream.Write(Type);
            stream.Write(Map);

            stream.Finalize(GamePackets.MapLoading);

            return stream;
        }

    }
    public unsafe struct MsgLoginHandler
    {

        [PacketAttribute(GamePackets.MapLoading)]
        public unsafe static void LoadMap(Client.GameClient client, ServerSockets.Packet packet)
        {

            if ((client.ClientFlag & Client.ServerFlag.AcceptLogin) == Client.ServerFlag.AcceptLogin)
            {
                try
                {

                    client.Player.ServerID = (ushort)Database.GroupServerList.MyServerInfo.ID;

                    client.Send(packet.HeroInfo(client.Player));

                    MsgChiInfo.MsgHandleChi.SendInfo(client, MsgChiInfo.Action.Upgrade, client);
                    client.Send(packet.FlowerCreate(Role.Core.IsBoy(client.Player.Body) ? MsgFlower.FlowerAction.Flower : MsgFlower.FlowerAction.FlowerSender
                        , 0, 0, client.Player.Flowers.RedRoses, client.Player.Flowers.RedRoses.Amount2day
                        , client.Player.Flowers.Lilies, client.Player.Flowers.Lilies.Amount2day
                        , client.Player.Flowers.Orchids, client.Player.Flowers.Orchids.Amount2day
                        , client.Player.Flowers.Tulips, client.Player.Flowers.Tulips.Amount2day));


                    if (client.Player.Flowers.FreeFlowers > 0)
                    {
                        client.Send(packet.FlowerCreate(Role.Core.IsBoy(client.Player.Body)
                            ? MsgFlower.FlowerAction.FlowerSender : MsgFlower.FlowerAction.Flower
                            , 0, 0, client.Player.Flowers.FreeFlowers));
                    }
                    client.Send(packet.NobilityIconCreate(client.Player.Nobility));
                    if (client.Player.Achievement != null)
                        client.Player.Achievement.Send(client, packet);

                    if (client.Player.BlessTime > 0)
                        client.Player.SendUpdate(packet, client.Player.BlessTime, MsgUpdate.DataType.LuckyTimeTimer);

                    client.Player.ProtectAttack(1000 * 10);
                    client.Player.CreateHeavenBlessPacket(packet, true);


                    if (MsgTournaments.MsgSchedules.CurrentTournament.Type == MsgTournaments.TournamentType.QuizShow
                        && MsgTournaments.MsgSchedules.CurrentTournament.Process == MsgTournaments.ProcesType.Alive)
                        MsgTournaments.MsgSchedules.CurrentTournament.Join(client, packet);


                    if (client.Player.DExpTime > 0)
                        client.Player.CreateExtraExpPacket(packet);
                    client.Player.UpdateExploitsRank();

                    if (client.Player.MyClan != null)
                    {
                        client.Player.MyClan.SendThat(packet, client);
                        client.Send(packet.ClanRelation(client.Player.MyClan.Ally.Values.ToList(), MsgClan.Info.LoginAlly));
                        client.Send(packet.ClanRelation(client.Player.MyClan.Enemy.Values.ToList(), MsgClan.Info.LoginEnemy));

                    }

                    client.Equipment.Show(packet);
                    client.Inventory.ShowALL(packet);

                    if (client.Player.MyJiangHu != null)
                        client.Player.MyJiangHu.LoginClient(packet, client);
                    else if (client.Player.Reborn == 2)
                    {
                        client.Send(packet.JiangHuInfoCreate(MsgJiangHuInfo.JiangMode.IconBar, "0"));
                    }

                    foreach (var chipower in client.Player.MyChi)
                        client.Player.MyChi.SendQueryUpdate(client, chipower, packet);

                    foreach (var item in client.Confiscator.RedeemContainer.Values)
                    {
                        var Dataitem = item;
                        Dataitem.DaysLeft = (uint)(TimeSpan.FromTicks(DateTime.Now.Ticks).Days - TimeSpan.FromTicks(Role.Instance.Confiscator.GetTimer(item.Date).Ticks).Days);
                        if (Dataitem.DaysLeft > 7)
                        {
                            Dataitem.Action = MsgDetainedItem.ContainerType.RewardCps;
                        }
                        if (Dataitem.Action != MsgDetainedItem.ContainerType.RewardCps)
                        {
                            Dataitem.Action = MsgDetainedItem.ContainerType.DetainPage;
                            Dataitem.Send(client, packet);
                        }
                        if (Dataitem.Action == MsgDetainedItem.ContainerType.RewardCps)
                            client.Confiscator.RedeemContainer.TryRemove(item.UID, out Dataitem);
                    }
                    foreach (var item in client.Confiscator.ClaimContainer.Values)
                    {
                        var Dataitem = item;
                        Dataitem.DaysLeft = (uint)(TimeSpan.FromTicks(DateTime.Now.Ticks).Days - TimeSpan.FromTicks(Role.Instance.Confiscator.GetTimer(item.Date).Ticks).Days);
                        if (Dataitem.RewardConquerPoints != 0)
                        {
                            Dataitem.Action = MsgDetainedItem.ContainerType.RewardCps;
                        }
                        Dataitem.Send(client, packet);
                        client.Confiscator.ClaimContainer[item.UID] = Dataitem;
                    }

                    if (MsgTournaments.MsgSchedules.GuildWar.RewardDeputiLeader.Contains(client.Player.UID))
                        client.Player.AddFlag(MsgUpdate.Flags.TopDeputyLeader, Role.StatusFlagsBigVector32.PermanentFlag, false);
                    if (MsgTournaments.MsgSchedules.GuildWar.RewardLeader.Contains(client.Player.UID))
                        client.Player.AddFlag(MsgUpdate.Flags.TopGuildLeader, Role.StatusFlagsBigVector32.PermanentFlag, false);

                    if (MsgTournaments.MsgSchedules.SuperGuildWar.RewardDeputiLeader.Contains(client.Player.UID))
                        client.Player.AddFlag(MsgUpdate.Flags.Surround2, Role.StatusFlagsBigVector32.PermanentFlag, false);
                    if (MsgTournaments.MsgSchedules.SuperGuildWar.RewardLeader.Contains(client.Player.UID))
                        client.Player.AddFlag(MsgUpdate.Flags.Surround1, Role.StatusFlagsBigVector32.PermanentFlag, false);
                    if (MsgTournaments.MsgSchedules.SuperGuildWar.RewardMember.Contains(client.Player.UID))
                        client.Player.AddFlag(MsgUpdate.Flags.Surround3, Role.StatusFlagsBigVector32.PermanentFlag, false);


                    #region AutoSpell
                  
                    Game.ServerAutoCheck.RemoveSpell(client);
                    #endregion
                    client.Player.PKPoints = client.Player.PKPoints;
                    //Reparar Tiempo cursed
                    //if (client.Player.CursedTimer > 0)
                    //{
                    //    client.Player.AddCursed(client.Player.CursedTimer);
                    //}

                    client.Send(packet.ServerTimerCreate());

                    MsgTournaments.MsgSchedules.ClassPkWar.LoginClient(client);
                   MsgTournaments.MsgSchedules.ElitePkTournament.GetTitle(client, packet);
                    MsgTournaments.MsgSchedules.TeamPkTournament.GetTitle(client, packet);
                    MsgTournaments.MsgSchedules.SkillTeamPkTournament.GetTitle(client, packet);
                    if (MsgTournaments.MsgSchedules.CouplesPKWar.Winner1 == client.Player.Name ||
                                MsgTournaments.MsgSchedules.CouplesPKWar.Winner2 == client.Player.Name)
                        client.Player.AddFlag(MsgUpdate.Flags.TopSpouse, Role.StatusFlagsBigVector32.PermanentFlag, false);
                    MsgTournaments.MsgSchedules.PkWar.AddTop(client);
                    if (MsgTournaments.MsgBroadcast.CurrentBroadcast.EntityID != 1)
                    {
                        client.Send(new MsgServer.MsgMessage(MsgTournaments.MsgBroadcast.CurrentBroadcast.Message
                            , "ALLUSERS"
                            , MsgTournaments.MsgBroadcast.CurrentBroadcast.EntityName
                            , MsgServer.MsgMessage.MsgColor.red
                            , MsgServer.MsgMessage.ChatMode.BroadcastMessage
                            ).GetArray(packet));
                    }


                    if (client.Player.Money > 0)
                        client.Player.SendUpdate(packet, client.Player.Money, MsgUpdate.DataType.Money);

                    client.Activeness.CheckTasks(packet);

                    //client.Activeness.IncreaseTask(1);
                    //client.Activeness.IncreaseTask(13);
                    //client.Activeness.IncreaseTask(25);

                    //if (client.Player.VipLevel > 1)
                    //{
                    //    client.Activeness.IncreaseTask(2);
                    //    client.Activeness.IncreaseTask(14);
                    //    client.Activeness.IncreaseTask(26);
                    //}

                    //if (client.Player.VipLevel == 1)
                    //{
                    //    client.Player.AddTitle(DeathWish.Game.MsgTournaments.MsgEliteTournament.top_typ.VIP1, true);
                    //}
                    //if (client.Player.VipLevel == 2)
                    //{
                    //    client.Player.AddTitle(DeathWish.Game.MsgTournaments.MsgEliteTournament.top_typ.VIP2, true);
                    //}
                    //if (client.Player.VipLevel == 3)
                    //{
                    //    client.Player.AddTitle(DeathWish.Game.MsgTournaments.MsgEliteTournament.top_typ.VIP3, true);
                    //}
                    //if (client.Player.VipLevel == 4)
                    //{
                    //    client.Player.AddTitle(DeathWish.Game.MsgTournaments.MsgEliteTournament.top_typ.VIP4, true);
                    //}
                    //if (client.Player.VipLevel == 5)
                    //{
                    //    client.Player.AddTitle(DeathWish.Game.MsgTournaments.MsgEliteTournament.top_typ.VIP5, true);
                    //}
                    //if (client.Player.VipLevel == 6)
                    //{
                    //    client.Player.AddTitle(DeathWish.Game.MsgTournaments.MsgEliteTournament.top_typ.VIP6, true);
                    //}

                    //if (client.Player.WarDropeFull >= 1000000)
                    //{
                    //    client.Player.AddTitle(DeathWish.Game.MsgTournaments.MsgEliteTournament.top_typ.GoldenRacer, true);
                    //}
                    //if (client.Player.WarDropeFull >= 1000000)
                    //{
                    //    client.Player.AddTitle(DeathWish.Game.MsgTournaments.MsgEliteTournament.top_typ.Legendary, true);
                    //}
                    //if (client.Player.WarDropeFull >= 1000000)
                    //{
                    //    client.Player.AddTitle(DeathWish.Game.MsgTournaments.MsgEliteTournament.top_typ.Peerless, true);
                    //}
                    //if (client.Player.WarDropeFull >= 1000000)
                    //{
                    //    client.Player.AddTitle(DeathWish.Game.MsgTournaments.MsgEliteTournament.top_typ.Outstanding, true);
                    //}
                    //if (client.Player.WarDropeFull >= 1000000)
                    //{
                    //    client.Player.AddTitle(DeathWish.Game.MsgTournaments.MsgEliteTournament.top_typ.Expert, true);
                    //}
                    //if (client.Player.WarDropeFull >= 1000000)
                    //{
                    //    client.Player.AddTitle(DeathWish.Game.MsgTournaments.MsgEliteTournament.top_typ.GM, true);
                    //}

                    //if (client.ProjectManager)
                    //{
                    //    client.Player.AddTitle(DeathWish.Game.MsgTournaments.MsgEliteTournament.top_typ.Player, true);
                    //}
                    //else
                    //{
                    //    client.Player.AddTitle(DeathWish.Game.MsgTournaments.MsgEliteTournament.top_typ.Player, true);
                    //}

                    client.Activeness.UpdateActivityPoints(packet);
                    client.Activeness.UpdateClaimRewards(packet);
                    client.Player.InnerPower.AddPotency(packet, client, 0);
                    client.Player.UpdateVip(packet);

                    client.Player.SendUpdate(packet, client.Player.Merchant, MsgUpdate.DataType.Merchant);

                    client.Player.MainFlag |= Role.Player.MainFlagType.CanClaim;
                    //client.Player.MainFlag |= Role.Player.MainFlagType.ShowSpecialItems;
                    client.Player.SendUpdate(packet, (uint)client.Player.MainFlag, Game.MsgServer.MsgUpdate.DataType.MainFlag, false);

                    //if ((client.Player.MainFlag & Role.Player.MainFlagType.CanClaim) != Role.Player.MainFlagType.CanClaim)
                    //{
                    //    client.Player.MainFlag |= Role.Player.MainFlagType.CanClaim;
                    //    client.Player.SendUpdate(packet, (uint)client.Player.MainFlag, Game.MsgServer.MsgUpdate.DataType.MainFlag, false);
                    //}

                    ActionQuery action = new ActionQuery()
                    {
                        ObjId = client.Player.UID,
                        Type = (ActionType)157,
                        dwParam = 2
                    };

                    client.Send(packet.ActionCreate(&action));

                    client.Send(packet.ServerConfig());

                    if (client.Player.InUnion)
                    {
                        client.Player.MyUnion.SendMyInfo(packet, client);
                    }
                    //if (Database.RechargeShop.RechargeAccounts.ContainsKey(client.Player.UID))
                    //{
                    //    if (Database.RechargeShop.RechargeAccounts[client.Player.UID] >= client.Player.RechargeProgress)
                    //    {
                    //        client.Player.RechargeProgress = Database.RechargeShop.RechargeAccounts[client.Player.UID];
                    //    }
                    //    else
                    //    {
                    //        Database.RechargeShop.RechargeAccounts[client.Player.UID] = client.Player.RechargeProgress;
                    //    }
                    //}
                    //else if (client.Player.RechargeProgress > 0)
                    //{
                    //    Database.RechargeShop.RechargeAccounts.TryAdd(client.Player.UID, client.Player.RechargeProgress);
                    //}

                    if (client.Player.VipLevel == 0)
                    {
                        client.Player.VipLevel = 1;
                        client.Player.SendUpdate(packet, client.Player.VipLevel, MsgUpdate.DataType.VIPLevel);
                        client.Player.UpdateVip(packet);
                    }

                    Database.TitleStorage.CheckUpUser(client, packet);

                    foreach (var title in client.Player.SpecialTitles)
                        client.Player.AddSpecialTitle(title, packet);
                    client.Player.SendSpecialTitle(packet);
                    client.Player.ActiveSpecialTitles(packet);

                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        client.Send(stream.AutoHuntCreate(0, 341));
                    }
                    //using (var rec = new ServerSockets.RecycledPacket())
                    //{
                    //    var stream = rec.GetStream();
                    //    MsgGoldenTree.LoginEvaluation(client, stream);
                    //}
                    if (client.Player.SecurityPassword != 0)
                    {
                        client.Send(packet.SecondaryPasswordCreate(MsgSecondaryPassword.ActionID.PasswordCorrect, 1, 0));
                    }
                    else
                        client.Player.IsCheckedPass = true;
                    MsgTournaments.MsgSchedules.DragonWar.AddTop(client);
                    MsgTournaments.MsgSchedules.KungfuSchool.AddTop(client);
                    MsgTournaments.MsgSchedules.TOPS.AddTop(client);
                    MsgTournaments.MsgSchedules.LastManStand.AddTop(client);
                    DeathWish.Game.MsgTournaments.MsgSchedules.SendSysMesage("Welcome " + client.Player.Name + " [ Online : " + Database.Server.GamePoll.Count + " ] & [ MaxOnline : " + Client.PoolProcesses.MaxOnline + " ]", MsgMessage.ChatMode.TopLeftSystem);
                    client.SendSysMesage("Welcome To " + Program.ServerConfig.ServerName + ".", MsgMessage.ChatMode.Talk);
                    client.SendSysMesage("The GuildWar Will End  23:00 Every Friday.", MsgMessage.ChatMode.Talk);
                    client.SendSysMesage("Official Site : " + Program.ServerConfig.OfficialWebSite + " ", MsgMessage.ChatMode.Talk);
                    client.SendSysMesage("Enjoy .", MsgMessage.ChatMode.Talk);
                    client.MyBotBooth.getbooth();
                    client.Player.UpdateInventorySash(packet);
                    if (client.Player.ExpProtection > 0)
                        client.Player.CreateExpProtection(packet, 0, false);

                    if (Database.AtributesStatus.IsTrojan(client.Player.Class)
                        || Database.AtributesStatus.IsTrojan(client.Player.FirstClass)
                        || Database.AtributesStatus.IsTrojan(client.Player.SecoundeClass))
                    {
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Cyclone))
                            client.MySpells.Add(packet, (ushort)Role.Flags.SpellID.Cyclone);
                    }

                    if (client.Inventory.HaveSpace(1))
                    {
                        foreach (var item in client.Equipment.ClientItems.Values)
                        {
                            if (item.Position >= (uint)Role.Flags.ConquerItem.Head && item.Position <= (uint)Role.Flags.ConquerItem.Wing)
                            {
                                if (client.Inventory.HaveSpace(1) && item.Position == (uint)Role.Flags.ConquerItem.RightWeapon
                                    && item.Position == (uint)Role.Flags.ConquerItem.LeftWeapon)
                                {
                                    if (!Database.ItemType.IsShield(item.ITEM_ID))
                                    {
                                        if (!Database.ItemType.Equipable(item.ITEM_ID, client))
                                        {
                                            client.Equipment.Remove((Role.Flags.ConquerItem)item.Position, packet);
                                        }
                                    }
                                }
                            }
                            else if (item.Position >= (uint)Role.Flags.ConquerItem.AleternanteHead && item.Position <= (uint)Role.Flags.ConquerItem.AleternanteGarment)
                            {
                                if (client.Inventory.HaveSpace(1) && item.Position == (uint)Role.Flags.ConquerItem.AleternanteRightWeapon
                                    && item.Position == (uint)Role.Flags.ConquerItem.AleternanteLeftWeapon)
                                {
                                    if (!Database.ItemType.IsShield(item.ITEM_ID))
                                    {
                                        if (!Database.ItemType.Equipable(item.ITEM_ID, client))
                                        {
                                            client.Equipment.RemoveAlternante((Role.Flags.ConquerItem)item.Position, packet);
                                        }
                                    }
                                }
                            }
                        }
                    }


                    MsgServer.MsgSameGroupServerList.GroupServer group = new MsgSameGroupServerList.GroupServer();
                    var GroupServers = Database.GroupServerList.GroupServers.Values.ToArray();
                    group.Servers = new MsgSameGroupServerList.Server[Database.GroupServerList.GroupServers.Count];
                    for (int x = 0; x < GroupServers.Length; x++)
                    {
                        var DBServer = GroupServers[x];
                        group.Servers[x] = new MsgSameGroupServerList.Server();
                        group.Servers[x].GroupID = DBServer.Group;
                        group.Servers[x].MapID = DBServer.MapID;
                        group.Servers[x].Name = DBServer.Name;
                        group.Servers[x].dwparam = 1;
                        group.Servers[x].X = DBServer.X;
                        group.Servers[x].Y = DBServer.Y;
                        group.Servers[x].ServerID = DBServer.ID;
                    }
                    client.Send(packet.CreateGroupServerList(group));


                    client.Warehouse.SendReturnedItems(packet);


                    client.ClientFlag &= ~Client.ServerFlag.AcceptLogin;
                    client.ClientFlag |= Client.ServerFlag.LoginFull;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }

            }
        }

    }
}

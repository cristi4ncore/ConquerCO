namespace DeathWish.Threading
{
    /// <summary>
    /// Controller for the player thread.
    /// </summary>
    public static class WorldTournaments
    {
        /// <summary>
        /// Handles the thread.
        /// </summary>
        public static void Handle()
        {
            try
            {
                Extensions.Time32 clock = Extensions.Time32.Now;
                System.DateTime DateNow = System.DateTime.Now;
                #region Console Online Count
                if (DeathWish.Client.PoolProcesses.Online > DeathWish.Client.PoolProcesses.MaxOnline)
                    DeathWish.Client.PoolProcesses.MaxOnline = DeathWish.Client.PoolProcesses.Online;
                if (Program.ServerConfig.IsInterServer)
                    MyConsole.Title = "[" + Database.GroupServerList.MyServerInfo.Name + "]QueuePackets: " + ServerSockets.PacketRecycle.Count + " Online " + Database.Server.GamePoll.Count + " Time: " + System.DateTime.Now.Hour + "/" + System.DateTime.Now.Minute + "/" + System.DateTime.Now.Second + "";
                else
                    MyConsole.Title = "[" + Program.ServerConfig.ServerName + "] - [Players Online]: " + DeathWish.Client.PoolProcesses.Online + " - [Max Online]: " + DeathWish.Client.PoolProcesses.MaxOnline + " - Game Clock: [" + System.DateTime.Now.Hour + ":" + System.DateTime.Now.Minute + ":" + System.DateTime.Now.Second + "]";
                #endregion
                #region OnlineCount
                var cmd = new COServer.Database.MySqlCommand(COServer.Database.MySqlCommandType.UPDATE);
                cmd.Update("online").Set("OnlineCount", Database.Server.GamePoll.Count)
                  .Where("Name", Program.ServerConfig.ServerName); cmd.Execute();
                #endregion
                if (clock > DeathWish.Program.UpdateServerStatus)
                {
                    DeathWish.Program.UpdateServerStatus = Extensions.Time32.Now.AddSeconds(5);
                    DeathWish.Program.LastServerPulse = System.DateTime.Now;
                }
                if (System.DateTime.Now > DeathWish.Program.LastGuildPulse.AddHours(24))
                {
                    foreach (var guilds in Role.Instance.Guild.GuildPoll.Values)
                    {
                        guilds.CreateMembersRank();
                        guilds.UpdateGuildInfo();
                    }
                    DeathWish.Program.LastGuildPulse = System.DateTime.Now;
                }
                Program.GlobalItems.Work();

                #region TeamPK
                foreach (var elitegroup in Game.MsgTournaments.MsgTeamPkTournament.EliteGroups)
                    elitegroup.timerCallback();
                #endregion
                #region SkillPK
                foreach (var elitegroup in Game.MsgTournaments.MsgSkillTeamPkTournament.EliteGroups)
                    elitegroup.timerCallback();
                #endregion
                #region ElitePK
                foreach (var elitegroup in Game.MsgTournaments.MsgEliteTournament.EliteGroups)
                    elitegroup.timerCallback();
                #endregion
                #region PokerTables
                foreach (var t in Poker.Database.Tables.Values)
                    PokerHandler.PokerTablesCallback(t, 0);
                #endregion
                #region Roulettes
                foreach (var roullet in Database.Roulettes.RoulettesPoll.Values)
                    roullet.work();
                #endregion
                Game.MsgTournaments.MsgSchedules.CheckUp(clock);
                Game.MsgTournaments.MsgBroadcast.Work();
                #region Monster
                ///////////////////////////////////////////// Spook///////
                if (DateNow.Minute == 02 && DateNow.Second == 00)
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        string Messaj = "The ThrillingSpook appeared In Safe Zone [Gob Center]! Defeat it!";
                        Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage(Messaj, Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));

                        foreach (var user in Database.Server.GamePoll.Values)
                            user.Player.MessageBox(Messaj, new System.Action<Client.GameClient>(p =>
                            {
                                p.Teleport(51, 61, 1004);
                            }
                        ), null, 120);

                        var map = Database.Server.ServerMaps[1004];
                        if (!map.ContainMobID(20160))
                        {


                            Database.Server.AddMapMonster(stream, map, 20160, 49, 49, 0, 0, 1);

                        }
                    }
                }
                ///////////////////////////////////////////// SnowBanshe/////
                if ( DateNow.Minute == 20 && DateNow.Second == 0)
                {

                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();//689 711
                        string Messaj = "The SnowBanshe appeared ! Defeat it!";
                        Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage(Messaj, Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                        foreach (var user in Database.Server.GamePoll.Values)
                            user.Player.MessageBox(Messaj, new System.Action<Client.GameClient>(p =>
                            {
                                p.Teleport(140, 447, 10250);
                            }
                        ), null, 120);
                        var map = Database.Server.ServerMaps[10250];
                        if (!map.ContainMobID(20070))
                        {

                            Database.Server.AddMapMonster(stream, map, 20070, 142, 436, 0, 0, 1);

                        }
                    }
                }
                ///////////////////////////////////////////// Nemesis//////////////////
                if (DateNow.Minute == 25 && DateNow.Second == 00)
                {
                    foreach (var user in Database.Server.GamePoll.Values)
                        user.Player.MessageBox("The NemesisTyrant have spawned Go to kill them.", new System.Action<Client.GameClient>(p => { p.Teleport(1023, 740, 10250); }), null, 120);
                    var map = Database.Server.ServerMaps[10250];
                    if (!map.ContainMobID(20300))
                    {

                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            Database.Server.AddMapMonster(stream, map, 20300, 1009, 733, 0, 0, 1);


                        }
                    }
                }
                ///////////////////////////////////////////////////////////////////////////////
                if ( DateNow.Minute == 40 && DateNow.Second == 00)
                {
                    foreach (var user in Database.Server.GamePoll.Values)
                        user.Player.MessageBox("The NemesisTyrant have spawned Go to kill them.", new System.Action<Client.GameClient>(p => { p.Teleport(350, 574, 10137); }), null, 120);
                    var map = Database.Server.ServerMaps[10137];
                    if (!map.ContainMobID(20300))
                    {

                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            Database.Server.AddMapMonster(stream, map, 20300, 343, 565, 0, 0, 1);


                        }
                    }
                }
                if (DateNow.Minute == 35 && DateNow.Second == 00)
                {

                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();//689 711
                        string Messaj = "The SnowBanshe appeared ! Defeat it!";
                        Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage(Messaj, Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                        foreach (var user in Database.Server.GamePoll.Values)
                            user.Player.MessageBox(Messaj, new System.Action<Client.GameClient>(p =>
                            {
                                p.Teleport(676, 716, 10137);
                            }
                        ), null, 120);
                        var map = Database.Server.ServerMaps[10137];
                        if (!map.ContainMobID(20070))
                        {

                            Database.Server.AddMapMonster(stream, map, 20070, 670, 702, 0, 0, 1);

                        }
                    }
                }             
                #endregion
                #region Save && Restart
                //if (DateNow.Hour == 00 && DateNow.Minute == 01 && DateNow.Second == 01)
                //{
                //    Program.ConsoleCMD("save");
                //}
                //if (DateNow.Hour == 00 && DateNow.Minute == 05 && DateNow.Second == 01)
                //{
                //    Program.ConsoleCMD("restart");
                //}
                #endregion
            }
            catch (System.Exception e) { MyConsole.WriteException(e); }
        }
    }
}

namespace DeathWish.Threading
{
    /// <summary>
    /// Controller for the player thread.
    /// </summary>
    public static class Server
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
                Database.Server.Reset();
                WebServer.Proces.work();
                if (clock > DeathWish.Program.SaveServerDatabase)
                {
                    DeathWish.Program.SaveDBPayers();
                    DeathWish.Program.SaveServerDatabase = Extensions.Time32.Now.AddMinutes(1);
                }
                DeathWish.Program.LastSavePulse = System.DateTime.Now;
                if (clock > DeathWish.Program.ResetRandom)
                {
                    DeathWish.Program.GetRandom.SetSeed(System.Environment.TickCount);
                    DeathWish.Program.ResetRandom = Extensions.Time32.Now.AddMinutes(30);
                }
                #region Back UP
                if (DateNow.Hour % 6 == 00 && DateNow.Minute == 00 && DateNow.Second == 00)//BackUP
                {
                    try
                    {
                        string create = Program.ServerConfig.DbLocation + "\\AABackUP\\" + System.DateTime.Now.Year + " - " + System.DateTime.Now.Month + " - " + System.DateTime.Now.Day + " ";
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
                            if (!System.IO.Directory.Exists(create))
                            {
                                var di = System.IO.Directory.CreateDirectory(create);
                                var di2 = System.IO.Directory.CreateDirectory(createUsers);
                                var di3 = System.IO.Directory.CreateDirectory(createspells);
                                var di4 = System.IO.Directory.CreateDirectory(createprofs);
                                var di5 = System.IO.Directory.CreateDirectory(createitems);
                                var di6 = System.IO.Directory.CreateDirectory(createquests);
                                var di7 = System.IO.Directory.CreateDirectory(createhouses);
                                var di8 = System.IO.Directory.CreateDirectory(createclans);
                                var di9 = System.IO.Directory.CreateDirectory(createguilds);
                                var di0 = System.IO.Directory.CreateDirectory(createunions);
                                MyConsole.WriteLine("Folders Created at " + create + "");
                                System.IO.File.Copy(Program.ServerConfig.DbLocation + "\\JiangHu.txt", create + "\\JiangHu.txt", true);
                                System.IO.File.Copy(Program.ServerConfig.DbLocation + "\\PrestigeRanking.txt", create + "\\PrestigeRanking.txt", true);
                                System.IO.File.Copy(Program.ServerConfig.DbLocation + "\\InnerPower.txt", create + "\\InnerPower.txt", true);
                                Program.Copy(Program.ServerConfig.DbLocation + "\\Users", createUsers);
                                Program.Copy(Program.ServerConfig.DbLocation + "\\PlayersSpells", createspells);
                                Program.Copy(Program.ServerConfig.DbLocation + "\\PlayersProfs", createprofs);
                                Program.Copy(Program.ServerConfig.DbLocation + "\\PlayersItems", createitems);
                                Program.Copy(Program.ServerConfig.DbLocation + "\\Quests", createquests);
                                Program.Copy(Program.ServerConfig.DbLocation + "\\Houses", createhouses);
                                Program.Copy(Program.ServerConfig.DbLocation + "\\Clans", createclans);
                                Program.Copy(Program.ServerConfig.DbLocation + "\\Guilds", createguilds);
                                Program.Copy(Program.ServerConfig.DbLocation + "\\Unions", createunions);
                                MyConsole.WriteLine("Done BackUp Database For today ( " + System.DateTime.Now.Year + " - " + System.DateTime.Now.Month + " - " + System.DateTime.Now.Day + " ) ");
                                return;
                            }
                            else
                            {
                                MyConsole.WriteLine("" + create + " Already Backed Up !");
                                return;
                            }
                        }
                        catch (System.IO.IOException ioex)
                        {
                            System.Console.WriteLine(ioex.Message);
                        }
                    }
                    catch (System.Exception e) { System.Console.WriteLine(e.ToString()); }
                }
                #endregion
            }
            catch (System.Exception e) { MyConsole.WriteException(e); }
        }
    }
}
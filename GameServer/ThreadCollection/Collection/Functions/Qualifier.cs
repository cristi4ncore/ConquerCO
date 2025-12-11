namespace DeathWish.Threading
{
    /// <summary>
    /// Controller for the player thread.
    /// </summary>
    public static class Qualifier
    {
        /// <summary>
        /// Handles the thread.
        /// </summary>
        public static void ArenaQualifier()
        {
            try
            {
                DeathWish.Game.MsgTournaments.MsgSchedules.Arena.CheckGroups();
                DeathWish.Game.MsgTournaments.MsgSchedules.Arena.CreateMatches();
                DeathWish.Game.MsgTournaments.MsgSchedules.Arena.VerifyMatches();
            }
            catch (System.Exception e) { MyConsole.WriteException(e); }
        }
        public static void TeamArenaQualifier()
        {
            try
            {
                DeathWish.Game.MsgTournaments.MsgSchedules.TeamArena.CheckGroups();
                DeathWish.Game.MsgTournaments.MsgSchedules.TeamArena.CreateMatches();
                DeathWish.Game.MsgTournaments.MsgSchedules.TeamArena.VerifyMatches();
            }
            catch (System.Exception e) { MyConsole.WriteException(e); }
        }
    }
}
namespace DeathWish.Threading
{
	/// <summary>
	/// Controller for the player thread.
	/// </summary>
	public static class AIBotTheards
	{
        /// <summary>
        /// Handles the thread.
        /// </summary>
        public static void Handle()
        {
            DeathWish.Client.PoolProcesses.BotTheard();
        }
    }
}
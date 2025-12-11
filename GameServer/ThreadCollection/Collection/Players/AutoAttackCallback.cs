namespace DeathWish.Threading
{
	/// <summary>
	/// Controller for the player thread.
	/// </summary>
	public static class AutoAttackCallback
	{
		/// <summary>
		/// Handles the thread.
		/// </summary>
		public static void Handle(Client.GameClient player)
		{
			DeathWish.Client.PoolProcesses.AutoAttackCallback(player);
		}
	}
}
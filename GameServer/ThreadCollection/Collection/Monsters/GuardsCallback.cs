namespace DeathWish.Threading
{
	/// <summary>
	/// Controller for the player thread.
	/// </summary>
	public static class GuardsCallback
	{
		/// <summary>
		/// Handles the thread.
		/// </summary>
		public static void Handle(Client.GameClient player)
		{
			DeathWish.Game.MsgMonster.PoolProcesses.GuardsCallback(player);
		}
	}
}
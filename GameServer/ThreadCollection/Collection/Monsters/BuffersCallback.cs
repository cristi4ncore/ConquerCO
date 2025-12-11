namespace DeathWish.Threading
{
	/// <summary>
	/// Controller for the player thread.
	/// </summary>
	public static class BuffersCallback
	{
		/// <summary>
		/// Handles the thread.
		/// </summary>
		public static void Handle(Client.GameClient player)
		{
			DeathWish.Game.MsgMonster.PoolProcesses.BuffersCallback(player);
		}
	}
}
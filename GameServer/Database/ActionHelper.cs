using Extensions.Threading;
using DeathWish.Game.MsgServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeathWish.Database
{
    public static class ActionHelper
    {
#region Actions
        public static Action<ServerSockets.Packet, Client.GameClient> LvlAction = null;
#endregion
        #if NewActionHelperPOP
        public static void Create()
        {
            LvlAction = new Action<ServerSockets.Packet, Client.GameClient>(Uplvl);
        }
#endif
#region Voids
        private static void Uplvl(ServerSockets.Packet stream, Client.GameClient client)
        {
            while (client.Player.Experience >= Database.Server.LevelInfo[Database.DBLevExp.Sort.User][(byte)client.Player.Level].Experience)
            {
                client.Player.Experience -= Database.Server.LevelInfo[Database.DBLevExp.Sort.User][(byte)client.Player.Level].Experience;
                ushort newlev = (ushort)(client.Player.Level + 1);
                client.UpdateLevel(stream, newlev);
                if (client.Player.Level >= 140)
                {
                    client.Player.Experience = 0;
                    break;
                }
            }
        }
#endregion
    }
}

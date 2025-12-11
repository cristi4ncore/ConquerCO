using DeathWish.Game.MsgServer;
using DeathWish.Role;
using DeathWish.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeathWish.Database.Mining
{
    class Process
    {
        public static uint[] Ores = new uint[]
       {
            1072010, 1072011, 1072012, 1072013,
            1072054, 1072049, 1072041, 1072041,
            1072016, 1072017, 1072018, 1072047,
            1072026, 1072026, 1072025, 1072014,
            1072015, 1072025, 1072019, 1072011
       };
        public static uint[] Gems = new uint[]
        {
            700001, 700011, 700021, 700031,700071,
            700041, 700051, 700061, 700061, 700011
        };
        public static bool IsPickAxe(uint ID)
        {
            return ID == 562000;
        }
        private static ThreadSafeRandom SafeRandom = new ThreadSafeRandom();
        public static bool PercentSuccess(double _chance)
        {
            return SafeRandom.NextDouble() * 100 < _chance;
        }
        public static unsafe void Handler(Client.GameClient client)
        {
            if (!client.Player.Mining)
                return;
            if (!client.Map.TypeStatus.HasFlag(Role.MapTypeFlags.MineEnable))
            {
                client.Player.StopMining();
                return;
            }
            Game.MsgServer.MsgGameItem MiningWeapon = null;
            if (!client.Equipment.Alternante)
            {
                if (!client.Equipment.TryGetEquip(Role.Flags.ConquerItem.RightWeapon, out MiningWeapon))
                {
                    client.CreateBoxDialog("You have to wear PickAxe to start mining.");
                    client.Player.StopMining();
                    return;
                }
            }
            else
            {
                if (!client.Equipment.TryGetEquip(Role.Flags.ConquerItem.AleternanteRightWeapon, out MiningWeapon))
                {
                    client.CreateBoxDialog("You have to wear PickAxe to start mining.");
                    client.Player.StopMining();
                    return;
                }
            }
            if (MiningWeapon == null)
            {
                client.CreateBoxDialog("You have to wear PickAxe to start mining.");
                client.Player.StopMining();
                return;
            }
            if (!IsPickAxe(MiningWeapon.ITEM_ID))
            {
                client.CreateBoxDialog("You have to wear PickAxe to start mining.");
                client.Player.StopMining();
                return;
            }
            if (!client.Inventory.HaveSpace(1))
            {
                client.CreateBoxDialog("Your inventory is full. You can not mine anymore items.");
                client.Player.StopMining();
                return;
            }
            if (client.Player.MiningAttempts == 0)
            {
                client.CreateBoxDialog("Sorry, you need to get some rest come back tomorrow.");
                client.Player.StopMining();
                return;
            }
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                ActionQuery daction = new ActionQuery()
                {
                    ObjId = client.Player.UID,
                    dwParam = client.Player.MiningAttempts,
                    Type = ActionType.Mining,
                    wParam1 = 24,
                    wParam2 = 68,
                };
                client.Player.View.SendView(stream.ActionCreate(&daction), true);                                       
            }
        }
    }
}

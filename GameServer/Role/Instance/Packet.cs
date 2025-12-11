using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeathWish
{
    public enum PacketType
    {
        SEND_ALL_PROCESS = 1,
        LoginKey,
        SEND_USERNAME_PASSWORD
    }
    public class LoaderPacket
    {
        public PacketType ID;

        public string Name;
    }
}

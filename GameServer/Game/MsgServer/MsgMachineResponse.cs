using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace DeathWish.Game.MsgServer
{
    public unsafe partial class MsgBuilder
    {
        public static unsafe ServerSockets.Packet MachineResponseCreate(this ServerSockets.Packet stream, MsgMachine.SlotMachineSubType Mode, byte WheelOne, byte WheelTwo, byte WheelThree, uint NpcID)
        {
            stream.InitWriter();
            stream.Write((byte)Mode);
            stream.Write(WheelOne);
            stream.Write(WheelTwo);
            stream.Write(WheelThree);
            stream.Write((ulong)0);
            stream.Write(NpcID);
            stream.Write((uint)0);
            stream.Finalize(GamePackets.MsgMachineResponse);
            return stream;
        }
    }
}
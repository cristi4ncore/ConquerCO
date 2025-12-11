using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeathWish.Game.MsgServer
{
    public class GameCharacterUpdates
    {
            public static void WriteUInt16(ushort arg, int offset, byte[] buffer)
        {
            if (buffer == null)
                return;
            if (offset > buffer.Length - 1)
                return;
            if (buffer.Length >= offset + sizeof(ushort))
            {
                unsafe
                {
#if UNSAFE
                    fixed (byte* Buffer = buffer)
                        *((ushort*)(Buffer + offset)) = arg;               
#else
                    buffer[offset] = (byte)arg;
                    buffer[offset + 1] = (byte)(arg >> 8);
#endif
                }
            }
        }
        public static void WriteUInt32(uint arg, int offset, byte[] buffer)
        {
            if (buffer == null)
                return;
            if (offset > buffer.Length - 1)
                return;
            if (buffer.Length >= offset + sizeof(uint))
            {
                unsafe
                {
#if UNSAFE
                    fixed (byte* Buffer = buffer)
                        *((uint*)(Buffer + offset)) = arg;
#else
                    buffer[offset] = (byte)arg;
                    buffer[offset + 1] = (byte)(arg >> 8);
                    buffer[offset + 2] = (byte)(arg >> 16);
                    buffer[offset + 3] = (byte)(arg >> 24);
#endif
                }
            }
        }
        public const uint
      Activated = 1 << 8,
      Deactivated = 1;
        public const uint
            Accelerated = 52,
            Decelerated = 53,
            Flustered = 54,
            Sprint = 55,
            DivineShield = 57,
            Stun = 58,
            Freeze = 59,
            Dizzy = 60,
            AzureShield = 93,
            SoulShacle = 111;
        byte[] Buffer;
        const byte minBufferSize = 16;
        public GameCharacterUpdates(bool Create)
        {
            if (Create)
            {
                Buffer = new byte[minBufferSize + 8];
                WriteUInt16(minBufferSize, 0, Buffer);
                WriteUInt16(2075, 2, Buffer);
            }
        }
        public uint UID
        {
            get { return BitConverter.ToUInt32(Buffer, 4); }
            set { WriteUInt32(value, 4, Buffer); }
        }
    }
    public unsafe static partial class MsgBuilder
    {
        public static unsafe ServerSockets.Packet GameUpdateCreate(this ServerSockets.Packet stream, uint UID
            , MsgGameUpdate.DataType ID, bool active, uint shownamount, uint time, uint amount = 0)
        {
            stream.InitWriter();


            stream.Write(UID);
            stream.Write((uint)1);//count
            stream.Write((uint)ID);
            stream.Write((uint)(active ? (uint)(1 << 8) : 1));
            stream.Write(shownamount);
            stream.Write(time);
          if(ID == MsgGameUpdate.DataType.Decelerated)
              stream.Write(uint.MaxValue - amount);
            else
            stream.Write(amount);



            stream.Finalize(GamePackets.GameUpdate);
            return stream;
        }
    }
    public unsafe class MsgGameUpdate
    {
        public enum DataType : uint
        {
            Accelerated = 52,
            Decelerated = 53,
            Flustered = 54,
            Sprint = 55,
            DivineShield = 57,
            Stun = 58,
            Freeze = 59,
            Dizzy = 60,
            AzureShield = 93,
            SoulShacle = 111
        }
    }
}

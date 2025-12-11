



using System;
using System.Text;
using System.Collections.Generic;

namespace DeathWish
{
    public unsafe class Writer
    {
       
        public static void WriteByte(byte arg, int offset, byte[] buffer)
        {
            if (buffer == null) return;
            if (offset > buffer.Length - 1) return;
            buffer[offset] = arg;
        }
        public static void WriteBoolean(bool arg, int offset, byte[] buffer)
        {
            if (buffer == null) return;
            if (offset > buffer.Length - 1) return;
            WriteByte(arg == true ? (byte)1 : (byte)0, offset, buffer);
        }
        public static void WriteUInt16(ushort arg, int offset, byte[] buffer)
        {
            if (buffer == null) return;
            if (offset > buffer.Length - 1) return;
            if (buffer.Length >= offset + sizeof(ushort))
            {
                unsafe
                {
                    fixed (byte* Buffer = buffer)
                        *((ushort*)(Buffer + offset)) = arg;
                }
            }
        }
        public static void WriteUInt32(uint arg, int offset, byte[] buffer)
        {
            if (buffer == null) return;
            if (offset > buffer.Length - 1) return;
            if (buffer.Length >= offset + sizeof(uint))
            {
                unsafe
                {
                    fixed (byte* Buffer = buffer)
                        *((uint*)(Buffer + offset)) = arg;
                }
            }
        }
        public static void WriteInt32(int arg, int offset, byte[] buffer)
        {
            if (buffer == null) return;
            if (offset > buffer.Length - 1) return;
            if (buffer.Length >= offset + sizeof(uint))
            {
                unsafe
                {
                    fixed (byte* Buffer = buffer)
                        *((int*)(Buffer + offset)) = arg;
                }
            }
        }
        public static void WriteUInt64(ulong arg, int offset, byte[] buffer)
        {
            if (buffer == null) return;
            if (offset > buffer.Length - 1) return;
            if (buffer.Length >= offset + sizeof(ulong))
            {
                unsafe
                {
                    fixed (byte* Buffer = buffer)
                        *((ulong*)(Buffer + offset)) = arg;
                }
            }
        }
      
        public static void Byte(byte arg, int offset, byte[] buffer)
        {
            if (buffer == null) return;
            if (offset > buffer.Length - 1) return;
            buffer[offset] = arg;
        }
        public static void Boolean(bool arg, int offset, byte[] buffer)
        {
            if (buffer == null) return;
            if (offset > buffer.Length - 1) return;
            WriteByte(arg == true ? (byte)1 : (byte)0, offset, buffer);
        }
        public static void Ushort(ushort arg, int offset, byte[] buffer)
        {
            if (buffer == null) return;
            if (offset > buffer.Length - 1) return;
            if (buffer.Length >= offset + sizeof(ushort))
            {
                unsafe
                {
                    fixed (byte* Buffer = buffer)
                    {
                        *((ushort*)(Buffer + offset)) = arg;
                    }
                }
            }
        }
        public static void Uint(uint arg, int offset, byte[] buffer)
        {
            if (buffer == null) return;
            if (offset > buffer.Length - 1) return;
            if (buffer.Length >= offset + sizeof(uint))
            {
                unsafe
                {
                    fixed (byte* Buffer = buffer)
                    {
                        *((uint*)(Buffer + offset)) = arg;
                    }
                }
            }
        }
        public static unsafe void Decimal(decimal arg, int offset, byte[] Buffer)
        {
            try
            {
                fixed (byte* buffer = Buffer)
                {
                    if (arg.GetType() == typeof(decimal))
                    {
                        *((decimal*)(buffer + offset)) = arg;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        public static void Int(int arg, int offset, byte[] buffer)
        {
            if (buffer == null) return;
            if (offset > buffer.Length - 1) return;
            if (buffer.Length >= offset + sizeof(uint))
            {
                unsafe
                {
                    fixed (byte* Buffer = buffer)
                    {
                        *((int*)(Buffer + offset)) = arg;
                    }
                }
            }
        }
        public static void Ulong(ulong arg, int offset, byte[] buffer)
        {
            if (buffer == null) return;
            if (offset > buffer.Length - 1) return;
            if (buffer.Length >= offset + sizeof(ulong))
            {
                unsafe
                {
                    fixed (byte* Buffer = buffer)
                    {
                        *((ulong*)(Buffer + offset)) = arg;
                    }
                }
            }
        }
        public static void WriteUshort(ushort arg, int offset, byte[] buffer)
        {
            if (buffer == null) return;
            if (offset > buffer.Length - 1) return;
            if (buffer.Length >= offset + sizeof(ushort))
            {
                unsafe
                {
                    fixed (byte* Buffer = buffer)
                    {
                        *((ushort*)(Buffer + offset)) = arg;
                    }
                }
            }
        }
        public static void WriteUint(uint arg, int offset, byte[] buffer)
        {
            if (buffer == null) return;
            if (offset > buffer.Length - 1) return;
            if (buffer.Length >= offset + sizeof(uint))
            {
                unsafe
                {
                    fixed (byte* Buffer = buffer)
                    {
                        *((uint*)(Buffer + offset)) = arg;
                    }
                }
            }
        }
        public static void WriteUlong(ulong arg, int offset, byte[] buffer)
        {
            if (buffer == null) return;
            if (offset > buffer.Length - 1) return;
            if (buffer.Length >= offset + sizeof(ulong))
            {
                unsafe
                {
                    fixed (byte* Buffer = buffer)
                    {
                        *((ulong*)(Buffer + offset)) = arg;
                    }
                }
            }
        }
       
    }
}
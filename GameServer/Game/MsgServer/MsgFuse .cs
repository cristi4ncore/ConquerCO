using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeathWish.Game.MsgServer
{
    public static unsafe partial class MsgBuilder
    {
        //offset 14 => CountItem
        //offset 16 + 4 => UID Items
        public static unsafe void GetFuse(this ServerSockets.Packet stream, out List<uint> items)
        {
            stream.SeekForward(10);
            ushort Count = stream.ReadUInt16();//14
            items = new List<uint>();//16
            for (byte x = 0; x < Count; x++)//16
                items.Add(stream.ReadUInt32());//16 //20 //24 //28
        }
    }
    public unsafe class MsgFuse
    {
        [Packet(GamePackets.MsgFuse)]
        private unsafe static void Process(Client.GameClient client, ServerSockets.Packet stream)
        {
            //byte[] buffer = new byte[stream.Size];
            //fixed (byte* ptr = buffer)
            //{
            //    stream.memcpy(ptr, stream.Memory, stream.Size);
            //}
            //stream.PrintPacket(buffer);
            List<uint> ItemsUIDS;
            stream.GetFuse(out ItemsUIDS);
            for (int x = 0; x < ItemsUIDS.Count; x++)
            {
                MsgGameItem itemuse;
                if (client.Inventory.ClientItems.TryGetValue(ItemsUIDS[x], out itemuse))
                {
                    if (CheckItems(itemuse))
                    {
                        var Item = GetPoints(itemuse);
                        if (Item[0] == 1)
                        {
                            client.Player.Money += Item[1];
                        }
                        else if (Item[0] == 2)
                        {
                            client.Player.ConquerPoints += Item[1];
                        }
                        else if (Item[0] == 3)
                        {
                            client.Player.BoundConquerPoints += (int)Item[1];
                        }
                        client.Inventory.Update(itemuse, Role.Instance.AddMode.REMOVE, stream);
                    }
                    else
                    {
                        client.CreateBoxDialog("This Item Can't Use it Only DB Which Can Craft");
                    }
                }
            }
        }
        public static bool CheckItems(MsgGameItem Item)
        {
            switch (Item.ITEM_ID)
            {
                case 1088000:
                    {
                        return true;
                    }
                default:
                    {
                        return false;
                    }
            }
        }
        public static uint[] GetPoints(MsgGameItem Item)
        {
            //TypeValue 1 => Money
            //TypeValue 2 => Cps
            //TypeValue 3 => Coins
            uint TypeValue = 0;
            uint Value = 0;
            switch (Item.ITEM_ID)
            {
                case 1088000:
                    {
                        TypeValue = 2;
                        Value = 215;
                        break;
                    }            
            }
            uint[] matrix = new uint[2];
            matrix[0] = TypeValue;
            matrix[1] = Value;
            return matrix;
        }
    }
}
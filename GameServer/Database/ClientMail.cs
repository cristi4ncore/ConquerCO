using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;

namespace DeathWish.Database
{
    public class ClientMail
    {
        public struct DBPrize
        {
            public unsafe fixed sbyte _Sender[32];
            public unsafe fixed sbyte _Subject[32];
            public unsafe fixed sbyte _Message[64];
            public unsafe string Subject
            {
                get { fixed (sbyte* bp = _Subject) { return new string(bp); } }
                set
                {
                    string text = value;
                    fixed (sbyte* bp = _Subject)
                    {
                        for (int i = 0; i < text.Length; i++)
                            bp[i] = (sbyte)text[i];
                    }
                }
            }
            public unsafe string Sender
            {
                get { fixed (sbyte* bp = _Sender) { return new string(bp); } }
                set
                {
                    string text = value;
                    fixed (sbyte* bp = _Sender)
                    {
                        for (int i = 0; i < text.Length; i++)
                            bp[i] = (sbyte)text[i];
                    }
                }
            }
            public unsafe string Message
            {
                get { fixed (sbyte* bp = _Message) { return new string(bp); } }
                set
                {
                    string text = value;
                    fixed (sbyte* bp = _Message)
                    {
                        for (int i = 0; i < text.Length; i++)
                            bp[i] = (sbyte)text[i];
                    }
                }
            }
            public uint ID;
            public uint cpsprize;
            public uint Attackment;
            public ulong goldprize;
            public ulong joyBeans;
            public ClientItems.DBItem Item;
            public long SentOn;

            public DBPrize GetDBPrize(DeathWish.Game.MsgServer.PrizeInfo prize)
            {
                ID = prize.ID;
                Subject = prize.Subject;
                Sender = prize.Sender;
                Message = prize.Message;
                cpsprize = prize.cpsprize;
                Attackment = prize.Attackment;
                goldprize = prize.goldprize;
                joyBeans = prize.JoyBeans;
                if (prize.Item != null)
                {
                    ClientItems.DBItem sItem = new ClientItems.DBItem();
                    Item = sItem.GetDBItem(prize.Item);
                }
                else
                {
                    Item = new ClientItems.DBItem();
                }
                SentOn = prize.SentOn.ToBinary();
                return this;
            }

            public DeathWish.Game.MsgServer.PrizeInfo GetClientPrize()
            {
                DeathWish.Game.MsgServer.PrizeInfo Prize = new Game.MsgServer.PrizeInfo();
                Prize.ID = ID;
                Prize.Subject = Subject;
                Prize.Sender = Sender;
                Prize.Message = Message;
                Prize.cpsprize = cpsprize;
                Prize.Attackment = Attackment;
                Prize.goldprize = goldprize;
                Prize.JoyBeans = joyBeans;
                Prize.Item = Item.UID == 0 ? null : Item.GetDataItem();
                Prize.SentOn = DateTime.FromBinary(SentOn);
                return Prize;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using DeathWish.Game.MsgFloorItem;

namespace DeathWish.Role.Instance
{
    public class Confiscator
    {
        public static Extensions.Counter CounterUID = new Extensions.Counter(1);

        public ConcurrentDictionary<uint, Game.MsgServer.MsgDetainedItem> RedeemContainer;
        public ConcurrentDictionary<uint, Game.MsgServer.MsgDetainedItem> ClaimContainer;

        public Confiscator()
        {

            RedeemContainer = new ConcurrentDictionary<uint, Game.MsgServer.MsgDetainedItem>();
            ClaimContainer = new ConcurrentDictionary<uint, Game.MsgServer.MsgDetainedItem>();
        }

        public bool IsEmptyReadem() { return RedeemContainer.IsEmpty; }
        public bool IsEmptyClaim() { return ClaimContainer.IsEmpty; }


        internal unsafe void AddItem(Client.GameClient Owner, Client.GameClient Gainer, Game.MsgServer.MsgGameItem GameItem, ServerSockets.Packet stream)
        {
            Game.MsgServer.MsgDetainedItem DetainedItem = Game.MsgServer.MsgDetainedItem.Create(GameItem);
            DetainedItem.UID = CounterUID.Next;
            DetainedItem.OwnerUID = Owner.Player.UID;
            DetainedItem.OwnerName = Owner.Player.Name;
            DetainedItem.GainerUID = Gainer.Player.UID;
            DetainedItem.GainerName = Gainer.Player.Name;
            DetainedItem.Refinary = Game.MsgServer.MsgItemExtra.Refinery.ShallowCopy(GameItem.Refinary);
            DetainedItem.Purification = Game.MsgServer.MsgItemExtra.Purification.ShallowCopy(GameItem.Purification);
            DetainedItem.ConquerPointsCost = CalculateCpsCost(DetainedItem);

            DateTime Timer = DateTime.Now;
            DetainedItem.Date = CreateTimer(Timer.Year, Timer.Month, Timer.Day);

            DetainedItem.Action = Game.MsgServer.MsgDetainedItem.ContainerType.DetainPage;
            DetainedItem.Send(Owner, stream);
            DetainedItem.Action = Game.MsgServer.MsgDetainedItem.ContainerType.ClaimPage;
            DetainedItem.Send(Gainer, stream);

            Owner.Confiscator.RedeemContainer.TryAdd(DetainedItem.UID, DetainedItem);
            Gainer.Confiscator.ClaimContainer.TryAdd(DetainedItem.UID, DetainedItem);


            //add Drop Effect
            ushort x = Owner.Player.X;
            ushort y = Owner.Player.Y;
            if (Owner.Map.AddGroundItem(ref x, ref y, 2))
            {
                Game.MsgFloorItem.MsgItemPacket DropEffect = Game.MsgFloorItem.MsgItemPacket.Create();
                DropEffect.DropType = Game.MsgFloorItem.MsgDropID.DropDetain;
                DropEffect.m_Color = (byte)GameItem.Color;
                DropEffect.m_ID = GameItem.ITEM_ID;
                if (GameItem.Purification.PurificationItemID != 0)
                    DropEffect.m_ID = GameItem.Purification.PurificationItemID;
                DropEffect.m_UID = GameItem.UID;
                DropEffect.m_X = x;
                DropEffect.m_Y = y;

                Owner.Player.View.SendView(stream.ItemPacketCreate(DropEffect), true);
            }
            //-------------------

            Database.Server.QueueContainer.QueueObj(Owner.Player.UID, Owner.Confiscator);
            Database.Server.QueueContainer.QueueObj(Gainer.Player.UID, Gainer.Confiscator);
        }
        internal static int CreateTimer(int year, int month, int day)
        {
            int Timer = year * 10000 + month * 100 + day;
            return Timer;
        }
        internal static DateTime GetTimer(int Timer)
        {
            int Year = Timer / 10000;
            int Month = (Timer / 100) - Year * 100;
            int Day = Timer - (Year * 10000) - (Month * 100);
            return new DateTime(Year, Month, Day);
        }
        internal static int CalculateCpsCost(Game.MsgServer.MsgDetainedItem Item)
        {
            const byte Multiplier = 1;

            //Oficial conquer calculation --------------------------
            int amount = 10;
            if (Item.ItemID % 10 == 9)
                amount += 40;
            switch (Item.Plus)
            {
                case 0: break;
                case 1: amount += 100; break;
                case 2: amount += 200; break;
                case 3: amount += 300; break;
                case 4: amount += 400; break;
                case 5: amount += 500; break;
                case 6: amount += 600; break;
                case 7: amount += 700; break;
                case 8: amount += 800; break;
                default:
                    amount += 1000; break;

            }
            switch (Database.ItemType.ItemPosition(Item.ItemID))
            {
                case (ushort)Role.Flags.ConquerItem.LeftWeapon:
                case (ushort)Role.Flags.ConquerItem.RightWeapon:
                case (ushort)Role.Flags.ConquerItem.AleternanteLeftWeapon:
                case (ushort)Role.Flags.ConquerItem.AleternanteRightWeapon:
                    {
                        if (Item.SocketOne != Flags.Gem.NoSocket && Item.SocketTwo != Flags.Gem.NoSocket)
                            amount += 500;
                        else if (Item.SocketOne != Flags.Gem.NoSocket)
                            amount += 1000;
                        break;
                    }
                default:
                    {
                        if (Item.SocketOne != Flags.Gem.NoSocket && Item.SocketTwo != Flags.Gem.NoSocket)
                            amount += 200;
                        else if (Item.SocketOne != Flags.Gem.NoSocket)
                            amount += 1000;
                        break;
                    }
            }
            switch (Item.Refinary.EffectLevel)
            {
                case 1: amount += 400; break;
                case 2: amount += 500; break;
                case 3: amount += 600; break;
                case 4: amount += 700; break;
                case 5: amount += 800; break;
                case 6: amount += 900; break;
                case 7: amount += 1000; break;
            }
            switch (Item.Purification.PurificationLevel)
            {
                case 1: amount += 1000; break;
                case 2: amount += 2000; break;
                case 3: amount += 3000; break;
                case 4: amount += 4000; break;
                case 5: amount += 5000; break;
                case 6: amount += 8000; break;
                case 7: amount += 10000; break;//p7
            }
            //--------------------------------

            return amount * Multiplier;
        }
    }
}

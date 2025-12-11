using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeathWish.Game.MsgServer
{
    public static unsafe partial class MsgBuilder
    {
        public static unsafe ServerSockets.Packet ReatreatChiInfoCreate(this ServerSockets.Packet stream, uint Count)
        {
            stream.InitWriter();
            stream.Write(Count);
            return stream;
        }
        public static unsafe ServerSockets.Packet AddRetreatedChiItem(this ServerSockets.Packet stream, Game.MsgServer.MsgChiInfo.ChiPowerType Type, Tuple<DeathWish.Role.Instance.Chi.ChiAttributeType, int>[] Gate, long Time)
        {
            int[] Powers = new int[Gate.Length];
            for (int x = 0; x < Gate.Length; x++)
                Powers[x] = Gate[x].Item2;
            stream.Write((byte)Type);
            var now = DateTime.FromBinary(Time);
            uint secs = (uint)(now.Year % 100 * 100000000 + (now.Month) * 1000000 + now.Day * 10000 + now.Hour * 100 + now.Minute);
            stream.Write(secs);
            if (Powers != null)
            {
                for (int x = 0; x < Powers.Length; x++)
                {
                    stream.Write(Powers[x]);
                }
            }
            else
                stream.ZeroFill(4 * sizeof(int));
            return stream;
        }
        public static unsafe ServerSockets.Packet CreateRetreatedChiItems(this ServerSockets.Packet stream, int Count)
        {
            stream.InitWriter();
            stream.Write((uint)Count);
            return stream;
        }
        public static unsafe ServerSockets.Packet ChiRetreatInfoFinalize(this ServerSockets.Packet stream)
        {
            stream.Finalize(GamePackets.MsgTrainingVitalityProtectInfo);
            return stream;
        }
    }
    public unsafe struct MsgTrainingVitalityProtectInfo
    {
        public static void SendInfo(Client.GameClient client)
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                int Count = 0;
                if (client.Player.MyChi.DragonTime != 0)
                    Count++;
                if (client.Player.MyChi.PhoenixTime != 0)
                    Count++;
                if (client.Player.MyChi.TigerTime != 0)
                    Count++;
                if (client.Player.MyChi.TurtleTime != 0)
                    Count++;
                stream.CreateRetreatedChiItems(Count);
                if (client.Player.MyChi.DragonTime != 0)
                {
                    stream.AddRetreatedChiItem(Game.MsgServer.MsgChiInfo.ChiPowerType.Dragon, DeathWish.Role.Instance.Retreating._ToTuple(client.Player.MyChi.DragonPower), client.Player.MyChi.DragonTime);
                }
                if (client.Player.MyChi.PhoenixTime != 0)
                {
                    stream.AddRetreatedChiItem(Game.MsgServer.MsgChiInfo.ChiPowerType.Phoenix, DeathWish.Role.Instance.Retreating._ToTuple(client.Player.MyChi.PhoenixPower), client.Player.MyChi.PhoenixTime);
                }
                if (client.Player.MyChi.TigerTime != 0)
                {
                    stream.AddRetreatedChiItem(Game.MsgServer.MsgChiInfo.ChiPowerType.Tiger, DeathWish.Role.Instance.Retreating._ToTuple(client.Player.MyChi.TigerPower), client.Player.MyChi.TigerTime);
                }
                if (client.Player.MyChi.TurtleTime != 0)
                {
                    stream.AddRetreatedChiItem(Game.MsgServer.MsgChiInfo.ChiPowerType.Turtle, DeathWish.Role.Instance.Retreating._ToTuple(client.Player.MyChi.TurtlePower), client.Player.MyChi.TurtleTime);
                }
                client.Send(stream.ChiRetreatInfoFinalize());
            }
        }
    }
}

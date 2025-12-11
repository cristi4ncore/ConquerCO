using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeathWish.MsgInterServer.Packets
{
    public static class RoleInfo
    {
        public static unsafe ServerSockets.Packet InterServerRoleInfoCreate(this ServerSockets.Packet stream, Role.Player user)
        {
            stream.InitWriter();
            stream.Write((ulong)user.RechargeProgress);
            if (user.Nobility != null)
                stream.Write(user.Nobility.Donation);
            else
                stream.ZeroFill(8);
            stream.Write(user.ChampionPoints);
            stream.Write(user.Money);
            stream.Write(user.Owner.HonorPoints);
            stream.Write(user.FirstRebornLevel);
            stream.Write(user.SecoundeRebornLevel);
            stream.Write(user.SecoundeClass);
            stream.Write((byte)(user.Reincarnation ? 1 : 0));
            stream.Write(user.QuizPoints);
          //  stream.Write(user.WHMoney);
            stream.Write(user.TournamentKills);
            stream.Write(user.InventorySashCount);
            stream.Write(user.MyFootBallPoints);
            stream.Write(user.Owner.BanCount);
            stream.Write(user.Owner.BotJailCount);
           // stream.Write(user.ExtraAtributes);
           // stream.Write(user.VipLevel);
           // stream.Write(user.VipPointsD);
          //  stream.Write(user.OnlineMinutes);
           // stream.Write(user.DonatePoints);
           // stream.Write(user.PIKAPoint);
           // stream.Write(user.Atributes);
           // stream.Write(user.Agility);
         //   stream.Write(user.Strength);
       //    stream.Write(user.Vitality);
           // stream.Write(user.Spirit);
         //   stream.Write(user.SharePoints);
         //   stream.Write(user.SoulPoint);


            stream.Finalize(PacketTypes.InterServer_RoleInfo);
            return stream;
        }
        public static unsafe void GetInterServerRoleInfo(this ServerSockets.Packet stream, Role.Player user)
        {
            user.RechargeProgress = (Role.Player.RechargeType)stream.ReadUInt64();
            user.Nobility.Donation = stream.ReadUInt64();
            user.AddChampionPoints(stream.ReadUInt32(), false);
            user.RacePoints = stream.ReadUInt32();
            user.Owner.HonorPoints = stream.ReadUInt32();
            user.FirstRebornLevel = stream.ReadUInt8();
            user.SecoundeRebornLevel = stream.ReadUInt8();
            user.SecoundeClass = stream.ReadUInt8();
            user.Reincarnation = stream.ReadUInt8() == 1 ? true : false;
            user.QuizPoints = stream.ReadUInt32();
          //  user.WHMoney = stream.ReadInt64();
            user.TournamentKills = stream.ReadUInt32();
            user.InventorySashCount = stream.ReadUInt16();
            user.MyFootBallPoints = stream.ReadUInt32();
            user.Owner.BanCount = stream.ReadUInt8();
            user.Owner.BotJailCount = stream.ReadUInt8();
           // user.ExtraAtributes = stream.ReadUInt16();
           // user.VipLevel = stream.ReadUInt8();
            //user.VipPointsD = stream.ReadUInt32();
            //user.OnlineMinutes = stream.ReadUInt32();
            //user.DonatePoints = stream.ReadUInt32();
            //user.PIKAPoint = stream.ReadUInt32();
            //user.Atributes = stream.ReadUInt16();
            //user.Agility = stream.ReadUInt16();
            //user.Strength = stream.ReadUInt16();
            //user.Vitality = stream.ReadUInt16();
            //user.Spirit = stream.ReadUInt16();
            //user.SharePoints = stream.ReadUInt32();
            //user.SoulPoint = stream.ReadUInt32();

        }
    }
}
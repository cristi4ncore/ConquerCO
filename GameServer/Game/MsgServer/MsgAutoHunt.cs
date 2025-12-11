using System;

namespace DeathWish.Game.MsgServer
{
    public static class MsgAutoHunt
    {
        [Flags]
        public enum Mode : byte
        {
            Icon = 0,
            Start = 1,
            EndAuto = 2,
            EXPGained = 3,
            SuddenlyGain = 4,
            FirstCreditKilledBy = 5,
            KilledBy = 6,
            ChangedMap = 7
        }
        public static unsafe ServerSockets.Packet AutoHuntCreate(this ServerSockets.Packet stream, ushort type, ulong Icon, ulong Exp = 0, string KillerName = null)
        {
            stream.InitWriter();
            stream.Write(type);
            stream.Write(Icon);
            stream.Write(Exp);
            stream.Write(KillerName);
            stream.Finalize(GamePackets.AutoHunt);
            return stream;
        }
        public static unsafe void GetAutoHuntOperation(this ServerSockets.Packet stream, out ushort Act)
        {
            Act = stream.ReadUInt16();
        }
        [Packet(GamePackets.AutoHunt)]
        private static unsafe void Process(Client.GameClient user, ServerSockets.Packet stream)
        {
            if (user.Player.Map == 59106 || user.Player.Map == 1700 || user.Player.Map == 26391 || user.Player.Map == 26392 || user.Player.Map == 26393 || user.Player.Map == 26394
                || user.Player.Map == 26395 || user.Player.Map == 10137 || user.Player.Map == 3359 || user.Player.Map == 3030 || user.Player.Map == 3032)
            {
                user.SendSysMesage("Auto hunt is not available on this map.");
                return;
            }
            ushort Action;
            stream.GetAutoHuntOperation(out Action);
            switch ((Mode)Action)
            {
                case Mode.Start:
                    {
                        if (user.Player.VipLevel > 0)
                        {                          
                            if (user.Player.OnXPSkill() != MsgUpdate.Flags.Normal)
                                user.Player.RemoveFlag(user.Player.OnXPSkill());
                            user.Send(stream.AutoHuntCreate(0, 341));
                            user.Send(stream.AutoHuntCreate(1, 341));
                            user.Player.OnAutoHunt = true;
                            user.Player.AutoHuntExp = 0;
                        }
                        break;
                    }
                case Mode.EndAuto:
                    {
                        if (user.Player.AutoHuntExp > 0)
                        {
                            user.Send(stream.AutoHuntCreate(3, 0, user.Player.AutoHuntExp));
                            user.IncreaseAutoExperience(stream, user.Player.AutoHuntExp);
                        }
                        user.Send(stream.AutoHuntCreate(2, 0, user.Player.AutoHuntExp));
                        user.Player.OnAutoHunt = false;
                        user.Player.AutoHuntExp = 0;
                        break;
                    }
                case Mode.EXPGained:
                    {
                        if (user.Player.AutoHuntExp > 0)
                        {
                            user.IncreaseAutoExperience(stream, user.Player.AutoHuntExp);
                        }
                        user.Send(stream.AutoHuntCreate(3, 0, user.Player.AutoHuntExp));
                        user.Player.AutoHuntExp = 0;
                        break;
                    }
                default: MyConsole.WriteLine("[AutoHunt] Unknown Action: " + Action + ""); break;
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeathWish.Game.MsgServer
{
    public static unsafe partial class MsgBuilder
    {
        public static unsafe ServerSockets.Packet TeamElitePKMatchUICreate(this ServerSockets.Packet stream, ushort ID, MsgTeamElitePKMatchUI.State state
            , MsgTeamElitePKMatchUI.EffectTyp effect, uint OpponentUID, string OpponentName, uint TimeLeft)
        {
            stream.InitWriter();

            stream.Write((uint)state);
            stream.Write((uint)effect);
            if (ID == 2230)
                stream.Write(0); //server id
            stream.Write(OpponentUID);
            stream.Write(OpponentName.Substring(0, Math.Min(OpponentName.Length, 28)), 28);
            stream.Write(TimeLeft);
            stream.Write((uint)0);
            stream.Finalize(ID);
            return stream;
        }
    }
    public unsafe struct MsgTeamElitePKMatchUI
    {
        public enum State : uint
        {
            Information = 7,
            BeginMatch = 2,
            Effect = 3,
            EndMatch = 4,
            Reward = 8
        }
        public enum EffectTyp : uint
        {
            Effect_Win = 1,
            Effect_Lose = 0
        }
    }
}

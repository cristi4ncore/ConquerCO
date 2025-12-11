using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeathWish.Game.MsgServer
{
    public static unsafe partial class MsgBuilder
    {
        public static unsafe ServerSockets.Packet SkillElitePKMatchUICreate(this ServerSockets.Packet stream, ushort ID, SkillElitePKMatchUI.State state
            , SkillElitePKMatchUI.EffectTyp effect, uint OpponentUID, string OpponentName, uint TimeLeft)
        {
            stream.InitWriter();

            stream.Write((uint)state);
            stream.Write((uint)effect);
            stream.Write(OpponentUID);
            stream.Write(OpponentName.Substring(0, Math.Min(OpponentName.Length, 28)), 28);
            stream.Write(TimeLeft);
            stream.Finalize(2250);

            return stream;
        }
    }
    public unsafe struct SkillElitePKMatchUI
    {
        public enum State : uint
        {
            BeginMatch = 2,
            Effect = 3,
            EndMatch = 4,
            Information = 7,
            Reward = 8
        }
        public enum EffectTyp : uint
        {
            Effect_Win = 1,
            Effect_Lose = 0
        }
    }
}

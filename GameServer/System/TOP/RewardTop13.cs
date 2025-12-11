using DeathWish.Game.MsgNpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeathWish.Game.MsgTournaments
{
    public class RewardTops13
    {
        public const int Map = 1988, RewardConquerPoints = 10000, FinishMinutes = 3, EndSignTime = 3;

        public static ProcesType Mode;
        public static DateTime FinishTimer = new DateTime();
        public static DateTime EndSignTimer = new DateTime();
        public static bool canjoinwar = false;
        public static bool GotPrize = false;
        public static bool TeamWon = false;
        
        public static bool AllowJoin()
        {
            return Mode == ProcesType.Idle && EndSignTimer > DateTime.Now;
        }
        public static bool TheLastPlayer()
        {

            return Database.Server.GamePoll.Values.Where(p => p.Player.Map == 1988 && p.Player.Alive).Count() == 1;
        }
     
    }
}

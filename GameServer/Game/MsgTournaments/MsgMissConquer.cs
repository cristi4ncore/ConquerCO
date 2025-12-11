using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeathWish.Game.MsgTournaments
{
    class MsgMissConquer
    {
        public const int
        Hour = 21,
        Minute = 30,
        toMinute = 35,
        EndSignTime = 34;
        private DateTime EndSignTimer = new DateTime();
        public void Open()
        {

            if (Mode == ProcesType.Dead)
            {
                Mode = ProcesType.Idle;
                EndSignTimer = DateTime.Now.AddMinutes(EndSignTime);
            }
        }
        public bool Started()
        {
            return Mode == ProcesType.Idle;
        }
        public bool AllowJoin()
        {
            return Mode == ProcesType.Idle && EndSignTimer > DateTime.Now;
        }
        private ProcesType Mode;
        public bool IsFinished() { return Mode == ProcesType.Dead; }
    }
}
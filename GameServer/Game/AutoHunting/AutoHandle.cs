using DeathWish.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeathWish.AutoHunting
{
    public class AutoHandle : Base
    {
        public AutoHandle(GameClient obj) : base(obj)
        {
            Enable = false;
            DirectionChange = 0;
            NextDirection = 0;
            AttackStamp = DateTime.Now;
        }
        public override void Start()
        {
            base.Start();
        }
        public override void End()
        {
            base.End();
        }
    }
}

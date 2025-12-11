using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeathWish.Game.MsgServer.AttackHandler.CheckAttack
{
  public  class BlockRefect
    {
      public static bool CanUseReflect(Client.GameClient user)
      {
          if (Program.BlockReflect.Contains(user.Player.Map)|| (Game.AISystem.UnlimitedArenaRooms.Maps.ContainsValue(user.Player.DynamicID) && user.Player.fbss == 1))
          {
                      return false;
          }
          return true;

         
      }
    }
}

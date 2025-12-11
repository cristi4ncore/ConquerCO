using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeathWish.Game.MsgServer.AttackHandler
{
   public class Pounce
    {
       public unsafe static void Execute(Client.GameClient user, InteractQuery Attack, ServerSockets.Packet stream, Dictionary<ushort, Database.MagicType.Magic> DBSpells)
       {
           Database.MagicType.Magic DBSpell;
           MsgSpell ClientSpell;
           if (CheckAttack.CanUseSpell.Verified(Attack, user, DBSpells, out ClientSpell, out DBSpell))
           {
               if (!Database.ItemType.IsShield(user.Player.LeftWeaponId))
                   return;
               if (user.Player.Map == 1 || user.Player.Map == 2 || user.Player.Map == 3 || user.Player.Map == 4
                   || user.Player.Map == 5 || user.Player.Map == 6 || user.Player.Map == 7 || user.Player.Map == 8
                   || user.Player.Map == 9 || user.Player.Map == 10 || user.Player.Map == 11 || user.Player.Map == 12
                   || user.Player.Map == 13 || user.Player.Map == 14 || user.Player.Map == 15 || user.Player.Map == 16
                   || user.Player.Map == 17 || user.Player.Map == 18 || user.Player.Map == 19 || user.Player.Map == 20
                   || user.Player.Map == 21 || user.Player.Map == 22 || user.Player.Map == 23 || user.Player.Map == 3313
                   || user.Player.Map == 3310 || user.Player.Map == 3311 || user.Player.Map == 3312 || user.Player.Map == 3313
                   || user.Player.Map == 3306 || user.Player.Map == 3307 || user.Player.Map == 3308 || user.Player.Map == 3309
                   || user.Player.Map == 3305 || user.Player.Map == 3304 || user.Player.Map == 3303 || user.Player.Map == 3302
                   || user.Player.Map == 3301 || user.Player.Map == 700
                   || user.Player.Map == 1038 || user.Player.Map == 3868 || user.Player.Map == 1002
                   || user.Player.Map == 30
                        || user.Player.Map == 31
                        || user.Player.Map == 32
                        || user.Player.Map == 33
                        || user.Player.Map == 34
                        || user.Player.Map == 35
                        || user.Player.Map == 36
                        || user.Player.Map == 37
                        || user.Player.Map == 38)
               {//
                   user.CreateBoxDialog("NO on Skils this map.");
                   return;
               }
               MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                  , 0, Attack.X, Attack.Y, ClientSpell.ID
                                  , ClientSpell.Level, ClientSpell.UseSpellSoul,0);
               Algoritms.Fan fan = new Algoritms.Fan(user.Player.X, user.Player.Y, Attack.X, Attack.Y, 8, 160);

               uint Experience = 0;
               user.Shift(Attack.X, Attack.Y, stream, false);
               foreach (Role.IMapObj target in user.Player.View.Roles(Role.MapObjectType.Monster))
               {
                   MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                   if (fan.IsInFan(target.X, target.Y))
                   //if (Calculate.Base.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y) <= 5)
                   {
                       if (CheckAttack.CanAttackMonster.Verified(user, attacked, DBSpell))
                       {
                           MsgSpellAnimation.SpellObj AnimationObj;
                           Calculate.Physical.OnMonster(user.Player, attacked, DBSpell, out AnimationObj);
                           AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                           Experience += ReceiveAttack.Monster.Execute(stream, AnimationObj, user, attacked);
                           MsgSpell.Targets.Enqueue(AnimationObj);
                        
                       }
                   }
               }
               foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.Player))
               {
                   var attacked = targer as Role.Player;
                   if (fan.IsInFan(attacked.X, attacked.Y))
                   //if (Calculate.Base.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y) <= 5)
                   {
                       if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell))
                       {
                           MsgSpellAnimation.SpellObj AnimationObj;
                           Calculate.Physical.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
                           AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                           ReceiveAttack.Player.Execute(AnimationObj, user, attacked);

                           MsgSpell.Targets.Enqueue(AnimationObj);
                       }
                   }

               }
               foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.SobNpc))
               {
                   var attacked = targer as Role.SobNpc;
                   if (fan.IsInFan(attacked.X, attacked.Y))
                   //if (Calculate.Base.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y) <= 5)
                   {
                       if (CheckAttack.CanAttackNpc.Verified(user, attacked, DBSpell))
                       {
                           MsgSpellAnimation.SpellObj AnimationObj;
                           Calculate.Physical.OnNpcs(user.Player, attacked, DBSpell, out AnimationObj);
                           AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                           Experience += ReceiveAttack.Npc.Execute(stream, AnimationObj, user, attacked);
                           MsgSpell.Targets.Enqueue(AnimationObj);
                       }
                   }
               }
               Updates.IncreaseExperience.Up(stream, user, Experience);
               Updates.UpdateSpell.CheckUpdate(stream, user, Attack, Experience, DBSpells);
               MsgSpell.SetStream(stream); 
               MsgSpell.Send(user);
               return;
           }
       }
    }
}

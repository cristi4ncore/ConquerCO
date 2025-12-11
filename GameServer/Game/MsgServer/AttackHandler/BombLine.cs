using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeathWish.Database;

namespace DeathWish.Game.MsgServer.AttackHandler
{
    public class BombLine
    {
        public unsafe static void Execute(Client.GameClient user, InteractQuery Attack, ServerSockets.Packet stream, Dictionary<ushort, Database.MagicType.Magic> DBSpells)
        {
            Database.MagicType.Magic DBSpell;
            MsgSpell ClientSpell;
            if (CheckAttack.CanUseSpell.Verified(Attack, user, DBSpells, out ClientSpell, out DBSpell))
            {
                MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                   , 0, Attack.X, Attack.Y, ClientSpell.ID
                                   , ClientSpell.Level, ClientSpell.UseSpellSoul);
                uint Experience = 0;
                foreach (Role.IMapObj target in user.Player.View.Roles(Role.MapObjectType.Monster))
                {
                    MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                    if (Calculate.Base.GetDistance(Attack.X, Attack.Y, attacked.X, attacked.Y) <= 6)
                    {
                        if (CheckAttack.CanAttackMonster.Verified(user, attacked, DBSpell))
                        {
                            if (attacked.Boss != 1)
                            {
                                Algoritms.InLineAlgorithm.coords coord = Algoritms.MoveCoords.CheckBombCoords(user.Player.X, user.Player.Y
                                , attacked.X, attacked.Y, 4, user.Map);

                                attacked.X = (ushort)coord.X;
                                attacked.Y = (ushort)coord.Y;

                                MsgSpellAnimation.SpellObj AnimationObj;
                                Calculate.Physical.OnMonster(user.Player, attacked, DBSpell, out AnimationObj);
                                AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                                Experience += ReceiveAttack.Monster.Execute(stream, AnimationObj, user, attacked);
                                AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);


                                AnimationObj.MoveX = (uint)coord.X;
                                AnimationObj.MoveY = (uint)coord.Y;
                                MsgSpell.Targets.Enqueue(AnimationObj);

                            }
                            else
                            {
                                MsgSpellAnimation.SpellObj AnimationObj;
                                Calculate.Physical.OnMonster(user.Player, attacked, DBSpell, out AnimationObj);
                                AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                                Experience += ReceiveAttack.Monster.Execute(stream, AnimationObj, user, attacked);
                                AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                                MsgSpell.Targets.Enqueue(AnimationObj);

                            }
                            /* Algoritms.InLineAlgorithm.coords coord = Algoritms.MoveCoords.CheckCoords(user.Player.X, user.Player.Y
                             , attacked.X, attacked.Y, 4, user.Map);

                             attacked.X = (ushort)coord.X;
                             attacked.Y = (ushort)coord.Y;*/

                            //MsgSpellAnimation.SpellObj AnimationObj;
                            //Calculate.Physical.OnMonster(user.Player, attacked, DBSpell, out AnimationObj);
                            //AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                            //Experience += ReceiveAttack.Monster.Execute(stream, AnimationObj, user, attacked);
                            //AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);


                            /*AnimationObj.MoveX = (uint)coord.X;
                            AnimationObj.MoveY = (uint)coord.Y;*/

                            //MsgSpell.Targets.Enqueue(AnimationObj);

                        }
                    }
                }
                foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.Player))
                {
                    var attacked = targer as Role.Player;
                    if (Calculate.Base.GetDistance(Attack.X, Attack.Y, attacked.X, attacked.Y) <= 6)
                    {
                        if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell))
                        {
                            Algoritms.InLineAlgorithm.coords coord = Algoritms.MoveCoords.CheckBombCoords(user.Player.X, user.Player.Y
                       , attacked.X, attacked.Y, 4, user.Map);
                            if (coord.X == 0) break;

                            if (!CheckAttack.CheckFloors.CheckGuildWar(user, coord.X, coord.Y))
                            {
                                continue;
                            }
                            attacked.Owner.Player.ProtectAttack(1300);
                            if (attacked.Owner.Player.Intensify == true)
                                attacked.Owner.Player.Intensify = false;
                            user.Map.View.MoveTo<Role.IMapObj>(attacked, coord.X, coord.Y);
                            attacked.X = (ushort)coord.X;
                            attacked.Y = (ushort)coord.Y;

                            MsgSpellAnimation.SpellObj AnimationObj;
                            Calculate.Physical.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
                            AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                            ReceiveAttack.Player.Execute(AnimationObj, user, attacked);

                            AnimationObj.MoveX = (uint)coord.X;
                            AnimationObj.MoveY = (uint)coord.Y;
                            attacked.View.Role(false, null);

                            MsgSpell.Targets.Enqueue(AnimationObj);
                        }
                    }

                }
                foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.SobNpc))
                {
                    var attacked = targer as Role.SobNpc;
                    if (Calculate.Base.GetDistance(Attack.X, Attack.Y, attacked.X, attacked.Y) <= 6)
                    {
                        if (CheckAttack.CanAttackNpc.Verified(user, attacked, DBSpell))
                        {
                            //if (user.Player.Map == 1038 || user.Player.Map == 3868)
                            //{
                            //    return;
                            //}
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

            }
        }

    }
}
//    public class BombLine
//    {
//        public unsafe static void Execute(Client.GameClient user, InteractQuery Attack, ServerSockets.Packet stream, Dictionary<ushort, Database.MagicType.Magic> DBSpells)
//        {
//            Database.MagicType.Magic DBSpell;
//            MsgSpell ClientSpell;
//            if (CheckAttack.CanUseSpell.Verified(Attack, user, DBSpells, out ClientSpell, out DBSpell))
//            {
//                MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
//                                   , 0, Attack.X, Attack.Y, ClientSpell.ID
//                                   , ClientSpell.Level, ClientSpell.UseSpellSoul);
//                int num = 0;

//                switch (ClientSpell.Level)
//                {
//                    case 0:
//                    case 1:
//                        num = 3;
//                        break;
//                    case 2:
//                    case 3:
//                        num = 4;
//                        break;
//                    default:
//                        num = 5;
//                        break;
//                }
//                uint Experience = 0;
//                foreach (Role.IMapObj target in user.Player.View.Roles(Role.MapObjectType.Monster))
//                {
//                    MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
//                    if (Calculate.Base.GetDistance(Attack.X, Attack.Y, attacked.X, attacked.Y) <= 6)
//                    {
//                        if (CheckAttack.CanAttackMonster.Verified(user, attacked, DBSpell))
//                        {
//                            if (num < 1) break;
//                            num--;
//                            MsgSpellAnimation.SpellObj AnimationObj;
//                            Calculate.Physical.OnMonster(user.Player, attacked, DBSpell, out AnimationObj);
//                            AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
//                            Experience += ReceiveAttack.Monster.Execute(stream, AnimationObj, user, attacked);
//                            AnimationObj.Hit = 1;
//                            AnimationObj.MoveX = target.X;
//                            AnimationObj.MoveY = target.Y;
//                            Server.ServerMaps[attacked.Map].Pushback(ref AnimationObj.MoveX, ref AnimationObj.MoveY, user.Player.Angle, 5);

//                            user.Map.View.MoveTo<Role.IMapObj>(attacked, (ushort)AnimationObj.MoveX, (ushort)AnimationObj.MoveY);
//                            attacked.X = (ushort)AnimationObj.MoveX;
//                            attacked.Y = (ushort)AnimationObj.MoveY;

//                            MsgSpell.Targets.Enqueue(AnimationObj);

//                        }
//                    }
//                }
//                foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.Player))
//                {
//                    var attacked = targer as Role.Player;
//                    if (Calculate.Base.GetDistance(Attack.X, Attack.Y, attacked.X, attacked.Y) <= 6)
//                    {
//                        if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell))
//                        {
//                            if (num < 1) break;
//                            num--;
//                            MsgSpellAnimation.SpellObj AnimationObj;
//                            Calculate.Physical.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
//                            AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
//                            ReceiveAttack.Player.Execute(AnimationObj, user, attacked);
//                            AnimationObj.Hit = 1;
//                            AnimationObj.MoveX = targer.X;
//                            AnimationObj.MoveY = targer.Y;
//                            attacked.Owner.Map.Pushback(ref AnimationObj.MoveX, ref AnimationObj.MoveY, user.Player.Angle, 5);

//                            if (!CheckAttack.CheckFloors.CheckGuildWar(attacked.Owner, (ushort)AnimationObj.MoveX, (ushort)AnimationObj.MoveY))
//                            {
//                                continue;
//                            }
//                            user.Map.View.MoveTo<Role.IMapObj>(attacked, (ushort)AnimationObj.MoveX, (ushort)AnimationObj.MoveY);
//                            attacked.X = (ushort)AnimationObj.MoveX;
//                            attacked.Y = (ushort)AnimationObj.MoveY;
//                            attacked.View.Role(false, null);

//                            MsgSpell.Targets.Enqueue(AnimationObj);

//                            attacked.AttackStamp = Extensions.Time32.Now.AddSeconds(2);

//                            attacked.Protect = Extensions.Time32.Now.AddSeconds(2);
//                        }
//                    }

//                }
//                foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.SobNpc))
//                {
//                    var attacked = targer as Role.SobNpc;
//                    if (Calculate.Base.GetDistance(Attack.X, Attack.Y, attacked.X, attacked.Y) <= 6)
//                    {
//                        if (CheckAttack.CanAttackNpc.Verified(user, attacked, DBSpell))
//                        {
//                            if (num < 1) break;
//                            num--;
//                            if (user.Player.Map == 1038 || user.Player.Map == 3868)
//                            {
//                                return;
//                            }
//                            MsgSpellAnimation.SpellObj AnimationObj;
//                            Calculate.Physical.OnNpcs(user.Player, attacked, DBSpell, out AnimationObj);
//                            AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
//                            Experience += ReceiveAttack.Npc.Execute(stream, AnimationObj, user, attacked);
//                            MsgSpell.Targets.Enqueue(AnimationObj);
//                        }
//                    }
//                }
//                Updates.IncreaseExperience.Up(stream, user, Experience);
//                Updates.UpdateSpell.CheckUpdate(stream, user, Attack, Experience, DBSpells);
//                MsgSpell.SetStream(stream);
//                MsgSpell.Send(user);

//            }
//        }

//    }
//}
////   public class BombLine
////    {
////       public unsafe static void Execute(Client.GameClient user, InteractQuery Attack, ServerSockets.Packet stream, Dictionary<ushort, Database.MagicType.Magic> DBSpells)
////       {
////           Database.MagicType.Magic DBSpell;
////           MsgSpell ClientSpell;
////           if (CheckAttack.CanUseSpell.Verified(Attack,user, DBSpells, out ClientSpell, out DBSpell))
////           {
////               MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
////                                  , 0, Attack.X, Attack.Y, ClientSpell.ID
////                                  , ClientSpell.Level, ClientSpell.UseSpellSoul);
////               uint Experience = 0;
////               foreach (Role.IMapObj target in user.Player.View.Roles(Role.MapObjectType.Monster))
////               {
////                   MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
////                   if (Calculate.Base.GetDistance(Attack.X, Attack.Y, attacked.X, attacked.Y) <= 6)
////                   {
////                       if (CheckAttack.CanAttackMonster.Verified(user, attacked, DBSpell))
////                       {
////                          /* Algoritms.InLineAlgorithm.coords coord = Algoritms.MoveCoords.CheckCoords(user.Player.X, user.Player.Y
////                           , attacked.X, attacked.Y, 4, user.Map);

////                           attacked.X = (ushort)coord.X;
////                           attacked.Y = (ushort)coord.Y;*/

////                           MsgSpellAnimation.SpellObj AnimationObj;
////                           Calculate.Physical.OnMonster(user.Player, attacked, DBSpell, out AnimationObj);
////                           AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
////                           Experience+=    ReceiveAttack.Monster.Execute(stream,AnimationObj, user, attacked);
////                           AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);


////                           /*AnimationObj.MoveX = (uint)coord.X;
////                           AnimationObj.MoveY = (uint)coord.Y;*/

////                           MsgSpell.Targets.Enqueue(AnimationObj);
                           
////                       }
////                   }
////               }
////               foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.Player))
////               {
////                   var attacked = targer as Role.Player;
////                   if (Calculate.Base.GetDistance(Attack.X, Attack.Y, attacked.X, attacked.Y) <= 6)
////                   {
////                       if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell))
////                       {
////                           Algoritms.InLineAlgorithm.coords coord = Algoritms.MoveCoords.CheckBombCoords(user.Player.X, user.Player.Y
////                      , attacked.X, attacked.Y, 4, user.Map);
////                           if (coord.X == 0) break;

////                           if (!CheckAttack.CheckFloors.CheckGuildWar(user, coord.X, coord.Y))
////                           {
////                               continue;
////                           }
                           
////                           user.Map.View.MoveTo<Role.IMapObj>(attacked, coord.X, coord.Y);
////                           attacked.X = (ushort)coord.X;
////                           attacked.Y = (ushort)coord.Y;

////                           MsgSpellAnimation.SpellObj AnimationObj;
////                           Calculate.Physical.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
////                           AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
////                           ReceiveAttack.Player.Execute(AnimationObj, user, attacked);
                         
////                           AnimationObj.MoveX = (uint)coord.X;
////                           AnimationObj.MoveY = (uint)coord.Y;
////                           attacked.View.Role(false,null);

////                           MsgSpell.Targets.Enqueue(AnimationObj);
////                       }
////                   }

////               }
////               foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.SobNpc))
////               {
////                   var attacked = targer as Role.SobNpc;
////                   if (Calculate.Base.GetDistance(Attack.X, Attack.Y, attacked.X, attacked.Y) <= 6)
////                   {
////                       if (CheckAttack.CanAttackNpc.Verified(user, attacked, DBSpell))
////                       {
////                           MsgSpellAnimation.SpellObj AnimationObj;
////                           Calculate.Physical.OnNpcs(user.Player, attacked, DBSpell, out AnimationObj);
////                           AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
////                           Experience += ReceiveAttack.Npc.Execute(stream,AnimationObj, user, attacked);
////                           MsgSpell.Targets.Enqueue(AnimationObj);
////                       }
////                   }
////               }
////               Updates.IncreaseExperience.Up(stream,user, Experience);
////               Updates.UpdateSpell.CheckUpdate(stream,user,Attack, Experience, DBSpells);
////               MsgSpell.SetStream(stream);
////               MsgSpell.Send(user);

////           }
////       }
  
////    }
////}

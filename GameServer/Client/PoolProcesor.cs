using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeathWish.Game.MsgServer;
using DeathWish.Game.MsgFloorItem;
using DeathWish.Game.MsgServer.AttackHandler;
using DeathWish.Game.MsgTournaments;
using DeathWish.Game;
using Extensions;
using DeathWish.Database;
using DeathWish.Role.Instance;
using static DeathWish.Role.Flags;

namespace DeathWish.Client
{
    public static class PoolProcesses
    {
        public static int Online
        {
            get
            {
                int current = Database.Server.GamePoll.Count;
                return current;
            }
        }
        public static int MaxOnline;
        public static unsafe void AutoAttackCallback(Client.GameClient client)
        {
            try
            {
                if (client == null || !client.FullLoading || client.Player == null)
                    return;

                if (client.Player.Alive == false && client.Player.CompleteLogin)
                {
                    if (DateTime.Now > client.Player.GhostStamp)
                    {
                        if (!client.Player.ContainFlag(MsgUpdate.Flags.Ghost))
                        {
                            client.Player.AddFlag(Game.MsgServer.MsgUpdate.Flags.Ghost, Role.StatusFlagsBigVector32.PermanentFlag, true);
                            if (client.Player.Body % 10 < 3)
                                client.Player.TransformationID = 99;
                            else
                                client.Player.TransformationID = 98;
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                client.Send(stream.MapStatusCreate(client.Player.Map, client.Map.ID, (uint)client.Map.TypeStatus));
                            }
                        }
                    }
                }
                Extensions.Time32 timer = Extensions.Time32.Now;

                if (client.OnAutoAttack && client.Player.Alive)
                {
                    if (client.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.Dizzy))
                    {
                        client.OnAutoAttack = false;
                        return;
                    }

                    InteractQuery action = new InteractQuery();
                    action = InteractQuery.ShallowCopy(client.AutoAttack);
                    client.Player.RandomSpell = action.SpellID;
                    MsgAttackPacket.Process(client, action);
                }
            }
            catch (Exception e)
            {
                MyConsole.WriteException(e);
            }

        }
        //public static unsafe void AutoAttackCallback(Client.GameClient client)
        //{
        //    try
        //    {
        //        if (client == null || !client.FullLoading || client.Player == null)
        //            return;
        //        if (client.AutoHandle != null)
        //        {
        //            client.AutoHandle.Run();
        //        }
        //        if (client.Player.Alive == false && client.Player.CompleteLogin)
        //        {
        //            if (DateTime.Now > client.Player.GhostStamp)
        //            {
        //                if (!client.Player.ContainFlag(MsgUpdate.Flags.Ghost))
        //                {
        //                    client.Player.AddFlag(Game.MsgServer.MsgUpdate.Flags.Ghost, Role.StatusFlagsBigVector32.PermanentFlag, true);
        //                    if (client.Player.Body % 10 < 3)
        //                        client.Player.TransformationID = 99;
        //                    else
        //                        client.Player.TransformationID = 98;
        //                    using (var rec = new ServerSockets.RecycledPacket())
        //                    {
        //                        var stream = rec.GetStream();
        //                        client.Send(stream.MapStatusCreate(client.Player.Map, client.Map.ID, (uint)client.Map.TypeStatus));

        //                        //if (Program.Block_PATH_FINDING.Contains(client.Map.ID))
        //                        //    client.Send(GuardShield.MsgGuardShield.LoaderMapStatue(client.Map.ID, 123456));
        //                        //else
        //                        //    client.Send(GuardShield.MsgGuardShield.LoaderMapStatue(client.Map.ID, 0));
        //                    }
        //                }
        //            }
        //        }
        //        Extensions.Time32 timer = Extensions.Time32.Now;

        //        if (client.OnAutoAttack && client.Player.Alive)
        //        {
        //            if (client.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.Dizzy))
        //            {
        //                client.OnAutoAttack = false;
        //                return;
        //            }

        //            InteractQuery action = new InteractQuery();
        //            action = InteractQuery.ShallowCopy(client.AutoAttack);
        //            client.Player.RandomSpell = action.SpellID;
        //            MsgAttackPacket.Process(client, action);
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        MyConsole.WriteException(e);
        //    }
        //}//Done
        public static unsafe void BuffersCallback(Client.GameClient client)//Done
        {
            try
            {
                if (client == null || !client.FullLoading)
                    return;
                Extensions.Time32 Timer = Extensions.Time32.Now;
                #region Intensify Archer
                if (client.Player.InUseIntensify)
                {
                    if (Timer > client.Player.IntensifyStamp.AddSeconds(3))
                    {
                        if (!client.Player.Intensify)
                        {
                            client.Player.Intensify = true;
                            client.Player.InUseIntensify = false;
                        }
                    }
                }
                #endregion
                #region HuntingBounsStamp
                if (Timer >= client.Player.HuntingBounsStamp)
                {
                    if (client.Player.HuntingBounsLayer > 0) { client.Player.HuntingBounsStamp = Timer.AddMinutes(5); client.Player.HuntingBounsLayer -= 1; }
                }
                #endregion
                //#region Stamina & Vigor
                //if (client.Player.Alive && !client.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.Fly))
                //{
                //    byte MaxStamina = (byte)(client.Player.HeavenBlessing > 0 ? 150 : 100);
                //    if (client.Equipment.UseMonkEpicWeapon)
                //    {
                //        MsgSpell user_spell = null;
                //        if (client.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.GraceofHeaven, out user_spell))
                //        {
                //            Database.MagicType.Magic DBSpell = Database.Server.Magic[user_spell.ID][user_spell.Level];
                //            MaxStamina += (byte)DBSpell.Damage;
                //        }
                //    }
                //    if (client.Player.Stamina < MaxStamina)
                //    {
                //        ushort addstamin = 0;
                //        if (client.Player.Action == Role.Flags.ConquerAction.Sit)
                //            addstamin += 8;
                //        else
                //            addstamin += 3;

                //        if (client.Player.Map == 7777 || client.Player.Map == 2579 || client.Player.Map == 2569 || client.Player.Map == 2567 || client.Player.Map == 8601 || client.Player.Map == 8602 || client.Player.Map == 7701 || client.Player.Map == 7702 || client.Player.Map == 7703 || client.Player.Map == 7704 || client.Player.Map == 7705 || client.Player.Map == 7706 || client.Player.Map == 7707 || client.Player.Map == 7708 || client.Player.Map == 7709 || client.Player.Map == 7710 || client.Player.Map == 7711 || client.Player.Map == 7712)
                //            addstamin += 100;
                //        if (client.Player.ContainFlag(MsgUpdate.Flags.WindWalkerFan))
                //        {
                //            if (Timer > client.Player.FanRecoverStamin.AddSeconds(5))
                //            {
                //                addstamin += (ushort)(addstamin * 50 / 100);
                //                client.Player.FanRecoverStamin = Extensions.Time32.Now;
                //            }
                //        }
                //        client.Player.Stamina = (ushort)Math.Min((int)(client.Player.Stamina + addstamin), MaxStamina);
                //        using (var rec = new ServerSockets.RecycledPacket())
                //        {
                //            var stream = rec.GetStream();
                //            client.Player.SendUpdate(stream, client.Player.Stamina, Game.MsgServer.MsgUpdate.DataType.Stamina);
                //        }
                //    }
                //    if (client.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.Ride))
                //    {
                //        if (client.Player.CheckInvokeFlag(Game.MsgServer.MsgUpdate.Flags.Ride, Timer))
                //        {
                //            if (client.Vigor < client.Status.MaxVigor)
                //            {
                //                client.Vigor = (ushort)Math.Min(client.Vigor + 2, client.Status.MaxVigor);

                //                using (var rec = new ServerSockets.RecycledPacket())
                //                {
                //                    var stream = rec.GetStream();
                //                    client.Send(stream.ServerInfoCreate(MsgServerInfo.Action.Vigor, client.Vigor));
                //                }
                //            }
                //        }
                //    }
                //}
                //#endregion
                #region XPList
                if (client.Player.Alive && !client.Player.OnAutoHunt)
                {
                    if (Timer > client.Player.XPListStamp.AddSeconds(4) && client.Player.Alive)
                    {
                        client.Player.XPListStamp = Timer.AddSeconds(4);
                        if (!client.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.XPList))
                        {
                            client.Player.XPCount++;
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                client.Player.SendUpdate(stream, client.Player.XPCount, MsgUpdate.DataType.XPCircle);
                                if (client.Player.XPCount >= 100)
                                {
                                    client.Player.XPCount = 0;
                                    client.Player.AddFlag(Game.MsgServer.MsgUpdate.Flags.XPList, 20, true);
                                    client.Player.SendString(stream, Game.MsgServer.MsgStringPacket.StringID.Effect, true, new string[1] { "xp" });
                                }
                            }
                        }
                    }
                }
                #endregion
                #region PK Points
                if (client.Player.PKPoints > 0)
                {
                    if (Timer > client.Player.PkPointsStamp.AddMinutes(6))
                    {
                        client.Player.PKPoints -= 1;
                        client.Player.PkPointsStamp = Extensions.Time32.Now;
                    }
                }
                #endregion
                #region BlackSpot
                if (client.Player.BlackSpot)
                {
                    if (Timer > client.Player.Stamp_BlackSpot)
                    {
                        client.Player.BlackSpot = false;
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();

                            client.Player.View.SendView(stream.BlackspotCreate(false, client.Player.UID), true);
                        }
                    }
                }
                #endregion
                #region LoginTimer
                if (Timer > client.Player.LoginTimer.AddHours(5))
                {
                    client.Player.LoginTimer = Extensions.Time32.Now;
                    client.Activeness.IncreaseTask(3);
                    client.Activeness.IncreaseTask(15);
                    client.Activeness.IncreaseTask(27);
                }
                #endregion
                #region flag
                foreach (var flag in client.Player.BitVector.GetFlags())
                {
                    if (flag.Expire(Timer))
                    {
                        if (flag.Key >= (int)Game.MsgServer.MsgUpdate.Flags.TyrantAura && flag.Key <= (int)Game.MsgServer.MsgUpdate.Flags.EartAura)
                        {
                            client.Player.AddAura(client.Player.UseAura, client.Player.OwnerUseAura, null, 0);
                        }
                        else
                        {

                            if (flag.Key == (int)Game.MsgServer.MsgUpdate.Flags.Superman || flag.Key == (int)Game.MsgServer.MsgUpdate.Flags.Cyclone
                                || flag.Key == (int)Game.MsgServer.MsgUpdate.Flags.SuperCyclone || flag.Key == (int)Game.MsgServer.MsgUpdate.Flags.Oblivion)
                            {
                                Role.KOBoard.KOBoardRanking.AddItem(new Role.KOBoard.Entry() { UID = client.Player.UID, Name = client.Player.Name, Points = client.Player.KillCounter }, true);
                            }
                            client.Player.RemoveFlag((Game.MsgServer.MsgUpdate.Flags)flag.Key);
                        }
                    }
                    if (flag.Key == (int)Game.MsgServer.MsgUpdate.Flags.ScarofEarth)
                    {
                        if (flag.CheckInvoke(Timer))
                        {
                            if (client.Player.ScarofEarthl != null && client.Player.AttackerScarofEarthl != null)
                            {
                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();

                                    var DBSpell = client.Player.ScarofEarthl;
                                    MsgSpellAnimation MsgSpell = new MsgSpellAnimation(
                                        client.Player.UID
                                          , 0, client.Player.X, client.Player.Y, DBSpell.ID
                                          , DBSpell.Level, 0, 1);

                                    MsgSpellAnimation.SpellObj AnimationObj = new MsgSpellAnimation.SpellObj()
                                    {
                                        UID = client.Player.UID,
                                        Damage = (uint)DBSpell.Damage2 / 2,

                                        Hit = 1
                                    };

                                    Game.MsgServer.AttackHandler.ReceiveAttack.Player.Execute(AnimationObj, client.Player.AttackerScarofEarthl, client.Player);
                                    MsgSpell.SetStream(stream);
                                    MsgSpell.Targets.Enqueue(AnimationObj);
                                    MsgSpell.Send(client);
                                }
                            }
                        }
                    }

                    else if (flag.Key == (int)Game.MsgServer.MsgUpdate.Flags.DragonFlow)
                    {
                        if (flag.CheckInvoke(Timer))
                        {
                            byte MaxStamina = (byte)(client.Player.HeavenBlessing > 0 ? 150 : 100);

                            if (client.Player.Stamina < MaxStamina)
                            {
                                client.Player.Stamina += 20;
                                client.Player.Stamina = (ushort)Math.Min((int)client.Player.Stamina, MaxStamina); using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    client.Player.SendUpdate(stream, client.Player.Stamina, Game.MsgServer.MsgUpdate.DataType.Stamina);
                                }
                            }
                        }
                    }
                    else if (flag.Key == (int)Game.MsgServer.MsgUpdate.Flags.HealingSnow)
                    {
                        if (flag.CheckInvoke(Timer) && client.Player.Alive)
                        {
                            if (client.Player.HitPoints < client.Status.MaxHitpoints || client.Player.Mana < client.Status.MaxMana)
                            {
                                MsgSpell spell;
                                if (client.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.HealingSnow, out spell))
                                {
                                    var arrayspells = Database.Server.Magic[(ushort)Role.Flags.SpellID.HealingSnow];
                                    var DbSpell = arrayspells[(ushort)Math.Min((int)spell.Level, arrayspells.Count - 1)];

                                    client.Player.HitPoints = (int)Math.Min(client.Status.MaxHitpoints, (int)(client.Player.HitPoints + DbSpell.Damage2));
                                    client.Player.Mana = (ushort)Math.Min(client.Status.MaxMana, (int)(client.Player.Mana + DbSpell.Damage3));
                                    client.Player.SendUpdateHP();
                                    client.Player.XPCount += 1;
                                }
                            }
                        }
                    }

                    else if (flag.Key == (int)Game.MsgServer.MsgUpdate.Flags.Electricity)
                    {
                        if (flag.CheckInvoke(Timer))
                        {
                            if (client.Player.HitPoints < client.Player.BleedDamage)
                            {
                                client.Player.RemoveFlag(MsgUpdate.Flags.Electricity);
                                client.Player.BleedDamage = 0;
                                goto jump;
                            }
                            client.Player.HitPoints = Math.Max(1, (int)(client.Player.HitPoints - client.Player.BleedDamage));

                        jump:

                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();

                                InteractQuery action = new InteractQuery()
                                {
                                    Damage = client.Player.BleedDamage,
                                    AtkType = MsgAttackPacket.AttackID.Physical,
                                    X = client.Player.X,
                                    Y = client.Player.Y,
                                    OpponentUID = client.Player.UID,
                                };
                                client.Player.AddFlag(MsgUpdate.Flags.Electricity, 0, true, 2);
                                client.Player.View.SendView(stream.InteractionCreate(&action), true);
                            }
                        }
                    }
                    else if (flag.Key == (int)Game.MsgServer.MsgUpdate.Flags.Poisoned)
                    {
                        if (flag.CheckInvoke(Timer))
                        {
                            if (client.Player.HitPoints == 1 || client.Status.Detoxication >= 100 || client.PerfectionStatus.ToxinEraser > 90)
                                break;
                            int damage = (int)Game.MsgServer.AttackHandler.Calculate.Base.CalculatePoisonDamageFog((uint)client.Player.HitPoints, client.Player.PoisonLevel);

                            if (client.Player.HitPoints == 1)
                            {
                                damage = 0;
                                goto jump;
                            }
                            client.Player.HitPoints = Math.Max(1, (int)(client.Player.HitPoints - damage));

                        jump:

                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();

                                InteractQuery action = new InteractQuery()
                                {
                                    Damage = damage,
                                    AtkType = MsgAttackPacket.AttackID.Physical,
                                    X = client.Player.X,
                                    Y = client.Player.Y,
                                    OpponentUID = client.Player.UID
                                };
                                client.Player.View.SendView(stream.InteractionCreate(&action), true);
                            }

                        }
                    }
                    else if (flag.Key == (int)Game.MsgServer.MsgUpdate.Flags.ShurikenVortex)
                    {
                        if (flag.CheckInvoke(Timer))
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();

                                InteractQuery action = new InteractQuery()
                                {
                                    UID = client.Player.UID,
                                    X = client.Player.X,
                                    Y = client.Player.Y,
                                    SpellID = (ushort)Role.Flags.SpellID.ShurikenEffect,
                                    AtkType = MsgAttackPacket.AttackID.Magic
                                };

                                MsgAttackPacket.ProcescMagic(client, stream.InteractionCreate(&action), action);
                            }
                        }
                    }
                    else if (flag.Key == (int)Game.MsgServer.MsgUpdate.Flags.RedName || flag.Key == (int)Game.MsgServer.MsgUpdate.Flags.BlackName)
                    {
                        if (flag.CheckInvoke(Timer))
                        {
                            if (client.Player.PKPoints > 0)
                                client.Player.PKPoints -= 1;

                            client.Player.PkPointsStamp = Extensions.Time32.Now;
                        }
                    }
                    else if (flag.Key == (int)Game.MsgServer.MsgUpdate.Flags.Cursed)
                    {
                        if (flag.CheckInvoke(Timer))
                        {
                            if (client.Player.CursedTimer > 0)
                                client.Player.CursedTimer -= 1;
                        }
                    }
                }
                #endregion
                #region Transform
                if (client.Player.OnTransform)
                {
                    if (client.Player.TransformInfo != null)
                    {
                        if (client.Player.TransformInfo.CheckUp(Timer))
                            client.Player.TransformInfo = null;
                    }
                }
                #endregion
                #region Praying
                if (client.Player.Alive && !client.Player.OnAutoHunt)
                {
                    if (client.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.Praying))
                    {
                        if (client.Player.BlessTime < 7200000 - 30000)
                        {
                            if (Timer > client.Player.CastPrayStamp.AddSeconds(30))
                            {
                                bool have = false;
                                foreach (var ownerpraying in client.Player.View.Roles(Role.MapObjectType.Player))
                                {
                                    if (Role.Core.GetDistance(client.Player.X, client.Player.Y, ownerpraying.X, ownerpraying.Y) <= 2)
                                    {
                                        var target = ownerpraying as Role.Player;
                                        if (target.ContainFlag(MsgUpdate.Flags.CastPray))
                                        {
                                            have = true;
                                            break;
                                        }
                                    }
                                }
                                if (!have)
                                    client.Player.RemoveFlag(MsgUpdate.Flags.Praying);
                                client.Player.CastPrayStamp = new Extensions.Time32(Timer.AllMilliseconds);
                                client.Player.BlessTime += 30000;
                            }
                        }
                        else
                            client.Player.BlessTime = 3100000;
                    }
                }
                #endregion
                //#region goldentree
                //#region GoldenTree
                //if (GlobalLotteryTable.TodayCondition == null)
                //{
                //    GlobalLotteryTable.TodayCondition = GlobalLotteryTable.Conditions.Values.OrderByDescending(i => DateTime.Now <= i.StartTime).FirstOrDefault();
                //}
                //#endregion
                //if (GlobalLotteryTable.TodayCondition != null)
                //{
                //    var gTime = GlobalLotteryTable.TodayCondition.StartTime;
                //    var blossomTime = gTime.AddSeconds(-10);
                //    using (var rec = new ServerSockets.RecycledPacket())
                //    {
                //        var stream = rec.GetStream();
                //        if (blossomTime.Day == DateTime.Now.Day && blossomTime.Hour == DateTime.Now.Hour && blossomTime.Minute == DateTime.Now.Minute && blossomTime.Second == DateTime.Now.Second)
                //        {
                //            MsgGoldenTree.Ready(client, stream);
                //        }
                //        else if (gTime.Day == DateTime.Now.Day && gTime.Hour == DateTime.Now.Hour && gTime.Minute == DateTime.Now.Minute && gTime.Second == DateTime.Now.Second)
                //        {
                //            MsgGoldenTree.BlossomEvaluation(client, stream);
                //        }
                //        else if (gTime.Day == DateTime.Now.Day && (gTime.Hour + 2) == DateTime.Now.Hour && gTime.Minute == DateTime.Now.Minute && gTime.Second == DateTime.Now.Second)
                //        {
                //            MsgGoldenTree.EndEvent();
                //        }
                //    }

                //}
                //#endregion
                #region CastPray
                if (client.Player.Alive)
                {
                    if (client.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.CastPray))
                    {
                        if (client.Player.BlessTime < 7200000 - 60000)
                        {
                            if (Timer > client.Player.CastPrayStamp.AddSeconds(30))
                            {
                                client.Player.CastPrayStamp = new Extensions.Time32(Timer.AllMilliseconds);
                                client.Player.BlessTime += 60000;
                            }
                        }
                        else
                            client.Player.BlessTime = 7200000;
                        if (Timer > client.Player.CastPrayActionsStamp.AddSeconds(5))
                        {
                            client.Player.CastPrayActionsStamp = new Extensions.Time32(Timer.AllMilliseconds);
                            foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Player))
                            {
                                if (Role.Core.GetDistance(client.Player.X, client.Player.Y, obj.X, obj.Y) <= 1)
                                {
                                    var Target = obj as Role.Player;
                                    if (Target.Reborn < 2)
                                    {
                                        if (!Target.ContainFlag(Game.MsgServer.MsgUpdate.Flags.Praying))
                                        {
                                            Target.AddFlag(Game.MsgServer.MsgUpdate.Flags.Praying, Role.StatusFlagsBigVector32.PermanentFlag, true);

                                            using (var rec = new ServerSockets.RecycledPacket())
                                            {
                                                var stream = rec.GetStream();
                                                ActionQuery action = new ActionQuery()
                                                {
                                                    ObjId = client.Player.UID,
                                                    dwParam = (uint)client.Player.Action,
                                                    Timestamp = (int)obj.UID
                                                };
                                                client.Player.View.SendView(stream.ActionCreate(&action), true);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else if (client.Player.BlessTime > 0)
                    {
                        if (!client.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.CastPray) && !client.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.Praying))
                        {

                            if (Timer > client.Player.CastPrayStamp.AddSeconds(2))
                            {
                                if (client.Player.BlessTime > 2000)
                                    client.Player.BlessTime -= 2000;
                                else
                                    client.Player.BlessTime = 0;
                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    client.Player.SendUpdate(stream, client.Player.BlessTime, Game.MsgServer.MsgUpdate.DataType.LuckyTimeTimer);
                                }
                                client.Player.CastPrayStamp = new Extensions.Time32(Timer.AllMilliseconds);
                            }
                        }
                    }
                }
                #endregion
                #region Team invite
                if (client.Player.Alive)
                {
                    if (client.Team != null)
                    {
                        if (client.Team.AutoInvite == true && client.Player.Map != 1036 && client.Team.CkeckToAdd())
                        {
                            if (Timer > client.Team.InviteTimer.AddSeconds(10))
                            {
                                client.Team.InviteTimer = Timer;
                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Player))
                                {
                                    if (!client.Team.SendInvitation.Contains(obj.UID))
                                    {
                                        client.Team.SendInvitation.Add(obj.UID);

                                        if ((obj as Role.Player).Owner.Team == null)
                                        {
                                            using (var rec = new ServerSockets.RecycledPacket())
                                            {
                                                var stream = rec.GetStream();

                                                obj.Send(stream.PopupInfoCreate(client.Player.UID, obj.UID, client.Player.Level, client.Player.BattlePower));

                                                stream.TeamCreate(MsgTeam.TeamTypes.InviteRequest, client.Player.UID);
                                                obj.Send(stream);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        if (client.Team.TeamLider(client))
                        {
                            if (Timer > client.Team.UpdateLeaderLocationStamp.AddSeconds(4))
                            {
                                client.Team.UpdateLeaderLocationStamp = Timer;
                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();

                                    ActionQuery action = new ActionQuery()
                                    {
                                        ObjId = client.Player.UID,
                                        dwParam = 1015,
                                        Type = ActionType.LocationTeamLieder,
                                        wParam1 = client.Team.Leader.Player.X,
                                        wParam2 = client.Team.Leader.Player.Y
                                    };
                                    client.Team.SendTeam(stream.ActionCreate(&action), client.Player.UID, client.Player.Map);
                                }
                            }
                        }
                    }
                }
                #endregion
            }
            catch (Exception e)
            {
                MyConsole.WriteException(e);
            }
        }
        public static unsafe void BotTheard()//Done
        {
            Game.AISystem.AIBotHandle.Action();
        }
        public static unsafe void FloorCallback(Client.GameClient client)//Done
        {
            try
            {
                if (client == null || !client.FullLoading || client.Player == null)
                    return;
                Extensions.Time32 Now = Extensions.Time32.Now;
                #region ManiacDance
                if (client.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.ManiacDance))
                {
                    if (Now > client.Player.ManiacDanceStamp)
                    {
                        client.Player.ManiacDanceStamp = Extensions.Time32.Now.AddMilliseconds(1000);
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            var ClientSpell = client.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.ManiacDance];
                            var DBSpell = Database.Server.Magic[(ushort)Role.Flags.SpellID.ManiacDance][0];
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(
                                client.Player.UID
                                  , 0, client.Player.X, client.Player.Y, ClientSpell.ID
                                  , ClientSpell.Level, ClientSpell.UseSpellSoul);
                            uint Experience = 0;

                            foreach (Role.IMapObj target in client.Player.View.Roles(Role.MapObjectType.Monster))
                            {
                                Game.MsgMonster.MonsterRole attacked = target as Game.MsgMonster.MonsterRole;
                                if (Game.MsgServer.AttackHandler.Calculate.Base.GetDistance(client.Player.X, client.Player.Y, attacked.X, attacked.Y) <= 5)
                                {
                                    if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackMonster.Verified(client, attacked, DBSpell))
                                    {
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Game.MsgServer.AttackHandler.Calculate.Range.OnMonster(client.Player, attacked, DBSpell, out AnimationObj);
                                        AnimationObj.Damage = Game.MsgServer.AttackHandler.Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                                        Experience += Game.MsgServer.AttackHandler.ReceiveAttack.Monster.Execute(stream, AnimationObj, client, attacked);
                                        MsgSpell.Targets.Enqueue(AnimationObj);

                                    }
                                }
                            }
                            foreach (Role.IMapObj targer in client.Player.View.Roles(Role.MapObjectType.Player))
                            {
                                var attacked = targer as Role.Player;
                                if (Game.MsgServer.AttackHandler.Calculate.Base.GetDistance(client.Player.X, client.Player.Y, attacked.X, attacked.Y) <= 5)
                                {
                                    if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackPlayer.Verified(client, attacked, DBSpell))
                                    {
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Game.MsgServer.AttackHandler.Calculate.Range.OnPlayer(client.Player, attacked, DBSpell, out AnimationObj);
                                        AnimationObj.Damage = Game.MsgServer.AttackHandler.Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                                        AnimationObj.Damage = AnimationObj.Damage * 250 / 100;
                                        Game.MsgServer.AttackHandler.ReceiveAttack.Player.Execute(AnimationObj, client, attacked);
                                        MsgSpell.Targets.Enqueue(AnimationObj);
                                    }
                                }

                            }
                            foreach (Role.IMapObj targer in client.Player.View.Roles(Role.MapObjectType.SobNpc))
                            {
                                var attacked = targer as Role.SobNpc;
                                if (Game.MsgServer.AttackHandler.Calculate.Base.GetDistance(client.Player.X, client.Player.Y, attacked.X, attacked.Y) <= 5)
                                {
                                    if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackNpc.Verified(client, attacked, DBSpell))
                                    {
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Game.MsgServer.AttackHandler.Calculate.Range.OnNpcs(client.Player, attacked, DBSpell, out AnimationObj);
                                        AnimationObj.Damage = AnimationObj.Damage * 20 / 100;
                                        AnimationObj.Damage = Game.MsgServer.AttackHandler.Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                                        Experience += Game.MsgServer.AttackHandler.ReceiveAttack.Npc.Execute(stream, AnimationObj, client, attacked);
                                        MsgSpell.Targets.Enqueue(AnimationObj);
                                    }
                                }
                            }
                            Game.MsgServer.AttackHandler.Updates.IncreaseExperience.Up(stream, client, Experience);

                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(client);
                        }
                    }
                }
                #endregion
                #region FloorSpells
                if (client.Player.FloorSpells.Count != 0)
                {
                    foreach (var ID in client.Player.FloorSpells)
                    {
                        switch (ID.Key)
                        {
                            #region Wind Physical
                            case (ushort)Role.Flags.SpellID.ShadowofChaser:
                                {

                                    var spellclient = ID.Value;
                                    Queue<Role.FloorSpell> RemoveSpells = new Queue<Role.FloorSpell>();

                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        foreach (var spell in spellclient.Spells.ToArray())
                                        {
                                            if (spellclient.CheckInvocke(Now, spell))
                                            {
                                                uint Experience = 0;
                                                RemoveSpells.Enqueue(spell);


                                                spellclient.X = spell.FloorPacket.m_X;
                                                spellclient.Y = spell.FloorPacket.m_Y;

                                                spellclient.CreateMsgSpell(0);



                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Monster))
                                                {
                                                    var monster = obj as Game.MsgMonster.MonsterRole;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= 3)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackMonster.Verified(client, monster, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Range.OnMonster(client.Player, monster, spell.DBSkill, out AnimationObj);
                                                            Experience += Game.MsgServer.AttackHandler.ReceiveAttack.Monster.Execute(stream, AnimationObj, client, monster);
                                                            AnimationObj.Damage = Game.MsgServer.AttackHandler.Calculate.Base.CalculateSoul(AnimationObj.Damage, spellclient.LevelHu);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Player))
                                                {
                                                    var target = obj as Role.Player;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= 3)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackPlayer.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Range.OnPlayer(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Player.Execute(AnimationObj, client, target);
                                                            AnimationObj.Damage = Game.MsgServer.AttackHandler.Calculate.Base.CalculateSoul(AnimationObj.Damage, spellclient.LevelHu);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.SobNpc))
                                                {
                                                    var target = obj as Role.SobNpc;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= 3)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackNpc.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Range.OnNpcs(client.Player, target, spell.DBSkill, out AnimationObj);

                                                            Experience += Game.MsgServer.AttackHandler.ReceiveAttack.Npc.Execute(stream, AnimationObj, client, target);

                                                            AnimationObj.Damage = Game.MsgServer.AttackHandler.Calculate.Base.CalculateSoul(AnimationObj.Damage, spellclient.LevelHu);
                                                            AnimationObj.Hit = 1;

                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                spellclient.SendView(stream, client);

                                                Game.MsgServer.AttackHandler.Updates.IncreaseExperience.Up(stream, client, Experience);

                                                spell.FloorPacket.DropType = Game.MsgFloorItem.MsgDropID.RemoveEffect;

                                                client.Player.View.SendView(stream.ItemPacketCreate(spell.FloorPacket), true);

                                            }
                                        }
                                    }
                                    while (RemoveSpells.Count > 0)
                                        spellclient.RemoveItem(RemoveSpells.Dequeue());

                                    if (spellclient.Spells.Count == 0)
                                    {
                                        Role.FloorSpell.ClientFloorSpells FloorSpell;
                                        client.Player.FloorSpells.TryRemove(spellclient.DBSkill.ID, out FloorSpell);
                                    }
                                    break;
                                }
                            case (ushort)Role.Flags.SpellID.HorrorofStomper:
                                {
                                    var spellclient = ID.Value;
                                    Queue<Role.FloorSpell> RemoveSpells = new Queue<Role.FloorSpell>();

                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        foreach (var spell in spellclient.Spells.ToArray())
                                        {
                                            if (spellclient.CheckInvocke(Now, spell))
                                            {
                                                uint Experience = 0;
                                                RemoveSpells.Enqueue(spell);


                                                client.Player.View.SendView(stream.ItemPacketCreate(spell.FloorPacket), true);

                                                spellclient.CreateMsgSpell(0);
                                                spellclient.SpellPacket.bomb = 1;
                                                spellclient.SpellPacket.UID = spell.FloorPacket.m_UID;
                                                spellclient.SpellPacket.X = spell.FloorPacket.OwnerX;
                                                spellclient.SpellPacket.Y = spell.FloorPacket.OwnerY;
                                                spellclient.SpellPacket.SpellLevel = spell.DBSkill.Level;


                                                var line = new Game.MsgServer.AttackHandler.Algoritms.Line(spell.FloorPacket.OwnerX, spell.FloorPacket.OwnerY, spell.FloorPacket.m_X, spell.FloorPacket.m_Y, 9);
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Monster))
                                                {
                                                    var monster = obj as Game.MsgMonster.MonsterRole;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) < 9)
                                                    {
                                                        if (line.InLine(obj.X, obj.Y))
                                                        {
                                                            if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackMonster.Verified(client, monster, spell.DBSkill))
                                                            {
                                                                MsgSpellAnimation MsgSpell = new MsgSpellAnimation(client.Player.UID
                                            , 0, spell.FloorPacket.OwnerX, spell.FloorPacket.OwnerY, spellclient.DBSkill.ID
                                            , spellclient.DBSkill.Level, 0);
                                                                Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                                Game.MsgServer.AttackHandler.Calculate.Physical.OnMonster(client.Player, monster, spell.DBSkill, out AnimationObj);
                                                                Experience += Game.MsgServer.AttackHandler.ReceiveAttack.Monster.Execute(stream, AnimationObj, client, monster);
                                                                AnimationObj.Damage = Game.MsgServer.AttackHandler.Calculate.Base.CalculateSoul(AnimationObj.Damage, spellclient.LevelHu);

                                                                MsgSpell.Targets.Enqueue(AnimationObj);

                                                                MsgSpell.SetStream(stream);
                                                                MsgSpell.Send(monster);
                                                            }
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Player))
                                                {
                                                    var target = obj as Role.Player;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= 9)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackPlayer.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Range.OnPlayer(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Player.Execute(AnimationObj, client, target);
                                                            AnimationObj.Damage = Game.MsgServer.AttackHandler.Calculate.Base.CalculateSoul(AnimationObj.Damage, spellclient.LevelHu);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }


                                                spellclient.SendView(stream, client);




                                                Game.MsgServer.AttackHandler.Updates.IncreaseExperience.Up(stream, client, Experience);

                                                spell.FloorPacket.DropType = Game.MsgFloorItem.MsgDropID.RemoveEffect;

                                                client.Player.View.SendView(stream.ItemPacketCreate(spell.FloorPacket), true);


                                            }
                                        }
                                    }
                                    while (RemoveSpells.Count > 0)
                                        spellclient.RemoveItem(RemoveSpells.Dequeue());

                                    if (spellclient.Spells.Count == 0)
                                    {
                                        Role.FloorSpell.ClientFloorSpells FloorSpell;
                                        client.Player.FloorSpells.TryRemove(spellclient.DBSkill.ID, out FloorSpell);
                                    }
                                    break;
                                }
                            case (ushort)Role.Flags.SpellID.PeaceofStomper:
                                {
                                    var spellclient = ID.Value;
                                    Queue<Role.FloorSpell> RemoveSpells = new Queue<Role.FloorSpell>();

                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        foreach (var spell in spellclient.Spells.ToArray())
                                        {
                                            if (spellclient.CheckInvocke(Now, spell))
                                            {
                                                uint Experience = 0;
                                                RemoveSpells.Enqueue(spell);

                                                spellclient.X = spell.FloorPacket.m_X;
                                                spellclient.Y = spell.FloorPacket.m_Y;

                                                spellclient.CreateMsgSpell(0);


                                                spellclient.SpellPacket.bomb = 1;


                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Monster))
                                                {
                                                    var monster = obj as Game.MsgMonster.MonsterRole;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= 4)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackMonster.Verified(client, monster, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnMonster(client.Player, monster, spell.DBSkill, out AnimationObj);
                                                            Experience += Game.MsgServer.AttackHandler.ReceiveAttack.Monster.Execute(stream, AnimationObj, client, monster);
                                                            AnimationObj.Damage = Game.MsgServer.AttackHandler.Calculate.Base.CalculateSoul(AnimationObj.Damage, spellclient.LevelHu);

                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                spellclient.SendView(stream, client);


                                                Game.MsgServer.AttackHandler.Updates.IncreaseExperience.Up(stream, client, Experience);

                                                spell.FloorPacket.DropType = Game.MsgFloorItem.MsgDropID.RemoveEffect;

                                                client.Player.View.SendView(stream.ItemPacketCreate(spell.FloorPacket), true);

                                            }
                                        }
                                    }
                                    while (RemoveSpells.Count > 0)
                                        spellclient.RemoveItem(RemoveSpells.Dequeue());

                                    if (spellclient.Spells.Count == 0)
                                    {
                                        Role.FloorSpell.ClientFloorSpells FloorSpell;
                                        client.Player.FloorSpells.TryRemove(spellclient.DBSkill.ID, out FloorSpell);


                                    }
                                    break;
                                }
                            #endregion
                            #region RageOfWar
                            case (ushort)Role.Flags.SpellID.RageofWar:
                                {
                                    var spellclient = ID.Value;
                                    Queue<Role.FloorSpell> RemoveSpells = new Queue<Role.FloorSpell>();

                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        var spells = spellclient.Spells.ToArray();
                                        foreach (var spell in spells)
                                        {
                                            if (spellclient.CheckInvocke(Now, spell))
                                            {
                                                uint Experience = 0;
                                                RemoveSpells.Enqueue(spell);
                                                //if (Attack)
                                                spellclient.X = spell.FloorPacket.m_X;
                                                spellclient.Y = spell.FloorPacket.m_Y;

                                                spellclient.CreateMsgSpell(0);


                                                spellclient.SpellPacket.bomb = 1;
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Monster))
                                                {
                                                    var monster = obj as Game.MsgMonster.MonsterRole;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= 3)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackMonster.Verified(client, monster, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnMonster(client.Player, monster, spell.DBSkill, out AnimationObj);
                                                            Experience += Game.MsgServer.AttackHandler.ReceiveAttack.Monster.Execute(stream, AnimationObj, client, monster);
                                                            AnimationObj.Damage = Game.MsgServer.AttackHandler.Calculate.Base.CalculateSoul(AnimationObj.Damage / 5, spellclient.LevelHu);

                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Player))
                                                {
                                                    var target = obj as Role.Player;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= 3)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackPlayer.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnPlayer(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            AnimationObj.Damage = AnimationObj.Damage * 70 / 100;
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Player.Execute(AnimationObj, client, target);
                                                            AnimationObj.Damage = Game.MsgServer.AttackHandler.Calculate.Base.CalculateSoul(AnimationObj.Damage, spellclient.LevelHu);

                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.SobNpc))
                                                {
                                                    var target = obj as Role.SobNpc;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= 3)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackNpc.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnNpcs(client.Player, target, spell.DBSkill, out AnimationObj);


                                                            Experience += Game.MsgServer.AttackHandler.ReceiveAttack.Npc.Execute(stream, AnimationObj, client, target);


                                                            AnimationObj.Damage = Game.MsgServer.AttackHandler.Calculate.Base.CalculateSoul(AnimationObj.Damage, spellclient.LevelHu);
                                                            AnimationObj.Hit = 1;//??

                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                spellclient.SendView(stream, client);

                                                Game.MsgServer.AttackHandler.Updates.IncreaseExperience.Up(stream, client, Experience);

                                                spell.FloorPacket.DropType = Game.MsgFloorItem.MsgDropID.RemoveEffect;

                                                foreach (var user in spellclient.GMap.View.Roles(Role.MapObjectType.Player, spellclient.X, spellclient.Y,
                                                     p => Role.Core.GetDistance(p.X, p.Y, spellclient.X, spellclient.Y) <= 18))
                                                    user.Send(stream.ItemPacketCreate(spell.FloorPacket));
                                            }
                                        }
                                    }
                                    while (RemoveSpells.Count > 0)
                                        spellclient.RemoveItem(RemoveSpells.Dequeue());

                                    if (spellclient.Spells.Count == 0)
                                    {
                                        Role.FloorSpell.ClientFloorSpells FloorSpell;
                                        client.Player.FloorSpells.TryRemove(spellclient.DBSkill.ID, out FloorSpell);
                                    }

                                    break;
                                }
                            #endregion
                            #region InfernalEcho
                            case (ushort)Role.Flags.SpellID.InfernalEcho:
                                {
                                    var spellclient = ID.Value;
                                    Queue<Role.FloorSpell> RemoveSpells = new Queue<Role.FloorSpell>();

                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        var spells = spellclient.Spells.ToArray();
                                        foreach (var spell in spells)
                                        {
                                            if (spellclient.CheckInvocke(Now, spell))
                                            {
                                                uint Experience = 0;
                                                RemoveSpells.Enqueue(spell);

                                                spellclient.X = spell.FloorPacket.m_X;
                                                spellclient.Y = spell.FloorPacket.m_Y;

                                                spellclient.CreateMsgSpell(0);


                                                spellclient.SpellPacket.bomb = 1;
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Monster))
                                                {
                                                    var monster = obj as Game.MsgMonster.MonsterRole;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= (int)(ID.Key == (ushort)Role.Flags.SpellID.InfernalEcho ? 2 : 3))
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackMonster.Verified(client, monster, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnMonster(client.Player, monster, spell.DBSkill, out AnimationObj);
                                                            Experience += Game.MsgServer.AttackHandler.ReceiveAttack.Monster.Execute(stream, AnimationObj, client, monster);
                                                            AnimationObj.Damage = Game.MsgServer.AttackHandler.Calculate.Base.CalculateSoul(AnimationObj.Damage, spellclient.LevelHu);

                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Player))
                                                {
                                                    var target = obj as Role.Player;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= (int)(ID.Key == (ushort)Role.Flags.SpellID.InfernalEcho ? 2 : 3))
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackPlayer.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnPlayer(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Player.Execute(AnimationObj, client, target);
                                                            AnimationObj.Damage = Game.MsgServer.AttackHandler.Calculate.Base.CalculateSoul(AnimationObj.Damage, spellclient.LevelHu);
                                                            AnimationObj.Damage = AnimationObj.Damage * 95 / 100;
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.SobNpc))
                                                {
                                                    var target = obj as Role.SobNpc;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= (int)(ID.Key == (ushort)Role.Flags.SpellID.InfernalEcho ? 2 : 3))
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackNpc.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnNpcs(client.Player, target, spell.DBSkill, out AnimationObj);


                                                            Experience += Game.MsgServer.AttackHandler.ReceiveAttack.Npc.Execute(stream, AnimationObj, client, target);


                                                            AnimationObj.Damage = Game.MsgServer.AttackHandler.Calculate.Base.CalculateSoul(AnimationObj.Damage, spellclient.LevelHu);
                                                            AnimationObj.Hit = 1;//??

                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                spellclient.SendView(stream, client);

                                                Game.MsgServer.AttackHandler.Updates.IncreaseExperience.Up(stream, client, Experience);

                                                spell.FloorPacket.DropType = Game.MsgFloorItem.MsgDropID.RemoveEffect;

                                                foreach (var user in spellclient.GMap.View.Roles(Role.MapObjectType.Player, spellclient.X, spellclient.Y,
                                                     p => Role.Core.GetDistance(p.X, p.Y, spellclient.X, spellclient.Y) <= 18))
                                                    user.Send(stream.ItemPacketCreate(spell.FloorPacket));
                                            }
                                        }
                                    }
                                    while (RemoveSpells.Count > 0)
                                        spellclient.RemoveItem(RemoveSpells.Dequeue());

                                    if (spellclient.Spells.Count == 0)
                                    {
                                        Role.FloorSpell.ClientFloorSpells FloorSpell;
                                        client.Player.FloorSpells.TryRemove(spellclient.DBSkill.ID, out FloorSpell);
                                    }

                                    break;
                                }
                            #endregion
                            #region  WrathOfTheEmperor
                            case (ushort)Role.Flags.SpellID.WrathoftheEmperor:
                                {
                                    var spellclient = ID.Value;
                                    Queue<Role.FloorSpell> RemoveSpells = new Queue<Role.FloorSpell>();

                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        var spells = spellclient.Spells.ToArray();
                                        foreach (var spell in spells)
                                        {
                                            if (spellclient.CheckInvocke(Now, spell))
                                            {
                                                uint Experience = 0;
                                                RemoveSpells.Enqueue(spell);

                                                spellclient.X = spell.FloorPacket.m_X;
                                                spellclient.Y = spell.FloorPacket.m_Y;

                                                spellclient.CreateMsgSpell(0);


                                                spellclient.SpellPacket.bomb = 1;
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Monster))
                                                {
                                                    var monster = obj as Game.MsgMonster.MonsterRole;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= (int)(ID.Key == (ushort)Role.Flags.SpellID.WrathoftheEmperor ? 2 : 3))
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackMonster.Verified(client, monster, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnMonster(client.Player, monster, spell.DBSkill, out AnimationObj);
                                                            Experience += Game.MsgServer.AttackHandler.ReceiveAttack.Monster.Execute(stream, AnimationObj, client, monster);
                                                            AnimationObj.Damage = Game.MsgServer.AttackHandler.Calculate.Base.CalculateSoul(AnimationObj.Damage, spellclient.LevelHu);

                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Player))
                                                {
                                                    var target = obj as Role.Player;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= (int)(ID.Key == (ushort)Role.Flags.SpellID.WrathoftheEmperor ? 2 : 3))
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackPlayer.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnPlayer(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Player.Execute(AnimationObj, client, target);
                                                            AnimationObj.Damage = Game.MsgServer.AttackHandler.Calculate.Base.CalculateSoul(AnimationObj.Damage, spellclient.LevelHu);

                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.SobNpc))
                                                {
                                                    var target = obj as Role.SobNpc;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= (int)(ID.Key == (ushort)Role.Flags.SpellID.WrathoftheEmperor ? 2 : 3))
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackNpc.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnNpcs(client.Player, target, spell.DBSkill, out AnimationObj);


                                                            Experience += Game.MsgServer.AttackHandler.ReceiveAttack.Npc.Execute(stream, AnimationObj, client, target);

                                                            AnimationObj.Damage = Game.MsgServer.AttackHandler.Calculate.Base.CalculateSoul(AnimationObj.Damage, spellclient.LevelHu);
                                                            AnimationObj.Hit = 1;//??

                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                spellclient.SendView(stream, client);

                                                Game.MsgServer.AttackHandler.Updates.IncreaseExperience.Up(stream, client, Experience);

                                                spell.FloorPacket.DropType = Game.MsgFloorItem.MsgDropID.RemoveEffect;

                                                foreach (var user in spellclient.GMap.View.Roles(Role.MapObjectType.Player, spellclient.X, spellclient.Y,
                                                     p => Role.Core.GetDistance(p.X, p.Y, spellclient.X, spellclient.Y) <= 18))
                                                    user.Send(stream.ItemPacketCreate(spell.FloorPacket));
                                            }
                                        }
                                    }
                                    while (RemoveSpells.Count > 0)
                                        spellclient.RemoveItem(RemoveSpells.Dequeue());

                                    if (spellclient.Spells.Count == 0)
                                    {
                                        Role.FloorSpell.ClientFloorSpells FloorSpell;
                                        client.Player.FloorSpells.TryRemove(spellclient.DBSkill.ID, out FloorSpell);
                                    }

                                    break;
                                }
                            #endregion
                            #region TwilightDance
                            case (ushort)Role.Flags.SpellID.TwilightDance:
                                {
                                    var spellclient = ID.Value;
                                    Queue<Role.FloorSpell> RemoveSpells = new Queue<Role.FloorSpell>();

                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        var spells = spellclient.Spells.ToArray();
                                        foreach (var spell in spells)
                                        {
                                            if (spellclient.CheckInvocke(Now, spell))
                                            {
                                                uint Experience = 0;

                                                RemoveSpells.Enqueue(spell);
                                                spellclient.CreateMsgSpell(client.Player.UID);

                                                int increased_attack = 0;

                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Monster))
                                                {
                                                    var monster = obj as Game.MsgMonster.MonsterRole;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= 3)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackMonster.Verified(client, monster, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnMonster(client.Player, monster, spell.DBSkill, out AnimationObj);
                                                            Experience += Game.MsgServer.AttackHandler.ReceiveAttack.Monster.Execute(stream, AnimationObj, client, monster);
                                                            AnimationObj.Damage = Game.MsgServer.AttackHandler.Calculate.Base.CalculateSoul(AnimationObj.Damage, spellclient.LevelHu);

                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Player))
                                                {
                                                    var target = obj as Role.Player;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= 3)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackPlayer.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnPlayer(client.Player, target, spell.DBSkill, out AnimationObj, true, increased_attack);
                                                            if (target.Owner.Player.Intensify == true)
                                                                target.Owner.Player.Intensify = false;
                                                            if (spellclient.Spells.Count == 3)
                                                                AnimationObj.Damage = AnimationObj.Damage * 105 / 100;
                                                            else if (spellclient.Spells.Count == 2)
                                                                AnimationObj.Damage = AnimationObj.Damage * 110 / 100;
                                                            else if (spellclient.Spells.Count == 1)
                                                                AnimationObj.Damage = AnimationObj.Damage * 120 / 100;
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Player.Execute(AnimationObj, client, target);
                                                            AnimationObj.Damage = Game.MsgServer.AttackHandler.Calculate.Base.CalculateSoul(AnimationObj.Damage, spellclient.LevelHu);

                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.SobNpc))
                                                {
                                                    var target = obj as Role.SobNpc;
                                                    if (Role.Core.GetDistance(client.Player.X, client.Player.Y, target.X, target.Y) < 18)
                                                    {
                                                        if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= 3)
                                                        {
                                                            if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackNpc.Verified(client, target, spell.DBSkill))
                                                            {
                                                                Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                                Game.MsgServer.AttackHandler.Calculate.Physical.OnNpcs(client.Player, target, spell.DBSkill, out AnimationObj);

                                                                Experience += Game.MsgServer.AttackHandler.ReceiveAttack.Npc.Execute(stream, AnimationObj, client, target);

                                                                AnimationObj.Damage = Game.MsgServer.AttackHandler.Calculate.Base.CalculateSoul(AnimationObj.Damage, spellclient.LevelHu);
                                                                AnimationObj.Hit = 1;

                                                                spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                            }
                                                        }
                                                    }
                                                }
                                                spellclient.SendView(stream, client);

                                                ActionQuery action = new ActionQuery()
                                                {
                                                    ObjId = spell.FloorPacket.ItemOwnerUID,
                                                    dwParam_Hi = spell.FloorPacket.m_Y,
                                                    dwParam_Lo = spell.FloorPacket.m_X,
                                                    wParam1 = spell.FloorPacket.OwnerX,
                                                    wParam2 = spell.FloorPacket.OwnerY,
                                                    Type = ActionType.RemoveTrap
                                                };

                                                //client.Player.View.SendView(stream.ActionCreate(&action), false);

                                                spell.FloorPacket.DropType = Game.MsgFloorItem.MsgDropID.RemoveEffect;

                                                foreach (var user in spellclient.GMap.View.Roles(Role.MapObjectType.Player, spellclient.X, spellclient.Y,
                                                 p => Role.Core.GetDistance(p.X, p.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= 18))
                                                {
                                                    if (user.DynamicID == client.Player.DynamicID)
                                                    {
                                                        user.Send(stream.ActionCreate(&action));

                                                        user.Send(stream.ItemPacketCreate(spell.FloorPacket));
                                                    }
                                                }

                                                Game.MsgServer.AttackHandler.Updates.IncreaseExperience.Up(stream, client, Experience);

                                            }

                                        }
                                    }
                                    while (RemoveSpells.Count > 0)
                                        spellclient.RemoveItem(RemoveSpells.Dequeue());

                                    if (spellclient.Spells.Count == 0)
                                    {
                                        Role.FloorSpell.ClientFloorSpells FloorSpell;
                                        client.Player.FloorSpells.TryRemove(spellclient.DBSkill.ID, out FloorSpell);
                                    }
                                    break;
                                }
                                #endregion
                        }
                    }
                }
                #endregion
                #region CheckItems
                foreach (var item in client.Player.View.Roles(Role.MapObjectType.Item))
                {
                    if (item.Alive == false)
                    {
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            var PItem = item as Game.MsgFloorItem.MsgItem;
                            if (PItem.IsTrap())
                            {
                                if (Game.MsgTournaments.MsgSchedules.CurrentTournament.Process == Game.MsgTournaments.ProcesType.Alive)
                                {
                                    //if (PItem.ItemBase.ITEM_ID == Game.MsgFloorItem.MsgItemPacket.DBShowerEffect)
                                    {

                                        if (Game.MsgTournaments.MsgSchedules.CurrentTournament.Type == Game.MsgTournaments.TournamentType.DBShower)
                                        {
                                            var tournament = Game.MsgTournaments.MsgSchedules.CurrentTournament as Game.MsgTournaments.MsgDBShower;
                                            tournament.DropDragonBall(PItem.X, PItem.Y, stream);
                                        }

                                    }
                                }
                                PItem.SendAll(stream, MsgDropID.RemoveEffect);
                            }
                            else
                                PItem.SendAll(stream, MsgDropID.Remove);
                            client.Map.View.LeaveMap<Role.IMapObj>(PItem);
                        }
                    }
                    else if (item.IsTrap())
                    {
                        if (client.Player.Map == 4006 && client.Player.JoinTowerOfMysteryLayer == 7)
                        {
                            if (!(Role.Core.GetDistance(client.Player.X, client.Player.Y, 44, 62) <= 3))
                            {
                                if (Role.Core.GetDistance(client.Player.X, client.Player.Y, item.X, item.Y) <= 2)
                                {

                                    if (DateTime.Now > client.Player.TowerOfMysteryFrezeeStamp)
                                    {
                                        client.Player.AddFlag(MsgUpdate.Flags.Freeze, 3, true);
                                        client.Player.TowerOfMysteryFrezeeStamp = DateTime.Now.AddSeconds(5);
                                    }

                                }
                                foreach (var user in client.Player.View.Roles(Role.MapObjectType.Player))
                                {
                                    if (Role.Core.GetDistance(user.X, user.Y, item.X, item.Y) <= 2)
                                    {
                                        var _user = user as Role.Player;
                                        if (DateTime.Now > _user.TowerOfMysteryFrezeeStamp)
                                        {
                                            _user.AddFlag(MsgUpdate.Flags.Freeze, 3, true);
                                            _user.TowerOfMysteryFrezeeStamp = DateTime.Now.AddSeconds(5);
                                        }

                                    }
                                }
                            }
                        }
                        var FloorItem = item as Game.MsgFloorItem.MsgItem;
                        if (FloorItem.ItemBase == null)
                            continue;
                        if (FloorItem.ItemBase.ITEM_ID == Game.MsgFloorItem.MsgItemPacket.NormalDaggerStorm
                           || FloorItem.ItemBase.ITEM_ID == Game.MsgFloorItem.MsgItemPacket.SoulOneDaggerStorm
                           || FloorItem.ItemBase.ITEM_ID == Game.MsgFloorItem.MsgItemPacket.SoulTwoDaggerStorm)
                        {
                            if (Now > FloorItem.AttackStamp.AddMilliseconds(800))
                            {
                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    foreach (var _monster in FloorItem.GMap.View.Roles(Role.MapObjectType.Monster, FloorItem.X, FloorItem.Y
                                        , p => Role.Core.GetDistance(p.X, p.Y, FloorItem.MsgFloor.m_X, FloorItem.MsgFloor.m_Y) <= 3))
                                    {
                                        var monster = _monster as Game.MsgMonster.MonsterRole;
                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackMonster.Verified(FloorItem.OwnerEffert, monster, FloorItem.DBSkill))
                                        {
                                            InteractQuery action = new InteractQuery()
                                            {
                                                AtkType = MsgAttackPacket.AttackID.Physical,
                                                X = monster.X,
                                                Y = monster.Y,
                                                // UID = FloorItem.OwnerEffert.Player.UID,
                                                OpponentUID = monster.UID
                                            };


                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                            Game.MsgServer.AttackHandler.Calculate.Range.OnMonster(FloorItem.OwnerEffert.Player, monster, FloorItem.DBSkill, out AnimationObj);
                                            Game.MsgServer.AttackHandler.ReceiveAttack.Monster.Execute(stream, AnimationObj, FloorItem.OwnerEffert, monster);
                                            //AnimationObj.Damage = Game.MsgServer.AttackHandler.Calculate.Base.CalculateSoul(AnimationObj.Damage, FloorItem.SpellSoul);
                                            AnimationObj.Damage = AnimationObj.Damage * 10 / 100;
                                            action.Damage = (int)AnimationObj.Damage;
                                            action.Effect = AnimationObj.Effect;


                                            monster.Send(stream.InteractionCreate(&action));

                                        }
                                    }
                                    foreach (var player in FloorItem.GMap.View.Roles(Role.MapObjectType.Player, FloorItem.X, FloorItem.Y
                                        , p => Game.MsgServer.AttackHandler.Calculate.Base.GetDistance(p.X, p.Y, FloorItem.MsgFloor.m_X, FloorItem.MsgFloor.m_Y) <= 3))
                                    {
                                        if (player.UID != FloorItem.OwnerEffert.Player.UID)
                                        {
                                            var atacked = player as Role.Player;
                                            if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackPlayer.Verified(FloorItem.OwnerEffert, atacked, FloorItem.DBSkill))
                                            {

                                                Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                Game.MsgServer.AttackHandler.Calculate.Range.OnPlayer(FloorItem.OwnerEffert.Player, atacked, FloorItem.DBSkill, out AnimationObj);
                                                Game.MsgServer.AttackHandler.ReceiveAttack.Player.Execute(AnimationObj, FloorItem.OwnerEffert, atacked);
                                                AnimationObj.Damage = Game.MsgServer.AttackHandler.Calculate.Base.CalculateSoul(AnimationObj.Damage, FloorItem.SpellSoul);

                                                InteractQuery action = new InteractQuery()
                                                {
                                                    AtkType = MsgAttackPacket.AttackID.Physical,
                                                    X = atacked.X,
                                                    Y = atacked.Y,
                                                    //  UID = FloorItem.OwnerEffert.Player.UID,
                                                    OpponentUID = atacked.UID
                                                };

                                                action.Damage = (int)AnimationObj.Damage;
                                                action.Effect = AnimationObj.Effect;

                                                atacked.View.SendView(stream.InteractionCreate(&action), true);

                                            }
                                        }

                                    }
                                }
                                FloorItem.AttackStamp = Now;
                            }
                        }
                    }
                }
                #endregion
            }
            catch (Exception e)
            {
                MyConsole.WriteException(e);
            }
        }
        public static unsafe void SecondsCallback(Client.GameClient client)
        {
            try
            {
                if (client == null || !client.FullLoading || client.Player == null || client.Player.CompleteLogin == false)
                    return;
                #region Remove Rider
                if (Program.RemoveRide.Contains(client.Player.Map) || (Game.AISystem.UnlimitedArenaRooms.Maps.ContainsValue(client.Player.DynamicID) && client.Player.fbss == 1))
                {
                    if (client.Player.ContainFlag(MsgUpdate.Flags.Ride))
                    {
                        client.Player.RemoveFlag(MsgUpdate.Flags.Ride);
                    }
                }
                #endregion
                if (client.Player.ConquerPointDropLimitLayer == 0)
                {
                    client.Player.ConquerPointDropLimitLayer = 1;
                }
                if (client.Player.ConquerPointDropLimitLayer > 1)
                {
                    if (DateTime.Now >= client.Player.ConquerPointsDropStamp)
                    {
                        client.Player.ConquerPointDropLimitLayer = 1;
                        client.Player.ConquerPointsDropStamp = DateTime.FromBinary(0);
                    }
                }
                Extensions.Time32 timer = Extensions.Time32.Now;
                DateTime Now64 = DateTime.Now;
                #region Room Flag WindWalker
                if (client.Player.Map == 5051 || client.Player.Map == 5053 || client.Player.Map == 5054 || client.Player.Map == 5055 || client.Player.Map == 5056 || client.Player.Map == 5057 || client.Player.Map == 5058)
                {
                    if (client.Player.ContainFlag(MsgUpdate.Flags.HealingSnow))
                    {
                        client.Player.RemoveFlag(MsgUpdate.Flags.HealingSnow);
                    }
                }
                if (client.Player.Map == 5051 || client.Player.Map == 5053 || client.Player.Map == 5054 || client.Player.Map == 5055 || client.Player.Map == 5056 || client.Player.Map == 5057 || client.Player.Map == 5058)
                {
                    if (client.Player.ContainFlag(MsgUpdate.Flags.FreezingPelter))
                    {
                        client.Player.RemoveFlag(MsgUpdate.Flags.FreezingPelter);
                    }
                }
                #endregion
                #region OnlineMinutes
                if (timer > client.Player.OnlineStamp.AddMinutes(1))
                {
                    client.Player.OnlineMinutes += 1;
                    client.Player.OnlineStamp = Extensions.Time32.Now;
                }
                #endregion
                //#region AutoHuntMinutes
                //if (client.Player.VipLevel < 4)
                //{
                //    if (client.Player.OnAutoHunt == true)
                //    {
                //        if (timer > client.Player.AutoHuntStamp.AddMinutes(1))
                //        {
                //            client.Player.AutoHuntMinutes += 1;
                //            client.Player.AutoHuntStamp = Extensions.Time32.Now;
                //        }
                //    }
                //}
                //if(client.Player.VipLevel==1 && client.Player.AutoHuntMinutes >= 180 )
                //{
                //    if (client.Player.OnAutoHunt == true)
                //    {
                //        using (var rec = new ServerSockets.RecycledPacket())
                //        {
                //            var stream = rec.GetStream();
                //            client.Send(stream.AutoHuntCreate(3, 0));
                //            client.Player.OnAutoHunt = false;
                //        }

                //        client.CreateBoxDialog("Sorry,You have used up the time Of AutoHunting [3 Hours] you are allowed during the day ,Up Your Vip Level To Be UnLimited");
                //    }
                //}
                //if (client.Player.VipLevel == 2 && client.Player.AutoHuntMinutes >= 360)
                //{
                //    if (client.Player.OnAutoHunt == true)
                //    {
                //        using (var rec = new ServerSockets.RecycledPacket())
                //        {
                //            var stream = rec.GetStream();
                //            client.Send(stream.AutoHuntCreate(3, 0));
                //            client.Player.OnAutoHunt = false;
                //        }
                //        client.CreateBoxDialog("Sorry,You have used up the time Of AutoHunting [6 Hours] you are allowed during the day ,Up Your Vip Level To Be UnLimited");

                //    }
                //}
                //if (client.Player.VipLevel == 3 && client.Player.AutoHuntMinutes >= 720)
                //{
                //    if (client.Player.OnAutoHunt == true)
                //    {
                //        using (var rec = new ServerSockets.RecycledPacket())
                //        {
                //            var stream = rec.GetStream();
                //            client.Send(stream.AutoHuntCreate(3, 0));
                //            client.Player.OnAutoHunt = false;
                //        }
                //        client.CreateBoxDialog("Sorry,You have used up the time Of AutoHunting [12 Hours] you are allowed during the day ,Up Your Vip Level To Be UnLimited");
                //    }
                //}
                //#endregion
                #region Ninja Custom
                if ((client.Player.Map == 8603 || client.Player.Map == 8604) && client.Player.LeftWeaponId == 0)
                {
                    if (client.Player.SpecialitemR == 0)
                    {
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            client.Player.AddSpecialitemR(stream, 616439);
                            client.Player.RightWeapsonSoul = 0;
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.SuperTwofoldBlade))
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.SuperTwofoldBlade, 4);
                        }
                    }
                }
                if (client.Player.Map == 8603 || client.Player.Map == 8604)
                {
                    if (client.Player.SpecialitemL == 0 && client.Player.SpecialitemR == 0 && client.Player.LeftWeaponId != 0)
                    {
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            client.Player.AddSpecialitemR(stream, 616439);
                            client.Player.AddSpecialitemL(stream, 616439);
                            client.Player.RightWeapsonSoul = 0;
                            client.Player.LeftWeapsonSoul = 0;
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.SuperTwofoldBlade))
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.SuperTwofoldBlade, 4);
                        }
                    }
                }
                #endregion
                #region FB/SS Events
                if (Game.AISystem.UnlimitedArenaRooms.Maps.ContainsValue(client.Player.DynamicID) && client.Player.fbss == 50)
                {
                    if (client.Player.SpecialitemL == 0 && client.Player.SpecialitemR == 0 && client.Player.LeftWeaponId != 0)
                    {
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            client.Player.AddSpecialitemR(stream, 410239);
                            client.Player.AddSpecialitemL(stream, 420239);
                            client.Player.RightWeapsonSoul = 0;
                            client.Player.LeftWeapsonSoul = 0;
                            client.Player.SetPkMode(Role.Flags.PKMode.PK);
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Hercules))
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Hercules, 4);
                        }
                    }
                }
                if (!Game.AISystem.UnlimitedArenaRooms.Maps.ContainsValue(client.Player.DynamicID))
                {
                    if (!Database.AtributesStatus.IsTrojan(client.Player.Class))
                    {
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Hercules))
                                client.MySpells.Remove((ushort)Role.Flags.SpellID.Hercules, stream);
                        }
                    }
                }
                #region FB/SS 1 Hand
                if ((Game.AISystem.UnlimitedArenaRooms.Maps.ContainsValue(client.Player.DynamicID) && client.Player.fbss == 10) || (Game.AISystem.UnlimitedArenaRooms.Maps.ContainsValue(client.Player.DynamicID) && client.Player.fbss == 1) || Program.FBSSAuto.Contains(client.Player.Map) || client.Player.Map == 9573)
                {
                    if (client.Player.SpecialitemR == 0 && client.Player.LeftWeaponId == 0)
                    {
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            client.Player.AddSpecialitemR(stream, 410239);
                            client.Player.RightWeapsonSoul = 0;
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FastBlader))
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.FastBlader, 4);
                        }
                    }
                }
                #endregion
                #region FB/SS 2 Hand
                if ((Game.AISystem.UnlimitedArenaRooms.Maps.ContainsValue(client.Player.DynamicID) && client.Player.fbss == 10) || (Game.AISystem.UnlimitedArenaRooms.Maps.ContainsValue(client.Player.DynamicID) && client.Player.fbss == 1) || Program.FBSSAuto.Contains(client.Player.Map) || client.Player.Map == 9573)
                {
                    if (client.Player.SpecialitemL == 0 && client.Player.SpecialitemR == 0 && client.Player.LeftWeaponId != 0)
                    {
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            //Equip.EquipBLade(stream, client, 420239, ConquerItem.AleternanteLeftWeapon);//RoyalSword
                            //Equip.EquipBLade(stream, client, 410239, ConquerItem.AleternanteRightWeapon);//FrostBlade
                            //client.Equipment.QueryEquipment(true);
                            //client.Player.View.SendView(client.Player.GetArray(stream, false), true);
                            client.Player.AddSpecialitemR(stream, 410239);
                            client.Player.AddSpecialitemL(stream, 420239);
                            client.Player.RightWeapsonSoul = 0;
                            client.Player.LeftWeapsonSoul = 0;
                            client.Player.SetPkMode(Role.Flags.PKMode.PK);
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ScrenSword))
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.ScrenSword, 4);
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FastBlader))
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.FastBlader, 4);
                        }
                    }
                }
                #endregion
                #region OUT FB/SS
                if (!Game.AISystem.UnlimitedArenaRooms.Maps.ContainsValue(client.Player.DynamicID) && !Program.FBSSAuto.Contains(client.Player.Map) && client.Player.Map != 1616 && client.Player.Map != 8604 && client.Player.Map != 8603 && client.Player.Map != 9573)
                {
                    if (client.Player.SpecialitemR != 0)
                    {
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            client.Player.RemoveSpecialitem(stream);
                            client.Player.RemoveSpecialitem1(stream);
                        }
                    }
                }
                if (client.Player.Map != 8604 && client.Player.Map != 8603)
                {
                    if (!Database.AtributesStatus.IsNinja(client.Player.Class))
                    {
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.SuperTwofoldBlade))
                                client.MySpells.Remove((ushort)Role.Flags.SpellID.SuperTwofoldBlade, stream);
                        }
                    }
                }
                #endregion
                #endregion
                #region Nobility
                if (client.Player.Nobility.PaidPeriod < DateTime.Now)
                {
                    if (client.Player.Nobility.PaidRank != Role.Instance.Nobility.NobilityRank.Serf)
                    {
                        client.Player.Nobility.Donation = client.Player.Nobility.DonationToBack;
                        client.Player.Nobility.DonationToBack = 0;
                        client.Player.Nobility.PaidRank = Role.Instance.Nobility.NobilityRank.Serf;
                        Program.NobilityRanking.UpdateRank(client.Player.Nobility);
                        client.Player.NobilityRank = client.Player.Nobility.Rank;
                        client.Equipment.QueryEquipment(client.Equipment.Alternante, false);
                        using (var rect = new ServerSockets.RecycledPacket())
                        {
                            var stream = rect.GetStream();
                            client.Send(stream.NobilityIconCreate(client.Player.Nobility));
                        }
                    }
                }
                #endregion
                #region vip Update
                if (client.Player.VipLevel == 1)
                {
                    if (client.Player.VipPointsD >= 1000)
                    {
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            client.Player.VipPointsD -= 1000;
                            client.Player.VipLevel = 2;
                            client.Player.SendUpdate(stream, client.Player.VipLevel, MsgUpdate.DataType.VIPLevel);
                            client.Player.UpdateVip(stream);
                            client.CreateBoxDialog("Congratulations! You upgrade Your Vip Level Top Level 2");

                        }
                    }

                }
                if (client.Player.VipLevel == 2)
                {
                    if (client.Player.VipPointsD >= 2000)
                    {
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            client.Player.VipPointsD -= 2000;
                            client.Player.VipLevel = 3;
                            client.Player.SendUpdate(stream, client.Player.VipLevel, MsgUpdate.DataType.VIPLevel);
                            client.Player.UpdateVip(stream);
                            client.CreateBoxDialog("Congratulations! You upgrade Your Vip Level Top Level 3");

                        }
                    }

                }
                if (client.Player.VipLevel == 3)
                {
                    if (client.Player.VipPointsD >= 4000)
                    {
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            client.Player.VipPointsD -= 4000;
                            client.Player.VipLevel = 4;
                            client.Player.SendUpdate(stream, client.Player.VipLevel, MsgUpdate.DataType.VIPLevel);
                            client.Player.UpdateVip(stream);
                            client.CreateBoxDialog("Congratulations! You upgrade Your Vip Level Top Level 4");

                        }
                    }

                }
                if (client.Player.VipLevel == 4)
                {
                    if (client.Player.VipPointsD >= 8000)
                    {
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            client.Player.VipPointsD -= 8000;
                            client.Player.VipLevel = 5;
                            client.Player.SendUpdate(stream, client.Player.VipLevel, MsgUpdate.DataType.VIPLevel);
                            client.Player.UpdateVip(stream);
                            client.CreateBoxDialog("Congratulations! You upgrade Your Vip Level Top Level 5");

                        }
                    }

                }
                if (client.Player.VipLevel == 5)
                {
                    if (client.Player.VipPointsD >= 16000)
                    {
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            client.Player.VipPointsD -= 16000;
                            client.Player.VipLevel = 6;
                            client.Player.SendUpdate(stream, client.Player.VipLevel, MsgUpdate.DataType.VIPLevel);
                            client.Player.UpdateVip(stream);
                            client.CreateBoxDialog("Congratulations! You upgrade Your Vip Level Top Level 6");

                        }
                    }

                }
                if (client.Player.VipLevel == 6)
                {
                    if (client.Player.VipPointsD >= 32000)
                    {
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            client.Player.VipPointsD -= 32000;
                            client.Player.VipLevel = 7;
                            client.Player.SendUpdate(stream, client.Player.VipLevel, MsgUpdate.DataType.VIPLevel);
                            client.Player.UpdateVip(stream);
                            client.CreateBoxDialog("Congratulations! You upgrade Your Vip Level Top Level 7");

                        }
                    }

                }
                #endregion
                #region Atribute Points
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    if (client.Player.Level >= 100)
                    {
                        if (client.Player.Agility + client.Player.Strength + client.Player.Vitality + client.Player.Spirit + client.Player.Atributes >= 902)
                        {
                            client.Player.Vitality = 1; client.Player.Agility = client.Player.Strength = client.Player.Spirit = 0; client.Player.Atributes = 899;
                            client.Player.SendUpdate(stream, client.Player.Strength, Game.MsgServer.MsgUpdate.DataType.Strength);
                            client.Player.SendUpdate(stream, client.Player.Agility, Game.MsgServer.MsgUpdate.DataType.Agility);
                            client.Player.SendUpdate(stream, client.Player.Spirit, Game.MsgServer.MsgUpdate.DataType.Spirit);
                            client.Player.SendUpdate(stream, client.Player.Vitality, Game.MsgServer.MsgUpdate.DataType.Vitality);
                            client.Player.SendUpdate(stream, client.Player.Atributes, Game.MsgServer.MsgUpdate.DataType.Atributes);
                            client.Equipment.QueryEquipment(client.Equipment.Alternante, false);
                        }
                    }
                    if (Database.AtributesStatus.IsWarrior(client.Player.Class) || Database.AtributesStatus.IsTrojan(client.Player.Class))
                    {
                        if (client.Player.Agility > 150)
                        {
                            client.Player.Vitality = 1; client.Player.Agility = client.Player.Strength = client.Player.Spirit = 0; client.Player.Atributes = 899;
                            client.Player.SendUpdate(stream, client.Player.Strength, Game.MsgServer.MsgUpdate.DataType.Strength);
                            client.Player.SendUpdate(stream, client.Player.Agility, Game.MsgServer.MsgUpdate.DataType.Agility);
                            client.Player.SendUpdate(stream, client.Player.Spirit, Game.MsgServer.MsgUpdate.DataType.Spirit);
                            client.Player.SendUpdate(stream, client.Player.Vitality, Game.MsgServer.MsgUpdate.DataType.Vitality);
                            client.Player.SendUpdate(stream, client.Player.Atributes, Game.MsgServer.MsgUpdate.DataType.Atributes);
                            client.Equipment.QueryEquipment(client.Equipment.Alternante, false);
                        }
                    }

                }
                #endregion
                #region electr
                if (!client.Player.Alive && client.Player.ContainFlag(MsgUpdate.Flags.Electricity))
                {
                    client.Player.RemoveFlag(MsgUpdate.Flags.Electricity);

                }
                #endregion
                #region bansystem
                if (client.Player.BadPoints >= 2)
                {
                    DateTime Time = DateTime.Now;
                    Time = DateTime.Now.AddMinutes(120);
                    client.Player.BannedChatStamp = Time;
                    client.Player.IsBannedChat = true;
                    WindowsAPI.IniFile write = new WindowsAPI.IniFile("\\Users\\" + client.Player.UID + ".ini");
                    write.Write<bool>("Character", "IsBannedChat", true);
                    write.Write<long>("Character", "BannedChatStamp", Time.ToBinary());
                    MsgSchedules.SendSysMesage("Bad News, " + client.Player.Name + " Has Bannded From Chat For 2 HoursBecause illegal words  .", MsgMessage.ChatMode.Center);
                    client.Player.BadPoints = 0;
                }
                #endregion
                #region Two Hand Safety Checks For Taoist
                if (Database.AtributesStatus.IsTaoist(client.Player.Class))
                {
                    if (client.Equipment.LeftWeapon != 0)
                    {
                        if (Database.ItemType.IsHossu(client.Equipment.LeftWeapon) == false)
                        {
                            if (client.Inventory.HaveSpace(1))
                            {
                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    client.Equipment.Remove(Role.Flags.ConquerItem.LeftWeapon, stream);
                                    client.Equipment.LeftWeapon = 0;
                                }
                            }
                        }
                    }
                }
                else if (Database.ItemType.IsTwoHand(client.Equipment.RightWeapon))
                {
                    if (client.Equipment.LeftWeapon != 0 && Database.ItemType.IsShield(client.Equipment.LeftWeapon) == false)
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (client.Equipment.Remove(Role.Flags.ConquerItem.LeftWeapon, stream) == false)
                                    client.Equipment.Remove(Role.Flags.ConquerItem.AleternanteLeftWeapon, stream);
                                client.Equipment.LeftWeapon = 0;
                            }
                        }
                    }
                }
                #endregion
                #region Nobility
                if (client.Player.NobilityRank == Role.Instance.Nobility.NobilityRank.Serf)
                {
                    Program.NobilityRanking.UpdateRank(client.Player.Nobility);
                    using (var rect = new ServerSockets.RecycledPacket())
                    {
                        var stream = rect.GetStream();
                        client.Send(stream.NobilityIconCreate(client.Player.Nobility));
                    }
                }
                #endregion
                #region VIP 7
                if (client.Player.VipLevel == 7 && client.Player.Alive)
                {
                    client.Player.ShieldEl3Tar(720);
                    client.Player.StigmaEl3Tar(720);
                }
                #endregion
                #region Event Base
                if (client.EventBase != null)
                {
                    var events = Program.Events.Find(x => x.EventTitle == client.EventBase.EventTitle);
                    if (events != null)
                    {
                        if (events.Stage == Game.MsgEvents.EventStage.Fighting)
                        {
                            events.CharacterChecks(client);
                            if (events.InTournament(client))
                            {
                                if (events.ReviveAllowed)
                                {
                                    if (!client.Player.Alive)
                                    {
                                        events.RevivePlayer(client);
                                    }
                                }
                            }
                        }
                    }
                }
                #endregion
                #region DragonBall Scroll
                if (client.Player.OnAutoHunt == true)
                {
                    using (var rect = new ServerSockets.RecycledPacket())
                    {
                        var stream = rect.GetStream();
                        if (client.Inventory.Contain(1088000, 10))
                        {
                            client.Inventory.Remove(1088000, 10, stream);
                            client.Inventory.Add(stream, 720028);
                            client.SendSysMesage("You Have Maked DB-Scroll ");
                        }
                    }
                }
                #endregion
                if (client.Player.MyJiangHu != null)
                {
                    client.Player.MyJiangHu.CheckStatus(client);
                }
                if (client.Player.Merchant == 1 && client.Player.MerchantApplicationEnd <= DateTime.Now)
                {
                    client.Player.Merchant = 255;
                }
                if (Database.AtributesStatus.IsWindWalker(client.Player.Class))
                {
                    if (timer > client.Player.WindWalkerEffect.AddSeconds(7))
                    {
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "TSM_SXJ_HPhf");
                            client.Player.WindWalkerEffect = Extensions.Time32.Now;
                        }
                    }
                }
                if (client.Player.Map == 601)
                {
                    if (!client.Map.ValidLocation(client.Player.X, client.Player.Y))
                    {
                        client.Teleport(64, 56, 601);
                    }
                }
                if (client.Player.Map == 44463)
                {
                    if (timer > client.Player.EarthStamp.AddSeconds(10))
                    {
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            Game.MsgFloorItem.MsgItemPacket effect = Game.MsgFloorItem.MsgItemPacket.Create();
                            effect.m_UID = (uint)Game.MsgFloorItem.MsgItemPacket.EffectMonsters.EarthquakeLeftRight;
                            effect.DropType = MsgDropID.Earth;
                            effect.m_X = client.Player.X;
                            effect.m_Y = client.Player.Y;
                            client.Send(stream.ItemPacketCreate(effect));
                        }
                        client.Player.EarthStamp = Extensions.Time32.Now;
                    }
                }
                if (client.Player.ExpProtection > 0)
                    client.Player.ExpProtection -= 1;
                if (client.Player.WaveofBlood)
                {
                    if (DateTime.Now > client.Player.WaveofBloodStamp.AddSeconds(8))
                    {
                        client.Player.WaveofBlood = false;
                        client.Player.XPCount += 1;//15
                    }
                }
                if (client.Player.StartVote)
                {
                    Database.VoteSystem.CheckUp(client);
                }
                if (client.Player.OnDefensePotion)
                {
                    if (timer > client.Player.OnDefensePotionStamp)
                    {
                        client.Player.OnDefensePotion = false;
                    }
                }
                if (client.Player.OnAttackPotion)
                {
                    if (timer > client.Player.OnAttackPotionStamp)
                    {
                        client.Player.OnAttackPotion = false;
                    }
                }
                if (Game.MsgTournaments.MsgSchedules.CurrentTournament.Process == Game.MsgTournaments.ProcesType.Alive)
                {
                    if (Game.MsgTournaments.MsgSchedules.CurrentTournament.Type == Game.MsgTournaments.TournamentType.SkillTournament)
                    {
                        var tournament = Game.MsgTournaments.MsgSchedules.CurrentTournament as Game.MsgTournaments.MsgSkillTournament;
                        tournament.Revive(timer, client);
                    }
                    if (Game.MsgTournaments.MsgSchedules.CurrentTournament.Type == Game.MsgTournaments.TournamentType.ExtremePk)
                    {
                        if (Game.MsgTournaments.MsgSchedules.CurrentTournament.InTournament(client))
                        {
                            var tournament = Game.MsgTournaments.MsgSchedules.CurrentTournament as Game.MsgTournaments.MsgExtremePk;
                            tournament.Revive(timer, client);
                        }
                    }
                }
                //if (client.Player.Map == 1038 && Game.MsgTournaments.MsgSchedules.GuildWar.Proces == Game.MsgTournaments.ProcesType.Alive)
                //{
                //        using (var rec = new ServerSockets.RecycledPacket())
                //        {
                //            var stream = rec.GetStream();
                //            Game.MsgServer.MsgMessage msg73 = new MsgMessage("------------Kill Board----------", MsgMessage.MsgColor.yellow, MsgMessage.ChatMode.FirstRightCorner);
                //            client.Send(msg73.GetArray(stream));
                //        }
                //    var array = Ranksss.newranks.Values.Where(i => i.MostKill >= 1).OrderByDescending(i => i.MostKill).ToArray();
                //    for (int x = 0; x < Math.Min(5, client.Map.Values.Length); x++)
                //    {
                //        using (var rec = new ServerSockets.RecycledPacket())
                //        {
                //            var stream = rec.GetStream();
                //            var element = array[x];
                //            Game.MsgServer.MsgMessage msg92 = new MsgMessage("No " + (x + 1).ToString() + "- " + array[x].NamePlayer + " Kills: " + array[x].MostKill + "", MsgMessage.MsgColor.yellow, MsgMessage.ChatMode.ContinueRightCorner);
                //            //foreach (var user in client.Map.Values)
                //                client.Send(msg92.GetArray(stream));
                //        }
                //    }
                //        using (var rec = new ServerSockets.RecycledPacket())
                //        {
                //            var stream = rec.GetStream();

                //            Game.MsgServer.MsgMessage msg84 = new MsgMessage("", MsgMessage.MsgColor.yellow, MsgMessage.ChatMode.ContinueRightCorner);
                //            Game.MsgServer.MsgMessage msg85 = new MsgMessage("--------------Death Board-----------", MsgMessage.MsgColor.yellow, MsgMessage.ChatMode.ContinueRightCorner);
                //            client.Send(msg84.GetArray(stream));
                //        client.Send(msg85.GetArray(stream));
                //        }
                //    var array1 = Ranksss.newranks.Values.Where(i => i.MostDeath >= 1).OrderByDescending(i => i.MostDeath).ToArray();
                //    for (int x = 0; x < Math.Min(5, client.Map.Values.Length); x++)
                //    {
                //        using (var rec = new ServerSockets.RecycledPacket())
                //        {
                //            var stream = rec.GetStream();
                //            var element = array1[x];
                //            Game.MsgServer.MsgMessage msg10 = new MsgMessage("No " + (x + 1).ToString() + "- " + array1[x].NamePlayer + " Dead : " + array1[x].MostDeath + " ", MsgMessage.MsgColor.yellow, MsgMessage.ChatMode.ContinueRightCorner);
                //            //      foreach (var user in client.Map.Values)
                //            client.Send(msg10.GetArray(stream));
                //        }
                //    }
                //    using (var rec = new ServerSockets.RecycledPacket())
                //    {
                //        var stream = rec.GetStream();
                //        Game.MsgServer.MsgMessage msg14 = new MsgMessage("", MsgMessage.MsgColor.yellow, MsgMessage.ChatMode.ContinueRightCorner);
                //        Game.MsgServer.MsgMessage msg15 = new MsgMessage("--------------Revive Board-----------", MsgMessage.MsgColor.yellow, MsgMessage.ChatMode.ContinueRightCorner);
                //        client.Send(msg14.GetArray(stream));
                //        client.Send(msg15.GetArray(stream));
                //    }
                //    var array2 = Ranksss.newranks.Values.Where(i => i.MostRevive >= 1).OrderByDescending(i => i.MostRevive).ToArray();
                //    for (int x = 0; x < Math.Min(5, client.Map.Values.Length); x++)
                //    {
                //        using (var rec = new ServerSockets.RecycledPacket())
                //        {
                //            var stream = rec.GetStream();
                //            var element = array2[x];
                //            Game.MsgServer.MsgMessage msg16 = new MsgMessage("No " + (x + 1).ToString() + "- " + array2[x].NamePlayer + " Dead : " + array2[x].MostRevive + " ", MsgMessage.MsgColor.yellow, MsgMessage.ChatMode.ContinueRightCorner);
                //         //   foreach (var user in client.Map.Values)
                //                client.Send(msg16.GetArray(stream));
                //        }
                //    }
                //}
                if (client.Player.Map == 1038 && Game.MsgTournaments.MsgSchedules.GuildWar.Proces == Game.MsgTournaments.ProcesType.Alive)
                {
                    foreach (var user in client.Map.Values.Where(e => e.Player.Map == 1038))
                    {
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            Game.MsgServer.MsgMessage msg3 = new MsgMessage("--------Kill Board-------", MsgMessage.MsgColor.yellow, MsgMessage.ChatMode.FirstRightCorner);
                            user.Send(msg3.GetArray(stream));
                        }
                    }
                    var array = Ranksss.newranks.Values.OrderByDescending(p => p.MostKill).ToArray();
                    for (int x = 0; x < Math.Min(5, client.Map.Values.Length); x++)
                    {
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            var element = array[x];
                            Game.MsgServer.MsgMessage msg2 = new MsgMessage("No " + (x + 1).ToString() + "- " + element.NamePlayer + " Kills: " + element.MostKill.ToString() + " ", MsgMessage.MsgColor.yellow, MsgMessage.ChatMode.ContinueRightCorner);
                            foreach (var user in client.Map.Values)
                                user.Send(msg2.GetArray(stream));
                        }
                    }
                    foreach (var user in client.Map.Values.Where(e => e.Player.Map == 1038))
                    {
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();

                            Game.MsgServer.MsgMessage msg4 = new MsgMessage("", MsgMessage.MsgColor.yellow, MsgMessage.ChatMode.ContinueRightCorner);
                            Game.MsgServer.MsgMessage msg5 = new MsgMessage("---------Death Board--------", MsgMessage.MsgColor.yellow, MsgMessage.ChatMode.ContinueRightCorner);
                            user.Send(msg4.GetArray(stream));
                            user.Send(msg5.GetArray(stream));
                        }
                    }
                    var array1 = Ranksss.newranks.Values.OrderByDescending(p => p.MostDeath).ToArray();
                    for (int x = 0; x < Math.Min(5, client.Map.Values.Length); x++)
                    {
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            var element = array1[x];
                            Game.MsgServer.MsgMessage msg1 = new MsgMessage("No " + (x + 1).ToString() + "- " + element.NamePlayer + " Have : " + element.MostDeath.ToString() + " Death", MsgMessage.MsgColor.yellow, MsgMessage.ChatMode.ContinueRightCorner);
                            foreach (var user in client.Map.Values)
                                user.Send(msg1.GetArray(stream));
                        }
                    }
                    foreach (var user in client.Map.Values.Where(e => e.Player.Map == 1038))
                    {
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();

                            Game.MsgServer.MsgMessage msg6 = new MsgMessage("", MsgMessage.MsgColor.yellow, MsgMessage.ChatMode.ContinueRightCorner);
                            Game.MsgServer.MsgMessage msg7 = new MsgMessage("---------Revive Board--------", MsgMessage.MsgColor.yellow, MsgMessage.ChatMode.ContinueRightCorner);
                            user.Send(msg6.GetArray(stream));
                            user.Send(msg7.GetArray(stream));
                        }
                    }
                    var array2 = Ranksss.newranks.Values.OrderByDescending(p => p.MostRevive).ToArray();
                    for (int x = 0; x < Math.Min(5, client.Map.Values.Length); x++)
                    {
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            var element = array2[x];
                            Game.MsgServer.MsgMessage msg10 = new MsgMessage("No " + (x + 1).ToString() + "- " + element.NamePlayer + " Have : " + element.MostRevive.ToString() + " Revive", MsgMessage.MsgColor.yellow, MsgMessage.ChatMode.ContinueRightCorner);
                            foreach (var user in client.Map.Values)
                                user.Send(msg10.GetArray(stream));
                        }
                    }
                }
                if (client.Player.Map == 1005 && client.Player.DynamicID == 0)
                {
                    if (!client.Player.Alive)
                    {
                        if (client.Player.DeadStamp.AddSeconds(3) < timer)
                        {
                            ushort x = 0; ushort y = 0;
                            client.Map.GetRandCoord(ref x, ref y);
                            client.Teleport(x, y, 1005, 0);
                        }
                    }
                    //if (client.Player.StampArenaScore.AddSeconds(1) < timer)
                    //{
                    //    client.SendSysMesage("[Arena  Stats]", MsgMessage.ChatMode.FirstRightCorner, MsgMessage.MsgColor.yellow);
                    //    client.SendSysMesage("Kills : " + client.Player.ArenaKills + " Deaths : " + client.Player.ArenaDeads + " ", MsgMessage.ChatMode.ContinueRightCorner, MsgMessage.MsgColor.yellow);

                    //    client.Player.StampArenaScore = timer;
                    //}
                    foreach (var user in client.Map.Values.Where(e => e.Player.Map == 1005))
                    {
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            Game.MsgServer.MsgMessage msg3 = new MsgMessage("---Your Score---", MsgMessage.MsgColor.yellow, MsgMessage.ChatMode.FirstRightCorner);
                            user.Send(msg3.GetArray(stream));
                        }
                    }
                    var array = client.Map.Values.OrderByDescending(p => p.Player.ArenaKills).ToArray();
                    for (int x = 0; x < Math.Min(10, client.Map.Values.Length); x++)
                    {
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            var element = array[x];
                            Game.MsgServer.MsgMessage msg2 = new MsgMessage("No " + (x + 1).ToString() + "- " + element.Player.Name + " Kills: " + element.Player.ArenaKills.ToString() + " Deaths: " + element.Player.ArenaDeads + "", MsgMessage.MsgColor.yellow, MsgMessage.ChatMode.ContinueRightCorner);
                            foreach (var user in client.Map.Values)
                                user.Send(msg2.GetArray(stream));
                        }
                    }
                    foreach (var user in client.Map.Values.Where(e => e.Player.Map == 1005))
                    {
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();

                            Game.MsgServer.MsgMessage msg4 = new MsgMessage("", MsgMessage.MsgColor.yellow, MsgMessage.ChatMode.ContinueRightCorner);
                            Game.MsgServer.MsgMessage msg5 = new MsgMessage("--------------Hit Board-----------", MsgMessage.MsgColor.yellow, MsgMessage.ChatMode.ContinueRightCorner);
                            user.Send(msg4.GetArray(stream));
                            user.Send(msg5.GetArray(stream));
                        }
                    }
                    var array1 = client.Map.Values.OrderByDescending(p => p.Player.Hits).ToArray();
                    for (int x = 0; x < Math.Min(10, client.Map.Values.Length); x++)
                    {
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            var element = array1[x];
                            Game.MsgServer.MsgMessage msg1 = new MsgMessage("No " + (x + 1).ToString() + "- " + element.Player.Name + " Have : " + element.Player.Hits.ToString() + " Hits", MsgMessage.MsgColor.yellow, MsgMessage.ChatMode.ContinueRightCorner);
                            foreach (var user in client.Map.Values)
                                user.Send(msg1.GetArray(stream));
                        }
                    }
                }
                #region Custom
                if (Game.AISystem.UnlimitedArenaRooms.Maps.ContainsValue(client.Player.DynamicID) && client.Player.fbss == 50)
                {
                    client.SendSysMesage("-----[Accuracy Rates]---", MsgMessage.ChatMode.FirstRightCorner, MsgMessage.MsgColor.pink);
                    foreach (var player in client.Map.Values.Where(e => e.Player.DynamicID == client.Player.DynamicID))
                    {
                        client.SendSysMesage(player.Player.Name + " " + Math.Round((double)(player.Player.Hits * 100.0 / Math.Max(1, player.Player.TotalHits)), 2) +
                            "%, Hits: " + player.Player.Hits + ", Miss: " +
                            (player.Player.TotalHits - player.Player.Hits) + ", M.C: " + player.Player.MaxChains,
                            MsgMessage.ChatMode.ContinueRightCorner);
                        if (player.Fake == true)
                        {
                            if (player.Player.Hits >= 100)
                            {
                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "sports_failure");
                                }
                                client.Teleport(300, 278, 1002);//الاعب
                                MsgSchedules.SendSysMesage("The Player: [ " + client.Player.Name + " ] , Has losed the fight vs HerculesBot with rate For Bot [" + Math.Round((double)(player.Player.Hits * 100.0 / Math.Max(1, player.Player.TotalHits)), 2) +
                            "% ] &&  Max.Chains: [" + player.Player.MaxChains + "] ", MsgMessage.ChatMode.World);
                            }
                        }
                        else
                        {
                            if (player.Player.Hits >= 100)
                            {
                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "sports_victory");
                                }
                                client.Teleport(300, 278, 1002);//الاعب
                                MsgSchedules.SendSysMesage("The Player: [ " + client.Player.Name + " ] , Has winned the fight vs HerculesBot with rate For Him [" + Math.Round((double)(player.Player.Hits * 100.0 / Math.Max(1, player.Player.TotalHits)), 2) +
"%],  && Max.Chains: [" + player.Player.MaxChains + "] ", MsgMessage.ChatMode.World);
                            }
                        }
                    }
                }
                if (Game.AISystem.UnlimitedArenaRooms.Maps.ContainsValue(client.Player.DynamicID) && client.Player.fbss != 10)
                {
                    client.SendSysMesage("-----[Accuracy Rates]---", MsgMessage.ChatMode.FirstRightCorner, MsgMessage.MsgColor.pink);
                    foreach (var player in client.Map.Values.Where(e => e.Player.DynamicID == client.Player.DynamicID))
                    {
                        client.SendSysMesage(player.Player.Name + " " + Math.Round((double)(player.Player.Hits * 100.0 / Math.Max(1, player.Player.TotalHits)), 2) +
                            "%, Hits: " + player.Player.Hits + ", Miss: " +
                            (player.Player.TotalHits - player.Player.Hits) + ", M.C: " + player.Player.MaxChains,
                            MsgMessage.ChatMode.ContinueRightCorner);
                        if (player.Fake == true)
                        {
                            if (player.Player.Hits >= 100)
                            {
                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "sports_failure");
                                }
                                client.Teleport(300, 278, 1002);//الاعب
                                MsgSchedules.SendSysMesage("The Player: [ " + client.Player.Name + " ] , Has losed the fight vs HerculesBot with rate For Bot [" + Math.Round((double)(player.Player.Hits * 100.0 / Math.Max(1, player.Player.TotalHits)), 2) +
"% ] &&  Max.Chains: [" + player.Player.MaxChains + "] ", MsgMessage.ChatMode.World);
                            }
                        }
                        else
                        {
                            if (player.Player.Hits >= 100)
                            {
                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "sports_victory");
                                }
                                client.Teleport(300, 278, 1002);//الاعب
                                MsgSchedules.SendSysMesage("The Player: [ " + client.Player.Name + " ] , Has Winned the fight vs HerculesBot with rate For Him [" + Math.Round((double)(player.Player.Hits * 100.0 / Math.Max(1, player.Player.TotalHits)), 2) +
"% ] &&  Max.Chains: [" + player.Player.MaxChains + "] ", MsgMessage.ChatMode.World);
                            }
                        }
                    }
                }
                if (Game.AISystem.UnlimitedArenaRooms.Maps.ContainsValue(client.Player.DynamicID) && client.Player.fbss == 10)
                {
                    client.SendSysMesage("-----[Accuracy Rates][UnLimitted]---", MsgMessage.ChatMode.FirstRightCorner, MsgMessage.MsgColor.pink);
                    foreach (var player in client.Map.Values.Where(e => e.Player.DynamicID == client.Player.DynamicID))
                    {
                        client.SendSysMesage(player.Player.Name + " " + Math.Round((double)(player.Player.Hits * 100.0 / Math.Max(1, player.Player.TotalHits)), 2) +
                            "%, Hits: " + player.Player.Hits + ", Miss: " +
                            (player.Player.TotalHits - player.Player.Hits) + ", M.C: " + player.Player.MaxChains,
                            MsgMessage.ChatMode.ContinueRightCorner);
                    }
                }
                #endregion
                if (client.Player.Map == 5051)
                {
                    if (!client.Player.Alive)
                    {
                        if (client.Player.DeadStamp.AddSeconds(1) < timer)
                        {
                            client.Teleport(300, 278, 1002);
                        }
                    }
                }
                #region Screen
                if (Program.ScreenFBRoom.Contains(client.Player.Map))
                {
                    if (client.Player.StampArenaScore.AddSeconds(1) < timer)
                    {
                        uint Rate = 0;
                        if (client.Player.MisShoot != 0)
                            Rate = (uint)(((float)client.Player.HitShoot / (float)client.Player.MisShoot) * 100f);

                        client.SendSysMesage("----[Room-FB/SS Stats]----", MsgMessage.ChatMode.FirstRightCorner, MsgMessage.MsgColor.blue);
                        client.SendSysMesage("", MsgMessage.ChatMode.ContinueRightCorner, MsgMessage.MsgColor.orange);

                        client.SendSysMesage("All-Shoots   : " + client.Player.MisShoot + " ", MsgMessage.ChatMode.ContinueRightCorner, MsgMessage.MsgColor.cyan);
                        client.SendSysMesage("Your-Hits : " + client.Player.HitShoot + " ", MsgMessage.ChatMode.ContinueRightCorner, MsgMessage.MsgColor.orange);
                        client.SendSysMesage("Remaining Times : " + client.Player.Arenapika + " ", MsgMessage.ChatMode.ContinueRightCorner, MsgMessage.MsgColor.orange);
                        client.SendSysMesage("", MsgMessage.ChatMode.ContinueRightCorner, MsgMessage.MsgColor.orange);
                        client.SendSysMesage(" Only Have 5 Times To Stay Alive  ", MsgMessage.ChatMode.ContinueRightCorner, MsgMessage.MsgColor.black);

                        client.Player.StampArenaScore = timer;
                    }
                }
                if (client.Player.Map == 8602)
                {
                    if (client.Player.StampArenaScore.AddSeconds(1) < timer)
                    {
                        uint Rate = 0;
                        if (client.Player.MisShoot != 0)
                            Rate = (uint)(((float)client.Player.HitShoot / (float)client.Player.MisShoot) * 100f);

                        client.SendSysMesage("----[Custom Room-FB/SS Stats]----", MsgMessage.ChatMode.FirstRightCorner, MsgMessage.MsgColor.blue);
                        client.SendSysMesage("", MsgMessage.ChatMode.ContinueRightCorner, MsgMessage.MsgColor.orange);

                        client.SendSysMesage("All-Shoots   : " + client.Player.MisShoot + " ", MsgMessage.ChatMode.ContinueRightCorner, MsgMessage.MsgColor.cyan);
                        client.SendSysMesage("Your-Hits : " + client.Player.HitShoot + " ", MsgMessage.ChatMode.ContinueRightCorner, MsgMessage.MsgColor.orange);
                        client.SendSysMesage("Remaining Times : " + client.Player.Arenapika + " ", MsgMessage.ChatMode.ContinueRightCorner, MsgMessage.MsgColor.orange);
                        client.SendSysMesage("", MsgMessage.ChatMode.ContinueRightCorner, MsgMessage.MsgColor.orange);
                        client.SendSysMesage(" Only Have 50 Times To Stay Alive  ", MsgMessage.ChatMode.ContinueRightCorner, MsgMessage.MsgColor.black);

                        client.Player.StampArenaScore = timer;
                    }
                }
                if (client.Player.Map == 8603 || client.Player.Map == 8604)
                {
                    if (client.Player.StampArenaScore.AddSeconds(1) < timer)
                    {
                        uint Rate = 0;
                        if (client.Player.MisShoot != 0)
                            Rate = (uint)(((float)client.Player.HitShoot / (float)client.Player.MisShoot) * 100f);

                        client.SendSysMesage("----[Custom Room-Ninja Stats]----", MsgMessage.ChatMode.FirstRightCorner, MsgMessage.MsgColor.blue);
                        client.SendSysMesage("", MsgMessage.ChatMode.ContinueRightCorner, MsgMessage.MsgColor.orange);
                        client.SendSysMesage("Your-Hits : " + client.Player.HitShoot + " ", MsgMessage.ChatMode.ContinueRightCorner, MsgMessage.MsgColor.orange);
                        client.SendSysMesage("Remaining Times : " + client.Player.Arenapika + " ", MsgMessage.ChatMode.ContinueRightCorner, MsgMessage.MsgColor.orange);
                        client.SendSysMesage("", MsgMessage.ChatMode.ContinueRightCorner, MsgMessage.MsgColor.orange);
                        client.SendSysMesage(" Only Have 50 Times To Stay Alive ", MsgMessage.ChatMode.ContinueRightCorner, MsgMessage.MsgColor.black);

                        client.Player.StampArenaScore = timer;
                    }
                }
                if (client.Player.Map == 8601)
                {
                    if (client.Player.StampArenaScore.AddSeconds(1) < timer)
                    {
                        uint Rate = 0;
                        if (client.Player.MisShoot != 0)
                            Rate = (uint)(((float)client.Player.HitShoot / (float)client.Player.MisShoot) * 100f);

                        client.SendSysMesage("----[Custom Room-FB/SS Stats]----", MsgMessage.ChatMode.FirstRightCorner, MsgMessage.MsgColor.blue);
                        client.SendSysMesage("", MsgMessage.ChatMode.ContinueRightCorner, MsgMessage.MsgColor.orange);

                        client.SendSysMesage("All-Shoots   : " + client.Player.MisShoot + " ", MsgMessage.ChatMode.ContinueRightCorner, MsgMessage.MsgColor.cyan);
                        client.SendSysMesage("Your-Hits : " + client.Player.HitShoot + " ", MsgMessage.ChatMode.ContinueRightCorner, MsgMessage.MsgColor.orange);
                        client.SendSysMesage("Remaining Times : " + client.Player.Arenapika + " ", MsgMessage.ChatMode.ContinueRightCorner, MsgMessage.MsgColor.orange);
                        client.SendSysMesage("", MsgMessage.ChatMode.ContinueRightCorner, MsgMessage.MsgColor.orange);
                        client.SendSysMesage(" Only Have 10 Times To Stay Alive  ", MsgMessage.ChatMode.ContinueRightCorner, MsgMessage.MsgColor.black);

                        client.Player.StampArenaScore = timer;
                    }
                }
                if (client.Player.Map == 3030)
                {
                    if (client.Player.StampArenaScore.AddSeconds(1) < timer)
                    {

                        client.SendSysMesage("----[DIABLO-EVENT]----", MsgMessage.ChatMode.FirstRightCorner, MsgMessage.MsgColor.blue);
                        client.SendSysMesage("", MsgMessage.ChatMode.ContinueRightCorner, MsgMessage.MsgColor.orange);
                        client.SendSysMesage("Your Score : " + client.Player.BattleFieldPoints + " ", MsgMessage.ChatMode.ContinueRightCorner, MsgMessage.MsgColor.orange);

                        client.Player.StampArenaScore = timer;
                    }
                }
                if (client.Player.Map == 2575)
                {
                    if (client.Player.StampArenaScore.AddSeconds(1) < timer)
                    {
                        uint Rate = 0;
                        if (client.Player.MisShoot != 0)
                            Rate = (uint)(((float)client.Player.HitShoot / (float)client.Player.MisShoot) * 100f);

                        client.SendSysMesage("----[Skill-War Stats]----", MsgMessage.ChatMode.FirstRightCorner, MsgMessage.MsgColor.blue);
                        client.SendSysMesage("", MsgMessage.ChatMode.ContinueRightCorner, MsgMessage.MsgColor.orange);

                        client.SendSysMesage("MaxLifes [ 4 ]  ", MsgMessage.ChatMode.ContinueRightCorner, MsgMessage.MsgColor.cyan);
                        client.SendSysMesage("Your MaxLifes Now : " + client.Player.SkillTournamentLifes + " ", MsgMessage.ChatMode.ContinueRightCorner, MsgMessage.MsgColor.orange);
                        client.SendSysMesage("", MsgMessage.ChatMode.ContinueRightCorner, MsgMessage.MsgColor.orange);
                        client.SendSysMesage(" IF You Have been Hit  [5 Times] You Will Eliminate   ", MsgMessage.ChatMode.ContinueRightCorner, MsgMessage.MsgColor.black);

                        client.Player.StampArenaScore = timer;
                    }
                }
                #endregion
                if (client.Player.Map == 2570 || client.Player.Map == 2578 || client.Player.Map == 2569 || client.Player.Map == 2579 || client.Player.Map == 2568 || client.Player.Map == 2567 || client.Player.Map == 8602 || client.Player.Map == 8601 || client.Player.Map == 8603 || client.Player.Map == 8604 || client.Player.Map == 7701 || client.Player.Map == 7702 || client.Player.Map == 7703 || client.Player.Map == 7704 || client.Player.Map == 7705 || client.Player.Map == 7706 || client.Player.Map == 7721 || client.Player.Map == 7722 || client.Player.Map == 7723 || client.Player.Map == 7724 || client.Player.Map == 7725 || client.Player.Map == 7726)
                {
                    if (!client.Player.Alive)
                    {
                        if (client.Player.DeadStamp.AddSeconds(1) < timer)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "sports_failure");
                            }
                            client.Teleport(440, 390, 1002, 0);
                        }
                    }
                }
                #region auto hp
                if (client.Player.Autohp >= 1)
                {
                    if (client.Inventory.Contain(1003010, 1))
                    {
                        if (DateTime.Now > client.Player.MedicineStamp.AddMilliseconds(150))
                        {
                            if (client.Player.ContainFlag(MsgUpdate.Flags.PoisonStar))
                                return;
                            if (client.Player.HitPoints == client.Status.MaxHitpoints)
                                return;
                            if (Program.NoHP.Contains(client.Player.Map))
                            {
                                client.SendSysMesage("No HP In This Map .");
                                return;
                            }
                            client.Player.HitPoints = Math.Min(client.Player.HitPoints + 10000, (int)client.Status.MaxHitpoints);
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                client.Inventory.Remove(1003010, 1, stream);
                                client.Player.MedicineStamp = DateTime.Now;
                            }
                            client.Player.Autohp -= 1;
                        }
                    }
                    else if (client.Inventory.Contain(725065, 1))
                    {
                        if (DateTime.Now > client.Player.MedicineStamp.AddMilliseconds(150))
                        {
                            if (client.Player.ContainFlag(MsgUpdate.Flags.PoisonStar))
                                return;
                            if (client.Player.HitPoints == client.Status.MaxHitpoints)
                                return;
                            if (Program.NoHP.Contains(client.Player.Map))
                            {
                                client.SendSysMesage("No HP In This Map .");
                                return;
                            }
                            client.Player.HitPoints = Math.Min(client.Player.HitPoints + 8000, (int)client.Status.MaxHitpoints);
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                client.Inventory.Remove(725065, 1, stream);
                                client.Player.MedicineStamp = DateTime.Now;
                            }
                            client.Player.Autohp -= 1;
                        }
                    }
                }
                #endregion
                client.Player.UpdateTaoistPower(timer);
                if (client.Player.X == 0 || client.Player.Y == 0)
                {
                    client.Teleport(300, 278, 1002);
                }
                if (client.Player.HeavenBlessing > 0)
                {
                    if (client.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.HeavenBlessing))
                    {
                        if (timer > client.Player.HeavenBlessTime)
                        {
                            client.Player.RemoveFlag(Game.MsgServer.MsgUpdate.Flags.HeavenBlessing);
                            client.Player.HeavenBlessing = 0;
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                client.Player.SendUpdate(stream, 0, Game.MsgServer.MsgUpdate.DataType.HeavensBlessing);
                                client.Player.SendUpdate(stream, Game.MsgServer.MsgUpdate.OnlineTraining.Remove, Game.MsgServer.MsgUpdate.DataType.OnlineTraining);

                                client.Player.Stamina = (ushort)Math.Min((int)client.Player.Stamina, 100);
                                client.Player.SendUpdate(stream, client.Player.Stamina, Game.MsgServer.MsgUpdate.DataType.Stamina);
                            }
                        }
                        if (client.Player.Map != 601 && client.Player.Map != 1039)
                        {
                            if (timer > client.Player.ReceivePointsOnlineTraining)
                            {
                                client.Player.ReceivePointsOnlineTraining = timer.AddMinutes(1);
                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    client.Player.SendUpdate(stream, Game.MsgServer.MsgUpdate.OnlineTraining.IncreasePoints, Game.MsgServer.MsgUpdate.DataType.OnlineTraining);//+10
                                }
                            }
                            if (timer > client.Player.OnlineTrainingTime)
                            {
                                client.Player.OnlineTrainingPoints += 100000;
                                client.Player.OnlineTrainingTime = timer.AddMinutes(10);
                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    client.Player.SendUpdate(stream, Game.MsgServer.MsgUpdate.OnlineTraining.ReceiveExperience, Game.MsgServer.MsgUpdate.DataType.OnlineTraining);
                                }
                            }
                        }
                    }
                }
                if (client.Player.EnlightenReceive > 0)
                {
                    if (DateTime.Now > client.Player.EnlightenTime.AddMinutes(20))
                    {
                        client.Player.EnlightenTime = DateTime.Now;
                        client.Player.EnlightenReceive -= 1;
                    }
                }
                if (client.Player.DExpTime > 0)
                {
                    client.Player.DExpTime -= 1;
                    if (client.Player.DExpTime == 0)
                        client.Player.RateExp = 1;
                }
            }
            catch (Exception e)
            {
                MyConsole.WriteException(e);
            }
        } //Done 
        public static void StaminaCallback(Client.GameClient client)
        {
            try
            {
                if (client == null || !client.FullLoading)
                    return;
                Extensions.Time32 Timer = Extensions.Time32.Now;
                if (client.Player.Alive && !client.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.Fly))
                {
                    byte MaxStamina = (byte)(client.Player.HeavenBlessing > 0 ? 150 : 100);
                    if (client.Equipment.UseMonkEpicWeapon)
                    {
                        MsgSpell user_spell = null;
                        if (client.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.GraceofHeaven, out user_spell))
                        {
                            Database.MagicType.Magic DBSpell = Database.Server.Magic[user_spell.ID][user_spell.Level];
                            MaxStamina += (byte)DBSpell.Damage;
                        }
                    }
                    if (client.Player.Stamina < MaxStamina)
                    {
                        ushort addstamin = 0;
                        if (client.Player.Action == Role.Flags.ConquerAction.Sit)
                            addstamin += 8;
                        else
                            addstamin += 3;

                        if (client.Player.Map == 2579 || client.Player.Map == 2569 || client.Player.Map == 2567 || client.Player.Map == 8601 || client.Player.Map == 8602 || client.Player.Map == 7701 || client.Player.Map == 7702 || client.Player.Map == 7703 || client.Player.Map == 7704 || client.Player.Map == 7705 || client.Player.Map == 7706 || client.Player.Map == 7707 || client.Player.Map == 7708 || client.Player.Map == 7709 || client.Player.Map == 7710 || client.Player.Map == 7711 || client.Player.Map == 7712)
                            addstamin += 100;
                        if (client.Player.ContainFlag(MsgUpdate.Flags.WindWalkerFan))
                        {
                            if (Timer > client.Player.FanRecoverStamin.AddSeconds(5))
                            {
                                addstamin += (ushort)(addstamin * 50 / 100);
                                client.Player.FanRecoverStamin = Extensions.Time32.Now;
                            }
                        }
                        client.Player.Stamina = (ushort)Math.Min((int)(client.Player.Stamina + addstamin), MaxStamina);
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            client.Player.SendUpdate(stream, client.Player.Stamina, Game.MsgServer.MsgUpdate.DataType.Stamina);
                        }
                    }
                    if (client.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.Ride))
                    {
                        if (client.Player.CheckInvokeFlag(Game.MsgServer.MsgUpdate.Flags.Ride, Timer))
                        {
                            if (client.Vigor < client.Status.MaxVigor)
                            {
                                client.Vigor = (ushort)Math.Min(client.Vigor + 2, client.Status.MaxVigor);

                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    client.Send(stream.ServerInfoCreate(MsgServerInfo.Action.Vigor, client.Vigor));
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MyConsole.WriteException(e);
            }

        }//Done 
        public static void CheckItemsTime(Client.GameClient client)//Done
        {
            try
            {
                if (client == null || !client.FullLoading || client.Player == null)
                    return;
                foreach (var item in client.AllMyTimeItems())
                {
                    if (DateTime.Now < item.EndDate)
                        continue;
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        if (client.Inventory.ClientItems.ContainsKey(item.UID))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                            continue;
                        }
                        if (client.Equipment.ClientItems.ContainsKey(item.UID))
                        {
                            Role.Flags.ConquerItem position = (Role.Flags.ConquerItem)Database.ItemType.ItemPosition(item.ITEM_ID);
                            client.Equipment.Remove(position, stream);
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                            client.Equipment.QueryEquipment(client.Equipment.Alternante);
                        }
                        if (client.MyWardrobe.ContainUID(item.UID))
                        {
                            MsgGameItem items2;
                            if (client.MyWardrobe.RemoveItem(item.UID, out items2))
                            {
                                if (item.IsEquip)
                                {
                                    client.Equipment.Remove((Role.Flags.ConquerItem)Database.ItemType.ItemPosition(item.ITEM_ID), stream);
                                    Game.MsgServer.MsgCoatStorage.CoatStorage store = new Game.MsgServer.MsgCoatStorage.CoatStorage();
                                    store.ActionID = MsgCoatStorage.Action.TakeOff;
                                    store.dwparam1 = items2.UID;
                                    store.dwpram2 = items2.ITEM_ID;
                                    client.Send(stream.CreateCoatStorage(store));
                                    client.Equipment.QueryEquipment(client.Equipment.Alternante);
                                }
                                client.Inventory.Update(items2, Role.Instance.AddMode.REMOVE, stream);
                                Game.MsgServer.MsgCoatStorage.CoatStorage astore = new Game.MsgServer.MsgCoatStorage.CoatStorage();
                                astore.ActionID = MsgCoatStorage.Action.Retrive;
                                astore.dwparam1 = items2.UID;
                                astore.dwpram2 = items2.ITEM_ID;
                                client.Send(stream.CreateCoatStorage(astore));
                                client.Send(stream.CreateCoatStorage(astore));
                            }
                        }
                        foreach (var Wh in client.Warehouse.ClientItems)
                        {
                            foreach (var item2 in Wh.Value.Values)
                            {
                                if (item2.UID == item.UID)
                                {
                                    client.Warehouse.RemoveItem(item2.UID, Wh.Key, stream);
                                    client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                }
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                MyConsole.WriteException(ex);
            }
        }
    }
}
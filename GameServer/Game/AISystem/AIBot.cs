using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeathWish.Client;
using DeathWish.Game.MsgServer;
using DeathWish.Role;
using DeathWish.ServerSockets;

namespace DeathWish.Game.AISystem
{
    public enum BotType
    {
        AIRoom,
      //  AIAutoHuntOffline,
    }
    public enum BotLevel
    {
        Noob = 0,
        Easy = 1,
        Normal = 2,
        Medium = 3,
        Hard = 4,
        Insane = 5,
        ProPlayer = 6,
       // MoAli = 7,
    }
    public enum BotSkill
    {
        Fastblade = 1045,
        ScrenSword = 1046,
       // Hercules = 1115,
    }
    public unsafe class AIBot
    {
        public static ConcurrentDictionary<uint, GameClient> Pool = new ConcurrentDictionary<uint, GameClient>();
        public GameClient Client = null;
        public GameClient Bot = null;
        public IMapObj Target = null;
        public int HP;
        public Time32 LastBotJump;
        public Time32 LastAttack;
        private int JumpSpeed = 0;
        public uint UID;
        private int Accuracy = 0;
      //q  public uint OwnerUID = 0;
        private int ShootChance = 0;
        public BotSkill Attack;
        public BotType Type;
        public DateTime Timer;
        public AIBot(GameClient Owner)
        {
            Client = Owner;
           // OwnerUID = Client.Player.UID ;
           // load();
        }
        public void SetLevel(BotLevel Level)
        {
            switch (Level)
            {
                case BotLevel.Noob:
                    {
                        JumpSpeed = 1600;
                        ShootChance = 10;
                        Accuracy = 5;
                        break;
                    }
                case BotLevel.Easy:
                    {
                        JumpSpeed = 1500;
                        ShootChance = 25;
                        Accuracy = 10;
                        break;
                    }
                case BotLevel.Normal:
                    {
                        JumpSpeed = 1400;
                        ShootChance = 35;
                        Accuracy = 20;
                        break;
                    }
                case BotLevel.Medium:
                    {
                        JumpSpeed = 1300;
                        ShootChance = 55;
                        Accuracy = 40;
                        break;
                    }
                case BotLevel.Hard:
                    {
                        JumpSpeed = 1200;
                        ShootChance = 70;
                        Accuracy = 50;
                        break;
                    }
                case BotLevel.Insane:
                    {
                        JumpSpeed = 1100;
                        ShootChance = 90;
                        Accuracy = 80;
                        break;
                    }
                case BotLevel.ProPlayer:
                    {
                        JumpSpeed = 1000;
                        ShootChance = 100;
                        Accuracy = 100;
                        break;
                    }
            }
        }
        public unsafe IMapObj GetTargetAIRoom()
        {
            if (Target == null)
            {
                foreach (IMapObj Obj in Bot.Player.View.Roles(MapObjectType.Player))
                {
                    if (Core.GetDistance(Bot.Player.X, Bot.Player.Y, Obj.X, Obj.Y) <= 18)
                    {
                        if (Obj.Alive)
                        {
                            Target = Obj;
                            break;
                        }
                    }
                    else return null;
                    break;
                }
            }
            return Target;
        }
        public void JumpingAIRoom()
        {
            if (Bot != null && Bot.Map != null && Bot.Player != null && Bot.Player.Map == 700)
            {
                if (Time32.Now >= Bot.MyAI.LastBotJump.AddMilliseconds(Bot.MyAI.JumpSpeed))
                {
                    ushort X = (ushort)Program.GetRandom.Next(Bot.Player.X - 1, Bot.Player.X + 1);
                    ushort Y = (ushort)Program.GetRandom.Next(Bot.Player.Y - 1, Bot.Player.Y + 1);
                    if (Target != null)
                    {
                        X = (ushort)Program.GetRandom.Next(Target.X - 3, Target.X + 3);
                        Y = (ushort)Program.GetRandom.Next(Target.Y - 3, Target.Y + 3);
                    }
                    if (!Bot.Map.ValidLocation(X, Y))
                    {
                        X = Bot.Player.X;
                        Y = Bot.Player.Y;
                    }
                    using (var rec = new RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        var action = new ActionQuery()
                        {
                            ObjId = Bot.Player.UID,
                            Type = ActionType.Jump,
                            wParam1 = Bot.Player.X,
                            wParam2 = Bot.Player.Y,
                            dwParam_Lo = X,
                            dwParam_Hi = Y,
                        };
                        Bot.Player.View.SendView(stream.ActionCreate(&action), true);
                        Bot.Player.Angle = Core.GetAngle(Bot.Player.X, Bot.Player.Y, X, Y);
                        Bot.Player.Action = Flags.ConquerAction.Jump;
                        Bot.Map.View.MoveTo<IMapObj>(Bot.Player, X, Y);
                        Bot.Player.X = X;
                        Bot.Player.Y = Y;
                        Bot.Player.View.Role(false, stream);
                        Bot.Player.LastMove = DateTime.Now;
                        LastBotJump = Time32.Now;
                    }
                }
            }
        }
        public void AttackingAIRoom()
        {
            GameClient attacked;
            if (Bot != null && Bot.Map != null && Bot.Player.View != null && Bot.Player != null && Bot.Player.HitPoints > 0 && Bot.Player.Map == 700)
            {
                if (Target == null) return;
                if (Time32.Now >= Bot.MyAI.LastAttack.AddMilliseconds(400))
                {
                    if (Database.Server.GamePoll.TryGetValue(Target.UID, out attacked))
                    {
                        using (var rec = new RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            ushort SpellID = 0;
                            if (Database.AtributesStatus.IsTrojan(Bot.Player.Class))
                            {
                                if (Attack == BotSkill.Fastblade)
                                {
                                    SpellID = 1045;
                                }
                                else if (Attack == BotSkill.ScrenSword)
                                {
                                    SpellID = 1046;
                                }
                                //else if (Attack == BotSkill.Hercules)
                                //{
                                //    SpellID = 1115;
                                //}
                            }
                            if (!Bot.Player.ContainFlag(MsgUpdate.Flags.Cyclone) && !Bot.Player.ContainFlag(MsgUpdate.Flags.FatalStrike))
                            {
                                Dictionary<ushort, Database.MagicType.Magic> Spells;
                                if (Database.Server.Magic.TryGetValue(SpellID, out Spells))
                                {
                                    MsgSpell ClientSpell;
                                    if (Bot.MySpells.ClientSpells.TryGetValue(SpellID, out ClientSpell))
                                    {
                                        Database.MagicType.Magic spell;
                                        if (Spells.TryGetValue(ClientSpell.Level, out spell))
                                        {
                                            if (SpellID != 0)
                                            {
                                                if (MyMath.Success(Bot.MyAI.ShootChance))
                                                {
                                                    var action = new InteractQuery();
                                                    action.AtkType = MsgAttackPacket.AttackID.Magic;
                                                    action.UID = Bot.Player.UID;
                                                    action.OpponentUID = attacked.Player.UID;
                                                    if (MyMath.Success(Bot.MyAI.Accuracy))
                                                    {
                                                        action.X = Target.X;
                                                        action.Y = Target.Y;
                                                    }
                                                    else
                                                    {
                                                        action.X = (ushort)(Target.X + 1);
                                                        action.Y = (ushort)(Target.Y + 1);
                                                    }
                                                    action.Damage = (int)SpellID;
                                                    action.SpellID = (ushort)SpellID;
                                                    MsgAttackPacket.Process(Bot, action);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    Bot.MyAI.LastAttack = Time32.Now;
                }
            }
        }
        public void RemoveingAIRoom()
        {
            if (Target == null) return;
            GameClient client;
            if (!Database.Server.GamePoll.TryGetValue(Target.UID, out client))
            {
                using (var rec = new RecycledPacket())
                {
                    var stream = rec.GetStream();
                    UnlimitedArenaRooms.Maps.Remove(Bot.Player.Map);
                    Bot.Map.RemoveCustom(Bot);
                }
            }
            if (Database.Server.GamePoll.TryGetValue(Target.UID, out client))
            {
                if (client.Player.Map != Bot.Player.Map)
                {
                    using (var rec = new RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        UnlimitedArenaRooms.Maps.Remove(Bot.Player.Map);
                        Bot.Map.RemoveCustom(Bot);
                    }
                }
            }
        }
        //public void AttackingAutoHunt()
        //{
        //    try
        //    {
        //        using (var rec = new ServerSockets.RecycledPacket())
        //        {
        //            var stream = rec.GetStream();
        //            if (Bot != null && Bot.Map != null && Bot.Player.View != null && Bot.Player != null && Bot.Player.HitPoints > 0 && Bot.Player.OnAutoHunt)
        //            {
        //                ushort xx = 0, yy = 0;
        //                Game.MsgMonster.MonsterRole TTarget = null;
        //                foreach (var target in Bot.Player.View.Roles(Role.MapObjectType.Monster))
        //                {
        //                    var attacked = target as Game.MsgMonster.MonsterRole;
        //                    if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackMonster.Verified(Bot, attacked, null))
        //                    {
        //                        xx = attacked.X;
        //                        yy = attacked.Y;
        //                        TTarget = attacked;
        //                        break;
        //                    }
        //                }
        //                if (TTarget != null && xx != 0 && yy != 0 && Time32.Now >= Bot.MyAI.LastAttack.AddMilliseconds(1200))
        //                {
        //                    var AttackPaket = new InteractQuery();
        //                    AttackPaket.OpponentUID = 0;
        //                    AttackPaket.UID = Bot.Player.UID;
        //                    AttackPaket.X = xx;
        //                    AttackPaket.Y = yy;
        //                    AttackPaket.SpellID = (ushort)Role.Flags.SpellID.ScatterFire;
        //                    MsgAttackPacket.ProcescMagic(Bot, stream, AttackPaket, true);
        //                    Bot.MyAI.LastAttack = Time32.Now;
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex);
        //    }
        //}
        //public void JumpingAutoHunt()
        //{
        //    if (Bot != null && Bot.Map != null && Bot.Player != null && Bot.Player.OnAutoHunt)
        //    {
        //        ushort X = (ushort)Program.GetRandom.Next(Bot.Player.X - 1, Bot.Player.X + 1);
        //        ushort Y = (ushort)Program.GetRandom.Next(Bot.Player.Y - 1, Bot.Player.Y + 1);
        //        bool stop = false;
        //        for (int xx = 0; xx < 100; xx++)
        //        {
        //            foreach (var target in Bot.Map.View.GetAllMapRoles(Role.MapObjectType.Monster))
        //            {
        //                if (Role.Core.GetDistance(target.X, target.Y, Bot.Player.X, Bot.Player.Y) <= xx)
        //                {
        //                    var attacked = target as Game.MsgMonster.MonsterRole;
        //                    if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackMonster.Verified(Bot, attacked, null))
        //                    {
        //                        if (attacked != null)
        //                        {
        //                            X = (ushort)Program.GetRandom.Next(attacked.X - 3, attacked.X + 3);
        //                            Y = (ushort)Program.GetRandom.Next(attacked.Y - 3, attacked.Y + 3);
        //                        }
        //                        if (!Bot.Map.ValidLocation(X, Y))
        //                        {
        //                            X = Bot.Player.X;
        //                            Y = Bot.Player.Y;
        //                            continue;
        //                        }
        //                        stop = true;
        //                        break;
        //                    }
        //                }
        //            }
        //            if (stop)
        //                break;
        //        }
        //        if (X != 0 && Y != 0 && Time32.Now >= Bot.MyAI.LastBotJump.AddMilliseconds(1400))
        //        {
        //            using (var rec = new ServerSockets.RecycledPacket())
        //            {
        //                var stream = rec.GetStream();
        //                var action = new ActionQuery()
        //                {
        //                    ObjId = Bot.Player.UID,
        //                    Type = ActionType.Jump,
        //                    wParam1 = Bot.Player.X,
        //                    wParam2 = Bot.Player.Y,
        //                    dwParam_Lo = X,
        //                    dwParam_Hi = Y,
        //                };
        //                Bot.Player.View.SendView(stream.ActionCreate(&action), true);
        //                Bot.Player.Angle = Role.Core.GetAngle(Bot.Player.X, Bot.Player.Y, X, Y);
        //                Bot.Player.Action = Role.Flags.ConquerAction.Jump;
        //                Bot.Map.View.MoveTo<Role.IMapObj>(Bot.Player, X, Y);
        //                Bot.Player.X = X;
        //                Bot.Player.Y = Y;
        //                Bot.Player.View.Role(false, stream);
        //                LastBotJump = Time32.Now;
        //            }
        //        }
        //    }
        //}
        //public void RemoveingAutoHunt()
        //{
        //    if (DateTime.Now >= Timer)
        //    {
        //        using (var rec = new ServerSockets.RecycledPacket())
        //        {
        //            var stream = rec.GetStream();
        //            if (Database.Server.GamePoll.ContainsKey(Bot.Player.UID - 60000000))
        //            {
        //                var User = Database.Server.GamePoll[Bot.Player.UID - 60000000];
        //                User.Player.ConquerPoints += Bot.Player.ConquerPoints;
        //                User.CreateBoxDialog("[Bot] time is over [CPS : ]" + Bot.Player.ConquerPoints + "");
        //                Bot.Player.ConquerPoints = 0;
        //            }
        //            else
        //            {
        //                uint ConquerPoints = 0;
        //                var reader = new WindowsAPI.IniFile("\\Users\\" + (Bot.Player.UID - 60000000) + ".ini");
        //                ConquerPoints = reader.ReadUInt32("Character", "ConquerPoints", 0);
        //                ConquerPoints += Bot.Player.ConquerPoints;
        //                Bot.Player.ConquerPoints = 0;
        //                reader.Write<uint>("Character", "ConquerPoints", ConquerPoints);
        //                //offline
        //            }
        //            Bot.Player.View.Clear(stream);
        //            Bot.Map.View.LeaveMap<Role.IMapObj>(Bot.Player);
        //            Database.Server.GamePoll.Remove(Bot.Player.UID);
        //            Pool.Remove(Bot.Player.UID);
        //        }
        //    }
        //}
        //public static void Save()
        //{
        //    foreach(var Bot in AIBot.Pool.Values)
        //    {
        //        WindowsAPI.IniFile write = new WindowsAPI.IniFile("\\UserBots\\" + (Bot.Player.UID - 60000000) + ".ini");
        //        write.Write<uint>("Bot", "Cps", Bot.Player.ConquerPoints);
        //    }
        //}
        //public void load()
        //{

        //    WindowsAPI.IniFile reader = new WindowsAPI.IniFile("\\UserBots\\");
        //    Bot.Player.ConquerPoints = reader.ReadUInt32("Bot", "Cps", 0);
        
        //}
    }
}
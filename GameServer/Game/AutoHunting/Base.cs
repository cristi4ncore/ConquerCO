using DeathWish.Client;
using DeathWish.Database;
using DeathWish.Game.MsgServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeathWish.AutoHunting
{
    public class Base
    {
        private static Random RobotRandom = new Random();

        private ushort[] SkillRobotTrojan = new ushort[] { 1045, 1046, 1115 };//FastBlade,ScentSword,Hercules
        private ushort[] SkillRobotArcher = new ushort[] { 8001 };//Scatter
        private ushort[] SkillRobotNinja = new ushort[] { 6000/*, 6001 */};//TwofoldBlades,ToxicFog
        private ushort[] SkillRobotMonk = new ushort[] { 10381, 10415 };//RadiantPalm,WhirlwindKick
        private ushort[] SkillRobotWater = new ushort[] { 1000 };//Thunder
        private ushort[] SkillRobotFire = new ushort[] { 1002 };//Tornado

        private ushort[] SkillRobotPirate = new ushort[] { 11050, 11110 };//Tornado

        private ushort[] SkillRobotAttacked = new ushort[] { 6000, 10381, 10415, 1000, 1002 };
        private ushort[] SkillXPRobot = new ushort[] { 1110, 6011 };//CycloneXP FatalStrike

        public const uint GetDistance = 8;
        public bool Enable;
        public uint DirectionChange;
        public uint NextDirection;
        public DateTime AttackStamp;

        GameClient client;

        private ushort X;
        private ushort Y;

        public Base(GameClient obj)
        {
            client = obj;
        }
        public virtual void Start()
        {
            Enable = true;
        }
        public virtual void End()
        {
            Enable = false;
            client.OnAutoAttack = false;
        }
        public virtual void Run()
        {
            if (Enable)
            {
                if (DateTime.Now < AttackStamp.AddMilliseconds(1000))
                    return;
                var mobs = client.Player.View.Roles(Role.MapObjectType.Monster).Where(e => Role.Core.GetDistance(e.X, e.Y, client.Player.X, client.Player.Y) < GetDistance).ToArray();
                if (mobs != null)
                {
                    if (mobs.Length > 0)
                    {

                        var Obj = mobs[RobotRandom.Next(0, mobs.Length)];
                        var entity = Obj as Game.MsgMonster.MonsterRole;
                        if (entity.HitPoints > 0 && !entity.Name.Contains("Guard"))
                        {
                            ushort X = (ushort)(Obj.X - 2), Y = Obj.Y;
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                Game.MsgServer.InterActionWalk inter = new Game.MsgServer.InterActionWalk()
                                {
                                    Mode = MsgInterAction.Action.Jump,
                                    X = X,
                                    Y = Y,
                                    UID = client.Player.UID,
                                    OponentUID = 1
                                };
                                unsafe
                                {
                                    client.Player.View.SendView(stream.InterActionWalk(&inter), true);
                                }
                                client.Player.Angle = Role.Core.GetAngle(client.Player.X, client.Player.Y, X, Y);
                                client.Player.Action = Role.Flags.ConquerAction.Jump;
                                Role.Core.IncXY(client.Player.Angle, ref X, ref Y);
                                client.Map.View.MoveTo<Role.IMapObj>(client.Player, X, Y);
                                client.Player.X = X;
                                client.Player.Y = Y;
                                client.Player.View.Role(false, stream);
                                DirectionChange = 0;
                                client.Player.LastMove = DateTime.Now;
                                Skill(entity);
                                NextDirection++;
                                AttackStamp = DateTime.Now;

                            }
                        }
                        else DirectionChange++;
                    }
                    else DirectionChange++;
                }
                else DirectionChange++;

                if (NextDirection > 7)
                {
                    var nexmobs = client.Map.View.GetAllMapRoles(Role.MapObjectType.Monster).Where(e => Role.Core.GetDistance(e.X, e.Y, client.Player.X, client.Player.Y) >= 18 && Role.Core.GetDistance(e.X, e.Y, client.Player.X, client.Player.Y) <= 31).ToArray();
                    if (nexmobs != null)
                    {
                        if (nexmobs.Length > 0)
                        {
                            var Obj = nexmobs[RobotRandom.Next(0, nexmobs.Length)];
                            var entity = Obj as Game.MsgMonster.MonsterRole;
                            if (entity.HitPoints > 0 && !entity.Name.Contains("Guard"))
                            {
                                Game.MsgServer.AttackHandler.Algoritms.InLineAlgorithm Line = new Game.MsgServer.AttackHandler.Algoritms.InLineAlgorithm(client.Player.X, entity.X, client.Player.Y, entity.Y, client.Map, 15, 0);
                                X = (ushort)Line.lcoords[(int)(Line.lcoords.Count() - 1)].X;
                                Y = (ushort)Line.lcoords[(int)(Line.lcoords.Count() - 1)].Y;
                                if (client.Map.AddGroundItem(ref X, ref Y))
                                {
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        Game.MsgServer.InterActionWalk inter = new Game.MsgServer.InterActionWalk()
                                        {
                                            Mode = MsgInterAction.Action.Jump,
                                            X = X,
                                            Y = Y,
                                            UID = client.Player.UID,
                                            OponentUID = 1
                                        };
                                        unsafe
                                        {
                                            client.Player.View.SendView(stream.InterActionWalk(&inter), true);
                                        }
                                        client.Player.Angle = Role.Core.GetAngle(client.Player.X, client.Player.Y, X, Y);
                                        client.Player.Action = Role.Flags.ConquerAction.Jump;
                                        Role.Core.IncXY(client.Player.Angle, ref X, ref Y);
                                        client.Map.View.MoveTo<Role.IMapObj>(client.Player, X, Y);
                                        client.Player.X = X;
                                        client.Player.Y = Y;
                                        client.Player.View.Role(false, stream);
                                        client.Player.LastMove = DateTime.Now;
                                        NextDirection = 0;
                                        DirectionChange = 0;
                                    }
                                }
                            }
                            else DirectionChange++;
                        }
                        else DirectionChange++;
                    }
                    else DirectionChange++;
                }

                if (DirectionChange > 3)
                {
                    foreach (var mapObj in client.Map.View.GetAllMapRoles(Role.MapObjectType.Monster))
                    {
                        var entity = mapObj as Game.MsgMonster.MonsterRole;
                        if (entity.HitPoints > 0 && !entity.Name.Contains("Guard"))
                        {
                            Game.MsgServer.AttackHandler.Algoritms.InLineAlgorithm Line = new Game.MsgServer.AttackHandler.Algoritms.InLineAlgorithm(client.Player.X, entity.X, client.Player.Y, entity.Y, client.Map, 15, 0);
                            X = (ushort)Line.lcoords[(int)(Line.lcoords.Count() - 1)].X;
                            Y = (ushort)Line.lcoords[(int)(Line.lcoords.Count() - 1)].Y;
                            if (client.Map.AddGroundItem(ref X, ref Y, 3))
                            {

                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    Game.MsgServer.InterActionWalk inter = new Game.MsgServer.InterActionWalk()
                                    {
                                        Mode = MsgInterAction.Action.Jump,
                                        X = X,
                                        Y = Y,
                                        UID = client.Player.UID,
                                        OponentUID = 1
                                    };
                                    unsafe
                                    {
                                        client.Player.View.SendView(stream.InterActionWalk(&inter), true);
                                    }
                                    client.Player.Angle = Role.Core.GetAngle(client.Player.X, client.Player.Y, X, Y);
                                    client.Player.Action = Role.Flags.ConquerAction.Jump;
                                    Role.Core.IncXY(client.Player.Angle, ref X, ref Y);
                                    client.Map.View.MoveTo<Role.IMapObj>(client.Player, X, Y);
                                    client.Player.X = X;
                                    client.Player.Y = Y;
                                    client.Player.View.Role(false, stream);
                                    client.Player.LastMove = DateTime.Now;
                                }
                                break;
                            }
                        }
                    }
                }
            }
        }
        public virtual void Skill(Game.MsgMonster.MonsterRole Obj)
        {
            if (Obj == null) return;
            if (Obj.HitPoints > 0 && !Obj.ContainFlag(MsgUpdate.Flags.Ghost) && !Obj.Name.Contains("Guard"))
            {
                ushort SpellID = 0;

                if (client.Player.Class >= 10 && client.Player.Class <= 15) SpellID = SkillRobotTrojan[RobotRandom.Next(SkillRobotTrojan.Length)];

                if (client.Player.Class >= 40 && client.Player.Class <= 45) SpellID = SkillRobotArcher[RobotRandom.Next(SkillRobotArcher.Length)];

                if (client.Player.Class >= 50 && client.Player.Class <= 55) SpellID = SkillRobotNinja[RobotRandom.Next(SkillRobotNinja.Length)];

                if (client.Player.Class >= 60 && client.Player.Class <= 65) SpellID = SkillRobotMonk[RobotRandom.Next(SkillRobotMonk.Length)];

                if (client.Player.Class >= 70 && client.Player.Class <= 75) SpellID = SkillRobotPirate[RobotRandom.Next(SkillRobotPirate.Length)];

                if (client.Player.Class >= 130 && client.Player.Class <= 135) SpellID = SkillRobotWater[RobotRandom.Next(SkillRobotWater.Length)];

                if (client.Player.Class >= 140 && client.Player.Class <= 145) SpellID = SkillRobotFire[RobotRandom.Next(SkillRobotFire.Length)];

                if (!client.Player.ContainFlag(MsgUpdate.Flags.Cyclone) && !client.Player.ContainFlag(MsgUpdate.Flags.FatalStrike) && client.Player.ContainFlag(MsgUpdate.Flags.XPList))
                {
                    List<ushort> SkillsXP = new List<ushort>();
                    ushort SkillXP = 0;
                    for (int i = 0; i < SkillXPRobot.Length; i++)
                    {
                        if (client.MySpells.ClientSpells.ContainsKey(SkillXPRobot[i]))
                            SkillsXP.Add(SkillXPRobot[i]);
                    }
                    if (SkillsXP.Count > 0)
                    {
                        SkillXP = SkillsXP[(ushort)RobotRandom.Next(SkillsXP.Count)];
                        if (SkillXP != 0)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();

                                InteractQuery action = new InteractQuery()
                                {
                                    AtkType = MsgAttackPacket.AttackID.Magic,
                                    UID = client.Player.UID,
                                    OpponentUID = client.Player.UID,
                                    X = client.Player.X,
                                    Y = client.Player.Y,
                                    Damage = (int)SkillXP
                                };
                                MsgAttackPacket.Process(client, action);
                            }
                        }
                    }
                }

                if (!client.Player.ContainFlag(MsgUpdate.Flags.Cyclone) && !client.Player.ContainFlag(MsgUpdate.Flags.FatalStrike))
                {
                    Dictionary<ushort, Database.MagicType.Magic> Spells;
                    if (Server.Magic.TryGetValue(SpellID, out Spells))
                    {
                        MsgSpell ClientSpell;
                        if (client.MySpells.ClientSpells.TryGetValue(SpellID, out ClientSpell))
                        {
                            Database.MagicType.Magic spell;
                            if (Spells.TryGetValue(ClientSpell.Level, out spell))
                            {
                                if (SpellID != 0 && spell != null && spell.UseStamina <= client.Player.Stamina && spell.UseMana <= client.Player.Mana)
                                {
                                    if (client.Player.Rate(50) || client.Player.Class >= 130 && client.Player.Class <= 135 || client.Player.Class >= 140 && client.Player.Class <= 145)
                                    {
                                        if (!(X == client.Player.X && Y == client.Player.Y) || X == 0 && Y == 0)
                                        {
                                            X = client.Player.X;
                                            Y = client.Player.Y;
                                            AttackStamp = DateTime.Now;
                                        }
                                        using (var rec = new ServerSockets.RecycledPacket())
                                        {
                                            var stream = rec.GetStream();
                                            InteractQuery action = new InteractQuery();
                                            action.AtkType = MsgAttackPacket.AttackID.Magic;
                                            action.UID = client.Player.UID;
                                            if (SkillRobotAttacked.Contains(SpellID))
                                                action.OpponentUID = Obj.UID;
                                            action.X = Obj.X;
                                            action.Y = Obj.Y;
                                            action.Damage = (int)SpellID;
                                            action.SpellID = (ushort)SpellID;
                                            MsgAttackPacket.Process(client, action);
                                        }
                                        return;
                                    }
                                }
                            }
                        }
                    }
                }

                if (Role.Core.GetDistance(Obj.X, Obj.Y, client.Player.X, client.Player.Y) <= 2 || client.Player.ContainFlag(MsgUpdate.Flags.FatalStrike))
                {
                    if (client.Player.Class != 135 && client.Player.Class != 145)
                    {
                        if (!(X == client.Player.X && Y == client.Player.Y) || X == 0 && Y == 0)
                        {
                            X = client.Player.X;
                            Y = client.Player.Y;
                            AttackStamp = DateTime.Now;
                        }
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            InteractQuery action = new InteractQuery();
                            action.AtkType = MsgAttackPacket.AttackID.Physical;
                            action.Damage = 0;
                            action.SpellID = 0;
                            action.SpellLevel = 0;
                            if (client.Player.Class >= 40 && client.Player.Class <= 45)
                            {
                                action.AtkType = MsgAttackPacket.AttackID.Magic;
                                action.Damage = 8001;
                                action.SpellID = 8001;
                            }
                            action.UID = client.Player.UID;
                            action.OpponentUID = Obj.UID;
                            action.X = Obj.X;
                            action.Y = Obj.Y;
                            MsgAttackPacket.Process(client, action);
                        }
                        return;
                    }
                }
            }
        }
    }
}

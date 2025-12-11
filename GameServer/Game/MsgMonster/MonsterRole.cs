using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeathWish.Game.MsgServer;
using DeathWish.Game.MsgFloorItem;
using DeathWish.MsgInterServer.Packets;
using DeathWish.Game.MsgNpc;
using DeathWish.Game.MsgTournaments;
using DeathWish.Client;
using DeathWish.Database;
using DeathWish.Role;

namespace DeathWish.Game.MsgMonster
{
    public unsafe class MonsterRole : Role.IMapObj
    {
        public const ushort
    UintStamp = 4,
    Uint_Mesh = 8,
    Uint_UID = 12,
    BitVector32 = 26,
    Uint16_HitPoints = 94,//90
    Uint16_Level = 100,
    Uint16_X = 102,
    Uint16_Y = 104,
    Byte_Fascing = 108,
    Byte_Action = 109,
    Byte_Boss = 203,

    Str_Count = 257,//246,//244,
    Str_NameLenght = 258,//247,//245,
    Str_Name = 259,//248,//246,
    Uint16_PLenght = 260;//249;//247;

        public class ScoreBoard
        {
            public string Name;
            public uint ScoreDmg;
        }
        public Dictionary<uint, ScoreBoard> Scores = new Dictionary<uint, ScoreBoard>();
        public struct DropBag
        {
            public uint Item;
            public int Rate;
        }

        public DateTime RemoveFloor = DateTime.Now;
        public int StampFloorSecounds = 0;

        public static List<uint> SpecialMonsters = new List<uint>()
        {
            20070,
            3130,
            3134,
            20300,
            213883,
            20300,
            20160,
            29370,
            29360,
            29300
            
            
        };

        public Client.GameClient AttackerScarofEarthl;
        public Database.MagicType.Magic ScarofEarthl;

        public int ExtraDamage { get { return Family.extra_damage; } }
        public int BattlePower { get { return Family.extra_battlelev; } }
        public bool AllowDynamic { get; set; }
        public bool IsTrap() { return false; }
        public uint IndexInScreen { get; set; }

        public Client.GameClient OwnerFloor;
        public Database.MagicType.Magic DBSpell;
        public ushort SpellLevel = 0;
        public DateTime FloorStampTimer = new DateTime();
        public bool IsFloor = false;
        public Game.MsgFloorItem.MsgItemPacket FloorPacket;

        public Dictionary<uint, MsgBossHarmRankingEntry> Hunters = new Dictionary<uint, MsgBossHarmRankingEntry>();
        public object scoreObj = new object();
        public bool BlackSpot = false;
        public Extensions.Time32 Stamp_BlackSpot = new Extensions.Time32();


        public int SizeAdd { get { return Family.AttackRange; } }

        public byte PoisonLevel = 0;

        private Extensions.Time32 DeadStamp = new Extensions.Time32();
        private Extensions.Time32 FadeAway = new Extensions.Time32();
        public Extensions.Time32 RespawnStamp = new Extensions.Time32();
        public Extensions.Time32 MoveStamp = new Extensions.Time32();
        public bool logged = false;

        public bool CanRespawn(Role.GameMap map)
        {
            Extensions.Time32 Now = Extensions.Time32.Now;
            if (Now > RespawnStamp)
            {
                if (!map.MonsterOnTile(RespawnX, RespawnY))
                {
                    return true;
                }
            }
            return false;

        }
        public uint GetScorePosition(Client.GameClient killer)
        {
            lock (scoreObj)
            {
                if (this.Hunters == null) return 0;

                var a = Hunters.Values.OrderByDescending(p => p.HunterScore).ToArray();
                if (!Alive && !logged)
                {
                    string log = "[Bosses] Name: " + Name + " | Killer: " + killer.Player.Name + " | Hunters: ";
                    foreach (var hunter in a.Take(5))
                        log += hunter.HunterName + "     |      ";
                    Database.ServerDatabase.LoginQueue.Enqueue(log);
                    logged = true;
                }
                for (int x = 0; x < a.Length; x++)
                {
                    if (a[x].HunterUID == killer.Player.UID)
                        return (uint)(x + 1);
                }
                return 0;
            }
        }
        public void SendScore(ServerSockets.Packet stream, Client.GameClient killer, uint Damage)
        {
            try
            {
                if (!Program.RankableFamilyIds.Contains(Family.ID))
                    return;
                if (this.Hunters == null)
                    this.Hunters = new Dictionary<uint, MsgBossHarmRankingEntry>();
                if (this.Hunters.ContainsKey(killer.Player.UID))
                {
                    this.Hunters[killer.Player.UID].HunterScore += Damage;
                }
                else if (!this.Hunters.ContainsKey(killer.Player.UID))
                {
                    this.Hunters.Add(killer.Player.UID, new MsgBossHarmRankingEntry() { HunterUID = killer.Player.UID, HunterScore = Damage, Rank = 0, HunterName = killer.Player.Name, ServerID = killer.Player.ServerID });
                }
                if (this.Hunters.Count == 0)
                    return;
                var array = this.Hunters.Values.OrderByDescending(p => p.HunterScore).ToArray();
                for (int x = 0; x < array.Length; x++)
                {
                    array[x].Rank = (uint)(x + 1);
                }
                MsgServer.MsgBossHarmRanking Rank = new MsgServer.MsgBossHarmRanking();
                Rank.MonsterID = this.Family.ID;
                Rank.Type = MsgBossHarmRanking.RankAction.ShowRespondForTheRest;
                Rank.Hunters = array;
                this.Send(stream.CreateBossHarmRankList(Rank));
                array = null;
            }
            catch (Exception ex)
            {
                MyConsole.WriteException(ex);
                MyConsole.SaveException(ex);
            }
        }
        public void Respawn(bool SendEffect = true)
        {
            using (var rev = new ServerSockets.RecycledPacket())
            {
                var stream = rev.GetStream();

                ClearFlags(false);




                HitPoints = (uint)Family.MaxHealth;
                State = MobStatus.Idle;

                Game.MsgServer.ActionQuery action;

                action = new MsgServer.ActionQuery()
                {
                    ObjId = UID,
                    Type = MsgServer.ActionType.RemoveEntity
                };

                Send(stream.ActionCreate(&action));

                Send(GetArray(stream, false));

                if (SendEffect)
                {
                    action.Type = ActionType.ReviveMonster;
                    Send(stream.ActionCreate(&action));
                }



                if (Family.MaxHealth > ushort.MaxValue)
                {
                    Game.MsgServer.MsgUpdate Upd = new Game.MsgServer.MsgUpdate(stream, UID, 2);
                    stream = Upd.Append(stream, Game.MsgServer.MsgUpdate.DataType.MaxHitpoints, Family.MaxHealth);
                    stream = Upd.Append(stream, Game.MsgServer.MsgUpdate.DataType.Hitpoints, HitPoints);
                    Send(Upd.GetArray(stream));
                }

            }
        }
        public void SendSysMesage(string Messaj, Game.MsgServer.MsgMessage.ChatMode ChatType = Game.MsgServer.MsgMessage.ChatMode.TopLeft
          , Game.MsgServer.MsgMessage.MsgColor color = Game.MsgServer.MsgMessage.MsgColor.red)
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage(Messaj, color, ChatType).GetArray(stream));
            }
        }
        public void SendBossSysMesage(string KillerName, int StudyPoints, Game.MsgServer.MsgMessage.ChatMode ChatType = Game.MsgServer.MsgMessage.ChatMode.Center
          , Game.MsgServer.MsgMessage.MsgColor color = Game.MsgServer.MsgMessage.MsgColor.red)
        {

            SendSysMesage("The [ " + Name.ToString() + " ] Has Been Destroyed By The Team " + KillerName.ToString() + "`s - Killer Get 2500 Cps & DIABLO Spin & 15 Coins & Every One In Team Take 500 Cps   .", ChatType, color);

        }
        public void GetBossReward(ServerSockets.Packet stream, Client.GameClient killer)
        {
            killer.Player.ConquerPoints += ServerRates.BigMonster;
            string logs = "[High Cps]" + killer.Player.Name + " get " + ServerRates.BigMonster + " From BigMonster he have " + killer.Player.ConquerPoints + "";
            Database.ServerDatabase.LoginQueue.Enqueue(logs);
            killer.SendSysMesage("You have received " + ServerRates.BigMonster + "  Conquer Points for being the last hitter of " + Name + "!", MsgMessage.ChatMode.TopLeft, MsgMessage.MsgColor.red);
            foreach (var hunter in Hunters.Values)
            {
                if (Database.Server.GamePoll.ContainsKey(hunter.HunterUID))
                {
                    var client = Server.GamePoll[hunter.HunterUID];
                    switch (this.GetScorePosition(client))
                    {
                        case 1:
                            client.Player.ConquerPoints += ServerRates.BossRank1;
                            client.SendSysMesage("You have received  " + ServerRates.BossRank1 + " Conquer Points for being 1st hunter while defeating " + Name + "!", MsgMessage.ChatMode.TopLeft, MsgMessage.MsgColor.red);
                            break;
                        case 2:
                            client.Player.ConquerPoints += ServerRates.BossRank2;
                            client.SendSysMesage("You have received " + ServerRates.BossRank2 + " Conquer Points for being 2nd hunter while defeating " + Name + "!", MsgMessage.ChatMode.TopLeft, MsgMessage.MsgColor.red);
                            break;
                        case 3:
                            client.Player.ConquerPoints += ServerRates.BossRank3;
                            client.SendSysMesage("You have received " + ServerRates.BossRank3 + " Conquer Points for being 3rd hunter while defeating " + Name + "!", MsgMessage.ChatMode.TopLeft, MsgMessage.MsgColor.red);
                            break;
                    }
                }
            }
          
        }

        public void Dead(ServerSockets.Packet stream, Client.GameClient killer, uint aUID, Role.GameMap GameMap)
        {
            if (Alive)
            {

                if (IsFloor)
                {

                    FloorPacket.DropType = MsgFloorItem.MsgDropID.RemoveEffect;
                    if (FloorPacket.m_ID == Game.MsgFloorItem.MsgItemPacket.Thundercloud)
                    {
                        ActionQuery _action;

                        _action = new ActionQuery()
                        {
                            ObjId = this.FloorPacket.m_UID,
                            Type = ActionType.RemoveEntity
                        };

                        this.View.SendScreen(stream.ActionCreate(&_action), this.GMap);


                        GMap.View.LeaveMap<Role.IMapObj>(this);
                        HitPoints = 0;
                        GameMap.SetMonsterOnTile(X, Y, false);
                    }
                    else if (FloorPacket.m_ID == Game.MsgFloorItem.MsgItemPacket.AuroraLotus)
                    {
                        byte revivers = SpellLevel >= 6 ? (byte)2 : (byte)1;
                        foreach (var user in View.Roles(GameMap, Role.MapObjectType.Player))
                        {
                            if (revivers == 0)
                                break;
                            if (user.Alive == false)
                            {
                                if (Role.Core.GetDistance(user.X, user.Y, X, Y) < 5)
                                {
                                    revivers--;
                                    var player = user as Role.Player;
                                    if (player.ContainFlag(MsgUpdate.Flags.SoulShackle) == false)
                                        player.Revive(stream);

                                }
                            }
                            user.Send(GetArray(stream, false));
                        }
                        GMap.View.LeaveMap<Role.IMapObj>(this);
                    }
                    else if (FloorPacket.m_ID == Game.MsgFloorItem.MsgItemPacket.FlameLotus)
                    {
                        FloorPacket.DropType = MsgFloorItem.MsgDropID.RemoveEffect;

                        foreach (var user in View.Roles(GameMap, Role.MapObjectType.Player))
                        {
                            if (user.UID != OwnerFloor.Player.UID && Role.Core.GetDistance(user.X, user.Y, this.X, this.Y) < 5)
                            {
                                var player = user as Role.Player;

                                Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                Game.MsgServer.AttackHandler.Calculate.Magic.OnPlayer(this.OwnerFloor.Player, player, this.DBSpell, out AnimationObj);
                                Game.MsgServer.AttackHandler.ReceiveAttack.Player.Execute(AnimationObj, this.OwnerFloor, player);
                                AnimationObj.Damage = Game.MsgServer.AttackHandler.Calculate.Base.CalculateSoul(AnimationObj.Damage, 0);

                                InteractQuery Attack = new InteractQuery();
                                Attack.UID = this.UID;
                                Attack.OpponentUID = player.UID;
                                Attack.Damage = (int)AnimationObj.Damage;
                                Attack.Effect = AnimationObj.Effect;
                                Attack.X = player.X;
                                Attack.Y = player.Y;
                                Attack.AtkType = MsgAttackPacket.AttackID.Physical;

                                stream.InteractionCreate(&Attack);

                                player.View.SendView(stream, true);

                            }
                            user.Send(this.GetArray(stream, false));
                        }
                        foreach (var obj in View.Roles(GameMap, Role.MapObjectType.Monster))
                        {
                            if (obj.UID != this.UID && Role.Core.GetDistance(obj.X, obj.Y, this.X, this.Y) < 5)
                            {
                                var monster = obj as Game.MsgMonster.MonsterRole;

                                Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                Game.MsgServer.AttackHandler.Calculate.Magic.OnMonster(this.OwnerFloor.Player, monster, this.DBSpell, out AnimationObj);
                                Game.MsgServer.AttackHandler.ReceiveAttack.Monster.Execute(stream, AnimationObj, monster.OwnerFloor, monster);
                                AnimationObj.Damage = Game.MsgServer.AttackHandler.Calculate.Base.CalculateSoul(AnimationObj.Damage, 0);

                                InteractQuery Attack = new InteractQuery();
                                Attack.UID = this.UID;
                                Attack.OpponentUID = this.UID;
                                Attack.Damage = (int)AnimationObj.Damage;
                                Attack.Effect = AnimationObj.Effect;
                                Attack.X = this.X;
                                Attack.Y = this.Y;
                                Attack.AtkType = MsgAttackPacket.AttackID.Physical;

                                stream.InteractionCreate(&Attack);


                                monster.View.SendScreen(stream, this.OwnerFloor.Map);

                            }

                        }
                        GMap.View.LeaveMap<Role.IMapObj>(this);
                    }
                    HitPoints = 0;
                    GameMap.SetMonsterOnTile(X, Y, false);
                    return;
                }


                RespawnStamp = Extensions.Time32.Now.AddSeconds(8 + Family.RespawnTime);

                if (BlackSpot)
                {
                    Send(stream.BlackspotCreate(false, UID));
                    BlackSpot = false;
                }

                ClearFlags(false);
                HitPoints = 0;
                AddFlag(MsgServer.MsgUpdate.Flags.Dead, Role.StatusFlagsBigVector32.PermanentFlag, true);
                DeadStamp = Extensions.Time32.Now;

                InteractQuery action = new InteractQuery()
                {
                    UID = aUID,
                    KilledMonster = true,
                    X = this.X,
                    Y = this.Y,
                    AtkType = MsgAttackPacket.AttackID.Death,
                    OpponentUID = UID
                };

                if (killer != null)
                {
                    //

                    //MoonBoxQuest
                    if (killer.Player.Map == 1047)
                    {
                        if (killer.Inventory.Contain(721014, 1))
                        {
                            killer.Player.MessageBox("You~received~a~Disturbed~Token!~Head~to~see~the~Ghost~for~more~details!", p => p.Send(stream.PathFinding(p.Player.UID, 1044, 12, 63, 600007)), null);
                        }
                        else
                        {
                            double chance = killer.Player.KillCounter / 100;
                            if (MyMath.Success(chance))
                            {
                                if (killer.Player.VipLevel >= 6)
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        killer.Inventory.Add(stream, 721014);
                                    }
                                    else
                                    {
                                        killer.Inventory.AddReturnedItem(stream, 721014);
                                    }
                                    killer.Player.MessageBox("You~received~a~Disturbed~Token!", null, null);
                                }
                                else
                                {
                                    DropItemID(killer, 721014, stream, 3, 59);
                                }
                            }
                        }
                    }
                    if (killer.Player.Map == 1045)
                    {
                        if (killer.Inventory.Contain(721012, 1))
                        {
                            killer.Player.MessageBox("You~received~a~Deserted~Token!~Head~to~see~the~Ghost~for~more~details!", p => p.Send(stream.PathFinding(p.Player.UID, 1045, 12, 63, 600009)), null);
                        }
                        else
                        {
                            double chance = killer.Player.KillCounter / 100;
                            if (MyMath.Success(chance))
                            {
                                if (killer.Player.VipLevel >= 6)
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        killer.Inventory.Add(stream, 721012);
                                    }
                                    else
                                    {
                                        killer.Inventory.AddReturnedItem(stream, 721012);
                                    }
                                    killer.Player.MessageBox("You~received~a~Deserted~Token!", null, null);
                                }
                                else
                                {
                                    DropItemID(killer, 721012, stream, 3, 59);
                                }
                            }
                        }
                    }
                    if (killer.Player.Map == 1043)
                    {
                        if (killer.Inventory.Contain(721010, 1))
                        {
                            killer.Player.MessageBox("You~received~a~Peace~Token!~Head~to~see~the~Ghost~for~more~details!", p => p.Send(stream.PathFinding(p.Player.UID, 1043, 12, 63, 600005)), null);
                        }
                        else
                        {
                            double chance = killer.Player.KillCounter / 100;
                            if (MyMath.Success(chance))
                            {
                                if (killer.Player.VipLevel >= 6)
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        killer.Inventory.Add(stream, 721010);
                                    }
                                    else
                                    {
                                        killer.Inventory.AddReturnedItem(stream, 721010);
                                    }
                                    killer.Player.MessageBox("You~received~a~Peace~Token!", null, null);
                                }
                                else
                                {
                                    DropItemID(killer, 721010, stream, 3, 59);
                                }
                            }
                        }
                    }
                    if (killer.Player.Map == 1048)
                    {
                        if (killer.Inventory.Contain(721015, 1))
                        {
                            killer.Player.MessageBox("You~received~a~Peace~Calm!~Head~to~see~the~Ghost~for~more~details!", p => p.Send(stream.PathFinding(p.Player.UID, 1048, 12, 63, 600015)), null);
                        }
                        else
                        {
                            double chance = killer.Player.KillCounter / 100;
                            if (MyMath.Success(chance))
                            {
                                if (killer.Player.VipLevel >= 6)
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        killer.Inventory.Add(stream, 721015);
                                    }
                                    else
                                    {
                                        killer.Inventory.AddReturnedItem(stream, 721015);
                                    }
                                    killer.Player.MessageBox("You~received~a~Calm~Token!", null, null);
                                }
                                else
                                {
                                    DropItemID(killer, 721015, stream, 3, 59);
                                }
                            }
                        }
                    }
                    if (killer.Player.Map == 1044)
                    {
                        if (killer.Inventory.Contain(721013, 1))
                        {
                            killer.Player.MessageBox("You~received~a~Peace~Chaos!~Head~to~see~the~Ghost~for~more~details!", p => p.Send(stream.PathFinding(p.Player.UID, 1044, 12, 63, 600007)), null);
                        }
                        else
                        {
                            double chance = killer.Player.KillCounter / 100;
                            if (MyMath.Success(chance))
                            {
                                if (killer.Player.VipLevel >= 6)
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        killer.Inventory.Add(stream, 721013);
                                    }
                                    else
                                    {
                                        killer.Inventory.AddReturnedItem(stream, 721013);
                                    }
                                    killer.Player.MessageBox("You~received~a~Chaos~Token!", null, null);
                                }
                                else
                                {
                                    DropItemID(killer, 721013, stream, 3, 59);
                                }
                            }
                        }
                    }
                    if (Family.ID == 0015)
                    {
                        double chance = killer.Player.KillCounter / 100;
                        if (MyMath.Success(chance))
                        {
                            DropItemID(killer, 1080001, stream);
                        }
                    }
                    if (killer.Player.Map == 1046)
                    {
                        if (killer.Inventory.Contain(721013, 1))
                        {
                            killer.Player.MessageBox("You~received~a~Peace~Prosperous!~Head~to~see~the~Ghost~for~more~details!", p => p.Send(stream.PathFinding(p.Player.UID, 1046, 12, 63, 600011)), null);
                        }
                        else
                        {

                            double chance = killer.Player.KillCounter / 100;
                            if (MyMath.Success(chance))
                            {
                                if (killer.Player.VipLevel >= 6)
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        killer.Inventory.Add(stream, 721013);
                                    }
                                    else
                                    {
                                        killer.Inventory.AddReturnedItem(stream, 721013);
                                    }
                                    killer.Player.MessageBox("You~received~a~Prosperous~Token!", null, null);
                                }
                                else
                                {
                                    DropItemID(killer, 721013, stream, 3, 59);
                                }
                            }
                        }
                    }

                    if (killer.Player.Map == 44461)
                    {
                        if (Family.ID == 452373)
                        {
                            killer.CreateBoxDialog("Duke of Hell has been enraged and revealed his true appearance. Hurry and defeat him to prevent the imperial mausoleum from destruction.");
                            killer.Teleport(killer.Player.X, killer.Player.Y, 44462, Database.Server.ServerMaps[44462].GenerateDynamicID());
                            Database.Server.AddMapMonster(stream, killer.Map, 452374, 336, 193, 2, 2, 1, killer.Player.DynamicID, true, MsgFloorItem.MsgItemPacket.EffectMonsters.None, "moveback");
                        }
                    }
                    else if (killer.Player.Map == 44462)
                    {
                        killer.Player.MessageBox("You`ve finally knocked back the Duke of Hell. However, the fierce fights have also triggered the emergency switch and the imperial mausoleum will soon collapse. Run!"
                            , new Action<Client.GameClient>(p => p.Player.QuestGUI.SendAutoPatcher(10090, 58, 164, 0)), null);

                        killer.Teleport(killer.Player.X, killer.Player.Y, 44463, Database.Server.ServerMaps[44463].GenerateDynamicID());
                    }
                    else if (killer.Player.Map == 44456)
                    {
                        if (Family.ID == 657218)
                        {
                            Database.Server.AddMapMonster(stream, killer.Map, 657219, killer.Player.X, killer.Player.Y, 6, 6, 1, killer.Player.DynamicID, true, MsgFloorItem.MsgItemPacket.EffectMonsters.None, "movego");
                        }
                        if (killer.Player.View.Roles(Role.MapObjectType.Monster).Count() <= 1)
                        {
                            killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 3799, 1);
                            killer.Player.QuestGUI.SendAutoPatcher("You`ve successfully driven off the pursuing devils. Hurry and report back to the Elder in the hall.", 10088, 123, 119, 200058);

                        }
                    }
                    else if (killer.Player.Map == 1791)
                    {
                        if (Family.ID == 2172)
                        {
                            Database.Server.AddMapMonster(stream, killer.Map, 2173, killer.Player.X, killer.Player.Y, 7, 7, 1, killer.Player.DynamicID, true, MsgFloorItem.MsgItemPacket.EffectMonsters.None, "movego");
                            Database.Server.AddMapMonster(stream, killer.Map, 2173, killer.Player.X, killer.Player.Y, 6, 6, 1, killer.Player.DynamicID, true, MsgFloorItem.MsgItemPacket.EffectMonsters.None, "movego");
                            Database.Server.AddMapMonster(stream, killer.Map, 2173, killer.Player.X, killer.Player.Y, 5, 5, 1, killer.Player.DynamicID, true, MsgFloorItem.MsgItemPacket.EffectMonsters.None, "movego");
                            Database.Server.AddMapMonster(stream, killer.Map, 2173, killer.Player.X, killer.Player.Y, 4, 4, 1, killer.Player.DynamicID, true, MsgFloorItem.MsgItemPacket.EffectMonsters.None, "movego");
                            Database.Server.AddMapMonster(stream, killer.Map, 2173, killer.Player.X, killer.Player.Y, 3, 3, 1, killer.Player.DynamicID, true, MsgFloorItem.MsgItemPacket.EffectMonsters.None, "movego");
                            killer.SendSysMesage("Some Lyra Ghosts are coming! Watch out!");
                        }
                    }
                    else if (killer.Player.Map == 1792)
                    {
                        if (Family.ID == 2170)
                        {
                            if (killer.Player.QuestGUI.CheckQuest(407, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (killer.Inventory.HaveSpace(1))
                                {
                                    killer.Inventory.Add(stream, 710216, 1);
                                    killer.SendSysMesage("You`ve killed the Crimson Viper and obtained a Purple Eye. Now go back to Maple Forest (788,456) to show Villager Chou.");
                                }
                                else
                                {
                                    killer.SendSysMesage("There`s not enough space in your inventory.");
                                }
                            }
                        }
                    }
                    else if (killer.Player.Map == 1794)
                    {
                        if (Family.ID == 2168)//brigard
                        {
                            if (killer.Player.QuestGUI.CheckQuest(404, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (Role.Core.Rate(20))
                                {
                                    if (!killer.Inventory.Contain(721806, 1))
                                    {
                                        if (killer.Inventory.HaveSpace(1))
                                        {
                                            killer.Inventory.Add(stream, 721806, 1);
                                            killer.SendSysMesage("The Brigands dropped a Bee Kettle! Use the kettle to leave here!");
                                        }
                                        else
                                        {
                                            killer.SendSysMesage("There`s not enough space in your inventory for the Bee Kettle!");
                                        }
                                    }
                                }
                            }
                        }
                        else if (Family.ID == 2167)//viper
                        {
                            if (killer.Player.QuestGUI.CheckQuest(404, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (Role.Core.Rate(25))
                                {
                                    if (!killer.Inventory.Contain(721803, 1))
                                    {
                                        if (killer.Inventory.HaveSpace(1))
                                        {
                                            killer.Inventory.Add(stream, 721803, 1);
                                            killer.SendSysMesage("You obtained a Poison Fang and got poisoned.");
                                            killer.SendSysMesage("Some Brigands are coming! Look out!");
                                            Database.Server.AddMapMonster(stream, killer.Map, 2168, killer.Player.X, killer.Player.Y, 7, 7, 1, killer.Player.DynamicID, true, MsgFloorItem.MsgItemPacket.EffectMonsters.None, "movego");
                                            Database.Server.AddMapMonster(stream, killer.Map, 2168, killer.Player.X, killer.Player.Y, 6, 6, 1, killer.Player.DynamicID, true, MsgFloorItem.MsgItemPacket.EffectMonsters.None, "movego");
                                            Database.Server.AddMapMonster(stream, killer.Map, 2168, killer.Player.X, killer.Player.Y, 6, 6, 1, killer.Player.DynamicID, true, MsgFloorItem.MsgItemPacket.EffectMonsters.None, "movego");
                                            Database.Server.AddMapMonster(stream, killer.Map, 2168, killer.Player.X, killer.Player.Y, 5, 5, 1, killer.Player.DynamicID, true, MsgFloorItem.MsgItemPacket.EffectMonsters.None, "movego");

                                        }
                                    }
                                }
                            }

                        }
                    }
                    else if (killer.Player.Map == 1787)
                    {
                        if (Family.ID == 2160)
                        {
                            if (killer.Player.QuestGUI.CheckQuest(527, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (!killer.Inventory.Contain(721777, 3))
                                {
                                    if (killer.Inventory.HaveSpace(3))
                                    {
                                        killer.Inventory.Add(stream, 721777, 3);
                                        killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 527, 0, 0, 0, 0, 1);
                                        killer.CreateBoxDialog("Nothing~but~these~three~Tower~Splinters~will~do.~Take~them.~Maybe~you`ll~get~something~from~them~at~the~Cauldron~(48,37).");
                                    }
                                    else
                                    {
                                        killer.CreateBoxDialog("Your~inventory~is~full.~Make~some~room~first,~and~come~back~for~the~Tower~Splinter.");
                                    }
                                }
                            }
                        }
                        else if (Family.ID == 2163)
                        {
                            if (killer.Player.QuestGUI.CheckQuest(527, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (!killer.Map.ContainMobID(2160))
                                {
                                    Database.Server.AddMapMonster(stream, killer.Map, 2160, killer.Player.X, killer.Player.Y, 3, 3, 1, killer.Player.DynamicID, true, MsgFloorItem.MsgItemPacket.EffectMonsters.None);
                                }
                                killer.CreateBoxDialog("The~ghost~of~Yang~Feng~has~appeared~at~(86,78).~Hurry~up~and~see~what`s~going~on~there!");
                            }
                        }
                    }
                    else if (killer.Player.Map == 1786)
                    {
                        if (Family.ID == 2159)
                        {
                            if (killer.Player.QuestGUI.CheckQuest(527, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (Role.Core.Rate(30))
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        if (!killer.Inventory.Contain(721789, 1))
                                        {
                                            killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 527, 0, 0, 1);
                                            killer.Inventory.Add(stream, 721789, 1);
                                            killer.CreateBoxDialog("You~received~the~Wasp~Spear.");
                                        }
                                    }
                                    else
                                    {
                                        killer.CreateBoxDialog("Your~inventory~is~full!");
                                    }
                                }
                            }
                        }
                        else if (Family.ID == 2178)
                        {
                            if (killer.Player.QuestGUI.CheckQuest(527, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (killer.Inventory.HaveSpace(1))
                                {
                                    if (!killer.Inventory.Contain(721790, 1))
                                    {

                                        killer.Inventory.Add(stream, 721790, 1);
                                        killer.CreateBoxDialog("You~received~the~Infernal~Axe.");
                                        killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 527, 0, 0, 0, 1);
                                    }
                                }
                                else
                                {
                                    killer.CreateBoxDialog("Your~inventory~is~full!");
                                }
                            }
                        }
                    }
                    if (killer.Player.Map == 1785)
                    {
                        if (Family.ID == 2166)
                        {
                            if (killer.Player.QuestGUI.CheckQuest(527, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (killer.Inventory.HaveSpace(1))
                                {
                                    if (!killer.Inventory.Contain(721788, 1))
                                    {
                                        killer.Inventory.Add(stream, 721788, 1);
                                        killer.CreateBoxDialog("You~obtained~the~Annatto~Blade.");
                                        killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 527, 0, 1);
                                    }
                                }
                                else
                                {
                                    killer.CreateBoxDialog("Your~inventory~is~full!");
                                }
                            }
                        }
                        else if (Family.ID == 2161)
                        {
                            if (killer.Player.QuestGUI.CheckQuest(527, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (Role.Core.Rate(30))
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        if (!killer.Inventory.Contain(721784, 1))
                                        {
                                            killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 527, 1);
                                            killer.Inventory.Add(stream, 721784, 1);
                                            killer.CreateBoxDialog("You~obtained~the~Glitter~Sword.");
                                        }
                                    }
                                    else
                                    {
                                        killer.CreateBoxDialog("Your~inventory~is~full!");
                                    }
                                }
                            }
                        }
                    }
                    else if (killer.Player.Map == 1783)
                    {
                        if (Family.ID == 2158)
                        {
                            if (killer.Player.QuestGUI.CheckQuest(526, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (killer.Player.QuestGUI.CheckObjectives(526, 20))
                                {
                                    killer.CreateBoxDialog("What~a~fight!~Congratulations,~you~can~make~your~escape~through~the~rift~now.");
                                }
                                else
                                {
                                    killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 526, 1);
                                    if (killer.Player.QuestGUI.CheckObjectives(526, 20))
                                    {
                                        killer.CreateBoxDialog("What~a~fight!~Congratulations,~you~can~make~your~escape~through~the~rift~now.");
                                    }
                                }
                            }

                        }
                    }
                    else if (killer.Player.Map == 1109)
                    {
                        if (Family.ID == 7696)
                        {
                            if (killer.Inventory.HaveSpace(1))
                            {
                                killer.Inventory.Add(stream, 3006392, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false);
                                killer.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e280");
                                killer.CreateBoxDialog("You~successfully~defeated~the~Heaven~Roc,~and~received~Roc`s~Feather!~It`s~amazing!");
                                killer.Teleport(213, 200, 1011);
                            }
                            else
                            {
                                killer.SendSysMesage("Your~inventory~is~full.~Please~make~some~room~first,~and~then~come~back~for~the~challenge.");
                                killer.Teleport(213, 200, 1011);
                            }
                        }
                    }
                    else if (killer.Player.Map == 1351)
                    {
                        if (Family.ID == 3141)//slinger
                        {
                            if (killer.Player.QuestGUI.CheckQuest(1841, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1841, 1);
                                if (killer.Player.QuestGUI.CheckObjectives(1841, 20))
                                {
                                    killer.Player.QuestGUI.FinishQuest(1841);
                                    killer.Player.Money += 100;
                                    killer.Player.SendUpdate(stream, killer.Player.Money, MsgUpdate.DataType.Money);
                                    killer.SendSysMesage("You received 100 Silver!");
                                    killer.CreateBoxDialog("You`ve~finished~the~first~step.~Now~you~may~commence~the~second~step.");
                                    killer.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "accession");
                                    var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.OldBeggar, killer.Player.Class, 1842);
                                    killer.Player.QuestGUI.Accept(ActiveQuest, 0);
                                }
                            }
                            else if (killer.Player.QuestGUI.CheckQuest(1843, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1843, 1);
                                if (killer.Player.QuestGUI.CheckObjectives(1843, 15, 15))
                                {
                                    killer.Player.QuestGUI.FinishQuest(1843);
                                    killer.Player.Money += 100;
                                    killer.Player.SendUpdate(stream, killer.Player.Money, MsgUpdate.DataType.Money);
                                    killer.SendSysMesage("You received 100 Silver!");
                                    killer.CreateBoxDialog("You`ve~accomplished~the~third~step.~Commence~the~fourth~step~now.~Keep~up~with~the~good~work.");
                                    killer.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "accession");
                                    var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.OldBeggar, killer.Player.Class, 1844);
                                    killer.Player.QuestGUI.Accept(ActiveQuest, 0);
                                }
                            }
                        }
                        else if (Family.ID == 3142)//goldghost
                        {
                            if (killer.Player.QuestGUI.CheckQuest(1843, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1843, 0, 1);
                                if (killer.Player.QuestGUI.CheckObjectives(1843, 15, 15))
                                {
                                    killer.Player.QuestGUI.FinishQuest(1843);
                                    killer.Player.Money += 100;
                                    killer.Player.SendUpdate(stream, killer.Player.Money, MsgUpdate.DataType.Money);
                                    killer.SendSysMesage("You received 100 Silver!");
                                    killer.CreateBoxDialog("You`ve~accomplished~the~third~step.~Commence~the~fourth~step~now.~Keep~up~with~the~good~work.");
                                    killer.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "accession");
                                    var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.OldBeggar, killer.Player.Class, 1844);
                                    killer.Player.QuestGUI.Accept(ActiveQuest, 0);
                                }
                            }
                            else if (killer.Player.QuestGUI.CheckQuest(1842, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1842, 1);
                                if (killer.Player.QuestGUI.CheckObjectives(1842, 10))
                                {
                                    killer.Player.QuestGUI.FinishQuest(1842);
                                    killer.Player.Money += 100;
                                    killer.Player.SendUpdate(stream, killer.Player.Money, MsgUpdate.DataType.Money);
                                    killer.SendSysMesage("You received 100 Silver!");
                                    killer.CreateBoxDialog("You`ve~killed~10~Gold~Ghosts.~Great!");
                                    killer.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "accession");

                                    var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.OldBeggar, killer.Player.Class, 1843);
                                    killer.Player.QuestGUI.Accept(ActiveQuest, 0);
                                }
                            }
                        }
                    }
                    else if (killer.Player.Map == 1352)//laibirint2
                    {
                        if (Family.ID == 3144)//blading
                        {
                            if (killer.Player.QuestGUI.CheckQuest(1844, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1844, 1);
                                if (killer.Player.QuestGUI.CheckObjectives(1844, 2))
                                {
                                    killer.Player.QuestGUI.FinishQuest(1844);
                                    killer.Player.Money += 100;
                                    killer.Player.SendUpdate(stream, killer.Player.Money, MsgUpdate.DataType.Money);
                                    killer.SendSysMesage("You received 100 Silver!");
                                    killer.CreateBoxDialog("You`ve~finished~the~fourth~step.~Now~commence~the~fifth~step.");
                                    killer.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "accession");
                                    var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.OldBeggar, killer.Player.Class, 1845);
                                    killer.Player.QuestGUI.Accept(ActiveQuest, 0);

                                }
                            }
                            else if (killer.Player.QuestGUI.CheckQuest(1846, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1846, 1);
                                if (killer.Player.QuestGUI.CheckObjectives(1846, 10, 10))
                                {
                                    killer.Player.QuestGUI.FinishQuest(1846);
                                    killer.Player.Money += 100;
                                    killer.Player.SendUpdate(stream, killer.Player.Money, MsgUpdate.DataType.Money);
                                    killer.SendSysMesage("You received 100 Silver!");
                                    killer.CreateBoxDialog("You`ve~finished~the~6th~step.~Now~commence~the~7th~step.");
                                    killer.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "accession");
                                    var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.OldBeggar, killer.Player.Class, 1847);
                                    killer.Player.QuestGUI.Accept(ActiveQuest, 0);
                                }
                            }
                        }
                        else if (Family.ID == 3145)//agilerat
                        {
                            if (killer.Player.QuestGUI.CheckQuest(1845, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (killer.Inventory.Contain(721926, 1))
                                {
                                    killer.CreateBoxDialog("You`ve~received~an~Agile~Rat~Fang~already.~You~cannot~take~any~more.");
                                }
                                else
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        killer.Inventory.Add(stream, 721926, 1);
                                        killer.CreateBoxDialog("You~received~an~Agile~Rat~Fang!");
                                    }
                                    else
                                    {
                                        killer.CreateBoxDialog("Your~inventory~is~full.");
                                    }
                                }
                            }
                            else if (killer.Player.QuestGUI.CheckQuest(1846, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1846, 0, 1);
                                if (killer.Player.QuestGUI.CheckObjectives(1846, 10, 10))
                                {
                                    killer.Player.QuestGUI.FinishQuest(1846);
                                    killer.Player.Money += 100;
                                    killer.Player.SendUpdate(stream, killer.Player.Money, MsgUpdate.DataType.Money);
                                    killer.SendSysMesage("You received 100 Silver!");
                                    killer.CreateBoxDialog("You`ve~finished~the~6th~step.~Now~commence~the~7th~step.");
                                    killer.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "accession");
                                    var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.OldBeggar, killer.Player.Class, 1847);
                                    killer.Player.QuestGUI.Accept(ActiveQuest, 0);
                                }
                            }
                        }
                    }
                    else if (killer.Player.Map == 1353)
                    {
                        if (Family.ID == 3147)//bluebird
                        {
                            if (killer.Player.QuestGUI.CheckQuest(1847, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1847, 1);
                                killer.CreateBoxDialog("You`ve~killed~a~Blue~Bird.~Kill~another~one~in~20~seconds.");
                                if (killer.Player.QuestGUI.CheckObjectives(1847, 2))
                                {
                                    killer.Player.QuestGUI.FinishQuest(1847);
                                    killer.Player.Money += 100;
                                    killer.Player.SendUpdate(stream, killer.Player.Money, MsgUpdate.DataType.Money);
                                    killer.SendSysMesage("You received 100 Silver!");

                                    killer.CreateBoxDialog("You`ve~finished~the~7th~step.~Now~commence~the~8th~step.");
                                    killer.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "accession");

                                    var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.OldBeggar, killer.Player.Class, 1848);
                                    killer.Player.QuestGUI.Accept(ActiveQuest, 0);
                                }
                            }
                            else if (killer.Player.QuestGUI.CheckQuest(1849, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1849, 1);
                                if (killer.Player.QuestGUI.CheckObjectives(1849, 5, 5))
                                {
                                    killer.Player.QuestGUI.FinishQuest(1849);
                                    killer.Player.Money += 100;
                                    killer.Player.SendUpdate(stream, killer.Player.Money, MsgUpdate.DataType.Money);
                                    killer.SendSysMesage("You received 100 Silver!");

                                    killer.CreateBoxDialog("You`ve~finished~the~9th~step.~Now~commence~the~10th~step.");
                                    killer.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "accession");

                                    var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.OldBeggar, killer.Player.Class, 1850);
                                    killer.Player.QuestGUI.Accept(ActiveQuest, 0);
                                }
                            }
                            else if (killer.Player.QuestGUI.CheckQuest(1850, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (killer.Inventory.Contain(721931, 1))
                                {
                                    killer.CreateBoxDialog("You`ve~received~a~Blue~Bird~Plume~already.~You~cannot~take~any~more.");
                                }
                                else
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        killer.Inventory.Add(stream, 721931);
                                        killer.CreateBoxDialog("You~received~a~Blue~Bird~Plume.");
                                    }
                                    else
                                    {
                                        killer.CreateBoxDialog("Your~inventory~is~full.");
                                    }
                                }
                            }
                        }
                        else if (Family.ID == 3148)//fiendbat
                        {
                            if (killer.Player.QuestGUI.CheckQuest(1848, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (Role.Core.Rate(50))
                                {
                                    if (killer.Inventory.Contain(721928, 1))
                                    {
                                        killer.CreateBoxDialog("You`ve~received~a~Fiend~Bat~Wing.~You~cannot~take~any~more.");
                                    }
                                    else
                                    {
                                        if (killer.Inventory.Contain(721929, 1))
                                        {
                                            killer.CreateBoxDialog("You`ve~received~the~Roast~Fiend~Bat~Wing.~Eat~it~now!");
                                        }
                                        else
                                        {
                                            if (killer.Inventory.HaveSpace(1))
                                            {
                                                killer.Inventory.Add(stream, 721928);
                                                killer.CreateBoxDialog("You~received~a~Fiend~Bat~Wing.");
                                            }
                                            else
                                            {
                                                killer.CreateBoxDialog("Your~inventory~is~full.");
                                            }
                                        }
                                    }
                                }
                            }
                            else if (killer.Player.QuestGUI.CheckQuest(1849, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1849, 0, 1);
                                if (killer.Player.QuestGUI.CheckObjectives(1849, 5, 5))
                                {
                                    killer.Player.QuestGUI.FinishQuest(1849);
                                    killer.Player.Money += 100;
                                    killer.Player.SendUpdate(stream, killer.Player.Money, MsgUpdate.DataType.Money);
                                    killer.SendSysMesage("You received 100 Silver!");

                                    killer.CreateBoxDialog("You`ve~finished~the~9th~step.~Now~commence~the~10th~step.");
                                    killer.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "accession");

                                    var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.OldBeggar, killer.Player.Class, 1850);
                                    killer.Player.QuestGUI.Accept(ActiveQuest, 0);
                                }
                            }
                            else if (killer.Player.QuestGUI.CheckQuest(1850, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (killer.Inventory.Contain(721931, 1))
                                {
                                    killer.CreateBoxDialog("You`ve~received~a~Blue~Bird~Plume~already.~You~cannot~take~any~more.");
                                }
                                else
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        killer.Inventory.Add(stream, 721931);
                                        killer.CreateBoxDialog("You~received~a~Blue~Bird~Plume.");
                                    }
                                    else
                                    {
                                        killer.CreateBoxDialog("Your~inventory~is~full.");
                                    }
                                }
                            }
                        }
                    }
                    else if (killer.Player.Map == 1354)
                    {
                        if (Family.ID == 3155)//minotaurlevel120
                        {
                            if (killer.Player.QuestGUI.CheckQuest(1850, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.Player.OnBluedBird = true;
                                killer.CreateBoxDialog("Use~the~Blue~Bird~Wing~in~3~seconds~to~ignite~it,~or~it~will~never~be~ignited.");

                                killer.Player.BlueBirdPlumeStamp = DateTime.Now.AddSeconds(3);
                            }
                        }
                    }
                    else if (killer.Player.Map == 1001)
                    {
                        if (Family.ID == 2232)//bullmaster
                        {
                            if (killer.Player.QuestGUI.CheckQuest(1838, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (killer.Inventory.Contain(710260, 1))
                                {
                                    killer.CreateBoxDialog("You~received~the~Bull~heart!");
                                }
                                else
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        killer.Inventory.Add(stream, 710260, 1);
                                        killer.CreateBoxDialog("You~killed~the~Bull~Master~and~received~its~heart.");
                                    }
                                    else
                                    {
                                        killer.CreateBoxDialog("Your~inventory~is~full.~You`ll~need~to~make~some~room,~first.");
                                    }
                                }
                            }
                        }
                        if (Family.ID == 2231)//madbull
                        {
                            if (killer.Player.QuestGUI.CheckQuest(1837, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (Role.Core.GetDistance(killer.Player.X, killer.Player.Y, 102, 292) <= 3)
                                {

                                    killer.Player.AddMapEffect(stream, 102, 292, "zf2-e027");
                                    if (killer.Inventory.Contain(710261, 3))
                                    {
                                        killer.SendSysMesage("You`ve collected 3 Bull Ghost.");
                                    }
                                    else
                                    {
                                        if (killer.Inventory.HaveSpace(1))
                                        {
                                            if (Role.Core.Rate(1, 3))
                                            {
                                                killer.Inventory.Add(stream, 710261, 1);
                                                killer.SendSysMesage("You obtained a Bull Ghost.");
                                                if (killer.Inventory.Contain(710261, 3))
                                                {
                                                    killer.SendSysMesage("You`ve collected 3 Bull Ghost.");
                                                }
                                            }
                                            else
                                            {
                                                killer.SendSysMesage("Well, pay more attention next time. You failed in retrieving the Bull Ghost.");
                                            }
                                        }
                                        else
                                        {
                                            killer.SendSysMesage("You inventory is full. Make some room and come back for the Bull Ghost.");
                                        }
                                    }
                                }
                                else
                                {
                                    killer.SendSysMesage("Only when you herd the Mad Bull to (102,292) before killing them can you capture its ghost.");
                                }
                            }
                        }
                        if (Family.ID == 57)//bullmonster
                        {
                            if (killer.Player.QuestGUI.CheckQuest(1836, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (killer.Player.OnFerentPill)
                                {
                                    if (killer.Inventory.Contain(710259, 10))
                                    {
                                        killer.SendSysMesage("You`ve obtained 10 Burnt Bull Horns and put it in your inventory.");
                                    }
                                    else
                                    {
                                        if (killer.Inventory.HaveSpace(1))
                                        {
                                            killer.Inventory.AddItemWitchStack(710259, 0, 1, stream);
                                            if (killer.Inventory.Contain(710259, 10))
                                            {
                                                killer.SendSysMesage("You`ve obtained 10 Burnt Bull Horns and put it in your inventory.");
                                            }
                                            else
                                            {
                                                killer.SendSysMesage("You`ve obtained a Burnt Bull Horn.");
                                            }
                                        }
                                        else
                                        {
                                            killer.SendSysMesage("Your inventory is full. Make some room and come back for the Burnt Bull Horn.");
                                        }
                                    }
                                }
                                else
                                    killer.SendSysMesage("Bring the Fervent Pill received from Master Mo Mo to go to the Mystic Castle. Use the pill first, and then kill the Bull Monsters to get 10 Burnt Bull Horns for Master Mo Mo.");
                            }
                        }
                        if (Family.ID == 2189)
                        {
                            if (!killer.Inventory.Contain(711518, 1))
                            {
                                if (killer.Inventory.HaveSpace(1))
                                {
                                    killer.Inventory.Add(stream, 711518);
                                    killer.CreateBoxDialog("You~killed~the~Jinx~Tomb~Bat~and~received~a~Jinx~Tomb~Bat~Heart!");
                                }
                                else
                                {
                                    killer.CreateBoxDialog("Your~inventory~is~full.~Hurry~to~make~1~empty~space.");
                                }
                            }

                        }
                        if (Family.ID == 56)
                        {
                            if (killer.Player.QuestGUI.CheckQuest(1832, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (killer.Inventory.HaveSpace(1))
                                {
                                    if (Role.Core.Rate(20))
                                    {
                                        if (!killer.Inventory.Contain(721877, 5))
                                        {
                                            killer.Inventory.AddItemWitchStack(721877, 0, 1, stream);
                                            killer.SendSysMesage("You found the BatEssence from the Tomb Bats.");
                                            killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1832, 1);
                                        }
                                    }
                                }
                                else
                                    killer.CreateBoxDialog("You~inventory~is~full.~You`ll~need~to~make~some~room,~first.");
                            }
                        }
                        if (Family.ID == 20)
                        {
                            if (killer.Player.QuestGUI.CheckQuest(1829, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (killer.Inventory.HaveSpace(1))
                                {
                                    if (Role.Core.Rate(20))
                                    {
                                        if (!killer.Inventory.Contain(721870, 1))
                                        {
                                            if (killer.Inventory.Contain(721876, 5))
                                            {
                                                killer.CreateBoxDialog("Sorry,~but~you`ve~already~had~5~Keys.");
                                            }
                                            else
                                            {
                                                killer.Inventory.AddItemWitchStack(721876, 0, 1, stream);
                                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1829, 1);
                                                killer.CreateBoxDialog("You~received~a~Key~from~the~Tomb~Bat.");
                                            }
                                        }
                                    }
                                }
                                else
                                    killer.CreateBoxDialog("You~inventory~is~full.~You`ll~need~to~make~some~room,~first.");
                            }

                        }
                    }
                    else if (killer.Player.Map == 3833)
                    {
                        if (Family.ID == 7485)
                        {
                            killer.Player.StageEpicTrojanQuest = 51;
                            if (!killer.Map.ContainMobID(7486))
                            {
                                Database.Server.AddMapMonster(stream, killer.Map, 7486, killer.Player.X, killer.Player.Y, 3, 3, 1, killer.Player.DynamicID, true, MsgFloorItem.MsgItemPacket.EffectMonsters.None, "ride_screen");
                            }

                            killer.SendSysMesage("The~earth~is~shaking,~and~the~Flame~Devastator~revived!");
                        }
                        else if (Family.ID == 7486)
                        {
                            killer.Player.StageEpicTrojanQuest = 52;
                            killer.Player.AddMapEffect(stream, 45, 45, "xidag_bafi");
                            if (!killer.Map.ContainMobID(7487))
                            {
                                Database.Server.AddMapMonster(stream, killer.Map, 7487, killer.Player.X, killer.Player.Y, 3, 3, 1, killer.Player.DynamicID, true, MsgFloorItem.MsgItemPacket.EffectMonsters.None, "xidag_bafi");
                            }
                        }
                        else if (Family.ID == 7487)
                        {

                            killer.CreateBoxDialog("Holding~the~Epic~Weapon~infused~with~the~blade~spirit,~you~beheaded~the~Flame~Devastator~and~burned~it~into~ashes.");
                            killer.Teleport(154, 132, 3832);
                            killer.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "role-select1");
                            bool finish = false;
                            foreach (var item in killer.Inventory.ClientItems.Values)
                            {
                                if (item.IsWeapon && !Database.ItemType.IsTwoHand(item.ITEM_ID) && !Database.ItemType.IsTrojanEpicWeapon(item.ITEM_ID))
                                {
                                    uint UpdateToEpic = (item.ITEM_ID % 1000) + 614000;
                                    item.ITEM_ID = UpdateToEpic;
                                    item.Mode = Role.Flags.ItemMode.Update;
                                    item.Send(killer, stream);
                                    finish = true;
                                    break;
                                }
                            }
                            if (finish == false)
                            {
                                foreach (var item in killer.Equipment.ClientItems.Values)
                                {
                                    if (item.IsWeapon && !Database.ItemType.IsTwoHand(item.ITEM_ID) && !Database.ItemType.IsTrojanEpicWeapon(item.ITEM_ID))
                                    {
                                        uint UpdateToEpic = (item.ITEM_ID % 1000) + 614000;
                                        item.ITEM_ID = UpdateToEpic;
                                        item.Mode = Role.Flags.ItemMode.Update;
                                        item.Send(killer, stream);
                                        finish = true;
                                        break;
                                    }
                                }
                            }
                            if (finish)
                            {
                                killer.Player.ResetEpicTrojan();


                                Program.SendGlobalPackets.Enqueue(new MsgMessage("" + killer.Player.Name + "~successfully~prevented~Twin~City~from~an~olden~massacre,~and~obtained~an~Epic~Weapon!", MsgMessage.MsgColor.red, MsgMessage.ChatMode.System).GetArray(stream));
                                killer.Inventory.Remove(3003340, 1, stream);
                            }
                        }
                    }
                    if (killer.Player.Map == 3831)
                    {
                        if (Family.ID == 7484)
                        {
                            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.MonkMisery, 0, 3272);
                            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.GeneralPakMap2, 0, 3273);
                            killer.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId);
                            killer.Player.StageEpicTrojanQuest = 40;
                            killer.Player.QuestGUI.Accept(ActiveQuest2, 0);
                            killer.Player.QuestGUI.SendAutoPatcher("The~Evil~Monk~Misery`s~death~laugh~sounds~weird.~Something~may~happen~to~the~Flame~Devastator.~Go~report~to~General~Pak!", 3831, 154, 130, (uint)NpcID.GeneralPakMap2);
                        }
                    }
                    else if (killer.Player.Map == 3834)
                    {
                        if (Family.ID == 7483)
                        {


                            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.GeneralPak, 0, 3271);
                            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.GeneralPak, 0, 3277);
                            killer.Player.StageEpicTrojanQuest = 30;
                            killer.Player.EpicTrijanKillGhostReaver = 1;
                            killer.Player.EpicTrojanMrMirrorPrograss += 1;



                            MsgServer.MsgGameItem item = new MsgServer.MsgGameItem();
                            item.Color = (Role.Flags.Color)2;
                            item.ITEM_ID = 1182;
                            MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(item, 163, 228, MsgFloorItem.MsgItem.ItemType.Effect, 0, 0, killer.Player.Map
                                   , 0, false, killer.Map, 4);

                            if (killer.Map.EnqueueItem(DropItem))
                                DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Effect);

                            if (killer.Player.EpicTrojanMrMirrorPrograss >= 18)
                            {
                                killer.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId);
                                killer.Player.QuestGUI.Accept(ActiveQuest2, 0);
                                killer.Player.MessageBox("All~the~18~Ghost~Reavers~in~the~Flame~Devastator`s~army~have~been~eliminated.~Report~back~to~General~Pak!", new Action<Client.GameClient>(p =>
                                {
                                    killer.Teleport(162, 218, 3830);
                                    killer.Player.QuestGUI.SendAutoPatcher(3830, 154, 130, 10581);

                                }), null);
                            }
                            else
                            {
                                killer.Player.MessageBox("A~Ghost~Reaver~fell~down~and~vanished.~So~far,~you~still~have~" + (18 - killer.Player.EpicTrojanMrMirrorPrograss) + "~Ghost~Reaver(s)~to~deal~with.", new Action<Client.GameClient>(p =>
                                {

                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var pstream = rec.GetStream();
                                        killer.ActiveNpc = (uint)Game.MsgNpc.NpcID.MrMirror2;
                                        Game.MsgNpc.NpcHandler.MrMirror2(killer, pstream, 0, "", 0);
                                    }
                                }), null);
                            }
                        }
                    }
                    #region DemonBox
                    else if (killer.MyHouse != null && killer.Player.DynamicID == killer.Player.UID)
                    {
                        if (Family.ID == 2435)
                        {
                            if (Core.Rate(1, 10000))
                            {
                                DropItemID(killer, 720679, stream, 3);
                                killer.CreateBoxDialog("You killed a Heaven Demon and found a Frost CP Pack (276,000CPs)!");
                                Program.SendGlobalPackets.Enqueue(new MsgMessage(killer.Player.Name + " found a Frost CP Pack (276,000CPs)!", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center).GetArray(stream));
                            }
                            else if (Core.Rate(10, 9999))
                            {
                                DropItemID(killer, 720678, stream, 3);
                                killer.CreateBoxDialog("You killed a Heaven Demon and found a Life CP Pack (54,000CPs)!");
                                Program.SendGlobalPackets.Enqueue(new MsgMessage(killer.Player.Name + " found a Life CP Pack (54,000CPs)!", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center).GetArray(stream));
                            }
                            else if (Core.Rate(3700, 9989))
                            {
                                DropItemID(killer, 720677, stream, 3);
                                killer.CreateBoxDialog("You killed a Heaven Demon and found a Blood CP Pack (20,000CPs)!");
                            }
                            else if (Core.Rate(1289, 6289))
                            {
                                DropItemID(killer, 720676, stream, 3);
                                killer.CreateBoxDialog("You killed a Heaven Demon and found a Soul CP Pack (10,000CPs)!");
                            }
                            else if (Core.Rate(1000, 5000))
                            {
                                DropItemID(killer, 720675, stream, 3);
                                killer.CreateBoxDialog("You killed a Heaven Demon and found a Ghost CP Pack (1,000CPs)!");
                            }
                            else
                            {
                                DropItemID(killer, 720680, stream, 3);
                                killer.CreateBoxDialog("You killed a Heaven Demon and found a Heaven Pill equal to the EXP of 2 and a half EXP Balls!");
                            }
                        }
                        else if (Family.ID == 2436)
                        {
                            if (Core.Rate(1, 10000))
                            {
                                DropItemID(killer, 720685, stream, 3);
                                killer.CreateBoxDialog("You killed a Chaos Demon and found a Nimbus CP Pack (414,000CPs)!");
                                Program.SendGlobalPackets.Enqueue(new MsgMessage(killer.Player.Name + " found a Nimbus CP Pack (414,000CPs)!", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center).GetArray(stream));
                            }
                            else if (Core.Rate(10, 9999))
                            {
                                DropItemID(killer, 720684, stream, 3);
                                killer.CreateBoxDialog("You killed a Chaos Demon and found a Butterfly CP Pack (81,000CPs)!");
                                Program.SendGlobalPackets.Enqueue(new MsgMessage(killer.Player.Name + " found a Butterfly CP Pack (81,000CPs)!", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center).GetArray(stream));
                            }
                            else if (Core.Rate(3700, 9989))
                            {
                                DropItemID(killer, 720683, stream, 3);
                                killer.CreateBoxDialog("You killed a Chaos Demon and found a Heart CP Pack (30,000CPs)!");
                            }
                            else if (Core.Rate(1289, 6289))
                            {
                                DropItemID(killer, 720682, stream, 3);
                                killer.CreateBoxDialog("You killed a Chaos Demon and found a Flower CP Pack (15,000CPs)!");
                            }
                            else if (Core.Rate(1000, 5000))
                            {
                                DropItemID(killer, 720681, stream, 3);
                                killer.CreateBoxDialog("You killed a Chaos Demon and found a Deity CP Pack (1,500CPs)!");
                            }
                            else
                            {
                                DropItemID(killer, 720686, stream, 3);
                                killer.CreateBoxDialog("You killed a Chaos Demon and found a Mystery Pill equal to the EXP of 2 and 1/3 EXP Balls!");
                            }
                        }
                        else if (Family.ID == 2437)
                        {
                            if (Core.Rate(1, 10000))
                            {
                                DropItemID(killer, 720691, stream, 3);
                                killer.CreateBoxDialog("You killed a Sacred Demon and found a Kylin CP Pack (621,000CPs)");
                                Program.SendGlobalPackets.Enqueue(new MsgMessage(killer.Player.Name + " found a Kylin CP Pack (621,000CPs)!", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center).GetArray(stream));
                            }
                            else if (Core.Rate(10, 9999))
                            {
                                DropItemID(killer, 720690, stream, 3);
                                killer.CreateBoxDialog("You killed a Sacred Demon and found a Rainbow CP Pack (121,500CPs)!");
                                Program.SendGlobalPackets.Enqueue(new MsgMessage(killer.Player.Name + " found a Rainbow CP Pack (121,500CPs)!", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center).GetArray(stream));
                            }
                            else if (Core.Rate(3700, 9989))
                            {
                                DropItemID(killer, 720689, stream, 3);
                                killer.CreateBoxDialog("You killed a Sacred Demon and found a Shadow CP Pack (40,000CPs)!");
                            }
                            else if (Core.Rate(1289, 6289))
                            {
                                DropItemID(killer, 720688, stream, 3);
                                killer.CreateBoxDialog("You killed a Sacred Demon and found a Jewel CP Pack (20,000CPs)!");
                            }
                            else if (Core.Rate(1000, 5000))
                            {
                                DropItemID(killer, 720687, stream, 3);
                                killer.CreateBoxDialog("You killed a Sacred Demon and found a Cloud CP Pack (2,000CPs)!");
                            }
                            else
                            {
                                DropItemID(killer, 720692, stream, 3);
                                killer.CreateBoxDialog("You killed a Sacred Demon and found a Wind Pill equal to the EXP of 5 EXP Balls!");
                            }
                        }
                        else if (Family.ID == 2438)
                        {
                            if (Core.Rate(1, 10000))
                            {
                                DropItemID(killer, 720697, stream, 3);
                                killer.CreateBoxDialog("You killed an Aurora Demon and found a Pilgrim CP Pack (931,500CPs)!");
                                Program.SendGlobalPackets.Enqueue(new MsgMessage(killer.Player.Name + " got a Pilgrim CP Pack (931,500CPs)!", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center).GetArray(stream));
                            }
                            else if (Core.Rate(10, 9999))
                            {
                                DropItemID(killer, 720696, stream, 3);
                                killer.CreateBoxDialog("You killed an Aurora Demon and found a Zephyr CP Pack (182,250CPs)!");
                                Program.SendGlobalPackets.Enqueue(new MsgMessage(killer.Player.Name + " found a Zephyr CP Pack (182,250CPs)!", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center).GetArray(stream));
                            }
                            else if (Core.Rate(3700, 9989))
                            {
                                DropItemID(killer, 720695, stream, 3);
                                killer.CreateBoxDialog("You killed an Aurora Demon and found an Earth CP Pack (50,000CPs)!");
                                Program.SendGlobalPackets.Enqueue(new MsgMessage(killer.Player.Name + " found an Earth CP Pack (50,000CPs)!", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center).GetArray(stream));
                            }
                            else if (Core.Rate(1289, 6289))
                            {
                                DropItemID(killer, 720694, stream, 3);
                                killer.CreateBoxDialog("You killed an Aurora Demon and found a Moon CP Pack (25,000CPs)!");
                            }
                            else if (Core.Rate(1000, 5000))
                            {
                                DropItemID(killer, 720693, stream, 3);
                                killer.CreateBoxDialog("You killed an Aurora Demon and found a Fog CP Pack (2,500CPs)!");
                            }
                            else
                            {
                                DropItemID(killer, 720698, stream, 3);
                                killer.CreateBoxDialog("You killed an Aurora Demon and got a Wind Pill equal to the EXP of 8 and 1/3 EXP Balls!");
                            }
                        }
                        else if (Family.ID == 2420)
                        {
                            if (Core.Rate(1, 10000))
                            {
                                DropItemID(killer, 720654, stream, 3);
                                killer.CreateBoxDialog("You killed a Demon and found a Joy CP Pack (13,800CPs)!");
                                Program.SendGlobalPackets.Enqueue(new MsgMessage(killer.Player.Name + " found a Joy CP Pack (13,800CPs)!", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center).GetArray(stream));
                            }
                            else if (Core.Rate(10, 9999))
                            {
                                DropItemID(killer, 720653, stream, 3);
                                killer.CreateBoxDialog("You killed a Demon and found a Dream CP Pack (2,700CPs)!");
                            }
                            else if (Core.Rate(3700, 9989))
                            {
                                DropItemID(killer, 720655, stream, 3);
                                killer.CreateBoxDialog("You killed a Demon and found a Mammon CP Pack (1,000CPs)!");
                            }
                            else if (Core.Rate(1289, 6289))
                            {
                                DropItemID(killer, 720656, stream, 3);
                                killer.CreateBoxDialog("You killed a Demon and found a Mascot CP Pack (500CPs)!");
                            }
                            else if (Core.Rate(1000, 5000))
                            {
                                DropItemID(killer, 720657, stream, 3);
                                killer.CreateBoxDialog("You killed a Demon and found a Hope CP Pack (50CPs)!");
                            }
                            else
                            {
                                DropItemID(killer, 720668, stream, 3);
                                killer.CreateBoxDialog("You killed a Demon and found a Magic Ball equal to the EXP of 1/6 of an EXP Ball!");
                            }
                        }
                        else if (Family.ID == 2421)
                        {
                            if (Core.Rate(1, 10000))
                            {
                                killer.CreateBoxDialog("You killed a Ancient Demon and found a Mystic CP Pack (27,600CPs)!");
                                DropItemID(killer, 720662, stream, 3);
                                Program.SendGlobalPackets.Enqueue(new MsgMessage(killer.Player.Name + " found a Mystic CP Pack (27,600CPs)!", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center).GetArray(stream));
                            }
                            else if (Core.Rate(10, 9999))
                            {
                                DropItemID(killer, 720661, stream, 3);
                                killer.CreateBoxDialog("You killed a Ancient Demon and found a Pure CP Pack (5,400CPs)!");
                            }
                            else if (Core.Rate(3700, 9989))
                            {
                                DropItemID(killer, 720660, stream, 3);
                                killer.CreateBoxDialog("You killed a Ancient Demon and found a Legend CP Pack (2,000CPs)!");
                            }
                            else if (Core.Rate(1289, 6289))
                            {
                                DropItemID(killer, 720659, stream, 3);
                                killer.CreateBoxDialog("You killed a Ancient Demon and found a Sweet CP Pack (1,000CPs)!");
                            }
                            else if (Core.Rate(1000, 5000))
                            {
                                DropItemID(killer, 720658, stream, 3);
                                killer.CreateBoxDialog("You killed a Ancient Demon and found a Festival CP Pack (100CPs)!");
                            }
                            else
                            {
                                DropItemID(killer, 720669, stream, 3);
                                killer.CreateBoxDialog("You killed the Ancient Demon and found a Super Ball equal to the EXP of 5/6 of an EXP Ball!");
                            }
                        }
                        else if (Family.ID == 2422)
                        {
                            if (Core.Rate(1, 10000))
                            {
                                DropItemID(killer, 720667, stream, 3);
                                killer.CreateBoxDialog("You killed a Flood Demon and found a Fantasy CP Pack (138,000CPs)!");
                                Program.SendGlobalPackets.Enqueue(new MsgMessage(killer.Player.Name + " found a Fantasy CP Pack (138,000CPs)!", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center).GetArray(stream));
                            }
                            else if (Core.Rate(10, 9999))
                            {
                                DropItemID(killer, 720666, stream, 3);
                                killer.CreateBoxDialog("You killed a Flood Demon and found a Star CP Pack (27,000CPs)!");
                                Program.SendGlobalPackets.Enqueue(new MsgMessage(killer.Player.Name + " found a Star CP Pack (27,000CPs)!", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center).GetArray(stream));
                            }
                            else if (Core.Rate(3700, 9989))
                            {
                                DropItemID(killer, 720665, stream, 3);
                                killer.CreateBoxDialog("You killed a Flood Demon and found a Cute CP Pack (10,000CPs)!");
                            }
                            else if (Core.Rate(1289, 6289))
                            {
                                DropItemID(killer, 720664, stream, 3);
                                killer.CreateBoxDialog("You killed a Flood Demon and found a Flare CP Pack (5,000CPs)!");
                            }
                            else if (Core.Rate(1000, 5000))
                            {
                                DropItemID(killer, 720663, stream, 3);
                                killer.CreateBoxDialog("You killed a Flood Demon and found a Violet CP Pack (500CPs)!");
                            }
                            else
                            {
                                DropItemID(killer, 720670, stream, 3);
                                killer.CreateBoxDialog("You killed the Flood Demon and found an Ultra Ball equal to EXP worth 1 and 2/3 EXP Balls!");
                            }
                        }
                    }
                    #endregion
                    if (killer.Player.QuestGUI.CheckQuest(2375, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                    {
                        uint spirites = 0;
                        if (Level < 70)
                            spirites = 1;
                        else if (Level >= 70 && Level <= 99)
                            spirites = 2;
                        else if (Level >= 100 && Level <= 119)
                            spirites = 3;
                        else if (Level >= 120 && Level < 140)
                            spirites = 4;
                        else if (Boss == 1 && Family.MaxHealth >= 1000000)
                            spirites = 1000;

                        killer.Player.DailySpiritBeadCount += spirites;
#if Arabic
                          killer.SendSysMesage("You received " + spirites + " spirites.", MsgMessage.ChatMode.System);
#else
                        killer.SendSysMesage("You received " + spirites + " spirites.", MsgMessage.ChatMode.System);
#endif

                        if (Game.MsgNpc.NpcHandler.GetDailySpiritBeadKills(killer) <= killer.Player.DailySpiritBeadCount)
                        {
                            if (!killer.Player.QuestGUI.CheckObjectives(2375, 1))
                            {
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 2375, 1, 1);
#if Arabic
                                 killer.CreateBoxDialog("You`ve~collected~enough~spirits,~and~you~can~use~the~bead~to~claim~a~reward,~now!");
#else
                                killer.CreateBoxDialog("You`ve~collected~enough~spirits,~and~you~can~use~the~bead~to~claim~a~reward,~now!");
#endif

                            }
                        }
                    }
                   
                    if (killer.DemonExterminator != null)
                        killer.DemonExterminator.UppdateJar(killer, Family.ID);
   
                    
                    //////////

                    else if (killer.Player.Map == 3935)
                    {
                        if (Family.ID == 40125)
                        {
                            killer.Send(stream.InterCreateItem(3600031));
                            killer.CreateBoxDialog("You've received a bounty of (Star Stone Pack) for conquering the divine Maniac!");
                        }
                        if (Family.ID == 40121 || Family.ID == 40122 || Family.ID == 40123 || Family.ID == 40124)
                        {
                            killer.Send(stream.InterCreateItem(3600031));
                            killer.CreateBoxDialog("You've received a bounty of (Star Stone Pack) for conquering the divine crystal!");

                        }
                        if (Family.ID == 40120)
                        {
                            if (killer.Player.QuestGUI.CheckQuest(35034, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (killer.Player.QuestGUI.CheckQuest(35034, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                                {
                                    if (killer.Player.QuestGUI.CheckKingDomQuest(35034, 1) == false)
                                    {
                                        killer.Player.QuestGUI.FinishQuest(35034);
                                        killer.CreateBoxDialog("You've completed the [Scramble for Justice] mission in the realm, you've received a bounty of (Senior Dragon Soul)");
                                    }
                                }
                            }
                            killer.Send(stream.InterCreateItem(3600027));
                        }

                        if (Family.ID == 40104 || Family.ID == 40109 || Family.ID == 40108)
                        {
                            if (killer.Player.QuestGUI.CheckQuest(35025, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (killer.Player.QuestGUI.CheckQuest(35025, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                                {
                                    if (killer.Player.QuestGUI.CheckKingDomQuest(35025, 1) == false)
                                    {
                                        killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 35025, 1);
                                        killer.CreateBoxDialog("You've defeated the divine beast! Hurry and claim your reward from the Kingdom Mission Envoy!");

                                    }
                                }
                            }
                        }

                        if (killer.Player.QuestGUI.CheckQuest(35025, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (killer.Player.QuestGUI.CheckQuest(35025, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            {
                                if (killer.Player.QuestGUI.CheckKingDomQuest(35025, 1) == false)
                                {
                                    if (Role.Core.GetDistance(killer.Player.X, killer.Player.Y, 367, 459) <= 18)
                                    {
                                        if (Role.Core.Rate(40))
                                        {
                                            if (killer.Player.View.ContainMobInScreen("WhiteTiger") == false)
                                                Database.Server.AddMapMonster(stream, killer.Map, 40104, killer.Player.X, killer.Player.Y, 367, 459, 1, 0, true, MsgFloorItem.MsgItemPacket.EffectMonsters.Night);
                                        }
                                    }
                                    else if (Role.Core.GetDistance(killer.Player.X, killer.Player.Y, 471, 371) <= 18)
                                    {
                                        if (Role.Core.Rate(40))
                                        {
                                            if (killer.Player.View.ContainMobInScreen("AzureDragon") == false)
                                                Database.Server.AddMapMonster(stream, killer.Map, 40109, killer.Player.X, killer.Player.Y, 471, 371, 1, 0, true, MsgFloorItem.MsgItemPacket.EffectMonsters.Night);
                                        }
                                    }
                                    else if (Role.Core.GetDistance(killer.Player.X, killer.Player.Y, 371, 289) <= 18)
                                    {
                                        if (Role.Core.Rate(40))
                                        {
                                            if (killer.Player.View.ContainMobInScreen("VermilionBird") == false)
                                                Database.Server.AddMapMonster(stream, killer.Map, 40108, killer.Player.X, killer.Player.Y, 371, 289, 1, 0, true, MsgFloorItem.MsgItemPacket.EffectMonsters.Night);
                                        }
                                    }
                                    else if (Role.Core.GetDistance(killer.Player.X, killer.Player.Y, 295, 337) <= 18)
                                    {
                                        if (Role.Core.Rate(40))
                                        {
                                          //  if (killer.Player.View.ContainMobInScreen("BlackTurtle") == false)
                                              //  Database.Server.AddMapMonster(stream, killer.Map, 40110, killer.Player.X, killer.Player.Y, 295, 337, 1, 0, true, MsgFloorItem.MsgItemPacket.EffectMonsters.Night);
                                        }
                                    }
                                }
                            }
                        }

                        if (killer.Player.QuestGUI.CheckQuest(35028, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (killer.Player.QuestGUI.CheckQuest(35028, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            {
                                if (killer.Player.QuestGUI.CheckKingDomQuest(35028, 50) == false)
                                {

                                    killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 35028, 1);

                                    if (killer.Player.QuestGUI.CheckKingDomQuest(35028, 50))
                                        killer.CreateBoxDialog("You've eliminated 50 enemies. Hurry and claim a bounty from Realm Crystal.");

                                }
                            }
                        }

                        if (killer.Map.ID == 30935)
                        {
                            killer.Player.KillCounter++;

                            if (killer.Player.KillCounter >= 100)
                                killer.CreateBoxDialog("You've eliminated 100 enemies. and have been sent to tc.");
                            killer.Teleport(300, 278, 1002);
                            killer.Player.KillCounter = 0;


                        }
                        if (killer.Player.QuestGUI.CheckQuest(35007, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (killer.Player.QuestGUI.CheckQuest(35007, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            {
                                if (killer.Player.QuestGUI.CheckKingDomQuest(35007, 300) == false)
                                {
                                    uint Points = (uint)Program.GetRandom.Next(2, 6);
                                    killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 35007, Points);
                                    uint[] Intentions;
                                    killer.Player.QuestGUI.GetQuestObjectives(35007, out Intentions);

                                    killer.SendSysMesage("You've received " + Math.Min(300, Intentions[0]).ToString() + " Strike Points by killing enemies fiercely. When you earn 300 Strike Points, you can claim a reward from the Kingdom Mission Envoy.", MsgMessage.ChatMode.System);
                                    if (killer.Player.QuestGUI.CheckKingDomQuest(35007, 300))
                                        killer.CreateBoxDialog("You've completed the [Thunder Strike] mission in the realm. Report back to the Kingdom Mission Envoy to claim your reward.");

                                }
                            }
                        }
                        goto jmp;
                    }
                    else if (killer.Player.Map == 3071)
                    {
                        if (Family.ID == 2700)
                        {
                            DropItemID(killer, 720028, stream, 6);
                           // DropItemID(killer, 780000, stream, 6);
                            DropItemID(killer, 3004245, stream, 6);

                            if (Role.Core.Rate(5, 100))
                                DropItemID(killer, 720028, stream, 6);
                            else
                            {
                                if (killer.Inventory.HaveSpace(1))
                                {
                                    killer.Inventory.Add(stream, 711609);
                                    killer.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "dispel4");
                                    killer.CreateBoxDialog("You~received~a~Gold~Coin!");
                                }
                                else
                                {
                                    killer.CreateBoxDialog("Your~inventory~is~full!");
                                }
                            }

                        }
                        else if (Family.ID == 2699)
                        {
                            if (killer.Inventory.HaveSpace(1))
                            {
                                killer.Inventory.Add(stream, 711610);
                                killer.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "dispel7");
                                killer.SendSysMesage("You~received~a~Silver~Coin!", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                killer.CreateBoxDialog("Your~inventory~is~full!");
                            }
                        }
                        else if (Family.ID == 7022)
                        {
                            if (Role.Core.Rate(1, 30))//to check !!!
                            {
                                if (Role.Core.Rate(1, 200))
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        killer.Inventory.Add(stream, 711609);
                                        killer.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "dispel4");
                                        killer.CreateBoxDialog("You~received~a~Gold~Coin!");
                                    }
                                    else
                                    {
                                        killer.CreateBoxDialog("Your~inventory~is~full!");
                                    }
                                }
                                else
                                {
                                    if (Role.Core.Rate(2, 199))
                                    {
                                        if (killer.Inventory.HaveSpace(1))
                                        {
                                            killer.Inventory.Add(stream, 711610);
                                            killer.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "dispel7");
                                            killer.SendSysMesage("You~received~a~Silver~Coin!", MsgMessage.ChatMode.System);
                                        }
                                        else
                                        {
                                            killer.CreateBoxDialog("Your~inventory~is~full!");
                                        }
                                    }
                                    else
                                    {

                                        if (killer.Inventory.HaveSpace(1))
                                        {
                                            uint[] O = { 711611, 711610, 711609 };
                                            int ID = new Random().Next(O.Length);

                                            killer.Inventory.Add(stream, O[ID]);
                                            if (ID == 711609)
                                                killer.CreateBoxDialog("You've got Goldcoin!");
                                            if (ID == 711610)
                                                killer.CreateBoxDialog("You've got SilverCoin!");
                                            if (ID == 711611)
                                                killer.CreateBoxDialog("You've got CooperCoin!");

                                            killer.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "dispel5");

                                        }
                                        else
                                        {
                                            killer.CreateBoxDialog("Your~inventory~is~full!");
                                        }
                                    }
                                }
                            }
                        }
                    }
                   
                    else if (killer.Player.Map == 3998)
                    {
                        if (Family.ID == 40903)
                        {
                            if (killer.Player.QuestGUI.CheckQuest(3634, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 3634, 1);
                                if (killer.Player.QuestGUI.CheckObjectives(3634, 10))
                                {
                                    killer.CreateBoxDialog("You've eliminated enough number of Anger Rats. Hurry and report back to Chong Yan Elder!");
                                }
                            }
                        }
                        else if (Family.ID == 40900)
                        {
                            if (killer.Player.QuestGUI.CheckQuest(3636, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (Role.Core.Rate(45))
                                {
                                    var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)MsgNpc.NpcID.BrokenForgeFurnace, killer.Player.Class, 3636);
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        if (killer.Inventory.Contain(3008752, 8) == false)
                                        {
                                            killer.Inventory.AddItemWitchStack(3008752, 0, 1, stream);
                                        }
                                        if (killer.Inventory.Contain(3008752, 8))
                                            killer.Player.QuestGUI.SendAutoPatcher("You've collected enough number of Rune Fragments. Go and try to complete the runes on the Forge Furnace.", ActiveQuest.FinishNpcId.Map, ActiveQuest.FinishNpcId.X, ActiveQuest.FinishNpcId.Y, ActiveQuest.FinishNpcId.ID);
                                    }
                                    else
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                }
                            }
                        }
                        else if (Family.ID == 40902)
                        {

                            if (killer.Player.QuestGUI.CheckQuest(3638, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (killer.Inventory.HaveSpace(1))
                                {
                                    var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)MsgNpc.NpcID.BrokenForgeFurnace, killer.Player.Class, 3638);
                                    if (!killer.Inventory.Contain(3008750, 1))
                                        killer.Inventory.Add(stream, 3008750);
                                    killer.Player.QuestGUI.SendAutoPatcher("The Violet Bat King fell down and dropped an ancient-style hammer.Hurry and take this hammer to the Forge Furnace.", ActiveQuest.FinishNpcId.Map, ActiveQuest.FinishNpcId.X, ActiveQuest.FinishNpcId.Y, ActiveQuest.FinishNpcId.ID);
                                }
                                else
                                    killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                            }
                        }
                        else if (Family.ID == 40904)
                        {
                            if (killer.Player.QuestGUI.CheckQuest(3641, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 3641, 1);

                                if (killer.Player.QuestGUI.CheckObjectives(3641, 15))
                                    killer.Player.QuestGUI.SendAutoPatcher("You've defeat enough number of Lava Scorpions. Now, you can appease the sacrificed Bright people.", 3998, 220, 294, 0);

                            }

                        }
                        else if (Family.ID == 40908)
                        {
                            if (killer.Player.QuestGUI.CheckQuest(3644, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (killer.Inventory.HaveSpace(1))
                                {
                                    var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)MsgNpc.NpcID.FlameAltar, killer.Player.Class, 3644);
                                    if (!killer.Inventory.Contain(3008742, 50))
                                        killer.Inventory.AddItemWitchStack(3008742, 0, 1, stream);
                                    else
                                        killer.Player.QuestGUI.SendAutoPatcher("You`ve collect enough number of Building Stones. Go and try to restore the ruined altar.", ActiveQuest.FinishNpcId.Map, ActiveQuest.FinishNpcId.X, ActiveQuest.FinishNpcId.Y, ActiveQuest.FinishNpcId.ID);
                                }
                                else
                                    killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                            }
                        }
                        else if (Family.ID == 40907)
                        {
                            if (killer.Player.QuestGUI.CheckQuest(3648, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (killer.Inventory.HaveSpace(1))
                                {
                                    var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)MsgNpc.NpcID.FlameAltar, killer.Player.Class, 3648);
                                    if (!killer.Inventory.Contain(3008748, 100))
                                        killer.Inventory.AddItemWitchStack(3008748, 0, 1, stream);
                                    else
                                        killer.Player.QuestGUI.SendAutoPatcher("You`ve collected enough number of Star Ores. Go and try to extract the Essence of Star at the altar.", ActiveQuest.FinishNpcId.Map, ActiveQuest.FinishNpcId.X, ActiveQuest.FinishNpcId.Y, ActiveQuest.FinishNpcId.ID);
                                }
                                else
                                    killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                            }

                        }
                        else if (Family.ID == 40905)
                        {
                            if (killer.Player.QuestGUI.CheckQuest(3645, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (killer.Inventory.HaveSpace(1))
                                {
                                    if (Role.Core.Rate(40))
                                    {//3008744
                                        var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)MsgNpc.NpcID.FlameAltar, killer.Player.Class, 3645);
                                        if (!killer.Inventory.Contain(3008743, 1))
                                            killer.Inventory.AddItemWitchStack(3008743, 0, 1, stream);
                                        else
                                            killer.Player.QuestGUI.SendAutoPatcher("You`ve retrieved the Wheel of Nature from the Clawed Rock Devil. Hurry and take the next action at the altar.", ActiveQuest.FinishNpcId.Map, ActiveQuest.FinishNpcId.X, ActiveQuest.FinishNpcId.Y, ActiveQuest.FinishNpcId.ID);
                                    }
                                }
                                else
                                    killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                            }
                        }
                        else if (Family.ID == 40906)
                        {
                            if (killer.Player.QuestGUI.CheckQuest(3646, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (killer.Inventory.HaveSpace(1))
                                {
                                    if (Role.Core.Rate(40))
                                    {
                                        var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)MsgNpc.NpcID.FlameAltar, killer.Player.Class, 3646);
                                        if (!killer.Inventory.Contain(3008745, 1))
                                        {

                                            if (!killer.Inventory.Contain(3008754, 5))
                                                killer.Inventory.AddItemWitchStack(3008754, 0, 1, stream);
                                            if (killer.Inventory.Contain(3008754, 5))
                                            {
                                                killer.Inventory.Remove(3008754, 5, stream);
                                                killer.Inventory.Add(stream, 3008745, 1);
                                                killer.Player.QuestGUI.SendAutoPatcher("You received the Earth Force! Hurry and transform it into Metal Force through the Wheel of Nature!", ActiveQuest.FinishNpcId.Map, ActiveQuest.FinishNpcId.X, ActiveQuest.FinishNpcId.Y, ActiveQuest.FinishNpcId.ID);
                                            }
                                        }
                                        else
                                            killer.Player.QuestGUI.SendAutoPatcher("You received the Earth Force! Hurry and transform it into Metal Force through the Wheel of Nature!", ActiveQuest.FinishNpcId.Map, ActiveQuest.FinishNpcId.X, ActiveQuest.FinishNpcId.Y, ActiveQuest.FinishNpcId.ID);
                                    }
                                }
                                else
                                    killer.CreateBoxDialog("You received the Earth Force! Hurry and transform it into Metal Force through the Wheel of Nature!");
                            }


                        }


                      
                    }

                    else if (killer.Player.Map == 1002)
                    {
                        if (Family.ID == 1)
                        {
                            if (killer.Player.QuestGUI.CheckQuest(1307, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1307, 1);
#if Arabic
                                 killer.CreateBoxDialog("You~killed~Pheasant~and~scared~the~Apes~away!");
#else
                                killer.CreateBoxDialog("You~killed~Pheasant~and~scared~the~Apes~away!");
#endif

                            }
                            if (killer.Player.QuestGUI.CheckQuest(824, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 824, 1);
                        }
                        else if (Family.ID == 2)
                        {
                            if (killer.Player.QuestGUI.CheckQuest(1007, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (!killer.Inventory.Contain(711236, 1))
                                {
                                    var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)MsgNpc.NpcID.TCViceCaptain, killer.Player.Class, 1007);
                                    killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1007, 1);
#if Arabic
  killer.Player.QuestGUI.SendAutoPatcher("You`ve~collected~a~Turtledove~Plume.~Please~take~it~to~TC~Vice~Captain.", 1002, ActiveQuest.FinishNpcId.X, ActiveQuest.FinishNpcId.Y, ActiveQuest.FinishNpcId.ID);

#else
                                    killer.Player.QuestGUI.SendAutoPatcher("You`ve~collected~a~Turtledove~Plume.~Please~take~it~to~TC~Vice~Captain.", 1002, ActiveQuest.FinishNpcId.X, ActiveQuest.FinishNpcId.Y, ActiveQuest.FinishNpcId.ID);

#endif

                                    killer.Inventory.Add(stream, 711236);
                                }
                            }
                            //1009
                        }
                        else if (Family.ID == 3)
                        {
                            if (killer.Player.QuestGUI.CheckQuest(1735, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1735, 1);
                                if (killer.Player.QuestGUI.CheckObjectives(1735, 100))
                                {
#if Arabic
                                       killer.CreateBoxDialog("You`ve~collected~enough~Feathers.~Now~you~can~go~report~to~Artisan~Luo.");
#else
                                    killer.CreateBoxDialog("You`ve~collected~enough~Feathers.~Now~you~can~go~report~to~Artisan~Luo.");
#endif

                                }
                            }
                            if (killer.Player.QuestGUI.CheckQuest(1009, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1009, 1);
                            if (killer.Player.QuestGUI.CheckQuest(2365, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 2365, 1);
                                if (killer.Player.QuestGUI.CheckObjectives(2365, 100))
                                {
                                    var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)MsgNpc.NpcID.XuLiang, killer.Player.Class, 2365);
#if Arabic
 killer.Player.QuestGUI.SendAutoPatcher("You`ve~killed~100~Robins!~Report~back~to~XuLiang~and~claim~your~reward.", 1002, ActiveQuest.FinishNpcId.X, ActiveQuest.FinishNpcId.Y, ActiveQuest.FinishNpcId.ID);

#else
                                    killer.Player.QuestGUI.SendAutoPatcher("You`ve~killed~100~Robins!~Report~back~to~XuLiang~and~claim~your~reward.", 1002, ActiveQuest.FinishNpcId.X, ActiveQuest.FinishNpcId.Y, ActiveQuest.FinishNpcId.ID);

#endif

                                }
                            }
                            if (killer.Player.QuestGUI.CheckQuest(1010, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {

                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1010, 1, 1);
                                if (!killer.Inventory.Contain(711238, 10))
                                    killer.Inventory.AddItemWitchStack(711238, 0, 1, stream);
                                if (killer.Player.QuestGUI.CheckObjectives(1010, 10))
                                {
                                    var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)Game.MsgNpc.NpcID.ArtisanLuo, killer.Player.Class, 1010);
#if Arabic
                                        killer.Player.QuestGUI.SendAutoPatcher("You`ve~collected~10~Robin~Feathers.~Please~take~them~to~Artisan~Luo.", 1002, ActiveQuest2.FinishNpcId.X, ActiveQuest2.FinishNpcId.Y, ActiveQuest2.FinishNpcId.ID);
                              
#else
                                    killer.Player.QuestGUI.SendAutoPatcher("You`ve~collected~10~Robin~Feathers.~Please~take~them~to~Artisan~Luo.", 1002, ActiveQuest2.FinishNpcId.X, ActiveQuest2.FinishNpcId.Y, ActiveQuest2.FinishNpcId.ID);

#endif
                                }
                            }
                        }
                        else if (Family.ID == 4)
                        {
                            if (killer.Player.QuestGUI.CheckQuest(1013, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (!killer.Inventory.Contain(711241, 1))
                                {
                                    var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)MsgNpc.NpcID.RuHua, killer.Player.Class, 1013);
                                    killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1013, 1);
#if Arabic
  killer.Player.QuestGUI.SendAutoPatcher("You`ve~killed~the~Apparition~and~received~a~Handkerchief!", 1002, ActiveQuest.FinishNpcId.X, ActiveQuest.FinishNpcId.Y, ActiveQuest.FinishNpcId.ID);

#else
                                    killer.Player.QuestGUI.SendAutoPatcher("You`ve~killed~the~Apparition~and~received~a~Handkerchief!", 1002, ActiveQuest.FinishNpcId.X, ActiveQuest.FinishNpcId.Y, ActiveQuest.FinishNpcId.ID);

#endif

                                    killer.Inventory.Add(stream, 711241);
                                }
                            }
                            if (killer.Player.QuestGUI.CheckQuest(1016, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
#if Arabic
                                killer.SendSysMesage("You've collected spirit of an Apparition and stored it in Sipirit Container.", MsgMessage.ChatMode.System);
#else
                                killer.SendSysMesage("You've collected spirit of an Apparition and stored it in Sipirit Container.", MsgMessage.ChatMode.System);
#endif

                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1016, 1);
                                if (killer.Player.QuestGUI.CheckObjectives(1016, 10))
                                {
                                    var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)MsgNpc.NpcID.Fortuneteller, killer.Player.Class, 1016);
#if Arabic
  killer.Player.QuestGUI.SendAutoPatcher("You`ve~collected~the~spirits~of~10~Apparitions.~Hurry~to~report~back~to~the~Fortuneteller!", 1002, ActiveQuest.FinishNpcId.X, ActiveQuest.FinishNpcId.Y, ActiveQuest.FinishNpcId.ID);

#else
                                    killer.Player.QuestGUI.SendAutoPatcher("You`ve~collected~the~spirits~of~10~Apparitions.~Hurry~to~report~back~to~the~Fortuneteller!", 1002, ActiveQuest.FinishNpcId.X, ActiveQuest.FinishNpcId.Y, ActiveQuest.FinishNpcId.ID);

#endif

                                }
                            }
                        }
                    }
                    else if (killer.Player.Map == 1013 && killer.Player.DynamicID != 0)
                    {
                        if (Family.ID == 14333)
                        {
                            if (killer.Player.QuestGUI.CheckQuest(1748, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1748, 1);
                                if (killer.Player.QuestGUI.CheckObjectives(1748, 5))
                                {
                                    killer.Teleport(299, 244, 1002);
#if Arabic
                                       killer.SendSysMesage("You`ve~returned~to~Royal~Doctor~Li.");
#else
                                    killer.SendSysMesage("You`ve~returned~to~Royal~Doctor~Li.");
#endif

                                }
                            }
                        }

                    }
                    else if (killer.Player.Map == 10250) //DeityLandEvent
                    {
                        if (Family.ID == 8419)
                            if (Role.Core.Rate(1, 70))
                            {
                                if (killer.Inventory.HaveSpace(1))
                                {
                                    killer.Inventory.Add(stream, 3301729);
                                    killer.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "dispel7");
                                    SendSysMesage(killer.Player.Name + " This Player Dropped [ LakeStone ].", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);
                                    killer.SendSysMesage("You~received~a~LakeStone!", MsgMessage.ChatMode.System);
                                }
                                else
                                {
                                    killer.CreateBoxDialog("Your~inventory~is~full!");
                                }
                            }
                    }
                    else if (killer.Player.Map == 1015)
                    {
                        if (Family.ID == 2171)
                        {
                            if (!killer.Inventory.Contain(710217, 1))
                            {
                                if (killer.Inventory.HaveSpace(1))
                                {
                                    //killer.Inventory.Add(stream, 710217, 1);
                                   // killer.Player.QuestGUI.SetQuestObjectives(stream, 408, 1);
                                    //killer.SendSysMesage("You`ve killed the Jade Courtesan and obtained her Jade Sceptre.");
                                    if (killer.Inventory.Contain(710216, 1))
                                    {
                                        if (killer.Inventory.Contain(710217, 1))
                                        {
                                            if (killer.Inventory.Contain(710218, 1))
                                            {
                                                if (killer.Inventory.Contain(710219, 1))
                                                {
                                                    killer.SendSysMesage("You`ve collected the Purple Eye, Jade Scepter, Jadeite, and Hairpin. Go to open the Teleport Platform on Bird Island (655,693).");
                                                }
                                                else
                                                {
                                                    killer.SendSysMesage("As long as you collect the Purple Eye, Jade Sceptre, Jadeite, and Hairpin, you can open the Platform (Bird Island 655,693) and go to the Rose Garden.");
                                                }
                                            }
                                            else
                                            {
                                                killer.SendSysMesage("As long as you collect the Purple Eye, Jade Sceptre, Jadeite, and Hairpin, you can open the Platform (Bird Island 655,693) and go to the Rose Garden.");
                                            }
                                        }
                                        else
                                        {
                                            killer.SendSysMesage("As long as you collect the Purple Eye, Jade Sceptre, Jadeite, and Hairpin, you can open the Platform (Bird Island 655,693) and go to the Rose Garden.");
                                        }
                                    }
                                    else
                                    {
                                        killer.SendSysMesage("As long as you collect the Purple Eye, Jade Sceptre, Jadeite, and Hairpin, you can open the Platform (Bird Island 655,693) and go to the Rose Garden.");
                                    }
                                }
                                else
                                {
                                    killer.SendSysMesage("Your inventory is full! Make some room, first.");
                                }
                            }

                        }
                        else if (Family.ID == 2157)//bloodyshaw
                        {
                            if (killer.Player.QuestGUI.CheckQuest(523, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (killer.Inventory.HaveSpace(1))
                                {
                                    if (!killer.Inventory.Contain(721781, 1))
                                    {
                                        killer.Inventory.Add(stream, 721781, 1);
                                        killer.CreateBoxDialog("Your~power~and~fortitude~overwhelmed~Bloody~Shawn,~and~you~obtained~his~Spear.~Grab~it~along~and~find~Ruan~Brother.");
                                    }
                                }
                                else
                                {
                                    killer.CreateBoxDialog("Your~inventory~is~full!");
                                }
                            }
                        }
                        else if (Family.ID == 2156)
                        {
                            if (killer.Player.QuestGUI.CheckQuest(522, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (!killer.Inventory.Contain(721778, 3))
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        killer.Inventory.AddItemWitchStack(721778, 0, 1, stream);
                                        killer.CreateBoxDialog("Trout~Elves~died~and~turned~into~fresh~fish.~Collect~3~fish~and~trade~with~Ye~Sheng~(Bird~Island~324,400)~for~his~Iron~Poise.");

                                    }
                                    else
                                    {
                                        killer.CreateBoxDialog("Your~inventory~is~full!");
                                    }
                                }
                            }
                        }
                        else if (Family.ID == 85)//senior
                        {
                            if (killer.Player.QuestGUI.CheckQuest(1824, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (Role.Core.Rate(30))
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        if (!killer.Inventory.Contain(710566, 1))
                                        {
                                            killer.SendSysMesage("You picked up the Scarlet Token. Now hurry up and give it to Felix!", MsgMessage.ChatMode.System);
                                            killer.Inventory.AddItemWitchStack(710566, 0, 1, stream);
                                        }
                                    }
                                    else
                                        killer.CreateBoxDialog("Sorry, your inventory is full. Make some room first.");
                                }

                            }
                        }
                        if (Family.ID == 2226)//banditdelivery
                        {
                            if (killer.Player.QuestGUI.CheckQuest(1823, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (Role.Core.Rate(30))
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        if (!killer.Inventory.Contain(710565, 1))
                                        {
                                            killer.SendSysMesage("You picked up the Pitch Token. Now hurry up and give it to Felix!", MsgMessage.ChatMode.System);
                                            killer.Inventory.AddItemWitchStack(710565, 0, 1, stream);
                                        }
                                    }
                                    else
                                        killer.CreateBoxDialog("Sorry, your inventory is full. Make some room first.");
                                }

                            }
                        }
                        if (Family.ID == 2225)
                        {
                            if (killer.Player.QuestGUI.CheckQuest(1822, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (Role.Core.Rate(50))
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        if (!killer.Inventory.Contain(710564, 1))
                                        {
                                            killer.SendSysMesage("You picked up the Black Token. Now hurry up and give it to Felix!", MsgMessage.ChatMode.System);
                                            killer.Inventory.AddItemWitchStack(710564, 0, 1, stream);
                                        }
                                    }
                                    else
                                        killer.CreateBoxDialog("Sorry, your inventory is full. Make some room first.");
                                }

                            }
                        }
                        if (Family.ID == 79)//banditL98
                        {
                            if (killer.Player.QuestGUI.CheckQuest(1821, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (Role.Core.Rate(30))
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        if (!killer.Inventory.Contain(710563, 1))
                                        {
                                            killer.SendSysMesage("You~picked~up~the~Indigo~Token.~Now~hurry~up~and~give~it~to~Felix!", MsgMessage.ChatMode.System);
                                            killer.Inventory.AddItemWitchStack(710563, 0, 1, stream);
                                        }
                                    }
                                    else
                                        killer.CreateBoxDialog("Sorry, your inventory is full. Make some room first.");
                                }
                            }
                            if (killer.Player.QuestGUI.CheckQuest(1642, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
#if Arabic
                                killer.SendSysMesage("You`ve killed a Banditti.", MsgMessage.ChatMode.System);
#else
                                killer.SendSysMesage("You`ve killed a Banditti.", MsgMessage.ChatMode.System);
#endif
                                if (Role.Core.Rate(40))
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        if (!killer.Inventory.Contain(711474, 10))
                                        {
                                            killer.SendSysMesage("You received a Shot Saber!", MsgMessage.ChatMode.System);
                                            killer.Inventory.AddItemWitchStack(711474, 0, 1, stream);
                                        }
                                    }
                                    else
                                    {
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                    }

                                }

                            }
                            if (killer.Player.QuestGUI.CheckQuest(1641, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
#if Arabic
                                killer.SendSysMesage("You`ve killed a BanditL98.", MsgMessage.ChatMode.System);
#else
                                killer.SendSysMesage("You`ve killed a BanditL98.", MsgMessage.ChatMode.System);
#endif
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1641, 1);

                            }
                            if (killer.Player.QuestGUI.CheckQuest(1647, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
#if Arabic
                                killer.SendSysMesage("You`ve killed a BanditL98.", MsgMessage.ChatMode.System);
#else
                                killer.SendSysMesage("You`ve killed a BanditL98.", MsgMessage.ChatMode.System);
#endif
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1647, 0, 1);

                            }

                        }
                        if (Family.ID == 84)//banditti
                        {
                            if (killer.Player.QuestGUI.CheckQuest(1651, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
#if Arabic
                                killer.SendSysMesage("You`ve killed a Banditti.", MsgMessage.ChatMode.System);
#else
                                killer.SendSysMesage("You`ve killed a Banditti.", MsgMessage.ChatMode.System);
#endif
                                if (Role.Core.Rate(40))
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        if (!killer.Inventory.Contain(711476, 1))
                                        {
                                            killer.SendSysMesage("You received 1 Yarn Ball!", MsgMessage.ChatMode.System);
                                            killer.Inventory.AddItemWitchStack(711476, 0, 1, stream);
                                        }
                                    }
                                    else
                                    {
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                    }

                                }

                            }

                            if (killer.Player.QuestGUI.CheckQuest(1652, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
#if Arabic
                                killer.SendSysMesage("You`ve killed a Banditti.", MsgMessage.ChatMode.System);
#else
                                killer.SendSysMesage("You`ve killed a Banditti.", MsgMessage.ChatMode.System);
#endif
                                if (Role.Core.Rate(40))
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        if (!killer.Inventory.Contain(711477, 1))
                                        {
                                            killer.SendSysMesage("You received Flower Wine!", MsgMessage.ChatMode.System);
                                            killer.Inventory.AddItemWitchStack(711477, 0, 1, stream);
                                        }
                                    }
                                    else
                                    {
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                    }

                                }

                            }

                            if (killer.Player.QuestGUI.CheckQuest(1650, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
#if Arabic
                                killer.SendSysMesage("You`ve killed a Banditti.", MsgMessage.ChatMode.System);
#else
                                killer.SendSysMesage("You`ve killed a Banditti.", MsgMessage.ChatMode.System);
#endif
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1650, 1);

                            }
                            if (killer.Player.QuestGUI.CheckQuest(1645, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
#if Arabic
                                killer.SendSysMesage("You`ve killed a Banditti.", MsgMessage.ChatMode.System);
#else
                                killer.SendSysMesage("You`ve killed a Banditti.", MsgMessage.ChatMode.System);
#endif
                                if (Role.Core.Rate(40))
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        if (!killer.Inventory.Contain(711475, 1))
                                        {
                                            killer.SendSysMesage("You received the Royal Sword!", MsgMessage.ChatMode.System);
                                            killer.Inventory.AddItemWitchStack(711475, 0, 1, stream);
                                        }
                                    }
                                    else
                                    {
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                    }

                                }

                            }
                        }
                        if (Family.ID == 2224)
                        {

                            if (killer.Player.QuestGUI.CheckQuest(1820, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (killer.Inventory.HaveSpace(1))
                                {
                                    if (!killer.Inventory.Contain(710562, 1))
                                    {
                                        killer.Inventory.Add(stream, 710562);
                                        killer.CreateBoxDialog("You picked up the Red Token. Now hurry up and give it to Felix!");
                                    }
                                }
                                else
                                {
                                    killer.CreateBoxDialog("Sorry, your inventory is full. Make some room first.");
                                }
                            }
                        }
                        if (Family.ID == 55)//banditL97
                        {


                            if (killer.Player.QuestGUI.CheckQuest(1820, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1820, 1);
                                if (killer.Player.QuestGUI.CheckObjectives(1820, 50))
                                {
                                    if (!killer.Inventory.Contain(710562, 1))
                                    {
                                        if (!killer.Player.View.ContainMobInScreen("BanditLeaderL97"))
                                        {
                                            killer.CreateBoxDialog("Bandit Leader L97 appeared on Bird Island (262,118). Get rid of him!");
                                            Database.Server.AddMapMonster(stream, killer.Map, 2224, killer.Player.X, killer.Player.Y, 3, 3, 1, killer.Player.DynamicID, true, MsgFloorItem.MsgItemPacket.EffectMonsters.None, "SuperXp-4");
                                        }

                                    }
                                }

                            }

                            if (killer.Player.QuestGUI.CheckQuest(1647, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
#if Arabic
                                killer.SendSysMesage("You`ve killed a BanditL97.", MsgMessage.ChatMode.System);
#else
                                killer.SendSysMesage("You`ve killed a BanditL97.", MsgMessage.ChatMode.System);
#endif
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1647, 1);

                            }

                            if (killer.Player.QuestGUI.CheckQuest(1635, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
#if Arabic
                                killer.SendSysMesage("You`ve killed a BanditL97.", MsgMessage.ChatMode.System);
#else
                                killer.SendSysMesage("You`ve killed a BanditL97.", MsgMessage.ChatMode.System);
#endif
                                if (Role.Core.Rate(20))
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        if (!killer.Inventory.Contain(711468, 1))
                                        {
                                            killer.SendSysMesage("You~received~a~box~of~Food~Supply!", MsgMessage.ChatMode.System);
                                            killer.Inventory.AddItemWitchStack(711468, 0, 1, stream);
                                        }
                                    }
                                    else
                                    {
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                    }

                                }

                            }

                            if (killer.Player.QuestGUI.CheckQuest(1632, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
#if Arabic
                                killer.SendSysMesage("You`ve killed a BanditL97.", MsgMessage.ChatMode.System);
#else
                                killer.SendSysMesage("You`ve killed a BanditL97.", MsgMessage.ChatMode.System);
#endif
                                if (Role.Core.Rate(20))
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        if (!killer.Inventory.Contain(711467, 5))
                                        {
                                            killer.SendSysMesage("You've received a piece of Bacon!", MsgMessage.ChatMode.System);
                                            killer.Inventory.AddItemWitchStack(711467, 0, 1, stream);
                                        }
                                    }
                                    else
                                    {
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                    }

                                }

                            }
                            if (killer.Player.QuestGUI.CheckQuest(1633, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
#if Arabic
                                killer.SendSysMesage("You`ve killed a BanditL97.", MsgMessage.ChatMode.System);
#else
                                killer.SendSysMesage("You`ve killed a BanditL97.", MsgMessage.ChatMode.System);
#endif

                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1633, 1);

                            }
                        }
                        if (Family.ID == 19)//hawkings
                        {
                            if (killer.Player.QuestGUI.CheckQuest(1814, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (killer.Inventory.HaveSpace(1))
                                {
                                    if (Role.Core.Rate(20))
                                    {
                                        if (!killer.Inventory.Contain(721910, 5))
                                        {
                                            killer.Inventory.AddItemWitchStack(721910, 0, 1, stream);
                                            killer.CreateBoxDialog("You've got a Hawk Claw!");
                                        }
                                        else
                                            killer.CreateBoxDialog("You've got 5 Hawk Claws! Hurry to deliver them to Doctor Know-it-All!");
                                    }
                                }
                                else
                                    killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                            }
                            if (killer.Player.QuestGUI.CheckQuest(1813, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (killer.Inventory.HaveSpace(1))
                                {
                                    if (Role.Core.Rate(20))
                                    {
                                        if (!killer.Inventory.Contain(721909, 1) && !killer.Inventory.Contain(721908, 1))
                                        {
                                            killer.Inventory.Add(stream, 721909);
                                            killer.CreateBoxDialog("You've got a Gallbladerr!");
                                        }
                                    }

                                }
                                else
                                    killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                            }

                            if (killer.Player.QuestGUI.CheckQuest(1630, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
#if Arabic
                                killer.SendSysMesage("You`ve killed a HawKingL93.", MsgMessage.ChatMode.System);
#else
                                killer.SendSysMesage("You`ve killed a HawKingL93.", MsgMessage.ChatMode.System);
#endif

                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1630, 0, 1);

                            }
                            if (killer.Player.QuestGUI.CheckQuest(1616, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
#if Arabic
                                killer.SendSysMesage("You`ve killed a HawKing.", MsgMessage.ChatMode.System);
#else
                                killer.SendSysMesage("You`ve killed a HawKing.", MsgMessage.ChatMode.System);
#endif
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1616, 2, 1);


                            }
                            if (killer.Player.QuestGUI.CheckQuest(1611, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
#if Arabic
                                killer.SendSysMesage("You`ve killed a HawKing.", MsgMessage.ChatMode.System);
#else
                                killer.SendSysMesage("You`ve killed a HawKing.", MsgMessage.ChatMode.System);
#endif
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1611, 1);

                            }
                            if (killer.Player.QuestGUI.CheckQuest(1618, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
#if Arabic
                                killer.SendSysMesage("You`ve killed a HawKing.", MsgMessage.ChatMode.System);
#else
                                killer.SendSysMesage("You`ve killed a HawKing.", MsgMessage.ChatMode.System);
#endif
                                if (Role.Core.Rate(20))
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        if (!killer.Inventory.Contain(711454, 5))
                                        {
                                            killer.SendSysMesage("You`ve~got~a~Feather!", MsgMessage.ChatMode.System);
                                            killer.Inventory.Add(stream, 711454);
                                        }
                                    }
                                    else
                                    {
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                    }

                                }

                            }


                            if (killer.Player.QuestGUI.CheckQuest(1610, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
#if Arabic
                                killer.SendSysMesage("You`ve killed a HawKing.", MsgMessage.ChatMode.System);
#else
                                killer.SendSysMesage("You`ve killed a HawKing.", MsgMessage.ChatMode.System);
#endif
                                if (Role.Core.Rate(20))
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        if (!killer.Inventory.Contain(720987, 1))
                                        {
                                            killer.SendSysMesage("You`ve~got~a~OldBook1!", MsgMessage.ChatMode.System);
                                            killer.Inventory.Add(stream, 720987);
                                        }
                                        else if (!killer.Inventory.Contain(720988, 1))
                                        {
                                            killer.SendSysMesage("You`ve~got~a~OldBook2!", MsgMessage.ChatMode.System);
                                            killer.Inventory.Add(stream, 720988);
                                        }
                                        else if (!killer.Inventory.Contain(720989, 1))
                                        {
                                            killer.SendSysMesage("You`ve~got~a~OldBook3!", MsgMessage.ChatMode.System);
                                            killer.Inventory.Add(stream, 720989);
                                        }
                                    }
                                    else
                                    {
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                    }

                                }

                            }

                        }
                        if (Family.ID == 4431179)//haw leader
                        {
                            if (killer.Player.QuestGUI.CheckQuest(1629, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
#if Arabic
                                killer.SendSysMesage("You`ve killed a HawkLeader.", MsgMessage.ChatMode.System);
#else
                                killer.SendSysMesage("You`ve killed a HawkLeader.", MsgMessage.ChatMode.System);
#endif

                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1629, 1);
                            }
                        }
                        if (Family.ID == 78)//hawkingsL93
                        {
                            if (killer.Player.QuestGUI.CheckQuest(1814, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (killer.Inventory.HaveSpace(1))
                                {
                                    if (Role.Core.Rate(20))
                                    {
                                        if (!killer.Inventory.Contain(721910, 5))
                                        {
                                            killer.Inventory.AddItemWitchStack(721910, 0, 1, stream);
                                            killer.CreateBoxDialog("You've got a Hawk Claw!");
                                        }
                                        else
                                            killer.CreateBoxDialog("You've got 5 Hawk Claws! Hurry to deliver them to Doctor Know-it-All!");
                                    }
                                }
                                else
                                    killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                            }
                            if (killer.Player.QuestGUI.CheckQuest(1813, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (Role.Core.Rate(20))
                                {
                                    if (!killer.Inventory.Contain(721909, 1) && !killer.Inventory.Contain(721908, 1))
                                    {
                                        killer.Inventory.Add(stream, 721909);
                                        killer.CreateBoxDialog("You've got a Gallbladerr!");
                                    }
                                }
                            }
                            if (killer.Player.QuestGUI.CheckQuest(1630, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
#if Arabic
                                killer.SendSysMesage("You`ve killed a HawKingL93.", MsgMessage.ChatMode.System);
#else
                                killer.SendSysMesage("You`ve killed a HawKingL93.", MsgMessage.ChatMode.System);
#endif

                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1630, 1);

                            }


                            if (killer.Player.QuestGUI.CheckQuest(1628, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
#if Arabic
                                killer.SendSysMesage("You`ve killed a HawKingL93.", MsgMessage.ChatMode.System);
#else
                                killer.SendSysMesage("You`ve killed a HawKingL93.", MsgMessage.ChatMode.System);
#endif
                                if (Role.Core.Rate(20))
                                {
                                    killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1628, 1);
                                }
                            }

                            if (killer.Player.QuestGUI.CheckQuest(1624, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
#if Arabic
                                killer.SendSysMesage("You`ve killed a HawKingL93.", MsgMessage.ChatMode.System);
#else
                                killer.SendSysMesage("You`ve killed a HawKingL93.", MsgMessage.ChatMode.System);
#endif
                                if (Role.Core.Rate(20))
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        if (!killer.Inventory.Contain(720993, 1))
                                        {
                                            killer.SendSysMesage("You~received~some~Hawk~Blood!", MsgMessage.ChatMode.System);
                                            killer.Inventory.Add(stream, 720993);
                                        }

                                    }
                                    else
                                    {
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                    }

                                }

                            }
                            if (killer.Player.QuestGUI.CheckQuest(1622, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
#if Arabic
                                killer.SendSysMesage("You`ve killed a HawKingL93.", MsgMessage.ChatMode.System);
#else
                                killer.SendSysMesage("You`ve killed a HawKingL93.", MsgMessage.ChatMode.System);
#endif
                                if (Role.Core.Rate(20))
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        if (!killer.Inventory.Contain(711456, 1))
                                        {
                                            killer.SendSysMesage("You~received~1~Hawk~Claw!", MsgMessage.ChatMode.System);
                                            killer.Inventory.Add(stream, 711456);
                                        }
                                        else if (!killer.Inventory.Contain(711457, 1))
                                        {
                                            killer.SendSysMesage("You~received~1~Hawk~Fang!", MsgMessage.ChatMode.System);
                                            killer.Inventory.Add(stream, 711457);
                                        }
                                    }
                                    else
                                    {
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                    }

                                }

                            }

                            if (killer.Player.QuestGUI.CheckQuest(1621, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
#if Arabic
                                killer.SendSysMesage("You`ve killed a HawKingL93.", MsgMessage.ChatMode.System);
#else
                                killer.SendSysMesage("You`ve killed a HawKingL93.", MsgMessage.ChatMode.System);
#endif
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1621, 1);

                            }
                        }
                        if (Family.ID == 77)//birdmanl88
                        {
                            //721906
                            if (killer.Player.QuestGUI.CheckQuest(1812, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {

                                if (Role.Core.Rate(40))
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        if (!killer.Inventory.Contain(721906, 10))
                                        {
                                            killer.Inventory.AddItemWitchStack(721906, 0, 1, stream);
                                            killer.CreateBoxDialog("You've got some Bird Blood!");
                                        }
                                        else
                                            killer.CreateBoxDialog("You've collected enough Bird Blood!");
                                    }
                                    else
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                }
                            }
                            if (killer.Player.QuestGUI.CheckQuest(1599, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
#if Arabic
                                killer.SendSysMesage("You`ve killed a BirdmanL88.", MsgMessage.ChatMode.System);
#else
                                killer.SendSysMesage("You`ve killed a BirdmanL88.", MsgMessage.ChatMode.System);
#endif
                                if (Role.Core.Rate(40))
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        if (!killer.Inventory.Contain(711445, 5))
                                        {
                                            killer.SendSysMesage("You`ve~got~a~Bird~Beak!", MsgMessage.ChatMode.System);
                                            killer.Inventory.Add(stream, 711445);
                                        }
                                    }
                                    else
                                    {
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                    }

                                }
                            }
                            if (killer.Player.QuestGUI.CheckQuest(1601, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
#if Arabic
                                killer.SendSysMesage("You`ve killed a BirdmanL88.", MsgMessage.ChatMode.System);
#else
                                killer.SendSysMesage("You`ve killed a BirdmanL88.", MsgMessage.ChatMode.System);
#endif
                                if (Role.Core.Rate(20))
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        if (!killer.Inventory.Contain(711446, 1))
                                        {
                                            killer.SendSysMesage("You`ve~got~a~Sharp~Claw!", MsgMessage.ChatMode.System);
                                            killer.Inventory.Add(stream, 711446);
                                        }
                                    }
                                    else
                                    {
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                    }

                                }
                            }
                            if (killer.Player.QuestGUI.CheckQuest(1605, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
#if Arabic
                                killer.SendSysMesage("You`ve killed a BirdmanL88.", MsgMessage.ChatMode.System);
#else
                                killer.SendSysMesage("You`ve killed a BirdmanL88.", MsgMessage.ChatMode.System);
#endif
                                if (Role.Core.Rate(20))
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        if (!killer.Inventory.Contain(711449, 5))
                                        {
                                            killer.SendSysMesage("You received a Birdman Feather!", MsgMessage.ChatMode.System);
                                            killer.Inventory.AddItemWitchStack(711449, 0, 1, stream);
                                        }
                                    }
                                    else
                                    {
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                    }

                                }

                            }
                            if (killer.Player.QuestGUI.CheckQuest(1604, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
#if Arabic
                                killer.SendSysMesage("You`ve killed a BirdmanL88.", MsgMessage.ChatMode.System);
#else
                                killer.SendSysMesage("You`ve killed a BirdmanL88.", MsgMessage.ChatMode.System);
#endif
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1604, 1);

                            }
                            if (killer.Player.QuestGUI.CheckQuest(1598, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
#if Arabic
                                killer.SendSysMesage("You`ve killed a BirdmanL88.", MsgMessage.ChatMode.System);
#else
                                killer.SendSysMesage("You`ve killed a BirdmanL88.", MsgMessage.ChatMode.System);
#endif
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1598, 1);

                            }

                        }
                        if (Family.ID == 18)//birdman
                        {
                            if (Family.ID == 18)//bird man
                            {
                                if (Role.Core.Rate(10))
                                {
                                    MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                                    DataItem.ITEM_ID = 729094;//birdclaw
                                    Database.ItemType.DBItem DBItem;
                                    if (Database.Server.ItemsBase.TryGetValue(729094, out DBItem))
                                    {
                                        DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                                    }
                                    DataItem.Color = Role.Flags.Color.Red;
                                    ushort xx = X;
                                    ushort yy = Y;
                                    if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                                    {
                                        MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                                        if (killer.Map.EnqueueItem(DropItem))
                                        {
                                            DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                        }
                                    }
                                }
                            }
                            if (Map == 30935)//birdman
                            {
                                if (Map == 30935)//bird man
                                {
                                    if (killer.Player.KillCounter >= 100)
                                    {
                                        killer.Teleport(300, 278, 1002);
                                    }
                                    if (Role.Core.Rate(50))
                                    {
                                        byte randskillbook = (byte)Program.GetRandom.Next(1, 10);
                                        uint itemz = 0;
                                        switch (randskillbook)
                                        {
                                            case 1: itemz = 730001; break;
                                            case 2: itemz = 730002; break;
                                            case 3: itemz = 730003; break;
                                            case 4: itemz = 730004; break;
                                            case 5: itemz = 730006; break;
                                            case 6: itemz = 730001; break;
                                            case 7: itemz = 730001; break;
                                            case 8: itemz = 730001; break;
                                            case 9: itemz = 730002; break;
                                            case 10: itemz = 730003; break;

                                            default: itemz = 725024; break;
                                        }
                                        uint randomitem = itemz;
                                        MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                                        DataItem.ITEM_ID = randomitem;//birdclaw
                                        Database.ItemType.DBItem DBItem;
                                        if (Database.Server.ItemsBase.TryGetValue(randomitem, out DBItem))
                                        {
                                            DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                                        }
                                        DataItem.Color = Role.Flags.Color.Red;
                                        DataItem.Bound = 1;
                                        ushort xx = X;
                                        ushort yy = Y;
                                        if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                                        {
                                            MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                                            if (killer.Map.EnqueueItem(DropItem))
                                            {
                                                DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                            }
                                        }
                                    }
                                }
                            }
                            if (killer.Player.QuestGUI.CheckQuest(1812, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {

                                if (Role.Core.Rate(40))
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        if (!killer.Inventory.Contain(721906, 10))
                                        {
                                            killer.Inventory.AddItemWitchStack(721906, 0, 1, stream);
                                            killer.CreateBoxDialog("You've got some Bird Blood!");
                                        }
                                        else
                                            killer.CreateBoxDialog("You've collected enough Bird Blood!");
                                    }
                                    else
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                }
                            }
                            if (killer.Player.Map == 30935)
                            {

                                if (Role.Core.Rate(10))
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        uint[] O = { 730002, 730001, 730003, 730006 };
                                        int ID = new Random().Next(O.Length);
                                        if (ID == 730001)
                                            killer.Inventory.Add100(O[ID], 1, 1, stream);
                                        if (ID == 730002)
                                            killer.Inventory.Add100(O[ID], 2, 1, stream);
                                        if (ID == 730003)
                                            killer.Inventory.Add100(O[ID], 3, 1, stream);
                                        if (ID == 730006)
                                            killer.Inventory.Add100(O[ID], 6, 1, stream, true);
                                        if (ID == 730001)
                                            killer.CreateBoxDialog("You've got Stone+1!");
                                        if (ID == 730002)
                                            killer.CreateBoxDialog("You've got Stone+2!");
                                        if (ID == 730003)
                                            killer.CreateBoxDialog("You've got Stone+3!");
                                        if (ID == 730006)
                                            killer.CreateBoxDialog("You've got Stone+6!");

                                    }
                                    else
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                }
                            }
                            if (killer.Player.QuestGUI.CheckQuest(1626, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
#if Arabic
                                killer.SendSysMesage("You`ve killed a Birdman.", MsgMessage.ChatMode.System);
#else
                                killer.SendSysMesage("You`ve killed a Birdman.", MsgMessage.ChatMode.System);
#endif
                                if (Role.Core.Rate(20))
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        if (!killer.Inventory.Contain(711464, 1))
                                        {
                                            killer.SendSysMesage("You~received~1~Golden~Beak!", MsgMessage.ChatMode.System);
                                            killer.Inventory.Add(stream, 711464);
                                        }
                                    }
                                    else
                                    {
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                    }

                                }
                            }

                            if (killer.Player.QuestGUI.CheckQuest(1597, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
#if Arabic
                                killer.SendSysMesage("You`ve killed a Birdman.", MsgMessage.ChatMode.System);
#else
                                killer.SendSysMesage("You`ve killed a Birdman.", MsgMessage.ChatMode.System);
#endif
                                if (Role.Core.Rate(20))
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        if (!killer.Inventory.Contain(720986, 1))
                                        {
                                            killer.SendSysMesage("You`ve~got~some~Birdman~Feathers!", MsgMessage.ChatMode.System);
                                            killer.Inventory.Add(stream, 720986);
                                        }
                                    }
                                    else
                                    {
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                    }

                                }
                            }
                            if (killer.Player.QuestGUI.CheckQuest(1592, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
#if Arabic
                                killer.SendSysMesage("You`ve killed a Birdman.", MsgMessage.ChatMode.System);
#else
                                killer.SendSysMesage("You`ve killed a Birdman.", MsgMessage.ChatMode.System);
#endif
                                if (Role.Core.Rate(20))
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        if (!killer.Inventory.Contain(711438, 5))
                                        {
                                            killer.SendSysMesage("You received a Birdman Arm!", MsgMessage.ChatMode.System);
                                            killer.Inventory.Add(stream, 711438);
                                        }
                                    }
                                    else
                                    {
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                    }

                                }
                            }

                            if (killer.Player.QuestGUI.CheckQuest(1591, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
#if Arabic
                                killer.SendSysMesage("You`ve killed a Birdman.", MsgMessage.ChatMode.System);
#else
                                killer.SendSysMesage("You`ve killed a Birdman.", MsgMessage.ChatMode.System);
#endif
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1591, 1);

                            }
                            if (killer.Player.QuestGUI.CheckQuest(1593, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (Role.Core.Rate(44))
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        killer.Inventory.Add(stream, 711439);
                                        killer.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "relive1");
                                        killer.CreateBoxDialog("You`ve~got~Xu~Fan`s~Tool~Bag!");
                                    }
                                    else
                                    {
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                    }
                                }

                            }

                        }
                    }
                    else if (killer.Player.Map == 1000)
                    {
                        if (Family.ID == 2163)
                        {
                            if (killer.Player.QuestGUI.CheckQuest(525, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (killer.Inventory.HaveSpace(1))
                                {
                                    if (!killer.Inventory.Contain(721782, 1))
                                    {
                                        killer.Inventory.Remove(721777, 1, stream);
                                        killer.Inventory.Add(stream, 721782, 1);
                                        killer.CreateBoxDialog("Your~gut~feeling~told~you~that~the~Bud~Handkerchief~has~something~to~do~with~Yang~Yun~in~Bird~Island.~Find~him~(732,516)~now.");
                                    }
                                    else
                                    {
                                        killer.CreateBoxDialog("Your~gut~feeling~told~you~that~the~Bud~Handkerchief~has~something~to~do~with~Yang~Yun~in~Bird~Island.~Find~him~(732,516)~now.");
                                    }
                                }
                                else
                                    killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                            }
                        }
                        else if (Family.ID == 2179)
                        {
                            if (killer.Player.QuestGUI.CheckQuest(525, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (Role.Core.Rate(20))
                                {
                                    if (!killer.Inventory.Contain(721782, 1))
                                    {
                                        killer.CreateBoxDialog("Caprice~Leader~has~appeared~at~(170,230).");
                                        Database.Server.AddMapMonster(stream, killer.Map, 2163, 170, 230, 3, 3, 1, killer.Player.DynamicID, true, MsgFloorItem.MsgItemPacket.EffectMonsters.None, "ride_screen");
                                    }
                                }
                            }
                        }
                        else if (Family.ID == 55434417)//angry blade ghost
                        {
                            if (killer.Player.QuestGUI.CheckQuest(1498, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
#if Arabic
                                killer.SendSysMesage("You`ve killed a AngryBladeGhost.", MsgMessage.ChatMode.System);
#else
                                killer.SendSysMesage("You`ve killed a AngryBladeGhost.", MsgMessage.ChatMode.System);
#endif

                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1498, 1);
                                if (killer.Player.QuestGUI.CheckObjectives(1498, 3))
                                {
#if Arabic
                                  killer.CreateBoxDialog("Sculptor~He~has~finished~the~inscription!~Go~visit~him,~right~away.");
#else
                                    killer.CreateBoxDialog("Sculptor~He~has~finished~the~inscription!~Go~visit~him,~right~away.");

#endif

                                }
                            }
                        }
                        if (Family.ID == 76)//blade ghost L83
                        {
                            if (killer.Player.QuestGUI.CheckQuest(1803, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (Role.Core.Rate(50))
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        if (!killer.Inventory.Contain(721846, 5))
                                        {
                                            killer.Inventory.Add(stream, 721846);
                                            killer.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "relive1");
                                            killer.CreateBoxDialog("You~picked~up~a~Blade~Ghost~Fang~and~put~it~in~your~inventory.");
                                        }
                                    }
                                    else
                                    {
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                    }
                                }

                            }

                            if (killer.Player.QuestGUI.CheckQuest(1801, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (Role.Core.Rate(25))
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        if (!killer.Inventory.Contain(721843, 1))
                                        {
                                            killer.Inventory.Add(stream, 721843);
                                            killer.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "relive1");
                                            killer.CreateBoxDialog("You~obtained~the~Stone~Spell.");
                                        }
                                    }
                                    else
                                    {
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                    }
                                }

                            }
                            if (killer.Player.QuestGUI.CheckQuest(1499, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
#if Arabic
                                killer.SendSysMesage("You`ve killed a Level 83 Blade Ghost.", MsgMessage.ChatMode.System);
#else
                                killer.SendSysMesage("You`ve killed a Level 83 Blade Ghost.", MsgMessage.ChatMode.System);
#endif

                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1499, 1);
                                if (killer.Player.QuestGUI.CheckObjectives(1499, 30))
                                {
#if Arabic
                                  killer.CreateBoxDialog("You`ve~killed~30~Level~83~Blade~Ghosts.~Go~report~to~Spring~General~Xu.");
#else
                                    killer.CreateBoxDialog("You`ve~killed~30~Level~83~Blade~Ghosts.~Go~report~to~Spring~General~Xu.");

#endif

                                }
                            }
                            if (killer.Player.QuestGUI.CheckQuest(1506, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (Role.Core.Rate(25))
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        killer.Inventory.Add(stream, 720968);
                                        killer.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "relive1");
                                        killer.CreateBoxDialog("You`ve~killed~the~Level~83~Blade~Ghost~and~obtained~an~Antidote.");
                                    }
                                    else
                                    {
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                    }
                                }

                            }

                            if (killer.Player.QuestGUI.CheckQuest(1500, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (Role.Core.Rate(25))
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        killer.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "relive1");
                                        killer.Inventory.Add(stream, 711407);
                                        killer.CreateBoxDialog("You`ve~killed~a~Level~83~Blade~Ghost~and~cut~its~ear.");
                                    }
                                    else
                                    {
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                    }
                                }

                            }

                        }
                        if (Family.ID == 17)//blade ghost
                        {

                            if (killer.Player.QuestGUI.CheckQuest(1802, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {

                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1802, 1);
                                if (killer.Player.QuestGUI.CheckObjectives(1802, 200))
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        killer.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "relive1");
                                        killer.Inventory.Add(stream, 721845);
                                        killer.CreateBoxDialog("You`ve~killed~200~Blade~Ghosts~and~found~a~Blunt~Sword!");
                                    }
                                    else
                                    {
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                    }
                                }
                            }
                            if (killer.Player.QuestGUI.CheckQuest(1501, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (Role.Core.Rate(40))
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        killer.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "relive1");
                                        killer.Inventory.Add(stream, 711413);
                                        killer.CreateBoxDialog("You`ve~killed~the~Blade~Ghost~and~cut~its~ear.");
                                    }
                                    else
                                    {
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                    }
                                }

                            }
                            if (killer.Player.QuestGUI.CheckQuest(1508, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (Role.Core.Rate(25))
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        killer.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "relive1");
                                        killer.Inventory.Add(stream, 711414);
                                        killer.CreateBoxDialog("You`ve~killed~the~Blade~Ghost~and~obtained~a~Straight~Hook.~Deliver~it~to~Elder~Jiang.");
                                    }
                                    else
                                    {
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                    }
                                }

                            }


                            if (killer.Player.QuestGUI.CheckQuest(1494, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (killer.Inventory.Contain(711406, 5))
                                {
                                    killer.SendSysMesage("You`ve gathered 5 Sharp Blades. Hurry to deliver them to Spring Vice General Ou!", MsgMessage.ChatMode.System);
                                }
                                if (Role.Core.Rate(25))
                                {
#if Arabic
                                    killer.CreateBoxDialog("You`ve~killed~the~Blade~Ghost~and~obtained~a~Sharp~Blade.");
#else
                                    killer.CreateBoxDialog("You`ve~killed~the~Blade~Ghost~and~obtained~a~Sharp~Blade.");
#endif
                                    killer.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "relive1");
                                    MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                                    DataItem.ITEM_ID = 711406;
                                    Database.ItemType.DBItem DBItem;
                                    if (Database.Server.ItemsBase.TryGetValue(711406, out DBItem))
                                    {
                                        DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                                    }
                                    DataItem.Color = Role.Flags.Color.Red;
                                    ushort xx = X;
                                    ushort yy = Y;
                                    if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                                    {
                                        MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                                        if (killer.Map.EnqueueItem(DropItem))
                                        {
                                            DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                        }
                                    }
                                }

                            }

                            if (killer.Player.QuestGUI.CheckQuest(1497, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (Role.Core.Rate(25))
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        killer.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "relive1");
                                        killer.Inventory.Add(stream, 711409);
                                        killer.CreateBoxDialog("You`ve~killed~the~Blade~Ghost~and~found~a~Sharp~Axe.~Deliver~it~to~Sculptor~He.");
                                    }
                                    else
                                    {
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                    }
                                }

                            }

                            if (killer.Player.QuestGUI.CheckQuest(1493, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
#if Arabic
                                killer.SendSysMesage("You`ve~killed~the~Blade~Ghost.", MsgMessage.ChatMode.System);
#else
                                killer.SendSysMesage("You`ve~killed~the~Blade~Ghost.", MsgMessage.ChatMode.System);
#endif

                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1493, 1);
                                if (killer.Player.QuestGUI.CheckObjectives(1493, 50))
                                {
#if Arabic
                                  killer.CreateBoxDialog("You`ve~killed~enough~Blade~Ghosts.~Go~report~to~Spring~General~Xu.");
#else
                                    killer.CreateBoxDialog("You`ve~killed~enough~Blade~Ghosts.~Go~report~to~Spring~General~Xu.");

#endif

                                }
                            }

                        }
                        if (Family.ID == 75)
                        {
                            if (killer.Player.QuestGUI.CheckQuest(1797, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (Role.Core.Rate(45))
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        byte rand = (byte)Program.GetRandom.Next(0, 3);
                                        switch (rand)
                                        {
                                            case 0:
                                                {
                                                    killer.Inventory.AddItemWitchStack(721952, 0, 1, stream);
                                                    killer.SendSysMesage("You Maple Stone came out from the Rock Monster`s body. Pick it up.", MsgMessage.ChatMode.System);
                                                    break;
                                                }
                                            case 1:
                                                {
                                                    killer.Inventory.AddItemWitchStack(721953, 0, 1, stream);
                                                    killer.SendSysMesage("A Ocean Stone came out from the Rock Monster`s body. Pick it up.", MsgMessage.ChatMode.System);
                                                    break;
                                                }
                                            default:
                                                {
                                                    killer.Inventory.AddItemWitchStack(721951, 0, 1, stream);
                                                    killer.SendSysMesage("A Leaf Stone came out from the Rock Monster`s body. Pick it up.", MsgMessage.ChatMode.System);
                                                    break;
                                                }
                                        }
                                    }
                                    else
                                    {
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                    }
                                }
                            }
                            if (killer.Player.QuestGUI.CheckQuest(1796, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (Role.Core.Rate(20))
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        if (!killer.Inventory.Contain(710581, 10))
                                        {
                                            killer.Inventory.AddItemWitchStack(710581, 0, 1, stream);
                                            killer.SendSysMesage("A Marble came out from the Rock Monster`s body. Check your inventory.", MsgMessage.ChatMode.System);
                                        }
                                        else
                                            killer.SendSysMesage("You've collected 10 Marbles. Take them to Farmer Lynn!", MsgMessage.ChatMode.System);
                                    }
                                    else
                                    {
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                    }
                                }

                            }


                            if (killer.Player.QuestGUI.CheckQuest(1485, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (Role.Core.Rate(20))
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        killer.Inventory.Add(stream, 711402);
                                        killer.CreateBoxDialog("Deliver~the~Monster~Tail~to~Convoy~Vice~Leader~Ling.");
                                    }
                                    else
                                    {
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                    }
                                }

                            }
                            if (killer.Player.QuestGUI.CheckQuest(1491, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (Role.Core.Rate(20))
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        killer.Inventory.Add(stream, 711405);
                                        killer.CreateBoxDialog("You`ve~killed~the~Level~78~Rock~Monster~and~obtained~a~Thunder~Stone.~Deliver~it~to~Ironsmith~Li.");
                                    }
                                    else
                                    {
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                    }
                                }

                            }
                            if (killer.Player.QuestGUI.CheckQuest(1484, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {

                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1484, 1);
                                if (killer.Player.QuestGUI.CheckObjectives(1484, 30))
                                {
#if Arabic
                                    killer.CreateBoxDialog("You`ve~killed~30~Level~78~Rock~Monsters.~Go~report~to~Convoy~Leader~Gu.");
#else
                                    killer.CreateBoxDialog("You`ve~killed~30~Level~78~Rock~Monsters.~Go~report~to~Convoy~Leader~Gu.");
#endif

                                }
                            }
                        }

                        if (Family.ID == 51)
                        {
                            if (killer.Player.QuestGUI.CheckQuest(1479, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
#if Arabic
                                killer.SendSysMesage("You`ve~killed~the~Rock~Monster.", MsgMessage.ChatMode.System);
#else
                                killer.SendSysMesage("You`ve~killed~the~Rock~Monster.", MsgMessage.ChatMode.System);
#endif

                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1479, 1);
                                if (killer.Player.QuestGUI.CheckObjectives(1479, 50))
                                {
#if Arabic
                                    killer.CreateBoxDialog("You`ve~killed~50~Rock~Monsters.~Go~report~to~General~ZhuGe.");
#else
                                    killer.CreateBoxDialog("You`ve~killed~50~Rock~Monsters.~Go~report~to~General~ZhuGe.");
#endif

                                }
                            }
                            if (killer.Player.QuestGUI.CheckQuest(1501, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (Role.Core.Rate(40))
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        killer.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "relive1");
                                        killer.Inventory.Add(stream, 711412);
                                        killer.CreateBoxDialog("You`ve~killed~the~Rock~Monster~and~obtained~Rock~Monster`s~Tail.");
                                    }
                                    else
                                    {
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                    }
                                }

                            }
                            if (killer.Player.QuestGUI.CheckQuest(1480, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (Role.Core.Rate(20))
                                {
#if Arabic
                                killer.CreateBoxDialog("You`ve~killed~the~Rock~Monster~and~obtained~a~Colorful~Stone.");
#else
                                    killer.CreateBoxDialog("You`ve~killed~the~Rock~Monster~and~obtained~a~Colorful~Stone.");
#endif
                                    MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                                    DataItem.ITEM_ID = 711349;
                                    Database.ItemType.DBItem DBItem;
                                    if (Database.Server.ItemsBase.TryGetValue(711349, out DBItem))
                                    {
                                        DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                                    }
                                    DataItem.Color = Role.Flags.Color.Red;
                                    ushort xx = X;
                                    ushort yy = Y;
                                    if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                                    {
                                        MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                                        if (killer.Map.EnqueueItem(DropItem))
                                        {
                                            DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                        }
                                    }
                                }
                            }

                            if (killer.Player.QuestGUI.CheckQuest(1481, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (Role.Core.Rate(30))
                                {
#if Arabic
                                killer.CreateBoxDialog("You`ve~killed~the~Rock~Monster~and~obtained~a~Grindstone.");
#else
                                    killer.CreateBoxDialog("You`ve~killed~the~Rock~Monster~and~obtained~a~Grindstone.");
#endif
                                    MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                                    DataItem.ITEM_ID = 711399;
                                    Database.ItemType.DBItem DBItem;
                                    if (Database.Server.ItemsBase.TryGetValue(711399, out DBItem))
                                    {
                                        DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                                    }
                                    DataItem.Color = Role.Flags.Color.Red;
                                    ushort xx = X;
                                    ushort yy = Y;
                                    if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                                    {
                                        MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                                        if (killer.Map.EnqueueItem(DropItem))
                                        {
                                            DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                        }
                                    }
                                }
                            }
                            if (killer.Player.QuestGUI.CheckQuest(1478, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (Role.Core.Rate(45))
                                {
                                    killer.CreateBoxDialog("You`ve~killed~the~Rock~Monster~and~obtained~a~slabstone.");
                                    killer.Inventory.AddItemWitchStack(720970, 0, 1, stream);
                                }
                            }
                        }
                        else if (Family.ID == 74)
                        {
                            if (Role.Core.Rate(5))
                            {
                                if (!killer.Player.QuestGUI.CheckQuest(1477, MsgQuestList.QuestListItem.QuestStatus.Finished))
                                {
#if Arabic
                                killer.CreateBoxDialog("You`ve~killed~the~Hill~Monster~and~received~a~Secret~Letter.");
#else
                                    killer.CreateBoxDialog("You`ve~killed~the~Hill~Monster~and~received~a~Secret~Letter.");
#endif
                                    MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                                    DataItem.ITEM_ID = 720975;
                                    Database.ItemType.DBItem DBItem;
                                    if (Database.Server.ItemsBase.TryGetValue(720975, out DBItem))
                                    {
                                        DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                                    }
                                    DataItem.Color = Role.Flags.Color.Red;
                                    ushort xx = X;
                                    ushort yy = Y;
                                    if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                                    {
                                        MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                                        if (killer.Map.EnqueueItem(DropItem))
                                        {
                                            DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                        }
                                    }
                                }
                            }
                            if (killer.Player.QuestGUI.CheckQuest(1471, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
#if Arabic
                                killer.SendSysMesage("You`ve~killed~a~Level~73~Hill~Monster.", MsgMessage.ChatMode.System);
#else
                                killer.SendSysMesage("You`ve~killed~a~Level~73~Hill~Monster.", MsgMessage.ChatMode.System);
#endif

                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1471, 1);
                                if (killer.Player.QuestGUI.CheckObjectives(1471, 30))
                                {
#if Arabic
                                        killer.CreateBoxDialog("You`ve~killed~30~Hill~Monsters~L73.~Go~report~to~Han~Cheng.");
");
#else
                                    killer.CreateBoxDialog("You`ve~killed~30~Hill~Monsters~L73.~Go~report~to~Han~Cheng.");
#endif

                                }
                            }
                            if (killer.Player.QuestGUI.CheckQuest(1476, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
#if Arabic
                                killer.SendSysMesage("You`ve~killed~a~Level~73~Hill~Monster.", MsgMessage.ChatMode.System);
#else
                                killer.SendSysMesage("You`ve~killed~a~Level~73~Hill~Monster.", MsgMessage.ChatMode.System);
#endif

                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1476, 0, 1);
                                if (killer.Player.QuestGUI.CheckObjectives(1476, 40, 10))
                                {
#if Arabic
                                    killer.CreateBoxDialog("You`ve~killed~40~Hill~Monsters~and~10~Hill~Monsters~L73.~Go~report~to~Han~Cheng.
");
#else
                                    killer.CreateBoxDialog("You`ve~killed~40~Hill~Monsters~and~10~Hill~Monsters~L73.~Go~report~to~Han~Cheng.");
#endif

                                }
                            }
                            if (killer.Player.QuestGUI.CheckQuest(1473, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (Role.Core.Rate(20))
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        killer.CreateBoxDialog("You`ve~killed~a~Level~73~Hill~Monster~and~obtained~an~armor.");
                                        killer.Inventory.Add(stream, 711416);
                                    }
                                    else
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                }
                            }
                            if (killer.Player.QuestGUI.CheckQuest(1472, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
#if Arabic
                                killer.SendSysMesage("You`ve~killed~a~Level~73~Hill~Monster.", MsgMessage.ChatMode.System);
#else
                                killer.SendSysMesage("You`ve~killed~a~Level~73~Hill~Monster.", MsgMessage.ChatMode.System);
#endif

                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1472, 1);
                                if (killer.Player.QuestGUI.CheckObjectives(1472, 20))
                                {
#if Arabic
                                    killer.CreateBoxDialog("You`ve~killed~20~Level~73~Hill~Monsters.~Go~report~to~Ironsmith~Li.");
#else
                                    killer.CreateBoxDialog("You`ve~killed~20~Level~73~Hill~Monsters.~Go~report~to~Ironsmith~Li.");
#endif

                                }
                            }

                        }
                        else if (Family.ID == 14)
                        {
                            if (killer.Player.QuestGUI.CheckQuest(1458, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (Role.Core.Rate(30))
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        killer.Inventory.Add(stream, 711341);
                                        killer.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "relive1");
                                        killer.CreateBoxDialog("You`ve~killed~the~Sand~Monster~and~received~a~Sandbag.");
                                    }
                                    else
                                    {
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                    }
                                }

                            }


                            if (killer.Player.QuestGUI.CheckQuest(1501, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (Role.Core.Rate(40))
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        killer.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "relive1");
                                        killer.Inventory.Add(stream, 711410);
                                        killer.CreateBoxDialog("You`ve~killed~the~Sand~Monster~and~received~a~Sand~Pill.");
                                    }
                                    else
                                    {
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                    }
                                }

                            }

                            if (killer.Player.QuestGUI.CheckQuest(1464, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
#if Arabic
                                     killer.SendSysMesage("You've~killed~a~Sand~Monster.", MsgMessage.ChatMode.System);
#else
                                killer.SendSysMesage("You've~killed~a~Sand~Monster.", MsgMessage.ChatMode.System);
#endif

                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1464, 1);
                                if (killer.Player.QuestGUI.CheckObjectives(1464, 40))
                                {
#if Arabic
                                                                       killer.CreateBoxDialog("You`ve~killed~40~Sand~Monsters.~After~you~kill~another~10~Level~68~Sand~Monsters,~you~can~go~report~to~Ke~Yulun.");
#else
                                    killer.CreateBoxDialog("You`ve~killed~40~Sand~Monsters.~After~you~kill~another~10~Level~68~Sand~Monsters,~you~can~go~report~to~Ke~Yulun.");
#endif

                                }
                            }
                            if (killer.Player.QuestGUI.CheckQuest(1451, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
#if Arabic
                                     killer.SendSysMesage("You've~killed~a~Sand~Monster.", MsgMessage.ChatMode.System);
#else
                                killer.SendSysMesage("You've~killed~a~Sand~Monster.", MsgMessage.ChatMode.System);
#endif

                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1451, 1);
                                if (killer.Player.QuestGUI.CheckObjectives(1451, 50))
                                {
#if Arabic
                                     killer.CreateBoxDialog("You`ve~killed~50~Sand~Monsters.~Go~report~to~the~DC~Vice~Captain.");
#else
                                    killer.CreateBoxDialog("You`ve~killed~50~Sand~Monsters.~Go~report~to~the~DC~Vice~Captain.");
#endif

                                }
                            }
                            if (killer.Player.QuestGUI.CheckQuest(1453, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (Role.Core.Rate(20))
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        killer.Inventory.Add(stream, 711338);
                                    }
                                    else
                                    {
#if Arabic
                                         killer.CreateBoxDialog("Please make 1 more space in your inventory.");
#else
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
#endif

                                    }
                                }
                            }
                        }
                        else if (Family.ID == 15)
                        {
                            if (killer.Player.QuestGUI.CheckQuest(1501, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (Role.Core.Rate(40))
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        killer.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "relive1");
                                        killer.Inventory.Add(stream, 711411);
                                        killer.CreateBoxDialog("You`ve~killed~the~Hill~Monster~and~received~a~Hill~Monster`s~Heart.");
                                    }
                                    else
                                    {
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                    }
                                }

                            }
                            if (killer.Player.QuestGUI.CheckQuest(1476, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
#if Arabic
                                killer.SendSysMesage("You've killed a Hill Monster.", MsgMessage.ChatMode.System);
#else
                                killer.SendSysMesage("You've killed a Hill Monster.", MsgMessage.ChatMode.System);
#endif
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1476, 1);
                                if (killer.Player.QuestGUI.CheckObjectives(1476, 40, 10))
                                {
#if Arabic
                                     killer.CreateBoxDialog("You`ve~killed~40~Hill~Monsters~and~10~Hill~Monsters~L73.~Go~report~to~Han~Cheng.");
#else
                                    killer.CreateBoxDialog("You`ve~killed~40~Hill~Monsters~and~10~Hill~Monsters~L73.~Go~report~to~Han~Cheng.");
#endif

                                }

                            }
                            if (killer.Player.QuestGUI.CheckQuest(1466, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
#if Arabic
                                killer.SendSysMesage("You've killed a Hill Monster.", MsgMessage.ChatMode.System);
#else
                                killer.SendSysMesage("You've killed a Hill Monster.", MsgMessage.ChatMode.System);
#endif
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1466, 1);
                                if (killer.Player.QuestGUI.CheckObjectives(1466, 50))
                                {
#if Arabic
                                     killer.CreateBoxDialog("You`ve~killed~50~Hill~Monsters.~Go~report~to~General~ZhuGe.");
#else
                                    killer.CreateBoxDialog("You`ve~killed~50~Hill~Monsters.~Go~report~to~General~ZhuGe.");
#endif

                                }

                            }
                            if (killer.Player.QuestGUI.CheckQuest(1467, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (Role.Core.Rate(30))
                                {
#if Arabic
                                killer.CreateBoxDialog("You`ve~killed~the~Hill~Monster~and~received~a~Hammer.");
#else
                                    killer.CreateBoxDialog("You`ve~killed~the~Hill~Monster~and~received~a~Hammer.");
#endif
                                    MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                                    DataItem.ITEM_ID = 711344;
                                    Database.ItemType.DBItem DBItem;
                                    if (Database.Server.ItemsBase.TryGetValue(711344, out DBItem))
                                    {
                                        DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                                    }
                                    DataItem.Color = Role.Flags.Color.Red;
                                    ushort xx = X;
                                    ushort yy = Y;
                                    if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                                    {
                                        MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                                        if (killer.Map.EnqueueItem(DropItem))
                                        {
                                            DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                        }
                                    }
                                }
                            }

                            if (killer.Player.QuestGUI.CheckQuest(1474, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (Role.Core.Rate(30))
                                {
#if Arabic
                                killer.CreateBoxDialog("You`ve~killed~the~monster~and~received~a~Heavy~Axe.");
#else
                                    killer.CreateBoxDialog("You`ve~killed~the~monster~and~received~a~Heavy~Axe.");
#endif
                                    MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                                    DataItem.ITEM_ID = 711415;
                                    Database.ItemType.DBItem DBItem;
                                    if (Database.Server.ItemsBase.TryGetValue(711415, out DBItem))
                                    {
                                        DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                                    }
                                    DataItem.Color = Role.Flags.Color.Red;
                                    ushort xx = X;
                                    ushort yy = Y;
                                    if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                                    {
                                        MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                                        if (killer.Map.EnqueueItem(DropItem))
                                        {
                                            DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                        }
                                    }
                                }
                            }

                            if (killer.Player.QuestGUI.CheckQuest(1465, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1465, 1);
                                if (killer.Player.QuestGUI.CheckObjectives(1465, 20))
                                {
#if Arabic
                                     killer.CreateBoxDialog("You`ve~killed~20~Hill~Monsters.~Go~report~to~General~ZhuGe.");
#else
                                    killer.CreateBoxDialog("You`ve~killed~20~Hill~Monsters.~Go~report~to~General~ZhuGe.");
#endif

                                }
                            }
                        }
                        else if (Family.ID == 73)
                        {

                            if (killer.Player.QuestGUI.CheckQuest(1464, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
#if Arabic
                                killer.SendSysMesage("You've killed a Level 68 Sand Monster.", MsgMessage.ChatMode.System);
#else
                                killer.SendSysMesage("You've killed a Level 68 Sand Monster.", MsgMessage.ChatMode.System);
#endif

                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1464, 0, 1);
                                if (killer.Player.QuestGUI.CheckObjectives(1464, 0, 10))
                                {
#if Arabic
                                      killer.CreateBoxDialog("You`ve~killed~40~Sand~Monsters~and~10~Level~68~Sand~Monsters.~Go~report~to~KeYulun.");
#else
                                    killer.CreateBoxDialog("You`ve~killed~40~Sand~Monsters~and~10~Level~68~Sand~Monsters.~Go~report~to~KeYulun.");
#endif
                                }
                            }
                            if (killer.Player.QuestGUI.CheckQuest(1463, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
#if Arabic
                                killer.SendSysMesage("You've killed a Level 68 Sand Monster.", MsgMessage.ChatMode.System);
#else
                                killer.SendSysMesage("You've killed a Level 68 Sand Monster.", MsgMessage.ChatMode.System);
#endif

                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1463, 1);
                            }
                            if (killer.Player.QuestGUI.CheckQuest(1460, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
#if Arabic
                                  killer.SendSysMesage("You've killed a Level 68 Sand Monster.", MsgMessage.ChatMode.System);
#else
                                killer.SendSysMesage("You've killed a Level 68 Sand Monster.", MsgMessage.ChatMode.System);
#endif

                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1460, 1);
                                if (killer.Player.QuestGUI.CheckObjectives(1460, 30))
                                {
#if Arabic
                                      killer.CreateBoxDialog("You`ve~killed~30~Level~68~Sand~Monsters.~Go~report~to~the~DC~Vice~Captain.");
#else
                                    killer.CreateBoxDialog("You`ve~killed~30~Level~68~Sand~Monsters.~Go~report~to~the~DC~Vice~Captain.");
#endif

                                }
                            }
                            if (killer.Player.QuestGUI.CheckQuest(1461, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (Role.Core.Rate(20))
                                {
#if Arabic
                                       killer.CreateBoxDialog("You`ve~killed~the~Level~68~Sand~Monster~and~received~a~Sand~Essence.");
#else
                                    killer.CreateBoxDialog("You`ve~killed~the~Level~68~Sand~Monster~and~received~a~Sand~Essence.");
#endif

                                    MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                                    DataItem.ITEM_ID = 711343;
                                    Database.ItemType.DBItem DBItem;
                                    if (Database.Server.ItemsBase.TryGetValue(711343, out DBItem))
                                    {
                                        DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                                    }
                                    DataItem.Color = Role.Flags.Color.Red;
                                    ushort xx = X;
                                    ushort yy = Y;
                                    if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                                    {
                                        MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                                        if (killer.Map.EnqueueItem(DropItem))
                                        {
                                            DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                        }
                                    }
                                }

                            }
                        }
                    }
                    else if (killer.Player.Map == 1020)
                    {
                        if (Family.ID == 14337)
                        {
                            if (killer.Player.QuestGUI.CheckQuest(1347, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1347, 1);


                            }
                        }
                        if (Family.ID == 14338)
                        {
                            if (killer.Player.QuestGUI.CheckQuest(1347, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1347, 0, 1);


                            }
                        }
                        if (Family.ID == 3050)
                        {
                            if (killer.Player.QuestGUI.CheckQuest(1346, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1346, 0, 1);
                                if (killer.Player.QuestGUI.CheckObjectives(1346, 0, 40))
                                {
#if Arabic
                                     killer.SendSysMesage("You killed 40 Heresy Snakeman.", MsgMessage.ChatMode.System);
#else
                                    killer.SendSysMesage("You killed 40 Heresy Snakeman.", MsgMessage.ChatMode.System);
#endif

                                }
                            }
                            if (killer.Player.QuestGUI.CheckQuest(1345, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1345, 0, 1);
                                if (killer.Player.QuestGUI.CheckObjectives(1345, 0, 20))
                                {
#if Arabic
                                      killer.SendSysMesage("You killed 20 Heresy Snakeman.", MsgMessage.ChatMode.System);
#else
                                    killer.SendSysMesage("You killed 20 Heresy Snakeman.", MsgMessage.ChatMode.System);
#endif

                                }
                            }
                            if (killer.Player.QuestGUI.CheckQuest(1359, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1359, 1);
                                if (killer.Player.QuestGUI.CheckObjectives(1359, 50))
                                {
#if Arabic
                                     killer.SendSysMesage("You killed 50 Heresy Snakeman.");
#else
                                    killer.SendSysMesage("You killed 50 Heresy Snakeman.");
#endif
                                }

                            }
                        }
                        if (Family.ID == 72)//Snakeman lev 63
                        {

                            if (killer.Player.QuestGUI.CheckQuest(1341, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (Role.Core.Rate(20))
                                {
                                    MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                                    DataItem.ITEM_ID = 720858;
                                    Database.ItemType.DBItem DBItem;
                                    if (Database.Server.ItemsBase.TryGetValue(720858, out DBItem))
                                    {
                                        DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                                    }
                                    DataItem.Color = Role.Flags.Color.Red;
                                    ushort xx = X;
                                    ushort yy = Y;
                                    if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                                    {
                                        MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                                        if (killer.Map.EnqueueItem(DropItem))
                                        {
                                            DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                        }
                                    }
                                }
                            }
                            if (killer.Player.QuestGUI.CheckQuest(1343, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1343, 1);
                                if (killer.Player.QuestGUI.CheckObjectives(1343, 50))
                                {
#if Arabic
                                     killer.SendSysMesage("You killed 50 Snakemen L63.");
#else
                                    killer.SendSysMesage("You killed 50 Snakemen L63.");
#endif

                                }
                            }
                            if (killer.Player.QuestGUI.CheckQuest(1334, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (Role.Core.Rate(20))
                                {
                                    MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                                    DataItem.ITEM_ID = 720855;
                                    Database.ItemType.DBItem DBItem;
                                    if (Database.Server.ItemsBase.TryGetValue(720855, out DBItem))
                                    {
                                        DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                                    }
                                    DataItem.Color = Role.Flags.Color.Red;
                                    ushort xx = X;
                                    ushort yy = Y;
                                    if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                                    {
                                        MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                                        if (killer.Map.EnqueueItem(DropItem))
                                        {
                                            DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                        }
                                    }
                                }

                            }
                        }
                        if (Family.ID == 13)//Snakeman
                        {

                            if (killer.Player.QuestGUI.CheckQuest(1346, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1346, 1);
                                if (killer.Player.QuestGUI.CheckObjectives(1346, 40))
                                {
#if Arabic
                                       killer.SendSysMesage("You killed 40 Snakeman.", MsgMessage.ChatMode.System);
#else
                                    killer.SendSysMesage("You killed 40 Snakeman.", MsgMessage.ChatMode.System);
#endif

                                }
                            }
                            if (killer.Player.QuestGUI.CheckQuest(1345, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1345, 1);
                                if (killer.Player.QuestGUI.CheckObjectives(1345, 20))
                                {
#if Arabic
                                      killer.SendSysMesage("You killed 20 Snakeman.", MsgMessage.ChatMode.System);
#else
                                    killer.SendSysMesage("You killed 20 Snakeman.", MsgMessage.ChatMode.System);
#endif

                                }
                            }

                            if (killer.Player.QuestGUI.CheckQuest(1360, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1360, 1);
                                if (killer.Player.QuestGUI.CheckObjectives(1360, 60))
                                {
#if Arabic
                                      killer.SendSysMesage("You killed 60 Snakeman.", MsgMessage.ChatMode.System);
#else
                                    killer.SendSysMesage("You killed 60 Snakeman.", MsgMessage.ChatMode.System);
#endif

                                }
                            }
                            if (killer.Player.QuestGUI.CheckQuest(1341, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (Role.Core.Rate(20))
                                {
                                    MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                                    DataItem.ITEM_ID = 720857;
                                    Database.ItemType.DBItem DBItem;
                                    if (Database.Server.ItemsBase.TryGetValue(720857, out DBItem))
                                    {
                                        DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                                    }
                                    DataItem.Color = Role.Flags.Color.Red;
                                    ushort xx = X;
                                    ushort yy = Y;
                                    if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                                    {
                                        MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                                        if (killer.Map.EnqueueItem(DropItem))
                                        {
                                            DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                        }
                                    }
                                }
                            }
                            if (killer.Player.QuestGUI.CheckQuest(1337, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (killer.Inventory.HaveSpace(1))
                                {
#if Arabic
                                    killer.CreateBoxDialog("You~received~the~Cloud~Riding~Scroll!");
#else
                                    killer.CreateBoxDialog("You~received~the~Cloud~Riding~Scroll!");
#endif

                                    if (killer.Inventory.Contain(720856, 1))
                                        killer.Inventory.Add(stream, 720856);
                                }
                                else
                                {
#if Arabic
                                     killer.SendSysMesage("Please make 1 more space in your inventory.");
#else
                                    killer.SendSysMesage("Please make 1 more space in your inventory.");
#endif

                                }
                            }

                            if (killer.Player.QuestGUI.CheckQuest(1335, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1335, 1);
                            }
                            if (killer.Player.QuestGUI.CheckQuest(1333, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (Role.Core.Rate(20))
                                {
                                    MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                                    DataItem.ITEM_ID = 720854;
                                    Database.ItemType.DBItem DBItem;
                                    if (Database.Server.ItemsBase.TryGetValue(720854, out DBItem))
                                    {
                                        DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                                    }
                                    DataItem.Color = Role.Flags.Color.Red;
                                    ushort xx = X;
                                    ushort yy = Y;
                                    if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                                    {
                                        MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                                        if (killer.Map.EnqueueItem(DropItem))
                                        {
                                            DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                        }
                                    }
                                }

                            }
                        }
                        if (Family.ID == 71)
                        {
                            if (killer.Player.QuestGUI.CheckQuest(1325, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1325, 1);
                                if (killer.Player.QuestGUI.CheckObjectives(1325, 50))
                                {
#if Arabic
                                      killer.SendSysMesage("You killed 50 Thunder Apes L58.", MsgMessage.ChatMode.System);
#else
                                    killer.SendSysMesage("You killed 50 Thunder Apes L58.", MsgMessage.ChatMode.System);
#endif

                                }
                            }
                            if (killer.Player.QuestGUI.CheckQuest(1328, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (Role.Core.Rate(20))
                                {
                                    MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                                    DataItem.ITEM_ID = 720850;
                                    Database.ItemType.DBItem DBItem;
                                    if (Database.Server.ItemsBase.TryGetValue(720850, out DBItem))
                                    {
                                        DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                                    }
                                    DataItem.Color = Role.Flags.Color.Red;
                                    ushort xx = X;
                                    ushort yy = Y;
                                    if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                                    {
                                        MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                                        if (killer.Map.EnqueueItem(DropItem))
                                        {
#if Arabic
                                              killer.SendSysMesage("A strange item dropped on the ground! Pick it up to see that is! ", MsgMessage.ChatMode.System);
#else
                                            killer.SendSysMesage("A strange item dropped on the ground! Pick it up to see that is! ", MsgMessage.ChatMode.System);
#endif

                                            DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                        }
                                    }
                                }

                            }
                        }
                        if (Family.ID == 12)// thunder
                        {
                            if (killer.Player.QuestGUI.CheckQuest(1327, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (Role.Core.Rate(20))
                                {
                                    MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                                    DataItem.ITEM_ID = 711353;
                                    Database.ItemType.DBItem DBItem;
                                    if (Database.Server.ItemsBase.TryGetValue(711353, out DBItem))
                                    {
                                        DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                                    }
                                    DataItem.Color = Role.Flags.Color.Red;
                                    ushort xx = X;
                                    ushort yy = Y;
                                    if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                                    {
                                        MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                                        if (killer.Map.EnqueueItem(DropItem))
                                        {
#if Arabic
                                            killer.SendSysMesage("A strange item dropped on the ground! Pick it up to see that is! ", MsgMessage.ChatMode.System);
#else
                                            killer.SendSysMesage("A strange item dropped on the ground! Pick it up to see that is! ", MsgMessage.ChatMode.System);
#endif

                                            DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                        }
                                    }
                                }
                            }
                            if (killer.Player.QuestGUI.CheckQuest(1324, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1324, 1);
                                if (killer.Player.QuestGUI.CheckObjectives(1324, 60))
                                {
#if Arabic
                                     killer.SendSysMesage("You killed 60 Thunder Apes.", MsgMessage.ChatMode.System);
#else
                                    killer.SendSysMesage("You killed 60 Thunder Apes.", MsgMessage.ChatMode.System);
#endif

                                }
                            }
                            if (killer.Player.QuestGUI.CheckQuest(1329, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (Role.Core.Rate(25))
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1329, 1);
                                        killer.Inventory.Add(stream, 720851);
#if Arabic
                                         killer.CreateBoxDialog("You received the Blaze Pill!");
#else
                                        killer.CreateBoxDialog("You received the Blaze Pill!");
#endif

                                    }
                                    else
                                    {
#if Arabic
                                            killer.CreateBoxDialog("Please make 1 more space in your inventory.");
#else
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
#endif

                                    }
                                }

                            }
                        }
                        if (Family.ID == 14336)
                        {
                            if (killer.Player.QuestGUI.CheckQuest(1356, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1356, 1);

                            }
                        }
                        if (Family.ID == 1)
                        {
                            if (killer.Player.QuestGUI.CheckQuest(1307, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1307, 1);
#if Arabic
                                killer.CreateBoxDialog("You~killed~Pheasant~and~scared~the~Apes~away!");
#else
                                killer.CreateBoxDialog("You~killed~Pheasant~and~scared~the~Apes~away!");
#endif

                            }
                            killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 824, 1);
                        }
                        if (Family.ID == 678000)
                        {
                            if (killer.Player.QuestGUI.CheckQuest(1351, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1351, 1);
                            }
                        }
                        if (Family.ID == 11)//giant ape
                        {
                            if (killer.Player.QuestGUI.CheckQuest(1363, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (Role.Core.Rate(30))
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {


                                        if (killer.Player.QuestGUI.CheckObjectives(1363, 1) == false)
                                        {
#if Arabic
                                             killer.SendSysMesage("You received 1 Ape Claw.", MsgMessage.ChatMode.System);
#else
                                            killer.SendSysMesage("You received 1 Ape Claw.", MsgMessage.ChatMode.System);
#endif

                                            killer.Inventory.Add(stream, 711357, 1);
                                            killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1363, 1);
                                        }
                                        else if (killer.Player.QuestGUI.CheckObjectives(1363, 0, 1) == false)
                                        {
#if Arabic
                                              killer.SendSysMesage("You received 1 Ape Bone.", MsgMessage.ChatMode.System);
#else
                                            killer.SendSysMesage("You received 1 Ape Bone.", MsgMessage.ChatMode.System);
#endif

                                            killer.Inventory.Add(stream, 711358, 1);
                                            killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1363, 0, 1);
                                        }
                                        else if (killer.Player.QuestGUI.CheckObjectives(1363, 0, 0, 1) == false)
                                        {
#if Arabic
                                             killer.SendSysMesage("You received 1 Ape Skin.", MsgMessage.ChatMode.System);
#else
                                            killer.SendSysMesage("You received 1 Ape Skin.", MsgMessage.ChatMode.System);
#endif

                                            killer.Inventory.Add(stream, 711359, 1);
                                            killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1363, 0, 0, 1);
                                        }
                                    }
                                    else
                                    {
#if Arabic
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
#else
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
#endif

                                    }
                                }
                            }
                            if (killer.Player.QuestGUI.CheckQuest(1316, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1316, 1);
                                if (killer.Player.QuestGUI.CheckObjectives(1316, 50))
                                {
#if Arabic
                                    killer.SendSysMesage("You killed 50 Giant Apes.");
#else
                                    killer.SendSysMesage("You killed 50 Giant Apes.");
#endif

                                }
                            }
                        }
                        if (Family.ID == 33212)
                        {
                            if (killer.Player.QuestGUI.CheckQuest(1756, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.Inventory.Add(stream, 721706);
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1756, 1);
                                var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)Game.MsgNpc.NpcID.GuardLi, killer.Player.Class, 1756);
#if Arabic
 killer.Player.QuestGUI.SendAutoPatcher("You~killed~the~monster~and~received~a~Paper~Figure!~Take~it~to~General~Zhao!", ActiveQuest.FinishNpcId.Map, ActiveQuest.FinishNpcId.X, ActiveQuest.FinishNpcId.Y, ActiveQuest.FinishNpcId.ID);

#else
                                killer.Player.QuestGUI.SendAutoPatcher("You~killed~the~monster~and~received~a~Paper~Figure!~Take~it~to~General~Zhao!", ActiveQuest.FinishNpcId.Map, ActiveQuest.FinishNpcId.X, ActiveQuest.FinishNpcId.Y, ActiveQuest.FinishNpcId.ID);

#endif

                            }
                        }
                        if (Family.ID == 70)
                        {
                            if (killer.Player.QuestGUI.CheckQuest(1753, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1753, 0, 0, 1);
#if Arabic
                                 killer.SendSysMesage("You,ve~collected~some~smells.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
#else
                                killer.SendSysMesage("You,ve~collected~some~smells.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
#endif

                                if (killer.Player.QuestGUI.CheckObjectives(1753, 0, 0, 10))
                                {
#if Arabic
                                      killer.CreateBoxDialog("You,ve collected the smell of Gaint Ape!");
#else
                                    killer.CreateBoxDialog("You,ve collected the smell of Gaint Ape!");
#endif

                                }
                            }
                        }
                        if (Family.ID == 13)
                        {
                            if (killer.Player.QuestGUI.CheckQuest(1753, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1753, 1);
#if Arabic
                                       killer.SendSysMesage("You,ve collected the smell of Snake Man!", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                         
#else
                                killer.SendSysMesage("You,ve collected the smell of Snake Man!", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);

#endif
                            }
                            if (killer.Player.QuestGUI.CheckQuest(1783, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (Role.Core.Rate(15))
                                {
                                    killer.Inventory.Add(stream, 729978);
                                    killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1783, 1);
                                    var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)MsgNpc.NpcID.GeneralJudd, killer.Player.Class, 1783);
#if Arabic
                                          killer.Player.QuestGUI.SendAutoPatcher("You,ve collected the smell of Snake Man!.", ActiveQuest.FinishNpcId.Map, ActiveQuest.FinishNpcId.X, ActiveQuest.FinishNpcId.Y, ActiveQuest.FinishNpcId.ID);
                               
#else
                                    killer.Player.QuestGUI.SendAutoPatcher("You,ve collected the smell of Snake Man!.", ActiveQuest.FinishNpcId.Map, ActiveQuest.FinishNpcId.X, ActiveQuest.FinishNpcId.Y, ActiveQuest.FinishNpcId.ID);

#endif
                                }
                            }
                        }
                        if (Family.ID == 14334)
                        {
                            if (killer.Player.QuestGUI.CheckQuest(1749, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1749, 0, 1);
                                if (killer.Player.QuestGUI.CheckObjectives(1749, 0, killer.Player.QuestMultiple * 5) && killer.Player.QuestGUI.CheckObjectives(1749, 1) == false)
                                {
                                    killer.Player.QuestMultiple = 1;
                                    Database.Server.AddMapMonster(stream, killer.Map, 14334, killer.Player.X, killer.Player.Y, 7, 7, 5, 0, true);
                                }
                                if (Role.Core.Rate(20))
                                {
                                    if (killer.Player.QuestGUI.CheckObjectives(1749, 1) == false)
                                    {
#if Arabic
                                         killer.CreateBoxDialog("The Ape Leader has appeared! Be extra cautious!");
#else
                                        killer.CreateBoxDialog("The Ape Leader has appeared! Be extra cautious!");
#endif

                                        Database.Server.AddMapMonster(stream, killer.Map, 14335, killer.Player.X, killer.Player.Y, 7, 7, 1, 0, true, MsgItemPacket.EffectMonsters.Night);
                                    }
                                }
                            }
                        }
                        if (Family.ID == 14335)
                        {
                            Game.MsgNpc.Dialog dialog = new Game.MsgNpc.Dialog(killer, stream);
#if Arabic
                              dialog.AddText("Amazing! You now have the treasure those Venomous Apes were guarding -the Devilish Sword.")
                                .FinalizeDialog();
#else
                            dialog.AddText("Amazing! You now have the treasure those Venomous Apes were guarding -the Devilish Sword.")
                              .FinalizeDialog();
#endif

                            killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1749, 1);
                            killer.Inventory.Remove(721701, 1, stream);
                            if (!killer.Inventory.Contain(721702, 1))
                                killer.Inventory.Add(stream, 721702);
                            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)MsgNpc.NpcID.DoctorLi, killer.Player.Class, 1749);
                            killer.Player.QuestGUI.SendAutoPatcher(ActiveQuest.FinishNpcId.Map, ActiveQuest.FinishNpcId.X, ActiveQuest.FinishNpcId.Y, ActiveQuest.FinishNpcId.ID);
                        }
                        if (Family.ID == 10)//macaque
                        {

                            if (killer.Player.QuestGUI.CheckQuest(1311, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (Role.Core.Rate(30))
                                {
                                    MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                                    DataItem.ITEM_ID = 711352;
                                    Database.ItemType.DBItem DBItem;
                                    if (Database.Server.ItemsBase.TryGetValue(729976, out DBItem))
                                    {
                                        DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                                    }
                                    DataItem.Color = Role.Flags.Color.Red;
                                    ushort xx = X;
                                    ushort yy = Y;
                                    if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                                    {
                                        MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                                        if (killer.Map.EnqueueItem(DropItem))
                                        {
                                            DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                        }
                                    }
                                }
                            }
                            if (killer.Player.QuestGUI.CheckQuest(1303, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1303, 1);
                            }
                            if (killer.Player.QuestGUI.CheckQuest(1314, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1314, 1);
                            }
                            if (killer.Player.QuestGUI.CheckQuest(1746, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (Role.Core.Rate(15))
                                {
                                    killer.Inventory.Add(stream, 721699);
                                    killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1746, 1);
#if Arabic
                                           killer.CreateBoxDialog("You`ve~found~a~Decayed~Heart!");
#else
                                    killer.CreateBoxDialog("You`ve~found~a~Decayed~Heart!");
#endif

                                }
                            }
                        }
                        if (Family.ID == 8305)//monkeyking
                        {
                            if (killer.Player.QuestGUI.CheckQuest(1305, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1305, 1);
#if Arabic
                                  killer.CreateBoxDialog("You~received~the~Monkey~King~Tail!");
#else
                                killer.CreateBoxDialog("You~received~the~Monkey~King~Tail!");
#endif

                                killer.Inventory.Add(stream, 720863);
                            }
                        }
                        if (Family.ID == 69)
                        {
                            if (killer.Player.QuestGUI.CheckQuest(1304, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1304, 1);
                            }
                        }
                        if (Family.ID == 11223)//vulture
                        {
                            if (killer.Player.QuestGUI.CheckQuest(1306, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (killer.Inventory.HaveSpace(1))
                                {
                                    killer.Inventory.Add(stream, 729976);
#if Arabic
                                     killer.CreateBoxDialog("You~received~a~Vulture~Feather.~Climb~up~to~the~City~Wall~at~(554,606)~to~burn~it!");
#else
                                    killer.CreateBoxDialog("You~received~a~Vulture~Feather.~Climb~up~to~the~City~Wall~at~(554,606)~to~burn~it!");
#endif

                                }
                                else
                                {
#if Arabic
                                     killer.CreateBoxDialog("Please make 1 more space in your inventory.");
#else
                                    killer.CreateBoxDialog("Please make 1 more space in your inventory.");
#endif

                                }
                            }
                            if (killer.Player.QuestGUI.CheckQuest(1301, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1301, 1);
                            }
                            if (Role.Core.Rate(15))
                            {
                                if (killer.Player.QuestGUI.CheckQuest(1736, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                                {
                                    killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1736, 1, 1);
#if Arabic
                                     killer.CreateBoxDialog("You`ve~found~a~Vulture~Feather.");
#else
                                    killer.CreateBoxDialog("You`ve~found~a~Vulture~Feather.");
#endif

                                }

                                MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                                DataItem.ITEM_ID = 729976;//birdclaw
                                Database.ItemType.DBItem DBItem;
                                if (Database.Server.ItemsBase.TryGetValue(729976, out DBItem))
                                {
                                    DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                                }
                                DataItem.Color = Role.Flags.Color.Red;
                                ushort xx = X;
                                ushort yy = Y;
                                if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                                {
                                    MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                                    if (killer.Map.EnqueueItem(DropItem))
                                    {
                                        DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                    }
                                }

                            }
                        }
                    }
                    else if (killer.Player.Map == 1011)//phoenix
                    {

                        if (Family.ID == 106)
                        {
                            if (killer.Player.QuestGUI.CheckQuest(1753, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1753, 0, 1);
#if Arabic
                                 killer.SendSysMesage("You,ve~collected~some~smells.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
#else
                                killer.SendSysMesage("You,ve~collected~some~smells.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
#endif

                                if (killer.Player.QuestGUI.CheckObjectives(1753, 0, 10))
                                {
#if Arabic
                                    
                                    killer.CreateBoxDialog("You,ve collected the smell of Rock Monster!");
                              
#else

                                    killer.CreateBoxDialog("You,ve collected the smell of Rock Monster!");

#endif
                                }
                            }
                        }

                        if (Family.ID == 1403)
                        {
                            if (killer.Player.QuestGUI.CheckQuest(1725, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {

                                killer.Inventory.Add(stream, 729973);
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1725, 1, 1);
                                var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)MsgNpc.NpcID.VeteranHong, killer.Player.Class, 1725);
#if Arabic
   killer.Player.QuestGUI.SendAutoPatcher("You~killed~Cloud~the~Lustful~and~found~a~Wooden~Box!.", ActiveQuest.FinishNpcId.Map, ActiveQuest.FinishNpcId.X, ActiveQuest.FinishNpcId.Y, ActiveQuest.FinishNpcId.ID);

#else
                                killer.Player.QuestGUI.SendAutoPatcher("You~killed~Cloud~the~Lustful~and~found~a~Wooden~Box!.", ActiveQuest.FinishNpcId.Map, ActiveQuest.FinishNpcId.X, ActiveQuest.FinishNpcId.Y, ActiveQuest.FinishNpcId.ID);

#endif

                            }
                        }
                        else if (Family.ID == 11007)
                        {
                            if (Role.Core.Rate(4))
                            {
                                MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                                DataItem.ITEM_ID = 721263;
                                Database.ItemType.DBItem DBItem;
                                if (Database.Server.ItemsBase.TryGetValue(729094, out DBItem))
                                {
                                    DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                                }
                                DataItem.Color = Role.Flags.Color.Red;
                                ushort xx = X;
                                ushort yy = Y;
                                if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                                {
                                    MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                                    if (killer.Map.EnqueueItem(DropItem))
                                    {
                                        DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                    }
                                }
                            }
                            if (killer.Player.QuestGUI.CheckQuest(1103, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1103, 1);
                                if (killer.Player.QuestGUI.CheckObjectives(1103, 10))
                                {
                                    var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)MsgNpc.NpcID.YuJing, killer.Player.Class, 1103);
#if Arabic
 killer.Player.QuestGUI.SendAutoPatcher("You`ve~killed~10~Bandits.~Go~tell~Yu~Jing~about~it.", ActiveQuest.FinishNpcId.Map, ActiveQuest.FinishNpcId.X, ActiveQuest.FinishNpcId.Y, ActiveQuest.FinishNpcId.ID);

#else
                                    killer.Player.QuestGUI.SendAutoPatcher("You`ve~killed~10~Bandits.~Go~tell~Yu~Jing~about~it.", ActiveQuest.FinishNpcId.Map, ActiveQuest.FinishNpcId.X, ActiveQuest.FinishNpcId.Y, ActiveQuest.FinishNpcId.ID);

#endif

                                }
                            }
                            if (killer.Player.QuestGUI.CheckQuest(1720, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1720, 1);
                                if (killer.Player.QuestGUI.CheckObjectives(1720, 5))
                                {
                                    var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)MsgNpc.NpcID.ThiefChen, killer.Player.Class, 1720);
#if Arabic
  killer.Player.QuestGUI.SendAutoPatcher("You`ve~killed~5~Bandits.~Now~go~find~Thief~Chen!.", ActiveQuest.FinishNpcId.Map, ActiveQuest.FinishNpcId.X, ActiveQuest.FinishNpcId.Y, ActiveQuest.FinishNpcId.ID);

#else
                                    killer.Player.QuestGUI.SendAutoPatcher("You`ve~killed~5~Bandits.~Now~go~find~Thief~Chen!.", ActiveQuest.FinishNpcId.Map, ActiveQuest.FinishNpcId.X, ActiveQuest.FinishNpcId.Y, ActiveQuest.FinishNpcId.ID);

#endif

                                }
                            }
                        }
                        else if (Family.ID == 7)//bandit
                        {
                            if (Role.Core.Rate(15))
                            {
                                if (killer.Player.QuestGUI.CheckQuest(1724, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                                {
                                    killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1724, 1);
                                    killer.Inventory.AddItemWitchStack(729971, 0, 1, stream);
                                    if (killer.Player.QuestGUI.CheckObjectives(1724, 1))
                                    {
                                        var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)MsgNpc.NpcID.VeteranHong, killer.Player.Class, 1724);
#if Arabic
 killer.Player.QuestGUI.SendAutoPatcher("Congratulations!~You~found~an~invitation~to~the~Treasure~Appraisal~Meeting.~Open~your~inventory~and~read~it.", ActiveQuest.FinishNpcId.Map, ActiveQuest.FinishNpcId.X, ActiveQuest.FinishNpcId.Y, ActiveQuest.FinishNpcId.ID);

#else
                                        killer.Player.QuestGUI.SendAutoPatcher("Congratulations!~You~found~an~invitation~to~the~Treasure~Appraisal~Meeting.~Open~your~inventory~and~read~it.", ActiveQuest.FinishNpcId.Map, ActiveQuest.FinishNpcId.X, ActiveQuest.FinishNpcId.Y, ActiveQuest.FinishNpcId.ID);

#endif

                                    }
                                }
                            }

                            if (killer.Player.QuestGUI.CheckQuest(1114, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1114, 1);
                                killer.Inventory.AddItemWitchStack(711322, 0, 1, stream);
#if Arabic
                                 killer.SendSysMesage("You've killed the Bandit and received a bag of grain!", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
#else
                                killer.SendSysMesage("You've killed the Bandit and received a bag of grain!", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
#endif


                                if (killer.Player.QuestGUI.CheckObjectives(1114, 10))
                                {
                                    var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)MsgNpc.NpcID.ForestGuvernor, killer.Player.Class, 1114);
#if Arabic
    killer.Player.QuestGUI.SendAutoPatcher("You`ve~collected~the~bags~of~10~Bandits.~Hurry~to~report~back~to~the~ForestGuvernor!", ActiveQuest.FinishNpcId.Map, ActiveQuest.FinishNpcId.X, ActiveQuest.FinishNpcId.Y, ActiveQuest.FinishNpcId.ID);

#else
                                    killer.Player.QuestGUI.SendAutoPatcher("You`ve~collected~the~bags~of~10~Bandits.~Hurry~to~report~back~to~the~ForestGuvernor!", ActiveQuest.FinishNpcId.Map, ActiveQuest.FinishNpcId.X, ActiveQuest.FinishNpcId.Y, ActiveQuest.FinishNpcId.ID);

#endif

                                }
                            }

                        }
                        else if (Family.ID == 66)//banditL33
                        {
                            if (Role.Core.Rate(40))
                            {
                                if (killer.Player.QuestGUI.CheckQuest(1115, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                                {
                                    killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1115, 1);
                                    killer.Inventory.AddItemWitchStack(720837, 0, 1, stream);
                                    var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)MsgNpc.NpcID.IntelligenceAgent, killer.Player.Class, 1115);
#if Arabic
                                         killer.Player.QuestGUI.SendAutoPatcher("You~killed~the~Bandit~and~found~a~Bandit`s~Coat!", ActiveQuest.FinishNpcId.Map, ActiveQuest.FinishNpcId.X, ActiveQuest.FinishNpcId.Y, ActiveQuest.FinishNpcId.ID);
                               
#else
                                    killer.Player.QuestGUI.SendAutoPatcher("You~killed~the~Bandit~and~found~a~Bandit`s~Coat!", ActiveQuest.FinishNpcId.Map, ActiveQuest.FinishNpcId.X, ActiveQuest.FinishNpcId.Y, ActiveQuest.FinishNpcId.ID);

#endif
                                }
                            }
                        }
                        else if (Family.ID == 11008)
                        {
                            if (killer.Player.QuestGUI.CheckQuest(1118, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1118, 1);
#if Arabic
                                 killer.Player.QuestGUI.SendAutoPatcher("You`ve~killed~the~Bandit~Leader.~Hurry~and~return~to~the~Bandit~Boss.", 1011, 227, 404, 8272);
#else
                                killer.Player.QuestGUI.SendAutoPatcher("You`ve~killed~the~Bandit~Leader.~Hurry~and~return~to~the~Bandit~Boss.", 1011, 227, 404, 8272);
#endif

                            }
                        }
                        else if (Family.ID == 8)//Ratlings
                        {
                            if (Role.Core.Rate(40))
                            {
                                if (killer.Player.QuestGUI.CheckQuest(1142, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                                {
                                    killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1142, 1);
                                    killer.Inventory.AddItemWitchStack(711334, 0, 1, stream);
#if Arabic
                                      killer.SendSysMesage("You received a Fury Core!", MsgMessage.ChatMode.System);
#else
                                    killer.SendSysMesage("You received a Fury Core!", MsgMessage.ChatMode.System);
#endif

                                    if (killer.Player.QuestGUI.CheckObjectives(1142, 5))
                                    {
                                        var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)Game.MsgNpc.NpcID.MilitiaLeopard, killer.Player.Class, 1142);
#if Arabic
                                          killer.Player.QuestGUI.SendAutoPatcher("You`ve~gathered~5~Fury~Cores!~Hurry~to~deliver~them~to~Militia~Leopard!", ActiveQuest2.FinishNpcId.Map, ActiveQuest2.FinishNpcId.X, ActiveQuest2.FinishNpcId.Y, ActiveQuest2.FinishNpcId.ID);
                                   
#else
                                        killer.Player.QuestGUI.SendAutoPatcher("You`ve~gathered~5~Fury~Cores!~Hurry~to~deliver~them~to~Militia~Leopard!", ActiveQuest2.FinishNpcId.Map, ActiveQuest2.FinishNpcId.X, ActiveQuest2.FinishNpcId.Y, ActiveQuest2.FinishNpcId.ID);

#endif
                                    }
                                }
                            }
                            if (killer.Player.QuestGUI.CheckQuest(1741, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1741, 1);
                                if (killer.Player.QuestGUI.CheckObjectives(1741, 20))
                                {
                                    var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)Game.MsgNpc.NpcID.DealerShen, killer.Player.Class, 1741);
#if Arabic
                                             killer.Player.QuestGUI.SendAutoPatcher("You`ve~killed~20~Rats!~Now~you~can~go~report~to~Dealer~Shen~(779,477).", ActiveQuest2.FinishNpcId.Map, ActiveQuest2.FinishNpcId.X, ActiveQuest2.FinishNpcId.Y, ActiveQuest2.FinishNpcId.ID);
                            
#else
                                    killer.Player.QuestGUI.SendAutoPatcher("You`ve~killed~20~Rats!~Now~you~can~go~report~to~Dealer~Shen~(779,477).", ActiveQuest2.FinishNpcId.Map, ActiveQuest2.FinishNpcId.X, ActiveQuest2.FinishNpcId.Y, ActiveQuest2.FinishNpcId.ID);

#endif
                                }
                            }



                            if (Role.Core.Rate(10))
                            {
                                if (killer.Player.QuestGUI.CheckQuest(1729, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                                {
                                    killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1729, 0, 1);
                                    killer.Inventory.AddItemWitchStack(721696, 0, 1, stream);
#if Arabic
                                    killer.CreateBoxDialog("You`ve~killed~the~Level~38~Fire~Rat~and~found~a~Ratling~Meat!");
#else
                                    killer.CreateBoxDialog("You`ve~killed~the~Level~38~Fire~Rat~and~found~a~Ratling~Meat!");
#endif

                                }
                            }
                            if (Role.Core.Rate(15))
                            {
                                if (killer.Player.QuestGUI.CheckQuest(1729, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                                {
                                    killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1729, 1, 0);
                                    killer.Inventory.AddItemWitchStack(721695, 0, 1, stream);
#if Arabic
                                      killer.CreateBoxDialog("You~found~a~Ratling~Eye~on~the~Ratling`s~body.");
#else
                                    killer.CreateBoxDialog("You~found~a~Ratling~Eye~on~the~Ratling`s~body.");
#endif

                                }
                            }
                            killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1121, 1);
                            if (killer.Player.QuestGUI.CheckObjectives(1121, 10))
                            {
                                var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)Game.MsgNpc.NpcID.BoldBoy, killer.Player.Class, 1121);
#if Arabic
                                        killer.Player.QuestGUI.SendAutoPatcher("You`ve~killed~10~Ratlings.~Go~tell~Bold~Boy~about~it", ActiveQuest2.FinishNpcId.Map, ActiveQuest2.FinishNpcId.X, ActiveQuest2.FinishNpcId.Y, ActiveQuest2.FinishNpcId.ID);
                         
#else
                                killer.Player.QuestGUI.SendAutoPatcher("You`ve~killed~10~Ratlings.~Go~tell~Bold~Boy~about~it", ActiveQuest2.FinishNpcId.Map, ActiveQuest2.FinishNpcId.X, ActiveQuest2.FinishNpcId.Y, ActiveQuest2.FinishNpcId.ID);

#endif
                            }
                            if (killer.Player.QuestGUI.CheckQuest(1127, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1127, 1);
                                if (killer.Player.QuestGUI.CheckObjectives(1127, 10))
                                {
                                    var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)Game.MsgNpc.NpcID.VillageHead, killer.Player.Class, 1127);
#if Arabic
                                                                        killer.Player.QuestGUI.SendAutoPatcher("You~have~killed~10~Ratlings!~Go~report~to~the~Village~Head!", ActiveQuest2.FinishNpcId.Map, ActiveQuest2.FinishNpcId.X, ActiveQuest2.FinishNpcId.Y, ActiveQuest2.FinishNpcId.ID);
#else
                                    killer.Player.QuestGUI.SendAutoPatcher("You~have~killed~10~Ratlings!~Go~report~to~the~Village~Head!", ActiveQuest2.FinishNpcId.Map, ActiveQuest2.FinishNpcId.X, ActiveQuest2.FinishNpcId.Y, ActiveQuest2.FinishNpcId.ID);
#endif

                                }
                            }
                            if (Role.Core.Rate(40))
                            {
                                if (killer.Player.QuestGUI.CheckQuest(1123, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                                {
                                    killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1123, 1);
                                    killer.Inventory.AddItemWitchStack(711325, 0, 1, stream);
                                    if (killer.Player.QuestGUI.CheckObjectives(1123, 10))
                                    {
                                        var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)Game.MsgNpc.NpcID.FurDealer, killer.Player.Class, 1123);
#if Arabic
                                         killer.Player.QuestGUI.SendAutoPatcher("You`ve~gathered~10~pieces~Ratling~Fur.~Go~deliver~them~to~the~Fur~Dealer!", ActiveQuest2.FinishNpcId.Map, ActiveQuest2.FinishNpcId.X, ActiveQuest2.FinishNpcId.Y, ActiveQuest2.FinishNpcId.ID);
#else
                                        killer.Player.QuestGUI.SendAutoPatcher("You`ve~gathered~10~pieces~Ratling~Fur.~Go~deliver~them~to~the~Fur~Dealer!", ActiveQuest2.FinishNpcId.Map, ActiveQuest2.FinishNpcId.X, ActiveQuest2.FinishNpcId.Y, ActiveQuest2.FinishNpcId.ID);
#endif

                                    }
                                }
                            }
                        }
                        else if (Family.ID == 6)
                        {
                            if (Role.Core.Rate(40))
                            {
                                if (killer.Player.QuestGUI.CheckQuest(401, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                                {
                                    if (killer.Inventory.Contain(710220, 5))
                                    {
                                        killer.SendSysMesage("You`ve obtained 5 pieces of Snake Meat, already.");
                                    }
                                    else
                                    {
                                        if (killer.Inventory.HaveSpace(1))
                                        {
                                            killer.Inventory.AddItemWitchStack(710220, 0, 1, stream);
                                            killer.SendSysMesage("You received a piece of Snake Meat!");
                                        }
                                        else
                                        {
                                            killer.SendSysMesage("Your inventory is full!");
                                        }
                                    }
                                }
                                if (killer.Player.QuestGUI.CheckQuest(1107, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                                {
                                    killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1107, 1);
                                    killer.Inventory.AddItemWitchStack(711321, 0, 1, stream);
#if Arabic
                                    killer.SendSysMesage("You've killed the Winged Snake and received a Snake Scale!", MsgMessage.ChatMode.System);
#else
                                    killer.SendSysMesage("You've killed the Winged Snake and received a Snake Scale!", MsgMessage.ChatMode.System);
#endif

                                    if (killer.Player.QuestGUI.CheckObjectives(1107, 10))
                                    {
                                        var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)Game.MsgNpc.NpcID.HunterWong, killer.Player.Class, 1107);
#if Arabic
                                             killer.Player.QuestGUI.SendAutoPatcher("You`ve~gathered~10~Snake~Scales!~Now~go~deliver~them~to~Hunter~Wong!", ActiveQuest2.FinishNpcId.Map, ActiveQuest2.FinishNpcId.X, ActiveQuest2.FinishNpcId.Y, ActiveQuest2.FinishNpcId.ID);
                                   
#else
                                        killer.Player.QuestGUI.SendAutoPatcher("You`ve~gathered~10~Snake~Scales!~Now~go~deliver~them~to~Hunter~Wong!", ActiveQuest2.FinishNpcId.Map, ActiveQuest2.FinishNpcId.X, ActiveQuest2.FinishNpcId.Y, ActiveQuest2.FinishNpcId.ID);

#endif
                                    }
                                }
                                if (killer.Player.QuestGUI.CheckQuest(1125, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                                {
                                    if (killer.Player.QuestGUI.CheckObjectives(1125, 1) == false)
                                    {
                                        killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1125, 1);
                                        killer.Inventory.AddItemWitchStack(711327, 0, 1, stream);
#if Arabic
                                         killer.SendSysMesage("You've killed the Winged Snake and received a Snake Meat!", MsgMessage.ChatMode.System);
#else
                                        killer.SendSysMesage("You've killed the Winged Snake and received a Snake Meat!", MsgMessage.ChatMode.System);
#endif


                                    }
                                }

                            }
                        }
                        else if (Family.ID == 3031)//cateran
                        {
                            if (Role.Core.Rate(40))
                            {
                                if (killer.Player.QuestGUI.CheckQuest(1128, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                                {
                                    killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1128, 1);
                                    if (killer.Player.QuestGUI.CheckObjectives(1128, 1))
                                    {
                                        killer.Inventory.Add(stream, 711329);
                                        var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)Game.MsgNpc.NpcID.VillageHead, killer.Player.Class, 1128);
#if Arabic
                                        
                                        killer.Player.QuestGUI.SendAutoPatcher("You`ve~received~a~Frost~Token!", ActiveQuest2.FinishNpcId.Map, ActiveQuest2.FinishNpcId.X, ActiveQuest2.FinishNpcId.Y, ActiveQuest2.FinishNpcId.ID);
                                   
#else

                                        killer.Player.QuestGUI.SendAutoPatcher("You`ve~received~a~Frost~Token!", ActiveQuest2.FinishNpcId.Map, ActiveQuest2.FinishNpcId.X, ActiveQuest2.FinishNpcId.Y, ActiveQuest2.FinishNpcId.ID);

#endif
                                    }
                                }
                            }
                        }
                        else if (Family.ID == 8303)
                        {
                            if (killer.Player.QuestGUI.CheckQuest(1129, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1129, 1);
                                if (killer.Player.QuestGUI.CheckObjectives(1129, 1))
                                {
                                    var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)Game.MsgNpc.NpcID.VillageHead, killer.Player.Class, 1129);
#if Arabic
                                        killer.Player.QuestGUI.SendAutoPatcher("You~defeated~the~Ratling~King!~Report~your~victory~to~the~Village~Head.", ActiveQuest2.FinishNpcId.Map, ActiveQuest2.FinishNpcId.X, ActiveQuest2.FinishNpcId.Y, ActiveQuest2.FinishNpcId.ID);
                               
#else
                                    killer.Player.QuestGUI.SendAutoPatcher("You~defeated~the~Ratling~King!~Report~your~victory~to~the~Village~Head.", ActiveQuest2.FinishNpcId.Map, ActiveQuest2.FinishNpcId.X, ActiveQuest2.FinishNpcId.Y, ActiveQuest2.FinishNpcId.ID);

#endif
                                }
                            }
                        }
                        else if (Family.ID == 113)
                        {
                            if (Role.Core.Rate(40))
                            {
                                if (killer.Player.QuestGUI.CheckQuest(1135, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                                {
                                    killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1135, 1, 1);
                                    killer.Inventory.AddItemWitchStack(711332, 0, 1, stream);
#if Arabic
                                     killer.SendSysMesage("You received a Fire Spirit Tooth.", MsgMessage.ChatMode.System);
#else
                                    killer.SendSysMesage("You received a Fire Spirit Tooth.", MsgMessage.ChatMode.System);
#endif

                                    if (killer.Player.QuestGUI.CheckObjectives(1135, 10))
                                    {
                                        var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)Game.MsgNpc.NpcID.MilitiaTiger, killer.Player.Class, 1135);
#if Arabic
                                             killer.Player.QuestGUI.SendAutoPatcher("You`ve~gathered~10~Spirit~Teeth!~Go~deliver~them~to~Militia~Tiger!", ActiveQuest2.FinishNpcId.Map, ActiveQuest2.FinishNpcId.X, ActiveQuest2.FinishNpcId.Y, ActiveQuest2.FinishNpcId.ID);
                                   
#else
                                        killer.Player.QuestGUI.SendAutoPatcher("You`ve~gathered~10~Spirit~Teeth!~Go~deliver~them~to~Militia~Tiger!", ActiveQuest2.FinishNpcId.Map, ActiveQuest2.FinishNpcId.X, ActiveQuest2.FinishNpcId.Y, ActiveQuest2.FinishNpcId.ID);

#endif
                                    }
                                }
                            }
                            if (killer.Player.QuestGUI.CheckQuest(1144, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1144, 1);
#if Arabic
                                 killer.SendSysMesage("You received a Fire Spirit Tooth.", MsgMessage.ChatMode.System);
#else
                                killer.SendSysMesage("You received a Fire Spirit Tooth.", MsgMessage.ChatMode.System);
#endif

                                if (killer.Player.QuestGUI.CheckObjectives(1144, 15))
                                {
                                    var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)Game.MsgNpc.NpcID.WarrantOfArrest, killer.Player.Class, 1144);
#if Arabic
                                          killer.Player.QuestGUI.SendAutoPatcher("You`ve~killed~15~Fire~Spirits!", ActiveQuest2.FinishNpcId.Map, ActiveQuest2.FinishNpcId.X, ActiveQuest2.FinishNpcId.Y, ActiveQuest2.FinishNpcId.ID);
                               
#else
                                    killer.Player.QuestGUI.SendAutoPatcher("You`ve~killed~15~Fire~Spirits!", ActiveQuest2.FinishNpcId.Map, ActiveQuest2.FinishNpcId.X, ActiveQuest2.FinishNpcId.Y, ActiveQuest2.FinishNpcId.ID);

#endif
                                }
                            }

                            //WarrantOfArrest, client.Player.Class, 1144);
                            //711332
                        }
                        else if (Family.ID == 68)
                        {
                            if (Role.Core.Rate(40))
                            {
                                if (killer.Player.QuestGUI.CheckQuest(1142, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                                {
                                    killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1142, 1);
                                    killer.Inventory.AddItemWitchStack(711334, 0, 1, stream);
#if Arabic
                                    
                                    killer.SendSysMesage("You received a Fury Core!", MsgMessage.ChatMode.System);
#else

                                    killer.SendSysMesage("You received a Fury Core!", MsgMessage.ChatMode.System);
#endif
                                    if (killer.Player.QuestGUI.CheckObjectives(1142, 1))
                                    {
                                        var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)Game.MsgNpc.NpcID.MilitiaLeopard, killer.Player.Class, 1142);
#if Arabic
                                             killer.Player.QuestGUI.SendAutoPatcher("You`ve~gathered~5~Fury~Cores!~Hurry~to~deliver~them~to~Militia~Leopard!", ActiveQuest2.FinishNpcId.Map, ActiveQuest2.FinishNpcId.X, ActiveQuest2.FinishNpcId.Y, ActiveQuest2.FinishNpcId.ID);
                                    
#else
                                        killer.Player.QuestGUI.SendAutoPatcher("You`ve~gathered~5~Fury~Cores!~Hurry~to~deliver~them~to~Militia~Leopard!", ActiveQuest2.FinishNpcId.Map, ActiveQuest2.FinishNpcId.X, ActiveQuest2.FinishNpcId.Y, ActiveQuest2.FinishNpcId.ID);

#endif
                                    }
                                }
                            }
                        }
                    }
                jmp:
                    if (killer.Player.OnXPSkill() == MsgUpdate.Flags.SuperCyclone || killer.Player.OnXPSkill() == MsgUpdate.Flags.Cyclone
                           || killer.Player.OnXPSkill() == MsgUpdate.Flags.Superman)
                    {
                        killer.Player.XPCount++;
                        killer.Player.KillCounter++;

                        if (killer.Player.OnXPSkill() != MsgServer.MsgUpdate.Flags.Normal)
                        {

                            action.KillCounter = killer.Player.KillCounter;
                            killer.Player.UpdateXpSkill();
                        }

                    }
                    else if (killer.Player.OnXPSkill() == MsgUpdate.Flags.Normal)
                    {
                        killer.Player.XPCount++;
                    }
                    else if (killer.Player.OnXPSkill() != MsgUpdate.Flags.BladeFlurry)
                    {
                        if (killer.Player.OnXPSkill() != MsgUpdate.Flags.Omnipotence)
                        {
                            killer.Player.KillCounter++;
                            if (killer.Player.KillCounter % 4 == 0)
                                killer.Player.XPCount++;
                        }
                    }

                }
                Send(stream.InteractionCreate(&action));
                if (RemoveOnDead)
                {
                    AddFlag(MsgUpdate.Flags.FadeAway, 10, false);
                    GMap.View.LeaveMap<Role.IMapObj>(this);
                    if (GMap.IsFlagPresent(X, Y, Role.MapFlagType.Monster))
                        GMap.cells[X, Y] &= ~Role.MapFlagType.Monster;
                }
                if (Map == 3935)
                    return;
                if (killer != null)
                {
                    if (Map == 2022)//dis city map 2
                    {
                        killer.Player.KillersDisCity += 1;
                    }
                    if (Map == 2024)// dis city map 4
                    {
                        if (Family.ID == 66432)//ultimate pluto
                        {

                            MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                            DataItem.ITEM_ID = 790001;
                            Database.ItemType.DBItem DBItem;
                            if (Database.Server.ItemsBase.TryGetValue(3004181, out DBItem))
                            {
                                DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                            }
                            DataItem.Color = Role.Flags.Color.Red;
                            ushort xx = X;
                            ushort yy = Y;
                            if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                            {
                                MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                                if (killer.Map.EnqueueItem(DropItem))
                                {
                                    DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                }
                            }
                            MsgTournaments.MsgSchedules.DisCity.KillTheUltimatePluto(killer);

                        }
                    }                    
                    if (Map == 3030 && Family.ID == 5053)
                    {
                        var xrand = new System.Random();
                        uint value = (uint)xrand.Next(1, 5);
                        killer.Player.BattleFieldPoints += value;
                        killer.SendSysMesage("You Have Get [ " + value + " ] D-Points.");
                    }
                    if (killer.Player.Map == 3071)
                    {
                        if (Family.ID == 2700)
                        {
                            DropItemID(killer, 720028, stream, 6);
                          //  DropItemID(killer, 780000, stream, 6);
                            DropItemID(killer, 3004245, stream, 6);

                            if (Role.Core.Rate(5, 100))
                                DropItemID(killer, 720028, stream, 6);
                            else
                            {
                                if (killer.Inventory.HaveSpace(1))
                                {
                                    killer.Inventory.Add(stream, 711609);
                                    killer.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "dispel4");
                                    killer.CreateBoxDialog("You~received~a~Gold~Coin!");
                                }
                                else
                                {
                                    killer.CreateBoxDialog("Your~inventory~is~full!");
                                }
                            }

                        }
                    }
                    if (killer.Player.Map == 3071 && Family.ID == 2699)
                    {
                        if (killer.Inventory.HaveSpace(1))
                        {
                            killer.Inventory.Add(stream, 711610);
                            killer.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "dispel7");
                            killer.SendSysMesage("You~received~a~Silver~Coin!", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            killer.CreateBoxDialog("Your~inventory~is~full!");
                        }
                    }
                    if (killer.Player.Map == 3071 && Family.ID == 7022)
                    {
                        if (Role.Core.Rate(1, 400))
                        {
                            if (killer.Inventory.HaveSpace(1))
                            {
                                killer.Inventory.Add(stream, 711609);
                                killer.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "dispel4");
                                killer.CreateBoxDialog("You~received~a~Gold~Coin!");
                            }
                            else
                            {
                                killer.CreateBoxDialog("Your~inventory~is~full!");
                            }
                        }
                    }
                    if (killer.Player.Map == 3071 && Family.ID == 7022)
                    {
                        if (Role.Core.Rate(2, 150))
                        {
                            if (killer.Inventory.HaveSpace(1))
                            {
                                killer.Inventory.Add(stream, 711610);
                                killer.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "dispel4");
                                killer.CreateBoxDialog("You~received~a~Silver~Coin!");
                            }
                            else
                            {
                                killer.CreateBoxDialog("Your~inventory~is~full!");
                            }
                        }
                    }
                    if (killer.Player.Map == 3071 && Family.ID == 7022)
                    {
                        if (Role.Core.Rate(3, 100))
                        {
                            if (killer.Inventory.HaveSpace(1))
                            {
                                killer.Inventory.Add(stream, 711611);
                                killer.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "dispel4");
                                killer.CreateBoxDialog("You~received~a~Copper~Coin!");
                            }
                            else
                            {
                                killer.CreateBoxDialog("Your~inventory~is~full!");
                            }
                        }
                    }
                    if (Map == 3030 && Family.ID == 2438)
                    {
                        killer.Player.BattleFieldPoints += 250;
                        killer.CreateBoxDialog("You Have Get [ 250 ] D-Points.");
                    }
                    if (Map == 3030 && Family.ID == 60915)
                    {
                        killer.Player.BattleFieldPoints += 2500;
                        killer.CreateBoxDialog("You Have Get [ 2500 ] D-Points.");
                        MsgSchedules.SendSysMesage("" + killer.Player.Name + " has Kill Trojan In DIABLO-Event And Get 2500 D-Points!.", MsgServer.MsgMessage.ChatMode.BroadcastMessage, MsgServer.MsgMessage.MsgColor.red);
                    }                  
                    if (Map == 3846)
                    {
                        uint PostionDrop = Family.ItemGenerator.GeneratePotionExtra();
                        if (Role.Core.Rate(40))
                        {
                            if (PostionDrop != 0)
                            {
                                ushort xx = X;
                                ushort yy = Y;
                                if (killer.Map.AddGroundItem(ref xx, ref yy))
                                {
                                    DropItem(stream, killer.Player.UID, killer.Map, PostionDrop, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, false, 0);
                                }
                            }
                        }
                        if (Family.ID == 20211)//box;
                        {
                            for (int x = 0; x < 2; x++)
                            {
                                PostionDrop = Family.ItemGenerator.GeneratePotionExtra(true);
                                if (PostionDrop != 0)
                                {
                                    ushort xx = X;
                                    ushort yy = Y;
                                    if (killer.Map.AddGroundItem(ref xx, ref yy))
                                    {
                                        DropItem(stream, killer.Player.UID, killer.Map, PostionDrop, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, false, 0);
                                    }
                                }
                            }
                        }                      
                    }
                    else if (Map == 1011 || Map == 1020)
                    {
                        if (Family.ID == 3130 || Family.ID == 3134)//titan/ ganoderma
                        {
                            if (Role.Core.Rate(50))
                            {
                                uint[] DropSpecialItems = new uint[] { Database.ItemType.MoonBox, 3003235, Database.ItemType.DragonBallScroll, 3005126/*chi 500*/
                                ,3005125/*study 500*/};

                                ushort xx = X;
                                ushort yy = Y;
                                if (killer.Map.AddGroundItem(ref xx, ref yy, 4))
                                {
                                    uint IDDrop = DropSpecialItems[Program.GetRandom.Next(0, DropSpecialItems.Length)];
                                    DropItem(stream, killer.Player.UID, killer.Map, DropSpecialItems[Program.GetRandom.Next(0, DropSpecialItems.Length)], xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, false, 0);

                                    string drop_name = Database.Server.ItemsBase[IDDrop].Name;
#if Arabic
                                     SendSysMesage("The " + Name.ToString() + " has been destroyed by the " + killer.Player.Name.ToString() + " ! he received one " + drop_name + " ", Game.MsgServer.MsgMessage.ChatMode.Center, MsgMessage.MsgColor.red);
                         
#else
                                    SendSysMesage("The " + Name.ToString() + " has been destroyed by the " + killer.Player.Name.ToString() + " ! he received one " + drop_name + " ", Game.MsgServer.MsgMessage.ChatMode.Center, MsgMessage.MsgColor.red);

#endif
                                }

                            }
                        }
                    }
                    if (Boss > 0 && Family.ID == 20100)//NightmareCaptain
                    {
                        uint ItemID = 3004465;
                        if (Role.Core.Rate(20))
                        {
                            ItemID = 3004463;
#if Arabic
                               killer.CreateBoxDialog("You've found an YinYangSeal.");
#else
                            killer.CreateBoxDialog("You've found an YinYangSeal.");
#endif

                        }
                        else
                        {
#if Arabic
                               killer.CreateBoxDialog("You wasn`t very lucky today .. I hope you will have a better luck next time ^^.");
#else
                            killer.CreateBoxDialog("You wasn`t very lucky today .. I hope you will have a better luck next time ^^.");
#endif

                        }
                        ushort xx = X;
                        ushort yy = Y;
                        if (killer.Map.AddGroundItem(ref xx, ref yy))
                        {
                            DropItem(stream, killer.Player.UID, killer.Map, ItemID, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, false, 0);
                        }
                        return;
                    }
                    if (Boss > 0 && Family.ID == 20101 && Map == 3851)//PurpleBanshee
                    {
                        uint ItemID = 3004465;
                        if (Role.Core.Rate(20))
                        {
                            ItemID = 3004464;
#if Arabic
                                  killer.CreateBoxDialog("You've found an Life`sEye.");
                            SendSysMesage("The PurpleBanshee has been destroyed by " + killer.Player.Name.ToString() + ", he got the Life`sEye", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);
                       
#else
                            killer.CreateBoxDialog("You've found an Life`sEye.");
                            SendSysMesage("The PurpleBanshee has been destroyed by " + killer.Player.Name.ToString() + ", he got the Life`sEye", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);

#endif
                        }
                        else
                        {
#if Arabic
                             killer.CreateBoxDialog("You wasn`t very lucky today .. I hope you will have a better luck next time ^^.");
#else
                            killer.CreateBoxDialog("You wasn`t very lucky today .. I hope you will have a better luck next time ^^.");
#endif

                        }
                        ushort xx = X;
                        ushort yy = Y;
                        if (killer.Map.AddGroundItem(ref xx, ref yy))
                        {
                            DropItem(stream, killer.Player.UID, killer.Map, ItemID, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, false, 0);
                        }
                        return;
                    }
                    if (Boss > 0 && Family.ID == 20101 && Map == 3825)//PurpleBanshee
                    {
                        //3003340 SolarBlade
                        uint ItemID = 3003340;
#if Arabic
                         killer.CreateBoxDialog("You've found an SolarBlade.");
                        SendSysMesage("The PurpleBanshee has been destroyed by " + killer.Player.Name.ToString() + ", he got the SolarBlade", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);
                     
#else
                        killer.CreateBoxDialog("You've found an SolarBlade.");
                        SendSysMesage("The PurpleBanshee has been destroyed by " + killer.Player.Name.ToString() + ", he got the SolarBlade", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);

#endif


                        ushort xx = X;
                        ushort yy = Y;
                        if (killer.Map.AddGroundItem(ref xx, ref yy, 5))
                        {
                            DropItem(stream, killer.Player.UID, killer.Map, ItemID, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, false, 0);
                        }
                        return;
                    }
                                     
                    if (Boss > 0 && ( Family.ID == 29360))//spook
                    {
                        if (killer.Player.Map == 10137)
                        {

                            const ushort GetStudyPoints = 1000;

                            List<uint> DropIems = Family.ItemGenerator.GenerateSoulsItems(6);
                            foreach (var ids in DropIems)
                            {
                                MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                                DataItem.ITEM_ID = ids;
                                Database.ItemType.DBItem DBItem;
                                if (Database.Server.ItemsBase.TryGetValue(ids, out DBItem))
                                {
                                    DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                                }
                                DataItem.Color = Role.Flags.Color.Red;
                                ushort xx = X;
                                ushort yy = Y;
                                if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                                {
                                    MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                                    if (killer.Map.EnqueueItem(DropItem))
                                    {
                                        killer.HeroRewards.AddGoal(709);
                                        DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                    }
                                }
                            }
                            if (killer.Player.DynamicID != killer.Player.UID)
                            {
                                for (int x = 0; x < 15; x++)
                                {
                                    uint id = Database.ItemType.DragonBall;
                                    if (x == 0)
                                        id = 730004;
                                    if (x == 1)
                                        id = 3009002;
                                    if (x == 2)
                                        id = 3005544;//PiratesToken
                                    if (x == 4)
                                        id = 730003;//ChiToken
                                    if (x == 5)
                                        id = 723718;//moneybag
                                    if (x == 6)
                                        id = 723717;//jaingToken
                                    if (x == 7)
                                        id = 3005411;
                                    if (x == 8)
                                        id = 730004;
                                    if (x == 9)
                                        id = 720957;//soulP6Bag
                                    if (x == 10)
                                        id = 720957;
                                    if (x == 11)
                                        id = 723717;
                                    if (x == 12)
                                        id = 3004181;//bone
                                    if (x == 13)
                                        id = 3004181;
                                    if (x == 14)
                                        id = 3004181;
                                    if (x == 15)
                                        id = 3004181;

                                    MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                                    DataItem.ITEM_ID = id;
                                    Database.ItemType.DBItem DBItem;
                                    if (Database.Server.ItemsBase.TryGetValue(id, out DBItem))
                                    {
                                        DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                                    }
                                    DataItem.Color = Role.Flags.Color.Red;
                                    ushort xx = (ushort)Program.GetRandom.Next(X - 7, X + 7);
                                    ushort yy = (ushort)Program.GetRandom.Next(Y - 7, Y + 7);
                                    if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                                    {
                                        MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                                        if (killer.Map.EnqueueItem(DropItem))
                                        {

                                            DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                        }
                                    }
                                }
                            }
                            if (killer.Team != null)
                            {
                                foreach (var member in killer.Team.Temates)
                                {
                                    if (member != null && member.client != null && member.client.Player != null && member.client.Player.SubClass != null)
                                    {
                                        if (member.client.Player.UID != killer.Player.UID)
                                        {
                                            member.client.Player.SubClass.AddStudyPoints(member.client, GetStudyPoints, stream);
                                        }
                                    }
                                }
                            }
                            if (killer.Player.SubClass != null)
                                killer.Player.SubClass.AddStudyPoints(killer, GetStudyPoints, stream);

                            return;
                        }
                    }
                    if (Family.ID == 10166 || Family.ID == 10167 || Family.ID == 10168)
                    {
                        killer.CreateBoxDialog("the usual drop in this map has been moved to Realm.");
                    }
                    if (Boss > 0 && (Family.ID == 20300 ))// NemesisTyrant
                    {
                        GetBossReward(stream, killer);
                        killer.Player.PIKAPoint += 1;
                        killer.Player.BoundConquerPoints += 15;
                        killer.Inventory.Add(stream, 3008733, 1);
                        const ushort GetStudyPoints = 4300;
                        MsgSchedules.SendSysMesage("" + killer.Player.Name + "  Kill   [ " + Name.ToString() + " ] and Get 2500 Cps & DIABLO Spin & 15 Coins & 1 EP . ", MsgServer.MsgMessage.ChatMode.BroadcastMessage, MsgServer.MsgMessage.MsgColor.white);
                        List<uint> DropIems = Family.ItemGenerator.GenerateSoulsItems(6);
                        foreach (var ids in DropIems)
                        {
                            MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                            DataItem.ITEM_ID = ids;
                            Database.ItemType.DBItem DBItem;
                            if (Database.Server.ItemsBase.TryGetValue(ids, out DBItem))
                            {
                                DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                            }
                            DataItem.Color = Role.Flags.Color.Red;
                            ushort xx = X;
                            ushort yy = Y;
                            if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                            {
                                MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                                if (killer.Map.EnqueueItem(DropItem))
                                {
                                    killer.HeroRewards.AddGoal(709);
                                    DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                }
                            }
                        }
                        if (killer.Player.DynamicID != killer.Player.UID)
                        {
                            for (int x = 0; x < 22; x++)
                            {
                                uint id = Database.ItemType.DragonBall;
                                if (x == 0)
                                    id = 3301224;//50 Vip Points
                                if (x == 1)
                                    id = 729847;//Token DIABLO
                                if (x == 2)
                                    id = 3304151;//Token Mount
                                if (x == 3)
                                    id = 3303698;//Token Garment
                                if (x == 4)
                                    id = 3303373;//KnowledgePill
                                if (x == 5)
                                    id = 3303373;//KnowledgePill
                                if (x == 6)
                                    id = 3004245;//Savage
                                if (x == 7)
                                    id = 3004245;//Material  
                                if (x == 8)
                                    id = 3009002;//P.Stone
                                if (x == 9)
                                    id = 3009002;//P.Stone
                                if (x == 10)
                                    id = 3007110;//Inner
                                if (x == 11)
                                    id = 3007110;//Inner
                                if (x == 12)
                                    id = 723715;//300K Gold
                                if (x == 13)
                                    id = 723715;//300K Gold
                                if (x == 14)
                                    id = 3002926;//FreeTrain
                                if (x == 15)
                                    id = 3002926;//FreeTrain
                                if (x == 16)
                                    id = 3002926;//FreeTrain
                                if (x == 17)
                                    id = 3001044;//fruit
                                if (x == 18)
                                    id = 3001044;//fruit
                                if (x == 19)
                                    id = 3301226;//ChiPoint
                                if (x == 20)
                                    id = 3004181;//ChiPoSavageint
                                MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                                DataItem.ITEM_ID = id;
                                Database.ItemType.DBItem DBItem;
                                if (Database.Server.ItemsBase.TryGetValue(id, out DBItem))
                                {
                                    DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                                }
                                DataItem.Color = Role.Flags.Color.Red;
                                ushort xx = (ushort)Program.GetRandom.Next(X - 5, X + 5);
                                ushort yy = (ushort)Program.GetRandom.Next(Y - 5, Y + 5);
                                if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                                {
                                    MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                                    if (killer.Map.EnqueueItem(DropItem))
                                    {

                                        DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                    }
                                }
                                //var now = Extensions.Time32.Now;
                                //RemoveView(now.AllSeconds, GMap);
                            }
                        }
                        if (killer.Team != null)
                        {
                            foreach (var member in killer.Team.Temates)
                            {
                                if (member != null && member.client != null && member.client.Player != null && member.client.Player.SubClass != null)
                                {
                                    if (member.client.Player.UID != killer.Player.UID)
                                    {
                                        member.client.Player.SubClass.AddStudyPoints(member.client, GetStudyPoints, stream);
                                        member.client.Player.ConquerPoints += 500;
                                        member.client.CreateBoxDialog("You Have Get 500 Cps As You One Of Team Killler");
                                    }
                                }
                            }
                        }
                        if (killer.Player.SubClass != null)
                            killer.Player.SubClass.AddStudyPoints(killer, GetStudyPoints, stream);
                        return;
                    }
                    if (Boss > 0 && (Family.ID == 20070))//Snow Banshee 
                    {
                        GetBossReward(stream, killer);
                        killer.Player.PIKAPoint += 1;
                        killer.Player.BoundConquerPoints += 15;
                        killer.Inventory.Add(stream, 3008733, 1);
                        const ushort GetStudyPoints = 4300;
                        MsgSchedules.SendSysMesage("" + killer.Player.Name + "  Kill   [ " + Name.ToString() + " ] and Get 2500 Cps & DIABLO Spin & 15 Coins & 1 EP . ", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.white);
                        List<uint> DropIems = Family.ItemGenerator.GenerateSoulsItems(6);
                        foreach (var ids in DropIems)
                        {
                            MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                            DataItem.ITEM_ID = ids;
                            Database.ItemType.DBItem DBItem;
                            if (Database.Server.ItemsBase.TryGetValue(ids, out DBItem))
                            {
                                DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                            }
                            DataItem.Color = Role.Flags.Color.Red;
                            ushort xx = X;
                            ushort yy = Y;
                            if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                            {
                                MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                                if (killer.Map.EnqueueItem(DropItem))
                                {
                                    killer.HeroRewards.AddGoal(709);
                                    DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                }
                            }
                        }
                        if (killer.Player.DynamicID != killer.Player.UID)
                        {
                            for (int x = 0; x < 22; x++)
                            {
                                uint id = Database.ItemType.DragonBall;
                                if (x == 0)
                                    id = 3301224;//50 Vip Points
                                if (x == 1)
                                    id = 729847;//Token DIABLO
                                if (x == 2)
                                    id = 3304151;//Token Mount
                                if (x == 3)
                                    id = 3303698;//Token Garment
                                if (x == 4)
                                    id = 3303373;//KnowledgePill
                                if (x == 5)
                                    id = 3303373;//KnowledgePill
                                if (x == 6)
                                    id = 3004245;//Savage
                                if (x == 7)
                                    id = 3004245;//Material  
                                if (x == 8)
                                    id = 3009002;//P.Stone
                                if (x == 9)
                                    id = 3009002;//P.Stone
                                if (x == 10)
                                    id = 3007110;//Inner
                                if (x == 11)
                                    id = 3007110;//Inner
                                if (x == 12)
                                    id = 723715;//300K Gold
                                if (x == 13)
                                    id = 723715;//300K Gold
                                if (x == 14)
                                    id = 3002926;//FreeTrain
                                if (x == 15)
                                    id = 3002926;//FreeTrain
                                if (x == 16)
                                    id = 3002926;//FreeTrain
                                if (x == 17)
                                    id = 3001044;//fruit
                                if (x == 18)
                                    id = 3001044;//fruit
                                if (x == 19)
                                    id = 3301226;//ChiPoint
                                if (x == 20)
                                    id = 3004181;//ChiPoSavageint
                                

                                MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                                DataItem.ITEM_ID = id;
                                Database.ItemType.DBItem DBItem;
                                if (Database.Server.ItemsBase.TryGetValue(id, out DBItem))
                                {
                                    DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                                }
                                DataItem.Color = Role.Flags.Color.Red;
                                ushort xx = (ushort)Program.GetRandom.Next(X - 5, X + 5);
                                ushort yy = (ushort)Program.GetRandom.Next(Y - 5, Y + 5);
                                if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                                {
                                    MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                                    if (killer.Map.EnqueueItem(DropItem))
                                    {

                                        DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                    }
                                }
                                //var now = Extensions.Time32.Now;
                                //RemoveView(now.AllSeconds, GMap);
                            }
                        }
                        if (killer.Team != null)
                        {
                            foreach (var member in killer.Team.Temates)
                            {
                                if (member != null && member.client != null && member.client.Player != null && member.client.Player.SubClass != null)
                                {
                                    if (member.client.Player.UID != killer.Player.UID)
                                    {
                                        member.client.Player.SubClass.AddStudyPoints(member.client, GetStudyPoints, stream);
                                        member.client.Player.ConquerPoints += 500;
                                        member.client.CreateBoxDialog("You Have Get 500 Cps As You One Of Killler Team");
                                    }
                                }
                            }
                        }
                        if (killer.Player.SubClass != null)
                            killer.Player.SubClass.AddStudyPoints(killer, GetStudyPoints, stream);
                        return;
                    }
                    //
                    if (Boss > 0 && (Family.ID == 60915))//Trojan Boss
                    {
                        if (killer.Player.DynamicID != killer.Player.UID)
                        {
                            for (int x = 0; x < 12; x++)
                            {
                                uint id = Database.ItemType.DragonBall;
                                MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                                if (x == 0)
                                {
                                    id = 729847;//diablo[t]
                                    DataItem.StackSize = 5;
                                }
                                if (x == 1)
                                {
                                    id = 729847;//diablo[t]
                                    DataItem.StackSize = 5;
                                }
                                if (x == 2)
                                {
                                    id = 3304151;//Mount[T]
                                    DataItem.StackSize = 5;
                                }
                                if (x == 3)
                                {
                                    id = 3304151;//Mount[T]
                                    DataItem.StackSize = 5;
                                }
                                if (x == 4)
                                {
                                    id = 3303698;//Garment[T]
                                    DataItem.StackSize = 5;
                                }
                                if (x == 5)
                                {
                                    id = 3303698;//Garment[T]
                                    DataItem.StackSize = 5;
                                }
                                if (x == 6)
                                    id = 3004245;//P7
                                if (x == 7)
                                    id = 3004245;//P7
                                DataItem.ITEM_ID = id;
                                Database.ItemType.DBItem DBItem;
                                if (Database.Server.ItemsBase.TryGetValue(id, out DBItem))
                                {
                                    DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                                }
                                DataItem.Color = Role.Flags.Color.Red;
                                ushort xx = (ushort)Program.GetRandom.Next(X - 5, X + 5);
                                ushort yy = (ushort)Program.GetRandom.Next(Y - 5, Y + 5);
                                if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                                {
                                    MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                                    if (killer.Map.EnqueueItem(DropItem))
                                    {

                                        DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                    }
                                }
                              
                            }
                        }
                        return;
                    }
                    //
                    if (Boss > 0 && ( Family.ID == 20160))//ThrillingSpook
                    {
                        GetBossReward(stream, killer);
                        killer.Player.ConquerPoints += 2500;
                        killer.Player.PIKAPoint += 1;
                        killer.Player.BoundConquerPoints += 30;
                        killer.Inventory.Add(stream, 3008733, 2);
                        const ushort GetStudyPoints = 4300;
                        // foreach (var obj in killer.Player.View.Roles(Role.MapObjectType.Monster)) ;
                        MsgSchedules.SendSysMesage("" + killer.Player.Name + "  Kill   [ " + Name.ToString() + " ] and Get 5000 Cps & 2 DIABLO Spin & 30 Coins & 1 EP ", MsgServer.MsgMessage.ChatMode.BroadcastMessage, MsgServer.MsgMessage.MsgColor.white);
                        List<uint> DropIems = Family.ItemGenerator.GenerateSoulsItems(6);
                        foreach (var ids in DropIems)
                        {
                            MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                            DataItem.ITEM_ID = ids;
                            Database.ItemType.DBItem DBItem;
                            if (Database.Server.ItemsBase.TryGetValue(ids, out DBItem))
                            {
                                DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                            }
                            DataItem.Color = Role.Flags.Color.Red;
                            ushort xx = X;
                            ushort yy = Y;
                            if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                            {
                                MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, 0, false, GMap);

                                if (killer.Map.EnqueueItem(DropItem))
                                {
                                    killer.HeroRewards.AddGoal(709);
                                    DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                }

                            }
                        }
                       
                        
                        if (killer.Player.DynamicID != killer.Player.UID)
                        {
                            for (int x = 0; x < 23; x++)
                            {
                                uint id = Database.ItemType.DragonBall;
                                if (x == 0)
                                    id = 3301224;//50 Vip Points
                                if (x == 1)
                                    id = 729847;//Token DIABLO
                                if (x == 2)
                                    id = 3304151;//Token Mount
                                if (x == 3)
                                    id = 3303698;//Token Garment
                                if (x == 4)
                                    id = 3303373;//KnowledgePill
                                if (x == 5)
                                    id = 3303373;//KnowledgePill
                                if (x == 6)
                                    id = 3004245;//Savage
                                if (x == 7)
                                    id = 3004245;//Material  
                                if (x == 8)
                                    id = 3009002;//P.Stone
                                if (x == 9)
                                    id = 3009002;//P.Stone
                                if (x == 10)
                                    id = 3007110;//Inner
                                if (x == 11)
                                    id = 3007110;//Inner
                                if (x == 12)
                                    id = 723715;//300K Gold
                                if (x == 13)
                                    id = 723715;//300K Gold
                                if (x == 14)
                                    id = 3002926;//FreeTrain
                                if (x == 15)
                                    id = 3002926;//FreeTrain
                                if (x == 16)
                                    id = 3002926;//FreeTrain
                                if (x == 17)
                                    id = 3001044;//fruit
                                if (x == 18)
                                    id = 3001044;//fruit
                                if (x == 19)
                                    id = 3301226;//ChiPoint
                                if (x == 20)
                                    id = 3004181;//ChiPoSavageint
                                if (x == 21)
                                    id = 3004181;//ChiPoSavageint

                                MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();

                                DataItem.ITEM_ID = id;
                                Database.ItemType.DBItem DBItem;
                                if (Database.Server.ItemsBase.TryGetValue(id, out DBItem))
                                {
                                    DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                                }
                                DataItem.Color = Role.Flags.Color.Red;
                                ushort xx = (ushort)Program.GetRandom.Next(X - 6, X + 6);
                                ushort yy = (ushort)Program.GetRandom.Next(Y - 6, Y + 6);
                                if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                                {
                                    MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, 0, false, GMap);

                                    if (killer.Map.EnqueueItem(DropItem))
                                    {

                                        DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                    }
                                }
                                //var now = Extensions.Time32.Now;
                                //RemoveView(now.AllSeconds, GMap);
                            }
                        }
                        if (killer.Team != null)
                        {
                            foreach (var member in killer.Team.Temates)
                            {
                                if (member != null && member.client != null && member.client.Player != null && member.client.Player.SubClass != null)
                                {
                                    if (member.client.Player.UID != killer.Player.UID)
                                    {
                                        member.client.Player.SubClass.AddStudyPoints(member.client, GetStudyPoints, stream);
                                        member.client.Player.ConquerPoints += 1000;
                                        member.client.CreateBoxDialog("You Have Get 1000 Cps As You One Of Team Killler");
                                    }
                                }
                            }
                        }
                        if (killer.Player.SubClass != null)
                            killer.Player.SubClass.AddStudyPoints(killer, GetStudyPoints, stream);
                        return;
                    }
                    if (Boss > 0 && (  Family.ID == 3970 || Family.ID == 20060))//SnowBanshe
                    {
                        GetBossReward(stream, killer);
                        const ushort GetStudyPoints = 1000;
                        List<uint> DropIems = Family.ItemGenerator.GenerateSoulsItems(6);
                        foreach (var ids in DropIems)
                        {
                            MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                            DataItem.ITEM_ID = ids;
                            Database.ItemType.DBItem DBItem;
                            if (Database.Server.ItemsBase.TryGetValue(ids, out DBItem))
                            {
                                DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                            }
                            DataItem.Color = Role.Flags.Color.Red;
                            ushort xx = X;
                            ushort yy = Y;
                            if (killer.Map.AddGroundItem(ref xx, ref yy, 1))
                            {
                                MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                                if (killer.Map.EnqueueItem(DropItem))
                                {
                                    killer.HeroRewards.AddGoal(709);
                                    DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                }
                            }
                        }
                        if (killer.Player.DynamicID != killer.Player.UID)
                        {
                            for (int x = 1; x < 17; x++)
                            {
                                uint id = Database.ItemType.DragonBall;
                                MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                                if (x == 1)
                                    id = 723717;
                                if (x == 2)
                                    id = 723717;
                                if (x == 3)
                                    id = 723717;
                                if (x == 4)
                                    id = 724002;
                                if (x == 5)
                                    id = 724002;
                                if (x == 7)
                                    id = 724002;
                                if (x == 8)
                                    id = 3009002;
                                if (x == 9)
                                    id = 3009002;
                                if (x == 10)
                                    id = 3009002;
                                if (x == 11)
                                    id = 3004181;
                                #region 100 TokenPrize
                                if (x == 12)
                                {
                                    id = 3200384;
                                    DataItem.StackSize = 10;
                                }
                                else if (x == 13)
                                {
                                    id = 3200384;
                                    DataItem.StackSize = 10;
                                }
                                else if (x == 14)
                                {
                                    id = 3200384;
                                    DataItem.StackSize = 10;
                                }
                                else if (x == 15)
                                {
                                    id = 3200384;
                                    DataItem.StackSize = 10;
                                }
                                else if (x == 16)
                                {
                                    id = 3200384;
                                    DataItem.StackSize = 10;
                                }
                                #endregion
                                DataItem.ITEM_ID = id;
                                Database.ItemType.DBItem DBItem;
                                if (Database.Server.ItemsBase.TryGetValue(id, out DBItem))
                                {
                                    DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                                }
                                DataItem.Color = Role.Flags.Color.Red;
                                ushort xx = (ushort)Program.GetRandom.Next(X - 7, X + 7);
                                ushort yy = (ushort)Program.GetRandom.Next(Y - 7, Y + 7);
                                if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                                {
                                    MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                                    if (killer.Map.EnqueueItem(DropItem))
                                    {

                                        DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                    }
                                }
                            }
                        }
                        if (killer.Team != null)
                        {
                            foreach (var member in killer.Team.Temates)
                            {
                                if (member != null && member.client != null && member.client.Player != null && member.client.Player.SubClass != null)
                                {
                                    if (member.client.Player.UID != killer.Player.UID)
                                    {
                                        member.client.Player.SubClass.AddStudyPoints(member.client, GetStudyPoints, stream);
                                    }
                                }
                            }
                        }
                        if (killer.Player.SubClass != null)
                            killer.Player.SubClass.AddStudyPoints(killer, GetStudyPoints, stream);
                        return;
                    }                       
                    if (killer.Player.Map == 4745 && Family.ID == 40937)
                    {
                        uint Items = 711533;
                        killer.Inventory.Add(stream, Items);
                        killer.Teleport(276, 360, 1002);
                    }
                    if (Boss > 0 && (Family.ID == 60898 || Family.ID == 60911  || Family.ID == 60914 || Family.ID == 60912 || Family.ID == 428094 || Family.ID == 60899 || Family.ID == 60891))
                    {
                        const ushort GetStudyPoints = 2000;


                        uint[] ItemsIDS = new uint[] { 720027};

                        {
                            uint ItemId = ItemsIDS[Program.GetRandom.Next(1, 2)];

                            MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                            DataItem.ITEM_ID = ItemId;
                            Database.ItemType.DBItem DBItem;
                            if (Database.Server.ItemsBase.TryGetValue(ItemId, out DBItem))
                            {
                                DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                            }
                            DataItem.Color = Role.Flags.Color.Red;

                            if (ItemId == 150059)
                                DataItem.Effect = Role.Flags.ItemEffect.Stigma;

                            if (ItemId == 120049)
                                DataItem.Effect = Role.Flags.ItemEffect.Shield;

                            ushort xx = X;
                            ushort yy = Y;
                            if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                            {
                                MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                                if (killer.Map.EnqueueItem(DropItem))
                                {
                                    killer.HeroRewards.AddGoal(709);
                                    DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                }
                            }
                        }
                        if (killer.Player.DynamicID != killer.Player.UID)
                        {
                            for (int x = 0; x < 1; x++)
                            {
                                uint id = Database.ItemType.DragonBall;
                                if (x == 0)
                                    id = 730004;

                                MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                                DataItem.ITEM_ID = id;
                                Database.ItemType.DBItem DBItem;
                                if (Database.Server.ItemsBase.TryGetValue(id, out DBItem))
                                {
                                    DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                                }
                                DataItem.Color = Role.Flags.Color.Red;
                                ushort xx = (ushort)Program.GetRandom.Next(X - 7, X + 7);
                                ushort yy = (ushort)Program.GetRandom.Next(Y - 7, Y + 7);
                                if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                                {
                                    MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                                    if (killer.Map.EnqueueItem(DropItem))
                                    {

                                        DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                    }
                                }
                            }
                        }
                        if (killer.Team != null)
                        {
                            foreach (var member in killer.Team.Temates)
                            {
                                if (member != null && member.client != null && member.client.Player != null && member.client.Player.SubClass != null)
                                {
                                    if (member.client.Player.UID != killer.Player.UID)
                                    {
                                        member.client.Player.SubClass.AddStudyPoints(member.client, GetStudyPoints, stream);
                                    }
                                }
                            }
                        }
                        if (killer.Player.SubClass != null)
                            killer.Player.SubClass.AddStudyPoints(killer, GetStudyPoints, stream);

                        if (Family.ID == 60914)
                           // DistributeBossPoints();
                        if (Family.ID == 60899)
                            //DistributeBossPoints();
                        if (Family.ID == 60915)
                          //  DistributeBossPoints();
                        if (Family.ID == 60913)
                           // DistributeBossPoints();
                        if (Family.ID == 60891)
                           // DistributeBossPoints();
                        if (Family.ID == 60911)
                          //  DistributeBossPoints();
                        if (Family.ID == 60898)
                           // DistributeBossPoints();
                        if (Family.ID == 428094)
                           // DistributeBossPoints();
                        if (Family.ID == 60912)
                           // DistributeBossPoints();

                        SendBossSysMesage(killer.Player.Name, GetStudyPoints, MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);


                        return;
                    }

                    if (Boss > 0 && Family.ID == 60913)//chaos Guard
                    {
                        const ushort GetStudyPoints = 50;
                        killer.Player.ConquerPoints += 7500;
                        if (killer.Player.DynamicID != killer.Player.UID)
                        {
                            for (int x = 0; x < 10; x++)
                            {
                                uint id = Database.ItemType.DragonBallScroll;
                                if (x == 0)
                                    id = 3301226;//ChiPoints
                                if (x == 1)
                                    id = 3301226;//ChiPoints
                                if (x == 2)
                                    id = 3303373;//KnowledgePill
                                if (x == 3)
                                    id = 3303373;//KnowledgePill
                                if (x == 4)
                                    id = 3303373;//KnowledgePill
                                if (x == 5)
                                    id = 3009002;//P.Stone
                                if (x == 6)
                                    id = 3009002;//P.Stone
                                if (x == 7)
                                    id = 3301226;//ChiPoints
                                if (x == 8)
                                    id = 3301226;//ChiPoints
                                if (x == 9)
                                    id = 3301226;//ChiPoints
                                MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                                DataItem.ITEM_ID = id;
                                Database.ItemType.DBItem DBItem;
                                if (Database.Server.ItemsBase.TryGetValue(id, out DBItem))
                                {
                                    DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                                }
                                DataItem.Color = Role.Flags.Color.Red;
                                ushort xx = (ushort)Program.GetRandom.Next(X - 4, X + 4);
                                ushort yy = (ushort)Program.GetRandom.Next(Y - 4, Y + 4);
                                if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                                {
                                    MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, killer.Player.UID, DynamicID, Map, 0 , false, GMap);

                                    if (killer.Map.EnqueueItem(DropItem))
                                    {

                                        DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                    }
                                }
                            }
                        }
                        if (killer.Team != null)
                        {
                            foreach (var member in killer.Team.Temates)
                            {
                                if (member != null && member.client != null && member.client.Player != null && member.client.Player.SubClass != null)
                                {
                                    if (member.client.Player.UID != killer.Player.UID)
                                    {
                                        member.client.Player.SubClass.AddStudyPoints(member.client, GetStudyPoints, stream);
                                        member.client.Player.ConquerPoints += 1000;
                                        member.client.CreateBoxDialog("You Have Get 1000 Cps As You One Of Team Killler");
                                    }
                                }
                            }
                        }
                        if (killer.Player.SubClass != null)
                            killer.Player.SubClass.AddStudyPoints(killer, GetStudyPoints, stream);
                        SendBossSysMesage(killer.Player.Name, GetStudyPoints, MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);


                        return;
                    }


                    if (Boss > 0 && (Family.ID == 20060))//terato dragon
                    {

                        const ushort GetStudyPoints = 1;
                     //   killer.Player.ConquerPoints += 10;
                        List<uint> DropIems = Family.ItemGenerator.GenerateSoulsItems(5);
                        foreach (var ids in DropIems)
                        {
                            MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                            DataItem.ITEM_ID = ids;
                            Database.ItemType.DBItem DBItem;
                            if (Database.Server.ItemsBase.TryGetValue(ids, out DBItem))
                            {
                                DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                            }
                            DataItem.Color = Role.Flags.Color.Red;
                            ushort xx = X;
                            ushort yy = Y;
                            if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                            {
                                MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                                if (killer.Map.EnqueueItem(DropItem))
                                {
                                    killer.HeroRewards.AddGoal(709);
                                    DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                }
                            }
                        }
                        if (killer.Team != null)
                        {
                            foreach (var member in killer.Team.Temates)
                            {
                                if (member != null && member.client != null && member.client.Player != null && member.client.Player.SubClass != null)
                                {
                                    if (member.client.Player.UID != killer.Player.UID)
                                    {
                                        member.client.Player.SubClass.AddStudyPoints(member.client, GetStudyPoints, stream);
                                    }
                                }
                            }
                        }
                        if (killer.Player.SubClass != null)
                            killer.Player.SubClass.AddStudyPoints(killer, GetStudyPoints, stream);
                        if (killer.Player.Map == 10166)
                        {
                            SendSysMesage(killer.Player.Name + " has defeated [Dragon Wraith] Prize 10 Cps ..", MsgServer.MsgMessage.ChatMode.SlideCrosTheServer, MsgServer.MsgMessage.MsgColor.white);
                        }
                        else
                        {
                            SendBossSysMesage(killer.Player.Name, GetStudyPoints, MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);
                        }

                        return;
                    }
                    if (Boss > 0 && Family.ID == 20090)//sword house
                    {
                        const ushort GetStudyPoints = 20;

                        List<uint> DropIems = Family.ItemGenerator.GenerateSoulsItems(3);
                        if (Role.Core.Rate(30))
                            DropIems.Add(711679);
                        if (Role.Core.Rate(30))
                            DropIems.Add(711188);


                        foreach (var ids in DropIems)
                        {
                            MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                            DataItem.ITEM_ID = ids;
                            Database.ItemType.DBItem DBItem;
                            if (Database.Server.ItemsBase.TryGetValue(ids, out DBItem))
                            {
                                DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                            }
                            DataItem.Color = Role.Flags.Color.Red;
                            ushort xx = X;
                            ushort yy = Y;
                            if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                            {
                                MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                                if (killer.Map.EnqueueItem(DropItem))
                                {
                                    DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                }
                            }
                        }
                        if (killer.Team != null)
                        {
                            foreach (var member in killer.Team.Temates)
                            {
                                if (member != null && member.client != null && member.client.Player != null && member.client.Player.SubClass != null)
                                {
                                    if (member.client.Player.UID != killer.Player.UID)
                                    {
                                        member.client.Player.SubClass.AddStudyPoints(member.client, GetStudyPoints, stream);
                                    }
                                }
                            }
                        }
                        if (killer.Player.SubClass != null)
                            killer.Player.SubClass.AddStudyPoints(killer, GetStudyPoints, stream);
                        SendBossSysMesage(killer.Player.Name, GetStudyPoints, MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);


                        return;
                    }
                    if (Boss > 0 &&  Family.ID == 40110)//sword house
                    {
                        const ushort GetStudyPoints = 2150;
                        killer.Player.ConquerPoints += 430;
                        //  killer.Player.Money += 10000000;
                        killer.Player.PIKAPoint += 1;
                        // killer.Inventory.Add(stream, 3004181);
                        //  killer.Inventory.Add(stream, 720957);
                        //killer.Player.InnerPower.AddPotency(stream, killer, 2000);
                        List<uint> DropIems = Family.ItemGenerator.GenerateSoulsItems(6);
                        MsgSchedules.SendSysMesage("" + killer.Player.Name + "  Kill   [ " + Name.ToString() + " ] 430 Cps   ", MsgServer.MsgMessage.ChatMode.BroadcastMessage, MsgServer.MsgMessage.MsgColor.white);
                        foreach (var ids in DropIems)
                        {
                            MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                            DataItem.ITEM_ID = ids;
                            Database.ItemType.DBItem DBItem;
                            if (Database.Server.ItemsBase.TryGetValue(ids, out DBItem))
                            {
                                DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                            }
                            DataItem.Color = Role.Flags.Color.Red;
                            ushort xx = X;
                            ushort yy = Y;
                            if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                            {
                                MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                                if (killer.Map.EnqueueItem(DropItem))
                                {
                                    DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                }
                            }
                        }
                        if (killer.Player.DynamicID != killer.Player.UID)
                        {
                            for (int x = 0; x < 24; x++)
                            {
                                uint id = Database.ItemType.DragonBallScroll;
                                if (x == 0)
                                    id = 730003;//Stone+3
                                if (x == 1)
                                    id = 3001044;//Stone+3
                                if (x == 2)
                                    id = 3001044;//Stone+3
                                if (x == 3)
                                    id = 1088000;//DB
                                if (x == 4)
                                    id = 1088000;//DB
                                if (x == 5)
                                    id = 3005129;//ChiPoints
                                if (x == 6)
                                    id = 3005129;//chiPoints
                                if (x == 7)
                                    id = 3002926;//Free Training Pill
                                if (x == 8)
                                    id = 3002926;//Free Training Pill
                                if (x == 9)
                                    id = 3002926;//Free Training Pill
                                if (x == 10)
                                    id = 3002926;//Free Training Pill
                                if (x == 11)
                                    id = 3002926;//Free Training Pill
                                if (x == 12)
                                    id = 3303373;//KnowledgePill
                                if (x == 13)
                                    id = 3303373;//KnowledgePill
                                if (x == 14)
                                    id = 3004181;//Savage
                                if (x == 15)
                                    id = 3004245;//Material  
                                if (x == 16)
                                    id = 3004245;//Material 
                                if (x == 17)
                                    id = 3004245;//Material 
                                if (x == 18)
                                    id = 3004245;//Material
                                if (x == 19)
                                    id = 3004245;//Material
                                if (x == 20)
                                    id = 720957;//soulP6Bag
                                if (x == 21)
                                    id = 3009003;//P.Stone
                                if (x == 22)
                                    id = 3009003;//P.Stone
                                if (x == 23)
                                    id = 3007110;//Inner Poit
                                if (x == 24)
                                    id = 3007110;//Inner Poit
                                MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                                DataItem.ITEM_ID = id;
                                Database.ItemType.DBItem DBItem;
                                if (Database.Server.ItemsBase.TryGetValue(id, out DBItem))
                                {
                                    DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                                }
                                DataItem.Color = Role.Flags.Color.Red;
                                ushort xx = (ushort)Program.GetRandom.Next(X - 4, X + 4);
                                ushort yy = (ushort)Program.GetRandom.Next(Y - 4, Y + 4);
                                if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                                {
                                    MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                                    if (killer.Map.EnqueueItem(DropItem))
                                    {

                                        DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                    }
                                }
                            }
                        }
                        if (killer.Team != null)
                        {
                            foreach (var member in killer.Team.Temates)
                            {
                                if (member != null && member.client != null && member.client.Player != null && member.client.Player.SubClass != null)
                                {
                                    if (member.client.Player.UID != killer.Player.UID)
                                    {
                                        member.client.Player.SubClass.AddStudyPoints(member.client, GetStudyPoints, stream);
                                    }
                                }
                            }
                        }
                        if (killer.Player.SubClass != null)
                            killer.Player.SubClass.AddStudyPoints(killer, GetStudyPoints, stream);
                        return;
                    }
                    if (Boss > 0 && Family.ID == 6643)
                    {
                        List<uint> DropIems = Family.ItemGenerator.GenerateSoulsItems(3);
                        foreach (var ids in DropIems)
                        {
                            MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                            DataItem.ITEM_ID = ids;
                            Database.ItemType.DBItem DBItem;
                            if (Database.Server.ItemsBase.TryGetValue(ids, out DBItem))
                            {
                                DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                            }
                            DataItem.Color = Role.Flags.Color.Red;
                            ushort xx = X;
                            ushort yy = Y;
                            if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                            {
                                MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                                if (killer.Map.EnqueueItem(DropItem))
                                {
                                    DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                }
                            }
                        }
                        return;
                    }
                    if ((Family.Settings & MonsterSettings.DropItemsOnDeath) == MonsterSettings.DropItemsOnDeath && Map != 3030)
                    {
                        {
                            if (killer.Player.VipLevel == 1)
                            {
                                uint rad = (uint)Program.GetRandom.Next(1, 8);
                                if (rad == 5)
                                {
                                    killer.Player.ConquerPoints += 3;
                                    killer.SendSysMesage("Congratulation You Have Get [ 3 ] CPS With Rate [12%]");
                                }
                            }
                            else if (killer.Player.VipLevel == 2)
                            {
                                uint rad = (uint)Program.GetRandom.Next(1, 6);
                                if (rad == 4)
                                {
                                    killer.Player.ConquerPoints += 3;
                                    killer.SendSysMesage("Congratulation You Have Get [ 3 ] CPS With Rate [16%]");
                                }
                            }
                            else if (killer.Player.VipLevel == 3)
                            {
                                uint rad = (uint)Program.GetRandom.Next(1, 4);
                                if (rad == 3)
                                {
                                    killer.Player.ConquerPoints += 3;
                                    killer.SendSysMesage("Congratulation You Have Get [ 3 ] CPS With Rate [25%]");
                                }
                            }
                            else if (killer.Player.VipLevel == 4)
                            {
                                uint rad = (uint)Program.GetRandom.Next(1, 4);
                                if (rad == 2|| rad== 3)
                                {
                                    killer.Player.ConquerPoints += 3;
                                    killer.SendSysMesage("Congratulation You Have Get [ 3 ] CPS With Rate [50%]");
                                }
                            }
                            else if (killer.Player.VipLevel == 5)
                            {
                                killer.Player.ConquerPoints += 3;
                                killer.SendSysMesage("Congratulation You Have Get [ 3 ] CPS With Rate [100%]");
                            }
                            else if (killer.Player.VipLevel == 6)
                            {
                                killer.Player.ConquerPoints += 5;
                                killer.SendSysMesage("Congratulation You Have Get [ 5 ] CPS With Rate [100%]");
                            }
                            else if (killer.Player.VipLevel > 6)
                            {
                                killer.Player.ConquerPoints += 7;
                                killer.SendSysMesage("Congratulation You Have Get [ 7 ] CPS With Rate [100%]");
                            }
                            if (Family.ID == 23973 || Family.ID == 22973 || Family.ID == 23974 || Family.ID == 3969)
                                return;
                            if (Family.MaxHealth > 100000 && Family.MaxHealth < 7000000 || Boss == 1)
                            {
                                List<uint> DropIems = Family.ItemGenerator.GenerateBossFamily();
                                foreach (var ids in DropIems)
                                {
                                    MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                                    DataItem.ITEM_ID = ids;
                                    Database.ItemType.DBItem DBItem;
                                    if (Database.Server.ItemsBase.TryGetValue(ids, out DBItem))
                                    {
                                        DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                                    }
                                    DataItem.Color = Role.Flags.Color.Red;
                                    ushort xx = X;
                                    ushort yy = Y;
                                    if (killer.Map.AddGroundItem(ref xx, ref yy))
                                    {
                                        MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                                        if (killer.Map.EnqueueItem(DropItem))
                                        {
                                            DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                        }
                                    }
                                }
                                return;
                            }
                            ushort rand = (ushort)(killer.Player.MyRandom.Next() % 1000);
                            byte count = 1;
                            if (rand > 40 && rand < 60)
                            {
                                ushort xx = X;
                                ushort yy = Y;
                                for (byte i = 0; i < count; i++)
                                {
                                    if (killer.Map.AddGroundItem(ref xx, ref yy))
                                    {
                                        uint ItemID = 0;
                                        uint Amount = Family.ItemGenerator.GenerateGold(out ItemID);
                                        if (killer.Player.Map == 1002 || killer.Player.Map == 1015)
                                        {
                                            if (killer.Player.VipLevel == 1)
                                            {
                                                DropItem(stream, killer.Player.UID, killer.Map, ItemID, xx, yy, MsgFloorItem.MsgItem.ItemType.Money, Amount, false, 0);
                                            }
                                           else if (killer.Player.VipLevel == 2)
                                            {
                                                uint rad = (uint)Program.GetRandom.Next(1, 10);
                                                if (rad == 8)
                                                {
                                                    killer.Player.Money += Amount;
                                                    killer.SendSysMesage("Congratulation You Have Collect Gold With Rate [10%]");
                                                }
                                                else
                                                {
                                                    DropItem(stream, killer.Player.UID, killer.Map, ItemID, xx, yy, MsgFloorItem.MsgItem.ItemType.Money, Amount, false, 0);
                                                }
                                            }
                                            else if (killer.Player.VipLevel == 3)
                                            {
                                                uint rad = (uint)Program.GetRandom.Next(1, 10);
                                                if (rad == 3 || rand == 6)
                                                {
                                                    killer.Player.Money += Amount;
                                                    killer.SendSysMesage("Congratulation You Have Collect Gold With Rate [20%]");
                                                }
                                                else
                                                {
                                                    DropItem(stream, killer.Player.UID, killer.Map, ItemID, xx, yy, MsgFloorItem.MsgItem.ItemType.Money, Amount, false, 0);
                                                }
                                            }
                                           else if (killer.Player.VipLevel == 4)
                                            {
                                                uint rad = (uint)Program.GetRandom.Next(1, 8);
                                                if (rad == 4 || rand == 7)
                                                {
                                                    killer.Player.Money += Amount;
                                                    killer.SendSysMesage("Congratulation You Have Collect Gold With Rate [25%]");
                                                }
                                                else
                                                {
                                                    DropItem(stream, killer.Player.UID, killer.Map, ItemID, xx, yy, MsgFloorItem.MsgItem.ItemType.Money, Amount, false, 0);
                                                }
                                            }
                                           else if (killer.Player.VipLevel == 5)
                                            {
                                                uint rad = (uint)Program.GetRandom.Next(1, 4);
                                                if (rad == 2 || rand == 3)
                                                {
                                                    killer.Player.Money += Amount;
                                                    killer.SendSysMesage("Congratulation You Have Collect Gold With Rate [50%]");
                                                }
                                                else
                                                {
                                                    DropItem(stream, killer.Player.UID, killer.Map, ItemID, xx, yy, MsgFloorItem.MsgItem.ItemType.Money, Amount, false, 0);
                                                }
                                            }
                                           else if (killer.Player.VipLevel >= 6)
                                            {
                                                killer.Player.Money += Amount;
                                                killer.SendSysMesage("Congratulation You Have Collect Gold With Rate [100%]");
                                            }
                                        }
                                        else
                                            DropItem(stream, killer.Player.UID, killer.Map, ItemID, xx, yy, MsgFloorItem.MsgItem.ItemType.Money, Amount, false, 0);
                                    }
                                }
                            }

                            else if (killer.Player.Map == 1002 || killer.Player.Map == 1015 || killer.Player.Map == 1011 || killer.Player.Map == 1020 || killer.Player.Map == 1000 || killer.Player.Map == 1001 || killer.Player.Map == 1351 || killer.Player.Map == 3031 || killer.Player.Map == 3036 || killer.Player.Map == 3037 || killer.Player.Map == 3038)
                            {
                                killer.Player.DBPoints += 1;
                                killer.Player.Bag15Cps += 1;
                                killer.Player.Bag10Cps += 1;
                                killer.Player.Bag5Cps += 1;
                                killer.Player.Bag3Cps += 1;
                                killer.Player.Bag1Cps += 1;
                                killer.Player.Chi200Points += 1;
                                killer.Player.Chi100Points += 1;
                                killer.Player.ExbBallPoints += 1;
                                if (killer.Player.DBPoints >= 3000)//Dragon Ball
                                {

                                    if (killer.Player.VipLevel == 1 )
                                    {

                                        ActionQuery action2;

                                        action2 = new ActionQuery()
                                        {
                                            ObjId = killer.Player.UID,
                                            Type = ActionType.DragonBall
                                        };
                                        killer.Send(stream.ActionCreate(&action2));
                                        DropItemID(killer, 1088000, stream, 5, 0);
                                        killer.Player.DBPoints = 0;
                                    }
                                    else if (killer.Player.VipLevel == 2)
                                    {
                                        uint rando = (uint)Program.GetRandom.Next(1, 5);
                                        if (rando == 3)
                                        {
                                            if (killer.Inventory.HaveSpace(1) && killer.Player.DBCollect == true)
                                            {

                                                ActionQuery action2;

                                                action2 = new ActionQuery()
                                                {
                                                    ObjId = killer.Player.UID,
                                                    Type = ActionType.DragonBall
                                                };
                                                killer.Send(stream.ActionCreate(&action2));
                                                killer.Inventory.Add(stream, 1088000);
                                                killer.Player.DBPoints = 0;
                                                killer.SendSysMesage("Congratulation You Have Get DragonBall In Items");
                                            }
                                            else
                                            {
                                                ActionQuery action2;

                                                action2 = new ActionQuery()
                                                {
                                                    ObjId = killer.Player.UID,
                                                    Type = ActionType.DragonBall
                                                };
                                                killer.Send(stream.ActionCreate(&action2));
                                                DropItemID(killer, 1088000, stream, 5, 0);
                                                killer.Player.DBPoints = 0;
                                            }
                                        }
                                        else
                                        {

                                            ActionQuery action2;

                                            action2 = new ActionQuery()
                                            {
                                                ObjId = killer.Player.UID,
                                                Type = ActionType.DragonBall
                                            };
                                            killer.Send(stream.ActionCreate(&action2));
                                            DropItemID(killer, 1088000, stream, 5, 0);
                                            killer.Player.DBPoints = 0;
                                        }
                                    }
                                    else if (killer.Player.VipLevel == 3)
                                    {
                                        uint rando = (uint)Program.GetRandom.Next(1, 8);
                                        if (rando == 4 || rando == 6)
                                        {
                                            if (killer.Inventory.HaveSpace(1) && killer.Player.DBCollect == true)
                                            {

                                                ActionQuery action2;

                                                action2 = new ActionQuery()
                                                {
                                                    ObjId = killer.Player.UID,
                                                    Type = ActionType.DragonBall
                                                };
                                                killer.Send(stream.ActionCreate(&action2));
                                                killer.Inventory.Add(stream, 1088000);
                                                killer.Player.DBPoints = 0;
                                                killer.SendSysMesage("Congratulation You Have Get DragonBall In Items");
                                            }
                                            else
                                            {

                                                ActionQuery action2;

                                                action2 = new ActionQuery()
                                                {
                                                    ObjId = killer.Player.UID,
                                                    Type = ActionType.DragonBall
                                                };
                                                killer.Send(stream.ActionCreate(&action2));
                                                DropItemID(killer, 1088000, stream, 5, 0);
                                                killer.Player.DBPoints = 0;
                                            }
                                        }
                                        else
                                        {

                                            ActionQuery action2;

                                            action2 = new ActionQuery()
                                            {
                                                ObjId = killer.Player.UID,
                                                Type = ActionType.DragonBall
                                            };
                                            killer.Send(stream.ActionCreate(&action2));
                                            DropItemID(killer, 1088000, stream, 5, 0);
                                            killer.Player.DBPoints = 0;
                                        }
                                    }
                                    else if (killer.Player.VipLevel == 4)
                                    {
                                        uint rando = (uint)Program.GetRandom.Next(1, 4);
                                        if (rando == 2|| rando == 3)
                                        {
                                            if (killer.Inventory.HaveSpace(1) && killer.Player.DBCollect == true)
                                            {

                                                ActionQuery action2;

                                                action2 = new ActionQuery()
                                                {
                                                    ObjId = killer.Player.UID,
                                                    Type = ActionType.DragonBall
                                                };
                                                killer.Send(stream.ActionCreate(&action2));
                                                killer.Inventory.Add(stream, 1088000);
                                                killer.Player.DBPoints = 0;
                                                killer.SendSysMesage("Congratulation You Have Get DragonBall In Items");
                                            }
                                            else
                                            {

                                                ActionQuery action2;

                                                action2 = new ActionQuery()
                                                {
                                                    ObjId = killer.Player.UID,
                                                    Type = ActionType.DragonBall
                                                };
                                                killer.Send(stream.ActionCreate(&action2));
                                                DropItemID(killer, 1088000, stream, 5, 0);
                                                killer.Player.DBPoints = 0;
                                            }
                                        }
                                        else
                                        {

                                            ActionQuery action2;

                                            action2 = new ActionQuery()
                                            {
                                                ObjId = killer.Player.UID,
                                                Type = ActionType.DragonBall
                                            };
                                            killer.Send(stream.ActionCreate(&action2));
                                            DropItemID(killer, 1088000, stream, 5, 0);
                                            killer.Player.DBPoints = 0;
                                        }
                                    }
                                    else if (killer.Player.VipLevel >= 5)
                                    {
                                        if (killer.Inventory.HaveSpace(1) && killer.Player.DBCollect == true)
                                        {

                                            ActionQuery action2;

                                            action2 = new ActionQuery()
                                            {
                                                ObjId = killer.Player.UID,
                                                Type = ActionType.DragonBall
                                            };
                                            killer.Send(stream.ActionCreate(&action2));
                                            killer.Inventory.Add(stream, 1088000);
                                            killer.Player.DBPoints = 0;
                                            killer.SendSysMesage("Congratulation You Have Get DragonBall In Items");
                                        }
                                        else
                                        {

                                            ActionQuery action2;

                                            action2 = new ActionQuery()
                                            {
                                                ObjId = killer.Player.UID,
                                                Type = ActionType.DragonBall
                                            };
                                            killer.Send(stream.ActionCreate(&action2));
                                            DropItemID(killer, 1088000, stream, 5, 0);
                                            killer.Player.DBPoints = 0;
                                        }
                                    }
                                }                               
                                else if (killer.Player.Bag15Cps >= 260)//15 Cps Bag
                                {
                                    if (killer.Player.VipLevel == 1)
                                    {
                                        DropItemID(killer, 3004036, stream, 5, 0);
                                        killer.Player.Bag15Cps = 0;
                                    }
                                    else if (killer.Player.VipLevel == 2)
                                    {
                                        uint rando = (uint)Program.GetRandom.Next(1, 5);
                                        if (rando == 3)
                                        {
                                            if (killer.Inventory.HaveSpace(1) && killer.Player.BagCpsCollet == true)
                                            {
                                                killer.Inventory.AddItemWitchStack(3004036, 0, 1, stream);
                                                killer.Player.Bag15Cps = 0;
                                                killer.SendSysMesage("Congratulation You Have Get [15] Cps-Bag In Items");
                                            }
                                            else
                                            {
                                                DropItemID(killer, 3004036, stream, 5, 0);
                                                killer.Player.Bag15Cps = 0;
                                            }
                                        }
                                        else
                                        {
                                            DropItemID(killer, 3004036, stream, 5, 0);
                                            killer.Player.Bag15Cps = 0;
                                        }
                                    }
                                    else if (killer.Player.VipLevel == 3)
                                    {
                                        uint rando = (uint)Program.GetRandom.Next(1, 8);
                                        if (rando == 4 || rando == 6)
                                        {
                                            if (killer.Inventory.HaveSpace(1) && killer.Player.BagCpsCollet == true)
                                            {
                                                killer.Inventory.AddItemWitchStack(3004036, 0, 1, stream);
                                                killer.Player.Bag15Cps = 0;
                                                killer.SendSysMesage("Congratulation You Have Get [15] Cps-Bag In Items");
                                            }
                                            else
                                            {
                                                DropItemID(killer, 3004036, stream, 5, 0);
                                                killer.Player.Bag15Cps = 0;
                                            }
                                        }
                                        else
                                        {
                                            DropItemID(killer, 3004036, stream, 5, 0);
                                            killer.Player.Bag15Cps = 0;
                                        }
                                    }
                                    else if (killer.Player.VipLevel == 4)
                                    {
                                        uint rando = (uint)Program.GetRandom.Next(1, 4);
                                        if (rando == 2|| rando == 3)
                                        {
                                            if (killer.Player.BagCpsCollet == true)
                                            {
                                                if (killer.Inventory.HaveSpace(1))
                                                {
                                                    killer.Inventory.AddItemWitchStack(3004036, 0, 1, stream);
                                                    killer.Player.Bag15Cps = 0;
                                                    killer.SendSysMesage("Congratulation You Have Get [15] Cps-Bag In Items");
                                                }
                                                else
                                                {
                                                    killer.Inventory.AddReturnedItem(stream, 3004036);
                                                    killer.Player.Bag15Cps = 0;
                                                }
                                            }
                                            else
                                            {
                                                DropItemID(killer, 3004036, stream, 5, 0);
                                                killer.Player.Bag15Cps = 0;
                                            }
                                        }
                                        else
                                        {
                                            DropItemID(killer, 3004036, stream, 5, 0);
                                            killer.Player.Bag15Cps = 0;
                                        }
                                    }
                                    else if (killer.Player.VipLevel >= 5)
                                    {
                                        if (killer.Player.BagCpsCollet == true)
                                        {
                                            if (killer.Inventory.HaveSpace(1))
                                            {
                                                killer.Inventory.AddItemWitchStack(3004036, 0, 1, stream);
                                                killer.Player.Bag15Cps = 0;
                                                killer.SendSysMesage("Congratulation You Have Get [15] Cps-Bag In Items");
                                            }
                                            else
                                            {
                                                killer.Inventory.AddReturnedItem(stream, 3004036);
                                                killer.Player.Bag15Cps = 0;
                                            }
                                        }
                                        else
                                        {
                                            DropItemID(killer, 3004036, stream, 5, 0);
                                            killer.Player.Bag15Cps = 0;
                                        }
                                    }
                                }
                                else if (killer.Player.Chi200Points >= 220)//200 Chi Points
                                {
                                    if (killer.Player.VipLevel == 1)
                                    {
                                        DropItemID(killer, 729477, stream, 5, 0);
                                        killer.Player.Chi200Points = 0;
                                    }
                                    else if (killer.Player.VipLevel == 2)
                                    {
                                        uint rando = (uint)Program.GetRandom.Next(1, 5);
                                        if (rando == 3)
                                        {
                                            if (killer.Inventory.HaveSpace(1) && killer.Player.chicollect == true)
                                            {
                                                killer.Inventory.AddItemWitchStack(729477, 0, 1, stream);
                                                killer.Player.Chi200Points = 0;
                                                killer.SendSysMesage("Congratulation You Have Get ChiPoints[200] In Items");
                                            }
                                            else
                                            {
                                                DropItemID(killer, 729477, stream, 5, 0);
                                                killer.Player.Chi200Points = 0;
                                            }
                                        }
                                        else
                                        {
                                            DropItemID(killer, 729477, stream, 5, 0);
                                            killer.Player.Chi200Points = 0;
                                        }
                                    }
                                    else if (killer.Player.VipLevel == 3)
                                    {
                                        uint rando = (uint)Program.GetRandom.Next(1, 8);
                                        if (rando == 4 || rando == 6)
                                        {
                                            if (killer.Inventory.HaveSpace(1) && killer.Player.chicollect == true)
                                            {
                                                killer.Inventory.AddItemWitchStack(729477, 0, 1, stream);
                                                killer.Player.Chi200Points = 0;
                                                killer.SendSysMesage("Congratulation You Have Get ChiPoints[200] In Items");
                                            }
                                            else
                                            {
                                                DropItemID(killer, 729477, stream, 5, 0);
                                                killer.Player.Chi200Points = 0;
                                            }
                                        }
                                        else
                                        {
                                            DropItemID(killer, 729477, stream, 5, 0);
                                            killer.Player.Chi200Points = 0;
                                        }
                                    }
                                    else if (killer.Player.VipLevel == 4)
                                    {
                                        uint rando = (uint)Program.GetRandom.Next(1, 4);
                                        if (rando == 2 || rando == 3)
                                        {
                                            if (killer.Player.chicollect == true)
                                            {
                                                if (killer.Inventory.HaveSpace(1))
                                                {
                                                    killer.Inventory.AddItemWitchStack(729477, 0, 1, stream);
                                                    killer.Player.Chi200Points = 0;
                                                    killer.SendSysMesage("Congratulation You Have Get ChiPoints[200] In Items");
                                                }
                                                else
                                                {
                                                    killer.Inventory.AddReturnedItem(stream, 729477);
                                                    killer.Player.Chi200Points = 0;
                                                }
                                            }
                                            else
                                            {
                                                DropItemID(killer, 729477, stream, 5, 0);
                                                killer.Player.Chi200Points = 0;
                                            }
                                        }
                                        else
                                        {
                                            DropItemID(killer, 729477, stream, 5, 0);
                                            killer.Player.Chi200Points = 0;
                                        }
                                    }
                                    else if (killer.Player.VipLevel >= 5)
                                    {
                                        if (killer.Player.chicollect == true)
                                        {
                                            if (killer.Inventory.HaveSpace(1))
                                            {
                                                killer.Inventory.AddItemWitchStack(729477, 0, 1, stream);
                                                killer.Player.Chi200Points = 0;
                                                killer.SendSysMesage("Congratulation You Have Get ChiPoints[200] In Items");
                                            }
                                            else
                                            {
                                                killer.Inventory.AddReturnedItem(stream, 729477);
                                                killer.Player.Chi200Points = 0;
                                            }
                                        }
                                        else
                                        {
                                            DropItemID(killer, 729477, stream, 5, 0);
                                            killer.Player.Chi200Points = 0;
                                        }
                                    }
                                }
                                else if (killer.Player.Bag10Cps >= 180)//10 Cps Bag
                                {
                                    if (killer.Player.VipLevel == 1)
                                    {
                                        DropItemID(killer, 3004035, stream, 5, 0);
                                        killer.Player.Bag10Cps = 0;
                                    }
                                    else if (killer.Player.VipLevel == 2)
                                    {
                                        uint rando = (uint)Program.GetRandom.Next(1, 5);
                                        if (rando == 3)
                                        {
                                            if (killer.Inventory.HaveSpace(1) && killer.Player.BagCpsCollet == true)
                                            {
                                                killer.Inventory.AddItemWitchStack(3004035, 0, 1, stream);
                                                killer.Player.Bag10Cps = 0;
                                                killer.SendSysMesage("Congratulation You Have Get [10] Cps-Bag In Items");
                                            }
                                            else
                                            {
                                                DropItemID(killer, 3004035, stream, 5, 0);
                                                killer.Player.Bag10Cps = 0;
                                            }
                                        }
                                        else
                                        {
                                            DropItemID(killer, 3004035, stream, 5, 0);
                                            killer.Player.Bag10Cps = 0;
                                        }
                                    }
                                    else if (killer.Player.VipLevel == 3)
                                    {
                                        uint rando = (uint)Program.GetRandom.Next(1, 8);
                                        if (rando == 4 || rando == 6)
                                        {
                                            if (killer.Inventory.HaveSpace(1) && killer.Player.BagCpsCollet == true)
                                            {
                                                killer.Inventory.AddItemWitchStack(3004035, 0, 1, stream);
                                                killer.Player.Bag10Cps = 0;
                                                killer.SendSysMesage("Congratulation You Have Get [10] Cps-Bag In Items");
                                            }
                                            else
                                            {
                                                DropItemID(killer, 3004035, stream, 5, 0);
                                                killer.Player.Bag10Cps = 0;
                                            }
                                        }
                                        else
                                        {
                                            DropItemID(killer, 3004035, stream, 5, 0);
                                            killer.Player.Bag10Cps = 0;
                                        }
                                    }
                                    else if (killer.Player.VipLevel == 4)
                                    {
                                        uint rando = (uint)Program.GetRandom.Next(1, 4);
                                        if (rando == 2 || rando == 3)
                                        {
                                            if (killer.Player.BagCpsCollet == true)
                                            {
                                                if (killer.Inventory.HaveSpace(1))
                                                {
                                                    killer.Inventory.AddItemWitchStack(3004035, 0, 1, stream);
                                                    killer.Player.Bag10Cps = 0;
                                                    killer.SendSysMesage("Congratulation You Have Get [10] Cps-Bag In Items");
                                                }
                                                else
                                                {
                                                    killer.Inventory.AddReturnedItem(stream, 3004035);
                                                    killer.Player.Bag10Cps = 0;
                                                }
                                            }
                                            else
                                            {
                                                DropItemID(killer, 3004035, stream, 5, 0);
                                                killer.Player.Bag10Cps = 0;
                                            }
                                        }
                                        else
                                        {
                                            DropItemID(killer, 3004035, stream, 5, 0);
                                            killer.Player.Bag10Cps = 0;
                                        }
                                    }
                                    else if (killer.Player.VipLevel >= 5)
                                    {
                                        if (killer.Player.BagCpsCollet == true)
                                        {
                                            if (killer.Inventory.HaveSpace(1) && killer.Player.BagCpsCollet == true)
                                            {
                                                killer.Inventory.AddItemWitchStack(3004035, 0, 1, stream);
                                                killer.Player.Bag10Cps = 0;
                                                killer.SendSysMesage("Congratulation You Have Get [10] Cps-Bag In Items");
                                            }
                                            else
                                            {
                                                killer.Inventory.AddReturnedItem(stream, 3004035);
                                                killer.Player.Bag10Cps = 0;
                                            }
                                        }
                                        else
                                        {
                                            DropItemID(killer, 3004035, stream, 5, 0);
                                            killer.Player.Bag10Cps = 0;
                                        }
                                    }
                                }
                                else if (killer.Player.Chi100Points >= 160)//100 Chi Points
                                {
                                    if (killer.Player.VipLevel == 1)
                                    {
                                        DropItemID(killer, 729476, stream, 5, 0);
                                        killer.Player.Chi100Points = 0;
                                    }
                                    else if (killer.Player.VipLevel == 2)
                                    {
                                        uint rando = (uint)Program.GetRandom.Next(1, 5);
                                        if (rando == 3)
                                        {
                                            if (killer.Inventory.HaveSpace(1) && killer.Player.chicollect == true)
                                            {
                                                killer.Inventory.AddItemWitchStack(729476, 0, 1, stream);
                                                killer.Player.Chi100Points = 0;
                                                killer.SendSysMesage("Congratulation You Have Get ChiPoints[100] In Items");
                                            }
                                            else
                                            {
                                                DropItemID(killer, 729476, stream, 5, 0);
                                                killer.Player.Chi100Points = 0;
                                            }
                                        }
                                        else
                                        {
                                            DropItemID(killer, 729476, stream, 5, 0);
                                            killer.Player.Chi100Points = 0;
                                        }
                                    }
                                    else if (killer.Player.VipLevel == 3)
                                    {
                                        uint rando = (uint)Program.GetRandom.Next(1, 8);
                                        if (rando == 4 || rando == 6)
                                        {
                                            if (killer.Inventory.HaveSpace(1) && killer.Player.chicollect == true)
                                            {
                                                killer.Inventory.AddItemWitchStack(729476, 0, 1, stream);
                                                killer.Player.Chi100Points = 0;
                                                killer.SendSysMesage("Congratulation You Have Get ChiPoints[100] In Items");
                                            }
                                            else
                                            {
                                                DropItemID(killer, 729476, stream, 5, 0);
                                                killer.Player.Chi100Points = 0;
                                            }
                                        }
                                        else
                                        {
                                            DropItemID(killer, 729476, stream, 5, 0);
                                            killer.Player.Chi100Points = 0;
                                        }
                                    }
                                    else if (killer.Player.VipLevel == 4)
                                    {
                                        uint rando = (uint)Program.GetRandom.Next(1, 4);
                                        if (rando == 2 || rando == 3)
                                        {
                                            if (killer.Player.chicollect == true)
                                            {
                                                if (killer.Inventory.HaveSpace(1))
                                                {
                                                    killer.Inventory.AddItemWitchStack(729476, 0, 1, stream);
                                                    killer.Player.Chi100Points = 0;
                                                    killer.SendSysMesage("Congratulation You Have Get ChiPoints[100] In Items");
                                                }
                                                else
                                                {
                                                    killer.Inventory.AddReturnedItem(stream, 729476);
                                                    killer.Player.Chi100Points = 0;
                                                }
                                            }
                                            else
                                            {
                                                DropItemID(killer, 729476, stream, 5, 0);
                                                killer.Player.Chi100Points = 0;
                                            }
                                        }
                                        else
                                        {
                                            DropItemID(killer, 729476, stream, 5, 0);
                                            killer.Player.Chi100Points = 0;
                                        }
                                    }
                                    else if (killer.Player.VipLevel >= 5)
                                    {
                                        if (killer.Player.chicollect == true)
                                        {
                                            if (killer.Inventory.HaveSpace(1) && killer.Player.chicollect == true)
                                            {
                                                killer.Inventory.AddItemWitchStack(729476, 0, 1, stream);
                                                killer.Player.Chi100Points = 0;
                                                killer.SendSysMesage("Congratulation You Have Get ChiPoints[100] In Items");
                                            }
                                            else
                                            {
                                                killer.Inventory.AddReturnedItem(stream, 729476);
                                                killer.Player.Chi100Points = 0;
                                            }
                                        }
                                        else
                                        {
                                            DropItemID(killer, 729476, stream, 5, 0);
                                            killer.Player.Chi100Points = 0;
                                        }
                                    }
                                }
                                else if (killer.Player.Bag5Cps >= 140)//5 Cps Bag
                                {
                                    if (killer.Player.VipLevel == 1)
                                    {
                                        DropItemID(killer, 727839, stream, 5, 0);
                                        killer.Player.Bag5Cps = 0;
                                    }
                                    else if (killer.Player.VipLevel == 2)
                                    {
                                        uint rando = (uint)Program.GetRandom.Next(1, 5);
                                        if (rando == 3)
                                        {
                                            if (killer.Inventory.HaveSpace(1) && killer.Player.BagCpsCollet == true)
                                            {
                                                killer.Inventory.AddItemWitchStack(727839, 0, 1, stream);
                                                killer.Player.Bag5Cps = 0;
                                                killer.SendSysMesage("Congratulation You Have Get [5] Cps-Bag In Items");
                                            }
                                            else
                                            {
                                                DropItemID(killer, 727839, stream, 5, 0);
                                                killer.Player.Bag5Cps = 0;
                                            }
                                        }
                                        else
                                        {
                                            DropItemID(killer, 727839, stream, 5, 0);
                                            killer.Player.Bag5Cps = 0;
                                        }
                                    }
                                    else if (killer.Player.VipLevel == 3)
                                    {
                                        uint rando = (uint)Program.GetRandom.Next(1, 8);
                                        if (rando == 4 || rando == 6)
                                        {
                                            if (killer.Inventory.HaveSpace(1) && killer.Player.BagCpsCollet == true)
                                            {
                                                killer.Inventory.AddItemWitchStack(727839, 0, 1, stream);
                                                killer.Player.Bag5Cps = 0;
                                                killer.SendSysMesage("Congratulation You Have Get [5] Cps-Bag In Items");
                                            }
                                            else
                                            {
                                                DropItemID(killer, 727839, stream, 5, 0);
                                                killer.Player.Bag5Cps = 0;
                                            }
                                        }
                                        else
                                        {
                                            DropItemID(killer, 727839, stream, 5, 0);
                                            killer.Player.Bag5Cps = 0;
                                        }
                                    }
                                    else if (killer.Player.VipLevel == 4)
                                    {
                                        uint rando = (uint)Program.GetRandom.Next(1, 4);
                                        if (rando == 2 || rando ==3)
                                        {
                                            if (killer.Player.BagCpsCollet == true)
                                            {
                                                if (killer.Inventory.HaveSpace(1))
                                                {
                                                    killer.Inventory.AddItemWitchStack(727839, 0, 1, stream);
                                                    killer.Player.Bag5Cps = 0;
                                                    killer.SendSysMesage("Congratulation You Have Get [5] Cps-Bag In Items");
                                                }
                                                else
                                                {
                                                    killer.Inventory.AddReturnedItem(stream, 727839);
                                                    killer.Player.Bag5Cps = 0;
                                                }
                                            }
                                            else
                                            {
                                                DropItemID(killer, 727839, stream, 5, 0);
                                                killer.Player.Bag5Cps = 0;
                                            }
                                        }
                                        else
                                        {
                                            DropItemID(killer, 727839, stream, 5, 0);
                                            killer.Player.Bag5Cps = 0;
                                        }
                                    }
                                    else if (killer.Player.VipLevel >= 5)
                                    {
                                        if (killer.Player.BagCpsCollet == true)
                                        {
                                            if (killer.Inventory.HaveSpace(1))
                                            {
                                                killer.Inventory.AddItemWitchStack(727839, 0, 1, stream);
                                                killer.Player.Bag5Cps = 0;
                                                killer.SendSysMesage("Congratulation You Have Get [5] Cps-Bag In Items");
                                            }
                                            else
                                            {
                                                killer.Inventory.AddReturnedItem(stream, 727839);
                                                killer.Player.Bag5Cps = 0;
                                            }
                                        }
                                        else
                                        {
                                            DropItemID(killer, 727839, stream, 5, 0);
                                            killer.Player.Bag5Cps = 0;
                                        }
                                    }
                                }                              
                                else if (killer.Player.Bag3Cps >= 80)// 3 Cps Bag
                                {
                                    if (killer.Player.VipLevel == 1)
                                    {
                                        DropItemID(killer, 727840, stream, 5, 0);
                                        killer.Player.Bag3Cps = 0;
                                    }
                                    else if (killer.Player.VipLevel == 2)
                                    {
                                        uint rando = (uint)Program.GetRandom.Next(1, 5);
                                        if (rando == 3)
                                        {
                                            if (killer.Inventory.HaveSpace(1) && killer.Player.BagCpsCollet == true)
                                            {
                                                killer.Inventory.AddItemWitchStack(727840, 0, 1, stream);
                                                killer.Player.Bag3Cps = 0;
                                                killer.SendSysMesage("Congratulation You Have Get [3] Cps-Bag In Items");
                                            }
                                            else
                                            {
                                                DropItemID(killer, 727840, stream, 5, 0);
                                                killer.Player.Bag3Cps = 0;
                                            }
                                        }
                                        else
                                        {
                                            DropItemID(killer, 727840, stream, 5, 0);
                                            killer.Player.Bag3Cps = 0;
                                        }
                                    }
                                    else if (killer.Player.VipLevel == 3)
                                    {
                                        uint rando = (uint)Program.GetRandom.Next(1, 8);
                                        if (rando == 4 || rando == 6)
                                        {
                                            if (killer.Inventory.HaveSpace(1) && killer.Player.BagCpsCollet == true)
                                            {
                                                killer.Inventory.AddItemWitchStack(727840, 0, 1, stream);
                                                killer.Player.Bag3Cps = 0;
                                                killer.SendSysMesage("Congratulation You Have Get [3] Cps-Bag In Items");
                                            }
                                            else
                                            {
                                                DropItemID(killer, 727840, stream, 5, 0);
                                                killer.Player.Bag3Cps = 0;
                                            }
                                        }
                                        else
                                        {
                                            DropItemID(killer, 727840, stream, 5, 0);
                                            killer.Player.Bag3Cps = 0;
                                        }
                                    }
                                    else if (killer.Player.VipLevel == 4)
                                    {
                                        uint rando = (uint)Program.GetRandom.Next(1, 4);
                                        if (rando == 2 || rando == 3)
                                        {
                                            if (killer.Player.BagCpsCollet == true)
                                            {
                                                if (killer.Inventory.HaveSpace(1))
                                                {
                                                    killer.Inventory.AddItemWitchStack(727840, 0, 1, stream);
                                                    killer.Player.Bag3Cps = 0;
                                                    killer.SendSysMesage("Congratulation You Have Get [3] Cps-Bag In Items");
                                                }
                                                else
                                                {
                                                    killer.Inventory.AddReturnedItem(stream, 727840);
                                                    killer.Player.Bag3Cps = 0;
                                                }
                                            }
                                            else
                                            {
                                                DropItemID(killer, 727840, stream, 5, 0);
                                                killer.Player.Bag3Cps = 0;
                                            }
                                        }
                                        else
                                        {
                                            DropItemID(killer, 727840, stream, 5, 0);
                                            killer.Player.Bag3Cps = 0;
                                        }
                                    }
                                    else if (killer.Player.VipLevel >= 5)
                                    {
                                        if (killer.Player.BagCpsCollet == true)
                                        {
                                            if (killer.Inventory.HaveSpace(1))
                                            {
                                                killer.Inventory.AddItemWitchStack(727840, 0, 1, stream);
                                                killer.Player.Bag3Cps = 0;
                                                killer.SendSysMesage("Congratulation You Have Get [3] Cps-Bag In Items");
                                            }
                                            else
                                            {
                                                killer.Inventory.AddReturnedItem(stream, 727840);
                                                killer.Player.Bag3Cps = 0;
                                            }
                                        }
                                        else
                                        {
                                            DropItemID(killer, 727840, stream, 5, 0);
                                            killer.Player.Bag3Cps = 0;
                                        }
                                    }

                                }
                                else if (killer.Player.Bag1Cps >= 50)
                                {
                                    if (killer.Player.VipLevel == 1)
                                    {
                                        DropItemID(killer, 727841, stream, 5, 0);
                                        killer.Player.Bag1Cps = 0;
                                    }
                                    else if (killer.Player.VipLevel == 2)
                                    {
                                        uint rando = (uint)Program.GetRandom.Next(1, 5);
                                        if (rando == 3)
                                        {
                                            if (killer.Inventory.HaveSpace(1) && killer.Player.BagCpsCollet == true)
                                            {
                                                killer.Inventory.AddItemWitchStack(727841, 0, 1, stream);
                                                killer.Player.Bag1Cps = 0;
                                                killer.SendSysMesage("Congratulation You Have Get [1] Cps-Bag In Items");
                                            }
                                            else
                                            {
                                                DropItemID(killer, 727841, stream, 5, 0);
                                                killer.Player.Bag1Cps = 0;
                                            }
                                        }
                                        else
                                        {
                                            DropItemID(killer, 727841, stream, 5, 0);
                                            killer.Player.Bag1Cps = 0;
                                        }
                                    }
                                    else if (killer.Player.VipLevel == 3)
                                    {
                                        uint rando = (uint)Program.GetRandom.Next(1, 8);
                                        if (rando == 4 || rando == 6)
                                        {
                                            if (killer.Inventory.HaveSpace(1) && killer.Player.BagCpsCollet == true)
                                            {
                                                killer.Inventory.AddItemWitchStack(727841, 0, 1, stream);
                                                killer.Player.Bag1Cps = 0;
                                                killer.SendSysMesage("Congratulation You Have Get [1] Cps-Bag In Items");
                                            }
                                            else
                                            {
                                                DropItemID(killer, 727841, stream, 5, 0);
                                                killer.Player.Bag1Cps = 0;
                                            }
                                        }
                                        else
                                        {
                                            DropItemID(killer, 727841, stream, 5, 0);
                                            killer.Player.Bag1Cps = 0;
                                        }
                                    }
                                    else if (killer.Player.VipLevel == 4)
                                    {
                                        uint rando = (uint)Program.GetRandom.Next(1, 4);
                                        if (rando == 2 || rando == 3)
                                        {
                                            if (killer.Player.BagCpsCollet == true)
                                            {
                                                if (killer.Inventory.HaveSpace(1))
                                                {
                                                    killer.Inventory.AddItemWitchStack(727841, 0, 1, stream);
                                                    killer.Player.Bag1Cps = 0;
                                                    killer.SendSysMesage("Congratulation You Have Get [1] Cps-Bag In Items");
                                                }
                                                else
                                                {
                                                    killer.Inventory.AddReturnedItem(stream, 720027);
                                                    killer.Player.Bag1Cps = 0;
                                                }
                                            }
                                            else
                                            {
                                                DropItemID(killer, 727841, stream, 5, 0);
                                                killer.Player.Bag1Cps = 0;
                                            }
                                        }
                                        else
                                        {
                                            DropItemID(killer, 727841, stream, 5, 0);
                                            killer.Player.Bag1Cps = 0;
                                        }
                                    }
                                    else if (killer.Player.VipLevel >= 5)
                                    {
                                        if (killer.Player.BagCpsCollet == true)
                                        {
                                            if (killer.Inventory.HaveSpace(1))
                                            {
                                                killer.Inventory.AddItemWitchStack(727841, 0, 1, stream);
                                                killer.Player.Bag1Cps = 0;
                                                killer.SendSysMesage("Congratulation You Have Get [1] Cps-Bag In Items");
                                            }
                                            else
                                            {
                                                killer.Inventory.AddReturnedItem(stream, 720027);
                                                killer.Player.Bag1Cps = 0;
                                            }
                                        }
                                        else
                                        {
                                            DropItemID(killer, 727841, stream, 5, 0);
                                            killer.Player.Bag1Cps = 0;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }


        private void DropItem(ServerSockets.Packet stream, uint OwnerItem, Role.GameMap map, uint ItemID, ushort XX, ushort YY, MsgFloorItem.MsgItem.ItemType typ
            , uint amount, bool special, byte ID_Quality, Client.GameClient user = null, Database.ItemType.DBItem DBItem = null)
        {
            MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();

            DataItem.ITEM_ID = ItemID;
            if (DataItem.Durability > 100)
            {
                DataItem.Durability = (ushort)Program.GetRandom.Next(100, DataItem.Durability / 10);
                DataItem.MaximDurability = DataItem.Durability;
            }

            else
            {
                DataItem.Durability = (ushort)Program.GetRandom.Next(1, 10);
                DataItem.MaximDurability = 10;
            }

            DataItem.Color = Role.Flags.Color.Red;
            if (typ == MsgFloorItem.MsgItem.ItemType.Item)
            {
                byte sockets;
                bool lucky = false;
                if (DataItem.IsEquip)
                {
                    if (!special)
                    {

                        lucky = (ID_Quality > 7); // q>unique
                        if (!lucky)
                            lucky = (DataItem.Plus = Family.ItemGenerator.GeneratePurity()) != 0;
                        if (!lucky)
                            lucky = (DataItem.Bless = Family.ItemGenerator.GenerateBless()) != 0;
                        if (!lucky)
                        {
                            if (DataItem.IsWeapon)
                            {
                                sockets = Family.ItemGenerator.GenerateSocketCount(DataItem.ITEM_ID);

                                if (sockets >= 1)
                                    DataItem.SocketOne = Role.Flags.Gem.EmptySocket;
                                else if (sockets == 2)
                                    DataItem.SocketTwo = Role.Flags.Gem.EmptySocket;
                            }
                        }
                    }
                    if (DBItem != null)
                    {
                        DataItem.Durability = (ushort)Program.GetRandom.Next(1, DBItem.Durability / 10 + 10);
                        DataItem.MaximDurability = (ushort)Program.GetRandom.Next(DataItem.Durability, DBItem.Durability);
                    }
                }
                else
                {
                    if (DBItem != null)
                        DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                }
            }
            if (user != null)
            {
                if (user.Player.VipLevel == 6)
                {
                    /* if (DataItem.IsEquip)
                     {
                         if (DataItem.Plus > 0)
                         {
                             user.Inventory.Update(DataItem, Role.Instance.AddMode.ADD, stream);
                             return;
                         }
                     }
                     else*/
                    if (DataItem.ITEM_ID == 1088000)
                    {
                        user.Inventory.Update(DataItem, Role.Instance.AddMode.ADD, stream);
                        return;
                    }
                }
            }

            MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, XX, YY, typ, amount, DynamicID, Map, OwnerItem, true, map);

            if (map.EnqueueItem(DropItem))
            {
                DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
            }
        }
        public void AddFadeAway(int time, Role.GameMap map)
        {
            if (!Alive)
            {
                Extensions.Time32 timer = new Extensions.Time32(time);
                if (timer > DeadStamp.AddSeconds(5))
                {
                    if (AddFlag(MsgServer.MsgUpdate.Flags.FadeAway, Role.StatusFlagsBigVector32.PermanentFlag, true))
                    {
                        FadeAway = timer;

                    }
                }
            }
        }
        public unsafe bool RemoveView(int time, Role.GameMap map)
        {
         if (ContainFlag(MsgServer.MsgUpdate.Flags.FadeAway) && State != MobStatus.Respawning)
            {
                Extensions.Time32 timer = new Extensions.Time32(time);
                if (timer > FadeAway.AddSeconds(3))
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();

                        ActionQuery action;

                        action = new ActionQuery()
                        {
                            ObjId = UID,
                            Type = ActionType.RemoveEntity
                        };

                        Send(stream.ActionCreate(&action));
                    }

                    State = MobStatus.Respawning;

                    map.View.MoveTo<Role.IMapObj>(this, RespawnX, RespawnY);

                    X = RespawnX;
                    Y = RespawnY;
                    Target = null;

                    return true;
                }
            }
            return false;
        }

        public void DropItemID(Client.GameClient killer, uint itemid, ServerSockets.Packet stream, byte range = 3, int mins = 0)
        {
            MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
            DataItem.ITEM_ID = itemid;
            Database.ItemType.DBItem DBItem;
            if (Database.Server.ItemsBase.TryGetValue(itemid, out DBItem))
            {
                DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
            }
            if (mins > 0)
            {
                DataItem.EndDate = DateTime.Now.AddMinutes(mins);
            }
            DataItem.Color = Role.Flags.Color.Red;
            ushort xx = X;
            ushort yy = Y;
            if (killer.Map.AddGroundItem(ref xx, ref yy, range))
            {
                MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                if (killer.Map.EnqueueItem(DropItem))
                {
                    DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                }
            }
        }
        public MonsterFamily Family;
        public MonsterView View;
        public MobStatus State;
        public Role.Player Target = null;
        public Extensions.Time32 AttackSpeed = new Extensions.Time32();

        public Role.StatusFlagsBigVector32 BitVector;
        public void AddSpellFlag(Game.MsgServer.MsgUpdate.Flags Flag, int Secounds, bool RemoveOnDead, int SecoundStamp = 0)
        {
            if (BitVector.ContainFlag((int)Flag))
                BitVector.TryRemove((int)Flag);
            AddFlag(Flag, Secounds, RemoveOnDead, SecoundStamp);
        }
        public bool AddFlag(Game.MsgServer.MsgUpdate.Flags Flag, int Secounds, bool RemoveOnDead, int StampSecounds = 0)
        {
            if (!BitVector.ContainFlag((int)Flag))
            {
                BitVector.TryAdd((int)Flag, Secounds, RemoveOnDead, StampSecounds);
                UpdateFlagOffset();
                return true;
            }
            return false;
        }
        public bool RemoveFlag(Game.MsgServer.MsgUpdate.Flags Flag, Role.GameMap map)
        {
            if (BitVector.ContainFlag((int)Flag))
            {
                BitVector.TryRemove((int)Flag);
                UpdateFlagOffset();
                return true;
            }
            return false;
        }
        public bool UpdateFlag(Game.MsgServer.MsgUpdate.Flags Flag, int Secounds, bool SetNewTimer, int MaxTime)
        {
            return BitVector.UpdateFlag((int)Flag, Secounds, SetNewTimer, MaxTime);
        }
        public void ClearFlags(bool SendScreem = false)
        {
            BitVector.GetClear();
            UpdateFlagOffset(SendScreem);
        }
        public bool ContainFlag(Game.MsgServer.MsgUpdate.Flags Flag)
        {
            return BitVector.ContainFlag((int)Flag);
        }
        private unsafe void UpdateFlagOffset(bool SendScreem = true)
        {
            if (SendScreem)
                SendUpdate(BitVector.bits, Game.MsgServer.MsgUpdate.DataType.StatusFlag);
        }

        public byte OpenBoss = 0;
        public uint Map { get; set; }
        public uint DynamicID { get; set; }

        public short GetMyDistance(ushort X2, ushort Y2)
        {
            return Role.Core.GetDistance(X, Y, X2, Y2);
        }
        public short OldGetDistance(ushort X2, ushort Y2)
        {
            return Role.Core.GetDistance(PX, PY, X2, Y2);
        }
        public bool InView(ushort X2, ushort Y2, byte distance)
        {
            return (!(OldGetDistance(X2, Y2) < distance) && GetMyDistance(X2, Y2) < distance);
        }


        public unsafe void Send(ServerSockets.Packet msg)
        {
            View.SendScreen(msg, GMap);
            //SendScores(msg);
        }
        public void UpdateMonsterView(Role.RoleView Target, ServerSockets.Packet stream)
        {
            foreach (var player in View.Roles(GMap, Role.MapObjectType.Player))
            {
                if (InView(player.X, player.Y, MonsterView.ViewThreshold))
                    player.Send(GetArray(stream, false));
            }
        }
        public bool UpdateMapCoords(ushort New_X, ushort New_Y, Role.GameMap _map)
        {
            if (!_map.IsFlagPresent(New_X, New_Y, Role.MapFlagType.Monster))
            {
                _map.SetMonsterOnTile(X, Y, false);
                _map.SetMonsterOnTile(New_X, New_Y, true);
                _map.View.MoveTo<MonsterRole>(this, New_X, New_Y);
                X = New_X;
                Y = New_Y;
                return true;
            }
            return false;
        }
        public void RemoveRole(Role.IMapObj obj)
        {

        }
        public Role.MapObjectType ObjType { get; set; }
        public unsafe string Name = "";

        public byte Boss = 0;
        public uint Mesh = 0;
        public uint UID { get; set; }
        public byte Level = 0;
        public uint HitPoints;

        public ushort RespawnX;
        public ushort RespawnY;


        public ushort PX = 0;
        public ushort PY = 0;
        public ushort _xx;
        public ushort _yy;

        public ushort X { get { return _xx; } set { PX = _xx; _xx = value; } }
        public ushort Y { get { return _yy; } set { PY = _yy; _yy = value; } }
        public Role.Flags.ConquerAction Action = Role.Flags.ConquerAction.None;
        public Role.Flags.ConquerAngle Facing = Role.Flags.ConquerAngle.East;
        public string LocationSpawn = "";
        public Role.GameMap GMap;
        public bool RemoveOnDead = false;
        public uint PetFlag = 0;


        public unsafe void SendString(ServerSockets.Packet stream, Game.MsgServer.MsgStringPacket.StringID id, params string[] args)
        {
            Game.MsgServer.MsgStringPacket packet = new Game.MsgServer.MsgStringPacket();
            packet.ID = id;
            packet.UID = UID;

            packet.Strings = args;
            Send(stream.StringPacketCreate(packet));
        }
        public MonsterRole(MonsterFamily Famil, uint _UID, string locationspawn, Role.GameMap _map)
        {
            AllowDynamic = false;
            GMap = _map;
            LocationSpawn = locationspawn;
            ObjType = Role.MapObjectType.Monster;
            Name = Famil.Name;
            Family = Famil;
            UID = _UID;
            Mesh = Famil.Mesh;
            Level = (byte)Famil.Level;
            HitPoints = (uint)Famil.MaxHealth;
            View = new MonsterView(this);
            State = MobStatus.Idle;
            BitVector = new Role.StatusFlagsBigVector32(32 * 7);//5
            Boss = Family.Boss;
            Facing = (Role.Flags.ConquerAngle)Program.GetRandom.Next(0, 8);

        }
        public uint CountHitTarget;
        public uint UIDTarget;
        public uint MaxCountHitTarget = (uint)Role.Core.Random.Next(8, 20);
        public void HitTargetState(bool create = false)
        {
            if (create)
            {
                CountHitTarget = 0;
                UIDTarget = 0;
                MaxCountHitTarget = (uint)Role.Core.Random.Next(12, 20);
            }
            else
            {
                if (Target != null)
                {
                    CountHitTarget++;
                    if (CountHitTarget > MaxCountHitTarget)
                    {
                        UIDTarget = Target.UID;
                        State = MobStatus.MoveNext;
                    }
                }
            }
        }
        public bool Alive { get { return HitPoints > 0; } }


        public unsafe ServerSockets.Packet GetArray(ServerSockets.Packet stream, bool view)
        {
            if (IsFloor && Mesh != 980)
            {
                return stream.ItemPacketCreate(this.FloorPacket);

            }
            stream.InitWriter();

            stream.Write(Extensions.Time32.Now.Value);
            stream.Write(Mesh);
            stream.Write(UID);
            stream.ZeroFill(10);

            for (int x = 0; x < BitVector.bits.Length; x++)
                stream.Write(BitVector.bits[x]);



            stream.ZeroFill(57);



            if (Boss > 0)
            {
                if (IsFloor)
                {
                    stream.Write(StampFloorSecounds);
                }
                else
                {
                    uint key = (uint)(Family.MaxHealth / 10000);
                    if (key != 0)
                        stream.Write((uint)(HitPoints / key));
                    else
                        stream.Write((uint)(HitPoints * Family.MaxHealth));
                }
            }
            else
            {
                if (IsFloor)
                {
                    stream.Write(StampFloorSecounds);
                }
                else
                    stream.Write(HitPoints);
            }
            stream.Write((ushort)0);
            stream.Write((ushort)Level);


            stream.Write(X);
            stream.Write(Y);
            stream.Write((ushort)0);
            stream.Write((byte)Facing);
            stream.Write((byte)Action);
            stream.ZeroFill(93);

            stream.Write((byte)Boss);
            stream.ZeroFill(50);




            if (IsFloor)
            {
                stream.Write((ushort)FloorPacket.m_ID);
                stream.Write((byte)0);
                stream.Write((uint)(OwnerFloor.Player.UID));
                stream.Write((ushort)9);
            }
            else
            {
                stream.ZeroFill(7);
                stream.Write((ushort)0);
            }


            stream.Write(0);
            stream.Write(0);

            stream.Write(0);
            //

            stream.Write(0);
            stream.Write(0);
            stream.Write(0);

            stream.Write(0);
            if (IsFloor)
            {
                stream.Write(PetFlag);//3?
            }
            else
                stream.Write(0);
            stream.Write((byte)0);
            stream.Write((uint)0);

            stream.Write(Name, string.Empty, string.Empty, string.Empty);
            stream.Finalize(Game.GamePackets.SpawnPlayer);
            //    MyConsole.PrintPacketAdvanced(stream.Memory, stream.Size);

            return stream;
        }
        public unsafe void SendUpdate(uint[] Value, Game.MsgServer.MsgUpdate.DataType datatype)
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();

                Game.MsgServer.MsgUpdate packet = new Game.MsgServer.MsgUpdate(stream, UID, 1);
                stream = packet.Append(stream, datatype, Value);
                stream = packet.GetArray(stream);
                Send(stream);
            }
        }
        bool ConfirmBoss()
        {
            if (Family.ID == 20160 || Family.ID == 60912 || Family.ID == 428094 || Family.ID == 60898 || Family.ID == 60911 || Family.ID == 60891 || Family.ID == 29300 || Family.ID == 60915 || Family.ID == 60899 || Family.ID == 60914 || Family.ID == 20300 || Family.ID == 20070 || Family.ID == 20060) return true;
            return false;
        }
    }
}

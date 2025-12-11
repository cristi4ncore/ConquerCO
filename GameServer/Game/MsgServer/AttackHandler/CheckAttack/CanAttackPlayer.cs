using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeathWish.Client;

namespace DeathWish.Game.MsgServer.AttackHandler.CheckAttack
{
    public class CanAttackPlayer
    {
        public static bool Verified(Client.GameClient client, Role.Player attacked
      , Database.MagicType.Magic DBSpell, bool flashingname = true)
        {
            if (!client.Player.Alive)
                return false;
            if (client.Map.ID == 10137)
            {
                if (client.Player.X < 145 || attacked.X < 145)
                {
                    client.SendSysMesage("You can't attack in the safe area", MsgMessage.ChatMode.Whisper, MsgMessage.MsgColor.red);
                    return false;
                }
            }
           
            if (client.Map.ID == 10166)
            {
                if (Calculate.Base.GetDistance(client.Player.X, client.Player.Y, 60, 128) < 10)
                {
                    client.SendSysMesage("You can't attack in the safe area", MsgMessage.ChatMode.Whisper, MsgMessage.MsgColor.red);
                    return false;
                }
                if (DateTime.Now.Minute <= 15)
                {
                    client.SendSysMesage("You can't attack in the first 15 minute of each hour", MsgMessage.ChatMode.Whisper, MsgMessage.MsgColor.red);
                    return false;
                }
            }
            if (client.Player.ContainFlag(MsgUpdate.Flags.ChillingSnow))
            {
                MsgServer.AttackHandler.CheckAttack.ProcessColdTime.ProcessAttack.Enqueue(new MsgServer.AttackHandler.CheckAttack.ProcessColdTime.Record()
                {
                    Attacker = client,
                    Attacked = attacked.Owner
                });
            }
            if (attacked.ContainFlag(MsgUpdate.Flags.ChillingSnow))
            {
                try
                {
                    if (DBSpell.IsSpellWithColdTime)
                    {
                        if (Role.Core.Rate(100 - 5 * (attacked.BattlePower - client.Player.BattlePower)))
                        {
                            int IncreasedColdTime = 0;
                            MsgSpell Spell;
                            if (attacked.Owner.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.ChillingSnow, out Spell))
                            {
                                var DBSpells = Database.Server.Magic[(ushort)Role.Flags.SpellID.ChillingSnow];
                                IncreasedColdTime = (int)DBSpells[(ushort)Math.Min(DBSpells.Count - 1, Spell.Level)].Damage;
                            }
                        }
                    }
                }
                catch
                {
                }
            }
            if (attacked.Map == 700 && attacked.DynamicID == 0)
                return false;
            if (!MsgTournaments.MsgSchedules.DragonIsland.Attackable(attacked.Map, attacked.X, attacked.Y))
                return false;
            if (!attacked.Alive)
                return false;
            if (attacked.Invisible)
                return false;
            if (client.Player.PkMode == Role.Flags.PKMode.Peace)
                return false;

            if (attacked.Name.Contains("[GM]") || attacked.Name.Contains("[PM]") || attacked.Name.Contains("[TQ]"))
                return false;
            if (client.Player.Map == 3935)
            {
                foreach (var server in Database.GroupServerList.GroupServers.Values)
                {
                    if (Role.Core.GetDistance((ushort)server.X, (ushort)server.Y, attacked.X, attacked.Y) <= 8)
                        return false;
                }
            }
            if (client.Player.Map == 1002)
            {
                if (client.Player.OnMyOwnServer)
                {
                    if (attacked.OnMyOwnServer == false)
                        return true;
                }
                else
                {
                    return false;
                }
            }
            if (Game.MsgTournaments.MsgSchedules.CurrentTournament.Type == MsgTournaments.TournamentType.LastManStand)
            {
                if (Game.MsgTournaments.MsgSchedules.CurrentTournament.InTournament(client))
                    if (Game.MsgTournaments.MsgSchedules.CurrentTournament.Process != MsgTournaments.ProcesType.Alive)
                        return false;
            }
            if (Game.MsgTournaments.MsgSchedules.CurrentTournament.Type == MsgTournaments.TournamentType.TopFight)
            {
                if (Game.MsgTournaments.MsgSchedules.CurrentTournament.InTournament(client))
                    if (Game.MsgTournaments.MsgSchedules.CurrentTournament.Process != MsgTournaments.ProcesType.Alive)
                        return false;
            }
            if (Game.MsgTournaments.MsgSchedules.CurrentTournament.Type == MsgTournaments.TournamentType.CouplesPK)
            {
                if (Game.MsgTournaments.MsgSchedules.CurrentTournament.InTournament(client))
                    if (Game.MsgTournaments.MsgSchedules.CurrentTournament.Process != MsgTournaments.ProcesType.Alive)
                        return false;
            }
            if (Game.MsgTournaments.MsgSchedules.CurrentTournament.Type == MsgTournaments.TournamentType.BettingCPs)
            {
                if (Game.MsgTournaments.MsgSchedules.CurrentTournament.InTournament(client))
                    if (Game.MsgTournaments.MsgSchedules.CurrentTournament.Process != MsgTournaments.ProcesType.Alive)
                        return false;
            }
            if (Game.MsgTournaments.MsgSchedules.CurrentTournament.Type == MsgTournaments.TournamentType.KillerOfElite)
            {
                if (Game.MsgTournaments.MsgSchedules.CurrentTournament.InTournament(client))
                    if (Game.MsgTournaments.MsgSchedules.CurrentTournament.Process != MsgTournaments.ProcesType.Alive)
                        return false;
            }

            if (Game.MsgTournaments.MsgSchedules.CurrentTournament.Type == MsgTournaments.TournamentType.ExtremePk)
            {
                if (Game.MsgTournaments.MsgSchedules.CurrentTournament.InTournament(client))
                    if (Game.MsgTournaments.MsgSchedules.CurrentTournament.Process != MsgTournaments.ProcesType.Alive)
                        return false;
            }
            if (client.Player.Map == 8898 && DateTime.Now.DayOfWeek == DayOfWeek.Saturday)
            {
                if(DateTime.Now.Hour == 21 && DateTime.Now.Minute >= 00 && DateTime.Now.Minute < 02)
                    return false;
            }
            if (Game.MsgTournaments.MsgSchedules.CurrentTournament.Type == MsgTournaments.TournamentType.DragonWar)
            {
                if (Game.MsgTournaments.MsgSchedules.CurrentTournament.InTournament(client))
                    if (Game.MsgTournaments.MsgSchedules.CurrentTournament.Process != MsgTournaments.ProcesType.Alive)
                        return false;
            }

            if (Game.MsgTournaments.MsgSchedules.CurrentTournament.Type == MsgTournaments.TournamentType.TeamDeathMatch)
            {
                if (Game.MsgTournaments.MsgSchedules.CurrentTournament.InTournament(client))
                {
                    if (Game.MsgTournaments.MsgSchedules.CurrentTournament.Process != MsgTournaments.ProcesType.Alive)
                        return false;
                }
            }
            if (Game.MsgTournaments.MsgSchedules.CurrentTournament.Type == MsgTournaments.TournamentType.KillTheCaptain)
            {
                if (Game.MsgTournaments.MsgSchedules.CurrentTournament.InTournament(client))
                {
                    if (Game.MsgTournaments.MsgSchedules.CurrentTournament.Process != MsgTournaments.ProcesType.Alive)
                        return false;
                    if (attacked.Owner.TeamKillTheCaptain == client.TeamKillTheCaptain)
                        return false;
                }
            }

            if (attacked.ContainFlag(MsgUpdate.Flags.Fly))
            {
                if (DBSpell == null)
                    return false;
                else if (!DBSpell.AttackInFly)
                    return false;
            }
            if (attacked.Owner.IsWatching())
            {
                return false;
            }
            if (client.IsWatching())
            {
                return false;
            }

            if (!attacked.AllowAttack())
                return false;
            if (client.InTeamQualifier())
            {
                if (client.Team != null && client.Player.Map == 700)
                {
                    if (!client.Team.Members.ContainsKey(attacked.UID))
                        return true;
                    else
                        return false;
                }
            }
            if (client.Player.Map == MsgTournaments.MsgCaptureTheFlag.MapID)
            {
                if (!MsgTournaments.MsgSchedules.CaptureTheFlag.Attackable(attacked))
                    return false;
            }
            if (client.Player.Map == MsgTournaments.MsgTeamDeathMatch.MapID)
            {
                if (client.Player.GarmentId == attacked.GarmentId)
                    return false;
            }
            if (client.Player.PkMode == Role.Flags.PKMode.Jiang)
            {
                if (attacked.JiangHuActive != 0)
                {
                    if (client.Player.Map == 1002 || client.Player.Map == 1000 || client.Player.Map == 1015 || client.Player.Map == 1020 || client.Player.Map == 1011)
                    {
                        if (client.Player.JiangPkFlag != Role.Instance.JiangHu.AttackFlag.None)
                        {
                            if ((client.Player.JiangPkFlag & Role.Instance.JiangHu.AttackFlag.NotHitFriends) == Role.Instance.JiangHu.AttackFlag.NotHitFriends)
                            {
                                return !client.Player.Associate.Contain(Role.Instance.Associate.Friends, attacked.UID);
                            }
                            if ((client.Player.JiangPkFlag & Role.Instance.JiangHu.AttackFlag.NoHitAlliesClan) == Role.Instance.JiangHu.AttackFlag.NoHitAlliesClan)
                            {
                                if (client.Player.MyClan != null)
                                    return !client.Player.MyClan.Ally.ContainsKey(attacked.ClanUID);
                            }
                            if ((client.Player.JiangPkFlag & Role.Instance.JiangHu.AttackFlag.NotHitAlliedGuild) == Role.Instance.JiangHu.AttackFlag.NotHitAlliedGuild)
                            {
                                if (client.Player.MyGuild != null)
                                    return !client.Player.MyGuild.Ally.ContainsKey(attacked.GuildID);
                            }
                            if ((client.Player.JiangPkFlag & Role.Instance.JiangHu.AttackFlag.NotHitClanMembers) == Role.Instance.JiangHu.AttackFlag.NotHitClanMembers)
                            {
                                return !(client.Player.ClanUID == attacked.ClanUID);
                            }
                            if ((client.Player.JiangPkFlag & Role.Instance.JiangHu.AttackFlag.NotHitGuildMembers) == Role.Instance.JiangHu.AttackFlag.NotHitGuildMembers)
                            {
                                return !(client.Player.GuildID == attacked.GuildID);
                            }
                        }
                        return true;
                    }
                }
                else
                    return false;
            }
            if (MsgTournaments.MsgSchedules.CurrentTournament.Type == MsgTournaments.TournamentType.DragonWar)
            {
                if (Program.BlockAttackMap.Contains(client.Player.Map) && client.Player.DynamicID == 0)
                    return false;
                else if (client.Player.DynamicID != 0)
                {
                    if (Program.BlockAttackMap.Contains(client.Player.DynamicID))
                        return false;
                }
            }
            if (Program.BlockAttackMap.Contains(client.Player.Map) && client.Player.DynamicID == 0)
                return false;
            else if (client.Player.DynamicID != 0)
            {
                if (Program.BlockAttackMap.Contains(client.Player.DynamicID))
                    return false;
            }
            if (client.Player.PkMode == Role.Flags.PKMode.PK)
            {
                if (client.Player.Map == 1080)
                {
                    if (client.Player.RedTeam && attacked.RedTeam)
                        return false;
                    else if (client.Player.BlueTeam && attacked.BlueTeam)
                        return false;
                }
                else if (client.EventBase != null && client.EventBase.Stage == Game.MsgEvents.EventStage.Fighting)
                {
                    if (!client.EventBase.FriendlyFire)
                        foreach (KeyValuePair<uint, Dictionary<uint, GameClient>> Team in client.EventBase.Teams)
                            if (Team.Value.ContainsKey(client.Player.UID) && Team.Value.ContainsKey(attacked.UID))
                                return false;
                }
            }
            if (client.Player.PkMode == Role.Flags.PKMode.Team)
            {
                if (client.Player.Map == 1080)
                {
                    if (client.Player.RedTeam && attacked.RedTeam)
                        return false;
                    else if (client.Player.BlueTeam && attacked.BlueTeam)
                        return false;
                }
                else if (client.EventBase != null && client.EventBase.Stage == Game.MsgEvents.EventStage.Fighting)
                {
                    if (!client.EventBase.FriendlyFire)
                        foreach (KeyValuePair<uint, Dictionary<uint, GameClient>> Team in client.EventBase.Teams)
                            if (Team.Value.ContainsKey(client.Player.UID) && Team.Value.ContainsKey(attacked.UID))
                                return false;
                }
                if (client.Team != null)
                {
                    if (client.Team.Members.ContainsKey(attacked.UID))
                        return false;
                }

                if (client.Player.Associate.Contain(Role.Instance.Associate.Friends, attacked.UID))
                    return false;

                if (client.Player.MyGuild != null)
                {
                    if (client.Player.GuildID == attacked.GuildID)
                        return false;

                    if (attacked.MyGuild != null)
                    {
                        if (client.Player.MyGuild.Ally.ContainsKey(attacked.GuildID))
                            return false;
                    }
                }
                if (client.Player.MyClan != null)
                {
                    if (client.Player.ClanUID == attacked.ClanUID)
                        return false;

                    if (attacked.MyClan != null)
                    {
                        if (client.Player.MyClan.Ally.ContainsKey(attacked.ClanUID))
                            return false;
                    }
                }
            }
            if (Program.ServerConfig.IsInterServer == false)
            {
                if (client.Player.PkMode != Role.Flags.PKMode.Capture && client.Player.PkMode != Role.Flags.PKMode.Peace)
                {
                    if (!attacked.ContainFlag(MsgUpdate.Flags.FlashingName) && !attacked.ContainFlag(MsgUpdate.Flags.RedName) && !attacked.ContainFlag(MsgUpdate.Flags.BlackName))
                    {
                        if (!Program.FreePkMap.Contains(attacked.Map) && attacked.DynamicID == 0)
                           // if (!(client.Player.Map == 1020 && MsgTournaments.MsgSchedules.PoleDomination.Proces == MsgTournaments.ProcesType.Alive))
                                if (!(client.Player.Map == 6570 && client.Player.DynamicID != 0))
                            client.Player.AddFlag(MsgUpdate.Flags.FlashingName, 30, true);

                    }
                    return true;
                }
            }
            if (client.Player.PkMode == Role.Flags.PKMode.CS)
            {
                if (client.Player.ServerID == attacked.ServerID)
                    return false;
            }
            if (client.Player.PkMode == Role.Flags.PKMode.Guild)
            {
                if (client.Player.MyGuild != null)
                {
                    if (client.Player.GuildID == attacked.GuildID)
                        return false;

                    if (attacked.MyGuild != null)
                    {
                        if (client.Player.MyGuild.Ally.ContainsKey(attacked.GuildID))
                            return false;
                    }
                }
                if (client.Player.MyClan != null)
                {
                    if (client.Player.ClanUID == attacked.ClanUID)
                        return false;

                    if (attacked.MyClan != null)
                    {
                        if (client.Player.MyClan.Ally.ContainsKey(attacked.ClanUID))
                            return false;
                    }
                }
            }
            if (client.Player.PkMode == Role.Flags.PKMode.Union)
            {
                if (client.Player.InUnion && attacked.InUnion)
                {
                    if (client.Player.MyUnion.UID == attacked.MyUnion.UID)
                        return false;
                }
            }
            if (client.Player.PkMode == Role.Flags.PKMode.Capture)
            {
                if (attacked.ContainFlag(MsgUpdate.Flags.FlashingName) || attacked.ContainFlag(MsgUpdate.Flags.BlackName))
                    return true;
                else
                    return false;
            }

            return true;
        }
    }
}
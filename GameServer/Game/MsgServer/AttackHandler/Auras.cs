using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeathWish.Game.MsgServer.AttackHandler
{
    public class Auras
    {
        public unsafe static void Execute(Client.GameClient user, InteractQuery Attack, ServerSockets.Packet stream, Dictionary<ushort, Database.MagicType.Magic> DBSpells)
        {
            Database.MagicType.Magic DBSpell;
            MsgSpell ClientSpell;
            if (CheckAttack.CanUseSpell.Verified(Attack, user, DBSpells, out ClientSpell, out DBSpell))
            {
                switch (ClientSpell.ID)
                {
                    case (ushort)Role.Flags.SpellID.MagicDefender:
                        {
                            if (user.Player.ContainFlag(MsgUpdate.Flags.Ride))
                                user.Player.RemoveFlag(MsgUpdate.Flags.Ride);

                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                        , user.Player.UID, Attack.X, Attack.Y, ClientSpell.ID
                                        , ClientSpell.Level, ClientSpell.UseSpellSoul);
                            if (!user.Player.ContainFlag(MsgUpdate.Flags.MagicDefender))
                            {
                                user.Player.AddFlag(MsgUpdate.Flags.MagicDefender, (int)DBSpell.Duration, true);
                                user.Player.SendUpdate(stream, Game.MsgServer.MsgUpdate.Flags.MagicDefender, DBSpell.Duration
           , 0, ClientSpell.Level, Game.MsgServer.MsgUpdate.DataType.AzureShield, true);
                            }
                            else
                            {
                                user.Player.RemoveFlag(MsgUpdate.Flags.MagicDefender);
                                user.Player.SendUpdate(stream, Game.MsgServer.MsgUpdate.Flags.MagicDefender, 0
               , 0, ClientSpell.Level, Game.MsgServer.MsgUpdate.DataType.AzureShield, true);
                            }

                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, 200, DBSpells);


                            break;
                        }
                    default:
                        {
                            MsgUpdate.Flags Aura = MsgUpdate.Flags.Normal;
                            MsgUpdate.Flags OwnerAura = MsgUpdate.Flags.Normal;
                            if (ClientSpell.ID == (ushort)Role.Flags.SpellID.FendAura)
                            {
                                Aura = MsgUpdate.Flags.FeandAura;
                                OwnerAura = MsgUpdate.Flags.OwnerFeandAura;
                            }
                            else if (ClientSpell.ID == (ushort)Role.Flags.SpellID.TyrantAura)
                            {
                                Aura = MsgUpdate.Flags.TyrantAura;
                                OwnerAura = MsgUpdate.Flags.OwnerTyrantAura;
                            }
                            else if (ClientSpell.ID == (ushort)Role.Flags.SpellID.MetalAura)
                            {
                                Aura = MsgUpdate.Flags.MetalAura;
                                OwnerAura = MsgUpdate.Flags.OwnerMetalAura;
                            }
                            else if (ClientSpell.ID == (ushort)Role.Flags.SpellID.WoodAura)
                            {
                                Aura = MsgUpdate.Flags.WoodAura;
                                OwnerAura = MsgUpdate.Flags.OwnerWoodAura;
                            }
                            else if (ClientSpell.ID == (ushort)Role.Flags.SpellID.WatherAura)
                            {
                                Aura = MsgUpdate.Flags.WaterAura;
                                OwnerAura = MsgUpdate.Flags.OwnerWaterAura;
                            }
                            else if (ClientSpell.ID == (ushort)Role.Flags.SpellID.FireAura)
                            {
                                Aura = MsgUpdate.Flags.FireAura;
                                OwnerAura = MsgUpdate.Flags.OwnerFireAura;
                            }
                            else if (ClientSpell.ID == (ushort)Role.Flags.SpellID.EarthAura)
                            {
                                Aura = MsgUpdate.Flags.EartAura;
                                OwnerAura = MsgUpdate.Flags.OwnerEartAura;
                            }
                            if (Aura == MsgUpdate.Flags.Normal)
                                return;
                            if (OwnerAura == MsgUpdate.Flags.Normal)
                                return;
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                        , user.Player.UID, Attack.X, Attack.Y, ClientSpell.ID
                                        , ClientSpell.Level, ClientSpell.UseSpellSoul);


                            bool IncreaseExp = false;
                            if (user.Player.UID == Attack.OpponentUID)
                            {
                                IncreaseExp = user.Player.AddAura(Aura, OwnerAura, DBSpell, Role.StatusFlagsBigVector32.PermanentFlag);

                            }
                            else
                            {
                                Role.IMapObj target;
                                if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Player))
                                {
                                    Role.Player attacked = target as Role.Player;
                                    IncreaseExp = attacked.AddAura(Aura, OwnerAura, DBSpell, Role.StatusFlagsBigVector32.PermanentFlag);
                                }
                            }

                            if (user.Team != null)
                            {
                                foreach (var target in user.Team.GetMembers())
                                {
                                    if (target.Player.UID == user.Player.UID)
                                        continue;
                                    if (target.Player.UseAura == Aura)
                                        continue;
                                    if (Role.Core.GetDistance(user.Player.X, user.Player.Y, target.Player.X, target.Player.Y) < 18)
                                    {
                                        target.Player.AddAura(Aura, OwnerAura, DBSpell, 30);
                                        MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(target.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                                    }
                                }
                            }

                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);

                            if (IncreaseExp)
                                Updates.UpdateSpell.CheckUpdate(stream, user, Attack, 200, DBSpells);
                            break;
                        }
                }
            }
        }
    }
}

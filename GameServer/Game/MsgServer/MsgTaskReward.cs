using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeathWish.Game.MsgServer
{
    public static class MsgTaskReward
    {
        public static unsafe void GetMsgTaskReward(this ServerSockets.Packet stream, out ActionID Action, out uint dwPram)
        {
            Action = (ActionID)stream.ReadUInt8();
            dwPram = stream.ReadUInt32();
        }
        public static unsafe ServerSockets.Packet MsgTaskRewardCreate(this ServerSockets.Packet stream, ActionID Action, uint dwParam, byte Count)
        {
            stream.InitWriter();
            stream.Write((byte)Action);
            stream.Write(dwParam);
            stream.Write(Count);
            stream.Finalize(GamePackets.MsgTaskReward);
            return stream;
        }
        public static bool Contains(ServerSockets.Packet stream, uint dwParam, ushort max, Client.GameClient client)
        {
            byte unm = 1;
            if (max == 10)
                unm = 10;
            else
                unm = 1;
            if (client.Inventory.HaveSpace(unm))
            {
 
                if (dwParam == 3651) //ConcertedEffortPack
                {
                    if (client.Inventory.Contain(3008733, max))
                    {
                        client.Inventory.Remove(3008733, max, stream);
                        return true;
                    }
                }
                if (dwParam == 7047)//Fortune Roulette For Trojan//7047MrBunNpc
                {
                    if (client.Player.BoundConquerPoints >= 219 * (uint)max)
                    {
                        client.Player.BoundConquerPoints -= 219 * max;
                        return true;
                    }
                }
                if (dwParam == 7048)//FortuneWheel
                {
                    if (client.Inventory.Contain(3322480, 1))
                    {
                        if (client.Player.Money >= 3000000 * (uint)max)
                        {
                            client.Player.Money -= 3000000 * (uint)max;
                            return true;
                        }
                        else
                        {
                            client.SendSysMesage("Sorry You dont Have 3,000,000 Silver");
                            return false;
                        }
                    }
                }
            }
            return false;
        }

        public enum ActionID : byte
        {
            Open = 0,
            SetReward = 1,
            Draw = 2,
            Claim = 3,
            Redraw = 4,
            Continue = 5,
            Times10Spinning = 7
        }
        [PacketAttribute(GamePackets.MsgTaskReward)]
        private unsafe static void Process(Client.GameClient user, ServerSockets.Packet stream)
        {
            ActionID Action;
            uint dwParam;
            stream.GetMsgTaskReward(out Action, out dwParam);
            switch (Action)
            {
                case ActionID.Times10Spinning:
                    {
                        if (!user.Inventory.HaveSpace(10))
                        {
                            user.SendSysMesage("Please make 10 more spaces in your inventory.");
                            break;
                        }
                        List<uint> Rewards = new List<uint>();
                        if (Database.TaskRewards.Rewards.TryGetValue(dwParam, out Rewards))
                        {
                            if (MsgTaskReward.Contains(stream, (uint)dwParam, 10, user))
                            {
                                for (int x = 0; x < 10; x++)
                                {
                                    user.Player.TaskRewardIndex = (uint)Program.GetRandom.Next(0, Rewards.Count);
                                    user.Player.TaskReward = Rewards.ToArray()[user.Player.TaskRewardIndex];
                                    user.Inventory.AddItemWitchStack(user.Player.TaskReward, 0, 1, stream, false);
                                    user.Send(stream.MsgTaskRewardCreate(ActionID.SetReward, user.Player.TaskRewardIndex, (byte)Rewards.Count));
                                    user.Player.TaskReward = 0;
                                }
                            }
                            else
                            {
                                user.SendSysMesage("You don't have 10 of The Same Items in You`r Inventory");
                                return;
                            }
                        }
                        break;
                    }
                case ActionID.Continue:
                case ActionID.Draw:
                    {
                        if (!user.Inventory.HaveSpace(1))
                        {
                            user.SendSysMesage("Please make 1 more space in your inventory.");
                            break;
                        }
                        List<uint> Rewards = new List<uint>();
                        if (Database.TaskRewards.Rewards.TryGetValue(dwParam, out Rewards))
                        {
                            if (MsgTaskReward.Contains(stream, (uint)dwParam, 1, user))
                            {
                                user.Player.TaskRewardIndex = (uint)Program.GetRandom.Next(0, Rewards.Count);
                                user.Player.TaskReward = Rewards.ToArray()[user.Player.TaskRewardIndex];
                                user.Send(stream.MsgTaskRewardCreate(ActionID.SetReward, user.Player.TaskRewardIndex, (byte)Rewards.Count));
                                user.Player.viptask = 1;
                            }
                            else
                            {
                                user.SendSysMesage("Sorry You can`t You Don`t Have More Items");
                            }
                            break;
                        }
                        else
                        {
                            user.SendSysMesage("Sorry You can`t You Don`t Have More Items");
                        }
                        break;
                    }
                case ActionID.Claim:
                    {
                        if (user.Player.TaskReward != 0)
                        {
                            user.SendSysMesage("You received a new item from the roulette prize.", MsgMessage.ChatMode.System);
                            user.Inventory.AddItemWitchStack(user.Player.TaskReward, 0, 1, stream, false);
                            user.Player.TaskReward = 0;
                        }
                        break;
                    }
                case ActionID.Redraw:
                    {
                        if (user.Player.TaskReward != 0)
                        {
                            if (user.Player.viptask == 1)
                            {
                                if (user.Player.VipLevel >= 4)
                                {
                                    if (!user.Inventory.HaveSpace(1))
                                    {
                                        user.SendSysMesage("Please make 1 more space in your inventory.");
                                        break;
                                    }
                                    List<uint> Rewards = new List<uint>();
                                    if (Database.TaskRewards.Rewards.TryGetValue(dwParam, out Rewards))
                                    {
                                        user.Player.TaskRewardIndex = (uint)Program.GetRandom.Next(0, Rewards.Count);
                                        user.Player.TaskReward = Rewards.ToArray()[user.Player.TaskRewardIndex];
                                        user.Send(stream.MsgTaskRewardCreate(ActionID.SetReward, user.Player.TaskRewardIndex, (byte)Rewards.Count));
                                        user.Player.viptask = 0;
                                    }
                                }
                                else
                                {
                                    user.CreateBoxDialog("Sorry You Must Up To VIP 4 To Get Free Redraw, You Must Claim Current Reward");
                                }
                            }
                            else
                                user.CreateBoxDialog("You Only Have 1 Free Redraw So You Must Claim Current Reward");
                        }
                        break;
                    }
            }
        }
    }
}
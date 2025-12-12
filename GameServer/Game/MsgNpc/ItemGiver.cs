using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeathWish.Game.MsgServer;

namespace DeathWish.Game.MsgNpc
{
    public unsafe class ItemGiver
    {
        [NpcAttribute(99999)]
        public static void GiveItems(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    dialog.AddText("Hello! I have some items for you. Please choose what you want:")
                        .AddOption("Stones", 1)
                        .AddOption("Mounts", 2)
                        .AddOption("Thanks.", 255)
                        .AddAvatar(63)
                        .FinalizeDialog();
                    break;
                case 1:
                    dialog.AddText("Which stone would you like?")
                        .AddOption("+1 Stone", 101)
                        .AddOption("+2 Stone", 102)
                        .AddOption("+3 Stone", 103)
                        .AddOption("+4 Stone", 104)
                        .AddOption("+5 Stone", 105)
                        .AddOption("+6 Stone", 106)
                        .AddOption("Thanks.", 255)
                        .AddAvatar(63)
                        .FinalizeDialog();
                    break;
                case 2:
                    dialog.AddText("Which mount would you like?")
                        .AddOption("Steed", 201)
                        .AddOption("Riding Crop", 202)
                        .AddOption("Thanks.", 255)
                        .AddAvatar(63)
                        .FinalizeDialog();
                    break;
                case 101:
                    client.Inventory.Add(stream, 730001, 1);
                    break;
                case 102:
                    client.Inventory.Add(stream, 730002, 1);
                    break;
                case 103:
                    client.Inventory.Add(stream, 730003, 1);
                    break;
                case 104:
                    client.Inventory.Add(stream, 730004, 1);
                    break;
                case 105:
                    client.Inventory.Add(stream, 730005, 1);
                    break;
                case 106:
                    client.Inventory.Add(stream, 730006, 1);
                    break;
                case 201:
                    client.Inventory.Add(stream, 300000, 1);
                    break;
                case 202:
                    client.Inventory.Add(stream, 203009, 1);
                    break;
            }
        }
    }
}

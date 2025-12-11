using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeathWish.Game.MsgNpc;

namespace DeathWish.Game.MsgNpc
{
    public class Dialog
    {
        private Client.GameClient client;
        public ServerSockets.Packet stream;

        public Dialog(Client.GameClient Client,ServerSockets.Packet _stream)
        {
            stream = _stream;
            client = Client;
        }

        public Dialog CreateMessageBox(string Text)
        {
            client.Send(stream.NpcReplyCreate(NpcReply.InteractTypes.MessageBox, Text, ushort.MaxValue, 0, true));
            return this;
        }

        public Dialog AddText(string text)
        {
            client.Send(stream.NpcReplyCreate(NpcReply.InteractTypes.Dialog, text, 0, 0));
            return this;
        }
        public Dialog Text(string text)
        {
            client.Send(stream.NpcReplyCreate(NpcReply.InteractTypes.Dialog, text, 0, 0));
            return this;
        }
        public Dialog AddAvatar(ushort id)
        {
            client.Send(stream.NpcReplyCreate(NpcReply.InteractTypes.Avatar, "", id, 0));
            return this;
        }
        public Dialog Send()
        {
            client.Send(stream.NpcReplyCreate(NpcReply.InteractTypes.Finish, "", 0, 0, false));
            return this;
        }
        public Dialog Option(string text, byte id)
        {
            client.Send(stream.NpcReplyCreate(NpcReply.InteractTypes.Option, text, 0, id));
            return this;
        }
        public Dialog AddOption(string text, byte id)
        {
            client.Send(stream.NpcReplyCreate(NpcReply.InteractTypes.Option, text, 0, id));
            return this;
        }
        public Dialog AddOption(string text)
        {
            client.Send(stream.NpcReplyCreate(NpcReply.InteractTypes.Option, text, 0, 255));
            return this;
        }
        public Dialog AddInput50(string text, byte id)
        {
            client.Send(stream.NpcReplyCreate(NpcReply.InteractTypes.Input, text, 50, id));
            return this;
        }
        public Dialog AddInput9(string text, byte id)
        {
            client.Send(stream.NpcReplyCreate(NpcReply.InteractTypes.Input, text, 9, id));
            return this;
        }
        public Dialog AddInput(string text, byte id)
        {
            client.Send(stream.NpcReplyCreate(NpcReply.InteractTypes.Input, text, 16, id));
            return this;
        }
        public unsafe void FinalizeDialog(bool messagebox = false)
        {
            if (!messagebox)
                client.Send(stream.NpcReplyCreate(NpcReply.InteractTypes.Finish, "", 0, 0, false));
        }
    }
}

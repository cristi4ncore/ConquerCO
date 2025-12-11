using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace DeathWish
{
    public class LoaderClient
    {
        public string Username;
        public string Password;
        public const int BufferSize = 20000;
        public static string LoaderPrograms = "database\\LoaderPrograms\\";
        public Socket workSocket = null;
        public string name = "";

        public byte[] buffer = new byte[BufferSize];
        public static List<LoaderClient> CLients = new List<LoaderClient>();
        public ServerSockets.SecuritySocket SecuritySocket;
        public List<string> ProcessNames = new List<string>();
        public void Disconnect()
        {

        }
        public unsafe void dasds(ServerSockets.Packet msg)
        {
            SecuritySocket.Send(msg);
        }
        private void Send(byte[] buf)
        {
            try
            {
                this.workSocket.Send(buf);
            }
            catch
            {
                CLients.Remove(this);
            }
        }
        public void SendLoginKey(byte[] buf)
        {
            this.ProcessNames.Clear();
            this.Send(buf);
        }
        public void GetALLProcess()
        {
            this.ProcessNames.Clear();
            byte[] array = new byte[8];
            array[0] = 1;
            this.Send(array);
        }

        public void KillClient()
        {
            this.ProcessNames.Clear();
            byte[] array = new byte[8];
            array[0] = 9;
            this.Send(array);
        }
    }
}

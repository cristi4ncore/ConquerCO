using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeathWish.Database
{
    public class NobilityTable
    {
        public static void Load()
        {
            WindowsAPI.IniFile ini = new WindowsAPI.IniFile("");
            foreach (string fname in System.IO.Directory.GetFiles(Program.ServerConfig.DbLocation + "\\Users\\"))
            {
                ini.FileName = fname;
                ushort Body = ini.ReadUInt16("Character", "Body", 1002);
                ushort Face = ini.ReadUInt16("Character", "Face", 0);
                uint UID = ini.ReadUInt32("Character", "UID", 0);
                string Name = ini.ReadString("Character", "Name", "None");
                long time = ini.ReadInt64("Character", "PaidPeriod", 0);
                byte Gender = 0;
                if ((byte)(Body % 10) >= 3)
                    Gender = 0;
                else
                    Gender = 1;
                uint Mesh = (uint)(Face * 10000 + Body);
                ulong donation = ini.ReadUInt64("Character", "DonationNobility", 0);
                Role.Instance.Nobility nobility = new Role.Instance.Nobility(UID, Name, donation, Mesh, Gender);
                Program.NobilityRanking.UpdateRank(nobility);
                if (DateTime.Now > DateTime.FromBinary(time))
                {
                    nobility.DonationToBack = ini.ReadUInt64("Character", "DonationToBack", 0);
                    nobility.PaidRank = (Role.Instance.Nobility.NobilityRank)ini.ReadByte("Character", "PaidRank", 0);
                    nobility.PaidPeriod = DateTime.FromBinary(time);
                    Program.NobilityRanking.UpdateRank(nobility);
                }
            }
        }
    }
}
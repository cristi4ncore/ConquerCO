using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeathWish.Database
{
    public class ChiTable
    {
        public static Tuple<DeathWish.Role.Instance.Chi.ChiAttributeType, int>[] ReadPowers(string str)
        {
            if (string.IsNullOrEmpty(str))
                return null;
            Tuple<DeathWish.Role.Instance.Chi.ChiAttributeType, int>[] Powers = new Tuple<Role.Instance.Chi.ChiAttributeType, int>[4];
            string[] strs = str.Split('|');
            int x = 0;
            foreach (var stri in strs)
            {
                string[] ss = stri.Split('-');
                Powers[x] = Tuple.Create((DeathWish.Role.Instance.Chi.ChiAttributeType)int.Parse(ss[0]), int.Parse(ss[1]));
                x++;
            }
            return Powers;
        }
        public static List<DeathWish.Role.Instance.Retreating> ReadPower(string str)
        {
            if (string.IsNullOrEmpty(str))
                return null;
            List<DeathWish.Role.Instance.Retreating> Powers = new List<DeathWish.Role.Instance.Retreating>(4);
            string[] strs = str.Split('|');
            int x = 0;
            foreach (var stri in strs)
            {
                string[] ss = stri.Split('-');
                Powers.Add(new DeathWish.Role.Instance.Retreating((DeathWish.Role.Instance.Chi.ChiAttributeType)int.Parse(ss[0]), int.Parse(ss[1])));
                x++;
            }
            return Powers;
        }
        public static string PowersToString(Tuple<DeathWish.Role.Instance.Chi.ChiAttributeType, int>[] Powers)
        {
            if (Powers == null)
                return "";
            string str = (int)Powers[0].Item1 + "-" + Powers[0].Item2 + "|" + (int)Powers[1].Item1 + "-" + Powers[1].Item2 + "|" + (int)Powers[2].Item1 + "-" + Powers[2].Item2 + "|" + (int)Powers[3].Item1 + "-" + Powers[3].Item2;
            return str;
        }
        public static string PowersToString(List<DeathWish.Role.Instance.Retreating> Powers)
        {
            if (Powers == null)
                return "";
            string str = (int)Powers[0].type + "-" + Powers[0].lvl + "|" + (int)Powers[1].type + "-" + Powers[1].lvl + "|" + (int)Powers[2].type + "-" + Powers[2].lvl + "|" + (int)Powers[3].type + "-" + Powers[3].lvl;
            return str;
        }
        public static void Load()
        {
            WindowsAPI.IniFile ini = new WindowsAPI.IniFile("");
            foreach (string fname in System.IO.Directory.GetFiles(Program.ServerConfig.DbLocation + "\\Users\\"))
            {
                ini.FileName = fname;
                uint UID = ini.ReadUInt32("Character", "UID", 0);
                Role.Instance.Chi playerchi = new Role.Instance.Chi(UID);
                string Name = ini.ReadString("Character", "Name", "None");
                playerchi.Name = Name;
                playerchi.ChiPoints = ini.ReadInt32("Character", "ChiPoints", 0);
                playerchi.Dragon.Load(ini.ReadString("Character", "Dragon", ""), UID, Name);
                playerchi.Phoenix.Load(ini.ReadString("Character", "Pheonix", ""), UID, Name);
                playerchi.Turtle.Load(ini.ReadString("Character", "Turtle", ""), UID, Name);
                playerchi.Tiger.Load(ini.ReadString("Character", "Tiger", ""), UID, Name);
#if RetreatSystem
                playerchi.DragonTime = ini.ReadInt64("Character", "DragonTime", 0);
                if (playerchi.DragonTime != 0)
                {
                    playerchi.DragonPower = ReadPower(ini.ReadString("Character", "RetreatedDragon", ""));
                }
                playerchi.PhoenixTime = ini.ReadInt64("Character", "PhoenixTime", 0);
                if (playerchi.PhoenixTime != 0)
                {
                    playerchi.PhoenixPower = ReadPower(ini.ReadString("Character", "RetreatedPhoenix", ""));
                }
                playerchi.TurtleTime = ini.ReadInt64("Character", "TurtleTime", 0);
                if (playerchi.TurtleTime != 0)
                {
                    playerchi.TurtlePower = ReadPower(ini.ReadString("Character", "RetreatedTurtle", ""));
                }
                playerchi.TigerTime = ini.ReadInt64("Character", "TigerTime", 0);
                if (playerchi.TigerTime != 0)
                {
                    playerchi.TigerPower = ReadPower(ini.ReadString("Character", "RetreatedTiger", ""));
                }
#endif
                if (playerchi.Dragon.UnLocked)
                {
                    Role.Instance.Chi.ChiPool.TryAdd(playerchi.UID, playerchi);
                    Program.ChiRanking.Upadte(Program.ChiRanking.Dragon, playerchi.Dragon);
                }
                if (playerchi.Phoenix.UnLocked)
                    Program.ChiRanking.Upadte(Program.ChiRanking.Phoenix, playerchi.Phoenix);
                if (playerchi.Tiger.UnLocked)
                    Program.ChiRanking.Upadte(Program.ChiRanking.Tiger, playerchi.Tiger);
                if (playerchi.Turtle.UnLocked)
                    Program.ChiRanking.Upadte(Program.ChiRanking.Turtle, playerchi.Turtle);
            }

        }
    }
}

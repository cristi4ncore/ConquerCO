using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using DeathWish.WindowsAPI;

namespace DeathWish.Game
{
	public class BotBoothDB
	{
		public struct Shopflag
		{
			public ushort X;

			public ushort Y;
		}

		public static BinaryFile binary = new BinaryFile();

		public static uint UID = 0;

		public static Dictionary<byte, Shopflag> Shopflags = new Dictionary<byte, Shopflag> {
	   { 1, new Shopflag { X = 271, Y = 218 } }, { 2, new Shopflag { X = 271, Y = 214 } }, { 3, new Shopflag { X = 271, Y = 210 } }, { 4, new Shopflag { X = 271, Y = 206 } }, { 5, new Shopflag { X = 271, Y = 202 } }, { 6, new Shopflag { X = 271, Y = 198 } }, { 7, new Shopflag { X = 271, Y = 194 } }, { 8, new Shopflag { X = 271, Y = 190 } }, { 9, new Shopflag { X = 271, Y = 186 } }, { 10, new Shopflag { X = 271, Y = 182 } }, { 11, new Shopflag { X = 271, Y = 178 } }, { 12, new Shopflag { X = 271, Y = 174 } }
	  ,{ 13, new Shopflag { X = 264, Y = 218 } }, { 14, new Shopflag { X = 264, Y = 214 } }, { 15, new Shopflag { X = 264, Y = 210 } }, { 16, new Shopflag { X = 264, Y = 206 } }, { 17, new Shopflag { X = 264, Y = 202 } }, { 18, new Shopflag { X = 264, Y = 198 } }, { 19, new Shopflag { X = 264, Y = 194 } }, { 20, new Shopflag { X = 264, Y = 190 } }, { 21, new Shopflag { X = 264, Y = 186 } }, { 22, new Shopflag { X = 264, Y = 182 } }, { 23, new Shopflag { X = 264, Y = 178 } }, { 24, new Shopflag { X = 264, Y = 174 } } //area 1

      ,{ 25, new Shopflag { X = 239, Y = 217} }, { 26, new Shopflag { X = 239, Y = 213 } }, { 27, new Shopflag { X = 239, Y = 209 } }, { 28, new Shopflag { X = 239, Y = 205 } }, { 29, new Shopflag { X = 239, Y = 201 } }, { 30, new Shopflag { X = 239, Y = 197 } }, { 31, new Shopflag { X = 239, Y = 193 } }, { 32, new Shopflag { X = 239, Y = 189 } }, { 33, new Shopflag { X = 239, Y = 185 } }, { 34, new Shopflag { X = 239, Y = 181 } }, { 35, new Shopflag { X = 239, Y = 177 } }, { 36, new Shopflag { X = 239, Y = 173 } }  //area2
       ,{ 37, new Shopflag { X = 232, Y = 217} }, { 38, new Shopflag { X = 232, Y = 213 } }, { 39, new Shopflag { X = 232, Y = 209 } }, { 40, new Shopflag { X = 232, Y = 205 } }, { 41, new Shopflag { X = 232, Y = 201 } }, { 42, new Shopflag { X = 232, Y = 197 } }, { 43, new Shopflag { X = 232, Y = 193 } }, { 44, new Shopflag { X = 232, Y = 189 } }, { 45, new Shopflag { X = 232, Y = 185 } }, { 46, new Shopflag { X = 232, Y = 181 } }, { 47, new Shopflag { X = 232, Y = 177 } }, { 48, new Shopflag { X = 232, Y = 173 } }
	  };
		public static void clear()
		{
			Directory.CreateDirectory(Program.ServerConfig.DbLocation + "\\PlayersbotBooth\\");
			DirectoryInfo directoryInfo = new DirectoryInfo(Program.ServerConfig.DbLocation + "\\PlayersbotBooth\\");
			FileInfo[] files = directoryInfo.GetFiles();
			foreach (FileInfo fileInfo in files)
			{
				fileInfo.Delete();
			}
			DirectoryInfo[] directories = directoryInfo.GetDirectories();
			foreach (DirectoryInfo directoryInfo2 in directories)
			{
				directoryInfo2.Delete(true);
			}
			Directory.CreateDirectory(Program.ServerConfig.DbLocation + "\\PlayersbotBooth\\Items\\");
		}
		public static void getboothinfo(string path, out uint uid, out uint profit)
		{
			uid = 0;
			profit = 0;
			IniFile ini = new IniFile("");
			ini.FileName = path;
			uid = UID = ini.ReadUInt32("BotBooth", "UID", 0);
			profit = ini.ReadUInt32("BotBooth", "Profits", 0);
		}
		public static void saveboothinfo(uint uid, uint profit)
		{
			UID = uid;
			string text = "[BotBooth]" + Environment.NewLine;
			object obj = text;
			text = string.Concat(obj, "UID=", uid, Environment.NewLine);
			object obj2 = text;
			text = string.Concat(obj2, "Profits=", profit, Environment.NewLine);
			File.WriteAllText(Program.ServerConfig.DbLocation + "PlayersbotBooth\\" + uid + ".ini", text);
		}

		public unsafe static void GetCount(void* count)
		{
			binary.Read(count, 4);
		}

		public unsafe static void SetCount(void* count)
		{
			binary.Write(count, 4);
		}

		public unsafe static void GetDBitem(void* item)
		{
			binary.Read(item, 152);
		}

		public unsafe static void SetDBitem(void* item)
		{
			if (!binary.Write(item, 152))
			{
				Console.WriteLine("[PlayerBotBooth] Can't write itemDB");
			}
		}

		public unsafe static void GetPerfectionitem(void* item)
		{
			binary.Read(item, 80);
		}

		public unsafe static void SetPerfectionitem(void* item)
		{
			if (!binary.Write(item, 80))
			{
				Console.WriteLine("[PlayerBotBooth] Can't write itemPerf");
			}
		}

		public static void EndRead()
		{			
			binary.Close();
		}

		public static void EndWrite()
		{
			binary.Close();
		}

		public static bool CanGetItems()
		{
			if (binary.Open(Program.ServerConfig.DbLocation + "\\PlayersbotBooth\\Items\\" + UID + ".bin", FileMode.Open))
			{
				return true;
			}
			Console.WriteLine("[PlayerBotBooth] Error Can't Load Player" + UID + " Items");
			return false;
		}

		public static bool CansaveItems()
		{
			if (binary.Open(Program.ServerConfig.DbLocation + "\\PlayersbotBooth\\Items\\" + UID + ".bin", FileMode.Create))
			{
				return true;
			}
			Console.WriteLine("[PlayerBotBooth] Error Can't save Player" + UID + " Items");
			return false;
		}
	}
}
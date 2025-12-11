using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeathWish.Database
{
   
   public class CoatStorage
    {
       public class StorageItem
       {
           public uint ID;
           public uint UnKnow;
           public uint Stars;
       }

       public static Dictionary<uint, StorageItem> Garments = new Dictionary<uint, StorageItem>();

       public static List<StorageItem> GarmentsBig = new List<StorageItem>();
       public static List<StorageItem> MountsBig = new List<StorageItem>();

       public static Dictionary<uint, StorageItem> Mounts = new Dictionary<uint, StorageItem>();

       public static List<StorageItem> OneStarGerment = new List<StorageItem>();
       public static List<StorageItem> TwoStarGerment = new List<StorageItem>();
       public static List<StorageItem> ThreeStarGerment = new List<StorageItem>();
       public static List<StorageItem> FourStarGerment = new List<StorageItem>();
       public static List<StorageItem> FiveStarGerment = new List<StorageItem>();

       public static List<StorageItem> OneStarMount = new List<StorageItem>();
       public static List<StorageItem> TwoStarMount = new List<StorageItem>();
       public static List<StorageItem> ThreeStarMount = new List<StorageItem>();
       public static List<StorageItem> FourStarMount = new List<StorageItem>();
       public static List<StorageItem> FiveStarMount = new List<StorageItem>();

       public static void Load()
       {
           string[] baseText = System.IO.File.ReadAllLines(Program.ServerConfig.DbLocation + "coat_storage_type.txt");
           foreach (var bas_line in baseText)
           {
               var line = bas_line.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                StorageItem item = new StorageItem();
               item.ID = uint.Parse(line[0]);
               item.UnKnow = uint.Parse(line[1]);
               item.Stars = uint.Parse(line[7]);
               byte Type = byte.Parse(line[2]);

               if (Type == 1)//garment
               {
                   Garments.Add(item.ID, item);

                   if (item.Stars >= 1)
                       GarmentsBig.Add(item);

                   if (item.Stars >= 1 && item.Stars <= 1)
                       OneStarGerment.Add(item);

                   if (item.Stars >= 2 && item.Stars <= 2)
                       TwoStarGerment.Add(item);

                   if (item.Stars >= 3 && item.Stars <= 3)
                       ThreeStarGerment.Add(item);

                   if (item.Stars >= 4 && item.Stars <= 4)
                       FourStarGerment.Add(item);

                   if (item.Stars >= 5 && item.Stars <= 5)
                       FiveStarGerment.Add(item);

               }
               else
               {
                   Mounts.Add(item.ID, item);

                   if (item.Stars >= 1)
                       MountsBig.Add(item);

                   if (item.Stars >= 1 && item.Stars <= 1)
                       OneStarMount.Add(item);

                   if (item.Stars >= 2 && item.Stars <= 2)
                       TwoStarMount.Add(item);

                   if (item.Stars >= 3 && item.Stars <= 3)
                       ThreeStarMount.Add(item);

                   if (item.Stars >= 4 && item.Stars <= 4)
                       FourStarMount.Add(item);

                   if (item.Stars >= 5 && item.Stars <= 5)
                       FiveStarMount.Add(item);

               }
           }
       }
       public static uint AmountStarGarments(Client.GameClient client, byte Star)
       {
           uint Count = 0;
           foreach (var garment in client.MyWardrobe.Items[(byte)Role.Instance.Wardrobe.ItemsType.Garment].Values)
           {
               StorageItem item;
               if (Garments.TryGetValue(garment.ITEM_ID, out item))
               {
                   if (item.Stars >= Star)
                       Count++;
               }
           }
           return Count;
       }
       public static uint AmountStarMount(Client.GameClient client, byte Star)
       {
           uint Count = 0;
           foreach (var mount in client.MyWardrobe.Items[(byte)Role.Instance.Wardrobe.ItemsType.Mount].Values)
           {
               StorageItem item;
               if (Mounts.TryGetValue(mount.ITEM_ID, out item))
               {
                   if (item.Stars >= Star)
                       Count++;
               }
           }
           return Count;
       }
    }
}

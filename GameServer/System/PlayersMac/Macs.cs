using COServer.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeathWish.WeCo.PlayersMac
{
    class Macs
    {
        public static string GetMac(string Account)
        {
            using (MySqlCommand command = new MySqlCommand(MySqlCommandType.SELECT).Select("accounts").Where("Username", Account))
            {
                using (MySqlReader reader = new MySqlReader(command))
                {
                    if (reader.Read())
                    {
                        return reader.ReadString("EarthID");
                    }
                    return null;
                }
            }
        }
    }
}

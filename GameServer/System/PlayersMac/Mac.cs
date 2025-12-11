using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using COServer.Database;
namespace DeathWish.Database
{
    public class Mac
    {
        public static string GetPlayerMac(string id)
        {
            try
            {
                using (var cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("accounts").Where("Username", id))
                using (var reader = cmd.CreateReader())
                {
                    if (reader.Read())
                    {
                        return reader.ReadString("EarthID");
                    }
                }
            }
            catch
            {

            }
            return "";
        }
    }
}

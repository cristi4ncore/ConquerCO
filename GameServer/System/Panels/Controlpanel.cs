using COServer.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
//  using SubSonic;


namespace DeathWish
{
    public partial class Controlpanel : Form
    {
        public Controlpanel()
        {
            InitializeComponent();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            Client.GameClient client = null;
            client = Client.GameClient.CharacterFromName(comboBox1.Text);
            if (client == null)
                return;
            CPs.Text = client.Player.ConquerPoints.ToString();
            Money.Text = client.Player.Money.ToString();
            OnlinePoint.Text = client.Player.OnlineMinutes.ToString();
            arenaPoint.Text = client.Player.BoundConquerPoints.ToString();
            Level.Text = client.Player.Level.ToString();
            textBox4.Text = client.Player.SecurityPassword.ToString();
            textBox2.Text = client.Player.VipLevel.ToString();
            donate.Text = client.Player.RacePoints.ToString();
            PLAYERX.Text = client.Player.X.ToString();
            PLAYERY.Text = client.Player.Y.ToString();
            MAPID.Text = client.Player.Map.ToString();

            switch (client.Player.Class)
            {
                #region Get Class
                case 10:
                case 11:
                case 12:
                case 13:
                case 14:
                case 15:
                    {
                        Class.Text = "Trojan";
                        break;
                    }
                case 20:
                case 21:
                case 22:
                case 23:
                case 24:
                case 25:
                    {
                        Class.Text = "Warrior";
                        break;
                    }
                case 40:
                case 41:
                case 42:
                case 43:
                case 44:
                case 45:
                    {
                        Class.Text = "Archer";
                        break;
                    }
                case 50:
                case 51:
                case 52:
                case 53:
                case 54:
                case 55:
                    {
                        Class.Text = "Ninja";
                        break;
                    }
                case 60:
                case 61:
                case 62:
                case 63:
                case 64:
                case 65:
                    {
                        Class.Text = "Monk";
                        break;
                    }
                case 130:
                case 131:
                case 132:
                case 133:
                case 134:
                case 135:
                    {
                        Class.Text = "Water";
                        break;
                    }
                case 140:
                case 141:
                case 142:
                case 143:
                case 144:
                case 145:
                    {
                        Class.Text = "Fire";
                        break;
                    }
                case 70:
                case 71:
                case 72:
                case 73:
                case 74:
                case 75:
                    {
                        Class.Text = "Pirate";
                        break;
                    }
                case 80:
                case 81:
                case 82:
                case 83:
                case 84:
                case 85:
                    {
                        Class.Text = "Dragon-W";
                        break;
                    }
                case 160:
                case 161:
                case 162:
                case 163:
                case 164:
                case 165:
                    {
                        Class.Text = "WindWalker";
                        break;
                    }
                    #endregion
            }
            switch (client.Player.Reborn)
            {
                case 2: Reborn.Text = "2nd Reborn"; break;
                case 1: Reborn.Text = "1st Reborn"; break;
                default: Reborn.Text = "Nono"; break;
            }
            double x = 0;
            textBox3.Text = x.ToString();
            textBox2.Text = client.Player.VipLevel.ToString();

        }
        private void button3_Click(object sender, EventArgs e)
        {
            Client.GameClient client = null;
            client = Client.GameClient.CharacterFromName(comboBox1.Text);
            if (client == null)
                return;
            client.Socket.Disconnect();

        }
        private void button1_Click_2(object sender, EventArgs e)
        {
            Client.GameClient client = null;
            client = Client.GameClient.CharacterFromName(comboBox1.Text);
            if (client == null)
                return;
            client.Player.ConquerPoints = uint.Parse(CPs.Text);
            client.Player.Money = long.Parse(Money.Text);
            client.Player.OnlineMinutes = uint.Parse(OnlinePoint.Text);
            client.Player.BoundConquerPoints = int.Parse(arenaPoint.Text);
            client.Player.Firstcredit = byte.Parse(textBox3.Text);
            client.Player.VipLevel = byte.Parse(textBox2.Text);
            client.Player.SecurityPassword = uint.Parse(textBox4.Text);
            client.Player.RacePoints = uint.Parse(donate.Text);
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                client.Player.SendUpdate(stream, client.Player.VipLevel, Game.MsgServer.MsgUpdate.DataType.VIPLevel);
                client.Player.UpdateVip(stream);
                client.UpdateLevel(stream, byte.Parse(Level.Text));
            }
        }
        private void Control_Load(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            foreach (var user in Database.Server.GamePoll.Values)
            {
                comboBox1.Items.Add(user.Player.Name);
            }
            foreach (var item in Database.Server.ItemsBase.Values)
            {
                comboBox3.Items.Add(item.Name + " " + item.ID);
            }
            foreach (var ban in Database.SystemBannedAccount.BannedPoll.Values)
            {
                comboBox2.Items.Add(ban.Name);
            }
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Client.GameClient client = null;
            client = Client.GameClient.CharacterFromName(comboBox1.Text);
            if (client == null)
                return;
            Entityidbox.Text = client.Player.UID.ToString();
            CPs.Text = client.Player.ConquerPoints.ToString();
            OnlinePoint.Text = client.Player.OnlineMinutes.ToString();
            arenaPoint.Text = client.Player.BoundConquerPoints.ToString();
            Money.Text = client.Player.Money.ToString();
            Level.Text = client.Player.Level.ToString();
            textBox2.Text = client.Player.VipLevel.ToString();
            textBox4.Text = client.Player.SecurityPassword.ToString();
            donate.Text = client.Player.RacePoints.ToString();
            PLAYERX.Text = client.Player.X.ToString();
            PLAYERY.Text = client.Player.Y.ToString();
            MAPID.Text = client.Player.Map.ToString();
            switch (client.Player.Class)
            {
                #region Get Class
                case 10:
                case 11:
                case 12:
                case 13:
                case 14:
                case 15:
                    {
                        Class.Text = "Trojan";
                        break;
                    }
                case 20:
                case 21:
                case 22:
                case 23:
                case 24:
                case 25:
                    {
                        Class.Text = "Warrior";
                        break;
                    }
                case 40:
                case 41:
                case 42:
                case 43:
                case 44:
                case 45:
                    {
                        Class.Text = "Archer";
                        break;
                    }
                case 50:
                case 51:
                case 52:
                case 53:
                case 54:
                case 55:
                    {
                        Class.Text = "Ninja";
                        break;
                    }
                case 60:
                case 61:
                case 62:
                case 63:
                case 64:
                case 65:
                    {
                        Class.Text = "Monk";
                        break;
                    }
                case 130:
                case 131:
                case 132:
                case 133:
                case 134:
                case 135:
                    {
                        Class.Text = "Water";
                        break;
                    }
                case 140:
                case 141:
                case 142:
                case 143:
                case 144:
                case 145:
                    {
                        Class.Text = "Fire";
                        break;
                    }
                case 70:
                case 71:
                case 72:
                case 73:
                case 74:
                case 75:
                    {
                        Class.Text = "Pirate";
                        break;
                    }
                case 80:
                case 81:
                case 82:
                case 83:
                case 84:
                case 85:
                    {
                        Class.Text = "Dragon-W";
                        break;
                    }
                case 160:
                case 161:
                case 162:
                case 163:
                case 164:
                case 165:
                    {
                        Class.Text = "WindWalker";
                        break;
                    }
                    #endregion
            }
            switch (client.Player.Reborn)
            {
                case 2: Reborn.Text = "2nd Reborn"; break;
                case 1: Reborn.Text = "1st Reborn"; break;
                default: Reborn.Text = "Nono"; break;
            }
            double x = 0;
            textBox3.Text = x.ToString();
            textBox2.Text = client.Player.VipLevel.ToString();
        }
        /*
        private void button6_Click(object sender, EventArgs e)
        {
            Client.GameClient client = null;
            client = Client.GameClient.CharacterFromName(comboBox1.Text);
            if (client == null)
                return;
            Database.ItemType.DBItem DBItem;
            string[] id = comboBox3.Text.Split(' ').ToArray();
            if(Database.Server.ItemsBase.TryGetValue(uint.Parse(id[1]), out DBItem))
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    client.Inventory.Add(uint.Parse(id[1]), byte.Parse(this.Plus.Text), DBItem, stream);
                    client.Inventory.AddSoul(uint.Parse(id[1]), 0, 0, 0, byte.Parse(this.Plus.Text), byte.Parse(this.Soc1.Text), byte.Parse(this.Soc2.Text), byte.Parse(this.HP.Text), byte.Parse(this.Bless.Text), byte.Parse(this.textBox7.Text), stream, false);//Necklace
                }
            }

        }
         */
        private void button6_Click(object sender, EventArgs e)
        {
            Client.GameClient client = null;
            client = Client.GameClient.CharacterFromName(comboBox1.Text);
            if (client == null)
                return;
            Database.ItemType.DBItem DBItem;
            string[] id = comboBox3.Text.Split(' ').ToArray();
            if (Database.Server.ItemsBase.TryGetValue(uint.Parse(id[1]), out DBItem))
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    client.Inventory.Add(uint.Parse(id[1]), byte.Parse(this.Plus.Text), DBItem, stream);
                }
            }

        }
        private void button5_Click_1(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            foreach (var user in Database.Server.GamePoll.Values)
            {
                comboBox1.Items.Add(user.Player.Name);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            foreach (var user in Database.Server.GamePoll.Values)
            {
                if (user.Player.Name.ToLower() == comboBox1.Text.ToLower())
                {
                    Database.SystemBannedAccount.AddBan(user.Player.UID, user.Player.Name, uint.Parse(textBox1.Text));
                    user.Socket.Disconnect();
                    break;
                }
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            JiangHu cp = new JiangHu();
            cp.ShowDialog();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Chi cp = new Chi();
            cp.ShowDialog();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            Client.GameClient client = null;
            client = Client.GameClient.CharacterFromName(comboBox1.Text);
            if (client == null)
                return;


            if (!client.Player.Name.Contains("[PM]"))
            {
                Game.MsgServer.MsgNameChange.ChangeName(client, client.Player.Name + "[PM]", true);
            }
            if (!client.ProjectManager)
            {
                client.GameMaster = true;

                client.Player.Level = 140;
                System.Windows.Forms.MessageBox.Show(client.Player.Name + " is now helpdesk");
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            Client.GameClient client = null;
            client = Client.GameClient.CharacterFromName(comboBox1.Text);
            if (client == null)
                return;
            if (!client.ProjectManager)
            {
                client.GameMaster = true;
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            Client.GameClient client = null;
            client = Client.GameClient.CharacterFromName(comboBox1.Text);
            if (client == null)
                return;
            if (!client.Inventory.HaveSpace(15))
            {
                System.Windows.Forms.MessageBox.Show("Character must have 15 free slots into inventory");
                return;
            }
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();

            }
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                client.Inventory.AddDIABLO(120269, 821033, 7, 3004136, 7, 54, 8, 13, 13, 255, 7, 1, stream, false, false, true, 0, 0, 0);//NecklaceTime
                client.Inventory.AddDIABLO(150269, 823059, 7, 3004163, 7, 54, 8, 13, 13, 255, 7, 1, stream, false, false, true, 0, 0, 0);//RingTime
                client.Inventory.AddDIABLO(160249, 824015, 7, 3004149, 7, 54, 8, 13, 13, 255, 7, 1, stream, false, false, true, 0, 0, 0);//BootTime
                client.Inventory.AddDIABLO1(202009, 54, 8, 123, 123, 1, 1, stream, false, false);//TowerTime
                client.Inventory.AddDIABLO1(201009, 54, 8, 103, 103, 1, 1, stream, false, false);//FanTime
                client.Inventory.AddDIABLO1(204009, 54, 8, 103, 123, 1, 1, stream, false, false);//WingTime
                client.Inventory.AddDIABLO1(203009, 54, 8, 0, 0, 1, 1, stream, false, false);//CropTime
                client.Inventory.AddDIABLO1(300000, 54, 8, 0, 0, 0, 1, stream, false, false);//SteedTime  
                client.Inventory.AddDIABLO(614439, 800111, 7, 3004141, 7, 54, 8, 13, 13, 255, 7, 1, stream, false, false, true, 0, 0, 0);//WeaponTime
                client.Inventory.AddDIABLO(614439, 800111, 7, 3004141, 7, 54, 8, 13, 13, 255, 7, 1, stream, false, false, true, 0, 0, 0);//WeaponTime
                client.Inventory.AddDIABLO(130309, 822071, 7, 3004139, 7, 54, 8, 13, 13, 255, 7, 1, stream, false, false, true, 0, 0, 0);//ArmorTime
                client.Inventory.AddDIABLO(118309, 820073, 7, 3004140, 7, 54, 8, 13, 13, 255, 7, 1, stream, false, false, true, 0, 0, 0);//HeaderTime
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            Client.GameClient client = null;
            client = Client.GameClient.CharacterFromName(comboBox1.Text);
            if (client == null)
                return;
            if (!client.Inventory.HaveSpace(15))
            {
                System.Windows.Forms.MessageBox.Show("Character must have 15 free slots into inventory");
                return;
            }
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();

            }
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                client.Inventory.AddDIABLO(120269, 821033, 7, 3004136, 7, 54, 8, 13, 13, 255, 7, 1, stream, false, false, true, 0, 0, 0);//NecklaceTime
                client.Inventory.AddDIABLO(150269, 823059, 7, 3004163, 7, 54, 8, 13, 13, 255, 7, 1, stream, false, false, true, 0, 0, 0);//RingTime
                client.Inventory.AddDIABLO(160249, 824015, 7, 3004149, 7, 54, 8, 13, 13, 255, 7, 1, stream, false, false, true, 0, 0, 0);//BootTime
                client.Inventory.AddDIABLO1(202009, 54, 8, 123, 123, 1, 1, stream, false, false);//TowerTime
                client.Inventory.AddDIABLO1(201009, 54, 8, 103, 103, 1, 1, stream, false, false);//FanTime
                client.Inventory.AddDIABLO1(204009, 54, 8, 103, 123, 1, 1, stream, false, false);//WingTime
                client.Inventory.AddDIABLO1(203009, 54, 8, 0, 0, 1, 1, stream, false, false);//CropTime
                client.Inventory.AddDIABLO1(300000, 54, 8, 0, 0, 0, 1, stream, false, false);//SteedTime 
                client.Inventory.AddDIABLO(624439, 801218, 7, 3004141, 7, 54, 8, 13, 13, 255, 7, 1, stream, false, false, true, 0, 0, 0);//WeaponTime
                client.Inventory.AddDIABLO(624439, 801218, 7, 3004141, 7, 54, 8, 13, 13, 255, 7, 1, stream, false, false, true, 0, 0, 0);//WeaponTime
                client.Inventory.AddDIABLO(131309, 822071, 7, 3004139, 7, 54, 8, 13, 13, 255, 7, 1, stream, false, false, true, 0, 0, 0);//ArmorTime
                client.Inventory.AddDIABLO(111309, 820073, 7, 3004140, 7, 54, 8, 13, 13, 255, 7, 1, stream, false, false, true, 0, 0, 0);//HeaderTime
            }
        }

        private void button12_Click(object sender, EventArgs e)
        {
            Client.GameClient client = null;
            client = Client.GameClient.CharacterFromName(comboBox1.Text);
            if (client == null)
                return;
            if (!client.Inventory.HaveSpace(15))
            {
                System.Windows.Forms.MessageBox.Show("Character must have 15 free slots into inventory");
                return;
            }
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();

            }
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                client.Inventory.AddDIABLO(120269, 821033, 7, 3004136, 7, 54, 8, 13, 13, 255, 7, 1, stream, false, false, true, 0, 0, 0);//NecklaceTime
                client.Inventory.AddDIABLO(150269, 823059, 7, 3004163, 7, 54, 8, 13, 13, 255, 7, 1, stream, false, false, true, 0, 0, 0);//RingTime
                client.Inventory.AddDIABLO(160249, 824015, 7, 3004149, 7, 54, 8, 13, 13, 255, 7, 1, stream, false, false, true, 0, 0, 0);//BootTime
                client.Inventory.AddDIABLO1(202009, 54, 8, 123, 123, 1, 1, stream, false, false);//TowerTime
                client.Inventory.AddDIABLO1(201009, 54, 8, 103, 103, 1, 1, stream, false, false);//FanTime
                client.Inventory.AddDIABLO1(204009, 54, 8, 103, 123, 1, 1, stream, false, false);//WingTime
                client.Inventory.AddDIABLO1(203009, 54, 8, 0, 0, 1, 1, stream, false, false);//CropTime
                client.Inventory.AddDIABLO1(300000, 54, 8, 0, 0, 0, 1, stream, false, false);//SteedTime  
                client.Inventory.AddDIABLO(613429, 800917, 7, 3004141, 7, 54, 8, 13, 13, 255, 7, 1, stream, false, false, true, 0, 0, 0);//WeaponTime
                client.Inventory.AddDIABLO(613429, 800917, 7, 3004141, 7, 54, 8, 13, 13, 255, 7, 1, stream, false, false, true, 0, 0, 0);//WeaponTime
                client.Inventory.AddDIABLO(500429, 800618, 7, 3004150, 7, 54, 8, 13, 13, 255, 7, 1, stream, false, false, true, 0, 0, 0);//WeaponTime
                client.Inventory.AddDIABLO(133309, 822071, 7, 3004139, 7, 54, 8, 13, 13, 255, 7, 1, stream, false, false, true, 0, 0, 0);//ArmorTime
                client.Inventory.AddDIABLO(113309, 820073, 7, 3004140, 7, 54, 8, 13, 13, 255, 7, 1, stream, false, false, true, 0, 0, 0);//HeaderTime
            }
        }

        private void button13_Click(object sender, EventArgs e)
        {
            Client.GameClient client = null;
            client = Client.GameClient.CharacterFromName(comboBox1.Text);
            if (client == null)
                return;
            if (!client.Inventory.HaveSpace(15))
            {
                System.Windows.Forms.MessageBox.Show("Character must have 15 free slots into inventory");
                return;
            }
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();

            }
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                client.Inventory.AddDIABLO(120269, 821033, 7, 3004136, 7, 54, 8, 13, 13, 255, 7, 1, stream, false, false, true, 0, 0, 0);//NecklaceTime
                client.Inventory.AddDIABLO(150269, 823059, 7, 3004163, 7, 54, 8, 13, 13, 255, 7, 1, stream, false, false, true, 0, 0, 0);//RingTime
                client.Inventory.AddDIABLO(160249, 824015, 7, 3004149, 7, 54, 8, 13, 13, 255, 7, 1, stream, false, false, true, 0, 0, 0);//BootTime
                client.Inventory.AddDIABLO1(202009, 54, 8, 123, 123, 1, 1, stream, false, false);//TowerTime
                client.Inventory.AddDIABLO1(201009, 54, 8, 103, 103, 1, 1, stream, false, false);//FanTime
                client.Inventory.AddDIABLO1(204009, 54, 8, 103, 123, 1, 1, stream, false, false);//WingTime
                client.Inventory.AddDIABLO1(203009, 54, 8, 0, 0, 1, 1, stream, false, false);//CropTime
                client.Inventory.AddDIABLO1(300000, 54, 8, 0, 0, 0, 1, stream, false, false);//SteedTime    
                client.Inventory.AddDIABLO(616439, 800111, 7, 3004141, 7, 54, 8, 13, 13, 255, 7, 1, stream, false, false, true, 0, 0, 0);//WeaponTime
                client.Inventory.AddDIABLO(616439, 800111, 7, 3004141, 7, 54, 8, 13, 13, 255, 7, 1, stream, false, false, true, 0, 0, 0);//WeaponTime
                client.Inventory.AddDIABLO(135309, 822071, 7, 3004139, 7, 54, 8, 13, 13, 255, 7, 1, stream, false, false, true, 0, 0, 0);//ArmorTime
                client.Inventory.AddDIABLO(112309, 820073, 7, 3004140, 7, 54, 8, 13, 13, 255, 7, 1, stream, false, false, true, 0, 0, 0);//HeaderTime
            }
        }

        private void button14_Click(object sender, EventArgs e)
        {
            Client.GameClient client = null;
            client = Client.GameClient.CharacterFromName(comboBox1.Text);
            if (client == null)
                return;
            if (!client.Inventory.HaveSpace(15))
            {
                System.Windows.Forms.MessageBox.Show("Character must have 15 free slots into inventory");
                return;
            }
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();

            }
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                client.Inventory.AddDIABLO(120269, 821033, 7, 3004136, 7, 54, 8, 13, 13, 255, 7, 1, stream, false, false, true, 0, 0, 0);//NecklaceTime
                client.Inventory.AddDIABLO(150269, 823059, 7, 3004163, 7, 54, 8, 13, 13, 255, 7, 1, stream, false, false, true, 0, 0, 0);//RingTime
                client.Inventory.AddDIABLO(160249, 824015, 7, 3004149, 7, 54, 8, 13, 13, 255, 7, 1, stream, false, false, true, 0, 0, 0);//BootTime
                client.Inventory.AddDIABLO1(202009, 54, 8, 123, 123, 1, 1, stream, false, false);//TowerTime
                client.Inventory.AddDIABLO1(201009, 54, 8, 103, 103, 1, 1, stream, false, false);//FanTime
                client.Inventory.AddDIABLO1(204009, 54, 8, 103, 123, 1, 1, stream, false, false);//WingTime
                client.Inventory.AddDIABLO1(203009, 54, 8, 0, 0, 1, 1, stream, false, false);//CropTime
                client.Inventory.AddDIABLO1(300000, 54, 8, 0, 0, 0, 1, stream, false, false);//SteedTime  
                client.Inventory.AddDIABLO(622439, 800725, 7, 3004141, 7, 54, 8, 13, 13, 255, 7, 1, stream, false, false, true, 0, 0, 0);//WeaponTime
                client.Inventory.AddDIABLO(622439, 800725, 7, 3004141, 7, 54, 8, 13, 13, 255, 7, 1, stream, false, false, true, 0, 0, 0);//WeaponTime
                client.Inventory.AddDIABLO(136309, 822071, 7, 3004139, 7, 54, 8, 13, 13, 255, 7, 1, stream, false, false, true, 0, 0, 0);//ArmorTime
                client.Inventory.AddDIABLO(143309, 820073, 7, 3004140, 7, 54, 8, 13, 13, 255, 7, 1, stream, false, false, true, 0, 0, 0);//HeaderTime
            }
        }

        private void button15_Click(object sender, EventArgs e)
        {
            Client.GameClient client = null;
            client = Client.GameClient.CharacterFromName(comboBox1.Text);
            if (client == null)
                return;
            if (!client.Inventory.HaveSpace(15))
            {
                System.Windows.Forms.MessageBox.Show("Character must have 15 free slots into inventory");
                return;
            }
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();

            }
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                client.Inventory.AddDIABLO(120269, 821033, 7, 3004136, 7, 54, 8, 13, 13, 255, 7, 1, stream, false, false, true, 0, 0, 0);//NecklaceTime
                client.Inventory.AddDIABLO(150269, 823059, 7, 3004163, 7, 54, 8, 13, 13, 255, 7, 1, stream, false, false, true, 0, 0, 0);//RingTime
                client.Inventory.AddDIABLO(160249, 824015, 7, 3004149, 7, 54, 8, 13, 13, 255, 7, 1, stream, false, false, true, 0, 0, 0);//BootTime
                client.Inventory.AddDIABLO1(202009, 54, 8, 123, 123, 1, 1, stream, false, false);//TowerTime
                client.Inventory.AddDIABLO1(201009, 54, 8, 103, 103, 1, 1, stream, false, false);//FanTime
                client.Inventory.AddDIABLO1(204009, 54, 8, 103, 123, 1, 1, stream, false, false);//WingTime
                client.Inventory.AddDIABLO1(203009, 54, 8, 0, 0, 1, 1, stream, false, false);//CropTime
                client.Inventory.AddDIABLO1(300000, 54, 8, 0, 0, 0, 1, stream, false, false);//SteedTime  
                client.Inventory.AddDIABLO(611439, 800811, 7, 3004141, 7, 54, 8, 13, 13, 255, 7, 1, stream, false, false, true, 0, 0, 0);//WeaponTime
                client.Inventory.AddDIABLO(612439, 800810, 7, 3004141, 7, 54, 8, 13, 13, 255, 7, 1, stream, false, false, true, 0, 0, 0);//WeaponTime
                client.Inventory.AddDIABLO(139309, 822071, 7, 3004139, 7, 54, 8, 13, 13, 255, 7, 1, stream, false, false, true, 0, 0, 0);//ArmorTime
                client.Inventory.AddDIABLO(144309, 820073, 7, 3004140, 7, 54, 8, 13, 13, 255, 7, 1, stream, false, false, true, 0, 0, 0);//HeaderTime
            }
        }

        private void button16_Click(object sender, EventArgs e)
        {
            Client.GameClient client = null;
            client = Client.GameClient.CharacterFromName(comboBox1.Text);
            if (client == null)
                return;
            if (!client.Inventory.HaveSpace(15))
            {
                System.Windows.Forms.MessageBox.Show("Character must have 15 free slots into inventory");
                return;
            }
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();

            }
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                client.Inventory.AddDIABLO(120269, 821033, 7, 3004136, 7, 54, 8, 13, 13, 255, 7, 1, stream, false, false, true, 0, 0, 0);//NecklaceTime
                client.Inventory.AddDIABLO(150269, 823059, 7, 3004163, 7, 54, 8, 13, 13, 255, 7, 1, stream, false, false, true, 0, 0, 0);//RingTime
                client.Inventory.AddDIABLO(160249, 824015, 7, 3004149, 7, 54, 8, 13, 13, 255, 7, 1, stream, false, false, true, 0, 0, 0);//BootTime
                client.Inventory.AddDIABLO1(202009, 54, 8, 123, 123, 1, 1, stream, false, false);//TowerTime
                client.Inventory.AddDIABLO1(201009, 54, 8, 103, 103, 1, 1, stream, false, false);//FanTime
                client.Inventory.AddDIABLO1(204009, 54, 8, 103, 123, 1, 1, stream, false, false);//WingTime
                client.Inventory.AddDIABLO1(203009, 54, 8, 0, 0, 1, 1, stream, false, false);//CropTime
                client.Inventory.AddDIABLO1(300000, 54, 8, 0, 0, 0, 1, stream, false, false);//SteedTime
                client.Inventory.AddDIABLO(617439, 801004, 7, 3004141, 7, 54, 8, 13, 13, 255, 7, 1, stream, false, false, true, 0, 0, 0);//WeaponTime
                client.Inventory.AddDIABLO(617439, 801004, 7, 3004141, 7, 54, 8, 13, 13, 255, 7, 1, stream, false, false, true, 0, 0, 0);//WeaponTime
                client.Inventory.AddDIABLO(138309, 822071, 7, 3004139, 7, 54, 8, 13, 13, 255, 7, 1, stream, false, false, true, 0, 0, 0);//ArmorTime
                client.Inventory.AddDIABLO(148309, 820073, 7, 3004140, 7, 54, 8, 13, 13, 255, 7, 1, stream, false, false, true, 0, 0, 0);//HeaderTime
            }
        }

        private void button17_Click(object sender, EventArgs e)
        {
            Client.GameClient client = null;
            client = Client.GameClient.CharacterFromName(comboBox1.Text);
            if (client == null)
                return;
            if (!client.Inventory.HaveSpace(15))
            {
                System.Windows.Forms.MessageBox.Show("Character must have 15 free slots into inventory");
                return;
            }
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();

            }
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();

                client.Inventory.AddDIABLO(120269, 821033, 7, 3004151, 7, 54, 8, 73, 73, 255, 7, 1, stream, false, false, true, 0, 0, 0);//BagTime
                client.Inventory.AddDIABLO(152279, 823060, 7, 3004148, 7, 54, 8, 73, 73, 255, 7, 1, stream, false, false, true, 0, 0, 0);//BarcletTime
                client.Inventory.AddDIABLO(160249, 824020, 7, 3004149, 7, 54, 8, 73, 73, 255, 7, 1, stream, false, false, true, 0, 0, 0);//BootTime
                client.Inventory.AddDIABLO1(202009, 54, 8, 123, 123, 1, 1, stream, false, false);//TowerTime
                client.Inventory.AddDIABLO1(201009, 54, 8, 103, 103, 1, 1, stream, false, false);//FanTime
                client.Inventory.AddDIABLO1(204009, 54, 8, 103, 123, 1, 1, stream, false, false);//WingTime
                client.Inventory.AddDIABLO1(203009, 54, 8, 0, 0, 1, 1, stream, false, false);//CropTime
                client.Inventory.AddDIABLO1(300000, 54, 8, 0, 0, 0, 1, stream, false, false);//SteedTime 
                client.Inventory.AddDIABLO(620439, 800522, 7, 3004138, 7, 54, 8, 73, 73, 255, 7, 1, stream, false, false, true, 0, 0, 0);//WaterTime
                client.Inventory.AddDIABLO(619439, 801104, 7, 3006170, 7, 54, 8, 0, 0, 255, 0, 1, stream, false, false, true, 0, 0, 0);//HossTime
                client.Inventory.AddDIABLO(134309, 822071, 7, 3004139, 7, 54, 8, 73, 73, 255, 7, 1, stream, false, false, true, 0, 0, 0);//ArmorTime
                client.Inventory.AddDIABLO(114309, 820073, 7, 3004140, 7, 54, 8, 73, 73, 255, 7, 1, stream, false, false, true, 0, 0, 0);//HeaderTime
            }

        }
        private void button18_Click(object sender, EventArgs e)
        {
            Client.GameClient client = null;
            client = Client.GameClient.CharacterFromName(comboBox1.Text);
            if (client == null)
                return;
            if (!client.Inventory.HaveSpace(15))
            {
                System.Windows.Forms.MessageBox.Show("Character must have 15 free slots into inventory");
                return;
            }
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();

                client.Inventory.AddDIABLO(120269, 821033, 7, 3004136, 7, 54, 8, 13, 13, 255, 7, 1, stream, false, false, true, 0, 0, 0);//NecklaceTime
                client.Inventory.AddDIABLO(150269, 823059, 7, 3004163, 7, 54, 8, 13, 13, 255, 7, 1, stream, false, false, true, 0, 0, 0);//RingTime
                client.Inventory.AddDIABLO(160249, 824015, 7, 3004149, 7, 54, 8, 13, 13, 255, 7, 1, stream, false, false, true, 0, 0, 0);//BootTime
                client.Inventory.AddDIABLO1(202009, 54, 8, 123, 123, 1, 1, stream, false, false);//TowerTime
                client.Inventory.AddDIABLO1(201009, 54, 8, 103, 103, 1, 1, stream, false, false);//FanTime
                client.Inventory.AddDIABLO1(204009, 54, 8, 103, 123, 1, 1, stream, false, false);//WingTime
                client.Inventory.AddDIABLO1(203009, 54, 8, 0, 0, 1, 1, stream, false, false);//CropTime
                client.Inventory.AddDIABLO1(300000, 54, 8, 0, 0, 0, 1, stream, false, false);//SteedTime    
                client.Inventory.AddDIABLO(626439, 801308, 7, 3004141, 7, 54, 8, 13, 13, 255, 7, 1, stream, false, false, true, 0, 0, 0);//WeaponTime
                client.Inventory.AddDIABLO(626439, 801308, 7, 3004141, 7, 54, 8, 13, 13, 255, 7, 1, stream, false, false, true, 0, 0, 0);//WeaponTime
                client.Inventory.AddDIABLO(101309, 822071, 7, 3004139, 7, 54, 8, 13, 13, 255, 7, 1, stream, false, false, true, 0, 0, 0);//ArmorTime
                client.Inventory.AddDIABLO(170309, 820073, 7, 3004140, 7, 54, 8, 13, 13, 255, 7, 1, stream, false, false, true, 0, 0, 0);//HeaderTime
            }
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void button20_Click(object sender, EventArgs e)
        {
            Client.GameClient client = null;
            client = Client.GameClient.CharacterFromName(comboBox1.Text);
            if (client == null)
                return;
            if (client.ProjectManager)
            {
                client.GameMaster = false;
            }
        }

        private void button19_Click(object sender, EventArgs e)
        {
            try
            {
                Database.SystemBannedAccount.RemoveBan(comboBox2.Text);
                comboBox2.Items.Clear();
                foreach (var ban in Database.SystemBannedAccount.BannedPoll.Values)
                {
                    comboBox2.Items.Add(ban.Name);
                }
            }
            catch
            {
                comboBox2.Items.Clear();
                foreach (var ban in Database.SystemBannedAccount.BannedPoll.Values)
                {
                    comboBox2.Items.Add(ban.Name);
                }
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button21_Click(object sender, EventArgs e)
        {
            Client.GameClient client = null;
            client = Client.GameClient.CharacterFromName(comboBox1.Text);
            if (client == null)
                return;
            //  client.ConquerPiraTes9 = true;

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void button22_Click(object sender, EventArgs e)
        {
            DeathWish.Panels.AccountsForm cp = new Panels.AccountsForm();
            cp.ShowDialog();
        }
        private void AccinFo_Click(object sender, EventArgs e)
        {
            using (var cmd = new MySqlCommand(MySqlCommandType.SELECT))
            {
                cmd.Select("accounts").Where("ID", Entityidbox.Text);
                using (MySqlReader rdr = new MySqlReader(cmd, true))
                {
                    if (rdr.Read())
                    {
                        //textBox1.Text = rdr.ReadUInt32("EntityID").ToString();
                        //textBox2.Text = rdr.ReadString("Password");
                        //textBox3.Text = rdr.ReadString("Email");
                        //textBox4.Text = rdr.ReadString("IP");
                        textBox5.Text = rdr.ReadString("Username").ToString();
                        textBox6.Text = rdr.ReadUInt32("Password").ToString();
                        textBox5.Enabled = true;
                        textBox6.Enabled = true;
                        //textBox2.Enabled = true;
                        //textBox3.Enabled = true;
                        //textBox5.Enabled = false;

                        //button1.Enabled = true;
                        //button2.Enabled = false;

                    }
                    else
                    {
                        System.Windows.Forms.MessageBox.Show("Username not found");
                    }
                }
            }
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void groupBox4_Enter(object sender, EventArgs e)
        {

        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void Plus_TextChanged(object sender, EventArgs e)
        {

        }

        private void Save_Click(object sender, EventArgs e)
        {
            Client.GameClient client = null;
            MAPID.Text = Convert.ToString(client.Player.Map);
            PLAYERX.Text = Convert.ToString(client.Player.X);
            PLAYERY.Text = Convert.ToString(client.Player.Y);
        }

        private void button23_Click(object sender, EventArgs e)
        {
            Client.GameClient client = null;
            client = Client.GameClient.CharacterFromName(comboBox1.Text);
            if (client == null)
                return;
            PLAYERX.Text = client.Player.X.ToString();
            PLAYERY.Text = client.Player.Y.ToString();
            MAPID.Text = client.Player.Map.ToString();
        }

        private void groupBox3_Enter(object sender, EventArgs e)
        {

        }

        private void donate_TextChanged(object sender, EventArgs e)
        {

        }

        private void label14_Click(object sender, EventArgs e)
        {

        }

        private void arenaPoint_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
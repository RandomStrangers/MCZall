using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace MCZall.Gui
{
    public partial class PropertyWindow : Form
    {
        public PropertyWindow()
        {
            InitializeComponent();
        }

        private void PropertyWindow_Load(object sender, EventArgs e)
        {
            Icon = ActiveForm.Icon;

            foreach (Control ctrl in tabPage1.Controls)
            {
                if (ctrl is ComboBox cmb)
                {
                    cmb.Items.Add("black");
                    cmb.Items.Add("navy");
                    cmb.Items.Add("green");
                    cmb.Items.Add("teal");
                    cmb.Items.Add("maroon");
                    cmb.Items.Add("purple");
                    cmb.Items.Add("gold");
                    cmb.Items.Add("silver");
                    cmb.Items.Add("gray");
                    cmb.Items.Add("blue");
                    cmb.Items.Add("lime");
                    cmb.Items.Add("aqua");
                    cmb.Items.Add("red");
                    cmb.Items.Add("pink");
                    cmb.Items.Add("yellow");
                    cmb.Items.Add("white");
                }
            }
            foreach (Control ctrl in groupBox2.Controls)
            {
                if (ctrl is ComboBox cmb)
                {
                    cmb.Items.Add("black");
                    cmb.Items.Add("navy");
                    cmb.Items.Add("green");
                    cmb.Items.Add("teal");
                    cmb.Items.Add("maroon");
                    cmb.Items.Add("purple");
                    cmb.Items.Add("gold");
                    cmb.Items.Add("silver");
                    cmb.Items.Add("gray");
                    cmb.Items.Add("blue");
                    cmb.Items.Add("lime");
                    cmb.Items.Add("aqua");
                    cmb.Items.Add("red");
                    cmb.Items.Add("pink");
                    cmb.Items.Add("yellow");
                    cmb.Items.Add("white");
                }
            }

            cmbDefaultRank.Items.Add("banned");
            cmbDefaultRank.Items.Add("guest");
            cmbDefaultRank.Items.Add("builder");
            cmbDefaultRank.Items.Add("advbuilder");
            cmbDefaultRank.Items.Add("operator");
            cmbDefaultRank.Items.Add("superop");
            cmbDefaultRank.SelectedIndex = 1;

            //Load server stuff
            LoadProp("properties/server.properties");
            LoadProp("properties/rank.properties");
            try { FillComm(); } catch { Server.s.Log("Commands cannot be edited until the server is started properly"); }
        }

        private void PropertyWindow_Unload(object sender, EventArgs e)
        {
            Window.prevLoaded = false;
        }

        public void FillComm()
        {
            int prevY = 8;

            ComboBox newCombo = new ComboBox();
            Label newLabel = new Label();

            foreach (GrpCommands.RankAllowance aV in GrpCommands.allowedCommands)
            {
                newCombo = new ComboBox();
                newLabel = new Label();

                newCombo.Items.Add("banned");
                newCombo.Items.Add("guest");
                newCombo.Items.Add("builder");
                newCombo.Items.Add("advbuilder");
                newCombo.Items.Add("operator");
                newCombo.Items.Add("superop");
                newCombo.Items.Add("noone");

                newLabel.TextAlign = ContentAlignment.MiddleRight;
                newCombo.DropDownStyle = ComboBoxStyle.DropDownList;

                newCombo.Name = "cmb" + aV.commandName;
                newLabel.Name = "lbl" + aV.commandName;

                newLabel.Text = aV.commandName + ":";
                newCombo.SelectedIndex = newCombo.Items.IndexOf(Level.PermissionToName(aV.rank));

                newCombo.Location = new Point(150, prevY);
                newLabel.Location = new Point(130 - newLabel.Width, prevY + 2);

                newCombo.Visible = true;
                newLabel.Visible = true;

                tabPage3.Controls.Add(newCombo);
                tabPage3.Controls.Add(newLabel);

                prevY += 25;
            }

            newCombo.Dispose();
            newLabel.Dispose();
        }

        public void SaveComm()
        {
            StreamWriter w = new StreamWriter(File.Create("properties/command.properties"));
            w.WriteLine("#   This file contains a reference to every command found in the server software");
            w.WriteLine("#   Use this file to specify which ranks get which commands");
            w.WriteLine("#   Current ranks: Banned, Guest, Builder, AdvBuilder, Operator, Admin (SuperOP)");
            w.WriteLine("");

            foreach (Control ctrl in tabPage3.Controls)
            {
                if (ctrl is ComboBox cmb)
                {
                    w.WriteLine(cmb.Name.Substring(3) + " = " + cmb.Items[cmb.SelectedIndex].ToString());
                }
            }

            w.Flush();
            w.Close();
            w.Dispose();

            Server.s.Log("SAVED: command.properties");
        }

        public void LoadProp(string givenPath)
        {
            if (File.Exists(givenPath))
            {
                string[] lines = File.ReadAllLines(givenPath);

                foreach (string line in lines)
                {
                    if (line != "" && line[0] != '#')
                    {
                        //int index = line.IndexOf('=') + 1; // not needed if we use Split('=')
                        string key = line.Split('=')[0].Trim();
                        string value = line.Split('=')[1].Trim();
                        string color;

                        int foundLimit;

                        switch (key.ToLower())
                        {
                            case "server-name":
                                if (ValidString(value, "![]:.,{}~-+()?_/\\ ")) txtName.Text = value;
                                else txtName.Text = "[MCZall] Minecraft server";
                                break;
                            case "motd":
                                if (ValidString(value, "![]&:.,{}~-+()?_/\\ ")) txtMOTD.Text = value;
                                else txtMOTD.Text = "Welcome to my server!";
                                break;
                            case "port":
                                try { txtPort.Text = Convert.ToInt32(value).ToString(); }
                                catch { txtPort.Text = "25565"; }
                                break;
                            case "verify-names":
                                chkVerify.Checked = value.ToLower() == "true";
                                break;
                            case "public":
                                chkPublic.Checked = value.ToLower() == "true";
                                break;
                            case "world-chat":
                                chkWorld.Checked = value.ToLower() == "true";
                                break;
                            case "max-players":
                                try
                                {
                                    if (Convert.ToByte(value) > 64)
                                    {
                                        value = "64";
                                    }
                                    else if (Convert.ToByte(value) < 1)
                                    {
                                        value = "1";
                                    }
                                    txtPlayers.Text = value;
                                }
                                catch
                                {
                                    Server.s.Log("max-players invalid! setting to default.");
                                    txtPlayers.Text = "12";
                                }
                                break;
                            case "max-maps":
                                try
                                {
                                    if (Convert.ToByte(value) > 35)
                                    {
                                        value = "35";
                                    }
                                    else if (Convert.ToByte(value) < 1)
                                    {
                                        value = "1";
                                    }
                                    txtMaps.Text = value;
                                }
                                catch
                                {
                                    Server.s.Log("max-maps invalid! setting to default.");
                                    txtMaps.Text = "5";
                                }
                                break;
                            case "irc":
                                chkIRC.Checked = value.ToLower() == "true";
                                break;
                            case "irc-server":
                                txtIRCServer.Text = value;
                                break;
                            case "irc-nick":
                                txtNick.Text = value;
                                break;
                            case "irc-channel":
                                txtChannel.Text = value;
                                break;
                            case "anti-tunnels":
                                ChkTunnels.Checked = value.ToLower() == "true";
                                break;
                            case "max-depth":
                                txtDepth.Text = value;
                                break;

                            case "overload":
                                try
                                {
                                    if (Convert.ToInt16(value) > 5000)
                                    {
                                        value = "4000";
                                        Server.s.Log("Max overload is 5000.");
                                    }
                                    else if (Convert.ToInt16(value) < 500)
                                    {
                                        value = "500";
                                        Server.s.Log("Min overload is 500");
                                    }
                                    txtOverload.Text = value;
                                }
                                catch
                                {
                                    txtOverload.Text = "1500";
                                }
                                break;

                            case "rplimit":
                                try { txtRP.Text = value; } catch { txtRP.Text = "500"; }
                                break;
                            case "rplimit-norm":
                                try { txtNormRp.Text = value; } catch { txtNormRp.Text = "10000"; }
                                break;

                            case "backup-time":
                                if (Convert.ToInt32(value) > 1) txtBackup.Text = value; else txtBackup.Text = "300";
                                break;

                            case "physicsrestart":
                                chkPhysicsRest.Checked = value.ToLower() == "true";
                                break;
                            case "deathcount":
                                chkDeath.Checked = value.ToLower() == "true";
                                break;

                            case "defaultcolor":
                                color = C.Parse(value);
                                if (color == "")
                                {
                                    color = C.Name(value);
                                    if (color != "")
                                    {
                                    }
                                    else
                                    {
                                        Server.s.Log("Could not find " + value);
                                        return;
                                    }
                                }
                                cmbDefaultColour.SelectedIndex = cmbDefaultColour.Items.IndexOf(C.Name(value)); break;

                            case "irc-color":
                                color = C.Parse(value);
                                if (color == "")
                                {
                                    color = C.Name(value); if (color != "") { } else { Server.s.Log("Could not find " + value); return; }
                                }
                                cmbIRCColour.SelectedIndex = cmbIRCColour.Items.IndexOf(C.Name(value)); break;

                            case "super-limit":
                                try { foundLimit = int.Parse(value); }
                                catch { Server.s.Log("Invalid " + key + ". Using default."); break; }
                                txtSuperLimit.Text = foundLimit.ToString();
                                break;
                            case "op-limit":
                                try { foundLimit = int.Parse(value); }
                                catch { Server.s.Log("Invalid " + key + ". Using default."); break; }
                                txtOpLimit.Text = foundLimit.ToString();
                                break;
                            case "adv-limit":
                                try { foundLimit = int.Parse(value); }
                                catch { Server.s.Log("Invalid " + key + ". Using default."); break; }
                                txtAdvLimit.Text = foundLimit.ToString();
                                break;
                            case "builder-limit":
                                try { foundLimit = int.Parse(value); }
                                catch { Server.s.Log("Invalid " + key + ". Using default."); break; }
                                txtBuilderLimit.Text = foundLimit.ToString();
                                break;
                            case "super-color":
                                color = C.Parse(value);
                                if (color == "")
                                {
                                    color = C.Name(value); if (color != "") { } else { Server.s.Log("Could not find " + value); return; }
                                }
                                cmbSuperColour.SelectedIndex = cmbSuperColour.Items.IndexOf(C.Name(value)); break;
                            case "op-color":
                                color = C.Parse(value);
                                if (color == "")
                                {
                                    color = C.Name(value); if (color != "") { } else { Server.s.Log("Could not find " + value); return; }
                                }
                                cmbOpColour.SelectedIndex = cmbOpColour.Items.IndexOf(C.Name(value)); break;
                            case "adv-color":
                                color = C.Parse(value);
                                if (color == "")
                                {
                                    color = C.Name(value); if (color != "") { } else { Server.s.Log("Could not find " + value); return; }
                                }
                                cmbAdvColour.SelectedIndex = cmbAdvColour.Items.IndexOf(C.Name(value)); break;
                            case "builder-color":
                                color = C.Parse(value);
                                if (color == "")
                                {
                                    color = C.Name(value); if (color != "") { } else { Server.s.Log("Could not find " + value); return; }
                                }
                                cmbBuilderColour.SelectedIndex = cmbBuilderColour.Items.IndexOf(C.Name(value)); break;
                            case "guest-color":
                                color = C.Parse(value);
                                if (color == "")
                                {
                                    color = C.Name(value); if (color != "") { } else { Server.s.Log("Could not find " + value); return; }
                                }
                                cmbGuestColour.SelectedIndex = cmbGuestColour.Items.IndexOf(C.Name(value)); break;
                            case "banned-color":
                                color = C.Parse(value);
                                if (color == "")
                                {
                                    color = C.Name(value); if (color != "") { } else { Server.s.Log("Could not find " + value); return; }
                                }
                                cmbBannedColour.SelectedIndex = cmbBannedColour.Items.IndexOf(C.Name(value)); break;
                            /*case "default-rank":
                                try {
                                    if (cmbDefaultRank.Items.IndexOf(value) != -1) {
                                        cmbDefaultRank.SelectedIndex = cmbDefaultRank.Items.IndexOf(value);
                                    } else {
                                        Server.s.Log("Invalid default rank");
                                        cmbDefaultRank.SelectedIndex = 1;
                                    }
                                } catch { cmbDefaultRank.SelectedIndex = 1; }
                                break;*/

                            case "old-help":
                                chkHelp.Checked = value.ToLower() == "true";
                                break;

                            case "cheapmessage":
                                chkCheap.Checked = value.ToLower() == "true";
                                break;
                            case "rank-super":
                                chkrankSuper.Checked = value.ToLower() == "true";
                                break;

                            case "afk-minutes":
                                try { txtafk.Text = Convert.ToInt16(value).ToString(); } catch { txtafk.Text = "10"; }
                                break;

                            case "afk-kick":
                                try { txtAFKKick.Text = Convert.ToInt16(value).ToString(); } catch { txtAFKKick.Text = "45"; }
                                break;

                            case "check-updates":
                                chkUpdates.Checked = value.ToLower() == "true";
                                break;
                            case "autoload":
                                chkAutoload.Checked = value.ToLower() == "true";
                                break;
                            case "parse-emotes":
                                chkSmile.Checked = value.ToLower() == "true";
                                break;
                            case "main-name":
                                txtMain.Text = value;
                                break;
                        }
                    }
                }
                //Save(givenPath);
            }
            //else Save(givenPath);
        }
        public bool ValidString(string str, string allowed)
        {
            string allowedchars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz01234567890" + allowed;
            foreach (char ch in str)
            {
                if (allowedchars.IndexOf(ch) == -1)
                {
                    return false;
                }
            }
            return true;
        }

        public void Save(string givenPath)
        {
            try
            {
                StreamWriter w = new StreamWriter(File.Create(givenPath));
                if (givenPath.IndexOf("server") != -1)
                {
                    w.WriteLine("# Edit the settings below to modify how your server operates. This is an explanation of what each setting does.");
                    w.WriteLine("#   server-name\t=\tThe name which displays on minecraft.net");
                    w.WriteLine("#   motd\t=\tThe message which displays when a player connects");
                    w.WriteLine("#   port\t=\tThe port to operate from");
                    w.WriteLine("#   console-only\t=\tRun without a GUI (useful for Linux servers with mono)");
                    w.WriteLine("#   verify-names\t=\tVerify the validity of names");
                    w.WriteLine("#   public\t=\tSet to true to appear in the public server list");
                    w.WriteLine("#   max-players\t=\tThe maximum number of connections");
                    w.WriteLine("#   max-maps\t=\tThe maximum number of maps loaded at once");
                    w.WriteLine("#   world-chat\t=\tSet to true to enable world chat");
                    w.WriteLine("#   guest-goto\t=\tSet to true to give guests goto and levels commands");
                    w.WriteLine("#   irc\t=\tSet to true to enable the IRC bot");
                    w.WriteLine("#   irc-nick\t=\tThe name of the IRC bot");
                    w.WriteLine("#   irc-server\t=\tThe server to connect to");
                    w.WriteLine("#   irc-channel\t=\tThe channel to join");
                    w.WriteLine("#   irc-port\t=\tThe port to use to connect");
                    w.WriteLine("#   irc-identify\t=(true/false)\tDo you want the IRC bot to Identify itself with nickserv. Note: You will need to register it's name with nickserv manually.");
                    w.WriteLine("#   irc-password\t=\tThe password you want to use if you're identifying with nickserv");
                    w.WriteLine("#   anti-tunnels\t=\tStops people digging below max-depth");
                    w.WriteLine("#   max-depth\t=\tThe maximum allowed depth to dig down");
                    w.WriteLine("#   backup-time\t=\tThe number of seconds between automatic backups");
                    w.WriteLine("#   overload\t=\tThe higher this is, the longer the physics is allowed to lag. Default 1500");
                    w.WriteLine();
                    w.WriteLine("#   Host\t=\tThe host name for the database (usually 127.0.0.1)");
                    w.WriteLine("#   Username\t=\tThe username you used to create the database (usually root)");
                    w.WriteLine("#   Password\t=\tThe password set while making the database");
                    w.WriteLine("#   DatabaseName\t=\tThe name of the database stored (Default = MCZall)");
                    w.WriteLine();
                    w.WriteLine("#   defaultColor\t=\tThe color code of the default messages (Default = &e)");
                    w.WriteLine();
                    w.WriteLine();
                    w.WriteLine("# Server options");
                    w.WriteLine("server-name = " + txtName.Text);
                    w.WriteLine("motd = " + txtMOTD.Text);
                    w.WriteLine("port = " + txtPort.Text);
                    w.WriteLine("verify-names = " + chkVerify.Checked.ToString());
                    w.WriteLine("public = " + chkPublic.Checked.ToString());
                    w.WriteLine("max-players = " + txtPlayers.Text);
                    w.WriteLine("max-maps = " + txtMaps.Text);
                    if (Player.ValidName(txtMain.Text)) w.WriteLine("main-name = " + txtMain.Text);
                    else w.WriteLine("main-name = main");
                    w.WriteLine("world-chat = " + chkWorld.Checked.ToString());
                    w.WriteLine("check-updates = " + chkUpdates.Checked.ToString());
                    w.WriteLine("autoload = " + chkAutoload.Checked.ToString());
                    w.WriteLine();
                    w.WriteLine("# irc bot options");
                    w.WriteLine("irc = " + chkIRC.Checked.ToString());
                    w.WriteLine("irc-nick = " + txtNick.Text);
                    w.WriteLine("irc-server = " + txtIRCServer.Text);
                    w.WriteLine("irc-channel = " + txtChannel.Text);
                    w.WriteLine("irc-port = " + Server.ircPort.ToString());
                    w.WriteLine("irc-identify = " + Server.ircIdentify.ToString());
                    w.WriteLine("irc-password = " + Server.ircPassword);
                    w.WriteLine();
                    w.WriteLine("# other options");
                    w.WriteLine("anti-tunnels = " + ChkTunnels.Checked.ToString());
                    w.WriteLine("max-depth = " + txtDepth.Text);
                    w.WriteLine("overload = " + txtOverload.Text);
                    w.WriteLine("rplimit = " + txtRP.Text);
                    w.WriteLine("physicsrestart = " + chkPhysicsRest.Checked.ToString());
                    w.WriteLine("old-help = " + chkHelp.Checked.ToString());
                    w.WriteLine("deathcount = " + chkDeath.Checked.ToString());
                    w.WriteLine("afk-minutes = " + txtafk.Text);
                    w.WriteLine("afk-kick = " + txtAFKKick.Text);
                    w.WriteLine("parse-emotes = " + chkSmile.Checked);
                    w.WriteLine();
                    w.WriteLine("# backup options");
                    w.WriteLine("backup-time = " + txtBackup.Text);
                    w.WriteLine();
                    w.WriteLine("#Error logging");
                    w.WriteLine("report-back = " + Server.reportBack.ToString().ToLower());
                    w.WriteLine();
                    w.WriteLine("#MySQL information");
                    w.WriteLine("Host = " + Server.MySQLHost);
                    w.WriteLine("Username = " + Server.MySQLUsername);
                    w.WriteLine("Password = " + Server.MySQLPassword);
                    w.WriteLine("DatabaseName = " + Server.MySQLDatabaseName);
                    w.WriteLine();
                    w.WriteLine("#Colors");
                    w.WriteLine("defaultColor = " + cmbDefaultColour.Items[cmbDefaultColour.SelectedIndex].ToString());
                    w.WriteLine("irc-color = " + cmbIRCColour.Items[cmbIRCColour.SelectedIndex].ToString());
                }
                else if (givenPath.IndexOf("rank") != -1)
                {
                    w.WriteLine("#Building command limits");
                    w.WriteLine("super-limit = " + txtSuperLimit.Text);
                    w.WriteLine("op-limit = " + txtOpLimit.Text);
                    w.WriteLine("adv-limit = " + txtAdvLimit.Text);
                    w.WriteLine("builder-limit = " + txtBuilderLimit.Text);
                    w.WriteLine("#Colours");
                    w.WriteLine("super-color = " + cmbSuperColour.Items[cmbSuperColour.SelectedIndex].ToString());
                    w.WriteLine("op-color = " + cmbOpColour.Items[cmbOpColour.SelectedIndex].ToString());
                    w.WriteLine("adv-color = " + cmbAdvColour.Items[cmbAdvColour.SelectedIndex].ToString());
                    w.WriteLine("builder-color = " + cmbBuilderColour.Items[cmbBuilderColour.SelectedIndex].ToString());
                    w.WriteLine("guest-color = " + cmbGuestColour.Items[cmbGuestColour.SelectedIndex].ToString());
                    w.WriteLine("banned-color = " + cmbBannedColour.Items[cmbBannedColour.SelectedIndex].ToString());
                    w.WriteLine();
                    w.WriteLine("#Misc");
                    w.WriteLine("cheapmessage = " + chkCheap.Checked.ToString());
                    w.WriteLine("rank-super = " + chkrankSuper.Checked.ToString());
                    /*try {
                    if (cmbDefaultRank.Items[cmbDefaultRank.SelectedIndex].ToString() != "")
                        w.WriteLine("default-rank = " + cmbDefaultRank.Items[cmbDefaultRank.SelectedIndex].ToString());
                    else
                        w.WriteLine("default-rank = " + Group.standard.name);
                    } catch { w.WriteLine("default-rank = guest"); }*/
                }
                w.Flush();
                w.Close();
                w.Dispose();

                Server.s.Log("SAVED: " + givenPath);
            }
            catch
            {
                Server.s.Log("SAVE FAILED! " + givenPath);
            }
        }

        private void CmbDefaultColour_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblDefault.BackColor = Color.FromName(cmbDefaultColour.Items[cmbDefaultColour.SelectedIndex].ToString());
        }

        private void CmbIRCColour_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblIRC.BackColor = Color.FromName(cmbIRCColour.Items[cmbIRCColour.SelectedIndex].ToString());
        }

        private void CmbSuperColour_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblSuper.BackColor = Color.FromName(cmbSuperColour.Items[cmbSuperColour.SelectedIndex].ToString());
        }
        private void CmbOpColour_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblOp.BackColor = Color.FromName(cmbOpColour.Items[cmbOpColour.SelectedIndex].ToString());
        }
        private void CmbAdvColour_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblAdv.BackColor = Color.FromName(cmbAdvColour.Items[cmbAdvColour.SelectedIndex].ToString());
        }
        private void CmbBuilderColour_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblBuilder.BackColor = Color.FromName(cmbBuilderColour.Items[cmbBuilderColour.SelectedIndex].ToString());
        }
        private void CmbGuestColour_SelectedIndexChanged(object sender, EventArgs e)
        {
            try { lblGuest.BackColor = Color.FromName(cmbGuestColour.Items[cmbGuestColour.SelectedIndex].ToString()); } catch { }
        }
        private void CmbBannedColour_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblBanned.BackColor = Color.FromName(cmbBannedColour.Items[cmbBannedColour.SelectedIndex].ToString());
        }

        void RemoveDigit(TextBox foundTxt)
        {
            try
            {
                int lastChar = int.Parse(foundTxt.Text[foundTxt.Text.Length - 1].ToString());
            }
            catch
            {
                foundTxt.Text = "";
            }
        }

        private void TxtPort_TextChanged(object sender, EventArgs e) { RemoveDigit(txtPort); }
        private void TxtPlayers_TextChanged(object sender, EventArgs e) { RemoveDigit(txtPlayers); }
        private void TxtMaps_TextChanged(object sender, EventArgs e) { RemoveDigit(txtMaps); }
        private void TxtBackup_TextChanged(object sender, EventArgs e) { RemoveDigit(txtBackup); }
        private void TxtOverload_TextChanged(object sender, EventArgs e) { RemoveDigit(txtOverload); }
        private void TxtDepth_TextChanged(object sender, EventArgs e) { RemoveDigit(txtDepth); }
        private void TxtSuperLimit_TextChanged(object sender, EventArgs e) { RemoveDigit(txtSuperLimit); }
        private void TxtOpLimit_TextChanged(object sender, EventArgs e) { RemoveDigit(txtOpLimit); }
        private void TxtBuilderLimit_TextChanged(object sender, EventArgs e) { RemoveDigit(txtBuilderLimit); }
        private void TxtAdvLimit_TextChanged(object sender, EventArgs e) { RemoveDigit(txtAdvLimit); }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            foreach (Control tP in tabControl.Controls)
                if (tP is TabPage) foreach (Control ctrl in tP.Controls)
                        if (ctrl is TextBox) if (ctrl.Text == "")
                            {
                                MessageBox.Show("A textbox has been left empty. It must be filled.\n" + ctrl.Name);
                                return;
                            }

            Save("properties/server.properties");
            Save("properties/rank.properties");
            SaveComm();

            //MessageBox.Show("New properties saved.\nRestart the server for them to take effect.");

            Properties.Load("properties/server.properties", true);
            Properties.Load("properties/rank.properties", true);
            GrpCommands.FillRanks();
            Group.InitAll();
            Dispose();
        }

        private void BtnDiscard_Click(object sender, EventArgs e)
        {
            Dispose();
        }

        private void BtnApply_Click(object sender, EventArgs e)
        {
            foreach (Control tP in tabControl.Controls)
                if (tP is TabPage) foreach (Control ctrl in tP.Controls)
                        if (ctrl is TextBox) if (ctrl.Text == "")
                            {
                                MessageBox.Show("A textbox has been left empty. It must be filled.\n" + ctrl.Name);
                                return;
                            }

            Save("properties/server.properties");
            Save("properties/rank.properties");
            SaveComm();

            Properties.Load("properties/server.properties", true);
            Properties.Load("properties/rank.properties", true);
            GrpCommands.FillRanks();
            Group.InitAll();
        }

        private void ToolTip_Popup(object sender, PopupEventArgs e)
        {

        }

        private void ChkBoxes_CheckedChanged(object sender, EventArgs e)
        {
            foreach (Control tP in tabControl.Controls)
                if (tP is TabPage) foreach (Control ctrl in tP.Controls)
                        if (ctrl is CheckBox cBox)
                        {
                            if (chkBoxes.Checked) cBox.Appearance = Appearance.Normal;
                            else cBox.Appearance = Appearance.Button;
                        }
        }

        private void TabPage2_Click(object sender, EventArgs e)
        {

        }

        private void TabPage1_Click(object sender, EventArgs e)
        {

        }

        private void ChkPhysicsRest_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void ChkGC_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}

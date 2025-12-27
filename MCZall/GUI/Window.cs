/*
	Copyright 2010 MCZall Team Licensed under the
	Educational Community License, Version 2.0 (the "License"); you may
	not use this file except in compliance with the License. You may
	obtain a copy of the License at
	
	http://www.osedu.org/licenses/ECL-2.0
	
	Unless required by applicable law or agreed to in writing,
	software distributed under the License is distributed on an "AS IS"
	BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
	or implied. See the License for the specific language governing
	permissions and limitations under the License.
*/
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace MCZall.Gui
{
    public partial class Window : Form
    {
        //readonly Regex regex = new Regex(@"^([1-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])(\." +
        //"([0-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])){3}$");
        // for cross thread use
        delegate void StringCallback(string s);
        delegate void PlayerListCallback(List<Player> players);
        delegate void ReportCallback(Report r);
        delegate void VoidDelegate();

        public static event EventHandler Minimize;
        public static bool Minimized = false;
        //static readonly NotifyIcon ntf = new NotifyIcon();

        internal static Server s;

        readonly bool shuttingDown = false;
        public Window()
        {
            InitializeComponent();
        }

        private void Window_Minimize(object sender, EventArgs e)
        {
            /*
            if (!Minimized) {
                Minimized = true;
                ntf.Text = "MCZall";
                ntf.Icon = this.Icon;
                ntf.Click += delegate {
                    try {
                        Minimized = false;
                        this.ShowInTaskbar = true;
                        this.Show();
                        WindowState = FormWindowState.Normal;
                    } catch (Exception ex) { MessageBox.Show(ex.Message); }
                };
                ntf.Visible = true;
                this.ShowInTaskbar = false;
            }
             */
        }

        private void Window_Load(object sender, EventArgs e)
        {
            Text = "<server name here>";
            Icon theIcon = new Icon(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("MCZall.Zallist.ico"));

            Icon = theIcon;

            s = new Server();
            s.OnLog += WriteLine;
            s.OnCommand += NewCommand;
            s.HeartBeatFail += HeartBeatFail;
            s.OnURLChange += UpdateUrl;
            s.OnPlayerListChange += UpdateClientList;
            s.OnSettingsUpdate += SettingsUpdate;
            s.Start();

            System.Timers.Timer MapTimer = new System.Timers.Timer(10000);
            MapTimer.Elapsed += delegate
            {
                UpdateMapList();
            }; MapTimer.Start();
        }

        void SettingsUpdate()
        {
            if (shuttingDown) return;
            if (txtLog.InvokeRequired)
            {
                VoidDelegate d = new VoidDelegate(SettingsUpdate);
                Invoke(d);
            }
            else
            {
                Text = Server.name + " MCZall Version: " + Server.Version;
            }
        }

        void HeartBeatFail()
        {
            WriteLine("Recent Heartbeat Failed");
        }

        delegate void LogDelegate(string message);

        /// <summary>
        /// Does the same as Console.Write() only in the form
        /// </summary>
        /// <param name="s">The string to write</param>
        public void Write(string s)
        {
            if (shuttingDown) return;
            if (txtLog.InvokeRequired)
            {
                LogDelegate d = new LogDelegate(Write);
                Invoke(d, new object[] { s });
            }
            else
            {
                txtLog.AppendText(s);
            }
        }
        /// <summary>
        /// Does the same as Console.WriteLine() only in the form
        /// </summary>
        /// <param name="s">The line to write</param>
        public void WriteLine(string s)
        {
            if (shuttingDown) return;
            if (InvokeRequired)
            {
                LogDelegate d = new LogDelegate(WriteLine);
                Invoke(d, new object[] { s });
            }
            else
            {
                txtLog.AppendText("\r\n" + s);
            }
        }
        /// <summary>
        /// Updates the list of client names in the window
        /// </summary>
        /// <param name="players">The list of players to add</param>
        public void UpdateClientList(List<Player> players)
        {
            if (InvokeRequired)
            {
                PlayerListCallback d = new PlayerListCallback(UpdateClientList);
                Invoke(d, new object[] { players });
            }
            else
            {
                liClients.Items.Clear();
                Player.players.ForEach(delegate (Player p) { liClients.Items.Add(p.name); });
            }
        }

        public void UpdateMapList(string s = "")
        {
            if (InvokeRequired)
            {
                LogDelegate d = new LogDelegate(UpdateMapList);
                Invoke(d, new object[] { s });
            }
            else
            {
                liMaps.Items.Clear();
                foreach (Level level in Server.levels)
                {
                    liMaps.Items.Add(level.name + " - " + level.physics);
                }
            }
        }

        /// <summary>
        /// Places the server's URL at the top of the window
        /// </summary>
        /// <param name="s">The URL to display</param>
        public void UpdateUrl(string s)
        {
            if (InvokeRequired)
            {
                StringCallback d = new StringCallback(UpdateUrl);
                Invoke(d, new object[] { s });
            }
            else
                txtUrl.Text = s;
        }


        private void Window_FormClosing(object sender, FormClosingEventArgs e)
        {
            Program.ExitProgram(false);
        }

        private void TxtInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Player.GlobalMessage("Console [&a" + Server.ZallState + Server.DefaultColor + "]: &f" + txtInput.Text);
                IRCBot.Say("Console [" + Server.ZallState + "]: " + txtInput.Text);
                WriteLine("<CONSOLE> " + txtInput.Text);
                txtInput.Clear();
            }
        }

        private void TxtCommands_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string sentCmd, sentMsg = "";

                if (txtCommands.Text.IndexOf(' ') != -1)
                {
                    sentCmd = txtCommands.Text.Split(' ')[0];
                    sentMsg = txtCommands.Text.Substring(txtCommands.Text.IndexOf(' ') + 1);
                }
                else if (txtCommands.Text != "")
                {
                    sentCmd = txtCommands.Text;
                }
                else
                {
                    return;
                }

                try
                {
                    Command.all.Find(sentCmd).Use(null, sentMsg);
                    NewCommand("CONSOLE: USED /" + sentCmd + " " + sentMsg);
                }
                catch
                {
                    NewCommand("CONSOLE: Failed command.");
                }

                txtCommands.Clear();
            }
        }

        private void BtnClose_Click(object sender, EventArgs e) { Program.ExitProgram(false); }
        private void BtnRestart_Click(object sender, EventArgs e) { Program.ExitProgram(true); }

        public void NewCommand(string p) { txtCommandsUsed.AppendText("\r\n" + p); }

        void ChangeCheck(string newCheck) { Server.ZallState = newCheck; }

        private void TxtBoxHost_TextChanged(object sender, EventArgs e)
        {
            ChangeCheck(txtBoxHost.Text);
        }

        private void BtnProperties_Click(object sender, EventArgs e)
        {
            if (!prevLoaded) { PropertyForm = new PropertyWindow(); prevLoaded = true; }
            PropertyForm.Show();
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            if (!Program.CurrentUpdate)
                Program.UpdateCheck();
            else
            {
                Thread messageThread = new Thread(new ThreadStart(delegate
                {
                    MessageBox.Show("Already checking for updates.");
                })); messageThread.Start();
            }
        }

        public static bool prevLoaded = false;
        Form PropertyForm;

        private void GBChat_Enter(object sender, EventArgs e)
        {

        }

        private void BtnExtra_Click(object sender, EventArgs e)
        {
            if (!prevLoaded) { PropertyForm = new PropertyWindow(); prevLoaded = true; }
            PropertyForm.Show();
            PropertyForm.Top = Top + Height - txtCommandsUsed.Height;
            PropertyForm.Left = Left;
        }
    }
}

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
using System.Windows.Forms;

namespace MCZall.Gui {
    partial class Window {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        protected override void WndProc(ref Message msg) {
            const int WM_SIZE = 0x0005;
            const int SIZE_MINIMIZED = 1;

            if ((msg.Msg == WM_SIZE) && ((int)msg.WParam == SIZE_MINIMIZED) && (Window.Minimize != null)) {
                this.Window_Minimize(this, EventArgs.Empty);
            }

            base.WndProc(ref msg);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.liClients = new System.Windows.Forms.ListBox();
            this.lable1 = new System.Windows.Forms.Label();
            this.txtUrl = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtInput = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtCommands = new System.Windows.Forms.TextBox();
            this.btnClose = new System.Windows.Forms.Button();
            this.gBChat = new System.Windows.Forms.GroupBox();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.liMaps = new System.Windows.Forms.ListBox();
            this.gBCommands = new System.Windows.Forms.GroupBox();
            this.txtCommandsUsed = new System.Windows.Forms.TextBox();
            this.txtBoxHost = new System.Windows.Forms.TextBox();
            this.lblHost = new System.Windows.Forms.Label();
            this.btnProperties = new System.Windows.Forms.Button();
            this.btnExtra = new System.Windows.Forms.Button();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.gBChat.SuspendLayout();
            this.gBCommands.SuspendLayout();
            this.SuspendLayout();
            // 
            // liClients
            // 
            this.liClients.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.liClients.FormattingEnabled = true;
            this.liClients.Location = new System.Drawing.Point(450, 61);
            this.liClients.Name = "liClients";
            this.liClients.ScrollAlwaysVisible = true;
            this.liClients.Size = new System.Drawing.Size(120, 199);
            this.liClients.TabIndex = 1;
            // 
            // lable1
            // 
            this.lable1.AutoSize = true;
            this.lable1.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lable1.Location = new System.Drawing.Point(13, 13);
            this.lable1.Name = "lable1";
            this.lable1.Size = new System.Drawing.Size(28, 13);
            this.lable1.TabIndex = 3;
            this.lable1.Text = "URL:";
            // 
            // txtUrl
            // 
            this.txtUrl.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtUrl.Location = new System.Drawing.Point(51, 10);
            this.txtUrl.Name = "txtUrl";
            this.txtUrl.ReadOnly = true;
            this.txtUrl.Size = new System.Drawing.Size(393, 21);
            this.txtUrl.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(22, 465);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(31, 13);
            this.label1.TabIndex = 11;
            this.label1.Text = "Chat:";
            // 
            // txtInput
            // 
            this.txtInput.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtInput.Location = new System.Drawing.Point(60, 462);
            this.txtInput.Name = "txtInput";
            this.txtInput.Size = new System.Drawing.Size(321, 21);
            this.txtInput.TabIndex = 12;
            this.txtInput.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TxtInput_KeyDown);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(387, 466);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 13);
            this.label2.TabIndex = 16;
            this.label2.Text = "Command:";
            // 
            // txtCommands
            // 
            this.txtCommands.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCommands.Location = new System.Drawing.Point(450, 463);
            this.txtCommands.Name = "txtCommands";
            this.txtCommands.Size = new System.Drawing.Size(86, 21);
            this.txtCommands.TabIndex = 15;
            this.txtCommands.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TxtCommands_KeyDown);
            // 
            // btnClose
            // 
            this.btnClose.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClose.Location = new System.Drawing.Point(526, 7);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(43, 23);
            this.btnClose.TabIndex = 17;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.BtnClose_Click);
            // 
            // gBChat
            // 
            this.gBChat.Controls.Add(this.txtLog);
            this.gBChat.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gBChat.Location = new System.Drawing.Point(19, 36);
            this.gBChat.Name = "gBChat";
            this.gBChat.Size = new System.Drawing.Size(425, 287);
            this.gBChat.TabIndex = 18;
            this.gBChat.TabStop = false;
            this.gBChat.Text = "Chat";
            this.gBChat.Enter += new System.EventHandler(this.GBChat_Enter);
            // 
            // txtLog
            // 
            this.txtLog.BackColor = System.Drawing.SystemColors.Window;
            this.txtLog.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtLog.Location = new System.Drawing.Point(6, 19);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtLog.Size = new System.Drawing.Size(413, 262);
            this.txtLog.TabIndex = 1;
            // 
            // liMaps
            // 
            this.liMaps.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.liMaps.FormattingEnabled = true;
            this.liMaps.Location = new System.Drawing.Point(450, 266);
            this.liMaps.Name = "liMaps";
            this.liMaps.ScrollAlwaysVisible = true;
            this.liMaps.Size = new System.Drawing.Size(120, 160);
            this.liMaps.TabIndex = 19;
            // 
            // gBCommands
            // 
            this.gBCommands.Controls.Add(this.txtCommandsUsed);
            this.gBCommands.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gBCommands.Location = new System.Drawing.Point(16, 329);
            this.gBCommands.Name = "gBCommands";
            this.gBCommands.Size = new System.Drawing.Size(428, 123);
            this.gBCommands.TabIndex = 20;
            this.gBCommands.TabStop = false;
            this.gBCommands.Text = "Commands";
            // 
            // txtCommandsUsed
            // 
            this.txtCommandsUsed.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCommandsUsed.Location = new System.Drawing.Point(9, 16);
            this.txtCommandsUsed.Multiline = true;
            this.txtCommandsUsed.Name = "txtCommandsUsed";
            this.txtCommandsUsed.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtCommandsUsed.Size = new System.Drawing.Size(413, 100);
            this.txtCommandsUsed.TabIndex = 0;
            // 
            // txtBoxHost
            // 
            this.txtBoxHost.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBoxHost.Location = new System.Drawing.Point(512, 436);
            this.txtBoxHost.Name = "txtBoxHost";
            this.txtBoxHost.Size = new System.Drawing.Size(58, 21);
            this.txtBoxHost.TabIndex = 15;
            this.txtBoxHost.Text = "Alive";
            this.txtBoxHost.TextChanged += new System.EventHandler(this.TxtBoxHost_TextChanged);
            this.txtBoxHost.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TxtCommands_KeyDown);
            // 
            // lblHost
            // 
            this.lblHost.AutoSize = true;
            this.lblHost.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHost.Location = new System.Drawing.Point(450, 439);
            this.lblHost.Name = "lblHost";
            this.lblHost.Size = new System.Drawing.Size(56, 13);
            this.lblHost.TabIndex = 16;
            this.lblHost.Text = "Host state:";
            // 
            // btnProperties
            // 
            this.btnProperties.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnProperties.Location = new System.Drawing.Point(450, 7);
            this.btnProperties.Name = "btnProperties";
            this.btnProperties.Size = new System.Drawing.Size(70, 23);
            this.btnProperties.TabIndex = 17;
            this.btnProperties.Text = "Properties";
            this.btnProperties.UseVisualStyleBackColor = true;
            this.btnProperties.Click += new System.EventHandler(this.BtnProperties_Click);
            // 
            // btnExtra
            // 
            this.btnExtra.Location = new System.Drawing.Point(542, 461);
            this.btnExtra.Name = "btnExtra";
            this.btnExtra.Size = new System.Drawing.Size(33, 23);
            this.btnExtra.TabIndex = 21;
            this.btnExtra.Text = "∇";
            this.btnExtra.UseVisualStyleBackColor = true;
            this.btnExtra.Click += new System.EventHandler(this.BtnExtra_Click);
            // 
            // btnUpdate
            // 
            this.btnUpdate.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnUpdate.Location = new System.Drawing.Point(450, 32);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(118, 23);
            this.btnUpdate.TabIndex = 17;
            this.btnUpdate.Text = "Update";
            this.btnUpdate.UseVisualStyleBackColor = true;
            this.btnUpdate.Click += new System.EventHandler(this.BtnUpdate_Click);
            // 
            // Window
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(580, 489);
            this.Controls.Add(this.btnExtra);
            this.Controls.Add(this.gBCommands);
            this.Controls.Add(this.liMaps);
            this.Controls.Add(this.gBChat);
            this.Controls.Add(this.btnUpdate);
            this.Controls.Add(this.btnProperties);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.lblHost);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtBoxHost);
            this.Controls.Add(this.txtCommands);
            this.Controls.Add(this.txtInput);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtUrl);
            this.Controls.Add(this.lable1);
            this.Controls.Add(this.liClients);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "Window";
            this.Text = "Window";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Window_FormClosing);
            Window.Minimize += new System.EventHandler(this.Window_Minimize);
            this.Load += new System.EventHandler(this.Window_Load);
            this.gBChat.ResumeLayout(false);
            this.gBChat.PerformLayout();
            this.gBCommands.ResumeLayout(false);
            this.gBCommands.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.ListBox liClients;
        private System.Windows.Forms.Label lable1;
        private System.Windows.Forms.TextBox txtUrl;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtInput;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtCommands;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.GroupBox gBChat;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.ListBox liMaps;
        private System.Windows.Forms.GroupBox gBCommands;
        private System.Windows.Forms.TextBox txtCommandsUsed;
        private System.Windows.Forms.TextBox txtBoxHost;
        private System.Windows.Forms.Label lblHost;
        private System.Windows.Forms.Button btnProperties;
        private System.Windows.Forms.Button btnExtra;
        private System.Windows.Forms.Button btnUpdate;
    }
}
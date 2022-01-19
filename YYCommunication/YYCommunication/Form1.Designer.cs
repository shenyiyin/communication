namespace YYCommunication
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.ConnectBtn = new System.Windows.Forms.Button();
            this.ViewText = new System.Windows.Forms.TextBox();
            this.SendText = new System.Windows.Forms.TextBox();
            this.SendBtn = new System.Windows.Forms.Button();
            this.objcbx = new System.Windows.Forms.ComboBox();
            this.flashBtn = new System.Windows.Forms.Button();
            this.filebtn = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.listenBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // ConnectBtn
            // 
            this.ConnectBtn.Location = new System.Drawing.Point(229, 9);
            this.ConnectBtn.Name = "ConnectBtn";
            this.ConnectBtn.Size = new System.Drawing.Size(69, 23);
            this.ConnectBtn.TabIndex = 0;
            this.ConnectBtn.Text = "Connect";
            this.ConnectBtn.UseVisualStyleBackColor = true;
            this.ConnectBtn.Click += new System.EventHandler(this.ConnectBtn_Click);
            // 
            // ViewText
            // 
            this.ViewText.Location = new System.Drawing.Point(12, 38);
            this.ViewText.Multiline = true;
            this.ViewText.Name = "ViewText";
            this.ViewText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.ViewText.Size = new System.Drawing.Size(412, 273);
            this.ViewText.TabIndex = 1;
            // 
            // SendText
            // 
            this.SendText.AllowDrop = true;
            this.SendText.Location = new System.Drawing.Point(12, 327);
            this.SendText.Multiline = true;
            this.SendText.Name = "SendText";
            this.SendText.Size = new System.Drawing.Size(314, 52);
            this.SendText.TabIndex = 2;
            this.SendText.DragDrop += new System.Windows.Forms.DragEventHandler(this.SendText_DragDrop);
            this.SendText.DragEnter += new System.Windows.Forms.DragEventHandler(this.SendText_DragEnter);
            // 
            // SendBtn
            // 
            this.SendBtn.Location = new System.Drawing.Point(349, 342);
            this.SendBtn.Name = "SendBtn";
            this.SendBtn.Size = new System.Drawing.Size(75, 23);
            this.SendBtn.TabIndex = 3;
            this.SendBtn.Text = "Send";
            this.SendBtn.UseVisualStyleBackColor = true;
            this.SendBtn.Click += new System.EventHandler(this.SendBtn_Click);
            // 
            // objcbx
            // 
            this.objcbx.FormattingEnabled = true;
            this.objcbx.Location = new System.Drawing.Point(12, 12);
            this.objcbx.Name = "objcbx";
            this.objcbx.Size = new System.Drawing.Size(115, 20);
            this.objcbx.TabIndex = 4;
            // 
            // flashBtn
            // 
            this.flashBtn.Location = new System.Drawing.Point(148, 9);
            this.flashBtn.Name = "flashBtn";
            this.flashBtn.Size = new System.Drawing.Size(69, 23);
            this.flashBtn.TabIndex = 5;
            this.flashBtn.Text = "flash";
            this.flashBtn.UseVisualStyleBackColor = true;
            this.flashBtn.Click += new System.EventHandler(this.flashBtn_Click);
            // 
            // filebtn
            // 
            this.filebtn.Location = new System.Drawing.Point(12, 385);
            this.filebtn.Name = "filebtn";
            this.filebtn.Size = new System.Drawing.Size(56, 23);
            this.filebtn.TabIndex = 6;
            this.filebtn.Text = "file";
            this.filebtn.UseVisualStyleBackColor = true;
            this.filebtn.Click += new System.EventHandler(this.filebtn_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // listenBtn
            // 
            this.listenBtn.Location = new System.Drawing.Point(74, 384);
            this.listenBtn.Name = "listenBtn";
            this.listenBtn.Size = new System.Drawing.Size(58, 24);
            this.listenBtn.TabIndex = 8;
            this.listenBtn.Text = "Video";
            this.listenBtn.UseVisualStyleBackColor = true;
            this.listenBtn.Click += new System.EventHandler(this.listenBtn_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(433, 410);
            this.Controls.Add(this.listenBtn);
            this.Controls.Add(this.filebtn);
            this.Controls.Add(this.flashBtn);
            this.Controls.Add(this.objcbx);
            this.Controls.Add(this.SendBtn);
            this.Controls.Add(this.SendText);
            this.Controls.Add(this.ViewText);
            this.Controls.Add(this.ConnectBtn);
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button ConnectBtn;
        private System.Windows.Forms.TextBox ViewText;
        private System.Windows.Forms.TextBox SendText;
        private System.Windows.Forms.Button SendBtn;
        private System.Windows.Forms.ComboBox objcbx;
        private System.Windows.Forms.Button flashBtn;
        private System.Windows.Forms.Button filebtn;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button listenBtn;
    }
}


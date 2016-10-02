namespace ScreenKeyboard
{
    partial class Form1
    {
        /// <summary>
        /// 必要なデザイナ変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナで生成されたコード

        /// <summary>
        /// デザイナ サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディタで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.Altcheck = new System.Windows.Forms.CheckBox();
            this.Ctrlcheck = new System.Windows.Forms.CheckBox();
            this.Shiftcheck = new System.Windows.Forms.CheckBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.listBox2 = new System.Windows.Forms.ListBox();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(90, 220);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(90, 23);
            this.button2.TabIndex = 8;
            this.button2.Text = "キャンセル";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(0, 220);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(90, 23);
            this.button1.TabIndex = 7;
            this.button1.Text = "決定";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 12;
            this.listBox1.Items.AddRange(new object[] {
            "半角/全角",
            "無変換",
            "スペース",
            "バックスペース",
            "変換(次候補)",
            "アプリケーションキー",
            "Enter",
            "Delete",
            "Insert",
            "Home",
            "End",
            "PageUp",
            "PageDown",
            "PrintScreen",
            "ScrollLock",
            "Pause",
            "NumLock",
            "Tab",
            "CapsLock",
            "Windowsキー(左)",
            "Windowsキー(右)",
            "Esc",
            "F1",
            "F2",
            "F3",
            "F4",
            "F5",
            "F6",
            "F7",
            "F8",
            "F9",
            "F10",
            "F11",
            "F12",
            "↑",
            "←",
            "→",
            "↓"});
            this.listBox1.Location = new System.Drawing.Point(-1, 0);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(173, 172);
            this.listBox1.TabIndex = 6;
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.setindex);
            this.listBox1.DoubleClick += new System.EventHandler(this.button1_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(0, -2);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(180, 222);
            this.tabControl1.TabIndex = 9;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.Altcheck);
            this.tabPage1.Controls.Add(this.Ctrlcheck);
            this.tabPage1.Controls.Add(this.Shiftcheck);
            this.tabPage1.Controls.Add(this.listBox1);
            this.tabPage1.Location = new System.Drawing.Point(4, 21);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(172, 197);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "通常キー";
            this.tabPage1.UseVisualStyleBackColor = true;
            this.tabPage1.Enter += new System.EventHandler(this.tabPage1_Enter);
            // 
            // Altcheck
            // 
            this.Altcheck.AutoSize = true;
            this.Altcheck.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Altcheck.Location = new System.Drawing.Point(119, 176);
            this.Altcheck.Name = "Altcheck";
            this.Altcheck.Size = new System.Drawing.Size(39, 16);
            this.Altcheck.TabIndex = 15;
            this.Altcheck.Text = "Alt";
            this.Altcheck.UseVisualStyleBackColor = true;
            // 
            // Ctrlcheck
            // 
            this.Ctrlcheck.AutoSize = true;
            this.Ctrlcheck.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Ctrlcheck.Location = new System.Drawing.Point(52, 176);
            this.Ctrlcheck.Name = "Ctrlcheck";
            this.Ctrlcheck.Size = new System.Drawing.Size(61, 16);
            this.Ctrlcheck.TabIndex = 14;
            this.Ctrlcheck.Text = "Control";
            this.Ctrlcheck.UseVisualStyleBackColor = true;
            // 
            // Shiftcheck
            // 
            this.Shiftcheck.AutoSize = true;
            this.Shiftcheck.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Shiftcheck.Location = new System.Drawing.Point(2, 176);
            this.Shiftcheck.Name = "Shiftcheck";
            this.Shiftcheck.Size = new System.Drawing.Size(48, 16);
            this.Shiftcheck.TabIndex = 13;
            this.Shiftcheck.Text = "Shift";
            this.Shiftcheck.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.listBox2);
            this.tabPage2.Location = new System.Drawing.Point(4, 21);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(172, 197);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "特殊キー";
            this.tabPage2.UseVisualStyleBackColor = true;
            this.tabPage2.Enter += new System.EventHandler(this.tabPage2_Enter);
            // 
            // listBox2
            // 
            this.listBox2.FormattingEnabled = true;
            this.listBox2.ItemHeight = 12;
            this.listBox2.Items.AddRange(new object[] {
            "音量アップ",
            " ダウン",
            " ミュート",
            "メディアプレーヤーの起動",
            " 次のトラックへ",
            " 前のトラックへ",
            " 再生/一時停止",
            " 停止",
            "「戻る」",
            "「進む」",
            "「更新」",
            "「中止」",
            "「検索」",
            "「お気に入り」",
            "「ホーム」",
            "メールの開始"});
            this.listBox2.Location = new System.Drawing.Point(-1, -1);
            this.listBox2.Name = "listBox2";
            this.listBox2.Size = new System.Drawing.Size(173, 196);
            this.listBox2.TabIndex = 7;
            this.listBox2.SelectedIndexChanged += new System.EventHandler(this.setindex);
            this.listBox2.DoubleClick += new System.EventHandler(this.button1_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(176, 243);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "設定";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.ResumeLayout(false);

        }



        #endregion

        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        public System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        public System.Windows.Forms.ListBox listBox2;
        private System.Windows.Forms.CheckBox Altcheck;
        private System.Windows.Forms.CheckBox Ctrlcheck;
        private System.Windows.Forms.CheckBox Shiftcheck;
    }
}
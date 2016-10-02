using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ScreenKeyboard.Properties;
using System.Runtime.InteropServices;
using System.Media;

namespace ScreenKeyboard
{
    public partial class tenkey : Form
    {
        int hi = 27, wi = 27 ,n=0;
        public int x=0, y=0;
        private static string moji = "", activename = "";
        private Button[] box = new Button[21];
        SoundPlayer sound;

        public tenkey()
        {
            InitializeComponent();
        }

        #region DLL

        //DLLをインポート
        [DllImport("user32.dll")]   //特定のウィンドウがあるか探索
        extern static IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]   //特定のウィンドウが画面に表示されているか探索
        extern static bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll")]   //特定のウィンドウをアクティブにする
        extern static bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]   //アクティブなウィンドウを取得
        extern static IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]   //指定したウィンドウの名前を取得する
        extern static IntPtr GetWindowText(IntPtr hWnd, StringBuilder lpStr, int nMaxCount);

        [DllImport("user32.dll")]   // 現在-のスタイルを取得
        private static extern UInt32 GetWindowLong(IntPtr hWnd, GWL index);

        [DllImport("user32.dll")]   //スタイルを反映
        private static extern UInt32 SetWindowLong(IntPtr hWnd, GWL index, UInt32 unValue);

        [DllImport("user32.dll")]   // ウィンドウを再描画
        private static extern UInt32 SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int width, int height, SWP flags);

        #endregion

        private void tenkey_Load_1(object sender, EventArgs e)
        {
            setNotActiveWindow(this.Handle);
            this.Location = new Point(x,y);
            set();
            this.Size = new System.Drawing.Size(wi * 5 + 8, 195);

            switch (Settings.Default.onshu)
            {
                case 0:
                    sound = new SoundPlayer(ScreenKeyboard.Properties.Resources.piko);
                    break;
                case 1:
                    sound = new SoundPlayer(ScreenKeyboard.Properties.Resources.poka);
                    break;
                case 2:
                    sound = new SoundPlayer(ScreenKeyboard.Properties.Resources.poko);
                    break;
                case 3:
                    sound = new SoundPlayer(ScreenKeyboard.Properties.Resources.pon);
                    break;
            }
            
            change();
        }

        void change()
        {

            button18.Location = new Point(0, 0);
            button8.Location = new Point(0, hi);
            button5.Location = new Point(0, button8.Location.Y + hi);
            button2.Location = new Point(0, button5.Location.Y + hi);
            button1.Location = new Point(0, button2.Location.Y + hi);

            button19.Location = new Point(wi, 0);
            button9.Location = new Point(wi, hi);
            button6.Location = new Point(wi, button9.Location.Y + hi);
            button3.Location = new Point(wi, button6.Location.Y + hi);
            button11.Location = new Point(wi, button3.Location.Y + hi);

            button20.Location = new Point(button19.Location.X + wi, 0);
            button10.Location = new Point(button9.Location.X + wi, hi);
            button7.Location = new Point(button6.Location.X + wi, button10.Location.Y + hi);
            button4.Location = new Point(button3.Location.X + wi, button7.Location.Y + hi);
            button12.Location = new Point(button11.Location.X + wi, button4.Location.Y + hi);

            progressBar1.Location = new Point(button20.Location.X + wi, 0);
            button17.Location = new Point(progressBar1.Location.X, button10.Location.Y);
            button13.Location = new Point(progressBar1.Location.X, button4.Location.Y);
            button14.Location = new Point(button12.Location.X + wi, button13.Location.Y + hi);
            button21.Location = new Point(button17.Location.X + wi, progressBar1.Location.Y + hi);
            button16.Location = new Point(button21.Location.X, button7.Location.Y);
            button15.Location = new Point(button14.Location.X + wi, button14.Location.Y);

            for (int i = 0; i < 19; i++)
            {
                box[i].Size = new System.Drawing.Size(wi, hi);
            }
            progressBar1.Size = new System.Drawing.Size(wi * 2, hi);
            button17.Size = new System.Drawing.Size(wi, hi * 2);
            button16.Size = new System.Drawing.Size(wi, hi * 2);

            //文字サイズの変更
            int w = this.Size.Width, h = this.Size.Height;
            if (w >= 427 && h >= 500)
            {
                changefont(72);
            }
            else if (w >= 252 && h >= 367)
            {
                changefont(48);
            }
            else if (w >= 210 && h >= 315)
            {
                changefont(36);
            }
            else if (w >= 179 && h >= 255)
            {
                changefont(26);
            }
            else if (w >= 160 && h >= 230)
            {
                changefont(22);
            }
            else
            {
                changefont(11);
            }
        }
        void changefont(int size)
        {
            int s;
            for (s = 0; s < 21; s++)
            {
                box[s].Font = new System.Drawing.Font("ＭＳ 明朝",size, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            
            }
        }
        void set()
        {

            box[0] = button1;
            box[1] = button2;
            box[2] = button3;
            box[3] = button4;
            box[4] = button5;
            box[5] = button6;
            box[6] = button7;
            box[7] = button8;
            box[8] = button9;
            box[9] = button10;
            box[10] = button11;
            box[11] = button12;
            box[12] = button13;
            box[13] = button14;
            box[14] = button15;
            box[15] = button18;
            box[16] = button19;
            box[17] = button20;
            box[18] = button21;
            box[19] = button16;
            box[20] = button17;

            //親フォームのフォントを引き継ぐ
            this.BackColor = Settings.Default.haikei;

            for (int i = 0; i < 21; i++)
            {
                box[i].Click += new EventHandler(tenkey_Click);
                box[i].MouseEnter += new EventHandler(tenkey_MouseEnter);
                box[i].ForeColor = Settings.Default.mojiiro;
            }
            progressBar1.Click += new EventHandler(progressBar1_Click);
            progressBar1.MouseEnter += new EventHandler(progressBar1_MouseEnter);

            if (Settings.Default.interval == -1)
            {
                progressBar1.Enabled = false;
            }
            else
            {
                progressBar1.Enabled = true;
                progressBar1.Maximum = Settings.Default.interval;
            }
        }

        //オンマウスイベント
        void tenkey_MouseEnter(object sender, EventArgs e)
        {
            timer1.Enabled = true;
            n = 0;
            progressBar1.Value = 0;
            moji = ""+((Button)sender).Tag;   
        }


        //プログレスバーイベント
        void progressBar1_Click(object sender, EventArgs e)
        {
            this.Activate();
            progressBar1.Value = 0;
            timer1.Enabled = false;
        }
        void progressBar1_MouseEnter(object sender, EventArgs e)
        {
            n = 0;
            progressBar1.Value = 0;
            timer1.Enabled = false;
        }

       
        //クリックイベント
        void tenkey_Click(object sender, EventArgs e)
        {
            progressBar1.Focus();
            n = 0;
            progressBar1.Value = 0;
            SendKeys.Send(""+((Button)sender).Tag);
        }

        //サイズチェンジイベント
        private void tenkey_SizeChanged(object sender, EventArgs e)
        {
            wi = (this.Size.Width - 8) / 5;
            hi = (this.Size.Height - 23) / 5;
            change();
        }

        //アクティブウィンドウの名前を取得
        void getname()
        {
            StringBuilder sb = new StringBuilder(100);
            IntPtr hwnd = GetForegroundWindow();    //アクティブウィンドウを取得
            GetWindowText(hwnd, sb, sb.Capacity);   //その名前を取得
            activename = sb.ToString();
        }

        //クリック直前にアクティブになっていたアプリケーションを再度アクティブに
        void activeapri()
        {
            //文字列のアプリケーションが開いているか探索
            IntPtr hwnd = FindWindow(null, activename);
            SetForegroundWindow(hwnd);  // アクティブにする
        }

        // アクティブ化しないウィンドウにする
        private void setNotActiveWindow(IntPtr hWnd)
        {
            UInt32 unSyle = GetWindowLong(hWnd, GWL.EXSTYLE);
            unSyle = (unSyle | WS_EX_NOACTIVATE);
            UInt32 unret = SetWindowLong(hWnd, GWL.EXSTYLE, unSyle);
            SetWindowPos(hWnd, IntPtr.Zero, 0, 0, 0, 0, SWP.NOMOVE | SWP.NOSIZE | SWP.NOZORDER | SWP.FRAMECHANGED);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            n++;
            if (n == Settings.Default.interval)
            {
                n = 0;
                SendKeys.Send(moji);
                if (Settings.Default.oto)
                {
                    sound.Play();
                }
                //連続
                if (Settings.Default.renzoku == false)
                {
                    timer1.Enabled = false;
                }
            }
            progressBar1.Value = (Settings.Default.interval == -1) ? 0 : n;
            
            //マウスの座標から処理
            int xx, yy;
            xx = Cursor.Position.X;
            yy = Cursor.Position.Y;

            if (xx < this.Location.X || xx > this.Location.X + this.Size.Width)
            {
                timer1.Enabled = false;
                n = 0;
                progressBar1.Value = 0;
            }
            else if (yy < this.Location.Y || yy > this.Location.Y + this.Size.Height)
            {
                timer1.Enabled = false;
                n = 0;
                progressBar1.Value = 0;
            }

        }

        #region Win32API
        const UInt32 WS_EX_NOACTIVATE = 0x8000000;  // アクティブ化されないスタイル
        private enum GWL : int
        {
            WINDPROC = -4,
            HINSTANCE = -6,
            HWNDPARENT = -8,
            STYLE = -16,
            EXSTYLE = -20,  
            USERDATA = -21,
            ID = -12
        }
        private enum SWP : int
        {
            NOSIZE = 0x0001,
            NOMOVE = 0x0002,
            NOZORDER = 0x0004,
            NOREDRAW = 0x0008,
            NOACTIVATE = 0x0010,
            FRAMECHANGED = 0x0020,
            SHOWWINDOW = 0x0040,
            HIDEWINDOW = 0x0080,
            NOCOPYBITS = 0x0100,
            NOOWNERZORDER = 0x0200,
            NOSENDCHANGING = 0x400
        }
        #endregion




    }

}
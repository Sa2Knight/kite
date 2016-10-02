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
using System.IO;

namespace ScreenKeyboard
{
    public partial class key : Form
    {
        #region 主にコントロールに関する変数
        StringReader str;
        Label[] labe = new Label[80];       //ラベル
        Button[] button1 = new Button[16];   //入力用ボタン
        Button[] button2 = new Button[5];   //カテゴリ用ボタン
        Button SHIFT, CTRL, ALT;       //特殊キー
        Color[] bc = new Color[110];        //背景色ボックス
        Color[] cc = new Color[110];        //選択色ボックス
        String[,] txt = new String[110, 5]; //表示文字ボックス
        String[,] inp = new String[110, 5]; //入力文字ボックス
        ProgressBar progress;               //プログレスバー
        int cnt0 = 0, cnt1 = 0, cnt2 = 0, cnt3 = 0, sum = 0; //各コントロールの個数
        #endregion

        #region スクリーンキーボードを操作するための変数
        bool scflag = false;
        bool alt =true, shift = true, ctrl = true;
        string activename;
        string[] input = new string[110];
        string moji = "";
        int n=0,interval = 15,keyflag=0;
        bool ren = false,cli=true;
        SoundPlayer sound = null;

        

        #endregion

        #region 下準備

        #region DLLインポート

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

        #region SendInput

        [DllImport("user32.dll")]
        extern static int SendInput(
        int nInputs,   // INPUT 構造体の数(イベント数)
        ref INPUT pInputs,   // INPUT 構造体
        int cbSize     // INPUT 構造体のサイズ
        );

        [DllImport("user32.dll")]
        static extern IntPtr GetMessageExtraInfo();

        [StructLayout(LayoutKind.Explicit)]  // アンマネージ DLL 対応用 struct 記述宣言
        struct INPUT
        {
            [FieldOffset(0)]
            public int type;  // 0 = INPUT_MOUSE(デフォルト), 1 = INPUT_KEYBOARD
            [FieldOffset(4)]
            public MOUSEINPUT mi;
            [FieldOffset(4)]
            public KEYBDINPUT ki;
            [FieldOffset(4)]
            public HARDWAREINPUT hi;
            // Note: struct の場合、デフォルト(パラメータなしの)コンストラクタは、
            //       言語側で定義済みで、フィールドを 0 に初期化する。
        }

        private struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public int mouseData;  // amount of wheel movement
            public int dwFlags;
            public int time;  // time stamp for the event
            public IntPtr dwExtraInfo;
            // Note: struct の場合、デフォルト(パラメータなしの)コンストラクタは、
            //       言語側で定義済みで、フィールドを 0 に初期化する。
        }

        private struct KEYBDINPUT
        {
            public int wVk;
            public int wScan;
            public int dwFlags;
            public int time;
            public IntPtr dwExtraInfo;
        }

        private struct HARDWAREINPUT        //For Win98 Win Me
        {
            public uint uMSG;
            public ushort wParamL;
            public ushort wParamH;
        }


        #region キーコード
        const int VK_LBUTTON = 0x01;
        const int VK_RBUTTON = 0x02;
        const int VK_LWIN = 0x5B;
        const int VK_RWIN = 0x5C;
        const int VK_A_KEY = 0x41;
        const int VK_ALT_KEY = 0x12;
        const int VK_I_KEY = 0x49;
        const int VK_U_KEY = 0x55;
        const int VK_E_KEY = 0x45;
        const int VK_O_KEY = 0x4F;
        const int VK_K_KEY = 0x4B;
        const int VK_S_KEY = 0x53;
        const int VK_T_KEY = 0x54;
        const int VK_N_KEY = 0x4E;
        const int VK_H_KEY = 0x48;
        const int VK_SPACE = 0x20;
        const int VK_CONVERT = 0x1C;
        const int VK_RETURN = 0x0D;
        const int VK_BACK = 0x08;
        const int VK_DELETE = 0x2E;
        const int INPUT_KEYBOARD = 1;
        const int VK_SHIFT = 0x10;
        const int VK_KANJI = 0x19;
        const int VK_HANGUL = 0x15;
        const int VK_CONTROL = 0x11;

        #endregion

        #endregion

        //起動時
        public key()
        {
            InitializeComponent();
        }

        //フォームロード
        private void key_Load(object sender, EventArgs e)
        {

            int i, s;
            str = new StringReader(Settings.Default.pure);

            //フォームサイズ
            int xx = Convert.ToInt32(str.ReadLine());
            int yy = Convert.ToInt32(str.ReadLine());
            this.Size = new Size(xx, yy);
            //フォームカラー
            this.BackColor = Color.FromArgb(Convert.ToInt32(str.ReadLine()), Convert.ToInt32(str.ReadLine()), Convert.ToInt32(str.ReadLine()), Convert.ToInt32(str.ReadLine()));

            //背景画像
            int imafu = Convert.ToInt32(str.ReadLine());
            try
            {
                Image ima = Image.FromFile(str.ReadLine());
                if (imafu == 0)
                {
                    this.BackgroundImage = new Bitmap(ima);
                }
                else
                {
                    this.BackgroundImage = new Bitmap(ima, this.Size);
                }
            }
            catch
            {
                this.BackgroundImage = null;
            }

            //入力サウンド
            string lo = str.ReadLine();
            try
            {
                sound = new SoundPlayer(lo);
            }
            catch
            {
                if (lo != "")
                MessageBox.Show(lo + "が見つかりません");
            }


            //フォームイベント
            this.MouseDown += new MouseEventHandler(form_mousedown);
            this.MouseEnter += new EventHandler(form_mouseEnter);

            //ラベル
            for (i = 0; i <= 79; i++)
            {
                //有効無効
                if (Convert.ToInt32(str.ReadLine()) == 1)
                {
                    //インスタンス生成
                    labe[cnt0] = new Label();

                    //ラベルのイベント登録
                    labe[cnt0].MouseDown += new MouseEventHandler(label_mousedown);
                    labe[cnt0].MouseEnter += new EventHandler(label_mouseEnter);
                    labe[cnt0].MouseLeave += new EventHandler(label_mouseLeave);

                    //位置
                    labe[cnt0].Location = new Point(Convert.ToInt32(str.ReadLine()), Convert.ToInt32(str.ReadLine()));

                    //デフォ設定
                    labe[cnt0].TextAlign = ContentAlignment.MiddleCenter;
                    labe[cnt0].BorderStyle = BorderStyle.Fixed3D;
                    labe[cnt0].Tag = cnt0;

                    //サイズ
                    labe[cnt0].Size = new Size((Convert.ToInt32(str.ReadLine())), Convert.ToInt32(str.ReadLine()));

                    //背景色
                    labe[cnt0].BackColor = Color.FromArgb(Convert.ToInt32(str.ReadLine()), Convert.ToInt32(str.ReadLine()), Convert.ToInt32(str.ReadLine()), Convert.ToInt32(str.ReadLine()));
                    bc[cnt0] = labe[cnt0].BackColor;

                    //選択色
                    cc[cnt0] = Color.FromArgb(Convert.ToInt32(str.ReadLine()), Convert.ToInt32(str.ReadLine()), Convert.ToInt32(str.ReadLine()), Convert.ToInt32(str.ReadLine()));

                    //文字色
                    labe[cnt0].ForeColor = Color.FromArgb(Convert.ToInt32(str.ReadLine()), Convert.ToInt32(str.ReadLine()), Convert.ToInt32(str.ReadLine()), Convert.ToInt32(str.ReadLine()));

                    //フォント
                    string name = str.ReadLine();       //書式名
                    float size = (float)Convert.ToDouble(str.ReadLine());   //サイズ
                    string st = str.ReadLine();
                    FontStyle n;    //太字とかオプション
                    if (st[0] == 'B') n = FontStyle.Bold;
                    else if (st[0] == 'I') n = FontStyle.Italic;
                    else if (st[0] == 'R') n = FontStyle.Regular;
                    else if (st[0] == 'S') n = FontStyle.Strikeout;
                    else n = FontStyle.Underline;
                    labe[cnt0].Font = new Font(name, size, n);

                    for (s = 0; s < 5; s++)
                    {
                        //表示文字と入力文字
                        txt[cnt0, s] = str.ReadLine();
                        inp[cnt0, s] = str.ReadLine();
                    }
                    labe[cnt0].Text = txt[cnt0, 0];
                    input[cnt0] = inp[cnt0, 0];

                    string m = str.ReadLine();
                    if (m == "1")
                    {
                        labe[cnt0].BorderStyle = BorderStyle.None;
                    }
                    else if (m == "2")
                    {
                        labe[cnt0].BorderStyle = BorderStyle.Fixed3D;
                    }
                    else
                    {
                        labe[cnt0].BorderStyle = BorderStyle.FixedSingle;
                    }

                    if (input[cnt0] == "") labe[cnt0].Enabled = false;
                    //フォームにコントロールを追加
                    this.Controls.Add(labe[cnt0]);
                    cnt0++;


                }
                //無効なら総スルー
                else
                {
                    for (s = 0; s < 30; s++) str.ReadLine();
                }
            }

            sum = cnt0;

            //入力用ボタン
            for (i = 80; i <= 94; i++)
            {
                //有効無効
                if (Convert.ToInt32(str.ReadLine()) == 1)
                {
                    //インスタンスの生成
                    button1[cnt1] = new Button();

                    //ボタンのイベント登録
                    button1[cnt1].MouseDown += new MouseEventHandler(inputbutton_MouseDown);
                    button1[cnt1].MouseEnter += new EventHandler(inputbutton_MouseEnter);
                    button1[cnt1].MouseLeave += new EventHandler(button_MouseLeave);
                    button1[cnt1].MouseEnter += new EventHandler(inputbutton_MouseEnter);

                    //デフォ設定
                    button1[cnt1].Tag = cnt1 + sum;
                    button1[cnt1].TextAlign = ContentAlignment.MiddleCenter;

                    //位置
                    button1[cnt1].Location = new Point(Convert.ToInt32(str.ReadLine()), Convert.ToInt32(str.ReadLine()));

                    //サイズ
                    button1[cnt1].Size = new Size((Convert.ToInt32(str.ReadLine())), Convert.ToInt32(str.ReadLine()));

                    //背景色
                    button1[cnt1].BackColor = Color.FromArgb(Convert.ToInt32(str.ReadLine()), Convert.ToInt32(str.ReadLine()), Convert.ToInt32(str.ReadLine()), Convert.ToInt32(str.ReadLine()));
                    bc[sum + cnt1] = button1[cnt1].BackColor;

                    //選択色
                    cc[sum + cnt1] = Color.FromArgb(Convert.ToInt32(str.ReadLine()), Convert.ToInt32(str.ReadLine()), Convert.ToInt32(str.ReadLine()), Convert.ToInt32(str.ReadLine()));

                    //文字色
                    button1[cnt1].ForeColor = Color.FromArgb(Convert.ToInt32(str.ReadLine()), Convert.ToInt32(str.ReadLine()), Convert.ToInt32(str.ReadLine()), Convert.ToInt32(str.ReadLine()));

                    //フォント
                    string name = str.ReadLine();       //書式名
                    float size = (float)Convert.ToDouble(str.ReadLine());   //サイズ
                    string st = str.ReadLine();
                    FontStyle n;    //太字とかオプション
                    if (st[0] == 'B') n = FontStyle.Bold;
                    else if (st[0] == 'I') n = FontStyle.Italic;
                    else if (st[0] == 'R') n = FontStyle.Regular;
                    else if (st[0] == 'S') n = FontStyle.Strikeout;
                    else n = FontStyle.Underline;
                    button1[cnt1].Font = new Font(name, size, n);

                    for (s = 0; s < 5; s++)
                    {
                        //表示文字と入力文字
                        txt[sum + cnt1, s] = str.ReadLine();
                        inp[sum + cnt1, s] = str.ReadLine();
                    }
                    button1[cnt1].Text = txt[sum + cnt1, 0];
                    input[cnt1 + sum] = inp[sum + cnt1, 0];
                    if (input[cnt1 + sum] == "") button1[cnt1].Enabled = false;
                    //フォームにコントロールの追加
                    this.Controls.Add(button1[cnt1]);
                    cnt1++;
                }
                //無効なら総スルー
                else
                {
                    for (s = 0; s < 29; s++) str.ReadLine();
                }
            }

            sum += cnt1;

            //カテゴリ用ボタン
            for (i = 95; i <= 99;  i++)
            {
                //有効無効
                if (Convert.ToInt32(str.ReadLine()) == 1)
                {
                    //インスタンスの生成
                    button2[cnt2] = new Button();

                    //ボタンのイベント登録
                    button2[cnt2].MouseDown += new MouseEventHandler(Kategori_MouseDown);
                    button2[cnt2].MouseEnter += new EventHandler(Kategori_MouseEnter);
                    button2[cnt2].MouseLeave += new EventHandler(button_MouseLeave);

                    //デフォ設定
                    button2[cnt2].Tag = sum + cnt2;
                    button2[cnt2].TextAlign = ContentAlignment.MiddleCenter;
                    button2[cnt2].Text = str.ReadLine();
             
                    //位置
                    button2[cnt2].Location = new Point(Convert.ToInt32(str.ReadLine()), Convert.ToInt32(str.ReadLine()));

                    //サイズ
                    button2[cnt2].Size = new Size((Convert.ToInt32(str.ReadLine())), Convert.ToInt32(str.ReadLine()));

                    //背景色
                    button2[cnt2].BackColor = Color.FromArgb(Convert.ToInt32(str.ReadLine()), Convert.ToInt32(str.ReadLine()), Convert.ToInt32(str.ReadLine()), Convert.ToInt32(str.ReadLine()));
                    bc[sum + cnt2] = button2[cnt2].BackColor;

                    //選択色
                    cc[sum + cnt2] = Color.FromArgb(Convert.ToInt32(str.ReadLine()), Convert.ToInt32(str.ReadLine()), Convert.ToInt32(str.ReadLine()), Convert.ToInt32(str.ReadLine()));

                    //文字色
                    button2[cnt2].ForeColor = Color.FromArgb(Convert.ToInt32(str.ReadLine()), Convert.ToInt32(str.ReadLine()), Convert.ToInt32(str.ReadLine()), Convert.ToInt32(str.ReadLine()));

                    //フォント
                    string name = str.ReadLine();       //書式名
                    float size = (float)Convert.ToDouble(str.ReadLine());   //サイズ
                    string st = str.ReadLine();
                    FontStyle n;    //太字とかオプション
                    if (st[0] == 'B') n = FontStyle.Bold;
                    else if (st[0] == 'I') n = FontStyle.Italic;
                    else if (st[0] == 'R') n = FontStyle.Regular;
                    else if (st[0] == 'S') n = FontStyle.Strikeout;
                    else n = FontStyle.Underline;
                    button2[cnt2].Font = new Font(name, size, n);

                    this.Controls.Add(button2[cnt2]);
                    cnt2++;
                }
                //無効ならスルー
                else
                {
                    for (s = 0; s < 20; s++) str.ReadLine();
                }
            }

            sum += cnt2;

            //特殊キー
            for (i = 100; i <= 102; i++)
            {
                //有効無効
                if (Convert.ToInt32(str.ReadLine()) == 1)
                {
                    Button b;
                    //インスタンスの生成
                    b = new Button();

                    //デフォ設定
                    b.Tag = sum + cnt3;
                    b.TextAlign = ContentAlignment.MiddleCenter;
                    b.Text = str.ReadLine();

                    //位置
                    b.Location = new Point(Convert.ToInt32(str.ReadLine()), Convert.ToInt32(str.ReadLine()));

                    //サイズ
                    b.Size = new Size((Convert.ToInt32(str.ReadLine())), Convert.ToInt32(str.ReadLine()));

                    //背景色
                    b.BackColor = Color.FromArgb(Convert.ToInt32(str.ReadLine()), Convert.ToInt32(str.ReadLine()), Convert.ToInt32(str.ReadLine()), Convert.ToInt32(str.ReadLine()));
                    bc[sum + cnt3] = b.BackColor;

                    //選択色
                    cc[sum + cnt3] = Color.FromArgb(Convert.ToInt32(str.ReadLine()), Convert.ToInt32(str.ReadLine()), Convert.ToInt32(str.ReadLine()), Convert.ToInt32(str.ReadLine()));

                    //文字色
                    b.ForeColor = Color.FromArgb(Convert.ToInt32(str.ReadLine()), Convert.ToInt32(str.ReadLine()), Convert.ToInt32(str.ReadLine()), Convert.ToInt32(str.ReadLine()));

                    //フォント
                    string name = str.ReadLine();       //書式名
                    float size = (float)Convert.ToDouble(str.ReadLine());   //サイズ
                    string st = str.ReadLine();
                    FontStyle n;    //太字とかオプション
                    if (st[0] == 'B') n = FontStyle.Bold;
                    else if (st[0] == 'I') n = FontStyle.Italic;
                    else if (st[0] == 'R') n = FontStyle.Regular;
                    else if (st[0] == 'S') n = FontStyle.Strikeout;
                    else n = FontStyle.Underline;
                    b.Font = new Font(name, size, n);

                    //フォームにコントロールの追加
                    this.Controls.Add(b);
                    cnt3++;

                    //b.MouseLeave += new EventHandler(button_MouseLeave);

                    //どの特殊キーなのか
                    if (i == 100)
                    {
                        SHIFT = b;
                        SHIFT.MouseDown += new MouseEventHandler(SHIFT_MouseDown);
                        SHIFT.MouseEnter += new EventHandler(Shift_MouseEnter);
                    }
                    else if (i == 101)
                    {
                        CTRL = b;
                        CTRL.MouseDown += new MouseEventHandler(CTLR_MouseDown);
                        CTRL.MouseEnter += new EventHandler(CTRL_MouseEnter);
                    }
                    else
                    {
                        ALT = b;
                        ALT.MouseDown += new MouseEventHandler(ALT_MouseDown);
                        ALT.MouseEnter += new EventHandler(ALT_MouseEnter);
                    }

                }
                //無効ならスルー
                else
                {
                    for (s = 0; s < 20; s++) str.ReadLine();
                }
            }

            //プログレスバー
            if (Convert.ToInt32(str.ReadLine()) == 1)
            {
                //インスタンスの生成
                progress = new ProgressBar();

                //位置、サイズ
                progress.Location = new Point(Convert.ToInt32(str.ReadLine()), Convert.ToInt32(str.ReadLine()));
                progress.Size = new Size(Convert.ToInt32(str.ReadLine()), Convert.ToInt32(str.ReadLine()));
                //設定
                progress.Maximum = 15;
                progress.Minimum = 0;

                //プログレスバーをコントロールに追加
                this.Controls.Add(progress);

                //プログレスバーのイベント登録
                progress.MouseDown += new MouseEventHandler(progress_MouseDown);
                progress.MouseEnter += new EventHandler(progress_MouseEnter);
                progress.MouseLeave+=new EventHandler(key_MouseLeave);
            }
            else
            {
                for (i = 0; i < 4; i++) str.ReadLine();
            }

            //透過度
            this.Opacity = Convert.ToDouble(str.ReadLine()) / 100.0;
            //連続入力
            ren = (Convert.ToInt32(str.ReadLine()) == 1) ? true : false;
            //入力待ち時間
            interval = Convert.ToInt32(str.ReadLine()) + 1;
            if (progress != null) progress.Maximum = interval;
            //クリックで入力
            cli = (Convert.ToInt32(str.ReadLine()) == 1) ? true : false;
            //オンマウスで入力
            if(Convert.ToInt32(str.ReadLine()) == 0)
            timer1.Tick -= timer1_Tick;
            
            setNotActiveWindow(this.Handle);
            str.Close();
        }

        #endregion

        #region イベント

        #region マウスダウン
        //プログレスバー
        void progress_MouseDown(object sender, MouseEventArgs e)
        {
            activeapri();
        }

        //入力用ボタン
        void inputbutton_MouseDown(object sender, MouseEventArgs e)
        {
            activeapri();
            n = 0;
            if(cli) send(input[(int)(((Button)sender).Tag)]);
        }

        //カテゴリボタン
        void Kategori_MouseDown(object sender, MouseEventArgs e)
        {
            activeapri();
            n = 0;
            if (!cli) return;
            int k = (int)(((Button)sender).Tag) - (int)button2[0].Tag;
            int i;
            for (i = 0; i < cnt0; i++)
            {
                Label la = ((Label)labe[i]);
                la.Text = txt[(int)la.Tag, k];
                input[(int)la.Tag] = inp[(int)la.Tag, k];
                la.Enabled = (input[(int)la.Tag] == "") ? false : true;
            }
            for (i = 0; i < cnt1; i++)
            {
                Button bu = ((Button)button1[i]);
                bu.Text = txt[(int)bu.Tag, k];
                input[(int)bu.Tag] = inp[(int)bu.Tag, k];
                bu.Enabled = (input[(int)bu.Tag] == "") ? false : true;
            }
        }
    
        //ＡＬＴキー
        void ALT_MouseDown(object sender, MouseEventArgs e)
        {
            activeapri();
            n = 0;
            if (!cli) return;
            //altキーオプションのオンオフ
            if (alt)
            {
                send_input_down(VK_ALT_KEY);
                ALT.BackColor = cc[(int)ALT.Tag];
                alt = false;
            }
            else
            {
                send_input_up(VK_ALT_KEY);
                ALT.BackColor = bc[(int)ALT.Tag];
                alt = true;
            }
        }

        //ＣＴＬＲキー
        void CTLR_MouseDown(object sender, MouseEventArgs e)
        {
            activeapri();
            n = 0;
            if (!cli) return;
            //ctrlキーオプションのオンオフ
            if (ctrl)
            {
                send_input_down(VK_CONTROL);
                CTRL.BackColor = cc[(int)CTRL.Tag];
                ctrl = false;
            }
            else
            {
                send_input_up(VK_CONTROL);
                CTRL.BackColor = bc[(int)CTRL.Tag];
                ctrl = true;
            }
        }

        //ＳＨＩＦＴキー
        void SHIFT_MouseDown(object sender, MouseEventArgs e)
        {
            activeapri();
            n = 0;
            if (!cli) return;
            //SHIFTキーオプションのオンオフ
            if (shift)
            {
                send_input_down(VK_SHIFT);
                SHIFT.BackColor = cc[(int)SHIFT.Tag];
                shift = false;
            }
            else
            {
                send_input_up(VK_SHIFT);
                SHIFT.BackColor = bc[(int)SHIFT.Tag];
                shift = true;
            }
        }

        //ラベル
        void label_mousedown(object sender, MouseEventArgs e)
        {
            activeapri();
            if(cli)send(input[(int)(((Label)sender).Tag)]);
        }

        //フォーム
        void form_mousedown(object sender, MouseEventArgs e)
        {
            activeapri();
        }
        #endregion

        #region マウスエンター

        //プログレスバー
        void progress_MouseEnter(object sender, EventArgs e)
        {
            timer1.Stop();
            progress.Value = 0;
            n = 0;
            getname();
            moji = "";
        }

        //入力ボタン
        void inputbutton_MouseEnter(object sender, EventArgs e) 
        {
            getnext();
            Button bu = ((Button)sender);
            int tag = (int)bu.Tag;
            bu.BackColor = cc[tag];
            moji = input[tag];
            keyflag = 1;
        }

        //カテゴリボタン
        void Kategori_MouseEnter(object sender, EventArgs e) 
        {
            getnext();
            Button bu = ((Button)sender);
            int tag = (int)bu.Tag;
            bu.BackColor = cc[tag];
            moji = tag.ToString();
            keyflag = 2;
        }

        //サイズチェンジボタン
        void SCB_MouseEnter(object sender, EventArgs e) 
        {
            getnext();
            Button bu = ((Button)sender);
            int tag = (int)bu.Tag;
            bu.BackColor = cc[tag];
            keyflag = 3;
        }

        //ＳＨＩＦＴ
        void Shift_MouseEnter(object sender, EventArgs e) 
        {
            getnext();
            Button bu = ((Button)sender);
            int tag = (int)bu.Tag;
            //bu.BackColor = cc[tag];
            keyflag = 4;
        }

        //CTRL
        void CTRL_MouseEnter(object sender, EventArgs e) 
        {
            getnext();
            Button bu = ((Button)sender);
            int tag = (int)bu.Tag;
            //bu.BackColor = cc[tag];
            keyflag = 5;
        }

        //ALT
        void ALT_MouseEnter(object sender, EventArgs e) 
        {
            getnext();
            Button bu = ((Button)sender);
            int tag = (int)bu.Tag;
            //bu.BackColor = cc[tag];
            keyflag = 6;
        }

        //ラベル
        void label_mouseEnter(object sender, EventArgs e)
        {
            getnext();
            Label la = (Label)sender;
            int tag = (int)la.Tag;
            la.BackColor = cc[tag];
            moji = input[tag];
            keyflag = 0;
        }

        //フォーム
        void form_mouseEnter(object sender, EventArgs e)
        {
            getname();
            if(progress!=null) progress.Value = 0;
            n = 0;
            timer1.Stop();
        }
        #endregion

        #region マウスリーヴ

        //ボタン
        void button_MouseLeave(object sender, EventArgs e)
        {
            Button bu = (Button)sender;
            int tag = (int)bu.Tag;
            bu.BackColor = bc[tag];
            key_MouseLeave(null, null);
        }

        //ラベル
        void label_mouseLeave(object sender, EventArgs e)
        {
            Label la = (Label)sender;
            int tag = (int)la.Tag;
            la.BackColor = bc[tag];
            key_MouseLeave(null, null);
        }

        //フォーム
        private void key_MouseLeave(object sender, EventArgs e)
        {
            timer1.Stop();
            n = 0;
            if (progress != null) progress.Value = 0;
        }

        #endregion


        #endregion

        #region よく使う処理群

        //タイマー入力
        private void timer1_Tick(object sender, EventArgs e)
        {
          
            n++;
            if (n >= interval)
            {
                //ラベル入力
                if (keyflag == 0)
                {
                    send(moji);
                }
                //ボタン入力
                else if (keyflag == 1)
                {
                    send(moji);
                }
                //カテゴリ変更
                else if(keyflag==2)
                {
                    int m = Convert.ToInt32(moji);
                    Kategori_MouseDown(button2[m - cnt1 - cnt0], null);
                }
                //サイズチェンジ
                else if (keyflag == 3)
                {
                }
                //SHIFT
                else if (keyflag == 4)
                {
                    SHIFT_MouseDown(SHIFT, null);
                }
                //CTRL
                else if (keyflag == 5)
                {
                    CTLR_MouseDown(CTRL, null);
                } 
                else
                {
                    ALT_MouseDown(ALT, null);
                }
                if(progress!=null) progress.Value = 0;
                if(!ren)timer1.Stop();
                n = 0;
            }
            if(progress!=null) progress.Value = n;
        }

        //SendKeys
        void send(string s)
        {

            if (s[0] == '@' && s[1] == '/')
            {
                readtxt(s.Substring(2));
            }
            else
            {
                SendKeys.Send(s);
            }
            n = 0;
            if (sound != null) sound.Play();
            if (progress != null) progress.Value = 0;
        }

        //オンマウス後の基本処理
        void getnext()
        {
            getname();
            timer1.Start();
            n = 0;
            if (progress == null) return;
            progress.Value = 0;
        }

        //Sendinput向けの文字列を解析
        void readtxt(string n)
        {
            //このメソッドに届く引数は既に@/を抜いた文字列
            int i = 0;
            bool s = false, c = false, a = false;
            int code;

            //最初の「/」が出るまでは修飾キーの有無を表す
            while (n[i] != '/')
            {
                if (n[i] == '+') s = true;
                if (n[i] == '^') c = true;
                if (n[i] == '%') a = true;
                i++;
            }

            //(i+1)から３文字がキーコードを表す
            code = Convert.ToInt32(n.Substring(i + 1, 3));
            send_input_down(code);
        }

        //SendInput用メソッド
        Action<int> send_input_down = (key) =>
        {
            INPUT input = new INPUT();
            input.type = 1;
            input.ki.wScan = 0;
            input.ki.time = 3000;
            input.ki.dwExtraInfo = GetMessageExtraInfo();

            input.ki.wVk = key;
            input.ki.dwFlags = 0;

            SendInput(1, ref input, Marshal.SizeOf(typeof(INPUT)));
        };
        Action<int> send_input_up = (key1) =>
        {
            INPUT input = new INPUT();
            input.type = 1;
            input.ki.wScan = 0;
            input.ki.time = 0;
            input.ki.dwExtraInfo = GetMessageExtraInfo();

            input.ki.wVk = key1;
            input.ki.dwFlags = 2;       //key_up
            SendInput(1, ref input, Marshal.SizeOf(typeof(INPUT)));
        };

        #endregion

        #region アクティブ化しないようにするための処理群

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

        #endregion

    }
}
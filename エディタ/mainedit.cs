using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Collections;
using System.Threading;
using System.Media;

namespace ScreenKeyboard
{
    public partial class mainedit : Form
    {
        public mainedit()
        {
            InitializeComponent();
        }
       


        #region 変数・構造体・列挙体の宣言

        //定数
        const int haba = 4;
        const int hose = 0;
        const int cnt = 104;
        const int maxhis = 100;

        //各コントロールの構造体
        public struct data
        {
            public Control con;   //コントロール
            public Color bc;     //背景色
            public Color cc;     //選択色
            public string name;  //ソフト上での名前
            //入力されるテキスト
            public string[] txt;
            public string[] inp;
        }
        //元に戻すで使う構造体
        public struct history
        {
            public int[] con;   //対象のコントロール
            public ArrayList databack;  //変更前のデータ 
            public ArrayList datatoward;
            public short mode;      //変更したデータの種類
        }
        public short moh;
        public object dah;
        //元に戻すで使う列挙体
        enum back { font, moji, en, lo, si, bc, cc, h1, h2, h3, h4, h5, i1, i2, i3, i4, i5, kuu, text,che,bord,back,sound,num,onnum};
        
        
        //コントロール用構造体インスタンス
        public data[] box = new data[120];

        Image backimage=null;
        SoundPlayer sound = null;
        
        bool imageflag = false;
        public history[] his = new history[maxhis];
        history lasthistory = new history();
        public Point[] hispoint;
        public Size[] hissize;
        
        public short onflag = 0;   //マウスが押されているか
        Control getcont;            //選択されたコントロール
        int cursorx, cursory;      //直前のマウスポジション
        int mousex, mousey;        //フォーム内の現在のマウスポジション
        int clickx, clicky;        //コントロールのクリックした場所
        int xr, xl, yu, yd;
        bool dorafla = false;
        bool tuika = false;
        public bool  soroe = false;
        public bool hukusuu = false;
        bool saveflag = true;
        int gori = 0;
        public int pops = 0;

        //複数選択時のスタック
        public Control[] stack = new Control[115];
        public short pop = -1;
        public BOSS bo;

        #endregion

        #region マウスを使っての操作

        //マウスポインタの位置でカーソルの種類を変更する
        void changecursor(Control n, MouseEventArgs e)
        {
            if (onflag == 0)
            {
                //左に伸ばす
                if (e.X <= haba)
                {
                    //左上
                    if (e.Y <= haba)
                    {
                        Cursor = Cursors.SizeNWSE;
                    }
                    //左下
                    else if (e.Y >= n.Size.Height - haba)
                    {
                        Cursor = Cursors.SizeNESW;
                    }
                    else
                    {
                        Cursor = Cursors.SizeWE;
                    }
                }
                //右に伸ばす
                else if (e.X >= n.Size.Width - haba)
                {
                    //右上
                    if (e.Y <= haba)
                    {
                        Cursor = Cursors.SizeNESW;
                    }
                    //右下
                    else if (e.Y >= n.Size.Height - haba)
                    {
                        Cursor = Cursors.SizeNWSE;
                    }
                    else
                    {
                        Cursor = Cursors.SizeWE;
                    }
                }
                //上に伸ばす
                else if (e.Y <= haba)
                {

                    Cursor = Cursors.SizeNS;
                }
                //下に伸ばす
                else if (e.Y >= n.Size.Height - haba)
                {

                    Cursor = Cursors.SizeNS;
                }

                //真ん中
                else
                {
                    Cursor = Cursors.Default;
                }
            }
        }
        
        //クリックした場所に応じて位置変更、サイズ変更のモードに切り替え
        void returnclick(Control n, MouseEventArgs e)
        {
            if (getcont == n || hukusuu == true)
            {
                //左端をつまむ
                if (clickx <= haba)
                {
                    if (e.Y <= haba)
                    {
                        onflag = 9;
                    }
                    else if (e.Y >= n.Size.Height - haba)
                    {
                        onflag = 7;
                    }
                    else
                    {
                        onflag = 3;
                    }
                }

                //右端をつまむ
                else if (clickx >= n.Size.Width - haba)
                {
                    if (e.Y <= haba)
                    {
                        onflag = 8;
                    }
                    else if (e.Y >= n.Size.Height - haba)
                    {
                        onflag = 6;
                    }
                    else
                    {
                        onflag = 2;
                    }
                }

                //上をつまむ
                else if (e.Y <= haba)
                {
                    onflag = 5;
                }

                //下をつまむ
                else if (e.Y >= n.Size.Height - haba)
                {
                    onflag = 4;
                }

                //中央
                else
                {
                    onflag = 1;
                }
            }
            else
            {
                onflag = 1;
            }

            //履歴の作成
            if (onflag == 1)
            {
                //複数なら
                if (hukusuu)
                {
                    //選択分配列確保
                    hispoint = new Point[pop + 1];
                    for (int i = 0; i <= pop; i++)
                    {
                        hispoint[i] = stack[i].Location;
                    }
                }
                else
                {
                    hispoint = new Point[1];
                    hispoint[0] = getcont.Location;
                }
            }
            else
            {
                if (hukusuu)
                {
                    hissize = new Size[pop + 1];
                    hispoint = new Point[pop + 1];
                    for (int i = 0; i <= pop; i++)
                    {
                        hissize[i] = stack[i].Size;
                        hispoint[i] = stack[i].Location;
                    }
                }
                else
                {
                    hissize = new Size[1];
                    hissize[0] = getcont.Size;
                    hispoint = new Point[1];
                    hispoint[0] = getcont.Location;
                }
            }
        }
        
        //ドラッグによる移動及びサイズ変更
        void sizechange(Control n)
        {
            int xx = Cursor.Position.X - cursorx;
            int yy = Cursor.Position.Y - cursory;
            cursorx = Cursor.Position.X;
            cursory = Cursor.Position.Y;
            switch (onflag)
            {
                case 1:
                    n.Location = new Point(mousex - clickx, mousey - clicky);
                    break;
                case 2:
                    n.Width = mousex-n.Left;
                    break;
                case 3:
                    int w = n.Width;
                    n.Width += n.Left-mousex;
                    n.Left += w-n.Width;
                    break;
                case 4:
                    n.Height = mousey - n.Top + hose;
                    break;
                case 5:
                    int h = n.Height;
                    n.Height += n.Top - mousey-hose;
                    n.Top += h - n.Height;
                    break;
                case 6:
                    n.Width = mousex - n.Left;
                    n.Height = mousey - n.Top + hose;
                    break;
                case 7:
                    int a = n.Width;
                    n.Width += n.Left - mousex;
                    n.Left += a - n.Width;
                    n.Height = mousey - n.Top + hose;
                    break;
                case 8:
                    n.Width = mousex - n.Left;
                    int z = n.Height;
                    n.Height += n.Top - mousey - hose;
                    n.Top += z - n.Height;
                    break;
                default:
                    int m = n.Width;
                    n.Width += n.Left - mousex;
                    n.Left += m - n.Width;
                    m = n.Height;
                    n.Height += n.Top - mousey - hose;
                    n.Top += m - n.Height;
                    break;
            }
            bo.setpropatymini(n, 1);
        }
        
        //フォームドラッグで範囲選択を行う
        void formdorag(MouseEventArgs e)
        {
            if (dorafla)
            {

                xr = e.X;
                xl = cursorx;
                yu = cursory;
                yd = e.Y;
                if (xl > xr)
                {
                    int n = xl;
                    xl = xr;
                    xr = n;
                }
                if (yu > yd)
                {
                    int n = yu;
                    yu = yd;
                    yd = n;
                }

                Invalidate();
            }
        }
        //範囲選択によって複数選択されたコントロールを一度に操作する
        void doragmaster()
        {
            //複数のコントロールを移動する
            if (onflag == 1)
            {
                Point pp;
                short p = pop;

                //まず一つのコントロールを通常ルーチンで移動し、移動前後のポイント差分を出す
                Control c = stack[0];
                pp = c.Location;
                sizechange(c);
                pp = new Point(c.Location.X - pp.X, c.Location.Y - pp.Y);

                //その差分だけ選択されたコントロールを移動する
                do
                {
                    //スタックにラベルが積んである
                    Control n = stack[p];
                    n.Location = new Point(n.Location.X + pp.X, n.Location.Y + pp.Y);
                    p--;
                } while (p > 0);
            }

            //複数のコントロールのサイズを変える
            else
            {
                //まず一つのコントロールのサイズ通常ルーチンでチェンジし、チェンジ前後のサイズ差を出す
                Size s;
                Point po;
                Control l = stack[0];
                s = l.Size;
                po = l.Location;
                sizechange(l);
                s = l.Size - s;
                po = new Point(l.Location.X - po.X, l.Location.Y - po.Y);


                //求めたサイズ差をもとに選択されたすべてのコントロールのサイズを変更する
                int p = pop;
                do
                {
                    Control m = stack[p];
                    m.Size += s;
                    m.Location = new Point(m.Location.X + po.X, m.Location.Y + po.Y);

                    p--;
                } while (p > 0);

            }
        }

        //カテゴリボタンにオンマウスで表示を切り替える
        public void changekategori(object sender, EventArgs e)
        {
            gori = (int)(((Button)sender).Tag) - 95;
            int i;
            for (i = 0; i <= 94; i++)
            {
                ((Control)box[i].con).Text = box[i].txt[gori];
            }
        }

        //リストボックスからコントロールを有効にする
        public void inputitems() 
        {
            try
            {
                resetstack();
                int i;
                int count = bo.listBox2.SelectedIndices.Count;

                if (count == 1)
                {
                    for (i = 0; i < cnt; i++)
                    {
                        if (bo.listBox2.SelectedItem.ToString() == box[i].name)
                        {
                            box[i].con.Enabled = true;
                            box[i].con.Visible = true;
                            getcont = box[i].con;
                            collthread((short)back.en, false);
                            break;
                        }
                    }
                }

                else
                {

                    string[] bb = new string[150];
                    pop = -1;
                    for (i = 0; i < count; i++)
                    {
                        bb[i] = bo.listBox2.SelectedItems[i].ToString();
                        for (int s = 0; s < cnt; s++)
                        {
                            if (bb[i] == box[s].name)
                            {
                                box[s].con.Enabled = true;
                                box[s].con.Visible = true;
                                stack[++pop] = box[s].con;
                                getcont = box[s].con;
                                break;
                            }
                        }
                    }
                    hukusuu = true;
                    collthread((short)back.en, false);
                    resetstack();
                }
            }
            catch { }
        }

        //リストボックスからコントロールを無効にする
        public void outputitems()
        {
            try
            {
                resetstack();
                int i;
                int count = bo.listBox1.SelectedIndices.Count;
                pop = -1;

                if (count == 1)
                {
                    for (i = 0; i < cnt; i++)
                    {
                        if (bo.listBox1.SelectedItem.ToString() == box[i].name)
                        {
                            box[i].con.Enabled = false;
                            box[i].con.Visible = false;
                            getcont = box[i].con;
                            collthread((short)back.en, true);
                            break;
                        }
                    }
                }

                else
                {
                    string[] bb = new string[150];
                    for (i = 0; i < count; i++)
                    {
                        bb[i] = bo.listBox1.SelectedItems[i].ToString();
                        for (int s = 0; s < cnt; s++)
                        {
                            if (bb[i] == box[s].name)
                            {
                                box[s].con.Enabled = false;
                                box[s].con.Visible = false;
                                stack[++pop] = box[s].con;
                                getcont = box[s].con;
                                break;
                            }
                        }
                    }
                    hukusuu = true;
                    collthread((short)back.en, true);
                    resetstack();
                }
            }
            catch { }
        }

        #endregion
     
        #region ファイル操作

        //編集したレイアウトを全て文字列に置き換えるメソッド
        public void getdata(ref string data)
        {
            int i, s;
            //フォームのサイズ
            data += this.Size.Width.ToString() + "\n";
            data += this.Size.Height.ToString() + "\n";
            //背景色ARGB順
            data += this.BackColor.A.ToString() + "\n";
            data += this.BackColor.R.ToString() + "\n";
            data += this.BackColor.G.ToString() + "\n";
            data += this.BackColor.B.ToString() + "\n";

            //引き伸ばすか
            data += Convert.ToInt32(imageflag).ToString() + "\n";
            //背景画像
            data += bo.backimagetextbox.Text + "\n"; 
            //入力サウンド
            if (sound == null) data += "" + "\n";
            else data += sound.SoundLocation.ToString() + "\n";
 
            //入力ラベル
            for (i = 0; i <= 79; i++)
            {
                Label n = (Label)box[i].con;
                //有効無効
                data += (n.Enabled == true) ? "1\n" : "0\n";
                //位置x,y
                data += n.Location.X.ToString() + "\n";
                data += n.Location.Y.ToString() + "\n";
                //サイズx,y
                data += n.Size.Width.ToString() + "\n";
                data += n.Size.Height.ToString() + "\n";
                //背景色ARGB順
                data += box[i].bc.A.ToString() + "\n";
                data += box[i].bc.R.ToString() + "\n";
                data += box[i].bc.G.ToString() + "\n";
                data += box[i].bc.B.ToString() + "\n";
                //選択色ARGB順
                data += box[i].cc.A.ToString() + "\n";
                data += box[i].cc.R.ToString() + "\n";
                data += box[i].cc.G.ToString() + "\n";
                data += box[i].cc.B.ToString() + "\n";
                //文字色ARGB順
                data += n.ForeColor.A.ToString() + "\n";
                data += n.ForeColor.R.ToString() + "\n";
                data += n.ForeColor.G.ToString() + "\n";
                data += n.ForeColor.B.ToString() + "\n";

                //フォント
                data += n.Font.Name + "\n";
                data += n.Font.SizeInPoints.ToString() + "\n";
                data += n.Font.Style.ToString() + "\n";
                //表示文字、入力文字カテゴリ順
                for (s = 0; s < 5; s++)
                {
                    data += box[i].txt[s] + "\n";
                    data += box[i].inp[s] + "\n";
                }

                //ボーダーライン
                if (n.BorderStyle == BorderStyle.None) data += "1\n";
                else if (n.BorderStyle == BorderStyle.Fixed3D) data += "2\n";
                else data += "3\n";
            }

            //入力ボタン
            for (i = 80; i <= 94; i++)
            {
                Control c = box[i].con;
                //有効無効
                data += (c.Enabled == true) ? "1\n" : "0\n";
                //配置
                data += c.Location.X.ToString() + "\n";
                data += c.Location.Y.ToString() + "\n";
                //サイズ
                data += c.Size.Width.ToString() + "\n";
                data += c.Size.Height.ToString() + "\n";
                //背景色ARGB順
                data += box[i].bc.A.ToString() + "\n";
                data += box[i].bc.R.ToString() + "\n";
                data += box[i].bc.G.ToString() + "\n";
                data += box[i].bc.B.ToString() + "\n";
                //選択色ARGB順
                data += box[i].cc.A.ToString() + "\n";
                data += box[i].cc.R.ToString() + "\n";
                data += box[i].cc.G.ToString() + "\n";
                data += box[i].cc.B.ToString() + "\n";
                //文字色ARGB順
                data += c.ForeColor.A.ToString() + "\n";
                data += c.ForeColor.R.ToString() + "\n";
                data += c.ForeColor.G.ToString() + "\n";
                data += c.ForeColor.B.ToString() + "\n";
                //フォント
                data += c.Font.Name + "\n";
                data += c.Font.SizeInPoints.ToString() + "\n";
                data += c.Font.Style.ToString() + "\n";
                //表示文字、入力文字カテゴリ順
                for (s = 0; s < 5; s++)
                {
                    data += box[i].txt[s] + "\n";
                    data += box[i].inp[s] + "\n";
                }
            }

            //カテゴリ用ボタン
            for (i = 95; i <= 99; i++)
            {
                Control c = box[i].con;
                //有効無効
                data += (c.Enabled == true) ? "1\n" : "0\n";
                //テキスト
                data += c.Text + "\n";
                //配置
                data += c.Location.X.ToString() + "\n";
                data += c.Location.Y.ToString() + "\n";
                //サイズ
                data += c.Size.Width.ToString() + "\n";
                data += c.Size.Height.ToString() + "\n";
                //背景色ARGB順
                data += box[i].bc.A.ToString() + "\n";
                data += box[i].bc.R.ToString() + "\n";
                data += box[i].bc.G.ToString() + "\n";
                data += box[i].bc.B.ToString() + "\n";
                //選択色ARGB順
                data += box[i].cc.A.ToString() + "\n";
                data += box[i].cc.R.ToString() + "\n";
                data += box[i].cc.G.ToString() + "\n";
                data += box[i].cc.B.ToString() + "\n";
                //文字色ARGB順
                data += c.ForeColor.A.ToString() + "\n";
                data += c.ForeColor.R.ToString() + "\n";
                data += c.ForeColor.G.ToString() + "\n";
                data += c.ForeColor.B.ToString() + "\n";
                //フォント
                data += c.Font.Name + "\n";
                data += c.Font.SizeInPoints.ToString() + "\n";
                data += c.Font.Style.ToString() + "\n";
            }

            //特殊ボタン
            for (i = 100; i <= 102; i++)
            {
                //特殊ボタン
                Control cc = box[i].con;
                //有効無効
                data += (cc.Enabled == true) ? "1\n" : "0\n";
                //テキスト
                data += cc.Text + "\n";
                //配置
                data += cc.Location.X.ToString() + "\n";
                data += cc.Location.Y.ToString() + "\n";
                //サイズ
                data += cc.Size.Width.ToString() + "\n";
                data += cc.Size.Height.ToString() + "\n";
                //背景色ARGB順
                data += box[i].bc.A.ToString() + "\n";
                data += box[i].bc.R.ToString() + "\n";
                data += box[i].bc.G.ToString() + "\n";
                data += box[i].bc.B.ToString() + "\n";
                //選択色ARGB順
                data += box[i].cc.A.ToString() + "\n";
                data += box[i].cc.R.ToString() + "\n";
                data += box[i].cc.G.ToString() + "\n";
                data += box[i].cc.B.ToString() + "\n";
                //文字色ARGB順
                data += cc.ForeColor.A.ToString() + "\n";
                data += cc.ForeColor.R.ToString() + "\n";
                data += cc.ForeColor.G.ToString() + "\n";
                data += cc.ForeColor.B.ToString() + "\n";
                //フォント
                data += cc.Font.Name + "\n";
                data += cc.Font.SizeInPoints.ToString() + "\n";
                data += cc.Font.Style.ToString() + "\n";
            }

            //プログレスバー
            data += (progressBar1.Enabled == true) ? "1\n" : "0\n";
            data += progressBar1.Location.X.ToString() + "\n";
            data += progressBar1.Location.Y.ToString() + "\n";
            data += progressBar1.Size.Width.ToString() + "\n";
            data += progressBar1.Size.Height.ToString() + "\n";

            //透過度
            data += bo.numericUpDown1.Value.ToString()+"\n";
            //連続入力チェックボックス
            data += (bo.checkBox5.Checked == true) ? "1\n" : "0\n";
            //入力待ち時間
            data += bo.comboBox1.SelectedIndex.ToString() + "\n";
            //クリックで入力する
            data += (bo.checkBox3.Checked == true) ? "1\n" : "0\n";
            //オンマウスで入力する
            data += (bo.checkBox2.Checked == true) ? "1\n" : "0\n";
        }

        //ファイルの保存   
        public bool Save()
        {
            string data = "";

            //保存処理
            richTextBox1.Visible = true;
            saveFileDialog1.Filter = "スクリーンキーボードエディットファイル(*.skb)|*.skb|" + "All files(*.*)|*.*";
            saveFileDialog1.FileName = "スクリーンキーボードデータ";
            //保存画面を出す
            if (saveFileDialog1.ShowDialog() != DialogResult.OK)
            {
                richTextBox1.Visible = false;
                return false;
            }

            getdata(ref data);

            //保存する
            richTextBox1.Text = data;
            richTextBox1.SaveFile(saveFileDialog1.FileName, RichTextBoxStreamType.TextTextOleObjs);
            richTextBox1.Text = "";
            richTextBox1.Visible = false;
            saveflag = true;
            return true;
        }

        //ファイルの変更を保存させる
        public bool shallwesave()
        {
            if (saveflag == false)
            {
                DialogResult mm;
                mm = MessageBox.Show("データが保存されていません。保存しますか？", "確認", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);
                if (mm==DialogResult.Yes)
                {
                    Save();
                    saveflag = true;
                }
         
                return (mm==DialogResult.Cancel)?false:true;
            }
            return true;
        }

        //ファイルの読み込み
        public void roodo()

        {
            openFileDialog1.FileName = "スクリーンキーボードデータ";
            openFileDialog1.Filter = "スクリーンキーボードエディットファイル(*.skb)|*.skb";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                MessageBox.Show(openFileDialog1.FileName);
                string s = openFileDialog1.FileName.Substring(openFileDialog1.FileName.LastIndexOf('.'));
                if (s != ".skb"&&s!="skb")
                {
                    MessageBox.Show("ファイル名が無効です", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                //ファイルを開く
                Stream str;
                StreamReader srm;
                str = openFileDialog1.OpenFile();
                srm = new StreamReader(str, Encoding.Default);
                string data = "";

                while (srm.Peek() >= 0)
                {
                    data += srm.ReadLine() + "\n";
                }
                StringReader sm = new StringReader(data);
                setdata(ref sm);
                sm.Close();
                str.Close();
                srm.Close();
            }
        }

        //デフォルトデータの読み込み
        public void defo()
        {
            StringReader srm;
            srm = new StringReader(Properties.Resources.デフォルトデータ);
            setdata(ref srm);
        }

        //読み込んだデータを元にコントロールを配置
        void setdata(ref StringReader srm)
        {
            //try
            //{
                int s;
                int i;
                int a, r, g, b;
                resetstack();
                his = new history[maxhis];
                bo.listBox1.Items.Clear();
                bo.listBox2.Items.Clear();
                bo.listBox3.Items.Clear();
                pops = 0;

                //フォームサイズ
                this.Size = new Size(Convert.ToInt32(srm.ReadLine()), Convert.ToInt32(srm.ReadLine()));
                a = Convert.ToInt32(srm.ReadLine());
                r = Convert.ToInt32(srm.ReadLine());
                g = Convert.ToInt32(srm.ReadLine());
                b = Convert.ToInt32(srm.ReadLine());
                this.BackColor = Color.FromArgb(a, r, g, b);

                //背景画像
                int imafla = Convert.ToInt32(srm.ReadLine());
                if ((bo.backimagetextbox.Text = srm.ReadLine()) != "")
                {
                    try
                    {
                        backimage = Image.FromFile(bo.backimagetextbox.Text);
                        if (imafla == 1)
                        {
                            imageflag = true;
                            this.BackgroundImage = new Bitmap(backimage, this.Size);
                        }
                        else
                        {
                            imageflag = false;
                            this.BackgroundImage = new Bitmap(backimage);
                        }
                    }
                    catch
                    {
                        MessageBox.Show(bo.backimagetextbox.Text + "\nが見つかりません", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        backimage = null;
                        this.BackgroundImage = null;
                    }
                }
                else
                {
                    if (imafla == 1) imageflag = true;
                    else imageflag = false;
                    this.backimage = null;
                    this.BackgroundImage = null;
                }

                //入力サウンド
                string lo = srm.ReadLine();
                if (lo == "")
                {
                    sound = null;
                    bo.soundtextbox.Text = "";
                }
                else
                {
                    sound = new SoundPlayer(lo);
                    bo.soundtextbox.Text = lo;
                }

                //ラベル
                for (i = 0; i <= 79; i++)
                {
                    Label c = ((Label)box[i].con);
                    //有効無効
                    int kk = Convert.ToInt32(srm.ReadLine());
                    if (kk == 1)
                    {
                        c.Enabled = true;
                        bo.listBox1.Items.Add(box[i].name);
                    }
                    else
                    {
                        c.Enabled = false;
                        bo.listBox2.Items.Add(box[i].name);
                    }
                    c.Visible = c.Enabled;
                    //表示位置
                    c.Location = new Point(Convert.ToInt32(srm.ReadLine()), Convert.ToInt32(srm.ReadLine()));
                    //サイズ
                    c.Size = new Size(Convert.ToInt32(srm.ReadLine()), Convert.ToInt32(srm.ReadLine()));
                    //背景色ARGB順
                    a = Convert.ToInt32(srm.ReadLine());
                    r = Convert.ToInt32(srm.ReadLine());
                    g = Convert.ToInt32(srm.ReadLine());
                    b = Convert.ToInt32(srm.ReadLine());
                    box[i].bc = Color.FromArgb(a, r, g, b);
                    c.BackColor = box[i].bc;
                    //選択色の変更
                    a = Convert.ToInt32(srm.ReadLine());
                    r = Convert.ToInt32(srm.ReadLine());
                    g = Convert.ToInt32(srm.ReadLine());
                    b = Convert.ToInt32(srm.ReadLine());
                    box[i].cc = Color.FromArgb(a, r, g, b);
                    //文字色の変更
                    a = Convert.ToInt32(srm.ReadLine());
                    r = Convert.ToInt32(srm.ReadLine());
                    g = Convert.ToInt32(srm.ReadLine());
                    b = Convert.ToInt32(srm.ReadLine());
                    c.ForeColor = Color.FromArgb(a, r, g, b);

                    //フォント
                    string name = srm.ReadLine();
                    float size = (float)Convert.ToDouble(srm.ReadLine());
                    string st = srm.ReadLine();
                    FontStyle n;
                    if (st[0] == 'B') n = FontStyle.Bold;
                    else if (st[0] == 'I') n = FontStyle.Italic;
                    else if (st[0] == 'R') n = FontStyle.Regular;
                    else if (st[0] == 'S') n = FontStyle.Strikeout;
                    else n = FontStyle.Underline;
                    c.Font = new Font(name, size, n);

                    //表示文字、入力文字
                    for (s = 0; s < 5; s++)
                    {
                        box[i].txt[s] = srm.ReadLine();
                        box[i].inp[s] = srm.ReadLine();
                    }
                    c.Text = box[i].txt[0];
                    string m = srm.ReadLine();
                    if (m == "1")
                    {
                        ((Label)box[i].con).BorderStyle = BorderStyle.None;
                    }
                    else if (m == "2")
                    {
                        ((Label)box[i].con).BorderStyle = BorderStyle.Fixed3D;
                    }
                    else
                    {
                        ((Label)box[i].con).BorderStyle = BorderStyle.FixedSingle;
                    }
                }

                //入力用ボタン
                for (i = 80; i <= 94; i++)
                {
                    Button c = ((Button)box[i].con);
                    //有効無効
                    int kk = Convert.ToInt32(srm.ReadLine());
                    if (kk == 1)
                    {
                        c.Enabled = true;
                        bo.listBox1.Items.Add(box[i].name);
                    }
                    else
                    {
                        c.Enabled = false;
                        bo.listBox2.Items.Add(box[i].name);
                    }
                    c.Visible = c.Enabled;
                    //表示位置
                    c.Location = new Point(Convert.ToInt32(srm.ReadLine()), Convert.ToInt32(srm.ReadLine()));
                    //サイズ
                    c.Size = new Size(Convert.ToInt32(srm.ReadLine()), Convert.ToInt32(srm.ReadLine()));
                    //背景色ARGB順
                    a = Convert.ToInt32(srm.ReadLine());
                    r = Convert.ToInt32(srm.ReadLine());
                    g = Convert.ToInt32(srm.ReadLine());
                    b = Convert.ToInt32(srm.ReadLine());
                    box[i].bc = Color.FromArgb(a, r, g, b);
                    c.BackColor = box[i].bc;
                    //選択色の変更
                    a = Convert.ToInt32(srm.ReadLine());
                    r = Convert.ToInt32(srm.ReadLine());
                    g = Convert.ToInt32(srm.ReadLine());
                    b = Convert.ToInt32(srm.ReadLine());
                    box[i].cc = Color.FromArgb(a, r, g, b);
                    //文字色の変更
                    a = Convert.ToInt32(srm.ReadLine());
                    r = Convert.ToInt32(srm.ReadLine());
                    g = Convert.ToInt32(srm.ReadLine());
                    b = Convert.ToInt32(srm.ReadLine());
                    c.ForeColor = Color.FromArgb(a, r, g, b);

                    //フォント
                    string name = srm.ReadLine();
                    float size = (float)Convert.ToInt32(srm.ReadLine());
                    string st = srm.ReadLine();
                    FontStyle n;
                    if (st[0] == 'B') n = FontStyle.Bold;
                    else if (st[0] == 'I') n = FontStyle.Italic;
                    else if (st[0] == 'R') n = FontStyle.Regular;
                    else if (st[0] == 'S') n = FontStyle.Strikeout;
                    else n = FontStyle.Underline;
                    c.Font = new Font(name, size, n);

                    //表示文字、入力文字
                    for (s = 0; s < 5; s++)
                    {
                        box[i].txt[s] = srm.ReadLine();
                        box[i].inp[s] = srm.ReadLine();
                    }
                    c.Text = box[i].txt[0];
                }

                //カテゴリ用ボタン
                for (i = 95; i <= 99; i++)
                {
                    Button c = ((Button)box[i].con);
                    //有効無効
                    int kk = Convert.ToInt32(srm.ReadLine());
                    if (kk == 1)
                    {
                        c.Enabled = true;
                        bo.listBox1.Items.Add(box[i].name);
                    }
                    else
                    {
                        c.Enabled = false;
                        bo.listBox2.Items.Add(box[i].name);
                    }
                    c.Visible = c.Enabled;
                    //テキスト
                    c.Text = srm.ReadLine();
                    //表示位置
                    c.Location = new Point(Convert.ToInt32(srm.ReadLine()), Convert.ToInt32(srm.ReadLine()));
                    //サイズ
                    c.Size = new Size(Convert.ToInt32(srm.ReadLine()), Convert.ToInt32(srm.ReadLine()));
                    //背景色ARGB順
                    a = Convert.ToInt32(srm.ReadLine());
                    r = Convert.ToInt32(srm.ReadLine());
                    g = Convert.ToInt32(srm.ReadLine());
                    b = Convert.ToInt32(srm.ReadLine());
                    box[i].bc = Color.FromArgb(a, r, g, b);
                    c.BackColor = box[i].bc;
                    //選択色の変更
                    a = Convert.ToInt32(srm.ReadLine());
                    r = Convert.ToInt32(srm.ReadLine());
                    g = Convert.ToInt32(srm.ReadLine());
                    b = Convert.ToInt32(srm.ReadLine());
                    box[i].cc = Color.FromArgb(a, r, g, b);
                    //文字色の変更
                    a = Convert.ToInt32(srm.ReadLine());
                    r = Convert.ToInt32(srm.ReadLine());
                    g = Convert.ToInt32(srm.ReadLine());
                    b = Convert.ToInt32(srm.ReadLine());
                    c.ForeColor = Color.FromArgb(a, r, g, b);

                    //フォント
                    string name = srm.ReadLine();
                    float size = (float)Convert.ToDouble(srm.ReadLine());
                    string st = srm.ReadLine();
                    FontStyle n;
                    if (st[0] == 'B') n = FontStyle.Bold;
                    else if (st[0] == 'I') n = FontStyle.Italic;
                    else if (st[0] == 'R') n = FontStyle.Regular;
                    else if (st[0] == 'S') n = FontStyle.Strikeout;
                    else n = FontStyle.Underline;
                    c.Font = new Font(name, size, n);
                }

                for (i = 100; i <= 102; i++)
                {
                    //特殊ボタン
                    Button cc = ((Button)box[i].con);
                    //有効無効
                    int kk = Convert.ToInt32(srm.ReadLine());
                    if (kk == 1)
                    {
                        cc.Enabled = true;
                        bo.listBox1.Items.Add(box[i].name);
                    }
                    else
                    {
                        cc.Enabled = false;
                        bo.listBox2.Items.Add(box[i].name);
                    }
                    cc.Visible = cc.Enabled;
                    //テキスト
                    cc.Text = srm.ReadLine();
                    //表示位置
                    cc.Location = new Point(Convert.ToInt32(srm.ReadLine()), Convert.ToInt32(srm.ReadLine()));
                    //サイズ
                    cc.Size = new Size(Convert.ToInt32(srm.ReadLine()), Convert.ToInt32(srm.ReadLine()));
                    //背景色ARGB順
                    a = Convert.ToInt32(srm.ReadLine());
                    r = Convert.ToInt32(srm.ReadLine());
                    g = Convert.ToInt32(srm.ReadLine());
                    b = Convert.ToInt32(srm.ReadLine());
                    box[i].bc = Color.FromArgb(a, r, g, b);
                    cc.BackColor = box[i].bc;
                    //選択色の変更
                    a = Convert.ToInt32(srm.ReadLine());
                    r = Convert.ToInt32(srm.ReadLine());
                    g = Convert.ToInt32(srm.ReadLine());
                    b = Convert.ToInt32(srm.ReadLine());
                    box[i].cc = Color.FromArgb(a, r, g, b);
                    //文字色の変更
                    a = Convert.ToInt32(srm.ReadLine());
                    r = Convert.ToInt32(srm.ReadLine());
                    g = Convert.ToInt32(srm.ReadLine());
                    b = Convert.ToInt32(srm.ReadLine());
                    cc.ForeColor = Color.FromArgb(a, r, g, b);

                    //フォント
                    string name = srm.ReadLine();
                    float size = (float)Convert.ToDouble(srm.ReadLine());
                    string st = srm.ReadLine();
                    FontStyle n;
                    if (st[0] == 'B') n = FontStyle.Bold;
                    else if (st[0] == 'I') n = FontStyle.Italic;
                    else if (st[0] == 'R') n = FontStyle.Regular;
                    else if (st[0] == 'S') n = FontStyle.Strikeout;
                    else n = FontStyle.Underline;
                    cc.Font = new Font(name, size, n);
                }
                //プログレスバー
                int si = Convert.ToInt32(srm.ReadLine());
                if (si == 1)
                {
                    progressBar1.Enabled = true;
                    bo.listBox1.Items.Add("プログレスバー");
                }
                else
                {
                    progressBar1.Enabled = false;
                    bo.listBox2.Items.Add("プログレスバー");
                }
                progressBar1.Visible = progressBar1.Enabled;
                progressBar1.Location = new Point(Convert.ToInt32(srm.ReadLine()), Convert.ToInt32(srm.ReadLine()));
                progressBar1.Size = new Size(Convert.ToInt32(srm.ReadLine()), Convert.ToInt32(srm.ReadLine()));
                progressBar1.Value = 0;
                box[103].bc = Color.Red;
                box[103].cc = Color.Green;
                box[104].bc = this.BackColor;
                box[104].cc = this.BackColor;
                box[104].con.Tag = 104;

                //透過度
                bo.numericUpDown1.Value = (decimal)(Convert.ToInt32(srm.ReadLine()));
                bo.numnum = (int)bo.numericUpDown1.Value;
                //連続入力チェックボックス
                bo.checkBox5.Checked = (Convert.ToInt32(srm.ReadLine()) == 1) ? true : false;
                //入力待ち時間
                bo.comboBox1.SelectedIndex = Convert.ToInt32(srm.ReadLine());
                bo.numon = bo.comboBox1.SelectedIndex;
                //クリックで入力
                bo.checkBox3.Checked = (Convert.ToInt32(srm.ReadLine()) == 1) ? true : false; ;
                //オンマウスで入力
                bo.checkBox2.Checked = (Convert.ToInt32(srm.ReadLine()) == 1) ? true : false;

                //セーブフラグをたてる
                saveflag = true;
            }
            //catch
            //{
            //    MessageBox.Show("ファイルが有効ではありません", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    defo();
            //}
       // }

        #endregion

        #region 背景画像関連

        //画像の参照
        public void getgrah()
        {
            resetstack();
            //オープンファイルダイアログのフィルターを変える
            openFileDialog1.FileName = "";
            openFileDialog1.Filter = "画像ファイル|*.JPEG;*.jpeg;*.jpg;*.PNG;*.GIF;*.bmp|すべてのファイル(*.*)|*.*";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                setgrah(openFileDialog1.FileName);
            }
        }

        //画像の設定
        void setgrah(string fn)
        {
            try
            {
                //ダミー変数に画像をつけて使えるか試す
                Image nn = Image.FromFile(fn);
                //ダミーはもういらない
                nn.Dispose();

                //変更がなければリターン
                if (bo.backimagetextbox.Text == fn) return;

                //画像をつける
                backimage = null;
                collthread((short)back.back, fn);
                bo.backimagetextbox.Text = fn;
                backimage = Image.FromFile(fn);
                if (imageflag)
                {
                    this.BackgroundImage = new Bitmap(backimage, this.Size);
                }
                else
                    this.BackgroundImage = new Bitmap(backimage);
            }
            catch
            {
                MessageBox.Show("ファイルが無効です", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //画像のクリア
        public void resetfrah()
        {
            resetstack();
            if (bo.backimagetextbox.Text == "") return;
            //backimage.Dispose();
            collthread((short)back.back, bo.backimagetextbox.Text);
            this.BackgroundImage = null;
            backimage = null;
            bo.backimagetextbox.Text = "";
        }

        //画像の表示切替
        public void grahmodechange()
        {
            if (backimage != null)
            {
                imageflag = !imageflag;
                if (imageflag == false)
                {
                    this.BackgroundImage = new Bitmap(backimage);
                }
                else
                {
                    this.BackgroundImage = new Bitmap(backimage, this.Size);
                }
            }
        }

        #endregion

        #region サウンド設定
        //サウンドの参照
        public void getsound()
        {
            resetstack();
            //オープンファイルダイアログのフィルターを変える
            openFileDialog1.FileName = "";
            openFileDialog1.Filter = "サウンドファイル(*.wav;*.mp3;)|*.wav;*.mp3;";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string str = openFileDialog1.FileName.Substring(openFileDialog1.FileName.LastIndexOf('.'));
                if (str==".mp3"||str==".wav"||str==".WAV")
                    setsound(openFileDialog1.FileName);
                else
                    MessageBox.Show("ファイル名が無効です", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //サウンドの設定
        void setsound(string fn)
        {
            if (bo.soundtextbox.Text == fn) return;
            collthread((short)back.sound, fn);
            bo.soundtextbox.Text = fn;
            sound = new SoundPlayer(fn);
        }
        //サウンドのリセット
        public void resetsound()
        {
            resetstack();
            if (bo.soundtextbox.Text == "") return;
            collthread((short)back.sound,"");
            sound = null;
            bo.soundtextbox.Text = "";
        }
        //サウンドの再生
        public void playsound()
        {
            if (sound != null)
            {
                try
                {
                    sound.Play();
                }
                catch
                {
                    MessageBox.Show(bo.soundtextbox.Text + "\nが見つかりません", "エラー",MessageBoxButtons.OK,MessageBoxIcon.Error);
                    sound = null;
                    bo.soundtextbox.Text = "";
                }
            }
        }
        #endregion

        #region コントロールイベント

        //その他ボタンマウスダウン
        void sonotabuttonDown(object sender, MouseEventArgs e)
        {
            Button n = (Button)sender;
            clickx = e.X;
            clicky = e.Y-hose;

            //揃えコマンド
            if (soroe)
            {
                soroemasu(n);
                return;
            }

            //左クリック
            if (e.Button == MouseButtons.Left)
            {
                //コントロールキー押しながら
                if (Control.ModifierKeys == Keys.Control&&getcont!=this)
                {
                    contsentaku(n);
                    return;
                }
                //ボタンのどこをクリックしたか
                if (n.BackColor == box[(int)n.Tag].cc)
                {
                    if (hukusuu) changestack(n);
                    returnclick(n, e);
                }
                //ドラッグ準備
                cursorx = Cursor.Position.X;
                cursory = Cursor.Position.Y;
                n.BringToFront();


                setthread();
            }

            //右クリック
            else
            {
                空白文字に設定するToolStripMenuItem.Visible = false;
                contextMenuStrip1.Show(Cursor.Position);
            }

            if (hukusuu == false || n.BackColor != box[Convert.ToInt32(n.Tag)].cc)
            {
                bo.setpropaty((Control)sender);
                getcont = n;
            }
        }
       
        //入力ボタン、ラベルマウスダウン
        void inputbuttonDown(object sender, MouseEventArgs e)
        {
            Control n = (Control)sender;
            clickx = e.X;
            clicky = e.Y-hose;


            //揃えコマンド
            if (soroe)
            {
                soroemasu(n);
                return;
            }

            //左クリック
            if (e.Button == MouseButtons.Left)
            {   //コントロールキー押しながら
                if (Control.ModifierKeys == Keys.Control&&getcont!=this)
                {
                    contsentaku(n);
                    return;
                }
                if (n.BackColor == box[(int)n.Tag].cc)
                {
                    if (hukusuu) changestack(n);
                    returnclick(n, e);
                }
                //ドラッグ準備
                //ボタンのどこをクリックしたか
                n.BringToFront();
                setthread();
            }

            //右クリック
            else
            {
                if(!hukusuu)
                空白文字に設定するToolStripMenuItem.Visible = true;
                contextMenuStrip1.Show(Cursor.Position);
            }

            if (hukusuu == false || n.BackColor != box[Convert.ToInt32(n.Tag)].cc)
            {
                bo.setpropaty(n);
                getcont = n;
            }
        }
      
        //カテゴリボタンマウスダウン
        void kategoributtondown(object sender, MouseEventArgs e)
        {
            Button n = (Button)sender;
            clickx = e.X;
            clicky = e.Y-hose;

            //揃えコマンド
            if (soroe)
            {
                soroemasu(n);
                return;
            }

            //左クリック
            if (e.Button == MouseButtons.Left)
            {
                //コントロールキー押しながら
                if (Control.ModifierKeys == Keys.Control&&getcont!=this)
                {
                    contsentaku(n);
                    return;
                }
                if (n.BackColor == box[(int)n.Tag].cc)
                {
                    if (hukusuu) changestack(n);
                    returnclick(n, e);
                }
                cursorx = Cursor.Position.X;
                cursory = Cursor.Position.Y;
                n.BringToFront();
                setthread();
            }
            //右クリック
            else
            {
                空白文字に設定するToolStripMenuItem.Visible = false;
                contextMenuStrip1.Show(Cursor.Position);
            }
            //ドラッグ準備
            if (hukusuu == false || n.BackColor != box[Convert.ToInt32(n.Tag)].cc)
            {
                if (n.BackColor == box[(int)n.Tag].cc) returnclick(n, e);
                bo.allfalse();
                bo.labelfontchange.Visible = true;
                bo.labelcolorchange.Visible = true;
                bo.textbox[0].Enabled = true;
                bo.textbox[1].Enabled = true;
                bo.textbox[4].Enabled = true;
                bo.textbox[5].Enabled = true;
                bo.checkBox1.Enabled = true;
                bo.setpropaty((Control)sender);
                getcont = n;
            }
        }
       
        //プログレスバーマウスダウン
        void progressberMouseDown(object sender, MouseEventArgs e)
        {
            clickx = e.X;
            clicky = e.Y - hose;
            button1.Focus();

            //揃えコマンド
            if (soroe)
            {
                soroemasu(progressBar1);
                return;
            }

            //左クリック
            if (e.Button == MouseButtons.Left)
            {
                //コントロールキー押しながら
                if (Control.ModifierKeys == Keys.Control&&getcont!=this)
                {
                    contsentaku(progressBar1);
                    return;
                }
                if (progressBar1.Value != 0)
                {
                    if (hukusuu) changestack(progressBar1);
                    returnclick(progressBar1, e);
                }
                cursorx = Cursor.Position.X;
                cursory = Cursor.Position.Y;
                progressBar1.BringToFront();
                setthread();
                getcont = progressBar1;
            }

            //右クリック
            else
            {
                空白文字に設定するToolStripMenuItem.Visible = false;
                contextMenuStrip1.Show(Cursor.Position);
            }
            //ドラッグ準備
            if (hukusuu == false || progressBar1.Value == 0)
            {

                bo.setpropaty((Control)sender);
                getcont = progressBar1;
            }
        }

        //マウスドラッグするスレッド
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            this.BackgroundImage = null;
            while (onflag != 0)
            {
                //シングルドラッグ
                if (hukusuu == false)
                {
                    sizechange(getcont);
                }
                //複数ドラッグ
                else
                {
                    doragmaster();
                }
            }
        }

        //スレッドを呼び出す
        void setthread()
        {
            if (!backgroundWorker1.IsBusy)
            {
                backgroundWorker1.RunWorkerAsync();
                CheckForIllegalCrossThreadCalls = false;
            }
        }
      
        //フォームマウスダウン
        void mainedit_MouseDown(object sender, MouseEventArgs e)
        {
 
            dorafla = true;
            cursorx = e.X;
            cursory = e.Y;
            //コントロールキー押しながらなら追加選択
            if (Control.ModifierKeys == Keys.Control)
            {
                tuika = true;
            }
            //ちがければフォーム選択として扱う
            else
            {
                contextMenuStrip1.Hide();

                tuika = false;
                resetstack();

                bo.setopropatyfirst(this);

                if (soroe) zahyo_button_naosu();
            }
        } 
      
        //マウスムーブ
        void inputbuttonMove(object sender, MouseEventArgs e)
        {
            Control n = ((Control)sender);
            mousex = e.X + n.Left;
            mousey = e.Y + n.Top - hose;
            bo.sutetasutext.Text = "マウス座標　　X=" + (mousex) + " Y=" + (mousey);

            if (Control.ModifierKeys == Keys.Control) return;

            //非ドラッグ時カーソルの変更
            if (n.BackColor == box[(int)n.Tag].cc)
            {
                changecursor(n, e);
            }
            else
            {
                Cursor = Cursors.Default;
            }
        }        
    
       
        //フォームマウスムー部
        void mainedit_MouseMove(object sender, MouseEventArgs e)
        {
            mousex = e.X;
            mousey = e.Y-hose;
            bo.sutetasutext.Text = "マウス座標　　X=" + e.X + " Y=" + e.Y;
            formdorag(e);
        }   
       
        //マウスアップ
        void mouseup(object sender, MouseEventArgs e)
        {
            if (onflag > 0)
            {
                //オンフラグをリセット
                if (onflag == 1)
                {
                    onflag = 0;
                    if (getcont.Location == (Point)hispoint[0]) return;
                    collthread((short)back.lo, hispoint[0]);
                }
                else
                {
                    onflag = 0;
                    if (getcont.Size == (Size)hissize[0]) return;
                    collthread((short)back.si, hissize[0]);
                }
                
            }
            if (backimage != null)
            {
                if (imageflag == true)
                {
                    this.BackgroundImage = new Bitmap(backimage, this.Size);
                }
                else
                this.BackgroundImage = new Bitmap(backimage);
            }
        }
       
        //フォームマウスアップ
        private void mainedit_MouseUp(object sender, MouseEventArgs e)
        {
            if (dorafla == false) return;
            dorafla = false;
            getcontril();
            xr = 0;
            yd = 0;
            Invalidate();
        }

        //フォームのサイズチェンジ
        private void mainedit_SizeChanged(object sender, EventArgs e)
        {
            if (bo.chicelabel.Text[0] != 'サ')
            {
                bo.allfalse();
                bo.setopropatyfirst(this);
            }
            if (bo.radioButton1.Enabled == true)
            {
                bo.radioButton1.Enabled = false;
                bo.radioButton2.Enabled = false;
                bo.radioButton3.Enabled = false;
            }
        }

        //フォームのマウスエンター
        private void mainedit_MouseEnter(object sender, EventArgs e)
        {
            Cursor = Cursors.Default;
            button1.Focus();
        }

        #endregion

        #region 右クリックメニュー

        private void 無効にするToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //コントロールを無効にする
            
            //アナログにチェックボックスを押したことにする
            bo.checkBox1.Checked = !bo.checkBox1.Checked;
            bo.checkBox1_CheckedChanged(null, null);
            ////複数一括処理
            //if (hukusuu)
            //{
            //    int p = pop;
            //    do
            //    {
            //        Control ch = stack[p];
            //        p--;
            //        ch.Enabled = false;
            //        ch.Visible = false;
            //        bo.listBox1.Items.Remove(box[(int)ch.Tag].name);
            //        bo.listBox2.Items.Add(box[(int)ch.Tag].name);
            //    } while (p >= 0);
            //}

            //else
            //{
            //    getcont.Enabled = false;
            //    getcont.Visible = false;
            //    bo.listBox1.Items.Remove(box[(int)getcont.Tag].name);
            //    bo.listBox2.Items.Remove(box[(int)getcont.Tag].name);
            //}
            
            //collthread((short)back.en, getcont.Enabled);
            resetstack();
        }
        private void 空白文字に設定するToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //履歴セット
            int n = (int)back.h1 + gori;
            collthread((short)n,"");
            n = (int)back.i1 + gori;
            collthread((short)n,"");

            //複数処理
            if (hukusuu)
            {
                int p = pop;
                do
                {
                    getcont = stack[p];
                    p--;
                    int tag = Convert.ToInt32(getcont.Tag);
                    box[tag].txt[gori] = "";
                    box[tag].inp[gori] = "";
                    getcont.Text = "";
                    bo.textbox[gori + 6].Text = "";
                    bo.textbox[gori + 11].Text = "";

                } while (p >= 0);
            }
            //シングル処理
            else
            {
                int tag = Convert.ToInt32(getcont.Tag);
                box[tag].txt[gori] = "";
                box[tag].inp[gori] = "";
                getcont.Text = "";
                bo.textbox[gori + 6].Text = "";
                bo.textbox[gori + 11].Text = "";
            }
        }
        private void 右クリック均等化(object sender, EventArgs e)
        {
            //押されたメニューに応じてボタンがクリックされたものと認識する
            int n = Convert.ToInt32(((ToolStripMenuItem)sender).Tag);
            switch (n)
            {
                case 1:
                    bo.Kintoukabutton(bo.xsetbutton, null);
                    break;
                case 2:
                    bo.Kintoukabutton(bo.ysetbutton, null);
                    break;
                case 3:
                    bo.Kintoukabutton(bo.button1, null);
                    break;
                case 4:
                    bo.Kintoukabutton(bo.button2, null);
                    break;
                case 5:
                    bo.Kintoukabutton(bo.bckintou, null);
                    break;
                case 6:
                    bo.Kintoukabutton(bo.cckintou, null);
                    break;
                case 7:
                    bo.Kintoukabutton(bo.fontkintou, null);
                    break;
                default:
                    bo.Kintoukabutton(bo.mojiirokintou, null);
                    break;

            }
        }

        #endregion 

        #region ショートカットキー

        //上下にキーボードで移動
        void go_UpDown(Keys keyData, int n)
        {
            //複数
            if (hukusuu)
            {
                int p = pop;
                do
                {
                    getcont = stack[p];
                    getcont.Top += n;
                    p--;
                } while (p >= 0);
            }
            //シングル
            else
            {
                getcont.Top += n;
                bo.textbox[1].Text = Convert.ToString((getcont.Top - hose));
            }
        }
       
        //左右にキーボードで移動
        void go_LeftRight(Keys keyData, int n)
        {
            //複数処理
            if (hukusuu)
            {
                int p = pop;
                do
                {
                    getcont = stack[p];
                    getcont.Left += n;
                    p--;
                } while (p >= 0);
            }
            //シングル
            else
            {
                getcont.Left += n;
                bo.textbox[0].Text = Convert.ToString(getcont.Left);
            }
        }
        
        //上下にキーボードでサイズチェンジ
        void change_UpDown(Keys keyData, int n)
        {
            //複数
            if (hukusuu)
            {
                int p = pop;
                do
                {
                    getcont = stack[p];
                    getcont.Height += n;
                    p--;
                } while (p >= 0);
            }
            //シングル
            else
            {
                getcont.Height += n;
                bo.textbox[4].Text = Convert.ToString((getcont.Height));
            }
        }
        
        //左右にキーボードでサイズチェンジ
        void change_LeftRight(Keys keyData, int n)
        {
            //複数
            if (hukusuu)
            {
                int p = pop;
                do
                {
                    getcont = stack[p];
                    getcont.Width += n;
                    p--;
                } while (p >= 0);
            }
            //シングル処理
            else
            {
                getcont.Width += n;
                bo.textbox[3].Text = Convert.ToString(getcont.Width);
            }
        }
        
        //デリートする
        void deleate()
        {
            if (getcont != this)
            {
                bo.checkBox1.Checked = !bo.checkBox1.Checked;
                bo.checkBox1_CheckedChanged(null, null);
            }
        }
        
        //TABキーで次のコントロールへ
        void go_next_Control()
        {
            int m = Convert.ToInt32(getcont.Tag);
            while (true)
            {
                m = (m == cnt) ? 0 : m + 1;
                if (box[m].con.Enabled) break;
            }
            getcont = box[m].con;
            bo.setpropaty(getcont);
        }

        //入力されたキーによって処理
        protected override bool ProcessDialogKey(Keys keyData)
        {
            button1.Focus();
            //ＡＬＴキー押しながら
            if (Control.ModifierKeys == Keys.Alt)
            {
                if ((keyData & Keys.KeyCode) == Keys.Up) change_UpDown(keyData, -1);
                else if ((keyData & Keys.KeyCode) == Keys.Down) change_UpDown(keyData, 1);
                else if ((keyData & Keys.KeyCode) == Keys.Left) change_LeftRight(keyData, -1);
                else if ((keyData & Keys.KeyCode) == Keys.Right) change_LeftRight(keyData, 1);
            }
            //Ctrlキー押しながら
            else if (Control.ModifierKeys == Keys.Control)
            {
                if ((keyData & Keys.KeyCode) == Keys.A) allsetstack();
                else if ((keyData & Keys.KeyCode) == Keys.Z) undo();
                else if ((keyData & Keys.KeyCode) == Keys.Y) redo();
                else if ((keyData & Keys.KeyCode) == Keys.D) deleate();
            }
            //オプションキーなし
            else
            {
                //矢印キー
                if (keyData == Keys.Up) go_UpDown(keyData, -1);
                else if (keyData == Keys.Down) go_UpDown(keyData, 1);
                else if (keyData == Keys.Left) go_LeftRight(keyData, -1);
                else if (keyData == Keys.Right) go_LeftRight(keyData, 1);
                //TABキー
                else if (keyData == Keys.Tab) go_next_Control();
            }
            return base.ProcessDialogKey(keyData);
        }

        #endregion

        #region 元に戻す関連

        //元に戻す
        public void undo()
        {
            if (pops == 0) return;
            int i;
            pops--;

            //最後ならラストヒストリーを作る
            if (pops+1 == bo.listBox3.Items.Count)
            {
                lasthistory = new history();
                lasthistory.con = new int[his[pops].con.Length + 1];
                lasthistory.databack = new ArrayList();
                lasthistory.mode = his[pops].mode;
                lasthistory.con = his[pops].con;
                setlast((back)his[pops].mode);
                resetstack();
            }
            //背景色
            if (his[pops].mode == (short)back.bc)
            {
                for (i = 0; i < his[pops].con.Length; i++)
                {
                    Control n = box[his[pops].con[i]].con;
                    Color c = (Color)his[pops].databack[i];
                    n.BackColor = c;
                    box[(int)n.Tag].bc = c;
                    
                }
            }
            //選択色
            else if (his[pops].mode == (short)back.cc)
            {
                for (i = 0; i < his[pops].con.Length; i++)
                {
                    Control n = box[his[pops].con[i]].con;
                    Color c = (Color)his[pops].databack[i];
                    box[(int)n.Tag].cc = c;
                }
            }
            //有効無効切り替え
            else if (his[pops].mode == (short)back.en)
            {
                for (i = 0; i < his[pops].con.Length; i++)
                {
                    Control n = box[his[pops].con[i]].con;
                    n.Enabled = !n.Enabled;
                    n.Visible = !n.Visible;
                    //リストボックスにセット
                    if (his[pops].mode == (short)back.en)
                    {
                        if (n.Enabled == true)
                        {
                            bo.listBox1.Items.Add(box[(int)n.Tag].name);
                            bo.listBox2.Items.Remove(box[(int)n.Tag].name);
                        }
                        else
                        {
                            bo.listBox2.Items.Add(box[(int)n.Tag].name);
                            bo.listBox1.Items.Remove(box[(int)n.Tag].name);
                        }
                    }
                }
            }
            //フォント
            else if (his[pops].mode == (short)back.font)
            {
                for (i = 0; i < his[pops].con.Length; i++)
                {
                    Control n = box[his[pops].con[i]].con;
                    Font c = (Font)his[pops].databack[i];
                    n.Font = c;
                }
            }
            //表示文字
            else if (his[pops].mode >= (short)back.h1 && his[pops].mode <= (short)back.h5)
            {
                int m = (int)his[pops].mode - (int)back.h1;
                for (i = 0; i < his[pops].con.Length; i++)
                {
                    string c = (string)his[pops].databack[i];
                    box[his[pops].con[i]].txt[m] = c;
                }
                changekategori(box[95 + m].con, null);
                
            }
            //入力文字
            else if (his[pops].mode >= (short)back.i1 && his[pops].mode <= (short)back.i5)
            {
                int m = (int)his[pops].mode - (int)back.i1;
                for (i = 0; i < his[pops].con.Length; i++)
                {
                    string c = (string)his[pops].databack[i];
                    box[his[pops].con[i]].inp[m] = c;
                }
                changekategori(box[95 + m].con, null);
            }
            //Location
            else if (his[pops].mode == (short)back.lo)
            {
                for (i = 0; i < his[pops].con.Length; i++)
                {
                    Control n = box[his[pops].con[i]].con;
                    Point c = (Point)his[pops].databack[i];
                    n.Location = c;
                }
            }
            //サイズ
            else if (his[pops].mode == (short)back.si)
            {
                int s = 0;
                for (i = 0; i < his[pops].con.Length; i++)
                {
                    Control n = box[his[pops].con[i]].con;
                    Size c = (Size)his[pops].databack[s];
                    s++;
                    Point p = (Point)his[pops].databack[s];
                    s++;
                    n.Size = c;
                    n.Location = p;
                }
            }
            //テキスト
            else if (his[pops].mode == (short)back.text)
            {
                for (i = 0; i < his[pops].con.Length; i++)
                {
                    Control n = box[his[pops].con[i]].con;
                    string c = (string)his[pops].databack[i];
                    n.Text = c;
                }
            }
            //文字色
            else if (his[pops].mode == (short)back.moji)
            {
                for (i = 0; i < his[pops].con.Length; i++)
                {
                    Control n = box[his[pops].con[i]].con;
                    Color c = (Color)his[pops].databack[i];
                    n.ForeColor = c;
                }
            }

            //チェックボックス
            else if (his[pops].mode == (short)back.che)
            {
                CheckBox n = (CheckBox)his[pops].databack[0];
                n.Checked = !n.Checked;
            }

            //透過度
            else if (his[pops].mode == (short)back.num)
            {
                bo.numericUpDown1.Value = Convert.ToDecimal(his[pops].databack[0]);
                bo.numnum = (int)bo.numericUpDown1.Value;
            }

            //オンマウス待機時間
            else if (his[pops].mode == (short)back.onnum)
            {
                bo.comboBox1.SelectedIndex = (int)his[pops].databack[0];
                bo.numon = bo.comboBox1.SelectedIndex;
            }


            //ボーダーライン
            else if (his[pops].mode == (short)back.bord)
            {
                for (i = 0; i < his[pops].con.Length; i++)
                {
                    Label la = (Label)box[his[pops].con[i]].con;
                    BorderStyle bor = (BorderStyle)his[pops].databack[i];
                    la.BorderStyle = bor;
                }
            }

            //背景画像
            else if (his[pops].mode == (short)back.back)
            {
                resetstack();
                try
                {
                    bo.backimagetextbox.Text = his[pops].databack[0].ToString();
                    backimage = Image.FromFile(bo.backimagetextbox.Text);
                    if (imageflag == true)
                    {
                        this.BackgroundImage = new Bitmap(backimage, this.Size);
                    }
                    else
                        this.BackgroundImage = new Bitmap(backimage);
                }
                catch
                {
                    if (his[pops].databack[0].ToString() != "")
                        MessageBox.Show(his[pops].databack[0].ToString() + "\nが見つかりません", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    backimage = null;
                    this.BackgroundImage = null;
                    bo.backimagetextbox.Text = "";
                }
            }
            //サウンド
            else if (his[pops].mode == (short)back.sound)
            {
                resetstack();
                string fn = his[pops].databack[0].ToString();
                if (fn != "")
                {
                    bo.soundtextbox.Text = fn;
                    sound = new SoundPlayer(fn);
                }
                else
                {
                    if (his[pops].databack[0].ToString() != "")
                        MessageBox.Show(his[pops].databack[0].ToString() + "\nが見つかりません", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    bo.soundtextbox.Text = "";
                    sound = null;
                }
            }

            //プロパティを最新の状態に更新
            if (hukusuu)
            {
                stackchange();
            }
            else
            {
                bo.setpropaty(getcont);
            }
            bo.listBox3.SelectedIndex = pops - 1;
            resetstack();
        }
        //繰り返す
        public void redo()
        {
            //バグ。あとで直して
            if (pops == bo.listBox3.Items.Count || bo.listBox3.Items.Count == 0) return;
            else if (pops == bo.listBox3.Items.Count - 1)
            {
                getlast();
                pops++;
                bo.listBox3.SelectedIndex++;
                return;
            }
            int i;

            //背景色
            if (his[pops].mode == (short)back.bc)
            {
                for (i = 0; i < his[pops].con.Length; i++)
                {
                    Control n = box[his[pops].con[i]].con;
                    Color c = (Color)his[pops].datatoward[i];
                    n.BackColor = c;
                    box[(int)n.Tag].bc = c;
                }
            }
            //選択色
            else if (his[pops].mode == (short)back.cc)
            {
                for (i = 0; i < his[pops].con.Length; i++)
                {
                    Control n = box[his[pops].con[i]].con;
                    Color c = (Color)his[pops].datatoward[i];
                    box[(int)n.Tag].cc = c;
                }
            }
            //有効無効切り替え
            else if (his[pops].mode == (short)back.en)
            {
                for (i = 0; i < his[pops].con.Length; i++)
                {
                    Control n = box[his[pops].con[i]].con;
                    n.Enabled = !n.Enabled;
                    n.Visible = !n.Visible;
                    //リストボックスにセット
                    if (his[pops].mode == (short)back.en)
                    {
                        if (n.Enabled == true)
                        {
                            bo.listBox1.Items.Add(box[(int)n.Tag].name);
                            bo.listBox2.Items.Remove(box[(int)n.Tag].name);
                        }
                        else
                        {
                            bo.listBox2.Items.Add(box[(int)n.Tag].name);
                            bo.listBox1.Items.Remove(box[(int)n.Tag].name);
                        }
                    }
                }
            }
            //フォント
            else if (his[pops].mode == (short)back.font)
            {
                for (i = 0; i < his[pops].con.Length; i++)
                {
                    Control n = box[his[pops].con[i]].con;
                    Font c = (Font)his[pops].datatoward[i];
                    n.Font = c;
                }
            }
            //表示文字
            else if (his[pops].mode >= (short)back.h1 && his[pops].mode <= (short)back.h5)
            {
                int m = (int)his[pops].mode - (int)back.h1;
                for (i = 0; i < his[pops].con.Length; i++)
                {
                    string c = (string)his[pops].datatoward[i];
                    box[his[pops].con[i]].txt[m] = c;
                }
                changekategori(box[95 + m].con, null);

            }
            //入力文字
            else if (his[pops].mode >= (short)back.i1 && his[pops].mode <= (short)back.i5)
            {
                int m = (int)his[pops].mode - (int)back.i1;
                for (i = 0; i < his[pops].con.Length; i++)
                {
                    string c = (string)his[pops].datatoward[i];
                    box[his[pops].con[i]].inp[m] = c;
                }
                changekategori(box[95 + m].con, null);
            }
            //Location
            else if (his[pops].mode == (short)back.lo)
            {
                for (i = 0; i < his[pops].con.Length; i++)
                {
                    Control n = box[his[pops].con[i]].con;
                    Point c = (Point)his[pops].datatoward[i];
                    n.Location = c;
                }
            }
            //サイズ
            else if (his[pops].mode == (short)back.si)
            {
                int s = 0;
                for (i = 0; i < his[pops].con.Length; i++)
                {
                    Control n = box[his[pops].con[i]].con;
                    Size c = (Size)his[pops].datatoward[s++];
                    Point p = (Point)his[pops].datatoward[s++]; 
                    n.Size = c;
                    n.Location = p;
                }
            }
            //テキスト
            else if (his[pops].mode == (short)back.text)
            {
                for (i = 0; i < his[pops].con.Length; i++)
                {
                    Control n = box[his[pops].con[i]].con;
                    string c = (string)his[pops].datatoward[i];
                    n.Text = c;
                }
            }
            //文字色
            else if (his[pops].mode == (short)back.moji)
            {
                for (i = 0; i < his[pops].con.Length; i++)
                {
                    Control n = box[his[pops].con[i]].con;
                    Color c = (Color)his[pops].datatoward[i];
                    n.ForeColor = c;
                }
            }

            ////チェックボックス
            //else if (his[pops].mode == (short)back.che)
            //{
            //    CheckBox n = (CheckBox)his[pops].datatoward[0];
            //    n.Checked = !n.Checked;
            //}

            ////オンマウス待機時間
            //else if (his[pops].mode == (short)back.onnum)
            //{
            //    bo.comboBox1.SelectedIndex = (int)his[pops].databack[0];
            //    bo.numon = bo.comboBox1.SelectedIndex;
            //}

            ////透過度
            //else if (his[pops].mode == (short)back.num)
            //{
            //    bo.numericUpDown1.Value = (decimal)his[pops].databack[0];
            //    bo.numnum = (int)bo.numericUpDown1.Value;
            //}

            //ボーダーライン
            else if (his[pops].mode == (short)back.bord)
            {
                for (i = 0; i < his[pops].con.Length; i++)
                {
                    Label la = (Label)box[his[pops].con[i]].con;
                    BorderStyle bor = (BorderStyle)his[pops].datatoward[i];
                    la.BorderStyle = bor;
                }
            }

            //背景画像
            else if (his[pops].mode == (short)back.back)
            {
                resetstack();
                try
                {
                    bo.backimagetextbox.Text = his[pops].datatoward[0].ToString();
                    backimage = Image.FromFile(bo.backimagetextbox.Text);
                    if (imageflag == true)
                    {
                        this.BackgroundImage = new Bitmap(backimage, this.Size);
                    }
                    else
                    this.BackgroundImage = new Bitmap(backimage);
                }
                catch
                {
                    if(his[pops].datatoward[0].ToString()!="")
                        MessageBox.Show(his[pops].datatoward[0].ToString() + "\nが見つかりません", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    backimage = null;
                    this.BackgroundImage = null;
                    bo.backimagetextbox.Text = "";
                }
            }
            //サウンド
            else if (his[pops].mode == (short)back.sound)
            {
                resetstack();
                string fn = his[pops].datatoward[0].ToString();
                if (fn != "")
                {
                    bo.soundtextbox.Text = fn;
                    sound = new SoundPlayer(fn);
                }
                else
                {
                    if (his[pops].datatoward[0].ToString() != "")
                        MessageBox.Show(his[pops].datatoward[0].ToString() + "\nが見つかりません", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    bo.soundtextbox.Text = "";
                    sound = null;
                }
            }
            //プロパティを最新の状態に更新
            if (hukusuu)
            {
                stackchange();
            }
            else
            {
                bo.setpropaty(getcont);
            }
            pops++;
            bo.listBox3.SelectedIndex = pops - 1;
            resetstack();
        }
        //履歴のクリア
        public void resethist()
        {
            bo.listBox3.Items.Clear();
            pops = 0;
            his = new history[maxhis];
        }
        //データの書き換え履歴の作成
        public void sethistory()
        {
            saveflag = false;

            //履歴５０で押し出し
            if (pops == maxhis)
            {

                pops = maxhis-1;
                for (int i = 1; i <= maxhis-1; i++)
                {
                    his[i - 1] = his[i];
                }
                bo.listBox3.Items.RemoveAt(0);
            }
            //やり直し中だったら上書き
            if (pops != bo.listBox3.Items.Count)
            {
                int n = bo.listBox3.Items.Count - pops;
                for (int i = 0; i < n; i++)
                {
                    bo.listBox3.Items.Remove(bo.listBox3.Items[bo.listBox3.Items.Count - 1]);
                }
            }
            string text="";

            ////チェックボックスの変更を最優先
            //if (moh==(int)back.che||moh==(int)back.onnum)
            //{
            //    text = "入力設定変更";
            //    his[pops].databack = new ArrayList();         //リストの作成
            //    his[pops].databack.Add(dah);                 //変更前データの代入
            //    his[pops].mode = moh;                        //データの種類
            //    bo.listBox3.Items.Add(text);
            //    bo.listBox3.SelectedIndex = pops;
            //    pops++;
            //    return;
            //}

            ////ヌメリックアップダンの変更を優先
            //if (moh==(int)back.num)
            //{
            //    text = "透過度変更";
            //    his[pops].databack = new ArrayList();
            //    his[pops].databack.Add(dah);
            //    his[pops].mode = moh;
            //    bo.listBox3.Items.Add(text);
            //    bo.listBox3.SelectedIndex = pops;
            //    pops++;
            //    return;
            //}

            //複数なら
            if (hukusuu)
            {
                his[pops].con = new int[pop + 1];           //選択しているコントロールの数分配列の生成
                his[pops].databack = new ArrayList();
                his[pops].datatoward = new ArrayList();
                his[pops].mode = moh;
                for (int i = 0; i <= pop; i++)
                {
                    his[pops].con[i] = (int)stack[i].Tag;   //選択中コントロール全て代入  
                }

                //モードによって変わる

                //有効無効
                if (moh == (short)back.en)
                {
                    text += "有効無効切り替え";
                    for (int i = 0; i <= pop; i++)
                    {
                        his[pops].databack.Add(stack[i].Enabled);
                        his[pops].datatoward.Add(!stack[i].Enabled);
                        //リストボックスにセット
                        if (his[pops].mode == (short)back.en)
                        {
                            if (stack[i].Enabled == true)
                            {
                                bo.listBox1.Items.Add(box[(int)stack[i].Tag].name);
                                bo.listBox2.Items.Remove(box[(int)stack[i].Tag].name);
                            }
                            else
                            {
                                bo.listBox2.Items.Add(box[(int)stack[i].Tag].name);
                                bo.listBox1.Items.Remove(box[(int)stack[i].Tag].name);
                            }
                        }

                    }
                    resetstack();
                }

                //背景色
                if (moh == (short)back.bc)
                {
                    text += "背景色変更";
                    for (int i = 0; i <= pop; i++)
                    {
                        his[pops].databack.Add(box[(int)stack[i].Tag].bc);
                        his[pops].datatoward.Add(dah);
                    }
                }
                //選択色
                else if (moh == (short)back.cc)
                {
                    text += "選択色変更";
                    for (int i = 0; i <= pop; i++)
                    {
                        his[pops].datatoward.Add(dah);
                        his[pops].databack.Add(box[(int)stack[i].Tag].cc);
                    }
                }
                //フォント
                else if (moh == (short)back.font)
                {
                    text += "フォント変更";
                    for (int i = 0; i <= pop; i++)
                    {
                        his[pops].datatoward.Add(dah);
                        his[pops].databack.Add(stack[i].Font);
                    }
                }
                //表示文字
                else if (moh >= (short)back.h1 && moh <= (short)back.h5)
                {
                    text += "表示文字変更";
                    int kate = moh - (int)back.h1;
                    for (int i = 0; i <= pop; i++)
                    {
                        his[pops].databack.Add(box[(int)stack[i].Tag].txt[kate]);
                        his[pops].datatoward.Add(dah);
                    }
                }
                //入力文字
                else if (moh >= (short)back.i1 && moh <= (short)back.i5)
                {
                    text += "入力文字変更";
                    int kate = moh - (int)back.i1;
                    for (int i = 0; i <= pop; i++)
                    {
                        his[pops].databack.Add(box[(int)stack[i].Tag].inp[kate]);
                        his[pops].datatoward.Add(dah);
                    }
                }
                //文字色
                else if (moh == (short)back.moji)
                {
                    text += "文字色変更";
                    for (int i = 0; i <= pop; i++)
                    {
                        his[pops].databack.Add(stack[i].ForeColor);
                        his[pops].datatoward.Add(dah);
                    }
                }
                //サイズ
                else if (moh == (short)back.si)
                {
                    text += "サイズ変更";
                    for (int i = 0; i <= pop; i++)
                    {
                        his[pops].databack.Add(hissize[i]);
                        his[pops].databack.Add(hispoint[i]);
                        his[pops].datatoward.Add(stack[i].Size);
                        his[pops].datatoward.Add(stack[i].Location);
                    }
                }
                //テキスト
                else if (moh == (short)back.text)
                {
                    text += "テキストの変更";
                    for (int i = 0; i <= pop; i++)
                    {
                        his[pops].databack.Add(stack[i].Text);
                        his[pops].datatoward.Add(dah);
                    }
                }
                //ロケーション
                else if (moh == (short)back.lo)
                {
                    text += "配置の変更";
                    for (int i = 0; i <= pop; i++)
                    {
                        his[pops].databack.Add(hispoint[i]);
                        his[pops].datatoward.Add(stack[i].Location);
                    }
                }
                //ボーダイライン
                else if (moh == (short)back.bord)
                {
                    text += "ボーダーラインの変更";
                    for (int i = 0; i <= pop; i++)
                    {
                        Label la = (Label)stack[i];
                        his[pops].datatoward.Add(dah);
                        his[pops].databack.Add(la.BorderStyle);
                    }

                }

                text += "(複数)";

            }
            //一つなら
            else
            {
                his[pops].con = new int[1];               //要素数1の配列生成
                his[pops].con[0] = (int)getcont.Tag;      //選択中コントロールの代入
                his[pops].databack = new ArrayList();         //リストの作成
                his[pops].datatoward = new ArrayList();
                his[pops].mode = moh;                    //データの種類

                //有効無効
                if (moh == (short)back.en)
                {
                    text += "有効無効切り替え";
                    his[pops].databack.Add(getcont.Enabled);
                    his[pops].datatoward.Add(!getcont.Enabled);
                    //リストボックスにセット
                    if (his[pops].mode == (short)back.en)
                    {
                        if (getcont.Enabled == true)
                        {
                            bo.listBox1.Items.Add(box[(int)getcont.Tag].name);
                            bo.listBox2.Items.Remove(box[(int)getcont.Tag].name);
                        }
                        else
                        {
                            bo.listBox2.Items.Add(box[(int)getcont.Tag].name);
                            bo.listBox1.Items.Remove(box[(int)getcont.Tag].name);
                        }
                    }
                    else
                    {
                        text += "(" +box[(int)getcont.Tag].name+ ")";
                        bo.listBox3.Items.Add(text);
                        bo.listBox3.SelectedIndex = pops;
                        pops++;
                        return;
                    }
                    
                    resetstack();
                }

                //背景色
                if (moh == (short)back.bc)
                {
                    text += "背景色変更";
                    his[pops].databack.Add(box[his[pops].con[0]].bc);
                    his[pops].datatoward.Add(dah);
                }
                //選択色
                else if (moh == (short)back.cc)
                {
                    text += "選択色変更";
                    his[pops].databack.Add(box[his[pops].con[0]].cc);
                    his[pops].datatoward.Add(dah);
                }
                //フォント
                else if (moh == (short)back.font)
                {
                    text += "フォント変更";
                    his[pops].databack.Add(getcont.Font);
                    his[pops].datatoward.Add(dah);
                }
                //表示文字
                else if (moh >= (short)back.h1 && moh <= (short)back.h5)
                {
                    text += "表示文字変更";
                    int kate = moh - (int)back.h1;
                    his[pops].databack.Add(box[his[pops].con[0]].txt[kate]);
                    his[pops].datatoward.Add(dah);
                }
                //入力文字
                else if (moh >= (short)back.i1 && moh <= (short)back.i5)
                {
                    text += "入力文字変更";
                    int kate = moh - (int)back.i1;
                    his[pops].databack.Add(box[his[pops].con[0]].inp[kate]);
                    his[pops].datatoward.Add(dah);
                }
                //文字色
                else if (moh == (short)back.moji)
                {
                    text += "文字色変更";
                    his[pops].databack.Add(getcont.ForeColor);
                    his[pops].datatoward.Add(dah);
                }
                //サイズ
                else if (moh == (short)back.si)
                {
                    text += "サイズ変更";
                    his[pops].databack.Add(hissize[0]);
                    his[pops].databack.Add(hispoint[0]);
                    his[pops].datatoward.Add(getcont.Size);
                    his[pops].datatoward.Add(getcont.Location);
                }
                //テキスト
                else if (moh == (short)back.text)
                {
                    text += "テキストの変更";
                    his[pops].databack.Add(getcont.Text);
                    his[pops].datatoward.Add(dah);
                }
                //ロケーション
                else if (moh == (short)back.lo)
                {
                    text += "配置の変更";
                    his[pops].databack.Add(hispoint[0]);
                    his[pops].datatoward.Add(getcont.Location);
                }
                //ボーダイライン
                else if (moh == (short)back.bord)
                {
                    text += "ボーダーラインの変更";
                    his[pops].databack.Add(((Label)getcont).BorderStyle);
                    his[pops].datatoward.Add(dah);
                }
                //画像
                else if (moh == (short)back.back)
                {
                    text += "背景画像の変更";
                    his[pops].databack.Add(bo.backimagetextbox.Text);
                    his[pops].datatoward.Add(dah);
                }

                //サウンド
                else if (moh == (short)back.sound)
                {
                    text += "サウンドの変更";
                    his[pops].databack.Add(bo.soundtextbox.Text);
                    his[pops].datatoward.Add(dah);
                }

                //else if (moh == (short)back.back) text += "背景画像の変更";
                //else if (moh == (short)back.sound) text += "サウンドの変更";
                //else text += "テキストの変更";

                if (text != "サウンドの変更" && text != "背景画像の変更") 
                text += "(" + box[(int)getcont.Tag].name + ")";
            }

            //// 確認用
            // string st="もーど "+moh+"\n";
            // //コントロール名
            // for (int i = 0; i < his[pops].con.Length; i++)
            // {
            //     st += his[pops].con[i].ToString() + "\n";
            // }
            // //データ
            // for (int i = 0; i < his[pops].data.Count; i++)
            // {
            //     st += his[pops].data[i].ToString() + "\n";
            // }
            //// MessageBox.Show(st);
            bo.listBox3.Items.Add(text);
            pops++;
            bo.listBox3.SelectedIndex = pops-1;
        }
        //ラストヒストリーの作成
        void setlast(back mode)
        {
            //font, moji, en, lo, si, bc, cc, h1, h2, h3, h4, h5, i1, i2, i3, i4, i5, kuu, text, sx, sy,che,bord,back,sound,num,onnum
            lasthistory.databack = new ArrayList();

            //フォント
            if (mode == back.font)
            {
                for (int i = 0; i < lasthistory.con.Length; i++)
                {
                    lasthistory.databack.Add(box[lasthistory.con[i]].con.Font);
                }
            }
            //文字色
            else if (mode == back.moji)
            {
                for (int i = 0; i < lasthistory.con.Length; i++)
                {
                    lasthistory.databack.Add(box[lasthistory.con[i]].con.ForeColor);
                }
            }
            //有効無効
            else if (mode == back.en)
            {
                for (int i = 0; i < lasthistory.con.Length; i++)
                {
                    lasthistory.databack.Add(box[lasthistory.con[i]].con.Enabled);
                }
            }
            //配置
            else if (mode == back.lo)
            {
                for (int i = 0; i < lasthistory.con.Length; i++)
                {
                    lasthistory.databack.Add(box[lasthistory.con[i]].con.Location);
                }
            }
            //サイズ
            else if (mode == back.si)
            {
                for (int i = 0; i < lasthistory.con.Length; i++)
                {
                    lasthistory.databack.Add(box[lasthistory.con[i]].con.Size);
                    lasthistory.databack.Add(box[lasthistory.con[i]].con.Location);
                }
            }
            //背景色
            else if (mode == back.bc)
            {
                for (int i = 0; i < lasthistory.con.Length; i++)
                {
                    lasthistory.databack.Add(box[lasthistory.con[i]].bc);
                }
            }
            //選択色
            else if (mode == back.cc)
            {
                for (int i = 0; i < lasthistory.con.Length; i++)
                {
                    lasthistory.databack.Add(box[lasthistory.con[i]].cc);
                }
            }
            //表示文字
            else if (mode >= back.h1 && mode <= back.h5)
            {
                int num = (int)mode - (int)back.h1;
                for (int i = 0; i < lasthistory.con.Length; i++)
                {
                    lasthistory.databack.Add(box[lasthistory.con[i]].txt[num]);
                }
            }
                //ボーダーライン
            else if (mode == back.bord)
            {
                for (int i = 0; i < lasthistory.con.Length; i++)
                {
                    Label n = (Label)box[lasthistory.con[i]].con;
                    lasthistory.databack.Add(n.BorderStyle);
                }
            }
            //入力文字
            else if (mode >= back.i1 && mode <= back.i5)
            {
                int num = (int)mode - (int)back.i1;
                for (int i = 0; i < lasthistory.con.Length; i++)
                {
                    lasthistory.databack.Add(box[lasthistory.con[i]].inp[num]);
                }
            }
            //テキスト
            else if (mode == back.text)
            {
                for (int i = 0; i < lasthistory.con.Length; i++)
                {
                    lasthistory.databack.Add(box[lasthistory.con[i]].con.Text);
                }
            }

            //画像
            else if (mode == back.back)
            {
                lasthistory.databack.Add(bo.backimagetextbox.Text);
            }
            //サウンド
            else if (mode == back.sound)
            {
                lasthistory.databack.Add(bo.soundtextbox.Text);
            }
            else MessageBox.Show("知らん");
        }
        //ラストヒストリーを参照する
        void getlast()
        {
            int i;
            
            //背景色
            if (lasthistory.mode == (short)back.bc)
            {
                for (i = 0; i < lasthistory.con.Length; i++)
                {
                    Control n = box[lasthistory.con[i]].con;
                    Color c = (Color)lasthistory.databack[i];
                    n.BackColor = c;
                    box[(int)n.Tag].bc = c;
                }
            }
            //選択色
            else if (lasthistory.mode == (short)back.cc)
            {
                for (i = 0; i < lasthistory.con.Length; i++)
                {
                    Control n = box[lasthistory.con[i]].con;
                    Color c = (Color)lasthistory.databack[i];
                    box[(int)n.Tag].cc = c;
                }
            }
            //有効無効切り替え
            else if (lasthistory.mode == (short)back.en)
            {
                for (i = 0; i < lasthistory.con.Length; i++)
                {
                    Control n = box[lasthistory.con[i]].con;
                    n.Enabled = !n.Enabled;
                    n.Visible = !n.Visible;
                    //リストボックスにセット
                    if (his[pops].mode == (short)back.en)
                    {
                        if (n.Enabled == true)
                        {
                            bo.listBox1.Items.Add(box[(int)n.Tag].name);
                            bo.listBox2.Items.Remove(box[(int)n.Tag].name);
                        }
                        else
                        {
                            bo.listBox2.Items.Add(box[(int)n.Tag].name);
                            bo.listBox1.Items.Remove(box[(int)n.Tag].name);
                        }
                    }
                }
            }
            //フォント
            else if (lasthistory.mode == (short)back.font)
            {
                for (i = 0; i < lasthistory.con.Length; i++)
                {
                    Control n = box[lasthistory.con[i]].con;
                    Font c = (Font)lasthistory.databack[i];
                    n.Font = c;
                }
            }
            //表示文字
            else if (lasthistory.mode >= (short)back.h1 && lasthistory.mode <= (short)back.h5)
            {
                int m = (int)lasthistory.mode - (int)back.h1;
                for (i = 0; i < lasthistory.con.Length; i++)
                {
                    string c = (string)lasthistory.databack[i];
                    box[lasthistory.con[i]].txt[m] = c;
                }
                changekategori(box[95 + m].con, null);

            }
            //入力文字
            else if (lasthistory.mode >= (short)back.i1 && lasthistory.mode <= (short)back.i5)
            {
                int m = (int)lasthistory.mode - (int)back.i1;
                for (i = 0; i < lasthistory.con.Length; i++)
                {
                    string c = (string)lasthistory.databack[i];
                    box[lasthistory.con[i]].inp[m] = c;
                }
                changekategori(box[95 + m].con, null);
            }
            //Location
            else if (lasthistory.mode == (short)back.lo)
            {
                for (i = 0; i < lasthistory.con.Length; i++)
                {
                    Control n = box[lasthistory.con[i]].con;
                    Point c = (Point)lasthistory.databack[i];
                    n.Location = c;
                }
            }
            //ボーダーライン
            else if (lasthistory.mode == (short)back.bord)
            {
                for (i = 0; i < lasthistory.con.Length; i++)
                {
                    Label n = (Label)box[lasthistory.con[i]].con;
                    BorderStyle bo = (BorderStyle)lasthistory.databack[i];
                    n.BorderStyle = bo;
                }
            }
            //サイズ
            else if (lasthistory.mode == (short)back.si)
            {
                int s = 0;
                for (i = 0; i < lasthistory.con.Length; i++)
                {
                    Control n = box[lasthistory.con[i]].con;
                    Size c = (Size)lasthistory.databack[s++];
                    Point p = (Point)lasthistory.databack[s++];
                    n.Size = c;
                    n.Location = p;
                }
            }
            //テキスト
            else if (lasthistory.mode == (short)back.text)
            {
                for (i = 0; i < lasthistory.con.Length; i++)
                {
                    Control n = box[lasthistory.con[i]].con;
                    string c = (string)lasthistory.databack[i];
                    n.Text = c;
                }
            }
            //文字色
            else if (lasthistory.mode == (short)back.moji)
            {
                for (i = 0; i < lasthistory.con.Length; i++)
                {
                    Control n = box[lasthistory.con[i]].con;
                    Color c = (Color)lasthistory.databack[i];
                    n.ForeColor = c;
                }
            }
            //画像
            else if (lasthistory.mode == (short)back.back)
            {
                try
                {
                    bo.backimagetextbox.Text = lasthistory.databack[0].ToString();
                    Image ima = Image.FromFile(bo.backimagetextbox.Text);
                    if (imageflag == true)
                    {
                        backimage = new Bitmap(ima, this.Size);
                    }
                    else
                    {
                        backimage = new Bitmap(ima);
                    }
                    this.BackgroundImage = backimage;
                }
                catch
                {
                    backimage = null;
                    bo.backimagetextbox.Text = "";
                    MessageBox.Show(lasthistory.databack[0].ToString() + "が見つかりません", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            //サウンド
            else if (lasthistory.mode == (short)back.sound)
            {
                try
                {
                    sound = new SoundPlayer(lasthistory.databack[0].ToString());
                    bo.soundtextbox.Text = sound.SoundLocation;
                }
                catch
                {
                    sound = null;
                    bo.soundtextbox.Text = "";
                    MessageBox.Show(lasthistory.databack[0].ToString() + "が見つかりません", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            //プロパティを最新の状態に更新
            if (hukusuu)
            {
                stackchange();
            }
            else
            {
                bo.setpropaty(getcont);
            }

            bo.listBox3.SelectedIndex = pops - 1;
            resetstack();
        }
        #endregion

        #region スタック操作

        //全ての選択状態をリセットし、フォームのプロパティを表示する
        public void resetstack()
        {
            
            for (int i = 0; i <= 103; i++) box[i].con.BackColor = box[i].bc;
            pop = -1;
            hukusuu = false;
            getcont = this;
            bo.setopropatyfirst(this);
        }

        //全ての選択状態をセットする
        void allsetstack()
        {
            //いったんリセット
            resetstack();
            pop = -1;
            //全てセットする
            for (int i = 0; i <= 103; i++)
            {
                if (box[i].con.Enabled == true)
                {
                    pop++;
                    stack[pop] = box[i].con;
                }
            }
            //編集画面を変える
            stackchange();
        }

        //スタックの内容から編集画面を変える
        void stackchange()
        {

            int i;
            short k, s, b;
            getcont = stack[0];
            bo.ch = getcont;
            if (pop >= 0)
            {

                //スタックのコントロールの種類を得る

                k = 0;
                s = 0;
                b = 0;
                for (i = 0; i <= pop; i++)
                {
                    stack[i].BringToFront();
                    if ((int)stack[i].Tag <= 95)
                    {
                        s = 1;
                        if ((int)stack[i].Tag >= 80) b = 1;
                    }
                    else if ((int)stack[i].Tag <= 102)
                    {
                        k = 1;
                    }
                    else
                    {
                        k = 2;
                    }
                }

                //一個選択は通常クリックと扱う
                if (pop == 0)
                {
                    bo.setpropaty(stack[0]);
                    pop = -1;
                    hukusuu = false;
                }
                else
                {

                    //一個以上選択で複数選択フラグをたてる
                    hukusuu = true;
                    bo.checkBox1.Checked = true;

                    //選択したコントロール全ての背景色を選択状態に
                    for (i = 0; i <= pop; i++)
                    {
                        Control n = stack[i];
                        n.BackColor = box[(int)n.Tag].cc;
                    }

                    //含まれるコントロールに応じてプロパティ欄の表示を切り替える
                    if (k == 0)
                    {
                        bo.alltrue();  //ラベルのみ
                        空白文字に設定するToolStripMenuItem.Visible = true;
                        if (b == 0)
                        {
                            bo.radioButton1.Enabled = true;
                            bo.radioButton2.Enabled = true;
                            bo.radioButton3.Enabled = true;
                            bo.radioButton1.Checked = false;
                            bo.radioButton2.Checked = false;
                            bo.radioButton3.Checked = false;
                        }
                        else
                        {
                            bo.radioButton1.Enabled = false;
                            bo.radioButton2.Enabled = false;
                            bo.radioButton3.Enabled = false;
                        }
                    }
                    else
                    {
                        bo.radioButton1.Enabled = false;
                        bo.radioButton2.Enabled = false;
                        bo.radioButton3.Enabled = false;
                        空白文字に設定するToolStripMenuItem.Visible = false;
                        bo.group2.Visible = false;  //カテゴリボタン含む
                        bo.group3.Visible = false;
                        bo.labelfontchange.Visible = true;
                        bo.labelcolorchange.Visible = true;
                        bo.checkBox1.Enabled = true;
                        bo.textbox[0].Enabled = true;
                        bo.textbox[1].Enabled = true;
                        bo.textbox[4].Enabled = true;
                        bo.textbox[5].Enabled = true;

                        if (k == 2)         //プログレスバー含む
                        {
                            bo.labelfontchange.Visible = false;
                            bo.labelcolorchange.Visible = false;
                            bo.textcc.Enabled = false;
                            bo.textbc.Enabled = false;
                            bo.group1.Height = 212;
                        }
                        if (s == 0)
                        {
                            bo.group1.Height = 241;
                        }
                        else
                        {
                            bo.group1.Height = 212;
                        }
                    }

                    bo.textbox[2].Text = "";
                    bo.textbox[3].Text = "";
                    bo.textbox[2].BackColor = bo.textbox[3].BackColor;
                    for (int m = 6; m <= 15; m++)
                    {
                        bo.textbox[m].Text = "";
                    }
                    bo.chicelabel.Text = "一括設定";
                }

            }
            //選択されなければフォームクリックの処理
            else //if(ModifierKeys==Keys.ControlKey)
            {
                bo.radioButton1.Enabled = false;
                bo.radioButton2.Enabled = false;
                bo.radioButton3.Enabled = false;
                pop = -1;
                bo.allfalse();
                getcont = this;
                bo.setopropatyfirst(this);
            }
            tuika = false;
        }

        //範囲選択完了時に選択範囲に含まれるコントロールをスタックに積む
        void getcontril()
        {
            int i, xxx, yyy;
            if (!tuika)
            {
                pop = -1;
            }

            //カテゴリボタン、特殊ボタン判定
            for (i = 95; i <= 102; i++)
            {
                Control n = box[i].con;
                xxx = (n.Location.X + n.Size.Width / 2);
                yyy = (n.Location.Y + n.Size.Height / 2);
                //選択範囲に含まれている
                if (xxx >= xl && xxx <= xr && yyy >= yu && yyy <= yd && n.Enabled)
                {
                    //未選択状態ならスタックに
                    if (n.BackColor == box[i].bc)
                    {
                        bo.group2.Visible = false;
                        n.BringToFront();
                        pop++;
                        stack[pop] = box[i].con;
                    }
                    //選択状態ならデリート
                    else
                    {
                        deleatstack(n);
                        n.BackColor = box[i].bc;
                    }
                }
            }

            //ラベル判定
            for (i = 0; i <= 79; i++)
            {
                Control n = box[i].con;
                xxx = (n.Location.X + (n.Size.Width / 2));
                yyy = (n.Location.Y + (n.Size.Height / 2));
                if (xxx >= xl && xxx <= xr && yyy >= yu && yyy <= yd && n.Enabled)
                {
                    if (n.BackColor == box[i].bc)
                    {
                        n.BringToFront();

                        pop++;
                        stack[pop] = box[i].con;
                    }
                    else
                    {
                        deleatstack(n);
                        n.BackColor = box[i].bc;
                    }
                }
            }

            //入力ボタン判定
            for (i = 80; i <= 94; i++)
            {
                Control n = box[i].con;
                xxx = (n.Location.X + (n.Size.Width / 2));
                yyy = (n.Location.Y + (n.Size.Height / 2));
                if (xxx >= xl && xxx <= xr && yyy >= yu && yyy <= yd && n.Enabled)
                {
                    if (n.BackColor == box[i].bc)
                    {
                        n.BringToFront();
                        pop++;
                        stack[pop] = box[i].con;
                    }
                    else
                    {
                        deleatstack(n);
                        n.BackColor = box[i].bc;
                    }
                }
            }

            //プログレスバー判定
            xxx = progressBar1.Location.X + (progressBar1.Size.Width / 2);
            yyy = progressBar1.Location.Y + (progressBar1.Size.Height / 2);
            if (xxx >= xl && xxx <= xr && yyy >= yu && yyy <= yd && progressBar1.Enabled)
            {
                if (progressBar1.BackColor == box[103].bc)
                {
                    progressBar1.BringToFront();
                    pop++;
                    stack[pop] = progressBar1;
                }
                else
                {
                    deleatstack(progressBar1);
                    progressBar1.BackColor = box[103].bc;
                }
            }

            stackchange();
        }

        //スタック中の引数コントロールと一番下のコントロールを交換する
        void changestack(Control n)
        {
            int i = 0;
            //引数のコントロールをスタック中から探索
            while (stack[i++] != n) ;
            i--;

            //入れ替え
            Control b = stack[i];
            stack[i] = stack[0];
            stack[0] = b;
        }

        //スタック中の引数のコントロールをスタックから取り除く
        void deleatstack(Control n)
        {
            int i = 0;
            //引数のコントロールをスタック中から探索
            while (stack[i++] != n) ;
            i--;

            //一番上のヤツでそこを上書きする
            if (pop >= 0)
            {
                stack[i] = stack[pop];
                pop--;
            }
            else
            {
                n.BackColor = box[(int)n.Tag].bc;
                getcont = this;
                bo.setopropatyfirst(this);
            }
        }

        #endregion

        #region 画面上部のメニュー

        //座標揃えボタンの初期化
        public void zahyo_button_naosu()
        {
            soroe = false;
            bo.xsetbutton.ForeColor = System.Drawing.SystemColors.ControlText;
            bo.xsetbutton.Text = "X座標";
            bo.ysetbutton.ForeColor = System.Drawing.SystemColors.ControlText;
            bo.ysetbutton.Text = "Y座標";
            bo.button1.ForeColor = System.Drawing.SystemColors.ControlText;
            bo.button1.Text = "幅";
            bo.button2.ForeColor = System.Drawing.SystemColors.ControlText;
            bo.button2.Text = "高さ";
            bo.bckintou.ForeColor = System.Drawing.SystemColors.ControlText;
            bo.bckintou.Text = "背景色";
            bo.cckintou.ForeColor = System.Drawing.SystemColors.ControlText;
            bo.cckintou.Text = "選択色";
            bo.fontkintou.ForeColor = System.Drawing.SystemColors.ControlText;
            bo.fontkintou.Text = "フォント";
            bo.mojiirokintou.ForeColor = System.Drawing.SystemColors.ControlText;
            bo.mojiirokintou.Text = "文字色";
            bo.zettairadio.Enabled = true;
            bo.soutairadio.Enabled = true;
        }

        //座標揃えるメソッド
        void soroemasu(Control n)
        {


            //絶対移動の対象選択
            if (bo.zettairadio.Checked == true||bo.bckintou.Text[0]=='ど'||bo.cckintou.Text[0]=='ど'||bo.fontkintou.Text[0]=='ど'||bo.mojiirokintou.Text[0]=='ど'||hukusuu==false)
            {
                if (bo.xsetbutton.Text[0] == 'ど')
                    bo.setxzahyo(n.Left);
                else if (bo.ysetbutton.Text[0] == 'ど')
                    bo.setyzahyo(n.Top - hose);
                else if (bo.button1.Text[0] == 'ど')
                    bo.setxsize(n.Width);
                else if (bo.button2.Text[0] == 'ど')
                    bo.setysize(n.Height);

                //プログレスバーはだめ
                else if (n != progressBar1)
                {
                    if (bo.bckintou.Text[0] == 'ど')
                        bo.setbc(box[(int)n.Tag].bc);
                    else if (bo.cckintou.Text[0] == 'ど')
                        bo.setcc(box[(int)n.Tag].cc);
                    else if (bo.fontkintou.Text[0] == 'ど')
                        bo.setfont(n.Font);
                    else
                        bo.setmojiiro(n.ForeColor);
                }
                else
                {
                    MessageBox.Show("プログレスバーは選択できません", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                zahyo_button_naosu();

            }

            //相対移動
            else
            {
                //スタック中のコントロールならスルー
                if (box[(int)n.Tag].cc == n.BackColor) return;
                if (bo.xsetbutton.Text[0] == 'ど') soutaimove(0, n);
                else if (bo.ysetbutton.Text[0] == 'ど') soutaimove(1, n);
                else if (bo.button1.Text[0] == 'ど') soutaimove(2, n);
                else if (bo.button2.Text[0] == 'ど') soutaimove(3, n);
                zahyo_button_naosu();
            }

        }

        //選択範囲を反転
        public void hanten()
        {
            int i;
            pop = -1;
            //カテゴリ、特殊ボタンのチェック
            for (i = 0; i <= 103; i++)
            {
                //使用されてないのはスルー
                if (box[i].con.Enabled)
                {
                    //選択されているコントロール
                    if (box[i].con.BackColor == box[i].cc)
                    {
                        box[i].con.BackColor = box[i].bc;
                    }
                    //選択されていないコントロール
                    else
                    {
                        pop++;
                        stack[pop] = box[i].con;
                        box[i].con.BackColor = box[i].cc;
                    }
                }
            }
            stackchange();

        }

        #endregion

        #region 補助メソッド

        /**フォームロード時の初期化**/
        private void mainedit_Load(object sender, EventArgs e)
        {
            kanren();
            getcont = this;
            // this.SetStyle(ControlStyles.Selectable, false);
            //button1.KeyDown += new KeyEventHandler(forcas);

            //コントロールの登録
            box[0].con = label1;
            box[1].con = label2;
            box[2].con = label3;
            box[3].con = label4;
            box[4].con = label5;
            box[5].con = label6;
            box[6].con = label7;
            box[7].con = label8;
            box[8].con = label9;
            box[9].con = label10;
            box[10].con = label11;
            box[11].con = label12;
            box[12].con = label13;
            box[13].con = label14;
            box[14].con = label15;
            box[15].con = label16;
            box[16].con = label17;
            box[17].con = label18;
            box[18].con = label19;
            box[19].con = label20;
            box[20].con = label21;
            box[21].con = label22;
            box[22].con = label23;
            box[23].con = label24;
            box[24].con = label25;
            box[25].con = label26;
            box[26].con = label27;
            box[27].con = label28;
            box[28].con = label29;
            box[29].con = label30;
            box[30].con = label31;
            box[31].con = label32;
            box[32].con = label33;
            box[33].con = label34;
            box[34].con = label35;
            box[35].con = label36;
            box[36].con = label37;
            box[37].con = label38;
            box[38].con = label39;
            box[39].con = label40;
            box[40].con = label41;
            box[41].con = label42;
            box[42].con = label43;
            box[43].con = label44;
            box[44].con = label45;
            box[45].con = label46;
            box[46].con = label47;
            box[47].con = label48;
            box[48].con = label49;
            box[49].con = label50;
            box[50].con = label51;
            box[51].con = label52;
            box[52].con = label53;
            box[53].con = label54;
            box[54].con = label55;
            box[55].con = label56;
            box[56].con = label57;
            box[57].con = label58;
            box[58].con = label59;
            box[59].con = label60;
            box[60].con = label61;
            box[61].con = label62;
            box[62].con = label63;
            box[63].con = label64;
            box[64].con = label65;
            box[65].con = label66;
            box[66].con = label67;
            box[67].con = label68;
            box[68].con = label69;
            box[69].con = label70;
            box[70].con = label71;
            box[71].con = label72;
            box[72].con = label73;
            box[73].con = label74;
            box[74].con = label75;
            box[75].con = label76;
            box[76].con = label77;
            box[77].con = label78;
            box[78].con = label79;
            box[79].con = label80;
            box[80].con = 入力ボタン1;
            box[81].con = 入力ボタン2;
            box[82].con = 入力ボタン3;
            box[83].con = 入力ボタン4;
            box[84].con = 入力ボタン5;
            box[85].con = 入力ボタン6;
            box[86].con = 入力ボタン7;
            box[87].con = 入力ボタン8;
            box[88].con = 入力ボタン9;
            box[89].con = 入力ボタン10;
            box[90].con = 入力ボタン11;
            box[91].con = 入力ボタン12;
            box[92].con = 入力ボタン13;
            box[93].con = 入力ボタン14;
            box[94].con = 入力ボタン15;
            box[95].con = カテゴリボタン1;
            box[96].con = カテゴリボタン2;
            box[97].con = カテゴリボタン3;
            box[98].con = カテゴリボタン4;
            box[99].con = カテゴリボタン5;
            box[100].con = ShiftKey;
            box[101].con = CtrlKey;
            box[102].con = ALTkey;
            box[103].con = progressBar1;
            box[104].con = this;


            //以下デフォで使用されないコントロール


            //コントロールのイベント登録

            int i;

            //コントロール共通イベントの追加
            for (i = 0; i <= 103; i++)
            {
                box[i].con.MouseUp += new MouseEventHandler(mouseup);
            }


            //プログレスバーのイベント登録
            progressBar1.MouseMove += new MouseEventHandler(inputbuttonMove);
            progressBar1.MouseDown += new MouseEventHandler(progressberMouseDown);
            box[103].cc = Color.Green;
            box[103].bc = Color.Red;
            box[103].name = "プログレスバー";
            progressBar1.BackColor = Color.Red;
            progressBar1.Tag = 103;

            //ラベルのイベント登録他
            for (i = 0; i <= 79; i++)
            {
                Label target = ((Label)box[i].con);
                target.MouseDown += new MouseEventHandler(inputbuttonDown);
                target.MouseUp += new MouseEventHandler(mouseup);
                target.MouseMove += new MouseEventHandler(inputbuttonMove);
                target.Tag = i;
                box[i].cc = System.Drawing.SystemColors.ControlDark;
                box[i].bc = System.Drawing.SystemColors.Control;
                box[i].txt = new string[5];
                box[i].inp = new string[5];
                box[i].name = "ラベル" + (i + 1);
            }
            //カテゴリボタンのイベント登録
            for (i = 95; i <= 99; i++)
            {
                Button target = ((Button)box[i].con);
                target.MouseDown += new MouseEventHandler(kategoributtondown);
                target.MouseUp += new MouseEventHandler(mouseup);
                target.MouseMove += new MouseEventHandler(inputbuttonMove);
                target.MouseEnter += new EventHandler(changekategori);
                target.Tag = i;
                box[i].txt = new string[5];
                box[i].inp = new string[5];
                box[i].name = "カテゴリボタン" + (i - 95);
                box[i].cc = System.Drawing.SystemColors.ControlDark;
                box[i].bc = System.Drawing.SystemColors.Control;

            }

            //入力ボタンのイベント登録
            for (i = 80; i <= 94; i++)
            {
                Button target = ((Button)box[i].con);
                target.MouseDown += new MouseEventHandler(inputbuttonDown);
                target.MouseMove += new MouseEventHandler(inputbuttonMove);
                target.Tag = i;
                box[i].txt = new string[5];
                box[i].inp = new string[5];
                box[i].name = "入力ボタン" + (i - 79);
                box[i].cc = System.Drawing.SystemColors.ControlDark;
                box[i].bc = System.Drawing.SystemColors.Control;
            }

            //オプションキーのイベント登録
            for (i = 100; i <= 102; i++)
            {
                ((Button)box[i].con).MouseDown += new MouseEventHandler(sonotabuttonDown);
                ((Button)box[i].con).MouseMove += new MouseEventHandler(inputbuttonMove);
                ((Button)box[i].con).Tag = i;
                box[i].txt = new string[5];
                box[i].inp = new string[5];
                box[i].cc = System.Drawing.SystemColors.ControlDark;
                box[i].bc = System.Drawing.SystemColors.Control;
            }
            box[100].name = "SHIFTキー";
            box[101].name = "CTRLキー";
            box[102].name = "ALTキー";

            box[104].name = "フォーム";
            this.MouseMove += new MouseEventHandler(mainedit_MouseMove);



            //ロード時はフォームのステータスを表示
            bo.group2.Visible = false;
            bo.group3.Visible = false;
            bo.checkBox1.Enabled = false;
            bo.textlx.Enabled = false;
            bo.textly.Enabled = false;
            bo.textcc.Enabled = false;
            mouseup(null, null);

            bo.setopropatyfirst(this);
            bo.labelfontchange.Visible = false;
            bo.labelcolorchange.Visible = false;

            //描画のちらつきをなくす
            this.SetStyle(
            ControlStyles.DoubleBuffer |         // 描画をバッファで実行する
            ControlStyles.UserPaint |            // 描画は（ＯＳでなく）独自に行う
            ControlStyles.AllPaintingInWmPaint,  // WM_ERASEBKGND を無視する
            true                                 // 指定したスタイルを適用「する」
            );


            //コマンドライン引数よりファイルを開く

            string[] cmds = System.Environment.GetCommandLineArgs();
            if (cmds.Length>1)
            {
                //取得したコマンドライン引数を元にアドレスを取得
                Properties.Settings.Default.adress = cmds[1];
                for (int m = 2; m < cmds.Length; m++) Properties.Settings.Default.adress += " " + cmds[m];
                Properties.Settings.Default.Save();

                //ファイルを開く
                StreamReader srm;
                srm = new StreamReader(Properties.Settings.Default.adress, Encoding.Default);
                string data = "";

                while (srm.Peek() >= 0)
                {
                    data += srm.ReadLine() + "\n";
                }
                StringReader sm = new StringReader(data);
                setdata(ref sm);
                Properties.Settings.Default.adress = "";
                Properties.Settings.Default.Save();
                sm.Close();
                srm.Close();

            }
            //引数がなければデフォルト起動
            else defo();

        }

        //コントロールキー押しながらで複数選択するメソッド
        void contsentaku(Control n)
        {
            if (hukusuu)
            {
                //既に選択中のコントロールをスタックから外す
                if (n.BackColor == box[(int)n.Tag].cc)
                {
                    n.BackColor = box[(int)n.Tag].bc;
                    int i;
                    //スタック中からそのコントロールを見つけ除外
                    for (i = 0; stack[i] != n; i++) ;
                    stack[i] = stack[pop];
                    stack[pop] = null;
                    pop--;
                }
                //未選択のコントロールをスタックに積む
                else
                {
                    n.BackColor = box[(int)n.Tag].cc;
                    pop++;
                    stack[pop] = n;
                    getcont = n;
                }
                stackchange();
            }
            else
            {
                //新たにスタックニつむ
                if (n.BackColor == box[(int)n.Tag].bc)
                {
                    stack[0] = getcont;
                    stack[1] = n;
                    n.BackColor = box[(int)n.Tag].cc;
                    pop = 1;
                    hukusuu = true;
                    stackchange();
                }
                //選択中のものを戻す
                else
                {
                    n.BackColor = box[(int)n.Tag].bc;
                    getcont = this;
                    bo.setopropatyfirst(this);
                }

            }
        }
       
        //スレッドを呼び出す
        public void collthread(short mode, object data)
        {
            moh = mode;
            dah = data;
            sethistory();
            //Thread ture = new Thread(new ThreadStart(sethistory));
            //ture.Start();
        }
      
        //範囲選択描画
        protected override void OnPaint(PaintEventArgs pea)
        {
            // Graphics オブジェクトの取得
            Graphics grfx = pea.Graphics;
            Pen pen = new Pen(Color.Green);
            pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
            // 四角形の内部を塗りつぶす
            grfx.DrawRectangle(pen, xl, yu, xr - xl, yd - yu);
            if (dorafla)
                bo.sutetasutext.Text = "マウス座標　　X=" + (mousex) + " Y=" + (mousey) + "          選択範囲サイズ　　X=" + (xr - xl) + " Y=" + (yd - yu);
        }

        //プログレスバーに限っては色の変更ではなくvalue値の変更
        private void progressBar1_BackColorChanged(object sender, EventArgs e)
        {
            if (progressBar1.BackColor == box[103].cc)
            {
                progressBar1.Value = 10;
            }
            else
            {
                progressBar1.Value = 0;
            }
        }

        //相対移動を補助するメソッド
        void soutaimove(int mode,Control target)
        {
            int m, p;
            hissize = new Size[pop+1];
            hispoint = new Point[pop + 1];
            //履歴の作成
            for (int i = 0; i <= pop; i++)
            {
                hissize[i] = stack[i].Size;
                hispoint[i] = stack[i].Location;
            }
            //X座標
            if (mode == 0)
            {
                p = stack[0].Left - target.Left;　//ターゲットと対象の差分を取得
                for (int i = 0; i <= pop; i++)  //差分だけスタック中のコントロールの移動
                {
                    stack[i].Left -= p;
                }
                collthread((short)back.lo, stack[0].Location);
            }
            //Y座標
            else if (mode == 1)
            {
                p = stack[0].Top - target.Top;　//ターゲットと対象の差分を取得
                for (int i = 0; i <= pop; i++)  //差分だけスタック中のコントロールの移動
                {
                    stack[i].Top -= p;
                }
                collthread((short)back.lo, stack[0].Location);
            }
            //幅
            else if (mode == 2)
            {
                p = stack[0].Width -target.Width;　//ターゲットと対象の差分を取得
                for (int i = 0; i <= pop; i++)  //差分だけスタック中のコントロールの移動
                {
                    stack[i].Width -= p;
                }
                collthread((short)back.si, stack[0].Size);
            }
            //高さ
            else if (mode == 3)
            {
                p = stack[0].Height - target.Height;　//ターゲットと対象の差分を取得
                for (int i = 0; i <= pop; i++)  //差分だけスタック中のコントロールの移動
                {
                    stack[i].Height -= p;
                }
                collthread((short)back.si, stack[0].Size);
            }
            hissize = null;
            hispoint = null;
        }

        //ファイルのドラッグ
        private void mainedit_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        //ファイルのドロップ
        private void mainedit_DragDrop(object sender, DragEventArgs e)
        {
            //ドロップされたファイルを取得
            string[] s = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            int n = s.Length;

            //ファイルの拡張子を取得
            int m = s[0].LastIndexOf('.') + 1;
            string kaku = s[0].Substring(m);
            
            //ＳＫＢファイルなら開く
            if (kaku == "skb")
            {
                //保存確認
                if(shallwesave()==false) return;

                //ファイルを開く
                StreamReader srm;
                srm = new StreamReader(s[0], Encoding.Default);
                string data = "";

                //文字列に置き換える
                while (srm.Peek() >= 0)
                {
                    data += srm.ReadLine() + "\n";
                }

                //skbファイルとして開く
                StringReader sm = new StringReader(data);
                setdata(ref sm);
                sm.Close();
                srm.Close();
            }

            //サウンドファイルの可能性
            if (kaku == "mp3" || kaku == "wav" || kaku == ".mp3" || kaku == ".wav")
            {
                setsound(s[0]);
            }

            //画像ファイルの可能性
            else
            {
                try
                {
                    //ダミー
                    Image ima = Image.FromFile(s[0]);
                    //キャッチされなければ背景画像に使用
                    setgrah(s[0]);
                }
                catch
                {
                    //MessageBox.Show("ドロップされたファイル(" + s[i] + ")\nは対応していないフォーマットである可能性があります", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

        }

        //コントロールを有効にしたとき画面外だったら端っこへ移動してやる
        private void 入力ボタン4_EnabledChanged(object sender, EventArgs e)
        {
            Control n = (Control)sender;
            if (n.Enabled == true)
            {
                //画面外なら
                if (n.Left > this.Width || n.Top > this.Height)
                {
                    n.Location = new Point(0, 0);
                }
            }
        }

        //リサイズ
        private void mainedit_Resize_1(object sender, EventArgs e)
        {
            resetstack();
        }

        //終了を阻止
        protected override CreateParams CreateParams
        {

            get
            {
                int CS_NOCLOSE = Convert.ToInt32("0x200", 16);

                CreateParams objCreateParams = base.CreateParams;

                objCreateParams.ClassStyle |= CS_NOCLOSE;

                return objCreateParams;

            }

        }

        #endregion

        //デバッグ用タイマー
        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                this.Text = stack[0].ToString();
            }
            catch { }
            //return;
            //try
            //{
            //    //this.Text = getcont.ToString();
            //    if (Control.ModifierKeys == Keys.Control)
            //    {
            //        this.Text = "押してる";
            //    }
            //    else
            //    {
            //        this.Text = "押してない";
            //    }
            //    this.Text += "   pop==" + pop;
            //    this.Text += "    pops==" + pops;
            //    this.Text += "   tuika==" + tuika.ToString();
            //    this.Text += "   hukusu==" + hukusuu.ToString();
            //    this.Text += "   onflag==" + onflag.ToString();
    
            //}
            //catch { }
        }

        //ファイルの関連付け
        void kanren()
        {
            return;

            //関連付ける拡張子
            string extension = ".skb";
            //実行するコマンドライン
            string commandline = "\"" + Application.ExecutablePath + "\" %1";
            //ファイルタイプ名
            string fileType = Application.ProductName;
            //説明（必要なし）
            string description = "スクリーンキーボードエディター";
            //動詞
            string verb = "open";
            //動詞の説明（エクスプローラのコンテキストメニューに表示される）
            //（必要なし）
            string verb_description = "スクリーンキーボードエディターで開く(&O)";
            //アイコンのパスとインデックス
            string iconPath = Application.ExecutablePath;
            int iconIndex = 0;

            //ファイルタイプを登録
            Microsoft.Win32.RegistryKey regkey =
                Microsoft.Win32.Registry.ClassesRoot.CreateSubKey(extension);
            regkey.SetValue("", fileType);
            regkey.Close();

            //ファイルタイプとその説明を登録
            Microsoft.Win32.RegistryKey shellkey =
                Microsoft.Win32.Registry.ClassesRoot.CreateSubKey(fileType);
            shellkey.SetValue("", description);

            //動詞とその説明を登録
            shellkey = shellkey.CreateSubKey("shell\\" + verb);
            shellkey.SetValue("", verb_description);

            //コマンドラインを登録
            shellkey = shellkey.CreateSubKey("command");
            shellkey.SetValue("", commandline);
            shellkey.Close();

            //アイコンの登録
            Microsoft.Win32.RegistryKey iconkey =
                Microsoft.Win32.Registry.ClassesRoot.CreateSubKey(
                fileType +ScreenKeyboard.Properties.Resources.icon);
            iconkey.SetValue("", iconPath + "," + iconIndex.ToString());
            iconkey.Close();
        }
    }
   
}



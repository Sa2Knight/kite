using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace ScreenKeyboard
{
    public partial class BOSS : Form
    {
        public BOSS()
        {
            InitializeComponent();
            this.IsMdiContainer = true;
        }

        //穴埋め

        #region 変数・構造体・列挙体の宣言

        public mainedit ma;
        public BOSS bo;

        public Label[] labelbox = new Label[17];
        public TextBox[] textbox = new TextBox[16];
        //元に戻すで使う列挙体
        enum back { font, moji, en,lo, si, bc, cc, h1, h2, h3, h4, h5, i1, i2, i3, i4, i5, kuu, text ,che,bord,back,sound,num,onnum};
        public string data = "", txt = "";
        public int kategori = 1;
        public Control ch = null;  //選択中のコントロール
        public int numnum = 100;
        public int numon = 19;
        const int hose = 0;

        #endregion

        #region 初期メソッド、補助メソッド
        //フォームロード
        private void BOSS_Load(object sender, EventArgs e)
        {
            splitContainer1.IsSplitterFixed = true;
            bosspaneler.IsSplitterFixed = true;
            //this.Controls.Remove(menupanel4);
            //this.Controls.Remove(menupanel3);
            panel1.Controls.Add(menupanel3);
            panel1.Controls.Add(menupanel4);
            mode1_Click(mode4, null);
            mode1_Click(mode1, null);

            ma = new mainedit();

            newToolStripMenuItem_Click(null, null);

            //初期化
            shokika();

            labelcolorchange.Click += new EventHandler(labelcolorchange_Click);
        }
        //初期化
        void shokika()
        {
            this.Location = new Point(0, ma.Location.Y);
            //コントロール配列に代入
            label1.Size = new Size(500, 500);
            menupanel3.Size = menupanel1.Size;
            menupanel3.Location = menupanel1.Location;
            menupanel3.BackColor = menupanel1.BackColor;
            menupanel4.Size = menupanel1.Size;
            menupanel4.Location = menupanel1.Location;
            menupanel4.BackColor = menupanel1.BackColor;
            labelbox[0] = labe1;
            labelbox[1] = labe2;
            labelbox[2] = labe3;
            labelbox[3] = labe4;
            labelbox[4] = labe5;
            labelbox[5] = labe6;
            labelbox[6] = labe7;
            labelbox[7] = labe8;
            labelbox[8] = labe9;
            labelbox[9] = labe10;
            labelbox[10] = labe11;
            labelbox[11] = labe12;
            labelbox[12] = labe13;
            labelbox[13] = labe14;
            labelbox[14] = labe15;
            labelbox[15] = labe16;
            labelbox[16] = labe17;

            textbox[0] = textlx;
            textbox[1] = textly;
            textbox[2] = textsx;
            textbox[3] = textsy;
            textbox[4] = textbc;
            textbox[5] = textcc;
            textbox[6] = texti1;
            textbox[7] = texti2;
            textbox[8] = texti3;
            textbox[9] = texti4;
            textbox[10] = texti5;
            textbox[11] = texte1;
            textbox[12] = texte2;
            textbox[13] = texte3;
            textbox[14] = texte4;
            textbox[15] = texte5;

            textbox[0].KeyUp += new KeyEventHandler(changelocationX);
            textbox[1].KeyUp += new KeyEventHandler(chagelocationY);
            textbox[2].KeyUp += new KeyEventHandler(changesizeX);
            textbox[3].KeyUp += new KeyEventHandler(chagesizeY);
            tekiteki.KeyUp += new KeyEventHandler(tekiteki_KeyUp);
            mode1.MouseEnter += new EventHandler(mode_MouseEnter);    
            mode3.MouseEnter += new EventHandler(mode_MouseEnter);
            mode4.MouseEnter += new EventHandler(mode_MouseEnter);
            mode1.MouseLeave += new EventHandler(mode_MouseLeave);
            mode3.MouseLeave += new EventHandler(mode_MouseLeave);
            mode4.MouseLeave += new EventHandler(mode_MouseLeave);

            for (int i = 6; i < 11; i++)
            {
                textbox[i].KeyUp += new KeyEventHandler(textchange);
                textbox[i + 5].KeyUp += new KeyEventHandler(inputchange);
            }
        }
        //直前に選択してたコントロールを選択状態から戻す
        void resetchoice()
        {
            //複数選択後ならまずそれを切る
            if (ma.hukusuu)
            {
                for (; ma.pop >= 0; ma.pop--)
                {
                    int t;
                    //スタックにラベルが積んである

                    t = Convert.ToInt32(ma.stack[ma.pop].Tag);
                    ma.stack[ma.pop].BackColor = ma.box[t].bc;
                }
                ma.hukusuu = false;
                return;
            }

            //ラベル
            else
            {
                int c = Convert.ToInt16(ch.Tag);
                ch.BackColor = ma.box[c].bc;
            }

        }
        //クリックしたコントロールを選択状態にする
        void setchoice()
        {
            int c = Convert.ToInt16(ch.Tag);
            ch.BackColor = ma.box[c].cc;
        }
        //入力文字の詳細窓を開く
        private void katekate(object sender, EventArgs e)
        {
            //フォームのインスタンス生成
            Form1 inputbox = new Form1();
            inputbox.boss = this;
            inputbox.StartPosition = FormStartPosition.Manual;
            inputbox.Location = Cursor.Position;
            inputbox.Top -= inputbox.Height;
            inputbox.Left -= inputbox.Width;
            //フォーム開く
            if (inputbox.ShowDialog() == DialogResult.OK)
            {
                //どのカテゴリのボタンが押されたか
                int n = Convert.ToInt32(((Button)sender).Tag);
                textbox[11 + n].Text = txt;
                //入力文字変更イベントへ飛ばす
                inputchange(textbox[11 + n], new KeyEventArgs(Keys.Enter));
            }
        }
        //プロパティ欄をすべてtrueにする
        public void alltrue()
        {
            labelfontchange.Visible = true;
            labelcolorchange.Visible = true;
            group2.Visible = true;
            group3.Visible = true;
            checkBox1.Enabled = true;
            textlx.Enabled = true;
            textly.Enabled = true;
            textcc.Enabled = true;
            textbc.Enabled = true;
            group1.Height = 212;
        }
        //プロパティ欄をほとんどfalseにする
        public void allfalse()
        {
            labelfontchange.Visible = false;
            labelcolorchange.Visible = false;
            group2.Visible = false;
            group3.Visible = false;
            checkBox1.Enabled = false;
            textlx.Enabled = false;
            textly.Enabled = false;
            textcc.Enabled = false;
        }

        #endregion

        #region セットプロパティ

        //コントロールが選択されたときその情報を全て表示する
        public void setpropaty(Control n)
        {
            resetchoice();
            ch = n;
            int k = (int)n.Tag;
            if (k <= 79)
            {
                alltrue();
                setchoice();
                checkBox1.Checked = ch.Enabled;
                radioButton1.Enabled = true;
                radioButton2.Enabled = true;
                radioButton3.Enabled = true;
                textbox[0].Text = "" + ch.Location.X;
                textbox[1].Text = "" + (ch.Location.Y - hose);
                textbox[2].Text = "" + ch.Size.Width;
                textbox[3].Text = "" + ch.Size.Height;
                textbox[4].Text = ma.box[k].bc.ToString();
                textbox[4].BackColor = ma.box[k].bc;
                textbox[5].Text = ma.box[k].cc.ToString();
                textbox[5].BackColor = ma.box[k].cc;

                for (int i = 0; i < 5; i++)
                {
                    textbox[6 + i].Text = ma.box[k].txt[i];
                    textbox[11 + i].Text = ma.box[k].inp[i];
                }

                //チェックボックスの切り替え
                Label m = (Label)ch;
                if (m.BorderStyle == BorderStyle.None) radioButton1.Checked = true;
                else if (m.BorderStyle == BorderStyle.FixedSingle) radioButton3.Checked = true;
                else radioButton2.Checked = true;
               
                chicelabel.Text = ma.box[k].name;
            }
            else if (k <= 102)
            {
                setchoice();
                //ボタンの種類に応じてプロパティの表示非表示を切り替える
                if (k <= 94)
                {
                    alltrue();
                }
                else if (k <= 99)
                {
                    allfalse();
                    group1.Height = 241;
                    labelfontchange.Visible = true;
                    labelcolorchange.Visible = true;
                    textbox[0].Enabled = true;
                    textbox[1].Enabled = true;
                    textbox[4].Enabled = true;
                    textbox[5].Enabled = true;
                    tekiteki.Text = ch.Text;
                    checkBox1.Enabled = true;
                }
                else
                {
                    allfalse();
                    tekiteki.Text = ((Button)ch).Text;
                    group1.Height = 241;
                    labelfontchange.Visible = true;
                    labelcolorchange.Visible = true;
                    textbox[0].Enabled = true;
                    textbox[1].Enabled = true;
                    textbox[4].Enabled = true;
                    textbox[5].Enabled = true;
                    checkBox1.Enabled = true;
                }
                checkBox1.Checked = ch.Enabled;
                textbox[0].Text = "" + ch.Location.X;
                textbox[1].Text = "" + (ch.Location.Y - hose);
                textbox[2].Text = "" + ch.Size.Width;
                textbox[3].Text = "" + ch.Size.Height;
                textbox[4].Text = ma.box[k].bc.ToString();
                textbox[4].BackColor = ma.box[k].bc;
                textbox[5].Text = ma.box[k].cc.ToString();
                textbox[5].BackColor = ma.box[k].cc;
                chicelabel.Text = ma.box[(int)ch.Tag].name;
                for (int i = 0; i < 5; i++)
                {
                    textbox[6 + i].Text = ma.box[k].txt[i];
                    textbox[11 + i].Text = ma.box[k].inp[i];
                }
                radioButton1.Enabled = false;
                radioButton2.Enabled = false;
                radioButton3.Enabled = false;
            }
            else if (k == 103)
            {
                setchoice();
                radioButton1.Enabled = false;
                radioButton2.Enabled = false;
                radioButton3.Enabled = false;
                labelfontchange.Visible = false;
                labelcolorchange.Visible = false;
                group2.Visible = false;
                group3.Visible = false;
                checkBox1.Enabled = true;
                textlx.Enabled = true;
                textly.Enabled = true;
                textcc.Enabled = false;
                textbc.Enabled = false;
                ProgressBar m = ((ProgressBar)n);
                checkBox1.Checked = m.Enabled;
                textbox[0].Text = "" + m.Location.X;
                textbox[1].Text = "" + (m.Location.Y - hose);
                textbox[2].Text = "" + m.Size.Width;
                textbox[3].Text = "" + m.Size.Height;
                textbox[4].BackColor = System.Drawing.Color.Gray;
                textbc.Text = "";
                textbox[4].Text = "";
                textbox[5].Text = "";
                textbox[5].BackColor = System.Drawing.Color.Gray;
                chicelabel.Text = "プログレスバー";

            }
            else
             setopropatyfirst(ma);
        }
        //フォームの情報を表示する
        
        public void setopropatyfirst(Form m)
        {
            ch = m;
            allfalse();
            group1.Height = 212;
            textlx.Text = "";
            textly.Text = "";
            textsx.Text = "" + m.Size.Width;
            textsy.Text = "" + m.Size.Height;
            textbc.BackColor = ma.BackColor;
            textbc.Enabled = true;
            textbc.Text = ma.BackColor.ToString();
            textcc.Text = "";
            radioButton1.Enabled = false;
            radioButton2.Enabled = false;
            radioButton3.Enabled = false;
            textcc.BackColor = Color.Gray;
            chicelabel.Text = "フォーム";
            ma.hukusuu = false;
        }
        
        //マウス操作でのサイズ、位置変更時にその情報を表示する
        public void setpropatymini(Control m, int a)
        {
            ch = m;
            if (a == 0)
            {
                checkBox1.Checked = m.Enabled;
                textbox[0].Text = "" + m.Location.X;
                textbox[1].Text = "" + (m.Location.Y - hose);
                textbox[2].Text = "" + m.Size.Width;
                textbox[3].Text = "" + m.Size.Height;
            }
            else if (a == 1)
            {

                checkBox1.Checked = m.Enabled;
                textbox[0].Text = "" + m.Location.X;
                textbox[1].Text = "" + (m.Location.Y - hose);
                textbox[2].Text = "" + m.Size.Width;
                textbox[3].Text = "" + m.Size.Height;
            }
            else if (a == 2)
            {
                checkBox1.Checked = m.Enabled;
                textbox[0].Text = "" + m.Location.X;
                textbox[1].Text = "" + (m.Location.Y - hose);
                textbox[2].Text = "" + m.Size.Width;
                textbox[3].Text = "" + m.Size.Height;
            }
            else
            {
                textlx.Text = "";
                textly.Text = "";
                textsx.Text = "" + m.Size.Width;
                textsy.Text = "" + m.Size.Height;
            }
        }

        #endregion

        #region プロパティ画面からデータの書き換え

        //有効無効の切り替え
        public void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            int p = ma.pop;
            //複数選択なら全部処理までループ
            if (ma.hukusuu == true)
            {
                
                do
                {
                    ch = (Control)ma.stack[p];
                    p--;

                    //リストボックスへの追加削除
                    if (checkBox1.Checked)
                    {
                        ch.Visible = true;
                        ch.Enabled = true;
                    }
                    else
                    {
                        ch.Visible = false;
                        ch.Enabled = false;
                    }
                    //スタックが切れるまで
                } while (p >= 0);
            }
            else
            {
                if (checkBox1.Checked)
                {
                    ch.Visible = true;
                    ch.Enabled = true;
                }
                else
                {
                    ch.Visible = false;
                    ch.Enabled = false;
                }
            }
            ma.collthread((short)back.en, ch.Enabled);
        }

        //サイズチェンジｘ
        void chagesizeY(object sender, KeyEventArgs e)
        {
            //入力された数値をもとにサイズを変更する
            if (e.KeyData == Keys.Enter)
            {
                if (textbox[3].Text.Length >= 3)
                {
                    //加算処理
                    if (textbox[3].Text[1] == '=')
                    {
                        if (textbox[3].Text[0] == '+')
                        {
                            try
                            {
                                int mm = Convert.ToInt32(textbox[3].Text.Substring(2));
                                kasanxsize(mm);
                            }
                            catch
                            {
                                MessageBox.Show("入力値が無効です","エラー",MessageBoxButtons.OK,MessageBoxIcon.Error);
                                textbox[3].Text = "" + ch.Size.Height;
                            }
                            return;
                        }
                        else if (textbox[3].Text[0] == '-')
                        {
                            try
                            {
                                int mm = Convert.ToInt32(textbox[3].Text.Substring(2)) * -1;
                                kasanxsize(mm);
                            }
                            catch
                            {
                                MessageBox.Show("入力値が無効です", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                textbox[3].Text = "" + ch.Size.Height;
                            }
                            return;
                        }
                    }
                }
                try
                {
                    int n = Convert.ToInt32(textbox[3].Text);
                    if (ch.Height == n) return;
                    setysize(n);
                    textbox[3].Text = ch.Height.ToString();
                }
                // 不正な入力で値を元に戻す
                catch
                {
                    MessageBox.Show("入力値が無効です", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textbox[3].Text = "" + ch.Size.Height;
                    return;
                }
            }
        }
        
        //サイズチェンジy
        void changesizeX(object sender, KeyEventArgs e)
        {
            //入力された数値をもとにサイズを変更する
            if (e.KeyData == Keys.Enter)
            {
                if (textbox[2].Text.Length >= 3)
                {
                    //加算処理
                    if (textbox[2].Text[1] == '=')
                    {
                        if (textbox[2].Text[0] == '+')
                        {
                            try
                            {
                                kasanysize(Convert.ToInt32(textbox[2].Text.Substring(2)));
                            }
                            catch
                            {
                                MessageBox.Show("入力値が無効です", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                textbox[2].Text = "" + ch.Width;
                            }
                            return;
                        }
                        else if (textbox[2].Text[0] == '-')
                        {
                            try
                            {
                                kasanysize(Convert.ToInt32(textbox[2].Text.Substring(2)) * -1);
                            }
                            catch
                            {
                                MessageBox.Show("入力値が無効です", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                textbox[2].Text = "" + ch.Width;
                            }
                            return;
                        }
                    }
                }
                try
                {
                    int n = Convert.ToInt32(textbox[2].Text);
                    if (n == ch.Width) return;
                    setxsize(n);
                    textbox[2].Text = "" + ch.Width;
                }
                // 不正な入力で値を元に戻す
                catch
                {
                    MessageBox.Show("入力値が無効です", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textbox[2].Text = "" + ch.Size.Width;
                    return;
                }
            }

        }

        //配置チェンジy
        void chagelocationY(object sender, KeyEventArgs e)
        {
            //入力された数値をもとにサイズを変更する
            if (e.KeyData == Keys.Enter)
            {
                if(textbox[1].Text.Length>=3){
                //加算処理
                    if (textbox[1].Text[1] == '=')
                    {
                        if (textbox[1].Text[0] == '+')
                        {
                            try
                            {
                                kasanxzahyo(Convert.ToInt32(textbox[1].Text.Substring(2)));
                            }
                            catch
                            {
                                MessageBox.Show("入力値が無効です", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                textbox[1].Text = "" + ch.Top + hose;
                            }
                            return;
                        }
                        else if (textbox[1].Text[0] == '-')
                        {
                            try
                            {
                                kasanxzahyo(Convert.ToInt32(textbox[1].Text.Substring(2)) * -1);
                            }
                            catch
                            {
                                MessageBox.Show("入力値が無効です", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                textbox[1].Text = "" + ch.Top + hose;
                            }
                            return;
                        }
                    }
                }
                try
                {
                    int n = Convert.ToInt32(textbox[1].Text);
                    if (n == ch.Top) return;
                    setyzahyo(n);
                }
                // 不正な入力で値を元に戻す
                catch
                {
                    MessageBox.Show("入力値が無効です", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textbox[1].Text = "" + ch.Top + hose;
                    return;
                }
            }
        }

        //配置チェンジz
        void changelocationX(object sender, KeyEventArgs e)
        {

            //入力された数値をもとにサイズを変更する
            if (e.KeyData == Keys.Enter)
            {
                if(textbox[0].Text.Length>=3){
                //加算処理
                    if (textbox[0].Text[1] == '=')
                    {
                        if (textbox[0].Text[0] == '+')
                        {
                            try
                            {
                                kasanyzahyo(Convert.ToInt32(textbox[0].Text.Substring(2)));
                            }
                            catch
                            {
                                MessageBox.Show("入力値が無効です", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                textbox[0].Text = "" + ch.Left;
                            }
                            return;
                        }
                        else if (textbox[0].Text[0] == '-')
                        {
                            try
                            {
                                kasanyzahyo(Convert.ToInt32(textbox[0].Text.Substring(2)) * -1);
                            }
                            catch
                            {
                                MessageBox.Show("入力値が無効です", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                textbox[0].Text = "" + ch.Left;
                            }
                            return;
                        }
                    }
                }
                try
                {
                    int n = Convert.ToInt32(textbox[0].Text);
                    if (n == ch.Left) return;
                    setxzahyo(n);
                }
                // 不正な入力で値を元に戻す
                catch
                {
                    MessageBox.Show("入力値が無効です", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textbox[0].Text = "" + ch.Left;
                    return;
                }
            }
        }

        //背景色変更
        private void textbc_Enter(object sender, EventArgs e)
        {
            colorDialog1.Color = ma.box[(int)ch.Tag].bc;
            if (colorDialog1.ShowDialog() == DialogResult.Cancel) return;
            if (colorDialog1.Color == ma.box[(int)ch.Tag].bc) return;
            setbc(colorDialog1.Color);
            textbc.Text = colorDialog1.Color.ToString();
            textbc.BackColor = colorDialog1.Color;
            labe4.Focus();
        }

        //選択色変更
        private void textcc_Enter(object sender, EventArgs e)
        {
            colorDialog1.Color = ma.box[(int)ch.Tag].cc;
            if (colorDialog1.ShowDialog() == DialogResult.Cancel) return;
            if (colorDialog1.Color == ma.box[(int)ch.Tag].cc) return;
            setcc(colorDialog1.Color);
            textcc.BackColor = colorDialog1.Color;
            textcc.Text = colorDialog1.Color.ToString();
            labe4.Focus();
        }

        //入力文字の変更
        void inputchange(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                int n = Convert.ToInt16(((TextBox)sender).Tag);
                string str = ((TextBox)sender).Text;
                short ba = (short)(back.i1 + n);

                if (str != ma.box[(int)ch.Tag].inp[n])
                {
                    int t = (int)back.i1 + n;
                    ma.collthread(ba,str);
                }

                //複数処理
                if (ma.hukusuu)
                {
                    int p = ma.pop;
                    do
                    {
                        short tag = Convert.ToInt16(ma.stack[p].Tag);
                        ma.box[tag].inp[n] = str;
                        p--;
                    } while (p >= 0);
                }

                //シングル処理

                else
                {
                    int m = Convert.ToInt16(ch.Tag);
                    ma.box[m].inp[n] = str;
                }


            }

        }

        //表示文字の変更
        void textchange(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                kategori = Convert.ToInt16(((TextBox)sender).Tag);
                string str = ((TextBox)sender).Text;

                if (str != ma.box[(int)ch.Tag].txt[kategori])
                {
                    int t = (int)back.h1 + kategori;
                    ma.collthread((short)t, str);
                }

                //複数処理
                if (ma.hukusuu)
                {
                    int p = ma.pop;
                    do
                    {
                        short tag = Convert.ToInt16(ma.stack[p].Tag);
                        ma.box[tag].txt[kategori] = ((TextBox)sender).Text;

                        p--;
                    } while (p >= 0);
                    ma.changekategori(ma.box[kategori + 95].con, null);
                }


            //シングル処理

                else
                {
                    int m = Convert.ToInt16(ch.Tag);
                    ma.box[m].txt[kategori] = ((TextBox)sender).Text;
                    ma.changekategori(ma.box[kategori + 95].con, null);
                }
            }
        }

        //フォントの変更
        private void labelfontchange_MouseDown(object sender, MouseEventArgs e)
        {
            if (ma.hukusuu) fontDialog1.Font = ma.stack[0].Font;
            else fontDialog1.Font = ch.Font;

            if (fontDialog1.ShowDialog() == DialogResult.Cancel) return;
            if (fontDialog1.Font.ToString() == ch.Font.ToString())  return;
            setfont(fontDialog1.Font);
        }

        //文字色変更
        void labelcolorchange_Click(object sender, EventArgs e)
        {
            if (ma.hukusuu) colorDialog1.Color = ma.stack[0].ForeColor;
            else colorDialog1.Color = ch.ForeColor;

            if (colorDialog1.ShowDialog() == DialogResult.Cancel) return;
            if (colorDialog1.Color == ch.ForeColor) return;
            setmojiiro(colorDialog1.Color);
        }

        //カテゴリボタン、特殊ボタンのテキスト
        void tekiteki_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (ma.hukusuu)
                {
                    ma.collthread((short)back.text, tekiteki.Text);
                    int p = ma.pop;
                    do
                    {
                        ((Button)ma.stack[p]).Text = tekiteki.Text;
                        p--;
                    } while (p >= 0);
                }
                else
                {
                    if (tekiteki.Text == ch.Text) return;
                    ma.collthread((short)back.text, tekiteki.Text);
                    ch.Text = tekiteki.Text;
                }
            }
        }

        #endregion

        #region コントロールのデータを引数に書き換えるメソッド郡

        //スタックにつまれたコントロールのX座標を引数にする
        public void setxzahyo(int x)
        {
            //履歴作る

            //複数処理
            if (ma.hukusuu == true)
            {
                ma.hispoint = new Point[ma.pop + 1];
                for (int i = 0; i <= ma.pop; i++) ma.hispoint[i] = ma.stack[i].Location;
                ma.collthread((short)back.lo, ch.Location);
                int p = ma.pop;
                do
                {
                    //全部処理までループ

                    ch = ma.stack[p];
                    p--;
                    ch.Left = x;

                    // 不正な入力で値を元に戻す
                } while (p >= 0);
            }
            //シングル処理
            else
            {
                ma.hispoint = new Point[1];
                ma.hispoint[0] = ch.Location;
                ma.collthread((short)back.lo, ch.Location);
                ch.Left = x;
            }
        }
        //スタックにつまれたコントロールのＹ座標を引数にする
        public void setyzahyo(int y)
        {
            //複数処理
            if (ma.hukusuu == true)
            {
                ma.hispoint = new Point[ma.pop + 1];
                for (int i = 0; i <= ma.pop; i++) ma.hispoint[i] = ma.stack[i].Location;
                ma.collthread((short)back.lo, ch.Location);
                int p = ma.pop;
                do
                {
                    //全部処理までループ
                    ch = ma.stack[p];
                    p--;
                    ch.Top = y + hose;

                    // 不正な入力で値を元に戻す
                } while (p >= 0);
            }
            //シングル処理
            else
            {
                ma.hispoint = new Point[1];
                ma.hispoint[0] = ch.Location;
                ma.collthread((short)back.lo, ch.Location);
                ch.Top = y + hose;
            }
        }
        //スタックにつまれたコントロールのＸ座標を引数分加算する
        public void kasanyzahyo(int x)
        {
            //履歴作る

            //複数処理
            if (ma.hukusuu == true)
            {
                ma.hispoint = new Point[ma.pop + 1];
                for (int i = 0; i <= ma.pop; i++) ma.hispoint[i] = ma.stack[i].Location;
                ma.collthread((short)back.lo, ch.Location);
                int p = ma.pop;
                do
                {
                    //全部処理までループ

                    ch = ma.stack[p];
                    p--;
                    ch.Left += x;

                    // 不正な入力で値を元に戻す
                } while (p >= 0);
            }
            //シングル処理
            else
            {
                ma.hispoint = new Point[1];
                ma.hispoint[0] = ch.Location;
                ma.collthread((short)back.lo, ch.Location);
                ch.Left += x;
            }
        }
        //スタックにつまれたコントロールのＹ座標を引数分加算する
        public void kasanxzahyo(int y)
        {
            //複数処理
            if (ma.hukusuu == true)
            {
                ma.hispoint = new Point[ma.pop + 1];
                for (int i = 0; i <= ma.pop; i++) ma.hispoint[i] = ma.stack[i].Location;
                ma.collthread((short)back.lo, ch.Location);
                int p = ma.pop;
                do
                {
                    //全部処理までループ
                    ch = ma.stack[p];
                    p--;
                    ch.Top += y + hose;

                    // 不正な入力で値を元に戻す
                } while (p >= 0);
            }
            //シングル処理
            else
            {
                ma.hispoint = new Point[1];
                ma.hispoint[0] = ch.Location;
                ma.collthread((short)back.lo, ch.Location);
                ch.Top += y + hose;
            }
        }
        //スタックにつまれたコントロールの幅を引数にする
        public void setxsize(int x)
        {
            
            //複数処理
            if (ma.hukusuu == true)
            {
                ma.hissize = new Size[ma.pop + 1];
                ma.hispoint = new Point[ma.pop + 1];
                for (int i = 0; i <= ma.pop; i++)
                {
                    ma.hissize[i] = ma.stack[i].Size;
                    ma.hispoint[i] = ma.stack[i].Location;
                }
                    ma.collthread((short)back.si, ch.Size);
                int p = ma.pop;
                do
                {
                    //全部処理までループ

                    ch = ma.stack[p];
                    p--;
                    ch.Width = x;

                    // 不正な入力で値を元に戻す
                } while (p >= 0);
            }
            //シングル処理
            else
            {
                ma.hissize = new Size[1];
                ma.hispoint = new Point[1];
                ma.hissize[0] = ch.Size;
                ma.hispoint[0] = ch.Location;
                ma.collthread((short)back.si, ch.Size);
                ch.Width = x;
            }
        }
        //スタックニつまれたコントロールの高さを引数にする
        public void setysize(int y)
        {
            //複数処理
            if (ma.hukusuu == true)
            {
                ma.hissize = new Size[ma.pop + 1];
                ma.hispoint = new Point[ma.pop + 1];
                for (int i = 0; i <= ma.pop; i++)
                {
                    ma.hissize[i] = ma.stack[i].Size;
                    ma.hispoint[i] = ma.stack[i].Location;
                }
                ma.collthread((short)back.si, ch.Size);

                int p = ma.pop;
                do
                {
                    //全部処理までループ

                    ch = ma.stack[p];
                    p--;
                    ch.Height = y;
                } while (p >= 0);
            }
            //シングル処理
            else
            {
                ma.hissize = new Size[1];
                ma.hissize[0] = ch.Size;
                ma.hispoint = new Point[1];
                ma.hispoint[0] = ch.Location;
                ma.collthread((short)back.si, ch.Size);
                ch.Height = y;
            }
        }
        //スタックにつまれたコントロールの幅を引数文化産する
        public void kasanysize(int x)
        {

            //複数処理
            if (ma.hukusuu == true)
            {
                ma.hissize = new Size[ma.pop + 1];
                ma.hispoint = new Point[ma.pop + 1];
                for (int i = 0; i <= ma.pop; i++)
                {
                    ma.hissize[i] = ma.stack[i].Size;
                    ma.hispoint[i] = ma.stack[i].Location;
                }
                ma.collthread((short)back.si, ch.Size);
                int p = ma.pop;
                do
                {
                    //全部処理までループ

                    ch = ma.stack[p];
                    p--;
                    ch.Width += x;

                    // 不正な入力で値を元に戻す
                } while (p >= 0);
            }
            //シングル処理
            else
            {
                ma.hissize = new Size[1];
                ma.hispoint = new Point[1];
                ma.hissize[0] = ch.Size;
                ma.hispoint[0] = ch.Location;
                ma.collthread((short)back.si, ch.Size);
                ch.Width += x;
            }
        }
        //スタックにつまれたコントロールの高さを引数ぶん加算する
        public void kasanxsize(int y)
        {
            //複数処理
            if (ma.hukusuu == true)
            {
                ma.hissize = new Size[ma.pop + 1];
                ma.hispoint = new Point[ma.pop + 1];
                for (int i = 0; i <= ma.pop; i++)
                {
                    ma.hissize[i] = ma.stack[i].Size;
                    ma.hispoint[i] = ma.stack[i].Location;
                }
                ma.collthread((short)back.si, ch.Size);

                int p = ma.pop;
                do
                {
                    //全部処理までループ

                    ch = ma.stack[p];
                    p--;
                    ch.Height += y;
                } while (p >= 0);
            }
            //シングル処理
            else
            {
                ma.hissize = new Size[1];
                ma.hissize[0] = ch.Size;
                ma.hispoint = new Point[1];
                ma.hispoint[0] = ch.Location;
                ma.collthread((short)back.si, ch.Size);
                ch.Height += y;
            }
        }
        //スタックにつまれたコントロールの背景色を引数にする
        public void setbc(Color n)
        {
            if (ma.progressBar1.Value == 10) return;
            ma.collthread((short)back.bc,n);
            if (ma.hukusuu)
            {
                int p = ma.pop;
                //一致してるのがないかチェック
                do
                {
                    short tag = Convert.ToInt16(ma.stack[p].Tag);
                    //一致はできない
                    if (ma.box[tag].cc == n)
                    {
                        MessageBox.Show(ma.box[tag].name + "の背景色と選択色が一致しているため変更に失敗しました", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    p--;
                } while (p >= 0);

                //変更
                p = ma.pop;
                do
                {
                    short tag = Convert.ToInt16(ma.stack[p].Tag);
                    ma.box[tag].bc = n;
                    p--;
                } while (p >= 0);
            }
            else
            {
                int tag = (int)ch.Tag;
                //一致はできない
                if (ma.box[tag].cc == n)
                {
                    MessageBox.Show(ma.box[tag].name + "の背景色と選択色が一致しているため変更に失敗しました", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                ma.box[tag].bc = n;
                if (ch == ma) ma.BackColor = n;
            }

        }
        //スタックにつまれたコントロールの選択色を引数にする
        public void setcc(Color n)
        {
            if (ma.progressBar1.Value == 10) return;
            ma.collthread((short)back.cc,n);
            if (ma.hukusuu)
            {
                int p = ma.pop;
                do
                {
                    short tag = Convert.ToInt16(ma.stack[p].Tag);
                    //一致はできない
                    if (ma.box[tag].bc == n)
                    {
                        MessageBox.Show(ma.box[tag].name + "の背景色と選択色が一致しているため変更に失敗しました", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    p--;
                } while (p >= 0);

                p = ma.pop;

                //変更
                do
                {
                    short tag = Convert.ToInt16(ma.stack[p].Tag);
                    ma.box[tag].cc = n;
                    ma.box[tag].con.BackColor = n;
                    p--;
                } while (p >= 0);
            }
            else
            {
                int tag = (int)ch.Tag;
                //一致はできない
                if (ma.box[tag].bc == n)
                {
                    MessageBox.Show(ma.box[tag].name + "の背景色と選択色が一致しているため変更に失敗しました", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                ma.box[tag].cc = n;
                ch.BackColor = n;
            }
        }
        //スタックにつまれたコントロールのフォントを引数にする
        public void setfont(Font n)
        {
            if (ma.progressBar1.Value == 10) return;
            ma.collthread((short)back.font,n);
            if (ma.hukusuu)
            {
                int p = ma.pop;
                do
                {
                    int tag = (int)ma.stack[p].Tag;
                    ma.box[tag].con.Font = n;
                    p--;
                } while (p >= 0);
            }
            else
            {
                int tag = (int)ch.Tag;
                ma.box[tag].con.Font = n;
            }
        }
        //スタックにつまれたコントロールの文字色を引数にする
        public void setmojiiro(Color n)
        {
            if (ma.progressBar1.Value == 10) return;
            ma.collthread((short)back.moji,n);
            if (ma.hukusuu)
            {
                int p = ma.pop;
                do
                {
                    int tag = (int)ma.stack[p].Tag;
                    ma.box[tag].con.ForeColor = n;
                    p--;
                } while (p >= 0);
            }
            else
            {
                int tag = (int)ch.Tag;
                ch.ForeColor = n;
            }
        }

        #endregion

        #region ファイル操作関係

        //データの新規作成
        public void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult n;
            //フォームを開いてたら
            if (ma.Visible == true)
            {
                if(ma.shallwesave()==false)return;
                ma.defo();
                ma.WindowState = FormWindowState.Normal;
            }
            //閉じてたら新しく開く
            else
            {
                ma = new mainedit();

                //互いに連動させる
                ma.bo = this;

                ma.TopLevel = false;
                this.Controls.Add(ma);
                ma.Show();
                ma.BringToFront();

                //画面分割
                splitContainer1.Panel1.Controls.Add(ma);
                //ma.defo();
            }
        }
        //データを開く
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(ma.shallwesave()==false)return;
            ma.roodo();
        }
        //データを保存
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ma.Save();
        }
        //アプリケーションの終了
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        //スクリーンキーボードの実
        private void 実行ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //レイアウトを文字列に置き換える
            data = "";
            ma.getdata(ref data);
            Properties.Settings.Default.pure = data;

            //フォームのインスタンス生成
            key form3 = new key();

            //エディタ自体は最小化
            this.WindowState = FormWindowState.Minimized;

            //0.1秒ほど待っておく
            System.Threading.Thread.Sleep(100);

            //起動
            form3.ShowDialog();
            System.Threading.Thread.Sleep(100);
            this.WindowState = FormWindowState.Normal;

            Properties.Settings.Default.pure = "";
        }        
        // 終了確認
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            if (ma.shallwesave() == true)
            {
                e.Cancel = false;
            }
            else
            {
                e.Cancel = true;
            }

        }

        #endregion

        #region リストボックスイベント郡

        //リストボックス1ダブルクリック
        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                if (listBox1.SelectedIndices.Count == 0) return;
                ma.outputitems();
            }
            catch { }
        }

        //リストボックス1キーダウン
        private void listBox1_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter && listBox1.SelectedIndices.Count != 0)
                {
                    ma.outputitems();
                    try
                    {
                        listBox1.SelectedIndex = 0;
                    }
                    catch { }
                }
            }
            catch { }
        }

        //リストボックス1の右クリック
        private void listBox1_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Right)
                {
                    if (listBox1.SelectedIndex == -1) return;
                    ma.outputitems();
                }
            }
            catch { }
        }

        private void listBox2_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                if (listBox2.SelectedIndices.Count == 0) return;
                ma.inputitems();
            }
            catch { }
        }

        private void listBox2_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter && listBox2.SelectedIndices.Count != 0)
                {
                    ma.inputitems();
                    try
                    {
                        listBox2.SelectedIndex = 0;
                    }
                    catch { }
                }
            }
            catch { }
        }

        private void listBox2_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Right)
                {
                    if (listBox2.SelectedIndex == -1) return;
                    ma.inputitems();
                }
            }
            catch { }
        }

        //リストボックス３クリック
        private void listBox3_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            //this.Text = "pops==" + ma.pops + "   select==" + listBox3.SelectedIndex;
            int nn = ma.pops - listBox3.SelectedIndex - 1;
            if (nn > 0)
            {
                int i;
                for (i = 0; i < nn; i++)
                {
                    ma.undo();
                }
            }
            else if (nn < 0)
            {
                nn = nn * -1;
                int i;
                for (i = 0; i < nn; i++)
                {
                    ma.redo();
                }
            }
        }

        //履歴の最初に戻る
        private void button9_Click(object sender, EventArgs e)
        {
            int n = ma.pops + 2;
            for (int i = 0; i < n; i++)
            {
                ma.undo();
            }
        }
        //リストボックス右クリックメニュー
        //private void フォームに追加削除ToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    if (フォームに追加削除ToolStripMenuItem.Text == "フォームに追加")
        //    {
        //        ma.inputitems();
        //    }
        //    else
        //    {
        //        ma.outputitems();
        //    }
        //}

        #endregion

        #region 画面上部メニュー

        //メニューの切り替え
        private void mode1_Click(object sender, EventArgs e)
        {
            int n = Convert.ToInt32(((Label)sender).Tag);
            if (n == 1)
            {
                menupanel1.Visible = true;
                menupanel3.Visible = false;
                menupanel4.Visible = false;
            }
            else if (n == 2)
            {

            }
            else if (n == 3)
            {
                menupanel3.Visible = true;
                menupanel1.Visible = false;
                menupanel4.Visible = false;
            }
            else
            {
                menupanel4.Visible = true;
                menupanel3.Visible = false;
                menupanel1.Visible = false;
            }
        }

        //モードマウスリーブ
        void mode_MouseLeave(object sender, EventArgs e)
        {
            ((Label)sender).BackColor = Color.Beige;
        }
       
        //モードマウスエンター
        void mode_MouseEnter(object sender, EventArgs e)
        {
            ((Label)sender).BackColor = Color.Silver;
        }
       
        //メニュー内のチェックボックスのチェックドチェンジ
        private void menucheckclick(object sender, EventArgs e)
        {
            //ma.collthread((short)back.che, sender);
        }

        //均等化ボタンイベント
        public void Kintoukabutton(object sender, EventArgs e)
        {
            ma.Focus();
            Button b = ((Button)sender);
            if (ma.soroe == false)
            {
                if (ch == ma && !ma.hukusuu)
                {
                    MessageBox.Show("フォーム上のコントロールを選択してからクリックしてください", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                //絶対移動

                b.ForeColor = Color.Red;
                b.Text = "どれに？";
                ma.soroe = true;
                zettairadio.Enabled = false;
                soutairadio.Enabled = false;

            }
            else
            {
                ma.zahyo_button_naosu();
            }
        }
        
        //選択範囲の反転ボタンイベント
        private void button3_Click_1(object sender, EventArgs e)
        {
            ma.hanten();
        }

        //背景画像参照ボタン
        private void opengazoubutton_Click(object sender, EventArgs e)
        {
            ma.getgrah();
        }
      
        //背景画像クリアボタン
        private void button7_Click(object sender, EventArgs e)
        {
            ma.resetfrah();
        }

        //元に戻すボタン
        private void 元に戻すToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ma.undo();
        }

        //やり直すボタン
        private void やり直しToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ma.redo();
        }

        //背景画像表示切替ボタン
        private void button3_Click(object sender, EventArgs e)
        {
            ma.grahmodechange();
        }

        //履歴のリセットボタン
        private void button4_Click(object sender, EventArgs e)
        {
            ma.resethist();
        }

        //音楽参照
        private void button5_Click(object sender, EventArgs e)
        {
            ma.getsound();
        }

        //音楽クリア
        private void button6_Click(object sender, EventArgs e)
        {
            ma.resetsound();
        }

        //音楽再生
        private void button8_Click(object sender, EventArgs e)
        {
            ma.playsound();
        }

        //ラベルのボーダーライン変更
        private void radioButton1_MouseDown(object sender, MouseEventArgs e)
        {
            int tag = Convert.ToInt32(((Control)sender).Tag);
            BorderStyle n;
            if (tag == 1) n = BorderStyle.None;
            else if (tag == 3) n = BorderStyle.FixedSingle;
            else n = BorderStyle.Fixed3D;

            if (ma.hukusuu)
            {
                if (((RadioButton)sender).Checked == false)
                {
                    ma.collthread((short)back.bord, n);
                }
                int p = ma.pop;
                do
                {
                    Label la = (Label)ma.stack[p];
                    la.BorderStyle = n;
                    p--;
                } while (p >= 0);
            }
            else
            {
                Label la = (Label)ch;
                if (((RadioButton)sender).Checked == false)
                    ma.collthread((short)back.bord,n);
                la.BorderStyle = n;
            }
        }

        //透過度の変更
        private void menupanel4_MouseLeave(object sender, EventArgs e)
        {
            //if (numnum != (int)numericUpDown1.Value)
            //{
            //    ma.collthread((short)back.num, numnum);
            //    numnum = (int)numericUpDown1.Value;
            //}
        }

        //オンマウス入力時間の変更
        private void comboBox1_MouseLeave(object sender, EventArgs e)
        {
            //menupanel4.Focus();
            //if (numon != comboBox1.SelectedIndex)
            //{
            //    ma.collthread((short)back.onnum, numon);
            //    numon = comboBox1.SelectedIndex;
            //}
        }

        #endregion

    }
}

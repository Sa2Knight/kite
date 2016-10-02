using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ScreenKeyboard
{
    public partial class Form1 : Form
    {
        public BOSS boss;
        string index;
        public Form1()
        {
            this.Location = new Point(1280, 400);
            InitializeComponent();
        }

        //フォームロード
        private void Form1_Load(object sender, EventArgs e)
        {
            listBox1.SelectedIndex = 0;
            listBox2.SelectedIndex = 0;
            index = listBox1.SelectedItem.ToString();
        }

        //セレクトインデックスチェンジ
        void setindex(object sender, System.EventArgs e)
        {
            index = ((ListBox)sender).SelectedItem.ToString();
        }

        //OK
        private void button1_Click(object sender, EventArgs e)
        {
            string str = "";
            string n = "";
            if (Shiftcheck.Checked) n = "+";
            if (Ctrlcheck.Checked) n += "^";
            if (Altcheck.Checked) n += "%";
            //boss.txt = str +"/"+ index;
            
            switch (index){
                case "スペース":
                    str = n+" ";
                    break;
                case "バックスペース":
                    str = n+"{BS}";
                    break;
                case "Enter":
                    str = n + "{ENTER}";
                    break;
                case "Delete":
                    str = n + "{DEL}";
                    break;
                case "Home":
                    str = n + "{HOME}";
                    break;
                case "End":
                    str = n + "{END}";
                    break;
                case "PageUp":
                    str = n + "{PGUP}";
                    break;
                case "PageDown":
                    str = n + "{PGDN}";
                    break;
                case "PrintScreen":
                    str = n + "{PRTSC}";
                    break;
                case "ScrollLock":
                    str = n + "{SCROLLLOCK}";
                    break;
                case "NumLock":
                    str = n + "{NUMLOCK}";
                    break;
                case "Tab":
                    str = n + "{TAB}";
                    break;
                case "CapsLock":
                    str = n + "{CAPSLOCK}";
                    break;
                case "ESC":
                    str = n + "{ESC}";
                    break;
                case "F1":
                    str = n + "{F1}";
                    break;
                case "F2":
                    str = n + "{F2}";
                    break;
                case "F3":
                    str = n + "{F3}";
                    break;
                case "F4":
                    str = n + "{F4}";
                    break;
                case "F5":
                    str = n + "{F5}";
                    break;
                case "F6":
                    str = n + "{F6}";
                    break;
                case "F7":
                    str = n + "{F7}";
                    break;
                case "F8":
                    str = n + "{F8}";
                    break;
                case "F9":
                    str = n + "{F9}";
                    break;
                case "F10":
                    str = n + "{F10}";
                    break;
                case "F11":
                    str = n + "{F11}";
                    break;
                case "F12":
                    str = n + "{F12}";
                    break;
                case "↑":
                    str = n + "{UP}";
                    break;
                case "←":
                    str = n + "{LEFT}";
                    break;
                case "→":
                    str = n + "{RIGHT}";
                    break;
                case "↓":
                    str = n + "{DOWN}";
                    break;
                case "Insert":
                    str = n + "{INSERT}";
                    break;
                case "半角/全角":
                    str = "@/" + n + "/025" + index;
                    break;
                case "無変換":
                    str = "@/" + n + "/029" + index;
                    break;
                case "変換(次候補)":
                    str = "@/" + n + "/028" + index;
                    break;
                case "アプリケーションキー":
                    str = "@/" + n + "/093" + index;
                    break;
                case "Pause":
                    str = "@/" + n + "/019" + index;
                    break;
                case "Windowsキー(右)":
                    str = "@/" + n + "/092" + index;
                    break;
                case "Windowsキー(左)":
                    str = "@/" + n + "/091" + index;
                    break;

                case "音量アップ":
                    str = "@/" + n + "/175" + index;
                    break;
                case " ダウン":
                    str = "@/" + n + "/174" + "音量ダウン";
                    break;
                case " ミュート":
                    str = "@/" + n + "/173" + "音量ミュート";
                    break;
                case "メディアプレーヤーの起動":
                    str = "@/" + n + "/181" + index;
                    break;
                case " 次のトラックへ":
                    str = "@/" + n + "/176" + "メディアプレーヤーの次のトラックへ";
                    break;
                case " 前のトラックへ":
                    str = "@/" + n + "/177" + "メディアプレーヤーの前のトラックへ";
                    break;
                case " 再生/一時停止":
                    str = "@/" + n + "/179" + "メディアプレーヤーの再生/一時停止";
                    break;
                case " 停止":
                    str = "@/" + n + "/178" + "メディアプレーヤーの停止";
                    break;
                case "「戻る」":
                    str = "@/" + n + "/166" + index;
                    break;
                case "「進む」":
                    str = "@/" + n + "/167" + index;
                    break;
                case "「更新」":
                    str = "@/" + n + "/168" + index;
                    break;
                case "「検索」":
                    str = "@/" + n + "/170" + index;
                    break;
                case "「中止」":
                    str = "@/" + n + "/169" + index;
                    break;
                case "「お気に入り」":
                    str = "@/" + n + "/171" + index;
                    break;
                case "「ホーム」":
                    str = "@/" + n + "/172" + index;
                    break;
                //case "コンピュータのスリープ":
                //    str = "@/" + n + "/095" + index;
                //    break;
                case "メールの開始":
                    str = "@/" + n + "/180" + index;
                    break;
                    
                default:
                    str = "作ってない";
                    break;
            }

            boss.txt = str;
            this.DialogResult = DialogResult.OK;
            Close();
        }
        //キャンセル
        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            Close();
        }

        //タブが開かれた１
        private void tabPage1_Enter(object sender, EventArgs e)
        {
            index = listBox1.SelectedItem.ToString();
        }

        //タブが開かれた２
        private void tabPage2_Enter(object sender, EventArgs e)
        {
            index = listBox2.SelectedItem.ToString();
            Altcheck.Checked = false;
            Ctrlcheck.Checked = false;
            Shiftcheck.Checked = false;
        }
    }
}

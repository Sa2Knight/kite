using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ScreenKeyboard.Properties;

namespace ScreenKeyboard
{
    public partial class input : Form
    {
        key form1 = new key();

        public input()
        {
            InitializeComponent();
        }

        //キャンセルボタン
        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        //OKボタン
        private void button1_Click(object sender, EventArgs e)
        {
            Settings.Default.click = checkBox1.Checked;
            Settings.Default.onmouse = checkBox2.Checked;
            Settings.Default.interval = comboBox1.SelectedIndex + 1;
            Settings.Default.renzoku = checkBox3.Checked;
            Settings.Default.hoil = checkBox4.Checked;
            Settings.Default.onshu = comboBox2.SelectedIndex;
            Settings.Default.toukado = Convert.ToInt32(textBox1.Text);
            Settings.Default.Save();
            Close();
        }
        
        //フォームロード
        private void input_Load(object sender, EventArgs e)
        {
            //前回のオプションのまま
            checkBox1.Checked = Settings.Default.click;
            checkBox2.Checked = Settings.Default.onmouse;
            checkBox3.Checked = Settings.Default.renzoku;
            checkBox4.Checked = Settings.Default.hoil;
            checkBox5.Checked = Settings.Default.oto;
            textBox1.Text = "" + Settings.Default.toukado;
            comboBox2.SelectedIndex = Settings.Default.onshu;
            comboBox1.SelectedIndex = Settings.Default.interval-1;
            groupBox2.Enabled = checkBox2.Checked;
        }

        //オンマウスで入力　のチェックボックス
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            //オンマウスチェックで詳細オプション表示
            groupBox2.Enabled = checkBox2.Checked;
        }

        //クリックで入力　のチェックボックス
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

    }
}

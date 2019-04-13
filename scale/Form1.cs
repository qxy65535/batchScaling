using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WuJian.Common;

namespace scale
{
    public partial class Form1 : Form
    {
        private string path = null;
        int width = 0;
        int height = 0;
        int quality = 80;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.Description = "请选择需要批量处理图片所在的文件夹";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (string.IsNullOrEmpty(dialog.SelectedPath))
                {
                    MessageBox.Show(this, "文件夹路径不能为空", "提示");
                    return;
                }
                path = dialog.SelectedPath;
                string labelPath = path;
                string[] paths = path.Split('\\');
                if (path.Length > 50)
                    labelPath = paths[0] + "\\" + paths[1] + "\\" + "..." + 
                        "\\" + paths[paths.Length - 1];
                label1.Text = labelPath;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            width = 0;
            height = 0;
            quality = trackBar1.Value;

            if (path == null || !Directory.Exists(path))
            {
                MessageBox.Show(this, "请选择图片所在的路径", "错误");
                return;
            }

            if (checkBox1.Checked == true)
            {
                if (!(int.TryParse(textBox1.Text, out width) && width>0))
                {
                    MessageBox.Show(this, "请填写指定宽度为正整数", "错误");
                    return;
                }
            }
            if (checkBox2.Checked == true)
            {
                if (!(int.TryParse(textBox2.Text, out height) && height > 0))
                {
                    MessageBox.Show(this, "请填写指定高度为正整数", "错误");
                    return;
                }
            }
            if (width <= 0 && height<=0)
            {
                MessageBox.Show(this, "请至少指定宽度和高度其中之一", "错误");
                return;
            }

            progressBar1.Value = 0;
            Thread thread = new Thread(ScaleImage);

            thread.Start(this);

        }

        private void ScaleImage(object frm)
        {
            DirectoryInfo imageDir = new DirectoryInfo(path);
            string[] imgtype = ".BMP|.JPG|.GIF|.PNG|.JPEG".Split('|');
            FileInfo[] thefileInfo = imageDir.GetFiles("*.*", SearchOption.TopDirectoryOnly);
            int count = thefileInfo.Length;
            int i = 0;
            foreach (FileInfo file in thefileInfo)
            {
                if (imgtype.Contains(file.Extension.ToUpper()))
                    WuJian.Common.Image.CutForCustom(file.FullName,
                        path + "/scaled",
                        path + "/scaled/" + file.Name.Split('.')[0] + file.Extension,
                        width, height, quality);
                i++;
                double process = (double)i / count * 100;
                progressBar1.BeginInvoke(new EventHandler((sender, e) =>
                {
                    progressBar1.Value = (int)process;
                }), null);
            }
            var frm2 = frm as Form1;
            frm2.Invoke(new Action(() => {
                MessageBox.Show(this, "处理完成", "提示");
            }));
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                textBox1.ReadOnly = false;
            } else
            {
                textBox1.ReadOnly = true;
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked == true)
            {
                textBox2.ReadOnly = false;
            }
            else
            {
                textBox2.ReadOnly = true;
            }
        }
    }
}

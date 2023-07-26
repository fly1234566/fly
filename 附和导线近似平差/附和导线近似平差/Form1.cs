using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Data.OleDb;
using System.Data.SqlClient;

namespace 附和导线近似平差
{
    public partial class Form1 : Form
    {
        List<Point> Known_P = new List<Point>();
        List<CZ> CZs = new List<CZ>();
        DataTable dt = new DataTable();
        List<double> As = new List<double>();
        double fx, fy, fj, sumS;
        bool flag = true;
        Rectangle rect;
        bool click1 = false, click2 = false, click3 = false, click4 = false;
        public Form1()
        {
            InitializeComponent();
        }

        private void 导入数据ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "正在导入数据中";
            try
            {
                dt.Clear(); Known_P.Clear(); CZs.Clear();
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Title = "导入数据";
                ofd.Filter = "文本文件|*.txt";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string path = ofd.FileName;
                    string line;
                    string fg = ",";
                    char[] cfg = fg.ToCharArray();
                    #region 数据读入
                    using (StreamReader sr = new StreamReader(path))
                    {
                        sr.ReadLine();//读取第一行的误差数据
                        while (!String.IsNullOrEmpty(line = sr.ReadLine()))
                        {
                            string[] datas = line.Split(cfg, StringSplitOptions.RemoveEmptyEntries);
                            int n = datas.Length;
                            switch (n)
                            {
                                case 3:
                                    {
                                        Point p = new Point();
                                        p.name = datas[0];
                                        p.X = double.Parse(datas[1]);
                                        p.Y = double.Parse(datas[2]);
                                        p.known = true;
                                        Known_P.Add(p);
                                        break;
                                    }
                                case 1:
                                    {

                                        if (CZs.FindIndex(s => s.name.Equals(datas[0])) == -1)
                                        {
                                            CZ cz = new CZ();
                                            cz.name = datas[0];
                                            datas = sr.ReadLine().Split(cfg, StringSplitOptions.RemoveEmptyEntries);
                                            if (datas[1] == "L")
                                            {
                                                GCZ gcz1 = new GCZ();
                                                gcz1.CZ = datas[0];
                                                gcz1.type = datas[1];
                                                gcz1.value = calculate.JTH2(datas[2]);//注意角度转换
                                                cz.gczs.Add(gcz1);
                                                datas = sr.ReadLine().Split(cfg, StringSplitOptions.RemoveEmptyEntries);
                                                GCZ gcz2 = new GCZ();
                                                gcz2.CZ = datas[0];
                                                gcz2.type = datas[1];
                                                gcz2.value = calculate.JTH2(datas[2]);
                                                cz.gczs.Add(gcz2);
                                            }
                                            if (datas[1] == "S")
                                            {
                                                GCZ gcz = new GCZ();
                                                gcz.CZ = datas[0];
                                                gcz.type = datas[1];
                                                gcz.value = double.Parse(datas[2]);
                                                cz.gczs.Add(gcz);
                                            }
                                            CZs.Add(cz);
                                        }
                                        else
                                        {
                                            CZ cz = CZs.Find(s => s.name.Equals(datas[0]));
                                            datas = sr.ReadLine().Split(cfg, StringSplitOptions.RemoveEmptyEntries);
                                            if (datas[1] == "L")
                                            {
                                                GCZ gcz1 = new GCZ();
                                                gcz1.CZ = datas[0];
                                                gcz1.type = datas[1];
                                                gcz1.value = calculate.JTH2(datas[2]);
                                                cz.gczs.Add(gcz1);
                                                datas = sr.ReadLine().Split(cfg, StringSplitOptions.RemoveEmptyEntries);
                                                GCZ gcz2 = new GCZ();
                                                gcz2.CZ = datas[0];
                                                gcz2.type = datas[1];
                                                gcz2.value = calculate.JTH2(datas[2]);
                                                cz.gczs.Add(gcz2);
                                            }
                                            if (datas[1] == "S")
                                            {
                                                GCZ gcz = new GCZ();
                                                gcz.CZ = datas[0];
                                                gcz.type = datas[1];
                                                gcz.value = double.Parse(datas[2]);
                                                cz.gczs.Add(gcz);
                                            }
                                        }

                                        break;
                                    }
                            }
                        }
                    }
                    #endregion
                    //开始已知点1
                    dt.Rows.Add(Known_P[0].name, "", "", "", "", "", Known_P[0].X, Known_P[0].Y);
                    dt.Rows.Add();
                    CZs[0].X = Known_P[1].X; CZs[0].Y = Known_P[1].Y;
                    CZs[CZs.Count - 1].X = Known_P[2].X; CZs[CZs.Count - 1].Y = Known_P[2].Y;
                    for (int i = 0; i < CZs.Count - 1; i++)
                    {
                        CZ cz = CZs[i];
                        JD j = calculate.HTJ(cz.gczs[1].value - cz.gczs[0].value);
                        dt.Rows.Add(cz.name, j.d + "°" + j.f + "′" + j.m.ToString("0.00") + "″", "", "", "", "", cz.X, cz.Y);
                        dt.Rows.Add("", "", "", cz.gczs[2].value, "", "", "", "");
                    }
                    JD tempj = calculate.HTJ(CZs[CZs.Count - 1].gczs[1].value - CZs[CZs.Count - 1].gczs[0].value);
                    dt.Rows.Add(CZs[CZs.Count - 1].name, tempj.d + "°" + tempj.f + "′" + tempj.m.ToString("0.00") + "″", "", "", "", "", CZs[CZs.Count - 1].X, CZs[CZs.Count - 1].Y);
                    dt.Rows.Add("", "", "", "", "", "", "", "");
                    dt.Rows.Add(Known_P[3].name, "", "", "", "", "", Known_P[3].X, Known_P[3].Y);
                    dt.Rows.Add();
                    dataGridView1.DataSource = dt;
                    toolStripStatusLabel1.Text = "数据导入成功";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            dt.Columns.Add("点名");
            dt.Columns.Add("观测角（°′″）");
            dt.Columns.Add("坐标方位角（°′″）");
            dt.Columns.Add("边长（m）");
            dt.Columns.Add("dx（m）");
            dt.Columns.Add("dy（m）");
            dt.Columns.Add("x（m）");
            dt.Columns.Add("y（m）");
        }

        private void 保存报告ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Title = "保存报告";
                sfd.Filter = "文本文件|*.txt";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    string path = sfd.FileName;
                    using (StreamWriter sw = new StreamWriter(path))
                    {
                        sw.Write(richTextBox1.Text);
                    }
                }
                toolStripStatusLabel1.Text = "保存报告成功";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void 画图ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                rect = pictureBox1.ClientRectangle;
                Drawing.Create(Known_P, rect);
                pictureBox1.Refresh();
                tabControl1.SelectedTab = tabControl1.TabPages[1];
                toolStripStatusLabel1.Text = "绘图成功";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void 保存图像ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "jpg图像|*.jpg";
                string path = "";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    path = sfd.FileName;
                }
                Bitmap bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
                pictureBox1.DrawToBitmap(bmp, new Rectangle(0, 0, bmp.Width, bmp.Height));
                bmp.Save(path);
                toolStripStatusLabel1.Text = "图像保存成功";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Drawing.DrawAx(g, rect);
            if (click3)
                Drawing.DrawYZLines(g, Known_P, rect, Color.Black);
            if (click4)
                Drawing.DrawczLines(g, CZs, rect, Color.Blue);
            Drawing.DrawPoints(g, CZs, rect, click1);
            Drawing.DrawYZPoints(g, Known_P, rect, click2);

        }

        private void pictureBox1_ClientSizeChanged(object sender, EventArgs e)
        {
            rect = pictureBox1.ClientRectangle;
            Drawing.Create(Known_P, rect);
            pictureBox1.Refresh();
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            Drawing.MouseDown(e);
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            Drawing.MouseUp(e);
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            Drawing.MouseMove(e);
            pictureBox1.Refresh();
        }

        private void pictureBox1_MouseWheel(object sender, MouseEventArgs e)
        {
            Drawing.Delta(e);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                click1 = true;
                pictureBox1.Refresh();
            }
            else
            {
                click1 = false; pictureBox1.Refresh();
            }
        }
        public OleDbConnection myconnection;
        private void button1_Click(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "正在连接数据库";
            String strConnection = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\tanfei\Desktop\Database31.accdb;Persist Security Info=False";
            try
            {
                myconnection = new OleDbConnection(strConnection);
                myconnection.Open(); //打开数据库
                //button1.Text = "数据库连接成功！";
            }
            catch (Exception ee)
            {
                MessageBox.Show("数据库连接失败！" + ee.ToString());
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                String SQL = "select * From 原始数据"; //准备SQL命令字符串
                OleDbDataAdapter objDataAdpter = new OleDbDataAdapter(); //构造数据操作对象
                objDataAdpter.SelectCommand = new OleDbCommand(SQL, myconnection); //构造命令对象
                DataSet ds = new DataSet(); //构造数据集 
                objDataAdpter.Fill(ds, "原始数据"); //取得全部数据
                dataGridView2.DataSource = ds.Tables[0]; //显示
            }
            catch (Exception ee)
            {
                MessageBox.Show("查询失败！" + ee.ToString());
            }

        }
        public string Access_Mdb_Insdelupd(string sql, string link)
        {
            try
            {
                using (OleDbConnection oleDbConnection = new OleDbConnection(link))
                {
                    DataSet dataSet = new DataSet();
                    oleDbConnection.Open();
                    OleDbCommand oleDbCommand = new OleDbCommand(sql, oleDbConnection);
                    int num = oleDbCommand.ExecuteNonQuery();
                    oleDbConnection.Close();
                    return "success" + num;
                }
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            int j = 1;
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                try
                {
                    string strSQL1 = "insert into 原始数据(测站名,ID,方位角,距离,x坐标,y坐标) values(" +"'"+ dataGridView1.Rows[i].Cells[0].Value.ToString()+"'" + "," + j.ToString() + "," + "'"+dataGridView1.Rows[i].Cells[2].Value.ToString()+"'" + "," + "'"+dataGridView1.Rows[i].Cells[3].Value.ToString() +"'" +"," + "'"+dataGridView1.Rows[i].Cells[6].Value.ToString()+"'" + "," +"'"+ dataGridView1.Rows[i].Cells[7].Value.ToString() +"'"+ ")";
                    OleDbDataAdapter objDataAdpter = new OleDbDataAdapter();
                    OleDbCommand thisCommand = new OleDbCommand(strSQL1, myconnection);
                    thisCommand.ExecuteNonQuery();
                    ////////////前面已经完成数据插入，下面是刷新显示///////////////////
                }
                catch (Exception ee)
                {
                    toolStripStatusLabel1.Text = "数据插入失败";
                }
                j++;
            }
            string strSQL2 = "select * From 原始数据";
            OleDbDataAdapter objDataAdpter1 = new OleDbDataAdapter();
            objDataAdpter1.SelectCommand = new OleDbCommand(strSQL2, myconnection);
            DataSet ds = new DataSet();
            objDataAdpter1.Fill(ds, "原始数据");
            dataGridView2.DataSource = ds.Tables[0];
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                string strSQL1 = "delete from 原始数据 where ID="+this.textBox1.Text;
                OleDbCommand thisCommand = new OleDbCommand(strSQL1, myconnection);
                thisCommand.ExecuteNonQuery();
                string strSQL2 = "select * From 原始数据";
                OleDbDataAdapter objDataAdpter1 = new OleDbDataAdapter();
                objDataAdpter1.SelectCommand = new OleDbCommand(strSQL2, myconnection);
                DataSet ds = new DataSet();
                objDataAdpter1.Fill(ds, "原始数据");
                dataGridView1.DataSource = ds.Tables[0];
            }
            catch (Exception ee)
            {
                toolStripStatusLabel1.Text = "数据删除失败";
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                string strSQL1 = "update 原始数据 set "+"'"+textBox3.Text+"'"+"="+textBox4.Text+"where ID="+textBox2.Text;
                OleDbCommand thisCommand = new OleDbCommand(strSQL1, myconnection);
                thisCommand.ExecuteNonQuery();
                string strSQL2 = "select * From 原始数据";
                OleDbDataAdapter objDataAdpter1 = new OleDbDataAdapter();
                objDataAdpter1.SelectCommand = new OleDbCommand(strSQL2, myconnection);
                DataSet ds = new DataSet();
                objDataAdpter1.Fill(ds, "原始数据");
                dataGridView1.DataSource = ds.Tables[0];
            }
            catch (Exception ee)
            {
                toolStripStatusLabel1.Text = "数据更新失败";
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            String strConnection = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\tanfei\Desktop\Database31.accdb;Persist Security Info=False";
            try
            {
                myconnection = new OleDbConnection(strConnection);
                myconnection.Open(); //打开数据库
                button6.Text = "数据库连接成功！";
            }
            catch (Exception ee)
            {
                MessageBox.Show("数据库连接失败！" + ee.ToString());
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            string str1 = "delete * from 原始数据";
            OleDbCommand thisCommand = new OleDbCommand(str1, myconnection);
            thisCommand.ExecuteNonQuery();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            try
            {
                String SQL = "select * From 原始数据 where ID=" + textBox5.Text ; //准备SQL命令字符串
                OleDbDataAdapter objDataAdpter = new OleDbDataAdapter(); //构造数据操作对象
                objDataAdpter.SelectCommand = new OleDbCommand(SQL, myconnection); //构造命令对象
                DataSet ds = new DataSet(); //构造数据集 
                objDataAdpter.Fill(ds, "原始数据"); //取得全部数据
                dataGridView1.DataSource = ds.Tables[0]; //显示
            }
            catch (Exception ee)
            {
                MessageBox.Show("查询失败！" + ee.ToString());
            }
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            //if (checkBox4.Checked == true)
            //{
            //    click4 = true;
            //    pictureBox1.Refresh();
            //}
            //else
            //{
            //    click4 = false; pictureBox1.Refresh();
            //}
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked == true)
            {
                click2 = true;
                pictureBox1.Refresh();
            }
            else
            {
                click2 = false; pictureBox1.Refresh();
            }
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            //if (checkBox3.Checked == true)
            //{
            //    click3 = true;
            //    pictureBox1.Refresh();
            //}
            //else
            //{ click3 = false; pictureBox1.Refresh(); }
        }

        private void 生成报告ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                richTextBox1.Clear();
                tabControl1.SelectedTab = tabPage3;
                richTextBox1.AppendText("----------------限差要求-----------------------\n");
                richTextBox1.AppendText("角度闭合差限差：" + (40 * Math.Sqrt(CZs.Count)).ToString("0.000") + "″\n");
                richTextBox1.AppendText("导线全长相对闭合差限差：" + 0.00020000 + "\n\n");
                richTextBox1.AppendText(
                    "----------------导线基本信息-----------------------\n" +
                    "测站数：" + CZs.Count + "\n" +
                    "导线全长：" + sumS + "\n" +
                    "角度闭合差：" + (fj * 180 / Math.PI * 3600).ToString("0.000") + "″\n" +
                    "各站角度改正值：" + ((fj * 180 / Math.PI * 3600) / CZs.Count).ToString("0.000") + "″\n" +
                    "Y坐标闭合差：" + fx + "\n" +
                    "X坐标闭合差：" + fy + "\n" +
                    "导线全长相对闭合差：" + (Math.Sqrt(Math.Pow(fx, 2) + Math.Pow(fy, 2)) / sumS).ToString("0.00000000") + "\n\n");
                richTextBox1.AppendText(
                   "----------------测站点坐标-----------------------\n" +
                   "    测站名       X坐标          Y坐标  \n");
                foreach (CZ cz in CZs)
                {
                    richTextBox1.AppendText(
                        cz.name.PadLeft(10) +
                        cz.X.ToString("0.0000").PadLeft(15) +
                        cz.Y.ToString("0.0000").PadLeft(15) + "\n");
                }
                richTextBox1.AppendText("\n" +
                   "----------------角度数据-----------------------\n" +
                   "测站名                   观测角                       方位角\n ");
                int i = 1;
                JD a1 = calculate.HTJ(As[0]);
                richTextBox1.AppendText(
                    "                                                 " +
                    a1.d + "°" + a1.f + "′" + a1.m.ToString("0.00") + "″" + "\n");
                foreach (CZ cz in CZs)
                {
                    JD j = calculate.HTJ(cz.ZJ);
                    JD a = calculate.HTJ(As[i]);
                    richTextBox1.AppendText(
                        cz.name.PadRight(20) +
                        j.d + "°" + j.f + "′" + j.m.ToString("0.00") + "″" + "\n" +
                        "                                                 " +
                        a.d + "°" + a.f + "′" + a.m.ToString("0.00") + "″" + "\n");
                    i++;
                }
                MessageBox.Show("报告生成成功");

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void 文件ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void 计算ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                calculate.JSA(Known_P, CZs, As, ref fj, flag);
                calculate.JSZBZL(CZs, As, ref fx, ref fy, ref sumS, Known_P[1], Known_P[2]);
                int k = 1;
                JD a1 = calculate.HTJ(calculate.G_A(Known_P[0], Known_P[1]));
                dataGridView1.Rows[1].Cells[2].Value = a1.d + "°" + a1.f + "′" + a1.m.ToString("0.00") + "″";
                for (int i = 0; i < CZs.Count - 1; i++)
                {
                    CZ cz = CZs[i];
                    JD j = calculate.HTJ(cz.ZJ);
                    int n = 2 + 2 * (i + 1) - 1;
                    JD a = calculate.HTJ(As[k]);
                    dataGridView1.Rows[n].Cells[2].Value = a.d + "°" + a.f + "′" + a.m.ToString("0.00") + "″"; k++;
                    dataGridView1.Rows[n].Cells[4].Value = cz.dx;
                    dataGridView1.Rows[n].Cells[5].Value = cz.dy;
                    dataGridView1.Rows[n + 1].Cells[1].Value = j.d + "°" + j.f + "′" + j.m.ToString("0.00") + "″";
                    dataGridView1.Rows[n - 1].Cells[6].Value = cz.X.ToString("0.0000");
                    dataGridView1.Rows[n - 1].Cells[7].Value = cz.Y.ToString("0.0000");
                }
                a1 = calculate.HTJ(calculate.G_A(Known_P[2], Known_P[3]));
                dataGridView1.Rows[2 * CZs.Count + 2 - 1].Cells[2].Value = a1.d + "°" + a1.f + "′" + a1.m.ToString("0.00") + "″";
                MessageBox.Show("计算成功");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

    }
}

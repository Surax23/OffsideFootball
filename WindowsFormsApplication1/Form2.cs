using System;
using System.Drawing;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
            
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            Data.db.Close_connection();
            Application.Exit();
        }

        public void moment_fix(object sender, EventArgs e)
        {
            Form frm = new Form();
            frm.Size = new Size(1, 1);
            frm.AllowTransparency = true;
            frm.BackColor = Color.AliceBlue;
            frm.TransparencyKey = frm.BackColor;
            frm.FormBorderStyle = FormBorderStyle.None;
            frm.Show();
            frm.Close();
            float time;
            if (string.IsNullOrEmpty(textBox1.Text) || !float.TryParse(textBox1.Text, out time))
            {
                return;
            }
            Data.offside_moment_num = Data.model.add_moment(time, Data.list, Data.ball);
            if (Data.offside_moment_num[0] > -1)
            {
                #region Создаем новую кнопку
                Button b = (Button)sender;
                Button temp = new Button();
                Button minus = new Button();
                panel1.Controls.Add(temp);
                panel1.Controls.Add(minus);
                temp.Name = "btn" + Data.model.curr_moment.ToString();
                temp.Text = Data.model.curr_moment.ToString();
                temp.Width = 35;
                temp.Height = 24;
                minus.Text = "-";
                minus.Name = Data.model.curr_moment.ToString();
                minus.Width = 35;
                minus.Height = 20;


                if (Data.model.curr_moment == 1)
                {
                    temp.Location = new Point(0, 3);
                    minus.Location = new Point(0, temp.Height+3);
                }
                else
                {
                    temp.Location = new Point(((Data.model.curr_moment-1) * (temp.Width + 5)) + panel1.AutoScrollPosition.X, 3);
                    minus.Location = new Point(((Data.model.curr_moment - 1) * (temp.Width + 5)) + panel1.AutoScrollPosition.X, temp.Height+3);
                }
                temp.Click += new EventHandler(button_moments_Click);
                minus.Click += new EventHandler(button_minus_click);
                #endregion

                #region Активируем оффсайдную кнопку
                if (Data.offside_moment_num[0] > 0)
                {
                    server_mode_sw(null, new EventArgs());
                    foreach (var control in panel1.Controls)
                    {
                        Button btn = control as Button;
                        if (btn != null)
                        {
                            if (btn.Name == "btn" + Data.offside_moment_num[0].ToString())
                            {
                                btn.PerformClick();
                            }
                        }
                    }
                }
                #endregion
                Program.form1.Activate();
            }
        }

        private void reset_app(object sender, EventArgs e)
        {
            // Сброс всего и сначала.
            if (MessageBox.Show("Сбросить модель?", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Data.db.Close_connection();
                Application.Restart();
            }
        }

        private void button_moments_Click(object sender, EventArgs e)
        {
            Button b = (Button)sender;
            textBox1.Text = Data.model.moment[Convert.ToInt16(b.Text)].time.ToString();
            Data.list.Clear();
            Data.ball = (Ball)Data.model.moment[Convert.ToInt16(b.Text)].ball.Clone();
            foreach (Player po in Data.model.moment[Convert.ToInt16(b.Text)].players)
            {
                Data.list.Add((Player)po.Clone());
            }
            Data.list.Add((Ball)Data.ball.Clone());

            Program.form1.Invalidate();
        }

        private void button_minus_click(object sender, EventArgs e)
        {
            // Удаление кнопок и моментов
            Button b = (Button)sender;
            foreach (var control in panel1.Controls)
            {
                Button btn = control as Button;
                if (btn != null)
                {
                    if (btn.Name == "btn" + (Data.model.curr_moment).ToString())
                    {
                        btn.Dispose();
                    }
                }
            }
            foreach (var control in panel1.Controls)
            {
                Button btn = control as Button;
                if (btn != null)
                {
                    if (btn.Name == (Data.model.curr_moment).ToString())
                    {
                        btn.Dispose();
                    }
                }
            }
            Data.db.Delete_moment(Data.model, Data.model.moment[int.Parse(b.Name)].time);
            Data.model.del_moment(int.Parse(b.Name));
        }

        private void test_start(object sender, EventArgs e)
        {
            Test test = new Test();
        }

        private void db_load(object sender, EventArgs e)
        {
            if (Data.db.Push_moments(Data.model))
            {
                MessageBox.Show("Снимки успешно загружены!");
            }
            else
            {
                MessageBox.Show("Error 0x00001207");
            }
        }

        private void search_moment(object sender, EventArgs e)
        {
            if (Data.model.curr_moment > 0)
            {
                float time = 0;
                if (float.TryParse(txtbx_find_time.Text, out time))
                {
                    float delta_time = 6000;
                    int k = 0;
                    for (int i = 1; i < Data.model.moment.Count; i++)
                    {
                        if (Math.Abs(time - Data.model.moment[i].time) < delta_time)
                        {
                            delta_time = Math.Abs(time - Data.model.moment[i].time);
                            k = i;
                        }
                    }
                    foreach (var control in panel1.Controls)
                    {
                        if (control is Button btn)
                        {
                            if (btn.Name == "btn" + k.ToString())
                            {
                                btn.PerformClick();
                            }
                        }
                    }
                    MessageBox.Show("Искомый снимок: " + k);
                } else
                {
                    MessageBox.Show("Введено неверное число!");
                }
            } else
            {
                MessageBox.Show("Ни одного снимка не добавлено!");
            }
        }

        public void server_mode_sw(object sender, EventArgs e)
        {
            if (!Data.server_mode)
            {
                button5.ForeColor = Color.Green;
                button5.Text = "Сервер запущен";
                Data.server_mode = true;
            }
            else
            {
                Data.server_mode = false;
                button5.Text = "Сервер не запущен";
                button5.ForeColor = Color.Red;
            }
        }
    }
}
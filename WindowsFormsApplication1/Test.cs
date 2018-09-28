using System;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public class Test
    {
        int k_first_rand = 4;
        string team;
        float dtime;
        int sch = 0;

        public Test()
        {
            foreach (var control in Program.form2.Controls)
            {
                TextBox txt = control as TextBox;
                if (txt != null)
                {
                    if (txt.Name == "textBox1")
                    {
                        if (!(float.TryParse(txt.Text, out dtime)))
                        {
                            dtime = 0;
                            txt.Text = "0";
                        }
                    }
                }
            }
            bool nextstep = true;
            var k = new Random();
            int team_k;
            while (nextstep)
            {
                sch += 1;
                dtime = dtime + k.Next(1, 10) * 0.1f;
                foreach (var control in Program.form2.Controls)
                {
                    TextBox txt = control as TextBox;
                    if (txt != null)
                    {
                        if (txt.Name == "textBox1")
                        {
                            txt.Text = dtime.ToString();
                        }
                    }
                }
                System.Threading.Thread.Sleep(Convert.ToInt16(dtime) * 100);
                int num = k.Next(1, 5);
                team_k = k.Next(0, 20);
                if (team_k > 10)
                {
                    team = Data.team1;
                }
                else
                {
                    team = Data.team2;
                }
                foreach (MoveObject mo in Data.list)
                {
                    int curr_point_x = 0;
                    int curr_point_y = 0;
                    do
                    {
                        curr_point_x = mo.x + k.Next(-30, 30);
                    } while (curr_point_x < 1 || curr_point_x > Program.form1.ClientSize.Width);
                    do
                    {
                        curr_point_y = mo.y + k.Next(-30, 30);
                    } while (curr_point_y < 1 || curr_point_y > Program.form1.ClientSize.Height);
                    if (k_first_rand > 0)
                    {
                        Player po = mo as Player;
                        if (po.team == Data.team1)
                        {
                            if (po.num == 4)
                            {
                                curr_point_y = 150;
                                curr_point_x = 580;
                                k_first_rand--;
                            }
                            if (po.num == 5)
                            {
                                curr_point_y = 350;
                                curr_point_x = 580;
                                k_first_rand--;
                            }
                        } else if (po.team == Data.team2)
                        {
                            if (po.num == 4)
                            {
                                curr_point_x = 80;
                                curr_point_y = 150;
                                k_first_rand--;
                            }
                            if (po.num == 5)
                            {
                                curr_point_x = 80;
                                curr_point_y = 350;
                                k_first_rand--;
                            }
                        }
                    }
                    int deltax = curr_point_x - mo.x;
                    int deltay = curr_point_y - mo.y;
                    mo.x = curr_point_x;
                    mo.y = curr_point_y;
                    if (mo != null)
                    {
                        //mo.Move(deltax, deltay);
                        Player tmp = mo as Player;
                        if (tmp != null)
                        {
                            tmp.Move(deltax, deltay);
                        }
                    }
                    Ball bo = mo as Ball;
                    if (bo != null)
                    {
                        foreach (MoveObject mo2 in Data.list)
                        {
                            Player po = mo2 as Player;
                            if (po != null)
                            {
                                if (po.team == team && po.num == num)
                                {
                                    int b_curr_point_x = po.x + k.Next(1, 3);
                                    int b_curr_point_y = po.y + k.Next(1, 3);
                                    int b_deltax = curr_point_x - bo.x;
                                    int b_deltay = curr_point_y - bo.y;
                                    bo.x = b_curr_point_x;
                                    bo.y = b_curr_point_y;
                                    bo.Move(b_deltax, b_deltay);
                                    Data.ball = (Ball)bo.Clone();
                                }
                            }
                        }
                    }
                }
                foreach (var control in Program.form2.Controls)
                {
                    Button btn = control as Button;
                    if (btn != null)
                    {
                        if (btn.Name == "fix_moment")
                        {
                            btn.PerformClick();
                            foreach (var control2 in Program.form2.panel1.Controls)
                            {
                                Button btn2 = control2 as Button;
                                if (btn2 != null)
                                {
                                    if (btn2.Name == "btn" + sch.ToString())
                                    {
                                        btn2.PerformClick();
                                    }
                                }
                            }
                            Program.form2.Invalidate();
                            Program.form1.Invalidate();
                        }
                    }
                }
                if (MessageBox.Show("Далее?", "", MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    nextstep = false;
                }
            }
        }
    }
}

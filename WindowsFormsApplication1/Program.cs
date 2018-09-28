using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{    
    class Data
    {
        static public List<MoveObject> list = new List<MoveObject>();
        static public Ball ball;
        static public string team1, team2;
        static public Model model;
        static public int[] offside_moment_num = new int[2];
        static public Database db = new Database();
        static public bool server_mode = false;
    }

    static class Program
    {
        static byte[] s_raw = new byte[1024];
        static public TcpListener listener;
        static TcpClient client = new TcpClient();
        static NetworkStream s_str;
        
        public static Form1 form1;
        public static Form2 form2;

        [STAThread]
        static void Main()
        {
            listener = new TcpListener(IPAddress.Any, port: 2004);
            listener.Start();
            Thread thr = new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                GetData();
            });
            thr.Start();

            Data.team1 = "Валенсия";
            Data.team2 = "Локомотив";
            Data.model = new Model(Data.team1, Data.team2);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            form1 = new Form1();
            form2 = new Form2();
            form1.Text = Data.team1 + " (красные), " + Data.team2 + " (синие)";
            form2.Show();
            Application.Run(form1);
        }

        static public void GetData()
        {
            while (true)
            {
                if (listener.Pending())
                {
                    client = listener.AcceptTcpClient();
                    s_str = client.GetStream();
                }
                else if (!listener.Pending())
                {
                    while (Data.server_mode && client.Connected)
                    {
                        s_str.Write(new byte[] { 0 }, 0, 1);
                        int length = s_str.Read(s_raw, 0, s_raw.Length);
                        int sign = int.Parse(Encoding.Default.GetString(s_raw, 0, length));
                        s_str.Flush();
                        if (sign == 0)
                        {
                            length = s_str.Read(s_raw, 0, s_raw.Length);
                            string superstring = Encoding.Default.GetString(s_raw, 0, length);
                            string[] subsuperstring = superstring.Split(' ');
                            float time = float.Parse(subsuperstring[0]);
                            int ball_x = int.Parse(subsuperstring[1]);
                            int ball_y = int.Parse(subsuperstring[2]);
                            int player_count = int.Parse(subsuperstring[3]);
                            foreach (var control in Program.form2.Controls)
                            {
                                if (control is TextBox txt)
                                {
                                    if (txt.Name == "textBox1")
                                    {
                                        txt.Invoke((MethodInvoker)delegate
                                        {
                                            txt.Text = time.ToString();
                                        });
                                    }
                                }
                            }
                            for (int i = 1; i <= player_count; i++)
                            {
                                string team = subsuperstring[i * 4];
                                int num = int.Parse(subsuperstring[i * 4 + 1]);
                                int x = int.Parse(subsuperstring[i * 4 + 2]);
                                int y = int.Parse(subsuperstring[i * 4 + 3]);
                                foreach (MoveObject mo in Data.list)
                                {
                                    int curr_point_x;
                                    int curr_point_y;
                                    int deltax;
                                    int deltay;
                                    if (mo is Player po)
                                    {
                                        if (po.team == team && po.num == num)
                                        {
                                            curr_point_x = x;
                                            curr_point_y = y;
                                            deltax = curr_point_x - po.x;
                                            deltay = curr_point_y - po.y;
                                            po.x = curr_point_x;
                                            po.y = curr_point_y;
                                            po.Move(deltax, deltay);
                                            //po.path.Transform(new Matrix(1, 0, 0, 1, deltax, deltay));
                                            //po.name.Transform(new Matrix(1, 0, 0, 1, deltax, deltay));
                                        }
                                    }
                                }
                            }
                            foreach (MoveObject mo in Data.list)
                            {
                                int curr_point_x;
                                int curr_point_y;
                                int deltax;
                                int deltay;
                                if (mo is Ball bo)
                                {
                                    curr_point_x = ball_x;
                                    curr_point_y = ball_y;
                                    deltax = curr_point_x - bo.x;
                                    deltay = curr_point_y - bo.y;
                                    bo.x = curr_point_x;
                                    bo.y = curr_point_y;
                                    bo.Move(deltax, deltay);
                                    //bo.path.Transform(new Matrix(1, 0, 0, 1, deltax, deltay));
                                }
                            }
                            form2.fix_moment.Invoke((MethodInvoker)delegate
                            {
                                form2.fix_moment.PerformClick();
                            });
                            form2.Invalidate();
                            form1.Invalidate();
                            s_str.Write(new byte[] { 0 }, 0, 1);
                        }
                        else
                        {
                            Data.server_mode = false;
                            MessageBox.Show("Клиент прекратил передачу данных.");
                        }
                    }
                }
                Thread.Sleep(1);
            }
        }
    }
}

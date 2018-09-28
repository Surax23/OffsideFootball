using System;
using System.Drawing;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        MoveObject currObj; // Объект, который в данный момент перемещается
        Point oldPoint, objDelta;
        Bitmap bmp;

        public Form1()
        {
            InitializeComponent();
            
            bmp = new Bitmap(this.ClientSize.Width, this.ClientSize.Height);
            //Заполнение списка объектами для прорисовки
            Init();
            //Обновление битмапа
            RefreshBitmap();
            DoubleBuffered = true;
            MouseDown += Form1_MouseDown;
            MouseUp += Form1_MouseUp;
            MouseMove += Form1_MouseMove;
            Paint += Form1_Paint;
        }

        void Form1_Paint(object sender, PaintEventArgs e)
        {
            if (bmp == null) return;
            RefreshBitmap();
            e.Graphics.DrawImage(bmp, 0, 0);
        }

        void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    //Считаем смещение курсора
                    int deltaX, deltaY;
                    deltaX = e.Location.X - oldPoint.X;
                    deltaY = e.Location.Y - oldPoint.Y;
                    //Смещаем нарисованный объект
                    if (currObj != null)
                    {
                        currObj.Move(deltaX, deltaY);
                        
                    } else if (currObj is Player tmp)
                    {
                        tmp.Move(deltaX, deltaY);
                    }
                    //Запоминаем новое положение курсора
                    oldPoint = e.Location;
                    
                    break;
                default:
                    break;
            }
            //Обновляем форму
            this.Refresh();
        }

        void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            //currObj.Pen.Width -= 1;//Возвращаем ширину пера
            if (currObj != null)
            {
                currObj.x = oldPoint.X - objDelta.X;
                currObj.y = oldPoint.Y - objDelta.Y;
                //MessageBox.Show(currObj.Team);
                for (var i = 0; i < Data.list.Count; i++)
                {
                    if (currObj == Data.list[i])
                    {
                        Data.list[i] = currObj;
                        currObj = null;
                        return;
                    }
                }
            }
            return;
        }

        void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            // Запоминаем положение курсора
            oldPoint = e.Location;
            // Ищем объект, в который попала точка •. Если таких несколько, то найден будет первый по списку
            foreach (MoveObject po in Data.list)
            {
                if (po.path.GetBounds().Contains(e.Location))
                {
                    currObj = po;// Запоминаем найденный объект
                    objDelta.X = e.Location.X - currObj.x;
                    objDelta.Y = e.Location.Y - currObj.y;
                    return;
                }
            }
        }

        public void RefreshBitmap()
        {
            if (bmp != null) bmp.Dispose();
            bmp = new Bitmap(this.ClientSize.Width, this.ClientSize.Height);
            //Прорисовка всех объектов из списка
            using (Graphics g = Graphics.FromImage(bmp))
            {
                foreach (MoveObject po in Data.list)
                {
                    g.FillPath(po.brush, po.path);
                    g.DrawPath(po.pen, po.path);
                    Player tmp = po as Player;
                    if (tmp!=null)
                    {
                        g.DrawPath(tmp.pen, tmp.name);
                    }
                    Ball tmp2 = po as Ball;
                    if (tmp2 != null)
                    {
                        Data.ball = tmp2.Clone() as Ball;
                    }
                }
                if (Data.offside_moment_num[0] > 0)
                {
                    g.DrawLine(new Pen(Color.FromArgb(232, 213, 0)), new Point(Data.model.moment[Data.offside_moment_num[0]].offside_x, 0), new Point(Data.model.moment[Data.offside_moment_num[0]].offside_x, ClientSize.Height));
                    foreach (Player po in Data.model.moment[Data.offside_moment_num[0]].players)
                    {
                        if (Data.model.moment[Data.offside_moment_num[1]].has_ball.num == po.num &&
                            Data.model.moment[Data.offside_moment_num[1]].has_ball.team == po.team)
                        {
                            g.FillEllipse(Brushes.Yellow, new Rectangle(new Point(po.x - 7, po.y - 7), new Size(14, 14)));
                        }
                    }
                }
            }
        }

        void Init()
        {
            Player po;
            int w = this.ClientSize.Width;
            // Расстановка команды "красных"
            po = new Player(new SolidBrush(Color.Tomato), Data.team1, 1, 40, 250);
            po.goalkeeper = true;
            Data.list.Add(po);
            po = new Player(new SolidBrush(Color.Tomato), Data.team1, 2, 100, 170);
            Data.list.Add(po);
            po = new Player(new SolidBrush(Color.Tomato), Data.team1, 3, 100, 340);
            Data.list.Add(po);
            po = new Player(new SolidBrush(Color.Tomato), Data.team1, 4, 250, 170);
            Data.list.Add(po);
            po = new Player(new SolidBrush(Color.Tomato), Data.team1, 5, 250, 340);
            Data.list.Add(po);
            // Расстановка команды "оранжевых"
            po = new Player(new SolidBrush(Color.Teal), Data.team2, 1, w - 40, 250);
            po.goalkeeper = true;
            Data.list.Add(po);
            po = new Player(new SolidBrush(Color.Teal), Data.team2, 2, w - 100, 170);
            Data.list.Add(po);
            po = new Player(new SolidBrush(Color.Teal), Data.team2, 3, w - 100, 340);
            Data.list.Add(po);
            po = new Player(new SolidBrush(Color.Teal), Data.team2, 4, w - 250, 170);
            Data.list.Add(po);
            po = new Player(new SolidBrush(Color.Teal), Data.team2, 5, w - 250, 340);
            Data.list.Add(po);
            // Мяч
            Ball b = new Ball(new SolidBrush(Color.White), ClientSize.Width / 2 + 3, ClientSize.Height / 2 + 3);
            Data.list.Add(b);
            Data.ball = b;
        }
        
        private void Form1_Activated(object sender, EventArgs e)
        {
            RefreshBitmap();
        }
    }
}
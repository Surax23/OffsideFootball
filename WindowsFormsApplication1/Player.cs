using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace WindowsFormsApplication1
{
    public class MoveObject : ICloneable
    {
        public GraphicsPath path; // Графика для точки игрока
        public int x, y, w; // Положение объекта в пространстве
        public Pen pen; // Установка пера
        public Brush brush; // Установка кисти

        public MoveObject(Brush brush, int x, int y)
        {
            w = 8;
            this.x = x;
            this.y = y;
            this.path = new GraphicsPath();
            this.pen = new Pen(Color.Black);
            this.brush = brush;
            this.path.AddEllipse(Rectangle.FromLTRB(x - w, y - w, x + w, y + w));
        }

        public virtual void Move(int deltax, int deltay)
        {
            //path.Transform(new Matrix(1, 0, 0, 1, deltax, deltay));
        }

        public object Clone()
        {
            MoveObject mo = new MoveObject(this.brush, this.x, this.y);
            mo.path = this.path.Clone() as GraphicsPath;
            return mo;
        }
    }

    public class Ball : MoveObject
    {
        public Ball(Brush brush, int x, int y) : base(brush, x, y)
        {
        }

        public override void Move(int deltax, int deltay)
        {
            path.Transform(new Matrix(1, 0, 0, 1, deltax, deltay));
        }

        public object Clone()
        {
            return new Ball(brush, x, y);
        }
    }

    public class Player : MoveObject
    {
        public GraphicsPath name; // Графика для номера
        public string team; // Имя команды
        public int num; // Номер в команде
        public bool have_ball; // Имеет ли игрок мяч
        public bool goalkeeper = false; // Является ли голкипером

        public Player(Brush brush, string team, int num, int x, int y) : base(brush, x, y)
        {
            w = 13;
            this.team = team;
            this.num = num;
            path = new GraphicsPath();
            name = new GraphicsPath();
            pen = new Pen(Color.Black);
            have_ball = false;
            path.AddEllipse(Rectangle.FromLTRB(x - w, y - w, x + w, y + w));
            name.AddString(Convert.ToString(num), new FontFamily("Courier New"), 0, 22f, new Point((x - w) - 2, (y - w) - 26), StringFormat.GenericDefault);
            name.AddEllipse(Rectangle.FromLTRB(x - (w + 5), y - (w + 5), x + (w + 5), y + (w + 5)));
        }

        public override void Move(int deltax, int deltay)
        {
            path.Transform(new Matrix(1, 0, 0, 1, deltax, deltay));
            name.Transform(new Matrix(1, 0, 0, 1, deltax, deltay));
        }

        public object Clone()
        {
            Player p = new Player(brush, team, num, x, y);
            p.have_ball = have_ball;
            p.goalkeeper = goalkeeper;
            p.name = name.Clone() as GraphicsPath;
            return p;
        }
    }


}
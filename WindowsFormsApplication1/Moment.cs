using System;
using System.Collections.Generic;
using System.Linq;

namespace WindowsFormsApplication1
{
    class Moment
    {
        public List<Player> players = new List<Player>();
        public Ball ball;
        public Player has_ball;
        public float time;
        public string name_attack;
        public List<Player> offside = new List<Player>();
        public int offside_x;
        public Player last_stand;
        public bool was_offside = false;
        string team1, team2;

        public Moment(float time_t, List<MoveObject> list, Ball ball_t, string team1_t, string team2_t)
        {
            team1 = team1_t;
            team2 = team2_t;
            time = time_t;
            ball = (Ball)ball_t.Clone();
            foreach (MoveObject mv in list)
            {
                if (mv is Player tmp)
                {
                    players.Add((Player)tmp.Clone());
                }
            }
            has_ball = where_ball();
            offside_players();
        }

        public Player where_ball() // У какого игрока мяч?
        {
            int k = -1;
            double min = 26;
            for (var i = 0; i < players.Count; i++)
            {
                if (min > Math.Sqrt(Math.Pow(ball.x - players[i].x, 2) + Math.Pow(ball.y - players[i].y, 2)))
                {
                    k = i;
                    min = Math.Sqrt(Math.Pow(ball.x - players[i].x, 2) + Math.Pow(ball.y - players[i].y, 2));
                }
            }
            for (int i = 0; i < players.Count; i++)
            {
                players[i].have_ball = false;
            }
            if (k != -1)
            {
                players[k].have_ball = true;
                name_attack = players[k].team;
                // А тут еще небольшая проверка, был ли этот игрок последним в защите.
                List<Player> temp = players.Select(po => (Player)po.Clone()).ToList();
                if (players[k].team == team1) // Для команды 1
                {
                    temp.Sort((x, y) => x.x.CompareTo(y.x));
                    foreach (Player po in temp)
                    {
                        if (po.team == team1)
                        {
                            last_stand = po;
                            break;
                        }

                    }
                }
                if (players[k].team == team2) // Для команды 2
                {
                    temp.Sort((x, y) => x.x.CompareTo(y.x));
                    temp.Reverse();
                    foreach (Player po in temp)
                    {
                        if (po.team == team2)
                        {
                            last_stand = po;
                            break;
                        }
                    }
                }
                return players[k] as Player;
            }
            else
            {
                return null;
            }
        }

        public void offside_players()
        {
            players.Sort((x, y) => x.x.CompareTo(y.x));
            // Ищем оффсайдников, если атакующая "команда 1"
            if (name_attack == team1)
            {
                bool flag = false;
                foreach (Player po in players.Reverse<Player>()) // Устанавливаем линию оффсайда
                {
                    if (po.team == team2 && flag)
                    {
                        if (ball.x < po.x)
                        {
                            offside_x = po.x + po.w;
                        }
                        else
                        {
                            offside_x = ball.x;
                        }
                        if (offside_x <= Program.form1.ClientSize.Width/2)
                        {
                            offside_x = Program.form1.ClientSize.Width / 2;
                        }
                        break;
                    }
                    else
                    {
                        flag = true;
                    }
                }
                foreach (Player po in players) // Добавляем игроков в лист оффсайдников
                {
                    if (po.team == team1 && (po.x + po.w) > offside_x)
                    {
                        offside.Add(po);
                    }
                }
            }
            else // Игроки в оффсайде при атакующей "команде 2"
            {
                bool flag = false;
                foreach (Player po in players) // Устанавливаем линию оффсайда
                {
                    if (po.team == team1 && flag)
                    {
                        if (ball.x > po.x)
                        {
                            offside_x = po.x - po.w;
                        }
                        else
                        {
                            offside_x = ball.x;
                        }
                        if (offside_x >= Program.form1.ClientSize.Width / 2)
                        {
                            offside_x = Program.form1.ClientSize.Width / 2;
                        }
                        break;
                    }
                    else
                    {
                        flag = true;
                    }
                }
                foreach (Player po in players) // Добавляем игроков в лист оффсайдников
                {
                    if (po.team == team2 && (po.x - po.w) < offside_x)
                    {
                        offside.Add(po);
                    }
                }
            }
        }
    }
}
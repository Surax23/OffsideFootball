using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    class Model
    {
        private float time;
        public List<Moment> moment = new List<Moment>();
        public int curr_moment;
        public string team1, team2;

        public Model(string tteam1, string tteam2)
        {
            team1 = tteam1;
            team2 = tteam2;
            time = 0;
            curr_moment = 0;
            moment.Add(null);
        }

        public void del_moment(int num)
        {
            moment.RemoveAt(num);
            curr_moment -= 1;
        }

        public int[] add_moment(float time_t, List<MoveObject> list, Ball ball_t)
        {
            curr_moment += 1;
            moment.Add(new Moment(time_t, list, ball_t, team1, team2));
            if (curr_moment > 1)
            {
                if (moment[curr_moment].has_ball != null && time_t > moment[curr_moment - 1].time)
                {
                    time = time_t;
                    return checkout();
                }
                else
                {
                    moment.RemoveAt(curr_moment);
                    curr_moment -= 1;
                    return new int[2] { -1, -1 };
                }
            }
            else
            {
                if (moment[curr_moment].has_ball != null)
                {
                    time = time_t;
                    return checkout();
                }
                else
                {
                    moment.RemoveAt(curr_moment);
                    curr_moment -= 1;
                    return new int[2] { -1, -1 };
                }
            }
        }

        public int[] checkout()
        {
            if (curr_moment - 1 >= 1)
            {
                if (moment[curr_moment].has_ball.team == moment[curr_moment - 1].has_ball.team &&
                    moment[curr_moment].has_ball.num != moment[curr_moment - 1].has_ball.num &&
                    moment[curr_moment - 1].offside.Count > 0)
                {
                    foreach (Player po in moment[curr_moment - 1].offside)
                    {
                        if (po.team == moment[curr_moment].has_ball.team &&
                       po.num == moment[curr_moment].has_ball.num)
                        {
                            clean_off(curr_moment - 1, curr_moment);
                            return new int[2] { curr_moment - 1, curr_moment };
                        }
                    }
                } else if (moment[curr_moment].has_ball.team == moment[curr_moment - 1].has_ball.team &&
                    moment[curr_moment].has_ball.num != moment[curr_moment - 1].has_ball.num)
                {
                    return new int[2] { 0, 0 };
                }
            }
            // Проверка для n-2
            if (curr_moment - 2 >= 1)
            {
                if (moment[curr_moment].has_ball.team == moment[curr_moment - 2].has_ball.team &&
                    moment[curr_moment].has_ball.num != moment[curr_moment - 2].has_ball.num &&
                    moment[curr_moment - 2].offside.Count > 0)
                {
                    foreach (Player po in moment[curr_moment - 2].offside)
                    {
                        if (po.team == moment[curr_moment].has_ball.team &&
                            po.num == moment[curr_moment].has_ball.num)
                        {
                            if (moment[curr_moment].has_ball.team != moment[curr_moment - 1].last_stand.team
                                && moment[curr_moment - 1].last_stand.num == moment[curr_moment - 1].has_ball.num
                                && moment[curr_moment - 1].last_stand.team == moment[curr_moment - 1].has_ball.team)
                            {
                                clean_off(curr_moment - 2, curr_moment);
                                return new int[2] { curr_moment - 2, curr_moment };
                            }
                            else
                            {
                                if (Math.Abs(moment[curr_moment - 2].time - moment[curr_moment - 1].time) < 0.45f)
                                {
                                    clean_off(curr_moment - 2, curr_moment);
                                    return new int[2] { curr_moment - 2, curr_moment };
                                }
                                else
                                {
                                    if (dirt_off(curr_moment - 2, curr_moment))
                                    {
                                        return new int[2] { curr_moment - 2, curr_moment };
                                    } else
                                    {
                                        return new int[2] { 0, 0 };
                                    }
                                }
                            }
                        }
                    }
                } else if (moment[curr_moment].has_ball.team == moment[curr_moment - 2].has_ball.team &&
                 moment[curr_moment].has_ball.num != moment[curr_moment - 2].has_ball.num)
                {
                    return new int[2] { 0, 0 };
                }
            }
            // Проверка для n-3
            if (curr_moment - 3 >= 1)
            {
                if (moment[curr_moment].has_ball.team == moment[curr_moment - 3].has_ball.team &&
                    moment[curr_moment].has_ball.num != moment[curr_moment - 3].has_ball.num &&
                    moment[curr_moment - 3].offside.Count > 0)
                {
                    foreach (Player po in moment[curr_moment - 3].offside)
                    {
                        if (po.team == moment[curr_moment].has_ball.team &&
                            po.num == moment[curr_moment].has_ball.num)
                        {
                            if (Math.Abs(moment[curr_moment - 3].time - moment[curr_moment - 2].time) < 0.45f &&
                               Math.Abs(moment[curr_moment - 2].time - moment[curr_moment - 1].time) < 0.45f)
                            {
                                if (dirt_off(curr_moment - 3, curr_moment))
                                {
                                    return new int[2] { curr_moment - 3, curr_moment };
                                } else
                                {
                                    return new int[2] { 0, 0 };
                                }
                            }
                        }
                    }
                } else if (moment[curr_moment].has_ball.team == moment[curr_moment - 3].has_ball.team &&
                 moment[curr_moment].has_ball.num != moment[curr_moment - 3].has_ball.num)
                {
                    return new int[2] { 0, 0 };
                }
            }
            // Проверка для n-4
            if (curr_moment - 4 >= 1)
            {
                if (moment[curr_moment].has_ball.team == moment[curr_moment - 4].has_ball.team &&
                    moment[curr_moment].has_ball.num != moment[curr_moment - 4].has_ball.num &&
                    moment[curr_moment - 4].offside.Count > 0)
                {
                    foreach (Player po in moment[curr_moment - 4].offside)
                    {
                        if (po.team == moment[curr_moment].has_ball.team &&
                            po.num == moment[curr_moment].has_ball.num)
                        {
                            if (Math.Abs(moment[curr_moment - 4].time - moment[curr_moment - 3].time) < 0.45f &&
                               Math.Abs(moment[curr_moment - 3].time - moment[curr_moment - 2].time) < 0.45f &&
                               Math.Abs(moment[curr_moment - 2].time - moment[curr_moment - 1].time) < 0.45f)
                            {
                                if (dirt_off(curr_moment - 4, curr_moment))
                                {
                                    return new int[2] { curr_moment - 4, curr_moment };
                                } else
                                {
                                    return new int[2] { 0, 0 };
                                }
                            }
                        }
                    }
                }
            }
            return new int[2] { 0, 0 };
        }

        public bool dirt_off(int first, int second)
        {
            if (MessageBox.Show("Оффсайд на усмотрение судьи на снимках " + first.ToString() + " и " + second.ToString() + "\r\n" + "Посчитать за оффсайд?", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                moment[first].was_offside = true;
                return true;
            }
            return false;
        }

        public void clean_off(int first, int second)
        {
            MessageBox.Show("Чистый оффсайд на снимках " + first.ToString() + " и " + second.ToString() + "\r\n", "", MessageBoxButtons.OK);
            moment[first].was_offside = true;
        }
    }
}

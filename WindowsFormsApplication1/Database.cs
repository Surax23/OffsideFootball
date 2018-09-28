using System;
using MySql.Data.MySqlClient;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    class Database
    {
        int curr_model = 0;
        string connStr = "server=localhost;user=root;database=football;password=123456;charset=utf8";
        MySqlConnection conn;
        MySqlCommand command;
        MySqlDataReader reader;
        string sql = "";


        public Database()
        {
            try
            {
                conn = new MySqlConnection(connStr);
                conn.Open();
            } catch
            {
                MessageBox.Show("Нет подключения к базе данных!");
            }
        }

        public bool Push_moments(Model model)
        {
            try
            {
                // Проверяем, нет ли текущей модели и добавляем.
                if (curr_model == 0)
                {
                    sql = "INSERT INTO model (team1,team2) VALUES('" + model.team1 + "','" + model.team2 + "');";
                    command = new MySqlCommand(sql, conn);
                    reader = command.ExecuteReader();
                    curr_model = (int)command.LastInsertedId;
                    reader.Close();
                }
                // А теперь добавляем данные, если их еще нет в таблицах.
                for (int i = 1; i < model.moment.Count; i++)
                {
                    sql = "SELECT COUNT(*) FROM moments WHERE id_model=" + curr_model + " AND time=" + model.moment[i].time.ToString(System.Globalization.CultureInfo.InvariantCulture) + ";";
                    command = new MySqlCommand(sql, conn);
                    object scalar = command.ExecuteScalar();
                    int count = Convert.ToInt32(scalar);
                    if (count < 1)
                    {
                        sql = "INSERT INTO moments (id_model,time) VALUES(" + curr_model + "," + model.moment[i].time.ToString(System.Globalization.CultureInfo.InvariantCulture) + ");";
                        command = new MySqlCommand(sql, conn);
                        reader = command.ExecuteReader();
                        int id_moments = (int)command.LastInsertedId;
                        reader.Close();
                        foreach (Player po in model.moment[i].players)
                        {
                            sql = "INSERT INTO players (id_model,x,y,team,num,id_moments) VALUES(" + curr_model + "," + po.x + "," + po.y + ",'" + po.team + "'," + po.num + "," + id_moments + ");";
                            command = new MySqlCommand(sql, conn);
                            reader = command.ExecuteReader();
                            reader.Close();
                        }
                        sql = "INSERT INTO ball (id_model,x,y,id_moments) VALUES(" + curr_model + "," + model.moment[i].ball.x + "," + model.moment[i].ball.y + "," + id_moments + ");";
                        command = new MySqlCommand(sql, conn);
                        reader = command.ExecuteReader();
                        reader.Close();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        public void Delete_moment(Model model, float time)
        {
            if (curr_model != 0)
            {
                float l_time = time - 0.000001f;
                float r_time = time + 0.000001f;
                sql = "delete from moments where id_model = " + curr_model + " AND time > " + l_time.ToString(System.Globalization.CultureInfo.InvariantCulture) + " AND time < " + r_time.ToString(System.Globalization.CultureInfo.InvariantCulture) + ";";
                command = new MySqlCommand(sql, conn);
                reader = command.ExecuteReader();
                reader.Close();
            }
        }

        public void Close_connection()
        {
            conn.Close();
        }
    }
}

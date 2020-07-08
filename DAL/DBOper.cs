using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Data;
using System.Configuration;

namespace DAL
{
    /// <summary>
    /// 这个层是用来和数据库连接的最底层
    /// </summary>
    public class DBOper
    {
        
        //数据库连接实例
        private static MySqlConnection conn;

        //数据库的连接方法
        private static MySqlConnection Connection
        {
            get
            {
                if (conn == null)
                {
                    string constr = ConfigurationManager.ConnectionStrings["database_mysql"].ConnectionString;
                    conn = new MySqlConnection(constr);
                }
                return conn;
            }
        }

        public static void CloseReader()
        {
            reader?.Close();
            Connection.Close();
        }

        public static void CloseMySqlConnection()
        {
            //if (conn.State == ConnectionState.Open)
                conn?.Close();
            Connection.Dispose();
        }

        public static int ExecuteCommand(string sql, params MySqlParameter[] values)
        {
            //Connection.Close();
            try
            {
                Connection.Open();
            }
            catch
            {

            }
            int affectedRows=0;
            using (MySqlCommand cmd = new MySqlCommand(sql, Connection))
            {
                cmd.Parameters.AddRange(values);
                affectedRows = cmd.ExecuteNonQuery();
            }
            Connection.Close();
            return affectedRows;
        }
        public static MySqlDataReader GetReader(string sql)
        {
            Connection.Open();
            MySqlCommand cmd = new MySqlCommand(sql, Connection);
            reader = cmd.ExecuteReader();
            return reader;
        }
        private static MySqlDataReader reader;
        public static MySqlDataReader GetReader(string sql, params MySqlParameter[] values)
        {
            Connection.Open();
            MySqlCommand cmd = new MySqlCommand(sql, Connection);
            cmd.Parameters.AddRange(values);
            reader = cmd.ExecuteReader();
            return reader;
        }
        public static DataTable GetDataTable(string sql)
        {
            //只能获取一个datatable;
            MySqlDataAdapter da = new MySqlDataAdapter(sql, Connection);
            DataSet ds = new DataSet();
            da.Fill(ds);
            return ds.Tables[0];
        }
        
        public static DataTable GetDataTable(string sql,params MySqlParameter[] values)
        {
            Connection.Open();
            MySqlCommand cmd = new MySqlCommand(sql,Connection);
            cmd.Parameters.AddRange(values);
            MySqlDataAdapter da = new MySqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            DataTable dt = ds.Tables[0].Copy();
            da.Dispose();
            ds.Dispose();
            Connection.Close();
            return dt;
        }

    }
}

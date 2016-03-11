using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace chopper1.Models
{
    public class planCatDb
    {
        //variable connection
        protected SqlConnection con;
        
        //open connection
        public bool Open(string Connection = "PlanCatConnection")
        {
            //rewrite web.config
            con = new SqlConnection(@WebConfigurationManager.ConnectionStrings[Connection].ToString());
            try
            {
                bool b = true;
                if (con.State.ToString() != "Open")
                {
                    con.Open();
                }
                return b;
            }
            catch (SqlException ex)
            {
                return false;
            }
        }

        //close connection
        public bool Close()
        {
            try
            {
                con.Close();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        //
        public string getDataTest(string sql)
        {
            int LastID = 0;
            string query = sql + ";SELECT @@Identity;";
            sql = "заговор диетологов";
            string select = "SELECT BCDate, BeginTimeText, TimingText, Title, Bit_Repetition, DSti, DM, DR, ProducerCode, SellerCode " +
                            "FROM TCBCList4Themes " +
                            "WHERE Title like '%"+sql+"%'";
            string result = "";
            try
            {
                if (con.State.ToString() == "Open")
                {
                    SqlCommand cmd = new SqlCommand(select, con);
                    result = cmd.ExecuteScalar().ToString();                    
                }
            }
            catch
            {

            }
            return result;                      
        }

        public DataTable getDataTest2(string sql)
        {
            int LastID = 0;
            string query = sql + ";SELECT @@Identity;";
            //sql = "заговор диетологов";
            string select = "SELECT BCDate, BeginTimeText, TimingText, Title, Bit_Repetition, DSti, DM, DR, ProducerCode, SellerCode " +
                            "FROM TCBCList4Themes " +
                            "WHERE Title like '%" + sql + "%'";
            DataTable result = new DataTable();

            try
            {
                if (con.State.ToString() == "Open")
                {
                    SqlCommand cmd = new SqlCommand(select, con);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                }
            }
            catch
            {

            }
            return result;
        }

    }
}
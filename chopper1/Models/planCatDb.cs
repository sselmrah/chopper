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
        //public bool Open(string Connection = "TSurfaceCatConnection")
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

        public DataTable getDataTest2(string title)
        {
            int LastID = 0;
            string query = title + ";SELECT @@Identity;";
            //sql = "заговор диетологов";
            string newSti = "DSti = CASE WHEN DSTI is null and DSTIstr is not null THEN DSTIstr ELSE CONVERT(varchar, DSTI) END";
            string newDM = "DM = CASE WHEN DM is null and DMstr is not null THEN DMstr ELSE CONVERT(varchar, DM) END";
            string newDR = "DR = CASE WHEN DR is null and DRstr is not null THEN DRstr ELSE CONVERT(varchar, DR) END";
            string select = "SELECT TimingText, Title, Bit_Repetition, CONVERT(VARCHAR(10), BCDate, 104) as strDate, datename(dw,BCDate) as DoW, BeginTimeText," + newSti + "," + newDM + "," + newDR + ", ProducerCode, SellerCode " +
                               "FROM TCBCList4Themes " +
                               "WHERE Title like '%" +
                               title +
                               "%'";

            /*
            string select = "SELECT BCDate, BeginTimeText, TimingText, Title, Bit_Repetition, DSti, DM, DR, ProducerCode, SellerCode " +
                            "FROM TCBCList4Themes " +
                            "WHERE Title like '%" + sql + "%'";
             */
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

        public DataTable getProdList()
        {            
            string select = "SELECT CAST(ProducerCode as varchar) + ' - ' + CAST(ProducerTitle as varchar) + ' ('+ cast(ProducerName as varchar)+')' FROM dbo.ProducerDict ORDER BY ProducerOrder";
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

        public DataTable advSearch(string Title = "", DateTime? DateMin = null, DateTime? DateMax = null, DateTime? TimeStartMin = null, DateTime? TimeStartMax = null, DateTime? TimingMin = null, DateTime? TimingMax = null, bool Monday = true, bool Tuesday = true, bool Wednesday = true, bool Thursday = true, bool Friday = true, bool Saturday = true, bool Sunday = true, List<string> Producers = null, bool orig = true, bool rep = true)
        {
            string title = Title;
            string sqlTitle = produceSqlTitleSearchString(Title);
            string dateMin = Convert.ToDateTime(DateMin).ToString("yyyy-MM-dd");
            if (dateMin == "0001-01-01") dateMin = "2005-01-01";
            string dateMax = Convert.ToDateTime(DateMax).ToString("yyyy-MM-dd");
            if (dateMax == "0001-01-01") dateMax = DateTime.Now.ToString("yyyy-MM-dd");
            double timeStartDbl = Convert.ToDateTime(TimeStartMin).Hour * 60 * 60+ Convert.ToDateTime(TimeStartMin).Minute * 60;
            if (timeStartDbl == 0) timeStartDbl = 0;
            double timeEndDbl = Convert.ToDateTime(TimeStartMax).Hour * 60 * 60+ Convert.ToDateTime(TimeStartMax).Minute * 60;
            if (timeEndDbl == 0) timeEndDbl = 23 * 60 * 60 + 59 * 60;
            double timingMinDbl = Convert.ToDateTime(TimingMin).Hour * 60 * 60 + Convert.ToDateTime(TimingMin).Minute * 60;
            if (timingMinDbl == 0) timingMinDbl = 1 * 60;
            double timingMaxDbl = Convert.ToDateTime(TimingMax).Hour * 60 * 60 + Convert.ToDateTime(TimingMax).Minute * 60;
            if (timingMaxDbl == 0) timingMaxDbl = 10 * 60 * 60;

            string prodStr = "";
            if (Producers != null)
            {
                prodStr += " AND (";
                if (Producers.Count()>0)
                {                    
                    foreach (string s in Producers)
                    {
                        prodStr += "ProducerCode='" + s.Left(s.IndexOf("-") - 1)+"' OR ";
                    }
                    prodStr = prodStr.Left(prodStr.Length - 4);
                }                
                prodStr += ")";
            }

            string dowStr = "";
            if (Monday|Tuesday|Wednesday|Thursday|Friday|Saturday|Sunday &! (Monday&Tuesday&Wednesday&Thursday&Friday&Saturday&Sunday))
            {
                dowStr += " AND (";
                if (Monday) dowStr += "datename(dw,BCDate)='Monday'";
                if (dowStr.Length > 6) dowStr += " OR ";
                if (Tuesday) dowStr += "datename(dw,BCDate)='Tuesday'";
                if (dowStr.Length > 6) dowStr += " OR ";
                if (Wednesday) dowStr += "datename(dw,BCDate)='Wednesday'";
                if (dowStr.Length > 6) dowStr += " OR ";
                if (Thursday) dowStr += "datename(dw,BCDate)='Thursday'";
                if (dowStr.Length > 6) dowStr += " OR ";
                if (Friday) dowStr += "datename(dw,BCDate)='Friday'";
                if (dowStr.Length > 6) dowStr += " OR ";
                if (Saturday) dowStr += "datename(dw,BCDate)='Saturday'";
                if (dowStr.Length > 6) dowStr += " OR ";
                if (Sunday) dowStr += "datename(dw,BCDate)='Sunday'";
                dowStr += ")";
            }

            string repStr = "";
            if (orig & !rep) repStr = " AND Bit_Repetition = 'false'";
            if (!orig & rep) repStr = " AND Bit_Repetition = 'true'";

            string dateStr = " AND BCDate >= '"+dateMin+"' AND BCDate <='" +dateMax+"'";
            string timingStr = " AND Timing >="+timingMinDbl.ToString()+" AND Timing<="+timingMaxDbl;
            string timestartStr = " AND BeginTime >=" + timeStartDbl.ToString() + " AND BeginTime<=" + timeEndDbl;



            string newSti = "DSti = CASE WHEN DSTI is null and DSTIstr is not null THEN DSTIstr ELSE CONVERT(varchar, DSTI) END";
            string newDM = "DM = CASE WHEN DM is null and DMstr is not null THEN DMstr ELSE CONVERT(varchar, DM) END";
            string newDR = "DR = CASE WHEN DR is null and DRstr is not null THEN DRstr ELSE CONVERT(varchar, DR) END";
            string select = "SELECT TimingText, Title, Bit_Repetition, CONVERT(VARCHAR(10), BCDate, 104) as strDate, datename(dw,BCDate) as DoW, BeginTimeText," + newSti + "," + newDM + "," + newDR + ", ProducerCode, SellerCode " +
                               "FROM TCBCList4Themes " +
                               "WHERE "
                               + sqlTitle                               
                               + prodStr 
                               + dowStr 
                               + dateStr 
                               + timingStr 
                               + timestartStr
                               + repStr
                               ;
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

        public string produceSqlTitleSearchString(string origTitle)
        {
            //Работаем с ключами "&, |, ^"
            //Скобки пока не поддерживаются

            string sqlTitle = "";

            sqlTitle += "Title like '%";

            //Убираем служебные символы с концов строки
            if (origTitle.Contains("^"))
            {
                if (origTitle.IndexOf("^")==0)
                {
                    origTitle = origTitle.Substring(1, origTitle.Length - 1);
                    //Если начинается на ^, меняем начало строки условия
                    sqlTitle = "Title NOT like '%";
                }
                if (origTitle.IndexOf("^")==origTitle.Length-1)
                {
                    origTitle = origTitle.Substring(0, origTitle.Length - 1);
                }
            }
            if (origTitle.Contains("|"))
            {
                if (origTitle.IndexOf("|") == 0)
                {
                    origTitle = origTitle.Substring(1, origTitle.Length - 1);
                }
                if (origTitle.IndexOf("|") == origTitle.Length - 1)
                {
                    origTitle = origTitle.Substring(0, origTitle.Length - 1);
                }
            }
            if (origTitle.Contains("&"))
            {
                if (origTitle.IndexOf("&") == 0)
                {
                    origTitle = origTitle.Substring(1, origTitle.Length - 1);
                }
                if (origTitle.IndexOf("&")==origTitle.Length-1)
                {
                    origTitle = origTitle.Substring(0, origTitle.Length - 1);
                }
            }
            

            if (origTitle.Contains("&")) origTitle = origTitle.Replace("&", "%' AND Title like '%");
            if (origTitle.Contains("|")) origTitle = origTitle.Replace("|", "%' OR Title like '%");
            if (origTitle.Contains("^")) origTitle = origTitle.Replace("^", "%' AND Title NOT like '%");
                       
            sqlTitle += origTitle;
            sqlTitle += "%'";

            return sqlTitle;
        }

    }
}
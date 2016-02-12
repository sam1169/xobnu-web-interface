using System;
using System.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace Xobnu.WebUI.Util
{
    public class DBHelper
    {
        internal static SqlConnection _DBConn;

        private bool OpenDBConnection(string connectionString)
        {
            try
            {
                _DBConn = new SqlConnection(connectionString);
                if (_DBConn.State == ConnectionState.Closed)
                {
                    _DBConn.Open();
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private bool CloseDBConnection()
        {
            try
            {
                if (_DBConn.State == ConnectionState.Open)
                {
                    _DBConn.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public DataSet GetDataSet(string sql, string connectionString)
        {
            try
            {
                if (OpenDBConnection(connectionString))
                {
                    SqlCommand cmd;
                    DataSet ds = new DataSet();
                    cmd = new SqlCommand(sql, _DBConn);
                    cmd.CommandTimeout = 60;
                    SqlDataAdapter sqlDA = new SqlDataAdapter(cmd);
                    sqlDA.Fill(ds, "dataset");
                    return ds;
                }
                else return null;
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                CloseDBConnection();
            }
        }



    }
}
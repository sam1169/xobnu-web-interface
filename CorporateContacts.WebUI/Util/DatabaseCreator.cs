using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using Microsoft.SqlServer.Smo;
using Microsoft.SqlServer.Management.Smo;

namespace Xobnu.WebUI.Util
{
    public class DatabaseCreator
    {

        string connectionstr = "";
        static string serverConnection = ConfigurationManager.ConnectionStrings["ServerConnection"].ConnectionString;
        SqlConnection myConn = new SqlConnection(serverConnection);

        public bool CreateUserDatabase(string dbName)
        {

            connectionstr = "CREATE DATABASE " + dbName;

            SqlCommand myCommand = new SqlCommand(connectionstr, myConn);

            try
            {
                myConn.Open();
                myCommand.ExecuteNonQuery();
                return true;
            }
            catch (System.Exception ex)
            {
                return false;
            }
            finally
            {
                if (myConn.State == ConnectionState.Open)
                {
                    myConn.Close();
                }
            }

        }

        public bool CreateDataBaseUser(string connCommand, string sqlConnection)
        {
            //"CREATE LOGIN AbolrousHazem  WITH PASSWORD = '340$Uuxwp7Mcxo7Khy'"
            SqlConnection myConnection = new SqlConnection(sqlConnection);
            SqlCommand myCommand = new SqlCommand(connCommand, myConnection);
            try
            {
                myConnection.Open();
                myCommand.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                if (myConnection.State == ConnectionState.Open)
                {
                    myConnection.Close();
                }
            }
        }


    }
}
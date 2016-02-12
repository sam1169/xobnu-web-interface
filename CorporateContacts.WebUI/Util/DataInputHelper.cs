using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.IO;
//using Excel = Microsoft.Office.Interop.Excel;
using System.Reflection;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Collections;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using Excel;
using ICSharpCode;

namespace Xobnu.WebUI.Util
{

    public class DataInputHelper
    {
        string _filePath = "";
        string _fileExtension = "";
        IExcelDataReader excelReader = null;

        public DataInputHelper(string filePath, string fileExtension)
        {
            _filePath = filePath;
            _fileExtension = fileExtension;

            FileStream stream = File.Open(_filePath, FileMode.Open, FileAccess.Read);

            if (_fileExtension == ".xls")
            {
                excelReader = ExcelReaderFactory.CreateBinaryReader(stream);
            }
            else
            {
                excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            }

            
        }

        public DataTable GetImportExcel()
        {
            excelReader.IsFirstRowAsColumnNames = false;
            DataSet result = excelReader.AsDataSet();

            DataTable dt = result.Tables[0];
            //foreach (DataRow row in dt.Rows)
            //{
            //    foreach (DataColumn col in dt.Columns)
            //        Console.WriteLine(row[col]);
            //    Console.WriteLine("".PadLeft(20, '='));
            //}        

            excelReader.Close();

            return dt;
        
        }


    }
}
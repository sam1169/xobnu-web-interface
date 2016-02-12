using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CorporateContacts.Domain.Entities;

namespace CorporateContacts.WebUI.Models
{
    public class SystemAdminDashboardViewModel
    {
        public CCErrorLog ErrorLog { get; set; }
        public List<CCErrorLog> ErrorLogList { get; set; }
        public Account Account { get; set; }
        public List<Account> ExistingAccounts { get; set; }
        public long SelectedAccountID { get; set; }
        public long SelectedInformationType { get; set; }
        public long SelectedConnectionID { get; set; }
        public ConnectionCheckBoxToggles SelectedConnectionCheckBoxes { get; set; }
        public SelectedConnectionDetails SelectedConnection { get; set; }
        public List<CCFolder> Folders { get; set; }
        public List<FolderDetails> FolderList { get; set; }
        public List<CCConnection> Connections { get; set; }
        public List<ConnectionDetails> ConnectionList { get; set; }
        public List<DBTableUsageDetails> TableUsageList { get; set; }
        public List<ErrorFilters> ErrorSourceFilterList { get; set; }
        public List<ErrorFilters> ErrorTypeFilterList { get; set; }

        public long FolderCount { get; set; }
        public long ConnectionCount { get; set; }
        public long UserCount { get; set; }
        public long AccountCount { get; set; }
        public long ItemCount { get; set; }
        public List<CCErrorLog> ErrorLogHistoryLimited { get; set; }

        public SystemAdminDashboardViewModel()
        {
            //Add List of Error Filters
            ErrorSourceFilterList = new List<ErrorFilters>();
            ErrorTypeFilterList = new List<ErrorFilters>();
            
            ErrorFilters EF1 = new ErrorFilters();
            EF1.Error = "Web";
            ErrorSourceFilterList.Add(EF1);

            ErrorFilters EF2 = new ErrorFilters();
            EF2.Error = "Sync";
            ErrorSourceFilterList.Add(EF2);

            ErrorFilters EF3 = new ErrorFilters();
            EF3.Error = "Sync Error";
            ErrorTypeFilterList.Add(EF3);

            ErrorFilters EF4 = new ErrorFilters();
            EF4.Error = "Database Error";
            ErrorTypeFilterList.Add(EF4);

            ErrorFilters EF5 = new ErrorFilters();
            EF5.Error = "Database Connection Error";
            ErrorTypeFilterList.Add(EF5);

            ErrorFilters EF6 = new ErrorFilters();
            EF6.Error = "Exchange Connection Error";
            ErrorTypeFilterList.Add(EF6);

            ErrorFilters EF7 = new ErrorFilters();
            EF7.Error = "Data Conversion Error";
            ErrorTypeFilterList.Add(EF7);

            ErrorFilters EF8 = new ErrorFilters();
            EF8.Error = "Type Check Error";
            ErrorTypeFilterList.Add(EF8);

            ErrorFilters EF9 = new ErrorFilters();
            EF9.Error = "File Load Error";
            ErrorTypeFilterList.Add(EF9);

            ErrorFilters EF10 = new ErrorFilters();
            EF10.Error = "Data Error";
            ErrorTypeFilterList.Add(EF10);

        }
    }

    public class ErrorFilters
    {
        public string Error { get; set; }
        public string ErrorState { get; set; }

        public ErrorFilters()
        {
            Error = "";
            ErrorState = "active";
        }
    }

    public class FolderDetails
    {
        public string FolderName { get; set; }
        public int ItemCount { get; set; }
    }

    public class ConnectionDetails  
    {
        public CCConnection Connection { get; set; }
        public CCFolder Folders { get; set; }
    }

    public class SelectedConnectionDetails
    {
        public CCConnection Connection { get; set; }
        public CCFolder Folder { get; set; }
    }

    public class ConnectionCheckBoxToggles
    {
        public string allowAdditions { get; set; }
        public string ignoreExsisting { get; set; }
        public string categoryFilter { get; set; }
        public string copyPhotos { get; set; }
        public string turnOffReminders { get; set; }
        public string tagSubject { get; set; }
        public string isRunning { get; set; }
        public string isPaused { get; set; }

        public ConnectionCheckBoxToggles()
        { 
            allowAdditions = "unchecked";
            ignoreExsisting = "unchecked";
            categoryFilter = "unchecked";
            copyPhotos = "unchecked";
            turnOffReminders = "unchecked";
            tagSubject = "unchecked";
            isRunning = "unchecked";
            isPaused = "unchecked";
        }
    }

    public class DBTableUsageDetails
    {
        public string TableName { get; set; }
        public string RecordCount { get; set; }
        public double TableUsage { get; set; }
    
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Xobnu.WebUI.Models;
using Xobnu.Domain.Abstract;
using Xobnu.Domain.Entities;
using System.Data;
using Microsoft.Exchange.WebServices.Data;
using Folder = Microsoft.Exchange.WebServices.Data.Folder;

using Xobnu.WebUI.Util;
using Xobnu.WebUI.Infrastructure;
using System.Web.Mvc;
using System.Reflection;
using System.Configuration;
using System.IO;

namespace Xobnu.WebUI.Util
{
    public class ConnectionImporter
    {
        ICCConnectionsRepo CCConnectionRepo;
        ICCFolderRepo CCFolderRepo;
        ICredentialRepo CCCredentialRepository;
        ICCFieldMappingsRepo CCFieldMappingsRepository;

        DataInputHelper inputdatas;
        long NumberOfConnections = 0;
        List<CCConnection> ConnectionList = new List<CCConnection>();

        private string rand = "00062008-0000-0000-C000-000000000046";

        public ConnectionImporter(ICCConnectionsRepo connection, ICCFolderRepo folder, ICredentialRepo creds, ICCFieldMappingsRepo fieldMaps)
        {
            CCConnectionRepo = connection;
            CCFolderRepo = folder;
            CCCredentialRepository = creds;
            CCFieldMappingsRepository = fieldMaps;
        }

        public ConnectionImportListSummaryModel ImportConnections(long fid, string aguid, string path, string fileExtension, long credID, string accessMethod, string syncDirection, long maxConnectionImportCount)
        {
            if (inputdatas == null)
                inputdatas = new DataInputHelper(path, fileExtension);

            bool _readheader = true;
            var _availablevalue = new List<Tuple<string, string>>();
            var colCount = 0;
            var rowCount = 0;
            ConnectionImportListSummaryModel connSummary = new ConnectionImportListSummaryModel();

            DataTable valus = inputdatas.GetImportExcel();

            var totalRowCount = (valus.Rows.Count) - 1;

            if (totalRowCount <= maxConnectionImportCount)
            {
                foreach (DataRow row in valus.Rows)
                {

                    CCConnection ConnectionObj = new CCConnection();

                    foreach (DataColumn col in valus.Columns)
                    {
                        string _colname = row[col].ToString();
                        if (_readheader) // run when it header 
                        {
                            continue;
                        }
                        else
                        {
                            if (colCount == 0)
                            {
                                ConnectionObj.SecondaryAccount = _colname;
                                colCount++;
                            }
                            else
                            {
                                if (_colname != "")
                                {
                                    ConnectionObj.CategoryFilterUsed = true;
                                    ConnectionObj.CategoryFilterValues = _colname;
                                }
                            }
                        }
                    }
                    colCount = 0;

                    if (rowCount != 0)
                    {
                        ConnectionList.Add(ConnectionObj);
                    }

                    rowCount++;
                    _readheader = false;
                }

                NumberOfConnections = ConnectionList.Count;
                ExchangeService srv = null;
                Credential creds = CCCredentialRepository.Credentials.Where(cre => cre.ID == credID).FirstOrDefault();

                string depassword = Encryption.DecryptStringAES(creds.Password, rand);
                if (creds.IsHostedExchange)
                {
                    srv = new ExchangeService(ExchangeVersion.Exchange2007_SP1);
                    if (creds.EmailAddress != null) { srv.Credentials = new WebCredentials(creds.EmailAddress, depassword); }
                    else { srv.Credentials = new WebCredentials(creds.UserName, depassword); }
                    srv.Url = new Uri(creds.URL);
                }
                else
                {
                    if (creds.ServerVersion == "2007SP1") srv = new ExchangeService(ExchangeVersion.Exchange2007_SP1);
                    else if (creds.ServerVersion == "2010") srv = new ExchangeService(ExchangeVersion.Exchange2010);
                    else if (creds.ServerVersion == "2010SP1") srv = new ExchangeService(ExchangeVersion.Exchange2010_SP1);
                    else if (creds.ServerVersion == "Exchange2010_SP1") srv = new ExchangeService(ExchangeVersion.Exchange2010_SP1);
                    else if (creds.ServerVersion == "Exchange2010_SP2") srv = new ExchangeService(ExchangeVersion.Exchange2010_SP2);
                    else srv = new ExchangeService(ExchangeVersion.Exchange2007_SP1);
                    if (creds.Domain != null) { srv.Credentials = new WebCredentials(creds.UserName, depassword, creds.Domain); }
                    else if (creds.UserName != null) { srv.Credentials = new WebCredentials(creds.UserName, depassword); }
                    else { srv.Credentials = new WebCredentials(creds.EmailAddress, depassword); }
                    srv.Url = new Uri(creds.URL);
                }

                int type = CCFolderRepo.CCFolders.FirstOrDefault(f => f.FolderID == fid).Type;
                long primarySourceID = 0;
                long credentialsID = 0;
                try { primarySourceID = Convert.ToInt64(fid); }
                catch { }

                try { credentialsID = Convert.ToInt64(creds.ID); }
                catch { }

                int successfulConnections = 0;

                foreach (var conToCreate in ConnectionList)
                {
                    bool addTopFolder = false;
                    ExchangeService ex = srv;
                    Folder fold = null;
                    Folder publicFolder = null;

                    //Xobnu.WebUI.Controllers.FolderController.conectionBeingAddedEmail = conToCreate.SecondaryAccount;

                    //System.Web.HttpContext.Current.Application["Name"] = "Value";

                    try
                    {
                        if (accessMethod == "1")
                        {
                            ex.ImpersonatedUserId = new ImpersonatedUserId(ConnectingIdType.SmtpAddress, conToCreate.SecondaryAccount);
                            if (type == 1)
                            {
                                fold = Folder.Bind(ex, new FolderId(WellKnownFolderName.Contacts, conToCreate.SecondaryAccount));
                            }
                            else
                            {
                                fold = Folder.Bind(ex, new FolderId(WellKnownFolderName.Calendar, conToCreate.SecondaryAccount));
                            }

                            addTopFolder = true;
                        }
                        else
                        {
                            //ex.ImpersonatedUserId = new ImpersonatedUserId(ConnectingIdType.SmtpAddress, conToCreate.SecondaryAccount);
                            if (type == 1)
                            {
                                fold = Folder.Bind(ex, new FolderId(WellKnownFolderName.Contacts, conToCreate.SecondaryAccount));
                            }
                            else
                            {
                                fold = Folder.Bind(ex, new FolderId(WellKnownFolderName.Calendar, conToCreate.SecondaryAccount));
                            }

                            addTopFolder = true;
                        }

                        //var f = Folder.Bind(ex, WellKnownFolderName.MsgFolderRoot);
                        TreeViewFolder mailBox = new TreeViewFolder();
                        TreeViewFolder publicBox = new TreeViewFolder();
                        string FolderSourceID = "";

                        if (fold != null)
                        {
                            var tree = LoadSubFolders(fold, new TreeViewFolder(), addTopFolder, fid);

                            mailBox.Name = "*Mail Folders";
                            mailBox.FolderClass = "All";

                            foreach (var it in tree.ChildFolders)
                            {
                                FolderSourceID = it.Id;
                                break;
                                //mailBox.ChildFolders.Add(it);
                            }


                        }

                        if (publicFolder != null)
                        {
                            var treepublic = LoadSubFolders(publicFolder, new TreeViewFolder(), addTopFolder, fid);

                            publicBox.Name = "*Public Folders";
                            publicBox.FolderClass = "All";

                            foreach (var it in treepublic.ChildFolders)
                            {
                                FolderSourceID = it.Id;
                                break;
                                //publicBox.ChildFolders.Add(it);
                            }


                        }



                        //Save the Connection
                        var connection = new Xobnu.Domain.Entities.CCConnection();
                        connection.FolderID = primarySourceID;
                        if (type == 1) { connection.FolderName = "Contacts"; }
                        else { connection.FolderName = "Calendar"; }


                        connection.Owner = creds.EmailAddress;

                        if (type == 1) { connection.Type = "Contact"; }
                        else { connection.Type = "Calendar"; }

                        connection.CredentialsID = credentialsID;
                        connection.AllowAdditions = false;
                        connection.IgnoreExisting = true;
                        connection.SyncDirection = syncDirection;
                        connection.CategoryFilterUsed = conToCreate.CategoryFilterUsed;
                        
                        if(connection.Type == "Contact")
                            connection.CopyPhotos = true;
                        else
                            connection.CopyPhotos = false;

                        if (connection.Type == "Calendar")
                            connection.TurnOffReminders = true;
                        else
                            connection.TurnOffReminders = false;
                        
                        connection.SourceID = FolderSourceID;
                        connection.Frequency = 1440;
                        connection.IsRunning = false;
                        connection.IsPaused = false;
                        connection.SecondaryAccount = conToCreate.SecondaryAccount;

                        if (accessMethod == "1")
                        {
                            connection.AccessType = 1;
                        }
                        else
                        {
                            connection.AccessType = 2;
                        }
                        string format = "yyyy-MM-dd HH:mm";
                        connection.LastSyncTime = ((DateTime)(System.Data.SqlTypes.SqlDateTime.MinValue)).ToString(format);
                        connection.CategoryFilterValues = conToCreate.CategoryFilterValues;
                        connection.SubjectTag = string.Empty;
                        connection.tagSubject = false;
                        connection.AccountGUID = aguid;
                        connection = CCConnectionRepo.SaveSubscription(connection);

                        if (connection != null)
                        {
                            var foldertag = new Xobnu.Domain.Entities.CCConnection();
                            var tagname = connection.FolderName + "[" + connection.ConnectionID + "]";
                            foldertag.Tag = tagname;
                            foldertag.ConnectionID = connection.ConnectionID;

                            CCConnectionRepo.SaveSubscriptionTag(foldertag);
                        }

                        var res = CCFieldMappingsRepository.SaveAllMappingFields(connection.FolderID, connection.ConnectionID, aguid);
                        successfulConnections++;
                    }
                    catch (Exception e)
                    {
                        connSummary.UnsuccessfulEmailList.Add(conToCreate.SecondaryAccount);

                    }
                }
                connSummary.NoOfSuccessfullConnections = successfulConnections;
                return connSummary;
            }
            else
            {
                ConnectionImportListSummaryModel connSummaryFail = new ConnectionImportListSummaryModel();
                connSummaryFail.maxConnectionLevelReach = true;

                return connSummaryFail;
            }
            
            

            
        }

        private TreeViewFolder LoadSubFolders(Folder fold, TreeViewFolder subtree, bool addTopFolder, long fid)
        {
            int type = CCFolderRepo.CCFolders.FirstOrDefault(f => f.FolderID == fid).Type;
            var view = new FolderView(100);
            view.Traversal = FolderTraversal.Shallow;
            var findFolderResults = fold.FindFolders(view);

            string typeClass = "Contact";

            if ((type) != null)
            {
                if (type == 1) { typeClass = "Contact"; }
                else { typeClass = "Appointment"; }
            }


            if (addTopFolder)
            {
                var childNode = new TreeViewFolder();
                childNode.Name = fold.DisplayName;
                childNode.Id = fold.Id.UniqueId;
                var fclass = fold.FolderClass;
                if (fclass == null) fclass = "All";
                if (fclass == "IPF.Contact") fclass = "Contact";
                else if (fclass == "IPF.Appointment") fclass = "Appointment";
                else if (fclass == "IPF.Task") fclass = "Task";
                else fclass = "All";
                childNode.FolderClass = fclass;
                subtree.ChildFolders.Add(childNode);
            }
            foreach (var item in findFolderResults.Folders)
            {
                var childNode = new TreeViewFolder();
                childNode.Name = item.DisplayName;
                childNode.Id = item.Id.UniqueId;
                var fclass = item.FolderClass;
                if (fclass == null) fclass = "All";
                if (fclass == "IPF.Contact") fclass = "Contact";
                else if (fclass == "IPF.Appointment") fclass = "Appointment";
                else if (fclass == "IPF.Task") fclass = "Task";
                else fclass = "All";
                childNode.FolderClass = fclass;
                if (item.ChildFolderCount > 0)
                {
                    if (typeClass == fclass)
                    {
                        subtree.ChildFolders.Add(LoadSubFolders(item, childNode, false, fid));
                    }
                    else if (fclass == "All")
                    {
                        subtree.ChildFolders.Add(LoadSubFolders(item, childNode, false, fid));
                    }
                }
                else
                {
                    if (typeClass == fclass)
                    {
                        subtree.ChildFolders.Add(childNode);
                    }
                }
            }
            return subtree;
        }

    }
}
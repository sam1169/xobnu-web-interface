using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Exchange.WebServices.Data;
using Xobnu.WebUI.Models;
using EWSItem = Microsoft.Exchange.WebServices.Data.Item;
using EWSContact = Microsoft.Exchange.WebServices.Data.Contact;
using EWSAppt = Microsoft.Exchange.WebServices.Data.Appointment;
using EWSGroupMember = Microsoft.Exchange.WebServices.Data.GroupMember;
using Microsoft.Exchange.WebServices.Autodiscover;
using System.Collections.ObjectModel;
//using Utils;
using System.IO;
using System.Text.RegularExpressions;

namespace Xobnu.WebUI.Util
{
    public class EWSCode
    {
        ExchangeService service = null;
        //  private Credentials _credentials = null;
        public string URL = "";

        public ConnectionConfig AutoDiscoverConnectionDetails(string email, string password)
        {
            //Try each exchange version until the correct one is found
            ConnectionConfig serviceConfig = new ConnectionConfig();
            try
            {
                service = new ExchangeService(ExchangeVersion.Exchange2013);
                service.Credentials = new WebCredentials(email, password);
                try
                {
                    service.AutodiscoverUrl(email, RedirectionUrlValidationCallback);
                }
                catch (Exception ex)
                {
                    string msg = ex.Message;
                    return null;
                }

                serviceConfig.url = service.Url.ToString();
                serviceConfig.version = GetVersion(GetExchangeVersion("2013", service));
                return serviceConfig;

                //TODO Figure out how to use Autodiscover service to get exchange version.
            }
            catch (ServiceVersionException ex1)
            {
                // Debug.DebugMessage(3, "Service Version Exception (2013): " + ex1.Message);
            }

            //Try 2010SP1 first - lowest common denominator of later versions
            try
            {
                service = new ExchangeService(ExchangeVersion.Exchange2010_SP1);
                service.Credentials = new WebCredentials(email, password);
                try
                {
                    service.AutodiscoverUrl(email, RedirectionUrlValidationCallback);
                }
                catch (Exception ex)
                {

                    return null;
                }
                serviceConfig.url = service.Url.ToString();
                serviceConfig.version = GetVersion(GetExchangeVersion("2010_SP1", service));
                return serviceConfig;

                //TODO Figure out how to use Autodiscover service to get exchange version.
            }
            catch (ServiceVersionException ex1)
            {
                // Debug.DebugMessage(3, "Service Version Exception (2010): " + ex1.Message);
            }

            //Failed so try earlier version - 2007SP1
            try
            {
                service = new ExchangeService(ExchangeVersion.Exchange2007_SP1);
                service.Credentials = new WebCredentials(email, password);
                try
                {
                    service.AutodiscoverUrl(email, RedirectionUrlValidationCallback);
                }
                catch (Exception ex)
                {

                    return null;
                }
                serviceConfig.url = service.Url.ToString();
                serviceConfig.version = GetVersion(GetExchangeVersion("2007_SP1", service));
                return serviceConfig;
            }
            catch (ServiceVersionException ex1)
            {
                // Debug.DebugMessage(3, "Service Version Exception (2007): " + ex1.Message);
            }



            //service could not be found
            return null;

        }      

        static bool RedirectionUrlValidationCallback(string redirectionUrl)
        {
            // The default for the validation callback is to reject the URL
            bool result = false;
            Uri redirectionUri = new Uri(redirectionUrl);
            if (redirectionUri.Scheme == "https")
            {
                result = true;
            }
            return result;
        }

        private static ExchangeVersion GetVersion(string version)
        {
            string versionSubString = version.Substring(0, 2);

            switch (versionSubString)
            {
                case "V2":
                    return ExchangeVersion.Exchange2013;
                    break;
                default:
                    switch (version)
                    {
                        case "2007SP1":
                        case "Exchange2007_SP1":
                            return ExchangeVersion.Exchange2007_SP1;
                            break;

                        case "2010":
                        case "Exchange2010":
                            return ExchangeVersion.Exchange2010;
                            break;

                        case "2010SP1":
                        case "Exchange2010_SP1":
                            return ExchangeVersion.Exchange2010_SP1;
                            break;

                        case "2010SP2":
                        case "Exchange2010_SP2":
                            return ExchangeVersion.Exchange2010_SP2;
                            break;
                        case "2013":
                        case "Exchange2013":
                        case "V2_14":
                            return ExchangeVersion.Exchange2013;
                            break;

                        default:
                            //Debug.DebugMessage(2, "GetVersion Error - missing exchange version: " + version);
                            return ExchangeVersion.Exchange2010_SP1;
                    }
            }


            
        }

        private string GetExchangeVersion(string version, ExchangeService service)
        {
            try
            {
                Folder myfolder = Folder.Bind(service, WellKnownFolderName.MsgFolderRoot);
                string versionFound = service.ServerInfo.VersionString;
                if (string.IsNullOrEmpty(versionFound)) return "";
                else return versionFound;
            }
            catch (Exception ex)
            {
                string errorType = string.Empty;
                string message = ex.Message;
                if (message == "The request failed. The remote server returned an error: (404) Not Found.") { errorType = "404"; }
                else if (message == "The request failed. The remote server returned an error: (401) Unauthorized.") { errorType = "401"; }
                else { errorType = ""; }
                return errorType;
            }
        }

        public string StandardConnection(string email, string password, string url)
        {

            try
            {
                //Try each exchange version until the correct one is found
                string VersionResponse = null;
                ConnectionConfig serviceConfig = new ConnectionConfig();

                WebCredentials credentials = new WebCredentials(email, password);
                Uri URL = new Uri(url);

                service = new ExchangeService(ExchangeVersion.Exchange2013);
                service.Credentials = credentials;
                service.Url = URL;              
               string foundVersion = GetExchangeVersion("2013", service);
               VersionResponse = foundVersion;
               if (foundVersion != "" && foundVersion != "404" && foundVersion != "401") return foundVersion;

                service = new ExchangeService(ExchangeVersion.Exchange2010_SP2);
                service.Credentials = credentials;
                service.Url = URL;               
               foundVersion = GetExchangeVersion("2010_SP2", service);
               VersionResponse = foundVersion;
               if (foundVersion != "" && foundVersion != "404" && foundVersion != "401") return foundVersion;

                service = new ExchangeService(ExchangeVersion.Exchange2010_SP1);
                service.Credentials = credentials;
                service.Url = URL;              
                foundVersion = GetExchangeVersion("2010_SP1", service);
                VersionResponse = foundVersion;
                if (foundVersion != "" && foundVersion != "404" && foundVersion != "401") return foundVersion;

                service = new ExchangeService(ExchangeVersion.Exchange2010);
                service.Credentials = credentials;
                service.Url = URL;               
                foundVersion = GetExchangeVersion("2010", service);
                VersionResponse = foundVersion;
                if (foundVersion != "" && foundVersion != "404" && foundVersion != "401") return foundVersion;

                service = new ExchangeService(ExchangeVersion.Exchange2007_SP1);
                service.Credentials = credentials;
                service.Url = URL;               
                foundVersion = GetExchangeVersion("2007_SP1", service);
                VersionResponse = foundVersion;
                if (foundVersion != "" && foundVersion != "404" && foundVersion != "401") return foundVersion;

                return VersionResponse;


            }
            catch (Exception ex)
            {
                // Debug.DebugMessage(2, "Error in Test Connection2: " + ex.Message);
                //MessageBox.Show("Error in Test Connection: " + ex.Message, "Test Connection");
                return "null";                
            }
        }

        public string StandardConnectionToInHouseExchanges(string username, string password, string url, string domain = null)
        {
            try
            {               
                //Try each exchange version until the correct one is found
                string VersionResponse = null;
                ConnectionConfig serviceConfig = new ConnectionConfig();
                WebCredentials credentials;
                if (domain == null)
                {
                     credentials = new WebCredentials(username, password);
                }
                else
                {
                     credentials = new WebCredentials(username, password, domain);
                }
                Uri URL = new Uri(url);

                service = new ExchangeService(ExchangeVersion.Exchange2013);
                service.Credentials = credentials;
                service.Url = URL;
                string foundVersion = GetExchangeVersion("2013", service);
                VersionResponse = foundVersion;
                if (foundVersion != "" && foundVersion != "404" && foundVersion != "401") return foundVersion;

                service = new ExchangeService(ExchangeVersion.Exchange2010_SP2);
                service.Credentials = credentials;
                service.Url = URL;
                foundVersion = GetExchangeVersion("2010_SP2", service);
                VersionResponse = foundVersion;
                if (foundVersion != "" && foundVersion != "404" && foundVersion != "401") return foundVersion;

                service = new ExchangeService(ExchangeVersion.Exchange2010_SP1);
                service.Credentials = credentials;
                service.Url = URL;
                foundVersion = GetExchangeVersion("2010_SP1", service);
                VersionResponse = foundVersion;
                if (foundVersion != "" && foundVersion != "404" && foundVersion != "401") return foundVersion;

                service = new ExchangeService(ExchangeVersion.Exchange2010);
                service.Credentials = credentials;
                service.Url = URL;
                foundVersion = GetExchangeVersion("2010", service);
                VersionResponse = foundVersion;
                if (foundVersion != "" && foundVersion != "404" && foundVersion != "401") return foundVersion;

                service = new ExchangeService(ExchangeVersion.Exchange2007_SP1);
                service.Credentials = credentials;
                service.Url = URL;
                foundVersion = GetExchangeVersion("2007_SP1", service);
                VersionResponse = foundVersion;
                if (foundVersion != "" && foundVersion != "404" && foundVersion != "401") return foundVersion;

                return VersionResponse;


            }
            catch (Exception ex)
            {
                // Debug.DebugMessage(2, "Error in Test Connection2: " + ex.Message);
               // MessageBox.Show("Error in Test Connection: " + ex.Message, "Test Connection");
                return "null";
            }

            return null;
        }
       
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CorporateContacts.Domain.Entities;

namespace CorporateContacts.WebUI.Util
{
    public static class FieldsConfigHelper
    {
        public static List<String> GetFieldForPrimaryConfig()
        {
            var myList = new List<string>();
            myList.Add("Title");
            myList.Add("First Name");
            myList.Add("Middle Name");
            myList.Add("Last Name");
            myList.Add("Suffix");
            myList.Add("Gender");
            myList.Add("Birthday");
            myList.Add("Anniversary");
            myList.Add("Job Title");
            myList.Add("Company");
            myList.Add("Department");
            myList.Add("Profession");
            myList.Add("Mobile Phone");
            myList.Add("Business Phone");
            myList.Add("Business Fax");
            myList.Add("Web Page");
            //myList.Add("Home Address");
            myList.Add("Home Address Street");
            myList.Add("Home Address City");
            myList.Add("Home Address State");
            myList.Add("Home Address Postal Code");
            myList.Add("Home Address Country");
            myList.Add("Home Phone");
            myList.Add("Home Fax");
            //myList.Add("Business Address");
            myList.Add("Business Address Street");
            myList.Add("Business Address City");
            myList.Add("Business Address State");
            myList.Add("Business Address Postal Code");
            myList.Add("Business Address Country");

            //myList.Add("Account"); //Account is an RDOAccount - not useful to sync
            myList.Add("Assistant's Name");
            myList.Add("Assistant's Phone");
            myList.Add("Billing Information");
            myList.Add("Business Phone 2");
            myList.Add("Business Address PO Box");
            myList.Add("Business Home Page");
            myList.Add("Callback");
            myList.Add("Car Phone");
            myList.Add("Categories");
            myList.Add("Children");
            myList.Add("Company Main Phone");
            myList.Add("Computer Network Name");
            myList.Add("Customer ID");
            myList.Add("Email Address");
            myList.Add("Email2 Address");
            myList.Add("Email3 Address");
            myList.Add("Full Name");
            myList.Add("Government ID Number");
            myList.Add("Hobbies");
            myList.Add("Home Phone 2");
            myList.Add("Home Address PO Box");
            myList.Add("IM Address");
            myList.Add("Internet Free/Busy Address");
            myList.Add("ISDN");
            myList.Add("Language");
            //myList.Add("City");
            //myList.Add("Country/Region");
            //myList.Add("Postal Code");
            //myList.Add("PO Box");
            //myList.Add("Prov/State");
            //myList.Add("Street Address");
            myList.Add("Manager's Name");
            myList.Add("Mileage");
            myList.Add("Nick Name");
            myList.Add("Office Location");
            myList.Add("Organizational ID Number");
            myList.Add("Other Address City");
            myList.Add("Other Address Country");
            myList.Add("Other Address Postal Code");
            myList.Add("Other Address PO Box");
            myList.Add("Other Address State");
            myList.Add("Other Address Street");
            myList.Add("Other Fax");
            myList.Add("Other Phone");
            myList.Add("Pager");
            myList.Add("Personal Home Page");
            myList.Add("Primary Phone");
            myList.Add("Radio Phone");
            myList.Add("Referred By");
            myList.Add("Spouse");
            myList.Add("Subject");
            myList.Add("Telex");
            myList.Add("TTY/TDD Phone");
            myList.Add("User1");
            myList.Add("User2");
            myList.Add("User3");
            myList.Add("User4");
            myList.Add("Notes");
            return myList;
        }

        public static List<String> GetFieldForAppointmentConfig()
        {
            var myList = new List<string>();
            myList.Add("Subject");
            myList.Add("Start Time");
            myList.Add("End Time");
            //myList.Add("Duration");
            myList.Add("Notes");
            myList.Add("Location");
            //myList.Add("Name");
            //myList.Add("Company");
            // myList.Add("TSDiary ID");
            myList.Add("Is Reminder Set");
            //myList.Add("RTF Body");
            // myList.Add("Lawyer Name");
            // myList.Add("Case Manager");
            myList.Add("Is All Day Event");
            myList.Add("Is Meeting");
            myList.Add("Optional Attendees");
            myList.Add("Required Attendees");
            myList.Add("Resources");
            myList.Add("Reminder Minutes Before Start");
            myList.Add("Billing Information");
            myList.Add("Categories");
            myList.Add("Last Modified");
            myList.Add("Last Modified By");
            myList.Add("Private");
            //myList.Add("AppointmentID");
            return myList;
        }

        public static List<String> GetFieldForAppointmentSimple()
        {
            var myList = new List<string>();
            myList.Add("Subject");
            myList.Add("Start Time");
            myList.Add("End Time");
            //myList.Add("Duration");
            myList.Add("Notes");
            myList.Add("Location");
            myList.Add("Categories");
            myList.Add("Is All Day Event");
            myList.Add("Is Meeting");
            myList.Add("Optional Attendees");
            myList.Add("Required Attendees");
            myList.Add("Resources");
            myList.Add("Is Reminder Set");
            myList.Add("Reminder Minutes Before Start");
            myList.Add("Private");
            return myList;
        }

        public static List<String> GetFieldForCrimeDiaryAppointment()
        {
            var myList = new List<string>();
            myList.Add("Subject");
            myList.Add("Start Time");
            myList.Add("Location");
            myList.Add("End Time");
            myList.Add("Notes");
            myList.Add("Is Reminder Set");
            //myList.Add("Duration");
            myList.Add("Lawyer Name");
            myList.Add("Case Manager");
            myList.Add("TSDiary ID");
            myList.Add("AppointmentID");
            myList.Add("Categories");
            return myList;
        }

        public static List<String> GetFieldForContactSimple()
        {
            var myList = new List<string>();
            myList.Add("First Name");
            myList.Add("Last Name");
            myList.Add("Job Title");
            myList.Add("Company");
            myList.Add("Mobile Phone");
            myList.Add("Business Phone");
            myList.Add("Email Address");
            myList.Add("Categories");
            myList.Add("Notes");
            return myList;
        }

        public static List<String> GetFieldForContactBusiness()
        {
            var myList = new List<string>();
            myList.Add("First Name");
            myList.Add("Last Name");
            myList.Add("Job Title");
            myList.Add("Company");
            myList.Add("Mobile Phone");
            myList.Add("Business Phone");
            myList.Add("Business Fax");
            myList.Add("Business Address Street");
            myList.Add("Business Address City");
            myList.Add("Business Address State");
            myList.Add("Business Address Postal Code");
            myList.Add("Business Address Country");
            myList.Add("Business Phone 2");
            myList.Add("Company Main Phone");
            myList.Add("Email Address");
            myList.Add("Email2 Address");
            myList.Add("Categories");
            myList.Add("Notes");
            return myList;
        }

        public static List<String> GetFieldForContactFull()
        {
            var myList = new List<string>();
            myList.Add("Title");
            myList.Add("First Name");
            myList.Add("Middle Name");
            myList.Add("Last Name");
            myList.Add("Suffix");
            myList.Add("Gender");
            myList.Add("Birthday");
            myList.Add("Anniversary");
            myList.Add("Job Title");
            myList.Add("Company");
            myList.Add("Department");
            myList.Add("Profession");
            myList.Add("Mobile Phone");
            myList.Add("Business Phone");
            myList.Add("Business Fax");
            myList.Add("Web Page");
            myList.Add("Home Address Street");
            myList.Add("Home Address City");
            myList.Add("Home Address State");
            myList.Add("Home Address Postal Code");
            myList.Add("Home Address Country");
            myList.Add("Home Phone");
            myList.Add("Home Fax");
            myList.Add("Business Address Street");
            myList.Add("Business Address City");
            myList.Add("Business Address State");
            myList.Add("Business Address Postal Code");
            myList.Add("Business Address Country");
            myList.Add("Assistant’s Name");
            myList.Add("Assistant’s Phone");
            myList.Add("Billing Information");
            myList.Add("Business Phone 2");
            myList.Add("Business Address PO Box");
            myList.Add("Business Home Page");
            myList.Add("Callback");
            myList.Add("Car Phone");
            myList.Add("Categories");
            myList.Add("Children");
            myList.Add("Company Main Phone");
            myList.Add("Computer Network Name");
            myList.Add("Customer ID");
            myList.Add("Email Address");
            myList.Add("Email2 Address");
            myList.Add("Email3 Address");
            myList.Add("Full Name");
            myList.Add("Government ID Number");
            myList.Add("Hobbies");
            myList.Add("Home Phone 2");
            myList.Add("Home Address PO Box");
            myList.Add("IM Address");
            myList.Add("Internet Free/Busy Address");
            myList.Add("ISDN");
            myList.Add("Language");
            myList.Add("Manager’s Name");
            myList.Add("Mileage");
            myList.Add("Nick Name");
            myList.Add("Office Location");
            myList.Add("Organizational ID Number");
            myList.Add("Other Address City");
            myList.Add("Other Address Country");
            myList.Add("Other Address Postal Code");
            myList.Add("Other Address PO Box");
            myList.Add("Other Address State");
            myList.Add("Other Address Street");
            myList.Add("Other Fax");
            myList.Add("Other Phone");
            myList.Add("Pager");
            myList.Add("Personal Home Page");
            myList.Add("Primary Phone");
            myList.Add("Radio Phone");
            myList.Add("Referred By");
            myList.Add("Spouse");
            myList.Add("Subject");
            myList.Add("Telex");
            myList.Add("TTY/TDD Phone");
            myList.Add("User1");
            myList.Add("User2");
            myList.Add("User3");
            myList.Add("User4");
            myList.Add("Notes");
            return myList;
        }

        public static string GetVariableType(string variable)
        {
            switch (variable)
            {
                case "Start Time":
                    return "DateTime";
                case "End Time":
                    return "DateTime";
                case "Duration":
                    return "long";
                default:
                    return "string";
            }
        }


        public static FolderField CreateFolderFieldFromName(string name)
        {
            FolderField ff = new FolderField();
            ff.FName = name;
            ff.FType = GetFieldType(name);
            ff.RealName = GetRealName(name);
            return ff;
        }

        public static FieldType GetFieldType(string fieldname)
        {
            switch (fieldname)
            {
                case "Birthday":
                case "Anniversary":
                    return FieldType.date;
                case "":
                    return FieldType.yesno;
                default:
                    return FieldType.text;
            }
        }

        public static string GetRealName(string caption)
        {
            switch (caption)
            {
                case "First Name":
                    return "GivenName";
                case "Last Name":
                    return "LastName";
                case "Company":
                    return "CompanyName";
                case "Mobile Phone":
                    return "MobilePhone";
                case "Business Phone":
                    return "BusinessPhone";
                case "Business Phone2":
                    return "Business2TelephoneNumber";
                case "Business Fax":
                    return "BusinessFaxNumber";
                case "Home Phone":
                    return "HomeTelephoneNumber";
                case "Home Phone2":
                    return "Home2TelephoneNumber";
                case "Home Fax":
                    return "HomeFaxNumber";
                case "Assistant's Phone":
                    return "AssistantTelephoneNumber";
                case "Callback":
                    return "CallbackTelephoneNumber";
                case "Car Phone":
                    return "CarTelephoneNumber";
                case "Company Main Phone":
                    return "CompanyMainTelephoneNumber";
                case "Email Address":
                    return "EmailAddress1";
                case "Internet Free/Busy Address":
                    return "InternetFreeBusyAddress";
                case "City":
                    return "MailingAddressCity";
                case "Country/Region":
                    return "MailingAddressCountry";
                case "Postal Code":
                    return "MailingAddressPostalCode";
                case "PO Box":
                    return "MailingAddressPostOfficeBox";
                case "Prov/State":
                    return "MailingAddressState";
                case "Street Address":
                    return "MailingAddressStreet";
                case "Other Fax":
                    return "OtherFaxNumber";
                case "Other Phone":
                    return "OtherTelephoneNumber";
                case "Primary Phone":
                    return "PrimaryTelephoneNumber";
                case "Radio Phone":
                    return "RadioTelephoneNumber";
                case "TTY/TDD Phone":
                    return "TTYTDDTelephoneNumber";
                case "Start Time":
                    return "Start";
                case "End Time":
                    return "End";
                default:
                    return caption.Replace(" ", "").Replace("'s", "").Replace("PO", "PostOffice");
            }
        }

    }
}
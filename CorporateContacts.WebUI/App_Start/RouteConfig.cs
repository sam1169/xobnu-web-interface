using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using LowercaseRoutesMVC;

namespace Xobnu.WebUI
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
          
            //routes.MapRoute(null, "", new { controller = "Home", action = "Index"});
            routes.MapRouteLowercase(null, "", new { controller = "Login", action = "Index" });
            routes.MapRouteLowercase(null, "Error", new { controller = "Home", action = "Error" });
            //url modifications
            routes.MapRouteLowercase(name: "Default1", url: "Login", defaults: new { controller = "Login", action = "Login" });
            routes.MapRouteLowercase(name: "Default2", url: "Forgotpassword", defaults: new { controller = "Login", action = "ForgotPassword" });
            routes.MapRouteLowercase(name: "Default3", url: "Signup", defaults: new { controller = "signup", action = "saveaccount" });
            routes.MapRouteLowercase(name: "Default4", url: "Home", defaults: new { controller = "Admin", action = "Index" });
            routes.MapRouteLowercase(name: "Default5", url: "Changepassword", defaults: new { controller = "Admin", action = "ChangePassword" });
            routes.MapRouteLowercase(name: "Default6", url: "Setupinstructions", defaults: new { controller = "Admin", action = "GetFile" });
            routes.MapRouteLowercase(name: "Default7", url: "Manage/Folders", defaults: new { controller = "Folder", action = "ManageFolders" });
            routes.MapRouteLowercase(name: "Default8", url: "Manage/Credentials", defaults: new { controller = "Folder", action = "ManageCredentials" });
            routes.MapRouteLowercase(name: "Default9", url: "Manage/folders/add", defaults: new { controller = "Folder", action = "AddFolder" });
            routes.MapRouteLowercase(name: "Default10",url: "Manage/credentials/add", defaults: new { controller = "Folder", action = "AddCredentials" });
            routes.MapRouteLowercase(name: "Default11",url: "SystemHealthStatus", defaults: new { controller = "Admin", action = "SystemHealthStatus" });
            routes.MapRouteLowercase(name: "Default12",url: "Account/Options", defaults: new { controller = "Admin", action = "AccountOptions" });
            routes.MapRouteLowercase(name: "Default13",url: "verifyuser", defaults: new { controller = "VerifyUserAccount", action = "VerifyEmailAddress", id = UrlParameter.Optional });
            routes.MapRouteLowercase(name: "Default14",url: "ResetPassword", defaults: new { controller = "Login", action = "ResetPassword", id = UrlParameter.Optional });
            routes.MapRouteLowercase(name: "Default15",url: "contacts/add/{id}", defaults: new { controller = "Folder", action = "AddContact", id = UrlParameter.Optional });
            routes.MapRouteLowercase(name: "Default16",url: "folder/viewconnections/{id}", defaults: new { controller = "Folder", action = "ViewConnections", id = UrlParameter.Optional });
            routes.MapRouteLowercase(name: "Default17",url: "Account/Billing/{id}", defaults: new { controller = "Admin", action = "BillingOptions", id = UrlParameter.Optional });
            routes.MapRouteLowercase(name: "Default18",url: "Folders/Contacts/{id}", defaults: new { controller = "Folder", action = "Items", id = UrlParameter.Optional });
            routes.MapRouteLowercase(name: "Default19",url: "Folders/Appointments/{id}", defaults: new { controller = "Folder", action = "viewAppointments", id = UrlParameter.Optional });
            routes.MapRouteLowercase(name: "Default20",url: "Subscription/{id}", defaults: new { controller = "Admin", action = "Subscription", id = UrlParameter.Optional });
            routes.MapRouteLowercase(name: "Default21",url: "Folders/{action}/{id}", defaults: new { controller = "Folder", action = "AppointmentListView", id = UrlParameter.Optional });
            routes.MapRoute(name: "Default", url: "{controller}/{action}/{id}", defaults: new { controller = "Login", action = "Index", id = UrlParameter.Optional });              
            
        }
    }
}
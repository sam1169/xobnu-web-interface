using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CorporateContacts.WebUI.Models.jsonmodels
{
   
    public class Period
    {
        public int start { get; set; }
        public int end { get; set; }
    }

   
    public class Plan
    {
        public string interval { get; set; }
        public string name { get; set; }
        public int created { get; set; }
        public int amount { get; set; }
        public string currency { get; set; }
        public string id { get; set; }
        public string @object { get; set; }
        public bool livemode { get; set; }
        public int interval_count { get; set; }
        public object trial_period_days { get; set; }       
        public string statement_description { get; set; }
    }

   
    public class Datum
    {
        public string id { get; set; }
        public string @object { get; set; }
        public string type { get; set; }
        public bool livemode { get; set; }
        public int amount { get; set; }
        public string currency { get; set; }
        public bool proration { get; set; }
        public Period period { get; set; }
        public int quantity { get; set; }
        public Plan plan { get; set; }
        public object description { get; set; }       
    }

    public class Lines
    {
        public List<Datum> data { get; set; }
        public int count { get; set; }
        public string @object { get; set; }
        public string url { get; set; }
    }

    
    public class Object
    {
        public int date { get; set; }
        public string id { get; set; }
        public int period_start { get; set; }
        public int period_end { get; set; }
        public Lines lines { get; set; }
        public int subtotal { get; set; }
        public int total { get; set; }
        public int amount { get; set; }
        public string customer { get; set; }
        public string @object { get; set; }
        public bool attempted { get; set; }
        public bool closed { get; set; }
        public bool forgiven { get; set; }
        public bool paid { get; set; }
        public bool livemode { get; set; }
        public int attempt_count { get; set; }
        public int amount_due { get; set; }
        public string currency { get; set; }
        public int starting_balance { get; set; }
        public int ending_balance { get; set; }
        public object next_payment_attempt { get; set; }
        public int webhooks_delivered_at { get; set; }
        public string charge { get; set; }
        public object discount { get; set; }
        public object application_fee { get; set; }
        public string subscription { get; set; }      
        public object statement_description { get; set; }
        public object description { get; set; }
        public object failure_message { get; set; }
        public Card @card { get; set; }
        public Source @source { get; set; }
    }

    public class Source
    {
        public string last4 { get; set; }
        public string brand { get; set; }
        public string funding { get; set; }
        public int exp_month { get; set; }
        public int exp_year { get; set; } 
    }

    public class Card
    {
        public string name { get; set; }
    }

    public class Data
    {
        public Object @object { get; set; }
    }

    public class InvoiceCreateViewModel
    {
        public int created { get; set; }
        public bool livemode { get; set; }
        public string id { get; set; }
        public string type { get; set; }
        public string @object { get; set; }
        public object request { get; set; }
        public Data data { get; set; }
    }

    public class ChargeFailedViewModel
    {
        public string type { get; set; }
        public string id { get; set; }
        public string @object { get; set; }
        public bool livemode { get; set; }
        public int amount { get; set; }
        public string status { get; set; }
        public string failureMessage { get; set; }
        public Data data { get; set; }
    }
}
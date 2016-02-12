using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CorporateContacts.Domain.Entities;

namespace CorporateContacts.WebUI.Models
{
    public class HealthMsgsViewModel
    {
        public List<CCHealthMsgs> HealthMsgList { get; set; }
        public CCHealthMsgs newObj { get; set; }
        public string FixDateTimeString { get; set; }


        public HealthMsgsViewModel()
        {
            HealthMsgList = new List<CCHealthMsgs>();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CorporateContacts.Domain.Entities;

namespace CorporateContacts.WebUI.Models
{
    public class AppointmentViewModel
    {
        public List<customAppointmentView> AppointmentListObjs { get; set; }
        public CCFolder FolderDetails { get; set; }
        public bool IsCrimeDiaryFields { get; set; }
        public LimitationsViewModel limitationsObj { get; set; }
        public string AppointmentList { get; set; }
    }

    public class customAppointmentView
    {
        public string subject { get; set; }
        public int startYear { get; set; }
        public int startMonth { get; set; }
        public int startDay { get; set; }
        public int startHrs { get; set; }
        public int startMins { get; set; }
        public int endYear { get; set; }
        public int endMonth { get; set; }
        public int endDay { get; set; }
        public int endHrs { get; set; }
        public int endMins { get; set; }
        public long ItemID { get; set; }
    }
}
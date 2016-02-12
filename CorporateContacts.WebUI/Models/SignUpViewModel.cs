using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Xobnu.Domain.Entities;

namespace Xobnu.WebUI.Models
{
    public class SignUpViewModel
    {
        public Account Account { get; set; }
        public User User { get; set; }
        public List<TimeZoneViewModel> TimeZoneList { get; set; }
    }
}
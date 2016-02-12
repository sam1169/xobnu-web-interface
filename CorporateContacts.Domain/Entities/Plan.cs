using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Xobnu.Domain.Entities
{
    [Table("tblPlans")]
    public class Plan
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int PlanLevel { get; set; }
        public bool Visible { get; set; }
    }
}

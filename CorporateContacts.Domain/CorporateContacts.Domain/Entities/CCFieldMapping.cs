using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CorporateContacts.Domain.Entities
{
    [Table("tblFieldMappings")]
    public class CCFieldMapping
    {
        [Key]
        public long ID { get; set; }
        public long ConnectionID { get; set; }
        public string FieldName { get; set; }
        public string Caption { get; set; }
        public long MappedFieldID { get; set; }
        public string AccountGUID { get; set; }
    }
}

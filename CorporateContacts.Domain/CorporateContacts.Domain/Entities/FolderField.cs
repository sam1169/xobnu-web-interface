using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorporateContacts.Domain.Entities
{
    [Table("tblFolderFields")]
    public class FolderField
    {
        public long AccountID { get; set; }
        public long ID { get; set; }
        public long FolderID { get; set; }
        public string FName { get; set; }
        [Column("FType")]
        public string FieldTypeStr { get; set; }
        [NotMapped]
        public FieldType FType
        {
            get
            {
                if (this.FieldTypeStr == "bigint") return FieldType.bigint;
                else if (this.FieldTypeStr == "number") return FieldType.number;
                else if (this.FieldTypeStr == "yesno") return FieldType.yesno;
                else if (this.FieldTypeStr == "date") return FieldType.date;
                else if (this.FieldTypeStr == "datetime") return FieldType.datetime;
                else if (this.FieldTypeStr == "memo") return FieldType.memo;
                else return FieldType.text;
            }
            set
            {
                if (value == FieldType.bigint) this.FieldTypeStr = "bigint";
                else if (value == FieldType.bigint) this.FieldTypeStr = "number";
                else if (value == FieldType.bigint) this.FieldTypeStr = "yesno";
                else if (value == FieldType.bigint) this.FieldTypeStr = "date";
                else if (value == FieldType.bigint) this.FieldTypeStr = "datetime";
                else if (value == FieldType.bigint) this.FieldTypeStr = "memo";
                else this.FieldTypeStr = "text";
            }
        }
        public string RealName { get; set; }
        public string TType { get; set; }
        public long MappedFieldID { get; set; }
    }

    public enum FieldType { text, bigint, number, yesno, date, datetime, memo }
}

using System;
using System.Collections.Generic;

namespace webServer.Models
{
    public partial class Msginfo
    {
        public int Id { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Body { get; set; }
        public int? Type { get; set; }
        public int? Ope { get; set; }
        public DateTime? Createtime { get; set; }
        public DateTime? Readtime { get; set; }
        public int? Readstatus { get; set; }
        public string Appid { get; set; }
    }
}

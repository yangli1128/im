using System;
using System.Collections.Generic;

namespace webServer.Models
{
    public partial class Accountinfo
    {
        public long Id { get; set; }
        public string AppSecret { get; set; }
        public DateTime? Createtime { get; set; }
        public string Appid { get; set; }
        public string Remark { get; set; }
    }
}

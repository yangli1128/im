﻿using System;
using System.Collections.Generic;

namespace webServer.Models
{
    public partial class Userinfo
    {
        public long Id { get; set; }
        public string Accid { get; set; }
        public DateTime? Createtime { get; set; }
        public string Appid { get; set; }
        public string Remark { get; set; }
    }
}

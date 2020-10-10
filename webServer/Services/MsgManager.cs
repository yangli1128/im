using CS.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace webServer.Services
{
    public class MsgManager : ServiceBase
    {
        public MsgManager(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
        public async Task Add(string appid, string from,  int ope,  string to,  int type,  string body)
        {
            db.Msginfo.Add(new Models.Msginfo() {
            Appid=appid,
            Body = body,
            Createtime = DateTime.Now,
            From = from,
            Ope = ope,
            Readstatus = 0,
            Readtime = null,
            To = to,
            Type = type,
            });
            await db.SaveChangesAsync();
        }
    }
}

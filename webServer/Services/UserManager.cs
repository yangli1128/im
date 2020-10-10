using CS.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace webServer.Services
{
    public class UserManager : ServiceBase
    {
        public UserManager(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
        public async Task<int> Add(string accid, string appid)
        {
            if(await db.Userinfo.Where(w => w.Accid == accid && w.Appid == appid).CountAsync()>0)
                return 1;
            db.Userinfo.Add(new Models.Userinfo()
            {
                Accid = accid,
                Appid = appid,
                Createtime = DateTime.Now,
                Remark = ""
            });
            await db.SaveChangesAsync();
            return 0;
        }

        public async Task<bool> CheckAccid(string accid, string appid)
        {
            return await db.Userinfo.Where(w => w.Accid == accid && w.Appid == appid).CountAsync() > 0;
        }
    }
}

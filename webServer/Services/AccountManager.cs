using CS.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using webServer.Models;

namespace webServer.Services
{
    public class AccountManager : ServiceBase
    {
        public AccountManager(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
        public async Task<int> Add(string appid)
        {
            if (await Get(appid) != null)
                return 1;
            db.Accountinfo.Add(new Models.Accountinfo()
            {
                Appid = appid,
                Createtime = DateTime.Now,
                Remark = "",
                AppSecret = Guid.NewGuid().ToString("N").ToLower()
            }); ;
            await db.SaveChangesAsync();
            return 0;
        }

        public async Task<Accountinfo> Get(string appid)
        {
            return await db.Accountinfo.Where(w => w.Appid == appid).FirstOrDefaultAsync();
        }
    }
}

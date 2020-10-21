using CS.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace webServer.Services
{
    public class ChannelManager : ServiceBase
    {
        public ChannelManager(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
        public async Task<int> Add(string channel, string accid, string appid)
        {
            if (await CheckChannel(channel, accid, appid))
                return 1;
            db.Groupinfo.Add(new Models.Groupinfo()
            {
                Groupid = channel,
                Accid = accid,
                Appid = appid,
                Createtime = DateTime.Now,
                Remark = ""
            });
            await db.SaveChangesAsync();
            return 0;
        }

        public async Task<bool> CheckChannel(string channel, string accid, string appid)
        {
            return await db.Groupinfo.Where(w => w.Groupid == channel && w.Accid == accid && w.Appid == appid).CountAsync() > 0;
        }
        public async Task<bool> CheckChannel(string channel, string appid)
        {
            return await db.Groupinfo.Where(w => w.Groupid == channel && w.Appid == appid).CountAsync() > 0;
        }
    }
}

using CS.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using webServer.Models;

namespace webServer.Services
{
    public class MsgManager : ServiceBase
    {
        public MsgManager(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
        public async Task<int> Add(string appid, string from, int ope, string to, int type, string body)
        {
            var v = new Models.Msginfo()
            {
                Appid = appid,
                Body = body,
                Createtime = DateTime.Now,
                From = from,
                Ope = ope,
                Readstatus = 0,
                Readtime = null,
                To = to,
                Type = type,
            };
            db.Msginfo.Add(v);
            await db.SaveChangesAsync();
            return v.Id;
        }

        public async Task Read(string appid, string from, string to)
        {
            db.Database.ExecuteSqlRaw("update msginfo set Readstatus=1,readtime=now() where appid=@appid and to=@to and from=@from and readstatus=0",
                new MySqlParameter("@appid", appid),
                new MySqlParameter("@to", from),
                new MySqlParameter("@from", to)
                );
        }

        public async Task<List<Models.Msginfo>> List(string appid, string from, string to)
        {
            return await db.Msginfo.Where(w => w.Appid == appid && ((w.From == from && w.To == to) || (w.From == to && w.To == from))).OrderByDescending(o => o.Id).Take(100).ToListAsync();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CS.Base;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using webServer.Services;

namespace webServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : BaseController
    {
        public string Ip => this.Request.Headers["X-Real-IP"].FirstOrDefault() ?? this.Request.HttpContext.Connection.RemoteIpAddress.ToString();

        private UserManager _userManager;
        /// <summary>
        /// 创建用户
        /// </summary>
        /// <param name="accid">用户IMID，唯一性，最大长度32字符</param>
        /// <returns></returns>
        [HttpPost("Create")]
        public async Task<AjaxResult<object>> Create([FromForm] Guid accid)
        {
            if (accid == Guid.Empty)
                return new AjaxResult<object>("accid未空");

            int i = await _userManager.Add(accid.ToString(), Appid);
            if (i == 1)
            {
                return new AjaxResult<object>("accid重复");
            }
            return new AjaxResult<object>(accid);
        }
        /// <summary>
        /// 获取链接地址
        /// </summary>
        /// <param name="accid"></param>
        /// <returns></returns>
        [HttpPost("ConnectServer")]
        public async Task<AjaxResult<object>> ConnectServer([FromForm] Guid accid)
        {
            if (accid == Guid.Empty)
                return new AjaxResult<object>("accid未空");
            //判断是否存在
            if (!await _userManager.CheckAccid(accid.ToString(), Appid))
            {
                return new AjaxResult<object>("accid不存在");
            }
            //获取token
            var wsserver = ImHelper.PrevConnectServer(accid, this.Ip);
            return new AjaxResult<object>((object)wsserver);
        }
        /// <summary>
        /// 判断是否在线
        /// </summary>
        /// <param name="accid"></param>
        /// <returns></returns>
        [HttpPost("HasOnline")]
        public async Task<AjaxResult<bool>> HasOnline([FromForm] Guid accid)
        {
            if (accid == Guid.Empty)
                return new AjaxResult<bool>("accid未空");
            if (!ImHelper.HasOnline(accid))
                return new AjaxResult<bool>(false);
            else
                return new AjaxResult<bool>(true);
        }
    }
}

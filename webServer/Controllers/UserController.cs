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
        private UserManager _userManager;
        public UserController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
        /// <summary>
        /// 创建用户
        /// </summary>
        /// <param name="accid">用户IMID，唯一性，最大长度32字符</param>
        /// <returns></returns>
        [HttpPost("Create")]
        public async Task<AjaxResult<object>> Create([FromForm] Guid accid)
        {
            int i = await _userManager.Add(accid.ToString(), "20201010001");
            if(i==1)
            {
                return new AjaxResult<object>("accid重复");
            }
            return new AjaxResult<object>( accid);
        }

    }
}

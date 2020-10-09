using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace webServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;

        public UserController(ILogger<UserController> logger)
        {
            _logger = logger;
        }
        /// <summary>
        /// 创建用户
        /// </summary>
        /// <param name="accid">用户IMID，唯一性，最大长度32字符</param>
        /// <returns></returns>
        [HttpPost("Create")]
        public async Task<string> Create([FromForm]string accid)
        {
            await Task.Yield();
            return accid;
        }

    }
}

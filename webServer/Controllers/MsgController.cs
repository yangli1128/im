using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using webServer.Services;

namespace webServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MsgController : BaseController
    {
        private readonly ILogger<UserController> _logger;

        private UserManager _userManager; 
        private MsgManager _msgManager;

        public MsgController(IServiceProvider serviceProvider):base(serviceProvider)
        {
        }

        /// <summary>
        /// 发送普通消息
        /// </summary>
        /// <param name="from">发送者accid</param>
        /// <param name="ope">0：点对点个人消息</param>
        /// <param name="to">ope==0是表示accid即用户id</param>
        /// <param name="type">0 表示文本消息,1 表示图片，</param>
        /// <param name="body">
        /// <![CDATA[
        /// 文本消息(type = 0)  
        /// 最大长度4000字符，JSON格式。  
        /// {  
        /// "msg":"哈哈哈"//消息内容  
        /// }  
        /// 图片消息(type = 1)  
        /// {  
        /// "name":"图片发送于2015-05-07 13:59",   //图片name  
        /// "md5":"9894907e4ad9de4678091277509361f7",    //图片文件md5  
        /// "url":"http://nimtest.nos.netease.com/cbc500e8-e19c-4b0f-834b-c32d4dc1075e",    //生成的url  
        /// "ext":"jpg",    //图片后缀  
        /// "w":6814,    //宽  
        /// "h":2332,    //高  
        /// "size":388245    //图片大小  
        /// }  
        /// ]]>
        /// </param>
        /// <returns></returns>
        [HttpPost("SendMsg")]
        public async Task<string> SendMsg([FromForm] Guid from, [FromForm] int ope, [FromForm] Guid to, [FromForm] int type, [FromForm] string body)
        {
            await _msgManager.Add("20201010001",from.ToString(),ope,to.ToString(),type,body);
            //判断自己是否在线
            if (!ImHelper.HasOnline(from))
                return "";
            //发送消息
            ImHelper.SendMessage(from, new[] { to }, (type,body), true);

            return from + ":" + to;
        }

    }
}

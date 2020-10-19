using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CS.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using webServer.Services;

namespace webServer.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class ChannelController : BaseController
    {

        private UserManager _userManager;
        private MsgManager _msgManager;
        private ChannelManager _channelManager;
        /// <summary>
        /// 创建房间
        /// </summary>
        /// <param name="accid"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        [HttpPost("CreateChannel")]
        public async Task<AjaxResult<object>> Create([FromForm] Guid accid, [FromForm] Guid channel)
        {
            if (channel == Guid.Empty)
                return new AjaxResult<object>("channel未空");
            if (!await _userManager.CheckAccid(accid.ToString(), Appid))
            {
                return new AjaxResult<object>("accid不存在");
            }
            int i = await _channelManager.Add(channel.ToString(), accid.ToString(), Appid);
            if (i == 1)
            {
                return new AjaxResult<object>("channel重复");
            }
            return new AjaxResult<object>(accid);
        }

        /// <summary>
        /// 群聊，获取群列表
        /// </summary>
        /// <returns></returns>
        [HttpPost("GetChannels")]
        public async Task<AjaxResult<object>> GetChannels()
        {
            return new AjaxResult<object>
            {
                code = 0,
                data = new { channels = ImHelper.GetChanList().Select(a => new { a.chan, a.online }) }
            };
        }

        /// <summary>
        /// 加入群聊，绑定消息频道
        /// </summary>
        /// <param name="accid">本地标识，若无则不传，接口会返回，请保存本地重复使用</param>
        /// <param name="channel">消息频道</param>
        /// <returns></returns>
        [HttpPost("SubscrChannel")]
        public async Task<AjaxResult<object>> SubscrChannel([FromForm] Guid accid, [FromForm] Guid channel)
        {
            if (!await _userManager.CheckAccid(accid.ToString(), Appid))
            {
                return new AjaxResult<object>("accid不存在");
            }
            //判断群是否存在
            if (await _channelManager.CheckChannel(channel.ToString(), accid.ToString(), Appid))
                return new AjaxResult<object>("channel不存在");

            ImHelper.JoinChan(accid, channel.ToString());
            return new AjaxResult<object>
            {
                code = 0
            };
        }
        /// <summary>
        /// 离开群聊
        /// </summary>
        /// <param name="accid"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        [HttpPost("unSubscrChannel")]
        public async Task<AjaxResult<object>> unSubscrChannel([FromForm] Guid accid, [FromForm] Guid channel)
        {
            if (!await _userManager.CheckAccid(accid.ToString(), Appid))
            {
                return new AjaxResult<object>("accid不存在");
            }
            //判断群是否存在
            if (await _channelManager.CheckChannel(channel.ToString(), accid.ToString(), Appid))
                return new AjaxResult<object>("channel不存在");

            ImHelper.LeaveChan(accid, channel.ToString());
            return new AjaxResult<object>
            {
                code = 0
            };
        }

    }
}

﻿using System;
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
    public class MsgController : BaseController
    {
        private readonly ILogger<UserController> _logger;

        private UserManager _userManager;
        private MsgManager _msgManager;
        private ChannelManager _channelManager;
        /// <summary>
        /// 发送普通消息
        /// </summary>
        /// <param name="from">发送者accid</param>
        /// <param name="ope">0：点对点个人消息,1：群消息</param>
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
        /// <param name="frominfo">
        /// <![CDATA[
        /// 发送者头像与昵称  
        /// {  
        ///     "Avator":"",  
        ///     "UserId":"",  
        ///     "NickName":""  
        /// }
        /// ]]>
        /// </param>
        /// <returns></returns>
        [HttpPost("SendMsg")]
        public async Task<AjaxResult<object>> SendMsg([FromForm] Guid from, [FromForm] int ope, [FromForm] Guid to, [FromForm] int type, [FromForm] string body, [FromForm] string frominfo)
        {
            //判断是否存在
            if (!await _userManager.CheckAccid(from.ToString(), Appid))
            {
                return new AjaxResult<object>("消息发送者 不存在");
            }
            if (ope == 0)
            {
                if (!await _userManager.CheckAccid(to.ToString(), Appid))
                {
                    return new AjaxResult<object>("用户id 不存在");
                }
            }
            else if (ope == 1)
            {
                if (!await _channelManager.CheckChannel(to.ToString(), Appid))
                {
                    return new AjaxResult<object>("群id 不存在");
                }
            }
            else
            {
                return new AjaxResult<object>("ope不存在");
            }

            int id = await _msgManager.Add(Appid, from.ToString(), ope, to.ToString(), type, body);
            //判断自己是否在线
            if (!ImHelper.HasOnline(from))
                return new AjaxResult<object>("你不在线");
            //发送消息
            if (ope == 0)//单聊
                ImHelper.SendMessage(from, new[] { to }, new { id, ope, type, to, body, frominfo,from }, true);
            else if (ope == 1)//群聊
                ImHelper.SendChanMessage(from, to.ToString(), new { id, ope, type, to, body, frominfo,from });

            return new AjaxResult<object>((object)(from + ":" + to));
        }
        /// <summary>
        /// 设置消息已读
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        [HttpPost("MsgRead")]
        public async Task<AjaxResult<object>> MsgRead([FromForm] Guid from, [FromForm] Guid to)
        {
            await _msgManager.Read(Appid, from.ToString(), to.ToString());
            int ope = -1;
            ImHelper.SendMessage(from, new[] { to }, new { ope, to, from }, false);

            return new AjaxResult<object>();
        }
        /// <summary>
        /// 获取消息列表100条数据
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        [HttpPost("MsgList")]
        public async Task<AjaxResult<List<Models.Msginfo>>> LoadMsg([FromForm] Guid from, [FromForm] Guid to)
        {
            var v= await _msgManager.List(Appid, from.ToString(), to.ToString());
            return new AjaxResult<List<Models.Msginfo>>(v);
        }
    }
}

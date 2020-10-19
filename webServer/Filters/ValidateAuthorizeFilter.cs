using CS.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using webServer.Services;

namespace webServer.Filters
{
    public class ValidateAuthorizeFilter : ActionFilterAttribute
    {
        AccountManager _accountManager;
        public ValidateAuthorizeFilter(AccountManager account)
        {
            _accountManager = account;
        }
        override public void OnActionExecuted(ActionExecutedContext context)
        {

        }

        override public void OnActionExecuting(ActionExecutingContext context)
        {
            var headers = context.HttpContext.Request.Headers;
            int tvali = TokenValidate(headers);

            if (tvali == 1)
            {
                RetrunLoginInfo(context, 414, "参数错误");
                return;
            }

        }

        private int TokenValidate(IHeaderDictionary headers)
        {
            string appid = headers["appid"];//开发者平台分配的 appid
            string nonce = headers["nonce"];//随机数（最大长度128个字符）
            string curTime = headers["curTime"];//当前UTC时间戳，从1970年1月1日0点0 分0 秒开始到现在的秒数(String)
            string checksum = headers["checksum"];//SHA1(AppSecret + Nonce + CurTime)，三个参数拼接的字符串，进行SHA1哈希计算，转化成16进制字符(String，小写)
            //时间处理
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1);
            if (long.TryParse(curTime, out long t))
                if (Math.Abs((long)ts.TotalSeconds - t) > 5 * 60)
                    return 1;
            //根据appid 获取 appSecret
            var r = _accountManager.Get(appid).Result;
            if (r == null)
                return 1;
            string appSecret = r.AppSecret;
            string check = Sha1Encrypt(appSecret + nonce + curTime, Encoding.UTF8);

            if (checksum != check)
            {
                return 1;
            }
            return 0;
        }
        public static string Sha1Encrypt(string content, Encoding encode)
        {
            byte[] StrRes = encode.GetBytes(content);
            HashAlgorithm iSHA = new SHA1CryptoServiceProvider();
            StrRes = iSHA.ComputeHash(StrRes);
            StringBuilder EnText = new StringBuilder();
            foreach (byte iByte in StrRes)
            {
                EnText.AppendFormat("{0:x2}", iByte);
            }
            return EnText.ToString().ToLower();
        }
        protected void RetrunLoginInfo(ActionExecutingContext context, int code, string msg)
        {
            AjaxResult<object> result = new AjaxResult<object>()
            {
                code = code,
                msg = msg
            };
            JsonResult jr = new JsonResult(result);
            context.Result = jr;
        }
    }
}

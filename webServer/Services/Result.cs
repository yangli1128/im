using System;
using System.Collections.Generic;
using System.Text;

namespace CS.Base
{
    public class AjaxResult<T>
    {
        public AjaxResult()
        { }

        public AjaxResult(string message)
        {
            code = 1;
            this.msg = message;
        }
        public AjaxResult(string message, int code)
        {
            this.code = code;
            this.msg = message;
        }
        public AjaxResult(T obj)
        {
            this.data = obj;
        }
        /// <summary>
        /// 错误代码，0：正常，其他值表示错误
        /// </summary>
        public int code { get; set; }
        /// <summary>
        /// 返回的信息提示
        /// </summary>
        public string msg { get; set; }
        /// <summary>
        /// 返回的数据
        /// </summary>
        public T data { get; set; }
    }
    /// <summary>
    /// bootstrap table 翻页返回结果
    /// </summary>
    public class FlipPageResult<T>
    {
        /// <summary>
        /// 总的数据记录
        /// </summary>
        public int total { get; set; }
        /// <summary>
        /// 当前的数据集合
        /// </summary>
        public List<T> rows { get; set; }
    }
}

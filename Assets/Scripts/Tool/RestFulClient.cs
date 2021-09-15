using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Assets.Scripts.Result;
namespace Assets.Scripts.Tool
{
    class RestFulClient
    {
        /// <summary>
        /// 请求类型
        /// </summary>
        public enum EnumHttpVerb
        {
            GET,
            POST,
            PUT,
            DELETE
        }

        public class RestClient
        {
            #region 属性
            /// <summary>
            /// 端点路径
            /// </summary>
            public string EndPoint { get; set; }

            /// <summary>
            /// 请求方式
            /// </summary>
            public EnumHttpVerb Method { get; set; }

            /// <summary>
            /// 文本类型（1、application/json 2、txt/html）
            /// </summary>
            public string ContentType { get; set; }

            /// <summary>
            /// 请求的数据(一般为JSon格式)
            /// </summary>
            public string PostData { get; set; }

            /// <summary>
            /// 错误信息
            /// </summary>
            public string ErrorMessage { get; set; }
            #endregion

            #region 初始化
            public RestClient()
            {
                EndPoint = "";
                Method = EnumHttpVerb.GET;
                ContentType = "application/json";
                PostData = "";
            }

            public RestClient(string endpoint)
            {
                EndPoint = endpoint;
                Method = EnumHttpVerb.GET;
                ContentType = "application/json";
                PostData = "";
            }

            public RestClient(string endpoint, EnumHttpVerb method)
            {
                EndPoint = endpoint;
                Method = method;
                ContentType = "application/json";
                PostData = "";
            }

            public RestClient(string endpoint, EnumHttpVerb method, string postData)
            {
                EndPoint = endpoint;
                Method = method;
                ContentType = "application/json";
                PostData = postData;
            }
            #endregion

            #region 方法
            /// <summary>
            /// http请求(不带参数请求)
            /// </summary>
            /// <returns></returns>
            public string HttpRequest()
            {
                return HttpRequest("");
            }

            /// <summary>
            /// http请求(带参数)
            /// </summary>
            /// <param name="parameters">parameters例如：?name=LiLei</param>
            /// <returns></returns>
            public string HttpRequest(string parameters)
            {
                var request = (HttpWebRequest)WebRequest.Create(EndPoint + parameters);
                request.Method = Method.ToString();
                request.ContentLength = 0;
                request.ContentType = ContentType;

                if (!string.IsNullOrEmpty(PostData) && Method == EnumHttpVerb.POST)
                {
                    var bytes = Encoding.UTF8.GetBytes(PostData);
                    request.ContentLength = bytes.Length;
                    using (var writeStream = request.GetRequestStream())
                    {
                        writeStream.Write(bytes, 0, bytes.Length);
                    }
                }
                HttpWebResponse response;
                try
                {
                    response = (HttpWebResponse)request.GetResponse();
                }
                catch (WebException ex)
                {
                    Debug.Log(ex.Message);
                    response = (HttpWebResponse)ex.Response;
                    using (Stream data = response.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(data))
                        {
                            string text = reader.ReadToEnd();
                            Debug.Log(text);
                            //显示提示
                            GetBadResultMsg(text);
                            return null;
                        }
                    }
                }
                string responseValue = null;
                using (Stream responseStream = response.GetResponseStream())
                {
                    
                    if (responseStream != null)
                        using (var reader = new StreamReader(responseStream))
                        {
                            responseValue = reader.ReadToEnd();
                            //Debug.Log(responseValue+"Value");
                        }
                }
                return responseValue;
            }
            #endregion

            private string GetBadResultMsg(string msg)
            {
                BadResult badResult = new BadResult();
                badResult = JsonUtility.FromJson<BadResult>(msg);
                ErrorMessage = badResult.message;
                return ErrorMessage;
            }
        }
    }
}

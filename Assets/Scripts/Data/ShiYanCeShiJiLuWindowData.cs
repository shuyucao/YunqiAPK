using Assets.Scripts.Constant;
using Assets.Scripts.Request;
using Assets.Scripts.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Assets.Scripts.Tool.RestFulClient;

namespace Assets.Scripts.Data
{
    public class ShiYanCeShiJiLuWindowData:Singleton<ShiYanCeShiJiLuWindowData>
    {
        //实验测试记录
        private ShiYanCeShiJiLuResult s_CeShiJiLuResult = null;

        public void SaveShiYanCeShiJiLuResult(ShiYanCeShiJiLuResult shiYanCeShiJiLu)
        {
            this.s_CeShiJiLuResult = shiYanCeShiJiLu;
        }

        public ShiYanCeShiJiLuResult GetShiYanCeShiJiLuResult()
        {
            return s_CeShiJiLuResult;
        }


        RestClient client = new RestClient();

        //获取实验测试记录方法
        public void GetShiYanJiLu()
        {
            client.EndPoint = CommonConstant.SERVER_URL_MEC_PROXY;
            client.Method = EnumHttpVerb.POST;
            ShiYanCeShiJiLuRequest shiYanCeShiJiLu = new ShiYanCeShiJiLuRequest();
            MacInfoRequest macInfo = new MacInfoRequest();
            shiYanCeShiJiLu.page = 0;
            shiYanCeShiJiLu.size = 10;
            //判断是账号登录还是手机登录
            if (LoginWindowData.Instance.ReadAccountLoginResult() != null)
            {
                macInfo.mac_key = LoginWindowData.Instance.ReadAccountLoginResult().mac_key;
                macInfo.token = LoginWindowData.Instance.ReadAccountLoginResult().access_token;
            }
            else if (LoginWindowData.Instance.ReadToKenSwapResult() != null)
            {
                macInfo.mac_key = LoginWindowData.Instance.ReadToKenSwapResult().data.mac_key;
                macInfo.token = LoginWindowData.Instance.ReadToKenSwapResult().data.access_token;
            }
            macInfo.session_key = LoginWindowData.Instance.ReadSessionResult().session_key;
            shiYanCeShiJiLu.macInfo = macInfo;
            client.PostData = JsonUtility.ToJson(shiYanCeShiJiLu);
            string resultCeShi = client.HttpRequest(CommonConstant.GET_SHIYANCESHI_LIST);

            if (string.IsNullOrEmpty(resultCeShi))
            {
                Debug.Log(client.ErrorMessage);
                return;
            }
            else
            {
                ShiYanCeShiJiLuResult shiYanCeShiJiLuResult = new ShiYanCeShiJiLuResult();
                shiYanCeShiJiLuResult = JsonUtility.FromJson<ShiYanCeShiJiLuResult>(resultCeShi);
                SaveShiYanCeShiJiLuResult(shiYanCeShiJiLuResult);
            }
        }
    }
}

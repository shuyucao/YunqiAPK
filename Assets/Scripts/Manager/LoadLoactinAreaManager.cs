using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.IO;
using System.Collections;
using Assets.Scripts.Result;
using Assets.Scripts.Constant;
using UnityEngine.Networking;
using Assets.Scripts.Data;

namespace Assets.Scripts.Manager
{

    public class LoadLoactinAreaManager
    {
        public static IEnumerator RequestLoactionArea()
        {
            UnityWebRequest request = UnityWebRequest.Get(CommonConstant.GET_LOCATIONURL);
            yield return request.SendWebRequest();
            if (string.IsNullOrEmpty(request.error))
            {
                ResponseBodyResult response = JsonUtility.FromJson<ResponseBodyResult>(request.downloadHandler.text);
                ShiYanCeShiWindowData.Instance.GetAreaName = response.content.address_detail.province;
                Debug.Log("地区名：" + ShiYanCeShiWindowData.Instance.GetAreaName);
            }
        }
    }
}

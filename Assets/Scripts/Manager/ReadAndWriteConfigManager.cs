using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.IO;
using System.Collections;
using Assets.Scripts.Result;
using Assets.Scripts.Constant;
using Assets.Scripts.Tool;

namespace Assets.Scripts.Manager
{
    //读写配置文件
    public static class ReadAndWriteConfigManager
    {
        //文件名称
        public const string FlieName = "/GatewayConfig.json";
        //文件的存储路径
        public static string FliePath = "";

        //从streamingAsset文件夹下的文件路径
        public static string SrcPath = "";

        //读取的json文本
        public static string result = "";

        public static GatewayConfigResult ipConfig;

        //读取配置文件
        public static GatewayConfigResult ReadConfig() 
        {
            FliePath = Application.persistentDataPath + FlieName;
            //FliePath = Application.streamingAssetsPath + FlieName;
            if (!File.Exists(FliePath))
            {
                return null;
            }
            else
            {
                StreamReader reader = new StreamReader(FliePath);
                if (reader == null)
                {
                    return null;
                }
                string jsonflie = reader.ReadToEnd();
                ipConfig = new GatewayConfigResult();
                ipConfig = JsonUtility.FromJson<GatewayConfigResult>(jsonflie);
                reader.Close();
                reader.Dispose();
                return ipConfig;
            }
        }

        //写入配置文件
        public static void WriteConfig(GatewayConfigResult ipConfig)
        {
            FliePath = Application.persistentDataPath + FlieName;
            if (!File.Exists(FliePath))
            {
                return;
            }
            else
            {
                FileInfo file = new FileInfo(FliePath);
                StreamWriter sw = file.CreateText();
                string json = JsonUtility.ToJson(ipConfig);
                sw.WriteLine(json);
                sw.Close();
                sw.Dispose();
            }
        }

        //保存Config
        public static void SaveConfig(GatewayConfigResult ipConfig) 
        {

            //if (ipConfig.MECUrl == "")
            //{
            //    CommonConstant.SERVER_URL_MEC_PROXY = ipConfig.MECUrlDefult + CommonConstant.SERVER_URL_VARIFY_VLAB;
            //    CommonConstant.SERVER_URL_MEC_PROXY_UC= ipConfig.MECUrlDefult + CommonConstant.SERVER_URL_VARIFY_UC_GATEWAY;
            //}
            //else
            //{
            //    CommonConstant.SERVER_URL_MEC_PROXY = ipConfig.MECUrl + CommonConstant.SERVER_URL_VARIFY_VLAB;
            //    CommonConstant.SERVER_URL_MEC_PROXY_UC = ipConfig.MECUrl + CommonConstant.SERVER_URL_VARIFY_UC_GATEWAY;
            //}
            //if (ipConfig.SBYUrl == "")
            //{
            //    CommonConstant.SERVER_URL_SBY_PROXY = ipConfig.SBYUrlDefult;
            //    CommonConstant.SERVER_URL_SBY_PROXYDEFULAT = ipConfig.SBYUrlDefult;
            //}
            //else
            //{
            //    CommonConstant.SERVER_URL_SBY_PROXY = ipConfig.SBYUrl;
            //}
            if (string.IsNullOrEmpty(ipConfig.MECUrl))
            {
                CommonConstant.SERVER_URL_MEC_PROXY = ipConfig.MECUrlDefult + CommonConstant.SERVER_URL_VARIFY_VLAB;
                CommonConstant.SERVER_URL_MEC_PROXY_UC = ipConfig.MECUrlDefult + CommonConstant.SERVER_URL_VARIFY_UC_GATEWAY;
            }
            else
            {
                CommonConstant.SERVER_URL_MEC_PROXY = ipConfig.MECUrl + CommonConstant.SERVER_URL_VARIFY_VLAB;
                CommonConstant.SERVER_URL_MEC_PROXY_UC = ipConfig.MECUrl + CommonConstant.SERVER_URL_VARIFY_UC_GATEWAY;
            }
            if (string.IsNullOrEmpty(ipConfig.SBYUrl))
            {
                CommonConstant.SERVER_URL_SBY_PROXY = ipConfig.SBYUrlDefult;
                CommonConstant.SERVER_URL_SBY_PROXYDEFULAT = ipConfig.SBYUrlDefult;
            }
            else
            {
                CommonConstant.SERVER_URL_SBY_PROXY = ipConfig.SBYUrl;
            }
            Debug.Log("中继："+CommonConstant.SERVER_URL_MEC_PROXY);
            Debug.Log("中继资源："+CommonConstant.SERVER_URL_MEC_PROXY_UC);
            Debug.Log("视博云："+CommonConstant.SERVER_URL_SBY_PROXY);
            PlayCyberCloundResource.Instance.InitCyberCloudApp();
        }

        //从streamingAssets读取config并写入persistentDataPath
        public static IEnumerator LoadConfig() 
        {
            string path = Application.persistentDataPath;
            if (!File.Exists(FliePath))
            {
                SrcPath = Application.streamingAssetsPath + FlieName;
                WWW www = new WWW(SrcPath);
                yield return www;
                result = www.text;

                FliePath = path + FlieName;
                FileInfo file = new FileInfo(FliePath);
                StreamWriter sw = file.CreateText();
                sw.WriteLine(result);
                sw.Close();
                sw.Dispose();

                GatewayConfigResult ipConfig = ReadConfig();
                SaveConfig(ipConfig);
            }
            else
            {
                GatewayConfigResult ipConfig = ReadConfig();
                SaveConfig(ipConfig);
            }
        }
    }
}

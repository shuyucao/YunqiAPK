using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using static Assets.Scripts.Config.PrefabType;
using Assets.Scripts.Manager;
using static Assets.Scripts.Tool.RestFulClient;
using Assets.Scripts.Constant;
using Assets.Scripts.Result;
using Assets.Scripts.Request;
using Assets.Scripts.Data;
using Assets.Scripts.Other;

namespace Assets.Scripts.Windows
{
    class ExperimentHistoryWindow : BaseWindow
    {
        #region 字段定义
        private GameObject EHResourcesContent;

        private ShiYanCeShiJiLuResult historyData;

        private Button EHReturnBtn;

        RestClient client = new RestClient();
        #endregion
        #region 窗口生命周期
        public override void OnInit()
        {
            prefabType = Window.ExperimentHistoryWindow;
            layer = WindowManager.Layer.Window;
        }
        public override void Init()
        {
            base.Init();
            InitData();
            InitComponent();
        }
        //面板打开时
        public override void OnShow(params object[] para)
        {
            ShowHistory();
        }
        //面板关闭时
        public override void OnClose()
        {
            base.OnClose();
        }
        #endregion
        #region 初始化

        private void InitData()
        {
            GetHistory();
        }

        private void InitComponent()
        {
            EHResourcesContent = GameObject.Find("EHResourcesContent");
            EHReturnBtn = GameObject.Find("EHReturnBtn").GetComponent<Button>();
            EHReturnBtn.onClick.AddListener(EHBtnOnClick);
        }
        #endregion
        #region 逻辑
        private void ShowHistory()
        {
            if (historyData.data.todayList.Count>0)
            {
                AddGritToHistory("·今天", historyData.data.todayList);
            }
            if (historyData.data.weekList.Count > 0)
            {
                AddGritToHistory("·本周", historyData.data.weekList);
            }
            if (historyData.data.moreList.Count > 0)
            {
                AddGritToHistory("·更多",historyData.data.moreList);
            }
            
        }
        private void GetHistory()
        {
            client.EndPoint = CommonConstant.SERVER_URL_MEC_PROXY;
            client.Method = EnumHttpVerb.POST;
            ShiYanCeShiJiLuRequest shiYanCeShiJiLu = new ShiYanCeShiJiLuRequest();
            MacInfoRequest macInfo = new MacInfoRequest();
            shiYanCeShiJiLu.page = 0;
            shiYanCeShiJiLu.size = 10;
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
            Debug.Log("实验测试记录：" + resultCeShi);

            if (string.IsNullOrEmpty(resultCeShi))
            {
                Debug.Log(client.ErrorMessage);
                return;
            }
            else
            {
                historyData = JsonUtility.FromJson<ShiYanCeShiJiLuResult>(resultCeShi);
            }
        }
        private void AddGritToHistory(string date, List<ShiYanJiLuItemList> list)
        {
            GameObject todayPrefab = Resources.Load<GameObject>("WindowGrid/TitleImageGrid");
            GameObject todayTitle = Instantiate(todayPrefab, EHResourcesContent.transform);
            todayTitle.GetComponentInChildren<Text>().text = date;
            GameObject todayContentPrefab = Resources.Load<GameObject>("WindowGrid/ResourcesContentGrid");
            GameObject todayContent = Instantiate(todayContentPrefab, EHResourcesContent.transform);
            foreach (ShiYanJiLuItemList item in list)
            {
                GameObject gridPrefab = Resources.Load<GameObject>("WindowGrid/EHGrid");
                GameObject grid = Instantiate(gridPrefab, todayContent.transform);
                grid.transform.GetChild(0).GetComponent<Text>().text = item.name;
                Image image = grid.transform.GetChild(1).GetComponent<Image>();
                StartCoroutine(AddImage(item.thumbnail, image));
            }
        }
        IEnumerator AddImage(string url, Image image)
        {
            WWW www = new WWW(url);
            yield return www;
            if (www.isDone)
            {
                Texture2D tex = www.texture;
                Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero);
                if (image != null)
                {
                    image.sprite = sprite;
                }
            }
        }
        #endregion
        #region 按钮点击事件

        private void EHBtnOnClick()
        {
            WindowManager.ReShow();
            WindowManager.Close("ExperimentHistoryWindow");
        }
        #endregion
    }
}

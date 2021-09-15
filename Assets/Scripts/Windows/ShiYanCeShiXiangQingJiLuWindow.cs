using Assets.Scripts.Config;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using Assets.Scripts.Data;
using Assets.Scripts.Result;
using Assets.Scripts.Request;
using static Assets.Scripts.Tool.RestFulClient;
using Assets.Scripts.Constant;
using Assets.Scripts.Manager;
using Pvr_UnitySDKAPI;
using XCharts;

namespace Assets.Scripts.Windows
{
    public class ShiYanCeShiXiangQingJiLuWindow:BaseWindow
    {

        RestClient client = new RestClient();

        //返回按钮
        private Button ReturnXqJiLu;

        private int TrainingMode = 0;

        private string EntryId = "";

        //次数
        private Button CountBtn;
        private GameObject CountBtnOneChildObj;
        private Text CountBtnText;

        //日
        private Button DateBtn;
        private GameObject DateBtnOneChildObj;
        private Text DateBtnText;

        //月
        private Button MonthBtn;
        private GameObject MonthBtnOneChildObj;
        private Text MonthBtnText;

        //年
        private Button YearBtn;
        private GameObject YearBtnOneChildObj;
        private Text YearBtnText;

        public Color PageDisableColor = new Color32(79, 75, 75, 255);
        //没选中字体的颜色
        private Color DeColor = new Color32(49, 255, 255, 255);
        //选中字体的颜色
        private Color SeColor = Color.white;

        //查看明细
        private Button ChaKanMingXiBtn;

        //练习
        private GameObject LanXiTipObj;
        private GameObject ZiCeLianTipsImage;
        private GameObject LianXiTipsImage;
        //自测
        private GameObject ZiCeTipObj;

        //实验用时
        private GameObject ShiYanTimerTileText;

        //实验用时的日期显示文本
        private Text ShiYanTimerDateInfoText;

        //实验用时的时间显示文本
        private Text ShiYanTimerInfoText;

        //得分
        private GameObject ShiYanScroeTile;
        //得分的显示文本
        private Text ShiYanScroeInfo;

        //得分
        private GameObject ShiYanYongShiTile;
        //得分的显示文本
        private Text ShiYanYongShiInfo;

        //平均得分
        private GameObject ShiYanPingJunScroeTileTongJi;

        private Text ShiYanPingJunScroeInfo;

        //平均用时
        private GameObject ShiYanPingJunYongShiTileTongJi;
        private Text ShiYanPingJunYongShiInfo;

        //训练次数
        private GameObject ShiYanXunLianCiShuTileTongJi;
        private Text ShiYanXunLianCiShuInfo;
        //当前页数
        private int s_currentPage = 1;

        //总页数
        private int s_totalCount = 0;

        //限制的个数
        private int limit = 10;

        //当前的选择的模式
        private int s_currentSearch_mode = 0;

        private GameObject ZheXianTuRect;

        //下一页按钮
        private Button NextPageBtn;
        private Text NextPageBtnText;

        //上一页按钮
        private Button PrePageBtn;
        private Text PrePageBtnText;

        //没有记录数据的界面
        private GameObject NoJiLuShuJuPanel;

        //有记录数据的界面
        private GameObject JiLuShuJuPanel;

        //数据是否加载完成，加载完成可以点击上一页下一页
        private bool isLoadComplete = false;
        private int LoadCompleteCount = 0;
        public override void OnInit()
        {
            base.OnInit();
            prefabType = PrefabType.Window.ShiYanCeShiXiangQingJiLuWindow;
            prefab = gameObject;
        }

        public override void Init()
        {
            base.Init();
            InitData();
            DefualtSeting();
        }

        public override void OnClose()
        {
            base.OnClose();
        }

        public override void OnHide()
        {
            base.OnHide();
        }

        public override void OnReShow()
        {
            base.OnReShow();
        }

        public override void OnShow(params object[] para)
        {
            base.OnShow(para);
        }

        private LineRenderer line;
        private void InitData() 
        {
            NoJiLuShuJuPanel = GameObject.Find("NOJiLu");
            JiLuShuJuPanel = GameObject.Find("YesJiLu");
            ZheXianTuRect = GameObject.Find("ZheXianTuNew");
            line = ZheXianTuRect.GetComponent<LineRenderer>();
            ReturnXqJiLu = GameObject.Find("ReturnXQJiLu").GetComponent<Button>();
            CountBtn = GameObject.Find("CiShuBtn").GetComponent<Button>();
            YearBtn = GameObject.Find("YearBtn").GetComponent<Button>();
            MonthBtn = GameObject.Find("MonthBtn").GetComponent<Button>();
            DateBtn = GameObject.Find("DateBtn").GetComponent<Button>();
            ChaKanMingXiBtn = GameObject.Find("MingXiBtn").GetComponent<Button>();
            CountBtnOneChildObj = GameObject.Find("CiShuBtn").transform.GetChild(0).gameObject;
            CountBtnText= GameObject.Find("CiShuBtn").transform.GetChild(1).GetComponent<Text>();
            YearBtnOneChildObj = GameObject.Find("YearBtn").transform.GetChild(0).gameObject;
            YearBtnText = GameObject.Find("YearBtn").transform.GetChild(1).GetComponent<Text>();

            MonthBtnOneChildObj = GameObject.Find("MonthBtn").transform.GetChild(0).gameObject;
            MonthBtnText = GameObject.Find("MonthBtn").transform.GetChild(1).GetComponent<Text>();

            DateBtnOneChildObj = GameObject.Find("DateBtn").transform.GetChild(0).gameObject;
            DateBtnText = GameObject.Find("DateBtn").transform.GetChild(1).GetComponent<Text>();

            ZiCeTipObj = GameObject.Find("ZiCeTips");
            LanXiTipObj = GameObject.Find("LainXiTips");
            ZiCeLianTipsImage = LanXiTipObj.transform.GetChild(2).gameObject;
            LianXiTipsImage = LanXiTipObj.transform.GetChild(1).gameObject;
            ShiYanTimerTileText = GameObject.Find("ShiYanTimerTile");
            ShiYanTimerDateInfoText = GameObject.Find("ShiYanTimerDateInfo").GetComponent<Text>();
            ShiYanTimerInfoText = GameObject.Find("ShiYanTimerInfo").GetComponent<Text>();

            ShiYanScroeTile = GameObject.Find("ShiYanScroeTile");
            ShiYanScroeInfo = GameObject.Find("ShiYanScroeInfo").GetComponent<Text>();

            ShiYanYongShiTile = GameObject.Find("ShiYanYongShiTile");
            ShiYanYongShiInfo= GameObject.Find("ShiYanYongShiInfo").GetComponent<Text>();
      
            ShiYanPingJunScroeTileTongJi = GameObject.Find("ShiYanPingJunScroeTileTongJi");
            ShiYanPingJunScroeInfo = GameObject.Find("ShiYanPingJunScroeInfo").GetComponent<Text>();
            ShiYanPingJunYongShiTileTongJi = GameObject.Find("ShiYanPingJunYongShiTileTongJi");
            ShiYanPingJunYongShiInfo = GameObject.Find("ShiYanPingJunYongShiInfo").GetComponent<Text>();
            ShiYanXunLianCiShuTileTongJi = GameObject.Find("ShiYanXunLianCiShuTileTongJi");
            ShiYanXunLianCiShuInfo = GameObject.Find("ShiYanPingJunCiShuShiInfo").GetComponent<Text>();
            NextPageBtn = GameObject.Find("NextPageBtn").GetComponent<Button>();
            PrePageBtn = GameObject.Find("PrePageBtn").GetComponent<Button>();
            NextPageBtnText = GameObject.Find("NextPageBtn").transform.GetChild(0).GetComponent<Text>();
            PrePageBtnText = GameObject.Find("PrePageBtn").transform.GetChild(0).GetComponent<Text>();

            NextPageBtn.onClick.AddListener(delegate () { NextPageOnClick(); });
            PrePageBtn.onClick.AddListener(delegate () { PrePageOnClick(); });
            ReturnXqJiLu.onClick.AddListener(ReturnOnClick);
            ChaKanMingXiBtn.onClick.AddListener(ChaKanMingXiOnClick);
            CountBtn.onClick.AddListener(CountOnClick);
            YearBtn.onClick.AddListener(YearOnClick);
            MonthBtn.onClick.AddListener(MonthOnClick);
            DateBtn.onClick.AddListener(DateOnClick);
        }

        //上一页
        private void PrePageOnClick()
        {
            if (isLoadComplete)
            {
                isLoadComplete = false;
                foreach (var item in ShiYanCeShiXiangQingJiLuWindowData.Instance.ZheXianItemsDic.Keys)
                {
                    Destroy(item);
                }
                ShiYanCeShiXiangQingJiLuWindowData.Instance.ZheXianItemsDic.Clear();
                line.positionCount = 0;
                ZheXianTuRect.GetComponent<HorizontalLayoutGroup>().enabled = true;
                s_currentPage -= 1;
                if (ShiYanCeShiXiangQingJiLuWindowData.Instance.IsClickCount)
                {
                    GetShiYanXiangQingJiLuCountInfo(ShiYanCeShiXiangQingMainWindowData.Instance.GetInstanceId);
                }
                else
                {

                    GetShiYanXiangQingJiLuYearMonthDateInfo(ShiYanCeShiXiangQingMainWindowData.Instance.GetInstanceId, s_currentSearch_mode);
                }
            }
        }

        //下一夜
        private void NextPageOnClick()
        {
            if (isLoadComplete)
            {
                isLoadComplete = false;
                foreach (var item in ShiYanCeShiXiangQingJiLuWindowData.Instance.ZheXianItemsDic.Keys)
                {
                    Destroy(item);
                }
                ShiYanCeShiXiangQingJiLuWindowData.Instance.ZheXianItemsDic.Clear();
                line.positionCount = 0;
                ZheXianTuRect.GetComponent<HorizontalLayoutGroup>().enabled = true;
                s_currentPage += 1;
                if (ShiYanCeShiXiangQingJiLuWindowData.Instance.IsClickCount)
                {
                    GetShiYanXiangQingJiLuCountInfo(ShiYanCeShiXiangQingMainWindowData.Instance.GetInstanceId);
                }
                else
                {
                    GetShiYanXiangQingJiLuYearMonthDateInfo(ShiYanCeShiXiangQingMainWindowData.Instance.GetInstanceId, s_currentSearch_mode);
                }
            }
         
        }

        private void DefualtSeting() 
        {
            LoadCompleteCount = 0;
            isLoadComplete = true;
            foreach (var item in ShiYanCeShiXiangQingJiLuWindowData.Instance.ZheXianItemsDic.Keys)
            {
                Destroy(item);
            }
            ShiYanCeShiXiangQingJiLuWindowData.Instance.ZheXianItemsDic.Clear();
            line.positionCount = 0;
            ZheXianTuRect.GetComponent<HorizontalLayoutGroup>().enabled = true;
            limit = 10;
            s_currentSearch_mode = 0;
            ShiYanCeShiXiangQingJiLuWindowData.Instance.GetShiYanCeShiXiangQingJiLuWindow = WindowManager.root.GetComponent<ShiYanCeShiXiangQingJiLuWindow>();
            ShiYanCeShiXiangQingJiLuWindowData.Instance.IsClickCount = true;
            ZiCeLianTipsImage.SetActive(false);
            LianXiTipsImage.SetActive(false);
            ShiYanPingJunScroeTileTongJi.SetActive(false);
            ShiYanPingJunYongShiTileTongJi.SetActive(false);
            ShiYanXunLianCiShuTileTongJi.SetActive(false);
            ShiYanTimerTileText.SetActive(true);
            ShiYanYongShiTile.SetActive(true);
            ShiYanScroeTile.SetActive(true);
            ShiYanScroeInfo.text = "";
            ShiYanTimerDateInfoText.text = "";
            ShiYanTimerInfoText.text = "";
            ShiYanYongShiInfo.text = "";
            LanXiTipObj.SetActive(true);
            ZiCeTipObj.SetActive(false);
            CountBtn.GetComponent<YearMonthDatePointer>().enabled = false;
            CountBtnOneChildObj.SetActive(true);
            CountBtnText.color = SeColor;
            YearBtnOneChildObj.SetActive(false);
            YearBtnText.color = DeColor;
            MonthBtnOneChildObj.SetActive(false);
            MonthBtnText.color = DeColor;
            DateBtnOneChildObj.SetActive(false);
            DateBtnText.color = DeColor;
            if (ShiYanCeShiXiangQingMainWindowData.Instance.IsNoJiLuData)
            {
                JiLuShuJuPanel.SetActive(true);
                NoJiLuShuJuPanel.SetActive(false);
                GetShiYanXiangQingJiLuCountInfo(ShiYanCeShiXiangQingMainWindowData.Instance.GetInstanceId);
            }
            else
            {
                JiLuShuJuPanel.SetActive(false);
                NoJiLuShuJuPanel.SetActive(true);
            }
        }

        //点击日
        private void DateOnClick()
        {
            CountBtn.GetComponent<YearMonthDatePointer>().enabled = true;
            YearBtn.GetComponent<YearMonthDatePointer>().enabled = true;
            MonthBtn.GetComponent<YearMonthDatePointer>().enabled = true;
            DateBtn.GetComponent<YearMonthDatePointer>().enabled = false;
            s_currentSearch_mode = 1;
            ShiYanCeShiXiangQingJiLuWindowData.Instance.IsClickCount = false;
            limit = 10;
            YearMonthDateOnClick();
            LanXiTipObj.SetActive(false);
            ZiCeTipObj.SetActive(true);
            DateBtnOneChildObj.SetActive(true);
            DateBtnText.color = SeColor;
            GetShiYanXiangQingJiLuYearMonthDateInfo(ShiYanCeShiXiangQingMainWindowData.Instance.GetInstanceId, 1);
        }

        //点击月
        private void MonthOnClick()
        {
            CountBtn.GetComponent<YearMonthDatePointer>().enabled = true;
            YearBtn.GetComponent<YearMonthDatePointer>().enabled = true;
            MonthBtn.GetComponent<YearMonthDatePointer>().enabled = false;
            DateBtn.GetComponent<YearMonthDatePointer>().enabled = true;
            s_currentSearch_mode = 2;
            ShiYanCeShiXiangQingJiLuWindowData.Instance.IsClickCount = false;
            limit = 12;
            YearMonthDateOnClick();
            LanXiTipObj.SetActive(false);
            ZiCeTipObj.SetActive(true);
            MonthBtnOneChildObj.SetActive(true);
            MonthBtnText.color = SeColor;
            NextPageBtn.interactable = false;
            PrePageBtn.interactable = false;
            NextPageBtnText.color = PageDisableColor;
            PrePageBtnText.color = PageDisableColor;
            GetShiYanXiangQingJiLuYearMonthDateInfo(ShiYanCeShiXiangQingMainWindowData.Instance.GetInstanceId, 2);
        }

        //点击年
        private void YearOnClick()
        {
            CountBtn.GetComponent<YearMonthDatePointer>().enabled = true;
            YearBtn.GetComponent<YearMonthDatePointer>().enabled = false;
            MonthBtn.GetComponent<YearMonthDatePointer>().enabled = true;
            DateBtn.GetComponent<YearMonthDatePointer>().enabled = true;
            s_currentSearch_mode = 3;
            ShiYanCeShiXiangQingJiLuWindowData.Instance.IsClickCount = false;
            limit = 10;
            YearMonthDateOnClick();
            LanXiTipObj.SetActive(false);
            ZiCeTipObj.SetActive(true);
            YearBtnOneChildObj.SetActive(true);
            YearBtnText.color = SeColor;
            GetShiYanXiangQingJiLuYearMonthDateInfo(ShiYanCeShiXiangQingMainWindowData.Instance.GetInstanceId, 3);
        }

        //点击次数按钮
        private void CountOnClick()
        {
            CountBtn.GetComponent<YearMonthDatePointer>().enabled = false;
            YearBtn.GetComponent<YearMonthDatePointer>().enabled = true;
            MonthBtn.GetComponent<YearMonthDatePointer>().enabled = true;
            DateBtn.GetComponent<YearMonthDatePointer>().enabled = true;
            ClearData();
            s_currentSearch_mode = 0;
            ShiYanCeShiXiangQingJiLuWindowData.Instance.IsClickCount = true;
            limit = 10;
            ShiYanPingJunScroeTileTongJi.SetActive(false);
            ShiYanPingJunYongShiTileTongJi.SetActive(false);
            ShiYanXunLianCiShuTileTongJi.SetActive(false);
            ShiYanTimerTileText.SetActive(true);
            ShiYanYongShiTile.SetActive(true);
            ShiYanScroeTile.SetActive(true);
            ShiYanScroeInfo.text = "";
            ShiYanTimerDateInfoText.text = "";
            ShiYanTimerInfoText.text = "";
            ShiYanYongShiInfo.text = "";
            ChaKanMingXiBtn.gameObject.SetActive(true);
            CountBtnOneChildObj.SetActive(true);
            CountBtnText.color = SeColor;
            YearBtnOneChildObj.SetActive(false);
            YearBtnText.color = DeColor;
            MonthBtnOneChildObj.SetActive(false);
            MonthBtnText.color = DeColor;
            DateBtnOneChildObj.SetActive(false);
            DateBtnText.color = DeColor;
            LanXiTipObj.SetActive(true);
            ZiCeTipObj.SetActive(false);
            GetShiYanXiangQingJiLuCountInfo(ShiYanCeShiXiangQingMainWindowData.Instance.GetInstanceId);
        }

        //查看明细
        private void ChaKanMingXiOnClick()
        {
            WindowManager.Hide("ShiYanCeShiXiangQingJiLuWindow");
            WindowManager.Open<ShiYanCeShiXiangQingJiLuDaTiWindow>();
        }

        //返回
        private void ReturnOnClick()
        {
            ShiYanCeShiXiangQingJiLuWindowData.Instance.TempSelectNodeObj = null;
            ShiYanCeShiXiangQingJiLuWindowData.Instance.SelectLien = null;
            WindowManager.Close("ShiYanCeShiXiangQingJiLuWindow");
        }

        //点击日，月，年
        private void YearMonthDateOnClick() 
        {
            ClearData();
            ShiYanPingJunScroeTileTongJi.SetActive(true);
            ShiYanPingJunYongShiTileTongJi.SetActive(true);
            ShiYanXunLianCiShuTileTongJi.SetActive(true);
            ShiYanTimerTileText.SetActive(false);
            ShiYanYongShiTile.SetActive(false);
            ShiYanScroeTile.SetActive(false);
            ShiYanScroeInfo.text = "";
            ShiYanTimerDateInfoText.text = "";
            ShiYanTimerInfoText.text = "";
            ShiYanYongShiInfo.text = "";
            ChaKanMingXiBtn.gameObject.SetActive(false);
            CountBtnOneChildObj.SetActive(false);
            CountBtnText.color = DeColor;
            YearBtnOneChildObj.SetActive(false);
            YearBtnText.color = DeColor;
            MonthBtnOneChildObj.SetActive(false);
            MonthBtnText.color = DeColor;
            DateBtnOneChildObj.SetActive(false);
            DateBtnText.color = DeColor;
            LanXiTipObj.SetActive(false);
            ZiCeTipObj.SetActive(true);
        }

        private void ClearData() 
        {
            LoadCompleteCount = 0;
            isLoadComplete = true;
            foreach (var item in ShiYanCeShiXiangQingJiLuWindowData.Instance.ZheXianItemsDic.Keys)
            {
                Destroy(item);
            }
            ShiYanCeShiXiangQingJiLuWindowData.Instance.ZheXianItemsDic.Clear();
            line.positionCount = 0;
            ZheXianTuRect.GetComponent<HorizontalLayoutGroup>().enabled = true;
            s_currentPage = 1;
            s_totalCount = 0;
        }

        //获取实验详情记录弹窗的次数信息
        private void GetShiYanXiangQingJiLuCountInfo(string instanceId) 
        {
            client.EndPoint = CommonConstant.SERVER_URL_MEC_PROXY;
            client.Method = EnumHttpVerb.POST;
            ShiYanCeShiXiangQingJiLuRequest yanCeShiXiangQingJiLuRequest = new ShiYanCeShiXiangQingJiLuRequest();
            yanCeShiXiangQingJiLuRequest.instanceId = instanceId;
            yanCeShiXiangQingJiLuRequest.page = s_currentPage;
            yanCeShiXiangQingJiLuRequest.size = limit;
            yanCeShiXiangQingJiLuRequest.search_mode = 0;
            MacInfoRequest macInfo = new MacInfoRequest();
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
            yanCeShiXiangQingJiLuRequest.macInfo = macInfo;
            client.PostData = JsonUtility.ToJson(yanCeShiXiangQingJiLuRequest);
            string result = client.HttpRequest(CommonConstant.GET_SHIYANXIANGQINGCESHIJILU);
            Debug.Log("实验次数:" + result);
            if (string.IsNullOrEmpty(result))
            {
                Debug.Log(client.ErrorMessage);
                return;
            }
            else
            {
                ShiYanCeShiXaingQingJiLuResult shiYanCeShiXaingQingJiLuResult = new ShiYanCeShiXaingQingJiLuResult();
                shiYanCeShiXaingQingJiLuResult = JsonUtility.FromJson<ShiYanCeShiXaingQingJiLuResult>(result);
                ShiYanCeShiXiangQingJiLuWindowData.Instance.SaveShiYanCeShiXaingQingJiLuResult(shiYanCeShiXaingQingJiLuResult);
                StartCoroutine(ShowItem(shiYanCeShiXaingQingJiLuResult));
            }
        }

        //请求年月日的信息
        private void GetShiYanXiangQingJiLuYearMonthDateInfo(string instanceId,int search_mode) 
        {
            client.EndPoint = CommonConstant.SERVER_URL_MEC_PROXY;
            client.Method = EnumHttpVerb.POST;
            ShiYanCeShiXiangQingJiLuRequest yanCeShiXiangQingJiLuRequest = new ShiYanCeShiXiangQingJiLuRequest();
            yanCeShiXiangQingJiLuRequest.instanceId = instanceId;
            yanCeShiXiangQingJiLuRequest.page = s_currentPage;
            yanCeShiXiangQingJiLuRequest.size = limit;
            yanCeShiXiangQingJiLuRequest.search_mode = search_mode;
            MacInfoRequest macInfo = new MacInfoRequest();
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
            yanCeShiXiangQingJiLuRequest.macInfo = macInfo;
            client.PostData = JsonUtility.ToJson(yanCeShiXiangQingJiLuRequest);
            string result = client.HttpRequest(CommonConstant.GET_SHIYANXIANGQINGCESHIJILU);
            if (string.IsNullOrEmpty(result))
            {
                Debug.Log(client.ErrorMessage);
                return;
            }
            else
            {
                ShiYanCeShiXiangQingJiLuYearResult yanCeShiXiangQingJiLuYearResult = new ShiYanCeShiXiangQingJiLuYearResult();
                yanCeShiXiangQingJiLuYearResult = JsonUtility.FromJson<ShiYanCeShiXiangQingJiLuYearResult>(result);
                ShiYanCeShiXiangQingJiLuWindowData.Instance.SaveShiYanCeShiXiangQingJiLuYearResult(yanCeShiXiangQingJiLuYearResult);
                StartCoroutine(ShowYearMonthDateItem(yanCeShiXiangQingJiLuYearResult, search_mode));
            }
        }

        //显示选中次数节点的的数据
        public void ShowSerisData(ShiYanCeShiXaingQingJiLuResult yanCeShiXaingQingJiLuResult, int index) 
        {
            EntryId = yanCeShiXaingQingJiLuResult.data.items[index].entryId;
            TrainingMode = yanCeShiXaingQingJiLuResult.data.items[index].trainingMode;
            if (yanCeShiXaingQingJiLuResult.data.items[index].trainingMode == 0)
            {
                LianXiTipsImage.SetActive(true);
                ZiCeLianTipsImage.SetActive(false);
            }
            else if (yanCeShiXaingQingJiLuResult.data.items[index].trainingMode == 1)
            {
                LianXiTipsImage.SetActive(false);
                ZiCeLianTipsImage.SetActive(true);
            }
            string[] splits = yanCeShiXaingQingJiLuResult.data.items[index].testTime.Split(' ');
            ShiYanTimerDateInfoText.text = splits[0];
            ShiYanTimerInfoText.text = splits[1];
            ShiYanScroeInfo.text = yanCeShiXaingQingJiLuResult.data.items[index].score + "分";
            if ((int)yanCeShiXaingQingJiLuResult.data.items[index].duration / 60 == 0)
            {
                ShiYanYongShiInfo.text = yanCeShiXaingQingJiLuResult.data.items[index].duration + "秒";
            }
            else
            {
                ShiYanYongShiInfo.text = ((int)yanCeShiXaingQingJiLuResult.data.items[index].duration / 60) + "分" + decimal.Round((yanCeShiXaingQingJiLuResult.data.items[index].duration % 60),0) + "秒";
            }
        }

        //显示日，月，年的信息
        public IEnumerator ShowShiYanXiangQingDateData(ShiYanCeShiXiangQingJiLuYearResult yanCeShiXiangQingJiLuYearResult,int search_mode) 
        {
            ShiYanPingJunScroeInfo.text = "";
            ShiYanPingJunYongShiInfo.text = "";
            ShiYanXunLianCiShuInfo.text = "";
            if (search_mode == 2)
            {
                line.positionCount = yanCeShiXiangQingJiLuYearResult.data.items.Count;
                float with = 794f / line.positionCount;
                for (int i = 0; i < yanCeShiXiangQingJiLuYearResult.data.items.Count; i++)
                {
                    string recordTime = yanCeShiXiangQingJiLuYearResult.data.items[i].recordTime;
                    float avgScore = yanCeShiXiangQingJiLuYearResult.data.items[i].avgScore;
                    int count = yanCeShiXiangQingJiLuYearResult.data.items[i].times;
                    float avgDuration = yanCeShiXiangQingJiLuYearResult.data.items[i].avgDuration;

                    GameObject go = ResManager.Instance.Load<GameObject>(ItemType.Item.ZheXianItem);
                    GameObject item = Instantiate(go);
                    item.transform.parent = ZheXianTuRect.transform;
                    item.transform.localPosition = Vector3.zero;
                    item.transform.localEulerAngles = Vector3.zero;
                    item.transform.localScale = Vector3.one;
                    item.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(with, 324f);
                    float heigth = 32.4f * avgScore;
                    item.transform.GetChild(0).localPosition = new Vector3(0, heigth, 0);
                    item.transform.GetChild(1).GetComponent<Text>().text = recordTime;
                    ShiYanCeShiXiangQingJiLuWindowData.Instance.ZheXianItemsDic.Add(item, i);
                    //显示第一个的信息
                    if (i == 0)
                    {
                        ShiYanPingJunScroeInfo.text = avgScore + "分";
                        if ((int)avgDuration / 60 == 0)
                        {
                            ShiYanPingJunYongShiInfo.text = avgDuration + "秒";
                        }
                        else 
                        {
                            ShiYanPingJunYongShiInfo.text = ((int)avgDuration / 60) + "分"+ Math.Round((avgDuration % 60),0) + "秒";
                        }
                        ShiYanXunLianCiShuInfo.text = count + "次";
                    }
                }
                yield return new WaitForSeconds(0f);
            }
            else
            {
                if (yanCeShiXiangQingJiLuYearResult.data.total % limit != 0)
                {
                    s_totalCount = Convert.ToInt32(yanCeShiXiangQingJiLuYearResult.data.total / limit) + 1;
                }
                else
                {
                    s_totalCount = Convert.ToInt32(yanCeShiXiangQingJiLuYearResult.data.total / limit);
                }
                if (s_totalCount == 0)
                {
                    NextPageBtn.interactable = false;
                    PrePageBtn.interactable = false;
                    NextPageBtnText.color = PageDisableColor;
                    PrePageBtnText.color = PageDisableColor;
                }
                if (s_currentPage < s_totalCount)
                {
                    if (s_currentPage == 1)
                    {
                        NextPageBtn.interactable = true;
                        PrePageBtn.interactable = false;
                        NextPageBtnText.color = DeColor;
                        PrePageBtnText.color = PageDisableColor;
                    }
                    else
                    {
                        NextPageBtn.interactable = true;
                        PrePageBtn.interactable = true;
                        NextPageBtnText.color = DeColor;
                        PrePageBtnText.color = DeColor;
                    }
                }
                else if (s_currentPage == s_totalCount && s_currentPage > 1)
                {
                    NextPageBtn.interactable = false;
                    PrePageBtn.interactable = true;
                    NextPageBtnText.color = PageDisableColor;
                    PrePageBtnText.color = DeColor;
                }
                else if (s_currentPage == s_totalCount && s_currentPage == 1)
                {
                    NextPageBtn.interactable = false;
                    PrePageBtn.interactable = false;
                    NextPageBtnText.color = PageDisableColor;
                    PrePageBtnText.color = PageDisableColor;
                }
                line.positionCount = yanCeShiXiangQingJiLuYearResult.data.items.Count;
                float with = 794f / line.positionCount;
                for (int i = 0; i < yanCeShiXiangQingJiLuYearResult.data.items.Count; i++)
                {
                    string recordTime = yanCeShiXiangQingJiLuYearResult.data.items[i].recordTime;
                    float avgScore = yanCeShiXiangQingJiLuYearResult.data.items[i].avgScore;
                    int count = yanCeShiXiangQingJiLuYearResult.data.items[i].times;
                    float avgDuration = yanCeShiXiangQingJiLuYearResult.data.items[i].avgDuration;

                    GameObject go = ResManager.Instance.Load<GameObject>(ItemType.Item.ZheXianItem);
                    GameObject item = Instantiate(go);
                    item.transform.parent = ZheXianTuRect.transform;
                    item.transform.localPosition = Vector3.zero;
                    item.transform.localEulerAngles = Vector3.zero;
                    item.transform.localScale = Vector3.one;
                    item.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(with, 324f);
                    float heigth = 32.4f * avgScore;
                    item.transform.GetChild(0).localPosition = new Vector3(0, heigth, 0);
                    item.transform.GetChild(1).GetComponent<Text>().text = recordTime;
                    ShiYanCeShiXiangQingJiLuWindowData.Instance.ZheXianItemsDic.Add(item, i);
                    LoadCompleteCount += 1;
                    //显示第一个的信息
                    if (i == 0)
                    {
                        ShiYanPingJunScroeInfo.text = avgScore + "分";
                        if ((int)avgDuration / 60 == 0)
                        {
                            ShiYanPingJunYongShiInfo.text = avgDuration + "秒";
                        }
                        else
                        {
                            ShiYanPingJunYongShiInfo.text = ((int)avgDuration / 60) + "分" + Math.Round((avgDuration % 60), 0) + "秒";
                        }
                        ShiYanXunLianCiShuInfo.text = count + "次";
                    }
                    if (LoadCompleteCount == yanCeShiXiangQingJiLuYearResult.data.items.Count)
                    {
                        LoadCompleteCount = 0;
                        isLoadComplete = true;
                    }
                }
                yield return new WaitForSeconds(0f);
            }
        }

        //年，月，日的情况下射线移入需要显示的数据
        public void ShowSerisDateDataInfo(ShiYanCeShiXiangQingJiLuYearResult yanCeShiXiangQingJiLuYearResult, int index) 
        {
            ShiYanPingJunScroeInfo.text = yanCeShiXiangQingJiLuYearResult.data.items[index].avgScore + "分";
            if ((int)yanCeShiXiangQingJiLuYearResult.data.items[index].avgDuration / 60 == 0)
            {
                ShiYanPingJunYongShiInfo.text = yanCeShiXiangQingJiLuYearResult.data.items[index].avgDuration + "秒";
            }
            else
            {
                ShiYanPingJunYongShiInfo.text = ((int)yanCeShiXiangQingJiLuYearResult.data.items[index].avgDuration / 60) + "分" +Math.Round((yanCeShiXiangQingJiLuYearResult.data.items[index].avgDuration % 60),0) + "秒";
            }
            ShiYanXunLianCiShuInfo.text = yanCeShiXiangQingJiLuYearResult.data.items[index].times + "次";
        }

        //显示年月日的Item
        private IEnumerator ShowYearMonthDateItem(ShiYanCeShiXiangQingJiLuYearResult yanCeShiXiangQingJiLuYearResult,int search_mode)
        {
            yield return StartCoroutine(ShowShiYanXiangQingDateData(yanCeShiXiangQingJiLuYearResult,search_mode));
            ZheXianTuRect.GetComponent<HorizontalLayoutGroup>().enabled = false;
            yield return new WaitForSeconds(0.01f);
            foreach (var item in ShiYanCeShiXiangQingJiLuWindowData.Instance.ZheXianItemsDic.Keys)
            {
                line.SetPosition(ShiYanCeShiXiangQingJiLuWindowData.Instance.ZheXianItemsDic[item], item.transform.GetChild(0).transform.position);
            }
        }

        //获取保存的信息
        public void GetSelectNodeData() 
        {
            ShiYanYongShiInfo.text = ShiYanCeShiXiangQingJiLuWindowData.Instance.GetShiYanYongShiInfo;
            ShiYanTimerDateInfoText.text = ShiYanCeShiXiangQingJiLuWindowData.Instance.GetShiYanTimeDateInfo;
            ShiYanTimerInfoText.text = ShiYanCeShiXiangQingJiLuWindowData.Instance.GetShiYanTimeInfo;
            ShiYanScroeInfo.text = ShiYanCeShiXiangQingJiLuWindowData.Instance.GetShiYanScroeInfo;
            TrainingMode = ShiYanCeShiXiangQingJiLuWindowData.Instance.GetTrainingMode;
            if (TrainingMode == 0)
            {
                LianXiTipsImage.SetActive(true);
                ZiCeLianTipsImage.SetActive(false);
            }
            else if (TrainingMode == 1)
            {
                LianXiTipsImage.SetActive(false);
                ZiCeLianTipsImage.SetActive(true);
            }
        }
        //加载次数的数据点
        private IEnumerator LoadItem(ShiYanCeShiXaingQingJiLuResult yanCeShiXaingQingJiLuResult) 
        {
            ShiYanTimerDateInfoText.text = "";
            ShiYanTimerInfoText.text = "";
            ShiYanScroeInfo.text = "";
            ShiYanYongShiInfo.text = "";
            LianXiTipsImage.SetActive(false);
            ZiCeLianTipsImage.SetActive(false);
            if (yanCeShiXaingQingJiLuResult.data.total % limit != 0)
            {
                s_totalCount = Convert.ToInt32(yanCeShiXaingQingJiLuResult.data.total / limit) + 1;
            }
            else
            {
                s_totalCount = Convert.ToInt32(yanCeShiXaingQingJiLuResult.data.total / limit);
            }
            if (s_totalCount == 0)
            {
                NextPageBtn.interactable = false;
                PrePageBtn.interactable = false;
                NextPageBtnText.color = PageDisableColor;
                PrePageBtnText.color = PageDisableColor;
            }
            if (s_currentPage < s_totalCount)
            {
                if (s_currentPage == 1)
                {
                    NextPageBtn.interactable = true;
                    PrePageBtn.interactable = false;
                    NextPageBtnText.color = DeColor;
                    PrePageBtnText.color = PageDisableColor;
                }
                else
                {
                    NextPageBtn.interactable = true;
                    PrePageBtn.interactable = true;
                    NextPageBtnText.color = DeColor;
                    PrePageBtnText.color = DeColor;
                }
            }
            else if (s_currentPage == s_totalCount && s_currentPage > 1)
            {
                NextPageBtn.interactable = false;
                PrePageBtn.interactable = true;
                NextPageBtnText.color = PageDisableColor;
                PrePageBtnText.color = DeColor;
            }
            else if (s_currentPage == s_totalCount && s_currentPage == 1)
            {
                NextPageBtn.interactable = false;
                PrePageBtn.interactable = false;
                NextPageBtnText.color = PageDisableColor;
                PrePageBtnText.color = PageDisableColor;
            }
            line.positionCount = yanCeShiXaingQingJiLuResult.data.items.Count;
            float with = 794f / yanCeShiXaingQingJiLuResult.data.items.Count;
            for (int i = 0; i < yanCeShiXaingQingJiLuResult.data.items.Count; i++)
            {
                GameObject go = ResManager.Instance.Load<GameObject>(ItemType.Item.ZheXianItem);
                GameObject item = Instantiate(go);
                item.transform.parent = ZheXianTuRect.transform;
                item.transform.localPosition = Vector3.zero;
                item.transform.localEulerAngles = Vector3.zero;
                item.transform.localScale = Vector3.one;
                item.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(with, 324f);
                float heigth = 32.4f * yanCeShiXaingQingJiLuResult.data.items[i].score;
                item.transform.GetChild(0).localPosition = new Vector3(0, heigth,0);
                item.transform.GetChild(1).GetComponent<Text>().text = yanCeShiXaingQingJiLuResult.data.items[i].testDate;
                //获取信息
                int trainingMode = yanCeShiXaingQingJiLuResult.data.items[i].trainingMode;
                string entryId = yanCeShiXaingQingJiLuResult.data.items[i].entryId;
                string testTime = yanCeShiXaingQingJiLuResult.data.items[i].testTime;
                float score = yanCeShiXaingQingJiLuResult.data.items[i].score;
                float duration = yanCeShiXaingQingJiLuResult.data.items[i].duration;

                item.GetComponent<Button>().onClick.AddListener(delegate () { ZheXianNodeDataOnClick(testTime, score, trainingMode, entryId, duration); });
                ShiYanCeShiXiangQingJiLuWindowData.Instance.ZheXianItemsDic.Add(item, i);
                yield return new WaitForSeconds(0f);
                LoadCompleteCount += 1;
                //默认显示第一条的数据
                if (i==0)
                {
                    ZheXianNodeDataOnClick(testTime, score, trainingMode, entryId, duration);
                    ShiYanCeShiXiangQingJiLuWindowData.Instance.TempSelectNodeObj = item.transform.GetChild(0).transform.GetChild(0).gameObject;
                    ShiYanCeShiXiangQingJiLuWindowData.Instance.SelectLien = item.transform.GetChild(2).gameObject;
                    ShiYanCeShiXiangQingJiLuWindowData.Instance.TempSelectNodeObj.SetActive(true);
                    ShiYanCeShiXiangQingJiLuWindowData.Instance.SelectLien.SetActive(true);
                }
                if (LoadCompleteCount == yanCeShiXaingQingJiLuResult.data.items.Count)
                {
                    LoadCompleteCount = 0;
                    isLoadComplete = true;
                }
            }
        }
        //显示次数的Item
        private IEnumerator ShowItem(ShiYanCeShiXaingQingJiLuResult yanCeShiXaingQingJiLuResult)
        {
            yield return StartCoroutine(LoadItem(yanCeShiXaingQingJiLuResult));
            ZheXianTuRect.GetComponent<HorizontalLayoutGroup>().enabled = false;
            yield return new WaitForSeconds(0.01f);
            foreach (var item in ShiYanCeShiXiangQingJiLuWindowData.Instance.ZheXianItemsDic.Keys)
            {
                line.SetPosition(ShiYanCeShiXiangQingJiLuWindowData.Instance.ZheXianItemsDic[item], item.transform.GetChild(0).transform.position);
            }
        }

        //点击选中的数据点
        private void ZheXianNodeDataOnClick(string testTime, float score, int trainingMode, string entryId, float duration)
        {
            string[] splits = testTime.Split(' ');
            ShiYanTimerDateInfoText.text = splits[0];
            ShiYanTimerInfoText.text = splits[1];
            ShiYanScroeInfo.text = score + "分";
            if ((int)duration / 60 == 0)
            {
                ShiYanYongShiInfo.text = duration + "秒";
            }
            else
            {
                ShiYanYongShiInfo.text = ((int)duration / 60) + "分" + Math.Round((duration % 60), 0) + "秒";
            }
            if (ShiYanCeShiXiangQingJiLuWindowData.Instance.IsClickCount)
            {
                ShiYanCeShiXiangQingJiLuWindowData.Instance.GetShiYanTimeDateInfo = ShiYanTimerDateInfoText.text;
                ShiYanCeShiXiangQingJiLuWindowData.Instance.GetShiYanTimeInfo = ShiYanTimerInfoText.text;
                ShiYanCeShiXiangQingJiLuWindowData.Instance.GetShiYanScroeInfo = ShiYanScroeInfo.text;
                ShiYanCeShiXiangQingJiLuWindowData.Instance.GetTrainingMode = trainingMode;
                ShiYanCeShiXiangQingJiLuWindowData.Instance.GetShiYanYongShiInfo = ShiYanYongShiInfo.text;
                ShiYanCeShiXiangQingJiLuDaTiWindowData.Instance.GetEntryID = entryId;
            }
        }
    }
}

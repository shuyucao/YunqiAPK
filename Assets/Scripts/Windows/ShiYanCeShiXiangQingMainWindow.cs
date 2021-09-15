using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using Assets.Scripts.Data;
using Assets.Scripts.Result;
using Assets.Scripts.Request;
using static Assets.Scripts.Tool.RestFulClient;
using Assets.Scripts.Constant;
using Assets.Scripts.Manager;
using UnityEngine.UI;
using Assets.Scripts.Config;
using DG.Tweening;
using XCharts.Examples;
using XCharts;
using Pvr_UnitySDKAPI;
using UnityThreading;

namespace Assets.Scripts.Windows 
{
    public class ShiYanCeShiXiangQingMainWindow : BaseWindow
    {
        RestClient client = new RestClient();

        //标题
        private Text TileNameTextXq;

        //标题图片
        private Image TileNameImageXq;

        //返回按钮
        private Button ReturnXq;

        //自测按钮
        private Button ZiCeBtn;

        //练习按钮
        private Button LianXiBtn;

        //测试进度的圆形进度条
        private Image ProcessImageXq;
        private Text ProcessTextXq;

        //实验介绍的文字
        private Text InfoTextXq;

        //分割线
        private GameObject FengeXianXQ;

        //图标物体
        private GameObject LogoObj;

        //左边显示测试课程的界面
        private GameObject LeftScrollPanelXq;
        private GameObject LeftContentView;

        //右边显示实验介绍及原理的
        private GameObject RightScrollPanelXq;
        private GameObject JIeShaoInfoViewContent;

        //滑动条
        private GameObject ScrollbarXq;

        //加载中物体
        private GameObject ShiYanCeShiXqLoadingObj;

        //加载失败物体
        private GameObject ShiYanCeShiXqLoadingFaildObj;
        private Button LoadingFaildBtn;

        //雷达图
        private GameObject LeiDaTuObj;

        //掌握程度的数据文本
        private GameObject ZhangWoChengDuObj;
        private Text ZhangWoChengDuNumText;

        //实验时长的查看详情按钮
        private GameObject ShiChangObj;
        private Button ShiChangXiangBtn;
        private Text ShiChangNumText;

        //易错项的查看详情按钮
        private GameObject YiCuoXiangObj;
        private Button YiCuoXiangBtn;
        private Text YiCuoNumText;

        //雷达图
        private RadarChart radarChart;

        //存储实验目的和实验原理的
        private List<GameObject> ShiYanMuDeAndYuanLiItems = new List<GameObject>();

        //存储实验科目的
        private List<GameObject> ShiYanCeShiKeMuLists = new List<GameObject>();

        ShiYanCeShiNeRongItemStudyResult yanCeShiNeRongItemStudyResult = null;

        private int LoadCompleteCount = 0;
        public override void OnInit()
        {
            base.OnInit();
            prefabType = PrefabType.Window.ShiYanCeShiXiangQingMainWindow;
            prefab = gameObject;
        }

        public override void Init()
        {
            base.Init();
            InitData();
            DefulatSetting();
            GetShiYanCeshiLeftAndCenterInfo();
        }

        private void InitData() 
        {
            TileNameTextXq = GameObject.Find("TileNameTextXQ").GetComponent<Text>();
            TileNameImageXq = GameObject.Find("TileNameImageXQ").GetComponent<Image>();
            ReturnXq = GameObject.Find("ReturnXQ").GetComponent<Button>();
            ZiCeBtn = GameObject.Find("ZiCeBtn").GetComponent<Button>();
            LianXiBtn = GameObject.Find("LianXiBtn").GetComponent<Button>();
            ProcessImageXq = GameObject.Find("ProcessImageXQ").GetComponent<Image>();
            ProcessTextXq = GameObject.Find("ProcessTextXQ").GetComponent<Text>();
            InfoTextXq = GameObject.Find("InfoTextXQ").GetComponent<Text>();
            FengeXianXQ = GameObject.Find("fengeXianXQ");
            LogoObj = GameObject.Find("LogoImageXiangQing");
            LeftScrollPanelXq = GameObject.Find("LeftScrollPanelXQ");
            LeftContentView = LeftScrollPanelXq.transform.GetChild(0).gameObject;
            RightScrollPanelXq= GameObject.Find("RightScrollPanelXQ");
            JIeShaoInfoViewContent = RightScrollPanelXq.transform.GetChild(0).gameObject;
            ScrollbarXq = GameObject.Find("ScrollbarXQ");
            ShiYanCeShiXqLoadingObj = GameObject.Find("ShiYanCeShiXqLoadingBtn");
            ShiYanCeShiXqLoadingFaildObj = GameObject.Find("ShiYanCeShiXqLoadingFaildBtn");
            LoadingFaildBtn = ShiYanCeShiXqLoadingFaildObj.GetComponent<Button>();
            LeiDaTuObj = GameObject.Find("LeiDaTu");
            radarChart = LeiDaTuObj.GetComponent<RadarChart>(); 
            ZhangWoChengDuObj = GameObject.Find("ZhangWoChengDu");
            ZhangWoChengDuNumText= GameObject.Find("ZhangWoChengDuNum").GetComponent<Text>();

            ShiChangObj = GameObject.Find("ShiChang");
            ShiChangNumText = GameObject.Find("ShiChangNum").GetComponent<Text>();
            ShiChangXiangBtn= ShiChangObj.transform.GetChild(3).GetComponent<Button>();

            YiCuoXiangObj = GameObject.Find("YiCuoXiang");
            YiCuoNumText = GameObject.Find("YICuoNum").GetComponent<Text>();
            YiCuoXiangBtn = YiCuoXiangObj.transform.GetChild(3).GetComponent<Button>();

            ReturnXq.onClick.AddListener(ReturnXqOnClick);
            LoadingFaildBtn.onClick.AddListener(LoadingFaildReLoadOnClick);
        }

        //加载失败点击重新加载
        private void LoadingFaildReLoadOnClick()
        {
            //判断网络
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                ShiYanCeShiXqLoadingFaildObj.SetActive(true);
                ShiYanCeShiXqLoadingObj.SetActive(false);
                return;
            }
            else
            {
                for (int i = 0; i < ShiYanCeShiKeMuLists.Count; i++)
                {
                    Destroy(ShiYanCeShiKeMuLists[i]);
                }
                ShiYanCeShiKeMuLists.Clear();
                ShiYanCeShiXqLoadingFaildObj.SetActive(false);
                ShiYanCeShiXqLoadingObj.SetActive(true);
                GetShiYanCeshiLeftAndCenterInfo();
            }
        }
        //返回
        private void ReturnXqOnClick()
        {
            WindowManager.ReShow();
            WindowManager.Close("ShiYanCeShiXiangQingMainWindow");
        }

        private void DefulatSetting()
        {
            yanCeShiNeRongItemStudyResult = null;
            ShiYanCeShiKeMuLists.Clear();
            ProcessImageXq.fillAmount = 0;
            ProcessTextXq.text = "0%";
            InfoTextXq.text = "";
            TileNameTextXq.text = "";
            LogoObj.SetActive(true);
            FengeXianXQ.SetActive(false);
            RightScrollPanelXq.SetActive(false);
            ScrollbarXq.SetActive(false);
            ZiCeBtn.gameObject.SetActive(false);
            LianXiBtn.gameObject.SetActive(false);
            ShiYanCeShiXqLoadingFaildObj.SetActive(false);
            ShiYanCeShiXqLoadingObj.SetActive(true);
            ZhangWoChengDuNumText.text = "0<size=30>%</size>";
            ShiChangNumText.text = "0<size=30>h</size>";
            YiCuoNumText.text = "0<size=30>个</size>";
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                ShiYanCeShiXqLoadingFaildObj.SetActive(true);
                ShiYanCeShiXqLoadingObj.SetActive(false);
            }
            for (int i = 0; i < ShiYanCeShiKeMuLists.Count; i++)
            {
                Destroy(ShiYanCeShiKeMuLists[i]);
            }
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

        private int createdElements = 0;
   
        //获取请求左边面板和实验Item信息
        private void GetShiYanCeshiLeftAndCenterInfo() 
        {
            if (ShiYanCeShiXiangQingMainWindowData.Instance.ShiYanCeShiId == "")
            {
                Debug.Log("没有该实验的ID");
                return;
            }
            client.EndPoint = CommonConstant.SERVER_URL_MEC_PROXY;
            client.Method = EnumHttpVerb.POST;
            ShiYanCeShiXiangQingMainRequest shiXiangQingMainRequest = new ShiYanCeShiXiangQingMainRequest();
            shiXiangQingMainRequest.testId = ShiYanCeShiXiangQingMainWindowData.Instance.ShiYanCeShiId;
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
            shiXiangQingMainRequest.macInfo = macInfo;
            client.PostData = JsonUtility.ToJson(shiXiangQingMainRequest);

            string result = client.HttpRequest(CommonConstant.GET_SHIYANCESHIINFO);
            Debug.Log("实验测试内容：" + result);
            if (string.IsNullOrEmpty(result))
            {
                Debug.Log(client.ErrorMessage);
                ShiYanCeShiXqLoadingFaildObj.SetActive(true);
                ShiYanCeShiXqLoadingObj.SetActive(false);
                return;
            }
            else
            {
                ShiYanCeShiNeRongOneResult shiYanCeShiNeRongOne = new ShiYanCeShiNeRongOneResult();
                shiYanCeShiNeRongOne = JsonUtility.FromJson<ShiYanCeShiNeRongOneResult>(result);
                ShiYanCeShiXiangQingMainWindowData.Instance.SaveShiYanCeShiNeRongOneResult(shiYanCeShiNeRongOne);
                ShowTileNameOne(shiYanCeShiNeRongOne);
            }
        }
       
        private void ShowTileNameOne(ShiYanCeShiNeRongOneResult shiYanCeShiNeRongOne)
        {
            TileNameTextXq.text = shiYanCeShiNeRongOne.data.activity_set_name;
            SetTileImageSize(TileNameTextXq);
            ProcessTextXq.text = (float.Parse(shiYanCeShiNeRongOne.data.testRate) * 100).ToString() + "%";
            ProcessImageXq.DOFillAmount((float.Parse(shiYanCeShiNeRongOne.data.testRate)), 1f);
            if (string.IsNullOrEmpty(shiYanCeShiNeRongOne.data.introduction))
            {
                //如果为空，不显示
                InfoTextXq.transform.parent.gameObject.SetActive(false);
            }
            else
            {
                InfoTextXq.transform.parent.gameObject.SetActive(true);
                InfoTextXq.text = shiYanCeShiNeRongOne.data.introduction;
            }
            LoadShiYanCeShiNeRongItemOne(shiYanCeShiNeRongOne);
        }

        //自适应标题的长度
        private void SetTileImageSize(Text text) 
        {
            float with = LayoutUtility.GetPreferredWidth(text.GetComponent<RectTransform>());
            if (with <= 170)
            {
                return;
            }
            else
            {
                TileNameImageXq.GetComponent<RectTransform>().sizeDelta = new Vector2(with + 160, 62);
            }
        }

        //自适应Item的大小
        private void SetTileButtonSize(GameObject button,Text text)
        {
            float heigth = LayoutUtility.GetPreferredHeight(text.GetComponent<RectTransform>());
            if (heigth <= 24)
            {
                return;
            }
            else
            {
                button.GetComponent<RectTransform>().sizeDelta = new Vector2(400, heigth + 23);
            }
        }

        //加载实验测试的内容
        private void LoadShiYanCeShiNeRongItemOne(ShiYanCeShiNeRongOneResult shiYanCeShiNeRongOne)
        {
            LoadCompleteCount = 0;
            if (shiYanCeShiNeRongOne.data.node.Count != 0)
            {
                LogoObj.SetActive(false);
                FengeXianXQ.SetActive(true);
                RightScrollPanelXq.SetActive(true);
                ScrollbarXq.SetActive(true);
                ZiCeBtn.gameObject.SetActive(true);
                LianXiBtn.gameObject.SetActive(true);
                ShiYanCeShiXqLoadingFaildObj.SetActive(false);
                ShiYanCeShiXqLoadingObj.SetActive(true);
                LayoutRebuilder.ForceRebuildLayoutImmediate(JIeShaoInfoViewContent.GetComponent<RectTransform>());
            }
            else
            {
                LogoObj.SetActive(true);
                FengeXianXQ.SetActive(false);
                RightScrollPanelXq.SetActive(false);
                ScrollbarXq.SetActive(false);
                ZiCeBtn.gameObject.SetActive(false);
                LianXiBtn.gameObject.SetActive(false);
                ShiYanCeShiXqLoadingFaildObj.SetActive(false);
                ShiYanCeShiXqLoadingObj.SetActive(false);
                return;
            }
            CreatItemOne(shiYanCeShiNeRongOne);
        }

        void CreatItemOne(ShiYanCeShiNeRongOneResult shiYanCeShiNeRongOne)
        {
            for (int i = 0; i < shiYanCeShiNeRongOne.data.node.Count; i++)
            {
                GameObject go = ResManager.Instance.Load<GameObject>(ItemType.Item.ShiYanXiangQingItem);
                GameObject item = Instantiate(go);
                item.transform.SetParent(LeftContentView.transform);
                item.transform.localEulerAngles = Vector3.zero;
                item.transform.localPosition = Vector3.zero;
                item.transform.localScale = Vector3.one;
                ShiYanCeShiKeMuLists.Add(item);
                string name = shiYanCeShiNeRongOne.data.node[i].node_name;
                item.transform.GetChild(0).GetComponent<Text>().text = name;
                string node_id = shiYanCeShiNeRongOne.data.node[i].node_id;
                SetTileButtonSize(item, item.transform.GetChild(0).GetComponent<Text>());
                item.GetComponent<Button>().onClick.AddListener(delegate () { ShiYanCeShiKeliItemOneOnClick(name, item, node_id); });
                if (i == 0)
                {
                    ShiYanCeShiKeliItemOneOnClick(name, item, node_id);
                }
                System.Threading.Interlocked.Increment(ref createdElements);
                LoadCompleteCount += 1;
                if (LoadCompleteCount == shiYanCeShiNeRongOne.data.node.Count)
                {
                    ShiYanCeShiXqLoadingObj.SetActive(false);
                }
            }
        }

        //点击生成的实验测试的Item
        private void ShiYanCeShiKeliItemOneOnClick(string name, GameObject item, string node_id)
        {
            yanCeShiNeRongItemStudyResult = null;
            for (int i = 0; i < ShiYanMuDeAndYuanLiItems.Count; i++)
            {
                DestroyImmediate(ShiYanMuDeAndYuanLiItems[i]);
            }
            SetAllNotSelect();
            item.transform.GetChild(2).gameObject.SetActive(true);
            item.transform.GetChild(2).GetChild(0).gameObject.SetActive(true);
            ShiYanMuDeAndYuanLiItems.Clear();
            ZiCeBtn.onClick.RemoveAllListeners();
            LianXiBtn.onClick.RemoveAllListeners();
            YiCuoXiangBtn.onClick.RemoveAllListeners();
            ShiChangXiangBtn.onClick.RemoveAllListeners();

            client.EndPoint = CommonConstant.SERVER_URL_MEC_PROXY;
            client.Method = EnumHttpVerb.POST;
            ShiYanCeShiXiangQingMainOneRequest shiYanCeShiXiangQingMainOneRequest = new ShiYanCeShiXiangQingMainOneRequest();
            shiYanCeShiXiangQingMainOneRequest.testId = ShiYanCeShiXiangQingMainWindowData.Instance.ShiYanCeShiId;
            shiYanCeShiXiangQingMainOneRequest.nodeId = node_id;
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
            shiYanCeShiXiangQingMainOneRequest.macInfo = macInfo;
            client.PostData = JsonUtility.ToJson(shiYanCeShiXiangQingMainOneRequest);
            string result = client.HttpRequest(CommonConstant.GET_SHIYANCESHIDETAILINFO);
            Debug.Log("实验测试详情内容：" + result);
            if (string.IsNullOrEmpty(result))
            {
                Debug.Log(client.ErrorMessage);
                return;
            }
            else
            {
                yanCeShiNeRongItemStudyResult = JsonUtility.FromJson<ShiYanCeShiNeRongItemStudyResult>(result);
                string src = yanCeShiNeRongItemStudyResult.data.testResult.src;
                string node_name = yanCeShiNeRongItemStudyResult.data.node_name;
                string instance_id = yanCeShiNeRongItemStudyResult.data.instance_id;
                for (int i = 0; i < 2; i++)
                {
                    GameObject go = ResManager.Instance.Load<GameObject>(ItemType.Item.XiangQiangJieShaoItem);
                    GameObject item1 = Instantiate(go);
                    item1.transform.parent = JIeShaoInfoViewContent.transform;
                    item1.transform.localPosition = Vector3.zero;
                    item1.transform.localEulerAngles = Vector3.zero;
                    item1.transform.localScale = Vector3.one;
                    ShiYanMuDeAndYuanLiItems.Add(item1);
                    if (i == 0)
                    {
                        item1.transform.GetChild(0).GetComponent<Text>().text = "实验目的";
                        item1.transform.GetChild(1).GetComponent<Text>().text = yanCeShiNeRongItemStudyResult.data.testResult.goal;
                    }
                    else
                    {
                        item1.transform.GetChild(0).GetComponent<Text>().text = "实验原理";
                        item1.transform.GetChild(1).GetComponent<Text>().text = yanCeShiNeRongItemStudyResult.data.testResult.theory;
                    }
                    LayoutRebuilder.ForceRebuildLayoutImmediate(JIeShaoInfoViewContent.GetComponent<RectTransform>());
                }
                ShowCeShiXiangQingInfoOne(yanCeShiNeRongItemStudyResult.data.study);
                ZiCeBtn.onClick.AddListener(delegate () { ZiCeXqOnClick(src, name); });
                LianXiBtn.onClick.AddListener(delegate () { LianXiXqClick(src, name); });
                YiCuoXiangBtn.onClick.AddListener(delegate () { YiCuoXiangXiangQingOnClick(instance_id); });
                ShiChangXiangBtn.onClick.AddListener(delegate () { ShiChangXiangXiangQingOnClick(instance_id); });
            }
        }

        //点击易错项里的查看详情按钮
        private void YiCuoXiangXiangQingOnClick(string instanceId)
        {
            ShiYanCeShiXiangQingMainWindowData.Instance.GetInstanceId = instanceId;
            WindowManager.Open<ShiYanCeShiYiCuoXiangWindow>();
        }

        //点击实验时长里的查看详情按钮
        private void ShiChangXiangXiangQingOnClick(string instanceId)
        {
            ShiYanCeShiXiangQingMainWindowData.Instance.GetInstanceId = instanceId;
            WindowManager.Open<ShiYanCeShiXiangQingJiLuWindow>();
        }

        //显示测试的雷达图，掌握程度，实验时长，易错项的信息
       // private void ShowCeShiXiangQingInfo(ShiYanCeShiNeRongNodeStudyDataResult study)
       // {
       //     //为true 显示上一次，false显示本次
       //     if (ShiYanCeShiXiangQingMainWindowData.Instance.IsLastShiYanCeShiMingXiInfo)
       //     {
       //         ZhangWoChengDuNumText.text =string.Format("{0}<size=30>%</size>",study.last_master_level*100);
       //         ShiChangNumText.text = string.Format("{0}<size=30>h</size>", Math.Round(study.duration / 3600, 1));
       //         YiCuoNumText.text =string.Format("{0}<size=30>个</size>",study.last_error_num);
       //     }
       //     else
       //     {
       //         ZhangWoChengDuNumText.text = string.Format("{0}<size=30>%</size>", study.master_level * 100);
       //         ShiChangNumText.text = string.Format("{0}<size=30>h</size>",Math.Round(study.duration/3600,1));
       //         YiCuoNumText.text = string.Format("{0}<size=30>个</size>", study.this_error_num);
       //     }
       //     radarChart.series.ClearData();
       //     radarChart.series.AddData("serie1", study.knowledge_ability);
       //     radarChart.series.AddData("serie1", study.skill_ability);
       //     radarChart.series.AddData("serie1", study.diligent_ability);
       //     radarChart.series.AddData("serie1", study.generalize_ability);
       //     radarChart.series.AddData("serie1", study.operation_ability);
       //     //如果易错项个数为0，不显示进入易错项人口，
       //     if (study.last_error_num == 0)
       //     {
       //         ShiYanCeShiXiangQingMainWindowData.Instance.IsNoYiCuoXiangData = false;
       //     }
       //     else
       //     {
       //         ShiYanCeShiXiangQingMainWindowData.Instance.IsNoYiCuoXiangData = true;
       //     }
       //     //如果实验时长为0，不显示进入实验时长入口，
       //     if (Math.Round(study.duration / 3600, 1) == 0f)
       //     {
       //         ShiYanCeShiXiangQingMainWindowData.Instance.IsNoJiLuData = false;
       //     }
       //     else
       //     {
       //         ShiYanCeShiXiangQingMainWindowData.Instance.IsNoJiLuData = true;
       //     }
       // }
        private void ShowCeShiXiangQingInfoOne(ShiYanCeShiNeRongStudyDataOneResult study)
        {
            //为true 显示上一次，false显示本次
            if (ShiYanCeShiXiangQingMainWindowData.Instance.IsLastShiYanCeShiMingXiInfo)
            {
                ZhangWoChengDuNumText.text = string.Format("{0}<size=30>%</size>", study.last_master_level * 100);
                ShiChangNumText.text = string.Format("{0}<size=30>h</size>", Math.Round(study.duration / 3600, 1));
                YiCuoNumText.text = string.Format("{0}<size=30>个</size>", study.last_error_num);
            }
            else
            {
                ZhangWoChengDuNumText.text = string.Format("{0}<size=30>%</size>", study.master_level * 100);
                ShiChangNumText.text = string.Format("{0}<size=30>h</size>", Math.Round(study.duration / 3600, 1));
                YiCuoNumText.text = string.Format("{0}<size=30>个</size>", study.this_error_num);
            }
            radarChart.series.ClearData();
            radarChart.series.AddData("serie1", study.knowledge_ability);
            radarChart.series.AddData("serie1", study.skill_ability);
            radarChart.series.AddData("serie1", study.diligent_ability);
            radarChart.series.AddData("serie1", study.generalize_ability);
            radarChart.series.AddData("serie1", study.operation_ability);
            //如果易错项个数为0，不显示进入易错项人口，
            if (study.last_error_num == 0)
            {
                ShiYanCeShiXiangQingMainWindowData.Instance.IsNoYiCuoXiangData = false;
            }
            else
            {
                ShiYanCeShiXiangQingMainWindowData.Instance.IsNoYiCuoXiangData = true;
            }
            //如果实验时长为0，不显示进入实验时长入口，
            if (Math.Round(study.duration / 3600, 1) == 0f)
            {
                ShiYanCeShiXiangQingMainWindowData.Instance.IsNoJiLuData = false;
            }
            else
            {
                ShiYanCeShiXiangQingMainWindowData.Instance.IsNoJiLuData = true;
            }
        }
        //练习
        private void LianXiXqClick(string src, string name)
        {
            Debug.Log("src :" + src);
            Debug.Log("name :" + name);
            UniversalLoadingWindowData.Instance.GetShiYanName = name;
            UniversalLoadingWindowData.Instance.GetShiYanURL = src;
            UniversalLoadingWindowData.Instance.GetPlayMode = ResPlayMode.VRPractice.ToString();
            UniversalLoadingWindowData.Instance.ResourcesType = "练习";
            WindowManager.Hide("ShiYanCeShiXiangQingMainWindow");
            WindowManager.Open<UniversalLoadingWindow>();
        }

        //自测
        private void ZiCeXqOnClick(string src, string name)
        {
            Debug.Log("src :" + src);
            Debug.Log("name :" + name);
            UniversalLoadingWindowData.Instance.GetShiYanName = name;
            UniversalLoadingWindowData.Instance.GetShiYanURL = src;
            UniversalLoadingWindowData.Instance.GetPlayMode = ResPlayMode.VRTest.ToString();
            UniversalLoadingWindowData.Instance.ResourcesType = "自测";
            WindowManager.Hide("ShiYanCeShiXiangQingMainWindow");
            WindowManager.Open<UniversalLoadingWindow>();
        }

        //设置为选中状态
        private void SetAllNotSelect() 
        {
            for (int i = 0; i < ShiYanCeShiKeMuLists.Count; i++)
            {
                ShiYanCeShiKeMuLists[i].transform.GetChild(2).gameObject.SetActive(false);
                ShiYanCeShiKeMuLists[i].transform.GetChild(2).GetChild(0).gameObject.SetActive(false);
            }
        }

        //通过手柄来滑动界面
        private void MovePanelByJoystick()
        {
            if (ShiYanXuanZeWindowData.Instance.TempObject == null)
            {
                return;
            }
            switch (Controller.UPvr_GetSwipeDirection(0))
            {
                case SwipeDirection.No:
                    break;
                case SwipeDirection.SwipeUp:
                    ShiYanXuanZeWindowData.Instance.TempObject.transform.Translate(Vector3.up * Time.deltaTime * 100);
                    break;
                case SwipeDirection.SwipeDown:
                    ShiYanXuanZeWindowData.Instance.TempObject.transform.Translate(Vector3.down * Time.deltaTime * 100);
                    break;
                case SwipeDirection.SwipeLeft:
                    ShiYanXuanZeWindowData.Instance.TempObject.transform.Translate(Vector3.left * Time.deltaTime * 100);
                    break;
                case SwipeDirection.SwipeRight:
                    ShiYanXuanZeWindowData.Instance.TempObject.transform.Translate(Vector3.right * Time.deltaTime * 100);
                    break;
                default:
                    break;
            }
            switch (Controller.UPvr_GetSwipeDirection(1))
            {
                case SwipeDirection.No:
                    break;
                case SwipeDirection.SwipeUp:
                    ShiYanXuanZeWindowData.Instance.TempObject.transform.Translate(Vector3.up * Time.deltaTime * 100);
                    break;
                case SwipeDirection.SwipeDown:
                    ShiYanXuanZeWindowData.Instance.TempObject.transform.Translate(Vector3.down * Time.deltaTime * 100);
                    break;
                case SwipeDirection.SwipeLeft:
                    ShiYanXuanZeWindowData.Instance.TempObject.transform.Translate(Vector3.left * Time.deltaTime * 100);
                    break;
                case SwipeDirection.SwipeRight:
                    ShiYanXuanZeWindowData.Instance.TempObject.transform.Translate(Vector3.right * Time.deltaTime * 100);
                    break;
                default:
                    break;
            }
        }

        public void LateUpdate()
        {
            //MovePanelByJoystick();
        }
    }
}


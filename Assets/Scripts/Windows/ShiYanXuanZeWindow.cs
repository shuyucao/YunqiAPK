using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Config;
using UnityEngine.UI;
using System;
using Assets.Scripts.Manager;
using Assets.Scripts.Tool;
using static Assets.Scripts.Tool.RestFulClient;
using Assets.Scripts.Constant;
using Assets.Scripts.Request;
using Assets.Scripts.Result;
using Assets.Scripts.Data;
using Pvr_UnitySDKAPI;

namespace Assets.Scripts.Windows
{
    public class ShiYanXuanZeWindow : BaseWindow
    {
        RestClient client = new RestClient();

        //教材管理按钮
        private Button textbookMgrBtn;

        //返回按钮
        private Button ReturnBtn;

        //教材的名称
        private Text JiaoCaiTileName;

        private Image JiaoCaiNameTileImag;

        //实验资源按钮
        private Toggle ShiYuanZiYuanBtn;

        //实验测试按钮
        private Toggle ShiYanCeShiBtn;

        //回到首页按钮
        private Button ReturnMainBtn;

        //背景图标
        private GameObject LogoObj;

        //分隔线
        private GameObject FenGeXian;

        //加载按钮
        private GameObject LoadingBtnObj;
        private Button LoadingBtn;
        private GameObject LoadingTxtObj;
        private GameObject LoadingFaildTxtObj;

        //章节选择下拉框
        private Dropdown ZhangJieXuanZeDropDown;
        private GameObject ZhangJieXuanZeDropDownPanel;

        //显示章节内容的Content
        private GameObject JiaoCaiNeRongContentObj;
        private ToggleGroup JiaoCaiNeRongContentTogg;
        private ScrollRect ScrollRectJiaoCaiNeRong;

        //显示实验介绍内容
        private GameObject ShiYanJieShaoPanel;
        private Text ShiYanJieShaoInfoText;
        //显示实验器材内容
        private Text ShiYanQIChaiInfoText;

        //无权限提示
        private GameObject NoQuanXianTipObj;

        //开始实验的按钮和物体
        private GameObject KaiShiYanBtnObj;
        private Button KaiShiYanBtn;


        //显示小的教材界面
        private GameObject JiaoCaiContentSmallObj;
        private GameObject JiaoCaiContentSmallViewObj;
        private ScrollRect ScrollRectSmall;

        //显示大的教材界面
        private GameObject JiaoCaiContentBigObj;
        private GameObject JiaoCaiContentBigViewObj;
        private ScrollRect ScrollRectBig;

        //未登录的界面
        private GameObject NoLoginStatusRightPanel;
        private Button RightLoginBtn;

        //登录状态的界面
        private GameObject LoginStatusRightPanel;
        private Image UserIcon;
        private Text UserNameTxt;
        private Text XueDuanXueNianTxt;
        private Button ChangeXueDuanXueNianBtn;

        //显示试验记录的面板
        private GameObject ShiYanJiLuContentObj;
        private GameObject ShiYanJiLuContentParentObj;
        private ToggleGroup ShiYanJiLuContentTogg;

        //无实验记录面板
        private GameObject NoShiYanJiLuPanelObj;

        //更多实验记录按钮
        private Button GengDuoJiLuBtn;

        //修改基础信息和退出登录的按钮
        private GameObject XiuGaiJIChuXinXiPanelObj;
        private Button JiChuXinXiChangeBtn;
        private Button LoginOutBtn;

        //显示选择章节的名称
        private Text ShowZhangJieNeRongTileName;
        public GameObject GetJiaoCaiContentSmallObj
        {
            get { return JiaoCaiContentSmallObj; }
        }

        public GameObject GetJiaoCaiContentBigObj
        {
            get { return JiaoCaiContentBigObj; }
        }

        //下拉框章节选择的名称列表
        private Dictionary<string, string> ZhangJieNameLists_Dic = new Dictionary<string, string>();
        private string zhangJieAll = "全部";

        //存储生成的小教材Item
        private Dictionary<string, GameObject> JiaoCaiSmallObj = new Dictionary<string, GameObject>();

        //存储生成的大教材Item
        private Dictionary<string, GameObject> JiaoCaiBigObj = new Dictionary<string, GameObject>();

        //存储获取的实验资源Item列表
        private List<GameObject> s_ShiYanWorkItemList = new List<GameObject>();

        //限制获取实验课程Item
        private int s_limitItem = 10;

        //当前页数
        private int s_currentPage = 1;

        //总页数
        private int s_totalPageCount = 0;

        //是否有网络
        private bool isNetworkReachability = true;

        //资源图片加载完成的个数
        private int LoadCommpleteCount = 0;

        private GameObject TempCeShiJiLuItem = null;

        //小教材的触发器
        private GameObject SmllTriggerImage;
        //大教材的触发器
        private GameObject BigeTriggerImage;

        public GameObject GetSmllTriggerImage 
        {
            get { return SmllTriggerImage; }
        }

        public GameObject GetBigeTriggerImage
        {
            get { return BigeTriggerImage; }
        }

        public override void OnInit()
        {
            base.OnInit();
            prefabType = PrefabType.Window.ShiYanXuanZeWindow;
            prefab = gameObject;
        }

        public override void Init()
        {
            base.Init();
            InitData();
            DefulatSetting();
        }

        //初始化窗口并绑定组件
        private void InitData()
        {
            SmllTriggerImage = GameObject.Find("SmllTriggerImage");
            BigeTriggerImage = GameObject.Find("BigeTriggerImage");
            TipChooseJiaoCai = GameObject.Find("TipsChooseJiaoCai");
            JiaoCaiNameTileImag = GameObject.Find("JiaoCaiNameTileImag").GetComponent<Image>();
            JiaoCaiTileName = GameObject.Find("JiaoCaiTileName").GetComponent<Text>();
            ShiYuanZiYuanBtn = GameObject.Find("ShiYanZiYuanXuanZeBtn").GetComponent<Toggle>();
            ShiYanCeShiBtn = GameObject.Find("ShiYanCeShiBtn").GetComponent<Toggle>();
            ReturnMainBtn = GameObject.Find("ReturnMainBtn").GetComponent<Button>();
            LoadingBtnObj = GameObject.Find("LoadingBtn");
            LoadingBtn = LoadingBtnObj.GetComponent<Button>();
            LoadingTxtObj = GameObject.Find("LoadingBtn").transform.GetChild(0).gameObject;
            LoadingFaildTxtObj = GameObject.Find("LoadingBtn").transform.GetChild(1).gameObject;
            ZhangJieXuanZeDropDownPanel = GameObject.Find("ZhangJIeXuanZeNameTxt");
            ZhangJieXuanZeDropDown = ZhangJieXuanZeDropDownPanel.transform.GetChild(0).GetComponent<Dropdown>();
            JiaoCaiNeRongContentObj = GameObject.Find("JiaoCaiNeRongContent");
            ScrollRectJiaoCaiNeRong = GameObject.Find("JiaoCaiNeRongPanel").GetComponent<ScrollRect>();
            ScrollRectJiaoCaiNeRong.onValueChanged.AddListener(ChangePageOnClick);

            JiaoCaiNeRongContentTogg = JiaoCaiNeRongContentObj.GetComponent<ToggleGroup>();
            ShiYanJieShaoPanel = GameObject.Find("ShiYanJieShaoPanel").transform.GetChild(0).gameObject;
            ShiYanJieShaoInfoText = GameObject.Find("ShiYanJieShaoInfoText").GetComponent<Text>();
            ShiYanQIChaiInfoText = GameObject.Find("ShiYanQIChaiInfoText").GetComponent<Text>();
            ShowZhangJieNeRongTileName = GameObject.Find("ZhangJieNeTileNameItem").GetComponent<Text>();
            NoQuanXianTipObj = GameObject.Find("NoQuanXianTip");
            FenGeXian = GameObject.Find("FenGeXian");
            LogoObj = GameObject.Find("Logo");
            KaiShiYanBtnObj = GameObject.Find("KaiShiShiYanBtn");
            KaiShiYanBtn = KaiShiYanBtnObj.GetComponent<Button>();
            JiaoCaiContentSmallObj = GameObject.Find("JiaoCaiContentSmall");
            JiaoCaiContentSmallViewObj = JiaoCaiContentSmallObj.transform.GetChild(0).gameObject;
            ScrollRectSmall = JiaoCaiContentSmallObj.GetComponent<ScrollRect>();

            JiaoCaiContentBigObj = GameObject.Find("JiaoCaiContentBig");
            JiaoCaiContentBigViewObj = JiaoCaiContentBigObj.transform.GetChild(0).gameObject;
            ScrollRectBig = JiaoCaiContentBigObj.GetComponent<ScrollRect>();

            NoLoginStatusRightPanel = GameObject.Find("NoLoginStatusRightPanel");
            RightLoginBtn = NoLoginStatusRightPanel.transform.GetChild(0).GetComponent<Button>();
            ReturnBtn = GameObject.Find("ReturnBtn").GetComponent<Button>();
            ReturnBtn.onClick.AddListener(ReturnSourecWindowOnClick);

            LoginStatusRightPanel = GameObject.Find("LoginStatusRightPanel");
            UserIcon = LoginStatusRightPanel.transform.GetChild(0).transform.GetChild(0).GetComponent<Image>();
            UserNameTxt = LoginStatusRightPanel.transform.GetChild(1).GetComponent<Text>();
            XueDuanXueNianTxt = LoginStatusRightPanel.transform.GetChild(2).GetComponent<Text>();
            ChangeXueDuanXueNianBtn = LoginStatusRightPanel.transform.GetChild(3).GetComponent<Button>();
            ShiYanJiLuContentObj = LoginStatusRightPanel.transform.GetChild(4).gameObject;
            ShiYanJiLuContentParentObj = ShiYanJiLuContentObj.transform.GetChild(0).gameObject;
            ShiYanJiLuContentTogg = ShiYanJiLuContentParentObj.GetComponent<ToggleGroup>();

            NoShiYanJiLuPanelObj = LoginStatusRightPanel.transform.GetChild(6).gameObject;
            GengDuoJiLuBtn = LoginStatusRightPanel.transform.GetChild(5).GetComponent<Button>();
            XiuGaiJIChuXinXiPanelObj = LoginStatusRightPanel.transform.GetChild(7).gameObject;
            JiChuXinXiChangeBtn = XiuGaiJIChuXinXiPanelObj.transform.GetChild(0).GetComponent<Button>();
            LoginOutBtn = XiuGaiJIChuXinXiPanelObj.transform.GetChild(1).GetComponent<Button>();

            ShiYanCeShiBtn.onValueChanged.AddListener(ShiYanCeShiOnClick);
            ReturnMainBtn.onClick.AddListener(ReturnMainOnClick);
            GengDuoJiLuBtn.onClick.AddListener(GengDuoJiLuOnClick);
            ChangeXueDuanXueNianBtn.onClick.AddListener(ChangeXueDuanXueNianOnClick);
            LoginOutBtn.onClick.AddListener(LoginOutOnClick);
            RightLoginBtn.onClick.AddListener(RightLoginBtnOnClick);
            JiChuXinXiChangeBtn.onClick.AddListener(JiChuXinXiChangeOnClick);
            LoadingBtn.onClick.AddListener(LoadingOnClick);
            ZhangJieXuanZeDropDown.onValueChanged.AddListener(ChooseZhangJieOnClick);

            textbookMgrBtn = GameObject.Find("TextbookMgrBtn").GetComponent<Button>();
            textbookMgrBtn.onClick.AddListener(TextbookBtnOnclick);

        }
       
        //换页
        private void ChangePageOnClick(Vector2 arg0)
        {
            //Debug.Log(ShiYanXuanZeWindowData.Instance.ScrollRectPosValue);
            if (ShiYanXuanZeWindowData.Instance.ScrollRectPosValue >= 0.01f && ShiYanXuanZeWindowData.Instance.ScrollRectPosValue <= 0.2f)
            {
                ShowZhangJieNeRongTileName.text = ZhangJieXuanZeDropDown.captionText.text;
                if (ZhangJieNameLists_Dic.ContainsKey(ShowZhangJieNeRongTileName.text))
                {
                    if (s_currentPage <= s_totalPageCount)
                    {
                        s_currentPage += 1;
                        string[] splits = ZhangJieNameLists_Dic[ShowZhangJieNeRongTileName.text].Split('/');
                        GetShiYanZiYuanInfo(splits[0], splits[1]);
                    }
                }
            }
        }

        //选中的教材，并自动定位到该教材
        private void SnapContent(ScrollRect scrollRect, RectTransform source, RectTransform target) 
        {
            Canvas.ForceUpdateCanvases();
            source.anchoredPosition = (Vector2)scrollRect.transform.InverseTransformPoint(source.position) - (Vector2)scrollRect.transform.InverseTransformPoint(target.position);
        }

        //返回来源页
        private void ReturnSourecWindowOnClick()
        {
            WindowManager.ReShow();
            WindowManager.Close("ShiYanXuanZeWindow");
        }

        //清除数据
        private void ClearData()
        {
            NoQuanXianTipObj.SetActive(false);
            ShiYanXuanZeWindowData.Instance.ScrollRectPosValue = -1f;
            LoadCommpleteCount = 0;
            s_currentPage = 1;
            s_totalPageCount = 0;
            TipChooseJiaoCai.SetActive(false);
            for (int i = 0; i < s_ShiYanWorkItemList.Count; i++)
            {
                DestroyImmediate(s_ShiYanWorkItemList[i]);
            }
            s_ShiYanWorkItemList.Clear();
            ShiYanJieShaoInfoText.text = "";
            LayoutRebuilder.ForceRebuildLayoutImmediate(ShiYanJieShaoPanel.GetComponent<RectTransform>());
        }

        //清除登录的等相关的信息
        private void ClearAccountLoginData() 
        {
            LoginWindowData.Instance.SaveAccountLoginResult(null);
            LoginWindowData.Instance.SaveSessionResult(null);
            LoginWindowData.Instance.SaveToKenSwapResult(null);
            LoginWindowData.Instance.SaveUserInfoResult(null);
            LoginWindowData.Instance.SaveSMSLoginResult(null);
            MainWindowData.Instance.SaveUserChooseInfoResult(null);
            LoginWindowData.Instance.IsLogin = false;
        }

        //加载
        private void LoadingOnClick()
        {
            LoadingBtnObj.SetActive(true);
            LoadingBtn.interactable = false;
            LoadingTxtObj.SetActive(true);
            LoadingFaildTxtObj.SetActive(false);
            ClearData();
            //重新加载实验资源
            ShowZhangJieNeRongTileName.text = ZhangJieXuanZeDropDown.captionText.text;
            if (ZhangJieNameLists_Dic.ContainsKey(ShowZhangJieNeRongTileName.text))
            {
                string[] splits = ZhangJieNameLists_Dic[ShowZhangJieNeRongTileName.text].Split('/');
                GetShiYanZiYuanInfo(splits[0], splits[1]);
            }
        }

        //基础信息修改
        private void JiChuXinXiChangeOnClick()
        {
            WindowManager.Hide("ShiYanXuanZeWindow");
            BaseInfoWindowData.Instance.FromWindow = "ShiYanXuanZeWindow";
            WindowManager.Open<BaseInfoWindow>();
        }

        //登录
        private void RightLoginBtnOnClick()
        {
            WindowManager.Hide("ShiYanXuanZeWindow");
            WindowManager.Open<LoginWindow>();
        }

        //退出登录
        private void LoginOutOnClick()
        {
            ShiYanXuanZeWindowData.Instance.GetRecentTextbooksID = "";
            PlayerPrefs.DeleteAll();
            ClearAccountLoginData();
            //WindowManager.ReShow();
            WindowManager.Close("MainWindow");
            WindowManager.Close("ShiYanXuanZeWindow");
            WindowManager.Open<MainWindow>();
        }

        //打开基础信息修改页面
        private void ChangeXueDuanXueNianOnClick()
        {
            XiuGaiJIChuXinXiPanelObj.SetActive(true);
        }

        //更多记录
        private void GengDuoJiLuOnClick()
        {
            WindowManager.Hide("ShiYanXuanZeWindow");
            WindowManager.Open<ExperimentHistoryWindow>();
        }

        //回到首页
        private void ReturnMainOnClick()
        {
            WindowManager.Close("ShiYanXuanZeWindow");
            WindowManager.Open<MainWindow>();
        }

        //点击实验测试
        private void ShiYanCeShiOnClick(bool arg0)
        {
            WindowManager.Close("ShiYanXuanZeWindow");
            WindowManager.Open<ShiYanCeShiWindow>();
        }

        //提示去选择教材的物体
        private GameObject TipChooseJiaoCai;
        //默认设置
        private void DefulatSetting()
        {
            ShiYanXuanZeWindowData.Instance.ScrollRectPosValue = -1f;
            SmllTriggerImage.SetActive(true);
            BigeTriggerImage.SetActive(false);
            NoQuanXianTipObj.SetActive(false);
            ZhangJieXuanZeDropDownPanel.SetActive(false);
            KaiShiYanBtnObj.SetActive(false);
            LoadingBtnObj.SetActive(false);
            LoadingBtn.interactable = false;
            LoadingTxtObj.SetActive(true);
            LoadingFaildTxtObj.SetActive(false);
            FenGeXian.SetActive(false);
            LogoObj.SetActive(true);
            ShiYanJieShaoPanel.SetActive(false);
            ShiYanJieShaoPanel.transform.parent.gameObject.SetActive(false);
            ShiYanJieShaoInfoText.text = "";
            ShiYanQIChaiInfoText.text = "";
            ShowZhangJieNeRongTileName.text = "";
            JiaoCaiContentBigObj.transform.localScale = Vector3.zero;
            JiaoCaiContentSmallObj.SetActive(true);
            NoShiYanJiLuPanelObj.SetActive(true);
            XiuGaiJIChuXinXiPanelObj.SetActive(false);
            s_currentPage = 1;
            if (LoginWindowData.Instance.IsLogin)
            {
                NoLoginStatusRightPanel.SetActive(false);
                LoginStatusRightPanel.SetActive(true);
                UserNameTxt.text = MainWindowData.Instance.GetUserName;
                XueDuanXueNianTxt.text = BaseInfoWindowData.Instance.SectionName + "·" + BaseInfoWindowData.Instance.GradeName;
                if (string.IsNullOrEmpty(LoginWindowData.Instance.GetUserInfoResult().data.imgUrl))
                {
                    UserIcon.gameObject.SetActive(false);
                }
                else
                {
                    UserIcon.gameObject.SetActive(true);
                    StartCoroutine(LoadImage(LoginWindowData.Instance.GetUserInfoResult().data.imgUrl, UserIcon));
                }
                //获取用户选择教材列表
                GetUsrChooseInfoList();
                //获取实验记录
                CreatShiYanJiLuItem(ShiYanCeShiJiLuWindowData.Instance.GetShiYanCeShiJiLuResult());
            }
            else
            {
                //读取本地的------------
                NoLoginStatusRightPanel.SetActive(true);
                LoginStatusRightPanel.SetActive(false);
                //获取本地教材选择记录
                NoLoginGetUsrChooseInfoList();

                if (ShiYanXuanZeWindowData.Instance.GetRecentTextbooksID != "")
                {
                    NoLoginLoadJiaoCaiItemOnClick(JiaoCaiBigObj[ShiYanXuanZeWindowData.Instance.GetRecentTextbooksID].transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text, ShiYanXuanZeWindowData.Instance.GetRecentTextbooksID);
                }
                else if (ShiYanXuanZeWindowData.Instance.GetRecentTextbooksID == "")
                {
                    JiaoCaiTileName.text = "教材选择";
                    SetTileImageSize(JiaoCaiTileName);
                    Debug.Log("未选中教材");
                }
            }
            if (ShiYanXuanZeWindowData.Instance.GetRecentTextbooksID == "")
            {
                TipChooseJiaoCai.SetActive(true);
                if (JiaoCaiSmallObj.Count == 0)
                {
                    TipChooseJiaoCai.transform.GetChild(0).gameObject.SetActive(false);
                    TipChooseJiaoCai.transform.GetChild(1).gameObject.SetActive(true);
                    TipChooseJiaoCai.transform.GetChild(1).gameObject.SetActive(true);
                }
                else
                {
                    TipChooseJiaoCai.transform.GetChild(0).gameObject.SetActive(true);
                    TipChooseJiaoCai.transform.GetChild(1).gameObject.SetActive(false);
                    TipChooseJiaoCai.transform.GetChild(1).gameObject.SetActive(false);
                }
            }
            else
            {
                TipChooseJiaoCai.SetActive(false);
            }
        }

        //教材名称文字来适配图片的大小
        private void SetTileImageSize(Text text) 
        {
            float with = LayoutUtility.GetPreferredWidth(text.GetComponent<RectTransform>());
            if (with <= 170)
            {
                return;
            }
            else
            {
                JiaoCaiNameTileImag.GetComponent<RectTransform>().sizeDelta = new Vector2(with + 160, 62);
            }
        }

        //生成试验记录内容Item
        private void CreatShiYanJiLuItem(ShiYanCeShiJiLuResult shiYanCeShiJiLu)
        {
          
           if (shiYanCeShiJiLu.data.todayList.Count == 0 && shiYanCeShiJiLu.data.weekList.Count == 0 && shiYanCeShiJiLu.data.moreList.Count == 0)
           {
               NoShiYanJiLuPanelObj.SetActive(true);
               GengDuoJiLuBtn.gameObject.SetActive(false);
               return;
           }
           else
           {
                GengDuoJiLuBtn.gameObject.SetActive(true);
                NoShiYanJiLuPanelObj.SetActive(false);
                //生成今天的标题Item
                CeratTimeDataItem("今天", shiYanCeShiJiLu.data.todayList);
                CeratTimeDataItem("一周内", shiYanCeShiJiLu.data.weekList);
                CeratTimeDataItem("更早", shiYanCeShiJiLu.data.moreList);
            }
        }

        //生成实验记录的归类日期，如今天，一周内，更早
        public void CeratTimeDataItem(string data,List<ShiYanJiLuItemList> yanJiLuItemTodayList) 
        {
            if (yanJiLuItemTodayList.Count == 0)
            {
                return;
            }
            GameObject tempObj = ResManager.Instance.Load<GameObject>(ItemType.Item.JiLuTimerItem);
            GameObject item = Instantiate(tempObj);
            item.transform.SetParent(ShiYanJiLuContentParentObj.transform);
            item.transform.localPosition = Vector3.zero;
            item.transform.localScale = Vector3.one;
            item.transform.localEulerAngles = Vector3.zero;
            item.transform.GetChild(1).GetComponent<Text>().text = data;

            for (int i = 0; i < yanJiLuItemTodayList.Count; i++)
            {
                string id = yanJiLuItemTodayList[i].id;
                string name = yanJiLuItemTodayList[i].name;
                string type = yanJiLuItemTodayList[i].type;
                string src = yanJiLuItemTodayList[i].src;
                if (yanJiLuItemTodayList[i].type == "COURSE")
                {
                    TempCeShiJiLuItem = ResManager.Instance.Load<GameObject>(ItemType.Item.JiLuItemCe);
                }
                else
                {
                    TempCeShiJiLuItem = ResManager.Instance.Load<GameObject>(ItemType.Item.JiLuItem);
                }
                GameObject item1 = Instantiate(TempCeShiJiLuItem);
                item1.transform.SetParent(ShiYanJiLuContentParentObj.transform);
                item1.transform.localPosition = Vector3.zero;
                item1.transform.localScale = Vector3.one;
                item1.transform.localEulerAngles = Vector3.zero;
                item1.transform.GetChild(0).GetComponent<Text>().text = name;
                item1.GetComponent<Button>().onClick.AddListener(delegate() { ShiYanJiLuNeRongOnClick(id,name,type,src); } );
            }
        }

        //点击实验记录
        private void ShiYanJiLuNeRongOnClick(string id, string name, string type, string src)
        {
            if (type == "COURSE")
            {
                ShiYanCeShiXiangQingMainWindowData.Instance.ShiYanCeShiName = name;
                ShiYanCeShiXiangQingMainWindowData.Instance.ShiYanCeShiId = id;
                ShiYanCeShiXiangQingMainWindowData.Instance.IsLastShiYanCeShiMingXiInfo = true;
                WindowManager.Hide("ShiYanXuanZeWindow");
                WindowManager.Open<ShiYanCeShiXiangQingMainWindow>();
            }
            else if (type == "VR")
            {
                UniversalLoadingWindowData.Instance.GetShiYanName = name;
                UniversalLoadingWindowData.Instance.GetShiYanURL = src;
                UniversalLoadingWindowData.Instance.GetPlayMode = ResPlayMode.VRCourse.ToString();
                UniversalLoadingWindowData.Instance.ResourcesType = "课件播放";
                WindowManager.Hide("ShiYanXuanZeWindow");
                WindowManager.Open<UniversalLoadingWindow>();
            }
        }

        public override void OnShow(params object[] para)
        {
            base.OnShow(para);
            if (ShiYanXuanZeWindowData.Instance.GetRecentTextbooksID == "")
            {
                TipChooseJiaoCai.SetActive(true);
                if (JiaoCaiSmallObj.Count == 0)
                {
                    TipChooseJiaoCai.transform.GetChild(0).gameObject.SetActive(false);
                    TipChooseJiaoCai.transform.GetChild(1).gameObject.SetActive(true);
                    TipChooseJiaoCai.transform.GetChild(2).gameObject.SetActive(true);
                }
                else
                {
                    TipChooseJiaoCai.transform.GetChild(0).gameObject.SetActive(true);
                    TipChooseJiaoCai.transform.GetChild(1).gameObject.SetActive(false);
                    TipChooseJiaoCai.transform.GetChild(2).gameObject.SetActive(false);
                }
            }
            else
            {
                TipChooseJiaoCai.SetActive(false);
            }
        }

        public override void OnReShow()
        {
            if (LoginWindowData.Instance.IsLogin) 
            {
                //刷新实验记录列表
                if (UniversalLoadingWindowData.Instance.AppStatusListen == "appExitDone")
                {
                    ShiYanCeShiJiLuWindowData.Instance.GetShiYanJiLu();
                    CreatShiYanJiLuItem(ShiYanCeShiJiLuWindowData.Instance.GetShiYanCeShiJiLuResult());
                }
            }
            if (ShiYanXuanZeWindowData.Instance.GetRecentTextbooksID == "")
            {
                TipChooseJiaoCai.SetActive(true);
                if (JiaoCaiSmallObj.Count == 0)
                {
                    TipChooseJiaoCai.transform.GetChild(0).gameObject.SetActive(false);
                    TipChooseJiaoCai.transform.GetChild(1).gameObject.SetActive(true);
                    TipChooseJiaoCai.transform.GetChild(2).gameObject.SetActive(true);
                }
                else
                {
                    TipChooseJiaoCai.transform.GetChild(0).gameObject.SetActive(true);
                    TipChooseJiaoCai.transform.GetChild(1).gameObject.SetActive(false);
                    TipChooseJiaoCai.transform.GetChild(2).gameObject.SetActive(false);
                }
            }
            else
            {
                TipChooseJiaoCai.SetActive(false);
            }
        }
        public override void OnClose()
        {
            base.OnClose();
        }

        private void Update()
        {
            //判断是否有网络
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                isNetworkReachability = false;
            }
            else
            {
                isNetworkReachability = true;
            }
            #region
            //if (controllerDemo.cube != null)
            //{
            //    if (controllerDemo.cube.tag =="JiaoCai")
            //    {
            //        controllerDemo.cube.transform.GetChild(2).gameObject.SetActive(true);
            //    }
            //    else if (controllerDemo.cube.name == "Canvas")
            //    {
            //        foreach (var item in JiaoCaiBigObj.Values)
            //        {
            //            item.transform.GetChild(2).gameObject.SetActive(false);
            //        }
            //    }
            //}
            //else
            //{
            //    foreach (var item in JiaoCaiBigObj.Values)
            //    {
            //        item.transform.GetChild(2).gameObject.SetActive(false);
            //    }
            //}
            //MovePanelByJoystick();
            #endregion
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

        //获取服务器图片资源
        private IEnumerator LoadImage(string url, Image image,Image image1)
        {
            WWW www = new WWW(url);
            yield return www;
            if (www.isDone)
            {
                Texture2D tex = www.texture;
                if (image != null)
                {
                    Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero);
                    image.sprite = sprite;
                    image1.sprite = sprite;
                }
            }
        }

        private IEnumerator LoadImage(string url, Image image)
        {
            WWW www = new WWW(url);
            yield return www;
            if (string.IsNullOrEmpty(www.error))
            {
                if (www.isDone)
                {
                    Texture2D tex = www.texture;
                    if (image != null)
                    {
                        Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero);
                        image.sprite = sprite;
                        LoadCommpleteCount += 1;
                        if (LoadCommpleteCount == s_ShiYanWorkItemList.Count)
                        {
                            LoadingBtnObj.SetActive(false);
                            if (s_ShiYanWorkItemList[0].transform.GetChild(2).transform.GetChild(0).gameObject.activeSelf)
                            {
                                NoQuanXianTipObj.SetActive(true);
                            }
                            else
                            {
                                NoQuanXianTipObj.SetActive(false);
                            }
                        }
                    }
                }
            }
            else
            {
                if (image.name == "UserIcon")
                {
                    image.gameObject.SetActive(false);
                }
            }
        }

        //剪裁图片
        private void ScaleTexture(Texture2D source, Image image)
        {
            int targetWith = (int)image.rectTransform.rect.width;
            int targetheight = (int)image.rectTransform.rect.height;

            Texture2D result = new Texture2D(targetWith, targetheight, source.format, false);
            Color[] rpixels = result.GetPixels(0);
            float incX = (1.0f / (float)targetWith);
            float incY = (1.0f / (float)targetWith);
            for (int px = 0; px < rpixels.Length; px++)
            {
                rpixels[px] = source.GetPixelBilinear(incX * ((float)px % targetWith), incY * ((float)Mathf.Floor(px / targetheight)));
            }
            result.SetPixels(rpixels, 0);
            result.Apply();
            Sprite sprite = Sprite.Create(result, new Rect(0, 0, targetWith, targetheight), Vector2.zero);
            image.sprite = sprite;
        }

        //获取实验资源信息
        public void GetShiYanZiYuanInfo(string id,string node_path)
        {
            client.EndPoint = CommonConstant.SERVER_URL_MEC_PROXY;
            client.Method = EnumHttpVerb.POST;
       
            if (LoginWindowData.Instance.IsLogin)
            {
                ShiYanZiYuanInfoRequest shiYanZiYuanInfo = new ShiYanZiYuanInfoRequest();
                //shiYanZiYuanInfo.offset = (s_currentPage - 1) * s_limitItem;
                shiYanZiYuanInfo.offset = s_currentPage;
                shiYanZiYuanInfo.limit = s_limitItem;
                shiYanZiYuanInfo.teachingMaterialId = id;
                shiYanZiYuanInfo.chapterPath = node_path;
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
                shiYanZiYuanInfo.macInfo = macInfo;
                client.PostData = JsonUtility.ToJson(shiYanZiYuanInfo);
            }
            else
            {
                NoLoginShiYanZiYuanRequest noLoginShiYanZiYuan = new NoLoginShiYanZiYuanRequest();
                noLoginShiYanZiYuan.offset = (s_currentPage - 1) * s_limitItem;
                noLoginShiYanZiYuan.limit = s_limitItem;
                noLoginShiYanZiYuan.teachingMaterialId = id;
                noLoginShiYanZiYuan.chapterPath = "";
                client.PostData = JsonUtility.ToJson(noLoginShiYanZiYuan);
            }
            string result = client.HttpRequest(CommonConstant.GET_SHIYANZIYUANINFO);
            Debug.Log("实验资源：" + result);
            if (string.IsNullOrEmpty(result))
            {
                Debug.Log(client.ErrorMessage);
                return;
            }
            else
            {
                ShiYanZiYuanInfoResult ziYuanInfoResult = new ShiYanZiYuanInfoResult();
                ziYuanInfoResult = JsonUtility.FromJson<ShiYanZiYuanInfoResult>(result);
                ShiYanXuanZeWindowData.Instance.SaveShiYanZiYuanInfoResult(ziYuanInfoResult);
                CreatJiaoCaiNeRongItem(ziYuanInfoResult);
            }
        }

        //请求用户教材选择信息
        private void GetUserChooseInfo()
        {
            client.EndPoint = CommonConstant.SERVER_URL_MEC_PROXY;
            client.Method = EnumHttpVerb.POST;
            UserChooseListRequest userChooseList = new UserChooseListRequest();
            MacInfoRequest macInfo = new MacInfoRequest();
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
            userChooseList.macInfo = macInfo;
            userChooseList.page = 0;
            userChooseList.size = 10;
            userChooseList.subject = "";
            userChooseList.edition = "";

            client.PostData = JsonUtility.ToJson(userChooseList);
            string result = client.HttpRequest(CommonConstant.GET_USER_CHOOSE_LIST);
            Debug.Log("result: " + result);
            if (string.IsNullOrEmpty(result))
            {
                Debug.Log(client.ErrorMessage);
                return;
            }
            else
            {
                UserChooseInfoResult chooseInfoResult = new UserChooseInfoResult();
                chooseInfoResult = JsonUtility.FromJson<UserChooseInfoResult>(result);
                MainWindowData.Instance.SaveUserChooseInfoResult(chooseInfoResult);
            }
        }

        //获取用选择教材列表
        public void GetUsrChooseInfoList()
        {
            GetUserChooseInfo();
            //加载教材信息
            LoadJiaoCaiItem(MainWindowData.Instance.GetUserChooseInfoResult());
            //选中教材
            if (ShiYanXuanZeWindowData.Instance.GetRecentTextbooksID != "")
            {
                JiaoCaiItemOnClick(JiaoCaiBigObj[ShiYanXuanZeWindowData.Instance.GetRecentTextbooksID].transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text, ShiYanXuanZeWindowData.Instance.GetRecentTextbooksID);
            }
            else if (ShiYanXuanZeWindowData.Instance.GetRecentTextbooksID == "")
            {
                JiaoCaiTileName.text = "教材选择";
                SetTileImageSize(JiaoCaiTileName);
                Debug.Log("未选中教材");
            }
        }

        //加载教材
        private void LoadJiaoCaiItem(UserChooseInfoResult userChooseInfo)
        {
            if (userChooseInfo.data.items.Count == 0)
            {
                return;
            }
            for (int i = 0; i < userChooseInfo.data.items.Count; i++)
            {
                string imgeUrl = userChooseInfo.data.items[i].teaching_material_icon;
                string jiaoCaiName = userChooseInfo.data.items[i].teaching_material_name;
                string id = userChooseInfo.data.items[i].teaching_material_id;
                GameObject objSmall = ResManager.Instance.Load<GameObject>(ItemType.Item.JiaoCaiItemSmall);
                GameObject objSmallTemp = Instantiate(objSmall);
                objSmallTemp.transform.SetParent(JiaoCaiContentSmallViewObj.transform);
                objSmallTemp.transform.localScale = Vector3.one;
                Image smallImge = objSmallTemp.GetComponent<Image>();
                JiaoCaiSmallObj.Add(id, objSmallTemp);

                GameObject objBig = ResManager.Instance.Load<GameObject>(ItemType.Item.JiaoCaiItemBig);
                GameObject objBigTemp = Instantiate(objBig);
                objBigTemp.transform.SetParent(JiaoCaiContentBigViewObj.transform);
                objBigTemp.transform.localScale = Vector3.one;
                Image bigImge = objBigTemp.GetComponent<Image>();
                Text bigText = objBigTemp.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>();
                bigText.text = jiaoCaiName;
                JiaoCaiBigObj.Add(id, objBigTemp);
                objBigTemp.GetComponent<Button>().onClick.AddListener(delegate () { JiaoCaiItemOnClick(jiaoCaiName, id); });
                //判断选中的是哪一本教材
                if (id == ShiYanXuanZeWindowData.Instance.GetRecentTextbooksID)
                {
                    objBigTemp.transform.GetChild(1).gameObject.SetActive(true);
                    objSmallTemp.transform.GetChild(1).gameObject.SetActive(true);
                    //定位到该教材
                    SnapContent(ScrollRectSmall, JiaoCaiContentSmallViewObj.GetComponent<RectTransform>(), objSmallTemp.GetComponent<RectTransform>());
                    SnapContent(ScrollRectBig, JiaoCaiContentBigViewObj.GetComponent<RectTransform>(), objBigTemp.GetComponent<RectTransform>());
                }
                else
                {
                    objBigTemp.transform.GetChild(1).gameObject.SetActive(false);
                    objSmallTemp.transform.GetChild(1).gameObject.SetActive(false);
                }
                StartCoroutine(LoadImage(imgeUrl, smallImge, bigImge));
            }
        }

        //点击教材
        private void JiaoCaiItemOnClick(string name, string id)
        {
            ClearData();
            if (id == "")
            {
                return;
            }
            foreach (var item in JiaoCaiBigObj.Values)
            {
                item.transform.GetChild(1).gameObject.SetActive(false);
            }
            foreach (var item in JiaoCaiSmallObj.Values)
            {
                item.transform.GetChild(1).gameObject.SetActive(false);
            }
            JiaoCaiSmallObj[id].transform.GetChild(1).gameObject.SetActive(true);
            JiaoCaiBigObj[id].transform.GetChild(1).gameObject.SetActive(true);
            //判断是否有网络
            if (isNetworkReachability)
            {
                LoadingBtnObj.SetActive(true);
                LoadingBtn.interactable = false;
                LoadingTxtObj.SetActive(true);
                LoadingFaildTxtObj.SetActive(false);
            }
            else
            {
                LoadingBtnObj.SetActive(true);
                LoadingBtn.interactable = true;
                LoadingTxtObj.SetActive(false);
                LoadingFaildTxtObj.SetActive(true);
                return;
            }
            JiaoCaiTileName.text = name;
            SetTileImageSize(JiaoCaiTileName);
            ShowZhangJieNeRongTileName.text = ZhangJieXuanZeDropDown.captionText.text;
            ZhangJieXuanZeDropDownPanel.SetActive(true);
            ShiYanJieShaoPanel.SetActive(true);
            ShiYanJieShaoPanel.transform.parent.gameObject.SetActive(true);
            KaiShiYanBtnObj.SetActive(true);
            KaiShiYanBtn.onClick.RemoveAllListeners();
            FenGeXian.SetActive(true);
            LogoObj.SetActive(false);
            //获取章节内容
            GetShiYanZhangJie(id);
            //获取实验资源
            GetShiYanZiYuanInfo(id, "");
        }

        //获取教材章节的内容
        public void GetShiYanZhangJie(string teachingMaterialId)
        {
            client.EndPoint = CommonConstant.SERVER_URL_MEC_PROXY;
            client.Method = EnumHttpVerb.POST;
            JIaoCaiZhangJieInfoRequest jIaoCaiZhangJieInfo = new JIaoCaiZhangJieInfoRequest();
            jIaoCaiZhangJieInfo.teachingMaterialId = teachingMaterialId;
            client.PostData = JsonUtility.ToJson(jIaoCaiZhangJieInfo);
            string result = client.HttpRequest(CommonConstant.GET_JIAOCAIZHANGJIEINFO);
            Debug.Log(result);
            if (string.IsNullOrEmpty(result))
            {
                Debug.Log(client.ErrorMessage);
                return;
            } 
            else
            {
                JiaoCaiZhangJieResult zhangJieResult = new JiaoCaiZhangJieResult();
                zhangJieResult = JsonUtility.FromJson<JiaoCaiZhangJieResult>(result);
                ShiYanXuanZeWindowData.Instance.SaveJiaoCaiZhangJieResult(zhangJieResult);
                AddZhangJieName(zhangJieResult, teachingMaterialId);
            }
        }

        //章节列表里面添加数据
        private void AddZhangJieName(JiaoCaiZhangJieResult jiaoCaiZhangJie, string teachingMaterialId)
        {
            ZhangJieNameLists_Dic.Clear();
            ZhangJieNameLists_Dic.Add(zhangJieAll, teachingMaterialId + "/" + "");
            for (int i = 0; i < jiaoCaiZhangJie.data.Count; i++)
            {
                ZhangJieNameLists_Dic.Add(jiaoCaiZhangJie.data[i].name, teachingMaterialId + "/" + jiaoCaiZhangJie.data[i].node_path);
            }
            AddZhangJieOptions(ZhangJieNameLists_Dic);
        }

        //往章节选择下拉框里添加数据
        private void AddZhangJieOptions(Dictionary<string, string> nameList)
        {
            ZhangJieXuanZeDropDown.ClearOptions();
            Dropdown.OptionData data = null;
            foreach (var item in nameList.Keys)
            {
                data = new Dropdown.OptionData();
                data.text = item;
                ZhangJieXuanZeDropDown.options.Add(data);
            }
            ZhangJieXuanZeDropDown.captionText.text = zhangJieAll;
            ShowZhangJieNeRongTileName.text = ZhangJieXuanZeDropDown.captionText.text;
        }

        //章节选择
        private void ChooseZhangJieOnClick(int arg0)
        {
            ClearData();
            ShowZhangJieNeRongTileName.text = ZhangJieXuanZeDropDown.captionText.text;
            if (ZhangJieNameLists_Dic.ContainsKey(ShowZhangJieNeRongTileName.text))
            {
                string[] splits = ZhangJieNameLists_Dic[ShowZhangJieNeRongTileName.text].Split('/');
                GetShiYanZiYuanInfo(splits[0], splits[1]);
            }
        }

        //生成教材内容Item
        private void CreatJiaoCaiNeRongItem(ShiYanZiYuanInfoResult shiYanZiYuanInfo)
        {
            LoadCommpleteCount = 0;
            if (shiYanZiYuanInfo.data.total == 0)
            {
                LoadingBtnObj.SetActive(false);
                if (JiaoCaiNeRongContentObj.transform.childCount == 0)
                {
                    NoQuanXianTipObj.SetActive(false);
                    KaiShiYanBtn.gameObject.SetActive(false);
                }
                else
                {
                    KaiShiYanBtn.gameObject.SetActive(true);
                }
                return;
            }
            KaiShiYanBtn.gameObject.SetActive(true);
            if (shiYanZiYuanInfo.data.total % s_limitItem != 0)
            {
                s_totalPageCount = Convert.ToInt32(shiYanZiYuanInfo.data.total / s_limitItem) + 1;
            }
            else
            {
                s_totalPageCount = Convert.ToInt32(shiYanZiYuanInfo.data.total / s_limitItem);
            }
            for (int i = 0; i < shiYanZiYuanInfo.data.chapter.Count; i++)
            {
                if (shiYanZiYuanInfo.data.chapter[i].type != "VR")
                {
                    continue;
                }
                ShiYanZiYuanItemResult ziYuanItemResult = shiYanZiYuanInfo.data.chapter[i];
                string imageUrl = shiYanZiYuanInfo.data.chapter[i].thumbnail;
                string nameText = shiYanZiYuanInfo.data.chapter[i].name;
                string description = shiYanZiYuanInfo.data.chapter[i].description;
                string src= shiYanZiYuanInfo.data.chapter[i].src;
                GameObject tempObj = ResManager.Instance.Load<GameObject>(ItemType.Item.ZhangJieItem);
                GameObject item = Instantiate(tempObj);
                item.transform.SetParent(JiaoCaiNeRongContentObj.transform);
                item.transform.localPosition = Vector3.zero;
                item.transform.localScale = Vector3.one;
                item.transform.localEulerAngles = Vector3.zero;
                item.GetComponent<Toggle>().group = JiaoCaiNeRongContentTogg;
                Image itemImge = item.transform.GetChild(2).transform.GetComponent<Image>();
                Text itemText = item.transform.GetChild(3).transform.GetComponent<Text>();
                itemText.text = nameText;
                GameObject lockObj = item.transform.GetChild(2).transform.GetChild(0).gameObject;
                item.GetComponent<Toggle>().onValueChanged.AddListener((bool arg0) => JiaoCaiNeRongOnClick(lockObj, src, description, ziYuanItemResult));
                if (shiYanZiYuanInfo.data.chapter[i].resource_operation.PLAY == "播放")
                {
                    lockObj.SetActive(false);
                }
                else
                {
                    lockObj.SetActive(true);
                }
                s_ShiYanWorkItemList.Add(item);
                StartCoroutine(LoadImage(imageUrl, itemImge));
            }
            s_ShiYanWorkItemList[0].GetComponent<Toggle>().isOn = true;
        }

        //点击教材内容
        private void JiaoCaiNeRongOnClick(GameObject lockObj, string src, string description, ShiYanZiYuanItemResult ziYuanItemResult)
        {
            KaiShiYanBtn.onClick.RemoveAllListeners();
            ShiYanJieShaoInfoText.text = description;
            ShiYanXuanZeWindowData.Instance.GetShiYanZiYuanItemResult = ziYuanItemResult;
            //Debug.Log("ShiYanXuanZeWindowData.Instance.GetShiYanZiYuanItemResult :" + ShiYanXuanZeWindowData.Instance.GetShiYanZiYuanItemResult.objectId);
            LayoutRebuilder.ForceRebuildLayoutImmediate(ShiYanJieShaoPanel.GetComponent<RectTransform>());
            string name = lockObj.transform.parent.parent.GetChild(3).GetComponent<Text>().text;
            lockObj.transform.parent.parent.GetChild(1).gameObject.SetActive(false);
            if (lockObj.activeSelf)
            {
                KaiShiYanBtn.interactable = false;
                if (LoadingBtnObj.activeSelf)
                {
                    NoQuanXianTipObj.SetActive(false);
                }
                else
                {
                    NoQuanXianTipObj.SetActive(true);
                }
            }
            else
            {
                KaiShiYanBtn.interactable = true;
                NoQuanXianTipObj.SetActive(false);
                KaiShiYanBtn.onClick.AddListener(delegate() { KaiShiShiYanOnClick(name, src); });
            }
        }

        //开始实验
        private void KaiShiShiYanOnClick(string name, string src)
        {
            Debug.Log(name + "：" + src);
            UniversalLoadingWindowData.Instance.GetShiYanName = name;
            UniversalLoadingWindowData.Instance.GetShiYanURL = src;
            UniversalLoadingWindowData.Instance.GetPlayMode = ResPlayMode.VRCourse.ToString();
            UniversalLoadingWindowData.Instance.ResourcesType = "课件播放";
            WindowManager.Hide("ShiYanXuanZeWindow");
            WindowManager.Open<UniversalLoadingWindow>();
        }

        //未登录状态获取本地的教材选择列表
        private void NoLoginGetUsrChooseInfoList() 
        {
            if (TextbookMgrWindowData.Instance.userChooseItemResults.Count == 0)
            {
                return;
            }
            for (int i = 0; i < TextbookMgrWindowData.Instance.userChooseItemResults.Count; i++)
            {
                string imgeUrl = TextbookMgrWindowData.Instance.userChooseItemResults[i].teaching_material_icon;
                string jiaoCaiName = TextbookMgrWindowData.Instance.userChooseItemResults[i].teaching_material_name;
                string id = TextbookMgrWindowData.Instance.userChooseItemResults[i].teaching_material_id;
                GameObject objSmall = ResManager.Instance.Load<GameObject>(ItemType.Item.JiaoCaiItemSmall);
                GameObject objSmallTemp = Instantiate(objSmall);
                objSmallTemp.transform.SetParent(JiaoCaiContentSmallViewObj.transform);
                objSmallTemp.transform.localScale = Vector3.one;
                Image smallImge = objSmallTemp.GetComponent<Image>();
                JiaoCaiSmallObj.Add(id, objSmallTemp);

                GameObject objBig = ResManager.Instance.Load<GameObject>(ItemType.Item.JiaoCaiItemBig);
                GameObject objBigTemp = Instantiate(objBig);
                objBigTemp.transform.SetParent(JiaoCaiContentBigViewObj.transform);
                objBigTemp.transform.localScale = Vector3.one;
                Image bigImge = objBigTemp.GetComponent<Image>();
                Text bigText = objBigTemp.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>();
                bigText.text = jiaoCaiName;
                JiaoCaiBigObj.Add(id, objBigTemp);
                objBigTemp.GetComponent<Button>().onClick.AddListener(delegate () { NoLoginLoadJiaoCaiItemOnClick(jiaoCaiName, id); });
                //判断选中的是哪一本教材
                if (id == ShiYanXuanZeWindowData.Instance.GetRecentTextbooksID)
                {
                    objBigTemp.transform.GetChild(1).gameObject.SetActive(true);
                    objSmallTemp.transform.GetChild(1).gameObject.SetActive(true);
                    //定位到该教材
                    SnapContent(ScrollRectSmall, JiaoCaiContentSmallViewObj.GetComponent<RectTransform>(), objSmallTemp.GetComponent<RectTransform>());
                    SnapContent(ScrollRectBig, JiaoCaiContentBigViewObj.GetComponent<RectTransform>(), objBigTemp.GetComponent<RectTransform>());
                }
                else
                {
                    objBigTemp.transform.GetChild(1).gameObject.SetActive(false);
                    objSmallTemp.transform.GetChild(1).gameObject.SetActive(false);
                }
                StartCoroutine(LoadImage(imgeUrl, smallImge, bigImge));
            }
        }

        private void NoLoginLoadJiaoCaiItemOnClick(string jiaoCaiName, string id)
        {
            ClearData();
            if (id == "")
            {
                return;
            }
            foreach (var item in JiaoCaiBigObj.Values)
            {
                item.transform.GetChild(1).gameObject.SetActive(false);
            }
            foreach (var item in JiaoCaiSmallObj.Values)
            {
                item.transform.GetChild(1).gameObject.SetActive(false);
            }
            JiaoCaiSmallObj[id].transform.GetChild(1).gameObject.SetActive(true);
            JiaoCaiBigObj[id].transform.GetChild(1).gameObject.SetActive(true);
            //判断是否有网络
            if (isNetworkReachability)
            {
               LoadingBtnObj.SetActive(true);
               LoadingBtn.interactable = false;
               LoadingTxtObj.SetActive(true);
               LoadingFaildTxtObj.SetActive(false);
            }
            else
            {
                LoadingBtnObj.SetActive(true);
                LoadingBtn.interactable = true;
                LoadingTxtObj.SetActive(false);
                LoadingFaildTxtObj.SetActive(true);
                return;
            }
            JiaoCaiTileName.text = jiaoCaiName;
            SetTileImageSize(JiaoCaiTileName);
            ShowZhangJieNeRongTileName.text = ZhangJieXuanZeDropDown.captionText.text;
            ZhangJieXuanZeDropDownPanel.SetActive(true);
            ShiYanJieShaoPanel.SetActive(true);
            ShiYanJieShaoPanel.transform.parent.gameObject.SetActive(true);
            KaiShiYanBtnObj.SetActive(true);
            KaiShiYanBtn.onClick.RemoveAllListeners();
            FenGeXian.SetActive(true);
            LogoObj.SetActive(false);
            //获取章节内容
            GetShiYanZhangJie(id);
            //获取实验资源
            GetShiYanZiYuanInfo(id, "");
        }
        private void TextbookBtnOnclick()
        {
            TextbookMgrWindowData.Instance.FormWindow = "ShiYanXuanZeWindow";
            WindowManager.Hide("ShiYanXuanZeWindow");
            WindowManager.Open<TextbookMgrWindow>();
        }

        //保存实验资源的记录
        public void SaveShiYanZiYuanJiLu(ShiYanZiYuanInfoResult shiYanZiYuanInfo) 
        {
            client.EndPoint = CommonConstant.SERVER_URL_MEC_PROXY;
            client.Method = EnumHttpVerb.POST;
            ShiYanCeShiZiYuanRequest yanCeShiZiYuanRequest = new ShiYanCeShiZiYuanRequest();
        }

    }
}

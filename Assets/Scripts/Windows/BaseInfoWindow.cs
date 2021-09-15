using System;
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
namespace Assets.Scripts.Windows
{
    class BaseInfoWindow : BaseWindow
    {
        #region 字段定义
        //学段学级面板定义
        private GameObject s_XueDuanSelectPanel;
        private GameObject s_NianJISelectPanel;
        private GameObject s_LoadingImage;

        private ToggleGroup s_XueDuanToggleGroup;
        private ToggleGroup s_NianJiToggleGroup;

        private Toggle s_CurrentXueDuanToggle = null;
        private Toggle s_CurrentNianJiToggle = null;

        private List<string> s_XueDuanList = new List<string>()/* { "小学", "初中", "高中" }*/;
        private List<string> s_NianJiPrimaryList = new List<string>() /*{ "一年级", "二年级", "三年级", "四年级", "五年级", "六年级" }*/;
        private List<string> s_NianJiMiddleList = new List<string>() /*{ "七年级", "八年级", "九年级" }*/;
        private List<string> s_NianJiHighList = new List<string>() /*{ "高一", "高二", "高三" }*/;
        private Dictionary<string, string> keyDic = new Dictionary<string, string>();//用来保存学段年级键值对。


        private List<Toggle> s_XueDuanToggles = new List<Toggle>();
        private List<Toggle> s_NianJiToggles = new List<Toggle>();

        private WindowGrid s_XueDuanGrid = WindowGrid.XueDuanGrid;
        private WindowGrid s_NianJiGrid = WindowGrid.NianJiGrid;

        private Button s_ConfirmBtn;
        private Button s_JumpLoginBtn;
        private Button s_ReturnBtn;
        private Button s_SkipBtn;

        private Text s_ErrorText;

        private bool stopRotate = true;

        //颜色
        Color32 s_OriColor = new Color32(49, 255, 255, 255);
        Color32 s_SelectColor = new Color32(255, 252, 206, 255);

        RestClient client = new RestClient();
        //所有的数据源
        GetGradeListResult gglr;
        #endregion
        #region 窗口生命周期
        public override void OnInit()
        {
            prefabType = Window.BaseInfoWindow;
            layer = WindowManager.Layer.Window;
        }
        public override void Init()
        {
            base.Init();
            InitData();
            InitButton();
        }
        //面板打开时
        public override void OnShow(params object[] para)
        {
            //读取数据然后显示
            GetGrade();
            ShowGradePanel();
            s_SkipBtn.gameObject.SetActive(false);
            s_ErrorText.gameObject.SetActive(false);
            //网络读取数据显示勾选
            if (BaseInfoWindowData.Instance.GradeName != null)
            {
                s_ReturnBtn.gameObject.SetActive(true);
                CheckToggleByUserInfo();
            }
            else
            {
                s_ReturnBtn.gameObject.SetActive(false);
                s_ConfirmBtn.interactable = false;
            }
            //登录状态显示处理
            if (LoginWindowData.Instance.IsLogin)
            {
                s_JumpLoginBtn.gameObject.SetActive(false);
            }
        }
        //面板关闭时
        public override void OnClose()
        {
            base.OnClose();
        }
        #endregion
        #region 初始化方法
        private void InitData()
        {
            //面板绑定
            s_XueDuanSelectPanel = GameObject.Find("XueDuanSelectPanel");
            s_NianJISelectPanel = GameObject.Find("NianJiSelectPanel");
            s_ErrorText = GameObject.Find("ErrorText").GetComponent<Text>();

            //选项管理器绑定
            s_XueDuanToggleGroup = s_XueDuanSelectPanel.GetComponentInChildren<ToggleGroup>();
            s_NianJiToggleGroup = s_NianJISelectPanel.GetComponentInChildren<ToggleGroup>();


        }
        private void InitButton()
        {
            s_ConfirmBtn = GameObject.Find("ConfirmBtn").GetComponent<Button>();
            s_JumpLoginBtn = GameObject.Find("JumpLoginBtn").GetComponent<Button>();
            s_ReturnBtn = GameObject.Find("ReturnBtn").GetComponent<Button>();
            s_SkipBtn = GameObject.Find("SkipBtn").GetComponent<Button>();

            s_ConfirmBtn.onClick.AddListener(ConfirmBtnOnClick);
            s_JumpLoginBtn.onClick.AddListener(JumpLoginBtnOnClick);
            s_ReturnBtn.onClick.AddListener(ReturnBtnOnClick);
            s_SkipBtn.onClick.AddListener(SkipBtnOnClick);

            s_LoadingImage = s_ConfirmBtn.transform.GetChild(1).gameObject;
        }
        #endregion
        #region 逻辑方法
        //获取学段年级数据
        private void GetGrade()
        {
            client.EndPoint = CommonConstant.SERVER_URL_MEC_PROXY;
            client.Method = EnumHttpVerb.GET;
            string result2 = client.HttpRequest(CommonConstant.GET_GRADE_LIST);
            gglr = new GetGradeListResult();
            gglr = JsonUtility.FromJson<GetGradeListResult>(result2);
            for (int i = 0; i < gglr.data.Count; i++)
            {
                s_XueDuanList.Add(gglr.data[i].value);
                for (int j = 0; j < gglr.data[i].grade_vos.Count; j++)
                {
                    if (gglr.data[i].value == "小学")
                    {
                        s_NianJiPrimaryList.Add(gglr.data[i].grade_vos[j].value);
                    }
                    else if (gglr.data[i].value == "初中")
                    {
                        s_NianJiMiddleList.Add(gglr.data[i].grade_vos[j].value);
                    }
                    else
                    {
                        s_NianJiHighList.Add(gglr.data[i].grade_vos[j].value);
                    }
                    keyDic.Add(gglr.data[i].grade_vos[j].value, gglr.data[i].grade_vos[j].key);
                }
                keyDic.Add(gglr.data[i].value,gglr.data[i].key);
            }
        }
        //根据用户信息勾选
        private void CheckToggleByUserInfo()
        {
            string currentXueDuan = BaseInfoWindowData.Instance.SectionName;
            string currentNianJi = BaseInfoWindowData.Instance.GradeName;
            if (currentXueDuan != null && currentNianJi != null)
            {
                for (int i = 0; i < s_XueDuanList.Count; i++)
                {
                    if (currentXueDuan == s_XueDuanList[i])
                    {
                        s_XueDuanToggles[i].isOn = true;
                    }
                }
                for (int i = 0; i < s_NianJiToggles.Count; i++)
                {
                    if (currentNianJi == s_NianJiToggles[i].GetComponentInChildren<Text>().text)
                    {
                        s_NianJiToggles[i].isOn = true;
                    }
                }
            }
        }
        //展示学段面板
        private void ShowGradePanel()
        {
            //处理学段数据
            for (int i = 0; i < s_XueDuanList.Count; i++)
            {
                GameObject goLoad = ResManager.Instance.Load<GameObject>(s_XueDuanGrid);
                GameObject go = Instantiate(goLoad, s_XueDuanToggleGroup.transform);
                Text itemtext = go.GetComponentInChildren<Text>();
                itemtext.text = s_XueDuanList[i];
                Toggle xueDuanToggle = go.GetComponent<Toggle>();
                s_XueDuanToggles.Add(xueDuanToggle);
                xueDuanToggle.group = s_XueDuanToggleGroup;
                xueDuanToggle.onValueChanged.AddListener((bool value) =>
                {
                    if (value)
                    {
                    //设置当前选段
                    s_CurrentXueDuanToggle = xueDuanToggle;
                    //清除右边学级显示
                    ClearNianJiToggle();
                    //显示当前年级面板
                    ShowNianJiPanel();
                    //字体颜色改变
                    itemtext.color = s_SelectColor;
                    }
                    else
                    {
                    //清除右边学级显示
                        ClearNianJiToggle();
                        s_CurrentXueDuanToggle = null;
                        s_CurrentNianJiToggle = null;
                    //字体颜色改变
                        itemtext.color = s_OriColor;
                        s_ConfirmBtn.interactable = false;
                        s_SkipBtn.gameObject.SetActive(false);
                    }
                }
                );
            }
        }
        //清空年级面板
        private void ClearNianJiToggle()
        {
            if (s_NianJiToggles.Count == 0)
            {
                return;
            }
            else
            {
                for (int i = 0; i < s_NianJiToggles.Count; i++)
                {
                    DestroyImmediate(s_NianJiToggles[i].gameObject);
                }
                s_NianJiToggles.Clear();
            }
        }
        //显示当前年级面板
        private void ShowNianJiPanel()
        {
            string currentNianJi = s_CurrentXueDuanToggle.GetComponentInChildren<Text>().text;
            List<string> showText = new List<string>();
            switch (currentNianJi)
            {
                case "小学":
                    showText = s_NianJiPrimaryList;
                    break;
                case "初中":
                    showText = s_NianJiMiddleList;
                    break;
                case "高中":
                    showText = s_NianJiHighList;
                    break;
            }
            //处理年级数据
            for (int i = 0; i < showText.Count; i++)
            {
                GameObject goLoad = ResManager.Instance.Load<GameObject>(s_NianJiGrid);
                GameObject go = Instantiate(goLoad, s_NianJiToggleGroup.transform);
                Text itemtext = go.GetComponentInChildren<Text>();
                itemtext.text = showText[i];
                Toggle nianJiToggle = go.GetComponent<Toggle>();
                s_NianJiToggles.Add(nianJiToggle);
                nianJiToggle.group = s_NianJiToggleGroup;
                nianJiToggle.onValueChanged.AddListener((bool value) =>
                {
                    if (value)
                    {
                    //设置当前年级
                    s_CurrentNianJiToggle = nianJiToggle;
                    //字体颜色改变
                    itemtext.color = s_SelectColor;
                        s_ConfirmBtn.interactable = true;
                        s_ConfirmBtn.transform.GetComponentInChildren<Text>().text = "完成";
                    }
                    else
                    {
                        s_CurrentNianJiToggle = null;
                    //字体颜色改变
                        itemtext.color = s_OriColor;
                        s_ConfirmBtn.interactable = false;
                        s_SkipBtn.gameObject.SetActive(false);
                    }
                }
                    );
            }
        }
        //刷新前一页数据并关掉面板
        private void ReFreshPreWindow()
        {
            if (BaseInfoWindowData.Instance.FromWindow == "TextbookSelectWindow")
            {
                WindowManager.Close("TextbookSelectWindow");
                WindowManager.Open<TextbookSelectWindow>();
            }
            else if (BaseInfoWindowData.Instance.FromWindow == "MainWindow")
            {
                WindowManager.Close("MainWindow");
                WindowManager.Open<MainWindow>();
            }
            else if (BaseInfoWindowData.Instance.FromWindow == "TextbookMgrWindow")
            {
                WindowManager.Close("TextbookMgrWindow");
                WindowManager.Open<TextbookMgrWindow>();
            }
            else if (BaseInfoWindowData.Instance.FromWindow == "ShiYanXuanZeWindow")
            {
                WindowManager.Close("ShiYanXuanZeWindow");
                WindowManager.Open<ShiYanXuanZeWindow>();
            }
            else if (BaseInfoWindowData.Instance.FromWindow == "ShiYanCeShiWindow")
            {
                WindowManager.Close("ShiYanCeShiWindow");
                WindowManager.Open<ShiYanCeShiWindow>();
            }
            else if (BaseInfoWindowData.Instance.FromWindow == "LoadingWindow")
            {
                WindowManager.Open<MainWindow>();
            }
            WindowManager.Close("BaseInfoWindow");
        }
        #endregion
        #region 按钮点击方法
        private void ConfirmBtnOnClick()
        {
            s_ConfirmBtn.interactable = false;
            s_ConfirmBtn.transform.GetChild(0).gameObject.SetActive(false);
            s_LoadingImage.SetActive(true);
            stopRotate = false;
            BaseInfoWindowData.Instance.SectionName= s_CurrentXueDuanToggle.GetComponentInChildren<Text>().text;
            BaseInfoWindowData.Instance.GradeName = s_CurrentNianJiToggle.GetComponentInChildren<Text>().text;

            BaseInfoWindowData.Instance.SectionID = keyDic[BaseInfoWindowData.Instance.SectionName];
            BaseInfoWindowData.Instance.GradeID = keyDic[BaseInfoWindowData.Instance.GradeName];
            //访问保存(登录状态保存未通)。
            if (LoginWindowData.Instance.IsLogin)
            {
                UserInfoResult userInfoResult = LoginWindowData.Instance.GetUserInfoResult();


                client.EndPoint = CommonConstant.SERVER_URL_MEC_PROXY;
                client.Method = EnumHttpVerb.POST;
                PostGradeRequest pgr = new PostGradeRequest();
                pgr.section = keyDic[BaseInfoWindowData.Instance.SectionName];
                pgr.graduateYear =keyDic[BaseInfoWindowData.Instance.GradeName];
                
                if (LoginWindowData.Instance.ReadAccountLoginResult() != null)
                {
                    pgr.macInfo.token = LoginWindowData.Instance.ReadAccountLoginResult().access_token;
                    pgr.macInfo.mac_key = LoginWindowData.Instance.ReadAccountLoginResult().mac_key;
                    pgr.userId = LoginWindowData.Instance.ReadAccountLoginResult().user_id;
                }
                else
                {
                    pgr.macInfo.token = LoginWindowData.Instance.ReadToKenSwapResult().data.access_token;
                    pgr.macInfo.mac_key = LoginWindowData.Instance.ReadToKenSwapResult().data.mac_key;
                    pgr.userId = LoginWindowData.Instance.ReadToKenSwapResult().data.user_id;
                }

                pgr.macInfo.session_key = LoginWindowData.Instance.ReadSessionResult().session_key;
                client.PostData = JsonUtility.ToJson(pgr);
                string result = client.HttpRequest(CommonConstant.POST_SAVE_GRADE);
                if (result == null)
                {
                    //保存失败。----------------------------------------------------------------------
                    s_ConfirmBtn.interactable = true;
                    s_ConfirmBtn.transform.GetChild(0).gameObject.SetActive(true);
                    s_ConfirmBtn.transform.GetComponentInChildren<Text>().text = "重试";
                    s_LoadingImage.SetActive(false);
                    s_SkipBtn.gameObject.SetActive(true);
                    stopRotate = true;
                }
                else
                {
                    //保存成功。窗口跳转
                    ReFreshPreWindow();
                    
                }

            }
            else
            {
                ReFreshPreWindow();
            }


        }
        private void JumpLoginBtnOnClick()
        {
            //跳转到登录页

            WindowManager.Hide("BaseInfoWindow");
            WindowManager.Open<LoginWindow>();
        }
        private void ReturnBtnOnClick()
        {
            //跳转回前一页
            WindowManager.ReShow();
            WindowManager.Close("BaseInfoWindow");
        }
        private void SkipBtnOnClick()
        {
            //跳转到首页
            WindowManager.Open<MainWindow>();
            WindowManager.Close("BaseInfoWindow");
        }

        private void Update()
        {
            if (!stopRotate)
            {
                s_LoadingImage.transform.Rotate(Vector3.forward, 3);
            }
        }
        #endregion
    }
}

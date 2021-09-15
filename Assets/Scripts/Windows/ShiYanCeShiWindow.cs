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
using UnityEngine.Networking;

namespace Assets.Scripts.Windows 
{
    public class ShiYanCeShiWindow : BaseWindow
    {
        RestClient client = new RestClient();

        //实验资源按钮
        private Toggle ShiYanZiYuanXuanZeBtnCeShiTogg;

        //回到首页按钮
        private Button ReturnMainBtnCeShiBtn;

        //地区选择下拉框
        private Dropdown ShiYanCeShiDiQuChooseDropDown;

        //未登录状态
        private GameObject NoLoginStatusRightPanelCeShiObj;

        //登录状态
        private GameObject LoginStatusRightPanelCeShiObj;

        //登录按钮
        private Button LoginBtn;

        //退出登录
        private Button LoginOutBtn;

        //更多实验记录按钮
        private Button GengDuoJiLuBtn;

        //修改基础信息按钮
        private Button JiChuXinXiXiuGaiBtnCeShiBtn;

        //修改基础信息面板
        private GameObject XiuGaiJIChuXinXiPanelCeShiObj;

        //没有实验记录
        private GameObject NoShiYanJiLuCeShiObj;

        //用户图标
        private Image UserIcon;

        //用户名
        private Text UserName;

        //学年学段
        private Text XueNianXueDuanText;

        //显示科目分类的类别
        private Text FenLeiMoKuaiNameText;

        //打开修改基础信息的按钮
        private Button ChangeXueDuanXueNianBtnCeShi;

        //实验记录的Content父物体
        private Transform ShiYanJiLuContenObj;

        //实验的学科Content父物体
        private Transform XueKeContentCeShiObj;
        //实验学科的ToggGroup
        private ToggleGroup XueKeContentToggGroup;

        //课程的Item父物体
        private GameObject ShiYanItemContentViewCeShi;

        //app背景图标
        private GameObject LogoCeShi;

        //加载的物体
        private GameObject ShiYanCeShiLoadingBtnObj;

        //加载失败的物体
        private GameObject NoQuanXianTipCeShiObj;

        //存储下拉框里面名称数据
        private Dictionary<string, string> AreaNameDic = new Dictionary<string, string>();
        private string s_subjectAllName = "全部";


        //限制获取实验课程Item
        private int s_limitItem = 50;

        //当前页数
        private int s_currentPage = 1;

        //总页数
        private int s_totalPageCount = 0;

        //用于实验测试资源检索
        private string s_selectTags = "";
        //地区标签
        private string s_selectRegionTags = "";

        //学科标签
        private string s_selectSubjectTags = "";

        //存储生成的实验课程
        private List<GameObject> s_ShiYanCeShiCourseItemList = new List<GameObject>();

        //字体颜色
        private Color colorText = new Color32(239, 235, 104, 255);

        //滑动组件
        private ScrollRect SubjectScrollRect;

        private GameObject TempCeShiJiLuItem = null;

        //科目的数量
        private int XueKeCount = 0;

        //存储所有学科的Tag
        private List<string> Tag_Subjects = new List<string>();

        //存储所有学科
        private List<GameObject> SubjectItems = new List<GameObject>();

        //存储所有学科名称
        private List<string> SubjectItemsName = new List<string>();

        public override void OnInit()
        {
            base.OnInit();
            prefabType = PrefabType.Window.ShiYanCeShiWindow;
            prefab = gameObject;
        }

        public override void Init()
        {
            base.Init();
            InitData();
            DefulatSetting();
        }

        public override void OnReShow()
        {
            base.OnReShow();
            ShiYanXuanZeWindowData.Instance.TempObject = ShiYanItemContentViewCeShi;
            if (LoginWindowData.Instance.IsLogin)
            {
                //刷新实验记录列表
                if (UniversalLoadingWindowData.Instance.AppStatusListen == "appExitDone")
                {
                    ShiYanCeShiJiLuWindowData.Instance.GetShiYanJiLu();
                    CreatShiYanJiLuItem(ShiYanCeShiJiLuWindowData.Instance.GetShiYanCeShiJiLuResult());
                }
            }
        }

        public override void OnClose()
        {
            base.OnClose();
            ShiYanXuanZeWindowData.Instance.TempObject = null;
        }

        public override void OnHide()
        {
            base.OnHide();
        }

        public override void OnShow(params object[] para)
        {
            base.OnShow(para);
        }

        //初始化
        private void InitData() 
        {
            ShiYanItemContentViewCeShi = GameObject.Find("ShiYanItemContentViewCeShi");
            ShiYanZiYuanXuanZeBtnCeShiTogg = GameObject.Find("ShiYanZiYuanXuanZeBtnCeShi").GetComponent<Toggle>();
            ReturnMainBtnCeShiBtn = GameObject.Find("ReturnMainBtnCeShi").GetComponent<Button>();
            ShiYanCeShiDiQuChooseDropDown = GameObject.Find("ShiYanCeShiDiQuChooseDropDown").GetComponent<Dropdown>();
            NoLoginStatusRightPanelCeShiObj = GameObject.Find("NoLoginStatusRightPanelCeShi");
            LoginStatusRightPanelCeShiObj = GameObject.Find("LoginStatusRightPanelCeShi");
            LoginBtn = GameObject.Find("RigthLoginCeShiBtn").GetComponent<Button>();
            LoginOutBtn = GameObject.Find("LoginOutBtnCeShi").GetComponent<Button>();
            XiuGaiJIChuXinXiPanelCeShiObj = GameObject.Find("XiuGaiJIChuXinXiPanelCeShi");
            JiChuXinXiXiuGaiBtnCeShiBtn = GameObject.Find("JiChuXinXiXiuGaiBtnCeShi").GetComponent<Button>();
            NoShiYanJiLuCeShiObj = GameObject.Find("NoShiYanJiLuCeShi");
            GengDuoJiLuBtn = GameObject.Find("GengDuoJiLuBtnCeShi").GetComponent<Button>();
            UserIcon = GameObject.Find("UserIconCeShi").GetComponent<Image>();
            UserName = GameObject.Find("UserNameCeShi").GetComponent<Text>();
            FenLeiMoKuaiNameText= GameObject.Find("FenLeiMoKuaiNameText").GetComponent<Text>();
            ShiYanCeShiWindowData.Instance.GetFenLeiMoKuaiNameText = FenLeiMoKuaiNameText;
            XueNianXueDuanText = GameObject.Find("XueDuanXueNianCeShi").GetComponent<Text>();
            ChangeXueDuanXueNianBtnCeShi = GameObject.Find("ChangeXueDuanXueNianBtnCeShi").GetComponent<Button>();
            ShiYanJiLuContenObj = GameObject.Find("JiLuContentInfoCeShi").transform;
            XueKeContentCeShiObj = GameObject.Find("ContentViewCeShi").transform;
            XueKeContentToggGroup = XueKeContentCeShiObj.GetComponent<ToggleGroup>();
            LogoCeShi = GameObject.Find("LogoCeShi");
            ShiYanCeShiLoadingBtnObj = GameObject.Find("ShiYanCeShiLoadingBtn");
            NoQuanXianTipCeShiObj = GameObject.Find("NoQuanXianTipCeShi");
            SubjectScrollRect = GameObject.Find("ShiYanItemContentCeShi").GetComponent<ScrollRect>();
            SubjectScrollRect.onValueChanged.AddListener(SubjectScrollRectOnClick);
            ShiYanXuanZeWindowData.Instance.TempObject = ShiYanItemContentViewCeShi;

            LoginBtn.onClick.AddListener(LoginOnClick);
            LoginOutBtn.onClick.AddListener(LogionOutClick);
            GengDuoJiLuBtn.onClick.AddListener(OpenShiYanJiLuWindowOnClick);
            ChangeXueDuanXueNianBtnCeShi.onClick.AddListener(OpenChangeInfoPanelOnClcik);
            JiChuXinXiXiuGaiBtnCeShiBtn.onClick.AddListener(ChangeJiChuXinXiOnClick);
            ShiYanZiYuanXuanZeBtnCeShiTogg.onValueChanged.AddListener(ChangeShiYanZiYuanOnClick);
            ReturnMainBtnCeShiBtn.onClick.AddListener(ReturnMainOnClick);
            ShiYanCeShiDiQuChooseDropDown.onValueChanged.AddListener(ChangeAreaOnClick);
        }

        //滑动换页
        private void SubjectScrollRectOnClick(Vector2 arg0)
        {
             if (ShiYanCeShiWindowData.Instance.ScrollRectPosValue >= 0.01f && ShiYanCeShiWindowData.Instance.ScrollRectPosValue <= 0.2f)
             {
                 if (s_currentPage <= s_totalPageCount)
                 {
                    s_currentPage += 1;
                    LoadZiYuan();
                 }
             }
        }

        //清除数据
        private void ClearData() 
        {
            XueKeCount = 0;
            ShiYanCeShiWindowData.Instance.ScrollRectPosValue = -1;
            s_currentPage = 1;
            s_totalPageCount = 0;
            FenLeiMoKuaiNameText.text = "";
            foreach (var item in s_ShiYanCeShiCourseItemList)
            {
                Destroy(item);
            }
            s_ShiYanCeShiCourseItemList.Clear();
            LayoutRebuilder.ForceRebuildLayoutImmediate(ShiYanItemContentViewCeShi.GetComponent<RectTransform>());
        }

        //打开修改基础信息的面板
        private void OpenChangeInfoPanelOnClcik()
        {
            XiuGaiJIChuXinXiPanelCeShiObj.SetActive(true);
        }

        //更多实验记录按钮
        private void OpenShiYanJiLuWindowOnClick()
        {
            WindowManager.Hide("ShiYanCeShiWindow");
            WindowManager.Open<ExperimentHistoryWindow>();
        }

        //修改基础信息按钮
        private void ChangeJiChuXinXiOnClick()
        {
            WindowManager.Hide("ShiYanCeShiWindow");
            BaseInfoWindowData.Instance.FromWindow = "ShiYanCeShiWindow";
            WindowManager.Open<BaseInfoWindow>();
        }

        //退出登录
        private void LogionOutClick()
        {
            ShiYanXuanZeWindowData.Instance.GetRecentTextbooksID = "";
            PlayerPrefs.DeleteAll();
            ClearAccountLoginData();
            //WindowManager.ReShow();
            WindowManager.Close("MainWindow");
            WindowManager.Close("ShiYanCeShiWindow");
            WindowManager.Open<MainWindow>();
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
            ShiYanCeShiJiLuWindowData.Instance.SaveShiYanCeShiJiLuResult(null);

            LoginWindowData.Instance.IsLogin = false;
        }

        //登录
        private void LoginOnClick()
        {
            WindowManager.Hide("ShiYanCeShiWindow");
            WindowManager.Open<LoginWindow>();
        }

        //回到首页
        private void ReturnMainOnClick()
        {
            WindowManager.Close("ShiYanCeShiWindow");
            WindowManager.Open<MainWindow>();
        }

        //切换到实验资源选择界面
        private void ChangeShiYanZiYuanOnClick(bool arg0)
        {
            WindowManager.Close("ShiYanCeShiWindow");
            WindowManager.Open<ShiYanXuanZeWindow>();
        }

        //默认设置
        private void DefulatSetting() 
        {
            XueKeCount = 0;
            ShiYanCeShiWindowData.Instance.ScrollRectPosValue = -1;
            s_currentPage = 1;
            s_totalPageCount = 0;
            LogoCeShi.SetActive(true);
            XiuGaiJIChuXinXiPanelCeShiObj.SetActive(false);
            NoShiYanJiLuCeShiObj.SetActive(true);
            ShiYanCeShiLoadingBtnObj.SetActive(false);
            NoQuanXianTipCeShiObj.SetActive(false);
            if (LoginWindowData.Instance.IsLogin)
            {
                NoQuanXianTipCeShiObj.SetActive(false);
                ShiYanCeShiLoadingBtnObj.SetActive(true);
                LoginStatusRightPanelCeShiObj.SetActive(true);
                NoLoginStatusRightPanelCeShiObj.SetActive(false);
            }
            else
            {
                ShiYanCeShiLoadingBtnObj.SetActive(false);
                NoQuanXianTipCeShiObj.SetActive(true);
                LoginStatusRightPanelCeShiObj.SetActive(false);
                NoLoginStatusRightPanelCeShiObj.SetActive(true);
            }
            GetUserInfo();
            GetAreaInfo(ShiYanCeShiWindowData.Instance.GetShiYanCeShiAreaAndXueKeResult());
        }

        //获取用户信息
        public void GetUserInfo() 
        {
            //登录状态
            if (LoginWindowData.Instance.IsLogin)
            {
                LoginStatusRightPanelCeShiObj.SetActive(true);
                NoLoginStatusRightPanelCeShiObj.SetActive(false);
                SetUserInfo();
            }
            else
            {
                LoginStatusRightPanelCeShiObj.SetActive(false);
                NoLoginStatusRightPanelCeShiObj.SetActive(true);
            }
        }

        //给界面的用户信息面板赋值
        private void SetUserInfo() 
        {
            UserName.text = MainWindowData.Instance.GetUserName;
            XueNianXueDuanText.text = BaseInfoWindowData.Instance.SectionName + "·" + BaseInfoWindowData.Instance.GradeName;
            if (string.IsNullOrEmpty(LoginWindowData.Instance.GetUserInfoResult().data.imgUrl))
            {
                UserIcon.gameObject.SetActive(false);
            }
            else
            {
                UserIcon.gameObject.SetActive(true);
                StartCoroutine(LoadImage(LoginWindowData.Instance.GetUserInfoResult().data.imgUrl, UserIcon));
            }
            CreatShiYanJiLuItem(ShiYanCeShiJiLuWindowData.Instance.GetShiYanCeShiJiLuResult());
        }

        //生成试验记录内容Item
        private void CreatShiYanJiLuItem(ShiYanCeShiJiLuResult shiYanCeShiJiLu)
        {
            if (shiYanCeShiJiLu.data.todayList.Count == 0 && shiYanCeShiJiLu.data.weekList.Count == 0 && shiYanCeShiJiLu.data.moreList.Count == 0)
            {
                NoShiYanJiLuCeShiObj.SetActive(true);
                GengDuoJiLuBtn.gameObject.SetActive(false);
                return;
            }
            else
            {
                GengDuoJiLuBtn.gameObject.SetActive(true);
                NoShiYanJiLuCeShiObj.SetActive(false);
                //生成今天的标题Item
                CeratTimeDataItem("今天", shiYanCeShiJiLu.data.todayList);
                CeratTimeDataItem("一周内", shiYanCeShiJiLu.data.weekList);
                CeratTimeDataItem("更早", shiYanCeShiJiLu.data.moreList);
            }
        }

        public void CeratTimeDataItem(string data, List<ShiYanJiLuItemList> yanJiLuItemTodayList)
        {
            if (yanJiLuItemTodayList.Count == 0)
            {
                return;
            }
            GameObject tempObj = ResManager.Instance.Load<GameObject>(ItemType.Item.JiLuTimerItem);
            GameObject item = Instantiate(tempObj);
            item.transform.SetParent(ShiYanJiLuContenObj);
            item.transform.localPosition = Vector3.zero;
            item.transform.localScale = Vector3.one;
            item.transform.localEulerAngles = Vector3.zero;
            item.transform.GetChild(1).GetComponent<Text>().text = data;

            for (int i = 0; i < yanJiLuItemTodayList.Count; i++)
            {
                string name = yanJiLuItemTodayList[i].name;
                string type = yanJiLuItemTodayList[i].type;
                string id = yanJiLuItemTodayList[i].id;
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
                item1.transform.SetParent(ShiYanJiLuContenObj);
                item1.transform.localPosition = Vector3.zero;
                item1.transform.localScale = Vector3.one;
                item1.transform.localEulerAngles = Vector3.zero;
                item1.transform.GetChild(0).GetComponent<Text>().text = name;
                item1.GetComponent<Button>().onClick.AddListener(delegate () { ShiYanJiLuNeRongOnClick(type,id,name,src); });
            }
        }

        //点击实验记录
        private void ShiYanJiLuNeRongOnClick(string type, string id, string name, string src)
        {
            if (type == "COURSE")
            {
                ShiYanCeShiXiangQingMainWindowData.Instance.ShiYanCeShiName = name;
                ShiYanCeShiXiangQingMainWindowData.Instance.ShiYanCeShiId = id;
                ShiYanCeShiXiangQingMainWindowData.Instance.IsLastShiYanCeShiMingXiInfo = true;
                WindowManager.Hide("ShiYanCeShiWindow");
                WindowManager.Open<ShiYanCeShiXiangQingMainWindow>();
            }
            else if (type == "VR")
            {
                UniversalLoadingWindowData.Instance.GetShiYanName = name;
                UniversalLoadingWindowData.Instance.GetShiYanURL = src;
                UniversalLoadingWindowData.Instance.GetPlayMode = ResPlayMode.VRCourse.ToString() ;
                UniversalLoadingWindowData.Instance.ResourcesType = "课件播放";
                WindowManager.Hide("ShiYanCeShiWindow");
                WindowManager.Open<UniversalLoadingWindow>();
            }
        }

        //获取服务器图片资源
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
                    }
                }
            }
            else
            {
                if (image.name == "UserIconCeShi")
                {
                    image.gameObject.SetActive(false);
                }
            }
        }

        //获取地形学科信息
        private void GetAreaInfo(ShiYanCeShiAreaAndXueKeResult ceShiAreaAndXueKeResult) 
        {
            if (ceShiAreaAndXueKeResult == null)
            {
                client.EndPoint = CommonConstant.SERVER_URL_MEC_PROXY;
                client.Method = EnumHttpVerb.POST;
                string result = client.HttpRequest(CommonConstant.GET_SHIYANCESHI_DIQU_XUEKE);
                if (string.IsNullOrEmpty(result))
                {
                    Debug.Log(client.ErrorMessage);
                    return;
                }
                else
                {
                    ShiYanCeShiAreaAndXueKeResult shiYanCeShiAreaAndXueKe = new ShiYanCeShiAreaAndXueKeResult();
                    shiYanCeShiAreaAndXueKe = JsonUtility.FromJson<ShiYanCeShiAreaAndXueKeResult>(result);
                    ShiYanCeShiWindowData.Instance.SaveShiYanCeShiAreaAndXueKeResult(shiYanCeShiAreaAndXueKe);
                    AddAreaItemName(shiYanCeShiAreaAndXueKe);
                }
            }
            else
            {
                AddAreaItemName(ceShiAreaAndXueKeResult);
            }
        }

        //地区下拉狂添加数据
        private void AddAreaItemName(ShiYanCeShiAreaAndXueKeResult shiYanCeShiAreaAndXueKe)
        {
            AreaNameDic.Clear();
            if (shiYanCeShiAreaAndXueKe.data.tag_region.child.Count == 0)
            {
                return;
            }
            for (int i = 0; i < shiYanCeShiAreaAndXueKe.data.tag_region.child.Count; i++)
            {
                AreaNameDic.Add(shiYanCeShiAreaAndXueKe.data.tag_region.child[i].tag_name, shiYanCeShiAreaAndXueKe.data.tag_region.child[i].tag_id);
            }
            AddAreaOptions(AreaNameDic);
            StartCoroutine(AddSubjectItem(shiYanCeShiAreaAndXueKe));
        }

        private void AddAreaOptions(Dictionary<string ,string> areaNameDic) 
        {
            int index = -1;
            ShiYanCeShiDiQuChooseDropDown.ClearOptions();
            Dropdown.OptionData optionData = null;
            foreach (var item in areaNameDic.Keys)
            {
                optionData = new Dropdown.OptionData();
                optionData.text = item;
                ShiYanCeShiDiQuChooseDropDown.options.Add(optionData);
            }
            foreach (var item in areaNameDic.Keys) 
            {
                index += 1;
                if (ShiYanCeShiWindowData.Instance.GetAreaName.Contains(item))
                {
                    ShiYanCeShiDiQuChooseDropDown.captionText.text = item;
                    s_selectRegionTags = AreaNameDic[item];
                    ShiYanCeShiDiQuChooseDropDown.value = index;
                    break;
                }
                else
                {
                    ShiYanCeShiDiQuChooseDropDown.captionText.text = "北京";
                    s_selectRegionTags = AreaNameDic["北京"];
                    ShiYanCeShiDiQuChooseDropDown.value = 0;
                }
            }
        }

        //切换地区
        private void ChangeAreaOnClick(int arg0)
        {
            ClearData();
            ShiYanCeShiLoadingBtnObj.SetActive(true);
            if (AreaNameDic.ContainsKey(ShiYanCeShiDiQuChooseDropDown.captionText.text))
            {
                s_selectRegionTags = AreaNameDic[ShiYanCeShiDiQuChooseDropDown.captionText.text];
                StartCoroutine(ShowSubjectCount(s_selectRegionTags));
                if (s_selectSubjectTags == "")
                {
                    s_selectTags = s_selectRegionTags;
                }
                else
                {
                    s_selectTags = s_selectRegionTags + "," + s_selectSubjectTags;
                }
            }
            //登录状态
            if (LoginWindowData.Instance.IsLogin)
            {
                LoadZiYuan();
            }
            else
            {
                ShiYanCeShiLoadingBtnObj.SetActive(false);
                return;
            }
        }

        //加载学科
        private IEnumerator AddSubjectItem(ShiYanCeShiAreaAndXueKeResult shiYanCeShiAreaAndXueKe) 
        {
            if (shiYanCeShiAreaAndXueKe.data.tag_subject.child.Count == 0)
            {
                yield break;
                //return;
            }
            GameObject go = ResManager.Instance.Load<GameObject>(ItemType.Item.ShiYanCeShiXueKeItem);
            GameObject item = Instantiate(go);
            SubjectItems.Add(item);
            item.transform.parent = XueKeContentCeShiObj;
            item.transform.localPosition = Vector3.zero;
            item.transform.localScale = Vector3.one;
            item.transform.GetChild(2).GetComponent<Text>().text = s_subjectAllName;
            item.GetComponent<Toggle>().group = XueKeContentToggGroup;
            string selectSubjectTagsAll = "";
            Tag_Subjects.Add(selectSubjectTagsAll);
            SubjectItemsName.Add(s_subjectAllName);
            item.GetComponent<Toggle>().onValueChanged.AddListener(delegate(bool arg0) { XueKeChangeOnClick(arg0, item, selectSubjectTagsAll,s_subjectAllName); });
            item.GetComponent<Toggle>().isOn = true;
           
            for (int i = 0; i < shiYanCeShiAreaAndXueKe.data.tag_subject.child.Count; i++)
            {
                GameObject go1 = ResManager.Instance.Load<GameObject>(ItemType.Item.ShiYanCeShiXueKeItem);
                GameObject item1 = Instantiate(go1);
                SubjectItems.Add(item1);
                item1.transform.parent = XueKeContentCeShiObj;
                item1.transform.localPosition = Vector3.zero;
                item1.transform.localScale = Vector3.one;
                string subjectName = shiYanCeShiAreaAndXueKe.data.tag_subject.child[i].tag_name;
                item1.transform.GetChild(2).GetComponent<Text>().text = shiYanCeShiAreaAndXueKe.data.tag_subject.child[i].tag_name;
                item1.GetComponent<Toggle>().group = XueKeContentToggGroup;
                string selectSubjectTagsItem = shiYanCeShiAreaAndXueKe.data.tag_subject.child[i].tag_id;
                Tag_Subjects.Add(selectSubjectTagsItem);
                SubjectItemsName.Add(subjectName);
                item1.GetComponent<Toggle>().onValueChanged.AddListener(delegate (bool arg0) { XueKeChangeOnClick(arg0, item1, selectSubjectTagsItem, subjectName); });
            }
            yield return new WaitForSeconds(0.05f);
            StartCoroutine(ShowSubjectCount(s_selectRegionTags));
        }

        //切换学科
        private void XueKeChangeOnClick(bool arg0, GameObject item, string selectSubjectTagsItem, string s_subjectAllName)
        {
            if (arg0)
            {
                ClearData();
                ShiYanCeShiLoadingBtnObj.SetActive(false);
                s_selectSubjectTags = selectSubjectTagsItem;
                if (selectSubjectTagsItem == "")
                {
                    s_selectTags = s_selectRegionTags;
                }
                else
                {
                    s_selectTags = s_selectRegionTags + "," + selectSubjectTagsItem;
                }
                item.transform.GetChild(1).gameObject.SetActive(false);
                item.transform.GetChild(2).GetComponent<Text>().color = new Color32(255, 252, 206, 255);
                //在登录状态下才有数据
                if (LoginWindowData.Instance.IsLogin)
                {
                    LoadZiYuan();
                }
                else
                {
                    return;
                }
            }
            else
            {
                item.transform.GetChild(2).GetComponent<Text>().color = new Color32(49, 255, 255, 255);
            }
        }

        private void LoadZiYuan() 
        { 
            LoadExperimentalModule();
            StartCoroutine(GetLoadExperimentalModuleItem(ShiYanCeShiWindowData.Instance.GetShiYanCeShiZiYuanResult()));
        }

        //加载实验各个模块数据，保存本地
        private void LoadExperimentalModule()
        {
            client.EndPoint = CommonConstant.SERVER_URL_MEC_PROXY;
            client.Method = EnumHttpVerb.POST;
            ShiYanCeShiZiYuanRequest ceShiZiYuanRequest = new ShiYanCeShiZiYuanRequest();
            ceShiZiYuanRequest.offset = (s_currentPage - 1) * s_limitItem;
            ceShiZiYuanRequest.limit = s_limitItem;
            ceShiZiYuanRequest.tags = s_selectTags;
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

            ceShiZiYuanRequest.macInfo = macInfo;
            client.PostData = JsonUtility.ToJson(ceShiZiYuanRequest);
            string result = client.HttpRequest(CommonConstant.GET_SHIYANCESHIZIYUAN_LIST);
            if (string.IsNullOrEmpty(result))
            {
                Debug.Log(client.ErrorMessage);
                return;
            }
            else
            {
                ShiYanCeShiZiYuanResult yanCeShiZiYuanResult = new ShiYanCeShiZiYuanResult();
                yanCeShiZiYuanResult = JsonUtility.FromJson<ShiYanCeShiZiYuanResult>(result);
                ShiYanCeShiWindowData.Instance.SaveShiYanCeShiZiYuanResult(yanCeShiZiYuanResult);
            }
        }

        //加载实验资源
        private IEnumerator GetLoadExperimentalModuleItem(ShiYanCeShiZiYuanResult yanCeShiZiYuanResult) 
        {
            if (yanCeShiZiYuanResult.data.total % s_limitItem != 0)
            {
                s_totalPageCount = Convert.ToInt32(yanCeShiZiYuanResult.data.total / s_limitItem) + 1;
            }
            else
            {
                s_totalPageCount = Convert.ToInt32(yanCeShiZiYuanResult.data.total / s_limitItem);
            }

            if (yanCeShiZiYuanResult.data.requireData.total != 0)
            {
                LogoCeShi.SetActive(false);
                FenLeiMoKuaiNameText.text = "必修";
                CreatItem("必修", yanCeShiZiYuanResult.data.requireData,true);
            }
            else
            {
                LogoCeShi.SetActive(true);
                if (yanCeShiZiYuanResult.data.skillData.total != 0) 
                {
                    LogoCeShi.SetActive(false);
                    FenLeiMoKuaiNameText.text = "技能提升";
                }
                else 
                {
                    LogoCeShi.SetActive(true);
                    if (yanCeShiZiYuanResult.data.courseData.total != 0) 
                    {
                        LogoCeShi.SetActive(false);
                        FenLeiMoKuaiNameText.text = "课程模块";
                    }
                    else
                    {
                        LogoCeShi.SetActive(true);
                        FenLeiMoKuaiNameText.text = "";
                    }   
                }
            }
            if (yanCeShiZiYuanResult.data.skillData.total != 0)
            {
                if (yanCeShiZiYuanResult.data.requireData.total == 0)
                {
                    CreatItem("技能提升", yanCeShiZiYuanResult.data.skillData, true);
                }
                else
                {
                    CreatItem("技能提升", yanCeShiZiYuanResult.data.skillData, false);
                }
            }
            if (yanCeShiZiYuanResult.data.courseData.total != 0)
            {
                if (yanCeShiZiYuanResult.data.skillData.total == 0 && yanCeShiZiYuanResult.data.requireData.total == 0) 
                {
                    CreatItem("课程模块", yanCeShiZiYuanResult.data.courseData, true);
                }
                else
                {
                    CreatItem("课程模块", yanCeShiZiYuanResult.data.courseData, false);
                }
            }
            yield return new WaitForSeconds(0f);
        }

        public void CreatItem(string name, ShiYanCeShiZiYuanDataResult shiYanCeShiZiYuanData,bool isShow) 
        {
            GameObject go = ResManager.Instance.Load<GameObject>(ItemType.Item.ShiYanCeShiViewItem);
            GameObject item = Instantiate(go);
            item.transform.parent = ShiYanItemContentViewCeShi.transform;
            item.transform.localPosition = Vector3.zero;
            item.transform.localEulerAngles = Vector3.zero;
            item.transform.localScale = Vector3.one;
            item.transform.GetChild(0).GetComponent<Text>().text = name;
            if (isShow)
            {
                item.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(1178f, 0);
            }
            Transform itemParent = item.transform.GetChild(1).transform;
            s_ShiYanCeShiCourseItemList.Add(item);
            for (int i = 0; i < shiYanCeShiZiYuanData.resource_info.Count; i++)
            {
                GameObject go1 = ResManager.Instance.Load<GameObject>(ItemType.Item.ShiYanItem);
                GameObject item1 = Instantiate(go1);
                item1.transform.parent = itemParent;
                item1.transform.localScale = Vector3.one;
                item1.transform.localPosition = Vector3.zero;
                item1.transform.localEulerAngles = Vector3.zero;
                ShiYanCeShiZiYuanInfoDataResult ceShiZiYuanInfoDataResult = shiYanCeShiZiYuanData.resource_info[i];
                string id = shiYanCeShiZiYuanData.resource_info[i].id;
                string src = shiYanCeShiZiYuanData.resource_info[i].src;
                string nameT = shiYanCeShiZiYuanData.resource_info[i].name;
                int experiment_nums = shiYanCeShiZiYuanData.resource_info[i].test.experiment_nums;
                int process = shiYanCeShiZiYuanData.resource_info[i].test.process;
                Slider slider = item1.transform.GetChild(0).GetChild(0).GetComponent<Slider>();
                Text processText = item1.transform.GetChild(0).GetChild(1).GetComponent<Text>();
                processText.text = string.Format("已掌握<color=#{0}>{1}/{2}</color>个实验", ColorUtility.ToHtmlStringRGBA(colorText), process,experiment_nums);
                slider.value = (float)process / experiment_nums;
                if (shiYanCeShiZiYuanData.resource_info[i].resource_operations.PLAY == "播放")
                {
                    item1.transform.GetChild(1).gameObject.SetActive(false);
                }
                else
                {
                    item1.transform.GetChild(1).gameObject.SetActive(true);
                }
                item1.GetComponent<Button>().onClick.AddListener(delegate () { ShiYanCeShiItemOnClick(item1.transform.GetChild(1).gameObject,src, nameT,id, ceShiZiYuanInfoDataResult); });
                StartCoroutine(LoadImage(shiYanCeShiZiYuanData.resource_info[i].thumbnail, item1.GetComponent<Image>()));
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(ShiYanItemContentViewCeShi.GetComponent<RectTransform>());
            BoxCollider collider = item.AddComponent<BoxCollider>();
            collider.isTrigger = true;
            float width = item.GetComponent<RectTransform>().rect.width;
            float heigth = LayoutUtility.GetPreferredHeight(item.transform.GetChild(1).GetComponent<RectTransform>()) + 44f;
            collider.center = new Vector3(1178/2f, -(heigth / 2), 0);
            collider.size = new Vector3(width, heigth, 0);
        }
        //点击实验测试的课程
        private void ShiYanCeShiItemOnClick(GameObject gameObject, string src, string name, string id, ShiYanCeShiZiYuanInfoDataResult ceShiZiYuanInfoDataResult)
        {
            if (gameObject.activeSelf)
            {
                NoQuanXianTipCeShiObj.SetActive(true);
            }
            else
            {
                ShiYanCeShiWindowData.Instance.GetShiYanCeShiZiYuanInfoDataResult = ceShiZiYuanInfoDataResult;
                NoQuanXianTipCeShiObj.SetActive(false);
                gameObject.transform.parent.transform.GetChild(2).gameObject.SetActive(false);
                ShiYanCeShiXiangQingMainWindowData.Instance.ShiYanCeShiName = name;
                ShiYanCeShiXiangQingMainWindowData.Instance.ShiYanCeShiId = id;
                ShiYanCeShiXiangQingMainWindowData.Instance.IsLastShiYanCeShiMingXiInfo = true;
                //调用视博云播放器---------------------
                WindowManager.Hide("ShiYanCeShiWindow");
                WindowManager.Open<ShiYanCeShiXiangQingMainWindow>();
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
            MovePanelByJoystick();
        }

        //显示每个学科分类下的数量
        private IEnumerator ShowSubjectCount(string s_selectRegionTags) 
        {
            for (int i = 0; i < SubjectItemsName.Count; i++)
            {
                SubjectItems[i].transform.GetChild(2).GetComponent<Text>().text = SubjectItemsName[i];
            }
            client.EndPoint = CommonConstant.SERVER_URL_MEC_PROXY;
            client.Method = EnumHttpVerb.POST;
            for (int i = 0; i < Tag_Subjects.Count; i++)
            {
                ShiYanCeShiZiYuanRequest ceShiZiYuanRequest = new ShiYanCeShiZiYuanRequest();
                ceShiZiYuanRequest.offset = s_currentPage - 1;
                ceShiZiYuanRequest.limit = s_limitItem;
                if (Tag_Subjects[i] == "")
                {
                    s_selectTags = s_selectRegionTags;
                }
                else
                {
                    s_selectTags = s_selectRegionTags + "," + Tag_Subjects[i];
                }
                ceShiZiYuanRequest.tags = s_selectTags;
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

                ceShiZiYuanRequest.macInfo = macInfo;
                client.PostData = JsonUtility.ToJson(ceShiZiYuanRequest);
                string result = client.HttpRequest(CommonConstant.GET_SHIYANCESHIZIYUAN_LIST);
                if (string.IsNullOrEmpty(result))
                {
                    Debug.Log(client.ErrorMessage);
                    yield break;
                    //return;
                }
                else
                {
                    ShiYanCeShiZiYuanResult yanCeShiZiYuanResult = new ShiYanCeShiZiYuanResult();
                    yanCeShiZiYuanResult = JsonUtility.FromJson<ShiYanCeShiZiYuanResult>(result);
                    ShiYanCeShiWindowData.Instance.SaveShiYanCeShiZiYuanResult(yanCeShiZiYuanResult);
                    XueKeCount = yanCeShiZiYuanResult.data.requireData.resource_info.Count + yanCeShiZiYuanResult.data.skillData.resource_info.Count + yanCeShiZiYuanResult.data.courseData.resource_info.Count;
                    SubjectItems[i].transform.GetChild(2).GetComponent<Text>().text = SubjectItems[i].transform.GetChild(2).GetComponent<Text>().text + "(" + XueKeCount + ")";
                }
            }
            yield return new WaitForSeconds(0.5f);
            ShiYanCeShiLoadingBtnObj.SetActive(false);
        }
    }
}


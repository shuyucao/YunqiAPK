using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Config;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.IO;
using System;
using Assets.Scripts.Manager;
using Assets.Scripts.Tool;
using Assets.Scripts.Util;
using static Assets.Scripts.Tool.RestFulClient;
using Assets.Scripts.Result;
using Assets.Scripts.Request;
using Assets.Scripts.Constant;
using UnityEngine.Networking;
using Assets.Scripts.Data;

namespace Assets.Scripts.Windows
{
    public class MainWindow : BaseWindow
    {
        RestClient client = new RestClient();

        //3本近期教材物体
        private GameObject OneBookObj;
        private GameObject TwoBookObj;
        private GameObject ThreeBookObj;

        //实验测试按钮
        private Button ShiYanCeShiBtn;

        //学段年级按钮
        private Button XueDuanNianJiChooseBtn;

        //显示学段年级的文本
        private Text ShowXueDuanNianJiTxt;

        //未登录界面
        private GameObject NoLoginStatusObj;

        //登录按钮
        private Button LoginBtn;

        //登录状态界面
        private GameObject LoginStatusObj;
        //用户图标
        private Image UserIconImg;
        //用户名称
        private Text UserNameTxt;
        //显示登录面板下学段年级的文本
        private Text ShowLoginXueDuanNianJiTxt;

        //试验记录按钮
        private Button ShiYanJiLuBtn;

        private Button ShiYanZiYuanBtn;

        //存储近期3本教程的物体
        private List<GameObject> s_ThreeJiaoCaiItemList = new List<GameObject>();
        public override void OnInit()
        {
            base.OnInit();
            prefabType = PrefabType.Window.MainWindow;
            prefab = gameObject;
        }

        //初始化
        public override void Init()
        {
            base.Init();
            InitData();
            AutoLogin();
            DefualtSetting();
        }

        //自动登录
        public void AutoLogin() 
        {
            if (PlayerPrefs.HasKey("account") && PlayerPrefs.HasKey("password"))
            {
                if (LoginWindowData.Instance.IsLogin)
                {
                    return;
                }
                GetSessions();
                AccountLoginOnClick();
            }
        }

        //账号登录按钮
        private void AccountLoginOnClick()
        {
            client.EndPoint = CommonConstant.SERVER_URL_MEC_PROXY_UC;
            client.Method = EnumHttpVerb.POST;
            AccountLoginRequest alr = new AccountLoginRequest();
            alr.login_name = CommonUtil.EncryptDes(PlayerPrefs.GetString("account"), LoginWindowData.Instance.ReadSessionResult().session_key);
            alr.login_name_type = "101_login_name";
            alr.org_code = "";
            alr.password = CommonUtil.EncryptDes(CommonUtil.EncryptMD5_Salt(PlayerPrefs.GetString("password")), LoginWindowData.Instance.ReadSessionResult().session_key);
            alr.session_id = LoginWindowData.Instance.ReadSessionResult().session_id;
            client.PostData = JsonUtility.ToJson(alr);
            //发送并获取返回数据
            string result = client.HttpRequest(CommonConstant.SERVER_URL_ACCOUNT_LOGIN);
            Debug.Log("登录信息：" + result);
            //如果登录失败
            if (String.IsNullOrEmpty(result))
            {
                Debug.Log(client.ErrorMessage);
                return;
            }
            //如果登录成功
            else
            {
                Debug.Log("登录成功");
                AccountLoginResult accountLoginResult = new AccountLoginResult();
                accountLoginResult = JsonUtility.FromJson<AccountLoginResult>(result);
                //保存返回数据
                LoginWindowData.Instance.SaveAccountLoginResult(accountLoginResult);

                GetUserDataInfo(accountLoginResult.user_id, accountLoginResult.access_token, accountLoginResult.mac_key);

                //获取实验记录
                GetShiYanJiLu(accountLoginResult);
                LoginWindowData.Instance.IsLogin = true;
            }
         }
        //获取账号登录实验记录
        private void GetShiYanJiLu(AccountLoginResult accountLoginResult)
        {
            client.EndPoint = CommonConstant.SERVER_URL_MEC_PROXY;
            client.Method = EnumHttpVerb.POST;
            ShiYanCeShiJiLuRequest shiYanCeShiJiLu = new ShiYanCeShiJiLuRequest();
            MacInfoRequest macInfo = new MacInfoRequest();
            shiYanCeShiJiLu.page = 0;
            shiYanCeShiJiLu.size = 10;
            macInfo.mac_key = accountLoginResult.mac_key;
            macInfo.token = accountLoginResult.access_token;
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
                ShiYanCeShiJiLuWindowData.Instance.SaveShiYanCeShiJiLuResult(shiYanCeShiJiLuResult);
            }
        }

        //获取用户信息
        private void GetUserDataInfo(string userId, string token, string mac_key)
        {
            client.EndPoint = CommonConstant.SERVER_URL_MEC_PROXY;
            client.Method = EnumHttpVerb.POST;
            UserInfoRequest userInfo = new UserInfoRequest();
            MacInfoRequest macInfo = new MacInfoRequest();
            macInfo.session_key = LoginWindowData.Instance.ReadSessionResult().session_key;
            macInfo.mac_key = mac_key;
            macInfo.token = token;
            userInfo.macInfo = macInfo;
            userInfo.userId = userId;
            client.PostData = JsonUtility.ToJson(userInfo);
            string userResult = client.HttpRequest(CommonConstant.GET_USERINFO);
            Debug.Log("用户信息：" + userResult);
            if (string.IsNullOrEmpty(userResult))
            {
                Debug.Log(client.ErrorMessage);
                return;
            }
            else
            {
                UserInfoResult userInfoResult = new UserInfoResult();
                userInfoResult = JsonUtility.FromJson<UserInfoResult>(userResult);
                LoginWindowData.Instance.SaveUserInfoResult(userInfoResult);
                BaseInfoWindowData.Instance.SectionName = userInfoResult.data.section_name;
                BaseInfoWindowData.Instance.GradeName = userInfoResult.data.grade_name;
            }
        }

        //初始化窗口并绑定组件
        private void InitData()
        {
            s_ThreeJiaoCaiItemList.Clear();
            OneBookObj = GameObject.Find("One");
            s_ThreeJiaoCaiItemList.Add(OneBookObj);
            TwoBookObj = GameObject.Find("Two");
            s_ThreeJiaoCaiItemList.Add(TwoBookObj);
            ThreeBookObj = GameObject.Find("Three");
            s_ThreeJiaoCaiItemList.Add(ThreeBookObj);


            ShiYanZiYuanBtn = GameObject.Find("ShiYanZiYuanMainBtn").GetComponent<Button>();
            ShiYanZiYuanBtn.onClick.AddListener(ShiYanZiYuanOnClick);

            ShiYanCeShiBtn = GameObject.Find("ShiYanCeShi").GetComponent<Button>();
            ShiYanJiLuBtn = GameObject.Find("ShiYanJiLu").GetComponent<Button>();
            XueDuanNianJiChooseBtn = GameObject.Find("ChangeXueDuanNianJiBtn").GetComponent<Button>();
            ShowXueDuanNianJiTxt = GameObject.Find("ShowXueDuanNianJiText").GetComponent<Text>();

            LoginStatusObj = GameObject.Find("LoginStatus");
            NoLoginStatusObj = GameObject.Find("NoLoginStatus");
            LoginBtn = NoLoginStatusObj.transform.GetChild(0).GetComponent<Button>();

            UserIconImg = LoginStatusObj.transform.GetChild(0).transform.GetChild(0).GetComponent<Image>();
            UserNameTxt = LoginStatusObj.transform.GetChild(1).GetComponent<Text>();
            ShowLoginXueDuanNianJiTxt = LoginStatusObj.transform.GetChild(2).GetComponent<Text>();

            ShiYanCeShiBtn.onClick.AddListener(ShiYanCeShiOnClick);
            ShiYanJiLuBtn.onClick.AddListener(ShiYanJiLuOnClick);
            LoginBtn.onClick.AddListener(LoginOnClick);
            XueDuanNianJiChooseBtn.onClick.AddListener(XueDuanNianJiChooseBtnOnClick);
        }

        //点击实验资源
        private void ShiYanZiYuanOnClick()
        {
            WindowManager.Hide("MainWindow");
            TextbookSelectWindowData.Instance.FromWindow = "MainWindow";
            WindowManager.Open<TextbookSelectWindow>();
        }

        //学段年级按钮
        private void XueDuanNianJiChooseBtnOnClick()
        {
            WindowManager.Hide("MainWindow");
            BaseInfoWindowData.Instance.FromWindow = "MainWindow";
            WindowManager.Open<BaseInfoWindow>();
        }

        //登录
        private void LoginOnClick()
        {
            WindowManager.Hide("MainWindow");
            WindowManager.Open<LoginWindow>();
        }

        //实验记录
        private void ShiYanJiLuOnClick()
        {
            WindowManager.Hide("MainWindow");
            WindowManager.Open<ExperimentHistoryWindow>();
        }

        //实验测试
        private void ShiYanCeShiOnClick()
        {
            WindowManager.Hide("MainWindow");
            WindowManager.Open<ShiYanCeShiWindow>();
        }

        //默认设置
        private void DefualtSetting()
        {
            if (LoginWindowData.Instance.IsLogin)
            {
                NoLoginStatusObj.SetActive(false);
                LoginStatusObj.SetActive(true);
                MainWindowData.Instance.GetUserName = LoginWindowData.Instance.GetUserInfoResult().data.name;
                ShowXueDuanNianJiTxt.text = BaseInfoWindowData.Instance.SectionName + "·" + BaseInfoWindowData.Instance.GradeName;
                ShowLoginXueDuanNianJiTxt.text = ShowXueDuanNianJiTxt.text;
                UserNameTxt.text = MainWindowData.Instance.GetUserName;
                if (string.IsNullOrEmpty(LoginWindowData.Instance.GetUserInfoResult().data.imgUrl))
                {
                    UserIconImg.gameObject.SetActive(false);
                }
                else
                {
                    UserIconImg.gameObject.SetActive(true);
                    StartCoroutine(AddImage(LoginWindowData.Instance.GetUserInfoResult().data.imgUrl, UserIconImg));
                }
                GetUserChooseInfo();
            }
            else
            {
                //未登录加载本地的近期教程列表------------------
                NoLoginStatusObj.SetActive(true);
                LoginStatusObj.SetActive(false);
                ShowXueDuanNianJiTxt.text = BaseInfoWindowData.Instance.SectionName + "·" + BaseInfoWindowData.Instance.GradeName;
                ShowLoginXueDuanNianJiTxt.text = "";
                UserIconImg.sprite = null;
                UserNameTxt.text = "";
            }
            LoadJiaoCaiQuShengImage(MainWindowData.Instance.GetUserChooseInfoResult());
        }

        //获取服务器图片资源
        IEnumerator AddImage(string url, Image image)
        {
            WWW www = new WWW(url);
            yield return www;
            if (string.IsNullOrEmpty(www.error))
            {
                if (www.isDone)
                {
                    Texture2D tex = www.texture;
                    Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero);
                    image.sprite = sprite;
                }
            }
            else
            {
                if (image.name == "Icon")
                {
                    image.gameObject.SetActive(false);
                }
            }
        }

        //获取用户选择教材列表
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
            else if(LoginWindowData.Instance.ReadToKenSwapResult() != null)
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

        //加载近期3本教材的缺省图，有数据就替换，没有就显示缺省图
        public void LoadJiaoCaiQuShengImage(UserChooseInfoResult userChooseInfo)
        {
            ClearData();
            if (LoginWindowData.Instance.IsLogin)
            {
                //当列表里面没有数据,加载缺省图
                if (userChooseInfo.data.items.Count == 0)
                {
                    if (s_ThreeJiaoCaiItemList.Count == 0)
                    {
                        return;
                    }
                    else
                    {
                        for (int i = 0; i < s_ThreeJiaoCaiItemList.Count; i++)
                        {
                            //s_ThreeJiaoCaiItemList[i].GetComponent<Image>().sprite = ResManager.Instance.Load<Sprite>(UiType.UI.JiaoCaiQuSheng);
                            //s_ThreeJiaoCaiItemList[i].transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = "";
                            //s_ThreeJiaoCaiItemList[i].transform.GetChild(0).gameObject.SetActive(false);
                            //s_ThreeJiaoCaiItemList[i].transform.GetChild(1).gameObject.SetActive(false);
                            ////s_ThreeJiaoCaiItemList[i].GetComponent<Button>().onClick.AddListener(delegate () { BookOnClick(""); });
                            //s_ThreeJiaoCaiItemList[i].GetComponent<Button>().interactable = false;
                            s_ThreeJiaoCaiItemList[i].SetActive(false);
                        }
                    }
                }
                else
                {
                    //当列表里面有数据,，数据不足3时,有数据的加载教程图片文字，没有的加载缺省图
                    if (userChooseInfo.data.items.Count < 3)
                    {
                        for (int i = 0; i < userChooseInfo.data.items.Count; i++)
                        {
                            string id = userChooseInfo.data.items[i].teaching_material_id;
                            s_ThreeJiaoCaiItemList[i].transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = userChooseInfo.data.items[i].teaching_material_name;
                            s_ThreeJiaoCaiItemList[i].transform.GetChild(0).gameObject.SetActive(true);
                            s_ThreeJiaoCaiItemList[i].transform.GetChild(1).gameObject.SetActive(false);
                            s_ThreeJiaoCaiItemList[i].GetComponent<Button>().onClick.AddListener(delegate() { BookOnClick(id); });
                            StartCoroutine(AddImage(MainWindowData.Instance.GetUserChooseInfoResult().data.items[i].teaching_material_icon, s_ThreeJiaoCaiItemList[i].GetComponent<Image>()));
                        }
                        for (int i = userChooseInfo.data.items.Count ; i < 3; i++)
                        {
                            //s_ThreeJiaoCaiItemList[i].GetComponent<Image>().sprite = ResManager.Instance.Load<Sprite>(UiType.UI.JiaoCaiQuSheng);
                            //s_ThreeJiaoCaiItemList[i].transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = "";
                            //s_ThreeJiaoCaiItemList[i].transform.GetChild(0).gameObject.SetActive(false);
                            //s_ThreeJiaoCaiItemList[i].transform.GetChild(1).gameObject.SetActive(false);
                            ////s_ThreeJiaoCaiItemList[i].GetComponent<Button>().onClick.AddListener(delegate () { BookOnClick(""); });
                            //s_ThreeJiaoCaiItemList[i].GetComponent<Button>().interactable = false;
                            s_ThreeJiaoCaiItemList[i].SetActive(false);
                        }
                    }
                    else
                    {
                        //当列表里面有数据,，数据大于3时,加载教程图片文字
                        for (int i = 0; i < s_ThreeJiaoCaiItemList.Count; i++)
                        {
                            string teaching_material_id = userChooseInfo.data.items[i].teaching_material_id;
                            s_ThreeJiaoCaiItemList[i].transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = userChooseInfo.data.items[i].teaching_material_name;
                            s_ThreeJiaoCaiItemList[i].transform.GetChild(0).gameObject.SetActive(true);
                            s_ThreeJiaoCaiItemList[i].transform.GetChild(1).gameObject.SetActive(false);
                            s_ThreeJiaoCaiItemList[i].GetComponent<Button>().onClick.AddListener(delegate () { BookOnClick(teaching_material_id); });
                            StartCoroutine(AddImage(MainWindowData.Instance.GetUserChooseInfoResult().data.items[i].teaching_material_icon, s_ThreeJiaoCaiItemList[i].GetComponent<Image>()));
                        }
                    }
                }
            }
            else
            {
                //读取本地的--------------
                if (TextbookMgrWindowData.Instance.userChooseItemResults.Count == 0)
                {
                    if (s_ThreeJiaoCaiItemList.Count == 0)
                    {
                        return;
                    }
                    for (int i = 0; i < s_ThreeJiaoCaiItemList.Count; i++)
                    {
                        //s_ThreeJiaoCaiItemList[i].GetComponent<Image>().sprite = ResManager.Instance.Load<Sprite>(UiType.UI.JiaoCaiQuSheng);
                        //s_ThreeJiaoCaiItemList[i].transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = "";
                        //s_ThreeJiaoCaiItemList[i].transform.GetChild(0).gameObject.SetActive(false);
                        //s_ThreeJiaoCaiItemList[i].transform.GetChild(1).gameObject.SetActive(false);
                        ////s_ThreeJiaoCaiItemList[i].GetComponent<Button>().onClick.AddListener(delegate () { BookOnClick(""); });
                        //s_ThreeJiaoCaiItemList[i].GetComponent<Button>().interactable = false;
                        s_ThreeJiaoCaiItemList[i].SetActive(false);
                    }
                }
                else
                {
                    //当列表里面有数据,，数据不足3时,有数据的加载教程图片文字，没有的加载缺省图
                    if (TextbookMgrWindowData.Instance.userChooseItemResults.Count < 3)
                    {
                        for (int i = 0; i < TextbookMgrWindowData.Instance.userChooseItemResults.Count; i++)
                        {
                            string id = TextbookMgrWindowData.Instance.userChooseItemResults[i].teaching_material_id;
                            s_ThreeJiaoCaiItemList[i].transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = TextbookMgrWindowData.Instance.userChooseItemResults[i].teaching_material_name;
                            s_ThreeJiaoCaiItemList[i].transform.GetChild(0).gameObject.SetActive(true);
                            s_ThreeJiaoCaiItemList[i].transform.GetChild(1).gameObject.SetActive(false);
                            s_ThreeJiaoCaiItemList[i].GetComponent<Button>().onClick.AddListener(delegate () { BookOnClick(id); });
                            StartCoroutine(AddImage(TextbookMgrWindowData.Instance.userChooseItemResults[i].teaching_material_icon, s_ThreeJiaoCaiItemList[i].GetComponent<Image>()));
                        }
                        for (int i = TextbookMgrWindowData.Instance.userChooseItemResults.Count; i < 3; i++)
                        {
                            //s_ThreeJiaoCaiItemList[i].GetComponent<Image>().sprite = ResManager.Instance.Load<Sprite>(UiType.UI.JiaoCaiQuSheng);
                            //s_ThreeJiaoCaiItemList[i].transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = "";
                            //s_ThreeJiaoCaiItemList[i].transform.GetChild(0).gameObject.SetActive(false);
                            //s_ThreeJiaoCaiItemList[i].transform.GetChild(1).gameObject.SetActive(false);
                            ////s_ThreeJiaoCaiItemList[i].GetComponent<Button>().onClick.AddListener(delegate () { BookOnClick(""); });
                            //s_ThreeJiaoCaiItemList[i].GetComponent<Button>().interactable = false;
                            s_ThreeJiaoCaiItemList[i].SetActive(false);
                        }
                    }
                    else
                    {
                        //当列表里面有数据,，数据大于3时,加载教程图片文字
                        for (int i = 0; i < s_ThreeJiaoCaiItemList.Count; i++)
                        {
                            string teaching_material_id = TextbookMgrWindowData.Instance.userChooseItemResults[i].teaching_material_id;
                            s_ThreeJiaoCaiItemList[i].transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = TextbookMgrWindowData.Instance.userChooseItemResults[i].teaching_material_name;
                            s_ThreeJiaoCaiItemList[i].transform.GetChild(0).gameObject.SetActive(true);
                            s_ThreeJiaoCaiItemList[i].transform.GetChild(1).gameObject.SetActive(false);
                            s_ThreeJiaoCaiItemList[i].GetComponent<Button>().onClick.AddListener(delegate () { BookOnClick(teaching_material_id); });
                            StartCoroutine(AddImage(TextbookMgrWindowData.Instance.userChooseItemResults[i].teaching_material_icon, s_ThreeJiaoCaiItemList[i].GetComponent<Image>()));
                        }
                    }
                }
            }
        }

        //点击近期教材
        private void BookOnClick(string teaching_material_id)
        {
            ShiYanXuanZeWindowData.Instance.GetRecentTextbooksID = teaching_material_id;
            for (int i = 0; i < s_ThreeJiaoCaiItemList.Count; i++)
            {
                s_ThreeJiaoCaiItemList[i].transform.GetChild(1).gameObject.SetActive(false);
            }
            WindowManager.Hide("MainWindow");

            WindowManager.Open<ShiYanXuanZeWindow>();
        }

        public void ClearData() 
        {
            s_ThreeJiaoCaiItemList.Clear();
            OneBookObj = GameObject.Find("One");
            s_ThreeJiaoCaiItemList.Add(OneBookObj);
            TwoBookObj = GameObject.Find("Two");
            s_ThreeJiaoCaiItemList.Add(TwoBookObj);
            ThreeBookObj = GameObject.Find("Three");
            s_ThreeJiaoCaiItemList.Add(ThreeBookObj);
        }
        public override void OnShow(params object[] para)
        {
            base.OnShow(para);
        }

        public override void OnClose()
        {
            base.OnClose();
        }

        //获取Sessions
        public void GetSessions()
        {
            client.EndPoint = CommonConstant.SERVER_URL_MEC_PROXY_UC;
            client.Method = EnumHttpVerb.POST;
            CreateSessionRequest csr = new CreateSessionRequest();
            csr.device_id = CommonUtil.GenerateDeviceId(SystemInfo.deviceUniqueIdentifier);
            //Json序列化
            client.PostData = JsonUtility.ToJson(csr);
            //请求访问生成session
            string result = client.HttpRequest(CommonConstant.SERVER_URL_SESSION_GET);
            Debug.Log("sessionKey :" + result);
            CreateSessionResult createSessionResult = new CreateSessionResult();
            createSessionResult = JsonUtility.FromJson<CreateSessionResult>(result);
            LoginWindowData.Instance.SaveSessionResult(createSessionResult);//保存session信息
        }
    }
}

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
using Assets.Scripts.Tool.KeyBoardGrid;
using BestHTTP;
using System.Text;

namespace Assets.Scripts.Windows
{
    public class LoginWindow : BaseWindow
    {
        RestClient client = new RestClient();
        //返回按钮
        private Button ReturnBtn;
        //账号登录Toggle
        private Toggle AccountTogg;
        //手机登录Toggle
        private Toggle IphoneTogg;

        //账号登录面板
        private GameObject AccountLoginPanel;
        //账号输入框
        private CustomInput AccountInputField;
        //密码输入框
        private CustomInput PasswordInputField;
        //验证码输入框
        private CustomInput VerifyCodeInputField;

        //验证码输入框
        private CustomInput IphoneVerifyCodeInputField;

        //获取验证码的按钮
        private Button GetVerifyCodeBtn;
        private Button GetIphoneVerifyCodeBtn;
        //验证码图片
        private Image VerifyCodeImg;
        private Image IphoneVerifyCodeImg;

        //账号不存在或错误的提示
        private GameObject AccountGridObj;
        private GameObject AccountErrorTip;
        private Text AccountErrorTipMsgTxt;

        //密码错误的提示
        private GameObject PasswordGridObj;
        private GameObject PasswordErrorTip;
        private Text PasswordErrorTipMsgTxt;

        //验证码错误的提示
        private GameObject VerifyCodeGridObj;
        private GameObject VerifyCodeErrorTip;
        private Text VerifyCodeErrorTipMsgTxt;

        //手机登录方式验证码错误的提示
        private GameObject IphoneVerifyCodeGridObj;
        private GameObject IphoneVerifyCodeGridErrorTip;
        private Text IphoneVerifyCodeGErrorTipMsgTxt;

        //账号登录按钮
        private Button AccoutLoginBtn;
        private GameObject AccoutLoginImgObj;

        //手机登录面板
        private GameObject IphoneLoginPanel;
        //手机号输入框
        private CustomInput IphoneNumInputField;
        //短信验证码输入框
        private CustomInput IphoneCodeInputField;
        //短信验证码错误提示
        private GameObject IponeCodeErrorTxt;

        private GameObject IphoneNumErrorMsgText;

        //获取短信验证码的按钮
        private Button GetIphoneCodeBtn;
        private Text GetIphoneCodeTipTxt;

        //手机登录按钮
        private Button IphoneLoginBtn;
        private GameObject IphoneLoginImgObj;
        //手机号登录多次失败提示
        private GameObject MoreErrorTipTxt;

        //网络错误提示文本
        private GameObject NetErrorTxt;

        //获取手机短信验证码的倒计时
        private int s_Timer = 60;

        //是否需要验证码
        private bool isNeedIDC = false;

        public bool IsNeedIDC
        {
            get { return isNeedIDC; }
        }

        //用户名
        private string s_UserName = "";
        //密码
        private string s_Password = "";
        //图片验证码
        private string s_IDCode = "";
        //手机号
        private string s_PhoneNumber = "";
        //短信验证码
        string s_PhoneCode = "";

        //隐私政策和协议
        private Button YinSiZhengCeBtn;
        private Button YongHuXieYiBtn;

        //判断是账号登录还是手机登录
        private string AocountOrIphoneLogin = "";
        public override void OnInit()
        {
            base.OnInit();
            prefabType = PrefabType.Window.LoginWindow;
            prefab = gameObject;
        }

        //初始化
        public override void Init()
        {
            base.Init();
            InitData();
            DefulatSetting();
            if (!LoginWindowData.Instance.IsLogin)
            {
                GetSessions();
            }
        }

        //初始化绑定物体
        private void InitData()
        {
            ReturnBtn = GameObject.Find("ReturnBtn").GetComponent<Button>();
            ReturnBtn.onClick.AddListener(ReturnOnClick);
            NetErrorTxt = GameObject.Find("NetErrorTip");

            YinSiZhengCeBtn = GameObject.Find("YinSiZhengCe").GetComponent<Button>();
            YinSiZhengCeBtn.onClick.AddListener(YinSiZhengCeOnClick);
            YongHuXieYiBtn = GameObject.Find("YongHuXieYi").GetComponent<Button>();
            YongHuXieYiBtn.onClick.AddListener(YongHuXieYiOnClick);

            AccountTogg = GameObject.Find("AccountLogin").GetComponent<Toggle>();
            AccountTogg.onValueChanged.AddListener(AccountLoginToggOnClick);
            AccountTogg.isOn = true;
            IphoneTogg = GameObject.Find("IphoneLogin").GetComponent<Toggle>();
            IphoneTogg.onValueChanged.AddListener(IphoneLoginToggOnClick);
            IphoneTogg.isOn = false;

            //账号登录面板下的组件
            AccountLoginPanel = GameObject.Find("AccountLoginPanel");
            AccountGridObj = GameObject.Find("AccountGrid");
            PasswordGridObj = GameObject.Find("PasswordGrid");
            VerifyCodeGridObj = GameObject.Find("VerifyCodeGrid");
            AccountErrorTip = GameObject.Find("AccountErrorTip");
            PasswordErrorTip = GameObject.Find("PasswordErrorTip");
            VerifyCodeErrorTip = GameObject.Find("VerifyCodeErrorTip");
            AccountErrorTipMsgTxt = AccountErrorTip.transform.GetChild(0).GetComponent<Text>();
            PasswordErrorTipMsgTxt = AccountErrorTip.transform.GetChild(0).GetComponent<Text>();
            VerifyCodeErrorTipMsgTxt = VerifyCodeErrorTip.transform.GetChild(0).GetComponent<Text>();

            AccountInputField = GameObject.Find("AccountInputField").GetComponent<CustomInput>();
            PasswordInputField = GameObject.Find("PasswordInputField").GetComponent<CustomInput>();
            VerifyCodeInputField = GameObject.Find("VerifyCodeInputField").GetComponent<CustomInput>();
            GetVerifyCodeBtn = GameObject.Find("CodeBtn").GetComponent<Button>();
            VerifyCodeImg = GameObject.Find("CodeBtn").GetComponent<Image>();
            AccoutLoginBtn = GameObject.Find("AccoutLoginBtn").GetComponent<Button>();
            AccoutLoginImgObj = GameObject.Find("AccoutLoginBtn").transform.GetChild(1).gameObject;

            //手机登录面板下的组件
            IphoneVerifyCodeGridObj = GameObject.Find("IphoneVerifyCodeGrid");
            IphoneVerifyCodeGridErrorTip = GameObject.Find("IphoneVerifyCodeErrorTip");
            IphoneVerifyCodeGErrorTipMsgTxt = IphoneVerifyCodeGridErrorTip.transform.GetChild(0).GetComponent<Text>();
            IphoneVerifyCodeInputField= GameObject.Find("IphoneVerifyCodeInputField").GetComponent<CustomInput>();
            GetIphoneVerifyCodeBtn= GameObject.Find("IphoneCodeBtn").GetComponent<Button>();
            IphoneVerifyCodeImg = GameObject.Find("IphoneCodeBtn").GetComponent<Image>();

            IphoneLoginPanel = GameObject.Find("IphoneLoginPanel");
            MoreErrorTipTxt = GameObject.Find("IponeMoreErrorTip");
            IphoneNumInputField = GameObject.Find("IphoneNumInputField").GetComponent<CustomInput>();
            IphoneCodeInputField = GameObject.Find("IponeCodeInputField").GetComponent<CustomInput>();
            IponeCodeErrorTxt = GameObject.Find("CodeErrorMsgText");
            IphoneNumErrorMsgText = GameObject.Find("IphoneNumErrorMsgText");
            GetIphoneCodeBtn = GameObject.Find("GetSSMCode").GetComponent<Button>();
            GetIphoneCodeTipTxt = GameObject.Find("GetSSMCode").transform.GetChild(0).GetComponent<Text>();
            IphoneLoginBtn = GameObject.Find("IphoneLoginBtn").GetComponent<Button>();
            IphoneLoginImgObj = GameObject.Find("IphoneLoginBtn").transform.GetChild(1).gameObject;

            AccoutLoginBtn.onClick.AddListener(AccountLoginOnClick);
            IphoneLoginBtn.onClick.AddListener(IphoneLoginOnClick);
            GetVerifyCodeBtn.onClick.AddListener(GetVerifyCodeOnClick);
            GetIphoneCodeBtn.onClick.AddListener(GetIphoneCodeOnClick);
            IphoneNumInputField.OnVariableChange += OnIphoneNumChange;
            GetIphoneVerifyCodeBtn.onClick.AddListener(GetVerifyCodeOnClick);
        }

        //用户协议
        private void YongHuXieYiOnClick()
        {
            YinSiZhengCeAndXieYiWindowData.Instance.GetYinSiOrXieYi = "YongHuXieYi";
            WindowManager.Hide("LoginWindow");
            WindowManager.Open<YinSiZhengCeAndXieYiWindow>();
        }

        //隐私政策
        private void YinSiZhengCeOnClick()
        {
            YinSiZhengCeAndXieYiWindowData.Instance.GetYinSiOrXieYi = "YinSiZhengCe";
            WindowManager.Hide("LoginWindow");
            WindowManager.Open<YinSiZhengCeAndXieYiWindow>();
        }

        //手机输入框发生变化时
        private void OnIphoneNumChange(string arg0)
        {
            if (string.IsNullOrEmpty(arg0))
            {
                GetIphoneCodeBtn.interactable = false;
                GetIphoneCodeTipTxt.color = new Color32(79, 75, 75, 255);
            }
            else
            {
                GetIphoneCodeBtn.interactable = true;
                GetIphoneCodeTipTxt.color = new Color32(49, 255, 255, 255);
            }
        }

        //获取手机短信验证码
        private void GetIphoneCodeOnClick()
        {
            //MusicManager.Instance.PlayEffect(MusicType.Music.YinXiao, false);
            GetIphoneCodeBtn.interactable = false;
            GetIphoneCodeTipTxt.color = new Color32(79, 75, 75, 255);
            InvokeRepeating("GetSSMCodeTimer", 0, 1);
            s_PhoneNumber = IphoneNumInputField.TextStr;
            if (isNeedIDC)
            {
                s_IDCode = IphoneVerifyCodeInputField.TextStr;
            }
            client.EndPoint = CommonConstant.SERVER_URL_MEC_PROXY_UC;
            client.Method = EnumHttpVerb.POST;
            SMSCodeSendRequest smsCodeSend = new SMSCodeSendRequest();
            //--------------------------------------------------------------------
            //需要添加到常量
            smsCodeSend.op_type = "3";
            smsCodeSend.country_code = "+86";
            //--------------------------------------------------------------------
            smsCodeSend.mobile = s_PhoneNumber;
            smsCodeSend.identify_code = s_IDCode;
            client.PostData = JsonUtility.ToJson(smsCodeSend);
            string result = client.HttpRequest(CommonConstant.SERVER_URL_SESSION_GET + "/" + LoginWindowData.Instance.ReadSessionResult().session_id + CommonConstant.SERVER_URL_SMS_CODE_SUBMIT);
            if (string.IsNullOrEmpty(result))
            {
                //手机号输入错误，输入正确后可以发送验证码
                CancelInvoke();
                s_Timer = 60;
                GetIphoneCodeTipTxt.color = new Color32(49, 255, 255, 255);
                GetIphoneCodeTipTxt.text = "重新发送";
                GetIphoneCodeBtn.interactable = true;
                ShowError(client.ErrorMessage);
                return;
            }
        }

        private void GetSSMCodeTimer()
        {
            s_Timer -= 1;
            GetIphoneCodeTipTxt.text = s_Timer + "s";
            if (s_Timer == 0)
            {
                CancelInvoke();
                s_Timer = 60;
                GetIphoneCodeTipTxt.color = new Color32(49, 255, 255, 255);
                GetIphoneCodeTipTxt.text = "重新发送";
                GetIphoneCodeBtn.interactable = true;
            }
        }

        //获取图形验证码
        private void GetVerifyCodeOnClick()
        {
            StartCoroutine(LoadImage(client.EndPoint + CommonConstant.SERVER_URL_SESSION_GET + "/" + LoginWindowData.Instance.ReadSessionResult().session_id + CommonConstant.SERVER_URL_IDENTIFY_CODE_GET));
        }

        //显示错误信息
        private void ShowError(string errorMessage)
        {
            if (errorMessage == "需要图形验证码")
            {
                isNeedIDC = true;
                if (AocountOrIphoneLogin == "Iphone")
                {
                    VerifyCodeGridObj.SetActive(false);
                    IphoneVerifyCodeGridObj.SetActive(true);
                }
                else if (AocountOrIphoneLogin == "Account")
                {
                    VerifyCodeGridObj.SetActive(true);
                    IphoneVerifyCodeGridObj.SetActive(false);
                }
                GetVerifyCodeOnClick();
            }
            if (errorMessage == "用户名或密码错误")
            {
                AccountErrorTip.SetActive(true);
            }
            else if (errorMessage == "无效的验证码")
            {
                if (AocountOrIphoneLogin == "Iphone")
                {
                    VerifyCodeErrorTip.SetActive(false);
                    IphoneVerifyCodeGridErrorTip.SetActive(true);
                }
                else if (AocountOrIphoneLogin == "Account")
                {
                    VerifyCodeErrorTip.SetActive(true);
                    IphoneVerifyCodeGridErrorTip.SetActive(false);
                }
            }
            else if (errorMessage == "缺少参数" || errorMessage == "短信验证码不正确" || errorMessage == "短信验证码未下发或已经过期")
            {
                IponeCodeErrorTxt.SetActive(true);
            }
            else if (errorMessage == "手机号码格式不正确")
            {
                IphoneNumErrorMsgText.SetActive(true);
            }
            else if (errorMessage == "短信验证码一天内输入错误次数不能超过上限(5次)!")
            {
                MoreErrorTipTxt.SetActive(true);
            }
        }

        IEnumerator LoadImage(string url)
        {
            UnityWebRequest webRequest = UnityWebRequest.Get(url);
            DownloadHandlerTexture downloadTexture = new DownloadHandlerTexture(true);
            webRequest.downloadHandler = downloadTexture;
            yield return webRequest.SendWebRequest();
            if (webRequest.isDone)
            {
                Texture2D tex = downloadTexture.texture;
                if (VerifyCodeImg != null)
                {
                    VerifyCodeImg.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero);
                }
                if (IphoneVerifyCodeImg != null)
                {
                    IphoneVerifyCodeImg.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero);
                }
            }
        }

        //手机号登录按钮
        private void IphoneLoginOnClick()
        {
            IphoneLoginBtn.interactable = false;
            IphoneLoginImgObj.SetActive(true);
            IphoneLoginBtn.gameObject.transform.GetChild(0).gameObject.SetActive(false);
            //获取字符
            s_PhoneNumber = IphoneNumInputField.TextStr;
            s_PhoneCode = IphoneCodeInputField.TextStr;
            //判断网络
            if (Application.internetReachability == NetworkReachability.NotReachable) 
            {
                IphoneLoginBtn.interactable = true;
                IphoneLoginImgObj.SetActive(false);
                IphoneLoginBtn.gameObject.transform.GetChild(0).gameObject.SetActive(true);
                NetErrorTxt.SetActive(true);
                return;
            }
            if (String.IsNullOrEmpty(s_PhoneNumber))
            {
                IphoneLoginBtn.interactable = true;
                IphoneLoginImgObj.SetActive(false);
                IphoneLoginBtn.gameObject.transform.GetChild(0).gameObject.SetActive(true);
                IphoneLoginBtn.gameObject.transform.GetChild(1).gameObject.SetActive(false);
                return;
            }
            client.EndPoint = CommonConstant.SERVER_URL_MEC_PROXY_UC;
            client.Method = EnumHttpVerb.POST;
            SMSLoginRequest loginRequest = new SMSLoginRequest();
            loginRequest.country_code = "+86";
            loginRequest.mobile = s_PhoneNumber;
            loginRequest.sms_code = s_PhoneCode;
            loginRequest.session_id = LoginWindowData.Instance.ReadSessionResult().session_id;
            client.PostData = JsonUtility.ToJson(loginRequest);
            string result = client.HttpRequest(CommonConstant.SERVER_URL_VARIFY_SMS_LOGIN);
            print(result);
            //如果登录失败
            if (String.IsNullOrEmpty(result))
            {
                GetVerifyCodeOnClick();
                IphoneLoginBtn.interactable = true;
                IphoneLoginImgObj.SetActive(false);
                IphoneLoginBtn.gameObject.transform.GetChild(0).gameObject.SetActive(true);
                ShowError(client.ErrorMessage);
                return;
            }
            //如果登录成功
            else
            {
                //ifNeedIDC = false;
                Debug.Log("登录成功");
                SMSLoginResult sMSLogin = new SMSLoginResult();
                sMSLogin = JsonUtility.FromJson<SMSLoginResult>(result);
                //保存返回数据
                LoginWindowData.Instance.SaveSMSLoginResult(sMSLogin);
                //------------------------------------------------------------------------------------
                //短信登录令牌切换账号密码登录令牌
                ChangeLPLogin(sMSLogin);
                //------------------------------------------------------------------------------------
                //跳转页面
                LoginWindowData.Instance.IsLogin = true;
                WindowManager.Close("LoginWindow");
                WindowManager.Open<MainWindow>();
            }
        }
        //短信登录令牌切换账号密码登录令牌
        private void ChangeLPLogin(SMSLoginResult sMSLogin)
        {
            client.EndPoint = CommonConstant.SERVER_URL_MEC_PROXY;
            client.Method = EnumHttpVerb.POST;
            TokenSwapRequest tokenSwapRequest = new TokenSwapRequest();
            tokenSwapRequest.account_id = sMSLogin.account_id;
            tokenSwapRequest.session_id = LoginWindowData.Instance.ReadSessionResult().session_id;
            MacInfoRequest macInfo = new MacInfoRequest();
            tokenSwapRequest.macInfo = macInfo;
            macInfo.mac_key = sMSLogin.mac_key;
            macInfo.token = sMSLogin.access_token;
            macInfo.session_key = LoginWindowData.Instance.ReadSessionResult().session_key;
            client.PostData = JsonUtility.ToJson(tokenSwapRequest);
            string result = client.HttpRequest(CommonConstant.SERVER_URL_VARIFY_SMS_LPZH);
            if (String.IsNullOrEmpty(result))
            {
                ShowError(client.ErrorMessage);
                return;
            }
            else
            {
                ToKenSwapResult toKenSwapResult = new ToKenSwapResult();
                toKenSwapResult = JsonUtility.FromJson<ToKenSwapResult>(result);
                LoginWindowData.Instance.SaveToKenSwapResult(toKenSwapResult);
                //保存登录手机号
                PlayerPrefs.SetString("IphoneNum", s_PhoneNumber);
                GetUserDataInfo(toKenSwapResult.data.user_id, toKenSwapResult.data.access_token, toKenSwapResult.data.mac_key);
                //获取实验记录
                GetShiYanJiLuIphone(toKenSwapResult);
            }
        }

        //账号登录按钮
        private void AccountLoginOnClick()
        {
            AccoutLoginBtn.interactable = false;
            AccoutLoginImgObj.SetActive(true);
            AccoutLoginBtn.gameObject.transform.GetChild(0).gameObject.SetActive(false);
            //判断是否有网络
            if (CheckNetWork())
            {
                return;
            }
            else
            {
                s_UserName = AccountInputField.TextStr;
                s_Password = PasswordInputField.TextStr;

                //s_UserName = "qazy10";
                //s_Password = "Vlab654321";
                if (String.IsNullOrEmpty(s_UserName) || String.IsNullOrEmpty(s_Password))
                {
                    AccoutLoginBtn.interactable = true;
                    AccoutLoginImgObj.SetActive(false);
                    AccoutLoginBtn.gameObject.transform.GetChild(0).gameObject.SetActive(true);
                    AccoutLoginBtn.gameObject.transform.GetChild(1).gameObject.SetActive(false);
                    return;
                }
                client.EndPoint = CommonConstant.SERVER_URL_MEC_PROXY_UC;
                client.Method = EnumHttpVerb.POST;
                AccountLoginRequest alr = new AccountLoginRequest();
                alr.login_name = CommonUtil.EncryptDes(s_UserName, LoginWindowData.Instance.ReadSessionResult().session_key);
                alr.login_name_type = "101_login_name";
                alr.org_code = "";
                alr.password = CommonUtil.EncryptDes(CommonUtil.EncryptMD5_Salt(s_Password), LoginWindowData.Instance.ReadSessionResult().session_key);
                alr.session_id = LoginWindowData.Instance.ReadSessionResult().session_id;
                if (isNeedIDC)
                {
                    s_IDCode = VerifyCodeInputField.TextStr;
                    //需要验证码的时候
                    alr.identify_code = s_IDCode;
                }
                else
                {
                    alr.identify_code = "";
                }
                client.PostData = JsonUtility.ToJson(alr);
                //发送并获取返回数据
                string result = client.HttpRequest(CommonConstant.SERVER_URL_ACCOUNT_LOGIN);
                Debug.Log("登录信息：" + result);
                //如果登录失败
                if (String.IsNullOrEmpty(result))
                {
                    GetVerifyCodeOnClick();
                    AccoutLoginBtn.interactable = true;
                    AccoutLoginImgObj.SetActive(false);
                    AccoutLoginBtn.gameObject.transform.GetChild(0).gameObject.SetActive(true);
                    AccoutLoginBtn.gameObject.transform.GetChild(1).gameObject.SetActive(false);
                    ShowError(client.ErrorMessage);
                    return;
                }
                //如果登录成功
                else
                {
                    //ifNeedIDC = false;
                    Debug.Log("登录成功");
                    AccountLoginResult accountLoginResult = new AccountLoginResult();
                    accountLoginResult = JsonUtility.FromJson<AccountLoginResult>(result);
                    //保存返回数据
                    LoginWindowData.Instance.SaveAccountLoginResult(accountLoginResult);
                    //跳转页面
                    GetUserDataInfo(accountLoginResult.user_id, accountLoginResult.access_token, accountLoginResult.mac_key);

                    //获取实验记录
                    GetShiYanJiLu(accountLoginResult);
                    LoginWindowData.Instance.IsLogin = true;
                    //保存账号密码
                    PlayerPrefs.SetString("account", s_UserName);
                    PlayerPrefs.SetString("password", s_Password);

                    WindowManager.Close("LoginWindow");
                    WindowManager.Open<MainWindow>();
                }
            }
        }
        public Button enterBtn;
        //选择手机登录Toggle
        private void IphoneLoginToggOnClick(bool arg0)
        {
            if (arg0)
            {
                AocountOrIphoneLogin = "Iphone";
                AccountLoginPanel.SetActive(false);
                IphoneLoginPanel.SetActive(true);
                AccountErrorTip.SetActive(false);
                PasswordErrorTip.SetActive(false);
                VerifyCodeErrorTip.SetActive(false);
                IphoneVerifyCodeGridErrorTip.SetActive(false);
                NetErrorTxt.SetActive(false);
                IponeCodeErrorTxt.SetActive(false);
                MoreErrorTipTxt.SetActive(false);
                VerifyCodeGridObj.SetActive(false);
                IphoneVerifyCodeGridObj.SetActive(false);

                VerifyCodeInputField.enterBtn = IphoneLoginBtn;
                AccountInputField.TextStr = "";
                PasswordInputField.TextStr = "";
                VerifyCodeInputField.TextStr = "";
                IphoneTogg.gameObject.transform.GetChild(1).gameObject.SetActive(false);
                AccountTogg.gameObject.transform.GetChild(2).GetComponent<Text>().color = new Color32(49, 255, 255, 255);
                IphoneTogg.gameObject.transform.GetChild(2).GetComponent<Text>().color = new Color32(255, 252, 206, 255);
            }
        }

        //选择账号登录Toggle
        private void AccountLoginToggOnClick(bool arg0)
        {
            if (arg0)
            {
                AocountOrIphoneLogin = "Account";
                VerifyCodeGridObj.SetActive(false);
                IphoneVerifyCodeGridObj.SetActive(false);
                VerifyCodeErrorTip.SetActive(false);
                IphoneVerifyCodeGridErrorTip.SetActive(false);
                AccountLoginPanel.SetActive(true);
                IphoneLoginPanel.SetActive(false);
                NetErrorTxt.SetActive(false);
                IponeCodeErrorTxt.SetActive(false);
                MoreErrorTipTxt.SetActive(false);
                IphoneCodeInputField.TextStr = "";
                IphoneNumInputField.TextStr = "";
                GetIphoneCodeBtn.interactable = false;
                VerifyCodeInputField.enterBtn = AccoutLoginBtn;
                AccountTogg.gameObject.transform.GetChild(1).gameObject.SetActive(false);
                AccountTogg.gameObject.transform.GetChild(2).GetComponent<Text>().color = new Color32(255, 252, 206, 255);
                IphoneTogg.gameObject.transform.GetChild(2).GetComponent<Text>().color = new Color32(49, 255, 255, 255);
                GetIphoneCodeTipTxt.text = "发送验证码";
                s_Timer = 60;
                CancelInvoke();
            }
        }

        //返回
        private void ReturnOnClick()
        {
            WindowManager.ReShow();
            WindowManager.Close("LoginWindow");
        }

        //默认设置
        private void DefulatSetting()
        {
            //读取上次保存的账号密码
            if (PlayerPrefs.HasKey("account")&& PlayerPrefs.HasKey("password"))
            {
                AccountInputField.TextStr = PlayerPrefs.GetString("account");
                PasswordInputField.TextStr = PlayerPrefs.GetString("password");
            }
            if (PlayerPrefs.HasKey("IphoneNum"))
            {
                IphoneNumInputField.TextStr = PlayerPrefs.GetString("IphoneNum");
            }
            AocountOrIphoneLogin = "Account";
            IphoneLoginImgObj.SetActive(false);
            AccoutLoginImgObj.SetActive(false);
            IphoneLoginBtn.interactable = true;
            AccoutLoginBtn.interactable = true;
            AccountLoginPanel.SetActive(true);
            IphoneLoginPanel.SetActive(false);
            AccountErrorTip.SetActive(false);
            PasswordErrorTip.SetActive(false);
            VerifyCodeErrorTip.SetActive(false);
            IphoneVerifyCodeGridErrorTip.SetActive(false);
            NetErrorTxt.SetActive(false);
            IponeCodeErrorTxt.SetActive(false);
            IphoneNumErrorMsgText.SetActive(false);
            MoreErrorTipTxt.SetActive(false);
            VerifyCodeGridObj.SetActive(false);
            IphoneVerifyCodeGridObj.SetActive(false);
            GetIphoneCodeBtn.interactable = false;
            GetIphoneCodeTipTxt.text = "发送验证码";
            GetIphoneCodeTipTxt.color = new Color32(79, 75, 75, 255);
            AccountTogg.gameObject.transform.GetChild(2).GetComponent<Text>().color = new Color32(255, 252, 206, 255);
            s_Timer = 60;
        }

        //检测网络
        private bool CheckNetWork()
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                NetErrorTxt.SetActive(true);
                AccoutLoginBtn.interactable = true;
                AccoutLoginImgObj.SetActive(false);
                AccoutLoginBtn.gameObject.transform.GetChild(0).gameObject.SetActive(true);
                return true;
            }
            else
            {
                NetErrorTxt.SetActive(false);
                return false;
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
                ShowError(client.ErrorMessage);
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

        //同过BestHttp插件请求的方式
        private void GetUserDataInfoBestHttp(string userId, string token, string mac_key) 
        {
            string url = CommonConstant.SERVER_URL_MEC_PROXY + CommonConstant.GET_USERINFO;
            Uri uri = new Uri(url);
            HTTPRequest request = new HTTPRequest(uri, HTTPMethods.Post, OnFinsish);
            request.SetHeader("Content-Type", "application/json; charset=UTF-8");
            UserInfoRequest userInfo = new UserInfoRequest();
            MacInfoRequest macInfo = new MacInfoRequest();
            macInfo.session_key = LoginWindowData.Instance.ReadSessionResult().session_key;
            macInfo.mac_key = mac_key;
            macInfo.token = token;
            userInfo.macInfo = macInfo;
            userInfo.userId = userId;
            string result = JsonUtility.ToJson(userInfo);
            request.RawData= Encoding.UTF8.GetBytes(result);
            request.Send();
        }

        private void OnFinsish(HTTPRequest originalRequest, HTTPResponse response)
        {
            if (originalRequest.State == HTTPRequestStates.Finished)
            {
                Debug.Log("responseText: " + response.DataAsText);
            }
        }

        public override void OnShow(params object[] para)
        {
            base.OnShow(para);
        }

        public override void OnClose()
        {
            base.OnClose();
        }

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


        //获取手机账号登录实验记录
        private void GetShiYanJiLuIphone(ToKenSwapResult toKenSwapResult)
        {
            client.EndPoint = CommonConstant.SERVER_URL_MEC_PROXY;
            client.Method = EnumHttpVerb.POST;
            ShiYanCeShiJiLuRequest shiYanCeShiJiLu = new ShiYanCeShiJiLuRequest();
            MacInfoRequest macInfo = new MacInfoRequest();
            shiYanCeShiJiLu.page = 0;
            shiYanCeShiJiLu.size = 10;
            macInfo.mac_key = toKenSwapResult.data.mac_key;
            macInfo.session_key = LoginWindowData.Instance.ReadSessionResult().session_key;
            macInfo.token = toKenSwapResult.data.access_token;
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
    }
}

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
    class TextbookSelectWindow: BaseWindow
    {
        #region 字段定义
        private GameObject s_TextbookUI;
        private GameObject s_EmptyTextbookUI;
        private GameObject s_LoadingTextbookUI;
        private GameObject s_LoadSubjectArea;
        private GameObject s_LoadTextbookArea;
        private GameObject s_LeftSlideBtnObj;
        private GameObject s_RightSlideBtnObj;


        private Image s_LoadingIcon;
        private Text s_LoadingText;
        private Text s_GradeText;

        private Button s_RetryBtn;
        private Button s_EditGradeBtn;
        private Button s_TextbookMgrBtn;
        private Button s_JumpLoginBtn;
        private Button s_ReturnBtn;



        private ToggleGroup s_SubjectTG;

        private Sprite s_LoadingFailImage;
        private Color32 s_LoadingFailColor = new Color32(255,44,43,255);
        private string s_LoadingFailString = "网络加载失败，点击重试";
        private string s_CurrentSelectSubject = "";
        //储存学科信息
        private List<GetSubjectItemResult> subjects = new List<GetSubjectItemResult>();
        //用于控制教材显示
        private List<GameObject> textbooks = new List<GameObject>();
        //用于显示学科内的教材数量
        private Dictionary<string, int> subjectSelected = new Dictionary<string, int>();
        //用于暂存读取到的教材信息
        public List<EditUserTextbookBodyRequest> userChooseItemResults = new List<EditUserTextbookBodyRequest>();
        //用于控制学科显示
        private List<GameObject> subjectsObjs = new List<GameObject>();

        RestClient client = new RestClient();

        private bool isLoading = true;
        #endregion
        #region 窗口生命周期
        public override void OnInit()
        {
            prefabType = Window.TextbookSelectWindow;
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
            //开启面板默认显示
            s_TextbookUI.SetActive(false);
            s_EmptyTextbookUI.SetActive(false);
            s_GradeText.text ="学段年级：" +BaseInfoWindowData.Instance.SectionName + "·" + BaseInfoWindowData.Instance.GradeName;
            //获取当前已选择教材-----------------------------------------------------------------------------------------
            GetSelectedTextbook();
            //获取当前学科
            GetCurrentSubject();
            //展示教材/没有教材-----------------------------------------------------------------------------------------
            ShowTextbookUI();
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
            s_TextbookUI = GameObject.Find("TextbookUI");
            s_LoadingTextbookUI = GameObject.Find("LoadingTextbookUI");
            s_EmptyTextbookUI = GameObject.Find("EmptyTextbookUI");

            s_LoadingIcon = GameObject.Find("LoadingIcon").GetComponent<Image>();
            s_LoadingText = GameObject.Find("LoadingText").GetComponent<Text>();

            s_LoadSubjectArea = GameObject.Find("SubjectSelectContent");
            s_LoadTextbookArea = GameObject.Find("LoadTextbookArea");


            s_LeftSlideBtnObj = GameObject.Find("LeftSlideBtn");
            s_RightSlideBtnObj = GameObject.Find("RightSlideBtn");

            s_SubjectTG = s_LoadSubjectArea.GetComponent<ToggleGroup>();

            //绑定左右滑动的物体
            s_LeftSlideBtnObj.GetComponent<ButtonLongPress>().parent = s_LoadTextbookArea;
            s_RightSlideBtnObj.GetComponent<ButtonLongPress>().parent = s_LoadTextbookArea;
        }
        private void InitButton()
        {
            s_RetryBtn = GameObject.Find("RetryBtn").GetComponent<Button>();
            s_RetryBtn.onClick.AddListener(RetryBtnOnClick);
            s_EditGradeBtn = GameObject.Find("EditGradeBtn").GetComponent<Button>();
            s_EditGradeBtn.onClick.AddListener(EditGradeBtnOnClick);
            s_TextbookMgrBtn = GameObject.Find("TextbookMgrBtn").GetComponent<Button>();
            s_TextbookMgrBtn.onClick.AddListener(TextbookMgrBtnOnClick);
            s_JumpLoginBtn = GameObject.Find("JumpLoginBtn").GetComponent<Button>();
            s_JumpLoginBtn.onClick.AddListener(JumpLoginBtnOnClick);
            s_ReturnBtn = GameObject.Find("TextbookSelectReturnBtn").GetComponent<Button>();
            s_ReturnBtn.onClick.AddListener(ReturnBtnOnClick);

            s_GradeText = s_EditGradeBtn.GetComponentInChildren<Text>();
        }
        #endregion
        #region 逻辑方法
        /// <summary>
        /// 加载失败
        /// </summary>
        private void LoadingFail()
        {
            //显示效果变化
            isLoading = false;
            s_LoadingIcon.sprite = Resources.Load<Sprite>("WindowUI/TextbookWindowUI/组 48") as Sprite;
            s_LoadingText.text = s_LoadingFailString;
            s_LoadingText.color = s_LoadingFailColor;
            //重试按钮激活。-----------------------------------------------------------------------------------------
        }
        //从服务器读取已选择信息
        private void GetSelectedTextbook()
        {
            if (LoginWindowData.Instance.IsLogin)
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
                userChooseList.subject = s_CurrentSelectSubject;
                userChooseList.edition = "";

                client.PostData = JsonUtility.ToJson(userChooseList);
                string result = client.HttpRequest(CommonConstant.GET_USER_CHOOSE_LIST);
                if (string.IsNullOrEmpty(result))
                {
                    Debug.Log(client.ErrorMessage);
                    return;
                }
                else
                {
                    UserChooseInfoResult chooseInfoResult = new UserChooseInfoResult();
                    chooseInfoResult = JsonUtility.FromJson<UserChooseInfoResult>(result);
                    userChooseItemResults.Clear();
                    for (int i = 0; i < chooseInfoResult.data.items.Count; i++)
                    {
                        //if ((chooseInfoResult.data.items[i].teaching_material_grade == BaseInfoWindowData.Instance.GradeID)|| string.IsNullOrEmpty(BaseInfoWindowData.Instance.GradeID))
                        //{
                            EditUserTextbookBodyRequest editUserTextbookBodyRequest = new EditUserTextbookBodyRequest();
                            editUserTextbookBodyRequest.source = "VR";
                            editUserTextbookBodyRequest.teaching_material_edition = "";
                            editUserTextbookBodyRequest.teaching_material_grade = chooseInfoResult.data.items[i].teaching_material_grade;
                            editUserTextbookBodyRequest.teaching_material_icon = chooseInfoResult.data.items[i].teaching_material_icon;
                            editUserTextbookBodyRequest.teaching_material_id = chooseInfoResult.data.items[i].teaching_material_id;
                            editUserTextbookBodyRequest.teaching_material_name = chooseInfoResult.data.items[i].teaching_material_name;
                            editUserTextbookBodyRequest.teaching_material_subject = chooseInfoResult.data.items[i].teaching_material_subject;
                            userChooseItemResults.Add(editUserTextbookBodyRequest);
                            if (subjectSelected.ContainsKey(chooseInfoResult.data.items[i].teaching_material_subject))
                            {
                                subjectSelected[chooseInfoResult.data.items[i].teaching_material_subject]++;
                            }
                            else
                            {
                                subjectSelected.Add(chooseInfoResult.data.items[i].teaching_material_subject, 1);
                            }
                        //}
                    }
                }
            }
            else
            {
                //从本地读取
                userChooseItemResults.Clear();
                for (int i = 0; i < TextbookMgrWindowData.Instance.userChooseItemResults.Count; i++)
                {
                    //if ((TextbookMgrWindowData.Instance.userChooseItemResults[i].teaching_material_grade == BaseInfoWindowData.Instance.GradeID) || string.IsNullOrEmpty(BaseInfoWindowData.Instance.GradeID))
                    //{
                        userChooseItemResults.Add(TextbookMgrWindowData.Instance.userChooseItemResults[i]);
                        if (subjectSelected.ContainsKey(TextbookMgrWindowData.Instance.userChooseItemResults[i].teaching_material_subject))
                        {
                            subjectSelected[TextbookMgrWindowData.Instance.userChooseItemResults[i].teaching_material_subject]++;
                        }
                        else
                        {
                            subjectSelected.Add(TextbookMgrWindowData.Instance.userChooseItemResults[i].teaching_material_subject, 1);
                        }
                    //}
                }
            }
        }
        //从服务器读取当前学科
        private void GetCurrentSubject()
        {
            client.EndPoint = CommonConstant.SERVER_URL_MEC_PROXY;
            client.Method = EnumHttpVerb.GET;
            string result2 = client.HttpRequest(CommonConstant.GET_SUBJECT_LIST);
            GetSubjectListResult gslr = JsonUtility.FromJson<GetSubjectListResult>(result2);
            foreach (GetSubjectItemResult item in gslr.data)
            {
                subjects.Add(item);
            }
        }
        private void ShowTextbookUI()
        {
            if (userChooseItemResults.Count !=0)
            {
                s_TextbookUI.SetActive(true);
                s_LoadingTextbookUI.SetActive(false);
                s_EmptyTextbookUI.SetActive(false);
                //生成“全部”toggle
                GameObject goLoadSub = ResManager.Instance.Load<GameObject>(WindowGrid.SubjectGrid);
                GameObject goTotal = Instantiate(goLoadSub, s_LoadSubjectArea.transform);
                subjectsObjs.Add(goTotal);
                goTotal.GetComponentInChildren<Text>().text = "全部" + "(" + userChooseItemResults.Count.ToString() + ")";
                Toggle toggleTotal = goTotal.GetComponent<Toggle>();
                toggleTotal.onValueChanged.AddListener((bool value) =>
                {
                    if (value)
                    {
                        
                        s_CurrentSelectSubject = "";
                        GetSelectedTextbook();
                        ShowTextbookGrid();
                    }
                    else
                    {

                    }
                }
                );
                toggleTotal.group = s_SubjectTG;
                for (int i = 0; i < subjects.Count; i++)
                {
                    GameObject go = Instantiate(goLoadSub, s_LoadSubjectArea.transform);
                    subjectsObjs.Add(go);
                    if (subjectSelected.ContainsKey(subjects[i].key))
                    {
                        go.GetComponentInChildren<Text>().text = subjects[i].value + "(" + subjectSelected[subjects[i].key] + ")";
                    }
                    else
                    {
                        go.GetComponentInChildren<Text>().text = subjects[i].value;
                    }
                    Toggle toggle = go.GetComponent<Toggle>();
                    toggle.group = s_SubjectTG;
                    int num = i;
                    toggle.onValueChanged.AddListener((bool value) =>
                    {
                        if (value)
                        {
                            s_CurrentSelectSubject = subjects[num].key;
                            GetSelectedTextbook();
                            ShowTextbookGrid();
                        }
                        else
                        {

                        }
                    }
                        );

                }
            }
            else
            {
                s_TextbookUI.SetActive(false);
                s_LoadingTextbookUI.SetActive(false);
                s_EmptyTextbookUI.SetActive(true);
            }
        }
        private void ShowTextbookGrid()
        {
            ClearTextbook();
            GameObject goLoad = ResManager.Instance.Load<GameObject>(WindowGrid.TextbookGrid);
            //生成textbook
            for (int i = 0; i < userChooseItemResults.Count; i++)
            {
                if (s_CurrentSelectSubject == "")
                {
                    GameObject go = Instantiate(goLoad, s_LoadTextbookArea.transform);
                    Toggle toggle = go.GetComponent<Toggle>();
                    int num = i;
                    //点击跳转实验选择界面
                    toggle.onValueChanged.AddListener((bool value) =>
                    {
                        ShiYanXuanZeWindowData.Instance.GetRecentTextbooksID = userChooseItemResults[num].teaching_material_id;
                        WindowManager.Hide("TextbookSelectWindow");
                        WindowManager.Open<ShiYanXuanZeWindow>();
                    }
                    );
                    textbooks.Add(go);
                    if (i != 0)
                    {
                        go.transform.GetChild(1).gameObject.SetActive(false);
                    }
                    //读图
                    StartCoroutine(AddImage(userChooseItemResults[i].teaching_material_icon, go.transform.GetChild(0).GetComponent<Image>()));
                    go.transform.GetChild(2).gameObject.GetComponentInChildren<Text>().text = userChooseItemResults[i].teaching_material_name;
                    continue;
                }
                //生成资源
                if (userChooseItemResults[i].teaching_material_subject == s_CurrentSelectSubject)
                {
                    GameObject go = Instantiate(goLoad, s_LoadTextbookArea.transform);
                    Toggle toggle = go.GetComponent<Toggle>();
                    int num = i;
                    //点击跳转实验选择界面
                    toggle.onValueChanged.AddListener((bool value) =>
                    {
                        ShiYanXuanZeWindowData.Instance.GetRecentTextbooksID = userChooseItemResults[num].teaching_material_id;
                        WindowManager.Hide("TextbookSelectWindow");
                        WindowManager.Open<ShiYanXuanZeWindow>();
                    }
                    );
                    textbooks.Add(go);
                    if (i != 0)
                    {
                        go.transform.GetChild(1).gameObject.SetActive(false);
                    }
                    //读图
                    StartCoroutine(AddImage(userChooseItemResults[i].teaching_material_icon, go.transform.GetChild(0).GetComponent<Image>()));
                    go.transform.GetChild(2).gameObject.GetComponentInChildren<Text>().text = userChooseItemResults[i].teaching_material_name;
                }
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
                if (image!=null)
                {
                    image.sprite = sprite;
                }
            }
        }
        private void ClearTextbook()
        {
            for (int i = 0; i < textbooks.Count; i++)
            {
                DestroyImmediate(textbooks[i]);
            }
            textbooks.Clear();
        }
        
        #endregion
        #region 按钮点击事件
        private void RetryBtnOnClick()
        {

        }
        private void EditGradeBtnOnClick()
        {
            WindowManager.Hide("TextbookSelectWindow");
            BaseInfoWindowData.Instance.FromWindow = "TextbookSelectWindow";
            WindowManager.Open<BaseInfoWindow>();
        }
        private void TextbookMgrBtnOnClick()
        {
            TextbookMgrWindowData.Instance.FormWindow = "TextbookSelectWindow";
            WindowManager.Hide("TextbookSelectWindow");
            WindowManager.Open<TextbookMgrWindow>();
        }
        private void JumpLoginBtnOnClick()
        {
            WindowManager.Hide("TextbookSelectWindow");
            WindowManager.Open<LoginWindow>();
        }
        private void ReturnBtnOnClick()
        {
            WindowManager.Close("MainWindow");
            WindowManager.Open<MainWindow>();
            WindowManager.Close("TextbookSelectWindow");
        }

        #endregion
        private void Update()
        {
            if (isLoading)
            {
                s_LoadingIcon.transform.Rotate(Vector3.forward, 3);
            }
        }
    }
}

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
using Assets.Scripts.Tool;
namespace Assets.Scripts.Windows
{
    class TextbookMgrWindow :BaseWindow
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
        private Button s_ConfirmBtn;
        private Button s_JumpLoginBtn;
        private Button s_ReturnBtn;
        private Button s_LoadMoreBtn;

        private ScrollRect s_TextbookContent;

        private ToggleGroup s_SubjectTG;

        private Dropdown s_TextbookEditionDropDown;

        private Sprite s_LoadingFailImage;

        private string currentSubject = "";

        private Color32 s_LoadingFailColor = new Color32(255, 44, 43, 255);
        private string s_LoadingFailString = "网络加载失败，点击重试";

        private int s_CurrentPage = 0;

        //用于控制学科显示
        private List<GameObject> subjectsObjs = new List<GameObject>();
        //用于控制教材显示
        private List<GameObject> textbooks = new List<GameObject>();
        //用于暂存本地所有的选择教材
        private List<EditUserTextbookBodyRequest> saveDatas = new List<EditUserTextbookBodyRequest>();

        bool isFirstLoad = true;

        //索引键值对；
        private Dictionary<string, string> getKeyDic = new Dictionary<string, string>();

        RestClient client = new RestClient();

        private bool isLoading = true;
        #endregion
        #region 窗口生命周期
        public override void OnInit()
        {
            prefabType = Window.TextbookMgrWindow;
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
            s_LoadingTextbookUI.SetActive(true);
            s_GradeText.text = "学段年级：" + BaseInfoWindowData.Instance.SectionName + "·" + BaseInfoWindowData.Instance.GradeName;
            //获取所有出版社信息
            GetAllPublisherData();
            //添加到下拉框里
            AddPressToDropDown();
            s_CurrentPressID = getKeyDic[s_TextbookEditionDropDown.options[0].text];
            //获取当前已选择教材
            GetSelectedTextbook();
            //获取当前学科
            GetCurrentSubject();
            //显示所有学科
            ShowAllSubject();

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
            s_TextbookUI = GameObject.Find("TextbookMgrUI");
            s_LoadingTextbookUI = GameObject.Find("LoadingTextbookMgrUI");
            s_EmptyTextbookUI = GameObject.Find("EmptyTextbookMgrUI");

            s_LoadingIcon = GameObject.Find("LoadingMgrIcon").GetComponent<Image>();
            s_LoadingText = GameObject.Find("LoadingMgrText").GetComponent<Text>();

            s_LoadSubjectArea = GameObject.Find("SubjectSelectMgrContent");
            s_LoadTextbookArea = GameObject.Find("LoadTextbookMgrArea");

            s_TextbookEditionDropDown = GameObject.Find("TextbookEditionDropDown").GetComponent<Dropdown>();

            s_SubjectTG = s_LoadSubjectArea.GetComponent<ToggleGroup>();

            s_LeftSlideBtnObj = GameObject.Find("LeftSlideMgrBtn");
            s_RightSlideBtnObj = GameObject.Find("RightSlideMgrBtn");

            s_TextbookContent = GameObject.Find("TextbookMgrContent").GetComponent<ScrollRect>();
            //下拉框绑定事件

            s_TextbookEditionDropDown.onValueChanged.AddListener(DropDownValueChange);

            //绑定左右滑动的物体
            s_LeftSlideBtnObj.GetComponent<ButtonLongPress>().parent = s_LoadTextbookArea;
            s_RightSlideBtnObj.GetComponent<ButtonLongPress>().parent = s_LoadTextbookArea;

            s_TextbookContent.onValueChanged.AddListener(DragContent);
        }
        private void DragContent(Vector2 vector2)
        {
            if (s_TextbookContent.horizontal)
            {
                if (/*Mathf.Abs( */s_TextbookContent.horizontalNormalizedPosition/*)*/> 0.99f)
                {
                    if (currentOffset < totalJiaoCais.Count)
                    {
                        s_LoadMoreBtn.gameObject.SetActive(true);
                        s_RightSlideBtnObj.SetActive(false);
                    }
                }
                else
                {
                    s_LoadMoreBtn.gameObject.SetActive(false);
                    s_RightSlideBtnObj.SetActive(true);
                }
            }
        }
   
        private void InitButton()
        {
            s_RetryBtn = GameObject.Find("RetryMgrBtn").GetComponent<Button>();
            s_RetryBtn.onClick.AddListener(RetryBtnOnClick);
            s_EditGradeBtn = GameObject.Find("EditGradeMgrBtn").GetComponent<Button>();
            s_EditGradeBtn.onClick.AddListener(EditGradeBtnOnClick);
            s_ConfirmBtn = GameObject.Find("TextbookConfirmMgrBtn").GetComponent<Button>();
            s_ConfirmBtn.onClick.AddListener(ConfirmBtnOnClick);
            s_JumpLoginBtn = GameObject.Find("JumpLoginMgrBtn").GetComponent<Button>();
            s_JumpLoginBtn.onClick.AddListener(JumpLoginBtnOnClick);
            s_ReturnBtn = GameObject.Find("TextbookMgrReturnBtn").GetComponent<Button>();
            s_ReturnBtn.onClick.AddListener(ReturnBtnOnClick);
            s_LoadMoreBtn = GameObject.Find("LoadMoreBtn").GetComponent<Button>();
            s_LoadMoreBtn.onClick.AddListener(LoadMoreBtnOnClick);




            s_GradeText = s_EditGradeBtn.GetComponentInChildren<Text>();
        }
        #endregion
        #region 逻辑方法
        //用于暂存所有的出版社-----------------------------------------------------------------------------------------------------
        private List<PressItemResult> pressItemResults = new List<PressItemResult>();
        private string s_CurrentPressID;
        //获取所有出版社信息
        private void GetAllPublisherData()
        {
            client.EndPoint = CommonConstant.SERVER_URL_MEC_PROXY;
            client.Method = EnumHttpVerb.POST;
            GetPressRequest jcr = new GetPressRequest();
            jcr.subjectTag = "";
            jcr.gradeTag = "";
            client.PostData = JsonUtility.ToJson(jcr);
            string result = client.HttpRequest(CommonConstant.GET_PRESS_LIST);
            PressListResult plr = new PressListResult();
            plr = JsonUtility.FromJson<PressListResult>(result);
            for (int i = 0; i < plr.data.Count; i++)
            {
                pressItemResults.Add(plr.data[i]);
            }
        }
        //添加出版社到下拉框
        private void AddPressToDropDown()
        {
            Dropdown.OptionData totalData = new Dropdown.OptionData();
            totalData.text = "全部已选择教材                              ";
            getKeyDic.Add("全部已选择教材                              ", "没有");
            s_TextbookEditionDropDown.options.Add(totalData);
            for (int i = 0; i < pressItemResults.Count; i++)
            {
                Dropdown.OptionData data = new Dropdown.OptionData();
                data.text = pressItemResults[i].name;
                s_TextbookEditionDropDown.options.Add(data);
                getKeyDic.Add(pressItemResults[i].name, pressItemResults[i].pressId);
            }
        }
        //下拉框点击事件
        private void DropDownValueChange(int num)
        {
            if (num == 0)
            {
                s_CurrentPressID = "没有";
                CreateAllSelectedTextbook();
                return;
            }
            s_CurrentPressID = getKeyDic[s_TextbookEditionDropDown.options[num].text];
            //获取所有当前年级，当前出版社下的所有教材信息
            GetAllTextbookData();
            //生成所有教材
            ShowAllTextbook();
            //生成所有学科旁边的数字
            ShowSubjectNum();
            //复选已有教材
            ReShowSelectedTextbook();
        }
        //储存学科信息--------------------------------------------------------------------------------------------------------------
        private List<GetSubjectItemResult> subjects = new List<GetSubjectItemResult>();
        //定位学科的显示
        private Dictionary<string, Text> subjectTextDic = new Dictionary<string, Text>();
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
                getKeyDic.Add(item.value, item.key);
                getKeyDic.Add(item.key, item.value);
            }
        }
        //显示所有学科Toggle
        private void ShowAllSubject()
        {
            //生成“全部”toggle
            GameObject goLoadSub = ResManager.Instance.Load<GameObject>(WindowGrid.SubjectGrid);
            GameObject goTotal = Instantiate(goLoadSub, s_LoadSubjectArea.transform);
            subjectsObjs.Add(goTotal);
            Text totalText = goTotal.GetComponentInChildren<Text>();
            totalText.text = "全部";
            subjectTextDic.Add("全部", totalText);
            Toggle toggleTotal = goTotal.GetComponent<Toggle>();
            toggleTotal.onValueChanged.AddListener((bool value) =>
            {
                if (value)
                {
                    currentSubject = "";
                    if (s_CurrentPressID == "没有")
                    {
                        CreateAllSelectedTextbook();
                    }
                    else
                    {
                        //获取所有当前年级，当前出版社下的所有教材信息
                        GetAllTextbookData();
                        //生成所有教材
                        ShowAllTextbook();
                        //生成所有学科旁边的数字
                        ShowSubjectNum();
                        //复选已有教材
                        ReShowSelectedTextbook();
                    }
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
                Text text = go.GetComponentInChildren<Text>();
                text.text = subjects[i].value;
                subjectTextDic.Add(subjects[i].value,text);
                Toggle toggle = go.GetComponent<Toggle>();
                toggle.group = s_SubjectTG;
                int num = i;
                toggle.onValueChanged.AddListener((bool value) =>
                {
                    if (value)
                    {
                        currentSubject = subjects[num].value;
                        if (s_CurrentPressID == "没有")
                        {
                            CreateAllSelectedTextbook();
                        }
                        else
                        {
                            //获取所有当前年级，当前出版社下的所有教材信息
                            GetAllTextbookData();
                            //生成所有教材
                            ShowAllTextbook();
                            //生成所有学科旁边的数字
                            ShowSubjectNum();
                            //复选已有教材
                            ReShowSelectedTextbook();
                        }
                    }
                    else
                    {

                    }
                }
                    );

            }
        }
        //生成全部已选择教材=======================================================================================================
        private void CreateAllSelectedTextbook()
        {
            ClearTextbook();
            
            if (saveDatas.Count != 0)
            {
                s_TextbookUI.SetActive(true);
                s_EmptyTextbookUI.SetActive(false);
                s_LoadingTextbookUI.SetActive(false);
            }
            else
            {
                s_TextbookUI.SetActive(false);
                s_EmptyTextbookUI.SetActive(true);
                s_LoadingTextbookUI.SetActive(false);
            }
            string compareSrc;
            if (string.IsNullOrEmpty(currentSubject))
            {
                compareSrc = "";
            }
            else
            {
                compareSrc = getKeyDic[currentSubject];
            }
            for (int i = 0; i < saveDatas.Count; i++)
            {
                if ((saveDatas[i].teaching_material_subject != compareSrc) && currentSubject!= "")
                {
                    continue;
                }
                GameObject goLoad = ResManager.Instance.Load<GameObject>(WindowGrid.TextbookMgrGrid);
                GameObject go = Instantiate(goLoad, s_LoadTextbookArea.transform);
                //接入自适应
                //go.GetComponentInChildren<Text>().text = saveDatas[i].teaching_material_name;
                



                Toggle toggle = go.transform.GetComponent<Toggle>();
                go.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
                EditUserTextbookBodyRequest editUserTextbookBodyRequest = saveDatas[i];
                int num = i;
                go.GetComponentInChildren<GridTextFitter>().ShowText = saveDatas[i].teaching_material_name;
                toggle.onValueChanged.AddListener((bool value) =>
                {
                    if (value)
                    {
                        Debug.Log("调用了Toggle");
                        saveDatas.Add(editUserTextbookBodyRequest);
                        go.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
                        ShowSubjectNum();
                    }
                    else
                    {
                        for (int j = 0; j < saveDatas.Count; j++)
                        {
                            if (saveDatas[j].teaching_material_id == editUserTextbookBodyRequest.teaching_material_id)
                            {
                                saveDatas.RemoveAt(j);
                            }
                        }
                        //saveDatas.RemoveAt(num);
                        go.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
                        ShowSubjectNum();
                    }
                });
                StartCoroutine(AddImage(saveDatas[i].teaching_material_icon, go.transform.GetChild(0).GetComponent<Image>()));
                textbooks.Add(go);
            }
            ShowSubjectNum();
            s_LoadMoreBtn.gameObject.SetActive(false);
        }
        //用于暂存所有的筛选后可选的教材--------------------------------------------------------------------------------------------
        private List<JiaoCaiDataInfoResult> totalJiaoCais = new List<JiaoCaiDataInfoResult>();

        //获取所有筛选过后的教材对象
        private void GetAllTextbookData()
        {
            ClearTextbook();
            
            client.EndPoint = CommonConstant.SERVER_URL_MEC_PROXY;
            client.Method = EnumHttpVerb.POST;
            JiaoCaiRequest jcr = new JiaoCaiRequest();
            if (!string.IsNullOrEmpty(currentSubject))
            {
                jcr.subjectTag = getKeyDic[currentSubject];
            }
            else
            {
                jcr.subjectTag = "";
            }
            jcr.gradeTag = "";/*BaseInfoWindowData.Instance.GradeID;*/
            jcr.pressId = s_CurrentPressID;
            client.PostData = JsonUtility.ToJson(jcr);
            string result = client.HttpRequest(CommonConstant.GET_JIAOCAI);
            TextbookSelectTotalResult tstr = new TextbookSelectTotalResult();
            tstr = JsonUtility.FromJson<TextbookSelectTotalResult>(result);
            for (int i = 0; i < tstr.data.Count; i++)
            {
                totalJiaoCais.Add(tstr.data[i]);
            }
            if (totalJiaoCais.Count!=0)
            {
                s_TextbookUI.SetActive(true);
                s_EmptyTextbookUI.SetActive(false);
                s_LoadingTextbookUI.SetActive(false);
            }
            else
            {
                s_TextbookUI.SetActive(false);
                s_EmptyTextbookUI.SetActive(true);
                s_LoadingTextbookUI.SetActive(false);
            }
            currentOffset = 0;
            currentPage = 0;
    }
        //点击一次加载数量
        private int loadLimit =6 ;
        private int currentOffset = 0;
        private int currentPage = 0;
        //生成所有筛选过后的对象。
        private void ShowAllTextbook()
        {
            s_TextbookContent.horizontalNormalizedPosition= 0;
            for (int i = currentOffset; i < currentOffset + loadLimit; i++)
            {
                if (((loadLimit*currentPage)+i-currentOffset)== totalJiaoCais.Count)
                {
                    currentOffset = totalJiaoCais.Count;
                    return;
                }
                GameObject goLoad = ResManager.Instance.Load<GameObject>(WindowGrid.TextbookMgrGrid);
                GameObject go = Instantiate(goLoad, s_LoadTextbookArea.transform);
                
                go.GetComponentInChildren<Text>().text = totalJiaoCais[i].name;
                //SetTextWithEllipsis(go.GetComponentInChildren<Text>(), totalJiaoCais[i].name);
                Toggle toggle = go.transform.GetComponent<Toggle>();
                int num = i;
                JiaoCaiDataInfoResult jiaoCaiDataInfoResult = totalJiaoCais[i];
                toggle.isOn = false;
                toggle.onValueChanged.AddListener((bool value)=>
                {
                    if (isReshow)
                    {
                        return;
                    }
                    if (value)
                    {
                        //缓存到本地
                        EditUserTextbookBodyRequest editUserTextbookBodyRequest = new EditUserTextbookBodyRequest();
                        editUserTextbookBodyRequest.source = "VR";
                        editUserTextbookBodyRequest.teaching_material_edition = s_CurrentPressID;
                        editUserTextbookBodyRequest.teaching_material_grade = jiaoCaiDataInfoResult.grade;
                        editUserTextbookBodyRequest.teaching_material_icon = jiaoCaiDataInfoResult.thumbnail;
                        editUserTextbookBodyRequest.teaching_material_id = jiaoCaiDataInfoResult.id;
                        editUserTextbookBodyRequest.teaching_material_name = jiaoCaiDataInfoResult.name;
                        editUserTextbookBodyRequest.teaching_material_subject = jiaoCaiDataInfoResult.subject;
                        saveDatas.Add(editUserTextbookBodyRequest);

                        go.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
                        ShowSubjectNum();
                    }
                    else
                    {
                        for (int j = 0; j < saveDatas.Count; j++)
                        {
                            if (totalJiaoCais[num].id == saveDatas[j].teaching_material_id)
                            {
                                saveDatas.RemoveAt(j);
                            }
                        }
                        go.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
                        ShowSubjectNum();
                    }
                });
               
                StartCoroutine(AddImage(totalJiaoCais[i].thumbnail, go.transform.GetChild(0).GetComponent<Image>()));
                textbooks.Add(go);
            }
            currentOffset = loadLimit * (currentPage + 1);
        }
        //刷新学科右边数字显示
        private void ShowSubjectNum()
        {
            int Total = 0;
            foreach (KeyValuePair<string, Text> st in subjectTextDic)
            {

                int num = 0;
                for (int i = 0; i < saveDatas.Count; i++)
                {
                    if (s_CurrentPressID == "没有")
                    {
                        if (st.Key == getKeyDic[saveDatas[i].teaching_material_subject])
                        {
                            num++;
                            Total++;
                        }
                    }
                    if (s_CurrentPressID == saveDatas[i].teaching_material_edition)
                    {
                        if (st.Key ==getKeyDic[saveDatas[i].teaching_material_subject])
                        {
                            num++;
                            Total++;
                        }
                    }
                }
                if (num != 0)
                {
                    st.Value.text = st.Key + "(" + num + ")";
                }
                else
                {
                    st.Value.text = st.Key;
                }
                
            }
            if (Total == 0)
            {
                subjectTextDic["全部"].text = "全部";
            }
            else
            {

                subjectTextDic["全部"].text = "全部" + "(" + Total + ")";
            }
        }
        //从服务器/本地读取已选择信息
        private void GetSelectedTextbook(string selectedSubject = null)
        {
            //userChooseItemResults.Clear();
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
                userChooseList.page = s_CurrentPage;
                userChooseList.size = 10;
                userChooseList.subject = selectedSubject;
                userChooseList.edition = "";

                client.PostData = JsonUtility.ToJson(userChooseList);
                string result = client.HttpRequest(CommonConstant.GET_USER_CHOOSE_LIST);
                //判断获取已选的教材信息
                if (string.IsNullOrEmpty(result))
                {
                    Debug.Log(client.ErrorMessage);
                    return;
                }
                else
                {
                    UserChooseInfoResult chooseInfoResult = new UserChooseInfoResult();
                    chooseInfoResult = JsonUtility.FromJson<UserChooseInfoResult>(result);
                    //userChooseItemResults.Clear();
                    for (int i = 0; i < chooseInfoResult.data.items.Count; i++)
                    {
                        //存储获取到的教材信息
                        EditUserTextbookBodyRequest editUserTextbookBodyRequest = new EditUserTextbookBodyRequest();
                        editUserTextbookBodyRequest.source = "VR";
                        editUserTextbookBodyRequest.teaching_material_edition = chooseInfoResult.data.items[i].teaching_material_edition;
                        editUserTextbookBodyRequest.teaching_material_grade = chooseInfoResult.data.items[i].teaching_material_grade;
                        editUserTextbookBodyRequest.teaching_material_icon = chooseInfoResult.data.items[i].teaching_material_icon;
                        editUserTextbookBodyRequest.teaching_material_id = chooseInfoResult.data.items[i].teaching_material_id;
                        editUserTextbookBodyRequest.teaching_material_name = chooseInfoResult.data.items[i].teaching_material_name;
                        editUserTextbookBodyRequest.teaching_material_subject = chooseInfoResult.data.items[i].teaching_material_subject;
                        saveDatas.Add(editUserTextbookBodyRequest);
                    }

                }
            }
            else
            {
                //从本地读取
                for (int i = 0; i < TextbookMgrWindowData.Instance.userChooseItemResults.Count; i++)
                {
                    saveDatas.Add(TextbookMgrWindowData.Instance.userChooseItemResults[i]);
                }
            }
        }
        //用于控制是否是重选
        private bool isReshow;
        //复选
        private void ReShowSelectedTextbook()
        {
            isReshow = true;
            for (int i = 0; i < totalJiaoCais.Count; i++)
            {
                for (int j = 0; j < saveDatas.Count; j++)
                {
                    if (totalJiaoCais[i].id == saveDatas[j].teaching_material_id)
                    {
                        textbooks[i].transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
                        textbooks[i].transform.GetComponent<Toggle>().isOn = true;
                    }
                }
            }
            isReshow = false;
        }
        //加载失败
        private void LoadingFail()
        {
            //显示效果变化
            isLoading = false;
            s_LoadingIcon.sprite = Resources.Load<Sprite>("WindowUI/TextbookWindowUI/组 48") as Sprite;
            s_LoadingText.text = s_LoadingFailString;
            s_LoadingText.color = s_LoadingFailColor;
            //重试按钮激活。-----------------------------------------------------------------------------------------
        }
        #endregion

        #region 工具方法
        //读图用工具
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
        //清空表内所有预制体。刷新调用
        private void ClearTextbook()
        {
            for (int i = 0; i < textbooks.Count; i++)
            {
                DestroyImmediate(textbooks[i]);
            }
            totalJiaoCais.Clear();
            textbooks.Clear();
        }

        public void SetTextWithEllipsis(Text textComponent, string value)
        {
            var generator = new TextGenerator();
            var rectTransform = textComponent.GetComponent<RectTransform>();
            var settings = textComponent.GetGenerationSettings(rectTransform.rect.size);
            generator.Populate(value, settings);
            var characterCountVisible = generator.characterCountVisible;
            var updatedText = value;
            if (value.Length > characterCountVisible)
            {
                updatedText = value.Substring(0, characterCountVisible - 3);
                updatedText += "…";
            }
            textComponent.text = updatedText;
        }

        #endregion
        #region 按钮点击事件
        private void RetryBtnOnClick()
        {

        }
        private void EditGradeBtnOnClick()
        {
            WindowManager.Hide("TextbookMgrWindow");
            BaseInfoWindowData.Instance.FromWindow = "TextbookMgrWindow";
            WindowManager.Open<BaseInfoWindow>();
        }
        private void ConfirmBtnOnClick()
        {
            if (LoginWindowData.Instance.IsLogin)
            {
                //数据操作
                client.EndPoint = CommonConstant.SERVER_URL_MEC_PROXY;
                client.Method = EnumHttpVerb.POST;
                EditUserTextbookRequest editUserTextbookRequest = new EditUserTextbookRequest();
                MacInfoRequest macInfoRequest = new MacInfoRequest();
                if (LoginWindowData.Instance.ReadAccountLoginResult() != null)
                {
                    macInfoRequest.mac_key = LoginWindowData.Instance.ReadAccountLoginResult().mac_key;
                    macInfoRequest.token = LoginWindowData.Instance.ReadAccountLoginResult().access_token;
                }
                else if (LoginWindowData.Instance.ReadToKenSwapResult() != null)
                {
                    macInfoRequest.mac_key = LoginWindowData.Instance.ReadToKenSwapResult().data.mac_key;
                    macInfoRequest.token = LoginWindowData.Instance.ReadToKenSwapResult().data.access_token;
                }
                macInfoRequest.session_key = LoginWindowData.Instance.ReadSessionResult().session_key;
                editUserTextbookRequest.macInfo = macInfoRequest;
                editUserTextbookRequest.body = saveDatas;
                client.PostData = JsonUtility.ToJson(editUserTextbookRequest);
                string result = client.HttpRequest(CommonConstant.EDIT_USER_TEXTBOOK);
            }
            TextbookMgrWindowData.Instance.userChooseItemResults.Clear();
            for (int i = 0; i < saveDatas.Count; i++)
            {
                TextbookMgrWindowData.Instance.userChooseItemResults.Add(saveDatas[i]);
            }
            if (TextbookMgrWindowData.Instance.FormWindow == "TextbookSelectWindow")
            {
                WindowManager.Open<TextbookSelectWindow>();
                WindowManager.Close("TextbookMgrWindow");
            }
            else if(TextbookMgrWindowData.Instance.FormWindow == "ShiYanXuanZeWindow")
            {
                WindowManager.Open<ShiYanXuanZeWindow>();
                WindowManager.Close("TextbookMgrWindow");
            }
            ShiYanXuanZeWindowData.Instance.GetRecentTextbooksID = "";
        }
        private void JumpLoginBtnOnClick()
        {
            WindowManager.Hide("TextbookMgrWindow");
            WindowManager.Open<LoginWindow>();
        }
        private void ReturnBtnOnClick()
        {
            WindowManager.ReShow();
            WindowManager.Close("TextbookMgrWindow");
        }
        private void LoadMoreBtnOnClick()
        {
            currentPage++;
            ShowAllTextbook();
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

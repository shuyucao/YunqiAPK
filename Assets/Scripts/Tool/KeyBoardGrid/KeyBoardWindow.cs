using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
namespace Assets.Scripts.Tool.KeyBoardGrid
{
    public class KeyBoardWindow: MonoBehaviour
    {
        #region 属性定义
        Transform t_LeftPanel;
        Transform t_MiddlePanel;
        Transform t_RightPanel;

        List<KeyBoardGridItem> keyBoardGridItems_LeftPanel;
        List<KeyBoardGridItem> keyBoardGridItems_MiddlePanel;
        List<KeyBoardGridItem> keyBoardGridItems_RightPanel;

        string[] symbleStrings = new string[30] { ",", ".", "?", "!", "'", ":", "~", "@", ";", "\"", "/", "(", ")", "_", "-", "+", "=", "#", "*", "%", "&", "\\", "[", "]", "<", ">", "{", "}", "|", "`" };
        string[] originStrings = new string[30];
        public bool isUpper = false;
        public bool isSymble = false;

        CustomInput inputText;
        [HideInInspector]
        public Button enterBtn;
        #endregion
        #region 初始化数据
         public void Init(CustomInput text)
        {
            SetInputField(text);
            InitData();
            GameObject prefabLoadBlock = Resources.Load<GameObject>("KeyBoard/Blocker");
            GameObject g_KeyboardBlock = Instantiate(prefabLoadBlock, GameObject.Find("Canvas").transform);
            KeyboardManager.Instance.blocker = g_KeyboardBlock;
        }
        public void SetInputField(CustomInput text)
        {
            this.inputText = text;
        }
        private void InitData()
        {
            InitTransform();
            InitItem();
            GetOriginString();

        }
        private void GetOriginString()
        {
            for (int i = 0; i < keyBoardGridItems_MiddlePanel.Count-2; i++)
            {
                originStrings[i] = keyBoardGridItems_MiddlePanel[i].itemText.text;
            }
        }
        //绑定三个面板位置
        private void InitTransform()
        {
            t_LeftPanel = GameObject.Find("KeyBoardLeftPanel").transform;
            t_MiddlePanel = GameObject.Find("KeyBoardMiddlePanel").transform;
            t_RightPanel = GameObject.Find("KeyBoardRightPanel").transform;
        }
        //绑定面板内按钮
        private void InitItem()
        {
            keyBoardGridItems_LeftPanel = new List<KeyBoardGridItem>();
            keyBoardGridItems_MiddlePanel = new List<KeyBoardGridItem>();
            keyBoardGridItems_RightPanel = new List<KeyBoardGridItem>();
            AddItemToList(keyBoardGridItems_LeftPanel, t_LeftPanel);
            AddItemToList(keyBoardGridItems_MiddlePanel, t_MiddlePanel);
            AddItemToList(keyBoardGridItems_RightPanel, t_RightPanel);

        }
        private void AddItemToList(List<KeyBoardGridItem> itemList ,Transform transform)
        {
            KeyBoardGridItem[]  keyBoardGridItems = transform.GetComponentsInChildren<KeyBoardGridItem>();
            for (int i = 0; i < keyBoardGridItems.Length; i++)
            {
                Button button = keyBoardGridItems[i].GetComponent<Button>();
                button.onClick.AddListener(keyBoardGridItems[i].OnClick);
                keyBoardGridItems[i].InitItem();
                keyBoardGridItems[i].keyBoardWindow = this;
                itemList.Add(keyBoardGridItems[i]) ;
            }
        }
        #endregion
        #region 按钮点击事件
        public void OnSymbelBtnClick()
        {
            if(isSymble)
            {
                for (int i = 0; i < keyBoardGridItems_MiddlePanel.Count - 2; i++)
                {
                    keyBoardGridItems_MiddlePanel[i].itemText.text = originStrings[i];
                    if(originStrings[i] == "Shift")
                    {
                        keyBoardGridItems_MiddlePanel[i].gameObject.transform.GetChild(1).gameObject.SetActive(true);
                        Color color;
                        color.r = 255f;
                        color.g = 255f;
                        color.b = 255f;
                        color.a = 0f;
                        keyBoardGridItems_MiddlePanel[i].itemText.color = color;
                    }
                }
                isSymble = false;
            }
            else
            {
                for (int i = 0; i < keyBoardGridItems_MiddlePanel.Count - 2; i++)
                {
                    keyBoardGridItems_MiddlePanel[i].itemText.text = symbleStrings[i];
                    if (originStrings[i] == "Shift")
                    {
                        keyBoardGridItems_MiddlePanel[i].itemImage.enabled = false;
                        keyBoardGridItems_MiddlePanel[i].gameObject.transform.GetChild(1).gameObject.SetActive(false);
                        Color color;
                        color.r = 255f;
                        color.g = 255f;
                        color.b = 255f;
                        color.a = 255f;
                        keyBoardGridItems_MiddlePanel[i].itemText.color = color;
                    }
                }
                isSymble = true;
                isUpper = false;
            }
           
        }
        public void OnDeleteBtnClick()
        {
            if(inputText.TextStr.Length==0)
            {
                return;
            }
            inputText.TextStr = inputText.TextStr.Substring(0, inputText.TextStr.Length-1);
        }
        public void OnEnterBtnClick()
        {
            enterBtn.onClick.Invoke();
            OnCloseBtnClick();
        }
        public void OnCloseBtnClick()
        {
            KeyboardManager.Instance.SetKeyboard(null);
            GameObject gameObject = KeyboardManager.Instance.blocker;

            Destroy(gameObject);
            Destroy(this.gameObject);

            KeyboardManager.Instance.SetKeyboard(null);
            KeyboardManager.Instance.blocker = null;
        }
        public void OnShiftBtnClick()
        {
            if (isUpper)
            {
                for (int i = 0; i < keyBoardGridItems_MiddlePanel.Count; i++)
                {
                    keyBoardGridItems_MiddlePanel[i].ToLowerLetter();
                }
                isUpper = false;
            }
            else
            {
                //切换所有大小写
                for (int i = 0; i < keyBoardGridItems_MiddlePanel.Count; i++)
                {
                    keyBoardGridItems_MiddlePanel[i].ToUpperLetter();
                }
                isUpper = true;
            }
        }
        public void OnSpaceBtnClick()
        {
            OnSimpleClick(" ");
        }
        public void OnSimpleClick(string str)
        {
            inputText.TextStr = inputText.TextStr + str;
        }
        #endregion
        void OnDestroy()
        {
            if (string.IsNullOrEmpty(inputText.TextStr))
            {
                inputText.MaskText.enabled = true;
            }
        }
    }
}

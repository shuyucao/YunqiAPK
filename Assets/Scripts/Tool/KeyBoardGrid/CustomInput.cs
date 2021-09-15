using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Text.RegularExpressions;
namespace Assets.Scripts.Tool.KeyBoardGrid
{
    public class CustomInput:MonoBehaviour
    {
        /// <summary>
        /// 输入的类型，是密码还是普通类型
        /// </summary>
        public enum InputType
        {
            Standard,
            Password,
        }

        /// <summary>
        /// 输入的限制类型=》无，数字，字符
        /// </summary>
        public enum Validation
        {
            None,
            Integer,
            CharOnly,
        }

        /// <summary>
        /// 键盘回车键对应的按钮功能
        /// </summary>
        public Button enterBtn;

        /// <summary>
        /// 输入框的显示类型
        /// </summary>
        public InputType inputType = InputType.Standard;

        /// <summary>
        /// 输入框值的类型
        /// </summary>
        public Validation validation = Validation.None;

        /// <summary>
        /// 用于响应选中当前输入框
        /// </summary>
        private Button thisBtn;

        KeyBoardWindow keyBoardWindow;

        /// <summary>
        /// 输入框的提示遮罩，在输入框值为空时隐藏
        /// </summary>
        public Text MaskText;

        /// <summary>
        /// Text值改变监听
        /// </summary>
        /// <param name="newText"></param>
        public delegate void OnTextChangeDelegate(string newString);

        public event OnTextChangeDelegate OnVariableChange;

        /// <summary>
        /// 输入框的显示。
        /// </summary>
        public Text Text;

        /// <summary>
        /// 密码模式用来存储数值
        /// </summary>
        private string textValue;

        /// <summary>
        /// 当前组件所在的幕布
        /// </summary>
        public Canvas Canvas;
        //private string m_str;
        [HideInInspector]
        public string TextStr
        {
            get
            {
                return textValue;
            }
            set
            {
                if (Text.text == value) return;
                //判断是否是密码模式
                if (inputType == InputType.Password)
                {
                    ShowPassword(value);
                }
                else
                {
                    if (validation == Validation.Integer)
                    {
                        if (!ValidateNum(value))
                        {
                            return;
                        }
                    }
                    Text.text = value;
                }
                if (OnVariableChange != null)
                    OnVariableChange(value);
                textValue = value;
            }
        }
        public static bool ValidateNum(string strNum)
        {
            return Regex.IsMatch(strNum, "^[0-9]*$");
        }


        void Awake()
        {
            thisBtn = GetComponent<Button>();
            thisBtn.onClick.AddListener(InputOnClick);
            OnVariableChange += OnTextValueChange;
        }

        /// <summary>
        /// 当选中当前输入框时
        /// </summary>
        void InputOnClick()
        {
            MaskText.enabled = false;
            CreateKeyBoard();
        }

        /// <summary>
        /// 创建键盘
        /// </summary>
        public void CreateKeyBoard()
        {
            GameObject prefabLoad = Resources.Load<GameObject>("KeyBoard/KeyBoard");
            GameObject g_Keyboard = Instantiate(prefabLoad, GameObject.Find("Canvas").transform);
            g_Keyboard.transform.position = new Vector3(this.transform.position.x, this.transform.position.y-2.5f, this.transform.position.z);
            keyBoardWindow = g_Keyboard.GetComponent<KeyBoardWindow>();
            keyBoardWindow.Init(this);
            keyBoardWindow.enterBtn = enterBtn;
            KeyboardManager.Instance.SetKeyboard(g_Keyboard);
        }
        public void ShowPassword(string strPW)
        {
            string stars = null;
            for (int i = 0; i < strPW.Length; i++)
            {
                stars = stars + "*";
            }
            Text.text = stars;
        }
        
        /// <summary>
        /// 字符串更改回调事件
        /// </summary>
        /// <param name="str"></param>
        void OnTextValueChange(string str)
        {
            if (str.Length>0)
            {
                MaskText.enabled = false;
            }
            else
            {
                MaskText.enabled = true;
            }
        }

        public void ForceLabelUpdate()
        {
            Debug.LogWarning("++++++++++++++++++++++++++++++++"+ inputType);
            if (inputType == InputType.Password)
            {
                ShowPassword(textValue);
            }
            if (inputType == InputType.Standard)
            {
                Text.text = textValue;
            }
        }


        /// <summary>
        /// 获取到Text的字符片段位置
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="text"></param>
        /// <param name="strFragment"></param>
        /// <returns></returns>
        public Vector3 GetPosAtText(Canvas canvas, Text text, string strFragment)
        {
            int strFragmentIndex = text.text.IndexOf(strFragment);//-1表示不包含strFragment
            Vector3 stringPos = Vector3.zero;
            if (strFragmentIndex > -1)
            {
                Vector3 firstPos = GetPosAtText(canvas, text, strFragmentIndex + 1);
                Vector3 lastPos = GetPosAtText(canvas, text, strFragmentIndex + strFragment.Length);
                stringPos = (firstPos + lastPos) * 0.5f;
            }
            else
            {
                stringPos = GetPosAtText(canvas, text, strFragmentIndex);
            }
            return stringPos;
        }

        /// <summary>
        /// 得到Text中字符的位置；canvas:所在的Canvas，text:需要定位的Text,charIndex:Text中的字符位置
        /// </summary>
        public Vector3 GetPosAtText(Canvas canvas, Text text, int charIndex)
        {
            string textStr = text.text;
            Vector3 charPos = Vector3.zero;
            if (charIndex <= textStr.Length && charIndex > 0)
            {
                TextGenerator textGen = new TextGenerator(textStr.Length);
                Vector2 extents = text.gameObject.GetComponent<RectTransform>().rect.size;
                textGen.Populate(textStr, text.GetGenerationSettings(extents));

                int newLine = textStr.Substring(0, charIndex).Split('\n').Length - 1;
                int whiteSpace = textStr.Substring(0, charIndex).Split(' ').Length - 1;
                int indexOfTextQuad = (charIndex * 4) + (newLine * 4) - 4;
                if (indexOfTextQuad < textGen.vertexCount)
                {
                    charPos = (textGen.verts[indexOfTextQuad].position +
                        textGen.verts[indexOfTextQuad + 1].position +
                        textGen.verts[indexOfTextQuad + 2].position +
                        textGen.verts[indexOfTextQuad + 3].position) / 4f;


                }
            }
            charPos /= canvas.scaleFactor;//适应不同分辨率的屏幕
            charPos = text.transform.TransformPoint(charPos);//转换为世界坐标
            return charPos;
        }
    }
}

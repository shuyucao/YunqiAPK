using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
namespace Assets.Scripts.Tool.KeyBoardGrid
{
    public class KeyBoardGridItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler/*,IPointerClickHandler,IPointerDownHandler,IPointerUpHandler*/
    {
        #region 属性定义
        [HideInInspector]
        public Image itemImage;

        [HideInInspector]
        public Text itemText;

        [HideInInspector]
        public KeyBoardWindow keyBoardWindow;
        #endregion
        #region 初始化方法
        public void InitItem()
        {
            itemImage = this.gameObject.GetComponent<Image>();
            itemText = this.gameObject.GetComponentInChildren<Text>();
        }
        #endregion
        public void OnClick()
        {
            string str = itemText.text;
            switch (str)
            {
                case "Shift":
                    keyBoardWindow.OnShiftBtnClick();
                    break;
                case "# + =":
                    keyBoardWindow.OnSymbelBtnClick();
                    break;
                case "Space":
                    keyBoardWindow.OnSpaceBtnClick();
                    break;
                case "Delete":
                    keyBoardWindow.OnDeleteBtnClick();
                    break;
                case "Enter":
                    keyBoardWindow.OnEnterBtnClick();
                    break;
                case "Close":
                    keyBoardWindow.OnCloseBtnClick();
                    break;
                default:
                    keyBoardWindow.OnSimpleClick(str);
                    break;
            }
        }
        public void ToUpperLetter()
        {
            string str = itemText.text;
            switch (str)
            {
                case "Shift":
                    itemImage.enabled = true;
                    break;
                case "# + =":
                    return;
                case "Space":
                    return;
                default:
                    itemText.text = itemText.text.ToUpper();
                    break;
            }
        }
        public void ToLowerLetter()
        {
            string str = itemText.text;
            switch (str)
            {
                case "Shift":
                    itemImage.enabled = false;
                    break;
                case "# + =":
                    return;
                case "Space":
                    return;
                default:
                    itemText.text = itemText.text.ToLower();
                    break;
            }
        }
        public void OnPointerEnter(PointerEventData eventData)
        {
            //图标变亮
            itemImage.enabled = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if(itemText.text=="Shift"&&keyBoardWindow.isUpper)
            {
                return;
            }
            if(itemText.text == "# + =" && keyBoardWindow.isSymble)
            {
                return;
            }
            else
            {
                itemImage.enabled = false;
            }
            //图标变暗
        }

        //public void OnPointerClick(PointerEventData eventData)
        //{
        //    OnClick();
        //}

        //public void OnPointerDown(PointerEventData eventData)
        //{
        //    return;
        //}

        //public void OnPointerUp(PointerEventData eventData)
        //{
        //    return;
        //}
    }
}

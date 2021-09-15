using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
namespace Assets.Scripts.Tool.KeyBoardGrid
{
    class Blocker:MonoBehaviour
    {
        Button button;
        void Start()
        {
            button = this.GetComponent<Button>();
            button.onClick.AddListener(BtnClick);
        }
        void BtnClick()
        {
            GameObject gameObject = KeyboardManager.Instance.GetKeyboard();
            KeyBoardWindow keyBoardWindow = gameObject.GetComponent<KeyBoardWindow>();
            keyBoardWindow.OnCloseBtnClick();
            //DestroyImmediate(gameObject);
            //DestroyImmediate(this.gameObject);
        }
    }
}

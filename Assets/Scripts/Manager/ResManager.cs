using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Collections;
using Assets.Scripts.Tool;
namespace Assets.Scripts.Manager
{
    public class ResManager : Singleton<ResManager>
    {
        public T Load<T>(object enumName)where T : Object
        {
            // 获取枚举类型的字符串形式
            string enumType = enumName.GetType().Name;

            //空的字符串
            string filePath = string.Empty;
            //寻找地址，每次新增页面或者音频资源需要添加
            switch (enumType)
            {
                case "Music":
                    {
                        filePath = "Music/" + enumName.ToString();
                        break;
                    }
                case "Window":
                    {
                        filePath = "Window/" + enumName.ToString();
                        break;
                    }
                case "WindowGrid":
                    {
                        filePath = "WindowGrid/" + enumName.ToString();
                        break;
                    }
                case "Item":
                    {
                        filePath = "Item/" + enumName.ToString();
                        break;
                    }
                case "UI":
                    {
                        filePath = "UI/" + enumName.ToString();
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
            return Resources.Load<T>(filePath);
        }
    }
}

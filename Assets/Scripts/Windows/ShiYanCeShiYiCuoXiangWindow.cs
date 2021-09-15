using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Assets.Scripts.Tool.RestFulClient;
using UnityEngine.UI;
using UnityEngine;
using Assets.Scripts.Constant;
using Assets.Scripts.Request;
using Assets.Scripts.Result;
using Assets.Scripts.Data;
using Assets.Scripts.Manager;
using System.Collections;
using Pvr_UnitySDKAPI;
using Assets.Scripts.Config;

namespace Assets.Scripts.Windows
{
    public  class ShiYanCeShiYiCuoXiangWindow:BaseWindow
    {
        //返回按钮
        private Button ReturnYiCuoXiang;

        //item的父物体
        private GameObject ViewParent;

        RestClient client = new RestClient();

        private GameObject TempObj = null;
        //有无易错项记录的界面
        private GameObject YesYiCouXiangPanel;
        private GameObject NoYiCouXiangPanel;
        public override void OnInit()
        {
            base.OnInit();
            prefabType = PrefabType.Window.ShiYanCeShiYiCuoXiangWindow;
            prefab = gameObject;
        }

        public override void Init()
        {
            base.Init();
            InitData();
            DefalutSeting();
        }


        private void InitData() 
        {
            YesYiCouXiangPanel = GameObject.Find("YesYiCuoXiang");
            NoYiCouXiangPanel = GameObject.Find("NoYiCuoXiang");
            ReturnYiCuoXiang = GameObject.Find("ReturnYiCuoXiang").GetComponent<Button>();
            ViewParent = GameObject.Find("ViewYiCuoXiang");

            ReturnYiCuoXiang.onClick.AddListener(ReturnYiCuoXiangOnClick);
        }

        //返回
        private void ReturnYiCuoXiangOnClick()
        {
            WindowManager.Close("ShiYanCeShiYiCuoXiangWindow");
        }

        private void DefalutSeting() 
        {
            if (ShiYanCeShiXiangQingMainWindowData.Instance.IsNoYiCuoXiangData)
            {
                YesYiCouXiangPanel.SetActive(true);
                NoYiCouXiangPanel.SetActive(false);
                GetShiYanYiCuoXiangInfo(ShiYanCeShiXiangQingMainWindowData.Instance.GetInstanceId);
            }
            else
            {
                YesYiCouXiangPanel.SetActive(false);
                NoYiCouXiangPanel.SetActive(true);
            }
        }

        //获取实验易错项的
        private void GetShiYanYiCuoXiangInfo(string instanceId) 
        {
            client.EndPoint = CommonConstant.SERVER_URL_MEC_PROXY;
            client.Method = EnumHttpVerb.POST;
            ShiYanCeShiYiCuoXiangRequest yanCeShiYiCuoXiangRequest = new ShiYanCeShiYiCuoXiangRequest();
            yanCeShiYiCuoXiangRequest.instanceId = instanceId;
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
            yanCeShiYiCuoXiangRequest.macInfo = macInfo;
            client.PostData = JsonUtility.ToJson(yanCeShiYiCuoXiangRequest);
            string result = client.HttpRequest(CommonConstant.GET_SHIYANCESHIYICUOXIANG);
            Debug.Log("易错项：" + result);
            if (string.IsNullOrEmpty(result))
            {
                Debug.Log(client.ErrorMessage);
                return;
            }
            else
            {
                ShiYanCeShiYiCuoXiangResult yanCeShiYiCuoXiangResult = new ShiYanCeShiYiCuoXiangResult();
                yanCeShiYiCuoXiangResult = JsonUtility.FromJson<ShiYanCeShiYiCuoXiangResult>(result);
                ShiYanCeShiYiCuoXiangWindowData.Instance.SaveShiYanCeShiYiCuoXiangResult(yanCeShiYiCuoXiangResult);
                StartCoroutine(LoadYiCuoXiangItem(yanCeShiYiCuoXiangResult));
            }
        }

        //加载Item
        private IEnumerator LoadYiCuoXiangItem(ShiYanCeShiYiCuoXiangResult yanCeShiYiCuoXiangResult)
        {
            if (yanCeShiYiCuoXiangResult.data.Count == 0)
            {
                yield break;
            }
            else
            {
                for (int i = 0; i < yanCeShiYiCuoXiangResult.data.Count; i++)
                {
                      if (i % 2 == 0)
                      {
                          TempObj = ResManager.Instance.Load<GameObject>(ItemType.Item.YiCuoXiangItem1);
                      }
                      else
                      {
                          TempObj = ResManager.Instance.Load<GameObject>(ItemType.Item.YiCuoXiangItem2);
                      }
                      GameObject item = Instantiate(TempObj);
                      item.transform.parent = ViewParent.transform;
                      item.transform.localScale = Vector3.one;
                      item.transform.localPosition = Vector3.zero;
                      item.transform.localEulerAngles = Vector3.zero;    
                      if (yanCeShiYiCuoXiangResult.data[i].is_new == 0)
                      {
                          item.transform.GetChild(0).gameObject.SetActive(false);
                      }
                      else if (yanCeShiYiCuoXiangResult.data[i].is_new == 1)
                      {
                          item.transform.GetChild(0).gameObject.SetActive(true);
                      }
                      item.transform.GetChild(1).GetComponent<Text>().text = (i + 1).ToString();
                      if (yanCeShiYiCuoXiangResult.data[i].error_type == 0)
                      {
                          item.transform.GetChild(2).GetComponent<Text>().text = "操作错误";
                      }
                      else if (yanCeShiYiCuoXiangResult.data[i].error_type == 1)
                      {
                          item.transform.GetChild(2).GetComponent<Text>().text = "记录错误";
                      }
                      item.transform.GetChild(3).GetComponent<Text>().text = yanCeShiYiCuoXiangResult.data[i].error_count.ToString();
                      item.transform.GetChild(4).GetComponent<Text>().text = yanCeShiYiCuoXiangResult.data[i].error_name;
                      item.transform.GetChild(5).GetComponent<Text>().text = yanCeShiYiCuoXiangResult.data[i].analysis;
                      item.transform.GetChild(6).GetComponent<Slider>().value = yanCeShiYiCuoXiangResult.data[i].error_rate;
                      float height1 = LayoutUtility.GetPreferredHeight(item.transform.GetChild(4).GetComponent<Text>().GetComponent<RectTransform>());
                      float height2 = LayoutUtility.GetPreferredHeight(item.transform.GetChild(5).GetComponent<Text>().GetComponent<RectTransform>());
                      if (height1 >= height2)
                      {
                          item.GetComponent<RectTransform>().sizeDelta = new Vector2(1203f, height1 + 36f);
                      }
                      else
                      {
                          item.GetComponent<RectTransform>().sizeDelta = new Vector2(1203f, height2 + 36f);
                      }
                      yield return new WaitForEndOfFrame();
                }
            }
        }


        public void Update()
        {
            MovePanelByJoystick();
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
        public override void OnShow(params object[] para)
        {
            base.OnShow(para);
        }

        public override void OnReShow()
        {
            base.OnReShow();
        }

        public override void OnHide()
        {
            base.OnHide();
        }

        public override void OnClose()
        {
            base.OnClose();
        }
    }
}

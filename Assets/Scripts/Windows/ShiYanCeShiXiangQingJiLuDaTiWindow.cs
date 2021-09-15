using Assets.Scripts.Config;
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

namespace Assets.Scripts.Windows
{
    public class ShiYanCeShiXiangQingJiLuDaTiWindow : BaseWindow
    {
        RestClient client = new RestClient();

        //返回
        private Button ReturnDaTiBtn;

        private GameObject DaTiContent;

        public override void OnInit()
        {
            base.OnInit();
            prefabType = PrefabType.Window.ShiYanCeShiXiangQingJiLuDaTiWindow;
            prefab = gameObject;
        }

        public override void Init()
        {
            base.Init();
            InitData();
            DefaultSeting();
        }

        
        //初始化组件
        private void InitData() 
        {
            ReturnDaTiBtn = GameObject.Find("ReturnXQJiLuDaTi").GetComponent<Button>();
            DaTiContent = GameObject.Find("DaTiContent");
            ShiYanXuanZeWindowData.Instance.TempObject = DaTiContent;
        }

        //默认设置
        private void DefaultSeting() 
        {
            ReturnDaTiBtn.onClick.AddListener(ReturnOnClick);
            if (ShiYanCeShiXiangQingJiLuDaTiWindowData.Instance.GetEntryID != "")
            {
                GetShiYanCeShiDaTiDataInfo(ShiYanCeShiXiangQingMainWindowData.Instance.GetInstanceId, ShiYanCeShiXiangQingJiLuDaTiWindowData.Instance.GetEntryID);
            }
        }

        //返回
        private void ReturnOnClick()
        {
            WindowManager.ReShow();
            WindowManager.Close("ShiYanCeShiXiangQingJiLuDaTiWindow");
        }

        //获取实验
        private void GetShiYanCeShiDaTiDataInfo(string instanceId,string entryId) 
        {
            client.EndPoint = CommonConstant.SERVER_URL_MEC_PROXY;
            client.Method = EnumHttpVerb.POST;
            ShiYanCeShiXingQingDaTiRequest yanCeShiXingQingDaTiRequest = new ShiYanCeShiXingQingDaTiRequest();
            yanCeShiXingQingDaTiRequest.instanceId = instanceId;
            yanCeShiXingQingDaTiRequest.entryId = entryId;
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
            yanCeShiXingQingDaTiRequest.macInfo = macInfo;
            client.PostData = JsonUtility.ToJson(yanCeShiXingQingDaTiRequest);
            string reslut = client.HttpRequest(CommonConstant.GET_SHIYANXIAINGQINGCESHIDATIJILU);
            if (string.IsNullOrEmpty(reslut))
            {
                Debug.Log(client.ErrorMessage);
                return;
            }
            else
            {
                ShiYanCeShiXiangQingDaTiDataResult ceShiXiangQingDaTiDataResult = new ShiYanCeShiXiangQingDaTiDataResult();
                ceShiXiangQingDaTiDataResult = JsonUtility.FromJson<ShiYanCeShiXiangQingDaTiDataResult>(reslut);
                ShiYanCeShiXiangQingJiLuDaTiWindowData.Instance.SaveShiYanCeShiXiangQingDaTiDataResult(ceShiXiangQingDaTiDataResult);
                StartCoroutine(LoadNodeItem(ceShiXiangQingDaTiDataResult));
            }
        }


        //加载数据Item
        private IEnumerator LoadNodeItem(ShiYanCeShiXiangQingDaTiDataResult ceShiXiangQingDaTiDataResult) 
        {
            if (ceShiXiangQingDaTiDataResult.data.Count == 0)
            {
                Debug.Log("没有数据");
                yield break;
            }
            else
            {
                for (int i = 0; i < ceShiXiangQingDaTiDataResult.data.Count; i++)
                {
                    GameObject TileItem = ResManager.Instance.Load<GameObject>(ItemType.Item.DaTiViewItem);
                    GameObject Item = Instantiate(TileItem);
                    Item.transform.parent = DaTiContent.transform;
                    Item.transform.localPosition = new Vector3(0, 0, -6);
                    Item.transform.localEulerAngles = Vector3.zero;
                    Item.transform.localScale = Vector3.one;
                    Item.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = ceShiXiangQingDaTiDataResult.data[i].step_number + "." + ceShiXiangQingDaTiDataResult.data[i].step_name;
                    Item.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = ceShiXiangQingDaTiDataResult.data[i].score + "分";
                    for (int j = 0; j < ceShiXiangQingDaTiDataResult.data[i].list.Count; j++)
                    {
                        GameObject InfoItem = ResManager.Instance.Load<GameObject>(ItemType.Item.DaTiJieXiItem);
                        GameObject Item1 = Instantiate(InfoItem);
                        Item1.transform.parent = Item.transform;
                        Item1.transform.localPosition = Vector3.zero;
                        Item1.transform.localEulerAngles = Vector3.zero;
                        Item1.transform.localScale = Vector3.one;
                        Item1.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = ceShiXiangQingDaTiDataResult.data[i].list[j].action_name;
                        Item1.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(1160, LayoutUtility.GetPreferredHeight(Item1.transform.GetChild(0).GetChild(1).GetComponent<RectTransform>()));
                        Item1.transform.GetChild(0).GetChild(6).GetChild(2).GetChild(0).GetComponent<Text>().text = ceShiXiangQingDaTiDataResult.data[i].list[j].analysis;
                        if (ceShiXiangQingDaTiDataResult.data[i].list[j].conclusion == 0)
                        {
                            Item1.transform.GetChild(0).GetChild(2).GetComponent<Text>().text = "";
                            Item1.transform.GetChild(0).GetChild(3).gameObject.SetActive(true);
                            Item1.transform.GetChild(1).gameObject.SetActive(false);
                        }
                        else if (ceShiXiangQingDaTiDataResult.data[i].list[j].conclusion == 1)
                        {
                            Item1.transform.GetChild(0).GetChild(2).GetComponent<Text>().text = ceShiXiangQingDaTiDataResult.data[i].list[j].actionScore.ToString();
                            Item1.transform.GetChild(0).GetChild(5).gameObject.SetActive(true);
                            Item1.transform.GetChild(1).GetComponent<Text>().text = "错误原因: " + ceShiXiangQingDaTiDataResult.data[i].list[j].reason;
                        }
                        else if (ceShiXiangQingDaTiDataResult.data[i].list[j].conclusion == 2 || ceShiXiangQingDaTiDataResult.data[i].list[j].conclusion == 3)
                        {
                            Item1.transform.GetChild(0).GetChild(2).GetComponent<Text>().text = ceShiXiangQingDaTiDataResult.data[i].list[j].actionScore.ToString();
                            Item1.transform.GetChild(0).GetChild(4).gameObject.SetActive(true);
                            Item1.transform.GetChild(1).GetComponent<Text>().text = "错误原因: " + ceShiXiangQingDaTiDataResult.data[i].list[j].reason;
                        }
                        LayoutRebuilder.ForceRebuildLayoutImmediate(DaTiContent.GetComponent<RectTransform>());
                    }
                    yield return new WaitForEndOfFrame();
                    LayoutRebuilder.ForceRebuildLayoutImmediate(DaTiContent.GetComponent<RectTransform>());
                }
            }
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

        private void Update()
        {
            //MovePanelByJoystick();
        }
        public override void OnShow(params object[] para)
        {
            base.OnShow(para);
        }

        public override void OnHide()
        {
            base.OnHide();
        }

        public override void OnReShow()
        {
            base.OnReShow();
            ShiYanXuanZeWindowData.Instance.TempObject = DaTiContent;
        }

        public override void OnClose()
        {
            base.OnClose();
            ShiYanXuanZeWindowData.Instance.TempObject = null;
        }
    }
}

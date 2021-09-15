using UnityEngine;
using System.Collections;
using System;
using Assets.Scripts.CyberCloud;

public class SliderController : MonoBehaviour {

    int touchXBegin;
    int touchYBegin;
    int touchXEnd;
    int touchYEnd;

    private bool lark2touchclock1 = false;
    private bool lark2touchclock2 = false;

    public SlideMode slideMode = SlideMode.Vertical;

    public enum SlideMode
    {
        None = -1,
        Vertical = 0,
        Horizontal = 1
    }


    private static SliderController _instance = null;

    public static SliderController Instance
    {
       get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("SliderController");
                _instance = go.AddComponent<SliderController>();
            }
            return _instance;
        }
    }


    float _Horizontal = 0.0f;
    float _Vertical = 0.0f;

    float _Horizontal_Whole = 0.0f;
    float _Vertical_Whole = 0.0f;

    public static event System.Action OnUp = delegate {};
    public static event System.Action OnDown = delegate {};
    public static event System.Action OnLeft = delegate {};
    public static event System.Action OnRight = delegate {};

    
    /// <summary>
    /// 一下滑一页 
    /// </summary>
    public static event System.Action OnUp_Whole = delegate { /*Debug.Log("向上滑了一页");*/ };
    public static event System.Action OnDown_Whole = delegate { /*Debug.Log("向下滑了一页");*/ };
    public static event System.Action OnLeft_Whole = delegate {/* Debug.Log("向左滑了一页"); */};
    public static event System.Action OnRight_Whole = delegate { /*Debug.Log("向右滑了一页");*/ };



    public float Horizontal
    {
        set
        {
            _Horizontal = value;
            if(_Horizontal == 1.0f)
            {
                if(OnRight != null) OnRight();
            }
            else if(_Horizontal == -1.0f)
            {
                if(OnLeft != null) OnLeft();
            }
        }
    }

    public float Vertical
    {
        set
        {
            _Vertical = value;
            if (_Vertical == 1.0f)
            {
                if(OnUp != null) OnUp();
            }
            else if (_Vertical == -1.0f)
            {
                if(OnDown != null) OnDown();
            }
        }
    }


    public float Horizontal_Whole
    {
        set
        {
            _Horizontal_Whole = value;
            if(_Horizontal_Whole == 1.0f)
            {
                if (OnRight_Whole != null) OnRight_Whole();
            }
            else if(_Horizontal_Whole == -1.0f)
            {
                if (OnLeft_Whole != null) OnLeft_Whole();
            }
        }
    }


    public float Vertical_Whole
    {
        set
        {
            _Vertical_Whole = value;
            if(_Vertical_Whole == 1.0f)
            {
                if (OnUp_Whole != null) OnUp_Whole();
            }
            else if(_Vertical_Whole == -1.0f)
            {
                if (OnDown_Whole != null) OnDown_Whole();
            }
        }
    }
 
    private string dm_curDevice = PicoDeviceMode.PicoNeoDK;


    // Use this for initialization
    void Start () {
        //dm_curDevice = Pvr_UnitySDKAPI.System.UPvr_GetDeviceMode();
    }
	
	// Update is called once per frame
	void Update ()
    {
        switch (dm_curDevice)
        {
            case PicoDeviceMode.PicoNeoDK:
            case PicoDeviceMode.PicoNeoDKS:
                SlideAlgorithmForDK();
                SlideAlgorithm();  
                break;
            case PicoDeviceMode.PicoNeo:
                //CV暂不知道是什么，怎么操控;
                SlideAlgorithm();
                break;
            case PicoDeviceMode.PicoGoblin:
                SlideAlgorithm();
                //SlideAlgorithm01();
                //SlideAlgorithm02();
                break;
        }
       
    }


    private float lastTime;

    /// <summary>
    /// 滑动算法  （DK/DKS使用）
    /// </summary>
    void SlideAlgorithmForDK()
    {
        if ((Input.GetKey(KeyCode.W)) || Input.GetAxis("Vertical") > 0.9f)
        {
            if (Time.time - lastTime > 1.0f)
            {
                lastTime = Time.time;
                OnUp_Whole();
            }
        }
        else if ((Input.GetKey(KeyCode.S)) || Input.GetAxis("Vertical") < -0.9f)
        {
            if (Time.time - lastTime > 1.0f)
            {
                lastTime = Time.time;
                OnDown_Whole();
            }
        }
    }




    /// <summary>
    /// 滑动计算 （使用sdk提供的算法及接口 与 SlideAlgorithm01 类似功能）
    /// </summary>
    void SlideAlgorithm()
    {
        /**上滑动**/

        //if (Pvr_UnitySDKAPI.Controller.UPvr_GetSwipeDirection(0) == Pvr_UnitySDKAPI.SwipeDirection.SwipeDown || Pvr_UnitySDKAPI.Controller.UPvr_GetSwipeDirection(1) == Pvr_UnitySDKAPI.SwipeDirection.SwipeDown)
        //{
        //    OnDown_Whole();
       // } /** 下滑动**/
        //else if (Pvr_UnitySDKAPI.Controller.UPvr_GetSwipeDirection(0) == Pvr_UnitySDKAPI.SwipeDirection.SwipeUp || Pvr_UnitySDKAPI.Controller.UPvr_GetSwipeDirection(1) == Pvr_UnitySDKAPI.SwipeDirection.SwipeUp)
        //{
        //    OnUp_Whole();
       // }  /** 左滑动**/
       // else if (Pvr_UnitySDKAPI.Controller.UPvr_GetSwipeDirection(0) == Pvr_UnitySDKAPI.SwipeDirection.SwipeLeft || Pvr_UnitySDKAPI.Controller.UPvr_GetSwipeDirection(1) == Pvr_UnitySDKAPI.SwipeDirection.SwipeLeft)
       // {
       //     OnLeft_Whole();
       // }  /** 右滑动**/
       // else if (Pvr_UnitySDKAPI.Controller.UPvr_GetSwipeDirection(0) == Pvr_UnitySDKAPI.SwipeDirection.SwipeRight || Pvr_UnitySDKAPI.Controller.UPvr_GetSwipeDirection(1) == Pvr_UnitySDKAPI.SwipeDirection.SwipeRight)
        //{
        //    OnRight_Whole();
       // }

    }




    //int touchXBegin0;
    //int touchXEnd0;
    //int touchYBegin0;
    //int touchYEnd0;



    /// <summary>
    /// 滑动算法01 (手指放到触摸板到离开触摸板算一下滑动)
    /// </summary>
//    private void SlideAlgorithm01()
//    {
//        if (Pvr_UnitySDKAPI.TouchPadPosition.x > 0 && Pvr_UnitySDKAPI.TouchPadPosition.y > 0)
//        {
//            if (!lark2touchclock1)
//            {
//                touchXBegin0 = Pvr_UnitySDKAPI.TouchPadPosition.x;
//                touchYBegin0 = Pvr_UnitySDKAPI.TouchPadPosition.y;
//                lark2touchclock1 = true;
//            }
//            touchYEnd0 = Pvr_UnitySDKAPI.TouchPadPosition.y;
//            touchXEnd0 = Pvr_UnitySDKAPI.TouchPadPosition.x;
//        }
//        else
//        {
//            //Debug.Log(" XBegin = " + touchXBegin0 + "    YBegin = " + touchYBegin0 + "  XEnd  = " + touchXEnd0 + "   YEnd  =" + touchYEnd0);
//            if (touchXEnd0 > touchXBegin0)
//            {
//                if (touchYEnd0 > touchYBegin0)
//                {
//                    if (touchXEnd0 - touchXBegin0 > 120 && ((touchXEnd0 - touchXBegin0) > (touchYEnd0 - touchYBegin0)))
//                    {
//                        //向上滑动
//                        //PicoVRManager.SDK.slideUp = 1;
//                        Vertical_Whole = 1.0f;
//                    }
//                    else if (touchYEnd0 - touchYBegin0 > 120 && ((touchYEnd0 - touchYBegin0) > (touchXEnd0 - touchXBegin0)))
//                    {
//                        //向右滑动
//                        //PicoVRManager.SDK.slideRight = 1;
//                        Horizontal_Whole = 1.0f;
//                    }
//                }
//                else
//                {
//                    if (touchXEnd0 - touchXBegin0 > 120 && ((touchXEnd0 - touchXBegin0) > (touchYBegin0 - touchYEnd0)))
//                    {
//                        //向上滑动
//                        //PicoVRManager.SDK.slideUp = 1;
//                        Vertical_Whole = 1.0f;
//                    }
//                    else if (touchYBegin0 - touchYEnd0 > 120 && ((touchYBegin0 - touchYEnd0) > (touchXEnd0 - touchXBegin0)))
//                    {
//                        //向左滑动
//                        //PicoVRManager.SDK.slideLeft = 1;
//                        Horizontal_Whole = -1.0f;
//                    }
//                }
//            }
//            else if (touchXEnd0 < touchXBegin0)
//            {
//                if (touchYEnd0 > touchYBegin0)
//                {
//                    if (touchXBegin0 - touchXEnd0 > 120 && ((touchXBegin0 - touchXEnd0) > (touchYEnd0 - touchYBegin0)))
//                    {
//                        //向下滑动
//                        //PicoVRManager.SDK.slideDown = 1;
//                        Vertical_Whole = -1.0f;
//                    }
//                    else if (touchYEnd0 - touchYBegin0 > 120 && ((touchYEnd0 - touchYBegin0) > (touchXBegin0 - touchXEnd0)))
//                    {
//                        //向右滑动
//                        //PicoVRManager.SDK.slideRight = 1;
//                        Horizontal_Whole = 1.0f;
//                    }
//                }
//                else
//                {
//                    if (touchXBegin0 - touchXEnd0 > 120 && ((touchXBegin0 - touchXEnd0) > (touchYBegin0 - touchYEnd0)))
//                    {
//                        //向下滑动
//                        //PicoVRManager.SDK.slideDown = 1;
//                        Vertical_Whole = -1.0f;
//                    }
//                    else if (touchYBegin0 - touchYEnd0 > 120 && ((touchYBegin0 - touchYEnd0) > (touchXBegin0 - touchXEnd0)))
//                    {
//                        //向左滑动
//                        //PicoVRManager.SDK.slideLeft = 1;
//                        Horizontal_Whole = -1.0f;
//                    }
//                }
//            }
//            else
//            {
//                //PicoVRManager.SDK.slideUp = 0;
//                //PicoVRManager.SDK.slideDown = 0;
//                //PicoVRManager.SDK.slideRight = 0;
//                //PicoVRManager.SDK.slideLeft = 0;
//            }
//            touchXEnd0 = 0;
//            touchXBegin0 = 0;
//            touchYBegin0 = 0;
//            touchYEnd0 = 0;
//            lark2touchclock1 = false;
//        }
//    }



//    int diff_X;
//    int diff_Y;

//    /// <summary>
//    /// 滑动算法02（手指在触摸板上移动就算滑动）
//    /// </summary>
//    private void SlideAlgorithm02()
//    {
//        if (Pvr_UnitySDKAPI.TouchPadPosition.x > 0 && Pvr_UnitySDKAPI.TouchPadPosition.y > 0)
//        {
////            Debug.Log(Pvr_UnitySDKAPI.TouchPadPosition.x + "   x---------y    "+ Pvr_UnitySDKAPI.TouchPadPosition.y);
//            if (!lark2touchclock2)
//            {
//                touchXBegin = Pvr_UnitySDKAPI.TouchPadPosition.x;
//                touchYBegin = Pvr_UnitySDKAPI.TouchPadPosition.y;
//                lark2touchclock2 = true;
//                return;
//            }
//            touchYEnd = Pvr_UnitySDKAPI.TouchPadPosition.y;
//            touchXEnd = Pvr_UnitySDKAPI.TouchPadPosition.x;
//            if (lark2touchclock2)
//            {
//                diff_X = touchXEnd - touchXBegin;
//                diff_Y = touchYEnd - touchYBegin;

//                if ((diff_X > 10))
//                {
//                    Vertical = 1.0f;
//                }
//                else if (diff_X < -10)
//                {
//                    Vertical = -1.0f;
//                }

//                if (diff_Y > 10)
//                {
//                    Horizontal = 1.0f;
//                }
//                else if (diff_Y < -10)
//                {
//                    Horizontal = -1.0f;
//                }
//                touchXBegin = touchXEnd;
//                touchYBegin = touchYEnd;
//                #region Test
//                //    if (touchXEnd > touchXBegin)
//                //    {
//                //        if (touchYEnd > touchYBegin)
//                //        {
//                //            if (touchXEnd - touchXBegin > 10 && ((touchXEnd - touchXBegin) > (touchYEnd - touchYBegin)))
//                //            {
//                //                //向上滑动
//                //                //PicoVRManager.SDK.slideUp = 1;
//                //                Vertical = 1.0f;
//                //            }
//                //            else if (touchYEnd - touchYBegin > 10 && ((touchYEnd - touchYBegin) > (touchXEnd - touchXBegin)))
//                //            {
//                //                //向右滑动
//                //                //PicoVRManager.SDK.slideRight = 1;
//                //                Horizontal = 1.0f;
//                //            }
//                //        }
//                //        else
//                //        {
//                //            if (touchXEnd - touchXBegin > 10 && ((touchXEnd - touchXBegin) > (touchYBegin - touchYEnd)))
//                //            {
//                //                //向上滑动
//                //                //PicoVRManager.SDK.slideUp = 1;
//                //                Vertical = 1.0f;
//                //            }
//                //            else if (touchYBegin - touchYEnd > 10 && ((touchYBegin - touchYEnd) > (touchXEnd - touchXBegin)))
//                //            {
//                //                //向左滑动
//                //                //PicoVRManager.SDK.slideLeft = 1;
//                //                Horizontal = -1.0f;
//                //            }
//                //        }
//                //    }
//                //    else
//                //    {
//                //        if (touchYEnd > touchYBegin)
//                //        {
//                //            if (touchXBegin - touchXEnd > 10 && ((touchXBegin - touchXEnd) > (touchYEnd - touchYBegin)))
//                //            {
//                //                //向下滑动
//                //                //PicoVRManager.SDK.slideDown = 1;
//                //                Vertical = -1.0f;
//                //            }
//                //            else if (touchYEnd - touchYBegin > 10 && ((touchYEnd - touchYBegin) > (touchXBegin - touchXEnd)))
//                //            {
//                //                //向右滑动
//                //                //PicoVRManager.SDK.slideRight = 1;
//                //                Horizontal = 1.0f;
//                //            }
//                //        }
//                //        else
//                //        {
//                //            if (touchXBegin - touchXEnd > 10 && ((touchXBegin - touchXEnd) > (touchYBegin - touchYEnd)))
//                //            {
//                //                //向下滑动
//                //                //PicoVRManager.SDK.slideDown = 1;
//                //                Vertical = -1.0f;
//                //            }
//                //            else if (touchYBegin - touchYEnd > 10 && ((touchYBegin - touchYEnd) > (touchXBegin - touchXEnd)))
//                //            {
//                //                //向左滑动
//                //                //PicoVRManager.SDK.slideLeft = 1;
//                //                Horizontal = -1.0f;
//                //            }
//                //        }
//                //    }
//                //}
//                #endregion
//            }
            
//        }
//        else
//        {
//            touchXEnd = 0;
//            touchXBegin = 0;
//            touchYBegin = 0;
//            touchYEnd = 0;
//            lark2touchclock2 = false;
//        }
//    }
}

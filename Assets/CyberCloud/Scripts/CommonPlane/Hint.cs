using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 弱提示
/// </summary>
public class Hint : MonoBehaviour
{
    /// <summary>
    /// 默认显示3秒
    /// </summary>
    [SerializeField]
    private int showTime = 3;
    [SerializeField]
    private UILabel lable;

    public float initY;
    // Use this for initialization
    void Awake()
    {
        
    }
    void Start () {

	}
	
	// Update is called once per frame
	void Update () {
        if (startCountdown) {
            CountdownTime += Time.deltaTime;
            if (CountdownTime >= showTime)
            {
                this.gameObject.SetActive(false);
                startCountdown = false;
            }
        }
       
	}
    /// <summary>
    /// 开启倒计时
    /// </summary>
    private bool startCountdown = false;
    private float CountdownTime = 0;

 
    /// <summary>
    /// 弱提示内容
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="showtime">显示时间 -1时一直显示</param>
    public void hintMsg(string msg,int showtime) {
        lable.text = msg;
        CountdownTime = 0;
        if (showtime > 0)
        {
            MyTools.PrintDebugLog("ucvr =================================== hintstart");
            startCountdown = true;
            showTime = showtime;
        }
        else {
            startCountdown = false;
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 游戏中或游戏启动中显示的碰撞器防止游戏启动中的误点
/// </summary>
public class GameOutBoxCollider : MonoBehaviour {
    public GameObject collider;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        ///只需要在启动过程中展示碰撞器
        if (GameAppControl.getGameRuning()&&DialogBase.isShow==false&&GameAppControl.getGameStarted()==false)
        {
            if (collider.activeSelf == false) {
                collider.SetActive(true);
            }
        }
        else {
            if (collider.activeSelf == true)
            {
                collider.SetActive(false);
            }
        }
	}
}

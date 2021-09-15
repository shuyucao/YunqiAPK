using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Manager;
using Assets.Scripts.Windows;
using UnityEngine.UI;
using Assets.Scripts.Data;
using Assets.Scripts.Tool;

public class ManagerController : MonoBehaviour
{
    public GameObject LeftCotroller;
    public GameObject RightController;
    public GameObject LeftCotrollerTips;
    public GameObject RightControllerTips;
    public GameObject LeftControllerRay;
    public GameObject RightControllerRay;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ReadAndWriteConfigManager.LoadConfig());
        StartCoroutine(LoadLoactinAreaManager.RequestLoactionArea());
        WindowManager.Init();
        WindowManager.Open<LoadingWindow>();
        //初始化手柄绑定
        PVRControllerManager.Instance.InitController(LeftCotroller, RightController, LeftCotrollerTips,RightControllerTips, LeftControllerRay, RightControllerRay);
    }
}

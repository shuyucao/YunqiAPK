using CvrDependentApi;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 将脚本绑定到场景对象上，将手柄和手柄方向点复制给属性 可以通过此脚本检测手柄碰撞
/// </summary>
public class Pvr_RayManager : MonoBehaviour {

    public GameObject leftdot;
    public GameObject leftmodel;
    public GameObject rightdot;
    public GameObject rightmodel;
    public GameObject controller;
    private Ray ray;
    public static RaycastHit rayhit;

	// Use this for initialization
	void Start ()
	{
	    ray = new Ray();
	    rayhit = new RaycastHit();


    }
    //Pvr_ControllerInit[] controllerchild;
    public  GameObject[] childActive(GameObject obj,bool active)
    {
        int count = obj.transform.childCount;
        GameObject[] backObjs = new GameObject[count];
        for (int i = 0; i < count; i++)
        {
            backObjs[i] = obj.transform.GetChild(i).gameObject;
            backObjs[i].SetActive(active);
        }
        return backObjs;
    }
   
    // Update is called once per frame
    void Update () {
       // Debug.Log("ucvr openCursor ControllerManager.showController:"+ ControllerManager.showController);
       // controllerchild = controller.GetComponentsInChildren<Pvr_ControllerInit>();
        if (ControllerManager.showController)
        {
            //controller.SetActive(true);
            controller.gameObject.transform.localScale = new Vector3(1,1,1);

            //childActive(controller, true);
            //Debug.Log("ucvr openCursor ================" );
            //childActive(gameObject,true);
        }
        else {
            controller.gameObject.transform.localScale = new Vector3(0, 0, 0);
            //controller.SetActive(false);
            // Pvr_ControllerInit controller33 = controller.GetComponent<Pvr_ControllerInit>();
            // if(controller33!=null)
            // controller33.controller4.SetActive(false);
        
            //childActive(controller, false);
            // childActive(gameObject, false);
        }
        if (leftdot == null || rightdot == null)
        {
            //Debug.LogError("====no control ray=====");
            // GameObject g = controllerPrefab.gameObject;
            // UniversalControllerActions u = g.GetComponentInChildren<UniversalControllerActions>();
           // WaveVR_ControllerPointer leftc = leftmodel.GetComponentInChildren<WaveVR_ControllerPointer>();
           // WaveVR_ControllerPointer rightc = rightmodel.GetComponentInChildren<WaveVR_ControllerPointer>();
           // leftdot = leftc != null?  leftc.gameObject:null;
           // rightdot = rightc!=null? rightc.gameObject:null;
            return;
        }
	    if (leftdot.activeSelf && !rightdot.activeSelf)
	    {
	        ray.direction = leftdot.transform.position - leftmodel.transform.position;
	        ray.origin = leftmodel.transform.position;
	    }
	    if (!leftdot.activeSelf && rightdot.activeSelf)
	    {
	        ray.direction = rightdot.transform.position - rightmodel.transform.position;
	        ray.origin = rightmodel.transform.position;
        }
        Debug.DrawRay(ray.origin, ray.direction, Color.red, 100);
        RaycastHit hit;
	    if (Physics.Raycast(ray, out hit))
	    {
            //Debug.LogError("ucvr Raycast           ***********************");
            //Debug.LogError(" ucvr getRaycastHit true========================================");
            rayhit = hit;
	    } else
            rayhit = new RaycastHit(); ;
		
	}
}

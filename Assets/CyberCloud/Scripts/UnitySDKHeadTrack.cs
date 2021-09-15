///////////////////////////////////////////////////////////////////////////////

///////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;

public class UnitySDKHeadTrack : MonoBehaviour
{

    public bool trackRotation = true;

    public bool trackPosition = true;

    public Transform target;  



    private Vector3 startPosition;

    private Quaternion startQuaternion;
    private bool dataClock;

    public void Awake()
    {
        if (target == null)
        {
            startPosition = transform.localPosition;
            startQuaternion = transform.localRotation;
        }
        else
        {
            startPosition = transform.position;
            startQuaternion = transform.rotation;
        }
        //Pvr_UnitySDKAPI.Sensor.UPvr_InitPsensor();

    }

    public Ray Gaze
    {
        get
        {
            UpdateHead();
            return new Ray(transform.position, transform.forward);
        }
    }

    void Update()
    {
        #if UNITY_ANDROID && !UNITY_EDITOR
            UpdateHead();
        #endif
    }
    public Quaternion rtest = Quaternion.identity;
    private void UpdateHead()
    {
        if (!CyberCloudConfig.NativeShowScreen)
        {
            return;
        }
        
        float[] r = MyTools.getDevicePose(0, 0);
        if (trackRotation)
        {
           // 
            //Quaternion rotate = new Quaternion(r[0], r[1],- r[2], -r[3]); ;
            Quaternion rotate = rtest;// new Quaternion(0.2f, 0,0, 1); ;

            if (target == null)
            {

                transform.localRotation = rotate;
            }
            else
            {
               // transform.rotation = rotate * target.rotation;
            }
        }

        else
        {
       
               transform.localRotation = Quaternion.identity;
       
        }
        if (trackPosition)
        {
            Vector3 pos = new Vector3(r[4], r[5], -r[6]); ;

            if (target == null)
            {
                transform.localPosition = pos;
            }
            else
            {
                //transform.position = target.position + target.rotation * pos;
            }
        }
    }

}

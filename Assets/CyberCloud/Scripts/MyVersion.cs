using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyVersion : MonoBehaviour {
  
    // Use this for initialization
    void Start () {
        string ucvrVersionCode = MyTools.getVersionCode();

        //XMPP_Cyber_portal _PicoNeo.时间. V.xxx
    
        if (CyberCloudConfig.currentType == CyberCloudConfig.DeviceTypes.Pico)
            MyTools.PrintDebugLog("ucvr XMPP_Cyber_portal_PicoNeo"+ ".V." + Application.version);
        else if (CyberCloudConfig.currentType == CyberCloudConfig.DeviceTypes.DaPeng)
            MyTools.PrintDebugLog("ucvr XMPP_Cyber_portal_DPM2Pro"  + ".V." + Application.version);
        else
            MyTools.PrintDebugLog("ucvr XMPP_Cyber_portal_"+CyberCloudConfig.currentType + ".V." + Application.version);

    }

    // Update is called once per frame
    void Update () {
		
	}
}

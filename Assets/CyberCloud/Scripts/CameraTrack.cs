using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTrack : MonoBehaviour {
    public Quaternion Orientation { get; protected set; }
    private float mouseX = 0;
    private float mouseY = 0;
    private float mouseZ = 0;
    private bool autoUntiltHead = false;
    private float neckModelScale = 0;
    private static readonly Vector3 neckOffset = new Vector3(0, 0.075f, 0.0805f);
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        #if UNITY_EDITOR
            UpdateSimulatedSensor();
            transform.localRotation = Orientation;
        #endif
    }
    private void UpdateSimulatedSensor()
    {
        bool rolled = false;
        if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
        {
            mouseX += Input.GetAxis("Mouse X") * 5;
            if (mouseX <= -180)
            {
                mouseX += 360;
            }
            else if (mouseX > 180)
            {
                mouseX -= 360;
            }
            mouseY -= Input.GetAxis("Mouse Y") * 2.4f;
            mouseY = Mathf.Clamp(mouseY, -91, 91);
        }
        else if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            rolled = true;
            mouseZ += Input.GetAxis("Mouse X") * 5;
            mouseZ = Mathf.Clamp(mouseZ, -91, 91);
        }
        if (!rolled && autoUntiltHead)
        {
            mouseZ = Mathf.Lerp(mouseZ, 0, Time.deltaTime / (Time.deltaTime + 0.1f));
        }
        var rot = Quaternion.Euler(mouseY, mouseX, mouseZ);
        var neck = (rot * neckOffset - neckOffset.y * Vector3.up) * neckModelScale;
        Matrix4x4 Matrix1 = Matrix4x4.TRS(neck, rot, Vector3.one);
        Orientation = Quaternion.LookRotation(Matrix1.GetColumn(2), Matrix1.GetColumn(1));
        // Pvr_UnitySDKManager.SDK.HeadPose = new Pvr_UnitySDKPose(Matrix1);

    }
}

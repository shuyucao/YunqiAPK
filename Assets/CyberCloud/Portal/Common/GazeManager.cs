using UnityEngine;
using System.Collections;

public class GazeManager : MonoBehaviour {

	public static GazeManager Instance { get; private set; }
	public UISprite GazeSprite;
	public float DelayTime=2.0f;
	public UICamera uiCamera;
	public GameObject HoverGameObject;
	private bool ShowGaze=false;
	private float startTime;

	public bool IsGazeUsing=true;

	void Awake ()
	{
		if (Instance != null && Instance != this) {
			Destroy (gameObject);
		}
		Instance = this;
	}

	void Start()
	{
		bool isConnectBluetooth = false;
		//PicoUnityActivity.CallObjectMethod<bool> (ref isConnectBluetooth, "hasConnectedInputBluetoothDevice");
		//Debug.Log ("[Oscar] The buletooth form android is "+isConnectBluetooth.ToString());
		
        ChangeGazeStatus (!isConnectBluetooth);
	}

	public void ChangeGazeStatus(bool isShow)
	{
		if (uiCamera != null) {
			IsGazeUsing=isShow;
			uiCamera.showTooltips=isShow;
		} else {
			Debug.Log("[Oscar] Not find the UI Camera");
		}
	}

	// Update is called once per frame
	void Update () {
	
		if (ShowGaze) {
		
			if(Time.time-startTime>DelayTime)
			{
				//exexute onclick event
				if(HoverGameObject!=null)
				{
					GazeSprite.fillAmount=0;
					ShowGaze=false;
                    MyTools.PrintDebugLog("ucvr onclick --1");
					UICamera.Notify(HoverGameObject, "OnClick", null);
					UICamera.Notify(HoverGameObject,"OnPress",true);
				}
			}
			else
			{
				float scale=(Time.time-startTime)/DelayTime;
				GazeSprite.fillAmount=scale;
			}
		}
	}

	public void Show(bool isShow,GameObject hoverGameObect)
	{
		ShowGaze = isShow;
		GazeSprite.fillAmount = 0.0f;
		HoverGameObject = hoverGameObect;
		TweenAlpha.Begin (this.gameObject,0.1f,isShow?1.0f:0.0f);
		if(isShow)
		{
			startTime=Time.time;
		}
	}
}

using UnityEngine;
using System.Collections;
using DG.Tweening;

public class LauncherButton : MonoBehaviour {

	public GameObject homeBtn;

	private Tween highlightRotateTween,highlightFadeTween;
    private Tweener homeDis;

    public GameObject highlight;
	public GameObject text;
	private Renderer renderer;
	private Transform transform;
	private Vector3 highlightResetLocalPosition;

    //public string gotoScreen;
    public EWingScreen gotoScreen;

	void Awake () {
		transform = highlight.transform;
		renderer = highlight.GetComponent<Renderer> ();
		highlightResetLocalPosition = transform.localPosition;
		UIEventListener.Get(this.gameObject).onHover=onHover;
		UIEventListener.Get(this.gameObject).onPress=onPress;
		UIEventListener.Get (this.gameObject).onClick = onClick;
	}

	 void onHover(GameObject obj,bool isHover)
	{
		if (isHover) {
			transform.localPosition = highlightResetLocalPosition;
			HideOrDisplayHightLigth(true);
			HideOrDisplayText(true);
			Quaternion targetRotation = transform.localRotation * Quaternion.Euler (-40.0f, -30.0f, 0.0f);
			highlightRotateTween = transform.DOLocalRotate (targetRotation.eulerAngles, 1, RotateMode.Fast).SetEase (Ease.OutExpo);
		} else {
            if (WingScreenManager.Instance.uiCamera.eventReceiverMask != 0)
            {
                HideOrDisplayHightLigth(false);
                HideOrDisplayText(false);
            }
        }
	}

    void OnEnable()
    {
        homeDis = homeBtn.transform.DOLocalMoveZ(0f, 1.0f).SetEase(Ease.InCubic);
    }

    void onPress(GameObject obj,bool isPress)
	{
		if (isPress) {
		} else {
            if (WingScreenManager.Instance.uiCamera.eventReceiverMask != 0)
            {
                HideOrDisplayHightLigth(false);
                HideOrDisplayText(false);
            }
        }
	}

	public void HideOrDisplayHightLigth(bool isDisplay)
	{
		float alpha = isDisplay ? 0.5f : 0.0f;
		highlightFadeTween =  renderer.material.DOFade (alpha, 0.5f);
	}

	public void HideOrDisplayText(bool isDisplay)
	{
		float alpha = isDisplay ? 1.0f : 0.0f;

		TweenAlpha.Begin (text,0.0f,alpha);
	}

	void onClick(GameObject obj)
	{
        //UIManager.Instance.uiCamera.eventReceiverMask = 0;
        //GameObject obj1 = GameObject.Find("UI Root/LauncherScreen");
        //TweenAlpha.Begin(obj1, 1.0f, 0.0f);
        Fall();

	}
    public bool istransition = false;
	void Fall()
	{
        if (istransition) return;
		HideOrDisplayHightLigth(false);
        istransition = true;
        homeDis = homeBtn.transform.DOLocalMoveZ(1200,1.0f).SetEase(Ease.InCubic).OnComplete(OnDone);
		//if (gotoScreen == EWingScreen.game) {
		//	CameraFader.Instance.FadeOut (1.0f);
		//}
	}


    
	void OnDone()
	{
        //backgroundManager.StartRippleTransition (gotoScreen);
        //DoTransition (gotoScreen);
        DoTransition(gotoScreen);
        istransition = false;
    }


    
    
    void DoTransition(EWingScreen gotoScreen){

        //这个文件没用到 EWingScreen的值改了所以注掉这个函数
        /*
        if (gotoScreen == EWingScreen.none)
        {
            Debug.LogError("goscreen is none");
            return;
        }
		if (gotoScreen == EWingScreen.game) {
            Application.LoadLevelAsync("Games");
            return;
		}
        WingScreenManager.Instance.PushScreen(gotoScreen);

        //WingScreenManager.Instance.ReplaceScreen(EWingScreen.userinfo);
        //WingScreenManager.Instance.ReplaceScreen();
        //WingScreenManager.Instance.PopScreen()

        //WingSceneManager.Instance.CurrentScene = WingScene.Player;

        //switch (gotoScreen) {

        //case WingScreenNames.MOVIE:
        //              //Application.LoadLevelAsync("Movies");
        //              WingScreenManager.Instance.PushScreen(prefabName);
        //              break;
        //case WingScreenNames.GAME_SCREEN:
        //              //Application.LoadLevelAsync("Games");
        //              WingScreenManager.Instance.PushScreen(prefabName);

        //              break;
        //case WingScreenNames.PROFILE_SCREEN:
        //              WingScreenManager.Instance.PushScreen("TestScreen.prefab");
        //              //UIManager.Instance.GoToScreen (gotoScreen, true);
        //              break;
        //}
         */
    }



    private IEnumerator StartLoading(string sceneName) {
		int displayProgress = 0;
		int toProgress = 0;
		AsyncOperation op = Application.LoadLevelAsync(sceneName);
		op.allowSceneActivation = false;
		while(op.progress < 0.9f) {
			toProgress = (int)op.progress * 100;
			while(displayProgress < toProgress) {
				++displayProgress;
//				uiSprite.fillAmount=displayProgress/100.0f;
//				uiLable.text="1234";
//				Debug.Log(uiLable.text);
//				uiSlider.value=displayProgress/100.0f;
//				
				
				yield return new WaitForEndOfFrame();
			}
		}
		
		toProgress = 100;
		while(displayProgress < toProgress){
			++displayProgress;
//			uiSlider.value=displayProgress/100.0f;
//			uiSprite.fillAmount=displayProgress/100.0f;
//			uiLable.text=displayProgress.ToString();
			yield return new WaitForEndOfFrame();
		}
		op.allowSceneActivation = true;
	}
}

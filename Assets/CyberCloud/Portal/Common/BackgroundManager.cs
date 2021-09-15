using UnityEngine;
using System.Collections;
using DG.Tweening;

public class BackgroundManager : MonoBehaviour {

	private const float TOOLTIP_FADEIN_DURATION = 0.8f;
	private const float TOOLTIP_FADEOUT_DURATION = 0.4f;

	public Material meshMaterial;
	public Renderer meshRenderer;
	public Transform sphereMesh;

	public float toolTipOffset = 0.5f;
	
	//private MainMenuButton curBut;
	public Texture2D logoTexture;
	public float tooltipScale = 1.6f;
	public float logoScale = 6f;

	public float rippleAnimTime = 0;

	//public Crosshair crossHair;

	private Tweener animTimeTween;
	private Tweener tooltipFadeTween;
	private Tweener flattenScreenTween, snapTween, darkenRadiusTween, darkenAmountTween;
	private Tweener colorFadeTween;

	public Transform rippleCentre;
	private const float RIPPLE_END_TIME = 25f;
	private const float RIPPLE_DURATION = 6f;
	private const float RIPPLE_DELAY = 0.8f;
	private const float TRANSITION_DELAY = 1.8f;
	
	private Sequence videoIntroTransition, videoOutroTransition;
	
	void Start () {

		ResetMaterial ();

		//VideoPanel.OnStartVideoIntroTransition += HandleOnStartVideoIntroTransition;
		//VideoPanel.OnStartVideoOutroTransition += HandleOnStartVideoOutroTransition;

		videoIntroTransition = DOTween.Sequence();
		videoOutroTransition = DOTween.Sequence();

		// sequence, property, delay, end value, duration, ease	 
		AddToSequence (videoIntroTransition, "_FlattenAmount",	0.7f, 1.0f, 1.0f, Ease.InQuad);
		AddToSequence (videoIntroTransition, "_DarkenRadius",	0.7f, 7.0f, 3.0f, Ease.InQuad);
		AddToSequence (videoIntroTransition, "_SnapAmount",		1.2f, 1.0f, 0.5f, Ease.InQuad);
		AddToSequence (videoIntroTransition, "_DarkenAmount",	2.0f, 0.0f, 1.0f, Ease.InQuad);
		AddToSequence (videoIntroTransition, "_ColorFade",		5.0f, 0.0f, 3.0f, Ease.InOutQuad);

		AddToSequence (videoOutroTransition, "_ColorFade",		0.0f, 1.0f, 1.5f, Ease.InOutQuad);
		AddToSequence (videoOutroTransition, "_FlattenAmount",	2.0f, 0.0f, 1.0f, Ease.InQuad);
		AddToSequence (videoOutroTransition, "_DarkenRadius",	2.0f, 0.0f, 0.5f, Ease.InQuad);
		AddToSequence (videoOutroTransition, "_SnapAmount",		2.0f, 0.0f, 1.0f, Ease.InQuad);
		AddToSequence (videoOutroTransition, "_DarkenAmount",	2.0f, 0.0f, 1.0f, Ease.InQuad);

		videoIntroTransition.SetAutoKill(false);
		videoOutroTransition.SetAutoKill(false);

		videoIntroTransition.Pause ();
		videoOutroTransition.Pause ();

		videoIntroTransition.OnComplete(IntroDone);

		videoOutroTransition.OnComplete(OutroDone);
	}

	void AddToSequence(Sequence seq, string property, float delay, float endValue, float duration, Ease ease){
		seq.Insert(delay, meshMaterial.DOFloat(endValue, property, duration).SetEase(ease));
	}

	void OnDestroy(){
		ResetMaterial();
	}

	void ResetMaterial(){
		meshMaterial.SetFloat("_FlattenAmount", 0);
		meshMaterial.SetFloat("_SnapAmount", 0);
		meshMaterial.SetFloat("_ColorFade", 1);
		meshMaterial.SetFloat("_DarkenRadius", 0);
		meshMaterial.SetFloat("_DarkenAmount", 0.3f);
	}

	void HandleOnStartVideoIntroTransition ()
	{
		ResetMaterial();
		//crossHair.Hide();
		videoIntroTransition.Restart();
	}

	void HandleOnStartVideoOutroTransition ()
	{
		meshRenderer.enabled = true;
		if (videoIntroTransition.IsPlaying()) {
			videoIntroTransition.Pause();
		}
		videoOutroTransition.Restart();
	}

	void IntroDone(){
		meshRenderer.enabled = false;
	}

	void OutroDone(){
		//crossHair.Show();
	}

	void Update () {
		Vector3 rippleCentrePos = sphereMesh.InverseTransformPoint(rippleCentre.position);
		Vector4 rippleData = new Vector4(rippleCentrePos.x, rippleCentrePos.y, rippleCentrePos.z, rippleAnimTime);
		
		meshMaterial.SetVector("_RippleData", rippleData);
	}

	private void MoveRippleAnimTime(float newTime, float duration, float delay){
		if (animTimeTween != null)
			animTimeTween.Kill();
		
		animTimeTween = DOTween.To (SetRippleAnimTime, 0, newTime, duration).SetDelay (delay);
	}
	
	private void SetRippleAnimTime(float newAnimTime){
		rippleAnimTime = newAnimTime;
	}

	void FadeoutDone(){
		meshMaterial.SetTexture("_MainTex", logoTexture);
		meshMaterial.SetFloat("_AspectRatio", logoTexture.width / logoTexture.height);
		meshMaterial.SetVector("_ProjectionCentre", new Vector4(0,0,0,-1));
		meshMaterial.SetFloat("_ProjectionAlpha", 1);
		meshMaterial.SetFloat("_Scale", logoScale);
	}

	public void ShowTooltip(Texture2D tooltipTexture){
		meshMaterial.SetFloat ("_ProjectionAlpha", 0);
		meshMaterial.SetTexture ("_MainTex", tooltipTexture);
		meshMaterial.SetFloat ("_AspectRatio", tooltipTexture.width / tooltipTexture.height);
		meshMaterial.SetVector ("_ProjectionCentre", new Vector4 (0, toolTipOffset, 0, 1));
		meshMaterial.SetFloat ("_Scale", tooltipScale);

		if (tooltipFadeTween != null)
			tooltipFadeTween.Kill();

		tooltipFadeTween = meshMaterial.DOFloat (1.0f, "_ProjectionAlpha", TOOLTIP_FADEIN_DURATION);
	}

	public void HideTooltip(){
		if (tooltipFadeTween != null)
			tooltipFadeTween.Kill();

		tooltipFadeTween = meshMaterial.DOFloat(0f, "_ProjectionAlpha", TOOLTIP_FADEOUT_DURATION).OnComplete(FadeoutDone);
	}


    //16.06.20 no use
//    public void StartRippleTransition(PicoScreen gotoScreen){
//        //MoveRippleAnimTime(RIPPLE_END_TIME, RIPPLE_DURATION, RIPPLE_DELAY);
////		Invoke("DoTransition", TRANSITION_DELAY);
////		MoveRippleAnimTime(RIPPLE_END_TIME, RIPPLE_DURATION, 0.0f);
//        //Invoke("DoTransition",gotoScreen,0.0f);
//        DoTransition (gotoScreen);
//    }

//    void DoTransition(PicoScreen gotoScreen){

//        switch (gotoScreen) {
//        case PicoScreen.MovieChooserScreen:
//            Application.LoadLevel ("Movies");
//            break;
//        case PicoScreen.GamesScreen:
//            Application.LoadLevel ("Games");
//            break;
//        case PicoScreen.ProfileScreen:
//            UIManager.Instance.GoToScreen (gotoScreen, true);
//            break;
//        }
//    }
}

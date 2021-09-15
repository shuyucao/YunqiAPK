using UnityEngine;
using System.Collections;
using DG.Tweening;

public class CameraFader : MonoBehaviour {

    public static CameraFader Instance { get; private set; }
    public static event System.Action FadeInDone, FadeOutDone;

    public float StartAlpha = 1.0f;
    public Color FadeColor;

    private Material mat;
    private Tweener fadeTween;

    void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
        }
        Instance = this;

        mat = GetComponent<Renderer>().material;
        Color newColor = FadeColor;
        newColor.a = StartAlpha;
        mat.color = newColor;
    }

    public void FadeTo(float newAlpha, float seconds){
		if (fadeTween != null)
			fadeTween.Kill ();

		fadeTween = mat.DOFade(newAlpha, seconds);
	}
	
	public void FadeIn(float seconds){
		GetComponent<Renderer>().enabled = true;

		if (fadeTween != null)
			fadeTween.Kill ();

		fadeTween = mat.DOFade(0, seconds).OnComplete(NotifyFadeInDone);
	}
	
	public void FadeOut(float seconds){
		GetComponent<Renderer>().enabled = true;

		if (fadeTween != null)
			fadeTween.Kill ();

		fadeTween = mat.DOFade(1, seconds).OnComplete(NotifyFadeOutDone);
	}
    public void FadeTo(float startValue, float endValue, float seconds)
    {
        GetComponent<Renderer>().enabled = true;
        if (fadeTween != null)
            fadeTween.Kill();
        Color color = mat.color;
        color.a = startValue;
        mat.color = color;
        fadeTween = mat.DOFade(endValue, seconds).OnComplete(NotifyFadeOutDone);
    }
    public void NotifyFadeInDone(){
		if (FadeInDone != null){
			FadeInDone();
		}
		GetComponent<Renderer>().enabled = false;
	}
	
	public void NotifyFadeOutDone(){
		if (FadeOutDone != null){
			FadeOutDone();
		}
	}
	
}

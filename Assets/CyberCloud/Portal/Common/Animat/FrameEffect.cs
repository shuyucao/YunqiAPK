using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FrameEffect : MonoBehaviour {
    [SerializeField]
    private UISprite sprite;

    private UISprite sprite0;

    private UISprite sprite1;

    Animation animat;


    private List<UISprite> spList;

    // Use this for initialization
    void Start ()
    {
        sprite.gameObject.SetActive(false);
        spList = new List<UISprite>();

        if (spList != null && spList.Count > 0)
        {
            sprite0 = spList[0];
            spList.RemoveAt(0);
        }
        else
            sprite0 = NGUITools.AddChild(gameObject, sprite.gameObject).GetComponent<UISprite>();
        sprite0.gameObject.SetActive(true);
        TweenAlpha tween = TweenAlpha.Begin(sprite0.gameObject, 0.8f, 0f);
        tween.delay = 0.5f;
        tween.SetOnFinished(() =>
        {
            spList.Add(sprite0);
        });
        TweenScale.Begin(sprite0.gameObject, 0.8f, Vector3.one * 1.1f).delay = 0.5f;

        StartCoroutine(CreatObj());
    }


     IEnumerator CreatObj()
    {
        yield return new WaitForSeconds(0.5f);
        UISprite sp;
        if(spList != null && spList.Count > 0)
        {
            sp = spList[0];
            spList.RemoveAt(0);
        }
        else
            sp = NGUITools.AddChild(gameObject, sprite.gameObject).GetComponent<UISprite>();
        sp.gameObject.SetActive(true);
        sp.transform.localScale = Vector3.one;
        sp.alpha = 1.0f;
        TweenAlpha tween = TweenAlpha.Begin(sp.gameObject, 0.8f, 0f);
        tween.delay = 0.5f;
        Debug.Log("XXXXXXXXXXXXXX");
        tween.SetOnFinished(() =>
        {
            spList.Add(sp);
            StartCoroutine(CreatObj());
        });
        TweenScale.Begin(sp.gameObject, 0.8f, Vector3.one * 1.1f).delay = 0.5f;
    }


}

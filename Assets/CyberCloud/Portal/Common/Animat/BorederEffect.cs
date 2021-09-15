using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BorederEffect : MonoBehaviour
{
    public List<GameObject> effectList;

    public List<Animator> effectPool;


	void Start () {
	
	}


    void Destroy()
    {
        
    }


    public void Add(GameObject effect)
    {
        if(effectList == null)
        {
            effectList = new List<GameObject>();
        }
        effectList.Add(effect);

        if(effectList.Count > 3)
        {
            int deleteCount = effectList.Count - 3;
           for(int i = 0;i < deleteCount; i++)
            {
                GameObject obj = effectList[0];
                effectList.Remove(obj);
                Destroy(obj);
            }
        }
    }


    public void AddPool(Animator effect)
    {
        if(effectPool == null)
        {
            effectPool = new List<Animator>();
        }
        effect.gameObject.SetActive(false);
        effect.gameObject.transform.localScale = Vector3.one;
        effect.GetComponent<UISprite>().alpha = 1.0f;
        if(!effectPool.Contains(effect)) effectPool.Add(effect);
    }


    public Animator UseEffect()
    {
        Animator effect = null;
        if(effectPool != null && effectPool.Count > 0)
        {
            effect = effectPool[0];
            effectPool.RemoveAt(0);
        }
        return effect;
    }




    public void Remove(GameObject effect)
    {
        if(effectList != null && effectList.Contains(effect))
        {
            effectList.Remove(effect);
        }
    }


    void OnEnable()
    {
        Animator effect = UseEffect();
        if(effect != null)
        {
            effect.gameObject.SetActive(true);
            effect.Play("borderEffect");
        }
    }



}

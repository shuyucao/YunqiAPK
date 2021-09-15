using UnityEngine;
using System.Collections;

public class BorderEffect01 : MonoBehaviour {

    private GameObject parent;
    BorederEffect borederEffect;

    void Start()
    {
        parent = this.transform.parent.gameObject;
        borederEffect = parent.GetComponent<BorederEffect>();
    }
    void Generate()
    {
        Animator effect = borederEffect.UseEffect();
        if(effect == null)
        {
            GameObject go = NGUITools.AddChild(parent, this.gameObject);
            go.transform.localPosition = Vector3.zero;
        }
        else
        {
            effect.gameObject.SetActive(true);
            effect.Play("borderEffect");
        }
        //if(borederEffect != null) borederEffect.Add(go);
    }

    public void DestroySelf()
    {
        borederEffect.AddPool(gameObject.GetComponent<Animator>());

        //borederEffect.Remove(gameObject);
        //Destroy(this.gameObject);
    }


    void OnDisable()
    {
        if(borederEffect != null) borederEffect.AddPool(gameObject.GetComponent<Animator>());
        //gameObject.GetComponent<Animator>().Stop();
    }


}

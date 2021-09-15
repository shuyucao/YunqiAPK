using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class exitEffect : MonoBehaviour {
    private Animation animation;
    public int maxShowTime = 3;
   
    // Use this for initialization
    private float t;
	void Start () {
       
        animation = this.gameObject.GetComponent<Animation>();
        t = 0;
        isPlaying = true;

    }
    public bool isPlaying = false;
    public void play() {
        isPlaying = true;
        t = 0;
        this.gameObject.SetActive(true);
    }
        // Update is called once per frame
    void Update () {
      //  this.transform.position =new Vector3( eixtBt.transform.position.x, this.transform.position.y, this.transform.position.z);
        t = t + Time.deltaTime;
        if (t > 3) {
            if (animation!=null)
                animation.Stop();
            this.gameObject.SetActive(false);
            isPlaying = false;
        }

	}
}

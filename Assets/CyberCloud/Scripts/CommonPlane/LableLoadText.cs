using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LableLoadText : MonoBehaviour {

    private UILabel lb;
    public string key;
    // Use this for initialization
    void Awake()
    {
        lb = this.GetComponent<UILabel>();
        string msg = Localization.Get(key);
        lb.text = msg;
    }
    void Start () {
    
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class myTest : MonoBehaviour {
    [SerializeField]
    private GameObject tagart;

    private Vector3 initP;
	// Use this for initialization
	void Start () {
        initP = this.transform.position;

    }
	
	// Update is called once per frame
	void Update () {
        this.transform.position = initP + tagart.transform.position;

    }
}

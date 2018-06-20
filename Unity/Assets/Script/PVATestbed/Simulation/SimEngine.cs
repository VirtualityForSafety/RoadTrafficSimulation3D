using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimEngine : MonoBehaviour {
    public float timeFactor = 1.0f;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Time.timeScale = SCPAR.SIM.PVATestbed.SimParameter.timeFactor;
        Time.fixedDeltaTime = 0.02F * Time.timeScale;
    }
}

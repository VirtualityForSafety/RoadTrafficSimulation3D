using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SCPAR.SIM.DataLogging;
/*
 * frame capturer랑 yolowrapperClass를 넣자
 * */
public class IntegrationClass : MonoBehaviour {

    //frame capture class
    [SerializeField]
    FrameCapturer frameCaptureCalss;

    //yolo c++ dll wrapper class
    [SerializeField]
    YoloWrapperClass yoloClass;

    //camera...
    [SerializeField]
    private Camera observationCamera;

    public void yoloTest()
    {
        int width, height;
        byte[] camData = frameCaptureCalss.CaptureSingleFrameRawImage(observationCamera, out width, out height);

        
        
        boundBox[] bboxList = yoloClass.detectNTrackResWithOneFrameWrapper(camData, width, height);

        
        Debug.Log("total number of object detected: " + bboxList.Length);

        for (int i = 0; i < bboxList.Length; i++)
        {
            Debug.Log("For " + i + "th object,");
            Debug.Log("For x element: " + bboxList[i].x);
            Debug.Log("For y element: " + bboxList[i].y);
            Debug.Log("For w element: " + bboxList[i].w);
            Debug.Log("For h element: " + bboxList[i].h);
            Debug.Log("For track_id element: " + bboxList[i].track_id);
            Debug.Log("For obj_id element: " + bboxList[i].obj_id);
        }
        
    }


	// Use this for initialization
	void Start () {

        
        
        yoloTest();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

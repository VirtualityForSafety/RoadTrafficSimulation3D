using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;





//c++에서의 bbox_t struct를 c#에서 마샬링하는거임
[StructLayout(LayoutKind.Sequential)]
public struct boundBox
{
    public int x,y,w,h;

    public float prob;

    public int obj_id;
    public int track_id;
};
public class YoloWrapperClass : MonoBehaviour {

    //YOLO의 초기화

    //[DllImport("SCPAR_DET_Core",EntryPoint = "?initializeDetectorNTracker@SCPAR_DET@@YAHXZ")]
    [DllImport("SCPAR_DET_Core")]
    public static extern int initializeDetectorNTracker();
    //dll 부를 때 경로 조심하셈
    //*************************************************************************************************************
    //*************************************************************************************************************
    //dll이 있는 곳에서 yolo weight 파일을 부르는게 아니라 unity 프로젝트 본체 폴더로부터 부름(Assets 폴더의 parent)
    //*************************************************************************************************************
    //*************************************************************************************************************
    //*************************************************************************************************************

    //한 개의 camera image frame을 c++로 넘겨주고 bounding box 관련 정보를 받는다
    //[DllImport("SCPAR_DET_Core", EntryPoint = "?DetectNTrackResWithOneFrame@SCPAR_DET@@YA_NPEADHHPEAPEAUboundBox@1@PEAH@Z", CallingConvention = CallingConvention.Cdecl)]
    [DllImport("SCPAR_DET_Core")]
    public static extern bool DetectNTrackResWithOneFrame(byte[] imageData, int width, int height, out System.IntPtr bbox, out int bBoxLength);

    //여러 개의 camera image frame을 c++로 넘겨주고 bouding box 관련 정보를 받는다
    //[DllImport("SCPAR_DET_Core", EntryPoint = "?DetectNTrackResWithMultiFrame@SCPAR_DET@@YA_NPEADHHHPEAPEAUboundBox@1@PEAH@Z", CallingConvention = CallingConvention.Cdecl)]
    [DllImport("SCPAR_DET_Core")]
    public static extern bool DetectNTrackResWithMultiFrame(byte[] imageData, int width, int height, int frameLength, out System.IntPtr bbox, out int bBoxLength);

    //[DllImport("SCPAR_DET_Core", EntryPoint = "?DetectNTrackResWithMultiFrame@SCPAR_DET@@YA_NPEADHHHPEAPEAUboundBox@1@PEAH@Z", CallingConvention = CallingConvention.Cdecl)]
    [DllImport("SCPAR_DET_Core")]
    public static extern int releaseDetectorNTracker();

    public boundBox[] detectNTrackResWithOneFrameWrapper(byte[] imageData, int width, int height)
    {
        
        initializeDetectorNTracker();
        

        /*
        boundBox[] bbox1;

        System.IntPtr ptrFromOneFrame;

        int bBoxLength;

        bbox1 = new boundBox[1];

        bbox1[0].x = 0;
        bbox1[0].y = 0;
        bbox1[0].w = 0;
        bbox1[0].h = 0;
        bbox1[0].track_id = 0;
        bbox1[0].obj_id = 0;

        
        DetectNTrackResWithOneFrame(imageData,width,height, out ptrFromOneFrame, out bBoxLength);
        
        System.IntPtr p = ptrFromOneFrame;

        bbox1 = new boundBox[bBoxLength];

        for (int i = 0; i < bBoxLength; i++)
        {
            bbox1[i] = (boundBox)Marshal.PtrToStructure(p, typeof(boundBox));

            p = new System.IntPtr(p.ToInt64() + Marshal.SizeOf(typeof(boundBox)));
        }
        */

        boundBox[] bbox1;
        bbox1 = new boundBox[1];

        bbox1[0].x = 0;
        bbox1[0].y = 0;
        bbox1[0].w = 0;
        bbox1[0].h = 0;
        bbox1[0].track_id = 0;
        bbox1[0].obj_id = 0;


        return bbox1;        
    }


	// Use this for initialization
	void Start () {
        
        /*
        Debug.Log("For init yolo, ");
        Debug.Log(initializeDetectorNTracker());

        Debug.Log("For single frame... ");
        boundBox[] bbox1;
        

        System.IntPtr ptrFromOneFrame;

        int bBoxLength;
        

        DetectNTrackResWithOneFrame(new byte[1] { 10 }, 240, 480, out ptrFromOneFrame, out bBoxLength);

        Debug.Log("boundbox length: " + bBoxLength);
        

        System.IntPtr p = ptrFromOneFrame;

        bbox1 = new boundBox[bBoxLength];

        for (int i = 0; i < bBoxLength; i++)
        {
            bbox1[i] = (boundBox)Marshal.PtrToStructure(p, typeof(boundBox));

            p = new System.IntPtr(p.ToInt64() + Marshal.SizeOf(typeof(boundBox)));
        }
                
        

        for (int i=0; i<bBoxLength; i++)
        {
            Debug.Log(i + "th bounding box element: ");
            Debug.Log("x: " + bbox1[i].x);
            Debug.Log("y: " + bbox1[i].y);
            Debug.Log("w: " + bbox1[i].w);
            Debug.Log("h: " + bbox1[i].h);

            Debug.Log("obj_id: " + bbox1[i].obj_id);
            Debug.Log("track_id: " + bbox1[i].track_id);
            
        }
        */
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}

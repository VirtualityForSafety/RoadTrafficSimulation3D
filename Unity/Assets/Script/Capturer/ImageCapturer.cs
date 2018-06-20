using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using SCPAR.SIM.PVATestbed;
using System.Runtime.CompilerServices;

namespace SCPAR.SIM.DataLogging
{
    public class ImageCapturer : MonoBehaviour
    {
        // current image resolution
        public int captureWidth = 1280;
        public int captureHeight = 720;

        public bool capture360 = true;
        private bool startCapture = false;
        public bool isReady = false;

        //screenshot 저장할 rect 저장
        public Rect rect;

        //render할 texture 저장
        public RenderTexture renderTexture;

        //camera에서 보이는 screen이 texture로 저장된 형태임
        public Texture2D screenShot;

        [SerializeField]
        private Camera observationCamera;

        [SerializeField]
        private Camera camera;

        public bool isCapturing = true;

        string fileSavePath;



        // Use this for initialization
        void Start()
        {
            isReady = false;
        }

        public void initialize(string path)
        {
            fileSavePath = path;
            isReady = true;
        }

        public void complete()
        {
            isReady = false;
        }

        //주어진 camera로부터 raw image를 불러와 리턴하는 함수
        //일단 single frame만 생각하자
        public byte[] CaptureSingleFrameRawImage(Camera _camera, out int width, out int height)
        {
            //camera set하기
            camera = _camera;

            if (renderTexture == null)
            {
                rect = new Rect(0, 0, captureWidth, captureHeight);

                renderTexture = new RenderTexture(captureWidth, captureHeight, 24);

                screenShot = new Texture2D(captureWidth, captureHeight, TextureFormat.RGB24, false);
            }

            camera.targetTexture = renderTexture;

            camera.Render();

            RenderTexture.active = renderTexture;
            screenShot.ReadPixels(rect, 0, 0);

            camera.targetTexture = null;
            RenderTexture.active = null;

            byte[] fileData = null;

            //screenshot으로부터 byte array가져오기 png encoding으로
            fileData = screenShot.EncodeToPNG();

            width = captureWidth;
            height = captureHeight;

            return fileData;
        }

        public void Capture(string fileName, Camera _camera)
        {
            if (isCapturing == true)
            {
                /*
                if (capture360)
                {
                    capture360Scene();
                }
                */
                if (observationCamera)
                {
                    captureScene(observationCamera, fileName);
                }
                else
                    captureScene(camera, fileName);
                
            }
        }

        /*
        void capture360Scene()
        {
            LS360VRCamera[] captureComp = Resources.FindObjectsOfTypeAll(typeof(LS360VRCamera)) as LS360VRCamera[];
            if (!startCapture)
            {
                captureComp[0].setCaptureState(true);
                startCapture = true;
            }
            else
                captureComp[0].setCaptureState(false);
        }
        */

        void captureScene(Camera camera, string timeText)
        {
            //camera = _rcc_camera.cinematicCam.rccCamera.cam;

            if (renderTexture == null)
            {
                //rect 크기 정의하고
                rect = new Rect(0, 0, captureWidth, captureHeight);

                //24 bit로 하기
                renderTexture = new RenderTexture(captureWidth, captureHeight, 24);

                screenShot = new Texture2D(captureWidth, captureHeight, TextureFormat.RGB24, false);
            }

            //camera의 texture 저장 가능
            camera.targetTexture = renderTexture;

            //수동적으로 camera render하는 듯
            camera.Render();

            RenderTexture.active = renderTexture;
            screenShot.ReadPixels(rect, 0, 0);

            camera.targetTexture = null;
            RenderTexture.active = null;


            //byte로 저장하기 위해서 header랑 data 부분 따로 정의하기
            //byte[] fileHeader = null;
            byte[] fileData = null;

            fileData = screenShot.EncodeToPNG();

            new System.Threading.Thread(() =>
            {
                System.IO.File.WriteAllBytes(fileSavePath + "/" + timeText + ".png", fileData);
                //Debug.Log("on thread...");
                //Debug.Log(fileSavePath + "/" + timeText + ".png");
            }).Start();
        }

        public void changeCaptureState()
        {
            isCapturing = !isCapturing;
        }
    }
}
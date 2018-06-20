using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

namespace SCPAR.SIM.DataLogging
{
    public class DataCapturer : MonoBehaviour
    {
        public bool isCapturing = true;
        public bool isReady = false;
        string fileSavePath;
        StreamWriter writer;

        // Use this for initialization
        void Start()
        {
            isReady = false;
        }

        public void initialize(string path, string fileName)
        {
            fileSavePath = path;
            writer = new StreamWriter(fileSavePath + fileName, true);
            isReady = true;
        }

        public void complete()
        {
            if (isReady)
            {
                isReady = false;
                writer.Close();
            }
        }

        void OnDisable(){
            complete();
        }

        public void Capture(string aLine)
        {
            //Debug.Log(aLine);
            writer.WriteLine(aLine);
        }

        public void Capture(string fileName, Camera camera, List<DataLabel> labels,
            //Vector3 relativePosition, Vector3 carHeadingDirection, float carVelocity, Vector3 pedHeadingDirection, float pedVelocity, 
            float accidentProb)
        {
            if (isCapturing && isReady)
            {
                //*
                string aLine = fileName + "," + labels.Count;
                for (int i=0; i< labels.Count; i++)
                {
                    aLine += "," + labels[i].ToString();
                                 //+ carHeadingDirection.x + "," + carHeadingDirection.y + "," + carHeadingDirection.z + "," + carVelocity + "," + pedHeadingDirection.x + "," + pedHeadingDirection.y + "," + pedHeadingDirection.z + "," + pedVelocity + ","                     
                }
                //Debug.Log(aLine);
                writer.WriteLine(aLine);
                //*/
            }
        }

        public void changeCaptureState()
        {
            isCapturing = !isCapturing;
        }
    }
}
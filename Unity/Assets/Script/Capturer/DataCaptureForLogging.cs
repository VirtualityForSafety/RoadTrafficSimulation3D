using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace SCPAR.SIM.DataLogging
{
    public class DataCaptureForLogging : MonoBehaviour
    {
        public bool isCapturing = true;
        public bool isReady = false;
        string fileSavePath;
        StreamWriter writer;

        // Use this for initialization
        void Start()
        {
            //isReady = false;
        }

        void createFolder(string filePath)
        {
            try
            {
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }

            }
            catch (IOException ex)
            {
                //Console.WriteLine(ex.Message);
            }
        }

        public void initialize(string path, string subpath, string fileName)
        {
            fileSavePath = path;
            createFolder(fileSavePath);
            createFolder(fileSavePath+subpath);
            writer = new StreamWriter(fileSavePath + subpath+ fileName, true);
            isReady = true;
        }

        public void complete()
        {
            if (isReady)
            {
                isReady = false;
                Debug.Log("On Complete");
                writer.Close();
            }
        }

        private void OnApplicationQuit()
        {
            Debug.Log("On Application Quit");
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
                for (int i = 0; i < labels.Count; i++)
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


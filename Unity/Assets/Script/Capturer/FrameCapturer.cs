using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using SCPAR.SIM.PVATestbed;

namespace SCPAR.SIM.DataLogging
{
    public class FrameCapturer : MonoBehaviour
    {
        public bool isCapturing = true;
        public bool isReady = false;
        string basePath;
        string fileSavePath;
        string folderName;
        string fileName;
        int folderIndex;
        Rect screenRect;

        public bool ensureDataConsistency = true;
        public bool ensureNonOverlapped = true;

        [SerializeField]
        private ImageCapturer imageCapturer;

        [SerializeField]
        private DataCapturer dataCapturer;

        RegionCapturer regionCapturer;

        [SerializeField]
        private PedestrianCamera pCamera;

        [SerializeField]
        private CarPool carPool;

        int interval = 40;

        int count = 0;

        int prvCarNumbers = -1;

        int writingCompleteIndex = -1;

        // Use this for initialization
        void Start()
        {
            basePath = Application.dataPath + "/../../Testset/Sequence/";
            folderName = "testFolder/";
            fileName = "testFile.csv";
            createFolder(basePath);
            isReady = false;
            folderIndex = 1;
            count = 0;

            if (imageCapturer == null)
                imageCapturer = transform.Find("ImageCapturer").GetComponent<ImageCapturer>();
            if (dataCapturer == null)
                dataCapturer = transform.Find("DataCapturer").GetComponent<DataCapturer>();
            if (carPool == null)
                carPool = GameObject.Find("CarPool").GetComponent<CarPool>();
            regionCapturer = new RegionCapturer();

            screenRect = new Rect(0, 0, imageCapturer.captureWidth, imageCapturer.captureHeight);
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
                Console.WriteLine(ex.Message);
            }
        }

        string createFolderWithName()
        {
            folderName = String.Format("Seq{0:D10}/", folderIndex);
            while(Directory.Exists(basePath + folderName))
            {
                folderIndex++;
                folderName = String.Format("Seq{0:D10}/", folderIndex);
            }
            return folderName.Substring(0, folderName.Length - 1);
        }

        void complete(bool success)
        {
            isReady = false;
            imageCapturer.complete();
            dataCapturer.complete();
            prvCarNumbers = -1;
            if(success && writingCompleteIndex>0)
                Debug.Log(writingCompleteIndex);
            writingCompleteIndex = -1;
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (isCapturing)
            {
                if (imageCapturer && dataCapturer && !isReady)
                {
                    fileName = createFolderWithName() + ".csv";
                    fileSavePath = basePath + folderName;
                    createFolder(fileSavePath);
                    imageCapturer.initialize(fileSavePath);
                    dataCapturer.initialize(fileSavePath, fileName);
                    imageCapturer.isCapturing = true;
                    dataCapturer.isCapturing = true;
                    isReady = true;
                    writingCompleteIndex = carPool.dataIndex;
                }
                //Debug.Log("FrameCapturer - ready");
                string timeText = System.DateTime.Now.ToString("yyyyMMddhhmmssfff");

                if (dataCapturer && dataCapturer.isReady)
                {
                    List<DataLabel> labels = new List<DataLabel>();
                    List<DataLabel> nonOccluded = new List<DataLabel>();
                    List<DataLabel> nonOverlapped = new List<DataLabel>();
                    for (int i = 0; i < carPool.cars.Count; i++)
                    {
                        Vector3 _relativePosition = carPool.cars[i].transform.position - pCamera.transform.position;
                        // check whether the 2D region of car overlaps screen area or not
                        Rect carRegion = regionCapturer.getScreenRect(pCamera.GetComponent<Camera>(), carPool.cars[i].GetComponent<Collider>(), imageCapturer.captureHeight);
                        if (screenRect.Overlaps(carRegion) && _relativePosition.z < 0)
                        {
                            float croppedX = Mathf.Max(carRegion.xMin, 0);
                            float croppedY = Mathf.Max(carRegion.yMin, 0);
                            Rect croppedRegion = new Rect(croppedX, croppedY, Mathf.Min(carRegion.xMax, imageCapturer.captureWidth - 1) - croppedX, Mathf.Min(carRegion.yMax, imageCapturer.captureHeight - 1)-croppedY);
                            if (croppedRegion.width >= 800 && croppedRegion.height >= 500)
                            {
                                continue;
                            }
                            // 1. image resolution : 40x40
                            if(croppedRegion.width>= 40 && croppedRegion.height>=40)
                                labels.Add(new DataLabel(carPool.cars[i].GetComponent<SCPAR.SIM.PVATestbed.Car>().getID(), _relativePosition, croppedRegion, Vector3.Distance(carPool.cars[i].transform.position, pCamera.transform.position), 0));
                        }
                    }

                    // sort by distance
                    if (labels.Count > 0)
                    {
                        labels.Sort(delegate (DataLabel a, DataLabel b)
                        {
                            return a.distance.CompareTo(b.distance);
                        });

                        // 2. check occlusions
                        RaycastHit hitInfo;
                        
                        for(int i=0; i<labels.Count; i++)
                        {
                            Vector3 dir = labels[i].relativeDistance;
                            if (Physics.Raycast(pCamera.transform.position, dir, out hitInfo, 1000))
                            {
                                //Debug.Log(hitInfo.collider.name + ", " + hitInfo.collider.tag);
                                if (hitInfo.collider.tag == "Car")
                                {
                                    nonOccluded.Add(labels[i]);
                                }
                            }
                        }
                        

                        if (ensureNonOverlapped)
                        {
                            // 3. check whether the region ovelaps or not
                            for (int i = 0; i < nonOccluded.Count; i++)
                            {
                                bool overlapped = false;
                                for (int k = 0; k < nonOverlapped.Count; k++)
                                {
                                    if (nonOverlapped[k].region.Overlaps(nonOccluded[i].region))
                                    {
                                        overlapped = true;
                                        break;
                                    }
                                }
                                if (!overlapped)
                                    nonOverlapped.Add(nonOccluded[i]);
                            }
                        }
                    }

                    List<DataLabel> resultLabel;

                    if (ensureNonOverlapped)
                        resultLabel = nonOverlapped;
                    else
                        resultLabel = nonOccluded;

                    if (resultLabel.Count == 0 || (prvCarNumbers >= 0 && resultLabel.Count != prvCarNumbers))
                    {
                        complete(false);
                        //Debug.Log(fileSavePath);
                        string[] filePaths = Directory.GetFiles(fileSavePath);
                        foreach (string filePath in filePaths)
                            File.Delete(filePath);
                        Directory.Delete(fileSavePath, true);
                        //if (Directory.Exists(fileSavePath)) { Directory.Delete(fileSavePath, true); }
                        count=0;
                        return;
                    }

                    dataCapturer.Capture(timeText, pCamera.GetComponent<Camera>(), resultLabel, 0);
                    prvCarNumbers = resultLabel.Count;
                    
                    //Debug.Log("DataCapturer - capture complete!");
                    //dataCapturer.Capture(timeText, pCamera.relativePosition, pCamera.carObject.transform.rotation.eulerAngles, pCamera.carObject.speed, pCamera.transform.rotation.eulerAngles, 1, 0);
                }

                if (imageCapturer && imageCapturer.isReady)
                {
                    imageCapturer.Capture(timeText, pCamera.GetComponent<Camera>());
                    //Debug.Log("ImageCapturer - capture complete!");
                }

                if (count % interval == interval - 1)
                {
                    complete(true);
                }
                count++;
            }
        }

        public byte[] CaptureSingleFrameRawImage(Camera _camera, out int width, out int height)
        {
            return imageCapturer.CaptureSingleFrameRawImage(_camera, out width, out height);
        }
    }
}
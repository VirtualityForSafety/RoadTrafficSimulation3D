using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

namespace SCPAR.SIM.DataLogging
{
    public class CarScenario : System.Object
    {
        public int frameIndex;
        public string carModelName;
        public int startJunctionIndex;
        public List<int> trackIndex;

        public CarScenario(int f, string c, int s, List<int> t)
        {
            frameIndex = f;
            carModelName = c;
            startJunctionIndex = s;
            trackIndex = new List<int>();
            for(int i=0; i<t.Count; i++)
            {
                trackIndex.Add(t[i]);
            }
        }
        
        public override string ToString()
        {
            return frameIndex + "," + carModelName + "," + startJunctionIndex + "," + trackIndex.Count;
        }
    }

    public class CarScenarioManager : MonoBehaviour
    {
        DataCapturer dataCapturer;
        bool loadMode;
        public string loadFileName = "VSC0000000000.csv";
        string basePath;
        string fileName;
        List<CarScenario> scenario;
        int scenarioIndex;

        private void Start()
        {
            dataCapturer = transform.Find("CarCapturer").GetComponent<DataCapturer>();
            scenario = new List<CarScenario>();
        }

        public void initialize(bool loadMode)
        {
            scenarioIndex = 0;
            this.loadMode = loadMode;
            basePath = Application.dataPath + "/../../Testset/VehicleScenario/";
            fileName = getFileName(basePath);
            if(!loadMode)
                dataCapturer.initialize(basePath, fileName);
            else
                load(basePath + loadFileName);
        }

        void load(string path)
        {
            foreach (var aLine in File.ReadAllLines(path))
            {
                string[] items = aLine.Split(',');
                List<int> trackIndex = new List<int>();
                for (int i = 0; i < int.Parse(items[3]); i++)
                {
                    trackIndex.Add(int.Parse(items[4 + i]));
                }
                scenario.Add(new CarScenario(int.Parse(items[0]), items[1], int.Parse(items[2]), trackIndex));
            }
        }

        string getFileName(string path)
        {
            int fileIndex = 0;
            string fileName = String.Format("VSC{0:D10}", fileIndex) + ".csv";
            while (File.Exists(path + fileName))
            {
                fileIndex++;
                fileName = String.Format("VSC{0:D10}", fileIndex) + ".csv";
            }
            return fileName;
        }

        public void save(int dataIndex, string carModelName, int index, ref List<int> trackIdx)
        {
            string dataString = dataIndex + "," + carModelName + "," + index + "," + trackIdx.Count;
            for (int i = 0; i < trackIdx.Count; i++)
                dataString += "," + trackIdx[i];
            dataCapturer.Capture(dataString);
        }

        public CarScenario executeLoadData(int frameIndex)
        {
            //while (frameIndex > scenario[scenarioIndex].frameIndex)
            //scenarioIndex++;
            if (scenarioIndex >= scenario.Count)
                return null;
            if (frameIndex == scenario[scenarioIndex].frameIndex)
            {
                if(scenarioIndex < scenario.Count)
                    scenarioIndex++;
                return scenario[scenarioIndex - 1];
            }
            else
                return null;
        }
    }
}
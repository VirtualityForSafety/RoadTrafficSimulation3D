using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using SCPAR.SIM.DataLogging;
using System;

namespace SCPAR.SIM.PVATestbed
{
    public class CarPool : MonoBehaviour
    {
        public Car dummyCar;
        public List<GameObject> cars;
        public World world;
        public TrafficLightManager trafficLightManager;
        public int numOfCars = 1;
        public List<SummonArea> summonAreas;
        int creatingInterval = 30;
        int createCount = -1;
        public bool saveData = true;
        public bool loadData = false;
        public int dataIndex = 0;
        Transform trajectoryParent;
        public int spawnRate = 2;
        CarScenarioManager carScenarioManager;
        DataCapturer dataCapturer;
        public bool fullDataLogging = false;
        public void Start()
        {
            if (loadData)
                saveData = false;
            cars = new List<GameObject>();
            world = GameObject.Find("World").GetComponent<World>();
            trafficLightManager = GameObject.Find("TrafficLightManager").GetComponent<TrafficLightManager>();
            createCount = -1;
            dataIndex = 0;
            trajectoryParent = GameObject.Find("CarTrajectory").transform;
            carScenarioManager = GetComponent<CarScenarioManager>();
            if (fullDataLogging)
            {
                dataCapturer = GetComponent<DataCapturer>();
                dataCapturer.initialize(Application.dataPath + "/../../Testset/ExperimentLog/", "totalFrame.csv");
            }
        }

        private void OnDisable()
        {
            if (fullDataLogging)
                dataCapturer.complete();
        }

        // Use this for initialization
        public void initialize()
        {
            for(int i=0; i<world.roadEnds.Count; i++)
            {
                generateSummonArea(world.roadEnds[i], world.roads);
            }

            if (loadData)
            {
                carScenarioManager.initialize(true);
            }
            else if (saveData)
            {
                carScenarioManager.initialize(false);
            }
        }

        public int getDataIndex()
        {
            return dataIndex;
        }

        void generateSummonArea(Junction source, List<Road> roads)
        {
            Vector3 summonPosition;
            Vector3 currentPosition = Vector3.zero;
            AbsDirection currentDirection = AbsDirection.NA;
            int dir = 0;
            for (; dir < 4; dir++)
            {
                if (source.connectRoadIdx[dir] >= 0)
                {
                    currentPosition = roads[source.connectRoadIdx[dir]].getRightmostLane().fromPoint;
                    currentDirection = roads[source.connectRoadIdx[dir]].direction;
                    break;
                }
            }
            summonPosition = currentPosition + Util.absDirection2Vec3(currentDirection) * SimParameter.unitBlockSize + Vector3.up*1.5f;// + Util.absDirection2Vec3(trajectory.getCurrentLane().direction);
            if(currentDirection == AbsDirection.N || currentDirection == AbsDirection.S)
                summonAreas.Add(UnityEngine.Object.Instantiate((Resources.Load("Prefab/SummonArea") as GameObject).GetComponent<SummonArea>(), summonPosition, Quaternion.identity, world.transform));
            else
                summonAreas.Add(UnityEngine.Object.Instantiate((Resources.Load("Prefab/SummonArea") as GameObject).GetComponent<SummonArea>(), summonPosition, Quaternion.Euler(0,90,0), world.transform));
        }

        void spawnCar(CarScenario aScenario)
        {
            int startJunctionIndex = aScenario == null ? world.getRandomEndPointIdx() : aScenario.startJunctionIndex;
            createCount = aScenario == null ? createCount : -1;
            if (!summonAreas[startJunctionIndex].hasCarInside && createCount <0)
            {
                //string carModelName = aScenario == null ? "Prefab/Vehicle/CarModel00" + (int)UnityEngine.Random.Range(1, 10) : aScenario.carModelName;
                string carModelName = "Prefab/Vehicle/CarModel001";
                cars.Add(UnityEngine.Object.Instantiate(Resources.Load(carModelName) as GameObject, this.transform));

                List<int> trackIdx = new List<int>();
                if (aScenario != null)
                    cars[cars.Count - 1].GetComponent<Car>().initialize(world.roadEnds[startJunctionIndex], world.roads, trafficLightManager, trajectoryParent, aScenario.trackIndex);
                else if (saveData)
                    cars[cars.Count - 1].GetComponent<Car>().initialize(world.roadEnds[startJunctionIndex], world.roads, trafficLightManager, trajectoryParent, ref trackIdx);
                else
                    cars[cars.Count - 1].GetComponent<Car>().initialize(world.roadEnds[startJunctionIndex], world.roads, trafficLightManager, trajectoryParent);
                createCount = creatingInterval;
                if (saveData)
                {
                    carScenarioManager.save(dataIndex, carModelName, startJunctionIndex, ref trackIdx);
                }
            }
        }

        public void operate()
        {
            if (loadData)
            {
                CarScenario aScenario = carScenarioManager.executeLoadData(dataIndex);
                if (aScenario != null)
                {
                    generateCar(aScenario);
                }
            }

            for(int i=0; i<cars.Count; i++)
            {
                if (!cars[i].GetComponent<Car>().alive)
                {
                    cars[i].GetComponent<Car>().removeTrajectory(trajectoryParent);
                    Destroy(cars[i]);
                    cars.RemoveAt(i);
                }
            }
        }

        void generateCar(CarScenario aScenario)
        {
            if (cars.Count < numOfCars)
            {
                spawnCar(aScenario);
            }

            if (createCount > -1)
                createCount--;
        }

        string record()
        {
            string resultString = dataIndex.ToString();
            /*
            if (eyeSeeComp.targets.Count == 0)
                return resultString;
            else
            {
                resultString += "," + eyeSeeComp.targets.Count;
                for (int i = 0; i < eyeSeeComp.targets.Count; i++)
                {
                    Vector3 dist = eyeSeeComp.targets[i].transform.position - Camera.main.transform.position;
                    resultString += "," + dist.x + "," + dist.z + "," + dist.magnitude + "," + eyeSeeComp.targets[i].GetComponent<Car>().speed + "," + eyeSeeComp.targets[i].transform.forward.x + "," + eyeSeeComp.targets[i].transform.forward.z;
                }

                return resultString;
            }
            */
            return resultString;
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            operate();
            if(!loadData)
                if (dataIndex % spawnRate == 0)
                    generateCar(null);
            if(fullDataLogging)
                dataCapturer.Capture(record());
            dataIndex++;
        }
    }
}
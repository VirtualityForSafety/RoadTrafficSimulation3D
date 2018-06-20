using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SCPAR.SIM.PVATestbed
{
    public class PedPool : MonoBehaviour
    {
        public List<GameObject> pedestrians;
        public World world;
        public TrafficLightManager trafficLightManager;
        public int numOfPeds = 1;

        // Use this for initialization
        public void initialize()
        {
            pedestrians = new List<GameObject>();
            world = GameObject.Find("World").GetComponent<World>();
            //spawnPedestrian(world.sidewalks);
        }

        void spawnPedestrian(List<Block> sidewalks)
        {
            for(int i=0; i< 5; i++)
            {
                if (sidewalks[i].blockDirection == BlockDirection.Intersection)
                    continue;
                pedestrians.Add(Object.Instantiate(Resources.Load("Prefab/Pedestrian/PedModel001") as GameObject, this.transform));
                pedestrians[pedestrians.Count - 1].transform.position = sidewalks[i].block.transform.position + Vector3.up * 4.5f;
                if (sidewalks[i].blockDirection==BlockDirection.Horizontal)
                    pedestrians[pedestrians.Count - 1].transform.Rotate(new Vector3(0, 90, 0));
                pedestrians[pedestrians.Count - 1].GetComponent<Pedestrian>().initialize();
            }
        }
        /*
        public void operate()
        {
            for(int i=0; i<cars.Count; i++)
            {
                if (!cars[i].GetComponent<Car>().alive)
                {
                    Destroy(cars[i]);
                    cars.RemoveAt(i);
                }
            }

            if(cars.Count < numOfCars)
            {
                spawnCar((int)Random.Range(1.0f,6.0f));
            }
        }*/

        private void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
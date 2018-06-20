using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SCPAR.SIM.PVATestbed
{
    public class AreaBlock : Block
    {
        override protected void createModel(ModelType givenType)
        {
            if(givenType == ModelType.AreaGrass)
                block = (GameObject)Instantiate(Resources.Load("Prefab/BlockGrass"));
            else if (givenType == ModelType.AreaBuilding)
                block = (GameObject)Instantiate(Resources.Load("Prefab/BlockBuilding"));
            else if (givenType == ModelType.AreaForest)
                block = (GameObject)Instantiate(Resources.Load("Prefab/BlockForest"));
            else if (givenType == ModelType.AreaSidewalk)
            {
                block = (GameObject)Instantiate(Resources.Load("Prefab/BlockSidewalk"));
                blockDirection = (isHorizontal) ? BlockDirection.Horizontal : BlockDirection.Vertical;
            }
            else if (givenType == ModelType.AreaSidewalkCorner)
            {
                block = (GameObject)Instantiate(Resources.Load("Prefab/BlockSidewalkCorner"));
                blockDirection = BlockDirection.Intersection;
                setTrafficLight();                
            }
            block.transform.parent = transform;
            block.transform.position = new Vector3(mapPosition.x * 4, SimParameter.areaHeight, mapPosition.y * 4);
            if (isHorizontal)
                block.transform.Rotate(new Vector3(0, 90, 0));
            if(areaPosition == AreaPosition.NE)
                block.transform.Rotate(new Vector3(0, 0, 0));
            else if (areaPosition == AreaPosition.NW)
                block.transform.Rotate(new Vector3(0, -90, 0));
            else if (areaPosition == AreaPosition.SW)
                block.transform.Rotate(new Vector3(0, 180, 0));
            else if (areaPosition == AreaPosition.SE)
                block.transform.Rotate(new Vector3(0, 90, 0));
        }

        void setTrafficLight()
        {
            GameObject trafficLight = (GameObject)Instantiate(Resources.Load("Prefab/TrafficLight"), block.transform);
            trafficLight.transform.localPosition = new Vector3(1.2f, 0, -1.2f);
            GameObject.Find("TrafficLightManager").GetComponent<TrafficLightManager>().addTrafficLight(trafficLight.GetComponent<TrafficLight>());
            //trafficLight.transform.parent = GameObject.Find("TrafficLightManager").transform;
            if (areaPosition == AreaPosition.NW)
                trafficLight.GetComponent<TrafficLight>().setDirection(AbsDirection.W);
            else if (areaPosition == AreaPosition.NE)
                trafficLight.GetComponent<TrafficLight>().setDirection(AbsDirection.N);
            else if (areaPosition == AreaPosition.SW)
                trafficLight.GetComponent<TrafficLight>().setDirection(AbsDirection.S);
            else if (areaPosition == AreaPosition.SE)
                trafficLight.GetComponent<TrafficLight>().setDirection(AbsDirection.E);
        }

        // Use this for initialization
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}

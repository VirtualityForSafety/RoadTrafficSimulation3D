using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SCPAR.SIM.PVATestbed
{
    public class RoadBlock : Block
    {
        override protected void createModel(ModelType givenType)
        {
            if(givenType == ModelType.RoadNormal)
                block = (GameObject)Instantiate(Resources.Load("Prefab/BlockRoad"));
            else if (givenType == ModelType.RoadIntersection)
                block = (GameObject)Instantiate(Resources.Load("Prefab/BlockIntersection"));
            block.transform.parent = transform;
            block.transform.position = new Vector3(mapPosition.x * SimParameter.unitBlockSize, 0, mapPosition.y * SimParameter.unitBlockSize);
            if (isHorizontal)
                block.transform.Rotate(new Vector3(0, 90, 0));
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

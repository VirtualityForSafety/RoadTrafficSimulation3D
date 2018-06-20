using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SCPAR.SIM.PVATestbed
{
    public enum LineType { SolidYellow, DashWhite, SolidWhite };
    public class Line : MonoBehaviour
    {
        public int length = 6;
        public List<GameObject> blocks;
        public bool leftAdjacent;
        public bool rightAdjacent;
        public bool rightmostAdjacent;
        public bool leftmostAdjacent;
        public List<Vector2> carsPositions;

        public void initialize(int fromPos, int toPos, float fixedPos, Vector2 center, bool isHorizontal, LineType type)
        {
            string modelName = "Prefab/LineSolidYellow";
            if (type == LineType.DashWhite)
                modelName = "Prefab/LineDotWhite";
            else if (type == LineType.SolidWhite)
                modelName = "Prefab/LineSolidWhite";

            length = Mathf.Abs(toPos - fromPos);
            Vector3 direction = new Vector3(0, 90, 0);
            for (int i=0; i< length; i++)
            {
                if (isHorizontal)
                    blocks.Add(Object.Instantiate((GameObject)Resources.Load(modelName), new Vector3((fromPos + i + (int)center.x) * SimParameter.unitBlockSize, 0.52f, (fixedPos + (int)center.y) * SimParameter.unitBlockSize), Quaternion.identity));
                else
                    blocks.Add(Object.Instantiate((GameObject)Resources.Load(modelName), new Vector3((fixedPos + (int)center.x) * SimParameter.unitBlockSize, 0.52f, (fromPos + i + (int)center.y) * SimParameter.unitBlockSize), Quaternion.Euler(direction)));
               
                blocks[blocks.Count - 1].transform.parent = transform;
            }
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

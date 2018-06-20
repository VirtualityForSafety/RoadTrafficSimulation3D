using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SCPAR.SIM.PVATestbed
{
    public class Lane : MonoBehaviour
    {
        public int length = 6;
        public int index;
        public List<Block> blocks;
        public Lane leftAdjacent;
        public Lane rightAdjacent;
        public Lane rightmostAdjacent;
        public Lane leftmostAdjacent;
        public AbsDirection direction;
        public bool isHorizontal;
        Road road;
        public Rect region;
        public Vector3 center;

        /// <summary>
        public Vector3 fromPoint;
        public Vector3 toPoint;
        /// </summary>

        public List<Vector2> carsPositions;
        public List<Vector3> entryPoints;

        public float getRelativePosition(Vector3 position)
        {
            return Vector3.Distance(fromPoint, position) / Vector3.Distance(fromPoint, toPoint);
        }

        public void initialize(Road givenRoad, int fromPos, int toPos, int fixedPos, int _index, Vector2 _center, AbsDirection givenDirection, bool _isHorizontal)
        {
            index = _index;
            road = givenRoad;
            center = _center;
            isHorizontal = _isHorizontal;
            direction = givenDirection;
            if (isHorizontal)
            {
                fromPoint = new Vector3((fromPos -0.5f + 2 + (int)center.x) * 4, 1.0f, (fixedPos + (int)center.y) * 4);
                toPoint = new Vector3((toPos - 0.5f - 2 + (int)center.x) * 4, 1.0f, (fixedPos + (int)center.y) * 4);
                Vector2 tempDirection = new Vector2(toPoint.x - fromPoint.x, toPoint.z - fromPoint.z);
                //Debug.Log(tempDirection + "\t" + givenDirection);
                if ( direction != Util.vec22AbsDirection(tempDirection.normalized)){
                    Vector3 temp = toPoint;
                    toPoint = fromPoint;
                    fromPoint = temp;
                }
            }
            else
            {
                fromPoint = new Vector3((fixedPos + (int)center.x) * 4, 1.0f, (fromPos + -0.5f + 2 + (int)center.y) * 4);
                toPoint = new Vector3((fixedPos + (int)center.x) * 4, 1.0f, (toPos - 0.5f - 2 + (int)center.y) * 4);
                Vector2 tempDirection = new Vector2(toPoint.x - fromPoint.x, toPoint.z - fromPoint.z);
                //Debug.Log(tempDirection + "\t" + givenDirection);
                if (direction != Util.vec22AbsDirection(tempDirection.normalized))
                {
                    Vector3 temp = toPoint;
                    toPoint = fromPoint;
                    fromPoint = temp;
                }
            }
            
            entryPoints = new List<Vector3>();
            length = Mathf.Abs(toPos - fromPos);
            if(isHorizontal)
                region = new Rect((float)(fromPos + center.x) * SimParameter.unitBlockSize, (float)(fixedPos + (int)center.y) * SimParameter.unitBlockSize, (float)(toPos - fromPos) * SimParameter.unitBlockSize, SimParameter.unitBlockSize);
            else
                region = new Rect((float)(fixedPos + center.x) * SimParameter.unitBlockSize, (float)(fromPos + (int)center.y) * SimParameter.unitBlockSize, SimParameter.unitBlockSize, (float)(toPos - fromPos) * SimParameter.unitBlockSize);

            for (int i=0; i< length; i++)
            {
                blocks.Add(Object.Instantiate((Resources.Load("Prefab/DummyObjects/DummyRoadBlock") as GameObject).GetComponent<RoadBlock>(), Vector3.zero, Quaternion.identity) as SCPAR.SIM.PVATestbed.RoadBlock);
                if (isHorizontal)
                    blocks[blocks.Count - 1].initialize(fromPos+i + (int)center.x, fixedPos + (int)center.y, ModelType.RoadNormal, AreaPosition.NA, isHorizontal);
                else
                    blocks[blocks.Count - 1].initialize(fixedPos + (int)center.x, fromPos + i + (int)center.y, ModelType.RoadNormal, AreaPosition.NA, isHorizontal);
                blocks[blocks.Count - 1].transform.parent = transform;
            }

            //generateEntryPoints(fromPos, toPos, fixedPos, center, isHorizontal);
        }

        void generateEntryPoints(int fromPos, int toPos, int fixedPos, Vector2 center, bool isHorizontal)
        {
            Object.Instantiate((GameObject)Resources.Load("Prefab/Waypoint"), fromPoint, Quaternion.identity, this.transform);
            Object.Instantiate((GameObject)Resources.Load("Prefab/Waypoint"), toPoint, Quaternion.identity, this.transform);
        }

        public Road getRoad() { return road; }

        public int getTurnDirection(Lane other)
        {
            return -1;
            //return road.getTurnDirection(other.road);
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

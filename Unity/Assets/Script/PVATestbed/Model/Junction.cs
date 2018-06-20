using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SCPAR.SIM.PVATestbed
{
    public class JunctionSize
    {
        public int numOfHrzLane;
        public int numOfVtcLane;
        public int lenOfHrzLane;
        public int lenOfVtcLane;

        public JunctionSize()
        {
            numOfHrzLane = 0;
            numOfVtcLane = 0;
            lenOfHrzLane = 0;
            lenOfVtcLane = 0;
        }

        public JunctionSize(int givenNumOfHrz, int givenNumOfVtc, int givenLenOfHrz, int givenLenOfVtc)
        {
            numOfHrzLane = givenNumOfHrz;
            numOfVtcLane = givenNumOfVtc;
            lenOfHrzLane = givenLenOfHrz;
            lenOfVtcLane = givenLenOfVtc;
        }
    }

    public class Junction : MonoBehaviour
    {
        public Vector2 coordIndex;
        public Vector2 center;
        public Vector3 centerWorld;
        public bool isNullJunction;
        public JunctionSize size;
        
        public int[] connectRoadIdx;
        public Area[] areas;
        public Area areaInter;

        public bool randomArea = true;

        public void initialize(Vector2 givenCoordIndex, Vector2 givenCenter, JunctionSize givenSize, ref List<Block> sidewalks)
        {
            initialize(givenCoordIndex, givenCenter);
            isNullJunction = false;
            size = givenSize;                
            buildJunction(ref sidewalks);
        }

        public void initialize(Vector2 givenCoordIndex, Vector2 givenCenter)
        {
            isNullJunction = true;
            coordIndex = givenCoordIndex;
            center = givenCenter;
            centerWorld = new Vector3(center.x * SimParameter.unitBlockSize, 0, center.y * SimParameter.unitBlockSize);
            connectRoadIdx = new int[4];
            for (int i = 0; i < 4; i++)
                connectRoadIdx[i] = -1;
        }

        public void buildJunction(ref List<Block> sidewalks)
        {

            // create intersection
            areaInter = Object.Instantiate((Resources.Load("Prefab/DummyObjects/DummyArea") as GameObject).GetComponent<Area>(), Vector3.zero, Quaternion.identity) as SCPAR.SIM.PVATestbed.Area;
            areaInter.initialize(size.numOfVtcLane * 2, size.numOfHrzLane * 2, center, AreaPosition.NA, AreaType.Intersection, ref sidewalks);
            areaInter.transform.parent = transform;

            areas = new Area[4];
            AreaType tempType = (AreaType)2; // AreaType.Forest;// (AreaType)Mathf.Round(Random.Range(0, 3));
            areas[(int)AreaPosition.NW] = Object.Instantiate((Resources.Load("Prefab/DummyObjects/DummyArea") as GameObject).GetComponent<Area>(), Vector3.zero, Quaternion.identity) as SCPAR.SIM.PVATestbed.Area;
            areas[(int)AreaPosition.NW].initialize(size.lenOfHrzLane, size.lenOfVtcLane, new Vector2(-(size.lenOfHrzLane /2+ size.numOfVtcLane) + (int)center.x,
                size.lenOfVtcLane /2+ size.numOfHrzLane + (int)center.y), AreaPosition.NW, tempType, ref sidewalks);
            areas[(int)AreaPosition.NW].transform.parent = transform;

            //tempType = (AreaType)Mathf.Round(Random.Range(0, 3));
            areas[(int)AreaPosition.NE] = Object.Instantiate((Resources.Load("Prefab/DummyObjects/DummyArea") as GameObject).GetComponent<Area>(), Vector3.zero, Quaternion.identity) as SCPAR.SIM.PVATestbed.Area;
            areas[(int)AreaPosition.NE].initialize(size.lenOfHrzLane, size.lenOfVtcLane, new Vector2(size.lenOfHrzLane / 2 + size.numOfVtcLane + (int)center.x,
                size.lenOfVtcLane / 2 + size.numOfHrzLane + (int)center.y), AreaPosition.NE, tempType, ref sidewalks);
            areas[(int)AreaPosition.NE].transform.parent = transform;

            //tempType = (AreaType)Mathf.Round(Random.Range(0, 3));
            areas[(int)AreaPosition.SE] = Object.Instantiate((Resources.Load("Prefab/DummyObjects/DummyArea") as GameObject).GetComponent<Area>(), Vector3.zero, Quaternion.identity) as SCPAR.SIM.PVATestbed.Area;
            areas[(int)AreaPosition.SE].initialize(size.lenOfHrzLane, size.lenOfVtcLane, new Vector2(size.lenOfHrzLane / 2 + size.numOfVtcLane + (int)center.x,
                -size.lenOfVtcLane / 2 - size.numOfHrzLane + (int)center.y), AreaPosition.SE, tempType, ref sidewalks);
            areas[(int)AreaPosition.SE].transform.parent = transform;

            //tempType = (AreaType)Mathf.Round(Random.Range(0, 3));
            areas[(int)AreaPosition.SW] = Object.Instantiate((Resources.Load("Prefab/DummyObjects/DummyArea") as GameObject).GetComponent<Area>(), Vector3.zero, Quaternion.identity) as SCPAR.SIM.PVATestbed.Area;
            areas[(int)AreaPosition.SW].initialize(size.lenOfHrzLane, size.lenOfVtcLane, new Vector2(-size.lenOfHrzLane / 2 - size.numOfVtcLane + (int)center.x,
                -size.lenOfVtcLane / 2 - size.numOfHrzLane + (int)center.y), AreaPosition.SW, tempType, ref sidewalks);
            areas[(int)AreaPosition.SW].transform.parent = transform;
        }
    }

}

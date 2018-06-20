using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SCPAR.SIM.PVATestbed
{
    public class Road : MonoBehaviour
    {
        public int index;
        List<Line> lines;
        public List<Lane> lanes;
        bool isHorizontal;
        public Junction source;
        public Junction target;
        public AbsDirection direction;
        int numOfLanes;
        float length;
        int currentLaneIdx;

        public void initialize(Junction givenSource, Junction givenTarget)
        {

            lanes = new List<Lane>();
            lines = new List<Line>();

            source = givenSource;
            target = givenTarget;
            direction = Util.vec22AbsDirection(givenTarget.coordIndex - givenSource.coordIndex);
            this.isHorizontal = (direction == AbsDirection.N || direction == AbsDirection.S) ? false : true;

            if(givenSource.isNullJunction || givenTarget.isNullJunction)
            {
                Vector2 center = givenSource.isNullJunction ? givenTarget.center : givenSource.center;
                JunctionSize size = givenSource.isNullJunction ? givenTarget.size : givenSource.size;

                center = source.center;
                if (isHorizontal)
                {
                    int startI = source.coordIndex.x < target.coordIndex.x ? -size.numOfHrzLane : 0;
                    length = size.lenOfHrzLane * SimParameter.unitBlockSize;
                    numOfLanes = size.numOfHrzLane;
                    for (int i = 0; i < size.numOfHrzLane; i++)
                    {
                        if (direction == AbsDirection.E)
                            createLane( center, size.numOfVtcLane, (size.lenOfHrzLane + size.numOfVtcLane), startI + i, i, direction, isHorizontal);
                        else if (direction == AbsDirection.W)
                            createLane( center, -(size.lenOfHrzLane + size.numOfVtcLane), -size.numOfVtcLane, startI + i, i, direction, isHorizontal);

                        if (startI + i == 0)
                            createRoadLine( center, -(size.lenOfHrzLane + size.numOfVtcLane), -size.numOfVtcLane, startI + i, isHorizontal);
                        if (i == 0)
                            continue;

                        if (direction == AbsDirection.E)
                            createRoadLine( center, size.numOfVtcLane, (size.lenOfHrzLane + size.numOfVtcLane), startI + i, isHorizontal);
                        else if (direction == AbsDirection.W)
                            createRoadLine( center, -(size.lenOfHrzLane + size.numOfVtcLane), -size.numOfVtcLane, startI + i, isHorizontal);
                    }
                }
                else
                {
                    int startI = source.coordIndex.y > target.coordIndex.y ? -size.numOfVtcLane : 0;
                    length = size.lenOfVtcLane * SimParameter.unitBlockSize;
                    numOfLanes = size.numOfVtcLane;
                    for (int i = 0; i < size.numOfVtcLane; i++)
                    {
                        if (direction == AbsDirection.N)
                        {
                            createLane( center, size.numOfHrzLane, (size.lenOfVtcLane + size.numOfHrzLane), startI + i, i, direction, isHorizontal);
                        }
                            
                        else if (direction == AbsDirection.S)
                        {
                            createLane( center, -(size.lenOfVtcLane + size.numOfHrzLane), -size.numOfHrzLane, startI + i, i, direction, isHorizontal);
                        }
                            

                        if (startI + i == 0)
                            createRoadLine( center, size.numOfHrzLane, (size.lenOfVtcLane + size.numOfHrzLane), startI + i , isHorizontal);
                        if (i == 0)
                            continue;

                        if (direction == AbsDirection.N)
                            createRoadLine( center, size.numOfHrzLane, (size.lenOfVtcLane + size.numOfHrzLane), startI + i, isHorizontal);
                        else if (direction == AbsDirection.S)
                            createRoadLine( center, -(size.lenOfVtcLane + size.numOfHrzLane), -size.numOfHrzLane, startI + i, isHorizontal);
                    }
                }
            }
            else
            {
                if (isHorizontal)
                {
                    if (source.size.numOfHrzLane != target.size.numOfHrzLane)
                        Debug.Log("ERROR: Road - initialize - numOfHrzLane is not matched.");
                    int numOfHrzLane = source.size.numOfHrzLane;
                    int startI = direction == AbsDirection.E ? -numOfHrzLane : 0;
                    length = (source.size.lenOfHrzLane + target.size.lenOfHrzLane) * SimParameter.unitBlockSize;
                    numOfLanes = numOfHrzLane;
                    for (int i = 0; i < numOfHrzLane; i++)
                    {
                        if (source.center.x < target.center.x)
                            createLane( source.center, source.size.numOfVtcLane, source.size.numOfVtcLane + (source.size.lenOfHrzLane + target.size.lenOfHrzLane), startI + i, i, direction, isHorizontal);
                        else
                            createLane( target.center, target.size.numOfVtcLane, target.size.numOfVtcLane + (target.size.lenOfHrzLane + source.size.lenOfHrzLane), startI + i, i, direction, isHorizontal);
                        if (startI + i == 0)
                            createRoadLine( target.center, target.size.numOfVtcLane, target.size.numOfVtcLane + (target.size.lenOfHrzLane + source.size.lenOfHrzLane), startI + i, isHorizontal);
                        if (i == 0)
                            continue;
                        if (source.center.x < target.center.x)
                            createRoadLine( source.center, source.size.numOfVtcLane, source.size.numOfVtcLane + (source.size.lenOfHrzLane + target.size.lenOfHrzLane), startI + i, isHorizontal);
                        else
                            createRoadLine( target.center, target.size.numOfVtcLane, target.size.numOfVtcLane + (target.size.lenOfHrzLane + source.size.lenOfHrzLane), startI + i, isHorizontal);
                    }
                }
                else
                {
                    if (source.size.numOfVtcLane != target.size.numOfVtcLane)
                        Debug.Log("ERROR: Road - initialize - numOfVtcLane is not matched.");
                    int numOfVtcLane = source.size.numOfVtcLane;
                    int startI = direction == AbsDirection.S ? -numOfVtcLane : 0;
                    length = (source.size.lenOfVtcLane + target.size.lenOfVtcLane) * SimParameter.unitBlockSize;
                    numOfLanes = numOfVtcLane;
                    for (int i = 0; i < numOfVtcLane; i++)
                    {
                        if (source.center.y < target.center.y)
                            createLane( source.center, source.size.numOfHrzLane, source.size.numOfHrzLane + (source.size.lenOfVtcLane + target.size.lenOfVtcLane), startI + i, i, direction, isHorizontal);
                        else
                            createLane( target.center, target.size.numOfHrzLane, target.size.numOfHrzLane + (target.size.lenOfVtcLane + source.size.lenOfVtcLane), startI + i, i, direction, isHorizontal);
                        if (startI + i == 0)
                            createRoadLine( source.center, source.size.numOfHrzLane, source.size.numOfHrzLane + (source.size.lenOfVtcLane + target.size.lenOfVtcLane), startI + i, isHorizontal);
                        if (i == 0)
                            continue;
                        if (source.center.y < target.center.y)
                            createRoadLine( source.center, source.size.numOfHrzLane, source.size.numOfHrzLane + (source.size.lenOfVtcLane + target.size.lenOfVtcLane), startI + i, isHorizontal);
                        else
                            createRoadLine( target.center, target.size.numOfHrzLane, target.size.numOfHrzLane + (target.size.lenOfVtcLane + source.size.lenOfVtcLane), startI + i, isHorizontal);
                    }
                }
            }
            if(direction==AbsDirection.E || direction == AbsDirection.S)
            {
                lanes.Reverse();
            }
        }

        public Lane getLeftmostLane()
        {
            return lanes[0];
            /*
            if (direction == AbsDirection.W || direction == AbsDirection.N)
            {
                return lanes[0];
            }                
            else if (direction == AbsDirection.E || direction == AbsDirection.S)
                return lanes[lanes.Count - 1];
            else
                return null;
                */
        }

        public Lane getLeftAdjacentLane(Lane lane)
        {
            if (lane.getRoad() != this)
                return null;
            else
            {
                Lane tempLane = getLeftmostLane();
                for (int i = 0; i < numOfLanes; i++)
                {
                    if (lane == lanes[i] && i > 0)
                        tempLane = lanes[i - 1];
                }
                return tempLane;
            }
        }

        public Lane getRightAdjacentLane(Lane lane)
        {
            if (lane.getRoad() != this)
                return null;
            else
            {
                Lane tempLane = getRightmostLane();
                for (int i = 0; i < numOfLanes; i++)
                {
                    if (lane == lanes[i] && i < numOfLanes-1)
                        tempLane = lanes[i + 1];
                }
                return tempLane;
            }
        }

        public Lane getRightmostLane()
        {
            return lanes[lanes.Count - 1];
            /*
            if (direction == AbsDirection.W || direction == AbsDirection.N)
                return lanes[lanes.Count - 1];
            else if (direction == AbsDirection.E || direction == AbsDirection.S)
                return lanes[0];
            else
                return null;
                */
        }

        private void createLane(Vector2 center, int fromPos, int endPos, int genIndex, int index, AbsDirection direction, bool isHorizontal)
        {
            lanes.Add(Object.Instantiate((Resources.Load("Prefab/DummyObjects/DummyLane") as GameObject).GetComponent<Lane>(), Vector3.zero, Quaternion.identity) as SCPAR.SIM.PVATestbed.Lane);
            lanes[lanes.Count - 1].initialize(this, fromPos, endPos, genIndex, index, center, direction, isHorizontal);
            lanes[lanes.Count - 1].transform.parent = transform;
        }

        private void createStopLine(Vector2 center, float fromPos, float fixedPos, bool isHorizontal, bool start)
        {
            Vector3 direction = new Vector3(0, 90, 0);
            if (start)
            {
                if (isHorizontal)
                    Object.Instantiate((GameObject)Resources.Load("Prefab/StopLine"), new Vector3((fromPos + (int)center.x) * SimParameter.unitBlockSize, 0.52f, (-fixedPos - 1 + (int)center.y) * SimParameter.unitBlockSize), Quaternion.Euler(direction), transform);
                else
                    Object.Instantiate((GameObject)Resources.Load("Prefab/StopLine"), new Vector3((fixedPos + (int)center.x) * SimParameter.unitBlockSize, 0.52f, (fromPos + (int)center.y) * SimParameter.unitBlockSize), Quaternion.identity, transform);
            }
            else
            {
                if (isHorizontal)
                    Object.Instantiate((GameObject)Resources.Load("Prefab/StopLine"), new Vector3((fromPos + (int)center.x) * SimParameter.unitBlockSize, 0.52f, (fixedPos + (int)center.y) * SimParameter.unitBlockSize), Quaternion.Euler(direction), transform);
                else
                    Object.Instantiate((GameObject)Resources.Load("Prefab/StopLine"), new Vector3((-fixedPos-1 + (int)center.x) * SimParameter.unitBlockSize, 0.52f, (fromPos + (int)center.y) * SimParameter.unitBlockSize), Quaternion.identity, transform);
            }
        }

        private void createCrossing(Vector2 center, float fromPos, float fixedPos, bool isHorizontal, bool start)
        {
            Vector3 direction = new Vector3(0, 90, 0);
            if (start)
            {
                if (isHorizontal)
                    Object.Instantiate((GameObject)Resources.Load("Prefab/CrossingLine"), new Vector3((fromPos + (int)center.x) * SimParameter.unitBlockSize, 0.0f, (-fixedPos - 1 + (int)center.y) * SimParameter.unitBlockSize), Quaternion.identity, transform);
                else
                    Object.Instantiate((GameObject)Resources.Load("Prefab/CrossingLine"), new Vector3((fixedPos + (int)center.x) * SimParameter.unitBlockSize, 0.0f, (fromPos + (int)center.y) * SimParameter.unitBlockSize), Quaternion.Euler(direction), transform);
            }
            else
            {
                if (isHorizontal)
                    Object.Instantiate((GameObject)Resources.Load("Prefab/CrossingLine"), new Vector3((fromPos + (int)center.x) * SimParameter.unitBlockSize, 0.0f, (fixedPos + (int)center.y) * SimParameter.unitBlockSize), Quaternion.identity, transform);
                else
                    Object.Instantiate((GameObject)Resources.Load("Prefab/CrossingLine"), new Vector3((-fixedPos - 1 + (int)center.x) * SimParameter.unitBlockSize, 0.0f, (fromPos + (int)center.y) * SimParameter.unitBlockSize), Quaternion.Euler(direction), transform);
            }
        }

        private void createRoadLine(Vector2 center, int fromPos, int endPos, int index, bool isHorizontal)
        {
            lines.Add(Object.Instantiate((Resources.Load("Prefab/DummyObjects/DummyLine") as GameObject).GetComponent<Line>(), Vector3.zero, Quaternion.identity) as SCPAR.SIM.PVATestbed.Line);       
            if (index == 0)
            {
                lines[lines.Count - 1].initialize(fromPos + 1, endPos - 1, (float)index - 0.5f, center, isHorizontal, LineType.SolidYellow);
                for(int i=0; i< numOfLanes; i++)
                {
                    createStopLine(center, fromPos + 1.6f, (float)index+i, isHorizontal, false);
                    createStopLine(center, endPos - 2.6f, (float)index + i, isHorizontal, true);
                    createCrossing(center, fromPos+1, (float)index + i, isHorizontal, true);
                    createCrossing(center, fromPos+1, (float)index + i, isHorizontal, false);
                    createCrossing(center, endPos-2, (float)index + i, isHorizontal, true);
                    createCrossing(center, endPos-2, (float)index + i, isHorizontal, false);
                }
            }                
            else
                lines[lines.Count - 1].initialize(fromPos +1 , endPos - 1, (float)index - 0.5f, center, isHorizontal, LineType.DashWhite);
            lines[lines.Count - 1].transform.parent = transform;
        }

        public int getTurnDirection(Road other)
        {
            return 0;
            /*
            int side1 = targetSideId;
            int side2 = other.sourceSideId;

            return (side2- side1 - 1 + 8) % 4
            */
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

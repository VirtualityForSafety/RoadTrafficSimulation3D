//#define DEBUG
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SCPAR.SIM.PVATestbed
{
    public class Trajectory : MonoBehaviour
    {
        Lane lane;
        public Junction source;
        Junction target;
        public List<Road> tracks;
        public Road currentRoad;
        public Lane currentLane;
        public int currentTrackIdx;
        public int currentTurnIdx;
        public int currentChangeIdx;
        public bool isNearTurning;
        public bool isFarTurning;
        public bool willNearTurn;
        public bool willFarTurn;
        bool isDebugging = false;

        RltDirection nextDirectionRrt;
        List<Vector3> turnWaypoints;
        List<Vector3> changeWaypoints;
        GameObject waypointChildren;
        string carID;

        public bool hasTurnWaypoint() { return turnWaypoints.Count > 0;  }
        public bool hasChangeWaypoint() { return changeWaypoints.Count > 0; }

        public Vector3 getChangeDestination() { if (!hasChangeWaypoint()) return new Vector3(999, 999, 999); else return changeWaypoints[changeWaypoints.Count - 1];  }

        public void createTrajectory(Junction initialJunction, List<Road> roads, string _carID)
        {
            transform.name = "Trj_" + _carID;
            source = initialJunction;
            tracks = new List<Road>();
            AbsDirection currentDirection = AbsDirection.NA;
            int dir = 0;
            for (; dir < 4; dir++)
            {
                if (source.connectRoadIdx[dir]>=0)
                {
                    addRoad2Track(roads[source.connectRoadIdx[dir]]);
                    currentDirection = roads[source.connectRoadIdx[dir]].direction;
                    break;
                }
            }
            if (dir==4 || currentDirection == AbsDirection.NA)
                Debug.Log("ERROR : Trajectory - createTrajectory - No connected neighbor.");
            Road nextRoad = null;
            for(int i=0; i<100; i++)
            {
                int result = getNextDestination(currentDirection, tracks[tracks.Count - 1].target, roads, ref nextRoad);
                if (result < 0)
                    break;
                addRoad2Track(nextRoad);
                currentDirection = (AbsDirection)result;
            }

            initialize(_carID);
        }

        // for create trajectory while saving data
        public void createTrajectory(Junction initialJunction, List<Road> roads, string _carID, ref List<int> trackIdx)
        {
            if (trackIdx == null)
                trackIdx = new List<int>();
            transform.name = "Trj_" + _carID;
            source = initialJunction;
            tracks = new List<Road>();
            AbsDirection currentDirection = AbsDirection.NA;
            int dir = 0;
            for (; dir < 4; dir++)
            {
                if (source.connectRoadIdx[dir] >= 0)
                {
                    addRoad2Track(roads[source.connectRoadIdx[dir]]);
                    trackIdx.Add(source.connectRoadIdx[dir]);
                    currentDirection = roads[source.connectRoadIdx[dir]].direction;
                    break;
                }
            }
            if (dir == 4 || currentDirection == AbsDirection.NA)
                Debug.Log("ERROR : Trajectory - createTrajectory - No connected neighbor.");
            Road nextRoad = null;
            for (int i = 0; i < 100; i++)
            {
                int result = getNextDestination(currentDirection, tracks[tracks.Count - 1].target, roads, ref nextRoad);
                if (result < 0)
                    break;
                addRoad2Track(nextRoad);
                trackIdx.Add(nextRoad.index);
                currentDirection = (AbsDirection)result;
            }

            initialize(_carID);
        }

        // for create trajectory from loaded data
        public void createTrajectory(Junction initialJunction, List<Road> roads, string _carID, List<int> trackIdx)
        {
            transform.name = "Trj_" + _carID;
            source = initialJunction;
            tracks = new List<Road>();
            for(int i=0; i<trackIdx.Count; i++)
                addRoad2Track(roads[trackIdx[i]]);
            initialize(_carID);
        }

        public bool isInNearTurningPhase()
        {
            return isNearTurning;
        }

        public bool isInFarTurningPhase()
        {
            return isFarTurning;
        }

        private void initialize(string _carID)
        {
            isNearTurning = false;
            isFarTurning = false;
            currentTrackIdx = 0;
            currentChangeIdx = 0;
            currentTurnIdx = 0;
            currentRoad = tracks[currentTrackIdx];
            setCurrentLane(currentRoad.getRightmostLane());
            waypointChildren = new GameObject();
            waypointChildren.transform.parent = transform;
            turnWaypoints = new List<Vector3>();
            changeWaypoints = new List<Vector3>();
            carID = _carID;
        }

        public Lane getCurrentLane() { return currentLane; }
        public Road getCurrentRoad() { return currentRoad; }

        public void updateCurrentLaneAtEnd()
        {
            Road nextRoad = getNextRoad();
            if (nextRoad == null)
                return;

            //Debug.Log(currentRoad.direction + "\t" + nextRoad.direction);
            // go straight
            if (nextRoad.direction == currentRoad.direction)
            {
                currentTrackIdx++;
                currentRoad = tracks[currentTrackIdx];
                
                currentLane = currentRoad.lanes[currentLane.index];
            }
            else if(Util.getRltDirection(currentRoad.direction, nextRoad.direction) == RltDirection.Left)
            {
                generateTurnWaypoint(currentLane, tracks[currentTrackIdx+1].getLeftmostLane());
                currentTrackIdx++;
                currentRoad = tracks[currentTrackIdx];
                isFarTurning = true;
                currentLane = currentRoad.getLeftmostLane();
            }
            else if (Util.getRltDirection(currentRoad.direction, nextRoad.direction) == RltDirection.Right)
            {
                generateTurnWaypoint(currentLane, tracks[currentTrackIdx + 1].getRightmostLane());
                currentTrackIdx++;
                currentRoad = tracks[currentTrackIdx];
                isNearTurning = true;
                currentLane = currentRoad.getRightmostLane();
            }
        }

        public RltDirection getNextRltDirection()
        {
            for (int i = currentTrackIdx; i < tracks.Count - 1; i++)
            {
                if (tracks[i].direction != tracks[i + 1].direction)
                {
                    if (Util.getRltDirection(tracks[i].direction, tracks[i + 1].direction) == RltDirection.Left)
                        return RltDirection.Left;
                    else if (Util.getRltDirection(tracks[i].direction, tracks[i + 1].direction) == RltDirection.Right)
                        return RltDirection.Right;
                }
            }
            return RltDirection.Straight;
        }

        public void setCurrentLane(Lane lane)
        {
            currentLane = lane;
            if (lane.getRoad() != currentLane.getRoad())
                currentRoad = lane.getRoad();

            Road nextRoad = getNextRoad();            
            if(nextRoad != null)
            {
                willNearTurn = Util.getRltDirection(currentRoad.direction, nextRoad.direction) == RltDirection.Right;
                willFarTurn = Util.getRltDirection(currentRoad.direction, nextRoad.direction) == RltDirection.Left;
            }
        }

        public void turnFinished()
        {
            removeAllTurnWaypoints();
            isFarTurning = false;
            isNearTurning = false;
            willNearTurn = false;
            willFarTurn = false;
        }

        private void addRoad2Track(Road road2Added)
        {
            // for debugging
            //road2Added.transform.parent = Object.Instantiate(road2Added, road2Added.transform.position, road2Added.transform.rotation, transform).transform;
            //
            tracks.Add(road2Added);
        }

        float getPt(float n1, float n2, float perc)
        {
            float diff = n2 - n1;
            return n1 + (diff * perc);
        }

        Vector2 getBezierPoint(float x1, float y1, float x2, float y2, float x3, float y3, float i)
        {
            float xa = getPt(x1, x2, i);
            float ya = getPt(y1, y2, i);
            float xb = getPt(x2, x3, i);
            float yb = getPt(y2, y3, i);
            
            float x = getPt(xa, xb, i);
            float y = getPt(ya, yb, i);

            return new Vector2(x, y);
            //return new Vector2(x2, y2);
        }

        Vector2 getBezierPoint(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t)
        {
            float u = 1f - t;
            float t2 = t * t;
            float u2 = u * u;
            float u3 = u2 * u;
            float t3 = t2 * t;

            Vector2 result =
                (u3) * p0 +
                (3f * u2 * t) * p1 +
                (3f * u * t2) * p2 +
                (t3) * p3;

            return result;
            //return new Vector2(x2, y2);
        }

        public void generateUnitWaypoint(Lane prevLane, Lane nextLane, float factor)
        {
            Vector2 middlePoint;
            if (!prevLane.isHorizontal)
            {
                float controlPoint_x = prevLane.toPoint.x;
                float controlPoint_z = nextLane.fromPoint.z;
                if (prevLane.toPoint.z < nextLane.fromPoint.z)
                    controlPoint_z += SimParameter.unitBlockSize / 2;
                else
                    controlPoint_z -= SimParameter.unitBlockSize / 2;
                if (prevLane.toPoint.x < nextLane.fromPoint.x)
                    controlPoint_x -= SimParameter.unitBlockSize / 2;
                else
                    controlPoint_x += SimParameter.unitBlockSize / 2;
                middlePoint = getBezierPoint(prevLane.toPoint.x, prevLane.toPoint.z, controlPoint_x, controlPoint_z, nextLane.fromPoint.x, nextLane.fromPoint.z, factor);
            }
            else
            {
                float controlPoint_x = nextLane.fromPoint.x;
                float controlPoint_z = prevLane.toPoint.z;
                if (prevLane.toPoint.z < nextLane.fromPoint.z)
                    controlPoint_z -= SimParameter.unitBlockSize / 2;
                else
                    controlPoint_z += SimParameter.unitBlockSize / 2;
                if (prevLane.toPoint.x < nextLane.fromPoint.x)
                    controlPoint_x += SimParameter.unitBlockSize / 2;
                else
                    controlPoint_x -= SimParameter.unitBlockSize / 2;
                middlePoint = getBezierPoint(prevLane.toPoint.x, prevLane.toPoint.z, nextLane.fromPoint.x, prevLane.toPoint.z, nextLane.fromPoint.x, nextLane.fromPoint.z, factor);
            }
            turnWaypoints.Add(new Vector3(middlePoint.x, prevLane.toPoint.y, middlePoint.y));

            if (isDebugging)
            {
                GameObject tempWaypointObj = GameObject.Instantiate((GameObject)Resources.Load("Prefab/Waypoint"), turnWaypoints[turnWaypoints.Count - 1], Quaternion.identity, transform);
                tempWaypointObj.name = "waypoint" + (turnWaypoints.Count - 1);
                tempWaypointObj.transform.parent = waypointChildren.transform;
            }
      
        }

        public void generateUnitWaypoint(Vector3 startPt, Vector3 endPt, bool isHorizontal, float factor)
        {
            Vector3 middlePoint = (startPt+endPt)/2.0f;
            if( isHorizontal)
                middlePoint = getBezierPoint(new Vector2(startPt.x, startPt.z), new Vector2(middlePoint.x, startPt.z), new Vector2(middlePoint.x, endPt.z), new Vector2(endPt.x, endPt.z), factor);
            else
                middlePoint = getBezierPoint(new Vector2(startPt.x, startPt.z), new Vector2(startPt.x, middlePoint.z), new Vector2(endPt.x, middlePoint.z), new Vector2(endPt.x, endPt.z), factor);
            changeWaypoints.Add(new Vector3(middlePoint.x, startPt.y, middlePoint.y));
            
            if (isDebugging)
            {
                GameObject tempWaypointObj = GameObject.Instantiate((GameObject)Resources.Load("Prefab/Waypoint"), changeWaypoints[changeWaypoints.Count - 1], Quaternion.identity, transform);
                tempWaypointObj.name = "waypoint" + (changeWaypoints.Count - 1);
                tempWaypointObj.transform.parent = waypointChildren.transform;
            }
        }

        public void generateTurnWaypoint(Lane prevLane, Lane nextLane)
        {
            //Debug.Log(carID + " - generateWayPoint - for turn");
            currentTurnIdx = 0;
            for (float i = 0; i < 1.1f; i += 0.1f)
            {
                generateUnitWaypoint(prevLane, nextLane, i);
            }
        }

        public void generateChangeWaypoint(Transform currentPos, float length, Lane nextLane)
        {
            //Debug.Log(carID + " - generateWayPoint - for lane change");
            currentChangeIdx = 0;
            Vector3 nextDirection = Util.absDirection2Vec3(nextLane.direction);
            Vector3 source = currentPos.position;
            if (nextLane.isHorizontal)
            {
                Vector3 target = new Vector3(currentPos.position.x + nextDirection.x * length * 4, currentPos.position.y, nextLane.fromPoint.z);
                for (float i = 0; i < 1.1f; i += 0.1f)
                {
                    //laneWaypoints.Add());
                    generateUnitWaypoint(source, target, nextLane.isHorizontal, i);
                }
            }
            else
            {
                Vector3 target = new Vector3(nextLane.fromPoint.x, currentPos.position.y, currentPos.position.z + nextDirection.z * length * 2);
                for (float i = 0; i < 1.1f; i += 0.1f)
                {
                    //laneWaypoints.Add(new Vector3(nextLane.fromPoint.x, currentPos.position.y, currentPos.position.z + nextDirection.z * length * 2));
                    generateUnitWaypoint(source, target, nextLane.isHorizontal, i);
                }
            }
        }

        public Lane getLeftAdjacentLane()
        {
            return currentRoad.getLeftAdjacentLane(currentLane);
        }

        public Lane getRightAdjacentLane()
        {
            return currentRoad.getRightAdjacentLane(currentLane);
        }

        public Vector3 getTurnWaypoint()
        {
            return getTurnWaypoint(currentTurnIdx+1);
        }

        public Vector3 getChangeWaypoint()
        {
            return getChangeWaypoint(currentChangeIdx + 1);
        }

        public Vector3 getChangeWaypoint(int index)
        {
            if (!hasChangeWaypoint())
                return new Vector3(9999, 9999, 9999);
            else if (index < 0 || index >= changeWaypoints.Count)
                return changeWaypoints[changeWaypoints.Count - 1];
            else
                return changeWaypoints[index];
        }

        public Vector3 getTurnWaypoint(int index)
        {
            if (!hasTurnWaypoint())
                return new Vector3(9999, 9999, 9999);
            else if (index < 0 || index >= turnWaypoints.Count)
                return turnWaypoints[turnWaypoints.Count - 1];
            else
                return turnWaypoints[index];
        }

        public void move2NextChangeWaypoint()
        {
            if (currentChangeIdx >= 0 && currentChangeIdx < changeWaypoints.Count)
                currentChangeIdx++; //removeTurnWaypoint(currentTurnIdx++);
            if (currentChangeIdx >= changeWaypoints.Count)
                removeAllChangeWaypoints();
        }

        public void move2NextTurnWaypoint()
        {
            if (currentTurnIdx >= 0 && currentTurnIdx < turnWaypoints.Count)
                currentTurnIdx++; //removeTurnWaypoint(currentTurnIdx++);
            if(currentTurnIdx>=turnWaypoints.Count)
                removeAllTurnWaypoints();
        }

        public void removeTurnWaypoint(int index)
        {
            Debug.Log("removeTurnWaypoint - " + index);
            if (!hasTurnWaypoint() || index < 0 || index >= turnWaypoints.Count)
                return;
            if (isDebugging)
            {
                Destroy(transform.Find("waypoint" + index).gameObject);
            }
            turnWaypoints.RemoveAt(index);
        }

        public void removeAllChangeWaypoints()
        {
            if (!hasChangeWaypoint())
                return;
            foreach (Transform child in waypointChildren.transform)
            {
                Destroy(child.gameObject);
            }

            changeWaypoints.Clear();
        }

        public void removeAllTurnWaypoints()
        {
            if (!hasTurnWaypoint())
                return;
            foreach (Transform child in waypointChildren.transform)
            {
                Destroy(child.gameObject);
            }

            turnWaypoints.Clear();
        }

        public Road getNextRoad()
        {
            if (currentTrackIdx + 1 >= tracks.Count)
                return null;
            else return tracks[currentTrackIdx + 1];
        }

        private RltDirection getRandomDirection()
        {
            float directionProb = Random.Range(0, 100);
            if (directionProb < SimParameter.probStraight)
                return RltDirection.Straight;
            else if (directionProb < SimParameter.probLeft)
                return RltDirection.Left;
            else
                return RltDirection.Right;
        }

        private RltDirection getRandomDirection(bool[] failedDirection)
        {
            int failureCount = 100;
            while(failureCount > 0)
            {
                int directionProb = Random.Range(0, 100);
                if (directionProb < SimParameter.probStraight && !failedDirection[(int)RltDirection.Straight])
                    return RltDirection.Straight;
                else if (directionProb < SimParameter.probLeft && !failedDirection[(int)RltDirection.Left])
                    return RltDirection.Left;
                else if (!failedDirection[(int)RltDirection.Right])
                    return RltDirection.Right;
            }
            return RltDirection.NA;
        }

        private int getNextDestination(AbsDirection prvDirection, Junction currentJunction, List<Road> roads, ref Road nextRoad)
        {
            if (currentJunction.isNullJunction)
                return -1;
            
            int failedDirection = 0;
            bool[] failedDirIndex = new bool[3];
            failedDirIndex[0] = false;
            failedDirIndex[1] = false;
            failedDirIndex[2] = false;

            while (failedDirection < 3)
            {
                // sample next destination
                RltDirection direction = getRandomDirection(failedDirIndex);
                if (direction == RltDirection.NA)
                {
                    Debug.Log("ERROR : Trajectory - getNextDestination - Error in generating random direction");
                    return -1;
                }
                if (direction == RltDirection.Straight)
                {
                    if (currentJunction.connectRoadIdx[(int)prvDirection]>=0)
                    {
                        nextRoad = roads[currentJunction.connectRoadIdx[(int)prvDirection]];
                        return (int)Util.addDirection(prvDirection, direction);
                    }
                    else
                    {
                        failedDirIndex[(int)RltDirection.Straight] = true;
                        failedDirection++;
                    }
                        
                }
                else if (direction == RltDirection.Left)
                {
                    if (currentJunction.connectRoadIdx[((int)prvDirection + 3) % 4] >= 0)
                    {
                        nextRoad = roads[currentJunction.connectRoadIdx[((int)prvDirection +3) % 4]];
                        return (int)Util.addDirection(prvDirection, direction);
                    }
                    else
                    {
                        failedDirIndex[(int)RltDirection.Left] = true;
                        failedDirection++;
                    }
                }
                else if (direction == RltDirection.Right)
                {
                    if (currentJunction.connectRoadIdx[((int)prvDirection + 1) % 4] >= 0)
                    {
                        nextRoad = roads[currentJunction.connectRoadIdx[((int)prvDirection + 1) % 4]];
                        return (int)Util.addDirection(prvDirection, direction);
                    }
                    else
                    {
                        failedDirIndex[(int)RltDirection.Right] = true;
                        failedDirection++;
                    }
                }
            }
            
            return -1;
        }

        void Update()
        {
            for(int i=0; i<tracks.Count; i++)
            {
                Debug.DrawLine(tracks[i].getLeftmostLane().fromPoint+Vector3.up, tracks[i].getLeftmostLane().toPoint + Vector3.up, Color.red);
            }
        }
    }
}
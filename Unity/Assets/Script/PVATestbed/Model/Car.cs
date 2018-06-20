using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace SCPAR.SIM.PVATestbed
{
    public class Car : MonoBehaviour
    {
        public string id;
        public float speed;
        float maxSpeed = 30;
        float timeHeadway = 1.5f;
        float maxAcceleration = 3;
        float maxDeceleration = 5;
        float vMin = 7;

        int termLaneChange = 100;
        int decisionCount = 0;

        Transform frontCollider;
        float s0 = 2;
        public float width;
        public float length;
        public float height;
        public bool alive = false;
        public Trajectory trajectory;
        TrafficLightManager trafficLightManager;
        public Lane preferedLane = null;
        GameObject model;
        Vector3 velocityDirection;
        
        private bool raycasting = false;        // Raycasts hits an obstacle now?
        public bool isChangingLanes;
        public bool isTurning;

        Vector3 raycastOffset;
        float[] leftDistance;
        float[] rightDistance;

        public bool ignoreWaypointNow = false;

        private float rayInput = 0f;        // Total ray input affected by raycast distances.
        
        public LayerMask carLayers = -1;

        public int wideRayLength = 20;
        public int tightRayLength = 20;
        public int sideRayLength = 3;
        public Car nextCar = null;

        float distanceToNextCarFront=9999;
        float distanceToNextCarLeft = 9999;
        float distanceToNextCarRight = 9999;

        public string getID() { return id; }

        void setIdentity()
        {
            id = "CAR"+model.GetInstanceID().ToString();
            //id = "HyunsooCar";
            this.transform.name = id;
        }

        public void initialize(Junction startingPosition, List<Road> roads, TrafficLightManager tlm, Transform trjParent)
        {
            gameObject.layer = LayerMask.NameToLayer("carLayer");
            isChangingLanes = false;
            isTurning = false;
            model = this.gameObject;
            trafficLightManager = tlm;

            setIdentity();

            trajectory = UnityEngine.Object.Instantiate((Resources.Load("Prefab/DummyObjects/DummyTrajectory") as GameObject).GetComponent<Trajectory>(), trjParent);
            trajectory.createTrajectory(startingPosition, roads, id);

            if (model.GetComponent<MeshCollider>() != null)
            {
                width = Mathf.Abs(model.GetComponent<MeshCollider>().bounds.min.x - model.GetComponent<MeshCollider>().bounds.max.x);
                length = Mathf.Abs(model.GetComponent<MeshCollider>().bounds.min.z - model.GetComponent<MeshCollider>().bounds.max.z);
                height = Mathf.Abs(model.GetComponent<MeshCollider>().bounds.min.y - model.GetComponent<MeshCollider>().bounds.max.y);
            }
            else if (model.GetComponent<BoxCollider>() != null)
            {
                width = model.GetComponent<BoxCollider>().size.x;
                height = model.GetComponent<BoxCollider>().size.y;
                length = model.GetComponent<BoxCollider>().size.z;
            }

            raycastOffset = new Vector3(width / 2 - 0.1f, 0, 0);
            alive = true;
            preferedLane = null;
            frontCollider = transform.Find("FrontCollider").transform;

            //setInitialPose(height/2);
            setInitialPose(0.2f);
            initializeDecisionCount();
        }

        public void initialize(Junction startingPosition, List<Road> roads, TrafficLightManager tlm, Transform trjParent, ref List<int> trackIdx)
        {
            gameObject.layer = LayerMask.NameToLayer("carLayer");
            isChangingLanes = false;
            isTurning = false;
            model = this.gameObject;
            trafficLightManager = tlm;

            setIdentity();

            trajectory = UnityEngine.Object.Instantiate((Resources.Load("Prefab/DummyObjects/DummyTrajectory") as GameObject).GetComponent<Trajectory>(), trjParent);
            trajectory.createTrajectory(startingPosition, roads, id, ref trackIdx);

            if (model.GetComponent<MeshCollider>() != null)
            {
                width = Mathf.Abs(model.GetComponent<MeshCollider>().bounds.min.x - model.GetComponent<MeshCollider>().bounds.max.x);
                length = Mathf.Abs(model.GetComponent<MeshCollider>().bounds.min.z - model.GetComponent<MeshCollider>().bounds.max.z);
                height = Mathf.Abs(model.GetComponent<MeshCollider>().bounds.min.y - model.GetComponent<MeshCollider>().bounds.max.y);
            }
            else if (model.GetComponent<BoxCollider>() != null)
            {
                width = model.GetComponent<BoxCollider>().size.x;
                height = model.GetComponent<BoxCollider>().size.y;
                length = model.GetComponent<BoxCollider>().size.z;
            }

            raycastOffset = new Vector3(width / 2 - 0.1f, 0, 0);
            alive = true;
            preferedLane = null;
            frontCollider = transform.Find("FrontCollider").transform;

            //setInitialPose(height/2);
            setInitialPose(0.2f);
            initializeDecisionCount();
        }

        public void initialize(Junction startingPosition, List<Road> roads, TrafficLightManager tlm, Transform trjParent, List<int> trackIdx)
        {
            gameObject.layer = LayerMask.NameToLayer("carLayer");
            isChangingLanes = false;
            isTurning = false;
            model = this.gameObject;
            trafficLightManager = tlm;

            setIdentity();

            trajectory = UnityEngine.Object.Instantiate((Resources.Load("Prefab/DummyObjects/DummyTrajectory") as GameObject).GetComponent<Trajectory>(), trjParent);
            trajectory.createTrajectory(startingPosition, roads, id, trackIdx);
            
            if (model.GetComponent<MeshCollider>() != null)
            {
                width = Mathf.Abs(model.GetComponent<MeshCollider>().bounds.min.x - model.GetComponent<MeshCollider>().bounds.max.x);
                length = Mathf.Abs(model.GetComponent<MeshCollider>().bounds.min.z - model.GetComponent<MeshCollider>().bounds.max.z);
                height = Mathf.Abs(model.GetComponent<MeshCollider>().bounds.min.y - model.GetComponent<MeshCollider>().bounds.max.y);
            }
            else if(model.GetComponent<BoxCollider>() != null)
            {
                width = model.GetComponent<BoxCollider>().size.x;
                height = model.GetComponent<BoxCollider>().size.y;
                length = model.GetComponent<BoxCollider>().size.z;
            }

            raycastOffset = new Vector3(width / 2 - 0.1f, 0, 0);
            alive = true;
            preferedLane = null;
            frontCollider = transform.Find("FrontCollider").transform;

            //setInitialPose(height/2);
            setInitialPose(0.2f);
            initializeDecisionCount();
        }

        private void setInitialPose(float vehicleHeight)
        {
            transform.position = trajectory.getCurrentLane().fromPoint + Vector3.up*vehicleHeight + Util.absDirection2Vec3(trajectory.getCurrentLane().direction)*SimParameter.unitBlockSize;// + Util.absDirection2Vec3(trajectory.getCurrentLane().direction);
            transform.rotation = Quaternion.Euler(Util.absDirection2Vec3Deg(trajectory.getCurrentLane().direction));
        }

        private float getCurPosition(Lane _lane)
        {
            return Vector3.Distance(transform.position, _lane.toPoint) / _lane.length;
        }

        public void setSpeed(float givenSpeed)
        {
            if (givenSpeed < 0)
                speed = 0;
            if (givenSpeed > maxSpeed)
                speed = maxSpeed;
        }

        public float getSpeed()
        {
            return speed;
        }

        void OnCollisionEnter(Collision col)
        {
            if (col.gameObject.GetComponent<Car>() != null && col.gameObject.GetComponent<Car>().speed==0)
            {
                Debug.Log("Collide!");
            }
        }

        public float getDistanceToStopLine()
        {
            return getDistanceToIntersection();
        }

        private float getAcceleration()
        {
            //*
            if(speed > vMin && trajectory.isInNearTurningPhase()){
                Vector3 currentAngles = transform.eulerAngles;
                float currentAngle = currentAngles.y;
                if (speed <= vMin)
                    return 0;
                else
                    return -1;
            }
            else if (speed > vMin && trajectory.isInFarTurningPhase())
            {
                Vector3 currentAngles = transform.eulerAngles;
                float currentAngle = currentAngles.y;
                if (speed <= vMin)
                    return 0;
                else
                    return -1;
            }
            else
            {
                //*/
                float distanceToNextCar = Mathf.Max(distanceToNextCarFront, 0);// Mathf.Max(Mathf.Min(Mathf.Min(distanceToNextCarFront, distanceToNextCarLeft), distanceToNextCarRight), 0);
                float a = maxAcceleration;
                float b = maxDeceleration;
                float deltaSpeed = speed - ((nextCar) != null ? nextCar.speed : 0);
                float freeRoadCoeff = Mathf.Pow((@speed / @maxSpeed), 4.0f);
                float distanceGap = s0;
                float timeGap = speed * timeHeadway;
                float breakGap = speed * deltaSpeed / (2 * Mathf.Sqrt(a * b));
                float safeDistance = distanceGap + timeGap + breakGap;
                float busyRoadCoeff = Mathf.Pow((safeDistance / distanceToNextCar), 2.0f);
                float safeIntersectionDistance = 1 + timeGap + speed * speed / (2 * b);
                float intersectionCoeff = Mathf.Pow((safeIntersectionDistance / getDistanceToStopLine()), 2);
                float coeff = 1 - freeRoadCoeff - busyRoadCoeff - intersectionCoeff;
                return maxAcceleration * coeff;
            }
            //return 2;
        }

        public float move(float delta)
        {
            float acceleration = getAcceleration();
            speed += acceleration * delta;
            //*
            
            //*/
            float step = speed * delta + 0.5f * acceleration * delta * delta;
            // TODO: hacks, should have changed speed
            //console.log 'bad IDM' if @trajectory.nextCarDistance.distance < step

            return step;
        }

        void changeLane(Lane _nextLane)
        {
            if (isChangingLanes)
                return;
            if (_nextLane == null)
                return;
            if (_nextLane == trajectory.getCurrentLane())
                return;
            //if(nextPosition = @current.position + 3 * @car.length
            //throw Error 'too late to change lane' unless nextPosition < @lane.length)
            startChangingLanes(_nextLane);
        }

        void finishChangingLane()
        {
            isChangingLanes = false;
            trajectory.removeAllChangeWaypoints();
        }

        void startChangingLanes(Lane _nextLane)
        {
            if (isChangingLanes)
                return;
            isChangingLanes = true;

            trajectory.generateChangeWaypoint(this.transform, length, _nextLane);

            updateCurrentLane(preferedLane);
        }

        RltDirection getTurnDirection()
        {
            if (trajectory.getNextRoad() == null)
                return RltDirection.NA;
            return Util.getRltDirection(trajectory.currentRoad.direction, trajectory.getNextRoad().direction);
        }

        void setSpeed()
        {
            Vector3 offset = transform.forward * move(Time.deltaTime);

            // very important!!
            // should be debugged
            if (!float.IsNaN(offset.x) && !float.IsNaN(offset.y) && !float.IsNaN(offset.z) && !float.IsInfinity(offset.x) && !float.IsInfinity(offset.y) && !float.IsInfinity(offset.z))
                this.transform.position += offset; 
            else
                alive = false;
        }

        bool willTurnSoon()
        {
            return trajectory.getCurrentLane().direction != trajectory.getNextRoad().direction;
        }

        float getDistanceToTurningPoint()
        {
            Vector3 destination = trajectory.tracks[trajectory.tracks.Count - 1].getLeftmostLane().toPoint;
            for (int i = trajectory.currentTrackIdx; i < trajectory.tracks.Count - 1; i++)
            {
                if (trajectory.tracks[i].direction != trajectory.tracks[i + 1].direction)
                {
                    if (Util.getRltDirection(trajectory.tracks[i].direction, trajectory.tracks[i + 1].direction) == RltDirection.Left)
                        destination = trajectory.tracks[i].getLeftmostLane().toPoint;
                    else if (Util.getRltDirection(trajectory.tracks[i].direction, trajectory.tracks[i + 1].direction) == RltDirection.Right)
                        destination = trajectory.tracks[i].getRightmostLane().toPoint;
                }
            }
            float distance = Vector3.Distance(frontCollider.position, destination) - length / 2; //Mathf.Abs(trajectory.getCurrentLane().length - length / 2 - );

            return distance;
        }

        float getDistanceToIntersection()
        {
            float distance = Vector3.Distance(frontCollider.position, trajectory.currentLane.toPoint) - 0.2f; //Mathf.Abs(trajectory.getCurrentLane().length - length / 2 - );
            //Debug.Log(distance);
            if (shouldStop())
                return distance;
            else
                return 9999.0f;
            /*
            if (isChangingLanes)
                return 9999.0f;
            else
                return distance;
                */
        }

        bool shouldStop()
        {
            if (trajectory.isNearTurning || trajectory.willNearTurn)
                return false;
            else if (trafficLightManager.getCurrentState(trajectory.getCurrentLane().direction) == TrafficState.CarGoPedStop)
                return false;
            else
                return true;
        }

        void visualizeCollider()
        {
            Vector3 forward = transform.TransformDirection(new Vector3(0, 0, 1));
            Vector3 pivotPos = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);
            RaycastHit hit;

            ////////////////////////////////////////////
            // 운전자 시점을 고려해서 업데이트
            ////////////////////////////////////////////
            /*
            Debug.DrawRay(pivotPos, Quaternion.AngleAxis(25, transform.up) * forward * wideRayLength, Color.white);
            Debug.DrawRay(pivotPos, Quaternion.AngleAxis(-25, transform.up) * forward * wideRayLength, Color.white);
            Debug.DrawRay(pivotPos, Quaternion.AngleAxis(90, transform.up) * forward * sideRayLength, Color.white);
            Debug.DrawRay(pivotPos, Quaternion.AngleAxis(-90, transform.up) * forward * sideRayLength, Color.white);
            */

            Debug.DrawRay(pivotPos + transform.right, Quaternion.AngleAxis(0, transform.up) * forward * tightRayLength, Color.white);
            Debug.DrawRay(pivotPos - transform.right, Quaternion.AngleAxis(-0, transform.up) * forward * tightRayLength, Color.white);
            

            // New bools effected by fixed raycasts.
            bool tightTurn = false;
            bool wideTurn = false;
            bool sideTurn = false;
            bool tightTurn1 = false;
            bool wideTurn1 = false;
            bool sideTurn1 = false;

            // New input steers effected by fixed raycasts.
            float newinputSteer0 = 0f;
            float newinputSteer1 = 0f;
            float newinputSteer2 = 0f;
            float newinputSteer3 = 0f;
            float newinputSteer4 = 0f;
            float newinputSteer5 = 0f;
            nextCar = null;
            Car[] nextCars = new Car[6];
            distanceToNextCarFront = 9999;
            distanceToNextCarLeft = 9999;
            distanceToNextCarRight = 9999;

            // Front Raycasts.
            if (Physics.Raycast(pivotPos  + raycastOffset, Quaternion.AngleAxis(0, transform.up) * forward, out hit, tightRayLength, carLayers) && !hit.collider.isTrigger && hit.transform.root != transform && hit.collider.GetComponent<Car>() !=null)
            {
                Debug.DrawRay(pivotPos + raycastOffset, Quaternion.AngleAxis(0, transform.up) * forward * tightRayLength, Color.red);
                //newinputSteer0 = Mathf.Lerp(-1f, 0f, (hit.distance / tightRayLength));
                newinputSteer0 = hit.distance;
                tightTurn = true;
                nextCars[0] = hit.collider.GetComponent<Car>();
            }
            else
            {
                newinputSteer0 = 9999f;
                tightTurn = false;
            }

            if (Physics.Raycast(pivotPos - raycastOffset, Quaternion.AngleAxis(-0, transform.up) * forward, out hit, tightRayLength, carLayers) && !hit.collider.isTrigger && hit.transform.root != transform && hit.collider.GetComponent<Car>() != null)
            {
                Debug.DrawRay(pivotPos - raycastOffset, Quaternion.AngleAxis(-0, transform.up) * forward * tightRayLength, Color.red);
                //newinputSteer1 = Mathf.Lerp(1f, 0f, (hit.distance / tightRayLength));
                newinputSteer1 = hit.distance;
                tightTurn1 = true;
                nextCars[1] = hit.collider.GetComponent<Car>();
            }
            else
            {
                newinputSteer1 = 9999f;
                tightTurn1 = false;
            }

            /*
            // Wide Raycasts.
            if (Physics.Raycast(pivotPos, Quaternion.AngleAxis(25, transform.up) * forward, out hit, wideRayLength, carLayers) && !hit.collider.isTrigger && hit.transform.root != transform && hit.collider.GetComponent<Car>() != null)
            {
                //Debug.Log(Vector3.Distance(hit.collider.transform.position, pivotPos));
                Debug.DrawRay(pivotPos, Quaternion.AngleAxis(25, transform.up) * forward * wideRayLength, Color.red);
                //newinputSteer2 = Mathf.Lerp(-.5f, 0f, (hit.distance / wideRayLength));
                newinputSteer2 = hit.distance;
                wideTurn = true;
                nextCars[2] = hit.collider.GetComponent<Car>();
            }

            else
            {
                newinputSteer2 = 0f;
                wideTurn = false;
            }

            // Side Raycasts.
            if (Physics.Raycast(pivotPos, Quaternion.AngleAxis(90, transform.up) * forward, out hit, sideRayLength, carLayers) && !hit.collider.isTrigger && hit.transform.root != transform && hit.collider.GetComponent<Car>() != null)
            {
                Debug.DrawRay(pivotPos, Quaternion.AngleAxis(90, transform.up) * forward * sideRayLength, Color.red);
                //newinputSteer4 = Mathf.Lerp(-1f, 0f, (hit.distance / sideRayLength));
                newinputSteer4 = hit.distance;
                sideTurn = true;
                nextCars[4] = hit.collider.GetComponent<Car>();
            }
            else
            {
                newinputSteer4 = 0f;
                sideTurn = false;
            }

            if (Physics.Raycast(pivotPos, Quaternion.AngleAxis(-25, transform.up) * forward, out hit, wideRayLength, carLayers) && !hit.collider.isTrigger && hit.transform.root != transform && hit.collider.GetComponent<Car>() != null)
            {
                //Debug.Log(Vector3.Distance(hit.collider.transform.position, pivotPos));
                Debug.DrawRay(pivotPos, Quaternion.AngleAxis(-25, transform.up) * forward * wideRayLength, Color.red);
                //newinputSteer3 = Mathf.Lerp(.5f, 0f, (hit.distance / wideRayLength));
                newinputSteer3 = hit.distance;
                wideTurn1 = true;
                nextCars[3] = hit.collider.GetComponent<Car>();
            }
            else
            {
                newinputSteer3 = 0f;
                wideTurn1 = false;
            }

            if (Physics.Raycast(pivotPos, Quaternion.AngleAxis(-90, transform.up) * forward, out hit, sideRayLength, carLayers) && !hit.collider.isTrigger && hit.transform.root != transform && hit.collider.GetComponent<Car>() != null)
            {
                Debug.DrawRay(pivotPos, Quaternion.AngleAxis(-90, transform.up) * forward * sideRayLength, Color.red);
                //newinputSteer5 = Mathf.Lerp(1f, 0f, (hit.distance / sideRayLength));
                newinputSteer5 = hit.distance;
                sideTurn1 = true;
                nextCars[5] = hit.collider.GetComponent<Car>();
            }
            else
            {
                newinputSteer5 = 0f;
                sideTurn1 = false;
            }
            */

            float btwCarGap = - length/2;

            distanceToNextCarFront = Mathf.Min(newinputSteer0 + btwCarGap, newinputSteer1 + btwCarGap);
            /*
            if(tightTurn && !tightTurn1)
            {
                nextCar = nextCars[0];
                distanceToNextCarFront = newinputSteer0 + btwCarGap;
            }
            else if (!tightTurn && tightTurn1)
            {
                nextCar = nextCars[1];
                distanceToNextCarFront = newinputSteer1 + btwCarGap;
            }
            else if (tightTurn && tightTurn1)
            {
                nextCar = newinputSteer0 < newinputSteer1 ? nextCars[0] : nextCars[1];
                distanceToNextCarFront = Mathf.Min(newinputSteer0 + btwCarGap, newinputSteer1 + btwCarGap);
            }
            */

            if (trajectory.isFarTurning && isChangingLanes) {
                if (wideTurn && !sideTurn)
                {
                    nextCar = nextCars[2];
                    distanceToNextCarRight = newinputSteer2 + btwCarGap;
                }
                else if (!wideTurn && sideTurn)
                {
                    nextCar = nextCars[4];
                    distanceToNextCarRight = newinputSteer4 + btwCarGap;
                }
                else if (wideTurn && sideTurn)
                {
                    nextCar = newinputSteer2 < newinputSteer4 ? nextCars[2] : nextCars[4];
                    distanceToNextCarRight = Mathf.Min(newinputSteer2 + btwCarGap, newinputSteer4 + btwCarGap);
                }
            }
            
            if(trajectory.isNearTurning && isChangingLanes)
            {
                if (wideTurn1 && !sideTurn1)
                {
                    nextCar = nextCars[3];
                    distanceToNextCarLeft = newinputSteer3 + btwCarGap;
                }
                else if (!wideTurn1 && sideTurn1)
                {
                    nextCar = nextCars[5];
                    distanceToNextCarLeft = newinputSteer5 + btwCarGap;
                }
                else if (wideTurn1 && sideTurn1)
                {
                    nextCar = newinputSteer3 < newinputSteer5 ? nextCars[3] : nextCars[5];
                    distanceToNextCarLeft = Mathf.Min(newinputSteer3 + btwCarGap, newinputSteer5 + btwCarGap);
                }
            }
            
            /*
            // Raycasts hits an obstacle now?
            if (wideTurn || wideTurn1 || sideTurn || sideTurn1)
                raycasting = true;
            else
                raycasting = false;

            // If raycast hits a collider, feed rayInput.
            if (raycasting)
                rayInput = (newinputSteer1 + newinputSteer2 + newinputSteer3 + newinputSteer4 + newinputSteer5 + newinputSteer0);
            else
            {
                rayInput = 0f;
            }

            // If rayInput is too much, ignore navigator input.
            if (raycasting && Mathf.Abs(rayInput) > .5f)
                ignoreWaypointNow = true;
            else
                ignoreWaypointNow = false;
                */
        }

        public void setAligned()
        {
            if (isTurning)
                return;
            float currentLerpDistance = 0.03f;
            float distanceBtwCenter = 0;
            //*
            if (preferedLane!=null && preferedLane.isHorizontal)
            {
                distanceBtwCenter = Mathf.Abs(preferedLane.fromPoint.z - transform.position.z);
                //transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, transform.position.y, preferedLane.fromPoint.z), currentLerpDistance);
            }

            else if (preferedLane != null && !preferedLane.isHorizontal)
            {
                distanceBtwCenter = Mathf.Abs(preferedLane.fromPoint.x - transform.position.x);
                //transform.position = Vector3.Lerp(transform.position, new Vector3(preferedLane.fromPoint.x, transform.position.y, transform.position.z), currentLerpDistance);
            }
            else if (trajectory.getCurrentLane().isHorizontal)
            {
                distanceBtwCenter = Mathf.Abs(trajectory.getCurrentLane().fromPoint.z- transform.position.z);
            }
            else if (!trajectory.getCurrentLane().isHorizontal)
            {
                distanceBtwCenter = Mathf.Abs(trajectory.getCurrentLane().fromPoint.x - transform.position.x);
            }
            //Debug.Log(distanceBtwCenter);
            //*/
            if (isChangingLanes)
            {
                if (trajectory.hasChangeWaypoint())
                {
                    //*
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(trajectory.getChangeWaypoint() - transform.position), currentLerpDistance);
                    //Debug.Log(trajectory.currentTurnIdx + "\t" + Vector3.Distance(transform.position, trajectory.getTurnWaypoint()));
                    if (Vector3.Distance(transform.position, trajectory.getChangeWaypoint()) < 2.0f)
                        trajectory.move2NextChangeWaypoint();

                    //transform.rotation = Quaternion.LookRotation(trajectory.getChangeDestination() - transform.position);
                }

                if(Vector3.Distance(trajectory.getChangeDestination(),transform.position) < 3.0f)
                {
                    finishChangingLane();
                }
            }
            else if(distanceBtwCenter>0.1f && !isAtTheEnd())
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(trajectory.getCurrentLane().toPoint - transform.position), 0.1f* Mathf.Min(distanceBtwCenter,1.0f));
            else
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(Util.absDirection2Vec3Deg(trajectory.getCurrentLane().direction)),0.1f);
        }

        void turn()
        {
            float currentLerpDistance = 0.2f;
            //transform.position = Vector3.Lerp(transform.position, trajectory.getCurrentLane().fromPoint, currentLerpDistance);

            //transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(trajectory.getCurrentLane().fromPoint - transform.position), currentLerpDistance);
            
            if (trajectory.hasTurnWaypoint())
            {
                //*
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(trajectory.getTurnWaypoint() - transform.position), currentLerpDistance);
                //Debug.Log(trajectory.currentTurnIdx + "\t" + Vector3.Distance(transform.position, trajectory.getTurnWaypoint()));
                if (Vector3.Distance(frontCollider.position, trajectory.getTurnWaypoint()) < 1.5f)
                    trajectory.move2NextTurnWaypoint();
                
            }
            else
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(trajectory.getCurrentLane().fromPoint - transform.position), currentLerpDistance);
            
            //Debug.Log(Vector3.Distance(transform.position, trajectory.getCurrentLane().fromPoint));
            if (Vector3.Distance(frontCollider.position, trajectory.getCurrentLane().fromPoint) < 1.5f)
            {
                isTurning = false;
                trajectory.turnFinished();
            }
        }

        bool shouldPrepareTurn()
        {
            return (getDistanceToTurningPoint() < length*4.0f ) ? true : false;
        }

        void updateCurrentLane(Lane lane)
        {
            trajectory.setCurrentLane(lane);
            preferedLane = null;
        }

        void initializeDecisionCount()
        {
            decisionCount = termLaneChange + UnityEngine.Random.Range(0, 30);
        }

        void prepareChangingLane(bool shouldTurn)
        {
            RltDirection turnDir = trajectory.getNextRltDirection();
            //Debug.Log(isChangingLanes.ToString() + turnDir.ToString());

            if (!isChangingLanes && turnDir != RltDirection.NA)
            {
                switch (turnDir)
                {
                    case RltDirection.Left:
                        if(shouldTurn)
                            preferedLane = trajectory.getCurrentRoad().getLeftmostLane();
                        else if(trajectory.currentLane != trajectory.getCurrentRoad().getLeftmostLane())
                        {
                            preferedLane = trajectory.getLeftAdjacentLane();
                        }                        
                        break;
                    case RltDirection.Right:
                        if(shouldTurn)
                            preferedLane = trajectory.getCurrentRoad().getRightmostLane();
                        else if (trajectory.currentLane != trajectory.getCurrentRoad().getRightmostLane())
                        {
                            preferedLane = trajectory.getRightAdjacentLane();
                        }                        
                        break;
                    default:
                        preferedLane = trajectory.getCurrentLane();
                        break;
                }
                if (preferedLane != trajectory.getCurrentLane())
                {
                    changeLane(preferedLane);
                }
            }
        }

        void changeLaneForTurn()
        {
            if (decisionCount < 0)
            {
                prepareChangingLane(true);
                initializeDecisionCount();
            }
            else if (decisionCount < 0)
            {
                prepareChangingLane(false);
                initializeDecisionCount();
            }
            decisionCount--;
        }

        public void removeTrajectory(Transform trjParent)
        {
            Destroy(trjParent.transform.Find("Trj_" + id).gameObject);
        }

        bool isAtTheEnd()
        {
            return (Vector3.Distance(frontCollider.position, trajectory.getCurrentLane().toPoint) < 2.0f + SimParameter.unitBlockSize*2) ? true : false;
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (transform.position.y < 0 || transform.position.y > 10)
                alive = false;
            if (alive)
            {
                if (!shouldStop() && isAtTheEnd())
                {
                    //Debug.Log("isAtTheEnd");
                    trajectory.updateCurrentLaneAtEnd();
                    if(trajectory.isFarTurning || trajectory.isNearTurning)
                    {
                        isTurning = true;
                        preferedLane = null;
                    }
                        

                }

                if (isTurning)
                    turn();
                else
                {
                    if (shouldPrepareTurn())
                    {
                        decisionCount = -1;
                    }
                    changeLaneForTurn();
                }
                setAligned();
                setSpeed();
                visualizeCollider();
            }

        }
    }
}
 
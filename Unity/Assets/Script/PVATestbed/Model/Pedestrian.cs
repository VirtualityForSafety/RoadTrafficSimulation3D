using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace SCPAR.SIM.PVATestbed
{
    public class Pedestrian : MonoBehaviour
    {
        string id;
        public float speed = 1.0f;
        public bool alive = false;
        
        GameObject carObject;

        public Vector3 relativePosition;

        public void setIdentity()
        {
            id = "PED" + this.GetInstanceID().ToString();
            this.transform.name = id;
        }

        public void initialize()
        {
            setIdentity();
            alive = true;
        }

        /*
        public void initialize(Junction startingPosition, World world)
        {
            model = this.gameObject;
            
            setIdentity();

            setInitialPose(startingPosition, world);
        }

        private void setInitialPose(Junction source, World world)
        {
            Vector3 currentPosition = Vector3.zero;
            AbsDirection currentDirection = AbsDirection.NA;
            int dir = 0;
            for (; dir < 4; dir++)
            {
                if (source.connectRoadIdx[dir] >= 0)
                {
                    currentPosition = world.roads[source.connectRoadIdx[dir]].getRightmostLane().fromPoint;
                    currentDirection = world.roads[source.connectRoadIdx[dir]].direction;
                    break;
                }
            }
            transform.position = currentPosition + Vector3.right * SimParameter.unitBlockSize + Vector3.up * 0.7f + Vector3.forward * 40.0f; ;
            transform.rotation = Util.absDirection2Quat(currentDirection);
        }
        */

        // Update is called once per frame
        void FixedUpdate()
        {
            if (transform.position.y < 0)
                alive = false;
            if (alive)
            {
                float step = speed * Time.deltaTime;
                if (carObject != null)
                    relativePosition = carObject.transform.position - this.transform.position;

                this.transform.position += transform.forward * step;
            }

        }
    }
}
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SCPAR.SIM.DataLogging;

namespace SCPAR.SIM.PVATestbed
{
    public class PedestrianPositioning : MonoBehaviour {
        public World world;
        public bool isReady = false;
        float speed = 1.0f;
        public int startJunctionIndex = 2;
        public bool colliderTest = false;
        // Use this for initialization
        void Start() {
            world = GameObject.Find("World").GetComponent<World>();
        }

        public void initialize()
        {
            float leftOrRight = 0;
            this.transform.position = world.junctions[startJunctionIndex].centerWorld + Vector3.forward * SimParameter.unitBlockSize * 2.7f + Vector3.right * SimParameter.unitBlockSize * (leftOrRight < 0.5 ? +2 : -3);
            if(colliderTest)
                this.transform.position += -Vector3.right * 6.0f;
            if (GetComponent<PedestrianCamera>()!=null)
                this.transform.position += Vector3.up * 1.7f;
            isReady = true;
        }
        
        // Update is called once per frame
        void Update() {

            if (isReady)
            {
                float step = speed * Time.deltaTime;
                //if (GetComponent<PedestrianCamera>() != null)
                    this.transform.position += -transform.forward * step;
                
            }
            else
            {
                if (world.isReady)
                {
                    initialize();
                    //float step = speed * Time.deltaTime;
                    //this.transform.position += -transform.forward * step * 16999;
                }
            }
        }
    }
}

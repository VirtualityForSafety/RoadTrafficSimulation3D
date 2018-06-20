using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SCPAR.SIM.PVATestbed
{
    public class JunctionSkyCamera : MonoBehaviour
    {
        public float height = 100;

        public GameObject targetCar;
        public World world;
        public Junction targetJunction;
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (world.isReady)
                targetJunction = world.junctions[0];
            if(targetJunction != null)
            {
                this.transform.position = new Vector3(targetJunction.centerWorld.x, targetJunction.centerWorld.y + height, targetJunction.centerWorld.z);
            }
            else if (targetCar != null)
                this.transform.position = new Vector3(targetCar.transform.position.x, targetCar.transform.position.y + height, targetCar.transform.position.z);
            
        }
    }
}
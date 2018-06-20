using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SCPAR.SIM.PVATestbed
{
    public class WorldGenerator : MonoBehaviour
    {
        public int loopNum = 3; // odd number is recommended
        public World world;

        // Use this for initialization
        void Start()
        {
            if(world==null)
                world = this.GetComponent<World>();
            world.createMap(loopNum);

            world.operate(); // for test
        }
        
        // Update is called once per frame
        void Update()
        {
            //if(world.isReady)
                //world.operate();
        }
    }
}


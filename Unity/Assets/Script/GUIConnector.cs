using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SCPAR.SIM.PVATestbed
{
    public class GUIConnector : MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void adjustStraightProb(float value)
        {
            SimParameter.probStraight = (int)value;
        }

        public void adjustLeftProb(float value)
        {
            SimParameter.probLeft = (int)value;
        }
    }
}


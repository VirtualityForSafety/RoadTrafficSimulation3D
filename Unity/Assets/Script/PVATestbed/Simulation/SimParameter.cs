using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SCPAR.SIM.PVATestbed
{
    public static class SimParameter
    {
        public static float unitBlockSize = 4.0f;
        public static float timeFactor = 1.0f;
        public static float areaHeight = 0.2f;
        public static float blockHeightHalf = 0.5f;

        public static int trafficLightInterval = 300;
        public static int warningInterval = 80;
        public static int crossingBlinkInterval = 20;

        public static int probStraight = 60;
        public static int probLeft = 80;
    }
}
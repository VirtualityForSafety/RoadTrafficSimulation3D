 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

namespace SCPAR.SIM.DataLogging
{
    public class DataLabel
    {
        public string id;
        public float distance;
        public float collisionProb;
        public Vector3 relativeDistance;
        public Rect region;

        public DataLabel(string i, Vector3 rD, Rect r, float d, float c)
        {
            id = i;
            relativeDistance = rD;
            region = r;
            collisionProb = c;
            distance = d;
        }

        public override string ToString()
        {
            return id + "," + region.xMin + "," + region.yMin + "," + region.xMax + "," + region.yMax + ","
                        + relativeDistance.x + "," + relativeDistance.y + "," + relativeDistance.z + "," + collisionProb;
        }
    }
}
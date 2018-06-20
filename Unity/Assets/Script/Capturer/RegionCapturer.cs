 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

namespace SCPAR.SIM.DataLogging
{
    public class RegionCapturer
    {
        public bool isCapturing = true;
        public bool isReady = false;
        
        public Rect getScreenRect(Camera cam, Collider collider, float screenHeight)
        {
            Vector3 cen = collider.bounds.center;
            Vector3 ext = collider.bounds.extents;

            Vector2 min = cam.WorldToScreenPoint(new Vector3(cen.x - ext.x, cen.y - ext.y, cen.z - ext.z));
            Vector2 max = min;

            //0
            Vector2 point = min;
            get_minMax(point, ref min, ref max);

            //1
            point = cam.WorldToScreenPoint(new Vector3(cen.x + ext.x, cen.y - ext.y, cen.z - ext.z));
            get_minMax(point, ref min, ref max);


            //2
            point = cam.WorldToScreenPoint(new Vector3(cen.x - ext.x, cen.y - ext.y, cen.z + ext.z));
            get_minMax(point, ref min, ref max);

            //3
            point = cam.WorldToScreenPoint(new Vector3(cen.x + ext.x, cen.y - ext.y, cen.z + ext.z));
            get_minMax(point, ref min, ref max);

            //4
            point = cam.WorldToScreenPoint(new Vector3(cen.x - ext.x, cen.y + ext.y, cen.z - ext.z));
            get_minMax(point, ref min, ref max);

            //5
            point = cam.WorldToScreenPoint(new Vector3(cen.x + ext.x, cen.y + ext.y, cen.z - ext.z));
            get_minMax(point, ref min, ref max);

            //6
            point = cam.WorldToScreenPoint(new Vector3(cen.x - ext.x, cen.y + ext.y, cen.z + ext.z));
            get_minMax(point, ref min, ref max);

            //7
            point = cam.WorldToScreenPoint(new Vector3(cen.x + ext.x, cen.y + ext.y, cen.z + ext.z));
            get_minMax(point, ref min, ref max);

            return new Rect(min.x, screenHeight - max.y, max.x - min.x, max.y - min.y);
        }

        void get_minMax(Vector2 point, ref Vector2 min, ref Vector2 max)
        {
            min = new Vector2(min.x >= point.x ? point.x : min.x, min.y >= point.y ? point.y : min.y);
            max = new Vector2(max.x <= point.x ? point.x : max.x, max.y <= point.y ? point.y : max.y);
        }
    }
}
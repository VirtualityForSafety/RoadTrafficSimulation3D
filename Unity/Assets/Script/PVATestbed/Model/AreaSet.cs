using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SCPAR.SIM.PVATestbed
{
    public class AreaSet:Object
    {
        List<Area> areas;
        Vector2 areaSetIndex;
        public Rect region;

        public AreaSet(Area area)
        {
            addArea(area);
        }

        public void addArea(Area areaToBeAdded)
        {            
            if (region.width == 0) // if the region is empty
                region = areaToBeAdded.region;
            else
                region = Rect.MinMaxRect(Mathf.Min(region.xMin, areaToBeAdded.region.xMin), Mathf.Min(region.yMin, areaToBeAdded.region.yMin), Mathf.Max(region.xMax, areaToBeAdded.region.xMax), Mathf.Max(region.yMax, areaToBeAdded.region.yMax));
        }
        
    }
    
}


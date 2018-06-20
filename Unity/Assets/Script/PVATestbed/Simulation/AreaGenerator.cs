using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SCPAR.SIM.PVATestbed
{
    public class AreaGenerator
    {
        List<Vector2> directions;

        public AreaGenerator() { directions = Util.getDirections(); }

        public void checkNeighbor(Area area, Vector2 junctionIndex, ref List<Junction> junctions, ref JunctionIndexer indexer, ref List<AreaSet> areaSet)
        {
            if (area.areaPosition == AreaPosition.NE)
            {
                if (!checkByDirection(junctionIndex, (int)AbsDirection.N, area, (int)AreaPosition.SE, ref junctions, ref indexer, ref areaSet) &&
                    !checkByDirection(junctionIndex, (int)AbsDirection.E, area, (int)AreaPosition.NW, ref junctions, ref indexer, ref areaSet))
                {
                    createNewAreaSet(area, ref areaSet);
                }
            }
            else if (area.areaPosition == AreaPosition.NW)
            {
                if (!checkByDirection(junctionIndex, (int)AbsDirection.N, area, (int)AreaPosition.SW, ref junctions, ref indexer, ref areaSet) &&
                    !checkByDirection(junctionIndex, (int)AbsDirection.W, area, (int)AreaPosition.NE, ref junctions, ref indexer, ref areaSet))
                {
                    createNewAreaSet(area, ref areaSet);
                }
            }
            else if (area.areaPosition == AreaPosition.SE)
            {
                if (!checkByDirection(junctionIndex, (int)AbsDirection.S, area, (int)AreaPosition.NE, ref junctions, ref indexer, ref areaSet) &&
                    !checkByDirection(junctionIndex, (int)AbsDirection.E, area, (int)AreaPosition.SW, ref junctions, ref indexer, ref areaSet))
                {
                    createNewAreaSet(area, ref areaSet);
                }
            }
            else if (area.areaPosition == AreaPosition.SW)
            {
                if (!checkByDirection(junctionIndex, (int)AbsDirection.S, area, (int)AreaPosition.NW, ref junctions, ref indexer, ref areaSet) &&
                    !checkByDirection(junctionIndex, (int)AbsDirection.W, area, (int)AreaPosition.SE, ref junctions, ref indexer, ref areaSet))
                {
                    createNewAreaSet(area, ref areaSet);
                }
            }
        }

        private void createNewAreaSet(Area area, ref List<AreaSet> areaSet)
        {
            areaSet.Add(new AreaSet(area));
            area.setAreaSetIndex(areaSet.Count - 1);
        }

        private bool checkByDirection(Vector2 junctionIndex, int direction, Area area, int targetPosition, ref List<Junction> junctions, ref JunctionIndexer indexer, ref List<AreaSet> areaSet)
        {
            bool isMerged = false;
            int neighborIndex = indexer.getIndex((directions[direction] + junctionIndex));
            if (indexer.getIndex((directions[direction] + junctionIndex)) >= 0)
            {

                int index = junctions[neighborIndex].areas[targetPosition].areaSetIndex;
                if (index >= 0)
                {
                    areaSet[index].addArea(area);
                    area.setAreaSetIndex(index);
                    isMerged = true;
                }
                else
                {
                    Debug.Log("ERROR: World - connectJunctions - Negative Index error");
                }
            }
            return isMerged;
        }
    }


    
}
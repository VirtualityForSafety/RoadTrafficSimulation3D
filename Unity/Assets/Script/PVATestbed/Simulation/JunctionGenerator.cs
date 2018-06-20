using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SCPAR.SIM.PVATestbed
{
    public class JunctionGenerator
    {
        List<Vector2> directions;
        AreaGenerator areaGenerator;

        public JunctionGenerator() { directions = Util.getDirections(); areaGenerator = new AreaGenerator(); }

        public JunctionSize setRandomParam(JunctionSize givenSize)
        {
            JunctionSize size = new JunctionSize();
            if (givenSize.numOfHrzLane == 0)
                //size.numOfHrzLane = (int)Mathf.Round(Random.Range(1.0f, 3.0f));
                size.numOfHrzLane = 2;
            else
                size.numOfHrzLane = givenSize.numOfHrzLane;

            if (givenSize.numOfVtcLane == 0)
                //size.numOfVtcLane = (int)Mathf.Round(Random.Range(1.0f, 3.0f));
                size.numOfVtcLane = 2;
            else
                size.numOfVtcLane = givenSize.numOfVtcLane;

            if (givenSize.lenOfHrzLane == 0)
                size.lenOfHrzLane = 14;//(int)Mathf.Round(Random.Range(size.numOfHrzLane * 2 + 4.0f, 15.0f)) * 2;
            else
                size.lenOfHrzLane = givenSize.lenOfHrzLane;

            if (givenSize.lenOfVtcLane == 0)
                size.lenOfVtcLane = 14; //(int)Mathf.Round(Random.Range(size.numOfVtcLane * 2 + 4.0f, 15.0f)) * 2;
            else
                size.lenOfVtcLane = givenSize.lenOfVtcLane;
            return size;
        }

        public void createJunction(Vector2 junctionCenter, JunctionSize junctionSize, Vector2 junctionIndex, ref List<Junction> junctions, ref JunctionIndexer indexer, ref List<AreaSet> areaSet, ref List<Block> sidewalks)
        {
            junctions.Add(Object.Instantiate((Resources.Load("Prefab/DummyObjects/DummyJunction") as GameObject).GetComponent<Junction>(), Vector3.zero, Quaternion.identity) as SCPAR.SIM.PVATestbed.Junction);
            junctions[junctions.Count - 1].initialize(junctionIndex, junctionCenter, junctionSize, ref sidewalks);
            junctions[junctions.Count - 1].transform.parent = GameObject.Find("World").transform;
            junctions[junctions.Count - 1].transform.name = "Junction" + (junctions.Count - 1).ToString();
            indexer.setIndex((int)junctionIndex.x, (int)junctionIndex.y, junctions.Count - 1);

            connectJunctions(junctionIndex, ref junctions, ref indexer, ref areaSet);
        }

        private void connectJunctions(Vector2 junctionIndex, ref List<Junction> junctions, ref JunctionIndexer indexer, ref List<AreaSet> areaSet)
        {
            areaGenerator.checkNeighbor(junctions[junctions.Count - 1].areas[0], junctionIndex, ref junctions, ref indexer, ref areaSet);
            areaGenerator.checkNeighbor(junctions[junctions.Count - 1].areas[1], junctionIndex, ref junctions, ref indexer, ref areaSet);
            areaGenerator.checkNeighbor(junctions[junctions.Count - 1].areas[2], junctionIndex, ref junctions, ref indexer, ref areaSet);
            areaGenerator.checkNeighbor(junctions[junctions.Count - 1].areas[3], junctionIndex, ref junctions, ref indexer, ref areaSet);
        }

        private Vector2 getNextCenter(Junction junction, JunctionSize newSize, int hrzWeight, int vtcWeight)
        {
            int prvWidth = junction.size.numOfVtcLane + junction.size.lenOfHrzLane;
            int prvHeight = junction.size.numOfHrzLane + junction.size.lenOfVtcLane;
            return new Vector2(junction.center.x + hrzWeight * (prvWidth + newSize.numOfVtcLane + newSize.lenOfHrzLane), junction.center.y + vtcWeight * (prvHeight + newSize.numOfHrzLane + newSize.lenOfVtcLane));
        }

        //private JunctionSize setNextSize(Junction junction, int hrzWeight, int vtcWeight)
        private JunctionSize getNextSize(Vector2 newJunctionIndex, ref List<Junction> junctions, ref JunctionIndexer indexer)
        {
            int hrzWeight = 0;
            int vtcWeight = 0;
            JunctionSize size = new JunctionSize();
            if (indexer.getIndex((int)newJunctionIndex.x + 1, (int)newJunctionIndex.y) >= 0)
            {
                //Debug.Log(indexer.getIndex((int)newJunctionIndex.x + 1, (int)newJunctionIndex.y));
                size.lenOfVtcLane = junctions[indexer.getIndex((int)newJunctionIndex.x + 1, (int)newJunctionIndex.y)].size.lenOfVtcLane;
                size.numOfHrzLane = junctions[indexer.getIndex((int)newJunctionIndex.x + 1, (int)newJunctionIndex.y)].size.numOfHrzLane;
                hrzWeight = 1;
            }
            if (indexer.getIndex((int)newJunctionIndex.x - 1, (int)newJunctionIndex.y) >= 0)
            {
                //Debug.Log(indexer.getIndex((int)newJunctionIndex.x - 1, (int)newJunctionIndex.y));
                size.lenOfVtcLane = junctions[indexer.getIndex((int)newJunctionIndex.x - 1, (int)newJunctionIndex.y)].size.lenOfVtcLane;
                size.numOfHrzLane = junctions[indexer.getIndex((int)newJunctionIndex.x - 1, (int)newJunctionIndex.y)].size.numOfHrzLane;
                hrzWeight = -1;
            }
            if (indexer.getIndex((int)newJunctionIndex.x, (int)newJunctionIndex.y + 1) >= 0)
            {
                //Debug.Log(indexer.getIndex((int)newJunctionIndex.x, (int)newJunctionIndex.y + 1));
                size.lenOfHrzLane = junctions[indexer.getIndex((int)newJunctionIndex.x, (int)newJunctionIndex.y + 1)].size.lenOfHrzLane;
                size.numOfVtcLane = junctions[indexer.getIndex((int)newJunctionIndex.x, (int)newJunctionIndex.y + 1)].size.numOfVtcLane;
                vtcWeight = 1;
            }
            if (indexer.getIndex((int)newJunctionIndex.x, (int)newJunctionIndex.y - 1) >= 0)
            {
                //Debug.Log(indexer.getIndex((int)newJunctionIndex.x, (int)newJunctionIndex.y - 1));
                size.lenOfHrzLane = junctions[indexer.getIndex((int)newJunctionIndex.x, (int)newJunctionIndex.y - 1)].size.lenOfHrzLane;
                size.numOfVtcLane = junctions[indexer.getIndex((int)newJunctionIndex.x, (int)newJunctionIndex.y - 1)].size.numOfVtcLane;
                vtcWeight = -1;
            }
            int hW = Mathf.Abs(hrzWeight);
            int vW = Mathf.Abs(vtcWeight);
            return new JunctionSize(hW * size.numOfHrzLane, vW * size.numOfVtcLane, vW * size.lenOfHrzLane, hW * size.lenOfVtcLane);
        }

        public void setAndCreateJunction(int repetition, ref JunctionSize size, ref Vector2 center, ref Vector2 junctionIndex, ref int dirIndex, ref List<Junction> junctions, ref JunctionIndexer indexer, ref List<AreaSet> areaSet, ref List<Block> sidewalks)
        {
            Vector2 direction = directions[dirIndex];

            for (int k = 0; k < repetition; k++)
            {
                Vector2 newJunctionIndex = junctionIndex + direction;
                if (junctions.Count > 0)
                    size = getNextSize(newJunctionIndex, ref junctions, ref indexer);//junctions[junctions.Count - 1], (int)direction.x, (int)direction.y);
                size = setRandomParam(size);
                if (junctions.Count > 0)
                    center = getNextCenter(junctions[junctions.Count - 1], size, (int)direction.x, (int)direction.y);
                junctionIndex = newJunctionIndex;
                createJunction(center, size, junctionIndex, ref junctions, ref indexer, ref areaSet, ref sidewalks);

            }

            dirIndex = (dirIndex + 1) % directions.Count;
        }

        private Vector2 getNextCenterForNullJunction(Junction junction, int hrzWeight, int vtcWeight)
        {
            int prvWidth = junction.size.numOfVtcLane + junction.size.lenOfHrzLane;
            int prvHeight = junction.size.numOfHrzLane + junction.size.lenOfVtcLane;
            return new Vector2(junction.center.x + hrzWeight * (prvWidth + junction.size.numOfVtcLane), junction.center.y + vtcWeight * (prvHeight+junction.size.numOfHrzLane));
        }

        public void createNullJunction(Vector2 junctionIndex, Junction relatedJunction, AbsDirection direction, ref List<Junction> roadEnds)
        {
            roadEnds.Add(Object.Instantiate((Resources.Load("Prefab/DummyObjects/DummyJunction") as GameObject).GetComponent<Junction>(), Vector3.zero, Quaternion.identity) as SCPAR.SIM.PVATestbed.Junction);
            Vector2 vecDirection = Util.absDirection2Vec2(direction);
            roadEnds[roadEnds.Count - 1].initialize(junctionIndex, getNextCenterForNullJunction(relatedJunction, (int)vecDirection.x, (int)vecDirection.y));
            roadEnds[roadEnds.Count - 1].transform.parent = GameObject.Find("World").transform;
            roadEnds[roadEnds.Count - 1].transform.name = "NullJunction" + (roadEnds.Count - 1).ToString();
        }
    }
}
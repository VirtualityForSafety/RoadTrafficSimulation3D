using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SCPAR.SIM.PVATestbed
{
    public class RoadGenerator
    {
        List<Vector2> directions;

        public RoadGenerator() { directions = Util.getDirections(); }

        public void connectByRoad(Junction source, AbsDirection connectDirection, GameObject roadParent, ref List<Junction> roadEnds,
            ref List<Road> roads, ref List<Junction> junctions, ref JunctionIndexer indexer, ref JunctionGenerator junctionGenerator)
        {
            int coDirection = (int)connectDirection;
            int opDirection = ((int)connectDirection + 2) % 4;

            if (indexer.getIndex(source.coordIndex + directions[coDirection]) >= 0)
            {
                Junction target = junctions[indexer.getIndex(source.coordIndex + directions[coDirection])];
                //Debug.Log(source.ToString() + "\t" + target.ToString() + "\r" + source.connectRoadIdx[coDirection] + "\t" + target.connectRoadIdx[opDirection]);
                if (source.connectRoadIdx[coDirection] < 0 && target.connectRoadIdx[opDirection] < 0)
                {
                    createRoads(source, target, ref roads, roadParent);
                }
                else if (source.connectRoadIdx[coDirection] < 0)
                {
                    source.connectRoadIdx[coDirection] = target.connectRoadIdx[opDirection];
                }
                else if (target.connectRoadIdx[opDirection] < 0)
                {
                    target.connectRoadIdx[opDirection] = source.connectRoadIdx[coDirection];
                }
            }
            else
            {
                junctionGenerator.createNullJunction(source.coordIndex + directions[coDirection], source, (AbsDirection)coDirection, ref roadEnds);
                createRoads(source, roadEnds[roadEnds.Count - 1], ref roads, roadParent);
            }
        }

        private void createUnitRoad(ref Junction source, ref Junction target, ref List<Road> roads, GameObject roadParent)
        {
            Road roadToAdded = Object.Instantiate((Resources.Load("Prefab/DummyObjects/DummyRoad") as GameObject).GetComponent<Road>(), Vector3.zero, Quaternion.identity, roadParent.transform) as SCPAR.SIM.PVATestbed.Road;
            roadToAdded.initialize(source, target);
            roads.Add(roadToAdded);
            roads[roads.Count - 1].index = roads.Count - 1;
            roadToAdded.transform.name = "Road" + (roads.Count - 1).ToString();
            source.connectRoadIdx[(int)Util.vec22AbsDirection(target.coordIndex - source.coordIndex)] = roads.Count - 1;
        }

        private void createRoads(Junction source, Junction target, ref List<Road> roads, GameObject roadParent)
        {
            // original direction
            createUnitRoad(ref source, ref target, ref roads, roadParent);

            // opposite direction
            createUnitRoad(ref target, ref source, ref roads, roadParent);
            
        }
    }
}
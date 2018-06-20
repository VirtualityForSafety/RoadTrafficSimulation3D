using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SCPAR.SIM.PVATestbed
{
    public class World : MonoBehaviour
    {
        /// <summary>
        /// for testing
        /// </summary>
        public Trajectory testTrajectory;

        public Area dummyArea;
        public Junction dummyJunction;
        public BuildingGenerator buildingGenerator;
        public List<Junction> junctions;
        public List<Junction> roadEnds;
        public List<Road> roads;
        public List<Block> sidewalks;
        public List<AreaSet> areaSet;
        public JunctionIndexer indexer;
        public JunctionGenerator junctionGenerator;
        public CarPool carPool;
        //public PedPool pedPool;
        public TrafficLightManager trafficLightManager;
        public bool isReady = false;

        RoadGenerator roadGenerator;

        public Junction getRandomEndPoint()
        {
            return roadEnds[(int)Random.Range(0, roadEnds.Count)];
        }

        public int getRandomEndPointIdx()
        {
            return (int)Random.Range(0, roadEnds.Count);
        }

        public void startDriving()
        {
            if (carPool == null)
                carPool = GameObject.Find("CarPool").GetComponent<CarPool>();
            /* pedestrian pool - not implemented yet
            if (pedPool == null)
                pedPool = GameObject.Find("PedPool").GetComponent<PedPool>();
            */
            if (trafficLightManager == null)
                trafficLightManager = GameObject.Find("TrafficLightManager").GetComponent<TrafficLightManager>();

            // to be fixed : this should be handled in diff
            carPool.initialize();
            //pedPool.initialize();
            trafficLightManager.initialize();


        }

        public void createMap(int loopNum)
        {
            // generate junctions
            Vector2 center = new Vector2(0, 0);
            Vector2 junctionIndex = Vector2.zero;
            JunctionSize size = new JunctionSize();
            int dirIndex = 0;
            indexer = new JunctionIndexer(loopNum);
            roads = new List<Road>();
            sidewalks = new List<Block>();
            roadGenerator = new RoadGenerator();
            junctionGenerator = new JunctionGenerator();

            // generate junction
            size = junctionGenerator.setRandomParam(size);
            junctionGenerator.createJunction(center, size, junctionIndex, ref junctions, ref indexer, ref areaSet, ref sidewalks);          
            for (int i=1; i<loopNum; i++)
            {
                junctionGenerator.setAndCreateJunction(i, ref size, ref center, ref junctionIndex, ref dirIndex, ref junctions, ref indexer, ref areaSet, ref sidewalks);
                junctionGenerator.setAndCreateJunction(i, ref size, ref center, ref junctionIndex, ref dirIndex, ref junctions, ref indexer, ref areaSet, ref sidewalks);
                
                if (i == loopNum - 1)
                    junctionGenerator.setAndCreateJunction(i, ref size, ref center, ref junctionIndex, ref dirIndex, ref junctions, ref indexer, ref areaSet, ref sidewalks);
            }

            // link junctions using road
            linkJunctions(GameObject.Find("Road"));

            // generate area
            if (buildingGenerator == null)
                buildingGenerator = UnityEngine.Object.Instantiate((Resources.Load("Prefab/DummyObjects/BuildingGenerator") as GameObject).GetComponent<BuildingGenerator>(), transform);
            buildingGenerator.generate(areaSet);
            isReady = true;
        }

        public void operate()
        {
            if (isReady)
            {
                startDriving();
            }
        }

        private void linkJunctions(GameObject roadParent)
        {
            
            for (int i=0; i<junctions.Count; i++)
            {
                Junction source = junctions[i];
                roadGenerator.connectByRoad(junctions[i], AbsDirection.E, roadParent, ref roadEnds, ref roads, ref junctions, ref indexer, ref junctionGenerator);
                roadGenerator.connectByRoad(junctions[i], AbsDirection.W, roadParent, ref roadEnds, ref roads, ref junctions, ref indexer, ref junctionGenerator);
                roadGenerator.connectByRoad(junctions[i], AbsDirection.S, roadParent, ref roadEnds, ref roads, ref junctions, ref indexer, ref junctionGenerator);
                roadGenerator.connectByRoad(junctions[i], AbsDirection.N, roadParent, ref roadEnds, ref roads, ref junctions, ref indexer, ref junctionGenerator);
            }
        }        

        private void mergeWithExistingAreaSet(int newIndex, int neighborIndex, int direction)
        {
            if (neighborIndex < 0 || newIndex < 0)
                return ;

            int index = junctions[neighborIndex].areas[direction].areaSetIndex;
            if (index >= 0)
            {
                Debug.Log(newIndex + " - " + neighborIndex + " - " + direction + " - " + index.ToString() + " - " );
                Area tempArea = junctions[newIndex].areas[direction];
                areaSet[index].addArea(tempArea);
                junctions[junctions.Count - 1].areas[direction].setAreaSetIndex(index);
            }
            else
            {
                Debug.Log("ERROR: World - connectJunctions - Negative Index error");
            }
        }

        private void Update()
        {
            //drawAreaSet();
        }

        private void drawAreaSet()
        {
            for (int i = 0; i < areaSet.Count; i++)
            {
                Rect region = areaSet[i].region;
                if (region.width > 0)
                {
                    Debug.DrawLine(new Vector3(region.xMin, 1, region.yMin), new Vector3(region.xMax, 1, region.yMin), Color.red);
                    Debug.DrawLine(new Vector3(region.xMax, 1, region.yMin), new Vector3(region.xMax, 1, region.yMax), Color.red);
                    Debug.DrawLine(new Vector3(region.xMin, 1, region.yMax), new Vector3(region.xMax, 1, region.yMax), Color.red);
                    Debug.DrawLine(new Vector3(region.xMin, 1, region.yMin), new Vector3(region.xMin, 1, region.yMax), Color.red);
                }
            }
        }
    }
}


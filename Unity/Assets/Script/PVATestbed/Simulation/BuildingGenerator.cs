using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SCPAR.SIM.PVATestbed
{
    public class BuildingGenerator : MonoBehaviour
    {
        public List<GameObject> buildings;

        public void generate(List<AreaSet> areaSet)
        {
            buildings = PrefabLoader.LoadAllPrefabs("Assets/Resources/Prefab/Building");
            buildings.Sort(SortBySize);

            /* clipping regions
            Rect centralPart1 = new Rect(0, -200, 20, 400);
            Rect centralPart2 = new Rect(-300, -300, 80, 600);
            Rect centralPart3 = new Rect(250, -300, 80, 600);
            */

            for (int i=0; i<areaSet.Count; i++)
            {
                /* clipping regions
                if ((areaSet[i].region.Overlaps(centralPart1) || areaSet[i].region.Overlaps(centralPart2) || areaSet[i].region.Overlaps(centralPart3)))
                    continue;
                */

                // first we build buildings for each four corner
                List<GameObject> buildingOnArea = new List<GameObject>();
                createABuilding(areaSet[i].region, ref buildingOnArea, AreaPosition.NW);
                createABuilding(areaSet[i].region, ref buildingOnArea, AreaPosition.NE);
                createABuilding(areaSet[i].region, ref buildingOnArea, AreaPosition.SE);
                createABuilding(areaSet[i].region, ref buildingOnArea, AreaPosition.SW);
                
                // then build other buidings between the corner buildings
                createBtwBuilding(buildingOnArea[0], buildingOnArea[1], areaSet[i].region, ref buildingOnArea, AbsDirection.N);
                createBtwBuilding(buildingOnArea[1], buildingOnArea[2], areaSet[i].region, ref buildingOnArea, AbsDirection.E);
                createBtwBuilding(buildingOnArea[0], buildingOnArea[3], areaSet[i].region, ref buildingOnArea, AbsDirection.W);
                createBtwBuilding(buildingOnArea[3], buildingOnArea[2], areaSet[i].region, ref buildingOnArea, AbsDirection.S);
            }
        }

        static int SortBySize(GameObject p1, GameObject p2)
        {
            float p1Size = p1.GetComponent<BoxCollider>().bounds.size.x * p1.GetComponent<BoxCollider>().bounds.size.z;
            float p2Size = p2.GetComponent<BoxCollider>().bounds.size.x * p2.GetComponent<BoxCollider>().bounds.size.z;
            return p1Size.CompareTo(p2Size);
        }

        private Rect getBoundingRect(GameObject object0)
        {
            //return Rect.MinMaxRect(object0.GetComponent<BoxCollider>().bounds.min.x, object0.GetComponent<BoxCollider>().bounds.min.z, object0.GetComponent<BoxCollider>().bounds.max.x, object0.GetComponent<BoxCollider>().bounds.max.z);
            return Rect.MinMaxRect(object0.GetComponent<BoxCollider>().bounds.min.x, object0.GetComponent<BoxCollider>().bounds.min.z, object0.GetComponent<BoxCollider>().bounds.max.x, object0.GetComponent<BoxCollider>().bounds.max.z);
        }

        private void createBtwBuilding(GameObject startObj, GameObject destObj, Rect region, ref List<GameObject> buildingOnArea, AbsDirection direction)
        {
            GameObject object0 = startObj;
            Rect tempRect = getBoundingRect(object0);
            while ( tempRect.Overlaps(region) && !tempRect.Overlaps(getBoundingRect(destObj)))//for(int i=0; i<3; i++)//
            {
                GameObject objectTemp = createABuildingBeside(object0, region, ref buildingOnArea, direction);
                tempRect = getBoundingRect(objectTemp);
                if (tempRect.Overlaps(getBoundingRect(destObj)))
                {
                    Destroy(objectTemp);
                    break;
                }
                buildingOnArea.Add(objectTemp);
                object0 = objectTemp;
            }
        }

        private GameObject createABuildingBeside(GameObject existing, Rect region, ref List<GameObject> buildingOnArea, AbsDirection direction)
        {
            GameObject aBuilding = getRandomBuilding();
            BoxCollider boxCollider = aBuilding.transform.GetComponent<BoxCollider>();
            Vector3 newPosition = Vector3.zero;
            if (direction == AbsDirection.N)
                newPosition = new Vector3(existing.GetComponent<BoxCollider>().bounds.max.x + boxCollider.bounds.size.x / 2, 
                    boxCollider.bounds.max.y + SimParameter.areaHeight + SimParameter.blockHeightHalf, region.yMax - boxCollider.bounds.size.z / 2 + Random.Range(0,0.3f));
            else if (direction == AbsDirection.S)
                newPosition = new Vector3(existing.GetComponent<BoxCollider>().bounds.max.x + boxCollider.bounds.size.x / 2,
                    boxCollider.bounds.max.y + SimParameter.areaHeight + SimParameter.blockHeightHalf, region.yMin + boxCollider.bounds.size.z / 2 - Random.Range(0, 0.3f));
            else if (direction == AbsDirection.W)
                newPosition = new Vector3(region.xMin + boxCollider.bounds.size.z / 2,// + Random.Range(0, 0.3f),
                    boxCollider.bounds.max.y + SimParameter.areaHeight + SimParameter.blockHeightHalf, existing.GetComponent<BoxCollider>().bounds.min.z - boxCollider.bounds.size.z / 2);
            else
                newPosition = new Vector3(region.xMax - boxCollider.bounds.size.z / 2,// - Random.Range(0, 0.3f),
                    boxCollider.bounds.max.y + SimParameter.areaHeight + SimParameter.blockHeightHalf, existing.GetComponent<BoxCollider>().bounds.min.z - boxCollider.bounds.size.z / 2);

            aBuilding.transform.position = newPosition;
            aBuilding.transform.parent = this.transform;
            buildingOnArea.Add(aBuilding);
            return aBuilding;
        }

        private void createABuilding(Rect region, ref List<GameObject> buildingOnArea, AreaPosition pos)
        {
            GameObject aBuilding = getRandomBuilding();
            BoxCollider boxCollider = aBuilding.transform.GetComponent<BoxCollider>();
            Vector3 newPosition = Vector3.zero;
            if(pos == AreaPosition.SW)
                newPosition = new Vector3(region.xMin + boxCollider.bounds.size.x / 2, boxCollider.bounds.max.y + SimParameter.areaHeight + SimParameter.blockHeightHalf, region.yMin + boxCollider.bounds.size.z / 2);
            else if (pos == AreaPosition.NW)
                newPosition = new Vector3(region.xMin + boxCollider.bounds.size.x / 2, boxCollider.bounds.max.y + SimParameter.areaHeight + SimParameter.blockHeightHalf, region.yMax - boxCollider.bounds.size.z / 2);
            else if (pos == AreaPosition.SE)
                newPosition = new Vector3(region.xMax - boxCollider.bounds.size.x / 2, boxCollider.bounds.max.y + SimParameter.areaHeight + SimParameter.blockHeightHalf, region.yMin + boxCollider.bounds.size.z / 2);
            else
                newPosition = new Vector3(region.xMax - boxCollider.bounds.size.x / 2, boxCollider.bounds.max.y + SimParameter.areaHeight + SimParameter.blockHeightHalf, region.yMax - boxCollider.bounds.size.z / 2);
            aBuilding.transform.position = newPosition;
            aBuilding.transform.parent = this.transform;
            buildingOnArea.Add(aBuilding);
        }

        private GameObject getRandomBuilding()
        {
            //GameObject aBuilding = Object.Instantiate(buildings[(int)Random.Range(0, buildings.Count)], Vector3.zero, Quaternion.identity);
            GameObject aBuilding = UnityEngine.Object.Instantiate(Resources.Load("Prefab/Building/Bld00" + ((int)Random.Range(0, 5) + 1)) as GameObject, Vector3.zero, Quaternion.identity);
            //aBuilding.transform.localScale = new Vector3(1, 1, 1);
            return aBuilding;
        }
    }
}
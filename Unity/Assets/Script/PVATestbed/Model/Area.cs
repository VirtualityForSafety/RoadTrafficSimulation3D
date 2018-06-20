using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SCPAR.SIM.PVATestbed
{

    public class Area : MonoBehaviour
    {
        public int width;
        public int height;
        public List<List<Block>> blocks;
        public AreaPosition areaPosition;
        public Vector2 center;
        public AreaType areaType;
        public Rect region;
        public int areaSetIndex;

        private void setRegion(int prefixWidth, int prefixHeight, AreaPosition pos)
        {
            int prefixBlockSize = (int)(SimParameter.unitBlockSize / 2.0f);
            region = Rect.MinMaxRect(center.x - prefixWidth, center.y - prefixHeight, center.x + prefixWidth, center.y + prefixHeight);
            region = Rect.MinMaxRect(region.xMin * SimParameter.unitBlockSize - prefixBlockSize+ (pos == AreaPosition.NE || pos == AreaPosition.SE ? +SimParameter.unitBlockSize : 0),
                region.yMin * SimParameter.unitBlockSize - prefixBlockSize + (pos == AreaPosition.NE || pos == AreaPosition.NW ? +SimParameter.unitBlockSize : 0),
                region.xMax * SimParameter.unitBlockSize - prefixBlockSize + (pos == AreaPosition.NW || pos == AreaPosition.SW ? -SimParameter.unitBlockSize : 0),
                region.yMax * SimParameter.unitBlockSize - prefixBlockSize + (pos == AreaPosition.SW || pos == AreaPosition.SE ? -SimParameter.unitBlockSize : 0));
        }

        public void initialize(int givenWidth, int givenHeight, Vector2 givenCenter, AreaPosition pos, AreaType type, ref List<Block> sidewalks)
        {
            areaSetIndex = -1;
            areaPosition = pos;
            center = givenCenter;
            width = givenWidth;
            height = givenHeight;
            transform.name = type.ToString();

            int prefixWidth = width / 2;
            int prefixHeight = height / 2;
            setRegion(prefixWidth, prefixHeight, pos);
            
            blocks = new List<List<Block>>();
            for(int i=0; i< width; i++) {
                blocks.Add(new List<Block>());
                for(int k=0; k< height; k++)
                {
                    if (putSidewalkBlock(i, k, width, height, prefixWidth, prefixHeight, ref sidewalks))
                    {
                        continue;
                    }
                    float idxX = i - prefixWidth + (int)center.x;
                    float idxY = k - prefixHeight + (int)center.y;
                    if (type == AreaType.Intersection)
                    {
                        blocks[i].Add(Object.Instantiate((Resources.Load("Prefab/DummyObjects/DummyRoadBlock") as GameObject).GetComponent<RoadBlock>(), Vector3.zero, Quaternion.identity) as SCPAR.SIM.PVATestbed.RoadBlock);
                        blocks[i][blocks[i].Count - 1].initialize(idxX, idxY, ModelType.RoadIntersection);
                    }
                    else if (type == AreaType.AreaBlock)
                    {
                        blocks[i].Add(Object.Instantiate((Resources.Load("Prefab/DummyObjects/DummyAreaBlock") as GameObject).GetComponent<AreaBlock>(), Vector3.zero, Quaternion.identity) as SCPAR.SIM.PVATestbed.AreaBlock);
                        blocks[i][blocks[i].Count - 1].initialize(idxX, idxY, ModelType.AreaBuilding);
                    }
                    /*
                    else if (type == AreaType.Grass)
                    {
                        blocks[i].Add(Object.Instantiate(model, Vector3.zero, Quaternion.identity) as SCPAR.SIM.PVATestbed.AreaBlock);
                        blocks[i][blocks[i].Count - 1].initialize(idxX, idxY, ModelType.AreaGrass);
                    }
                    else if (type == AreaType.Building)
                    {
                        blocks[i].Add(Object.Instantiate(model, Vector3.zero, Quaternion.identity) as SCPAR.SIM.PVATestbed.AreaBlock);
                        blocks[i][blocks[i].Count - 1].initialize(idxX, idxY, ModelType.AreaBuilding);
                    }
                    else if (type == AreaType.Forest)
                    {
                        blocks[i].Add(Object.Instantiate(model, Vector3.zero, Quaternion.identity) as SCPAR.SIM.PVATestbed.AreaBlock);
                        blocks[i][blocks[i].Count - 1].initialize(idxX, idxY, ModelType.AreaForest);
                    }
                    */

                    blocks[i][blocks[i].Count - 1].transform.parent = transform;
                }
            }

            // create more roadblock for four corners of an intersection
            if(type == AreaType.Intersection)
            {
                RoadBlock temp = Object.Instantiate((Resources.Load("Prefab/DummyObjects/DummyRoadBlock") as GameObject).GetComponent<RoadBlock>(), Vector3.zero, Quaternion.identity, transform) as SCPAR.SIM.PVATestbed.RoadBlock;
                temp.initialize(-1-prefixWidth + (int)center.x, -1-prefixHeight + (int)center.y, ModelType.RoadNormal);
                temp = Object.Instantiate((Resources.Load("Prefab/DummyObjects/DummyRoadBlock") as GameObject).GetComponent<RoadBlock>(), Vector3.zero, Quaternion.identity, transform) as SCPAR.SIM.PVATestbed.RoadBlock;
                temp.initialize(width - prefixWidth + (int)center.x, height - prefixHeight + (int)center.y, ModelType.RoadNormal);
                temp = Object.Instantiate((Resources.Load("Prefab/DummyObjects/DummyRoadBlock") as GameObject).GetComponent<RoadBlock>(), Vector3.zero, Quaternion.identity, transform) as SCPAR.SIM.PVATestbed.RoadBlock;
                temp.initialize(width - prefixWidth + (int)center.x, -1 - prefixHeight + (int)center.y, ModelType.RoadNormal);
                temp = Object.Instantiate((Resources.Load("Prefab/DummyObjects/DummyRoadBlock") as GameObject).GetComponent<RoadBlock>(), Vector3.zero, Quaternion.identity, transform) as SCPAR.SIM.PVATestbed.RoadBlock;
                temp.initialize(-1 - prefixWidth + (int)center.x, height - prefixHeight + (int)center.y, ModelType.RoadNormal);
            }
        }

        public void setAreaSetIndex(int index)
        {
            areaSetIndex = index;
        }

        private bool putSidewalkBlock(int x, int y, int width, int height, int prefixWidth, int prefixHeight, ref List<Block> sidewalks)
        {
            if (areaPosition == AreaPosition.NA)
                return false;
            else if(areaPosition == AreaPosition.NE)
            {
                if (y == 0 || x == 0)
                {
                    blocks[x].Add(Object.Instantiate((Resources.Load("Prefab/DummyObjects/DummyAreaBlock") as GameObject).GetComponent<AreaBlock>(), Vector3.zero, Quaternion.identity) as SCPAR.SIM.PVATestbed.AreaBlock);
                    if(x==0 && y==0)
                        blocks[x][blocks[x].Count - 1].initialize(x - prefixWidth + (int)center.x, y - prefixHeight + (int)center.y, ModelType.AreaSidewalkCorner, areaPosition);
                    else
                        blocks[x][blocks[x].Count - 1].initialize(x - prefixWidth + (int)center.x, y - prefixHeight + (int)center.y, ModelType.AreaSidewalk, AreaPosition.NA, y==0);
                    blocks[x][blocks[x].Count - 1].transform.parent = transform;
                    sidewalks.Add(blocks[x][blocks[x].Count - 1]);
                    return true;
                }
            }
            else if (areaPosition == AreaPosition.NW)
            {
                if (y == 0 || x == width-1)
                {
                    blocks[x].Add(Object.Instantiate((Resources.Load("Prefab/DummyObjects/DummyAreaBlock") as GameObject).GetComponent<AreaBlock>(), Vector3.zero, Quaternion.identity) as SCPAR.SIM.PVATestbed.AreaBlock);
                    if (y == 0 && x == width - 1)
                        blocks[x][blocks[x].Count - 1].initialize(x - prefixWidth + (int)center.x, y - prefixHeight + (int)center.y, ModelType.AreaSidewalkCorner, areaPosition);
                    else
                        blocks[x][blocks[x].Count - 1].initialize(x - prefixWidth + (int)center.x, y - prefixHeight + (int)center.y, ModelType.AreaSidewalk, AreaPosition.NA, y == 0);
                    blocks[x][blocks[x].Count - 1].transform.parent = transform;
                    sidewalks.Add(blocks[x][blocks[x].Count - 1]);
                    return true;
                }
            }
            else if (areaPosition == AreaPosition.SE)
            {
                if (y == height - 1 || x == 0)
                {
                    blocks[x].Add(Object.Instantiate((Resources.Load("Prefab/DummyObjects/DummyAreaBlock") as GameObject).GetComponent<AreaBlock>(), Vector3.zero, Quaternion.identity) as SCPAR.SIM.PVATestbed.AreaBlock);
                    if (y == height - 1 && x == 0)
                        blocks[x][blocks[x].Count - 1].initialize(x - prefixWidth + (int)center.x, y - prefixHeight + (int)center.y, ModelType.AreaSidewalkCorner, areaPosition);
                    else
                        blocks[x][blocks[x].Count - 1].initialize(x - prefixWidth + (int)center.x, y - prefixHeight + (int)center.y, ModelType.AreaSidewalk, AreaPosition.NA, y == height - 1);
                    blocks[x][blocks[x].Count - 1].transform.parent = transform;
                    sidewalks.Add(blocks[x][blocks[x].Count - 1]);
                    return true;
                }
            }
            else if (areaPosition == AreaPosition.SW)
            {
                if (y == height - 1 || x == width - 1)
                {
                    blocks[x].Add(Object.Instantiate((Resources.Load("Prefab/DummyObjects/DummyAreaBlock") as GameObject).GetComponent<AreaBlock>(), Vector3.zero, Quaternion.identity) as SCPAR.SIM.PVATestbed.AreaBlock);
                    if (y == height - 1 && x == width - 1)
                        blocks[x][blocks[x].Count - 1].initialize(x - prefixWidth + (int)center.x, y - prefixHeight + (int)center.y, ModelType.AreaSidewalkCorner, areaPosition);
                    else
                        blocks[x][blocks[x].Count - 1].initialize(x - prefixWidth + (int)center.x, y - prefixHeight + (int)center.y, ModelType.AreaSidewalk, AreaPosition.NA, y == height - 1);
                    blocks[x][blocks[x].Count - 1].transform.parent = transform;
                    sidewalks.Add(blocks[x][blocks[x].Count - 1]);
                    return true;
                }
            }
            return false;
        }
    }

}

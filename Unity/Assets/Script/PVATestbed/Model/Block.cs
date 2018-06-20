using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SCPAR.SIM.PVATestbed
{
    public enum BlockDirection { Horizontal = 0, Vertical = 1, Intersection = 2 };

    public enum ModelType { RoadNormal = 0, RoadIntersection = 1, AreaGrass = 2, AreaBuilding = 3, AreaForest = 4, AreaSidewalk = 5, AreaSidewalkCorner = 6 };

    public class Block : MonoBehaviour
    {
        [SerializeField]
        public Vector2 mapPosition;
        
        public BlockDirection blockDirection; // will be used for sidewalk

        public bool isHorizontal;

        public AreaPosition areaPosition;

        public GameObject block;

        public void initialize(float x, float y, ModelType type, AreaPosition _position = AreaPosition.NA, bool givenIsHorizontal = true)
        {
            mapPosition = new Vector2(x, y);
            isHorizontal = givenIsHorizontal;
            areaPosition = _position;
            createModel(type);
        }

        virtual protected void createModel(ModelType type) { }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}

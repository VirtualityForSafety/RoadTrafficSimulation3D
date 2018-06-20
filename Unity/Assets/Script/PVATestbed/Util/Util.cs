using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SCPAR.SIM.PVATestbed
{
    public enum AreaPosition { NA = -1, NW = 0, NE = 1, SE = 2, SW = 3 }
    public enum AbsDirection { NA = -1, S = 0, W = 1, N = 2, E = 3 }
    public enum RltDirection { NA = -1, Straight = 0, Left = 1, Right = 2 }
    public enum AreaType { Intersection = 0, Sidewalk = 1, AreaBlock = 2 }

    public class JunctionIndexer
    {
        int width;
        int[][] indices;

        public JunctionIndexer(int loopNum)
        {
            width = loopNum + 2;
            indices = new int[width][];
            for (int i = 0; i < width; i++)
            {
                indices[i] = new int[width];
                for (int k = 0; k < width; k++)
                {
                    indices[i][k] = -1;
                }
            }
        }

        public void setIndex(int x, int y, int value)
        {
            indices[x + width / 2][y + width / 2] = value;
        }

        public int getIndex(int x, int y)
        {
            if (x + width / 2 < 0 || x + width / 2 >= width || y + width / 2 < 0 || y + width / 2 >= width)
                return -1;

            return indices[x + width / 2][y + width / 2];
        }

        public int getIndex(Vector2 vec2)
        {
            int x = (int)vec2.x;
            int y = (int)vec2.y;
            if (x + width / 2 < 0 || x + width / 2 >= width || y + width / 2 < 0 || y + width / 2 >= width)
                return -1;

            return indices[x + width / 2][y + width / 2];
        }
    }

    public static class Util
    {
        public static RltDirection getRltDirection(AbsDirection curDir, AbsDirection nextDir)
        {
            int diff = (nextDir - curDir + 4)%4;
            if (diff == 0)
                return RltDirection.Straight;
            else if (diff == 1)
                return RltDirection.Right;
            else if (diff == 3)
                return RltDirection.Left;
            else
                Debug.Log("Util - getRltDirection - ERROR ");
            return RltDirection.NA;
        }

        public static Vector3 absDirection2Vec3Deg(AbsDirection absDir)
        {
            if (absDir == AbsDirection.N)
                return new Vector3(0, 0, 0);
            else if(absDir == AbsDirection.S)
                return new Vector3(0, 180, 0);
            else if(absDir == AbsDirection.E)
                return new Vector3(0, 90, 0);
            else if(absDir == AbsDirection.W)
                return new Vector3(0, 270, 0);
            else
                return new Vector3(0, 0, 0);
        }

        public static Quaternion absDirection2Quat(AbsDirection absDir)
        {
            if (absDir == AbsDirection.N)
                return Quaternion.Euler(new Vector3(0, 0, 0));
            else if (absDir == AbsDirection.S)
                return Quaternion.Euler(new Vector3(0, 180, 0));
            else if (absDir == AbsDirection.E)
                return Quaternion.Euler(new Vector3(0, 90, 0));
            else if (absDir == AbsDirection.W)
                return Quaternion.Euler(new Vector3(0, 270, 0));
            else
                return Quaternion.Euler(new Vector3(0, 0, 0));
        }

        public static Vector3 absDirection2Vec3(AbsDirection absDir)
        {
            if (absDir == AbsDirection.N)
                return new Vector3(0, 0, 1);
            else if (absDir == AbsDirection.S)
                return new Vector3(0, 0, -1);
            else if (absDir == AbsDirection.E)
                return new Vector3(1, 0, 0);
            else if (absDir == AbsDirection.W)
                return new Vector3(-1, 0, 0);
            else
                return new Vector3(0, 0, 0);
        }

        public static AbsDirection addDirection(AbsDirection absDir, RltDirection rrtDir)
        {
            if (rrtDir == RltDirection.Straight)
                return absDir;
            else if (rrtDir == RltDirection.Left)
                return (AbsDirection)(((int)absDir + 3) % 4);
            else if (rrtDir == RltDirection.Right)
                return (AbsDirection)(((int)absDir + 1) % 4);
            else
            {
                Debug.Log("Util - addDirection - No RltDirection : " + rrtDir);
                return AbsDirection.NA;
            }                
        }

        public static Vector2 absDirection2Vec2(AbsDirection absDir)
        {
            if (absDir == AbsDirection.N)
                return Vector2.up;
            else if (absDir == AbsDirection.S)
                return Vector2.down;
            else if (absDir == AbsDirection.W)
                return Vector2.left;
            else if (absDir == AbsDirection.E)
                return Vector2.right;
            else
                return Vector2.zero;
        }

        public static AbsDirection vec22AbsDirection(Vector2 vecDirection)
        {
            AbsDirection direction = AbsDirection.NA;
            if (vecDirection.x > 0 && vecDirection.y == 0)
                direction = AbsDirection.E;
            else if (vecDirection.x < 0 && vecDirection.y == 0)
                direction = AbsDirection.W;
            else if (vecDirection.x == 0 && vecDirection.y > 0)
                direction = AbsDirection.N;
            else if (vecDirection.x == 0 && vecDirection.y < 0)
                direction = AbsDirection.S;
            return direction;
        }

        public static List<Vector2> getDirections()
        {
            List<Vector2> directions = new List<Vector2>();
            directions.Add(new Vector2(0, -1));
            directions.Add(new Vector2(-1, 0));
            directions.Add(new Vector2(0, 1));
            directions.Add(new Vector2(1, 0));
            return directions;
        }
    }
}

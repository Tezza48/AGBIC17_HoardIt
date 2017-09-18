using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace HoardIt.Core
{
    [Serializable]
    public struct Point
    {
        [SerializeField]
        int x;
        [SerializeField]
        int y;

        public int X { get { return x; } set { x = value; } }
        public int Y { get { return y; } set { y = value; } }

        public Point(int _x, int _y)
        {
            x = _x;
            y = _y;
        }

        public static explicit operator Vector2(Point rhs)
        {
            return new Vector2(rhs.x, rhs.y);
        }
    }
}

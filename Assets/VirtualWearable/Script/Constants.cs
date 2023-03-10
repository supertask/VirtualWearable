using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualWearable
{
    public class Constants
    {
        public class App
        {
            //public static float Size = 3.0f;
            public static float Size = 2.4f;
            //public static float LocalPositionY = 0.018f;
            public static float LocalPositionY = 0.028f;
            public static float LocalRotationBeginAngle = 0.0f;
            public static float LocalRotationEndAngle = 360.0f;
        }
        public class HandWing
        {
            public static Vector2 ClippingRange = new Vector2(-0.04f, 0.575144f); //x = begin x position, y = end x position
        }
    }
}
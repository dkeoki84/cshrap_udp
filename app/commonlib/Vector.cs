using System;

namespace CommonLib
{
    public class Vector3
    {
        public float v1 = 0;
        public float v2 = 0;
        public float v3 = 0;

        public Vector3() {}
        public Vector3(float v1, float v2, float v3) {
            this.v1 = v1;
            this.v2 = v2;
            this.v3 = v3;
        }

        public void dump()
        {
            Console.WriteLine("{0}, {1}, {2}", v1, v2, v3);
        }
    }

    public class Vector6
    {
        public float v1 = 0;
        public float v2 = 0;
        public float v3 = 0;
        public float v4 = 0;
        public float v5 = 0;
        public float v6 = 0;

        public Vector6() {}
        public Vector6(
            float v1,
            float v2,
            float v3,
            float v4,
            float v5,
            float v6
        ) {
            this.v1 = v1;
            this.v2 = v2;
            this.v3 = v3;
            this.v4 = v4;
            this.v5 = v5;
            this.v6 = v6;
        }

        public void dump()
        {
            Console.WriteLine(
                "{0}, {1}, {2}, {3}, {4}, {5}",
                v1, v2, v3, v4, v5, v6
            );
        }
    }
}

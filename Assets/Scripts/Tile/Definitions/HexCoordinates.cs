using UnityEngine;
using System;

namespace Tile {
    // NOTE : Å¸ÀÏÀÇ ÁÂÇ¥
    [Serializable]
    public struct HexCoordinates {
        [SerializeField] private int x, z;

        public int X { get { return x; } }
        public int Y { get { return -X - Z; } }
        public int Z { get { return z; } }

        public HexCoordinates(int x, int z) {
            this.x = x;
            this.z = z;
        }

        public static bool operator ==(HexCoordinates hexCoordinates1, HexCoordinates hexCoordinates2) =>
            hexCoordinates1.X == hexCoordinates2.X && hexCoordinates1.Y == hexCoordinates2.Y && hexCoordinates1.Z == hexCoordinates2.Z;
        public static bool operator !=(HexCoordinates hexCoordinates1, HexCoordinates hexCoordinates2) =>
            hexCoordinates1.X != hexCoordinates2.X || hexCoordinates1.Y != hexCoordinates2.Y || hexCoordinates1.Z != hexCoordinates2.Z;

        public static HexCoordinates operator /(HexCoordinates hexCoordinates1, HexCoordinates hexCoordinates2) {
            if (hexCoordinates2.X == 0 || hexCoordinates2.Z == 0) throw new DivideByZeroException();
            return new HexCoordinates(hexCoordinates1.X / hexCoordinates2.X, hexCoordinates1.Z / hexCoordinates2.Z);
        }
        public static HexCoordinates operator /(HexCoordinates hexCoordinates, int num) {
            if (num == 0) throw new DivideByZeroException();
            return new HexCoordinates(hexCoordinates.X / num, hexCoordinates.Y / num);
        }

        public static HexCoordinates FromOffsetCoordinates(int x, int z) {
            return new HexCoordinates(x - z / 2, z);
        }
        public override string ToString() {
            return "(" +
                X.ToString() + ", " + Y.ToString() + ", " + Z.ToString() + ")";
        }

        public string ToStringOnSeparateLines() {
            return X.ToString() + "\n" + Y.ToString() + "\n" + Z.ToString();
        }

        public static HexCoordinates FromPosition(Vector3 position) {
            float x = position.x / (HexMetrics.innerRadius * 2f);
            float y = -x;
            float offset = position.z / (HexMetrics.outerRadius * 3f);
            x -= offset;
            y -= offset;
            int iX = Mathf.RoundToInt(x);
            int iY = Mathf.RoundToInt(y);
            int iZ = Mathf.RoundToInt(-x - y);

            if (iX + iY + iZ != 0) {
                float dX = Mathf.Abs(x - iX);
                float dY = Mathf.Abs(y - iY);
                float dZ = Mathf.Abs(-x - y - iZ);

                if (dX > dY && dX > dZ) {
                    iX = -iY - iZ;
                }
                else if (dZ > dY) {
                    iZ = -iX - iY;
                }
            }

            return new HexCoordinates(iX, iZ);
        }
    }
}
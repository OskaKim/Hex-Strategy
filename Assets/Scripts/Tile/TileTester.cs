using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace Tile
{
    public class TileTester : MonoBehaviour
    {
        [SerializeField] IndexPair getIndex;
        [SerializeField] MyCamera.CameraModel cameraModel;
        [SerializeField] public HexCoordinates findPathStart;
        [SerializeField] public HexCoordinates findPathEnd;
        [SerializeField] public List<Tile> path;
    }
}
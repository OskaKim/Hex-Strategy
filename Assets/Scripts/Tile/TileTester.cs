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
        [SerializeField] public IndexPair findPathStart;
        [SerializeField] public IndexPair findPathEnd;
        [SerializeField] public List<Tile> path;
    }
}
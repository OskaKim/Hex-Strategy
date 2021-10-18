using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UINavigation : MonoBehaviour {
    [SerializeField] private Transform destinationUITransform;
    private PathFinderManager pathFinder;

    private void OnEnable() {
        pathFinder = PathFinderManager.GetInstance();
        pathFinder.StartPathFindingEvent += OnStartPathFindingEvent;
        destinationUITransform.gameObject.SetActive(false);
    }

    private void OnDisable() {
        pathFinder.StartPathFindingEvent -= OnStartPathFindingEvent;
    }

    private void OnStartPathFindingEvent(bool isPlayer, PathFinder pathFinder) {
        destinationUITransform.gameObject.SetActive(isPlayer);
        if (isPlayer) {
            var destinationTilePosition = pathFinder.DestinationTile.transform.position;
            destinationUITransform.position = new Vector3(destinationTilePosition.x, destinationUITransform.position.y, destinationTilePosition.z);
        }
    }
}

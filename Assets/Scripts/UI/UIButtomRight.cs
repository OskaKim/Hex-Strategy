using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIButtomRight : MonoBehaviour
{
    public void OnClickNextTurn() {
        TurnManager.MoveForward();
    }
}

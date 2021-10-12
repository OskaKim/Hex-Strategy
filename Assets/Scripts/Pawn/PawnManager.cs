using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawnManager : MonoBehaviour
{
    private static PawnManager instance;
    public  static PawnManager GetInstance() { return instance; }

    public List<List<Pawn>> allPawns = new List<List<Pawn>>();

    // TODO : 임시 값. 플레이어 수 만큼 생성
    private int currentPlayers = 2;

    private void Awake() {
        instance = this;
    }

    private void OnEnable() {
        TurnManager.MoveForwardEvent += TurnMoveForward;
    }

    private void OnDisable() {
        TurnManager.MoveForwardEvent -= TurnMoveForward;
    }

    private void Start() {
        for(int i = 0; i < currentPlayers; ++i) {
            allPawns.Add(new List<Pawn>());
        }
    }

    public void AddPawn(Pawn pawn, int playerNum) {
        Debug.Log(playerNum);
        Debug.Log(allPawns);
        allPawns[playerNum].Add(pawn);
    }

    private void TurnMoveForward(int currentTurn) {
        StartCoroutine(PawnTurnUpdate());
    }

    private IEnumerator PawnTurnUpdate() {
        // NOTE : 각 플레이어의 턴 업데이터
        for (int i = 0; i < currentPlayers; ++i) {
            var currentPawns = allPawns[i];
            currentPawns.ForEach(x => x.MoveForwardPath());
            yield return null;
        }
        yield break;
    }
}

using System;

public static class TurnManager
{
    public static event Action<int> MoveForwardEvent;

    private static int currentTurn = 0;

    public static void MoveForward() {
        MoveForwardEvent(++currentTurn);
    }
}

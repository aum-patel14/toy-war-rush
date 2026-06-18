// TOY WAR RUSH - GameStateMachine.cs
// Optional helper for validating state transitions.

using System;
using System.Collections.Generic;

public class GameStateMachine
{
    private readonly Dictionary<GameState, HashSet<GameState>> _allowedTransitions = new()
    {
        { GameState.Boot, new HashSet<GameState> { GameState.MainMenu } },
        { GameState.MainMenu, new HashSet<GameState> { GameState.Playing } },
        { GameState.Playing, new HashSet<GameState> { GameState.Paused, GameState.Victory, GameState.Defeat } },
        { GameState.Paused, new HashSet<GameState> { GameState.Playing, GameState.MainMenu } },
        { GameState.Victory, new HashSet<GameState> { GameState.MainMenu, GameState.Playing } },
        { GameState.Defeat, new HashSet<GameState> { GameState.MainMenu, GameState.Playing } },
    };

    public bool CanTransition(GameState from, GameState to)
    {
        return _allowedTransitions.TryGetValue(from, out var allowed) && allowed.Contains(to);
    }

    public bool TryTransition(GameState from, GameState to, Action onSuccess)
    {
        if (!CanTransition(from, to)) return false;
        onSuccess?.Invoke();
        return true;
    }
}

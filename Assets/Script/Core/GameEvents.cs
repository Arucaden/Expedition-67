using System;
using UnityEngine;

namespace Expedition67.Core
{
    public static class GameEvents
    {
        public static Action<Vector2, float> OnNoiseMade;
        public static Action<Vector2> OnGuardAlerted;
        public static Action OnLevelCompleted;
        public static Action OnPlayerCaught;
        public static Action OnGamePaused;
        public static Action OnGameResumed;
        public static Action OnLevelRestarted;
    }
}

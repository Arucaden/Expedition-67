using UnityEngine;
using Expedition67.Core;

namespace Expedition67
{
    public class ObjectiveArea : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                GameEvents.OnLevelCompleted?.Invoke();
            }
        }
    }
}

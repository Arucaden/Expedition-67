using UnityEngine;
using Expedition67.Core;

namespace Expedition67.Enemies.Guard
{
    public class GuardState_Suspicious : BaseState<GuardController>
    {
        private float delayTimer;

        public GuardState_Suspicious(GuardController guard)
        {
            this.owner = guard;
        }

        public override void OnEnter()
        {
            delayTimer = 0f;

            if (owner.Agent != null)
            {
                owner.Agent.isStopped = true;
            }

            if (owner.AlertIcon != null)
            {
                owner.AlertIcon.SetActive(true);
            }

            Vector2 direction = owner.LastKnownPosition - (Vector2)owner.transform.position;
            if (direction != Vector2.zero)
            {
                owner.transform.up = direction.normalized;
            }
        }

        public override void OnUpdate()
        {
            delayTimer += Time.deltaTime;

            if (delayTimer >= owner.SuspiciousDelay)
            {
                owner.ChangeState(owner.SearchingState);
            }
        }

        public override void OnExit()
        {
            if (owner.AlertIcon != null)
            {
                owner.AlertIcon.SetActive(false);
            }

            if (owner.Agent != null)
            {
                owner.Agent.isStopped = false;
            }
        }
    }
}

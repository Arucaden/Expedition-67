using UnityEngine;
using Expedition67.Core;

namespace Expedition67.Enemies.Guard
{
    public class GuardState_Searching : BaseState<GuardController>
    {
        private float searchTimer;

        public GuardState_Searching(GuardController guard)
        {
            this.owner = guard;
        }

        public override void OnEnter()
        {
            searchTimer = owner.SearchDuration;
            if (owner.Agent != null)
            {
                owner.Agent.speed = owner.PatrolSpeed;
            }
        }

        public override void OnUpdate()
        {
            if (Vector2.Distance(owner.transform.position, owner.LastKnownPosition) > 0.1f)
            {
                if (owner.Agent != null)
                {
                    owner.Agent.SetDestination(owner.LastKnownPosition);
                }
            }
            else
            {
                owner.transform.Rotate(0, 0, 90f * Time.deltaTime);

                searchTimer -= Time.deltaTime;
                if (searchTimer <= 0)
                {
                    owner.ChangeState(owner.IdleState);
                }
            }
        }

        public override void OnExit()
        {
            if (owner.Agent != null) 
            {
                owner.Agent.ResetPath();
            }
        }
    }
}

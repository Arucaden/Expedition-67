using UnityEngine;
using Expedition67.Core;

namespace Expedition67.Enemies.Guard
{
    public class GuardState_Alert : BaseState<GuardController>
    {
        private float alertTimer;

        public GuardState_Alert(GuardController guard)
        {
            this.owner = guard;
        }

        public override void OnEnter()
        {
            alertTimer = 0f;

            if (owner.Agent != null)
            {
                owner.Agent.speed = owner.ChaseSpeed;
            }

            if (owner.Fov != null)
            {
                owner.Fov.SetAlertState(true);
            }

            if(owner.Player != null)
            {
                GameEvents.OnGuardAlerted?.Invoke(owner.Player.position);
            }
        }

        public override void OnUpdate()
        {
            if (owner.CanSeePlayer())
            {
                owner.LastKnownPosition = owner.Player.position;

                if (owner.Agent != null)
                {
                    owner.Agent.SetDestination(owner.Player.position);
                }

                alertTimer += Time.deltaTime;
                if (alertTimer >= owner.TimeToCatch)
                {
                    GameEvents.OnPlayerCaught?.Invoke();
                    alertTimer = 0f;
                }
            }
            else
            {
                owner.ChangeState(owner.SearchingState);
            }
        }

        public override void OnExit()
        {
            if (owner.Fov != null)
            {
                owner.Fov.SetAlertState(false);
            }

            if (owner.Agent != null) 
            {
                owner.Agent.ResetPath();
            }
        }
    }
}

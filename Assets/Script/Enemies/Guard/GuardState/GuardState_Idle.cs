using UnityEngine;
using Expedition67.Core;

namespace Expedition67.Enemies.Guard
{
    public class GuardState_Idle : BaseState<GuardController>
    {
        private int currentPatrolIndex = 0;
        private float waitTimer = 0f;
        private bool isWaiting = false;

        public GuardState_Idle(GuardController guard)
        {
            this.owner = guard;
        }

        public override void OnEnter()
        {
            if (owner.Agent != null)
            {
                owner.Agent.speed = owner.PatrolSpeed;
            }
            isWaiting = false;
        }

        public override void OnUpdate()
        {
            Patrol();
        }

        public override void OnExit()
        {
            owner.Agent.ResetPath();
        }

        private void Patrol()
        {
            if (owner.PatrolPoints == null || owner.PatrolPoints.Length == 0) 
            {
                return;
            }

            if (isWaiting)
            {
                waitTimer -= Time.deltaTime;
                if (waitTimer <= 0)
                {
                    isWaiting = false;

                    if (currentPatrolIndex < owner.PatrolPoints.Length)
                    {
                        currentPatrolIndex++;
                    }
                    else
                    {
                        if (owner.LoopPatrol)
                        {
                            currentPatrolIndex = 0;
                        }
                        else
                        {
                            return;
                        }
                    }
                }
                else
                {
                    return;
                }
            }

            Vector2 targetPos;
            if (currentPatrolIndex < owner.PatrolPoints.Length)
            {
                targetPos = owner.PatrolPoints[currentPatrolIndex].position;
            }
            else
            {
                targetPos = owner.InitialPosition;
            }

            owner.Agent.SetDestination(targetPos);

            if (!owner.Agent.pathPending && owner.Agent.remainingDistance < 0.1f)
            {
                if (!isWaiting)
                {
                    if (!owner.LoopPatrol && currentPatrolIndex == owner.PatrolPoints.Length)
                    {
                        return;
                    }

                    isWaiting = true;
                    waitTimer = owner.WaypointWaitTime;
                }
            }
        }
    }
}

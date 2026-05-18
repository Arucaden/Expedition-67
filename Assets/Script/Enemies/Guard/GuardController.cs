using UnityEngine;
using UnityEngine.AI;
using Expedition67.Core;

namespace Expedition67.Enemies.Guard
{
    public class GuardController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float patrolSpeed = 2f;
        public float PatrolSpeed => patrolSpeed;
        
        [SerializeField] private float chaseSpeed = 4f;
        public float ChaseSpeed => chaseSpeed;

        [Header("Hearing & Communication")]
        [SerializeField] private float hearingRange = 5f;
        public float HearingRange => hearingRange;
        
        [SerializeField] private float communicationRadius = 15f;
        public float CommunicationRadius => communicationRadius;

        [Header("References")]
        [SerializeField] private GameObject alertIcon;
        public GameObject AlertIcon => alertIcon;

        [Header("Behavior")]
        [SerializeField] private float searchDuration = 3f;
        public float SearchDuration => searchDuration;
        
        [SerializeField] private float timeToCatch = 1.5f;
        public float TimeToCatch => timeToCatch;
        
        [SerializeField] private float suspiciousDelay = 1.5f;
        public float SuspiciousDelay => suspiciousDelay;

        [Header("Patrol")]
        [SerializeField] private Transform[] patrolPoints;
        public Transform[] PatrolPoints => patrolPoints;
        
        [SerializeField] private bool loopPatrol = true;
        public bool LoopPatrol => loopPatrol;
        
        [SerializeField] private float waypointWaitTime = 2f;
        public float WaypointWaitTime => waypointWaitTime;

        public Transform Player { get; set; }
        public Vector2 LastKnownPosition { get; set; }
        public Vector2 InitialPosition { get; private set; }

        [SerializeField] private NavMeshAgent agent;
        public NavMeshAgent Agent => agent;

        [SerializeField] private EnemyDetectionCone fov;
        public EnemyDetectionCone Fov => fov;

        public StateMachine<GuardController> StateMachine { get; private set; } = new();

        public GuardState_Idle IdleState { get; private set; }
        public GuardState_Suspicious SuspiciousState { get; private set; }
        public GuardState_Alert AlertState { get; private set; }
        public GuardState_Searching SearchingState { get; private set; }

        private void Awake()
        {
            InitialPosition = transform.position;

            if (agent == null) agent = GetComponent<NavMeshAgent>();
            if (agent != null)
            {
                agent.updateRotation = false;
                agent.updateUpAxis = false;
            }

            if (fov == null) fov = GetComponentInChildren<EnemyDetectionCone>();

            IdleState = new GuardState_Idle(this);
            SuspiciousState = new GuardState_Suspicious(this);
            AlertState = new GuardState_Alert(this);
            SearchingState = new GuardState_Searching(this);
        }

        private void OnEnable()
        {
            GameEvents.OnGuardAlerted += HandleGuardAlerted;
            GameEvents.OnNoiseMade += HandleNoiseMade;
        }

        private void OnDisable()
        {
            GameEvents.OnGuardAlerted -= HandleGuardAlerted;
            GameEvents.OnNoiseMade -= HandleNoiseMade;
        }

        private void Start()
        {
            if (Player == null && GameManager.Instance != null)
            {
                var playerController = GameManager.Instance.GetPlayer();
                if (playerController != null)
                {
                    Player = playerController.transform;
                }
            }

            ChangeState(IdleState);
        }

        private void Update()
        {
            if (CanSeePlayer() && StateMachine.CurrentState != AlertState)
            {
                ChangeState(AlertState);
            }

            StateMachine.OnUpdate();

            if (Agent != null && Agent.velocity.sqrMagnitude > 0.01f)
            {
                transform.up = Agent.velocity.normalized;
            }
        }

        public void ChangeState(BaseState<GuardController> newState)
        {
            StateMachine.ChangeState(newState);
        }

        public bool CanSeePlayer()
        {
            if (Player == null || fov == null) 
            {
                return false;
            }
            
            return fov.CanSeeTarget(Player);
        }

        private void HandleGuardAlerted(Vector2 alertPosition)
        {
            if (StateMachine.CurrentState == IdleState)
            {
                float distance = Vector2.Distance(transform.position, alertPosition);
                if (distance <= CommunicationRadius)
                {
                    LastKnownPosition = alertPosition;
                    ChangeState(SearchingState);
                }
            }
        }

        private void HandleNoiseMade(Vector2 noisePosition, float noiseRadius)
        {
            if (StateMachine.CurrentState == IdleState)
            {
                float distance = Vector2.Distance(transform.position, noisePosition);
                if (distance <= noiseRadius + HearingRange)
                {
                    LastKnownPosition = noisePosition;
                    ChangeState(SuspiciousState);
                }
            }
        }
    }
}

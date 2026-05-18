using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Expedition67.Enemies
{
    public class EnemyDetectionCone : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform eyePoint;
        [SerializeField] private Light2D visionLight;

        [Header("FOV Settings")]
        [SerializeField] private float viewRadius = 5f;
        [Range(0f, 360f)]
        [SerializeField] private float viewAngle = 90f;

        [Header("Obstacles")]
        [SerializeField] private LayerMask obstacleMask;

        [Header("Colors")]
        [SerializeField] private Color normalColor = Color.yellow;
        [SerializeField] private Color alertColor = Color.red;

        private void Reset()
        {
            eyePoint = transform;
            visionLight = GetComponentInChildren<Light2D>();
        }

        private void Awake()
        {
            if (eyePoint == null)
            {
                eyePoint = transform;
            }

            SyncLight();
        }

        private void OnValidate()
        {
            if (eyePoint == null) 
            {
                eyePoint = transform;
            }

            SyncLight();
        }

        public bool CanSeeTarget(Transform target)
        {
            if (target == null) return false;

            Vector2 origin = eyePoint.position;
            Vector2 targetPosition = target.position;

            Vector2 directionToTarget = targetPosition - origin;
            float distanceToTarget = directionToTarget.magnitude;

            if (distanceToTarget > viewRadius) 
            {
                return false;
            }

            directionToTarget.Normalize();

            float angleToTarget = Vector2.Angle(eyePoint.up, directionToTarget);

            if (angleToTarget > viewAngle / 2f) 
            {
                return false;
            }

            RaycastHit2D hit = Physics2D.Raycast(origin, directionToTarget, distanceToTarget, obstacleMask);

            return hit.collider == null;
        }

        private void SyncLight()
        {
            if (visionLight == null) return;

            visionLight.pointLightOuterRadius = viewRadius;
            visionLight.pointLightOuterAngle = viewAngle;

            visionLight.pointLightInnerRadius = 0f;
            visionLight.pointLightInnerAngle = 0f;

            visionLight.color = normalColor;
        }

        public void SetAlertState(bool isAlert)
        {
            if (visionLight != null)
            {
                visionLight.color = isAlert ? alertColor : normalColor;
            }
        }

        private void OnDrawGizmos()
        {
            Transform originTransform = eyePoint != null ? eyePoint : transform;
            Vector3 origin = originTransform.position;

            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(origin, viewRadius);

            Vector3 leftDirection = Quaternion.Euler(0f, 0f, viewAngle / 2f) * originTransform.up;
            Vector3 rightDirection = Quaternion.Euler(0f, 0f, -viewAngle / 2f) * originTransform.up;

            Gizmos.color = Color.red;
            Gizmos.DrawLine(origin, origin + leftDirection * viewRadius);
            Gizmos.DrawLine(origin, origin + rightDirection * viewRadius);
        }
    }
}
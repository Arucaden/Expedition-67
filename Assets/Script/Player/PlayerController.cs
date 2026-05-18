using UnityEngine;
using UnityEngine.InputSystem;
using Expedition67.Core;

namespace Expedition67.Player
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private SpriteRenderer spriteRenderer;

        [Header("Sprites")]
        [SerializeField] private Sprite spriteUp;
        [SerializeField] private Sprite spriteDown;
        [SerializeField] private Sprite spriteSide;

        [Header("Movement")]
        [SerializeField] private float walkSpeed = 5f;
        [SerializeField] private float sneakSpeed = 2.5f;

        [Header("Noise")]
        [SerializeField] private float walkNoiseRadius = 5f;
        [SerializeField] private float sneakNoiseRadius = 0f;

        [Header("Input")]
        [SerializeField] private InputActionReference moveAction;
        [SerializeField] private InputActionReference sneakAction;

        private Vector2 currentMovement;
        private bool isSneaking;

        private void OnEnable()
        {
            if (moveAction != null)
            {
                moveAction.action.Enable();   
            }
            if (sneakAction != null) 
            {
                sneakAction.action.Enable();
            }
        }

        private void OnDisable()
        {
            if (moveAction != null) 
            {
                moveAction.action.Disable();
            }
            if (sneakAction != null) 
            {
                sneakAction.action.Disable();
            }
        }

        private void Update()
        {
            currentMovement = moveAction.action.ReadValue<Vector2>();
            isSneaking = sneakAction.action.IsPressed();

            UpdateSprite();
        }

        private void FixedUpdate()
        {
            Move();
        }

        private void UpdateSprite()
        {
            if (currentMovement == Vector2.zero) 
            {
                return;
            }

            if (Mathf.Abs(currentMovement.x) > Mathf.Abs(currentMovement.y) && spriteSide != null)
            {
                spriteRenderer.sprite = spriteSide;
                spriteRenderer.flipX = currentMovement.x < 0;
            }
            else
            {
                if (currentMovement.y > 0 && spriteUp != null)
                {
                    spriteRenderer.sprite = spriteUp;
                }
                else if (currentMovement.y < 0 && spriteDown != null)
                {
                    spriteRenderer.sprite = spriteDown;
                }
                spriteRenderer.flipX = false;
            }
        }

        private void Move()
        {
            float currentSpeed = isSneaking ? sneakSpeed : walkSpeed;
            float currentNoiseRadius = isSneaking ? sneakNoiseRadius : walkNoiseRadius;

            rb.linearVelocity = currentMovement * currentSpeed;

            if (currentMovement != Vector2.zero && currentNoiseRadius > 0f)
            {
                GameEvents.OnNoiseMade?.Invoke(transform.position, currentNoiseRadius);
            }
        }
    }
}

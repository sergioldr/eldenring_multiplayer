using UnityEngine;

namespace SL
{
    public class CharacterLocomotionManager : MonoBehaviour
    {
        private CharacterManager characterManager;

        [Header("Ground Check & Jumping")]
        [SerializeField] protected float gravityForce = -5.55f;
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private float groundCheckSphereRadius = 1f;
        [SerializeField] protected Vector3 jumpForceVelocity;
        [SerializeField] protected float groundForceVelocity = -20f;
        [SerializeField] protected float fallStartForceVelocity = -5f;
        private protected bool fallingVelocityHasBeenSet = false;
        private protected float inAirTimer = 0.0f;


        protected virtual void Awake()
        {
            characterManager = GetComponent<CharacterManager>();
        }

        protected virtual void Update()
        {
            HandleGroundCheck();

            if (characterManager.GetIsGrounded())
            {
                if (jumpForceVelocity.y < 0)
                {
                    inAirTimer = 0.0f;
                    fallingVelocityHasBeenSet = false;
                    jumpForceVelocity.y = groundForceVelocity;
                }
            }
            else
            {
                if (!characterManager.GetIsJumping() && !fallingVelocityHasBeenSet)
                {
                    fallingVelocityHasBeenSet = true;
                    jumpForceVelocity.y = fallStartForceVelocity;
                }

                inAirTimer += Time.deltaTime;
                characterManager.GetCharacterAnimator().SetFloat("InAirTimer", inAirTimer);
                jumpForceVelocity.y += gravityForce * Time.deltaTime;
            }

            // APPLY GRAVITY
            characterManager.GetCharacterController().Move(jumpForceVelocity * Time.deltaTime);
        }

        protected void HandleGroundCheck()
        {
            bool isGrounded = Physics.CheckSphere(characterManager.transform.position, groundCheckSphereRadius, groundLayer);
            characterManager.SetIsGrounded(isGrounded);
        }

        protected void OnDrawGizmosSelected()
        {
            Gizmos.DrawSphere(characterManager.transform.position, groundCheckSphereRadius);
        }
    }
}


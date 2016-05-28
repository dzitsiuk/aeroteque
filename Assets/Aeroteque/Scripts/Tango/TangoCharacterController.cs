using System;
using UnityEngine;

namespace Characters.FirstPerson
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    public class TangoCharacterController : MonoBehaviour
    {
        public static event GameActions.SimpleAction Landing;
        
        [Serializable]
        public class MovementSettings
        {
            public bool autoJump = false;
            public float JumpForce = 30f;
            public Vector3 DescentForce = Vector3.zero;
        }


        [Serializable]
        public class AdvancedSettings
        {
            public float groundCheckDistance = 0.01f; // distance for checking if the controller is grounded ( 0.01f seems to work best for this )
            public float stickToGroundHelperDistance = 0.5f; // stops the character
            public float groundDrag = 5f; // rigidbody's drag on ground
            public bool airControl; // can the user control the direction that is being moved in the air
            public float airSpeedMultiplier = 1f; // Speed multiplier when in air
            public float airDrag = 1f; // rigidbody's drag in air
        }


        public Camera cam;
        public MovementSettings movementSettings = new MovementSettings();
        public AdvancedSettings advancedSettings = new AdvancedSettings();

        private Rigidbody m_RigidBody;
        private CapsuleCollider m_Capsule;
        private Collider m_Collider;
        private float m_YRotation;
        private bool m_JumpRequested, m_PreviouslyGrounded, m_IsInAir, m_IsGrounded;


        public Vector3 Velocity
        {
            get { return m_RigidBody.velocity; }
        }

        public bool Grounded
        {
            get { return m_IsGrounded; }
        }

        public bool IsInAir
        {
            get { return m_IsInAir; }
        }


        private void Start() {
            m_RigidBody = GetComponent<Rigidbody>();
            m_Capsule = GetComponent<CapsuleCollider>();
            m_Collider = GetComponent<Collider>();
        }


        private void Update()
        {
            // If falling down, turn on collider.
            if (m_RigidBody.velocity.y <= 0f && !m_Collider.enabled) {
                m_Collider.enabled = true;
            }
        }

        private void FixedUpdate()
        {
            GroundCheck();

            if (m_IsGrounded) {
                m_RigidBody.drag = advancedSettings.groundDrag;
           
                if (!m_PreviouslyGrounded) {
                    GetComponent<Rigidbody>().velocity = GetComponent<Rigidbody>().velocity * 0.5f;
                    
                    // Just landed.
                    if (Landing != null) {
                        Landing();
                    }
                    
                    if (movementSettings.autoJump) {
                        Jump();
                    }
                }
            } else {  // Not grounded.
                m_RigidBody.drag = advancedSettings.airDrag;
                
                if (m_RigidBody.velocity.y < 0f) {
					m_RigidBody.AddForce(movementSettings.DescentForce, ForceMode.Acceleration);
                }
            }
        }
        
        private void Jump() {
            m_RigidBody.drag = advancedSettings.airDrag;
            m_RigidBody.velocity = new Vector3(m_RigidBody.velocity.x, 0f, m_RigidBody.velocity.z);
            m_RigidBody.AddForce(new Vector3(0f, movementSettings.JumpForce, 0f), ForceMode.Impulse);
            
            // Turn off collider when going up to avoid bumping into platforms.
            m_Collider.enabled = false;
            
            m_IsInAir = true;
        }

        /// sphere cast down just beyond the bottom of the capsule to see if the capsule is colliding round the bottom
        private void GroundCheck()
        {
            m_PreviouslyGrounded = m_IsGrounded;
            RaycastHit hitInfo;
            if (Physics.SphereCast(transform.position, m_Capsule.radius, Vector3.down, out hitInfo,
                                   ((m_Capsule.height / 2f) - m_Capsule.radius) + advancedSettings.groundCheckDistance))
            {
                m_IsGrounded = true;
            }
            else
            {
                m_IsGrounded = false;
            }
            if (!m_PreviouslyGrounded && m_IsGrounded && m_IsInAir)
            {
                m_IsInAir = false;
            }
        }
    }
}

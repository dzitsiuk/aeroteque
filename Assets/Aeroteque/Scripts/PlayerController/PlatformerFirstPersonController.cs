//-----------------------------------------------------------------------
// Aeroteque - desktop, VR and Google Tango game made with Unity.
// Copyright (C) 2016  Dustyroom
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//-----------------------------------------------------------------------
using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Characters.FirstPerson;

namespace Characters.FirstPerson
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    public class PlatformerFirstPersonController : MonoBehaviour
    {
        public static event GameActions.SimpleAction Landing;
        
        [Serializable]
        public class MovementSettings
        {
            public float Speed = 20f;
            public ForceMode forceMode = ForceMode.Impulse;
            
            [Space]
            public float ForwardMultiplier = 1.0f;   // Speed when walking forward
            public float BackwardMultiplier = 1.0f;  // Speed when walking backwards
            public float StrafeMultiplier = 1.0f;    // Speed when walking sideways
            
            [Space]
            public float RunMultiplier = 2.0f;   // Speed when sprinting
            public KeyCode RunKey = KeyCode.LeftShift;
            
            [Space]
            public bool autoJump = false;
            public bool doubleJump = false;
            public float JumpForce = 30f;
            public Vector3 DescentForce = Vector3.zero;
            
            [Space]
            public AnimationCurve SlopeCurveModifier = new AnimationCurve(
                new Keyframe(-90.0f, 1.0f), new Keyframe(0.0f, 1.0f), new Keyframe(90.0f, 0.0f));
            [HideInInspector]
            public float CurrentTargetSpeed = 8f;

#if !MOBILE_INPUT
            private bool m_Running;
#endif
            private bool m_UsedDoubleJump = false;

            public void UpdateDesiredTargetSpeed(Vector2 input)
            {
                if (input == Vector2.zero) return;
                CurrentTargetSpeed = Speed;
                if (input.x > 0 || input.x < 0)
                {
                    //strafe
                    CurrentTargetSpeed *= StrafeMultiplier;
                }
                if (input.y < 0)
                {
                    //backwards
                    CurrentTargetSpeed *= BackwardMultiplier;
                }
                if (input.y > 0)
                {
                    // Forwards /handled last as if strafing and moving forward at the same time 
                    // forwards speed should take precedence.
                    CurrentTargetSpeed *= ForwardMultiplier;
                }
#if !MOBILE_INPUT
                if (Input.GetKey(RunKey))
                {
                    CurrentTargetSpeed *= RunMultiplier;
                    m_Running = true;
                }
                else
                {
                    m_Running = false;
                }
#endif
            }

#if !MOBILE_INPUT
            public bool Running
            {
                get { return m_Running; }
            }
#endif

            public bool UsedDoubleJump
            {
                get { return m_UsedDoubleJump; }
                set { m_UsedDoubleJump = value; }
            }
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
        public MouseLook mouseLook = new MouseLook();
        public AdvancedSettings advancedSettings = new AdvancedSettings();


        private Rigidbody m_RigidBody;
        private CapsuleCollider m_Capsule;
        private Collider m_Collider;
        private float m_YRotation;
        private Vector3 m_GroundContactNormal;
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

        public bool Running
        {
            get
            {
#if !MOBILE_INPUT
                return movementSettings.Running;
#else
	            return false;
#endif
            }
        }


        private void Start() {
            m_RigidBody = GetComponent<Rigidbody>();
            m_Capsule = GetComponent<CapsuleCollider>();
            m_Collider = GetComponent<Collider>();
            mouseLook.Init(transform, cam.transform);
        }


        private void Update()
        {
            RotateView();

            if (CrossPlatformInputManager.GetButtonDown("Jump") && !m_JumpRequested) {
                m_JumpRequested = true;
            }
            
            // If falling down, turn on collider.
            if (m_RigidBody.velocity.y <= 0f && !m_Collider.enabled) {
                m_Collider.enabled = true;
            }
        }


        private void FixedUpdate()
        {
            GroundCheck();
            Vector2 input = GetInput();

            // If can move and try to move
            if ((Mathf.Abs(input.x) > float.Epsilon || Mathf.Abs(input.y) > float.Epsilon) &&
                (advancedSettings.airControl || m_IsGrounded))
            {
                // always move along the camera forward as it is the direction that it being aimed at
                // cam.transform.up * 0.001f is needed to still move if looking straight down
                float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(cam.transform.localRotation.x / cam.transform.localRotation.w);
                float lookSwap = (0f * angleX) < 90f? 1f : -1f;
                Vector3 desiredMove = (cam.transform.forward + cam.transform.up * 0.001f) * lookSwap * input.y +
                                      cam.transform.right * input.x;
                // project desired movement vector onto the ground to avoid walking on air
                desiredMove = Vector3.ProjectOnPlane(desiredMove, m_GroundContactNormal).normalized;
                desiredMove *= movementSettings.CurrentTargetSpeed;
                
                float airFactor = m_IsGrounded? 1f : advancedSettings.airSpeedMultiplier;
                desiredMove *= airFactor;
                
                float horizontalVelSqr = Vector3.ProjectOnPlane(m_RigidBody.velocity, m_GroundContactNormal).sqrMagnitude;
                float targetVelSqr = movementSettings.CurrentTargetSpeed * movementSettings.CurrentTargetSpeed * 
                        airFactor * airFactor;
                if (horizontalVelSqr < targetVelSqr) {
                    m_RigidBody.AddForce(desiredMove * SlopeMultiplier(), movementSettings.forceMode);
                }
            }

            if (m_IsGrounded) {
                movementSettings.UsedDoubleJump = false;
                m_RigidBody.drag = advancedSettings.groundDrag;

                if (m_JumpRequested) {
                    Jump();
                }
                
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
            }
            else
            {  // Not grounded.
                if (m_JumpRequested && movementSettings.doubleJump && !movementSettings.UsedDoubleJump) {
                    // Use double jump.
                    Jump();
                    movementSettings.UsedDoubleJump = true;
                }
                
                m_RigidBody.drag = advancedSettings.airDrag;
                if (m_PreviouslyGrounded && !m_IsInAir) {
                    StickToGroundHelper();
                }
                
                if (m_RigidBody.velocity.y < 0f) {
					m_RigidBody.AddForce(movementSettings.DescentForce, ForceMode.Acceleration);
                }
            }
            m_JumpRequested = false;
        }
        
        private void Jump() {
            m_RigidBody.drag = advancedSettings.airDrag;
            m_RigidBody.velocity = new Vector3(m_RigidBody.velocity.x, 0f, m_RigidBody.velocity.z);
            m_RigidBody.AddForce(new Vector3(0f, movementSettings.JumpForce, 0f), ForceMode.Impulse);
            
            // Turn off collider when going up to avoid bumping into platforms.
            m_Collider.enabled = false;
            
            m_IsInAir = true;
        }


        private float SlopeMultiplier() {
            float angle = Vector3.Angle(m_GroundContactNormal, Vector3.up);
            return movementSettings.SlopeCurveModifier.Evaluate(angle);
        }


        private void StickToGroundHelper()
        {
            RaycastHit hitInfo;
            if (Physics.SphereCast(transform.position, m_Capsule.radius, Vector3.down, out hitInfo,
                                   ((m_Capsule.height / 2f) - m_Capsule.radius) +
                                   advancedSettings.stickToGroundHelperDistance))
            {
                if (Mathf.Abs(Vector3.Angle(hitInfo.normal, Vector3.up)) < 85f)
                {
                    m_RigidBody.velocity = Vector3.ProjectOnPlane(m_RigidBody.velocity, hitInfo.normal);
                }
            }
        }


        private Vector2 GetInput()
        {
            Vector2 input = new Vector2
            {
                x = CrossPlatformInputManager.GetAxis("Horizontal"),
                y = CrossPlatformInputManager.GetAxis("Vertical")
            };
            movementSettings.UpdateDesiredTargetSpeed(input);
            return input;
        }


        private void RotateView()
        {
            //avoids the mouse looking if the game is effectively paused
            if (Mathf.Abs(Time.timeScale) < float.Epsilon) return;

            // get the rotation before it's changed
            float oldYRotation = transform.eulerAngles.y;

            mouseLook.LookRotation(transform, cam.transform);

            if (m_IsGrounded || advancedSettings.airControl)
            {
                // Rotate the rigidbody velocity to match the new direction that the character is looking
                Quaternion velRotation = Quaternion.AngleAxis(transform.eulerAngles.y - oldYRotation, Vector3.up);
                m_RigidBody.velocity = velRotation * m_RigidBody.velocity;
            }
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
                m_GroundContactNormal = hitInfo.normal;
            }
            else
            {
                m_IsGrounded = false;
                m_GroundContactNormal = Vector3.up;
            }
            if (!m_PreviouslyGrounded && m_IsGrounded && m_IsInAir)
            {
                m_IsInAir = false;
            }
        }
    }
}

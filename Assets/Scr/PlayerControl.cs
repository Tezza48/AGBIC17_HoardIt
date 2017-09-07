using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Input = HoardIt.Core.Input;

namespace HoardIt.Assets
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerControl : MonoBehaviour
    {
        [Header("Movement")]
        public float m_MoveSpeedBase = 400.0f;// Speed to move at
        public float m_RollDistance = 2.0f;// distance to cover in a roll
        public float m_RollTime = 0.5f;// time it takes to roll
        public float m_RollCooldown = 2.0f;// cooldown before player can roll again

        [Header("Player")]
        public Transform Ref_PlayerArtwork;
        [SerializeField][Range(0, 3)]
        private int m_PlayerIndex = 0;// which player is this controlling

        private float m_FacingRotation;

        private Rigidbody2D m_Rigidbody;

#if Debug || _Debug
        public float _RigidSpeed;
#endif

        // Use this for initialization
        void Start ()
        {
            m_Rigidbody = GetComponent<Rigidbody2D>();
	    }
	
	    // Update is called once per frame
	    void Update ()
        {
            if (Input.getButtonDown(m_PlayerIndex, 1))
            {

            }
	    }

        void FixedUpdate()
        {
            Vector2 direction = new Vector2(Input.getHorizontal(m_PlayerIndex), Input.getVertical(m_PlayerIndex));
            m_FacingRotation = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            if (direction.magnitude > 0)
            {
                direction.Normalize();
                Ref_PlayerArtwork.rotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, m_FacingRotation - 90));
            }

            m_Rigidbody.velocity = direction * m_MoveSpeedBase * Time.deltaTime;

#if Debug || _Debug
            _RigidSpeed = m_Rigidbody.velocity.magnitude;
# endif
        }
    }
}

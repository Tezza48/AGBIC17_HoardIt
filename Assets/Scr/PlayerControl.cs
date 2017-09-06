#define Debug
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
        public float MOVEMENT_DAMPENING = 2.0f;

        //[SerializeField][Range(1.0f, 50.0f)]
        public float BASE_MOVE_SPEED = 1.0f;

        [SerializeField][Range(0, 3)]
        private int m_PlayerIndex = 0;// which player is this controlling

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

	    }

        void FixedUpdate()
        {
            Vector2 direction = new Vector2(Input.getHorizontal(m_PlayerIndex), Input.getVertical(m_PlayerIndex));

            if (direction.magnitude > 0)
                direction.Normalize();
            //else if(m_Rigidbody.velocity.magnitude > 0)
            //{
            //    direction = -m_Rigidbody.velocity / MOVEMENT_DAMPENING;
            //}

            m_Rigidbody.velocity = direction * BASE_MOVE_SPEED * Time.deltaTime;

#if Debug || _Debug
            _RigidSpeed = m_Rigidbody.velocity.magnitude;
#endif

            bool roll = Input.getButtonDown(m_PlayerIndex, 0);

            if (roll)
            {
                Debug.Log("Do Roll");
            }
        }
    }
}

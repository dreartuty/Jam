using Unity.Netcode;
using UnityEngine;
#if NEW_INPUT_SYSTEM_INSTALLED
using UnityEngine.InputSystem;
#endif

namespace Unity.Multiplayer.Center.NetcodeForGameObjectsExample
{
    public class ClientAuthoritativeMovement : NetworkBehaviour
    {
        public float speed = 5;
        public float acceleration = 2;
        public float deceleration = 4;
        private float currentAcceleration;

        void Update()
        {

            if (!IsOwner || !IsSpawned) return;

            if (Input.GetAxisRaw("Horizontal") != 0)
            {
                currentAcceleration += Time.deltaTime * acceleration * Input.GetAxisRaw("Horizontal");
                if (currentAcceleration > 1) currentAcceleration = 1;
                else if (currentAcceleration < -1) currentAcceleration = -1;
            }
            else if (currentAcceleration != 0)
            {
                currentAcceleration -= Time.deltaTime * deceleration;
                if (currentAcceleration < 0) currentAcceleration = 0;
            }
            var multiplier = currentAcceleration * speed * Time.deltaTime;
            transform.position += new Vector3(multiplier, 0, 0);
        }
    }
}
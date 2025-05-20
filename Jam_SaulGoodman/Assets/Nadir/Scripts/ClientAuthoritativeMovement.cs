using Unity.Netcode;
using UnityEngine;
#if NEW_INPUT_SYSTEM_INSTALLED
using UnityEngine.InputSystem;
#endif

namespace Unity.Multiplayer.Center.NetcodeForGameObjectsExample
{
    public class ClientAuthoritativeMovement : NetworkBehaviour
    {
        public float Speed = 5;

        void Update()
        {

            if (!IsOwner || !IsSpawned) return;

            var multiplier = Speed * Time.deltaTime * Input.GetAxisRaw("Horizontal");
            transform.position += new Vector3(multiplier, 0, 0);


/*#if ENABLE_INPUT_SYSTEM && NEW_INPUT_SYSTEM_INSTALLED
            // New input system backends are enabled.
            if (Keyboard.current.aKey.isPressed)
            {
                transform.position += new Vector3(-multiplier, 0, 0);
            }
            else if (Keyboard.current.dKey.isPressed)
            {
                transform.position += new Vector3(multiplier, 0, 0);
            }
#else
            // Old input backends are enabled.
            if (Input.GetKey(KeyCode.A))
            {
                transform.position += new Vector3(-multiplier, 0, 0);
            }
            else if(Input.GetKey(KeyCode.D))
            {
                transform.position += new Vector3(multiplier, 0, 0);
            }

#endif*/
        }
    }
}

using System.Numerics;
using Unity.Netcode;
using Unity.Netcode.Components;
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
        private NetworkTransform netTransform;

        public UnityEngine.Vector3 TargetPosition1;
        public UnityEngine.Vector3 TargetPosition2;


        private void Awake()
        {
            netTransform = GetComponent<NetworkTransform>();
        }

        void Update()
        {

            if (!IsSpawned) return;

            if (Input.GetAxisRaw("Horizontal") != 0)
            {
                currentAcceleration += Time.deltaTime * acceleration * Input.GetAxisRaw("Horizontal");
                if (currentAcceleration > 1) currentAcceleration = 1;
                else if (currentAcceleration < -1) currentAcceleration = -1;
            }
            else if (currentAcceleration != 0)
            {
                if (currentAcceleration > 0)
                {
                    currentAcceleration -= Time.deltaTime * deceleration;
                    if (currentAcceleration < 0) currentAcceleration = 0;
                }
                //a
                else if (currentAcceleration < 0)
                {
                    currentAcceleration += Time.deltaTime * deceleration;
                    if (currentAcceleration > 0) currentAcceleration = 0;
                }
            }
            var multiplier = currentAcceleration * speed * Time.deltaTime;
            transform.position += new UnityEngine.Vector3(multiplier, 0, 0);

            TargetPosition1 = GameManager.Instance.TargetPosition1.Value;
            TargetPosition2 = GameManager.Instance.TargetPosition2.Value;

            Debug.Log(NetworkManager.Singleton.LocalClientId);
        }

        public void OnEnable()
        {
            GameManager.Instance.TargetPosition1.OnValueChanged += OnTargetPosition1Changed;
            GameManager.Instance.TargetPosition2.OnValueChanged += OnTargetPosition2Changed;
        }
        public void OnDisable()
        {
            GameManager.Instance.TargetPosition1.OnValueChanged -= OnTargetPosition1Changed;
            GameManager.Instance.TargetPosition2.OnValueChanged -= OnTargetPosition2Changed;
        }
        private void OnTargetPosition1Changed(UnityEngine.Vector3 previousValue, UnityEngine.Vector3 newValue)
        {
            if (NetworkManager.Singleton.LocalClientId == 1)
                transform.position = newValue;
            else return;


        }
        private void OnTargetPosition2Changed(UnityEngine.Vector3 previousValue, UnityEngine.Vector3 newValue)
        {
            if (NetworkManager.Singleton.LocalClientId == 2)
                transform.position = newValue;
            else return;
        }
    }
}
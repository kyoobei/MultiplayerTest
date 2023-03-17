using Fusion;
using UnityEngine;
using TMPro;
namespace Gelo.Game
{
    public class PlayerInput : NetworkBehaviour
    {
        private NetworkCharacterControllerPrototype m_networkCharController;

        [Networked]
        public Vector3 MovementDirection { get; set; }
        [SerializeField]
        private float m_movementSpeed;

        private void Awake()
        {
            m_networkCharController = GetComponent<NetworkCharacterControllerPrototype>();
        }
        public override void FixedUpdateNetwork()
        {
            Vector3 direction;
            if (GetInput(out NetworkInputData data))
            {
                direction = default;

                if (data.IsDown((int)InputButtons.Forward))
                {
                    direction += Vector3.forward;
                }
                if (data.IsDown((int)InputButtons.Backward))
                {
                    direction -= Vector3.forward;
                }
                if (data.IsDown((int)InputButtons.Left))
                {
                    direction -= Vector3.right;
                }
                if (data.IsDown((int)InputButtons.Right))
                {
                    direction += Vector3.right;
                }

                direction = direction.normalized;

                MovementDirection = direction;
            }
            else
            {
                direction = MovementDirection;
            }

            if (m_networkCharController)
            {
                m_networkCharController.Move(direction);
            }
        }
    }
}

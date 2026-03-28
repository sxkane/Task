using UnityEngine;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        public PlayerStats Stats { get; private set; }
        public PlayerInputHandler Input { get; private set; }
        public PlayerMove Move { get; private set; }
        public PlayerVisual Visual { get; private set; }

        public Vector2 AimDirection { get; private set; } = Vector2.right;
        public bool FacingRight { get; private set; } = true;
        
        private void Update()
        {
            Vector2 move = Input.MoveInput;

            if (move != Vector2.zero)
            {
                AimDirection = move.normalized;

                if (move.x != 0)
                    FacingRight = move.x > 0;
            }
        }

        public void Initialize(PlayerStats stats)
        {
            Stats = stats;
            Input = GetComponent<PlayerInputHandler>();
            Move = GetComponent<PlayerMove>();
            Visual = GetComponentInChildren<PlayerVisual>();
            
            Stats.Initialize();
            Input.Initialize();
            Move.Initialize();
            Visual.Initialize();
        }
    }
}

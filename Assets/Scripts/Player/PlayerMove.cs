using UnityEngine;

namespace Player
{
    public class PlayerMove : MonoBehaviour
    {
        private Rigidbody2D _rb;
        private PlayerController _player;

        private bool _initialized;

        private void FixedUpdate()
        {
            if (!_initialized)
                return;
            
            _rb.linearVelocity =
                _player.Input.MoveInput * (5f * _player.Stats.MoveSpeedMultiplier);
        }

        public void Initialize()
        {
            _rb = GetComponent<Rigidbody2D>();
            _player = GetComponent<PlayerController>();

            _initialized = true;
        }
    }
}
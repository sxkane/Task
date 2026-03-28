using UnityEngine;

namespace Player
{
    public class PlayerVisual : MonoBehaviour
    {
        private PlayerController _player;
        private Animator _animator;

        private bool _initialized;
        
        private void Update()
        {
            if (!_initialized)
                return;
            
            transform.localScale = new Vector3(
                _player.FacingRight ? 1 : -1,
                1, 1);
        
            bool moving = _player.Input.MoveInput != Vector2.zero;
            _animator.SetBool("Move",  moving);
        }
        
        public void Initialize()
        {
            _player = GetComponentInParent<PlayerController>();
            _animator = GetComponent<Animator>();
            
            _initialized = true;
        }
    }
}
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerInputHandler : MonoBehaviour
    {
        public Vector2 MoveInput { get; private set; }
    
        private InputAction _moveAction;
        private bool _isInitialized;
        
        private void Update()
        {
            if (!_isInitialized)
                return;
            
            MoveInput = _moveAction.ReadValue<Vector2>();
        }

        public void Initialize()
        {
            _moveAction = InputSystem.actions.FindAction("Move");
            _isInitialized = true;
        }
    }
}
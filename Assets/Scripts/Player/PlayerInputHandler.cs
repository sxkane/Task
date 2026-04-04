using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerInputHandler : MonoBehaviour
    {
        public Vector2 MoveInput { get; private set; }
    
        private InputAction _moveAction;
        private bool _isInitialized;
        private bool _isInputEnabled;
        
        private void Update()
        {
            if (!_isInitialized || !_isInputEnabled)
            {
                MoveInput = Vector2.zero;
                return;
            }
            
            MoveInput = _moveAction.ReadValue<Vector2>();
        }

        public void Initialize()
        {
            _moveAction = InputSystem.actions.FindAction("Move");
            _isInitialized = true;
            _isInputEnabled = true;
        }

        public void EnableInput()
        {
            if (!_isInitialized)
                return;

            _isInputEnabled = true;
        }

        public void DisableInput()
        {
            MoveInput = Vector2.zero;
            _isInputEnabled = false;
        }
    }
}

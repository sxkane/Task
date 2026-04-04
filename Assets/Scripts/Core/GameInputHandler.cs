using UnityEngine;
using UnityEngine.InputSystem;

namespace Core
{
    public class GameInputHandler : MonoBehaviour
    {
        public bool IsPaused { get; private set; }
        
        private InputAction _pauseAction;
        private bool _isInitialized;
        private bool _isInputEnabled;

        private void Update()
        {
            if (!_isInitialized || !_isInputEnabled)
                return;

            if (_pauseAction.WasPressedThisFrame())
                IsPaused = true;
        }

        public void Initialize()
        {
            _pauseAction = InputSystem.actions.FindAction("Pause");
            _isInitialized = _pauseAction != null;
            _isInputEnabled = _isInitialized;
            IsPaused = false;
        }

        public void EnableInput()
        {
            if (!_isInitialized)
                return;

            _isInputEnabled = true;
        }

        public void DisableInput()
        {
            _isInputEnabled = false;
        }
    }
}
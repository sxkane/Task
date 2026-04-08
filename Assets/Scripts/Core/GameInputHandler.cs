using UnityEngine;
using UnityEngine.InputSystem;

namespace Core
{
    public class GameInputHandler : MonoBehaviour
    {
        private InputAction _pauseAction;
        private bool _pauseRequested;
        private bool _isInitialized;
        private bool _isInputEnabled;

        private void Update()
        {
            if (!_isInitialized || !_isInputEnabled)
                return;

            if (_pauseAction.WasPressedThisFrame())
                _pauseRequested = true;
        }

        public void Initialize()
        {
            _pauseAction = InputSystem.actions.FindAction("Pause");
            _isInitialized = _pauseAction != null;
            _isInputEnabled = _isInitialized;
            _pauseRequested = false;
        }

        public bool ConsumePauseRequest()
        {
            if (!_pauseRequested)
                return false;

            _pauseRequested = false;
            return true;
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
            _pauseRequested = false;
        }
    }
}

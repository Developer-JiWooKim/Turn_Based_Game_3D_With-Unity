using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.MyAssets.Scripts.WeaponSelectScene
{

    public class WeaponPreviewRotator : MonoBehaviour
    {
        [SerializeField] private float _autoRotateSpeed = 30f;  // 자동 회전 속도
        [SerializeField] private float _dragRotateSpeed = 0.3f; // 드래그 회전 속도

        private bool _isDragging = false;
        private Vector2 _lastMousePosition;

        private void Update()
        {
            if (_isDragging)
            {
                Vector2 currentMousePosition = Mouse.current.position.ReadValue();
                Vector2 delta = currentMousePosition - _lastMousePosition;
                transform.Rotate(Vector3.up, -delta.x * _dragRotateSpeed, Space.World);
                transform.Rotate(Vector3.right, delta.y * _dragRotateSpeed, Space.World);
                _lastMousePosition = currentMousePosition;
            }
            else
            {
                transform.Rotate(Vector3.up, _autoRotateSpeed * Time.deltaTime, Space.World);
            }
        }

        public void OnDragStart()
        {
            _isDragging = true;
            _lastMousePosition = Mouse.current.position.ReadValue();
        }

        public void OnDragEnd()
        {
            _isDragging = false;
        }

    }

}

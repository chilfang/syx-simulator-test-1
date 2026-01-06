using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    Vector2 cameraMovementVector = Vector2.zero;

    public float movementSensitivity = 2.5f;
    public float zoomSensitivity = 1f;

    private void FixedUpdate () {
        

        
    }

    private void LateUpdate () {
        Camera.main.transform.position += (Vector3) (cameraMovementVector * movementSensitivity * Time.deltaTime * Camera.main.orthographicSize);
    }

    //--------------------------------[[ Input ]]--------------------------------
    public void Move(InputAction.CallbackContext context) {
        cameraMovementVector = context.ReadValue<Vector2>();
    }

    public void Zoom (InputAction.CallbackContext context) {
        Camera.main.orthographicSize += context.ReadValue<float>() * zoomSensitivity * (Camera.main.orthographicSize / 20);
        Camera.main.orthographicSize = math.clamp(Camera.main.orthographicSize, 5, 100);
    }
}

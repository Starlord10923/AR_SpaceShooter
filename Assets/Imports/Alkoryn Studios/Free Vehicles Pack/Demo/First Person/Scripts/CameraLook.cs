using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLook : MonoBehaviour
{
    [SerializeField]
    private Transform fps_camera;

    [SerializeField]
    private float mouseSensivity = 100;

    // The input of the mouse in the X axis
    private float inputX;
    // The input of the mouse in the Y axis
    private float inputY;

    private Transform _myTransform;

    // The x rotation of the player
    private float xRotation;

    // Start is called before the first frame update
    void Start()
    {
        // Get the transform of this player
        _myTransform = transform;
        // Lock the cursor to the center of the screen
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        inputX = Input.GetAxis("Mouse X") * mouseSensivity * Time.deltaTime;
        inputY = Input.GetAxis("Mouse Y") * mouseSensivity * Time.deltaTime;

        xRotation -= inputY;
        // So the player can't look further than -70 and 70 in the Y axis
        xRotation = Mathf.Clamp(xRotation, -70, 70);

        // Rotate the camera in the Y axis
        fps_camera.localRotation = Quaternion.Euler(xRotation, 0, 0);
        // Rotate the body in the X axis
        _myTransform.Rotate(Vector3.up * inputX);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float gravity = -9.81f;
    [SerializeField]
    private float speed = 12f;

    [SerializeField]
    private Transform cam;

    // The character controller of this player
    private CharacterController cc;

    // Move along the X axis
    private float xAxis;
    // Move along the Z axis
    private float zAxis;

    // Start is called before the first frame update
    void Start()
    {
        cc = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        xAxis = Input.GetAxis("Horizontal");
        zAxis = Input.GetAxis("Vertical");

        // The direction that we will move in
        Vector3 moveDirection = cam.forward * zAxis + cam.right * xAxis + Vector3.up * gravity;

        cc.Move(speed * Time.deltaTime * moveDirection);
    }
}

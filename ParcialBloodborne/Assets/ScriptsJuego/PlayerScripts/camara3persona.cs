using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camara3persona : MonoBehaviour
{
    public float sensiMouse = 3.0f;

    private float rotY, rotX;

    public Transform target;

    public float distanceTarget = 3.0f;

    Vector3 curRotation;
    Vector3 smoothVelocity = Vector3.zero;

    [SerializeField]
    private float smoothTime = 0.2f;

    [SerializeField]
    private Vector2 MaxMinRota = new Vector2(-20, 40);


    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensiMouse;
        float mouseY = Input.GetAxis("Mouse Y") * sensiMouse;

        rotX -= mouseY;
        rotY -= mouseX;

        rotX = Mathf.Clamp(rotX, MaxMinRota.x, MaxMinRota.y);

        Vector3 nextRotation = new Vector3(rotX, rotY);


        curRotation = Vector3.SmoothDamp(curRotation, nextRotation, ref smoothVelocity, smoothTime);
        transform.localEulerAngles = curRotation;


        transform.position = target.position - transform.forward * distanceTarget;


    }
}

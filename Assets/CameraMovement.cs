using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{   
    public Transform target;
    public float smoothTime = 0.3f;

    private Vector3 velocity = Vector3.zero;

    void LateUpdate()
    {
        // Smoothly move the camera towards the target
        Vector3 targetPosition = new Vector3(Mathf.Min(0.2f,Mathf.Max(target.position.x, 0.0f)), Mathf.Min(5.0f,Mathf.Max(target.position.y, 0.0f)));
        Vector3 smoothPosition = Vector3.SmoothDamp(new Vector3(transform.position.x,transform.position.y,-10), new Vector3(targetPosition.x,targetPosition.y,-10), ref velocity, smoothTime);
        transform.position = smoothPosition;
    }
}

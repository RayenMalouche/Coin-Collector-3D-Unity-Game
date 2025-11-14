using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // Assign the Sphere's Transform here
    public Vector3 offset = new Vector3(0, 5, -10); // Adjust for your desired camera angle (e.g., above and behind)
    public float smoothSpeed = 0.125f; // Smoothing factor

    void Start()
    {
        if (target == null)
        {
            Debug.LogError("Assign a target (Sphere) to CameraFollow script!");
        }
    }

    void LateUpdate()
    {
        if (target != null)
        {
            Vector3 desiredPosition = target.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;

            transform.LookAt(target); // Always face the target
        }
    }
}

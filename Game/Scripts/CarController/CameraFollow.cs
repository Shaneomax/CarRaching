using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public float moveSmoothness = 5f;      // ✅ Set in Inspector or default here
    public float rotationSmoothness = 5f;  // ✅ Higher = smoother

    public Vector3 moveOffset;
    public Vector3 rotationOffset;

    public Transform carTarget;

    void HandleMovement()
    {
        Vector3 targetPos = carTarget.TransformPoint(moveOffset);
        transform.position = Vector3.Lerp(transform.position, targetPos, moveSmoothness * Time.deltaTime);
    }

    void HandleRotation()
    {
        var direction = carTarget.position - transform.position;
        var rotation = Quaternion.LookRotation(direction + rotationOffset, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, rotationSmoothness * Time.deltaTime);
    }

    void LateUpdate()
    {
        HandleMovement();
        HandleRotation();
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float m_RotationSpeed = 180f;
    private float xAngle = 0f, yAngle = 0f;

    void Update()
    {
        xAngle = Mathf.Clamp(xAngle - Input.GetAxis("Mouse Y") * m_RotationSpeed * Time.deltaTime, -85f, 85f);
        yAngle += Input.GetAxis("Mouse X") * m_RotationSpeed * Time.deltaTime;
        transform.rotation = Quaternion.Euler(xAngle, yAngle, 0f);
    }
}

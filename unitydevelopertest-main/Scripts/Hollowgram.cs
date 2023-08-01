using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hollowgram : MonoBehaviour
{
    float r, desiredDuration = 0.2f;
    public void RotatePlayerGravZAxis(float ang)
    {
        float Angle = Mathf.SmoothDampAngle(transform.eulerAngles.x, ang, ref r, desiredDuration);
        Vector3 desiredRot = new Vector3(Angle, 0f, 0f);
        transform.rotation = Quaternion.Euler(desiredRot);
    }

    public void RotatePlayerGravXAxis(float ang)
    {
        float Angle = Mathf.SmoothDampAngle(transform.eulerAngles.z, ang, ref r, desiredDuration);
        Vector3 desiredRot = new Vector3(0f, 0f, Angle);
        transform.rotation = Quaternion.Euler(desiredRot);
    }

    public void RotatePlayerGravYAxis(float ang)
    {
        float Angle = Mathf.SmoothDampAngle(transform.eulerAngles.z, ang, ref r, desiredDuration);
        Vector3 desiredRot = new Vector3(0f, 0f, Angle);
        transform.rotation = Quaternion.Euler(desiredRot);
    }
}

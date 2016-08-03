using UnityEngine;
using System.Collections.Generic;

public class CameraControl : MonoBehaviour {

    public float sensitivityX = 2.0f;
    public float sensitivityY = 2.0f;

    public float minimumY = -89.0f;
    public float maximumY = 89.0f;

    // Based off mouse axis, not rotational axis
    private float rotationX = 0.0f;
    private float rotationY = 0.0f;

    private List<float> rotArrayX = new List<float>();
    float rotAverageX = 0.0f;

    private List<float> rotArrayY = new List<float>();
    float rotAverageY = 0.0f;

    public float frameCounter = 5;

    Quaternion originalRotation;

    void Start()
    {
        originalRotation = transform.localRotation;
    }

    void Update()
    {
        rotAverageY = 0.0f;
        rotAverageX = 0.0f;

        rotationY += Input.GetAxisRaw("Mouse Y") * sensitivityY;
        rotationX += Input.GetAxisRaw("Mouse X") * sensitivityX;

        // Average out mouse speed to smooth rotation for less jitter
        rotArrayY.Add(rotationY);
        rotArrayX.Add(rotationX);

        if (rotArrayY.Count >= frameCounter)
        {
            rotArrayY.RemoveAt(0);
        }
        if (rotArrayX.Count >= frameCounter)
        {
            rotArrayX.RemoveAt(0);
        }

        for (int j = 0; j < rotArrayY.Count; j++)
        {
            rotAverageY += rotArrayY[j];
        }
        for (int i = 0; i < rotArrayX.Count; i++)
        {
            rotAverageX += rotArrayX[i];
        }

        rotAverageY /= rotArrayY.Count;
        rotAverageX /= rotArrayX.Count;

        // Clamp x-axis to 89 degrees
        rotAverageY = ClampAngle(rotAverageY, minimumY, maximumY);

        // Apply rotation to camera
        Quaternion yQuaternion = Quaternion.AngleAxis(rotAverageY, Vector3.left);
        Quaternion xQuaternion = Quaternion.AngleAxis(rotAverageX, Vector3.up);

        transform.localRotation = originalRotation * Quaternion.Lerp(transform.localRotation, xQuaternion, 1.0f) * Quaternion.Lerp(transform.localRotation, yQuaternion, 1.0f);
    }

    public static float ClampAngle(float angle, float min, float max)
    {
        angle = angle % 360;
        if ((angle >= -360F) && (angle <= 360F))
        {
            if (angle < -360F)
            {
                angle += 360F;
            }
            if (angle > 360F)
            {
                angle -= 360F;
            }
        }
        return Mathf.Clamp(angle, min, max);
    }
}

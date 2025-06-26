using UnityEngine;
using System.Collections;

public class PlayerMove : MonoBehaviour
{
    public float speed = 300f;
    public bool IsMoving { get; private set; } = false;

    public IEnumerator Roll(Vector3 direction)
    {
        IsMoving = true;

        float remainingAngle = 90f;
        Vector3 rotationCenter = transform.position + direction / 2 + Vector3.down / 2;
        Vector3 rotationAxis = Vector3.Cross(Vector3.up, direction);

        while (remainingAngle > 0)
        {
            float rotationAngle = Mathf.Min(Time.deltaTime * speed, remainingAngle);
            transform.RotateAround(rotationCenter, rotationAxis, rotationAngle);
            remainingAngle -= rotationAngle;
            yield return null;
        }

        transform.position = new Vector3(
            RoundToNearest(transform.position.x, 0.5f),
            RoundToNearest(transform.position.y, 0.5f),
            RoundToNearest(transform.position.z, 0.5f)
        );

        IsMoving = false;
    }

    private float RoundToNearest(float value, float factor)
    {
        return Mathf.Round(value / factor) * factor;
    }
}

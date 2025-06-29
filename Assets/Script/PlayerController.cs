using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public Transform cubeA;
    public Transform cubeB;
    public float speed = 300f;

    public bool isMoving = false;
    private Vector2 moveInput;

    void Update()
    {
        if (isMoving || moveInput == Vector2.zero)
            return;

        Vector3 direction = Vector3.zero;

        if (moveInput.x > 0)
            direction = Vector3.right;
        else if (moveInput.x < 0)
            direction = Vector3.left;
        else if (moveInput.y > 0)
            direction = Vector3.forward;
        else if (moveInput.y < 0)
            direction = Vector3.back;

        if (direction == Vector3.zero)
            return;

        bool isVertical = Mathf.Approximately(cubeA.position.x, cubeB.position.x);
        bool isStanding = isVertical && Mathf.Approximately(cubeA.position.z, cubeB.position.z);
        bool isVerticalShifted = isVertical && !Mathf.Approximately(cubeA.position.z, cubeB.position.z);

        if (isStanding)
        {
            if (Mathf.Approximately(cubeA.position.y, cubeB.position.y))
                return;

            StartCoroutine(RollUsingPivot(
                cubeA.position.y < cubeB.position.y ? cubeA : cubeB,
                cubeA.position.y < cubeB.position.y ? cubeB : cubeA,
                direction));
        }
        else if (isVerticalShifted)
        {
            if (direction == Vector3.forward)
            {
                StartCoroutine(RollUsingPivot(
                    cubeA.position.z > cubeB.position.z ? cubeA : cubeB,
                    cubeA.position.z > cubeB.position.z ? cubeB : cubeA,
                    direction));
            }
            else if (direction == Vector3.back)
            {
                StartCoroutine(RollUsingPivot(
                    cubeA.position.z < cubeB.position.z ? cubeA : cubeB,
                    cubeA.position.z < cubeB.position.z ? cubeB : cubeA,
                    direction));
            }
            else if (direction == Vector3.left)
            {
                if (cubeA.position.x < cubeB.position.x)
                    StartCoroutine(RollUsingPivot(cubeA, cubeB, direction));
                else
                    StartCoroutine(RollUsingPivot(cubeB, cubeA, direction));
            }
            else if (direction == Vector3.right)
            {
                if (cubeA.position.x > cubeB.position.x)
                    StartCoroutine(RollUsingPivot(cubeA, cubeB, direction));
                else
                    StartCoroutine(RollUsingPivot(cubeB, cubeA, direction));
            }
        }
        else
        {
            if (direction == Vector3.left)
            {
                if (cubeA.position.x < cubeB.position.x)
                    StartCoroutine(RollUsingPivot(cubeA, cubeB, direction));
                else
                    StartCoroutine(RollUsingPivot(cubeB, cubeA, direction));
            }
            else if (direction == Vector3.right)
            {
                if (cubeA.position.x > cubeB.position.x)
                    StartCoroutine(RollUsingPivot(cubeA, cubeB, direction));
                else
                    StartCoroutine(RollUsingPivot(cubeB, cubeA, direction));
            }
            else if (direction == Vector3.forward)
            {
                if (cubeA.position.z < cubeB.position.z)
                    StartCoroutine(RollUsingPivot(cubeA, cubeB, direction));
                else
                    StartCoroutine(RollUsingPivot(cubeB, cubeA, direction));
            }
            else if (direction == Vector3.back)
            {
                if (cubeA.position.z > cubeB.position.z)
                    StartCoroutine(RollUsingPivot(cubeA, cubeB, direction));
                else
                    StartCoroutine(RollUsingPivot(cubeB, cubeA, direction));
            }
        }

        moveInput = Vector2.zero;
    }

    IEnumerator RollUsingPivot(Transform pivotCube, Transform followerCube, Vector3 direction)
    {
        isMoving = true;

        Vector3 rotationAxis = Vector3.Cross(Vector3.up, direction);
        Vector3 rotationCenter = pivotCube.position + direction / 2f + Vector3.down / 2f;

        float remainingAngle = 90f;

        while (remainingAngle > 0)
        {
            float rotationAngle = Mathf.Min(Time.deltaTime * speed, remainingAngle);

            pivotCube.RotateAround(rotationCenter, rotationAxis, rotationAngle);
            followerCube.RotateAround(rotationCenter, rotationAxis, rotationAngle);

            remainingAngle -= rotationAngle;
            yield return null;
        }

        pivotCube.position = RoundVector(pivotCube.position, 0.5f);
        followerCube.position = RoundVector(followerCube.position, 0.5f);

        if (pivotCube.position.x > followerCube.position.x || pivotCube.position.z > followerCube.position.z)
        {
            Transform temp = cubeA;
            cubeA = cubeB;
            cubeB = temp;
        }

        isMoving = false;
    }

    private Vector3 RoundVector(Vector3 v, float factor)
    {
        return new Vector3(
            Mathf.Round(v.x / factor) * factor,
            Mathf.Round(v.y / factor) * factor,
            Mathf.Round(v.z / factor) * factor
        );
    }

    public void MoveUp() => TryMove(Vector3.forward);
    public void MoveDown() => TryMove(Vector3.back);
    public void MoveLeft() => TryMove(Vector3.left);
    public void MoveRight() => TryMove(Vector3.right);

    private void TryMove(Vector3 direction)
    {
        if (isMoving)
            return;

        moveInput = new Vector2(direction.x, direction.z);
    }
}

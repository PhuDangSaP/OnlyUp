using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Transform point1;
    public Transform point2;
    public float speed;
    public float delayTime = 2f; // Delay in seconds

    private bool isMovingToTarget1 = true;
    private bool isPlayerOnPlatform = false;
    private float delayTimer = 0f;

    private void Update()
    {
        if (isPlayerOnPlatform)
        {
            MovePlatform();
        }
        else
        {
            ReturnToStart();
        }
    }

    private void MovePlatform()
    {
        Transform targetPoint = isMovingToTarget1 ? point1 : point2;

        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, step);

        if (Vector3.Distance(transform.position, targetPoint.position) < 0.01f)
        {
            if (targetPoint == point2)
            {
                // If the platform reached point2, start the delay timer
                delayTimer += Time.deltaTime;

                if (delayTimer >= delayTime)
                {
                    // Reset the timer and switch to point1
                    delayTimer = 0f;
                    isMovingToTarget1 = true;
                }
            }
            else
            {
                // If the platform reached point1, reset the delay timer
                delayTimer = 0f;
                isMovingToTarget1 = !isMovingToTarget1;
            }
        }
    }

    private void ReturnToStart()
    {
        Transform targetPoint = point1;

        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, step);

        if (Vector3.Distance(transform.position, targetPoint.position) < 0.01f)
        {
            // If the platform reached point1, reset the delay timer
            delayTimer = 0f;
            isMovingToTarget1 = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            isPlayerOnPlatform = true;
            // Attach the player to the elevator so that it moves with the platform.
            other.transform.SetParent(transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            isPlayerOnPlatform = false;
            // Detach the player from the elevator.
            other.transform.SetParent(null);
        }
    }
}

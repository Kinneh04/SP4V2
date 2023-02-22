using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wave : MonoBehaviour
{
    public float yMin = -12f;
    public float yMax = -10f;
    public float movementTime = 5f;
    private float startValue;
    private float endValue;
    private bool movingUp = false;
    private float elapsedTime = 0f;

    void Update()
    {
        // Increment elapsed time since last frame
        elapsedTime += Time.deltaTime;

        // Calculate y position based on current elapsed time
        float y = Mathf.Lerp(startValue, endValue, elapsedTime / movementTime);

        // Check if we've reached the top of the movement
        if (y >= yMax)
        {
            // Set movingUp to false and reset elapsed time
            movingUp = false;
            elapsedTime = 0f;
            startValue = yMax;
            endValue = yMin;
        }

        // Check if we've reached the bottom of the movement
        if (y <= yMin)
        {
            // Set movingUp to true and reset elapsed time
            movingUp = true;
            elapsedTime = 0f;
            startValue = yMin;
            endValue = yMax;
        }

        // Set object's y position based on calculated value
        transform.position = new Vector3(transform.position.x, y, transform.position.z);
    }
}

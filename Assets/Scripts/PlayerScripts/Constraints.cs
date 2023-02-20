using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constraints : MonoBehaviour
{
    private Quaternion _initialRotation;

    private void Awake()
    {
        _initialRotation = transform.rotation;
    }
    private void LateUpdate()
    {
        transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
    }
    public void ResetRotation()
    {
        transform.rotation = _initialRotation;
    }
}

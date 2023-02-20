using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Workbench : MonoBehaviour
{
    public int Level;

    void Awake()
    {
    }


    public void SetLevel(int level)
    {
        Level = level;
    }
}

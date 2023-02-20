using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Workbench : MonoBehaviour
{
    public int Level;

    void Awake()
    {
        Level = 1;
    }


    public void SetLevel(int level)
    {
        Level = level;
    }
}

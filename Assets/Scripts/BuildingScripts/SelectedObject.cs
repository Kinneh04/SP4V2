using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectedObject : MonoBehaviour
{
    public int Stability = 100;
    public StructureTypes type;
    public Slider slider;
    public TMPro.TMP_Text stabilityLabel;
    public TMPro.TMP_Text costLabel;
    public Image actionImage;
}

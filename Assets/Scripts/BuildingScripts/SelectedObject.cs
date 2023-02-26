using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectedObject : MonoBehaviour
{
    public StructureTypes type;
    public Slider slider;
    public TMPro.TMP_Text stabilityLabel;
    public TMPro.TMP_Text costLabel;
    public CanvasGroup decayingHint;
    public Image actionImage;
}

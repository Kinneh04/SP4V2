using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatText : MonoBehaviour
{
    public bool fade = true;
    protected float FadeTime;
    public string CurrentText;
    // Start is called before the first frame update
    void Awake()
    {
        FadeTime = 0.0f;
        CurrentText = "";
    }

    public bool SetMessage (string message)
    {
        if (message != CurrentText)
        {
            CurrentText = message;
            GetComponent<TMPro.TMP_Text>().text = CurrentText;
            FadeTime = 10.0f;
            fade = false;
            return true;
        }
        return false;
    }

    public void ReplaceMessage(ChatText txt)
    {
        FadeTime = txt.FadeTime;
        fade = txt.fade;
        CurrentText = txt.CurrentText;
        GetComponent<TMPro.TMP_Text>().text = CurrentText;
    }

    private void Update()
    {
        if (FadeTime <= 0)
            fade = true;
        else
            FadeTime -= Time.deltaTime;
    }
}

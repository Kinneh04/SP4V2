using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CircularMenu : MonoBehaviour
{
    public List<MenuButton> buttons = new List<MenuButton>();
    private Vector2 centerCirclePos = new Vector2(0.5f, 0.5f);
    private Vector2 fromVector2M = new Vector2(0.5f, 1.0f); // Position from middle of screen horizontally
    private Vector2 MousePos;
    private Vector2 toVector2M; // Convert mousepos to screen coordinates

    private int menuItemsNo;
    public int CurrMenuItem;
    private int OldMenuItem;

    // Information Display
    public Image InfoIcon;
    public TMPro.TMP_Text InfoText;
    public TMPro.TMP_Text InfoDesc;
    public TMPro.TMP_Text InfoWood;

    private void Start()
    {
        menuItemsNo = buttons.Count;
        foreach (MenuButton button in buttons)
        {
            button.buttonImage.color = button.NormalColor;
        }
        CurrMenuItem = 0;
        OldMenuItem = 0;
    }

    private void Update()
    {
        GetCurrMenuItem();
    }

    public void GetCurrMenuItem()
    {
        MousePos = new Vector2(Input.mousePosition.x, Input.mousePosition.y); // Not using Input.mouosePosition directly to ensure that it is Vector2
        toVector2M = new Vector2(MousePos.x / Screen.width, MousePos.y / Screen.height);

        // Calculate angle in menu circle
        float angle = (Mathf.Atan2(fromVector2M.y - centerCirclePos.y, fromVector2M.x - centerCirclePos.x) - Mathf.Atan2(toVector2M.y - centerCirclePos.y, toVector2M.x - centerCirclePos.x)) * Mathf.Rad2Deg;

        if (angle < 0) // Angle on left side of screen becomes negative! So add 360 (Eg -90 + 360 = 270deg)
            angle += 360;

        // 360 / num of menu items = how many degrees each button takes up in circular menu
        // So angle / button gives the index of which button is currently selected
        CurrMenuItem = (int)(angle / (360 / menuItemsNo));
        if (CurrMenuItem >= menuItemsNo)
            CurrMenuItem = menuItemsNo - 1;

        if (CurrMenuItem != OldMenuItem) // Check if user has changed to a new item
        {
            buttons[OldMenuItem].buttonImage.color = buttons[OldMenuItem].NormalColor;
            buttons[OldMenuItem].icon.color = Color.white;
            OldMenuItem = CurrMenuItem;

            buttons[CurrMenuItem].buttonImage.color = buttons[CurrMenuItem].HighlightColor;
            buttons[CurrMenuItem].icon.color = Color.black;

            // Update information
            InfoIcon.sprite = buttons[CurrMenuItem].icon.sprite;
            InfoText.text = buttons[CurrMenuItem].name;
            InfoDesc.text = buttons[CurrMenuItem].description;
            if (buttons[CurrMenuItem].wood == 0)
            {
                InfoWood.text = "";
            }
            else if (buttons[CurrMenuItem].isStone)
            {
                InfoWood.text = buttons[CurrMenuItem].wood + "x Stone";
            }
            else
            {
                InfoWood.text = buttons[CurrMenuItem].wood + "x Wood";
            }
        }
    }
}

[System.Serializable]
public class MenuButton
{
    public string name;
    public string description;
    public int wood;
    public bool isStone = false;
    public Image buttonImage;
    public Image icon;
    public Color NormalColor = Color.white;
    public Color HighlightColor = Color.red;
    public Color PressedColor = Color.magenta;
}

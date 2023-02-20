using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;



public class CraftQuantity : MonoBehaviour
{
    CraftingManager CM;

    [SerializeField]
    protected TMP_Text AmountText;

    // Start is called before the first frame update
    void Awake()
    {
        CM = GameObject.FindGameObjectWithTag("Crafting").GetComponent<CraftingManager>();
    }

    public void ChangeAmount()
    {
        AmountText.text = CM.CraftAmount.ToString();
    }

    public void IncreaseCount()
    {
        CM.CraftAmount += 1;
        CM.UpdateAmountCost();
    }

    public void DecreaseCount()
    {
        CM.CraftAmount -= 1;
        if (CM.CraftAmount < 0)
            CM.CraftAmount = 0;
        CM.UpdateAmountCost();
    }

}

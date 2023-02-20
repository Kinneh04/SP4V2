using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealProperties : MonoBehaviour
{
    public string NameOfHeal;
    public int HealAmount = 1;
    PlayerProperties pp;
    InventoryManager PlayerInventory;
    public float timebeforeHeal = 1.0f;
    public bool Healsbleed;
    public bool HealsPoison;
    public int HealsRadiationAmount;
    public float infectionChance;
    public void useHealitem()
    {
        StartCoroutine(UseHeal());
    }
    IEnumerator UseHeal()
    {
        pp = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerProperties>();
        PlayerInventory = GameObject.FindGameObjectWithTag("Inventory").GetComponent<InventoryManager>();
        yield return new WaitForSeconds(timebeforeHeal);
        pp.HealHealth(HealAmount, Healsbleed, HealsPoison, infectionChance);
        pp.RadiationAmount -= HealsRadiationAmount;
        PlayerInventory.RemoveQuantityFromSlot(PlayerInventory.EquippedSlot, 1);
        Destroy(gameObject);
    }
}

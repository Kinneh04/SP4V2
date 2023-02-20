using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsumableProperty : MonoBehaviour
{
    public int GivesFoodAmount;
    public int GivesWaterAmount;
    public float PoisonChance;
    public float HealChance;
    public float FillingChance;

    PlayerProperties pp;
    private void Start()
    {
        pp = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerProperties>();
    }
    public void Eatfood()
    {
        pp.Hunger += GivesFoodAmount;
        pp.Thirst += GivesWaterAmount;

        if (pp.Hunger > 100) pp.Hunger = 100;
        if (pp.Thirst > 100) pp.Thirst = 100;

        float p = Random.Range(0, 100);
        if(p < PoisonChance)
        {
            pp.isPoisoned = true;
            pp.PoisonTimer = 20;
            pp.PoisonIcon.SetActive(true);
        }

        float h = Random.Range(0, 100);
        if(h < FillingChance)
        {
            pp.isFull = true;
            pp.FullIcon.SetActive(true);
            pp.FullTimer = 20;
        }

        h = Random.Range(0, 100);
        if(h < HealChance)
        {
            print("AAAAAAAAAAA");
            pp.isHealing = true;
            pp.HealIcon.SetActive(true);
            pp.Healtimer = 20f;
        }
    }
}

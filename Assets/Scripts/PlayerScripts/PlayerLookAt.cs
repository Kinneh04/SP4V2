using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerLookAt : MonoBehaviour
{
    public float maxDistance = 10.0f;
    public TMP_Text tmpTextUI;
    public PlayerProperties playerProperties;
    public bool showDot = true;
    public GameObject RHand;
    public LayerMask layers; // Exclude layers like floor and buildpreview
    public GameObject dot;

    public List<string> BannedWordsFromLookAt = new List<string>();
    void LateUpdate()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        //Vector3 forward = transform.TransformDirection(Vector3.forward) * 10;
        //Debug.DrawLine(transform.position, forward, Color.green);
        if (Physics.Raycast(ray, out hit, maxDistance, layers))
        {
            string name = hit.transform.gameObject.name;
            string tagname = hit.transform.gameObject.tag;
            name = name.Replace("(Clone)", "");
            name = name.Replace("(clone)", "");
            if (hit.transform.parent == RHand.transform)
            {
                tmpTextUI.text = "";
                print("Supposed to be null");
            }
            else if (hit.transform.gameObject.tag == "SleepingPoint")
            {
                if (!hit.transform.gameObject.GetComponent<SleepingBagProperties>().isUsed)
                    tmpTextUI.text = "claim sleeping bag [E]";
                else tmpTextUI.text = "Sleeping bag already claimed!";

                playerProperties.PlayerLookingAtItem = hit.transform.gameObject;
            }
            else if (hit.transform.gameObject.tag == "Crate")
            {

                tmpTextUI.text = "Loot crate [E]";

                playerProperties.PlayerLookingAtItem = hit.transform.gameObject;
            }
            else if (hit.transform.gameObject.tag == "Campfire")
            {

                tmpTextUI.text = "access campfire [E]";

                playerProperties.PlayerLookingAtItem = hit.transform.gameObject;
            }
            else if (hit.transform.gameObject.tag == "ToolCupboard")
            {

                tmpTextUI.text = "Open Tool Cupboard [E]";

                playerProperties.PlayerLookingAtItem = hit.transform.gameObject;
            }
            else
            {
                foreach (string word in BannedWordsFromLookAt)
                {
                    if (tagname == word)
                    {
                        tmpTextUI.text = " ";
                        return;
                    }
                }
                tmpTextUI.text = name;
                playerProperties.PlayerLookingAtItem = hit.transform.gameObject;
            }
            //if (hit.transform.gameObject.tag != "Floor" && hit.transform.gameObject.tag != "Unmarked" && hit.transform.gameObject.tag != "Unmarked")
            //{
            //    if (hit.transform.gameObject.GetComponent<WeaponInfo>() != null)
            //        tmpTextUI.text = hit.transform.gameObject.GetComponent<WeaponInfo>().sGetGunName();
            //    else
            //        tmpTextUI.text = hit.transform.name;
            //    playerProperties.PlayerLookingAtItem = hit.transform.gameObject;
            //}
            //else
            //{
            //    tmpTextUI.text = " ";
            //    playerProperties.PlayerLookingAtItem = null;

            //}
        }
        else
        {
            tmpTextUI.text = " ";
            playerProperties.PlayerLookingAtItem = null;
        }

        if (!showDot) tmpTextUI.text = "";
    }
}

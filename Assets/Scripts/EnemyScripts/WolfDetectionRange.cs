using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfDetectionRange : MonoBehaviour
{
    // Start is called before the first frame update

    List<GameObject> DetectedPlayers = new List<GameObject>();
    List<GameObject> DetectedPrey = new List<GameObject>();

    public void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name + ", " + other.gameObject.tag);
        if (other.gameObject.CompareTag("Player"))
        {
            DetectedPlayers.Add(other.gameObject);
            // Targeting
            WolfAI wolf = gameObject.GetComponentInParent<WolfAI>();
            if (wolf.TargetPlayer == null)
            {
                wolf.TargetPlayer = other.gameObject;
                wolf.change = true;
            }
            else if (Vector3.Distance(wolf.transform.position, wolf.TargetPlayer.transform.position) > Vector3.Distance(wolf.transform.position, other.gameObject.transform.position))
            {
                wolf.TargetPlayer = other.gameObject;
                wolf.change = true;
            }
        }
        else if (other.gameObject.CompareTag("Chicken"))
        {
            Debug.Log("Found Chicken");
            DetectedPrey.Add(other.gameObject);
            WolfAI wolf = gameObject.GetComponentInParent<WolfAI>();
            if (wolf.Prey == null)
            {
                wolf.Prey = other.gameObject;
                wolf.change = true;
            }
            else if (Vector3.Distance(wolf.transform.position, wolf.Prey.transform.position) > Vector3.Distance(wolf.transform.position, other.gameObject.transform.position))
            {
                wolf.Prey = other.gameObject;
                wolf.change = true;
            }
        }
        else if (other.gameObject.CompareTag("Deer"))
        {
            DetectedPrey.Add(other.gameObject);
            WolfAI wolf = gameObject.GetComponentInParent<WolfAI>();
            if (wolf.Prey == null)
            {
                wolf.Prey = other.gameObject;
                wolf.change = true;
            }
            else if (Vector3.Distance(wolf.transform.position, wolf.Prey.transform.position) > Vector3.Distance(wolf.transform.position, other.gameObject.transform.position))
            {
                wolf.Prey = other.gameObject;
                wolf.change = true;
            }
        }
    }

    public void OnTriggerExit(Collider other)
    {

        if (other.gameObject.CompareTag("Player"))
        {
            bool TargetLeft = false;
            WolfAI wolf = gameObject.GetComponentInParent<WolfAI>();
            if (wolf.TargetPlayer == other.gameObject)
            {
                TargetLeft = true;
                wolf.TargetPlayer = null;
                wolf.change = true;
                DetectedPlayers.Remove(other.gameObject);
            }
            if (TargetLeft)
            {
                for (int i = 0; i < DetectedPlayers.Count; i++)
                {
                    if (wolf.TargetPlayer == null)
                    {
                        wolf.TargetPlayer = DetectedPlayers[0];
                    }
                    else if (Vector3.Distance(wolf.transform.position, wolf.TargetPlayer.transform.position) > Vector3.Distance(wolf.transform.position, DetectedPlayers[i].transform.position))
                    {
                        wolf.TargetPlayer = DetectedPlayers[i];
                    }
                }
            }
        }
        else if (other.gameObject.CompareTag("Wolf"))
        {
            bool PreyLeft = false;
            WolfAI wolf = gameObject.GetComponentInParent<WolfAI>();
            if (wolf.Prey == other.gameObject)
            {
                PreyLeft = true;
                wolf.Prey = null;
                wolf.change = true;
                DetectedPrey.Remove(other.gameObject);
            }
            if (PreyLeft)
            {
                for (int i = 0; i < DetectedPrey.Count; i++)
                {
                    if (wolf.Prey == null)
                    {
                        wolf.Prey = DetectedPrey[0];
                    }
                    else if (Vector3.Distance(wolf.transform.position, wolf.Prey.transform.position) > Vector3.Distance(wolf.transform.position, DetectedPrey[i].transform.position))
                    {
                        wolf.Prey = DetectedPrey[i];
                    }
                }
            }
        }
    }
}

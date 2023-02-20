using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionRange : MonoBehaviour
{
    // Start is called before the first frame update

    List<GameObject> DetectedPlayers = new List<GameObject>(); 

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            DetectedPlayers.Add(other.gameObject);
            // Targeting
            Enemy enemy = gameObject.GetComponentInParent<Enemy>();
            if (enemy.TargetPlayer == null) 
            {
                enemy.TargetPlayer = other.gameObject;
            }
            else if (Vector3.Distance(enemy.transform.position, enemy.TargetPlayer.transform.position) > Vector3.Distance(enemy.transform.position, other.gameObject.transform.position))
            {
                enemy.TargetPlayer = other.gameObject;
            }
        }
        if (other.gameObject.CompareTag("Monument"))
        {
            Enemy enemy = gameObject.GetComponentInParent<Enemy>();
            enemy.Structure = other.gameObject;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        
        if (other.gameObject.CompareTag("Player"))
        {
            bool TargetLeft = false;
            Enemy enemy = gameObject.GetComponentInParent<Enemy>();
            if (enemy.TargetPlayer == other.gameObject)
            {
                TargetLeft = true;
                enemy.TargetPlayer = null;
                DetectedPlayers.Remove(other.gameObject);
            }
            if (TargetLeft)
            {
                for (int i = 0; i < DetectedPlayers.Count; i++)
                {
                    if (enemy.TargetPlayer == null)
                    {
                        enemy.TargetPlayer = DetectedPlayers[0];
                    }
                    else if (Vector3.Distance(enemy.transform.position, enemy.TargetPlayer.transform.position) > Vector3.Distance(enemy.transform.position, DetectedPlayers[i].transform.position))
                    {
                        enemy.TargetPlayer = DetectedPlayers[i];
                    }
                }
            }
        }
    }
}

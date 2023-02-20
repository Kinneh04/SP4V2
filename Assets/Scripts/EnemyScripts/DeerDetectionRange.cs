using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeerDetectionRange : MonoBehaviour
{
    // Start is called before the first frame update

    List<GameObject> DetectedPredator = new List<GameObject>();

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            DetectedPredator.Add(other.gameObject);
            // Targeting
            DeerAI deer = gameObject.GetComponentInParent<DeerAI>();
            if (deer.TargetPlayer == null)
            {
                deer.TargetPlayer = other.gameObject;
                deer.change = true;
            }
            else if (Vector3.Distance(deer.transform.position, deer.TargetPlayer.transform.position) > Vector3.Distance(deer.transform.position, other.gameObject.transform.position))
            {
                deer.TargetPlayer = other.gameObject;
                deer.change = true;
            }
        }
        else if (other.gameObject.CompareTag("Wolf"))
        {
            DetectedPredator.Add(other.gameObject);
            DeerAI deer = gameObject.GetComponentInParent<DeerAI>();
            if (deer.Predator == null)
            {
                deer.Predator = other.gameObject;
                deer.change = true;
            }
            else if (Vector3.Distance(deer.transform.position, deer.Predator.transform.position) > Vector3.Distance(deer.transform.position, other.gameObject.transform.position))
            {
                deer.Predator = other.gameObject;
                deer.change = true;
            }
        }
    }

    public void OnTriggerExit(Collider other)
    {

        if (other.gameObject.CompareTag("Player"))
        {
            bool TargetLeft = false;
            DeerAI deer = gameObject.GetComponentInParent<DeerAI>();
            if (deer.TargetPlayer == other.gameObject)
            {
                TargetLeft = true;
                deer.TargetPlayer = null;
                deer.change = true;
                DetectedPredator.Remove(other.gameObject);
            }
            if (TargetLeft)
            {
                for (int i = 0; i < DetectedPredator.Count; i++)
                {
                    if (deer.TargetPlayer == null)
                    {
                        deer.TargetPlayer = DetectedPredator[0];
                    }
                    else if (Vector3.Distance(deer.transform.position, deer.TargetPlayer.transform.position) > Vector3.Distance(deer.transform.position, DetectedPredator[i].transform.position))
                    {
                        deer.TargetPlayer = DetectedPredator[i];
                    }
                }
            }
        }
        else if (other.gameObject.CompareTag("Wolf"))
        {
            bool PredatorLeft = false;
            DeerAI deer = gameObject.GetComponentInParent<DeerAI>();
            if (deer.Predator == other.gameObject)
            {
                PredatorLeft = true;
                deer.Predator = null;
                deer.change = true;
                DetectedPredator.Remove(other.gameObject);
            }
            if (PredatorLeft)
            {
                for (int i = 0; i < DetectedPredator.Count; i++)
                {
                    if (deer.Predator == null)
                    {
                        deer.Predator = DetectedPredator[0];
                    }
                    else if (Vector3.Distance(deer.transform.position, deer.Predator.transform.position) > Vector3.Distance(deer.transform.position, DetectedPredator[i].transform.position))
                    {
                        deer.Predator = DetectedPredator[i];
                    }
                }
            }
        }
    }
}

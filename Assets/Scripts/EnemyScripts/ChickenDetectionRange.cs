using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChickenDetectionRange : MonoBehaviour
{
    // Start is called before the first frame update

    List<GameObject> DetectedPredator = new List<GameObject>();

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            DetectedPredator.Add(other.gameObject);
            // Targeting
            ChickenAI chicken = gameObject.GetComponentInParent<ChickenAI>();
            if (chicken.TargetPlayer == null)
            {
                chicken.TargetPlayer = other.gameObject;
                chicken.change = true;
            }
            else if (Vector3.Distance(chicken.transform.position, chicken.TargetPlayer.transform.position) > Vector3.Distance(chicken.transform.position, other.gameObject.transform.position))
            {
                chicken.TargetPlayer = other.gameObject;
                chicken.change = true;
            }
        }
        else if (other.gameObject.CompareTag("Wolf"))
        {
            DetectedPredator.Add(other.gameObject);
            ChickenAI chicken = gameObject.GetComponentInParent<ChickenAI>();
            if (chicken.Predator == null)
            {
                chicken.Predator = other.gameObject;
                chicken.change = true;
            }
            else if (Vector3.Distance(chicken.transform.position, chicken.Predator.transform.position) > Vector3.Distance(chicken.transform.position, other.gameObject.transform.position))
            {
                chicken.Predator = other.gameObject;
                chicken.change = true;
            }
        }
    }

    public void OnTriggerExit(Collider other)
    {

        if (other.gameObject.CompareTag("Player"))
        {
            bool TargetLeft = false;
            ChickenAI chicken = gameObject.GetComponentInParent<ChickenAI>();
            if (chicken.TargetPlayer == other.gameObject)
            {
                TargetLeft = true;
                chicken.TargetPlayer = null;
                chicken.change = true;
                DetectedPredator.Remove(other.gameObject);
            }
            if (TargetLeft)
            {
                for (int i = 0; i < DetectedPredator.Count; i++)
                {
                    if (chicken.TargetPlayer == null)
                    {
                        chicken.TargetPlayer = DetectedPredator[0];
                    }
                    else if (Vector3.Distance(chicken.transform.position, chicken.TargetPlayer.transform.position) > Vector3.Distance(chicken.transform.position, DetectedPredator[i].transform.position))
                    {
                        chicken.TargetPlayer = DetectedPredator[i];
                    }
                }
            }
        }
        else if (other.gameObject.CompareTag("Wolf"))
        {
            bool PredatorLeft = false;
            ChickenAI chicken = gameObject.GetComponentInParent<ChickenAI>();
            if (chicken.Predator == other.gameObject)
            {
                PredatorLeft = true;
                chicken.Predator = null;
                chicken.change = true;
                DetectedPredator.Remove(other.gameObject);
            }
            if (PredatorLeft)
            {
                for (int i = 0; i < DetectedPredator.Count; i++)
                {
                    if (chicken.Predator == null)
                    {
                        chicken.Predator = DetectedPredator[0];
                    }
                    else if (Vector3.Distance(chicken.transform.position, chicken.Predator.transform.position) > Vector3.Distance(chicken.transform.position, DetectedPredator[i].transform.position))
                    {
                        chicken.Predator = DetectedPredator[i];
                    }
                }
            }
        }
    }
}

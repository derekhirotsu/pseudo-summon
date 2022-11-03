using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleShotPatternBehaviour : MonoBehaviour, IShotPattern
{
    public void Fire(GameObject gameObject, Vector3 origin, Vector3 direction)
    {
        Instantiate(gameObject, origin, Quaternion.LookRotation(direction));
    }
}

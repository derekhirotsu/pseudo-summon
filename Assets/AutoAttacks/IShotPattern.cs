using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IShotPattern
{
    public void Fire(GameObject gameObject, Vector3 origin, Vector3 direction);
}

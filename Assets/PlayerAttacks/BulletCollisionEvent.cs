using System;
using UnityEngine;

public class BulletCollisionEvent : EventArgs
{
    private Collider _trigger;
    public Collider Trigger
    {
        get { return _trigger; }
    }

    public BulletCollisionEvent(Collider collider)
    {
        _trigger = collider;
    }
}

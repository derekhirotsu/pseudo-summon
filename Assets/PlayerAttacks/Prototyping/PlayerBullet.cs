using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _timeToLive;

    private float _timeAlive = 0f;

    public System.Action<PlayerBullet> TestAction;

    private void FixedUpdate()
    {
        if (_timeAlive >= _timeToLive)
        {
            Destroy(gameObject);
            return;
        }

        _timeAlive += Time.fixedDeltaTime;


        transform.position += (_moveSpeed * Time.fixedDeltaTime * transform.forward);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag != "Boss") return;
        Debug.Log("hit boss");

        TestAction?.Invoke(this);

        //Destroy(gameObject);

        //TestAction = null;
    }

    public void SetDirection(Vector3 direction)
    {
        transform.rotation = Quaternion.LookRotation(direction);
    }
}
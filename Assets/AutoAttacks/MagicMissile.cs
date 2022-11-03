using System.Collections;
using System.Collections.Generic;
using UnityEditor.AnimatedValues;
using UnityEngine;

public class MagicMissile : MonoBehaviour
{
    [SerializeField] private GameObject magicMissile;
    [SerializeField] private SoundFile missileBarrageFireSfx;
    [SerializeField] LayerMask _targetMask;
    [Range(0f, 360f)][SerializeField] private float _arcDegrees;

    private AudioProvider _audio;

    private float halfArcDegrees
    {
        get { return _arcDegrees / 2; }
    }

    private void Awake()
    {
        _audio = GetComponent<AudioProvider>();
    }

    public void Cast()
    {
        _audio.PlaySound(missileBarrageFireSfx);
        float offset = Random.Range(-halfArcDegrees, halfArcDegrees);
        Vector3 aimVector = Quaternion.AngleAxis(offset, transform.up) * transform.forward;
        SpawnBullet(magicMissile, transform.position, aimVector);
    }

    private void SpawnBullet(GameObject bulletPrefab, Vector3 origin, Vector3 direction)
    {
        TestBullet newBullet = Instantiate(bulletPrefab, origin, Quaternion.identity).GetComponent<TestBullet>();
        newBullet.SetTargetLayer(_targetMask);
        newBullet.SetDirection(direction);
    }

    private void OnDrawGizmosSelected()
    {
        if (_arcDegrees <= 0f)
        {
            Debug.DrawRay(transform.position, transform.forward * 10f, Color.green);
            return;
        }


        Vector3 leftAimVector = Quaternion.AngleAxis(-halfArcDegrees, transform.up) * transform.forward;
        Vector3 rightAimVector = Quaternion.AngleAxis(halfArcDegrees, transform.up) * transform.forward;

        Debug.DrawRay(transform.position, leftAimVector * 10f, Color.blue);
        Debug.DrawRay(transform.position, rightAimVector * 10f, Color.blue);
    }
}

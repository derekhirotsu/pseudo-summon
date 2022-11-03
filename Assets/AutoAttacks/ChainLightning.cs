using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainLightning : MonoBehaviour
{
    [SerializeField] LayerMask _targetMask;
    [SerializeField] private GameObject _chainLightning;
    [SerializeField] private SoundFile _lightningFireSfx;

    private AudioProvider _audio;

    private void Awake()
    {
        _audio = GetComponent<AudioProvider>();
    }

    public void Cast()
    {
        _audio.PlaySound(_lightningFireSfx);
        SpawnBullet(_chainLightning, transform.position, transform.forward);
    }

    private void SpawnBullet(GameObject bulletPrefab, Vector3 origin, Vector3 direction)
    {
        TestBullet newBullet = Instantiate(bulletPrefab, origin, Quaternion.identity).GetComponent<TestBullet>();
        newBullet.SetTargetLayer(_targetMask);
        newBullet.SetDirection(direction);
    }

    private void OnDrawGizmosSelected()
    {
        Debug.DrawRay(transform.position, transform.forward * 10f, Color.green);
    }
}

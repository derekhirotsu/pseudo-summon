using PseudoSummon;
using PseudoSummon.Audio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LariatBurst : MonoBehaviour
{
    [SerializeField] LayerMask _targetMask;
    [SerializeField] private GameObject _lariatBurst;
    [SerializeField] private SoundFile _lariatFireSfx;

    private AudioProvider _audio;

    private void Awake()
    {
        _audio = GetComponent<AudioProvider>();
    }

    public void Cast()
    {
        _audio.PlaySound(_lariatFireSfx);
        SpawnBullet(_lariatBurst, transform.position, transform.forward);
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

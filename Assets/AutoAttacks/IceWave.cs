using PseudoSummon;
using PseudoSummon.Audio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceWave : MonoBehaviour // ISpellPattern?
{
    [SerializeField] private GameObject _iceShard;
    [SerializeField] private LayerMask _targetMask;
    [Min(1)][SerializeField] private int _projectileCount;
    [Range(0f, 360f)][SerializeField] private float _arcDegrees;
    [SerializeField] private SoundFile _iceWaveFireSfx;

    private AudioProvider _audio;

    private float halfArcDegrees
    {
        get { return _arcDegrees / 2; }
    }

    private float intervals
    {
        get { return _projectileCount - 1; }
    }

    private float degreesPerInterval
    {
        get { return _arcDegrees / intervals; }
    }

    private bool projectSingleVector
    {
        get { return _projectileCount == 1 || _arcDegrees <= 0f; }
    }

    private void Awake()
    {
        _audio = GetComponent<AudioProvider>();
    }

    public void Cast()
    {
        _audio.PlaySound(_iceWaveFireSfx);

        if (projectSingleVector)
        {
            SpawnBullet(_iceShard, transform.position, transform.forward);
            return;
        }

        for (int i = 0; i < _projectileCount; i++)
        {
            float offset = (i * degreesPerInterval) - halfArcDegrees;
            Vector3 aimVector = Quaternion.AngleAxis(offset, transform.up) * transform.forward;
            SpawnBullet(_iceShard, transform.position, aimVector);
        }
    }

    private void SpawnBullet(GameObject bulletPrefab, Vector3 origin, Vector3 direction)
    {
        TestBullet newBullet = Instantiate(bulletPrefab, origin, Quaternion.identity).GetComponent<TestBullet>();
        newBullet.SetTargetLayer(_targetMask);
        newBullet.SetDirection(direction);
    }

    private void OnDrawGizmosSelected()
    {
        if (projectSingleVector)
        {
            Debug.DrawRay(transform.position, transform.forward * 10f, Color.green);
            return;
        }

        for (int i = 0; i < _projectileCount; i++)
        {
            float offset = (i * degreesPerInterval) - halfArcDegrees;
            Vector3 aimVector = Quaternion.AngleAxis(offset, transform.up) * transform.forward;
            Debug.DrawRay(transform.position, aimVector * 10f, Color.red);
        }
    }
}

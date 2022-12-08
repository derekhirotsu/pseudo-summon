using PseudoSummon.Audio;
using UnityEngine;

public class PlayerSpell : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private AudioProvider _audio;
    [SerializeField] private float _fireInterval;
    [SerializeField] private GameObject _bullet;
    [SerializeField] private SoundFile playerShootSfx;
    [SerializeField] private bool _semiAuto;

    private float _fireCooldown = 0f;
    public bool _canFire = true;
    private bool _isFiring = false;

    public bool ReadyToFire
    {
        get { return _canFire && _fireCooldown <= 0f && _isFiring; }
    }

    private void Update()
    {
        if (_fireCooldown > 0f)
        {
            _fireCooldown -= Time.deltaTime;
        }

        if (ReadyToFire)
        {
            Fire();
        }
    }

    public void OnAttackDown()
    {
        _isFiring = true;
    }

    public void OnAttackUp()
    {
        _isFiring = false;
    }


    private void Fire()
    {
        if (_semiAuto)
        {
            _isFiring = false;
        }

        GameObject newBullet = Instantiate(_bullet, transform.position, transform.rotation);
        _fireCooldown = _fireInterval;
        _audio.PlaySound(playerShootSfx);
        _animator.SetBool("FireSide", !_animator.GetBool("FireSide"));
        _animator.Play("Fire");
    }
}

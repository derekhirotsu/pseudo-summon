using PseudoSummon.Audio;
using UnityEngine;

public class PrimaryAttackSpell : BaseSpell
{
    [SerializeField] private Animator _animator;
    [SerializeField] private AudioProvider _audio;
    [SerializeField] private SoundFile playerShootSfx;
    [SerializeField] private GameObject _collisionParticle;

    [SerializeField] private float _fireInterval;
    private float _fireCooldown = 0f;

    public override void OnAttackDown()
    {
        _isFiring = true;
    }

    public override void OnAttackUp()
    {
        _isFiring = false;
    }

    protected override void Update()
    {
        if (_fireCooldown > 0f)
        {
            _fireCooldown -= Time.deltaTime;
        }

        if (!IsReadyToFire())
        {
            return;
        }

        if (SpawnInstance(transform.position, transform.rotation))
        {
            _fireCooldown = _fireInterval;
            _audio.PlaySound(playerShootSfx);
            _animator.SetBool("FireSide", !_animator.GetBool("FireSide"));
            _animator.Play("Fire");
        }
    }

    private bool IsReadyToFire()
    {
        return CanFire && _fireCooldown <= 0f && _isFiring;
    }

    protected override void OnBulletCollision(object sender, BulletCollisionEvent e)
    {
        base.OnBulletCollision(sender, e);
        PooledPlayerBullet bullet = sender as PooledPlayerBullet;

        GameObject destroyEffect = Instantiate(_collisionParticle, bullet.transform.position, bullet.transform.rotation);
        Destroy(destroyEffect, 0.8f);
    }
}

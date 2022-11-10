using System.Collections;
using System.Collections.Generic;
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
    private bool _canFire = true;
    private bool _isFiring = false;

    private Dictionary<int, GameObject> _list;

    private void Start()
    {
        _list = new Dictionary<int, GameObject>();
    }

    private void Update()
    {
        //Debug.DrawRay(transform.position, transform.forward * 10, Color.blue);
        if (_fireCooldown > 0f)
        {
            _fireCooldown -= Time.deltaTime;
        }

        if (_canFire && _fireCooldown <= 0f && _isFiring)
        {
            Fire();
        }
    }

    public void OnAttackDown()
    {
        if (_canFire && _fireCooldown <= 0f)
        {
            Fire();
        }
    }

    public void OnAttackUp()
    {
        _canFire = true;
        _isFiring = false;
    }


    private void Fire()
    {
        if (_semiAuto)
        {
            _canFire = false;
        }
        _isFiring = true;

        GameObject newBullet = Instantiate(_bullet, transform.position, transform.rotation);
        PlayerBullet b = newBullet.GetComponent<PlayerBullet>();
        _list.Add(newBullet.GetInstanceID(), newBullet);
        b.TestAction += OnTestAction;

        _fireCooldown = _fireInterval;
        _audio.PlaySound(playerShootSfx);
        _animator.SetBool("FireSide", !_animator.GetBool("FireSide"));
        _animator.Play("Fire");
    }

    private void OnTestAction(PlayerBullet bullet)
    {
        Debug.Log("responding to test action");

        bullet.TestAction -= OnTestAction;

        Destroy(bullet.gameObject);

        //foreach(KeyValuePair<int, GameObject> entry in _list)
        //{
        //    Debug.Log(entry.Key + " - " + entry.Value);
        //}
    }
}

using PseudoSummon;
using PseudoSummon.Audio;
using System.Collections;
using UnityEngine;

public class spell_Fireball : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float projectileSpeed;
    [SerializeField] private float spellDuration;
    [SerializeField] private SoundFile fireballSfx;
    private AudioProvider _audio;

    private readonly Vector3 Left = Vector3.left;
    private readonly Vector3 Right = Vector3.right;
    private readonly Vector3 Up = Vector3.forward;
    private readonly Vector3 Down = Vector3.back;

    private void Awake()
    {
        _audio = GetComponent<AudioProvider>();
    }

    public void Cast(int pattern)
    {
        StartCoroutine(CastCoroutine(pattern));
    }

    // TODO: This is a code smell that can probably be refactored.
    // Idea: spawn invisbile bullets that have the spawning behaviour on them.
    // Have the spell list spawn these bullets instead of just passing an ID.
    // ID spawns different spell "roots" that define the spawning behaviour of each volley
    // rather than picking a method from this switch case.
    //
    // basically a bullet with a bullet manager on it that spawns more bullets
    public IEnumerator CastCoroutine(int pattern)
    {
        switch (pattern)
        {
            case 0:
                TopLeftToRight();
                break;
            case 1:
                TopRightToLeft();
                break;
            case 2:
                BottomLeftToRight();
                break;
            case 3:
                BottomRightToLeft();
                break;
            case 4:
                LeftTopToBottom();
                break;
            case 5:
                RightTopToBottom();
                break;
            case 6:
                LeftBottomToTop();
                break;
            case 7:
                RightBottomToTop();
                break;
            case 8:
                StartCoroutine(LeftToRightTopToBottom());
                break;
            case 9:
                StartCoroutine(LeftToRightBottomToTop());
                break;
            case 10:
                StartCoroutine(RightToLeftTopToBottom());
                break;
            case 11:
                StartCoroutine(RightToLeftBottomToTop());
                break;
            case 12:
                StartCoroutine(TopToBottomLeftToRight());
                break;
            case 13:
                StartCoroutine(TopToBottomRightToLeft());
                break;
        }

        _audio.PlaySound(fireballSfx);

        yield return new WaitForSeconds(spellDuration);
        Destroy(gameObject);
    }

    private Vector3 CreateSpawnVector(float x, float z)
    {
        return new Vector3(x, 1f, z);
    }

    private void CreateProjectile(Vector3 origin, Vector3 direction)
    {
        GameObject go = Instantiate(projectilePrefab, origin, Quaternion.identity);

        go.transform.SetParent(gameObject.transform);

        FireballProjectile fireball = go.GetComponent<FireballProjectile>();

        fireball.SetDirection(direction);
        fireball.SetSpeed(projectileSpeed);
    }

    // pattern 0
    private void TopLeftToRight()
    {
        for (int i = 0; i < 5; i++)
        {
            Vector3 location = CreateSpawnVector(-15f, 6.5f - (i * 1.35f));
            CreateProjectile(location, Right);
        }
    }

    // pattern 1
    private void TopRightToLeft()
    {
        for (int i = 0; i < 5; i++)
        {
            Vector3 location = CreateSpawnVector(15f, 6.5f - (i * 1.35f));
            CreateProjectile(location, Left);
        }
    }

    // pattern 2
    private void BottomLeftToRight()
    {
        for (int i = 0; i < 5; i++)
        {
            Vector3 location = CreateSpawnVector(-15, -6.5f + (i * 1.35f));
            CreateProjectile(location, Right);
        }
    }

    // pattern 3
    private void BottomRightToLeft()
    {
        for (int i = 0; i < 5; i++)
        {
            Vector3 location = CreateSpawnVector(15, -6.5f + (i * 1.35f));
            CreateProjectile(location, Left);
        }
    }

    // pattern 4
    private void LeftTopToBottom()
    {
        for (int i = 0; i < 7; i++)
        {
            Vector3 location = CreateSpawnVector(-9f + (i * 1.38f), 15f);
            CreateProjectile(location, Down);
        }
    }

    // pattern 5
    private void RightTopToBottom()
    {
        for (int i = 0; i < 7; i++)
        {
            Vector3 location = CreateSpawnVector(9f - (i * 1.38f), 15f);
            CreateProjectile(location, Down);
        }
    }

    // pattern 6
    private void LeftBottomToTop()
    {
        for (int i = 0; i < 7; i++)
        {
            Vector3 location = CreateSpawnVector(-9f + (i * 1.38f), -15f);
            CreateProjectile(location, Up);
        }
    }

    // pattern 7
    private void RightBottomToTop()
    {
        for (int i = 0; i < 7; i++)
        {
            Vector3 location = CreateSpawnVector(9f - (i * 1.38f), -15f);
            CreateProjectile(location, Up);
        }
    }

    // pattern 8
    private IEnumerator LeftToRightTopToBottom()
    {
        WaitForSeconds interval = new WaitForSeconds(0.17f);

        for (int i = 0; i < 10; i++)
        {
            Vector3 location = CreateSpawnVector(-15f, 6.5f - (i * 1.4f));
            CreateProjectile(location, Right);
            yield return interval;
        }
    }

    // pattern 9
    private IEnumerator LeftToRightBottomToTop()
    {
        WaitForSeconds interval = new WaitForSeconds(0.17f);

        for (int i = 0; i < 10; i++)
        {
            Vector3 location = CreateSpawnVector(-15, -6.5f + (i * 1.4f));
            CreateProjectile(location, Right);
            yield return interval;
        }
    }

    // pattern 10
    private IEnumerator RightToLeftTopToBottom()
    {
        WaitForSeconds interval = new WaitForSeconds(0.17f);

        for (int i = 0; i < 10; i++)
        {
            Vector3 location = CreateSpawnVector(9, 6.5f - (i * 1.4f));
            CreateProjectile(location, Left);
            yield return interval;
        }
    }

    // pattern 11
    private IEnumerator RightToLeftBottomToTop()
    {
        WaitForSeconds interval = new WaitForSeconds(0.17f);

        for (int i = 0; i < 10; i++)
        {
            Vector3 location = CreateSpawnVector(15, -6.5f + (i * 1.4f));
            CreateProjectile(location, Left);
            yield return interval;
        }
    }

    // pattern 12
    private IEnumerator TopToBottomLeftToRight()
    {
        WaitForSeconds interval = new WaitForSeconds(0.15f);

        for (int i = 0; i < 14; i++)
        {
            Vector3 location = CreateSpawnVector(-9f + (i * 1.4f), 15f);
            CreateProjectile(location, Down);
            yield return interval;
        }
    }

    // pattern 13
    private IEnumerator TopToBottomRightToLeft()
    {
        WaitForSeconds interval = new WaitForSeconds(0.15f);

        for (int i = 0; i < 14; i++)
        {
            Vector3 location = CreateSpawnVector(9f - (i * 1.4f), 15f);
            CreateProjectile(location, Down);
            yield return interval;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spell_Fireball : MonoBehaviour
{
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] float projectileSpeed;
    [SerializeField] float spellDuration;
    [SerializeField] private SoundFile fireballSfx;
    private AudioSource audioSource;

    Transform platform;

    enum Side {Top, Bottom, Left, Right};

    void Awake() {
        audioSource = this.GetComponent<AudioSource>();
    }

    public void Cast(int pattern)
    {
        StartCoroutine(CastCoroutine(pattern));
    }

    public IEnumerator CastCoroutine(int pattern) {
        switch (pattern) {
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
        
        PlaySoundEffect(fireballSfx);

        yield return new WaitForSeconds(this.spellDuration);
        Destroy(this.gameObject);
    }

    public Vector3 CreateDirectionVector(int spawnSide) {
        switch (spawnSide) {
            case 0:
                return new Vector3(0, 0, -1);
            case 1:
                return new Vector3(0, 0, 1);
            case 2:
                return new Vector3(1, 0, 0);
            case 3:
                return new Vector3(-1, 0, 0);
            default:
                return new Vector3(0, 0, -1);
        }
    }

    Vector3 SetLocation(int spawnSide) {
        float x = 0;
        float z = 0;

        switch (spawnSide) {
            case 0:
                x = Random.Range(this.platform.transform.position.x - 9f, this.platform.transform.position.x + 9f);
                z = 15f;
                break;
            case 1:
                x = Random.Range(this.platform.transform.position.x - 9f, this.platform.transform.position.x + 9f);
                z = -15f;
                break;
            case 2:
                x = -15f;
                z = Random.Range(this.platform.transform.position.z - 6.5f, this.platform.transform.position.z + 6.5f);
                break;
            case 3:
                x = 15f;
                z = Random.Range(this.platform.transform.position.z - 6.5f, this.platform.transform.position.z + 6.5f);
                break;
        }

        return new Vector3(x, 1f, z);
    }

    Vector3 CreateSpawnVector(float x, float z) {
        return new Vector3(x, 1f, z);
    }

    void CreateProjectile(Vector3 location, int direction) {
        GameObject go = Instantiate(this.projectilePrefab, location, Quaternion.identity);

        go.transform.SetParent(this.gameObject.transform);

        FireballProjectile fireball = go.GetComponent<FireballProjectile>();

        fireball.SetDirection(CreateDirectionVector(direction));
        fireball.SetSpeed(this.projectileSpeed);
    }

    // pattern 0
    void TopLeftToRight() {
        for (int i = 0; i < 5; i++) {
            Vector3 location = CreateSpawnVector(-15f, 6.5f - (i * 1.35f));
            CreateProjectile(location, 2);
        }
    }

    // pattern 1
    void TopRightToLeft() {
        for (int i = 0; i < 5; i++) {
            Vector3 location = CreateSpawnVector(15f, 6.5f - (i * 1.35f));
            CreateProjectile(location, 3);
        }
    }

    // pattern 2
    void BottomLeftToRight() {
        for (int i = 0; i < 5; i++) {
            Vector3 location = CreateSpawnVector(-15, -6.5f + (i * 1.35f));
            CreateProjectile(location, 2);
        }
    }

    // pattern 3
    void BottomRightToLeft() {
        for (int i = 0; i < 5; i++) {
            Vector3 location = CreateSpawnVector(15, -6.5f + (i * 1.35f));
            CreateProjectile(location, 3);
        }
    }

    // pattern 4
    void LeftTopToBottom(){
        for (int i = 0; i < 7; i++) {
            Vector3 location = CreateSpawnVector(-9f + (i * 1.38f), 15f);
            CreateProjectile(location, 0);
        }
    }

    // pattern 5
    void RightTopToBottom(){
        for (int i = 0; i < 7; i++) {
            Vector3 location = CreateSpawnVector(9f - (i * 1.38f), 15f);
            CreateProjectile(location, 0);
        }
    }

    // pattern 6
    void LeftBottomToTop(){
        for (int i = 0; i < 7; i++) {
            Vector3 location = CreateSpawnVector(-9f + (i * 1.38f), -15f);
            CreateProjectile(location, 1);
        }
    }

    // pattern 7
    void RightBottomToTop(){
        for (int i = 0; i < 7; i++) {
            Vector3 location = CreateSpawnVector(9f - (i * 1.38f), -15f);
            CreateProjectile(location, 1);
        }
    }

    // pattern 8
    IEnumerator LeftToRightTopToBottom() {
        for (int i = 0; i < 10; i++) {
            Vector3 location = CreateSpawnVector(-15f, 6.5f - (i * 1.4f));
            CreateProjectile(location, 2);
            yield return new WaitForSeconds(0.17f);
        }
    }

    // pattern 9
    IEnumerator LeftToRightBottomToTop() {
        for (int i = 0; i < 10; i++) {
            Vector3 location = CreateSpawnVector(-15, -6.5f + (i * 1.4f));
            CreateProjectile(location, 2);
            yield return new WaitForSeconds(0.17f);
        }
    }

    // pattern 10
    IEnumerator RightToLeftTopToBottom() {
        for (int i = 0; i < 10; i++) {
            Vector3 location = CreateSpawnVector(9, 6.5f - (i * 1.4f));
            CreateProjectile(location, 3);
            yield return new WaitForSeconds(0.17f);
        }
    }

    // pattern 11
    IEnumerator RightToLeftBottomToTop() {
        for (int i = 0; i < 10; i++) {
            Vector3 location = CreateSpawnVector(15, -6.5f + (i * 1.4f));
            CreateProjectile(location, 3);
            yield return new WaitForSeconds(0.17f);
        }
    }

    // pattern 12
    IEnumerator TopToBottomLeftToRight() {
        for (int i = 0; i < 14; i++) {
            Vector3 location = CreateSpawnVector(-9f + (i * 1.4f), 15f);
            CreateProjectile(location, 0);
            yield return new WaitForSeconds(0.15f);
        }
    }

    // pattern 13
    IEnumerator TopToBottomRightToLeft() {
        for (int i = 0; i < 14; i++) {
            Vector3 location = CreateSpawnVector(9f - (i * 1.4f), 15f);
            CreateProjectile(location, 0);
            yield return new WaitForSeconds(0.15f);
        }
    }

    // Audio ------------------------------------------

    bool IsAudioValid(SoundFile soundFile) {
        Debug.Log(audioSource);
        if (audioSource == null || soundFile == null) {
            return false;
        }

        return true;
    }

    void SetAudioPitch(SoundFile soundFile) {
        audioSource.pitch = 1f;
        if (soundFile.randomizePitch) {
            audioSource.pitch = Random.Range(soundFile.minPitch, soundFile.maxPitch);
        }
    }

    void PlaySoundEffect(SoundFile soundFile) {
        if (!IsAudioValid(soundFile)) {
            return;
        }

        SetAudioPitch(soundFile);
        audioSource.PlayOneShot(soundFile.audioClip, soundFile.volumeScale);
    }
}

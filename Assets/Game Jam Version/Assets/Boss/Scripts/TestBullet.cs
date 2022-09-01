using UnityEngine;

public class TestBullet : MonoBehaviour
{
    [SerializeField] private float bulletSpeed;
    [SerializeField] private int bulletDamage = 1;
    [SerializeField] private float timeToLive;
    [SerializeField] private LayerMask wallCollisionLayer;
    [SerializeField] protected LayerMask targetLayer;
    [SerializeField] protected GameObject optionalDeathParticle;

    private Vector3 direction;
    protected bool directionSet = false;

    #region UnityFunctions

    private void Start()
    {
        Destroy(gameObject, timeToLive);
    }

    private void FixedUpdate()
    {
        transform.position += (direction.normalized * bulletSpeed * Time.fixedDeltaTime);

        if (directionSet)
        {
            transform.rotation = Quaternion.LookRotation(direction.normalized);
        }
    }

    private void OnDisable()
    {
        if (optionalDeathParticle != null)
        {
            GameObject particle = Instantiate(optionalDeathParticle, transform.position, transform.rotation);
            Destroy(particle, 0.8f);
        }
    }

    private void OnTriggerEnter(Collider entity) {
        if (wallCollisionLayer.Contains(entity.gameObject.layer)) {
            HandleWallCollision(entity);
        }

        if (targetLayer.Contains(entity.gameObject.layer)) {
            HandleTargetCollision(entity);
        }

    }

    #endregion

    public void SetDirection(Vector3 newDirection) {
        direction = newDirection;
        directionSet = true;
    }

    public void SetTargetLayer(LayerMask targets) {
        targetLayer = targets;
    }

    private void HandleWallCollision(Collider entity)
    {
        Destroy(gameObject);
    }

    private void HandleTargetCollision(Collider entity)
    {
        HealthTracker entityHealth = entity.gameObject.GetComponent<HealthTracker>();

        if (entityHealth != null)
        {
            entityHealth.ModifyHealth(-bulletDamage);
        }

        Destroy(gameObject);
    }
}

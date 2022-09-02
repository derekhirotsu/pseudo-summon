using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Transform _platform;
    [SerializeField] private float _platformExtentsX = 9f;
    [SerializeField] private float _platformExtentsZ = 6.5f;

    private float xMinBound;
    private float xMaxBound;

    private float zMinBound;
    private float zMaxBound;

    private void Start()
    {
        xMinBound = _platform.position.x - _platformExtentsX;
        xMaxBound = _platform.position.x + _platformExtentsX;
        zMinBound = _platform.position.z - _platformExtentsZ;
        zMaxBound = _platform.position.z + _platformExtentsZ;
    }

    public void Move(Vector3 moveDirection, float moveSpeed)
    {
        if (_platform == null)
        {
            Debug.LogWarning("No platform assigned!");
            return;
        }

        // 1. Move the player
        transform.position += moveDirection.normalized * moveSpeed;

        // Clamp X-axis movement values
        float clampedX = Mathf.Clamp(transform.position.x, xMinBound, xMaxBound);

        // Clamp Z-axis movement values
        float clampedZ = Mathf.Clamp(transform.position.z, zMinBound, zMaxBound);

        transform.position = new Vector3(clampedX, transform.position.y, clampedZ);
    }
}

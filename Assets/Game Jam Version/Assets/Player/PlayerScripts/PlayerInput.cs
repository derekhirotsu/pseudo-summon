using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] protected Vector3 rawInputVector;
    public Vector3 RawInputVector { get { return rawInputVector; } }

    [SerializeField] protected Vector3 inputVector;
    public Vector3 InputVector { get { return inputVector; } }

    [SerializeField] protected Vector3 aimVector;
    public Vector3 AimVector { get { return aimVector; } }

    // Update is called once per frame
    void Update()
    {
        // Get Raw Axis (binary input, 0 or 1)
        Vector3 rawVec = Vector3.zero;
        rawVec.x = Input.GetAxisRaw("Horizontal");
        rawVec.y = Input.GetAxisRaw("Vertical");
        rawInputVector = rawVec;

        // Get Axis (analog input, from 0 to 1)
        Vector3 axis = Vector3.zero;
        axis.x = Input.GetAxis("Horizontal");
        axis.y = Input.GetAxis("Vertical");
        inputVector = axis;
    }
}

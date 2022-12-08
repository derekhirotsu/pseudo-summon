using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class InputProvider : MonoBehaviour
{
    private const string RollInputName = "Roll";
    private const string SecondaryFireInputName = "Fire2";
    private const string PauseInputName = "Pause";

    [SerializeField] Camera _camera;

    public Vector3 AimVector { get; private set; }
    public Vector3 MovementVector { get; private set; }

    public event Action OnRoll;
    public event Action OnSecondaryFire;
    public event Action OnPause;
    public event Action OnPrimaryFireDown;
    public event Action OnPrimaryFireUp;

    private void Awake()
    {
        _camera = _camera ? _camera : Camera.main;
    }

    private void Update()
    {
        CheckRollInput();
        CheckSecondaryFireInput();
        CheckPauseInput();
        CheckPrimaryFireInput();
    }

    public Vector3 GetAimVector(Vector3 currentPosition)
    {
        Vector3 screenPosition = _camera.WorldToScreenPoint(currentPosition);
        Vector3 cursorVector = (Input.mousePosition - screenPosition).normalized;

        AimVector = new Vector3(cursorVector.x, 0, cursorVector.y);

        //Debug.DrawRay(currentPosition, AimVector * 5f, Color.red);

        return AimVector;
    }

    public Vector3 GetMovementVector()
    {
        Vector3 rawVec = Vector3.zero;
        rawVec.x = Input.GetAxisRaw("Horizontal");
        rawVec.z = Input.GetAxisRaw("Vertical");
        MovementVector = rawVec;

        return MovementVector;
    }

    private void CheckRollInput()
    {
        if (Input.GetButtonDown(RollInputName))
        {
            OnRoll?.Invoke();
        }
    }

    private void CheckSecondaryFireInput()
    {
        if (Input.GetButtonDown(SecondaryFireInputName))
        {
            OnSecondaryFire?.Invoke();
        }
    }

    private void CheckPauseInput()
    {
        if (Input.GetButtonDown(PauseInputName))
        {
            OnPause?.Invoke();
        }
    }

    private void CheckPrimaryFireInput()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            OnPrimaryFireDown?.Invoke();
        }

        else if (Input.GetButtonUp("Fire1"))
        {
            OnPrimaryFireUp?.Invoke();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEditor.AnimatedValues;
using UnityEngine;

public class TestPlayer : MonoBehaviour
{
    [SerializeField] private PlayerSpell spell;
    [SerializeField] private PooledPlayerSpell _pooledSpell;
    [SerializeField] private Camera _camera;

    Vector3 _aimVector;

    private void Update()
    {
        GetAimVector();
        transform.LookAt(transform.position + _aimVector);

        //if (Input.GetMouseButtonDown(0))
        //{
        //    spell.OnAttackDown();
        //}

        //if (Input.GetMouseButtonUp(0))
        //{
        //    spell.OnAttackUp();
        //}
    }

    private void GetAimVector()
    {
        Vector3 screenPosition = _camera.WorldToScreenPoint(transform.position);
        Vector3 cursorVector = (Input.mousePosition - screenPosition).normalized;

        _aimVector = new Vector3(cursorVector.x, 0, cursorVector.y);
    }
}

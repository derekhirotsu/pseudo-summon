using System.Collections;
using System.Collections.Generic;
using UnityEditor.AnimatedValues;
using UnityEngine;

public class TestPlayer : MonoBehaviour
{
    [SerializeField] private Camera _camera;

    [SerializeField] private List<BaseSpell> _spells;

    Vector3 _aimVector;
    BaseSpell _currentSpell;
    int _currentSpellIndex = 0;

    private void Start()
    {
        _currentSpell = _spells[_currentSpellIndex];
    }

    private void Update()
    {
        GetAimVector();
        transform.LookAt(transform.position + _aimVector);

        if (Input.GetMouseButtonDown(0))
        {
            _currentSpell.OnAttackDown();
        }

        if (Input.GetMouseButtonUp(0))
        {
            _currentSpell.OnAttackUp();
        }

        if (Input.GetButtonDown("Pause"))
        {
            _currentSpell.CanFire = !_currentSpell.CanFire;
        }

        if (Input.GetButtonDown("Roll"))
        {
            _currentSpellIndex++;

            if (_currentSpellIndex >= _spells.Count)
            {
                _currentSpellIndex = 0;
            }
            _currentSpell.OnAttackUp();
            _currentSpell = _spells[_currentSpellIndex];
        }
    }

    private void GetAimVector()
    {
        Vector3 screenPosition = _camera.WorldToScreenPoint(transform.position);
        Vector3 cursorVector = (Input.mousePosition - screenPosition).normalized;

        _aimVector = new Vector3(cursorVector.x, 0, cursorVector.y);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BossSpell/Sequence")]
public class SpellSequence : ScriptableObject
{
    [SerializeField] private List<int> spellId;
    public List<int> SpellID { get { return spellId; } }

    [SerializeField] private List<float> delays;
    public List<float> Delays { get { return delays; } }

}

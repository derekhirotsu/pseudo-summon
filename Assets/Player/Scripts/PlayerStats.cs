using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PseudoSummon
{
    [CreateAssetMenu(menuName = "PlayerStats")]
    public class PlayerStats : ScriptableObject
    {
        [Header("Movement")]
        [SerializeField, Min(0)] public float BaseMoveSpeed;
        [SerializeField, Min(0)] public float RollSpeed;
        [SerializeField, Min(0)] public float RollDuration;
        [SerializeField, Min(0)] public float RollInvincibilityDuration;

        [Header("Combat")]
        [SerializeField, Min(0)] public float PrimaryFireAttackInterval;
        [SerializeField, Min(0)] public int BusterHitsToCharge;
        [SerializeField, Min(0)] public float BusterCooldownTime;
        [SerializeField] public LayerMask AttackCollisionMask;

    }
}

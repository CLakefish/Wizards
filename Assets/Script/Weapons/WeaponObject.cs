using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Utilities/Weapon Objects/Weapon Object")]
public class WeaponObject :  ScriptableObject
{
    public GameObject weaponPrefab;
    public RuntimeAnimatorController weaponAnimationRig;
}

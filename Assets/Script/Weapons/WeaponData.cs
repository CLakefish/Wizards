using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WeaponData
{
    public abstract class WeaponBase : MonoBehaviour
    {
        public int currentAmmo;
        public int maxAmmo;

        public WeaponFireType fireType;

        public abstract void Shoot();
        public abstract void Reload();
    }

    public interface IWeapon
    {
        void OnEquip();
        void OnUnequip();
        void OnAttach();
        void OnReEquip();
    }

    public enum WeaponFireType
    {
        Single,
        Multi,
    }
}

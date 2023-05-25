using System.Collections;
using System.Collections.Generic;
using PlayerUtil; using WeaponData;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Position")]
    public Transform weaponPosition;

    [Header("Weapons")]
    public List<GameObject> visuals;
    public List<GameObject> instantiated;
    public List<WeaponObject> heldObjects;
    public List<int> heldObjectAmmoCounts;
    public int selectedIndex;

    [Header("Held Weapon")]
    public WeaponObject currentHeldWeaponObject;
    public WeaponBase currentHeldWeaponBase;
    public IWeapon currentWeaponInterface;
    public GameObject currentHeldGameObject;

    private void Start()
    {
        SwapWeapon(heldObjects[0], 0);
        heldObjectAmmoCounts[selectedIndex] = currentHeldWeaponBase.currentAmmo;
    }

    // Add a way to store the weapon information
    public void SwapWeapon(WeaponObject newWeapon, int index)
    {
        // 1
        if (heldObjects.Count <= 1) return;

        // This can be improved

        if (currentWeaponInterface != null) currentWeaponInterface.OnUnequip();

        currentHeldWeaponObject = newWeapon;

        if (!visuals.Contains(currentHeldWeaponObject.weaponPrefab))
        {
            currentHeldGameObject = Instantiate(currentHeldWeaponObject.weaponPrefab, weaponPosition, true);
            visuals.Add(currentHeldWeaponObject.weaponPrefab);
            instantiated.Add(currentHeldGameObject);
        }
        else
        {
            currentHeldGameObject = instantiated[visuals.IndexOf(currentHeldWeaponObject.weaponPrefab)].gameObject;
        }

        currentHeldWeaponBase = currentHeldGameObject.GetComponentInChildren<WeaponBase>();
        currentWeaponInterface = currentHeldGameObject.GetComponentInChildren<IWeapon>();

        currentHeldWeaponBase.currentAmmo = heldObjectAmmoCounts[index];

        if (currentWeaponInterface != null)
        {
            currentWeaponInterface.OnAttach();
            currentWeaponInterface.OnEquip();
        }
    }

    private void Update()
    {
        if (currentHeldWeaponBase.fireType == WeaponFireType.Single)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                currentHeldWeaponBase.Shoot();
            }
        }
        else
        {
            if (Input.GetMouseButton(0))
            {
                currentHeldWeaponBase.Shoot();
            }
        }

        heldObjectAmmoCounts[selectedIndex] = currentHeldWeaponBase.currentAmmo;

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (selectedIndex < heldObjects.Count - 1) selectedIndex++;
            else selectedIndex = 0;

            SwapWeapon(heldObjects[selectedIndex], selectedIndex);
        }
    }
}

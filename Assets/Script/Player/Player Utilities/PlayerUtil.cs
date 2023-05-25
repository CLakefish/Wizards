using System.Collections;
using System.Collections.Generic; using System; using WeaponData;
using UnityEngine;

namespace PlayerUtil
{
    public static class PlayerUtilities
    {
        // To be added

        public static Transform NearestObj(GameObject[] objs, Transform position)
        {
            Transform closestTarget = null;
            float closestDistance = Mathf.Infinity;
            Vector3 currentPos = position.transform.position;

            for (int i = 0; i < objs.Length; i++)
            {
                Vector3 dirToTarget = objs[i].transform.position - currentPos;
                float distance = dirToTarget.sqrMagnitude;

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestTarget = objs[i].transform;
                }
            }

            return closestTarget;
        }
    }

    [Serializable]
    public struct WeaponData
    {
        public WeaponObject weaponObject;
        public WeaponBase weaponBase;
    }
}

using System.Collections;
using System.Collections.Generic; using PlayerUtil; using WeaponData;
using UnityEngine;

public class Pistol : WeaponBase, IWeapon
{
    [Header("Hittable Layers")]
    public LayerMask hitLayer;

    [Header("Weapon Values")]
    public float fireRate;
    float previousFireTime;

    [Header("Trail Renderer")]
    public TrailRenderer vfxTrail;
    public RaycastData baseRayData;
    public MeshRenderer mesh;
    //public float speed;

    [Header("Variables")]
    //public bool canBounce;
    //public int bounceCount;
    [Space()]
    //public bool findNearestTarget;
    public bool continuousTargetting;
    public int targetMaxDistance;

    const float hitCorrection = 2000f;

    // Update is called once per frame
    void Update()
    {

    }

    public void OnAttach()
    {
        //throw new System.NotImplementedException();
    }

    public void OnEquip()
    {
        mesh.enabled = true;
        //throw new System.NotImplementedException();
    }

    public void OnUnequip()
    {
        mesh.enabled = false;
        //throw new System.NotImplementedException();
    }

    public override void Reload()
    {
        currentAmmo = maxAmmo;
        //throw new System.NotImplementedException();
    }

    public override void Shoot()
    {
        if (currentAmmo <= 1)
        {
            Reload();
            return;
        }

        if (Time.time >= fireRate + previousFireTime && currentAmmo != 0)
        {
            previousFireTime = Time.time;

            currentAmmo--;
            Debug.Log(currentAmmo);

            // Raycast to Mouse Input Position in world
            Ray ray = new Ray(transform.position, transform.rotation * Vector3.forward);

            // Trail rendering
            TrailRenderer trail = Instantiate(vfxTrail, transform.position, Camera.main.transform.rotation);

            // Raycast Hit reference
            RaycastHit raycast;


            RaycastData rayData = new();
            rayData.Values(baseRayData);
            //Debug.DrawRay(transform.position, ray.direction * 100, Color.blue, 2f);

            // Hitting target differences
            if (Physics.Raycast(ray, out raycast, 10000, hitLayer))
            {
                StartCoroutine(RenderTrail(trail, raycast, raycast.point, raycast.normal, rayData, true));
            }
            else
            {
                StartCoroutine(RenderTrail(trail, raycast, ray.direction * 4500, new Vector3(0f, 0f, 0f), rayData, false));
            }
        }

        //throw new System.NotImplementedException();
    }

    // Issues with function:
    // Requires a cleanup script for the trail renderers
    // Have to tell if it has hit a target or not
    // Bullets dont have data stored yet
    public IEnumerator RenderTrail(TrailRenderer trail, RaycastHit raycast, Vector3 point, Vector3 normal, RaycastData rayData, bool hit)
    {
        Vector3 startPos = trail.transform.position;
        Vector3 direction = (point - trail.transform.position).normalized;

        float distance = Vector3.Distance(trail.transform.position, point);
        float startDistance = distance;

        while (distance > 0)
        {
            if (trail == null) yield break;

            trail.transform.position = Vector3.Lerp(startPos, point, 1 - (distance / startDistance));
            distance -= Time.deltaTime * rayData.speed;

            yield return null;
        }

        trail.transform.position = point;

        if (hit)
        {
            // Spawn Hit Object via Object Pooling

            if (raycast.collider.tag == "Reflect") rayData.findNearestTarget = true;

            // Finding nearet
            if (rayData.findNearestTarget)
            {
                Vector3 projectileDirection = Vector3.Reflect(direction, normal);

                if (trail == null) yield break;

                if (GameObject.FindGameObjectWithTag("Enemy") != null)
                {
                    List<GameObject> objs = new();

                    foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Enemy"))
                    {
                        if (rayData.hitEnemies.Contains(obj) == false)
                        {

                            objs.Add(obj);
                        }
                    }

                    foreach (GameObject obj in objs)
                    {
                        Debug.DrawRay(obj.transform.position, Vector3.up, Color.green, 2f);
                    }

                    GameObject nearestObject = null;

                    if (objs.Count != 0)
                    {
                        nearestObject = PlayerUtilities.NearestObj(objs.ToArray(), trail.transform).gameObject;

                        if (Vector3.Distance(nearestObject.transform.position, trail.transform.position) <= targetMaxDistance)
                        {
                            projectileDirection = (nearestObject.transform.position - point).normalized;
                        }
                    }

                    if (Physics.Raycast(point, projectileDirection, out RaycastHit h, Mathf.Infinity, hitLayer))
                    {
                        yield return new WaitForEndOfFrame();

                        rayData.hitEnemies.Add(nearestObject);
                        rayData.findNearestTarget = rayData.continuousTargetting;

                        yield return StartCoroutine(RenderTrail(trail, h, (nearestObject != null) ? nearestObject.transform.position : h.point, h.normal, rayData, true));
                    }
                    else
                    {
                        yield return new WaitForEndOfFrame();

                        yield return StartCoroutine(RenderTrail(trail, h, projectileDirection * hitCorrection, Vector3.zero, rayData, false));
                    }
                }
            }

            // Bouncing bullets *REQURES WORK*

            if (rayData.canBounce)
            {
                // Data for bounce
                if (raycast.collider.tag != "Reflect") rayData.bounceCount--;

                // Since bouncing has a different way of functioning with finding the nearest target, you have to define it specifically. Which is dumb af but since it doesn't take much to process idc
                // Reflected direction
                Vector3 bounceDir = Vector3.Reflect(direction, normal);

                // Bounce count scenarios. Eventually since this is a switch it can have something specific for if its the first bounce, could be cool?
                switch (rayData.bounceCount)
                {
                    // Every other case
                    case (>= 1):

                        Vector3 bounceDirection = Vector3.Reflect(direction, normal);

                        // Raycast in the proper direction, if it hits there is a different case than if it does not. I.e. VFX and Damaging
                        if (Physics.Raycast(point, bounceDirection, out RaycastHit h, Mathf.Infinity, hitLayer))
                        {
                            yield return new WaitForEndOfFrame();

                            rayData.findNearestTarget = continuousTargetting ? true : false;

                            yield return StartCoroutine(RenderTrail(trail, h, h.point, h.normal, rayData, true));
                        }
                        else
                        {
                            yield return new WaitForEndOfFrame();

                            yield return StartCoroutine(RenderTrail(trail, h, bounceDirection * hitCorrection, Vector3.zero, rayData, false));
                        }

                        break;

                    // Death Case
                    case (<= 0):


                        while (trail != null)
                        {
                            if (trail == null) yield break;

                            trail.transform.position = point;
                            yield return null;
                        }

                        break;
                }
            }
        }
    }

    public void OnReEquip()
    {
        mesh.enabled = true;
        //throw new System.NotImplementedException();
    }
}

[System.Serializable]
public struct RaycastData
{
    public int bounceCount;
    public int damage;

    public float speed;

    internal bool findNearestTarget;
    public bool continuousTargetting;
    public bool canBounce;

    public List<GameObject> hitEnemies,
                            hitReflectors;

    public void Values(RaycastData te)
    {
        bounceCount = te.bounceCount;
        speed = te.speed;
        findNearestTarget = te.findNearestTarget;
        canBounce = te.canBounce;
        continuousTargetting = te.continuousTargetting;

        hitEnemies = new List<GameObject>();
        hitReflectors = new List<GameObject>();
    }
}
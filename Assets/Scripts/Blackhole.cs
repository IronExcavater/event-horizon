using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class Blackhole : MonoBehaviour
{
    List<Rigidbody> debrisRb = new List<Rigidbody>();
    SphereCollider coll;
    DebrisSpawner spawner;

    [SerializeField] bool useGravity = true;
    public float gravity = 1f;
    public float maxBlackholeSize;

    float previousRadius;
    float targetRadius;

    [SerializeField] float lerpSpeed = 10f;
    float currentLerpTime;
    float totalLerpTime;

    private void Awake()
    {
        coll = GetComponent<SphereCollider>();
        spawner = GetComponent<DebrisSpawner>();

        targetRadius = coll.radius;
        previousRadius = targetRadius;
    }

    public void Add(GameObject _obj)
    {
        debrisRb.Add(_obj.GetComponent<Rigidbody>());

    }

    private void FixedUpdate()
    {
        if (useGravity)
        {
            if(debrisRb != null && debrisRb.Count > 0)
            {
                foreach (var _rb in debrisRb)
                {
                    float dist = Vector3.Distance(transform.position, _rb.gameObject.transform.position);
                    Vector3 dir = (transform.position - _rb.gameObject.transform.position).normalized;
                    _rb.AddForce(dir * (1 / (dist / gravity)));
                }
            }
        }
    }
    private void Update()
    {
        if(targetRadius != coll.radius)
        {
            currentLerpTime += lerpSpeed * Time.deltaTime;
            coll.radius = Mathf.Lerp(previousRadius, targetRadius, currentLerpTime / totalLerpTime);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Debris"))
        {
            //increase blackhole size
            previousRadius = coll.radius;
            targetRadius += other.gameObject.GetComponent<Consumable>().size / 8;
            gravity = Mathf.Pow(coll.radius, 1.5f);

            //calculate lerp time
            currentLerpTime = 0;
            totalLerpTime = Mathf.Abs(targetRadius - coll.radius) / lerpSpeed;

            //extend spawn area
            spawner.ExtendSpawner(coll.radius);

            //reduce blackhole size when max size is reached

            //destroy debris
            debrisRb.Remove(other.GetComponent<Rigidbody>());
            Destroy(other.gameObject, 0.2f);
            return;
        }
    }
}

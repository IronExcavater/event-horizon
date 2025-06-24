using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(BlackHoleScaleManager))]
public class Blackhole : MonoBehaviour
{
    List<Rigidbody2D> debrisRb = new List<Rigidbody2D>();
    CircleCollider2D coll;
    DebrisSpawner spawner;
    BlackHoleScaleManager scale;

    [SerializeField] bool useGravity = true;
    public float gravity = 1f;
    public float maxBlackholeSize;

    float targetRadius;
    int resetCount = 0;

    int debrisCount = 0;

    private void Awake()
    {
        coll = GetComponent<CircleCollider2D>();
        spawner = GetComponent<DebrisSpawner>();
        scale = GetComponent<BlackHoleScaleManager>();

        targetRadius = transform.localScale.x;
    }

    public void Add(GameObject _obj)
    {
        debrisRb.Add(_obj.GetComponent<Rigidbody2D>());
    }
    //public void AddForce()
    //{
        
    //}

    public void RemoveDebris(Rigidbody2D _debrisRb)
    {
        debrisRb.Remove(_debrisRb);
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
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Debris"))
        {
            //increase blackhole size
            Consumable debrisProperties = other.gameObject.GetComponent<Consumable>();
            targetRadius += debrisProperties.size / (8 * targetRadius);
            gravity = Mathf.Pow(targetRadius + resetCount * maxBlackholeSize, 1.2f);

            //extend spawn area
            spawner.ExtendSpawner(targetRadius);

            //increase visual size
            scale.currentScale = targetRadius;

            //counter
            debrisCount += Mathf.RoundToInt(debrisProperties.size);

            //reduce blackhole size when max size is reached
            if(targetRadius >= maxBlackholeSize)
            {
                spawner.SetDefaultDist(maxBlackholeSize);
                resetCount++;
                targetRadius = 1f;

                Debug.Log("Debris Count: " + debrisCount + ", Reset(s): " + resetCount);

                Invoke("SizeReset", 0.1f);
            }

            //destroy debris
            debrisProperties.Damage();
        }
    }

    private void SizeReset()
    {
        //reset size
        scale.currentScale = targetRadius;
    }
}

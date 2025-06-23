using System.Collections.Generic;
using UnityEngine;

public class Blackhole : MonoBehaviour
{
    List<Rigidbody> debrisRb = new List<Rigidbody>();

    [SerializeField] bool useGravity = true;
    public float gravity = 1f;

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
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Debris"))
        {
            debrisRb.Remove(other.GetComponent<Rigidbody>());
            Destroy(other.gameObject);
            return;
        }
    }
}

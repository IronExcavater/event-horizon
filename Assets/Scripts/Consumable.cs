using System.Collections;
using UnityEngine;

public class Consumable : MonoBehaviour
{
    public float size;

    CircleCollider2D collider2D;
    Rigidbody2D rigidbody;
    ParticleSystem particle;

    int points; //points receive

    public float triggerTime = 1f;

    private void Awake()
    {
        collider2D = GetComponent<CircleCollider2D>();
        rigidbody = GetComponent<Rigidbody2D>();
    }
    public virtual void Damage() //for blackhole consumption
    {
        //remove from blackhole
        DebrisSpawner.instance.blackhole.RemoveDebris(rigidbody);

        //spawn particles
        if (particle != null)
        {
            particle.Play();

            collider2D.enabled = false;

            Destroy(gameObject, particle.main.duration);
        }
        else
        {
            collider2D.enabled = false;

            Destroy(gameObject, 0.5f);
        }
    }
    public virtual void DamageWithPoints() //for player consumption
    {
        //add to score
        DebrisSpawner.instance.score.UpdateScore(points);

        //spawn points ui/text?

        //remove from blackhole
        DebrisSpawner.instance.blackhole.RemoveDebris(rigidbody);

        //spawn particles
        if (particle != null)
        {
            particle.Play();

            collider2D.enabled = false;

            Destroy(gameObject, particle.main.duration);
        }
        else
        {
            collider2D.enabled = false;

            Destroy(gameObject, 0.5f);
        }
    }
    public void SetPoints(int _points)
    {
        points = _points;
    }
    public virtual void TriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Debris"))
        {
            //Damage collision
            collision.GetComponent<Consumable>()?.DamageWithPoints();
            DamageWithPoints();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        TriggerEnter2D(collision);
    }

    public IEnumerator JustLaunched()
    {
        collider2D.isTrigger = true;
        yield return new WaitForSeconds(triggerTime);
        collider2D.isTrigger = false;
    }
}

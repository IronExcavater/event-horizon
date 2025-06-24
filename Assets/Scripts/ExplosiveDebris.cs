using UnityEngine;
using System.Collections;

public class ExplosiveDebris : Consumable
{
    [SerializeField] float explodeRadius = 5f;

    [SerializeField] LayerMask damageableLayer;

    public override void DamageWithPoints()
    {
        StartCoroutine(Explode());

        base.DamageWithPoints();
    }
    public override void Damage()
    {

        StartCoroutine(PointlessExplode());

        base.Damage();
    }

    IEnumerator Explode()
    {
        //explode
        Collider2D[] colls = Physics2D.OverlapCircleAll(transform.position, explodeRadius, damageableLayer);

        //Debug Visuals
        Debug.DrawLine(transform.position, transform.position + Vector3.up * explodeRadius, Color.red, 0.2f);
        Debug.DrawLine(transform.position, transform.position + Vector3.down * explodeRadius, Color.red, 0.2f);
        Debug.DrawLine(transform.position, transform.position + Vector3.left * explodeRadius, Color.red, 0.2f);
        Debug.DrawLine(transform.position, transform.position + Vector3.right * explodeRadius, Color.red, 0.2f);

        yield return new WaitForEndOfFrame();

        if (colls.Length > 0)
        {
            foreach (var coll in colls)
            {
                coll.GetComponent<Consumable>().DamageWithPoints();
            }
        }
    }
    IEnumerator PointlessExplode()
    {
        //explode
        Collider2D[] colls = Physics2D.OverlapCircleAll(transform.position, explodeRadius, damageableLayer);

        //Debug Visuals
        Debug.DrawLine(transform.position, transform.position + Vector3.up * explodeRadius, Color.red, 0.2f);
        Debug.DrawLine(transform.position, transform.position + Vector3.down * explodeRadius, Color.red, 0.2f);
        Debug.DrawLine(transform.position, transform.position + Vector3.left * explodeRadius, Color.red, 0.2f);
        Debug.DrawLine(transform.position, transform.position + Vector3.right * explodeRadius, Color.red, 0.2f);

        yield return new WaitForEndOfFrame();

        if (colls.Length > 0)
        {
            foreach (var coll in colls)
            {
                coll.GetComponent<Consumable>().Damage();
            }
        }
    }
}

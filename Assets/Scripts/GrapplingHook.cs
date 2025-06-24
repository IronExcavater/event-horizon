using System.Collections;
using UnityEngine;

public class GrapplingHook : MonoBehaviour
{
    //Joint2D joint2D;
    LineRenderer line;
    Vector2 startPos;
    [SerializeField]
    Vector2 targetPos;
    Transform parent;

    PlayerController pc;

    Coroutine shootRoutine;

    [SerializeField]
    float shootTime,
        retractTime,
        ropeLen;

    bool isGrappling;
    bool grappled;

    Collider2D coll;

    private void Awake()
    {
        //joint2D = GetComponent<Joint2D>();
        line = GetComponentInParent<LineRenderer>(true);
        coll = GetComponent<Collider2D>();
    }

    private void Start()
    {
        startPos = transform.localPosition;
        parent = transform.parent;
        pc = parent.GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (Input.GetMouseButton(1))
        {
            line.enabled = true;
        }
        else
        {
            line.enabled = false;
        }

        if (Input.GetMouseButtonUp(1))
        {
            if (shootRoutine != null)
                return;

            if (grappled)
            {
                StartCoroutine(LaunchItem());
                return;
            }

            shootRoutine = StartCoroutine(Shoot(startPos, targetPos, shootTime));
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Rigidbody2D collRb = collision.GetComponent<Rigidbody2D>();
            collision.transform.parent = transform;
            collRb.linearVelocity = Vector2.zero;
            collRb.angularVelocity = 0;
            collRb.totalForce = Vector2.zero;
            collRb.totalTorque = 0;
            collRb.simulated = false;

            grappled = true;
        }
    }

    void ParentSelf(bool setParent)
    {
        transform.parent = setParent ? parent : null;
    }

    IEnumerator Shoot(Vector2 _startPos, Vector2 _targetPos, float duration)
    {
        float t = 0;
        while (t <= shootTime)
        {
            transform.localPosition = Vector2.Lerp(_startPos, _targetPos, t / shootTime);
            t += Time.deltaTime;
            yield return null;
        }

        if (_targetPos.x > 1f)
            shootRoutine = StartCoroutine(Shoot(_targetPos, _startPos, retractTime));
        else
            shootRoutine = null;
    }

    IEnumerator LaunchItem()
    {
        pc.ImpactCoroutine(transform.right * -1, 2);
        coll.enabled = false;

        int childCount = transform.childCount;
        Rigidbody2D[] rbs = transform.GetComponentsInChildren<Rigidbody2D>();
        for (int i = 1; i < rbs.Length; i++)
        {
            rbs[i].simulated = true;
            rbs[i].AddForce(transform.right * 25, ForceMode2D.Impulse);
            rbs[i].AddTorque(Random.Range(7f, 12f) * (Mathf.RoundToInt(Random.value)*2-1));
            rbs[i].transform.parent = null;
        }

        transform.localPosition = startPos;
        grappled = false;

        yield return new WaitForSeconds(0.2f);
        coll.enabled = true;

        yield return null;
    }
}

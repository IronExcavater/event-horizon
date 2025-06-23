using System.Collections;
using UnityEngine;

public class GrapplingHook : MonoBehaviour
{
    Joint2D joint2D;
    Vector2 startPos;
    [SerializeField]
    Vector2 targetPos;
    Transform parent;

    Coroutine shootRoutine;

    [SerializeField]
    float shootTime,
        retractTime,
        ropeLen;

    bool isGrappling;
    bool grappled;

    private void Awake()
    {
        joint2D = GetComponent<Joint2D>();
    }

    private void Start()
    {
        startPos = transform.localPosition;
        parent = transform.parent;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (shootRoutine != null || grappled)
                return;

            shootRoutine = StartCoroutine(Shoot(startPos, targetPos, shootTime));
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.transform.parent = transform;
            ParentSelf(false);
            if (shootRoutine != null)
                StopCoroutine(shootRoutine);

            EnableJoint(true);
            grappled = true;
        }
    }

    void EnableJoint(bool enable)
    {
        joint2D.enabled = enable;
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

   
}

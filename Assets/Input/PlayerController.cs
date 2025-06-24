using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    float rotateSpeed = 5f,
          moveSpeed = 10f,
          maxFuel = 100f,
          fuelDrainRate = 10f;

    [SerializeField] private RectTransform fuelGauge;
    [SerializeField] private Image fuelFillImage;
    [SerializeField] Color fillCol;
    [SerializeField] private AudioSource engineSource;

    public FuelManager fuelManager;

    Camera mainCam;
    Rigidbody2D rb;

    bool isBoosting = false;
    float currentFuel;
    Vector3 impactOffset = Vector3.zero;
    bool fuelBuffed = false;

    public float cumulativeMass = 0;

    private void Awake()
    {
        mainCam = Camera.main;
        rb = GetComponent<Rigidbody2D>();
        currentFuel = maxFuel;
        fuelManager.maxFuel = maxFuel;
    }

    private void Update()
    {
        if (Input.GetMouseButton(0) && currentFuel > 0f)
        {
            isBoosting = true;
        }
        else
            isBoosting = false;

        if (fuelGauge != null)
        {
            Vector2 mouseScreenPos = Input.mousePosition;
            fuelGauge.position = mouseScreenPos;
        }

        fuelManager.currentFuel = currentFuel;
        engineSource.volume = moveSpeed * SpeedMultiplier();

    }

    private void FixedUpdate()
    {
        RotateSelf();

        mainCam.transform.position = Vector3.Lerp(mainCam.transform.position, new Vector3(transform.position.x, transform.position.y, -10), Time.fixedDeltaTime * 7);

        if (currentFuel > 0f)
        {
            MoveTowardsTarget();
            float drain = SpeedMultiplier() * fuelDrainRate * Time.fixedDeltaTime;
            currentFuel = Mathf.Max(currentFuel - drain, fuelBuffed?maxFuel:0);
            /*if (currentFuel <= 0f)
                isBoosting = false;*/
        }
        /*else
        {
            float drain = fuelDrainRate * Time.deltaTime;
            currentFuel = Mathf.Max(currentFuel - drain, 0f);

            if (currentFuel <= 0f)
                isBoosting = false;
        }*/
    }
    Coroutine fuelBuffRoutine;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Debris"))
        {
            AudioManager.PlaySfxOneShot(AudioManager.Audio.explosion);
            currentFuel -= 20f;
        }
        else if (collision.CompareTag("Powerup"))
        {
            AudioManager.PlaySfxOneShot(AudioManager.Audio.powerUp);
            /*currentFuel = maxFuel;*/
            if (fuelBuffRoutine != null)
                StopCoroutine(fuelBuffRoutine);
            fuelBuffRoutine = StartCoroutine(FuelBuff(5f));
            DebrisSpawner.instance.blackhole.RemoveDebris(rb);
        }
    }

    public IEnumerator FuelBuff(float duration)
    {
        float t = 0f;
        currentFuel = maxFuel;
        fuelBuffed = true;
        fuelFillImage.color = Color.green;
        while (t < duration)
        {
            t += Time.deltaTime;
            yield return null;
        }

        fuelFillImage.color = fillCol;
        fuelBuffed = false;
    }

    void RotateSelf()
    {
        Vector2 targetDir = TargetDir();
        float angle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg;

        Quaternion rot = Quaternion.Euler(new Vector3(0, 0, angle));
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * rotateSpeed + SpeedMultiplier());
    }

    Vector3 BlackholePull()
    {
        float dist = Vector3.Distance(DebrisSpawner.instance.blackhole.transform.position, transform.position);
        Vector3 dir = (DebrisSpawner.instance.blackhole.transform.position - transform.position).normalized;

        return (dir * (1 / (dist / DebrisSpawner.instance.blackhole.gravity)));

        //float dist = Vector3.Distance(transform.position, _rb.gameObject.transform.position);
        //Vector3 dir = (transform.position - _rb.gameObject.transform.position).normalized;
        //_rb.AddForce(dir * (1 / (dist / gravity)));
    }

    /// <summary>
    /// Moves rocket in the direction it's facing. Returns the forward velocity.
    /// </summary>
    Vector3 MoveTowardsTarget()
    {
        Vector3 vector = transform.right * Time.fixedDeltaTime * moveSpeed * SpeedMultiplier();
        if(DebrisSpawner.instance.maxDist >= Vector3.Distance(DebrisSpawner.instance.blackhole.transform.position, transform.position))
        {
            transform.position += vector + impactOffset + (BlackholePull() * 0.7f);
        }
        else
        {
            transform.position += vector + impactOffset + (BlackholePull() * 100f);
        }

        return vector;
    }

    /* public IEnumerator ImpactCoroutine(Vector3 hitDir, float impactMagnitude)
     {
         impactTime = 0;
         float increment = 0.3f;
         while (impactMagnitude > 0.1f)
         {
             Debug.Log("impacting");
             ImpactVector(hitDir, impactMagnitude);
             impactMagnitude = Mathf.Lerp(impactMagnitude, 0, Time.deltaTime * increment);
             yield return new WaitForFixedUpdate();
             impactTime = Mathf.Clamp01(impactTime + increment);
         }
         impactTime = 1;
     }*/

    public IEnumerator ImpactCoroutine(Vector3 hitDir, float impactMagnitude)
    {
        float decayRate = 5f;
        Vector3 impactVelocity = hitDir.normalized * impactMagnitude;
        while (impactVelocity.magnitude > 0.1f)
        {
            Debug.Log("test");
            impactOffset = impactVelocity;
            impactVelocity = Vector3.Lerp(impactVelocity, Vector3.zero, decayRate * Time.deltaTime);
            yield return null;
        }

        impactOffset = Vector3.zero;
    }

    /// <summary>
    /// A multiplier based on cursor distance from rocket. Applies an x2 boost if isBoosting
    /// </summary>
    /// <returns></returns>
    float SpeedMultiplier()
    {
        float mouseDist = Vector2.Distance(MouseTargetPos(), transform.position);
        if (mouseDist < 0.3f)
            mouseDist = 0;

        Debug.Log(MassMultiplier());
        return -MassMultiplier() + (!isBoosting ? Mathf.Clamp(mouseDist / 2, 0, 1):
                                                    Mathf.Clamp(mouseDist, 0.5f, 2));
    }

    float MassMultiplier()
    {
        return Mathf.Clamp(cumulativeMass, 0, 4f) / 5;
    }

    Vector2 MouseTargetPos()
    {
        return mainCam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
    }

    Vector2 TargetDir()
    {
        return MouseTargetPos() - (Vector2)transform.position;
    }

    public void PlayerDeath()
    {
        Destroy(gameObject, 0.1f);
    }
}

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

    Camera mainCam;
    Rigidbody2D rb;

    bool isBoosting = false;
    float currentFuel;

    private void Awake()
    {
        mainCam = Camera.main;
        rb = GetComponent<Rigidbody2D>();
        currentFuel = maxFuel;
    }

    private void Update()
    {
        RotateSelf();
        MoveTowardsTarget();

        if (Input.GetMouseButton(0) && currentFuel > 0f)
        {
            isBoosting = true;
        }
        else
            isBoosting = false;
    }

    private void LateUpdate()
    {
        mainCam.transform.position = Vector3.Lerp(mainCam.transform.position, new Vector3(transform.position.x, transform.position.y, -10), Time.deltaTime * 6);

        // Do the fuel thing here
        if (isBoosting && currentFuel > 0f)
        {
            float drain = SpeedMultiplier() * fuelDrainRate * Time.deltaTime;
            currentFuel = Mathf.Max(currentFuel - drain, 0f);

            if (currentFuel <= 0f)
                isBoosting = false;
        }
        else
        {
            float drain = fuelDrainRate * Time.deltaTime;
            currentFuel = Mathf.Max(currentFuel - drain, 0f);

            if (currentFuel <= 0f)
                isBoosting = false;
        }

        if (fuelGauge != null)
        {
            Vector2 mouseScreenPos = Input.mousePosition;
            fuelGauge.position = mouseScreenPos;
        }

        if (fuelFillImage != null)
        {
            fuelFillImage.fillAmount = currentFuel / maxFuel;
        }
    }

    void RotateSelf()
    {
        Vector2 targetDir = TargetDir();
        float angle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg;

        Quaternion rot = Quaternion.Euler(new Vector3(0, 0, angle));
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * rotateSpeed);
    }

    /// <summary>
    /// Moves rocket in the direction it's facing.
    /// </summary>
    void MoveTowardsTarget()
    {
        transform.position += transform.right * Time.deltaTime * moveSpeed * SpeedMultiplier();
    }

    /// <summary>
    /// A multiplier based on cursor distance from rocket. Applies an x2 boost if isBoosting
    /// </summary>
    /// <returns></returns>
    float SpeedMultiplier()
    {
        Debug.Log(Vector2.Distance(MouseTargetPos(), transform.position) / 2);
        return !isBoosting ? Mathf.Clamp(Vector2.Distance(MouseTargetPos(), transform.position) / 2, 0, 1) :
                            Mathf.Clamp(Vector2.Distance(MouseTargetPos(), transform.position), 0.5f, 2);
    }
    Vector2 MouseTargetPos()
    {
        return mainCam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
    }

    Vector2 TargetDir()
    {
        return MouseTargetPos() - (Vector2)transform.position;
    }
}

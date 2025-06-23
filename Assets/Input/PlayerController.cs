using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    float rotateSpeed = 5f,
          moveSpeed = 10f;

    Camera mainCam;
    Rigidbody2D rb;

    bool isBoosting = false;

    private void Awake()
    {
        mainCam = Camera.main;
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        RotateSelf();
        MoveTowardsTarget();

        if (Input.GetMouseButton(0))
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

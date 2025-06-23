using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    float rotateSpeed = 5f,
          moveSpeed = 10f;

    Camera mainCam;
    bool isBoosting = false;

    private void Awake()
    {
        mainCam = Camera.main;
    }
    private void Update()
    {
        RotateSelf();
        MoveTowardsTarget();

        if (Input.GetKey(KeyCode.Space))
        {
            isBoosting = true;
        }
        else
            isBoosting = false;
    }

    private void LateUpdate()
    {
        mainCam.transform.position = Vector3.Lerp(mainCam.transform.position, new Vector3(transform.position.x, transform.position.y, -10), Time.deltaTime * 6);
    }

    void RotateSelf()
    {
        Vector2 targetDir = TargetDir();
        float angle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg;

        Quaternion rot = Quaternion.Euler(new Vector3(0, 0, angle));
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime*rotateSpeed);
    }

    void MoveTowardsTarget()
    {
        transform.position += transform.right * Time.deltaTime * moveSpeed * SpeedMultiplier();
    }

    Vector2 MouseTargetPos()
    {
        return mainCam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
    }

    Vector2 TargetDir()
    {
        return MouseTargetPos() - (Vector2)transform.position;
    }

    /// <summary>
    /// Based on mouse world distance from transform position
    /// </summary>
    /// <returns></returns>
    float SpeedMultiplier()
    {
        Debug.Log(Vector2.Distance(MouseTargetPos(), transform.position)/2);
        return Mathf.Clamp(Vector2.Distance(MouseTargetPos(), transform.position)/2, isBoosting?0.5f:0, isBoosting?2:1);
    }
}

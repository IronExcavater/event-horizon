using UnityEngine;

public class BlackHoleScaleManager : MonoBehaviour
{
    public Vector3 blackHoleScale;
    public float smoothTime;
    private Vector3 _velocity;

    // Update is called once per frame
    void Update()
    {
        transform.localScale = Vector3.SmoothDamp(transform.localScale, blackHoleScale, ref _velocity, smoothTime);
    }
}

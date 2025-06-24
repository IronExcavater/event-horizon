using UnityEngine;

public class BlackHoleScaleManager : MonoBehaviour
{
    [Header("Black Hole Scale Settings")]
    public float currentScale = 1f;
    private Vector3 blackHoleScale;
    public float smoothTime;
    private Vector3 _velocity;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        blackHoleScale = new Vector3(currentScale, currentScale, currentScale);
        transform.localScale = Vector3.SmoothDamp(transform.localScale, blackHoleScale, ref _velocity, smoothTime);
        audioSource.volume = Mathf.Clamp01(currentScale/15);
    }
}

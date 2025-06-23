using Animation;
using UnityEngine;

public class BlackHoleScaleManager : MonoBehaviour
{
    public float blackHoleScale;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = new Vector3(blackHoleScale, blackHoleScale, blackHoleScale);
        AnimationManager.CreateTween(this, scale => transform.localScale = scale, transform.localScale, () => blackHoleScale, null, Easing.EaseInOutCubic);
    }
}

using UnityEngine;

public class MenuCameraRotate : MonoBehaviour
{
    [SerializeField] private Vector3 rotationFrequency = new Vector3(0.2f, 0.3f, 0.1f);
    [SerializeField] private Vector3 rotationAmplitude = new Vector3(30f, 45f, 15f);

    private float _time;

    private void Update()
    {
        _time += Time.deltaTime;

        float x = Mathf.Sin(_time * rotationFrequency.x) * rotationAmplitude.x;
        float y = Mathf.Sin(_time * rotationFrequency.y) * rotationAmplitude.y;
        float z = Mathf.Sin(_time * rotationFrequency.z) * rotationAmplitude.z;

        transform.rotation = Quaternion.Euler(x, y, z);
    }
}

using UnityEngine;

public class RandomRotate : MonoBehaviour
{
    private Vector3 randomStartingRotation;
    public float rotationSpeed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        randomStartingRotation = new Vector3(Random.Range(0, 359), Random.Range(0, 359), Random.Range(0, 359));
        transform.rotation = Quaternion.Euler(randomStartingRotation);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.forward, Time.deltaTime * rotationSpeed);
    }
}

using UnityEngine;

[CreateAssetMenu(fileName = "Debris", menuName = "Scriptable Objects/Debris")]
public class Debris: ScriptableObject
{
    public float spawnWeight;
    public int points;
    public GameObject[] debris;
    public Debris(float _weight, GameObject[] _debris)
    {
        spawnWeight = _weight;
        debris = _debris;
    }
}

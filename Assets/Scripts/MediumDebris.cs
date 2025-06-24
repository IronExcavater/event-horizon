using UnityEngine;

public class MediumDebris : Consumable
{
    [SerializeField] int splitAmount = 3;

    public override void DamageWithPoints()
    {
        DebrisSpawner.instance.SpawnDebris(0, transform.position, splitAmount);

        base.DamageWithPoints();
    }
}

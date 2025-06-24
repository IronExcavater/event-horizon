using UnityEngine;

public class PowerupDebris : Consumable
{
    public enum PowerUp
    {
        None,
        Shield,
        Fuel,
        Sticky
        //add powerups
    }

    public PowerUp buff;

    //add reference to player to apply power ups

    public override void DamageWithPoints()
    {
        switch (buff)
        {
            case PowerUp.None:
                break;
            case PowerUp.Shield:
                Shield();
                break;
            case PowerUp.Fuel:
                Fuel();
                break;
            case PowerUp.Sticky:
                Sticky();
                break;
        }

        base.DamageWithPoints();
    }

    void Shield()
    {
        //Shield that tanks a hit
    }
    void Fuel()
    {
        //Infinite fuel for a few seconds
    }
    void Sticky()
    {
        //Sticky hook?
    }
}

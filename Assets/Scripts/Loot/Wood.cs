using UnityEngine;

public class Wood : Loot
{
    public override void Collected()
    {
        StaticClass.Instance.inventory.wood += 1;
        base.Collected();
    }
    
}


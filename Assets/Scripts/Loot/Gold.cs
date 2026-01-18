using UnityEngine;

public class Gold : Loot
{
    public override void Collected()
    {
        StaticClass.Instance.inventory.gold += 1;
        base.Collected();
    }


}

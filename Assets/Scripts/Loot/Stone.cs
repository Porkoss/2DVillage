using UnityEngine;

public class Stone : Loot
{

    public override void Collected()
    {
        StaticClass.Instance.inventory.stone += 1;
        base.Collected();
    }

}

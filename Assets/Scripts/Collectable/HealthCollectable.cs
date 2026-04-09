using UnityEngine;

public class HealthCollectable : Health
{
    
    public override void Death()
    {
        //Debug.Log(gameObject.name + " is dead"); 
        //SoundManager.PlayRandomSoundFromType(SoundType.Die, 0.4f);
        //Collectable collectable = GetComponent<Collectable>();
    }
}

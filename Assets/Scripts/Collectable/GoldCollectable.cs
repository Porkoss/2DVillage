using Unity.VisualScripting;
using UnityEngine;

public class GoldCollectable : Collectable
{
    [SerializeField] GameObject VFXPrefab;
    public override void PlayAnimationForPlayer()
    {
        
        playerAnimator.SetTrigger("Mine");
    }

    public override void PlayAnimationForCollectable()
    {
        
        collectableAnimator.SetTrigger("Mine");

    }


    public override void SetUp()
    {
        base.SetUp();
        float Offset = Random.value;
        collectableAnimator.SetFloat("Offset", Offset);
    }

    public override void EndGameObject()
    {
        base.EndGameObject();
        var Vfx = Instantiate(VFXPrefab, transform.position, Quaternion.identity);
        Vfx.GetComponent<VFXHandler>().bIsInstantiable = true;
    }

}

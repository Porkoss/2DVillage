using UnityEngine;

public class StoneCollectable : Collectable
{
    [SerializeField] GameObject VFXPrefab;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void PlayAnimationForPlayer()
    {
        
        playerAnimator.SetTrigger("Mine");
    }

    public override void PlayAnimationForCollectable()
    {

        collectableAnimator.SetTrigger("Mine");

    }

    public override void EndGameObject()
    {
        base.EndGameObject();
        var Vfx = Instantiate(VFXPrefab, transform.position,Quaternion.identity);
        Vfx.GetComponent<VFXHandler>().bIsInstantiable = true;
    }
}

using UnityEngine;

public class TreeCollectable : Collectable
{
    private bool bIsStump=false;
    [SerializeField] private float SecondLifeTimer;
    [SerializeField] private float runningSecondLifeTimer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void PlayAnimationForPlayer()
    {
        
        playerAnimator.SetTrigger("Chop");
    }


    public override void PlayAnimationForCollectable()
    {
        if (health.IsAlive())
        {
            
            collectableAnimator.SetTrigger("Chop");

        }
        if(!health.IsAlive()) 
        {
            collectableAnimator.SetTrigger("TreeFall");
        }

    }

    public override void EndGameObject()
    {
        bIsStump = true;
    }

    protected override void Update()
    {

        if (runningSecondLifeTimer > SecondLifeTimer && bIsStump)
        {
            runningSecondLifeTimer = 0;
            health.FullHeal();
            collectableAnimator.SetTrigger("NewTree");
            bIsStump=false;
        }

        runningSecondLifeTimer += Time.deltaTime;
    }
}

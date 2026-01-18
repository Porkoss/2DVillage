using UnityEngine;

public class TreeCollectable : Collectable
{
    private bool bIsStump=false;
    [SerializeField] private float SecondLifeTimer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void PlayAnimationForPlayer()
    {
        
        playerAnimator.SetTrigger("Chop");
    }


    public override void PlayAnimationForCollectable()
    {
        if (Health > 0)
        {
            
            collectableAnimator.SetTrigger("Chop");
            //TO DO : Add Sound
        }
        if(Health <= 0) 
        {
            collectableAnimator.SetTrigger("TreeFall");
            //TO DO : Add Sound
        }

    }

    public override void EndGameObject()
    {
        bIsStump = true;
    }

    protected override void Update()
    {
        if (runningRegenTimer > RegenTimer && !bIsStump)
        {
            Health = MaxHealth;
            runningRegenTimer = 0;
            //maybe reset "breaking animation" not sure if the project will reach this place
        }
        if (Health != MaxHealth)
        {
            runningRegenTimer += Time.deltaTime;
        }
        if (runningRegenTimer > SecondLifeTimer && bIsStump)
        {
            Health = MaxHealth;
            runningRegenTimer = 0;
            collectableAnimator.SetTrigger("NewTree");
            bIsStump=false;
            //TO DO : play regen animation
        }
    }
}

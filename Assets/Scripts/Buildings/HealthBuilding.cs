using UnityEngine;

public class HealthBuilding : Health
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    int DestructionStep=2;
    public override void Death()
    {
        Debug.Log(gameObject.name + " is dead");

        StaticClass.Instance.listOfBuiltBuilding.Remove(gameObject);
        
    }

    public override bool TakingDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth<= (DestructionStep/3.0f) * maxHealth)
        {
            gameObject.GetComponent<Building>().StepByStepFireDestruction();
            DestructionStep--;
        }

        
        if (currentHealth <= 0)
        {
            Death();
            return true;
        }
        return false;
    }

    public override void Reset()
    {
        DestructionStep = 2;
        base.Reset();
    }
    
    public override bool HealingDamage(float damageHealed)
    {
        if (currentHealth > (DestructionStep / 3.0f) * maxHealth)
        {
            gameObject.GetComponent<Building>().StepByStepFireReconstruction();
            DestructionStep++;
        }
        return base.HealingDamage(damageHealed);
    }

}

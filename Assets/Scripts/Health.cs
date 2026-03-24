using UnityEngine;

public class Health: MonoBehaviour
{
	[SerializeField] protected float currentHealth;
	[SerializeField] protected float maxHealth;

	[SerializeField] protected float regenTimer = 10f;
	 protected float runningRegenTimer = 0f;

    public virtual void Death()
	{
		Debug.Log(gameObject.name + " is dead"); 
        SoundManager.PlayRandomSoundFromType(SoundType.Die, 0.4f);
        Destroy(gameObject);
	}
	public virtual bool TakingDamage(float damage)
	{
		Debug.Log(gameObject.name + " is taking " + damage);
		currentHealth -= damage;
		runningRegenTimer = 0;
		if (currentHealth <= 0)
		{	
			Death();
			return true;
		}
		return false;
	}

	public virtual bool HealingDamage(float damageHealed)
	{
		if (currentHealth >= maxHealth)
		{
			return false;
		}
		else
		{
			currentHealth += damageHealed;
			if(currentHealth > maxHealth)
			{
				currentHealth = maxHealth;	
			}
			return true;
		}
	}


	public void FullHeal()
	{
		currentHealth = maxHealth;
	}
	public virtual void Reset()
	{
		currentHealth = maxHealth;
	}

	public float GetHealthPercent()
	{
		return currentHealth/maxHealth;
	}

	public void passiveRegen()
    {
        if (runningRegenTimer > regenTimer)
        {
            runningRegenTimer = 0;
            currentHealth = maxHealth;
        }
        runningRegenTimer += Time.deltaTime;

    }

	public float GetHealth()
	{
		return currentHealth;
	}

	public bool IsAlive()
	{
		return currentHealth > 0;
	}
    public void Update()
    {
		passiveRegen();
    }


	
}
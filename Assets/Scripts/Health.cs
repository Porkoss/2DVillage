using UnityEngine;

public class Health: MonoBehaviour
{
	[SerializeField] float currentHealth;
	[SerializeField] float maxHealth;
	//TO DO : herites this class in order to adapt to entities ?
    void Death()
	{
		Debug.Log(gameObject.name + " is dead"); //TO DO : update this code if necessary
		//Destroy(gameObject);
	}
	public bool TakingDamage(float damage)
	{
		Debug.Log(gameObject.name + " is taking " + damage);
		currentHealth -= damage;
		if (currentHealth <= 0)
		{
			Death();
			return true;
		}
		return false;
	}
}
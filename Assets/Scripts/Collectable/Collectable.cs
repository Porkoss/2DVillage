using JetBrains.Annotations;
using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.Rendering;

public class Collectable : MonoBehaviour
{
    // purpose of this Script is to handle the collectable object that need to be break down by player to get loot 
    [SerializeField] List<GameObject> droppableObjects;
    [SerializeField] List<int> droppableQuantities;

    [Header("Health")]
    public float Health = 2f;
    public float MaxHealth = 2f;
    public float RegenTimer = 3f;
    protected float runningRegenTimer = 0f;
    //TO DO : rework code with health component

    protected PlayerController controller;
    protected Animator playerAnimator;
    protected Animator collectableAnimator;

    private void Start()
    {
        if (StaticClass.Instance)
        {
            controller = StaticClass.Instance.player.GetComponent<PlayerController>();
            playerAnimator = StaticClass.Instance.player.GetComponent<Animator>();
        }

        collectableAnimator = GetComponent<Animator>();
        SetUp();
    }

    protected virtual void Update()
    {
        if(runningRegenTimer > RegenTimer)
        {
            Health = MaxHealth;
            runningRegenTimer = 0;
            //maybe reset "breaking animation" not sure if the project will reach this place
        }
        if (Health != MaxHealth)
        {
            runningRegenTimer += Time.deltaTime;
        }
    }
    public void Break()
    {
        int whatDropFromList= Random.Range(0,droppableObjects.Count-1);

        try
        {
            int HowMuchDrop = Random.Range(1, droppableQuantities[whatDropFromList]);
            for (int i=0;  i<HowMuchDrop; i++)
            {
                GenerateOneObject(droppableObjects[whatDropFromList]);
            }
        }
        catch
        {
            Debug.Log("Droppable List is uncomplete for object "+ gameObject.name);
        }

        EndGameObject();
        //add animation here
        
    }


    public void GenerateOneObject(GameObject prefab)
    {
        GameObject lootable = Instantiate(prefab, transform.position, Quaternion.identity);
        //add logic here to handle the "emission" of the loot ( prob add a rigidbody to the prefab and add a small random up force here by taking lootable go
        Rigidbody2D rb = lootable.GetComponent<Rigidbody2D>();
        Vector2 force = new Vector2(Random.Range(-0.3f,0.3f)*100, Random.Range(0.1f,0.3f)*1000);
        rb.AddForce(force);
    }
    
    public virtual void EndGameObject()
    {
        Destroy(gameObject);
    }
    public void TakeDamage(float damage)
    {
        if(Health <= 0)
        {
            return; //fail safe
        }
        Health-= damage;
        runningRegenTimer = 0f;
        Debug.Log(gameObject.name + "has " + Health);
        PlayAnimationForPlayer();
        PlayAnimationForCollectable();

    }

    public virtual void PlayAnimationForPlayer()
    {

    }

    public virtual void PlayAnimationForCollectable()
    {

    }

    public void OnHitAnimationEnded()
    {
        if (Health <= 0)
        {
            Break();
        }
    }

     public  virtual void SetUp()
    {

    }

}

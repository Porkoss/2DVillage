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



    protected PlayerController controller;
    protected Animator playerAnimator;
    protected Animator collectableAnimator;

    public Health health;

    private void Start()
    {
        if (StaticClass.Instance)
        {
            controller = StaticClass.Instance.player.GetComponent<PlayerController>();
            playerAnimator = StaticClass.Instance.player.GetComponent<Animator>();
        }
        health = GetComponent<Health>();
        collectableAnimator = GetComponent<Animator>();
        SetUp();
    }

    protected virtual void Update()
    {

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


    public void TakeDamage(float damage)
    {
        health.TakingDamage(damage);
        Debug.Log(gameObject.name + "has " + health.GetHealth());
        PlayAnimationForPlayer();
        PlayAnimationForCollectable();
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


    public virtual void PlayAnimationForPlayer()
    {

    }

    public virtual void PlayAnimationForCollectable()
    {

    }

    public void OnHitAnimationEnded()
    {
        if (!health.IsAlive())
        {
            Break();
        }
    }

     public  virtual void SetUp()
    {

    }

}

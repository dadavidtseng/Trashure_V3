using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Monster : Mover
{
    //Logic
    public float triggerLength = 5;
    public float chaseLength = 25;
    private bool chasing;
    private bool collidingWithPlayer;
    private Transform playerTransform;
    private Vector3 startingPosition;
    public Player player;

    public int hitTime;

    //Hitbox
    public ContactFilter2D filter;
    private BoxCollider2D hitbox;
    private Collider2D[] hits = new Collider2D[10];
    

    protected override void Start()
    {
        base.Start();
        playerTransform = player.transform;
        startingPosition = transform.position;
        hitbox = transform.GetChild(0).GetComponent<BoxCollider2D>();
    }

    private void FixedUpdate()
    {
        //Is the player in range?
        if(Vector3.Distance(playerTransform.position, startingPosition) < chaseLength)
        {
            if(Vector3.Distance(playerTransform.position, startingPosition) < triggerLength)
            {
                chasing = true;
            }

            if(chasing)
            {
                if(!collidingWithPlayer)
                {
                    UpdateMotor((playerTransform.position - transform.position).normalized);
                    hitTime++;
                    Death();
                }
            }
            else
            {
                UpdateMotor(startingPosition - transform.position);
            }
        }
        else
        {
            UpdateMotor(startingPosition - transform.position);
            chasing = false;
        }

        //Check for overlaps
        collidingWithPlayer = false;
        boxCollider.OverlapCollider(filter, hits);
        for (int i = 0; i < hits.Length; i++)
        {
            if(hits[i] == null)
                continue;

            if(hits[i].tag == "Player" && hits[i].name == "Player")
            {
                collidingWithPlayer = true;
            }

            //The array is not cleaned up, so we do it ourself
            hits[i] = null;
        }
    }

    protected override void Death()
    {
        if (hitTime == 300)
        {
            Destroy(gameObject);
            Debug.Log("X");
        }
    }
}

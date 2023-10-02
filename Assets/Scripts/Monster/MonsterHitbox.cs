using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterHitbox : Collidable
{
    //Damage
    public int damage =1;
    public float pushForce = 5;

    protected override void OnCollide(Collider2D coll)
    {
        if(coll.tag == "Player" && coll.name == "Player")
        {
            //Creat a new damage object, then we'll send it to the fighter we've hit
            Damage dmg = new Damage
            {
                damageAmount = damage,
                origin = transform.position,
                pushForce = pushForce
            }; 
            
            coll.SendMessage("ReceiveDamage", dmg);
        }  
    }
}
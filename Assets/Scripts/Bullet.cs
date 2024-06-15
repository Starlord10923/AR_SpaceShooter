using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage = 2f;
    public float speed = 10f;
    public AudioSource audioSource;
    public AudioClip hitClip;
    public GameObject hitEffect;

    void Start()
    {
        Destroy(gameObject, 3f);
    }
    void Update()
    {
        transform.position += transform.forward *speed * Time.deltaTime;
    }
    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Enemy")){
            Debug.Log("Hit Enemy : "+other.gameObject.name+" : "+other.transform.position);
            other.GetComponent<BattleUnit>()?.TakeDamage(damage);
            
            audioSource.clip = hitClip;
            audioSource.Play();

            GameObject hitEffectInstance = Instantiate(hitEffect, transform.position, transform.rotation);
            Destroy(hitEffectInstance, 1.05f);
        }else if(other.CompareTag("Player")){
            Debug.Log("Hit Player : "+other.gameObject.name);
            // other.GetComponent<BattleUnit>()?.TakeDamage(damage);
        }
        
    }
}

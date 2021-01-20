using System.Collections;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Stats")]
    [SerializeField] float health = 100;
    [SerializeField] int scoreValue = 50;

    [Header("Shooting")]
    [SerializeField] float shotCounter;
    [SerializeField] float minTimeBetweenShots = 0.2f;
    [SerializeField] float maxTimeBetweenShots = 3f;
    [SerializeField] GameObject enemyLaserPrefab;

    [Header("Effects")]
    [SerializeField] GameObject deathVFX;
    [SerializeField] float explosionDuration = 1f;
    
    [SerializeField] AudioClip enemyDeathSound;
    [SerializeField] [Range(0,1)] float enemyDeathSoundVolume = 0.75f;

    [SerializeField] AudioClip shootSound;
    [SerializeField] [Range(0, 1)] float shootSoundVolume = 0.25f;

    [SerializeField] float enemyLaserSpeed = 10f;


    void Start()
    {
        shotCounter = Random.Range(minTimeBetweenShots, maxTimeBetweenShots);
    }

    
    void Update()
    {
        CountDownAndShoot();
    }

    private void CountDownAndShoot()
    {
        
        shotCounter -= Time.deltaTime;
        if (shotCounter <= 0f)
        {
            EnemyFire();
            shotCounter = Random.Range(minTimeBetweenShots, maxTimeBetweenShots);
        }
    }
   
    private void EnemyFire()
    {
        GameObject enemyLaser = Instantiate(enemyLaserPrefab, transform.position, Quaternion.identity) as GameObject;
    
        enemyLaser.GetComponent<Rigidbody2D>().velocity = new Vector2(0, -enemyLaserSpeed);

        AudioSource.PlayClipAtPoint(shootSound, Camera.main.transform.position, shootSoundVolume);
    }

    
    private void OnTriggerEnter2D(Collider2D other)
    {
        
        DamageDealer damageDealer = other.gameObject.GetComponent<DamageDealer>();

       
        if(!damageDealer)
        {
            return;
        }

        ProcessHit(damageDealer);
    }

    
    private void ProcessHit(DamageDealer damageDealer)
    {
        health -= damageDealer.GetDamage();
    
        damageDealer.Hit();
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        
        FindObjectOfType<GameSession>().AddToScore(scoreValue);
       
        Destroy(gameObject);
      
        GameObject explosion = Instantiate(deathVFX, transform.position, Quaternion.identity);
        
        Destroy(explosion, explosionDuration);
       
        AudioSource.PlayClipAtPoint(enemyDeathSound, Camera.main.transform.position, enemyDeathSoundVolume);
    }
}

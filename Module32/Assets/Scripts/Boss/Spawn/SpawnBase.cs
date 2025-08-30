using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy;

public class SpawnBase : MonoBehaviour
{
    public GameObject prefab;
    public GameObject pack;
    public GameObject[] waypoints;

    private bool _spawned = false;
    private GameObject spawnedEnemy;
    private bool _isInAttackZone = false;
    private bool _isShooting = false;

    private void OnTriggerEnter(Collider other)
    {
        _isInAttackZone = true;

        if (!_spawned && other.transform.CompareTag("Player"))
        {
            spawnedEnemy = Instantiate(prefab, transform.position, transform.rotation, pack.transform);

            EnemyWalk enemyWalk = spawnedEnemy.GetComponent<EnemyWalk>();
            if (enemyWalk != null)
            {
                enemyWalk.enemyPack = pack;
                enemyWalk.waypoints = waypoints;
            }

            _spawned = true;
        }

        if (spawnedEnemy != null && other.transform.CompareTag("Player") && !_isShooting)
        {
            EnemyWalk enemyWalk = spawnedEnemy.GetComponent<EnemyWalk>();
            EnemyBase enemyBase = spawnedEnemy.GetComponent<EnemyBase>();
            EnemyShoot enemyShoot = spawnedEnemy.GetComponent<EnemyShoot>();

            if (enemyWalk != null)
            {
                enemyWalk._canWalk = false;
            }

            if (enemyBase != null)
            {
                enemyBase.lookAtPlayer = true;
            }

            if (enemyShoot != null)
            {
                enemyShoot.Init();
                _isShooting = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            _isInAttackZone = false;

            if (spawnedEnemy != null && _isShooting)
            {
                EnemyWalk enemyWalk = spawnedEnemy.GetComponent<EnemyWalk>();
                EnemyBase enemyBase = spawnedEnemy.GetComponent<EnemyBase>();
                EnemyShoot enemyShoot = spawnedEnemy.GetComponent<EnemyShoot>();

                if (enemyWalk != null)
                {
                    enemyWalk._canWalk = true;
                }

                if (enemyBase != null)
                {
                    enemyBase.lookAtPlayer = false;
                }

                if (enemyShoot != null)
                {
                    enemyShoot.StopShooting();
                    _isShooting = false;
                }
            }
        }
    }
}
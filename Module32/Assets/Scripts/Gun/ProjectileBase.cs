using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy;

public class ProjectileBase : MonoBehaviour
{
    public float timeToDestroy = 2f;
    public float speed = 50f;
    public int damageAmount = 1;
    public List<string> tagsToHit;

    private Vector3 _targetDirection;
    private bool _isBossProjectile = false;
    private bool _directionSet = false;

    public float heightOffset = 1f;

    private void Awake()
    {
        Destroy(gameObject, timeToDestroy);
    }

    private void Start()
    {
        if (_isBossProjectile && !_directionSet)
        {
            SetDirectionToPlayer();
        }
    }

    void Update()
    {
        if (_isBossProjectile)
        {
            transform.Translate(_targetDirection * speed * Time.deltaTime, Space.World);
        }
        else
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }
    }

    private void SetDirectionToPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);

            Vector3 targetPosition = player.transform.position;

            targetPosition.y += heightOffset;

            _targetDirection = (targetPosition - transform.position).normalized;
            _directionSet = true;

            RotateTowardsTarget(targetPosition);
        }
        else
        {
            _targetDirection = transform.forward;
        }
    }

    private void RotateTowardsTarget(Vector3 targetPosition)
    {
        Vector3 directionToTarget = (targetPosition - transform.position).normalized;

        if (directionToTarget != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 1f);
        }
    }

    public void SetAsBossProjectile(bool isBossProjectile)
    {
        _isBossProjectile = isBossProjectile;

        if (_isBossProjectile && !_directionSet)
        {
            SetDirectionToPlayer();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        foreach (var tg in tagsToHit)
        {
            if (collision.transform.tag == tg)
            {
                var damageable = collision.transform.GetComponent<IDamageable>();

                if (damageable != null)
                {
                    Vector3 dir = collision.transform.position - transform.position;
                    dir = -dir.normalized;
                    dir.y = 0;

                    damageable.Damage(damageAmount, dir);
                    Destroy(gameObject);
                }

                break;
            }
        }
    }
}
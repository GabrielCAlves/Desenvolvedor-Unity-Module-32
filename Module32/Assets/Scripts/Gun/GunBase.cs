using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy;
using Animation;

public class GunBase : MonoBehaviour
{
    public ProjectileBase prefabProjectile;

    public Transform positionToShoot;

    public float timeBetweenShoot = .2f;

    public float speed = 50f;

    //public AudioSource audioSource;

    private Coroutine _currentCoroutine;

    private bool _isShooting = false;

    protected virtual IEnumerator ShootCoroutine()
    {
        while (_isShooting)
        {
            Shoot();
            yield return new WaitForSeconds(timeBetweenShoot);
        }
    }

    public virtual void Shoot()
    {
        var projectile = Instantiate(prefabProjectile);
        projectile.transform.position = positionToShoot.position;
        projectile.transform.rotation = positionToShoot.rotation;

        if (transform.parent != null && transform.parent.name.Contains("Boss"))
        {
            projectile.GetComponent<ProjectileBase>().SetAsBossProjectile(true);
        }
    }

    public void StartShoot()
    {
        _isShooting = true;
        _currentCoroutine = StartCoroutine(ShootCoroutine());
    }

    public void StopShoot()
    {
        if (_currentCoroutine != null)
        {
            _isShooting = false;
            StopCoroutine(_currentCoroutine);
        }
    }

    //public void PlayShootSound()
    //{
    //    audioSource.Play();
    //}
}

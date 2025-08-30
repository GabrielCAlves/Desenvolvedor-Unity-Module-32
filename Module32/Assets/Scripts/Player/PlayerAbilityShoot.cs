using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAbilityShoot : PlayerAbilityBase
{
    public List<GunBase> gunBaseList;
    public Transform gunPosition;

    private GunBase _currentGun;
    private int gunNumberSelected = 0;

    protected override void Init()
    {
        base.Init();

        CreateGun();

        inputs.Gameplay.Shoot.performed += ctx => StartShoot();
        inputs.Gameplay.Shoot.canceled += ctx => CancelShoot();

        inputs.Gameplay.SwitchGun1.performed += ctx => { SwitchGun(0); };
        inputs.Gameplay.SwitchGun2.performed += ctx => { SwitchGun(1); };
    }

    private void CreateGun()
    {
        _currentGun = Instantiate(gunBaseList[gunNumberSelected], gunPosition);

        _currentGun.transform.localPosition = _currentGun.transform.localEulerAngles = Vector3.zero;
    }

    private void SwitchGun(int gunNumber)
    {
        //if (_currentGun != null && _currentGun.name == (gunBaseList[gunNumber].name.ToString()+"(Clone)"))
        if (gunNumber == gunNumberSelected)
        {
            Debug.Log("Arma já equipada");
            return;
        }

        if (_currentGun != null)
        {
            DestroyGun();
        }

        gunNumberSelected = gunNumber;

        Debug.Log("Arma " + (gunNumberSelected + 1) + " selecionada!");

        CreateGun();
    }

    private void DestroyGun()
    {
        Destroy(_currentGun.gameObject);
        _currentGun = null;
    }

    private void StartShoot()
    {
        _currentGun.StartShoot();
        Debug.Log("Start Shoot");
    }

    private void CancelShoot()
    {
        _currentGun.StopShoot();
        Debug.Log("Stop Shoot");
    }
}

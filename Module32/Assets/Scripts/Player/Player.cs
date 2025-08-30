using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Ebac.StateMachine;

public class Player : MonoBehaviour, IDamageable
{
    public Rigidbody playerRigidbody;
    public Transform cameraTransform;

    [Header("Setup")]
    public SOPlayerSetup soPlayerSetup;

    //[Header("HealthBase")]
    //public HealthBase healthBase;
    public float startLife = 10;
    public float currentLife;

    [Header("Jump Collision Check")]
    public Collider collider;
    public float distToGround;
    public float spaceToGround = .1f;
    //public ParticleSystem jumpVFX;

    [Header("Rotation")]
    public float rotationSpeed = 1f;

    [Header("Friction Settings")]
    public float turningFrictionMultiplier = 0.3f;
    public float minFriction = 0.1f;

    private Animator _currentPlayer;
    
    public enum PlayerStates
    {
        RUN,
        IDLE,
        DEATH
    }

    public StateMachine<PlayerStates> stateMachine;

    public string death = "DeathTrigger";

    [Header("Configurações de Rotação")]
    public CharacterController characterController;
    public float speed = 1f;
    public float turnSpeed = 1f;
    public float gravity = 9.8f;

    [Header("Scale Settings")]
    public bool isScaling = false;

    [Header("Flash")]
    public List<FlashColor> flashcolors;


    private Vector3 _regularScale;
    //private Vector3 _jumpPoint;
    private float _currentSpeed;
    private bool _colided = false;
    private bool _isInTheGround = false;
    private bool _alreadyScaled = false;

    private bool _cancelOtherAnim = false;


    private float vSpeed = 0f;

    #region LIFE
    public void Damage(float damage)
    {
        currentLife -= damage;

        flashcolors.ForEach(i =>
        {
            i.Flash();
        });

        if (currentLife <= 0)
        {
            Destroy(gameObject, 2f);
        }
    }

    public void Damage(float damage, Vector3 dir)
    {
        Damage(damage);
    }

    #endregion
    
    private void Awake()
    {
        _currentPlayer = Instantiate(soPlayerSetup.player, transform);

        if(collider != null)
        {
            distToGround = collider.bounds.extents.y;
        }

        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }

        Init();
    }

    private void Init()
    {
        currentLife = startLife;
    }

    private void Start()
    {
        _regularScale = gameObject.transform.localScale;

        FlashColor[] mdls = gameObject.GetComponentsInChildren<FlashColor>();

        for (int i = 0; i < mdls.Length; ++i)
        {
            flashcolors.Add(mdls[i]);
            flashcolors[i].Start();
        }

        stateMachine = new StateMachine<PlayerStates>();

        stateMachine.Init();
        stateMachine.RegisterStates(PlayerStates.RUN, new StateBase());
        stateMachine.RegisterStates(PlayerStates.IDLE, new StateBase());
        stateMachine.RegisterStates(PlayerStates.DEATH, new StateBase());

        stateMachine.SwitchState(PlayerStates.IDLE, soPlayerSetup, _currentPlayer);
    }

    void Update()
    {   
        HandleMovement();
        
        if (!_isInTheGround)
        {
            jumpState();
        }

        if (currentLife <= 0 && !_cancelOtherAnim)
        {
            _cancelOtherAnim = true;
            stateMachine.SwitchState(PlayerStates.DEATH, soPlayerSetup, _currentPlayer, death);
        }
    }

    private void HandleMovement()
    {
        if (!_cancelOtherAnim)
        {
            if (Input.GetKey(KeyCode.LeftControl))
            {
                _currentSpeed = soPlayerSetup.speedRun;
                _currentPlayer.speed = 2;
            }
            else
            {
                _currentSpeed = soPlayerSetup.speed;
                _currentPlayer.speed = 1;
            }

            transform.Rotate(0, Input.GetAxis("Horizontal") * turnSpeed * Time.deltaTime, 0);

            var inputAxisVertical = Input.GetAxis("Vertical");
            var speedVector = transform.forward * inputAxisVertical * _currentSpeed;

            if (characterController.isGrounded)
            {
                Debug.Log("characterController.isGrounded: "+characterController.isGrounded);
                Debug.Log("_isInTheGround: " + _isInTheGround);
                
                vSpeed = 0;
                if (Input.GetKeyDown(KeyCode.Space) && _isInTheGround)
                {
                    vSpeed = soPlayerSetup.forceJump;

                    DOTween.Kill(playerRigidbody.transform);

                    //playerRigidbody.velocity = Vector3.up * soPlayerSetup.forceJump;
                    //playerRigidbody.transform.localScale = Vector3.one;

                    HandleScaleJump();
                    //PlayJumpVFX();

                    _isInTheGround = false;
                    _colided = false;
                }

                //HandleJump();
            }

            vSpeed -= gravity * Time.deltaTime;
            speedVector.y = vSpeed;

            characterController.Move(speedVector * Time.deltaTime);

            if(inputAxisVertical != 0)
            {
                stateMachine.SwitchState(PlayerStates.RUN, soPlayerSetup, _currentPlayer);
            }
            else
            {
                stateMachine.SwitchState(PlayerStates.IDLE, soPlayerSetup, _currentPlayer);
            }
        }
    }

    private void jumpState()
    {
        //if (_jumpPoint != null)
        //{
        //    if(_jumpPoint.y != gameObject.transform.position.y)
        //    {
        //        if (_jumpPoint.y < gameObject.transform.position.y)
        //        {
        //            _currentPlayer.SetBool(soPlayerSetup.boolJumpUp, true);
        //            _currentPlayer.SetBool(soPlayerSetup.boolJumpDown, false);
        //            new WaitForEndOfFrame();
        //        }
        //        else if (_jumpPoint.y > gameObject.transform.position.y)
        //        {
        //            _currentPlayer.SetBool(soPlayerSetup.boolJumpUp, false);
        //            _currentPlayer.SetBool(soPlayerSetup.boolJumpDown, true);
        //            new WaitForEndOfFrame();
        //        }
        //    }
        //}

        //_jumpPoint.y = gameObject.transform.position.y;
    }

    private void ScaleCharacter(float scaleZ, float scaleY, float scaleX)
    {
        isScaling = true;

        // Para todas as animações de escala atuais
        DOTween.Kill(playerRigidbody.transform);

        // Aplica a escala diretamente no transform, não no rigidbody
        transform.DOScale(new Vector3(scaleX, scaleY, scaleZ), soPlayerSetup.animationDuration)
            .SetLoops(2, LoopType.Yoyo)
            .SetEase(soPlayerSetup.ease)
            .OnComplete(() => {
                isScaling = false;
                playerRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
            });
    }

    //private void HandleJump()
    //{
    //    if (Input.GetKeyDown(KeyCode.Space) && _isInTheGround)
    //    {
    //        vSpeed = soPlayerSetup.forceJump;

    //        DOTween.Kill(playerRigidbody.transform);

    //        playerRigidbody.velocity = Vector3.up * soPlayerSetup.forceJump;
    //        playerRigidbody.transform.localScale = Vector3.one;

    //        HandleScaleJump();
    //        //PlayJumpVFX();

    //        _isInTheGround = false;
    //        _colided = false;
    //    }
    //}

    //private void PlayJumpVFX()
    //{
    //    VFXManager.Instance.PlayVFXByType(VFXManager.VFXType.JUMP, transform.position);
    //}

    private void HandleScaleJump()
    {
        ScaleCharacter(soPlayerSetup.jumpScaleZ, soPlayerSetup.jumpScaleY, soPlayerSetup.jumpScaleX);
    }

    private void HandleLanding()
    {
        if (_colided)
        {
            //_currentPlayer.SetBool(soPlayerSetup.boolJumpUp, false);
            //_currentPlayer.SetBool(soPlayerSetup.boolJumpDown, false);
            //_currentPlayer.SetBool(soPlayerSetup.boolJumpLanding, true);

            DOTween.Kill(playerRigidbody.transform);

            HandleScaleLanding();
        }

    }

    private void HandleScaleLanding()
    {
        ScaleCharacter(soPlayerSetup.landingScaleZ, soPlayerSetup.landingScaleY, soPlayerSetup.landingScaleX);
        
        playerRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground") && _isInTheGround)
        {
            return;
        }
        else if (collision.gameObject.CompareTag("Ground") && !_isInTheGround)
        {
            _colided = true;
            _isInTheGround = true;

            HandleLanding();
        }

        Debug.Log("Entrou em contato");
    }

    private void OnCollisionStay(Collision collision)
    {
        Debug.Log("Contato ativo");
        //_currentPlayer.SetBool(soPlayerSetup.boolJumpDown, false);
        //_currentPlayer.SetBool(soPlayerSetup.boolJumpLanding, false);
        if (!_alreadyScaled && collision.gameObject.CompareTag("Ground") && !isScaling)
        {
            ScaleCharacter(_regularScale.z, _regularScale.y, _regularScale.x);
            _alreadyScaled = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        Debug.Log("Sem contato");
        _alreadyScaled = false;
    }
}

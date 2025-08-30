using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Animation;

namespace Enemy
{
    public class EnemyBase : MonoBehaviour, IDamageable
    {
        public Collider collider;
        public FlashColor flashColor;
        public ParticleSystem particleSystem;

        public float startLife = 10f;

        [SerializeField] protected float _currentLife;

        public int damage = 5;

        public GameObject enemyPack;

        public bool lookAtPlayer = false;

        [Header("Start Animation")]
        public float startAnimationDuration = .2f;
        public Ease startAnimationEase = Ease.OutBack;
        public bool startWithBornAnimation = true;


        [Header("Animation Setups")]
        [SerializeField] private AnimationBase _animationBase;

        private Player _player;

        private void Awake()
        {
            Init();
        }

        public virtual void Init()
        {
            ResetLife();

            if (startWithBornAnimation)
            {
                BornAnimation();
            }
        }

        private void Start()
        {
            _player = GameObject.FindObjectOfType<Player>();
        }

        public virtual void Update()
        {
            if (lookAtPlayer)
            {
                transform.LookAt(_player.transform.position);
            }
        }

        protected void ResetLife()
        {
            _currentLife = startLife;
        }

        private void OnCollisionEnter(Collision collision)
        {
            Player p = collision.transform.GetComponent<Player>();

            if(p != null)
            {
                PlayAnimationByTrigger(AnimationType.ATTACK);
                p.Damage(5);
                PlayAnimationByTrigger(AnimationType.IDLE);
            }
        }

        public void Damage(float damage)
        {
            OnDamage(damage);
        }

        public void Damage(float damage, Vector3 dir)
        {
            transform.DOMove(transform.position - dir, .1f);
            OnDamage(damage);
        }

        public void OnDamage(float amount)
        {
            if (flashColor != null)
            {
                Debug.Log("flashColor != null");
                flashColor.Flash();
            }
            else
            {
                Debug.Log("flashColor == null");
            }

            _currentLife -= amount;

            if (_currentLife <= 0)
            {
                if(particleSystem != null)
                {
                    particleSystem.Emit(15);
                    particleSystem.transform.SetParent(null);
                }
                
                Kill();
            }
        }

        protected virtual void Kill()
        {
            OnKill();
        }

        protected virtual void OnKill()
        {
            if (collider != null)
            {
                collider.enabled = false;
            }

            PlayAnimationByTrigger(AnimationType.DEATH);
            Destroy(enemyPack, 2f);
        }

        #region ANIMATION
        private void BornAnimation()
        {
            transform.DOScale(0, startAnimationDuration).SetEase(startAnimationEase).From();
        }

        public void PlayAnimationByTrigger(AnimationType animationType)
        {
            _animationBase.PlayAnimationByTrigger(animationType);
        }

        #endregion


    }
}
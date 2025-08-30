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

    // Altura de compensação para acertar o player
    public float heightOffset = 1f;

    private void Awake()
    {
        Destroy(gameObject, timeToDestroy);
    }

    private void Start()
    {
        // Se for projétil do Boss, calcula a direção para o player
        if (_isBossProjectile && !_directionSet)
        {
            SetDirectionToPlayer();
        }
    }

    void Update()
    {
        if (_isBossProjectile)
        {
            // Move na direção fixa calculada no início
            transform.Translate(_targetDirection * speed * Time.deltaTime, Space.World);
        }
        else
        {
            // Move para frente (comportamento normal)
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }
    }

    private void SetDirectionToPlayer()
    {
        // Encontra o player na cena
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            // Calcula a distância entre o projétil e o player
            float distance = Vector3.Distance(transform.position, player.transform.position);
            Debug.Log($"Distância até o player: {distance} unidades");

            // Pega a posição do player e ajusta a altura
            Vector3 targetPosition = player.transform.position;

            // Ajusta a altura do alvo para compensar a diferença
            targetPosition.y += heightOffset;

            // Calcula a direção do projétil para o ponto ajustado
            _targetDirection = (targetPosition - transform.position).normalized;
            _directionSet = true;

            // Rotaciona o projétil para olhar na direção do movimento
            RotateTowardsTarget(targetPosition);

            Debug.Log($"Direção do projétil do Boss definida. Altura ajustada: {heightOffset}, Distância: {distance}");
        }
        else
        {
            Debug.LogWarning("Player não encontrado! Usando direção padrão.");
            _targetDirection = transform.forward;
        }
    }

    private void RotateTowardsTarget(Vector3 targetPosition)
    {
        // Calcula a rotação necessária para mirar no alvo
        Vector3 directionToTarget = (targetPosition - transform.position).normalized;

        // Se a direção não for zero (para evitar erro de normalização)
        if (directionToTarget != Vector3.zero)
        {
            // Cria uma rotação que olha na direção do alvo
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);

            // Aplica a rotação suavemente (opcional - pode ser instantânea também)
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 1f);

            // Ou para rotação instantânea:
            // transform.rotation = targetRotation;
        }
    }

    // Método público para definir se é um projétil do boss
    public void SetAsBossProjectile(bool isBossProjectile)
    {
        _isBossProjectile = isBossProjectile;

        // Se for ativado como projétil do boss e ainda não tem direção definida
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
using System.Collections;
using System.Linq;
using UnityEngine;

public class PlayerAttack : MonoBehaviour, IAttack
{
    [SerializeField] private FloatValue basePower;
    [SerializeField] private FloatValue attackInterval;
    [SerializeField] private AudioSource swordAttack;
    [SerializeField] private Weapon weapon;
    [SerializeField] private GameObjectValue attackTarget;

    //private IDamageable target;
    private bool attackOnGoing;

    public FloatValue BasePower
    {
        get => basePower;
        set => basePower = value;
    }

    public void Attack(IDamageable thisTarget)
    {
        if (thisTarget == null) return;
        swordAttack.Play();
        thisTarget.TakeDamage(BasePower);
    }

    private IEnumerator AttackOnInterval(IDamageable entity)
    {
        attackOnGoing = true;
        while (entity.CurrentHealth.RuntimeValue > 0f)
        {
            Attack(entity);
            if (attackOnGoing) yield return new WaitForSeconds(attackInterval.RuntimeValue);
            else yield break;
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (IsTargetInRange())
            {
                Debug.Log("Target acquired, ready to attack");
                StartCoroutine(AttackOnInterval(attackTarget as IDamageable));
            }
        }

        if (!IsTargetInRange() && attackOnGoing)
        {
            Debug.Log("Stopping active coroutine...");
            attackOnGoing = false;
        }
    }

    private bool IsTargetInRange()
    {
        if(attackTarget.Value != null) 
            return Vector3.Distance(transform.position, attackTarget.Value.transform.position) <= weapon.Range;
        
        return false;
    }
        
    
}

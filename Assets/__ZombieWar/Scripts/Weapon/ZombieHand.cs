using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieHand : Weapon
{
    [SerializeField] LayerMask playerLayer;
    [SerializeField] float attackRadius;
    bool isInAnimation = false;
    AnimationEventTrigger animationEventTrigger;
    public override bool IsAttackable => !isInAnimation;

    public override void Init(WeaponSO weaponSO, Character holder)
    {
        base.Init(weaponSO, holder);
        animationEventTrigger = holder.Animator.GetComponent<AnimationEventTrigger>();
        animationEventTrigger.OnAnimationEventTriggered += HandleAnimationEventTrigger;
    }

    protected override void OnDestroy()
    {
        animationEventTrigger.OnAnimationEventTriggered -= HandleAnimationEventTrigger;
    }

    void HandleAnimationEventTrigger(AnimationEventTrigger.TriggeredEventData data)
    {
        if (data.stringParam == "Launch")
        {
            ThrowProjectile();
        }
        else if (data.stringParam == "End")
        {
            isInAnimation = false;
            holder.CharacterMovement.IsStop = false;
        }
    }

    public override void Attack(Vector3 target)
    {
        holder.Animator.SetTrigger("Attack");
        holder.CharacterMovement.IsStop = true;
        isInAnimation = true;
    }

    void ThrowProjectile()
    {
        var projectile = GetProjectile<ZombieAttackProjectile>();
        if (projectile)
        {
            projectile.gameObject.SetActive(true);
            projectile.Launch(Vector3.zero, 0);
        }
    }

    protected override void ProjectileCollide(Collision other)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, attackRadius, playerLayer);
        if (colliders.Length > 0)
        {
            foreach (var collider in colliders)
            {
                if (collider.TryGetComponent(out Character character))
                {
                    character.Hit(weaponDamage, new HitData()
                    {
                        forcePosition = transform.position,
                        forcePower = weaponForce,
                        forceNormal = -transform.forward
                    });
                }
            }
            audioSource.PlayOneShot(audioSource.clip);
            Instantiate(hitParticlePrefab, transform.position, Quaternion.LookRotation(-transform.forward));
        }
        //Spawn hit particle at contact point
    }
}

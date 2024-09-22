using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AnimationEventTrigger;

public class Grenade : Weapon
{
    [SerializeField] GameObject model;
    [SerializeField] float explodeRange;
    [SerializeField] LayerMask explodeLayer;
    bool isInAnimation = false;
    AnimationEventTrigger animationEventTrigger;

    public override bool IsAttackable => !isInAnimation;
    Vector3? target = null;
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

    void HandleAnimationEventTrigger(TriggeredEventData data)
    {
        if (data.stringParam == "Launch")
        {
            ThrowProjectile();
            model.SetActive(false);
        }
        else if (data.stringParam == "Attackable")
        {
            isInAnimation = false;
            model.SetActive(true);
        }
    }

    public override void Attack(Vector3 target)
    {
        holder.Animator.SetTrigger("Attack");
        this.target = target;
    }

    void ThrowProjectile()
    {
        if (target == null) return;
        var projectile = GetProjectile();
        if (projectile)
        {
            projectile.gameObject.SetActive(true);
            Vector3 direction = (Vector3)target;
            projectile.Launch(direction, 0);
            audioSource.PlayOneShot(audioSource.clip);
        }
        target = null;
    }

    protected override void ProjectileCollide(Collision other)
    {
        Collider[] colliders = Physics.OverlapSphere(other.GetContact(0).point, explodeRange / 2, explodeLayer);
        if (colliders.Length > 0)
        {
            foreach (var collider in colliders)
            {
                if (collider.TryGetComponent(out Character character))
                {
                    character.Hit(weaponDamage, new HitData()
                    {
                        forcePosition = other.GetContact(0).point,
                        forcePower = weaponForce
                    });
                }
            }
        }

        //Spawn hit particle at contact point
        var ps = Instantiate(hitParticlePrefab, other.GetContact(0).point, Quaternion.LookRotation(other.GetContact(0).normal));
        ps.transform.localScale *= explodeRange;
    }
}

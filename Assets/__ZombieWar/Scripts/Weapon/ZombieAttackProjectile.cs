using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieAttackProjectile : Projectile
{
    public override void Launch(Vector3 _, float __)
    {
        gameObject.SetActive(false);
        cld.enabled = false;
        OnCollide?.Invoke(null);
        ReturnMethod?.Invoke(this);
    }

    public override void ResetToOrgState()
    {
        launching = false;
    }
}

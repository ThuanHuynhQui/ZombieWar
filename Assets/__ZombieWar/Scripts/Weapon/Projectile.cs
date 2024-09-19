using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    protected Action<Projectile> ReturnMethod = delegate { };
    protected Action<Collision> OnCollide = delegate { };
    [SerializeField] Rigidbody rb;
    [SerializeField] Collider cld;
    protected bool launching;
    protected Vector3 direction;
    protected float speed;
    protected float lifeTime = 5;
    protected float remainingLifeTime = 0;

    public virtual void Setup(Action<Projectile> returnMethod, Action<Collision> collideMethod)
    {
        ReturnMethod = returnMethod;
        OnCollide = collideMethod;
    }
    public virtual void Launch(Vector3 direction, float speed)
    {
        transform.parent = null;
        transform.rotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
        this.speed = speed;
        this.direction = direction;
        launching = true;
    }

    public T Cast<T>() where T : Projectile
    {
        return this as T;
    }

    public virtual void ResetToOrgState()
    {
        cld.enabled = true;
        launching = false;
        remainingLifeTime = lifeTime;
        rb.velocity = Vector3.zero;
        transform.localPosition = Vector3.zero;
    }

    protected virtual void FixedUpdate()
    {
        if (!launching) return;
        rb.velocity = direction.normalized * speed;
        remainingLifeTime -= Time.fixedDeltaTime;
        if (remainingLifeTime <= 0)
        {
            ReturnMethod?.Invoke(this);
        }
    }

    protected virtual void OnTriggerEnter(Collider other)
    {

    }

    private void OnCollisionEnter(Collision other)
    {
        gameObject.SetActive(false);
        cld.enabled = false;
        OnCollide?.Invoke(other);
        ReturnMethod?.Invoke(this);
    }
}

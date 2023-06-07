using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    private Vector3 direction;
    private float damage;
    [SerializeField] private float speed;
    private float lifetime;

    // Update is called once per frame
    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;

        lifetime -= Time.deltaTime;

        if (lifetime <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void Init(Vector3 direction, float damage, float lifetime)
    {
        this.damage = damage;
        this.direction = direction;
        this.lifetime = lifetime;
    }
}

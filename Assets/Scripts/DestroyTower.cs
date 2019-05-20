using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyTower : MonoBehaviour
{
    public void BlocksCleanUp()
    {
        foreach (Transform child in transform)
        {
            float force_x = Random.Range(-1.0f, 1.0f);
            float force_y = Random.Range(0.0f, 1.0f);

            var force = new Vector2(force_x, force_y);
            force *= 10.0f;

            child.gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
            child.gameObject.GetComponent<Rigidbody2D>().AddForce(force, ForceMode2D.Impulse);
            Destroy(child.gameObject, 3.0f);
        }
    }
}
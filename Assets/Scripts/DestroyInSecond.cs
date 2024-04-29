using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyInSecond : MonoBehaviour
{
    [SerializeField] float secondToDestroy = 1f;
    void Start()
    {
        Destroy(gameObject, secondToDestroy);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitArrowAnimation : MonoBehaviour
{
    private Vector3 scaleChange = new Vector3(-0.1f, -0.1f, -0.1f);

    void Start()
    {
        Destroy(gameObject, .5f);
    }

    void Update()
    {
        gameObject.transform.localScale += scaleChange;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRotate : MonoBehaviour
{
    [SerializeField] float speed = 360f;
    [SerializeField] Vector3 angle;

    void OnEnable()
    {
        StartCoroutine(nameof(RotateCoroutine));
    }

    IEnumerator RotateCoroutine()
    {
        while (true)
        {
            transform.Rotate(speed * angle * Time.deltaTime);

            yield return null;
        }
    }
}

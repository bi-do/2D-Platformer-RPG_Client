using System.Collections;
using UnityEngine;

public class Effect : MonoBehaviour
{
    void OnEnable()
    {
        StartCoroutine(DestroyRoutine());
    }

    IEnumerator DestroyRoutine()
    {

        yield return new WaitForSeconds(3f);

        Destroy(this.gameObject);
    }
}

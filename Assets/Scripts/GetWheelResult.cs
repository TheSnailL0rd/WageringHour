using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GetWheelResult : MonoBehaviour
{
    public int result;

    private void OnTriggerEnter(Collider other)
    {
        if (int.TryParse(other.gameObject.name, out int intResult))
            result = intResult;
    }
}

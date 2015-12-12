using UnityEngine;
using System.Collections;

public class Wait : MonoBehaviour
{

	public static IEnumerator WaitTime (float timeToWait)
    {
        yield return new WaitForSeconds(timeToWait);

	}
	
	void Update () {
	
	}
}

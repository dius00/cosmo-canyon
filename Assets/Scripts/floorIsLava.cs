using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class floorIsLava : MonoBehaviour
{
 		void OnCollisionEnter(Collision collision)
		{
        	  Destroy(collision.collider.gameObject);
		}
}

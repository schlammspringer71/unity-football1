using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class Coin : MonoBehaviour {
	
	public float rotationSpeed = 60;	// degrees per second
	
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
		transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime,Space.World);	
	}
	
}

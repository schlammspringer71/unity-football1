using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]  							
public class ThirdPersonController : MonoBehaviour {
	
	public float rotationSpeed = 60.0f;										
	public float walkingSpeed = 80.0f;										
	public float jumpSpeed = 8.0f;
	public float mouseSensitivity = 30;
	public float airSpeed 	= 5f;
	public float gravity = 9.81f;

	 public Camera cam1;
	 public Camera cam2;

	Vector3 velocity = Vector3.zero;
	
	CharacterController characterController;

	public Rigidbody rb;

	// Use this for initialization
	void Start () {
		 characterController = this.GetComponent<CharacterController>();
		//rb = this.GetComponent<Rigidbody>();


		//cam1.enabled = true;
		//cam2.enabled = true;

		Debug.Log("displays connected: " + Display.displays.Length);
		// Display.displays[0] is the primary, default display and is always ON, so start at index 1.
		// Check if additional displays are available and activate each.

		//Display.displays[0].Activate();
		//Display.displays[1].Activate();

		

	}
	
	// Update is called once per frame
	void Update () {


		if (Input.GetKeyDown(KeyCode.R))
		{
			Application.LoadLevel(0);
		}

		if (Input.GetKeyDown(KeyCode.N)) {
			Display.displays[0].Activate();


		}

		if (Input.GetKeyDown(KeyCode.M))
		{
			Display.displays[1].Activate();


		}





		float forward = Input.GetAxis("Vertical");
		float strafe = Input.GetAxis("Horizontal");  //  Input.GetAxis("Strafe");						
		float rotate = Input.GetAxis("Mouse X") * mouseSensitivity;

		Vector3 airVelocity= Vector3.zero;

	
	
	
		if(characterController.isGrounded)
		{
			velocity = this.transform.right * walkingSpeed * strafe + 
				this.transform.forward * walkingSpeed * forward;				
			
			if(Input.GetButton ("Jump"))
			{
				velocity.y = jumpSpeed;
				
			}		
		}
		else // Anpassung der Bewegung in der Luft
		{
			airVelocity = forward * airSpeed * this.transform.forward + strafe * airSpeed * this.transform.right;
		}		
		
		// Gravitation 
		velocity.y-=gravity * Time.deltaTime;
						
 		characterController.Move((velocity + airVelocity)  *  Time.deltaTime);
		rb.AddForce((velocity + airVelocity) * Time.deltaTime, ForceMode.Force);
		transform.Rotate(Vector3.up * rotate  * Time.deltaTime);
	}



}

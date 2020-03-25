using UnityEngine;
using System.Collections;

public class ThirdPersonCameraController : MonoBehaviour {
	
	public Transform target;

	public float sensitivity = 60;
	
	public float minXAngle = -360;
	public float maxXAngle = 360;
	float xAngle;
	float yAngle;
	
	public float minDistance = 3;
	public float maxDistance = 15;
	
	float currentDistance = 10;
	float targetDistance;
	public float correctionSpeed = 20;
	public float zoomSpeed = 20;

	bool isHoming = true;
	public float homingSpeed = 5f;
	
	void Start()
	{
		if(target == null)	// Deaktiviere Skrip, wenn kein Ziel gesetzt ist
		{
			this.enabled = false;
		}
		else 
		{
			// Orientierung des Ziels speichern
			xAngle = target.eulerAngles.x;
			yAngle = target.eulerAngles.y;		
		}
		// Zieldistanz setzen;
		targetDistance = currentDistance;
	}
	
	void LateUpdate()
	{
		// Eingaben abfragen
		float mouseX = Input.GetAxis("Mouse X") * sensitivity;
		float mouseY = Input.GetAxis("Mouse Y") * sensitivity;
		float scrollWheel = Input.GetAxis("Mouse ScrollWheel");
		
		float forward = Input.GetAxis("Vertical");
		float strafe = 1; // Input.GetAxis("Strafe");	
		
		// Homing aktivieren, wenn Bewegungseingaben vorliegen
		if(forward != 0 || strafe != 0) 
		{
			isHoming = true;
			//Debug.Log ("Homing Start");
		}
		else if(isHoming) 
		{
			isHoming = false;
			//Debug.Log ("Homing End");
		}
		
		// letzten Neigungswinkel speichern
		float lastXAngle = xAngle;
		
		// bei linker Maustaste Drehung in 2 Achsen und Homing beenden
		if(Input.GetMouseButton(0))	
		{
			isHoming = false;
			xAngle += mouseY * Time.deltaTime * sensitivity;
			yAngle += mouseX * Time.deltaTime * sensitivity;			
		}
		// bei rechter Maustaste nur Neigung ändern
		if(Input.GetMouseButton(1))	
		{
			xAngle += mouseY * Time.deltaTime * sensitivity;
		}
		
		// Kamera sachte hinter das Ziel bewegen (Homing)
		if(isHoming )
		{
			// Berechung des neuen Drehwinkels mittels linearer Interpolation
			yAngle = Mathf.LerpAngle( yAngle,target.eulerAngles.y,homingSpeed * Time.deltaTime);
			// Homing beenden, wenn die Winkelabweichungen eine Schwelle unterschreitet
			if(Mathf.Abs(yAngle - target.eulerAngles.y)  < 0.1f ) 
			{
				isHoming = false; 
				yAngle = target.eulerAngles.y;
				//Debug.Log ("Homing End");
			}
		}
		
		// Kollision der Kamera mit dem Boden prüfen (keine Durchdringung möglich)
		float distanceCorrection = 0;
		float cameraRadius = 0.5f; // Radius der Kugel um die Kamera
		
		// Kollisionsabfrage mittels SphereCast gegen den Layer "Terrain"
		int layerMask =1 << LayerMask.NameToLayer("Terrain");
		RaycastHit hit = new RaycastHit();
		
		// Richtung des Casts aus RDifferenz zwischen Kamera- und Zielposition
		Vector3 rayDirection = this.transform.position -target.position ;
		if(Physics.SphereCast(target.position, cameraRadius,rayDirection, out hit,targetDistance,layerMask))
		{				
			// Distanz der Kamera korrigieren, so dass diese am Boden entlang gleitet
			distanceCorrection = -targetDistance + hit.distance;
			//Debug.Log ("Correcting Distance: " + distanceCorrection);
		}
		
		// Aktualisierung der Soll-Kameraentfernung (Zoom per Scrollrad) 
		// !!!Änderung im Vergleich zur Druckversion: Framerateabhängigkeit des Mausradscrollings entfernt
		//		keine Anpassung der Mouse ScrollWheel Sensitivity im InputManager
		// alt:	targetDistance = Mathf.Clamp(Mathf.SmoothStep(targetDistance, targetDistance - scrollWheel, Time.deltaTime * zoomSpeed), minDistance,maxDistance);
		targetDistance = Mathf.Clamp(targetDistance - (scrollWheel * zoomSpeed* Time.deltaTime ), minDistance,maxDistance);
		// Kollisionskorrigierte Distanz
		currentDistance = Mathf.Clamp(targetDistance+distanceCorrection,minDistance, maxDistance);

		// Neigungswinkel n den Bereich von -360° bis +360° abbilden
		xAngle = ClampAngle(xAngle,minXAngle,maxXAngle);
		// letzten Neiguingswinkel wiederherstellen, wenn die minimal-Distanz erreicht ist, eine Distanzkorrektur vorliegt und außerdem weiter geneigt werden soll
	
		if(currentDistance == minDistance && distanceCorrection != 0 && (mouseY)<0) 
		{
			xAngle = lastXAngle ;
		}
		
		
		// Rotation und POsition setzen
		this.transform.rotation =  Quaternion.Euler(xAngle,yAngle,0);
		this.transform.position = target.position + 
			this.transform.TransformDirection(new Vector3(0,0,-currentDistance));
	}
	
	static float ClampAngle ( float angle,  float  min , float max) 
	{
		if (angle < -360)
			angle += 360;
		if (angle > 360)
			angle -= 360;
		return Mathf.Clamp (angle, min, max);
	}
}

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameManager : MonoBehaviour {
	
	int coinsTotal;
	int coinsCollected = 0;
	Player player;
	
	
	public float time = 10;
	float timeLeft;
	int score = 0;
	
	
	
	public Text scoreText;
	public Text messageText;
	public Text timeText;
	
	
	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
		
		//vor v4.3.1: coinsTotal = FindObjectsOfType(typeof(Coin)).Length;
		coinsTotal = FindObjectsOfType<Coin>().Length;
		coinsCollected = 0;
		score = 0;
		
		if(scoreText != null) 
			scoreText.text = coinsCollected.ToString() + "/" + coinsTotal.ToString();
		
		timeLeft = time;
		// Zeitzähler starten
		// InvokeRepeating("UpdateTime",0,1);
		if(timeText != null) 
			timeText.text = timeLeft.ToString();
		
		messageText.enabled = false;	
	}
	
	
	void CoinCollected(Coin coin)
	{
		//	coin.audio.Play();
		//Destroy( coin.gameObject, 2);
		//coin.gameObject.setActive(false);
		
		// coin.renderer.enabled = false;
		coinsCollected++;
		if(scoreText != null) 
			scoreText.text = coinsCollected.ToString() + "/" + coinsTotal.ToString();
		
		// Wenn alle Münzen gesammelt wurde, Nachricht über Zeit anzeigen (Coroutines: parallele Handlungsstränge, nicht mit Threads verwechseln)
		if(coinsCollected == coinsTotal)
		{
			StartCoroutine(ShowTimedMessage("Bravo ! Du hast alle Münzen eingesammelt \n" +
				"Jetzt aber schnell zum Ziel",3));				
		}
	}
	
	// Coroutine: Nachricht für eine gewisse Zeit einblenden einblenden
	IEnumerator ShowTimedMessage(string message, int time)
	{
		if(messageText != null)
		{			
			messageText.color = Color.yellow;
			messageText.enabled = true;
			messageText.text = "Bravo ! Du hast alle Münzen eingesammelt \n" +
				"Jetzt aber schnell zum Ziel" ;
						
			yield return new WaitForSeconds(time); // warten
			messageText.enabled = false;
		}
	}
	
	// NOrmale Nachricht einblenden
	void ShowMessage(string message, Color color)
	{
		
		StopCoroutine("ShowTimedMessage");	// evtl. laufende Coroutine abbrechen (wegen Deaktivierung am Ende)
		if(messageText != null)
		{		
			messageText.color = color;
			messageText.enabled = true;
			messageText.text = message ;
		}	
	}
	
	void FinishReached()
	{
	
		score = (int)(timeLeft+coinsCollected);
		ShowMessage("Geschafft\n" +
				"Du hast " + coinsCollected.ToString() + " Münzen gesammelt\n" +
				"Du hast " + timeLeft.ToString() + " Sekunden Zeit\n" + score.ToString() + ((score != 1) ? " Punkte" : " Punkt"), Color.green);
		StopGame();
	}
	
	void StopGame()
	{	
		CancelInvoke("UpdateTime");
		player.Finished(score);
	}
	

	
	// Zeitaktualisieren
	void UpdateTime () {
	
		//Debug.Log ("TimeUpdate");
		if(timeLeft > 0)
			timeLeft--;
		else // Zeit ist abgelaufen
		{				
			ShowMessage ("Die Zeit ist abgelaufen.\n Du hast verloren.",Color.red);
			StopGame();			
		}
		if(timeText != null) 
			timeText.text = timeLeft.ToString();
		
	}
	
	
	
	// Alternative zum Sendmessage > Singleton
	//	nutzbare Funktionen sollten public sein
	//	Bsp: GameManager.Instance.StopGame()
//	private static GameManager instance = null;
//    public static GameManager Instance
//    {
//        get
//        {
//            if (null == (object)instance)
//            {	
//                instance = FindObjectOfType(typeof(GameManager)) as GameManager;
//				
//                if (null == (object)instance)
//                {
//                    var go = new GameObject("GameManager");
//                    instance = go.AddComponent<GameManager>();                    
//                }
//            }
//            return instance;
//        }
//    }
		
}

using UnityEngine;
using System.Collections;

public class GestionBouton : MonoBehaviour {
	public string MessageToSend;

	// Update is called once per frame
	void Update () {
	
	}
	void OnMouseDown() {
		GameObject.Find("GameManager").SendMessage(MessageToSend);
	}
}

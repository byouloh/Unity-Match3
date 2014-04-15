using UnityEngine;
using System.Collections;

public class GUIManager : MonoBehaviour {
	int Difficulte ;
	bool infini ;

	void Start () {
		Application.targetFrameRate =60;
		float Size =(((float)Screen.height/(float)Screen.width)*4.555f);
		GameObject.Find("Main Camera").GetComponent<Camera>().orthographicSize =Size;
		GameObject.Find("GUICAMERA").GetComponent<Camera>().orthographicSize =Size;

	}


	// Gestion mode
	void Fini(){
		infini =false ;
		PassageDif();
	}
	void Infini(){
		infini =true ;
		PassageDif();
	}
	void PassageDif(){
		GameObject.Find("Mode").transform.localPosition = new Vector3( -10f,0,0);
		GameObject.Find("Ingame").transform.localPosition = new Vector3( -10f,0,0);
		GameObject.Find("Difficulte").transform.localPosition = new Vector3( 0f,0,0);
	}

	// gestion dificulté
	void VE(){
		Difficulte =2 ;
		PassageEnJeux();
	}
	void E(){
		Difficulte =3 ;
		PassageEnJeux();
	}
	void M(){
		Difficulte =4 ;
		PassageEnJeux();
	}
	void H(){
		Difficulte =5 ;
		PassageEnJeux();
	}
	void VH(){
		Difficulte =6 ;
		PassageEnJeux();
	}
	void PassageEnJeux(){
		GameObject.Find("Mode").transform.localPosition = new Vector3( -10f,0,0);
		GameObject.Find("Difficulte").transform.localPosition = new Vector3( -10f,0,0);
		GameObject.Find("Ingame").transform.localPosition = new Vector3( 0f,0,0);
		gameObject.GetComponent<GAME>().CreateLevel(Difficulte,infini);

	}


	// gestion in game
	void Reset(){
		gameObject.GetComponent<GAME>().DestroyLevel();
		gameObject.GetComponent<GAME>().CreateLevel(Difficulte,infini);
	}
	void Menu(){
		GameObject.Find("Ingame").transform.localPosition = new Vector3( -10f,0,0);
		GameObject.Find("Difficulte").transform.localPosition = new Vector3( -10f,0,0);
		GameObject.Find("Mode").transform.localPosition = new Vector3( 0f,0,0);
		gameObject.GetComponent<GAME>().DestroyLevel();
	}
	void CheckUI(){
		gameObject.GetComponent<GAME>().Check();

	}



}

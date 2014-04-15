using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GAME : MonoBehaviour {

	public Sprite Sprite1 ;
	public Sprite Sprite2 ;
	public Sprite Sprite3 ;
	public Sprite Sprite4 ;
	public Sprite Sprite5 ;
	public Sprite Sprite6 ;
	public Sprite SpriteLock ;
	public Sprite BG ;
	public GameObject Effect1 ;
	public GameObject Effect2 ;
	public GameObject Effect3 ;
	public GameObject Effect4 ;
	public GameObject Effect5 ;
	public GameObject Effect6 ;

	public int TailleGrille;
	public int PastilleColors;
	public bool ModeInfini ;
	public bool ChuteHorizontal;
	public bool AutoriserDeplacementInutil;
	public bool AfficherPossibiliter;


	public float SpeedMove;
	public float SpeedFall;
	public float SpeedDestroy;


	public bool Cadenas ;
	public int CadenasChanceSur100;




	int[,] TableauItem ;
	GameObject GameGrid ;
	GameObject Target;
	int DernierDeplacement;// 1= gauche 2= droide 3= haut 4 = bas
	Vector2 PositionStart;
	Vector2 PositionFinish;
	bool DeplacementDemander;
	bool DeplacementAutoriser= true ;



	public void CreateLevel(int Dificulte, bool end){
		DeplacementAutoriser =false;
		ModeInfini=end;
		PastilleColors=Dificulte;
		PastilleColors= PastilleColors+1;
		GameGrid = new GameObject();
		GameGrid.name ="GameGrid["+TailleGrille.ToString()+","+TailleGrille.ToString()+"]";
		GameGrid.transform.position = new Vector3 (0,0,0);
		StartCoroutine(GENERATEBACKGROUND());

	}

	public void DestroyLevel(){
		ModeInfini=false;
		Destroy(GameGrid);

	}
	IEnumerator GENERATEBACKGROUND(){
		int x ;
		int y ;

		for (y = 0; y < TailleGrille ;y++){
			for (x = 0; x < TailleGrille ;x++){
				var Case = new GameObject();
				Case.transform.parent = GameGrid.transform ;
				Case.name ="[BackGroung]"+"("+x.ToString()+","+y.ToString()+")";
				Case.transform.position = new Vector3 (x,y,2);
				Case.transform.localScale = new Vector3 (0.4f,0.4f,0);
				Case.AddComponent<SpriteRenderer>().sprite =BG ;
				Case.GetComponent<SpriteRenderer>().color =new Color (0.4f,0.4f,0.4f) ;
				iTween.ScaleFrom(Case,new Vector3 (0f,0f,0f), 0.3f);
				yield return new WaitForSeconds(0.001f);
				if (x == 0 && y ==1){
					StartCoroutine(GENERATEMAP());
				}
						
			}			
		}

	}

	void Update () {

		if ( DeplacementAutoriser){
			if(Input.GetMouseButtonDown(0)){
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit hit;
				if(Physics.Raycast(ray, out hit, 20f)){
					if ( hit.collider.gameObject.tag == "ITEM"){
						Target =hit.collider.gameObject;
						PositionStart = new Vector2(Input.mousePosition.x,Input.mousePosition.y);
						DeplacementDemander=true ;
					}	
				}
			}
			if(Input.GetMouseButtonUp(0)){
				if (DeplacementDemander  ){
					CleanColoredPossibility();
					DernierDeplacement=0;
					PositionFinish = new Vector2(Input.mousePosition.x,Input.mousePosition.y);
					if ( Vector2.Distance(PositionStart,PositionFinish) >= 20 ){
						if(Mathf.Abs(PositionFinish.x-PositionStart.x) >Mathf.Abs(PositionFinish.y-PositionStart.y)){ // si deplacement horizontal
							if (PositionStart.x -PositionFinish.x < 0){
								Debug.Log( "DROITE");
								DernierDeplacement =1;
								DeplacementDroite();
							}
							else{
								Debug.Log( "GAUCHE");
								DernierDeplacement =2;
								DeplacementGauche();
							}

						}
						else { // deplacement verticale
							if (PositionStart.y -PositionFinish.y < 0){
								Debug.Log( "HAUT");
								DernierDeplacement =3;
								DeplacementHaut();
							}
							else{
								Debug.Log( "BAS");
								DernierDeplacement =4;
								DeplacementBas();
							}
							
						}

					}
					DeplacementDemander=false ;
					DeplacementAutoriser= false;
					StartCoroutine(AttenteApresDeplacement(SpeedMove));


				}
			}
		}

		
	}
	IEnumerator GENERATEMAP(){

		int x ;
		int y ;

		TableauItem =new int[TailleGrille,TailleGrille]; // initialisation tableau

		for (x = 0; x < TailleGrille ;x++){
			for (y = 0; y < TailleGrille ;y++){
				TableauItem[x,y]=(int)UnityEngine.Random.Range(1, PastilleColors); // assigne valeur aléatoire pour les pastilles	
			}			
		}
		while( !TableauCorrect() );

		for (y = 0; y < TailleGrille ;y++){
			for (x = 0; x < TailleGrille ;x++){
				CreateItem(x,y,TableauItem[x,y]);
				yield return new WaitForSeconds(0.001f);
			}			
		}
		DeplacementAutoriser= true;
	}

	GameObject CreateItem(int x,int y, int item){

		var Case = new GameObject();
		Case.transform.parent = GameGrid.transform ;
		Case.name ="["+x.ToString()+","+y.ToString()+"]";
		Case.transform.position = new Vector3 (x,y,0);
		Case.transform.tag ="ITEM";
		switch (item)
		{
		case 6:
			Case.AddComponent<SpriteRenderer>().sprite =Sprite6 ;

			break;
		case 5:
			Case.AddComponent<SpriteRenderer>().sprite =Sprite5 ;
			break;
		case 4:
			Case.AddComponent<SpriteRenderer>().sprite =Sprite4 ;
			break;
		case 3:
			Case.AddComponent<SpriteRenderer>().sprite =Sprite3 ;
			break;
		case 2:
			Case.AddComponent<SpriteRenderer>().sprite =Sprite2 ;
			break;
		case 1:
			Case.AddComponent<SpriteRenderer>().sprite =Sprite1 ;
			break;

		}
		Case.AddComponent<BoxCollider>();
		if (Cadenas && UnityEngine.Random.Range(0, 100) <= CadenasChanceSur100 ){
			Case.transform.tag ="LOCK";
			var Lock = new GameObject();
			Lock.transform.parent = Case.transform ;
			Lock.transform.localPosition = new Vector3 (0,0,-1);
			Lock.name ="[LockItem]";
			Lock.AddComponent<SpriteRenderer>().sprite =SpriteLock ;
			Lock.GetComponent<SpriteRenderer>().color = new Color ( 0f,0f,0f,0.7f);
		}

		return Case;
	}

	
	bool TableauCorrect(){
		Debug.Log ("VERIFICATION TABLEAU");
		int x ;
		int y ;
		int ChangementEffectuer = 0 ;
		// vérification en ligne
		for (y = 0; y < TailleGrille ;y++){
			for (x = 0; x < TailleGrille-2 ;x++){
				if (TableauItem[x,y] == TableauItem[x+1,y] && TableauItem[x+1,y] == TableauItem[x+2,y]){
					TableauItem[x+1,y]=(int)UnityEngine.Random.Range(1, PastilleColors);
					ChangementEffectuer =1;
					Debug.Log ("COMBO H ligne "+y.ToString()+" aux colones "+x.ToString()+" a "+(x+3).ToString());
				}
			}			
		}

		// vérification en colone
		for (x = 0; x < TailleGrille ;x++){
			for (y = 0; y < TailleGrille-2 ;y++){
				if (TableauItem[x,y] == TableauItem[x,y+1] && TableauItem[x,y+1] == TableauItem[x,y+2]){
					TableauItem[x,y+1]=(int)UnityEngine.Random.Range(1, PastilleColors);
					ChangementEffectuer =1;
					Debug.Log ("COMBO V colone "+x.ToString()+" aux ligne "+y.ToString()+" a "+(y+3).ToString());

				}
			}			
		}
		if (ChangementEffectuer == 1 ){
			Debug.Log ("ERREUR TABLEAU ,VERIFICATION EN COURS");
			return false;
		}
		else {
			Debug.Log ("TABLEAU OK AFFICHAGE EN COURS");
			return true;
		}
	}


	void DeplacementDroite(){
		int x;
		int y;
		Int32.TryParse(Target.name.Substring(1, 1), out x);	
		Int32.TryParse(Target.name.Substring(3, 1), out y);	
		
		if (x < TailleGrille-1  ){//&& GameObject.Find("["+(x+1).ToString()+","+y.ToString()+"]").tag != "LOCK" ){
			// recupére l'objet de destination
			GameObject Target2 ;
			Target2 = GameObject.Find("["+(x+1).ToString()+","+y.ToString()+"]"); 
			
			// echange les positions
			iTween.MoveTo(Target,new Vector3(x+1,y,0),SpeedMove);
			if ( Target2 != null )
				iTween.MoveTo(Target2,new Vector3(x,y,0),SpeedMove);
			
			
			//echange les noms
			Target.name="["+(x+1).ToString()+","+y.ToString()+"]";
			if ( Target2 != null )
			Target2.name="["+x.ToString()+","+y.ToString()+"]";
			
			int start = TableauItem[x,y];
			int fin = TableauItem[x+1,y];
			TableauItem[x,y] = fin ;
			TableauItem[x+1,y] = start ;
		}
		else{
			DernierDeplacement=0;
		}
	}
	void DeplacementGauche(){
		int x;
		int y;
		Int32.TryParse(Target.name.Substring(1, 1), out x);	
		Int32.TryParse(Target.name.Substring(3, 1), out y);	
		
		if (x >0 ){//&& GameObject.Find("["+(x-1).ToString()+","+y.ToString()+"]").tag != "LOCK"){
			// recupére l'objet de destination
			GameObject Target2 ;
			Target2 = GameObject.Find("["+(x-1).ToString()+","+y.ToString()+"]"); 
			
			// echange les positions
			iTween.MoveTo(Target,new Vector3(x-1,y,0),SpeedMove);
			if ( Target2 != null )
				iTween.MoveTo(Target2,new Vector3(x,y,0),SpeedMove);
			
			
			//echange les noms
			Target.name="["+(x-1).ToString()+","+y.ToString()+"]";
			if ( Target2 != null )
			Target2.name="["+x.ToString()+","+y.ToString()+"]";
			
			int start = TableauItem[x,y];
			int fin = TableauItem[x-1,y];
			TableauItem[x,y] = fin ;
			TableauItem[x-1,y] = start ;
		}
		else{
			DernierDeplacement=0;
		}
	}
	void DeplacementBas(){
		int x;
		int y;
		Int32.TryParse(Target.name.Substring(1, 1), out x);	
		Int32.TryParse(Target.name.Substring(3, 1), out y);	
		
		if (y > 0 ){//&& GameObject.Find("["+x.ToString()+","+(y-1).ToString()+"]").tag != "LOCK"){
			// recupére l'objet de destination
			GameObject Target2 ;
			Target2 = GameObject.Find("["+x.ToString()+","+(y-1).ToString()+"]"); 
			
			// echange les positions
			iTween.MoveTo(Target,new Vector3(x,y-1,0),SpeedMove);
			if ( Target2 != null )
				iTween.MoveTo(Target2,new Vector3(x,y,0),SpeedMove);
			
			
			//echange les noms
			Target.name="["+x.ToString()+","+(y-1).ToString()+"]";
			if ( Target2 != null )
			Target2.name="["+x.ToString()+","+y.ToString()+"]";
			
			int start = TableauItem[x,y];
			int fin = TableauItem[x,y-1];
			TableauItem[x,y] = fin ;
			TableauItem[x,y-1] = start ;
		}
		else{
			DernierDeplacement=0;
		}
	}
	void DeplacementHaut(){
		int x;
		int y;
		Int32.TryParse(Target.name.Substring(1, 1), out x);	
		Int32.TryParse(Target.name.Substring(3, 1), out y);	
		
		if (y < TailleGrille-1 ){//&& GameObject.Find("["+x.ToString()+","+(y+1).ToString()+"]").tag != "LOCK"){
			// recupére l'objet de destination
			if(TableauItem[x,y+1] != 0){
				// recupére l'objet de destination
				GameObject Target2 ;
				Target2 = GameObject.Find("["+x.ToString()+","+(y+1).ToString()+"]"); 
				// echange les positions
				iTween.MoveTo(Target,new Vector3(x,y+1,0),SpeedMove);
				iTween.MoveTo(Target2,new Vector3(x,y,0),SpeedMove);
				
				
				//echange les noms
				Target.name="["+x.ToString()+","+(y+1).ToString()+"]";
				Target2.name="["+x.ToString()+","+y.ToString()+"]";
				
				int start = TableauItem[x,y];
				int fin = TableauItem[x,y+1];
				TableauItem[x,y] = fin ;
				TableauItem[x,y+1] = start ;
			}
		}
		else{
			DernierDeplacement=0;
		}
	}
	void VerificationApresMouvement(){
		Debug.Log ("VERIFICATION TABLEAU");
		int x ;
		int y ;
		int[,] deleteARRAYS = new int[TailleGrille,TailleGrille];
		bool Destruction = false;

		// vérification en ligne
		for (y = 0; y < TailleGrille ;y++){
			for (x = 0; x < TailleGrille-2 ;x++){
				if (TableauItem[x,y] == TableauItem[x+1,y] && TableauItem[x+1,y] == TableauItem[x+2,y]&& TableauItem[x,y] != 0){
					Destruction =true ;
					deleteARRAYS[x,y]=1;
					deleteARRAYS[x+1,y]=1;
					deleteARRAYS[x+2,y]=1;
				}
			}			
		}
		
		// vérification en colone
		for (x = 0; x < TailleGrille ;x++){
			for (y = 0; y < TailleGrille-2 ;y++){
				if (TableauItem[x,y] == TableauItem[x,y+1] && TableauItem[x,y+1] == TableauItem[x,y+2] && TableauItem[x,y] != 0){
					Destruction=true;
					deleteARRAYS[x,y]=1;
					deleteARRAYS[x,y+1]=1;
					deleteARRAYS[x,y+2]=1;


				}
			}			
		}
		// suppression des case

		if (Destruction){
			int [,]SpecialDestructX = new int[TailleGrille,7];
			int [,]SpecialDestructY = new int[TailleGrille,7];
			DernierDeplacement=0;
			for (x = 0; x < TailleGrille ;x++){
				for (y = 0; y < TailleGrille ;y++){
					if (deleteARRAYS[x,y]== 1 ){
						SpecialDestructX [x,TableauItem[x,y]]+=1;
						SpecialDestructY [y,TableauItem[x,y]]+=1;

						if (GameObject.Find("["+x.ToString()+","+y.ToString()+"]").tag == "ITEM" ){
							TableauItem[x,y]=0;
							GameObject GameobjectADetruire =GameObject.Find("["+x.ToString()+","+y.ToString()+"]");
							if (GameobjectADetruire != null ){
								GameobjectADetruire.name = "DESTROY["+x.ToString()+"-"+y.ToString()+"]";
								StartCoroutine(DestroyCases(GameobjectADetruire));
							}
						}
						else if (GameObject.Find("["+x.ToString()+","+y.ToString()+"]").tag == "LOCK" ){
							GameObject GameobjectADetruire =GameObject.Find("["+x.ToString()+","+y.ToString()+"]/[LockItem]");
							if (GameobjectADetruire != null ){
								GameObject.Find("["+x.ToString()+","+y.ToString()+"]").tag = "ITEM";
								GameobjectADetruire.name = "DESTROY[LockItem]";
								StartCoroutine(DestroyCases(GameobjectADetruire));
							}
						}

					}
				}			
			}
			bool SpecialDestruct =false ;
			for (x = 0; x < TailleGrille ;x++){
				for (y = 0; y < 7 ;y++){
					if (SpecialDestructX [x,y] >= 4){
						SpecialDestruct= true;
						StartCoroutine(DestroyLineY(x,y));
					}
					if (SpecialDestructY [x,y] >= 4){
						SpecialDestruct=true;
						StartCoroutine(DestroyLineX(x,y));
					}
				}
			}

			if (SpecialDestruct ){
				StartCoroutine(AttenteDeDestruction(SpeedDestroy));
			}
			else{
				StartCoroutine(AttenteDeDestruction(SpeedDestroy));
			}

		}
		else if (!AutoriserDeplacementInutil && DernierDeplacement !=0){
			switch (DernierDeplacement)
			{
			case 1:
				DeplacementGauche();
				break;
			case 2:
				DeplacementDroite();
				break;
			case 3:
				DeplacementBas();
				break;
			case 4:
				DeplacementHaut();
				break;
			case 0:

				break;
			}
			DernierDeplacement=0;
			StartCoroutine(AttenteApresDeplacement(SpeedMove));

		}
		else{
			ChuteDesCases();
		}

	}
	IEnumerator DestroyLineX(int x,int Couleurs){
		GameObject Effect ;
		if (Couleurs == 1 ){
			 Effect = (GameObject)Instantiate(Effect1, new Vector3( -1f , x ,-1.6f), Quaternion.identity);
		}
		else if (Couleurs == 2 ){
			 Effect = (GameObject)Instantiate(Effect2, new Vector3( -1f , x ,-1.6f), Quaternion.identity);
		}
		else if (Couleurs == 3 ){
			 Effect = (GameObject)Instantiate(Effect3, new Vector3( -1f , x ,-1.6f), Quaternion.identity);
		}
		else if (Couleurs == 4 ){
			 Effect = (GameObject)Instantiate(Effect4, new Vector3( -1f , x ,-1.6f), Quaternion.identity);
		}
		else if (Couleurs == 5 ){
			 Effect = (GameObject)Instantiate(Effect5, new Vector3( -1f , x ,-1.6f), Quaternion.identity);
		}
		else if (Couleurs == 6 ){
			 Effect = (GameObject)Instantiate(Effect6, new Vector3( -1f , x ,-1.6f), Quaternion.identity);
		}
		else{
			Effect = null ;
		}

		if ( Effect == null ){
		}
		else {
			iTween.MoveTo(Effect,new Vector3 (9.5f,x,-1.6f), 1.5f);
			yield return new WaitForSeconds(1.5f);
			Destroy (Effect);
		}
		
		
	}
	IEnumerator DestroyLineY(int x,int Couleurs){
		GameObject Effect ;
		if (Couleurs == 1 ){
			Effect = (GameObject)Instantiate(Effect1, new Vector3( x , -1f ,-1.6f), Quaternion.identity);
		}
		else if (Couleurs == 2 ){
			Effect = (GameObject)Instantiate(Effect2, new Vector3( x , -1f ,-1.6f), Quaternion.identity);
		}
		else if (Couleurs == 3 ){
			Effect = (GameObject)Instantiate(Effect3, new Vector3( x , -1f ,-1.6f), Quaternion.identity);
		}
		else if (Couleurs == 4 ){
			Effect = (GameObject)Instantiate(Effect4, new Vector3( x , -1f ,-1.6f), Quaternion.identity);
		}
		else if (Couleurs == 5 ){
			Effect = (GameObject)Instantiate(Effect5, new Vector3( x , -1f ,-1.6f), Quaternion.identity);
		}
		else if (Couleurs == 6 ){
			Effect = (GameObject)Instantiate(Effect6, new Vector3( x , -1f ,-1.6f), Quaternion.identity);
		}
		else{
			Effect = null ;
		}
		
		if ( Effect == null ){
		}
		else {
			iTween.MoveTo(Effect,new Vector3 (x,9.5f,-1.6f), 1.5f);
			yield return new WaitForSeconds(1.5f);
			Destroy (Effect);
		}
		
	}

	IEnumerator DestroyCases(GameObject destroyItem){
		iTween.ScaleTo(destroyItem,new Vector3 (0f,0f,0f), SpeedDestroy);
		yield return new WaitForSeconds(SpeedDestroy);
		Destroy (destroyItem);

		
	}
	IEnumerator AttenteDeDestruction ( float time ){
		yield return new WaitForSeconds(time);
		ChuteDesCases();
	}
	
	void ChuteDesCases(){
		int x ;
		int y ;
		bool chute = false;
		// vérification en ligne
		for (x = 0; x < TailleGrille ;x++){// pour chaque ligne
			int emplacementarrive=0;
			int emplacementdedepart=0;
			int etatderecherche =0;
			for (y = 0; y < TailleGrille ;y++){ // et chaque colonne case par case
				if (TableauItem[x,y] == 0  && etatderecherche == 0){ // si la case est null
					emplacementarrive=y;
					etatderecherche =1;
				}
				if (TableauItem[x,y] != 0  && etatderecherche == 1){
					etatderecherche =2;
					emplacementdedepart=y ;
				}
			}
			if(emplacementdedepart != 0){
				int yBIS;
				for(yBIS = 0; yBIS < (TailleGrille-emplacementdedepart) ; yBIS++){ // pour toute les cases du dessus

					if (TableauItem[x,emplacementdedepart+yBIS]!=0 ){
						chute =true;
						GameObject MovedDown =GameObject.Find("["+x.ToString()+","+(emplacementdedepart+yBIS).ToString()+"]");
						iTween.MoveTo(MovedDown,new Vector3(x,emplacementarrive+yBIS,0),SpeedFall);
						MovedDown.name="["+x.ToString()+","+(emplacementarrive+yBIS).ToString()+"]";
						TableauItem[x,emplacementarrive+yBIS]=TableauItem[x,emplacementdedepart+yBIS];
						TableauItem[x,emplacementdedepart+yBIS] = 0;
					}
				}
			}
		}
		if(ChuteHorizontal){
			ChuteDesCasesHorizontal(chute);
		}
		else if (chute ){
			StartCoroutine(AttenteApresDeplacement((SpeedFall+0.1f)));
		}
		else if (ModeInfini){
			RemplissageVide();
		}
		else{
			DeplacementAutoriser=true;
		}
	}
	void ChuteDesCasesHorizontal(bool ChuteVerticale){
		int x ;
		int y ;
		bool chute = false;
		// vérification en ligne
		for (y = 0; y < TailleGrille ;y++){// pour chaque ligne
			int emplacementarrive=0;
			int emplacementdedepart=0;
			int etatderecherche =0;
			for (x = 0; x < TailleGrille ;x++){ // et chaque colonne case par case
				if (TableauItem[x,y] == 0  && etatderecherche == 0){ // si la case est null
					emplacementarrive=x;
					etatderecherche =1;
				}
				if (TableauItem[x,y] != 0  && etatderecherche == 1){
					etatderecherche =2;
					emplacementdedepart=x ;
				}
			}
			if(emplacementdedepart != 0){
				int xBIS;
				for(xBIS = 0; xBIS < (TailleGrille-emplacementdedepart) ; xBIS++){ // pour toute les cases du dessus
					
					if (TableauItem[emplacementdedepart+xBIS,y]!=0 ){
						chute =true;
						GameObject MovedDown =GameObject.Find("["+(emplacementdedepart+xBIS).ToString()+","+y.ToString()+"]");
						iTween.MoveTo(MovedDown,new Vector3((emplacementarrive+xBIS),y,0),SpeedFall);
						MovedDown.name="["+(emplacementarrive+xBIS).ToString()+","+y.ToString()+"]";
						TableauItem[(emplacementarrive+xBIS),y]=TableauItem[(emplacementdedepart+xBIS),y];
						TableauItem[(emplacementdedepart+xBIS),y] = 0;
					}
				}
			}
		}
		if(chute || ChuteVerticale){
			StartCoroutine(AttenteApresDeplacement((SpeedFall+0.1f)));
		}
		else if (ModeInfini){
			RemplissageVide();
		}
		else{
			DeplacementAutoriser=true;
		}
	}


	void RemplissageVide(){
		int x ;
		int y ;
		bool RemplacementEffectuer=false;
		for (x = 0; x < TailleGrille ;x++){
			for (y = 0; y < TailleGrille ;y++){
				if (TableauItem[x,y]== 0 ){
					RemplacementEffectuer=true;

					int ItemColor=(int)UnityEngine.Random.Range(1, PastilleColors);
					TableauItem[x,y]=ItemColor;
					GameObject Newobjet = CreateItem(x,y,ItemColor);
					iTween.MoveFrom(Newobjet, new Vector3 ( x,y+TailleGrille,0),SpeedFall) ;
					
				}
			}			
		}
		if (RemplacementEffectuer){
			StartCoroutine(AttenteApresDeplacement(SpeedFall));
		}
		else{
			DeplacementAutoriser=true;
		}

	}

	IEnumerator AttenteApresDeplacement(float time){
		yield return new WaitForSeconds(time);
		VerificationApresMouvement();
	}

//	void OnGUI() {
//		if (GUI.Button(new Rect(10, 10, 100, 50), "Reset")){
//			Application.LoadLevel(0);
//		}	
//	}

	public void Check(){
		if (DeplacementAutoriser){
			Debug.Log ("Possibilité?");
			int x ;
			int y ;
			int z ;
			int[,,] MouvementUtilePossible = new int[TailleGrille,TailleGrille,5]; //0= mouvementUtile    1= gauche 2=droite 3=haut  4=bas
			for (y = 0; y < TailleGrille ;y++){
				for (x = 0; x < TailleGrille ;x++){
					for (z = 0; z < 5 ;z++){
						MouvementUtilePossible[x,y,z]=0;
					}
				}
			}


			int ValeurdeCouleurs;
			// vérification en ligne
			for (y = 0; y < TailleGrille ;y++){
				for (x = 0; x < TailleGrille-1 ;x++){
					ValeurdeCouleurs =TableauItem[x,y];
					if ( x < TailleGrille-2 && TableauItem[x,y] == TableauItem[x+2,y] && TableauItem[x,y] != 0){// verification 101
						//				010
						// verification 101
						//				010
						if (y >=1 && TableauItem[x+1,y-1]== ValeurdeCouleurs ){// verification au en dessous
							Debug.Log ( "haut sur la case "+(x+1).ToString()+" "+(y-1).ToString());
							MouvementUtilePossible[x+1,y-1,3]+=1;
							StartCoroutine(ColoredPossibility(x+1,y-1));
						}
						if (y <TailleGrille-1 && TableauItem[x+1,y+1]== ValeurdeCouleurs ){// verification au dessus
							Debug.Log ( "bas sur la case "+(x+1).ToString()+" "+(y+1).ToString());
							MouvementUtilePossible[x+1,y+1,4]+=1;
							StartCoroutine(ColoredPossibility(x+1,y+1));
						}
					}
					if (TableauItem[x,y] == TableauItem[x+1,y] && TableauItem[x,y] != 0){ // verrification 11

						if ( x >= 1){//si au moin 1 case libre  a gauche
							if(y <TailleGrille-1 && TableauItem[x-1,y+1]== ValeurdeCouleurs){
								Debug.Log ( "bas sur la case "+(x-1).ToString()+" "+(y+1).ToString());
								MouvementUtilePossible[x-1,y+1,4]+=1;
								StartCoroutine(ColoredPossibility(x-1,y+1));
							}
							if(y >=1 && TableauItem[x-1,y-1]== ValeurdeCouleurs){
								Debug.Log ( "haut sur la case "+(x-1).ToString()+" "+(y-1).ToString());
								MouvementUtilePossible[x-1,y-1,3]+=1;
								StartCoroutine(ColoredPossibility(x-1,y-1));
							}
							if ( x >=2 && TableauItem[x-2,y]== ValeurdeCouleurs){ // si 2 case libre a gauche
								Debug.Log ( "droite sur la case "+(x-2).ToString()+" "+(y).ToString());
								MouvementUtilePossible[x-2,y,2]+=1;
								StartCoroutine(ColoredPossibility(x-2,y));
							}
						}

						if (x <TailleGrille-2){// si une case libre a droite
							if(y <TailleGrille-1 && TableauItem[x+2,y+1]== ValeurdeCouleurs){
								Debug.Log ( "bas sur la case "+(x+2).ToString()+" "+(y+1).ToString());
								MouvementUtilePossible[x+2,y+1,4]+=1;
								StartCoroutine(ColoredPossibility(x+2,y+1));
							}
							if(y >=1 && TableauItem[x+2,y-1]== ValeurdeCouleurs){
								Debug.Log ( "haut sur la case "+(x+2).ToString()+" "+(y-1).ToString());
								MouvementUtilePossible[x+2,y-1,3]+=1;
								StartCoroutine(ColoredPossibility(x+2,y-1));
							}
							if (x <TailleGrille-3 && TableauItem[x+3,y]== ValeurdeCouleurs){ // si 2 case libre a droite
								Debug.Log ( "gauche sur la case "+(x+3).ToString()+" "+(y).ToString());
								MouvementUtilePossible[x+3,y,1]+=1;
								StartCoroutine(ColoredPossibility(x+3,y));
							}
						}

					}


				}			
			}
			
			// vérification en colone
			for (x = 0; x < TailleGrille ;x++){
				for (y = 0; y < TailleGrille-1 ;y++){
					ValeurdeCouleurs =TableauItem[x,y];
					if (y < TailleGrille-2 && TableauItem[x,y] == TableauItem[x,y+2 ]&& TableauItem[x,y] != 0){
						//				010
						// verification 101
						//				010
						if (x >=1 && TableauItem[x-1,y+1]== ValeurdeCouleurs ){// verification au en dessous
							Debug.Log ( "Droite sur la case "+(x-1).ToString()+" "+(y+1).ToString());
							MouvementUtilePossible[x-1,y+1,2]+=1;
							StartCoroutine(ColoredPossibility(x-1,y+1));
						}
						if (x <TailleGrille-1 && TableauItem[x+1,y+1]== ValeurdeCouleurs ){// verification au dessus
							Debug.Log ( "Gauche sur la case "+(x+1).ToString()+" "+(y+1).ToString());
							MouvementUtilePossible[x+1,y+1,1]+=1;
							StartCoroutine(ColoredPossibility(x+1,y+1));
						}
					}
					if (TableauItem[x,y] == TableauItem[x,y+1] && TableauItem[x,y] != 0){ // verrification 11

						if ( y >= 1){//si au moin 1 case libre  a en bas
							if(x <TailleGrille-1 && TableauItem[x+1,y-1]== ValeurdeCouleurs){
								Debug.Log ( "gauche sur la case "+(x+1).ToString()+" "+(y-1).ToString());
								MouvementUtilePossible[x+1,y-1,1]+=1;
								StartCoroutine(ColoredPossibility(x+1,y-1));
							}
							if(x >=1 && TableauItem[x-1,y-1]== ValeurdeCouleurs){
								Debug.Log ( "droite sur la case "+(x-1).ToString()+" "+(y-1).ToString());
								MouvementUtilePossible[x-1,y-1,2]+=1;
								StartCoroutine(ColoredPossibility(x-1,y-1));
							}
							if ( y >=2 && TableauItem[x,y-2]== ValeurdeCouleurs){ // si 2 case libre a gauche
								Debug.Log ( "haut sur la case "+x.ToString()+" "+(y-2).ToString());
								MouvementUtilePossible[x,y-2,3]+=1;
								StartCoroutine(ColoredPossibility(x,y-2));
							}
						}
						
						if (y <TailleGrille-2){// si une case libre en haut
							if(x <TailleGrille-1 && TableauItem[x+1,y+2]== ValeurdeCouleurs){
								Debug.Log ( "gauche sur la case "+(x+1).ToString()+" "+(y+2).ToString());
								MouvementUtilePossible[x+1,y+2,1]+=1;
								StartCoroutine(ColoredPossibility(x+1,y+2));
							}
							if(x >=1 && TableauItem[x-1,y+2]== ValeurdeCouleurs){
								Debug.Log ( "droite sur la case "+(x-1).ToString()+" "+(y+2).ToString());
								MouvementUtilePossible[x-1,y+2,2]+=1;
								StartCoroutine(ColoredPossibility(x-1,y+2));
							}
							if (y <TailleGrille-3 && TableauItem[x,y+3]== ValeurdeCouleurs){ // si 2 case libre a droite
								Debug.Log ( "bas sur la case "+x.ToString()+" "+(y+3).ToString());
								MouvementUtilePossible[x,y+3,4]+=1;
								StartCoroutine(ColoredPossibility(x,y+3));
							}
						}
						
					}
				}			
			}
			for (y = 0; y < TailleGrille ;y++){ // recherche de correspondance de mouvement pour detecter les mouvement qui s'entrevauche
				for (x = 0; x < TailleGrille ;x++){
					if (TableauItem[x,y] != 0 && GameObject.Find("["+x.ToString()+","+y.ToString()+"]").tag == "ITEM"){
						for (z = 1; z < 5 ;z++){
							if (MouvementUtilePossible[x,y,z] > 0){
								MouvementUtilePossible[x,y,0]+=1;
							}
						}

						if (MouvementUtilePossible[x,y,1] > 0 && MouvementUtilePossible[x-1,y,2] > 0){ // si deplacement gauche favorable et autre pastille veux un deplacement droit
							MouvementUtilePossible[x,y,0]+=10;
						} 
						if (MouvementUtilePossible[x,y,2] > 0 && MouvementUtilePossible[x+1,y,1] > 0 ){
							MouvementUtilePossible[x,y,0]+=10;
						}
						if (MouvementUtilePossible[x,y,3] > 0 && MouvementUtilePossible[x,y+1,4] > 0){
							MouvementUtilePossible[x,y,0]+=10;
						}
						if (MouvementUtilePossible[x,y,4] > 0 && MouvementUtilePossible[x,y-1,3] > 0) {
							MouvementUtilePossible[x,y,0]+=10;
						}
					
					}
				}
			}
			for (x = 0; x < TailleGrille ;x++){
				for (y = 0; y < TailleGrille ;y++){
					if (MouvementUtilePossible[x,y,0] >= 10 ){
						StartCoroutine(ColoredPossibility2(x,y,0f));
					}
					else if 
					(MouvementUtilePossible[x,y,0] > 0 ){
						StartCoroutine(ColoredPossibility2(x,y,0.7f));
					}	
				}
			}

		}
	}



	void CleanColoredPossibility(){
		int x ;
		int y ;
		for (x = 0; x < TailleGrille ;x++){
			for (y = 0; y < TailleGrille ;y++){
				GameObject.Find("[BackGroung]"+"("+x.ToString()+","+y.ToString()+")").GetComponent<SpriteRenderer>().color =new Color (0.4f,0.4f,0.4f) ;
				
			}
		}

	}

	IEnumerator ColoredPossibility (int x, int y ){
		GameObject CaseToColor = GameObject.Find("[BackGroung]"+"("+x.ToString()+","+y.ToString()+")");
	//	CaseToColor.GetComponent<SpriteRenderer>().color =new Color (10f,10f,1f) ;
		yield return new WaitForSeconds(0.1f);
		//CaseToColor.GetComponent<SpriteRenderer>().color =new Color (0.4f,0.4f,0.4f) ;
	}
	IEnumerator ColoredPossibility2(int x, int y, float couleurs ){
		GameObject CaseToColor = GameObject.Find("[BackGroung]"+"("+x.ToString()+","+y.ToString()+")");
		CaseToColor.GetComponent<SpriteRenderer>().color =new Color (couleurs,couleurs,couleurs) ;
		yield return new WaitForSeconds(0.1f);
		//CaseToColor.GetComponent<SpriteRenderer>().color =new Color (0.4f,0.4f,0.4f) ;
	}
	
}



using UnityEngine;
using System.Collections;

public class KeyboardController : MonoBehaviour {
	public UnityOSCListener mainOSCListener;
	public GameObject puppet_IIMKaragozAsHorse;
	public GameObject puppet_IIM_Bird_001;
	public GameObject puppet_Dragon;
	public GameObject puppet_IIM_Male_Duo;
	public GameObject puppet_IIM_Karagoz_Women_Dancer_001;
	public GameObject puppet_IIM_Karagoz_Male_001; 
	public GameObject puppet_IIM_Soldier_001;
	public GameObject prop_IIM_Prop_Chair_001;
	public GameObject prop_Book_UO_Shirins_Kiosk;
	public GameObject prop_IIM_Prop_High_House;
	public GameObject prop_IIM_Prop_High_House_animated;
	public GameObject prop_IIM_Prop_Pot;
	public GameObject prop_IIM_Prop_Pot_Breakable;
	public GameObject directionalLight;
	public GameObject puppet_IIM_Bird_Uni2D;
	public GameObject puppet_IIM_Musician;
	public GameObject puppet_BW_Billy_Bottle;
	public GameObject puppet_BW_Billy_Violin;
	public GameObject puppet_BW_Mrs_Martin;
	public GameObject puppet_BW_Tozer;
	public GameObject puppet_BW_Jemima;
	public GameObject puppet_BW_Mrs_Brown;


	private PuppetHelperIIM_Bird_Uni2D birdUni2DHelper;





	bool toggle1 = true; // iim bird
	bool toggle2 = false; // dragon
	bool toggle3 = false;
	bool toggle4 = false;
	bool toggle5 = false;
	bool toggle6 = false;
	bool toggle7 = false;
	bool toggle8 = false;
	bool toggle9 = false;
	bool toggle10 = false;
	bool toggle11 = false;
	bool toggle12 = true;
	bool toggle13 = false; // puppet IIM bird Uni2D 
	bool toggle14 = false; // puppet_IIM_Musician 
	bool toggle15 = false; // puppet_IIM_Karagoz_as_Horse

	bool monochromeState;

	// Use this for initialization
	void Start () {
	
	}


	public void SetPuppet_IIMKaragozAsHorse(GameObject go) {
		
		puppet_IIMKaragozAsHorse = go;
		
		
	}


	public void SetPuppet_IIMMusician(GameObject go) {

		puppet_IIM_Musician = go;

		
	}
	public void SetPuppet_IIMBirdUni(GameObject go) {
		
		puppet_IIM_Bird_Uni2D = go;




		//birdUni2DHelper = (PuppetHelperIIM_Bird_Uni2D)FindObjectOfType(typeof(PuppetHelperIIM_Bird_Uni2D));
		
	}


	// puppet_IIM_Bird_001
	public void SetPuppet_IIM_Bird_001(GameObject go) {
		
		puppet_IIM_Bird_001 = go;
		
	}

	// puppet_IIM_Bird_001
	public void SetPuppet_Dragon(GameObject go) {
		
		puppet_Dragon = go;
		
	}
	// Update is called once per frame
	void Update () {
	
		if (Input.GetKeyUp("1")){

			if (puppet_IIM_Bird_001) {
				if (toggle1==false){

					puppet_IIM_Bird_001.EnableChildren(true);
					//puppet_IIM_Bird_001.SetActive(true); 
					toggle1 = true;
				} else {
					puppet_IIM_Bird_001.EnableChildren(false);
					//puppet_IIM_Bird_001.SetActive(false); 
					toggle1=false;
				}
			}
		}
		if (puppet_Dragon) {
		if (Input.GetKeyUp("2")){
				if (toggle2==false){
					puppet_Dragon.SetActive(true); toggle2 = true;
				} else {
					puppet_Dragon.SetActive(false); toggle2=false;
				}
			}
		}
		if (Input.GetKeyUp("3")){
			if (toggle3==false){
				puppet_IIM_Male_Duo.SetActive(true); toggle3 = true;
			} else {
				puppet_IIM_Male_Duo.SetActive(false); toggle3=false;
			}
		}

		if (Input.GetKeyUp("4")){
			if (toggle4==false){
				puppet_IIM_Karagoz_Women_Dancer_001.SetActive(true); toggle4 = true;
			} else {
				puppet_IIM_Karagoz_Women_Dancer_001.SetActive(false); toggle4=false;
			}
		}

		if (Input.GetKeyUp("5")){
			if (toggle5==false){
				puppet_IIM_Karagoz_Male_001.SetActive(true); toggle5 = true;
			} else {
				puppet_IIM_Karagoz_Male_001.SetActive(false); toggle5=false;
			}
		}
		
		if (Input.GetKeyUp("6")){
			if (toggle6==false){
				puppet_IIM_Soldier_001.SetActive(true); toggle6 = true;
			} else {
				puppet_IIM_Soldier_001.SetActive(false); toggle6=false;
			}
		}


		if (Input.GetKeyUp("7")){
			if (toggle13==false){
				puppet_IIM_Bird_Uni2D.EnableChildren(true);
				//puppet_IIM_Bird_Uni2D.SetActive(true); 
				 
				toggle13 = true;
			} else {
				puppet_IIM_Bird_Uni2D.EnableChildren(false);
				//puppet_IIM_Bird_Uni2D.SetActive(false); 
			
				toggle13=false;
			}
		}

		if (Input.GetKeyUp("8")){
			if (toggle14==false){
				puppet_IIM_Musician.SetActive(true); toggle14 = true;
			} else {
				puppet_IIM_Musician.SetActive(false); toggle14=false;
			}
		}

		if (Input.GetKeyUp("9")){
			if (toggle15==false){
				puppet_IIMKaragozAsHorse.SetActive(true); toggle15 = true;
			} else {
				puppet_IIMKaragozAsHorse.SetActive(false); toggle15 = false;
			}
		}


		if (Input.GetKeyUp("q")){
			if (toggle7==false){
				prop_IIM_Prop_High_House.SetActive(true); toggle7 = true;
			} else {
				prop_IIM_Prop_High_House.SetActive(false); toggle7=false;
			}
		}
		if (Input.GetKeyUp("w")){
			if (toggle8==false){
				prop_Book_UO_Shirins_Kiosk.SetActive(true); toggle8 = true;
			} else {
				prop_Book_UO_Shirins_Kiosk.SetActive(false); toggle8=false;
			}
		}
		if (Input.GetKeyUp("e")){
			if (toggle9==false){
				prop_IIM_Prop_Chair_001.SetActive(true); toggle9 = true;
			} else {
				prop_IIM_Prop_Chair_001.SetActive(false); toggle9=false;
			}
		}
		if (Input.GetKeyUp("r")){
			if (toggle10==false){
				prop_IIM_Prop_Pot.SetActive(true); toggle10 = true;
			} else {
				prop_IIM_Prop_Pot.SetActive(false); toggle10 = false;
			}
		}
		if (Input.GetKeyUp("t")){
			if (toggle11==false){
				prop_IIM_Prop_Pot_Breakable.SetActive(true); toggle11 = true;
			} else {
				prop_IIM_Prop_Pot_Breakable.SetActive(false); toggle11=false;
			}
		}


		if (Input.GetKeyUp("space")){
			if (toggle12==false){
				directionalLight.SetActive(true); toggle12 = true;

				mainOSCListener.setMonochromeState(false);
				if (puppet_IIM_Bird_Uni2D) {

					puppet_IIM_Bird_Uni2D.GetComponent<PuppetHelperIIM_Bird_Uni2D>().normalColorMode();
				} else {  }

				if (puppet_Dragon) {
					
					puppet_Dragon.GetComponent<PuppetHelperDragon_001>().normalColorMode();
				} else { } 


				if (puppet_IIMKaragozAsHorse) {
					
					puppet_IIMKaragozAsHorse.GetComponent<PuppetHelperIIM_Karagoz_As_Horse>().normalColorMode();
				} else { } 

				if (puppet_IIMKaragozAsHorse) {
					
					puppet_IIMKaragozAsHorse.GetComponent<PuppetHelperIIM_Karagoz_As_Horse>().normalColorMode();
				} else { }

				if (prop_IIM_Prop_High_House_animated) {
					
					//Color prop_color = prop_IIM_Prop_High_House_animated.renderer.material.color;

					prop_IIM_Prop_High_House_animated.renderer.material.color = Color.white;
				} else { }


			} else {
				directionalLight.SetActive(false); toggle12=false;

				mainOSCListener.setMonochromeState(true);
				if (puppet_Dragon) {
					
					puppet_Dragon.GetComponent<PuppetHelperDragon_001>().monochromeColorMode();
				} else { } 

				if (puppet_IIM_Bird_Uni2D) {
					
					puppet_IIM_Bird_Uni2D.GetComponent<PuppetHelperIIM_Bird_Uni2D>().monochromeColorMode();
				} else { } 

				if (puppet_IIMKaragozAsHorse) {
					
					puppet_IIMKaragozAsHorse.GetComponent<PuppetHelperIIM_Karagoz_As_Horse>().monochromeColorMode();
				} else { }

				if (prop_IIM_Prop_High_House_animated) {
					
					prop_IIM_Prop_High_House_animated.renderer.material.color = Color.black;
				} else { }

			}
		}



	}
}

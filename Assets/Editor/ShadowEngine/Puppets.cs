//C# Example

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

class Puppets : EditorWindow {


	//string myString = "Hello World";
	//bool groupEnabled;
	bool myBool = true;
	bool blurBool = false;
	bool motionBlurBool = false;
	//float myFloat = 1.23f;
	GameObject selected;
	GameObject BW_3D_Bottle;
	GameObject BW_3D_Bottle_Arm;
	GameObject BW_3D_Bottle_Leg;
	GameObject BW_with_Violin;
	GameObject BW_with_Violin_Arm;
	GameObject BW_with_Violin_Leg;
	GameObject BW_Tozer_Dog;
	GameObject BW_Tozer_Dog_Leg;
	GameObject BW_Tozer_Dog_Body;
	GameObject BW_Mrs_Brown_L;
	GameObject BW_Mrs_Brown_L_Bushell;
	GameObject BW_Mrs_Brown_R;
	GameObject BW_Mrs_Brown_R_Bushell;
	GameObject BW_Mary_Jane;
	GameObject BW_Mary_Jane_Pie;
	GameObject BW_Mary_Jane_Body;
	GameObject BW_Mrs_Martin;
	GameObject BW_Mrs_Martin_Arm;
	GameObject BW_Mrs_Martin_Body;
	GameObject BW_Jemima;
	GameObject BW_Jemima_Arm;
	GameObject BW_Jemima_Umbrella;
	GameObject BW_Policeman;
	GameObject BW_Policeman_Arm;
	GameObject BW_Policeman_LegR;
	GameObject BW_Policeman_LegL;
	GameObject BW_Policeman_Hat;


	//private GameObject go;

	// Add menu item named "My Window" to the Window menu
    [MenuItem ("ShadowEngine/Puppets Selection")]

	static void Init() {
		Debug.Log("In ShadowEngine Window Init");

		// Get existing open window or if none, make a new one:
		Puppets window = ( Puppets )EditorWindow.GetWindow( typeof( Puppets ) );
		window.Show ();



	}

    public static void  ShowWindow () {
       // EditorWindow.GetWindow(typeof(ShadowEngine));

    }
    

    void OnGUI () {
        // The actual window code goes here


		BW_3D_Bottle		= GameObject.Find("BW_3D_Bottle/Billy - Bottle - HeadController");
		BW_3D_Bottle_Arm	= GameObject.Find("BW_3D_Bottle/Billy - Bottle - HeadController/ArmController");
		BW_3D_Bottle_Leg	= GameObject.Find("BW_3D_Bottle/Billy - Bottle - HeadController/LegController");

		BW_with_Violin 		= GameObject.Find("BW_with_Violin/Billy - Violin - HeadController");
		BW_with_Violin_Arm 	= GameObject.Find("BW_with_Violin/Billy - Violin - HeadController/ArmController");
		BW_with_Violin_Leg 	= GameObject.Find("BW_with_Violin/Billy - Violin - HeadController/LegController");

		BW_Tozer_Dog 		= GameObject.Find("BW_Tozer_Dog/Dog - HeadController");
		BW_Tozer_Dog_Leg 	= GameObject.Find("BW_Tozer_Dog/Dog - HeadController/LegController");
		BW_Tozer_Dog_Body 	= GameObject.Find("BW_Tozer_Dog/Dog - HeadController/BodyController");


		BW_Mrs_Brown_R 			= GameObject.Find("BW_Mrs_Brown_R/Mrs Brown - HeadController");
		BW_Mrs_Brown_R_Bushell	= GameObject.Find("BW_Mrs_Brown_R/Mrs Brown - HeadController/BushellController");

		BW_Mrs_Brown_L			= GameObject.Find("BW_Mrs_Brown_L/Mrs Brown - HeadController");
		BW_Mrs_Brown_L_Bushell	= GameObject.Find("BW_Mrs_Brown_L/Mrs Brown - HeadController/BushellController");

		BW_Mary_Jane		= GameObject.Find("BW_Mary_Jane/Mary-Jane - HeadController");
		BW_Mary_Jane_Pie	= GameObject.Find("BW_Mary_Jane/Mary-Jane - HeadController/PieController");
		BW_Mary_Jane_Body	= GameObject.Find("BW_Mary_Jane/Mary-Jane - HeadController/BodyController");

		BW_Mrs_Martin		= GameObject.Find("BW_Mrs_Martin/Mrs Martin - HeadController");
		BW_Mrs_Martin_Arm	= GameObject.Find("BW_Mrs_Martin/Mrs Martin - HeadController/ArmController");
		BW_Mrs_Martin_Body	= GameObject.Find("BW_Mrs_Martin/Mrs Martin - HeadController/BodyController");

		BW_Jemima			= GameObject.Find("BW_Jemima/Jemima - HeadController");
		BW_Jemima_Umbrella	= GameObject.Find("BW_Jemima/Jemima - HeadController/UmbrellaController");
		BW_Jemima_Arm		= GameObject.Find("BW_Jemima/Jemima - HeadController/ArmController");

		BW_Policeman		= GameObject.Find("BW_Policeman/Policeman - HeadController");
		BW_Policeman_Arm	= GameObject.Find("BW_Policeman/Policeman - HeadController/ArmController");
		BW_Policeman_LegR	= GameObject.Find("BW_Policeman/Policeman - HeadController/LegRController");
		BW_Policeman_LegL	= GameObject.Find("BW_Policeman/Policeman - HeadController/LegLController");
		BW_Policeman_Hat	= GameObject.Find("BW_Policeman/Policeman - HeadController/HatController");

		GUILayout.Label ("Puppet Selection", EditorStyles.boldLabel);
		//GUI.color = Color.white;
		SetButtonColor(Color.white,Color.white,Color.white);

			GameObject[] objs = Selection.gameObjects;
		EditorGUILayout.BeginHorizontal();

		// BUTTON Billy Waters Set
		if (GUILayout.Button ("Billy Waters\nAll Puppets",  GUILayout.Width(90), GUILayout.Height(90))) {
			DoBillyAllPuppetSelection();
		}

		// BUTTON BW Violin
		if (GUILayout.Button ("BW Violin",  GUILayout.Width(70), GUILayout.Height(90))) {
			DoSelection(BW_with_Violin);
		}
		SetButtonColor(Color.blue,Color.white,Color.white);

		if (GUILayout.Button ("\u235F",  GUILayout.Width(20), GUILayout.Height(90))) {
			RotatePuppet(BW_with_Violin);
			DoSelection(BW_with_Violin);
		}
		SetButtonColor(Color.white,Color.white,Color.white);

		// BUTTON BW Bottle
		if (GUILayout.Button ("BW Bottle",  GUILayout.Width(68), GUILayout.Height(90))) {
			DoSelection(BW_3D_Bottle);
		}

	// Start Button
		SetButtonColor(Color.blue,Color.white,Color.white);
		if (GUILayout.Button ("\u235F",  GUILayout.Width(20), GUILayout.Height(90))) {
			RotatePuppet(BW_3D_Bottle);
			DoSelection(BW_3D_Bottle);
		}
		SetButtonColor(Color.white,Color.white,Color.white);
	// End Button

		// BUTTON Tozer Dog
		if (GUILayout.Button ("Tozer Dog",  GUILayout.Width(66), GUILayout.Height(90))) {
			DoSelection(BW_Tozer_Dog);
		}

	// Start Button
		SetButtonColor(Color.blue,Color.white,Color.white);
		if (GUILayout.Button ("\u235F",  GUILayout.Width(20), GUILayout.Height(90))) {


			RotatePuppet(BW_Tozer_Dog);
			DoSelection(BW_Tozer_Dog);
		}
		SetButtonColor(Color.white,Color.white,Color.white);
	// End Button

		// BUTTON BW_Mrs_Brown_L
		if (GUILayout.Button ("Mrs Brown\nL",  GUILayout.Width(66), GUILayout.Height(90))) {

			DoSelection(BW_Mrs_Brown_L);
		}
	// Start Button
		SetButtonColor(Color.blue,Color.white,Color.white);
		if (GUILayout.Button ("\u235F",  GUILayout.Width(20), GUILayout.Height(90))) {
			

			RotatePuppet(BW_Mrs_Brown_L);
			DoSelection(BW_Mrs_Brown_L);
		}
		SetButtonColor(Color.white,Color.white,Color.white);
	// End Button
	
		// BUTTON BW_Mrs_Brown_R
		if (GUILayout.Button ("Mrs Brown\nR",  GUILayout.Width(66), GUILayout.Height(90))) {
			DoSelection(BW_Mrs_Brown_R);
		}

	// Start Button
		SetButtonColor(Color.blue,Color.white,Color.white);
		if (GUILayout.Button ("\u235F",  GUILayout.Width(20), GUILayout.Height(90))) {
			
			
			RotatePuppet(BW_Mrs_Brown_R);
			DoSelection(BW_Mrs_Brown_R);
		}
		SetButtonColor(Color.white,Color.white,Color.white);
	// End Button

		// BUTTON BW_Mary_Jane
		if (GUILayout.Button ("Mary-Jane",  GUILayout.Width(66), GUILayout.Height(90))) {
			DoSelection(BW_Mary_Jane);
		}
	// Start Button
		SetButtonColor(Color.blue,Color.white,Color.white);
		if (GUILayout.Button ("\u235F",  GUILayout.Width(20), GUILayout.Height(90))) {
			
			
			RotatePuppet(BW_Mary_Jane);
			DoSelection(BW_Mary_Jane);
		}
		SetButtonColor(Color.white,Color.white,Color.white);
	// End Button


	
		// BUTTON BW_Jemima
		if (GUILayout.Button ("Jemima",  GUILayout.Width(68), GUILayout.Height(90))) {
			DoSelection(BW_Jemima);
		}

	// Start Button
		SetButtonColor(Color.blue,Color.white,Color.white);
		if (GUILayout.Button ("\u235F",  GUILayout.Width(20), GUILayout.Height(90))) {
			
			
			RotatePuppet(BW_Jemima);
			DoSelection(BW_Jemima);
		}
		SetButtonColor(Color.white,Color.white,Color.white);
	// End Button
	
		// BUTTON BW_Policeman
		if (GUILayout.Button ("Policeman",  GUILayout.Width(68), GUILayout.Height(90))) {
			DoSelection(BW_Policeman);
		}

	// Start Button
		SetButtonColor(Color.blue,Color.white,Color.white);
		if (GUILayout.Button ("\u235F",  GUILayout.Width(20), GUILayout.Height(90))) {
			
			
			RotatePuppet(BW_Policeman);
			DoSelection(BW_Policeman);
		}
		SetButtonColor(Color.white,Color.white,Color.white);
	// End Button

		EditorGUILayout.EndHorizontal();


// NEW ROW SMALL BUTTONS

		EditorGUILayout.BeginHorizontal();
		// Select Full Set Switches Left and Right Figures

		SetButtonColor(Color.cyan,Color.white,Color.white);

		if (GUILayout.Button ("Left",  GUILayout.Width(43), GUILayout.Height(27))) {
			DoSelectionAllLeftFacing();
		}
		if (GUILayout.Button ("Right",  GUILayout.Width(44), GUILayout.Height(27))) {
			DoSelectionAllRightFacing();
		}

		// select BW Violin sub-controllers
		if (GUILayout.Button ("Arm",  GUILayout.Width(43), GUILayout.Height(27))) {
			DoSelection(BW_with_Violin_Arm);
		}
		if (GUILayout.Button ("Leg",  GUILayout.Width(44), GUILayout.Height(27))) {
			DoSelection(BW_with_Violin_Leg);
		}

		GUILayout.Space(2);
		// select BW Bottle sub-controllers
		if (GUILayout.Button ("Arm",  GUILayout.Width(43), GUILayout.Height(27))) {
			DoSelection(BW_3D_Bottle_Arm);
		}
		if (GUILayout.Button ("Leg",  GUILayout.Width(44), GUILayout.Height(27))) {
			DoSelection(BW_3D_Bottle_Leg);
		}

		// select Tozer Dog sub-controllers
		if (GUILayout.Button ("Paw",  GUILayout.Width(42), GUILayout.Height(27))) {
			DoSelection(BW_Tozer_Dog_Leg);
		}
		if (GUILayout.Button ("Body",  GUILayout.Width(44), GUILayout.Height(27))) {
			DoSelection(BW_Tozer_Dog_Body);
		}

		// select Mrs Brown Left sub-controllers
		if (GUILayout.Button ("Bushell",  GUILayout.Width(88), GUILayout.Height(27))) {
			DoSelection(BW_Mrs_Brown_L_Bushell);
		}
		GUILayout.Space(2);
		// select Mrs Brown Right sub-controllers
		if (GUILayout.Button ("Bushell",  GUILayout.Width(88), GUILayout.Height(27))) {
			DoSelection(BW_Mrs_Brown_R_Bushell);
		}

		GUILayout.Space(2);
		// select Mary-Jane sub-controllers
		if (GUILayout.Button ("Pie",  GUILayout.Width(42), GUILayout.Height(27))) {
			DoSelection(BW_Mary_Jane_Pie);
		}
		if (GUILayout.Button ("Body",  GUILayout.Width(44), GUILayout.Height(27))) {
			DoSelection(BW_Mary_Jane_Body);
		}
		GUILayout.Space(2);
		// select Jemima sub-controllers
		if (GUILayout.Button ("Brolly",  GUILayout.Width(42), GUILayout.Height(27))) {
			DoSelection(BW_Jemima_Umbrella);
		}
		if (GUILayout.Button ("Arm",  GUILayout.Width(44), GUILayout.Height(27))) {
			DoSelection(BW_Jemima_Arm);
		}

		// select Policeman sub-controllers
		if (GUILayout.Button ("A",  GUILayout.Width(20), GUILayout.Height(27))) {
			DoSelection(BW_Policeman_Arm);
		}
		if (GUILayout.Button ("L",  GUILayout.Width(21), GUILayout.Height(27))) {
			DoSelection(BW_Policeman_LegL);
		}
		if (GUILayout.Button ("R",  GUILayout.Width(21), GUILayout.Height(27))) {
			DoSelection(BW_Policeman_LegR);
		}
		if (GUILayout.Button ("H",  GUILayout.Width(20), GUILayout.Height(27))) {
			DoSelection(BW_Policeman_Hat);
		}


		EditorGUILayout.EndHorizontal();


// NEW ROW BIG BUTTONS
		EditorGUILayout.BeginHorizontal();


		SetButtonColor(Color.white,Color.white,Color.white);

		// BUTTON Mrs Martin
		if (GUILayout.Button ("Mrs\nMartin",  GUILayout.Width(68), GUILayout.Height(90))) {
			DoSelection(BW_Mrs_Martin);
		}


	// Start Button
		SetButtonColor(Color.blue,Color.white,Color.white);
		if (GUILayout.Button ("\u235F",  GUILayout.Width(20), GUILayout.Height(90))) {
			RotatePuppet(BW_Mrs_Martin);
			DoSelection(BW_Mrs_Martin);
		}
		SetButtonColor(Color.white,Color.white,Color.white);
	// End Button

		// BUTTON Deselect All

		SetButtonColor(Color.yellow,Color.white,Color.white);

		if (GUILayout.Button ("Deselect All",  GUILayout.Width(90), GUILayout.Height(90))) {
			Selection.objects = new UnityEngine.Object[0];
		}

		SetButtonColor(Color.blue,Color.white,Color.white);


		if (GUILayout.Button ("Rotate\nBW with Violin",  GUILayout.Width(90), GUILayout.Height(90))) {

			RotatePuppet(BW_with_Violin);
		}
		if (GUILayout.Button ("Rotate\nBW with Bottle",  GUILayout.Width(90), GUILayout.Height(90))) {
			
			RotatePuppet(BW_3D_Bottle);
		}
		if (GUILayout.Button ("Rotate\nDog",  GUILayout.Width(90), GUILayout.Height(90))) {
			
			RotatePuppet(BW_Tozer_Dog);
		}

		// Set background, content and text color
		SetButtonColor(Color.red,Color.white,Color.white);
		//GUI.backgroundColor = Color.red;
		//GUI.contentColor = Color.white;
		//GUI.color= Color.white;


		if (EditorApplication.isPlaying) {
			SetButtonColor(Color.red,Color.white,Color.white);
		if (GUILayout.Button ("Stop",  GUILayout.Width(90), GUILayout.Height(90))) {

		EditorApplication.isPlaying = false ;}
		} else {
			SetButtonColor(Color.green,Color.white,Color.white);
			if (GUILayout.Button ("Play",  GUILayout.Width(90), GUILayout.Height(90))) {
				
				EditorApplication.isPlaying = true ;}

		}
	
// Test Camera Effects Settings
		if (blurBool == true) {
			SetButtonColor(Color.red,Color.white,Color.white);
			if (GUILayout.Button ("No Blur",  GUILayout.Width(90), GUILayout.Height(90))) {
				Camera.main.gameObject.GetComponent<Blur>().enabled =  false;
				//Camera.main.gameObject.GetComponent<Blur>().blurIterations =  1;
				//Camera.main.gameObject.GetComponent<Blur>().blurSize =  0.0f;
				blurBool = false;


			
			
			}
		} else {
			SetButtonColor(Color.green,Color.white,Color.white);
			if (GUILayout.Button ("Do Blur",  GUILayout.Width(90), GUILayout.Height(90))) {
				Camera.main.gameObject.GetComponent<Blur>().enabled =  true;
				//Camera.main.gameObject.GetComponent<Blur>().blurIterations =  1;
				//Camera.main.gameObject.GetComponent<Blur>().blurSize =  3.0f;
				blurBool = true;
			}
			
		}
// Test Camera Effects Settings
		if (motionBlurBool == true) {
			SetButtonColor(Color.red,Color.white,Color.white);
			if (GUILayout.Button ("No\nMotion Blur",  GUILayout.Width(90), GUILayout.Height(90))) {
				Camera.main.gameObject.GetComponent<MotionBlur>().enabled =  false;
				//Camera.main.gameObject.GetComponent<Blur>().blurIterations =  1;
				//Camera.main.gameObject.GetComponent<Blur>().blurSize =  0.0f;
				Camera.main.gameObject.GetComponent<MotionBlur>().blurAmount =  0.0f;
				motionBlurBool = false;
				
				
				
				
			}
		} else {
			SetButtonColor(Color.green,Color.white,Color.white);
			if (GUILayout.Button ("Do\nMotion Blur",  GUILayout.Width(90), GUILayout.Height(90))) {
				Camera.main.gameObject.GetComponent<MotionBlur>().enabled =  true;
				//Camera.main.gameObject.GetComponent<Blur>().blurIterations =  1;
				Camera.main.gameObject.GetComponent<MotionBlur>().blurAmount =  0.6f;
				motionBlurBool = true;
			}
			
		}

		

		
		

		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();

		SetButtonColor(Color.cyan,Color.white,Color.white);


		// select Mrs Martin sub-controllers
		if (GUILayout.Button ("Arm",  GUILayout.Width(42), GUILayout.Height(27))) {
			DoSelection(BW_Mrs_Martin_Arm);
		}

		if (GUILayout.Button ("Body",  GUILayout.Width(44), GUILayout.Height(27))) {
			DoSelection(BW_Mrs_Martin_Body);
		}



		EditorGUILayout.EndHorizontal();


		
	}

// FUNCTIONS
	void RotatePuppet(GameObject go) {
		// might be a good idea to store the current selection
				
		Selection.objects = new UnityEngine.Object[0];
			
		GameObject parent		= go.transform.parent.gameObject;
				
				
				
		List<GameObject> children = parent.GetChildren();

				
		int count = children.Count;
		//GameObject[] newSelection2 = new GameObject[count];
		//for(int i = 0; i < count; i++)
		//{
		//	Debug.Log(i);
		//	newSelection2[i] = children.name;
		//}
			
		GameObject[] newSelection2 = new GameObject[count];
		int i = 0;
		foreach(GameObject str in children)
		{	
			newSelection2[i] = str;
			Undo.RegisterUndo(str, "Rotate " + str.name);
			
			if (str.transform.rotation.y == 0)
			{
				//Camera.main.gameObject.GetComponent<MotionBlur>().enabled =  true;
				//Camera.main.gameObject.GetComponent<Blur>().blurIterations =  1;
				//Camera.main.gameObject.GetComponent<MotionBlur>().blurAmount =  0.2f;

				str.transform.eulerAngles = new Vector3(0,180f,0);
				//Camera.main.gameObject.GetComponent<MotionBlur>().enabled =  false;
				//Camera.main.gameObject.GetComponent<Blur>().blurIterations =  1;
				//Camera.main.gameObject.GetComponent<MotionBlur>().blurAmount =  0.0f;
			
			} else { 
				//Camera.main.gameObject.GetComponent<MotionBlur>().enabled =  true;
				//Camera.main.gameObject.GetComponent<Blur>().blurIterations =  1;
				//Camera.main.gameObject.GetComponent<MotionBlur>().blurAmount =  0.2f;
				str.transform.eulerAngles = new  Vector3(0,0,0);
				//Camera.main.gameObject.GetComponent<MotionBlur>().enabled =  false;
				//Camera.main.gameObject.GetComponent<Blur>().blurIterations =  1;
				//Camera.main.gameObject.GetComponent<MotionBlur>().blurAmount =  0.0f;
			}
			
			Debug.Log(str);
			i++;
		}
		Selection.objects = newSelection2;
		// rotate children
		
		
		//Selection.objects = new UnityEngine.Object[0];

	}

	void DoSelection(GameObject go) {
		
		// reset selection
		Selection.objects = new UnityEngine.Object[0];

		// select object
		selected = go;
		Selection.activeObject=selected;
		EditorGUIUtility.PingObject(selected);

	}

	void DoSelectionAllLeftFacing() {



	}

	
	void DoSelectionAllRightFacing() {
		
		
		
	}
// Select All

	void DoBillyAllPuppetSelection() {

		// do multiple selections
		// reset current selections

		Selection.objects = new UnityEngine.Object[0];


		GameObject[] newSelection = new GameObject[9];

		newSelection[0] = BW_with_Violin;
		newSelection[1] = BW_Tozer_Dog;		
		newSelection[2] = BW_Mrs_Brown_R;
		newSelection[3] = BW_Mrs_Brown_L;
		newSelection[4] = BW_Mary_Jane;
		newSelection[5] = BW_Mrs_Martin;
		newSelection[6] = BW_Jemima;		
		newSelection[7] = BW_Policeman;	
		newSelection[8] = BW_3D_Bottle;	

		Selection.objects = newSelection;

	}

	void SetButtonColor (Color buttonBackgroundColor, Color buttonContentColor, 
	                     Color buttonColor) {

		GUI.backgroundColor = buttonBackgroundColor;
		GUI.contentColor = buttonContentColor;
		GUI.color= buttonColor;

	}
}
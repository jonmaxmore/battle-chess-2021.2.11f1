using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Networking;



public enum CameraAngle{
	menu=0,
	whiteTeam=1,
	blackTeam=2
}
public class GameUIControl : MonoBehaviour
{
	[SerializeField] private GameObject[] cameraAngles;
	public static GameUIControl instance;

 


	
	// Awake is called when the script instance is being loaded.
	private void Awake()
	{
		instance=this;
	}
    // Start is called before the first frame update
    void Start()
    {
	   
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	
	//Cameras
	public void ChangeCamera(CameraAngle index){
		for (int i = 0; i < cameraAngles.Length; i++) {
			cameraAngles[i].SetActive(false);
		}
	 
		cameraAngles[(int)index].SetActive(true);

	}
}

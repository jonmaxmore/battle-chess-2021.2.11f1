using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class LoadingText : MonoBehaviour
{
	public TextMeshProUGUI loadingText;
	public string[ ] chessTips = new string[ ]{"Don’t make too many moves with your pawns or try to pick off your opponent’s pawns.", "Kate", "Adam", "Mia"} ;
	int TextNumberToShow;


// Start is called before the first frame update
    void Start()
    {
	    InvokeRepeating("showTextInArray", 0, 6);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
	void showTextInArray()
	{
		loadingText.text = chessTips[TextNumberToShow];
  
		TextNumberToShow++;
		if (TextNumberToShow  ==chessTips.Length)
			TextNumberToShow = 0;
  
	}
}

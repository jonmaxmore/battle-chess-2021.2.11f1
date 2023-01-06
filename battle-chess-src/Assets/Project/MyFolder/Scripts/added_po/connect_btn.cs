using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using LitJson;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class connect_btn : MonoBehaviour
{
    // Start is called before the first frame update
    // Update is called once per frame
    public TextMeshProUGUI address;
    public TMP_InputField c_username;
    public GameObject failed;
    public GameObject login_failed;
    public GameObject login_screen;
    public GameObject signup_screen;

    bool flag=true;
    void Start() {
        Global.GetDomain();

        StartCoroutine(delay());

        Debug.Log("---- Current Domain : " + Global.currentDomain);
    }
    IEnumerator delay(){
        yield return new WaitForSeconds(0.5f); 
        Web3.Initialize();
    }

    void Update() {
        if(flag){
            Debug.Log("start get address");
            address.text=Web3.get_address();
            if(address.text=="false"||address.text==null||address.text==""){
                Debug.Log("start get address again");
                return;
            }
            else login_connect();
        }
    }
    public void login_connect()
    {   
        flag=false;
        string user_address = address.text;

        WWWForm formData = new WWWForm();
        formData.AddField("address", user_address);

        string requestURL = Global.currentDomain + "/api/w_login";


        UnityWebRequest www = UnityWebRequest.Post(requestURL, formData);
        //requestURL += "?username=" + username + "&password=" + password;
        //UnityWebRequest www = UnityWebRequest.Get(requestURL);
        //www.SetRequestHeader("Accept", "application/json");
        //www.uploadHandler.contentType = "application/json";
        StartCoroutine(iRequest(www));

    }
        public void OnClickConnectButton(){
            Web3.Initialize();
        }
        public void OnClickSingupButton()
    {
        string user_address = address.text;

        string user_name=c_username.text;
        WWWForm formData = new WWWForm();
        formData.AddField("address", user_address);
        formData.AddField("username", user_name);
        
        string requestURL = Global.currentDomain + "/api/w_signup";

        UnityWebRequest www = UnityWebRequest.Post(requestURL, formData);
        //requestURL += "?username=" + username + "&password=" + password;
        //UnityWebRequest www = UnityWebRequest.Get(requestURL);
        //www.SetRequestHeader("Accept", "application/json");
        //www.uploadHandler.contentType = "application/json";
        StartCoroutine(iRequest(www));

    }

        IEnumerator iRequest(UnityWebRequest www)
    {
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
            failed.transform.GetComponent<TextMeshProUGUI>().text="Connect error";
            failed.SetActive(true);
            yield break;
        }

        string resultData = www.downloadHandler.text;

        if (string.IsNullOrEmpty(resultData))
        {
            Debug.Log("Result Data Empty");
            failed.transform.GetComponent<TextMeshProUGUI>().text="Result Data Empty";
            failed.SetActive(true);
            yield break;
        }


        JsonData json = JsonMapper.ToObject(resultData);
        string response = json["success"].ToString();

        if(response=="-2")
        {
            failed.transform.GetComponent<TextMeshProUGUI>().text="User name is already exist.";
            failed.SetActive(true);
        }
        else if (response != "1")
        {
            Debug.Log(response);
            Debug.Log("Login Failed");

            login_screen.SetActive(false);
            signup_screen.SetActive(true);

        }
        else
        {
                Global.m_user = new User();
                Global.m_user.id = long.Parse(json["data"]["id"].ToString());
                Global.m_user.name = json["data"]["name"].ToString();
                Global.m_user.score = long.Parse(json["data"]["score"].ToString());
                
                failed.SetActive(false);
                SceneManager.LoadScene("MainMenu");
        }

        
    }
}


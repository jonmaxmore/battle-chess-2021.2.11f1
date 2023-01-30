using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Global
{
	public static string DOMAIN = "54.179.131.233";
	public static int PORT = 7877;
	public static bool SSL_ENALBLED = false;

    public static bool isTesting = false;
	public static int testingPort = 7877;
    public static string testingDomain = "localhost";
    public static bool socketConnected = false;

    public static string currentDomain = "";

    public static User m_user;
    public static string savedData;
    public static bool isLoading = false;
    public static bool nextLoad;

    public static string GetDomain()
    {
        currentDomain = DOMAIN;

         if (SSL_ENALBLED)
         {
             currentDomain = "https://" + currentDomain;
         }
         else
         {
           currentDomain = "http://" + currentDomain;
         }

        if (PORT != 0)
        {
            currentDomain += ":" + PORT;
        }

        //if (isTesting == true)
        //{
        //    currentDomain = "http://" + testingDomain + ":" + testingPort;
        //}
        return currentDomain;
    }
}

[Serializable]
public class User
{
    public long id;
    public string name;
    public long score;

    public User(long id = -1, string name = "", long score = 0)
    {
        this.id = id;
        this.name = name;
        this.score = score;
    }
}

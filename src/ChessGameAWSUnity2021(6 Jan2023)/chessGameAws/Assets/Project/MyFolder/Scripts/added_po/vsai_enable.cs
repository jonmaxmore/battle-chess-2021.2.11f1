using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class vsai_enable : MonoBehaviour
{
    public void vs_ai_enalbe()
    {
        PlayerPrefs.SetInt("VsCPU", 1);
    }
}

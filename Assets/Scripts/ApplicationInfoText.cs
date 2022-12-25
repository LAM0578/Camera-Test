using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class ApplicationInfoText : MonoBehaviour
{
    private void Awake()
    {
        m_Text = this.gameObject.GetComponent<Text>();
        ShowTextHead = 
            "Press \"R\" to reset player position.\n" +
            $"{Application.productName} By {Application.companyName} \n" +
            $"Application Version: {Application.version} Unity Version: {Application.unityVersion}\n";
    }

    private static string ShowTextHead;
    private Text m_Text;
    private int Frame;
    private float lastInterval;

    // [DllImport("mono.dll")]
    // public static extern long mono_gc_get_used_size();

    private void Update()
    {
        ++Frame;
        float timeNow = Time.realtimeSinceStartup;
        float interval = timeNow - lastInterval;
        if (interval > 0.25f)
        {
            // float usedMemory = mono_gc_get_used_size();
            // SystemInfo.systemMemorySize
            m_Text.text = 
                ShowTextHead + 
                $"FPS: {(float)Frame / interval}\n";
                // $"Memory: {usedMemory} / {SystemInfo.systemMemorySize}";

            Frame = 0;
            lastInterval = timeNow;
        }
# if UNITY_STANDALONE
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Escape)) Application.Quit();
        else if (Input.GetKeyDown(KeyCode.Escape) && Screen.fullScreen) Screen.fullScreen = false;
# endif
    }
}

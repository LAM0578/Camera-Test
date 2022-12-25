using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ApplicationInfoText : MonoBehaviour
{
    private void Awake()
        => this
            .gameObject
            .GetComponent<Text>()
            .text = "Press \"R\" to reset player position.\n" + 
            $"{Application.productName} By {Application.companyName} \n" +
            $"Application Version: {Application.version} Unity Version: {Application.unityVersion}";
}

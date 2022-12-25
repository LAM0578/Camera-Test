using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CameraTest
{
    public class ButtonManager : MonoBehaviour
    {
        public static ButtonManager Instance { get; private set; }
        
        private void Awake()
        {
            Instance = this;
            m_buttons = (ButtonItem[])Resources.FindObjectsOfTypeAll(typeof(ButtonItem));
            // Debug
            foreach (var item in m_buttons)
                print($"ButtonItem: {item.gameObject.name}");
        }

        private ButtonItem[] m_buttons;
        [HideInInspector]
        public ButtonItem CurrentEnter;
        public GameObject CameraGameObject;
        public Camera WorldCamera;

        /// <summary> Update button status. </summary>
        private void Update()
        {
            for (int i = 0; i < m_buttons.Length; i++) 
                m_buttons[i].UpdateInfo(CameraManager.Instance.PlayerPosition);
        }

        /// <summary> Set alpha of all buttons. </summary>
        /// <param name="isEnter"> Is enter penal. </param>
        public void SetButton(bool isEnter)
        {
            foreach (var item in m_buttons) 
                item.ButtonControl(isEnter);
        }
    }
}

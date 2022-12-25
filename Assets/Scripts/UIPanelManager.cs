using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace CameraTest
{
    public class UIPanelManager : MonoBehaviour
    {
        public static UIPanelManager Instance { get; private set; }
        
        private void Awake() 
        {
            Instance = this;
            m_CanvasGroup = this.gameObject.GetComponent<CanvasGroup>();
            m_CanvasGroup.alpha = 0;
            m_CanvasGroup.gameObject.SetActive(false);
        }

        private CanvasGroup m_CanvasGroup;
        private bool m_IsEnterPanel;
        public float Duration = 1.5f;
        public float CanMutuallyRange = 10f;
        public float HideRange = 15f;

        /// <summary> Control the panel hide or show. </summary>
        /// <param name="isEnter"> Is enter penal. </param>
        public void PanelControl(bool isEnter)
        {
            if (isEnter)
            {
                m_IsEnterPanel = true;
                m_CanvasGroup.gameObject.SetActive(true);
            } 
            m_CanvasGroup.DOFade(isEnter ? 1 : 0, Duration).OnComplete(
                () => {
                    if (!isEnter && m_IsEnterPanel)
                    {
                        m_IsEnterPanel = false;
                        m_CanvasGroup.gameObject.SetActive(false);
                    }
                }
            );
        }
    }
}

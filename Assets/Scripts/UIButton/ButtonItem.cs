using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using CameraTest.Util;

namespace CameraTest
{
    #pragma warning disable
    public class ButtonItem : MonoBehaviour
    {
        private void Awake() => m_GameObject = this.gameObject;

        private GameObject m_GameObject;
        [SerializeField]
        private Button m_Button;
        [SerializeField]
        private Vector3 m_Offset;

        private bool IsPlaying;
        private bool? LastEnableStatus = null;


        /// <summary> Update m_GameObject in UI Position set the position to m_Button. </summary>
        /// <param name="pos"> Player position. </param>
        public void UpdateInfo(Vector3 pos)
        {
            PlayerInRange(pos);
            m_Button.gameObject.SetActive(IsInView());
            if (!m_Button.gameObject.activeSelf) return;
            // If the button or world camera is null we must return
            if (m_Button == null || ButtonManager.Instance.WorldCamera == null) return;
            var screenPosition = ButtonManager.Instance.WorldCamera.WorldToScreenPoint(
                m_GameObject.transform.position + m_Offset);
            // Reset game object position to screen position result by screen resolution and ratio
            float ratio = (float)Screen.width / (float)Screen.height;
            var finalPosition = new Vector3(
                screenPosition.x / Screen.width * 1920f,
                screenPosition.y / Screen.height * 1080f * ((16f / 9f) / ratio)
            );
            m_Button.GetComponent<RectTransform>().anchoredPosition = finalPosition;
        }

        /// <summary> Check m_GameObject is in camera view. </summary>
        public bool IsInView()
        {
            var transform = ButtonManager.Instance.WorldCamera.transform;
            var pos = m_GameObject.transform.position;
            var viewPoint = ButtonManager.Instance.WorldCamera.WorldToViewportPoint(pos);
            var dir = (pos - transform.position).normalized;
            float dot = Vector3.Dot(transform.forward, dir);
            return dot > 0 && 
                viewPoint.x >= CameraManager.Instance.OutScreenRange.x && 
                viewPoint.x <= CameraManager.Instance.OutScreenRange.y && 
                viewPoint.y >= CameraManager.Instance.OutScreenRange.x && 
                viewPoint.y <= CameraManager.Instance.OutScreenRange.y;
        }

        /// <summary> Control camera look at this game object. </summary>
        public void LookTo()
        {
            if (ButtonManager.Instance.WorldCamera == null || IsPlaying) return;
            IsPlaying = true;
            // Find nearest ground height and set y to hit height and translate player to ground.
            var playerPosition = CameraManager.Instance.PlayerPosition;
            if (Physics.Raycast(playerPosition, Vector3.down, out var hitInfo))
            {
                // Because the position is not player foot position, so we add a offset in here.
                playerPosition.y = hitInfo.point.y + 1f;
                // Set position to player
                CameraManager.Instance.PlayerPosition = playerPosition;
            }
            var gameObject = ButtonManager.Instance.CameraGameObject;
            // Get start rotate
            var rot = gameObject.transform.eulerAngles;
            // Set camera look at target and get it
            gameObject.transform.LookAt(m_GameObject.transform.position);
            var lookAtRot = gameObject.transform.eulerAngles;
            // Set camera rotate to start rotate
            gameObject.transform.eulerAngles = rot;
            // Do rotate
            // print($"rotVec: {rotVec}, lookAtRotVec: {lookAtRotVec}, fixRotVec: {lookAtRotVec - rotVec}");
            gameObject.transform.DOBlendableRotateBy(lookAtRot - rot, 1.5f).SetEase(Ease.OutCubic).OnUpdate(() => {
                var rotSelf = gameObject.transform.eulerAngles;
                rotSelf.z = 0;
                gameObject.transform.eulerAngles = rotSelf;
            });
            // gameObject.transform.DORotate(lookAtRotVec, CameraManager.Instance.Duration).SetEase(Ease.OutCubic);
            IsPlaying = false;
            CameraManager.Instance.OnPanelEnter(
                GetPlayerToCenterDistance(CameraManager.Instance.PlayerPosition));
            ButtonManager.Instance.CurrentEnter = this;
        }

        /// <summary> Return player position to center point distance. </summary>
        /// <param name="pos"> Player position. </param>
        /// <returns> The distance of player position to center point. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float GetPlayerToCenterDistance(Vector3 pos)
        {
            Vector3 center = m_GameObject.transform.position;
            return (pos - center).magnitude;
        }

        /// <summary> Control m_Button enable by player position. </summary>
        /// <param name="pos"> Player position. </param>
        private void PlayerInRange(Vector3 pos)
        {
            float radius = UIPanelManager.Instance.HideRange;
            // Check the position is in a shpere range
            float distance = GetPlayerToCenterDistance(pos);
            bool inRange = distance <= radius;
            // If is not in range we use ButtonControlInternal to hide m_Button
            // print($"Player in {this.gameObject.name} range: {inRange}, distance: {distance}");
            if (LastEnableStatus == null || LastEnableStatus != inRange)
            {
                LastEnableStatus = inRange;
                ButtonControlInternal(!inRange);
            }
        }

        /// <summary> Control the button is hide or show. </summary>
        /// <param name="isEnter"> Is enter penal. </param>
        public void ButtonControl(bool isEnter)
        {
            if (!(LastEnableStatus ?? false)) isEnter = true;
            ButtonControlInternal(isEnter);
        }
    
        /// <summary> Control the button is hide or show and ignore conditional restrictions. </summary>
        /// <param name="isEnter"> Is enter penal. </param>
        private void ButtonControlInternal(bool isEnter)
        {
            m_Button.GetComponent<CanvasGroup>().DOFade(isEnter ? 0 : 1, UIPanelManager.Instance.Duration);
            m_Button.GetComponent<CanvasGroup>().blocksRaycasts = !isEnter;
            m_Button.GetComponent<CanvasGroup>().interactable = !isEnter;
        }
    }
}

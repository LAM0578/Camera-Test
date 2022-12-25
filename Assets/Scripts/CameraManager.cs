using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using CameraTest.Util;

namespace CameraTest
{
    public class CameraManager : MonoBehaviour
    {
        public static CameraManager Instance { get; private set; }
        
        private void Awake()
        {
            Instance = this;
            m_GameObject = this.gameObject;
            m_Camera = m_GameObject.GetComponentInChildren<Camera>();
            // m_CurrentPosition = m_Camera.transform.localPosition;
            m_Rigidbody = ControlGameObject.GetComponent<Rigidbody>();
        } 

        private GameObject m_GameObject;
        private Camera m_Camera;
        private Rigidbody m_Rigidbody;
        private bool m_IsEnterPenal;
        private bool m_LastEnterPenalStatus;
        private float m_ScrollScale;
        
        private bool IsPlaying;
        public Vector3 PlayerPosition
        {
            get => ControlGameObject.transform.position;
            set => ControlGameObject.transform.position = value;
        }
        public KeyCode[] ControlKeys = new[]{
            KeyCode.W,
            KeyCode.S,
            KeyCode.A,
            KeyCode.D
        };

        [Header("Camera")]
        public float Duration = 1.5f;
        public Vector3 AppendPosition;
        [Tooltip("X: Min Range, Y: Max Range")]
        public Vector2 OutScreenRange;
        public float EnterFov;
        public float QuitFov;
        // public bool AlwaysPositiveRotateValue;
        public GameObject ControlGameObject;

        [Header("Controller")]
        public float Speed;
        public float ScrollSpeed;
        public float RotateSpeed;
        public float InGroundRange = 0.1f;
        public bool ScrollUseRange;
        [Tooltip("X: Scroll Min, Y: Scroll Max")]
        public Vector2 ScrollRange;
        public Vector3 ResetPosition;

        [Header("Speed Scale")]
        public float SpeedScale = 200;
        public float JumpSpeedScale = 50;

        /// <summary> Update panel controller and player move controller. </summary>
        private void Update()
        {
            // print($"IsPlaying: {IsPlaying}, IsEnterPenal: {m_IsEnterPenal}, LastEnterPenalStatus: {m_LastEnterPenalStatus}");
            if (Input.GetKeyDown(KeyCode.R))
                ControlGameObject.transform.position = ResetPosition;
            // Scroll
            m_ScrollScale -= Input.GetAxis("Mouse ScrollWheel");
            if (ScrollUseRange)
            {
                if (m_ScrollScale < ScrollRange.x) m_ScrollScale = ScrollRange.x;
                if (m_ScrollScale > ScrollRange.y) m_ScrollScale = ScrollRange.y;
            }
            // Move camera
            var pos = ControlGameObject.transform.position;
            var rot = ControlGameObject.transform.rotation;
            m_GameObject.transform.position = pos + rot * Quaternion.Euler(rot.y, rot.x, 0) * new Vector3(0,1,0);
            // Set scroll
            m_GameObject.transform.Translate(Vector3.back * m_ScrollScale * ScrollSpeed);
            // Rotate camera and quit penal
            if (Input.GetMouseButton(0))
            {
                if(!EventSystem.current.IsPointerOverGameObject()) OnPanelQuit();
                ControlGameObject.transform.eulerAngles = new Vector3(0, m_GameObject.transform.eulerAngles.y, 0);
                // We can't use "if (m_IsEnterPenal) return;" in here, it won't work.
                if (!m_IsEnterPenal)
                {
                    // Rotation Controller
                    float mx = Input.GetAxis("Mouse X");
                    float my = Input.GetAxis("Mouse Y");
                    m_GameObject.transform.RotateAround(pos, Vector3.up, mx * RotateSpeed);
                    m_GameObject.transform.RotateAround(pos, m_GameObject.transform.right, -my * RotateSpeed);
                }
            }
            if (!m_IsEnterPenal)
            {
                float speedAppend = Input.GetKey(KeyCode.LeftShift) ? 5 : 0;
                // Position Controller
                if (Input.GetKey(ControlKeys[0])) 
                    m_Rigidbody.AddForce(
                        ControlGameObject.transform.forward * Time.deltaTime * (Speed + speedAppend) * SpeedScale
                    );
                if (Input.GetKey(ControlKeys[1])) 
                    m_Rigidbody.AddForce(
                        -ControlGameObject.transform.forward * Time.deltaTime * (Speed + speedAppend) * SpeedScale
                    );
                if (Input.GetKey(ControlKeys[2])) 
                    m_Rigidbody.AddForce(
                        -ControlGameObject.transform.right * Time.deltaTime * (Speed + speedAppend) * SpeedScale
                    );
                if (Input.GetKey(ControlKeys[3])) 
                    m_Rigidbody.AddForce(
                        ControlGameObject.transform.right * Time.deltaTime * (Speed + speedAppend) * SpeedScale
                    );
                if (Input.GetKeyDown(KeyCode.Space) && 
                    Physics.Raycast(PlayerPosition, Vector3.down, InGroundRange))
                {
                    m_Rigidbody.AddForce(Vector3.up * 75 * Speed);
                    if (!InputUtil.GetAnyKeysPressed(ControlKeys))
                        m_Rigidbody.AddForce(
                            ControlGameObject.transform.forward * (Speed + speedAppend) * JumpSpeedScale
                        );
                }
            }
        }

        /// <summary> Set camera position and FOV to enter panel value. </summary>
        public void OnPanelEnter(float distance)
        {
            if (distance > UIPanelManager.Instance.CanMutuallyRange) return;
            if (m_IsEnterPenal) return;
            m_IsEnterPenal = true;
            m_LastEnterPenalStatus = true;
            ButtonManager.Instance.SetButton(true);
            UIPanelManager.Instance.PanelControl(true);
            m_Camera.transform.DOBlendableLocalMoveBy(AppendPosition, Duration).SetEase(Ease.OutCubic);
            m_Camera.DOFieldOfView(EnterFov, Duration).SetEase(Ease.OutCubic);
        }

        /// <summary> Set camera position and FOV to original value. </summary>
        public void OnPanelQuit()
        {
            if (!m_IsEnterPenal) return;
            m_LastEnterPenalStatus = false;
            ButtonManager.Instance.SetButton(false);
            UIPanelManager.Instance.PanelControl(false);
            m_Camera.transform.DOLocalMove(new Vector3(), Duration * 0.75f).SetEase(Ease.OutCubic);
            m_Camera.GetComponent<Camera>()
                .DOFieldOfView(QuitFov, Duration)
                .SetEase(Ease.OutCubic)
                .OnComplete(() => 
                {
                    if (!m_LastEnterPenalStatus && m_IsEnterPenal) m_IsEnterPenal = false;
                });
            if (ButtonManager.Instance.CurrentEnter != null) ButtonManager.Instance.CurrentEnter = null;
        }
    }
}

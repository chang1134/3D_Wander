﻿using UnityEngine;
#if UNITY_5_3_OR_NEWER
using UnityEngine.SceneManagement;
using UnityEngine.Video;
#endif

namespace Invector.CharacterController
{
    public class vThirdPersonInput : MonoBehaviour
    {
        #region variables

        [Header("Default Inputs")]
        public string horizontalInput = "Horizontal";
        public string verticallInput = "Vertical";
        public KeyCode jumpInput = KeyCode.Space;
        public KeyCode strafeInput = KeyCode.Tab;
        public KeyCode sprintInput = KeyCode.LeftShift;

        [Header("Camera Settings")]
        public string rotateCameraXInput ="Mouse X";
        public string rotateCameraYInput = "Mouse Y";

        protected vThirdPersonCamera tpCamera;                // acess camera info        
        [HideInInspector]
        public string customCameraState;                    // generic string to change the CameraState        
        [HideInInspector]
        public string customlookAtPoint;                    // generic string to change the CameraPoint of the Fixed Point Mode        
        [HideInInspector]
        public bool changeCameraState;                      // generic bool to change the CameraState        
        [HideInInspector]
        public bool smoothCameraState;                      // generic bool to know if the state will change with or without lerp  
        [HideInInspector]
        public bool keepDirection;                          // keep the current direction in case you change the cameraState

        protected vThirdPersonController cc;                // access the ThirdPersonController component                

        #endregion

        protected virtual void Start()
        {
            CharacterInit();
        }

        protected virtual void CharacterInit()
        {
            cc = GetComponent<vThirdPersonController>();
            if (cc != null)
                cc.Init();

            tpCamera = FindObjectOfType<vThirdPersonCamera>();
            if (tpCamera) tpCamera.SetMainTarget(this.transform);

            //Cursor.visible = false;
            //Cursor.lockState = CursorLockMode.Locked;
        }

        protected virtual void LateUpdate()
        {
            if (cc == null) return;             // returns if didn't find the controller		    
            InputHandle();                      // update input methods
            UpdateCameraStates();               // update camera states
        }

        protected virtual void FixedUpdate()
        {
            if (Input.GetMouseButtonDown(0))
            {
                //射线检测，播放视频
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit))
                {
                    VideoPlayer videoPlayer = hit.transform.GetComponent<VideoPlayer>();
                    if (videoPlayer != null)
                    {
                        if (videoPlayer.isPlaying)
                        {
                            videoPlayer.Pause();
                        }
                        else
                        {
                            videoPlayer.Play();
                        }
                    }
                }
            }
            cc.AirControl();
            CameraInput();
        }

        protected virtual void Update()
        {
            cc.UpdateMotor();                   // call ThirdPersonMotor methods               
            cc.UpdateAnimator();                // call ThirdPersonAnimator methods		               
        }

        protected virtual void InputHandle()
        {
            ExitGameInput();
            CameraInput();

            if (!cc.lockMovement)
            {
                MoveCharacter();
                SprintInput();
                StrafeInput();
                JumpInput();
            }
        }

        #region Basic Locomotion Inputs      

        protected virtual void MoveCharacter()
        {
            cc.input.x = ScrollCircle.Instance.horizontalValue;// Input.GetAxis(horizontalInput);
            cc.input.y = ScrollCircle.Instance.verticalValue;//Input.GetAxis(verticallInput);
        }

        protected virtual void StrafeInput()
        {
            if (Input.GetKeyDown(strafeInput))
                cc.Strafe();
        }

        protected virtual void SprintInput()
        {
            if (Input.GetKeyDown(sprintInput))
                cc.Sprint(true);
            else if (Input.GetKeyUp(sprintInput))
                cc.Sprint(false);
        }

        protected virtual void JumpInput()
        {
            if (Input.GetKeyDown(jumpInput))
                cc.Jump();
        }

        protected virtual void ExitGameInput()
        {
            // just a example to quit the application 
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (!Cursor.visible)
                    Cursor.visible = true;
                else
                    Application.Quit();
            }
        }

        #endregion

        #region Camera Methods
        private bool isCanRotate = false;
        private Vector3 oldPos = Vector3.zero;
        protected virtual void CameraInput()
        {
            if (tpCamera == null)
                return; 
            for (int i = 0; i < Input.touchCount; i++)
            {
                //判断是否是虚拟摇杆的点
                if (Vector3.Distance(Input.touches[i].position, ScrollCircle.Instance.dragPos) < 30 && Input.touches[i].position != Vector2.zero)
                {
                    continue;
                }
                switch (Input.touches[i].phase)
                {
                    case TouchPhase.Began:
                        isCanRotate = !ScrollCircle.Instance.isContineMouse(Input.touches[i].position);
                        break;
                    case TouchPhase.Moved:
                        var Y = (Input.touches[i].position.y - oldPos.y) / 10;
                        var X = (Input.touches[i].position.x - oldPos.x) / 10;

                        if (Vector3.Distance(Input.touches[i].position, oldPos) < 50)
                        {
                            tpCamera.RotateCamera(X, Y);
                            // tranform Character direction from camera if not KeepDirection
                            if (!keepDirection)
                                cc.UpdateTargetDirection(tpCamera != null ? tpCamera.transform : null);
                            // rotate the character with the camera while strafing        
                            RotateWithCamera(tpCamera != null ? tpCamera.transform : null);
                        }
                        oldPos = Input.touches[i].position;
                        break;
                    case TouchPhase.Ended:
                        isCanRotate = false;
                        break;
                }
            }
        }

        protected virtual void UpdateCameraStates()
        {
            // CAMERA STATE - you can change the CameraState here, the bool means if you want lerp of not, make sure to use the same CameraState String that you named on TPCameraListData
            if (tpCamera == null)
            {
                tpCamera = FindObjectOfType<vThirdPersonCamera>();
                if (tpCamera == null)
                    return;
                if (tpCamera)
                {
                    tpCamera.SetMainTarget(this.transform);
                    tpCamera.Init();
                }
            }            
        }

        protected virtual void RotateWithCamera(Transform cameraTransform)
        {
            if (cc.isStrafing && !cc.lockMovement && !cc.lockMovement)
            {                
                cc.RotateWithAnotherTransform(cameraTransform);                
            }
        }

        #endregion     
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

namespace Jun.Camera
{
    public class MainCamera : MonoBehaviour
    {
        private enum WheelStateType
        {
            CenterTarget = 0,
            ZoomOutPlayer,
            ZoomInPlayer
        }

        [Header("Camera Control Compoent"), Space(5)]
        [SerializeField] private CameraFocus cameraFocus;
        [SerializeField] private CinemachineVirtualCamera virtualCamera;
        private CinemachineComponentBase componentBase;

        [Header("Camera Control Property"), Space(5)]
        [SerializeField] private Transform currentFloorCenter;
        [SerializeField] private List<Transform> floorCenterList = new List<Transform>();
        [SerializeField, Range(0f, 100f)] private float maxFloorCenterDistance;
        [SerializeField] private Transform playerPos;
        [SerializeField, Range(0f, 1f)] private float rotationSpeed = 0.15F;
        [SerializeField, Range(0, 10)] private int zoomSpeed = 5;
        [SerializeField] private WheelStateType wheelState;
        [SerializeField] private float[] cameraDistances = new float[3];
        private float currentWheelCount = 0f;
        private bool isRunningWheelCor = false;
        private bool isRunningInitWheelCor = false;
        private int wheelStateLength = System.Enum.GetValues(typeof(WheelStateType)).Length;

        private Vector3 eulerAngle;
        private Vector2 rotationDelta;
        private Quaternion rotation;

        private void Start()
        {
            if (cameraFocus == null)
                cameraFocus = FindObjectOfType<CameraFocus>();

            if (virtualCamera == null)
                virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();

            if (playerPos == null)
                playerPos = FindObjectOfType<Player.Player>().transform;

            if (floorCenterList.Count <= 0)
            {
                var list = GameObject.FindGameObjectsWithTag("FloorCenter");

                foreach (var mapPos in list)
                    floorCenterList.Add(mapPos.transform);
            }

            if (floorCenterList.Count > 0)
                currentFloorCenter = floorCenterList[0];

            wheelState = WheelStateType.ZoomInPlayer;
            componentBase = virtualCamera.GetCinemachineComponent(CinemachineCore.Stage.Body);

            ResetRotation();

            StartCoroutine(CheckFloorCenter());

            SetCameraFocus();
        }

        private void Update()
        {
            RotationControl();
            ZoomControl();

            cameraFocus.transform.rotation = Quaternion.Lerp(
                cameraFocus.transform.rotation, rotation, Time.deltaTime * 5f);
        }

        private IEnumerator CheckFloorCenter()
        {
            WaitForSeconds delay = new WaitForSeconds(0.5f);

            while (true)
            {
                yield return null;

                for(int i = 0; i < floorCenterList.Count; i++)
                {
                    // var temp = Mathf.Abs(playerPos.position.y - floorCenterList[i].position.y);
                    float distance = Vector3.Distance(playerPos.position, floorCenterList[i].position);

                    if (distance < maxFloorCenterDistance)
                    {
                        currentFloorCenter = floorCenterList[i];
                        SetCameraFocus();
                    }
                }

                yield return delay;
            }
        }

        /// <summary>
        /// QŰ�� ������ ī�޶� ȸ����ų �� �ִ�.
        /// </summary>
        private void RotationControl()
        {
            if (Keyboard.current.qKey.isPressed)
            {
                Hun.Manager.CursorManager.Instance.CursorHideAction();
                Hun.Manager.CursorManager.Instance.ChangeCursor(Hun.Manager.CursorManager.CursorType.Arrow);

                var rotationDelta = this.rotationDelta * rotationSpeed;

                eulerAngle.x -= rotationDelta.y;
                eulerAngle.y += rotationDelta.x;

                eulerAngle.x = eulerAngle.x > 180 ? eulerAngle.x - 360 : eulerAngle.x;
                eulerAngle.x = Mathf.Clamp(eulerAngle.x, -20F, 60F);

                rotation = Quaternion.Euler(eulerAngle);
            }
            else
            {
                Hun.Manager.CursorManager.Instance.CursorShowAction();
            }
        }

        /// <summary>
        /// ���콺 �ٷ� ī�޶� Ȯ��, ����� �� �ִ�.
        /// </summary> 
        private void ZoomControl()
        {
            //float wheelInput = Mouse.current.scroll.ReadValue().y;
            var wheelInput = Input.mouseScrollDelta;

            if (wheelInput.y == 0)
            {
                // wheel �Է��� ���� �ð����� ������ wheelCount �ʱ�ȭ
                if (isRunningInitWheelCor == false)
                    StartCoroutine(InitializeWheelCor());
            }
            else
            {
                if (isRunningWheelCor)
                    return;

                currentWheelCount += wheelInput.y;

                if (currentWheelCount >= 4)
                {
                    if ((int)wheelState + 1 >= wheelStateLength)
                        wheelState = 0;
                    else
                        ++wheelState;

                    StopCoroutine(CameraZoomCor());
                    StartCoroutine(CameraZoomCor());
                }
                else if (currentWheelCount <= -4)
                {
                    if ((int)wheelState - 1 < 0)
                        wheelState = (WheelStateType)wheelStateLength - 1;
                    else
                        --wheelState;

                    StopCoroutine(CameraZoomCor());
                    StartCoroutine(CameraZoomCor());
                }
                else
                {
                    return;
                }

                SetCameraFocus();

                /*                currentWheelCount = 0;
                                isRunningWheelCor = false;

                                StopCoroutine(InitializeWheelCor());*/
            }
        }

        private void SetCameraFocus()
        {
            if (currentFloorCenter == null)
                return;

            var targetPos = (wheelState == WheelStateType.CenterTarget) ? currentFloorCenter : playerPos;
            cameraFocus.StartCoroutine(cameraFocus.SetTarget(targetPos));
            //cameraFocus.SetTarget();
        }

        /// <summary>
        /// �� ���¸� �ʱ�ȭ �Ѵ�.
        /// </summary>
        private IEnumerator InitializeWheelCor()
        {
            currentWheelCount = 0;
            isRunningInitWheelCor = true;

            yield return new WaitForSeconds(5f);

            isRunningInitWheelCor = false;
        }

        /// <summary>
        /// �ٽ� ���� �����µ� �����̸� �ش�.
        /// </summary>
        private IEnumerator RunningWheelCor()
        {
            isRunningWheelCor = true;

            if(wheelState == WheelStateType.CenterTarget)
                yield return new WaitForSeconds(2f);
            else
                yield return new WaitForSeconds(1f);

            isRunningWheelCor = false;
        }

        private IEnumerator CameraZoomCor()
        {
            // ���� ������ ������ �ٲ��� �ʾҴٸ�, ���� ������ ���ϵ��� ������.
            if (isRunningWheelCor == false)
                StartCoroutine(RunningWheelCor());

            while (true)
            {
                if (componentBase is CinemachineFramingTransposer)
                {
                    //var originDistance = (componentBase as CinemachineFramingTransposer).m_CameraDistance;
                    //(componentBase as CinemachineFramingTransposer).m_CameraDistance =
                    //Mathf.Lerp(originDistance, cameraDistances[(int)wheelState], zoomSpeed * Time.deltaTime);
                    //Mathf.SmoothStep(originDistance, cameraDistances[(int)wheelState], 0.5f);

                    var originSize = virtualCamera.m_Lens.OrthographicSize;
                    virtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(
                        originSize, cameraDistances[(int)wheelState], zoomSpeed * Time.deltaTime);

                    var gap = (componentBase as CinemachineFramingTransposer).m_CameraDistance - cameraDistances[(int)wheelState];
                    if (Mathf.Abs(gap) <= 0.05f)
                    {
                        (componentBase as CinemachineFramingTransposer).m_CameraDistance = cameraDistances[(int)wheelState];
                        break;
                    }

/*                    var gap = (componentBase as CinemachineFramingTransposer).m_CameraDistance - cameraDistances[(int)wheelState];
                    if (Mathf.Abs(gap) <= 0.05f)
                        break;*/
                }
                else // CinemachineVirtualCamera�� Body ������ Transposer���
                {
                    virtualCamera.m_Lens.FieldOfView = cameraDistances[(int)wheelState] * 10f;
                }

                yield return null;
            }
        }

        /// <summary>
        /// ȭ�� ȸ���� �߻��ϴ� �޼���
        /// </summary>
        /// <param name="inputValue">Delta</param>
        private void OnRotate(InputValue inputValue)
        {
            rotationDelta = inputValue.Get<Vector2>();
        }

        /// <summary>
        /// ī�޶� ����
        /// </summary>
        private void OnReset()
        {
            ResetRotation();
        }

        /// <summary>
        /// ȸ�� ���¸� �÷��̾� ���� �������� �����մϴ�.
        /// </summary>
        private void ResetRotation()
        {
            cameraFocus.ResetRotation();
            rotation = cameraFocus.transform.rotation;
            eulerAngle = rotation.eulerAngles;
        }
    }
}
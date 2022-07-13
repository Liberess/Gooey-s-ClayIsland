using UnityEngine;
using UnityEngine.SceneManagement;

namespace Hun.Obstacle
{
    public class StageObject : MonoBehaviour
    {
        [SerializeField] private string stageName;
        [SerializeField] private int stageNum;
        [SerializeField] private int requirementShineLampNum;

        [Header("== Material ==")]
        [SerializeField] private Material lockMat;
        [SerializeField] private Material OpenMat;

        private Renderer objRenderer;

        private bool isOpen = false;

        private void Awake()
        {
            objRenderer = GetComponent<Renderer>();
        }

        private void Start()
        {
            CheckShineLamp();
            ////시작 시 이벤트를 등록해 줍니다.
            //SceneManager.sceneLoaded += LoadedsceneEvent;
        }
        //private void LoadedsceneEvent(Scene scene, LoadSceneMode mode)
        //{
        //    CheckShineLamp();
        //}

        public void CheckShineLamp()
        {
            int value = Manager.DataManager.Instance.GameData.gameSaveFiles[0].prismPiece;

            if (requirementShineLampNum <= value)
            {
                objRenderer.material = OpenMat;
                isOpen = true;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (isOpen && other.CompareTag("Player"))
            {
                SceneManager.LoadScene(stageName);
            }
        }
    }
}
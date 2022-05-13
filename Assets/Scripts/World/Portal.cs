using UnityEngine;

namespace Hun.Obstacle
{
    public enum PortalType
    {
        Stage = 0,
        Level
    }

    public enum StageName
    {
        Stage0 = 0,
        Stage1,
        Stage2,
        Stage3,
        Stage4
    }

    public class Portal : MonoBehaviour
    {
        public enum EffectType
        {
            Enter = 0,
            Interact
        }

        [SerializeField] private PortalType portalType;
        public PortalType PortalType { get => portalType; }

        [SerializeField] private EffectType eftType;
        public EffectType EftType { get => eftType; }

        [SerializeField] private StageName stageName;
        public StageName StageName { get => stageName; }

        public Transform TargetPos { get; private set; }

        private void Start()
        {
            try
            {
                TargetPos = transform.Find("TargetPos");
                TargetPos.gameObject.GetComponent<MeshRenderer>().enabled = false;
            }
            catch
            {

            }
        }

        public void ActiveStagePortal()
        {
            string sceneName = stageName.ToString() + "Scene";
            Hun.Manager.GameManager.Instance.LoadScene(sceneName);
        }
    }
}
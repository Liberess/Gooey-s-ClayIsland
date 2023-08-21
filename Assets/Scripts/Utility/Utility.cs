using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Hun.Utility
{
    public static class Utility
    {
        /// <summary>
        /// center를 중심으로 distance만큼의 범위로
        /// areaMask에 포함되는 random한 좌표를 반환한다.
        /// </summary>
        public static Vector3 GetRandPointOnNavMesh(Vector3 center, float distance, int areaMask)
        {
            Vector3 randPos = Vector3.zero;
            NavMeshHit hit;

            for(int i = 0; i < 30; i++)
            {
                randPos = Random.insideUnitSphere * distance + center;

                if (NavMesh.SamplePosition(randPos, out hit, distance, areaMask))
                    return hit.position;
            }

            return Vector3.zero;
        }

        /// <summary>
        /// 평균(mean)과 표준편차(standard)를 통해
        /// 정규분포 난수를 생성한다.
        /// </summary>
        public static float GetRandNormalDistribution(float mean, float standard)
        {
            var x1 = Random.Range(0f, 1f);
            var x2 = Random.Range(0f, 1f);
            return mean + standard * (Mathf.Sqrt(-2.0f * Mathf.Log(x1)) * Mathf.Sin(2.0f * Mathf.PI * x2));
        }

        /// <summary>
        /// 임의의 확률을 선택한다.
        /// ex) bool epicItem = GCR(0.001) → 1/1000의 확률로 크리티컬이 뜬다.
        /// </summary>
        public static bool GetChanceResult(float chance)
        {
            if (chance < 0.0000001f)
                chance = 0.0000001f;

            bool success = false;
            int randAccuracy = 10000000; // 천만. 천만분의 chance의 확률이다.
            float randHitRange = chance * randAccuracy;

            int rand = Random.Range(1, randAccuracy + 1);
            if (rand <= randHitRange)
                success = true;

            return success;
        }

        /// <summary>
        /// 임의의 퍼센트 확률을 선택한다.
        /// ex) bool critical = GPCR(30) → 30% 확률로 크리티컬이 뜬다.
        /// </summary>
        public static bool GetPercentageChanceResult(float perChance)
        {
            if (perChance < 0.0000001f)
                perChance = 0.0000001f;

            perChance = perChance / 100;

            bool success = false;
            int randAccuracy = 10000000; // 천만. 천만분의 chance의 확률이다.
            float randHitRange = perChance * randAccuracy;

            int rand = Random.Range(1, randAccuracy + 1);
            if (rand <= randHitRange)
                success = true;

            return success;
        }
        
        public static GameObject GetNearestObjectByList(List<GameObject> list, Vector3 pos)
        {
            float minDistance = float.MaxValue;
            GameObject tempObj = null;

            foreach (var obj in list)
            {
                float tempDistance = Vector3.Distance(
                    pos, obj.transform.position);

                if (tempDistance <= minDistance)
                {
                    tempObj = obj;
                    minDistance = tempDistance;
                }
            }

            return tempObj;
        }
    }
}
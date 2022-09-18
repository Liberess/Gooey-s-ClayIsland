using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Hun.Utility
{
    public static class Utility
    {
        /// <summary>
        /// center�� �߽����� distance��ŭ�� ������
        /// areaMask�� ���ԵǴ� random�� ��ǥ�� ��ȯ�Ѵ�.
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
        /// ���(mean)�� ǥ������(standard)�� ����
        /// ���Ժ��� ������ �����Ѵ�.
        /// </summary>
        public static float GetRandNormalDistribution(float mean, float standard)
        {
            var x1 = Random.Range(0f, 1f);
            var x2 = Random.Range(0f, 1f);
            return mean + standard * (Mathf.Sqrt(-2.0f * Mathf.Log(x1)) * Mathf.Sin(2.0f * Mathf.PI * x2));
        }

        /// <summary>
        /// ������ Ȯ���� �����Ѵ�.
        /// ex) bool epicItem = GCR(0.001) �� 1/1000�� Ȯ���� ũ��Ƽ���� ���.
        /// </summary>
        public static bool GetChanceResult(float chance)
        {
            if (chance < 0.0000001f)
                chance = 0.0000001f;

            bool success = false;
            int randAccuracy = 10000000; // õ��. õ������ chance�� Ȯ���̴�.
            float randHitRange = chance * randAccuracy;

            int rand = Random.Range(1, randAccuracy + 1);
            if (rand <= randHitRange)
                success = true;

            return success;
        }

        /// <summary>
        /// ������ �ۼ�Ʈ Ȯ���� �����Ѵ�.
        /// ex) bool critical = GPCR(30) �� 30% Ȯ���� ũ��Ƽ���� ���.
        /// </summary>
        public static bool GetPercentageChanceResult(float perChance)
        {
            if (perChance < 0.0000001f)
                perChance = 0.0000001f;

            perChance = perChance / 100;

            bool success = false;
            int randAccuracy = 10000000; // õ��. õ������ chance�� Ȯ���̴�.
            float randHitRange = perChance * randAccuracy;

            int rand = Random.Range(1, randAccuracy + 1);
            if (rand <= randHitRange)
                success = true;

            return success;
        }
        
        public static GameObject GetNearestObjectByList(List<GameObject> list, Vector3 pos)
        {
            float minDistance = 1000.0f;
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
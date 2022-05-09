using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Hun.Entity.Enemy
{
    public class Enemy : LivingEntity
    {
        private enum State
        {
            Patrol = 0,     //순찰
            Tracking,       //추적
            AttackBegin, //공격 준비
            Attacking      //공격
        }

        [SerializeField] private State state;

        [SerializeField] private EnemyData myData;
        public EnemyData MyData { get => myData; }

        public LivingEntity targetEntity { get; private set; }
        private List<LivingEntity> targetEntityList = new List<LivingEntity>();

        private float turnSmoothVelocity;
        private float attackDistance;

        [Header("== Battle Setting ==")]
        [SerializeField] private Transform eyeTransform;
        public Transform EyeTransform { get => eyeTransform; }

        [SerializeField] private Transform attackRoot;
        public Transform AttackRoot { get => attackRoot; }

        [Header("== Reward Setting ==")]
        //[SerializeField] private Coin[] coinPrefabs = new Coin[3];
        [SerializeField] private float exp = 5f;

        private RaycastHit[] hits = new RaycastHit[10];
        private List<LivingEntity> lastAttackedTargets = new List<LivingEntity>();
        private bool hasTarget => targetEntity != null && !targetEntity.IsDead;

        private Vector3 prePos;
        private Vector3 nowPos;
        private float gap;

        private Animator anim;
        private NavMeshAgent navAgent;

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (attackRoot != null)
            {
                Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
                Gizmos.DrawSphere(attackRoot.position, myData.AttackRadius);
            }

            if (eyeTransform != null)
            {
                var leftEyeRotation = Quaternion.AngleAxis(-myData.FieldOfView * 0.5f, Vector3.up);
                var leftRayDirection = leftEyeRotation * transform.forward;
                Handles.color = new Color(1f, 1f, 1f, 0.2f);
                Handles.DrawSolidArc(eyeTransform.position, Vector3.up,
                    leftRayDirection, myData.FieldOfView, myData.ViewDistance);
            }
        }
#endif

        private void Awake()
        {
            anim = GetComponent<Animator>();
            navAgent = GetComponent<NavMeshAgent>();

            var attackPivot = attackRoot.position;
            attackPivot.y = transform.position.y;

            attackDistance = Vector3.Distance(transform.position, attackPivot) + myData.AttackRadius;

            navAgent.isStopped = false;
            navAgent.stoppingDistance = attackDistance;
            navAgent.speed = myData.PatrolSpeed;
        }

        private void Start()
        {
            prePos = transform.position;

            // 게임 오브젝트 활성화와 동시에 AI의 추적 루틴 시작
            StartCoroutine(UpdatePath());
            StartCoroutine(CheckStopProcess());
        }

        private void Update()
        {
            if (IsDead)
                return;

            nowPos = transform.position;

            if (state == State.Tracking)
            {
                var distance = Vector3.Distance(targetEntity.transform.position, transform.position);
                if (distance <= attackDistance)
                    BeginAttack();
            }

/*            if (navAgent.isStopped)
                anim.SetBool("isMove", false);
            else
                anim.SetBool("isMove", true);*/

            //anim.SetFloat("Speed", navAgent.desiredVelocity.magnitude);
        }

        private void FixedUpdate()
        {
            if (IsDead)
                return;

            if (state == State.AttackBegin || state == State.Attacking)
            {
                var lookRotation = Quaternion.LookRotation(targetEntity.transform.position - transform.position);
                var targetAngleY = lookRotation.eulerAngles.y;

                targetAngleY = Mathf.SmoothDampAngle
                    (transform.eulerAngles.y, targetAngleY, ref turnSmoothVelocity, myData.TurnSmoothTime);
                transform.eulerAngles = Vector3.up * targetAngleY;
            }

            if (state == State.Attacking)
            {
                var direction = transform.forward;
                var deltaDistance = navAgent.velocity.magnitude * Time.deltaTime;

                //움직이는 궤적에 있는 콜라이더 감지
                var size = Physics.SphereCastNonAlloc(attackRoot.position,
                    myData.AttackRadius, direction, hits, deltaDistance, myData.WhatIsTarget);

                for (int i = 0; i < size; i++)
                {
                    var attackTargetEntity = hits[i].collider.GetComponent<LivingEntity>();

                    if (attackTargetEntity != null && !lastAttackedTargets.Contains(attackTargetEntity))
                    {
                        var message = new DamageMessage();
                        message.dmgAmount = myData.Damage;
                        message.damager = gameObject;

                        if (hits[i].distance <= 0f)
                            message.hitPoint = attackRoot.position;
                        else
                            message.hitPoint = hits[i].point;

                        message.hitNormal = hits[i].normal;

                        attackTargetEntity.ApplyDamage(message);
                        lastAttackedTargets.Add(attackTargetEntity);
                        break;
                    }
                }
            }
        }

        public void Setup(int _heart, int _damage, float _runSpeed, float _patrolSpeed)
        {
            originHeart = _heart;
            Heart = _heart;

            myData.Setup(_heart, _damage, _runSpeed, _patrolSpeed);

            // 탐색 단계 애니메이션
            navAgent.speed = _patrolSpeed;
        }

        public void SetExp(float _exp) => exp = _exp;

        private IEnumerator UpdatePath()
        {
            while (!IsDead)
            {
                if (hasTarget)
                {
                    if (state == State.Patrol)
                    {
                        state = State.Tracking;
                        navAgent.speed = myData.RunSpeed;
                    }
                    navAgent.SetDestination(targetEntity.transform.position);
                }
                else
                {
                    prePos = transform.position;

                    if (targetEntity != null)
                        targetEntity = null;

                    if (state != State.Patrol)
                    {
                        state = State.Patrol;
                        navAgent.speed = myData.PatrolSpeed;
                    }

                    if (navAgent.remainingDistance <= 1f)
                    {
                        var patrolTargetPos = Utility.Utility.GetRandPointOnNavMesh(
                            transform.position, 20f, NavMesh.AllAreas);
                        navAgent.SetDestination(patrolTargetPos);
                    }

                    var colliders = Physics.OverlapSphere(
                        eyeTransform.position, myData.ViewDistance, myData.WhatIsTarget);

                    foreach (var collider in colliders)
                    {
                        if (!IsTargetOnSight(collider.transform))
                            continue;

                        var livingEntity = collider.GetComponent<LivingEntity>();

                        if (livingEntity != null && !livingEntity.IsDead)
                        {
                            targetEntity = livingEntity;
                            targetEntityList.Add(livingEntity);
                            break;
                        }
                    }
                }
                yield return new WaitForSeconds(0.05f);
            }
        }

        private IEnumerator CheckStopProcess()
        {
            yield return new WaitForSeconds(1f);

            nowPos = transform.position;
            gap = Vector3.Distance(prePos, nowPos);
            if (Mathf.Abs(gap) <= 0.1f)
            {
                var patrolTargetPos = Utility.Utility.GetRandPointOnNavMesh(
                    transform.position, 20f, NavMesh.AllAreas);
                navAgent.SetDestination(patrolTargetPos);
            }

            StartCoroutine(CheckStopProcess());
        }

        public override void ApplyDamage(DamageMessage dmgMsg)
        {
            base.ApplyDamage(dmgMsg);

            if (targetEntity == null) //아직 추격할 대상이 없는데, 공격을 당했다면
                targetEntity = dmgMsg.damager.GetComponent<LivingEntity>();

            //EffectManager.Instance.PlayHitEffect(dmgMsg.hitPoint, dmgMsg.hitNormal, transform, EffectType.Flesh);
            //AudioManager.Instance.PlaySFX("MonsterHit");
        }

        private bool IsTargetOnSight(Transform target)
        {
            // 눈의 위치에서 타겟을 향한 방향
            var direction = target.position - eyeTransform.position;

            // 높이(수직) 각도 차이는 신경쓰지 않기 위한 것!!!
            direction.y = eyeTransform.forward.y;

            if (Vector3.Angle(direction, eyeTransform.forward) > myData.FieldOfView * 0.5f)
                return false;

            RaycastHit hit;

            if (Physics.Raycast(eyeTransform.position, direction,
                out hit, myData.ViewDistance, myData.WhatIsTarget))
            {
                if (hit.transform == target)
                    return true;
            }

            return false;
        }

        #region Attack
        public void BeginAttack()
        {
            state = State.AttackBegin;
            navAgent.isStopped = true;
            anim.SetTrigger("doAttack");
        }

        public void EnableAttack()
        {
            state = State.Attacking;
            lastAttackedTargets.Clear();
        }

        public void DisableAttack()
        {
            if (hasTarget)
                state = State.Tracking;
            else
                state = State.Patrol;

            navAgent.isStopped = false;
        }
        #endregion

        public override void Die()
        {
            base.Die();

            navAgent.enabled = false;
            anim.applyRootMotion = true;
            anim.SetTrigger("doDie");

            //AudioManager.Instance.PlaySFX("MonsterDie");

            //GetComponent<Enemy>().enabled = false;
            gameObject.layer = 15;
        }
    }
}
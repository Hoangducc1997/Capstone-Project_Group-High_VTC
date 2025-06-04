using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using TMPro;

public class NpcAI : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform playerTransform;
    public Transform[] patrolPoints;
    public float talkRange = 10f;

    [Header("Dialogue UI")]
    public GameObject dialogueUI;
    public TMP_Text dialogueText;
    public List<string> dialogueLines;

    private Node rootNode;
    private Animator animator;

    private bool hasTalked = false;
    private bool isTalking = false;
    private bool isPlayerInRange = false;

    private int currentLineIndex = 0;

    private InputSystem_Actions inputActions;
    private Coroutine typingCoroutine;

    void Awake()
    {
        inputActions = new InputSystem_Actions();
        //inputActions.Player.Talk.performed += OnTalkPressed;
    }

    void OnEnable()
    {
        inputActions.Enable();
    }

    void OnDisable()
    {
        inputActions.Disable();
    }

    void Start()
    {
        dialogueUI.SetActive(false);
        rootNode = new Selector(new List<Node>
        {
            new Sequence(new List<Node>
            {
                new CheckPlayerDistance(playerTransform, transform, talkRange, () => !hasTalked),
                new ApproachPlayer(transform, playerTransform, agent, () => isPlayerInRange = true),
                new WaitForPlayerInput(() => isPlayerInRange = true) // 👈 Đảm bảo duy trì trạng thái
            }),

            new Patrol(agent, patrolPoints)
        });

    }

    void Update()
    {
        rootNode.Evaluate();
    }

    private void OnTalkPressed(InputAction.CallbackContext context)
    {
        if (!isPlayerInRange || hasTalked) return;

        if (!isTalking)
        {
            isTalking = true;
            currentLineIndex = 0;
            dialogueUI.SetActive(true);
            if (typingCoroutine != null) StopCoroutine(typingCoroutine);
            typingCoroutine = StartCoroutine(TypeLine(dialogueLines[currentLineIndex]));
        }
        else
        {
            currentLineIndex++;
            if (currentLineIndex >= dialogueLines.Count)
            {
                dialogueUI.SetActive(false);
                isTalking = false;
                hasTalked = true;
            }
            else
            {
                if (typingCoroutine != null) StopCoroutine(typingCoroutine);
                typingCoroutine = StartCoroutine(TypeLine(dialogueLines[currentLineIndex]));
            }
        }
    }

    private IEnumerator TypeLine(string line)
    {
        dialogueText.text = "";
        foreach (char c in line)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(0.02f);
        }
    }

    // --------------------------
    public class WaitForPlayerInput : Node
    {
        private System.Action markInRange;

        public WaitForPlayerInput(System.Action markInRange)
        {
            this.markInRange = markInRange;
        }

        public override NodeState Evaluate()
        {
            markInRange?.Invoke();
            return NodeState.SUCCESS;
        }
    }

    // --------------------------
    public class Patrol : Node
    {
        private NavMeshAgent agent;
        private Transform[] patrolPoints;
        private int currentIndex = 0;

        public Patrol(NavMeshAgent agent, Transform[] patrolPoints)
        {
            this.agent = agent;
            this.patrolPoints = patrolPoints;
        }

        public override NodeState Evaluate()
        {
            if (patrolPoints.Length == 0) return NodeState.FAILURE;

            if (!agent.hasPath || agent.remainingDistance < 0.5f)
            {
                currentIndex = (currentIndex + 1) % patrolPoints.Length;
                agent.SetDestination(patrolPoints[currentIndex].position);
            }

            return NodeState.RUNNING;
        }
    }

    // --------------------------
    public class ApproachPlayer : Node
    {
        private Transform npc;
        private Transform player;
        private NavMeshAgent agent;
        private System.Action onArrive;
        private float stopDistance = 2f;
        private bool hasArrived = false;

        public ApproachPlayer(Transform npc, Transform player, NavMeshAgent agent, System.Action onArrive)
        {
            this.npc = npc;
            this.player = player;
            this.agent = agent;
            this.onArrive = onArrive;
        }

        public override NodeState Evaluate()
        {
            float distance = Vector3.Distance(npc.position, player.position);

            if (distance > stopDistance)
            {
                agent.SetDestination(player.position);
                return NodeState.RUNNING;
            }
            else
            {
                agent.ResetPath();

                if (!hasArrived)
                {
                    onArrive?.Invoke(); // 👈 Gán isPlayerInRange = true đúng lúc
                    hasArrived = true;
                }

                Vector3 dir = (player.position - npc.position).normalized;
                dir.y = 0;
                if (dir != Vector3.zero)
                    npc.rotation = Quaternion.LookRotation(dir);


                return NodeState.SUCCESS;
            }
        }
    }

    // --------------------------
    public class CheckPlayerDistance : Node
    {
        private Transform player;
        private Transform npc;
        private float range;
        private System.Func<bool> condition;

        public CheckPlayerDistance(Transform player, Transform npc, float range, System.Func<bool> condition)
        {
            this.player = player;
            this.npc = npc;
            this.range = range;
            this.condition = condition;
        }

        public override NodeState Evaluate()
        {
            float dist = Vector3.Distance(player.position, npc.position);
            return (dist < range && condition()) ? NodeState.SUCCESS : NodeState.FAILURE;
        }
    }
}

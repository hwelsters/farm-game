using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathController : MonoBehaviour
{
    //SOME STRUCT DEFINITIONS
    [Serializable]
    struct TimeChunk
    {
        [Header("Movement")]
        public Vector2 startPosition;
        public Vector2 endPosition;

        [Header("Animation - Before")]
        public AnimationStates beforeAnimation;
        public Direction beforeDirection;

        [Header("Animation - After")]
        public AnimationStates afterAnimation;
        public Direction afterDirection;


        public float stopDuration;
    }

    [Serializable]
    struct Schedule { public TimeChunk[] timeChunks; }

    [SerializeField] private int[] weeklySchedule;

    //ENUMS
    public enum Direction
    {
        LEFT, 
        RIGHT,
        UP, 
        DOWN
    };

    public enum AnimationStates
    {
        IDLE = 0,
        MOVING = 1
    };

    //ARRAY IS IN ORDER 
    //DIRECTION ENUM VALUES WILL CORRELATE WITH THE ARRAY VALUES

    public static string[] animationState = new string[]
    {
        "Idle",
        "Moving"
    };

    public static Vector2[] directionVector2 = new Vector2[]
    {
        new Vector2(-1f, 0f),
        new Vector2(1f, 0f),
        new Vector2(0f, 1f),
        new Vector2(0f, -1f)
    };

    //VARIABLES
    [Header("Daily schedule")]
    [SerializeField] private Schedule[] schedules;

    [Header("Daily married schedule")]
    [SerializeField] private Schedule[] marriedSchedules;

    private int currentTimeChunk = 0;

    private bool walkInProgress = false;

    private const float WALK_SPEED = 0.5f;
    private const float RUN_SPEED = 1f;

    private float currentSpeed = 0.5f;

    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
        currentSpeed = WALK_SPEED;
    }

    private void FixedUpdate()
    {
        if (!walkInProgress && currentTimeChunk < GetChunkCount())
        {
            walkInProgress = true;
            StartCoroutine(Move());
        }
    }

    private IEnumerator Move()
    {
        Vector3 endPosition = GetEndPosition();
        transform.position = GetStartPosition();

        animator.SetBool(GetBeforeAnimation(), true);

        Vector2 beforeDirection = GetBeforeDirectionVector2();
        animator.SetFloat("Horizontal", beforeDirection.x);
        animator.SetFloat("Vertical", beforeDirection.y);

        while (( transform.position - endPosition ).sqrMagnitude > float.Epsilon)
        {
            Vector2 newPosition = Vector2.MoveTowards(transform.position, endPosition, currentSpeed * Time.deltaTime);
            transform.position = newPosition;
            yield return null;
        }

        Vector2 afterDirection = GetAfterDirectionVector2();
        animator.SetFloat("Horizontal", afterDirection.x);
        animator.SetFloat("Vertical", afterDirection.y);

        animator.SetBool(GetBeforeAnimation(), false);
        animator.SetBool(GetAfterAnimation(), true);

        yield return new WaitForSeconds(GetStopDuration());

        animator.SetBool(GetAfterAnimation(), false);

        currentTimeChunk++;
        walkInProgress = false;
    }

    //STOPS MOVING WHEN PLAYER IS CLOSE
    private void OnTriggerEnter2D (Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            //currentSpeed = 0f;
            return;
        }

        Hittable hittable = other.GetComponent<Hittable>();
        if (hittable != null)
        {
            hittable.GetDestroyed();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            currentSpeed = WALK_SPEED;
        }
    }
    
    //GET FUNCTIONS
    private int GetChunkCount()
    {
        return GetSchedule().timeChunks.Length;
    }

    private Vector3 GetStartPosition()
    {
        return (Vector3) GetChunk().startPosition;
    }

    private Vector3 GetEndPosition()
    {
        return (Vector3) GetChunk().endPosition;
    }

    private string GetBeforeAnimation()
    {
        return animationState[(int) GetChunk().beforeAnimation];
    }

    private string GetAfterAnimation()
    {
        return animationState[(int) GetChunk().afterAnimation];
    }

    private float GetStopDuration()
    {
        return GetChunk().stopDuration;
    }

    private Schedule GetSchedule()
    {
        return schedules[weeklySchedule[DayCycle.day % weeklySchedule.Length]];
    }

    private TimeChunk GetChunk()
    {
        return GetSchedule().timeChunks[currentTimeChunk];
    }

    private Vector2 GetBeforeDirectionVector2()
    {
        return directionVector2[(int) GetChunk().beforeDirection];
    }

    private Vector2 GetAfterDirectionVector2()
    {
        return directionVector2[(int) GetChunk().afterDirection];
    }
}

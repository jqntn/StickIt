using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIParticleAnimation : MonoBehaviour
{

    [Header("GAME OBJECT CANVAS_______________")]
    public RectTransform rawImage;
    public ParticleSystem ps;

    [Header("ANIMATION_____________________")]
    public bool hasRandomStart = true;
    [Tooltip("Start in the middle by default\nActivate it if you want the start to have an offset")]
    public bool hasRandomStartOffset = true;
    public StartPosition startPosition = StartPosition.UP;
    public float minSpeed = 100.0f;
    public float maxSpeed = 500.0f;
    public float minTrailMultiplier = .1f;
    public float maxTrailMultiplier = 10.0f;
    public AnimationCurve curve = new AnimationCurve();
    public AnimationCurve curveTrail = new AnimationCurve();
    public Vector2 min = new Vector2(-35.0f, -35.0f);
    public Vector2 max = new Vector2(35.0f, 35.0f);

    [Header("DEBUG_________________________")]
    [SerializeField] private Vector2 dimension = new Vector2(.0f, .0f);

    [SerializeField] private Vector2 startPos = new Vector2(.0f, 200.0f);
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private MoveDirection direction;
    [SerializeField] private Coroutine coroutine = null;
    [SerializeField] private float speed = 0.0f;

    private void OnEnable()
    {
        rectTransform = GetComponent<RectTransform>();

        float width = rawImage.rect.width / 2.0f;
        float height = rawImage.rect.height / 2.0f;
        dimension = new Vector2(max.x, max.y);

        if (hasRandomStart)
        {
            int rng = Random.Range(0, 4);
            switch (rng)
            {
                case 0: startPosition = StartPosition.UP; break;
                case 1: startPosition = StartPosition.DOWN; break;
                case 2: startPosition = StartPosition.LEFT; break;
                case 3: startPosition = StartPosition.RIGHT; break;
                default: startPosition = StartPosition.UP; break;
            }
        }

        SearchStartPos();
        rectTransform.anchoredPosition = startPos;
        SearchDirection();
    }
    private void Update()
    {
        Move();
        SwitchDirection();
        SwitchTrail();
    }

    private void SwitchTrail()
    {
        if(ps == null) { return; }

        var main = ps.main;
        var shape = ps.shape;
        float ratio = 0.0f;
        switch (direction)
        {
            case MoveDirection.UP:
                ratio = (shape.position.y + max.y) / (dimension.y * 2.0f);
                main.startSpeedMultiplier = Mathf.Lerp(minTrailMultiplier, maxTrailMultiplier, curveTrail.Evaluate(ratio));
                speed = Mathf.Lerp(minSpeed, maxSpeed, curve.Evaluate(ratio));
                break;
            case MoveDirection.DOWN:
                //ratio = Mathf.Abs(shape.position.y) / dimension.y;
                ratio = (shape.position.y + max.y) / (dimension.y * 2.0f);
                main.startSpeedMultiplier = Mathf.Lerp(maxTrailMultiplier, minTrailMultiplier, curveTrail.Evaluate(ratio));
                speed = Mathf.Lerp(minSpeed, maxSpeed, curve.Evaluate(ratio));
                break;
            case MoveDirection.LEFT:
                //ratio = Mathf.Abs(shape.position.x) / dimension.x;
                ratio = (shape.position.x + max.x) / (dimension.x * 2.0f);
                main.startSpeedMultiplier = Mathf.Lerp(maxTrailMultiplier, minTrailMultiplier, curveTrail.Evaluate(ratio));
                speed = Mathf.Lerp(minSpeed, maxSpeed, curve.Evaluate(ratio));
                break;
            case MoveDirection.RIGHT:
                //ratio = Mathf.Abs(shape.position.x) / dimension.x;
                ratio = (shape.position.x + max.x) / (dimension.x * 2.0f);
                main.startSpeedMultiplier = Mathf.Lerp(minTrailMultiplier, maxTrailMultiplier, curveTrail.Evaluate(ratio));
                speed = Mathf.Lerp(minSpeed, maxSpeed, curve.Evaluate(ratio));
                break;
        }

    }

    private void Move()
    {
        var shape = ps.shape;
        switch (direction)
        {
            case MoveDirection.RIGHT:
                shape.position = new Vector3(
                    Mathf.Clamp(shape.position.x + speed * Time.deltaTime, min.x, max.x),
                    shape.position.y,
                    shape.position.z);
                break;
            case MoveDirection.LEFT:
                shape.position = new Vector3(
                    Mathf.Clamp(shape.position.x - speed * Time.deltaTime, min.x, max.x),
                    shape.position.y,
                    shape.position.z);
                break;
            case MoveDirection.UP:
                shape.position = new Vector3(
                    shape.position.x,
                    Mathf.Clamp(shape.position.y + speed * Time.deltaTime, min.y, max.y),
                    shape.position.z);
                break;
            case MoveDirection.DOWN:
                shape.position = new Vector3(
                    shape.position.x,
                    Mathf.Clamp(shape.position.y - speed * Time.deltaTime, min.y, max.y),
                    shape.position.z);
                break;
        }
    }

    private void SwitchDirection()
    {
        var shape = ps.shape;
        switch (direction)
        {
            case MoveDirection.RIGHT:
                // if too much on the right > move DOWN
                if(shape.position.x >= max.x)
                {
                    direction = MoveDirection.DOWN;
                    shape.rotation = new Vector3(.0f, .0f, 90f);
                }
                break;
            case MoveDirection.LEFT:
                // if too much on the left > move UP
                if (shape.position.x <= min.x)
                {
                    direction = MoveDirection.UP;
                    shape.rotation = new Vector3(.0f, .0f, -90f);
                }
                break;
            case MoveDirection.UP:
                // if too much upward > move RIGHT
                if (shape.position.y >= max.y)
                {
                    direction = MoveDirection.RIGHT;
                    shape.rotation = new Vector3(.0f, .0f, 180f);
                }
                break;
            case MoveDirection.DOWN:
                // if too much downward > move LEFT
                if (shape.position.y <= min.y)
                {
                    direction = MoveDirection.LEFT;
                    shape.rotation = new Vector3(.0f, .0f, 0f);
                }
                break;
        }
    }
    /// <summary>
    ///         Search the start Pos of the particle depending of startPosition 
    /// </summary>
    private void SearchStartPos()
    {
        float offset = 0.1f;
        // slight +1 and -1 is to avoid conflict and changing direction immediatly
        switch (startPosition)
        {
            case StartPosition.UP:
                if (hasRandomStartOffset) { 
                    startPos.x = Random.Range(min.x + offset, max.x - offset); 
                }
                else { 
                    startPos.x = .0f; 
                }

                startPos.y = dimension.y;
                break;
            case StartPosition.DOWN:
                if (hasRandomStartOffset) { 
                    startPos.x = Random.Range(min.x + offset, max.x - offset); 
                }
                else { 
                    startPos.x = .0f; 
                }

                startPos.y = -dimension.y;
                break;
            case StartPosition.LEFT:
                startPos.x = dimension.x;
                if (hasRandomStartOffset) { 
                    startPos.y = Random.Range(min.y + offset, max.y - offset); 
                }
                else {
                    startPos.y = .0f; 
                }

                startPos.x = -dimension.x;
                break;
            case StartPosition.RIGHT:
                startPos.x = -dimension.x;
                if (hasRandomStartOffset) { 
                    startPos.y = Random.Range(min.y + offset, max.y - offset); 
                }
                else { 
                    startPos.y = .0f; 
                }

                startPos.x = dimension.x;
                break;
        }
    }

    /// <summary>
    ///         Get The direction the particle should move to
    /// </summary>
    private void SearchDirection()
    {
        switch (startPosition)
        {
            case StartPosition.UP:
                direction = MoveDirection.RIGHT;
                break;
            case StartPosition.DOWN:
                direction = MoveDirection.LEFT;
                break;
            case StartPosition.LEFT:
                direction = MoveDirection.UP;
                break;
            case StartPosition.RIGHT:
                direction = MoveDirection.DOWN;
                break;
        }
    }
}

public enum StartPosition
{
    UP,
    DOWN,
    LEFT,
    RIGHT,
}

public enum MoveDirection
{
   UP,
   DOWN,
   LEFT,
   RIGHT,
}

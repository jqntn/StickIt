using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// TO DO :
//      Make it move around
public class UIParticleAnimation : MonoBehaviour
{
    // open a bool for which sens you want to turn
    // don't forget to create one Effect for the winner and another for the loser
    // do a permanent effect arount each square (material emmisive)

    [Header("GAME OBJECT CANVAS_______________")]
    public RectTransform rawImage;


    [Header("ANIMATION_____________________")]
    public bool hasRandomStart = true;
    [Tooltip("Start in the middle by default\nActivate it if you want the start to have an offset")]
    public bool hasRandomStartOffset = true;
    public StartPosition startPosition = StartPosition.UP;
    public float speed = 100.0f;
    public Vector2 offset = new Vector2(10.0f, 10.0f);


    [Header("DEBUG_________________________")]
    [SerializeField] private Vector2 dimension = new Vector2(.0f, .0f);
    [SerializeField] private Vector2 min = new Vector2(.0f, .0f);
    [SerializeField] private Vector2 max = new Vector2(.0f, .0f);
    [SerializeField] private Vector2 startPos = new Vector2(.0f, 200.0f);
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private MoveDirection direction;
    [SerializeField] 

    private void OnEnable()
    {
        rectTransform = GetComponent<RectTransform>();

        float width = rawImage.rect.width / 2.0f;
        float height = rawImage.rect.height / 2.0f;
        dimension = new Vector2(width, height);
        min.x = rawImage.anchoredPosition.x - width;
        max.x = rawImage.anchoredPosition.x + width;
        min.y = rawImage.anchoredPosition.y - height;
        max.y = rawImage.anchoredPosition.y + height;

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
    }

    private void Move()
    {
        switch (direction)
        {
            case MoveDirection.RIGHT:
                rectTransform.anchoredPosition = new Vector2(
                    Mathf.Clamp(rectTransform.anchoredPosition.x + speed * Time.deltaTime, min.x - offset.x, max.x + offset.x),
                    rectTransform.anchoredPosition.y
                    );
                break;
            case MoveDirection.LEFT:

                rectTransform.anchoredPosition = new Vector2(
                    Mathf.Clamp(rectTransform.anchoredPosition.x - speed * Time.deltaTime, min.x - offset.x, max.x + offset.x),
                    rectTransform.anchoredPosition.y
                    );
                break;
            case MoveDirection.UP:
                rectTransform.anchoredPosition = new Vector2(
                    rectTransform.anchoredPosition.x,
                    Mathf.Clamp(rectTransform.anchoredPosition.y + speed * Time.deltaTime, min.y - offset.y, max.y + offset.y)
                    );
                break;
            case MoveDirection.DOWN:
                rectTransform.anchoredPosition = new Vector2(
                    rectTransform.anchoredPosition.x,
                    Mathf.Clamp(rectTransform.anchoredPosition.y - speed * Time.deltaTime, min.y - offset.y, max.y + offset.y)
                    );
                break;
        }
    }

    private void SwitchDirection()
    {
        switch (direction)
        {
            case MoveDirection.RIGHT:
                // if too much on the right
                if (rectTransform.anchoredPosition.x >= max.x + offset.x)
                {
                    direction = MoveDirection.DOWN;
                }
                break;
            case MoveDirection.LEFT:
                // if too much on the left
                if (rectTransform.anchoredPosition.x <= min.x - offset.x)
                {
                    direction = MoveDirection.UP;
                }
                break;
            case MoveDirection.UP:
                // if too much upward
                if (rectTransform.anchoredPosition.y >= max.y + offset.y)
                {
                    direction = MoveDirection.RIGHT;
                }
                break;
            case MoveDirection.DOWN:
                // if too much downward
                if (rectTransform.anchoredPosition.y <= min.y - offset.y)
                {
                    direction = MoveDirection.LEFT;
                }
                break;
        }
    }
    /// <summary>
    ///         Search the start Pos of the particle depending of startPosition 
    /// </summary>
    private void SearchStartPos()
    {
        // slight +1 and -1 is to avoid conflict and changing direction immediatly
        switch (startPosition)
        {
            case StartPosition.UP:
                if (hasRandomStartOffset) { 
                    startPos.x = Random.Range(min.x + 1, max.x - 1); 
                }
                else { 
                    startPos.x = .0f; 
                }

                startPos.y = dimension.y;
                break;
            case StartPosition.DOWN:
                if (hasRandomStartOffset) { 
                    startPos.x = Random.Range(min.x + 1, max.x - 1); 
                }
                else { 
                    startPos.x = .0f; 
                }

                startPos.y = -dimension.y;
                break;
            case StartPosition.LEFT:
                startPos.x = dimension.x;
                if (hasRandomStartOffset) { 
                    startPos.y = Random.Range(min.y + 1, max.y - 1); 
                }
                else {
                    startPos.y = .0f; 
                }

                startPos.x = -dimension.x;
                break;
            case StartPosition.RIGHT:
                startPos.x = -dimension.x;
                if (hasRandomStartOffset) { 
                    startPos.y = Random.Range(min.y + 1, max.y - 1); 
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

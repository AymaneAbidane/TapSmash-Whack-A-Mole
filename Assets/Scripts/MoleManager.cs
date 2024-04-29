using System.Collections;
using UnityEngine;

public class MoleManager : MonoBehaviour
{
    [Header("Graphics")]
    [SerializeField] private Sprite mole;
    [SerializeField] private Sprite moleHardHat;
    [SerializeField] private Sprite moleHatBroken;
    [SerializeField] private Sprite moleHit;
    [SerializeField] private Sprite moleHatHit;

    // The offset of the sprite to hide it.
    private Vector2 startPosition = new(0f, -0.8f);
    private Vector2 endPosition = Vector2.zero;
    // How long it takes to show a mole.
    private float showDuration = 0.5f;
    private float duration = 1f;
    private bool isHitable = true;

    public enum MoleType { Standard, HardHat, Bomb };
    private MoleType moleType;
    private float hardRate = 0.25f;
    private float bombRate = 0f;
    private int lives;
    private int moleIndex = 0;
    [SerializeField] private int timePenaltyValue = 5;
    [SerializeField] private int standardMoleScoreValue = 10;
    [SerializeField] private int hardMoleScoreValue = 20;
    [SerializeField] float yOffsetScoreFloatingText = .5f;
    [SerializeField] float yOffsetTimeFloatingText = 1.0f;

    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider2D;
    private Animator animator;
    private Vector2 boxOffset;
    private Vector2 boxSize;
    private Vector2 boxOffsetHidden;
    private Vector2 boxSizeHidden;

    private static GameManager _gameManagerInstance;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        // Work out collider values.
        boxOffset = boxCollider2D.offset;
        boxSize = boxCollider2D.size;
        boxOffsetHidden = new Vector2(boxOffset.x, -startPosition.y / 2f);
        boxSizeHidden = new Vector2(boxSize.x, 0f);

        if (_gameManagerInstance == null)
        {
            _gameManagerInstance = FindObjectOfType<GameManager>();
            if (_gameManagerInstance == null)
            {
                Debug.LogError("No GameManager found in the scene.");
            }
        }
    }

    public void Activate(int level)
    {
        SetLevel(level);
        CreateNext();
        StartCoroutine(ShowHide(startPosition, endPosition));
    }

    private void OnMouseDown()
    {
        if (isHitable)
        {
            switch (moleType)
            {
                case MoleType.Standard:
                    spriteRenderer.sprite = moleHit;
                    StopAllCoroutines();
                    StartCoroutine(QuickHide());
                    _gameManagerInstance.AddScore(moleIndex, standardMoleScoreValue);
                    Vector3 standardTextPosition = transform.position + new Vector3(0f, yOffsetScoreFloatingText, 0f); // yOffset is the distance above the object's center
                    _gameManagerInstance.ShowScoreFloatingText("+" + standardMoleScoreValue.ToString(), standardTextPosition);

                    isHitable = false;
                    break;
                case MoleType.HardHat:
                    if (lives == 2)
                    {
                        spriteRenderer.sprite = moleHatBroken;
                        lives--;
                    }
                    else
                    {
                        spriteRenderer.sprite = moleHatHit;
                        StopAllCoroutines();
                        StartCoroutine(QuickHide());
                        _gameManagerInstance.AddScore(moleIndex, hardMoleScoreValue);
                        Vector3 hardTextPosition = transform.position + new Vector3(0f, yOffsetScoreFloatingText, 0f); // yOffset is the distance above the object's center
                        _gameManagerInstance.ShowScoreFloatingText("+" + hardMoleScoreValue.ToString(), hardTextPosition);

                        isHitable = false;
                    }
                    break;
                case MoleType.Bomb:
                    _gameManagerInstance.GameOver(1);
                    break;
                default:
                    break;
            }
        }
    }


    private IEnumerator ShowHide(Vector2 start, Vector2 end)
    {
        // Make sure to start at the start.
        transform.localPosition = start;

        // Show the mole.
        float elapsed = 0f;
        while (elapsed < showDuration)
        {
            transform.localPosition = Vector2.Lerp(start, end, elapsed / showDuration);
            // interpelate the collider with lerp from hidden to show
            boxCollider2D.offset = Vector2.Lerp(boxOffsetHidden, boxOffset, elapsed / showDuration);
            boxCollider2D.size = Vector2.Lerp(boxSizeHidden, boxSize, elapsed / showDuration);
            // Update at max framerate.
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Make sure we're exactly at the end.
        transform.localPosition = end;
        // 
        boxCollider2D.offset = boxOffset;
        boxCollider2D.size = boxSize;

        // Wait for duration to pass.
        yield return new WaitForSeconds(duration);

        // Hide the mole.
        elapsed = 0f;
        while (elapsed < showDuration)
        {
            transform.localPosition = Vector2.Lerp(end, start, elapsed / showDuration);
            // interpelate the collider with lerp from show to hidden
            boxCollider2D.offset = Vector2.Lerp(boxOffset, boxOffsetHidden, elapsed / showDuration);
            boxCollider2D.size = Vector2.Lerp(boxSize, boxSizeHidden, elapsed / showDuration);
            // Update at max framerate.
            elapsed += Time.deltaTime;
            yield return null;
        }
        // Make sure we're exactly back at the start position.
        transform.localPosition = start;
        //
        boxCollider2D.offset = boxOffsetHidden;
        boxCollider2D.size = boxSizeHidden;

        // If we got to the end and it's still hitable then we missed it.
        if (isHitable)
        {
            isHitable = false;
            // We only give time penalty if it isn't a bomb.
            _gameManagerInstance.Missed(moleIndex, moleType != MoleType.Bomb);
            Vector3 timeTextPosition = transform.position + new Vector3(0f, yOffsetTimeFloatingText, 0f); // yOffset is the distance above the object's center
            _gameManagerInstance.ShowTimeFloatingText("-" + timePenaltyValue.ToString() + "s", timeTextPosition);

        }
    }

    private IEnumerator QuickHide()
    {
        yield return new WaitForSeconds(0.25f);

        if (!isHitable)
        {
            Hide();
        }
    }

    public void Hide()
    {
        // Set the mole parameters to hide it.
        transform.localPosition = startPosition;
        //Set Collider to Hidden
        boxCollider2D.offset = boxOffsetHidden;
        boxCollider2D.size = boxSizeHidden;
    }
    private void CreateNext()
    {
        float random = Random.Range(0f, 1f);
        if (random < bombRate)
        {
            moleType = MoleType.Bomb;
            animator.enabled = true;
        }
        else
        {
            animator.enabled = false;
            random = Random.Range(0f, 1f);
            if (random < hardRate)
            {
                //create hard one
                moleType = MoleType.HardHat;
                spriteRenderer.sprite = moleHardHat;
                lives = 2;
            }
            else
            {
                //create standard one
                moleType = MoleType.Standard;
                spriteRenderer.sprite = mole;
                lives = 1;
            }
        }
        // Mark as hittable to register an onclick event.
        isHitable = true;
    }
    // As the level progresses the game gets harder.
    private void SetLevel(int level)
    {
        float durationMin = Mathf.Clamp(1 - level * 0.1f, 0.5f, 1f);
        float durationMax = Mathf.Clamp(2 - level * 0.1f, 1f, 2f);
        duration = Random.Range(durationMin, durationMax);

        bombRate = Mathf.Min(level * 0.02f, 0.1f);

        hardRate = Mathf.Min(level * 0.015f, 0.5f);
    }


    // Used by the game manager to uniquely identify moles. 
    public void SetIndex(int index)
    {
        moleIndex = index;
    }

    // Used to freeze the game on finish.
    public void StopGame()
    {
        isHitable = false;
        StopAllCoroutines();
    }
}

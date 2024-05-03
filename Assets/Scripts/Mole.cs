using System;
using System.Collections;
using UnityEngine;

public class Mole : MonoBehaviour
{
    //private void OnEnable()
    //{
    //    MoleManager.Instance.RegisterMole(this);
    //}

    //private void OnDisable()
    //{
    //    MoleManager.Instance.UnregisterMole(this);
    //}
    public static Mole Instance { get; private set; }
    public enum MoleType { Standard, HardHat, Bomb };

    private MoleType moleType;

    [SerializeField] private Vector2 moleStartPosition = new(0f, -0.8f);
    [SerializeField] private Vector2 moleEndPosition = Vector2.zero;
    
    [SerializeField] private Sprite moleSprite;
    [SerializeField] private Sprite moleHardHatSprite;
    [SerializeField] private Sprite moleHatBrokenSprite;
    [SerializeField] private Sprite moleHitedSprite;
    [SerializeField] private Sprite moleHatHitedSprite;

    //[SerializeField] private int timePenaltyValue = 5;
    //[SerializeField] float yOffsetOfScoreFloatingText = 1.0f;
    //[SerializeField] float yOffsetOfTimeFloatingText = 1.0f;

    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider2D;
    private Animator animator;
    private Vector2 boxColliderOffset;
    private Vector2 boxColliderSize;
    private Vector2 boxColliderOffsetHidden;
    private Vector2 boxColliderSizeHidden;

    // How long it takes to show a mole
    private float showMoleDuration = .5f;
    private float showMoleFullAnimationDuration = 1f;
    private float hardMoleRate = 0.25f;
    private float bombRate = 0f;
    private int moleLives;
    private int moleIndex = 0;
    private bool isHitable = true;

    private void Awake()
    {
        Instance = this;
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        // Work out collider values.
        boxColliderOffset = boxCollider2D.offset;
        boxColliderSize = boxCollider2D.size;
        boxColliderOffsetHidden = new Vector2(boxColliderOffset.x, -moleStartPosition.y / 2f);
        boxColliderSizeHidden = new Vector2(boxColliderSize.x, 0f);
    }

    public void Activate(int level)
    {
        SetLevel(level);
        CreateNext();
        StartCoroutine(ShowHide(moleStartPosition, moleEndPosition));
    }
    
    private void OnMouseDown()
    {
        if (isHitable)
        {
            switch (moleType)
            {
                case MoleType.Standard:
                    spriteRenderer.sprite = moleHitedSprite;
                    GameManager.Instance.MoleHited(moleIndex);
                    StopAllCoroutines();
                    StartCoroutine(QuickHide());
                    isHitable = false;
                    break;
                case MoleType.HardHat:
                    if (moleLives == 2)
                    {
                        spriteRenderer.sprite = moleHatBrokenSprite;
                        moleLives--;
                    }
                    else
                    {
                        spriteRenderer.sprite = moleHatHitedSprite;
                        GameManager.Instance.MoleHited(moleIndex);
                        StopAllCoroutines();
                        StartCoroutine(QuickHide());
                        isHitable = false;
                    }
                    break;
                case MoleType.Bomb:
                    //GameManager.Instance.GameOver(1);
                    StopGame();
                    GameManager.Instance.GameOver();
                    break;
                default:
                    break;
            }
        }
    }


    private IEnumerator ShowHide(Vector2 moleStartingPosition, Vector2 moleEndingPosition)
    {
        // Make sure to start at the start.
        transform.localPosition = moleStartingPosition;

        // Show the mole.
        float elapsed = 0f;
        while (elapsed < showMoleDuration)
        {
            transform.localPosition = Vector2.Lerp(moleStartingPosition, moleEndingPosition, elapsed / showMoleDuration);
            // interpelate the collider with lerp from hidden to show
            boxCollider2D.offset = Vector2.Lerp(boxColliderOffsetHidden, boxColliderOffset, elapsed / showMoleDuration);
            boxCollider2D.size = Vector2.Lerp(boxColliderSizeHidden, boxColliderSize, elapsed / showMoleDuration);
            // Update at max framerate.
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Make sure we're exactly at the end.
        transform.localPosition = moleEndingPosition;
        /// interpelate the collider with lerp from show to hidden
        boxCollider2D.offset = boxColliderOffset;
        boxCollider2D.size = boxColliderSize;

        // Wait for duration to pass.
        yield return new WaitForSeconds(showMoleFullAnimationDuration);

        // Hide the mole.
        elapsed = 0f;
        while (elapsed < showMoleDuration)
        {
            transform.localPosition = Vector2.Lerp(moleEndingPosition, moleStartingPosition, elapsed / showMoleDuration);
            // interpelate the collider with lerp from show to hidden
            boxCollider2D.offset = Vector2.Lerp(boxColliderOffset, boxColliderOffsetHidden, elapsed / showMoleDuration);
            boxCollider2D.size = Vector2.Lerp(boxColliderSize, boxColliderSizeHidden, elapsed / showMoleDuration);
            // Update at max framerate.
            elapsed += Time.deltaTime;
            yield return null;
        }
        // Make sure we're exactly back at the start position.
        transform.localPosition = moleStartingPosition;
        //
        boxCollider2D.offset = boxColliderOffsetHidden;
        boxCollider2D.size = boxColliderSizeHidden;

        // If we got to the end and it's still hitable then we missed it.
        if (isHitable)
        {
            isHitable = false;
            if (moleType != MoleType.Bomb)
            {
                GameManager.Instance.MoleMissed(moleIndex, moleType != MoleType.Bomb);
            }
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
        transform.localPosition = moleStartPosition;
        //Set Collider to Hidden
        boxCollider2D.offset = boxColliderOffsetHidden;
        boxCollider2D.size = boxColliderSizeHidden;
    }
    private void CreateNext()
    {
        float random = UnityEngine.Random.Range(0f, 1f);
        if (random < bombRate)
        {
            moleType = MoleType.Bomb;
            animator.enabled = true;
        }
        else
        {
            animator.enabled = false;
            random = UnityEngine.Random.Range(0f, 1f);
            if (random < hardMoleRate)
            {
                //create hard one
                moleType = MoleType.HardHat;
                spriteRenderer.sprite = moleHardHatSprite;
                moleLives = 2;
            }
            else
            {
                //create standard one
                moleType = MoleType.Standard;
                spriteRenderer.sprite = moleSprite;
                moleLives = 1;
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

        showMoleFullAnimationDuration = UnityEngine.Random.Range(durationMin, durationMax);

        bombRate = Mathf.Min(level * 0.02f, 0.1f);
        hardMoleRate = Mathf.Min(level * 0.015f, 0.5f);
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

using System;
using System.Collections;
using UnityEngine;

public class Mole : MonoBehaviour {
    public static Mole Instance { get; private set; }
    public enum MoleType { Standard, HardHat, Bomb };

    private MoleType moleType;

    [SerializeField] private Vector2 moleStartPosition = new Vector2(0f, -0.8f);
    [SerializeField] private Vector2 moleEndPosition = Vector2.zero;

    [SerializeField] private Sprite moleSprite;
    [SerializeField] private Sprite moleHardHatSprite;
    [SerializeField] private Sprite moleHatBrokenSprite;
    [SerializeField] private Sprite moleHitedSprite;
    [SerializeField] private Sprite moleHatHitedSprite;

    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider2D;
    private Animator animator;
    private Vector2 boxColliderOffset;
    private Vector2 boxColliderSize;
    private Vector2 boxColliderOffsetHidden;
    private Vector2 boxColliderSizeHidden;

    private float showMoleDuration = 0.4f; // Reduced duration for faster display
    private float showMoleFullAnimationDuration = 0.8f; // Reduced duration for faster display
    private float hardMoleRate = 0.3f; // Increased rate of hard hat moles
    private float bombRate = 0.05f; // Increased rate of bomb moles
    private int moleLives;
    private int moleIndex = 0;
    private bool isHitable = true;

    private void Awake() {
        Instance = this;
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        boxColliderOffset = boxCollider2D.offset;
        boxColliderSize = boxCollider2D.size;
        boxColliderOffsetHidden = new Vector2(boxColliderOffset.x, -moleStartPosition.y / 2f);
        boxColliderSizeHidden = new Vector2(boxColliderSize.x, 0f);
    }

    public void Activate(int level) {
        SetLevel(level);
        CreateNext();
        StartCoroutine(ShowHide(moleStartPosition, moleEndPosition));
    }

    private void OnMouseDown() {
        if (isHitable) {
            switch (moleType) {
                case MoleType.Standard:
                    spriteRenderer.sprite = moleHitedSprite;
                    GameManager.Instance.MoleHited(moleIndex);
                    StopAllCoroutines();
                    StartCoroutine(QuickHide());
                    isHitable = false;
                    break;
                case MoleType.HardHat:
                    if (moleLives == 2) {
                        spriteRenderer.sprite = moleHatBrokenSprite;
                        moleLives--;
                    }
                    else {
                        spriteRenderer.sprite = moleHatHitedSprite;
                        GameManager.Instance.MoleHited(moleIndex);
                        StopAllCoroutines();
                        StartCoroutine(QuickHide());
                        isHitable = false;
                    }
                    break;
                case MoleType.Bomb:
                    StopGame();
                    GameManager.Instance.GameOver();
                    break;
                default:
                    break;
            }
        }
    }

    private IEnumerator ShowHide(Vector2 moleStartingPosition, Vector2 moleEndingPosition) {
        transform.localPosition = moleStartingPosition;

        float elapsed = 0f;
        while (elapsed < showMoleDuration) {
            transform.localPosition = Vector2.Lerp(moleStartingPosition, moleEndingPosition, elapsed / showMoleDuration);
            boxCollider2D.offset = Vector2.Lerp(boxColliderOffsetHidden, boxColliderOffset, elapsed / showMoleDuration);
            boxCollider2D.size = Vector2.Lerp(boxColliderSizeHidden, boxColliderSize, elapsed / showMoleDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = moleEndingPosition;
        boxCollider2D.offset = boxColliderOffset;
        boxCollider2D.size = boxColliderSize;

        yield return new WaitForSeconds(showMoleFullAnimationDuration);

        elapsed = 0f;
        while (elapsed < showMoleDuration) {
            transform.localPosition = Vector2.Lerp(moleEndingPosition, moleStartingPosition, elapsed / showMoleDuration);
            boxCollider2D.offset = Vector2.Lerp(boxColliderOffset, boxColliderOffsetHidden, elapsed / showMoleDuration);
            boxCollider2D.size = Vector2.Lerp(boxColliderSize, boxColliderSizeHidden, elapsed / showMoleDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = moleStartingPosition;
        boxCollider2D.offset = boxColliderOffsetHidden;
        boxCollider2D.size = boxColliderSizeHidden;

        if (isHitable) {
            isHitable = false;
            if (moleType != MoleType.Bomb) {
                GameManager.Instance.MoleMissed(moleIndex, moleType != MoleType.Bomb);
            }
        }
    }

    private IEnumerator QuickHide() {
        yield return new WaitForSeconds(0.25f);

        if (!isHitable) {
            Hide();
        }
    }

    public void Hide() {
        transform.localPosition = moleStartPosition;
        boxCollider2D.offset = boxColliderOffsetHidden;
        boxCollider2D.size = boxColliderSizeHidden;
    }

    private void CreateNext() {
        float random = UnityEngine.Random.Range(0f, 1f);
        if (random < bombRate) {
            moleType = MoleType.Bomb;
            animator.enabled = true;
        }
        else {
            animator.enabled = false;
            random = UnityEngine.Random.Range(0f, 1f);
            if (random < hardMoleRate) {
                moleType = MoleType.HardHat;
                spriteRenderer.sprite = moleHardHatSprite;
                moleLives = 2;
            }
            else {
                moleType = MoleType.Standard;
                spriteRenderer.sprite = moleSprite;
                moleLives = 1;
            }
        }
        isHitable = true;
    }

    private void SetLevel(int level) {
        float durationMin = Mathf.Clamp(1 - level * 0.1f, 0.2f, 0.8f); // Faster display time
        float durationMax = Mathf.Clamp(2 - level * 0.1f, 0.5f, 1.5f); // Faster display time

        showMoleFullAnimationDuration = UnityEngine.Random.Range(durationMin, durationMax);

        bombRate = Mathf.Min(level * 0.02f, 0.15f); // Increased bomb rate
        hardMoleRate = Mathf.Min(level * 0.015f, 0.6f); // Increased hard mole rate
    }

    public void SetIndex(int index) {
        moleIndex = index;
    }

    public void StopGame() {
        isHitable = false;
        StopAllCoroutines();
    }
}


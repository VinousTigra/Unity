using System.Collections;
using UnityEngine;

public class DogController : MonoBehaviour
{
    [SerializeField] private Animator animator;

    [Header("Animation")]
    [SerializeField] private string happyTrigger = "Happy";
    [SerializeField] private string leaveTrigger = "Leave";

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip barkClip;

    [Header("Leaving")]
    [SerializeField] private Transform leaveTarget;
    [SerializeField] private float leaveDuration = 3f;

    private bool hasLeft;
    private bool lastAnimatorActive;
    private RuntimeAnimatorController lastController;

    private void Awake()
    {
        animator = GetComponent<Animator>();

        Debug.Log($"DOG AWAKE: animator={animator}, enabled={animator != null && animator.enabled}, controller={animator?.runtimeAnimatorController}");
    }

    private void Update()
    {
        bool currentActive = animator != null && animator.isActiveAndEnabled;
        RuntimeAnimatorController currentController = animator != null
            ? animator.runtimeAnimatorController
            : null;

        if (currentActive != lastAnimatorActive || currentController != lastController)
        {
            Debug.Log($"DOG ANIM CHANGED: active={currentActive}, controller={currentController}");
            lastAnimatorActive = currentActive;
            lastController = currentController;
        }
    }

    private void OnEnable()
    {
        GameEvents.DishServed += OnDishServed;
    }

    private void OnDisable()
    {
        GameEvents.DishServed -= OnDishServed;
    }

    private void OnDishServed(DishResult result)
    {
        Debug.Log($"DOG EVENT: DishServed received. Success = {result.IsSuccessful}");

        if (hasLeft)
        {
            return;
        }

        if (result.IsSuccessful)
        {
            ReactHappy();
        }
        else
        {
            ReactFailure();
        }
    }

    private void ReactHappy()
    {
        Debug.Log($"DOG ANIM: active={animator != null && animator.isActiveAndEnabled}, controller={animator?.runtimeAnimatorController}, initialized={animator?.isInitialized}");

        if (animator != null)
        {
            animator.enabled = true;
            animator.SetTrigger(happyTrigger);
        }

        if (audioSource != null && barkClip != null)
            audioSource.PlayOneShot(barkClip);
    }

    private void ReactFailure()
    {
        hasLeft = true;

        if (animator != null)
        {
            animator.SetTrigger(leaveTrigger);
        }

        StartCoroutine(LeaveRoutine());
    }

    private IEnumerator LeaveRoutine()
    {
        if (leaveTarget == null) yield break;

        Vector3 start = transform.position;
        Vector3 target = leaveTarget.position;

        target.y = start.y;

        Vector3 direction = target - start;

        if (direction.sqrMagnitude > 0.001f)
            transform.rotation = Quaternion.LookRotation(-direction);

        float elapsed = 0f;

        while (elapsed < leaveDuration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / leaveDuration);
            transform.position = Vector3.Lerp(start, target, t);

            yield return null;
        }

        transform.position = target;

        // —обака ушла Ч пр€чем еЄ на 30 секунд.
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
            renderer.enabled = false;

        yield return new WaitForSeconds(30f);

        // ¬озвращаем собаку обратно.
        transform.position = start;

        foreach (Renderer renderer in renderers)
            renderer.enabled = true;

        hasLeft = false;

        if (animator != null)
            animator.Play("iddle", 0, 0f);

        GameEvents.ShowStatus("—обака вернулась");
    }
}
using System.Collections;
using UnityEngine;

public class DogController : MonoBehaviour
{
    [SerializeField]
    private Animator animator;

    [Header("Animation")]
    [SerializeField]
    private string happyTrigger = "Happy";

    [SerializeField]
    private string leaveTrigger = "Leave";

    [SerializeField]
    private string idleStateName = "Idle";

    [Header("Audio")]
    [SerializeField]
    private AudioSource audioSource;

    [SerializeField]
    private AudioClip barkClip;

    [Header("Leaving")]
    [SerializeField]
    private Transform leaveTarget;

    [SerializeField]
    private float leaveDuration = 3f;

    [SerializeField]
    private float returnDelay = 30f;

    private bool hasLeft;

    private void Awake()
    {
        if (animator == null)
        {
            animator =
                GetComponentInChildren<Animator>();
        }

        if (audioSource == null)
        {
            audioSource =
                GetComponent<AudioSource>();
        }
    }

    private void OnEnable()
    {
        GameEvents.DishServed +=
            OnDishServed;
    }

    private void OnDisable()
    {
        GameEvents.DishServed -=
            OnDishServed;
    }

    private void OnDishServed(
        DishResult result)
    {
        if (result == null ||
            hasLeft)
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
        if (animator != null)
        {
            animator.enabled = true;
            animator.SetTrigger(
                happyTrigger
            );
        }

        if (audioSource != null &&
            barkClip != null)
        {
            audioSource.PlayOneShot(
                barkClip
            );
        }
    }

    private void ReactFailure()
    {
        if (leaveTarget == null)
        {
            Debug.LogWarning(
                "DogController: Leave Target не назначен."
            );

            return;
        }

        hasLeft = true;

        if (animator != null)
        {
            animator.SetTrigger(
                leaveTrigger
            );
        }

        StartCoroutine(
            LeaveRoutine()
        );
    }

    private IEnumerator LeaveRoutine()
    {
        Vector3 start =
            transform.position;

        Quaternion startRotation =
            transform.rotation;

        Vector3 target =
            leaveTarget.position;

        target.y = start.y;

        Vector3 direction =
            target - start;

        if (direction.sqrMagnitude >
            0.001f)
        {
            transform.rotation =
                Quaternion.LookRotation(
                    -direction
                );
        }

        float elapsed = 0f;

        while (elapsed < leaveDuration)
        {
            elapsed += Time.deltaTime;

            float t =
                Mathf.Clamp01(
                    elapsed /
                    leaveDuration
                );

            transform.position =
                Vector3.Lerp(
                    start,
                    target,
                    t
                );

            yield return null;
        }

        transform.position = target;

        Renderer[] renderers =
            GetComponentsInChildren<Renderer>();

        foreach (Renderer renderer
                 in renderers)
        {
            renderer.enabled = false;
        }

        yield return new WaitForSeconds(
            returnDelay
        );

        transform.position = start;
        transform.rotation = startRotation;

        foreach (Renderer renderer
                 in renderers)
        {
            renderer.enabled = true;
        }

        hasLeft = false;

        if (animator != null &&
            !string.IsNullOrWhiteSpace(
                idleStateName))
        {
            animator.Play(
                idleStateName,
                0,
                0f
            );
        }

        GameEvents.ShowStatus(
            "Собака вернулась"
        );
    }
}
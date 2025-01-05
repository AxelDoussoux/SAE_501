using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using TomAg;

public class PlayerAgile : NetworkBehaviour
{
    private PlayerStrong strongPlayerInRange;
    private float holdTime = 0f;
    private const float REQUIRED_HOLD_TIME = 2f;
    private bool isHolding = false;
    private bool canCharge = true;
    private Image progressBar;
    private PlayerController playerController;

    private float targetFillAmount = 0f;
    private float currentFillAmount = 0f;
    private float fillSpeed = 3f;
    private float shrinkSpeed = 5f;
    private float cooldownTime = 2f;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsOwner)
        {
            playerController = GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.onAgileInteractStart += OnAgileInteractStart;
                playerController.onAgileInteractStop += OnAgileInteractStop;
            }

            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas != null)
            {
                progressBar = canvas.transform.Find("ChargeBar")?.GetComponent<Image>();
                if (progressBar != null)
                {
                    progressBar.gameObject.SetActive(false);
                }
                else
                {
                    Debug.LogError("ChargeBar non trouvée dans le Canvas!");
                }
            }
            else
            {
                Debug.LogError("Canvas non trouvé dans la scène!");
            }
        }
    }

    private void OnDisable()
    {
        if (IsOwner && playerController != null)
        {
            playerController.onAgileInteractStart -= OnAgileInteractStart;
            playerController.onAgileInteractStop -= OnAgileInteractStop;
        }
    }

    private void OnAgileInteractStart()
    {
        if (!canCharge || strongPlayerInRange == null) return;

        if (!isHolding)
        {
            isHolding = true;
            if (progressBar != null)
            {
                progressBar.gameObject.SetActive(true);
                currentFillAmount = 0f;
            }
        }
    }

    private void OnAgileInteractStop()
    {
        if (isHolding)
        {
            StartShrinking();
            isHolding = false;
        }
    }

    private void Update()
    {
        if (!IsOwner || !canCharge) return;

        if (strongPlayerInRange != null && isHolding)
        {
            holdTime += Time.deltaTime;
            targetFillAmount = holdTime / REQUIRED_HOLD_TIME;

            if (progressBar != null)
            {
                currentFillAmount = Mathf.Lerp(currentFillAmount, targetFillAmount, Time.deltaTime * fillSpeed);
                progressBar.fillAmount = currentFillAmount;

                if (currentFillAmount > 0.95f)
                {
                    float pulse = Mathf.PingPong(Time.time * 3f, 0.1f);
                    progressBar.transform.localScale = Vector3.one * (1f + pulse);
                }
            }

            if (holdTime >= REQUIRED_HOLD_TIME)
            {
                Debug.Log("Délai atteint, lancement !");
                strongPlayerInRange.LaunchPlayer();
                StartCooldown();
                ResetHoldState();
            }
        }
    }

    // Le reste du code reste inchangé...


private void StartCooldown()
    {
        canCharge = false;
        Invoke(nameof(ResetCanCharge), cooldownTime);
    }

    private void ResetCanCharge()
    {
        canCharge = true;
    }

    private void StartShrinking()
    {
        if (progressBar != null && progressBar.gameObject.activeSelf)
        {
            holdTime = 0f;
            isHolding = false;
            targetFillAmount = 0f;
            StartCoroutine(ShrinkProgressBar());
        }
    }

    private System.Collections.IEnumerator ShrinkProgressBar()
    {
        while (currentFillAmount > 0)
        {
            currentFillAmount = Mathf.Lerp(currentFillAmount, 0f, Time.deltaTime * shrinkSpeed);
            progressBar.fillAmount = currentFillAmount;
            progressBar.transform.localScale = Vector3.one;

            if (currentFillAmount < 0.01f)
            {
                progressBar.gameObject.SetActive(false);
                break;
            }
            yield return null;
        }
    }

    private void ResetHoldState()
    {
        holdTime = 0f;
        isHolding = false;
        targetFillAmount = 0f;
        currentFillAmount = 0f;

        if (progressBar != null && IsOwner)
        {
            progressBar.transform.localScale = Vector3.one;
            progressBar.gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerStrong strongPlayer = other.GetComponent<PlayerStrong>();
        if (strongPlayer != null)
        {
            strongPlayerInRange = strongPlayer;
            Debug.Log("Joueur fort détecté !");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerStrong strongPlayer = other.GetComponent<PlayerStrong>();
        if (strongPlayer != null && strongPlayerInRange == strongPlayer)
        {
            strongPlayerInRange = null;
            StartShrinking();
            Debug.Log("Joueur fort hors de portée !");
        }
    }
}
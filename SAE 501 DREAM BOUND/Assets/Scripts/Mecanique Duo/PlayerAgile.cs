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

    // Called when the NetworkBehaviour spawns on the client/host
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsOwner)
        {
            playerController = GetComponent<PlayerController>();
            if (playerController != null)
            {
                // Registering event listeners for interaction start and stop
                playerController.onAgileInteractStart += OnAgileInteractStart;
                playerController.onAgileInteractStop += OnAgileInteractStop;
            }

            // Finding the progress bar in the scene
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas != null)
            {
                progressBar = canvas.transform.Find("ChargeBar")?.GetComponent<Image>();
                if (progressBar != null)
                {
                    progressBar.gameObject.SetActive(false); // Hide the progress bar initially
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

    // Called when the component is disabled
    private void OnDisable()
    {
        if (IsOwner && playerController != null)
        {
            // Unregistering event listeners to prevent memory leaks
            playerController.onAgileInteractStart -= OnAgileInteractStart;
            playerController.onAgileInteractStop -= OnAgileInteractStop;
        }
    }

    // Called when the player starts interacting with the "PlayerStrong" object
    private void OnAgileInteractStart()
    {
        if (!canCharge || strongPlayerInRange == null) return;

        if (!isHolding)
        {
            isHolding = true;
            if (progressBar != null)
            {
                progressBar.gameObject.SetActive(true); // Show the progress bar
                currentFillAmount = 0f;
            }
        }
    }

    // Called when the player stops interacting with the "PlayerStrong" object
    private void OnAgileInteractStop()
    {
        if (isHolding)
        {
            StartShrinking();
            isHolding = false;
        }
    }

    // Called every frame to update the hold time and the progress bar
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
                    progressBar.transform.localScale = Vector3.one * (1f + pulse); // Add a pulsing effect to the progress bar
                }
            }

            if (holdTime >= REQUIRED_HOLD_TIME)
            {
                Debug.Log("Délai atteint, lancement !");
                strongPlayerInRange.LaunchPlayer(); // Launch the strong player
                StartCooldown();
                ResetHoldState();
            }
        }
    }

    // Starts a cooldown period before the next charge can occur
    private void StartCooldown()
    {
        canCharge = false;
        Invoke(nameof(ResetCanCharge), cooldownTime);
    }

    // Resets the charge flag after the cooldown
    private void ResetCanCharge()
    {
        canCharge = true;
    }

    // Starts the shrinking effect of the progress bar when the interaction stops
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

    // Shrinks the progress bar over time when the interaction is interrupted
    private System.Collections.IEnumerator ShrinkProgressBar()
    {
        while (currentFillAmount > 0)
        {
            currentFillAmount = Mathf.Lerp(currentFillAmount, 0f, Time.deltaTime * shrinkSpeed);
            progressBar.fillAmount = currentFillAmount;
            progressBar.transform.localScale = Vector3.one;

            if (currentFillAmount < 0.01f)
            {
                progressBar.gameObject.SetActive(false); // Hide the progress bar when it's empty
                break;
            }
            yield return null;
        }
    }

    // Resets the state of the interaction
    private void ResetHoldState()
    {
        holdTime = 0f;
        isHolding = false;
        targetFillAmount = 0f;
        currentFillAmount = 0f;

        if (progressBar != null && IsOwner)
        {
            progressBar.transform.localScale = Vector3.one;
            progressBar.gameObject.SetActive(false); // Hide the progress bar
        }
    }

    // Called when the player enters the trigger area of the "PlayerStrong" object
    private void OnTriggerEnter(Collider other)
    {
        PlayerStrong strongPlayer = other.GetComponent<PlayerStrong>();
        if (strongPlayer != null)
        {
            strongPlayerInRange = strongPlayer;
            Debug.Log("Joueur fort détecté !");
        }
    }

    // Called when the player exits the trigger area of the "PlayerStrong" object
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

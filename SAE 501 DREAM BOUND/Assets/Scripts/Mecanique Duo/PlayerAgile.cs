using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class PlayerAgile : NetworkBehaviour
{
    private PlayerStrong strongPlayerInRange;
    private float holdTime = 0f;
    private const float REQUIRED_HOLD_TIME = 2f;
    private bool isHolding = false;
    private bool canCharge = true; // Permet de vérifier si un saut peut être chargé
    private Image progressBar;

    // Variables pour l'effet de progression
    private float targetFillAmount = 0f;
    private float currentFillAmount = 0f;
    private float fillSpeed = 3f; // Vitesse de l'effet de remplissage
    private float shrinkSpeed = 5f; // Vitesse de l'effet de réduction
    private float cooldownTime = 2f; // Temps de cooldown après un saut

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsOwner)
        {
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

    private void Update()
    {
        if (!IsOwner || !canCharge) return; // Vérifier si un saut peut être chargé

        if (strongPlayerInRange != null)
        {
            if (Input.GetKey(KeyCode.F))
            {
                if (!isHolding)
                {
                    isHolding = true;
                    if (progressBar != null)
                    {
                        progressBar.gameObject.SetActive(true);
                        currentFillAmount = 0f;
                    }
                }

                holdTime += Time.deltaTime;
                targetFillAmount = holdTime / REQUIRED_HOLD_TIME;

                if (progressBar != null)
                {
                    // Animation fluide de remplissage
                    currentFillAmount = Mathf.Lerp(currentFillAmount, targetFillAmount, Time.deltaTime * fillSpeed);
                    progressBar.fillAmount = currentFillAmount;

                    // Effet de pulsation lorsque proche du remplissage
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
                    StartCooldown(); // Déclencher le cooldown
                    ResetHoldState();
                }
            }
            else if (Input.GetKeyUp(KeyCode.F))
            {
                StartShrinking();
            }
        }
    }

    private void StartCooldown()
    {
        canCharge = false; // Désactiver la possibilité de charger un nouveau saut
        Invoke(nameof(ResetCanCharge), cooldownTime); // Réactiver après le délai
    }

    private void ResetCanCharge()
    {
        canCharge = true; // Réactiver la possibilité de charger un saut
    }

    private void StartShrinking()
    {
        if (progressBar != null && progressBar.gameObject.activeSelf)
        {
            holdTime = 0f;
            isHolding = false;
            targetFillAmount = 0f;

            // Commencer la coroutine de réduction progressive
            StartCoroutine(ShrinkProgressBar());
        }
    }

    private System.Collections.IEnumerator ShrinkProgressBar()
    {
        while (currentFillAmount > 0)
        {
            currentFillAmount = Mathf.Lerp(currentFillAmount, 0f, Time.deltaTime * shrinkSpeed);
            progressBar.fillAmount = currentFillAmount;
            progressBar.transform.localScale = Vector3.one; // Réinitialiser l'échelle

            // Si la barre est presque vide, la désactiver
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

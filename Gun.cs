using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public GameObject Bullet;
    public Transform shootPoint;
    public float Force = 20f;
    public AudioSource shootSound;
    public float fireRate = 0.1f;
    public GameObject muzzleFlash;

    public AnimationClip shootingAnimation;
    public AnimationClip reloadingAnimation;

    public Transform startAimPosition;
    public Transform aimPosition;

    private bool safety = false;
    private bool canShoot = true; // Flag to control shooting
    private float cooldownTimer = 0f; // Timer to track the cooldown between shots

    private bool isAiming = false; // Flag to indicate aiming
    private Vector3 originalPosition; // Original position of the gun

    private Animator gunAnimator; // Animator component for playing animations

    private bool isReloading = false; // Flag to indicate reloading state

    private Coroutine aimCoroutine; // Coroutine reference for aiming movement

    void Start()
    {
        originalPosition = transform.localPosition;
        gunAnimator = GetComponent<Animator>();
    }

    void Update()
    {
        // Update the cooldown timer
        if (!canShoot)
        {
            cooldownTimer += Time.deltaTime;

            // Check if the cooldown time has passed the fire rate delay
            if (cooldownTimer >= fireRate)
            {
                canShoot = true;
                cooldownTimer = 0f;
            }
        }

        // Toggle aiming when right mouse button is pressed
        if (Input.GetMouseButtonDown(1))
        {
            if (!isReloading)
            {
                isAiming = !isAiming;

                if (isAiming)
                {
                    aimCoroutine = StartCoroutine(Aim(aimPosition.position));
                }
                else
                {
                    if (aimCoroutine != null)
                        StopCoroutine(aimCoroutine);

                    aimCoroutine = StartCoroutine(Aim(startAimPosition.position));
                }
            }
        }

        if (Input.GetMouseButton(0) && canShoot && !isReloading)
        {
            if (safety == false)
            {
                Fire();
            }

            if (safety == true)
            {
                return;
            }
        }

        if (Input.GetKeyDown(KeyCode.R) && canShoot && !isReloading)
        {
            Reload();
        }
    }

    void Fire()
    {
        GameObject bullet = Instantiate(Bullet, shootPoint.position, shootPoint.rotation);
        Rigidbody bulletRigidbody = bullet.GetComponent<Rigidbody>();
        bulletRigidbody.AddForce(shootPoint.forward * Force, ForceMode.Impulse);

        shootSound.Play();
        canShoot = false;
        cooldownTimer = 0f;

        // Trigger shooting animation
        if (shootingAnimation != null)
        {
            gunAnimator.Play(shootingAnimation.name, 0, 0); // Play from the beginning of the animation
        }
    }

    void Reload()
    {
        if (!isReloading)
        {
            // Perform the reloading logic here
            Debug.Log("Reloading...");

            // Trigger reloading animation
            if (reloadingAnimation != null)
            {
                gunAnimator.Play(reloadingAnimation.name);
            }

            isReloading = true;
            StartCoroutine(CompleteReload());
        }
    }

    IEnumerator CompleteReload()
    {
        yield return new WaitForSeconds(reloadingAnimation.length);
        isReloading = false;
        canShoot = true;
    }

    IEnumerator Aim(Vector3 targetPosition)
    {
        float duration = 0.2f;
        float elapsed = 0f;
        Vector3 initialPosition = transform.localPosition;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            transform.localPosition = Vector3.Lerp(initialPosition, targetPosition, t);
            yield return null;
        }

        transform.localPosition = targetPosition;
    }
}

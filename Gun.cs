using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform bulletSpawnPoint;
    public float bulletForce = 20f;
    public AudioSource shootSound;
    public float fireRate = 0.1f;
    public GameObject muzzleFlash;
    public GameObject silencer;
    public AudioSource silencedShootSound;
    public Animator gunAnimator;
    public AnimationClip shootAnimation;
    public AnimationClip aimAnimation;
    public AnimationClip reloadAnimation;

    private bool safety = false;
    private bool silenced = false;
    private bool canShoot = true;
    private float cooldownTimer = 0f;

    private int shootAnimationHash;
    private int aimAnimationHash;
    private int reloadAnimationHash;

    private void Awake()
    {
        shootAnimationHash = Animator.StringToHash(shootAnimation.name);
        aimAnimationHash = Animator.StringToHash(aimAnimation.name);
        reloadAnimationHash = Animator.StringToHash(reloadAnimation.name);
    }

    private void Update()
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

        if (Input.GetMouseButton(0) && canShoot)
        {
            if (safety == false)
            {
                Fire();
            }
            else
            {
                return;
            }
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            silenced = !silenced;
            silencer.SetActive(silenced);
            muzzleFlash.SetActive(!silenced);
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            safety = !safety;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Reload();
        }
    }

    private void Fire()
    {
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
        Rigidbody bulletRigidbody = bullet.GetComponent<Rigidbody>();
        bulletRigidbody.AddForce(bulletSpawnPoint.forward * bulletForce, ForceMode.Impulse);

        if (silenced)
        {
            silencedShootSound.Play();
        }
        else
        {
            shootSound.Play();
        }

        canShoot = false;
        cooldownTimer = 0f;

        // Set the shooting animation state to loop
        gunAnimator.Play(shootAnimationHash, -1, 0f);
        gunAnimator.SetBool(shootAnimationHash, true);

        // Stop aim and reload animations if playing
        gunAnimator.SetBool(aimAnimationHash, false);
        gunAnimator.SetBool(reloadAnimationHash, false);
    }

    private void Reload()
    {
        // Play reload animation
        gunAnimator.Play(reloadAnimationHash);

        // Stop aim and shooting animations if playing
        gunAnimator.SetBool(aimAnimationHash, false);
        gunAnimator.SetBool(shootAnimationHash, false);

        // Add your reloading logic here
        // This is where you can handle the reloading process, such as updating ammo count, playing reload sound, etc.
    }

    public void StartAimAnimation()
    {
        // Start aim animation
        gunAnimator.SetBool(aimAnimationHash, true);

        // Stop shooting animation if playing
        gunAnimator.SetBool(shootAnimationHash, false);
    }

    public void StopAimAnimation()
    {
        // Stop aim animation
        gunAnimator.SetBool(aimAnimationHash, false);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FingerBangScript : MonoBehaviour
{
    public AudioClip[] soundEffects;
    AudioSource audioSource;

    Animator deezHandsAnimator;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        deezHandsAnimator = transform.Find("bonbon_hands").gameObject.GetComponent<Animator>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            int clipIndex = (int)Random.Range(0, soundEffects.Length);
            audioSource.PlayOneShot(soundEffects[clipIndex]);

            deezHandsAnimator.SetTrigger("MouseClicked");
        }
    }
}

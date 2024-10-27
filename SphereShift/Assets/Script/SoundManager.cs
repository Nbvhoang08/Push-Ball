using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Unity.Collections.AllocatorManager;
using UnityEngine.UIElements;

namespace Script
{
    public class SoundManager : Singleton<SoundManager>
    {

        [Header("Audio Sources")]
        [SerializeField] private AudioSource musicSource; // Cho background music
        [SerializeField] private AudioSource sfxSource;   // Cho sound effects

        [Header("Audio Clips")]
        [SerializeField] private AudioClip backgroundMusic;
        [SerializeField] private AudioClip clickSound;
        [SerializeField] private AudioClip moveSound;
        [SerializeField] private AudioClip PopUpSound;

        [Header("Volume Settings")]
        [SerializeField][Range(0f, 1f)] private float musicVolume = 0.3f;
        [SerializeField][Range(0f, 1f)] private float sfxVolume = 0.5f;

       protected override void Awake()
       {
            base.Awake();
            InitializeAudio();
       }

        private void InitializeAudio()
        {
            // Tạo và thiết lập AudioSource cho music nếu chưa có
            if (musicSource == null)
            {
                musicSource = gameObject.AddComponent<AudioSource>();
                musicSource.loop = true;
                musicSource.playOnAwake = true;
            }

            // Tạo và thiết lập AudioSource cho sfx nếu chưa có
            if (sfxSource == null)
            {
                sfxSource = gameObject.AddComponent<AudioSource>();
                sfxSource.loop = false;
                sfxSource.playOnAwake = false;
            }

            // Thiết lập volume
            musicSource.volume = musicVolume;
            sfxSource.volume = sfxVolume;

            // Bắt đầu phát nhạc nền
            if (backgroundMusic != null)
            {
                musicSource.clip = backgroundMusic;
                musicSource.Play();
            }
        }

        public void PlayClickSound()
        {
            if (clickSound != null)
            {
                sfxSource.PlayOneShot(clickSound, sfxVolume);
            }
        }

        public void PlayMoveSound()
        {
            if (moveSound != null)
            {
                sfxSource.PlayOneShot(moveSound, sfxVolume);
            }
        }


        public void PlayPopUpSound()
        {
            if (PopUpSound != null)
            {
                sfxSource.PlayOneShot(PopUpSound, sfxVolume);
            }
        }

        // Các phương thức điều chỉnh âm lượng
        public void SetMusicVolume(float volume)
        {
            musicVolume = Mathf.Clamp01(volume);
            if (musicSource != null)
            {
                musicSource.volume = musicVolume;
            }
        }

        public void SetSFXVolume(float volume)
        {
            sfxVolume = Mathf.Clamp01(volume);
            if (sfxSource != null)
            {
                sfxSource.volume = sfxVolume;
            }
        }

        public void ToggleMusic(bool isOn)
        {
            if (musicSource != null)
            {
                if (isOn)
                {
                    musicSource.UnPause();
                }
                else
                {
                    musicSource.Pause();
                }
            }
        }

        public void ToggleSFX(bool isOn)
        {
            if (sfxSource != null)
            {
                sfxSource.mute = !isOn;
            }
        }
    }

}


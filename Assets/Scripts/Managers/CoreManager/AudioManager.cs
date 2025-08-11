namespace HDU.Managers
{
    using Cysharp.Threading.Tasks;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    public class AudioManager : IManager
    {
        private AudioSource _bgmAudioSource = new AudioSource();
        private AudioSource[] _efAudioSources = new AudioSource[EFAUDIONMAXCOUNT];
        private Dictionary<AudioClip, AudioSource> _dictPlayingClips = new Dictionary<AudioClip, AudioSource>();

        private const int EFAUDIONMAXCOUNT = 20;
        public const float MAXFXVOLUME = 0.3f;
        public const float MAXBGMVOLUME = 0.3f;

        public float MasterVolume { get; private set; } = 1.0f;
        public float FXVolume { get; private set; } = 1.0f;
        public float BGMVolume { get; private set; } = 1.0f;

        public void Init()
        {
            GameObject root = GameObject.Find("#Sound");
            if (root == null)
            {
                root = new GameObject { name = "#Sound" };
                UnityEngine.Object.DontDestroyOnLoad(root);

                GameObject bgmGO = new GameObject { name = "BGMPlayer" };
                GameObject efGO = new GameObject { name = "EFPlayer" };

                bgmGO.transform.SetParent(root.transform);
                efGO.transform.SetParent(root.transform);

                _bgmAudioSource = bgmGO.AddComponent<AudioSource>();
                _bgmAudioSource.loop = true;

                for (int i = 0; i < EFAUDIONMAXCOUNT; i++)
                {
                    _efAudioSources[i] = efGO.AddComponent<AudioSource>();
                }
            }
        }

        public void Clear()
        {

        }

        // ���� admob ���� ����Ƽ ��������� ũ���� �̽������� ����
        // https://github.com/googleanalytics/google-analytics-plugin-for-unity/issues/166
        public void SetEnableAudioSource(bool isActive)
        {
            _bgmAudioSource.enabled = isActive;
            for (int i = 0; i < _efAudioSources.Length; i++)
                _efAudioSources[i].enabled = isActive;
        }

        public void SetVolume(Define.CoreDefine.ESoundType type, float volume)
        {
            switch (type)
            {
                case Define.CoreDefine.ESoundType.MasterVolume:
                    MasterVolume = volume;
                    SetVolume(Define.CoreDefine.ESoundType.Bgm, BGMVolume);
                    SetVolume(Define.CoreDefine.ESoundType.FX, FXVolume);
                    break;
                case Define.CoreDefine.ESoundType.Bgm:
                    BGMVolume = volume;
                    _bgmAudioSource.volume = BGMVolume * MasterVolume;
                    break;
                case Define.CoreDefine.ESoundType.FX:
                    FXVolume = volume;
                    for (int i = 0; i < EFAUDIONMAXCOUNT; i++)
                        _efAudioSources[i].volume = FXVolume * MasterVolume;
                    break;
            }
        }

        /// <summary>
        /// ���� �̿��� �Ҵ�Ǿ��ִ� ���� ���
        /// </summary>
        /// <param name="audioClip"> ���� ������ </param>
        /// <param name="type"> ���� Ÿ�� </param>
        /// <param name="pitch"> ���� ������</param>
        public void Play(AudioClip audioClip, Define.CoreDefine.ESoundType type = Define.CoreDefine.ESoundType.FX, float pitch = 1.0f, float volumeRatio = 1)
        {
            if (audioClip == null)
                return;

            if (type == Define.CoreDefine.ESoundType.Bgm)
            {
                AudioSource audioSource = _bgmAudioSource;

                if (audioSource.isPlaying)
                    audioSource.Stop();

                audioSource.pitch = pitch;
                audioSource.clip = audioClip;
                audioSource.Play();
            }
            else
            {
                if (FXVolume <= 0 ) return;

                if (_dictPlayingClips.ContainsKey(audioClip))
                    return;
                else
                {
                    AudioSource sourceToUser = null;

                    foreach (var source in _efAudioSources)
                    {
                        if (!source.isPlaying)
                        {
                            sourceToUser = source;
                            break;
                        }
                    }
                    if (sourceToUser != null)
                    {
                        sourceToUser.volume = FXVolume * volumeRatio;
                        sourceToUser.pitch = pitch;
                        sourceToUser.PlayOneShot(audioClip);
                        _dictPlayingClips.Add(audioClip, sourceToUser);

                        RemoveClipAfterPlay(audioClip).Forget();
                    }
                }
                return;
            }
        }

        private async UniTask RemoveClipAfterPlay(AudioClip clip)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(0.1f));
            _dictPlayingClips.Remove(clip);
        }

        public void Resume(Define.CoreDefine.ESoundType type)
        {
            switch (type)
            {
                case Define.CoreDefine.ESoundType.Bgm:
                    _bgmAudioSource.volume = BGMVolume;
                    break;
                case Define.CoreDefine.ESoundType.FX:
                    for (int i = 0; i < _efAudioSources.Length; i++)
                        _efAudioSources[i].volume = FXVolume;
                    break;
            }

        }
        public void Stop(Define.CoreDefine.ESoundType type)
        {
            switch (type)
            {
                case Define.CoreDefine.ESoundType.Bgm:
                    _bgmAudioSource.volume = 0f;
                    break;
                case Define.CoreDefine.ESoundType.FX:
                    for (int i = 0; i < _efAudioSources.Length; i++)
                        _efAudioSources[i].volume = 0f;
                    break;
            }
        }

    }
}
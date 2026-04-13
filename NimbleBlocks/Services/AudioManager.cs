using System;
using System.IO;
using System.Reflection;
using Plugin.SimpleAudioPlayer;

namespace NimbleBlocks
{
    /// <summary>
    /// Centralized audio manager for handling music and sound effects across the app
    /// </summary>
    public class AudioManager
    {
        private static AudioManager _instance;
        private ISimpleAudioPlayer _musicPlayer;
        private ISimpleAudioPlayer _soundEffectPlayer;

        private double _musicVolume = 0.6;
        private double _soundVolume = 0.8;

        public static AudioManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new AudioManager();
                }
                return _instance;
            }
        }

        private AudioManager()
        {
            InitializeAudioPlayers();
        }

        private void InitializeAudioPlayers()
        {
            try
            {
                // Initialize music player
                _musicPlayer = CrossSimpleAudioPlayer.CreateSimpleAudioPlayer();
                var musicStream = GetStreamFromFile("Sounds/music game.mp3");
                if (musicStream != null)
                {
                    _musicPlayer.Load(musicStream);
                    _musicPlayer.Loop = true;
                    _musicPlayer.Volume = _musicVolume;
                    System.Diagnostics.Debug.WriteLine("AudioManager: Music loaded successfully");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("AudioManager: Failed to load music stream");
                }

                // Initialize sound effect player
                _soundEffectPlayer = CrossSimpleAudioPlayer.CreateSimpleAudioPlayer();
                var soundStream = GetStreamFromFile("Sounds/block remove sound.mp3");
                if (soundStream != null)
                {
                    _soundEffectPlayer.Load(soundStream);
                    _soundEffectPlayer.Volume = _soundVolume;
                    System.Diagnostics.Debug.WriteLine("AudioManager: Sound effects loaded successfully");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("AudioManager: Failed to load sound effects stream");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"AudioManager: Error initializing audio players: {ex.Message}");
            }
        }

        private Stream GetStreamFromFile(string filename)
        {
            try
            {
                var assembly = typeof(AudioManager).Assembly;
                var resourceName = "NimbleBlocks." + filename.Replace("/", ".");
                System.Diagnostics.Debug.WriteLine($"AudioManager: Looking for resource: {resourceName}");

                var stream = assembly.GetManifestResourceStream(resourceName);
                if (stream == null)
                {
                    var resourceNames = assembly.GetManifestResourceNames();
                    System.Diagnostics.Debug.WriteLine("AudioManager: Available resources:");
                    foreach (var name in resourceNames)
                    {
                        System.Diagnostics.Debug.WriteLine($"  - {name}");
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"AudioManager: Successfully loaded resource: {resourceName}");
                }

                return stream;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"AudioManager: Error loading resource {filename}: {ex.Message}");
                return null;
            }
        }

        #region Music Control

        public void PlayMusic()
        {
            try
            {
                if (_musicPlayer != null && _musicVolume > 0 && !_musicPlayer.IsPlaying)
                {
                    _musicPlayer.Play();
                    System.Diagnostics.Debug.WriteLine("AudioManager: Music started");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"AudioManager: Error playing music: {ex.Message}");
            }
        }

        public void StopMusic()
        {
            try
            {
                if (_musicPlayer != null && _musicPlayer.IsPlaying)
                {
                    _musicPlayer.Stop();
                    System.Diagnostics.Debug.WriteLine("AudioManager: Music stopped");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"AudioManager: Error stopping music: {ex.Message}");
            }
        }

        public void PauseMusic()
        {
            try
            {
                if (_musicPlayer != null && _musicPlayer.IsPlaying)
                {
                    _musicPlayer.Pause();
                    System.Diagnostics.Debug.WriteLine("AudioManager: Music paused");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"AudioManager: Error pausing music: {ex.Message}");
            }
        }

        public double MusicVolume
        {
            get => _musicVolume;
            set
            {
                _musicVolume = Math.Max(0, Math.Min(1, value));
                if (_musicPlayer != null)
                {
                    _musicPlayer.Volume = _musicVolume;

                    // Handle on/off based on volume
                    if (_musicVolume > 0 && !_musicPlayer.IsPlaying)
                    {
                        _musicPlayer.Play();
                    }
                    else if (_musicVolume == 0 && _musicPlayer.IsPlaying)
                    {
                        _musicPlayer.Stop();
                    }
                }
                System.Diagnostics.Debug.WriteLine($"AudioManager: Music volume set to {_musicVolume:F2}");
            }
        }

        public bool IsMusicPlaying => _musicPlayer?.IsPlaying ?? false;

        #endregion

        #region Sound Effects Control

        public void PlayBlockRemoveSound()
        {
            try
            {
                if (_soundEffectPlayer != null && _soundVolume > 0)
                {
                    if (_soundEffectPlayer.IsPlaying)
                    {
                        _soundEffectPlayer.Stop();
                    }
                    _soundEffectPlayer.Play();
                    System.Diagnostics.Debug.WriteLine("AudioManager: Block remove sound played");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"AudioManager: Error playing block remove sound: {ex.Message}");
            }
        }

        public void PlaySoundEffect()
        {
            // Generic sound effect for UI interactions
            PlayBlockRemoveSound();
        }

        public double SoundVolume
        {
            get => _soundVolume;
            set
            {
                _soundVolume = Math.Max(0, Math.Min(1, value));
                if (_soundEffectPlayer != null)
                {
                    _soundEffectPlayer.Volume = _soundVolume;
                }
                System.Diagnostics.Debug.WriteLine($"AudioManager: Sound volume set to {_soundVolume:F2}");
            }
        }

        #endregion

        #region Cleanup

        public void Dispose()
        {
            try
            {
                if (_musicPlayer != null)
                {
                    _musicPlayer.Stop();
                    _musicPlayer.Dispose();
                    _musicPlayer = null;
                }

                if (_soundEffectPlayer != null)
                {
                    _soundEffectPlayer.Stop();
                    _soundEffectPlayer.Dispose();
                    _soundEffectPlayer = null;
                }
                System.Diagnostics.Debug.WriteLine("AudioManager: Disposed successfully");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"AudioManager: Error disposing: {ex.Message}");
            }
        }

        #endregion
    }
}
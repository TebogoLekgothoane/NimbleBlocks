using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using Plugin.SimpleAudioPlayer;
using NimbleBlocks.Helpers;
namespace NimbleBlocks
{
	public partial class MainPage : ContentPage
	{
		public MainPage()
		{
			InitializeComponent();
			UpdateScoresLabel();
			StartAnimations();
			SetupSliders();

			// Start music using AudioManager
			AudioManager.Instance.PlayMusic();
		}

		private async void StartAnimations()
		{
			// Title animation - pulse effect
			await TitleLabel.ScaleTo(1.1, 1000, Easing.SinInOut);
			await TitleLabel.ScaleTo(1.0, 1000, Easing.SinInOut);


			// Continuous pulse animation
			Device.StartTimer(TimeSpan.FromSeconds(2), () =>
			{
				Device.BeginInvokeOnMainThread(async () =>
				{
					await TitleLabel.ScaleTo(1.05, 500, Easing.SinInOut);
					await TitleLabel.ScaleTo(1.0, 500, Easing.SinInOut);
				});
				return true; // Keep repeating
			});
		}

		private void AnimateParticle(Label particle, int delay, int duration)
		{
			Device.StartTimer(TimeSpan.FromMilliseconds(delay), () =>
			{
				Device.BeginInvokeOnMainThread(async () =>
				{
					// Optimized floating and fading animation with cancellation token
					var cancellationToken = new CancellationTokenSource();

					try
					{
						while (!cancellationToken.Token.IsCancellationRequested)
						{
							await particle.FadeTo(0.3, 1000, Easing.SinInOut);
							if (cancellationToken.Token.IsCancellationRequested) break;

							await particle.FadeTo(1.0, 1000, Easing.SinInOut);
							if (cancellationToken.Token.IsCancellationRequested) break;

							await particle.ScaleTo(1.3, 800, Easing.SinInOut);
							if (cancellationToken.Token.IsCancellationRequested) break;

							await particle.ScaleTo(1.0, 800, Easing.SinInOut);
							if (cancellationToken.Token.IsCancellationRequested) break;

							await Task.Delay(2000, cancellationToken.Token);
						}
					}
					catch (OperationCanceledException)
					{
						// Animation was cancelled, which is expected
					}
				});
				return false; // Don't repeat the timer
			});
		}

		private async void OnStartNewGame(object sender, EventArgs e)
		{
			// Play sound effect using AudioManager
			AudioManager.Instance.PlaySoundEffect();

			// Button press animation
			await StartButton.ScaleTo(0.95, 100, Easing.SinInOut);
			await StartButton.ScaleTo(1.0, 100, Easing.SinInOut);

			// Add a small delay for visual feedback
			await Task.Delay(150);
			await Navigation.PushAsync(new Level());
		}

		private async void OnResetScores(object sender, EventArgs e)
		{
			// Play sound effect using AudioManager
			AudioManager.Instance.PlaySoundEffect();

			// Button press animation
			await ResetButton.ScaleTo(0.95, 100, Easing.SinInOut);
			await ResetButton.ScaleTo(1.0, 100, Easing.SinInOut);

			// Score reset animation
			await ScoresLabel.FadeTo(0.3, 200);
			(Application.Current as App).PlayerWins = 0;
			(Application.Current as App).AppWins = 0;
			UpdateScoresLabel();
			await ScoresLabel.FadeTo(1.0, 200);
		}

		private void UpdateScoresLabel()
		{
			var app = Application.Current as App;
			ScoresLabel.Text = $"{app.PlayerWins}    VS    {app.AppWins}";
		}

		private void SetupSliders()
		{
			// Setup sound slider - get initial value from AudioManager
			SoundSlider.Value = AudioManager.Instance.SoundVolume;
			SoundSlider.ValueChanged += OnSoundSliderChanged;
			UpdateSoundLabel();

			// Setup music slider - get initial value from AudioManager
			MusicSlider.Value = AudioManager.Instance.MusicVolume;
			MusicSlider.ValueChanged += OnMusicSliderChanged;
			UpdateMusicLabel();
		}

		private void OnSoundSliderChanged(object sender, ValueChangedEventArgs e)
		{
			try
			{
				// Update AudioManager volume
				AudioManager.Instance.SoundVolume = e.NewValue;

				// Play preview sound if volume is not zero
				if (e.NewValue > 0)
				{
					AudioManager.Instance.PlaySoundEffect();
				}

				System.Diagnostics.Debug.WriteLine($"Sound volume changed to: {e.NewValue:F2}");
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"Error in sound slider change: {ex.Message}");
			}
			finally
			{
				UpdateSoundLabel();
			}
		}

		private void OnMusicSliderChanged(object sender, ValueChangedEventArgs e)
		{
			try
			{
				// Update AudioManager music volume (handles play/stop automatically)
				AudioManager.Instance.MusicVolume = e.NewValue;

				System.Diagnostics.Debug.WriteLine($"Music volume changed to: {e.NewValue:F2}");
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"Error in music slider change: {ex.Message}");
			}
			finally
			{
				UpdateMusicLabel();
			}
		}

		private void UpdateSoundLabel()
		{
			var percentage = (int)(SoundSlider.Value * 100);
			SoundValueLabel.Text = $"{percentage}%";
		}

		private void UpdateMusicLabel()
		{
			var percentage = (int)(MusicSlider.Value * 100);
			MusicValueLabel.Text = $"{percentage}%";
		}

		private async void OnSettingsMenuClicked(object sender, EventArgs e)
		{
			// Play sound effect using AudioManager
			AudioManager.Instance.PlaySoundEffect();

			// Show modal dialog with fade animation
			AudioSettingsModal.IsVisible = true;
			AudioSettingsModal.Opacity = 0.0;

			// Fade in animation
			await AudioSettingsModal.FadeTo(1.0, 300, Easing.CubicOut);
		}

		private async void OnCloseAudioSettingsClicked(object sender, EventArgs e)
		{
			// Play sound effect using AudioManager
			AudioManager.Instance.PlaySoundEffect();

			// Fade out animation
			await AudioSettingsModal.FadeTo(0.0, 300, Easing.CubicIn);

			AudioSettingsModal.IsVisible = false;
		}


		protected override async void OnAppearing()
		{
			base.OnAppearing();
			UpdateScoresLabel();

			// Fade in animation when page appears
			this.Opacity = 0;
			await this.FadeTo(1.0, 500, Easing.CubicInOut);

			// Ensure music is playing (AudioManager handles this globally)
			AudioManager.Instance.PlayMusic();
		}

		protected override void OnDisappearing()
		{
			base.OnDisappearing();
			// Don't dispose audio players - they're managed globally by AudioManager
		}
	}
}
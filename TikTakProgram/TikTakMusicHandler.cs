using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using NAudio.Wave;

namespace TikTakProgram;

public static class TikTakMusicHandler
{
    private static IWavePlayer? waveOut;
    private static WaveStream? reader;
    private static bool isMusicOn= true;
    private static bool _shouldLoopGameMusic = false;
    private static IWavePlayer? _gameMusicPlayer;
    private static WaveStream? _gameMusicStream;

    

    public static bool IsMusicOn => isMusicOn;

    public static void StartMainLoop()
    {
        if (!isMusicOn) return;

        Stop();
        Assembly assembly = Assembly.GetExecutingAssembly();
        string resourceName = "TikTakProgram.Musics.MainMusic.wav";

        using Stream? stream = assembly.GetManifestResourceStream(resourceName);
        if (stream == null)
        {
            Console.WriteLine("Cannot load music resource.");
            return;
        }

        reader = new WaveFileReader(stream);
        waveOut = new WaveOutEvent();
        waveOut.Volume = 0.7f;
        waveOut.Init(reader);
        waveOut.Play();
    }

    public static void Stop()
    {
        waveOut?.Stop();
        reader?.Dispose();
        waveOut?.Dispose();
        waveOut = null;
        reader = null;
    }

    public static void SwitchMusicState()
    {
        isMusicOn = !isMusicOn;
        Console.WriteLine(isMusicOn ? "Music ON" : "Music OFF");

        if (isMusicOn)
            StartMainLoop();
        else if (_shouldLoopGameMusic)
        {
            GameProcessSoundAsync().ConfigureAwait(false);
        }
        else
            Stop();
    }

    public static async Task PlayWinSoundAsync()
    {
        if (!isMusicOn) return;
        await PlayOneShotAsync("TikTakProgram.Musics.WinSound.wav");
    }

    public static async Task PlayLoseSoundAsync()
    {
        if (!isMusicOn) return;
        await PlayOneShotAsync("TikTakProgram.Musics.LoseSound.wav");
    }

    public static async Task PlayTieSoundAsync()
    {
        if (!isMusicOn) return;
        await PlayOneShotAsync("TikTakProgram.Musics.TieSound.wav");
    }

    public static async Task GameProcessSoundAsync()
    {
        if (!IsMusicOn) return;

        StopGameMusic();

        try
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            using Stream? stream = assembly.GetManifestResourceStream("TikTakProgram.Musics.GameProcessSound.wav");
            if (stream == null) return;

            _shouldLoopGameMusic = true;

            using WaveFileReader waveReader = new WaveFileReader(stream);
            _gameMusicStream = new LoopStream(waveReader); 
            _gameMusicPlayer = new WaveOutEvent();
            _gameMusicPlayer.Init(_gameMusicStream);
            _gameMusicPlayer.Play();

            await Task.CompletedTask;
        }
        catch
        {
            StopGameMusic();
        }
    }


    public static void StopGameMusic()
    {
        _gameMusicPlayer?.Stop();
        _gameMusicPlayer?.Dispose();
        _gameMusicStream?.Dispose();
        _gameMusicPlayer = null;
        _gameMusicStream = null;
        _shouldLoopGameMusic = false;
    }


    private static async Task PlayOneShotAsync(string resourceName)
    {
        await Task.Run(() =>
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            using Stream? stream = assembly.GetManifestResourceStream(resourceName);
            if (stream == null) return;

            using WaveFileReader reader = new WaveFileReader(stream);
            using WaveOutEvent waveOut = new WaveOutEvent();
            waveOut.Init(reader);
            waveOut.Play();

            while (waveOut.PlaybackState == PlaybackState.Playing)
            {
                Thread.Sleep(50);
            }
        });
    }
}
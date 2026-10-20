using UnityEngine;

public static class StorySfx
{
    static AudioSource source;
    static AudioClip ding;
    static AudioClip pop;
    static AudioClip tap;
    static AudioClip rustle;
    static System.Random noise;

    public static void PlayDing()
    {
        Play(ref ding, BuildDing, 0.45f);
    }

    public static void PlayPop()
    {
        Play(ref pop, BuildPop, 0.5f);
    }

    public static void PlayTap()
    {
        Play(ref tap, BuildTap, 0.75f);
    }

    public static void PlayRustle()
    {
        Play(ref rustle, BuildRustle, 0.5f);
    }

    static void Play(ref AudioClip slot, System.Func<AudioClip> make, float volume)
    {
        Ensure();
        if (slot == null)
            slot = make();
        if (source != null && slot != null)
            source.PlayOneShot(slot, volume);
    }

    static void Ensure()
    {
        if (source != null)
            return;
        var go = new GameObject("StorySfx");
        Object.DontDestroyOnLoad(go);
        source = go.AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.spatialBlend = 0f;
    }

    static AudioClip MakeClip(string name, float duration, System.Func<float, float> sample)
    {
        const int rate = 44100;
        int count = Mathf.Max(1, Mathf.RoundToInt(rate * duration));
        var clip = AudioClip.Create(name, count, 1, rate, false);
        var data = new float[count];
        for (int i = 0; i < count; i++)
            data[i] = Mathf.Clamp(sample(i / (float)rate), -1f, 1f);
        clip.SetData(data, 0);
        return clip;
    }

    // A soft wooden "ding": two close harmonics with a slow, rounded decay.
    static AudioClip BuildDing()
    {
        return MakeClip("ding", 0.6f, t =>
        {
            float attack = Mathf.Clamp01(t / 0.02f);
            float decay = Mathf.Exp(-t * 6f);
            float body = Mathf.Sin(2f * Mathf.PI * 587f * t)
                + 0.4f * Mathf.Sin(2f * Mathf.PI * 880f * t)
                + 0.15f * Mathf.Sin(2f * Mathf.PI * 1174f * t);
            return body * attack * decay * 0.3f;
        });
    }

    // A gentle "du" pop instead of a sharp electronic blip.
    static AudioClip BuildPop()
    {
        return MakeClip("pop", 0.2f, t =>
        {
            float attack = Mathf.Clamp01(t / 0.008f);
            float decay = Mathf.Exp(-t * 20f);
            float body = Mathf.Sin(2f * Mathf.PI * 330f * t)
                + 0.35f * Mathf.Sin(2f * Mathf.PI * 165f * t);
            return body * attack * decay * 0.34f;
        });
    }

    // The soft "du" played whenever a UI button is pressed.
    static AudioClip BuildTap()
    {
        return MakeClip("tap", 0.22f, t =>
        {
            float attack = Mathf.Clamp01(t / 0.006f);
            float decay = Mathf.Exp(-t * 17f);
            float body = Mathf.Sin(2f * Mathf.PI * 520f * t)
                + 0.3f * Mathf.Sin(2f * Mathf.PI * 260f * t);
            return body * attack * decay * 0.34f;
        });
    }

    // A soft leafy "shh" for a hiding spot that holds nothing.
    static AudioClip BuildRustle()
    {
        if (noise == null)
            noise = new System.Random(20260929);
        return MakeClip("rustle", 0.42f, t =>
        {
            float decay = Mathf.Exp(-t * 9f) * Mathf.Clamp01(t / 0.01f);
            float hiss = (float)(noise.NextDouble() * 2.0 - 1.0);
            float sway = 0.6f + 0.4f * Mathf.Sin(2f * Mathf.PI * 7f * t);
            return hiss * decay * sway * 0.22f;
        });
    }
}

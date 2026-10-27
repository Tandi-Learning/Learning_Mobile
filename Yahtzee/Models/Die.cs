using CommunityToolkit.Mvvm.ComponentModel;

namespace Yahtzee.Models;

/// <summary>
/// One of the five dice on the game board. Presentation-only: <see cref="Value"/> is the face the
/// player sees, <see cref="IsHeld"/> is whether the next roll leaves it untouched.
/// </summary>
/// <remarks>
/// <see cref="Value"/> 0 means "not rolled yet", which is how the board opens before the first roll;
/// it is the only value that is not a face. Scoring must not treat it as a 1.
/// </remarks>
public partial class Die : ObservableObject
{
    public const int FaceCount = 6;

    /// <summary>First Unicode die-face code point (U+2680, ⚀).</summary>
    private const int FirstFaceCodePoint = 0x2680;

    /// <summary>U+2610 ☐ BALLOT BOX: an unrolled die, monochrome and the same size as the faces.</summary>
    private const int EmptyFaceCodePoint = 0x2610;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Glyph))]
    private int _value = 0;

    [ObservableProperty]
    private bool _isHeld;

    /// <summary>Die face for the current value: ⚀ ⚁ ⚂ ⚃ ⚄ ⚅ — or ☐ while the die is unrolled.</summary>
    public string Glyph => Value == 0
        ? char.ConvertFromUtf32(EmptyFaceCodePoint)
        : char.ConvertFromUtf32(FirstFaceCodePoint + Math.Clamp(Value, 1, FaceCount) - 1);
}

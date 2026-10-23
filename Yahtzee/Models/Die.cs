using CommunityToolkit.Mvvm.ComponentModel;

namespace Yahtzee.Models;

/// <summary>
/// One of the five dice on the game board. Presentation-only: <see cref="Value"/> is the face the
/// player sees, <see cref="IsHeld"/> is whether the next roll leaves it untouched.
/// </summary>
public partial class Die : ObservableObject
{
    public const int FaceCount = 6;

    /// <summary>First Unicode die-face code point (U+2680, ⚀).</summary>
    private const int FirstFaceCodePoint = 0x2680;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Glyph))]
    private int _value = 1;

    [ObservableProperty]
    private bool _isHeld;

    /// <summary>Die face for the current value: ⚀ ⚁ ⚂ ⚃ ⚄ ⚅.</summary>
    public string Glyph => char.ConvertFromUtf32(FirstFaceCodePoint + Math.Clamp(Value, 1, FaceCount) - 1);
}

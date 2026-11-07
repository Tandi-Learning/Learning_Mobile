using Yahtzee.Models;
using Yahtzee.PageModels;

namespace Yahtzee.Pages;

public partial class GamePage : ContentPage
{
    public GamePage(GamePageModel model)
    {
        InitializeComponent();
        BindingContext = model;

        // Page and page model are both transient and built per navigation, so this subscription lives
        // exactly as long as the model it hangs off.
        model.Rolled += OnRolled;
    }

    /// <summary>
    /// A roll only re-faces the dice that are not held, so only those move: a held die that jumped would
    /// read as having been rolled as well.
    /// </summary>
    private void OnRolled(object? sender, EventArgs e)
    {
        foreach (IView child in DiceRow.Children)
        {
            if (child is not Border die || die.BindingContext is Die { IsHeld: true })
            {
                continue;
            }

            _ = BounceAsync(die);
        }
    }

    /// <summary>
    /// A dip and recovery, short enough to read as a roll rather than as a layout hiccup. Scale is animated
    /// through, not set, so a die that was mid-bounce cannot be left small.
    /// </summary>
    private static async Task BounceAsync(Border die)
    {
        die.CancelAnimations();
        await die.ScaleToAsync(0.82, 70, Easing.CubicOut);
        await die.ScaleToAsync(1, 110, Easing.CubicIn);
    }
}

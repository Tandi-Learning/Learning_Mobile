using Yahtzee.Scoring;

namespace Yahtzee.Tests;

/// <summary>
/// Every category, both bonuses and the joker rules. The expected values come from the standard Yahtzee
/// scorecard, not from what the code happens to do — if a test and the code disagree, read PLAN.md's
/// Decisions block before assuming the test is wrong.
/// </summary>
public class ScoringRulesTests
{
    [Theory]
    [InlineData(ScoringCategory.Ones, new[] { 1, 1, 1, 1, 1 }, 5)]
    [InlineData(ScoringCategory.Ones, new[] { 2, 3, 4, 5, 6 }, 0)]
    [InlineData(ScoringCategory.Twos, new[] { 2, 2, 5, 6, 1 }, 4)]
    [InlineData(ScoringCategory.Twos, new[] { 1, 3, 4, 5, 6 }, 0)]
    [InlineData(ScoringCategory.Threes, new[] { 3, 3, 3, 1, 2 }, 9)]
    [InlineData(ScoringCategory.Fours, new[] { 4, 4, 4, 4, 1 }, 16)]
    [InlineData(ScoringCategory.Fives, new[] { 5, 5, 1, 2, 3 }, 10)]
    [InlineData(ScoringCategory.Sixes, new[] { 6, 6, 6, 6, 6 }, 30)]
    [InlineData(ScoringCategory.Sixes, new[] { 1, 2, 3, 4, 5 }, 0)]
    public void UpperCategoriesCountMatchingFaces(ScoringCategory category, int[] dice, int expected) =>
        Assert.Equal(expected, ScoringRules.Score(category, dice));

    [Theory]
    [InlineData(new[] { 2, 2, 2, 3, 4 }, 13)]
    [InlineData(new[] { 1, 1, 2, 3, 4 }, 0)]
    [InlineData(new[] { 3, 3, 3, 3, 3 }, 15)]
    public void ThreeOfAKindScoresEveryDie(int[] dice, int expected) =>
        Assert.Equal(expected, ScoringRules.Score(ScoringCategory.ThreeOfAKind, dice));

    [Theory]
    [InlineData(new[] { 2, 2, 2, 2, 4 }, 12)]
    [InlineData(new[] { 2, 2, 2, 3, 4 }, 0)]
    [InlineData(new[] { 5, 5, 5, 5, 5 }, 25)]
    public void FourOfAKindScoresEveryDie(int[] dice, int expected) =>
        Assert.Equal(expected, ScoringRules.Score(ScoringCategory.FourOfAKind, dice));

    [Theory]
    [InlineData(new[] { 3, 3, 3, 2, 2 }, 25)]
    [InlineData(new[] { 4, 4, 4, 1, 1 }, 25)]
    [InlineData(new[] { 2, 2, 3, 3, 4 }, 0)]
    [InlineData(new[] { 2, 2, 2, 2, 2 }, 0)]
    public void FullHouseIsThreePlusTwo(int[] dice, int expected) =>
        Assert.Equal(expected, ScoringRules.Score(ScoringCategory.FullHouse, dice));

    [Theory]
    [InlineData(new[] { 1, 2, 3, 4, 6 }, 30)]
    [InlineData(new[] { 2, 3, 4, 5, 5 }, 30)]
    [InlineData(new[] { 3, 4, 5, 6, 6 }, 30)]
    [InlineData(new[] { 1, 2, 3, 5, 6 }, 0)]
    [InlineData(new[] { 1, 1, 2, 2, 3 }, 0)]
    public void SmallStraightIsAnyFourInARow(int[] dice, int expected) =>
        Assert.Equal(expected, ScoringRules.Score(ScoringCategory.SmallStraight, dice));

    [Theory]
    [InlineData(new[] { 1, 2, 3, 4, 5 }, 40)]
    [InlineData(new[] { 2, 3, 4, 5, 6 }, 40)]
    [InlineData(new[] { 1, 2, 3, 4, 4 }, 0)]
    [InlineData(new[] { 1, 2, 3, 4, 6 }, 0)]
    public void LargeStraightIsFiveInARow(int[] dice, int expected) =>
        Assert.Equal(expected, ScoringRules.Score(ScoringCategory.LargeStraight, dice));

    [Theory]
    [InlineData(new[] { 5, 5, 5, 5, 5 }, 50)]
    [InlineData(new[] { 5, 5, 5, 5, 4 }, 0)]
    public void YahtzeeIsFiveOfAKind(int[] dice, int expected) =>
        Assert.Equal(expected, ScoringRules.Score(ScoringCategory.Yahtzee, dice));

    [Theory]
    [InlineData(new[] { 1, 1, 2, 5, 6 }, 15)]
    [InlineData(new[] { 6, 6, 6, 6, 6 }, 30)]
    [InlineData(new[] { 1, 2, 3, 4, 5 }, 15)]
    public void ChanceScoresEveryDie(int[] dice, int expected) =>
        Assert.Equal(expected, ScoringRules.Score(ScoringCategory.Chance, dice));

    [Theory]
    [InlineData(new[] { 1, 1, 1, 1, 1 }, true)]
    [InlineData(new[] { 6, 6, 6, 6, 6 }, true)]
    [InlineData(new[] { 6, 6, 6, 6, 5 }, false)]
    public void IsFiveOfAKindRecognisesOneFace(int[] dice, bool expected) =>
        Assert.Equal(expected, ScoringRules.IsFiveOfAKind(dice));

    [Theory]
    [InlineData(0, 0)]
    [InlineData(62, 0)]
    [InlineData(63, 35)]
    [InlineData(105, 35)]
    public void UpperBonusPays35At63Points(int upperPoints, int expected) =>
        Assert.Equal(expected, ScoringRules.UpperBonus(upperPoints));

    [Theory]
    [InlineData(false, 0)]
    [InlineData(true, 100)]
    public void YahtzeeBonusNeedsAnAlreadyFilledYahtzeeBox(bool holdsFifty, int expected) =>
        Assert.Equal(expected, ScoringRules.YahtzeeBonus(holdsFifty));

    // A joker is a five of a kind rolled after the Yahtzee box already holds 50. It rescues the three
    // categories that would otherwise score nothing.
    [Theory]
    [InlineData(ScoringCategory.FullHouse, new[] { 2, 2, 2, 2, 2 }, 25)]
    [InlineData(ScoringCategory.SmallStraight, new[] { 4, 4, 4, 4, 4 }, 30)]
    [InlineData(ScoringCategory.LargeStraight, new[] { 3, 3, 3, 3, 3 }, 40)]
    public void JokerPaysTheFixedValue(ScoringCategory category, int[] dice, int expected) =>
        Assert.Equal(expected, ScoringRules.Score(category, dice, yahtzeeBoxHoldsFifty: true));

    [Theory]
    [InlineData(ScoringCategory.FullHouse, new[] { 2, 2, 2, 2, 2 }, 0)]
    [InlineData(ScoringCategory.SmallStraight, new[] { 4, 4, 4, 4, 4 }, 0)]
    [InlineData(ScoringCategory.LargeStraight, new[] { 3, 3, 3, 3, 3 }, 0)]
    public void WithoutAFilledYahtzeeBoxFiveOfAKindScoresNothing(
        ScoringCategory category, int[] dice, int expected) =>
        Assert.Equal(expected, ScoringRules.Score(category, dice));

    // The joker also decides nothing for the upper categories: five 4s pay 20 in Fours either way, and 0
    // in any other upper box, matching the official rule.
    [Theory]
    [InlineData(ScoringCategory.Fours, 20)]
    [InlineData(ScoringCategory.Sixes, 0)]
    [InlineData(ScoringCategory.ThreeOfAKind, 20)]
    [InlineData(ScoringCategory.FourOfAKind, 20)]
    [InlineData(ScoringCategory.Chance, 20)]
    [InlineData(ScoringCategory.Yahtzee, 50)]
    public void JokerLeavesTheOtherCategoriesAlone(ScoringCategory category, int expected)
    {
        int[] dice = [4, 4, 4, 4, 4];

        Assert.Equal(expected, ScoringRules.Score(category, dice));
        Assert.Equal(expected, ScoringRules.Score(category, dice, yahtzeeBoxHoldsFifty: true));
    }

    // Value 0 is "not rolled yet" (see Die). Scoring it as a face would quietly turn an unrolled die into
    // a point, so the rules refuse a roll that is not five dice from 1-6.
    [Theory]
    [InlineData(new[] { 1, 2, 3, 4 })]
    [InlineData(new[] { 1, 2, 3, 4, 5, 6 })]
    public void RejectsTheWrongNumberOfDice(int[] dice) =>
        Assert.Throws<ArgumentException>(() => ScoringRules.Score(ScoringCategory.Chance, dice));

    [Theory]
    [InlineData(new[] { 0, 2, 3, 4, 5 })]
    [InlineData(new[] { 1, 2, 3, 4, 7 })]
    public void RejectsFacesOutsideOneToSix(int[] dice) =>
        Assert.Throws<ArgumentOutOfRangeException>(() => ScoringRules.Score(ScoringCategory.Chance, dice));
}

namespace Yahtzee.Scoring;

/// <summary>
/// The 13 categories a player can fill in, in scorecard order. The derived rows — upper bonus, the two
/// section totals and the Yahtzee bonus — are deliberately absent: nobody chooses them, they follow from
/// the scores.
/// </summary>
/// <remarks>
/// <see cref="Ones"/> through <see cref="Sixes"/> keep their face value, so <c>(int)category</c> is the
/// face they count.
/// </remarks>
public enum ScoringCategory
{
    Ones = 1,
    Twos,
    Threes,
    Fours,
    Fives,
    Sixes,
    ThreeOfAKind,
    FourOfAKind,
    FullHouse,
    SmallStraight,
    LargeStraight,
    Yahtzee,
    Chance,
}

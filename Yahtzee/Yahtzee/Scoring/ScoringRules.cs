namespace Yahtzee.Scoring;

/// <summary>
/// The scoring table: pure functions from five die values to points, with no MAUI or model types
/// (see PLAN.md, "Scoring is pure logic"). Whether a category may be taken is a turn rule and is decided
/// by the caller, not here — these methods answer only "what would this category pay?".
/// </summary>
public static class ScoringRules
{
    /// <summary>Dice in a roll. The scoring functions are defined for exactly this many.</summary>
    public const int DiceCount = 5;

    private const int FaceCount = 6;

    /// <summary>63 points in the six upper categories earns the bonus.</summary>
    public const int UpperBonusThreshold = 63;

    public const int UpperBonusPoints = 35;
    public const int FullHousePoints = 25;
    public const int SmallStraightPoints = 30;
    public const int LargeStraightPoints = 40;
    public const int YahtzeePoints = 50;

    /// <summary>Each further Yahtzee, once the Yahtzee box already holds 50.</summary>
    public const int YahtzeeBonusPoints = 100;

    /// <summary>
    /// Points for <paramref name="category"/> on <paramref name="dice"/>.
    /// </summary>
    /// <param name="yahtzeeBoxHoldsFifty">
    /// True when the Yahtzee row is already filled with 50. A five of a kind is then a joker: Full House
    /// and both straights pay their fixed value instead of the 0 they would otherwise score. The rows the
    /// joker changes nothing for (the upper categories, three/four of a kind, Chance) are not special-cased,
    /// because five of a kind already scores there normally.
    /// </param>
    /// <exception cref="ArgumentException">Not exactly five dice.</exception>
    /// <exception cref="ArgumentOutOfRangeException">A die is outside 1–6; 0 means "not rolled".</exception>
    public static int Score(ScoringCategory category, IReadOnlyList<int> dice, bool yahtzeeBoxHoldsFifty = false)
    {
        int[] counts = CountFaces(dice);
        int total = dice.Sum();
        bool fiveOfAKind = IsFiveOfAKind(counts);
        bool joker = yahtzeeBoxHoldsFifty && fiveOfAKind;

        return category switch
        {
            ScoringCategory.Ones => FaceScore(face: 1, counts),
            ScoringCategory.Twos => FaceScore(face: 2, counts),
            ScoringCategory.Threes => FaceScore(face: 3, counts),
            ScoringCategory.Fours => FaceScore(face: 4, counts),
            ScoringCategory.Fives => FaceScore(face: 5, counts),
            ScoringCategory.Sixes => FaceScore(face: 6, counts),

            // A five of a kind is also a three and a four of a kind, so it scores the full sum here.
            ScoringCategory.ThreeOfAKind => HasKind(3, counts) ? total : 0,
            ScoringCategory.FourOfAKind => HasKind(4, counts) ? total : 0,

            ScoringCategory.FullHouse => HasFullHouse(counts) || joker ? FullHousePoints : 0,
            ScoringCategory.SmallStraight => HasSmallStraight(counts) || joker ? SmallStraightPoints : 0,
            ScoringCategory.LargeStraight => HasLargeStraight(counts) || joker ? LargeStraightPoints : 0,

            ScoringCategory.Yahtzee => fiveOfAKind ? YahtzeePoints : 0,
            ScoringCategory.Chance => total,

            _ => throw new ArgumentOutOfRangeException(nameof(category), category, "Not a scored category."),
        };
    }

    /// <summary>Five of the same face.</summary>
    public static bool IsFiveOfAKind(IReadOnlyList<int> dice) => IsFiveOfAKind(CountFaces(dice));

    /// <summary>35 points once the six upper categories total 63 or more, otherwise nothing.</summary>
    public static int UpperBonus(int upperSectionPoints) =>
        upperSectionPoints >= UpperBonusThreshold ? UpperBonusPoints : 0;

    /// <summary>
    /// 100 points for a five of a kind rolled while the Yahtzee box already held 50. The Yahtzee that first
    /// filled that box earns nothing, which is what the false case means.
    /// </summary>
    public static int YahtzeeBonus(bool yahtzeeBoxHoldsFifty) =>
        yahtzeeBoxHoldsFifty ? YahtzeeBonusPoints : 0;

    /// <summary>How many dice show each face; index 0 is unused.</summary>
    private static int[] CountFaces(IReadOnlyList<int> dice)
    {
        ArgumentNullException.ThrowIfNull(dice);

        if (dice.Count != DiceCount)
        {
            throw new ArgumentException($"A roll is exactly {DiceCount} dice; got {dice.Count}.", nameof(dice));
        }

        int[] counts = new int[FaceCount + 1];
        foreach (int value in dice)
        {
            if (value is < 1 or > FaceCount)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(dice), value, "A die is 1-6. 0 means the die has not been rolled.");
            }

            counts[value]++;
        }

        return counts;
    }

    private static bool IsFiveOfAKind(int[] counts) => counts.Max() == DiceCount;

    private static int FaceScore(int face, int[] counts) => counts[face] * face;

    private static bool HasKind(int kind, int[] counts) => counts.Any(count => count >= kind);

    private static bool HasFullHouse(int[] counts) => counts.Contains(3) && counts.Contains(2);

    private static bool HasSmallStraight(int[] counts) =>
        HasRun(counts, first: 1, length: 4)
        || HasRun(counts, first: 2, length: 4)
        || HasRun(counts, first: 3, length: 4);

    /// <summary>
    /// All five faces in a row. Four in a row is not enough here: the fifth die can be a duplicate rather
    /// than the end of the straight ([1,2,3,4,4] is a small straight, not a large one).
    /// </summary>
    private static bool HasLargeStraight(int[] counts) =>
        HasRun(counts, first: 1, length: 5) || HasRun(counts, first: 2, length: 5);

    private static bool HasRun(int[] counts, int first, int length)
    {
        for (int face = first; face < first + length; face++)
        {
            if (counts[face] == 0)
            {
                return false;
            }
        }

        return true;
    }
}

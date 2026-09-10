using System.Collections.ObjectModel;

namespace Yahtzee.Models;


public static class DiceRollPayloadExtension
{
    extension(ObservableCollection<Dice> dices)
    {
        public Dictionary<CATEGORIES, int> ProcessDiceValues()
        {
            var scores = new Dictionary<CATEGORIES, int>();

            scores[CATEGORIES.ACES] = dices.SumValue(CATEGORIES.ACES);
            scores[CATEGORIES.TWOS] = dices.SumValue(CATEGORIES.TWOS);
            scores[CATEGORIES.THREES] = dices.SumValue(CATEGORIES.THREES);
            scores[CATEGORIES.FOURS] = dices.SumValue(CATEGORIES.FOURS);
            scores[CATEGORIES.FIVES] = dices.SumValue(CATEGORIES.FIVES);
            scores[CATEGORIES.SIXES] = dices.SumValue(CATEGORIES.SIXES);
            scores[CATEGORIES.THREE_OF_A_KIND] = dices.SumOfAKind(CATEGORIES.THREE_OF_A_KIND);
            scores[CATEGORIES.FOUR_OF_A_KIND] = dices.SumOfAKind(CATEGORIES.FOUR_OF_A_KIND);
            scores[CATEGORIES.FULLHOUSE] = dices.SumOfFullhouse();
            scores[CATEGORIES.SMALL_STRAIGHT] = dices.SumOfStraight(CATEGORIES.SMALL_STRAIGHT);
            scores[CATEGORIES.LARGE_STRAIGHT] = dices.SumOfStraight(CATEGORIES.LARGE_STRAIGHT);
            scores[CATEGORIES.CHANCE] = dices.SumOfChance();
            scores[CATEGORIES.YAHTZEE] = dices.SumOfYahtzee();

            return scores;
        }

        private int SumValue(CATEGORIES category)
        {
            var value = category switch
            {
                CATEGORIES.ACES => 1,
                CATEGORIES.TWOS => 2,
                CATEGORIES.THREES => 3,
                CATEGORIES.FOURS => 4,
                CATEGORIES.FIVES => 5,
                CATEGORIES.SIXES => 6
            };
            return dices.Where(d => d.Value == value).Sum(d => d.Value);
        }

        private int SumOfAKind(CATEGORIES category)
        {
            var grouped = dices.GroupBy(
                            d => d.Value,
                            (key, dice) => new
                            {
                                Key = key,
                                GroupCount = dice.Count()
                            });

            var count = category == CATEGORIES.FOUR_OF_A_KIND ? 4 : 3;
            if (grouped.Max(g => g.GroupCount) >= count)
                return dices.Sum(d => d.Value);
            else
                return 0;
        }

        private int SumOfFullhouse()
        {
            var grouped = dices.GroupDiceByScore();

            if (grouped.Count() == 2 &&
                grouped.Any(g => g.GroupCount == 2) &&
                grouped.Any(g => g.GroupCount == 3))
            {
                return 25;
            }
            else
            {
                return 0;
            }
        }

        private int SumOfStraight(CATEGORIES category)
        {
            int compareSeq = category == CATEGORIES.SMALL_STRAIGHT ? 4 : 5;
            int score = category == CATEGORIES.SMALL_STRAIGHT ? 30 : 40;
            int seq = 0;
            int currentValue = 0;
            List<int> values = dices.Select(d => d.Value).Order().Distinct().ToList();

            bool isStraight = false;
            foreach (var value in values)
            {
                seq = (currentValue == 0 || currentValue == value - 1) ? seq + 1 : 0;
                isStraight = (category == CATEGORIES.SMALL_STRAIGHT && seq == 4) ||
                    (category == CATEGORIES.LARGE_STRAIGHT && seq == 5);
                if (isStraight) break;
                currentValue = value;
            }

            return isStraight ? score : 0;
        }

        private int SumOfChance()
        {
            return dices.Sum(d => d.Value);
        }

        private int SumOfYahtzee()
        {
            var grouped = dices.GroupDiceByScore();
            return (grouped.Count()  == 1 && grouped.Any(g => g.GroupCount == 5)) ? 50 : 0;
        }

        private IEnumerable<(int Key, int GroupCount)> GroupDiceByScore()
        {
            var grouped = dices.GroupBy(
                d => d.Value,
                (key, dice) => (key, dice.Count()));

            return grouped;
        }
        //private IEnumerable<ScoreGroup> GroupDiceByScore()
        //{
        //    var grouped = dices.GroupBy(
        //        d => d.Value,
        //        (key, dice) => new ScoreGroup
        //        {
        //            Key = key,
        //            GroupCount = dice.Count()
        //        });

        //    return grouped;
        //}
    }
}

public record DiceRollPayload(ObservableCollection<Dice> dices)
{
    public Dictionary<CATEGORIES, int> DiceScores { get; set; } = dices.ProcessDiceValues();
}
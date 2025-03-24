using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahtzee.Models;

public enum DICE_POSITION { FIRST = 0, SECOND = 1, THIRD = 2, FOURTH = 3, FIFTH = 4 }
public enum ROLL_COUNT { FIRST = 0, SECOND = 1, THIRD = 2 }

public enum CATEGORIES
{
    ACES,
    TWOS,
    THREES,
    FOURS,
    FIVES,
    SIXES,
    THREE_OF_A_KIND,
    FOUR_OF_A_KIND,
    FULLHOUSE,
    SMALL_STRAIGHT,
    LARGE_STRAIGHT,
    CHANCE,
    YAHTZEE,
    UPPER_SCORE,
    UPPER_BONUS,
    UPPER_TOTAL,
    LOWER_SCORE,
    YAHTZEE_BONUS,
    LOWER_TOTAL,
    GRAND_TOTAL,
    NONE
}

internal static class CONSTANTS
{
    public static string GREEN_LED = "green_led.png";
    public static string RED_LED = "red_led.png";

    public static string DICE_BLANK_IMAGE = "dice_blank.png";
    public static string DICE_1_IMAGE = "dice_1.png";
    public static string DICE_2_IMAGE = "dice_2.png";
    public static string DICE_3_IMAGE = "dice_3.png";
    public static string DICE_4_IMAGE = "dice_4.png";
    public static string DICE_5_IMAGE = "dice_5.png";
    public static string DICE_6_IMAGE = "dice_6.png";

    public static string HOLD_BGBOLOR = "Red";
    public static string NORMAL_BGCOLOR = "Transparent";
}

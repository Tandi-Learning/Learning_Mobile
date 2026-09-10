using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahtzee.Models;

public enum DICE_POSITION { 
    NONE,
    FIRST = 0, 
    SECOND = 1, 
    THIRD = 2, 
    FOURTH = 3, 
    FIFTH = 4 
}
public enum ROLL_COUNT { FIRST = 0, SECOND = 1, THIRD = 2 }
public enum DICE_STATE
{
    OPEN,
    HOLD
}

public enum CATEGORIES
{
    NONE = -1,
    ACES = 0,
    TWOS = 1,
    THREES = 2,
    FOURS = 3,
    FIVES = 4,
    SIXES = 5,
    THREE_OF_A_KIND = 6,
    FOUR_OF_A_KIND = 7,
    FULLHOUSE = 8,
    SMALL_STRAIGHT = 9,
    LARGE_STRAIGHT = 10,
    CHANCE = 11,
    YAHTZEE = 12,
    UPPER_SCORE = 13,
    UPPER_BONUS = 14,
    UPPER_TOTAL = 15,
    LOWER_SCORE = 16,
    YAHTZEE_BONUS = 17,
    LOWER_TOTAL = 18,
    GRAND_TOTAL = 19
}

internal static class CONSTANTS
{
    public static string ON_IMAGE = "on.png";
    public static string OFF_IMAGE = "off.png";

    public static string GREEN_LED_IMAGE = "green_led.png";
    public static string RED_LED_IMAGE = "red_led.png";

    public static string DICE_BLANK_IMAGE = "dice_blank.png";
    public static string DICE_1_IMAGE = "dice_1.png";
    public static string DICE_2_IMAGE = "dice_2.png";
    public static string DICE_3_IMAGE = "dice_3.png";
    public static string DICE_4_IMAGE = "dice_4.png";
    public static string DICE_5_IMAGE = "dice_5.png";
    public static string DICE_6_IMAGE = "dice_6.png";

    public static string RED_BGCOLOR = "Red";
    public static string TRANSPARENT_BGCOLOR = "Transparent";
}

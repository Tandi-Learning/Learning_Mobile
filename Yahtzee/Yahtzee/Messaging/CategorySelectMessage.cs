using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Text;
using Yahtzee.Models;

namespace Yahtzee.Messaging;

internal class CategorySelectMessage : ValueChangedMessage<CATEGORIES>
{
    public bool GameOver { get; set; }

    public CategorySelectMessage(CATEGORIES value, bool gameOver) : base(value)
    {
        GameOver = gameOver;
    }
}

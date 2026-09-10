using CommunityToolkit.Mvvm.Messaging.Messages;
using Yahtzee.Models;

namespace Yahtzee.Messaging;

public class DiceRollMessage : ValueChangedMessage<DiceRollPayload>
{
    public DiceRollMessage(DiceRollPayload value) : base(value) { }
}
using CommunityToolkit.Mvvm.Messaging;
using GloboTicket.Admin.Mobile.Messages;
using GloboTicket.Admin.Mobile.ViewModels;

namespace GloboTicket.Admin.Mobile.Tests.ViewModels;

public class EventListItemViewModelTests
{
    public static TheoryData<Guid, string?, string?, double, DateTime?, EventStatusEnum, List<string>?, CategoryViewModel?> Cases =
    new()
    {
            {
                Guid.NewGuid(), "imageUrl1", "Event1", 10, DateTime.UtcNow, EventStatusEnum.AlmostSoldOut, new List<string> { "Artists1" },
                new CategoryViewModel { Id = Guid.NewGuid(), Name = "Category1" }
            },
            {
                Guid.NewGuid(), "imageUrl2", "Event2", 20, DateTime.MaxValue, EventStatusEnum.Cancelled, new List<string> { "Artists2" },
                new CategoryViewModel { Id = Guid.NewGuid(), Name = "Category2" }
            },
            {
                Guid.Empty, null, null, 0, DateTime.MinValue, EventStatusEnum.SalesClosed, null, null
            }
    };

    [Theory, MemberData(nameof(Cases))]
    public void EventListItem_Initialized_PropertiesCorrectlySet(Guid id, string? imageUrl, string? name,
        double price, DateTime date, EventStatusEnum status, List<string>? artists, CategoryViewModel? category)
    {
        // Arrange

        // Act
        var sut = new EventListItemViewModel(id, name, price, date, artists, status, imageUrl, category);

        // Assert
        Assert.Equal(id, sut.Id);
        Assert.Equal(name, sut.Name);
        Assert.Equal(price, sut.Price);
        Assert.Equal(date, sut.Date);
        Assert.Equal(artists, sut.Artists);
        Assert.Equal(status, sut.EventStatus);
        Assert.Equal(imageUrl, sut.ImageUrl);
        Assert.Equal(category, sut.Category);
    }

    [Fact]
    public void EventListItem_Initialized_SubscribedToStatusChangedMessage()
    {
        // Arrange
        var id = Guid.NewGuid();
        var name = "Event1";
        var price = 1.0;
        var date = DateTime.UtcNow;
        var artists = new List<string>();
        var status = EventStatusEnum.AlmostSoldOut;

        // Act
        var sut = new EventListItemViewModel(id, name, price, date, artists, status);

        // Assert
        Assert.True(WeakReferenceMessenger.Default.IsRegistered<StatusChangedMessage>(sut));
    }

    [Theory]
    [InlineData(EventStatusEnum.AlmostSoldOut, EventStatusEnum.Cancelled)]
    [InlineData(EventStatusEnum.Cancelled, EventStatusEnum.AlmostSoldOut)]
    [InlineData(EventStatusEnum.AlmostSoldOut, EventStatusEnum.AlmostSoldOut)]
    public void StatusChangedMessageReceivedForId_StatusUpdated(EventStatusEnum originalValue, EventStatusEnum newValue)
    {
        // Arrange
        var id = Guid.NewGuid();
        var name = "Event1";
        var price = 1.0;
        var date = DateTime.UtcNow;
        var artists = new List<string>();

        var sut = new EventListItemViewModel(id, name, price, date, artists, originalValue);

        Assert.Equal(originalValue, sut.EventStatus);

        // Act
        WeakReferenceMessenger.Default.Send(new StatusChangedMessage(id, newValue));

        // Assert
        Assert.Equal(newValue, sut.EventStatus);
    }

    [Theory]
    [InlineData(EventStatusEnum.AlmostSoldOut, EventStatusEnum.Cancelled)]
    [InlineData(EventStatusEnum.Cancelled, EventStatusEnum.AlmostSoldOut)]
    [InlineData(EventStatusEnum.AlmostSoldOut, EventStatusEnum.AlmostSoldOut)]
    public void StatusChangedMessageReceivedForOtherId_StatusUpdated(EventStatusEnum originalValue, EventStatusEnum newValue)
    {
        // Arrange
        var id = Guid.NewGuid();
        var name = "Event1";
        var price = 1.0;
        var date = DateTime.UtcNow;
        var artists = new List<string>();

        var sut = new EventListItemViewModel(id, name, price, date, artists, originalValue);

        Assert.Equal(originalValue, sut.EventStatus);

        // Act
        WeakReferenceMessenger.Default.Send(new StatusChangedMessage(Guid.NewGuid(), newValue));

        // Assert
        Assert.Equal(originalValue, sut.EventStatus);
    }
}

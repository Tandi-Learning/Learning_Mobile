using CommunityToolkit.Mvvm.ComponentModel;
using GloboTicket.Admin.Mobile.ViewModels;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using static System.Net.WebRequestMethods;
//using CommunityToolkit.Maui.Core.Extensions;
//using CommunityToolkit.Mvvm.ComponentModel;
//using CommunityToolkit.Mvvm.Input;
//using CommunityToolkit.Mvvm.Messaging;
//using GloboTicket.Admin.Mobile.Messages;
//using GloboTicket.Admin.Mobile.Models;
//using GloboTicket.Admin.Mobile.Services;
//using GloboTicket.Admin.Mobile.ViewModels.Base;

namespace GloboTicket.Admin.Mobile.ViewModels;

//public partial class EventDetailViewModel : ViewModelBase, IQueryAttributable
//public class EventDetailViewModel : INotifyPropertyChanged
public class EventDetailViewModel : ObservableObject
{
    private Guid _id;
    private string _name = default!;
    private double _price;
    private string _imageUrl;
    private EventStatusEnum _eventStatus;
    private DateTime _date = DateTime.Now;
    private string _description;
    private List<string> _artists = new();
    private CategoryViewModel _category = new();

    public Guid Id
    {
        get => _id;
        set => SetProperty<Guid>(ref _id, value);
    }

    public string Name
    {
        get => _name;
        set => SetProperty<string>(ref _name, value);
    }

    public double Price
    {
        get => _price;
        set => SetProperty<double>(ref _price, value);
    }

    public string ImageUrl
    {
        get => _imageUrl;
        set => SetProperty<string>(ref _imageUrl, value);
    }

    public EventStatusEnum EventStatus
    {
        get => _eventStatus;
        set => SetProperty<EventStatusEnum>(ref _eventStatus, value);
    }

    public DateTime Date
    {
        get => _date;
        set => SetProperty<DateTime>(ref _date, value);
    }

    public string Description
    {
        get => _description;
        set => SetProperty<string>(ref _description, value);
    }

    public List<string> Artists
    {
        get => _artists;
        set => SetProperty<List<string>>(ref _artists, value);
    }

    public CategoryViewModel Category
    {
        get => _category;
        set => SetProperty<CategoryViewModel>(ref _category, value);
    }

    private bool _showLargerImage;

    public bool ShowLargerImage
    {
        get => _showLargerImage;
        set 
        {
            if (SetProperty<bool>(ref _showLargerImage, value))
                OnPropertyChanged(nameof(ShowThumbnailImage));
        }
    }

    public bool ShowThumbnailImage => !ShowLargerImage;

    public ICommand CancelEventCommand
    {
        get;
    }

    private void CancelEvent() => EventStatus = EventStatusEnum.Cancelled;

    private bool CanCancelEvent() => EventStatus != EventStatusEnum.Cancelled && Date.AddHours(-4) > DateTime.Now;

    public EventDetailViewModel()
    {
        CancelEventCommand = new Command(CancelEvent, CanCancelEvent);

        Id = Guid.Parse("EE272F8B-6096-4CB6-8625-BB4BB2D89E8B");
        Name = "John Egberts Live";
        Price = 65;
        ImageUrl = "https://lindseybroospluralsight.blob.core.windows.net/globoticket/images/banjo.jpg";
        EventStatus = EventStatusEnum.OnSale;
        Date = DateTime.Now.AddMonths(6);
        Description =
            "Join John for his farewell tour across 15 continents. John really needs no introduction since he has already mesmerized the world with his banjo.";
        Artists = new List<string> { "John Egbert", "Jane Egbert" };
        Category = new CategoryViewModel
        {
            Id = Guid.Parse("B0788D2F-8003-43C1-92A4-EDC76A7C5DDE"),
            Name = "Concert"
        };
    }

    //public event PropertyChangedEventHandler? PropertyChanged;

    //public void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    //    => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}

using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GloboTicket.Admin.Mobile.ViewModels;

public class CategoryViewModel : ObservableObject
{
    private Guid _id;
    private string _name = default!;

    public Guid Id { 
        get => _id; 
        set => SetProperty<Guid>(ref _id, value); 
    }

    public string Name {
        get => _name;
        set => SetProperty<string>(ref _name, value);
    }
}

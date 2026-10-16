using Yahtzee.Models;
using Yahtzee.PageModels;

namespace Yahtzee.Pages
{
    public partial class MainPage : ContentPage
    {
        public MainPage(MainPageModel model)
        {
            InitializeComponent();
            BindingContext = model;
        }
    }
}
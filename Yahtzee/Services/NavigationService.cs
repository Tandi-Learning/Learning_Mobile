using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahtzee.Services;

public class NavigationService : INavigationService
{
    public async Task GotoMainPage()
    {
        await Shell.Current.GoToAsync("//main-page");
    }

    public async Task GotoGamePage()
    {
        await Shell.Current.GoToAsync("//game-page");
    }

    public async Task GotoScoresPage()
    {
        await Shell.Current.GoToAsync("//scores-page");
    }
}

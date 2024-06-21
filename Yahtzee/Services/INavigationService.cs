using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahtzee.Services;

public interface INavigationService
{
    Task GotoMainPage();

    Task GotoGamePage();

    Task GotoScoresPage();    
}

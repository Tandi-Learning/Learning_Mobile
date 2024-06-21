using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahtzee.Models;

public enum SessionStatus 
{
    InProgress,
    Abandoned,
    Completed
};

class Session
{
    public int Id { get; set; }
    public SessionStatus Status { get; set; }
}

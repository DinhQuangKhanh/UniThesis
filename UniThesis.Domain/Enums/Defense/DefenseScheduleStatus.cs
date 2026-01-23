using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniThesis.Domain.Enums.Defense
{
    public enum DefenseScheduleStatus
    {
        Scheduled = 0,
        InProgress = 1,
        Completed = 2,
        Postponed = 3,
        Cancelled = 4
    }
}

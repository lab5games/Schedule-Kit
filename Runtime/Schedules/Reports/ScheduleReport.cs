
namespace Lab5Games.ScheduleKit
{
    public class ScheduleReport
    {
        public readonly Schedule.States state;

        public ScheduleReport(Schedule schedule)
        {
            state = schedule.state;
        }
    }
}

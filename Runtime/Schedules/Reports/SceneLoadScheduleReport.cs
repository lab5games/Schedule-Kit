
namespace Lab5Games.ScheduleKit
{
    public class SceneLoadScheduleReport : ScheduleReport
    {
        SceneLoadSchedule m_Schedule;

        public SceneLoadScheduleReport(SceneLoadSchedule schedule) : base(schedule)
        {
            m_Schedule = schedule;
        }

        public void Activate()
        {
            m_Schedule.Activeate();
        }
    }
}

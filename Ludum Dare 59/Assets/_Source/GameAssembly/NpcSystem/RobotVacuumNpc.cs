namespace NpcSystem
{
    public class RobotVacuumNpc : BaseNpc
    {
        protected override void Tick() => Patrol();
    }
}

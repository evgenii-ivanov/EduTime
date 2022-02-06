using CommandLine;

namespace EduTime.AdminCli.CliOptions
{
    public enum MigrationDirections
    {
        Up,
        Down
    }

    [Verb("migration", HelpText = "Database migration")]
    public class MigrationOptions
    {
        [Option('d', "direction", Required = true, HelpText = "Database migration direction")]
        public MigrationDirections Direction { get; set; }
    }
}

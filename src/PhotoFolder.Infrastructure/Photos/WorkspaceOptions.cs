namespace PhotoFolder.Infrastructure.Photos
{
    public class WorkspaceOptions
    {
        public string Path { get; set; } = "%appdata%\\PhotoFolder\\workspaces";
        public bool ApplyMigrations { get; set; } = true;
    }
}

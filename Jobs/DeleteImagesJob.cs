using Quartz;

namespace Photon.ImageDb.Jobs;

public class DeleteImagesJob : IJob
{
    public Task Execute(IJobExecutionContext context)
    {
        var images = Directory.GetFiles("Images", "*", SearchOption.AllDirectories)
            .Where(file => File.GetCreationTime(file) < DateTime.Now.AddHours(-5));
        foreach (var image in images)
        {
            File.Delete(image);
        }
        var folders = Directory.GetDirectories("Images", "*", SearchOption.AllDirectories);
        foreach (var folder in folders)
        {
            if (Directory.GetFiles(folder).Length == 0)
            {
                Directory.Delete(folder);
            }
        }
        return Task.CompletedTask;
    }
}
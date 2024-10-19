using Photon.ImageDb.Jobs;
using Photon.ImageDb.Middleware;
using Quartz;
using Quartz.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
Directory.CreateDirectory("Images");
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<AuthenticationMiddleware>();
builder.Services.AddQuartz(q =>
{
    var jobKey = new JobKey("DeleteImagesJob");
    q.AddJob<DeleteImagesJob>(opts => opts.WithIdentity(jobKey));
    q.AddTrigger(opts => opts
        .ForJob(jobKey)
        .WithIdentity("DeleteImagesJob-trigger")
        .WithSimpleSchedule(x => x.WithIntervalInHours(4).RepeatForever()));
});
builder.Services.AddQuartzServer(options =>
{
    options.WaitForJobsToComplete = true;
});
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<AuthenticationMiddleware>();
app.MapControllers();
app.Run();
using Consul;
using Microsoft.AspNetCore.Mvc; 

var builder = WebApplication.CreateBuilder(args);


builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5001); 
});




builder.Services.AddRazorPages()
    .AddRazorPagesOptions(options =>
    {
        options.Conventions.ConfigureFilter(new IgnoreAntiforgeryTokenAttribute());
    });

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}





app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapRazorPages(); 





var consulClient = new ConsulClient(config =>
{
    config.Address = new Uri("http://consul:8500"); 
});





var registration = new AgentServiceRegistration
{
    ID = "web_app", 
    Name = "web_app", 
    Address = "web_app", 
    Port = 5001 
};

await consulClient.Agent.ServiceRegister(registration);

app.Run();

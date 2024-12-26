using Consul;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5000); 
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}




app.UseHttpsRedirection();

app.UseAuthorization();
app.MapControllers();

var consulClient = new ConsulClient(config =>
{
    config.Address = new Uri("http://consul:8500");
});

var registration = new AgentServiceRegistration
{
    ID = "redactor_pdf",
    Name = "redactor_pdf",
    Address = "redactor_pdf",
    Port = 5000
};

await consulClient.Agent.ServiceRegister(registration);

app.Run();

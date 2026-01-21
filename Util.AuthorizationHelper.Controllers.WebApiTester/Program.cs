
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Util.AuthorizationHelper.Controllers.Controllers;
using Util.AuthorizationHelper.Controllers.WebApiTester.Controllers;

namespace Util.AuthorizationHelper.Controllers.WebApiTester;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();

        builder.AddControllerWithConvention<ControllerA, ControllerA.Convention, ControllerA.Convention.MyOptions>()
            .Configure(x => { });
        builder.AddControllerWithConvention<ControllerB, ControllerB.Convention, ControllerB.Convention.MyOptions>()
            .Configure(x => { }); ;
        builder.AddControllerWithConvention<ControllerC, ControllerC.Convention, ControllerC.Convention.MyOptions>()
            .Configure(x => { }); ;

        builder.Services.AddAuthentication();
        builder.Services.AddAuthorization(x =>
        {
            x.AddPolicy(Const.Polici, p => p.AddRequirements(new AssertionRequirement(c =>
            {
                Console.WriteLine($"We're checking the policy {Const.Polici}!!!");
                return true;
            })));

            x.AddPolicy(Const.Polici2, p => p.AddRequirements(new AssertionRequirement(c =>
            {
                Console.WriteLine($"We're checking the policy {Const.Polici2}!!!");
                return true;
            })));
        });

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();
        
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseSwagger();
        app.UseSwaggerUI();


        app.MapControllers();

        app.Run();
    }
}
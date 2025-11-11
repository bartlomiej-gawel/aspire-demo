var builder = DistributedApplication.CreateBuilder(args);

// builder.AddPostgres("demo-postgres")
//     .WithImage("postgres:latest")
//     .WithBindMount("../../.containers/postgres", "/var/lib/postgresql/18/docker")
//     .WithLifetime(ContainerLifetime.Persistent);

builder.AddViteApp("demo-app", "../../apps/demo-app")
    .WithEnvironment("BROWSER", "none")
    .WithExternalHttpEndpoints();

builder.AddViteApp("demo-backoffice-app", "../../apps/demo-backoffice-app")
    .WithEnvironment("BROWSER", "none")
    .WithExternalHttpEndpoints();

builder.Build().Run();
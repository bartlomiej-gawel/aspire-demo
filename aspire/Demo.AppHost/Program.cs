var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("demo-postgres")
    .WithImage("postgres:latest")
    .WithBindMount("../../.containers/postgres", "/var/lib/postgresql/18/docker")
    .WithLifetime(ContainerLifetime.Persistent)
    .AddDatabase("demo-db");

builder.AddNpmApp("demo-app", "../../apps/demo-app")
    .WithReference(weatherApi)
    .WithEnvironment("BROWSER", "none")
    .WithHttpEndpoint(env: "VITE_PORT")
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();

builder.AddNpmApp("demo-backoffice-app", "../../apps/demo-backoffice-app")
    .WithReference(weatherApi)
    .WithEnvironment("BROWSER", "none")
    .WithHttpEndpoint(env: "VITE_PORT")
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();

builder.Build().Run();
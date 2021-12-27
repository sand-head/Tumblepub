using Tumblepub.EventSourcing.Extensions;
using Tumblepub.GraphQL;

var builder = WebApplication.CreateBuilder(args);

// configure event sourcing
builder.AddEventSourcing(builder.Configuration.GetConnectionString("Database"));

// add GraphQL support using HotChocolate
builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

// maps "/graphql" to the GraphQL API endpoint
app.MapGraphQL();

app.Run();

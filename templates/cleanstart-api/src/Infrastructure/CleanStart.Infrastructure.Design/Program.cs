using CleanStart.Infrastructure.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<CleanStartDbContext>(x => x.UseNpgsql(builder.Configuration.GetConnectionString("CleanStart"),
                                                                    ops =>
                                                                    {
                                                                      ops.MigrationsAssembly("CleanStart.Infrastructure.Design")
                                                                        .MigrationsHistoryTable("EF_MIGRATIONS");
                                                                    }));

var app = builder.Build();
app.Run();
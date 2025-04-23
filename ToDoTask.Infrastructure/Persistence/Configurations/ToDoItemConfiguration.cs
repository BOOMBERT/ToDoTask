using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ToDoTask.Domain.Entities;

namespace ToDoTask.Infrastructure.Persistence.Configurations;

internal class ToDoItemConfiguration : IEntityTypeConfiguration<ToDoItem>
{
    public void Configure(EntityTypeBuilder<ToDoItem> builder)
    {
        builder.Property(toDoItem => toDoItem.Title)
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(toDoItem => toDoItem.Description)
            .IsRequired()
            .HasMaxLength(512);

        builder.Property(toDoItem => toDoItem.ExpiryDateTime)
            .IsRequired();

        builder.Property(toDoItem => toDoItem.CompletionPercentage)
            .IsRequired()
            .HasPrecision(5, 2)
            .HasDefaultValue(0m);
    }
}

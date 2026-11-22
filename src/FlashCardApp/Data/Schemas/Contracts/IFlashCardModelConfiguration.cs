using FlashCardApp.Models.Commons;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlashCardApp.Data.Configurations.Contracts;

public interface IFlashCardModelConfiguration
{
    void Apply(ModelBuilder builder);
}
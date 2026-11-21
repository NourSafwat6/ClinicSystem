using ClinicSystem.API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace ClinicSystem.API.Data.Configurations
{
    public class VisitTypeConfiguration : IEntityTypeConfiguration<VisitType>
    {

        public void Configure(EntityTypeBuilder<VisitType> builder)
        {
            builder.ToTable("visit_types", t =>
            {
                t.HasCheckConstraint(
                    "CK_VisitTypes_DurationMin",
                    "duration_min BETWEEN 5 AND 30");

                t.HasCheckConstraint(
                    "CK_VisitTypes_Price",
                    "price >= 0");
            });

            builder.HasKey(v => v.Code);

            builder.Property(v => v.Code)
                .HasColumnName("code")
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(v => v.NameAr)
                .HasColumnName("name_ar")
                .IsRequired();

            builder.Property(v => v.DurationMin)
                .HasColumnName("duration_min")
                .IsRequired();

            builder.Property(v => v.Price)
                .HasColumnName("price")
                .IsRequired();

            builder.Property(v => v.PatientCanBook)
                .HasColumnName("patient_can_book")
                .HasDefaultValue(true)
                .IsRequired();

            builder.Property(v => v.Sort)
                .HasColumnName("sort")
                .HasDefaultValue(0)
                .IsRequired();
            builder.HasData(
    new VisitType
    {
        Code = "kashf",
        NameAr = "كشف",
        DurationMin = 20,
        Price = 300,
        PatientCanBook = true,
        Sort = 1
    },
    new VisitType
    {
        Code = "reexam",
        NameAr = "إعادة كشف",
        DurationMin = 20,
        Price = 200,
        PatientCanBook = true,
        Sort = 2
    },
    new VisitType
    {
        Code = "followup",
        NameAr = "متابعة",
        DurationMin = 30,
        Price = 75,
        PatientCanBook = true,
        Sort = 3
    }
);
        }
    }
}

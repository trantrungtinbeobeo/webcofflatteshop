using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace webcofflatteshop.Migrations
{
    public partial class AddProfileEmailVerification : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                IF COL_LENGTH('AspNetUsers', 'ProfileBackgroundImageUrl') IS NULL
                BEGIN
                    ALTER TABLE AspNetUsers ADD ProfileBackgroundImageUrl nvarchar(300) NULL
                END
                """);

            migrationBuilder.Sql("""
                IF COL_LENGTH('AspNetUsers', 'PendingEmail') IS NULL
                BEGIN
                    ALTER TABLE AspNetUsers ADD PendingEmail nvarchar(256) NULL
                END
                """);

            migrationBuilder.Sql("""
                IF COL_LENGTH('AspNetUsers', 'EmailVerificationCode') IS NULL
                BEGIN
                    ALTER TABLE AspNetUsers ADD EmailVerificationCode nvarchar(10) NULL
                END
                """);

            migrationBuilder.Sql("""
                IF COL_LENGTH('AspNetUsers', 'EmailVerificationCodeExpiresAt') IS NULL
                BEGIN
                    ALTER TABLE AspNetUsers ADD EmailVerificationCodeExpiresAt datetime2 NULL
                END
                """);

            migrationBuilder.Sql("""
                IF COL_LENGTH('AspNetUsers', 'EmailVerificationCodeSentAt') IS NULL
                BEGIN
                    ALTER TABLE AspNetUsers ADD EmailVerificationCodeSentAt datetime2 NULL
                END
                """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                IF COL_LENGTH('AspNetUsers', 'ProfileBackgroundImageUrl') IS NOT NULL
                BEGIN
                    ALTER TABLE AspNetUsers DROP COLUMN ProfileBackgroundImageUrl
                END
                """);

            migrationBuilder.Sql("""
                IF COL_LENGTH('AspNetUsers', 'PendingEmail') IS NOT NULL
                BEGIN
                    ALTER TABLE AspNetUsers DROP COLUMN PendingEmail
                END
                """);

            migrationBuilder.Sql("""
                IF COL_LENGTH('AspNetUsers', 'EmailVerificationCode') IS NOT NULL
                BEGIN
                    ALTER TABLE AspNetUsers DROP COLUMN EmailVerificationCode
                END
                """);

            migrationBuilder.Sql("""
                IF COL_LENGTH('AspNetUsers', 'EmailVerificationCodeExpiresAt') IS NOT NULL
                BEGIN
                    ALTER TABLE AspNetUsers DROP COLUMN EmailVerificationCodeExpiresAt
                END
                """);

            migrationBuilder.Sql("""
                IF COL_LENGTH('AspNetUsers', 'EmailVerificationCodeSentAt') IS NOT NULL
                BEGIN
                    ALTER TABLE AspNetUsers DROP COLUMN EmailVerificationCodeSentAt
                END
                """);
        }
    }
}

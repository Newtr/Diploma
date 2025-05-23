using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace EvacProject.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "academic_degrees",
                columns: table => new
                {
                    AcademicDegreeId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_academic_degrees", x => x.AcademicDegreeId);
                });

            migrationBuilder.CreateTable(
                name: "departments",
                columns: table => new
                {
                    DepartmentId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_departments", x => x.DepartmentId);
                });

            migrationBuilder.CreateTable(
                name: "faculties",
                columns: table => new
                {
                    FacultyId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_faculties", x => x.FacultyId);
                });

            migrationBuilder.CreateTable(
                name: "forms_of_study",
                columns: table => new
                {
                    FormOfStudyId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_forms_of_study", x => x.FormOfStudyId);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FirstName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Patronymic = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "admins",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    DepartmentId = table.Column<int>(type: "integer", nullable: false),
                    AcademicDegreeId = table.Column<int>(type: "integer", nullable: false),
                    PrivilegeGranted = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PrivilegeExpiry = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_admins", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_admins_academic_degrees_AcademicDegreeId",
                        column: x => x.AcademicDegreeId,
                        principalTable: "academic_degrees",
                        principalColumn: "AcademicDegreeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_admins_departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "departments",
                        principalColumn: "DepartmentId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_admins_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "students",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    StudentNumber = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    FacultyId = table.Column<int>(type: "integer", nullable: false),
                    FormOfStudyId = table.Column<int>(type: "integer", nullable: false),
                    AdmissionDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TicketIssueDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TicketExpiryDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TelegramChatId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_students", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_students_faculties_FacultyId",
                        column: x => x.FacultyId,
                        principalTable: "faculties",
                        principalColumn: "FacultyId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_students_forms_of_study_FormOfStudyId",
                        column: x => x.FormOfStudyId,
                        principalTable: "forms_of_study",
                        principalColumn: "FormOfStudyId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_students_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_admins_AcademicDegreeId",
                table: "admins",
                column: "AcademicDegreeId");

            migrationBuilder.CreateIndex(
                name: "IX_admins_DepartmentId",
                table: "admins",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_students_FacultyId",
                table: "students",
                column: "FacultyId");

            migrationBuilder.CreateIndex(
                name: "IX_students_FormOfStudyId",
                table: "students",
                column: "FormOfStudyId");

            migrationBuilder.CreateIndex(
                name: "IX_students_StudentNumber",
                table: "students",
                column: "StudentNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_students_TelegramChatId",
                table: "students",
                column: "TelegramChatId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "admins");

            migrationBuilder.DropTable(
                name: "students");

            migrationBuilder.DropTable(
                name: "academic_degrees");

            migrationBuilder.DropTable(
                name: "departments");

            migrationBuilder.DropTable(
                name: "faculties");

            migrationBuilder.DropTable(
                name: "forms_of_study");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}

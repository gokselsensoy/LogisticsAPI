using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class replace_geography : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE OR REPLACE FUNCTION public.st_x(geography)
                 RETURNS double precision
                 LANGUAGE sql
                 IMMUTABLE STRICT
                AS $function$
                    SELECT ST_X($1::geometry);
                $function$;
            ");

            migrationBuilder.Sql(@"
                CREATE OR REPLACE FUNCTION public.st_y(geography)
                 RETURNS double precision
                 LANGUAGE sql
                 IMMUTABLE STRICT
                AS $function$
                    SELECT ST_Y($1::geometry);
                $function$;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP FUNCTION IF EXISTS public.st_x(geography);");
            migrationBuilder.Sql("DROP FUNCTION IF EXISTS public.st_y(geography);");
        }
    }
}

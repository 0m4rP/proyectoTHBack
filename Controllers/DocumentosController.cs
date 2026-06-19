using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using proyectSystemTh.Data;
using proyectSystemTh.DTOs;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace proyectSystemTh.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public DocumentosController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // ==========================================
        // 1. OBTENER PERIODOS DE NÓMINA DISPONIBLES
        // ==========================================
        [HttpGet("nominas-disponibles/{idEmpleado}")]
        public async Task<IActionResult> GetNominasDisponibles(int idEmpleado)
        {
            var query = "SELECT idNomina, periodoMes, periodoAnio FROM nomina WHERE idEmpleado = {0} ORDER BY periodoAnio DESC, periodoMes DESC";

            var nominas = await _context.Database
                .SqlQueryRaw<NominaPeriodoDto>(query, idEmpleado)
                .ToListAsync();

            return Ok(nominas);
        }

        // ==========================================
        // 2. DESCARGAR CERTIFICACIÓN LABORAL (PDF)
        // ==========================================
        [HttpGet("certificacion/{idEmpleado}")]
        public async Task<IActionResult> DescargarCertificacion(int idEmpleado)
        {
            var empleado = await _context.Empleados.FirstOrDefaultAsync(e => e.IdEmpleado == idEmpleado);
            if (empleado == null) return NotFound("Empleado no encontrado");

            var salario = await _context.Contratos
                .Where(c => c.EmpleadoIdEmpleado == idEmpleado)
                .Select(c => c.SalarioContrato)
                .FirstOrDefaultAsync();

            var pdfStream = new MemoryStream();
            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(3, Unit.Centimetre);
                    page.Size(PageSizes.Letter);
                    page.DefaultTextStyle(x => x.FontFamily("Arial").FontSize(11).LineHeight(1.5f));

                    page.Header().Text("SISTEMA DE GESTIÓN HUMANA SGTGH")
                        .SemiBold().FontSize(10).FontColor(Colors.Grey.Medium).AlignCenter();

                    page.Content().PaddingTop(2, Unit.Centimetre).Column(col =>
                    {
                        col.Item().Text("CERTIFICACIÓN LABORAL").Bold().FontSize(16).AlignCenter();
                        col.Item().PaddingTop(2, Unit.Centimetre).Text("A QUIEN PUEDA INTERESAR:");

                        col.Item().PaddingTop(1, Unit.Centimetre).Text(
                            $"Que el(la) señor(a) {empleado.NombreEmpleado} {empleado.ApellidoEmpleado}, " +
                            $"identificado(a) con los registros internos del sistema, labora en nuestra compañía " +
                            $"desempeñando el cargo de {empleado.Cargo}, con una asignación salarial mensual de " +
                            $"${salario:N2} M/CTE.");

                        col.Item().PaddingTop(1, Unit.Centimetre).Text(
                            $"La presente certificación se expide a solicitud del interesado el día {DateTime.Today:dd 'de' MMMM 'de' yyyy}.");

                        col.Item().PaddingTop(3, Unit.Centimetre).Text("Cordialmente,");
                        col.Item().PaddingTop(1, Unit.Centimetre).Text("___________________________").Bold();
                        col.Item().Text("Área de Gestión Humana").Bold();
                    });
                });
            }).GeneratePdf(pdfStream);

            pdfStream.Position = 0;
            return File(pdfStream, "application/pdf", $"Certificacion_{empleado.NombreEmpleado}.pdf");
        }

        // ==========================================
        // 3. DESCARGAR COMPROBANTE DE NÓMINA (PDF)
        // ==========================================
        [HttpGet("nomina-pdf/{idNomina}")]
        public async Task<IActionResult> DescargarNominaPdf(int idNomina)
        {
            var datos = await _context.Nominas
                .Where(n => n.IdNomina == idNomina)
                .Select(n => new ComprobanteDetalleDto
                {
                    IdNomina = n.IdNomina,
                    PeriodoMes = n.PeriodoMes,
                    PeriodoAnio = n.PeriodoAnio,
                    SalarioBase = n.SalarioBase,
                    TotalDevengado = n.TotalDevengado,
                    TotalDeducciones = n.TotalDeducciones,
                    NetoPagar = n.NetoPagar,
                    FechaPago = n.FechaPago,
                    NombreCompleto = _context.Empleados
                        .Where(e => e.IdEmpleado == n.IdEmpleado)
                        .Select(e => e.NombreEmpleado + " " + e.ApellidoEmpleado)
                        .FirstOrDefault() ?? "Empleado Desconocido",
                    Cargo = _context.Empleados
                        .Where(e => e.IdEmpleado == n.IdEmpleado)
                        .Select(e => e.Cargo)
                        .FirstOrDefault() ?? "Sin Cargo"
                })
                .FirstOrDefaultAsync();

            if (datos == null)
                return NotFound("No se encontraron registros de pago para la nómina solicitada.");

            // Mapeo de meses numéricos a nombres para el reporte
            string mesNombre = datos.PeriodoMes switch
            {
                1 => "Enero",
                2 => "Febrero",
                3 => "Marzo",
                4 => "Abril",
                5 => "Mayo",
                6 => "Junio",
                7 => "Julio",
                8 => "Agosto",
                9 => "Septiembre",
                10 => "Octubre",
                11 => "Noviembre",
                12 => "Diciembre",
                _ => "Desconocido"
            };

            // Generación del documento interactivo con QuestPDF
            var pdfStream = new MemoryStream();
            QuestPDF.Fluent.Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(2, QuestPDF.Infrastructure.Unit.Centimetre);
                    page.Size(QuestPDF.Helpers.PageSizes.Letter);
                    page.DefaultTextStyle(x => x.FontFamily("Arial").FontSize(11));

                    page.Header().Column(col =>
                    {
                        col.Item().Text("SISTEMA DE GESTIÓN HUMANA SGTGH").Bold().FontSize(14).AlignCenter();
                        col.Item().Text($"COMPROBANTE DE PAGO DE NÓMINA - PERIODO {mesNombre.ToUpper()} {datos.PeriodoAnio}")
                            .FontSize(10).FontColor(QuestPDF.Helpers.Colors.Grey.Darken1).AlignCenter();
                        col.Item().PaddingTop(5).LineHorizontal(1).LineColor(QuestPDF.Helpers.Colors.Grey.Lighten2);
                    });

                    page.Content().PaddingTop(1, QuestPDF.Infrastructure.Unit.Centimetre).Column(col =>
                    {
                        // Información del empleado
                        col.Item().Text($"Colaborador: {datos.NombreCompleto}").SemiBold();
                        col.Item().Text($"Cargo: {datos.Cargo}");
                        col.Item().Text($"Fecha de Pago: {datos.FechaPago:dd/MM/yyyy}");

                        col.Item().PaddingTop(1, QuestPDF.Infrastructure.Unit.Centimetre);

                        // Tabla de valores
                        col.Item().Table(tabla =>
                        {
                            tabla.ColumnsDefinition(cols =>
                            {
                                cols.RelativeColumn(3);
                                cols.RelativeColumn(1);
                            });

                            // Encabezados
                            tabla.Cell().Background(QuestPDF.Helpers.Colors.Grey.Lighten2).Padding(5).Text("Concepto").Bold();
                            tabla.Cell().Background(QuestPDF.Helpers.Colors.Grey.Lighten2).Padding(5).Text("Valor").Bold().AlignRight();

                            // Filas
                            tabla.Cell().Padding(5).Text("Salario Base");
                            tabla.Cell().Padding(5).Text($"${datos.SalarioBase:N2}").AlignRight();

                            tabla.Cell().Padding(5).Text("Total Devengado (Extras/Bonos)");
                            tabla.Cell().Padding(5).Text($"${datos.TotalDevengado:N2}").AlignRight();

                            tabla.Cell().Padding(5).Text("Total Deducciones (Salud/Pensión)").FontColor(QuestPDF.Helpers.Colors.Red.Medium);
                            tabla.Cell().Padding(5).Text($"-${datos.TotalDeducciones:N2}").AlignRight().FontColor(QuestPDF.Helpers.Colors.Red.Medium);

                            // Total Neto
                            tabla.Cell().BorderTop(1).Padding(5).Text("NETO RECIBIDO EN BANCO").Bold();
                            tabla.Cell().BorderTop(1).Padding(5).Text($"${datos.NetoPagar:N2}").Bold().AlignRight().FontColor(QuestPDF.Helpers.Colors.Green.Darken1);
                        });
                    });

                    page.Footer().AlignCenter().Text(text =>
                    {
                        text.CurrentPageNumber().FontSize(10);
                        text.Span(" / ").FontSize(10);
                        text.TotalPages().FontSize(10);
                    });
                });
            }).GeneratePdf(pdfStream);

            pdfStream.Position = 0;
            return File(pdfStream, "application/pdf", $"Comprobante_Nomina_{mesNombre}_{datos.PeriodoAnio}.pdf");
        }

        // ==========================================
        // 4. GENERAR FORMATO DE SOLICITUD DE VACACIONES (PDF)
        // ==========================================
        [HttpGet("formato-vacaciones/{idEmpleado}")]
        public async Task<IActionResult> DescargarFormatoVacaciones(int idEmpleado)
        {
            // 1. Consultamos los datos del empleado directamente del contexto
            var usuario = await _context.Empleados.FirstOrDefaultAsync(u => u.IdEmpleado == idEmpleado);
            if (usuario == null) return NotFound("Empleado no encontrado");

            // 2. Lógica de cálculo de vacaciones (Reutilizando tu algoritmo de InforUsers)
            DateTime hoy = DateTime.Today;
            DateTime inicio = usuario.FechaContrato.ToDateTime(TimeOnly.MinValue);

            int tiempoAntiguedad = ((hoy.Year - inicio.Year) * 12) + hoy.Month - inicio.Month;
            if (hoy.Day < inicio.Day)
            {
                tiempoAntiguedad--;
            }

            int diasGanados = Math.Max(0, tiempoAntiguedad);
            int diasTomados = usuario.DiasTomados;
            int vacacionesDisponibles = Math.Max(0, diasGanados - diasTomados);

            // 3. Generación del PDF con QuestPDF
            var pdfStream = new MemoryStream();
            QuestPDF.Fluent.Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(2, QuestPDF.Infrastructure.Unit.Centimetre);
                    page.Size(QuestPDF.Helpers.PageSizes.Letter);
                    page.DefaultTextStyle(x => x.FontFamily("Arial").FontSize(11).LineHeight(1.3f));

                    // ENCABEZADO
                    page.Header().Column(col =>
                    {
                        col.Item().Text("SISTEMA DE GESTIÓN HUMANA SGTGH").Bold().FontSize(14).AlignCenter();
                        col.Item().Text("FORMATO DE SOLICITUD Y AUTORIZACIÓN DE VACACIONES")
                            .SemiBold().FontSize(11).FontColor(QuestPDF.Helpers.Colors.Grey.Darken2).AlignCenter();
                        col.Item().PaddingTop(5).LineHorizontal(1).LineColor(QuestPDF.Helpers.Colors.Grey.Lighten2);
                    });

                    // CONTENIDO
                    page.Content().PaddingTop(1, QuestPDF.Infrastructure.Unit.Centimetre).Column(col =>
                    {
                        col.Item().Text("1. DATOS GENERALES DEL SOLICITANTE").Bold().FontSize(12).FontColor(QuestPDF.Helpers.Colors.Blue.Darken3);

                        // Tabla de datos del empleado
                        col.Item().PaddingTop(5).Table(tabla =>
                        {
                            tabla.ColumnsDefinition(cols =>
                            {
                                cols.RelativeColumn(1);
                                cols.RelativeColumn(2);
                            });

                            tabla.Cell().Background(QuestPDF.Helpers.Colors.Grey.Lighten3).Padding(6).Text("Nombre Completo:").Bold();
                            tabla.Cell().Padding(6).Text($"{usuario.NombreEmpleado} {usuario.ApellidoEmpleado}");

                            tabla.Cell().Background(QuestPDF.Helpers.Colors.Grey.Lighten3).Padding(6).Text("Cargo:").Bold();
                            tabla.Cell().Padding(6).Text(usuario.Cargo);

                            tabla.Cell().Background(QuestPDF.Helpers.Colors.Grey.Lighten3).Padding(6).Text("Fecha de Contratación:").Bold();
                            tabla.Cell().Padding(6).Text($"{usuario.FechaContrato:dd/MM/yyyy}");

                            tabla.Cell().Background(QuestPDF.Helpers.Colors.Grey.Lighten3).Padding(6).Text("Fecha de Solicitud:").Bold();
                            tabla.Cell().Padding(6).Text($"{DateTime.Today:dd/MM/yyyy} (Hoy)");
                        });

                        col.Item().PaddingTop(1.5f, QuestPDF.Infrastructure.Unit.Centimetre);
                        col.Item().Text("2. ESTADO DE VACACIONES (CONTROL INTERNO)").Bold().FontSize(12).FontColor(QuestPDF.Helpers.Colors.Blue.Darken3);

                        // Tabla de control de días devueltos por el sistema
                        col.Item().PaddingTop(5).Table(tabla =>
                        {
                            tabla.ColumnsDefinition(cols =>
                            {
                                cols.RelativeColumn(2);
                                cols.RelativeColumn(1);
                            });

                            tabla.Cell().Padding(6).Text("Días Totales Ganados por Antigüedad:");
                            tabla.Cell().Padding(6).Text($"{diasGanados} días").AlignRight();

                            tabla.Cell().Padding(6).Text("Días Disfrutados a la Fecha:");
                            tabla.Cell().Padding(6).Text($"{diasTomados} días").AlignRight();

                            tabla.Cell().Background(QuestPDF.Helpers.Colors.Blue.Lighten5).Padding(6).Text("Saldo de Vacaciones Disponibles:").Bold();
                            tabla.Cell().Background(QuestPDF.Helpers.Colors.Blue.Lighten5).Padding(6).Text($"{vacacionesDisponibles} días").Bold().AlignRight().FontColor(QuestPDF.Helpers.Colors.Blue.Darken3);
                        });

                        // Sección para llenar fechas
                        col.Item().PaddingTop(1.5f, QuestPDF.Infrastructure.Unit.Centimetre);
                        col.Item().Text("3. PERIODO A DISFRUTAR (A LLENAR POR EL JEFE INMEDIATO)").Bold().FontSize(12).FontColor(QuestPDF.Helpers.Colors.Blue.Darken3);

                        col.Item().PaddingTop(8).Text("Fecha de Inicio (Primer día de descanso):  ______ / ______ / 2026");
                        col.Item().PaddingTop(6).Text("Fecha de Reincorporación Laboral:  ______ / ______ / 2026");
                        col.Item().PaddingTop(6).Text("Total de Días Hábiles Autorizados:  ___________ Días");

                        // SECCIÓN DE FIRMAS
                        col.Item().PaddingTop(3, QuestPDF.Infrastructure.Unit.Centimetre);

                        col.Item().Table(tabla =>
                        {
                            tabla.ColumnsDefinition(cols =>
                            {
                                cols.RelativeColumn(1);
                                cols.ConstantColumn(40); // espacio en blanco en el medio
                                cols.RelativeColumn(1);
                            });

                            tabla.Cell()
                                .BorderTop(1)
                                .PaddingTop(5)
                                .AlignCenter()
                                .Text("Firma del Colaborador");

                            tabla.Cell(); // columna vacía (espaciador)

                            tabla.Cell()
                                .BorderTop(1)
                                .PaddingTop(5)
                                .AlignCenter()
                                .Text("Firma Jefe Inmediato / Empresa");
                        });
                    });

                    // PIE DE PÁGINA
                    page.Footer().AlignCenter().Text(text =>
                    {
                        text.CurrentPageNumber().FontSize(9).FontColor(QuestPDF.Helpers.Colors.Grey.Medium);
                        text.Span(" / ").FontSize(9).FontColor(QuestPDF.Helpers.Colors.Grey.Medium);
                        text.TotalPages().FontSize(9).FontColor(QuestPDF.Helpers.Colors.Grey.Medium);
                    });
                });
            }).GeneratePdf(pdfStream);

            pdfStream.Position = 0;
            return File(pdfStream, "application/pdf", $"Solicitud_Vacaciones_{usuario.NombreEmpleado}.pdf");
        }
    }
}
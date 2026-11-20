using System.Net;

namespace Application.Services
{
    /// <summary>
    /// HTML del email de invitación. Está escrito para clientes de correo (Gmail, Outlook...):
    /// todo con tablas y estilos inline, sin CSS externo ni imágenes. El logo se dibuja con
    /// celdas de tabla porque Gmail no muestra SVG y un PNG exigiría una URL pública.
    /// </summary>
    public static class InvitacionEmailTemplate
    {
        // Paleta de la marca
        private const string Tinta = "#1B2430";
        private const string Azul = "#1A5DAB";
        private const string AzulSuave = "#EAF1FB";
        private const string Naranja = "#D9703F";
        private const string Verde = "#0E8A7E";
        private const string Fuente = "Arial, Helvetica, sans-serif";

        public static string Construir(
            string nombreInvitador,
            string nombreViaje,
            string emailInvitado,
            DateTime fechaExpiracion,
            string link,
            DateTime? fechaInicio = null,
            DateTime? fechaFin = null)
        {
            var invitador = WebUtility.HtmlEncode(nombreInvitador);
            var viaje = WebUtility.HtmlEncode(nombreViaje);
            var email = WebUtility.HtmlEncode(emailInvitado);
            var vence = fechaExpiracion.ToString("dd/MM/yyyy");

            var fechas = fechaInicio.HasValue
                ? fechaFin.HasValue && fechaFin.Value.Date != fechaInicio.Value.Date
                    ? $"{fechaInicio:dd/MM/yyyy} al {fechaFin:dd/MM/yyyy}"
                    : $"{fechaInicio:dd/MM/yyyy}"
                : null;
            var filaFechas = fechas == null
                ? ""
                : $"""<p style="margin:6px 0 0;font-family:{Fuente};font-size:14px;color:#5B6B80;">📅 {fechas}</p>""";

            return $"""
<!DOCTYPE html>
<html lang="es">
<head>
<meta charset="utf-8">
<meta name="viewport" content="width=device-width, initial-scale=1">
<title>Invitación a {viaje}</title>
</head>
<body style="margin:0;padding:0;background:#F1F4F9;">
<table role="presentation" width="100%" cellpadding="0" cellspacing="0" border="0" bgcolor="#F1F4F9">
<tr><td align="center" style="padding:32px 12px;">

  <table role="presentation" width="600" cellpadding="0" cellspacing="0" border="0" style="width:100%;max-width:600px;background:#FFFFFF;border-radius:16px;overflow:hidden;">

    <!-- franja de marca -->
    <tr><td style="font-size:0;line-height:0;">
      <table role="presentation" width="100%" cellpadding="0" cellspacing="0" border="0"><tr>
        <td width="60%" height="6" bgcolor="{Azul}" style="font-size:0;line-height:0;">&nbsp;</td>
        <td width="22%" height="6" bgcolor="{Naranja}" style="font-size:0;line-height:0;">&nbsp;</td>
        <td width="18%" height="6" bgcolor="{Verde}" style="font-size:0;line-height:0;">&nbsp;</td>
      </tr></table>
    </td></tr>

    <!-- logo -->
    <tr><td style="padding:30px 36px 8px;">
      <table role="presentation" cellpadding="0" cellspacing="0" border="0"><tr>
        <td valign="middle">{Marca()}</td>
        <td valign="middle" style="padding-left:14px;font-family:{Fuente};font-size:30px;font-weight:700;letter-spacing:-0.5px;color:{Tinta};">Entre-<span style="color:{Azul};">Todos</span></td>
      </tr></table>
    </td></tr>

    <!-- titulo -->
    <tr><td style="padding:22px 36px 0;">
      <h1 style="margin:0;font-family:{Fuente};font-size:26px;line-height:1.25;font-weight:700;color:{Tinta};">¡Te invitaron a un viaje!</h1>
      <p style="margin:12px 0 0;font-family:{Fuente};font-size:16px;line-height:1.55;color:#44546A;"><strong style="color:{Tinta};">{invitador}</strong> quiere que te sumes para dividir los gastos entre todos, sin planillas ni estrés.</p>
    </td></tr>

    <!-- tarjeta del viaje -->
    <tr><td style="padding:22px 36px 0;">
      <table role="presentation" width="100%" cellpadding="0" cellspacing="0" border="0" bgcolor="{AzulSuave}" style="border-radius:12px;border-left:5px solid {Azul};">
        <tr><td style="padding:18px 22px;">
          <p style="margin:0;font-family:{Fuente};font-size:11px;font-weight:700;letter-spacing:1.5px;color:{Azul};">VIAJE</p>
          <p style="margin:4px 0 0;font-family:{Fuente};font-size:22px;font-weight:700;color:{Tinta};">{viaje}</p>
          {filaFechas}
        </td></tr>
      </table>
    </td></tr>

    <!-- boton -->
    <tr><td align="center" style="padding:28px 36px 6px;">
      <table role="presentation" cellpadding="0" cellspacing="0" border="0"><tr>
        <td align="center" bgcolor="{Azul}" style="border-radius:10px;">
          <a href="{link}" style="display:inline-block;padding:15px 34px;font-family:{Fuente};font-size:16px;font-weight:700;color:#FFFFFF;text-decoration:none;border-radius:10px;">Ver mi invitación &rarr;</a>
        </td>
      </tr></table>
    </td></tr>

    <!-- pasos -->
    <tr><td style="padding:26px 36px 0;">
      <p style="margin:0 0 10px;font-family:{Fuente};font-size:14px;font-weight:700;color:{Tinta};">Cómo sumarte</p>
      <table role="presentation" width="100%" cellpadding="0" cellspacing="0" border="0">
        {Paso("1", Azul, $"Ingresá o creá tu cuenta con este mismo email: <strong>{email}</strong>.")}
        {Paso("2", Naranja, "Entrá a la sección <strong>Invitaciones</strong>.")}
        {Paso("3", Verde, "Aceptá la invitación y listo: el viaje aparece en <strong>Mis viajes</strong>.")}
      </table>
    </td></tr>

    <!-- vencimiento -->
    <tr><td style="padding:22px 36px 30px;">
      <p style="margin:0;font-family:{Fuente};font-size:13px;line-height:1.5;color:#7A889B;">⏳ Esta invitación vence el <strong>{vence}</strong>. Si no esperabas este mensaje, podés ignorarlo.</p>
    </td></tr>

    <!-- pie -->
    <tr><td bgcolor="{Tinta}" align="center" style="padding:20px 36px;">
      <p style="margin:0;font-family:{Fuente};font-size:14px;font-weight:700;color:#FFFFFF;">Viaja libre, divide fácil, vive entre todos.</p>
      <p style="margin:6px 0 0;font-family:{Fuente};font-size:12px;color:#9FB0C6;">© {DateTime.UtcNow.Year} Entre-Todos</p>
    </td></tr>

  </table>

</td></tr>
</table>
</body>
</html>
""";
        }

        private static string Paso(string numero, string color, string texto) => $"""
<tr>
  <td width="34" valign="top" style="padding:0 0 10px;">
    <table role="presentation" cellpadding="0" cellspacing="0" border="0"><tr>
      <td width="24" height="24" align="center" valign="middle" bgcolor="{color}" style="border-radius:12px;font-family:{Fuente};font-size:13px;font-weight:700;color:#FFFFFF;">{numero}</td>
    </tr></table>
  </td>
  <td valign="top" style="padding:3px 0 10px;font-family:{Fuente};font-size:14px;line-height:1.5;color:#44546A;">{texto}</td>
</tr>
""";

        // Monograma "ET" del logo, dibujado con celdas de tabla (50 x 45 px).
        // Columnas: tallo E | barras E | punta E | barra T izq | tallo T | barra T der.
        private static string Marca()
        {
            string Vacia(int w, int h) => $"""<td width="{w}" height="{h}" style="font-size:0;line-height:0;">&nbsp;</td>""";
            string Llena(int w, int h, string color) => $"""<td width="{w}" height="{h}" bgcolor="{color}" style="font-size:0;line-height:0;">&nbsp;</td>""";
            string Punto(string color) => $"""<td width="8" height="7" style="font-size:0;line-height:0;"><div style="width:8px;height:7px;border-radius:4px;background:{color};font-size:0;line-height:0;">&nbsp;</div></td>""";
            string Linea(int w, string? fondo) =>
                $"""<td width="{w}" height="7" valign="middle" {(fondo == null ? "" : $"bgcolor=\"{fondo}\"")} style="font-size:0;line-height:0;"><div style="height:2px;background:{Naranja};font-size:0;line-height:2px;">&nbsp;</div></td>""";

            return $"""
<table role="presentation" cellpadding="0" cellspacing="0" border="0" style="border-collapse:collapse;">
<tr>{Llena(8, 7, Tinta)}{Llena(12, 7, Tinta)}{Llena(6, 7, Tinta)}{Llena(8, 7, Azul)}{Llena(8, 7, Azul)}{Llena(8, 7, Azul)}</tr>
<tr>{Llena(8, 12, Tinta)}{Vacia(12, 12)}{Vacia(6, 12)}{Vacia(8, 12)}{Llena(8, 12, Azul)}{Vacia(8, 12)}</tr>
<tr>{Llena(8, 7, Tinta)}{Llena(12, 7, Tinta)}{Linea(6, null)}{Punto(Naranja)}{Linea(8, Azul)}{Punto(Verde)}</tr>
<tr>{Llena(8, 12, Tinta)}{Vacia(12, 12)}{Vacia(6, 12)}{Vacia(8, 12)}{Llena(8, 12, Azul)}{Vacia(8, 12)}</tr>
<tr>{Llena(8, 7, Tinta)}{Llena(12, 7, Tinta)}{Llena(6, 7, Tinta)}{Vacia(8, 7)}{Llena(8, 7, Azul)}{Vacia(8, 7)}</tr>
</table>
""";
        }
    }
}

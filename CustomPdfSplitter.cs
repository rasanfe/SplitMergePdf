using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitMergePdf
{
    /// <summary>
    /// Versión propia de <see cref="PdfSplitter"/> (la clase de iText que parte un
    /// PDF en varios). iText nos delega UNA sola decisión: dónde escribir cada
    /// trozo. Para eso hay que heredar y sobreescribir <see cref="GetNextPdfWriter"/>;
    /// no se puede pasar el patrón de nombres por parámetro, de ahí esta clase.
    /// </summary>
    internal class CustomPdfSplitter : PdfSplitter
    {
        private int _order;                          // contador para numerar los ficheros de salida
        private readonly string _destinationFolder;  // ruta base (carpeta + nombre sin extensión)

        public CustomPdfSplitter(PdfDocument pdfDocument, string destinationFolder) : base(pdfDocument)
        {
            _destinationFolder = destinationFolder;
            _order = 1;
        }

        // iText llama a este método una vez por cada trozo. Devolvemos el writer
        // que apunta al fichero destino, numerándolos: nombre_1.pdf, nombre_2.pdf...
        // (_order++ devuelve el valor actual y luego incrementa).
        protected override PdfWriter GetNextPdfWriter(PageRange documentPageRange)
        {
            return new PdfWriter(_destinationFolder + "_" + _order++ + ".pdf");

        }
    }
}

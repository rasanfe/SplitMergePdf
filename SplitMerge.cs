using iText.Forms;
using iText.Forms.Fields;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Pdfa;
using iText.Forms.Util;
using static iText.IO.Codec.TiffWriter;
using iText.Kernel.XMP.Impl;
using iText.Signatures;
using System.Xml.Linq;
using static System.Net.WebRequestMethods;
using System.Reflection.PortableExecutable;
using System.Collections.ObjectModel;
using System.Text;
using iText.Kernel.Exceptions;
using System.Linq.Expressions;

namespace SplitMergePdf
{
    /// <summary>
    /// Divide (split) y une (merge) ficheros PDF con iText7.
    ///
    /// Consumo desde PowerBuilder: a diferencia de los ejemplos de formularios,
    /// aquí los métodos devuelven un <see cref="bool"/>/<see cref="int"/> y, si
    /// algo falla, el motivo queda en la propiedad <see cref="errorText"/>. Tras
    /// llamar a un método, si os devuelve false (o 0 páginas) consultáis
    /// <c>errorText</c> para saber qué pasó.
    /// </summary>
    public class SplitMerge

    {
        /// <summary>Último mensaje de error. Queda en null si la operación fue bien.</summary>
        public string? errorText { set; get; }

        /// <summary>
        /// Une varios PDFs en uno solo (<paramref name="targetPdf"/>). Antes de
        /// copiar cada fichero le quita la firma digital (la "aplana") y se salta
        /// los protegidos por contraseña.
        /// </summary>
        /// <param name="fileNames">Rutas de los PDFs a unir, en orden.</param>
        /// <param name="targetPdf">Ruta del PDF combinado de salida.</param>
        /// <returns>true si se completó; false si hubo error (ver <see cref="errorText"/>).</returns>
        public bool MergeFiles(string[] fileNames, string targetPdf)
        {
            bool merged = true;
            try
            {
                // SetSmartMode(true): iText reutiliza objetos repetidos (fuentes,
                // imágenes...) entre documentos en vez de duplicarlos. El PDF unido
                // pesa bastante menos cuando los originales comparten recursos.
                PdfWriter writer = new PdfWriter(targetPdf).SetSmartMode(true);
                PdfDocument pdfDoc = new PdfDocument(writer);

                int numDoc = 0;
                foreach (string file in fileNames)
                {

                    bool isPasswordProtected = IsPasswordProtected(file); //----->Falta comprobar funcion

                    if (isPasswordProtected)
                    {
                        continue;
                    }
                    else
                    {
                        numDoc++;
                        RemoveSign(file);   //----->Me gusta como queda, Quita la firmas y si hay imagen se quedan.
                        //RenameFields(file, numDoc); //Cambia el nombre de las firmas para que no se pierda la imagen, pero se queda la firma rota.

                        PdfReader reader = new PdfReader(file).SetUnethicalReading(true);
                        PdfDocument srcDoc = new PdfDocument(reader);

                        // CopyPagesTo vuelca todas las páginas del documento origen
                        // (de la 1 a la última) al documento destino que vamos uniendo.
                        srcDoc.CopyPagesTo(1, srcDoc.GetNumberOfPages(), pdfDoc);
                        srcDoc.Close();
                    }

                }
                pdfDoc.Close();
                writer.Close();
            }
            catch (Exception ex)
            {
                errorText = ex.Message;
                merged = false;
            }
            return merged;
        }

        /// <summary>
        /// Divide un PDF en varios, <b>una página por fichero</b>. Cada trozo se
        /// guarda en <paramref name="outputPath"/> con el nombre del original más
        /// un sufijo numérico (lo pone <see cref="CustomPdfSplitter"/>).
        /// </summary>
        /// <param name="inputFile">PDF a dividir.</param>
        /// <param name="outputPath">Carpeta donde dejar los trozos.</param>
        /// <returns>Número de páginas/ficheros generados (0 si hubo error, ver <see cref="errorText"/>).</returns>
        public int SplitFiles(string inputFile, string outputPath)
        {
            int numberOfPages = 0;
            try
            {

                string fileNameWithOutExtension = FileService.GetFileNameWithoutExtension(inputFile);

                PdfReader reader = new PdfReader(inputFile).SetUnethicalReading(true);
                PdfDocument document = new PdfDocument(reader);

                // PdfSplitter (clase base de iText) es quien parte el documento; nuestra
                // CustomPdfSplitter solo decide el nombre de cada fichero de salida.
                CustomPdfSplitter splitter = new CustomPdfSplitter(document, outputPath + "\\" + fileNameWithOutExtension);
                // SplitByPageCount(1): un documento nuevo por cada página.
                var splittedDocs = splitter.SplitByPageCount(1);

                // iText devuelve los trozos abiertos: hay que cerrar cada uno para que
                // se escriba a disco. De paso contamos cuántos salieron.
                foreach (var splittedDoc in splittedDocs)
                {
                    splittedDoc.Close();
                    numberOfPages++;
                }

            }
            catch (Exception ex)
            {
                errorText = ex.Message;
            }
            return numberOfPages;
        }

        /// <summary>
        /// Quita la firma digital del PDF aplanando <b>solo</b> los campos de firma
        /// (/Sig). Trabaja sobre un fichero temporal "_unsing.pdf" y luego lo renombra
        /// encima del original. Truco: al aplanar, la firma deja de ser válida pero su
        /// representación visual (la imagen del sello, si tenía) se conserva en la
        /// página. El resto de campos del formulario quedan intactos e interactivos.
        /// </summary>
        internal void RemoveSign(string inputFile)
        {
            try
            {
                PdfReader reader = new PdfReader(inputFile).SetUnethicalReading(true);

                string fileNameWithOutExtension = FileService.GetFileNameWithoutExtension(inputFile);
                string outputPath = FileService.GetDirectoryName(inputFile);
                string outputFile = outputPath + "\\" + fileNameWithOutExtension + "_unsing.pdf";

                PdfWriter writer = new PdfWriter(outputFile).SetSmartMode(true);

                PdfDocument pdfDoc = new PdfDocument(reader, writer);
                PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);

                // Marcamos para aplanado SOLO los campos cuyo tipo es /Sig (firmas).
                // FlattenFields con campos marcados via PartialFormFlattening aplana
                // únicamente esos: la imagen del sello se "quema" en la página y el campo
                // de firma desaparece, mientras los demás campos siguen interactivos.
                bool anySig = false;
                foreach (KeyValuePair<string, PdfFormField> entry in form.GetAllFormFields())
                {
                    if (PdfName.Sig.Equals(entry.Value.GetFormType()))
                    {
                        form.PartialFormFlattening(entry.Key);
                        anySig = true;
                    }
                }
                if (anySig) form.FlattenFields();
                pdfDoc.Close();
                reader.Close();
                writer.Close();
                // Sustituimos el original por el temporal sin firma.
                FileService.FileRename(outputFile, inputFile);
            }
            catch (Exception ex)
            {
                errorText = ex.Message;
            }
        }


        /// <summary>
        /// Comprueba si el PDF está protegido por contraseña. Lo hace "a la brava":
        /// intenta abrirlo sin clave y, si iText lanza <see cref="BadPasswordException"/>,
        /// es que sí lo está. Fijaos que aquí NO usamos SetUnethicalReading, porque
        /// lo que queremos es justo detectar el cifrado real.
        /// </summary>
        internal bool IsPasswordProtected(string inputFile)
        {
            PdfReader reader = new PdfReader(inputFile);
            try
            {
                PdfDocument pdfDoc = new PdfDocument(reader);
                reader.Close();
                return false;
            }
            catch (BadPasswordException)
            {
                return true;
            }
        }

        /// <summary>
        /// Renombra todos los campos del formulario añadiéndoles un sufijo numérico
        /// (<paramref name="numDoc"/>). Alternativa a <see cref="RemoveSign"/> para
        /// el merge: evita colisiones de nombres entre documentos, pero deja la
        /// firma "rota" en pantalla. Por eso en MergeFiles usamos RemoveSign.
        /// </summary>
        internal void RenameFields(string inputFile, int numDoc)
        {
            try
            {
                PdfReader reader = new PdfReader(inputFile).SetUnethicalReading(true);

                string fileNameWithoutExtension = FileService.GetFileNameWithoutExtension(inputFile);
                string outputPath = FileService.GetDirectoryName(inputFile);
                string outputFile = Path.Combine(outputPath, fileNameWithoutExtension + "_renamed.pdf");

                PdfWriter writer = new PdfWriter(outputFile).SetSmartMode(true);

                using (PdfDocument pdfDoc = new PdfDocument(reader, writer))
                {
                    PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
                    IDictionary<string, PdfFormField> fields = form.GetAllFormFields();

                    foreach (var name in fields.Keys.ToList())
                    {
                        form.RenameField(name, $"{name}{numDoc}");
                    }
                }

                FileService.FileRename(outputFile, inputFile);
            }
            catch (Exception ex)
            {
                errorText = ex.Message;
            }
        }




    }
}

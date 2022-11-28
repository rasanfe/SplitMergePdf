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
    public class SplitMerge

    {
        public string? errorText { set; get; }
        public bool MergeFiles(string[] fileNames, string targetPdf)
        {
            bool merged = true;
            try
            {
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

        public int SplitFiles(string inputFile, string outputPath)
        {
            int numberOfPages = 0;
            try
            {

                string fileNameWithOutExtension = FileService.GetFileNameWithoutExtension(inputFile);

                PdfReader reader = new PdfReader(inputFile).SetUnethicalReading(true);
                PdfDocument document = new PdfDocument(reader);

                CustomPdfSplitter splitter = new CustomPdfSplitter(document, outputPath + "\\" + fileNameWithOutExtension);
                var splittedDocs = splitter.SplitByPageCount(1);

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

                form = PdfAcroForm.GetAcroForm(pdfDoc, true);

                form.FlattenFields();
                pdfDoc.Close();
                reader.Close();
                writer.Close();
                FileService.FileRename(outputFile, inputFile);
            }
            catch (Exception ex)
            {
                errorText = ex.Message;
            }
        }


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
        internal void RenameFields(string inputFile, int numDoc)
        {
            try
            {
                PdfReader reader = new PdfReader(inputFile).SetUnethicalReading(true);

                string fileNameWithOutExtension = FileService.GetFileNameWithoutExtension(inputFile);
                string outputPath = FileService.GetDirectoryName(inputFile);
                string outputFile = outputPath + "\\" + fileNameWithOutExtension + "_renamed.pdf";

                PdfWriter writer = new PdfWriter(outputFile).SetSmartMode(true);

                PdfDocument pdfDoc = new PdfDocument(reader, writer);
                PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);

                form = PdfAcroForm.GetAcroForm(pdfDoc, true);

                IDictionary<String, PdfFormField> fields = form.GetFormFields();

                ICollection<string> keys = new Collection<string>();
                foreach (string name in fields.Keys)
                {
                    keys.Add(name);
                }


                // See the renamed field in the console
                foreach (String name in keys)
                {
                    form.RenameField(name, String.Format("{0}{1}", name, numDoc));
                }

                pdfDoc.Close();
                reader.Close();
                writer.Close();
                FileService.FileRename(outputFile, inputFile);
            }
            catch (Exception ex)
            {
                errorText = ex.Message;
            }
        }

    }
}

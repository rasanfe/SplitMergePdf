using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitMergePdf
{
    internal class CustomPdfSplitter : PdfSplitter
    {
        private int _order;
        private readonly string _destinationFolder;

        public CustomPdfSplitter(PdfDocument pdfDocument, string destinationFolder) : base(pdfDocument)
        {
            _destinationFolder = destinationFolder;
            _order = 1;
        }

        protected override PdfWriter GetNextPdfWriter(PageRange documentPageRange)
        {
           return new PdfWriter(_destinationFolder+"_" + _order++ + ".pdf");

        }
    }
}

using System;
using System.Reflection;
using SplitMergePdf;


    
Console.WriteLine("Probamos el metodo: public bool MergeFiles(string[] fileNames, string targetPdf)");

SplitMerge lnpdf = new SplitMerge();

string[] filenames = new string[5];

filenames[0] = "C:\\proyecto pw2022\\Blog\\NET 6\\SplitMergePdf\\TestSplitMergePdf\\TestSource\\prueba_pagina1_para_join.pdf";
filenames[1] = "C:\\proyecto pw2022\\Blog\\NET 6\\SplitMergePdf\\TestSplitMergePdf\\TestSource\\proteger.pdf";
filenames[2] = "C:\\proyecto pw2022\\Blog\\NET 6\\SplitMergePdf\\TestSplitMergePdf\\TestSource\\prueba_pagina2_para_join.pdf";
filenames[3] = "C:\\proyecto pw2022\\Blog\\NET 6\\SplitMergePdf\\TestSplitMergePdf\\TestSource\\prueba_pagina1_para_join_sign.pdf";
filenames[4] = "C:\\proyecto pw2022\\Blog\\NET 6\\SplitMergePdf\\TestSplitMergePdf\\TestSource\\prueba_pagina2_para_join_sign.pdf";

string targetPdf = "C:\\proyecto pw2022\\Blog\\NET 6\\SplitMergePdf\\TestSplitMergePdf\\TestResult\\MergeTestResult.pdf";

if (File.Exists(targetPdf))
{
    File.Delete(targetPdf);
}

bool result = lnpdf.MergeFiles(filenames, targetPdf);

if (result)
{
    Console.WriteLine("Resultado OK. Archivo MergeTestResult.pdf Creado con éxito.");
}
else
{
    Console.WriteLine("Error ejecutanto test MergeFiles.");

}

Console.WriteLine("");
Console.WriteLine("");
Console.WriteLine("Probamos el metodo: public int SplitFiles(string inputFile, string outputPath)");

string inputFile = "C:\\proyecto pw2022\\Blog\\NET 6\\SplitMergePdf\\TestSplitMergePdf\\TestSource\\prueba_4_paginas_para_split.pdf";
string outputPath = "C:\\proyecto pw2022\\Blog\\NET 6\\SplitMergePdf\\TestSplitMergePdf\\TestResult";

int numPages = lnpdf.SplitFiles(inputFile, outputPath);

if (numPages > 0)
{
    Console.WriteLine("Resultado OK. Se han creado {0} archivos con éxito.", numPages);
}
else
{
    Console.WriteLine("Error ejecutanto test SplitFiles.");
}
Console.WriteLine("");
Console.WriteLine("");

Console.WriteLine("¿ Desea eliminar los archivos generados ? S/N");
string borrar = Console.ReadLine();



if (borrar.ToUpper().Equals("S"))
{
    List<string> strFiles = Directory.GetFiles(outputPath, "*.pdf", SearchOption.TopDirectoryOnly).ToList();
    foreach (string fichero in strFiles)
    {
        File.Delete(fichero);
    }
}

System.Diagnostics.Process.Start("explorer", outputPath);


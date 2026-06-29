using System;
using System.IO;

namespace SplitMergePdf
{
    /// <summary>
    /// Envoltorio sobre <see cref="System.IO.Path"/> con validación previa de la
    /// entrada (nulo/vacío y barra final). Centraliza el manejo de rutas para que
    /// el resto del código no repita comprobaciones. Es <c>internal static</c>:
    /// utilidad interna de la librería, no se expone a PowerBuilder.
    /// </summary>
    internal static class FileService
    {
        private static string errorText = "";

        /// <summary>Devuelve el nombre del fichero con extensión (p. ej. "doc.pdf").</summary>
        public static string GetFilename(string fileInput)
        {

            if (string.IsNullOrEmpty(fileInput))
            {
                errorText = "Input File cannot be null or Empty";
                throw new ArgumentNullException(paramName: nameof(fileInput), message: errorText);
            }

            if (EndsInDirectorySeparator(fileInput))
            {
                fileInput = Path.TrimEndingDirectorySeparator(fileInput);
            }

            try
            {
                string fileOut = Path.GetFileName(fileInput);
                return fileOut;
            }
            catch (Exception ex)
            {
                errorText = ex.Message;
                return System.String.Empty;
            }
        }

        /// <summary>Devuelve la extensión del fichero, incluido el punto (p. ej. ".pdf").</summary>
        public static string GetExtension(string fileInput)
        {

            if (string.IsNullOrEmpty(fileInput))
            {
                errorText = "Input File cannot be null or Empty";
                throw new ArgumentNullException(paramName: nameof(fileInput), message: errorText);
            }

            if (EndsInDirectorySeparator(fileInput))
            {
                fileInput = Path.TrimEndingDirectorySeparator(fileInput);
            }


            try
            {
                string fileOut = Path.GetExtension(fileInput);
                return fileOut;
            }
            catch (Exception ex)
            {
                errorText = ex.Message;
                return System.String.Empty;
            }
        }

        /// <summary>Devuelve el nombre del fichero sin extensión (p. ej. "doc"). Lo usa el split para nombrar los trozos.</summary>
        public static string GetFileNameWithoutExtension(string fileInput)
        {
            if (EndsInDirectorySeparator(fileInput))
            {
                fileInput = Path.TrimEndingDirectorySeparator(fileInput);
            }


            if (string.IsNullOrEmpty(fileInput))
            {
                errorText = "Input File cannot be null or Empty";
                throw new ArgumentNullException(paramName: nameof(fileInput), message: errorText);
            }

            try
            {
                string fileOut = Path.GetFileNameWithoutExtension(fileInput);
                return fileOut;
            }
            catch (Exception ex)
            {
                errorText = ex.Message;
                return System.String.Empty;
            }
        }

        /// <summary>Devuelve la ruta con la extensión cambiada por <paramref name="extension"/>.</summary>
        public static string ChangeExtension(string fileInput, string extension)
        {

            if (string.IsNullOrEmpty(fileInput))
            {
                errorText = "Input File cannot be null or Empty";
                throw new ArgumentNullException(paramName: nameof(fileInput), message: errorText);
            }

            if (EndsInDirectorySeparator(fileInput))
            {
                fileInput = Path.TrimEndingDirectorySeparator(fileInput);
            }

            try
            {
                string fileOut = Path.ChangeExtension(fileInput, extension);
                return fileOut;
            }
            catch (Exception ex)
            {
                errorText = ex.Message;
                return System.String.Empty;
            }
        }

        /// <summary>Devuelve la carpeta que contiene el fichero (cadena vacía si no hay).</summary>
        public static string GetDirectoryName(string fileInput)
        {

            if (string.IsNullOrEmpty(fileInput))
            {
                errorText = "Input File cannot be null or Empty";
                throw new ArgumentNullException(paramName: nameof(fileInput), message: errorText);
            }

            if (EndsInDirectorySeparator(fileInput))
            {
                fileInput = Path.TrimEndingDirectorySeparator(fileInput);
            }


            try
            {
                string? fileOut = Path.GetDirectoryName(fileInput);
                return fileOut ?? string.Empty;
            }
            catch (Exception ex)
            {
                errorText = ex.Message;
                return System.String.Empty;
            }
        }

        /// <summary>Indica si la ruta termina en barra ("\" o "/"). Se usa para limpiarla antes de partirla.</summary>
        public static bool EndsInDirectorySeparator(string fileInput)
        {

            if (string.IsNullOrEmpty(fileInput))
            {
                errorText = "Input File cannot be null or Empty";
                throw new ArgumentNullException(paramName: nameof(fileInput), message: errorText);
            }

            bool isEndsInDirectorySeparator = Path.EndsInDirectorySeparator(fileInput);

            return isEndsInDirectorySeparator;
        }

        /// <summary>
        /// Mueve/renombra <paramref name="fileInput"/> a <paramref name="newFile"/>,
        /// borrando antes el destino si ya existía (File.Move no sobreescribe). Lo
        /// usamos para dejar el fichero temporal encima del original.
        /// </summary>
        public static bool FileRename(string fileInput, string newFile)
        {

            if (string.IsNullOrEmpty(fileInput))
            {
                errorText = "Input File cannot be null or Empty";
                throw new ArgumentNullException(paramName: nameof(fileInput), message: errorText);
            }

            if (!File.Exists(fileInput))
            {
                errorText = "Input File not Exists";
                throw new ArgumentException(paramName: nameof(fileInput), message: errorText);
            }

            if (string.IsNullOrEmpty(newFile))
            {
                errorText = "New File Name cannot be null or Empty";
                throw new ArgumentNullException(paramName: nameof(fileInput), message: errorText);
            }

            try
            {
                if (File.Exists(newFile))
                {
                    File.Delete(newFile);
                }

                File.Move(fileInput, newFile);
                return true;
            }
            catch (Exception ex)
            {
                errorText = ex.Message;
                return false;
            }

        }

    }
}
